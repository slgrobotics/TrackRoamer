
//#define TRACEDEBUG
//#define TRACEDEBUGTICKS
//#define TRACELOG

using System;
using System.Collections.Generic;
using System.ComponentModel;
using Microsoft.Ccr.Core;
using Microsoft.Dss.Core.Attributes;
using Microsoft.Dss.ServiceModel.Dssp;
using Microsoft.Dss.ServiceModel.DsspServiceBase;
using W3C.Soap;

//for XSLT:
using Microsoft.Dss.Core.DsspHttpUtilities;
using Microsoft.Dss.Core.DsspHttp;
using System.Net;
using System.Collections.Specialized;

using cons = Microsoft.Dss.Services.Constructor;
using submgr = Microsoft.Dss.Services.SubscriptionManager;
using coord = Microsoft.Robotics.Services.Coordination;

using trmotor = TrackRoamer.Robotics.Services.TrackRoamerServices.Motor.Proxy;
using motor = Microsoft.Robotics.Services.Motor.Proxy;

using trencoder = TrackRoamer.Robotics.Services.TrackRoamerServices.Encoder.Proxy;
using encoder = Microsoft.Robotics.Services.Encoder.Proxy;

using drive = Microsoft.Robotics.Services.Drive.Proxy;

using TrackRoamer.Robotics.Utility.LibSystem;

namespace TrackRoamer.Robotics.Services.TrackRoamerDrive
{
    [Contract(Contract.Identifier)]
    [AlternateContract(drive.Contract.Identifier)]
    [DisplayName("(User) TrackRoamer Differential Drive")]
    [Description("Provides access to an TrackRoamer differential motor drive - coordinates two motors that function together.\n(Uses the Generic Differential Drive contract.)\n(Partner with the 'TrackRoamerPowerBrick' service.)")]
    [ActivationSettings(ShareDispatcher = false, ExecutionUnitsPerDispatcher = 6)]      // allow more threads for dispatcher (default is 2 threads, way too low)
    class TrackRoamerDriveService : DsspServiceBase
    {
        /// <summary>
        /// Service state
        /// </summary>
        [ServiceState]
        [InitialStatePartner(Optional = true)]
        private drive.DriveDifferentialTwoWheelState _state = new drive.DriveDifferentialTwoWheelState();

        drive.DriveRequestOperation _internalPendingDriveOperation = drive.DriveRequestOperation.NotSpecified;

        /// <summary>
        /// Main service port
        /// </summary>
        [ServicePort("/TrackRoamerDrive", AllowMultipleInstances = false)]
        drive.DriveOperations _mainPort = new drive.DriveOperations();

        // This is the internal drive port for excuting the drive operations that must wait for encoders ticks reaching preset value:
        //  driveDistance, and rotateDegrees.
        private PortSet<drive.DriveDistance, drive.RotateDegrees> _internalDriveOperationsPort = new PortSet<drive.DriveDistance, drive.RotateDegrees>();

        // Port used for canceling a driveDistance or RotateDegrees operation.
        //private Port<drive.CancelPendingDriveOperation> _internalDriveCancelOperationPort = new Port<drive.CancelPendingDriveOperation>();

        [SubscriptionManagerPartner]
        submgr.SubscriptionManagerPort _subMgrPort = new submgr.SubscriptionManagerPort();

        [Partner(Partners.LeftMotor,
            Contract = motor.Contract.Identifier,
            CreationPolicy = PartnerCreationPolicy.UsePartnerListEntry)]
        motor.MotorOperations _leftMotorPort = new motor.MotorOperations();

        [Partner(Partners.RightMotor,
            Contract = motor.Contract.Identifier,
            CreationPolicy = PartnerCreationPolicy.UsePartnerListEntry)]
        motor.MotorOperations _rightMotorPort = new motor.MotorOperations();

        [Partner(Partners.LeftEncoder,
            Optional = false,
            Contract = encoder.Contract.Identifier,
            CreationPolicy = PartnerCreationPolicy.UsePartnerListEntry)]
        encoder.EncoderOperations _leftEncoderPort = new encoder.EncoderOperations();
        Port<encoder.UpdateTickCount> _leftEncoderTickPort = new Port<encoder.UpdateTickCount>();   // notifications from Left Encoder
        //encoder.EncoderOperations _leftEncoderTickPort = new encoder.EncoderOperations();             // notifications - ticks and state from Left Encoder
        bool _leftEncoderTickEnabled = false;

        [Partner(Partners.RightEncoder,
            Optional = false,
            Contract = encoder.Contract.Identifier,
            CreationPolicy = PartnerCreationPolicy.UsePartnerListEntry)]
        encoder.EncoderOperations _rightEncoderPort = new encoder.EncoderOperations();
        Port<encoder.UpdateTickCount> _rightEncoderTickPort = new Port<encoder.UpdateTickCount>();   // notifications from Right Encoder
        //encoder.EncoderOperations _rightEncoderTickPort = new encoder.EncoderOperations();             // notifications - ticks and state from Right Encoder
        bool _rightEncoderTickEnabled = false;

        // we are permanently subscribed to encoder ticks, but allow or disallow them to be processed via flags:
        private bool EncoderTicksEnabled
        {
            set
            {
                _rightEncoderTickEnabled = value;
                _leftEncoderTickEnabled = value;
            }
        }

        //For XSLT
        DsspHttpUtilitiesPort _httpUtilities = new DsspHttpUtilitiesPort();
        [EmbeddedResource("TrackRoamer.Robotics.Services.TrackRoamerDrive.TrackRoamerDrive.xslt")]
        string _transform = null;

        /// <summary>
        /// Polling the encoder state
        /// </summary>
        private Port<DateTime> encodersPollingPort = new Port<DateTime>();


        /// <summary>
        /// TrackRoamerDrive Service constructor
        /// </summary>
        public TrackRoamerDriveService(DsspServiceCreationPort creationPort)
            : base(creationPort)
        {
            LogInfo("TrackRoamerDriveService:TrackRoamerDriveService() -- port: " + creationPort.ToString());
        }

