//-----------------------------------------------------------------------
//  This file is part of Microsoft Robotics Developer Studio Code Samples.
//
//  Copyright (C) Microsoft Corporation.  All rights reserved.
//
//  $File: GpsComm.cs $ $Revision: 1 $
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.IO.Ports;
using System.Text;

using Microsoft.Ccr.Core;
using Microsoft.Robotics.Services.Sensors.Gps;
using W3C.Soap;
using Microsoft.Robotics.Services.Sensors.Gps.Properties;


namespace Microsoft.Robotics.Services.Sensors.Gps
{
    // Data received from the Gps device
    internal class GpsDataPort : PortSet<string, string[], byte[], Exception> { }

    /// <summary>
    /// A Ccr Port for managing a Gps serial port connection.
    /// </summary>
    internal class GpsConnection
    {
        public bool captureNmea;
        public string nmeaFileName = string.Empty;

        /// <summary>
        /// Default GpsConnection constructor
        /// </summary>
        /// <param name="config"></param>
        /// <param name="gpsDataPort"></param>
        public GpsConnection(MicrosoftGpsConfig config, GpsDataPort gpsDataPort)
        {
            this._config = config;
            this._gpsDataPort = gpsDataPort;
        }

        /// <summary>
        /// Open a Gps Log file and return output to the Gps Service
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public bool Open(string fileName)
        {
            if (_serialPort != null)
                Close();

            if (_gpsDataPort == null)
                _gpsDataPort = new GpsDataPort();

            if (File.Exists(fileName))
            {
                StreamReader r = File.OpenText(fileName);
                while (!r.EndOfStream)
                {
                    string line = r.ReadLine().Trim();
                    int ix = line.IndexOf('*');
                    if (ix < 0)
                    {
                        _gpsDataPort.Post(new InvalidDataException(Resources.InvalidGpsStream));
                    }
                    else
                    {
                        if (ValidChecksum(line))
                        {
                            string[] fields = line.Substring(0, ix).Split(',');
                            _gpsDataPort.Post(fields);
                        }
                        else
                            _gpsDataPort.Post(new InvalidDataException(Resources.FailedChecksumValidation + line));
                    }
                }
                r.Close();
                return true;
            }
            return false;
        }

        /// <summary>
        /// Open a GPS serial port.
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
            _serialPort.Encoding = Encoding.ASCII;
            _serialPort.NewLine = "\r\n";
            _serialPort.ReadTimeout = 1100;

            if (_gpsDataPort == null)
                _gpsDataPort = new GpsDataPort();

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
        /// Attempt to find and open a Microsoft Gps on any serial port.
        /// <returns>True if a Gps unit was found</returns>
        /// </summary>
        public bool FindGps()
        {

            foreach (string spName in SerialPort.GetPortNames())
            {
                Console.WriteLine("Checking for Gps on " + spName);
                if (TrySerialPort(spName))
                {
                    return Open(_config.CommPort, _config.BaudRate);
                }
            }

            return false;
        }

        /// <summary>
        /// Returns the serial port configuration
        /// </summary>
        public MicrosoftGpsConfig MicrosoftGpsConfig
        {
            get { return _config; }
        }

        #region Private data members

        /// <summary>
        /// Hexadecimal digits
        /// </summary>
        private static char[] hexDigits = { '0', '1', '2', '3', '4', '5', '6', '7', '8', '9', 'A', 'B', 'C', 'D', 'E', 'F' };

        /// <summary>
        /// Valid baud rates
        /// </summary>
        private static readonly int[] baudRates = { 4800, 9600, 19200, 38400, 2400 };

        /// <summary>
        /// The Gps hardware configuration
        /// </summary>
        private MicrosoftGpsConfig _config;

        /// <summary>
        /// The low level Gps serial port
        /// </summary>
        private SerialPort _serialPort = new SerialPort();

