//-----------------------------------------------------------------------
//  This file is part of Microsoft Robotics Developer Studio Code Samples.
//
//  Copyright (C) Microsoft Corporation.  All rights reserved.
//
//  $File: CommLink.cs $ $Revision: 1 $
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO.Ports;
using System.Text;
using System.Xml;

using Microsoft.Ccr.Core;
using Microsoft.Dss.Core;
using Microsoft.Dss.Core.Attributes;
using Microsoft.Dss.ServiceModel.Dssp;
using Microsoft.Dss.ServiceModel.DsspServiceBase;
using Microsoft.Dss.Services.ConsoleOutput;

using TrackRoamer.Robotics.Utility.LibSystem;

namespace TrackRoamer.Robotics.Services.TrackRoamerUsrf
{
    internal class LRFCommLinkPort : PortSet<LinkMeasurement, LinkPowerOn, LinkReset, LinkConfirm, LinkStatus, Exception>
    {
    }

    internal class LinkMeasurement
    {
        public DateTime TimeStamp;
        public Units Units;
        public int[] Ranges;
        public int ScanIndex;
        public int TelegramIndex;
    }

    /// <summary>
    /// Units of measure
    /// </summary>
    [DataContract]
    public enum Units
    {
        /// <summary>
        /// Centimeters
        /// </summary>
        Centimeters,
        /// <summary>
        /// Millimeters
        /// </summary>
        Millimeters
    }

    internal class LinkPowerOn
    {
        public LinkPowerOn()
        {
        }

        public LinkPowerOn(string description)
        {
            Description = description;
        }

        public string Description;
    }

    internal class LinkReset
    {
    }

    internal class LinkConfirm
    {
    }

    internal class CommLink : CcrServiceBase, IDisposable
    {
        LRFCommLinkPort _internalPort;
        SerialIOManager _serial;
        string _parent;
        ConsoleOutputPort _console;
        string _description;
        string _portName;
        int _rate;

        public new DispatcherQueue TaskQueue
        {
            get { return base.TaskQueue; }
        }

        public CommLink(DispatcherQueue dispatcherQueue, string port, LRFCommLinkPort internalPort)
            : base(dispatcherQueue)
        {
            Tracer.Trace("CommLink::CommLink() port=" + port);

            _internalPort = internalPort;
            _portName = port;

            _serial = new SerialIOManager(dispatcherQueue, _portName);
            Activate<ITask>(
                Arbiter.Receive<Packet>(true, _serial.Responses, PacketHandler),
                Arbiter.Receive<Exception>(true, _serial.Responses, ExceptionHandler)
            );
        }

        public string Parent
        {
            get { return _parent; }
            set
            {
                _parent = value;
                _serial.Parent = value;
            }
        }

        public ConsoleOutputPort Console
        {
            get { return _console; }
            set
            {
                _console = value;
                _serial.Console = value;
            }
        }

        SuccessFailurePort Send(Packet packet)
        {
            Tracer.Trace("CommLink::Send()");

            SerialIOManager.Send send = new SerialIOManager.Send();
            send.Packet = packet;

            _serial.OperationsPort.Post(send);

            return send.ResponsePort;
        }

        public SuccessFailurePort Initialize()
        {
            Tracer.Trace("CommLink::Initialize()");

            if (_serial.BaudRate != 9600)
            {
                _rate = 9600;
            }
            return new SuccessFailurePort(); // Send(Packet.InitializeAndReset);
        }

        /// <summary>
        /// Sets the Baud rate used to communicate with the LRF.
        /// Acceptable values are 38400, 19200 and 9600
        /// </summary>
        public SuccessFailurePort SetDataRate(int rate)
        {
            Tracer.Trace("CommLink::SetDataRate() rate=" + rate);

            /*
            Packet packet;
            switch (rate)
            {
                case 38400:
                    packet = Packet.MonitoringMode(0x40);
                    break;

                case 19200:
                    packet = Packet.MonitoringMode(0x41);
                    break;

                case 9600:
                    packet = Packet.MonitoringMode(0x42);
                    break;

                default:
                    SuccessFailurePort port = new SuccessFailurePort();
                    port.Post(new ArgumentException("Baud Rate (only 9600, 19200 and 38400 supported)"));
                    return port;
            }
            _rate = rate;
            return Send(packet);
            */

            return new SuccessFailurePort();
        }

