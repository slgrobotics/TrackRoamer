using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;
using System.Windows.Forms;
using System.Threading;
using Microsoft.DirectX.DirectInput;

using Microsoft.Win32.SafeHandles;
using System.Globalization;
using System.Runtime.InteropServices; 

using LibSystem;
using LibLvrGenericHid;

namespace LibPicSensors
{
    public enum ProximityBoardCommand
    {
        TestEcho,           // = 0; compatible with LVR test suite, will send back bytes that were send to the board.

        SafePosture,        // sets all actuators in safe/known position, and disables unsolicited input and automatic movements.

        ServoPositionSet,

        ServoPositionGet,

        ServoSweepStart,
        ServoSweepStop,
        ServoSweepParams,

        PingDistanceGet,

        DataContinuousStart,
        DataContinuousStop
    }

    public class AsyncInputFrameArgs : EventArgs
    {
        public double dPos1Mks;
        public double dPos2Mks;

        public double dPing1DistanceMm;
        public double dPing2DistanceMm;

        public AsyncInputFrameArgs(int servo1target, int servo2target, int ping1value, int ping2value)
        {
            dPos1Mks = ProximityModule.servoTargetToMks(servo1target);
            dPos2Mks = ProximityModule.servoTargetToMks(servo2target);

            dPing1DistanceMm = ProximityModule.pingValueToDistanceMm(ping1value);
            dPing2DistanceMm = ProximityModule.pingValueToDistanceMm(ping2value);
        }
    }

    public partial class ProximityModule
	{
        // as defined on the board:
        //const int SERVO_VALUE_MIN = 1275;		// minimum 1275 to make pulse  850 mks
        //const int SERVO_VALUE_MAX = 3150;	    // maximum 3150 to make pulse 2100 mks
        const int SERVO_VALUE_MIN  = 750;		// minimum 750 to make pulse  500 mks
        const int SERVO_VALUE_MAX = 3750;	    // maximum 3750 to make pulse 2500 mks
        const int SERVO_VALUE_MID = 2250;	    // neutral 2250 to make pulse 1500 mks

        // we are talking microseconds here, when it comes to servo pulses, passed as parameters:
        //const int SERVO_MKS_MIN = 850;		    // minimum pulse  850 mks
        //const int SERVO_MKS_MAX = 2100;	        // maximum pulse 2100 mks
        const int SERVO_MKS_MIN = 500;		    // minimum pulse  500 mks
        const int SERVO_MKS_MAX = 2500;	        // maximum pulse 2500 mks

        public event EventHandler<AsyncInputFrameArgs> HasReadFrame;

        // Invoke the HasReadFrame event; called whenever the Read completes:
        protected virtual void OnDataFrameComplete(AsyncInputFrameArgs e)
        {
            if (HasReadFrame != null)
            {
                HasReadFrame(this, e);
            }
        }

        ~ProximityModule()
        {
            if (myDeviceDetected)
            {
                if (inDataContinuousMode)
                {
                    DataContinuousStop();
                }

                ServoSweepEnable(false);

                Close();
            }
        }

        public void SafePosture()
        {
            try
            {
                Tracer.Trace("SafePosture()");

                if (!myDeviceDetected)
                {
                    myDeviceDetected = FindTheHid(vendorId, productId);
                }

                if (myDeviceDetected)
                {
                    //  Set the size of the Output report buffer.   

                    byte[] outputReportBuffer = new byte[MyHid.Capabilities.OutputReportByteLength];

                    //  Store the report ID (command) in the first byte of the buffer:

                    outputReportBuffer[0] = (byte)0;      // will turn out on the board as Output Report ID

                    outputReportBuffer[1] = (byte)ProximityBoardCommand.SafePosture;      // will be analyzed in a switch

                    ReadAndWriteToDevice(outputReportBuffer);
                    //WriteToDevice(outputReportBuffer);
                }
                else
                {
                    Tracer.Error("FindTheHid() unsuccessful, skipped the SafePosture command");
                }
            }
            catch (Exception ex)
            {
                Tracer.Error(ex);
                throw;
            }
        }

