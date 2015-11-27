//#define TRACEDEBUG
//#define TRACEDEBUGTICKS
//#define TRACELOG

using System;

using Microsoft.Dss.ServiceModel.Dssp;
using Microsoft.Dss.ServiceModel.DsspServiceBase;

using TrackRoamer.Robotics.Utility.LibSystem;
using TrackRoamer.Robotics.LibMapping;

using drive = Microsoft.Robotics.Services.Drive.Proxy;
using powerbrick = TrackRoamer.Robotics.Services.TrackRoamerBrickPower.Proxy;
using proxibrick = TrackRoamer.Robotics.Services.TrackRoamerBrickProximityBoard.Proxy;

namespace TrackRoamer.Robotics.Services.TrackRoamerBehaviors
{
    partial class TrackRoamerBehaviorsService : DsspServiceBase
    {
        #region Drive Update and Notification handlers - State, Encoders, Velocity

        /// <summary>
        /// Handles Update notification from the Drive partner
        /// </summary>
        /// <remarks>Posts a <typeparamref name="DriveUpdate"/> request to itself.</remarks>
        /// <param name="update">notification</param>
        void DriveUpdateNotification(drive.Update update)
        {
            //LogInfo("****************************** DriveBehaviorServiceBase:: DriveUpdateNotification: state.IsEnabled=" + update.Body.IsEnabled);
            _mainPort.Post(new DriveUpdate(update.Body));
        }

        /// <summary>
        /// Handles DriveUpdate request
        /// </summary>
        /// <param name="update">request</param>
        void DriveUpdateHandler(DriveUpdate update)
        {
            //LogInfo("****************************** DriveBehaviorServiceBase:: DriveUpdateHandler: state.IsEnabled=" + update.Body.IsEnabled);
            _state.DriveState = update.Body;
            //_state.Velocity = (VelocityFromWheel(update.Body.LeftWheel) + VelocityFromWheel(update.Body.RightWheel)) / 2;
            update.ResponsePort.Post(DefaultUpdateResponseType.Instance);
        }

        /// <summary>
        /// Handles UpdateTickCount notification from the Encoder partners
        /// </summary>
        /// <remarks>Posts a <typeparamref name="EncoderReplace"/> request to itself.</remarks>
        /// <param name="update">notification</param>
        //void EncoderUpdateTickCountNotificationLeft(encoder.UpdateTickCount update)
        //{
        //    //update.Body.Count;
        //    //update.Body.TimeStamp;
        //    //_mainPort.Post(new EncoderUpdate(update.Body, 1));
        //}

        //void EncoderUpdateTickCountNotificationRight(encoder.UpdateTickCount update)
        //{
        //    //_mainPort.Post(new EncoderUpdate(update.Body, 2));
        //}

        /// <summary>
        /// Handles EncoderUpdate request
        /// </summary>
        /// <param name="update">request</param>
        //void EncoderUpdateHandler(EncoderUpdate update)
        //{
        //    LogInfo("****************************** DriveBehaviorServiceBase:: EncoderUpdateHandler: id=" + update.HardwareIdentifier + "   count=" + update.Body.Count);
        //
        //    //_state.EncoderStateLeft = update.Body;
        //    //_state.Velocity = (VelocityFromWheel(update.Body.LeftWheel) + VelocityFromWheel(update.Body.RightWheel)) / 2;
        //    update.ResponsePort.Post(DefaultUpdateResponseType.Instance);
        //}

