﻿using System;
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

using TrackRoamer.Robotics.LibMapping;
using TrackRoamer.Robotics.LibBehavior;

namespace TrackRoamer.Robotics.LibGuiWpf
{
    /// <summary>
    /// Interaction logic for MapperViewControl.xaml
    /// </summary>
    public partial class MapperViewControl : UserControl
    {
        // http://www.c-sharpcorner.com/UploadFile/raj1979/WPFRectangle08282008234151PM/WPFRectangle.aspx

        #region Dependency Methods - Direction

        /// <summary>
        /// Dependency property to Get/Set the Current Direction value 
        /// </summary>
        public static readonly DependencyProperty CurrentDirectionProperty =
            DependencyProperty.Register("CurrentDirection", typeof(Direction), typeof(MapperViewControl),
            new PropertyMetadata(null, new PropertyChangedCallback(MapperViewControl.OnCurrentDirectionPropertyChanged)));

        /// <summary>
        /// Gets/Sets the Current Direction value
        /// </summary>
        public Direction CurrentDirection
        {
            get
            {
                return (Direction)GetValue(CurrentDirectionProperty);
            }
            set
            {
                SetValue(CurrentDirectionProperty, value);
            }
        }

        private static void OnCurrentDirectionPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            //Get access to the instance of MapperViewControl whose property value changed
            MapperViewControl gauge = d as MapperViewControl;
            gauge.OnCurrentDirectionChanged(e);
        }

        public virtual void OnCurrentDirectionChanged(DependencyPropertyChangedEventArgs e)
        {
            Direction newValue = (Direction)e.NewValue;

            if (newValue != null && newValue.heading.HasValue)
            {
                MapperVicinity mv = CurrentMapper;

                mv.rotateTo((double)newValue.heading);

                CurrentMapper = mv;
            }
        }

        #endregion // Dependency Methods - Direction

        public RoutePlanner _currentRoutePlanner;

        public RoutePlanner CurrentRoutePlanner
        {
            get
            {
                return _currentRoutePlanner;
            }
            set
            {
                _currentRoutePlanner = value;
            }
        }

        #region Dependency Methods - Mapper

        /// <summary>
        /// Dependency property to Get/Set the current Mapper value 
        /// </summary>
        //public static readonly DependencyProperty CurrentMapperProperty =
        //    DependencyProperty.Register("CurrentMapper", typeof(MapperVicinity), typeof(MapperViewControl),
        //    new PropertyMetadata(null, new PropertyChangedCallback(MapperViewControl.OnCurrentMapperPropertyChanged)));

        private MapperVicinity _currentMapper;

        /// <summary>
        /// Gets/Sets the current value
        /// </summary>
        public MapperVicinity CurrentMapper
        {
            get
            {
                return _currentMapper; // (MapperVicinity)GetValue(CurrentMapperProperty);
            }
            set
            {
                //SetValue(CurrentMapperProperty, value);

                _currentMapper = value;

                RedrawMap();
            }
        }

        //private static void OnCurrentMapperPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        //{
        //    //Get access to the instance of MapperViewControl whose Mapper property value changed
        //    MapperViewControl gauge = d as MapperViewControl;
        //    gauge.OnCurrentMapperChanged(e);
        //}

        //public virtual void OnCurrentMapperChanged(DependencyPropertyChangedEventArgs e)
        //{
        //    MapperVicinity newValue = (MapperVicinity)e.NewValue;

        //    CurrentMapper = newValue;
        //}

        public string StatusString { private get; set; }

        #endregion // Dependency Methods - Mapper

        // a set of transforms to draw pointers in the right places; create them once:
        private ScaleTransform st = new ScaleTransform(1.0d, 1.0d);

        public MapperViewControl()
        {
            InitializeComponent();

            canvasRel.MouseMove += canvas_MouseMove;
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            CurrentMapper = new MapperVicinity();
        }

        private DateTime lastRedrawMap = DateTime.Now;
        private const double redrawIntervalMs = 200.0d;

        public void RedrawMap()
        {
            if ((DateTime.Now - lastRedrawMap).TotalMilliseconds > redrawIntervalMs)
            {
                lastRedrawMap = DateTime.Now;
                Dispatcher.Invoke(DispatcherPriority.Normal, (Action)(() =>
                {
                    drawAll();
                }));
            }
        }