        /// <summary>
        /// Service Startup Handler
        /// </summary>
        protected override void Start()
        {
            LogInfo("TrackRoamerDriveService:: Start() ");

            InitState();

            _internalPendingDriveOperation = drive.DriveRequestOperation.NotSpecified;

            // send configuration commands to partner services
            SpawnIterator(ConfigureDrive);

            _state.TimeStamp = DateTime.Now;

            //needed for HttpPost
            _httpUtilities = DsspHttpUtilitiesService.Create(Environment);

            base.Start();

            // Interleave to manage internal drive operations (driveDistance and RotateDegrees)
            Activate(
                new Interleave(
                    new ExclusiveReceiverGroup(
                                    Arbiter.ReceiveWithIterator(true, this.encodersPollingPort, this.PollEncoders),
                                    Arbiter.ReceiveWithIteratorFromPortSet<drive.DriveDistance>(true, _internalDriveOperationsPort, InternalDriveDistanceHandler),
                                    Arbiter.ReceiveWithIteratorFromPortSet<drive.RotateDegrees>(true, _internalDriveOperationsPort, InternalRotateDegreesHandler)
                    ),
                    new ConcurrentReceiverGroup())
                   );
        }

        public int EncodersPollingInterval = 1000;     // milliseconds

        /// <summary>
        /// a slow polling process to keep encoders state in sync. Not the ticks-updating mechanism, which is subscription based
        /// </summary>
        /// <param name="dt"></param>
        /// <returns></returns>
        private IEnumerator<ITask> PollEncoders(DateTime dt)
        {
            try{
                pollEncoderState();
            }
            finally
            {
                // Ensure we haven't been droppped
                if (ServicePhase == ServiceRuntimePhase.Started)
                {
                    // Issue another polling request
                    Activate(TimeoutPort(EncodersPollingInterval).Receive(this.encodersPollingPort.Post));
                }
            }

            yield break;
        }

        #region InitState()

        private void InitState()
        {
            LogInfo("TrackRoamerDriveService:: InitState() _state=" + _state);

            if (_state == null)
            {
                // no partner-supplied initial state found - create default one:

                LogInfo("TrackRoamerDriveService:: InitState()  (null) - creating one");

                //_state = new TrackRoamerDriveState();
                _state = new drive.DriveDifferentialTwoWheelState();

                _state.DistanceBetweenWheels = TrackRoamerDriveParams.DistanceBetweenWheels;

                _state.LeftWheel = new motor.WheeledMotorState();
                _state.LeftWheel.Radius = TrackRoamerDriveParams.WheelRadius;
                _state.LeftWheel.GearRatio = TrackRoamerDriveParams.WheelGearRatio;
                _state.LeftWheel.MotorState = new motor.MotorState();
                _state.LeftWheel.MotorState.HardwareIdentifier = 1;
                _state.LeftWheel.MotorState.Name = "Left Motor";
                _state.LeftWheel.MotorState.PowerScalingFactor = TrackRoamerDriveParams.MotorPowerScalingFactor;
                _state.LeftWheel.MotorState.ReversePolarity = true;

                _state.RightWheel = new motor.WheeledMotorState();
                _state.RightWheel.Radius = TrackRoamerDriveParams.WheelRadius;
                _state.RightWheel.GearRatio = TrackRoamerDriveParams.WheelGearRatio;
                _state.RightWheel.MotorState = new motor.MotorState();
                _state.RightWheel.MotorState.HardwareIdentifier = 2;
                _state.RightWheel.MotorState.Name = "Right Motor";
                _state.RightWheel.MotorState.PowerScalingFactor = TrackRoamerDriveParams.MotorPowerScalingFactor;
                _state.RightWheel.MotorState.ReversePolarity = true;

                _state.LeftWheel.EncoderState = new encoder.EncoderState();
                _state.LeftWheel.EncoderState.HardwareIdentifier = 1;
                _state.LeftWheel.EncoderState.TicksPerRevolution = TrackRoamerDriveParams.EncoderTicksPerWheelRevolution;

                _state.RightWheel.EncoderState = new encoder.EncoderState();
                _state.RightWheel.EncoderState.HardwareIdentifier = 2;
                _state.RightWheel.EncoderState.TicksPerRevolution = TrackRoamerDriveParams.EncoderTicksPerWheelRevolution;

                _state.IsEnabled = true;
                _state.TimeStamp = DateTime.Now;
            	//_state.InternalPendingDriveOperation = drive.DriveRequestOperation.NotSpecified;  -- not available in drive.Proxy

                LogInfo("TrackRoamerDriveService:: InitState(): saving state");

                SaveState(_state);
            }
            else
            {
                LogInfo("TrackRoamerDriveService:: InitState() (not null) _state.DistanceBetweenWheels=" + _state.DistanceBetweenWheels);
                //    + " PowerScalingFactor=" + _state.LeftWheel.MotorState.PowerScalingFactor + "/" + _state.RightWheel.MotorState.PowerScalingFactor);
            	//_state.InternalPendingDriveOperation = drive.DriveRequestOperation.NotSpecified;  -- not available in drive.Proxy
	            _state.TimeStamp = DateTime.Now;
            }
        }
        #endregion // InitState()

        #region ConfigureDrive() iterator

