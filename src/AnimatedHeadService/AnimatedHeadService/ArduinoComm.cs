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
using System.Diagnostics;

using Microsoft.Ccr.Core;
using W3C.Soap;

using TrackRoamer.Robotics.Services.AnimatedHeadService.Properties;
using System.Threading;
using Trackroamer.Library.LibArduinoComm;

namespace TrackRoamer.Robotics.Services.AnimatedHeadService
{
    // Most of the code here is close to GpsComm.cs from Microsoft RDS R3

    // Data received from the Animated Head
    internal class AnimatedHeadDataPort : PortSet<string, string[], byte[], Exception> { }

    /// <summary>
    /// A CCR Port for managing an Animated Head serial port connection.
    /// </summary>
    internal class AnimatedHeadConnection : IArduinoComm
    {
        /// <summary>
        /// Default AnimatedHeadConnection constructor
        /// </summary>
        /// <param name="config"></param>
        /// <param name="ahDataPort"></param>
        public AnimatedHeadConnection(AnimatedHeadConfig config, AnimatedHeadDataPort ahDataPort)
        {
            this._config = config;
            this._ahDataPort = ahDataPort;
        }

        public bool Open(string comPort)
        {
            return true;
        }

        /// <summary>
        /// Open an Animated Head serial port.
        /// </summary>
        /// <param name="comPort"></param>
        /// <param name="baudRate"></param>
        /// <returns>true for success</returns>
        public bool Open(int comPort, int baudRate)
        {
            if (_serialPort != null)
                Close();

            if (!ValidBaudRate(baudRate))
                throw new System.ArgumentException(Resources.InvalidBaudRate, "baudRate");

            _serialPort = new SerialPort("COM" + comPort.ToString(CultureInfo.InvariantCulture), baudRate, Parity.None, 8, StopBits.One);
            _serialPort.Handshake = Handshake.None;
            _serialPort.Encoding = Encoding.ASCII;      // that's only for text read, not binary
            _serialPort.NewLine = "\r\n";               // that's only for text read, not binary
            _serialPort.ReadTimeout = 1100;

            if (_ahDataPort == null)
                _ahDataPort = new AnimatedHeadDataPort();

            bool serialPortOpened = false;
            try
            {
                if (comPort > 0)
                {
                    if (TrySerialPort(_serialPort.PortName, baudRate))
                    {
                        _serialPort.Open();
                        serialPortOpened = true;
                        _serialPort.DataReceived += new SerialDataReceivedEventHandler(serialPort_DataReceived);
                    }
                }
            }
            catch
            {
                if (serialPortOpened)
                {
                    serialPortOpened = false;
                    _serialPort.DataReceived -= new SerialDataReceivedEventHandler(serialPort_DataReceived);
                }
            }

            return serialPortOpened;
        }

        /// <summary>
        /// Attempt to find and open an Animated Head on any serial port.
        /// <returns>True if an Animated Head was found</returns>
        /// </summary>
        public bool FindAnimatedHead()
        {
            foreach (string spName in SerialPort.GetPortNames())
            {
                Console.WriteLine("IP: Checking for Animated Head on " + spName);
                if (TrySerialPort(spName))
                {
                    Console.WriteLine("OK: Animated Head found on " + spName + "    baud rate " + _config.BaudRate);
                    return Open(_config.CommPort, _config.BaudRate);
                }
            }

            return false;
        }

        /// <summary>
        /// Returns the serial port configuration
        /// </summary>
        public AnimatedHeadConfig AnimatedHeadConfig
        {
            get { return _config; }
        }

        #region Private data members

        /// <summary>
        /// Valid baud rates
        /// </summary>
        private static readonly int[] baudRates = { 57600 };

        /// <summary>
        /// The Animated Head hardware configuration
        /// </summary>
        private AnimatedHeadConfig _config;

        /// <summary>
        /// The low level Animated Head serial port
        /// </summary>
        private SerialPort _serialPort = new SerialPort();

        /// <summary>
        /// The CCR port for sending out Animated Head Data
        /// </summary>
        private AnimatedHeadDataPort _ahDataPort;

        #endregion

        #region Data Received Event Handler

