//#define TRACEDEBUG
//#define TRACEDEBUGTICKS
//#define TRACELOG

using Microsoft.Dss.ServiceModel.DsspServiceBase;

using TrackRoamer.Robotics.Utility.LibSystem;
using TrackRoamer.Robotics.LibMapping;

using gps = Microsoft.Robotics.Services.Sensors.Gps.Proxy;

namespace TrackRoamer.Robotics.Services.TrackRoamerBehaviors
{
    partial class TrackRoamerBehaviorsService : DsspServiceBase
    {
        #region GPS messages handler

        // see http://www.gpsinformation.org/dale/nmea.htm  for NMEA packages semantics
        // The most important NMEA sentences include the GGA which provides the current Fix data, the RMC which provides the minimum gps sentences information, and the GSA which provides the Satellite status data.

        protected bool traceGps = false;

        /// <summary>
        /// Handle GPS GPGGA Notifications - Altitude and backup position - Time, position and fix type data 
        /// </summary>
        /// <param name="notification">GPGGA packet notification</param>
        private void GpGgaHandler(gps.GpGgaNotification notification)
        {
            /*
                GGA - essential fix data which provide 3D location and accuracy data.
              
                $GPGGA,123519,4807.038,N,01131.000,E,1,08,0.9,545.4,M,46.9,M,,*47

                Where:
                     GGA          Global Positioning System Fix Data
                     123519       Fix taken at 12:35:19 UTC
                     4807.038,N   Latitude 48 deg 07.038' N
                     01131.000,E  Longitude 11 deg 31.000' E
                     1            Fix quality: 0 = invalid
                                               1 = GPS fix (SPS)
                                               2 = DGPS fix
                                               3 = PPS fix
			                       4 = Real Time Kinematic
			                       5 = Float RTK
                                               6 = estimated (dead reckoning) (2.3 feature)
			                       7 = Manual input mode
			                       8 = Simulation mode
                     08           Number of satellites being tracked
                     0.9          Horizontal dilution of position
                     545.4,M      Altitude, Meters, above mean sea level
                     46.9,M       Height of geoid (mean sea level) above WGS84
                                      ellipsoid
                     (empty field) time in seconds since last DGPS update
                     (empty field) DGPS station ID number
                     *47          the checksum data, always begins with *
             
                If the height of geoid is missing then the altitude should be suspect.
                Some non-standard implementations report altitude with respect to the ellipsoid rather than geoid altitude.
                Some units do not report negative altitudes at all. This is the only sentence that reports altitude.
             */

            if (!_mapperVicinity.robotState.ignoreGps)
            {
                if (!notification.Body.IsValid)
                {
                    Tracer.Error("the GPS reported GPGGA with IsValid=false");
                }
                else
                {
                    if (traceGps)
                    {
                        Tracer.Trace(string.Format("the GPS reported GPGGA: {0}  Lat: {1}  Lon: {2}", notification.Body.LastUpdate, notification.Body.Latitude, notification.Body.Longitude));
                    }

                    GpsState state = _state.gpsState;

                    // Position Fix Indicator:
                    state.GPGGA_PositionFixIndicator = notification.Body.PositionFixIndicator;
                    // Altitude:
                    state.GPGGA_AltitudeMeters = notification.Body.AltitudeMeters;
                    // Coordinates:
                    state.GPGGA_Latitude = notification.Body.Latitude;
                    state.GPGGA_Longitude = notification.Body.Longitude;
                    // Precision:
                    state.GPGGA_HorizontalDilutionOfPrecision = notification.Body.HorizontalDilutionOfPrecision;
                    state.GPGGA_SatellitesUsed = notification.Body.SatellitesUsed;
                    // Last Updated:
                    state.GPGGA_LastUpdate = notification.Body.LastUpdate;

                    if (!_testBumpMode && !_state.Dropping && !_doUnitTest)
                    {
                        Decide(SensorEventSource.Gps);
                    }
                }
            }
        }