        private IEnumerator<ITask> ConfigureDrive()
        {
            LogInfo("TrackRoamerDriveService:: ConfigureDrive()");

            bool noError = true;

            // Configure motor connections
            motor.Replace configureLeftMotor = new motor.Replace();
            configureLeftMotor.Body = _state.LeftWheel.MotorState;
            _leftMotorPort.Post(configureLeftMotor);

            motor.Replace configureRightMotor = new motor.Replace();
            configureRightMotor.Body = _state.RightWheel.MotorState;
            _rightMotorPort.Post(configureRightMotor);

            yield return Arbiter.Choice(configureLeftMotor.ResponsePort,
                delegate(DefaultReplaceResponseType success) { LogInfo("    Left Motor Configured"); },
                delegate(Fault fault) { LogError(fault); noError = false; });

            yield return Arbiter.Choice(configureRightMotor.ResponsePort,
                delegate(DefaultReplaceResponseType success) { LogInfo("    Right Motor Configured"); },
                delegate(Fault fault) { LogError(fault); noError = false; });

            // Configure encoder connections, and permanently subscribe to the encoders on internal ports.
            if (_leftEncoderPort != null)
            {
                encoder.Replace configureLeftEncoder = new encoder.Replace();
                configureLeftEncoder.Body = _state.LeftWheel.EncoderState;
                _leftEncoderPort.Post(configureLeftEncoder);

                yield return Arbiter.Choice(configureLeftEncoder.ResponsePort,
                    delegate(DefaultReplaceResponseType success) { LogInfo("    Left Encoder Configured"); },
                    delegate(Fault fault) { LogError(fault); noError = false; });

                encoder.Subscribe op = new encoder.Subscribe();
                op.Body = new SubscribeRequestType();
                op.NotificationPort = _leftEncoderTickPort;
                _leftEncoderPort.Post(op);

                yield return (Arbiter.Choice(op.ResponsePort,
                    delegate(SubscribeResponseType response)
                    {
                        //subscription was successful, start listening for encoder replace messages
                        Activate(Arbiter.Receive<encoder.UpdateTickCount>(true, _leftEncoderTickPort,       // "true" here makes listener to subscription permanent
                            delegate(encoder.UpdateTickCount update)
                            {
#if TRACEDEBUGTICKS
                                LogInfo("Drive: left encoder tick: " + update.Body.Count);
#endif // TRACEDEBUGTICKS
                                StopMotorWithEncoderHandler(_leftEncoderTickPort, "left", update, _leftMotorPort);
                            }));
                    },
                    delegate(Fault fault) { LogError(fault); noError = false; }
                ));
            }

            if (_rightEncoderPort != null)
            {
                encoder.Replace configureRightEncoder = new encoder.Replace();
                configureRightEncoder.Body = _state.RightWheel.EncoderState;
                _rightEncoderPort.Post(configureRightEncoder);

                yield return Arbiter.Choice(configureRightEncoder.ResponsePort,
                    delegate(DefaultReplaceResponseType success) { LogInfo("    Right Encoder Configured"); },
                    delegate(Fault fault) { LogError(fault); noError = false; });

                encoder.Subscribe op2 = new encoder.Subscribe();
                op2.Body = new SubscribeRequestType();
                op2.NotificationPort = _rightEncoderTickPort;
                _rightEncoderPort.Post(op2);

                yield return (Arbiter.Choice(op2.ResponsePort,
                    delegate(SubscribeResponseType response)
                    {
                        //subscription was successful, start listening for encoder replace messages
                        Activate(Arbiter.Receive<encoder.UpdateTickCount>(true, _rightEncoderTickPort,       // "true" here makes listener to subscription permanent
                            delegate(encoder.UpdateTickCount update)
                            {
#if TRACEDEBUGTICKS
                                LogInfo("Drive: right encoder tick: " + update.Body.Count);
#endif // TRACEDEBUGTICKS
                                StopMotorWithEncoderHandler(_rightEncoderTickPort, "right", update, _rightMotorPort);
                            }
                        ));
                    },
                    delegate(Fault fault) { LogError(fault); noError = false; }
                ));
            }

            if (noError)
            {
                LogInfo("TrackRoamerDriveService:: ConfigureDrive() - success");
                _state.IsEnabled = true;

                // Start the encoder polling interval
                this.encodersPollingPort.Post(DateTime.Now);
            }
            else
            {
                LogError("TrackRoamerDriveService:: ConfigureDrive() - failure");
                _state.IsEnabled = false;
            }

            yield break;
        }
        #endregion // ConfigureDrive()

        #region Operation Handlers - implementing Microsoft DriveDifferentialTwoWheel

        /// <summary>
        /// Get Handler
        /// </summary>
        /// <param name="get"></param>
        /// <returns></returns>
        [ServiceHandler(ServiceHandlerBehavior.Concurrent)]
        public IEnumerator<ITask> GetHandler(drive.Get get)
        {
            get.ResponsePort.Post(_state);
            yield break;
        }

        /// <summary>
        /// Update Handler
        /// </summary>
        /// <param name="update"></param>
        /// <returns></returns>
        [ServiceHandler(ServiceHandlerBehavior.Exclusive)]
        public IEnumerator<ITask> UpdateHandler(drive.Update update)
        {
            _state = update.Body;
            _state.TimeStamp = DateTime.Now;
            update.ResponsePort.Post(new DefaultUpdateResponseType());
            yield break;
        }

        /// <summary>
        /// Subscribe Handler
        /// </summary>
        /// <param name="subscribe"></param>
        /// <returns></returns>
        [ServiceHandler(ServiceHandlerBehavior.Exclusive)]
        public IEnumerator<ITask> SubscribeHandler(drive.Subscribe subscribe)
        {
            yield return Arbiter.Choice(
                SubscribeHelper(_subMgrPort, subscribe.Body, subscribe.ResponsePort),
                delegate(SuccessResult success)
                {
                    drive.Update update = new drive.Update(_state);
                    SendNotificationToTarget<drive.Update>(subscribe.Body.Subscriber, _subMgrPort, update);
                },
                delegate(Exception ex)
                {
                    LogError(ex);
                    throw ex;
                }
            );
        }

        /// <summary>
        /// Reliable Subscribe Handler
        /// </summary>
        /// <param name="subscribe"></param>
        /// <returns></returns>
        [ServiceHandler(ServiceHandlerBehavior.Exclusive)]
        public IEnumerator<ITask> ReliableSubscribeHandler(drive.ReliableSubscribe subscribe)
        {
            yield return Arbiter.Choice(
                SubscribeHelper(_subMgrPort, subscribe.Body, subscribe.ResponsePort),
                delegate(SuccessResult success)
                {
                    drive.Update update = new drive.Update(_state);
                    SendNotificationToTarget<drive.Update>(subscribe.Body.Subscriber, _subMgrPort, update);
                },
                delegate(Exception ex)
                {
                    LogError(ex);
                    throw ex;
                }
            );
        }

        [ServiceHandler(ServiceHandlerBehavior.Teardown)]
        public virtual IEnumerator<ITask> DropHandler(DsspDefaultDrop drop)
        {
            LogInfo("TrackRoamerDriveService:DropHandler()");

            base.DefaultDropHandler(drop);

            yield break;
        }

        /// <summary>
        /// Handles EnableDrive requests
        /// </summary>
        /// <param name="enabledrive">request message</param>
        /// <returns></returns>
        [ServiceHandler(ServiceHandlerBehavior.Exclusive)]
        public IEnumerator<ITask> EnableDriveHandler(drive.EnableDrive enableDrive)
        {

            _state.IsEnabled = enableDrive.Body.Enable;
            _state.TimeStamp = DateTime.Now;

#if TRACELOG
            LogInfo("TrackRoamerDriveService:EnableDriveHandler() - enable=" + _state.IsEnabled);
#endif

            // if we are enabling the drive, validate that the motors are configured.
            if (enableDrive.Body.Enable)
            {
                try
                {
                    ValidateDriveConfiguration(true);   // we must have encoders for the drive to work properly
                }
                catch (InvalidOperationException)
                {
                    // If validation fails,
                    // force the state to not be enabled.
                    _state.IsEnabled = false;

                    // rethrow the fault
                    throw;
                }
            }

            // send notification to subscription manager
            drive.Update update = new drive.Update(_state);
            SendNotification<drive.Update>(_subMgrPort, update);

            enableDrive.ResponsePort.Post(DefaultUpdateResponseType.Instance);
            yield break;
        }

        #region Drive Power Handler

