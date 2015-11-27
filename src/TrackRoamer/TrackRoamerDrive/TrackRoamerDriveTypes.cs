using System;
using System.Collections.Generic;
using System.ComponentModel;
using Microsoft.Ccr.Core;
using Microsoft.Dss.Core.Attributes;
using Microsoft.Dss.ServiceModel.Dssp;
using Microsoft.Dss.ServiceModel.DsspServiceBase;
using W3C.Soap;

using drive = Microsoft.Robotics.Services.Drive.Proxy;

namespace TrackRoamer.Robotics.Services.TrackRoamerDrive
{
    /// <summary>
    /// TrackRoamerDrive contract class
    /// </summary>
    public sealed class Contract
    {
        /// <summary>
        /// DSS contract identifer for TrackRoamerDrive
        /// </summary>
        [DataMember]
        public const string Identifier = "http://schemas.trackroamer.com/robotics/2011/02/trackroamerdrive.html";
    }

    internal sealed class TrackRoamerDriveParams
    {
        // keep in mind that these are overwritten by  TrackRoamerDrive\Config\TrackRoamer.TrackRoamerBot.Drive.Config.xml
        // which is copied to  C:\Microsoft Robotics Dev Studio 2008 R3\Config  on every build:

        public const int MotorPowerScalingFactor = 100;            // 0 to 100 percent power you want to see on the wheels
        public const double DistanceBetweenWheels = 0.570;         // meters
        public const double WheelRadius = 0.1805;                  // meters
        public const double WheelGearRatio = 0.136;                // teeth on motor / teeth on wheel
        public const int EncoderTicksPerWheelRevolution = 6150;    // encoder ticks per full rotation of a wheel
    }

    /// <summary>
    /// Partners
    /// </summary>
    [DataContract]
    public static class Partners
    {
        /// <summary>
        /// Left Encoder
        /// </summary>
        [DataMember]
        public const string LeftEncoder = "LeftEncoder";
        /// <summary>
        /// Right Encoder
        /// </summary>
        [DataMember]
        public const string RightEncoder = "RightEncoder";
        /// <summary>
        /// Left Motor
        /// </summary>
        [DataMember]
        public const string LeftMotor = "LeftMotor";
        /// <summary>
        /// Right Motor
        /// </summary>
        [DataMember]
        public const string RightMotor = "RightMotor";
    }
}


