using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Trackroamer.Library.LibHandHardware
{
    public enum HandChannels
    {
        // Shoulder and arm direct control:
        SHOULDER_PAN = 1,
        SHOULDER_TILT = 2,
        SHOULDER_TURN = 3,
        ELBOW_ANGLE = 4,

        // Wrist and hand via I1C relay:
        THUMB = 5,
        INDEX_FINGER = 6,
        MIDDLE_FINGER = 7,
        PINKY = 8,
        WRIST_TURN=9
    }
}
