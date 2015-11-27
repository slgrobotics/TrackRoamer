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
    /// a simplified version of DirectionData capable of DSS serialization
    /// </summary>
    [DataContract]
    public class DirectionDataDssSerializable //: IDssSerializable
    {
        [DataMember]
        public DateTime TimeStamp;

        [DataMember]
        public double heading;

        [DataMember]
        public double? bearing;

        public DirectionDataDssSerializable()
        {
        }

        public DirectionDataDssSerializable(DirectionData directionData)
        {
            TimeStamp = new DateTime(directionData.TimeStamp);

            heading = directionData.heading;
            bearing = directionData.bearing;
        }
    }
}
