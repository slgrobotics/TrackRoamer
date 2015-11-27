using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TrackRoamer.Robotics.LibMapping
{
    /// <summary>
    /// concrete class describing an obstacle detected by sensors
    /// </summary>
    public class DetectedObstacle : DetectedObjectBase
    {
        public DetectedObstacle()
        {
            objectType = DetectedObjectType.Obstacle;
        }

        public DetectedObstacle(GeoPosition pos)
            : base(pos)
        {
            objectType = DetectedObjectType.Obstacle;
        }

        public DetectedObstacle(Direction dir, Distance dist)
            : base(dir, dist)
        {
            objectType = DetectedObjectType.Obstacle;
        }
    }
}
