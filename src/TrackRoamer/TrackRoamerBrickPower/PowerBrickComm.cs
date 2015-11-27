/*
* Copyright (c) 2011..., Sergei Grichine
* All rights reserved.
*
* Redistribution and use in source and binary forms, with or without
* modification, are permitted provided that the following conditions are met:
*     * Redistributions of source code must retain the above copyright
*       notice, this list of conditions and the following disclaimer.
*     * Redistributions in binary form must reproduce the above copyright
*       notice, this list of conditions and the following disclaimer in the
*       documentation and/or other materials provided with the distribution.
*     * Neither the name of Sergei Grichine nor the
*       names of contributors may be used to endorse or promote products
*       derived from this software without specific prior written permission.
*
* THIS SOFTWARE IS PROVIDED BY Sergei Grichine ''AS IS'' AND ANY
* EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED
* WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE
* DISCLAIMED. IN NO EVENT SHALL Sergei Grichine BE LIABLE FOR ANY
* DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES
* (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES;
* LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND
* ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT
* (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS
* SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
*
* this is a X11 (BSD Revised) license - you do not have to publish your changes,
* although doing so, donating and contributing is always appreciated
*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.IO;
using System.IO.Ports;
using System.Globalization;

using Microsoft.Ccr.Core;
using W3C.Soap;

using TrackRoamer.Robotics.Hardware.LibRoboteqController;
using TrackRoamer.Robotics.Services.TrackRoamerBrickPower.Properties;

namespace TrackRoamer.Robotics.Services.TrackRoamerBrickPower
{
    // Data received from RoboteQ AX2850 controller
    internal class RbtqDataPort : PortSet<string, string[], short[], Exception> { }

    /// <summary>
    /// A CCR Port for managing a RoboteQ AX2850 controller serial port connection.
    /// </summary>
    internal class BrickConnection
    {
        /// <summary>
        /// Default BrickConnection constructor
        /// </summary>
        /// <param name="config"></param>
        /// <param name="rbtqDataPort"></param>
        public BrickConnection(PowerControllerConfig config, RbtqDataPort rbtqDataPort, TrackRoamerBrickPowerService service)
        {
            this.PowerControllerConfig = config;                  // keep the pointer
            this._rbtqDataPort = rbtqDataPort;      // where to send incoming data
            this._service = service;                // used for tracing

            _service.LogInfoViaService("BrickConnection()");

            _controller = new ControllerRQAX2850_CCR(PowerControllerConfig.PortName);

            // wireup handlers for drive-related data - encoders and whiskers:
            _controller.onValueReceived_EncoderLeftAbsolute += new OnValueReceived(m_controller_onValueReceived_EncoderLeftAbsolute);
            _controller.onValueReceived_EncoderRightAbsolute += new OnValueReceived(m_controller_onValueReceived_EncoderRightAbsolute);

            _controller.onValueReceived_EncoderSpeed += new OnValueReceived(m_controller_onValueReceived_EncoderSpeed);

            _controller.onValueReceived_DigitalInputF += new OnValueReceived(m_controller_onValueReceived_DigitalInputF);           // WhiskerLeft
            _controller.onValueReceived_DigitalInputEmerg += new OnValueReceived(m_controller_onValueReceived_DigitalInputEmerg);   // WhiskerRight

            _controller.onValueReceived_DigitalInputE += new OnValueReceived(_controller_onValueReceived_DigitalInputE);

            // some measured values coming from the controller as result of queries:
            _controller.onValueReceived_Voltage += new OnValueReceived(_controller_onValueReceived_Voltage);
            _controller.onValueReceived_MotorPower += new OnValueReceived(_controller_onValueReceived_MotorPower);
            _controller.onValueReceived_MotorAmps += new OnValueReceived(_controller_onValueReceived_MotorAmps);
            _controller.onValueReceived_AnalogInputs += new OnValueReceived(_controller_onValueReceived_AnalogInputs);
            _controller.onValueReceived_HeatsinkTemperature += new OnValueReceived(_controller_onValueReceived_HeatsinkTemperature);

            HcConnected = false;
        }

        ~BrickConnection()
        {
            if (_controller != null)
            {
                _controller.Dispose();
                _controller = null;
            }
        }

        public string StatusLabel { get { return _controller.CurrentStatusLabel; } }     // "grabbed", "monitored", "online - receiving data", "not connected" etc.

        private int _comPort = -1;
        private int _baudRate;

        public bool ReOpen()
        {
            if (_comPort < 1 || _baudRate < 4800)
                return false;

            return Open(_comPort, _baudRate);
        }

        /// <summary>
        /// Open a RoboteQ AX2850 serial port.
        /// </summary>
        /// <param name="comPort"></param>
        /// <param name="baudRate"></param>
        /// <returns>true for success</returns>
        public bool Open(int comPort, int baudRate)
        {
            _comPort = comPort;
            _baudRate = baudRate;

            _service.LogInfoViaService("BrickConnection: Open() port=" + comPort + "  baud=" + baudRate);

            if (_serialPort != null)
            {
                Close();    // will set  _serialPort=null  and remove event handler (serialPort_DataReceived)
            }

            if (!ValidBaudRate(baudRate))
            {
                throw new System.ArgumentException(Resources.InvalidBaudRate, "baudRate");
            }

            _serialPort = new SerialPort("COM" + comPort.ToString(CultureInfo.InvariantCulture), baudRate, Parity.Even, 7, StopBits.One);
            _serialPort.Handshake = Handshake.None;
            _serialPort.Encoding = Encoding.ASCII;
            _serialPort.NewLine = "\r";
            _serialPort.DtrEnable = true;
            _serialPort.RtsEnable = true;
            _serialPort.ReadTimeout = 1100;

            if (_rbtqDataPort == null)
            {
                _rbtqDataPort = new RbtqDataPort();
            }

            bool serialPortOpened = false;
            try
            {
                if (comPort > 0)
                {
                    if (TrySerialPort(_serialPort.PortName, baudRate))
                    {
                        // Open the port for communications
                        _serialPort.Open();
                        _serialPort.DiscardInBuffer();
                        _serialPort.DataReceived += new SerialDataReceivedEventHandler(serialPort_DataReceived);
                        serialPortOpened = true;

                        _controller.serialPort = _serialPort;
                        _controller.init(); // serialPort must be set to avoid reopening in ensurePort()
                    }
                }
            }
            catch(Exception exc)
            {
                _service.LogInfoViaService("Error in BrickConnection: Open() port=" + comPort + "  baud=" + baudRate + " " + exc);

                if (serialPortOpened)
                {
                    serialPortOpened = false;
                    _serialPort.DataReceived -= new SerialDataReceivedEventHandler(serialPort_DataReceived);
                }
            }

            return serialPortOpened;
        }

        /// <summary>
        /// Attempt to find and open a Power Controller (RoboteQ AX2850) on any serial port.
        /// <returns>True if a RoboteQ AX2850 was found</returns>
        /// </summary>
        public bool FindPowerController()
        {
            _service.LogInfoViaService("BrickConnection: FindPowerController()");

            foreach (string spName in SerialPort.GetPortNames())
            {
                _service.LogInfoViaService("IP: Checking for RoboteQ AX2850 on " + spName);
                if (TrySerialPort(spName))
                {
                    _service.LogInfoViaService("OK: RoboteQ AX2850 found on " + spName + "    baud rate " + PowerControllerConfig.BaudRate);
                    return Open(PowerControllerConfig.CommPort, PowerControllerConfig.BaudRate);
                }
            }

            return false;
        }

        /// <summary>
        /// The RoboteQ AX2850 hardware configuration. Points to caller's _state.PowerControllerConfig
        /// </summary>
        public PowerControllerConfig PowerControllerConfig { get; set; }

        #region Private data members

        /// <summary>
        /// Valid baud rates - actually AX2850 allows 9600 by default.
        /// </summary>
        private static readonly int[] baudRates = { 9600 };

        /// <summary>
        /// The low level RoboteQ AX2850 serial port
        /// </summary>
        private SerialPort _serialPort = null;

        /// <summary>
        /// The CCR port for sending out RoboteQ AX2850 Data
        /// </summary>
        private RbtqDataPort _rbtqDataPort;

        private ControllerRQAX2850_CCR _controller = null;

        private TrackRoamerBrickPowerService _service = null;

        internal long frameCounter = 0;
        internal long errorCounter = 0;

        // drive supporting values:
        public event OnValueReceived onValueReceived_WhiskerLeft;
        public event OnValueReceived onValueReceived_WhiskerRight;

        public event OnValueReceived onValueReceived_EncoderLeftAbsolute;
        public event OnValueReceived onValueReceived_EncoderRightAbsolute;

        public event OnValueReceived onValueReceived_EncoderSpeed;

        // other measured values:
        public event OnValueReceived onValueReceived_AnalogInputs;
        public event OnValueReceived onValueReceived_DigitalInputE;
        public event OnValueReceived onValueReceived_Voltage;
        public event OnValueReceived onValueReceived_MotorPower;
        public event OnValueReceived onValueReceived_MotorAmps;
        public event OnValueReceived onValueReceived_HeatsinkTemperature;

        public bool HcConnected { get; private set; }

        public bool isHcInError { get { return _controller == null ? false : _controller.isInError; } }

        #endregion

        #region Data Received Event Handler

        /// <summary>
        /// Serial Port data event handler
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void serialPort_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            //_service.LogInfoViaService("BrickConnection: serialPort_DataReceived() e.EventType=" + e.EventType);

            if (e.EventType == SerialData.Chars)
            {
                if (_controller != null)
                {
                    try
                    {
                        //  may be monitoring line that comes from controller in R/C mode:
                        //      :0000000000FE00009898396AB7B70000  - page 103 of controller doc
                        //      the 9898 is temperature 1 and 2
                        //      the 396A is voltages 
                        //      the 0000 at the end is speed - 00 left 00 right

                        // will be ignored if not in "grabbed" state;

                        // will be passed to m_currentQueue.onStringReceived() if that queue is processing interaction.
                        // such processing will result in calling "...onValueReceived...()" handlers.

                        _controller.port_DataReceived(sender, e);
                    }
                    catch (ArgumentException ex)
                    {
                        if (ex.Message == "Offset and length were out of bounds for the array or count is greater than the number of elements from index to the end of the source collection.")
                        {
                            Exception invalidBaudRate = new ArgumentException(Resources.InvalidBaudRate);
                            _rbtqDataPort.Post(invalidBaudRate);
                        }
                        else
                        {
                            Exception ex2 = new ArgumentException(Resources.ErrorReadingFromSerialPort, ex);
                            _rbtqDataPort.Post(ex2);
                        }
                    }
                    catch (TimeoutException ex)
                    {
                        _rbtqDataPort.Post(ex);
                        // Ignore timeouts for now.
                    }
                    catch (IOException ex)
                    {
                        string errorInfo = Resources.ErrorReadingFromSerialPort;
                        Exception ex2 = new IOException(errorInfo, ex);
                        _rbtqDataPort.Post(ex2);
                    }
                    catch (Exception ex)
                    {
                        _rbtqDataPort.Post(ex);
                    }
                }
            }
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Validates the specified baud rate
        /// </summary>
        /// <param name="baudRate"></param>
        /// <returns></returns>
        public static bool ValidBaudRate(int baudRate)
        {
            foreach (int validRate in baudRates)
                if (baudRate == validRate)
                    return true;

            return false;
        }

        /// <summary>
        /// Close the connection to a serial port.
        /// </summary>
        internal void Close()
        {
            _service.LogInfoViaService("BrickConnection: Close()");

            if (_serialPort != null)
            {
                if (_serialPort.IsOpen)
                {
                    _serialPort.DataReceived -= new SerialDataReceivedEventHandler(serialPort_DataReceived);
                    try
                    {
                        _serialPort.Close();
                    }
                    catch
                    {
                    }
                }

                _serialPort = null;
            }
        }

        /// <summary>
        /// Attempt to read RoboteQ AX2850 data from a serial port at any baud rate.
        /// </summary>
        /// <param name="spName"></param>
        /// <returns></returns>
        private bool TrySerialPort(string spName)
        {
            return TrySerialPort(spName, -1);   // try all baud rates from the "baudRates" list
        }

        private readonly char[] digits = { '0', '1', '2', '3', '4', '5', '6', '7', '8', '9' };

        /// <summary>
        /// Attempt to read RoboteQ AX2850 data from a serial port.
        /// </summary>
        /// <param name="spName"></param>
        /// <param name="tryBaud">when > 0, try only the specified baud rate</param>
        /// <returns></returns>
        private bool TrySerialPort(string spName, int tryBaud)
        {
            _service.LogInfoViaService("BrickConnection: TrySerialPort() port=" + spName + "  baud=" + tryBaud);

            PowerControllerConfig.PortName = spName;
            int comPort = 0;
            if (!int.TryParse(spName.Substring(3), out comPort))
            {
                int ixStart = spName.IndexOfAny(digits);
                int ixEnd = ixStart;
                while (ixStart >= 0 && ixEnd < spName.Length && spName[ixEnd] >= '0' && spName[ixEnd] <= '9')
                    ixEnd++;
                if (ixStart >= 0)
                    comPort = int.Parse(spName.Substring(ixStart, ixEnd - ixStart), CultureInfo.InvariantCulture);
            }

            PowerControllerConfig.CommPort = comPort;

            // see C:\Microsoft Robotics Dev Studio 4\projects\MicrosoftGps\GpsComm.cs : 410 for port search example. We don't do it here. Just assume the port is good to open.

            PowerControllerConfig.BaudRate = tryBaud > 0 ? tryBaud : 9600;

            return true;
        }

        /// <summary>
        /// Search serial port data for binary characters.
        /// Unrecognized data indicates that we are
        /// operating in binary mode or receiving data at
        /// the wrong baud rate.
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        private static bool UnrecognizedData(string data)
        {
            foreach (char c in data)
            {
                if ((c < 32 || c > 120) && (c != 10 && c != 13))
                {
                    return true;
                }
            }
            return false;
        }

        #endregion

        #region HardwareController high-level operations

        #region HardwareController Lifecycle

        public bool ReConnect(out string errorMessage)
        {
            _service.LogInfoViaService("BrickConnection:Connect(PowerControllerConfig.PortName=" + PowerControllerConfig.PortName + ")  Connected=" + HcConnected);

            HcConnected = _controller.isOnline;

            try
            {
                //if (_serialPort != null && !_serialPort.IsOpen)
                //{
                //    if (ReOpen())
                //    {
                //        HcConnected = _controller.isOnline;
                //    }
                //}

                errorMessage = string.Empty;

                if (HcConnected)
                {
                    _service.LogInfoViaService("connected to " + PowerControllerConfig.PortName);
                }
                else
                {
                    bool needReOpen = false;
                    string msg = "Warning: not connected to " + PowerControllerConfig.PortName;
                    if (_serialPort != null)
                    {
                        msg += "    _serialPort exists";
                        if (_serialPort.IsOpen)
                        {
                            msg += " and open";
                        }
                        else
                        {
                            needReOpen = true;
                        }
                    }
                    else
                    {
                        needReOpen = true;
                    }

                    _service.LogInfoViaService(msg + "  needReOpen=" + needReOpen);

                    if (needReOpen)
                    {
                        if (ReOpen())
                        {
                            HcConnected = _controller.isOnline;
                        }
                    }
                }

                return HcConnected;
            }
            catch (Exception ex)
            {
                errorMessage = string.Format("Error connecting TrackRoamerBrickPower to port {0}: {1}", PowerControllerConfig.PortName, ex.Message);
                return false;
            }
        }

        #endregion // HardwareController Lifecycle

        #region HardwareController incoming data events - drive support

        void m_controller_onValueReceived_DigitalInputEmerg(object sender, MeasuredValuesEventArgs ev)
        {
            if (onValueReceived_WhiskerRight != null)
            {
                onValueReceived_WhiskerRight(this, ev);
            }
        }

        void m_controller_onValueReceived_DigitalInputF(object sender, MeasuredValuesEventArgs ev)
        {
            if (onValueReceived_WhiskerLeft != null)
            {
                onValueReceived_WhiskerLeft(this, ev);
            }
        }

        void m_controller_onValueReceived_EncoderLeftAbsolute(object sender, MeasuredValuesEventArgs ev)
        {
            if (onValueReceived_EncoderLeftAbsolute != null)
            {
                onValueReceived_EncoderLeftAbsolute(this, ev);
            }
        }

        void m_controller_onValueReceived_EncoderRightAbsolute(object sender, MeasuredValuesEventArgs ev)
        {
            if (onValueReceived_EncoderRightAbsolute != null)
            {
                onValueReceived_EncoderRightAbsolute(this, ev);
            }
        }

        void m_controller_onValueReceived_EncoderSpeed(object sender, MeasuredValuesEventArgs ev)
        {
            if (onValueReceived_EncoderSpeed != null)
            {
                onValueReceived_EncoderSpeed(this, ev);
            }
        }

        #endregion // HardwareController incoming data events - drive support

        #region Handlers for other measured values coming from the controller

        void _controller_onValueReceived_AnalogInputs(object sender, MeasuredValuesEventArgs ev)
        {
            // values are: "Analog_Input_1", "Analog_Input_2"

            if (onValueReceived_AnalogInputs != null)
            {
                onValueReceived_AnalogInputs(this, ev);
            }
        }

        void _controller_onValueReceived_DigitalInputE(object sender, MeasuredValuesEventArgs ev)
        {
            // values are: "Digital_Input_E"

            if (onValueReceived_DigitalInputE != null)
            {
                onValueReceived_DigitalInputE(this, ev);
            }
        }

        void _controller_onValueReceived_Voltage(object sender, MeasuredValuesEventArgs ev)
        {
            // values are: "Main_Battery_Voltage", "Internal_Voltage"

            if (onValueReceived_Voltage != null)
            {
                onValueReceived_Voltage(this, ev);
            }
        }

        void _controller_onValueReceived_MotorAmps(object sender, MeasuredValuesEventArgs ev)
        {
            // values are: "Motor_Amps_Left", "Motor_Amps_Right"

            if (onValueReceived_MotorAmps != null)
            {
                onValueReceived_MotorAmps(this, ev);
            }
        }

        void _controller_onValueReceived_MotorPower(object sender, MeasuredValuesEventArgs ev)
        {
            // values are: "Motor_Power_Left", "Motor_Power_Right"

            if (onValueReceived_MotorPower != null)
            {
                onValueReceived_MotorPower(this, ev);
            }
        }

        void _controller_onValueReceived_HeatsinkTemperature(object sender, MeasuredValuesEventArgs ev)
        {
            // values are: "HeatsinkTemperature_Left", "HeatsinkTemperature_Right"

            if (onValueReceived_HeatsinkTemperature != null)
            {
                onValueReceived_HeatsinkTemperature(this, ev);
            }
        }

        #endregion // Handlers for measured values coming from the controller

        #region Actuator methods - commanding controller actions (i.e. SetSpeed, ResetEncoder)

        /// <summary>
        /// Set Speed  -1.0 to 1.0
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        public void SetSpeed(double? left, double? right)
        {
            //_service.LogInfoViaService("BrickConnection:SetSpeed(L=" + left + ",R=" + right + ")");

            if ((left != null && (left < -1.0d || left > 1.0d))
                || ((right != null) && (right < -1.0d || right > 1.0d)))
            {
                throw new SystemException("Invalid Speed!");
            }

            if (left != null && _controller != null)
            {
                int speedLeft = (int)(left * 127.0d);
                _controller.SetMotorPowerOrSpeedLeft(speedLeft);
            }

            if (right != null && _controller != null)
            {
                int speedRight = (int)(right * 127.0d);
                _controller.SetMotorPowerOrSpeedRight(speedRight);
            }
        }

        public void ResetEncoderLeft()
        {
            //_service.LogInfoViaService("BrickConnection : ResetEncoderLeft()");

            _controller.ResetEncoderLeft();
        }

        public void ResetEncoderRight()
        {
            //_service.LogInfoViaService("BrickConnection : ResetEncoderRight()");

            _controller.ResetEncoderRight();
        }

        public void SetOutputC(bool on)
        {
            _service.LogInfoViaService("BrickConnection : SetOutputC(" + on + ")");

            _controller.SetOutputC(on);
        }

        #endregion // Actuator methods - commanding controller actions (i.e. SetSpeed, ResetEncoder)

        private DateTime m_lastGrabAttempt = DateTime.Now;
        private int m_betweenGrabsSec = 5;

        internal void ExecuteMain()
        {
            //_service.LogTrace("BrickConnection:ExecuteMain()");

            if (_controller != null)
            {
                try
                {
                    if (_controller.isOnline)
                    {
                        if (!_controller.isGrabbed && DateTime.Now > m_lastGrabAttempt.AddSeconds(m_betweenGrabsSec))
                        {
                            _controller.tenCRcnt = 0;           // kick _controller.GrabController();
                            m_lastGrabAttempt = DateTime.Now;
                        }

                        _controller.GrabController();
                    }

                    _controller.ExecuteMain();

                    this.HcConnected = _controller.isOnline;

                    frameCounter = _controller.frameCounter;
                    errorCounter = _controller.errorCounter;
                }
                catch (Exception exc)
                {
                    _service.LogInfoViaService("BrickConnection:ExecuteMain(): " + exc);
                }
            }
        }

        #endregion // HardwareController high-level operations
    }
}
