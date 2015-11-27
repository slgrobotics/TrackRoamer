//-----------------------------------------------------------------------
//  This file is part of Microsoft Robotics Developer Studio Code Samples.
//
//  Copyright (C) Microsoft Corporation.  All rights reserved.
//
//  $File: MicrosoftGps.cs $ $Revision: 1 $
//-----------------------------------------------------------------------

//#define TRACEDATA

using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Drawing;
using System.Drawing.Imaging;
using System.Globalization;
using System.IO;
using System.Net;
using System.Net.Mime;
using System.Security.Permissions;
using W3C.Soap;

using Microsoft.Ccr.Core;
using Microsoft.Dss.Core;
using Microsoft.Dss.Core.Attributes;
using Microsoft.Dss.Core.DsspHttp;
using Microsoft.Dss.Core.DsspHttpUtilities;
using Microsoft.Dss.ServiceModel.Dssp;
using Microsoft.Dss.ServiceModel.DsspServiceBase;
using Microsoft.Robotics.Services.Sensors.Gps.Properties;

using submgr = Microsoft.Dss.Services.SubscriptionManager;


namespace Microsoft.Robotics.Services.Sensors.Gps
{

    /// <summary>
    /// Microsoft Gps Service Implementation
    /// </summary>
    [Contract(Contract.Identifier)]
    [DisplayName("(User) Microsoft GPS-360")]
    [Description("Provides GPS data in NMEA 0183 output format.")]
    public class MicrosoftGpsService : DsspServiceBase
    {
        private bool simulateFromLogNmea = false;

        [EmbeddedResource("Microsoft.Robotics.Services.Sensors.Gps.MicrosoftGps.user.xslt")]
        private string _transformGpsData = null;

        [EmbeddedResource("Microsoft.Robotics.Services.Sensors.Gps.MicrosoftGpsMap.user.xslt")]
        private string _transformGpsMap = null;

        private const double _knotsToMetersPerSecond = 0.514444444;
        private const string _root = "/MicrosoftGps";
        private const string _showTop = "/top";
        private const string _showMap = "/map";
        private const string _configFile = ServicePaths.Store + "/MicrosoftGps.config.xml";

        const string Tag_GpGga = "GPGGA";
        const string Tag_GpGsa = "GPGSA";
        const string Tag_GpGll = "GPGLL";
        const string Tag_GpGsv = "GPGSV";
        const string Tag_GpRmc = "GPRMC";
        const string Tag_GpVtg = "GPVTG";

        /// <summary>
        /// The Microsoft Gps main port
        /// </summary>
        [ServicePort(_root, AllowMultipleInstances = true)]
#if DEBUG
        [SuppressMessage("Microsoft.Performance", "CA1805", Justification = "ServicePort requires field initialization")]
#endif
        private MicrosoftGpsOperations _mainPort = new MicrosoftGpsOperations();

        /// <summary>
        /// The Microsoft Gps current state
        /// </summary>
        [InitialStatePartner(Optional = true, ServiceUri = _configFile)]
        private GpsState _state = new GpsState();

        /// <summary>
        /// The subscription manager which keeps track of subscriptions to our Gps data
        /// </summary>
        [Partner("SubMgr", Contract = submgr.Contract.Identifier, CreationPolicy = PartnerCreationPolicy.CreateAlways)]
        submgr.SubscriptionManagerPort _subMgrPort = new submgr.SubscriptionManagerPort();

        /// <summary>
        /// Communicate with the Gps hardware
        /// </summary>
        private GpsConnection _gpsConnection = null;

        /// <summary>
        /// A CCR port for receiving Gps data
        /// </summary>
        private GpsDataPort _gpsDataPort = new GpsDataPort();

        /// <summary>
        /// Http helpers
        /// </summary>
        private DsspHttpUtilitiesPort _httpUtilities = new DsspHttpUtilitiesPort();

        /// <summary>
        /// Default Constructor
        /// </summary>
        public MicrosoftGpsService(DsspServiceCreationPort creationPort) :
                base(creationPort)
        {
        }

        /// <summary>
        /// Service Start
        /// </summary>
        protected override void Start()
        {
            if (_state == null)
            {
                _state = new GpsState();
                _state.MicrosoftGpsConfig = new MicrosoftGpsConfig();
                _state.MicrosoftGpsConfig.CommPort = 0;
                _state.MicrosoftGpsConfig.CaptureHistory = true;
                _state.MicrosoftGpsConfig.CaptureNmea = true;
                _state.MicrosoftGpsConfig.RetrackNmea = false;

                SaveState(_state);
            }
            else
            {
                // Clear old Gps readings
                _state.GpGga = null;
                _state.GpGll = null;
                _state.GpGsa = null;
                _state.GpGsv = null;
                _state.GpRmc = null;
                _state.GpVtg = null;
            }

            _httpUtilities = DsspHttpUtilitiesService.Create(Environment);

            if (_state.MicrosoftGpsConfig == null)
                _state.MicrosoftGpsConfig = new MicrosoftGpsConfig();

            // Publish the service to the local Node Directory
            DirectoryInsert();

            if (simulateFromLogNmea)
            {
                SpawnIterator(SimulateMicrosoftGps);
            }
            else
            {
                _gpsConnection = new GpsConnection(_state.MicrosoftGpsConfig, _gpsDataPort);

                setCaptureNmea();

                SpawnIterator(ConnectToMicrosoftGps);
            }

            // Listen on the main port for requests and call the appropriate handler.
            Interleave mainInterleave = ActivateDsspOperationHandlers();

            mainInterleave.CombineWith(new Interleave(new ExclusiveReceiverGroup(
                Arbiter.Receive<string[]>(true, _gpsDataPort, DataReceivedHandler)
                ,Arbiter.Receive<Exception>(true, _gpsDataPort, ExceptionHandler)
#if TRACEDATA
                // see Info messages with data strings coming from GPS at http://localhost:50000/console/output
                ,Arbiter.Receive<string>(true, _gpsDataPort, MessageHandler)
#endif
                ),
                new ConcurrentReceiverGroup()));

            // display HTTP service Uri
            LogInfo(LogGroups.Console, "Overhead view [" + FindServiceAliasFromScheme(Uri.UriSchemeHttp) + "/top]\r\n*   Service uri: ");
            LogInfo(LogGroups.Console, "Map view [" + FindServiceAliasFromScheme(Uri.UriSchemeHttp) + "/map]\r\n*   Service uri: ");
            LogInfo(LogGroups.Console, string.Format("Switches: CaptureHistory={0}   CaptureNmea={1}   RetrackNmea={2}", _state.MicrosoftGpsConfig.CaptureHistory, _state.MicrosoftGpsConfig.CaptureNmea, _state.MicrosoftGpsConfig.RetrackNmea));
            Console.WriteLine(string.Format("Switches: CaptureHistory={0}   CaptureNmea={1}   RetrackNmea={2}", _state.MicrosoftGpsConfig.CaptureHistory, _state.MicrosoftGpsConfig.CaptureNmea, _state.MicrosoftGpsConfig.RetrackNmea));
        }