        /// <summary>
        /// Handles SetDrivePower requests
        /// </summary>
        /// <param name="setdrivepower">request message</param>
        /// <returns></returns>
        [ServiceHandler(ServiceHandlerBehavior.Exclusive)]
        public IEnumerator<ITask> SetDrivePowerHandler(drive.SetDrivePower setDrivePower)
        {
            ValidateDriveConfiguration(false);

#if TRACELOG
            Tracer.Trace("TR Drive: SetDrivePowerHandler()  =============================== TR handler ====================================     left=" + setDrivePower.Body.LeftWheelPower + "   right=" + setDrivePower.Body.RightWheelPower);
#endif
            cancelCurrentOperation();

            _state.TimeStamp = DateTime.Now;

            PortSet<DefaultUpdateResponseType, Fault> responsePort = new PortSet<DefaultUpdateResponseType, Fault>();

            // Add a coordination header to our motor requests
            // so that advanced motor implementations can
            // coordinate the individual motor reqests.
            coord.ActuatorCoordination coordination = new coord.ActuatorCoordination(2);

            motor.SetMotorPower leftPower = new motor.SetMotorPower(new motor.SetMotorPowerRequest() { TargetPower = setDrivePower.Body.LeftWheelPower });
            leftPower.ResponsePort = responsePort;
            leftPower.AddHeader(coordination);
            _leftMotorPort.Post(leftPower);

            motor.SetMotorPower rightPower = new motor.SetMotorPower(new motor.SetMotorPowerRequest() { TargetPower = setDrivePower.Body.RightWheelPower });
            rightPower.ResponsePort = responsePort;
            rightPower.AddHeader(coordination);
            _rightMotorPort.Post(rightPower);

            // send notification to subscription manager
            drive.Update update = new drive.Update(_state);
            SendNotification<drive.Update>(_subMgrPort, update);

            Activate(Arbiter.MultipleItemReceive<DefaultUpdateResponseType, Fault>(responsePort, 2,
                delegate(ICollection<DefaultUpdateResponseType> successList, ICollection<Fault> failureList)
                {
                    if (successList.Count == 2)
                        setDrivePower.ResponsePort.Post(new DefaultUpdateResponseType());

                    foreach (Fault fault in failureList)
                    {
                        setDrivePower.ResponsePort.Post(fault);
                        break;
                    }
                }));

            pollEncoderState();

            yield break;
        }

        #endregion //  Drive Power Handler

        #region Drive Speed Handler

        /// <summary>
        /// Handles SetDriveSpeed requests
        /// </summary>
        /// <param name="setDriveSpeed">request message</param>
        /// <returns></returns>
        [ServiceHandler(ServiceHandlerBehavior.Exclusive)]
        public IEnumerator<ITask> SetDriveSpeedHandler(drive.SetDriveSpeed setDriveSpeed)
        {
            ValidateDriveConfiguration(true);

            cancelCurrentOperation();

            _state.TimeStamp = DateTime.Now;

            LogError("Drive speed is not implemented");

            throw new NotImplementedException();
        }

        #endregion // Drive Speed Handler

        #region Rotate Degrees Handler

        /// <summary>
        /// Handles RotateDegrees requests (positive degrees turn counterclockwise)
        /// </summary>
        /// <param name="rotatedegrees">request message</param>
        /// <returns></returns>
        [ServiceHandler(ServiceHandlerBehavior.Exclusive)]
        public IEnumerator<ITask> RotateDegreesHandler(drive.RotateDegrees rotateDegrees)
        {
            ValidateDriveConfiguration(true);
            if (_state.DistanceBetweenWheels <= 0)
            {
                rotateDegrees.ResponsePort.Post(new Fault());
                throw new InvalidOperationException("The wheel encoders are not properly configured");
            }
            else
            {
#if TRACELOG
                Tracer.Trace("^^^^^^^^^^^^^^^^^^^^^ TrackRoamerDriveService:: RotateDegreesHandler(degrees=" + rotateDegrees.Body.Degrees + ")");
#endif
                _state.TimeStamp = DateTime.Now;

                // send immediate response
                rotateDegrees.ResponsePort.Post(DefaultUpdateResponseType.Instance);

                // post request to internal port.
                _internalDriveOperationsPort.Post(rotateDegrees);

            }
            yield break;
        }

        int stopLeftWheelAt;
        int stopRightWheelAt;