        public void setRobotPositionAndDirection(GeoPosition pos, Direction dir)
        {
            MapperVicinity mapperVicinity = CurrentMapper;

            if (pos != null)
            {
                mapperVicinity.robotPosition = (GeoPosition)pos.Clone();
            }

            if (dir != null)
            {
                mapperVicinity.robotDirection = (Direction)dir.Clone();
            }

            // --------- debug ------------
            /*
            GeoPosition pos1 = (GeoPosition)pos.Clone();

            pos1.translate(new Distance(1.0d), new Distance(1.0d));     // robot coordinates - right forward

            DetectedObstacle dobst1 = new DetectedObstacle(pos1) { color = Colors.Red };

            mapperVicinity.AddDetectedObject(dobst1);

            GeoPosition pos2 = (GeoPosition)pos.Clone();

            pos2.translate(new Distance(-1.0d), new Distance(1.0d));     // robot coordinates - left forward

            DetectedObstacle dobst2 = new DetectedObstacle(pos2) { color = Colors.Yellow };

            mapperVicinity.AddDetectedObject(dobst2);

            mapperVicinity.computeMapPositions();
             */
            // --------- end debug ------------

            RedrawMap();
        }

        #region Mouse move routed event

        private double metersPerPixelX;
        private double metersPerPixelY;

        void canvas_MouseMove(object sender, MouseEventArgs mea)
        {
            try
            {
                Point pos = mea.GetPosition(canvasRel);

                // first get it in pixels, relative to center point:
                double x = pos.X - canvasRel.ActualWidth / 2.0d;
                double y = -(pos.Y - canvasRel.ActualHeight / 2.0d);

                // and now convert to meters, relative to robot center:
                x *= metersPerPixelX;
                y *= metersPerPixelY;

                MapperVicinity mapperVicinity = CurrentMapper;

                GeoPosition gp = new GeoPosition(mapperVicinity.robotPosition);

                gp.translate(new Distance(x), new Distance(y));

                RoutedEventArgsMouseMoved newEventArgs = new RoutedEventArgsMouseMoved(MapperViewControl.MouseMovedEvent) { xMeters = x, yMeters = y, geoPosition = gp };
                RaiseEvent(newEventArgs);
            }
            catch { }
        }

        // Create a custom routed event by first registering a RoutedEventID. This event uses the bubbling routing strategy
        public static readonly RoutedEvent MouseMovedEvent = EventManager.RegisterRoutedEvent(
            "MouseMoved", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(MapperViewControl));

        // Provide CLR accessors for the event
        public event RoutedEventHandler MouseMoved
        {
            add { AddHandler(MouseMovedEvent, value); }
            remove { RemoveHandler(MouseMovedEvent, value); }
        }


        #endregion // Mouse move routed event

        #region Draw cells, robot etc.

        private MapperVicinity mapperVicinity;
        private double relCellWidth;
        private double relCellHeight;

        private void drawAll()
        {
            if (this.IsLoaded)
            {
                List<UIElement> relList = new List<UIElement>();
                List<UIElement> geoList = new List<UIElement>();

                bool northUp = (bool)northUpCheckBox.IsChecked;

                List<UIElement> unrotatedList = northUp ? geoList : relList;

                mapperVicinity = CurrentMapper;

                labelLat.Content = mapperVicinity.robotPosition.Lat;
                labelLng.Content = mapperVicinity.robotPosition.Lng;

                relCellWidth = canvasRel.ActualWidth / mapperVicinity.RelMapWidth;
                relCellHeight = canvasRel.ActualHeight / mapperVicinity.RelMapHeight;

                // compose lists of objects to be drawn later:
                drawTurnState(geoList, unrotatedList);
                drawRelCells(relList);
                drawReferenceCircle(unrotatedList);
                drawGeoCells(geoList);
                drawDetectedObjects(relList, unrotatedList, 0);
                drawMission(relList, unrotatedList);
                drawStatusString(unrotatedList);

                // now clear canvases and draw the objects from the lists:
                canvasRel.Children.Clear();
                canvasGeo.Children.Clear();

                rotateGeoAndTurnstateCanvas(unrotatedList, northUp);

                foreach(UIElement uie in relList)
                {
                    canvasRel.Children.Add(uie);
                }

                foreach (UIElement uie in geoList)
                {
                    canvasGeo.Children.Add(uie);
                }

                // draw other elements on the map:
                drawRobot();

                drawHeadingBearingPointers();

                rotateToNorthUp(northUp);
            }
        }

        #region rotateGeoAndTurnstateCanvas()