        private void setCaptureNmea()
        {
            if (_gpsConnection != null)
            {
                _gpsConnection.captureNmea = false;
                //_gpsConnection.nmeaFileName = ServicePaths.Store + "/MicrosoftGps.nmea";
                _gpsConnection.nmeaFileName = Path.Combine(@"C:\Microsoft Robotics Dev Studio 4\store", string.Format("MicrosoftGps_{0:yyyyMMdd_HHmmss}.nmea", DateTime.Now));
                _gpsConnection.captureNmea = _state.MicrosoftGpsConfig.CaptureNmea;
                Console.WriteLine("FYI: setCaptureNmea(): " + _state.MicrosoftGpsConfig.CaptureNmea);
            }
            else
            {
                Console.WriteLine("Warning: _gpsConnection is null in setCaptureNmea()");
            }
        }

        #region Service Handlers

        /// <summary>
        /// Handle Errors
        /// </summary>
        /// <param name="ex"></param>
        private void ExceptionHandler(Exception ex)
        {
            LogError(ex.Message);
        }

        /// <summary>
        /// Handle messages
        /// </summary>
        /// <param name="message"></param>
        private void MessageHandler(string message)
        {
            LogInfo(message);
        }

        /// <summary>
        /// Handle inbound Gps Data
        /// </summary>
        /// <param name="fields"></param>
        private void DataReceivedHandler(string[] fields)
        {
            //string nmeaLine = string.Join(",", fields);       // no checksum
            //Console.WriteLine(nmeaLine);

            if (fields[0].Equals("$GPGGA"))
                GgaHandler(fields);
            else if (fields[0].Equals("$GPGSA"))
                GsaHandler(fields);
            else if (fields[0].Equals("$GPGLL"))
                GllHandler(fields);
            else if (fields[0].Equals("$GPGSV"))
                GsvHandler(fields);
            else if (fields[0].Equals("$GPRMC"))
                RmcHandler(fields);
            else if (fields[0].Equals("$GPVTG"))
                VtgHandler(fields);
            else
            {
                string line = string.Join(",", fields);
                MessageHandler(Resources.UnhandledGpsData + line);
            }
        }

        /// <summary>
        /// Handle Gps GPGGA data packet
        /// </summary>
        /// <param name="fields"></param>
        private void GgaHandler(string[] fields)
        {
            try
            {
                if (_state.GpGga == null)
                    _state.GpGga = new GpGga();

                _state.GpGga.IsValid = false;

                if (fields.Length != 15)
                {
                    MessageHandler(string.Format(CultureInfo.InvariantCulture, "Invalid Number of parameters in GPGGA ({0}/{1})", fields.Length, 15));
                    return;
                }

                _state.GpGga.LastUpdate = DateTime.Now;
                _state.GpGga.UTCPosition = DateTime.Now.ToUniversalTime().Date.Add(ParseUTCTime(fields[1]));  // hhmmss.sss
                _state.GpGga.Latitude = ParseLatitude(fields[2], fields[3]);
                _state.GpGga.Longitude = ParseLongitude(fields[4], fields[5]);
                _state.GpGga.PositionFixIndicator = (PositionFixIndicator)int.Parse(fields[6], CultureInfo.InvariantCulture); // table 3
                _state.GpGga.SatellitesUsed = int.Parse(fields[7], CultureInfo.InvariantCulture);
                _state.GpGga.AltitudeUnits = fields[10];        // M-Meters
                _state.GpGga.GeoIdSeparation = fields[11];
                _state.GpGga.GeoIdSeparationUnits = fields[12]; // M-Meters
                _state.GpGga.AgeOfDifferentialCorrection = fields[13]; // null when DGPS not used
                _state.GpGga.DiffRefStationId = fields[14];

                if (_state.GpGga.PositionFixIndicator != PositionFixIndicator.FixNotAvailable)
                {
                    _state.GpGga.HorizontalDilutionOfPrecision = double.Parse(fields[8], CultureInfo.InvariantCulture); // 50.0 is bad
                    _state.GpGga.AltitudeMeters = double.Parse(fields[9], CultureInfo.InvariantCulture);            // no geoid corrections, values are WGS84 ellipsoid heights

                    _state.GpGga.IsValid = true;
                }
                SendNotification(_subMgrPort, new GpGgaNotification(_state.GpGga), Tag_GpGga, _state.GpGga.IsValid.ToString());
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Exception in GgaHandler(): " + ex);
                _gpsDataPort.Post(ex);
            }
        }

