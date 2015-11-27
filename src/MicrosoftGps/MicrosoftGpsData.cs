//-----------------------------------------------------------------------
//  This file is part of Microsoft Robotics Developer Studio Code Samples.
//
//  Copyright (C) Microsoft Corporation.  All rights reserved.
//
//  $File: MicrosoftGpsData.cs $ $Revision: 1 $
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;

using W3C.Soap;

using Microsoft.Dss.Core.Attributes;
using Microsoft.Dss.ServiceModel.Dssp;

namespace Microsoft.Robotics.Services.Sensors.Gps
{

    #region Gps State Type
    /// <summary>
    /// MicrosoftGps State
    /// </summary>
    [DataContract]
    [Description("The GPS sensor state")]
    public class GpsState
    {
        #region Private data members
        private bool _connected;
        private MicrosoftGpsConfig _gpsConfig;
        private GpGll _gpGll;
        private GpGga _gpGga;
        private GpGsa _gpGsa;
        private GpGsv _gpGsv;
        private GpRmc _gpRmc;
        private GpVtg _gpVtg;
        #endregion

        /// <summary>
        /// Is the Gps unit currently connected.
        /// </summary>
        [DataMember]
        [Description("Indicates that the GPS is connected.")]
        [Browsable(false)]
        public bool Connected
        {
            get { return _connected; }
            set { _connected = value; }
        }

        /// <summary>
        /// History
        /// </summary>
        [DataMember]
        [Browsable(false)]
        [Description("Identifies the history of the GPS sensors.")]
        public List<EarthCoordinates> History = new List<EarthCoordinates>();

        /// <summary>
        /// Serial Port Configuration
        /// </summary>
        [DataMember(IsRequired = true)]
        [Description("Specifies the serial port where the GPS is connected.")]
        public MicrosoftGpsConfig MicrosoftGpsConfig
        {
            get
            {
                return this._gpsConfig;
            }
            set
            {
                this._gpsConfig = value;
            }
        }

        /// <summary>
        /// Altitude and backup position
        /// </summary>
        [DataMember(IsRequired = false)]
        [Browsable(false)]
        [Description("Indicates altitude and backup position.")]
        public GpGga GpGga
        {
            get { return _gpGga; }
            set { _gpGga = value; }
        }

        /// <summary>
        /// Primary Position
        /// </summary>
        [DataMember(IsRequired = false)]
        [Browsable(false)]
        [Description("Indicates the primary position.")]
        public GpGll GpGll
        {
            get
            {
                return this._gpGll;
            }
            set
            {
                this._gpGll = value;
            }
        }

        /// <summary>
        /// Precision
        /// </summary>
        [DataMember(IsRequired = false)]
        [Browsable(false)]
        [Description("Indicates the GPS receiver operating mode, satellites used in the position solution, and DOP values.")]
        public GpGsa GpGsa
        {
            get
            {
                return this._gpGsa;
            }
            set
            {
                this._gpGsa = value;
            }
        }

        /// <summary>
        /// Satellites
        /// </summary>
        [DataMember(IsRequired = false)]
        [Browsable(false)]
        [Description("Indicates the number of GPS satellites in view, satellite ID numbers, elevation, azimuth, and SNR values.")]
        public GpGsv GpGsv
        {
            get
            {
                return this._gpGsv;
            }
            set
            {
                this._gpGsv = value;
            }
        }

        /// <summary>
        /// Backup course, speed, and position
        /// </summary>
        [DataMember(IsRequired = false)]
        [Browsable(false)]
        [Description("Indicates the time, date, position, course and speed data.")]
        public GpRmc GpRmc
        {
            get
            {
                return this._gpRmc;
            }
            set
            {
                this._gpRmc = value;
            }
        }

        /// <summary>
        /// Ground Speed and Course
        /// </summary>
        [DataMember(IsRequired = false)]
        [Browsable(false)]
        [Description("Indicates the course and speed information relative to the ground.")]
        public GpVtg GpVtg
        {
            get
            {
                return this._gpVtg;
            }
            set
            {
                this._gpVtg = value;
            }
        }

    }

    #endregion

    #region Gps Configuration

