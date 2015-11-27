//#define TRACEDEBUG
//#define TRACEDEBUGTICKS
//#define TRACELOG

using System;
using System.Collections.Generic;

using W3C.Soap;
using Microsoft.Ccr.Core;
using Microsoft.Dss.ServiceModel.Dssp;
using Microsoft.Dss.ServiceModel.DsspServiceBase;

using ccrwpf = Microsoft.Ccr.Adapters.Wpf;

using TrackRoamer.Robotics.Utility.LibSystem;
using TrackRoamer.Robotics.LibMapping;
using TrackRoamer.Robotics.Utility.LibPicSensors;

using sicklrf = Microsoft.Robotics.Services.Sensors.SickLRF.Proxy;

namespace TrackRoamer.Robotics.Services.TrackRoamerBehaviors
{
    partial class TrackRoamerBehaviorsService : DsspServiceBase
    {
        private sicklrf.State _laserData = null;     // not part of the state, but still accessible from all components

        #region Laser handlers

        /// <summary>
        /// Handles the <typeparamref name="LaserRangeFinderUpdate"/> request.
        /// </summary>
        /// <param name="update">request</param>
        protected void LaserRangeFinderUpdateHandler(LaserRangeFinderUpdate update)
        {
            //Tracer.Trace("LaserRangeFinderUpdateHandler() - Update");

            try
            {
                if (!_doSimulatedLaser)  // if simulated, ignore real data - do not call Decide()
                {
                    _laserData = (sicklrf.State)update.Body.Clone();   // laserData.DistanceMeasurements is cloned here all right

                    _state.MostRecentLaserTimeStamp = _laserData.TimeStamp;

                    updateMapperWithOdometryData();

                    if (!_mapperVicinity.robotState.ignoreLaser)        // if asked to ignore laser, we just do not update mapper with obstacles, but still call Decide() and everything else
                    {
                        updateMapperWithLaserData(_laserData);
                    }
                    else
                    {
                        lock (_mapperVicinity)
                        {
                            _mapperVicinity.computeMapPositions();
                        }
                    }

                    if (!_testBumpMode && !_state.Dropping && !_doUnitTest)
                    {
                        Decide(SensorEventSource.LaserScanning);
                    }

                    setGuiCurrentLaserData(new LaserDataSerializable() { TimeStamp = _laserData.TimeStamp.Ticks, DistanceMeasurements = (int[])_laserData.DistanceMeasurements.Clone() });
                }
            }
            catch (Exception exc)
            {
                Tracer.Trace("LaserRangeFinderUpdateHandler() - " + exc);
            }

            update.ResponsePort.Post(DefaultUpdateResponseType.Instance);
        }

        // actual physical scan range of the laser range finder:
        private const double maxReliableRangeMeters = 2.90d;        // after that distance sonar/laser beam findings are considered "no obstacle there"
        private const double minReliableRangeMeters = 0.20d;        // closer than that distance sonar/laser beam findings are considered "noise"
        private const double angleMinValue = -90.0d;
        private const double angleMaxValue = 90.0d;

        private const double forwardAngle = (angleMaxValue - angleMinValue) / 2.0d;
        private const int step = 6;   // for speed and given that we are actually dealing with sonar data, pick only Nth points. 

        protected void updateMapperWithLaserData(sicklrf.State laserData)
        {
            int numRays = laserData.DistanceMeasurements.Length;

            List<IDetectedObject> laserObjects = new List<IDetectedObject>(numRays / step + 5);

            for (int i = 0; i < laserData.DistanceMeasurements.Length; i += step)
            {
                double rangeMeters = laserData.DistanceMeasurements[i] / 1000.0d;  // DistanceMeasurements is in millimeters;

                if (rangeMeters > minReliableRangeMeters && rangeMeters < maxReliableRangeMeters)
                {
                    double relBearing = forwardAngle - i * 180.0d / numRays;

                    GeoPosition pos1 = (GeoPosition)_mapperVicinity.robotPosition.Clone();

                    pos1.translate(new Direction() { heading = _mapperVicinity.robotDirection.heading, bearingRelative = relBearing }, new Distance(rangeMeters));

                    DetectedObstacle dobst1 = new DetectedObstacle()
                    {
                        geoPosition = pos1,
                        firstSeen = laserData.TimeStamp.Ticks,
                        lastSeen = laserData.TimeStamp.Ticks,
                        detectorType = DetectorType.SONAR_SCANNING,
                        objectType = DetectedObjectType.Obstacle
                    };

                    dobst1.SetColorByType();

                    laserObjects.Add(dobst1);
                }
            }

            if (laserObjects.Count > 0)
            {
                int countBefore = 0;
                lock (_mapperVicinity)
                {
                    countBefore = _mapperVicinity.Count;
                    _mapperVicinity.AddRange(laserObjects);
                    _mapperVicinity.computeMapPositions();
                }
                //Tracer.Trace(string.Format("laser ready - added {0} to {1}", laserObjects.Count, countBefore));
            }
        }

