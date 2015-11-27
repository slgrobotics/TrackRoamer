
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
        #region Strategy()



        private RobotStrategyType strategyPrev = RobotStrategyType.None;
        private DateTime lastSafePosture = DateTime.Now;

        /// <summary>
        /// Strategy chooses behavior for the moment and sets its parameters.
        /// Strategy would know goal and performance criteria to be able to estimate how far we are from the goal
        /// and change behavior if needed.
        /// 
        /// Strategy is commanded by Interaction, which sets the objectives and chooses what game (strategy) to play.
        /// </summary>
        private void Strategy()
        {
            bool isNewStrategy = _mapperVicinity.robotState.robotStrategyType != strategyPrev;

            if (isNewStrategy)
            {
                strategyPrev = _mapperVicinity.robotState.robotStrategyType;
                HeadlightsOff();
                lastSafePosture = DateTime.Now;
                SafePosture();
                Talker.Say(10, "New strategy " + strategyPrev);
            }

            // as a minimum, a strategy should set a goal and a RobotTacticsType. Then rely on the tactics to avoid obstacles.
            // More sophisticated behavior will evaluate obstacles, and offer tactics a number of plans.

            switch (_mapperVicinity.robotState.robotStrategyType)
            {
                case RobotStrategyType.None:
                    _mapperVicinity.robotState.robotTacticsType = RobotTacticsType.None;     // active Stop
                    if ((DateTime.Now - lastSafePosture).TotalSeconds > 5.0d)
                    {
                        lastSafePosture = DateTime.Now;
                        SafePosture();
                    }
                    break;

                case RobotStrategyType.InTransition:
                    _mapperVicinity.robotState.robotTacticsType = RobotTacticsType.InTransition;     // do not command the Power Brick; leave it for Strategy.
                    break;

                case RobotStrategyType.LayInWait:
                    _mapperVicinity.robotState.robotTacticsType = RobotTacticsType.LayInWait;
                    break;

                case RobotStrategyType.GoalAndBack:
                    // choose robotTacticsType - current tactics:
                    _mapperVicinity.robotState.robotTacticsType = RobotTacticsType.GoStraightToDirection;
                    break;

                case RobotStrategyType.RouteFollowing:
                    // follow GPS waypoints ("Mission"):
                    StrategyRouteFollowing();
                    break;

                case RobotStrategyType.PersonFollowing:
                    if (isNewStrategy)
                    {
                        StrategyPersonFollowingInit();
                    }

                    // follow a Kinect skeleton or a red shirt:
                    StrategyPersonFollowing();
                    break;

                case RobotStrategyType.PersonHunt:
                    // choose robotTacticsType - current tactics:
                    _mapperVicinity.robotState.robotTacticsType = RobotTacticsType.None;
                    break;

                case RobotStrategyType.RandomScan:
#if OBSOLETE_PLANANDAVOID_LOGIC
                    // choose robotTacticsType - current tactics:
                    _mapperVicinity.robotState.robotTacticsType = RobotTacticsType.PlanAndAvoid;
#else // OBSOLETE_PLANANDAVOID_LOGIC
                    _mapperVicinity.robotState.robotTacticsType = RobotTacticsType.None;     // active Stop
#endif // OBSOLETE_PLANANDAVOID_LOGIC
                    break;
            }

            // set parameters so that the tactics can perform:
        }

		/*
        /// <summary>
        /// Will apply necessary logic to compute CollisionState and will stop the robot and return false if it detects a "must stop" condition.
        /// </summary>
        /// <returns></returns>
        private bool CheckForAndAvoidObstacles()
        {
            // we may come here with just proximity and other data, but not laser data.
            return PerformAvoidCollision(_currentRoutePlan);   // may result in state set to MovingState.Unable (if mustStop)

            //return !_state.collisionState.mustStop;
        }
		*/

        #endregion // Strategy()

        #region Helpers

        protected void setCurrentGoalDistance(double? distanceMeters)
        {
            _mapperVicinity.robotDirection.distanceToGoalMeters = distanceMeters;
        }

        private DateTime lastPrintedBearing = DateTime.Now;

        /// <summary>
        /// set Current Goal Bearing based on parameter bearingRelative
        /// </summary>
        /// <param name="bearingRelativeToRobot">Goal bearing relative to robot, degrees</param>
        protected void setCurrentGoalBearingRelativeToRobot(double bearingRelativeToRobot)
        {
            double? goalBearing = _mapperVicinity.robotDirection.bearingRelative;

            if (!goalBearing.HasValue || Math.Abs(goalBearing.Value - bearingRelativeToRobot) > 2.0d)   // ignore differences less than 2 degrees
            {
                // update mapper with Direction data:
                Direction newDir = new Direction()
                {
                    TimeStamp = DateTime.Now.Ticks,
                    heading = _mapperVicinity.robotDirection.heading
                };

                newDir.bearingRelative = bearingRelativeToRobot;   // will calculate absolute bearing

                _mapperVicinity.robotDirection = newDir;    // will also call mapperVicinity.computeMapPositions();

                if ((DateTime.Now - lastPrintedBearing).TotalSeconds > 5.0d)
                {
                    lastPrintedBearing = DateTime.Now;
                    Tracer.Trace("------------- goal bearing=" + _mapperVicinity.robotDirection.bearing + "   (supplied bearingRelative=" + bearingRelativeToRobot + ")");
                    //Talker.Say(10, "" + Math.Round(bearingRelativeToRobot));
                    //Talker.Say(10, "" + Math.Round(_mapperVicinity.robotDirection.bearing));
                }

                setGuiCurrentDirection(newDir);
            }
        }

        #endregion // Helpers
    }
}
