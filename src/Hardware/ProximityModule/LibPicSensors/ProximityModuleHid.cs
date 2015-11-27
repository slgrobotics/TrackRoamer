using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;
using System.Windows.Forms;
using System.Threading;

using Microsoft.Win32.SafeHandles;
using System.Globalization;
using System.Runtime.InteropServices;

using TrackRoamer.Robotics.Utility.LibSystem;
using TrackRoamer.Robotics.Utility.LibLvrGenericHid;

namespace TrackRoamer.Robotics.Utility.LibPicSensors
{
    public class ProximityModuleEventArgs : System.EventArgs
    {
        public string description;
    }

	public partial class ProximityModule
	{
        // see http://msdn.microsoft.com/en-us/magazine/cc163417.aspx
        private static object _lock = new object();
        private static MessageWindow _window;
        public static IntPtr WindowHandle;

        internal class MessageWindow : Form
        {
            internal ProximityModule picpxmod;

            protected override void WndProc(ref Message m)
            {
                try
                {
                    //  The OnDeviceChange routine processes WM_DEVICECHANGE messages.

                    if (m.Msg == DeviceManagement.WM_DEVICECHANGE)
                    {
                        picpxmod.OnDeviceChange(m);
                    }

                    //  Let the base form process the message.

                    base.WndProc(ref m);
                }
                catch (Exception ex)
                {
                    Tracer.Error(ex);
                    throw;
                }
            }

        }

        /// <summary>
        /// we need this to catch device change events in a non-Winforms environment, which we have in MRDS DSS host.
        /// </summary>
        private void EnsureEventsWindowInitialized()
        {
            lock (_lock)
            {
                if (_window == null)
                {
                    using (ManualResetEvent mre =
                          new ManualResetEvent(false))
                    {
                        Thread t = new Thread((ThreadStart)delegate
                        {
                            _window = new MessageWindow();
                            _window.picpxmod = this;
                            WindowHandle = _window.Handle;
                            mre.Set();
                            Application.Run();
                        });
                        t.Name = "MessageEvents";
                        t.IsBackground = true;
                        t.Start();

                        mre.WaitOne();
                    }
                }
            }
        }

        public event EventHandler<ProximityModuleEventArgs> DeviceAttachedEvent;

        // Invoke the DeviceAttachedEvent event; called whenever the device is attached:
        protected virtual void OnDeviceAttached(ProximityModuleEventArgs e)
        {
            if (DeviceAttachedEvent != null)
            {
                DeviceAttachedEvent(this, e);
            }
        }

        public event EventHandler<ProximityModuleEventArgs> DeviceDetachedEvent;

        // Invoke the DeviceDetachedEvent event; called whenever the device is detached:
        protected virtual void OnDeviceDetached(ProximityModuleEventArgs e)
        {
            if (DeviceDetachedEvent != null)
            {
                DeviceDetachedEvent(this, e);
            }
        }

        private IntPtr deviceNotificationHandle;
        private Boolean exclusiveAccess;
        private SafeFileHandle hidHandle;
        private String hidUsage;
        private Boolean myDeviceDetected;
        private String myDevicePathName;
        private SafeFileHandle readHandle;
        private SafeFileHandle writeHandle;

        private Debugging MyDebugging = new Debugging(); //  For viewing results of API calls via Tracer.Write.
        private DeviceManagement MyDeviceManagement = new DeviceManagement();
        private Hid MyHid = null; 

        public readonly Int32 vendorId = 0;     //0x0925;       // see PIC Firmware - usb_descriptors.c lines 178,179
        public readonly Int32 productId = 0;    //0x7001;

        bool UseControlTransfersOnly = false;


        ///  <summary>
        ///  Define a class of delegates that point to the Hid.ReportIn.Read function.
        ///  The delegate has the same parameters as Hid.ReportIn.Read.
        ///  Used for asynchronous reads from the device.       
        ///  </summary>

        private delegate void ReadInputReportDelegate(SafeFileHandle hidHandle, SafeFileHandle readHandle, SafeFileHandle writeHandle, ref Boolean myDeviceDetected, ref Byte[] readBuffer, ref Boolean success);

