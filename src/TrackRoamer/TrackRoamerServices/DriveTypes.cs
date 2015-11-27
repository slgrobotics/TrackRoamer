//-----------------------------------------------------------------------
//  This file is part of Microsoft Robotics Developer Studio Code Samples.
//
//  Copyright (C) Microsoft Corporation.  All rights reserved.
//
//  $File: DriveTypes.cs $ $Revision: 1 $
//-----------------------------------------------------------------------

using Microsoft.Ccr.Core;
using Microsoft.Dss.Core.Attributes;
using Microsoft.Dss.Core.DsspHttp;
using Microsoft.Dss.ServiceModel.Dssp;

using System;
using System.Collections.Generic;
using System.ComponentModel;
using W3C.Soap;

using motor = Microsoft.Robotics.Services.Motor;

namespace Microsoft.Robotics.Services.Drive
{

    /// <summary>
    /// Dss Drive Contract
    /// </summary>
    public static class Contract
    {
        /// <summary>
        /// Drive contract
        /// </summary>
        public const string Identifier = "http://schemas.microsoft.com/robotics/2006/05/drive.html";
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
        typeof(Get),
        typeof(HttpGet),
        typeof(HttpPost),
        typeof(ReliableSubscribe),
        typeof(Subscribe),
        typeof(Update),
        typeof(EnableDrive),
        typeof(SetDrivePower),
        typeof(SetDriveSpeed),
        typeof(RotateDegrees),
        typeof(DriveDistance),
        typeof(AllStop))
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
        public static implicit operator Port<Subscribe>(DriveOperations portSet)
        {
            if (portSet == null) return null;
            return (Port<Subscribe>)portSet[typeof(Subscribe)];
        }
        /// <summary>
        /// Implicit conversion
        /// </summary>
        public static implicit operator Port<Get>(DriveOperations portSet)
        {
            if (portSet == null) return null;
            return (Port<Get>)portSet[typeof(Get)];
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
        public static implicit operator Port<ReliableSubscribe>(DriveOperations portSet)
        {
            if (portSet == null) return null;
            return (Port<ReliableSubscribe>)portSet[typeof(ReliableSubscribe)];
        }
        /// <summary>
        /// Implicit conversion
        /// </summary>
        public static implicit operator Port<EnableDrive>(DriveOperations portSet)
        {
            if (portSet == null) return null;
            return (Port<EnableDrive>)portSet[typeof(EnableDrive)];
        }
        /// <summary>
        /// Implicit conversion
        /// </summary>
        public static implicit operator Port<SetDrivePower>(DriveOperations portSet)
        {
            if (portSet == null) return null;
            return (Port<SetDrivePower>)portSet[typeof(SetDrivePower)];
        }
        /// <summary>
        /// Implicit conversion
        /// </summary>
        public static implicit operator Port<SetDriveSpeed>(DriveOperations portSet)
        {
            if (portSet == null) return null;
            return (Port<SetDriveSpeed>)portSet[typeof(SetDriveSpeed)];
        }
        /// <summary>
        /// Implicit conversion
        /// </summary>
        public static implicit operator Port<RotateDegrees>(DriveOperations portSet)
        {
            if (portSet == null) return null;
            return (Port<RotateDegrees>)portSet[typeof(RotateDegrees)];
        }
        /// <summary>
        /// Implicit conversion
        /// </summary>
        public static implicit operator Port<DriveDistance>(DriveOperations portSet)
        {
            if (portSet == null) return null;
            return (Port<DriveDistance>)portSet[typeof(DriveDistance)];
        }
        /// <summary>
        /// Implicit conversion
        /// </summary>
        public static implicit operator Port<AllStop>(DriveOperations portSet)
        {
            if (portSet == null) return null;
            return (Port<AllStop>)portSet[typeof(AllStop)];
        }

    }


    #region Operation Port Definitions

    /// <summary>
    /// Operation Retrieve Drive State
    /// </summary>
    [Description("Gets the drive's current state.")]
    public class Get : Get<GetRequestType, PortSet<DriveDifferentialTwoWheelState, Fault>>
    {
    }

    /// <summary>
    /// Operation Update Drive State
    /// </summary>
    [Description("Updates (or indicates an update to) the drive's state.")]
    public class Update : Update<DriveDifferentialTwoWheelState, PortSet<DefaultUpdateResponseType, Fault>>
    {
        /// <summary>
        /// Default Constructor
        /// </summary>
        public Update() { }