    /// <summary>
    /// MicrosoftGps Serial Port Configuration
    /// </summary>
    [DataContract]
    [DisplayName("(User) MicrosoftGps Configuration")]
    [Description("MicrosoftGps Serial Port Configuration")]
    public class MicrosoftGpsConfig: ICloneable
    {
        #region Private data members
        private int _commPort;
        private string _portName;
        private int _baudRate;
        private bool _captureHistory;
        private bool _captureNmea;
        private bool _retrackNmea;
        private string _configurationStatus;
        #endregion

        /// <summary>
        /// The Serial Comm Port
        /// </summary>
        [DataMember]
        [Description("Gps COM Port")]
        public int CommPort
        {
            get { return this._commPort; }
            set { this._commPort = value; }
        }

        /// <summary>
        /// The Serial Port Name or the File name containing Gps readings
        /// </summary>
        [DataMember]
        [Description("The Serial Port Name or the File name containing Gps readings (Default blank)")]
        public string PortName
        {
            get { return this._portName; }
            set { this._portName = value; }
        }

        /// <summary>
        /// Baud Rate
        /// </summary>
        [DataMember]
        [Description("Gps Baud Rate")]
        public int BaudRate
        {
            get { return this._baudRate; }
            set { this._baudRate = value; }
        }


        /// <summary>
        /// Capture Gps Coordinate History
        /// </summary>
        [DataMember]
        [Description("Capture Gps Coordinate History")]
        public bool CaptureHistory
        {
            get { return _captureHistory; }
            set { _captureHistory = value; }
        }

        /// <summary>
        /// Capture Gps Nmea stream
        /// </summary>
        [DataMember]
        [Description("Capture Gps Nmea stream")]
        public bool CaptureNmea
        {
            get { return _captureNmea; }
            set { _captureNmea = value; }
        }

        /// <summary>
        /// Retrack/Simulate Gps Nmea stream
        /// </summary>
        [DataMember]
        [Description("Retrack Simulate Gps Nmea stream")]
        public bool RetrackNmea
        {
            get { return _retrackNmea; }
            set { _retrackNmea = value; }
        }

        /// <summary>
        /// Configuration Status
        /// </summary>
        [DataMember]
        [Browsable(false)]
        [Description("Gps Configuration Status")]
        public string ConfigurationStatus
        {
            get { return _configurationStatus; }
            set { _configurationStatus = value; }
        }

        /// <summary>
        /// Default Constructor
        /// </summary>
        public MicrosoftGpsConfig()
        {
            this.BaudRate = 4800;
            this.CaptureHistory = false;
            this.CaptureNmea = false;
            this.RetrackNmea = false;
            this.CommPort = 0;
            this.PortName = string.Empty;
        }

        #region ICloneable Members

        /// <summary>
        /// Clone MicrosoftGpsConfig
        /// </summary>
        /// <returns></returns>
        public object Clone()
        {

            MicrosoftGpsConfig config = new MicrosoftGpsConfig();
            config.BaudRate = this.BaudRate;
            config.CaptureHistory = this.CaptureHistory;
            config.CaptureNmea = this.CaptureNmea;
            config.RetrackNmea = this.RetrackNmea;
            config.CommPort = this.CommPort;
            config.PortName = this.PortName;
            return config;
        }

        #endregion
    }

    /// <summary>
    /// A Microsoft Gps Command
    /// <remarks>Use with SendMicrosoftGpsCommand()</remarks>
    /// </summary>
    [DataContract]
    [Description("A Microsoft Gps Command")]
    public class MicrosoftGpsCommand
    {
        private string _command;

        /// <summary>
        /// Command
        /// </summary>
        [DataMember]
        public string Command
        {
            get
            {
                return this._command;
            }
            set
            {
                this._command = value;
            }
        }
    }

    /// <summary>
    /// standard subscribe request type
    /// </summary>
    [DataContract]
    [Description("Standard Subscribe request")]
    public class SubscribeRequest : SubscribeRequestType
    {
        /// <summary>
        /// Which message types to subscribe to
        /// </summary>
        [DataMember]
        public GpsMessageType MessageTypes;

        /// <summary>
        /// Only subscribe to messages when IsValid is true
        /// </summary>
        [DataMember]
        public bool ValidOnly;
    }

    #endregion

    #region Gps Data Structures