        //  This delegate has the same parameters as AccessForm.
        //  Used in accessing the application's form from a different thread.

        private delegate void MarshalToForm(String action, String textToAdd);

        ///  <summary>
        ///  Called when a WM_DEVICECHANGE message has arrived,
        ///  indicating that a device has been attached or removed.
        ///  </summary>
        ///  
        ///  <param name="m"> a message with information about the device </param>

        public void OnDeviceChange(Message m)
        {
            Tracer.Trace("ProximityModule (HID) USB OnDeviceChange() - m.WParam=" + m.WParam);

            try
            {
                switch (m.WParam.ToInt32())
                {
                    case DeviceManagement.DBT_DEVNODES_CHANGED:
                        // this one comes independent of the registration.
                        Tracer.Trace("DBT_DEVNODES_CHANGED");
                        if (!myDeviceDetected)
                        {
                            myDeviceDetected = FindTheHid(vendorId, productId);

                            if (myDeviceDetected)
                            {
                                OnDeviceAttached(new ProximityModuleEventArgs() { description = m.ToString() });
                            }
                        }
                        break;

                    case DeviceManagement.DBT_DEVICEARRIVAL:
                        // you have to issue MyDeviceManagement.RegisterForDeviceNotifications() first to receive this notification.
                        //  If WParam contains DBT_DEVICEARRIVAL, a device has been attached.

                        Tracer.Trace("A USB device has been attached.");

                        //  Find out if it's the device we're communicating with.

                        if (MyDeviceManagement.DeviceNameMatch(m, myDevicePathName))
                        {
                            Tracer.Trace("OK: My USB device (Proximity Module) attached.");
                            OnDeviceAttached(new ProximityModuleEventArgs() { description = m.ToString() });
                        }

                        break;

                    case DeviceManagement.DBT_DEVICEREMOVECOMPLETE:
                        // you have to issue MyDeviceManagement.RegisterForDeviceNotifications() first to receive this notification.
                        //  If WParam contains DBT_DEVICEREMOVAL, a device has been removed.

                        Tracer.Trace("Warning: A USB device has been removed.");

                        //  Find out if it's the device we're communicating with.

                        if (MyDeviceManagement.DeviceNameMatch(m, myDevicePathName))
                        {

                            Tracer.Trace("Warning: My device (Proximity Module) removed.");

                            //  Set MyDeviceDetected False so on the next data-transfer attempt,
                            //  FindTheHid() will be called to look for the device 
                            //  and get a new handle.

                            myDeviceDetected = false;
                            OnDeviceDetached(new ProximityModuleEventArgs() { description = m.ToString() });
                        }
                        break;
                }
            }
            catch (Exception ex)
            {
                Tracer.Error(ex);
                throw;
            }
        }

        private void GetInputReportBufferSize()
        {
            Int32 numberOfInputBuffers = 0;
            Boolean success;

            try
            {
                //  Get the number of input buffers.

                success = MyHid.GetNumberOfInputBuffers(hidHandle, ref numberOfInputBuffers);

                Tracer.Trace("Number of Input Buffers = " + numberOfInputBuffers);
            }
            catch (Exception ex)
            {
                Tracer.Error(ex);
                throw;
            }
        }

        ///  <summary>
        ///  Uses a series of API calls to locate a HID-class device
        ///  by its Vendor ID and Product ID.
        ///  Fills myDevicePathName with a path to device found, readHandle and writeHandle. Registers for Device Notifications (attached / detached type of events).
        ///  </summary>
        ///          
        ///  <returns>
        ///   True if the device is detected, False if not detected.
        ///  </returns>

