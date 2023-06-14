
//#define TRACEDEBUG
//#define TRACEDEBUGTICKS
//#define TRACELOG

#region The Using statements

using System;
using System.Collections.Generic;
using System.Linq;
using System.ComponentModel;

using System.IO;
using System.Net;
using System.Net.Mime;
using System.Windows.Media;
using System.Windows.Media.Media3D;

using W3C.Soap;
using Microsoft.Ccr.Core;
using Microsoft.Dss.Core;
using Microsoft.Dss.Core.Utilities;
using Microsoft.Dss.Core.Attributes;
using Microsoft.Dss.ServiceModel.Dssp;
using Microsoft.Dss.Services.Serializer;
using Microsoft.Dss.ServiceModel.DsspServiceBase;
using Microsoft.Dss.Core.DsspHttp;
using Microsoft.Dss.Core.DsspHttpUtilities;
using Microsoft.Robotics.Common;
using rpm=Microsoft.Robotics.PhysicalModel;
using common = Microsoft.Robotics.Common;

using System.Windows;
using ccrwpf = Microsoft.Ccr.Adapters.Wpf;

using TrackRoamer.Robotics.Utility.LibSystem;
using TrackRoamer.Robotics.LibMapping;
using TrackRoamer.Robotics.LibBehavior;
using TrackRoamer.Robotics.Utility.LibPicSensors;
using libguiwpf = TrackRoamer.Robotics.LibGuiWpf;

using bumper = Microsoft.Robotics.Services.ContactSensor.Proxy;
//using bumper = TrackRoamer.Robotics.Services.TrackRoamerServices.Bumper.Proxy;
using drive = Microsoft.Robotics.Services.Drive.Proxy;
using trdrive = TrackRoamer.Robotics.Services.TrackRoamerDrive.Proxy;
using encoder = Microsoft.Robotics.Services.Encoder.Proxy;
using sicklrf = Microsoft.Robotics.Services.Sensors.SickLRF.Proxy;
using powerbrick = TrackRoamer.Robotics.Services.TrackRoamerBrickPower.Proxy;
using proxibrick = TrackRoamer.Robotics.Services.TrackRoamerBrickProximityBoard.Proxy;
using gps = Microsoft.Robotics.Services.Sensors.Gps.Proxy;
using chrum6orientationsensor = TrackRoamer.Robotics.Hardware.ChrUm6OrientationSensor.Proxy;
using pololumaestro = TrackRoamer.Robotics.Hardware.PololuMaestroService.Proxy;
using animhead = TrackRoamer.Robotics.Services.AnimatedHeadService.Proxy;

using kinect = Microsoft.Robotics.Services.Sensors.Kinect;
using kinectProxy = Microsoft.Robotics.Services.Sensors.Kinect.Proxy;
using nui = Microsoft.Kinect;
using depthcam = Microsoft.Robotics.Services.DepthCamSensor;
using sr = Microsoft.Robotics.Services.Sensors.Kinect.MicArraySpeechRecognizer.Proxy;

using dssp = Microsoft.Dss.ServiceModel.Dssp;

#endregion // The Using statements

namespace TrackRoamer.Robotics.Services.TrackRoamerBehaviors
{
	/// <summary>
	/// takes care of all the DSS Service plumbing to allow derived class handle higher level behavior related code
	/// </summary>
    // The ActivationSettings attribute with Sharing == false makes the runtime dedicate a dispatcher thread pool just for this service.
    // ExecutionUnitsPerDispatcher	- Indicates the number of execution units allocated to the dispatcher
    // ShareDispatcher	            - Inidicates whether multiple service instances can be pooled or not
    [ActivationSettings(ShareDispatcher = false, ExecutionUnitsPerDispatcher = 8)]
    [DisplayName("(User) TrackRoamer Behaviors")]
    [Description("The TrackRoamer Behaviors Service")]
    [Contract(Contract.Identifier)]
    partial class TrackRoamerBehaviorsService : DsspServiceBase
	{
        protected const int settlingTime = 500;         // Time to wait, ms, after each move to let things settle

        // set which devices you want to use for direction and orientation. It is better on UM6, but proxibrick will do.
        protected bool USE_ORIENTATION_UM6 = true;
        protected bool USE_DIRECTION_UM6_QUATERNION = true;
        protected bool USE_DIRECTION_UM6_EULER = false;

        protected bool USE_DIRECTION_PROXIBRICK = false;
        protected bool USE_ORIENTATION_PROXIBRICK = false;

        public double PowerScale { get { return _state.PowerScale; } set { _state.PowerScale = value; } }   // all power and speed below are multiplied by PowerScale
                                                                                                            // also see MotorPowerScalingFactor=100 in the TrackRoamerDriveTypes.cs

        public const double UnscaledMaximumForwardVelocityMmSec = 300.0d;       // mm/sec
        public const double UnscaledModerateForwardVelocityMmSec = 200.0d;      // mm/sec
        public const double UnscaledMinimumForwardVelocityMmSec = 100.0d;       // mm/sec

        public const double UnscaledMaximumBackwardVelocityMmSec = 250.0d;      // mm/sec
        public const double UnscaledModerateBackwardVelocityMmSec = 180.0d;     // mm/sec
        public const double UnscaledMinimumBackwardVelocityMmSec = 100.0d;      // mm/sec

        public const double UnscaledMaximumTurnPower = 0.20d;                   // of 1.0=full power
        public const double UnscaledModerateTurnPower = 0.14d;                  // of 1.0=full power
        public const double UnscaledMinimumTurnPower = 0.09d;                   // of 1.0=full power


        /// <summary>
        /// The max. velocity with which to move. Velocity translates to power -1...1 by dividing velocity by 1000, so velocity must be below 1000
        /// It is then multiplied by PowerScale to slow down the robot.
        /// </summary>
        public double MaximumForwardVelocityMmSec   { get { return UnscaledMaximumForwardVelocityMmSec * PowerScale; } }    // mm/sec
        public double ModerateForwardVelocityMmSec  { get { return UnscaledModerateForwardVelocityMmSec * PowerScale; } }   // mm/sec
        public double MinimumForwardVelocityMmSec   { get { return UnscaledMinimumForwardVelocityMmSec * PowerScale; } }    // mm/sec

        public double MaximumBackwardVelocityMmSec  { get { return UnscaledMaximumBackwardVelocityMmSec * PowerScale; } }   // mm/sec
        public double ModerateBackwardVelocityMmSec { get { return UnscaledModerateBackwardVelocityMmSec * PowerScale; } }  // mm/sec
        public double MinimumBackwardVelocityMmSec  { get { return UnscaledMinimumBackwardVelocityMmSec * PowerScale; } }   // mm/sec

        /// <summary>
        /// The max. power with which to turn.
        /// It is multiplied by PowerScale to slow down the robot.
        /// </summary>
        public double MaximumTurnPower      { get { return UnscaledMaximumTurnPower * PowerScale; } }       // of 1.0=full power
        public double ModerateTurnPower     { get { return UnscaledModerateTurnPower * PowerScale; } }      // of 1.0=full power
        public double MinimumTurnPower      { get { return UnscaledMinimumTurnPower * PowerScale; } }       // of 1.0=full power

        private const double turnHeadingFactor = 1.0d;      // account for overshooting on the turn

        protected const int backupDistanceSteepTurn = 200;  // mm to move back before turning 45 degrees or more

        // Note: 1. See MinimumMotorPower in TrackRoamerServices\TrackRoamerMotor.cs  - it defines the minimum value for TurnPower and speed
        //       2. The power is used for turns, speed for drive. Speed = Power*1000
        //       3. motor with encoder feedback is driven for wheel speed. Setting the power on turns actually defines (requests) certain wheel speed (specifically, Power*1000).

        #region main port and state

        [InitialStatePartner(Optional = true)]
        [ServiceState(StateTransform = "TrackRoamer.Robotics.Services.TrackRoamerBehaviors.TrackRoamerBehaviors.xslt")]
        protected TrackRoamerBehaviorsState _state = new TrackRoamerBehaviorsState();

		[ServicePort("/trackroamerbehaviors", AllowMultipleInstances = false)]
		protected TrackRoamerBehaviorsOperations _mainPort = new TrackRoamerBehaviorsOperations();
		#endregion

        protected ccrwpf.WpfServicePort wpfServicePort;

        // maintain a local mapper and pass it to UI when necessary:
        protected MapperVicinity _mapperVicinity = new MapperVicinity();
        protected RoutePlanner _routePlanner;

