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
using System.ComponentModel;
using Microsoft.Ccr.Core;
using Microsoft.Dss.Core.Attributes;
using Microsoft.Dss.ServiceModel.Dssp;
using Microsoft.Dss.ServiceModel.DsspServiceBase;
using W3C.Soap;

using Microsoft.Dss.Core.DsspHttp;
using System.Diagnostics.CodeAnalysis;

namespace TrackRoamer.Robotics.Hardware.ChrUm6OrientationSensor
{
    /// <summary>
    /// ChrUm6OrientationSensor contract class
    /// </summary>
    public sealed class Contract
    {
        /// <summary>
        /// DSS contract identifer for ChrUm6OrientationSensor
        /// </summary>
        [DataMember]
        public const string Identifier = "http://schemas.trackroamer.com/2011/12/chrum6orientationsensor.html";
    }

    #region Operation Ports

    /// <summary>
    /// ChrUm6OrientationSensor main operations port
    /// </summary>
    [ServicePort]
    public class ChrUm6OrientationSensorOperations : PortSet<DsspDefaultLookup,
            DsspDefaultDrop,
            Get,
            Configure,
            FindChrConfig,
            Subscribe,
            SendChrUm6OrientationSensorCommand,
            HttpGet,
            HttpPost,
            ProcGyroNotification,
            ProcAccelNotification,
            ProcMagNotification,
            EulerNotification,
            QuaternionNotification>
    {
    }

    /// <summary>
    /// Gets the current state of the CH Robotics UM6 Orientation Sensor - get operation
    /// </summary>
#if DEBUG
    [SuppressMessage("Microsoft.Naming", "CA1716", Justification="Get is a Dss reserved word.")]
#endif
    [DisplayName("(User) Get")]
    [Description("Gets the current state of the CH Robotics UM6 Orientation Sensor.")]
    public class Get : Get<GetRequestType, PortSet<ChrUm6OrientationSensorState, Fault>>
    {
        /// <summary>
        /// Creates a new instance of Get
        /// </summary>
        public Get()
        {
        }

        /// <summary>
        /// Creates a new instance of Get
        /// </summary>
        /// <param name="body">the request message body</param>
        public Get(GetRequestType body)
            : base(body)
        {
        }

        /// <summary>
        /// Creates a new instance of Get
        /// </summary>
        /// <param name="body">the request message body</param>
        /// <param name="responsePort">the response port for the request</param>
        public Get(GetRequestType body, PortSet<ChrUm6OrientationSensorState, Fault> responsePort)
            : base(body, responsePort)
        {
        }
    }

    /// <summary>
    /// Configure CHR UM6 Sensor Operation
    /// </summary>
    [DisplayName("(User) Configure")]
    [Description("Configures a CH Robotics UM6 Orientation Sensor.")]
    public class Configure : Update<ChrUm6OrientationSensorConfig, PortSet<DefaultUpdateResponseType, Fault>>
    {
        /// <summary>
        /// Default Constructor
        /// </summary>
        public Configure() { }
        /// <summary>
        /// Initialization Constructor
        /// </summary>
        /// <param name="config"></param>
        public Configure(ChrUm6OrientationSensorConfig config)
        {
            this.Body = config;
        }
    }

    /// <summary>
    /// ChrUm6OrientationSensor subscribe operation
    /// </summary>
    public class Subscribe : Subscribe<SubscribeRequest, PortSet<SubscribeResponseType, Fault>, ChrUm6OrientationSensorOperations>
    {
        /// <summary>
        /// Creates a new instance of Subscribe
        /// </summary>
        public Subscribe()
        {
        }

        /// <summary>
        /// Creates a new instance of Subscribe
        /// </summary>
        /// <param name="body">the request message body</param>
        public Subscribe(SubscribeRequest body)
            : base(body)
        {
        }

        /// <summary>
        /// Creates a new instance of Subscribe
        /// </summary>
        /// <param name="body">the request message body</param>
        /// <param name="responsePort">the response port for the request</param>
        public Subscribe(SubscribeRequest body, PortSet<SubscribeResponseType, Fault> responsePort)
            : base(body, responsePort)
        {
        }
    }

    /// <summary>
    /// Finds and configures an installed CHR UM6 Sensor device.
    /// </summary>
    [DisplayName("(User) Find CH Robotics UM6 Orientation Sensor")]
    [Description("Finds and configures an installed CH Robotics UM6 Orientation Sensor.")]
    public class FindChrConfig : Query<ChrUm6OrientationSensorConfig, PortSet<ChrUm6OrientationSensorConfig, Fault>>
    {
        /// <summary>
        /// Finds and configures an installed CHR UM6 Sensor.
        /// </summary>
        public FindChrConfig()
        {
        }

        /// <summary>
        /// Finds and configures an installed CHR UM6 Sensor.
        /// </summary>
        /// <param name="body"></param>
        public FindChrConfig(ChrUm6OrientationSensorConfig body)
            : base(body)
        {
        }

