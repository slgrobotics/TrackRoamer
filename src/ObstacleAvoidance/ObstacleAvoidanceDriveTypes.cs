//------------------------------------------------------------------------------
//  <copyright file="ObstacleAvoidanceDriveTypes.cs" company="Microsoft Corporation">
//      Copyright (C) Microsoft Corporation.  All rights reserved.
//  </copyright>
//------------------------------------------------------------------------------

namespace Microsoft.Robotics.Services.ObstacleAvoidanceDrive
{
    using System;
    using Microsoft.Ccr.Core;
    using Microsoft.Dss.Core.Attributes;
    using Microsoft.Dss.Core.DsspHttp;
    using Microsoft.Dss.ServiceModel.Dssp;
    using Microsoft.Robotics.PhysicalModel;

    /// <summary>
    /// Contract class
    /// </summary>
    public sealed class Contract
    {
        /// <summary>
        /// Contract identifier
        /// </summary>
        [DataMember]
        public const string Identifier = "http://schemas.microsoft.com/2011/07/obstacleavoidancedrive.user.html";
    }

    /// <summary>
    /// Operations port
    /// </summary>
    public class ObstacleAvoidanceDriveOperationsPort : PortSet<Get, HttpQuery, HttpGet, DsspDefaultGet, DsspDefaultLookup, DsspDefaultDrop>
    {
    }

    /// <summary>
    /// Service state
    /// </summary>
    [DataContract]
    public class ObstacleAvoidanceDriveState
    {
        /// <summary>
        /// Gets or sets robot width in meters
        /// </summary>
        [DataMember]
        public double RobotWidth { get; set; }

        /// <summary>
        /// Gets or sets max power allowed per wheel
        /// </summary>
        [DataMember]
        public double MaxPowerPerWheel { get; set; }

        /// <summary>
        /// Gets or sets the minimum rotation speed
        /// </summary>
        [DataMember]
        public double MinRotationSpeed { get; set; }

        /// <summary>
        /// Gets or sets the depth camera position relative to the floor plane 
        /// and the projection of the center of mass of the robot to the floor plane
        /// </summary>
        [DataMember]
        public Vector3 DepthCameraPosition { get; set; }

        /// <summary>
        /// Gets or sets the obstacle avoidance controller state
        /// </summary>
        [DataMember]
        public PIDController Controller { get; set; }

        /// <summary>
        /// Gets or sets the maximum allowed change in Power from one call to SetPower to the next
        /// Smaller numbers will cause smoother accelrations, but can also increase chance of collision with 
        /// obstacles
        /// </summary>
        [DataMember]
        public double MaxDeltaPower { get; set; }
    }

    /// <summary>
    /// Get operation
    /// </summary>
    public class Get : Get<GetRequestType, DsspResponsePort<ObstacleAvoidanceDriveState>>
    {
    }

    /// <summary>
    /// Partner names
    /// </summary>
    [DataContract]
    public class Partners
    {
        /// <summary>
        /// Drive service
        /// </summary>
        [DataMember]
        public const string Drive = "Drive";

        /// <summary>
        /// Depth cam service
        /// </summary>
        [DataMember]
        public const string DepthCamSensor = "DepthCamera";

        /// <summary>
        /// IR sensor array
        /// </summary>
        [DataMember]
        public const string InfraredSensorArray = "InfraredSensorArray";

        /// <summary>
        /// Sonar analog sensors
        /// </summary>
        [DataMember]
        public const string SonarSensorArray = "SonarSensorArray";

        /// <summary>
        /// Time we are willing to wait for each partner to start
        /// </summary>
        [DataMember]
        public const int PartnerEnumerationTimeoutInSeconds = 120;
    }
}