    /// <summary>
    /// Time, position and fix type data
    /// </summary>
    [DataContract]
    public class GpGga
    {
        #region Private Members
        private System.DateTime _utcPosition;
        private double _latitude;
        private double _longitude;
        private PositionFixIndicator _positionFixIndicator;
        private int _satellitesUsed;
        private double _horizontalDilutionOfPrecision;
        private double _AltitudeMeters;
        private string _altitudeUnits;
        private string _geoIdSeparation;
        private string _geoIdSeparationUnits;
        private string _ageOfDiffCorr;
        private string _diffRefStationId;
        private bool _isValid;
        private System.DateTime _lastUpdate;
        #endregion

        /// <summary>
        /// Is Valid
        /// </summary>
        [DataMember]
        [Description("Indicates the GGA is valid.")]
        public bool IsValid
        {
            get
            {
                return this._isValid;
            }
            set
            {
                this._isValid = value;
            }
        }

        /// <summary>
        /// Last Update
        /// </summary>
        [DataMember]
        [Description("Indicates the time of the last reading update.")]
        public System.DateTime LastUpdate
        {
            get
            {
                return this._lastUpdate;
            }
            set
            {
                this._lastUpdate = value;
            }
        }

        /// <summary>
        /// UTC Position
        /// </summary>
        [DataMember]
        [Description("Identifies the UTC time of the reading.")]
        public System.DateTime UTCPosition
        {
            get
            {
                return this._utcPosition;
            }
            set
            {
                this._utcPosition = value;
            }
        }
        /// <summary>
        /// Latitude
        /// </summary>
        [DataMember]
        [Description("Indicates the latitude.")]
        public double Latitude
        {
            get
            {
                return this._latitude;
            }
            set
            {
                this._latitude = value;
            }
        }
        /// <summary>
        /// Longitude
        /// </summary>
        [DataMember]
        [Description("Indicates the longitude.")]
        public double Longitude
        {
            get
            {
                return this._longitude;
            }
            set
            {
                this._longitude = value;
            }
        }
        /// <summary>
        /// Position Fix Indicator
        /// </summary>
        [DataMember]
        [Description("Indicates the position fix indicator.")]
        public PositionFixIndicator PositionFixIndicator
        {
            get
            {
                return this._positionFixIndicator;
            }
            set
            {
                this._positionFixIndicator = value;
            }
        }
        /// <summary>
        /// Satellites Used
        /// </summary>
        [DataMember]
        [Description("Indicates the number of satellites used.")]
        public int SatellitesUsed
        {
            get
            {
                return this._satellitesUsed;
            }
            set
            {
                this._satellitesUsed = value;
            }
        }
        /// <summary>
        /// Horizontal Dilution Of Precision
        /// <remarks>Latitude and Longitude</remarks>
        /// </summary>
        [DataMember]
        [Description("Indicates the Horizontal Dilution Of Precision.")]
        public double HorizontalDilutionOfPrecision
        {
            get
            {
                return this._horizontalDilutionOfPrecision;
            }
            set
            {
                this._horizontalDilutionOfPrecision = value;
            }
        }
        /// <summary>
        /// Altitude.
        /// <remarks>No geoid corrections, values are WGS84 ellipsoid heights</remarks></summary>
        [DataMember]
        [Description("Indicates the altitude (m).\n(No geoid corrections, values are WGS84 ellipsoid heights.)")]
        public double AltitudeMeters
        {
            get
            {
                return this._AltitudeMeters;
            }
            set
            {
                this._AltitudeMeters = value;
            }
        }
        /// <summary>
        /// M-Meters
        /// </summary>
        [DataMember]
        [Description("Indicates the altitude units (m = meters).")]
        public string AltitudeUnits
        {
            get
            {
                return this._altitudeUnits;
            }
            set
            {
                this._altitudeUnits = value;
            }
        }
        /// <summary>
        /// Geo Id Separation
        /// </summary>
        [DataMember]
        [Description("Indicates Geo Id separation.")]
        public string GeoIdSeparation
        {
            get
            {
                return this._geoIdSeparation;
            }
            set
            {
                this._geoIdSeparation = value;
            }
        }
        /// <summary>
        /// M - Meters
        /// </summary>
        [DataMember]
        [Description("Indicates Geo Id separation units (m = meters).")]
        public string GeoIdSeparationUnits
        {
            get
            {
                return this._geoIdSeparationUnits;
            }
            set
            {
                this._geoIdSeparationUnits = value;
            }
        }
        /// <summary>
        /// Null when DGPS not used
        /// </summary>
        [DataMember]
        [Description("Indicates Age of Differential Correction.\n(Null when DGPS not used.)")]
        public string AgeOfDifferentialCorrection
        {
            get
            {
                return this._ageOfDiffCorr;
            }
            set
            {
                this._ageOfDiffCorr = value;
            }
        }
        /// <summary>
        /// Diff Ref Station Id
        /// </summary>
        [DataMember]
        [Description("Indicates Diff Ref Station Id.")]
        public string DiffRefStationId
        {
            get
            {
                return this._diffRefStationId;
            }
            set
            {
                this._diffRefStationId = value;
            }
        }
    }

