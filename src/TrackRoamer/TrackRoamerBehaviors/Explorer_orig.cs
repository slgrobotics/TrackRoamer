//-----------------------------------------------------------------------
//  This file is part of Microsoft Robotics Developer Studio Code Samples.
//
//  Copyright (C) Microsoft Corporation.  All rights reserved.
//
//  $File: Explorer.cs $ $Revision: 1 $
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;


using W3C.Soap;
using Microsoft.Ccr.Core;
using Microsoft.Dss.Core;
using Microsoft.Dss.Core.Attributes;
using Microsoft.Dss.ServiceModel.Dssp;
using Microsoft.Dss.Services.Serializer;
using Microsoft.Dss.ServiceModel.DsspServiceBase;

using bumper = Microsoft.Robotics.Services.ContactSensor.Proxy;
using drive = Microsoft.Robotics.Services.Drive.Proxy;
using sicklrf = Microsoft.Robotics.Services.Sensors.SickLRF.Proxy;

using dssp = Microsoft.Dss.ServiceModel.Dssp;
using System.ComponentModel;

namespace Microsoft.Robotics.Services.Explorer
{
    [DisplayName("Explorer")]
    [Description("Provides access simple exploration behavior for a robot using a differential drive, bumpers, and a laser range finder.")]
    [Contract(Contract.Identifier)]
    class Explorer : DsspServiceBase
    {
        #region constants
        // The explorer uses there constants to evaluate open space
        // and adjust speed and rotation of the differential drive.
        // Change those values to match your robot.
        // You could expose them on the explorers state type and make the
        // service configurable.

        /// <summary>
        /// Amount to backup when hitting an obstacle.
        /// </summary>
        const int BackupDistance = -300; // mm

        /// <summary>
        /// If an obstacle comes within thisdistance the robot stops moving.
        /// </summary>
        const int ObstacleDistance = 500; // mm

        /// <summary>
        /// If the robot is mapping and has this much open space ahead it stops mapping
        /// and enters the space.
        /// </summary>
        const int SafeDistance = 2000; // mm

        /// <summary>
        /// The width of the corridor that must be safe in order to got from mapping to moving.
        /// </summary>
        const int CorridorWidthMapping = 350; // mm

        /// <summary>
        /// The minimum free distance that is required to drive with max. velocity.
        /// </summary>
        const int FreeDistance = 3000; // mm

        /// <summary>
        /// If the free space is at least this distance the robot operates at 1/2 max. velocity otherwise
        /// the robot slows down to 1/4 of max. vel.
        /// </summary>
        const int AwareOfObstacleDistance = 1500; // mm

        /// <summary>
        /// The max. velocity with which to move.
        /// </summary>
        const int MaximumForwardVelocity = 1000; // mm/sec

        /// <summary>
        /// The with of the corridor in which obstacles effect velocity.
        /// </summary>
        const int CorridorWidthMoving = 500; // mm

        /// <summary>
        /// If no laser data is received within this time the robot stops.
        /// </summary>
        const int WatchdogTimeout = 500; // msec

        /// <summary>
        /// Interval between timer notifications.
        /// </summary>
        const int WatchdogInterval = 100; // msec
        #endregion

        #region main port and state
        State _state = new State();

        [ServicePort("/explorer")]
        ExplorerOperations _mainPort = new ExplorerOperations();
        #endregion

        #region partners
        #region bumper partner
        [Partner("Bumper", Contract = bumper.Contract.Identifier, CreationPolicy = PartnerCreationPolicy.UseExisting)]
        bumper.ContactSensorArrayOperations _bumperPort = new bumper.ContactSensorArrayOperations();
        bumper.ContactSensorArrayOperations _bumperNotify = new bumper.ContactSensorArrayOperations();
        #endregion

