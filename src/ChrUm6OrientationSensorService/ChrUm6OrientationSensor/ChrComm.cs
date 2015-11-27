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

using TrackRoamer.Robotics.Hardware.ChrUm6OrientationSensor.Properties;
using TrackRoamer.Robotics.Hardware.ChrUm6OrientationSensor.ChrInterface;

namespace TrackRoamer.Robotics.Hardware.ChrUm6OrientationSensor
{
    // See UM6 datasheet for more info. Source code in C++ for working with UM6 is at http://sourceforge.net/projects/chrinterface/  (get SVN tarball there).
    // Most of the code here is close to GpsComm.cs from Microsoft RDS R3

    // Data received from the CHR UM6 Sensor
    internal class ChrDataPort : PortSet<string, string[], short[], Exception> { }

    /// <summary>
    /// A CCR Port for managing a CHR UM6 Sensor serial port connection.
    /// </summary>
    internal class ChrConnection
    {
        /// <summary>
        /// Default ChrConnection constructor
        /// </summary>
        /// <param name="config"></param>
        /// <param name="chrDataPort"></param>
        public ChrConnection(ChrUm6OrientationSensorConfig config, ChrDataPort chrDataPort)
        {
            this._config = config;
            this._chrDataPort = chrDataPort;
        }

        /// <summary>
        /// Open a Data Log file or a COM port and return output to the CHR UM6 Sensor Service
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public bool Open(string fileName)
        {
            if (_serialPort != null)
                Close();

            if (_chrDataPort == null)
                _chrDataPort = new ChrDataPort();

            if (File.Exists(fileName))
            {
                StreamReader r = File.OpenText(fileName);
                while (!r.EndOfStream)
                {
                    string line = r.ReadLine().Trim();
                    int ix = line.IndexOf('*');
                    if (ix < 0)
                    {
                        _chrDataPort.Post(new InvalidDataException(Resources.InvalidChrStream));
                    }
                    else
                    {
                        if (ValidChecksum(line))
                        {
                            string[] fields = line.Substring(0, ix).Split(',');
                            _chrDataPort.Post(fields);
                        }
                        else
                            _chrDataPort.Post(new InvalidDataException(Resources.FailedChecksumValidation + line));
                    }
                }
                r.Close();
                return true;
            }
            return false;
        }

        /// <summary>
        /// Open a CHR UM6 Sensor serial port.
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

            if (_chrDataPort == null)
                _chrDataPort = new ChrDataPort();

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
        /// Attempt to find and open a CHR UM6 Sensor on any serial port.
        /// <returns>True if a CHR UM6 Sensor was found</returns>
        /// </summary>
        public bool FindChr()
        {

            foreach (string spName in SerialPort.GetPortNames())
            {
                Console.WriteLine("IP: Checking for CH Robotics UM6 Orientation Sensor on " + spName);
                if (TrySerialPort(spName))
                {
                    Console.WriteLine("OK: CH Robotics UM6 Orientation Sensor found on " + spName + "    baud rate " + _config.BaudRate);
                    return Open(_config.CommPort, _config.BaudRate);
                }
            }

            return false;
        }

        /// <summary>
        /// Returns the serial port configuration
        /// </summary>
        public ChrUm6OrientationSensorConfig ChrUm6OrientationSensorConfig
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
        private static readonly int[] baudRates = { 9600, 19200, 38400, 115200 };

        /// <summary>
        /// The CHR UM6 Sensor hardware configuration
        /// </summary>
        private ChrUm6OrientationSensorConfig _config;

        /// <summary>
        /// The low level Chr serial port
        /// </summary>
        private SerialPort _serialPort = new SerialPort();

        /// <summary>
        /// The CCR port for sending out Chr Data
        /// </summary>
        private ChrDataPort _chrDataPort;

        #endregion

        #region OnSerialPacketReceived()

        private bool doTracePackets = false;

