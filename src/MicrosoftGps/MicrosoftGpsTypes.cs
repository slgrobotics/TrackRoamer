//-----------------------------------------------------------------------
//  This file is part of Microsoft Robotics Developer Studio Code Samples.
//
//  Copyright (C) Microsoft Corporation.  All rights reserved.
//
//  $File: MicrosoftGpsTypes.cs $ $Revision: 1 $
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

using W3C.Soap;

using Microsoft.Ccr.Core;
using Microsoft.Dss.Core.Attributes;
using Microsoft.Dss.Core.DsspHttp;
using Microsoft.Dss.ServiceModel.Dssp;
using System.ComponentModel;


namespace Microsoft.Robotics.Services.Sensors.Gps
{
    #region Operation Ports

    /// <summary>
    /// Microsoft Gps Main Operation Port
    /// </summary>
    [ServicePort]
    public class MicrosoftGpsOperations : PortSet
    {
        /// <summary>
        /// Microsoft Gps Main Operation Port
        /// </summary>
        public MicrosoftGpsOperations()
            : base(
                typeof(DsspDefaultLookup),
                typeof(DsspDefaultDrop),
                typeof(Get),
                typeof(Configure),
                typeof(FindGpsConfig),
                typeof(Subscribe),
                typeof(SendMicrosoftGpsCommand),
                typeof(HttpGet),
                typeof(HttpPost),
                typeof(GpGgaNotification),
                typeof(GpGllNotification),
                typeof(GpGsaNotification),
                typeof(GpGsvNotification),
                typeof(GpRmcNotification),
                typeof(GpVtgNotification)
        ) { }


        #region Implicit Operators