        #region drive partner
        [Partner("Drive", Contract = drive.Contract.Identifier, CreationPolicy = PartnerCreationPolicy.UseExisting)]
        drive.DriveOperations _drivePort = new drive.DriveOperations();
        drive.DriveOperations _driveNotify = new drive.DriveOperations();
        #endregion

        #region laser range finder partner
        [Partner("Laser", Contract = sicklrf.Contract.Identifier, CreationPolicy = PartnerCreationPolicy.UseExistingOrCreate)]
        sicklrf.SickLRFOperations _laserPort = new sicklrf.SickLRFOperations();
        sicklrf.SickLRFOperations _laserNotify = new sicklrf.SickLRFOperations();
        #endregion
        #endregion

        public Explorer(DsspServiceCreationPort creationPort) :
                base(creationPort)
        {
        }

        protected override void Start()
        {
            #region request handler setup
            Activate(
                Arbiter.Interleave(
                    new TeardownReceiverGroup(
                        Arbiter.Receive<DsspDefaultDrop>(false, _mainPort, DropHandler)
                    ),
                    new ExclusiveReceiverGroup(
                        Arbiter.Receive<LaserRangeFinderResetUpdate>(true, _mainPort, LaserRangeFinderResetUpdateHandler),
                        Arbiter.Receive<LaserRangeFinderUpdate>(true, _mainPort, LaserRangeFinderUpdateHandler),
                        Arbiter.Receive<BumpersUpdate>(true, _mainPort, BumpersUpdateHandler),
                        Arbiter.Receive<BumperUpdate>(true, _mainPort, BumperUpdateHandler),
                        Arbiter.Receive<DriveUpdate>(true, _mainPort, DriveUpdateHandler),
                        Arbiter.Receive<WatchDogUpdate>(true, _mainPort, WatchDogUpdateHandler)
                    ),
                    new ConcurrentReceiverGroup(
                        Arbiter.Receive<Get>(true, _mainPort, GetHandler),
                        Arbiter.Receive<dssp.DsspDefaultLookup>(true, _mainPort, DefaultLookupHandler)
                    )
                )
            );
            #endregion

            #region notification handler setup
            Activate(
                Arbiter.Interleave(
                    new TeardownReceiverGroup(),
                    new ExclusiveReceiverGroup(),
                    new ConcurrentReceiverGroup(
                        Arbiter.Receive<sicklrf.Reset>(true, _laserNotify, LaserResetNotification),
                        Arbiter.Receive<drive.Update>(true, _driveNotify, DriveUpdateNotification),
                        Arbiter.Receive<bumper.Replace>(true, _bumperNotify, BumperReplaceNotification),
                        Arbiter.Receive<bumper.Update>(true, _bumperNotify, BumperUpdateNotification)
                    )
                )
            );

            // We cannot replicate the activation of laser notifications because the
            // handler uses Test() to skip old laser notifications.
            Activate(
                Arbiter.ReceiveWithIterator<sicklrf.Replace>(false, _laserNotify, LaserReplaceNotification)
            );
            #endregion

            // Start watchdog timer
            _mainPort.Post(new WatchDogUpdate(new WatchDogUpdateRequest(DateTime.Now)));

            // Create Subscriptions
            _bumperPort.Subscribe(_bumperNotify);
            _drivePort.Subscribe(_driveNotify);
            _laserPort.Subscribe(_laserNotify);

            DirectoryInsert();
        }

        #region DSS operation handlers (Get, Drop)
        public void DropHandler(DsspDefaultDrop drop)
        {
            // Currently it is not possible to activate a handler that returns
            // IEnumerator<ITask> in a teardown group. Thus we have to spawn the iterator ourselves
            // to acheive the same effect.
            // This issue will be addressed in upcomming releases.
            SpawnIterator(drop, DoDrop);
        }

