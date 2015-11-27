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

using TrackRoamer.Robotics.Utility.LibPicSensors;
using TrackRoamer.Robotics.Utility.LibSystem;

namespace TrackRoamer.Robotics.Services.TrackRoamerBrickProximityBoard
{
    internal class ProximityBoardCcrServiceCommander : CcrServiceBase, IDisposable
    {
        PBCommandPort _pbCommanderDataEventsPort;    // where to post data, destined for TrackRoamerBrickProximityBoardService

        ProximityBoardManager _pmManager;   // down at the stack
        
        string _parent;     // for tracing
        ConsoleOutputPort _console;
        string _description = "TrackRoamer Proximity Board CCR Service Commander";

        public new DispatcherQueue TaskQueue
        {
            get { return base.TaskQueue; }
        }

        // called from TrackRoamerBrickProximityBoardService:
        public ProximityBoardCcrServiceCommander(DispatcherQueue dispatcherQueue, PBCommandPort uplinkPort, Int32 vendorId, Int32 productId)
            : base(dispatcherQueue)
        {
            Tracer.Trace(string.Format("ProximityBoardCcrServiceCommander::ProximityBoardCcrServiceCommander() vendorId = 0x{0:X} productId = 0x{0:X}", vendorId, productId));

            _pbCommanderDataEventsPort = uplinkPort;

            _pmManager = new ProximityBoardManager(dispatcherQueue, vendorId, productId);

            _pmManager.PicPxMod.DeviceAttachedEvent += new EventHandler<ProximityModuleEventArgs>(picpxmod_DeviceAttachedEvent);
            _pmManager.PicPxMod.DeviceDetachedEvent += new EventHandler<ProximityModuleEventArgs>(picpxmod_DeviceDetachedEvent);

            _pmManager.Start();

            Activate<ITask>(
                Arbiter.Receive<SonarData>(true, _pmManager.MeasurementsPort, SonarDataHandler),
                Arbiter.Receive<DirectionData>(true, _pmManager.MeasurementsPort, DirectionDataHandler),
                Arbiter.Receive<AccelerometerData>(true, _pmManager.MeasurementsPort, AccelerometerDataHandler),
                Arbiter.Receive<ProximityData>(true, _pmManager.MeasurementsPort, ProximityDataHandler),
                Arbiter.Receive<ParkingSensorData>(true, _pmManager.MeasurementsPort, ParkingSensorDataHandler),
                Arbiter.Receive<AnalogData>(true, _pmManager.MeasurementsPort, AnalogDataHandler),
                Arbiter.Receive<Exception>(true, _pmManager.MeasurementsPort, ExceptionHandler)
            );
        }

        public string Parent        // for tracing
        {
            get { return _parent; }
            set
            {
                _parent = value;
                _pmManager.Parent = value;
            }
        }

        public ConsoleOutputPort Console
        {
            get { return _console; }
            set
            {
                _console = value;
                _pmManager.Console = value;
            }
        }

        public string Description
        {
            get { return _description; }
        }

        void LogInfo(string format, params object[] args)
        {
            string msg = string.Format(format, args);

            // this won't compile under 4 Beta:
            //Microsoft.Dss.ServiceModel.DsspServiceBase.DsspServiceBase.Log(TraceLevel.Info,
            //                    TraceLevel.Info,
            //                    new XmlQualifiedName("ProximityBoardCcrServiceCommander", Contract.Identifier),
            //                    _parent,
            //                    msg,
            //                    null,
            //                    _console);
        }

        void ExceptionHandler(Exception e)
        {
            Tracer.Trace("ProximityBoardCcrServiceCommander::ExceptionHandler() exc=" + e);

            _pbCommanderDataEventsPort.Post(e);
        }

        void picpxmod_DeviceAttachedEvent(object sender, ProximityModuleEventArgs e)
        {
            Tracer.Trace("ProximityBoardCcrServiceCommander::picpxmod_DeviceAttachedEvent(): " + e.description);

            _pbCommanderDataEventsPort.Post(new ProximityBoardUsbDeviceAttached(e.description));
        }

        void picpxmod_DeviceDetachedEvent(object sender, ProximityModuleEventArgs e)
        {
            Tracer.Trace("ProximityBoardCcrServiceCommander::picpxmod_DeviceDetachedEvent(): " + e.description);

            _pbCommanderDataEventsPort.Post(new ProximityBoardUsbDeviceDetached(e.description));
        }

        void SonarDataHandler(SonarData sonarData)
        {
            //Tracer.Trace("ProximityBoardCcrServiceCommander::SonarDataHandler()");

            int packetLength = sonarData.angles.Count;      // typically 26 packets, each covering 7 degrees;  SortedList<int,int> angles

            if (packetLength != 26)
            {
                Tracer.Error("ProximityBoardCcrServiceCommander::SonarDataHandler()  not a standard sonar sweep measurement, angles.Count=" + packetLength + "  (expected 26) -- ignored");
            }
            else
            {
                _pbCommanderDataEventsPort.Post(sonarData);
            }
        }

        void DirectionDataHandler(DirectionData directionData)
        {
            //Tracer.Trace("ProximityBoardCcrServiceCommander::DirectionDataHandler()");

            _pbCommanderDataEventsPort.Post(directionData);
        }