        private void rotateToNorthUp(bool northUp)
        {
            MapperVicinity mapperVicinity = CurrentMapper;

            TransformGroup geort = null;
            TransformGroup robotrt = null;

            if (northUp)
            {
                double frameWidth = canvasRel.ActualWidth;
                double frameHeight = canvasRel.ActualHeight;

                double geoCellWidth = frameWidth / mapperVicinity.GeoMapWidth;
                double geoCellHeight = frameHeight / mapperVicinity.GeoMapHeight;

                geort = new TransformGroup();
                robotrt = new TransformGroup();

                double angle = 0.0d;    // degrees

                if (mapperVicinity.robotDirection != null && mapperVicinity.robotDirection.heading.HasValue)
                {
                    angle = (double)mapperVicinity.robotDirection.heading;
                }

                double arads = angle * Math.PI / 180.0d;
                double asin = Math.Abs(Math.Sin(arads));
                double acos = Math.Abs(Math.Cos(arads));

                double scaleY = (geoCellWidth * asin + geoCellHeight * acos) / (geoCellHeight * (asin + acos));
                double scaleX = (geoCellWidth * acos + geoCellHeight * asin) / (geoCellWidth * (asin + acos));

                RotateTransform rt = new RotateTransform(angle, 0.0d, 0.0d);

                TranslateTransform tt1 = new TranslateTransform(-canvasRel.ActualWidth / 2.0d, -canvasRel.ActualHeight / 2.0d);
                TranslateTransform tt2 = new TranslateTransform(canvasRel.ActualWidth / 2.0d, canvasRel.ActualHeight / 2.0d);

                ScaleTransform st = new ScaleTransform(scaleX, scaleY);

                geort.Children.Add(tt1);
                geort.Children.Add(st);
                geort.Children.Add(rt);
                geort.Children.Add(tt2);

                RotateTransform rrt = new RotateTransform(angle, 0.0d, 0.0d);

                TranslateTransform rtt1 = new TranslateTransform(-canvasRel.ActualWidth / 2.0d, -canvasRel.ActualHeight / 2.0d);
                TranslateTransform rtt2 = new TranslateTransform(canvasRel.ActualWidth / 2.0d, canvasRel.ActualHeight / 2.0d);

                robotrt.Children.Add(rtt1);
                robotrt.Children.Add(rrt);
                robotrt.Children.Add(rtt2);
            }

            canvasRel.RenderTransform = geort;
            robotPanel.RenderTransform = robotrt;
        }

        private void rotateGeoAndTurnstateCanvas(List<UIElement> unrotatedList, bool northUp)
        {
            MapperVicinity mapperVicinity = CurrentMapper;

            TransformGroup geort = null;

            double frameWidth = canvasRel.ActualWidth;
            double frameHeight = canvasRel.ActualHeight;

            double geoCellWidth = frameWidth / mapperVicinity.GeoMapWidth;
            double geoCellHeight = frameHeight / mapperVicinity.GeoMapHeight;

            // draw compass labels:
            double x0 = geoCellWidth + 6;
            double y0 = frameHeight / 2 - geoCellHeight * 1.5d;
            double cSize = 35;

            Label lN = new Label() { Content = "N", Margin = new Thickness(x0, y0 - cSize, 0, 0), FontSize = 20, FontWeight = FontWeights.Bold };
            unrotatedList.Add(lN);

            Label lS = new Label() { Content = "S", Margin = new Thickness(x0, y0 + cSize, 0, 0), FontSize = 20, FontWeight = FontWeights.Bold };
            unrotatedList.Add(lS);

            Label lW = new Label() { Content = "W", Margin = new Thickness(x0 - cSize, y0, 0, 0), FontSize = 20, FontWeight = FontWeights.Bold };
            unrotatedList.Add(lW);

            Label lE = new Label() { Content = "E", Margin = new Thickness(x0 + cSize, y0, 0, 0), FontSize = 20, FontWeight = FontWeights.Bold };
            unrotatedList.Add(lE);

            double angle = 0.0d;    // degrees

            if (mapperVicinity.robotDirection != null && mapperVicinity.robotDirection.heading.HasValue)
            {
                angle = -(double)mapperVicinity.robotDirection.heading;
            }

            string content = Math.Round(-angle).ToString();
            double shift = content.Length * 3.0d - 2.0d;
            Label lDir = new Label() { Content = content, Margin = new Thickness(x0 - shift, y0 + 2.0d, 0, 0), FontSize = 18, FontWeight = FontWeights.Bold };
            unrotatedList.Add(lDir);

            if (!northUp)
            {
                double arads = angle * Math.PI / 180.0d;
                double asin = Math.Abs(Math.Sin(arads));
                double acos = Math.Abs(Math.Cos(arads));

                double scaleY = (geoCellWidth * asin + geoCellHeight * acos) / (geoCellHeight * (asin + acos));
                double scaleX = (geoCellWidth * acos + geoCellHeight * asin) / (geoCellWidth * (asin + acos));

                geort = new TransformGroup();

                RotateTransform rt = new RotateTransform(angle, 0.0d, 0.0d);

                TranslateTransform tt1 = new TranslateTransform(-canvasRel.ActualWidth / 2.0d, -canvasRel.ActualHeight / 2.0d);
                TranslateTransform tt2 = new TranslateTransform(canvasRel.ActualWidth / 2.0d, canvasRel.ActualHeight / 2.0d);

                ScaleTransform st = new ScaleTransform(scaleX, scaleY);

                geort.Children.Add(tt1);
                geort.Children.Add(st);
                geort.Children.Add(rt);
                geort.Children.Add(tt2);
            }

            canvasGeo.RenderTransform = geort;
            turnPanel.RenderTransform = geort;
        }
        #endregion // rotateGeoAndTurnstateCanvas()