        IEnumerator<ITask> DoDrop(DsspDefaultDrop drop)
        {
            if (_state.IsActive)
            {
                LogInfo("Explorer service is being dropped while moving, Requesting Stop.");

                yield return Arbiter.Choice(
                    StopMoving(),
                    delegate(DefaultUpdateResponseType response) { },
                    delegate(Fault fault) { }
                );

                yield return Arbiter.Choice(
                    DisableMotor(),
                    delegate(DefaultUpdateResponseType response) { },
                    delegate(Fault fault) { }
                );
            }

            LogInfo("Dropping Explorer.");

            base.DefaultDropHandler(drop);
        }

        void GetHandler(Get get)
        {
            get.ResponsePort.Post(_state);
        }
        #endregion

        #region Watch dog timer handlers
        void WatchDogUpdateHandler(WatchDogUpdate update)
        {
            TimeSpan sinceLaser = update.Body.TimeStamp - _state.MostRecentLaser;

            if (sinceLaser.TotalMilliseconds >= WatchdogTimeout && !_state.IsUnknown)
            {
                LogInfo("Stop requested, last laser data seen at " + _state.MostRecentLaser);
                StopMoving();
                _state.LogicalState = LogicalState.Unknown;
            }

            Activate(
               Arbiter.Receive(
                   false,
                   TimeoutPort(WatchdogInterval),
                   delegate(DateTime ts)
                   {
                       _mainPort.Post(new WatchDogUpdate(new WatchDogUpdateRequest(ts)));
                   }
               )
            );

            update.ResponsePort.Post(DefaultUpdateResponseType.Instance);
        }
        #endregion

        #region  Bumper handlers
        /// <summary>
        /// Handles Replace notifications from the Bumper partner
        /// </summary>
        /// <remarks>Posts a <typeparamref name="BumpersUpdate"/> to itself.</remarks>
        /// <param name="replace">notification</param>
        void BumperReplaceNotification(bumper.Replace replace)
        {
            _mainPort.Post(new BumpersUpdate(replace.Body));
        }

        /// <summary>
        /// Handles Update notification from the Bumper partner
        /// </summary>
        /// <remarks>Posts a <typeparamref name="BumperUpdate"/> to itself.</remarks>
        /// <param name="update">notification</param>
        void BumperUpdateNotification(bumper.Update update)
        {
            _mainPort.Post(new BumperUpdate(update.Body));
        }

        /// <summary>
        /// Handles the <typeparamref name="BumpersUpdate"/> request.
        /// </summary>
        /// <param name="update">request</param>
        void BumpersUpdateHandler(BumpersUpdate update)
        {
            if (_state.IsMoving && BumpersPressed(update.Body))
            {
                Bumped();
            }
            update.ResponsePort.Post(DefaultUpdateResponseType.Instance);
        }

        /// <summary>
        /// Handles the <typeparamref name="BumperUpdate"/> request.
        /// </summary>
        /// <param name="update">request</param>
        void BumperUpdateHandler(BumperUpdate update)
        {
            if (_state.IsMoving && update.Body.Pressed)
            {
                Bumped();
            }
            update.ResponsePort.Post(DefaultUpdateResponseType.Instance);
        }

        /// <summary>
        /// Stops the robot. If the robot was going forward it backs up.
        /// </summary>
        private void Bumped()
        {
            if (_state.Velocity <= 0.0)
            {
                LogInfo("Rear and/or Front bumper pressed, Stopping");
                // either a rear bumper or both front and rear
                // bumpers are pressed. STOP!
                StopTurning();
                StopMoving();

                _state.LogicalState = LogicalState.Unknown;
                _state.Countdown = 3;
            }
            else
            {
                LogInfo("Front bumper pressed, Backing up by " + (-BackupDistance) + "mm");
                // only a front bumper is pressed.
                // move back <BackupDistance> mm;
                StopTurning();
                Translate(BackupDistance);

                _state.LogicalState = LogicalState.Unknown;
                _state.Countdown = 5;
            }
        }

