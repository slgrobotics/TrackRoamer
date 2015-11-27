using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TrackRoamer.Robotics.Utility.LibPicSensors
{
    public class SensorsState
    {
        public bool irbValid;

        public byte irbE1;
        public byte irbE2;
        public byte irbE3;
        public byte irbE4;

        public byte irbO1;
        public byte irbO2;
        public byte irbO3;
        public byte irbO4;

        public bool parkingSensorsValid;

        public uint parkingSensorsCount;     // how many sensors are connected, 4 or 8
        public byte[] parkingSensors = new byte[8];     // raw data
        public double[] parkingSensorsMeters = new double[8]; // converted to meters
        public double parkingSensorMetersLF;
        public double parkingSensorMetersRF;
        public double parkingSensorMetersLB;
        public double parkingSensorMetersRB;

        public bool compassValid;

        public double compassHeading;

        public bool accelValid;

        public double accelX;
        public double accelY;
        public double accelZ;

        public double analogValue1;

        public void mapAnalogData(int channel, byte lsb, byte msb)
        {
            switch (channel)
            {
                case 0:
                    analogValue1 = (((ushort)msb << 8) + (ushort)lsb);
                    break;
            }
        }

        /// <summary>
        /// converts data sent by the Proximity board to something we can use here.
        /// </summary>
        public void mapParkingSensorsData()
        {
            parkingSensorsCount /= 8;       // it comes as 32 for 4 sensors (64 for 8?)
            parkingSensorsValid = parkingSensorsCount > 0;  // as reported from the board - count=0 means invalid data

            if (parkingSensorsValid)    // do mapping only if data is valid
            {
                for (int i = 0; i < parkingSensorsCount; i++)
                {
                    parkingSensorsMeters[i] = Math.Round((26.0d - ((double)parkingSensors[i])) * 2.3d / 26.0d, 2);
                }
                parkingSensorMetersLF = parkingSensorsMeters[2];
                parkingSensorMetersRF = parkingSensorsMeters[0];
                parkingSensorMetersLB = parkingSensorsMeters[3];
                parkingSensorMetersRB = parkingSensorsMeters[1];
            }
        }
    }
}
