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
using System.ComponentModel;
using Microsoft.Ccr.Core;
using Microsoft.Dss.Core;
using Microsoft.Dss.Core.Attributes;
using Microsoft.Dss.ServiceModel.Dssp;
using Microsoft.Dss.ServiceModel.DsspServiceBase;
using W3C.Soap;

using submgr = Microsoft.Dss.Services.SubscriptionManager;

using gps = Microsoft.Robotics.Services.Sensors.Gps.Proxy;
using chrum6orientationsensor = TrackRoamer.Robotics.Hardware.ChrUm6OrientationSensor.Proxy;

using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Media3D;
using ccrwpf = Microsoft.Ccr.Adapters.Wpf;

using TrackRoamer.Robotics.Utility.LibSystem;
using libguiwpf = TrackRoamer.Robotics.LibGuiWpf;
using TrackRoamer.Robotics.Utility.LibPicSensors;
using TrackRoamer.Robotics.LibMapping;
using TrackRoamer.Robotics.LibBehavior;

namespace TrackRoamer.Robotics.Services.TrackroamerLocationAndMapping
{
    [Contract(Contract.Identifier)]
    [DisplayName("TrackroamerLocationAndMapping")]
    [Description("TrackroamerLocationAndMapping service (no description provided)")]
    class TrackroamerLocationAndMappingService : DsspServiceBase
    {
        /// <summary>
        /// Service state
        /// </summary>
        [ServiceState]
        TrackroamerLocationAndMappingState _state = new TrackroamerLocationAndMappingState();

        /// <summary>
        /// Main service port
        /// </summary>
        [ServicePort("/TrackroamerLocationAndMapping", AllowMultipleInstances = true)]
        TrackroamerLocationAndMappingOperations _mainPort = new TrackroamerLocationAndMappingOperations();

        [SubscriptionManagerPartner]
        submgr.SubscriptionManagerPort _submgrPort = new submgr.SubscriptionManagerPort();

        /// <summary>
        /// MicrosoftGpsService partner
        /// </summary>
        [Partner("MicrosoftGpsService", Contract = gps.Contract.Identifier, CreationPolicy = PartnerCreationPolicy.UseExistingOrCreate)]
        gps.MicrosoftGpsOperations _microsoftGpsServicePort = new gps.MicrosoftGpsOperations();
        gps.MicrosoftGpsOperations _microsoftGpsServiceNotify = new gps.MicrosoftGpsOperations();

        /// <summary>
        /// ChrUm6OrientationSensorService partner
        /// </summary>
        [Partner("ChrUm6OrientationSensorService", Contract = chrum6orientationsensor.Contract.Identifier, CreationPolicy = PartnerCreationPolicy.UseExistingOrCreate)]
        chrum6orientationsensor.ChrUm6OrientationSensorOperations _chrUm6OrientationSensorServicePort = new chrum6orientationsensor.ChrUm6OrientationSensorOperations();
        chrum6orientationsensor.ChrUm6OrientationSensorOperations _chrUm6OrientationSensorServiceNotify = new chrum6orientationsensor.ChrUm6OrientationSensorOperations();

        // port for WPF window:
        protected ccrwpf.WpfServicePort _wpfServicePort;

        // maintain a local mapper and pass it to UI when necessary:
        protected MapperVicinity _mapperVicinity = new MapperVicinity();
        protected RoutePlanner _routePlanner;
        protected double robotCornerDistanceMeters = 0.0d;   // to account for robot dimensions when adding measurements from corner-located proximity sensors.

        protected double _currentGoalBearing = 30.0d;

        /// <summary>
        /// Service constructor
        /// </summary>
        public TrackroamerLocationAndMappingService(DsspServiceCreationPort creationPort)
            : base(creationPort)
        {
            _routePlanner = new RoutePlanner(_mapperVicinity);

            _mapperVicinity.robotDirection = new Direction() { heading = 45.0d, bearing = 110.0d };

        }

        /// <summary>
        /// Service start
        /// </summary>
        protected override void Start()
        {
            // 
            // Add service specific initialization here
            // 

            // Start listening to CH Robotics UM6 Orientation Sensor:
            SubscribeToChrUm6OrientationSensor();

            // Start listening to GPS:
            SubscribeToGps();

            SpawnIterator(InitializeGui);

            base.Start();
        }