        /// <summary>
        /// The CCR port for sending out Gps Data
        /// </summary>
        private GpsDataPort _gpsDataPort;

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
                        //System.Diagnostics.Debug.WriteLine(line);
                        int ix = line.IndexOf('*');
                        if (ix < 0)
                        {
                            _gpsDataPort.Post(new InvalidDataException(Resources.InvalidGpsStream));
                        }
                        else
                        {
                            if (ValidChecksum(line))
                            {
                                if (captureNmea && !string.IsNullOrEmpty(nmeaFileName))
                                {
                                    //Console.WriteLine(line);
                                    File.AppendAllText(nmeaFileName, line + "\r\n");
                                }
                                string[] fields = line.Substring(0, ix).Split(',');
                                _gpsDataPort.Post(fields);
                            }
                            else
                                _gpsDataPort.Post(new InvalidDataException(Resources.FailedChecksumValidation + line));
                        }
                    }

                }
                catch (ArgumentException ex)
                {
                    if (ex.Message == "Offset and length were out of bounds for the array or count is greater than the number of elements from index to the end of the source collection.")
                    {
                        Exception invalidBaudRate = new ArgumentException(Resources.InvalidBaudRate);
                        _gpsDataPort.Post(invalidBaudRate);
                    }
                    else
                    {
                        Exception ex2 = new ArgumentException(Resources.ErrorReadingFromSerialPort, ex);
                        _gpsDataPort.Post(ex2);
                    }
                }
                catch (TimeoutException ex)
                {
                    _gpsDataPort.Post(ex);
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
                    _gpsDataPort.Post(ex2);

                }
                catch (Exception ex)
                {
                    _gpsDataPort.Post(ex);
                }
            }
        }

        #endregion

        #region Public Methods
        /// <summary>
        /// Add a GPS checksum to the specified command
        /// </summary>
        /// <param name="cmd"></param>
        /// <returns></returns>
        public static string AddChecksum(string cmd)
        {
            cmd = cmd.Trim();

            if (cmd.StartsWith("$", StringComparison.Ordinal))
                cmd = cmd.Substring(1);

            int ix = cmd.LastIndexOf('*');
            if (ix >= 0)
                cmd = cmd.Substring(0, ix);

            int checkSum = 0;
            foreach (char c in cmd)
            {
                checkSum = checkSum ^ (int)c;
            }
            int checkSumHi = checkSum / 16;
            int checkSumLo = checkSum - (checkSumHi * 16);

            return string.Format(CultureInfo.InvariantCulture, "\r\n${0}*{1}{2}\r\n", cmd, hexDigits[checkSumHi], hexDigits[checkSumLo]);
        }
        #endregion

        #region Private Methods



        /// <summary>
        /// Validate GPS checksum
        /// </summary>
        /// <param name="cmd"></param>
        /// <returns></returns>
        private static bool ValidChecksum(string cmd)
        {
            cmd = cmd.Trim();
            string originalChecksum = string.Empty;

            if (cmd.StartsWith("$", StringComparison.Ordinal))
                cmd = cmd.Substring(1);

            int ix = cmd.LastIndexOf('*');
            if (ix >= 0)
            {
                originalChecksum = cmd.Substring(ix + 1);
                cmd = cmd.Substring(0, ix);
            }

            int checkSum = 0;
            foreach (char c in cmd)
            {
                checkSum = checkSum ^ (int)c;
            }
            int checkSumHi = checkSum / 16;
            int checkSumLo = checkSum - (checkSumHi * 16);

            string calcChecksum = string.Format(CultureInfo.InvariantCulture, "{0}{1}", hexDigits[checkSumHi], hexDigits[checkSumLo]);

            return (calcChecksum == originalChecksum);
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
        internal void Close()
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
        /// Attempt to read GPS data from a serial port at any baud rate.
        /// </summary>
        /// <param name="spName"></param>
        /// <returns></returns>
        private bool TrySerialPort(string spName)
        {
            return TrySerialPort(spName, -1);
        }

        private readonly char[] digits = { '0', '1', '2', '3', '4', '5', '6', '7', '8', '9' };

        /// <summary>
        /// Attempt to read GPS data from a serial port.
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
                    if (s.Length == 0)
                    {
                        System.Threading.Thread.Sleep(1000);
                        s = p.ReadExisting();
                    }
                    if (s.Length == 0)
                        return false;

                    // we are receiving data.

                    bool hasCrLf = s.Contains("\r\n");
                    bool invalidData = UnrecognizedData(s);

                    if (invalidData || !hasCrLf)
                    {
                        System.Threading.Thread.Sleep(1000);
                        s = p.ReadExisting();
                    }
                    hasCrLf = s.Contains("\r\n");

                    // Try another baud rate
                    if (!hasCrLf)
                        continue;

                    p.ReadTimeout = 2000;
                    s = p.ReadLine();
                    if (s.StartsWith("$GP", StringComparison.Ordinal))
                        return true;
                    s = p.ReadLine();
                    if (s.StartsWith("$GP", StringComparison.Ordinal))
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
