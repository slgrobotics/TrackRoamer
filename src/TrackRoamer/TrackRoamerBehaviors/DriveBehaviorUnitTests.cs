//#define TRACEDEBUG
//#define TRACEDEBUGTICKS
//#define TRACELOG

using System;
using System.Collections.Generic;
using System.Linq;
using System.ComponentModel;

using System.IO;
using System.Net;
using System.Net.Mime;

using W3C.Soap;
using Microsoft.Ccr.Core;
using Microsoft.Dss.Core;
using Microsoft.Dss.Core.Attributes;
using Microsoft.Dss.ServiceModel.Dssp;
using Microsoft.Dss.Services.Serializer;
using Microsoft.Dss.ServiceModel.DsspServiceBase;
using Microsoft.Dss.Core.DsspHttp;
using Microsoft.Dss.Core.DsspHttpUtilities;

using bumper = Microsoft.Robotics.Services.ContactSensor.Proxy;
//using bumper = TrackRoamer.Robotics.Services.TrackRoamerServices.Bumper.Proxy;
using drive = Microsoft.Robotics.Services.Drive.Proxy;
using trdrive = TrackRoamer.Robotics.Services.TrackRoamerDrive.Proxy;
using encoder = Microsoft.Robotics.Services.Encoder.Proxy;
using sicklrf = Microsoft.Robotics.Services.Sensors.SickLRF.Proxy;
using powerbrick = TrackRoamer.Robotics.Services.TrackRoamerBrickPower.Proxy;
using proxibrick = TrackRoamer.Robotics.Services.TrackRoamerBrickProximityBoard.Proxy;
using pololumaestro = TrackRoamer.Robotics.Hardware.PololuMaestroService.Proxy;

using dssp = Microsoft.Dss.ServiceModel.Dssp;

using TrackRoamer.Robotics.LibMapping;
using TrackRoamer.Robotics.Utility.LibSystem;

namespace TrackRoamer.Robotics.Services.TrackRoamerBehaviors
{
	/// <summary>
	/// takes care of all the DSS Service plumbing to allow derived class handle higher level behavior related code
	/// </summary>
    partial class TrackRoamerBehaviorsService : DsspServiceBase
    {
        #region Unit Testing

        //protected bool _doUnitTest = true;                    // "true" disables laser and calls simple behaviors like "Straight" or "Square" to verify that the plumbing works.
        //protected bool _doDecisionStraightForward = true;     // "true" disables Decide so that it always decides to hust move straight forward. Useful for timing evaluation and bumper debugging.

        protected bool _doUnitTest = false;                     // "true" disables laser and calls simple behaviors like "Straight" or "Square" to verify that the plumbing works.
        protected bool _unitTestSensorsOn = false;               // "true" will leave laser, proximity and other sensors on; false turns them all off.
        protected bool _doDecisionStraightForward = false;      // "true" disables Decide so that it always decides to just move straight forward. Useful for timing evaluation and bumper debugging.
        protected bool _doDecisionDontMove = false;             // "true" disables Decide so that it does nothing, and no commands are issued. Useful for wheel encoder and bumper debugging.

        protected bool _doSimulatedLaser = false;               // "true" disables real laser and replaces it with simulated wide-open laser view
        protected bool _simulatedLaserRandomize = true;         // if _doSimulatedLaser=true, some randomization is imposed on laser frames, otherwise it is wide open.
        protected int _simulatedLaserWatchdogInterval = 10000;  // if _doSimulatedLaser=true, a frame comes every ... ms
        protected bool _testBumpMode = false;	                // "true" disables Decide and disregards moving speed to allow debugging (while stationary) of simple moves activated by events from the Bumper/Whiskers 


        double utForwardVelocity;
        double utTurnPower;
        double utPowerScale = 1.0d;

        const int repeatCount = 1;          // Number of times to repeat behavior

        // Values for "exact" movements using DriveDistance and RotateDegrees
        protected float driveDistanceMeters = 10.0f * (float)Distance.METERS_PER_FOOT;     // Drive 0.5f = 50cm   5.0f = 5m
        protected float rotateAngle = 90.0f;            // Turn 90 degrees to the right (+) or left (-)

