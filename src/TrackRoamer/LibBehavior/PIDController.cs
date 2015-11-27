using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Dss.Core;
using Microsoft.Dss.Core.Attributes;

using TrackRoamer.Robotics.Utility.LibSystem;

namespace TrackRoamer.Robotics.LibBehavior
{
    /// <summary>
    /// Simple PID controller - see http://en.wikipedia.org/wiki/PID_controller
    /// </summary>     
    [DataContract()]
    public class PIDController : IDssSerializable
    {
        #region Default parameter values

        /// <summary>
        /// Default proportional constant
        /// </summary>
        public const double ProportionalGainDefault = 3.0d;

        /// <summary>
        /// Default integral constant
        /// </summary>
        public const double IntegralGainDefault = 0.1;

        /// <summary>
        /// Default derivative constant
        /// </summary>
        public const double DerivativeGainDefault = 0.5;

        #endregion // Default parameter values

        #region Actual parameter values - choose them carefully

        /// <summary>
        ///  Proportional constant
        /// </summary>
        [DataMember]
        public double Kp = ProportionalGainDefault;

        /// <summary>
        /// Integral constant
        /// </summary>
        [DataMember]
        public double Ki = IntegralGainDefault;

        /// <summary>
        /// Derivative constant
        /// </summary>
        [DataMember]
        public double Kd = DerivativeGainDefault;

        #endregion // Actual parameter values

        #region Maximums - exceeding them causes reset or limiting, must be set for specific use

        /// <summary>
        /// Maximum integral error
        /// </summary>
        [DataMember]
        public double MaxIntegralError = 2.0d;

        /// <summary>
        /// Maximum update interval - resets if updates dont come before that
        /// </summary>
        [DataMember]
        public double MaxUpdateIntervalSec = 10.0d;

        /// <summary>
        /// the calculated value is always within +- MaxPidValue and 0 if it is too small
        /// </summary>
        [DataMember]
        public double MaxPidValue = 1.0d;

        /// <summary>
        /// the calculated value is always within +- MaxPidValue and 0 if it is too small
        /// </summary>
        [DataMember]
        public double MinPidValue = 0.0d;

        #endregion // Maximums - exceeding them causes reset or limiting, must be set for specific use

        /// <summary>
        /// Name used for tracing, a label for this PID controller and its calculated value
        /// </summary>
        [DataMember]
        public string Name = string.Empty;

        #region Running state variables

        /// <summary>
        /// Previous error
        /// </summary>
        [DataMember(IsRequired=false)]
        public double PreviousError;

        /// <summary>
        /// Most recent error
        /// </summary>
        [DataMember(IsRequired = false)]
        public double CurrentError;

        /// <summary>
        /// Derivative error
        /// </summary>
        [DataMember(IsRequired = false)]
        public double DerivativeErrorPerSecond;

        /// <summary>
        /// Accumulated error
        /// </summary>
        [DataMember(IsRequired = false)]
        public double IntegralError;

        /// <summary>
        /// when this PID controller was last time updated with measurement
        /// </summary>
        private long lastCall = DateTime.Now.Ticks;

        #endregion // Running state variables

        /// <summary>
        /// Update the controller state
        /// </summary>
        /// <param name="newError">The new error value</param>
        /// <param name="measurementTimestamp">Time the measurement was taken, in ticks</param>
        public void Update(double newError, long measurementTimestamp)
        {
            double updateIntervalSec = ((double)(measurementTimestamp - lastCall)) / TimeSpan.TicksPerSecond;

            lastCall = measurementTimestamp;

            //Tracer.Trace("PID {0}: Update:   IntegralError={1}   PreviousError={2}   CurrentError={3}   newError={4}   updateInterval={5} ms", Name, IntegralError, PreviousError, CurrentError, newError, (updateIntervalSec * 1000.0d));

            PreviousError = CurrentError;
            CurrentError = newError;
            if (updateIntervalSec > MaxUpdateIntervalSec)
            {
                Tracer.Trace(string.Format("PID {0}: Update: reset IntegralError - interval {1} seconds is too long", Name, updateIntervalSec));
                // it has taken too long between updates, reset
                PreviousError = CurrentError;
                IntegralError = 0.0d;
            }
            
            else if (updateIntervalSec > 0.0d)
            {
                DerivativeErrorPerSecond = (CurrentError - PreviousError) / updateIntervalSec;

                IntegralError += CurrentError;

                LimitIntegralError();
            }
        }

