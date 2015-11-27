using System;
using System.Linq;
using System.Collections.Generic;
using System.ComponentModel;
using Microsoft.Ccr.Core;
using Microsoft.Dss.Core;
using Microsoft.Dss.Core.Attributes;
using Microsoft.Dss.ServiceModel.Dssp;
using Microsoft.Dss.ServiceModel.DsspServiceBase;
using W3C.Soap;

using TrackRoamer.Robotics.Utility.LibPicSensors;

namespace TrackRoamer.Robotics.Services.TrackRoamerBrickProximityBoard
{
    /// <summary>
    /// a simplified version of ProximityData capable of DSS serialization
    /// </summary>
    [DataContract]
    public class ProximityDataDssSerializable //: IDssSerializable
    {
        [DataMember]
        public DateTime TimeStamp;

        // distance in meters for every IR Proximity sensor:

        [DataMember]
        public double mfl;      // front-left
        [DataMember]
        public double mffl;     // front-front-left
        [DataMember]
        public double mffr;
        [DataMember]
        public double mfr;

        [DataMember]
        public double mbl;       // back-left
        [DataMember]
        public double mbbl;
        [DataMember]
        public double mbbr;
        [DataMember]
        public double mbr;

        // for use internally in top image generation, distances in meters arranged in certain order for DrawHelper:
        internal double[] arrangedForDrawing = new double[8];

        public ProximityDataDssSerializable()
        {
        }

        public ProximityDataDssSerializable(ProximityData proximityData)
        {
            TimeStamp = new DateTime(proximityData.TimeStamp);

            // clockwise starting from rear right side:
            mbr = arrangeProximityReading(proximityData.mbr, 0);
            mbbr = arrangeProximityReading(proximityData.mbbr, 1);
            mbbl = arrangeProximityReading(proximityData.mbbl, 2);
            mbl = arrangeProximityReading(proximityData.mbl, 3);

            mfl = arrangeProximityReading(proximityData.mfl, 4);
            mffl = arrangeProximityReading(proximityData.mffl, 5);
            mffr = arrangeProximityReading(proximityData.mffr, 6);
            mfr = arrangeProximityReading(proximityData.mfr, 7);

        }

        /// <summary>
        /// returns meters from IR data and arranges data to an array for internal use (for top image generation)
        /// </summary>
        /// <param name="meters">IR distance</param>
        /// <param name="placeToIdx">where to put it for internal use</param>
        /// <returns>approximation in meters</returns>
        private double arrangeProximityReading(double meters, int placeToIdx)
        {
            arrangedForDrawing[placeToIdx] = meters;

            return meters;
        }
    }
}