        // this will be called after all initialization:
        private void performUnitTest()
        {
            LogInfo("DriveBehaviorServiceBase: performUnitTest() Started");

            utForwardVelocity = ModerateForwardVelocityMmSec;
            utTurnPower = ModerateTurnPower;

            if (_state.collisionState == null)
            {
                _state.collisionState = new CollisionState();
            }

            // to test drive operation, execute the geometric pattern

            //SpawnIterator(BehaviorMoveForward);

            //SpawnIterator(BehaviorExercisePololuMaestro);

            //SpawnIterator(BehaviorPushForward);

            SpawnIterator(BehaviorTurn);

            //SpawnIterator(BehaviorTurnAndMoveForward);

            //SpawnIterator(BehaviorSquare);

            //SpawnIterator(BehaviorStraight);

            // SpawnIterator(BehaviorStraightInterruptTurn);

            //SpawnIterator(BehaviorKata);

            LogInfo("DriveBehaviorServiceBase: performUnitTest() finished");
        }

        #region BehaviorExercisePololuMaestro

        // Iterator to execute the Behavior
        // It is important to use an Iterator so that it can relinquish control
        // when there is nothing to do, i.e. yield return
        protected IEnumerator<ITask> BehaviorExercisePololuMaestro()
        {
            LogInfo("DriveBehaviorServiceBase: BehaviorExercisePololuMaestro() Started");

            Talker.Say(10, "waiting  for Behavior Exercise Pololu Maestro");

            // Wait for the robot to initialize, otherwise it will miss the initial command
            for (int i = 10; i > 0; i--)
            {
                LogInfo(LogGroups.Console, i.ToString());
                yield return Timeout(1000);
            }

            Talker.Say(10, "starting Behavior Exercise Pololu Maestro");

            // Wait for settling time
            yield return Timeout(settlingTime);

            byte channel = ServoChannelMap.leftGunTilt;

            for (int i = 1; i <= 50; i++)
            {
                int servoPos = 1000 + 20 * i;

                //Talker.Say(10, "servo " + servoPos);

                pololumaestro.ChannelValuePair cvp = new pololumaestro.ChannelValuePair() { Channel = channel, Target = (ushort)(servoPos * 4) };

                List<pololumaestro.ChannelValuePair> channelValues = new List<pololumaestro.ChannelValuePair>();

                channelValues.Add(cvp);

                pololumaestro.PololuMaestroCommand cmd = new pololumaestro.PololuMaestroCommand() { Command = "set", ChannelValues = channelValues };

                _pololuMaestroPort.Post(new pololumaestro.SendPololuMaestroCommand(cmd));

                // wait some time
                yield return Timeout(1000);
            }

            Talker.Say(10, "Behavior Exercise Pololu Maestro finished");

            // done
            yield break;
        }
        #endregion // BehaviorExercisePololuMaestro

        #region BehaviorKata

        protected IEnumerator<ITask> BehaviorKata()
        {
            LogInfo("DriveBehaviorServiceBase: BehaviorKata() Started");

            Talker.Say(2, "starting Behavior Kata");

            // Wait for settling time
            yield return Timeout(15000);

            /*
            SpawnIterator<TurnAndMoveParameters, Handler>(
                new TurnAndMoveParameters()
                {
                    rotateAngle = 30,
                    rotatePower = MaximumTurnPower,
                    speed = (int)Math.Round(ModerateForwardVelocity),
                    desiredMovingState = MovingState.FreeForwards
                },
                delegate()
                {
                },
                TurnAndMoveForward);
             * */

            /*
            Kata kata = new Kata() { name="My Kata" };

            kata.Add(new KataStep()
                        {
                            name = "Turn 30 backup 400",
                            rotateAngle = 30,
                            rotatePower = MaximumTurnPower,
                            speed = (int)Math.Round(ModerateForwardVelocity),
                            distance = -400,
                            desiredMovingState = MovingState.Unknown
                        }
                    );

            kata.Add(new KataStep()
                        {
                            name = "Turn -30 forward 400",
                            rotateAngle = -30,
                            rotatePower = MaximumTurnPower,
                            speed = (int)Math.Round(ModerateForwardVelocity),
                            distance = 400,
                            desiredMovingState = MovingState.Unknown
                        }
                    );
            */

            Kata kata = KataHelper.KataByName("avoid.*").FirstOrDefault();

            if (kata != null)
            {
                Talker.Say(5, "kata " + kata.name);

                SpawnIterator<Kata, Handler>(
                    kata,
                    delegate()
                    {
                        Talker.Say(5, "kata success: " + kata.success + "  count: " + kata.successfulStepsCount);
                    },
                    KataRunner);
            }
            else
            {
                LogError("DriveBehaviorServiceBase: BehaviorKata() - cannot find appropriate Kata");
            }

            LogInfo("DriveBehaviorServiceBase: BehaviorKata() finished");
        }

