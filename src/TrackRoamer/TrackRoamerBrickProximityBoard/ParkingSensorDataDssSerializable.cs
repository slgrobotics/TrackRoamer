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
    /// a simplified version of ParkingSensorData capable of DSS serialization
    /// </summary>
    [DataContract]
    public class ParkingSensorDataDssSerializable //: IDssSerializable
    {
        [DataMember]
        public DateTime TimeStamp;

        [DataMember]
        public double parkingSensorMetersLF;

        [DataMember]
        public double parkingSensorMetersRF;

        [DataMember]
        public double parkingSensorMetersLB;

        [DataMember]
        public double parkingSensorMetersRB;

        public ParkingSensorDataDssSerializable()
        {
        }

        // for use internally in top image generation, distances in meters arranged in certain order for DrawHelper:
        internal double[] arrangedForDrawing = new double[4];

        public ParkingSensorDataDssSerializable(ParkingSensorData parkingSensorData)
        {
            TimeStamp = new DateTime(parkingSensorData.TimeStamp);

            parkingSensorMetersLF = parkingSensorData.parkingSensorMetersLF;
            parkingSensorMetersRF = parkingSensorData.parkingSensorMetersRF;
            parkingSensorMetersLB = parkingSensorData.parkingSensorMetersLB;
            parkingSensorMetersRB = parkingSensorData.parkingSensorMetersRB;

            arrangedForDrawing[0] = parkingSensorMetersRB;
            arrangedForDrawing[1] = parkingSensorMetersLB;
            arrangedForDrawing[2] = parkingSensorMetersLF;
            arrangedForDrawing[3] = parkingSensorMetersRF;
        }
    }
}
