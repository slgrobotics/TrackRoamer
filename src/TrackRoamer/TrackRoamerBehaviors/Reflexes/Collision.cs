using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.ComponentModel;

using Microsoft.Ccr.Core;
using Microsoft.Dss.ServiceModel.Dssp;
using Microsoft.Dss.Core.Attributes;
using Microsoft.Dss.ServiceModel.DsspServiceBase;
using dssp = Microsoft.Dss.ServiceModel.Dssp;
using Microsoft.Dss.Core.DsspHttp;
using W3C.Soap;

using TrackRoamer.Robotics.LibMapping;

using bumper = Microsoft.Robotics.Services.ContactSensor.Proxy;
using drive = Microsoft.Robotics.Services.Drive.Proxy;
using sicklrf = Microsoft.Robotics.Services.Sensors.SickLRF.Proxy;
using encoder = Microsoft.Robotics.Services.Encoder.Proxy;
using proxibrick = TrackRoamer.Robotics.Services.TrackRoamerBrickProximityBoard.Proxy;

using TrackRoamer.Robotics.Utility.LibSystem;
using TrackRoamer.Robotics.LibMapping;
using TrackRoamer.Robotics.LibBehavior;

namespace TrackRoamer.Robotics.Services.TrackRoamerBehaviors
{
    partial class TrackRoamerBehaviorsService : DsspServiceBase
    {
        #region Collision Related

        private const double ignoreOldSensorReadingsIntervalSec = 3.0d;     // ignore sensor readings that are older than this number of seconds. Sensor(s) must be off if that happens.

        private const double speedFactor = 1.0d / 3.0d;   // multiply distance by speedFactor to get allowable speed towards the obstacle. Denominator is allowable time to obstacle in seconds.
        private const double speedTresholdMms = 70.0d;    // below this speed a "canMove..." should be set to false, the obstacle is too close. 70 mm/s for 3 seconds gives 200mm... that's close enough.

        // IR proximity sensor reports meters 0...1.4d;  parking sensor reports meters 0...2.5d
        private const double proxiTreshold = 0.8d;        // how far in meters we allow obstacles to be detectable by the IR sensors (sensor readings will be ignored if above this)
        private const double psiTreshold = 1.2d;          // same for the parking sensor

        private const double proxiTresholdTurn1 = 0.6d;   // how far in meters we allow side obstacles to forbid turning ("canTurn...")
        private const double proxiTresholdTurn2 = 0.4d;   // same for the front/rear facing sensors - this is smaller value as we don't need much room in front/back to turn

        private double canMoveForwardDistanceM;
        private double canMoveBackwardsDistanceM;

        /// <summary>
        /// Based on distanceToObstacle calculates canMoveForward, canMoveBackwards and related distances and speeds
        /// </summary>
        /// <param name="distanceToObstacleMeters"></param>
        /// <param name="isForward"></param>
        public void adjustNearCollisionSpeed(double distanceToObstacleMeters, bool isForward)
        {
            int canMoveDistanceMm = (int)(distanceToObstacleMeters * 1000.0d);
            double canMoveSpeedMms = canMoveDistanceMm * speedFactor;
            bool canMove = canMoveSpeedMms > speedTresholdMms;
            CollisionState cs = _state.collisionState;

            if (isForward)
            {
                cs.canMoveForwardDistanceMm = canMoveDistanceMm;
                cs.canMoveForwardSpeedMms = Math.Min(canMoveSpeedMms, MaximumForwardVelocityMmSec);
                cs.canMoveForward = canMove;
            }
            else
            {
                cs.canMoveBackwardsDistanceMm = canMoveDistanceMm;
                cs.canMoveBackwardsSpeedMms = Math.Min(canMoveSpeedMms, MaximumBackwardVelocityMmSec);
                cs.canMoveBackwards = canMove;
            }
        }

