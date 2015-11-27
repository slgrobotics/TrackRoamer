using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using TrackRoamer.Robotics.LibMapping;
using TrackRoamer.Robotics.Utility.LibSystem;

namespace TrackRoamer.Robotics.LibBehavior
{
    public enum WaypointState
    {
        None,
        SelectedAsTarget,
        Passed,
        CouldNotReach
    }

    /// <summary>
    /// LocationWp is similar to MavLink's Locationwp, but usable anywhere in C# planner
    /// </summary>
    public class LocationWp
    {
        public int number;              // sequential number taken from column 1 of the file
        public MAV_CMD id;				// command id
        public bool isHome;             // home waypoint is marked by this flag.
        public WaypointState waypointState;
        public DateTime? estimatedTimeOfArrival;    // when we set to reach the waypoint, we estimate arrival
        public CoordinateFrameOption coordinateFrameOption;
        public GeoPosition geoPosition;
        public double p1;				// param 1
        public double p2;				// param 2
        public double p3;				// param 3
        public double p4;				// param 4

        /// <summary>
        /// this constructor is used only for reading mission files
        /// </summary>
        /// <param name="lw"></param>
        internal LocationWp(Locationwp lw)
        {
            waypointState = WaypointState.None;

            number = lw.number;
            id = (MAV_CMD)Enum.Parse(typeof(MAV_CMD), lw.id.ToString());    // command id

            isHome = lw.ishome != 0;

            coordinateFrameOption = lw.options == 1 ? CoordinateFrameOption.MAV_FRAME_GLOBAL_RELATIVE_ALT : CoordinateFrameOption.MAV_FRAME_GLOBAL;

            // alt is in meters, can be above the ground (AGL) or above mean sea level (MSL), depending on coordinateFrameOption
            geoPosition = new GeoPosition(lw.lng, lw.lat, lw.alt);

            // p1-p4 are just float point numbers that can be added to waypoints and be interpreted by behaviors. We pass them all directly.
            p1 = lw.p1;
            p2 = lw.p2;
            p3 = lw.p3;
            p4 = lw.p4;
        }

        public Direction directionToWp(GeoPosition myPos, Direction myDir) { return new Direction() { heading = myDir.heading, bearing = myPos.bearing(this.geoPosition) }; }

        public Distance distanceToWp(GeoPosition myPos) { return geoPosition.distanceFrom(myPos); }

    };
}