        /// <summary>
        /// Handle Gps GPGSA data packet
        /// </summary>
        /// <param name="fields"></param>
        private void GsaHandler(string[] fields)
        {
            try
            {
                if (_state.GpGsa == null)
                    _state.GpGsa = new GpGsa();

                _state.GpGsa.IsValid = false;

                if (fields.Length != 18)
                {
                    MessageHandler(string.Format(CultureInfo.InvariantCulture, "Invalid Number of parameters in GPGSA ({0}/{1})", fields.Length, 18));
                    return;
                }
                string Mode1 = fields[1];        // M-Manual  A-Automatic switch between 2D and 3d
                int Mode2 = int.Parse(fields[2], CultureInfo.InvariantCulture);        // 1-Fix not available, 2-2D, 3-3D

                string Status;
                switch (Mode2)
                {
                    case 2:
                        Status = "2D Fix";
                        break;
                    case 3:
                        Status = "3D Fix";
                        break;
                    default:
                        Status = "Fix not available";
                        break;
                }

                int satelliteId;
                for(int i = 0; i < 12; i++)
                {
                    if (int.TryParse(fields[i + 3], NumberStyles.Integer, CultureInfo.InvariantCulture, out satelliteId))
                        _state.GpGsa._satelliteUsed[i] = satelliteId;
                    else
                        _state.GpGsa._satelliteUsed[i] = 0;
                }
                double SphericalDilutionOfPrecision = -1.0d;
                double.TryParse(fields[15], out SphericalDilutionOfPrecision);

                double HorizontalDilutionOfPrecision = -1.0d;
                double.TryParse(fields[16], out HorizontalDilutionOfPrecision);

                double VerticalDilutionOfPrecision = -1.0d;
                double.TryParse(fields[17], out VerticalDilutionOfPrecision);

                _state.GpGsa.LastUpdate = DateTime.Now;
                _state.GpGsa.Status = Status;
                _state.GpGsa.AutoManual = Mode1;
                _state.GpGsa.Mode = (GsaMode)Mode2;
                _state.GpGsa.SphericalDilutionOfPrecision = SphericalDilutionOfPrecision;
                _state.GpGsa.HorizontalDilutionOfPrecision = HorizontalDilutionOfPrecision;
                _state.GpGsa.VerticalDilutionOfPrecision = VerticalDilutionOfPrecision;

                _state.GpGsa.IsValid = (_state.GpGsa.Mode != GsaMode.NoFix);

                if (_state.MicrosoftGpsConfig.CaptureHistory
                    && _state.GpGsa != null && _state.GpGsa.IsValid     // GSA: Precision data.
                    && (_state.GpGll != null && _state.GpGll.IsValid || _state.GpRmc != null && _state.GpRmc.IsValid)
                    && _state.GpGga != null && _state.GpGga.IsValid)    // GGA: Altitude and backup position.
                {
                    double Latitude, Longitude;
                    DateTime LastUpdate;

                    if (_state.GpGll != null && _state.GpGll.IsValid)
                    {
                        // GLL: Primary Position.
                        Latitude = _state.GpGll.Latitude;
                        Longitude = _state.GpGll.Longitude;
                        LastUpdate = _state.GpGll.LastUpdate;
                    }
                    else
                    {
                        // RMC: Backup course, speed, and position.
                        Latitude = _state.GpRmc.Latitude;
                        Longitude = _state.GpRmc.Longitude;
                        LastUpdate = _state.GpRmc.LastUpdate;
                    }

                    EarthCoordinates ec = new EarthCoordinates(Latitude, Longitude, _state.GpGga.AltitudeMeters, LastUpdate, _state.GpGsa.HorizontalDilutionOfPrecision, _state.GpGsa.VerticalDilutionOfPrecision);
                    _state.History.Add(ec);
                    if (_state.History.Count % 100 == 0)
                        SaveState(_state);
                }

                SendNotification(_subMgrPort, new GpGsaNotification(_state.GpGsa), Tag_GpGsa, _state.GpGsa.IsValid.ToString());
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Exception in GsaHandler(): " + ex);
                _gpsDataPort.Post(ex);
            }
        }

        /// <summary>
        /// Handle Gps GPGLL data packet
        /// </summary>
        /// <param name="fields"></param>
        private void GllHandler(string[] fields)
        {
            try
            {
                if (_state.GpGll == null)
                    _state.GpGll = new GpGll();

                _state.GpGll.IsValid = false;

                if (fields.Length != 7)
                {
                    MessageHandler(string.Format(CultureInfo.InvariantCulture, "Invalid Number of parameters in GPGLL ({0}/{1})", fields.Length, 7));
                    return;
                }

                _state.GpGll.LastUpdate = DateTime.Now;
                _state.GpGll.Latitude = ParseLatitude(fields[1], fields[2]);
                _state.GpGll.Longitude = ParseLongitude(fields[3], fields[4]);
                _state.GpGll.GllTime = DateTime.Now.ToUniversalTime().Date.Add(ParseUTCTime(fields[5]));
                _state.GpGll.Status = (fields[6].Length > 0) ? fields[6].Substring(0, 1) : string.Empty;
                _state.GpGll.IsValid = (_state.GpGll.Status == "A");

                SendNotification(_subMgrPort, new GpGllNotification(_state.GpGll), Tag_GpGll, _state.GpGll.IsValid.ToString());
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Exception in GllHandler(): " + ex);
                _gpsDataPort.Post(ex);
            }

        }

