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
using System.Threading;

using TrackRoamer.Robotics.Utility.LibPicSensors;
using TrackRoamer.Robotics.LibMapping;
using TrackRoamer.Robotics.LibBehavior;
using TrackRoamer.Robotics.Utility.LibSystem;
using TrackRoamer.Robotics.LibGuiWpf;

namespace TrackRoamer.Robotics.WpfProximityModuleDemo
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public ProximityModule _picpxmod = null;

        GeoPosition robotPositionDefault = new GeoPosition(-117.0d, 34.0d);
        Direction robotDirectionDefault = new Direction() { heading = -10.0d, bearing = -20.0d };

        // maintain a local mapper and pass it to UI when necessary:
        private MapperVicinity mapperVicinity = new MapperVicinity();
        private RoutePlanner routePlanner;
        protected double robotCornerDistanceMeters = 0.0d;   // to account for robot dimensions when adding measurements from corner-located proximity sensors.

        public MainWindow()
        {
            InitializeComponent();

            routePlanner = new RoutePlanner(mapperVicinity);

            this.Closing += new System.ComponentModel.CancelEventHandler(MainWindow_Closing);

            //create picpxmod device.
            _picpxmod = new ProximityModule(0x0925, 0x7001);    // see PIC Firmware - usb_descriptors.c lines 178,179

            _picpxmod.HasReadFrameEvent += pmFrameCompleteHandler;

            _picpxmod.DeviceAttachedEvent += new EventHandler<ProximityModuleEventArgs>(_picpxmod_DeviceAttachedEvent);
            _picpxmod.DeviceDetachedEvent += new EventHandler<ProximityModuleEventArgs>(_picpxmod_DeviceDetachedEvent);

            bool deviceAttached = _picpxmod.FindTheHid();

            pmValuesLabel.Content = deviceAttached ?
                "Proximity Board Found" : string.Format("Proximity Board NOT Found\r\nYour USB Device\r\nmust have:\r\n vendorId=0x{0:X}\r\nproductId=0x{1:X}", _picpxmod.vendorId, _picpxmod.productId);

            mapperVicinity.robotPosition = (GeoPosition)robotPositionDefault.Clone();

            mapperVicinity.robotDirection = (Direction)robotDirectionDefault.Clone();

            // we will need this later:
            robotCornerDistanceMeters = Math.Sqrt(mapperVicinity.robotState.robotLengthMeters * mapperVicinity.robotState.robotLengthMeters + mapperVicinity.robotState.robotWidthMeters * mapperVicinity.robotState.robotWidthMeters) / 2.0d;

            // --------- debug ------------
            GeoPosition pos1 = (GeoPosition)mapperVicinity.robotPosition.Clone();

            //pos1.translate(new Distance(1.0d), new Distance(1.0d));     // geo coordinates - East North

            pos1.translate(new Direction() { heading = mapperVicinity.robotDirection.heading, bearingRelative = 45.0d }, new Distance(1.0d));     // robot coordinates - forward right

            DetectedObstacle dobst1 = new DetectedObstacle(pos1) { color = Colors.Red };

            mapperVicinity.Add(dobst1);

            GeoPosition pos2 = (GeoPosition)mapperVicinity.robotPosition.Clone();

            //pos2.translate(new Distance(-1.0d), new Distance(1.0d));     // geo coordinates - West North

            pos2.translate(new Direction() { heading = mapperVicinity.robotDirection.heading, bearingRelative = -45.0d }, new Distance(1.0d));     // robot coordinates - forward left

            DetectedObstacle dobst2 = new DetectedObstacle(pos2) { color = Colors.Yellow };

            //mapperVicinity.Add(dobst2);

            mapperVicinity.computeMapPositions();
            // --------- end debug ------------

        }

        void MainWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (mapWindow != null)
            {
                mapWindow.Close();
                mapWindow = null;
            }
        }

        delegate void UpdateLabelDelegate(string txt);

        void updatePmValuesLabel(string txt)
        {
            pmValuesLabel.Content = txt;
        }

        void _picpxmod_DeviceDetachedEvent(object sender, ProximityModuleEventArgs e)
        {
            this.Dispatcher.Invoke(new UpdateLabelDelegate(updatePmValuesLabel), "USB: Device Detached");
        }

        void _picpxmod_DeviceAttachedEvent(object sender, ProximityModuleEventArgs e)
        {
            this.Dispatcher.Invoke(new UpdateLabelDelegate(updatePmValuesLabel), "USB: Device Attached");
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

        private void WebsiteLink_Click(object sender, RoutedEventArgs e)
        {
            // open URL:
            Hyperlink source = sender as Hyperlink;

            if (source != null)
            {
                System.Diagnostics.Process.Start(source.NavigateUri.ToString());
            }
        }

        // =================================================================================================================

        private ProximityBoard board = new ProximityBoard();

        private const int TEST_SAMPLES_COUNT = 100;
        private int inTestSamples = TEST_SAMPLES_COUNT;

        private void ResetSweepCollector()
        {
            board = new ProximityBoard();

            inTestSamples = TEST_SAMPLES_COUNT;
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

        private long lastCompassSliderTimestamp = 0L;
        private double lastCompassSlider = -1.0d;

        private void pmFrameCompleteHandler_UI(object sender, AsyncInputFrameArgs aira)
        {
            long timestamp = aira.timestamp;

            // have raw data displayed in a text box:

            StringBuilder frameValue = new StringBuilder();

            frameValue.AppendFormat("{0}   POT: {1}\r\n", aira.dPos1Mks, aira.sensorsState.potValue);
            frameValue.AppendFormat("{0}\r\n", aira.dPos2Mks);
            frameValue.AppendFormat("{0}  ", aira.dPing1DistanceM * 1000.0d);   // print distance in milimeters
            frameValue.AppendFormat("{0}\r\n", aira.fromPingScanStop);
            frameValue.AppendFormat("{0}\r\n", aira.dPing2DistanceM * 1000.0d);

            // expect 7 on "not in sight", 3 on "far", 1 on "medium" and 0 on "close":
            frameValue.AppendFormat("{0} ", aira.sensorsState.irbE1);
            frameValue.AppendFormat("{0} ", aira.sensorsState.irbE2);
            frameValue.AppendFormat("{0} ", aira.sensorsState.irbE3);
            frameValue.AppendFormat("{0} ", aira.sensorsState.irbE4);

            frameValue.AppendFormat("{0} ", aira.sensorsState.irbO1);
            frameValue.AppendFormat("{0} ", aira.sensorsState.irbO2);
            frameValue.AppendFormat("{0} ", aira.sensorsState.irbO3);
            frameValue.AppendFormat("{0}\r\n", aira.sensorsState.irbO4);

            frameValue.AppendFormat("{0} : {1:0.0} * {2:0.0} * {3:0.0} * {4:0.0} \r\n", aira.sensorsState.parkingSensorsCount,
                aira.sensorsState.parkingSensorMetersLB, aira.sensorsState.parkingSensorMetersLF, aira.sensorsState.parkingSensorMetersRF, aira.sensorsState.parkingSensorMetersRB);

            /*
            Tracer.Trace(string.Format("=== xpsi: {0} : {1} {2} {3} {4} : {5} {6} {7} {8}", aira.sensorsState.parkingSensorsCount,
                aira.sensorsState.parkingSensors[0], aira.sensorsState.parkingSensors[1], aira.sensorsState.parkingSensors[2], aira.sensorsState.parkingSensors[3],
                aira.sensorsState.parkingSensors[4], aira.sensorsState.parkingSensors[5], aira.sensorsState.parkingSensors[6], aira.sensorsState.parkingSensors[7]));

            Tracer.Trace(string.Format("=== bpsi: {0} : {1} {2} {3} {4} : {5} {6} {7} {8}", aira.sensorsState.parkingSensorsCount,
                Convert.ToString(aira.sensorsState.parkingSensors[0],2), Convert.ToString(aira.sensorsState.parkingSensors[1],2), Convert.ToString(aira.sensorsState.parkingSensors[2],2), Convert.ToString(aira.sensorsState.parkingSensors[3],2),
                Convert.ToString(aira.sensorsState.parkingSensors[4],2), Convert.ToString(aira.sensorsState.parkingSensors[5],2), Convert.ToString(aira.sensorsState.parkingSensors[6],2), Convert.ToString(aira.sensorsState.parkingSensors[7],2)));
            */

            //frameValue.AppendFormat("{0} {1} {2} {3}\r\n", aira.sensorsState.parkingSensors[4], aira.sensorsState.parkingSensors[5], aira.sensorsState.parkingSensors[6], aira.sensorsState.parkingSensors[7]);

            pmValuesLabel.Content = frameValue.ToString();

            // Proximity sensors display their data in dedicated control:

            lastProxData = new ProximityData() { TimeStamp = timestamp };

            lastProxData.setProximityData(aira.sensorsState.irbE1, aira.sensorsState.irbE2, aira.sensorsState.irbE3, aira.sensorsState.irbE4,
                                        aira.sensorsState.irbO1, aira.sensorsState.irbO2, aira.sensorsState.irbO3, aira.sensorsState.irbO4);

            lastProxDataChanged = true;

            // accelerometer has its own control for displaying data:

            lastAccData = new AccelerometerDataWpf() { TimeStamp = timestamp };

            lastAccData.setAccelerometerData(aira.sensorsState.accelX, aira.sensorsState.accelY, aira.sensorsState.accelZ);

            lastAccDataChanged = true;

            // compass has its own control for displaying data:

            if (aira.sensorsState.compassHeading >= 0.0d && aira.sensorsState.compassHeading <= 359.999999d && (lastDirData == null || Math.Abs(lastDirData.heading - aira.sensorsState.compassHeading) > 2.0d))
            {
                lastDirData = new DirectionData() { heading = aira.sensorsState.compassHeading, TimeStamp = timestamp };

                lastDirDataChanged = true;
            }
            else if ((bool)compassCheckBox.IsChecked && (lastCompassSlider != compassSlider.Value || timestamp > lastCompassSliderTimestamp + 20000000L))
            {
                // simulated direction, controlled by a slider:

                lastCompassSliderTimestamp = timestamp;
                lastCompassSlider = compassSlider.Value;

                double hdg = compassSlider.Value;   // -180 ... 180
                if (hdg < 0)
                {
                    hdg += 360.0d;      // 0...360
                }

                lastDirData = new DirectionData() { heading = hdg, TimeStamp = timestamp };

                lastDirDataChanged = true;
            }

            if (aira.sensorsState.parkingSensorsValid)
            {
                lastPsiData = new ParkingSensorData()
                {
                    parkingSensorMetersLB = aira.sensorsState.parkingSensorMetersLB,
                    parkingSensorMetersLF = aira.sensorsState.parkingSensorMetersLF,
                    parkingSensorMetersRB = aira.sensorsState.parkingSensorMetersRB,
                    parkingSensorMetersRF = aira.sensorsState.parkingSensorMetersRF,
                    TimeStamp = timestamp
                };

                lastPsiDataChanged = true;
            }

            // frames that are marked "" feed the sonar sweep view controls. They are updated in real time:

            if (aira.fromPingScanStop)
            {
                int angleRaw1 = (int)aira.dPos1Mks;
                double distM1 = aira.dPing1DistanceM;

                int angleRaw2 = (int)aira.dPos2Mks;
                double distM2 = aira.dPing2DistanceM;


                if (inTestSamples > 0)
                {
                    // prerun - for a while just try figuring out what comes in - sweep angle ranges for both sides

                    inTestSamples--;

                    if (inTestSamples < TEST_SAMPLES_COUNT - 10)        // first few frames are garbled
                    {
                        board.registerAnglesRaw(angleRaw1, angleRaw2);

                        if (inTestSamples == 0)     // last count
                        {
                            board.finishPrerun();

                            Tracer.Trace(board.ToString());
                        }
                    }
                }
                else
                {
                    // real sonar data is coming, and we finished prerun and know how many beams per sweep we are dealing with

                    lastRangeReadingSet = new RangeReadingSet() { left = board.angleRawToSonarAngle(new RangeReading(angleRaw1, distM1, timestamp)), right = board.angleRawToSonarAngle(new RangeReading(angleRaw2, distM2, timestamp)) };

                    lastRangeReadingSetChanged = true;

                    lastRangeReadingSet.combo1 = board.angleRawToSonarAngleCombo(1, angleRaw1, distM1, timestamp);
                    lastRangeReadingSet.combo2 = board.angleRawToSonarAngleCombo(2, angleRaw2, distM2, timestamp);
                    lastRangeReadingSet.hasCombo = true;

                    if (angleRaw1 == board.rawAngle1_Min || angleRaw1 == board.rawAngle1_Max)
                    {
                        lastRangeReadingSet.sweepFrameReady = true;
                    }
                }
            }

            // sanity check:
            if (lastRangeReadingSetChanged && lastRangeReadingSet.sweepFrameReady && lastDirData == null)
            {
                // we probably don't have compass connected - have a fake dir data for now.
                lastDirData = new DirectionData() { heading = 0.0d, TimeStamp = timestamp };
                lastDirDataChanged = true;
            }

            // update all controls with collected data:

            setSonarViewControls(timestamp);

            updateMapperVicinity(timestamp, sweepViewControlCombo.sonarData);

            lastRangeReadingSetChanged = false;

            setViewControls(timestamp);
        }

        // we keep all processed frame data here, so that View Controls could be updated at reasonable rate and in one setViewControls() method:
        private long lastViewControlsUpdatedTimestamp = 0L;
        private long viewControlsUpdateIntervalTicks = 1000L * 10000L; 

        private ProximityData lastProxData = null;
        private bool lastProxDataChanged = false;

        private AccelerometerDataWpf lastAccData = null;
        private bool lastAccDataChanged = false;

        private DirectionData lastDirData = null;
        private bool lastDirDataChanged = false;

        private ParkingSensorData lastPsiData = null;
        private bool lastPsiDataChanged = false;

        class RangeReadingSet
        {
            public RangeReading left;
            public RangeReading right;
            public bool hasCombo = false;
            public RangeReading combo1;
            public RangeReading combo2;
            public bool sweepFrameReady = false;
        }

        private RangeReadingSet lastRangeReadingSet = null;
        private bool lastRangeReadingSetChanged = false;

        /// <summary>
        /// update map with the detected objects
        /// </summary>
        /// <param name="timestamp"></param>
        private void updateMapperVicinity(long timestamp, SonarData p)
        {
            if (lastRangeReadingSetChanged && lastRangeReadingSet.sweepFrameReady && p.angles.Keys.Count == 26)
            {
                RangeReading rrFirst = p.getLatestReadingAt(p.angles.Keys.First());
                RangeReading rrLast = p.getLatestReadingAt(p.angles.Keys.Last());

                double sweepAngle = rrFirst.angleDegrees - rrLast.angleDegrees;
                double forwardAngle = sweepAngle / 2.0d;

                foreach (int angle in p.angles.Keys)
                {
                    RangeReading rr = p.getLatestReadingAt(angle);

                    double rangeMeters = rr.rangeMeters + robotCornerDistanceMeters;      // sensor is on the corner, adjust for that
                    double relBearing = rr.angleDegrees - forwardAngle;

                    GeoPosition pos1 = (GeoPosition)mapperVicinity.robotPosition.Clone();

                    pos1.translate(new Direction() { heading = mapperVicinity.robotDirection.heading, bearingRelative = relBearing }, new Distance(rangeMeters));

                    DetectedObstacle dobst1 = new DetectedObstacle(pos1) {
                        geoPosition = pos1,
                        firstSeen = timestamp,
                        lastSeen = timestamp,
                        color = Colors.Red,
                        detectorType = DetectorType.SONAR_SCANNING,
                        objectType = DetectedObjectType.Obstacle 
                    };

                    mapperVicinity.Add(dobst1);
                }

                // make sure we have the latest heading info in the mapper, and map positions have been computed:
 
                Direction dir = new Direction() { heading = lastDirData.heading, bearing = mapperVicinity.robotDirection.bearing, TimeStamp = timestamp };

                mapperVicinity.robotDirection = dir;        // will call mapperVicinity.computeMapPositions();

                updateMapWindow();
            }

            mapperTraceLabel.Content = "" + mapperVicinity.detectedObjects.Count + " objects";
        }

        private void updateMapWindow()
        {
            if (mapWindow != null)
            {
                mapWindow.planRoute();    // will also redraw the map
            }
        }

        /// <summary>
        /// update all View Controls at reasonable rate
        /// </summary>
        /// <param name="timestamp"></param>
        private void setSonarViewControls(long timestamp)
        {
            // sonar sweep controls are displayed in real time, as the data arrives at the rate of 26 readings per second: 
            if (lastRangeReadingSetChanged)
            {
                sweepViewControlLeft.CurrentValue = lastRangeReadingSet.left;
                sweepViewControlRight.CurrentValue = lastRangeReadingSet.right;

                if (lastRangeReadingSet.hasCombo)
                {
                    sweepViewControlCombo.CurrentValue = lastRangeReadingSet.combo1;
                    sweepViewControlCombo.CurrentValue = lastRangeReadingSet.combo2;

                    /*
                    if (lastRangeReadingSet.sweepFrameReady)
                    {
                        Tracer.Trace("Sweep Frame Ready");

                        SonarData p = sweepViewControlCombo.sonarData;

                        foreach (int angle in p.angles.Keys)
                        {
                            RangeReading rr = p.getLatestReadingAt(angle);
                            double range = rr.rangeMeters * 1000.0d;        // millimeters
                            //ranges.Add(range);
                            Tracer.Trace("&&&&&&&&&&&&&&&&&&&&&&&&&&&& angle=" + angle + " range=" + range);
                            /*
                             * typical measurement:
                             * for dual radar:
                                angles: 26
                                Sweep Frame Ready
                                angle=883 range=2052
                                angle=982 range=2047
                                angle=1081 range=394
                                angle=1179 range=394
                                angle=1278 range=398
                                angle=1377 range=390
                                angle=1475 range=390
                                angle=1574 range=390
                                angle=1673 range=399
                                angle=1771 range=416
                                angle=1870 range=1972
                                angle=1969 range=2182
                                angle=2067 range=3802
                                angle=2166 range=245
                                angle=2265 range=241
                                angle=2364 range=224
                                angle=2462 range=211
                                angle=2561 range=202
                                angle=2660 range=202
                                angle=2758 range=135
                                angle=2857 range=135
                                angle=2956 range=135
                                angle=3054 range=228
                                angle=3153 range=254
                                angle=3252 range=248
                                angle=3350 range=244
                            * /
                        }
                    }
                    */
                }
            }
        }

        private void setViewControls(long timestamp)
        {
            // non-sonar data is displayed at our discretion, at time interval or real time:
            viewControlsUpdateIntervalTicks = (long)updateDelaySlider.Value * 10000L;

            if (timestamp - lastViewControlsUpdatedTimestamp > viewControlsUpdateIntervalTicks)
            {
                lastViewControlsUpdatedTimestamp = timestamp;

                if (lastProxDataChanged)
                {
                    lastProxDataChanged = false;

                    robotProximityViewControl1.CurrentValue = lastProxData;
                }

                if (lastAccDataChanged)
                {
                    lastAccDataChanged = false;

                    lastAccData.computeVectors();

                    robotOrientationViewControl1.CurrentValue = lastAccData;
                }

                if (lastDirDataChanged)
                {
                    lastDirDataChanged = false;

                    if (mapperVicinity.robotDirection.bearing.HasValue)
                    {
                        lastDirData.bearing = (double)mapperVicinity.robotDirection.bearing;
                    }

                    robotDirectionViewControl1.CurrentDirection = lastDirData;

                    if (mapWindow != null)
                    {
                        Direction dir = new Direction() { heading = lastDirData.heading, TimeStamp = timestamp };

                        mapWindow.mapperViewControl1.CurrentDirection = dir;
                    }
                }

                if (lastPsiDataChanged)
                {
                    lastPsiDataChanged = false;

                    robotParkingSensorViewControl1.CurrentValue = lastPsiData;
                }
            }
        }

        private void pmSetDefaultSweep()
        {
            _picpxmod.ServoSweepParams(1, ProximityBoard.sweepMin, ProximityBoard.sweepMax, ProximityBoard.sweepStartPos1, ProximityBoard.sweepStep, ProximityBoard.initialDirectionUp, ProximityBoard.sweepRate);
            _picpxmod.ServoSweepParams(2, ProximityBoard.sweepMin, ProximityBoard.sweepMax, ProximityBoard.sweepStartPos2, ProximityBoard.sweepStep, ProximityBoard.initialDirectionUp, ProximityBoard.sweepRate);
        }

        private void pmSetDefaultSweepButton_Click(object sender, RoutedEventArgs e)
        {
            pmSetDefaultSweep();
        }

        private void pmSafePostureButton_Click(object sender, RoutedEventArgs e)
        {
            _picpxmod.SafePosture();
        }

        private void pmServoSweepStartButton_Click(object sender, RoutedEventArgs e)
        {
            //pmSonarViewControl1.Reset();
            _picpxmod.ServoSweepEnable(true);
        }

        private void pmServoSweepStopButton_Click(object sender, RoutedEventArgs e)
        {
            _picpxmod.ServoSweepEnable(false);
        }

        private void pmDataContinuousStartButton_Click(object sender, RoutedEventArgs e)
        {
            ResetSweepCollector();
            _picpxmod.DataContinuousStart();
        }

        private void pmDataContinuousStopButton_Click(object sender, RoutedEventArgs e)
        {
            _picpxmod.DataContinuousStop();
        }

        private MapWindow mapWindow = null;

        private void mapButton_Click(object sender, RoutedEventArgs e)
        {
            if (mapWindow == null)
            {
                mapButton.IsEnabled = false;

                mapWindow = new MapWindow();
                mapWindow.Closing += new System.ComponentModel.CancelEventHandler(mapWindow_Closing);
                mapWindow.Show();

                // we do it after "Show" to ensure that we override default mapper and that the Draw is enabled:
                mapWindow.mapperViewControl1.CurrentMapper = mapperVicinity;
                mapWindow.mapperViewControl1.CurrentRoutePlanner = routePlanner;
            }
        }

        void mapWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            mapButton.IsEnabled = true;
            mapWindow = null;
        }

        private void compassSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if ((bool)compassCheckBox.IsChecked)
            {
                //Slider slider = (Slider)sender;

                double newHeading = e.NewValue;

                Direction dir = new Direction() { heading = newHeading, bearing = mapperVicinity.robotDirection.bearing, TimeStamp = DateTime.Now.Ticks };

                mapperVicinity.robotDirection = dir;        // will call mapperVicinity.computeMapPositions();

                robotDirectionViewControl1.CurrentDirection = new DirectionData() { bearing = (double)dir.bearing, heading = (double)dir.heading, TimeStamp = dir.TimeStamp };

                updateMapWindow();
            }
        }

        private void bearingTextBox_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                double newBearing;

                if (double.TryParse(bearingTextBox.Text, out newBearing))
                {
                    newBearing = Direction.to360(newBearing);

                    Direction dir = new Direction() { heading = mapperVicinity.robotDirection.heading, bearing = newBearing, TimeStamp = DateTime.Now.Ticks };

                    mapperVicinity.robotDirection = dir;        // will call mapperVicinity.computeMapPositions();

                    robotDirectionViewControl1.CurrentDirection = new DirectionData() { bearing = (double)dir.bearing, heading = (double)dir.heading, TimeStamp = dir.TimeStamp };

                    updateMapWindow();
                }
            }
        }

        // =================================================================================================================
    }
}