        #endregion // BehaviorKata

        #region BehaviorMoveForward

        // Iterator to execute the Behavior
        // It is important to use an Iterator so that it can relinquish control
        // when there is nothing to do, i.e. yield return
        protected IEnumerator<ITask> BehaviorMoveForward()
        {
            LogInfo("DriveBehaviorServiceBase: BehaviorMoveForward() Started");

            Talker.Say(10, "waiting  for Behavior Move Forward");

            // Wait for the robot to initialize, otherwise it will
            // miss the initial command
            for (int i = 10; i > 0; i--)
            {
                LogInfo(LogGroups.Console, i.ToString());
                yield return Timeout(1000);
            }

            // Wait for settling time
            yield return Timeout(settlingTime);

            Talker.Say(10, "starting Behavior Move Forward");

            double speedMms = utForwardVelocity * utPowerScale;

            // a fire-and-forget command to move forward:
            SetDriveSpeed(speedMms, speedMms);

            // wait some time
            for (int i = 10; i > 0; i--)
            {
                LogInfo(i.ToString());
                yield return Timeout(1000);
            }

            // we expect the drive to stop at the command, not by completion:
            StopMoving();

            Talker.Say(10, "Behavior Move Forward finished");

            // done
            yield break;
        }
        #endregion // BehaviorMoveForward

        #region BehaviorPushForward

        // Iterator to execute the Behavior
        // It is important to use an Iterator so that it can relinquish control
        // when there is nothing to do, i.e. yield return
        protected IEnumerator<ITask> BehaviorPushForward()
        {
            LogInfo("DriveBehaviorServiceBase: BehaviorPushForward() Started");

            Talker.Say(10, "waiting  for Behavior Push Forward");

            // Wait for the robot to initialize, otherwise it will
            // miss the initial command
            for (int i = 10; i > 0; i--)
            {
                LogInfo(LogGroups.Console, i.ToString());
                yield return Timeout(1000);
            }

            Talker.Say(10, "starting Behavior Push Forward");

            // Wait for settling time
            yield return Timeout(settlingTime);

            for (int i = 1; i <= 50; i++)
            {
                Talker.Say(10, "push " + i);

                double speedMms = utForwardVelocity * utPowerScale;

                // a fire-and-forget command to move forward:
                SetDriveSpeed(speedMms, speedMms);

                // wait some time
                LogInfo(i.ToString());
                yield return Timeout(2000);
            }

            // we expect the drive to stop at the command, not by completion:
            StopMoving();

            Talker.Say(10, "Behavior Push Forward finished");

            // done
            yield break;
        }
        #endregion // BehaviorPushForward

        #region BehaviorTurnAndMoveForward

        // Iterator to execute the Behavior
        // It is important to use an Iterator so that it can relinquish control
        // when there is nothing to do, i.e. yield return
        protected IEnumerator<ITask> BehaviorTurnAndMoveForward()
        {
            LogInfo("DriveBehaviorServiceBase: BehaviorTurnAndMoveForward() Started");

            Talker.Say(2, "starting Behavior Turn And Move Forward");

            // Wait for settling time
            yield return Timeout(settlingTime);

            LogInfo(LogGroups.Console, "Turning " + (rotateAngle > 0.0d ? "Right " : "Left "));

            bool success = true;
            Fault fault = null;

            // First turn:
            yield return Arbiter.Choice(
                TurnByAngle((int)rotateAngle, utTurnPower * utPowerScale),
                delegate(DefaultUpdateResponseType response) { success = true; },
                delegate(Fault f) { success = false; fault = f; }
            );

            // If the RotateDegrees was accepted, then wait for it to complete.
            // It is important not to wait if the request failed.
            if (success)
            {
                DriveStageContainer driveStage = new DriveStageContainer();
                yield return WaitForCompletion(driveStage);
                LogInfo("WaitForCompletion() returned: " + driveStage.DriveStage);

                if (_mapperVicinity.turnState != null)
                {
                    _mapperVicinity.turnState.finished = DateTime.Now;
                    _mapperVicinity.turnState.hasFinished = true;
                }
                success = driveStage.DriveStage == drive.DriveStage.Completed;
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
            }

            if (success)
            {
                // Wait for settling time
                //yield return Timeout(settlingTime);

                double speedMms = utForwardVelocity * utPowerScale;

                // a fire-and-forget command to move forward:
                SetDriveSpeed(speedMms, speedMms);

                // wait some time
                for (int i = 10; i > 0; i--)
                {
                    LogInfo(i.ToString());
                    yield return Timeout(1000);
                }

                // we expect the drive to stop at the command, not by completion:
                StopMoving();

                Talker.Say(2, "Behavior Turn And Move Forward finished");
            }
            else
            {
                Talker.Say(2, "Behavior Turn And Move Forward canceled");
            }

            // done
            yield break;
        }
        #endregion // BehaviorTurnAndMoveForward

