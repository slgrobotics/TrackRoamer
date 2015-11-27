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

using TrackRoamer.Robotics.Utility.LibPicSensors;

namespace TrackRoamer.Robotics.LibGuiWpf
{
    /// <summary>
    /// Interaction logic for RobotDirectionViewControl.xaml
    /// </summary>
    public partial class RobotDirectionViewControl : UserControl
    {
        private Path pointerHeading = null;
        private Path pointerBearing = null;

        public RobotDirectionViewControl()
        {
            InitializeComponent();

            // create Heading and Bearing pointers, add them to the grid:

            var dt = (DataTemplate)Resources["TemplateXaml"];
            pointerHeading = (Path)dt.LoadContent();
            grid1.Children.Add(pointerHeading);

            pointerBearing = (Path)dt.LoadContent();
            pointerBearing.Fill = Brushes.LightGreen;
            pointerBearing.Opacity = 0.5;
            grid1.Children.Add(pointerBearing);
        }

        #region Dependency Methods - Direction

        /// <summary>
        /// Dependency property to Get/Set the Current Direction value 
        /// </summary>
        public static readonly DependencyProperty CurrentDirectionProperty =
            DependencyProperty.Register("CurrentDirection", typeof(DirectionData), typeof(RobotDirectionViewControl),
            new PropertyMetadata(null, new PropertyChangedCallback(RobotDirectionViewControl.OnCurrentDirectionPropertyChanged)));


        /// <summary>
        /// Gets/Sets the Current Direction value
        /// </summary>
        public DirectionData CurrentDirection
        {
            get
            {
                return (DirectionData)GetValue(CurrentDirectionProperty);
            }
            set
            {
                SetValue(CurrentDirectionProperty, value);
            }
        }

        private static void OnCurrentDirectionPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            //Get access to the instance of RobotDirectionViewControl whose property value changed
            RobotDirectionViewControl gauge = d as RobotDirectionViewControl;
            gauge.OnCurrentDirectionChanged(e);
        }

        // a set of transforms to draw pointers in the right places; create them once:
        private ScaleTransform st = new ScaleTransform(1.4d, 1.0d);
        private TranslateTransform tt = new TranslateTransform(-70.0d, 0.0d);   // more than half the length of the beam - pre-rotate
        private TranslateTransform tt1 = new TranslateTransform(50.0d, 0.0d);   // to the center of the control - post-rotate

        public virtual void OnCurrentDirectionChanged(DependencyPropertyChangedEventArgs e)
        {
            DirectionData newValue = (DirectionData)e.NewValue;

            double heading = newValue.heading;
            double bearing = newValue.bearing;

            headingLabel.Content = String.Format("{0,6:0.0}", heading);

            RotateTransform rtH = new RotateTransform(heading + 90, 0, 10);
            TransformGroup tgH = new TransformGroup();
            tgH.Children.Add(tt);
            tgH.Children.Add(st);
            tgH.Children.Add(rtH);
            tgH.Children.Add(tt1);
            pointerHeading.RenderTransform = tgH;

            bearingLabel.Content = String.Format("{0,6:0.0}", bearing);

            RotateTransform rtB = new RotateTransform(bearing + 90, 0, 10);
            TransformGroup tgB = new TransformGroup();
            tgB.Children.Add(tt);
            tgB.Children.Add(st);
            tgB.Children.Add(rtB);
            tgB.Children.Add(tt1);
            pointerBearing.RenderTransform = tgB;
        }

        #endregion // Dependency Methods - Direction
    }
}
