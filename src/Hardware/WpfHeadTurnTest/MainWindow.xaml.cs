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

using System.Threading;
using System.Windows.Threading;
using System.Diagnostics;

using kinect = Microsoft.Kinect;

using TrackRoamer.Robotics.LibBehavior;
using TrackRoamer.Robotics.LibActuators;
using System.IO;
using System.Collections.ObjectModel;

namespace WpfHeadTurnTest
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        double samplingIntervalSeconds = 0.1d;
        double samplingIntervalMilliSeconds;
        double displayIntervalMilliSeconds = 300.0d;

        DispatcherTimer dispatcherTimer;

        PanTiltAlignment _panTiltAlignment;

        #region MainWindow lifecycle

        public MainWindow()
        {
            InitializeComponent();

            ItemsSetpoint = new ObservableCollection<KeyValuePair<int, double>>();
            ItemsServo = new ObservableCollection<KeyValuePair<int, double>>();
            ItemsMeasured = new ObservableCollection<KeyValuePair<int, double>>();
            this.DataContext = this;

            samplingIntervalMilliSeconds = samplingIntervalSeconds * 1000.0d;

            InitPid();

            dispatcherTimer = new DispatcherTimer(DispatcherPriority.Normal, this.Dispatcher);
            dispatcherTimer.Interval = new TimeSpan((long)(samplingIntervalMilliSeconds * TimeSpan.TicksPerMillisecond));
            dispatcherTimer.Tick += new EventHandler(dispatcherTimer_Tick);

            this.Closing += new System.ComponentModel.CancelEventHandler(MainWindow_Closing);
        }

        private void Window_Loaded(object sender, EventArgs e)
        {
            _panTiltAlignment = PanTiltAlignment.RestoreOrDefault();

            // panKinectTargetPosScrollBar.IsEnabled = false;

            SafePosture();

            InitKinect();

            InitPidControls();

            CollectAndApplyPidControls();

            dispatcherTimer.Start();
        }

        void MainWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            dispatcherTimer.Stop();
            SafePosture();

            if (null != this.sensor)
            {
                this.sensor.Stop();
            }
        }

        private void Window_Closed(object sender, EventArgs e)
        {
        }

        #endregion // MainWindow lifecycle

        #region Helpers

        delegate void UpdateLabelDelegate(string txt);

        void updatePmValuesLabel(string txt)
        {
            pmValuesLabel.Content = txt;
        }

        void updateTargetPosLabel(string txt)
        {
            TargetPosLabel.Content = txt;
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

        #endregion // Helpers

        #region Servo controls

        PololuMaestroConnector pololuMaestroConnector = new PololuMaestroConnector();

        private void setPanKinect(double degreesFromCenter)
        {
            this.pololuMaestroConnector.setPanKinect(degreesFromCenter);
        }

        private void SafePosture()
        {
            pololuMaestroConnector.setPanTilt(0.0d, 0.0d, 0.0d, 0.0d, 0.0d);
        }

        #endregion // Servo and gun controls

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
                //skeletonImage.Source = this.imageSource;

                // Turn on the skeleton stream to receive skeleton frames
                this.sensor.SkeletonStream.Enable(parameters);
                this.sensor.DepthStream.Enable();
                this.sensor.ColorStream.Enable();

                // Add an event handler to be called whenever there is new color frame data
                this.sensor.SkeletonFrameReady += this.SensorSkeletonFrameReady;
                //this.sensor.DepthFrameReady += this.SensorDepthFrameReady;
                this.sensor.ColorFrameReady += this.SensorColorFrameReady;

                // Allocate space to put the color pixels we'll receive
                this.colorFramePixels = new byte[this.sensor.ColorStream.FramePixelDataLength];

                // Allocate space to put the depth pixels we'll receive
                //this.depthPixels = new short[this.sensor.DepthStream.FramePixelDataLength];

                // Allocate space to put the color pixels we'll get as result of Depth pixels conversion. One depth pixel will amount to BGR - three color pixels plus one unused
                //this.colorDepthPixels = new byte[this.sensor.DepthStream.FramePixelDataLength * 4];

                // This is the bitmap we'll display on-screen. To work with  bitmap extensions(http://writeablebitmapex.codeplex.com/) must be PixelFormats.Pbgra32
                this.colorBitmapVideo = new WriteableBitmap(this.sensor.ColorStream.FrameWidth, this.sensor.ColorStream.FrameHeight, 96.0, 96.0, PixelFormats.Pbgra32, null);
                //this.colorBitmapDepth = new WriteableBitmap(this.sensor.DepthStream.FrameWidth, this.sensor.DepthStream.FrameHeight, 96.0, 96.0, PixelFormats.Bgr32, null);

                // Set the image we display to point to the bitmap where we'll put the image data
                videoImage.Source = this.colorBitmapVideo;
                //depthImage.Source = this.colorBitmapDepth;

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

        DateTime lastReadServos = DateTime.MinValue;

        /// <summary>
        /// Bitmap that will hold color information for video
        /// </summary>
        private WriteableBitmap colorBitmapVideo;

        /// <summary>
        /// Intermediate storage for the color frame pixels
        /// </summary>
        private byte[] colorFramePixels;

        void SensorColorFrameReady(object sender, kinect.ColorImageFrameReadyEventArgs e)
        {
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

        /// <summary>
        /// Event handler for Kinect sensor's SkeletonFrameReady event
        /// </summary>
        /// <param name="sender">object sending the event</param>
        /// <param name="e">event arguments</param>
        private void SensorSkeletonFrameReady(object sender, kinect.SkeletonFrameReadyEventArgs e)
        {
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

        private TrajectoryPredictor predictor = new TrajectoryPredictor();

        private bool inServoControl = false;

        private void processSkeletons(kinect.Skeleton[] skeletons)
        {
            int iSkeleton = 0;
            int iSkeletonsTracked = 0;

            double panAngle = double.NaN;       // degrees from forward; right positive
            double tiltAngle = double.NaN;
            double targetX = double.NaN;        // meters
            double targetY = double.NaN;
            double targetZ = double.NaN;

            kinect.Skeleton trackedSkeleton = (from s in skeletons
                                               where s.TrackingState == kinect.SkeletonTrackingState.Tracked
                                               select s).FirstOrDefault();

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
                            //RenderClippedEdges(skel, dc);
                            //this.DrawBonesAndJoints(skel, dc);

                            //if (gameOnCheckBox.IsChecked.GetValueOrDefault()
                            //    && skel.Joints[kinect.JointType.Head].TrackingState == kinect.JointTrackingState.Tracked
                            //    && skel.Joints[kinect.JointType.HandLeft].TrackingState == kinect.JointTrackingState.Tracked
                            //    && skel.Joints[kinect.JointType.HandRight].TrackingState == kinect.JointTrackingState.Tracked
                            //    && skel.Joints[kinect.JointType.HandLeft].Position.Y > skel.Joints[kinect.JointType.Head].Position.Y
                            //    && skel.Joints[kinect.JointType.HandRight].Position.Y > skel.Joints[kinect.JointType.Head].Position.Y)
                            //{
                            //    if (!handsUp && (DateTime.Now - lastHandsUp).TotalSeconds > 1.0d)
                            //    {
                            //        speak();
                            //        lastHandsUp = DateTime.Now;
                            //        handsUp = true;
                            //        shootBoth();
                            //    }
                            //    else
                            //    {
                            //        handsUp = false;
                            //    }
                            //}

                            // http://social.msdn.microsoft.com/Forums/en-AU/kinectsdk/thread/d821df8d-39ca-44e3-81e7-c907d94acfca  - data.UserIndex is always 254

                            //bool targetedUser = skel.UserIndex == 1;
                            bool targetedUser = skel == trackedSkeleton;

                            if (targetedUser)
                            {
                                targetX = skel.Position.X;   // meters
                                targetY = skel.Position.Y;
                                targetZ = skel.Position.Z;

                                // can set Pan or Tilt to NaN:
                                panAngle = -Math.Atan2(targetX, targetZ) * 180.0d / Math.PI;
                                tiltAngle = Math.Atan2(targetY, targetZ) * 180.0d / Math.PI;
                            }

                            iSkeletonsTracked++;
                        }

                        iSkeleton++;
                    } // for each skeleton
                }
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

                        if (gameOnCheckBox.IsChecked.GetValueOrDefault())
                        {
                            this.Dispatcher.Invoke(new UpdateLabelDelegate(updatePmValuesLabel), string.Format("X: {0:0.000}\r\nY: {1:0.000}\r\nZ: {2:0.000}\r\nPan: {3:0}\r\nTilt: {4:0}", targetX, targetY, targetZ, panAngle, tiltAngle));

                            //pololuMaestroConnector.setPanTilt(futurePoint.panAngle, futurePoint.tiltAngle, futurePoint.panAngle, futurePoint.tiltAngle, 0.0d);

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
                                double kinectPanDesiredDegrees = targetPanRelativeToRobot;      // will be processed in computeHeadTurn() when head turn measurement comes.

                                panKinectTargetPosScrollBar.Value = _panTiltAlignment.mksPanKinect(kinectPanDesiredDegrees);
                            }
                        }
                    }

                    inServoControl = false;
                }
            }
            else if (gameOnCheckBox.IsChecked.GetValueOrDefault())
            {
                // lost skeletons, center the head:
                panKinectTargetPosScrollBar.Value = 1500.0d;
            }
        }

        #endregion // Kinect related

        private double panKinectSetpointMks = 1500;

        private void setPanKinectMks(double mks)
        {
            int valMks = (int)Math.Round(Math.Min(Math.Max(mks,861.0d),2190.0d));

            this.pololuMaestroConnector.setPanKinectMks(valMks);
        }

        private void panKinectTargetPosScrollBar_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            panKinectSetpointMks = e.NewValue;

            //setPanKinectMks(panKinectSetpointMks);

            if (TargetPosLabel != null && TargetPosLabel.IsLoaded)
            {
                TargetPosLabel.Content = string.Format("{0,5:0}", panKinectSetpointMks);
                //this.Dispatcher.Invoke(new UpdateLabelDelegate(updateTargetPosLabel), string.Format("{0}", panKinectMks));
            }
        }

        bool canDisplay = false;

        private void displayMeasuredValue(double measuredValueMks, double measuredValueDegrees)
        {
            if (!canDisplay)
                return;

            if (MeasuredPosLabel != null && MeasuredPosLabel.IsLoaded)
            {
                MeasuredPosLabel.Content = string.Format("{0,5:0}   {1,5:0}", measuredValueMks, measuredValueDegrees);
            }

            if (panKinectMeasuredPosScrollBar != null && panKinectMeasuredPosScrollBar.IsLoaded)
            {
                panKinectMeasuredPosScrollBar.Value = measuredValueMks;
            }
        }

        private void displayServoInput(double servoInputMks)
        {
            if (!canDisplay)
                return;

            if (panKinectServoPosBar != null && panKinectServoPosBar.IsLoaded)
            {
                panKinectServoPosBar.Value = servoInputMks;
            }

            if (ServoPosLabel != null && ServoPosLabel.IsLoaded)
            {
                ServoPosLabel.Content = string.Format("{0,5:0}", servoInputMks);
            }
        }

        private DateTime lastDisplayTime;
        private double processingTimeMs;
        private double measuredKinectPanDegrees;

        private int index = 1;
        private Random random = new Random();

        // chart control binds to these collections: 
        public ObservableCollection<KeyValuePair<int, double>> ItemsSetpoint { get; set; }
        public ObservableCollection<KeyValuePair<int, double>> ItemsServo { get; set; }
        public ObservableCollection<KeyValuePair<int, double>> ItemsMeasured { get; set; }

        private void dispatcherTimer_Tick(object sender, EventArgs e)
        {
            DateTime tickTime = DateTime.Now;
            TimeSpan samplingTimeSpan = tickTime - lastReadServos;

            if ((tickTime - lastDisplayTime).TotalMilliseconds > displayIntervalMilliSeconds)
            {
                canDisplay = true;
                lastDisplayTime = tickTime;
            }

            // take a measurement where the head actually is via feedback potentiometer. We spend around 1ms here:
            int measuredValue = pololuMaestroConnector.TryGetTarget(ServoChannelMap.headPanFeedback);   // 153 right, 400 center, 644 left. Shows divided by 4 in Mini Maestro UI

            double measuredValueDegrees = _panTiltAlignment.degreesPanKinect(measuredValue);

            this.measuredKinectPanDegrees = measuredValueDegrees;

            double measuredValueMks = MapMeasuredPos(measuredValueDegrees);

            displayMeasuredValue(measuredValueMks, measuredValueDegrees);

            double errorMks = panKinectSetpointMks - measuredValueMks;

            // compute servo input:
            double servoInputMks = ComputeServoInput(panKinectSetpointMks, measuredValueMks, (ulong)(tickTime.Ticks / TimeSpan.TicksPerMillisecond));

            displayServoInput(servoInputMks);

            // apply servo input to Kinect pan actuator. We spend around 1ms here:
            if (UsePIDCheckBox.IsChecked.GetValueOrDefault())
            {
                setPanKinectMks(servoInputMks);
            }
            else
            {
                setPanKinectMks(panKinectSetpointMks);
            }

            // housekeeping:
            if (canDisplay && pmValuesLabel != null && pmValuesLabel.IsLoaded)
            {
                // display last processingTimeMs - not distorted by canDisplay:
                pmValuesLabel.Content = string.Format("diff: {0,5:0}\nms: {1,6:#.00}\nms: {2,6:#.000}", errorMks, samplingTimeSpan.TotalMilliseconds, processingTimeMs);
            }

            // plot the graphs:
            this.Dispatcher.BeginInvoke(new Action(() =>
            {
                if (index >= 100)
                {
                    ItemsSetpoint.RemoveAt(0);
                    ItemsServo.RemoveAt(0);
                    ItemsMeasured.RemoveAt(0);
                }
                else
                {
                    for (index = 0; index < 99; index++)
                    {
                        ItemsSetpoint.Add(new KeyValuePair<int, double>(index, panKinectSetpointMks));
                        ItemsServo.Add(new KeyValuePair<int, double>(index, servoInputMks));
                        ItemsMeasured.Add(new KeyValuePair<int, double>(index, measuredValueMks));
                    }
                }

                ItemsSetpoint.Add(new KeyValuePair<int, double>(index, panKinectSetpointMks));
                ItemsServo.Add(new KeyValuePair<int, double>(index, servoInputMks));
                ItemsMeasured.Add(new KeyValuePair<int, double>(index, measuredValueMks));
                index++;
            }));

            if (!canDisplay)
            {
                TimeSpan processingTimeSpan = DateTime.Now - tickTime;      // most of time we spent in pololuMaestroConnector - reading input and controlling servos.
                processingTimeMs = processingTimeSpan.TotalMilliseconds;
            }
            lastReadServos = tickTime;
            canDisplay = false;
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
            catch(Exception ex)
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

        private void CalibrateKinectPanFeedbackButton_Click(object sender, RoutedEventArgs e)
        {
            CalibrateKinectPlatform();
        }

        #region Kinect pan calibrating

        private bool KinectCalibratingInProgress = false;

        private void CalibrateKinectPlatform()
        {
            double analogValueAt0 = 0.0d;
            double analogValueAtM70 = 0.0d;
            double analogValueAtP70 = 0.0d;

            Thread.Sleep(500);

            // at this point Kinect platform is probably already set to 0 degrees, so we don't need to wait that long for it to settle. 
            KinectCalibratingInProgress = true;
            setPanKinect(0.0d);
            Thread.Sleep(2000);

            Debug.WriteLine("Calibrating Kinect Platform");

            analogValueAt0 = pololuMaestroConnector.TryGetTarget(ServoChannelMap.headPanFeedback);

            Debug.WriteLine("Calibrating Kinect Platform at 70");

            setPanKinect(70.0d);
            Thread.Sleep(4000);
            analogValueAtP70 =  pololuMaestroConnector.TryGetTarget(ServoChannelMap.headPanFeedback);

            Debug.WriteLine("Calibrating Kinect Platform at -70");

            setPanKinect(-70.0d);
            Thread.Sleep(4000);
            analogValueAtM70 = pololuMaestroConnector.TryGetTarget(ServoChannelMap.headPanFeedback);

            setPanKinect(0.0d);

            // expecting values:  -70=374  0=581  +70=859   (obsolete, old platform)
            // expecting values:  -70=583  0=403  +70=219   (new two-gun platform, PIC-measured)
            // expecting values:  -70=566  0=391  +70=214   (new two-gun platform, Pololu Mini Maestro measured)

            Debug.WriteLine(string.Format("Calibrating Kinect Platform: -70={0}  0={1}  +70={2}", analogValueAtM70, analogValueAt0, analogValueAtP70));

            double spanHKinect0_M70 = 70.0d;
            double spanHKinect0_P70 = 70.0d;

            _panTiltAlignment.computeCalibrationKinectAnalog(analogValueAtM70, analogValueAt0, analogValueAtP70, spanHKinect0_M70, spanHKinect0_P70);

            // to produce default values for PanTiltAlignment:
            CalibratedValuesTextBox.Text = string.Format("{0} {1} {2}", _panTiltAlignment.panFactorKinectAnalogPlus, _panTiltAlignment.panFactorKinectAnalogMinus, _panTiltAlignment.panAlignKinectAnalog);

            /*
            // for debugging - position the platform in the same spots again and check tracing at DriveBehaviorProxibrick.cs:trpbUpdateAnalogNotification()
            Thread.Sleep(5000);
            setPanKinect(70.0d);
            Thread.Sleep(5000);
            Debug.WriteLine("Checking calibration at +70 - measured value is " + currentPanKinect + " degrees");
            setPanKinect(0.0d);
            Thread.Sleep(5000);
            Debug.WriteLine("Checking calibration at +0 - measured value is " + currentPanKinect + " degrees");
            setPanKinect(-70.0d);
            Thread.Sleep(5000);
            Debug.WriteLine("Checking calibration at -70 - measured value is " + currentPanKinect + " degrees");
            */

            KinectCalibratingInProgress = false;

            // do some sanity checking:
            bool calibratedOk = analogValueAtM70 > 530 && analogValueAtM70 < 620
                                && analogValueAt0 > 350 && analogValueAt0 < 450
                                && analogValueAtP70 > 180 && analogValueAtP70 < 250;

            Debug.WriteLine(calibratedOk ? "Finished Calibrating Kinect Platform - OK" : "Error! Error! Error Calibrating Kinect Platform");
        }

        #endregion // Kinect pan calibrating

        private void ApplyPidButton_Click(object sender, RoutedEventArgs e)
        {
            CollectAndApplyPidControls();
        }
    }
}

