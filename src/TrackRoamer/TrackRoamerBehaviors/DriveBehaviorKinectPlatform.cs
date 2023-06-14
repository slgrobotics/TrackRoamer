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

using kinectProxy = Microsoft.Robotics.Services.Sensors.Kinect.Proxy;
using pololumaestro = TrackRoamer.Robotics.Hardware.PololuMaestroService.Proxy;
using System.Diagnostics;

namespace TrackRoamer.Robotics.Services.TrackRoamerBehaviors
{
    partial class TrackRoamerBehaviorsService : DsspServiceBase
    {
        /*
         * In Trackroamer design there is a rotating platform that carries Kinect and two Nerf Swarmfire guns. 
         * The guns have their own pan and tilt actuators, independent but relative to the Platform.
         * The platform's current pan angle is measured by Proximity Brick - raw analog value is delivered by trpbUpdateAnalogNotification()
         * and stored in _state.MostRecentAnalogValues.analogValue1.
         * Calibrated/scaled value in degrees is constantly updating in _state.currentPanKinect, but can be used only if calibration was successful.
         */

        protected double? kinectPanDesiredDegrees = null;     // degrees, relative to robot; usually pointing to target; used as a setpoint for PID controlling the platform servo. Current measured turn is in _state.currentPanKinect
        protected Object kinectPanDesiredDegreesLock = new Object();
        protected int kinectTiltDesiredDegrees = 5;

        // Kinect Pan actual degrees stored in _state.currentPanKinect
        // Kinect Tilt actual degrees stored in _state.currentTiltKinect. They are related to horizon as measured by Kinect device's accelerometer.

        private DateTime lastKinectTiltUpdate = DateTime.Now.AddSeconds(30.0d);
        private double kinectTiltUpdateIntervalSec = 5.0d;

        #region Kinect Platform direct control

        /// <summary>
        /// Sets pan for Kinect platform.
        /// Can be called only from CalibrateKinectPlatform() and computeAndExecuteKinectPlatformTurn(), as normally the platform is under PID control.
        /// To set desired pan setpoint, use 
        /// </summary>
        /// <param name="panKinectDegreesFromCenter">Positive values turn the Kinect platform to the right</param>
        private void setPanKinectPlatform(double panKinectDegreesFromCenter)
        {
            //panKinectDegreesFromCenter = 16.56d;      // 16.56 degrees is 1/2 frame to the side, should hit the side of the alignment frame

            double mks = _panTiltAlignment.mksPanKinect(panKinectDegreesFromCenter);

            setPanKinectMks(mks);
        }

        private void setPanKinectMks(double mks)
        {
            int panKinectMks = (int)Math.Round(mks);

            if (panKinectMks != panKinectMksLast)
            {
                panKinectMksLast = panKinectMks;

                // only allow rotation within -80, +80 degrees. Physical limits are a tad outside -90, +90
                double mks80 = _panTiltAlignment.mksPanKinect(80.0d);
                double mks80M = _panTiltAlignment.mksPanKinect(-80.0d);

                if (mks >= Math.Min(mks80, mks80M) && mks <= Math.Max(mks80, mks80M))
                {
                    ServoPositionSetUs(ServoChannelMap.panKinect, mks);
                }
            }
        }

        #endregion // Kinect Platform direct control

        #region Kinect Platform calibration

        private bool KinectPlatformCalibratedOk = false;
        private bool KinectPlatformCalibrationInProgress = false;
        private int panKinectMksLast = 0;

        private int panKinectMksMin = 1000;      // will be overwritten based on calibration
        private int panKinectMksMax = 2000;

