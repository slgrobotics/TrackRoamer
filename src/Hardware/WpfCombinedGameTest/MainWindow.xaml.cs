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
using System.Speech.Synthesis;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;
using System.IO;

using System.Threading;
using System.Threading.Tasks;
using System.Windows.Threading;
using System.Speech.Synthesis;
using System.Diagnostics;

using kinect = Microsoft.Kinect;

using TrackRoamer.Robotics.LibBehavior;
using TrackRoamer.Robotics.LibActuators;
using TrackRoamer.Robotics.LibBehavior;
using Trackroamer.Library.LibAnimatronics;
using Trackroamer.Robotics.AnimatronicHead;

namespace WpfCombinedGameTest
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        #region Kinect platform pan variables

        double samplingIntervalSeconds = 0.1d;
        double samplingIntervalMilliSeconds;
        double displayIntervalMilliSeconds = 300.0d;

        DispatcherTimer dispatcherTimer;

        #endregion // Kinect platform pan variables

        DispatcherTimer dispatcherTimerGun;     // ~250ms shooting interval
        DispatcherTimer dispatcherTimerGun2;
        DispatcherTimer dispatcherTimerAnimScheduler;

        TrackRoamer.Robotics.LibBehavior.PanTiltAlignment _panTiltAlignment;
        Trackroamer.Library.LibAnimatronics.PanTiltAlignment _panTiltAlignmentAnim;


        IBrickConnector brickConnector;

        Animations animations = new Animations();
        AnimationCombo animationCombo = new AnimationCombo();

        #region MainWindow lifecycle

        public MainWindow()
        {
            InitializeComponent();

            EnsureBrickConnector();

            trackWhatComboBox.Items.Add("CG");
            trackWhatComboBox.Items.Add(kinect.JointType.HandLeft);
            trackWhatComboBox.Items.Add(kinect.JointType.HandRight);
            trackWhatComboBox.Items.Add(kinect.JointType.FootLeft);
            trackWhatComboBox.Items.Add(kinect.JointType.FootRight);

            trackWhatComboBox.SelectedIndex = 0;

            dispatcherTimerGun = new DispatcherTimer(DispatcherPriority.Normal, this.Dispatcher);
            dispatcherTimerGun.Tick += new EventHandler(dispatcherTimerGun_Tick);

            dispatcherTimerGun2 = new DispatcherTimer(DispatcherPriority.Normal, this.Dispatcher);
            dispatcherTimerGun2.Tick += new EventHandler(dispatcherTimerGun2_Tick);

            dispatcherTimerAnimScheduler = new DispatcherTimer(DispatcherPriority.Normal, this.Dispatcher);
            dispatcherTimerAnimScheduler.Tick += new EventHandler(dispatcherTimerAnimScheduler_Tick);

            #region Kinect platform pan related

            samplingIntervalMilliSeconds = samplingIntervalSeconds * 1000.0d;

            InitPid();

            dispatcherTimer = new DispatcherTimer(DispatcherPriority.Normal, this.Dispatcher);
            dispatcherTimer.Interval = new TimeSpan((long)(samplingIntervalMilliSeconds * TimeSpan.TicksPerMillisecond));
            dispatcherTimer.Tick += new EventHandler(dispatcherTimer_Tick);

            #endregion // Kinect platform pan related

            this.Closing += new System.ComponentModel.CancelEventHandler(MainWindow_Closing);
        }

        private void EnsureBrickConnector()
        {
            brickConnector = new BrickConnectorArduino();
        }

        private void OpenBrickConnector()
        {
            brickConnector.Open("COM16");
        }

        private void Window_Loaded(object sender, EventArgs e)
        {
            _panTiltAlignment = TrackRoamer.Robotics.LibBehavior.PanTiltAlignment.RestoreOrDefault();
            _panTiltAlignmentAnim = Trackroamer.Library.LibAnimatronics.PanTiltAlignment.RestoreOrDefault();

            bool rightGun = rightGunCheckBox.IsChecked.GetValueOrDefault();

            if (rightGun)
            {
                //panAlignScrollBar.Value = _panTiltAlignment.panAlignGunRight;
                //panFactorScrollBar.Value = _panTiltAlignment.panFactorGunRight;
                //tiltAlignScrollBar.Value = _panTiltAlignment.tiltAlignGunRight;
                //tiltFactorScrollBar.Value = _panTiltAlignment.tiltFactorGunRight;
            }
            else
            {
                //panAlignScrollBar.Value = _panTiltAlignment.panAlignGunLeft;
                //panFactorScrollBar.Value = _panTiltAlignment.panFactorGunLeft;
                //tiltAlignScrollBar.Value = _panTiltAlignment.tiltAlignGunLeft;
                //tiltFactorScrollBar.Value = _panTiltAlignment.tiltFactorGunLeft;
            }

            timeGunOnMsTextBox.Text = string.Format("{0:0}", _panTiltAlignment.timeGunOnMsGunLeft);

            OpenBrickConnector();

            SafePosture();

            InitKinect();

            soundsBasePathDefault = @"C:\Projects\Robotics\src\Hardware\Music";

            soundsBasePathLabel.Content = "Sound files folder: " + soundsBasePathDefault;

            PopulatePlaySoundCombo();

            InitPidControls();

            CollectAndApplyPidControls();

            dispatcherTimer.Start();

            speak("Ready for action!");
        }

        void MainWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            dispatcherTimer.Stop();

            SafePosture();

            brickConnector.Close();

            if (null != this.sensor)
            {
                this.sensor.Stop();
            }
        }

        private void Window_Closed(object sender, EventArgs e)
        {
        }

        private void rightGunCheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            //panAlignScrollBar.Value = _panTiltAlignment.panAlignGunLeft;
            //panFactorScrollBar.Value = _panTiltAlignment.panFactorGunLeft;
            //tiltAlignScrollBar.Value = _panTiltAlignment.tiltAlignGunLeft;
            //tiltFactorScrollBar.Value = _panTiltAlignment.tiltFactorGunLeft;
        }

        private void rightGunCheckBox_Checked(object sender, RoutedEventArgs e)
        {
            //panAlignScrollBar.Value = _panTiltAlignment.panAlignGunRight;
            //panFactorScrollBar.Value = _panTiltAlignment.panFactorGunRight;
            //tiltAlignScrollBar.Value = _panTiltAlignment.tiltAlignGunRight;
            //tiltFactorScrollBar.Value = _panTiltAlignment.tiltFactorGunRight;
        }

        #endregion // MainWindow lifecycle

        #region Helpers

        delegate void UpdateLabelDelegate(string txt);

        void updatePmValuesLabel(string txt)
        {
            //pmValuesLabel.Content = txt;
        }

        void updatePanMksLabel(string txt)
        {
            //panMksLabel.Content = txt;
        }

        void updateTiltMksLabel(string txt)
        {
            //tiltMksLabel.Content = txt;
        }

        /// <summary>
        /// Displays an exception to the user by popping up a message box.
        /// </summary>
        void displayException(Exception exception)
        {
            StringBuilder stringBuilder = new StringBuilder();
            do
            {
                stringBuilder.Append(exception.Message + "  ");
                exception = exception.InnerException;
            }
            while (exception != null);

            this.Dispatcher.Invoke(new UpdateLabelDelegate(updatePmValuesLabel), stringBuilder.ToString());
        }

        SpeechSynthesizer synthesizer = new SpeechSynthesizer();

        private void speak(string tosay = null, int rate = 0)
        {
            if (string.IsNullOrEmpty(tosay))
            {
                tosay = "Peace!";
            }

            synthesizer.SpeakAsyncCancelAll();

            // Voice:
            //synthesizer.SelectVoice("Microsoft Anna");    // the only one installed by default
            //synthesizer.SelectVoice("Microsoft Sam");

            //var aaa = synthesizer.GetInstalledVoices();

            // Volume:
            synthesizer.Volume = 100;  // 0...100

            // talking speed:
            synthesizer.Rate = rate;     // -10...10

            // Synchronous
            //synthesizer.Speak("Hi Speak something ");

            // Asynchronous
            synthesizer.SpeakAsync(tosay);
        }

        #endregion // Helpers

        #region Animation controls

        private void setAnimationCombo(string comboName, bool? dr = null)
        {
            double scale = scaleScrollBar.Value / 100.0d;

            if (currentAnimationName != comboName || !currentAnimationRepeating && (DateTime.Now - currentAnimationTime).TotalSeconds > 10.0d * scale)
            {
                bool doRepeat = dr.HasValue ? dr.Value : RepeatCheckBox.IsChecked.GetValueOrDefault();

                if (!string.IsNullOrEmpty(comboName))
                {
                    string[] animNames = animationCombo[comboName];
                    setAnimationCombo(animNames, scale, doRepeat);

                    currentAnimationName = comboName;
                    currentAnimationRepeating = doRepeat;
                    currentAnimationTime = DateTime.Now;
                }
            }
        }

        public void setAnimationCombo(string[] animNames, double scale = 1.0d, bool doRepeat = false)
        {
            SetAnimations(animNames, scale, doRepeat);
        }

        private void SetAnimations(string[] animNames, double scale = 1.0d, bool doRepeat = false)
        {
            foreach (string name in animNames)
            {
                Animation anim = animations[name];

                brickConnector.setAnimation(anim, scale, doRepeat);
            }
        }

        private void scaleScrollBar_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            setScale(e.NewValue);
        }

        private void setScale(double scale)
        {
            this.Dispatcher.Invoke(new UpdateLabelDelegate(updateScaleLabel), string.Format("{0,4:0}%", scale));
        }

        void updateScaleLabel(string txt)
        {
            scaleLabel.Content = txt;
        }

        #endregion // Animation controls

        #region Servo and gun controls

        PololuMaestroConnector pololuMaestroConnector = new PololuMaestroConnector();

        private double panKinectSetpointMks = 1500;

        private void setPanKinectMks(double mks)
        {
            int valMks = (int)Math.Round(Math.Min(Math.Max(mks, 861.0d), 2190.0d));

            this.pololuMaestroConnector.setPanKinectMks(valMks);
        }

        private void setPanKinect(double degreesFromCenter)
        {
            this.pololuMaestroConnector.setPanKinect(degreesFromCenter);
        }

        private void setPan(double degreesFromCenter)
        {
            bool rightGun = rightGunCheckBox.IsChecked.GetValueOrDefault();

            this.pololuMaestroConnector.setPan(degreesFromCenter, rightGun);

            this.Dispatcher.Invoke(new UpdateLabelDelegate(updatePmValuesLabel), string.Format("Pan: {0:0}\r\nTilt: {1:0}", this.pololuMaestroConnector.currentPanGunLeft, this.pololuMaestroConnector.currentTiltGunLeft));
            this.Dispatcher.Invoke(new UpdateLabelDelegate(updatePanMksLabel), string.Format("{0,4} mks {1,4:0} degrees", this.pololuMaestroConnector.panMksLastGunLeft, this.pololuMaestroConnector.currentPanGunLeft));
        }

        private void setTilt(double degreesFromCenter)
        {
            bool rightGun = rightGunCheckBox.IsChecked.GetValueOrDefault();

            this.pololuMaestroConnector.setTilt(degreesFromCenter, rightGun);

            this.Dispatcher.Invoke(new UpdateLabelDelegate(updatePmValuesLabel), string.Format("Pan: {0:0}\r\nTilt: {1:0}", this.pololuMaestroConnector.currentPanGunLeft, this.pololuMaestroConnector.currentTiltGunLeft));
            this.Dispatcher.Invoke(new UpdateLabelDelegate(updateTiltMksLabel), string.Format("{0,4} mks {1,4:0} degrees", this.pololuMaestroConnector.tiltMksLastGunLeft, this.pololuMaestroConnector.currentTiltGunLeft));
        }

        private void SafePosture()
        {
            GunOff(true);
            GunOff(false);
            headlightsBoth(false);
            pololuMaestroConnector.setPanTilt(0.0d, 0.0d, 0.0d, 0.0d, 0.0d);
            brickConnector.clearAnimations();
            brickConnector.setPanTilt(0.0d, 0.0d);
        }

        private void GunOn(bool rightGun)
        {
            shootButton.Content = "Boom!";

            isShooting = true;

            pololuMaestroConnector.TrySetTarget(rightGun ? ServoChannelMap.rightGunTrigger : ServoChannelMap.leftGunTrigger, (ushort)(2000 * 4));
        }

        private void GunOff(bool rightGun)
        {
            pololuMaestroConnector.TrySetTarget(rightGun ? ServoChannelMap.rightGunTrigger : ServoChannelMap.leftGunTrigger, (ushort)(1000 * 4));

            isShooting = false;

            shootButton.Content = "Shoot";
        }

        //ushort posIAmHit = 0;

        private void HeadlightOn(bool rightHeadlight)
        {
            pololuMaestroConnector.TrySetTarget(rightHeadlight ? ServoChannelMap.rightHeadlight : ServoChannelMap.leftHeadlight, (ushort)(2000 * 4));
        }

        private void HeadlightOff(bool rightHeadlight)
        {
            pololuMaestroConnector.TrySetTarget(rightHeadlight ? ServoChannelMap.rightHeadlight : ServoChannelMap.leftHeadlight, (ushort)(1000 * 4));
        }

        private void headlightsBoth(bool on)
        {
            if (on)
            {
                HeadlightOn(true);
                HeadlightOn(false);
            }
            else
            {
                HeadlightOff(true);
                HeadlightOff(false);
            }
        }

        #endregion // Servo and gun controls

        #region Kinect platform pan related

        private double measuredKinectPanDegrees;

        private int index = 1;

        private void dispatcherTimer_Tick(object sender, EventArgs e)
        {
            DateTime tickTime = DateTime.Now;
            TimeSpan samplingTimeSpan = tickTime - lastReadServos;

            // take a measurement where the head actually is via feedback potentiometer. We spend around 1ms here:
            int measuredValue = pololuMaestroConnector.TryGetTarget(ServoChannelMap.headPanFeedback);   // 153 right, 400 center, 644 left. Shows divided by 4 in Mini Maestro UI

            double measuredValueDegrees = _panTiltAlignment.degreesPanKinect(measuredValue);

            this.measuredKinectPanDegrees = measuredValueDegrees;

            double measuredValueMks = MapMeasuredPos(measuredValueDegrees);

            double errorMks = panKinectSetpointMks - measuredValueMks;

            // compute servo input:
            double servoInputMks = ComputeServoInput(panKinectSetpointMks, measuredValueMks, (ulong)(tickTime.Ticks / TimeSpan.TicksPerMillisecond));

            // apply servo input to Kinect platform pan actuator. We spend around 1ms here:
            if (UsePIDCheckBox.IsChecked.GetValueOrDefault())
            {
                setPanKinectMks(servoInputMks);
            }
            else
            {
                setPanKinectMks(panKinectSetpointMks);
            }

            lastReadServos = tickTime;
        }

        private PIDControllerA pidA = null;

        private void InitPid()
        {
            pidA = new PIDControllerA(0, 0, 0, 0, 0, 0, PidDirection.DIRECT, (ulong)samplingIntervalMilliSeconds); // PIDControllerA.GetMillis());
            //pidA.SetOutputLimits(850.0d, 2200.0d);
            pidA.SetSampleTime((int)samplingIntervalMilliSeconds);
        }

        private void InitPidControls()
        {
            textBoxKp.Text = "0.5";
            textBoxKi.Text = "0.1";
            textBoxKd.Text = "0.01";

            textBoxMin.Text = "-400";
            textBoxMax.Text = "400";
            //textBoxIntegralMax.Text = "36000";
        }

        private void CollectAndApplyPidControls()
        {
            try
            {
                double kp = double.Parse(textBoxKp.Text.Trim());
                double ki = double.Parse(textBoxKi.Text.Trim());
                double kd = double.Parse(textBoxKd.Text.Trim());

                double min = double.Parse(textBoxMin.Text.Trim());
                double max = double.Parse(textBoxMax.Text.Trim());
                //double integralMax = double.Parse(textBoxIntegralMax.Text.Trim());        // not used on pidA

                pidA.SetTunings(kp, ki, kd);
                pidA.SetOutputLimits(min, max);
            }
            catch (Exception ex)
            {
            }
        }

        /// <summary>
        /// computes PID output
        /// </summary>
        /// <param name="setpointMks"></param>
        /// <param name="measuredValueMks"></param>
        /// <param name="millis">current time value</param>
        /// <returns></returns>
        private double ComputeServoInput(double setpointMks, double measuredValueMks, ulong millis)
        {
            double servoInputMks; // = setpointMks;

            pidA.mySetpoint = setpointMks;
            pidA.myInput = measuredValueMks;
            pidA.Compute(millis);

            servoInputMks = measuredValueMks + pidA.myOutput;
            //servoInputMks = pidA.myOutput;

            return servoInputMks;
        }

        private Ema measuredValueEma = new Ema(5);

        private double MapMeasuredPos(double measuredValueDegrees)
        {
            double measuredValueMks = _panTiltAlignment.mksPanKinect(measuredValueDegrees);

            // compute exponential moving average to smooth the measured data:
            double measuredValueEmaMks = measuredValueEma.Compute(measuredValueMks);

            return measuredValueEmaMks;
        }

        private void ApplyPidButton_Click(object sender, RoutedEventArgs e)
        {
            CollectAndApplyPidControls();
        }

        #endregion // Kinect platform pan related

        #region Kinect related

        /// <summary>
        /// Active Kinect sensor
        /// </summary>
        private kinect.KinectSensor sensor;

        private int ElevationAngle;

        private int targetElevationAngle = int.MinValue;
        private bool isElevationTaskOutstanding;

        /// <summary>
        /// Pen used for drawing bones that are currently tracked
        /// </summary>
        private readonly Pen trackedBonePen = new Pen(Brushes.Green, 6);

        /// <summary>
        /// Brush used for drawing joints that are currently tracked
        /// </summary>
        private readonly Brush trackedJointBrush = new SolidColorBrush(Color.FromArgb(255, 68, 192, 68));

        /// <summary>
        /// Pen used for drawing bones that are currently inferred
        /// </summary>        
        private readonly Pen inferredBonePen = new Pen(Brushes.Gray, 1);

        /// <summary>
        /// Brush used for drawing joints that are currently inferred
        /// </summary>        
        private readonly Brush inferredJointBrush = Brushes.Yellow;

        /// <summary>
        /// Brush used to draw skeleton center point
        /// </summary>
        private readonly Brush centerPointBrush = Brushes.Blue;

        /// <summary>
        /// Thickness of drawn joint lines
        /// </summary>
        private const double JointThickness = 3;

        /// <summary>
        /// Width of output drawing
        /// </summary>
        private const float RenderWidth = 640.0f;

        /// <summary>
        /// Height of our output drawing
        /// </summary>
        private const float RenderHeight = 480.0f;

        /// <summary>
        /// Thickness of body center ellipse
        /// </summary>
        private const double BodyCenterThickness = 10;

        /// <summary>
        /// Thickness of clip edge rectangles
        /// </summary>
        private const double ClipBoundsThickness = 10;

        /// <summary>
        /// Drawing group for skeleton rendering output
        /// </summary>
        private DrawingGroup drawingGroup;

        /// <summary>
        /// Drawing image that we will display
        /// </summary>
        private DrawingImage imageSource;

        int totalFrames = 0;
        int lastFrames = 0;
        DateTime lastTime = DateTime.MaxValue;

        // We want to control how depth data gets converted into false-color data
        // for more intuitive visualization, so we keep 32-bit color frame buffer versions of
        // these, to be updated whenever we receive and process a 16-bit frame.
        const int RED_IDX = 2;
        const int GREEN_IDX = 1;
        const int BLUE_IDX = 0;
        byte[] depthFrame32 = new byte[320 * 240 * 4];


        Dictionary<kinect.JointType, Brush> jointColors = new Dictionary<kinect.JointType, Brush>() { 
            {kinect.JointType.HipCenter, new SolidColorBrush(Color.FromRgb(169, 176, 155))},
            {kinect.JointType.Spine, new SolidColorBrush(Color.FromRgb(169, 176, 155))},
            {kinect.JointType.ShoulderCenter, new SolidColorBrush(Color.FromRgb(168, 230, 29))},
            {kinect.JointType.Head, new SolidColorBrush(Color.FromRgb(200, 0,   0))},
            {kinect.JointType.ShoulderLeft, new SolidColorBrush(Color.FromRgb(79,  84,  33))},
            {kinect.JointType.ElbowLeft, new SolidColorBrush(Color.FromRgb(84,  33,  42))},
            {kinect.JointType.WristLeft, new SolidColorBrush(Color.FromRgb(255, 126, 0))},
            {kinect.JointType.HandLeft, new SolidColorBrush(Color.FromRgb(215,  86, 0))},
            {kinect.JointType.ShoulderRight, new SolidColorBrush(Color.FromRgb(33,  79,  84))},
            {kinect.JointType.ElbowRight, new SolidColorBrush(Color.FromRgb(33,  33,  84))},
            {kinect.JointType.WristRight, new SolidColorBrush(Color.FromRgb(77,  109, 243))},
            {kinect.JointType.HandRight, new SolidColorBrush(Color.FromRgb(37,   69, 243))},
            {kinect.JointType.HipLeft, new SolidColorBrush(Color.FromRgb(77,  109, 243))},
            {kinect.JointType.KneeLeft, new SolidColorBrush(Color.FromRgb(69,  33,  84))},
            {kinect.JointType.AnkleLeft, new SolidColorBrush(Color.FromRgb(229, 170, 122))},
            {kinect.JointType.FootLeft, new SolidColorBrush(Color.FromRgb(255, 126, 0))},
            {kinect.JointType.HipRight, new SolidColorBrush(Color.FromRgb(181, 165, 213))},
            {kinect.JointType.KneeRight, new SolidColorBrush(Color.FromRgb(71, 222,  76))},
            {kinect.JointType.AnkleRight, new SolidColorBrush(Color.FromRgb(245, 228, 156))},
            {kinect.JointType.FootRight, new SolidColorBrush(Color.FromRgb(77,  109, 243))}
        };


        void InitKinect()
        {
            // parameters used to smooth the skeleton data
            kinect.TransformSmoothParameters parameters = new kinect.TransformSmoothParameters();
            parameters.Smoothing = 0.3f;
            parameters.Correction = 0.3f;
            parameters.Prediction = 0.4f;
            parameters.JitterRadius = 0.05f;
            parameters.MaxDeviationRadius = 0.05f;

            // =============================================================
            // create Kinect device:

            // Look through all sensors and start the first connected one.
            // This requires that a Kinect is connected at the time of app startup.
            // To make your app robust against plug/unplug, 
            // it is recommended to use KinectSensorChooser provided in Microsoft.Kinect.Toolkit
            foreach (var potentialSensor in kinect.KinectSensor.KinectSensors)
            {
                if (potentialSensor.Status == kinect.KinectStatus.Connected)
                {
                    this.sensor = potentialSensor;
                    break;
                }
            }

            if (null != this.sensor)
            {
                // Create the drawing group we'll use for drawing
                this.drawingGroup = new DrawingGroup();

                // Create an image source that we can use in our image control
                this.imageSource = new DrawingImage(this.drawingGroup);

                // Display the drawing using our image control
                skeletonImage.Source = this.imageSource;

                // Turn on the skeleton stream to receive skeleton frames
                this.sensor.SkeletonStream.Enable(parameters);
                this.sensor.DepthStream.Enable();
                this.sensor.ColorStream.Enable();

                // Add an event handler to be called whenever there is new color frame data
                this.sensor.SkeletonFrameReady += this.SensorSkeletonFrameReady;
                this.sensor.DepthFrameReady += this.SensorDepthFrameReady;
                this.sensor.ColorFrameReady += this.SensorColorFrameReady;

                // Allocate space to put the color pixels we'll receive
                this.colorFramePixels = new byte[this.sensor.ColorStream.FramePixelDataLength];

                // Allocate space to put the depth pixels we'll receive
                this.depthPixels = new short[this.sensor.DepthStream.FramePixelDataLength];

                // Allocate space to put the color pixels we'll get as result of Depth pixels conversion. One depth pixel will amount to BGR - three color pixels plus one unused
                this.colorDepthPixels = new byte[this.sensor.DepthStream.FramePixelDataLength * 4];

                // This is the bitmap we'll display on-screen. To work with  bitmap extensions(http://writeablebitmapex.codeplex.com/) must be PixelFormats.Pbgra32
                this.colorBitmapVideo = new WriteableBitmap(this.sensor.ColorStream.FrameWidth, this.sensor.ColorStream.FrameHeight, 96.0, 96.0, PixelFormats.Pbgra32, null);
                this.colorBitmapDepth = new WriteableBitmap(this.sensor.DepthStream.FrameWidth, this.sensor.DepthStream.FrameHeight, 96.0, 96.0, PixelFormats.Bgr32, null);

                // Set the image we display to point to the bitmap where we'll put the image data
                videoImage.Source = this.colorBitmapVideo;
                depthImage.Source = this.colorBitmapDepth;

                // Start the sensor!
                try
                {
                    this.sensor.Start();
                }
                catch (IOException)
                {
                    this.sensor = null;
                }
            }

            if (null == this.sensor)
            {
                System.Windows.MessageBox.Show("Runtime initialization failed. Please make sure Kinect device is plugged in.");
            }

            //nui.VideoStream.Open(ImageStreamType.Video, 2, ImageResolution.Resolution640x480, ImageType.Color);
            //nui.DepthStream.Open(ImageStreamType.Depth, 2, ImageResolution.Resolution320x240, ImageType.DepthAndPlayerIndex);

            lastTime = DateTime.Now;

            isElevationTaskOutstanding = false;
            ElevationAngle = 0;

            EnsureElevationAngle();
        }

        /// <summary>
        /// borrowed from C:\Projects\Kinect\Developer Toolkit v1.7.0\Samples\C#\KinectWpfViewers\KinectSensorManager.cs
        /// </summary>
        private void EnsureElevationAngle()
        {
            // We cannot set the angle on a sensor if it is not running.
            // We will therefore call EnsureElevationAngle when the requested angle has changed or if the
            // sensor transitions to the Running state.
            if ((null == sensor) || (kinect.KinectStatus.Connected != sensor.Status) || !sensor.IsRunning)
            {
                return;
            }

            this.targetElevationAngle = this.ElevationAngle;

            // If there already a background task, it will notice the new targetElevationAngle
            if (!this.isElevationTaskOutstanding)
            {
                // Otherwise, we need to start a new task
                this.StartElevationTask();
            }
        }

        private void StartElevationTask()
        {
            int lastSetElevationAngle = int.MinValue;

            if (null != sensor)
            {
                this.isElevationTaskOutstanding = true;

                Task.Factory.StartNew(
                    () =>
                    {
                        int angleToSet = this.targetElevationAngle;

                        // Keep going until we "match", assuming that the sensor is running
                        while ((lastSetElevationAngle != angleToSet) && sensor.IsRunning)
                        {
                            // We must wait at least 1 second, and call no more frequently than 15 times every 20 seconds
                            // So, we wait at least 1350ms afterwards before we set backgroundUpdateInProgress to false.
                            sensor.ElevationAngle = angleToSet;
                            lastSetElevationAngle = angleToSet;
                            Thread.Sleep(1350);

                            angleToSet = this.targetElevationAngle;
                        }
                    }).ContinueWith(
                            results =>
                            {
                                // This can happen if the Kinect transitions from Running to not running
                                // after the check above but before setting the ElevationAngle.
                                if (results.IsFaulted)
                                {
                                    var exception = results.Exception;

                                    Debug.WriteLine("Set Elevation Task failed with exception " + exception);
                                }

                                // We caught up and handled all outstanding angle requests.
                                // However, more may come in after we've stopped checking, so
                                // we post this work item back to the main thread to determine
                                // whether we need to start the task up again.
                                this.Dispatcher.BeginInvoke((Action)(() =>
                                {
                                    if (this.targetElevationAngle !=
                                        lastSetElevationAngle)
                                    {
                                        this.StartElevationTask();
                                    }
                                    else
                                    {
                                        // If there's nothing to do, we can set this to false.
                                        this.isElevationTaskOutstanding = false;
                                    }
                                }));
                            });
            }
        }

        // Converts a 16-bit grayscale depth frame which includes player indexes into a 32-bit frame
        // that displays different players in different colors
        byte[] convertDepthFrame(byte[] depthFrame16, int width, int height, int bubbleOuterRadiusMm, int bubbleThicknessMm, out bool threatDetected)
        {
            // bubbleOuterRadiusMm makes sense between 1000 and 4200 mm, and bubbleThicknessMm is good at 200
            threatDetected = false;

            int windowWidth = width / 2;
            int windowHeight = height / 2;

            int i16 = 0, i32 = 0;

            for (int h = 0; h < height; h++)
            {
                for (int w = 0; w < width; w++)
                {
                    bool inWindow = false;

                    depthFrame32[i32 + RED_IDX] = 0;
                    depthFrame32[i32 + GREEN_IDX] = 0;
                    depthFrame32[i32 + BLUE_IDX] = 0;

                    inWindow = (w > (width - windowWidth) / 2 && w < (width + windowWidth) / 2 && h > (height - windowHeight) / 2 && h < (height + windowHeight) / 2);

                    if (w == width / 2 || h == height / 2)
                    {
                        depthFrame32[i32 + BLUE_IDX] = 255;
                    }

                    int player = depthFrame16[i16] & 0x07;
                    int realDepth = (depthFrame16[i16 + 1] << 5) | (depthFrame16[i16] >> 3);
                    // transform 13-bit depth information into an 8-bit intensity appropriate
                    // for display (we disregard information in most significant bit)
                    byte intensity = (byte)(255 - (255 * realDepth / 0x0fff));

                    if (realDepth > bubbleOuterRadiusMm - bubbleThicknessMm && realDepth < bubbleOuterRadiusMm)
                    {
                        depthFrame32[i32 + RED_IDX] = 255; // intensity;
                        if (inWindow)
                        {
                            threatDetected = true;
                        }
                    }
                    else if (inWindow)
                    {
                        //depthFrame32[i32 + (posIAmHit < 500 ? GREEN_IDX : RED_IDX)] = 64;
                        depthFrame32[i32 + GREEN_IDX] = 64;
                    }

                    i16 += 2;
                    i32 += 4;

                    if (i16 >= depthFrame16.Length || i32 >= depthFrame32.Length)
                    {
                        return depthFrame32;
                    }
                }
            }

            /*
            for (; i16 < depthFrame16.Length && i32 < depthFrame32.Length; i16 += 2, i32 += 4)
            {
                int player = depthFrame16[i16] & 0x07;
                int realDepth = (depthFrame16[i16 + 1] << 5) | (depthFrame16[i16] >> 3);
                // transform 13-bit depth information into an 8-bit intensity appropriate
                // for display (we disregard information in most significant bit)
                byte intensity = (byte)(255 - (255 * realDepth / 0x0fff));

                depthFrame32[i32 + RED_IDX] = 0;
                depthFrame32[i32 + GREEN_IDX] = 0;
                depthFrame32[i32 + BLUE_IDX] = 0;

                if (realDepth > 1000 && realDepth < 1300)
                {
                    depthFrame32[i32 + RED_IDX] = 255; // intensity;
                }

                / *
                // choose different display colors based on player
                switch (player)
                {
                    case 0:
                        depthFrame32[i32 + RED_IDX] = (byte)(intensity / 2);
                        depthFrame32[i32 + GREEN_IDX] = (byte)(intensity / 2);
                        depthFrame32[i32 + BLUE_IDX] = (byte)(intensity / 2);
                        break;
                    case 1:
                        depthFrame32[i32 + RED_IDX] = intensity;
                        break;
                    case 2:
                        depthFrame32[i32 + GREEN_IDX] = intensity;
                        break;
                    case 3:
                        depthFrame32[i32 + RED_IDX] = (byte)(intensity / 4);
                        depthFrame32[i32 + GREEN_IDX] = (byte)(intensity);
                        depthFrame32[i32 + BLUE_IDX] = (byte)(intensity);
                        break;
                    case 4:
                        depthFrame32[i32 + RED_IDX] = (byte)(intensity);
                        depthFrame32[i32 + GREEN_IDX] = (byte)(intensity);
                        depthFrame32[i32 + BLUE_IDX] = (byte)(intensity / 4);
                        break;
                    case 5:
                        depthFrame32[i32 + RED_IDX] = (byte)(intensity);
                        depthFrame32[i32 + GREEN_IDX] = (byte)(intensity / 4);
                        depthFrame32[i32 + BLUE_IDX] = (byte)(intensity);
                        break;
                    case 6:
                        depthFrame32[i32 + RED_IDX] = (byte)(intensity / 2);
                        depthFrame32[i32 + GREEN_IDX] = (byte)(intensity / 2);
                        depthFrame32[i32 + BLUE_IDX] = (byte)(intensity);
                        break;
                    case 7:
                        depthFrame32[i32 + RED_IDX] = (byte)(255 - intensity);
                        depthFrame32[i32 + GREEN_IDX] = (byte)(255 - intensity);
                        depthFrame32[i32 + BLUE_IDX] = (byte)(255 - intensity);
                        break;
                }
                * /
            }
             */
            return depthFrame32;
        }


        bool threatDetected = false;
        DateTime lastThreatDetected = DateTime.MinValue;

        DateTime lastReadServos = DateTime.MinValue;
        ushort lastPosIAmHit = 0;

        /// <summary>
        /// Bitmap that will hold color information for video
        /// </summary>
        private WriteableBitmap colorBitmapVideo;

        /// <summary>
        /// Bitmap that will hold color information for depth
        /// </summary>
        private WriteableBitmap colorBitmapDepth;

        /// <summary>
        /// Intermediate storage for the depth data received from the camera
        /// </summary>
        private short[] depthPixels;

        /// <summary>
        /// Intermediate storage for the depth data converted to color
        /// </summary>
        private byte[] colorDepthPixels;

        /// <summary>
        /// Intermediate storage for the color frame data
        /// </summary>
        private byte[] colorFramePixels;

        void SensorColorFrameReady(object sender, kinect.ColorImageFrameReadyEventArgs e)
        {
            if (isShooting)
            {
                return;
            }

            using (kinect.ColorImageFrame colorFrame = e.OpenColorImageFrame())
            {
                if (colorFrame != null)
                {
                    // Copy the pixel data from the image to a temporary array
                    colorFrame.CopyPixelDataTo(this.colorFramePixels);

                    for (int i = 3; i < this.colorFramePixels.Length - 4; i += 4)
                    {
                        this.colorFramePixels[i] = 255;  // set the alpha to max
                    }

                    // Write the pixel data into our bitmap
                    this.colorBitmapVideo.WritePixels(
                        new Int32Rect(0, 0, this.colorBitmapVideo.PixelWidth, this.colorBitmapVideo.PixelHeight),
                        this.colorFramePixels,
                        this.colorBitmapVideo.PixelWidth * sizeof(int),
                        0);
                }
            }

            // draw a sighting frame for precise alignment:
            // Horisontally the frame spans 16.56 degrees on every side and 12.75 degrees either up or down (at 74" the size it covers is 44"W by 33.5"H, i.e. 33.12 degrees by 25.5 degrees)
            // see http://writeablebitmapex.codeplex.com/
            //int sightFrameColor = 255;  // Pbgra32
            Color sightFrameColor = Colors.Red;
            colorBitmapVideo.DrawLine((int)colorBitmapVideo.Width / 2, 0, (int)colorBitmapVideo.Width / 2, (int)colorBitmapVideo.Height, sightFrameColor);
            colorBitmapVideo.DrawLine(0, (int)colorBitmapVideo.Height / 2, (int)colorBitmapVideo.Width, (int)colorBitmapVideo.Height / 2, sightFrameColor);
            colorBitmapVideo.DrawRectangle((int)colorBitmapVideo.Width / 4, (int)colorBitmapVideo.Height / 4,
                                            (int)colorBitmapVideo.Width * 3 / 4, (int)colorBitmapVideo.Height * 3 / 4, sightFrameColor);
        }

        void SensorDepthFrameReady(object sender, kinect.DepthImageFrameReadyEventArgs e)
        {
            if (isShooting)
            {
                return;
            }

            //if ((DateTime.Now - lastReadServos).TotalMilliseconds > 50.0d)
            //{
            //    pololuMaestroConnector.TryGetTarget(ServoChannelMap.iAmHitInput);
            //    lastReadServos = DateTime.Now;
            //    this.Dispatcher.Invoke(new UpdateLabelDelegate(updatePmValuesLabel), string.Format("I Am Hit: {0:0.000}", posIAmHit));
            //    if (posIAmHit > 500 && lastPosIAmHit < 500)
            //    {
            //        shootBoth();
            //    }
            //    lastPosIAmHit = posIAmHit;
            //}

            using (kinect.DepthImageFrame depthFrame = e.OpenDepthImageFrame())
            {
                if (depthFrame != null)
                {
                    // Copy the pixel data from the image to a temporary array
                    depthFrame.CopyPixelDataTo(this.depthPixels);

                    // Convert the depth to RGB
                    int colorPixelIndex = 0;
                    for (int i = 0; i < this.depthPixels.Length; ++i)
                    {
                        // discard the portion of the depth that contains only the player index
                        short depth = (short)(this.depthPixels[i] >> kinect.DepthImageFrame.PlayerIndexBitmaskWidth);

                        // to convert to a byte we're looking at only the lower 8 bits
                        // by discarding the most significant rather than least significant data
                        // we're preserving detail, although the intensity will "wrap"
                        // add 1 so that too far/unknown is mapped to black
                        byte intensity = (byte)((depth + 1) & byte.MaxValue);

                        // Write out blue byte
                        this.colorDepthPixels[colorPixelIndex++] = intensity;

                        // Write out green byte
                        this.colorDepthPixels[colorPixelIndex++] = intensity;

                        // Write out red byte                        
                        this.colorDepthPixels[colorPixelIndex++] = intensity;

                        // We're outputting BGR, the last byte in the 32 bits is unused so skip it
                        // If we were outputting BGRA, we would write alpha here.
                        ++colorPixelIndex;
                    }

                    // Write the pixel data into our bitmap
                    this.colorBitmapDepth.WritePixels(
                        new Int32Rect(0, 0, this.colorBitmapDepth.PixelWidth, this.colorBitmapDepth.PixelHeight),
                        this.colorDepthPixels,
                        this.colorBitmapDepth.PixelWidth * sizeof(int),
                        0);
                }
            }

            /*
            //PlanarImage Image = e.ImageFrame.Image;
            threatDetected = false;

            int bubbleOuterRadiusMm = (int)bubbleScrollBar.Value;
            int bubbleThicknessMm = (int)bubbleThicknessScrollBar.Value;

            byte[] convertedDepthFrame = convertDepthFrame(Image.Bits, Image.Width, Image.Height, bubbleOuterRadiusMm, bubbleThicknessMm, out threatDetected);

            depthImage.Source = BitmapSource.Create(
                Image.Width, Image.Height, 96, 96, PixelFormats.Bgr32, null, convertedDepthFrame, Image.Width * 4);
            */

            ++totalFrames;

            DateTime cur = DateTime.Now;
            if (cur.Subtract(lastTime) > TimeSpan.FromSeconds(1))
            {
                int frameDiff = totalFrames - lastFrames;
                lastFrames = totalFrames;
                lastTime = cur;
                frameRate.Text = frameDiff.ToString() + " fps";
            }

            if (isTracking && threatDetected && (DateTime.Now - lastThreatDetected).TotalSeconds > 1.0d)
            {
                lastThreatDetected = DateTime.Now;
                if ((bool)shootOnBubbleCheckBox.IsChecked)
                {
                    shootBoth();
                }
                speak("back off!");
            }
        }

        bool handsUp = false;
        DateTime lastHandsUp = DateTime.MinValue;

        bool isTracking = false;
        DateTime lastTracking = DateTime.MinValue;
        DateTime lastNotTracking = DateTime.Now;

        TrajectoryPredictor predictor = new TrajectoryPredictor();

        /// <summary>
        /// Event handler for Kinect sensor's SkeletonFrameReady event
        /// </summary>
        /// <param name="sender">object sending the event</param>
        /// <param name="e">event arguments</param>
        private void SensorSkeletonFrameReady(object sender, kinect.SkeletonFrameReadyEventArgs e)
        {
            if (isShooting)
            {
                return;
            }

            kinect.Skeleton[] skeletons = new kinect.Skeleton[0];

            using (kinect.SkeletonFrame skeletonFrame = e.OpenSkeletonFrame())
            {
                if (skeletonFrame != null)
                {
                    skeletons = new kinect.Skeleton[skeletonFrame.SkeletonArrayLength];
                    skeletonFrame.CopySkeletonDataTo(skeletons);
                }
            }

            processSkeletons(skeletons);
        }

        bool inServoControl = false;

        string currentAnimationName = string.Empty;
        bool currentAnimationRepeating = false;
        DateTime currentAnimationTime = DateTime.Now;

        private void processSkeletons(kinect.Skeleton[] skeletons)
        {
            bool gameOn = gameOnCheckBox.IsChecked.GetValueOrDefault();
            int iSkeleton = 0;
            int iSkeletonsTracked = 0;

            double panAngle = double.NaN;       // degrees from forward; right positive
            double tiltAngle = double.NaN;
            double targetX = double.NaN;        // meters
            double targetY = double.NaN;
            double targetZ = double.NaN;

            // find skeleton with visible head, closest to the camera:
            kinect.Skeleton trackedSkeleton = (from s in skeletons
                                               where s.TrackingState == kinect.SkeletonTrackingState.Tracked && s.Joints[kinect.JointType.Head].TrackingState == kinect.JointTrackingState.Tracked
                                               orderby s.Joints[kinect.JointType.Head].Position.Z
                                               select s
                                               ).FirstOrDefault();

            using (DrawingContext dc = this.drawingGroup.Open())
            {
                // Draw a transparent background to set the render size
                dc.DrawRectangle(Brushes.Black, null, new Rect(0.0, 0.0, RenderWidth, RenderHeight));

                if (skeletons.Length != 0)
                {
                    foreach (kinect.Skeleton skel in skeletons)
                    {
                        if (kinect.SkeletonTrackingState.Tracked == skel.TrackingState)
                        {
                            RenderClippedEdges(skel, dc);

                            this.DrawBonesAndJoints(skel, dc);

                            // http://social.msdn.microsoft.com/Forums/en-AU/kinectsdk/thread/d821df8d-39ca-44e3-81e7-c907d94acfca  - data.UserIndex is always 254

                            //bool targetedUser = skel.UserIndex == 1;
                            bool targetedUser = skel == trackedSkeleton;

                            if (targetedUser)
                            {
                                kinect.Joint targetJoint = skel.Joints[kinect.JointType.WristLeft];
                                bool useTargetJoint = false;

                                switch (trackWhatComboBox.SelectedValue.ToString())
                                {
                                    case "HandLeft":
                                        //targetJoint = skel.Joints[kinect.JointType.WristLeft];
                                        useTargetJoint = true;
                                        break;

                                    case "HandRight":
                                        targetJoint = skel.Joints[kinect.JointType.WristRight];
                                        useTargetJoint = true;
                                        break;

                                    case "FootLeft":
                                        targetJoint = skel.Joints[kinect.JointType.FootLeft];
                                        useTargetJoint = true;
                                        break;

                                    case "FootRight":
                                        targetJoint = skel.Joints[kinect.JointType.FootRight];
                                        useTargetJoint = true;
                                        break;

                                    default:    // GC - use skel.Position.*
                                        targetX = skel.Position.X;   // meters
                                        targetY = skel.Position.Y;
                                        targetZ = skel.Position.Z;
                                        break;
                                }

                                if (!useTargetJoint || useTargetJoint && targetJoint.TrackingState == kinect.JointTrackingState.Tracked)
                                {
                                    if (useTargetJoint)
                                    {
                                        targetX = targetJoint.Position.X;
                                        targetY = targetJoint.Position.Y;
                                        targetZ = targetJoint.Position.Z;
                                    }

                                    // can set Pan or Tilt to NaN:
                                    panAngle = -Math.Atan2(targetX, targetZ) * 180.0d / Math.PI;
                                    tiltAngle = Math.Atan2(targetY, targetZ) * 180.0d / Math.PI;
                                }

                                if (gameOn && singCycle == -1)
                                {
                                    bool huCurr = skel.Joints[kinect.JointType.Head].TrackingState == kinect.JointTrackingState.Tracked
                                                && skel.Joints[kinect.JointType.WristLeft].TrackingState == kinect.JointTrackingState.Tracked
                                                && skel.Joints[kinect.JointType.WristRight].TrackingState == kinect.JointTrackingState.Tracked
                                                && skel.Joints[kinect.JointType.WristLeft].Position.Y > skel.Joints[kinect.JointType.Head].Position.Y
                                                && skel.Joints[kinect.JointType.WristRight].Position.Y > skel.Joints[kinect.JointType.Head].Position.Y;

                                    if (huCurr != handsUp)
                                    {
                                        if (!handsUp && (DateTime.Now - lastHandsUp).TotalSeconds > 1.0d)
                                        {
                                            speak();
                                            lastHandsUp = DateTime.Now;
                                            shootBoth();
                                        }
                                        handsUp = huCurr;
                                    }

                                    bool leftHandOut = skel.Joints[kinect.JointType.Spine].TrackingState == kinect.JointTrackingState.Tracked
                                                && skel.Joints[kinect.JointType.WristLeft].TrackingState == kinect.JointTrackingState.Tracked
                                                && skel.Joints[kinect.JointType.WristLeft].Position.X < skel.Joints[kinect.JointType.Spine].Position.X - 0.6f;   // meters

                                    bool leftHandForward = skel.Joints[kinect.JointType.Spine].TrackingState == kinect.JointTrackingState.Tracked
                                                && skel.Joints[kinect.JointType.WristLeft].TrackingState == kinect.JointTrackingState.Tracked
                                                && skel.Joints[kinect.JointType.WristLeft].Position.Z < skel.Joints[kinect.JointType.Spine].Position.Z - 0.4f;   // meters

                                    if (leftHandOut)
                                    //if (leftHandForward)
                                    {
                                        HeadlightOn(true);
                                    }
                                    else
                                    {
                                        HeadlightOff(true);
                                    }

                                    bool rightHandOut = skel.Joints[kinect.JointType.Spine].TrackingState == kinect.JointTrackingState.Tracked
                                                && skel.Joints[kinect.JointType.WristRight].TrackingState == kinect.JointTrackingState.Tracked
                                                && skel.Joints[kinect.JointType.WristRight].Position.X > skel.Joints[kinect.JointType.Spine].Position.X + 0.6f;   // meters

                                    bool rightHandForward = skel.Joints[kinect.JointType.Spine].TrackingState == kinect.JointTrackingState.Tracked
                                                && skel.Joints[kinect.JointType.WristRight].TrackingState == kinect.JointTrackingState.Tracked
                                                && skel.Joints[kinect.JointType.WristRight].Position.Z < skel.Joints[kinect.JointType.Spine].Position.Z - 0.4f;   // meters

                                    if (rightHandOut)
                                    //if (rightHandForward)
                                    {
                                        HeadlightOn(false);
                                    }
                                    else
                                    {
                                        HeadlightOff(false);
                                    }

                                    if (rightHandForward)
                                    {
                                        setAnimationCombo("Mad");
                                    }
                                    else if (leftHandForward)
                                    {
                                        setAnimationCombo("Mad2");
                                    }
                                    else
                                    {
                                        if (panAngle > 10)
                                        {
                                            setAnimationCombo("Alert_lookright", true);
                                        }
                                        else if (panAngle < -10)
                                        {
                                            setAnimationCombo("Alert_lookleft", true);
                                        }
                                        else
                                        {
                                            setAnimationCombo("Alert", true);
                                        }
                                    }
                                }
                            }

                            iSkeletonsTracked++;
                        }
                        else if (skel.TrackingState == kinect.SkeletonTrackingState.PositionOnly)
                        {
                            dc.DrawEllipse(
                            this.centerPointBrush,
                            null,
                            this.SkeletonPointToScreen(skel.Position),
                            BodyCenterThickness,
                            BodyCenterThickness);
                        }

                        iSkeleton++;
                    } // for each skeleton
                }

                // prevent drawing outside of our render area
                this.drawingGroup.ClipGeometry = new RectangleGeometry(new Rect(0.0, 0.0, RenderWidth, RenderHeight));
            }

            if (iSkeletonsTracked > 0)
            {
                if (!inServoControl)
                {
                    inServoControl = true;

                    if (!double.IsNaN(panAngle) && !double.IsNaN(tiltAngle))
                    {
                        TrajectoryPoint currentPoint = new TrajectoryPoint()
                        {
                            X = targetX,
                            Y = targetY,
                            Z = targetZ,
                            panAngle = panAngle,
                            tiltAngle = tiltAngle
                        };

                        double arrowSpeedMSec = 25.0d;
                        double timeToTargetS = targetZ / arrowSpeedMSec; // +0.01d;   // +0.1d;     // full shot cycle takes 0.2 sec; arrow will leave the barrel at about 0.1s. High value will lead to a lot of jittering.

                        TrajectoryPoint futurePoint = predictor.predict(currentPoint, new TimeSpan((long)(timeToTargetS * TimeSpan.TicksPerSecond)));

                        //double deltaX = futurePoint.X - currentPoint.X;
                        //double deltaY = futurePoint.Y - currentPoint.Y;

                        if (enableKinectCheckBox.IsChecked.HasValue && enableKinectCheckBox.IsChecked.Value)
                        {
                            #region Kinect platform pan related

                            //this.Dispatcher.Invoke(new UpdateLabelDelegate(updatePmValuesLabel), string.Format("X: {0:0.000}\r\nY: {1:0.000}\r\nZ: {2:0.000}\r\nPan: {3:0}\r\nTilt: {4:0}", targetX, targetY, targetZ, panAngle, tiltAngle));

                            // see C:\Projects\Robotics\src\TrackRoamer\TrackRoamerBehaviors\Strategy\StrategyPersonFollowing.cs : 233

                            double targetPanRelativeToHead = futurePoint.panAngle;
                            double targetPanRelativeToRobot = measuredKinectPanDegrees + targetPanRelativeToHead;

                            //Tracer.Trace("==================  currentPanKinect=" + _state.currentPanKinect + "   targetJoint.Pan=" + targetJoint.Pan + "   targetPanRelativeToRobot=" + targetPanRelativeToRobot);

                            // guns rotate (pan) with Kinect, but tilt independently of Kinect. They are calibrated when Kinect tilt = 0
                            //targetPan = targetPanRelativeToHead;
                            //targetTilt = futurePoint.tiltAngle + kinectTiltActualDegrees;

                            double kinectTurnEstimate = targetPanRelativeToRobot - measuredKinectPanDegrees;

                            double smallMovementsAngleTreshold = 10.0d;
                            bool shouldTurnKinect = Math.Abs(kinectTurnEstimate) > smallMovementsAngleTreshold;         // don't follow small movements
                            //kinectPanDesiredDegrees = shouldTurnKinect ? (double?)targetPanRelativeToRobot : null;    // will be processed in computeHeadTurn() when head turn measurement comes.

                            if (shouldTurnKinect)
                            {
                                panKinectSetpointMks = _panTiltAlignment.mksPanKinect(targetPanRelativeToRobot);
                            }

                            #endregion // Kinect platform pan related

                            // Kinect platform pan will be turned by PID, leave the value as null here:
                            pololuMaestroConnector.setPanTilt(futurePoint.panAngle, futurePoint.tiltAngle, futurePoint.panAngle, futurePoint.tiltAngle, null);
                        }
                    }

                    inServoControl = false;
                }

                if (!isTracking && (DateTime.Now - lastTracking).TotalSeconds > 2.0d && gameOnCheckBox.IsChecked.GetValueOrDefault())
                {
                    if (gameTalkCheckBox.IsChecked.GetValueOrDefault())
                    {
                        speak("hello beautiful!");
                    }
                    lastTracking = DateTime.Now;
                    isTracking = true;
                }
            }
            else
            {
                if ((DateTime.Now - lastNotTracking).TotalSeconds > 5.0d && gameOnCheckBox.IsChecked.GetValueOrDefault())
                {
                    if (gameTalkCheckBox.IsChecked.GetValueOrDefault())
                    {
                        speak("come out, come out, wherever you are!");
                    }
                    lastNotTracking = DateTime.Now;

                    if (enableKinectCheckBox.IsChecked.HasValue && enableKinectCheckBox.IsChecked.Value && singCycle == -1)
                    {
                        SafePosture();
                    }
                }
                isTracking = false;
                panKinectSetpointMks = 1500.0d;
            }
        }

        #region Draw Skeletons

        /// <summary>
        /// Draws indicators to show which edges are clipping skeleton data
        /// </summary>
        /// <param name="skeleton">skeleton to draw clipping information for</param>
        /// <param name="drawingContext">drawing context to draw to</param>
        private static void RenderClippedEdges(kinect.Skeleton skeleton, DrawingContext drawingContext)
        {
            if (skeleton.ClippedEdges.HasFlag(kinect.FrameEdges.Bottom))
            {
                drawingContext.DrawRectangle(
                    Brushes.Red,
                    null,
                    new Rect(0, RenderHeight - ClipBoundsThickness, RenderWidth, ClipBoundsThickness));
            }

            if (skeleton.ClippedEdges.HasFlag(kinect.FrameEdges.Top))
            {
                drawingContext.DrawRectangle(
                    Brushes.Red,
                    null,
                    new Rect(0, 0, RenderWidth, ClipBoundsThickness));
            }

            if (skeleton.ClippedEdges.HasFlag(kinect.FrameEdges.Left))
            {
                drawingContext.DrawRectangle(
                    Brushes.Red,
                    null,
                    new Rect(0, 0, ClipBoundsThickness, RenderHeight));
            }

            if (skeleton.ClippedEdges.HasFlag(kinect.FrameEdges.Right))
            {
                drawingContext.DrawRectangle(
                    Brushes.Red,
                    null,
                    new Rect(RenderWidth - ClipBoundsThickness, 0, ClipBoundsThickness, RenderHeight));
            }
        }

        /// <summary>
        /// Draws a skeleton's bones and joints
        /// </summary>
        /// <param name="skeleton">skeleton to draw</param>
        /// <param name="drawingContext">drawing context to draw to</param>
        private void DrawBonesAndJoints(kinect.Skeleton skeleton, DrawingContext drawingContext)
        {
            // Render Torso
            this.DrawBone(skeleton, drawingContext, kinect.JointType.Head, kinect.JointType.ShoulderCenter);
            this.DrawBone(skeleton, drawingContext, kinect.JointType.ShoulderCenter, kinect.JointType.ShoulderLeft);
            this.DrawBone(skeleton, drawingContext, kinect.JointType.ShoulderCenter, kinect.JointType.ShoulderRight);
            this.DrawBone(skeleton, drawingContext, kinect.JointType.ShoulderCenter, kinect.JointType.Spine);
            this.DrawBone(skeleton, drawingContext, kinect.JointType.Spine, kinect.JointType.HipCenter);
            this.DrawBone(skeleton, drawingContext, kinect.JointType.HipCenter, kinect.JointType.HipLeft);
            this.DrawBone(skeleton, drawingContext, kinect.JointType.HipCenter, kinect.JointType.HipRight);

            // Left Arm
            this.DrawBone(skeleton, drawingContext, kinect.JointType.ShoulderLeft, kinect.JointType.ElbowLeft);
            this.DrawBone(skeleton, drawingContext, kinect.JointType.ElbowLeft, kinect.JointType.WristLeft);
            this.DrawBone(skeleton, drawingContext, kinect.JointType.WristLeft, kinect.JointType.HandLeft);

            // Right Arm
            this.DrawBone(skeleton, drawingContext, kinect.JointType.ShoulderRight, kinect.JointType.ElbowRight);
            this.DrawBone(skeleton, drawingContext, kinect.JointType.ElbowRight, kinect.JointType.WristRight);
            this.DrawBone(skeleton, drawingContext, kinect.JointType.WristRight, kinect.JointType.HandRight);

            // Left Leg
            this.DrawBone(skeleton, drawingContext, kinect.JointType.HipLeft, kinect.JointType.KneeLeft);
            this.DrawBone(skeleton, drawingContext, kinect.JointType.KneeLeft, kinect.JointType.AnkleLeft);
            this.DrawBone(skeleton, drawingContext, kinect.JointType.AnkleLeft, kinect.JointType.FootLeft);

            // Right Leg
            this.DrawBone(skeleton, drawingContext, kinect.JointType.HipRight, kinect.JointType.KneeRight);
            this.DrawBone(skeleton, drawingContext, kinect.JointType.KneeRight, kinect.JointType.AnkleRight);
            this.DrawBone(skeleton, drawingContext, kinect.JointType.AnkleRight, kinect.JointType.FootRight);

            // Render Joints
            foreach (kinect.Joint joint in skeleton.Joints)
            {
                Brush drawBrush = null;

                if (joint.TrackingState == kinect.JointTrackingState.Tracked)
                {
                    drawBrush = this.trackedJointBrush;
                }
                else if (joint.TrackingState == kinect.JointTrackingState.Inferred)
                {
                    drawBrush = this.inferredJointBrush;
                }

                if (drawBrush != null)
                {
                    drawingContext.DrawEllipse(drawBrush, null, this.SkeletonPointToScreen(joint.Position), JointThickness, JointThickness);
                }
            }
        }

        /// <summary>
        /// Maps a SkeletonPoint to lie within our render space and converts to Point
        /// </summary>
        /// <param name="skelpoint">point to map</param>
        /// <returns>mapped point</returns>
        private Point SkeletonPointToScreen(kinect.SkeletonPoint skelpoint)
        {
            // Convert point to depth space.  
            // We are not using depth directly, but we do want the points in our 640x480 output resolution.
            kinect.DepthImagePoint depthPoint = this.sensor.MapSkeletonPointToDepth(
                                                                             skelpoint,
                                                                             kinect.DepthImageFormat.Resolution640x480Fps30);
            return new Point(depthPoint.X, depthPoint.Y);
        }

        /// <summary>
        /// Draws a bone line between two joints
        /// </summary>
        /// <param name="skeleton">skeleton to draw bones from</param>
        /// <param name="drawingContext">drawing context to draw to</param>
        /// <param name="jointType0">joint to start drawing from</param>
        /// <param name="jointType1">joint to end drawing at</param>
        private void DrawBone(kinect.Skeleton skeleton, DrawingContext drawingContext, kinect.JointType jointType0, kinect.JointType jointType1)
        {
            kinect.Joint joint0 = skeleton.Joints[jointType0];
            kinect.Joint joint1 = skeleton.Joints[jointType1];

            // If we can't find either of these joints, exit
            if (joint0.TrackingState == kinect.JointTrackingState.NotTracked ||
                joint1.TrackingState == kinect.JointTrackingState.NotTracked)
            {
                return;
            }

            // Don't draw if both points are inferred
            if (joint0.TrackingState == kinect.JointTrackingState.Inferred &&
                joint1.TrackingState == kinect.JointTrackingState.Inferred)
            {
                return;
            }

            // We assume all drawn bones are inferred unless BOTH joints are tracked
            Pen drawPen = this.inferredBonePen;
            if (joint0.TrackingState == kinect.JointTrackingState.Tracked && joint1.TrackingState == kinect.JointTrackingState.Tracked)
            {
                drawPen = this.trackedBonePen;
            }

            drawingContext.DrawLine(drawPen, this.SkeletonPointToScreen(joint0.Position), this.SkeletonPointToScreen(joint1.Position));
        }

        #endregion // Draw Skeletons

        #endregion // Kinect related

        #region Controls actions - button clicks etc

        private void pmSafePostureButton_Click(object sender, RoutedEventArgs e)
        {
            speak("safety activated");
            SafePosture();
        }

        private void shootButton_Click(object sender, RoutedEventArgs e)
        {
            shoot();
        }

        private void shoot()
        {
            bool rightGun = rightGunCheckBox.IsChecked.GetValueOrDefault();

            GunOn(rightGun);

            // 250ms plants a good single shot.

            dispatcherTimerGun.Interval = new TimeSpan((long)((rightGun ? _panTiltAlignment.timeGunOnMsGunRight : _panTiltAlignment.timeGunOnMsGunLeft) * TimeSpan.TicksPerMillisecond));
            dispatcherTimerGun.Start();
        }

        private bool isShooting = false;

        private void dispatcherTimerGun_Tick(object sender, EventArgs e)
        {
            bool rightGun = rightGunCheckBox.IsChecked.GetValueOrDefault();

            GunOff(rightGun);

            dispatcherTimerGun.Stop();
        }

        private void shootBoth()
        {
            GunOn(true);
            GunOn(false);

            // 250ms plants a good single shot. Take the max of left and right guns.

            dispatcherTimerGun2.Interval = new TimeSpan((long)(Math.Max(_panTiltAlignment.timeGunOnMsGunRight, _panTiltAlignment.timeGunOnMsGunLeft) * TimeSpan.TicksPerMillisecond));
            dispatcherTimerGun2.Start();
        }

        private void dispatcherTimerGun2_Tick(object sender, EventArgs e)
        {
            GunOff(true);
            GunOff(false);

            dispatcherTimerGun2.Stop();
        }

        private void trackWhatComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //panAlignLabel.Content = trackWhatComboBox.SelectedValue.ToString();
        }

        private void timeGunOnMsTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            double.TryParse(timeGunOnMsTextBox.Text, out _panTiltAlignment.timeGunOnMsGunLeft);
        }

        #endregion // Controls actions - button clicks etc

        #region Singing and Playing Music

        bool isPlayingMusic = false;
        AnimationSchedule sched = new AnimationSchedule();

        private void PlaySoundButton_Click(object sender, RoutedEventArgs e)
        {
            string filename = "" + PlaySoundComboBox.SelectedValue;
            if (isPlayingMusic)
            {
                //try
                //{
                    //stopAnimationSchedule();
                    dispatcherTimerAnimScheduler.Stop();
                    brickConnector.clearAnimations();
                    setAnimationCombo("Alert");
                    PlaySoundButton.Content = "Play Sound";
                    MediaPlayer.Stop();
                    isPlayingMusic = false;
                    singCycle = -1;     // -1 allows other animations
                //}
                //catch
                //{
                //}
            }
            else if(!string.IsNullOrWhiteSpace(filename))
            {
                isPlayingMusic = true;
                singCycle = 0;
                playSound(filename); //"Cow_Moo-Mike_Koenig-42670858.mp3");
                PlaySoundButton.Content = "Stop";
                brickConnector.clearAnimations();
                dispatcherTimerAnimScheduler.Interval = TimeSpan.FromSeconds(1.43d);     // 2.1d - MrLonely; 0.9 - EndofTheWorld; 1.43d - LoveStory
                dispatcherTimerAnimScheduler.Start();
                //startAnimationSchedule();
            }
        }

        public void startAnimationSchedule()
        {
            long tts = DateTime.Now.Ticks;

            Debug.WriteLine("-------------- started --------------------");

            foreach (AnimationCommand curr in sched.commands)
            {
                double curtime = (DateTime.Now.Ticks - tts) / TimeSpan.TicksPerSecond;
                int timeToSleep = (int)((curr.secToStart - curtime) * 1000.0d);
                if (timeToSleep > 0)
                {
                    Thread.Sleep(timeToSleep);
                }
                curtime = (DateTime.Now.Ticks - tts) / TimeSpan.TicksPerSecond;
                Debug.WriteLine("running: " + curr.cmd + "    at " + curtime);

                switch (curr.cmd)
                {
                    case "Play":
                        break;

                    case "Combo":
                        setAnimationCombo(curr.cmdParams);
                        break;

                    case "Default":
                        brickConnector.setDefaultAnimations();
                        break;

                    case "Clear":
                        brickConnector.clearAnimations();
                        break;
                }
            }
            Debug.WriteLine("finished");
        }

        public void stopAnimationSchedule()
        {
            Debug.WriteLine("-------------- stopped --------------------");
        }


        int singCycle = -1;     // -1 allows other animations

        private void dispatcherTimerAnimScheduler_Tick(object sender, EventArgs e)
        {
            switch (singCycle++)
            {
                case 0:
                    setAnimationCombo("Alert");
                    break;

                case 1:
                case 2:
                    setAnimationCombo("Alert_lookright");
                    break;

                case 3:
                case 4:
                    setAnimationCombo("Alert_lookleft");
                    break;

                case 5:
                case 6:
                    setAnimationCombo("Acknowledge");
                    break;

                case 7:
                case 8:
                    setAnimationCombo("Yell");
                    break;

                case 10:
                case 9:
                    //brickConnector.clearAnimations();
                    brickConnector.setDefaultAnimations();
                    dispatcherTimerAnimScheduler.Stop();
                    break;

                case 11:

                default:
                    dispatcherTimerAnimScheduler.Stop();
                    break;
                    //return;
            }
//            dispatcherTimer3.Interval = TimeSpan.FromSeconds(1.0d);
//          dispatcherTimer3.Start();
        }

        private string soundsBasePathDefault;          // if left null, GUI will not show any files in the dropdown
        private Random random = new Random();
        private List<string> SoundFiles;

        private void playSound(string filename, double volume = 0.5d, string soundsBasePath = null)
        {
            if (MediaPlayer.IsLoaded && (!string.IsNullOrEmpty(soundsBasePath) || !string.IsNullOrEmpty(soundsBasePathDefault)))
            {
                string filePath = System.IO.Path.Combine(string.IsNullOrEmpty(soundsBasePath) ? soundsBasePathDefault : soundsBasePath, filename);
                if (File.Exists(filePath))
                {
                    try
                    {
                        MediaPlayer.Source = new Uri("file://" + filePath);
                        MediaPlayer.Volume = volume;      // The media's volume represented on a linear scale between 0 and 1. The default is 0.5
                        MediaPlayer.Play();
                    }
                    catch
                    {
                    }
                }
            }
        }

        private void PopulatePlaySoundCombo()
        {
            if (!string.IsNullOrEmpty(soundsBasePathDefault))
            {
                DirectoryInfo di = new DirectoryInfo(soundsBasePathDefault);
                bool haveFiles = false;

                if (di.Exists)
                {
                    SoundFiles = (from f in di.GetFiles()
                                  where f.Name.EndsWith(".wav", StringComparison.InvariantCultureIgnoreCase) || f.Name.EndsWith(".mp3", StringComparison.InvariantCultureIgnoreCase)
                                  select f.Name).ToList();

                    if (SoundFiles != null && SoundFiles.Any())
                    {
                        PlaySoundComboBox.ItemsSource = SoundFiles;
                        haveFiles = true;
                    }
                }

                if (!haveFiles)
                {
                    PlaySoundButton.IsEnabled = false;
                    PlaySoundComboBox.IsEnabled = false;
                    soundsBasePathLabel.Content += "     (not found)";
                }
            }
        }

        #endregion // Singing and Playing Music
    }

    public class AnimationCommand
    {
        public double secToStart;
        public string cmd;
        public string cmdParams;
    }


    public class AnimationSchedule
    {
        public AnimationCommand[] commands = new AnimationCommand[]
        {
            new AnimationCommand() { secToStart = 0, cmd = "Play", cmdParams = "10 Track 10_LoveStory.mp3" },
            new AnimationCommand() { secToStart = 3.2, cmd = "Combo", cmdParams = "Alert" },
            new AnimationCommand() { secToStart = 6, cmd = "Combo", cmdParams = "Alert_lookleft" },
            new AnimationCommand() { secToStart = 8, cmd = "Combo", cmdParams = "Alert_lookright" },
            new AnimationCommand() { secToStart = 10, cmd = "Combo", cmdParams = "Acknowledge" },
            new AnimationCommand() { secToStart = 12, cmd = "Combo", cmdParams = "Yell" },
            new AnimationCommand() { secToStart = 14, cmd = "Default" }
        };
    }
}