        #region BehaviorTurn

        // Iterator to execute the Behavior
        // It is important to use an Iterator so that it can relinquish control
        // when there is nothing to do, i.e. yield return
        protected IEnumerator<ITask> BehaviorTurn()
        {
            LogInfo("DriveBehaviorServiceBase: BehaviorTurn() Started");

            Talker.Say(2, "starting Behavior Turn And Move Forward");

            // Wait for the robot to initialize, otherwise it will
            // miss the initial command
            for (int i = 10; i > 0; i--)
            {
                LogInfo(LogGroups.Console, i.ToString());
                yield return Timeout(1000);
            }

            LogInfo(LogGroups.Console, "Turning " + (rotateAngle > 0.0d ? "Right " : "Left "));

            bool success = true;
            Fault fault = null;

            // Turn:
            yield return Arbiter.Choice(
                TurnByAngle((int)rotateAngle, utTurnPower * utPowerScale),
                delegate(DefaultUpdateResponseType response) { success = true; },
                delegate(Fault f) { success = false; fault = f; }
            );

            // If the RotateDegrees was accepted, then wait for it to complete.
            // It is important not to wait if the request failed.
            if (success)
            {
                DriveStageContainer driveStage = new DriveStageContainer();
                yield return WaitForCompletion(driveStage);
                LogInfo("WaitForCompletion() returned: " + driveStage.DriveStage);

                if (_mapperVicinity.turnState != null)
                {
                    _mapperVicinity.turnState.finished = DateTime.Now;
                    _mapperVicinity.turnState.hasFinished = true;
                }
                success = driveStage.DriveStage == drive.DriveStage.Completed;
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
            }

            // done
            yield break;
        }
        #endregion // BehaviorTurn

        #region BehaviorStraight

        // Iterator to execute the Behavior
        // It is important to use an Iterator so that it can relinquish control
        // when there is nothing to do, i.e. yield return
        protected IEnumerator<ITask> BehaviorStraight()
        {
            LogInfo("DriveBehaviorServiceBase: BehaviorStraight() Started");

            // Wait for the robot to initialize, otherwise it will miss the initial command
            for (int i = 10; i > 0; i--)
            {
                LogInfo(LogGroups.Console, i.ToString());
                yield return Timeout(1000);
            }

            Talker.Say(2, "starting Behavior Straight");

            LogInfo("Starting STRAIGHT using Controlled Moves ...");

            // Make sure that the drive is enabled first!
            //EnableMotor();

            for (int times = 1; times <= repeatCount; times++)
            {
                // Wait for settling time
                yield return Timeout(settlingTime);

                // This code uses the Translate operation to control the robot. These are not precise,
                // but they should be better than using timers and they should also work regardless of the type of robot.

                bool success = true;
                Fault fault = null;

                LogInfo("Drive Straight Ahead  - starting step " + times);

                // Drive straight ahead
                yield return Arbiter.Choice(
                    Translate((int)(driveDistanceMeters * 1000.0d), utForwardVelocity * utPowerScale),
                    delegate(DefaultUpdateResponseType response) { success = true; },
                    delegate(Fault f) { success = false; fault = f; }
                );

                // If the DriveDistance was accepted, then wait for it to complete.
                // It is important not to wait if the request failed.
                // NOTE: This approach only works if you always wait for a
                // completion message. If you send any other drive request
                // while the current one is active, then the current motion
                // will be canceled, i.e. cut short.
                if (success)
                {
                    DriveStageContainer driveStage = new DriveStageContainer();
                    yield return WaitForCompletion(driveStage);
                    LogInfo("WaitForCompletion() returned: " + driveStage.DriveStage);

                    success = driveStage.DriveStage == drive.DriveStage.Completed;
                }
                else
                {
                    LogError("Error occurred on Translate: " + fault);
                }

                LogInfo("Drive Straight Ahead  - finished step " + times);
            }

            // And finally make sure that the robot is stopped!
            //StopMoving();

            LogInfo("STRAIGHT Finished, robot stopped");

            Talker.Say(2, "Behavior Straight finished");

            yield break;
        }
        #endregion // BehaviorStraight