        /// <summary>
        /// evaluates proximity and parking sensor data and translates all of it into
        /// actionable restrictions in CollisionState object
        /// </summary>
        public void computeCollisionState(RoutePlan plan, double? intendedVelocity)
        {
            CollisionState cs = _state.collisionState;

            //cs.initRestrictive();
            cs.initPermissive(FreeDistanceMm, MaximumForwardVelocityMmSec, MaximumBackwardVelocityMmSec);

            // initialize to the maximums:
            canMoveForwardDistanceM = FreeDistanceMm * 2.0d / 1000.0d;
            canMoveBackwardsDistanceM = FreeDistanceMm * 2.0d / 1000.0d;

            //Tracer.Trace("IR ffl=" + _state.MostRecentProximity.mfl + "m PS MetersLF=" + _state.MostRecentParkingSensor.parkingSensorMetersLF + "m");

            if(intendedVelocity == null)
            {
                intendedVelocity = _state.Velocity;
            }

            bool isMovingForward = intendedVelocity > 0.01d;        // m/s
            bool isMovingBackwards = intendedVelocity < -0.01d;

            int? distance = null;   // mm

            if (plan != null && plan.isGoodPlan)
            {
                double? bestHeadingRelative = plan.bestHeadingRelative(_mapperVicinity);
                if (bestHeadingRelative != null && bestHeadingRelative.Value > -45.0d && bestHeadingRelative.Value < 45.0d)
                {
                    distance = (int)(plan.legMeters.Value * 1000.0d);
                    if (distance < ObstacleDistanceMm)
                    {
                        cs.canMoveByPlan = false;
                    }
                    else
                    {
                        cs.canMoveByPlan = true;
                    }
                }
            }

            DateTime sensorInvalidateTime = DateTime.Now.AddSeconds(-ignoreOldSensorReadingsIntervalSec);     // ignore sensor readings that are older than this time

            StringBuilder msb = new StringBuilder();    // build the message here

            // we  compute full state every time, but format the message so it is relevant to the direction we move to.

            // forward sensors: -------------------------------------------------------------------------------------------

            proxibrick.ProximityDataDssSerializable MostRecentProximity = _state.MostRecentProximity;
            proxibrick.ParkingSensorDataDssSerializable MostRecentParkingSensor = _state.MostRecentParkingSensor;

            if (MostRecentProximity != null && MostRecentProximity.TimeStamp > sensorInvalidateTime)
            {
                double mfl = MostRecentProximity.mfl;
                if (mfl < proxiTreshold)
                {
                    if (isMovingForward)
                    {
                        msb.AppendFormat("Front Left Side proximity detected obstacle at {0}m; ", mfl);
                    }

                    cs.canTurnLeft &= mfl > proxiTresholdTurn1;

                    if (mfl < canMoveForwardDistanceM)
                    {
                        canMoveForwardDistanceM = mfl;
                    }
                }

                double mffl = MostRecentProximity.mffl;
                if (mffl < proxiTreshold)
                {
                    if (isMovingForward)
                    {
                        msb.AppendFormat("FF_Left proximity detected obstacle at {0}m; ", mffl);
                    }

                    //cs.canTurnLeft &= mffl > proxiTresholdTurn2;

                    if (mffl < canMoveForwardDistanceM)
                    {
                        canMoveForwardDistanceM = mffl;
                    }
                }

                double mffr = MostRecentProximity.mffr;
                if (mffr < proxiTreshold)
                {
                    if (isMovingForward)
                    {
                        msb.AppendFormat("FF_Right proximity detected obstacle at {0}m; ", mffr);
                    }

                    //cs.canTurnRight &= mffr > proxiTresholdTurn2;

                    if (mffr < canMoveForwardDistanceM)
                    {
                        canMoveForwardDistanceM = mffr;
                    }
                }

                double mfr = MostRecentProximity.mfr;
                if (mfr < proxiTreshold)
                {
                    if (isMovingForward)
                    {
                        msb.AppendFormat("Front Right Side proximity detected obstacle at {0}m; ", mfr);
                    }

                    cs.canTurnRight &= mfr > proxiTresholdTurn1;

                    if (mfr < canMoveForwardDistanceM)
                    {
                        canMoveForwardDistanceM = mfr;
                    }
                }
            }

            if (MostRecentParkingSensor != null && MostRecentParkingSensor.TimeStamp > sensorInvalidateTime)
            {
                double parkingSensorMetersLF = MostRecentParkingSensor.parkingSensorMetersLF;
                if (parkingSensorMetersLF < psiTreshold)
                {
                    if (isMovingForward)
                    {
                        msb.AppendFormat("Front Left parking sensor detected obstacle at {0}m; ", parkingSensorMetersLF);
                    }

                    if (parkingSensorMetersLF < canMoveForwardDistanceM)
                    {
                        canMoveForwardDistanceM = parkingSensorMetersLF;
                    }
                }

                double parkingSensorMetersRF = MostRecentParkingSensor.parkingSensorMetersRF;
                if (parkingSensorMetersRF < psiTreshold)
                {
                    if (isMovingForward)
                    {
                        msb.AppendFormat("Front Right parking sensor detected obstacle at {0}m; ", parkingSensorMetersRF);
                    }

                    if (parkingSensorMetersRF < canMoveForwardDistanceM)
                    {
                        canMoveForwardDistanceM = parkingSensorMetersRF;
                    }
                }
            }

            // backward sensors: -------------------------------------------------------------------------------------------

            if (MostRecentProximity != null && MostRecentProximity.TimeStamp > sensorInvalidateTime)
            {
                double mbl = MostRecentProximity.mbl;
                if (mbl < proxiTreshold)
                {
                    if (isMovingBackwards)
                    {
                        msb.AppendFormat("Back Left Side proximity detected obstacle at {0}m; ", mbl);
                    }

                    cs.canTurnRight &= mbl > proxiTresholdTurn1;

                    if (mbl < canMoveBackwardsDistanceM)
                    {
                        canMoveBackwardsDistanceM = mbl;
                    }
                }

                double mbbl = MostRecentProximity.mbbl;
                if (mbbl < proxiTreshold)
                {
                    if (isMovingBackwards)
                    {
                        msb.AppendFormat("BB_Left proximity detected obstacle at {0}m; ", mbbl);
                    }

                    //cs.canTurnRight &= mbbl > proxiTresholdTurn2;

                    if (mbbl < canMoveBackwardsDistanceM)
                    {
                        canMoveBackwardsDistanceM = mbbl;
                    }
                }

                double mbbr = MostRecentProximity.mbbr;
                if (mbbr < proxiTreshold)
                {
                    if (isMovingBackwards)
                    {
                        msb.AppendFormat("BB_Right proximity detected obstacle at {0}m; ", mbbr);
                    }

                    //cs.canTurnLeft &= mbbr > proxiTresholdTurn2;

                    if (mbbr < canMoveBackwardsDistanceM)
                    {
                        canMoveBackwardsDistanceM = mbbr;
                    }
                }

                double mbr = MostRecentProximity.mbr;
                if (mbr < proxiTreshold)
                {
                    if (isMovingBackwards)
                    {
                        msb.AppendFormat("Back Right Side proximity detected obstacle at {0}m; ", mbr);
                    }

                    cs.canTurnLeft &= mbr > proxiTresholdTurn1;

                    if (mbr < canMoveBackwardsDistanceM)
                    {
                        canMoveBackwardsDistanceM = mbr;
                    }
                }
            }

            if (MostRecentParkingSensor != null && MostRecentParkingSensor.TimeStamp > sensorInvalidateTime)
            {
                double parkingSensorMetersLB = MostRecentParkingSensor.parkingSensorMetersLB;
                if (parkingSensorMetersLB < psiTreshold)
                {
                    if (isMovingBackwards)
                    {
                        msb.AppendFormat("Back Left parking sensor detected obstacle at {0}m; ", parkingSensorMetersLB);
                    }

                    if (parkingSensorMetersLB < canMoveBackwardsDistanceM)
                    {
                        canMoveBackwardsDistanceM = parkingSensorMetersLB;
                    }
                }

                double parkingSensorMetersRB = MostRecentParkingSensor.parkingSensorMetersRB;
                if (parkingSensorMetersRB < psiTreshold)
                {
                    if (isMovingBackwards)
                    {
                        msb.AppendFormat("Back Right parking sensor detected obstacle at {0}m; ", parkingSensorMetersRB);
                    }

                    if (parkingSensorMetersRB < canMoveBackwardsDistanceM)
                    {
                        canMoveBackwardsDistanceM = parkingSensorMetersRB;
                    }
                }
            }

            if (_state.MostRecentWhiskerLeft)
            {
                msb.Append("Left Whisker; ");
                canMoveForwardDistanceM = 0.0d;
                cs.canMoveForward = false;
                cs.canTurnRight = false;
                cs.canTurnLeft = false;
            }

            if (_state.MostRecentWhiskerRight)
            {
                msb.Append("Right Whisker; ");
                canMoveForwardDistanceM = 0.0d;
                cs.canMoveForward = false;
                cs.canTurnRight = false;
                cs.canTurnLeft = false;
            }

            adjustNearCollisionSpeed(canMoveForwardDistanceM, true);
            adjustNearCollisionSpeed(canMoveBackwardsDistanceM, false);

            // compute the "mustStop" and "message":
            if (isMovingForward)
            {
                cs.mustStop = !cs.canMoveForward;
            }
            else if (isMovingBackwards)
            {
                cs.mustStop = !cs.canMoveBackwards;
            }

            if (msb.Length > 0 || cs.mustStop)
            {
                if (isMovingForward)
                {
                    msb.Insert(0, "While moving forward: ");
                }
                else if (isMovingBackwards)
                {
                    msb.Insert(0, "While moving backwards: ");
                }

                if (cs.mustStop)
                {
                    msb.Insert(0, "!! MUST STOP !! ");
                }
            }
            else
            {
                if (isMovingForward)
                {
                    msb.Append("Can continue forward: ");
                }
                else if (isMovingBackwards)
                {
                    msb.Append("Can continue backwards: ");
                }
                else
                {
                    msb.Append("Can move: ");
                }
            }

            msb.AppendFormat(" FWD: {0} to {1} mm at {2:F0} mm/s   BKWD: {3} to {4} mm at {5:F0} mm/s   LEFT:{6}  RIGHT:{7}",
                                cs.canMoveForward, cs.canMoveForwardDistanceMm, cs.canMoveForwardSpeedMms,
                                cs.canMoveBackwards, cs.canMoveBackwardsDistanceMm, cs.canMoveBackwardsSpeedMms,
                                cs.canTurnLeft, cs.canTurnRight);

            if (cs.canMoveByPlan.HasValue && !cs.canMoveByPlan.Value)
            {
                msb.AppendFormat("; best plan obstacle too close at {0} mm; ", distance);
            }

            string message = msb.ToString();

            //if (!string.Equals(message, _lastMessage))
            //{
            //    Tracer.Trace(message);
            //    _lastMessage = message;
            //}
            cs.message = message;
        }

