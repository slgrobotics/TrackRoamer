//#define TRACEDEBUG
//#define TRACEDEBUGTICKS
//#define TRACELOG

using System;

using System.Windows.Media.Media3D;

using Microsoft.Ccr.Core;
using Microsoft.Dss.ServiceModel.DsspServiceBase;

using ccrwpf = Microsoft.Ccr.Adapters.Wpf;

using TrackRoamer.Robotics.Utility.LibSystem;
using TrackRoamer.Robotics.LibMapping;
using libguiwpf = TrackRoamer.Robotics.LibGuiWpf;

using proxibrick = TrackRoamer.Robotics.Services.TrackRoamerBrickProximityBoard.Proxy;
using chrum6orientationsensor = TrackRoamer.Robotics.Hardware.ChrUm6OrientationSensor.Proxy;
using TrackRoamer.Robotics.Hardware.ChrUm6OrientationSensor.Proxy;
using Microsoft.Dss.ServiceModel.Dssp;
using W3C.Soap;
using System.Collections.Generic;

namespace TrackRoamer.Robotics.Services.TrackRoamerBehaviors
{
    partial class TrackRoamerBehaviorsService : DsspServiceBase
    {
        #region CH Robotics UM6 Orientation Sensor messages handlers

        /// <summary>
        /// Handle CH Robotics UM6 Orientation Sensor Notification - ProcGyro
        /// </summary>
        /// <param name="notification">ProcGyro notification</param>
        private void ChrProcGyroHandler(chrum6orientationsensor.ProcGyroNotification notification)
        {
            Tracer.Trace(string.Format("the UM6 Sensor reported ProcGyro: {0}   {1}   {2}   {3}", notification.Body.LastUpdate, notification.Body.xRate, notification.Body.yRate, notification.Body.zRate));

            // not set up in UM6, all we are using is USE_ORIENTATION_UM6 || USE_DIRECTION_UM6_QUATERNION
        }

        /// <summary>
        /// Handle CH Robotics UM6 Orientation Sensor Notification - ProcAccel
        /// </summary>
        /// <param name="notification">ProcAccel notification</param>
        private void ChrProcAccelHandler(chrum6orientationsensor.ProcAccelNotification notification)
        {
            Tracer.Trace(string.Format("the UM6 Sensor reported ProcAccel: {0}   {1}   {2}   {3}", notification.Body.LastUpdate, notification.Body.xAccel, notification.Body.yAccel, notification.Body.zAccel));

            // not set up in UM6, all we are using is USE_ORIENTATION_UM6 || USE_DIRECTION_UM6_QUATERNION
        }

        /// <summary>
        /// Handle CH Robotics UM6 Orientation Sensor Notification - ProcMag
        /// </summary>
        /// <param name="notification">ProcMag notification</param>
        private void ChrProcMagHandler(chrum6orientationsensor.ProcMagNotification notification)
        {
            Tracer.Trace(string.Format("the UM6 Sensor reported ProcMag: {0}   {1}   {2}   {3}", notification.Body.LastUpdate, notification.Body.x, notification.Body.y, notification.Body.z));

            // not set up in UM6, all we are using is USE_ORIENTATION_UM6 || USE_DIRECTION_UM6_QUATERNION
        }

        private const double CHR_EULER_YAW_FACTOR = 180.0d / 16200.0d;     // mag heading is reported as a short within +-16200 range. This is to convert it to degrees.
        private const double CHR_EULER_YAW_TRUE_NORTHOFFSET = 14.0;        // manual correction to the Euler yaw, to align it with true North reading. 14.0 is True to Magnetic North in Southern California.
        private const double CHR_QUATERNION_YAW_TRUE_NORTH_OFFSET = 14.0;  // manual correction to the Quaternion yaw, to align it with true North reading.

        /// <summary>
        /// Handle CH Robotics UM6 Orientation Sensor Notification - Euler
        /// </summary>
        /// <param name="notification">Euler notification</param>
        private void ChrEulerHandler(chrum6orientationsensor.EulerNotification notification)
        {
            //Tracer.Trace(string.Format("the UM6 Sensor reported Euler: {0}  PHI={1}   THETA={2}   PSI={3}", notification.Body.LastUpdate, notification.Body.phi, notification.Body.theta, notification.Body.psi));

            if (USE_DIRECTION_UM6_EULER)
            {
                try
                {
                    // mag heading is reported as a short within +-16200 range
                    double magHeading = Direction.to180(notification.Body.psi * CHR_EULER_YAW_FACTOR + CHR_EULER_YAW_TRUE_NORTHOFFSET);      // convert to degrees and ensure that it is within +- 180 degrees and related to True North.

                    // we still use proxibrick.DirectionDataDssSerializable because it is the direction data in the State:
                    proxibrick.DirectionDataDssSerializable newDir = new proxibrick.DirectionDataDssSerializable() { TimeStamp = DateTime.Now, heading = magHeading };

                    setCurrentDirection(newDir);
                }
                catch (Exception exc)
                {
                    Tracer.Trace("ChrEulerHandler() - " + exc);
                }
            }
        }