        /// <summary>
        /// Handle Gps GPGSV data packet
        /// </summary>
        /// <param name="fields"></param>
        private void GsvHandler(string[] fields)
        {
            try
            {
                if (_state.GpGsv == null)
                    _state.GpGsv = new GpGsv();

                _state.GpGsv.IsValid = false;

                int NumberOfMessages = int.Parse(fields[1], CultureInfo.InvariantCulture);
                int MessageNumber = int.Parse(fields[2], CultureInfo.InvariantCulture);
                int satellitesInView = int.Parse(fields[3], CultureInfo.InvariantCulture);
                int priorMessages = (4 * (MessageNumber - 1));
                int currentMessages = Math.Min(4, satellitesInView - priorMessages);
                int ColumnsExpected = 4 + (currentMessages * 4);
                if (fields.Length != ColumnsExpected)
                {
                    MessageHandler(string.Format(CultureInfo.InvariantCulture, "Invalid Number of parameters in GPGSV ({0}/{1})", fields.Length, ColumnsExpected));
                    return;
                }

                if (_state.GpGsv._gsvSatellites == null || _state.GpGsv._gsvSatellites.Length != satellitesInView)
                {
                    _state.GpGsv._gsvSatellites = new GsvSatellite[satellitesInView];
                    for (int i = 0; i < satellitesInView; i++)
                        _state.GpGsv._gsvSatellites[i] = new GsvSatellite();
                }
                int number;
                for (int i = 0; i < currentMessages; i++)
                {
                    string satelliteId = fields[4 + (i * 4)];
                    if (!string.IsNullOrEmpty(satelliteId) && int.TryParse(satelliteId, NumberStyles.Integer, CultureInfo.InvariantCulture, out number))
                        _state.GpGsv._gsvSatellites[priorMessages + i].Id = number;
                    string elevationDegrees = fields[5 + (i * 4)];
                    if (!string.IsNullOrEmpty(elevationDegrees) && int.TryParse(elevationDegrees, NumberStyles.Integer, CultureInfo.InvariantCulture, out number))
                        _state.GpGsv._gsvSatellites[priorMessages + i].ElevationDegrees = number;
                    string azimuthDegrees = fields[6 + (i * 4)];
                    if (!string.IsNullOrEmpty(azimuthDegrees) && int.TryParse(azimuthDegrees, NumberStyles.Integer, CultureInfo.InvariantCulture, out number))
                        _state.GpGsv._gsvSatellites[priorMessages + i].AzimuthDegrees = number;
                    string signalToNoiseRatio = fields[7 + (i * 4)];
                    if (!string.IsNullOrEmpty(signalToNoiseRatio) && int.TryParse(signalToNoiseRatio, NumberStyles.Integer, CultureInfo.InvariantCulture, out number))
                        _state.GpGsv._gsvSatellites[priorMessages + i].SignalToNoiseRatio = number;
                    else
                        _state.GpGsv._gsvSatellites[priorMessages + i].SignalToNoiseRatio = -1;
                }

                _state.GpGsv.LastUpdate = DateTime.Now;
                _state.GpGsv.SatellitesInView = satellitesInView;
                _state.GpGsv.IsValid = (satellitesInView > 1);

                SendNotification(_subMgrPort, new GpGsvNotification(_state.GpGsv), Tag_GpGsv, _state.GpGsv.IsValid.ToString());

            }
            catch (Exception ex)
            {
                Debug.WriteLine("Exception in GsvHandler(): " + ex);
                _gpsDataPort.Post(ex);
            }
        }

        /// <summary>
        /// Handle Gps GPRMC data packet
        /// </summary>
        /// <param name="fields"></param>
        private void RmcHandler(string[] fields)
        {
            try
            {
                if (_state.GpRmc == null)
                    _state.GpRmc = new GpRmc();

                _state.GpRmc.IsValid = false;

                if (fields.Length < 11 || fields.Length > 13)
                {
                    MessageHandler(string.Format(CultureInfo.InvariantCulture, "Invalid Number of parameters in GPRMC ({0}/{1})", fields.Length, 12));
                    return;
                }

                string UtcTime = fields[1];
                string Status = fields[2];   // V

                string latitude = fields[3];
                string northSouth = fields[4];
                string longitude = fields[5];
                string eastWest = fields[6];

                double speedKnots = 0;
                if (IsNumericDouble(fields[7]))
                {
                    speedKnots = double.Parse(fields[7], CultureInfo.InvariantCulture);
                }
                string courseDegrees = fields[8];
                string dateDDMMYY = fields[9];                           //120120 is bad data
                // string magneticVariationDegrees = fields[10];

                _state.GpRmc.LastUpdate = DateTime.Now;
                _state.GpRmc.Status = Status;
                _state.GpRmc.Latitude = ParseLatitude(latitude, northSouth);
                _state.GpRmc.Longitude = ParseLongitude(longitude, eastWest);
                _state.GpRmc.SpeedMetersPerSecond = KnotsToMetersPerSecond(speedKnots);
                if (IsNumericDouble(courseDegrees))
                {
                    _state.GpRmc.CourseDegrees = double.Parse(courseDegrees, CultureInfo.InvariantCulture);
                }
                _state.GpRmc.DateTime = ParseUTCDateTime(dateDDMMYY, UtcTime);  // hhmmss.sss

                // Validate!
                _state.GpRmc.IsValid = (Status != "V");

                SendNotification(_subMgrPort, new GpRmcNotification(_state.GpRmc), Tag_GpRmc, _state.GpRmc.IsValid.ToString());

            }
            catch (Exception ex)
            {
                Debug.WriteLine("Exception in RmcHandler(): " + ex);
                _gpsDataPort.Post(ex);
            }
        }

        /// <summary>
        /// Handle Gps GPVTG data packet
        /// </summary>
        /// <param name="fields"></param>
        private void VtgHandler(string[] fields)
        {
            try
            {
                if (_state.GpVtg == null)
                    _state.GpVtg = new GpVtg();

                _state.GpVtg.IsValid = false;

                if (fields.Length != 9 && fields.Length != 10)
                {
                    MessageHandler(string.Format(CultureInfo.InvariantCulture, "Invalid Number of parameters in GPVTG ({0}/{1})", fields.Length, 9));
                    return;
                }

                _state.GpVtg.LastUpdate = DateTime.Now;

                string CourseDegrees = fields[1];
                string Reference = fields[2];	// T = True
                string Course2 = fields[3];
                string Reference2 = fields[4];	// M = Magnetic
                string Speed1 = fields[5];
                string Units1 = fields[6];		// N = Knots
                string Speed2 = fields[7];
                string Units2 = fields[8];		// K = Kilometer per hour

                if (IsNumericDouble(CourseDegrees) && (Reference == "T" || Reference2 != "T" || !IsNumericDouble(Course2)))
                    _state.GpVtg.CourseDegrees = double.Parse(CourseDegrees, CultureInfo.InvariantCulture);
                else if (IsNumericDouble(Course2))
                    _state.GpVtg.CourseDegrees = double.Parse(Course2, CultureInfo.InvariantCulture);

                double speed = -1;
                string units = string.Empty;

                if (IsNumericDouble(Speed1) && (Units1 == "N" || Units1 == "K"))
                {
                    speed = double.Parse(Speed1, CultureInfo.InvariantCulture);
                    units = Units1;
                }
                else if (IsNumericDouble(Speed2) && (Units2 == "N" || Units2 == "K"))
                {
                    speed = double.Parse(Speed2, CultureInfo.InvariantCulture);
                    units = Units2;
                }
                if (speed < 0)
                    return;

                if (units.Equals("N", StringComparison.OrdinalIgnoreCase))
                    _state.GpVtg.SpeedMetersPerSecond = KnotsToMetersPerSecond(speed);
                else if (units.Equals("K", StringComparison.OrdinalIgnoreCase))
                    _state.GpVtg.SpeedMetersPerSecond = speed * 1000.0 / 60.0 / 60.0;

                _state.GpVtg.IsValid = true;

                SendNotification(_subMgrPort, new GpVtgNotification(_state.GpVtg), Tag_GpVtg, _state.GpVtg.IsValid.ToString());
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Exception in VtgHandler(): " + ex);
                _gpsDataPort.Post(ex);
            }
        }