        /// <summary>
        /// Checks whether at least one of the contact sensors is pressed.
        /// </summary>
        /// <param name="bumpers"><code>true</code> if at least one bumper in <paramref name="bumpers"/> is pressed, otherwise <code>false</code></param>
        /// <returns></returns>
        private bool BumpersPressed(bumper.ContactSensorArrayState bumpers)
        {
            if (bumpers.Sensors == null)
            {
                return false;
            }
            foreach (bumper.ContactSensor s in bumpers.Sensors)
            {
                if (s.Pressed)
                {
                    return true;
                }
            }
            return false;
        }
        #endregion

        #region Drive handlers
        /// <summary>
        /// Handles Update notification from the Drive partner
        /// </summary>
        /// <remarks>Posts a <typeparamref name="DriveUpdate"/> request to itself.</remarks>
        /// <param name="update">notification</param>
        void DriveUpdateNotification(drive.Update update)
        {
            _mainPort.Post(new DriveUpdate(update.Body));
        }

        /// <summary>
        /// Handles DriveUpdate request
        /// </summary>
        /// <param name="update">request</param>
        void DriveUpdateHandler(DriveUpdate update)
        {
            _state.DriveState = update.Body;
            _state.Velocity = (VelocityFromWheel(update.Body.LeftWheel) + VelocityFromWheel(update.Body.RightWheel)) / 2;
            update.ResponsePort.Post(DefaultUpdateResponseType.Instance);
        }

        /// <summary>
        /// Computes the wheel velocity in mm/s.
        /// </summary>
        /// <param name="wheel">wheel</param>
        /// <returns>velocity</returns>
        private int VelocityFromWheel(Microsoft.Robotics.Services.Motor.Proxy.WheeledMotorState wheel)
        {
            if (wheel == null)
            {
                return 0;
            }
            return (int)(1000 * wheel.WheelSpeed); // meters to millimeters
        }
        #endregion

        #region Laser handlers

        /// <summary>
        /// Handles Replace notifications from the Laser partner
        /// </summary>
        /// <remarks>Posts a <typeparamref name="LaserRangeFinderUpdate"/> to itself.</remarks>
        /// <param name="replace">notification</param>
        /// <returns>task enumerator</returns>
        IEnumerator<ITask> LaserReplaceNotification(sicklrf.Replace replace)
        {
            // When this handler is called a couple of notifications may
            // have piled up. We only want the most recent one.
            sicklrf.State laserData = GetMostRecentLaserNotification(replace.Body);

            LaserRangeFinderUpdate request = new LaserRangeFinderUpdate(laserData);

            _mainPort.Post(request);

            yield return Arbiter.Choice(
                request.ResponsePort,
                delegate(DefaultUpdateResponseType response) { },
                delegate(Fault fault) { }
            );

            // Skip messages that have been queued up in the meantime.
            // The notification that are lingering are out of data by now.
            GetMostRecentLaserNotification(laserData);

            // Reactivate the handler.
            Activate(
                Arbiter.ReceiveWithIterator<sicklrf.Replace>(false, _laserNotify, LaserReplaceNotification)
            );
        }

        /// <summary>
        /// Handles the <typeparamref name="LaserRangeFinderUpdate"/> request.
        /// </summary>
        /// <param name="update">request</param>
        void LaserRangeFinderUpdateHandler(LaserRangeFinderUpdate update)
        {
            sicklrf.State laserData = update.Body;
            _state.MostRecentLaser = laserData.TimeStamp;

            int distance = FindNearestObstacleInCorridor(laserData, CorridorWidthMapping, 45);

            // AvoidCollision and EnterOpenSpace have precedence over
            // all other state transitions and are thus handled first.
            AvoidCollision(distance);
            EnterOpenSpace(distance);

            UpdateLogicalState(laserData, distance);

            update.ResponsePort.Post(DefaultUpdateResponseType.Instance);
        }

