using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using TrackRoamer.Robotics.LibBehavior;
using pololumaestro = TrackRoamer.Robotics.Hardware.PololuMaestroService.Proxy;

namespace TrackRoamer.Robotics.Services.TrackRoamerBehaviors
{
    public class GunTurret
    {
        public string ID;
        public PanTiltAlignment panTiltAlignment;
        public byte channelGunPan;
        public byte channelGunTilt;
        public byte channelGunTrigger;

        // current state:
        public double currentPanGun;
        public double currentTiltGun;

        public int panGunMksLast = 0;
        public int tiltGunMksLast = 0;

        public bool isShooting = false;
        public bool isParked = false;

        // rotation limits:
        public int mksPanMax;
        public int mksPanMin;
        public int mksTiltMax;
        public int mksTiltMin;

        // preset positions:
        public double degreesPanReady = 0.0d;
        public double degreesTiltReady = 0.0d;

        public double degreesPanParked = 0.0d;
        public double degreesTiltParked = 90.0d;

        public delegate void ServoPositionSetUs(List<pololumaestro.ChannelValuePair> channelValues);

        public ServoPositionSetUs servoPositionSetUs;

        /// <summary>
        /// park the gun in safe position
        /// </summary>
        public void Park()
        {
            SetPanTilt(degreesPanParked, degreesTiltParked);
            isParked = true;
        }

        /// <summary>
        /// turn the gun in neutral position ready to aim and shoot
        /// </summary>
        public void Ready()
        {
            if (isParked)
            {
                SetPanTilt(degreesPanReady, degreesTiltReady);
            }
        }

        public void SetPanTilt(double panDegreesFromCenter, double tiltDegreesFromCenter)
        {
            List<pololumaestro.ChannelValuePair> channelValues = new List<pololumaestro.ChannelValuePair>();

            GetPanTiltValues(panDegreesFromCenter, tiltDegreesFromCenter, channelValues);

            servoPositionSetUs(channelValues);
        }

        public void SetTrigger(bool on)
        {
            if (isParked && on)
            {
                return;
            }

            // Pololu servo target is in units of quarter microseconds.
            // For typical servos, 6000 is neutral and the acceptable range is 4000-8000.
            UInt16 target = (UInt16)((on ? 2000 : 1000) << 2);

            pololumaestro.ChannelValuePair cvp = new pololumaestro.ChannelValuePair() { Channel = channelGunTrigger, Target = target };

            List<pololumaestro.ChannelValuePair> channelValues = new List<pololumaestro.ChannelValuePair>();

            channelValues.Add(cvp);

            servoPositionSetUs(channelValues);

            isShooting = on;
        }

        /// <summary>
        /// add ChannelValuePairs to channelValues
        /// </summary>
        /// <param name="panDegreesFromCenter"></param>
        /// <param name="tiltDegreesFromCenter"></param>
        /// <param name="channelValues"></param>
        public void GetPanTiltValues(double panDegreesFromCenter, double tiltDegreesFromCenter, List<pololumaestro.ChannelValuePair> channelValues)
        {
            bool isLeftGun = ID == ServoChannelMap.GunIdLeft;

            currentPanGun = panDegreesFromCenter;

            double mksPan = isLeftGun ? panTiltAlignment.mksPanGunLeft(panDegreesFromCenter) : panTiltAlignment.mksPanGunRight(panDegreesFromCenter);

            int panMks = (int)mksPan;

            if (panMks != panGunMksLast)
            {
                panGunMksLast = panMks;

                // Pololu servo target is in units of quarter microseconds.
                // For typical servos, 6000 is neutral and the acceptable range is 4000-8000.

                channelValues.Add(new pololumaestro.ChannelValuePair() { Channel = channelGunPan, Target = (UInt16)(panMks << 2) });
                isParked = false;
            }

            currentTiltGun = tiltDegreesFromCenter;

            double mksTilt = isLeftGun ? panTiltAlignment.mksTiltGunLeft(tiltDegreesFromCenter) : panTiltAlignment.mksTiltGunRight(tiltDegreesFromCenter);

            int tiltMks = (int)mksTilt;

            if (tiltMks != tiltGunMksLast)
            {
                tiltGunMksLast = tiltMks;

                // Pololu servo target is in units of quarter microseconds.
                // For typical servos, 6000 is neutral and the acceptable range is 4000-8000.

                channelValues.Add(new pololumaestro.ChannelValuePair() { Channel = channelGunTilt, Target = (UInt16)(tiltMks << 2) });
                isParked = false;
            }
        }
    }
}
