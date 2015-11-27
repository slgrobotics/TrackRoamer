using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;

using Microsoft.Ccr.Core;
using Microsoft.Dss.Services.ConsoleOutput;

using TrackRoamer.Robotics.Utility.LibPicSensors;
using TrackRoamer.Robotics.Utility.LibSystem;

namespace TrackRoamer.Robotics.Services.TrackRoamerBrickProximityBoard
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

    internal class ProximityBoardManager : CcrServiceBase
    {
        private ProximityModule _picpxmod;

        public ProximityModule PicPxMod { get { return _picpxmod; } }

        #region Messages definitions - Operations, Responses and others 

        public class Operations : PortSet<Open, Close, SetServoSweepRate, SetContinuous, StopContinuous, MeasureOnce, RequestStatus>
        {
        }

        public class ResponsePort : PortSet<SonarData, DirectionData, AccelerometerData, ProximityData, ParkingSensorData, AnalogData, Exception>
        {
        }

        public class Command
        {
            public SuccessFailurePort ResponsePort = new SuccessFailurePort();
        }

        public class Open : Command
        {
            // Open is an empty operatiion, but we need to flush the internal queue - so let it be.
        }

        public class Close : Command
        {
        }

        public class SetContinuous : Command
        {
        }

        public class StopContinuous : Command
        {
        }

        public class MeasureOnce : Command
        {
        }

        public class RequestStatus : Command
        {
        }

        public class SetServoSweepRate : Command
        {
            public SetServoSweepRate()
            {
                SweepStartPos1 = ProximityBoard.sweepStartPos1;
                SweepStartPos2 = ProximityBoard.sweepStartPos2;
                SweepStep = ProximityBoard.sweepStep;
                SweepSteps = ProximityBoard.sweepSteps;
                SweepMax = ProximityBoard.sweepMax;
                SweepMin = ProximityBoard.sweepMin;
                SweepRate = ProximityBoard.sweepRate;
                InitialDirectionUp = ProximityBoard.initialDirectionUp;
            }

            public double SweepStartPos1 { get; set; }      // us
            public double SweepStartPos2 { get; set; }      // us
            public double SweepStep { get; set; }           // How big a single step is. These are not us, but some internal to PIC microcontroller code number
            public int SweepSteps { get; set; }             // not used, would be nice to calculate SweepStep based on number of steps
            public double SweepMax { get; set; }            // us
            public double SweepMin { get; set; }            // us
            public double SweepRate { get; set; }           // How fast the servos will move (wait on a step). Must be divisible by 7, due to microcontroller servo pulse cycle interactions.
            public bool InitialDirectionUp { get; set; }
        }

        internal class Recv
        {
        }

        #endregion // Messages definitions - Operations, Responses and others

        public Operations   OperationsPort = new Operations();          // receives commands from ProximityBoardCcrServiceCommander
        public ResponsePort MeasurementsPort = new ResponsePort();      // passes the data up to ProximityBoardCcrServiceCommander

        Port<Recv> DataReadyPort = new Port<Recv>();                    // signals to pick up data from the queues go through this port

        public string Parent;   // fro tracing

        public ConsoleOutputPort Console;

        public ProximityBoardManager(DispatcherQueue dispatcherQueue, Int32 vendorId, Int32 productId)
            : base(dispatcherQueue)
        {
            // create picpxmod device.
            _picpxmod = new ProximityModule(vendorId, productId);

            _picpxmod.HasReadFrameEvent += pmFrameCompleteHandler;

            Tracer.Trace(string.Format("ProximityBoardManager: vendorId = 0x{0:X} productId = 0x{0:X}", vendorId, productId));
        }

        public void Start()
        {
            Activate(WaitForOpen());
        }

        #region State Machine interleaves

        // a "closed" state, when data is ignored and we are just waiting for Open command from ProximityBoardCcrServiceCommander:
        Interleave WaitForOpen()
        {
            Tracer.Trace("ProximityBoardManager: WaitForOpen() interleave started");

            return Arbiter.Interleave(
                new TeardownReceiverGroup
                (
                    Arbiter.Receive<Open>(false, OperationsPort, OpenHandler)
                ),
                new ExclusiveReceiverGroup
                (
                ),
                new ConcurrentReceiverGroup
                (
                    Arbiter.Receive(true, DataReadyPort, IgnoreDataHandler),
                    Arbiter.Receive(true, OperationsPort.P1, CommandUnavailable),
                    Arbiter.Receive(true, OperationsPort.P2, CommandUnavailable),
                    Arbiter.Receive(true, OperationsPort.P3, CommandUnavailable)
                ));
        }

        // a "working" state, when data is processed and we are accepting Close, SetServoSweepRate and other commands from ProximityBoardCcrServiceCommander:
        Interleave Connected()
        {
            Tracer.Trace("ProximityBoardManager: Connected() interleave started");

            return Arbiter.Interleave(
                new TeardownReceiverGroup
                (
                    Arbiter.Receive<Close>(false, OperationsPort, CloseHandler)
                ),
                new ExclusiveReceiverGroup
                (
                    Arbiter.Receive<SetServoSweepRate>(true, OperationsPort, SetServoSweepRateHandler),
                    Arbiter.Receive<SetContinuous>(true, OperationsPort, SetContinuousHandler),
                    Arbiter.Receive<StopContinuous>(true, OperationsPort, StopContinuousHandler),
                    Arbiter.Receive<Recv>(true, DataReadyPort, DataHandler)
                ),
                new ConcurrentReceiverGroup
                (
                    Arbiter.Receive(true, OperationsPort.P0, CommandUnavailable)
                ));
        }

        #endregion // State Machine interleaves

        // =================================================================================================================

        #region Processing Data Frame from LibPicSensors.ProximityModule - pmFrameCompleteHandler()

        // This will be called whenever the data is read from the board:
        private void pmFrameCompleteHandler(object sender, AsyncInputFrameArgs aira)
        {
            /*
            Tracer.Trace("Async frame arrived. " + now);

            StringBuilder frameValue = new StringBuilder();

            frameValue.AppendFormat("ProximityBoardManager: frame arrived: {0} ", aira.dPos1Mks);
            frameValue.AppendFormat("{0} ", aira.dPos2Mks);
            frameValue.AppendFormat("{0} ", aira.dPing1DistanceMm);
            frameValue.AppendFormat("{0}", aira.dPing2DistanceMm);

            Tracer.Trace(frameValue.ToString());
            */

            bool haveData = false;

            // IR Proximity sensors data:

            ProximityData proxData = new ProximityData() { TimeStamp = aira.timestamp };

            proxData.setProximityData(aira.sensorsState.irbE1, aira.sensorsState.irbE2, aira.sensorsState.irbE3, aira.sensorsState.irbE4,
                                        aira.sensorsState.irbO1, aira.sensorsState.irbO2, aira.sensorsState.irbO3, aira.sensorsState.irbO4);

            lock (_proxiDatas)
            {
                _proxiDatas.Enqueue(proxData);
            }
            haveData = true;                    // allow DataPort.Post(new Recv()) to happen, with data already enqueued in the ...Datas 

            // accelerometer data:

            AccelerometerData accData = new AccelerometerData() { TimeStamp = aira.timestamp };

            accData.setAccelerometerData(aira.sensorsState.accelX, aira.sensorsState.accelY, aira.sensorsState.accelZ);

            lock (_accelDatas)
            {
                _accelDatas.Enqueue(accData);
            }
            haveData = true;                    // allow DataPort.Post(new Recv()) to happen, with data already enqueued in the ...Datas 

            // compass data:

            DirectionData dirData = null;

            if (aira.sensorsState.compassHeading >= 0.0d && aira.sensorsState.compassHeading <= 359.999999d)
            {
                dirData = new DirectionData() { heading = aira.sensorsState.compassHeading, TimeStamp = aira.timestamp };

                lock (_directionDatas)
                {
                    _directionDatas.Enqueue(dirData);
                }
                haveData = true;                    // allow DataPort.Post(new Recv()) to happen, with data already enqueued in the ...Datas 
            }

            ParkingSensorData psiData = null;

            if (aira.sensorsState.parkingSensorsValid)
            {
                psiData = new ParkingSensorData() { TimeStamp = aira.timestamp };

                psiData.setParkingSensorData(aira.sensorsState);

                lock (_parkingSensorsDatas)
                {
                    _parkingSensorsDatas.Enqueue(psiData);
                }
                haveData = true;                    // allow DataPort.Post(new Recv()) to happen, with data already enqueued in the ...Datas 
            }

            // frames that are marked "fromPingScanStop" feed the sonar sweep view controls:

            if (aira.fromPingScanStop)
            {
                int angleRaw1 = (int)aira.dPos1Mks;
                double distM1 = aira.dPing1DistanceM;

                //Tracer.Trace(String.Format("angleRaw1 = {0} us", angleRaw1));
                //Tracer.Trace(String.Format("distMm1 = {0} mm", distMm1));

                int angleRaw2 = (int)aira.dPos2Mks;
                double distM2 = aira.dPing2DistanceM;

                //Tracer.Trace(String.Format("angleRaw2 = {0} us", angleRaw2));
                //Tracer.Trace(String.Format("distMm2 = {0} mm", distMm2));

                haveData = haveData && setSonarRayReading(angleRaw1, distM1, angleRaw2, distM2, aira.timestamp);
            }

            if (haveData)
            {
                AnalogValue1 = aira.sensorsState.analogValue1;

                // just post internally an empty signal message, the receiver - DataHandler(Recv recv) should examine the ...Datas and forward data to Responses port
                DataReadyPort.Post(new Recv());
            }
        }

        // buffers for incoming measurements. All enqueueing / dequeueing is not thread safe, and needs locking:
        private Queue<SonarData>         _sonarDatas = new Queue<SonarData>();
        private Queue<DirectionData>     _directionDatas = new Queue<DirectionData>();
        private Queue<AccelerometerData> _accelDatas = new Queue<AccelerometerData>();
        private Queue<ProximityData>     _proxiDatas = new Queue<ProximityData>();
        private Queue<ParkingSensorData> _parkingSensorsDatas = new Queue<ParkingSensorData>();

        private int _badCount = 0;

        private long timestampLastReading = 0L;
        private long timestampLastSweepReading = 0L;
        private SonarData sonarData = new SonarData();

        private double AnalogValue1Prev;
        private double AnalogValue1;    // (pin 2 RA0/AN0 on PIC4550):
                                        // 0v = 0
                                        // 1v = 220
                                        // 2v = 415
                                        // 3v = 630
                                        // 4v = 835
                                        // 4.88v = 1023

        private ProximityBoard board = new ProximityBoard();    // robot-specific sensor geometry and other settings and conversions

        private const int TEST_SAMPLES_COUNT = 100;
        private int inTestSamples = TEST_SAMPLES_COUNT;

        private void ResetSweepCollector()
        {
            lock (_sonarDatas)
            {
                _sonarDatas.Clear();
            }

            lock (_directionDatas)
            {
                _directionDatas.Clear();
            }

            lock (_accelDatas)
            {
                _accelDatas.Clear();
            }

            lock (_proxiDatas)
            {
                _proxiDatas.Clear();
            }

            lock (_parkingSensorsDatas)
            {
                _parkingSensorsDatas.Clear();
            }

            _badCount = 0;

            timestampLastReading = 0L;
            timestampLastSweepReading = 0L;
            sonarData = new SonarData();

            board = new ProximityBoard();

            inTestSamples = TEST_SAMPLES_COUNT;
        }

        public bool setSonarRayReading(int angleRaw1, double rangeMeters1, int angleRaw2, double rangeMeters2, long timestamp)
        {
            bool haveData = false;

            //Tracer.Trace("setReading() sonar: " + angleRaw1 + "   " + rangeMeters1 + "   " + angleRaw2 + "   " + rangeMeters2);

            if (inTestSamples > 0)
            {
                inTestSamples--;

                // for a while just try figuring out what comes in - sweep angle ranges for both sides

                if (inTestSamples > TEST_SAMPLES_COUNT - 10)        // first few frames are garbled
                {
                    return haveData;
                }

                board.registerAnglesRaw(angleRaw1, angleRaw2);

                if (inTestSamples == 0)     // last count
                {
                    board.finishPrerun();

                    Tracer.Trace(board.ToString());
                }

                return haveData;
            }

            if (angleRaw1 == board.rawAngle1_Min || angleRaw1 == board.rawAngle1_Max)
            {
                RangeReading rr1 = board.angleRawToSonarAngleCombo(1, angleRaw1, rangeMeters1, timestamp);
                RangeReading rr2 = board.angleRawToSonarAngleCombo(2, angleRaw2, rangeMeters2, timestamp);

                sonarData.addRangeReading(rr1, rr2, timestamp);

                if (sonarData.angles.Count == 26)
                {
                    lock (_sonarDatas)
                    {
                        _sonarDatas.Enqueue(sonarData);
                    }
                    haveData = true;                    // allow DataPort.Post(new Recv()) to happen, with data already enqueued in the ...Datas 

                    _badCount = 0;

                    TimeSpan sinceLastReading = new TimeSpan(timestamp - timestampLastReading);
                    TimeSpan sinceLastSweepReading = new TimeSpan(timestamp - timestampLastSweepReading);

                    //Tracer.Trace("SONAR PACKET READY -- angles: " + sonarData.angles.Count + "  packets: " + _sonarDatas.Count
                    //    + (new DateTime(timestamp)).ToString()
                    //    + String.Format("   {0:0.000} s/ray   {1:0.000} s/sweep", sinceLastReading.TotalSeconds, sinceLastSweepReading.TotalSeconds));

                    timestampLastSweepReading = timestamp;
                }
                else
                {
                    _badCount++;
                    Tracer.Trace(string.Format("BAD SONAR PACKET -- angles: {0}  angleRaw1: {1}  -   #{2} since last good packet", sonarData.angles.Count, angleRaw1, _badCount));
                }

                sonarData = new SonarData();   // prepare for the next one
            }

            timestampLastReading = timestamp;

            RangeReading rrr1 = board.angleRawToSonarAngleCombo(1, angleRaw1, rangeMeters1, timestamp);
            RangeReading rrr2 = board.angleRawToSonarAngleCombo(2, angleRaw2, rangeMeters2, timestamp);

            sonarData.addRangeReading(rrr1, rrr2, timestamp);

            return haveData;
        }

        #endregion // Processing Data Frame

        // =================================================================================================================

        #region DataReadyPort handlers

        /// <summary>
        /// This handler is activated by empty Recv messages coming to internal DataReadyPort.
        /// The data comes here from pmFrameCompleteHandler
        /// This is our opportunity to purge stale data from the queues. 
        /// The data posted to Responses port is then picked up by ProximityBoardCcrServiceCommander:SonarDataHandler() and others,
        /// to be later passed to TrackRoamerBrickProximityBoard.
        /// </summary>
        /// <param name="recv"></param>
        private void DataHandler(Recv recv)
        {
            /*
            if(_sonarDatas.Count > 1 || _directionDatas.Count > 1 || _accelDatas.Count > 1 || _proxDatas.Count > 1 || _parkingSensorsDatas.Count > 1)
            {
                // for debugging, log if queues get more than 1 piece of each data type:
                Tracer.Trace("ProximityBoardManager: DataHandler()  sonars: " + _sonarDatas.Count + "  compass: " + _directionDatas.Count + "  accels: " + _accelDatas.Count + "  proxs: " + _proxDatas.Count + "  parking: " + _parkingSensorsDatas.Count);
            }
            */

            if (_sonarDatas.Count > 0)
            {
                SonarData sonarData = null;

                lock (_sonarDatas)
                {
                    while (_sonarDatas.Count > 0)
                    {
                        sonarData = _sonarDatas.Last();
                        _sonarDatas.Clear();
                    }
                }

                if (sonarData != null)
                {
                    MeasurementsPort.Post(sonarData);
                }
            }

            if (_directionDatas.Count > 0)
            {
                DirectionData directionData = null;

                lock (_directionDatas)
                {
                    while (_directionDatas.Count > 0)
                    {
                        directionData = _directionDatas.Last();
                        _directionDatas.Clear();
                    }
                }

                if (directionData != null)
                {
                    MeasurementsPort.Post(directionData);
                }
            }

            if (_accelDatas.Count > 0)
            {
                AccelerometerData accelerometerData = null;

                lock (_accelDatas)
                {
                    while (_accelDatas.Count > 0)
                    {
                        accelerometerData = _accelDatas.Last();
                        _accelDatas.Clear();
                    }
                }

                if (accelerometerData != null)
                {
                    MeasurementsPort.Post(accelerometerData);
                }
            }

            if (_proxiDatas.Count > 0)
            {
                ProximityData proximityData = null;

                lock (_proxiDatas)
                {
                    while (_proxiDatas.Count > 0)
                    {
                        proximityData = _proxiDatas.Last();
                        _proxiDatas.Clear();
                    }
                }

                if (proximityData != null)
                {
                    MeasurementsPort.Post(proximityData);
                }
            }

            if (_parkingSensorsDatas.Count > 0)
            {
                ParkingSensorData psiData = null;

                lock (_parkingSensorsDatas)
                {
                    while (_parkingSensorsDatas.Count > 0)
                    {
                        psiData = _parkingSensorsDatas.Last();
                        _parkingSensorsDatas.Clear();
                    }
                }

                if (psiData != null)
                {
                    MeasurementsPort.Post(psiData);
                }
            }

            //Tracer.Trace("AnalogData:  current=" + AnalogValue1 + "    prev=" + AnalogValue1Prev);

            // and last but not least - post AnalogData - only if any of the values changed:
            if (AnalogValue1 != AnalogValue1Prev)
            {
                AnalogValue1Prev = AnalogValue1;
                AnalogData analogData = new AnalogData()
                {
                    analogValue1 = AnalogValue1,
                    TimeStamp = DateTime.Now.Ticks 
                };
                MeasurementsPort.Post(analogData);
            }
        }

        private void IgnoreDataHandler(Recv recv)
        {
            lock (_sonarDatas)
            {
                _sonarDatas.Clear();
            }

            lock (_directionDatas)
            {
                _directionDatas.Clear();
            }

            lock (_accelDatas)
            {
                _accelDatas.Clear();
            }

            lock (_proxiDatas)
            {
                _proxiDatas.Clear();
            }

            lock (_parkingSensorsDatas)
            {
                _parkingSensorsDatas.Clear();
            }
        }

        #endregion // DataReadyPort handlers

        // =================================================================================================================

        #region Open, Close, SetServoSweepRate and other operations handlers

        private void OpenHandler(Open open)
        {
            Tracer.Trace("ProximityBoardManager: OpenHandler()");

            try
            {
                if (_picpxmod.FindTheHid())
                {
                    open.ResponsePort.Post(new SuccessResult());

                    Activate(Connected());
                }
                else
                {
                    open.ResponsePort.Post(new Exception("Cannot find Proximity Board HID. Is the board connected via USB?"));

                    Activate(WaitForOpen());
                }
            }
            catch (Exception e)
            {
                open.ResponsePort.Post(new Exception(e.Message));

                Activate(WaitForOpen());
            }
        }

        private void CloseHandler(Close close)
        {
            Tracer.Trace("ProximityBoardManager: CloseHandler()");

            _picpxmod.Dispose();        // will call SafePosture

            // empty the queue:
            Recv recv;
            while (DataReadyPort.Test(out recv))
                ;

            close.ResponsePort.Post(new SuccessResult());

            Activate(WaitForOpen());
        }

        private void SetServoSweepRateHandler(SetServoSweepRate s)
        {
            Tracer.Trace("ProximityBoardManager: SetServoSweepRateHandler()");

            _picpxmod.ServoSweepParams(1, s.SweepMin, s.SweepMax, s.SweepStartPos1, s.SweepStep, s.InitialDirectionUp, s.SweepRate);
            _picpxmod.ServoSweepParams(2, s.SweepMin, s.SweepMax, s.SweepStartPos2, s.SweepStep, s.InitialDirectionUp, s.SweepRate);

            // to avoid missed commands, send it twice:
            _picpxmod.ServoSweepParams(1, s.SweepMin, s.SweepMax, s.SweepStartPos1, s.SweepStep, s.InitialDirectionUp, s.SweepRate);
            _picpxmod.ServoSweepParams(2, s.SweepMin, s.SweepMax, s.SweepStartPos2, s.SweepStep, s.InitialDirectionUp, s.SweepRate);

            s.ResponsePort.Post(new SuccessResult());
        }

        private void SetContinuousHandler(SetContinuous setContinuous)
        {
            Tracer.Trace("ProximityBoardManager: SetContinuousHandler()");

            _picpxmod.ServoSweepEnable(true);

            ResetSweepCollector();

            _picpxmod.DataContinuousStart();

            // to avoid missed commands, send it twice:
            _picpxmod.DataContinuousStart();

            setContinuous.ResponsePort.Post(new SuccessResult());
        }

        private void StopContinuousHandler(StopContinuous stopContinuous)
        {
            Tracer.Trace("ProximityBoardManager: StopContinuousHandler()");

            _picpxmod.SafePosture();

            stopContinuous.ResponsePort.Post(new SuccessResult());
        }

        void CommandUnavailable(Command cmd)
        {
            cmd.ResponsePort.Post(new Exception("The requested command is not available in the current state"));
        }

        #endregion // Open, Close, SetServoSweepRate and other operations handlers
    }
}