        /// <summary>
        /// If the robot is mapping and there is sufficient open space directly ahead, enter this space.
        /// </summary>
        /// <param name="distance"></param>
        private void EnterOpenSpace(int distance)
        {
            if (distance > SafeDistance && _state.IsMapping)
            {
                // We are mapping but can see plenty of free space ahead.
                // The robot should go into this space.

                StopTurning();
                _state.LogicalState = LogicalState.FreeForwards;
                _state.Countdown = 4;
            }
        }

        /// <summary>
        /// If the robot is moving and an obstacle is too close, stop and map the environment for a way around it.
        /// </summary>
        /// <param name="distance"></param>
        private void AvoidCollision(int distance)
        {
            if (distance < ObstacleDistance && _state.IsMoving)
            {
                //
                // We are moving and there is something less than <LaserObstacleDistance>
                // millimeters from the center of the robot. STOP.
                //

                StopMoving();
                _state.LogicalState = LogicalState.Unknown;
                _state.Countdown = 0;
            }
        }

        /// <summary>
        /// Transitions to the most appropriate state.
        /// </summary>
        /// <param name="laserData">most recently sensed laser range data</param>
        /// <param name="distance">closest obstacle in corridor ahead</param>
        private void UpdateLogicalState(sicklrf.State laserData, int distance)
        {
            if (_state.Countdown > 0)
            {
                _state.Countdown--;
            }
            else if (_state.IsUnknown)
            {
                StartMapping(laserData, distance);
            }
            else if (_state.IsMoving)
            {
                Move(laserData, distance);
            }
            else if (_state.IsMapping)
            {
                Map(laserData, distance);
            }
        }

        /// <summary>
        /// Implements the "Moving" meta state.
        /// </summary>
        /// <param name="laserData">most recently sensed laser range data</param>
        /// <param name="distance">closest obstacle in corridor ahead</param>
        private void Move(sicklrf.State laserData, int distance)
        {
            switch (_state.LogicalState)
            {
                case LogicalState.AdjustHeading:
                    AdjustHeading();
                    break;
                case LogicalState.FreeForwards:
                    AdjustVelocity(laserData, distance);
                    break;
                default:
                    LogInfo("Explorer.Move() called in illegal state");
                    break;
            }
        }

        /// <summary>
        /// Implements the "Mapping" meta state.
        /// </summary>
        /// <param name="laserData">most recently sensed laser range data</param>
        /// <param name="distance">closest obstacle in corridor ahead</param>
        private void Map(sicklrf.State laserData, int distance)
        {
            switch (_state.LogicalState)
            {
                case LogicalState.RandomTurn:
                    RandomTurn();
                    break;
                case LogicalState.MapSurroundings:
                    _state.Mapped = true;
                    LogInfo("Turning 180 deg to map");
                    Turn(180);

                    _state.LogicalState = LogicalState.MapSouth;
                    _state.Countdown = 15;
                    break;
                case LogicalState.MapSouth:
                    LogInfo("Mapping the View South");
                    _state.South = laserData;
                    Turn(180);

                    _state.LogicalState = LogicalState.MapNorth;
                    _state.Countdown = 15;
                    break;
                case LogicalState.MapNorth:
                    LogInfo("Mapping the View North");
                    _state.NewHeading = FindBestComposite(_state.South, laserData);
                    LogInfo("Map suggest turn: " + _state.NewHeading);
                    _state.South = null;
                    _state.LogicalState = LogicalState.AdjustHeading;
                    break;
                default:
                    LogInfo("Explorer.Map() called in illegal state");
                    break;
            }
        }

