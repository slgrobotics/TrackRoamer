using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Windows.Media.Media3D;

namespace TrackRoamer.Robotics.LibGuiWpf
{
    /// <summary>
    /// a container for orientation data, like quaternion-based attitude data.
    /// </summary>
    public class OrientationData
    {
        public DateTime timestamp;

        private Quaternion _attitudeQuaternion;
        private double _heading;
        private double _pitch;
        private double _roll;
        private bool EulerAnglesComputed = false;

        public Quaternion attitudeQuaternion { get { return _attitudeQuaternion; }  set { _attitudeQuaternion = value; _attitudeQuaternion.Normalize(); EulerAnglesComputed = false; } }

        //  Angles in radians - filled by calling computeEulerAngles():
        public double heading { get { computeEulerAngles(); return _heading; } private set { _heading = value; }  }
        public double yaw       { get { return heading; } }     // same as heading
        public double pitch { get { computeEulerAngles(); return _pitch; } private set { _pitch = value; } }
        public double roll { get { computeEulerAngles(); return _roll; } private set { _roll = value; } }

        public Transform3D robotOrientationTransform
        {
            get
            {
                Transform3DGroup transform = new Transform3DGroup();

                attitudeQuaternion.Normalize();

                Rotation3D rotation = new QuaternionRotation3D(attitudeQuaternion);

                transform.Children.Add(new RotateTransform3D(rotation));

                return transform;
            }
        }

        /// <summary>
        /// fills heading (yaw) pitch and roll based on attitudeQuaternion. Beware of singularities. Angles in radians.
        /// </summary>
        public void computeEulerAngles()
        {
            // see http://www.euclideanspace.com/maths/geometry/rotations/conversions/quaternionToEuler/index.htm  for quaternions to Euler angles formula. Second (non-normalized) version used.

            // also see C:\Microsoft Robotics Dev Studio 4\samples\Sensors\CameraSensorUtilities\Utilities.cs : QuaternionToEulerRadians()

            if (!EulerAnglesComputed)
            {
                double sqx = attitudeQuaternion.X * attitudeQuaternion.X;
                double sqy = attitudeQuaternion.Y * attitudeQuaternion.Y;
                double sqz = attitudeQuaternion.Z * attitudeQuaternion.Z;
                double sqw = attitudeQuaternion.W * attitudeQuaternion.W;

                double unit = sqx + sqy + sqz + sqw;        // if normalized then one, otherwise correction factor
                double test = attitudeQuaternion.X * attitudeQuaternion.Y + attitudeQuaternion.Z * attitudeQuaternion.W;

                if (test > 0.499 * unit)
                {  
                    // singularity at north pole
                    heading = 2 * Math.Atan2(attitudeQuaternion.X, attitudeQuaternion.W);
                    pitch = Math.PI / 2;
                    roll = 0;
                    EulerAnglesComputed = true;
                    return;
                }

                if (test < -0.499 * unit)
                { 
                    // singularity at south pole
                    heading = -2 * Math.Atan2(attitudeQuaternion.X, attitudeQuaternion.W);
                    pitch = -Math.PI / 2;
                    roll = 0;
                    EulerAnglesComputed = true;
                    return;
                }

                // original formula:
                //heading = Math.Atan2(2 * attitudeQuaternion.Y * attitudeQuaternion.W - 2 * attitudeQuaternion.X * attitudeQuaternion.Z, sqx - sqy - sqz + sqw);
                //pitch = Math.Asin(2 * test / unit);
                //roll = Math.Atan2(2 * attitudeQuaternion.X * attitudeQuaternion.W - 2 * attitudeQuaternion.Y * attitudeQuaternion.Z, -sqx + sqy - sqz + sqw);

                // corrected for rotations in the ViewControl:
                heading = -Math.Atan2(2 * attitudeQuaternion.Y * attitudeQuaternion.W - 2 * attitudeQuaternion.X * attitudeQuaternion.Z, sqx - sqy - sqz + sqw);
                roll = -Math.Asin(2 * test / unit);
                pitch = Math.Atan2(2 * attitudeQuaternion.X * attitudeQuaternion.W - 2 * attitudeQuaternion.Y * attitudeQuaternion.Z, -sqx + sqy - sqz + sqw);

                EulerAnglesComputed = true;
            }
        }
    }
}