        private Boolean FindTheHid(Int32 myVendorID, Int32 myProductID)
        {
            Boolean someHidDevicesFound = false;
            String[] devicePathName = new String[128];
            String functionName = "";
            Guid hidGuid = Guid.Empty;
            Int32 memberIndex = 0;
            Boolean success = false;

            try
            {
                myDeviceDetected = false;

                Tracer.Trace(string.Format("FindTheHid(0x{0:X04}, 0x{1:X04})", myVendorID, myProductID));

                //  ***
                //  API function: 'HidD_GetHidGuid

                //  Purpose: Retrieves the interface class GUID for the HID class.

                //  Accepts: 'A System.Guid object for storing the GUID.
                //  ***

                Hid.HidD_GetHidGuid(ref hidGuid);

                functionName = "GetHidGuid";
                Tracer.Trace(MyDebugging.ResultOfAPICall(functionName));
                Tracer.Trace("  GUID for system HIDs: " + hidGuid.ToString());

                //  Fill an array with the device path names of all attached HIDs.

                someHidDevicesFound = MyDeviceManagement.FindDeviceFromGuid(hidGuid, ref devicePathName);

                //  If there is at least one HID, attempt to read the Vendor ID and Product ID
                //  of each device until there is a match or all devices have been examined.
                //
                //  Fill myDevicePathName with a path to device found.

                if (someHidDevicesFound)
                {
                    memberIndex = 0;

                    // Tracer.Trace("  total number of HID devices: " + devicePathName.Length);        // will be something like 128, a lot of empty paths there.

                    do
                    {
                        //  ***
                        //  API function:
                        //  CreateFile

                        //  Purpose:
                        //  Retrieves a handle to a device.

                        //  Accepts:
                        //  A device path name returned by SetupDiGetDeviceInterfaceDetail
                        //  The type of access requested (read/write).
                        //  FILE_SHARE attributes to allow other processes to access the device while this handle is open.
                        //  A Security structure or IntPtr.Zero. 
                        //  A creation disposition value. Use OPEN_EXISTING for devices.
                        //  Flags and attributes for files. Not used for devices.
                        //  Handle to a template file. Not used.

                        //  Returns: a handle without read or write access.
                        //  This enables obtaining information about all HIDs, even system
                        //  keyboards and mice. 
                        //  Separate handles are used for reading and writing.
                        //  ***

                        if (!string.IsNullOrEmpty(devicePathName[memberIndex]))
                        {
                            Tracer.Trace("  trying HID device path '" + devicePathName[memberIndex] + "'");

                        hidHandle = FileIO.CreateFile(devicePathName[memberIndex], 0, FileIO.FILE_SHARE_READ | FileIO.FILE_SHARE_WRITE, IntPtr.Zero, FileIO.OPEN_EXISTING, 0, 0);

                        functionName = "CreateFile";
                        Tracer.Trace(MyDebugging.ResultOfAPICall(functionName));
                            Tracer.Trace("  FindTheHid(): some HID device found, returned handle: " + hidHandle.ToString());

                        if (!hidHandle.IsInvalid)
                        {
                            //  The returned handle is valid, 
                            //  so find out if this is the device we're looking for.

                            //  Set the Size property of DeviceAttributes to the number of bytes in the structure.

                            MyHid.DeviceAttributes.Size = Marshal.SizeOf(MyHid.DeviceAttributes);

                            //  ***
                            //  API function:
                            //  HidD_GetAttributes

                            //  Purpose:
                            //  Retrieves a HIDD_ATTRIBUTES structure containing the Vendor ID, 
                            //  Product ID, and Product Version Number for a device.

                            //  Accepts:
                            //  A handle returned by CreateFile.
                            //  A pointer to receive a HIDD_ATTRIBUTES structure.

                            //  Returns:
                            //  True on success, False on failure.
                            //  ***                            

                            success = Hid.HidD_GetAttributes(hidHandle, ref MyHid.DeviceAttributes);

                            if (success)
                            {
                                Tracer.Trace("  HIDD_ATTRIBUTES structure filled without error.");
                                Tracer.Trace("  Structure size: " + MyHid.DeviceAttributes.Size);
                                Tracer.Trace(string.Format("  Vendor ID: 0x{0:X04}", MyHid.DeviceAttributes.VendorID));
                                Tracer.Trace(string.Format("  Product ID: 0x{0:X04}", MyHid.DeviceAttributes.ProductID));
                                Tracer.Trace(string.Format("  Version Number: 0x{0:X04}", MyHid.DeviceAttributes.VersionNumber));

                                //  Find out if the device matches the one we're looking for.

                                if ((MyHid.DeviceAttributes.VendorID == myVendorID) && (MyHid.DeviceAttributes.ProductID == myProductID))
                                {

                                    Tracer.Trace("  My device detected");

                                    myDeviceDetected = true;

                                    //  Save the DevicePathName for OnDeviceChange().

                                    myDevicePathName = devicePathName[memberIndex];
                                }
                                else
                                {
                                    //  It's not a match, so close the handle.

                                    Tracer.Trace("  (This is not My Device)");

                                    myDeviceDetected = false;
                                    hidHandle.Close();
                                }
                            }
                            else
                            {
                                //  There was a problem in retrieving the information.

                                Tracer.Trace("  Error in filling HIDD_ATTRIBUTES structure.");
                                myDeviceDetected = false;
                                hidHandle.Close();
                            }
                        }
                        }

                        //  Keep looking until we find the device or there are no devices left to examine.

                        memberIndex = memberIndex + 1;
                    }
                    while (!((myDeviceDetected || (memberIndex == devicePathName.Length))));
                }

                if (myDeviceDetected)
                {
                    //  The device was detected.
                    //  Register to receive notifications if the device is removed or attached.

                    success = MyDeviceManagement.RegisterForDeviceNotifications(myDevicePathName, WindowHandle, hidGuid, ref deviceNotificationHandle);

                    Tracer.Trace("RegisterForDeviceNotifications = " + success);

                    //  Learn the capabilities of the device.

                    MyHid.Capabilities = MyHid.GetDeviceCapabilities(hidHandle);

                    if (success)
                    {
                        //  Find out if the device is a system mouse or keyboard.

                        hidUsage = MyHid.GetHidUsage(MyHid.Capabilities);

                        //  Get the Input report buffer size.

                        GetInputReportBufferSize();

                        //  Get handles to use in requesting Input and Output reports.

                        readHandle = FileIO.CreateFile(myDevicePathName, FileIO.GENERIC_READ, FileIO.FILE_SHARE_READ | FileIO.FILE_SHARE_WRITE, IntPtr.Zero, FileIO.OPEN_EXISTING, FileIO.FILE_FLAG_OVERLAPPED, 0);

                        functionName = "CreateFile, ReadHandle";
                        Tracer.Trace(MyDebugging.ResultOfAPICall(functionName));
                        Tracer.Trace("  FindTheHid(): success, returned handle: " + readHandle.ToString());

                        if (readHandle.IsInvalid)
                        {
                            exclusiveAccess = true;
                            Tracer.Error("The device is a system " + hidUsage + ". Applications can access Feature reports only.");
                        }
                        else
                        {
                            writeHandle = FileIO.CreateFile(myDevicePathName, FileIO.GENERIC_WRITE, FileIO.FILE_SHARE_READ | FileIO.FILE_SHARE_WRITE, IntPtr.Zero, FileIO.OPEN_EXISTING, 0, 0);

                            functionName = "CreateFile, WriteHandle";
                            Tracer.Trace(MyDebugging.ResultOfAPICall(functionName));
                            Tracer.Trace("  FindTheHid(): handle valid, returned handle: " + writeHandle.ToString());

                            //  Flush any waiting reports in the input buffer. (optional)

                            MyHid.FlushQueue(readHandle);
                        }
                    }
                }
                else
                {
                    //  The device wasn't detected.

                    Tracer.Error(string.Format("My Device not found - need a HID with vendorId={0}, productId={1}.", vendorId, productId));
                }
                return myDeviceDetected;
            }
            catch (Exception ex)
            {
                Tracer.Error(ex);
                throw;
            }
        }