        /// <summary>
        /// Adjusts the velocity based on environment.
        /// </summary>
        /// <param name="laserData">most recently sensed laser range data</param>
        /// <param name="distance">closest obstacle in corridor ahead</param>
        private void AdjustVelocity(sicklrf.State laserData, int distance)
        {
            _state.Mapped = false;
            int test = FindBestFrom(laserData, 0, _state.Velocity / 10, CorridorWidthMoving);

            if (distance > FreeDistance)
            {
                MoveForward(MaximumForwardVelocity);

                if (Math.Abs(test) < 10)
                {
                    Turn(test / 2);
                }
            }
            else if (distance > AwareOfObstacleDistance)
            {
                MoveForward(MaximumForwardVelocity / 2);

                if (Math.Abs(test) < 45)
                {
                    Turn(test / 2);
                }
            }
            else
            {
                MoveForward(MaximumForwardVelocity / 4);

                Turn(test);
                _state.Countdown = Math.Abs(test / 10);
            }
        }

        /// <summary>
        /// Implements the "AdjustHeading" state.
        /// </summary>
        private void AdjustHeading()
        {
            LogInfo("Step Turning to: " + _state.NewHeading);
            Turn(_state.NewHeading);

            _state.LogicalState = LogicalState.FreeForwards;
            _state.Countdown = Math.Abs(_state.NewHeading / 10);
        }

        /// <summary>
        /// Implements the "RandomTurn" state.
        /// </summary>
        private void RandomTurn()
        {
            _state.NewHeading = new Random().Next(-115, 115);
            LogInfo("Start Turning (random) to: " + _state.NewHeading);
            Turn(_state.NewHeading);

            _state.LogicalState = LogicalState.Unknown;
            _state.Countdown = 2 + Math.Abs(_state.NewHeading / 10);
        }

        /// <summary>
        /// Transitions to "Mapping" meta state or "AdjustHeading" state depending on
        /// environment.
        /// </summary>
        /// <param name="laserData">most recently sensed laser range data</param>
        /// <param name="distance">closest obstacle in corridor ahead</param>
        private void StartMapping(sicklrf.State laserData, int distance)
        {
            StopMoving();

            if (distance < ObstacleDistance)
            {
                if (_state.Mapped)
                {
                    // We have been mapping before but do not seem to
                    // have found anything.
                    _state.LogicalState = LogicalState.RandomTurn;
                }
                else
                {
                    _state.LogicalState = LogicalState.MapSurroundings;
                }
            }
            else
            {
                int step = Math.Min(ObstacleDistance, distance - CorridorWidthMapping);
                // find the best angle from step mm in front of
                // our current position
                _state.NewHeading = FindBestFrom(laserData, 0, step, CorridorWidthMapping);

                LogInfo("Step: " + step + " Turn: " + _state.NewHeading);
                Translate(step);

                _state.LogicalState = LogicalState.AdjustHeading;
                _state.Countdown = step / 50 + Math.Abs(_state.NewHeading / 10);
            }
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
            if (_state.LogicalState != LogicalState.Unknown)
            {
                LogInfo("Stop requested: laser reported reset");
                StopMoving();

                _state.LogicalState = LogicalState.Unknown;
                _state.Countdown = 0;
            }
            update.ResponsePort.Post(DefaultUpdateResponseType.Instance);
        }

        /// <summary>
        /// Respresent a laser range finder reading
        /// </summary>
        class RangeData
        {
            /// <summary>
            /// Creates a new instance.
            /// </summary>
            /// <param name="distance">measured distance</param>
            /// <param name="heading">heading in degrees</param>
            public RangeData(int distance, double heading)
            {
                _distance = distance;
                _heading = heading;
            }

            int _distance;
            double _heading;

            /// <summary>
            /// Gets the distance in milimeters.
            /// </summary>
            public int Distance
            {
                get { return _distance; }
            }

            /// <summary>
            /// Gets the heading in degrees.
            /// </summary>
            public double Heading
            {
                get { return _heading; }
            }

            /// <summary>
            /// Comparer to sort instances by distance.
            /// </summary>
            /// <param name="first">first reading</param>
            /// <param name="second">second reading</param>
            /// <returns>a value les than 0 if  <paramref name="first"/> is closer than <paramref name="second"/>, 0 if both have the same distance, a value greater 0 otherwise</returns>
            static public int ByDistance(RangeData first, RangeData second)
            {
                return first._distance.CompareTo(second._distance);
            }
        }