        private IEnumerator<ITask> CalibrateKinectPlatform()
        {
            // Analog value is delivered by trpbUpdateAnalogNotification().
            // Allocate three values for three calibration positions:
            double analogValueAt0   = 0.0d;
            double analogValueAtM70 = 0.0d;
            double analogValueAtP70 = 0.0d;

            KinectPlatformCalibratedOk = false;
            KinectPlatformCalibrationInProgress = true;

            yield return Timeout(2000);

            Talker.Say(9, "Warning: Calibrating Kinect Platform");

            yield return Timeout(1000);

            // at this point Kinect platform is probably already set to 0 degrees, so we don't need to wait that long for it to settle. 
            setPanKinectPlatform(0.0d);
            yield return Timeout(2000);

            analogValueAt0 = _state.MostRecentAnalogValues.analogValue1;

            //Talker.Say(9, "Turning Kinect Platform to 70");

            int i = 0;
            for (; i <= 70; i += 2)
            {
                setPanKinectPlatform((double)i);
                yield return Timeout(80);
            }
            yield return Timeout(1000);
            analogValueAtP70 = _state.MostRecentAnalogValues.analogValue1;

            //Talker.Say(9, "Turning Kinect Platform to -70");

            for (; i >= -70; i -= 2)
            {
                setPanKinectPlatform((double)i);
                yield return Timeout(80);
            }
            yield return Timeout(1000);
            analogValueAtM70 = _state.MostRecentAnalogValues.analogValue1;

            // back to 0 degrees:
            for (; i <= 0; i += 2)
            {
                setPanKinectPlatform((double)i);
                yield return Timeout(80);
            }

            // expecting values:  -70=526  0=368  +70=203   (new two-gun platform, PIC-measured on Pin2)
            // expecting values:  -70=566  0=391  +70=214   (new two-gun platform, Pololu Mini Maestro measured in hardware test. Not available via PololuMaestroService)

            Tracer.Trace(string.Format("Calibrating Kinect Platform: -70={0}  0={1}  +70={2}", analogValueAtM70, analogValueAt0, analogValueAtP70));

            double spanHKinect0_M70 = 70.0d;
            double spanHKinect0_P70 = 70.0d;

            _panTiltAlignment.computeCalibrationKinectAnalog(analogValueAtM70, analogValueAt0, analogValueAtP70, spanHKinect0_M70, spanHKinect0_P70);

            /*
            // for debugging - position the platform in the same spots again and check tracing at DriveBehaviorProxibrick.cs:trpbUpdateAnalogNotification()
            yield return Timeout(5000);
            setPanKinect(70.0d);
            yield return Timeout(5000);
            Tracer.Trace("Checking calibration at +70 - measured value is " + currentPanKinect + " degrees");
            setPanKinect(0.0d);
            yield return Timeout(5000);
            Tracer.Trace("Checking calibration at +0 - measured value is " + currentPanKinect + " degrees");
            setPanKinect(-70.0d);
            yield return Timeout(5000);
            Tracer.Trace("Checking calibration at -70 - measured value is " + currentPanKinect + " degrees");
            */

            // do some sanity checking:
            KinectPlatformCalibratedOk =
                                   analogValueAtM70 > 510 && analogValueAtM70 < 670
                                && analogValueAt0   > 340 && analogValueAt0   < 470
                                && analogValueAtP70 > 100 && analogValueAtP70 < 280;

            Talker.Say(9, KinectPlatformCalibratedOk ? "Finished Calibrating" : "Error! Error! Error Calibrating Kinect Platform");

            KinectPlatformCalibrationInProgress = false;

            // if calibration succeeded, the platform will be smoothly handled in computeAndExecuteKinectPlatformTurn() using PID and EMA.
            SetDesiredKinectPlatformPan(0.0d);
            SetDesiredKinectTilt(5.0d);    // slightly up, expecting a human
            if (KinectPlatformCalibratedOk)
            {
                panKinectMksMin = 1000;
                panKinectMksMax = 2000;

                SpawnIterator(RunKinectPlatformPid);
            }

            SetLightsNormal();
       }

        #endregion // Kinect Platform calibration

        #region Kinect Tilt control

        private IEnumerator<ITask> RunKinectPlatformPid()
        {
            while (true)
            {
                DateTime started = DateTime.Now;

                computeAndExecuteKinectPlatformTurn();

                double timeSpentMs = (DateTime.Now - started).TotalMilliseconds;
                double timeToSleep = samplingIntervalMilliSeconds - timeSpentMs - 9.0d;     // small adjustment based on measured values

                if (timeToSleep > 5.0d)
                {
                    yield return Timeout(timeToSleep);
                }
            }
        }

        protected void AdjustKinectTilt()
        {
            // take care of the Kinect tilt:
            if ((DateTime.Now - lastKinectTiltUpdate).TotalSeconds > kinectTiltUpdateIntervalSec)
            {
                lastKinectTiltUpdate = DateTime.Now;
                // Calls to set the elevation angle are limited to one per second and a maximum of 15 calls in any 20-second period.
                UpdateKinectTilt(kinectTiltDesiredDegrees);
            }
        }

