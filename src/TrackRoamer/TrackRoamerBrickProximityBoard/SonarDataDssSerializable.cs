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
    /// a simplified version of SonarData capable of DSS serialization
    /// </summary>
    [DataContract]
    public class SonarDataDssSerializable //: IDssSerializable
    {
        [DataMember]
        public DateTime TimeStamp;

        [DataMember]
        public int Count = 0;

        [DataMember]
        public int[] RawAngles;

        [DataMember]
        public double[] RangeMeters;

        public SonarDataDssSerializable()
        {
        }

        public SonarDataDssSerializable(SonarData sonarData)
        {
            TimeStamp = new DateTime(sonarData.TimeStamp);

            Count = sonarData.angles.Count;

            RawAngles = sonarData.angles.Keys.ToArray<int>();

            RangeMeters = (from v in sonarData.angles.Values select v.rangeMeters).ToArray<double>();
        }
    }
}
