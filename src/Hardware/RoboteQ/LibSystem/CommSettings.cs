using System;
using System.Collections.Generic;
using System.Text;

using System.IO;
using System.Xml;
using System.Xml.Serialization;

namespace LibSystem
{
	/// <summary>
	/// Parity settings
	/// </summary>
	public enum Parity
	{
		/// <summary>
		/// Characters do not have a parity bit.
		/// </summary>
		none = 0,
		/// <summary>
		/// If there are an odd number of 1s in the data bits, the parity bit is 1.
		/// </summary>
		odd = 1,
		/// <summary>
		/// If there are an even number of 1s in the data bits, the parity bit is 1.
		/// </summary>
		even = 2,
		/// <summary>
		/// The parity bit is always 1.
		/// </summary>
		mark = 3,
		/// <summary>
		/// The parity bit is always 0.
		/// </summary>
		space = 4
	};

	/// <summary>
	/// Stop bit settings
	/// </summary>
	public enum StopBits
	{
		/// <summary>
		/// Line is asserted for 1 bit duration at end of each character
		/// </summary>
		one = 0,
		/// <summary>
		/// Line is asserted for 1.5 bit duration at end of each character
		/// </summary>
		onePointFive = 1,
		/// <summary>
		/// Line is asserted for 2 bit duration at end of each character
		/// </summary>
		two = 2
	};

	/// <summary>
	/// Uses for RTS or DTR pins
	/// </summary>
	public enum HSOutput
	{
		/// <summary>
		/// Pin is asserted when this station is able to receive data.
		/// </summary>
		handshake = 2,
		/// <summary>
		/// Pin is asserted when this station is transmitting data (RTS on NT, 2000 or XP only).
		/// </summary>
		gate = 3,
		/// <summary>
		/// Pin is asserted when this station is online (port is open).
		/// </summary>
		online = 1,
		/// <summary>
		/// Pin is never asserted.
		/// </summary>
		none = 0
	};

	/// <summary>
	/// Standard handshake methods
	/// </summary>
	public enum Handshake
	{
		/// <summary>
		/// No handshaking
		/// </summary>
		none,
		/// <summary>
		/// Software handshaking using Xon / Xoff
		/// </summary>
		XonXoff,
		/// <summary>
		/// Hardware handshaking using CTS / RTS
		/// </summary>
		CtsRts,
		/// <summary>
		/// Hardware handshaking using DSR / DTR
		/// </summary>
		DsrDtr
	}

	/// <summary>
	/// Byte type with enumeration constants for ASCII control codes.
	/// </summary>
	public enum ASCII : byte
	{
		NULL = 0x00, SOH = 0x01, STH = 0x02, ETX = 0x03, EOT = 0x04, ENQ = 0x05, ACK = 0x06, BELL = 0x07,
		BS = 0x08, HT = 0x09, LF = 0x0A, VT = 0x0B, FF = 0x0C, CR = 0x0D, SO = 0x0E, SI = 0x0F, DC1 = 0x11,
		DC2 = 0x12, DC3 = 0x13, DC4 = 0x14, NAK = 0x15, SYN = 0x16, ETB = 0x17, CAN = 0x18, EM = 0x19,
		SUB = 0x1A, ESC = 0x1B, FS = 0x1C, GS = 0x1D, RS = 0x1E, US = 0x1F, SP = 0x20, DEL = 0x7F
	}