        #endregion

        #region Operation Handlers

        /// <summary>
        /// Http Get Handler
        /// </summary>
        /// <param name="httpGet"></param>
        /// <returns></returns>
        [ServiceHandler(ServiceHandlerBehavior.Concurrent)]
        public virtual IEnumerator<ITask> HttpGetHandler(HttpGet httpGet)
        {
            HttpListenerRequest request = httpGet.Body.Context.Request;
            HttpListenerResponse response = httpGet.Body.Context.Response;

            Stream image = null;
            HttpResponseType rsp = null;

            string path = request.Url.AbsolutePath.ToLowerInvariant();

            if (path.StartsWith(_root, StringComparison.InvariantCultureIgnoreCase))
            {
                if (path.EndsWith(_showTop, StringComparison.InvariantCultureIgnoreCase))
                {
                    image = GenerateTop(800);
                    if (image != null)
                    {
                        SendJpeg(httpGet.Body.Context, image);
                    }
                    else
                    {
                        httpGet.ResponsePort.Post(Fault.FromCodeSubcodeReason(
                            W3C.Soap.FaultCodes.Receiver,
                            DsspFaultCodes.OperationFailed,
                            "Unable to generate Image"));
                    }
                    yield break;
                }
                else if (path.EndsWith(_showMap, StringComparison.InvariantCultureIgnoreCase))
                {
                    rsp = new HttpResponseType(HttpStatusCode.OK, _state, _transformGpsMap);
                    httpGet.ResponsePort.Post(rsp);
                    yield break;
                }
            }

            rsp = new HttpResponseType(HttpStatusCode.OK, _state, _transformGpsData);
            httpGet.ResponsePort.Post(rsp);
            yield break;

        }

        /// <summary>
        /// Http Post Handler
        /// </summary>
        /// <param name="httpPost"></param>
        /// <returns></returns>
        [ServiceHandler(ServiceHandlerBehavior.Exclusive)]
        public IEnumerator<ITask> HttpPostHandler(HttpPost httpPost)
        {
            // Use helper to read form data
            ReadFormData readForm = new ReadFormData(httpPost);
            _httpUtilities.Post(readForm);

            // Wait for result
            Activate(Arbiter.Choice(readForm.ResultPort,
                delegate(NameValueCollection parameters)
                {
                    if (!string.IsNullOrEmpty(parameters["Action"])
                        && parameters["Action"] == "MicrosoftGpsConfig"
                        )
                    {
                        if (parameters["buttonOk"] == "Search")
                        {
                            FindGpsConfig findConfig = new FindGpsConfig();
                            _mainPort.Post(findConfig);
                            Activate(
                                Arbiter.Choice(
                                    Arbiter.Receive<MicrosoftGpsConfig>(false, findConfig.ResponsePort,
                                        delegate(MicrosoftGpsConfig response)
                                        {
                                            HttpPostSuccess(httpPost);
                                        }),
                                    Arbiter.Receive<Fault>(false, findConfig.ResponsePort,
                                        delegate(Fault f)
                                        {
                                            HttpPostFailure(httpPost, f);
                                        })
                                )
                            );

                        }
                        else if (parameters["buttonOk"] == "Connect and Update")
                        {

                            MicrosoftGpsConfig config = (MicrosoftGpsConfig)_state.MicrosoftGpsConfig.Clone();
                            int port;
                            if (int.TryParse(parameters["CommPort"], out port) && port >= 0)
                            {
                                config.CommPort = port;
                                config.PortName = "COM" + port.ToString();
                            }

                            int baud;
                            if (int.TryParse(parameters["BaudRate"], out baud) && GpsConnection.ValidBaudRate(baud))
                            {
                                config.BaudRate = baud;
                            }

                            config.CaptureHistory = ((parameters["CaptureHistory"] ?? "off") == "on");
                            config.CaptureNmea = ((parameters["CaptureNmea"] ?? "off") == "on");
                            config.RetrackNmea = ((parameters["RetrackNmea"] ?? "off") == "on");

                            Console.WriteLine(string.Format("Switches: CaptureHistory={0}   CaptureNmea={1}   RetrackNmea={2}", config.CaptureHistory, config.CaptureNmea, config.RetrackNmea));

                            Configure configure = new Configure(config);
                            _mainPort.Post(configure);
                            Activate(
                                Arbiter.Choice(
                                    Arbiter.Receive<DefaultUpdateResponseType>(false, configure.ResponsePort,
                                        delegate(DefaultUpdateResponseType response)
                                        {
                                            HttpPostSuccess(httpPost);
                                        }),
                                    Arbiter.Receive<Fault>(false, configure.ResponsePort,
                                        delegate(Fault f)
                                        {
                                            HttpPostFailure(httpPost, f);
                                        })
                                )
                            );
                        }

                    }
                    else
                    {
                        HttpPostFailure(httpPost, null);
                    }
                },
                delegate(Exception Failure)
                {
                    LogError(Failure.Message);
                })
            );
            yield break;
        }

        /// <summary>
        /// Send Http Post Success Response
        /// </summary>
        /// <param name="httpPost"></param>
        private void HttpPostSuccess(HttpPost httpPost)
        {
            HttpResponseType rsp =
                new HttpResponseType(HttpStatusCode.OK, _state, _transformGpsData);
            httpPost.ResponsePort.Post(rsp);
        }

        /// <summary>
        /// Send Http Post Failure Response
        /// </summary>
        /// <param name="httpPost"></param>
        /// <param name="fault"></param>
        private static void HttpPostFailure(HttpPost httpPost, Fault fault)
        {
            HttpResponseType rsp =
                new HttpResponseType(HttpStatusCode.BadRequest, fault);
            httpPost.ResponsePort.Post(rsp);
        }


