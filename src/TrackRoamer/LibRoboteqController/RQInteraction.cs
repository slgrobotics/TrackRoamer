using System;
using System.Collections.Generic;
using System.Text;

namespace TrackRoamer.Robotics.Hardware.LibRoboteqController
{
	public class MeasuredValuesEventArgs : System.EventArgs
	{
		//public string name;
		public long timestamp;	// ticks
		public double? value1;
		public double? value2;
		public double? value3;

		public MeasuredValuesEventArgs(long ts)
		{
			timestamp = ts;
		}
	}

	public delegate void OnValueReceived(object sender, MeasuredValuesEventArgs e);

	/// <summary>
	/// controller interaction = command, query - involves sent line and a response
	/// </summary>
	internal abstract class RQInteraction
	{
		internal event OnValueReceived onValueReceived;
		internal string toSend;
		internal int linesToExpect = 1;							// including echo
		internal List<String> received = new List<String>();
		internal long whenSentTicks;
		internal long whenReceivedTicks;
		internal int timeoutMs = 1000;

		internal virtual void reset()
		{
			received.Clear();
		}

		internal abstract void interpretResponse(long timestamp);		// may throw exception

		internal virtual void OnValueReceived()
		{
			if (onValueReceived != null)
			{
				onValueReceived(this, new MeasuredValuesEventArgs(whenReceivedTicks));
			}
		}
	}
}
