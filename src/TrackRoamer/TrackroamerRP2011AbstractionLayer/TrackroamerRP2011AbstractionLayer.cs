using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Net;
using System.Text;
using W3C.Soap;

using Microsoft.Ccr.Core;
using Microsoft.Dss.Core.Attributes;
using Microsoft.Dss.Core.DsspHttp;
using Microsoft.Dss.ServiceModel.Dssp;
using Microsoft.Dss.ServiceModel.DsspServiceBase;

using sonarproxy = Microsoft.Robotics.Services.SonarSensorArray.Proxy;
using irproxy = Microsoft.Robotics.Services.InfraredSensorArray.Proxy;
using batteryproxy = Microsoft.Robotics.Services.Battery.Proxy;

using adc = Microsoft.Robotics.Services.ADCPinArray;
using battery = Microsoft.Robotics.Services.Battery;
using ir = Microsoft.Robotics.Services.InfraredSensorArray;
using sonar = Microsoft.Robotics.Services.SonarSensorArray;
using submgr = Microsoft.Dss.Services.SubscriptionManager;

using proxibrick = TrackRoamer.Robotics.Services.TrackRoamerBrickProximityBoard.Proxy;
using TrackRoamer.Robotics.Utility.LibSystem;

namespace TrackRoamer.Robotics.Services.TrackroamerRP2011AbstractionLayer
{
    // see C:\Microsoft Robotics Dev Studio 4\samples\Platforms\ReferencePlatform2011\MarkRobot\Sensors.cs
    //     C:\Microsoft Robotics Dev Studio 4\samples\Platforms\ReferencePlatform2011\MarkRobot\MarkRobot.cs
    //     C:\Microsoft Robotics Dev Studio 4\samples\Common\SonarState.cs  and other files there

    /// <summary>
    ///  Main class for the service (also see the Sensors related part of it in Sensors.cs)
    /// </summary>
    [Contract(Contract.Identifier)]
    [AlternateContract(ir.Contract.Identifier)]
    [AlternateContract(sonar.Contract.Identifier)]
    [AlternateContract(battery.Contract.Identifier)]
    [DisplayName("(User) Trackroamer RP2011 Abstraction Layer")]
    [Description("Trackroamer Reference Platform 2011 Abstraction Layer Service")]
    public partial class TrackroamerRP2011AbstractionLayerService : DsspServiceBase
    {
        /// <summary>
        /// Default IR array hardware identifiers
        /// </summary>
        private static readonly int[] DefaultInfraredArrayIdentifiers = { 2, 1, 0 };

        /// <summary>
        /// Default Sonar array hardware identifiers
        /// </summary>
        private static readonly int[] DefaultSonarArrayIdentifiers = { 9, 8 };

        /// <summary>
        /// Default maximum battery power - assuming 12v battery
        /// </summary>
        private const int DefaultMaxBatteryPower = 12;

        /// <summary>
        /// Service state
        /// </summary>
        [ServiceState]
        [InitialStatePartner(Optional = true)]
        private TrackroamerRP2011AbstractionLayerState state;

        /// <summary>
        /// Initialization flag for service startup
        /// </summary>
        private bool initialized = false;

        [SubscriptionManagerPartner]
        private submgr.SubscriptionManagerPort submgrPort = new submgr.SubscriptionManagerPort();

        /// <summary>
        /// Main service port
        /// </summary>
        [ServicePort("/TrackroamerRP2011AbstractionLayer", AllowMultipleInstances = false)]
        private TrackroamerRP2011AbstractionLayerOperations _mainPort = new TrackroamerRP2011AbstractionLayerOperations();

        /// <summary>
        /// TrackRoamerBrickProximityBoardService partner
        /// </summary>
        [Partner("TrackRoamerProximityBrick", Contract = proxibrick.Contract.Identifier, CreationPolicy = PartnerCreationPolicy.UseExisting)]
        proxibrick.TrackRoamerBrickProximityBoardOperations _trackRoamerBrickProximityBoardServicePort = new proxibrick.TrackRoamerBrickProximityBoardOperations();
        proxibrick.TrackRoamerBrickProximityBoardOperations _trackRoamerBrickProximityBoardServiceNotify = new proxibrick.TrackRoamerBrickProximityBoardOperations();

        /// <summary>
        /// Service constructor
        /// </summary>
        public TrackroamerRP2011AbstractionLayerService(DsspServiceCreationPort creationPort)
            : base(creationPort)
        {
        }