        /// <summary>
        /// gets encoder ticks (distance) notifications directly from Power Brick
        /// </summary>
        /// <param name="update"></param>
        private void EncoderNotificationHandler(powerbrick.UpdateMotorEncoder update)
        {
            // Note: update.Body.LeftDistance / update.Body.RightDistance are absolute encoder ticks, that grow or decrease as the wheel turns. Forward rotation produces positive increments on both wheels.
            switch (update.Body.HardwareIdentifier)
            {
                case 1: // Left
                    {
                        double leftDistance = update.Body.LeftDistance ?? 0.0d;
                        if (RobotState.IsEncoderSpeedReversed)
                        {
                            leftDistance = -leftDistance;
                        }
#if TRACEDEBUGTICKS
                        Tracer.Trace("****************************** DriveBehaviorServiceBase:: EncoderNotificationHandler() -- notification - Left: d=" + leftDistance + "  t=" + update.Body.Timestamp.Ticks + "  dt=" + (DateTime.Now.Ticks - update.Body.Timestamp.Ticks));
#endif // TRACEDEBUGTICKS
                        _state.WheelsEncoderState.LeftMostRecent = update.Body.Timestamp;
                        incrementOdometry(leftDistance - (_state.WheelsEncoderState.LeftDistance ?? 0.0d), null);     // we are interested in increments
                        _state.WheelsEncoderState.LeftDistance = update.Body.LeftDistance.HasValue ? (double?)leftDistance : null;
                        if (_mapperVicinity.turnState != null && !_mapperVicinity.turnState.hasFinished)
                        {
                            proxibrick.DirectionDataDssSerializable mostRecentDirection = _state.MostRecentDirection;
                            if (mostRecentDirection != null)
                            {
                                _mapperVicinity.turnState.directionCurrent = new Direction() { heading = mostRecentDirection.heading, TimeStamp = update.Body.Timestamp.Ticks };
                            }
                        }
                    }
                    break;

                case 2: // Right
                    {
                        double rightDistance = update.Body.RightDistance ?? 0.0d;
                        if (RobotState.IsEncoderSpeedReversed)
                        {
                            rightDistance = -rightDistance;
                        }
#if TRACEDEBUGTICKS
                        Tracer.Trace("****************************** DriveBehaviorServiceBase:: EncoderNotificationHandler() -- notification - Right: d=" + rightDistance + "  t=" + update.Body.Timestamp.Ticks + "  dt=" + (DateTime.Now.Ticks - update.Body.Timestamp.Ticks));
#endif // TRACEDEBUGTICKS
                        _state.WheelsEncoderState.RightMostRecent = update.Body.Timestamp;
                        incrementOdometry(null, rightDistance - (_state.WheelsEncoderState.RightDistance ?? 0.0d));     // we are interested in increments
                        _state.WheelsEncoderState.RightDistance = update.Body.RightDistance.HasValue ? (double?)rightDistance : null;
                        if (_mapperVicinity.turnState != null && !_mapperVicinity.turnState.hasFinished)
                        {
                            proxibrick.DirectionDataDssSerializable mostRecentDirection = _state.MostRecentDirection;
                            if (mostRecentDirection != null)
                            {
                                _mapperVicinity.turnState.directionCurrent = new Direction() { heading = mostRecentDirection.heading, TimeStamp = update.Body.Timestamp.Ticks };
                            }
                        }
                    }
                    break;

                default:
                    LogError("Error: ****************************** DriveBehaviorServiceBase:: EncoderNotificationHandler() -- notification - bad HardwareIdentifier=" + update.Body.HardwareIdentifier);
                    break;
            }
        }

