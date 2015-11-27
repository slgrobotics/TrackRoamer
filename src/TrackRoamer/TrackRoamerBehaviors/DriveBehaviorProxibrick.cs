//#define TRACEDEBUG
//#define TRACEDEBUGTICKS
//#define TRACELOG

using System;

using Microsoft.Ccr.Core;
using Microsoft.Dss.ServiceModel.DsspServiceBase;

using ccrwpf = Microsoft.Ccr.Adapters.Wpf;

using TrackRoamer.Robotics.Utility.LibSystem;
using TrackRoamer.Robotics.LibMapping;
using TrackRoamer.Robotics.Utility.LibPicSensors;
using TrackRoamer.Robotics.LibBehavior;

using proxibrick = TrackRoamer.Robotics.Services.TrackRoamerBrickProximityBoard.Proxy;

namespace TrackRoamer.Robotics.Services.TrackRoamerBehaviors
{
    partial class TrackRoamerBehaviorsService : DsspServiceBase
    {
        protected double robotCornerDistanceMeters = 0.0d;   // to account for robot dimensions when adding measurements from corner-located proximity sensors.

        #region Proxibrick - Accelerometer, Compass, Proximity handlers

        void trpbUpdateAccelerometerNotification(proxibrick.UpdateAccelerometerData update)
        {
            //LogInfo("DriveBehaviorServiceBase: trpbUpdateAccelerometerNotification()");

            //Tracer.Trace("DriveBehaviorServiceBase:  trpbUpdateAccelerometerNotification()");

            if (USE_ORIENTATION_PROXIBRICK)
            {
                try
                {
                    _state.MostRecentAccelerometer = update.Body;

                    setGuiCurrentAccelerometer(_state.MostRecentAccelerometer);

                    if (!_doUnitTest)
                    {
                        Decide(SensorEventSource.Orientation);
                    }
                }
                catch (Exception exc)
                {
                    Tracer.Trace("trpbUpdateAccelerometerNotification() - " + exc);
                }
            }
        }

        protected void setGuiCurrentAccelerometer(proxibrick.AccelerometerDataDssSerializable acc)
        {
            if (_mainWindow != null)
            {
                ccrwpf.Invoke invoke = new ccrwpf.Invoke(delegate()
                {
                    _mainWindow.CurrentAccelerometer = new AccelerometerData() { TimeStamp = acc.TimeStamp.Ticks, accX = acc.accX, accY = acc.accY, accZ = acc.accZ };
                }
                );

                wpfServicePort.Post(invoke);

                Arbiter.Activate(TaskQueue,
                    invoke.ResponsePort.Choice(
                        s => { }, // delegate for success
                        ex => { } //Tracer.Error(ex) // delegate for failure
                ));
            }
        }

        void trpbUpdateDirectionNotification(proxibrick.UpdateDirectionData update)
        {
            //LogInfo("DriveBehaviorServiceBase: trpbUpdateDirectionNotification()");

            //Tracer.Trace("DriveBehaviorServiceBase:  trpbUpdateDirectionNotification()");

            if (USE_DIRECTION_PROXIBRICK)
            {
                try
                {
                    proxibrick.DirectionDataDssSerializable newDir = update.Body;

                    setCurrentDirection(newDir);
                }
                catch (Exception exc)
                {
                    Tracer.Trace("trpbUpdateDirectionNotification() - " + exc);
                }
            }
        }