        /// <summary>
        /// Implicit Operator for Port of DsspDefaultLookup
        /// </summary>
        /// <param name="portSet"></param>
        /// <returns></returns>
        public static implicit operator Port<DsspDefaultLookup>(MicrosoftGpsOperations portSet)
        {
            if (portSet == null) return null;
            return (Port<DsspDefaultLookup>)portSet[typeof(DsspDefaultLookup)];
        }
        /// <summary>
        /// Implicit Operator for Port of DsspDefaultDrop
        /// </summary>
        /// <param name="portSet"></param>
        /// <returns></returns>
        public static implicit operator Port<DsspDefaultDrop>(MicrosoftGpsOperations portSet)
        {
            if (portSet == null) return null;
            return (Port<DsspDefaultDrop>)portSet[typeof(DsspDefaultDrop)];
        }
        /// <summary>
        /// Implicit Operator for Port of Get
        /// </summary>
        /// <param name="portSet"></param>
        /// <returns></returns>
        public static implicit operator Port<Get>(MicrosoftGpsOperations portSet)
        {
            if (portSet == null) return null;
            return (Port<Get>)portSet[typeof(Get)];
        }
        /// <summary>
        /// Implicit Operator for Port of Configure
        /// </summary>
        /// <param name="portSet"></param>
        /// <returns></returns>
        public static implicit operator Port<Configure>(MicrosoftGpsOperations portSet)
        {
            if (portSet == null) return null;
            return (Port<Configure>)portSet[typeof(Configure)];
        }
        /// <summary>
        /// Implicit Operator for Port of FindGpsConfig
        /// </summary>
        /// <param name="portSet"></param>
        /// <returns></returns>
        public static implicit operator Port<FindGpsConfig>(MicrosoftGpsOperations portSet)
        {
            if (portSet == null) return null;
            return (Port<FindGpsConfig>)portSet[typeof(FindGpsConfig)];
        }
        /// <summary>
        /// Implicit Operator for Port of Subscribe
        /// </summary>
        /// <param name="portSet"></param>
        /// <returns></returns>
        public static implicit operator Port<Subscribe>(MicrosoftGpsOperations portSet)
        {
            if (portSet == null) return null;
            return (Port<Subscribe>)portSet[typeof(Subscribe)];
        }
        /// <summary>
        /// Implicit Operator for Port of SendMicrosoftGpsCommand
        /// </summary>
        /// <param name="portSet"></param>
        /// <returns></returns>
        public static implicit operator Port<SendMicrosoftGpsCommand>(MicrosoftGpsOperations portSet)
        {
            if (portSet == null) return null;
            return (Port<SendMicrosoftGpsCommand>)portSet[typeof(SendMicrosoftGpsCommand)];
        }
        /// <summary>
        /// Implicit Operator for Port of HttpGet
        /// </summary>
        /// <param name="portSet"></param>
        /// <returns></returns>
        public static implicit operator Port<HttpGet>(MicrosoftGpsOperations portSet)
        {
            if (portSet == null) return null;
            return (Port<HttpGet>)portSet[typeof(HttpGet)];
        }
        /// <summary>
        /// Implicit Operator for Port of HttpPost
        /// </summary>
        /// <param name="portSet"></param>
        /// <returns></returns>
        public static implicit operator Port<HttpPost>(MicrosoftGpsOperations portSet)
        {
            if (portSet == null) return null;
            return (Port<HttpPost>)portSet[typeof(HttpPost)];
        }
        /// <summary>
        /// Implicit Operator for Port of GpGgaNotification
        /// </summary>
        /// <param name="portSet"></param>
        /// <returns></returns>
        public static implicit operator Port<GpGgaNotification>(MicrosoftGpsOperations portSet)
        {
            if (portSet == null) return null;
            return (Port<GpGgaNotification>)portSet[typeof(GpGgaNotification)];
        }
        /// <summary>
        /// Implicit Operator for Port of GpGllNotification
        /// </summary>
        /// <param name="portSet"></param>
        /// <returns></returns>
        public static implicit operator Port<GpGllNotification>(MicrosoftGpsOperations portSet)
        {
            if (portSet == null) return null;
            return (Port<GpGllNotification>)portSet[typeof(GpGllNotification)];
        }
        /// <summary>
        /// Implicit Operator for Port of GpGsaNotification
        /// </summary>
        /// <param name="portSet"></param>
        /// <returns></returns>
        public static implicit operator Port<GpGsaNotification>(MicrosoftGpsOperations portSet)
        {
            if (portSet == null) return null;
            return (Port<GpGsaNotification>)portSet[typeof(GpGsaNotification)];
        }
        /// <summary>
        /// Implicit Operator for Port of GpGsvNotification
        /// </summary>
        /// <param name="portSet"></param>
        /// <returns></returns>
        public static implicit operator Port<GpGsvNotification>(MicrosoftGpsOperations portSet)
        {
            if (portSet == null) return null;
            return (Port<GpGsvNotification>)portSet[typeof(GpGsvNotification)];
        }
        /// <summary>
        /// Implicit Operator for Port of GpRmcNotification
        /// </summary>
        /// <param name="portSet"></param>
        /// <returns></returns>
        public static implicit operator Port<GpRmcNotification>(MicrosoftGpsOperations portSet)
        {
            if (portSet == null) return null;
            return (Port<GpRmcNotification>)portSet[typeof(GpRmcNotification)];
        }
        /// <summary>
        /// Implicit Operator for Port of GpVtgNotification
        /// </summary>
        /// <param name="portSet"></param>
        /// <returns></returns>
        public static implicit operator Port<GpVtgNotification>(MicrosoftGpsOperations portSet)
        {
            if (portSet == null) return null;
            return (Port<GpVtgNotification>)portSet[typeof(GpVtgNotification)];
        }

        #endregion

        #region Post Methods

