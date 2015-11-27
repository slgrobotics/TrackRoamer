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

using bumper = Microsoft.Robotics.Services.ContactSensor;
using coord = Microsoft.Robotics.Services.Coordination;
using drive = Microsoft.Robotics.Services.Drive;
using encoder = Microsoft.Robotics.Services.Encoder;
using motor = Microsoft.Robotics.Services.Motor;
using submgr = Microsoft.Dss.Services.SubscriptionManager;

//for XSLT
using Microsoft.Dss.Core.DsspHttpUtilities;
using Microsoft.Dss.Core.DsspHttp;
using System.Net;
using System.Collections.Specialized;

//for construction
using cons = Microsoft.Dss.Services.Constructor;
using System.ComponentModel;


namespace Microsoft.Robotics.Services.Drive
{

    /// <summary>
    /// Drive Differential Two Wheel Service Implementation
    /// </summary>
    [DisplayName("Generic Differential Drive")]
    [Description("Provides access to a differential drive (that coordinates two motors that function together).")]
    [Contract(Contract.Identifier)]
    [DssServiceDescription("http://msdn.microsoft.com/library/dd145254.aspx")]
    public class DriveDifferentialTwoWheel : DsspServiceBase
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
        private DriveDifferentialTwoWheelState _state = new DriveDifferentialTwoWheelState();

        [ServicePort("/DriveDifferentialTwoWheel", AllowMultipleInstances = true)]
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
        encoder.EncoderOperations _leftEncoderPort = new encoder.EncoderOperations();

        [Partner(Partners.RightEncoder,
            Optional = true,
            Contract = encoder.Contract.Identifier,
            CreationPolicy = PartnerCreationPolicy.UsePartnerListEntry)]
        encoder.EncoderOperations _rightEncoderPort = new encoder.EncoderOperations();

        //For XSLT
        DsspHttpUtilitiesPort _httpUtilities = new DsspHttpUtilitiesPort();
        const string _transform = ServicePaths.EmbeddedResources + "/RoboticsCommon" +"/Microsoft.Robotics.Drive.xslt";

        /// <summary>
        /// DriveDifferentialTwoWheel Constructor
        /// </summary>
        public DriveDifferentialTwoWheel(DsspServiceCreationPort creationPort) :
                base(creationPort)
        {
        }

        /// <summary>
        /// Service Startup Handler
        /// </summary>
        protected override void Start()
        {
            InitState();

            // send configuration commands to partner services
            SpawnIterator(ConfigureDrive);

            _state.TimeStamp = DateTime.Now;

            //needed for HttpPost
            _httpUtilities = DsspHttpUtilitiesService.Create(Environment);

            // Listen for each operation type and call its Service Handler
            ActivateDsspOperationHandlers();

            // Publish the service to the local Node Directory
            DirectoryInsert();

            // display HTTP service Uri
            LogInfo(LogGroups.Console, "Service uri: ");

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
            if (_state != null)
            {
                _state.TimeStamp = DateTime.Now;
                return;
            }

            _state = new DriveDifferentialTwoWheelState();
            _state.LeftWheel = new motor.WheeledMotorState();
            _state.LeftWheel.MotorState = new motor.MotorState();
            _state.LeftWheel.MotorState.HardwareIdentifier = 1;
            _state.LeftWheel.MotorState.Name = "Left Motor";
            _state.LeftWheel.MotorState.PowerScalingFactor = 1;


            _state.RightWheel = new motor.WheeledMotorState();
            _state.RightWheel.MotorState = new motor.MotorState();
            _state.RightWheel.MotorState.HardwareIdentifier = 2;
            _state.RightWheel.MotorState.Name = "Right Motor";
            _state.RightWheel.MotorState.PowerScalingFactor = 1;

            _state.LeftWheel.EncoderState = new encoder.EncoderState();
            _state.RightWheel.EncoderState = new encoder.EncoderState();

            _state.IsEnabled = true;
            _state.TimeStamp = DateTime.Now;
            _state.InternalPendingDriveOperation = DriveRequestOperation.NotSpecified;

            SaveState(_state);
        }


