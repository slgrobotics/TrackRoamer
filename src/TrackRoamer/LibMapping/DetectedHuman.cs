using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Windows.Media;

namespace TrackRoamer.Robotics.LibMapping
{
    public class DetectedHuman : DetectedObjectBase
    {
        public DetectedHuman()
        {
            SetHuman();
        }

        private void SetHuman()
        {
            objectType = DetectedObjectType.Human;
            objectKind = DetectedObjectKind.Position;
            hasShadowBehind = false;
            color = Colors.Red;
        }

        public DetectedHuman(GeoPosition pos)
            : base(pos)
        {
            SetHuman();
        }

        public DetectedHuman(Direction dir, Distance dist)
            : base(dir, dist)
        {
            SetHuman();
        }
    }
}
