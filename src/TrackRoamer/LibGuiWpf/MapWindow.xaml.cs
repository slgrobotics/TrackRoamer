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
using System.Windows.Shapes;

using TrackRoamer.Robotics.LibMapping;

namespace TrackRoamer.Robotics.LibGuiWpf
{
    /// <summary>
    /// Interaction logic for MapWindow.xaml
    /// </summary>
    public partial class MapWindow : Window
    {
        public MapWindow()
        {
            InitializeComponent();
        }

        private GeoPosition robotPositionDefault = new GeoPosition(-117.0d, 34.0d);     // will be overwritten by invoking code
        private Direction robotDirectionDefault = new Direction() { heading = 0.0d };   // will be overwritten by invoking code

        private void mapperViewControl1_Loaded(object sender, RoutedEventArgs e)
        {
            mapperViewControl1.setRobotPositionAndDirection(robotPositionDefault, robotDirectionDefault);   // will be overwritten by invoking code

            manualControlCheckBox.IsChecked = true;
        }

        public void setBearingText(string bearing)
        {
            if (!bearingTextBox.IsKeyboardFocused)
            {
                bearingTextBox.Text = bearing;
            }
        }

        private void bearingTextBox_KeyUp(object sender, KeyEventArgs e)
        {
            double newBearing;

            if (e.Key == Key.Enter)
            {
                MapperVicinity _mapperVicinity = mapperViewControl1.CurrentMapper;

                if(_mapperVicinity != null && _mapperVicinity.robotDirection != null && _mapperVicinity.robotDirection.heading.HasValue && double.TryParse(bearingTextBox.Text, out newBearing))
                {
                    newBearing = Direction.to360(newBearing);

                    Direction dir = new Direction() { heading = _mapperVicinity.robotDirection.heading, bearing = newBearing, TimeStamp = DateTime.Now.Ticks };

                    _mapperVicinity.robotDirection = dir;        // will call mapperVicinity.computeMapPositions();

                    mapperViewControl1.RedrawMap();
                }
            }
        }