        /// <summary>
        /// Allocation and assignments for first run
        /// </summary>
        /// <returns>True if initialization succeeded, otherwise False</returns>
        private bool Initialize()
        {
            if (this.initialized)
            {
                return this.initialized;
            }

            try
            {
                // No persisted state file, create a new one
                if (this.state == null)
                {
                    this.state = new TrackroamerRP2011AbstractionLayerState();
                }

                // Populate the IR sensor state
                if (this.state.InfraredSensorState == null)
                {
                    this.state.InfraredSensorState = new ir.InfraredSensorArrayState();
                }

                if (this.state.InfraredSensorState.Sensors == null)
                {
                    this.state.InfraredSensorState.Sensors = new List<Microsoft.Robotics.Services.Infrared.InfraredState>();
                }

                if (this.state.InfraredSensorState.Sensors.Count == 0)
                {
                    foreach (int irIdentifier in DefaultInfraredArrayIdentifiers)
                    {
                        this.state.InfraredSensorState.Sensors.Add(new Microsoft.Robotics.Services.Infrared.InfraredState() { HardwareIdentifier = irIdentifier });
                    }
                }

                // Populate the Sonar sensor state
                if (this.state.SonarSensorState == null)
                {
                    this.state.SonarSensorState = new sonar.SonarSensorArrayState();
                }

                if (this.state.SonarSensorState.Sensors == null)
                {
                    this.state.SonarSensorState.Sensors = new List<Microsoft.Robotics.Services.Sonar.SonarState>();
                }

                if (this.state.SonarSensorState.Sensors.Count == 0)
                {
                    foreach (int sonarIdentifier in DefaultSonarArrayIdentifiers)
                    {
                        this.state.SonarSensorState.Sensors.Add(new Microsoft.Robotics.Services.Sonar.SonarState() { HardwareIdentifier = sonarIdentifier });
                    }
                }

                if (this.state.BatteryState == null)
                {
                    this.state.BatteryState = new Microsoft.Robotics.Services.Battery.BatteryState();
                }

                if (this.state.BatteryState.MaxBatteryPower == 0)
                {
                    this.state.BatteryState.MaxBatteryPower = DefaultMaxBatteryPower;
                }
            }
            catch (Exception e)
            {
                LogError(e);
                this.Shutdown();
                return false;
            }

            this.state.LastStartTime = DateTime.Now;

            SaveState(this.state);

            base.Start();

            // Make sure the pin polling port is in the main interleave because it modifies service state
            MainPortInterleave.CombineWith(
                                           new Interleave(
                                           new ExclusiveReceiverGroup(
                                                Arbiter.ReceiveWithIterator(true, this.sensorPollingPort, this.PollSensors),
                                                Arbiter.Receive<proxibrick.UpdateSonarData>(true, _trackRoamerBrickProximityBoardServiceNotify, trpbUpdateSonarNotification),
                                                Arbiter.Receive<proxibrick.UpdateProximityData>(true, _trackRoamerBrickProximityBoardServiceNotify, trpbUpdateProximityNotification),
                                                Arbiter.Receive<proxibrick.UpdateParkingSensorData>(true, _trackRoamerBrickProximityBoardServiceNotify, trpbUpdateParkingSensorNotification)
                                           )
                                           ,new ConcurrentReceiverGroup(
                                                //Arbiter.Receive<drive.Update>(true, this.driveNotifyPort, this.DriveNotification)
                                           )
                                          ));

            //this.controllerDrivePort.Post(new drive.Subscribe() { NotificationPort = this.driveNotifyPort });
            //this.controllerDrivePort.Post(new drive.ReliableSubscribe() { NotificationPort = this.driveNotifyPort });

            Type[] notifyMeOf = new Type[] { typeof(proxibrick.UpdateSonarData), typeof(proxibrick.UpdateProximityData), typeof(proxibrick.UpdateParkingSensorData) };

            _trackRoamerBrickProximityBoardServicePort.Subscribe(_trackRoamerBrickProximityBoardServiceNotify, notifyMeOf);


            // Start the sensors polling interval
            this.sensorPollingPort.Post(DateTime.Now);

            return true;
        }

        /// <summary>
        /// Retrieve the COM port specified in the SerialCOMService state.
        /// Perform initialization if COM port is correct and available.
        /// </summary>
        /// <returns>Enumerator of type ITask</returns>
        private IEnumerator<ITask> InternalInitialize()
        {
            this.initialized = this.Initialize();

            yield break;
        }

        /// <summary>
        /// Service start.
        /// </summary>
        protected override void Start()
        {
            if (!this.initialized)
            {
                SpawnIterator(this.InternalInitialize);
            }
        }

        /// <summary>
        /// Handler for GET operations
        /// </summary>
        /// <param name="get">A GET instance</param>
        /// <returns>A CCR task enumerator</returns>
        [ServiceHandler]
        public void GetHandler(Get get)
        {
            get.ResponsePort.Post(this.state);
        }

        /// <summary>
        /// Handles HttpGet requests
        /// </summary>
        /// <param name="httpget">request message</param>
        [ServiceHandler]
        public void HttpGetHandler(Microsoft.Dss.Core.DsspHttp.HttpGet httpget)
        {
            HttpResponseType resp = new HttpResponseType(HttpStatusCode.OK, this.state);
            httpget.ResponsePort.Post(resp);
        }

        /// <summary>
        /// Handles Subscribe messages
        /// </summary>
        /// <param name="subscribe">The subscribe request</param>
        /// <returns>Enumerator of type ITask</returns>
        [ServiceHandler]
        public IEnumerator<ITask> SubscribeHandler(Subscribe subscribe)
        {
            SuccessFailurePort responsePort = SubscribeHelper(this.submgrPort, subscribe.Body, subscribe.ResponsePort);
            yield return responsePort.Choice();

            var success = (SuccessResult)responsePort;
            if (success != null)
            {
                SendNotificationToTarget<Replace>(subscribe.Body.Subscriber, this.submgrPort, this.state);
            }

            yield break;
        }

        /// <summary>
        /// Drop handler
        /// </summary>
        /// <param name="drop">DSS default drop type</param>
        [ServiceHandler]
        public void DropHandler(DsspDefaultDrop drop)
        {
            this.DefaultDropHandler(drop);
        }
    }
}