        /// <summary>
        /// Gets the Baud rate used to communicate with the LRF.
        /// Acceptable values are 38400, 19200 and 9600
        /// </summary>
        public int BaudRate
        {
            get { return _serial.BaudRate; }
        }

        public string Description
        {
            get { return _description; }
        }

        void PacketHandler(Packet packet)
        {
            //Tracer.Trace("CommLink::PacketHandler()");

            DateTime timeStamp = DateTime.Now;

            OnMeasurement(packet, timeStamp);

            /*
            if (packet == null || !packet.GoodChecksum)
            {
                return;
            }
            switch (packet.Response)
            {
                case 0x91:
                    LogInfo("Reset");
                    OnReset(packet);
                    break;
                case 0x90:
                    LogInfo("Power On");
                    OnPowerOn(packet);
                    break;
                case 0xA0:
                    LogInfo("Confirm");
                    OnConfirm(packet);
                    break;
                case 0xB0:
                    OnMeasurement(packet, timeStamp);
                    break;
                case 0xB1:
                    LogInfo("Status");
                    OnStatus(packet);
                    break;
                default:
                    LogInfo("Unknown Packet: {0}", packet.Response);
                    break;
            }
            */
        }

        void LogInfo(string format, params object[] args)
        {
            string msg = string.Format(format, args);

            DsspServiceBase.Log(TraceLevel.Info,
                                TraceLevel.Info,
                                new XmlQualifiedName("CommLink", Contract.Identifier),
                                _parent,
                                msg,
                                null,
                                _console);
        }

        void ExceptionHandler(Exception e)
        {
            Tracer.Trace("CommLink::ExceptionHandler() exc=" + e);

            _internalPort.Post(e);
        }

        private void OnStatus(Packet p)
        {
            Tracer.Trace("CommLink::OnStatus()");

            // _internalPort.Post(new LinkStatus(p.Data));
        }

        private void OnReset(Packet p)
        {
            Tracer.Trace("CommLink::OnReset()");

            // SetRate();

            // _internalPort.Post(new LinkReset());
        }

        private void OnConfirm(Packet p)
        {
            Tracer.Trace("CommLink::OnConfirm()");

            // _internalPort.Post(new LinkConfirm());
        }

        public SuccessFailurePort SetRate()
        {
            Tracer.Trace("CommLink::SetRate()");

            SuccessFailurePort port;
            port = new SuccessFailurePort();

            /*
            if (_rate != 0)
            {
                SerialIOManager.SetRate setRate = new SerialIOManager.SetRate(_rate);

                _serial.OperationsPort.Post(setRate);

                port = setRate.ResponsePort;
            }
            else
            {
                port = new SuccessFailurePort();
                port.Post(new Exception("Rate not set"));
            }
            */

            return port;
        }