        /// <summary>
        /// Rotate the the drive (positive degrees turn counterclockwise)
        /// </summary>
        /// <param name="degrees">(positive degrees turn counterclockwise)</param>
        /// <param name="power">(-1.0 to 1.0)</param>
        IEnumerator<ITask> RotateUntilDegrees(double degrees, double power)
        {
#if TRACELOG
            Tracer.Trace("^^^^^^^^^^^^^^^^^^^^^ TrackRoamerDriveService:: RotateUntilDegrees(degrees=" + degrees + ",  power=" + power + ")");
#endif

            EncoderTicksEnabled = false;

            //reset encoders
            encoder.Reset Lreset = new encoder.Reset();
            _leftEncoderPort.Post(Lreset);
            yield return (Arbiter.Choice(Lreset.ResponsePort,
                delegate(DefaultUpdateResponseType response) { },
                delegate(Fault fault) { LogError(fault); }
            ));

            encoder.Reset Rreset = new encoder.Reset();
            _rightEncoderPort.Post(Rreset);
            yield return (Arbiter.Choice(Rreset.ResponsePort,
                delegate(DefaultUpdateResponseType response) { },
                delegate(Fault fault) { LogError(fault); }
            ));

            double arcDistance = Math.Abs(degrees) * _state.DistanceBetweenWheels * Math.PI / 360.0d;

            //compute tick to stop at
            stopLeftWheelAt = (int)Math.Round(arcDistance / (2.0d * Math.PI * _state.LeftWheel.Radius / _state.LeftWheel.EncoderState.TicksPerRevolution));
            stopRightWheelAt = (int)Math.Round(arcDistance / (2.0d * Math.PI * _state.RightWheel.Radius / _state.RightWheel.EncoderState.TicksPerRevolution));

            EncoderTicksEnabled = true;

            pollEncoderState();     // get starting encoder state

            //start moving

            // start rotate operation
            _state.RotateDegreesStage = drive.DriveStage.Started;

            drive.RotateDegrees rotateUpdate = new drive.RotateDegrees();
            rotateUpdate.Body.RotateDegreesStage = drive.DriveStage.Started;
#if TRACELOG
            Tracer.Trace("++++++++++++++++++ DRIVE: RotateUntilDegrees() DriveStage.Started");
#endif
            _internalDriveOperationsPort.Post(rotateUpdate);

            PortSet<DefaultUpdateResponseType, Fault> responsePort = new PortSet<DefaultUpdateResponseType, Fault>();

            double rightPow;
            double leftPow;

            if (degrees > 0)
            {
                rightPow = power;
                leftPow = -power;
            }
            else
            {
                rightPow = -power;
                leftPow = power;
            }

            motor.SetMotorPower leftPower = new motor.SetMotorPower(new motor.SetMotorPowerRequest() { TargetPower = leftPow });
            leftPower.ResponsePort = responsePort;
            _leftMotorPort.Post(leftPower);

            motor.SetMotorPower rightPower = new motor.SetMotorPower(new motor.SetMotorPowerRequest() { TargetPower = rightPow });
            rightPower.ResponsePort = responsePort;
            _rightMotorPort.Post(rightPower);

#if TRACELOG
            Tracer.Trace("=============== TrackRoamerDriveService:: RotateUntilDegrees() start moving: degrees=" + degrees);
            Tracer.Trace("=============== TrackRoamerDriveService:: RotateUntilDegrees() will stop wheels at:  Left=" + stopLeftWheelAt + " Right=" + stopRightWheelAt);
#endif

            Activate(Arbiter.MultipleItemReceive<DefaultUpdateResponseType, Fault>(responsePort, 2,
                delegate(ICollection<DefaultUpdateResponseType> successList, ICollection<Fault> failureList)
                {
                    foreach (Fault fault in failureList)
                    {
                        LogError(fault);
                    }
                }
            ));

#if TRACELOG
            Tracer.Trace("=============== TrackRoamerDriveService:: RotateUntilDegrees() calling DriveWaitForCompletionDual() - waiting for both sides to complete...");
#endif

            yield return DriveWaitForCompletionDual();

#if TRACELOG
            Tracer.Trace("=============== TrackRoamerDriveService:: RotateUntilDegrees() - both sides completed, send notification of RotateDegrees complete to subscription manager");
#endif

            // send notification of RotateDegrees complete to subscription manager
            rotateUpdate.Body.RotateDegreesStage = drive.DriveStage.Completed;
#if TRACELOG
            Tracer.Trace("++++++++++++++++++ DRIVE: RotateUntilDegrees() DriveStage.Completed");
#endif
            _internalDriveOperationsPort.Post(rotateUpdate);

            _state.RotateDegreesStage = drive.DriveStage.Completed;
        }

        #endregion // Rotate Degrees Handler

        #region Drive Distance Handler

        /// <summary>
        /// Handles DriveDistance requests
        /// </summary>
        /// <param name="drivedistance">request message</param>
        /// <returns></returns>
        [ServiceHandler(ServiceHandlerBehavior.Exclusive)]
        public IEnumerator<ITask> DriveDistanceHandler(drive.DriveDistance driveDistance)
        {
            // If configuration is invalid, an InvalidException is thrown.
            ValidateDriveConfiguration(true);
            _state.TimeStamp = DateTime.Now;

#if TRACELOG
            Tracer.Trace("==================== TrackRoamerDriveService:: DriveDistanceHandler(distance=" + driveDistance.Body.Distance + ")");
#endif

            // send immediate response
            driveDistance.ResponsePort.Post(DefaultUpdateResponseType.Instance);

            // post request to internal port.
            _internalDriveOperationsPort.Post(driveDistance);

            yield break;
        }

        /// <summary>
        /// drives a specified number of meters
        /// </summary>
        IEnumerator<ITask> DriveUntilDistance(double distance, double power)
        {
#if TRACELOG
            Tracer.Trace("=============== TrackRoamerDriveService:: DriveUntilDistance(distance=" + distance + " meters,  power=" + power + ")");
#endif

            EncoderTicksEnabled = false;

            //reset encoders
            encoder.Reset Lreset = new encoder.Reset();
            _leftEncoderPort.Post(Lreset);
            yield return (Arbiter.Choice(Lreset.ResponsePort,
                delegate(DefaultUpdateResponseType response) { },
                delegate(Fault fault) { LogError(fault); }
            ));

            encoder.Reset Rreset = new encoder.Reset();
            _rightEncoderPort.Post(Rreset);
            yield return (Arbiter.Choice(Rreset.ResponsePort,
                delegate(DefaultUpdateResponseType response) { },
                delegate(Fault fault) { LogError(fault); }
            ));

            //compute tick to stop at
            stopLeftWheelAt = (int)Math.Round(Math.Abs(distance) / (2.0 * 3.14159 * _state.LeftWheel.Radius / _state.LeftWheel.EncoderState.TicksPerRevolution));
            stopRightWheelAt = (int)Math.Round(Math.Abs(distance) / (2.0 * 3.14159 * _state.RightWheel.Radius / _state.RightWheel.EncoderState.TicksPerRevolution));

            EncoderTicksEnabled = true;

            pollEncoderState();     // get starting encoder state

            //start moving

            double Pow;

            if (distance > 0)
                Pow = power;
            else
                Pow = -power;

            PortSet<DefaultUpdateResponseType, Fault> responsePort = new PortSet<DefaultUpdateResponseType, Fault>();

            // send notification of driveDistance start to subscription manager
            _state.DriveDistanceStage = drive.DriveStage.Started;

            drive.DriveDistance driveUpdate = new drive.DriveDistance();
            driveUpdate.Body.DriveDistanceStage = drive.DriveStage.Started;
#if TRACELOG
            Tracer.Trace("++++++++++++++++++ DRIVE: DriveUntilDistance() DriveStage.Started");
#endif
            _internalDriveOperationsPort.Post(driveUpdate);

            motor.SetMotorPower leftPower = new motor.SetMotorPower(new motor.SetMotorPowerRequest() { TargetPower = Pow });
            leftPower.ResponsePort = responsePort;
            _leftMotorPort.Post(leftPower);

            motor.SetMotorPower rightPower = new motor.SetMotorPower(new motor.SetMotorPowerRequest() { TargetPower = Pow });
            rightPower.ResponsePort = responsePort;
            _rightMotorPort.Post(rightPower);

#if TRACELOG
            Tracer.Trace("=============== TrackRoamerDriveService:: DriveUntilDistance() start moving: distance=" + distance + " meters");
            Tracer.Trace("=============== TrackRoamerDriveService:: DriveUntilDistance() will stop wheels at:  Left=" + stopLeftWheelAt + "   Right=" + stopRightWheelAt);
#endif

            Activate(Arbiter.MultipleItemReceive<DefaultUpdateResponseType, Fault>(responsePort, 2,
                delegate(ICollection<DefaultUpdateResponseType> successList, ICollection<Fault> failureList)
                {
                    foreach (Fault fault in failureList)
                    {
                        LogError(fault);
                    }
                }
            ));

#if TRACELOG
            Tracer.Trace("=============== TrackRoamerDriveService:: DriveUntilDistance() calling DriveWaitForCompletionDual() - waiting for both sides to complete...");
#endif

            yield return DriveWaitForCompletionDual();

            // send notification of driveDistance complete to subscription manager
            driveUpdate.Body.DriveDistanceStage = drive.DriveStage.Completed;
#if TRACELOG
            Tracer.Trace("++++++++++++++++++ DRIVE: DriveUntilDistance() DriveStage.Completed");
#endif
            _internalDriveOperationsPort.Post(driveUpdate);

            _state.DriveDistanceStage = drive.DriveStage.Completed;
        }