	/// <summary>
	/// Set the public fields to supply settings to CommBase.
	/// </summary>
	public class CommBaseSettings
	{
		/// <summary>
		/// Port Name (default: "COM1:")
		/// </summary>
		public string port = "COM1:";
		/// <summary>
		/// Baud Rate (default: 2400) unsupported rates will throw "Bad settings"
		/// </summary>
		public int baudRate = 2400;
		/// <summary>
		/// The parity checking scheme (default: none)
		/// </summary>
		public Parity parity = Parity.none;
		/// <summary>
		/// Number of databits 1..8 (default: 8) unsupported values will throw "Bad settings"
		/// </summary>
		public int dataBits = 8;
		/// <summary>
		/// Number of stop bits (default: one)
		/// </summary>
		public StopBits stopBits = StopBits.one;
		/// <summary>
		/// If true, transmission is halted unless CTS is asserted by the remote station (default: false)
		/// </summary>
		public bool txFlowCTS = false;
		/// <summary>
		/// If true, transmission is halted unless DSR is asserted by the remote station (default: false)
		/// </summary>
		public bool txFlowDSR = false;
		/// <summary>
		/// If true, transmission is halted when Xoff is received and restarted when Xon is received (default: false)
		/// </summary>
		public bool txFlowX = false;
		/// <summary>
		/// If false, transmission is suspended when this station has sent Xoff to the remote station (default: true)
		/// Set false if the remote station treats any character as an Xon.
		/// </summary>
		public bool txWhenRxXoff = true;
		/// <summary>
		/// If true, received characters are ignored unless DSR is asserted by the remote station (default: false)
		/// </summary>
		public bool rxGateDSR = false;
		/// <summary>
		/// If true, Xon and Xoff characters are sent to control the data flow from the remote station (default: false)
		/// </summary>
		public bool rxFlowX = false;
		/// <summary>
		/// Specifies the use to which the RTS output is put (default: none)
		/// </summary>
		public HSOutput useRTS = HSOutput.none;
		/// <summary>
		/// Specidies the use to which the DTR output is put (default: none)
		/// </summary>
		public HSOutput useDTR = HSOutput.none;
		/// <summary>
		/// The character used to signal Xon for X flow control (default: DC1)
		/// </summary>
		public ASCII XonChar = ASCII.DC1;
		/// <summary>
		/// The character used to signal Xoff for X flow control (default: DC3)
		/// </summary>
		public ASCII XoffChar = ASCII.DC3;
		//JH 1.2: Next two defaults changed to 0 to use new defaulting mechanism dependant on queue size.
		/// <summary>
		/// The number of free bytes in the reception queue at which flow is disabled
		/// (Default: 0 = Set to 1/10th of actual rxQueue size)
		/// </summary>
		public int rxHighWater = 0;
		/// <summary>
		/// The number of bytes in the reception queue at which flow is re-enabled
		/// (Default: 0 = Set to 1/10th of actual rxQueue size)
		/// </summary>
		public int rxLowWater = 0;
		/// <summary>
		/// Multiplier. Max time for Send in ms = (Multiplier * Characters) + Constant
		/// (default: 0 = No timeout)
		/// </summary>
		public uint sendTimeoutMultiplier = 0;
		/// <summary>
		/// Constant.  Max time for Send in ms = (Multiplier * Characters) + Constant (default: 0)
		/// </summary>
		public uint sendTimeoutConstant = 0;
		/// <summary>
		/// Requested size for receive queue (default: 0 = use operating system default)
		/// </summary>
		public int rxQueue = 0;
		/// <summary>
		/// Requested size for transmit queue (default: 0 = use operating system default)
		/// </summary>
		public int txQueue = 0;
		/// <summary>
		/// If true, the port will automatically re-open on next send if it was previously closed due
		/// to an error (default: false)
		/// </summary>
		public bool autoReopen = false;

		/// <summary>
		/// If true, subsequent Send commands wait for completion of earlier ones enabling the results
		/// to be checked. If false, errors, including timeouts, may not be detected, but performance
		/// may be better.
		/// </summary>
		public bool checkAllSends = true;