        /// <summary>
        /// Post(DsspDefaultLookup)
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public void Post(DsspDefaultLookup item) { base.PostUnknownType(item); }
        /// <summary>
        /// Post(DsspDefaultDrop)
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public void Post(DsspDefaultDrop item) { base.PostUnknownType(item); }
        /// <summary>
        /// Post(Get)
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public void Post(Get item) { base.PostUnknownType(item); }
        /// <summary>
        /// Post(Configure)
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public void Post(Configure item) { base.PostUnknownType(item); }
        /// <summary>
        /// Post(FindGpsConfig)
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public void Post(FindGpsConfig item) { base.PostUnknownType(item); }
        /// <summary>
        /// Post(Subscribe)
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public void Post(Subscribe item) { base.PostUnknownType(item); }
        /// <summary>
        /// Post(SendMicrosoftGpsCommand)
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public void Post(SendMicrosoftGpsCommand item) { base.PostUnknownType(item); }
        /// <summary>
        /// Post(HttpGet)
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public void Post(HttpGet item) { base.PostUnknownType(item); }
        /// <summary>
        /// Post(HttpPost)
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public void Post(HttpPost item) { base.PostUnknownType(item); }
        /// <summary>
        /// Post(GpGgaNotification)
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public void Post(GpGgaNotification item) { base.PostUnknownType(item); }
        /// <summary>
        /// Post(GpGllNotification)
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public void Post(GpGllNotification item) { base.PostUnknownType(item); }
        /// <summary>
        /// Post(GpGsaNotification)
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public void Post(GpGsaNotification item) { base.PostUnknownType(item); }
        /// <summary>
        /// Post(GpGsvNotification)
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public void Post(GpGsvNotification item) { base.PostUnknownType(item); }
        /// <summary>
        /// Post(GpRmcNotification)
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public void Post(GpRmcNotification item) { base.PostUnknownType(item); }
        /// <summary>
        /// Post(GpVtgNotification)
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public void Post(GpVtgNotification item) { base.PostUnknownType(item); }

        #endregion

        #region Operation Helpers

