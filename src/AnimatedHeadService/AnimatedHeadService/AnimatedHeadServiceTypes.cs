using System;
using System.Collections.Generic;
using System.ComponentModel;
using Microsoft.Ccr.Core;
using Microsoft.Dss.Core.Attributes;
using Microsoft.Dss.Core.DsspHttp;
using Microsoft.Dss.ServiceModel.Dssp;
using Microsoft.Dss.ServiceModel.DsspServiceBase;
using W3C.Soap;
using Trackroamer.Library.LibAnimatronics;

namespace TrackRoamer.Robotics.Services.AnimatedHeadService
{
    /// <summary>
    /// AnimatedHeadService contract class
    /// </summary>
    public sealed class Contract
    {
        /// <summary>
        /// DSS contract identifer for AnimatedHeadService
        /// </summary>
        [DataMember]
        public const string Identifier = "http://schemas.trackroamer.com/robotics/2013/12/animatedheadservice.html";
    }

    /// <summary>
    /// AnimatedHeadService state
    /// </summary>
    [DataContract]
    [Description("The Animated Head state")]
    public class AnimatedHeadServiceState
    {
        /// <summary>
        /// Is the Animated Head currently connected.
        /// </summary>
        [DataMember]
        [Description("Indicates that the Animated Head is connected.")]
        [Browsable(false)]
        public bool Connected { get; set; }

        /// <summary>
        /// Serial Port Configuration
        /// </summary>
        [DataMember(IsRequired = true)]
        [Description("Specifies the serial port where the Animated Head is connected.")]
        public AnimatedHeadConfig AnimatedHeadServiceConfig { get; set; }
    }

    /// <summary>
    /// AnimatedHeadService main operations port
    /// </summary>
    [ServicePort]
    public class AnimatedHeadServiceOperations : PortSet<DsspDefaultLookup,
        DsspDefaultDrop,
        Get,
        HttpGet,
        HttpPost,
        SendArduinoDeviceCommand,
        Subscribe>
    {
    }

    /// <summary>
    /// AnimatedHeadService get operation
    /// </summary>
    public class Get : Get<GetRequestType, PortSet<AnimatedHeadServiceState, Fault>>
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
        public Get(GetRequestType body, PortSet<AnimatedHeadServiceState, Fault> responsePort)
            : base(body, responsePort)
        {
        }
    }

    /// <summary>
    /// Sends a command to the Arduino Device.
    /// </summary>
    [DisplayName("(User) Send Arduino Device Command")]
    [Description("Sends a command to the Arduino Device, invoking corresoonding animations etc..")]
    public class SendArduinoDeviceCommand : Update<ArduinoDeviceCommand, PortSet<DefaultUpdateResponseType, Fault>>
    {
        /// <summary>
        /// Sends a command to the Arduino Device.
        /// </summary>
        public SendArduinoDeviceCommand()
        {
        }

        /// <summary>
        /// Sends a command to the Arduino Device.
        /// </summary>
        /// <param name="body"></param>
        public SendArduinoDeviceCommand(ArduinoDeviceCommand body)
            : base(body)
        {
        }

        /// <summary>
        /// Sends a command to the Arduino Device.
        /// </summary>
        /// <param name="body"></param>
        /// <param name="responsePort"></param>
        public SendArduinoDeviceCommand(ArduinoDeviceCommand body, Microsoft.Ccr.Core.PortSet<Microsoft.Dss.ServiceModel.Dssp.DefaultUpdateResponseType, W3C.Soap.Fault> responsePort)
            : base(body, responsePort)
        {
        }
    }

    public enum AnimatedHeadCommands
    {
        //SET_VALUE,
        ANIMATIONS_CLEAR,
        ANIMATIONS_DEFAULT,
        //SET_FRAMES,

        SetAnim,
        AddAnim,
        SetAnimCombo
    }

    /// <summary>
    /// A Arduino Device Command
    /// <remarks>Use with SendArduinoDeviceCommand)</remarks>
    /// </summary>
    [DataContract]
    [Description("A Arduino Device Command")]
    public class ArduinoDeviceCommand
    {
        [DataMember]
        public AnimatedHeadCommands Command { get; set; }     // "set"

        [DataMember]
        public string Args { get; set; }

        [DataMember]
        public double Scale { get; set; }

        [DataMember]
        public bool? doRepeat { get; set; }

        public override string ToString()
        {
            return string.Format("Command: {0}  Args: {1}  Scale: {2}  doRepeat: {3}", Command, Args, Scale, doRepeat);
        }
    }

    /// <summary>
    /// AnimatedHeadService subscribe operation
    /// </summary>
    public class Subscribe : Subscribe<SubscribeRequestType, PortSet<SubscribeResponseType, Fault>>
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
        public Subscribe(SubscribeRequestType body)
            : base(body)
        {
        }

        /// <summary>
        /// Creates a new instance of Subscribe
        /// </summary>
        /// <param name="body">the request message body</param>
        /// <param name="responsePort">the response port for the request</param>
        public Subscribe(SubscribeRequestType body, PortSet<SubscribeResponseType, Fault> responsePort)
            : base(body, responsePort)
        {
        }
    }
}


