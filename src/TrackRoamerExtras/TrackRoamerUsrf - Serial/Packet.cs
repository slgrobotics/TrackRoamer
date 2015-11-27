//-----------------------------------------------------------------------
//  This file is part of Microsoft Robotics Developer Studio Code Samples.
//
//  Copyright (C) Microsoft Corporation.  All rights reserved.
//
//  $File: Packet.cs $ $Revision: 1 $
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Text;
using System.IO.Ports;
using System.Diagnostics;
using Microsoft.Dss.ServiceModel.Dssp;
using System.Xml;
using Microsoft.Dss.Services.ConsoleOutput;
using Microsoft.Dss.ServiceModel.DsspServiceBase;

using TrackRoamer.Robotics.Utility.LibSystem;

namespace TrackRoamer.Robotics.Services.TrackRoamerUsrf
{
    internal class Packet : SonarData
    {
    }

    /*
    internal class Packet
    {
        byte[] _data;

        #region Constructors

        public Packet(List<byte> data)
        {
            _data = data.ToArray();
        }

        Packet(byte stx, byte address, byte command)
        {
            _data = new byte[7];

            _data[0] = stx;
            _data[1] = address;
            Write(2, 1);
            _data[4] = command;
            Write(5, CalculatedChecksum);
        }

        Packet(byte stx, byte address, byte[] command)
        {
            _data = new byte[6 + command.Length];
            _data[0] = stx;
            _data[1] = address;
            Write(2, (ushort)command.Length);
            command.CopyTo(_data, 4);
            Write(4 + command.Length, CalculatedChecksum);
        }

        #endregion

        #region Component accessors
        public byte STX
        {
            get { return _data[0]; }
        }
        public byte Address
        {
            get { return _data[1]; }
        }
        public ushort Length
        {
            get { return ReadUshort(2); }
        }
        public byte Command
        {
            get { return _data[4]; }
        }
        public byte Response
        {
            get { return _data[4]; }
        }
        public byte[] Data
        {
            get
            {
                byte[] data = new byte[Length];

                Array.Copy(_data, 4, data, 0, Length);

                return data;
            }
        }
        public ushort Checksum
        {
            get { return ReadUshort(4 + Length); }
        }
        public ushort CalculatedChecksum
        {
            get { return CreateCRC(_data, 0, 4 + Length); }
        }

        public bool GoodChecksum
        {
            get
            {
                return CalculatedChecksum == Checksum;
            }
        }
        #endregion

        #region static Constructors

        public static Packet InitializeAndReset
        {
            get
            {
                return new Packet(0x02, 0x00, 0x10);
            }
        }

        public static Packet MonitoringMode(byte mode)
        {
            return new Packet(0x02, 0x00, new byte[] { 0x20, mode });
        }

        public static Packet RequestMeasured(byte mode)
        {
            return new Packet(0x02, 0x00, new byte[] {0x30, mode});
        }

        public static Packet Status
        {
            get
            {
                return new Packet(0x02, 0x00, 0x31);
            }
        }

        #endregion

        #region IO
        public void Send(SerialPort port)
        {
            Tracer.Trace("Packet::Send()");

            //port.Write(_data, 0, _data.Length);
        }

        public static Packet Read(SerialPort port)
        {
            Tracer.Trace("Packet::Read()");

            try
            {
                List<byte> data = new List<byte>();

                byte b = 0;

                while (b != 0x02)
                {
                    if (port.BytesToRead == 0)
                    {
                        return null;
                    }
                    b = (byte)port.ReadByte();
                }
                // STX
                data.Add(b);
                // Address
                data.Add((byte)port.ReadByte());
                // Low Length
                data.Add((byte)port.ReadByte());
                // High Length
                data.Add((byte)port.ReadByte());

                ushort length = MakeUshort(data[2], data[3]);

                for (int i = 0; i < length; i++)
                {
                    data.Add((byte)port.ReadByte());
                }

                // Low Checksum
                data.Add((byte)port.ReadByte());
                // High Checksum
                data.Add((byte)port.ReadByte());

                return new Packet(data);
            }
            catch
            {
                return null;
            }
        }
        #endregion

        #region Utility Functions

        public ushort ReadUshort(int index)
        {
            return (ushort)(_data[index] | (_data[index + 1] << 8));
        }

        void Write(int index, ushort value)
        {
            _data[index] = (byte)(value & 0xFF);
            _data[index + 1] = (byte)((value >> 8) & 0xFF);
        }

        public static ushort MakeUshort(byte LowByte, byte HighByte)
        {
            return (ushort)(LowByte | (HighByte << 8));
        }
        #endregion

        #region CRC Function
        const ushort CRC16_GEN_POL = 0x8005;
        static ushort CreateCRC(byte[] CommData, int start, int end)
        {
            ushort crc;
            byte low, high;

            crc = 0;
            low = 0;

            for (int index = start; index < end; index++)
            {
                high = low;
                low = CommData[index];

                if ((crc & 0x8000) == 0x8000)
                {
                    crc = (ushort)((crc & 0x7FFF) << 1);
                    crc ^= CRC16_GEN_POL;
                }
                else
                {
                    crc <<= 1;
                }
                crc ^= MakeUshort(low, high);
            }
            return crc;
        }
        #endregion

    }
    */

