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
            SetObstacle();
        }

        public void SetObstacle()
        {
            objectType = DetectedObjectType.Obstacle;
            objectKind = DetectedObjectKind.Position;
            hasShadowBehind = true;
        }

        public DetectedObstacle(GeoPosition pos)
            : base(pos)
        {
            SetObstacle();
        }

        public DetectedObstacle(Direction dir, Distance dist)
            : base(dir, dist)
        {
            SetObstacle();
        }
    }
}
