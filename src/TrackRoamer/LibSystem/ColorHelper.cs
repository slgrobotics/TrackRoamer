using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Drawing;

namespace TrackRoamer.Robotics.Utility.LibSystem
{
    public static class ColorHelper
    {
        /// <summary>
        /// produces color between near and far (red and green, for example) given the distances
        /// </summary>
        /// <param name="nearColor"></param>
        /// <param name="farColor"></param>
        /// <param name="nearLimit"></param>
        /// <param name="farLimit"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static Color LinearColor(Color nearColor, Color farColor, int nearLimit, int farLimit, int currentDistance)
        {
            if (currentDistance <= nearLimit)
            {
                return nearColor;
            }
            else if (currentDistance >= farLimit)
            {
                return farColor;
            }

            int span = farLimit - nearLimit;
            int pos = currentDistance - nearLimit;

            int r = (nearColor.R * (span - pos) + farColor.R * pos) / span;
            int g = (nearColor.G * (span - pos) + farColor.G * pos) / span;
            int b = (nearColor.B * (span - pos) + farColor.B * pos) / span;

            return Color.FromArgb(r, g, b);
        }
    }
}
