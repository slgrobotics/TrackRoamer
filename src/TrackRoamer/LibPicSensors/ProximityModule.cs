using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Threading;

using Microsoft.Win32.SafeHandles;
using System.Globalization;
using System.Runtime.InteropServices;

using TrackRoamer.Robotics.Utility.LibSystem;
using TrackRoamer.Robotics.Utility.LibLvrGenericHid;

namespace TrackRoamer.Robotics.Utility.LibPicSensors
{
	public partial class ProximityModule
	{
        private System.Windows.Forms.Timer picUsbTickerTimer = null;

        public ProximityModule(Int32 _vendorId, Int32 _productId)
        {
            Tracer.Trace(string.Format("IP: ProximityModule(vendorId={0}, productId={1})", _vendorId, _productId));

            // USB Device ID for Proximity Module. Must match definitions in Microchip PIC microcontroller code (USB Device - HID - Proximity Module\Generic HID - Firmware\usb_descriptors.c lines 178, 179):
            vendorId = _vendorId;
            productId = _productId;

            /// we need this to catch device change events in a non-Winforms environment, which we have in MRDS DSS host:
            EnsureEventsWindowInitialized();

            Startup();
                 
            Tracer.Trace("OK: ProximityModule()");
        }

        public void Open()
        {
            try
            {
                bool isConnected = FindTheHid(vendorId, productId);

                if (isConnected)
                {
                    Tracer.Trace("OK: USB Interface connected with PIC USB Proximity Board");
                }
                else
                {
                    string str = string.Format("USB Interface could not connect with device with Vendor ID={0} Product ID={1}", vendorId, productId);
                    Tracer.Error(str);
                    throw new Exception(str);
                }

                // run a monitoring thread in case we need to reset things:
                picUsbTickerTimer = new System.Windows.Forms.Timer();
                picUsbTickerTimer.Interval = 1000;    // ms
                picUsbTickerTimer.Tick += new EventHandler(picUsbTicker);
                picUsbTickerTimer.Start();

                Tracer.Trace("OK: PIC Proximity Board ticker ON");
            }
            catch (Exception ex)
            {
                Tracer.Error(ex);
                throw;
            }             

        }

		public void Close()
		{
			if (picUsbTickerTimer != null)
			{
				picUsbTickerTimer.Enabled = false;
				picUsbTickerTimer.Dispose();
				picUsbTickerTimer = null;

                Tracer.Trace("OK: PIC Proximity Board ticker OFF");
            }

            ShutdownHid();
        }

        public void picUsbTicker(object obj, System.EventArgs args)
        {
            // a monitoring thread in case we need to reset things

            try
            {
                //Tracer.Trace("...picUsb ticker... " + DateTime.Now);

                //  Don't allow another transfer request until this one completes.
                //  Move the focus away from cmdOnce to prevent the focus from 
                //  switching to the next control in the tab order on disabling the button.

                ;
                //ReadAndWriteToDevice();
            }
            catch (Exception ex)
            {
                Tracer.Error(ex);
                throw;
            }

            picUsbTickerTimer.Enabled = true;   // for the next cycle
        }
	}
}
