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
using TrackRoamer.Robotics.LibBehavior;
using TrackRoamer.Robotics.LibGuiWpf;

namespace TrackRoamer.Robotics.WpfProximityModuleDemo
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

        // these two are here to avoid null exceptions; real ones are in MainWindow:
        private GeoPosition robotPositionDefault = new GeoPosition(-117.0d, 34.0d);     // will be overwritten by invoking code in MainWindow
        private Direction robotDirectionDefault = new Direction() { heading = 10.0d, bearing = -10.0d };   // will be overwritten by invoking code in MainWindow

        private void mapperViewControl1_Loaded(object sender, RoutedEventArgs e)
        {
            mapperViewControl1.setRobotPositionAndDirection(robotPositionDefault, robotDirectionDefault);   // will be overwritten by invoking code
        }

        private void buttonReset_Click(object sender, RoutedEventArgs e)
        {
            MapperVicinity mv = new MapperVicinity();

            mapperViewControl1.CurrentMapper = mv;
            mapperViewControl1.CurrentRoutePlanner = new RoutePlanner(mv);

            mapperViewControl1.setRobotPositionAndDirection(robotPositionDefault, robotDirectionDefault);
        }

        private void buttonUp_Click(object sender, RoutedEventArgs e)
        {
            MapperVicinity mv = mapperViewControl1.CurrentMapper;

            mv.translate(0.0d, 0.3048d);

            planRoute();
        }

        private void buttonDown_Click(object sender, RoutedEventArgs e)
        {
            MapperVicinity mv = mapperViewControl1.CurrentMapper;

            mv.translate(0.0d, -0.3048d);

            planRoute();
        }

        private void buttonLeft_Click(object sender, RoutedEventArgs e)
        {
            MapperVicinity mv = mapperViewControl1.CurrentMapper;

            mv.translate(-0.3048d, 0.0d);

            planRoute();
        }

        private void buttonRight_Click(object sender, RoutedEventArgs e)
        {
            MapperVicinity mv = mapperViewControl1.CurrentMapper;

            mv.translate(0.3048d, 0.0d);

            planRoute();
        }

        private void buttonRotateL_Click(object sender, RoutedEventArgs e)
        {
            rotate(-1);
        }

        private void buttonRotateR_Click(object sender, RoutedEventArgs e)
        {
            rotate(1);
        }

        /// <summary>
        /// rotates robot, reading the "textBoxRotate" control
        /// </summary>
        /// <param name="dir">direction of rotation,  1 - rotates robot to the right, -1 - to the left</param>
        private void rotate(int dir)
        {
            double angle;

            if (double.TryParse(textBoxRotate.Text, out angle))     // degrees
            {
                MapperVicinity mv = mapperViewControl1.CurrentMapper;

                mv.rotate(angle * dir);

                planRoute();
            }
        }

        private void MapperMouseMoved_Handler(object sender, RoutedEventArgs e)
        {
            RoutedEventArgsMouseMoved ee = (RoutedEventArgsMouseMoved)e;

            labelXYMeters.Content = "Meters:\r\nx=" + ee.xMeters + "\r\ny=" + ee.yMeters;

            labelXYGeo.Content = "Coord:\r\nx=" + ee.geoPosition.Lng + "\r\ny=" + ee.geoPosition.Lat;

            MapperVicinity mv = mapperViewControl1.CurrentMapper;

            Distance dfX = ee.geoPosition.distanceFrom(mv.robotPosition);

            Distance dfXe = ee.geoPosition.distanceFromExact(mv.robotPosition);

            labelFree.Content = dfX.Meters.ToString() + "\r\n" + dfXe.Meters.ToString();
        }

        private void planRouteButton_Click(object sender, RoutedEventArgs e)
        {
            planRoute();
        }

        public void planRoute()
        {
            MapperVicinity mv = mapperViewControl1.CurrentMapper;

            RoutePlanner routePlanner = mapperViewControl1.CurrentRoutePlanner;

            RoutePlan plan = routePlanner.planRoute();

            bestHeadingLabel.Content = "Best Heading abs=" + (plan.bestHeading.HasValue ? Math.Round((double)plan.bestHeading).ToString() : "N/A") + " rel (turn)=" + (plan.bestHeading.HasValue ? Math.Round((double)plan.bestHeading).ToString() : "N/A");
            closestObstacleLabel.Content = "ClosestObstacle at " + (plan.closestObstacleMeters.HasValue ? (Math.Round((double)plan.closestObstacleMeters, 2).ToString() + " m") : "N/A");
            legLengthLabel.Content = "Leg Length = " + (plan.legMeters.HasValue ? (Math.Round((double)plan.legMeters, 2).ToString() + " m") : "N/A (turn only)");
            timeSpentLabel.Content = plan.timeSpentPlanning.ToString();

            mapperViewControl1.RedrawMap();
        }
    }
}