        protected DsspHttpUtilitiesPort _httpUtilities;

        public class DriveStageContainer
        {
            public drive.DriveStage DriveStage;
        }

		/// <summary>
		/// If no laser data is received within this time the robot stops.
		/// </summary>
		//const int WatchdogTimeout = 500; // msec
		const int WatchdogTimeout = 10000; // msec

		/// <summary>
		/// Interval between timer notifications.
		/// </summary>
		//const int WatchdogInterval = 100; // msec
        int WatchdogInterval = 1000; // msec

		#region Partners ports

		#region Bumper partner

		//[Partner("TrackRoamerBumperService", Contract = trackroamerservices.Bumper.Proxy.Contract.Identifier, CreationPolicy = PartnerCreationPolicy.UseExisting, Optional = false)]
		//protected pxbumper.ContactSensorArrayOperations _trackroamerbumperPort = new pxbumper.ContactSensorArrayOperations();

		[Partner("Bumper", Contract = bumper.Contract.Identifier, CreationPolicy = PartnerCreationPolicy.UseExisting, Optional = false)]
		protected bumper.ContactSensorArrayOperations _bumperPort = new bumper.ContactSensorArrayOperations();
		protected bumper.ContactSensorArrayOperations _bumperNotify = new bumper.ContactSensorArrayOperations();
		#endregion

		#region Differential Drive partner
        /// <summary>
        /// DriveDifferentialTwoWheel partner. Used for DriveByDistance, TurnByAngle requests, which require wait and should go directly to the drive.
        /// </summary>
        [Partner("TrackRoamerDrive", Contract = drive.Contract.Identifier, CreationPolicy = PartnerCreationPolicy.UsePartnerListEntry, Optional = false)]
        protected drive.DriveOperations _drivePort = new drive.DriveOperations();
        protected drive.DriveOperations _driveNotify = new drive.DriveOperations();

        // This port is sent a message every time that there is a
        // Canceled or Complete message from the Drive, so it can
        // be used to wait for completion.
        protected Port<drive.DriveStage> _completionPort = new Port<drive.DriveStage>();

        #endregion  // Differential Drive partner

		#region Obstacle Avoidance Drive partner

        /// <summary>
        /// ObstacleAvoidanceDrive partner. Used for SetDrivePower requests in SetDrivePower(), which allow being tweaked/overrridden by ObstacleAvoidanceDrive intermediary and don't require wait.
        /// </summary>
        [Partner("ObstacleAvoidanceDrive", Contract = drive.Contract.Identifier, CreationPolicy = PartnerCreationPolicy.UsePartnerListEntry, Optional = false)]
        drive.DriveOperations _obstacleAvoidanceDrivePort = new drive.DriveOperations();

        #endregion // Obstacle Avoidance Drive partner

        #region Encoder partners
        // this didn't work as supposed - both subscriptions received notifications from the same encoder, no matter what I tried.
		// I resorted to receiving notifications directly from TrackRoamerBot instead, which is faster anyway.
		
        /*
        [Partner("drive:LeftEncoder",
           Optional = false,
            Contract = encoder.Contract.Identifier,
           CreationPolicy = PartnerCreationPolicy.UseExisting)]
        protected encoder.EncoderOperations _encoderPortLeft = new encoder.EncoderOperations();
        protected encoder.EncoderOperations _encoderNotifyLeft = new encoder.EncoderOperations();

        [Partner("drive:RightEncoder",
           Optional = false,
            Contract = encoder.Contract.Identifier,
           CreationPolicy = PartnerCreationPolicy.UseExisting)]
        protected encoder.EncoderOperations _encoderPortRight = new encoder.EncoderOperations();
        protected encoder.EncoderOperations _encoderNotifyRight = new encoder.EncoderOperations();
        */
		#endregion

        #region Powerbrick partner - for wheel speed and generally in case we want to consider hardware data directly
        [Partner("TrackRoamerPowerBrick", Contract = powerbrick.Contract.Identifier, CreationPolicy = PartnerCreationPolicy.UseExisting, Optional = false)]
        protected powerbrick.TrackRoamerBrickPowerOperations _trackroamerbotPort = new powerbrick.TrackRoamerBrickPowerOperations();
        private Port<powerbrick.UpdateMotorEncoder> notificationPortEncoder = new Port<powerbrick.UpdateMotorEncoder>();
        private Port<powerbrick.UpdateMotorEncoderSpeed> notificationPortEncoderSpeed = new Port<powerbrick.UpdateMotorEncoderSpeed>();
        #endregion

		#region laser range and proximity brick partners

		[Partner("Laser", Contract = sicklrf.Contract.Identifier, CreationPolicy = PartnerCreationPolicy.UseExistingOrCreate)]
		protected sicklrf.SickLRFOperations _laserPort = new sicklrf.SickLRFOperations();
		protected sicklrf.SickLRFOperations _laserNotify = new sicklrf.SickLRFOperations();

        /// <summary>
        /// TrackRoamerBrickProximityBoardService partner
        /// </summary>
        [Partner("TrackRoamerProximityBrick", Contract = proxibrick.Contract.Identifier, CreationPolicy = PartnerCreationPolicy.UseExistingOrCreate)]
        protected proxibrick.TrackRoamerBrickProximityBoardOperations _trackRoamerBrickProximityBoardServicePort = new proxibrick.TrackRoamerBrickProximityBoardOperations();
        protected proxibrick.TrackRoamerBrickProximityBoardOperations _trackRoamerBrickProximityBoardServiceNotify = new proxibrick.TrackRoamerBrickProximityBoardOperations();

        #endregion // laser range and proximity brick partners

        #region AHRS - accelerometer, gyros and compass partners

        /// <summary>
        /// ChrUm6OrientationSensorService partner
        /// </summary>
        [Partner("ChrUm6OrientationSensorService", Contract = chrum6orientationsensor.Contract.Identifier, CreationPolicy = PartnerCreationPolicy.UseExistingOrCreate)]
        protected chrum6orientationsensor.ChrUm6OrientationSensorOperations _chrUm6OrientationSensorServicePort = new chrum6orientationsensor.ChrUm6OrientationSensorOperations();
        protected chrum6orientationsensor.ChrUm6OrientationSensorOperations _chrUm6OrientationSensorServiceNotify = new chrum6orientationsensor.ChrUm6OrientationSensorOperations();

        #endregion // AHRS - accelerometer, gyros and compass partners

        #region GPS patrner

        /// <summary>
        /// MicrosoftGpsService partner
        /// </summary>
        [Partner("MicrosoftGpsService", Contract = gps.Contract.Identifier, CreationPolicy = PartnerCreationPolicy.UseExistingOrCreate)]
        protected gps.MicrosoftGpsOperations _microsoftGpsServicePort = new gps.MicrosoftGpsOperations();
        protected gps.MicrosoftGpsOperations _microsoftGpsServiceNotify = new gps.MicrosoftGpsOperations();

        #endregion // GPS patrner

        #region Pololu Maestro Device patrner

        /// <summary>
        /// Pololu Maestro Device partner
        /// </summary>
        [Partner("PololuMaestroService", Contract = pololumaestro.Contract.Identifier, CreationPolicy = PartnerCreationPolicy.UseExistingOrCreate)]
        protected pololumaestro.PololuMaestroServiceOperations _pololuMaestroPort = new pololumaestro.PololuMaestroServiceOperations();
        protected pololumaestro.PololuMaestroServiceOperations _pololuMaestroNotify = new pololumaestro.PololuMaestroServiceOperations();

        protected LightsHelper _lightsHelper = new LightsHelper();

        #endregion // Pololu Maestro Device patrner

        #region Animated Head Device partner

        /// <summary>
        /// Animated Head Device partner
        /// </summary>
        [Partner("AnimatedHeadService", Contract = animhead.Contract.Identifier, CreationPolicy = PartnerCreationPolicy.UseExistingOrCreate)]
        protected animhead.AnimatedHeadServiceOperations _animatedHeadCommandPort = new animhead.AnimatedHeadServiceOperations();
        protected animhead.AnimatedHeadServiceOperations _animatedHeadNotify = new animhead.AnimatedHeadServiceOperations();

        #endregion // Animated Head Device partner

        #region Speech recognizer partner ports