        ///  <summary>
        ///  Sends an Output report, then may retrieve an Input report.
        ///  </summary>

        private byte[] ExchangeInputAndOutputReports(byte[] outputReportBuffer, bool doOutputReport, bool doInputReport, int expectedInputReportID, EventHandler<AsyncInputReportArgs> readCompleteHandler)
        {
            StringBuilder byteValue = null;
            int count = 0;
            byte[] inputReportBuffer = null;
            bool success = false;

            try
            {
                success = false;

                //  Don't attempt to exchange reports if valid handles aren't available
                //  (as for a mouse or keyboard under Windows 2000/XP.)

                if (!readHandle.IsInvalid && !writeHandle.IsInvalid)
                {
                    //  Don't attempt to send an Output report if the HID has no Output report.

                    if (doOutputReport && MyHid.Capabilities.OutputReportByteLength > 0)
                    {
                        //  Write a report.

                        if ((UseControlTransfersOnly) == true)
                        {

                            //  Use a control transfer to send the report,
                            //  even if the HID has an interrupt OUT endpoint.

                            Hid.OutputReportViaControlTransfer myOutputReport = new Hid.OutputReportViaControlTransfer();
                            success = myOutputReport.Write(outputReportBuffer, writeHandle);
                        }
                        else
                        {

                            //  Use WriteFile to send the report.
                            //  If the HID has an interrupt OUT endpoint, WriteFile uses an 
                            //  interrupt transfer to send the report. 
                            //  If not, WriteFile uses a control transfer.

                            Hid.OutputReportViaInterruptTransfer myOutputReport = new Hid.OutputReportViaInterruptTransfer();
                            success = myOutputReport.Write(outputReportBuffer, writeHandle);
                        }

                        if (success)
                        {
                            ;
                            /*
                            byteValue = new StringBuilder();
                            byteValue.AppendFormat("An Output report has been written. Output Report ID: {0:X02}\r\n Output Report Data: ", outputReportBuffer[0]);

                            for (count = 0; count <= outputReportBuffer.Length - 1; count++)
                            {
                                //  Display bytes as 2-character hex strings.
                                byteValue.AppendFormat("{0:X02} ", outputReportBuffer[count]);
                            }
                            Tracer.Trace(byteValue.ToString());
                            */
                        }
                        else
                        {
                            Tracer.Error("The attempt to write an Output report has failed.");
                        }
                    }
                    else
                    {
                        //Tracer.Trace("No attempt to send an Output report was made.");
                        if (doOutputReport)
                        {
                            Tracer.Error("The HID doesn't have an Output report, but it was requested.");
                        }
                    }

                    //  Read an Input report.

                    //  Don't attempt to send an Input report if the HID has no Input report.
                    //  (The HID spec requires all HIDs to have an interrupt IN endpoint,
                    //  which suggests that all HIDs must support Input reports.)

                    if (doInputReport && MyHid.Capabilities.InputReportByteLength > 0)
                    {
                        success = false;

                        //  Set the size of the Input report buffer. 

                        inputReportBuffer = new byte[MyHid.Capabilities.InputReportByteLength];

                        if (UseControlTransfersOnly || readCompleteHandler == null)
                        {
                            //  Read a report using a control transfer.

                            Hid.InputReportViaControlTransfer myInputReport = new Hid.InputReportViaControlTransfer();

                            //  Read the report via HidD_GetInputReport  http://www.osronline.com/DDKx/intinput/hidfunc_3hgy.htm
                            // - "If the top-level collection includes report IDs, the caller must set the first byte of the buffer to a nonzero report ID; otherwise the caller must set the first byte to zero."
                            inputReportBuffer[0] = (byte)expectedInputReportID;

                            myInputReport.Read(hidHandle, readHandle, writeHandle, ref myDeviceDetected, ref inputReportBuffer, ref success);

                            if (success)
                            {
                                ;
                                /*
                                byteValue = new StringBuilder();
                                byteValue.AppendFormat("ExchangeInputAndOutputReports(): An Input report has been read via ControlTransfer. Input Report ID: {0:X02}\r\n Input Report Data: ", inputReportBuffer[0]);

                                for (count = 0; count <= inputReportBuffer.Length - 1; count++)
                                {
                                    //  Display bytes as 2-character Hex strings.
                                    byteValue.AppendFormat("{0:X02} ", inputReportBuffer[count]);
                                }
                                Tracer.Trace(byteValue.ToString());
                                */
                            }
                            else
                            {
                                Tracer.Error("ExchangeInputAndOutputReports(): The attempt to read an Input report has failed.");
                            }
                        }
                        else
                        {
                            //  Read a report using interrupt transfers.                
                            //  To enable reading a report without blocking the main thread, this
                            //  application uses an asynchronous delegate.

                            //Tracer.Trace("IP: Arranging asyncronous read via Interrupt Transfer");

                            IAsyncResult ar = null;
                            Hid.InputReportViaInterruptTransfer myInputReport = new Hid.InputReportViaInterruptTransfer();
                            if (readCompleteHandler != null)
                            {
                                myInputReport.HasReadData += readCompleteHandler;
                            }

                            //  Define a delegate for the Read method of myInputReport.

                            ReadInputReportDelegate MyReadInputReportDelegate = new ReadInputReportDelegate(myInputReport.Read);

                            //  The BeginInvoke method calls myInputReport.Read to attempt to read a report.
                            //  The method has the same parameters as the Read function,
                            //  plus two additional parameters:
                            //  GetInputReportData is the callback procedure that executes when the Read function returns.
                            //  MyReadInputReportDelegate is the asynchronous delegate object.
                            //  The last parameter can optionally be an object passed to the callback.

                            ar = MyReadInputReportDelegate.BeginInvoke(hidHandle, readHandle, writeHandle, ref myDeviceDetected, ref inputReportBuffer, ref success, new AsyncCallback(GetInputReportData), MyReadInputReportDelegate);
                        }
                    }
                    else
                    {
                        //Tracer.Trace("No attempt to read an Input report was made.");
                        if (doInputReport)
                        {
                            Tracer.Error("The HID doesn't have an Input report, but it was requested.");
                        }
                    }
                }
                else
                {
                    Tracer.Error("Invalid handle. The device is probably a system mouse or keyboard.");
                    Tracer.Error("No attempt to write an Output report or read an Input report was made.");
                }

                return inputReportBuffer;   // may still be null
            }
            catch (Exception ex)
            {
                Tracer.Error(ex);
                throw;
            }
        }

