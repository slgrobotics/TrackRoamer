using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TrackRoamer.Robotics.Utility.LibPicSensors
{
    public class ParkingSensorData
    {
        public long TimeStamp = 0L;

        public double parkingSensorMetersLF;
        public double parkingSensorMetersRF;
        public double parkingSensorMetersLB;
        public double parkingSensorMetersRB;

        public virtual void setParkingSensorData(SensorsState sensorsState)
        {
            parkingSensorMetersLF = sensorsState.parkingSensorMetersLF;
            parkingSensorMetersRF = sensorsState.parkingSensorMetersRF;
            parkingSensorMetersLB = sensorsState.parkingSensorMetersLB;
            parkingSensorMetersRB = sensorsState.parkingSensorMetersRB;
        }
    }
}