        private void OnMeasurement(Packet p, DateTime TimeStamp)
        {
            //Tracer.Trace("CommLink::OnMeasurement()");

            int packetLength = p.angles.Count;      // typically 26 packets, each covering 7 degrees

            if (packetLength != 26)
            {
                Tracer.Error("CommLink::OnMeasurement()  not a standard measurement, angles.Count=" + packetLength + "  (expected 26) -- ignored");
                return;
            }

            LinkMeasurement lsd = new LinkMeasurement();
            lsd.TimeStamp = TimeStamp;
            lsd.Units = Units.Millimeters;
            int angularRange = 180;
            int mesLength = angularRange + 1;       // 181
            lsd.Ranges = new int[mesLength];        // millimeters, with 1 degree resolution
            List<double> ranges = new List<double>();

            foreach (int angle in p.angles.Keys)
            {
                RangeReading rr = p.getLatestReadingAt(angle);
                double range = rr.rangeMeters * 1000.0d;        // millimeters
                ranges.Add(range);
                //Tracer.Trace("&&&&&&&&&&&&&&&&&&&&&&&&&&&& angle=" + angle + " range=" + range);
                /*
                 * typical measurement:
                    PACKET READY -- angles: 26  packets: 1
                     angle=150 range=1440
                     angle=190 range=1450
                     angle=230 range=1450
                     angle=270 range=1450
                     angle=310 range=1460
                     angle=350 range=1540
                     angle=390 range=1540
                     angle=430 range=1700
                     angle=470 range=1700
                     angle=510 range=1740
                     angle=550 range=2260
                     angle=590 range=1100
                     angle=630 range=1100
                     angle=670 range=1090
                     angle=710 range=1100
                     angle=750 range=1090
                     angle=790 range=1090
                     angle=830 range=1090
                     angle=870 range=1090
                     angle=910 range=1700
                     angle=950 range=1710
                     angle=990 range=1730
                     angle=1030 range=1720
                     angle=1070 range=1710
                     angle=1110 range=3500
                     angle=1150 range=3500
                */
            }

            int step = (int)Math.Round((double)mesLength / (double)packetLength);   // typically round(6.96) = 7

            // if we smooth the measurements, Decide() has better chance of sorting the values right. It does not like 7 degrees steps. 
            // we need these for exponential moving average:
            double emaPeriod = 4.0d;
            double emaMultiplier = 2.0d / (1.0d + emaPeriod);
            double? emaPrev = null;

            for (int i = 0; i < mesLength; i++)
            {
                int angleIndex = i / step;

                //ushort range = (ushort)(i * 40);
                ushort range = (ushort)Math.Round(ranges[angleIndex]);

                if (range > 0x1FF7)
                {
                    range = 0x2000;
                }

                // calculate exponential moving average - smooth the curve a bit:
                double? ema = !emaPrev.HasGoodValue() ? range : ((range - emaPrev) * emaMultiplier + emaPrev);
                emaPrev = ema;

                int iRange = (int)Math.Round((double)ema);

                //Tracer.Trace("&&&&&&&&&&&&&&&&&&&&&&&&&&&&   i=" + i + " range=" + range + " ema=" + iRange);

                //lsd.Ranges[i] = range; // 5000; // milimeters
                lsd.Ranges[i] = iRange;  // 5000; // milimeters
            }

            lsd.ScanIndex = -1;
            lsd.TelegramIndex = -1;

            _internalPort.Post(lsd);

            /*
            byte[] data = p.Data;
            LinkMeasurement lsd = new LinkMeasurement();
            lsd.TimeStamp = TimeStamp;

            ushort lengthAndFlags = Packet.MakeUshort(data[1], data[2]);
            int length = lengthAndFlags & 0x3FF;

            switch (lengthAndFlags >> 14)
            {
                case 0:
                    lsd.Units = Units.Centimeters;
                    break;
                case 1:
                    lsd.Units = Units.Millimeters;
                    break;
                default:
                    return;
            }

            lsd.Ranges = new int[length];

            int offset = 3;
            for (int i = 0; i < length; i++, offset += 2)
            {
                ushort range = Packet.MakeUshort(data[offset], data[offset + 1]);
                if (range > 0x1FF7)
                {
                    range = 0x2000;
                }
                lsd.Ranges[i] = range;
            }


            if (offset < p.Length - 1)
            {
                lsd.ScanIndex = data[offset++];
            }
            else
            {
                lsd.ScanIndex = -1;
            }
            if (offset < p.Length - 1)
            {
                lsd.TelegramIndex = data[offset++];
            }
            else
            {
                lsd.TelegramIndex = -1;
            }

            _internalPort.Post(lsd);
            */
        }


        private void OnPowerOn(Packet p)
        {
            Tracer.Trace("CommLink::OnPowerOn()");

            /*
            _description = "";
            byte[] data = p.Data;
            int length = data.Length;

            for (int i = 1; i < length - 1; i++)
            {
                _description = _description + ((char)data[i]);
            }

            _internalPort.Post(new LinkPowerOn(_description));
             */
        }

        #region IDisposable Members

        public void Dispose()
        {
            Tracer.Trace("CommLink::Dispose()");

            Close();
        }

        #endregion

        public SuccessFailurePort Open()
        {
            Tracer.Trace("CommLink::Open()");

            SerialIOManager.Open open = new SerialIOManager.Open();

            _serial.OperationsPort.Post(open);

            return open.ResponsePort;
        }

        public SuccessFailurePort Close()
        {
            Tracer.Trace("CommLink::Close()");

            SerialIOManager.Close close = new SerialIOManager.Close();

            _serial.OperationsPort.Post(close);

            return close.ResponsePort;
        }

        /*
        public SuccessFailurePort SetContinuous()
        {
            Tracer.Trace("CommLink::SetContinuous()");

            return Send(Packet.MonitoringMode(0x24));
        }

        public SuccessFailurePort StopContinuous()
        {
            Tracer.Trace("CommLink::StopContinuous()");

            return Send(Packet.MonitoringMode(0x25));
        }

        public SuccessFailurePort MeasureOnce()
        {
            Tracer.Trace("CommLink::MeasureOnce()");

            return Send(Packet.RequestMeasured(0x01));
        }

        public SuccessFailurePort RequestStatus()
        {
            Tracer.Trace("CommLink::RequestStatus()");

            return Send(Packet.Status);
        }
        */
    }