        /// <summary>
        /// called when the AHRS/Compass reports new orientation. Preserves bearing to the goal.
        /// </summary>
        /// <param name="newDir"></param>
        protected void setCurrentDirection(proxibrick.DirectionDataDssSerializable newDir)
        {
            proxibrick.DirectionDataDssSerializable curDir = _state.MostRecentDirection;

            if (curDir == null || Math.Abs(newDir.heading - curDir.heading) > 0.9d || DateTime.Now > curDir.TimeStamp.AddSeconds(3.0d)) // only react on significant changes in direction
            {
                _state.MostRecentDirection = newDir;  //.Clone()

                //Tracer.Trace("heading: " + newDir.heading);

                if (_mapperVicinity.turnState != null && !_mapperVicinity.turnState.hasFinished) { _mapperVicinity.turnState.directionCurrent = new Direction() { heading = newDir.heading, TimeStamp = newDir.TimeStamp.Ticks }; }

                // update mapper with Direction data:
                _mapperVicinity.robotDirection = new Direction() { TimeStamp = newDir.TimeStamp.Ticks, heading = newDir.heading, bearing = _mapperVicinity.robotDirection.bearing };   // set same bearing to the new Direction

                // update mapper with Odometry data:
                updateMapperWithOdometryData();

                // update GUI (compass control):
                setGuiCurrentDirection(newDir);

                if (!_doUnitTest)
                {
                    Decide(SensorEventSource.Compass);
                }
            }
        }

        /// <summary>
        /// update GUI with current direction
        /// </summary>
        /// <param name="dir"></param>
        protected void setGuiCurrentDirection(proxibrick.DirectionDataDssSerializable dir)
        {
            if (_mainWindow != null)
            {
                ccrwpf.Invoke invoke = new ccrwpf.Invoke(delegate()
                {
                    _mainWindow.CurrentDirection = new DirectionData() { TimeStamp = dir.TimeStamp.Ticks, heading = dir.heading, bearing = _mapperVicinity.robotDirection.bearing };
                }
                );

                wpfServicePort.Post(invoke);

                Arbiter.Activate(TaskQueue,
                    invoke.ResponsePort.Choice(
                        s => { }, // delegate for success
                        ex => { } //Tracer.Error(ex) // delegate for failure
                ));
            }
        }

        /// <summary>
        /// update GUI with current direction
        /// </summary>
        /// <param name="dir"></param>
        protected void setGuiCurrentDirection(Direction dir)
        {
            if (_mainWindow != null)
            {
                ccrwpf.Invoke invoke = new ccrwpf.Invoke(delegate()
                {
                    _mainWindow.CurrentDirection = new DirectionData() { TimeStamp = dir.TimeStamp, heading = (double)dir.heading, bearing = _mapperVicinity.robotDirection.bearing };
                }
                );

                wpfServicePort.Post(invoke);

                Arbiter.Activate(TaskQueue,
                    invoke.ResponsePort.Choice(
                        s => { }, // delegate for success
                        ex => { } //Tracer.Error(ex) // delegate for failure
                ));
            }
        }

        void trpbUpdateProximityNotification(proxibrick.UpdateProximityData update)
        {
            //LogInfo("DriveBehaviorServiceBase: trpbUpdateProximityNotification()");

            //Tracer.Trace("DriveBehaviorServiceBase:  trpbUpdateProximityNotification()");

            if (!_mapperVicinity.robotState.ignoreProximity)
            {
                try
                {
                    proxibrick.ProximityDataDssSerializable prx = update.Body;

                    _state.MostRecentProximity = prx;

                    updateMapperWithOdometryData();

                    updateMapperWithProximityData(prx);

                    setGuiCurrentProximity(prx);

                    if (!_doUnitTest)
                    {
                        Decide(SensorEventSource.IrDirected);
                    }
                }
                catch (Exception exc)
                {
                    Tracer.Trace("trpbUpdateProximityNotification() - " + exc);
                }
            }
        }

        // this is how IR sensors are pointed on the robot (see DrawHelper for similar array):
        private static double[] angles8 = { 37.5d, 62.5d, 117.5d, 142.5d, 217.5d, 242.5d, -62.5d, -37.5d };     // eight IR sensors on the corners