        /// <summary>
        /// Handle CH Robotics UM6 Orientation Sensor Notification - Quaternion
        /// </summary>
        /// <param name="notification">Quaternion notification</param>
        private void ChrQuaternionHandler(chrum6orientationsensor.QuaternionNotification notification)
        {
            //Tracer.Trace(string.Format("the UM6 Sensor reported Quaternion: {0}   {1}   {2}   {3}   {4}", notification.Body.LastUpdate, notification.Body.a, notification.Body.b, notification.Body.c, notification.Body.d));

            if (!_mapperVicinity.robotState.ignoreAhrs && (USE_ORIENTATION_UM6 || USE_DIRECTION_UM6_QUATERNION))
            {
                try
                {
                    // we switch a and b here to match WPF quaternion's orientation. Maybe there is a proper transformation to do it, but this seems to work as well.
                    Quaternion aq = new Quaternion(notification.Body.b, notification.Body.a, notification.Body.c, notification.Body.d);     // X, Y, Z, W components in WPF correspond to a, b, c, d in CH UM6 and Wikipedia

                    // have to turn it still around the Y axis (facing East):
                    Vector3D axis = new Vector3D(0, 1, 0);
                    aq = aq * new Quaternion(axis, 180);

                    libguiwpf.OrientationData attitudeData = new libguiwpf.OrientationData() { timestamp = notification.Body.LastUpdate, attitudeQuaternion = aq };

                    if (USE_ORIENTATION_UM6)
                    {
                        setGuiCurrentAttitude(attitudeData);

                        if (!_doUnitTest && !USE_DIRECTION_UM6_QUATERNION)
                        {
                            // do it now if direction is not taken from UM6 quaternion, otherwise let setCurrentDirection() call the Decide():
                            Decide(SensorEventSource.Orientation);
                        }
                    }

                    if (USE_DIRECTION_UM6_QUATERNION)
                    {
                        // do what compass data handlers do:
                        proxibrick.DirectionDataDssSerializable newDir = new proxibrick.DirectionDataDssSerializable() { TimeStamp = notification.Body.LastUpdate, heading = toDegrees(attitudeData.heading) + CHR_QUATERNION_YAW_TRUE_NORTH_OFFSET };

                        setCurrentDirection(newDir);
                    }
                }
                catch (Exception exc)
                {
                    Tracer.Trace("ChrQuaternionHandler() - " + exc);
                }
            }
        }

        protected void setGuiCurrentAttitude(libguiwpf.OrientationData attitudeData)
        {
            if (_mainWindow != null)
            {
                ccrwpf.Invoke invoke = new ccrwpf.Invoke(delegate()
                {
                    _mainWindow.CurrentAttitude = attitudeData;
                }
                );

                wpfServicePort.Post(invoke);

                Arbiter.Activate(TaskQueue,
                    invoke.ResponsePort.Choice(
                        s => { }, // delegate for success
                        ex => { } //Tracer.Error(ex) // delegate for failure
                ));
            }
        }

        #endregion // CH Robotics UM6 Orientation Sensor messages handlers

        #region CH Robotics UM6 Orientation Sensor initialization and Commands

        /// <summary>
        /// while the robot is stationary and level, we need to zero Gyros and Accelerometer X,Y plane
        /// </summary>
        /// <returns></returns>
        private IEnumerator<ITask> AhrsZeroGyrosAndAccelerometer()
        {
            Console.WriteLine("AhrsZeroGyrosAndAccelerometer - started");

            yield return Timeout(500);

            chrSetAccelRefVector();

            yield return Timeout(1000);

            chrZeroRateGyros();

            yield break;
        }

        public void chrZeroRateGyros()
        {
            chrSendCommand("ZeroRateGyros");
        }

        public void chrSetAccelRefVector()
        {
            chrSendCommand("SetAccelRefVector");
        }

        public void chrSetMagnRefVector()
        {
            chrSendCommand("SetMagnRefVector");
        }

        public void chrSendCommand(string sCmd)
        {
            ChrUm6OrientationSensorCommand cmd = new ChrUm6OrientationSensorCommand() { Command = sCmd };
            SendChrUm6OrientationSensorCommand sendCmd = new SendChrUm6OrientationSensorCommand(cmd);

            _chrUm6OrientationSensorServicePort.Post(sendCmd);
            Activate(
                Arbiter.Choice(
                    Arbiter.Receive<DefaultUpdateResponseType>(false, sendCmd.ResponsePort,
                        delegate(DefaultUpdateResponseType response)
                        {
                        }),
                    Arbiter.Receive<Fault>(false, sendCmd.ResponsePort,
                        delegate(Fault f)
                        {
                        })
                )
            );
        }

        #endregion // CH Robotics UM6 Orientation Sensor initialization and Commands

    }
}
