using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;

using W3C.Soap;

using Microsoft.Dss.Core.Attributes;
using Microsoft.Dss.ServiceModel.Dssp;


namespace TrackRoamer.Robotics.Services.AnimatedHeadService
{
    #region Animated Head Configuration

    /// <summary>
    /// AnimatedHead Serial Port Configuration
    /// </summary>
    [DataContract]
    [DisplayName("(User) AnimatedHead Configuration")]
    [Description("AnimatedHead Serial Port Configuration")]
    public class AnimatedHeadConfig : ICloneable
    {
        /// <summary>
        /// The Serial Comm Port
        /// </summary>
        [DataMember]
        [Description("Animated Head COM Port")]
        public int CommPort { get; set; }

        /// <summary>
        /// The Serial Port Name
        /// </summary>
        [DataMember]
        [Description("The Serial Port Name (Default blank)")]
        public string PortName { get; set; }

        /// <summary>
        /// Baud Rate
        /// </summary>
        [DataMember]
        [Description("Animated Head Baud Rate (57600)")]
        public int BaudRate { get; set; }

        /// <summary>
        /// Configuration Status
        /// </summary>
        [DataMember]
        [Browsable(false)]
        [Description("Animated Head Configuration Status")]
        public string ConfigurationStatus { get; set; }

        /// <summary>
        /// Default Constructor
        /// </summary>
        public AnimatedHeadConfig()
        {
            this.BaudRate = 57600;
            this.CommPort = 0;
            this.PortName = string.Empty;
        }

        #region ICloneable implementation

        public object Clone()
        {
            return new AnimatedHeadConfig()
            {
                BaudRate = this.BaudRate,
                CommPort = this.CommPort,
                PortName = this.PortName
            };
        }

        #endregion  // ICloneable implementation
    }

    /// <summary>
    /// An Animated Head Command
    /// <remarks>Use with SendAnimatedHeadCommand()</remarks>
    /// </summary>
    [DataContract]
    [Description("An Animated Head Command")]
    public class AnimatedHeadCommand
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
        public AnimatedHeadMessageType MessageTypes;

        /// <summary>
        /// Only subscribe to messages when IsValid is true
        /// </summary>
        [DataMember]
        public bool ValidOnly;
    }

    #endregion  // Animated Head Configuration

    public enum AnimatedHeadMessageType
    {
        /// <summary>
        /// No-message mask
        /// </summary>
        None = 0,

    }

    public class SerialPacket
    {
    }
}
