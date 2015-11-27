//-----------------------------------------------------------------------
//  This file is part of Microsoft Robotics Developer Studio Code Samples.
//
//  Copyright (C) Microsoft Corporation.  All rights reserved.
//
//  $File: SerialIOManager.cs $ $Revision: 1 $
//-----------------------------------------------------------------------

using System;
using Microsoft.Ccr.Core;
using System.IO.Ports;
using Microsoft.Dss.Services.ConsoleOutput;

namespace TrackRoamer.Robotics.Services.TrackRoamerUsrf
{
    internal class BadPacketException : Exception
    {
        int _count;

        public BadPacketException(int count)
            : base("Incorrect Checksum on " + count + " SickLRF packets")
        {
            _count = count;
        }

        public int Count
        {
            get { return _count; }
            set { _count = value; }
        }
    }

    internal class SerialIOManager : CcrServiceBase
    {
        public class Operations : PortSet<Open, Close, SetRate, Send>
        {
        }

        public class ResponsePort : PortSet<Packet, Exception>
        {
        }

        public class Command
        {
            public SuccessFailurePort ResponsePort = new SuccessFailurePort();
        }

        public class Open : Command
        {
        }

        public class Close : Command
        {
        }

        public class SetRate : Command
        {
            public SetRate()
            {
            }

            public SetRate(int rate)
                : base()
            {
                _rate = rate;
            }

            int _rate;
            public int Rate
            {
                get { return _rate; }
                set { _rate = value; }
            }
        }

        public class Send : Command
        {
            public Send()
            {
            }

            public Send(Packet packet)
            {
                _packet = packet;
            }

            Packet _packet;
            public Packet Packet
            {
                get { return _packet; }
                set { _packet = value; }
            }
        }

        class Recv
        {
        }

        public Operations OperationsPort = new Operations();
        public ResponsePort Responses = new ResponsePort();

        Port<Recv> DataPort = new Port<Recv>();
        SerialPort _port;
        PacketBuilder _builder = new PacketBuilder();
        string _portName;
        int _badCount = 0;
        string _parent;
        ConsoleOutputPort _console;

        public SerialIOManager(DispatcherQueue dispatcherQueue, string portName)
            : base(dispatcherQueue)
        {
            _portName = portName;
            CreatePort(9600);

            _builder = new PacketBuilder();
            _builder.Parent = _parent;
            _builder.Console = _console;

            Activate(WaitForOpen());
        }

        public int BaudRate
        {
            get { return _port == null ? 9600 : _port.BaudRate; }
        }

        private void CreatePort(int rate)
        {
            _port = new SerialPort(_portName, rate, Parity.None, 8, StopBits.One);
            _port.DataReceived += _port_DataReceived;
            _port.ErrorReceived += _port_ErrorReceived;
            _port.DtrEnable = true;
            _port.RtsEnable = true;
        }

        void _port_ErrorReceived(object sender, SerialErrorReceivedEventArgs e)
        {
            Responses.Post(new Exception(e.EventType.ToString()));
        }

        void _port_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            DataPort.Post(new Recv());
        }

        void CommandUnavailable(Command cmd)
        {
            cmd.ResponsePort.Post(new Exception("The requested command is not available in the current state"));
        }

        Interleave WaitForOpen()
        {
            return Arbiter.Interleave(
                new TeardownReceiverGroup
                (
                    Arbiter.Receive<Open>(false,OperationsPort,OpenHandler)
                ),
                new ExclusiveReceiverGroup
                (
                ),
                new ConcurrentReceiverGroup
                (
                    Arbiter.Receive(true, DataPort, IgnoreDataHandler),
                    Arbiter.Receive(true, OperationsPort.P1, CommandUnavailable),
                    Arbiter.Receive(true, OperationsPort.P2, CommandUnavailable),
                    Arbiter.Receive(true, OperationsPort.P3, CommandUnavailable)
                ));
        }


        Interleave Connected()
        {
            return Arbiter.Interleave(
                new TeardownReceiverGroup
                (
                    Arbiter.Receive<Close>(false,OperationsPort,CloseHandler)
                ),
                new ExclusiveReceiverGroup
                (
                    Arbiter.Receive<SetRate>(true, OperationsPort, SetRateHandler),
                    Arbiter.Receive<Send>(true, OperationsPort, SendHandler),
                    Arbiter.Receive<Recv>(true, DataPort, DataHandler)
                ),
                new ConcurrentReceiverGroup
                (
                    Arbiter.Receive(true, OperationsPort.P0, CommandUnavailable)
                ));
        }

        void IgnoreDataHandler(Recv recv)
        {
            if (_port == null ||
                _port.BytesToRead <= 0)
            {
                return;
            }
            byte[] buffer = new byte[_port.BytesToRead];

            _port.Read(buffer, 0, buffer.Length);
        }

        void DataHandler(Recv recv)
        {
            while (DataPort.Test() != null) ;

            if (_port == null ||
                _port.BytesToRead <= 0)
            {
                return;
            }
            byte[] buffer = new byte[_port.BytesToRead];

            int read = _port.Read(buffer, 0, buffer.Length);
            if (read == 0)
            {
                return;
            }

            _builder.Add(buffer, read);
            while (_builder.HasPacket)
            {
                Responses.Post(_builder.RemovePacket());
            }
            if (_builder.BadPackets > 0)
            {
                if (_builder.BadPackets != _badCount)
                {
                    //
                    // only report bad packets if the number has changed.
                    //
                    _badCount = _builder.BadPackets;
                    Responses.Post(new BadPacketException(_builder.BadPackets));
                }
            }
            else
            {
                _badCount = 0;
            }
        }

        void CloseHandler(Close close)
        {
            _port.DataReceived -= _port_DataReceived;
            _port.ErrorReceived -= _port_ErrorReceived;
            _port.Close();
            _port = null;

            _builder = new PacketBuilder();
            _builder.Parent = _parent;
            _builder.Console = _console;

            Recv recv;
            while (DataPort.Test(out recv)) ;

            close.ResponsePort.Post(new SuccessResult());

            Activate(WaitForOpen());
        }

        void SendHandler(Send send)
        {
            try
            {
                //send.Packet.Send(_port);
                send.ResponsePort.Post(new SuccessResult());
            }
            catch (Exception e)
            {
                send.ResponsePort.Post(new Exception(e.Message));
            }
        }

        void SetRateHandler(SetRate setRate)
        {
            try
            {
                _port.Close();
                CreatePort(setRate.Rate);
                _port.Open();

                setRate.ResponsePort.Post(new SuccessResult());
            }
            catch (Exception e)
            {
                setRate.ResponsePort.Post(new Exception(e.Message));
            }
        }

        void OpenHandler(Open open)
        {
            try
            {
                if (_port == null)
                {
                    CreatePort(9600);
                }
                _port.Open();
                open.ResponsePort.Post(new SuccessResult());

                Activate(Connected());
            }
            catch (Exception e)
            {
                open.ResponsePort.Post(new Exception(e.Message));

                Activate(WaitForOpen());
            }
        }

        public string Parent
        {
            get { return _parent; }
            set
            {
                _parent = value;
                _builder.Parent = value;
            }
        }

        public ConsoleOutputPort Console
        {
            get { return _console; }
            set
            {
                _console = value;
                _builder.Console = value;
            }
        }
    }

}