        /// <summary>
        /// gets encoder speed notifications directly from Power Brick
        /// </summary>
        /// <param name="update"></param>
        private void EncoderSpeedNotificationHandler(powerbrick.UpdateMotorEncoderSpeed update)
        {
#if TRACEDEBUGTICKS
            Tracer.Trace("****************************** DriveBehaviorServiceBase:: EncoderSpeedNotificationHandler() -- update - Speed:   Left=" + update.Body.LeftSpeed + " Right=" + update.Body.RightSpeed + "  t=" + update.Body.Timestamp.Ticks + "  dt=" + (DateTime.Now.Ticks - update.Body.Timestamp.Ticks));
#endif // TRACEDEBUGTICKS
            _state.WheelsEncoderState.LeftSpeed = VelocityFromWheelSpeedMetersPerSec(update.Body.LeftSpeed);
            _state.WheelsEncoderState.RightSpeed = VelocityFromWheelSpeedMetersPerSec(update.Body.RightSpeed);
            double lv = (double)_state.WheelsEncoderState.LeftSpeed;     //   m/s
            double rv = (double)_state.WheelsEncoderState.RightSpeed;    //   m/s
            double velocity = (lv + rv) / 2.0d;
            _state.Velocity = velocity;
            // fill robotState to be used for drawing on the next redraw cycle.
            _mapperVicinity.robotState.medianVelocity = velocity;
            _mapperVicinity.robotState.leftSpeed = lv;
            _mapperVicinity.robotState.rightSpeed = rv;
            if (_mapperVicinity.turnState != null && !_mapperVicinity.turnState.hasFinished)
            {
                proxibrick.DirectionDataDssSerializable mostRecentDirection = _state.MostRecentDirection;
                if (mostRecentDirection != null)
                {
                    _mapperVicinity.turnState.directionCurrent = new Direction() { heading = mostRecentDirection.heading, TimeStamp = update.Body.Timestamp.Ticks };
                }
            }
#if TRACEDEBUGTICKS
            Tracer.Trace("****************************** DriveBehaviorServiceBase:: EncoderSpeedNotificationHandler() -- Median Velocity:  " + Math.Round(velocity, 3) + " m/s   (l=" + lv + " r=" + rv + ")");
#endif // TRACEDEBUGTICKS
        }

        #region Odometry calculations

        // see http://rossum.sourceforge.net/papers/DiffSteer/   
        //     http://rossum.sourceforge.net/tools/MotionApplet/MotionApplet.html 
        //     http://sourceforge.net/projects/rossum/files/MotionApplet/MotionApplet03/

        protected double? ticksL = null;
        protected double? ticksR = null;

        protected void resetOdometry()
        {
            ticksL = null;
            ticksR = null;
        }

        protected void incrementOdometry(double? dL, double? dR)
        {
            if (dL.HasValue)
            {
                ticksL = ticksL.HasValue ? ticksL + dL : dL;
            }
            else
            {
                dL = 0.0d;  // must have value for calculations below
            }

            if (dR.HasValue)
            {
                ticksR = ticksR.HasValue ? ticksR + dR : dR;
            }
            else
            {
                dR = 0.0d;
            }

            double wheelRadius = RobotState.wheelRadius;                    // = 0.1805d; -- meters
            double TicksPerRevolution = RobotState.ticksPerRevolution;      // = 6150;
            double bodyWidth = RobotState.distanceBetweenWheels;            // = 0.570d; -- meters

            double wheelFactor = 2.0d * Math.PI * wheelRadius / TicksPerRevolution;
            double distanceLeft = dL.Value * wheelFactor;
            double distanceRight = dR.Value * wheelFactor;

            // Now, calculate the final angle, and use that to estimate
            // the final position.  See Gary Lucas' paper for derivations
            // of the equations.

            double theta = _mapperVicinity.currentOdometryTheta + (distanceLeft - distanceRight) / bodyWidth;   // radians

            _mapperVicinity.currentOdometryX += ((distanceRight + distanceLeft) / 2.0d) * Math.Cos(theta);      // meters
            _mapperVicinity.currentOdometryY += ((distanceRight + distanceLeft) / 2.0d) * Math.Sin(theta);      // meters

            _mapperVicinity.currentOdometryTheta = theta;

#if TRACEDEBUGTICKS
            Tracer.Trace("****************************** DriveBehaviorServiceBase:: incrementOdometry() -- distance meters Left: " + distanceLeft + "  Right: " + distanceRight + "   heading degrees: " + Direction.to360(toDegrees(_mapperVicinity.currentOdometryTheta)));
#endif // TRACEDEBUGTICKS

            if (_mapperVicinity.robotState.ignoreAhrs)
            {
                // do what compass data handlers usually do:
                proxibrick.DirectionDataDssSerializable newDir = new proxibrick.DirectionDataDssSerializable() { TimeStamp = DateTime.Now, heading = Direction.to360(toDegrees(_mapperVicinity.currentOdometryTheta)) };

                setCurrentDirection(newDir);
            }
        }

        #endregion // Odometry calculations

