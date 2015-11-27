using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TrackRoamer.Robotics.LibMapping
{
    /// <summary>
    /// Relative Position is robot-related coordinate system expressed in meters and degrees (if angular)
    /// All objects in MapperVicinity have RelPosition coordinates and move as the robot moves.
    /// </summary>
    public class RelPosition : IComparable, ICloneable
    {
        // rectangular grid:

        public double X;    // meters from the robot, forward is positive
        public double Y;    // meters from the robot, right is positive

        // angular system:

        public Distance dist;
        public Direction dir;   // straight forward is 0, right is positive


        public RelPosition(Direction direction, Distance distance)
        {
            this.dir = (Direction)direction.Clone();
            this.dist = (Distance)distance.Clone();

            if (direction.bearingRelative.HasValue)
            {
                double bearingRad = direction.bearingRelative.Value * Math.PI / 180.0d;

                X = distance.Meters * Math.Sin(bearingRad);
                Y = distance.Meters * Math.Cos(bearingRad);
            }
            else if (direction.bearing.HasValue)
            {
                double bearingRad = direction.bearing.Value * Math.PI / 180.0d;

                X = distance.Meters * Math.Sin(bearingRad);
                Y = distance.Meters * Math.Cos(bearingRad);
            }
        }

        public override string ToString()
        {
            return string.Format("({0},{1}):({2},{3})", X, Y, dist, dir);
        }

        #region ICloneable Members

        public object Clone()
        {
            RelPosition ret = (RelPosition) this.MemberwiseClone();  // shallow copy, only value types are cloned

            ret.dist = (Distance)this.dist.Clone();
            ret.dir = (Direction)this.dir.Clone();

            return ret;
        }

        #endregion // ICloneable Members

        #region ICompareable Members

        /// <summary>
        /// Comparing two obstacles
        /// the closer the oject, the larger it is for Compare purposes
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public int CompareTo(object other)
        {
            return (int)((dist.Meters - ((RelPosition)other).dist.Meters) * 100.0d);    // precision up to 10mm
        }

        #endregion // ICompareable Members

    }
}
