using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TrackRoamer.Robotics.Utility.LibPicSensors
{
    public class ProximityData
    {
        public long TimeStamp = 0L;

        // distance in meters for every IR Proximity sensor:

        public double mfl;      // front-left
        public double mffl;     // front-front-left
        public double mffr;
        public double mfr;

        public double mbl;       // back-left
        public double mbbl;
        public double mbbr;
        public double mbr;

        // raw data for every IR Proximity sensor:

        public byte fl;      // front-left
        public byte ffl;     // front-front-left
        public byte ffr;
        public byte fr;

        public byte bl;       // back-left
        public byte bbl;
        public byte bbr;
        public byte br;

        public void setProximityData(byte irbE1, byte irbE2, byte irbE3, byte irbE4, byte irbO1, byte irbO2, byte irbO3, byte irbO4)
        {
            fl = irbO2;
            ffl = irbE1;
            ffr = irbO1;
            fr = irbE2;

            bl = irbE3;
            bbl = irbO4;
            bbr = irbE4;
            br = irbO3;

            mfl = rawToMeters(fl);
            mffl = rawToMeters(ffl);
            mffr = rawToMeters(ffr);
            mfr = rawToMeters(fr);

            mbl = rawToMeters(bl);
            mbbl = rawToMeters(bbl);
            mbbr = rawToMeters(bbr);
            mbr = rawToMeters(br);
        }

        private double rawToMeters(byte raw)
        {
            // roughly raw 7 is about 1.4 meters, it goes to 1 at about 0.5m

            return Math.Round(((double)raw) * 0.2d, 2);
        }
    }
}
