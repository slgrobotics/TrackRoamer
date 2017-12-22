using System;
using System.Collections.Generic;
using System.Diagnostics;
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
using Trackroamer.Library.LibHandHardware;

namespace WpfHandTest
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        PanTiltAlignment _panTiltAlignment;

        IBrickConnector brickConnector;

        private static double?[] SafePosture = new double?[] { 1.0d, -25.0d, 25.0d, -28.0d, 1.0d, 1.0d, 1.0d, 1.0d, 1.0d };

        private static double?[] ZeroPosture = new double?[] { 1.0d, 1.0d, 1.0d, 1.0d, 1.0d, 1.0d, 1.0d, 1.0d, 1.0d };   // can't have 0

        private static double?[] HandUpPosture = new double?[] { 1.0d, 29.0d, -29.0d, 29.0d, null, null, null, null, -29.0d };

        private static double?[] HandDownPosture = new double?[] { 1.0d, -29.0d, 29.0d, 29.0d, null, null, null, null, 29.0d };

        private static double?[] HandForwardPosture = new double?[] { 19d, -1.0d, 1.0d, 20.0d, null, null, null, null, 36.0d };

        private static double?[] HandChestPosture = new double?[] { 19d, -1.0d, -3.0d, -22.0d, null, null, null, null, 36.0d };

        private static double?[] HandForwardGrabPreparePosture = new double?[] { -28d, -14.0d, 1.0d, 30.0d, 60, -60, -60, -60, 28.0d };

        private static double?[] HandForwardGrabPosture = new double?[] { -28d, -14.0d, 1.0d, 30.0d, -90.0d, 60.0d, 60.0d, 60.0d, 28.0d };

        private static double?[] GrabPosture = new double?[] { null, null, null, null, -90.0d, 60.0d, 60.0d, 60.0d, null };

        private static double?[] GrabReleasePosture = new double?[] { null, null, null, null, 60, -60, -60, -60, null };

        #region MainWindow lifecycle

        public MainWindow()
        {
            EnsureBrickConnector();

            InitializeComponent();

            this.Closing += new System.ComponentModel.CancelEventHandler(MainWindow_Closing);
        }

        private void EnsureBrickConnector()
        {
            brickConnector = new BrickConnectorArduino();
        }

        private void OpenBrickConnector()
        {
            //brickConnector.Open("COM6", 115200);    // hand
            brickConnector.Open("COM11", 115200);  // shoulder
        }

        private async void Window_Loaded(object sender, EventArgs e)
        {
            // UI is up at this moment.

            _panTiltAlignment = PanTiltAlignment.RestoreOrDefault();

            OpenBrickConnector();

            await AssumePostureScrollbars(SafePosture);

            speak("Ready for action!");
        }

        void MainWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            Debug.WriteLine("IP: MainWindow_Closing");

            AssumeSafePosture();

            brickConnector.Close();

            Debug.WriteLine("OK: MainWindow_Closing done");
        }

        private void Window_Closed(object sender, EventArgs e)
        {
        }

        #endregion // MainWindow lifecycle

        #region Helpers

        /// <summary>
        /// direct command, not via scrollbars
        /// </summary>
        void AssumeSafePosture()
        {
            AssumePosture(SafePosture);
        }

        /// <summary>
        /// direct command, not via scrollbars
        /// can be called when UI is not up
        /// </summary>
        private void AssumePosture(double?[] postureValues)
        {
            if (postureValues[8].HasValue) brickConnector.setWristTurn(postureValues[8].Value);
            if (postureValues[2].HasValue) brickConnector.setShoulderTurn(postureValues[2].Value);
            if (postureValues[1].HasValue) brickConnector.setShoulderTilt(postureValues[1].Value);
            if (postureValues[3].HasValue) brickConnector.setElbowAngle(postureValues[3].Value);
            if (postureValues[0].HasValue) brickConnector.setShoulderPan(postureValues[0].Value);
            if (postureValues[5].HasValue) brickConnector.setIndexFinger(postureValues[5].Value);
            if (postureValues[6].HasValue) brickConnector.setMiddleFinger(postureValues[6].Value);
            if (postureValues[7].HasValue) brickConnector.setPinky(postureValues[7].Value);
            if (postureValues[4].HasValue) brickConnector.setThumb(postureValues[4].Value);
        }

        /// <summary>
        /// call from UI thread when UI is up.
        /// Will set scrollbars and that will call their respective "*_ValueChanged()" handlers
        /// </summary>
        private async Task AssumePostureScrollbars(double?[] postureValues)
        {
            int waitIntervalMs = 300;

            if (postureValues[8].HasValue)
            {
                wristTurnScrollBar.Value = postureValues[8].Value;
                await Task.Delay(waitIntervalMs);
            }
            if (postureValues[2].HasValue)
            {
                turnScrollBar.Value = postureValues[2].Value;
                await Task.Delay(waitIntervalMs);
            }
            if (postureValues[1].HasValue)
            {
                tiltScrollBar.Value = postureValues[1].Value;
                await Task.Delay(waitIntervalMs);
            }
            if (postureValues[3].HasValue)
            {
                elbowScrollBar.Value = postureValues[3].Value;
                await Task.Delay(waitIntervalMs);
            }
            if (postureValues[0].HasValue)
            {
                panScrollBar.Value = postureValues[0].Value;
                await Task.Delay(waitIntervalMs);
            }
            if (postureValues[5].HasValue)
            {
                indexFingerScrollBar.Value = postureValues[5].Value;
                await Task.Delay(waitIntervalMs);
            }
            if (postureValues[6].HasValue)
            {
                middleFingerScrollBar.Value = postureValues[6].Value;
                await Task.Delay(waitIntervalMs);
            }
            if (postureValues[7].HasValue)
            {
                pinkyScrollBar.Value = postureValues[7].Value;
                await Task.Delay(waitIntervalMs);
            }
            if (postureValues[4].HasValue)
            {
                thumbScrollBar.Value = postureValues[4].Value;
                await Task.Delay(waitIntervalMs);
            }
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

        void updateTurnMksLabel(string txt)
        {
            turnMksLabel.Content = txt;
        }

        void updateElbowMksLabel(string txt)
        {
            elbowMksLabel.Content = txt;
        }

        void updateThumbMksLabel(string txt)
        {
            thumbMksLabel.Content = txt;
        }

        void updateIndexFingerMksLabel(string txt)
        {
            indexFingerMksLabel.Content = txt;
        }

        void updateMiddleFingerMksLabel(string txt)
        {
            middleFingerMksLabel.Content = txt;
        }

        void updatePinkyMksLabel(string txt)
        {
            pinkyMksLabel.Content = txt;
        }

        void updateWristTurnMksLabel(string txt)
        {
            wristTurnMksLabel.Content = txt;
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

            //this.Dispatcher.Invoke(new UpdateLabelDelegate(updatePmValuesLabel), stringBuilder.ToString());
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

        private void UpdatePmValues()
        {
            this.Dispatcher.Invoke(new UpdateLabelDelegate(updatePmValuesLabel),
                string.Format("Shoulder Pan: {0:0}\r\nShoulder Tilt: {1:0}\r\nShoulder Turn: {2:0}\r\nElbow Angle: {3:0}\r\nThumb: {4:0}\r\nIndex Finger: {5:0}\r\nMiddle Finger: {6:0}\r\nPinky: {7:0}\r\nWrist Turn: {8:0}",
                                this.brickConnector.currentShoulderPan, this.brickConnector.currentShoulderTilt, this.brickConnector.currentShoulderTurn, this.brickConnector.currentElbowAngle, this.brickConnector.currentThumb,
                                this.brickConnector.currentIndexFinger, this.brickConnector.currentMiddleFinger, this.brickConnector.currentPinky, this.brickConnector.currentWristTurn)
            );
        }

        #endregion // Helpers

        #region Brick actuators setters

        private void setShoulderPan(double degreesFromCenter)
        {
            this.brickConnector.setShoulderPan(degreesFromCenter);

            UpdatePmValues();
            this.Dispatcher.Invoke(new UpdateLabelDelegate(updatePanMksLabel), string.Format("{0,4} mks {1,4:0} degrees", this.brickConnector.shoulderPanMksLast, this.brickConnector.currentShoulderPan));
        }

        private void setShoulderTilt(double degreesFromCenter)
        {
            this.brickConnector.setShoulderTilt(degreesFromCenter);

            UpdatePmValues();
            this.Dispatcher.Invoke(new UpdateLabelDelegate(updateTiltMksLabel), string.Format("{0,4} mks {1,4:0} degrees", this.brickConnector.shoulderTiltMksLast, this.brickConnector.currentShoulderTilt));
        }

        private void setShoulderTurn(double degreesFromCenter)
        {
            this.brickConnector.setShoulderTurn(degreesFromCenter);

            UpdatePmValues();
            this.Dispatcher.Invoke(new UpdateLabelDelegate(updateTurnMksLabel), string.Format("{0,4} mks {1,4:0} degrees", this.brickConnector.shoulderTurnMksLast, this.brickConnector.currentShoulderTurn));
        }

        private void setElbowAngle(double degreesFromCenter)
        {
            this.brickConnector.setElbowAngle(degreesFromCenter);

            UpdatePmValues();
            this.Dispatcher.Invoke(new UpdateLabelDelegate(updateElbowMksLabel), string.Format("{0,4} mks {1,4:0} degrees", this.brickConnector.elbowAngleMksLast, this.brickConnector.currentElbowAngle));
        }

        private void setThumb(double degreesFromCenter)
        {
            this.brickConnector.setThumb(degreesFromCenter);

            UpdatePmValues();
            this.Dispatcher.Invoke(new UpdateLabelDelegate(updateThumbMksLabel), string.Format("{0,4} mks {1,4:0} degrees", this.brickConnector.thumbMksLast, this.brickConnector.currentThumb));
        }

        private void setIndexFinger(double degreesFromCenter)
        {
            this.brickConnector.setIndexFinger(degreesFromCenter);

            UpdatePmValues();
            this.Dispatcher.Invoke(new UpdateLabelDelegate(updateIndexFingerMksLabel), string.Format("{0,4} mks {1,4:0} degrees", this.brickConnector.indexFingerMksLast, this.brickConnector.currentIndexFinger));
        }

        private void setMiddleFinger(double degreesFromCenter)
        {
            this.brickConnector.setMiddleFinger(degreesFromCenter);

            UpdatePmValues();
            this.Dispatcher.Invoke(new UpdateLabelDelegate(updateMiddleFingerMksLabel), string.Format("{0,4} mks {1,4:0} degrees", this.brickConnector.middleFingerMksLast, this.brickConnector.currentMiddleFinger));
        }

        private void setPinky(double degreesFromCenter)
        {
            this.brickConnector.setPinky(degreesFromCenter);

            UpdatePmValues();
            this.Dispatcher.Invoke(new UpdateLabelDelegate(updatePinkyMksLabel), string.Format("{0,4} mks {1,4:0} degrees", this.brickConnector.pinkyMksLast, this.brickConnector.currentPinky));
        }

        private void setWristTurn(double degreesFromCenter)
        {
            this.brickConnector.setWristTurn(degreesFromCenter);

            UpdatePmValues();
            this.Dispatcher.Invoke(new UpdateLabelDelegate(updateWristTurnMksLabel), string.Format("{0,4} mks {1,4:0} degrees", this.brickConnector.wristTurnMksLast, this.brickConnector.currentWristTurn));
        }

        #endregion // Brick actuators setters

        #region Controls actions - button clicks etc

        private void panScrollBar_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            setShoulderPan(e.NewValue);
        }

        private void tiltScrollBar_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            setShoulderTilt(e.NewValue);
        }

        private void turnScrollBar_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            setShoulderTurn(e.NewValue);
        }

        private void elbowScrollBar_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            setElbowAngle(e.NewValue);
        }

        private void thumbScrollBar_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            setThumb(e.NewValue);
        }

        private void indexFingerScrollBar_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            setIndexFinger(e.NewValue);
        }

        private void middleFingerScrollBar_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            setMiddleFinger(e.NewValue);
        }

        private void pinkyScrollBar_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            setPinky(e.NewValue);
        }

        private void wristTurnScrollBar_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            setWristTurn(e.NewValue);
        }

        private async void SafePostureButton_Click(object sender, RoutedEventArgs e)
        {
            await AssumePostureScrollbars(SafePosture);
        }

        #endregion // Controls actions - button clicks etc

        private async void HandUpButton_Click(object sender, RoutedEventArgs e)
        {
            await AssumePostureScrollbars(HandUpPosture);
        }

        private async void HandDownButton_Click(object sender, RoutedEventArgs e)
        {
            await AssumePostureScrollbars(HandDownPosture);
        }

        private async void HandForwardButton_Click(object sender, RoutedEventArgs e)
        {
            await AssumePostureScrollbars(HandForwardPosture);
        }

        private async void HandChestButton_Click(object sender, RoutedEventArgs e)
        {
            await AssumePostureScrollbars(HandChestPosture);
        }

        private async void HandGrabPrepareButton_Click(object sender, RoutedEventArgs e)
        {
            await AssumePostureScrollbars(HandForwardGrabPreparePosture);
        }

        private async void HandForwardGrabButton_Click(object sender, RoutedEventArgs e)
        {
            await AssumePostureScrollbars(HandForwardGrabPosture);
        }

        private async void ZeroButton_Click(object sender, RoutedEventArgs e)
        {
            await AssumePostureScrollbars(ZeroPosture);
        }

        private async void AnimateGrabBottleButton_Click(object sender, RoutedEventArgs e)
        {
            int waitIntervalMs = 5000;
            int waitIntervalShortMs = 2500;

            GrabBottleButton.IsEnabled = false;

            Debug.WriteLine("GrabBottleButton_Click - Chest");
            speak("Chest");
            await AssumePostureScrollbars(HandChestPosture);
            await Task.Delay(waitIntervalMs);

            Debug.WriteLine("Grab Prepare");
            speak("Grab Prepare");
            await AssumePostureScrollbars(HandForwardGrabPreparePosture);
            await Task.Delay(waitIntervalShortMs);

            Debug.WriteLine("Grab");
            speak("Grab");
            await AssumePostureScrollbars(GrabPosture);
            await Task.Delay(waitIntervalShortMs);

            Debug.WriteLine("Chest again");
            speak("Chest again");
            await AssumePostureScrollbars(HandChestPosture);
            await Task.Delay(waitIntervalMs);

            Debug.WriteLine("Hand Forward - give");
            speak("Give");
            await AssumePostureScrollbars(HandForwardPosture);
            await Task.Delay(waitIntervalMs);

            speak("Release");
            await AssumePostureScrollbars(GrabReleasePosture);
            await Task.Delay(waitIntervalMs);
            Debug.WriteLine("Chest again finally");

            speak("Safe Posture");
            await AssumePostureScrollbars(SafePosture);
            await Task.Delay(waitIntervalMs);

            speak("Done");
            GrabBottleButton.IsEnabled = true;
        }
    }
}