        /// <summary>
        /// set all signal lights to reflect current Collision State
        /// </summary>
        private void setCollisionLights()
        {
            CollisionState cs = _state.collisionState;

            SetLightsWhiskers();
            SetLightsMustStop(cs.mustStop);
            SetLightsCanMoveForward(cs.canMoveForward);
            SetLightsCanMoveBackwards(cs.canMoveBackwards);
            SetLightsCanTurnLeft(cs.canTurnLeft);
            SetLightsCanTurnRight(cs.canTurnRight);
        }

        private string _lastMessage = string.Empty;
        private DateTime lastStopAnnounced = DateTime.Now;

        /// <summary>
        /// If the robot is moving or intends to move, and an obstacle is too close, return false. The caller must take care to stop the movement.
        /// You can then map the environment for a way around it, or see what options are in _state.collisionState
        /// </summary>
        /// <param name="distance"></param>
        private bool PerformAvoidCollision(RoutePlan plan, double? intendedVelocity = null)
        {
            computeCollisionState(plan, intendedVelocity);

            setCollisionLights();

            string messageCollisionState = _state.collisionState.message;

            bool mustStop = false;

            if (_state.IsMoving)
            {
                // we may come here with just proximity and other data, but not laser data.
                if (_state.collisionState.canMoveByPlan.HasValue && !_state.collisionState.canMoveByPlan.Value)
                {
                    //
                    // We are moving and there is something less than < ObstacleDistance>
                    // millimeters from the center of the robot (within field of view) on the planned leg. STOP.
                    //

                    LogInfo("TrackRoamerBehaviorsService: AvoidCollision() - best plan obstacle - closer than ObstacleDistance=" + ObstacleDistanceMm);
                    mustStop = true;
                }
            }

            setMovingStateDetail(messageCollisionState);

            if (_state.collisionState.mustStop || mustStop)
            {
                //string messageToSay = "stop."; // "avoid collision - must stop";

                //if ((DateTime.Now - lastStopAnnounced).TotalMilliseconds > 3000.0d)
                //{
                //    Talker.Say(5, messageToSay);
                //    lastStopAnnounced = DateTime.Now;
                //}
                LogHistory(5, "avoid collision - must stop");

                return false;   // has to stop
            }
            else
            {
                return true;    // does not have to stop
            }
        }

        #endregion // Collision Related
    }
}