        #region drawRelCells()

        private void drawRelCells(List<UIElement> relList)
        {
            try
            {
                double frameWidth = canvasRel.ActualWidth;
                double frameHeight = canvasRel.ActualHeight;

                metersPerPixelX = MapperSettings.elementSizeMeters * mapperVicinity.RelMapWidth / frameWidth;
                metersPerPixelY = MapperSettings.elementSizeMeters * mapperVicinity.RelMapHeight / frameHeight;

                for (int i = 0; i < mapperVicinity.RelMapHeight; i++)
                {
                    for (int j = 0; j < mapperVicinity.RelMapWidth; j++)
                    {
                        MapCell cell = mapperVicinity.relCellAt(j, i);

                        SolidColorBrush myBrush = null;

                        bool isEmpty = cell.val < 1;

                        if (!isEmpty)
                        {
                            Rectangle rect = new Rectangle();
                            rect.Width = relCellWidth;
                            rect.Height = relCellHeight;
                            rect.RadiusX = 4;
                            rect.RadiusY = 4;
                            rect.Margin = new Thickness(j * relCellWidth, i * relCellHeight, 0, 0);

                            rect.Fill = myBrush;
                            rect.Stroke = Brushes.Green;
                            rect.StrokeThickness = 0.5d;

                            relList.Add(rect);
                        }
                    }
                }
            }
            catch { }
        }
        #endregion // drawRelCells()

        #region drawGeoCells()

        // Create a couple of SolidColorBrush and use it to paint the rectangles:
        private SolidColorBrush myBrush0 = new SolidColorBrush(Colors.White);
        private SolidColorBrush myBrush1 = new SolidColorBrush(Colors.Black);
        private SolidColorBrush myBrushP = new SolidColorBrush(Colors.LightGreen);
        private SolidColorBrush myBrushB = new SolidColorBrush(Colors.Green);
        private SolidColorBrush myBrushH = new SolidColorBrush(Colors.Purple);

        private void drawGeoCells(List<UIElement> geoList)
        {
            try
            {
                MapperVicinity mapperVicinity = CurrentMapper;

                double frameWidth = canvasGeo.ActualWidth;
                double frameHeight = canvasGeo.ActualHeight;

                double geoCellWidth = frameWidth / mapperVicinity.GeoMapWidth;
                double geoCellHeight = frameHeight / mapperVicinity.GeoMapHeight;

                metersPerPixelX = MapperSettings.elementSizeMeters * mapperVicinity.GeoMapWidth / frameWidth;
                metersPerPixelY = MapperSettings.elementSizeMeters * mapperVicinity.GeoMapHeight / frameHeight;

                for (int i = 0; i < mapperVicinity.GeoMapHeight; i++)
                {
                    for (int j = 0; j < mapperVicinity.GeoMapWidth; j++)
                    {
                        MapCell cell = mapperVicinity.geoCellAt(j, i);

                        SolidColorBrush myBrush = null;

                        bool isEmpty = cell.val < 1;

                        if (_currentRoutePlanner != null)
                        {
                            // see if the cell belongs to any path, and allow to see if  that was the best path:
                            var query = from cp in _currentRoutePlanner.cellPaths
                                        where cp.ContainsCell(cell)
                                        orderby cp.isBest descending
                                        select cp;

                            if (query.Any())
                            {
                                myBrush = query.First().isBest ? myBrushB : myBrushP;
                                isEmpty = false;    // belongs to a path
                            }
                        }

                        if (!isEmpty)
                        {
                            if (myBrush == null)
                            {
                                myBrush = cell.HasHuman ? myBrushH : (cell.val < 1 ? myBrush0 : new SolidColorBrush(cell.DisplayColor));
                            }
                            Rectangle rect = new Rectangle();
                            rect.Width = geoCellWidth;
                            rect.Height = geoCellHeight;
                            rect.RadiusX = 4;
                            rect.RadiusY = 4;
                            rect.Margin = new Thickness(j * geoCellWidth, i * geoCellHeight, 0, 0);

                            bool isCenterCell = i == mapperVicinity.GeoMapHeight / 2 && j == mapperVicinity.GeoMapWidth / 2;

                            rect.Stroke = isCenterCell ? Brushes.Magenta : Brushes.Red;
                            rect.StrokeThickness = isCenterCell ? 5 : 0.5d;
                            rect.Fill = myBrush;
                            geoList.Add(rect);

                            if (cell.val > 0)
                            {
                                double fontSize = 10.0d;

                                Label cellValueLabel = new Label() { FontSize = fontSize, Foreground = Brushes.Black, Background = myBrush };
                                cellValueLabel.Content = cell.val;
                                cellValueLabel.Margin = rect.Margin;
                                geoList.Add(cellValueLabel);
                            }

                        }
                    }
                }
            }
            catch { }
        }
        #endregion // drawGeoCells()