        /// <summary>
        /// Required Lookup request body type
        /// </summary>
        public virtual PortSet<LookupResponse, Fault> DsspDefaultLookup()
        {
            LookupRequestType body = new LookupRequestType();
            DsspDefaultLookup op = new DsspDefaultLookup(body);
            this.Post(op);
            return op.ResponsePort;

        }
        /// <summary>
        /// Post Dssp Default Lookup and return the response port.
        /// </summary>
        public virtual PortSet<LookupResponse, Fault> DsspDefaultLookup(LookupRequestType body)
        {
            DsspDefaultLookup op = new DsspDefaultLookup();
            op.Body = body ?? new LookupRequestType();
            this.Post(op);
            return op.ResponsePort;

        }
        /// <summary>
        /// A request to drop the service.
        /// </summary>
        public virtual PortSet<DefaultDropResponseType, Fault> DsspDefaultDrop()
        {
            DropRequestType body = new DropRequestType();
            DsspDefaultDrop op = new DsspDefaultDrop(body);
            this.Post(op);
            return op.ResponsePort;

        }
        /// <summary>
        /// Post Dssp Default Drop and return the response port.
        /// </summary>
        public virtual PortSet<DefaultDropResponseType, Fault> DsspDefaultDrop(DropRequestType body)
        {
            DsspDefaultDrop op = new DsspDefaultDrop();
            op.Body = body ?? new DropRequestType();
            this.Post(op);
            return op.ResponsePort;

        }
        /// <summary>
        /// Required Get body type
        /// </summary>
        public virtual PortSet<GpsState, Fault> Get()
        {
            GetRequestType body = new GetRequestType();
            Get op = new Get(body);
            this.Post(op);
            return op.ResponsePort;

        }
        /// <summary>
        /// Post Get and return the response port.
        /// </summary>
        public virtual PortSet<GpsState, Fault> Get(GetRequestType body)
        {
            Get op = new Get();
            op.Body = body ?? new GetRequestType();
            this.Post(op);
            return op.ResponsePort;

        }
        /// <summary>
        /// MicrosoftGps Serial Port Configuration
        /// </summary>
        public virtual PortSet<DefaultUpdateResponseType, Fault> Configure()
        {
            MicrosoftGpsConfig body = new MicrosoftGpsConfig();
            Configure op = new Configure(body);
            this.Post(op);
            return op.ResponsePort;

        }
        /// <summary>
        /// Post Configure and return the response port.
        /// </summary>
        public virtual PortSet<DefaultUpdateResponseType, Fault> Configure(MicrosoftGpsConfig body)
        {
            Configure op = new Configure();
            op.Body = body ?? new MicrosoftGpsConfig();
            this.Post(op);
            return op.ResponsePort;

        }
        /// <summary>
        /// MicrosoftGps Serial Port Configuration
        /// </summary>
        public virtual PortSet<MicrosoftGpsConfig, Fault> FindGpsConfig()
        {
            MicrosoftGpsConfig body = new MicrosoftGpsConfig();
            FindGpsConfig op = new FindGpsConfig(body);
            this.Post(op);
            return op.ResponsePort;

        }
        /// <summary>
        /// Post Find Gps Config and return the response port.
        /// </summary>
        public virtual PortSet<MicrosoftGpsConfig, Fault> FindGpsConfig(MicrosoftGpsConfig body)
        {
            FindGpsConfig op = new FindGpsConfig();
            op.Body = body ?? new MicrosoftGpsConfig();
            this.Post(op);
            return op.ResponsePort;

        }
        /// <summary>
        /// Post Subscribe and return the response port.
        /// </summary>
        public virtual PortSet<SubscribeResponseType, Fault> Subscribe(IPort notificationPort)
        {
            Subscribe op = new Subscribe();
            op.Body = new SubscribeRequest();
            op.NotificationPort = notificationPort;
            this.Post(op);
            return op.ResponsePort;

        }
        /// <summary>
        /// Post Subscribe and return the response port.
        /// </summary>
        public virtual PortSet<SubscribeResponseType, Fault> Subscribe(SubscribeRequest body, IPort notificationPort)
        {
            Subscribe op = new Subscribe();
            op.Body = body ?? new SubscribeRequest();
            op.NotificationPort = notificationPort;
            this.Post(op);
            return op.ResponsePort;

        }
        /// <summary>
        /// A Microsoft Gps Command
        /// <remarks>Use with SendMicrosoftGpsCommand()</remarks></summary>
        public virtual PortSet<DefaultUpdateResponseType, Fault> SendMicrosoftGpsCommand()
        {
            MicrosoftGpsCommand body = new MicrosoftGpsCommand();
            SendMicrosoftGpsCommand op = new SendMicrosoftGpsCommand(body);
            this.Post(op);
            return op.ResponsePort;

        }
        /// <summary>
        /// Post Send Microsoft Gps Command and return the response port.
        /// </summary>
        public virtual PortSet<DefaultUpdateResponseType, Fault> SendMicrosoftGpsCommand(MicrosoftGpsCommand body)
        {
            SendMicrosoftGpsCommand op = new SendMicrosoftGpsCommand();
            op.Body = body ?? new MicrosoftGpsCommand();
            this.Post(op);
            return op.ResponsePort;

        }
        /// <summary>
        /// DsspHttp Get request body
        /// </summary>
        public virtual PortSet<HttpResponseType, Fault> HttpGet()
        {
            HttpGetRequestType body = new HttpGetRequestType();
            HttpGet op = new HttpGet(body);
            this.Post(op);
            return op.ResponsePort;

        }
        /// <summary>
        /// Post Http Get and return the response port.
        /// </summary>
        public virtual PortSet<HttpResponseType, Fault> HttpGet(HttpGetRequestType body)
        {
            HttpGet op = new HttpGet();
            op.Body = body ?? new HttpGetRequestType();
            this.Post(op);
            return op.ResponsePort;

        }
        /// <summary>
        /// DsspHttp Post request body
        /// </summary>
        public virtual PortSet<HttpResponseType, Fault> HttpPost()
        {
            HttpPostRequestType body = new HttpPostRequestType();
            HttpPost op = new HttpPost(body);
            this.Post(op);
            return op.ResponsePort;

        }
        /// <summary>
        /// Post Http Post and return the response port.
        /// </summary>
        public virtual PortSet<HttpResponseType, Fault> HttpPost(HttpPostRequestType body)
        {
            HttpPost op = new HttpPost();
            op.Body = body ?? new HttpPostRequestType();
            this.Post(op);
            return op.ResponsePort;

        }
        /// <summary>
        /// Time, position and fix type data
        /// </summary>
        public virtual DsspResponsePort<DefaultUpdateResponseType> GpGgaNotification()
        {
            GpGga body = new GpGga();
            GpGgaNotification op = new GpGgaNotification(body);
            this.Post(op);
            return op.ResponsePort;

        }
        /// <summary>
        /// Post Gp Gga Notification and return the response port.
        /// </summary>
        public virtual DsspResponsePort<DefaultUpdateResponseType> GpGgaNotification(GpGga body)
        {
            GpGgaNotification op = new GpGgaNotification();
            op.Body = body ?? new GpGga();
            this.Post(op);
            return op.ResponsePort;

        }
        /// <summary>
        /// Latitude, longitude, UTC time of position fix and status
        /// </summary>
        public virtual DsspResponsePort<DefaultUpdateResponseType> GpGllNotification()
        {
            GpGll body = new GpGll();
            GpGllNotification op = new GpGllNotification(body);
            this.Post(op);
            return op.ResponsePort;

        }
        /// <summary>
        /// Post Gp Gll Notification and return the response port.
        /// </summary>
        public virtual DsspResponsePort<DefaultUpdateResponseType> GpGllNotification(GpGll body)
        {
            GpGllNotification op = new GpGllNotification();
            op.Body = body ?? new GpGll();
            this.Post(op);
            return op.ResponsePort;

        }
        /// <summary>
        /// GPS receiver operating mode, satellites used in the position solution, and DOP values
        /// </summary>
        public virtual DsspResponsePort<DefaultUpdateResponseType> GpGsaNotification()
        {
            GpGsa body = new GpGsa();
            GpGsaNotification op = new GpGsaNotification(body);
            this.Post(op);
            return op.ResponsePort;

        }
        /// <summary>
        /// Post Gp Gsa Notification and return the response port.
        /// </summary>
        public virtual DsspResponsePort<DefaultUpdateResponseType> GpGsaNotification(GpGsa body)
        {
            GpGsaNotification op = new GpGsaNotification();
            op.Body = body ?? new GpGsa();
            this.Post(op);
            return op.ResponsePort;

        }
        /// <summary>
        /// The number of GPS satellites in view satellite ID numbers, elevation, azimuth, and SNR values.
        /// </summary>
        public virtual DsspResponsePort<DefaultUpdateResponseType> GpGsvNotification()
        {
            GpGsv body = new GpGsv();
            GpGsvNotification op = new GpGsvNotification(body);
            this.Post(op);
            return op.ResponsePort;

        }
        /// <summary>
        /// Post Gp Gsv Notification and return the response port.
        /// </summary>
        public virtual DsspResponsePort<DefaultUpdateResponseType> GpGsvNotification(GpGsv body)
        {
            GpGsvNotification op = new GpGsvNotification();
            op.Body = body ?? new GpGsv();
            this.Post(op);
            return op.ResponsePort;

        }
        /// <summary>
        /// Time, date, position, course and speed data
        /// </summary>
        public virtual DsspResponsePort<DefaultUpdateResponseType> GpRmcNotification()
        {
            GpRmc body = new GpRmc();
            GpRmcNotification op = new GpRmcNotification(body);
            this.Post(op);
            return op.ResponsePort;

        }
        /// <summary>
        /// Post Gp Rmc Notification and return the response port.
        /// </summary>
        public virtual DsspResponsePort<DefaultUpdateResponseType> GpRmcNotification(GpRmc body)
        {
            GpRmcNotification op = new GpRmcNotification();
            op.Body = body ?? new GpRmc();
            this.Post(op);
            return op.ResponsePort;

        }
        /// <summary>
        /// Course and speed information relative to the ground
        /// </summary>
        public virtual DsspResponsePort<DefaultUpdateResponseType> GpVtgNotification()
        {
            GpVtg body = new GpVtg();
            GpVtgNotification op = new GpVtgNotification(body);
            this.Post(op);
            return op.ResponsePort;

        }
        /// <summary>
        /// Post Gp Vtg Notification and return the response port.
        /// </summary>
        public virtual DsspResponsePort<DefaultUpdateResponseType> GpVtgNotification(GpVtg body)
        {
            GpVtgNotification op = new GpVtgNotification();
            op.Body = body ?? new GpVtg();
            this.Post(op);
            return op.ResponsePort;

        }