        ///  <summary>
        ///  Retrieves Input report data and status information.
        ///  This routine is called automatically when myInputReport.Read
        ///  returns. Calls several marshaling routines to access the main form.
        ///  </summary>
        ///  
        ///  <param name="ar"> an object containing status information about the asynchronous operation. </param>

        private void GetInputReportData(IAsyncResult ar)
        {
            StringBuilder byteValue = null;
            int count = 0;
            byte[] inputReportBuffer = null;
            bool success = false;

            try
            {
                // Define a delegate using the IAsyncResult object.

                ReadInputReportDelegate deleg = ((ReadInputReportDelegate)(ar.AsyncState));

                //  Get the IAsyncResult object and the values of other paramaters that the
                //  BeginInvoke method passed ByRef.

                deleg.EndInvoke(ref myDeviceDetected, ref inputReportBuffer, ref success, ar);

                //  Display the received report data in the form's list box.

                if ((ar.IsCompleted && success))
                {
                    ;
                    /*
                    byteValue = new StringBuilder();
                    byteValue.AppendFormat("GetInputReportData(): An Input report has been read asyncronously via Interrupt Transfer. Input Report ID: {0:X02}\r\n Input Report Data: ", inputReportBuffer[0]);

                    for (count = 0; count <= inputReportBuffer.Length - 1; count++)
                    {
                        //  Display bytes as 2-character Hex strings.
                        byteValue.AppendFormat("{0:X02} ", inputReportBuffer[count]);
                    }
                    Tracer.Trace(byteValue.ToString());
                    */
                }
                else
                {
                    Tracer.Error("GetInputReportData(): The attempt to read an Input report has failed.");
                }
            }
            catch (Exception ex)
            {
                Tracer.Error(ex);
                throw;
            }
        }