        /// <summary>
        /// Serial Port data event handler
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void serialPort_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            if (e.EventType == SerialData.Chars)
            {
                int i = 0;
                string line;
                try
                {
                    i = _serialPort.BytesToRead;
                    while (_serialPort.BytesToRead > 25 && i > 10)
                    {
                        line = string.Empty;
                        line = _serialPort.ReadLine();
                        i -= (line.Length + 2);

                        Console.WriteLine(line);

                        int ix = line.IndexOf('*');
                        if (ix < 0)
                        {
                            _ahDataPort.Post(new InvalidDataException(Resources.InvalidAnimatedHeadStream));
                        }
                        else
                        {
                            if (ValidChecksum(line))
                            {
                                string[] fields = line.Substring(0, ix).Split(',');
                                _ahDataPort.Post(fields);
                            }
                            else
                                _ahDataPort.Post(new InvalidDataException(Resources.FailedChecksumValidation + line));
                        }
                    }

                }
                catch (ArgumentException ex)
                {
                    if (ex.Message == "Offset and length were out of bounds for the array or count is greater than the number of elements from index to the end of the source collection.")
                    {
                        Exception invalidBaudRate = new ArgumentException(Resources.InvalidBaudRate);
                        _ahDataPort.Post(invalidBaudRate);
                    }
                    else
                    {
                        Exception ex2 = new ArgumentException(Resources.ErrorReadingFromSerialPort, ex);
                        _ahDataPort.Post(ex2);
                    }
                }
                catch (TimeoutException ex)
                {
                    _ahDataPort.Post(ex);
                    // Ignore timeouts for now.
                }
                catch (IOException ex)
                {
                    string errorInfo = Resources.ErrorReadingFromSerialPort;
                    if (i > 0 && _serialPort.IsOpen)
                    {
                        byte[] b = new byte[i];
                        _serialPort.Read(b, 0, i);
                        StringBuilder sb = new StringBuilder(i);
                        foreach (byte c in b)
                            sb.Append((char)c);

                        errorInfo += "\r\nEnd of buffer: [" + sb.ToString() + "]";
                    }

                    Exception ex2 = new IOException(errorInfo, ex);
                    _ahDataPort.Post(ex2);

                }
                catch (Exception ex)
                {
                    _ahDataPort.Post(ex);
                }
            }
        }

        #endregion

        #region Public Methods

        public string getPortName()
        {
            return _serialPort.PortName;
        }

        public void SendToArduino(ToArduino toArduino)
        {
            Console.WriteLine("*** SendToArduino(): " + toArduino);
            _serialPort.WriteLine(toArduino.ToString());
        }

        public void SendToArduino2(ToArduino toArduino1, ToArduino toArduino2)
        {
            Console.WriteLine("*** SendToArduino2(): " + toArduino1 + "  :  " + toArduino2);
            _serialPort.WriteLine(toArduino1.ToString());
            _serialPort.WriteLine(toArduino2.ToString());
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Validate CHR UM6 Sensor checksum
        /// </summary>
        /// <param name="cmd"></param>
        /// <returns></returns>
        private static bool ValidChecksum(string cmd)
        {
            cmd = cmd.Trim();
            string originalChecksum = string.Empty;

            if (cmd.StartsWith("*", StringComparison.Ordinal))
                cmd = cmd.Substring(1);

            // TODO: calculate checksum

            return true;
        }

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
        public void Close()
        {
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
        /// Attempt to read Animated Head data from a serial port at any baud rate.
        /// </summary>
        /// <param name="spName"></param>
        /// <returns></returns>
        private bool TrySerialPort(string spName)
        {
            return TrySerialPort(spName, -1);   // try all baud rates from the "baudRates" list
        }

        private readonly char[] digits = { '0', '1', '2', '3', '4', '5', '6', '7', '8', '9' };

        /// <summary>
        /// Attempt to read Animated Head data from a serial port.
        /// </summary>
        /// <param name="spName"></param>
        /// <param name="tryBaud">when > 0, try only the specified baud rate</param>
        /// <returns></returns>
        private bool TrySerialPort(string spName, int tryBaud)
        {

            _config.PortName = spName;
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

            _config.CommPort = comPort;
            SerialPort p = null;
            foreach (int baud in baudRates)
            {
                if (tryBaud > 0 && baud != tryBaud)
                    continue;

                _config.BaudRate = baud;
                try
                {
                    p = new SerialPort(_config.PortName, _config.BaudRate, Parity.None, 8, StopBits.One);
                    p.Handshake = Handshake.None;
                    p.Encoding = Encoding.ASCII;
                    p.Open();
                    string s = p.ReadExisting();
                    p.WriteLine("*0 127 -127");         // expecting "*ARD COMM HEAD V1.0*" in return
                    s = p.ReadExisting();
                    if (s.Length == 0)
                    {
                        System.Threading.Thread.Sleep(1000);
                        s = p.ReadExisting();
                    }
                    if (s.Length == 0)
                        return false;

                    // we are receiving data.

                    bool hasSnp = s.Contains("*ARD COMM HEAD");

                    if (!hasSnp)
                    {
                        Thread.Sleep(1000);
                        s = p.ReadExisting();
                    }
                    hasSnp = s.Contains("*ARD COMM HEAD");

                    // Try another baud rate
                    if (!hasSnp)
                        continue;

                    return true;
                }
                catch (UnauthorizedAccessException)
                {
                    break;
                }
                catch { }
                finally
                {
                    if (p != null)
                    {
                        if (p.IsOpen)
                            p.Close();
                        p.Dispose();
                    }
                }
            }
            return false;
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
    }
}
