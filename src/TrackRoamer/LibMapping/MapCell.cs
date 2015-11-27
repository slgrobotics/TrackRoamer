using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Windows.Media;

namespace TrackRoamer.Robotics.LibMapping
{
    public class MapCell : List<IDetectedObject>
    {
        public List<Color> colors = new List<Color>();

        // position on the grid:
        public int x;
        public int y;

        public int val { get { return this.Count; } set { ; } }

        public void AddDetectedObject(IDetectedObject obj)
        {
            this.Add(obj);

            if(!colors.Contains(obj.color))
            {
                colors.Add(obj.color);
            }
        }

        public Color DisplayColor
        {
            get
            {
                switch (colors.Count())
                {
                    case 0:
                        return Colors.Green;
                    case 1:
                        return colors[0];
                    default:
                        return Colors.Red;
                }
            }
        }

        public bool HasHuman
        {
            get { return (from d in this where d.objectType == DetectedObjectType.Human select d).Any(); }
        }

    }
}
