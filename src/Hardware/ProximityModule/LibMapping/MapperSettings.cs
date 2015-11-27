using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Configuration;

namespace TrackRoamer.Robotics.LibMapping
{
    public static class MapperSettings
    {
        public static readonly int nW = 39;
        public static readonly int nH = 39;
        public static readonly double elementSizeMeters = 0.25d; // 0.3048d;  // 1 ft = 0.3048d meters
        public static readonly double robotWidthMeters = 0.66d;
        public static readonly double robotLengthMeters = 0.82d;
        public static readonly double referenceCircleRadiusMeters = 2.0d;

        public const int UNITS_DISTANCE_DEFAULT = 1; //Distance.UNITS_DISTANCE_M;

        public static int unitsDistance = UNITS_DISTANCE_DEFAULT;
        public static int coordStyle = 1;				// "N37°28.893'  W117°43.368'"

        static MapperSettings()
        {
            // Warning: in Designer the ConfigurationManager will not read App.config properly.
            // make sure the defaults are reasonable and absence of App.config does not cause undefined values.

            int tmpSz;

            if (int.TryParse(ConfigurationManager.AppSettings["MapperVicinityMapSize"], out tmpSz))
            {
                nH = nW = tmpSz;

                double.TryParse(ConfigurationManager.AppSettings["MapperVicinityMapElementSizeMeters"], out elementSizeMeters);
                double.TryParse(ConfigurationManager.AppSettings["RobotWidthMeters"], out robotWidthMeters);
                double.TryParse(ConfigurationManager.AppSettings["RobotLengthMeters"], out robotLengthMeters);
                double.TryParse(ConfigurationManager.AppSettings["ReferenceCircleRadiusMeters"], out referenceCircleRadiusMeters);
            }

            nH = nW - 4;        // for debugging make it non-square
        }
    }
}
