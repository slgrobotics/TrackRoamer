using System;
using System.Collections.Generic;
using System.ComponentModel;
using Microsoft.Ccr.Core;
using Microsoft.Dss.Core.Attributes;
using Microsoft.Dss.ServiceModel.Dssp;
using Microsoft.Dss.ServiceModel.DsspServiceBase;
using W3C.Soap;
using submgr = Microsoft.Dss.Services.SubscriptionManager;
using trackroamerdrive = TrackRoamer.Robotics.Services.TrackRoamerDrive.Proxy;
using drive = Microsoft.Robotics.Services.Drive.Proxy;
using chrum6orientationsensor = TrackRoamer.Robotics.Hardware.ChrUm6OrientationSensor.Proxy;
using gps = Microsoft.Robotics.Services.Sensors.Gps.Proxy;
using depthcam = Microsoft.Robotics.Services.DepthCamSensor;
using micarrayspeechrecognizer = Microsoft.Robotics.Services.Sensors.Kinect.MicArraySpeechRecognizer.Proxy;
using micarrayspeechrecognizergui = Microsoft.Robotics.Services.Sensors.Kinect.MicArraySpeechRecognizerGui.Proxy;
using trackroamerusrf = TrackRoamer.Robotics.Services.TrackRoamerUsrf.Proxy;
using sicklrf = Microsoft.Robotics.Services.Sensors.SickLRF.Proxy;
using trackroamerproximitybrick = TrackRoamer.Robotics.Services.TrackRoamerBrickProximityBoard.Proxy;
using trackroamerpowerbrick = TrackRoamer.Robotics.Services.TrackRoamerBot.Proxy;
using bumper = TrackRoamer.Robotics.Services.TrackRoamerServices.Bumper.Proxy;
using contactsensor = Microsoft.Robotics.Services.ContactSensor.Proxy;
//using encoder = TrackRoamer.Robotics.Services.TrackRoamerServices.Encoder.Proxy;
using encoder = Microsoft.Robotics.Services.Encoder.Proxy;

namespace TrackRoamer.Robotics.Services.ObstacleAvoidanceDrive
{
    [Contract(Contract.Identifier)]
    [DisplayName("(User) Trackroamer Obstacle Avoidance Drive")]
    [Description("Trackroamer Obstacle Avoidance Drive service")]
    class TrackroamerObstacleAvoidanceDriveService : DsspServiceBase
    {
        /// <summary>
        /// Service state
        /// </summary>
        [ServiceState]
        TrackroamerObstacleAvoidanceDriveState _state = new TrackroamerObstacleAvoidanceDriveState();

        /// <summary>
        /// Main service port
        /// </summary>
        [ServicePort("/TrackroamerObstacleAvoidanceDrive", AllowMultipleInstances = false)]
        TrackroamerObstacleAvoidanceDriveOperations _mainPort = new TrackroamerObstacleAvoidanceDriveOperations();

        #region partners

        [SubscriptionManagerPartner]
        submgr.SubscriptionManagerPort _submgrPort = new submgr.SubscriptionManagerPort();

        /// <summary>
        /// TrackRoamerDriveService partner
        /// </summary>
        [Partner("TrackRoamerDriveService", Contract = trackroamerdrive.Contract.Identifier, CreationPolicy = PartnerCreationPolicy.UseExisting, Optional = false)]
        drive.DriveOperations _trackRoamerDriveServicePort = new drive.DriveOperations();
        drive.DriveOperations _trackRoamerDriveServiceNotify = new drive.DriveOperations();

        /// <summary>
        /// ChrUm6OrientationSensorService partner
        /// </summary>
        [Partner("ChrUm6OrientationSensorService", Contract = chrum6orientationsensor.Contract.Identifier, CreationPolicy = PartnerCreationPolicy.UseExistingOrCreate)]
        chrum6orientationsensor.ChrUm6OrientationSensorOperations _chrUm6OrientationSensorServicePort = new chrum6orientationsensor.ChrUm6OrientationSensorOperations();
        chrum6orientationsensor.ChrUm6OrientationSensorOperations _chrUm6OrientationSensorServiceNotify = new chrum6orientationsensor.ChrUm6OrientationSensorOperations();

        /// <summary>
        /// MicrosoftGpsService partner
        /// </summary>
        [Partner("MicrosoftGpsService", Contract = gps.Contract.Identifier, CreationPolicy = PartnerCreationPolicy.UseExistingOrCreate)]
        gps.MicrosoftGpsOperations _microsoftGpsServicePort = new gps.MicrosoftGpsOperations();
        gps.MicrosoftGpsOperations _microsoftGpsServiceNotify = new gps.MicrosoftGpsOperations();

        /// <summary>
        /// DepthCam sensor port
        /// </summary>
        [Partner(
            "service:DepthCamera",
            Contract = depthcam.Contract.Identifier,
            Optional = false,
            CreationPolicy = PartnerCreationPolicy.UseExisting)]
        private depthcam.DepthCamSensorOperationsPort depthCameraPort = new depthcam.DepthCamSensorOperationsPort();


