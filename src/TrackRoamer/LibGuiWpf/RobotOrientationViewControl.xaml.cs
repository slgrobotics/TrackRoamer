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
        private GeometryModel3D mGeometry;

        public RobotOrientationViewControl()
        {
            InitializeComponent();

            BuildSolid();
        }

        private void BuildSolid()
        {
            // Define 3D mesh object
            MeshGeometry3D mesh = new MeshGeometry3D();

            mesh.Positions.Add(new Point3D(-0.5, -0.5, 1));
            //mesh.Normals.Add(new Vector3D(0, 0, 1));
            mesh.Positions.Add(new Point3D(0.5, -0.5, 1));
            //mesh.Normals.Add(new Vector3D(0, 0, 1));
            mesh.Positions.Add(new Point3D(0.5, 0.5, 1));
            //mesh.Normals.Add(new Vector3D(0, 0, 1));
            mesh.Positions.Add(new Point3D(-0.5, 0.5, 1));
            //mesh.Normals.Add(new Vector3D(0, 0, 1));

            mesh.Positions.Add(new Point3D(-1, -0.1, -1));
            //mesh.Normals.Add(new Vector3D(0, 0, -1));
            mesh.Positions.Add(new Point3D(1, -0.1, -1));
            //mesh.Normals.Add(new Vector3D(0, 0, -1));
            mesh.Positions.Add(new Point3D(1, 0.1, -1));
            //mesh.Normals.Add(new Vector3D(0, 0, -1));
            mesh.Positions.Add(new Point3D(-1, 0.1, -1));
            //mesh.Normals.Add(new Vector3D(0, 0, -1));

            // Front face
            mesh.TriangleIndices.Add(0);
            mesh.TriangleIndices.Add(1);
            mesh.TriangleIndices.Add(2);
            mesh.TriangleIndices.Add(2);
            mesh.TriangleIndices.Add(3);
            mesh.TriangleIndices.Add(0);

            // Back face
            mesh.TriangleIndices.Add(6);
            mesh.TriangleIndices.Add(5);
            mesh.TriangleIndices.Add(4);
            mesh.TriangleIndices.Add(4);
            mesh.TriangleIndices.Add(7);
            mesh.TriangleIndices.Add(6);

            // Right face
            mesh.TriangleIndices.Add(1);
            mesh.TriangleIndices.Add(5);
            mesh.TriangleIndices.Add(2);
            mesh.TriangleIndices.Add(5);
            mesh.TriangleIndices.Add(6);
            mesh.TriangleIndices.Add(2);

            // Top face
            mesh.TriangleIndices.Add(2);
            mesh.TriangleIndices.Add(6);
            mesh.TriangleIndices.Add(3);
            mesh.TriangleIndices.Add(3);
            mesh.TriangleIndices.Add(6);
            mesh.TriangleIndices.Add(7);

            // Bottom face
            mesh.TriangleIndices.Add(5);
            mesh.TriangleIndices.Add(1);
            mesh.TriangleIndices.Add(0);
            mesh.TriangleIndices.Add(0);
            mesh.TriangleIndices.Add(4);
            mesh.TriangleIndices.Add(5);

            // Right face
            mesh.TriangleIndices.Add(4);
            mesh.TriangleIndices.Add(0);
            mesh.TriangleIndices.Add(3);
            mesh.TriangleIndices.Add(3);
            mesh.TriangleIndices.Add(7);
            mesh.TriangleIndices.Add(4);

            // material:

            MaterialGroup mg = new MaterialGroup();

            mg.Children.Add(new DiffuseMaterial(Brushes.Brown));

            // Geometry creation
            mGeometry = new GeometryModel3D(mesh, mg);
            RobotGeometryModel3D.Children.Add(mGeometry);
            RobotGeometryModel3D.Transform = new Transform3DGroup();

            setRotation();
        }

        /// <summary>
        /// apply whatever we have in totalRotation to the scene
        /// </summary>
        private void setRotation()
        {
            Transform3DGroup tg = RobotGeometryModel3D.Transform as Transform3DGroup;
            tg.Children.Clear();
            tg.Children.Add(_currentAttitude.robotOrientationTransform);

            //_currentAttitude.computeEulerAngles(); - this is implicit in the angles' getters

            xLabel.Content = String.Format("Yaw:{0,4:0}", toDegrees(_currentAttitude.heading));      // {0,6:0.00}

            yLabel.Content = String.Format("Pitch:{0,4:0}", toDegrees(_currentAttitude.pitch));

            zLabel.Content = String.Format("Roll:{0,4:0}", toDegrees(_currentAttitude.roll));
        }


        #region CurrentValue (as AccelerometerDataWpf)

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

        #endregion // CurrentValue (as AccelerometerDataWpf)

        #region CurrentAttitude (as OrientationData)

        private OrientationData _currentAttitude = new OrientationData() { timestamp = DateTime.Now, attitudeQuaternion = new Quaternion() };

        /// <summary>
        /// Dependency property to Get/Set the current Attitude 
        /// </summary>
        public static readonly DependencyProperty CurrentAttitudeProperty =
            DependencyProperty.Register("CurrentAttitude", typeof(OrientationData), typeof(RobotOrientationViewControl),
            new PropertyMetadata(null, new PropertyChangedCallback(RobotOrientationViewControl.OnCurrentAttitudePropertyChanged)));

        /// <summary>
        /// Gets/Sets the current Attitude
        /// </summary>
        public OrientationData CurrentAttitude
        {
            get
            {
                return (OrientationData)GetValue(CurrentAttitudeProperty);
            }
            set
            {
                SetValue(CurrentAttitudeProperty, value);
            }
        }

        private static void OnCurrentAttitudePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            //Get access to the instance of RobotOrientationViewControl whose property value changed
            RobotOrientationViewControl gauge = d as RobotOrientationViewControl;
            gauge.OnCurrentAttitudeChanged(e);

        }

        public virtual void OnCurrentAttitudeChanged(DependencyPropertyChangedEventArgs e)
        {
            _currentAttitude = (OrientationData)e.NewValue;

            setRotation();
        }

        #endregion // CurrentValue (as OrientationData)

        #region Helper methods

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

        public double toDegrees(double radians)
        {
            return radians * 180.0d / Math.PI;
        }

        #endregion // Helper methods
    }
}