        /// <summary>
        /// Speech recognizer partner ports
        /// see C:\Microsoft Robotics Dev Studio 4\samples\Sensors\Kinect\MicArray\SpeechRecognizerGui\SpeechRecognizerGui.cs 
        /// </summary>
        [Partner("SpeechRecognizer", Contract = sr.Contract.Identifier, CreationPolicy = PartnerCreationPolicy.UseExistingOrCreate)]
        sr.SpeechRecognizerOperations speechRecoPort = new sr.SpeechRecognizerOperations();
        sr.SpeechRecognizerOperations speechRecoNotifyPort = new sr.SpeechRecognizerOperations();

        #endregion // Speech recognizer partner ports

        #endregion // partners ports

        public TrackRoamerBehaviorsService(DsspServiceCreationPort creationPort)
            : base(creationPort)
		{
            LogInfo("DriveBehaviorServiceBase:DriveBehaviorServiceBase() -- port: " + creationPort.ToString() + "  threads per CPU: " + Dispatcher.ThreadsPerCpu);

            _routePlanner = new RoutePlanner(_mapperVicinity);

            _mapperVicinity.robotDirection = new Direction() { heading = 45.0d, bearing = -110.0d };

            _panTiltAlignment = PanTiltAlignment.RestoreOrDefault();

            // Kinect UI - by default, lets show all the good stuff:
            this.IncludeDepth = true;
            this.IncludeVideo = true;
            this.IncludeSkeletons = true;
        }

		#region Start() and subsribe

        protected override void Start()
        {
            LogInfo("DriveBehaviorServiceBase:Start()  _doUnitTest=" + _doUnitTest + "   _doSimulatedLaser=" + _doSimulatedLaser + "   _testBumpMode=" + _testBumpMode);

            base.Start();   // start MainPortInterleave; wireup [ServiceHandler] methods

            InitGunTurrets();

            // HeadlightsOff(); -- this will be done by SafePositions in PololuMaestroService

            if (_state == null)
            {
                // no partner-supplied initial state found - create default one:

                LogInfo("DriveBehaviorServiceBase:: initial _state null");

                _state = new TrackRoamerBehaviorsState();

                SaveState(_state);
            }
            else
            {
                LogInfo("DriveBehaviorServiceBase:: initial _state supplied by partner");

                _state.Init();  // clear and re-create some members that should not be taken from the saved state
            }

            _httpUtilities = DsspHttpUtilitiesService.Create(Environment);

            if (_doUnitTest)
            {
                Talker.Say(9, "unit test mode");
            }

            if (_testBumpMode)
            {
                Talker.Say(9, "bumper test mode");
            }

            if (_doSimulatedLaser)
            {
                Talker.Say(9, "laser will be simulated");

                WatchdogInterval = _simulatedLaserWatchdogInterval;    // about the time ultrasonic sensor makes a sweep
            }

            if (!_doUnitTest || _unitTestSensorsOn)
            {
                // Handlers that need write or exclusive access to state go under
                // the exclusive group. Handlers that need read or shared access, and can be
                // concurrent to other readers, go to the concurrent group.
                // Other internal ports can be included in interleave so you can coordinate
                // intermediate computation with top level handlers.

                #region request handler setup
                MainPortInterleave.CombineWith(new Interleave(
                        new TeardownReceiverGroup(),
                        new ExclusiveReceiverGroup(
                            Arbiter.Receive<LaserRangeFinderResetUpdate>(true, _mainPort, LaserRangeFinderResetUpdateHandler),
                            Arbiter.Receive<LaserRangeFinderUpdate>(true, _mainPort, LaserRangeFinderUpdateHandler),
                            Arbiter.Receive<BumpersArrayUpdate>(true, _mainPort, BumpersArrayUpdateHandler),
                            Arbiter.Receive<BumperUpdate>(true, _mainPort, BumperUpdateHandler),
                            Arbiter.Receive<DriveUpdate>(true, _mainPort, DriveUpdateHandler),
                            //Arbiter.Receive<EncoderUpdate>(true, _mainPort, EncoderUpdateHandler),
                            Arbiter.Receive<WatchDogUpdate>(true, _mainPort, WatchDogUpdateHandler)
                        ),
                        new ConcurrentReceiverGroup(
                            Arbiter.Receive<Get>(true, _mainPort, GetHandler),
                            //Arbiter.Receive<HttpGet>(true, _mainPort, HttpGetHandler),
                            Arbiter.Receive<dssp.DsspDefaultLookup>(true, _mainPort, DefaultLookupHandler)
                        )
                    )
                );
                #endregion

                #region notification handler setup
                MainPortInterleave.CombineWith(new Interleave(
                        new TeardownReceiverGroup(),
                        new ExclusiveReceiverGroup(),
                        new ConcurrentReceiverGroup(
                            Arbiter.Receive<sicklrf.Reset>(true, _laserNotify, LaserResetNotification),

                            Arbiter.Receive<proxibrick.UpdateAccelerometerData>(true, _trackRoamerBrickProximityBoardServiceNotify, trpbUpdateAccelerometerNotification),
                            Arbiter.Receive<proxibrick.UpdateDirectionData>(true, _trackRoamerBrickProximityBoardServiceNotify, trpbUpdateDirectionNotification),
                            Arbiter.Receive<proxibrick.UpdateProximityData>(true, _trackRoamerBrickProximityBoardServiceNotify, trpbUpdateProximityNotification),
                            Arbiter.Receive<proxibrick.UpdateParkingSensorData>(true, _trackRoamerBrickProximityBoardServiceNotify, trpbUpdateParkingSensorNotification),
                            Arbiter.Receive<proxibrick.UpdateAnalogData>(true, _trackRoamerBrickProximityBoardServiceNotify, trpbUpdateAnalogNotification),

                            Arbiter.Receive<drive.Update>(true, _driveNotify, DriveUpdateNotification),
                            Arbiter.ReceiveWithIterator<drive.DriveDistance>(true, _driveNotify, DriveDistanceUpdateHandler),
                            Arbiter.ReceiveWithIterator<drive.RotateDegrees>(true, _driveNotify, RotateDegreesUpdateHandler),

                            //Arbiter.Receive<encoder.UpdateTickCount>(true, _encoderNotifyLeft, EncoderUpdateTickCountNotificationLeft),
                            //Arbiter.Receive<encoder.UpdateTickCount>(true, _encoderNotifyRight, EncoderUpdateTickCountNotificationRight),

                            Arbiter.Receive<powerbrick.UpdateMotorEncoder>(true, notificationPortEncoder, EncoderNotificationHandler),
                            Arbiter.Receive<powerbrick.UpdateMotorEncoderSpeed>(true, notificationPortEncoderSpeed, EncoderSpeedNotificationHandler),
                            Arbiter.Receive<bumper.Replace>(true, _bumperNotify, BumperReplaceNotification),
                            Arbiter.Receive<bumper.Update>(true, _bumperNotify, BumperUpdateNotification)
                        )
                    )
                );
                #endregion

                // We cannot replicate the activation of laser notifications because the 
                // handler uses Test() to skip old laser notifications.
                // Activate the handler once, it will re-activate itself:
                Activate(
                    Arbiter.ReceiveWithIterator<sicklrf.Replace>(false, _laserNotify, LaserReplaceNotificationHandler)
                );

                if (USE_ORIENTATION_UM6 || USE_DIRECTION_UM6_QUATERNION || USE_DIRECTION_UM6_EULER)
                {
                    // Start listening to CH Robotics UM6 Orientation Sensor:
                    SubscribeToChrUm6OrientationSensor();
                }

                // Start listening to GPS:
                SubscribeToGps();

                // Start watchdog timer - we need it to detect that the laser does not return current picture and stop:
                _mainPort.Post(new WatchDogUpdate(new WatchDogUpdateRequest(DateTime.Now)));
            }
            else
            {
                // for unit tests - drive only operation:
                #region request handler setup
                MainPortInterleave.CombineWith(new Interleave(
                        new TeardownReceiverGroup(),
                        new ExclusiveReceiverGroup(
                            Arbiter.Receive<BumpersArrayUpdate>(true, _mainPort, BumpersArrayUpdateHandler),
                            Arbiter.Receive<BumperUpdate>(true, _mainPort, BumperUpdateHandler),
                            //Arbiter.Receive<EncoderUpdate>(true, _mainPort, EncoderUpdateHandler),
                            Arbiter.Receive<DriveUpdate>(true, _mainPort, DriveUpdateHandler)
                        ),
                        new ConcurrentReceiverGroup(
                            Arbiter.Receive<Get>(true, _mainPort, GetHandler),
                            //Arbiter.Receive<HttpGet>(true, _mainPort, HttpGetHandler),
                            Arbiter.Receive<dssp.DsspDefaultLookup>(true, _mainPort, DefaultLookupHandler)
                        )
                    )
                );
                #endregion

                #region notification handler setup
                MainPortInterleave.CombineWith(new Interleave(
                        new TeardownReceiverGroup(),
                        new ExclusiveReceiverGroup(),
                        new ConcurrentReceiverGroup(
                            Arbiter.Receive<drive.Update>(true, _driveNotify, DriveUpdateNotification),
                            Arbiter.ReceiveWithIterator<drive.DriveDistance>(true, _driveNotify, DriveDistanceUpdateHandler),
                            Arbiter.ReceiveWithIterator<drive.RotateDegrees>(true, _driveNotify, RotateDegreesUpdateHandler),

                            //Arbiter.Receive<encoder.UpdateTickCount>(true, _encoderNotifyLeft, EncoderUpdateTickCountNotificationLeft),
                            //Arbiter.Receive<encoder.UpdateTickCount>(true, _encoderNotifyRight, EncoderUpdateTickCountNotificationRight),

                            Arbiter.Receive<powerbrick.UpdateMotorEncoder>(true, notificationPortEncoder, EncoderNotificationHandler),
                            Arbiter.Receive<powerbrick.UpdateMotorEncoderSpeed>(true, notificationPortEncoderSpeed, EncoderSpeedNotificationHandler),
                            Arbiter.Receive<bumper.Replace>(true, _bumperNotify, BumperReplaceNotification),
                            Arbiter.Receive<bumper.Update>(true, _bumperNotify, BumperUpdateNotification)
                        )
                    )
                );
                #endregion
            }

            // Subscribe to the direct drive for notification messages:
            _drivePort.Subscribe(_driveNotify);

            //_encoderPortLeft.Subscribe(_encoderNotifyLeft);
            //_encoderPortRight.Subscribe(_encoderNotifyRight);

            SubscribeToPowerBrick();    // for Encoders distance and speed

            _bumperPort.Subscribe(_bumperNotify);

            SpawnIterator(this.AhrsZeroGyrosAndAccelerometer);

            SpawnIterator(this.InitializeSpeechRecognizer);

            // create WPF adapter
            this.wpfServicePort = ccrwpf.WpfAdapter.Create(TaskQueue);

            SpawnIterator(InitializeGui);

            if (!_doUnitTest || _unitTestSensorsOn)
            {
                Tracer.Trace("DriveBehaviorServiceBase:Start():  calling Subscribe() for laser range, proximity, accelerometer, compass partners");

                _laserPort.Subscribe(_laserNotify);

                Type[] notifyMeOf = new Type[] { 
                    typeof(proxibrick.UpdateAccelerometerData),
                    typeof(proxibrick.UpdateDirectionData),
                    typeof(proxibrick.UpdateProximityData),
                    typeof(proxibrick.UpdateParkingSensorData),
                    typeof(proxibrick.UpdateAnalogData)
                };

                _trackRoamerBrickProximityBoardServicePort.Subscribe(_trackRoamerBrickProximityBoardServiceNotify, notifyMeOf);

                SpawnIterator(this.InitializeKinectUI);
            }

            SafePosture();

            SetLightsTest();    // SetLightsNormal() is called at the end of CalibrateKinectPlatform iteration

            SpawnIterator(this.LightsControlLoop);

            SpawnIterator(this.ConnectObstacleAvoidanceDrive);

            SpawnIterator(this.CalibrateKinectPlatform);

            SpawnIterator(this.TestAnimatedHead);

            if (_doUnitTest)
            {
                performUnitTest();
            }
            else
            {
                SpawnIterator(this.InitializeDecisionMainLoop);
            }

            // we will need this later:
            robotCornerDistanceMeters = Math.Sqrt(_mapperVicinity.robotState.robotLengthMeters * _mapperVicinity.robotState.robotLengthMeters + _mapperVicinity.robotState.robotWidthMeters * _mapperVicinity.robotState.robotWidthMeters) / 2.0d;
        }