    internal class PacketBuilder
    {
        List<byte> _data = new List<byte>();
        Queue<Packet> _packets = new Queue<Packet>();

        enum State
        {
            STX,
            Address,
            LowLength,
            HighLength,
            Body,
            LowChecksum,
            HighChecksum
        }

        State _state;
        int _length;
        int _dropped;
        int _total;
        bool _missedFirst;
        int _badCount;
        string _parent;
        ConsoleOutputPort _console;

        public PacketBuilder()
        {
            Tracer.Trace("PacketBuilder::PacketBuilder()");

            _dropped = 0;
            _total = 0;
            _state = State.STX;
            _missedFirst = false;
            _badCount = 0;
        }

        /// <summary>
        /// hopefully not crippled string came in
        /// </summary>
        /// <param name="str"></param>
        private void onStringReceived(string str, long timestamp)
        {
            try
            {
                interpretCsaString(str, timestamp);
            }
            catch (Exception exc)
            {
                Tracer.Error(exc.ToString());
            }
        }

        private void interpretCsaString(string str, long timestamp)
        {
            string[] tokens = str.Split(new char[] { ' ' });

            int i = 0;

            while (i < tokens.GetLength(0))
            {
                string token = tokens[i];

                //Tracer.Trace(token); // + " " + tokens[i + 1] + " " + tokens[i + 2] + " " + tokens[i + 3]);

                switch (token)
                {
                    case "ACC":
                        interpretAccelerationData(Convert.ToDouble(tokens[i + 1]), Convert.ToDouble(tokens[i + 2]), Convert.ToDouble(tokens[i + 3]), timestamp);
                        i += 3;
                        break;

                    case "HDG":
                        interpretCompassData(Convert.ToDouble(tokens[i + 1]), timestamp);
                        i += 1;
                        break;

                    case "SON":
                        interpretSonarData(Convert.ToInt32(tokens[i + 1]), Convert.ToDouble(tokens[i + 2]), timestamp);
                        i += 3;
                        break;
                }
                i++;
            }
        }

        private const double GfCnv = 0.022d;	// 0.022 puts 1G = 9.8

        private void interpretAccelerationData(double accX, double accY, double accZ, long timestamp)
        {
            /*
            accX *= GfCnv;
            accY *= GfCnv;
            accZ *= GfCnv;

            this.accelXLabel.Text = String.Format("{0}", accX);
            this.accelYLabel.Text = String.Format("{0}", accY);
            this.accelZLabel.Text = String.Format("{0}", accZ);

            robotViewControl1.setAccelerometerData(accX, accY, accZ);

            if (oscTransmitter != null)
            {
                OSCMessage oscAccelData = new OSCMessage("/accel-g");
                oscAccelData.Append((float)(accX * 5.0d));
                oscAccelData.Append((float)(accY * 5.0d));
                oscAccelData.Append((float)(accZ * 5.0d));

                oscTransmitter.Send(oscAccelData);
            }
             * */
        }

        double lastHeading = -1;

        private void interpretCompassData(double heading, long timestamp)
        {
            /*
            heading /= 10.0d;
            this.compassDataLabel.Text = String.Format("{0}", heading);
            if (lastHeading != heading)
            {
                compassBearing = heading;
                this.compassViewControl1.CompassBearing = compassBearing;
                lastHeading = heading;
            }
             * */
        }

        private void interpretSonarData(int angleRaw, double distCm, long timestamp)
        {
            //Tracer.Trace("PacketBuilder::interpretSonarData() Bearing=" + String.Format("{0}", angleRaw) + "    RangeCM=" + String.Format("{0}", distCm));

            setReading(angleRaw, 0.01d * distCm, timestamp);
        }

        private int numNumbers;
        private long timestampLastReading = 0L;
        public Packet sonarData = new Packet();
        int lastAngleRaw = 0;       // actually width of the pulse to the servo - from 150 to 1150

