using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TrackRoamer.Robotics.Utility.LibPicSensors
{
    public class AnalogData
    {
        public long TimeStamp = 0L;

        public double analogValue1;

        public virtual void setAnalogData(SensorsState sensorsState)
        {
            analogValue1 = sensorsState.analogValue1;
        }
    }
}