    /// <summary>
    /// Latitude, longitude, UTC time of position fix and status
    /// </summary>
    [DataContract]
    [Description("Indicates latitude, longitude, UTC time of position fix and status.")]
    public class GpGll
    {
        #region Private Members
        private bool _isValid;
        private System.DateTime _lastUpdate;
        private double _latitude;
        private double _longitude;
        private string _status;
        private System.DateTime _gllTime;
        private string _marginOfError;
        #endregion

        /// <summary>
        /// Status == "A"
        /// </summary>
        [DataMember]
        [Description("Indicates the GLL is valid.")]
        public bool IsValid
        {
            get
            {
                return this._isValid;
            }
            set
            {
                this._isValid = value;
            }
        }
        /// <summary>
        /// Last Update
        /// </summary>
        [DataMember]
        [Description("Indicates the time of the last reading update.")]
        public System.DateTime LastUpdate
        {
            get
            {
                return this._lastUpdate;
            }
            set
            {
                this._lastUpdate = value;
            }
        }

        /// <summary>
        /// Latitude
        /// </summary>
        [DataMember]
        [Description("Indicates the latitude.")]
        public double Latitude
        {
            get
            {
                return this._latitude;
            }
            set
            {
                this._latitude = value;
            }
        }
        /// <summary>
        /// Longitude
        /// </summary>
        [DataMember]
        [Description("Indicates the longitude.")]
        public double Longitude
        {
            get
            {
                return this._longitude;
            }
            set
            {
                this._longitude = value;
            }
        }
        /// <summary>
        /// Status
        /// </summary>
        [DataMember]
        [Description("Indicates the GLL status.")]
        public string Status
        {
            get
            {
                return this._status;
            }
            set
            {
                this._status = value;
            }
        }
        /// <summary>
        /// gll Time
        /// </summary>
        [DataMember]
        [Description("Indicates the GLL time.")]
        public System.DateTime GllTime
        {
            get
            {
                return this._gllTime;
            }
            set
            {
                this._gllTime = value;
            }
        }
        /// <summary>
        /// Margin Of Error
        /// </summary>
        [DataMember]
        [Description("Indicates the Margin of Error.")]
        public string MarginOfError
        {
            get
            {
                return this._marginOfError;
            }
            set
            {
                this._marginOfError = value;
            }
        }
    }

    /// <summary>
    /// GPS receiver operating mode, satellites used in the position solution, and DOP values
    /// </summary>
    [DataContract]
    [Description("Indicates GPS receiver operating mode, satellites used in the position solution, and DOP values")]
    public class GpGsa
    {
        #region Private Members
        private bool _isValid;
        private System.DateTime _lastUpdate;
        private string _status;
        private string _autoManual;
        private GsaMode _mode;
        private double _sphericalDilutionOfPrecision;
        private double _horizontalDilutionOfPrecision;
        private double _verticalDilutionOfPrecision;
        internal int[] _satelliteUsed = new int[12];
        #endregion

        /// <summary>
        /// Gsa Is Valid
        /// </summary>
        [DataMember]
        [Description("Indicates the GSA is valid.")]
        public bool IsValid
        {
            get
            {
                return this._isValid;
            }
            set
            {
                this._isValid = value;
            }
        }

        /// <summary>
        /// Gsa Last Update
        /// </summary>
        [DataMember]
        [Description("Indicates the time of the last reading update.")]
        public System.DateTime LastUpdate
        {
            get
            {
                return this._lastUpdate;
            }
            set
            {
                this._lastUpdate = value;
            }
        }

        /// <summary>
        /// Gsa Status
        /// </summary>
        [DataMember]
        [Description("Indicates the GSA status.")]
        public string Status
        {
            get
            {
                return this._status;
            }
            set
            {
                this._status = value;
            }
        }