        /// <summary>
        /// Handle GPS GPGLL Notifications - Primary Position
        /// </summary>
        /// <param name="notification">GPGLL packet notification</param>
        private void GpGllHandler(gps.GpGllNotification notification)
        {
            /*
                GLL - Geographic Latitude and Longitude is a holdover from Loran data and some old units may not send the time and data active information if they are emulating Loran data.
                If a gps is emulating Loran data they may use the LC Loran prefix instead of GP.

                  $GPGLL,4916.45,N,12311.12,W,225444,A,*1D

                Where:
                     GLL          Geographic position, Latitude and Longitude
                     4916.46,N    Latitude 49 deg. 16.45 min. North
                     12311.12,W   Longitude 123 deg. 11.12 min. West
                     225444       Fix taken at 22:54:44 UTC
                     A            Data Active or V (void)
                     *iD          checksum data
             
                Note that, as of the 2.3 release of NMEA, there is a new field in the GLL sentence at the end just prior to the checksum. For more information on this field see here.
             */

            if (!_mapperVicinity.robotState.ignoreGps)
            {
                if (!notification.Body.IsValid)
                {
                    Tracer.Error("the GPS reported GPGGA with IsValid=false");
                }
                else
                {
                    if (traceGps)
                    {
                        Tracer.Trace(string.Format("the GPS reported GPGLL: {0}  Lat: {1}  Lon: {2}", notification.Body.LastUpdate, notification.Body.Latitude, notification.Body.Longitude));
                    }

                    GpsState state = _state.gpsState;

                    // Position Fix Indicator:
                    state.GPGLL_MarginOfError = notification.Body.MarginOfError;
                    // Altitude:
                    state.GPGLL_Status = notification.Body.Status;
                    // Coordinates:
                    state.GPGLL_Latitude = notification.Body.Latitude;
                    state.GPGLL_Longitude = notification.Body.Longitude;
                    // Last Updated:
                    state.GPGLL_LastUpdate = notification.Body.LastUpdate;

                    if (!_testBumpMode && !_state.Dropping && !_doUnitTest)
                    {
                        Decide(SensorEventSource.Gps);
                    }
                }
            }
        }

        /// <summary>
        /// Handle GPS GPGSA Notifications - Precision - GPS receiver operating mode 
        /// </summary>
        /// <param name="notification">GPGSA packet notification</param>
        private void GpGsaHandler(gps.GpGsaNotification notification)
        {
            /*
                GSA - GPS DOP and active satellites. This sentence provides details on the nature of the fix.
                      It includes the numbers of the satellites being used in the current solution and the DOP.
                      DOP (dilution of precision) is an indication of the effect of satellite geometry on the accuracy of the fix.
                      It is a unitless number where smaller is better. For 3D fixes using 4 satellites a 1.0 would be considered to be a perfect number, however for overdetermined solutions it is possible to see numbers below 1.0.

                There are differences in the way the PRN's are presented which can effect the ability of some programs to display this data.
                For example, in the example shown below there are 5 satellites in the solution and the null fields are scattered indicating that the almanac would show satellites in the null positions that are not being used as part of this solution.
                Other receivers might output all of the satellites used at the beginning of the sentence with the null field all stacked up at the end.
                This difference accounts for some satellite display programs not always being able to display the satellites being tracked.
                Some units may show all satellites that have ephemeris data without regard to their use as part of the solution but this is non-standard.

                  $GPGSA,A,3,04,05,,09,12,,,24,,,,,2.5,1.3,2.1*39

                Where:
                     GSA      Satellite status
                     A        Auto selection of 2D or 3D fix (M = manual) 
                     3        3D fix - values include: 1 = no fix
                                                       2 = 2D fix
                                                       3 = 3D fix
                     04,05... PRNs of satellites used for fix (space for 12) 
                     2.5      PDOP (dilution of precision) 
                     1.3      Horizontal dilution of precision (HDOP) 
                     2.1      Vertical dilution of precision (VDOP)
                     *39      the checksum data, always begins with *
             */

            if (!_mapperVicinity.robotState.ignoreGps)
            {
                if (!notification.Body.IsValid)
                {
                    Tracer.Error("the GPS reported GPGSA with IsValid=false");
                }
                else
                {
                    if (traceGps)
                    {
                        Tracer.Trace(string.Format("the GPS reported GPGSA: {0} Precision: {1}  Mode: {2}", notification.Body.LastUpdate, notification.Body.HorizontalDilutionOfPrecision, notification.Body.Mode));
                    }

                    GpsState state = _state.gpsState;

                    // Status (like "2D Fix"):
                    state.GPGSA_Status = notification.Body.Status;

                    // Mode (like "Fix2D"):
                    state.GPGSA_Mode = notification.Body.Mode;

                    // Position Dilution Of Precision:
                    state.GPGSA_SphericalDilutionOfPrecision = notification.Body.SphericalDilutionOfPrecision;

                    // Horizontal Dilution Of Precision:
                    state.GPGSA_HorizontalDilutionOfPrecision = notification.Body.HorizontalDilutionOfPrecision;

                    // Vertical Dilution Of Precision:
                    state.GPGSA_VerticalDilutionOfPrecision = notification.Body.VerticalDilutionOfPrecision;

                    // Last Updated:
                    state.GPGSA_LastUpdate = notification.Body.LastUpdate;

                    if (!_testBumpMode && !_state.Dropping && !_doUnitTest)
                    {
                        Decide(SensorEventSource.Gps);
                    }
                }
            }
        }