        #endregion // Drive Distance Handler

        #region AllStop Handler

        /// <summary>
        /// Handles AllStop requests
        /// </summary>
        /// <param name="allstop">request message</param>
        /// <returns></returns>
        [ServiceHandler(ServiceHandlerBehavior.Exclusive)]
        public IEnumerator<ITask> AllStopHandler(drive.AllStop allStop)
        {
            /*
                from http://social.msdn.microsoft.com/Forums/en-US/roboticssimulation/thread/13fb26c8-75fe-43b9-9e65-6a915e7d2560 :
                In RDS 2008 there is some discussion of the Generic Differential Drive in the Help file.
                AllStop should cause the current operation to be cancelled, as you expect.
                It will then disable the drive and you will have to issue an EnableDrive request before you can get the robot to move again.
                This is by design so that AllStop can act as an Emergency Stop and prevent any further action without an explicit reset.
                There should be a notification from the drive service that the drive power has been set to zero after the AllStop.
             */

#if TRACELOG
            LogInfo("TrackRoamerDriveService:: AllStopHandler()");
#endif

            cancelCurrentOperation();

            drive.SetDrivePower zeroPower = new drive.SetDrivePower();
            zeroPower.Body = new drive.SetDrivePowerRequest(0.0d, 0.0d);
            zeroPower.ResponsePort = allStop.ResponsePort;
            _mainPort.Post(zeroPower);

            pollEncoderState();     // get starting encoder state

            yield break;
        }

        #endregion // AllStop Handler

        /// <summary>
        /// ResetEncoders handler
        /// </summary>
        /// <param name="resetEncoders">ResetEncoders message</param>
        /// <returns>A CCR task enumerator</returns>
        [ServiceHandler(ServiceHandlerBehavior.Exclusive)]
        public IEnumerator<ITask> ResetEncodersHandler(drive.ResetEncoders resetEncoders)
        {
            encoder.Reset rightResetRequest = null;
            encoder.Reset leftResetRequest = null;

            if (this._rightEncoderPort != null)
            {
                rightResetRequest = new encoder.Reset();

                this._rightEncoderPort.Post(rightResetRequest);
            }

            if (this._leftEncoderPort != null)
            {
                leftResetRequest = new encoder.Reset();

                this._leftEncoderPort.Post(leftResetRequest);
            }

            if (rightResetRequest != null)
            {
                yield return rightResetRequest.ResponsePort.Choice(EmptyHandler, EmptyHandler);
            }

            if (leftResetRequest != null)
            {
                yield return leftResetRequest.ResponsePort.Choice(EmptyHandler, EmptyHandler);
            }

            this.SendNotification<drive.ResetEncoders>(this._subMgrPort, resetEncoders.Body);

            resetEncoders.ResponsePort.Post(new DefaultUpdateResponseType());

            yield break;
        }

        #endregion // Operation Handlers (implementing MS Generic Drive)

        #region HTTP Get / Post
        /// <summary>
        /// Http Get Handler.  Needed for XSLT transform
        /// </summary>
        [ServiceHandler(ServiceHandlerBehavior.Concurrent)]
        public virtual IEnumerator<ITask> HttpGetHandler(HttpGet httpGet)
        {
            HttpListenerRequest request = httpGet.Body.Context.Request;
            HttpListenerResponse response = httpGet.Body.Context.Response;

            string path = request.Url.AbsolutePath;

            HttpResponseType rsp = new HttpResponseType(HttpStatusCode.OK, _state, _transform);
            httpGet.ResponsePort.Post(rsp);
            yield break;

        }

        /// <summary>
        /// Http Post Handler.  Handles http form inputs
        /// </summary>
        [ServiceHandler(ServiceHandlerBehavior.Exclusive)]
        public void HttpPostHandler(HttpPost httpPost)
        {
            HttpPostRequestData formData = httpPost.GetHeader<HttpPostRequestData>();

            try
            {
                DsspOperation operation = formData.TranslatedOperation;

                if (operation is drive.DriveDistance)
                {
                    _mainPort.Post((drive.DriveDistance)operation);
                }
                else if (operation is drive.SetDrivePower)
                {
                    _mainPort.Post((drive.SetDrivePower)operation);
                }
                else if (operation is drive.RotateDegrees)
                {
                    _mainPort.Post((drive.RotateDegrees)operation);
                }
                else if (operation is drive.EnableDrive)
                {
                    _mainPort.Post((drive.EnableDrive)operation);
                }
                else if (operation is drive.AllStop)
                {
                    _mainPort.Post((drive.AllStop)operation);
                }
                else if (operation is drive.ResetEncoders)
                {
                    _mainPort.Post((drive.ResetEncoders)operation);
                }
                else
                {
                    NameValueCollection parameters = formData.Parameters;

                    if (parameters["StartDashboard"] != null)
                    {
                        string Dashboardcontract = "http://schemas.microsoft.com/robotics/2006/01/simpledashboard.user.html";
                        ServiceInfoType info = new ServiceInfoType(Dashboardcontract);
                        cons.Create create = new cons.Create(info);
                        create.TimeSpan = DsspOperation.DefaultShortTimeSpan;

                        ConstructorPort.Post(create);
                        Activate(Arbiter.Choice(
                            create.ResponsePort,
                            delegate(CreateResponse createResponse) { },
                            delegate(Fault f) { LogError(f); }
                        ));
                    }
                    else if (parameters["DrivePower"] != null)
                    {
                        double power = double.Parse(parameters["Power"]);
                        drive.SetDrivePowerRequest drivepower = new drive.SetDrivePowerRequest();
                        drivepower.LeftWheelPower = power;
                        drivepower.RightWheelPower = power;
                        _mainPort.Post(new drive.SetDrivePower(drivepower));
                    }
                    else
                    {
                        throw new InvalidOperationException();
                    }
                }
                HttpPostSuccess(httpPost);
            }
            catch
            {
                HttpPostFailure(httpPost);
            }
        }