        #region Enable Obstacle Avoidance Drive

        /// <summary>
        /// Connect to the Obstacle Avoidance Diff Drive for "Drive Forward operation
        /// </summary>
        /// <returns>An Iterator</returns>
        private IEnumerator<ITask> ConnectObstacleAvoidanceDrive()
        {
            var request = new drive.EnableDriveRequest { Enable = true };

            if (this._obstacleAvoidanceDrivePort != null)
            {
                yield return Arbiter.Choice(this._obstacleAvoidanceDrivePort.EnableDrive(request), EmptyHandler, LogError);
            }
        }

        #endregion // Enable Obstacle Avoidance Drive

        #region Subscribe to Power Brick for encoders

        /// <summary>
        /// Subscribe directly to Power Brick for encoders (distance and speed)
        /// </summary>
        private void SubscribeToPowerBrick()
        {
            Type[] notifyMeOfDistance = new Type[] { typeof(powerbrick.UpdateMotorEncoder) };

            Tracer.Trace("DriveBehaviorServiceBase: calling Subscribe() for UpdateMotorEncoder");

            // Subscribe to the TrackRoamerBot and wait for a response
            Activate(
                Arbiter.Choice(_trackroamerbotPort.Subscribe(notificationPortEncoder, notifyMeOfDistance),
                    delegate(SubscribeResponseType Rsp)
                    {
                        // Subscription was successful, update our state with subscription status:
                        LogInfo("DriveBehaviorServiceBase: Subscription to Power Brick Service for UpdateMotorEncoder succeeded");
                    },
                    delegate(Fault F)
                    {
                        LogError("DriveBehaviorServiceBase: Subscription to Power Brick Service for UpdateMotorEncoder failed");
                    }
                )
            );

            Type[] notifyMeOfSpeed = new Type[] { typeof(powerbrick.UpdateMotorEncoderSpeed) };

            Tracer.Trace("DriveBehaviorServiceBase: calling Subscribe() for UpdateMotorEncoderSpeed");

            // Subscribe to the TrackRoamerBot and wait for a response
            Activate(
                Arbiter.Choice(_trackroamerbotPort.Subscribe(notificationPortEncoderSpeed, notifyMeOfSpeed),
                    delegate(SubscribeResponseType Rsp)
                    {
                        // Subscription was successful, update our state with subscription status:
                        LogInfo("DriveBehaviorServiceBase: Subscription to Power Brick Service for UpdateMotorEncoderSpeed succeeded");
                    },
                    delegate(Fault F)
                    {
                        LogError("DriveBehaviorServiceBase: Subscription to Power Brick Service for UpdateMotorEncoderSpeed failed");
                    }
                )
            );
        }

        #endregion // Subscribe to Power Brick for encoders

        #region Subscribe to GPS and UM6

        /// <summary>
        /// Subscribe to the CH Robotics UM6 Orientation Sensor service
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

        #endregion // Start() and subsribe

        #region InitializeGui - WPF

        protected libguiwpf.MainWindow _mainWindow = null;
        protected SoundsHelper _soundsHelper = null;

        private IEnumerator<ITask> InitializeGui()
        {
            var runWindow = this.wpfServicePort.RunWindow(() => new libguiwpf.MainWindow(_state.followDirectionPidControllerAngularSpeed, _state.followDirectionPidControllerLinearSpeed, SoundSkinFactory.soundsBasePathDefault));

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
            _mainWindow.PowerScaleAdjusted += delegate
            {
                PowerScale = _mainWindow.PowerScale;
                Tracer.Trace("PowerScaleAdjusted: " + PowerScale);
            };
            _mainWindow.PidControllersUpdated += delegate
            {
                Tracer.Trace("PidControllersUpdated - saving state"); 
                SaveState(_state);
            };

            // for convenience mark the initial robot position:
            DetectedObstacle dobst1 = new DetectedObstacle()
            {
                geoPosition = (GeoPosition)_mapperVicinity.robotPosition.Clone(),
                firstSeen = DateTime.Now.Ticks,
                lastSeen = DateTime.Now.Ticks,
                detectorType = DetectorType.NONE,
                objectType = DetectedObjectType.Mark,
                timeToLiveSeconds = 3600
            };

            dobst1.SetColorByType();

            lock (_mapperVicinity)
            {
                _mapperVicinity.Add(dobst1);
            }

            _mainWindow.setMapper(_mapperVicinity, _routePlanner);
            _mainWindow.PowerScale = PowerScale;

            _soundsHelper = new SoundsHelper(_mainWindow);
        }