        #endregion
    }

    /// <summary>
    /// Gets the current state of the GPS device.
    /// </summary>
#if DEBUG
    [SuppressMessage("Microsoft.Naming", "CA1716", Justification="Get is a Dss reserved word.")]
#endif
    [DisplayName("(User) Get")]
    [Description("Gets the current state of the GPS device.")]
    public class Get : Get<GetRequestType, PortSet<GpsState, Fault>>
    {
        /// <summary>
        /// Gets the current state of the GPS device.
        /// </summary>
        public Get()
        {
        }

        /// <summary>
        /// Gets the current state of the GPS device.
        /// </summary>
        /// <param name="body"></param>
        public Get(Microsoft.Dss.ServiceModel.Dssp.GetRequestType body)
            : base(body)
        {
        }

        /// <summary>
        /// Gets the current state of the GPS device.
        /// </summary>
        /// <param name="body"></param>
        /// <param name="responsePort"></param>
        public Get(Microsoft.Dss.ServiceModel.Dssp.GetRequestType body, Microsoft.Ccr.Core.PortSet<GpsState, W3C.Soap.Fault> responsePort)
            : base(body, responsePort)
        {
        }
    }

    /// <summary>
    /// Configure Microsoft Gps Operation
    /// </summary>
    [DisplayName("(User) Configure")]
    [Description("Configures a GPS device.")]
    public class Configure : Update<MicrosoftGpsConfig, PortSet<DefaultUpdateResponseType, Fault>>
    {
        /// <summary>
        /// Default Constructor
        /// </summary>
        public Configure() { }
        /// <summary>
        /// Initialization Constructor
        /// </summary>
        /// <param name="config"></param>
        public Configure(MicrosoftGpsConfig config)
        {
            this.Body = config;
        }
    }

