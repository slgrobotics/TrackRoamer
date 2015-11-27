using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TrackRoamer.Robotics.Utility.LibPicSensors
{
    /// <summary>
    /// container for a single reading from sonar
    /// </summary>
    public class RangeReading //: IComparable
    {
        public int angleRaw { get; private set; }			    // servo pulse width or any other measure, in PIC units, range for example 150 - 1160 for Futaba servo on Parallax BasicStamp
        public double angleDegrees { get; set; }		        // degrees -- not used here, but may be used on the processing end

        public double rangeMeters { get; private set; } 		// meters

        public long timestamp { get; set; }

        public RangeReading(int _angleRaw, double _rangeMeters, long _timestamp)
        {
            angleRaw = _angleRaw;
            rangeMeters = _rangeMeters;
            timestamp = _timestamp == -1L ? DateTime.Now.Ticks : _timestamp;
        }

        //public int CompareTo(object obj)
        //{
        //    // newer object is greater than older object.
        //    return (int)(this.timestamp - ((RangeReading)obj).timestamp);
        //}
    }

}

