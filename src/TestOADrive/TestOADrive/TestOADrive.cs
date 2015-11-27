using System;
using System.Collections.Generic;
using System.ComponentModel;
using Microsoft.Ccr.Core;
using Microsoft.Dss.Core.Attributes;
using Microsoft.Dss.ServiceModel.Dssp;
using Microsoft.Dss.ServiceModel.DsspServiceBase;
using W3C.Soap;
using submgr = Microsoft.Dss.Services.SubscriptionManager;
using trdrive = TrackRoamer.Robotics.Services.TrackRoamerDrive.Proxy;
using drive = Microsoft.Robotics.Services.Drive.Proxy;
using obstacleavoidancedrive = Microsoft.Robotics.Services.ObstacleAvoidanceDrive.Proxy;

namespace TrackRoamer.Robotics.Services.TestOADrive
{
    [Contract(Contract.Identifier)]
    [DisplayName("TestOADrive")]
    [Description("TestOADrive service (no description provided)")]
    class TestOADriveService : DsspServiceBase
    {
        protected const int settlingTime = 500;         // Time to wait, ms, after each move to let things settle

        public const double ModerateForwardVelocity = 120; // mm/sec
        public const double ModerateTurnPower = 0.085d; // of 1.0=full power
        public const double PowerScale = 1.0d;   // all above power and speed are multiplied by PowerScale

        double utForwardVelocity = ModerateForwardVelocity;
        //double utTurnPower = ModerateTurnPower;
        double utPowerScale = PowerScale;

        /// <summary>
        /// Service state
        /// </summary>
        [ServiceState]
        TestOADriveState _state = new TestOADriveState();

        /// <summary>
        /// Main service port
        /// </summary>
        [ServicePort("/TestOADrive", AllowMultipleInstances = false)]
        TestOADriveOperations _mainPort = new TestOADriveOperations();

        [SubscriptionManagerPartner]
        submgr.SubscriptionManagerPort _submgrPort = new submgr.SubscriptionManagerPort();

        /// <summary>
        /// TrackRoamerDriveService partner
        /// </summary>
        //[Partner("TrackRoamerDriveService", Contract = trdrive.Contract.Identifier, CreationPolicy = PartnerCreationPolicy.UseExisting, Optional = false)]
        //drive.DriveOperations _drivePort = new drive.DriveOperations();
        ////drive.DriveOperations _trackRoamerDriveServiceNotify = new drive.DriveOperations();

        /// <summary>
        /// DriveDifferentialTwoWheel partner. Used for DriveByDistance, TurnByAngle requests, which require wait and should go directly to the drive.
        /// </summary>
        [Partner("TrackRoamerDrive", Contract = drive.Contract.Identifier, CreationPolicy = PartnerCreationPolicy.UsePartnerListEntry, Optional = false)]
        protected drive.DriveOperations _drivePort = new drive.DriveOperations();

        #region Obstacle Avoidance Drive partner

        /// <summary>
        /// ObstacleAvoidanceDriveService partner
        /// </summary>
        //[Partner("ObstacleAvoidanceDriveService", Contract = obstacleavoidancedrive.Contract.Identifier, CreationPolicy = PartnerCreationPolicy.UseExisting)]
        //obstacleavoidancedrive.ObstacleAvoidanceDriveOperationsPort _obstacleAvoidanceDriveServicePort = new obstacleavoidancedrive.ObstacleAvoidanceDriveOperationsPort();

        /// <summary>
        /// ObstacleAvoidanceDrive partner. Used for SetDrivePower requests in MoveForward(), which allow being tweaked/overrridden by ObstacleAvoidanceDrive intermediary and don't require wait.
        /// </summary>
        [Partner("ObstacleAvoidanceDrive", Contract = drive.Contract.Identifier, CreationPolicy = PartnerCreationPolicy.UsePartnerListEntry, Optional = false)]
        drive.DriveOperations _obstacleAvoidanceDrivePort = new drive.DriveOperations();

        #endregion // Obstacle Avoidance Drive partner

