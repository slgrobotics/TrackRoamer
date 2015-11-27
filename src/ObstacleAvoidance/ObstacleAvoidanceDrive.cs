//------------------------------------------------------------------------------
//  <copyright file="ObstacleAvoidanceDrive.cs" company="Microsoft Corporation">
//      Copyright (C) Microsoft Corporation.  All rights reserved.
//  </copyright>
//------------------------------------------------------------------------------
namespace Microsoft.Robotics.Services.ObstacleAvoidanceDrive
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Drawing;
    using System.Drawing.Imaging;
    using System.IO;
    using System.Net;
    using Microsoft.Ccr.Adapters.WinForms;
    using Microsoft.Ccr.Core;
    using Microsoft.Dss.Core.Attributes;
    using Microsoft.Dss.Core.DsspHttp;
    using Microsoft.Dss.ServiceModel.Dssp;
    using Microsoft.Dss.ServiceModel.DsspServiceBase;
    using Microsoft.Robotics.Common;
    using Microsoft.Robotics.PhysicalModel;
    using Microsoft.Robotics.Services.Sensors.Kinect;
    using W3C.Soap;
    using depthcam = Microsoft.Robotics.Services.DepthCamSensor;
    using game = Microsoft.Robotics.Services.GameController.Proxy;
    using gendrive = Microsoft.Robotics.Services.Drive.Proxy;
    using infraredsensorarray = Microsoft.Robotics.Services.InfraredSensorArray.Proxy;
    using sonarsensorarray = Microsoft.Robotics.Services.SonarSensorArray.Proxy;
    using submgr = Microsoft.Dss.Services.SubscriptionManager;

    /// <summary>
    /// Obstacle Avoidance Drive Service
    /// </summary>
    [AlternateContract(gendrive.Contract.Identifier)]
    [Contract(Contract.Identifier)]
    [DisplayName("(User) Obstacle Avoidance Drive")]
    [Description("Semi autonomous drive service utilizing a depthcam and proximity sensors for obstacle avoidance and open space explore")]
    public partial class ObstacleAvoidanceDriveService : DsspServiceBase
    {
        /// <summary>
        /// Minimum valid depth reading in millimeters
        /// </summary>
        private const int MinValidDepth = 800;

        /// <summary>
        /// Maximum valid depth reading in millimeters
        /// </summary>
        private const int MaxValidDepth = 4000;

        /// <summary>
        /// This reading has been interpreted as being the floor 
        /// </summary>
        private const int Floor = -2;

        /// <summary>
        /// No reading from teh depth camera
        /// </summary>
        private const int NoReading = -1;

        /// <summary>
        /// Readings less than 800 millimeters
        /// </summary>
        private const int Near = 0;

        /// <summary>
        /// When turning, we want to eliminate drastic differences between wheel power settings.
        /// not doing so may result in unpractically fast rotation that essentially makes robot 
        /// uncontrollable
        /// </summary>
        private const double MaxPowerDifferenceBetweenWheels = 0.3;

        /// <summary>
        /// Interval for depth camera and sensor sampling. Decrease for smoother driving but increased cpu cost
        /// </summary>
        private const double SamplingIntervalInSeconds = 0.075;

        /// <summary>
        /// Preferred depth cam image width
        /// </summary>
        private const int DefaultDepthCamImageWidth = 320;

        /// <summary>
        /// Preferred depth cam image height
        /// </summary>
        private const int DefaultDepthcamImageHeight = 240;

        /// <summary>
        /// Default robot width
        /// </summary>
        private const double DefaultRobotWidthInMeters = 0.45;

        /// <summary>
        /// Default minimum rotation speed
        /// </summary>
        private const double DefaultMinimumRotateInPlaceSpeed = 0.05;

        /// <summary>
        /// Default maximum motor speed
        /// </summary>
        private const double DefaultMaximumMotorSpeed = 0.5;

        /// <summary>
        /// Default maximum change in motor power from one SetPower to another
        /// </summary>
        private const double DefaultMaxDeltaPower = 0.1;

        /// <summary>
        /// Kinect camera has inactive pixel columns at far edge. 
        /// We use this to calculate proper midpoint of horizontal profile
        /// </summary>
        private const int DeadZoneColumnCount =
            (int)(KinectCameraConstants.PercentImageColumnsAtRightEdgeNotActive * (double)DefaultDepthCamImageWidth);

        /// <summary>
        /// The minimum depth in millimeters that is required for space to be considered open.
        /// This affects how soon the robot will begin turning to avoid obstacles.
        /// </summary>
        private int minDepthForOpenSpaceMM = 900;
        
        /// <summary>
        /// The maximum depth, for IR and Sonar sensors to be used in avoidance
        /// We use the Kinect Mimimum Valid Depth so that sensors will only used where Kinect cannot
        /// </summary>
        private double maxDepthForAnalogSensorMeters = (double)MinValidDepth / 1000;

        #region partners

        /// <summary>
        /// Primary contract operations port
        /// </summary>
        [ServicePort("/obstacleavoidancedrive", AllowMultipleInstances = false)]
        private ObstacleAvoidanceDriveOperationsPort mainPort = new ObstacleAvoidanceDriveOperationsPort();

        /// <summary>
        /// Partner service implementing drive contract and actually responsible for moving the robot
        /// </summary>
        [Partner(
            Partners.Drive,
            Optional = false,
            Contract = gendrive.Contract.Identifier,
            CreationPolicy = PartnerCreationPolicy.UsePartnerListEntry)]
        private gendrive.DriveOperations robotDrivePort = new gendrive.DriveOperations();

        /// <summary>
        /// DepthCam sensor port
        /// </summary>
        [Partner(
            Partners.DepthCamSensor,
            Contract = depthcam.Contract.Identifier,
            Optional = false,
            CreationPolicy = PartnerCreationPolicy.UsePartnerListEntry)]
        private depthcam.DepthCamSensorOperationsPort depthCameraPort = new depthcam.DepthCamSensorOperationsPort();

        /// <summary>
        /// IR sensor array port
        /// </summary>
        [Partner(
            Partners.InfraredSensorArray,
            Optional = true,
            Contract = infraredsensorarray.Contract.Identifier, CreationPolicy = PartnerCreationPolicy.UsePartnerListEntry)]
        private infraredsensorarray.InfraredSensorOperations infraredSensorArrayPort = new infraredsensorarray.InfraredSensorOperations();

        /// <summary>
        /// Sonar port
        /// </summary>
        [Partner(
            Partners.SonarSensorArray,
            Optional = true,
            Contract = sonarsensorarray.Contract.Identifier, CreationPolicy = PartnerCreationPolicy.UsePartnerListEntry)]
        private sonarsensorarray.SonarSensorOperations sonarSensorArrayPort = new sonarsensorarray.SonarSensorOperations();

        /// <summary>
        /// Game Controller partner
        /// </summary>
        /// <remarks>Always create one of these, even if there is no Game Controller attached</remarks>
        [Partner("GameController", Contract = game.Contract.Identifier, CreationPolicy = PartnerCreationPolicy.UseExistingOrCreate)]
        private game.GameControllerOperations gameControllerPort = new game.GameControllerOperations();

        /// <summary>
        /// GameController Notifications Port
        /// </summary>
        private game.GameControllerOperations gameControllerNotify = new game.GameControllerOperations();

        /// <summary>
        /// Service state
        /// </summary>
        [InitialStatePartner(Optional = true, ServiceUri = "samples/config/obstacleavoidancedrive.user.config.xml")]
        private ObstacleAvoidanceDriveState state = new ObstacleAvoidanceDriveState();

        /// <summary>
        /// Subscription manager port
        /// </summary>
        [SubscriptionManagerPartner]
        private submgr.SubscriptionManagerPort submgr = new submgr.SubscriptionManagerPort();

        /// <summary>
        /// A handle to the main WinForm UI
        /// </summary>
        private ObstacleAvoidanceForm obstacleAvoidanceForm;

        /// <summary>
        /// Port for the UI to send messages back to here (main service)
        /// </summary>
        private ObstacleAvoidanceFormEvents eventsPort = new ObstacleAvoidanceFormEvents();

        #endregion

        /// <summary>
        /// Gets a value indicating whether we have infrared sensors
        /// </summary>
        private bool HasInfraredSensors
        {
            get
            {
                return this.infraredSensorArrayPort != null;
            }
        }

        /// <summary>
        /// Gets a value indicating whether we have sonar sensors
        /// </summary>
        private bool HasSonarSensors
        {
            get
            {
                return this.sonarSensorArrayPort != null;
            }
        }

        /// <summary>
        /// The drive contract service port
        /// </summary>
        [AlternateServicePort(
            "/genericobstacleavoidancedrive",
            AllowMultipleInstances = false,
            AlternateContract = gendrive.Contract.Identifier)]
        private gendrive.DriveOperations alternateDrivePort = new gendrive.DriveOperations();

        /// <summary>
        /// Initializes a new instance of the ObstacleAvoidanceDriveService class
        /// </summary>
        /// <param name="creationPort">The creation port</param>
        public ObstacleAvoidanceDriveService(DsspServiceCreationPort creationPort)
            : base(creationPort)
        {
            PartnerEnumerationTimeout = TimeSpan.FromSeconds(Partners.PartnerEnumerationTimeoutInSeconds);
        }

        /// <summary>
        /// Start the service
        /// </summary>
        protected override void Start()
        {
            if (this.state == null)
            {
                this.state = new ObstacleAvoidanceDriveState();
            }

            if (this.state.RobotWidth == 0)
            {
                this.state.RobotWidth = DefaultRobotWidthInMeters;
            }

            if (this.state.DepthCameraPosition.Y == 0)
            {
                this.state.DepthCameraPosition = new PhysicalModel.Vector3(0, 0.6f, -0.15f);
            }

            if (this.state.Controller == null)
            {
                this.state.Controller = new PIDController();
            }

            if (this.state.MaxPowerPerWheel <= 0 || this.state.MaxPowerPerWheel > 1)
            {
                this.state.MaxPowerPerWheel = DefaultMaximumMotorSpeed;
            }

            if (this.state.MinRotationSpeed <= 0 || this.state.MinRotationSpeed > this.state.MaxPowerPerWheel)
            {
                this.state.MinRotationSpeed = DefaultMinimumRotateInPlaceSpeed;
            }

            if (this.state.MaxDeltaPower == 0)
            {
                this.state.MaxDeltaPower = DefaultMaxDeltaPower;
            }

            this.depthCameraDefaultPose.Position = this.state.DepthCameraPosition;

            // C:\Microsoft Robotics Dev Studio 4\samples\Config\obstacleavoidancedrive.user.config.xml
            this.SaveState(this.state);
            base.Start();

            // Handlers that need write or exclusive access to state go under
            // the exclusive group. Handlers that need read or shared access, and can be
            // concurrent to other readers, go to the concurrent group.
            // Other internal ports can be included in interleave so you can coordinate
            // intermediate computation with top level handlers.
            MainPortInterleave.CombineWith(
            Arbiter.Interleave(
                new TeardownReceiverGroup(),
                new ExclusiveReceiverGroup(
                    Arbiter.ReceiveWithIterator<OnLoad>(true, this.eventsPort, this.OnLoadHandler),
                    Arbiter.Receive<OnClosed>(true, this.eventsPort, this.OnClosedHandler),
                    Arbiter.ReceiveWithIterator(true, this.samplePort, this.SampleSensors),
                    Arbiter.ReceiveWithIterator<OnPIDChanges>(true, this.eventsPort, this.OnPIDChangesHandler)),
                new ConcurrentReceiverGroup(
                    Arbiter.ReceiveWithIterator<game.UpdateButtons>(true, this.gameControllerNotify, this.JoystickUpdateButtonsHandler))));

            WinFormsServicePort.Post(new RunForm(this.CreateForm));

            // kick off sampling interval
            this.samplePort.Post(DateTime.UtcNow);
        }

        /// <summary>
        /// Inverse projection matrix for viewspace to pixelspace conversions
        /// </summary>
        private PhysicalModel.Matrix inverseProjectionMatrix;

        #region WinForms interaction

        /// <summary>
        /// Create the main Windows Form
        /// </summary>
        /// <returns>A Dashboard Form</returns>
        private System.Windows.Forms.Form CreateForm()
        {
            return new ObstacleAvoidanceForm(this.eventsPort, this.state);
        }

        /// <summary>
        /// Handle the Form Load event for the ObstacleAvoidance Form
        /// </summary>
        /// <param name="onLoad">The load message</param>
        /// <returns>An iterator</returns>
        private IEnumerator<ITask> OnLoadHandler(OnLoad onLoad)
        {
            this.obstacleAvoidanceForm = onLoad.ObstacleAvoidanceForm;

            LogInfo("Loaded Form");

            yield return this.SubscribeToJoystick();
        }

        /// <summary>
        /// Handle the Form Closed event for the Dashboard Form
        /// </summary>
        /// <param name="onClosed">The closed message</param>
        private void OnClosedHandler(OnClosed onClosed)
        {
            if (onClosed.ObstacleAvoidanceForm == this.obstacleAvoidanceForm)
            {
                LogInfo("Form Closed");
            }
        }

        /// <summary>
        /// Subscribe to the Joystick
        /// </summary>
        /// <returns>A Choice</returns>
        private Choice SubscribeToJoystick()
        {
            return Arbiter.Choice(this.gameControllerPort.Subscribe(this.gameControllerNotify), EmptyHandler, LogError);
        }

        /// <summary>
        /// Handle updates to the buttons on the Gamepad
        /// </summary>
        /// <param name="update">The parameter is not used.</param>
        /// <returns>An Iterator</returns>
        private IEnumerator<ITask> JoystickUpdateButtonsHandler(game.UpdateButtons update)
        {
            if (this.obstacleAvoidanceForm != null)
            {
                //WinFormsServicePort.FormInvoke(() => this.obstacleAvoidanceForm.UpdateJoystickButtons(update.Body));
            }

            yield break;
        }

        /// <summary>
        /// Handle PID changes Commands
        /// </summary>
        /// <param name="onPIDChanges">The PID change request</param>
        /// <returns>An Iterator</returns>
        private IEnumerator<ITask> OnPIDChangesHandler(OnPIDChanges onPIDChanges)
        {
            if (onPIDChanges.ObstacleAvoidanceForm == this.obstacleAvoidanceForm)
            {
                // Only update the state here because this is an Exclusive handler
                this.state.Controller.Kp = onPIDChanges.Kp;
                this.state.Controller.Ki = onPIDChanges.Ki;
                this.state.Controller.Kd = onPIDChanges.Kd;

                this.state.Controller.MaxPidValue = onPIDChanges.MaxPidValue;
                this.state.Controller.MinPidValue = onPIDChanges.MinPidValue;
                this.state.Controller.MaxIntegralError = onPIDChanges.MaxIntegralError;

                if (onPIDChanges.DoSaveState)
                {
                    // C:\Microsoft Robotics Dev Studio 4\samples\Config\obstacleavoidancedrive.user.config.xml
                    SaveState(this.state);
                }

                yield break;
            }
        }
        #endregion
        /// <summary>
        /// Initializes depth processing variables
        /// </summary>
        /// <param name="invProjMatrix">Dept camera's inverse projection Matrix</param>
        private void InitializeDepthProcessing(Matrix invProjMatrix)
        {
            this.inverseProjectionMatrix = invProjMatrix;

            // pre calculate pixel space thresholds for floor and ceiling removal. We allow for
            // a small (10cm) margin on floor detection, and we cut-off the ceiling 2m above the camera
            DepthImageUtilities.ComputeCeilingAndFloorDepthsInPixelSpace(
                cameraPose: this.depthCameraDefaultPose,
                floorThreshold: -0.1f,
                ceilingThreshold: 2f,
                invProjectionMatrix: this.inverseProjectionMatrix,
                floorCeilingMinDepths: ref this.floorCeilingMinDepths,
                floorCeilingMaxDepths: ref this.floorCeilingMaxDepths,
                width: DefaultDepthCamImageWidth,
                height: DefaultDepthcamImageHeight,
                depthRange: (float)KinectCameraConstants.MaximumRangeMeters);
        }

        /// <summary>
        /// Handles enable operations
        /// </summary>
        /// <param name="enable">Enable operation</param>
        /// <returns>CCR ITask enumerator</returns>
        [ServiceHandler(PortFieldName = "alternateDrivePort")]
        public IEnumerator<ITask> HandleEnable(gendrive.EnableDrive enable)
        {
            this.pendingSetDrivePower = null;
            this.state.Controller.Reset();

            var responsePort = this.robotDrivePort.EnableDrive(enable.Body.Enable);
            yield return responsePort.Choice();

            Fault f = responsePort;
            if (f != null)
            {
                enable.ResponsePort.Post(f);
                yield break;
            }

            enable.ResponsePort.Post(DefaultUpdateResponseType.Instance);
            this.SendNotification(this.submgr, enable);
        }

        /// <summary>
        /// Handles get operation
        /// </summary>
        /// <param name="get">Get operation</param>
        /// <returns>CCR ITask enumerator</returns>
        [ServiceHandler(PortFieldName = "alternateDrivePort")]
        public IEnumerator<ITask> HandleGet(gendrive.Get get)
        {
            var responsePort = this.robotDrivePort.Get();
            yield return responsePort.Choice();

            Fault f = responsePort;
            if (f != null)
            {
                get.ResponsePort.Post(f);
                yield break;
            }

            // return the state of the underlying drive.
            gendrive.DriveDifferentialTwoWheelState s = responsePort;
            get.ResponsePort.Post(s);
        }

        /// <summary>
        /// Http Get handler on Drive port
        /// </summary>
        /// <param name="get">HttpGet operation</param>
        /// <returns>CCR ITask enumerator</returns>
        [ServiceHandler(PortFieldName = "alternateDrivePort")]
        public IEnumerator<ITask> HandleDriveHttpGet(HttpGet get)
        {
            var responsePort = this.robotDrivePort.Get();
            yield return responsePort.Choice();

            Fault f = responsePort;
            if (f != null)
            {
                get.ResponsePort.Post(f);
                yield break;
            }

            gendrive.DriveDifferentialTwoWheelState s = responsePort;
            get.ResponsePort.Post(new HttpResponseType(s));
            yield break;
        }

        /// <summary>
        /// Handles subscribe operation
        /// </summary>
        /// <param name="subscribe">Subscribe operation</param>
        [ServiceHandler(PortFieldName = "alternateDrivePort")]
        public void HandleSubscribe(gendrive.Subscribe subscribe)
        {
            this.SubscribeHelper(this.submgr, subscribe, subscribe.ResponsePort);
        }

        /// <summary>
        /// Handles reliable subscribe operation
        /// </summary>
        /// <param name="subscribe">Subscribe operation</param>
        [ServiceHandler(PortFieldName = "alternateDrivePort")]
        public void HandleReliableSubscribe(gendrive.ReliableSubscribe subscribe)
        {
            this.SubscribeHelper(this.submgr, subscribe, subscribe.ResponsePort);
        }

        /// <summary>
        /// Pending power request
        /// </summary>
        private gendrive.SetDrivePowerRequest pendingSetDrivePower;

        /// <summary>
        /// Handles setPower operation
        /// </summary>
        /// <param name="setPower">SetPower operation</param>
        [ServiceHandler(PortFieldName = "alternateDrivePort", QueueDepthLimit = 1)]
        public void HandleSetPower(gendrive.SetDrivePower setPower)
        {
            // If either wheel power is greater than the max power per wheel
            // scale both wheels down so that the higher of them is equal to max power
            double higherPower = Math.Max(
                Math.Abs(setPower.Body.LeftWheelPower),
                Math.Abs(setPower.Body.RightWheelPower));
            if (higherPower > this.state.MaxPowerPerWheel)
            {
                double scalingFactor = higherPower / this.state.MaxPowerPerWheel;
                setPower.Body.LeftWheelPower /= scalingFactor;
                setPower.Body.RightWheelPower /= scalingFactor;
            }

            this.NormalizeRotationSpeed(setPower);

            this.pendingSetDrivePower = setPower.Body;

            if (this.pendingSetDrivePower.LeftWheelPower == 0 &&
                this.pendingSetDrivePower.RightWheelPower == 0)
            {
                this.SetPowerWithAcceleration(0, 0);
                this.pendingSetDrivePower = null;
            }

            // simple cache power request. We will apply proper control and do sensor fusion
            // for obstacle avoidance, as part of our sampling loop
            setPower.ResponsePort.Post(DefaultUpdateResponseType.Instance);
        }

        /// <summary>
        /// Ensures that rotation speed can not exceed a preset max value
        /// </summary>
        /// <param name="setPower">Drive Power object </param>
        private void NormalizeRotationSpeed(gendrive.SetDrivePower setPower)
        {
            double currentPowerDifference = Math.Max(setPower.Body.LeftWheelPower, setPower.Body.RightWheelPower) -
                Math.Min(setPower.Body.LeftWheelPower, setPower.Body.RightWheelPower);

            if (MaxPowerDifferenceBetweenWheels < currentPowerDifference)
            {
                double scalingFactor = currentPowerDifference / MaxPowerDifferenceBetweenWheels;
                setPower.Body.LeftWheelPower /= scalingFactor;
                setPower.Body.RightWheelPower /= scalingFactor;
            }
        }

        /// <summary>
        /// Handles SetSpeed operation, which is not implemented in this service
        /// </summary>
        /// <param name="setSpeed">SetSpeed operation</param>
        [ServiceHandler(PortFieldName = "alternateDrivePort", QueueDepthLimit = 1)]
        public void HandleSetSpeed(gendrive.SetDriveSpeed setSpeed)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Resets the encoders, not implemented in this service
        /// </summary>
        /// <param name="reset">Request message</param>
        [ServiceHandler(PortFieldName = "alternateDrivePort")]
        public void HandleEncoderReset(gendrive.ResetEncoders reset)
        {
            this.robotDrivePort.Post(new gendrive.ResetEncoders { ResponsePort = reset.ResponsePort });
        }

        /// <summary>
        /// Handles the AllStop message.
        /// </summary>
        /// <param name="allStop">The message.</param>
        [ServiceHandler(PortFieldName = "alternateDrivePort")]
        public void HandleAllStop(gendrive.AllStop allStop)
        {
            this.robotDrivePort.Post(new gendrive.AllStop { ResponsePort = allStop.ResponsePort });
        }

        /// <summary>
        /// Handles the RotateDegrees message.
        /// </summary>
        /// <param name="rotate">The message.</param>
        [ServiceHandler(PortFieldName = "alternateDrivePort")]
        public void HandleRotateDegrees(gendrive.RotateDegrees rotate)
        {
            this.robotDrivePort.Post(new gendrive.RotateDegrees { ResponsePort = rotate.ResponsePort });
        }

        /// <summary>
        /// Handles the DriveDistance message.
        /// </summary>
        /// <param name="drive">The message.</param>
        [ServiceHandler(PortFieldName = "alternateDrivePort")]
        public void HandleDriveDistance(gendrive.DriveDistance drive)
        {
            drive.ResponsePort.Post(Fault.FromCodeSubcode(FaultCodes.Receiver, DsspFaultCodes.MessageNotSupported));
        }

        /// <summary>
        /// Http query handler
        /// </summary>
        /// <param name="query">Query operation</param>
        /// <returns>CCR ITask enumerator</returns>
        [ServiceHandler]
        public IEnumerator<ITask> HandleHttpQuery(HttpQuery query)
        {
            return this.HttpHandler(query.Body.Context, query.ResponsePort);
        }

        /// <summary>
        /// Http get handler
        /// </summary>
        /// <param name="get">Get operation</param>
        /// <returns>CCR ITask enumerator</returns>
        [ServiceHandler]
        public IEnumerator<ITask> HandleHttpGet(HttpGet get)
        {
            get.ResponsePort.Post(new HttpResponseType(this.state));
            yield break;
        }

        /// <summary>
        /// Handles http requests
        /// </summary>
        /// <param name="context">Http context</param>
        /// <param name="responsePort">Response port</param>
        /// <returns>CCR ITask enumerator</returns>
        private IEnumerator<ITask> HttpHandler(HttpListenerContext context, PortSet<HttpResponseType, Fault> responsePort)
        {
            HttpListenerRequest request = context.Request;
            HttpListenerResponse response = context.Response;

            string type = "image/png";
            ImageFormat format = ImageFormat.Png;

            if (request.Url.Query.ToLowerInvariant().EndsWith("depth"))
            {
                Bitmap image = null;

                if (!this.TryCreateDepthImage(out image))
                {
                    responsePort.Post(Fault.FromException(new InvalidOperationException()));
                    yield break;
                }

                using (image)
                {
                    using (var memStream = new MemoryStream())
                    {
                        image.Save(memStream, ImageFormat.Png);
                        response.AddHeader("Cache-Control", "No-cache");
                        response.ContentType = type;
                        memStream.WriteTo(response.OutputStream);
                    }
                }

                Microsoft.Dss.Core.DsspHttpUtilities.Utilities.HttpClose(context);
            }
            else if (request.Url.Query.ToLowerInvariant().EndsWith("floor"))
            {
                // show the pre-calculated floor/ceiling map
                if (this.floorCeilingMaxDepths == null)
                {
                    responsePort.Post(Fault.FromException(new InvalidOperationException()));
                    yield break;
                }

                // insert the depth image bits
                var imageBits = new byte[DefaultDepthCamImageWidth * DefaultDepthcamImageHeight * 3];
                for (int i = 0; i < this.depthDownSampleBuffer.Length; i++)
                {
                    var depth8bpp = (this.floorCeilingMaxDepths[i] * 255) / (int)MaxValidDepth;
                    imageBits[(i * 3) + 0] = (byte)depth8bpp;
                    imageBits[(i * 3) + 1] = (byte)depth8bpp;
                    imageBits[(i * 3) + 2] = (byte)depth8bpp;
                }

                using (var image = new Bitmap(DefaultDepthCamImageWidth, DefaultDepthcamImageHeight, PixelFormat.Format24bppRgb))
                {
                    var raw = image.LockBits(
                                new Rectangle(0, 0, DefaultDepthCamImageWidth, DefaultDepthcamImageHeight),
                                ImageLockMode.WriteOnly,
                                PixelFormat.Format24bppRgb);
                    System.Runtime.InteropServices.Marshal.Copy(imageBits, 0, raw.Scan0, imageBits.Length);
                    image.UnlockBits(raw);
                    using (var memStream = new MemoryStream())
                    {
                        image.Save(memStream, ImageFormat.Png);
                        response.AddHeader("Cache-Control", "No-cache");
                        response.ContentType = type;
                        memStream.WriteTo(response.OutputStream);
                    }
                }

                Microsoft.Dss.Core.DsspHttpUtilities.Utilities.HttpClose(context);
            }
            else
            {
                responsePort.Post(Fault.FromException(new NotSupportedException()));
                yield break;
            }

            response.Close();
        }

        /// <summary>
        /// Create depth image representation for visual
        /// </summary>
        /// <param name="image">DepthImage outcome</param>
        /// <returns>Boolean indicating success or failure to create the image</returns>
        private bool TryCreateDepthImage(out Bitmap image)
        {
            image = null;

            if (this.depthDownSampleBuffer == null)
            {
                return false;
            }

            // Create the depth image with depth profile and sensor profile at the bottom
            // Convert data to 24bpp RGB stream so we can show differnt depth profiles in different colors.
            const int ProfileHeight = 30;

            // insert the depth image bits
            var imageBits = new byte[DefaultDepthCamImageWidth * (DefaultDepthcamImageHeight + ProfileHeight + ProfileHeight + ProfileHeight) * 3];
            for (int i = 0; i < this.depthDownSampleBuffer.Length; i++)
            {
                if (this.depthDownSampleBuffer[i] == (short)NoReading)
                {
                    // Dark red for "no reading" values.
                    imageBits[(i * 3) + 0] = (byte)60;
                    imageBits[(i * 3) + 1] = (byte)60;
                    imageBits[(i * 3) + 2] = (byte)100;
                }
                else
                {
                    var depth8bpp = (this.depthDownSampleBuffer[i] * 255) / (int)MaxValidDepth;
                    imageBits[(i * 3) + 0] = (byte)depth8bpp;
                    imageBits[(i * 3) + 1] = (byte)depth8bpp;
                    imageBits[(i * 3) + 2] = (byte)depth8bpp;
                }
            }

            // Insert the depth profile bits (tinted blue)
            for (int i = DefaultDepthcamImageHeight; i < DefaultDepthcamImageHeight + ProfileHeight; i++)
            {
                for (int j = 0; j < DefaultDepthCamImageWidth; j++)
                {
                    var depth8bpp = (this.depthProfile[j] * 255) / (int)MaxValidDepth;
                    imageBits[(i * DefaultDepthCamImageWidth * 3) + ((j * 3) + 0)] = 255;
                    imageBits[(i * DefaultDepthCamImageWidth * 3) + ((j * 3) + 1)] = (byte)depth8bpp;
                    imageBits[(i * DefaultDepthCamImageWidth * 3) + ((j * 3) + 2)] = (byte)depth8bpp;
                }
            }

            // Insert the ir sensor profile bits tinted red
            for (int i = DefaultDepthcamImageHeight + ProfileHeight; i < DefaultDepthcamImageHeight + ProfileHeight + ProfileHeight; i++)
            {
                for (int j = 0; j < DefaultDepthCamImageWidth; j++)
                {
                    if (this.irSensorProfile == null || this.irSensorProfile[j] == 0)
                    {
                        imageBits[(i * DefaultDepthCamImageWidth * 3) + ((j * 3) + 0)] = 255;
                        imageBits[(i * DefaultDepthCamImageWidth * 3) + ((j * 3) + 1)] = 255;
                        imageBits[(i * DefaultDepthCamImageWidth * 3) + ((j * 3) + 2)] = 255;
                    }
                    else
                    {
                        var depth8bpp = (this.irSensorProfile[j] * 255) / (int)(int)MaxValidDepth;
                        imageBits[(i * DefaultDepthCamImageWidth * 3) + ((j * 3) + 0)] = (byte)depth8bpp;
                        imageBits[(i * DefaultDepthCamImageWidth * 3) + ((j * 3) + 1)] = (byte)depth8bpp;
                        imageBits[(i * DefaultDepthCamImageWidth * 3) + ((j * 3) + 2)] = 255;
                    }
                }
            }

            // Insert the sonar sensor profile bits tinted green
            for (int i = DefaultDepthcamImageHeight + ProfileHeight + ProfileHeight; i < DefaultDepthcamImageHeight + ProfileHeight + ProfileHeight + ProfileHeight; i++)
            {
                for (int j = 0; j < DefaultDepthCamImageWidth; j++)
                {
                    if (this.sonarSensorProfile == null || this.sonarSensorProfile[j] == 0)
                    {
                        imageBits[(i * DefaultDepthCamImageWidth * 3) + ((j * 3) + 0)] = 255;
                        imageBits[(i * DefaultDepthCamImageWidth * 3) + ((j * 3) + 1)] = 255;
                        imageBits[(i * DefaultDepthCamImageWidth * 3) + ((j * 3) + 2)] = 255;
                    }
                    else
                    {
                        var depth8bpp = (this.sonarSensorProfile[j] * 255) / (int)(int)MaxValidDepth;
                        imageBits[(i * DefaultDepthCamImageWidth * 3) + ((j * 3) + 0)] = (byte)depth8bpp;
                        imageBits[(i * DefaultDepthCamImageWidth * 3) + ((j * 3) + 1)] = 255;
                        imageBits[(i * DefaultDepthCamImageWidth * 3) + ((j * 3) + 2)] = (byte)depth8bpp;
                    }
                }
            }

            image = new Bitmap(DefaultDepthCamImageWidth, DefaultDepthcamImageHeight + ProfileHeight + ProfileHeight + ProfileHeight, PixelFormat.Format24bppRgb);
            var raw = image.LockBits(
                        new Rectangle(0, 0, DefaultDepthCamImageWidth, DefaultDepthcamImageHeight + ProfileHeight + ProfileHeight + ProfileHeight),
                        ImageLockMode.WriteOnly,
                        PixelFormat.Format24bppRgb);
            System.Runtime.InteropServices.Marshal.Copy(imageBits, 0, raw.Scan0, imageBits.Length);
            image.UnlockBits(raw);

            return true;
        }

        /// <summary>
        /// Pre calculated depths to floor plane
        /// </summary>
        private short[] floorCeilingMinDepths;

        /// <summary>
        /// Pre calculated depths to ceiling plane
        /// </summary>
        private short[] floorCeilingMaxDepths;

        /// <summary>
        /// Default pose for depthcam sensor
        /// </summary>
        private PhysicalModel.Pose depthCameraDefaultPose = new PhysicalModel.Pose()
        {
            Orientation = new PhysicalModel.Quaternion(0, 0, 0, 1)
        };

        /// <summary>
        /// Port used to periodically sample sensors
        /// </summary>
        private Port<DateTime> samplePort = new Port<DateTime>();

        /// <summary>
        /// Samples sensors used for obstacle avoidance
        /// </summary>
        /// <param name="timestamp">Sampling timestamp</param>
        /// <returns>CCR ITask enumerator</returns>
        private IEnumerator<ITask> SampleSensors(DateTime timestamp)
        {
            var startTime = Utilities.ElapsedSecondsSinceStart;
            try
            {
                if (!this.CheckIfValidPendingDriveRequest(this.pendingSetDrivePower))
                {
                    yield break;
                }

                // If this is a command to move in reverse, then execute it
                // and skip obstacle avoidance
                if (this.pendingSetDrivePower.LeftWheelPower < 0 && this.pendingSetDrivePower.RightWheelPower < 0)
                {
                    this.SetPowerWithAcceleration(this.pendingSetDrivePower.LeftWheelPower, this.pendingSetDrivePower.RightWheelPower);
                    yield break;
                }

                PortSet<infraredsensorarray.InfraredSensorArrayState, Fault> infraredGetResponsePort = null;
                PortSet<sonarsensorarray.SonarSensorArrayState, Fault> sonarGetResponsePort = null;

                // sample proximity depth cam sensor in paraller
                // Sample depthcam. Set timeout so we dont wait forever
                var getDepthCamState = new depthcam.Get();
                getDepthCamState.TimeSpan = TimeSpan.FromSeconds(SamplingIntervalInSeconds * 5);
                this.depthCameraPort.Post(getDepthCamState);

                if (this.HasInfraredSensors)
                {
                    // issue ir sensor array get
                    infraredGetResponsePort = this.infraredSensorArrayPort.Get();
                }

                if (this.HasSonarSensors)
                {
                    // issue sonar sensor array get
                    sonarGetResponsePort = this.sonarSensorArrayPort.Get();
                }

                yield return getDepthCamState.ResponsePort.Choice();
                depthcam.DepthCamSensorState depthCamState = getDepthCamState.ResponsePort;
                if (depthCamState == null)
                {
                    LogError((Fault)getDepthCamState.ResponsePort);
                    yield break;
                }

                if (this.floorCeilingMinDepths == null)
                {
                    this.InitializeDepthProcessing(depthCamState.InverseProjectionMatrix);
                }

                var depthImage = depthCamState.DepthImage;
                if (depthCamState.DepthImageSize.Width > DefaultDepthCamImageWidth ||
                    depthCamState.DepthImageSize.Height > DefaultDepthcamImageHeight)
                {
                    depthImage = this.DownSampleDepthImage(depthCamState);
                }
                else
                {
                    depthImage = depthCamState.DepthImage.Clone() as short[];
                }

                // remove floor and ceiling
                int floorPixelCount;
                DepthImageUtilities.FilterOutGroundAndAboveRobotValues(
                    floorHoleAsObstacleDepthThresholdAlongDepthAxisInMillimeters: 5000,
                    floorDetectionMarginInMillimeters: 200,
                    preFilterNoReadingDepthValue: (int)NoReading,
                    postFilterNoReadingDepthValue: (int)MaxValidDepth,
                    floorDepthValue: (int)Floor,
                    minValidDepthMillimeters: (short)MinValidDepth,
                    maxValidDepthMillimeters: (short)MaxValidDepth,
                    depthData: depthImage,
                    floorCeilingDepths: this.floorCeilingMaxDepths,
                    numberOfFloorPixels: out floorPixelCount);

                var minDepthProfile = DepthImageUtilities.CalculateHorizontalDepthProfileAsShortArray(
                    depthImage,
                    DefaultDepthCamImageWidth,
                    DefaultDepthcamImageHeight,
                    (short)NoReading,
                    (short)Floor,
                    DeadZoneColumnCount,
                    (int)MinValidDepth,
                    (int)MaxValidDepth,
                    DefaultDepthCamImageWidth);

                // Cache the minDepthProfile before ir sensor fusion so we can track changes made via sensor fusion.  
                // The cached profile can be visualized via the HttpQuery handler to see how sensors are contributing to the depth profile
                short[] temp = minDepthProfile.Clone() as short[];
                this.irSensorProfile = new short[minDepthProfile.Length];
                if (this.HasInfraredSensors)
                {
                    yield return infraredGetResponsePort.Choice();
                    infraredsensorarray.InfraredSensorArrayState infraredState = infraredGetResponsePort;

                    // A fault during the Get will cause this to be null
                    if (infraredState != null)
                    {
                        this.FuseDepthProfileWithIrReadings(depthCamState.FieldOfView, infraredState, minDepthProfile);

                        for (int i = 0; i < minDepthProfile.Length; i++)
                        {
                            if (minDepthProfile[i] != temp[i])
                            {
                                this.irSensorProfile[i] = minDepthProfile[i];
                            }
                        }
                    }
                }

                // cache the minDepthProfile before sonar sensor fusion so we can track changes made via sensor fusion.
                // The cached profile can be visualized via the HttpQuery handler to see how sensors are contributing to the depth profile
                temp = minDepthProfile.Clone() as short[];
                this.sonarSensorProfile = new short[minDepthProfile.Length];
                if (this.HasSonarSensors)
                {
                    yield return sonarGetResponsePort.Choice();
                    sonarsensorarray.SonarSensorArrayState sonarState = sonarGetResponsePort;

                    // A fault during the Get will cause this to be null
                    if (sonarState != null)
                    {
                        this.FuseDepthProfileWithSonarReadings(depthCamState.FieldOfView, sonarState, minDepthProfile);
                        for (int i = 0; i < minDepthProfile.Length; i++)
                        {
                            if (minDepthProfile[i] != temp[i])
                            {
                                this.sonarSensorProfile[i] = minDepthProfile[i];
                            }
                        }
                    }
                }

                this.depthProfile = minDepthProfile.Clone() as short[];
                this.depthDownSampleBuffer = depthImage.Clone() as short[];

                // Asynchronously update depth profile image
                Spawn(this.UpdateDepthProfileImage);

                this.AvoidObstacles(minDepthProfile, depthCamState.MaximumRange);
            }
            finally
            {
                var endTime = Utilities.ElapsedSecondsSinceStart;
                var elapsed = endTime - startTime;
                var remainderToNextSamplingTime = SamplingIntervalInSeconds - elapsed;
                if (remainderToNextSamplingTime <= 0)
                {
                    // schedule immediately
                    remainderToNextSamplingTime = 0.015;
                }

                if (this.ServicePhase == ServiceRuntimePhase.Started)
                {
                    // schedule next sampling interval
                    Activate(this.TimeoutPort(
                        TimeSpan.FromSeconds(remainderToNextSamplingTime)).Receive(
                        (dt) => this.samplePort.Post(dt)));
                }
            }
        }

        /// <summary>
        /// Set Drive Power while honoring a maximum power delta setting
        /// </summary>
        /// <param name="leftPower">Requested power to the left motor</param>
        /// <param name="rightPower">Requested power to the right motor</param>
        private void SetPowerWithAcceleration(double leftPower, double rightPower)
        {
            double nextLeftPower = leftPower;
            double nextRightPower = rightPower;

            if (leftPower == 0 && rightPower == 0)
            {
                // Ignore max delta if request is all-stop
                nextLeftPower = 0;
                nextRightPower = 0;
            }
            else
            {
                if (Math.Abs(leftPower - this.lastLeftPower) > this.state.MaxDeltaPower)
                {
                    nextLeftPower = this.lastLeftPower + (Math.Sign(leftPower - this.lastLeftPower) * this.state.MaxDeltaPower);
                }

                if (Math.Abs(rightPower - this.lastRightPower) > this.state.MaxDeltaPower)
                {
                    nextRightPower = this.lastRightPower + (Math.Sign(rightPower - this.lastRightPower) * this.state.MaxDeltaPower);
                }
            }

            var responsePort = this.robotDrivePort.SetDrivePower(nextLeftPower, nextRightPower);
            Activate(responsePort.Choice(
                success =>
                {
                    this.lastLeftPower = nextLeftPower;
                    this.lastRightPower = nextRightPower;
                },
                fault =>
                {
                    LogError(fault);
                }));
        }

        /// <summary>
        /// Update depth profile image on form
        /// </summary>
        private void UpdateDepthProfileImage()
        {
            // Update depth profile image
            Bitmap image = null;

            if (!this.TryCreateDepthImage(out image))
            {
                throw new InvalidOperationException("Unable to create depth profile image since depth sample is not available");
            }

            var setImage = new FormInvoke(() => this.obstacleAvoidanceForm.DepthProfileImage = image);

            WinFormsServicePort.Post(setImage);

            Arbiter.Choice(setImage.ResultPort, EmptyHandler, e => LogError(null, "Unable to set depth profile image on form", Fault.FromException(e)));
        }

        /// <summary>
        /// Combines IR readings with depth camera horizontal min depth profile
        /// </summary>
        /// <param name="depthCamFov">Depth cam field of view</param>
        /// <param name="infraredState">Infrared sensor array state</param>
        /// <param name="minDepthProfile">Depth cam min depth profile</param>
        private void FuseDepthProfileWithIrReadings(
            double depthCamFov,
            infraredsensorarray.InfraredSensorArrayState infraredState,
            short[] minDepthProfile)
        {
            double[] sensorOrientations = new double[3];
            double[] sensorReadings = new double[3];

            for (int i = 0; i < infraredState.Sensors.Count; i++)
            {
                if (i >= sensorOrientations.Length)
                {
                    break;
                }

                var aa = PhysicalModel.Quaternion.ToAxisAngle(
                    new PhysicalModel.Quaternion(
                        infraredState.Sensors[i].Pose.Orientation.X,
                        infraredState.Sensors[i].Pose.Orientation.Y,
                        infraredState.Sensors[i].Pose.Orientation.Z,
                        infraredState.Sensors[i].Pose.Orientation.W));
                double orientationOfIrSensor = aa.Angle * Math.Sign(aa.Axis.Y);

                // Calculate the depth offset that must be applied to sensor data to account 
                // for the position of the sensor relative to the depth camera
                double foresetOfSensorToDepthCamera = (this.state.RobotWidth * 0.5) - this.state.DepthCameraPosition.Z;
                sensorOrientations[i] = orientationOfIrSensor;

                // If the sensor does not return a valid value, or the sensor returns a distance within
                // the depth cam sensing range, we want to favor the depth camera
                sensorReadings[i] = infraredState.Sensors[i].DistanceMeasurement;
                if (sensorReadings[i] == 0 || sensorReadings[i] > this.maxDepthForAnalogSensorMeters)
                {
                    sensorReadings[i] = (double)MaxValidDepth / 1000;
                }
                else
                {
                    sensorReadings[i] += foresetOfSensorToDepthCamera;
                }
            }

            DepthImageUtilities.FuseDepthProfilesWithIrReadings(
                depthCamFov,
                (int)MinValidDepth,
                minDepthProfile,
                sensorOrientations,
                sensorReadings,
                0,
                3);
        }

        /// <summary>
        /// Combines Sonar readings with depth camera horizontal min depth profile
        /// </summary>
        /// <param name="depthCamFov">Depth cam field of view</param>
        /// <param name="sonarState">Sonar sensor array state</param>
        /// <param name="minDepthProfile">Depth cam min depth profile</param>
        private void FuseDepthProfileWithSonarReadings(
            double depthCamFov,
            sonarsensorarray.SonarSensorArrayState sonarState,
            short[] minDepthProfile)
        {
            double[] sensorOrientations = new double[sonarState.Sensors.Count];
            double[] sensorReadings = new double[sonarState.Sensors.Count];

            for (int i = 0; i < sonarState.Sensors.Count; i++)
            {
                if (i >= sensorOrientations.Length)
                {
                    break;
                }

                var aa = PhysicalModel.Quaternion.ToAxisAngle(
                    new PhysicalModel.Quaternion(
                        sonarState.Sensors[i].Pose.Orientation.X,
                        sonarState.Sensors[i].Pose.Orientation.Y,
                        sonarState.Sensors[i].Pose.Orientation.Z,
                        sonarState.Sensors[i].Pose.Orientation.W));
                double orientationOfSonarSensor = aa.Angle * Math.Sign(aa.Axis.Y);

                // Calculate the depth offset that must be applied to sensor data to account 
                // for the position of the sensor relative to the depth camera
                double foresetOfSensorToDepthCamera = (this.state.RobotWidth * 0.5) - this.state.DepthCameraPosition.Z;
                sensorOrientations[i] = orientationOfSonarSensor;

                // If the sensor does not return a valid value, or the sensor returns a distance within
                // the depth cam sensing range, we want to favor the depth camera
                sensorReadings[i] = sonarState.Sensors[i].DistanceMeasurement;
                if (sensorReadings[i] == 0 || sensorReadings[i] > this.maxDepthForAnalogSensorMeters)
                {
                    sensorReadings[i] = (double)MaxValidDepth / 1000;
                }
                else
                {
                    sensorReadings[i] += foresetOfSensorToDepthCamera;
                }
            }

            DepthImageUtilities.FuseDepthProfileWithSonarReadings(
                (int)MinValidDepth,
                minDepthProfile,
                minDepthProfile.Length / 3,
                sensorReadings[0],
                minDepthProfile.Length - (minDepthProfile.Length / 3),
                sensorReadings[1]);
        }

        /// <summary>
        /// Last controller update in seconds
        /// </summary>
        private double lastControllerUpdate;

        /// <summary>
        /// Uses min depth profile to calculate drive control signal 
        /// for open space and obstacle avoidance
        /// </summary>
        /// <param name="minDepthProfile">Depth profile</param>
        /// <param name="maxDepthInMeters">Max depth range</param>
        private void AvoidObstacles(short[] minDepthProfile, double maxDepthInMeters)
        {
            if (!this.CheckIfValidPendingDriveRequest(this.pendingSetDrivePower))
            {
                return;
            }

            var currentTimeInSeconds = Utilities.ElapsedSecondsSinceStart;
            var elapsed = currentTimeInSeconds - this.lastControllerUpdate;
            this.lastControllerUpdate = currentTimeInSeconds;

            float smallestProjectedWidthSquared = 0;
            int bestStartIndex = 0;
            int avgDepthOfOpening = 0;

            double nearObstacleIndex = 0;
            double nearObstacleCount = 0;
            int bestWidthInPixels = 0;
            var invPrjMatrix = this.inverseProjectionMatrix;
            DepthImageUtilities.CalculateWidthOfOpenSpaceFromDepthHorizontalProfile(
                DefaultDepthCamImageWidth,
                DefaultDepthcamImageHeight,
                DeadZoneColumnCount,
                ref invPrjMatrix,
                minDepthProfile,
                DefaultDepthcamImageHeight / 2,
                (float)(this.state.RobotWidth * this.state.RobotWidth),
                this.minDepthForOpenSpaceMM,
                out smallestProjectedWidthSquared,
                out bestStartIndex,
                out avgDepthOfOpening,
                out nearObstacleIndex,
                out nearObstacleCount,
                out bestWidthInPixels);

            this.FusePendingDriveCommandWithOpenSpaceControl(
                maxDepthInMeters,
                minDepthProfile,
                elapsed,
                smallestProjectedWidthSquared,
                bestStartIndex,
                avgDepthOfOpening,
                nearObstacleIndex,
                nearObstacleCount,
                bestWidthInPixels);
        }

        /// <summary>
        /// Checks if autonomous obstacle avoidance should proceed
        /// </summary>
        /// <param name="pendingOperation">Pending drive command</param>
        /// <returns>False if no pending drive request or last request was for stop motion</returns>
        private bool CheckIfValidPendingDriveRequest(gendrive.SetDrivePowerRequest pendingOperation)
        {
            if (pendingOperation == null ||
                (pendingOperation.LeftWheelPower == 0 &&
                 pendingOperation.RightWheelPower == 0))
            {
                // nothing to do. No motion
                this.state.Controller.Reset();
                return false;
            }

            return true;
        }

        /// <summary>
        /// Creates a common drive command fusing pending user suggestion for motion, with control signal
        /// calculated for midpoint of open space and away from obstacles
        /// </summary>
        /// <param name="maxRangeInMeters">Max range in meters</param>
        /// <param name="minDepthProfile">Depth profile</param>
        /// <param name="elapsedTime">Elapsed time since last control update</param>
        /// <param name="smallestProjectedWidthSquared">Smallest width squared</param>
        /// <param name="bestStartIndex">Best open space candidate index in horizontal profile</param>
        /// <param name="avgDepthOfOpening">Average depth within open space</param>
        /// <param name="nearObstacleIndex">Horizontal profile start index of nearest obstacle</param>
        /// <param name="nearObstacleCount">Number of columns classifed as obstacle</param>
        /// <param name="bestWidthInPixelColumns">Best width of open space in column count</param>
        private void FusePendingDriveCommandWithOpenSpaceControl(
            double maxRangeInMeters,
            short[] minDepthProfile,
            double elapsedTime,
            float smallestProjectedWidthSquared,
            int bestStartIndex,
            int avgDepthOfOpening,
            double nearObstacleIndex,
            double nearObstacleCount,
            int bestWidthInPixelColumns)
        {
            var relativeLeftRightLocationOfNearestObstacle = 0.0;
            var relativeObstacleSize = 0.0;

            // depth image in kinect has 4 rightmost pixel columns returning zeros
            const int DepthImageDeadColumnsCount = 4;

            if (nearObstacleCount > 0)
            {
                relativeLeftRightLocationOfNearestObstacle = (nearObstacleIndex / nearObstacleCount) / (double)minDepthProfile.Length;
                relativeObstacleSize = nearObstacleCount / (double)minDepthProfile.Length;
            }
            else
            {
                relativeLeftRightLocationOfNearestObstacle = 0;
                relativeObstacleSize = 0;
            }

            var opening = Math.Sqrt(smallestProjectedWidthSquared);

            var invHorizontalDepthProfileLength = 1.0 / (double)(minDepthProfile.Length - DepthImageDeadColumnsCount);

            var actualWidthOfOpenSpaceCorridor = opening;
            var normalizedWidthOfOpenSpace = bestWidthInPixelColumns * invHorizontalDepthProfileLength;

            var normalizedMidpointOfOpenSpaceCorridor =
                ((double)(bestStartIndex + (bestWidthInPixelColumns / 2))) * invHorizontalDepthProfileLength;

            bool isPathBlocked = false;

            // Set a flag indicating we see no open space
            if (actualWidthOfOpenSpaceCorridor <= this.state.RobotWidth)
            {
                isPathBlocked = true;
            }
            else
            {
                isPathBlocked = false;
            }

            double centerOfOpeningAsControllerError = normalizedMidpointOfOpenSpaceCorridor - 0.5;

            if (relativeLeftRightLocationOfNearestObstacle > 0)
            {
                var obstacleOffsetWithZeroAtCenter = relativeLeftRightLocationOfNearestObstacle - 0.5;

                // if we are close to an obstacle bias center of opening towards the opposite side
                centerOfOpeningAsControllerError = (-obstacleOffsetWithZeroAtCenter + centerOfOpeningAsControllerError) / 2;
            }

            this.state.Controller.Update(centerOfOpeningAsControllerError, elapsedTime);

            var pendingRequest = this.pendingSetDrivePower;
            if (!this.CheckIfValidPendingDriveRequest(pendingRequest))
            {
                return;
            }

            double angularSpeedSuggestion = pendingRequest.LeftWheelPower -
                pendingRequest.RightWheelPower;
            double linearSpeedSuggestion = (pendingRequest.LeftWheelPower +
                pendingRequest.RightWheelPower) / 2;

            double angularSpeed, linearSpeed;
            this.CalculateControlWithSuggestedSpeeds(
                linearSpeedSuggestion,
                angularSpeedSuggestion,
                normalizedWidthOfOpenSpace,
                relativeObstacleSize,
                isPathBlocked,
                out angularSpeed,
                out linearSpeed);

            // convert back to left/right power
            var leftPower = angularSpeed + linearSpeed;
            var rightPower = -angularSpeed + linearSpeed;

            var maxMag = Math.Max(Math.Abs(leftPower), Math.Abs(rightPower));
            if (maxMag > 1)
            {
                // scale power values so they are within -1 to 1
                leftPower /= maxMag;
                rightPower /= maxMag;
            }

            this.SetPowerWithAcceleration(leftPower, rightPower);
        }

        /// <summary>
        /// Calculate linear and angular speed.
        /// Since our depth range is large, maximum speed is reached on the first part of the range.
        /// </summary>
        /// <param name="speedSuggestion">Suggested speed</param>
        /// <param name="angularSpeedSuggestion">Suggested angular speed</param>
        /// <param name="normalizedWidthOfOpenSpace">Normalized width of open space</param>
        /// <param name="relativeObstacleSize">Relative obstacle size</param>
        /// <param name="isPathBlocked">True if paht is blocked</param>
        /// <param name="angularSpeed">Returns the calculated angular speed</param>
        /// <param name="speed">Returns the calculated linear speed</param>
        internal void CalculateControlWithSuggestedSpeeds(
            double speedSuggestion,
            double angularSpeedSuggestion,
            double normalizedWidthOfOpenSpace,
            double relativeObstacleSize,
            bool isPathBlocked,
            out double angularSpeed,
            out double speed)
        {
            speed = speedSuggestion;

            double notUsed;
            double angularSpeedFromMidPointOfOpenSpace;
            this.state.Controller.CalculateControl(out angularSpeedFromMidPointOfOpenSpace, out notUsed);

            var openSpaceFactor = (normalizedWidthOfOpenSpace + (1 - relativeObstacleSize)) / 2;
            speed = speedSuggestion * openSpaceFactor;

            // angular speed is affected by linear speed and open space available. The less open space the more
            // we use the suggested angular speed(which is usually slower so we end up rotating more cautiously)                
            angularSpeed = (angularSpeedSuggestion * openSpaceFactor) +
                (angularSpeedFromMidPointOfOpenSpace * (1 - openSpaceFactor));

            if (angularSpeedSuggestion == 0)
            {
                angularSpeed = angularSpeedFromMidPointOfOpenSpace;
            }

            if (speedSuggestion == 0)
            {
                angularSpeed = angularSpeedSuggestion;
            }

            // special case: Path is blocked
            if (isPathBlocked)
            {
                if (speed > 0)
                {
                    speed = 0;
                }
            }

            // if there is very little linear and angular velocity,
            // normalize them to make the sum of the velocities equal our minimum rotational speed
            if ((speed + Math.Abs(angularSpeed)) < this.state.MinRotationSpeed)
            {
                double normalizingFactor = this.state.MinRotationSpeed / (speed + Math.Abs(angularSpeed));
                angularSpeed *= normalizingFactor;
                speed *= normalizingFactor;
            }
        }

        /// <summary>
        /// Cached downsampled depth image
        /// </summary>
        private short[] depthDownSampleBuffer;

        /// <summary>
        /// Cached horizontalDepthProfile;
        /// </summary>
        private short[] depthProfile;

        /// <summary>
        /// Cached ir sensor profile
        /// </summary>
        private short[] irSensorProfile;

        /// <summary>
        /// Cached sonar sensor profile
        /// </summary>
        private short[] sonarSensorProfile;

        /// <summary>
        /// Last power set to the left motor
        /// </summary>
        private double lastLeftPower;

        /// <summary>
        /// Last power set to the right motor
        /// </summary>
        private double lastRightPower;

        /// <summary>
        /// Downsamples depth image
        /// </summary>
        /// <param name="depthCamState">Depth camera state</param>
        /// <returns>Downsampled depth buffer</returns>
        private short[] DownSampleDepthImage(depthcam.DepthCamSensorState depthCamState)
        {
            if (this.depthDownSampleBuffer == null)
            {
                this.depthDownSampleBuffer = new short[DefaultDepthcamImageHeight * DefaultDepthCamImageWidth];
            }

            // assume depth image divisible by default width/height
            int columnStep = depthCamState.DepthImageSize.Width / DefaultDepthCamImageWidth;
            int rowStep = depthCamState.DepthImageSize.Height / DefaultDepthcamImageHeight;

            DepthImageUtilities.DownSampleDepthImage(depthCamState, this.depthDownSampleBuffer, columnStep, rowStep);

            return this.depthDownSampleBuffer;
        }
    }
}