        #region drawRobot()

        private void drawRobot()
        {
            try
            {
                RobotState rs = mapperVicinity.robotState;
                string controlName = "robotStateViewControl1";

                double robotWidth = rs.robotWidthMeters * relCellWidth / MapperSettings.elementSizeMeters;
                double robotHeight = rs.robotLengthMeters * relCellHeight / MapperSettings.elementSizeMeters;
                double robotX = (robotPanel.ActualWidth - robotWidth) / 2.0d;
                double robotY = (robotPanel.ActualHeight - robotHeight) / 2.0d;

                RobotStateViewControl rsvc = robotPanel.FindName(controlName) as RobotStateViewControl;
                if (rsvc == null)
                {
                    rsvc = new RobotStateViewControl()
                    {
                        Name = controlName,
                        robotState = rs,
                        Width = robotWidth,
                        Height = robotHeight,
                        Margin = new Thickness(robotX, robotY, 0, 0)
                    };

                    robotPanel.Children.Add(rsvc);
                    robotPanel.RegisterName(rsvc.Name, rsvc);
                }
                else
                {
                    if (rsvc.Width != robotWidth || rsvc.Height != robotHeight)
                    {
                        rsvc.Width = robotWidth;
                        rsvc.Height = robotHeight;
                        rsvc.Margin = new Thickness(robotX, robotY, 0, 0);
                    }
                    rsvc.robotState = rs;
                    rsvc.redrawRobot();
                    //Console.WriteLine(string.Format("drawRobot() - using existing control - scale: power={0} speed={1}", rsvc.maxAbsPower, rsvc.maxAbsSpeed));
                }
            }
            catch { }
        }

        private void drawTurnState(List<UIElement> geoList, List<UIElement> unrotatedList)
        {
            TurnState currentTurnState = mapperVicinity.turnState;

            bool inTurn = currentTurnState != null && currentTurnState.inTurn;
            string ts = inTurn ? currentTurnState.ToString() : "not in turn";

            double x0 = 20.0d;
            double y0 = 50.0d;

            Label lblTurn = new Label() { Content = ts, Margin = new Thickness(x0, y0, 0, 0), FontSize = 12, FontWeight = FontWeights.Normal };

            unrotatedList.Add(lblTurn);

            try
            {
                string controlName = "TurnStateViewControl1";

                double controlWidth = 3.0d * relCellWidth / MapperSettings.elementSizeMeters;
                double controlHeight = 3.0d * relCellHeight / MapperSettings.elementSizeMeters;
                double controlX = (turnPanel.ActualWidth - controlWidth) / 2.0d;
                double controlY = (turnPanel.ActualHeight - controlHeight) / 2.0d;

                TurnStateViewControl tsvc = turnPanel.FindName(controlName) as TurnStateViewControl;
                if (tsvc == null)
                {
                    tsvc = new TurnStateViewControl()
                    {
                        Name = controlName,
                        turnState = currentTurnState,
                        Width = controlWidth,
                        Height = controlHeight,
                        Margin = new Thickness(controlX, controlY, 0, 0)
                    };
                    turnPanel.Children.Add(tsvc);
                    turnPanel.RegisterName(tsvc.Name, tsvc);
                }
                else
                {
                    if (tsvc.Width != controlWidth || tsvc.Height != controlHeight)
                    {
                        tsvc.Width = controlWidth;
                        tsvc.Height = controlHeight;
                        tsvc.Margin = new Thickness(controlX, controlY, 0, 0);
                    }
                    tsvc.turnState = currentTurnState;
                    tsvc.redraw();
                    //Console.WriteLine(string.Format("draw() - using existing control - scale: power={0} speed={1}", rsvc.maxAbsPower, rsvc.maxAbsSpeed));
                }
            }
            catch { }
        }

