using System;
using System.Collections.Generic;
using System.Text;
using System.Globalization;

using TrackRoamer.Robotics.Utility.LibSystem;

namespace TrackRoamer.Robotics.Hardware.LibRoboteqController
{
	internal abstract class RQQuery : RQInteraction
	{
		private string[]	m_valueNames;
		public string[] ValueNames { get { return m_valueNames; } }

		private int[]		m_intValues;
		public int[] intValues { get { return m_intValues; } }

		private double[]	m_doubleValues;
		public double[] doubleValues { get { return m_doubleValues; } }

		private Dictionary<String, RQMeasuredValue> m_measuredValues;

		protected RQQuery(string query, Dictionary<String, RQMeasuredValue> measuredValues)
		{
			this.toSend = "?" + query;
			m_measuredValues = measuredValues;
		}

		protected void init(string[] vn)
		{
			this.m_valueNames = vn;
			this.m_intValues = new int[m_valueNames.GetLength(0)];
			this.m_doubleValues = new double[m_valueNames.GetLength(0)];
			this.linesToExpect = m_valueNames.GetLength(0) + 1;
		}

		private void checkResponseSanity()
		{
			if (received.Count != this.linesToExpect || !toSend.Equals(received[0]))
			{
				string errMsg = "bad response to '" + toSend + "' - received '" + received[0] + "'  count=" + received.Count;
				Tracer.Error(errMsg);
				throw new Exception(errMsg);
			}
		}

		protected virtual double convertValue(string sHexVal, int index)
		{
			int iVal = Int32.Parse(sHexVal, NumberStyles.HexNumber);
			return (double)iVal;
		}

		private bool doTrace = false;

		internal override void interpretResponse(long timestamp)
		{
			whenReceivedTicks = timestamp;

			checkResponseSanity();

			long ticksElapsed = 0L;
			StringBuilder strb = null;

			if (doTrace)
			{
				ticksElapsed = timestamp - whenSentTicks;
				strb = new StringBuilder();

				for (int i = 0; i < received.Count; i++)
				{
					strb.Append(received[i]);
					strb.Append(" ");
				}

				strb.Append(" <==> ");
			}

            bool goodValue = true;

			for (int i=0; i < received.Count-1; i++)
			{
				try
				{
					string sHexVal = (string)received[i+1];
					m_intValues[i] = Int32.Parse(sHexVal, NumberStyles.HexNumber);
					m_doubleValues[i] = convertValue(sHexVal, i);
					if (doTrace)
					{
						strb.Append("   ");
						strb.Append(m_valueNames[i]);
						strb.Append("=");
						strb.Append(String.Format("{0:F}", m_doubleValues[i]).Replace(".00", ""));
					}
				}
				catch
				{
					if (doTrace)
					{
                        goodValue = false;
						strb.Append(" ***");
					}
				}
			}

            if (goodValue)
            {
                OnValueReceived();
            }

			if (doTrace)
			{
				Tracer.Trace("interpretResponse: " + received.Count + "   " + String.Format("{0:F1}", ticksElapsed / 10000.0d) + " ms  " + strb.ToString());
			}

			for (int i = 0; i < m_valueNames.GetLength(0); i++)
			{
				RQMeasuredValue measuredValue;
				bool mustAdd = false;

				if (m_measuredValues.ContainsKey(m_valueNames[i]))
				{
					measuredValue = (RQMeasuredValue)m_measuredValues[m_valueNames[i]];
				}
				else
				{
					measuredValue = new RQMeasuredValue();
					mustAdd = true;
				}

				lock (measuredValue)
				{
					measuredValue.timestamp = timestamp;
					measuredValue.valueName = m_valueNames[i];
					measuredValue.stringValue = (string)received[i + 1];
					measuredValue.intValue = m_intValues[i];
					measuredValue.doubleValue = m_doubleValues[i];
				}

				if (mustAdd)
				{
					m_measuredValues.Add(m_valueNames[i], measuredValue);
				}
			}
		}
	}

	internal class RQQueryMotorPower : RQQuery
	{
		internal RQQueryMotorPower(Dictionary<String, RQMeasuredValue> measuredValues)
			: base("V", measuredValues)
		{
			init(new string[] { "Motor_Power_Left", "Motor_Power_Right" });
		}
	}

	internal class RQQueryMotorAmps : RQQuery
	{
		internal RQQueryMotorAmps(Dictionary<String, RQMeasuredValue> measuredValues)
			: base("A", measuredValues)
		{
			init(new string[] { "Motor_Amps_Left", "Motor_Amps_Right" });
		}
	}

	internal class RQQueryAnalogInputs : RQQuery
	{
		internal RQQueryAnalogInputs(Dictionary<String, RQMeasuredValue> measuredValues)
			: base("P", measuredValues)
		{
			init(new string[] { "Analog_Input_1", "Analog_Input_2" });
		}

		protected override double convertValue(string sHexVal, int index)
		{
			return (double)RQCompressedHex.convertToInt(sHexVal);
		}
	}

	internal class RQQueryDigitalInputs : RQQuery
	{
		internal RQQueryDigitalInputs(Dictionary<String, RQMeasuredValue> measuredValues)
			: base("I", measuredValues)
		{
			init(new string[] { "Digital_Input_E", "Digital_Input_F", "Digital_Input_Emergency_Stop" });
		}
	}