        void _mainWindow_Closing(object sender, CancelEventArgs e)
        {
            _mainWindow = null;
        }
        #endregion // InitializeGui - WPF

        #region DSS required operation handlers (Get, Drop)

        [ServiceHandler(ServiceHandlerBehavior.Teardown)]
        public IEnumerator<ITask> DoDrop(DsspDefaultDrop drop)
        {
            Talker.Say(9, "Dropping Drive Behavior Service Base");

            _state.Dropping = true;

            ClearHeadAnimations();

            AllLightsOff();

            // we trust the brick to shutdown gracefully. If we start issuing commands here, the drive and brick will not respond anyway,
            // as they are going down too.

            yield return Timeout(1000);

            //if (_state.IsActive)
            //{
            //    LogInfo("DriveBehaviorServiceBase service is being dropped while moving, Requesting Stop.");

            //    yield return Arbiter.Choice(
            //        StopMoving(),
            //        delegate(DefaultUpdateResponseType response) { },
            //        delegate(Fault fault) { }
            //    );

            //    yield return Arbiter.Choice(
            //        DisableMotor(),
            //        delegate(DefaultUpdateResponseType response) { },
            //        delegate(Fault fault) { }
            //    );
            //}

            LogInfo("Dropping DriveBehaviorServiceBase - calling base.DefaultDropHandler(drop)");

            base.DefaultDropHandler(drop);

            yield break;
		}

		void GetHandler(Get get)
		{
			get.ResponsePort.Post(_state);
		}

		#endregion

        #region simulatedLaser()

        protected sicklrf.State simulatedLaser()
        {
            sicklrf.State laserData = new sicklrf.State();
            laserData.Description = "simulated";
            laserData.DistanceMeasurements = new int[181];
            for (int i = 0; i < laserData.DistanceMeasurements.Length; i++)
            {
                laserData.DistanceMeasurements[i] = 3500;
            }
            laserData.Units = sicklrf.Units.Millimeters;
            laserData.AngularResolution = 1.0d;
            laserData.AngularRange = 180;
            laserData.TimeStamp = DateTime.Now;

            _state.MostRecentLaserTimeStamp = laserData.TimeStamp;

            return laserData;
        }
        #endregion // simulatedLaser()

        #region Watchdog timer handler

        void WatchDogUpdateHandler(WatchDogUpdate update)
		{
            if (_doSimulatedLaser)
            {
                if (!_testBumpMode && !_state.Dropping)
                {
                    _laserData = simulatedLaser();

                    Decide(SensorEventSource.LaserScanning);      // we disable real laser events, so we need to call Decide somehow
                }
                else
                {
                    // avoid errors due to timeout:
                    _state.MostRecentLaserTimeStamp = DateTime.Now;
                }
            }

			TimeSpan sinceLaser = update.Body.TimeStamp - _state.MostRecentLaserTimeStamp;

            //LogInfo("DriveBehaviorServiceBase: WatchDogUpdateHandler()");

			if (sinceLaser.TotalMilliseconds >= WatchdogTimeout && !_state.IsUnknown)
			{
				LogInfo("Stop requested, last laser data seen at " + _state.MostRecentLaserTimeStamp);
				StopMoving();
                _state.MovingState = MovingState.Unknown;
			}

			Activate(
			   Arbiter.Receive(
				   false,
				   TimeoutPort(WatchdogInterval),
				   delegate(DateTime ts)
				   {
					   // kick off the timer again for the next cycle
					   _mainPort.Post(new WatchDogUpdate(new WatchDogUpdateRequest(ts)));
				   }
			   )
			);

			update.ResponsePort.Post(DefaultUpdateResponseType.Instance);
		}
		#endregion

        #region Drive Completion Handlers

        /// <summary>
        /// DriveDistanceUpdateHandler - Posts a message on _completionPort - a Canceled or Complete
        /// </summary>
        /// <param name="distance"></param>
        /// <returns></returns>
        protected IEnumerator<ITask> DriveDistanceUpdateHandler(drive.DriveDistance distanceMsg)
        {
            LogInfo("DriveBehaviorServiceBase:: DriveDistanceUpdateHandler()   Distance=" + distanceMsg.Body.Distance + "    DriveDistanceStage=" + distanceMsg.Body.DriveDistanceStage);

            // This code could be collapsed, but this structure makes it
            // clear and also easy to change
            if (distanceMsg.Body.DriveDistanceStage == drive.DriveStage.Canceled)
            {
                LogInfo("DriveDistance Canceled after traveling " + distanceMsg.Body.Distance + " meters");
                _completionPort.Post(distanceMsg.Body.DriveDistanceStage);
            }
            if (distanceMsg.Body.DriveDistanceStage == drive.DriveStage.Completed)
            {
                LogInfo("DriveDistance Complete");
                _completionPort.Post(distanceMsg.Body.DriveDistanceStage);
            }
            yield break;
        }

        /// <summary>
        /// RotateDegreesUpdateHandler - Posts a message on _completionPort - a Canceled or Complete
        /// </summary>
        /// <param name="rotate"></param>
        /// <returns></returns>
        protected IEnumerator<ITask> RotateDegreesUpdateHandler(drive.RotateDegrees rotateMsg)
        {
            LogInfo("DriveBehaviorServiceBase:: RotateDegreesUpdateHandler()   Degrees=" + rotateMsg.Body.Degrees + "    RotateDegreesStage=" + rotateMsg.Body.RotateDegreesStage);

            if (rotateMsg.Body.RotateDegreesStage == drive.DriveStage.Canceled)
            {
                LogInfo("RotateDegrees Canceled after rotating " + rotateMsg.Body.Degrees + " degrees");
                _completionPort.Post(rotateMsg.Body.RotateDegreesStage);
            }
            if (rotateMsg.Body.RotateDegreesStage == drive.DriveStage.Completed)
            {
                LogInfo("RotateDegrees Complete");
                _completionPort.Post(rotateMsg.Body.RotateDegreesStage);
            }
            yield break;
        }

        /// <summary>
        /// WaitForCompletion - Helper function to wait on Completion Port
        /// </summary>
        /// <returns>Receiver suitable for waiting on</returns>
        public Receiver<drive.DriveStage> WaitForCompletion(DriveStageContainer _driveStage)
        {
            LogInfo(string.Format("++++++++++++++++++  DriveBehaviorServiceBase:: WaitForCompletion()"));
            // Note that this method does nothing with the drive status
            //return Arbiter.Receive(false, _completionPort, EmptyHandler<drive.DriveStage>);
            //return Arbiter.Receive(false, _completionPort, driveCompletionHandler);
            return Arbiter.Receive(false, _completionPort,
                    delegate(drive.DriveStage driveStage) {
                        LogInfo("++++++++++++++++++  DriveBehaviorServiceBase:: WaitForCompletion() delegate ---------------------- driveStage=" + driveStage);
                        _driveStage.DriveStage = driveStage;
                    }
                );
        }

        //public void driveCompletionHandler(drive.DriveStage driveStage)
        //{
        //    LogInfo("++++++++++++++++++  DriveBehaviorServiceBase:: driveCompletionHandler() ---------------------- driveStage=" + driveStage);
        //}

        /// <summary>
        /// WaitForCompletion - Helper function to wait on Completion Port
        /// </summary>
        /// <returns>Receiver suitable for waiting on</returns>
        //public JoinSinglePortReceiver WaitForCompletion()
        //{
        //    LogInfo(string.Format("++++++++++++++++++  DriveBehaviorServiceBase:: WaitForCompletion()"));

        //    // Note that this method does nothing with the drive status
        //    return Arbiter.MultipleItemReceive(false, _completionPort, 2, driveCompletionHandler);
        //}

        //public void driveCompletionHandler(drive.DriveStage[] driveStages)
        //{
        //    LogInfo("++++++++++++++++++  DriveBehaviorServiceBase:: driveCompletionHandler()");

        //    foreach (drive.DriveStage driveStage in driveStages)
        //    {
        //        LogInfo("++++++++++++++++++  driveStage=" + driveStage);
        //    }
        //}

        #endregion // Drive Completion Handlers

        #region Drive helper methods - Simple Drive commands

        private int lastWheelPowerLeft = int.MaxValue;
        private int lastWheelPowerRight = int.MaxValue;
        private DateTime lastSetWheelPower = DateTime.Now;
        private const double wheelPowerPrecisionMultiplier = 100.0d;   // larger value will cause better precision and more frequent drive power updates. 100 is 1% off the full -1.0...1.0 power range
        private bool useObstacleAvoidanceService = false;