        #region InitializeGui - WPF

        protected libguiwpf.MainWindow _mainWindow = null;

        private IEnumerator<ITask> InitializeGui()
        {
            // create WPF adapter
            this._wpfServicePort = ccrwpf.WpfAdapter.Create(TaskQueue);

            var runWindow = this._wpfServicePort.RunWindow(() => new libguiwpf.MainWindow());

            yield return (Choice)runWindow;

            var exception = (Exception)runWindow;
            if (exception != null)
            {
                LogError(exception);
                StartFailed();
                yield break;
            }

            // need double cast because WPF adapter doesn't know about derived window types
            var userInterface = ((Window)runWindow) as libguiwpf.MainWindow;
            if (userInterface == null)
            {
                var e = new ApplicationException("User interface was expected to be libguiwpf.MainWindow");
                LogError(e);
                throw e;
            }
            _mainWindow = userInterface;
            _mainWindow.Closing += new CancelEventHandler(_mainWindow_Closing);

            // for convenience mark the initial robot position:
            DetectedObstacle dobst1 = new DetectedObstacle()
            {
                geoPosition = (GeoPosition)_mapperVicinity.robotPosition.Clone(),
                firstSeen = DateTime.Now.Ticks,
                lastSeen = DateTime.Now.Ticks,
                color = Colors.Green,
                detectorType = DetectorType.NONE,
                objectType = DetectedObjectType.Mark,
                timeToLiveSeconds = 3600
            };

            lock (_mapperVicinity)
            {
                _mapperVicinity.Add(dobst1);
            }

            _mainWindow.setMapper(_mapperVicinity, _routePlanner);
        }

        void _mainWindow_Closing(object sender, CancelEventArgs e)
        {
            _mainWindow = null;
        }
        #endregion // InitializeGui - WPF

        #region Subscribe to GPS and UM6

        /// <summary>
        /// Subscribe to the ChrUm6OrientationSensor service
        /// </summary>
        private void SubscribeToChrUm6OrientationSensor()
        {
            Tracer.Trace("SubscribeToChrUm6OrientationSensor()");

            Type[] notifyMeOf = new Type[] { 
                    // choose those which you need (and which your UM6 is actually configured to send):
                    typeof(chrum6orientationsensor.ProcGyroNotification),
                    typeof(chrum6orientationsensor.ProcAccelNotification),
                    typeof(chrum6orientationsensor.ProcMagNotification),
                    typeof(chrum6orientationsensor.EulerNotification),
                    typeof(chrum6orientationsensor.QuaternionNotification)
                };

            // Subscribe to the ChrUm6OrientationSensor service, receive notifications on the _microsoftChrUm6OrientationSensorServiceNotify.
            _chrUm6OrientationSensorServicePort.Subscribe(_chrUm6OrientationSensorServiceNotify, notifyMeOf);

            // Start listening for updates from the ChrUm6OrientationSensor service.
            Activate(
                    Arbiter.Interleave(
                        new TeardownReceiverGroup(),
                        new ExclusiveReceiverGroup(),
                        new ConcurrentReceiverGroup(
                            Arbiter.Receive<chrum6orientationsensor.ProcGyroNotification>(true, (Port<chrum6orientationsensor.ProcGyroNotification>)_chrUm6OrientationSensorServiceNotify[typeof(chrum6orientationsensor.ProcGyroNotification)], ChrProcGyroHandler),
                            Arbiter.Receive<chrum6orientationsensor.ProcAccelNotification>(true, (Port<chrum6orientationsensor.ProcAccelNotification>)_chrUm6OrientationSensorServiceNotify[typeof(chrum6orientationsensor.ProcAccelNotification)], ChrProcAccelHandler),
                            Arbiter.Receive<chrum6orientationsensor.ProcMagNotification>(true, (Port<chrum6orientationsensor.ProcMagNotification>)_chrUm6OrientationSensorServiceNotify[typeof(chrum6orientationsensor.ProcMagNotification)], ChrProcMagHandler),
                            Arbiter.Receive<chrum6orientationsensor.EulerNotification>(true, (Port<chrum6orientationsensor.EulerNotification>)_chrUm6OrientationSensorServiceNotify[typeof(chrum6orientationsensor.EulerNotification)], ChrEulerHandler),
                            Arbiter.Receive<chrum6orientationsensor.QuaternionNotification>(true, (Port<chrum6orientationsensor.QuaternionNotification>)_chrUm6OrientationSensorServiceNotify[typeof(chrum6orientationsensor.QuaternionNotification)], ChrQuaternionHandler)
                        )
                    )
                );
        }