        /// <summary>
        /// Get Handler
        /// </summary>
        /// <param name="get"></param>
        /// <returns></returns>
        [ServiceHandler(ServiceHandlerBehavior.Concurrent)]
        public virtual IEnumerator<ITask> GetHandler(Get get)
        {
            get.ResponsePort.Post(_state);
            yield break;
        }

        /// <summary>
        /// Configure Handler
        /// </summary>
        /// <param name="update"></param>
        /// <returns></returns>
        [ServiceHandler(ServiceHandlerBehavior.Exclusive)]
        public virtual IEnumerator<ITask> ConfigureHandler(Configure update)
        {
            _state.MicrosoftGpsConfig = update.Body;
            bool connected = _gpsConnection.Open(_state.MicrosoftGpsConfig.CommPort, _state.MicrosoftGpsConfig.BaudRate);

            if (_state.MicrosoftGpsConfig.CaptureHistory)
                _state.History = new List<EarthCoordinates>();
            else
                _state.History = null;

            SaveState(_state);

            setCaptureNmea();

            update.ResponsePort.Post(DefaultUpdateResponseType.Instance);
            yield break;
        }

        /// <summary>
        /// Subscribe Handler
        /// </summary>
        /// <param name="subscribe"></param>
        /// <returns></returns>
        [ServiceHandler(ServiceHandlerBehavior.Exclusive)]
        public virtual IEnumerator<ITask> SubscribeHandler(Subscribe subscribe)
        {
            SubscribeRequest request = subscribe.Body;

            submgr.InsertSubscription insert = new submgr.InsertSubscription(request);
            insert.Body.FilterType = submgr.FilterType.Default;

            string valid = request.ValidOnly ? "True" : null;

            List<submgr.QueryType> query = new List<submgr.QueryType>();

            if (request.MessageTypes == GpsMessageType.All ||
                request.MessageTypes == GpsMessageType.None)
            {
                if (request.ValidOnly)
                {
                    query.Add(new submgr.QueryType(null, valid));
                }
            }
            else
            {
                if ((request.MessageTypes & GpsMessageType.GPGGA) != 0)
                {
                    query.Add(new submgr.QueryType(Tag_GpGga, valid));
                }
                if ((request.MessageTypes & GpsMessageType.GPGLL) != 0)
                {
                    query.Add(new submgr.QueryType(Tag_GpGll, valid));
                }
                if ((request.MessageTypes & GpsMessageType.GPGSA) != 0)
                {
                    query.Add(new submgr.QueryType(Tag_GpGsa, valid));
                }
                if ((request.MessageTypes & GpsMessageType.GPGSV) != 0)
                {
                    query.Add(new submgr.QueryType(Tag_GpGsv, valid));
                }
                if ((request.MessageTypes & GpsMessageType.GPRMC) != 0)
                {
                    query.Add(new submgr.QueryType(Tag_GpRmc, valid));
                }
                if ((request.MessageTypes & GpsMessageType.GPVTG) != 0)
                {
                    query.Add(new submgr.QueryType(Tag_GpVtg, valid));
                }
            }

            if (query.Count > 0)
            {
                insert.Body.QueryList = query.ToArray();
            }
            _subMgrPort.Post(insert);

            yield return Arbiter.Choice(
                insert.ResponsePort,
                subscribe.ResponsePort.Post,
                subscribe.ResponsePort.Post
            );
        }

        /// <summary>
        /// Find a Microsoft GPS on any serial port
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        [ServiceHandler(ServiceHandlerBehavior.Exclusive)]
        public virtual IEnumerator<ITask> FindGpsConfigHandler(FindGpsConfig query)
        {
            _state.Connected = _gpsConnection.FindGps();
            if (_state.Connected)
            {
                _state.MicrosoftGpsConfig = _gpsConnection.MicrosoftGpsConfig;
                SaveState(_state);
            }
            query.ResponsePort.Post(_state.MicrosoftGpsConfig);
            yield break;
        }



        /// <summary>
        /// SendGpsCommand Handler
        /// </summary>
        /// <param name="update"></param>
        /// <returns></returns>
        [ServiceHandler(ServiceHandlerBehavior.Exclusive)]
        public virtual IEnumerator<ITask> SendGpsCommandHandler(SendMicrosoftGpsCommand update)
        {
            update.ResponsePort.Post(
                Fault.FromException(
                    new NotImplementedException("The Microsoft Gps service is a sample.  Sending commands to the GPS is an exercise left to the community.")));

            yield break;
        }

        /// <summary>
        /// Shut down the GPS connection
        /// </summary>
        /// <returns></returns>
        [ServiceHandler(ServiceHandlerBehavior.Teardown)]
        public virtual IEnumerator<ITask> DropHandler(DsspDefaultDrop drop)
        {
            if (_state.Connected)
            {
                _gpsConnection.Close();
                _state.Connected = false;
            }

            base.DefaultDropHandler(drop);
            yield break;
        }

        #endregion

        #region GPS NMEA Log Reader for simulation

        private IEnumerator<ITask> SimulateMicrosoftGps()
        {
            yield return TimeoutPort(20000).Receive();  // initial startup delay

            string nmeaFileName = @"C:\Microsoft Robotics Dev Studio 4\store\MicrosoftGps - Copy.nmea";

            // Open and read the file.
            using (StreamReader r = File.OpenText(nmeaFileName))
            {
                string line;
                while ((line = r.ReadLine()) != null)
                {
                    Console.WriteLine(line);

                    int ix = line.IndexOf('*');
                    if (ix > 0)
                    {
                        string[] fields = line.Substring(0, ix).Split(',');
                        _gpsDataPort.Post(fields);
                    }

                    yield return TimeoutPort(1000).Receive();
                }
                r.Close();
            }

            yield break;
        }

        #endregion // GPS NMEA Log Reader for simulation

        #region Gps Helpers