        /// <summary>
        /// sends commands to _obstacleAvoidanceDrivePort, making sure that they are not sent unneccessarily often
        /// </summary>
        /// <param name="powerLeft">Left Power -1.0 ... +1.0 and will be limited to this range</param>
        /// <param name="powerRight">Right Power -1.0 ... +1.0 and will be limited to this range</param>
        private void SetDrivePower(double powerLeft, double powerRight)
        {
            double wheelPowerLeft = Math.Max(-1.0d, Math.Min(1.0d, powerLeft));
            double wheelPowerRight = Math.Max(-1.0d, Math.Min(1.0d, powerRight));

            _mapperVicinity.robotState.leftPower = wheelPowerLeft;
            _mapperVicinity.robotState.rightPower = wheelPowerRight;

            // unless there is a noticeable change in power level, or a 0.5 second timeout, do not send a command to the drive:
            int pwrL = (int)Math.Round(wheelPowerLeft * wheelPowerPrecisionMultiplier);
            int pwrR = (int)Math.Round(wheelPowerRight * wheelPowerPrecisionMultiplier);

            DateTime now = DateTime.Now;

            if (lastWheelPowerLeft != pwrL || lastWheelPowerRight != pwrR || (now - lastSetWheelPower).TotalSeconds > 0.5d)
            {
                lastWheelPowerLeft = pwrL;
                lastWheelPowerRight = pwrR;
                lastSetWheelPower = now;

                //Tracer.Trace("Behavior Base: SetDrivePower()  =========================== to OA ========================================    wheelPowerLeft=" + wheelPowerLeft + "   wheelPowerRight=" + wheelPowerRight);

                if (useObstacleAvoidanceService)
                {
                    // normally we trust the Obstacle Avoidance Service to act as a proxy for the actual Differential Drive:

                    var responsePort = _obstacleAvoidanceDrivePort.SetDrivePower(wheelPowerLeft, wheelPowerRight);

                    Activate(responsePort.Choice(
                        success =>
                        {
                        },
                        fault =>
                        {
                            LogError(fault);
                        }));
                }
                else
                {
                    // for debugging call actual Differential Drive directly:

                    var responsePort = _drivePort.SetDrivePower(wheelPowerLeft, wheelPowerRight);

                    Activate(responsePort.Choice(
                        success =>
                        {
                        },
                        fault =>
                        {
                            LogError(fault);
                        }));
                }
            }
        }

        /// <summary>
        /// Sets the forward velocity of the drive (can be 0 to stop). Does not require wait for complete.
		/// </summary>
        /// <param name="speedLeftMms">Left velocity in mm/s, 0 to 1000 and will be limited to this range</param>
        /// <param name="speedRightMms">Right velocity in mm/s, 0 to 1000 and will be limited to this range</param>
        protected void SetDriveSpeed(double speedLeftMms, double speedRightMms)
		{
            //LogInfo(string.Format("DriveBehaviorServiceBase:: SetDriveSpeed() speedLeft={0} speedRight={1} mm/sec  {2}", speedLeftMms, speedRightMms, currentCompass));

            //Talker.Say(1, (speedLeft == 0.0d && speedRight == 0.0d) ? "Stop Moving" : ("Move Forward speed " + speedLeft + " " + speedRight));

            if ((_state.DriveState == null || !_state.DriveState.IsEnabled) && speedLeftMms != 0.0d && speedRightMms != 0.0d)
			{
				EnableDrive();
			}

			// Drive.cs says "Drive speed is not implemented"
			//drive.SetDriveSpeedRequest request = new drive.SetDriveSpeedRequest();
			//request.LeftWheelSpeed = (double)speed / 1000.0; // millimeters to meters 
			//request.RightWheelSpeed = (double)speed / 1000.0; // millimeters to meters 
			//return _drivePort.SetDriveSpeed(request);

            // use power equivallent:
            SetDrivePower(speedLeftMms / 1000.0d, speedRightMms / 1000.0d);

            /*
            if(useObstacleAvoidanceService && speedLeftMms == 0.0d && speedRightMms == 0.0d)
            {
                // when stop is ordered, and we are using Obstacle Avoidance Service, also command the drive via direct ports:

                Tracer.Trace("Behavior Base: SetDriveSpeed()  --------------------------- STOP (direct to drive) ----------------------------------------");

                var responsePortDd = _drivePort.SetDrivePower(0.0d, 0.0d);

                Activate(responsePortDd.Choice(
                    success =>
                    {
                    },
                    fault =>
                    {
                        LogError(fault);
                    }));
            }
            */
        }

		/// <summary>
		/// Turns the drive relative to its current heading. Requires wait for complete.
		/// </summary>
        /// <param name="angle">angle in degrees; positive is turning right</param>
        /// <param name="power">power -1.0 ... +1.0</param>
        /// <returns>response port</returns>
		protected PortSet<DefaultUpdateResponseType, Fault> TurnByAngle(int angle, double power)
		{
            LogInfo(string.Format("DriveBehaviorServiceBase:: TurnByAngle():   angle={0} ({1}) power={2} {3}", angle, angle > 0 ? "right" : "left", power, currentCompass));

            Talker.Say(1, "Turn " + (angle > 0 ? "Right " : "Left ") + Math.Abs(angle) + " Degrees");

            if (_state.DriveState == null || !_state.DriveState.IsEnabled)
			{
				EnableDrive();
			}

            proxibrick.DirectionDataDssSerializable mostRecentDirection = _state.MostRecentDirection;

            if (mostRecentDirection != null)
            {
                proxibrick.DirectionDataDssSerializable curentDir = (proxibrick.DirectionDataDssSerializable)mostRecentDirection.Clone();

                _mapperVicinity.turnState = new TurnState()
                {
                    directionInitial = new Direction() { heading = curentDir.heading, TimeStamp = DateTime.Now.Ticks },
                    directionCurrent = new Direction() { heading = curentDir.heading, TimeStamp = DateTime.Now.Ticks },
                    directionDesired = new Direction() { heading = curentDir.heading + angle, TimeStamp = DateTime.Now.Ticks }
                };
            }

            // positive angle is turning right (i.e. left wheel goes forward):
            double leftPower = angle > 0 ? power : -power;

            _mapperVicinity.robotState.leftPower = leftPower;
            _mapperVicinity.robotState.rightPower = -leftPower;

            return _drivePort.RotateDegrees(-angle, power);
		}

        /// <summary>
        /// Turns the drive relative to its current heading. A shoot-and-forget version.
        /// </summary>
        /// <param name="angle">angle in degrees; positive is turning right</param>
        /// <param name="power">power -1.0 ... +1.0</param>
        /// <param name="nothing">ignored, used to provide different signature from the portset returning method</param>
        protected void TurnByAngle(int angle, double power, bool nothing)
        {
            var responsePortDd = TurnByAngle(angle, power);

            Activate(responsePortDd.Choice(
                success =>
                {
                },
                fault =>
                {
                    LogError(fault);
                }));
        }

		/// <summary>
        /// Moves the drive forward for the specified distance. Requires wait for complete.
		/// </summary>
		/// <param name="step">distance in mm</param>
        /// <param name="speed">speed in mm/sec, 0 to 1000</param>
        /// <returns>response port</returns>
		protected PortSet<DefaultUpdateResponseType, Fault> Translate(int stepMm, double speed)
		{
            LogInfo(string.Format("DriveBehaviorServiceBase:: Translate():   step={0} mm  speed={1} mm/s {1}", stepMm, speed, currentCompass));

            Talker.Say(1, "Translate " + stepMm);

			if (_state.DriveState == null || !_state.DriveState.IsEnabled)
			{
				EnableDrive();
			}

			double distance = (double)stepMm / 1000.0;			// millimeters to meters
			double power = Math.Min(speed / 1000.0, 1.0d);		// millimeters/sec to power setting 0...1

            _mapperVicinity.robotState.leftPower = power;
            _mapperVicinity.robotState.rightPower = power;
            
            return _drivePort.DriveDistance(distance, power);
		}

		/// <summary>
        /// Moves the drive forward for the specified distance. A shoot-and-forget version.
		/// </summary>
		/// <param name="step">distance in mm</param>
        /// <param name="speed">speed in mm/sec, 0 to 1000</param>
        /// <param name="nothing">ignored, used to provide different signature from the portset returning method</param>
        protected void Translate(int stepMm, double speed, bool nothing)
        {
            var responsePortDd = Translate(stepMm, speed);

            Activate(responsePortDd.Choice(
                success =>
                {
                },
                fault =>
                {
                    LogError(fault);
                }));
        }

