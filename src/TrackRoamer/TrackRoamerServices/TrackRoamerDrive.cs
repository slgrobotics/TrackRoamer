//-----------------------------------------------------------------------
//  This file is part of Microsoft Robotics Developer Studio Code Samples.
//
//  Copyright (C) Microsoft Corporation.  All rights reserved.
//
//  $File: Drive.cs $ $Revision: 1 $
//-----------------------------------------------------------------------

using Microsoft.Ccr.Core;
using Microsoft.Dss.Core;
using Microsoft.Dss.Core.Attributes;
using Microsoft.Dss.ServiceModel.Dssp;
using Microsoft.Dss.ServiceModel.DsspServiceBase;
using System;
using System.Collections.Generic;
using System.Security.Permissions;
using W3C.Soap;

//for XSLT
using Microsoft.Dss.Core.DsspHttpUtilities;
using Microsoft.Dss.Core.DsspHttp;
using System.Net;
using System.Collections.Specialized;

//for construction
using cons = Microsoft.Dss.Services.Constructor;
using System.ComponentModel;

using submgr = Microsoft.Dss.Services.SubscriptionManager;
using coord = Microsoft.Robotics.Services.Coordination.Proxy;

using drive = Microsoft.Robotics.Services.Drive.Proxy;
using encoder = Microsoft.Robotics.Services.Encoder.Proxy;
using motor = Microsoft.Robotics.Services.Motor.Proxy;
//using bumper = Microsoft.Robotics.Services.ContactSensor.Proxy;

using trackroamerbot = TrackRoamer.Robotics.Services.TrackRoamerBot.Proxy;

namespace TrackRoamer.Robotics.Services.TrackRoamerServices.Drive
{

    /// <summary>
    /// TrackRoamer Drive Service - Drive Differential Two Wheel Service Implementation
    /// </summary>
    [Contract(Contract.Identifier)]
    //[AlternateContract(drive.Contract.Identifier)]
    [DisplayName("TrackRoamer Differential Drive")]
    [Description("Provides access to an TrackRoamer differential motor drive - coordinates two motors that function together.\n(Uses the Generic Differential Drive contract.)\n(Partner with the 'TrackRoamerBot' service.)")]
    [DssServiceDescription("http://msdn.microsoft.com/library/dd145254.aspx")]
    public class TrackRoamerDriveService : DsspServiceBase
    {
        /// <summary>
        /// Default Left Motor Name
        /// </summary>
        public const string DefaultLeftMotorName = "/LeftMotor";

        /// <summary>
        /// Default Right Motor Name
        /// </summary>
        public const string DefaultRightMotorName = "/RightMotor";

        [InitialStatePartner(Optional = true)]
        private TrackRoamerDriveState _state = new TrackRoamerDriveState();

        [ServicePort("/trackroamer/drive", AllowMultipleInstances = true)]
        private DriveOperations _mainPort = new DriveOperations();

        // This is the internal drive port for excuting the drive operations:
        //  driveDistance, and rotateDegrees.
        private PortSet<drive.DriveDistance, drive.RotateDegrees> _internalDriveOperationsPort = new PortSet<drive.DriveDistance, drive.RotateDegrees>();

        // Port used for canceling a driveDistance or RotateDegrees operation.
        private Port<drive.CancelPendingDriveOperation> _internalDriveCancalOperationPort = new Port<drive.CancelPendingDriveOperation>();