        /// <summary>
        /// Service constructor
        /// </summary>
        public TestOADriveService(DsspServiceCreationPort creationPort)
            : base(creationPort)
        {
        }

        /// <summary>
        /// Service start
        /// </summary>
        protected override void Start()
        {
            base.Start();

            SpawnIterator(this.ConnectObstacleAvoidanceDrive);

            performUnitTest();
        }

        private void performUnitTest()
        {
            LogInfo("DriveBehaviorServiceBase: performUnitTest() Started");

            // to test drive operation, execute the geometric pattern

            //SpawnIterator(BehaviorMoveForward);

            SpawnIterator(BehaviorPushForward);

            //SpawnIterator(BehaviorTurn);

            //SpawnIterator(BehaviorTurnAndMoveForward);

            //SpawnIterator(BehaviorSquare);

            //SpawnIterator(BehaviorStraight);

            // SpawnIterator(BehaviorStraightInterruptTurn);

            //SpawnIterator(BehaviorKata);

            LogInfo("DriveBehaviorServiceBase: performUnitTest() finished");
        }

        #region BehaviorPushForward

        // Iterator to execute the Behavior
        // It is important to use an Iterator so that it can relinquish control
        // when there is nothing to do, i.e. yield return
        protected IEnumerator<ITask> BehaviorPushForward()
        {
            LogInfo("DriveBehaviorServiceBase: BehaviorPushForward() Started");

            Console.WriteLine("waiting  for Behavior Push Forward");

            // Wait for the robot to initialize, otherwise it will
            // miss the initial command
            for (int i = 10; i > 0; i--)
            {
                yield return Timeout(1000);
            }

            Console.WriteLine("starting Behavior Push Forward");

            // Wait for settling time
            yield return Timeout(settlingTime);

            for (int i = 1; i <= 10; i++)
            {
                Console.WriteLine("push " + i);

                // a fire-and-forget command to move forward:
                MoveForward(utForwardVelocity * utPowerScale);

                // wait some time
                LogInfo(i.ToString());
                yield return Timeout(2000);
            }

            // we expect the drive to stop at the command, not by completion:
            StopMoving();

            Console.WriteLine("Behavior Move Forward finished");

            // done
            yield break;
        }
        #endregion // BehaviorPushForward

        /// <summary>
        /// Sets the velocity of the drive to 0.
        /// </summary>
        /// <returns></returns>
        protected PortSet<DefaultUpdateResponseType, Fault> StopMoving()
        {
            LogInfo("DriveBehaviorServiceBase:: StopMoving()");

            return MoveForward(0);
        }


        protected PortSet<DefaultUpdateResponseType, Fault> MoveForward(double speed)
        {
            LogInfo(string.Format("DriveBehaviorServiceBase:: MoveForward() speed={0} mm/sec", speed));

            Console.WriteLine(speed == 0.0d ? "Stop Moving" : ("Move Forward speed " + speed));

            //if (speed != 0.0d)
            //{
            //    EnableDrive();
            //}

            // use power equivallent:
            double wheelPower = Math.Max(-1.0d, Math.Min(1.0d, (double)speed / 1000.0d)); // get it to -1...1 range, as wheel power must be in this range 

            if (speed == 0.0d)
            {
                Console.WriteLine("Behavior Base: MoveForward()  --------------------------- to TR ----------------------------------------    speed=" + speed + "   wheelPower=" + wheelPower);
                _obstacleAvoidanceDrivePort.SetDrivePower(wheelPower, wheelPower);
                return _drivePort.SetDrivePower(wheelPower, wheelPower);
            }
            else
            {
                Console.WriteLine("Behavior Base: MoveForward()  =========================== to OA ========================================    speed=" + speed + "   wheelPower=" + wheelPower);
                //var request = new drive.SetDrivePowerRequest
                //{
                //    LeftWheelPower = wheelPower,
                //    RightWheelPower = wheelPower
                //};

                //return _obstacleAvoidanceDrivePort.SetDrivePower(request);
                return _obstacleAvoidanceDrivePort.SetDrivePower(wheelPower, wheelPower);
            }
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