        protected void updateMapperWithProximityData(proxibrick.ProximityDataDssSerializable proximityData)
        {
            double[] arrangedForMapper = new double[8];

            arrangedForMapper[4] = proximityData.mfl;
            arrangedForMapper[5] = proximityData.mffl;
            arrangedForMapper[6] = proximityData.mffr;
            arrangedForMapper[7] = proximityData.mfr;

            arrangedForMapper[3] = proximityData.mbl;
            arrangedForMapper[2] = proximityData.mbbl;
            arrangedForMapper[1] = proximityData.mbbr;
            arrangedForMapper[0] = proximityData.mbr;

            for (int i = 0; i < arrangedForMapper.GetLength(0); i++)
            {
                double rangeMeters = arrangedForMapper[i];

                if (rangeMeters < 1.4d)
                {
                    rangeMeters += robotCornerDistanceMeters;      // sensor is on the corner, adjust for that

                    double relBearing = angles8[i] + 90.0d;

                    GeoPosition pos1 = (GeoPosition)_mapperVicinity.robotPosition.Clone();

                    pos1.translate(new Direction() { heading = _mapperVicinity.robotDirection.heading, bearingRelative = relBearing }, new Distance(rangeMeters));

                    DetectedObstacle dobst1 = new DetectedObstacle()
                    {
                        geoPosition = pos1,
                        firstSeen = proximityData.TimeStamp.Ticks,
                        lastSeen = proximityData.TimeStamp.Ticks,
                        detectorType = DetectorType.IR_DIRECTED,
                        objectType = DetectedObjectType.Obstacle
                    };

                    dobst1.SetColorByType();

                    lock (_mapperVicinity)
                    {
                        _mapperVicinity.Add(dobst1);
                    }
                }
            }
        }

        protected void setGuiCurrentProximity(proxibrick.ProximityDataDssSerializable dir)
        {
            if (_mainWindow != null)
            {
                ccrwpf.Invoke invoke = new ccrwpf.Invoke(
                    delegate()
                    {
                        _mainWindow.CurrentProximity = new ProximityData()
                        {
                            TimeStamp = dir.TimeStamp.Ticks,
                            mbbl = dir.mbbl,
                            mbbr = dir.mbbr,
                            mbl = dir.mbl,
                            mbr = dir.mbr,
                            mffl = dir.mffl,
                            mffr = dir.mffr,
                            mfl = dir.mfl,
                            mfr = dir.mfr
                        };
                    }
                );

                wpfServicePort.Post(invoke);

                Arbiter.Activate(TaskQueue,
                    invoke.ResponsePort.Choice(
                        s => { }, // delegate for success
                        ex => { } //Tracer.Trace(ex) // delegate for failure
                ));
            }
        }

        void trpbUpdateParkingSensorNotification(proxibrick.UpdateParkingSensorData update)
        {
            //LogInfo("DriveBehaviorServiceBase: trpbUpdateParkingSensorNotification()");

            //Tracer.Trace("DriveBehaviorServiceBase:  trpbUpdateParkingSensorNotification()");

            if (!_mapperVicinity.robotState.ignoreParkingSensor)
            {
                try
                {
                    proxibrick.ParkingSensorDataDssSerializable pds = update.Body;

                    _state.MostRecentParkingSensor = pds;

                    updateMapperWithOdometryData();

                    updateMapperWithParkingSensorData(pds);

                    setGuiCurrentParkingSensor(pds);

                    if (!_doUnitTest)
                    {
                        Decide(SensorEventSource.SonarDirected);
                    }
                }
                catch (Exception exc)
                {
                    Tracer.Trace("trpbUpdateParkingSensorNotification() - " + exc);
                }
            }
        }

        private Ema analogValue1Ema = new Ema(7);  // filter out spikes from imperfect kinect platform pan measurement

