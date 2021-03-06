using System;
using System.Collections.Generic;
using System.Text;

namespace TrackRoamer.Robotics.Hardware.LibRoboteqController
{
	public class RQMeasuredValue : Loggable
	{
		public long timestamp = 0;		// ticks
		public string valueName;
		public string stringValue;
		public int intValue;
		public double doubleValue;

		public override string ToString()
		{
			return valueName + "=" + doubleValue;
		}

		// interface Loggable:
		public string toLogValueString()
		{
			return String.Format("{0}", doubleValue);
			//return String.Format("{0:F4}", doubleValue);
		}
	}
}
