using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Dss.Core;
using Microsoft.Dss.Core.Attributes;
using gps = Microsoft.Robotics.Services.Sensors.Gps.Proxy;

namespace TrackRoamer.Robotics.LibMapping
{
    /// <summary>
    /// represents GPS data as it comes from GPS Service. Provides methods to retrieve current best coordinates and data quality
    /// in the terms good enough for SLAM computations 
    /// </summary>
    [DataContract]
    public class GpsState : IDssSerializable
    {
        public gps.PositionFixIndicator     GPGGA_PositionFixIndicator;
        public double                       GPGGA_AltitudeMeters;
        public double                       GPGGA_Latitude;
        public double                       GPGGA_Longitude;
        public double                       GPGGA_HorizontalDilutionOfPrecision;
        public int                          GPGGA_SatellitesUsed;
        public DateTime?                    GPGGA_LastUpdate;

        public string                       GPGLL_MarginOfError;
        public string                       GPGLL_Status;
        public double                       GPGLL_Latitude;
        public double                       GPGLL_Longitude;
        public DateTime?                    GPGLL_LastUpdate;

        public string                       GPGSA_Status;
        public gps.GsaMode                  GPGSA_Mode;
        public double                       GPGSA_SphericalDilutionOfPrecision;
        public double                       GPGSA_HorizontalDilutionOfPrecision;
        public double                       GPGSA_VerticalDilutionOfPrecision;
        public DateTime?                    GPGSA_LastUpdate;

        public int                          GPGSV_SatellitesInView;
        public DateTime?                    GPGSV_LastUpdate;

        public string                       GPRMC_Status;
        public double                       GPRMC_Latitude;
        public double                       GPRMC_Longitude;
        public DateTime?                    GPRMC_LastUpdate;

        public double                       GPVTG_CourseDegrees;
        public double                       GPVTG_SpeedMetersPerSecond;
        public DateTime?                    GPVTG_LastUpdate;

        #region IDssSerializable semi-fake implementation to avoid DSSProxy warning

        /// <summary>
        /// copies all members to a target
        /// </summary>
        /// <param name="target"></param>
        public virtual void CopyTo(IDssSerializable target)
        {
            // throw new NotImplementedException("class GpsState does not have to implement IDssSerializable - do not call CopyTo()");

            GpsState typedTarget = target as GpsState;

            if (typedTarget == null)
                throw new ArgumentException("GpsState::CopyTo({0}) requires type {0}", this.GetType().FullName);

            typedTarget.GPGGA_PositionFixIndicator = this.GPGGA_PositionFixIndicator;
            typedTarget.GPGGA_AltitudeMeters = this.GPGGA_AltitudeMeters;
            typedTarget.GPGGA_Latitude = this.GPGGA_Latitude;
            typedTarget.GPGGA_Longitude = this.GPGGA_Longitude;
            typedTarget.GPGGA_HorizontalDilutionOfPrecision = this.GPGGA_HorizontalDilutionOfPrecision;
            typedTarget.GPGGA_SatellitesUsed = this.GPGGA_SatellitesUsed;
            typedTarget.GPGGA_LastUpdate = this.GPGGA_LastUpdate;

            typedTarget.GPGLL_MarginOfError = this.GPGLL_MarginOfError;
            typedTarget.GPGLL_Status = this.GPGLL_Status;
            typedTarget.GPGLL_Latitude = this.GPGLL_Latitude;
            typedTarget.GPGLL_Longitude = this.GPGLL_Longitude;
            typedTarget.GPGLL_LastUpdate = this.GPGLL_LastUpdate;

            typedTarget.GPGSA_Status = this.GPGSA_Status;
            typedTarget.GPGSA_Mode = this.GPGSA_Mode;
            typedTarget.GPGSA_SphericalDilutionOfPrecision = this.GPGSA_SphericalDilutionOfPrecision;
            typedTarget.GPGSA_HorizontalDilutionOfPrecision = this.GPGSA_HorizontalDilutionOfPrecision;
            typedTarget.GPGSA_VerticalDilutionOfPrecision = this.GPGSA_VerticalDilutionOfPrecision;
            typedTarget.GPGSA_LastUpdate = this.GPGSA_LastUpdate;

            typedTarget.GPGSV_SatellitesInView = this.GPGSV_SatellitesInView;
            typedTarget.GPGSV_LastUpdate = this.GPGSV_LastUpdate;

            typedTarget.GPRMC_Status = this.GPRMC_Status;
            typedTarget.GPRMC_Latitude = this.GPRMC_Latitude;
            typedTarget.GPRMC_Longitude = this.GPRMC_Longitude;
            typedTarget.GPRMC_LastUpdate = this.GPRMC_LastUpdate;

            typedTarget.GPVTG_CourseDegrees = this.GPVTG_CourseDegrees;
            typedTarget.GPVTG_SpeedMetersPerSecond = this.GPVTG_SpeedMetersPerSecond;
            typedTarget.GPVTG_LastUpdate = this.GPVTG_LastUpdate;
        }

        /// <summary>
        /// do not call, method Not Implemented
        /// </summary>
        /// <returns></returns>
        public object Clone()
        {
            throw new NotImplementedException("class GpsState does not have to implement IDssSerializable - do not call Clone()");
        }

        /// <summary>
        /// do not call, method Not Implemented
        /// </summary>
        /// <param name="writer"></param>
        public virtual void Serialize(System.IO.BinaryWriter writer)
        {
            throw new NotImplementedException("class GpsState does not have to implement IDssSerializable - do not call Serialize()");
        }

        /// <summary>
        /// do not call, method Not Implemented
        /// </summary>
        /// <param name="reader"></param>
        /// <returns></returns>
        public virtual object Deserialize(System.IO.BinaryReader reader)
        {
            throw new NotImplementedException("class GpsState does not have to implement IDssSerializable - do not call Deserialize()");
        }
        #endregion  // IDssSerializable semi-fake implementation to avoid DSSProxy warning

    }
}