        /// <summary>
        /// Send Http Post Success Response
        /// </summary>
        private void HttpPostSuccess(HttpPost httpPost)
        {
            HttpResponseType rsp =
                new HttpResponseType(HttpStatusCode.OK, _state, _transform);
            httpPost.ResponsePort.Post(rsp);
        }

        /// <summary>
        /// Send Http Post Failure Response
        /// </summary>
        private void HttpPostFailure(HttpPost httpPost)
        {
            HttpResponseType rsp =
                new HttpResponseType(HttpStatusCode.BadRequest, _state, _transform);
            httpPost.ResponsePort.Post(rsp);
        }

        #endregion // HTTP Get / Post

        #region ValidateDriveConfiguration() helper

        /// <summary>
        /// Validate that the motors are configured and the drive is enabled.
        /// <remarks>Throws an exception (converted to fault)
        /// when the service is not properly configured.</remarks>
        /// </summary>
        /// <param name="requireEncoders">validate encoder configuration</param>
        private void ValidateDriveConfiguration(bool requireEncoders)
        {
            if (_leftMotorPort == null || _rightMotorPort == null)
            {
                LogError("The motors are not connected.");
                throw new InvalidOperationException();
            }

            if (!_state.IsEnabled)
            {
                LogError("The differential drive is not enabled.");
                throw new InvalidOperationException();
            }

            if (requireEncoders)
            {
                if (_state.LeftWheel == null
                    || _state.LeftWheel.Radius == 0.0
                    || _state.LeftWheel.EncoderState == null
                    || _state.LeftWheel.EncoderState.TicksPerRevolution == 0
                    || _state.RightWheel == null
                    || _state.RightWheel.Radius == 0.0
                    || _state.RightWheel.EncoderState == null
                    || _state.RightWheel.EncoderState.TicksPerRevolution == 0
                    )
                {
                    LogError("The wheel encoders are not properly configured.");
                    throw new InvalidOperationException();
                }

            }
        }

        #endregion // ValidateDriveConfiguration() helper

        #region Internal Drive Operation Handlers

        // This port is sent a message every time that there is a
        // Canceled or Complete message from the Drive, so it can
        // be used to wait for completion.
        private Port<drive.DriveStage> completionPort = new Port<drive.DriveStage>();

        /// <summary>
        /// DriveWaitForCompletion - Helper function to wait on Completion Port
        /// </summary>
        /// <returns>Receiver suitable for waiting on</returns>
        private Receiver<drive.DriveStage> DriveWaitForCompletion()
        {
            // Note that this method does nothing with the drive status
            return Arbiter.Receive(false, completionPort, EmptyHandler<drive.DriveStage>);
        }

        /// <summary>
        /// DriveWaitForCompletionDual - Helper function to wait on Completion Port for two events
        /// </summary>
        /// <returns>Receiver suitable for waiting on</returns>
        public JoinSinglePortReceiver DriveWaitForCompletionDual()
        {
#if TRACELOG
            //Tracer.Trace(string.Format("++++++++++++++++++  TrackRoamerDrive:: DriveWaitForCompletionDual()"));
#endif

            // Note that this method does nothing with the drive status
            return Arbiter.MultipleItemReceive(false, completionPort, 2, driveCompletionHandler);
        }

        public void driveCompletionHandler(drive.DriveStage[] driveStages)
        {
#if TRACELOG
            //Tracer.Trace("++++++++++++++++++  TrackRoamerDrive:: driveCompletionHandler()");

            //foreach (drive.DriveStage driveStage in driveStages)
            //{
            //    Tracer.Trace("++++++++++++++++++  driveStage=" + driveStage);
            //}
#endif
            pollEncoderState();
        }

        /// <summary>
        /// get both encoders' state and save it in drive state
        /// </summary>
        private void pollEncoderState()
        {
            encoder.Get get = new encoder.Get();

            _leftEncoderPort.Post(get);

            Activate(Arbiter.Receive<encoder.EncoderState>(false, get.ResponsePort,
                delegate(encoder.EncoderState response)
                {
                    _state.LeftWheel.EncoderState = response;
                }
            ));

            get = new encoder.Get();

            _rightEncoderPort.Post(get);

            Activate(Arbiter.Receive<encoder.EncoderState>(false, get.ResponsePort,
                delegate(encoder.EncoderState response)
                {
                    _state.RightWheel.EncoderState = response;
                }
            ));
        }

        void StopMotorWithEncoderHandler(Port<encoder.UpdateTickCount> encoderNotificationPort, string side, encoder.UpdateTickCount update, motor.MotorOperations motorPort)
        {
            int stopWheelAt;
            bool ignore;

            switch (side)
            {
                case "left":
                    stopWheelAt = stopLeftWheelAt;
                    ignore = !_leftEncoderTickEnabled;
                    break;

                default:
                case "right":
                    stopWheelAt = stopRightWheelAt;
                    ignore = !_rightEncoderTickEnabled;
                    break;
            }

            if (!ignore)
            {
#if TRACEDEBUGTICKS
                Tracer.Trace("^^^^^^^^^^^^^^^^^^^^^ TrackRoamerDriveService:: StopMotorWithEncoderHandler() " + side + " encoder at=" + update.Body.Count + "    will stop wheel at=" + stopWheelAt);
#endif // TRACEDEBUGTICKS

                if (update.Body.Count >= stopWheelAt)
                {
                    switch (side)
                    {
                        case "left":
                            _leftEncoderTickEnabled = false;
                            break;

                        default:
                        case "right":
                            _rightEncoderTickEnabled = false;
                            break;
                    }
                    // whatever else got stuck there, we are not interested. Keep the port clear.
                    //Port<encoder.UpdateTickCount> port = (Port<encoder.UpdateTickCount>)encoderNotificationPort[typeof(encoder.UpdateTickCount)];
                    //port.Clear();

                    motor.SetMotorPower stop = new motor.SetMotorPower(new motor.SetMotorPowerRequest() { TargetPower = 0 });
                    motorPort.Post(stop);
                    Activate(Arbiter.Choice(stop.ResponsePort,
                        delegate(DefaultUpdateResponseType resp)
                        {
#if TRACEDEBUGTICKS
                            Tracer.Trace("^^^^^^^^^^^^^^^^^^^^^ TrackRoamerDriveService:: StopMotorWithEncoderHandler() " + side + " - motor stopped by encoder !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!");
#endif // TRACEDEBUGTICKS
                        },
                        delegate(Fault fault) { LogError(fault); }
                    ));

#if TRACEDEBUGTICKS
                    Tracer.Trace("^^^^^^^^^^^^^^^^^^^^^ TrackRoamerDriveService:: StopMotorWithEncoderHandler() " + side + " - Sending to completionPort: DriveStage.Completed !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!");
#endif // TRACEDEBUGTICKS
                    completionPort.Post(drive.DriveStage.Completed);
                }
            }
        }