        protected void setGuiCurrentLaserData(LaserDataSerializable laserData)
        {
            if (_mainWindow != null)
            {
                ccrwpf.Invoke invoke = new ccrwpf.Invoke(
                    delegate()
                    {
                        _mainWindow.CurrentLaserData = laserData;   // update sonar sweep control
                        //_mainWindow.setMapper(_mapperVicinity);     // this is pretty much only calling Redraw on the map
                        _mainWindow.RedrawMap();
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

        /// <summary>
        /// Handles Replace notifications from the Laser partner
        /// </summary>
        /// <remarks>Posts a <typeparamref name="LaserRangeFinderUpdate"/> to itself.</remarks>
        /// <param name="replace">notification</param>
        /// <returns>task enumerator</returns>
        IEnumerator<ITask> LaserReplaceNotificationHandler(sicklrf.Replace replace)
        {
            //Tracer.Trace("LaserReplaceNotificationHandler() - Replace");

            // When this handler is called a couple of notifications may
            // have piled up. We only want the most recent one.
            sicklrf.State laserData = GetMostRecentLaserNotification(replace.Body);

            LaserRangeFinderUpdate laserUpdate = new LaserRangeFinderUpdate(laserData);

            _mainPort.Post(laserUpdate);    // calls LaserRangeFinderUpdateHandler() with laserUpdate

            yield return Arbiter.Choice(
                laserUpdate.ResponsePort,
                delegate(DefaultUpdateResponseType response) { },
                delegate(Fault fault) { }
            );

            // Skip messages that have been queued up in the meantime.
            // The notification that are lingering are out of date by now.
            GetMostRecentLaserNotification(laserData);

            // Reactivate the handler.
            Activate(
                Arbiter.ReceiveWithIterator<sicklrf.Replace>(false, _laserNotify, LaserReplaceNotificationHandler)
            );

            yield break;
        }

        /// <summary>
        /// Gets the most recent laser notification. Older notifications are dropped.
        /// </summary>
        /// <param name="laserData">last known laser data</param>
        /// <returns>most recent laser data</returns>
        private sicklrf.State GetMostRecentLaserNotification(sicklrf.State laserData)
        {
            sicklrf.Replace testReplace;

            // _laserNotify is a PortSet<>, P3 represents IPort<sicklrf.Replace> that
            // the portset contains
            int count = _laserNotify.P3.ItemCount - 1;

            for (int i = 0; i < count; i++)
            {
                testReplace = _laserNotify.Test<sicklrf.Replace>();
                if (testReplace.Body.TimeStamp > laserData.TimeStamp)
                {
                    laserData = testReplace.Body;
                }
            }

            if (count > 0)
            {
                LogInfo(string.Format("Dropped {0} laser readings (laser start)", count));
            }
            return laserData;
        }

        /// <summary>
        /// Handles the reset notification of the Laser partner.
        /// </summary>
        /// <remarks>Posts a <typeparamref name="LaserRangeFinderResetUpdate"/> to itself.</remarks>
        /// <param name="reset">notification</param>
        void LaserResetNotification(sicklrf.Reset reset)
        {
            _mainPort.Post(new LaserRangeFinderResetUpdate(reset.Body));
        }

        /// <summary>
        /// Handle the <typeparamref name="LaserRangeFinderResetUpdate"/> request.
        /// </summary>
        /// <remarks>Stops the robot.</remarks>
        /// <param name="update">request</param>
        void LaserRangeFinderResetUpdateHandler(LaserRangeFinderResetUpdate update)
        {
            if (_state.MovingState != MovingState.Unknown)
            {
                LogInfo("Stop requested: laser reported reset");
                StopMoving();

                _state.MovingState = MovingState.Unknown;
                _state.Countdown = 0;
            }
            update.ResponsePort.Post(DefaultUpdateResponseType.Instance);
        }

        #endregion
    }
}