        #region BehaviorStraightInterruptTurn

        // Iterator to execute the Behavior
        // It is important to use an Iterator so that it can relinquish control
        // when there is nothing to do, i.e. yield return
        protected IEnumerator<ITask> BehaviorStraightInterruptTurn()
        {
            LogInfo("DriveBehaviorServiceBase: BehaviorStraightInterruptTurn() Started ----------------------------------------------");

            //Talker.Say(2, "starting Behavior Straight Interrupt Turn");

            for (int times = 1; times <= repeatCount; times++)
            {
                SpawnIterator(BehaviorStraightInterruptTurn_Straight);

                yield return Timeout(10000);

                SpawnIterator(BehaviorStraightInterruptTurn_Turn);

                LogInfo("DriveBehaviorServiceBase: BehaviorStraightInterruptTurn() - finished step " + times);
            }

            LogInfo("DriveBehaviorServiceBase: BehaviorStraightInterruptTurn()  Finished ----------------------------------------------");

            //Talker.Say(2, "Behavior Straight Interrupt Turn finished");

            yield break;
        }

        protected IEnumerator<ITask> BehaviorStraightInterruptTurn_Straight()
        {
            LogInfo("DriveBehaviorServiceBase: BehaviorStraightInterruptTurn_Straight() Started");

            bool success = true;
            Fault fault = null;

            // Drive straight ahead
            yield return Arbiter.Choice(
                // 3 meters takes about 30 seconds:
                Translate((int)(3.0d * 1000.0d), utForwardVelocity * utPowerScale),
                delegate(DefaultUpdateResponseType response) { success = true; },
                delegate(Fault f) { success = false; fault = f; }
            );

            // If the DriveDistance was accepted, then wait for it to complete.
            // It is important not to wait if the request failed.
            if (success)
            {
                DriveStageContainer driveStage = new DriveStageContainer();
                yield return WaitForCompletion(driveStage);
                LogInfo("WaitForCompletion() returned: " + driveStage.DriveStage);

                success = driveStage.DriveStage == drive.DriveStage.Completed;
            }
            else
            {
                LogError("Error occurred on Translate: " + fault);
            }

            if (success)
            {
                LogInfo("BehaviorStraightInterruptTurn_Straight() Finished --------");
            }
            else
            {
                LogInfo("BehaviorStraightInterruptTurn_Straight() Canceled --------");
            }

            yield break;
        }

        // Iterator to execute the Behavior
        // It is important to use an Iterator so that it can relinquish control
        // when there is nothing to do, i.e. yield return
        protected IEnumerator<ITask> BehaviorStraightInterruptTurn_Turn()
        {
            LogInfo("DriveBehaviorServiceBase: BehaviorStraightInterruptTurn_Turn() Started");

            bool success = true;
            Fault fault = null;

            LogInfo(LogGroups.Console, "Turning " + (rotateAngle > 0.0d ? "Right " : "Left "));

            // turn first:
            yield return Arbiter.Choice(
                TurnByAngle((int)rotateAngle, utTurnPower * utPowerScale),
                delegate(DefaultUpdateResponseType response) { success = true; },
                delegate(Fault f) { success = false; fault = f; }
            );

            // If the RotateDegrees was accepted, then wait for it to complete.
            // It is important not to wait if the request failed.
            if (success)
            {
                DriveStageContainer driveStage = new DriveStageContainer();
                yield return WaitForCompletion(driveStage);
                LogInfo("WaitForCompletion() returned: " + driveStage.DriveStage);

                if (_mapperVicinity.turnState != null)
                {
                    _mapperVicinity.turnState.finished = DateTime.Now;
                    _mapperVicinity.turnState.hasFinished = true;
                }
                success = driveStage.DriveStage == drive.DriveStage.Completed;
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
            }

            if (success)
            {
                LogInfo("BehaviorStraightInterruptTurn_Turn() Finished --------");
            }
            else
            {
                LogInfo("BehaviorStraightInterruptTurn_Turn() Canceled --------");
            }

            yield break;
        }
        #endregion // BehaviorStraightInterruptTurn