    /// <summary>
    /// Microsoft Gps Subscribe Operation
    /// </summary>
    [Description("Subscribe to GPS sensor data.")]
    public class Subscribe : Subscribe<SubscribeRequest, PortSet<SubscribeResponseType, Fault>, MicrosoftGpsOperations> { }

    /// <summary>
    /// Finds and configures an installed Microsoft GPS device.
    /// </summary>
    [DisplayName("(User) FindGPSDevice")]
    [Description("Finds and configures an installed Microsoft GPS device.")]
    public class FindGpsConfig : Query<MicrosoftGpsConfig, PortSet<MicrosoftGpsConfig, Fault>>
    {
        /// <summary>
        /// Finds and configures an installed Microsoft GPS device.
        /// </summary>
        public FindGpsConfig()
        {
        }

        /// <summary>
        /// Finds and configures an installed Microsoft GPS device.
        /// </summary>
        /// <param name="body"></param>
        public FindGpsConfig(MicrosoftGpsConfig body)
            : base(body)
        {
        }

        /// <summary>
        /// Finds and configures an installed Microsoft GPS device.
        /// </summary>
        /// <param name="body"></param>
        /// <param name="responsePort"></param>
        public FindGpsConfig(MicrosoftGpsConfig body, Microsoft.Ccr.Core.PortSet<MicrosoftGpsConfig, W3C.Soap.Fault> responsePort)
            : base(body, responsePort)
        {
        }
    }


    /// <summary>
    /// Sends a command to the GPS device.
    /// </summary>
    [DisplayName("(User) SendGPSCommand")]
    [Description("Sends a command to the GPS device.")]
    public class SendMicrosoftGpsCommand : Update<MicrosoftGpsCommand, PortSet<DefaultUpdateResponseType, Fault>>
    {
        /// <summary>
        /// Sends a command to the GPS device.
        /// </summary>
        public SendMicrosoftGpsCommand()
        {
        }

        /// <summary>
        /// Sends a command to the GPS device.
        /// </summary>
        /// <param name="body"></param>
        public SendMicrosoftGpsCommand(MicrosoftGpsCommand body)
            :
                base(body)
        {
        }

        /// <summary>
        /// Sends a command to the GPS device.
        /// </summary>
        /// <param name="body"></param>
        /// <param name="responsePort"></param>
        public SendMicrosoftGpsCommand(MicrosoftGpsCommand body, Microsoft.Ccr.Core.PortSet<Microsoft.Dss.ServiceModel.Dssp.DefaultUpdateResponseType, W3C.Soap.Fault> responsePort)
            :
                base(body, responsePort)
        {
        }
    }