        /// <summary>
        /// Handle GPS GPGSV Notifications - Satellites In View
        /// </summary>
        /// <param name="notification">GPGSV packet notification</param>
        private void GpGsvHandler(gps.GpGsvNotification notification)
        {
            /*
                GSV - Satellites in View shows data about the satellites that the unit might be able to find based on its viewing mask and almanac data.
                        It also shows current ability to track this data. Note that one GSV sentence only can provide data for up to 4 satellites and thus there may need to be 3 sentences for the full information.
                        It is reasonable for the GSV sentence to contain more satellites than GGA might indicate since GSV may include satellites that are not used as part of the solution.
                        It is not a requirment that the GSV sentences all appear in sequence.
                        To avoid overloading the data bandwidth some receivers may place the various sentences in totally different samples since each sentence identifies which one it is.

                        The field called SNR (Signal to Noise Ratio) in the NMEA standard is often referred to as signal strength.
                        SNR is an indirect but more useful value that raw signal strength.
                        It can range from 0 to 99 and has units of dB according to the NMEA standard, but the various manufacturers send different ranges of numbers with different starting numbers
                        so the values themselves cannot necessarily be used to evaluate different units. The range of working values in a given gps will usually show a difference of about 25 to 35 between the lowest and highest values,
                        however 0 is a special case and may be shown on satellites that are in view but not being tracked.

                  $GPGSV,2,1,08,01,40,083,46,02,17,308,41,12,07,344,39,14,22,228,45*75

                Where:
                      GSV          Satellites in view
                      2            Number of sentences for full data
                      1            sentence 1 of 2
                      08           Number of satellites in view

                      01           Satellite PRN number
                      40           Elevation, degrees
                      083          Azimuth, degrees
                      46           SNR - higher is better
                           for up to 4 satellites per sentence
                      *75          the checksum data, always begins with *
             */

            if (!_mapperVicinity.robotState.ignoreGps)
            {
                if (!notification.Body.IsValid)
                {
                    Tracer.Error("the GPS reported GPGSV with IsValid=false");
                }
                else
                {
                    if (traceGps)
                    {
                        Tracer.Trace(string.Format("the GPS reported GPGSV: {0} Satellites in View: {1}   {2}", notification.Body.LastUpdate, notification.Body.GsvSatellites.Count, notification.Body.SatellitesInView));
                    }

                    GpsState state = _state.gpsState;

                    // Number of Satellites In View
                    state.GPGSV_SatellitesInView = notification.Body.SatellitesInView;

                    // Last Updated:
                    state.GPGSV_LastUpdate = notification.Body.LastUpdate;

                    if (!_testBumpMode && !_state.Dropping && !_doUnitTest)
                    {
                        Decide(SensorEventSource.Gps);
                    }
                }
            }
        }

