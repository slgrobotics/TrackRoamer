using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.ComponentModel;

namespace TrackRoamer.Robotics.LibMapping
{
    /// <summary>
    /// a lightweight robot state model for display and obstacle avoidance.
    /// contains current wheel power, speed, to help making decisions
    /// </summary>
    [Serializable]
    public class RobotState
    {
        public double robotWidthMeters;     // = 0.66d;     - see MapperSettings, TrackRoamerDrive.TrackRoamerDriveParams
        public double robotLengthMeters;    // = 0.82d;

        public const double wheelRadius = 0.1805d;                  // meters, also found in the Drive config, TrackRoamerDrive.TrackRoamerDriveParams
        public const double ticksPerRevolution = 6150;
        public const double distanceBetweenWheels = 0.570;          // meters


        public const double velocityFromWheelSpeedFactor = 42.5d / 1000.0d;

        public const bool IsEncoderSpeedReversed = true; 

        public double leftPower;    // -1...0...1
        public double leftSpeed;    // m/s

        public double rightPower;    // -1...0...1
        public double rightSpeed;    // m/s

        public double medianVelocity;    // m/s

        public bool manualControl;
        public string manualControlCommand;

        public bool ignoreGps;
        public bool ignoreAhrs;
        public bool ignoreLaser;
        public bool ignoreProximity;
        public bool ignoreParkingSensor;
        public bool ignoreKinectSounds;
        public bool ignoreRedShirt;
        public bool ignoreKinectSkeletons;
        public bool doLostTargetRoutine;
        public bool doPhotos;
        public bool doVicinityPlanning;

        public RobotStrategyType robotStrategyType = RobotStrategyType.None;

        public RobotTacticsType robotTacticsType = RobotTacticsType.None;
    }

    public enum RobotStrategyType
    {
        [Description(@"None")]
        None,

        [Description(@"In Transition")]
        InTransition,

        [Description(@"Lay In Wait")]
        LayInWait,

        [Description(@"Goal And Back")]
        GoalAndBack,

        [Description(@"Route Following")]
        RouteFollowing,

        [Description(@"Person Following")]
        PersonFollowing,

        [Description(@"Person Hunt")]
        PersonHunt,

        [Description(@"Random Scan")]
        RandomScan
    }

    public enum RobotTacticsType
    {
        [Description(@"None")]
        None,

        [Description(@"In Transition")]
        InTransition,

        [Description(@"Lay In Wait")]
        LayInWait,

        [Description(@"Follow Direction")]
        FollowDirection,

        [Description(@"Go Straight To Direction")]
        GoStraightToDirection,

        [Description(@"Plan And Avoid")]
        PlanAndAvoid
    }

    [Serializable]
    public class TurnState
    {
        public Direction directionInitial;
        public Direction directionCurrent;
        public Direction directionDesired;
        public DateTime started = DateTime.Now;
        public DateTime finished = DateTime.MinValue;
        public bool hasFinished = false;
        public bool wasCanceled = false;

        public bool isValid
        {
            get
            {
                return directionInitial != null && directionInitial.heading.HasValue
                    && directionCurrent != null && directionCurrent.heading.HasValue
                    && directionDesired != null && directionDesired.heading.HasValue;
            }
        }

        public bool inTurn
        {
            get
            {
                return isValid
                        //&& started != DateTime.MinValue
                        //&& (finished == DateTime.MinValue || (hasFinished || wasCanceled) && DateTime.Now < finished.AddSeconds(2.0));
                        && (finished == DateTime.MinValue || (hasFinished || wasCanceled) && DateTime.Now < finished.AddSeconds(5.0));
            }
        }

        public override string ToString()
        {
            double headingInitial = directionInitial != null && directionInitial.heading.HasValue ? directionInitial.heading.Value : double.NaN;
            double headingCurrent = directionCurrent != null && directionCurrent.heading.HasValue ? directionCurrent.heading.Value : double.NaN;
            double headingDesired = directionDesired != null && directionDesired.heading.HasValue ? directionDesired.heading.Value : double.NaN;

            return string.Format("From {0} to {1} now at {2} finished: {3} canceled: {4}", headingInitial, headingDesired, headingCurrent, hasFinished, wasCanceled);
        }
    }
}