    /// <summary>
    /// GPGGA Notification
    /// </summary>
    [DisplayName("(User) UpdateGGAData")]
    [Description("Indicates an update of the GGA: Altitude and backup position.")]
    public class GpGgaNotification : Update<GpGga, DsspResponsePort<DefaultUpdateResponseType>>
    {
        /// <summary>
        /// Default Constructor
        /// </summary>
        /// <param name="data"></param>
        public GpGgaNotification(GpGga data) { this.Body = data; }

        /// <summary>
        /// Empty constructor
        /// </summary>
        public GpGgaNotification() {}
    }


    /// <summary>
    /// GPGLL Notification
    /// </summary>
    [DisplayName("(User) UpdateGLLData")]
    [Description("Indicates an update of the GLL: Primary Position.")]
    public class GpGllNotification : Update<GpGll, DsspResponsePort<DefaultUpdateResponseType>>
    {
        /// <summary>
        /// Default Constructor
        /// </summary>
        /// <param name="data"></param>
        public GpGllNotification(GpGll data) { this.Body = data; }
        /// <summary>
        /// Empty constructor
        /// </summary>
        public GpGllNotification() { }
    }

     /// <summary>
    /// GPGSA Notification
    /// </summary>
    [DisplayName("(User) UpdateGPGSAData")]
    [Description("Indicates an update of the GSA: Precision data.")]
    public class GpGsaNotification : Update<GpGsa, DsspResponsePort<DefaultUpdateResponseType>>
    {
        /// <summary>
        /// Default Constructor
        /// </summary>
        /// <param name="data"></param>
        public GpGsaNotification(GpGsa data) { this.Body = data; }
        /// <summary>
        /// Empty constructor
        /// </summary>
        public GpGsaNotification() { }
    }

    /// <summary>
    /// GPGSV Notification
    /// </summary>
    [DisplayName("(User) UpdateGPGSVData")]
    [Description("Indicates an update of the GSV: Satellites data.")]
    public class GpGsvNotification : Update<GpGsv, DsspResponsePort<DefaultUpdateResponseType>>
    {
        /// <summary>
        /// Default Constructor
        /// </summary>
        /// <param name="data"></param>
        public GpGsvNotification(GpGsv data) { this.Body = data; }

        /// <summary>
        /// Empty constructor
        /// </summary>
        public GpGsvNotification() { }
    }

    /// <summary>
    /// GPRMC Notification
    /// </summary>
    [DisplayName("(User) UpdateGPRMCData")]
    [Description("Indicates an update of the RMC: Backup course, speed, and position.")]
    public class GpRmcNotification : Update<GpRmc, DsspResponsePort<DefaultUpdateResponseType>>
    {
        /// <summary>
        /// Default Constructor
        /// </summary>
        /// <param name="data"></param>
        public GpRmcNotification(GpRmc data) { this.Body = data; }

        /// <summary>
        /// Empty constructor
        /// </summary>
        public GpRmcNotification() { }
    }

    /// <summary>
    /// GPVTG Notification
    /// </summary>
    [DisplayName("(User) UpdateGPVTGData")]
    [Description("Indicates an update of the VTG: Ground Speed and Course.")]
    public class GpVtgNotification : Update<GpVtg, DsspResponsePort<DefaultUpdateResponseType>>
    {
        /// <summary>
        /// Default Constructor
        /// </summary>
        /// <param name="data"></param>
        public GpVtgNotification(GpVtg data) { this.Body = data; }

        /// <summary>
        /// Empty constructor
        /// </summary>
        public GpVtgNotification() { }
    }

    #endregion

    #region Microsoft Gps Contract
    /// <summary>
    /// MicrosoftGps Contract
    /// </summary>
    public static class Contract
    {
        /// <summary>
        /// MicrosoftGps Contract
        /// </summary>
        public const string Identifier = "http://schemas.microsoft.com/robotics/2007/03/microsoftgps.user.html";
    }
    #endregion

}
