//------------------------------------------------------------------------------
//  <copyright file="KinectUI.xaml.cs" company="Microsoft Corporation">
//      Copyright (C) Microsoft Corporation.  All rights reserved.
//  </copyright>
//------------------------------------------------------------------------------

namespace TrackRoamer.Robotics.Services.TrackRoamerBehaviors
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Windows;
    using System.Windows.Media;
    using System.Windows.Media.Imaging;
    using System.Windows.Shapes;
    using Microsoft.Kinect;
    using common = Microsoft.Robotics.Common;
    using kinect = Microsoft.Robotics.Services.Sensors.Kinect.Proxy;
    using System.Windows.Controls;

    /// <summary>
    /// Main UI window - responsible for showing the user interface, accepting user input and communicating with KinectUIService when there is need to talk to Kinect service
    /// </summary>
    public partial class KinectUI : Window
    {
        // Using a DependencyProperty as the backing store.  This enables animation, styling, binding, etc...

        /// <summary>
        /// Viseme Property
        /// </summary>
        public static readonly DependencyProperty VisemeProperty =
            DependencyProperty.Register("KinectUIString", typeof(int), typeof(KinectUI));

        /// <summary>
        /// Reference to DSS service for sending operations to the Kinect Service
        /// </summary>
        private TrackRoamerBehaviorsService kinectUIService;

        /// <summary>
        /// Used to integrate recent frame readings to come up with a reasonable running FPS average
        /// </summary>
        private List<double> recentFrameRateContainer = new List<double>();

        /// <summary>
        /// Used to calculate running FPS
        /// </summary>
        private double lastFrameUpdatedTime = 0;

        /// <summary>
        /// Framerate achieved
        /// </summary>
        private double runningFPS = 0;

        // Pre-create brushes to draw body parts and joints for perf and convenience

        /// <summary>
        /// Black Brush
        /// </summary>
        private SolidColorBrush blackBrush = new SolidColorBrush(Colors.Black);

        /// <summary>
        /// Green Brush
        /// </summary>
        private SolidColorBrush greenBrush = new SolidColorBrush(Colors.Green);

        /// <summary>
        /// Red Brush
        /// </summary>
        private SolidColorBrush redBrush = new SolidColorBrush(Colors.Red);

        /// <summary>
        /// Yellow Brush
        /// </summary>
        private SolidColorBrush yellowBrush = new SolidColorBrush(Colors.Gold);

        /// <summary>
        /// Wheat Brush
        /// </summary>
        private SolidColorBrush wheatBrush = new SolidColorBrush(Colors.Wheat); 

        /// <summary>
        /// Magenta Brush
        /// </summary>
        private SolidColorBrush magentaBrush = new SolidColorBrush(Colors.Magenta); 

        /// <summary>
        /// View was updated atleast once
        /// </summary>
        private bool viewUpdatedAtLeastOnce = false;

        /// <summary>
        /// We need this when pre-processing skeletal frame to calculate point positions relative to the depth frame
        /// </summary>
        public static double DisplayWindowWidth = 0;

        /// <summary>
        /// We need this when pre-processing skeletal frame to calculate point positions relative to the depth frame
        /// </summary>
        public static double DisplayWindowHeight = 0;

        /// <summary>
        /// We need this when pre-processing skeletal frame to calculate point positions relative to the color image
        /// </summary>
        public static int ColorImageWidth = 0;

        /// <summary>
        /// We need this when pre-processing skeletal frame to calculate point positions relative to the color image
        /// </summary>
        public static int ColorImageHeight = 0;

        /// <summary>
        /// We need this when processing depth values
        /// </summary>
        public static int MaxValidDepth = 0;
                
        /// <summary>
        /// Creates a new instance of the user interface
        /// </summary>
        public KinectUI()
        {
            this.InitializeComponent();
        }

        /// <summary>
        /// Show a message when Kinect UI was closed without shutting down the node.
        /// </summary>
        /// <param name="sender">The parameter is not used.</param>
        /// <param name="e">The parameter is not used.</param>
        private void KinectUI_Closed(object sender, EventArgs e)
        {
            MessageBox.Show(
                this,
                Properties.Resources.NodeIsNotClosed,
                Title,
                MessageBoxButton.OK,
                MessageBoxImage.Information);
        }

        /// <summary>
        /// Creates a new instance of the user interface
        /// </summary>
        /// <param name="service">The service that handles communication with the Kinect service</param>
        internal KinectUI(TrackRoamerBehaviorsService service)
            : this()
        {
            this.kinectUIService = service;

            KinectUI.DisplayWindowHeight = this.SkeletonCanvas.ActualHeight;
            KinectUI.DisplayWindowWidth = this.SkeletonCanvas.ActualWidth;
            this.DepthCB.IsChecked = false; //  this.kinectUIService.IncludeDepth;
            this.SkeletalCB.IsChecked = this.kinectUIService.IncludeSkeletons;
            this.VideoCB.IsChecked = this.kinectUIService.IncludeVideo;
            this.VideoCBProcessed.IsChecked = true;

            this.Closed += this.KinectUI_Closed;
        }

        int cycleCounter = 0;     // helps to only draw every N-th frame

        /// <summary>
        /// Draw the frame
        /// </summary>
        /// <param name="framePreProcessor">Frame preprocessor</param>
        public void DrawFrame(FramePreProcessor framePreProcessor)
        {
            this.RegisterFrameRead();       // frame rate calculation

            if (cycleCounter % 2 == 0)      // only draw every N-th frame
            {
                bool doRgb = (bool)this.VideoCB.IsChecked;
                bool doDepth = (bool)this.DepthCB.IsChecked;
                bool doSkel = (bool)this.SkeletalCB.IsChecked;

                if (doRgb)
                {
                    this.DrawRGBImage(framePreProcessor);
                }

                if (doDepth)
                {
                    this.DrawDepthImage(framePreProcessor);
                }

                this.SkeletonCanvas.Children.Clear();

                if (doSkel)
                {
                    this.DrawSkeletons(framePreProcessor);
                }

                // infrequently adjust opacity 
                if (cycleCounter % 20 == 0)
                {
                    this.RgbImage.Opacity = doRgb ? 1.0d : 0.0d;
                    this.DepthImage.Opacity = doDepth ? (doRgb ? 0.6d : 1.0d) : 0.0d;
                }
            }

            cycleCounter++;
        }

        /// <summary>
        /// Draw the RGB Image
        /// </summary>
        /// <param name="framePreProcessor">Frame preprocessor</param>
        private void DrawRGBImage(FramePreProcessor framePreProcessor)
        {
            if (null == framePreProcessor.RawFrames.RawColorFrameData)
            {
                return;
            }

            BitmapSource bitmapSource = BitmapSource.Create(
                framePreProcessor.RawFrames.RawColorFrameInfo.Width,
                framePreProcessor.RawFrames.RawColorFrameInfo.Height, 
                96, 
                96, 
                PixelFormats.Bgr32, 
                null,
                framePreProcessor.RawFrames.RawColorFrameData,
                framePreProcessor.RawFrames.RawColorFrameInfo.Width * 
                    framePreProcessor.RawFrames.RawColorFrameInfo.BytesPerPixel);

            this.RgbImage.Source = bitmapSource;

            if (this.kinectUIService.IncludeVideoProcessed)
            {
                this.RgbImageProcessed.Visibility = System.Windows.Visibility.Visible;
                this.RgbImageProcessed.Source = FramePreProcessor.ToBitmapSource(framePreProcessor.BitmapProcessed);
            }
            else
            {
                this.RgbImageProcessed.Visibility = System.Windows.Visibility.Hidden;
            }
        }

        /// <summary>
        /// Overlay depth image
        /// </summary>
        /// <param name="framePreProcessor">Pre processed frame</param>
        private void DrawDepthImage(FramePreProcessor framePreProcessor)
        {
            byte[] depthImageBytes = framePreProcessor.GetDepthBytes();

            this.DepthImage.Source = BitmapSource.Create(
                framePreProcessor.RawFrames.RawDepthFrameInfo.Width,
                framePreProcessor.RawFrames.RawDepthFrameInfo.Height, 
                96, 
                96, 
                PixelFormats.Bgr32, 
                null,
                depthImageBytes,
                framePreProcessor.RawFrames.RawDepthFrameInfo.Width * PixelFormats.Bgr32.BitsPerPixel / 8);
        }

        private double canvasFactor;  // used for scaling lines and font to allow window resizing

        /// <summary>
        /// A method for drawing all joints and bones
        /// </summary>
        /// <param name="framePreProcessor">Pre processed frame, including skeletal data</param>
        private void DrawSkeletons(FramePreProcessor framePreProcessor)
        {
            this.SkeletonQualityText.Text = string.Empty;
            canvasFactor = this.SkeletonCanvas.ActualWidth + this.SkeletonCanvas.ActualHeight;

            var tmpAllSkeletons = framePreProcessor.AllSkeletons;  // get a snapshot of the pointer to allocated array, and then take sweet time processing it knowing it will not change

            foreach (var skeleton in tmpAllSkeletons.Where(skeleton => skeleton.IsSkeletonActive))
            {
                this.DrawBodySegments(skeleton, skeleton.IsMainSkeleton ? this.greenBrush : this.wheatBrush);

                this.DrawJoints(skeleton);

                this.UpdateAdditionalSkeletalInfoField(skeleton);
            }
        }

        /// <summary>
        /// Draws bones
        /// </summary>        
        /// <param name="skeleton">PreProcessed Skeleton data</param>
        /// <param name="currentSkeletonBrush">Brush for the skeleton</param>
        private void DrawBodySegments(VisualizableSkeletonInformation skeleton, Brush currentSkeletonBrush)
        {
            int thickness = (int)Math.Max(5.0d, canvasFactor / 200.0d);

            this.DrawBodySegment(
                skeleton, 
                currentSkeletonBrush,
                thickness,
                JointType.ShoulderCenter, 
                JointType.Head);

            this.DrawBodySegment(
                skeleton, 
                currentSkeletonBrush,
                thickness,
                JointType.HipCenter, 
                JointType.Spine, 
                JointType.ShoulderCenter);

            this.DrawBodySegment(
                skeleton, 
                currentSkeletonBrush,
                thickness,
                JointType.ShoulderCenter, 
                JointType.ShoulderLeft, 
                JointType.ElbowLeft, 
                JointType.WristLeft, 
                JointType.HandLeft);

            this.DrawBodySegment(
                skeleton, 
                currentSkeletonBrush,
                thickness,
                JointType.ShoulderCenter, 
                JointType.ShoulderRight, 
                JointType.ElbowRight, 
                JointType.WristRight, 
                JointType.HandRight);

            this.DrawBodySegment(
                skeleton, 
                currentSkeletonBrush,
                thickness,
                JointType.HipCenter, 
                JointType.HipLeft, 
                JointType.KneeLeft, 
                JointType.AnkleLeft, 
                JointType.FootLeft);

            this.DrawBodySegment(
                skeleton, 
                currentSkeletonBrush,
                thickness,
                JointType.HipCenter, 
                JointType.HipRight, 
                JointType.KneeRight, 
                JointType.AnkleRight, 
                JointType.FootRight);
        }

        /// <summary>
        /// Sets the text for skeletal tracking quality (i.e. clipped bottom, etc)
        /// </summary>
        /// <param name="theSkeleton">Skeleton data</param>
        private void UpdateAdditionalSkeletalInfoField(VisualizableSkeletonInformation theSkeleton)
        {
            this.SkeletonQualityText.Text += theSkeleton.SkeletonQuality;
        }

        private void DrawText(double x, double y, string text, Brush colorBrushForeground, Brush colorBrushBackground)
        {
            TextBlock textBlock = new TextBlock();
            textBlock.Padding = new Thickness(Math.Max(1.0d, canvasFactor / 200.0d));
            textBlock.FontFamily = new FontFamily("Arial");
            textBlock.FontSize = Math.Max(12.0d, canvasFactor / 70.0d);
            textBlock.Text = text;
            textBlock.Foreground = colorBrushForeground;
            textBlock.Background = colorBrushBackground;
            Canvas.SetLeft(textBlock, x);
            Canvas.SetTop(textBlock, y);
            this.SkeletonCanvas.Children.Add(textBlock);
        }

        /// <summary>
        /// Draws joint points. Uses different colors to indicate the quality of joint tracking
        /// </summary>
        /// <param name="theSkeleton">The Skeleton Info</param>
        private void DrawJoints(VisualizableSkeletonInformation theSkeleton)
        {
            int thickness = (int)Math.Max(5.0d, canvasFactor / 200.0d);

            foreach (VisualizableJoint joint in theSkeleton.JointPoints.Values)
            {
                int thisJointThickness = joint.IsJointOfInterest ? (thickness*2) : thickness;

                Line jointLine = new Line();

                jointLine.X1 = joint.JointCoordinates.X - thisJointThickness;
                jointLine.X2 = joint.JointCoordinates.X + thisJointThickness;
                jointLine.Y1 = joint.JointCoordinates.Y;
                jointLine.Y2 = joint.JointCoordinates.Y;
                jointLine.StrokeThickness = thisJointThickness * 2;

                jointLine.Stroke = this.greenBrush;     // draws tracked joints in Green

                if (JointTrackingState.Inferred == joint.TrackingState)
                {
                    jointLine.Stroke = this.yellowBrush;
                }
                else if (JointTrackingState.NotTracked == joint.TrackingState)
                {
                    jointLine.Stroke = this.redBrush;
                }

                this.SkeletonCanvas.Children.Add(jointLine);

                if (joint.jointType == JointType.Head)
                {
                    // draw Skeleton infor near the Head joint:
                    string idString = theSkeleton.ToString();

                    DrawText(joint.JointCoordinates.X + canvasFactor / 40.0d,
                             joint.JointCoordinates.Y - canvasFactor / 30.0d,
                             idString,
                             theSkeleton.IsMainSkeleton ? this.greenBrush : this.wheatBrush,
                             blackBrush);
                }
            }
        }

        /// <summary>
        /// Draw a collection of connected bones
        /// </summary>
        /// <param name="theSkeleton">Skeletal data</param>
        /// <param name="brush">Brush color</param>
        /// <param name="thickness">Thickness of the bone (we may use different thickenss for better visualization)</param>
        /// <param name="ids">Joint IDs to connect</param>
        private void DrawBodySegment(VisualizableSkeletonInformation theSkeleton, Brush brush, int thickness, params JointType[] ids)
        {
            Polyline polyline = new Polyline();

            for (int i = 0; i < ids.Length; ++i)
            {
                polyline.Points.Add(theSkeleton.JointPoints[ids[i]].JointCoordinates);
            }

            polyline.Stroke = brush;
            polyline.StrokeThickness = thickness;

            this.SkeletonCanvas.Children.Add(polyline);
        }

        /// <summary>
        /// Used to calculate FPS
        /// </summary>
        private void RegisterFrameRead()
        {
            this.recentFrameRateContainer.Add(common.Utilities.ElapsedSecondsSinceStart - this.lastFrameUpdatedTime);
            this.lastFrameUpdatedTime = common.Utilities.ElapsedSecondsSinceStart;

            // we integrate over last 30 frames - this gives us a reasonably accurate and smooth 'running FPS' figure 
            if (this.recentFrameRateContainer.Count >= 30)
            {
                this.recentFrameRateContainer.RemoveAt(0);
            }
        }

        /// <summary>
        /// Calculate FPS
        /// </summary>
        private void CalculateEffectiveFrameRate()
        {
            double combinedDelayAcrossRecentFrames = this.recentFrameRateContainer.Sum();

            this.runningFPS = combinedDelayAcrossRecentFrames / this.recentFrameRateContainer.Count;

            this.FPS.Text = string.Format("Effective FPS: {0:##}", 1 / this.runningFPS);
        }

        /// <summary>
        /// Callback to update UI elements that represent KinectService state(config) such as image resolution, etc
        /// </summary>
        /// <param name="state">Kinect state</param>
        internal void UpdateState(kinect.KinectState state)
        {
            this.CalculateEffectiveFrameRate();

            this.UpdateControlValuesBasedOnState(state);

            if (!this.viewUpdatedAtLeastOnce)
            {
                this.UpdateImmutableControlsOnFirstStateRead(state);
            }

            this.viewUpdatedAtLeastOnce = true;
        }

        /// <summary>
        /// Update Control Values based on state
        /// </summary>
        /// <param name="state">Kinect state</param>
        private void UpdateControlValuesBasedOnState(kinect.KinectState state)
        {
            this.TransformSmooth.IsChecked = state.TransformSmooth;

            // we don't want to update those fields while user may be typing something
            if (!this.TiltDegrees.IsFocused)
            {
                this.TiltDegrees.Text = ((int)state.TiltDegrees).ToString();
            }

            if (!this.Smoothing.IsFocused)
            {
                this.Smoothing.Text = state.SkeletalEngineTransformSmoothParameters.Smoothing.ToString();
            }

            if (!this.Correction.IsFocused)
            {
                this.Correction.Text = state.SkeletalEngineTransformSmoothParameters.Correction.ToString();
            }

            if (!this.Prediction.IsFocused)
            {
                this.Prediction.Text = state.SkeletalEngineTransformSmoothParameters.Prediction.ToString();
            }

            if (!this.JitterRadius.IsFocused)
            {
                this.JitterRadius.Text = state.SkeletalEngineTransformSmoothParameters.JitterRadius.ToString();
            }

            if (!this.MaxDeviationRadius.IsFocused)
            {
                this.MaxDeviationRadius.Text = state.SkeletalEngineTransformSmoothParameters.MaxDeviationRadius.ToString();
            }
        }

        /// <summary>
        /// We want to disable controls that can't be used due to config state (i.e. can't choose to view video if video frame read was not configured)
        /// </summary>
        /// <param name="state">Kinect state</param>
        private void UpdateImmutableControlsOnFirstStateRead(kinect.KinectState state)
        {
            if (!state.UseColor)
            {
                this.VideImageType.IsEnabled = false;
                this.VideoCB.IsEnabled = false;
                this.VideoCB.IsChecked = false;
            }

            if (!state.UseSkeletalTracking)
            {
                this.SkeletalCB.IsChecked = false;
                this.SkeletalCB.IsEnabled = false;
                this.TransformSmooth.IsEnabled = false;
                this.Smoothing.IsEnabled = false;
                this.Correction.IsEnabled = false;
                this.Prediction.IsEnabled = false;
                this.JitterRadius.IsEnabled = false;
                this.MaxDeviationRadius.IsEnabled = false;
            }

            if (!state.UseDepth)
            {
                this.DepthCB.IsChecked = false;
                this.DepthCB.IsEnabled = false;
            }

            this.DeviceID.Text = state.DeviceID.ToString();
            this.FrameRate.Text = state.FrameRate.ToString();
            this.VideImageType.Text = state.ColorImageFormat.ToString();
            this.DepthImageType.Text = state.DepthImageFormat.ToString();
            this.DepthCamAlternate.Text = state.IsDepthServiceUpdateEnabled == true ? "yes" : "no";
            this.WebCamAlternate.Text = state.IsWebCamServiceUpdateEnabled == true ? "yes" : "no";

            // cache the resolution of the color image
            switch (state.ColorImageFormat)
            {
                case ColorImageFormat.RgbResolution1280x960Fps12:
                    KinectUI.ColorImageWidth = 1280;
                    KinectUI.ColorImageHeight = 960;
                    break;
                case ColorImageFormat.RawYuvResolution640x480Fps15:
                case ColorImageFormat.RgbResolution640x480Fps30:
                case ColorImageFormat.YuvResolution640x480Fps15:
                default:
                    KinectUI.ColorImageWidth = 640;
                    KinectUI.ColorImageHeight = 480;
                    break;
            }
        }

        /// <summary>
        /// Displays a fault message
        /// </summary>
        /// <param name="fault">Error raised</param>
        internal void ShowFault(W3C.Soap.Fault fault)
        {
            var error = Properties.Resources.ErrorMessage;

            if (fault.Reason != null && fault.Reason.Length > 0 && !string.IsNullOrEmpty(fault.Reason[0].Value))
            {
                error = fault.Reason[0].Value;
            }

            MessageBox.Show(
                this,
                error,
                Title,
                MessageBoxButton.OK,
                MessageBoxImage.Error);
        }

        /// <summary>
        /// Update tilt value in the control
        /// </summary>
        /// <param name="sender">The parameter is not used.</param>
        /// <param name="e">The parameter is not used.</param>
        private void OnUpdateTilt(object sender, RoutedEventArgs e)
        {
            int degrees = int.Parse(this.TiltDegrees.Text);

            if (degrees < (int)kinect.KinectReservedSampleValues.MinimumTiltAngle || degrees > (int)kinect.KinectReservedSampleValues.MaximumTiltAngle)
            {
                string message = string.Format(
                    "Please enter an integer value between {0} and {1}. That's the range that Kinect camara supports",
                    (int)kinect.KinectReservedSampleValues.MinimumTiltAngle,
                    (int)kinect.KinectReservedSampleValues.MaximumTiltAngle);

                MessageBox.Show(message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);

                return;
            }

            this.kinectUIService.SetDesiredKinectTilt(degrees);
            this.kinectUIService.UpdateKinectTilt(degrees);
        }

        /// <summary>
        /// Transform smoothing checkbox updated
        /// </summary>
        /// <param name="sender">The parameter is not used.</param>
        /// <param name="e">The parameter is not used.</param>
        private void TransformSmoothChecked(object sender, RoutedEventArgs e)
        {
            if (this.viewUpdatedAtLeastOnce)
            {
                this.UpdateTransformSmooth();
            }
        }

        /// <summary>
        /// Transform field updated
        /// </summary>
        /// <param name="sender">The parameter is not used.</param>
        /// <param name="e">The parameter is not used.</param>
        private void FieldUpdated(object sender, RoutedEventArgs e)
        {
            this.UpdateTransformSmooth();
        }

        /// <summary>
        /// Update smoothing parameters
        /// </summary>
        private void UpdateTransformSmooth()
        {
            float smoothing = float.Parse(this.Smoothing.Text);
            float correction = float.Parse(this.Correction.Text);
            float prediction = float.Parse(this.Prediction.Text);
            float jitterRadius = float.Parse(this.JitterRadius.Text);
            float maxDeviationRadius = float.Parse(this.MaxDeviationRadius.Text);

            this.kinectUIService.UpdateSkeletalSmoothing(
                this.TransformSmooth.IsChecked == true,
                smoothing,
                correction,
                prediction,
                jitterRadius,
                maxDeviationRadius);
        }

        /// <summary>
        /// Handle video check box events
        /// </summary>
        /// <param name="sender">The parameter is not used.</param>
        /// <param name="e">The parameter is not used.</param>
        private void VideoCB_Checked(object sender, RoutedEventArgs e)
        {
            if (null != this.kinectUIService)
            {
                this.kinectUIService.IncludeVideo = (bool)this.VideoCB.IsChecked;
            }
        }

        private void VideoCBProcessed_Checked(object sender, RoutedEventArgs e)
        {
            if (null != this.kinectUIService)
            {
                this.kinectUIService.IncludeVideoProcessed = (bool)this.VideoCBProcessed.IsChecked;
            }
        }

        /// <summary>
        /// Handle depth check box events
        /// </summary>
        /// <param name="sender">The parameter is not used.</param>
        /// <param name="e">The parameter is not used.</param>
        private void DepthCB_Checked(object sender, RoutedEventArgs e)
        {
            if (null != this.kinectUIService)
            {
                this.kinectUIService.IncludeDepth = (bool)this.DepthCB.IsChecked;
            }
        }

        /// <summary>
        /// Handle skeletal check box events
        /// </summary>
        /// <param name="sender">The parameter is not used.</param>
        /// <param name="e">The parameter is not used.</param>
        private void SkeletalCB_Checked(object sender, RoutedEventArgs e)
        {
            if (null != this.kinectUIService)
            {
                this.kinectUIService.IncludeSkeletons = (bool)this.SkeletalCB.IsChecked;
            }
        }

        private void SkeletonCanvas_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            KinectUI.DisplayWindowHeight = this.SkeletonCanvas.ActualHeight;
            KinectUI.DisplayWindowWidth = this.SkeletonCanvas.ActualWidth;
        }
    }
}