        void AccelerometerDataHandler(AccelerometerData accelerometerData)
        {
            //Tracer.Trace("ProximityBoardCcrServiceCommander::AccelerometerDataHandler()");

            _pbCommanderDataEventsPort.Post(accelerometerData);
        }

        void ProximityDataHandler(ProximityData proximityData)
        {
            //Tracer.Trace("ProximityBoardCcrServiceCommander::ProximityDataHandler()");

            _pbCommanderDataEventsPort.Post(proximityData);
        }

        void ParkingSensorDataHandler(ParkingSensorData parkingSensorData)
        {
            //Tracer.Trace("ProximityBoardCcrServiceCommander::ParkingSensorDataHandler()");

            _pbCommanderDataEventsPort.Post(parkingSensorData);
        }

        void AnalogDataHandler(AnalogData analogData)
        {
            //Tracer.Trace("ProximityBoardCcrServiceCommander::AnalogDataHandler()");

            _pbCommanderDataEventsPort.Post(analogData);
        }

        #region IDisposable Members

        public void Dispose()
        {
            Tracer.Trace("ProximityBoardCcrServiceCommander::Dispose()");

            Close();
        }

        #endregion

        #region OperationsPort Commands

        public SuccessFailurePort Open()
        {
            Tracer.Trace("ProximityBoardCcrServiceCommander::Open()");

            ProximityBoardManager.Open open = new ProximityBoardManager.Open();

            _pmManager.OperationsPort.Post(open);

            return open.ResponsePort;
        }

        public SuccessFailurePort Close()
        {
            Tracer.Trace("ProximityBoardCcrServiceCommander::Close()");

            ProximityBoardManager.Close close = new ProximityBoardManager.Close();

            _pmManager.OperationsPort.Post(close);

            return close.ResponsePort;
        }

        /// <summary>
        /// Try finding the hid. May return failure if not found.
        /// </summary>
        public SuccessFailurePort TryFindTheHid()
        {
            Tracer.Trace("ProximityBoardCcrServiceCommander::TryFindTheHid()");

            SuccessFailurePort port = new SuccessFailurePort();

            if (_pmManager.PicPxMod.FindTheHid())
            {
                port.Post(new SuccessResult());
            }
            else
            {
                port.Post(new Exception("HID not found"));
            }
            return port;
        }

        public SuccessFailurePort SetServoSweepRate()
        {
            Tracer.Trace("ProximityBoardCcrServiceCommander::SetRate()");

            ProximityBoardManager.SetServoSweepRate sweepRate = new ProximityBoardManager.SetServoSweepRate();

            _pmManager.OperationsPort.Post(sweepRate);

            return sweepRate.ResponsePort;
        }

        public SuccessFailurePort SetContinuous()
        {
            Tracer.Trace("ProximityBoardCcrServiceCommander::SetContinuous()");

            ProximityBoardManager.SetContinuous setContinuous = new ProximityBoardManager.SetContinuous();

            _pmManager.OperationsPort.Post(setContinuous);

            return setContinuous.ResponsePort;
        }

        public SuccessFailurePort StopContinuous()
        {
            Tracer.Trace("ProximityBoardCcrServiceCommander::StopContinuous()");

            ProximityBoardManager.StopContinuous stopContinuous = new ProximityBoardManager.StopContinuous();

            _pmManager.OperationsPort.Post(stopContinuous);

            return stopContinuous.ResponsePort;
        }

        public SuccessFailurePort MeasureOnce()
        {
            Tracer.Trace("ProximityBoardCcrServiceCommander::MeasureOnce()");

            ProximityBoardManager.MeasureOnce measureOnce = new ProximityBoardManager.MeasureOnce();

            _pmManager.OperationsPort.Post(measureOnce);

            return measureOnce.ResponsePort;
        }

        public SuccessFailurePort RequestStatus()
        {
            Tracer.Trace("ProximityBoardCcrServiceCommander::RequestStatus()");

            ProximityBoardManager.RequestStatus requestStatus = new ProximityBoardManager.RequestStatus();

            _pmManager.OperationsPort.Post(requestStatus);

            return requestStatus.ResponsePort;
        }
        #endregion // OperationsPort Commands
    }

    #region Commands definitions

    internal class PBCommandPort : PortSet<SonarData, DirectionData, AccelerometerData, ProximityData, ParkingSensorData, AnalogData, ProximityBoardUsbDeviceAttached, ProximityBoardUsbDeviceDetached, ProximityBoardConfirm, Exception> 
    {
    }

    internal class ProximityBoardUsbDeviceAttached
    {
        public ProximityBoardUsbDeviceAttached()
        {
        }

        public ProximityBoardUsbDeviceAttached(string description)
        {
            Description = description;
        }

        public string Description;
    }

    internal class ProximityBoardUsbDeviceDetached
    {
        public ProximityBoardUsbDeviceDetached()
        {
        }

        public ProximityBoardUsbDeviceDetached(string description)
        {
            Description = description;
        }

        public string Description;
    }

    internal class ProximityBoardConfirm    // for future enhancements, in case we will need to receive a command confirmation from the Proximity Board
    {
    }

    #endregion // Commands
}
