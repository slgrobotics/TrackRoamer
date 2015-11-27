using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using TrackRoamer.Robotics.LibMapping;

namespace TrackRoamer.Robotics.LibBehavior
{
    public class RoutePlan
    {
        public double? bestHeading;             // degrees absolute
        public double? legMeters;               // can be null if only turn is planned
        public double? closestObstacleAlongBestPathMeters;   // within the "planning" scan; used to evaluate safe speed 
        public GeoPosition goalPosition;
        public List<RoutePlan> nextSteps;
        public List<RoutePlan> fallBackPlans;   // if this plan cannot be accepted, here are plan B, C...
        public TimeSpan timeSpentPlanning;

        public bool isGoodPlan { get { return legMeters.HasValue && bestHeading.HasValue; } }

        public RoutePlan()
        {
            nextSteps = new List<RoutePlan>();

            fallBackPlans = new List<RoutePlan>();
        }

        public double? bestHeadingRelative(MapperVicinity mapper)     // degrees, relative to robot direction
        {
            return Direction.to180(bestHeading - mapper.robotDirection.heading);
        }
    }
}