        private IEnumerator<ITask> ConfigureDrive()
        {
            bool noError = true;

            // Configure motor connections
            motor.Replace configureLeftMotor = new motor.Replace();
            configureLeftMotor.Body = _state.LeftWheel.MotorState;
            _leftMotorPort.Post(configureLeftMotor);

            motor.Replace configureRightMotor = new motor.Replace();
            configureRightMotor.Body = _state.RightWheel.MotorState;
            _rightMotorPort.Post(configureRightMotor);

            yield return Arbiter.Choice(configureLeftMotor.ResponsePort,
                delegate(DefaultReplaceResponseType success) { LogInfo("Left Motor Port set"); },
                delegate(Fault fault) { LogError(fault); noError = false; });

            yield return Arbiter.Choice(configureRightMotor.ResponsePort,
                delegate(DefaultReplaceResponseType success) { LogInfo("Right Motor Port set"); },
                delegate(Fault fault) { LogError(fault); noError = false; });

            // Configure encoder connections
            if (_leftEncoderPort != null)
            {
                encoder.Replace configureLeftEncoder = new encoder.Replace();
                configureLeftEncoder.Body = _state.LeftWheel.EncoderState;
                _leftEncoderPort.Post(configureLeftEncoder);

                yield return Arbiter.Choice(configureLeftEncoder.ResponsePort,
                    delegate(DefaultReplaceResponseType success) { LogInfo("Left Encoder Port set"); },
                    delegate(Fault fault) { LogError(fault); noError = false; });
            }

            if (_rightEncoderPort != null)
            {
                encoder.Replace configureRightEncoder = new encoder.Replace();
                configureRightEncoder.Body = _state.RightWheel.EncoderState;
                _rightEncoderPort.Post(configureRightEncoder);

                yield return Arbiter.Choice(configureRightEncoder.ResponsePort,
                    delegate(DefaultReplaceResponseType success) { LogInfo("Right Encoder Port set"); },
                    delegate(Fault fault) { LogError(fault); noError = false; });

            }


            if (noError)
                _state.IsEnabled = true;

            yield break;
        }

        #region Operation Handlers