        /// <summary>
        /// Sets the Current Direction value
        /// </summary>
        public Direction CurrentDirection
        {
            set
            { 
                // _mapperVicinity.robotDirection already set elsewhere, just redraw the pointer.
                if (this.IsLoaded)
                {
                    mapperViewControl1.CurrentDirection = value;
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
                if (this.IsLoaded)
                {
                    comboBoxTactics.SelectedValue = value;
                }
            }
        }

        /// <summary>
        /// Sets the StatusString value
        /// </summary>
        public string StatusString
        {
            set
            {
                if (this.IsLoaded)
                {
                    mapperViewControl1.StatusString = value;
                }
            }
        }

        #region the Ignore checkboxes handlers

        private void ignoreGpsCheckBox_CheckedUnchecked(object sender, RoutedEventArgs e)
        {
            MapperVicinity _mapperVicinity = mapperViewControl1.CurrentMapper;

            if (_mapperVicinity != null)
            {
                _mapperVicinity.robotState.ignoreGps = (bool)ignoreGpsCheckBox.IsChecked;
            }
        }

        private void ignoreAhrsCheckBox_CheckedUnchecked(object sender, RoutedEventArgs e)
        {
            MapperVicinity _mapperVicinity = mapperViewControl1.CurrentMapper;

            if (_mapperVicinity != null)
            {
                _mapperVicinity.robotState.ignoreAhrs = (bool)ignoreAhrsCheckBox.IsChecked;
            }
        }

        private void ignoreLaserCheckBox_CheckedUnchecked(object sender, RoutedEventArgs e)
        {
            MapperVicinity _mapperVicinity = mapperViewControl1.CurrentMapper;

            if (_mapperVicinity != null)
            {
                _mapperVicinity.robotState.ignoreLaser = (bool)ignoreLaserCheckBox.IsChecked;
            }
        }

        private void ignoreProximityCheckBox_CheckedUnchecked(object sender, RoutedEventArgs e)
        {
            MapperVicinity _mapperVicinity = mapperViewControl1.CurrentMapper;

            if (_mapperVicinity != null)
            {
                _mapperVicinity.robotState.ignoreProximity = (bool)ignoreProximityCheckBox.IsChecked;
            }
        }

        private void ignoreParkingSensorCheckBox_CheckedUnchecked(object sender, RoutedEventArgs e)
        {
            MapperVicinity _mapperVicinity = mapperViewControl1.CurrentMapper;

            if (_mapperVicinity != null)
            {
                _mapperVicinity.robotState.ignoreParkingSensor = (bool)ignoreParkingSensorCheckBox.IsChecked;
            }
        }

        private void ignoreKinectSoundsCheckBox_CheckedUnchecked(object sender, RoutedEventArgs e)
        {
            MapperVicinity _mapperVicinity = mapperViewControl1.CurrentMapper;

            if (_mapperVicinity != null)
            {
                _mapperVicinity.robotState.ignoreKinectSounds = (bool)ignoreKinectSoundsCheckBox.IsChecked;
            }
        }

        private void ignoreRedShirtCheckBox_CheckedUnchecked(object sender, RoutedEventArgs e)
        {
            MapperVicinity _mapperVicinity = mapperViewControl1.CurrentMapper;

            if (_mapperVicinity != null)
            {
                _mapperVicinity.robotState.ignoreRedShirt = (bool)ignoreRedShirtCheckBox.IsChecked;
            }
        }

        private void ignoreKinectSkeletonsCheckBox_CheckedUnchecked(object sender, RoutedEventArgs e)
        {
            MapperVicinity _mapperVicinity = mapperViewControl1.CurrentMapper;

            if (_mapperVicinity != null)
            {
                _mapperVicinity.robotState.ignoreKinectSkeletons = (bool)ignoreKinectSkeletonsCheckBox.IsChecked;
            }
        }

        private void doLostTargetRoutineCheckBox_CheckedUnchecked(object sender, RoutedEventArgs e)
        {
            MapperVicinity _mapperVicinity = mapperViewControl1.CurrentMapper;

            if (_mapperVicinity != null)
            {
                _mapperVicinity.robotState.doLostTargetRoutine = (bool)doLostTargetRoutineCheckBox.IsChecked;
            }
        }

        private void doPhotosCheckBox_CheckedUnchecked(object sender, RoutedEventArgs e)
        {
            MapperVicinity _mapperVicinity = mapperViewControl1.CurrentMapper;

            if (_mapperVicinity != null)
            {
                _mapperVicinity.robotState.doPhotos = (bool)doPhotosCheckBox.IsChecked;
            }
        }

        private void doVicinityPlanningCheckBox_CheckedUnchecked(object sender, RoutedEventArgs e)
        {
            MapperVicinity _mapperVicinity = mapperViewControl1.CurrentMapper;

            if (_mapperVicinity != null)
            {
                _mapperVicinity.robotState.doVicinityPlanning = (bool)doVicinityPlanningCheckBox.IsChecked;
            }
        }

        #endregion // the Ignore checkboxes handlers

        private void manualControlCheckBox_CheckedUnchecked(object sender, RoutedEventArgs e)
        {
            MapperVicinity _mapperVicinity = mapperViewControl1.CurrentMapper;

            if (_mapperVicinity != null)
            {
                _mapperVicinity.robotState.manualControl = (bool)manualControlCheckBox.IsChecked;

                TurnState currentTurnState = _mapperVicinity.turnState;
                currentTurnState.directionDesired = null;
            }
        }

        private void manualControlButtonStop_Click(object sender, RoutedEventArgs e)
        {
            MapperVicinity _mapperVicinity = mapperViewControl1.CurrentMapper;

            if (_mapperVicinity != null)
            {
                _mapperVicinity.robotState.manualControlCommand = "stop";
                _mapperVicinity.robotState.manualControl = true;
           }
        }

        private void manualControlButtonTurn_Click(object sender, RoutedEventArgs e)
        {
            MapperVicinity _mapperVicinity = mapperViewControl1.CurrentMapper;

            if (_mapperVicinity != null)
            {
                _mapperVicinity.robotState.manualControlCommand = "turn";
            }
        }

        private void comboBoxStrategy_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            MapperVicinity _mapperVicinity = mapperViewControl1.CurrentMapper;

            if (_mapperVicinity != null)
            {
                _mapperVicinity.robotState.robotStrategyType = (RobotStrategyType)comboBoxStrategy.SelectedValue;
            }
        }

        #region Keeping aspect ratio of the window during resize

        private bool isSizeChangeDefered = false;

        private void Window_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            // Keep Acpect Ratio - see http://stackoverflow.com/questions/1755329/keep-aspect-ratio-of-usercontrol
            const double factor = 1.0d;

            if (isSizeChangeDefered)
                return;

            isSizeChangeDefered = true;
            try
            {
                if (e.WidthChanged)
                {
                    this.Height = e.NewSize.Width * factor - 145; // toolPanel2.Width;
                }
                if (e.HeightChanged)
                {
                    this.Width = e.NewSize.Height / factor + 145; // toolPanel2.Width;
                }
            }
            finally
            {
                // e.Handled = true;
                isSizeChangeDefered = false;
            }
        }

        private bool isPseudoMaximized = false;

        private void Window_StateChanged(object sender, EventArgs e)
        {
            if (this.WindowState == System.Windows.WindowState.Maximized)
            {
                this.WindowState = System.Windows.WindowState.Normal;

                if (isPseudoMaximized)
                {
                    isPseudoMaximized = false;
                    this.Height = Math.Max(780.0d, SystemParameters.PrimaryScreenHeight / 4.0d);
                    this.Top = SystemParameters.PrimaryScreenHeight / 8.0d;
                }
                else
                {
                    this.Height = SystemParameters.PrimaryScreenHeight - 80.0d;
                    this.Top = 5.0d;
                    isPseudoMaximized = true;
                }
            }
        }

        #endregion // Keeping aspect ratio of the window during resize

        //private void comboBoxTactics_SelectionChanged(object sender, SelectionChangedEventArgs e)
        //{
        //    MapperVicinity _mapperVicinity = mapperViewControl1.CurrentMapper;

        //    if (_mapperVicinity != null)
        //    {
        //        _mapperVicinity.robotState.robotTacticsType = (RobotTacticsType)comboBoxTactics.SelectedValue;
        //    }
        //}
    }
}
