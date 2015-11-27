//-----------------------------------------------------------------------
//  This file is part of Microsoft Robotics Developer Studio Code Samples.
//
//  Copyright (C) Microsoft Corporation.  All rights reserved.
//
//  $File: TrackRoamerDriveTypes.cs $ $Revision: 1 $
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.ComponentModel;

using Microsoft.Ccr.Core;
using Microsoft.Dss.Core;
using Microsoft.Dss.Core.Attributes;
using Microsoft.Dss.Core.DsspHttp;
using Microsoft.Dss.ServiceModel.Dssp;

using W3C.Soap;

using trackroamerbot = TrackRoamer.Robotics.Services.TrackRoamerBot.Proxy;
using drive = Microsoft.Robotics.Services.Drive.Proxy;
using motor = Microsoft.Robotics.Services.Motor.Proxy;

namespace TrackRoamer.Robotics.Services.TrackRoamerServices.Drive
{
    /// <summary>
    /// TrackRoamerDrive Contract
    /// </summary>
    public static class Contract
    {
        /// The Unique Contract Identifier for the TrackRoamerDrive service
        public const String Identifier = "http://schemas.trackroamer.com/robotics/2009/04/trackroamerdrive.html";
    }

    [DataContract]
    public class TrackRoamerDriveState : drive.DriveDifferentialTwoWheelState, IDssSerializable
    {
        //public bool driveCommandInProgress;

        //public drive.DriveRequestOperation pendingDriveOperation;

        public drive.DriveRequestOperation InternalPendingDriveOperation;
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

    /// <summary>
    /// Drive Operations Port
    /// </summary>
    [ServicePort]
    public class DriveOperations : PortSet
    {
        /// <summary>
        /// Default constructor
        /// </summary>
        public DriveOperations()
            : base(
        typeof(DsspDefaultLookup),
        typeof(DsspDefaultDrop),
        typeof(drive.Get),
        typeof(HttpGet),
        typeof(HttpPost),
        typeof(drive.ReliableSubscribe),
        typeof(drive.Subscribe),
        typeof(Update),
        typeof(drive.EnableDrive),
        typeof(drive.SetDrivePower),
        typeof(drive.SetDriveSpeed),
        typeof(drive.RotateDegrees),
        typeof(drive.DriveDistance),
        typeof(drive.AllStop))
        {
        }

        /// <summary>
        /// Untyped post
        /// </summary>
        /// <param name="item"></param>
        public void Post(object item)
        {
            base.PostUnknownType(item);
        }

        /// <summary>
        /// Implicit conversion
        /// </summary>
        public static implicit operator Port<DsspDefaultLookup>(DriveOperations portSet)
        {
            if (portSet == null) return null;
            return (Port<DsspDefaultLookup>)portSet[typeof(DsspDefaultLookup)];
        }
        /// <summary>
        /// Implicit conversion
        /// </summary>
        public static implicit operator Port<DsspDefaultDrop>(DriveOperations portSet)
        {
            if (portSet == null) return null;
            return (Port<DsspDefaultDrop>)portSet[typeof(DsspDefaultDrop)];
        }
        /// <summary>
        /// Implicit conversion
        /// </summary>
        public static implicit operator Port<HttpGet>(DriveOperations portSet)
        {
            if (portSet == null) return null;
            return (Port<HttpGet>)portSet[typeof(HttpGet)];
        }
        /// <summary>
        /// Implicit conversion
        /// </summary>
        public static implicit operator Port<HttpPost>(DriveOperations portSet)
        {
            if (portSet == null) return null;
            return (Port<HttpPost>)portSet[typeof(HttpPost)];
        }
        /// <summary>
        /// Implicit conversion
        /// </summary>
        public static implicit operator Port<drive.Subscribe>(DriveOperations portSet)
        {
            if (portSet == null) return null;
            return (Port<drive.Subscribe>)portSet[typeof(drive.Subscribe)];
        }
        /// <summary>
        /// Implicit conversion
        /// </summary>
        public static implicit operator Port<drive.Get>(DriveOperations portSet)
        {
            if (portSet == null) return null;
            return (Port<drive.Get>)portSet[typeof(drive.Get)];
        }
        /// <summary>
        /// Implicit conversion
        /// </summary>
        public static implicit operator Port<Update>(DriveOperations portSet)
        {
            if (portSet == null) return null;
            return (Port<Update>)portSet[typeof(Update)];
        }
        /// <summary>
        /// Implicit conversion
        /// </summary>
        public static implicit operator Port<drive.ReliableSubscribe>(DriveOperations portSet)
        {
            if (portSet == null) return null;
            return (Port<drive.ReliableSubscribe>)portSet[typeof(drive.ReliableSubscribe)];
        }
        /// <summary>
        /// Implicit conversion
        /// </summary>
        public static implicit operator Port<drive.EnableDrive>(DriveOperations portSet)
        {
            if (portSet == null) return null;
            return (Port<drive.EnableDrive>)portSet[typeof(drive.EnableDrive)];
        }
        /// <summary>
        /// Implicit conversion
        /// </summary>
        public static implicit operator Port<drive.SetDrivePower>(DriveOperations portSet)
        {
            if (portSet == null) return null;
            return (Port<drive.SetDrivePower>)portSet[typeof(drive.SetDrivePower)];
        }
        /// <summary>
        /// Implicit conversion
        /// </summary>
        public static implicit operator Port<drive.SetDriveSpeed>(DriveOperations portSet)
        {
            if (portSet == null) return null;
            return (Port<drive.SetDriveSpeed>)portSet[typeof(drive.SetDriveSpeed)];
        }
        /// <summary>
        /// Implicit conversion
        /// </summary>
        public static implicit operator Port<drive.RotateDegrees>(DriveOperations portSet)
        {
            if (portSet == null) return null;
            return (Port<drive.RotateDegrees>)portSet[typeof(drive.RotateDegrees)];
        }
        /// <summary>
        /// Implicit conversion
        /// </summary>
        public static implicit operator Port<drive.DriveDistance>(DriveOperations portSet)
        {
            if (portSet == null) return null;
            return (Port<drive.DriveDistance>)portSet[typeof(drive.DriveDistance)];
        }
        /// <summary>
        /// Implicit conversion
        /// </summary>
        public static implicit operator Port<drive.AllStop>(DriveOperations portSet)
        {
            if (portSet == null) return null;
            return (Port<drive.AllStop>)portSet[typeof(drive.AllStop)];
        }

    }


    #region Operation Port Definitions

    /// <summary>
    /// Operation Update Drive State
    /// </summary>
    [Description("Updates (or indicates an update to) the drive's state.")]
    public class Update : Update<TrackRoamerDriveState, PortSet<DefaultUpdateResponseType, Fault>>
    {
        /// <summary>
        /// Default Constructor
        /// </summary>
        public Update() { }

        /// <summary>
        /// Initialization Constructor
        /// </summary>
        /// <param name="state"></param>
        public Update(TrackRoamerDriveState state) { this.Body = state; }
    }

    // other operations are good enough in the Microsoft.Robotics.Services.Drive.Proxy namespace, they are compatible with standard Differential Drive

    #endregion // Operation Port Definitions
}
