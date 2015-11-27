using System;
using System.Text;
using System.Collections.Generic;
using System.ComponentModel;
using Microsoft.Ccr.Core;
using Microsoft.Dss.Core.Attributes;
using Microsoft.Dss.Core.DsspHttp;
using Microsoft.Dss.ServiceModel.Dssp;
using Microsoft.Dss.ServiceModel.DsspServiceBase;
using W3C.Soap;

namespace TrackRoamer.Robotics.Hardware.PololuMaestroService
{
	/// <summary>
	/// Pololu Maestro Service contract class
	/// </summary>
	public sealed class Contract
	{
		/// <summary>
        /// DSS contract identifer for Pololu Maestro Service
		/// </summary>
		[DataMember]
		public const string Identifier = "http://schemas.trackroamer.com/2012/02/pololumaestroservice.html";
	}

    [DataContract]
    public class SafePosition
    {
        [DataMember]
        public byte channel { get; set; }            // Channel number - from 0 to 23

        [DataMember]
        public ushort positionUs { get; set; }       // microseconds, typically 850...2150
    }

	/// <summary>
    /// Pololu Maestro Service state
	/// </summary>
	[DataContract]
	public class PololuMaestroServiceState
	{
        [DataMember]
        public List<SafePosition> SafePositions { get; set; }
	}
	
	/// <summary>
    /// Pololu Maestro Service main operations port
	/// </summary>
	[ServicePort]
	public class PololuMaestroServiceOperations : PortSet<DsspDefaultLookup,
        DsspDefaultDrop,
        Get,
        HttpGet,
        HttpPost,
        SendPololuMaestroCommand,
        Subscribe>
	{
	}
	
	/// <summary>
    /// Pololu Maestro Service get operation
	/// </summary>
	public class Get : Get<GetRequestType, PortSet<PololuMaestroServiceState, Fault>>
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
		public Get(GetRequestType body, PortSet<PololuMaestroServiceState, Fault> responsePort)
			: base(body, responsePort)
		{
		}
	}

    /// <summary>
    /// Sends a command to the Pololu Maestro Device.
    /// </summary>
    [DisplayName("(User) Send Pololu Maestro Device Command")]
    [Description("Sends a command to the Pololu Maestro Device, sets value on servo pins.")]
    public class SendPololuMaestroCommand : Update<PololuMaestroCommand, PortSet<DefaultUpdateResponseType, Fault>>
    {
        /// <summary>
        /// Sends a command to the Pololu Maestro Device.
        /// </summary>
        public SendPololuMaestroCommand()
        {
        }

        /// <summary>
        /// Sends a command to the Pololu Maestro Device.
        /// </summary>
        /// <param name="body"></param>
        public SendPololuMaestroCommand(PololuMaestroCommand body)
            : base(body)
        {
        }

        /// <summary>
        /// Sends a command to the Pololu Maestro Device.
        /// </summary>
        /// <param name="body"></param>
        /// <param name="responsePort"></param>
        public SendPololuMaestroCommand(PololuMaestroCommand body, Microsoft.Ccr.Core.PortSet<Microsoft.Dss.ServiceModel.Dssp.DefaultUpdateResponseType, W3C.Soap.Fault> responsePort)
            : base(body, responsePort)
        {
        }
    }


    /// <summary>
    /// A Pololu Maestro channel-value pair
    /// <remarks>Use with SendPololuMaestroCommand()</remarks>
    /// </summary>
    [DataContract]
    [Description("A Pololu Maestro channel-value pair")]
    public class ChannelValuePair
    {
        [DataMember]
        public byte Channel { get; set; }       // Channel number - from 0 to 23, crosses over connected devices.

        //   Target, in units of quarter microseconds.  For typical servos,
        //   6000 is neutral and the acceptable range is 4000-8000.
        //   A good servo will take 880 to 2200 us (3520 to 8800 in quarters)
        [DataMember]
        public ushort Target { get; set; }

        public override string ToString()
        {
            return string.Format("{0}:{1}", Channel, Target);
        }
    }

    /// <summary>
    /// A Pololu Maestro Device Command
    /// <remarks>Use with SendPololuMaestroCommand)</remarks>
    /// </summary>
    [DataContract]
    [Description("A Pololu Maestro Device Command")]
    public class PololuMaestroCommand
    {
        [DataMember]
        public string Command { get; set; }     // "set"

        [DataMember]
        public List<ChannelValuePair> ChannelValues { get; set; }

        public override string ToString()
        {
            StringBuilder sbValues = new StringBuilder();
            foreach (ChannelValuePair cvp in ChannelValues)
            {
                sbValues.AppendFormat("{0} ", cvp.ToString());
            }
            return string.Format("{0} - {1}", Command, sbValues.ToString().Trim());
        }
    }

	/// <summary>
	/// Pololu Maestro Service subscribe operation
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



