using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TrackRoamer.Robotics.Services.TrackRoamerBehaviors
{
    public static class ServoChannelMap
    {
        // we are using Pololu Maestro Mini 12 http://www.pololu.com/catalog/product/1352
        public const int CHANNELS_PER_DEVICE = 12;

        // channels are 0...11 for the first controller, 12...23 for the second and so on.
        // device #00025536
        public const byte notUsed0 = 0;
        public const byte notUsed1 = 1;
        public const byte platformRightRed = 2;
        public const byte platformRightYellow = 3;
        public const byte notUsed4 = 4;
        public const byte notUsed5 = 5;
        public const byte platformRightGreen = 6;
        public const byte notUsed7 = 7;
        public const byte platformLeftRed = 8;
        public const byte platformLeftYellow = 9;
        public const byte platformLeftGreen = 10;
        public const byte notUsed11 = 11;

        private const byte secondDeviceOffset = CHANNELS_PER_DEVICE;

        // device #00025966
        public const byte centerLeftGreen = 0 + secondDeviceOffset;
        public const byte centerLeftRed = 1 + secondDeviceOffset;
        public const byte centerLeftYellow = 2 + secondDeviceOffset;
        public const byte centerRightGreen = 3 + secondDeviceOffset;
        public const byte centerRightRed = 4 + secondDeviceOffset;
        public const byte centerRightYellow = 5 + secondDeviceOffset;
        public const byte frontLeftGreen = 6 + secondDeviceOffset;
        public const byte frontRightGreen = 7 + secondDeviceOffset;
        public const byte rearLeftRed = 8 + secondDeviceOffset;
        public const byte rearLeftGreen = 9 + secondDeviceOffset;
        public const byte rearRightRed = 10 + secondDeviceOffset;
        public const byte rearRightGreen = 11 + secondDeviceOffset;

        private const byte thirdDeviceOffset = CHANNELS_PER_DEVICE * 2;

        // device #00050972
        public const byte leftGunTrigger = 0 + thirdDeviceOffset;
        public const byte rightGunTrigger = 1 + thirdDeviceOffset;
        public const byte leftGunPan = 2 + thirdDeviceOffset;
        public const byte rightGunPan = 3 + thirdDeviceOffset;
        public const byte leftGunTilt = 4 + thirdDeviceOffset;
        public const byte rightGunTilt = 5 + thirdDeviceOffset;
        public const byte panKinect = 6 + thirdDeviceOffset;        // Kinect platform pan servo
        public const byte rightHeadlight = 7 + thirdDeviceOffset;
        public const byte leftHeadlight = 8 + thirdDeviceOffset;
        public const byte notUsed33 = 9 + thirdDeviceOffset;
        public const byte notUsed34 = 10 + thirdDeviceOffset;
        public const byte headPanFeedback = 11 + thirdDeviceOffset;   // input for head pan feedback

        public const byte channelsCount = CHANNELS_PER_DEVICE * 3;

        // a list of channels that are not configured for lights:
        public static byte[] notLightChannels = new byte[] { leftGunPan, rightGunPan, leftGunTilt, rightGunTilt, leftGunTrigger, rightGunTrigger, panKinect, leftHeadlight, rightHeadlight, headPanFeedback };

        public const string GunIdLeft  = "Gun Left";
        public const string GunIdRight = "Gun Right";
    }
}