        /// <summary>
        /// Get Handler
        /// </summary>
        /// <param name="get"></param>
        /// <returns></returns>
        [ServiceHandler(ServiceHandlerBehavior.Concurrent)]
        public IEnumerator<ITask> GetHandler(Get get)
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
        public IEnumerator<ITask> SubscribeHandler(Subscribe subscribe)
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
        public IEnumerator<ITask> ReliableSubscribeHandler(ReliableSubscribe subscribe)
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
        /// Enable Drive Handler
        /// </summary>
        /// <param name="enableDrive"></param>
        /// <returns></returns>
        [ServiceHandler(ServiceHandlerBehavior.Exclusive)]
        public IEnumerator<ITask> EnableDriveHandler(EnableDrive enableDrive)
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
            drive.Update update = new drive.Update(_state);
            SendNotification<drive.Update>(_subMgrPort, update);

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
        public IEnumerator<ITask> SetDrivePowerHandler(SetDrivePower setDrivePower)
        {
            ValidateDriveConfiguration(false);
            _state.TimeStamp = DateTime.Now;

            PortSet<DefaultUpdateResponseType, Fault> responsePort = new PortSet<DefaultUpdateResponseType, Fault>();

            // Add a coordination header to our motor requests
            // so that advanced motor implementations can
            // coordinate the individual motor reqests.
            coord.ActuatorCoordination coordination = new coord.ActuatorCoordination(2);

            Motor.SetMotorPower leftPower = new Motor.SetMotorPower(setDrivePower.Body.LeftWheelPower);
            leftPower.ResponsePort = responsePort;
            leftPower.AddHeader(coordination);
            _leftMotorPort.Post(leftPower);

            Motor.SetMotorPower rightPower = new Motor.SetMotorPower(setDrivePower.Body.RightWheelPower);
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
        public IEnumerator<ITask> SetDriveSpeedHandler(SetDriveSpeed setDriveSpeed)
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
        public IEnumerator<ITask> RotateDegreesHandler(RotateDegrees rotateDegrees)
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

        /// <summary>
        /// Rotate the the drive (positive degrees turn counterclockwise)
        /// </summary>
        /// <param name="degrees">(positive degrees turn counterclockwise)</param>
        /// <param name="power">(-1.0 to 1.0)</param>
        IEnumerator<ITask> RotateUntilDegrees(double degrees, double power)
        {
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

            double arcDistance = Math.Abs(degrees) * _state.DistanceBetweenWheels * 3.14159 / 360;

            //compute tick to stop at
            int stopLeftWheelAt = (int)Math.Round(arcDistance / (2.0 * 3.14159 * _state.LeftWheel.Radius / _state.LeftWheel.EncoderState.TicksPerRevolution));
            int stopRightWheelAt = (int)Math.Round(arcDistance / (2.0 * 3.14159 * _state.RightWheel.Radius / _state.RightWheel.EncoderState.TicksPerRevolution));

            // Subscribe to the encoders on internal ports
            encoder.EncoderOperations leftNotificationPort = new encoder.EncoderOperations();
            encoder.EncoderOperations rightNotificationPort = new encoder.EncoderOperations();

            encoder.Subscribe op = new encoder.Subscribe();
            op.Body = new SubscribeRequestType();
            op.NotificationPort = leftNotificationPort;
            _leftEncoderPort.Post(op);

            yield return (Arbiter.Choice(op.ResponsePort,
                delegate(SubscribeResponseType response)
                {
                    //subscription was successful, start listening for encoder replace messages
                    Activate(Arbiter.Receive<encoder.UpdateTickCount>(false, leftNotificationPort,
                        delegate(encoder.UpdateTickCount update)
                        {
                            StopMotorWithEncoderHandler(leftNotificationPort, update, _leftMotorPort, stopLeftWheelAt);
                        }));
                },
                delegate(Fault fault) { LogError(fault); }
            ));


            encoder.Subscribe op2 = new encoder.Subscribe();
            op2.Body = new SubscribeRequestType();
            op2.NotificationPort = rightNotificationPort;
            _leftEncoderPort.Post(op2);
            yield return (Arbiter.Choice(op2.ResponsePort,
                delegate(SubscribeResponseType response)
                {
                    //subscription was successful, start listening for encoder replace messages
                    Activate(Arbiter.Receive<encoder.UpdateTickCount>(false, rightNotificationPort,
                        delegate(encoder.UpdateTickCount update)
                        {
                            StopMotorWithEncoderHandler(rightNotificationPort, update, _rightMotorPort, stopRightWheelAt);
                        }
                    ));
                },
                delegate(Fault fault) { LogError(fault); }
            ));


            //start moving

            // start rotate operation
            _state.RotateDegreesStage = DriveStage.Started;

            RotateDegrees rotateUpdate = new RotateDegrees();
            rotateUpdate.Body.RotateDegreesStage = DriveStage.Started;
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

            Motor.SetMotorPower leftPower = new Motor.SetMotorPower(leftPow);
            leftPower.ResponsePort = responsePort;
            _leftMotorPort.Post(leftPower);

            Motor.SetMotorPower rightPower = new Motor.SetMotorPower(rightPow);
            rightPower.ResponsePort = responsePort;
            _rightMotorPort.Post(rightPower);

            Activate(Arbiter.MultipleItemReceive<DefaultUpdateResponseType, Fault>(responsePort, 2,
                delegate(ICollection<DefaultUpdateResponseType> successList, ICollection<Fault> failureList)
                {
                    foreach (Fault fault in failureList)
                    {
                        LogError(fault);
                    }
                }
            ));

            _state.RotateDegreesStage = DriveStage.Completed;
            //complete
            rotateUpdate.Body.RotateDegreesStage = DriveStage.Completed;
            _internalDriveOperationsPort.Post(rotateUpdate);
        }

        #endregion

        #region Drive Distance

        /// <summary>
        /// Drive Distance Handler
        /// </summary>
        /// <param name="driveDistance"></param>
        /// <returns></returns>
        [ServiceHandler(ServiceHandlerBehavior.Exclusive)]
        public IEnumerator<ITask> DriveDistanceHandler(DriveDistance driveDistance)
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
            int stopLeftWheelAt = (int)Math.Round(Math.Abs(distance) / (2.0 * 3.14159 * _state.LeftWheel.Radius / _state.LeftWheel.EncoderState.TicksPerRevolution));
            int stopRightWheelAt = (int)Math.Round(Math.Abs(distance) / (2.0 * 3.14159 * _state.RightWheel.Radius / _state.RightWheel.EncoderState.TicksPerRevolution));

            // Subscribe to the encoders on internal ports
            encoder.EncoderOperations leftNotificationPort = new encoder.EncoderOperations();
            encoder.EncoderOperations rightNotificationPort = new encoder.EncoderOperations();

            encoder.Subscribe op = new encoder.Subscribe();
            op.Body = new SubscribeRequestType();
            op.NotificationPort = leftNotificationPort;
            _leftEncoderPort.Post(op);
            yield return (Arbiter.Choice(op.ResponsePort,
                delegate(SubscribeResponseType response)
                {
                    //subscription was successful, start listening for encoder replace messages
                    Activate(Arbiter.Receive<encoder.UpdateTickCount>(false, leftNotificationPort,
                        delegate(encoder.UpdateTickCount update)
                        {
                            StopMotorWithEncoderHandler(leftNotificationPort, update, _leftMotorPort, stopLeftWheelAt);
                        }));
                },
                delegate(Fault fault) { LogError(fault); }
            ));


            encoder.Subscribe op2 = new encoder.Subscribe();
            op2.Body = new SubscribeRequestType();
            op2.NotificationPort = rightNotificationPort;
            _leftEncoderPort.Post(op2);
            yield return (Arbiter.Choice(op2.ResponsePort,
                delegate(SubscribeResponseType response)
                {
                    //subscription was successful, start listening for encoder replace messages
                    Activate(Arbiter.Receive<encoder.UpdateTickCount>(false, rightNotificationPort,
                        delegate(encoder.UpdateTickCount update)
                        {
                            StopMotorWithEncoderHandler(rightNotificationPort, update, _rightMotorPort, stopRightWheelAt);
                        }
                    ));
                },
                delegate(Fault fault) { LogError(fault); }
            ));


            //start moving

            double Pow;

            if (distance > 0)
                Pow = power;
            else
                Pow = -power;

            PortSet<DefaultUpdateResponseType, Fault> responsePort = new PortSet<DefaultUpdateResponseType, Fault>();


            // send notification of driveDistance start to subscription manager
            _state.DriveDistanceStage = DriveStage.Started;

            DriveDistance driveUpdate = new DriveDistance();
            driveUpdate.Body.DriveDistanceStage = DriveStage.Started;
            _internalDriveOperationsPort.Post(driveUpdate);

            Motor.SetMotorPower leftPower = new Motor.SetMotorPower(Pow);
            leftPower.ResponsePort = responsePort;
            _leftMotorPort.Post(leftPower);

            Motor.SetMotorPower rightPower = new Motor.SetMotorPower(Pow);
            rightPower.ResponsePort = responsePort;
            _rightMotorPort.Post(rightPower);

            Activate(Arbiter.MultipleItemReceive<DefaultUpdateResponseType, Fault>(responsePort, 2,
                delegate(ICollection<DefaultUpdateResponseType> successList, ICollection<Fault> failureList)
                {
                    foreach (Fault fault in failureList)
                    {
                        LogError(fault);
                    }
                }
            ));

            // send notification of driveDistance complete to subscription manager
            driveUpdate.Body.DriveDistanceStage = DriveStage.Completed;
            _internalDriveOperationsPort.Post(driveUpdate);

            _state.DriveDistanceStage = DriveStage.Completed;

        }

