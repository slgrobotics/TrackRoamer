using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
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
using System.Threading;

using TrackRoamer.Robotics.Utility.LibPicSensors;
using TrackRoamer.Robotics.LibMapping;
using TrackRoamer.Robotics.LibBehavior;
using TrackRoamer.Robotics.Utility.LibSystem;

namespace TrackRoamer.Robotics.LibGuiWpf
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow(PIDController _followDirectionPidControllerAngularSpeed, PIDController _followDirectionPidControllerLinearSpeed, string soundsBasePath = null)
        {
            followDirectionPidControllerAngularSpeed = _followDirectionPidControllerAngularSpeed;
            followDirectionPidControllerLinearSpeed = _followDirectionPidControllerLinearSpeed;

            soundsBasePathDefault = soundsBasePath;

            InitializeComponent();

            CreateMapWindow();

            soundsBasePathLabel.Content = "Sound files folder: " + soundsBasePathDefault;

            PopulatePlaySoundCombo();

            this.Closing += new System.ComponentModel.CancelEventHandler(MainWindow_Closing);

        }

        void MainWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (mapWindow != null)
            {
                mapWindow.Close();
                mapWindow = null;
            }
        }

        #region Playing sounds using MediaPlayer

        public void PlayRandomSound()
        {
            Dispatcher.Invoke(DispatcherPriority.Normal, (Action)(() =>
            {
                playRandomSound();
            }));
        }

        public void PlaySound(string filename, double volume, string soundsBasePath = null)
        {
            Dispatcher.Invoke(DispatcherPriority.Normal, (Action)(() =>
            {
                playSound(filename, volume, soundsBasePath);
            }));
        }

        private void playRandomSound()
        {
            if (SoundFiles != null && SoundFiles.Any())
            {
                int index = random.Next(SoundFiles.Count);
                PlaySoundComboBox.SelectedIndex = index;
                string filename = SoundFiles[index];
                playSound(filename); //"Cow_Moo-Mike_Koenig-42670858.mp3");
            }
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
                    PlayRandomSoundButton.IsEnabled = false;
                    PlaySoundComboBox.IsEnabled = false;
                    soundsBasePathLabel.Content += "     (not found)";
                }
            }
        }

        private void PlaySoundButton_Click(object sender, RoutedEventArgs e)
        {
            string filename = "" + PlaySoundComboBox.SelectedValue;
            playSound(filename); //"Cow_Moo-Mike_Koenig-42670858.mp3");
        }

        private void PlayRandomSoundButton_Click(object sender, RoutedEventArgs e)
        {
            playRandomSound();
        }

        #endregion // Playing sounds using MediaPlayer

        /// <summary>
        /// Sets the Current LaserData value
        /// </summary>
        public LaserDataSerializable CurrentLaserData
        {
            set
            {
                if (this.IsLoaded)
                {
                    sweepViewControlCombo.CurrentLaserData = value;
                }
            }
        }

        /// <summary>
        /// Sets the Current Attitude value
        /// </summary>
        public OrientationData CurrentAttitude
        {
            set
            {
                if (this.IsLoaded)
                {
                    robotOrientationViewControl1.CurrentAttitude = value;
                }
            }
        }

        /// <summary>
        /// Sets the Current Accelerometer value
        /// </summary>
        public AccelerometerData CurrentAccelerometer
        {
            set
            {
                if (this.IsLoaded)
                {
                    AccelerometerDataWpf adw = new AccelerometerDataWpf() { accX = value.accX, accY = value.accY, accZ = value.accZ, TimeStamp = value.TimeStamp };
                    adw.computeVectors();
                    robotOrientationViewControl1.CurrentValue = adw;
                }
            }
        }

        /// <summary>
        /// Sets the Current Direction value
        /// </summary>
        public DirectionData CurrentDirection
        {
            set
            {
                if (this.IsLoaded)
                {
                    robotDirectionViewControl1.CurrentDirection = value;
                    if (mapWindow != null)
                    {
                        // _mapperVicinity.robotDirection already set, just redraw the pointer.
                        mapWindow.CurrentDirection = new Direction() { heading = value.heading, bearing = value.bearing, TimeStamp = value.TimeStamp };
                    }
                }
            }
        }

        /// <summary>
        /// Sets the Current Proximity value
        /// </summary>
        public ProximityData CurrentProximity
        {
            set
            {
                if (this.IsLoaded)
                {
                    robotProximityViewControl1.CurrentValue = value;
                }
            }
        }

        /// <summary>
        /// Sets the Current ParkingSensor value
        /// </summary>
        public ParkingSensorData CurrentParkingSensor
        {
            set
            {
                if (this.IsLoaded)
                {
                    robotParkingSensorViewControl1.CurrentValue = value;
                }
            }
        }

        /// <summary>
        /// Sets the Current Tactics value
        /// </summary>
        public RobotTacticsType CurrentTactics
        {
            set
            {
                if (this.IsLoaded && mapWindow != null)
                {
                    mapWindow.CurrentTactics = value;
                }
            }
        }

        private string _statusString;

        /// <summary>
        /// Sets the Status String value
        /// </summary>
        public string StatusString
        {
            set
            {
                _statusString = value;

                Dispatcher.Invoke(DispatcherPriority.Normal, (Action)(() =>
                {
                    setStatusString();
                }));

            }
        }

        private void setStatusString()
        {
            if (this.IsLoaded && mapWindow != null)
            {
                mapWindow.StatusString = _statusString;
            }
        }

        private MapWindow mapWindow = null;

        private void mapButton_Click(object sender, RoutedEventArgs e)
        {
            CreateMapWindow();
        }

        private void CreateMapWindow()
        {
            if (mapWindow == null)
            {
                mapButton.IsEnabled = false;

                mapWindow = new MapWindow();
                mapWindow.Closing += new System.ComponentModel.CancelEventHandler(mapWindow_Closing);
                mapWindow.Show();

                // have some mapper on the mapping window, until it is replaced by real one.
                // we do it after "Show" to ensure that we override default mapper and that the Draw is enabled:
                setMapper(new MapperVicinity(), null);
            }
        }

        private MapperVicinity _mapperVicinity = null;
        private RoutePlanner _routePlanner = null;

        public void setMapper(MapperVicinity mapper, RoutePlanner routePlanner)
        {
            if (mapWindow != null)
            {
                _mapperVicinity = mapper;
                _routePlanner = routePlanner;

                mapWindow.mapperViewControl1.CurrentMapper = mapper;
                mapWindow.mapperViewControl1.CurrentRoutePlanner = routePlanner;
            }
        }

        public void RedrawMap()
        {
            if (mapWindow != null)
            {
                if (_mapperVicinity != null)
                {
                    if (_mapperVicinity.robotDirection != null)
                    {
                        mapWindow.setBearingText(string.Format("{0:#}", _mapperVicinity.robotDirection.bearing));
                    }

                    if (_mapperVicinity.robotState != null)
                    {
                        mapWindow.manualControlCheckBox.IsChecked = _mapperVicinity.robotState.manualControl;
                        mapWindow.ignoreGpsCheckBox.IsChecked = _mapperVicinity.robotState.ignoreGps;
                        mapWindow.ignoreAhrsCheckBox.IsChecked = _mapperVicinity.robotState.ignoreAhrs;
                        mapWindow.ignoreLaserCheckBox.IsChecked = _mapperVicinity.robotState.ignoreLaser;
                        mapWindow.ignoreProximityCheckBox.IsChecked = _mapperVicinity.robotState.ignoreProximity;
                        mapWindow.ignoreParkingSensorCheckBox.IsChecked = _mapperVicinity.robotState.ignoreParkingSensor;
                    }
                }

                mapWindow.mapperViewControl1.RedrawMap();
            }
        }

        void mapWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            mapButton.IsEnabled = true;
            mapWindow = null;
        }

        private void bearingTextBox_KeyUp(object sender, KeyEventArgs e)
        {
            double newBearing;

            if (e.Key == Key.Enter && _mapperVicinity != null && _mapperVicinity.robotDirection != null && _mapperVicinity.robotDirection.heading.HasValue && double.TryParse(bearingTextBox.Text, out newBearing))
            {
                newBearing %= 360.0d;

                Direction dir = new Direction() { heading = _mapperVicinity.robotDirection.heading, bearing = newBearing, TimeStamp = DateTime.Now.Ticks };

                _mapperVicinity.robotDirection = dir;        // will call mapperVicinity.computeMapPositions();

                robotDirectionViewControl1.CurrentDirection = new DirectionData() { bearing = dir.bearing, heading = (double)dir.heading, TimeStamp = dir.TimeStamp };

                if (mapWindow != null)
                {
                    mapWindow.mapperViewControl1.RedrawMap();
                }
            }
        }

        #region PID Controllers controls related

        public PIDController followDirectionPidControllerAngularSpeed;
        public PIDController followDirectionPidControllerLinearSpeed;

        public event EventHandler PidControllersUpdated;

        private void SetPidControllersValues()
        {
            if (followDirectionPidControllerAngularSpeed != null)
            {
                textBoxAngularKp.Text = followDirectionPidControllerAngularSpeed.Kp.ToString();
                textBoxAngularKd.Text = followDirectionPidControllerAngularSpeed.Kd.ToString();
                textBoxAngularKi.Text = followDirectionPidControllerAngularSpeed.Ki.ToString();
                textBoxAngularMax.Text = followDirectionPidControllerAngularSpeed.MaxPidValue.ToString();
                textBoxAngularMin.Text = followDirectionPidControllerAngularSpeed.MinPidValue.ToString();
                textBoxAngularIntegralMax.Text = followDirectionPidControllerAngularSpeed.MaxIntegralError.ToString();
            }

            if (followDirectionPidControllerLinearSpeed != null)
            {
                textBoxLinearKp.Text = followDirectionPidControllerLinearSpeed.Kp.ToString();
                textBoxLinearKd.Text = followDirectionPidControllerLinearSpeed.Kd.ToString();
                textBoxLinearKi.Text = followDirectionPidControllerLinearSpeed.Ki.ToString();
                textBoxLinearMax.Text = followDirectionPidControllerLinearSpeed.MaxPidValue.ToString();
                textBoxLinearMin.Text = followDirectionPidControllerLinearSpeed.MinPidValue.ToString();
                textBoxLinearIntegralMax.Text = followDirectionPidControllerLinearSpeed.MaxIntegralError.ToString();
            }
        }

        private void buttonUpdatePidControllers_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (this.IsLoaded && followDirectionPidControllerAngularSpeed != null && followDirectionPidControllerLinearSpeed != null)
                {
                    followDirectionPidControllerAngularSpeed.Kp = double.Parse(textBoxAngularKp.Text);
                    followDirectionPidControllerAngularSpeed.Kd = double.Parse(textBoxAngularKd.Text);
                    followDirectionPidControllerAngularSpeed.Ki = double.Parse(textBoxAngularKi.Text);
                    followDirectionPidControllerAngularSpeed.MaxPidValue = double.Parse(textBoxAngularMax.Text);
                    followDirectionPidControllerAngularSpeed.MinPidValue = double.Parse(textBoxAngularMin.Text);
                    followDirectionPidControllerAngularSpeed.MaxIntegralError = double.Parse(textBoxAngularIntegralMax.Text);

                    followDirectionPidControllerLinearSpeed.Kp = double.Parse(textBoxLinearKp.Text);
                    followDirectionPidControllerLinearSpeed.Kd = double.Parse(textBoxLinearKd.Text);
                    followDirectionPidControllerLinearSpeed.Ki = double.Parse(textBoxLinearKi.Text);
                    followDirectionPidControllerLinearSpeed.MaxPidValue = double.Parse(textBoxLinearMax.Text);
                    followDirectionPidControllerLinearSpeed.MinPidValue = double.Parse(textBoxLinearMin.Text);
                    followDirectionPidControllerLinearSpeed.MaxIntegralError = double.Parse(textBoxLinearIntegralMax.Text);
                }

                if (PidControllersUpdated != null)
                {
                    // someone is subscribed, throw event
                    PidControllersUpdated(this, new EventArgs());
                }
            }
            catch
            {
            }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            SetPidControllersValues();
        }

        #endregion // PID Controllers controls related

        #region PowerScale slider control related

        public event EventHandler PowerScaleAdjusted;

        public double _powerScale;

        public double PowerScale
        {
            get
            {
                return sliderPowerScale.Value;
            }

            set
            {
                Dispatcher.Invoke(DispatcherPriority.Normal, (Action)(() =>
                {
                    _powerScale = value;

                    if (sliderPowerScale.IsLoaded)
                    {
                        sliderPowerScale.Value = value;
                    }
                }));
            }
        }

        private void sliderPowerScale_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            labelPowerScale.Content = string.Format("{0:#.00}", e.NewValue);

            if (PowerScaleAdjusted != null)
            {
                // someone is subscribed, throw event
                PowerScaleAdjusted(this, new EventArgs());
            }
        }

        private void sliderPowerScale_Loaded(object sender, RoutedEventArgs e)
        {
            sliderPowerScale.Value = _powerScale;
        }

        #endregion // PowerScale slider control related
    }
}
