//-----------------------------------------------------------------------
//  This file is part of Microsoft Robotics Developer Studio Code Samples.
//
//  Copyright (C) Microsoft Corporation.  All rights reserved.
//
//  $File: ExplorerTypes.cs $ $Revision: 1 $
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.ComponentModel;

using Microsoft.Ccr.Core;
using Microsoft.Dss.ServiceModel.Dssp;
using Microsoft.Dss.Core.Attributes;
using dssp = Microsoft.Dss.ServiceModel.Dssp;
using W3C.Soap;

using bumper = Microsoft.Robotics.Services.ContactSensor.Proxy;
using drive = Microsoft.Robotics.Services.Drive.Proxy;
using sicklrf = Microsoft.Robotics.Services.Sensors.SickLRF.Proxy;

namespace TrackRoamer.Robotics.Services.TrackRoamerExplorer
{

    /// <summary>
    /// DSS Contract for Explorer
    /// </summary>
    static class Contract
    {
        /// <summary>
        /// The DSS Namespace for Explorer
        /// </summary>
		public const string Identifier = "http://schemas.trackroamer.com/robotics/2009/04/trackroamerexplorer.html";
    }

    /// <summary>
    /// The Explorer Operations Port
    /// </summary>
    class ExplorerOperations : PortSet<
        DsspDefaultLookup,
        DsspDefaultDrop,
        Get,
        BumperUpdate,
        BumpersUpdate,
        DriveUpdate,
        LaserRangeFinderUpdate,
        LaserRangeFinderResetUpdate,
        WatchDogUpdate
        >
    {
    }

    [Description("Provides an update to a bumper's state.")]
    class BumperUpdate : Update<bumper.ContactSensor, PortSet<DefaultUpdateResponseType, Fault>>
    {
        public BumperUpdate(bumper.ContactSensor body)
            : base(body)
        { }

        public BumperUpdate()
        {}
    }

    [Description("Provides an update to the set of bumpers' state.")]
    class BumpersUpdate : Update<bumper.ContactSensorArrayState, PortSet<DefaultUpdateResponseType, Fault>>
    {
        public BumpersUpdate(bumper.ContactSensorArrayState body)
            : base(body)
        { }

        public BumpersUpdate()
        {}
    }

    [Description("Provides an update to the drive's state.")]
    class DriveUpdate : Update<drive.DriveDifferentialTwoWheelState, PortSet<DefaultUpdateResponseType, Fault>>
    {
        public DriveUpdate(drive.DriveDifferentialTwoWheelState body)
            : base(body)
        { }

        public DriveUpdate()
        {}
    }

    [Description("Provides an update to the laser range finder state.")]
    class LaserRangeFinderUpdate : Update<sicklrf.State, PortSet<DefaultUpdateResponseType, Fault>>
    {
        public LaserRangeFinderUpdate(sicklrf.State body)
            : base(body)
        { }

        public LaserRangeFinderUpdate()
        {}
    }

    [Description("Provides an update for the laser range finder reset request.")]
    class LaserRangeFinderResetUpdate : Update<sicklrf.ResetType, PortSet<DefaultUpdateResponseType, Fault>>
    {
        public LaserRangeFinderResetUpdate(sicklrf.ResetType body)
            : base(body)
        { }

        public LaserRangeFinderResetUpdate()
        {}
    }

    [Description("Provides an update to the watchdog state.")]
    class WatchDogUpdate : Update<WatchDogUpdateRequest, PortSet<DefaultUpdateResponseType, Fault>>
    {
        public WatchDogUpdate(WatchDogUpdateRequest body)
            : base(body)
        { }

        public WatchDogUpdate()
        {}
    }

    [DataContract]
    public class WatchDogUpdateRequest
    {
        private DateTime _timeStamp;

        [DataMember]
        [Description("Indicates the time of the reading (in ms).")]
        public DateTime TimeStamp
        {
            get { return _timeStamp; }
            set { _timeStamp = value; }
        }

        public WatchDogUpdateRequest(DateTime timeStamp)
        {
            TimeStamp = timeStamp;
        }

        public WatchDogUpdateRequest()
        { }
    }

    /// <summary>
    /// DSS Get Definition for Explorer
    /// </summary>
    [Description("Gets the current state of the Explorer service.")]
    class Get : Get<dssp.GetRequestType, PortSet<State, W3C.Soap.Fault>>
    {
    }
}
