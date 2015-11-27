using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TrackRoamer.Robotics.LibMapping
{
    /// <summary>
    /// a lightweight robot state model for display and obstacle avoidance.
    /// contains current wheel power, speed, to help making decisions
    /// </summary>
    [Serializable]
    public class RobotState
    {
        public double robotWidthMeters;
        public double robotLengthMeters;

        public double leftPower;    // -1...0...1
        public double leftSpeed;    // m/s

        public double rightPower;    // -1...0...1
        public double rightSpeed;    // m/s

        public double medianVelocity;    // m/s

        public bool manualControl;
        public string manualControlCommand;
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
                        //&& (finished == DateTime.MinValue || (hasFinished || wasCanceled) && finished.AddSeconds(2.0) > DateTime.Now);
                        && (finished == DateTime.MinValue || (hasFinished || wasCanceled) && finished.AddSeconds(5.0) > DateTime.Now);
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
