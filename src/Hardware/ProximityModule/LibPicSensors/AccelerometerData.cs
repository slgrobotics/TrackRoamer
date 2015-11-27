using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TrackRoamer.Robotics.Utility.LibPicSensors
{
    public class AccelerometerData
    {
        public long TimeStamp = 0L;

        // accelerometer values (x - forward, y - left, z - up)
        public double accX;
        public double accY;
        public double accZ;

        public virtual void setAccelerometerData(double aX, double aY, double aZ)
        {
            accX = aX;
            accY = aY;
            accZ = aZ;
        }
    }
}