	#region heatsink temperature converter

	internal class RQTemperature
	{
		internal static double convertToCelcius(string sHexVal)
		{
			// page 63 of the doc; returns temperature in Celcius
			int AnaValue = Int32.Parse(sHexVal, NumberStyles.HexNumber);

			// Interpolation table. Analog readings at -40 to 150 oC, in 5o intervals
			int[] TempTable = new int[] {248, 246, 243, 240, 235, 230, 224, 217, 208, 199, 188, 177,
										165, 153, 140, 128, 116, 104,93, 83, 74, 65, 58, 51, 45, 40, 35, 31, 27, 24, 21,
										19, 17, 15, 13, 12, 11, 9, 8};
			int LoTemp, HiTemp, lobound, hibound;
			double temp;
			int i = 38;
			while (TempTable[i] < AnaValue && i > 0)
				i--;
			if (i < 0)
				i = 0;
			if (i == 38)
			{
				return 150.0d;
			}
			else
			{
				LoTemp = i * 5 - 40;
				HiTemp = LoTemp + 5;
				lobound = TempTable[i];
				hibound = TempTable[i + 1];
				temp = (double)LoTemp + (5.0d * ((double)(AnaValue - lobound) * 100.0d / (double)(hibound - lobound))) / 100.0d;
				return temp;
			}
		}
	}

	#endregion // heatsink temperature converter

	internal class RQQueryHeatsinkTemperature : RQQuery
	{
		internal RQQueryHeatsinkTemperature(Dictionary<String, RQMeasuredValue> measuredValues)
			: base("M", measuredValues)
		{
			init(new string[] { "Heatsink_Temperature_Left", "Heatsink_Temperature_Right" });
		}

		protected override double convertValue(string sHexVal, int index)
		{
			return RQTemperature.convertToCelcius(sHexVal);
		}
	}

	#region voltage converter

	internal class RQVoltage
	{
		internal static double convertToMainVoltage(string sHexVal)
		{
			// page 62 of the doc; returns voltage
			int iVal = Int32.Parse(sHexVal, NumberStyles.HexNumber);
			return 55.0d * (double)iVal / 256.0d;
		}

		internal static double convertToInternalVoltage(string sHexVal)
		{
			// page 62 of the doc; returns voltage
			int iVal = Int32.Parse(sHexVal, NumberStyles.HexNumber);
			return 28.5d * (double)iVal / 256.0d;
		}
	}

	#endregion // voltage converter

	internal class RQQueryVoltage : RQQuery
	{
		internal RQQueryVoltage(Dictionary<String, RQMeasuredValue> measuredValues)
			: base("E", measuredValues)
		{
			init(new string[] { "Main_Battery_Voltage", "Internal_Voltage" });
		}

		protected override double convertValue(string sHexVal, int index)
		{
			return (index == 0) ? RQVoltage.convertToMainVoltage(sHexVal) : RQVoltage.convertToInternalVoltage(sHexVal);
		}
	}

	#region counter and analog values converter

	internal class RQCompressedHex
	{
		internal static int convertToInt(string sHexVal)
		{
			// page 150 of the doc; returns int counter

			// When reading the counter value into a microcomputer, the reverse operation must take
			// place: any output that is less than 8 digit long must be completed with a string of 0�s if the
			// first digit is of value 0 to 7, or with a string of F�s if the first digit is of value 8 to F.
			// The resulting Hex representation of a signed 32-bit number must then be converted to
			// binary or decimal as required by the application.

			// analog inputs' values: -128 = 0V   0 = 2.5V    127 = 5V

			if (sHexVal.Length < 8 && "89ABCDEF".IndexOf(sHexVal.Substring(0, 1)) != -1)
			{
				sHexVal = "FFFFFFFFF".Substring(0, 8 - sHexVal.Length) + sHexVal;
				return Int32.Parse(sHexVal, NumberStyles.HexNumber);
			}
			return Int32.Parse(sHexVal, NumberStyles.HexNumber);
		}
	}

	#endregion // counter converter

	internal class RQQueryEncoderLeftAbsolute : RQQuery
	{
		internal RQQueryEncoderLeftAbsolute(Dictionary<String, RQMeasuredValue> measuredValues)
			: base("Q0", measuredValues)
		{
			init(new string[] { "Encoder_Absolute_Left" });
		}

		protected override double convertValue(string sHexVal, int index)
		{
			return (double)RQCompressedHex.convertToInt(sHexVal);
		}
	}
	
	internal class RQQueryEncoderRightAbsolute : RQQuery
	{
		internal RQQueryEncoderRightAbsolute(Dictionary<String, RQMeasuredValue> measuredValues)
			: base("Q1", measuredValues)
		{
			init(new string[] { "Encoder_Absolute_Right" });
		}

		protected override double convertValue(string sHexVal, int index)
		{
			return (double)RQCompressedHex.convertToInt(sHexVal);
		}
	}

	internal class RQQueryEncoderSpeed : RQQuery
	{
		internal RQQueryEncoderSpeed(Dictionary<String, RQMeasuredValue> measuredValues)
			: base("Z", measuredValues)
		{
			init(new string[] { "Encoder_Speed_Left", "Encoder_Speed_Right" });
		}

		protected override double convertValue(string sHexVal, int index)
		{
			return (double)RQCompressedHex.convertToInt(sHexVal);
		}
	}
}