        public void ServoPositionSet(int servoNumber, double dPosMks)
        {
            try
            {
                Tracer.Trace("ServoPositionSet(" + servoNumber + "," + dPosMks + ")");

                dPosMks = Math.Min(Math.Max(dPosMks, SERVO_MKS_MIN), SERVO_MKS_MAX);        // cut it to the allowed limits

                if (!myDeviceDetected)
                {
                    myDeviceDetected = FindTheHid(vendorId, productId);
                }

                if (myDeviceDetected)
                {
                    //  Set the size of the Output report buffer.   

                    byte[] outputReportBuffer = new byte[MyHid.Capabilities.OutputReportByteLength];

                    //  Store the report ID (command) in the first byte of the buffer:

                    outputReportBuffer[0] = (byte)0;      // will turn out on the board as Output Report ID

                    outputReportBuffer[1] = (byte)ProximityBoardCommand.ServoPositionSet;      // will be analyzed in a switch

                    outputReportBuffer[2] = (byte)servoNumber;      // which servo

                    int servotarget = mksToServoTarget(dPosMks);

                    // data bytes 0 and 1 of the Output Report - hid_report_out[]:
                    intToBuffer(outputReportBuffer, 3, (UInt16)servotarget);

                    ReadAndWriteToDevice(outputReportBuffer);
                    //WriteToDevice(outputReportBuffer);
                }
                else
                {
                    Tracer.Error("FindTheHid() unsuccessful, skipped the ServoPositionSet command");
                }
            }
            catch (Exception ex)
            {
                Tracer.Error(ex);
                throw;
            }
        }

        public void ServoSweepParams(int servoNumber, double sweepMin, double sweepMax, double sweepStartPos, double sweepStep, bool initialDirectionUp, double sweepRate)
        {
            try
            {
                Tracer.Trace("ServoSweepParams(" + servoNumber + "," + sweepMin + "," + sweepMax + "," + sweepStep + "," + sweepRate + ")");

                sweepMin = Math.Min(Math.Max(sweepMin, SERVO_MKS_MIN), SERVO_MKS_MAX);        // cut it to the allowed limits
                sweepMax = Math.Min(Math.Max(sweepMax, SERVO_MKS_MIN), SERVO_MKS_MAX);        // cut it to the allowed limits

                if (!myDeviceDetected)
                {
                    myDeviceDetected = FindTheHid(vendorId, productId);
                }

                if (myDeviceDetected)
                {
                    //  Set the size of the Output report buffer.   

                    byte[] outputReportBuffer = new byte[MyHid.Capabilities.OutputReportByteLength];

                    //  Store the report ID (command) in the first byte of the buffer:

                    outputReportBuffer[0] = (byte)0;      // will turn out on the board as Output Report ID

                    outputReportBuffer[1] = (byte)ProximityBoardCommand.ServoSweepParams;      // will be analyzed in a switch

                    outputReportBuffer[2] = (byte)servoNumber;      // which servo

                    int iSweepMin = mksToServoTarget(sweepMin);
                    int iSweepMax = mksToServoTarget(sweepMax);
                    int iSweepStart = mksToServoTarget(sweepStartPos);
                    int iSweepStep = (int)sweepStep;
                    int iSweepRate = (int)sweepRate;

                    intToBuffer(outputReportBuffer, 3, (UInt16)iSweepMin);
                    intToBuffer(outputReportBuffer, 5, (UInt16)iSweepMax);
                    intToBuffer(outputReportBuffer, 7, (UInt16)iSweepStart);
                    intToBuffer(outputReportBuffer, 9, (UInt16)iSweepStep);
                    intToBuffer(outputReportBuffer, 11, (UInt16)iSweepRate);
                    outputReportBuffer[13] = (byte)(initialDirectionUp ? 1 : -1); 

                    ReadAndWriteToDevice(outputReportBuffer);
                    //WriteToDevice(outputReportBuffer);
                }
                else
                {
                    Tracer.Error("FindTheHid() unsuccessful, skipped the ServoSweepParams command");
                }
            }
            catch (Exception ex)
            {
                Tracer.Error(ex);
                throw;
            }
        }

        private UInt16 intFromBuffer(byte[] buffer, int offset)
        {
            UInt16 ret = (UInt16)(buffer[offset] + ((int)buffer[offset + 1] << 8));

            return ret;
        }

