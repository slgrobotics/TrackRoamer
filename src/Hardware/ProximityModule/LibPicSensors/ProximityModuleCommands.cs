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

    public partial class ProximityModule : IDisposable
	{
        const byte INPUT_REPORT_ID      = 1;    // regular (reply for output) input report ID
        const byte INPUT_CONT_REPORT_ID = 5;    // unsolicited (continuous) input report ID
        const byte OUTPUT_REPORT_ID     = 2;

        public event EventHandler<AsyncInputFrameArgs> HasReadFrameEvent;

        // Invoke the HasReadFrame event; called whenever the Read completes:
        protected virtual void OnDataFrameComplete(AsyncInputFrameArgs e)
        {
            if (HasReadFrameEvent != null)
            {
                HasReadFrameEvent(this, e);
            }
        }

        ~ProximityModule()
        {
            Shutdown();
        }

        public void Dispose()
        {
            Shutdown();
        }

        public bool FindTheHid()
        {
            try
            {
                Tracer.Trace("PM Command::FindTheHid()");

                if (!myDeviceDetected)
                {
                    myDeviceDetected = FindTheHid(vendorId, productId);
                }

            }
            catch (Exception ex)
            {
                Tracer.Error(ex);
                throw;
            }
            return myDeviceDetected;
        }

        private void Shutdown()
        {
            // try to stop activity on the board, putting all actuators and sensors to safe posture.

            if (myDeviceDetected)
            {
                try
                {
                    if (inDataContinuousMode)
                    {
                        DataContinuousStop();
                    }

                    SafePosture();
                }
                catch { }

                Close();
            }
        }

        public void SafePosture()
        {
            try
            {
                Tracer.Trace("PM Command::SafePosture()");

                if (!myDeviceDetected)
                {
                    myDeviceDetected = FindTheHid(vendorId, productId);
                }

                if (myDeviceDetected)
                {
                    //  Set the size of the Output report buffer.   

                    byte[] outputReportBuffer = new byte[MyHid.Capabilities.OutputReportByteLength];

                    //  Store the report ID (command) in the first byte of the buffer:

                    outputReportBuffer[0] = OUTPUT_REPORT_ID;      // will turn out on the board as Output Report ID

                    outputReportBuffer[1] = (byte)ProximityBoardCommand.SafePosture;      // will be analyzed in a switch

                    ReadAndWriteToDevice(outputReportBuffer, INPUT_REPORT_ID);
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
                Tracer.Trace("PM Command::ServoPositionSet(" + servoNumber + "," + dPosMks + ")");

                dPosMks = ProximityBoard.toWithinServoRangeMks(dPosMks);        // cut it to the allowed limits

                if (!myDeviceDetected)
                {
                    myDeviceDetected = FindTheHid(vendorId, productId);
                }

                if (myDeviceDetected)
                {
                    //  Set the size of the Output report buffer.   

                    byte[] outputReportBuffer = new byte[MyHid.Capabilities.OutputReportByteLength];

                    //  Store the report ID (command) in the first byte of the buffer:

                    outputReportBuffer[0] = OUTPUT_REPORT_ID;      // will turn out on the board as Output Report ID

                    outputReportBuffer[1] = (byte)ProximityBoardCommand.ServoPositionSet;      // will be analyzed in a switch

                    outputReportBuffer[2] = (byte)servoNumber;      // which servo

                    int servotarget = ProximityBoard.mksToServoTarget(dPosMks);

                    // data bytes 0 and 1 of the Output Report - hid_report_out[]:
                    intToBuffer(outputReportBuffer, 3, (UInt16)servotarget);

                    ReadAndWriteToDevice(outputReportBuffer, INPUT_REPORT_ID);
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
                Tracer.Trace("PM Command::ServoSweepParams(" + servoNumber + "," + sweepMin + "," + sweepMax + "," + sweepStep + "," + sweepRate + ")");

                sweepMin = ProximityBoard.toWithinServoRangeMks(sweepMin);        // cut it to the allowed limits
                sweepMax = ProximityBoard.toWithinServoRangeMks(sweepMax);

                if (!myDeviceDetected)
                {
                    myDeviceDetected = FindTheHid(vendorId, productId);
                }

                if (myDeviceDetected)
                {
                    //  Set the size of the Output report buffer.   

                    byte[] outputReportBuffer = new byte[MyHid.Capabilities.OutputReportByteLength];

                    //  Store the report ID (command) in the first byte of the buffer:

                    outputReportBuffer[0] = OUTPUT_REPORT_ID;      // will turn out on the board as Output Report ID

                    outputReportBuffer[1] = (byte)ProximityBoardCommand.ServoSweepParams;      // will be analyzed in a switch

                    outputReportBuffer[2] = (byte)servoNumber;      // which servo

                    int iSweepMin = ProximityBoard.mksToServoTarget(sweepMin);
                    int iSweepMax = ProximityBoard.mksToServoTarget(sweepMax);
                    int iSweepStart = ProximityBoard.mksToServoTarget(sweepStartPos);
                    int iSweepStep = (int)sweepStep;
                    int iSweepRate = (int)sweepRate;

                    intToBuffer(outputReportBuffer, 3, (UInt16)iSweepMin);
                    intToBuffer(outputReportBuffer, 5, (UInt16)iSweepMax);
                    intToBuffer(outputReportBuffer, 7, (UInt16)iSweepStart);
                    intToBuffer(outputReportBuffer, 9, (UInt16)iSweepStep);
                    intToBuffer(outputReportBuffer, 11, (UInt16)iSweepRate);
                    outputReportBuffer[13] = (byte)(initialDirectionUp ? 1 : -1);

                    ReadAndWriteToDevice(outputReportBuffer, INPUT_REPORT_ID);
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

        public double ServoPositionGet(int servoNumber)
        {
            double dPosMks = 0;

            try
            {
                Tracer.Trace("PM Command::ServoPositionGet(" + servoNumber + ")");

                if (!myDeviceDetected)
                {
                    myDeviceDetected = FindTheHid(vendorId, productId);
                }

                if (myDeviceDetected)
                {
                    //  Set the size of the Output report buffer.   

                    byte[] outputReportBuffer = new byte[MyHid.Capabilities.OutputReportByteLength];

                    //  Store the report ID (command) in the first byte of the buffer:

                    outputReportBuffer[0] = OUTPUT_REPORT_ID;      // will turn out on the board as Output Report ID

                    outputReportBuffer[1] = (byte)ProximityBoardCommand.ServoPositionGet;      // will be analyzed in a switch

                    outputReportBuffer[2] = (byte)servoNumber;      // which servo

                    byte[] inputReportBuffer = ReadAndWriteToDevice(outputReportBuffer, INPUT_REPORT_ID);

                    // data bytes 1 and 2 of the Input Report contain servo position:

                    int servotarget = intFromBuffer(inputReportBuffer, 1);

                    dPosMks = ProximityBoard.servoTargetToMks(servotarget);

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

        /// <summary>
        /// returns distance in meters
        /// </summary>
        /// <param name="pingUnitNumber"></param>
        /// <returns></returns>
        public double PingDistanceGet(int pingUnitNumber)
        {
            double dPingDistanceM = 0;

            try
            {
                Tracer.Trace("PM Command::PingDistanceGet(" + pingUnitNumber + ")");

                if (!myDeviceDetected)
                {
                    myDeviceDetected = FindTheHid(vendorId, productId);
                }

                if (myDeviceDetected)
                {
                    //  Set the size of the Output report buffer.   

                    byte[] outputReportBuffer = new byte[MyHid.Capabilities.OutputReportByteLength];

                    //  Store the report ID (command) in the first byte of the buffer:

                    outputReportBuffer[0] = OUTPUT_REPORT_ID;      // will turn out on the board as Output Report ID

                    outputReportBuffer[1] = (byte)ProximityBoardCommand.PingDistanceGet;      // will be analyzed in a switch

                    outputReportBuffer[2] = (byte)pingUnitNumber;      // which servo

                    byte[] inputReportBuffer = ReadAndWriteToDevice(outputReportBuffer, INPUT_REPORT_ID);

                    // data bytes 1 and 2 of the Input Report contain ping response value:

                    int pingResponseValue = intFromBuffer(inputReportBuffer, 1);

                    dPingDistanceM = ProximityBoard.pingValueToDistanceM(pingResponseValue);

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

            return dPingDistanceM;
        }

        public void ServoSweepEnable(bool enable)
        {
            try
            {
                Tracer.Trace("PM Command::ServoSweepStart()");

                if (!myDeviceDetected)
                {
                    myDeviceDetected = FindTheHid(vendorId, productId);
                }

                if (myDeviceDetected)
                {
                    //  Set the size of the Output report buffer.   

                    byte[] outputReportBuffer = new byte[MyHid.Capabilities.OutputReportByteLength];

                    //  Store the report ID (command) in the first byte of the buffer:

                    outputReportBuffer[0] = OUTPUT_REPORT_ID;      // will turn out on the board as Output Report ID

                    outputReportBuffer[1] = (byte)(enable ? ProximityBoardCommand.ServoSweepStart : ProximityBoardCommand.ServoSweepStop);      // will be analyzed in a switch

                    ReadAndWriteToDevice(outputReportBuffer, INPUT_REPORT_ID);
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
                Tracer.Trace("PM Command::DataContinuousStart()");

                if (!myDeviceDetected)
                {
                    myDeviceDetected = FindTheHid(vendorId, productId);
                }

                if (myDeviceDetected)
                {
                    //  Set the size of the Output report buffer.   

                    byte[] outputReportBuffer = new byte[MyHid.Capabilities.OutputReportByteLength];

                    //  Store the report ID (command) in the first byte of the buffer:

                    outputReportBuffer[0] = OUTPUT_REPORT_ID;      // will turn out on the board as Output Report ID

                    outputReportBuffer[1] = (byte)ProximityBoardCommand.DataContinuousStart;      // will be analyzed in a switch

                    ReadAndWriteToDevice(outputReportBuffer, INPUT_REPORT_ID);

                    inDataContinuousMode = true;

                    ReadFromDevice(new EventHandler<AsyncInputReportArgs>(DataReadCompleteHandler), INPUT_CONT_REPORT_ID);

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
            // do some quick sanity checking here to avoid exceptions:

            uint parkingSensorsCount = (uint)aira.InputBuffer[23];

            switch (parkingSensorsCount)
            {
                case 0:
                case 32:
                case 64:
                    break;
                default:
                    return;
            }

            switch (aira.InputBuffer[9])
            {
                case 0:
                case 1:
                    break;
                default:
                    return;
            }
 
            /*
            Tracer.Trace("PM Command: Async data arrived. " + DateTime.Now);


            StringBuilder byteValue = new StringBuilder();
            byteValue.Append("DataReadCompleteHandler(): Input Report Data: ");

            for (int count = 0; count <= aira.InputBuffer.Length - 1; count++)
            {
                //  Display bytes as 2-character Hex strings.
                byteValue.AppendFormat("{0:X02} ", aira.InputBuffer[count]);
            }
            Tracer.Trace(byteValue.ToString());
            */

            int servo1target = intFromBuffer(aira.InputBuffer, 1);
            int servo2target = intFromBuffer(aira.InputBuffer, 3);

            int ping1value = intFromBuffer(aira.InputBuffer, 5);
            int ping2value = intFromBuffer(aira.InputBuffer, 7);

            SensorsState sensState = new SensorsState();

            bool fromPingScanStop = aira.InputBuffer[9] > 0;

            // infrared distance sensors:
            sensState.irbE1 = aira.InputBuffer[10];
            sensState.irbE2 = aira.InputBuffer[11];
            sensState.irbE3 = aira.InputBuffer[12];
            sensState.irbE4 = aira.InputBuffer[13];

            sensState.irbO1 = aira.InputBuffer[14];
            sensState.irbO2 = aira.InputBuffer[15];
            sensState.irbO3 = aira.InputBuffer[16];
            sensState.irbO4 = aira.InputBuffer[17];

            sensState.compassHeading = (((uint)aira.InputBuffer[18] << 8) + (uint)aira.InputBuffer[19]) / 10.0d;

            sensState.accelX = ProximityBoard.toAccel(aira.InputBuffer[20]);
            sensState.accelY = ProximityBoard.toAccel(aira.InputBuffer[21]);
            sensState.accelZ = ProximityBoard.toAccel(aira.InputBuffer[22]);

            // ultrasound car parking sensors - bytes 23 to 31 (only first 4 bytes used, next 4 are reserved for 8-sensor device):
            sensState.parkingSensorsCount = parkingSensorsCount;          // 32 or 0 for invalid
            for (int i = 0; i < parkingSensorsCount / 8; i++)
            {
                sensState.parkingSensors[i] = aira.InputBuffer[24 + i];
            }
            sensState.mapParkingSensorsData();

            sensState.mapPotValueData(aira.InputBuffer[28], aira.InputBuffer[29]);    // LSB, MSB

            // calibration for POT data (pin 2 RA0/AN0 on PIC4550):
            // 0v = 0
            // 1v = 220
            // 2v = 415
            // 3v = 630
            // 4v = 835
            // 4.88v = 1023

            AsyncInputFrameArgs args = new AsyncInputFrameArgs(servo1target, servo2target, ping1value, ping2value, fromPingScanStop, sensState);

            OnDataFrameComplete(args);

            if (inDataContinuousMode)
            {
                // initiate next read:
                ReadFromDevice(new EventHandler<AsyncInputReportArgs>(DataReadCompleteHandler), INPUT_CONT_REPORT_ID);
            }
        }

        public void DataContinuousStop()
        {
            try
            {
                Tracer.Trace("PM Command::DataContinuousStop()");

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

                    outputReportBuffer[0] = OUTPUT_REPORT_ID;      // will turn out on the board as Output Report ID

                    outputReportBuffer[1] = (byte)ProximityBoardCommand.DataContinuousStop;      // will be analyzed in a switch

                    ReadAndWriteToDevice(outputReportBuffer, INPUT_REPORT_ID);
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