    internal class LinkStatus
    {
        private byte[] _data;

        // 0-5
        public string SoftwareVersion { get { return MakeString(0, 7); } }
        // 7
        public byte OperatingMode { get { return _data[7]; } }
        // 8
        public byte OperatingStatus { get { return _data[8]; } }
        // 9-15
        public string ManufacturerCode { get { return MakeString(9, 8); } }
        // 17
        public byte Variant { get { return _data[17]; } }
        // 18-33
        public ushort[] Pollution { get { return MakeUshortArray(18, 8); } }
        // 34-41
        public ushort[] ReferencePollution { get { return MakeUshortArray(34, 4); } }
        // 42-57
        public ushort[] CalibratingPollution { get { return MakeUshortArray(42, 8); } }
        // 58-65
        public ushort[] CalibratingReferencePollution { get { return MakeUshortArray(58, 4); } }
        // 66-66
        public ushort MotorRevolutions { get { return MakeUshort(66); } }
        // 70-71
        public ushort ReferenceScale1Dark100 { get { return MakeUshort(70); } }
        // 74-75
        public ushort ReferenceScale2Dark100 { get { return MakeUshort(74); } }
        // 76-77
        public ushort ReferenceScale1Dark66 { get { return MakeUshort(76); } }
        // 80-81
        public ushort ReferenceScale2Dark66 { get { return MakeUshort(80); } }
        // 82-83
        public ushort SignalAmplitude { get { return MakeUshort(82); } }
        // 84-85
        public ushort CurrentAngle { get { return MakeUshort(84); } }
        // 86-87
        public ushort PeakThreshold { get { return MakeUshort(86); } }
        // 88-89
        public ushort AngleofMeasurement { get { return MakeUshort(88); } }
        // 90-91
        public ushort CalibrationSignalAmplitude { get { return MakeUshort(90); } }
        // 92-93
        public ushort TargetStopThreshold { get { return MakeUshort(92); } }
        // 94-95
        public ushort TargetPeakThreshold { get { return MakeUshort(94); } }
        // 96-97
        public ushort ActualStopThreshold { get { return MakeUshort(96); } }
        // 98-99
        public ushort ActualPeakThreshold { get { return MakeUshort(98); } }
        // 101
        public byte MeasuringMode { get { return _data[101]; } }
        // 102-103
        public ushort ReferenceTargetSingle { get { return MakeUshort(102); } }
        // 104-105
        public ushort ReferenceTargetMean { get { return MakeUshort(104); } }
        // 106-107
        public ushort ScanningAngle { get { return MakeUshort(106); } }
        // 108-109
        public ushort AngularResolution { get { return MakeUshort(108); } }
        // 110
        public byte RestartMode { get { return _data[110]; } }
        // 111
        public byte RestartTime { get { return _data[111]; } }
        // 115-116
        public ushort BaudRate { get { return MakeUshort(115); } }
        // 117
        public byte EvaluationNumber { get { return _data[117]; } }
        // 118
        public byte PermanentBaudRate { get { return _data[118]; } }
        // 119
        public byte LMSAddress { get { return _data[119]; } }
        // 120
        public byte FieldSetNumber { get { return _data[120]; } }
        // 121
        public byte CurrentMVUnit { get { return _data[121]; } }
        // 122
        public byte LaserSwitchOff { get { return _data[122]; } }
        // 123-130
        public string BootPROMVersion { get { return MakeString(123, 8); } }
        // 131-144
        public ushort[] CalibrationValues { get { return MakeUshortArray(131, 7); } }

        public LinkStatus(byte[] data)
        {
            if (data.Length < 145)
            {
                throw new ArgumentException("data");
            }
            _data = new byte[144];
            Array.Copy(data, 1, _data, 0, 144);
        }

        ushort MakeUshort(int index)
        {
            return (ushort)(_data[index] | (_data[index + 1] << 8));
        }

        string MakeString(int index, int length)
        {
            string ret = "";
            for (int i = 0; i < length; i++)
            {
                ret = ret + (char)_data[index++];
            }
            return ret;
        }

        ushort[] MakeUshortArray(int index, int length)
        {
            ushort[] array = new ushort[length];

            for (int i = 0; i < length; i++)
            {
                array[i] = MakeUshort(index);
                index += 2;
            }

            return array;
        }
    }

}