        /// <summary>
        /// Subscribe to the GPS service
        /// </summary>
        private void SubscribeToGps()
        {
            Tracer.Trace("SubscribeToGps()");

            Type[] notifyMeOf = new Type[] { 
                    // choose those which you need (and which your GPS actually sends):
                    typeof(gps.GpGgaNotification),      // Altitude and backup position
                    typeof(gps.GpGllNotification),      // Primary Position
                    typeof(gps.GpGsaNotification),      // Precision
                    typeof(gps.GpGsvNotification),      // Satellites
                    typeof(gps.GpRmcNotification),      // Backup course, speed, and position
                    typeof(gps.GpVtgNotification)       // Ground Speed and Course
                };

            // Subscribe to the GPS service, receive notifications on the _microsoftGpsServiceNotify.
            _microsoftGpsServicePort.Subscribe(_microsoftGpsServiceNotify, notifyMeOf);

            // Start listening for updates from the GPS and UM6 services:
                Activate(
                    Arbiter.Interleave(
                        new TeardownReceiverGroup(),
                        new ExclusiveReceiverGroup(),
                        new ConcurrentReceiverGroup(
                            Arbiter.Receive<gps.GpGgaNotification>(true, (Port<gps.GpGgaNotification>)_microsoftGpsServiceNotify[typeof(gps.GpGgaNotification)], GpGgaHandler),
                            Arbiter.Receive<gps.GpGllNotification>(true, (Port<gps.GpGllNotification>)_microsoftGpsServiceNotify[typeof(gps.GpGllNotification)], GpGllHandler),
                            Arbiter.Receive<gps.GpGsaNotification>(true, (Port<gps.GpGsaNotification>)_microsoftGpsServiceNotify[typeof(gps.GpGsaNotification)], GpGsaHandler),
                            Arbiter.Receive<gps.GpGsvNotification>(true, (Port<gps.GpGsvNotification>)_microsoftGpsServiceNotify[typeof(gps.GpGsvNotification)], GpGsvHandler),
                            Arbiter.Receive<gps.GpRmcNotification>(true, (Port<gps.GpRmcNotification>)_microsoftGpsServiceNotify[typeof(gps.GpRmcNotification)], GpRmcHandler),
                            Arbiter.Receive<gps.GpVtgNotification>(true, (Port<gps.GpVtgNotification>)_microsoftGpsServiceNotify[typeof(gps.GpVtgNotification)], GpVtgHandler)
                        )
                    )
                );
        }

        #endregion // Subscribe to GPS and UM6

        #region Handle GPS messages

        /// <summary>
        /// Handle GPS GPGGA Notifications - Altitude and backup position
        /// </summary>
        /// <param name="notification">GPGGA packet notification</param>
        private void GpGgaHandler(gps.GpGgaNotification notification)
        {
            Tracer.Trace(string.Format("the GPS reported GPGGA: {0}  Lat: {1}  Lon: {2}", notification.Body.LastUpdate, notification.Body.Latitude, notification.Body.Longitude));
        }

        /// <summary>
        /// Handle GPS GPGLL Notifications - Primary Position
        /// </summary>
        /// <param name="notification">GPGLL packet notification</param>
        private void GpGllHandler(gps.GpGllNotification notification)
        {
            Tracer.Trace(string.Format("the GPS reported GPGLL: {0}  Lat: {1}  Lon: {2}", notification.Body.LastUpdate, notification.Body.Latitude, notification.Body.Longitude));
        }

        /// <summary>
        /// Handle GPS GPGSA Notifications - Precision
        /// </summary>
        /// <param name="notification">GPGSA packet notification</param>
        private void GpGsaHandler(gps.GpGsaNotification notification)
        {
            Tracer.Trace(string.Format("the GPS reported GPGSA: {0} Precision: {1}  Mode: {2}", notification.Body.LastUpdate, notification.Body.HorizontalDilutionOfPrecision, notification.Body.Mode));
        }

