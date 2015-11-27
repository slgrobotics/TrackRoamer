using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TrackRoamer.Robotics.Utility.LibPicSensors
{
    /// <summary>
    /// the most basic container for heading and bearing.
    /// </summary>
    public class DirectionData
    {
        public long TimeStamp = 0L;

        // see http://answers.yahoo.com/question/index?qid=20081117160002AADh95q
        // see http://www.rvs.uni-bielefeld.de/publications/Incidents/DOCS/Research/Rvs/Misc/Additional/Reports/adf.gif

        /*
            Heading is not always the direction an aircraft is moving. That is called 'course'. Heading is the direction the aircraft is pointing.
            The aircraft may be drifting a little or a lot due to a crosswind.
            Bearing is the angle in degrees (clockwise) between North and the direction to the destination or nav aid.
            Relative bearing is the angle in degrees (clockwise) between the heading of the aircraft and the destination or nav aid.
         */

        public double heading;      // degrees; same as "course" for a ground platform; true North is "0"
        public double? bearing;     // degrees; usually to target or obstacle; absolute to true North, null if no target is present
    }
}
