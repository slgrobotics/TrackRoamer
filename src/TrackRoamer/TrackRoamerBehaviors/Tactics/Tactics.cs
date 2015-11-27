using System;
using System.Collections.Generic;
using System.Linq;

using Microsoft.Dss.ServiceModel.DsspServiceBase;

using sicklrf = Microsoft.Robotics.Services.Sensors.SickLRF.Proxy;

using TrackRoamer.Robotics.Utility.LibSystem;
using TrackRoamer.Robotics.LibMapping;
using TrackRoamer.Robotics.LibBehavior;

namespace TrackRoamer.Robotics.Services.TrackRoamerBehaviors
{
    partial class TrackRoamerBehaviorsService : DsspServiceBase
    {
        #region Tactics()

        private void Tactics()
        {
            // Tactics
            switch (_mapperVicinity.robotState.robotTacticsType)
            {
                case RobotTacticsType.None:     // active Stop
                    stopCount = 1;
                    break;

                case RobotTacticsType.InTransition:     // do not command the Power Brick; leave it for Strategy.
                    stopCount = 0;  // do not send stop commands to power brick
                    break;

                case RobotTacticsType.LayInWait:
                    stopCount = 1;
                    break;

                case RobotTacticsType.FollowDirection:
                    if (!_state.IsTurning
                            && _state.MovingState != MovingState.InTransition
                            && _state.MovingState != MovingState.BumpedBackingUp
                       )
                    {
                        initTactics(_mapperVicinity.robotState.doVicinityPlanning);
                        TacticsFollowDirection();      // make sure FollowDirectionForwardSpeed has been set by Strategy()
                        finishTactics();
                    }
                    break;

                case RobotTacticsType.GoStraightToDirection:
                    if (!_state.IsTurning
                            && _state.MovingState != MovingState.InTransition
                            && _state.MovingState != MovingState.BumpedBackingUp
                       )
                    {
                        initTactics(_mapperVicinity.robotState.doVicinityPlanning);
                        TacticsGoStraightToDirection();
                        finishTactics();
                    }
                    break;

                case RobotTacticsType.PlanAndAvoid:
#if OBSOLETE_PLANANDAVOID_LOGIC
                    if (_laserData != null && _laserData.DistanceMeasurements != null && (DateTime.Now - _laserData.TimeStamp).TotalSeconds < 2.0d)
                    {
                        // allow to change direction only if previous turn is complete
                        if (!_state.IsTurning
                            && _state.MovingState != MovingState.InTransition
                            && _state.MovingState != MovingState.BumpedBackingUp
                            && (DateTime.Now - _state.LastTurnCompleted).TotalSeconds > TurningWaitInterval)     // let the sensor make a sweep after the turn stops, otherwise skip the reading
                        {
                            initTactics(true);

                            TacticsPlanAndAvoid();

                            // ===== no more writing to currentStatusGraphics bitmaps after this point.

                            finishTactics();
                        }
                    }
                    else if (_state.IsMoving)
                    {
                        // we may come here with just proximity and other data, but not laser data.
                        bool canMove = PerformAvoidCollision(null);   // may result in state set to MovingState.Unable (if mustStop)
                    }
#endif // OBSOLETE_PLANANDAVOID_LOGIC
                    break;
            }
        }

        #endregion // Tactics()

        #region Tactics helpers

        protected RoutePlan _currentRoutePlan = null;
        private string sPlan = "no good plan";

        protected DateTime lastDrawTop = DateTime.MinValue;
        private const double DrawTopInterval = 1.0d;    // allow thinking only after so many seconds

        /// <summary>
        /// prepares HTTP graphics and planner, can call _routePlanner.planRoute() if doPlanRoute is true
        /// </summary>
        /// <param name="doPlanRoute"></param>
        private void initTactics(bool doPlanRoute)
        {
            bool canDrawTop = (DateTime.Now - lastDrawTop).TotalSeconds > DrawTopInterval;

            if (currentStatusGraphics != null)
            {
                currentStatusGraphics.Dispose();
                currentStatusGraphics = null;
            }

            // we limit drawing to the cycles when laser data comes in, to limit frequency of drawing:
            if (canDrawTop)
            {
                lastDrawTop = DateTime.Now;
                currentStatusGraphics = new StatusGraphics();
            }

            // ===== we can start writing to currentStatusGraphics bitmaps at will here.

            //Tracer.Trace("calling planRoute()");

            _routePlanner.ObstacleDistanceMeters = ((double)ObstacleDistanceMm) / 1000.0d * 1.5d;

            if (doPlanRoute)
            {
                _currentRoutePlan = _routePlanner.planRoute();
            }
            else
            {
                _currentRoutePlan = null;
            }

            if (canDrawTop)
            {
                // draw laser sweep data and basic markings:
                int fieldOfView = (int)Math.Round(_routePlanner.sweepAngleNormal / 2.0d);   // to either side

                //Tracer.Trace("calling GenerateTop()");

                GenerateTop(_laserData, fieldOfView, _currentRoutePlan);
            }

            sPlan = "no good plan";
        }

        /// <summary>
        /// finishes a tactics run, moves graphics where it will be accessible to HTTP
        /// </summary>
        private void finishTactics()
        {
            if (currentStatusGraphics != null)
            {
                // move current StatusGraphics to last, to be available for HttpGet:
                //lock (lockStatusGraphics)
                {
                    StatusGraphics tmp = lastStatusGraphics;
                    lastStatusGraphics = currentStatusGraphics;
                    if (tmp != null)
                    {
                        tmp.Dispose();
                    }
                    currentStatusGraphics = null;
                }
            }
        }

        /// <summary>
        /// see if current plan chain has a plan good enough to satisfy collisionState constraints; 
        /// </summary>
        /// <param name="plan"></param>
        private RoutePlan critiquePlanChain(RoutePlan plan)
        {
            RoutePlan ret = plan;

            if (plan != null)
            {
                double? bestHeadingRelative = plan.bestHeadingRelative(_mapperVicinity);

                if (bestHeadingRelative.HasValue)
                {
                    if (!_state.collisionState.canTurnLeft && bestHeadingRelative < 0 || !_state.collisionState.canTurnRight && bestHeadingRelative > 0)
                    {
                        Tracer.Trace("Unable to turn " + bestHeadingRelative + " degrees due to collisionState");

                        // TODO: see if any of plan.fallBackPlans can be used

                        ret = null;
                    }
                }
            }

            return ret;    // selected from initial plan chain
        }

        #endregion // Tactics helpers
    }
}