        #region BehaviorSquare

        // Iterator to execute the Behavior
        // It is important to use an Iterator so that it can relinquish control
        // when there is nothing to do, i.e. yield return
        IEnumerator<ITask> BehaviorSquare()
        {
            LogInfo(LogGroups.Console, "DriveBehaviorServiceBase: BehaviorSquare Started");

            Talker.Say(2, "starting Behavior Square");

            // Wait for the robot to initialize, otherwise it will
            // miss the initial command
            for (int i = 10; i > 0; i--)
            {
                LogInfo(LogGroups.Console, i.ToString());
                yield return Timeout(1000);
            }

            LogInfo(LogGroups.Console, "Starting SQUARE using Controlled Moves ...");

            // Make sure that the drive is enabled first!
            //EnableMotor();

            int times = 1;
            for (; times <= repeatCount; times++)
            {
                // Drive along the four sides of a square
                for (int side = 0; side < 4; side++)
                {
                    bool success = true;
                    Fault fault = null;

                    LogInfo(LogGroups.Console, "Driving Straight Ahead - side " + side);

                    // Drive straight ahead
                    yield return Arbiter.Choice(
                        Translate((int)(driveDistanceMeters * 1000.0d), utForwardVelocity * utPowerScale),
                        delegate(DefaultUpdateResponseType response) { success = true; },
                        delegate(Fault f) { success = false; fault = f; }
                    );

                    // If the DriveDistance was accepted, then wait for it to complete.
                    // It is important not to wait if the request failed.
                    // NOTE: This approach only works if you always wait for a
                    // completion message. If you send any other drive request
                    // while the current one is active, then the current motion
                    // will be canceled, i.e. cut short.
                    if (success)
                    {
                        DriveStageContainer driveStage = new DriveStageContainer();
                        yield return WaitForCompletion(driveStage);
                        LogInfo("WaitForCompletion() returned: " + driveStage.DriveStage);

                        success = driveStage.DriveStage == drive.DriveStage.Completed;
                    }
                    else
                    {
                        LogError("Error occurred on Translate: " + fault);
                    }

                    // Wait for settling time
                    yield return Timeout(settlingTime);

                    LogInfo(LogGroups.Console, "Turning " + (rotateAngle > 0.0d ? "Right " : "Left ") + " - side " + side);

                    // Now turn:
                    yield return Arbiter.Choice(
                        TurnByAngle((int)rotateAngle, utTurnPower * utPowerScale),
                        delegate(DefaultUpdateResponseType response) { success = true; },
                        delegate(Fault f) { success = false; fault = f; }
                    );

                    // If the RotateDegrees was accepted, then wait for it to complete.
                    // It is important not to wait if the request failed.
                    if (success)
                    {
                        DriveStageContainer driveStage = new DriveStageContainer();
                        yield return WaitForCompletion(driveStage);
                        LogInfo("WaitForCompletion() returned: " + driveStage.DriveStage);

                        if (_mapperVicinity.turnState != null)
                        {
                            _mapperVicinity.turnState.finished = DateTime.Now;
                            _mapperVicinity.turnState.hasFinished = true;
                        }
                        success = driveStage.DriveStage == drive.DriveStage.Completed;
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
                    }

                    // Wait for settling time
                    yield return Timeout(settlingTime);
                }
            }

            // And finally make sure that the robot is stopped!
            //StopMoving();

            LogInfo(LogGroups.Console, "BehaviorSquare Finished after completing " + (times-1) + " cycles; robot stopped");

            Talker.Say(2, "Behavior Square finished");

            yield break;
        }

        #endregion // BehaviorSquare           

        #endregion // Unit Testing

    }
}