        /// <summary>
        /// Handle GPS GPGSV Notifications - Satellites
        /// </summary>
        /// <param name="notification">GPGSV packet notification</param>
        private void GpGsvHandler(gps.GpGsvNotification notification)
        {
            Tracer.Trace(string.Format("the GPS reported GPGSV: {0} Satellites in View: {1}   {2}", notification.Body.LastUpdate, notification.Body.GsvSatellites.Count, notification.Body.SatellitesInView));
        }

        /// <summary>
        /// Handle GPS GPRMC Notifications - Backup course, speed, and position (Sirf III sends this)
        /// </summary>
        /// <param name="notification">GPRMC packet notification</param>
        private void GpRmcHandler(gps.GpRmcNotification notification)
        {
            Tracer.Trace(string.Format("the GPS reported GPRMC: {0}  Lat: {1}  Lon: {2}", notification.Body.LastUpdate, notification.Body.Latitude, notification.Body.Longitude));
        }

        /// <summary>
        /// Handle GPS GPVTG Notifications - Ground Speed and Course
        /// </summary>
        /// <param name="notification">GPVTG packet notification</param>
        private void GpVtgHandler(gps.GpVtgNotification notification)
        {
            Tracer.Trace(string.Format("the GPS reported GPVTG: {0}  CourseDegrees: {1}  SpeedMetersPerSecond: {2}", notification.Body.LastUpdate, notification.Body.CourseDegrees, notification.Body.SpeedMetersPerSecond));
        }

        #endregion // Handle GPS messages

        #region Handle UM6 messages

        /// <summary>
        /// Handle CH Robotics UM6 Orientation Sensor Notification - ProcGyro
        /// </summary>
        /// <param name="notification">ProcGyro notification</param>
        private void ChrProcGyroHandler(chrum6orientationsensor.ProcGyroNotification notification)
        {
            Tracer.Trace(string.Format("the UM6 Sensor reported ProcGyro: {0}   {1}   {2}   {3}", notification.Body.LastUpdate, notification.Body.xRate, notification.Body.yRate, notification.Body.zRate));
        }

        /// <summary>
        /// Handle CH Robotics UM6 Orientation Sensor Notification - ProcAccel
        /// </summary>
        /// <param name="notification">ProcAccel notification</param>
        private void ChrProcAccelHandler(chrum6orientationsensor.ProcAccelNotification notification)
        {
            Tracer.Trace(string.Format("the UM6 Sensor reported ProcAccel: {0}   {1}   {2}   {3}", notification.Body.LastUpdate, notification.Body.xAccel, notification.Body.yAccel, notification.Body.zAccel));
        }

        /// <summary>
        /// Handle CH Robotics UM6 Orientation Sensor Notification - ProcMag
        /// </summary>
        /// <param name="notification">ProcMag notification</param>
        private void ChrProcMagHandler(chrum6orientationsensor.ProcMagNotification notification)
        {
            Tracer.Trace(string.Format("the UM6 Sensor reported ProcMag: {0}   {1}   {2}   {3}", notification.Body.LastUpdate, notification.Body.x, notification.Body.y, notification.Body.z));
        }

        private DirectionData MostRecentDirection = null;
        private const double CHR_EILER_YAW_FACTOR = 180.0d / 16200.0d;      // mag heading is reported as a ahort within +-16200 range. This is to convert it to degrees.

