using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.IO;
using System.Xml;
using System.Xml.Serialization;

using TrackRoamer.Robotics.Utility.LibSystem;

namespace TrackRoamer.Robotics.LibBehavior
{
    [Serializable]
    public class PanTiltAlignment
    {
        public double panFactorKinect;
        public double panAlignKinect;

        public double panFactorKinectAnalogPlus;
        public double panFactorKinectAnalogMinus;
        public double panAlignKinectAnalog;

        public double panFactorGunLeft;
        public double tiltFactorGunLeft;

        public double panAlignGunLeft;
        public double tiltAlignGunLeft;
        public double angleGunLeft;         // degrees

        public double timeGunOnMsGunLeft;

        public double panFactorGunRight;
        public double tiltFactorGunRight;

        public double panAlignGunRight;
        public double tiltAlignGunRight;
        public double angleGunRight;         // degrees

        public double timeGunOnMsGunRight;

        private static PanTiltAlignment instance;

        public static string filename = @"C:\temp\PanTiltAlignment.xml";

        private PanTiltAlignment()
        {
            panFactorKinect = 10.0d;

            panFactorGunLeft = 10.0d;
            tiltFactorGunLeft = 10.0d;

            panFactorGunRight = 10.0d;
            tiltFactorGunRight = 10.0d;

            panAlignKinect = 0.0d;

            panAlignGunLeft = 0.0d;
            tiltAlignGunLeft = 0.0d;
            angleGunLeft = 0.0d;

            panAlignGunRight = 0.0d;
            tiltAlignGunRight = 0.0d;
            angleGunRight = 0.0d;

            timeGunOnMsGunLeft = 250.0d;   // 250ms plants a good single shot.
            timeGunOnMsGunRight = 250.0d;
        }

        public static PanTiltAlignment getInstance()
        {
            if (instance == null)
            {
                instance = new PanTiltAlignment();
            }
            return instance;
        }

        public double mksPanGunLeft(double panDegreesFromCenter)
        {
            return 1500.0d + panAlignGunLeft + panDegreesFromCenter * panFactorGunLeft;
        }

        public double mksTiltGunLeft(double tiltDegreesFromCenter)
        {
            return 1500.0d + tiltAlignGunLeft + tiltDegreesFromCenter * tiltFactorGunLeft;
        }

        public double mksPanGunRight(double panDegreesFromCenter)
        {
            return 1500.0d + panAlignGunRight + panDegreesFromCenter * panFactorGunRight;
        }

        public double mksTiltGunRight(double tiltDegreesFromCenter)
        {
            return 1500.0d + tiltAlignGunRight + tiltDegreesFromCenter * tiltFactorGunRight;
        }

        public double mksPanKinect(double panDegreesFromCenter)
        {
            return 1500.0d + panAlignKinect - panDegreesFromCenter * panFactorKinect;
        }

        public double degreesPanKinect(double analogValue)
        {
            double tmp = panAlignKinectAnalog + analogValue;
            return tmp > 0.0d ? tmp * panFactorKinectAnalogPlus : tmp * panFactorKinectAnalogMinus;
        }

        /// <summary>
        /// computes PanTiltAlignment aligns and factors for Kinect pan analog measurements, given three points (center, -70, +70)
        /// </summary>
        public void computeCalibrationKinectAnalog(double analogValueAtMinus, double analogValueAtZero, double analogValueAtPlus, double spanHKinectZeroToMinus, double spanHKinectZeroToPlus)
        {
            panAlignKinectAnalog = -analogValueAtZero;       // offset in analog units

            // compute factors for both positive and negative travel, compensating for non-linear measurement:

            panFactorKinectAnalogMinus = spanHKinectZeroToMinus / (analogValueAtZero - analogValueAtMinus);      // degrees per analog unit
            panFactorKinectAnalogPlus = spanHKinectZeroToPlus / (analogValueAtPlus - analogValueAtZero);

            Tracer.Trace("panAlignKinectAnalog=" + panAlignKinectAnalog + "    panFactorKinectAnalogPlus=" + panFactorKinectAnalogPlus + "     panFactorKinectAnalogMinus=" + panFactorKinectAnalogMinus);
        }

        public static void Save()
        {
            using (TextWriter writer = new StreamWriter(filename, false))
            {
                XmlSerializer x = new System.Xml.Serialization.XmlSerializer(typeof(PanTiltAlignment));
                x.Serialize(writer, instance);
            }
        }

        public static PanTiltAlignment RestoreOrDefault()
        {
            if (File.Exists(filename))
            {
                using (TextReader reader = new StreamReader(filename))
                {
                    XmlSerializer x = new System.Xml.Serialization.XmlSerializer(typeof(PanTiltAlignment));
                    instance = (PanTiltAlignment)x.Deserialize(reader);
                }
            }
            else
            {
                instance = new PanTiltAlignment();
            }
            return instance;
        }
    }
}