        #endregion

        void StopMotorWithEncoderHandler(encoder.EncoderOperations encoderNotificationPort, encoder.UpdateTickCount msg, Motor.MotorOperations motorPort, int stopWheelAt)
        {
            if (msg.Body.Count >= stopWheelAt)
            {
                Motor.SetMotorPower stop = new Motor.SetMotorPower(0);
                motorPort.Post(stop);
                Arbiter.Choice(stop.ResponsePort,
                    delegate(DefaultUpdateResponseType resp) { },
                    delegate(Fault fault) { LogError(fault); }
                );
            }
            else
            {
                // Call self to continue waiting for notifications
                Activate(Arbiter.Receive<encoder.UpdateTickCount>(false, encoderNotificationPort,
                    delegate(encoder.UpdateTickCount update)
                    {
                        StopMotorWithEncoderHandler(encoderNotificationPort, update, motorPort, stopWheelAt);
                    }
                ));
            }
        }

        /// <summary>
        /// All Stop Handler
        /// </summary>
        /// <param name="allStop"></param>
        /// <returns></returns>
        [ServiceHandler(ServiceHandlerBehavior.Exclusive)]
        public IEnumerator<ITask> AllStopHandler(AllStop allStop)
        {
            SetDrivePower zeroPower = new SetDrivePower();
            zeroPower.Body = new SetDrivePowerRequest(0.0, 0.0);
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

                if (operation is DriveDistance)
                {
                    _mainPort.Post((DriveDistance)operation);
                }
                else if (operation is SetDrivePower)
                {
                    _mainPort.Post((SetDrivePower)operation);
                }
                else if (operation is RotateDegrees)
                {
                    _mainPort.Post((RotateDegrees)operation);
                }
                else if (operation is EnableDrive)
                {
                    _mainPort.Post((EnableDrive)operation);
                }
                else if (operation is AllStop)
                {
                    _mainPort.Post((AllStop)operation);
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
                        SetDrivePowerRequest drivepower = new SetDrivePowerRequest();
                        drivepower.LeftWheelPower = power;
                        drivepower.RightWheelPower = power;
                        _mainPort.Post(new SetDrivePower(drivepower));
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
