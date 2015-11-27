using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TrackRoamer.Robotics.LibMapping
{
    [Serializable]
    public class Direction : ICloneable
    {
        // see http://answers.yahoo.com/question/index?qid=20081117160002AADh95q
        // see http://www.rvs.uni-bielefeld.de/publications/Incidents/DOCS/Research/Rvs/Misc/Additional/Reports/adf.gif

        /*
            Heading is not always the direction an aircraft is moving. That is called 'course'. Heading is the direction the aircraft is pointing.
            The aircraft may be drifting a little or a lot due to a crosswind.
            Bearing is the angle in degrees (clockwise) between North and the direction to the destination or nav aid.
            Relative bearing is the angle in degrees (clockwise) between the heading of the aircraft and the destination or nav aid.
         */

        /// <summary>
        /// Heading is the direction the robot is pointing, degrees; same as "course" for a ground platform; true North is "0"
        /// </summary>
        public double? heading;

        /// <summary>
        /// Bearing is the angle in degrees (clockwise) between North and the direction to the destination or nav aid.
        /// Degrees; usually to target or obstacle; absolute, related to true North
        /// </summary>
        public double? bearing;

        /// <summary>
        /// distance to goal, meters; usually to target or obstacle; can be null, but not negative.
        /// </summary>
        public double? distanceToGoalMeters;


        public double? course       // degrees, guaranteed to be between 0...360
        {
            get
            {
                return to360(heading);
            }
            set
            {
                heading = value % 360.0d;
                if (heading < 0.0d)
                {
                    heading += 360.0d;
                }
            }
        }

        public double? bearingRelative    // use only if heading is defined
        {
            get
            {
                if (heading.HasValue && bearing.HasValue)
                {
                    // calculate turn is between -180...180:

                    return to180((double)(bearing - heading));
                }
                else
                {
                    return null;
                }
            }
            set
            {
                if (heading.HasValue && value.HasValue)
                {
                    bearing = to360((double)(heading + value));
                }
                else
                {
                    bearing = null;
                }
            }
        }

        public double turnRelative
        {
            get
            {
                if (heading.HasValue && bearing.HasValue)
                {
                    // calculate turn is between -180...180:

                    return to180((double)(bearing - heading));
                }
                else
                {
                    return 0.0d;
                }
            }
        }

        public long TimeStamp = 0L;

        // for compass related calculations, use GeoPosition::magneticVariation() to offset true North

        public Direction()
        {
        }

        #region ICloneable Members

        public object Clone()
        {
            // Direction ret = new Direction() { bearing = this.bearing, heading = this.heading };
            return this.MemberwiseClone();  // shallow copy, only value types are cloned
        }

        #endregion // ICloneable Members

        /// <summary>
        /// normalize angle to be within 0...360
        /// </summary>
        /// <param name="angle"></param>
        /// <returns></returns>
        public static double? to360(double? angle)
        {
            double? ret = null;

            if (angle.HasValue)
            {
                ret = angle % 360.0d;
                if (ret < 0.0d)
                {
                    ret += 360.0d;
                }
            }
            return ret;
        }

        public static double to360(double angle)
        {
            angle %= 360.0d;

            if (angle < 0.0d)
            {
                angle += 360.0d;
            }
            return angle;
        }

        /// <summary>
        /// normalize angle to be within -180...180
        /// </summary>
        /// <param name="angle"></param>
        /// <returns></returns>
        public static double? to180(double? angle)
        {
            double? ret = null;

            if (angle.HasValue)
            {
                ret = angle % 360.0d;

                if (ret > 180.0d)
                {
                    ret -= 360.0d;
                }
                if (ret < -180.0d)
                {
                    ret += 360.0d;
                }
            }
            return ret;
        }

        public static double to180(double angle)
        {
            angle %= 360.0d;

            if (angle > 180.0d)
            {
                angle -= 360.0d;
            }
            if (angle < -180.0d)
            {
                angle += 360.0d;
            }
            return angle;
        }

        /// <summary>
        /// for most direction display operations we need int, given angle in rads
        /// </summary>
        /// <param name="angle"></param>
        /// <returns></returns>
        public static int to180fromRad(double radians)
        {
            double angle = radians * 180.0d / Math.PI;

            angle %= 360.0d;

            if (angle > 180.0d)
            {
                angle -= 360.0d;
            }
            if (angle < -180.0d)
            {
                angle += 360.0d;
            }
            return (int)Math.Round(angle);
        }

        public override string ToString()
        {
            return string.Format("({0},{1})", heading, bearing);
        }
    }
}