        private void intToBuffer(byte[] buffer, int offset, UInt16 val)
        {
            buffer[offset] = (byte)(val & 0xFF);
            buffer[offset + 1] = (byte)((val & 0xFF00) >> 8);
        }

        internal static double servoTargetToMks(int servotarget)
        {
            return Math.Round((double)(SERVO_MKS_MIN + (servotarget - SERVO_VALUE_MIN) * (SERVO_MKS_MAX - SERVO_MKS_MIN) / (SERVO_VALUE_MAX - SERVO_VALUE_MIN)));
        }

        internal static int mksToServoTarget(double dPosMks)
        {
            return (int)Math.Round(SERVO_VALUE_MIN + (SERVO_VALUE_MAX - SERVO_VALUE_MIN) * (dPosMks - SERVO_MKS_MIN) / (SERVO_MKS_MAX - SERVO_MKS_MIN));
        }

        internal static double pingValueToDistanceMm(int pingResponseValue)
        {
            return Math.Max(0.0d, Math.Round((double)(pingResponseValue - 1300.0d) * 1320.0d / (12500.0d - 1300.0d)));
        }

        public double ServoPositionGet(int servoNumber)
        {
            double dPosMks = 0;

            try
            {
                Tracer.Trace("ServoPositionGet(" + servoNumber + ")");

                if (!myDeviceDetected)
                {
                    myDeviceDetected = FindTheHid(vendorId, productId);
                }

                if (myDeviceDetected)
                {
                    //  Set the size of the Output report buffer.   

                    byte[] outputReportBuffer = new byte[MyHid.Capabilities.OutputReportByteLength];

                    //  Store the report ID (command) in the first byte of the buffer:

                    outputReportBuffer[0] = (byte)0;      // will turn out on the board as Output Report ID

                    outputReportBuffer[1] = (byte)ProximityBoardCommand.ServoPositionGet;      // will be analyzed in a switch

                    outputReportBuffer[2] = (byte)servoNumber;      // which servo

                    byte[] inputReportBuffer = ReadAndWriteToDevice(outputReportBuffer);

                    // data bytes 1 and 2 of the Input Report contain servo position:

                    int servotarget = intFromBuffer(inputReportBuffer, 1);

                    dPosMks = servoTargetToMks(servotarget);

                }
                else
                {
                    Tracer.Error("FindTheHid() unsuccessful, skipped the ServoPositionGet command");
                }
            }
            catch (Exception ex)
            {
                Tracer.Error(ex);
                throw;
            }

            return dPosMks;
        }

        public double PingDistanceGet(int pingUnitNumber)
        {
            double dPingDistanceMm = 0;

            try
            {
                Tracer.Trace("PingDistanceGet(" + pingUnitNumber + ")");

                if (!myDeviceDetected)
                {
                    myDeviceDetected = FindTheHid(vendorId, productId);
                }

                if (myDeviceDetected)
                {
                    //  Set the size of the Output report buffer.   

                    byte[] outputReportBuffer = new byte[MyHid.Capabilities.OutputReportByteLength];

                    //  Store the report ID (command) in the first byte of the buffer:

                    outputReportBuffer[0] = (byte)0;      // will turn out on the board as Output Report ID

                    outputReportBuffer[1] = (byte)ProximityBoardCommand.PingDistanceGet;      // will be analyzed in a switch

                    outputReportBuffer[2] = (byte)pingUnitNumber;      // which servo

                    byte[] inputReportBuffer = ReadAndWriteToDevice(outputReportBuffer);

                    // data bytes 1 and 2 of the Input Report contain ping response value:

                    int pingResponseValue = intFromBuffer(inputReportBuffer, 1);

                    dPingDistanceMm = pingValueToDistanceMm(pingResponseValue);

                }
                else
                {
                    Tracer.Error("FindTheHid() unsuccessful, skipped the PingDistanceGet command");
                }
            }
            catch (Exception ex)
            {
                Tracer.Error(ex);
                throw;
            }

            return dPingDistanceMm;
        }