        /// <summary>
        /// Handle GPS GPRMC Notifications - Backup course, speed, and position (Sirf III sends this) - Time, date, position, course and speed data
        /// </summary>
        /// <param name="notification">GPRMC packet notification</param>
        private void GpRmcHandler(gps.GpRmcNotification notification)
        {
            /*
                RMC - NMEA has its own version of essential gps pvt (position, velocity, time) data. It is called RMC, The Recommended Minimum, which will look similar to:

                $GPRMC,123519,A,4807.038,N,01131.000,E,022.4,084.4,230394,003.1,W*6A

                Where:
                     RMC          Recommended Minimum sentence C
                     123519       Fix taken at 12:35:19 UTC
                     A            Status A=active or V=Void.
                     4807.038,N   Latitude 48 deg 07.038' N
                     01131.000,E  Longitude 11 deg 31.000' E
                     022.4        Speed over the ground in knots
                     084.4        Track angle in degrees True
                     230394       Date - 23rd of March 1994
                     003.1,W      Magnetic Variation
                     *6A          The checksum data, always begins with *
             
                Note that, as of the 2.3 release of NMEA, there is a new field in the RMC sentence at the end just prior to the checksum: 
              
                The last version 2 iteration of the NMEA standard was 2.3. It added a mode indicator to several sentences which is used to indicate the kind of fix the receiver currently has.
                This indication is part of the signal integrity information needed by the FAA. The value can be A=autonomous, D=differential, E=Estimated, N=not valid, S=Simulator.
                Sometimes there can be a null value as well. Only the A and D values will correspond to an Active and reliable Sentence.
                This mode character has been added to the RMC, RMB, VTG, and GLL, sentences and optionally some others including the BWC and XTE sentences.
             */

            if (!_mapperVicinity.robotState.ignoreGps)
            {
                if (!notification.Body.IsValid)
                {
                    Tracer.Error("the GPS reported GPRMC with IsValid=false");
                }
                else
                {
                    if (traceGps)
                    {
                        Tracer.Trace(string.Format("the GPS reported GPRMC: {0}  Lat: {1}  Lon: {2}", notification.Body.LastUpdate, notification.Body.Latitude, notification.Body.Longitude));
                    }

                    GpsState state = _state.gpsState;

                    // Status  (like "A"):
                    state.GPRMC_Status = notification.Body.Status;

                    // Latitude
                    state.GPRMC_Latitude = notification.Body.Latitude;

                    // Longitude:
                    state.GPRMC_Longitude = notification.Body.Longitude;

                    // Last Updated:
                    state.GPRMC_LastUpdate = notification.Body.LastUpdate;

                    if (!_testBumpMode && !_state.Dropping && !_doUnitTest)
                    {
                        Decide(SensorEventSource.Gps);
                    }
                }
            }
        }

        /// <summary>
        /// Handle GPS GPVTG Notifications - Ground Speed and Course
        /// </summary>
        /// <param name="notification">GPVTG packet notification</param>
        private void GpVtgHandler(gps.GpVtgNotification notification)
        {
            /*
                VTG - Velocity made good. The gps receiver may use the LC prefix instead of GP if it is emulating Loran output.

                  $GPVTG,054.7,T,034.4,M,005.5,N,010.2,K*48

                where:
                        VTG          Track made good and ground speed
                        054.7,T      True track made good (degrees)
                        034.4,M      Magnetic track made good
                        005.5,N      Ground speed, knots
                        010.2,K      Ground speed, Kilometers per hour
                        *48          Checksum
                Note that, as of the 2.3 release of NMEA, there is a new field in the VTG sentence at the end just prior to the checksum. For more information on this field see here.

                Receivers that don't have a magnetic deviation (variation) table built in will null out the Magnetic track made good.
             */

            if (!_mapperVicinity.robotState.ignoreGps)
            {
                if (!notification.Body.IsValid)
                {
                    Tracer.Error("the GPS reported GPVTG with IsValid=false");
                }
                else
                {
                    if (traceGps)
                    {
                        Tracer.Trace(string.Format("the GPS reported GPVTG: {0}  CourseDegrees: {1}  SpeedMetersPerSecond: {2}", notification.Body.LastUpdate, notification.Body.CourseDegrees, notification.Body.SpeedMetersPerSecond));
                    }

                    GpsState state = _state.gpsState;

                    // CourseDegrees:
                    state.GPVTG_CourseDegrees = notification.Body.CourseDegrees;

                    // SpeedMetersPerSecond:
                    state.GPVTG_SpeedMetersPerSecond = notification.Body.SpeedMetersPerSecond;

                    // Last Updated:
                    state.GPVTG_LastUpdate = notification.Body.LastUpdate;

                    if (!_testBumpMode && !_state.Dropping && !_doUnitTest)
                    {
                        Decide(SensorEventSource.Gps);
                    }
                }
            }
        }

        #endregion // Handle GPS messages

    }
}
