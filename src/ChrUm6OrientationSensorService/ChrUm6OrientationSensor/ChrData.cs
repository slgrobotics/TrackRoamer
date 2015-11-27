/*
* Copyright (c) 2011..., Sergei Grichine
* All rights reserved.
*
* Redistribution and use in source and binary forms, with or without
* modification, are permitted provided that the following conditions are met:
*     * Redistributions of source code must retain the above copyright
*       notice, this list of conditions and the following disclaimer.
*     * Redistributions in binary form must reproduce the above copyright
*       notice, this list of conditions and the following disclaimer in the
*       documentation and/or other materials provided with the distribution.
*     * Neither the name of Sergei Grichine nor the
*       names of contributors may be used to endorse or promote products
*       derived from this software without specific prior written permission.
*
* THIS SOFTWARE IS PROVIDED BY Sergei Grichine ''AS IS'' AND ANY
* EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED
* WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE
* DISCLAIMED. IN NO EVENT SHALL Sergei Grichine BE LIABLE FOR ANY
* DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES
* (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES;
* LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND
* ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT
* (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS
* SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
*
* this is a X11 (BSD Revised) license - you do not have to publish your changes,
* although doing so, donating and contributing is always appreciated
*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;

using W3C.Soap;

using Microsoft.Dss.Core.Attributes;
using Microsoft.Dss.ServiceModel.Dssp;

namespace TrackRoamer.Robotics.Hardware.ChrUm6OrientationSensor
{
    public enum Um6RegisterAddress
    {
        // see UM1B.XML
        UM6_GYRO_PROC = 92,
        UM6_ACCEL_PROC = 94,
        UM6_MAG_PROC = 96,
        UM6_EULER = 98,
        UM6_QUAT = 100
    }

    #region Orientation Sensor State Type

    // Some of the code here is close to GpsData.cs from Microsoft RDS R3

    /// <summary>
    /// CH Robotics UM6 Orientation Sensor State
    /// </summary>
    [DataContract]
    [Description("The CHR UM6 Orientation Sensor state")]
    public class ChrUm6OrientationSensorState
    {
        /// <summary>
        /// Is the Chr unit currently connected.
        /// </summary>
        [DataMember]
        [Description("Indicates that the CHR UM6 Sensor is connected.")]
        [Browsable(false)]
        public bool Connected { get; set; }

        /// <summary>
        /// Serial Port Configuration
        /// </summary>
        [DataMember(IsRequired = true)]
        [Description("Specifies the serial port where the CHR UM6 Sensor is connected.")]
        public ChrUm6OrientationSensorConfig ChrUm6OrientationSensorConfig { get; set; }

        /// <summary>
        /// Orientation Data - ProcGyro
        /// </summary>
        [DataMember(IsRequired = false)]
        [Browsable(false)]
        [Description("Indicates Orientation Data - ProcGyro.")]
        public DataProcGyro ProcGyro { get; set; }

        /// <summary>
        /// Orientation Data - ProcAccel
        /// </summary>
        [DataMember(IsRequired = false)]
        [Browsable(false)]
        [Description("Indicates Orientation Data - ProcAccel.")]
        public DataProcAccel ProcAccel { get; set; }

        /// <summary>
        /// Orientation Data - ProcMag
        /// </summary>
        [DataMember(IsRequired = false)]
        [Browsable(false)]
        [Description("Indicates Orientation Data - ProcMag.")]
        public DataProcMag ProcMag { get; set; }

        /// <summary>
        /// Orientation Data - Euler angles
        /// </summary>
        [DataMember(IsRequired = false)]
        [Browsable(false)]
        [Description("Indicates Orientation Data - Euler angles.")]
        public DataEuler Euler { get; set; }

        /// <summary>
        /// Orientation Data - Quaternion
        /// </summary>
        [DataMember(IsRequired = false)]
        [Browsable(false)]
        [Description("Indicates Orientation Data - Quaternion.")]
        public DataQuaternion Quaternion { get; set; }
    }

    #endregion //  Chr State Type

    #region UM6 Configuration

    /// <summary>
    /// ChrUm6OrientationSensor Serial Port Configuration
    /// </summary>
    [DataContract]
    [DisplayName("(User) ChrUm6OrientationSensor Configuration")]
    [Description("ChrUm6OrientationSensor Serial Port Configuration")]
    public class ChrUm6OrientationSensorConfig : ICloneable
    {
        /// <summary>
        /// The Serial Comm Port
        /// </summary>
        [DataMember]
        [Description("Chr COM Port")]
        public int CommPort { get; set; }

        /// <summary>
        /// The Serial Port Name or the File name containing Chr readings
        /// </summary>
        [DataMember]
        [Description("The Serial Port Name or the File name containing Chr readings (Default blank)")]
        public string PortName { get; set; }

        /// <summary>
        /// Baud Rate
        /// </summary>
        [DataMember]
        [Description("Chr Baud Rate")]
        public int BaudRate { get; set; }

        /// <summary>
        /// Configuration Status
        /// </summary>
        [DataMember]
        [Browsable(false)]
        [Description("Chr Configuration Status")]
        public string ConfigurationStatus { get; set; }

        /// <summary>
        /// Default Constructor
        /// </summary>
        public ChrUm6OrientationSensorConfig()
        {
            this.BaudRate = 115200;
            this.CommPort = 0;
            this.PortName = string.Empty;
        }

        #region ICloneable implementation

        public object Clone()
        {
            return new ChrUm6OrientationSensorConfig()
            {
                BaudRate = this.BaudRate,
                CommPort = this.CommPort,
                PortName = this.PortName
            };
        }

        #endregion  // ICloneable implementation
    }

    /// <summary>
    /// A CHR UM6 Sensor Command
    /// <remarks>Use with SendChrUm6OrientationSensorCommand()</remarks>
    /// </summary>
    [DataContract]
    [Description("A CHR UM6 Sensor Command")]
    public class ChrUm6OrientationSensorCommand
    {
        [DataMember]
        public string Command { get; set; }
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
        public ChrMessageType MessageTypes;

        /// <summary>
        /// Only subscribe to messages when IsValid is true
        /// </summary>
        [DataMember]
        public bool ValidOnly;
    }

    #endregion

    #region UM6 Data Structures

    /// <summary>
    ///  Processed gyroscope data
    /// </summary>
    [DataContract]
    [Description("Indicates the processed gyroscope data.")]
    public class DataProcGyro
    {
        [DataMember]
        [Description("X-axis rate gyro output after alignment, bias, and scale compensation have been applied. Stored as 16-bit signed integer.")]
        public short xRate;

        [DataMember]
        [Description("Y-axis rate gyro output after alignment, bias, and scale compensation have been applied. Stored as 16-bit signed integer.")]
        public short yRate;

        [DataMember]
        [Description("Z-axis rate gyro output after alignment, bias, and scale compensation have been applied. Stored as 16-bit signed integer.")]
        public short zRate;

        [DataMember]
        [Description("Indicates the time of the last reading update.")]
        public System.DateTime LastUpdate { get; set; }
    }

    /// <summary>
    /// Processed accelerometer data
    /// </summary>
    [DataContract]
    [Description("Indicates the processed accelerometer data.")]
    public class DataProcAccel
    {
        [DataMember]
        [Description("X-axis accelerometer output after alignment, bias, and scale compensation have been applied. Stored as two 16-bit signed integer.")]
        public short xAccel;

        [DataMember]
        [Description("Y-axis accelerometer output after alignment, bias, and scale compensation have been applied. Stored as two 16-bit signed integer.")]
        public short yAccel;

        [DataMember]
        [Description("Z-axis accelerometer output after alignment, bias, and scale compensation have been applied. Stored as two 16-bit signed integer.")]
        public short zAccel;

        [DataMember]
        [Description("Indicates the time of the last reading update.")]
        public System.DateTime LastUpdate { get; set; }
    }

    /// <summary>
    /// Processed magnitometer data
    /// </summary>
    [DataContract]
    [Description("Indicates the processed magnitometer data.")]
    public class DataProcMag
    {
        [DataMember]
        [Description("X-axis magnetometer output after soft and hard iron calibration. Stored as 16-bit signed integer.")]
        public short x;

        [DataMember]
        [Description("Y-axis magnetometer output after soft and hard iron calibration. Stored as 16-bit signed integer.")]
        public short y;

        [DataMember]
        [Description("Z-axis magnetometer output after soft and hard iron calibration. Stored as 16-bit signed integer.")]
        public short z;

        [DataMember]
        [Description("Indicates the time of the last reading update.")]
        public System.DateTime LastUpdate { get; set; }
    }

    /// <summary>
    /// Euler angles data
    /// </summary>
    [DataContract]
    [Description("Indicates the Euler angles data.")]
    public class DataEuler
    {
        [DataMember]
        [Description("Euler PHI - Roll angle in degrees. Stored as 16-bit signed integer.")]
        public short phi;

        [DataMember]
        [Description("Euler THETA - Pitch angle in degrees. Stored as 16-bit signed integer.")]
        public short theta;

        [DataMember]
        [Description("Euler PSI - Yaw angle in degrees. Stored as 16-bit signed integer.")]
        public short psi;

        [DataMember]
        [Description("Indicates the time of the last reading update.")]
        public System.DateTime LastUpdate { get; set; }
    }

    /// <summary>
    /// Quaternion data
    /// </summary>
    [DataContract]
    [Description("Indicates the attitude quaternion data.")]
    public class DataQuaternion
    {
        [DataMember]
        [Description("Attitude quaternion part a. Stored as two 16-bit signed integer.")]
        public short a;

        [DataMember]
        [Description("Attitude quaternion part b. Stored as two 16-bit signed integer.")]
        public short b;

        [DataMember]
        [Description("Attitude quaternion part c. Stored as two 16-bit signed integer.")]
        public short c;

        [DataMember]
        [Description("Attitude quaternion part d. Stored as two 16-bit signed integer.")]
        public short d;

        [DataMember]
        [Description("Indicates the time of the last reading update.")]
        public System.DateTime LastUpdate { get; set; }
    }
    #endregion

    /// <summary>
    /// Chr Message Type - bitwise OR for subscribing to selected types only
    /// </summary>
    [DataContract, Flags]
    [Description("Identifies the UM6 message types bitmask for subscriptions.")]
    public enum ChrMessageType
    {
        /// <summary>
        /// No-message mask
        /// </summary>
        None = 0,

        /// <summary>
        /// processed gyroscope data
        /// </summary>
        GYRO_PROC = 0x01,

        /// <summary>
        /// processed accelerometer data
        /// </summary>
        ACCEL_PROC = 0x02,

        /// <summary>
        /// processed magnitometer data
        /// </summary>
        MAG_PROC = 0x04,

        /// <summary>
        /// Euler angles (beware of 90 degrees singularity)
        /// </summary>
        EULER = 0x08,

        /// <summary>
        /// quaternion data (no singularities)
        /// </summary>
        QUAT = 0x10,

        /// <summary>
        /// Subscribe to all message types
        /// </summary>
        All = GYRO_PROC | ACCEL_PROC | MAG_PROC | EULER | QUAT      // use "|" to combine all types here into a bitmask
    }

}

