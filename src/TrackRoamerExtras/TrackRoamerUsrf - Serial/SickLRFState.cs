//-----------------------------------------------------------------------
//  This file is part of Microsoft Robotics Developer Studio Code Samples.
//
//  Copyright (C) Microsoft Corporation.  All rights reserved.
//
//  $File: SickLRFState.cs $ $Revision: 1 $
//-----------------------------------------------------------------------

using System;
using System.ComponentModel;
using Microsoft.Dss.Core.Attributes;


namespace TrackRoamer.Robotics.Services.TrackRoamerUsrf
{
    /// <summary>
    /// Used to hold state data for the SickLRF service.
    /// </summary>
    [DataContract]
    [Description("The state of the TrackRoamer Ultrasound range finder (URF).")]
    public class State
    {
        #region private fields
        private string _description;
        private int[] _distanceMeasurements;
        private int _angularRange;
        private double _angularResolution;
        private Units _units;
        private DateTime _timeStamp;
        private string _linkState;
        private int _comPort;
        #endregion

        #region data members
        /// <summary>
        /// Description of the SickLRF device returned at Power On.
        /// </summary>
        [DataMember, Browsable(false)]
        [Description("Description of the TrackRoamer Ultrasound device.")]
        public string Description
        {
            get { return _description; }
            set { _description = value; }
        }

        /// <summary>
        /// Array of distance readings.
        /// </summary>
        [DataMember, Browsable(false)]
        [Description("The set of distance measurements returned.")]
        public int[] DistanceMeasurements
        {
            get { return _distanceMeasurements; }
            set { _distanceMeasurements = value; }
        }

        /// <summary>
        /// Angular range of the measurement.
        /// </summary>
        [DataMember, Browsable(false)]
        [Description("The angular range of the measurement.")]
        public int AngularRange
        {
            get { return _angularRange; }
            set { _angularRange = value; }
        }

        /// <summary>
        /// Angular resolution of a given reading.
        /// </summary>
        [DataMember, Browsable(false)]
        [Description("The angular resolution of the measurement.")]
        public double AngularResolution
        {
            get { return _angularResolution; }
            set { _angularResolution = value; }
        }

        /// <summary>
        /// The units of the data in <paramref name="DistanceMeasurements"/>.
        /// </summary>
        [DataMember, Browsable(false)]
        [Description("Units used for the distance measurements.")]
        public Units Units
        {
            get { return _units; }
            set { _units = value; }
        }

        /// <summary>
        /// Time at which the SickLRF unit sent these measurements over the serial link.
        /// </summary>
        [DataMember, Browsable(false)]
        [Description("The time at which the measurements were sent.")]
        public DateTime TimeStamp
        {
            get { return _timeStamp; }
            set { _timeStamp = value; }
        }

        /// <summary>
        /// Status of the communications link to the SickLRF unit.
        /// </summary>
        [DataMember, Browsable(false)]
        [Description("Status of the communications link.")]
        public string LinkState
        {
            get { return _linkState; }
            set { _linkState = value; }
        }

        /// <summary>
        /// Serial port to use for connection to the range finder.
        /// </summary>
        [DataMember]
        [Description("Com port to use for connection to the range finder")]
        public int ComPort
        {
            get { return _comPort; }
            set { _comPort = value; }
        }
        #endregion
    }
}