        /// <summary>
        /// Calculate control.
        /// </summary>
        /// <returns>new PID value</returns>
        public double CalculateControl()
        {
            double pidValue =
                CurrentError * Kp +
                IntegralError * Ki +
                DerivativeErrorPerSecond * Kd;

            //Tracer.Trace("PID {3}: CalculateControl:   IntegralError={0}   PreviousError={1}   CurrentError={2}   {3}={4}", IntegralError, PreviousError, CurrentError, Name, pidValue);

            return LimitOutput(pidValue);
        }

        /// <summary>
        /// Keep Integral Error within limits, to avoid runaway integrator
        /// </summary>
        private void LimitIntegralError()
        {
            if (IntegralError >= MaxIntegralError ||
                IntegralError <= -MaxIntegralError)
            {
                IntegralError = MaxIntegralError * Math.Sign(IntegralError);
                //Tracer.Trace(string.Format("PID {0}: Update: IntegralError too large, limited to {1}", Name, IntegralError));
            }
        }

        /// <summary>
        /// keep PID output within preset limits, and zero small output (less than MinPidValue) to avoid oscillations
        /// </summary>
        /// <param name="pidValue"></param>
        /// <returns></returns>
        private double LimitOutput(double pidValue)
        {
            if (Math.Abs(pidValue) > MaxPidValue)
            {
                double limitedPidValue = MaxPidValue * Math.Sign(pidValue);
                //Tracer.Trace(string.Format("PID {0}: CalculateControl: value {1} too large - limited to {2}", Name, pidValue, limitedPidValue));
                pidValue = limitedPidValue;
            }
            else if (Math.Abs(pidValue) < MinPidValue)
            {
                //Tracer.Trace(string.Format("PID {0}: CalculateControl: value {1} too small - set to 0", Name, pidValue));
                pidValue = 0.0d;
            }

            return pidValue;
        }

        /// <summary>
        /// Reset state
        /// </summary>
        public void Reset()
        {
            Tracer.Trace(string.Format("PID {0}: Reset", Name));

            PreviousError = CurrentError = IntegralError = 0;
        }

        #region IDssSerializable semi-fake implementation to avoid DSSProxy warning

        /// <summary>
        /// copies all members to a target
        /// </summary>
        /// <param name="target"></param>
        public virtual void CopyTo(IDssSerializable target)
        {
            // throw new NotImplementedException("class PIDController does not have to implement IDssSerializable - do not call CopyTo()");

            PIDController typedTarget = target as PIDController;

            if (typedTarget == null)
                throw new ArgumentException("PIDController::CopyTo({0}) requires type {0}", this.GetType().FullName);

            typedTarget.Kp = this.Kp;
            typedTarget.Ki = this.Ki;
            typedTarget.Kd = this.Kd;
            typedTarget.MaxIntegralError = this.MaxIntegralError;
            typedTarget.MaxUpdateIntervalSec = this.MaxUpdateIntervalSec;
            typedTarget.MaxPidValue = this.MaxPidValue;
            typedTarget.MinPidValue = this.MinPidValue;
            typedTarget.Name = this.Name;
            typedTarget.PreviousError = this.PreviousError;
            typedTarget.CurrentError = this.CurrentError;
            typedTarget.DerivativeErrorPerSecond = this.DerivativeErrorPerSecond;
            typedTarget.IntegralError = this.IntegralError;
        }

        /// <summary>
        /// do not call, method Not Implemented
        /// </summary>
        /// <returns></returns>
        public object Clone()
        {
            throw new NotImplementedException("class PIDController does not have to implement IDssSerializable - do not call Clone()");
        }

        /// <summary>
        /// do not call, method Not Implemented
        /// </summary>
        /// <param name="writer"></param>
        public virtual void Serialize(System.IO.BinaryWriter writer)
        {
            throw new NotImplementedException("class PIDController does not have to implement IDssSerializable - do not call Serialize()");
        }

        /// <summary>
        /// do not call, method Not Implemented
        /// </summary>
        /// <param name="reader"></param>
        /// <returns></returns>
        public virtual object Deserialize(System.IO.BinaryReader reader)
        {
            throw new NotImplementedException("class PIDController does not have to implement IDssSerializable - do not call Deserialize()");
        }

        #endregion  // IDssSerializable semi-fake implementation to avoid DSSProxy warning
    }
}