        public void SetDesiredKinectPlatformPan(double? degrees)
        {
            lock (kinectPanDesiredDegreesLock)
            {
                kinectPanDesiredDegrees = degrees;
            }
        }

        public void SetDesiredKinectTilt(double degrees)
        {
            int d = (int)Math.Round(Math.Min(27.0d, Math.Max(-27.0d, degrees)));

            kinectTiltDesiredDegrees = d;
        }

        private int lastKinectTiltDegrees = 0;

        public void UpdateKinectTilt(int degrees)
        {
           Tracer.Trace("Kinect tilt: desired degrees: " + degrees);

            /*
             * Nothing I tried worked here. It hangs for a second, preventing main loop flow.
             * 

           if(degrees == lastKinectTiltDegrees)
            {
                return;
            }

            lastKinectTiltDegrees = degrees; // fault or success

            kinectProxy.UpdateTiltRequest request = new kinectProxy.UpdateTiltRequest();
            request.Tilt = degrees;
 
            Tracer.Trace("Kinect tilt: trying...    degrees: " + degrees);

            Activate(
            this.kinectPort.UpdateTilt(request).Choice(   // this will hang for a second or two, can't call it often
                    success =>
                    {
                        // nothing to do
                        Tracer.Trace("Kinect tilt: success, degrees: " + degrees);
                    },
                    fault =>
                    {
                        Tracer.Error("failed to update Kinect tilt to degrees: " + degrees);
                        // the fault handler is outside the WPF dispatcher
                        // to perfom any UI related operation we need to go through the WPF adapter

                        // show an error message
                        this.wpfServicePort.Invoke(() => this.userInterface.ShowFault(fault));
                    }
                ));
            */

            /*
            Activate(
                Arbiter.Choice(
                    this.kinectPort.UpdateTilt(request),   // this will hang for a second or two, can't call it often
                    success =>
                    {
                        // nothing to do
                        Tracer.Trace("tilt: success, degrees: " + degrees);
                    },
                    fault =>
                    {
                        Tracer.Error("failed to update Kinect tilt to degrees: " + degrees);
                        // the fault handler is outside the WPF dispatcher
                        // to perfom any UI related operation we need to go through the WPF adapter

                        // show an error message
                        this.wpfServicePort.Invoke(() => this.userInterface.ShowFault(fault));
                    }));
            */
        }

        #endregion // Kinect Tilt control

        #region Kinect Platform PID/EMA control

        double samplingIntervalMilliSeconds = 50.0d; // Sleep interval for background iterator. Proximity Brick delivers roughly 26 reading per second, servo is happy at 10 changes per second
        double samplingIntervalMeasuredMilliSeconds;
        int pidSampleTimeMs = 20;     // treshold after which PID will recalculate, better keep it small


        double displayIntervalMilliSeconds = 1000.0d;
        private DateTime lastDisplayTime;

        private DateTime lastReadServos = DateTime.MinValue;
        private double servoInputMks = 1500.0d;

        private bool usePID = true;