        /// <summary>
        /// Connect to a Microsoft Gps.
        /// If no configuration exists, search for the connection.
        /// </summary>
        private IEnumerator<ITask> ConnectToMicrosoftGps()
        {
            try
            {
                _state.GpGga = null;
                _state.GpGll = null;
                _state.GpGsa = null;
                _state.GpGsv = null;
                _state.GpRmc = null;
                _state.GpVtg = null;
                _state.Connected = false;

                if (_state.MicrosoftGpsConfig.CommPort != 0 && _state.MicrosoftGpsConfig.BaudRate != 0)
                {
                    _state.MicrosoftGpsConfig.ConfigurationStatus = "Opening Gps on Port " + _state.MicrosoftGpsConfig.CommPort.ToString();
                    Console.WriteLine(_state.MicrosoftGpsConfig.ConfigurationStatus + "  Baud: " + _state.MicrosoftGpsConfig.BaudRate);
                    _state.Connected = _gpsConnection.Open(_state.MicrosoftGpsConfig.CommPort, _state.MicrosoftGpsConfig.BaudRate);
                    Console.WriteLine("Connected: " + _state.Connected);
                }
                else if (File.Exists(_state.MicrosoftGpsConfig.PortName))
                {
                    _state.MicrosoftGpsConfig.ConfigurationStatus = "Opening Gps on " + _state.MicrosoftGpsConfig.PortName;
                    Console.WriteLine(_state.MicrosoftGpsConfig.ConfigurationStatus);
                    _state.Connected = _gpsConnection.Open(_state.MicrosoftGpsConfig.PortName);
                    Console.WriteLine("Connected: " + _state.Connected);
                }
                else
                {
                    _state.MicrosoftGpsConfig.ConfigurationStatus = "Searching for the GPS Port";
                    Console.WriteLine(_state.MicrosoftGpsConfig.ConfigurationStatus);
                    _state.Connected = _gpsConnection.FindGps();
                    Console.WriteLine("Connected: " + _state.Connected);
                    if (_state.Connected)
                    {
                        _state.MicrosoftGpsConfig = _gpsConnection.MicrosoftGpsConfig;
                        SaveState(_state);
                        Console.WriteLine("Port:: " + _state.MicrosoftGpsConfig.CommPort + "  Baud: " + _state.MicrosoftGpsConfig.BaudRate);
                    }
                }
            }
            catch (UnauthorizedAccessException ex)
            {
                LogError(ex);
            }
            catch (IOException ex)
            {
                LogError(ex);
            }
            catch (ArgumentOutOfRangeException ex)
            {
                LogError(ex);
            }
            catch (ArgumentException ex)
            {
                LogError(ex);
            }
            catch (InvalidOperationException ex)
            {
                LogError(ex);
            }

            if (!_state.Connected)
            {
                _state.MicrosoftGpsConfig.ConfigurationStatus = "Not Connected";
                LogInfo(LogGroups.Console, "The Microsoft GPS is not detected.\r\n*   To configure the Microsoft Gps, navigate to: ");
            }
            else
            {
                _state.MicrosoftGpsConfig.ConfigurationStatus = "Connected";
            }
            yield break;
        }

        /// <summary>
        /// Convert knots to meters per second
        /// </summary>
        /// <param name="knots"></param>
        /// <returns></returns>
        public static double KnotsToMetersPerSecond(double knots)
        {
            if (knots >= 0)
                return knots * _knotsToMetersPerSecond;
            return -1;
        }

        /// <summary>
        /// True when the specified string is a valid double
        /// </summary>
        /// <param name="numeric"></param>
        /// <returns></returns>
        public static bool IsNumericDouble(string numeric)
        {
            if (string.IsNullOrEmpty(numeric))
                return false;

            bool decimalFound = false;
            numeric = numeric.Trim();
            if (numeric.StartsWith("+", StringComparison.Ordinal) || numeric.StartsWith("-", StringComparison.Ordinal))
                numeric = numeric.Substring(1);

            if (numeric.Length == 0)
                return false;

            foreach (char c in numeric)
            {
                if (c >= '0' && c <= '9')
                    continue;
                if ((c == '.') && !decimalFound)
                {
                    decimalFound = true;
                    continue;
                }
                return false;
            }
            return true;
        }

        /// <summary>
        /// Parse Gps Latitude
        /// </summary>
        /// <param name="Latitude"></param>
        /// <param name="NorthSouth"></param>
        /// <returns></returns>
        private static double ParseLatitude(string Latitude, string NorthSouth)
        {
            if (string.IsNullOrEmpty(Latitude) || string.IsNullOrEmpty(NorthSouth) || Latitude.Length < 3)
                return 0.0d;

            // LATITUDE
            bool IsNorth = (NorthSouth != "S");
            double degrees = Double.Parse(Latitude.Substring(0, 2), CultureInfo.InvariantCulture);
            double minutes = Double.Parse(Latitude.Substring(2), CultureInfo.InvariantCulture);
            double dLatitude = degrees + minutes / 60.0;
            if (!IsNorth)
            {
                dLatitude = -dLatitude;
            }

            return dLatitude;
        }

        /// <summary>
        /// Parse Gps Longitude
        /// </summary>
        /// <param name="Longitude"></param>
        /// <param name="EastWest"></param>
        /// <returns></returns>
        private static double ParseLongitude(string Longitude, string EastWest)
        {
            if (string.IsNullOrEmpty(Longitude) || string.IsNullOrEmpty(EastWest) || Longitude.Length < 3)
                return 0.0d;

            // LONGITUDE
            bool IsWest = (EastWest != "E");
            double degrees = Double.Parse(Longitude.Substring(0, 3), CultureInfo.InvariantCulture);
            double minutes = Double.Parse(Longitude.Substring(3), CultureInfo.InvariantCulture);
            double dLongitude = degrees + minutes / 60.0;
            if (IsWest)
            {
                dLongitude = -dLongitude;
            }
            return dLongitude;
        }

