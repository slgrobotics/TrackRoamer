//------------------------------------------------------------------------------
//  <copyright file="ObstacleAvoidanceDriveTypes.cs" company="Microsoft Corporation">
//      Copyright (C) Microsoft Corporation.  All rights reserved.
//  </copyright>
//------------------------------------------------------------------------------

namespace Microsoft.Robotics.Services.ObstacleAvoidanceDrive
{
    using System;
    using System.Diagnostics;
    using Microsoft.Ccr.Core;
    using Microsoft.Dss.Core.Attributes;
    using Microsoft.Dss.Core.DsspHttp;
    using Microsoft.Dss.ServiceModel.Dssp;
    using Microsoft.Robotics.PhysicalModel;

    /// <summary>
    /// Simple PID controller - see http://en.wikipedia.org/wiki/PID_controller
    /// </summary>     
    [DataContract]
    public class PIDController
    {
        #region Default parameter values

        /// <summary>
        /// Default proportional constant
        /// </summary>
        public const double ProportionalGainDefault = 0.25;

        /// <summary>
        /// Default integral constant
        /// </summary>
        public const double IntegralGainDefault = 0.02;

        /// <summary>
        /// Default derivative constant
        /// </summary>
        public const double DerivativeGainDefault = 0.05;

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
        public double MaxIntegralError = 20.0d;

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
        public string Name = "ObstacleAvoidancePID";

        #region Running state variables

        /// <summary>
        /// Previous error
        /// </summary>
        public double PreviousError;

        /// <summary>
        /// Most recent error
        /// </summary>
        public double CurrentError;

        /// <summary>
        /// Derivative error
        /// </summary>
        public double DerivativeError;

        /// <summary>
        /// Accumulated error
        /// </summary>
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
        /// <param name="updateIntervalSec">Time since last update</param>
        // <param name="measurementTimestamp">Time the measurement was taken, in ticks</param>
        public void Update(double newError, double updateIntervalSec)
        //public void Update(double newError, long measurementTimestamp)
        {
            //double updateIntervalSec = ((double)(measurementTimestamp - lastCall)) / TimeSpan.TicksPerSecond;

            //lastCall = measurementTimestamp;

            //Debug.WriteLine("PID {0}: Update:   IntegralError={1}   PreviousError={2}   CurrentError={3}   newError={4}   updateInterval={5} ms", Name, IntegralError, PreviousError, CurrentError, newError, (updateIntervalSec * 1000.0d));

            PreviousError = CurrentError;
            CurrentError = newError;
            if (updateIntervalSec > MaxUpdateIntervalSec)
            {
                // it has taken too long between updates, reset
                PreviousError = CurrentError;
                IntegralError = 0.0d;
            }

            if (updateIntervalSec > 0.0d)
            {
                DerivativeError = (CurrentError - PreviousError) / updateIntervalSec;

                //IntegralError += DerivativeError;
                IntegralError += CurrentError;

                if (IntegralError >= MaxIntegralError ||
                    IntegralError <= -MaxIntegralError)
                {
                    IntegralError = MaxIntegralError * Math.Sign(IntegralError);
                    //Debug.WriteLine(string.Format("PID {0}: Update: IntegralError too large, limited to {1}", Name, IntegralError));
                }
            }
        }

        /// <summary>
        /// Calculate control. It does not produce a linear speed
        /// </summary>
        /// <param name="angularSpeed">Calculated angular speed</param>
        /// <param name="speed">Calculated linear speed (not used)</param>
        public void CalculateControl(out double angularSpeed, out double speed)
        {
            double pidValue =
                CurrentError * Kp +
                IntegralError * Ki +
                DerivativeError * Kd;

            speed = 0;

            if (Math.Abs(pidValue) > MaxPidValue)
            {
                double limitedPidValue = MaxPidValue * Math.Sign(pidValue);
                //Debug.WriteLine(string.Format("PID {0}: CalculateControl: value {1} too large - limited to {2}", Name, pidValue, limitedPidValue));
                pidValue = limitedPidValue;
            }
            else if (Math.Abs(pidValue) < MinPidValue)
            {
                //Debug.WriteLine(string.Format("PID {0}: CalculateControl: value {1} too small - set to 0", Name, pidValue));
                pidValue = 0.0d;
            }

            angularSpeed = pidValue;
        }

        /// <summary>
        /// Reset state
        /// </summary>
        public void Reset()
        {
            //Debug.WriteLine(string.Format("PID {0}: Reset", Name));

            PreviousError = CurrentError = IntegralError = 0;
        }
    }
}