        #endregion // drawRobot()

        #region drawHeadingBearingPointers()

        public void drawHeadingBearingPointers()
        {
            try
            {
                string controlNameB = "pointerBearing1";
                string controlNameH = "pointerHeading1";

                if (mapperVicinity.robotDirection != null && mapperVicinity.robotDirection.heading.HasValue && mapperVicinity.robotDirection.bearing.HasValue)
                {
                    Path pointerBearing = robotPanel.FindName(controlNameB) as Path;

                    if (pointerBearing == null)
                    {
                        // create Bearing pointer, add it to the robotPanel:

                        var dt = (DataTemplate)Resources["TemplateXaml"];

                        pointerBearing = (Path)dt.LoadContent();
                        pointerBearing.Name = controlNameB;
                        pointerBearing.Fill = Brushes.LightGreen;
                        robotPanel.Children.Add(pointerBearing);        // suitable canvas
                        robotPanel.RegisterName(pointerBearing.Name, pointerBearing);
                    }

                    Path pointerHeading = robotPanel.FindName(controlNameH) as Path;

                    if (pointerHeading == null)
                    {
                        // create Heading pointer, add it to the robotPanel:

                        var dt = (DataTemplate)Resources["TemplateXaml"];

                        pointerHeading = (Path)dt.LoadContent();
                        pointerHeading.Name = controlNameH;
                        robotPanel.Children.Add(pointerHeading);        // suitable canvas
                        robotPanel.RegisterName(pointerHeading.Name, pointerHeading);
                    }

                    double frameWidth = canvasRel.ActualWidth;
                    double frameHeight = canvasRel.ActualHeight;

                    double arrowShift = Math.Min(frameWidth, frameHeight) * 0.5d;   // how far the pointer will be from the center of the map

                    double bearing = (double)mapperVicinity.robotDirection.bearingRelative;

                    TranslateTransform tt = new TranslateTransform(-arrowShift, 0.0d);
                    TranslateTransform tt1 = new TranslateTransform(frameWidth / 2.0d, frameHeight / 2.0d - 10.0d);   // to the center of the control - post-rotate

                    //bearingLabel.Content = String.Format("{0,6:0.0}", bearing);

                    RotateTransform rtB = new RotateTransform(bearing + 90, 0, 10);
                    TransformGroup tgB = new TransformGroup();

                    tgB.Children.Add(tt);
                    tgB.Children.Add(st);
                    tgB.Children.Add(rtB);
                    tgB.Children.Add(tt1);

                    pointerBearing.RenderTransform = tgB;
                    pointerBearing.Visibility = Visibility.Visible;

                    // similarly draw heading arrow:
                    double heading = -(double)mapperVicinity.robotDirection.heading;

                    TranslateTransform ttH = new TranslateTransform(-arrowShift * 1.05d, 0.0d);
                    TranslateTransform ttH1 = new TranslateTransform(frameWidth / 2.0d, frameHeight / 2.0d - 10.0d);   // to the center of the control - post-rotate

                    //headingLabel.Content = String.Format("{0,6:0.0}", heading);

                    RotateTransform rtH = new RotateTransform(heading + 90, 0, 10);
                    TransformGroup tgH = new TransformGroup();

                    tgH.Children.Add(ttH);
                    tgH.Children.Add(st);
                    tgH.Children.Add(rtH);
                    tgH.Children.Add(ttH1);

                    pointerHeading.RenderTransform = tgH;
                }
                else
                {
                    Path pointerBearing = robotPanel.FindName(controlNameB) as Path;

                    if (pointerBearing != null)
                    {
                        pointerBearing.Visibility = Visibility.Hidden;
                    }
                }
            }
            catch { }
        }

        #endregion // drawHeadingBearingPointers()

        #region drawReferenceCircle()

        private static double[] circleDistances = new double[] { 1.0d, 2.0d, 3.0d, 4.0d };

