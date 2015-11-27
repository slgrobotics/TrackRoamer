//#define TRACEDEBUG
//#define TRACEDEBUGTICKS
//#define TRACELOG

using System;
using System.Linq;
using System.Collections.Generic;

using Microsoft.Ccr.Core;
using Microsoft.Dss.ServiceModel.DsspServiceBase;

using TrackRoamer.Robotics.Utility.LibSystem;
using TrackRoamer.Robotics.LibBehavior;

using pololumaestro = TrackRoamer.Robotics.Hardware.PololuMaestroService.Proxy;

namespace TrackRoamer.Robotics.Services.TrackRoamerBehaviors
{
    partial class TrackRoamerBehaviorsService : DsspServiceBase
    {
        #region Servo and gun controls

        PanTiltAlignment _panTiltAlignment;

        private GunTurret GunTurretLeft;
        private GunTurret GunTurretRight;

        private GunTurret[] GunTurrets;

        private void InitGunTurrets()
        {
            GunTurretLeft = new GunTurret()
            {
                panTiltAlignment = _panTiltAlignment,
                ID = ServoChannelMap.GunIdLeft,
                channelGunPan = ServoChannelMap.leftGunPan,
                channelGunTilt = ServoChannelMap.leftGunTilt,
                channelGunTrigger = ServoChannelMap.leftGunTrigger,
                servoPositionSetUs = this.ServoPositionSetUs,
                degreesPanParked = 0.0d,
                degreesTiltParked = 70.0d,
            };

            GunTurretRight = new GunTurret()
            {
                panTiltAlignment = _panTiltAlignment,
                ID = ServoChannelMap.GunIdRight,
                channelGunPan = ServoChannelMap.rightGunPan,
                channelGunTilt = ServoChannelMap.rightGunTilt,
                channelGunTrigger = ServoChannelMap.rightGunTrigger,
                servoPositionSetUs = this.ServoPositionSetUs,
                degreesPanParked = 0.0d,
                degreesTiltParked = 70.0d,
            };

            GunTurrets = new GunTurret[] { GunTurretLeft, GunTurretRight };
        }

        /// <summary>
        /// pan/tilt control for gun only
        /// </summary>
        /// <param name="panDegreesFromCenter"></param>
        /// <param name="tiltDegreesFromCenter"></param>
        private void setGunPanTilt(double panDegreesFromCenter, double tiltDegreesFromCenter)
        {
            GunTurretLeft.SetPanTilt(panDegreesFromCenter, tiltDegreesFromCenter);
            GunTurretRight.SetPanTilt(panDegreesFromCenter, tiltDegreesFromCenter);
        }

        private void setGunsParked()
        {
            GunTurretLeft.Park();
            GunTurretRight.Park();
        }

        class AngleAtTime
        {
            public double angle;
            public long ticks;
        }

        /// <summary>
        /// preferred method for pan/tilt control - sets it for both Nerf Guns
        /// </summary>
        /// <param name="panGunDegreesFromCenter">Positive values turn the turret to the right</param>
        /// <param name="tiltGunDegreesFromCenter">Positive values tilt the turret up</param>
        private void setPanTilt(double panGunDegreesFromCenter, double tiltGunDegreesFromCenter)
        {
            //panGunDegreesFromCenter = 16.56d;     // 16.56 degrees is 1/2 frame to the side, should hit the side of the alignment frame

            List<pololumaestro.ChannelValuePair> channelValues = new List<pololumaestro.ChannelValuePair>();

            // add commands to turn gun turrets:
            GunTurretLeft.GetPanTiltValues(panGunDegreesFromCenter, tiltGunDegreesFromCenter, channelValues);
            GunTurretRight.GetPanTiltValues(panGunDegreesFromCenter, tiltGunDegreesFromCenter, channelValues);

            ServoPositionSetUs(channelValues);
        }

        private void SafePosture()
        {
            GunTriggerOff();

            if (!KinectPlatformCalibrationInProgress && KinectPlatformCalibratedOk)
            {
                setGunsParked();

                SetDesiredKinectPlatformPan(0.0d);
                SetDesiredKinectTilt(5.0d);    // slightly up, expecting a human
            }
        }

        /// <summary>
        /// starts firing - activates firing mechanism of the gun until GunTriggerOff() is issued.
        /// </summary>
        private void GunTriggerOn()
        {
            Tracer.Trace("Boom!");

            GunTurretLeft.SetTrigger(true);
            GunTurretRight.SetTrigger(true);
        }

        /// <summary>
        /// completes firing - stops firing mechanism of the gun.
        /// </summary>
        private void GunTriggerOff()
        {
            GunTurretLeft.SetTrigger(false);
            GunTurretRight.SetTrigger(false);
        }

        private IEnumerator<ITask> ShootGunOnce()
        {
            GunTriggerOn();

            // 250ms plants a good single shot.
            yield return TimeoutPort((int)(_panTiltAlignment.timeGunOnMsGunLeft)).Receive();

            GunTriggerOff();

            yield break;
        }

        private IEnumerator<ITask> ShootGunMany(int howManyRounds)
        {
            GunTriggerOn();

            // 250ms plants a good single shot; for many shots we adjust that value:
            int timeToShootMs = (int)(_panTiltAlignment.timeGunOnMsGunLeft * 0.7d * howManyRounds);

            yield return TimeoutPort(timeToShootMs).Receive();

            GunTriggerOff();

            yield break;
        }

        /// <summary>
        /// Attempts to set the target (width of pulses sent) for a single channel.
        /// </summary>
        /// <param name="channel">Channel number from 0 to 23.</param>
        /// <param name="us">Desired servo position, microseconds
        /// </param>
        private void ServoPositionSetUs(Byte channel, double us)
        {
            try
            {
                // Pololu servo target is in units of quarter microseconds.
                // For typical servos, 6000 is neutral and the acceptable range is 4000-8000.
                UInt16 target = (UInt16)Math.Round(us * 4.0d);

                pololumaestro.ChannelValuePair cvp = new pololumaestro.ChannelValuePair() { Channel = channel, Target = target };

                List<pololumaestro.ChannelValuePair> channelValues = new List<pololumaestro.ChannelValuePair>();

                channelValues.Add(cvp);

                pololumaestro.PololuMaestroCommand cmd = new pololumaestro.PololuMaestroCommand() { Command = "set", ChannelValues = channelValues };

                _pololuMaestroPort.Post(new pololumaestro.SendPololuMaestroCommand(cmd));
            }
            catch (Exception exception)
            {
                LogError(exception);
            }
        }

        /// <summary>
        /// Attempts to set the target (width of pulses sent) for multiple channels.
        /// </summary>
        /// <param name="channelValues"></param>
        private void ServoPositionSetUs(List<pololumaestro.ChannelValuePair> channelValues)
        {
            if (channelValues.Any())
            {
                try
                {
                    pololumaestro.PololuMaestroCommand cmd = new pololumaestro.PololuMaestroCommand() { Command = "set", ChannelValues = channelValues };

                    _pololuMaestroPort.Post(new pololumaestro.SendPololuMaestroCommand(cmd));
                }
                catch (Exception exception)
                {
                    LogError(exception);
                }
            }
        }

        //private void TryGetTarget(Byte channel)
        //{
        //    try
        //    {
        //    }
        //    catch (Exception exception)
        //    {
        //        LogError(exception);
        //    }
        //}

        #endregion // Servo and Gun controls

    }
}