        /// <summary>
        /// Gsa Auto Manual
        /// </summary>
        [DataMember]
        [Description("Indicates whether GSA Auto or Manual is set.")]
        public string AutoManual
        {
            get
            {
                return this._autoManual;
            }
            set
            {
                this._autoManual = value;
            }
        }

        /// <summary>
        /// Gsa Mode
        /// </summary>
        [DataMember]
        [Description("Indicates the GSA mode.")]
        public GsaMode Mode
        {
            get
            {
                return this._mode;
            }
            set
            {
                this._mode = value;
            }
        }

        /// <summary>
        /// Gsa Spherical Dilution Of Precision
        /// <remarks>Accuracy of the 3-D coordinates</remarks>
        /// </summary>
        [DataMember]
        [Description("Indicates the GSA Spherical Dilution Of Precision (accuracy of the 3-D coordinates).")]
        public double SphericalDilutionOfPrecision
        {
            get
            {
                return this._sphericalDilutionOfPrecision;
            }
            set
            {
                this._sphericalDilutionOfPrecision = value;
            }
        }

        /// <summary>
        /// Gsa Horizontal Dilution Of Precision
        /// <remarks>Latitude and Longitude</remarks>
        /// </summary>
        [DataMember]
        [Description("Indicates the GSA Horizontal Dilution Of Precision (latitude and longitude).")]
        public double HorizontalDilutionOfPrecision
        {
            get
            {
                return this._horizontalDilutionOfPrecision;
            }
            set
            {
                this._horizontalDilutionOfPrecision = value;
            }
        }

        /// <summary>
        /// Gsa Vertical Dilution Of Precision (Altitude)
        /// </summary>
        [DataMember]
        [Description("Indicates the GSA Vertical Dilution Of Precision (altitude).")]
        public double VerticalDilutionOfPrecision
        {
            get
            {
                return this._verticalDilutionOfPrecision;
            }
            set
            {
                this._verticalDilutionOfPrecision = value;
            }
        }

        /// <summary>
        /// Satellite Used
        /// </summary>
        [DataMember]
#if DEBUG
        [SuppressMessage("Microsoft.Usage", "CA2227")]
#endif
        [Description("Identifies the set of satelites used.")]
        public List<int> SatelliteUsed
        {
            get { return new List<int>(_satelliteUsed); }
            set { _satelliteUsed = value.ToArray(); }
        }
    }

    /// <summary>
    /// The number of GPS satellites in view, satellite ID numbers, elevation, azimuth, and SNR values.
    /// </summary>
    [DataContract]
    [Description("Indicates the number of GPS satellites in view, satellite ID numbers, elevation, azimuth, and SNR values.")]
    public class GpGsv
    {
        #region Private Members
        private System.DateTime _lastUpdate;
        private bool _isValid;
        private int _satellitesInView;
        internal GsvSatellite[] _gsvSatellites;
        #endregion

        /// <summary>
        /// Gsv Is Valid
        /// </summary>
        [DataMember]
        [Description("Indicates the GSV is valid.")]
        public bool IsValid
        {
            get
            {
                return this._isValid;
            }
            set
            {
                this._isValid = value;
            }
        }

        /// <summary>
        /// Gsv Last Update
        /// </summary>
        [DataMember]
        [Description("Indicates the time of the last reading update.")]
        public System.DateTime LastUpdate
        {
            get
            {
                return this._lastUpdate;
            }
            set
            {
                this._lastUpdate = value;
            }
        }

        /// <summary>
        /// Number of Gsv Satellites In View
        /// </summary>
        [DataMember]
        [Description("Indicates the number of GSV satellites in view.")]
        public int SatellitesInView
        {
            get
            {
                return this._satellitesInView;
            }
            set
            {
                this._satellitesInView = value;
            }
        }

        /// <summary>
        /// Satellites in View
        /// </summary>
        [DataMember]
#if DEBUG
        [SuppressMessage("Microsoft.Usage", "CA2227", Justification = "DataMember's can not be read-only")]
#endif
        [Description("Identifies the satellites in view.")]
        public List<GsvSatellite> GsvSatellites
        {
            get { return new List<GsvSatellite>(_gsvSatellites); }
            set { _gsvSatellites = value.ToArray(); }
        }
    }

    /// <summary>
    /// Info about an individual satellite
    /// </summary>
    [DataContract]
    [Description("Identifies information about an individual satellite.")]
    public class GsvSatellite
    {
        /// <summary>
        /// Satellite Id
        /// </summary>
        [DataMember]
        [Description("Indicates the satellite Id.")]
        public int Id;