        /// <summary>
        /// Handle CH Robotics UM6 Orientation Sensor Notification - Euler
        /// </summary>
        /// <param name="notification">Euler notification</param>
        private void ChrEulerHandler(chrum6orientationsensor.EulerNotification notification)
        {
            Tracer.Trace(string.Format("the UM6 Sensor reported Euler: {0}  PHI={1}   THETA={2}   PSI={3}", notification.Body.LastUpdate, notification.Body.phi, notification.Body.theta, notification.Body.psi));

            try
            {
                // mag heading is reported as a ahort within +-16200 range
                double magHeading = Direction.to180(notification.Body.psi * CHR_EILER_YAW_FACTOR); // convert to degrees and ensure that it is within +- 180 degrees.

                DirectionData newDir = new DirectionData() { TimeStamp = DateTime.Now.Ticks, heading = magHeading };

                DirectionData curDir = MostRecentDirection;

                if (curDir == null || Math.Abs(newDir.heading - curDir.heading) > 0.9d) // only react on significant changes in direction
                {
                    MostRecentDirection = newDir;  //.Clone()

                    if (_mapperVicinity.robotDirection.bearing.HasValue)
                    {
                        _currentGoalBearing = (double)_mapperVicinity.robotDirection.bearing;
                    }

                    if (_mapperVicinity.turnState != null && !_mapperVicinity.turnState.hasFinished)
                    {
                        _mapperVicinity.turnState.directionCurrent = new Direction() { heading = newDir.heading, TimeStamp = newDir.TimeStamp };
                    }

                    // update mapper with Direction data:
                    _mapperVicinity.robotDirection = new Direction() { TimeStamp = newDir.TimeStamp, heading = newDir.heading, bearing = _currentGoalBearing };

                    // update mapper with Odometry data:
                    //updateMapperWithOdometryData();

                    // update GUI (compass control):
                    setGuiCurrentDirection(newDir);

                    //if (!_doUnitTest)
                    //{
                    //    Decide(SensorEventSource.Compass, null);
                    //}
                }
            }
            catch (Exception exc)
            {
                Tracer.Trace("ChrEulerHandler() - " + exc);
            }
        }

        protected void setGuiCurrentDirection(DirectionData dir)
        {
            if (_mainWindow != null)
            {
                ccrwpf.Invoke invoke = new ccrwpf.Invoke(delegate()
                {
                    _mainWindow.CurrentDirection = new DirectionData() { TimeStamp = dir.TimeStamp, heading = dir.heading, bearing = _currentGoalBearing };
                }
                );

                _wpfServicePort.Post(invoke);

                Arbiter.Activate(TaskQueue,
                    invoke.ResponsePort.Choice(
                        s => { }, // delegate for success
                        ex => { } //Tracer.Error(ex) // delegate for failure
                ));
            }
        }

        /// <summary>
        /// Handle CH Robotics UM6 Orientation Sensor Notification - Quaternion
        /// </summary>
        /// <param name="notification">Quaternion notification</param>
        private void ChrQuaternionHandler(chrum6orientationsensor.QuaternionNotification notification)
        {
            Tracer.Trace(string.Format("the UM6 Sensor reported Quaternion: {0}   {1}   {2}   {3}   {4}", notification.Body.LastUpdate, notification.Body.a, notification.Body.b, notification.Body.c, notification.Body.d));

            try
            {
                // we switch a and b here to match WPF quaternion's orientation. Maybe there is a proper transformation to do it, but this seems to work as well.
                Quaternion aq = new Quaternion(notification.Body.b, notification.Body.a, notification.Body.c, notification.Body.d); // X, Y, Z, W components in WPF correspond to a, b, c, d in CH UM6 and Wikipedia

                // have to turn it still around the Y axis (facing East):
                Vector3D axis = new Vector3D(0, 1, 0);
                aq = aq * new Quaternion(axis, 180);

                libguiwpf.OrientationData attitudeData = new libguiwpf.OrientationData() { timestamp = notification.Body.LastUpdate, attitudeQuaternion = aq };

                setGuiCurrentAttitude(attitudeData);
            }
            catch (Exception exc)
            {
                Tracer.Trace("ChrQuaternionHandler() - " + exc);
            }
        }

        protected void setGuiCurrentAttitude(libguiwpf.OrientationData attitudeData)
        {
            if (_mainWindow != null)
            {
                ccrwpf.Invoke invoke = new ccrwpf.Invoke(delegate()
                {
                    _mainWindow.CurrentAttitude = attitudeData;
                }
                );

                _wpfServicePort.Post(invoke);

                Arbiter.Activate(TaskQueue,
                    invoke.ResponsePort.Choice(
                        s => { }, // delegate for success
                        ex => { } //Tracer.Error(ex) // delegate for failure
                ));
            }
        }

        #endregion // Handle UM6 messages

        /// <summary>
        /// Handles Subscribe messages (when another service subscribes to this one)
        /// </summary>
        /// <param name="subscribe">the subscribe request</param>
        [ServiceHandler]
        public void SubscribeHandler(Subscribe subscribe)
        {
            Tracer.Trace("SubscribeHandler() received Subscription request from Subscriber=" + subscribe.Body.Subscriber);

            SubscribeHelper(_submgrPort, subscribe.Body, subscribe.ResponsePort);
        }
    }
}