        /// <summary>
        /// Initialization Constructor
        /// </summary>
        /// <param name="state"></param>
        public Update(DriveDifferentialTwoWheelState state) { this.Body = state; }
    }

    /// <summary>
    /// Operation Enable Drive
    /// </summary>
    [Description("Enables (or disables) a drive (or indicates whether a drive is enabled).")]
    public class EnableDrive : Update<EnableDriveRequest, PortSet<DefaultUpdateResponseType, Fault>>
    {
        /// <summary>
        /// public constructor
        /// </summary>
        public EnableDrive()
        {
            Body = new EnableDriveRequest();
        }
    }

    /// <summary>
    /// Operation Update Motor Power
    /// </summary>
    [Description("Sets (or indicates a change to) the drive's power.")]
    public class SetDrivePower : Update<SetDrivePowerRequest, PortSet<DefaultUpdateResponseType, Fault>>
    {
        /// <summary>
        /// public constructor
        /// </summary>
        public SetDrivePower() { }

        /// <summary>
        /// SetDrivePower Initialization Constructor
        /// </summary>
        /// <param name="body"></param>
        public SetDrivePower(SetDrivePowerRequest body)
        {
            Body = body;
        }
    }

    /// <summary>
    /// Operation Update Motor Speed
    /// </summary>
    [Description("Sets (or indicates) the drive speed.")]
    public class SetDriveSpeed : Update<SetDriveSpeedRequest, PortSet<DefaultUpdateResponseType, Fault>>
    {
    }

    /// <summary>
    /// Request the drive to rotate or turn in position (positive values turn counterclockwise).
    /// </summary>
    [Description("Request the drive to rotate or turn in position (positive values turn counterclockwise).")]
    public class RotateDegrees : Update<RotateDegreesRequest, PortSet<DefaultUpdateResponseType, Fault>>
    {
        /// <summary>
        /// Request the drive to rotate or turn in position (positive values turn counterclockwise).
        /// </summary>
        public RotateDegrees() { }

        /// <summary>
        /// Request the drive to rotate or turn in position (positive values turn counterclockwise).
        /// </summary>
        /// <param name="body"></param>
        public RotateDegrees(RotateDegreesRequest body)
        {
            Body = body;
        }
    }


    /// <summary>
    /// Operation drive a specified distance, then stop
    /// </summary>
    [Description("Updates (or indicates and update to) a distance setting for the drive.")]
    public class DriveDistance : Update<DriveDistanceRequest, PortSet<DefaultUpdateResponseType, Fault>>
    {
        /// <summary>
        /// public constructor
        /// </summary>
        public DriveDistance() { }

        /// <summary>
        /// DriveDistance Initialization Constructor
        /// </summary>
        /// <param name="body"></param>
        public DriveDistance(DriveDistanceRequest body)
        {
            Body = body;
        }
    }

    /// <summary>
    /// Emergency Stop
    /// <remarks>overrides long running commands</remarks>
    /// </summary>
    [Description("Stops the drive.")]
    public class AllStop : Update<AllStopRequest, PortSet<DefaultUpdateResponseType, Fault>>
    {
        /// <summary>
        /// public constructor
        /// </summary>
        public AllStop()
        {
            Body = new AllStopRequest();
        }
    }

    /// <summary>
    /// Operation to subscribe to drive notifications
    /// </summary>
    public class Subscribe : Subscribe<SubscribeRequestType, PortSet<SubscribeResponseType, Fault>>
    {
    }

    /// <summary>
    /// Operation to reliable subscribe to drive notifications
    /// </summary>
    public class ReliableSubscribe : Subscribe<ReliableSubscribeRequestType, PortSet<SubscribeResponseType, Fault>>
    {
    }

    #endregion

    #region internal types
    //======================================================================
    // Operations that are internal to a generic differential drive service


    /// <summary>
    /// Cancel Pending Drive Operation (used internally)
    /// <remarks>Used to cancel drive operations that are monitoring the internal drive cancellation port.</remarks>
    /// <remarks>This type is internal to a service that implements the generic differential drive contract.</remarks>
    /// <remarks>cancels long running commands (drive distance or rotate degrees)</remarks>
    /// </summary>
    [Description("Cancels a pending drive distance or rotate degrees operation.")]
    [DataContract]
    public class CancelPendingDriveOperation : Update<CancelPendingDriveOperationRequest, PortSet<DefaultUpdateResponseType, Fault>>
    {
        /// <summary>
        /// public constructor
        /// </summary>
        public CancelPendingDriveOperation()
        {
            Body = new CancelPendingDriveOperationRequest();
        }
    }

    #endregion
}