        private drive.DriveDistance pendingDriveDistance = null;
        private drive.RotateDegrees pendingRotateDegrees = null;

        private void cancelCurrentOperation()
        {
            switch (_internalPendingDriveOperation)
            {
                case drive.DriveRequestOperation.DriveDistance:
                    {
#if TRACELOG
                        Tracer.Trace("cancelCurrentOperation() - DriveDistance - completionPort.ItemCount=" + completionPort.ItemCount);
#endif

                        if (pendingDriveDistance != null)
                        {
                            EncoderTicksEnabled = false;
                            // send notification to subscription manager
                            pendingDriveDistance.Body.DriveDistanceStage = drive.DriveStage.Canceled;
                            double distanceTraveled = pendingDriveDistance.Body.Distance / 2.0d;        // TODO: need to compute real distance traveled
                            pendingDriveDistance.Body.Distance = distanceTraveled;
                            SendNotification<drive.DriveDistance>(_subMgrPort, pendingDriveDistance);
                            pendingDriveDistance = null;
                        }
                    }
                    break;

                case drive.DriveRequestOperation.RotateDegrees:
                    {
#if TRACELOG
                        Tracer.Trace("cancelCurrentOperation() - RotateDegrees - completionPort.ItemCount=" + completionPort.ItemCount);
#endif

                        if (pendingRotateDegrees != null)
                        {
                            EncoderTicksEnabled = false;
                            // send notification to subscription manager
                            pendingRotateDegrees.Body.RotateDegreesStage = drive.DriveStage.Canceled;
                            double angleRotated = pendingRotateDegrees.Body.Degrees / 2.0d;             // TODO: need to compute real angle rotated
                            pendingRotateDegrees.Body.Degrees = angleRotated;
                            SendNotification<drive.RotateDegrees>(_subMgrPort, pendingRotateDegrees);
                            pendingRotateDegrees = null;
                        }
                    }
                    break;

                case drive.DriveRequestOperation.NotSpecified:
                    break;

                default:
                    Tracer.Trace("Warning: cancelCurrentOperation() - no pending wait-type operation to cancel - current operation = " + _internalPendingDriveOperation);
                    break;
            }
            _internalPendingDriveOperation = drive.DriveRequestOperation.NotSpecified;
        }

        /// <summary>
        /// Internal drive distance operation handler
        /// </summary>
        /// <param name="driveDistance"></param>
        /// <returns></returns>
        public virtual IEnumerator<ITask> InternalDriveDistanceHandler(drive.DriveDistance driveDistance)
        {
#if TRACELOG
            Tracer.Trace("InternalDriveDistanceHandler() - DriveStage." + driveDistance.Body.DriveDistanceStage);
#endif

            switch (driveDistance.Body.DriveDistanceStage)
            {
                case drive.DriveStage.InitialRequest:
                    cancelCurrentOperation();
                    pendingDriveDistance = driveDistance;   // we will need it for possible cancelation
                    // _state.InternalPendingDriveOperation = drive.DriveRequestOperation.DriveDistance; - not available in Proxy
                    _internalPendingDriveOperation = drive.DriveRequestOperation.DriveDistance;
                    SpawnIterator<double, double>(driveDistance.Body.Distance, driveDistance.Body.Power, DriveUntilDistance);
                    break;

                case drive.DriveStage.Started:
                    SendNotification<drive.DriveDistance>(_subMgrPort, driveDistance.Body);
                    break;

                case drive.DriveStage.Completed:
                    pendingDriveDistance = null;
                    // _state.InternalPendingDriveOperation = drive.DriveRequestOperation.NotSpecified; - not available in Proxy
                    _internalPendingDriveOperation = drive.DriveRequestOperation.NotSpecified;
                    SendNotification<drive.DriveDistance>(_subMgrPort, driveDistance.Body);
                    break;

                case drive.DriveStage.Canceled:
                    pendingDriveDistance = null;
                    // _state.InternalPendingDriveOperation = drive.DriveRequestOperation.NotSpecified; - not available in Proxy
                    _internalPendingDriveOperation = drive.DriveRequestOperation.NotSpecified;
                    SendNotification<drive.DriveDistance>(_subMgrPort, driveDistance.Body);
                    break;
            }

            yield break;
        }

        /// <summary>
        /// Internal rotate degrees handler
        /// </summary>
        /// <param name="rotateDegrees"></param>
        /// <returns></returns>
        public virtual IEnumerator<ITask> InternalRotateDegreesHandler(drive.RotateDegrees rotateDegrees)
        {
#if TRACELOG
            Tracer.Trace("InternalRotateDegreesHandler() - DriveStage." + rotateDegrees.Body.RotateDegreesStage);
#endif

            switch (rotateDegrees.Body.RotateDegreesStage)
            {
                case drive.DriveStage.InitialRequest:
                    cancelCurrentOperation();
                    pendingRotateDegrees = rotateDegrees;
                    // _state.InternalPendingDriveOperation = drive.DriveRequestOperation.RotateDegrees; - not available in Proxy
                    _internalPendingDriveOperation = drive.DriveRequestOperation.RotateDegrees;
                    SpawnIterator<double, double>(rotateDegrees.Body.Degrees, rotateDegrees.Body.Power, RotateUntilDegrees);
                    break;

                case drive.DriveStage.Started:
                    SendNotification<drive.RotateDegrees>(_subMgrPort, rotateDegrees.Body);
                    break;

                case drive.DriveStage.Completed:
                    pendingRotateDegrees = null;
                    // _state.InternalPendingDriveOperation = drive.DriveRequestOperation.NotSpecified; - not available in Proxy
                    _internalPendingDriveOperation = drive.DriveRequestOperation.NotSpecified;
                    SendNotification<drive.RotateDegrees>(_subMgrPort, rotateDegrees.Body);
                    break;

                case drive.DriveStage.Canceled:
                    pendingRotateDegrees = null;
                    // _state.InternalPendingDriveOperation = drive.DriveRequestOperation.NotSpecified; - not available in Proxy
                    _internalPendingDriveOperation = drive.DriveRequestOperation.NotSpecified;
                    SendNotification<drive.RotateDegrees>(_subMgrPort, rotateDegrees.Body);
                    break;
            }

            yield break;
        }

        #endregion // Internal Drive Operation Handlers

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


