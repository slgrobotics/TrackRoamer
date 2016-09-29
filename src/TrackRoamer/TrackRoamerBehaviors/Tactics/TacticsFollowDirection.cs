using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.ComponentModel;

using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
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
using sicklrf = Microsoft.Robotics.Services.Sensors.SickLRF.Proxy;
using kinect = Microsoft.Kinect;

using dssp = Microsoft.Dss.ServiceModel.Dssp;

using TrackRoamer.Robotics.Utility.LibSystem;
using TrackRoamer.Robotics.LibMapping;
using TrackRoamer.Robotics.LibBehavior;

namespace TrackRoamer.Robotics.Services.TrackRoamerBehaviors
{
    partial class TrackRoamerBehaviorsService : DsspServiceBase
    {
        // externally defined parameters:
        private double FollowDirectionMaxVelocityMmSec = 0.0d;                  // desired value, negative if moving backwards
        private double? FollowDirectionTargetDistanceToGoalMeters = null;       // desired value - we want to be at this distance to the target

        private DateTime lastPrintedTurnRelative = DateTime.Now;

        #region FollowDirection() tactics

        private void TacticsFollowDirection()
        {
            Direction curDir = null;
            //bool performedAvoidCollision = false;

            if (_state.MovingState == MovingState.BumpedBackingUp)
            {
                Tracer.Trace("TacticsFollowDirection waiting MovingState " + _state.MovingState + " to finish");
                return;
            }
            //else if (_mapperVicinity.robotState.doVicinityPlanning && FollowDirectionMaxVelocityMmSec > 0.01d)
            //{
            //    if (_currentRoutePlan != null && _currentRoutePlan.isGoodPlan)
            //    {
            //        // AvoidCollision and EnterOpenSpace have precedence over
            //        // all other state transitions and are thus handled first.
            //        bool canMove = PerformAvoidCollision(_currentRoutePlan);
            //        if (!canMove)
            //        {
            //            StopMoving();
            //            _state.MovingState = MovingState.Unable;
            //            _state.Countdown = 0;   // 0 = immediate response
            //        }
            //        performedAvoidCollision = true;

            //        // at this point constraints and allowable speeds and distances are known and stored in _state.collisionState
            //        // we must use them while executing the plan.

            //        // good place to analyze the plan against constraints and set MovingState.Unable
            //        _currentRoutePlan = critiquePlanChain(_currentRoutePlan);

            //        if (_currentRoutePlan != null && _currentRoutePlan.isGoodPlan)
            //        {
            //            double? distanceToGoalMeters = _mapperVicinity.robotDirection.distanceToGoalMeters;     // can be null, but not negative

            //            if (distanceToGoalMeters.HasValue)
            //            {
            //                distanceToGoalMeters = Math.Min(distanceToGoalMeters.Value, _currentRoutePlan.legMeters.Value);
            //            }
            //            else
            //            {
            //                distanceToGoalMeters = _currentRoutePlan.legMeters.Value;
            //            }
            //            curDir = new Direction() {
            //                heading = _mapperVicinity.robotDirection.heading,
            //                bearing = _currentRoutePlan.bestHeading,
            //                distanceToGoalMeters = distanceToGoalMeters
            //            };
            //        }
            //        else
            //        {
            //            Tracer.Trace("Best Plan no good after critique");
            //            _state.MovingState = MovingState.Unable;    // no good plan in the chain
            //        }
            //    }
            //    else
            //    {
            //        Tracer.Trace("Best Plan no good");
            //        _state.MovingState = MovingState.Unable;        // no good plan in the chain
            //        //curDir = _mapperVicinity.robotDirection;      // also in _currentGoalBearing
            //    }
            //}
            //else
            {
                // no planning needed - by the setting or while backing up. Ignore _currentRoutePlan
                //bool canMove = PerformAvoidCollision(null);
                //if (!canMove)
                //{
                //    StopMoving();
                //    _state.MovingState = MovingState.Unable;
                //    _state.Countdown = 0;   // 0 = immediate response
                //}
                //performedAvoidCollision = true;

                // actual distance to target:
                double? distanceToGoalMeters = _mapperVicinity.robotDirection.distanceToGoalMeters;     // can be null, but not negative

                curDir = new Direction() {
                    heading = _mapperVicinity.robotDirection.heading,   // robot heading
                    bearing = _mapperVicinity.robotDirection.bearing,   // bearing to target
                    distanceToGoalMeters = distanceToGoalMeters         // actual distance to target
                };
                //distanceToGoalMeters = _state.collisionState.canMoveBackwardsDistanceMm / 1000.0d;
            }

            if (curDir != null && curDir.bearing.HasValue && curDir.bearingRelative.HasValue )
            {
                // Strategy asks us to turn or/and drive. We assume that collision avoidance has been taken care of.
                // this is a marching order, no thinking.

                //LogHistory(10, "Follow Direction:  bearing=" + curDir.bearing + "   heading=" + curDir.heading);

                //Tracer.Trace("**************  Follow Direction:  bearing=" + curDir.bearing + "   turnRelative=" + curDir.turnRelative + "   heading=" + curDir.heading);

                double turnRelative = curDir.turnRelative;          // degrees  -180...180
                bool steepTurn = Math.Abs(turnRelative) > 40.0d;

                if ((DateTime.Now - lastPrintedTurnRelative).TotalSeconds > 5.0d)
                {
                    lastPrintedTurnRelative = DateTime.Now;
                    //Tracer.Trace("------------- turnRelative=" + turnRelative + "    steepTurn=" + steepTurn);
                    //Talker.Say(10, "" + Math.Round(turnRelative) + (steepTurn ? " steep" : ""));
                    //Talker.Say(10, "" + Math.Round(_mapperVicinity.robotDirection.bearing));
                }

                long nowTicks = DateTime.Now.Ticks;

                // call Angular Speed PID controller:
                _state.followDirectionPidControllerAngularSpeed.Update(turnRelative, nowTicks);

                double angularPidRate = _state.followDirectionPidControllerAngularSpeed.CalculateControl();
                double? linearPidRate = null; // -100.0 ... +100.0
                double distanceToCoverMm = 0.0d;

                //Tracer.Trace("Follow Direction:  steepTurn=" + steepTurn + "  curDir.distanceToGoalMeters=" + curDir.distanceToGoalMeters + "   FollowDirectionTargetDistanceToGoalMeters=" + FollowDirectionTargetDistanceToGoalMeters);

                if (!steepTurn && curDir.distanceToGoalMeters.HasValue && FollowDirectionTargetDistanceToGoalMeters.HasValue)
                {
                    distanceToCoverMm = (curDir.distanceToGoalMeters.Value - FollowDirectionTargetDistanceToGoalMeters.Value) * 1000.0d;

                    //Tracer.Trace("Follow Direction:  distanceToCoverMm=" + distanceToCoverMm);

                    // call Linear Speed PID controller:
                    _state.followDirectionPidControllerLinearSpeed.Update(distanceToCoverMm, nowTicks);

                    linearPidRate = _state.followDirectionPidControllerLinearSpeed.CalculateControl();
                }

                //Tracer.Trace("Follow Direction:  steepTurn=" + steepTurn + "  turnRelative=" + turnRelative + "  TargetDistanceToGoal=" + FollowDirectionTargetDistanceToGoalMeters.Value + "  distanceToGoal=" + curDir.distanceToGoalMeters
                //                + "  distanceToCover=" + distanceToCoverMm + "     PID:  angularPidRate=" + angularPidRate + "  linearPidRate=" + linearPidRate);

                double absFollowDirectionMaxVelocityMmSec = Math.Abs(FollowDirectionMaxVelocityMmSec);

                double linearPower = steepTurn || !linearPidRate.HasValue ? 0.0d : (linearPidRate.Value * absFollowDirectionMaxVelocityMmSec / 1000000.0d);      // power to speedMmSec ratio is 1000, linear PID rate is within the range -1000...+1000

                // convert back to left/right power, which must be within the range -1.0 ... +1.0 and will be limited to this range by SetDrivePower()

                // angular pid rate is within the range +-MaxPidValue in the followDirectionPidController
                double angularPower = (angularPidRate / _state.followDirectionPidControllerAngularSpeed.MaxPidValue) * ModerateTurnPower;

                double leftSpeed  = (linearPower + angularPower) * 1000.0d;
                double rightSpeed = (linearPower - angularPower) * 1000.0d;

                //Tracer.Trace("Follow Direction:  angularPower=" + angularPower + "  linearPower=" + linearPower);

                double wheelSpeedLimitMmSec = absFollowDirectionMaxVelocityMmSec < 0.01d ? MaximumForwardVelocityMmSec : absFollowDirectionMaxVelocityMmSec;

                SetDriveSpeed(
                    Math.Sign(leftSpeed) * Math.Min(Math.Abs(leftSpeed), wheelSpeedLimitMmSec),
                    Math.Sign(rightSpeed) * Math.Min(Math.Abs(rightSpeed), wheelSpeedLimitMmSec)
                    );
            }
            else //if (!performedAvoidCollision)
            {
                // We are not tasked by Strategy with turning or driving. Strategy could be unsure or unable to do anything, collision state is unknown.
                // We still might be moving, and if that's dangerous - we need to stop.
                bool canMove = PerformAvoidCollision(null);
                if (!canMove)
                {
                    StopMoving();
                    _state.MovingState = MovingState.Unable;
                    _state.Countdown = 0;   // 0 = immediate response
                }
            }
        }

        #endregion // FollowDirection() tactics
    }
}