        public void OnSerialPacketReceived( SerialPacket packet )
        {
            //Console.WriteLine(string.Format("packet: {0} {1} {2}", packet.DataLength, packet.IsBatch ? " batch" : "", packet.Address));

            // see ...\ChrInterface\UM1B.XML for register addresses
            switch (packet.Address)
            {
                case 92:
                    /*
                     * expecting batch of two registers:
		                <Register address="92">
			                <Name>UM6_GYRO_PROC_XY</Name>
			                <Description>X and Y-axis rate gyro output after alignment, bias, and scale compensation have been applied. Stored as two 16-bit signed integers.</Description>
		                </Register>
		
		                <Register address="93">
			                <Name>UM6_GYRO_PROC_Z</Name>
			                <Description>Z-axis rate gyro output after alignment, bias, and scale compensation have been applied. Stored as 16-bit signed integer.</Description>
		                </Register>
                     */
                    {
                        // convert 8 bytes into 4 signed shorts:
                        short[] rates = new short[4];
                        rates[0] = packet.Address;

                        for(int i=0; i < 6 ;i+=2)
                        {
                            rates[i / 2 + 1] = (short)((packet.GetDataByte(i) << 8) + packet.GetDataByte(i+1));
                        }

                        if(doTracePackets) Console.WriteLine(string.Format("UM6_GYRO_PROC: {0} {1} {2}", rates[1], rates[2], rates[3]));

                        _chrDataPort.Post(rates);
                    }
                    break;

                case 94:
                    /*
                     * expecting batch of two registers:
		                <Register address="94">
			                <Name>UM6_ACCEL_PROC_XY</Name>
			                <Description>X and Y-axis accelerometer output after alignment, bias, and scale compensation have been applied. Stored as two 16-bit signed integers.</Description>
		                </Register>
		
		                <Register address="95">
			                <Name>UM6_ACCEL_PROC_Z</Name>
			                <Description>Z-axis accelerometer output after alignment, bias, and scale compensation have been applied. Stored as 16-bit signed integer.</Description>
		                </Register>
                     */
                    {
                        // convert 8 bytes into 4 signed shorts:
                        short[] accels = new short[4];
                        accels[0] = packet.Address;

                        for(int i=0; i < 6 ;i+=2)
                        {
                            accels[i / 2 + 1] = (short)((packet.GetDataByte(i) << 8) + packet.GetDataByte(i+1));
                        }

                        if (doTracePackets) Console.WriteLine(string.Format("UM6_ACCEL_PROC: {0} {1} {2}", accels[1], accels[2], accels[3]));

                        _chrDataPort.Post(accels);
                    }
                    break;

                case 96:
                    /*
                     * expecting batch of two registers:
		                <Register address="96">
			                <Name>UM6_MAG_PROC_XY</Name>
			                <Description>X and Y-axis magnetometer output after soft and hard iron calibration. Stored as 16-bit signed integers.</Description>
		                </Register>
		
		                <Register address="97">
			                <Name>UM6_MAG_PROC_Z</Name>
			                <Description>Z-axis magnetometer output after soft and hard iron calibration. Stored as 16-bit signed integer.</Description>
		                </Register>
                     */
                    {
                        // convert 8 bytes into 4 signed shorts:
                        short[] mags = new short[4];
                        mags[0] = packet.Address;

                        for(int i=0; i < 6 ;i+=2)
                        {
                            mags[i / 2 + 1] = (short)((packet.GetDataByte(i) << 8) + packet.GetDataByte(i+1));
                        }

                        if (doTracePackets) Console.WriteLine(string.Format("UM6_MAG_PROC: {0} {1} {2}", mags[1], mags[2], mags[3]));

                        _chrDataPort.Post(mags);
                    }
                    break;

                case 98:
                    /*
                     * expecting batch of two registers:
		                <Register address="98">
			                <Name>UM6_EULER_PHI_THETA</Name>
			                <Description>Roll and pitch angles.  Stored as 16-bit signed integers.</Description>
		                </Register>
		
		                <Register address="99">
			                <Name>UM6_EULER_PSI</Name>
			                <Description>Yaw angle in degrees.  Stored as 16-bit signed integer.</Description>
		                </Register>
                     */
                    {
                        // convert 8 bytes into 4 signed shorts:
                        short[] eulers = new short[4];
                        eulers[0] = packet.Address;

                        for(int i=0; i < 6 ;i+=2)
                        {
                            eulers[i / 2 + 1] = (short)((packet.GetDataByte(i) << 8) + packet.GetDataByte(i+1));
                        }

                        if (doTracePackets) Console.WriteLine(string.Format("UM6_EULER: PHI={0}  THETA={1}  PSI={2}", eulers[1], eulers[2], eulers[3]));

                        _chrDataPort.Post(eulers);
                    }
                    break;

                case 100:
                    /*
                     * expecting batch of two registers:
                        <Register address="100">
                            <Name>UM6_QUAT_AB</Name>
                            <Description>First two elements of attitude quaternion. Stored as two 16-bit signed integers.</Description>
                        </Register>
		
                        <Register address="101">
                            <Name>UM6_QUAT_CD</Name>
                            <Description>Third and fourth elements of attitude quaternion. Stored as two 16-bit signed integers.</Description>
                        </Register>
                     */
                    {
                        // convert 8 bytes into 4 signed shorts:
                        short[] quaternion = new short[5];
                        quaternion[0] = packet.Address;

                        for (int i = 0; i < 8; i += 2)
                        {
                            quaternion[i / 2 + 1] = (short)((packet.GetDataByte(i) << 8) + packet.GetDataByte(i + 1));
                        }

                        if (doTracePackets) Console.WriteLine(string.Format("UM6_QUAT: {0} {1} {2} {3}", quaternion[1], quaternion[2], quaternion[3], quaternion[4]));

                        _chrDataPort.Post(quaternion);
                    }
                    break;

                default:
                    //if (doTracePackets)
                    Console.WriteLine(string.Format("Unexpected packet: Address={0}   descr={1:x4}   HasData={2}   IsBatch={3}   BatchLength={4}   DataLength={5}   CommandFailed={6}",
                                                        packet.Address, packet.PacketDescriptor, packet.HasData, packet.IsBatch, packet.BatchLength, packet.DataLength, packet.CommandFailed));
                    break;
            }

        }

