using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Windows.Media;

namespace TrackRoamer.Robotics.LibMapping
{
    public enum DetectedObjectType { Mark, Unknown, Obstacle, Human }

    public enum DetectorType { NONE, SONAR_SCANNING, SONAR_DIRECTED, IR_DIRECTED, WHISKERS } 

    public interface IDetectedObject : IComparable
    {
        DetectedObjectType objectType { get; }

        GeoPosition geoPosition { get; }

        RelPosition relPosition { get; set; }

        long firstSeen { get; }
        long lastSeen { get; }
        int timeToLiveSeconds { get; set; }

        bool isDead { get; }

        int cellCounter { get; set; }

        Color color { get; set; } 
    }
}
