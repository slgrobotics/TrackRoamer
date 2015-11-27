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

using Trackroamer.Library.LibAnimatronics;

namespace Trackroamer.Robotics.AnimatronicHead
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        PanTiltAlignment _panTiltAlignment;

        IBrickConnector brickConnector;

        Animations animations = new Animations();
        AnimationCombo animationCombo = new AnimationCombo();

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
            brickConnector.Open("COM12");
        }

        private void Window_Loaded(object sender, EventArgs e)
        {
            _panTiltAlignment = PanTiltAlignment.RestoreOrDefault();

            OpenBrickConnector();

            SafePosture();

            this.ListAnimCombos.ItemsSource = animationCombo.Keys;
            this.ListAnim.ItemsSource = animations.Keys;

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

        void updateJawMksLabel(string txt)
        {
            jawMksLabel.Content = txt;
        }

        void updateScaleLabel(string txt)
        {
            scaleLabel.Content = txt;
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

        #region Servo and gun controls

        private void setPan(double degreesFromCenter)
        {
            this.brickConnector.setPan(degreesFromCenter);

            this.Dispatcher.Invoke(new UpdateLabelDelegate(updatePmValuesLabel), string.Format("Pan: {0:0}\r\nTilt: {1:0}", this.brickConnector.currentPan, this.brickConnector.currentTilt));
            this.Dispatcher.Invoke(new UpdateLabelDelegate(updatePanMksLabel), string.Format("{0,4} mks {1,4:0} degrees", this.brickConnector.panMksLast, this.brickConnector.currentPan));
        }

        private void setTilt(double degreesFromCenter)
        {
            this.brickConnector.setTilt(degreesFromCenter);

            this.Dispatcher.Invoke(new UpdateLabelDelegate(updatePmValuesLabel), string.Format("Pan: {0:0}\r\nTilt: {1:0}", this.brickConnector.currentPan, this.brickConnector.currentTilt));
            this.Dispatcher.Invoke(new UpdateLabelDelegate(updateTiltMksLabel), string.Format("{0,4} mks {1,4:0} degrees", this.brickConnector.tiltMksLast, this.brickConnector.currentTilt));
        }

        private void setJaw(double degreesFromCenter)
        {
            this.brickConnector.setJaw(degreesFromCenter);

            this.Dispatcher.Invoke(new UpdateLabelDelegate(updatePmValuesLabel), string.Format("Pan: {0:0}\r\nTilt: {1:0}", this.brickConnector.currentPan, this.brickConnector.currentTilt));
            this.Dispatcher.Invoke(new UpdateLabelDelegate(updateJawMksLabel), string.Format("{0,4} mks {1,4:0} degrees", this.brickConnector.jawMksLast, this.brickConnector.currentJaw));
        }

        private void setScale(double scale)
        {
            this.Dispatcher.Invoke(new UpdateLabelDelegate(updateScaleLabel), string.Format("{0,4:0}%", scale));
        }

        void SafePosture()
        {
            brickConnector.setPanTilt(0.0d, 0.0d);
        }

        #endregion // Servo and gun controls

        #region Controls actions - button clicks etc

        private void panScrollBar_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            setPan(e.NewValue);
        }

        private void tiltScrollBar_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            setTilt(e.NewValue);
        }

        private void jawScrollBar_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            setJaw(e.NewValue);
        }

        private void scaleScrollBar_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            setScale(e.NewValue);
        }

        #endregion // Controls actions - button clicks etc

        private void ClearAnimationsButton_Click(object sender, RoutedEventArgs e)
        {
            brickConnector.clearAnimations();
        }

        private void DefaultAnimationsButton_Click(object sender, RoutedEventArgs e)
        {
            brickConnector.setDefaultAnimations();
        }

        private void AddAnimComboButton_Click(object sender, RoutedEventArgs e)
        {
            SetSelectedAnimationCombo();
        }

        private void SetAnimComboButton_Click(object sender, RoutedEventArgs e)
        {
            brickConnector.clearAnimations();
            SetSelectedAnimationCombo();
        }

        private void SetSelectedAnimationCombo()
        {
            string comboName = "" + ListAnimCombos.SelectedValue;
            double scale = scaleScrollBar.Value / 100.0d;
            bool doRepeat = RepeatCheckBox.IsChecked.GetValueOrDefault();

            if (!string.IsNullOrEmpty(comboName))
            {
                string[] animNames = animationCombo[comboName];
                setAnimationCombo(animNames, scale, doRepeat);
            }
        }

        private void AddAnimButton_Click(object sender, RoutedEventArgs e)
        {
            SetSelectedAnimations();
        }

        private void SetAnimButton_Click(object sender, RoutedEventArgs e)
        {
            brickConnector.clearAnimations();
            SetSelectedAnimations();
        }

        private void SetSelectedAnimations()
        {
            var selected = ListAnim.SelectedItems;

            foreach (var item in selected)
            {
                string name = "" + item;

                if (!string.IsNullOrEmpty(name))
                {
                    Animation anim = animations[name];

                    brickConnector.setAnimation(anim, scaleScrollBar.Value / 100.0d, RepeatCheckBox.IsChecked.GetValueOrDefault());
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

        // http://mcadams.posc.mu.edu/ike.htm
        private string speech0 = @"Today, the solitary inventor, tinkering in his shop,
has been overshadowed by task forces of scientists in laboratories and testing fields.
In the same fashion, the free university, historically the fountainhead of free ideas and scientific discovery,
has experienced a revolution in the conduct of research. Partly because of the huge costs involved,
a government contract becomes virtually a substitute for intellectual curiosity.
For every old black board there are now hundreds of new electronic computers...";

        private string speech1 = @"the three laws of robo-tics formulated by isaac asimov are:
1 - a robot may not injure a human being or, through inaction, allow a human being to come to harm.
2 - a robot must obey the orders given to it by human beings, except where such orders would conflict with the first law.
3 - a robot must protect its own existence as long as such protection does not conflict with the first or second laws.
in later books, a zero law was introduced:
0 - a robot may not harm humanity, or, by inaction, allow humanity to come to harm.";

        private string speech2 = @"My own opinion is enough for me, and I claim the right to have it defended against any consensus,
any majority, anywhere, any place, any time. And anyone who disagrees with this can pick a number, get in line, and kiss my behind.";
        
        private void speech1Button_Click(object sender, RoutedEventArgs e)
        {
            MessageTextBox.Text = speech0;
        }

        private void speech2Button_Click(object sender, RoutedEventArgs e)
        {
            MessageTextBox.Text = speech1;
        }

        private void speech3Button_Click(object sender, RoutedEventArgs e)
        {
            MessageTextBox.Text = speech2;
        }

        private void SpeakButton_Click(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(MessageTextBox.Text))
            {
                speak(MessageTextBox.Text);
            }
            else
            {
                speak(speech0);
            }
        }

        private void GenerateAnimComboButton_Click(object sender, RoutedEventArgs e)
        {
            var selected = ListAnim.SelectedItems;
            StringBuilder sb = new StringBuilder();

            sb.AppendLine("\r\n            this.Add(\"your_name_here\", new string[] {");

            foreach (var item in selected)
            {
                sb.AppendLine("                 \"" + item + "\",");
            }

            sb.AppendLine("             });");

            Debug.WriteLine(sb.ToString());

            MessageTextBox.Text = sb.ToString();
        }
    }
}
