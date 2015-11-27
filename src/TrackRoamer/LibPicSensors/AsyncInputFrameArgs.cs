using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TrackRoamer.Robotics.Utility.LibPicSensors
{
    public class AsyncInputFrameArgs : EventArgs
    {
        public long timestamp;

        public bool fromPingScanStop;

        public double dPos1Mks;
        public double dPos2Mks;

        public double dPing1DistanceM;
        public double dPing2DistanceM;

        public SensorsState sensorsState;

        public AsyncInputFrameArgs(int servo1target, int servo2target, int ping1value, int ping2value, bool fpss, SensorsState sensState)
        {
            timestamp = DateTime.Now.Ticks;

            fromPingScanStop = fpss;

            dPos1Mks = ProximityBoard.servoTargetToMks(servo1target);
            dPos2Mks = ProximityBoard.servoTargetToMks(servo2target);

            dPing1DistanceM = ProximityBoard.pingValueToDistanceM(ping1value);
            dPing2DistanceM = ProximityBoard.pingValueToDistanceM(ping2value);

            sensorsState = sensState;
        }
    }
}

