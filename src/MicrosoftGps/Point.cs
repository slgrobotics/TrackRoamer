//-----------------------------------------------------------------------
//  This file is part of Microsoft Robotics Developer Studio Code Samples.
//
//  Copyright (C) Microsoft Corporation.  All rights reserved.
//
//  $File: Point.cs $ $Revision: 1 $
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using Microsoft.Dss.Core.Attributes;


namespace Microsoft.Robotics.Services.Sensors.Gps
{

    /// <summary>
    /// A Three Dimension point
    /// </summary>
    [DataContract]
    public class Point3
    {

        /// <summary>
        /// X Coordinate
        /// </summary>
        [DataMember]
        public double X;

        /// <summary>
        /// Y Coordinate
        /// </summary>
        [DataMember]
        public double Y;

        /// <summary>
        /// Z Coordinate
        /// </summary>
        [DataMember]
        public double Z;

        #region Constructors and Conversions

        /// <summary>
        /// Default Point3 Constructor
        /// </summary>
        public Point3()
        {
        }

        /// <summary>
        /// Point3 initialization constructor
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="z"></param>
        public Point3(double x, double y, double z)
        {
            this.X = x;
            this.Y = y;
            this.Z = z;
        }

        #endregion
    }
}