        public void ServoSweepEnable(bool enable)
        {
            try
            {
                Tracer.Trace("ServoSweepStart()");

                if (!myDeviceDetected)
                {
                    myDeviceDetected = FindTheHid(vendorId, productId);
                }

                if (myDeviceDetected)
                {
                    //  Set the size of the Output report buffer.   

                    byte[] outputReportBuffer = new byte[MyHid.Capabilities.OutputReportByteLength];

                    //  Store the report ID (command) in the first byte of the buffer:

                    outputReportBuffer[0] = (byte)0;      // will turn out on the board as Output Report ID

                    outputReportBuffer[1] = (byte)(enable ? ProximityBoardCommand.ServoSweepStart : ProximityBoardCommand.ServoSweepStop);      // will be analyzed in a switch

                    ReadAndWriteToDevice(outputReportBuffer);
                }
                else
                {
                    Tracer.Error("FindTheHid() unsuccessful, skipped the ServoSweepStart command");
                }
            }
            catch (Exception ex)
            {
                Tracer.Error(ex);
                throw;
            }
        }

        private bool inDataContinuousMode = false;

        public void DataContinuousStart()
        {
            try
            {
                Tracer.Trace("DataContinuousStart()");

                if (!myDeviceDetected)
                {
                    myDeviceDetected = FindTheHid(vendorId, productId);
                }

                if (myDeviceDetected)
                {
                    //  Set the size of the Output report buffer.   

                    byte[] outputReportBuffer = new byte[MyHid.Capabilities.OutputReportByteLength];

                    //  Store the report ID (command) in the first byte of the buffer:

                    outputReportBuffer[0] = (byte)0;      // will turn out on the board as Output Report ID

                    outputReportBuffer[1] = (byte)ProximityBoardCommand.DataContinuousStart;      // will be analyzed in a switch

                    ReadAndWriteToDevice(outputReportBuffer);

                    inDataContinuousMode = true;

                    ReadFromDevice(new EventHandler<AsyncInputReportArgs>(DataReadCompleteHandler));

                }
                else
                {
                    Tracer.Error("FindTheHid() unsuccessful, skipped the DataContinuousStart command");
                }
            }
            catch (Exception ex)
            {
                Tracer.Error(ex);
                throw;
            }
        }

        // This will be called whenever the data is read from the board:
        private void DataReadCompleteHandler(object sender, AsyncInputReportArgs aira)
        {
            Tracer.Trace("Async data arrived. " + DateTime.Now);


            StringBuilder byteValue = new StringBuilder();
            byteValue.Append("DataReadCompleteHandler(): Input Report Data: ");

            for (int count = 0; count <= aira.InputBuffer.Length - 1; count++)
            {
                //  Display bytes as 2-character Hex strings.
                byteValue.AppendFormat("{0:X02} ", aira.InputBuffer[count]);
            }
            Tracer.Trace(byteValue.ToString());

            int servo1target = intFromBuffer(aira.InputBuffer, 1);
            int servo2target = intFromBuffer(aira.InputBuffer, 3);

            int ping1value = intFromBuffer(aira.InputBuffer, 5);
            int ping2value = intFromBuffer(aira.InputBuffer, 7);

            AsyncInputFrameArgs args = new AsyncInputFrameArgs(servo1target, servo2target, ping1value, ping2value);

            OnDataFrameComplete(args);

            if (inDataContinuousMode)
            {
                // initiate next read:
                ReadFromDevice(new EventHandler<AsyncInputReportArgs>(DataReadCompleteHandler));
            }
        }

        public void DataContinuousStop()
        {
            try
            {
                Tracer.Trace("DataContinuousStop()");

                inDataContinuousMode = false;

                if (!myDeviceDetected)
                {
                    myDeviceDetected = FindTheHid(vendorId, productId);
                }

                if (myDeviceDetected)
                {
                    //  Set the size of the Output report buffer.   

                    byte[] outputReportBuffer = new byte[MyHid.Capabilities.OutputReportByteLength];

                    //  Store the report ID (command) in the first byte of the buffer:

                    outputReportBuffer[0] = (byte)0;      // will turn out on the board as Output Report ID

                    outputReportBuffer[1] = (byte)ProximityBoardCommand.DataContinuousStop;      // will be analyzed in a switch

                    ReadAndWriteToDevice(outputReportBuffer);
                }
                else
                {
                    Tracer.Error("FindTheHid() unsuccessful, skipped the DataContinuousStop command");
                }
            }
            catch (Exception ex)
            {
                Tracer.Error(ex);
                throw;
            }
        }

	}
}
