using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Windows.Media.Media3D;

using TrackRoamer.Robotics.Utility.LibPicSensors;

namespace TrackRoamer.Robotics.LibGuiWpf
{
    /// <summary>
    /// on top of its base class AccelerometerData, AccelerometerDataWpf precomputes vectors needed for transforms, and (when needed) the Transform itself.
    /// </summary>
    public class AccelerometerDataWpf : AccelerometerData
    {
        // WPF values (x - right, y - up, z - backwards to the viewer -- http://msdn.microsoft.com/en-us/library/ms753347.aspx )
        public double wpfX;
        public double wpfY;
        public double wpfZ;

        public double gTotal = 10.0d;

        public Vector3D orientationDownVector;       // vector aligned with the downwards direction of robot body

        /// <summary>
        /// precompute vectors needed for transforms
        /// </summary>
       public void computeVectors()
        {
            wpfX = -accY;
            wpfY = accZ;
            wpfZ = -accX;

            orientationDownVector = new Vector3D(wpfX, wpfY, wpfZ);

            gTotal = orientationDownVector.Length;

            orientationDownVector.Normalize();

            //this.Refresh();
        }

        private static Vector3D vCoordDown = new Vector3D(0, 0, 1);            // vector pointing down in WPF coordinate space

        public Transform3D robotOrientationTransform
        {
            get
            {
                // http://blogs.msdn.com/b/jgalasyn/archive/2007/05/08/pointing-a-3d-model-along-a-direction-vector.aspx
                // Generate a rotation that will rotate from pointing along vCoordDown to pointing along this.orientationDownVector.

                double angle = Math.Acos(Vector3D.DotProduct(vCoordDown, this.orientationDownVector) / (vCoordDown.Length * this.orientationDownVector.Length));
                Vector3D perpVector = Vector3D.CrossProduct(vCoordDown, this.orientationDownVector);
                var rot = new AxisAngleRotation3D(perpVector, angle * 180 / Math.PI);

                return new RotateTransform3D(rot);
            }
        }
    }
}