        /// <summary>
        /// Finds the best free corridor (maximum free space ahead) in a 360 degree scan.
        /// </summary>
        /// <param name="south">the backward half of the scan</param>
        /// <param name="north">the forward half of the scan</param>
        /// <returns>beast heading in degrees</returns>
        private int FindBestComposite(sicklrf.State south, sicklrf.State north)
        {
            sicklrf.State composite = new sicklrf.State();

            composite.DistanceMeasurements = new int[720];

            for (int i = 0; i < 720; i++)
            {
                if (i < 180)
                {
                    composite.DistanceMeasurements[i] = south.DistanceMeasurements[i + 180];
                }
                else if (i < 540)
                {
                    composite.DistanceMeasurements[i] = north.DistanceMeasurements[i - 180];
                }
                else
                {
                    composite.DistanceMeasurements[i] = south.DistanceMeasurements[i - 540];
                }
            }

            composite.AngularResolution = 0.5;
            composite.AngularRange = 360;
            composite.Units = north.Units;

            return FindBestFrom(composite, 0, 0, CorridorWidthMoving);
        }

        /// <summary>
        /// Finds the best heading in a 180 degree laser scan
        /// </summary>
        /// <param name="laserData">laser scan</param>
        /// <param name="dx">horizontal offset</param>
        /// <param name="dy">vertical offset</param>
        /// <param name="width">width of corridor that must be free</param>
        /// <returns>best heading in degrees</returns>
        private int FindBestFrom(sicklrf.State laserData, int dx, int dy, int width)
        {
            int count = laserData.DistanceMeasurements.Length;
            double span = Math.PI * laserData.AngularRange / 180.0;

            List<RangeData> ranges = new List<RangeData>();

            for (int i = 0; i < count; i++)
            {
                int range = laserData.DistanceMeasurements[i];
                double angle = span * i / count - span / 2.0;

                double x = range * Math.Sin(angle) - dx;
                double y = range * Math.Cos(angle) - dy;

                angle = Math.Atan2(-x, y);
                range = (int)Math.Sqrt(x * x + y * y);

                ranges.Add(new RangeData(range, angle));
            }

            ranges.Sort(RangeData.ByDistance);

            for (int i = 0; i < ranges.Count; i++)
            {
                RangeData curr = ranges[i];

                double delta = Math.Atan2(width, curr.Distance);
                double low = curr.Heading - delta;
                double high = curr.Heading + delta;

                for (int j = i + 1; j < ranges.Count; j++)
                {
                    if (ranges[j].Heading > low &&
                        ranges[j].Heading < high)
                    {
                        ranges.RemoveAt(j);
                        j--;
                    }

                }
            }

            ranges.Reverse();

            int bestDistance = ranges[0].Distance;
            double bestHeading = ranges[0].Heading;
            Random rand = new Random();

            for (int i = 0; i < ranges.Count; i++)
            {
                if (ranges[i].Distance < bestDistance)
                {
                    break;
                }
                if (rand.Next(i + 1) == 0)
                {
                    bestHeading = ranges[i].Heading;
                }
            }

            return -(int)Math.Round(180 * bestHeading / Math.PI);
        }

        /// <summary>
        /// Finds closest obstacle in a corridor.
        /// </summary>
        /// <param name="laserData">laser scan</param>
        /// <param name="width">corridor width</param>
        /// <param name="fov">field of view in degrees</param>
        /// <returns>distance to the closest obstacle</returns>
        private int FindNearestObstacleInCorridor(sicklrf.State laserData, int width, int fov)
        {
            int index;
            int best = 8192;
            int count = laserData.DistanceMeasurements.Length;
            double rangeLow = -laserData.AngularRange / 2.0;
            double rangeHigh = laserData.AngularRange / 2.0;
            double span = laserData.AngularRange;

            for (index = 0; index < count; index++)
            {
                double angle = rangeLow + (span * index) / count;
                if (Math.Abs(angle) < fov)
                {
                    angle = angle * Math.PI / 180;

                    int range = laserData.DistanceMeasurements[index];
                    int x = (int)(range * Math.Sin(angle));
                    int y = (int)(range * Math.Cos(angle));

                    if (Math.Abs(x) < width)
                    {
                        if (range < best)
                        {
                            best = range;
                        }
                    }
                }
            }

            return best;
        }
        #endregion