        /// <summary>
        /// SpeechRecognizer partner
        /// </summary>
        //[Partner("SpeechRecognizer", Contract = micarrayspeechrecognizer.Contract.Identifier, CreationPolicy = PartnerCreationPolicy.UseExistingOrCreate)]
        //micarrayspeechrecognizer.SpeechRecognizerOperations _speechRecognizerPort = new micarrayspeechrecognizer.SpeechRecognizerOperations();
        //micarrayspeechrecognizer.SpeechRecognizerOperations _speechRecognizerNotify = new micarrayspeechrecognizer.SpeechRecognizerOperations();

        /// <summary>
        /// SpeechRecognizerGui partner
        /// </summary>
        //[Partner("SpeechRecognizerGui", Contract = micarrayspeechrecognizergui.Contract.Identifier, CreationPolicy = PartnerCreationPolicy.UseExistingOrCreate)]
        //micarrayspeechrecognizergui.SpeechRecognizerGuiOperations _speechRecognizerGuiPort = new micarrayspeechrecognizergui.SpeechRecognizerGuiOperations();
        //micarrayspeechrecognizergui.SpeechRecognizerGuiOperations _speechRecognizerGuiNotify = new micarrayspeechrecognizergui.SpeechRecognizerGuiOperations();

        /// <summary>
        /// TrackRoamerUsrfService partner
        /// </summary>
        [Partner("TrackRoamerUsrfService", Contract = trackroamerusrf.Contract.Identifier, CreationPolicy = PartnerCreationPolicy.UseExistingOrCreate)]
        sicklrf.SickLRFOperations _trackRoamerUsrfServicePort = new sicklrf.SickLRFOperations();
        sicklrf.SickLRFOperations _trackRoamerUsrfServiceNotify = new sicklrf.SickLRFOperations();

        /// <summary>
        /// TrackRoamerBrickProximityBoardService partner
        /// </summary>
        [Partner("TrackRoamerProximityBrick", Contract = trackroamerproximitybrick.Contract.Identifier, CreationPolicy = PartnerCreationPolicy.UseExistingOrCreate)]
        trackroamerproximitybrick.TrackRoamerBrickProximityBoardOperations _trackRoamerBrickProximityBoardServicePort = new trackroamerproximitybrick.TrackRoamerBrickProximityBoardOperations();
        trackroamerproximitybrick.TrackRoamerBrickProximityBoardOperations _trackRoamerBrickProximityBoardServiceNotify = new trackroamerproximitybrick.TrackRoamerBrickProximityBoardOperations();

        /// <summary>
        /// TrackRoamerBotService partner
        /// </summary>
        [Partner("TrackRoamerPowerBrick", Contract = trackroamerpowerbrick.Contract.Identifier, CreationPolicy = PartnerCreationPolicy.UseExistingOrCreate)]
        trackroamerpowerbrick.TrackRoamerBotOperations _trackRoamerBotServicePort = new trackroamerpowerbrick.TrackRoamerBotOperations();
        trackroamerpowerbrick.TrackRoamerBotOperations _trackRoamerBotServiceNotify = new trackroamerpowerbrick.TrackRoamerBotOperations();

        /// <summary>
        /// BumperService partner
        /// </summary>
        [Partner("BumperService", Contract = bumper.Contract.Identifier, CreationPolicy = PartnerCreationPolicy.UseExistingOrCreate)]
        contactsensor.ContactSensorArrayOperations _bumperServicePort = new contactsensor.ContactSensorArrayOperations();
        contactsensor.ContactSensorArrayOperations _bumperServiceNotify = new contactsensor.ContactSensorArrayOperations();

        /// <summary>
        /// EncoderService partner
        /// </summary>
        //[Partner("EncoderService", Contract = encoder.Contract.Identifier, CreationPolicy = PartnerCreationPolicy.UseExistingOrCreate)]
        //encoder.EncoderOperations _encoderServicePort = new encoder.EncoderOperations();
        //encoder.EncoderOperations _encoderServiceNotify = new encoder.EncoderOperations();

        #region Encoder partners
        // this didn't work as supposed - both subscriptions received notifications from the same encoder, no matter what I tried.
        // I resorted to receiving notifications directly from TrackRoamerBot instead.

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

        #endregion // Encoder partners

        #endregion // partners


        /// <summary>
        /// Service constructor
        /// </summary>
        public TrackroamerObstacleAvoidanceDriveService(DsspServiceCreationPort creationPort)
            : base(creationPort)
        {
        }

        /// <summary>
        /// Service start
        /// </summary>
        protected override void Start()
        {

            // 
            // Add service specific initialization here
            // 

            base.Start();
        }

        /// <summary>
        /// Handles Subscribe messages
        /// </summary>
        /// <param name="subscribe">the subscribe request</param>
        [ServiceHandler]
        public void SubscribeHandler(Subscribe subscribe)
        {
            SubscribeHelper(_submgrPort, subscribe.Body, subscribe.ResponsePort);
        }
    }
}


