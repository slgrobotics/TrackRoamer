/*
 * Copyright (c) 2013..., Sergei Grichine   http://trackroamer.com
 * 
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at 
 *    http://www.apache.org/licenses/LICENSE-2.0
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 *    
 * this is a no-warranty no-liability permissive license - you do not have to publish your changes,
 * although doing so, donating and contributing is always appreciated
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.IO;
using System.Xml;
using System.Xml.Serialization;
using System.Diagnostics;

namespace Trackroamer.Library.LibAnimatronics
{
    [Serializable]
    public class PanTiltAlignment
    {

        public double panFactor;
        public double tiltFactor;
        // default values derived during calibration:
        public double panFactorAnalogPlus = -0.397727272727273d;
        public double panFactorAnalogMinus = -0.397727272727273d;
        public double panAlignAnalog = -391.0d;

        public double panAlign;
        public double tiltAlign;

        private static PanTiltAlignment instance;

        public static string filename = @"C:\temp\PanTiltAlignmentAnimatronicHead.xml";

        private PanTiltAlignment()
        {
            panFactor = 10.0d;
            tiltFactor = 10.0d;

            panAlign = 0.0d;
            tiltAlign = 0.0d;
        }

        public static PanTiltAlignment getInstance()
        {
            if (instance == null)
            {
                instance = new PanTiltAlignment();
            }
            return instance;
        }

        public double mksPan(double panDegreesFromCenter)
        {
            return 1500.0d + panAlign + panDegreesFromCenter * panFactor;
        }

        public double mksTilt(double tiltDegreesFromCenter)
        {
            return 1500.0d + tiltAlign + tiltDegreesFromCenter * tiltFactor;
        }

        public double degreesPan(double analogValue)
        {
            double tmp = panAlignAnalog + analogValue;
            return tmp > 0.0d ? tmp * panFactorAnalogPlus : tmp * panFactorAnalogMinus;
        }

        /// <summary>
        /// computes PanTiltAlignment aligns and factors for pan analog measurements, given three points (center, -70, +70)
        /// </summary>
        public void computeCalibrationAnalog(double analogValueAtMinus, double analogValueAtZero, double analogValueAtPlus, double spanHZeroToMinus, double spanHZeroToPlus)
        {
            panAlignAnalog = -analogValueAtZero;       // offset in analog units

            // compute factors for both positive and negative travel, compensating for non-linear measurement:

            panFactorAnalogMinus = spanHZeroToMinus / (analogValueAtZero - analogValueAtMinus);      // degrees per analog unit
            panFactorAnalogPlus = spanHZeroToPlus / (analogValueAtPlus - analogValueAtZero);

            Debug.WriteLine("panAlignAnalog=" + panAlignAnalog + "    panFactorAnalogPlus=" + panFactorAnalogPlus + "     panFactorAnalogMinus=" + panFactorAnalogMinus);
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
