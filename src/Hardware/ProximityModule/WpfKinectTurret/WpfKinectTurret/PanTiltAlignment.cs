using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Xml;
using System.Xml.Serialization;

namespace WpfKinectTurret
{
    [Serializable]
    public class PanTiltAlignment
    {
        public double panFactor;
        public double tiltFactor;

        public double panAlign;
        public double tiltAlign;

        private static PanTiltAlignment instance;

        public static string filename = @"C:\temp\PanTiltAlignment.xml";

        private PanTiltAlignment()
        {
            panFactor = 10.0d;
            tiltFactor = 10.0d;

            panAlign = 0;
            tiltAlign = 0;
        }

        public static PanTiltAlignment getInstance()
        {
            if (instance == null)
            {
                instance = new PanTiltAlignment();
            }
            return instance;
        }

        public static void Save()
        {
            using (TextWriter writer = new StreamWriter(filename, false))
            {
                XmlSerializer x = new System.Xml.Serialization.XmlSerializer(typeof(PanTiltAlignment));
                x.Serialize(writer, instance);
            }
        }

        public static void Restore()
        {
            if (File.Exists(filename))
            {
                using (TextReader reader = new StreamReader(filename))
                {
                    XmlSerializer x = new System.Xml.Serialization.XmlSerializer(typeof(PanTiltAlignment));
                    instance = (PanTiltAlignment)x.Deserialize(reader);
                }
            }
        }
    }
}