        /// <summary>
        /// Finds and configures an installed CHR UM6 Sensor.
        /// </summary>
        /// <param name="body"></param>
        /// <param name="responsePort"></param>
        public FindChrConfig(ChrUm6OrientationSensorConfig body, Microsoft.Ccr.Core.PortSet<ChrUm6OrientationSensorConfig, W3C.Soap.Fault> responsePort)
            : base(body, responsePort)
        {
        }
    }


    /// <summary>
    /// Sends a command to the CHR UM6 Sensor.
    /// </summary>
    [DisplayName("(User) Send CHR UM6 Sensor Command")]
    [Description("Sends a command to the CHR UM6 Sensor.")]
    public class SendChrUm6OrientationSensorCommand : Update<ChrUm6OrientationSensorCommand, PortSet<DefaultUpdateResponseType, Fault>>
    {
        /// <summary>
        /// Sends a command to the CHR UM6 Sensor.
        /// </summary>
        public SendChrUm6OrientationSensorCommand()
        {
        }

        /// <summary>
        /// Sends a command to the CHR UM6 Sensor.
        /// </summary>
        /// <param name="body"></param>
        public SendChrUm6OrientationSensorCommand(ChrUm6OrientationSensorCommand body)
            :
                base(body)
        {
        }

        /// <summary>
        /// Sends a command to the CHR UM6 Sensor.
        /// </summary>
        /// <param name="body"></param>
        /// <param name="responsePort"></param>
        public SendChrUm6OrientationSensorCommand(ChrUm6OrientationSensorCommand body, Microsoft.Ccr.Core.PortSet<Microsoft.Dss.ServiceModel.Dssp.DefaultUpdateResponseType, W3C.Soap.Fault> responsePort)
            :
                base(body, responsePort)
        {
        }
    }

    /// <summary>
    /// QUAT Notification
    /// </summary>
    [DisplayName("(User) UpdateProcGyroData")]
    [Description("Indicates an update of processed rate gyroscope data after alignment, bias, and scale compensation have been applied.")]
    public class ProcGyroNotification : Update<DataProcGyro, DsspResponsePort<DefaultUpdateResponseType>>
    {
        /// <summary>
        /// Default Constructor
        /// </summary>
        /// <param name="data"></param>
        public ProcGyroNotification(DataProcGyro data) { this.Body = data; }

        /// <summary>
        /// Empty constructor
        /// </summary>
        public ProcGyroNotification() { }
    }

    /// <summary>
    /// QUAT Notification
    /// </summary>
    [DisplayName("(User) UpdateProcAccelData")]
    [Description("Indicates an update of processed accelerometer data after alignment, bias, and scale compensation have been applied.")]
    public class ProcAccelNotification : Update<DataProcAccel, DsspResponsePort<DefaultUpdateResponseType>>
    {
        /// <summary>
        /// Default Constructor
        /// </summary>
        /// <param name="data"></param>
        public ProcAccelNotification(DataProcAccel data) { this.Body = data; }

        /// <summary>
        /// Empty constructor
        /// </summary>
        public ProcAccelNotification() { }
    }

    /// <summary>
    /// QUAT Notification
    /// </summary>
    [DisplayName("(User) UpdateProcMagData")]
    [Description("Indicates an update of processed magnitometer data after soft and hard iron calibration.")]
    public class ProcMagNotification : Update<DataProcMag, DsspResponsePort<DefaultUpdateResponseType>>
    {
        /// <summary>
        /// Default Constructor
        /// </summary>
        /// <param name="data"></param>
        public ProcMagNotification(DataProcMag data) { this.Body = data; }

        /// <summary>
        /// Empty constructor
        /// </summary>
        public ProcMagNotification() { }
    }

    /// <summary>
    /// QUAT Notification
    /// </summary>
    [DisplayName("(User) UpdateEulerData")]
    [Description("Indicates an update of the orientation in the form of Euler angles.")]
    public class EulerNotification : Update<DataEuler, DsspResponsePort<DefaultUpdateResponseType>>
    {
        /// <summary>
        /// Default Constructor
        /// </summary>
        /// <param name="data"></param>
        public EulerNotification(DataEuler data) { this.Body = data; }

        /// <summary>
        /// Empty constructor
        /// </summary>
        public EulerNotification() { }
    }

    /// <summary>
    /// QUAT Notification
    /// </summary>
    [DisplayName("(User) UpdateQuaternionData")]
    [Description("Indicates an update of the orientation in the form of attitude quaternion.")]
    public class QuaternionNotification : Update<DataQuaternion, DsspResponsePort<DefaultUpdateResponseType>>
    {
        /// <summary>
        /// Default Constructor
        /// </summary>
        /// <param name="data"></param>
        public QuaternionNotification(DataQuaternion data) { this.Body = data; }

        /// <summary>
        /// Empty constructor
        /// </summary>
        public QuaternionNotification() { }
    }

    #endregion
}


