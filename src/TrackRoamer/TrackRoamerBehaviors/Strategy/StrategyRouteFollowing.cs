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
        #region StrategyRouteFollowing()

        private const double WAYPOINT_CANTREACH_SECONDS = 30;

        private void StrategyRouteFollowing()
        {
            LocationWp nextWp = _routePlanner.mission.nextTargetWp;
            FollowDirectionTargetDistanceToGoalMeters = 0.0d;

            if (nextWp != null)
            {
                Direction dirToWp = nextWp.directionToWp(_mapperVicinity.robotPosition, _mapperVicinity.robotDirection);

                Distance distToWp = nextWp.distanceToWp(_mapperVicinity.robotPosition);

                if (distToWp.Meters < 2.0d)
                {
                    nextWp.waypointState = WaypointState.Passed;     // will be ignored on the next cycle
                    Talker.Say(10, "Waypoint " + nextWp.number + " passed");
                }
                else if (nextWp.estimatedTimeOfArrival.HasValue && (DateTime.Now - nextWp.estimatedTimeOfArrival.Value).TotalSeconds > WAYPOINT_CANTREACH_SECONDS)
                {
                    nextWp.waypointState = WaypointState.CouldNotReach;     // will be ignored on the next cycle
                    Talker.Say(10, "Waypoint " + nextWp.number + " could not reach");
                }
                else
                {
                    setCurrentGoalBearingRelativeToRobot(dirToWp.bearingRelative.Value);
                    setCurrentGoalDistance(distToWp.Meters);
                    FollowDirectionMaxVelocityMmSec = Math.Min(distToWp.Meters > 3.0d ? MaximumForwardVelocityMmSec : ModerateForwardVelocityMmSec,
                                                                _state.collisionState.canMoveForwardSpeedMms);     // mm/sec

                    Tracer.Trace(string.Format("IP: distToWp.Meters= {0}, FollowDirectionMaxVelocityMmSec={1}", distToWp.Meters, FollowDirectionMaxVelocityMmSec));

                    if (!nextWp.estimatedTimeOfArrival.HasValue && FollowDirectionMaxVelocityMmSec != 0.0d)
                    {
                        nextWp.waypointState = WaypointState.SelectedAsTarget;
                        double timeToReachSec = (distToWp.Meters * 1000.0d) / (FollowDirectionMaxVelocityMmSec);        // mm/sec
                        nextWp.estimatedTimeOfArrival = DateTime.Now.AddSeconds(timeToReachSec);
                    }

                    // choose current tactics:
                    _mapperVicinity.robotState.robotTacticsType = RobotTacticsType.FollowDirection;
                }
            }
            else
            {
                Talker.Say(10, "Last Waypoint " + nextWp.number + " passed, stopping");
                // choose current tactics - we are done, stop:
                _mapperVicinity.robotState.robotTacticsType = RobotTacticsType.None;
                _mapperVicinity.robotDirection.bearing = null;
            }
        }

        #endregion // StrategyRouteFollowing()
    }
}