        #endregion // OnSerialPacketReceived()

        #region Data Received Event Handler

        // C:\Projects\Robotics\src\CHR Interface\CHR Interface\SerialConnector.h
        private const int	RX_BUFFER_SIZE		=1000;
        private const int	RX_PACKET_BUF_SIZE	=20;
        private const int	TX_PACKET_BUF_SIZE	=20;

        private const int	MAX_PACKET_BYTES	=100;

        private byte[] RXBuffer = new byte[RX_BUFFER_SIZE];
    	private int RXBufPtr;		// Points to the index in the serial buffer where the next item should be placed

        /// <summary>
        /// Serial Port data event handler
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void serialPort_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            if (e.EventType == SerialData.Chars)
            {
                int BytesReturned;
                bool found_packet;
                int packet_start_index;
                int packet_index;
                int bytes_to_read = 0;

                // See UM6 datasheet for more info. Source code in C++ for working with UM6 is at http://sourceforge.net/projects/chrinterface/  (get SVN tarball there)
                // The code below is direct translation from C++
                // See C:\Projects\Robotics\src\CHR Interface\CHR Interface\SerialConnector.cpp

                try
                {
                    bytes_to_read = _serialPort.BytesToRead;

                    if (bytes_to_read + RXBufPtr >= RX_BUFFER_SIZE)
                    {
                        bytes_to_read = RX_BUFFER_SIZE - 1 - RXBufPtr;
                    }

                    BytesReturned = _serialPort.Read(RXBuffer, RXBufPtr, bytes_to_read);

                    RXBufPtr += BytesReturned;

                    // If there are enough bytes in the buffer to construct a full packet, then check data.
                    // There are RXbufPtr bytes in the buffer at any given time
                    while (RXBufPtr >= 7)
                    {
                        // Search for the packet start sequence
                        found_packet = false;
                        packet_start_index = 0;
                        int skippedBytes = 0;
                        for (packet_index = 0; packet_index < (RXBufPtr - 2); packet_index++)
                        {
                            if (RXBuffer[packet_index] == 's' && RXBuffer[packet_index + 1] == 'n' && RXBuffer[packet_index + 2] == 'p')
                            {
                                found_packet = true;
                                packet_start_index = packet_index;
                                break;
                            }
                            skippedBytes++;
                        }

                        // for debugging, see if we are skipping any bytes between the packets:
                        //if (skippedBytes > 0)
                        //{
                        //    Console.WriteLine("skipped " + skippedBytes);
                        //}

		                // If packet start sequence was not found, then remove all but the last three bytes from the buffer.  This prevents
		                // bad data from filling the buffer up.
		                if( !found_packet )
		                {
			                RXBuffer[0] = RXBuffer[RXBufPtr-3];
			                RXBuffer[1] = RXBuffer[RXBufPtr-2];
			                RXBuffer[2] = RXBuffer[RXBufPtr-1];

			                RXBufPtr = 3;
		                }

		                // If a packet start sequence was found, extract the packet descriptor byte.
		                // Make sure that there are enough bytes in the buffer to consitute a full packet
		                int indexed_buffer_length = RXBufPtr - packet_start_index;
		                if (found_packet && (indexed_buffer_length >= 7))
		                {
			                byte packet_descriptor = RXBuffer[packet_start_index + 3];
			                byte address = RXBuffer[packet_start_index + 4];

			                // Check the R/W bit in the packet descriptor.  If it is set, then this packet does not contain data 
			                // (the packet is either reporting that the last write operation was succesfull, or it is reporting that
			                // a command finished).
			                // If the R/W bit is cleared and the batch bit B is cleared, then the packet is 11 bytes long.  Make sure
			                // that the buffer contains enough data to hold a complete packet.
			                bool HasData;
			                bool IsBatch;
			                int BatchLength;

			                int packet_length = 0;

			                if( ( (packet_descriptor & 0x80) != 0 ) )       // Has Data?
			                {
				                HasData = true;
			                }
			                else
			                {
				                HasData = false;
			                }

			                if( ( (packet_descriptor & 0x40) != 0 ) )       // Is Batch? (always is, two registers packed together, for example 100 and 101 for quaternions).
			                {
				                IsBatch = true;
			                }
			                else
			                {
				                IsBatch = false;
			                }
			
			                if( HasData && !IsBatch )
			                {
				                packet_length = 11;
			                }
			                else if( HasData && IsBatch )
			                {
                                // from the Datasheet p.24:
                                // Do some bit-level manipulation to determine if the packet contains data and if it is a batch
                                // We have to do this because the individual bits in the PT byte specify the contents of the packet.
                                // uint8_t packet_has_data = (PT >> 7) & 0x01; // Check bit 7 (HAS_DATA)
                                // uint8_t packet_is_batch = (PT >> 6) & 0x01; // Check bit 6 (IS_BATCH)
                                // uint8_t batch_length = (PT >> 2) & 0x0F; // Extract the batch length (bits 2 through 5)

				                // If this is a batch operation, then the packet length is: length = 5 + 4*L + 2, where L is the length of the batch.
				                // Make sure that the buffer contains enough data to parse this packet.
				                BatchLength = (packet_descriptor >> 2) & 0x0F;
				                packet_length = 5 + 4*BatchLength + 2;				
			                }
			                else if( !HasData )
			                {
				                packet_length = 7;
			                }

			                if( indexed_buffer_length < packet_length )
			                {
				                return;
			                }

			                SerialPacket NewPacket = new SerialPacket();

			                // If code reaches this point, then there enough bytes in the RX buffer to form a complete packet.
			                NewPacket.Address = address;
			                NewPacket.PacketDescriptor = packet_descriptor;

			                // Copy data bytes into packet data array:
			                int data_start_index = packet_start_index + 5;
			                for( int i = 0; i < NewPacket.DataLength; i++ )
			                {
				                NewPacket.SetDataByte( i, RXBuffer[data_start_index + i] );
			                }

			                // Now extract the expected checksum from the packet:
			                UInt16 Checksum = (UInt16)(((UInt16)RXBuffer[packet_start_index + packet_length - 2] << 8) | ((UInt16)RXBuffer[packet_start_index + packet_length - 1]));

			                // Compute the checksum on our side and compare with the one given in the packet.  If different, ignore this packet.
			                NewPacket.ComputeChecksum();

			                if( Checksum == NewPacket.Checksum )
			                {
				                OnSerialPacketReceived( NewPacket );
			                }
			                else
			                {
                                string message = string.Format("Received packet with bad checksum {0} (expected {1}). Packet discarded.", Checksum, NewPacket.Checksum);

                                // debug:
                                Console.WriteLine("Error: " + message);

                                _chrDataPort.Post(new InvalidDataException(Resources.FailedChecksumValidation + " " + message));
                            }

			                // At this point, the newest packet has been parsed and copied into the RXPacketBuffer array.
			                // Copy all received bytes that weren't part of this packet into the beginning of the
                            // buffer.  Then, reset RXbufPtr.
                            for (int index = 0; index < (RXBufPtr - (packet_start_index + packet_length)); index++)
                            {
                                RXBuffer[index] = RXBuffer[(packet_start_index + packet_length) + index];
                            }

			                RXBufPtr -= (packet_start_index + packet_length);
		                }
		                else
		                {
			                return;
		                }
                    }               // end while RXBufPtr >= 7

                }
                catch (ArgumentException ex)
                {
                    if (ex.Message == "Offset and length were out of bounds for the array or count is greater than the number of elements from index to the end of the source collection.")
                    {
                        Exception invalidBaudRate = new ArgumentException(Resources.InvalidBaudRate);
                        _chrDataPort.Post(invalidBaudRate);
                    }
                    else
                    {
                        Exception ex2 = new ArgumentException(Resources.ErrorReadingFromSerialPort, ex);
                        _chrDataPort.Post(ex2);
                    }
                }
                catch (TimeoutException ex)
                {
                    _chrDataPort.Post(ex);
                    // Ignore timeouts for now.
                }
                catch (IOException ex)
                {
                    string errorInfo = Resources.ErrorReadingFromSerialPort;
                    if (bytes_to_read > 0)
                    {
                        byte[] b = new byte[bytes_to_read];
                        int ii = _serialPort.Read(b, 0, bytes_to_read);

                        errorInfo += "\r\nEnd of buffer: " + ii + " bytes wasted";
                    }

                    Exception ex2 = new IOException(errorInfo, ex);
                    _chrDataPort.Post(ex2);

                }
                catch (Exception ex)
                {
                    _chrDataPort.Post(ex);
                }
            }
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// sends command to UM6 device
        /// </summary>
        /// <param name="command"></param>
        public void SendCommand(string command)
        {
            byte address = 0;

            switch (command)
            {
                case "ZeroRateGyros":
                    address = SerialPacket.UM6_ZERO_GYROS;          // see UM2B.XML - UM6_ZERO_GYROS - "Zero Rate Gyros"
                    // expect packet 11 in response () - two data words, "Bias of x-axis rate gyro" and "Bias of y-axis rate gyro"
                    break;

                case "SetAccelRefVector":
                    address = SerialPacket.UM6_SET_ACCEL_REF;       // see UM2B.XML - UM6_SET_ACCEL_REF - "Set Accelerometer Reference Vector"
                    // no response expected.
                    break;

                case "SetMagnRefVector":
                    address = SerialPacket.UM6_SET_MAG_REF;         // see UM2B.XML - UM6_SET_MAG_REF - "Set Magnetometer Reference Vector"
                    // no response expected. The Euler values and quaternion will slowly rotate to have current robot position show as
                    // Magnetic North (0 degrees). It will take ~10 sec.
                    break;
            }

            if(address != 0)
            {
                // Commands are initiated by executing a read command for the relevant command address using the UART.

                SerialPacket NewPacket = new SerialPacket(address);

                // If code reaches this point, then there enough bytes in the RX buffer to form a complete packet.
                NewPacket.PacketDescriptor = 0; // data length 0, not a batch, doesn't have data

                byte[] bytes = NewPacket.ToByteArray();     // will also compute checksum

                _serialPort.Write(bytes, 0, bytes.Length);
            }
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
        /// Attempt to read CHR UM6 Sensor data from a serial port at any baud rate.
        /// </summary>
        /// <param name="spName"></param>
        /// <returns></returns>
        private bool TrySerialPort(string spName)
        {
            return TrySerialPort(spName, -1);   // try all baud rates from the "baudRates" list
        }

        private readonly char[] digits = { '0', '1', '2', '3', '4', '5', '6', '7', '8', '9' };

        /// <summary>
        /// Attempt to read CHR UM6 Sensor data from a serial port.
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

                    bool hasSnp = s.Contains("snp");

                    if (!hasSnp)
                    {
                        System.Threading.Thread.Sleep(1000);
                        s = p.ReadExisting();
                    }
                    hasSnp = s.Contains("snp");

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