		/// <summary>
		/// Sets the velocity of the drive to 0.
		/// </summary>
		protected void StopMoving()
		{
            //LogInfo("DriveBehaviorServiceBase:: StopMoving()");

			SetDriveSpeed(0.0d, 0.0d);
		}

		/// <summary>
		/// Sets the turning velocity to 0.
		/// </summary>
		protected void StopTurning()
		{
            //LogInfo("DriveBehaviorServiceBase:: StopTurning()");

            _state.IsTurning = false;
            _state.LastTurnCompleted = DateTime.Now;

            StopMoving();
		}

		/// <summary>
		/// Enables the drive
		/// </summary>
		protected void EnableDrive()
		{
            LogInfo("DriveBehaviorServiceBase:: EnableDrive()");

            var responsePortDd = _drivePort.EnableDrive(true);

            Activate(responsePortDd.Choice(
                success =>
                {
                },
                fault =>
                {
                    LogError(fault);
                }));
		}

		/// <summary>
		/// Disables the drive
		/// </summary>
		protected void DisableDrive()
		{
            LogInfo("DriveBehaviorServiceBase:: DisableDrive");

            var responsePortDd = _drivePort.EnableDrive(false);

            Activate(responsePortDd.Choice(
                success =>
                {
                },
                fault =>
                {
                    LogError(fault);
                }));
        }

        #endregion  // Drive helper methods - Simple Drive commands

        #region Combination moves and other moving iterators

        protected DateTime lastInTransitionStarted = DateTime.MinValue;

        /// <summary>
        /// performs a turn and sets the speed before exiting, may leave robot moving straight.
        /// exits immediately after the turn is completed (or interrupted).
        /// </summary>
        /// <param name="tamp">contains turn angle and desired speed</param>
        /// <param name="onComplete">handler to be called after the turn is completed and the speed is set</param>
        /// <returns></returns>
        protected IEnumerator<ITask> TurnAndMoveForward(TurnAndMoveParameters tamp, Handler onComplete)
        {
            LogInfo("DriveBehaviorServiceBase: TurnAndMoveForward() Started" + currentCompass);

            bool lastOpSuccess = true;   // must be true in case translate back is not needed

            _state.MovingState = MovingState.InTransition;
            // onComplete handler may set MovingState to whatever appropriate. We can set MovingState.Unknown on any error or interruption, and at the end to tamp.desiredMovingState
            lastInTransitionStarted = DateTime.Now;

            Talker.Say(2, "Turn " + (tamp.rotateAngle > 0 ? "Right " : "Left ") + Math.Abs(tamp.rotateAngle) + " And Move Forward");

            LogInfo(LogGroups.Console, "Turning " + (tamp.rotateAngle > 0 ? "right " : "left ") + Math.Abs(tamp.rotateAngle) + " degrees,  power=" + tamp.rotatePower + " - and move forward at speed=" + tamp.speed);

            tamp.success = false;
            Fault fault = null;
            int rotateAngle = tamp.rotateAngle;
            double? desiredHeading = Direction.to360(_mapperVicinity.robotDirection.heading + rotateAngle);

            CollisionState collisionState = _state.collisionState;

            if (Math.Abs(rotateAngle) > 45 && collisionState != null && collisionState.canMoveBackwards)
            {
                LogInfo("Steep Turn " + rotateAngle + " - backing up some...");
                // back up a bit to allow some front space for the turn:

                yield return Arbiter.Choice(
                    Translate(-backupDistanceSteepTurn, tamp.speed),
                    delegate(DefaultUpdateResponseType response) { lastOpSuccess = true; },
                    delegate(Fault f) {
                        lastOpSuccess = false;
                        _state.MovingState = MovingState.Unknown;
                        fault = f; }
                );

                // If the Translate was accepted, then wait for it to complete.
                // It is important not to wait if the request failed.
                if (lastOpSuccess)
                {
                    DriveStageContainer driveStage = new DriveStageContainer();
                    yield return WaitForCompletion(driveStage);
                    LogInfo("WaitForCompletion() returned: " + driveStage.DriveStage);

                    lastOpSuccess = driveStage.DriveStage == drive.DriveStage.Completed;

                    if (!lastOpSuccess)
                    {
                        _state.MovingState = MovingState.Unknown;
                    }
                }
                else
                {
                    _state.MovingState = MovingState.Unknown;
                    LogError("Error occurred in TurnAndMoveForward() on Backup Sharp Turn: " + fault);
                }

                _mapperVicinity.robotState.leftPower = 0.0d;
                _mapperVicinity.robotState.rightPower = 0.0d;
            }

            if (lastOpSuccess)   // either we didn't need to backup, or backup completed successfully.
            {
                if (Math.Abs(rotateAngle) > 3)
                {

                    int tryCount = 0;

                    while (lastOpSuccess && Math.Abs(rotateAngle) > 3 && ++tryCount < 3)
                    {
                        LogInfo(LogGroups.Console, "Turning " + (tryCount==1?"start":"retry") + ": current heading=" + _mapperVicinity.robotDirection.heading + "   desiredHeading=" + desiredHeading + "   rotateAngle=" + rotateAngle);

                        yield return Arbiter.Choice(
                            TurnByAngle(rotateAngle, tamp.rotatePower),
                            delegate(DefaultUpdateResponseType response)
                            {
                                LogInfo("success in TurnByAngle()" + currentCompass);
                                lastOpSuccess = true;
                            },
                            delegate(Fault f)
                            {
                                LogInfo("fault in TurnByAngle()");
                                lastOpSuccess = false;
                                _state.MovingState = MovingState.Unknown;
                                fault = f;
                            }
                        );

                        // If the RotateDegrees was accepted, then wait for it to complete.
                        // It is important not to wait if the request failed.
                        if (lastOpSuccess)
                        {
                            TrackRoamerBehaviorsState state = _state;
                            state.IsTurning = true;    // can't trust laser while turning
                            state.LastTurnStarted = state.LastTurnCompleted = DateTime.Now;     // reset watchdog

                            DriveStageContainer driveStage = new DriveStageContainer();
                            yield return WaitForCompletion(driveStage);
                            LogInfo("WaitForCompletion() returned: " + driveStage.DriveStage);

                            lastOpSuccess = driveStage.DriveStage == drive.DriveStage.Completed;

                            if (lastOpSuccess)
                            {
                                if (_mapperVicinity.turnState != null)
                                {
                                    _mapperVicinity.turnState.finished = DateTime.Now;
                                    _mapperVicinity.turnState.hasFinished = true;
                                }
                            }
                            else
                            {
                                if (_mapperVicinity.turnState != null)
                                {
                                    _mapperVicinity.turnState.finished = DateTime.Now;
                                    _mapperVicinity.turnState.hasFinished = true;
                                    _mapperVicinity.turnState.wasCanceled = true;
                                }
                                LogInfo("op failure");
                                state.MovingState = MovingState.Unknown;
                            }

                            state.IsTurning = false;
                            state.LastTurnCompleted = DateTime.Now;

                            if (tamp.speed != 0 && lastOpSuccess)
                            {
                                // a fire-and-forget command to move forward:
                                LogInfo("calling SetDriveSpeed()");
                                SetDriveSpeed(tamp.speed, tamp.speed);
                            }
                            else
                            {
                                // make sure we display zero power if SetDriveSpeed() was not called:
                                _mapperVicinity.robotState.leftPower = 0.0d;
                                _mapperVicinity.robotState.rightPower = 0.0d;
                            }

                            rotateAngle = (int)Direction.to180(desiredHeading - _mapperVicinity.robotDirection.heading);

                            LogInfo(LogGroups.Console, "Turning end: current heading=" + _mapperVicinity.robotDirection.heading + "   desiredHeading=" + desiredHeading + "   remains rotateAngle=" + rotateAngle);

                        }
                        else
                        {
                            lastOpSuccess = false;
                            _state.MovingState = MovingState.Unknown;
                            LogError("Error occurred in TurnAndMoveForward() on TurnByAngle: " + fault);
                        }
                    }
                }
                else
                {
                    // no need to turn (angle too small) - just set the speed:
                    _state.IsTurning = false;
                    if (tamp.speed != 0)
                    {
                        // a fire-and-forget command to move forward, if speed is specified:
                        LogInfo("calling SetDriveSpeed()");
                        SetDriveSpeed(tamp.speed, tamp.speed);
                    }
                }
            }

            // we come here with MovingState.InTransition or MovingState.Unknown
            tamp.success = lastOpSuccess;
            _state.MovingState = lastOpSuccess ? tamp.desiredMovingState : MovingState.Unknown;   // that's for now, onComplete may set it to whatever appropriate

            LogInfo("calling onComplete()");
            onComplete();   // check tamp.success, will be false if DriveStage.Cancel or other interruption occured.

            // done
            yield break;
        }