        /// <summary>
        /// Computes the wheel traveled distance in meters.
        /// </summary>
        /// <param name="ticks">tic</param>
        /// <returns>distance in meters for given amount of ticks</returns>
        private double? DistanceFromWheelTicks(double? ticks)
        {
            // "ticks" is actually encoder reading, which for Trackroamer is 6150 per one revolution of a wheel, on a 0.1805 meters radius wheels
            // gear ratio 0.136 is not playing into calculations (already accounted for in the above).
            // see src\TrackRoamer\TrackRoamerServices\Config\TrackRoamer.TrackRoamerBot.GenericDrive.Config.xml for settings (geometry) of the differential drive

            if (ticks.HasValue)
            {
                double wheelRadius = RobotState.wheelRadius;                    // = 0.1805d;
                double TicksPerRevolution = RobotState.ticksPerRevolution;      // = 6150;

                double factor = wheelRadius * 2.0d * Math.PI / TicksPerRevolution;
                double? ret = factor * ticks.Value;		 // meters
                return (ret < 10.0d) ? ret : 0.0d;       // sanity check
            }
            else
            {
                return null;
            }
        }

        private double MedianDistanceFromWheelTicks(double? ticksL, double? ticksR)
        {
            // "ticks" is actually encoder reading, which for Trackroamer is 6150 per one revolution of a wheel, on a 0.1805 meters radius wheels
            // gear ratio 0.136 is not playing into calculations (already accounted for in the above).
            // see src\TrackRoamer\TrackRoamerServices\Config\TrackRoamer.TrackRoamerBot.GenericDrive.Config.xml for settings (geometry) of the differential drive

            double wheelRadius = RobotState.wheelRadius;                    // = 0.1805d;
            double TicksPerRevolution = RobotState.ticksPerRevolution;      // = 6150;
            double factor = wheelRadius * 2.0d * Math.PI / TicksPerRevolution;

            double ticksValueL = ticksL.HasValue ? ticksL.Value : 0.0d;
            double ticksValueR = ticksR.HasValue ? ticksR.Value : 0.0d;

            double ret = factor * (ticksValueL + ticksValueR) / 2.0d;		 // meters
            return (ret < 10.0d) ? ret : 0.0d;   // sanity check
        }

        /// <summary>
        /// Computes the wheel velocity in m/s.
        /// </summary>
        /// <param name="speed">encoder speed reading</param>
        /// <returns>velocity meters/sec</returns>
        private double? VelocityFromWheelSpeedMetersPerSec(double? speed)
        {
            if (speed.HasValue)
            {
                // "speed" is actually encoder speed reading, tuned to be 0 at still and 120 at max
                // it reads "4" when one wheel rotation takes 7 seconds
                double factor = RobotState.velocityFromWheelSpeedFactor;
                if (RobotState.IsEncoderSpeedReversed)
                {
                    factor = -factor;
                }
                return factor * speed;		//     meters/sec
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// Computes the wheel velocity in mm/s.
        /// </summary>
        /// <param name="wheel">wheel</param>
        /// <returns>velocity</returns>
        private int VelocityFromWheel(Microsoft.Robotics.Services.Motor.Proxy.WheeledMotorState wheel)
        {
            if (wheel == null)
            {
                return 0;
            }
            return (int)(1000 * wheel.WheelSpeed); // meters to millimeters
        }

        /// <summary>
        /// update mapper with Odometry data. can be called any time.
        /// </summary>
        private void updateMapperWithOdometryData()
        {
            // update mapper with Odometry data:
            if (ticksL.HasValue || ticksR.HasValue)
            {
                double medianDistanceTraveled = MedianDistanceFromWheelTicks(ticksL, ticksR);

                resetOdometry();
                //Tracer.Trace("====== DriveBehaviorServiceBase:  updateMapperWithOdometryData: moved " + Math.Round(medianDistanceTraveled * 1000.0d) + " mm");
                _mapperVicinity.robotPosition.translate(_mapperVicinity.robotDirection, new Distance(medianDistanceTraveled));
            }
        }

        #endregion  // Drive Update and Notification handlers - State, Encoders, Velocity

    }
}
