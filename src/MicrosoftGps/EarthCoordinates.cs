//-----------------------------------------------------------------------
//  This file is part of Microsoft Robotics Developer Studio Code Samples.
//
//  Copyright (C) Microsoft Corporation.  All rights reserved.
//
//  $File: EarthCoordinates.cs $ $Revision: 1 $
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.ComponentModel;
using Microsoft.Dss.Core.Attributes;


namespace Microsoft.Robotics.Services.Sensors.Gps
{

    /// <summary>
    /// Convert Gps Latitude, Longitude and Altitude to Earth Coordinates in meters
    /// <remarks>
    /// Based on an algorithm published by C. Gregg Carlson, South Dakota State University
    /// "Reading and understanding a Gps receiver"
    /// http://plantsci.sdstate.edu/precisionfarm/paper/papers/EARTHMOD.pdf
    /// </remarks>
    /// </summary>
    [DataContract]
    public class EarthCoordinates
    {
        #region Private Members
        private const double _equatorialRadius = 6378137.0000;
        private const double _polarRadius = 6356752.3142;

        private double _trueLatitudeAngle;
        private double _earthLatitudeRadius;
        private double _polarAxisMeters;
        private double _equatorialPlaneMeters;

        private double _latitude;
        private double _longitude;
        private double _altitudeMeters;
        private DateTime _dateTime;
        private double _horizontalDilutionOfPrecision;
        private double _verticalDilutionOfPrecision;

        #endregion

        /// <summary>
        /// Latitude
        /// </summary>
        [DataMember]
        [Description("Indicates the latitude.")]
        public double Latitude
        {
            get { return _latitude; }
            set { _latitude = value; }
        }

        /// <summary>
        /// Longitude
        /// </summary>
        [DataMember]
        [Description("Indicates the longitude.")]
        public double Longitude
        {
            get { return _longitude; }
            set { _longitude = value; }
        }

        /// <summary>
        /// Altitude in Meters
        /// </summary>
        [DataMember]
        [Description("Indicates the altitude (m).")]
        public double AltitudeMeters
        {
            get { return _altitudeMeters; }
            set { _altitudeMeters = value; }
        }

        /// <summary>
        /// Latitude and Longitude Dilution Of Precision
        /// </summary>
        [DataMember]
        [Description("Indicates the latitude and longitude dilution of precision.")]
        public double HorizontalDilutionOfPrecision
        {
            get { return _horizontalDilutionOfPrecision; }
            set { _horizontalDilutionOfPrecision = value; }
        }

        /// <summary>
        /// Altitude Dilution Of Precision
        /// </summary>
        [Description("Indicates the altitude dilution of precision.")]
        [DataMember]
        public double VerticalDilutionOfPrecision
        {
            get { return _verticalDilutionOfPrecision; }
            set { _verticalDilutionOfPrecision = value; }
        }

        /// <summary>
        /// Time of reading
        /// </summary>
        [DataMember]
        [Description("Indicates the time of the reading.")]
        public DateTime DateTime
        {
            get { return _dateTime; }
            set { _dateTime = value; }
        }

        /// <summary>
        /// Distance in meters between two Earth Coordinates.
        /// </summary>
        /// <param name="start"></param>
        /// <returns></returns>
        public double DistanceFromStart(EarthCoordinates start)
        {
            // Distance between start and end (Latitude and Altitude) in meters
            double x = Pythagorean(start._polarAxisMeters - this._polarAxisMeters, start._equatorialPlaneMeters - this._equatorialPlaneMeters);

            // Distance between start and end Longitude in meters.
            double y = 2.0 * Math.PI * ((((start._polarAxisMeters + this._polarAxisMeters) / 2.0)) / 360.0) * (start.Longitude - this.Longitude);

            // Distance between start and end
            return Pythagorean(x,y);
        }

        /// <summary>
        /// Returns three dimensional cartesian coordinates for this EarthCoordinate
        /// with vertical orientation to the center of the earth
        /// and horizontal orientation to the polar coordinates
        /// <remarks>
        /// X is meters -East/+West from the start
        /// Y is meters -South/+North from the start
        /// Z is meters -Below/+Above the start
        /// </remarks>
        /// </summary>
        /// <param name="start"></param>
        /// <returns></returns>
        public Point3 OffsetFromStart(EarthCoordinates start)
        {
            // Distance between start and end Longitude in meters.
            double y = 2.0 * Math.PI * ((((start._polarAxisMeters + this._polarAxisMeters) / 2.0)) / 360.0) * (start.Longitude - this.Longitude);

            // Difference in altitude
            double z = (this.AltitudeMeters - start.AltitudeMeters);

            // To calculate the difference in Latitude,
            // find the Longitude and Altitude midpoints
            double midLongitude = (start.Longitude + this.Longitude) / 2.0;
            double midAltitude = (start.AltitudeMeters + this.AltitudeMeters) / 2.0;

            EarthCoordinates lat1;
            if ((Math.Abs(midAltitude) > 20.0) || (Math.Abs(midLongitude) > 0.0001))
            {
                // The points are spread apart,
                // so plot two new points at the starting and ending Latitude
                // and using the midpoints of Longitude and Altitude.
                lat1 = new EarthCoordinates(start.Latitude, midLongitude, midAltitude, start.DateTime, start.HorizontalDilutionOfPrecision, start.VerticalDilutionOfPrecision);
            }
            else
            {
                // The points are near the same Longitude and Altitude,
                // so calculate Latitude using the start Longitude and Altitude.
                lat1 = start;
                midLongitude = start.Longitude;
                midAltitude = start.AltitudeMeters;
            }

            EarthCoordinates lat2 = new EarthCoordinates(this.Latitude, midLongitude, midAltitude, this.DateTime, this.HorizontalDilutionOfPrecision, this.VerticalDilutionOfPrecision);

            // Finally, measure the distance between these points.
            double x = lat2.DistanceFromStart(lat1);


            return new Point3(x, y, z);
        }