        ///  <summary>
        ///  Initiates exchanging reports. 
        ///  The application sends a report and requests to read a report.
        ///  </summary>

        private void WriteToDevice(byte[] outputReportBuffer)
        {
            //Tracer.Trace("WriteToDevice() " + DateTime.Now);

            try
            {
                //  If the device hasn't been detected, was removed, or timed out on a previous attempt
                //  to access it, look for the device.

                if ((myDeviceDetected == false))
                {
                    myDeviceDetected = FindTheHid(vendorId, productId);
                }

                if ((myDeviceDetected == true))
                {
                    //  Get the bytes to send in a report from the combo boxes.

                    //  An option button selects whether to exchange Input and Output reports
                    //  or Feature reports.

                    //if ((optInputOutput.Checked == true))
                    {
                        ExchangeInputAndOutputReports(outputReportBuffer, true, false, 0, null);   // do not expect any response.
                    }
                    //else
                    //{
                    //    ExchangeFeatureReports();
                    //}
                }
            }
            catch (Exception ex)
            {
                Tracer.Error(ex);
                throw;
            }
        }

        private byte[] ReadAndWriteToDevice(byte[] outputReportBuffer, int expectedInputReportID)
        {
            byte[] inputReportBuffer = null;

            //Tracer.Trace("ReadAndWriteToDevice() " + DateTime.Now);

            try
            {
                //  If the device hasn't been detected, was removed, or timed out on a previous attempt
                //  to access it, look for the device.

                if ((myDeviceDetected == false))
                {
                    myDeviceDetected = FindTheHid(vendorId, productId);
                }

                if ((myDeviceDetected == true))
                {
                    //  An option button selects whether to exchange Input and Output reports
                    //  or Feature reports.

                    //if ((optInputOutput.Checked == true))
                    {
                        inputReportBuffer = ExchangeInputAndOutputReports(outputReportBuffer, true, true, expectedInputReportID, null);    // expect, wait for and read a response.
                    }
                    //else
                    //{
                    //    ExchangeFeatureReports();
                    //}
                }
                return inputReportBuffer;
            }
            catch (Exception ex)
            {
                Tracer.Error(ex);
                throw;
            }
        }

