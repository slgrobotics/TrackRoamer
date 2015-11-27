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
    /// a simplified version of AccelerometerData capable of DSS serialization
    /// </summary>
    [DataContract]
    public class AccelerometerDataDssSerializable //: IDssSerializable
    {
        [DataMember]
        public DateTime TimeStamp;

        // accelerometer values (x - forward, y - left, z - up)

        [DataMember]
        public double accX;

        [DataMember]
        public double accY;

        [DataMember]
        public double accZ;

        public AccelerometerDataDssSerializable()
        {
        }

        public AccelerometerDataDssSerializable(AccelerometerData accelerometerData)
        {
            TimeStamp = new DateTime(accelerometerData.TimeStamp);

            accX = accelerometerData.accX;
            accY = accelerometerData.accY;
            accZ = accelerometerData.accZ;
        }
    }
}