		/// <summary>
		/// Pre-configures settings for most modern devices: 8 databits, 1 stop bit, no parity and
		/// one of the common handshake protocols. Change individual settings later if necessary.
		/// </summary>
		/// <param name="Port">The port to use (i.e. "COM1:")</param>
		/// <param name="Baud">The baud rate</param>
		/// <param name="Hs">The handshake protocol</param>
		public void SetStandard(string Port, int Baud, Handshake Hs)
		{
			dataBits = 8; stopBits = StopBits.one; parity = Parity.none;
			port = Port; baudRate = Baud;
			switch (Hs)
			{
				case Handshake.none:
					txFlowCTS = false; txFlowDSR = false; txFlowX = false;
					rxFlowX = false; useRTS = HSOutput.online; useDTR = HSOutput.online;
					txWhenRxXoff = true; rxGateDSR = false;
					break;
				case Handshake.XonXoff:
					txFlowCTS = false; txFlowDSR = false; txFlowX = true;
					rxFlowX = true; useRTS = HSOutput.online; useDTR = HSOutput.online;
					txWhenRxXoff = true; rxGateDSR = false;
					XonChar = ASCII.DC1; XoffChar = ASCII.DC3;
					break;
				case Handshake.CtsRts:
					txFlowCTS = true; txFlowDSR = false; txFlowX = false;
					rxFlowX = false; useRTS = HSOutput.handshake; useDTR = HSOutput.online;
					txWhenRxXoff = true; rxGateDSR = false;
					break;
				case Handshake.DsrDtr:
					txFlowCTS = false; txFlowDSR = true; txFlowX = false;
					rxFlowX = false; useRTS = HSOutput.online; useDTR = HSOutput.handshake;
					txWhenRxXoff = true; rxGateDSR = false;
					break;
			}
		}

		/// <summary>
		/// Save the object in XML format to a stream
		/// </summary>
		/// <param name="s">Stream to save the object to</param>
		public void SaveAsXML(Stream s)
		{
			XmlSerializer sr = new XmlSerializer(this.GetType());
			sr.Serialize(s, this);
		}

		/// <summary>
		/// Create a new CommBaseSettings object initialised from XML data
		/// </summary>
		/// <param name="s">Stream to load the XML from</param>
		/// <returns>CommBaseSettings object</returns>
		public static CommBaseSettings LoadFromXML(Stream s)
		{
			return LoadFromXML(s, typeof(CommBaseSettings));
		}

		/// <summary>
		/// Create a new object loading members from the stream in XML format.
		/// Derived class should call this from a static method i.e.:
		/// return (ComDerivedSettings)LoadFromXML(s, typeof(ComDerivedSettings));
		/// </summary>
		/// <param name="s">Stream to load the object from</param>
		/// <param name="t">Type of the derived object</param>
		/// <returns></returns>
		protected static CommBaseSettings LoadFromXML(Stream s, Type t)
		{
			XmlSerializer sr = new XmlSerializer(t);
			try
			{
				return (CommBaseSettings)sr.Deserialize(s);
			}
			catch
			{
				return null;
			}
		}
	}


	/// <summary>
	/// Extends CommBaseSettings to add the settings used by CommLine.
	/// </summary>
	public class CommLineSettings : CommBaseSettings
	{
		/// <summary>
		/// Maximum size of received string (default: 256)
		/// </summary>
		public int rxStringBufferSize = 256;
		/// <summary>
		/// ASCII code that terminates a received string (default: CR)
		/// </summary>
		public ASCII rxTerminator = ASCII.CR;
		/// <summary>
		/// ASCII codes that will be ignored in received string (default: null)
		/// </summary>
		public ASCII[] rxFilter;
		/// <summary>
		/// Maximum time (ms) for the Transact method to complete (default: 500)
		/// </summary>
		public int transactTimeout = 500;
		/// <summary>
		/// ASCII codes transmitted after each Send string (default: null)
		/// </summary>
		public ASCII[] txTerminator;

		public static new CommLineSettings LoadFromXML(Stream s)
		{
			return (CommLineSettings)LoadFromXML(s, typeof(CommLineSettings));
		}
	}

}