        private byte[] ReadFromDevice(EventHandler<AsyncInputReportArgs> readCompleteHandler, int expectedInputReportID)
        {
            byte[] inputReportBuffer = null;

            //Tracer.Trace("ReadFromDevice() " + DateTime.Now);

            try
            {
                //  If the device hasn't been detected, was removed, or timed out on a previous attempt
                //  to access it, look for the device.

                if ((myDeviceDetected == false))
                {
                    myDeviceDetected = FindTheHid(vendorId, productId);
                }

                if ((myDeviceDetected == true))
                {
                    //  An option button selects whether to exchange Input and Output reports
                    //  or Feature reports.

                    //if ((optInputOutput.Checked == true))
                    {
                        inputReportBuffer = ExchangeInputAndOutputReports(null, false, true, expectedInputReportID, readCompleteHandler);  // wait for any input from the device, call readCompleteHandler when it comes.
                    }
                    //else
                    //{
                    //    ExchangeFeatureReports();
                    //}
                }
                return inputReportBuffer;
            }
            catch (Exception ex)
            {
                Tracer.Error(ex);
                throw;
            }
        }

        ///  <summary>
        ///  Perform actions that must execute when the program ends.
        ///  </summary>

        private void ShutdownHid()
        {
            try
            {
                //  Close open handles to the device.

                if (!(hidHandle == null))
                {
                    if (!(hidHandle.IsInvalid))
                    {
                        hidHandle.Close();
                    }
                }

                if (!(readHandle == null))
                {
                    if (!(readHandle.IsInvalid))
                    {
                        readHandle.Close();
                    }
                }

                if (!(writeHandle == null))
                {
                    if (!(writeHandle.IsInvalid))
                    {
                        writeHandle.Close();
                    }
                }

                //  Stop receiving notifications.

                MyDeviceManagement.StopReceivingDeviceNotifications(deviceNotificationHandle);

                myDeviceDetected = false;
            }
            catch (Exception ex)
            {
                Tracer.Error(ex);
                throw;
            }
        }

        ///  <summary>
        ///  Perform actions that must execute when the program starts.
        ///  </summary>

        private void Startup()
        {
            try
            {
                MyHid = new Hid();

                //  Default USB Vendor ID and Product ID:

                //txtVendorID.Text = "0925";
                //txtProductID.Text = "7001";
            }
            catch (Exception ex)
            {
                Tracer.Error(ex);
                throw;
            }
        }         
        
   
    }
}