        /// <summary>
        /// Elevation in degrees
        /// <remarks>Maximum 90</remarks>
        /// </summary>
        [DataMember]
        [Description("Indicates the elevation in degrees (maximum 90).")]
        public int ElevationDegrees;

        /// <summary>
        /// Azimuth in degrees
        /// <remarks>Range 0-359</remarks>
        /// </summary>
        [DataMember]
        [Description("Indicates the azimuth in degrees (range 0-359).")]
        public int AzimuthDegrees;

        /// <summary>
        /// Signal to Noise Ratio
        /// <remarks>Range 0-99, -1 when not tracking</remarks>
        /// </summary>
        [DataMember]
        [Description("Indicates the Signal-to-Noise-Ratio (range 0-99, -1 when not tracking).")]
        public int SignalToNoiseRatio;
    }

    /// <summary>
    /// Time, date, position, course and speed data
    /// </summary>
    [DataContract]
    [Description("Indicates the time, date, position, course and speed data.")]
    public class GpRmc
    {
        #region Private Members
        private bool _isValid;
        private System.DateTime _lastUpdate;
        private System.DateTime _dateTime;
        private string _status;
        private double _latitude;
        private double _longitude;
        private double _speedMetersPerSecond;
        private double _courseDegrees;
        #endregion

        /// <summary>
        /// Rmc Is Valid
        /// </summary>
        [DataMember]
        [Description("Indicates the RMC is valid.")]
        public bool IsValid
        {
            get
            {
                return this._isValid;
            }
            set
            {
                this._isValid = value;
            }
        }

        /// <summary>
        /// Rmc Last Update
        /// </summary>
        [DataMember]
        [Description("Indicates the time of the last reading update.")]
        public System.DateTime LastUpdate
        {
            get
            {
                return this._lastUpdate;
            }
            set
            {
                this._lastUpdate = value;
            }
        }

        /// <summary>
        /// Rmc Date Time
        /// </summary>
        [DataMember]
        [Description("Indicates the RMC time.")]
        public System.DateTime DateTime
        {
            get
            {
                return this._dateTime;
            }
            set
            {
                this._dateTime = value;
            }
        }
        /// <summary>
        /// Rmc Status
        /// </summary>
        [DataMember]
        [Description("Indicates the RMC status.")]
        public string Status
        {
            get
            {
                return this._status;
            }
            set
            {
                this._status = value;
            }
        }
        /// <summary>
        /// Rmc Latitude
        /// </summary>
        [DataMember]
        [Description("Indicates the latitude.")]
        public double Latitude
        {
            get
            {
                return this._latitude;
            }
            set
            {
                this._latitude = value;
            }
        }
        /// <summary>
        /// Rmc Longitude
        /// </summary>
        [DataMember]
        [Description("Indicates the longitude.")]
        public double Longitude
        {
            get
            {
                return this._longitude;
            }
            set
            {
                this._longitude = value;
            }
        }
        /// <summary>
        /// Rmc Speed Meters Per Second
        /// </summary>
        [DataMember]
        [Description("Indicates the RMC speed (meters per second).")]
        public double SpeedMetersPerSecond
        {
            get
            {
                return this._speedMetersPerSecond;
            }
            set
            {
                this._speedMetersPerSecond = value;
            }
        }
        /// <summary>
        /// Rmc Course Degrees
        /// </summary>
        [DataMember]
        [Description("Indicates the RMC course degrees.")]
        public double CourseDegrees
        {
            get
            {
                return this._courseDegrees;
            }
            set
            {
                this._courseDegrees = value;
            }
        }
    }

    /// <summary>
    /// Course and speed information relative to the ground
    /// </summary>
    [DataContract]
    [Description("Indicates the course and speed information relative to the ground.")]
    public class GpVtg
    {
        #region Private Members
        private bool _isValid;
        private System.DateTime _lastUpdate;
        private double _speedMetersPerSecond;
        private double _courseDegrees;
        #endregion

        /// <summary>
        /// Vtg Is Valid
        /// </summary>
        [DataMember]
        [Description("Indicates that the VTG is valid.")]
        public bool IsValid
        {
            get
            {
                return this._isValid;
            }
            set
            {
                this._isValid = value;
            }
        }