        private void drawReferenceCircle(List<UIElement> relList)
        {
            try
            {
                double frameWidth = canvasRel.ActualWidth;
                double frameHeight = canvasRel.ActualHeight;
                foreach (double radius in circleDistances)
                {
                    // radius = MapperSettings.referenceCircleRadiusMeters;
                    double diamW = radius * 2.0d * relCellWidth / MapperSettings.elementSizeMeters;
                    double diamH = radius * 2.0d * relCellHeight / MapperSettings.elementSizeMeters;

                    // draw reference circle (usually 2 meters):
                    Ellipse refCircle = new Ellipse();
                    refCircle.Width = diamW;
                    refCircle.Height = diamH;
                    refCircle.Margin = new Thickness((frameWidth - refCircle.Width) / 2.0d, (frameHeight - refCircle.Height) / 2.0d, 0, 0);

                    refCircle.Stroke = Brushes.Cyan;
                    refCircle.StrokeThickness = 2;
                    relList.Add(refCircle);
                }
            }
            catch { }
        }
        #endregion // drawReferenceCircle()

        #region drawDetectedObjects()

        private void drawDetectedObjects(List<UIElement> relList, List<UIElement> unrotatedList, int method)
        {
            try
            {
                double frameWidth = canvasRel.ActualWidth;
                double frameHeight = canvasRel.ActualHeight;

                double metersPerPixelX = MapperSettings.elementSizeMeters / (frameWidth / mapperVicinity.RelMapWidth);
                double metersPerPixelY = MapperSettings.elementSizeMeters / (frameHeight / mapperVicinity.RelMapHeight);

                double latitudeFactor = Math.Cos(mapperVicinity.robotPosition.Y);
                double xMeters = Distance.METERS_PER_DEGREE * latitudeFactor;

                int i = 0;

                switch (method)
                {
                    case 0:
                    default:
                        i = mapperVicinity.Count;
                        break;

                    case 1:
                        lock (mapperVicinity)
                        {
                            foreach (IDetectedObject idobj in mapperVicinity)
                            {
                                DetectedObjectViewControl dovc = null;

                                if (idobj.objectType == DetectedObjectType.Mark)
                                {
                                    dovc = new DetectedObjectViewControl() { color = idobj.color, size = 21 };
                                }
                                else
                                {
                                    dovc = new DetectedObjectViewControl() { color = idobj.color };
                                }

                                /*
                                GeoPosition pos = idobj.geoPosition;

                                double distXmeters = (pos.Lng - mapperVicinity.robotPosition.Lng) * Distance.METERS_PER_DEGREE * latitudeFactor;
                                double distYmeters = (pos.Lat - mapperVicinity.robotPosition.Lat) * Distance.METERS_PER_DEGREE;
                                */

                                RelPosition pos = idobj.relPosition;

                                double distXmeters = pos.X;
                                double distYmeters = pos.Y;

                                double xPix = frameWidth / 2.0d + distXmeters / metersPerPixelX;
                                double yPix = frameHeight / 2.0d - distYmeters / metersPerPixelY;

                                dovc.Margin = new Thickness(xPix, yPix, 0, 0);

                                relList.Add(dovc);
                                i++;
                            }
                        }
                        break;
                }

                // draw number of Detected Objects:
                double x0 = 20.0d;
                double y0 = 20.0d;

                Label lN = new Label() { Content = string.Format("{0} objects", i), Margin = new Thickness(x0, y0, 0, 0), FontSize = 10, FontWeight = FontWeights.Normal };
                unrotatedList.Add(lN);

            }
            catch { }
        }
        #endregion // drawDetectedObjects()

        #region drawStatusString()

        private void drawStatusString(List<UIElement> unrotatedList)
        {
            if (!string.IsNullOrEmpty(StatusString))
            {
                double frameHeight = canvasRel.ActualHeight;

                // draw Status String:
                double x0 = 20.0d;
                double y0 = frameHeight - 30.0d;

                Label ssl = new Label() { Content = StatusString, Margin = new Thickness(x0, y0, 0, 0), FontSize = 12, FontWeight = FontWeights.Normal };
                unrotatedList.Add(ssl);
            }
        }

        #endregion // drawStatusString()

        #region drawMission()

