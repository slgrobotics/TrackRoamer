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

using TrackRoamer.Robotics.LibMapping;

namespace TrackRoamer.Robotics.LibGuiWpf
{
    /// <summary>
    /// Interaction logic for RobotStateViewControl.xaml
    /// </summary>
    public partial class RobotStateViewControl : UserControl
    {
        public RobotState robotState = new RobotState()
        {
            // have some values here for designer; in runtime robotState should be set on instantiation. 
            robotLengthMeters = 0.82d,
            robotWidthMeters = 0.66d,

            leftPower = 0.0d,
            leftSpeed = 0.0d,

            rightPower = 0.0d,
            rightSpeed = 0.0d,
            medianVelocity = 0.0d,

            manualControl = true,
            manualControlCommand = string.Empty,

            ignoreGps = false,
            ignoreAhrs = false,
            ignoreLaser = false,
            ignoreProximity = false,
            ignoreParkingSensor = false,
            ignoreKinectSounds = false,
            ignoreRedShirt = false,
            ignoreKinectSkeletons = false,
            doLostTargetRoutine = false,
            doPhotos = false,
            doVicinityPlanning = false
        };

        // to simplify scaling, wheel power and speed bars are scaled to the maximum observed so far.
        public double maxAbsPower { get; private set; }
        public double maxAbsSpeed { get; private set; }

        public RobotStateViewControl()
        {
            InitializeComponent();
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            drawRobot();
        }

        public void redrawRobot()
        {
            drawRobot();
        }

