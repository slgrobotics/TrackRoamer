using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Windows.Media;

namespace TrackRoamer.Robotics.LibMapping
{
    public enum DetectedObjectKind { Position, Angle }  // we may know position of the obstacle, or just a direction where it is (i.e. sound)

    public enum DetectedObjectType {
        Unknown,
        Mark,
        Obstacle,
        Human,
        Sound,
        MoveTarget,
        GunTarget
    }

    public enum DetectorType {
        NONE,
        SONAR_SCANNING,
        SONAR_DIRECTED,
        IR_DIRECTED,
        KINECT_DEPTH,
        KINECT_SKELETON,
        KINECT_CAMERA,
        KINECT_MIC,
        WHISKERS
    } 

    public interface IDetectedObject : IComparable
    {
        DetectedObjectKind objectKind { get; }

        DetectedObjectType objectType { get; }

        DetectorType detectorType { get; }

        bool hasShadowBehind { get; }    // a wall is likely to have cells behind it inaccessible, while we can drive around/behind a human

        GeoPosition geoPosition { get; set; }

        RelPosition relPosition { get; }

        void updateRelPosition(GeoPosition myPos, Direction myDir);     // recalculates relPosition

        Direction directionTo(GeoPosition myPos, Direction myDir);

        Distance distanceTo(GeoPosition myPos);

        long firstSeen { get; }
        long lastSeen { get; set; }
        int timeToLiveSeconds { get; set; }

        bool isDead { get; }

        int cellCounter { get; set; }

        Color color { get; }
    }
}
