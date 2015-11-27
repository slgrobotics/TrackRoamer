using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TrackRoamer.Robotics.LibMapping
{
    public class MapCell : List<IDetectedObject>
    {
        public List<IDetectedObject> detectedObjects { get { return this; } }

        // position on the grid:
        public int x;
        public int y;

        public int val { get { return this.Count; } set { ; } }
    }
}