        /// <summary>
        /// Vtg Last Update
        /// </summary>
        [DataMember]
        [Description("Indicates the time of the last reading update.")]
        public System.DateTime LastUpdate
        {
            get
            {
                return this._lastUpdate;
            }
            set
            {
                this._lastUpdate = value;
            }
        }

        /// <summary>
        /// Speed Meters Per Second
        /// </summary>
        [DataMember]
        [Description("Indicates the speed (meters per second).")]
        public double SpeedMetersPerSecond
        {
            get
            {
                return this._speedMetersPerSecond;
            }
            set
            {
                this._speedMetersPerSecond = value;
            }
        }
        /// <summary>
        /// Course Degrees
        /// </summary>
        [DataMember]
        [Description("Indicates the course degrees.")]
        public double CourseDegrees
        {
            get
            {
                return this._courseDegrees;
            }
            set
            {
                this._courseDegrees = value;
            }
        }
    }
    #endregion

    #region Gps Enums

    /// <summary>
    /// Position Fix Indicator (Fix quality)
    ///             0 = invalid
    ///             1 = GPS fix (SPS)
    ///             2 = DGPS fix
    ///             3 = PPS fix
    ///             4 = Real Time Kinematic
    ///             5 = Float RTK
    ///             6 = estimated (dead reckoning) (2.3 feature)
    ///             7 = Manual input mode
    ///             8 = Simulation mode
    /// </summary>
    [DataContract]
    [Description("Identifies the Position Fix Indicator settings.")]
    public enum PositionFixIndicator
    {
        /// <summary>
        /// Fix Not Available or invalid
        /// </summary>
        [DataMember]
        FixNotAvailable = 0,
        /// <summary>
        /// GpsSPS Mode - basic fix available
        /// </summary>
        [DataMember]
        GpsSPSMode = 1,
        /// <summary>
        /// Differential GpsSPS Mode - fix available
        /// </summary>
        [DataMember]
        GpsDifferentialSPSMode = 2,
        /// <summary>
        /// GpsPPS Mode - fix available
        /// </summary>
        [DataMember]
        GpsPPSMode = 3,
        /// <summary>
        /// Real Time Kinematic Mode - fix available
        /// </summary>
        [DataMember]
        GpsRTKMode = 4,
        /// <summary>
        /// Float RTK Mode - fix available
        /// </summary>
        [DataMember]
        GpsFloatRTKMode = 5,
        /// <summary>
        /// Estimated (dead reckoning) Mode - estimated fix available (2.3 feature)
        /// </summary>
        [DataMember]
        GpsDeadReckoningMode = 6,
        /// <summary>
        /// Manual input mode - fix available
        /// </summary>
        [DataMember]
        GpsManualInputMode = 7,
        /// <summary>
        /// Simulation Mode - fix available
        /// </summary>
        [DataMember]
        GpsSimulationMode = 8
    }

    /// <summary>
    /// Gps Message Type
    /// </summary>
    [DataContract,Flags]
    [Description("Identifies the NMEA message types.")]
    public enum GpsMessageType
    {
        /// <summary>
        /// No message
        /// </summary>
        None = 0,

        /// <summary>
        /// Altitude and backup position
        /// </summary>
        GPGGA = 0x01,

        /// <summary>
        /// Primary Position
        /// </summary>
        GPGLL = 0x02,

        /// <summary>
        /// Precision
        /// </summary>
        GPGSA = 0x04,

        /// <summary>
        /// Satellites
        /// </summary>
        GPGSV = 0x08,

        /// <summary>
        /// Backup course, speed, and position
        /// </summary>
        GPRMC = 0x10,

        /// <summary>
        /// Ground Speed and Course
        /// </summary>
        GPVTG = 0x20,

        /// <summary>
        /// All message types
        /// </summary>
        All = GPGGA | GPGLL | GPGSA | GPGSV | GPRMC | GPVTG
    }

    /// <summary>
    /// Gsa Mode
    /// </summary>
    [DataContract]
    [Description("Identifies the GSA mode.")]
    public enum GsaMode
    {
        /// <summary>
        /// Default value
        /// </summary>
        [DataMember]
        None = 0,

        /// <summary>
        /// No Fix
        /// </summary>
        [DataMember]
        NoFix = 1,

        /// <summary>
        /// Fix2D
        /// </summary>
        [DataMember]
        Fix2D = 2,

        /// <summary>
        /// Fix3D
        /// </summary>
        [DataMember]
        Fix3D = 3,
    }



    #endregion

}
