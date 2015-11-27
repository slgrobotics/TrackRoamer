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

        private static double[] SafePostures = new double[] { 1.0d, -25.0d, 25.0d, -28.0d, 1.0d, -29.0d, -29.0d, -29.0d, 1.0d };

        private static double[] ZeroPostures = new double[] { 1.0d, 1.0d, 1.0d, 1.0d, 1.0d, 1.0d, 1.0d, 1.0d, 1.0d };   // can't have 0

        private static double[] HandUpPostures = new double[] { 1.0d, 29.0d, -29.0d, 29.0d, 1.0d, -29.0d, -29.0d, -29.0d, -29.0d };

        private static double[] HandDownPostures = new double[] { 1.0d, -29.0d, 29.0d, 29.0d, 1.0d, -29.0d, -29.0d, -29.0d, 29.0d };

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
            brickConnector.Open("COM5", 115200);  // shoulder
        }

        private void Window_Loaded(object sender, EventArgs e)
        {
            // UI is up at this moment.

            _panTiltAlignment = PanTiltAlignment.RestoreOrDefault();

            OpenBrickConnector();

            AssumePostureScrollbars(SafePostures);

            speak("Ready for action!");
        }

        void MainWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            Debug.WriteLine("IP: MainWindow_Closing");

            SafePosture();

            brickConnector.Close();

            Debug.WriteLine("OK: MainWindow_Closing done");
        }

        private void Window_Closed(object sender, EventArgs e)
        {
        }

        #endregion // MainWindow lifecycle

        #region Helpers

        void SafePosture()
        {
            AssumePosture(SafePostures);
        }

        /// <summary>
        /// can be called when UI is not up
        /// </summary>
        void AssumePosture(double[] postureValues)
        {
            brickConnector.setShoulderPan(postureValues[0]);
            brickConnector.setShoulderTilt(postureValues[1]);
            brickConnector.setShoulderTurn(postureValues[2]);
            brickConnector.setElbowAngle(postureValues[3]);
            brickConnector.setThumb(postureValues[4]);
            brickConnector.setIndexFinger(postureValues[5]);
            brickConnector.setMiddleFinger(postureValues[6]);
            brickConnector.setPinky(postureValues[7]);
            brickConnector.setWristTurn(postureValues[8]);
        }

        /// <summary>
        /// call from UI thread when UI is up.
        /// Will set scrollbars and that will call their respective "*_ValueChanged()" handlers
        /// </summary>
        void AssumePostureScrollbars(double[] postureValues)
        {
            panScrollBar.Value = postureValues[0];
            tiltScrollBar.Value = postureValues[1];
            turnScrollBar.Value = postureValues[2];
            elbowScrollBar.Value = postureValues[3];
            thumbScrollBar.Value = postureValues[4];
            indexFingerScrollBar.Value = postureValues[5];
            middleFingerScrollBar.Value = postureValues[6];
            pinkyScrollBar.Value = postureValues[7];
            wristTurnScrollBar.Value = postureValues[8];
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

        private void SafePostureButton_Click(object sender, RoutedEventArgs e)
        {
            AssumePostureScrollbars(SafePostures);
        }

        #endregion // Controls actions - button clicks etc

        private void HandUpButton_Click(object sender, RoutedEventArgs e)
        {
            AssumePostureScrollbars(HandUpPostures);
        }

        private void HandDownButton_Click(object sender, RoutedEventArgs e)
        {
            AssumePostureScrollbars(HandDownPostures);
        }

        private void ZeroButton_Click(object sender, RoutedEventArgs e)
        {
            AssumePostureScrollbars(ZeroPostures);
        }
    }
}