        void trpbUpdateAnalogNotification(proxibrick.UpdateAnalogData update)
        {
            //LogInfo("DriveBehaviorServiceBase: trpbUpdateAnalogNotification()  value=" + update.Body);

            //Tracer.Trace("DriveBehaviorServiceBase:  trpbUpdateAnalogNotification()  analogValue1=" + update.Body.analogValue1 + "    TimeStamp=" + update.Body.TimeStamp);

            //Tracer.Trace("DriveBehaviorServiceBase:  trpbUpdateAnalogNotification()  analogValue1=" + update.Body.analogValue1 + "    TimeStamp=" + update.Body.TimeStamp + "    angle=" + PanTiltAlignment.getInstance().degreesPanKinect(update.Body.analogValue1));

            try
            {
                proxibrick.AnalogDataDssSerializable pds = update.Body;
                double analogValue1 = pds.analogValue1;

                // just overwrite the values in place, no need to lock or create another object:
                _state.MostRecentAnalogValues.analogValue1 = analogValue1;
                _state.MostRecentAnalogValues.TimeStamp = pds.TimeStamp;

                analogValue1 = analogValue1Ema.Compute(analogValue1);   // filter out spikes from imperfect measurement

                _state.currentPanKinect = PanTiltAlignment.getInstance().degreesPanKinect(analogValue1);

                computeAndExecuteKinectPlatformTurn();
            }
            catch (Exception exc)
            {
                Tracer.Trace("trpbUpdateAnalogNotification() - " + exc);
            }
        }

        // this is how parking sensors are pointed on the robot (see DrawHelper for similar array):
        private static double[] angles4 = { 77.5d, 102.5d, 257.5d, -77.5d };        // four Parking Sensor heads, two looking forward, two - backwards at a slight angle

        protected void updateMapperWithParkingSensorData(proxibrick.ParkingSensorDataDssSerializable psData)
        {
            double[] arrangedForMapper = new double[4];

            arrangedForMapper[0] = psData.parkingSensorMetersRB;
            arrangedForMapper[1] = psData.parkingSensorMetersLB;
            arrangedForMapper[2] = psData.parkingSensorMetersLF;
            arrangedForMapper[3] = psData.parkingSensorMetersRF;

            for (int i = 0; i < arrangedForMapper.GetLength(0); i++)
            {
                double rangeMeters = arrangedForMapper[i];

                if (rangeMeters < 2.2d)
                {
                    rangeMeters += robotCornerDistanceMeters;      // sensor is on the corner, adjust for that

                    double relBearing = angles4[i] + 90.0d;

                    GeoPosition pos1 = (GeoPosition)_mapperVicinity.robotPosition.Clone();

                    pos1.translate(new Direction() { heading = _mapperVicinity.robotDirection.heading, bearingRelative = relBearing }, new Distance(rangeMeters));

                    DetectedObstacle dobst1 = new DetectedObstacle()
                    {
                        geoPosition = pos1,
                        firstSeen = psData.TimeStamp.Ticks,
                        lastSeen = psData.TimeStamp.Ticks,
                        detectorType = DetectorType.SONAR_DIRECTED,
                        objectType = DetectedObjectType.Obstacle
                    };

                    dobst1.SetColorByType();

                    lock (_mapperVicinity)
                    {
                        _mapperVicinity.Add(dobst1);
                    }
                }
            }
        }

        protected void setGuiCurrentParkingSensor(proxibrick.ParkingSensorDataDssSerializable dir)
        {
            if (_mainWindow != null)
            {
                ccrwpf.Invoke invoke = new ccrwpf.Invoke(
                    delegate()
                    {
                        _mainWindow.CurrentParkingSensor = new ParkingSensorData()
                        {
                            TimeStamp = dir.TimeStamp.Ticks,
                            parkingSensorMetersLB = dir.parkingSensorMetersLB,
                            parkingSensorMetersLF = dir.parkingSensorMetersLF,
                            parkingSensorMetersRB = dir.parkingSensorMetersRB,
                            parkingSensorMetersRF = dir.parkingSensorMetersRF
                        };
                    }
                );

                wpfServicePort.Post(invoke);

                Arbiter.Activate(TaskQueue,
                    invoke.ResponsePort.Choice(
                        s => { }, // delegate for success
                        ex => { } //Tracer.Trace(ex) // delegate for failure
                ));
            }
        }

        #endregion // Proxibrick handlers

    }
}