        #region Intermediate Calculations

        /// <summary>
        /// Calculate the true latitude angle based on ellipsoid model of the earth.
        /// <remarks>WGS-84 Spheroid model (National Imagery and Mapping Agency, 1997)</remarks>
        /// </summary>
        /// <param name="latitude"></param>
        /// <returns></returns>
        private static double TrueLatitudeAngle(double latitude)
        {
            return (Math.Atan(Math.Pow(_polarRadius, 2.0) / Math.Pow(_equatorialRadius, 2.0) * Math.Tan(latitude * Math.PI / 180.0))) * 180.0 / Math.PI;
        }

        /// <summary>
        /// Calculate the latitudinal radius of the earth
        /// at the specified angle from the equator
        /// and taking into consideration the altitude
        /// measured in meters above sea level.
        /// </summary>
        /// <param name="trueAngle"></param>
        /// <param name="altitudeMeters"></param>
        /// <returns></returns>
        private static double EarthLatitudeRadius(double trueAngle, double altitudeMeters)
        {
            return Math.Pow((1 / (Math.Pow((Math.Cos(trueAngle * Math.PI / 180.0)), 2.0) / Math.Pow(_equatorialRadius, 2.0) + Math.Pow((Math.Sin(trueAngle * Math.PI / 180.0)), 2.0) / Math.Pow(_polarRadius, 2.0))), 0.5) + altitudeMeters;
        }

        /// <summary>
        /// Calculate the hypotenuse of a right triangle.
        /// </summary>
        /// <param name="side1"></param>
        /// <param name="side2"></param>
        /// <returns></returns>
        private static double Pythagorean(double side1, double side2)
        {
            return Math.Pow(Math.Pow(side1, 2.0) + Math.Pow(side2, 2.0), 0.5);
        }
        #endregion

        #region Constructors

        /// <summary>
        /// Default Constructor
        /// </summary>
        public EarthCoordinates() { }

        /// <summary>
        /// Initialization Constructor
        /// </summary>
        /// <param name="latitude"></param>
        /// <param name="longitude"></param>
        /// <param name="altitudeMeters"></param>
        public EarthCoordinates(double latitude, double longitude, double altitudeMeters)
        {
            Initialize(latitude, longitude, altitudeMeters, DateTime.Now, 0.0, 0.0);
        }


        /// <summary>
        /// Initialization Constructor
        /// </summary>
        /// <param name="latitude"></param>
        /// <param name="longitude"></param>
        /// <param name="altitudeMeters"></param>
        /// <param name="dateTime"></param>
        /// <param name="horizontalDilutionOfPrecision"></param>
        /// <param name="verticalDilutionOfPrecision"></param>
        public EarthCoordinates(double latitude, double longitude, double altitudeMeters, DateTime dateTime, double horizontalDilutionOfPrecision, double verticalDilutionOfPrecision)
        {
            Initialize(latitude, longitude, altitudeMeters, dateTime, horizontalDilutionOfPrecision, verticalDilutionOfPrecision);
        }


        /// <summary>
        /// Initialize earth coordinates
        /// </summary>
        /// <param name="latitude"></param>
        /// <param name="longitude"></param>
        /// <param name="altitudeMeters"></param>
        /// <param name="dateTime"></param>
        /// <param name="horizontalDilutionOfPrecision"></param>
        /// <param name="verticalDilutionOfPrecision"></param>
        private void Initialize(double latitude, double longitude, double altitudeMeters, DateTime dateTime, double horizontalDilutionOfPrecision, double verticalDilutionOfPrecision)
        {
            this._latitude = latitude;
            this._longitude = longitude;
            this._altitudeMeters = altitudeMeters;
            this._dateTime = dateTime;
            this._horizontalDilutionOfPrecision = horizontalDilutionOfPrecision;
            this._verticalDilutionOfPrecision = verticalDilutionOfPrecision;
            this._trueLatitudeAngle = TrueLatitudeAngle(latitude);
            this._earthLatitudeRadius = EarthLatitudeRadius(_trueLatitudeAngle, altitudeMeters);

            // Meters from the Polar Axis
            this._polarAxisMeters = this._earthLatitudeRadius * Math.Cos(_trueLatitudeAngle * Math.PI / 180.0);

            // Meters from the Equatorial plane
            this._equatorialPlaneMeters = this._earthLatitudeRadius * Math.Sin(_trueLatitudeAngle * Math.PI / 180.0);
        }

        #endregion

    }

}