        #region Drive helper method

        /// <summary>
        /// Sets the forward velocity of the drive.
        /// </summary>
        /// <param name="speed">velocity in mm/s</param>
        /// <returns>response port</returns>
        private PortSet<DefaultUpdateResponseType, Fault> MoveForward(int speed)
        {
            LogInfo(string.Format("MoveForward speed={0}", speed));
            if ((_state.DriveState == null || !_state.DriveState.IsEnabled) && speed != 0)
            {
                EnableMotor();
            }

            drive.SetDriveSpeedRequest request = new drive.SetDriveSpeedRequest();

            request.LeftWheelSpeed = (double)speed / 1000.0; // millimeters to meters
            request.RightWheelSpeed = (double)speed / 1000.0; // millimeters to meters

            return _drivePort.SetDriveSpeed(request);
        }

        /// <summary>
        /// Turns the drive relative to its current heading.
        /// </summary>
        /// <param name="angle">angle in degrees</param>
        /// <returns>response port</returns>
        PortSet<DefaultUpdateResponseType, Fault> Turn(int angle)
        {
            if (_state.DriveState == null || !_state.DriveState.IsEnabled)
            {
                EnableMotor();
            }

            drive.RotateDegreesRequest request = new drive.RotateDegreesRequest();
            request.Degrees = (double)(-angle);

            return _drivePort.RotateDegrees(request);
        }

        /// <summary>
        /// Moves the drive forward for the specified distance.
        /// </summary>
        /// <param name="step">distance in mm</param>
        /// <returns>response port</returns>
        PortSet<DefaultUpdateResponseType, Fault> Translate(int step)
        {
            if (_state.DriveState == null || !_state.DriveState.IsEnabled)
            {
                EnableMotor();
            }

            drive.DriveDistanceRequest request = new drive.DriveDistanceRequest();
            request.Distance = (double)step / 1000.0; // millimeters to meters

            return _drivePort.DriveDistance(request);
        }

        /// <summary>
        /// Sets the velocity of the drive to 0.
        /// </summary>
        /// <returns></returns>
        PortSet<DefaultUpdateResponseType, Fault> StopMoving()
        {
            return MoveForward(0);
        }

        /// <summary>
        /// Sets the turning velocity to 0.
        /// </summary>
        /// <returns>response port</returns>
        PortSet<DefaultUpdateResponseType, Fault> StopTurning()
        {
            return Turn(0);
        }

        /// <summary>
        /// Enables the drive
        /// </summary>
        /// <returns>response port</returns>
        PortSet<DefaultUpdateResponseType, Fault> EnableMotor()
        {
            return EnableMotor(true);
        }

        /// <summary>
        /// Disables the drive
        /// </summary>
        /// <returns>repsonse port</returns>
        PortSet<DefaultUpdateResponseType, Fault> DisableMotor()
        {
            return EnableMotor(false);
        }

        /// <summary>
        /// Sets the drives enabled state.
        /// </summary>
        /// <param name="enable">new enables state</param>
        /// <returns>response port</returns>
        PortSet<DefaultUpdateResponseType, Fault> EnableMotor(bool enable)
        {
            drive.EnableDriveRequest request = new drive.EnableDriveRequest();
            request.Enable = enable;

            return _drivePort.EnableDrive(request);
        }
        #endregion
    }
}