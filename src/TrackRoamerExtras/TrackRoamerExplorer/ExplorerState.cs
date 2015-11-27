//-----------------------------------------------------------------------
//  This file is part of Microsoft Robotics Developer Studio Code Samples.
//
//  Copyright (C) Microsoft Corporation.  All rights reserved.
//
//  $File: ExplorerState.cs $ $Revision: 1 $
//-----------------------------------------------------------------------

using Microsoft.Dss.Core.Attributes;

using System;
using System.Collections.Generic;
using System.ComponentModel;

using sicklrf = Microsoft.Robotics.Services.Sensors.SickLRF.Proxy;
using drive = Microsoft.Robotics.Services.Drive.Proxy;

namespace TrackRoamer.Robotics.Services.TrackRoamerExplorer
{
    [DataContract]
    public class State
    {
        #region private fields
        private int _countdown;
        private LogicalState _logicalState;
        private int _newHeading;
        private int _velocity;
        private sicklrf.State _south;
        private bool _mapped;
        private DateTime _mostRecentLaser = DateTime.Now;
        private drive.DriveDifferentialTwoWheelState _driveState;
        #endregion

        #region data members
        [DataMember(IsRequired=false)]
        [Description("The last known state of the drive.")]
        public drive.DriveDifferentialTwoWheelState DriveState
        {
            get { return _driveState; }
            set { _driveState = value; }
        }

        [DataMember]
        [Description("Identifies the countdown to the next state change.")]
        public int Countdown
        {
            get { return _countdown; }
            set { _countdown = value; }
        }

        [DataMember]
        [Description("Identifies the logical state of the explorer.")]
        public LogicalState LogicalState
        {
            get { return _logicalState; }
            set { _logicalState = value; }
        }

        [DataMember]
        [Description("Identifies the new heading set for the drive.")]
        public int NewHeading
        {
            get { return _newHeading; }
            set { _newHeading = value; }
        }

        [DataMember]
        [Description("Identifies the velocity set for the drive.")]
        public int Velocity
        {
            get { return _velocity; }
            set { _velocity = value; }
        }

        [DataMember]
        [Description("Identifies the last known laser reading in the opposite direction.")]
        public sicklrf.State South
        {
            get { return _south; }
            set { _south = value; }
        }

        [DataMember]
        [Description("Identifies whether a full sweep scan of the environment has been completed.")]
        public bool Mapped
        {
            get { return _mapped; }
            set { _mapped = value; }
        }

        [DataMember]
        [Description("Identifies the time of the most recent laser reading.")]
        public DateTime MostRecentLaser
        {
            get { return _mostRecentLaser; }
            set { _mostRecentLaser = value; }
        }
        #endregion

        #region internal helper accessors for meta states
        internal bool IsMapping
        {
            get
            {
                return
                    LogicalState == LogicalState.RandomTurn ||
                    LogicalState == LogicalState.MapNorth ||
                    LogicalState == LogicalState.MapSouth ||
                    LogicalState == LogicalState.MapSurroundings;
            }
        }

        internal bool IsUnknown
        {
            get
            {
                return LogicalState == LogicalState.Unknown;
            }
        }

        internal bool IsMoving
        {
            get
            {
                return IsActive && !IsMapping;
            }
        }

        internal bool IsActive
        {
            get
            {
                return !IsUnknown;
            }
        }
        #endregion
    }

    [DataContract]
    public enum LogicalState
    {
        Unknown,
        RandomTurn,
        AdjustHeading,
        FreeForwards,
        MapSurroundings,
        MapNorth,
        MapSouth,
    }
}