        private void drawMission(List<UIElement> relList, List<UIElement> unrotatedList)
        {
            if (_currentRoutePlanner != null && _currentRoutePlanner.mission != null && _currentRoutePlanner.mission.waypoints.Any())
            {
                try
                {
                    double frameWidth = canvasRel.ActualWidth;
                    double frameHeight = canvasRel.ActualHeight;

                    double metersPerPixelX = MapperSettings.elementSizeMeters / (frameWidth / mapperVicinity.RelMapWidth);
                    double metersPerPixelY = MapperSettings.elementSizeMeters / (frameHeight / mapperVicinity.RelMapHeight);

                    double latitudeFactor = Math.Cos(mapperVicinity.robotPosition.Y);
                    double xMeters = Distance.METERS_PER_DEGREE * latitudeFactor;

                    int i = 0;

                    LocationWp prevWp = null;
                    double xPixPrev=0, yPixPrev=0;

                    foreach(LocationWp curWp in _currentRoutePlanner.mission.waypoints)
                    {
                        Color wpColor;
                        switch (curWp.waypointState)
                        {
                            default:
                            case WaypointState.None:
                                wpColor = Colors.Blue;
                                break;
                            case WaypointState.SelectedAsTarget:
                                wpColor = Colors.Orange;
                                break;
                            case WaypointState.Passed:
                                wpColor = Colors.Green;
                                break;
                            case WaypointState.CouldNotReach:
                                wpColor = Colors.Red;
                                break;
                        }

                        DetectedObjectViewControl dovc = new DetectedObjectViewControl() { color = wpColor, size = curWp.isHome ? 121 : 41 };

                        Direction dirToWp = curWp.directionToWp(mapperVicinity.robotPosition, mapperVicinity.robotDirection);

                        Distance distToWp = curWp.distanceToWp(mapperVicinity.robotPosition);

                        double metersToWaypoint = Math.Round(distToWp.Meters, 1);

                        distToWp /= 10.0d;     // fit to small scale vicinity map

                        RelPosition pos = new RelPosition(dirToWp, distToWp);

                        double distXmeters = pos.X;
                        double distYmeters = pos.Y;

                        double xPix = frameWidth / 2.0d + distXmeters / metersPerPixelX;
                        double yPix = frameHeight / 2.0d - distYmeters / metersPerPixelY;

                        dovc.Margin = new Thickness(xPix, yPix, 0, 0);

                        relList.Add(dovc);

                        string wptCommand = curWp.id == MAV_CMD.WAYPOINT ? (curWp.isHome ? "HOME" : string.Empty) : curWp.id.ToString();

                        string strSecToWp = string.Empty;

                        if (curWp.waypointState == WaypointState.SelectedAsTarget && curWp.estimatedTimeOfArrival.HasValue)
                        {
                            strSecToWp = string.Format(" {0:#} sec", (curWp.estimatedTimeOfArrival.Value - DateTime.Now).TotalSeconds);
                        }

                        Label labelWp = new Label() {
                            Content = string.Format("WPT{0}: {1} m {2} deg {3}{4}", curWp.number, metersToWaypoint, Math.Round((double)dirToWp.bearingRelative, 1), wptCommand, strSecToWp),
                            Margin = new Thickness(xPix + 10, yPix + 10, 0, 0),
                            FontSize = 14,
                            FontWeight = FontWeights.Bold
                        };

                        relList.Add(labelWp);

                        if (prevWp != null)
                        {
                            //Line lineToWp = new Line() { X1 = xPixPrev, Y1 = yPixPrev, X2 = xPix, Y2 = yPix, Stroke=Brushes.Red, StrokeThickness = 4.0d, StrokeEndLineCap = PenLineCap.Triangle };
                            //relList.Add(lineToWp);

                            Brush brush = Brushes.Blue;
                            double thickness = 4.0d;

                            if (prevWp.waypointState == WaypointState.Passed && curWp.waypointState == WaypointState.Passed)
                            {
                                // leg has been traveled:
                                brush = Brushes.Green;
                            }
                            else if (curWp.waypointState == WaypointState.SelectedAsTarget)
                            {
                                // current leg:
                                brush = Brushes.OrangeRed;
                                thickness = 6.0d;
                            }

                            relList.Add(LineArrowHelper.DrawLinkArrow(new Point(xPixPrev, yPixPrev), new Point(xPix, yPix), brush, thickness));
                        }

                        prevWp = curWp;
                        xPixPrev = xPix;
                        yPixPrev = yPix;
                        i++;
                    }
                }
                catch { }
            }
        }
        #endregion // drawMission()

        private void northUpCheckBox_Changed(object sender, RoutedEventArgs e)
        {
            drawAll();
        }

        #endregion // Draw cells, robot etc.

        private void UserControl_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            RedrawMap();
        }
    }
}
