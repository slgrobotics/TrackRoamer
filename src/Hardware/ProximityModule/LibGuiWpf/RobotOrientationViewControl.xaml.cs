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

using System.Windows.Media.Media3D;

using TrackRoamer.Robotics.Utility.LibSystem;

namespace TrackRoamer.Robotics.LibGuiWpf
{
    /// <summary>
    /// Interaction logic for RobotOrientationViewControl.xaml
    /// </summary>
    public partial class RobotOrientationViewControl : UserControl
    {
        /// <summary>
        /// Dependency property to Get/Set the current value 
        /// </summary>
        public static readonly DependencyProperty CurrentValueProperty =
            DependencyProperty.Register("CurrentValue", typeof(AccelerometerDataWpf), typeof(RobotOrientationViewControl),
            new PropertyMetadata(null, new PropertyChangedCallback(RobotOrientationViewControl.OnCurrentValuePropertyChanged)));


        /// <summary>
        /// Gets/Sets the current value
        /// </summary>
        public AccelerometerDataWpf CurrentValue
        {
            get
            {
                return (AccelerometerDataWpf)GetValue(CurrentValueProperty);
            }
            set
            {
                SetValue(CurrentValueProperty, value);
            }
        }

        public RobotOrientationViewControl()
        {
            InitializeComponent();
        }

        #region Methods

        private static void OnCurrentValuePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            //Get access to the instance of RobotOrientationViewControl whose property value changed
            RobotOrientationViewControl gauge = d as RobotOrientationViewControl;
            gauge.OnCurrentValueChanged(e);

        }

        public virtual void OnCurrentValueChanged(DependencyPropertyChangedEventArgs e)
        {
            AccelerometerDataWpf newValue = (AccelerometerDataWpf)e.NewValue;

            xLabel.Content = String.Format("{0,6:0.00}", newValue.accX);

            yLabel.Content = String.Format("{0,6:0.00}", newValue.accY);

            zLabel.Content = String.Format("{0,6:0.00}", newValue.accZ);

            RobotGeometryModel3D.Transform = newValue.robotOrientationTransform;
        }

        /// <summary>
        /// matrix by three rotation angles
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="z"></param>
        /// <returns></returns>
        Matrix3D CalculateRotationMatrix(double x, double y, double z)
        {
            Matrix3D matrix = new Matrix3D();

            matrix.Rotate(new Quaternion(new Vector3D(1, 0, 0), x));
            matrix.Rotate(new Quaternion(new Vector3D(0, 1, 0) * matrix, y));
            matrix.Rotate(new Quaternion(new Vector3D(0, 0, 1) * matrix, z));

            return matrix;
        }

        #endregion // Methods
    }
}