        /// <summary>
        /// Parse UTC Date and Time
        /// </summary>
        /// <param name="DateDDMMYY"></param>
        /// <param name="UTCPosition"></param>
        /// <returns></returns>
        private static DateTime ParseUTCDateTime(string DateDDMMYY, string UTCPosition)
        {
            DateTime date = DateTime.Now.Date;
            if (DateDDMMYY.Length > 0)
            {
                string DateMMDDYY = string.Format(CultureInfo.InvariantCulture, "{0}/{1}/{2}", DateDDMMYY.Substring(2, 2), DateDDMMYY.Substring(0, 2), DateDDMMYY.Substring(4, 2));
                date = System.Convert.ToDateTime(DateMMDDYY, CultureInfo.InvariantCulture).Date;
            }

            // hhmmss.sss
            int hh = int.Parse(UTCPosition.Substring(0, 2), CultureInfo.InvariantCulture);
            int mm = int.Parse(UTCPosition.Substring(2, 2), CultureInfo.InvariantCulture);
            int ss = int.Parse(UTCPosition.Substring(4, 2), CultureInfo.InvariantCulture);
            int ms = int.Parse(UTCPosition.Substring(7), CultureInfo.InvariantCulture);
            DateTime Utc = new DateTime(date.Year, date.Month, date.Day, hh, mm, ss, ms);
            DateTime adjustedDate = Utc.ToLocalTime();
            return adjustedDate;
        }

        /// <summary>
        /// Parse UTC Time
        /// </summary>
        /// <param name="UTCPosition"></param>
        /// <returns></returns>
        private static TimeSpan ParseUTCTime(string UTCPosition)
        {
            if (UTCPosition.Length < 8)
                return TimeSpan.MinValue;

            // hhmmss.sss
            int hh = int.Parse(UTCPosition.Substring(0, 2), CultureInfo.InvariantCulture);
            int mm = int.Parse(UTCPosition.Substring(2, 2), CultureInfo.InvariantCulture);
            int ss = int.Parse(UTCPosition.Substring(4, 2), CultureInfo.InvariantCulture);
            int ms = int.Parse(UTCPosition.Substring(7), CultureInfo.InvariantCulture);

            DateTime date = DateTime.Now.Date;
            DateTime Utc = new DateTime(date.Year, date.Month, date.Day, hh, mm, ss, ms);
            DateTime adjustedDate = Utc.ToLocalTime();
            TimeSpan time = new TimeSpan(0, adjustedDate.Hour, adjustedDate.Minute, adjustedDate.Second, adjustedDate.Millisecond);
            return time;
        }

        #endregion

        #region Image Helpers
        /// <summary>
        /// Post an image to the Http web request
        /// </summary>
        /// <param name="context"></param>
        /// <param name="stream"></param>
        private void SendJpeg(HttpListenerContext context, Stream stream)
        {
            WriteResponseFromStream write = new WriteResponseFromStream(context, stream, MediaTypeNames.Image.Jpeg);

            _httpUtilities.Post(write);

            Activate(
                Arbiter.Choice(
                    write.ResultPort,
                    delegate(Stream res)
                    {
                        stream.Close();
                    },
                    delegate(Exception e)
                    {
                        stream.Close();
                        LogError(e);
                    }
                )
            );
        }


        /// <summary>
        /// Generate a bitmap showing an overhead view of the gps waypoints
        /// </summary>
        /// <param name="width"></param>
        /// <returns></returns>
        private Stream GenerateTop(int width)
        {
            MemoryStream memory = null;

            int height = width * 3 / 4;
            using (Bitmap bmp = new Bitmap(width, height))
            {
                using (Graphics g = Graphics.FromImage(bmp))
                {
                    g.Clear(Color.LightGray);
                    g.DrawRectangle(Pens.White, new Rectangle(-1, -1, 3, 3));

                    if (_state.History == null || _state.History.Count < 1)
                    {
                        g.DrawString("No Data - check 'Capture History' box to see the track", new Font(FontFamily.GenericSansSerif, 16, GraphicsUnit.Pixel), Brushes.Red, new Point(10, 10));
                    }
                    else // plot a simple map from the Gps waypoints
                    {

                        double minLongitude = 9999.0, minLatitude = 9999.0, minAltitude = 9999999.0;
                        double maxLongitude = -9999.0, maxLatitude = -9999.0, maxAltitude = -9999999.0;

                        foreach (EarthCoordinates ec in _state.History)
                        {
                            minLongitude = Math.Min(minLongitude, ec.Longitude);
                            minLatitude = Math.Min(minLatitude, ec.Latitude);
                            minAltitude = Math.Min(minAltitude, ec.AltitudeMeters);
                            maxLongitude = Math.Max(maxLongitude, ec.Longitude);
                            maxLatitude = Math.Max(maxLatitude, ec.Latitude);
                            maxAltitude = Math.Max(maxAltitude, ec.AltitudeMeters);
                        }

                        if (minLongitude < 0)
                        {
                            double hold = minLongitude;
                            minLongitude = maxLongitude;
                            maxLongitude = hold;
                        }

                        EarthCoordinates start = new EarthCoordinates(minLatitude, minLongitude, minAltitude);
                        EarthCoordinates end = new EarthCoordinates(maxLatitude, maxLongitude, maxAltitude);
                        Point3 box = end.OffsetFromStart(start);
                        double scale = Math.Max(box.X / (double)width, box.Y / (double)height);

                        Point lastPoint = Point.Empty;
                        Point currentPoint;
                        foreach (EarthCoordinates ec in _state.History)
                        {
                            box = ec.OffsetFromStart(start);
                            currentPoint = new Point(width - (int)(box.Y / scale) - 10, height - (int)(box.X / scale) - 10);

                            // Draw the point.
                            int dop = (int)Math.Truncate(ec.HorizontalDilutionOfPrecision);
                            Rectangle r = new Rectangle(currentPoint.X - dop, currentPoint.Y - dop, dop * 2, dop * 2);
                            g.FillEllipse(Brushes.Azure, r);
                            g.DrawEllipse(Pens.Yellow, r);

                            if (lastPoint == Point.Empty)
                                g.DrawString("Start", new Font(FontFamily.GenericSansSerif, 16, GraphicsUnit.Pixel), Brushes.Green, currentPoint);
                            else
                            {
                                g.DrawLine(Pens.Red, lastPoint, currentPoint);
                            }
                            lastPoint = currentPoint;
                        }
                    }
                }

                memory = new MemoryStream();
                bmp.Save(memory, ImageFormat.Jpeg);
                memory.Position = 0;

            }
            return memory;
        }
        #endregion
    }
}
