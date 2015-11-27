using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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

using System.Speech.Synthesis;

using TrackRoamer.Robotics.Utility.LibPicSensors;
using TrackRoamer.Robotics.Utility.LibSystem;

using Microsoft.Research.Kinect.Nui;

namespace WpfKinectTurret
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public ProximityModule _picpxmod = null;
        bool deviceAttached = false;
        DispatcherTimer dispatcherTimer;

        public MainWindow()
        {
            InitializeComponent();

            trackWhatComboBox.Items.Add("CG");
            trackWhatComboBox.Items.Add(JointID.WristLeft);
            trackWhatComboBox.Items.Add(JointID.WristRight);
            trackWhatComboBox.Items.Add(JointID.FootLeft);
            trackWhatComboBox.Items.Add(JointID.FootRight);

            trackWhatComboBox.SelectedIndex = 1;

            dispatcherTimer = new DispatcherTimer(DispatcherPriority.Normal, this.Dispatcher);
            dispatcherTimer.Tick += new EventHandler(dispatcherTimer_Tick);

            this.Closing += new System.ComponentModel.CancelEventHandler(MainWindow_Closing);
        }

        private void WebsiteLink_Click(object sender, RoutedEventArgs e)
        {
            // open URL:
            Hyperlink source = sender as Hyperlink;

            if (source != null)
            {
                System.Diagnostics.Process.Start(source.NavigateUri.ToString());
            }
        }

        // This will be called whenever the data is read from the board; it will work in a different thread:
        private void pmFrameCompleteHandler(object sender, AsyncInputFrameArgs aira)
        {
            Tracer.Trace("Async sonar frame arrived. " + DateTime.Now);

            if (this.Dispatcher.CheckAccess())
            {
                // do work on UI thread
                pmFrameCompleteHandler_UI(sender, aira);
            }
            else
            {
                // we normally get here - the async read from HID is happening on a different thread:
                this.Dispatcher.Invoke(new Action<object, AsyncInputFrameArgs>(pmFrameCompleteHandler_UI), sender, aira);                // or BeginInvoke()
            }
        }

        private void pmFrameCompleteHandler_UI(object sender, AsyncInputFrameArgs aira)
        {
            long timestamp = aira.timestamp;
        }

        delegate void UpdateLabelDelegate(string txt);

        void updatePmValuesLabel(string txt)
        {
            pmValuesLabel.Content = txt;
        }

        void updatePanMksLabel(string txt)
        {
            panMksLabel.Content = txt;
        }

        void updateTiltMksLabel(string txt)
        {
            tiltMksLabel.Content = txt;
        }

        void _picpxmod_DeviceDetachedEvent(object sender, ProximityModuleEventArgs e)
        {
            deviceAttached = false;
            this.Dispatcher.Invoke(new UpdateLabelDelegate(updatePmValuesLabel), "USB: Device Detached");
        }

        void _picpxmod_DeviceAttachedEvent(object sender, ProximityModuleEventArgs e)
        {
            this.Dispatcher.Invoke(new UpdateLabelDelegate(updatePmValuesLabel), "USB: Device Attached");
            deviceAttached = true;

            panMksLast = 0;
            tiltMksLast = 0;
        }

        protected override void OnInitialized(EventArgs e)
        {
            base.OnInitialized(e);
        }

        protected override void OnClosed(EventArgs e)
        {
            _picpxmod.Dispose();

            base.OnClosed(e);
        }

        void MainWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
        }

        private void pmSafePostureButton_Click(object sender, RoutedEventArgs e)
        {
            speak("safety activated");
            _picpxmod.SafePosture();
        }

        private void panScrollBar_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            setPan(e.NewValue);
        }

        private void tiltScrollBar_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            setTilt(e.NewValue);
        }

        double currentPan;
        double currentTilt;

        int panMksLast = 0;
        int tiltMksLast = 0;

        private void setPan(double degreesFromCenter)
        {
            currentPan = degreesFromCenter;

            double mks = 1500.0d + PanTiltAlignment.getInstance().panAlign - degreesFromCenter * PanTiltAlignment.getInstance().panFactor;

            int panMks = (int)mks;

            if (panMks != panMksLast)
            {
                panMksLast = panMks;

                if (_picpxmod != null && deviceAttached)
                {
                    _picpxmod.ServoPositionSet(1, mks);
                    this.Dispatcher.Invoke(new UpdateLabelDelegate(updatePmValuesLabel), string.Format("Pan: {0:0}\r\nTilt: {1:0}", currentPan, currentTilt));
                    this.Dispatcher.Invoke(new UpdateLabelDelegate(updatePanMksLabel), string.Format("{0,4} mks {1,4:0} degrees", panMksLast, currentPan));
                }
            }
        }

        private void setTilt(double degreesFromCenter)
        {
            currentTilt = degreesFromCenter;

            double mks = 1500.0d + PanTiltAlignment.getInstance().tiltAlign + degreesFromCenter * PanTiltAlignment.getInstance().tiltFactor;

            int tiltMks = (int)mks;

            if (tiltMks != tiltMksLast)
            {
                tiltMksLast = tiltMks;

                if (_picpxmod != null && deviceAttached)
                {
                    _picpxmod.ServoPositionSet(2, mks);
                    this.Dispatcher.Invoke(new UpdateLabelDelegate(updatePmValuesLabel), string.Format("Pan: {0:0}\r\nTilt: {1:0}", currentPan, currentTilt));
                    this.Dispatcher.Invoke(new UpdateLabelDelegate(updateTiltMksLabel), string.Format("{0,4} mks {1,4:0} degrees", tiltMksLast, currentTilt));
                }
            }
        }

        // ==========================================================================================================================

        Runtime nui;
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


        Dictionary<JointID, Brush> jointColors = new Dictionary<JointID, Brush>() { 
            {JointID.HipCenter, new SolidColorBrush(Color.FromRgb(169, 176, 155))},
            {JointID.Spine, new SolidColorBrush(Color.FromRgb(169, 176, 155))},
            {JointID.ShoulderCenter, new SolidColorBrush(Color.FromRgb(168, 230, 29))},
            {JointID.Head, new SolidColorBrush(Color.FromRgb(200, 0,   0))},
            {JointID.ShoulderLeft, new SolidColorBrush(Color.FromRgb(79,  84,  33))},
            {JointID.ElbowLeft, new SolidColorBrush(Color.FromRgb(84,  33,  42))},
            {JointID.WristLeft, new SolidColorBrush(Color.FromRgb(255, 126, 0))},
            {JointID.HandLeft, new SolidColorBrush(Color.FromRgb(215,  86, 0))},
            {JointID.ShoulderRight, new SolidColorBrush(Color.FromRgb(33,  79,  84))},
            {JointID.ElbowRight, new SolidColorBrush(Color.FromRgb(33,  33,  84))},
            {JointID.WristRight, new SolidColorBrush(Color.FromRgb(77,  109, 243))},
            {JointID.HandRight, new SolidColorBrush(Color.FromRgb(37,   69, 243))},
            {JointID.HipLeft, new SolidColorBrush(Color.FromRgb(77,  109, 243))},
            {JointID.KneeLeft, new SolidColorBrush(Color.FromRgb(69,  33,  84))},
            {JointID.AnkleLeft, new SolidColorBrush(Color.FromRgb(229, 170, 122))},
            {JointID.FootLeft, new SolidColorBrush(Color.FromRgb(255, 126, 0))},
            {JointID.HipRight, new SolidColorBrush(Color.FromRgb(181, 165, 213))},
            {JointID.KneeRight, new SolidColorBrush(Color.FromRgb(71, 222,  76))},
            {JointID.AnkleRight, new SolidColorBrush(Color.FromRgb(245, 228, 156))},
            {JointID.FootRight, new SolidColorBrush(Color.FromRgb(77,  109, 243))}
        };

        private void Window_Loaded(object sender, EventArgs e)
        {
            PanTiltAlignment.Restore();

            PanTiltAlignment pta = PanTiltAlignment.getInstance();

            panAlignScrollBar.Value = pta.panAlign;
            panFactorScrollBar.Value = pta.panFactor;
            tiltAlignScrollBar.Value = pta.tiltAlign;
            tiltFactorScrollBar.Value = pta.tiltFactor;

            // =============================================================
            //create picpxmod device:

            _picpxmod = new ProximityModule(0x0925, 0x7001);    // see PIC Firmware - usb_descriptors.c lines 178,179

            _picpxmod.HasReadFrameEvent += pmFrameCompleteHandler;

            _picpxmod.DeviceAttachedEvent += new EventHandler<ProximityModuleEventArgs>(_picpxmod_DeviceAttachedEvent);
            _picpxmod.DeviceDetachedEvent += new EventHandler<ProximityModuleEventArgs>(_picpxmod_DeviceDetachedEvent);

            deviceAttached = _picpxmod.FindTheHid();

            pmValuesLabel.Content = deviceAttached ?
                "Proximity Board Found" : string.Format("Proximity Board NOT Found\r\nYour USB Device\r\nmust have:\r\n vendorId=0x{0:X}\r\nproductId=0x{1:X}", _picpxmod.vendorId, _picpxmod.productId);

            // =============================================================
            // create Kinect device:

            nui = new Runtime();

            try
            {
                nui.Initialize(RuntimeOptions.UseDepthAndPlayerIndex | RuntimeOptions.UseSkeletalTracking | RuntimeOptions.UseColor);
            }
            catch (InvalidOperationException)
            {
                System.Windows.MessageBox.Show("Runtime initialization failed. Please make sure Kinect device is plugged in.");
                return;
            }

            // parameters used to smooth the skeleton data
            nui.SkeletonEngine.TransformSmooth = true;
            TransformSmoothParameters parameters = new TransformSmoothParameters();
            parameters.Smoothing = 0.7f;
            parameters.Correction = 0.3f;
            parameters.Prediction = 0.4f;
            parameters.JitterRadius = 1.0f;
            parameters.MaxDeviationRadius = 0.5f;
            nui.SkeletonEngine.SmoothParameters = parameters;

            try
            {
                nui.VideoStream.Open(ImageStreamType.Video, 2, ImageResolution.Resolution640x480, ImageType.Color);
                nui.DepthStream.Open(ImageStreamType.Depth, 2, ImageResolution.Resolution320x240, ImageType.DepthAndPlayerIndex);
            }
            catch (InvalidOperationException)
            {
                System.Windows.MessageBox.Show("Failed to open stream. Please make sure to specify a supported image type and resolution.");
                return;
            }

            lastTime = DateTime.Now;

            nui.DepthFrameReady += new EventHandler<ImageFrameReadyEventArgs>(nui_DepthFrameReady);
            nui.SkeletonFrameReady += new EventHandler<SkeletonFrameReadyEventArgs>(nui_SkeletonFrameReady);
            nui.VideoFrameReady += new EventHandler<ImageFrameReadyEventArgs>(nui_ColorFrameReady);

            speak("come on!");

        }

        // Converts a 16-bit grayscale depth frame which includes player indexes into a 32-bit frame
        // that displays different players in different colors
        byte[] convertDepthFrame(byte[] depthFrame16)
        {
            for (int i16 = 0, i32 = 0; i16 < depthFrame16.Length && i32 < depthFrame32.Length; i16 += 2, i32 += 4)
            {
                int player = depthFrame16[i16] & 0x07;
                int realDepth = (depthFrame16[i16 + 1] << 5) | (depthFrame16[i16] >> 3);
                // transform 13-bit depth information into an 8-bit intensity appropriate
                // for display (we disregard information in most significant bit)
                byte intensity = (byte)(255 - (255 * realDepth / 0x0fff));

                depthFrame32[i32 + RED_IDX] = 0;
                depthFrame32[i32 + GREEN_IDX] = 0;
                depthFrame32[i32 + BLUE_IDX] = 0;

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
            }
            return depthFrame32;
        }

        void nui_DepthFrameReady(object sender, ImageFrameReadyEventArgs e)
        {
            PlanarImage Image = e.ImageFrame.Image;
            byte[] convertedDepthFrame = convertDepthFrame(Image.Bits);

            depth.Source = BitmapSource.Create(
                Image.Width, Image.Height, 96, 96, PixelFormats.Bgr32, null, convertedDepthFrame, Image.Width * 4);

            ++totalFrames;

            DateTime cur = DateTime.Now;
            if (cur.Subtract(lastTime) > TimeSpan.FromSeconds(1))
            {
                int frameDiff = totalFrames - lastFrames;
                lastFrames = totalFrames;
                lastTime = cur;
                frameRate.Text = frameDiff.ToString() + " fps";
            }
        }

        private Point getDisplayPosition(Joint joint)
        {
            float depthX, depthY;
            nui.SkeletonEngine.SkeletonToDepthImage(joint.Position, out depthX, out depthY);
            depthX = depthX * 320; //convert to 320, 240 space
            depthY = depthY * 240; //convert to 320, 240 space
            int colorX, colorY;
            ImageViewArea iv = new ImageViewArea();
            // only ImageResolution.Resolution640x480 is supported at this point
            nui.NuiCamera.GetColorPixelCoordinatesFromDepthPixel(ImageResolution.Resolution640x480, iv, (int)depthX, (int)depthY, (short)0, out colorX, out colorY);

            // map back to skeleton.Width & skeleton.Height
            return new Point((int)(skeleton.Width * colorX / 640.0), (int)(skeleton.Height * colorY / 480));
        }

        Polyline getBodySegment(Microsoft.Research.Kinect.Nui.JointsCollection joints, Brush brush, params JointID[] ids)
        {
            PointCollection points = new PointCollection(ids.Length);
            for (int i = 0; i < ids.Length; ++i)
            {
                points.Add(getDisplayPosition(joints[ids[i]]));
            }

            Polyline polyline = new Polyline();
            polyline.Points = points;
            polyline.Stroke = brush;
            polyline.StrokeThickness = 5;
            return polyline;
        }

        bool handsUp = false;
        DateTime lastHandsUp = DateTime.MinValue;

        bool isTracking = false;
        DateTime lastTracking = DateTime.MinValue;
        DateTime lastNotTracking = DateTime.Now;

        void nui_SkeletonFrameReady(object sender, SkeletonFrameReadyEventArgs e)
        {
            SkeletonFrame skeletonFrame = e.SkeletonFrame;
            int iSkeleton = 0;
            int iSkeletonsTracked = 0;
            Brush[] brushes = new Brush[6];
            brushes[0] = new SolidColorBrush(Color.FromRgb(255, 0, 0));
            brushes[1] = new SolidColorBrush(Color.FromRgb(0, 255, 0));
            brushes[2] = new SolidColorBrush(Color.FromRgb(64, 255, 255));
            brushes[3] = new SolidColorBrush(Color.FromRgb(255, 255, 64));
            brushes[4] = new SolidColorBrush(Color.FromRgb(255, 64, 255));
            brushes[5] = new SolidColorBrush(Color.FromRgb(128, 128, 255));

            skeleton.Children.Clear();
            foreach (SkeletonData data in skeletonFrame.Skeletons)
            {
                if (SkeletonTrackingState.Tracked == data.TrackingState)
                {
                    double targetX = data.Position.X;
                    double targetY = data.Position.Y;
                    double targetZ = data.Position.Z;

                    switch (trackWhatComboBox.SelectedValue.ToString())
                    {
                        case "WristLeft":
                            targetX = data.Joints[JointID.WristLeft].Position.X;
                            targetY = data.Joints[JointID.WristLeft].Position.Y;
                            targetZ = data.Joints[JointID.WristLeft].Position.Z;
                            break;

                        case "WristRight":
                            targetX = data.Joints[JointID.WristRight].Position.X;
                            targetY = data.Joints[JointID.WristRight].Position.Y;
                            targetZ = data.Joints[JointID.WristRight].Position.Z;
                            break;

                        case "FootLeft":
                            targetX = data.Joints[JointID.FootLeft].Position.X;
                            targetY = data.Joints[JointID.FootLeft].Position.Y;
                            targetZ = data.Joints[JointID.FootLeft].Position.Z;
                            break;

                        case "FootRight":
                            targetX = data.Joints[JointID.FootRight].Position.X;
                            targetY = data.Joints[JointID.FootRight].Position.Y;
                            targetZ = data.Joints[JointID.FootRight].Position.Z;
                            break;

                        default:    // GC
                            break;
                    }



                    double panAngle = Math.Atan(targetX / targetZ) * 180.0d / Math.PI;
                    double tiltAngle = Math.Atan(targetY / targetZ) * 180.0d / Math.PI;

                    if (enableKinectCheckBox.IsChecked.HasValue && enableKinectCheckBox.IsChecked.Value)
                    {
                        this.Dispatcher.Invoke(new UpdateLabelDelegate(updatePmValuesLabel), string.Format("X: {0:0.000}\r\nY: {1:0.000}\r\nZ: {2:0.000}\r\nPan: {3:0}\r\nTilt: {4:0}", targetX, targetY, targetZ, panAngle, tiltAngle));

                        setPan(panAngle);
                        setTilt(tiltAngle);
                    }

                    // Draw bones
                    Brush brush = brushes[iSkeleton % brushes.Length];
                    skeleton.Children.Add(getBodySegment(data.Joints, brush, JointID.HipCenter, JointID.Spine, JointID.ShoulderCenter, JointID.Head));
                    skeleton.Children.Add(getBodySegment(data.Joints, brush, JointID.ShoulderCenter, JointID.ShoulderLeft, JointID.ElbowLeft, JointID.WristLeft, JointID.HandLeft));
                    skeleton.Children.Add(getBodySegment(data.Joints, brush, JointID.ShoulderCenter, JointID.ShoulderRight, JointID.ElbowRight, JointID.WristRight, JointID.HandRight));
                    skeleton.Children.Add(getBodySegment(data.Joints, brush, JointID.HipCenter, JointID.HipLeft, JointID.KneeLeft, JointID.AnkleLeft, JointID.FootLeft));
                    skeleton.Children.Add(getBodySegment(data.Joints, brush, JointID.HipCenter, JointID.HipRight, JointID.KneeRight, JointID.AnkleRight, JointID.FootRight));

                    if (data.Joints[JointID.WristLeft].Position.Y > data.Joints[JointID.Head].Position.Y && data.Joints[JointID.WristRight].Position.Y > data.Joints[JointID.Head].Position.Y)
                    {
                        if (!handsUp && (DateTime.Now - lastHandsUp).TotalSeconds > 1.0d)
                        {
                            speak();
                            lastHandsUp = DateTime.Now;
                            handsUp = true;
                        }
                        else
                        {
                            handsUp = false;
                        }
                    }

                    // Draw joints
                    foreach (Joint joint in data.Joints)
                    {
                        Point jointPos = getDisplayPosition(joint);
                        Line jointLine = new Line();
                        jointLine.X1 = jointPos.X - 3;
                        jointLine.X2 = jointLine.X1 + 6;
                        jointLine.Y1 = jointLine.Y2 = jointPos.Y;
                        jointLine.Stroke = jointColors[joint.ID];
                        jointLine.StrokeThickness = 6;
                        skeleton.Children.Add(jointLine);
                    }

                    iSkeletonsTracked++;
                }
                iSkeleton++;
            } // for each skeleton

            if (iSkeletonsTracked > 0)
            {
                if (!isTracking && (DateTime.Now - lastTracking).TotalSeconds > 2.0d)
                {
                    speak("hello beautiful!");
                    lastTracking = DateTime.Now;
                    isTracking = true;
                }
            }
            else
            {
                if ((DateTime.Now - lastNotTracking).TotalSeconds > 5.0d)
                {
                    speak("come out, come out, wherever you are!");
                    lastNotTracking = DateTime.Now;

                    if (_picpxmod != null && deviceAttached)
                    {
                        _picpxmod.SafePosture();
                    }
                }
                isTracking = false;
            }
        }

        void nui_ColorFrameReady(object sender, ImageFrameReadyEventArgs e)
        {
            // 32-bit per pixel, RGBA image
            PlanarImage Image = e.ImageFrame.Image;
            video.Source = BitmapSource.Create(
                Image.Width, Image.Height, 96, 96, PixelFormats.Bgr32, null, Image.Bits, Image.Width * Image.BytesPerPixel);
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            nui.Uninitialize();
            //Environment.Exit(0);
        }

        private void panFactorScrollBar_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            try
            {
                PanTiltAlignment.getInstance().panFactor = panFactorScrollBar.Value;
                panFactorLabel.Content = string.Format("{0,5:0}", PanTiltAlignment.getInstance().panFactor);
                setPan(currentPan);
            } catch { };
        }

        private void tiltFactorScrollBar_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            try
            {
                PanTiltAlignment.getInstance().tiltFactor = tiltFactorScrollBar.Value;
                tiltFactorLabel.Content = string.Format("{0,5:0}", PanTiltAlignment.getInstance().tiltFactor);
                setTilt(currentTilt);
            }
            catch { };
        }

        private void panAlignScrollBar_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            try
            {
                PanTiltAlignment.getInstance().panAlign = panAlignScrollBar.Value;
                panAlignLabel.Content = string.Format("{0,5:0}", PanTiltAlignment.getInstance().panAlign);
                setPan(currentPan);
            }
            catch { };
        }

        private void tiltAlignScrollBar_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            try
            {
                PanTiltAlignment.getInstance().tiltAlign = tiltAlignScrollBar.Value;
                tiltAlignLabel.Content = string.Format("{0,5:0}", PanTiltAlignment.getInstance().tiltAlign);
                setTilt(currentTilt);
            }
            catch { };
        }

        SpeechSynthesizer synthesizer = new SpeechSynthesizer();

        private void speak(string tosay = null)
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
            synthesizer.Rate = 0;     // -10...10

            // Synchronous
            //synthesizer.Speak("Hi Speak something ");

            // Asynchronous
            synthesizer.SpeakAsync(tosay);
        }

        private void saveAlignmentButton_Click(object sender, RoutedEventArgs e)
        {
            PanTiltAlignment.Save();
        }


        private void shootButton_Click(object sender, RoutedEventArgs e)
        {
            //_picpxmod.ServoPositionSet(99, 1);

            shootButton.Content = "Boom!";

            dispatcherTimer.Interval = new TimeSpan(0, 0, 3);
            dispatcherTimer.Start();
        }

        private void dispatcherTimer_Tick(object sender, EventArgs e)
        {
            dispatcherTimer.Stop();

            shootButton.Content = "Shoot";

            //_picpxmod.ServoPositionSet(99, 0);
        }

        private void trackWhatComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //panAlignLabel.Content = trackWhatComboBox.SelectedValue.ToString();
        }

    }
}