        /// <summary>
        /// Takes kinectPanDesiredDegrees and _state.currentPanKinect and sets Pololu servo values to turn head appropriately.
        /// We call this method immediately after receiving the measurement and storing it in _state.currentPanKinect,
        /// and also on a regular basis from the background iterator (RunKinectPlatformPid()).
        /// </summary>
        protected void computeAndExecuteKinectPlatformTurn()
        {
            double? panDegreesFromCenter = null;

            lock (kinectPanDesiredDegreesLock)
            {
                // kinectPanDesiredDegrees can be changed to null while we are in this method - make local copy:
                panDegreesFromCenter = kinectPanDesiredDegrees;
            }

            if (panDegreesFromCenter.HasValue && !KinectPlatformCalibrationInProgress && KinectPlatformCalibratedOk)
            {
                DateTime tickTime = DateTime.Now;
                ulong millis = (ulong)(tickTime.Ticks / TimeSpan.TicksPerMillisecond);

                if (pidA == null)
                {
                    InitPid(millis);
                }

                TimeSpan samplingTimeSpan = tickTime - lastReadServos;
                samplingIntervalMeasuredMilliSeconds = samplingTimeSpan.TotalMilliseconds;

                double measuredValueMks = MapMeasuredPos(_state.currentPanKinect);

                double panKinectSetpointMks = _panTiltAlignment.mksPanKinect(panDegreesFromCenter.Value);

                double errorMks = panKinectSetpointMks - measuredValueMks;

                // compute servo input:
                servoInputMks = ComputeServoInputWithPID(servoInputMks, panKinectSetpointMks, measuredValueMks, millis);

                // apply servo input to Kinect pan actuator. We spend around 1ms here:
                if (usePID)
                {
                    setPanKinectMks(servoInputMks);
                }
                else
                {
                    setPanKinectMks(panKinectSetpointMks);
                }

                // housekeeping:
                if ((tickTime - lastDisplayTime).TotalMilliseconds > displayIntervalMilliSeconds)
                {
                    lastDisplayTime = tickTime;

                    // display last processingTimeMs - not distorted by canDisplay:
                    Debug.WriteLine(string.Format("diff: {0,5:0}     sampling interval ms:    measured: {1,6:#.00}  PID SampleTime: {2,6:#.00}", errorMks, samplingIntervalMeasuredMilliSeconds, samplingIntervalMilliSeconds));
                }

                lastReadServos = tickTime;
            }
        }

        //private Ema measuredValueEma = new Ema(5);

        /// <summary>
        /// use exponential moving average to smooth the measured analog platform positioning data 
        /// </summary>
        /// <param name="measuredValueDegrees"></param>
        /// <returns></returns>
        private double MapMeasuredPos(double measuredValueDegrees)
        {
            double measuredValueMks = _panTiltAlignment.mksPanKinect(measuredValueDegrees);
            return measuredValueMks;

            // see trpbUpdateAnalogNotification() - analog value from proxibrick is smoothened already by its EMA

            // compute exponential moving average to smooth the measured data:
            //double measuredValueEmaMks = measuredValueEma.Compute(measuredValueMks);

            //return measuredValueEmaMks;
        }

        // pidA is responsible for a difference between the measuredValueMks
        private PIDControllerA pidA = null;

        private void InitPid(ulong millis)
        {
            double kp = 0.0d;
            double ki = 0.0d;
            double kd = 0.0d;
            double limits = 300.0d;     // PID output limits, a single cycle correction max

            kp = 0.055d;
            //ki = 0.000001d;
            kd = 0.005d;

            pidA = new PIDControllerA(0, 0, 0, kp, ki, kd, PidDirection.DIRECT, millis);

            pidA.SetSampleTime(pidSampleTimeMs);

            pidA.SetOutputLimits(-limits, limits, 70);

            pidA.Initialize();
        }

        /// <summary>
        /// computes PID output
        /// </summary>
        /// <param name="setpointMks"></param>
        /// <param name="measuredValueMks"></param>
        /// <param name="millis">current time value</param>
        /// <returns></returns>
        private double ComputeServoInputWithPID(double servoInputMks, double setpointMks, double measuredValueMks, ulong millis)
        {
            // we come here every 50ms, millis is a very large number increasing by 50 every time
            //servoInputMks = setpointMks;

            // don't allow PID inputs exceed platform limitations:
            setpointMks = GeneralMath.constrain(setpointMks, (double)panKinectMksMin, (double)panKinectMksMax);
            measuredValueMks = GeneralMath.constrain(measuredValueMks, (double)panKinectMksMin, (double)panKinectMksMax);

            pidA.mySetpoint = setpointMks;          // around 1500. Where we want to be.
            pidA.myInput = measuredValueMks;        // around 1500. Where we currently are.

            pidA.Compute(millis);

            // pidA operates on error=mySetpoint - myInput
            // produces pidA.myOutput - a number around 0, limited by pidA.SetOutputLimits()

            //Tracer.Trace("pidA:  setpointMks=" + setpointMks + "     measuredValueMks=" + measuredValueMks + "    millis=" + millis + "     pidA.myOutput=" + pidA.myOutput);

            // don't allow PID output exceed platform limitations:
            servoInputMks = GeneralMath.constrain(servoInputMks + pidA.myOutput, (double)panKinectMksMin, (double)panKinectMksMax);

            return servoInputMks;
        }

        #endregion // Kinect Platform PID/EMA control


    }
}