        private void drawRobot()
        {
            try
            {
                double robotWidth = canvas1.ActualWidth;
                double robotHeight = canvas1.ActualHeight;

                SolidColorBrush myBrushR = new SolidColorBrush(Colors.Yellow);

                canvas1.Children.Clear();

                // draw robot boundaries:
                Rectangle rect = new Rectangle();
                rect.Width = robotWidth;
                rect.Height = robotHeight;
                rect.RadiusX = 10;
                rect.RadiusY = 10;
                rect.Margin = new Thickness(0, 0, 0, 0);
                rect.Stroke = Brushes.Red;
                rect.StrokeThickness = 3;
                rect.Fill = myBrushR;
                canvas1.Children.Add(rect);

                double barWidth = robotWidth / 8.0d;

                // draw median velocity vector:
                double medianVelocityAbs = Math.Abs(robotState.medianVelocity);
                if (medianVelocityAbs > 0.001d)
                {
                    if (medianVelocityAbs > maxAbsSpeed)
                    {
                        maxAbsSpeed = medianVelocityAbs;    // store new high for scaling
                    }
                    Rectangle rectMS = new Rectangle();
                    rectMS.Width = barWidth;
                    rectMS.Height = robotHeight * medianVelocityAbs / (maxAbsSpeed * 2.0d);
                    double shiftY = robotHeight / 2.0d - (robotState.medianVelocity > 0.0d ? rectMS.Height : 0.0d);
                    rectMS.Margin = new Thickness(0.0d + (robotWidth - barWidth) / 2.0d, 0.0d + shiftY, 0, 0);
                    rectMS.Fill = Brushes.Red;
                    canvas1.Children.Add(rectMS);
                }

                // draw power bars and speed bars:
                double leftPowerAbs = Math.Abs(robotState.leftPower);
                if (leftPowerAbs > 0.001d)
                {
                    if (leftPowerAbs > maxAbsPower)
                    {
                        maxAbsPower = leftPowerAbs;    // store new high for scaling
                    }
                    Rectangle rectPwrL = new Rectangle();
                    rectPwrL.Width = barWidth;
                    rectPwrL.Height = robotHeight * leftPowerAbs / (maxAbsPower * 2.0d);
                    double shiftY = robotHeight / 2.0d - (robotState.leftPower > 0.0d ? rectPwrL.Height : 0.0d);
                    rectPwrL.Margin = new Thickness(0.0d + barWidth / 2.0d, 0.0d + shiftY, 0, 0);
                    rectPwrL.Fill = Brushes.Green;
                    canvas1.Children.Add(rectPwrL);
                }

                double rightPowerAbs = Math.Abs(robotState.rightPower);
                if (rightPowerAbs > 0.001d)
                {
                    if (rightPowerAbs > maxAbsPower)
                    {
                        maxAbsPower = rightPowerAbs;    // store new high for scaling
                    }
                    Rectangle rectPwrR = new Rectangle();
                    rectPwrR.Width = barWidth;
                    rectPwrR.Height = robotHeight * rightPowerAbs / (maxAbsPower * 2.0d);
                    double shiftY = robotHeight / 2.0d - (robotState.rightPower > 0.0d ? rectPwrR.Height : 0.0d);
                    rectPwrR.Margin = new Thickness(0.0d + robotWidth - barWidth * 1.5d, 0.0d + shiftY, 0, 0);
                    rectPwrR.Fill = Brushes.Green;
                    canvas1.Children.Add(rectPwrR);
                }

                double leftSpeedAbs = Math.Abs(robotState.leftSpeed);
                if (leftSpeedAbs > 0.001d)
                {
                    if (leftSpeedAbs > maxAbsSpeed)
                    {
                        maxAbsSpeed = leftSpeedAbs;    // store new high for scaling
                    }
                    Rectangle rectSpdL = new Rectangle();
                    rectSpdL.Width = barWidth;
                    rectSpdL.Height = robotHeight * leftSpeedAbs / (maxAbsSpeed * 2.0d);
                    double shiftY = robotHeight / 2.0d - (robotState.leftSpeed > 0.0d ? rectSpdL.Height : 0.0d);
                    rectSpdL.Margin = new Thickness(0.0d - barWidth - 4.0d, 0.0d + shiftY, 0, 0);
                    rectSpdL.Fill = Brushes.Red;
                    canvas1.Children.Add(rectSpdL);
                }

                double rightSpeedAbs = Math.Abs(robotState.rightSpeed);
                if (rightSpeedAbs > 0.001d)
                {
                    if (rightSpeedAbs > maxAbsSpeed)
                    {
                        maxAbsSpeed = rightSpeedAbs;    // store new high for scaling
                    }
                    Rectangle rectSpdR = new Rectangle();
                    rectSpdR.Width = barWidth;
                    rectSpdR.Height = robotHeight * rightSpeedAbs / (maxAbsSpeed * 2.0d);
                    double shiftY = robotHeight / 2.0d - (robotState.rightSpeed > 0.0d ? rectSpdR.Height : 0.0d);
                    rectSpdR.Margin = new Thickness(0.0d + robotWidth + 2.0d, 0.0d + shiftY, 0, 0);
                    rectSpdR.Fill = Brushes.Red;
                    canvas1.Children.Add(rectSpdR);
                }

                // draw a small circle in the middle to mark the center:
                Ellipse centerCircle = new Ellipse();
                centerCircle.Width = 0.04d * 2.0d * robotWidth / MapperSettings.elementSizeMeters;
                centerCircle.Height = 0.04d * 2.0d * robotWidth / MapperSettings.elementSizeMeters;
                centerCircle.Margin = new Thickness((robotWidth - centerCircle.Width) / 2.0d, (robotHeight - centerCircle.Height) / 2.0d, 0, 0);

                centerCircle.Stroke = Brushes.Red;
                centerCircle.StrokeThickness = 4;
                canvas1.Children.Add(centerCircle);

                // draw front of the robot indicator arrow:
                var dt = (DataTemplate)Resources["TemplateXaml"];

                Path pointerBearing = (Path)dt.LoadContent();
                pointerBearing.Fill = Brushes.Green;
                pointerBearing.Width = robotWidth * 0.6d;
                pointerBearing.Height = robotHeight * 0.4d;
                pointerBearing.Margin = new Thickness(robotWidth * 0.2d, -pointerBearing.Height, 0, 0);
                canvas1.Children.Add(pointerBearing);

                // let the max values melt in time, so that a spike will not affect the bars on the long run:
                maxAbsPower *= 0.95d;
                maxAbsSpeed *= 0.95d;

                double fontSize = 10.0d;

                Label speedLabel = new Label() { FontSize = fontSize, Foreground = Brushes.Red };
                speedLabel.Content = "speed";
                speedLabel.Margin = new Thickness(robotWidth * 0.17d, robotHeight * 0.5d, 0.0d, 0.0d);
                canvas1.Children.Add(speedLabel);

                Label powerLabel = new Label() { FontSize = fontSize, Foreground = Brushes.Green };
                powerLabel.Content = "power";
                powerLabel.Margin = new Thickness(robotWidth * 0.15d, robotHeight * 0.64d, 0.0d, 0.0d);
                canvas1.Children.Add(powerLabel);

            }
            catch (Exception exc)
            {
                Console.WriteLine("drawRobot() - " + exc);
            }
        }
    }
}