        // Iterator to execute the turn
        // It is important to use an Iterator so that it can relinquish control when there is nothing to do, i.e. yield return
        protected IEnumerator<ITask> TurnByDegree(TurnAndMoveParameters tamp, Handler onComplete)
        {
            LogInfo("DriveBehaviorServiceBase: TurnByDegree() Started");

            bool lastOpSuccess = true;

            _state.MovingState = MovingState.InTransition;
            // onComplete handler may set MovingState to whatever appropriate. We can set MovingState.Unknown on any error or interruption, and at the end to tamp.desiredMovingState
            lastInTransitionStarted = DateTime.Now;

            LogInfo(LogGroups.Console, "Turning " + (tamp.rotateAngle > 0 ? "right " : "left ") + Math.Abs(tamp.rotateAngle) + " degrees,  power=" + tamp.rotatePower);

            tamp.success = false;
            Fault fault = null;
            int rotateAngle = tamp.rotateAngle;
            double? desiredHeading = Direction.to360(_mapperVicinity.robotDirection.heading + rotateAngle);

            // Turn:
            yield return Arbiter.Choice(
                            TurnByAngle(rotateAngle, tamp.rotatePower),
                            delegate(DefaultUpdateResponseType response)
                            {
                                LogInfo("success in TurnByAngle()" + currentCompass);
                                lastOpSuccess = true;
                            },
                            delegate(Fault f)
                            {
                                LogInfo("fault in TurnByAngle()");
                                lastOpSuccess = false;
                                _state.MovingState = MovingState.Unknown;
                                fault = f;
                            }
            );

            // If the RotateDegrees was accepted, then wait for it to complete.
            // It is important not to wait if the request failed.
            if (lastOpSuccess)
            {
                DriveStageContainer driveStage = new DriveStageContainer();
                yield return WaitForCompletion(driveStage);
                LogInfo("WaitForCompletion() returned: " + driveStage.DriveStage);

                if (_mapperVicinity.turnState != null)
                {
                    _mapperVicinity.turnState.finished = DateTime.Now;
                    _mapperVicinity.turnState.hasFinished = true;
                }
                lastOpSuccess = driveStage.DriveStage == drive.DriveStage.Completed;
            }
            else
            {
                if (_mapperVicinity.turnState != null)
                {
                    _mapperVicinity.turnState.finished = DateTime.Now;
                    _mapperVicinity.turnState.hasFinished = true;
                    _mapperVicinity.turnState.wasCanceled = true;
                }
                LogError("Error occurred on TurnByAngle: " + fault);
                _state.MovingState = MovingState.Unknown;
            }

            // done
            _state.IsTurning = false;
            _state.LastTurnCompleted = DateTime.Now;

            _mapperVicinity.robotState.leftPower = 0.0d;
            _mapperVicinity.robotState.rightPower = 0.0d;

            yield break;
        }

        #endregion // Combination moves and other moving iterators

        #region Other helpers

        public double toDegrees(double radians)
        {
            return radians * 180.0d / Math.PI;
        }

        public double toRadians(double degrees)
        {
            return degrees * Math.PI / 180.0d;
        }

        /// <summary>
        /// for tracing we combine some compass related data into a string 
        /// </summary>
        public string currentCompass
        {
            get
            {
                try
                {
                    Direction robotDirection = _mapperVicinity.robotDirection;

                    return " - Current compass: heading= " + Math.Round((double)robotDirection.heading)
                         + " bearing=" + Math.Round((double)robotDirection.bearing)
                         + " (relative=" + Math.Round((double)robotDirection.bearingRelative)
                         + " turn=" + Math.Round((double)robotDirection.turnRelative) + ")";
                }
                catch { return " current compass: undefined"; }
            }
        }

        protected void LogHistory(int severityLevel, string msg)
        {
            StatusGraphics.HistoryDecisions.Record(new HistoryItem() { timestamp = DateTime.Now.Ticks, level = severityLevel, message = msg });
        }

        /// <summary>
        /// makes the string visible via state (including HTTP) and a line at the bottom of the mapping window.
        /// </summary>
        /// <param name="str"></param>
        protected void setMovingStateDetail(string str)
        {
            //LogInfo(str);
            _state.MovingStateDetail = str;
            if (_mainWindow != null)
            {
                _mainWindow.StatusString = str;
            }
        }

        private RobotTacticsType lastGuiTactics = RobotTacticsType.None;

        protected void setGuiCurrentTactics(RobotTacticsType tactics)
        {
            if (_mainWindow != null && tactics != lastGuiTactics)
            {
                lastGuiTactics = tactics;

                Tracer.Trace("tactics: " + tactics);

                ccrwpf.Invoke invoke = new ccrwpf.Invoke(delegate()
                {
                    _mainWindow.CurrentTactics = tactics;
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

        #endregion // Other helpers

        #region HttpGet related

        protected StatusGraphics currentStatusGraphics = null;

        protected StatusGraphics lastStatusGraphics = new StatusGraphics();

        protected object lockStatusGraphics = new object();

        const string _root = "/trackroamerbehaviors";
        const string _north = _root + "/north";
        const string _composite = _root + "/composite";
        const string _status = _root + "/status";
        const string _historySaid = _root + "/history";
        const string _historyDecisions = _root + "/decisions";

        // example: http://localhost:50000/trackroamerbehaviors/north

        [ServiceHandler(ServiceHandlerBehavior.Concurrent)]
        public void HttpGetHandler(HttpGet httpGet)
        {
            HttpListenerRequest request = httpGet.Body.Context.Request;
            HttpListenerResponse response = httpGet.Body.Context.Response;
            string path = string.Empty;

            Stream image = null;
            Stream text = null;

            if (!_state.Dropping)
            {
                path = request.Url.AbsolutePath;

                //Tracer.Trace("Path: '" + path + "'");

                if (path == _north)
                {
                    image = lastStatusGraphics.northMemory;
                }
                else if (path == _composite)
                {
                    image = lastStatusGraphics.compositeMemory;
                }
                else if (path == _status)
                {
                    image = lastStatusGraphics.statusMemory;
                }
                else if (path == _historySaid)
                {
                    text = lastStatusGraphics.historySaidMemory;
                }
                else if (path == _historyDecisions)
                {
                    text = lastStatusGraphics.historyDecisionsMemory;
                }
                else if (path == _root || path == _root + "/raw")
                {
                    HttpResponseType rsp = new HttpResponseType(HttpStatusCode.OK, _state, base.StateTransformPath);
                    //HttpResponseType rsp = new HttpResponseType(HttpStatusCode.OK, _state, getStateTransformPath());
                    httpGet.ResponsePort.Post(rsp);
                    return;
                }
            }

            if (text != null)
            {
                SendHttp(httpGet.Body.Context, text, MediaTypeNames.Text.Html);
            }
            else if (image != null)
            {
                SendHttp(httpGet.Body.Context, image, MediaTypeNames.Image.Jpeg);
            }
            else
            {
                httpGet.ResponsePort.Post(Fault.FromCodeSubcodeReason(
                    W3C.Soap.FaultCodes.Receiver,
                    DsspFaultCodes.OperationFailed,
                    "Unable to generate text or image for path '" + path + "'"));
            }
        }

        private void SendHttp(HttpListenerContext context, Stream stream, string mediaType)
        {
            //lock (lockStatusGraphics)
            {
                WriteResponseFromStream write = new WriteResponseFromStream(context, stream, mediaType);

                _httpUtilities.Post(write);

                Activate(
                    Arbiter.Choice(
                        write.ResultPort,
                        delegate(Stream res)
                        {
                            stream.Close();
                        },
                        delegate(Exception e)
                        {
                            stream.Close();
                            LogError(e);
                        }
                    )
                );
            }
        }

        #endregion // HttpGet related

#if TRACEDEBUG
        protected new void LogInfo(string str)
        {
            Tracer.Trace(str);
        }
        protected new void LogError(string str)
        {
            Tracer.Error(str);
        }
#endif // TRACEDEBUG
    }
}
