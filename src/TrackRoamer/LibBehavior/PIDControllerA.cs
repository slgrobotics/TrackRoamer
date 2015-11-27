/*
 * Copyright (c) 2013..., Sergei Grichine   http://trackroamer.com
 * 
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at 
 *    http://www.apache.org/licenses/LICENSE-2.0
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 *    
 * this is a no-warranty no-liability permissive license - you do not have to publish your changes,
 * although doing so, donating and contributing is always appreciated
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TrackRoamer.Robotics.LibBehavior
{
    public enum PidMode
    {
        MANUAL, AUTOMATIC
    }

    public enum PidDirection
    {
        DIRECT, REVERSE
    }

    /// <summary>
    /// this is a C# port of Arduino library PID_v_1 by Brett Beauregard <br3ttb@gmail.com> brettbeauregard.com
    /// For an ultra-detailed explanation of why the code is the way it is, please visit:
    ///    http://brettbeauregard.com/blog/2011/04/improving-the-beginners-pid-introduction/
    ///    http://playground.arduino.cc/Code/PIDLibrary
    /// </summary>
    public class PIDControllerA
    {
        /// <summary>
        /// Just because you set the Kp=-1 doesn't mean it actually happened.  these
        /// functions query the internal state of the PID.  they're here for display 
        /// purposes.  this are the functions the PID Front-end uses for example
        /// </summary>
        public double GetKp() { return dispKp; }
        public double GetKi() { return dispKi; }
        public double GetKd() { return dispKd; }
        public PidMode GetMode() { return inAuto ? PidMode.AUTOMATIC : PidMode.MANUAL; }
        public PidDirection GetDirection() { return controllerDirection; }


        private double dispKp;				// * we'll hold on to the tuning parameters in user-entered 
        private double dispKi;				//   format for display purposes
        private double dispKd;				//

        public double kp;                   // * (P)roportional Tuning Parameter
        public double ki;                   // * (I)ntegral Tuning Parameter
        public double kd;                   // * (D)erivative Tuning Parameter

        public double myInput;              // Input, Output, and Setpoint variables
        public double myOutput;             //
        public double mySetpoint;           //

        private ulong lastTime;
        private double IntegralError, lastInput;

        private ulong SampleTime;
        private double outMin, outMax, maxIntegralError;

        private bool inAuto = true;
        private PidDirection controllerDirection = PidDirection.DIRECT;

        /// <summary>
        /// The parameters specified here are those for for which we can't set up 
        /// reliable defaults, so we need to have the user set them.
        /// </summary>
        /// <param name="Input"></param>
        /// <param name="Output"></param>
        /// <param name="Setpoint"></param>
        /// <param name="Kp"></param>
        /// <param name="Ki"></param>
        /// <param name="Kd"></param>
        /// <param name="ControllerDirection"></param>
        /// <param name="millis">time in milliseconds</param>
        public PIDControllerA(double Input, double Output, double Setpoint,
                                double Kp, double Ki, double Kd,
                                PidDirection ControllerDirection, ulong millis)
        {
            myOutput = Output;
            myInput = Input;
            mySetpoint = Setpoint;
            inAuto = true;

            SetOutputLimits(0.0d, 255.0d, 255.0d);	//default output limit corresponds to the arduino pwm limits

            SampleTime = 100L;						//default Controller Sample Time is 0.1 seconds

            SetControllerDirection(ControllerDirection);
            SetTunings(Kp, Ki, Kd);

            lastTime = (millis - SampleTime) > 0 ? (millis - SampleTime) : 0;
        }

        /// <summary>
        /// Keep Integral Error within limits, to avoid runaway integrator
        /// </summary>
        private void LimitIntegralError()
        {
            if (IntegralError > maxIntegralError)
            {
                IntegralError = maxIntegralError * 0.75d; // 0.0d; // maxIntegralError;
            }
            else if (IntegralError < -maxIntegralError)
            {
                IntegralError = -maxIntegralError * 0.75d; // 0.0d; // -maxIntegralError;
            }
        }

        /// <summary>
        /// keep PID output within preset limits
        /// </summary>
        /// <param name="output"></param>
        /// <returns></returns>
        private double LimitOutput(double output)
        {
            if (output > outMax) output = outMax;
            else if (output < outMin) output = outMin;
            return output;
        }

        /// <summary>
        /// This, as they say, is where the magic happens.  this function should be called
        /// every time "void loop()" executes.  the function will decide for itself whether a new
        /// pid Output needs to be computed.  returns true when the output is computed,
        /// false when nothing has been done.
        /// </summary>
        /// <param name="millis">current time in milliseconds</param>
        /// <returns></returns>
        public bool Compute(ulong millis)
        {
            if (!inAuto)
                return false;

            ulong timeChange = (millis - lastTime);

            if (timeChange >= SampleTime)
            {
                /* Compute all the working error variables */
                double input = myInput;
                double error = mySetpoint - input;
                IntegralError += (ki * error);
                LimitIntegralError();
                double dInput = (input - lastInput);

                /* Compute PID Output */
                double output = kp * error + IntegralError - kd * dInput;

                myOutput = LimitOutput(output);

                /* Remember some variables for next time */
                lastInput = input;
                lastTime = millis;
                return true;
            }
            else 
                return false;
        }

        /// <summary>
        /// This function allows the controller's dynamic performance to be adjusted. 
        /// it's called automatically from the constructor, but tunings can also
        /// be adjusted on the fly during normal operation
        /// </summary>
        /// <param name="Kp"></param>
        /// <param name="Ki"></param>
        /// <param name="Kd"></param>
        public void SetTunings(double Kp, double Ki, double Kd)
        {
            if (Kp < 0 || Ki < 0 || Kd < 0) return;

            dispKp = Kp; dispKi = Ki; dispKd = Kd;

            double SampleTimeInSec = ((double)SampleTime) / 1000;
            kp = Kp;
            ki = Ki * SampleTimeInSec;
            kd = Kd / SampleTimeInSec;

            if (controllerDirection == PidDirection.REVERSE)
            {
                kp = (0 - kp);
                ki = (0 - ki);
                kd = (0 - kd);
            }
        }

        /// <summary>
        /// sets the period, in Milliseconds, at which the calculation is performed
        /// </summary>
        /// <param name="NewSampleTime"></param>
        public void SetSampleTime(int NewSampleTime)
        {
            if (NewSampleTime > 0)
            {
                double ratio = (double)NewSampleTime / (double)SampleTime;
                ki *= ratio;
                kd /= ratio;
                SampleTime = (ulong)NewSampleTime;
            }
        }

        /// <summary>
        /// This function will be used far more often than SetInputLimits.  
        ///  While the input to the controller will generally be in the 0-1023 range (which is
        ///  the default already,)  the output will be a little different.  maybe they'll
        ///  be doing a time window and will need 0-8000 or something.  Or maybe they'll
        ///  want to clamp it from 0-125.  Who knows.  At any rate, that can all be done here.
        /// </summary>
        /// <param name="Min"></param>
        /// <param name="Max"></param>
        public void SetOutputLimits(double Min, double Max, double MaxIntegralError)
        {
            if (Min >= Max) return;

            outMin = Min;
            outMax = Max;
            maxIntegralError = MaxIntegralError;

            if (inAuto)
            {
                myOutput = LimitOutput(myOutput);

                LimitIntegralError();
            }
        }

        /// <summary>
        /// Allows the controller Mode to be set to manual (0) or Automatic (non-zero)
        /// when the transition from manual to auto occurs, the controller is
        /// automatically initialized
        /// </summary>
        /// <param name="Mode"></param>
        public void SetMode(PidMode mode)
        {
            bool newAuto = (mode == PidMode.AUTOMATIC);
            if (newAuto == !inAuto)
            {
                // we just went from manual to auto
                Initialize();
            }
            inAuto = newAuto;
        }

        /// <summary>
        /// does all the things that need to happen to ensure a bumpless transfer
        /// from manual to automatic mode.
        /// </summary>
        public void Initialize()
        {
            IntegralError = myOutput;
            lastInput = myInput;
            LimitIntegralError();
        }

        /// <summary>
        /// The PID will either be connected to a DIRECT acting process (+Output leads 
        /// to +Input) or a REVERSE acting process(+Output leads to -Input.)  we need to
        /// know which one, because otherwise we may increase the output when we should
        /// be decreasing.  This is called from the constructor.
        /// </summary>
        /// <param name="direction"></param>
        public void SetControllerDirection(PidDirection direction)
        {
            if (inAuto && direction != controllerDirection)
            {
                kp = (0 - kp);
                ki = (0 - ki);
                kd = (0 - kd);
            }
            controllerDirection = direction;
        }

        /// <summary>
        /// convenience method to feed constructor and Compute()
        /// </summary>
        /// <returns></returns>
        public static ulong GetMillis()
        {
            return (ulong)(DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond);
        }
    }
}