        [Partner("SubMgr", Contract = submgr.Contract.Identifier, CreationPolicy = PartnerCreationPolicy.CreateAlways)]
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
            Optional = true,
            Contract = encoder.Contract.Identifier,
            CreationPolicy = PartnerCreationPolicy.UsePartnerListEntry)]
        encoder.EncoderOperations _leftEncoderCmdPort = new encoder.EncoderOperations();
        encoder.EncoderOperations _leftEncoderTickPort = new encoder.EncoderOperations();
        bool _leftEncoderTickEnabled = false;

        [Partner(Partners.RightEncoder,
            Optional = true,
            Contract = encoder.Contract.Identifier,
            CreationPolicy = PartnerCreationPolicy.UsePartnerListEntry)]
        encoder.EncoderOperations _rightEncoderCmdPort = new encoder.EncoderOperations();
        encoder.EncoderOperations _rightEncoderTickPort = new encoder.EncoderOperations();
        bool _rightEncoderTickEnabled = false;

        //For XSLT
        DsspHttpUtilitiesPort _httpUtilities = new DsspHttpUtilitiesPort();
        [EmbeddedResource("TrackRoamer.Robotics.Services.TrackRoamerServices.TrackRoamerGenericDriveState.xslt")]
        string _transform = null;

        /// <summary>
        /// Default Service Constructor
        /// </summary>
        public TrackRoamerDriveService(DsspServiceCreationPort creationPort) :
                base(creationPort)
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

            // send configuration commands to partner services
            SpawnIterator(ConfigureDrive);

            _state.TimeStamp = DateTime.Now;

            //needed for HttpPost
            _httpUtilities = DsspHttpUtilitiesService.Create(Environment);

            // Listen for each operation type and call its Service Handler
            //ActivateDsspOperationHandlers();

            // Publish the service to the local Node Directory
            //DirectoryInsert();

            base.Start();

            // display HTTP service Uri
            LogInfo("TrackRoamerDriveService:: Service uri: ");

            //for XSLT and images
            MountEmbeddedResources("/RoboticsCommon");

            // Interleave to manage internal drive operations (driveDistance and RotateDegrees)
            Activate(
                new Interleave(
                    new ExclusiveReceiverGroup(
                    Arbiter.ReceiveWithIteratorFromPortSet<drive.DriveDistance>(true, _internalDriveOperationsPort, InternalDriveDistanceHandler),
                    Arbiter.ReceiveWithIteratorFromPortSet<drive.RotateDegrees>(true, _internalDriveOperationsPort, InternalRotateDegreesHandler)
                    ),
                    new ConcurrentReceiverGroup())
                   );
        }

        private void InitState()
        {
            LogInfo("TrackRoamerDriveService:: InitState() _state=" + _state);

            if (_state == null)
            {
                LogInfo("TrackRoamerDriveService:: InitState()  (null)");

                _state = new TrackRoamerDriveState();

                _state.DistanceBetweenWheels = 0.5715;

                _state.LeftWheel = new motor.WheeledMotorState();
                _state.LeftWheel.Radius = 0.1805;
                _state.LeftWheel.GearRatio = 0.136;
                _state.LeftWheel.MotorState = new motor.MotorState();
                _state.LeftWheel.MotorState.HardwareIdentifier = 1;
                _state.LeftWheel.MotorState.Name = "Left Motor";
                _state.LeftWheel.MotorState.PowerScalingFactor = 30;
                _state.LeftWheel.MotorState.ReversePolarity = false;

                _state.RightWheel = new motor.WheeledMotorState();
                _state.RightWheel.Radius = 0.1805;
                _state.RightWheel.GearRatio = 0.136;
                _state.RightWheel.MotorState = new motor.MotorState();
                _state.RightWheel.MotorState.HardwareIdentifier = 2;
                _state.RightWheel.MotorState.Name = "Right Motor";
                _state.RightWheel.MotorState.PowerScalingFactor = 30;
                _state.RightWheel.MotorState.ReversePolarity = false;

                _state.LeftWheel.EncoderState = new encoder.EncoderState();
                _state.LeftWheel.EncoderState.HardwareIdentifier = 1;
                _state.LeftWheel.EncoderState.TicksPerRevolution = 2993;

                _state.RightWheel.EncoderState = new encoder.EncoderState();
                _state.RightWheel.EncoderState.HardwareIdentifier = 2;
                _state.RightWheel.EncoderState.TicksPerRevolution = 2993;

                _state.IsEnabled = true;
                _state.TimeStamp = DateTime.Now;

                LogInfo("TrackRoamerDriveService:: InitState(): saving state");

                SaveState(_state);
            }
            else
            {
                LogInfo("TrackRoamerDriveService:: InitState() (not null) _state.DistanceBetweenWheels=" + _state.DistanceBetweenWheels);
            }

            _state.InternalPendingDriveOperation = drive.DriveRequestOperation.NotSpecified;
            _state.TimeStamp = DateTime.Now;
        }


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
                delegate(DefaultReplaceResponseType success) { LogInfo("    Left Motor Port set"); },
                delegate(Fault fault) { LogError(fault); noError = false; });

            yield return Arbiter.Choice(configureRightMotor.ResponsePort,
                delegate(DefaultReplaceResponseType success) { LogInfo("    Right Motor Port set"); },
                delegate(Fault fault) { LogError(fault); noError = false; });

            // Configure encoder connections
            if (_leftEncoderCmdPort != null)
            {
                encoder.Replace configureLeftEncoder = new encoder.Replace();
                configureLeftEncoder.Body = _state.LeftWheel.EncoderState;
                _leftEncoderCmdPort.Post(configureLeftEncoder);

                yield return Arbiter.Choice(configureLeftEncoder.ResponsePort,
                    delegate(DefaultReplaceResponseType success) { LogInfo("    Left Encoder Port set"); },
                    delegate(Fault fault) { LogError(fault); noError = false; });

                encoder.Subscribe op = new encoder.Subscribe();
                op.Body = new SubscribeRequestType();
                op.NotificationPort = _leftEncoderTickPort;
                _leftEncoderCmdPort.Post(op);

                yield return (Arbiter.Choice(op.ResponsePort,
                    delegate(SubscribeResponseType response)
                    {
                        //subscription was successful, start listening for encoder replace messages
                        Activate(Arbiter.Receive<encoder.UpdateTickCount>(true, _leftEncoderTickPort,
                            delegate(encoder.UpdateTickCount update)
                            {
                                StopMotorWithEncoderHandler(_leftEncoderTickPort, "left", update, _leftMotorPort);
                            }));
                    },
                    delegate(Fault fault) { LogError(fault); }
                ));
            }

            if (_rightEncoderCmdPort != null)
            {
                encoder.Replace configureRightEncoder = new encoder.Replace();
                configureRightEncoder.Body = _state.RightWheel.EncoderState;
                _rightEncoderCmdPort.Post(configureRightEncoder);

                yield return Arbiter.Choice(configureRightEncoder.ResponsePort,
                    delegate(DefaultReplaceResponseType success) { LogInfo("    Right Encoder Port set"); },
                    delegate(Fault fault) { LogError(fault); noError = false; });

                encoder.Subscribe op2 = new encoder.Subscribe();
                op2.Body = new SubscribeRequestType();
                op2.NotificationPort = _rightEncoderTickPort;
                _leftEncoderCmdPort.Post(op2);

                yield return (Arbiter.Choice(op2.ResponsePort,
                    delegate(SubscribeResponseType response)
                    {
                        //subscription was successful, start listening for encoder replace messages
                        Activate(Arbiter.Receive<encoder.UpdateTickCount>(true, _rightEncoderTickPort,
                            delegate(encoder.UpdateTickCount update)
                            {
                                StopMotorWithEncoderHandler(_rightEncoderTickPort, "right", update, _rightMotorPort);
                            }
                        ));
                    },
                    delegate(Fault fault) { LogError(fault); }
                ));
            }

            if (noError)
            {
                LogInfo("TrackRoamerDriveService:: ConfigureDrive() - success");
                _state.IsEnabled = true;
            }

            yield break;
        }

        #region Operation Handlers

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
        public IEnumerator<ITask> UpdateHandler(Update update)
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
                    Update update = new Update(_state);
                    SendNotificationToTarget<Update>(subscribe.Body.Subscriber, _subMgrPort, update);
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
                    Update update = new Update(_state);
                    SendNotificationToTarget<Update>(subscribe.Body.Subscriber, _subMgrPort, update);
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
        /// Enable Drive Handler
        /// </summary>
        /// <param name="enableDrive"></param>
        /// <returns></returns>
        [ServiceHandler(ServiceHandlerBehavior.Exclusive)]
        public IEnumerator<ITask> EnableDriveHandler(drive.EnableDrive enableDrive)
        {
            _state.IsEnabled = enableDrive.Body.Enable;
            _state.TimeStamp = DateTime.Now;

            // if we are enabling the drive, validate that the motors are configured.
            if (enableDrive.Body.Enable)
            {
                try
                {
                    ValidateDriveConfiguration(false);
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
            Update update = new Update(_state);
            SendNotification<Update>(_subMgrPort, update);

            enableDrive.ResponsePort.Post(DefaultUpdateResponseType.Instance);
            yield break;
        }

        #region Drive Power

        /// <summary>
        /// Set Drive Power Handler
        /// </summary>
        /// <param name="setDrivePower"></param>
        /// <returns></returns>
        [ServiceHandler(ServiceHandlerBehavior.Exclusive)]
        public IEnumerator<ITask> SetDrivePowerHandler(drive.SetDrivePower setDrivePower)
        {
            ValidateDriveConfiguration(false);
            _state.TimeStamp = DateTime.Now;

            PortSet<DefaultUpdateResponseType, Fault> responsePort = new PortSet<DefaultUpdateResponseType, Fault>();

            // Add a coordination header to our motor requests
            // so that advanced motor implementations can
            // coordinate the individual motor reqests.
            coord.ActuatorCoordination coordination = new coord.ActuatorCoordination();

            motor.SetMotorPower leftPower = new motor.SetMotorPower(new motor.SetMotorPowerRequest() { TargetPower = setDrivePower.Body.LeftWheelPower } );
            leftPower.ResponsePort = responsePort;
            leftPower.AddHeader(coordination);
            _leftMotorPort.Post(leftPower);

            motor.SetMotorPower rightPower = new motor.SetMotorPower(new motor.SetMotorPowerRequest() { TargetPower = setDrivePower.Body.RightWheelPower } );
            rightPower.ResponsePort = responsePort;
            rightPower.AddHeader(coordination);
            _rightMotorPort.Post(rightPower);

            // send notification to subscription manager
            Update update = new Update(_state);
            SendNotification<Update>(_subMgrPort, update);

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

            yield break;
        }
        #endregion

        #region Drive Speed
        /// <summary>
        /// Set Drive Speed Handler
        /// </summary>
        /// <param name="setDriveSpeed"></param>
        /// <returns></returns>
        [ServiceHandler(ServiceHandlerBehavior.Exclusive)]
        public IEnumerator<ITask> SetDriveSpeedHandler(drive.SetDriveSpeed setDriveSpeed)
        {
            ValidateDriveConfiguration(true);
            _state.TimeStamp = DateTime.Now;
            LogError("Drive speed is not implemented");
            throw new NotImplementedException();
        }
        #endregion

        #region Rotate Degrees

        /// <summary>
        /// Rotate Degrees Handler (positive degrees turn counterclockwise)
        /// </summary>
        /// <param name="rotateDegrees"></param>
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
                _state.TimeStamp = DateTime.Now;

                // send immediate response
                rotateDegrees.ResponsePort.Post(DefaultUpdateResponseType.Instance);


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
            LogInfo("^^^^^^^^^^^^^^^^^^^^^ TrackRoamerDriveService:: RotateUntilDegrees(degrees=" + degrees + ",  power=" + power + ")");

            _leftEncoderTickEnabled = false;
            _rightEncoderTickEnabled = false;

            //reset encoders
            encoder.Reset Lreset = new encoder.Reset();
            _leftEncoderCmdPort.Post(Lreset);
            yield return (Arbiter.Choice(Lreset.ResponsePort,
                delegate(DefaultUpdateResponseType response) { },
                delegate(Fault fault) { LogError(fault); }
            ));

            encoder.Reset Rreset = new encoder.Reset();
            _rightEncoderCmdPort.Post(Rreset);
            yield return (Arbiter.Choice(Rreset.ResponsePort,
                delegate(DefaultUpdateResponseType response) { },
                delegate(Fault fault) { LogError(fault); }
            ));

            double arcDistance = Math.Abs(degrees) * _state.DistanceBetweenWheels * Math.PI / 360.0d;

            //compute tick to stop at
            stopLeftWheelAt = (int)Math.Round(arcDistance / (2.0d * Math.PI * _state.LeftWheel.Radius / _state.LeftWheel.EncoderState.TicksPerRevolution));
            stopRightWheelAt = (int)Math.Round(arcDistance / (2.0d * Math.PI * _state.RightWheel.Radius / _state.RightWheel.EncoderState.TicksPerRevolution));

            _leftEncoderTickEnabled = true;
            _rightEncoderTickEnabled = true;

            //start moving

            // start rotate operation
            _state.RotateDegreesStage = drive.DriveStage.Started;

            drive.RotateDegrees rotateUpdate = new drive.RotateDegrees();
            rotateUpdate.Body.RotateDegreesStage = drive.DriveStage.Started;
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

            motor.SetMotorPower leftPower = new motor.SetMotorPower(new motor.SetMotorPowerRequest() { TargetPower = leftPow } );
            leftPower.ResponsePort = responsePort;
            _leftMotorPort.Post(leftPower);

            motor.SetMotorPower rightPower = new motor.SetMotorPower(new motor.SetMotorPowerRequest() { TargetPower = rightPow } );
            rightPower.ResponsePort = responsePort;
            _rightMotorPort.Post(rightPower);

            LogInfo("=============== TrackRoamerDriveService:: RotateUntilDegrees() start moving: degrees=" + degrees);
            LogInfo("=============== TrackRoamerDriveService:: RotateUntilDegrees() will stop wheels at:  Left=" + stopLeftWheelAt + " Right=" + stopRightWheelAt);

            Activate(Arbiter.MultipleItemReceive<DefaultUpdateResponseType, Fault>(responsePort, 2,
                delegate(ICollection<DefaultUpdateResponseType> successList, ICollection<Fault> failureList)
                {
                    foreach (Fault fault in failureList)
                    {
                        LogError(fault);
                    }
                }
            ));

            LogInfo("=============== TrackRoamerDriveService:: RotateUntilDegrees() calling WaitForCompletion() - waiting for the first side to complete...");

            yield return WaitForCompletion();

            LogInfo("=============== TrackRoamerDriveService:: RotateUntilDegrees() calling WaitForCompletion() - other side should complete too...");

            yield return WaitForCompletion();

            LogInfo("=============== TrackRoamerDriveService:: RotateUntilDegrees() - both sides completed, send notification of RotateDegrees complete to subscription manager");

            // send notification of RotateDegrees complete to subscription manager
            rotateUpdate.Body.RotateDegreesStage = drive.DriveStage.Completed;
            _internalDriveOperationsPort.Post(rotateUpdate);

            _state.RotateDegreesStage = drive.DriveStage.Completed;
        }

        #endregion

        #region Drive Distance

        /// <summary>
        /// Drive Distance Handler
        /// </summary>
        /// <param name="driveDistance"></param>
        /// <returns></returns>
        [ServiceHandler(ServiceHandlerBehavior.Exclusive)]
        public IEnumerator<ITask> DriveDistanceHandler(drive.DriveDistance driveDistance)
        {
            // If configuration is invalid, an InvalidException is thrown.
            ValidateDriveConfiguration(true);
            _state.TimeStamp = DateTime.Now;

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
            LogInfo("=============== TrackRoamerDriveService:: DriveUntilDistance(distance=" + distance + " meters,  power=" + power + ")");

            _leftEncoderTickEnabled = false;
            _rightEncoderTickEnabled = false;

            //reset encoders
            encoder.Reset Lreset = new encoder.Reset();
            _leftEncoderCmdPort.Post(Lreset);
            yield return (Arbiter.Choice(Lreset.ResponsePort,
                delegate(DefaultUpdateResponseType response) { },
                delegate(Fault fault) { LogError(fault); }
            ));

            encoder.Reset Rreset = new encoder.Reset();
            _rightEncoderCmdPort.Post(Rreset);
            yield return (Arbiter.Choice(Rreset.ResponsePort,
                delegate(DefaultUpdateResponseType response) { },
                delegate(Fault fault) { LogError(fault); }
            ));

            //compute tick to stop at
            stopLeftWheelAt = (int)Math.Round(Math.Abs(distance) / (2.0 * 3.14159 * _state.LeftWheel.Radius / _state.LeftWheel.EncoderState.TicksPerRevolution));
            stopRightWheelAt = (int)Math.Round(Math.Abs(distance) / (2.0 * 3.14159 * _state.RightWheel.Radius / _state.RightWheel.EncoderState.TicksPerRevolution));

            _leftEncoderTickEnabled = true;
            _rightEncoderTickEnabled = true;

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
            _internalDriveOperationsPort.Post(driveUpdate);

            motor.SetMotorPower leftPower = new motor.SetMotorPower(new motor.SetMotorPowerRequest() { TargetPower = Pow } );
            leftPower.ResponsePort = responsePort;
            _leftMotorPort.Post(leftPower);

            motor.SetMotorPower rightPower = new motor.SetMotorPower(new motor.SetMotorPowerRequest() { TargetPower = Pow } );
            rightPower.ResponsePort = responsePort;
            _rightMotorPort.Post(rightPower);

            LogInfo("=============== TrackRoamerDriveService:: DriveUntilDistance() start moving: distance=" + distance + " meters");
            LogInfo("=============== TrackRoamerDriveService:: DriveUntilDistance() will stop wheel at:  Left=" + stopLeftWheelAt + "   Right=" + stopRightWheelAt);

            Activate(Arbiter.MultipleItemReceive<DefaultUpdateResponseType, Fault>(responsePort, 2,
                delegate(ICollection<DefaultUpdateResponseType> successList, ICollection<Fault> failureList)
                {
                    foreach (Fault fault in failureList)
                    {
                        LogError(fault);
                    }
                }
            ));

            LogInfo("=============== TrackRoamerDriveService:: DriveUntilDistance() calling WaitForCompletion() - waiting for the first side to complete...");

            yield return WaitForCompletion();

            LogInfo("=============== TrackRoamerDriveService:: DriveUntilDistance() calling WaitForCompletion() - other side should complete too...");

            yield return WaitForCompletion();

            LogInfo("=============== TrackRoamerDriveService:: DriveUntilDistance() - both sides completed, send notification of driveDistance complete to subscription manager");

            // send notification of driveDistance complete to subscription manager
            driveUpdate.Body.DriveDistanceStage = drive.DriveStage.Completed;
            _internalDriveOperationsPort.Post(driveUpdate);

            _state.DriveDistanceStage = drive.DriveStage.Completed;
        }

        #endregion

        // This port is sent a message every time that there is a
        // Canceled or Complete message from the Drive, so it can
        // be used to wait for completion.
        private Port<drive.DriveStage> completionPort = new Port<drive.DriveStage>();

        /// <summary>
        /// WaitForCompletion - Helper function to wait on Completion Port
        /// </summary>
        /// <returns>Receiver suitable for waiting on</returns>
        private Receiver<drive.DriveStage> WaitForCompletion()
        {
            // Note that this method does nothing with the drive status
            return Arbiter.Receive(false, completionPort, EmptyHandler<drive.DriveStage>);
        }

        void StopMotorWithEncoderHandler(encoder.EncoderOperations encoderNotificationPort, string side, encoder.UpdateTickCount update, motor.MotorOperations motorPort)
        {
            //LogInfo("^^^^^^^^^^^^^^^^^^^^^ TrackRoamerDriveService:: StopMotorWithEncoderHandler() " + side + " encoder at=" + update.Body.Count + "    will stop wheel at=" + stopWheelAt);

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

            if (!ignore && update.Body.Count >= stopWheelAt)
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

                motor.SetMotorPower stop = new motor.SetMotorPower(new motor.SetMotorPowerRequest() { TargetPower = 0 } );
                motorPort.Post(stop);
                Arbiter.Choice(stop.ResponsePort,
                    delegate(DefaultUpdateResponseType resp) {
                        LogInfo("^^^^^^^^^^^^^^^^^^^^^ TrackRoamerDriveService:: StopMotorWithEncoderHandler() " + side + " - motor stopped !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!");
                    },
                    delegate(Fault fault) { LogError(fault); }
                );

                LogInfo("^^^^^^^^^^^^^^^^^^^^^ TrackRoamerDriveService:: StopMotorWithEncoderHandler() " + side + " - Sending internal DriveStage.Completed !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!");
                completionPort.Post(drive.DriveStage.Completed);
            }
        }

        /// <summary>
        /// All Stop Handler
        /// </summary>
        /// <param name="allStop"></param>
        /// <returns></returns>
        [ServiceHandler(ServiceHandlerBehavior.Exclusive)]
        public IEnumerator<ITask> AllStopHandler(drive.AllStop allStop)
        {
            drive.SetDrivePower zeroPower = new drive.SetDrivePower();
            zeroPower.Body = new drive.SetDrivePowerRequest(0.0d, 0.0d);
            zeroPower.ResponsePort = allStop.ResponsePort;
            _mainPort.Post(zeroPower);
            yield break;
        }

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
                else
                {
                    NameValueCollection parameters = formData.Parameters;

                    if (parameters["StartDashboard"] != null)
                    {
                        string Dashboardcontract = "http://schemas.microsoft.com/robotics/2006/01/simpledashboard.html";
                        ServiceInfoType info = new ServiceInfoType(Dashboardcontract);
                        cons.Create create = new cons.Create(info);
                        create.TimeSpan = DsspOperation.DefaultShortTimeSpan;

                        ConstructorPort.Post(create);
                        Arbiter.Choice(
                            create.ResponsePort,
                            delegate(CreateResponse createResponse) { },
                            delegate(Fault f) { LogError(f); }
                        );
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
        #endregion


        #endregion

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

        #region Internal Drive Handlers
        /// <summary>
        /// Internal drive distance operation handler
        /// </summary>
        /// <param name="driveDistance"></param>
        /// <returns></returns>
        public virtual IEnumerator<ITask> InternalDriveDistanceHandler(drive.DriveDistance driveDistance)
        {
            switch (driveDistance.Body.DriveDistanceStage)
            {
                case drive.DriveStage.InitialRequest:
                    _state.InternalPendingDriveOperation = drive.DriveRequestOperation.DriveDistance;
                    SpawnIterator<double, double>(driveDistance.Body.Distance, driveDistance.Body.Power, DriveUntilDistance);
                    break;

                case drive.DriveStage.Started:
                    SendNotification<drive.DriveDistance>(_subMgrPort, driveDistance.Body);
                    break;

                case drive.DriveStage.Completed:
                    _state.InternalPendingDriveOperation = drive.DriveRequestOperation.NotSpecified;
                    SendNotification<drive.DriveDistance>(_subMgrPort, driveDistance.Body);
                    break;

                case drive.DriveStage.Canceled:
                    _state.InternalPendingDriveOperation = drive.DriveRequestOperation.NotSpecified;
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
            switch (rotateDegrees.Body.RotateDegreesStage)
            {
                case drive.DriveStage.InitialRequest:
                    _state.InternalPendingDriveOperation = drive.DriveRequestOperation.RotateDegrees;
                    SpawnIterator<double, double>(rotateDegrees.Body.Degrees, rotateDegrees.Body.Power, RotateUntilDegrees);
                    break;

                case drive.DriveStage.Started:
                    SendNotification<drive.RotateDegrees>(_subMgrPort, rotateDegrees.Body);
                    break;

                case drive.DriveStage.Completed:
                    _state.InternalPendingDriveOperation = drive.DriveRequestOperation.NotSpecified;
                    SendNotification<drive.RotateDegrees>(_subMgrPort, rotateDegrees.Body);
                    break;

                case drive.DriveStage.Canceled:
                    _state.InternalPendingDriveOperation = drive.DriveRequestOperation.NotSpecified;
                    SendNotification<drive.RotateDegrees>(_subMgrPort, rotateDegrees.Body);
                    break;
            }

            yield break;
        }
        #endregion
    }
}