        public void setReading(int angleRaw, double rangeMeters, long timestamp)
        {
            //Tracer.Trace("sonar: " + angleRaw + "   " + rangeMeters);

            if (angleRaw < lastAngleRaw)
            {
                _packets.Enqueue(sonarData);
                _badCount = 0;

                Tracer.Trace("PACKET READY -- angles: " + sonarData.angles.Count + "  packets: " + _packets.Count);
                numNumbers = sonarData.angles.Count;

                sonarData = new Packet();   // prepare for the next one
            }

            lastAngleRaw = angleRaw;

            timestampLastReading = timestamp;

            sonarData.addRangeReading(angleRaw, rangeMeters, timestamp);
        }


        private StringBuilder m_sb = new StringBuilder(256);
        
        private int m_wCnt = 0;		// watchdog "W" consecutive count to make sure we've grabbed the controller all right

        public void Add(byte[] buffer, int length)
        {
            Encoding enc = Encoding.ASCII;

            //string str = enc.GetString(buffer);
            //Tracer.Trace("PacketBuilder::Add() str=" + str);

            long timestamp = DateTime.Now.Ticks;
            int dropped = 0;

            lock (m_sb)
            {
                for (int index = 0; index < length; index++)
                {
                    byte b = buffer[index];

                    byte[] bb = new byte[1];
                    bb[0] = b;
                    string RxString = enc.GetString(bb);
                    if (RxString == "\r")
                    {
                        string strData = m_sb.ToString();
                        //Tracer.Trace("PacketBuilder::Add() str=" + strData);
                        onStringReceived(strData, timestamp);
                        m_sb.Remove(0, m_sb.Length);
                    }
                    else
                    {
                        m_wCnt = 0;
                        m_sb.Append(RxString);
                    }

                    /*
                    switch (_state)
                    {
                        case State.STX:
                            if (b == 0x02)
                            {
                                _state = State.Address;
                                _data.Add(b);
                                if (_missedFirst)
                                {
                                    LogInfo("SickLRF: packet sync problem, found new STX");
                                    _missedFirst = false;
                                }
                            }
                            else
                            {
                                if (!_missedFirst)
                                {
                                    LogInfo("SickLRF: packet sync problem (no STX), resyncing");
                                    _missedFirst = true;
                                }
                                dropped++;
                            }
                            break;

                        case State.Address:
                            _state = State.LowLength;
                            _data.Add(b);
                            break;

                        case State.LowLength:
                            _state = State.HighLength;
                            _data.Add(b);
                            _length = b;
                            break;

                        case State.HighLength:
                            _data.Add(b);
                            _length |= b << 8;
                            if (_length >= 1024)
                            {
                                LogInfo("SickLRF Packet length too big, {0} bytes",
                                    _length);

                                dropped += _data.Count;
                                _badCount++;

                                _state = State.STX;
                                _data.Clear();
                            }
                            else if (_length > 0)
                            {
                                _state = State.Body;
                            }
                            else
                            {
                                _state = State.LowChecksum;
                            }
                            break;

                        case State.Body:
                            _data.Add(b);
                            _length--;
                            if (_length == 0)
                            {
                                _state = State.LowChecksum;
                            }
                            break;

                        case State.LowChecksum:
                            _data.Add(b);
                            _state = State.HighChecksum;
                            break;

                        case State.HighChecksum:
                            _data.Add(b);
                            Packet p = new Packet(_data);

                            if (p.GoodChecksum)
                            {
                                _packets.Enqueue(p);
                                _badCount = 0;
                            }
                            else
                            {
                                LogInfo("SickLRF Bad Checksum: packet {0}, calc {1}",
                                    p.Checksum,
                                    p.CalculatedChecksum);
                                dropped += _data.Count;
                                _badCount++;
                            }

                            _data.Clear();
                            _state = State.STX;
                            break;
                    }
                     * */
                }
            }

            _total += length;
            _dropped += dropped;

            if (_total > 0x10000)
            {
                _dropped = _dropped * 4 / 5;
                _total = _total * 4 / 5;
            }

            if (dropped > 0)
            {
                LogInfo("SickLRF Noise: {0}:{1}", dropped, length);
            }
        }

        public bool HasPacket
        {
            get
            {
                return _packets.Count > 0;
            }
        }

        public int BadPackets
        {
            get
            {
                return _badCount;
            }
        }

        public Packet RemovePacket()
        {
            return _packets.Dequeue();
        }

        public int Noise
        {
            get { return 100 * _dropped / _total; }
        }

        public string Parent
        {
            get { return _parent; }
            set { _parent = value; }
        }

        public ConsoleOutputPort Console
        {
            get { return _console; }
            set { _console = value; }
        }

        void LogInfo(string format, params object[] args)
        {
            string msg = string.Format(format, args);

            DsspServiceBase.Log(
                TraceLevel.Info,
                TraceLevel.Info,
                new XmlQualifiedName("PacketBuilder", Contract.Identifier),
                _parent,
                msg,
                null,
                _console);
        }
    }
}
