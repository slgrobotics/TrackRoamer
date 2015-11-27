using System;
using System.Collections.Generic;
using System.Text;

using LibSystem;

namespace LibRoboteqController
{
	internal class RQCommand : RQInteraction
	{
		internal RQInteractionQueue queue = null;
		private int m_resendCount = 0;

		protected RQCommand(string cmd)
		{
			this.toSend = "!" + cmd;
			linesToExpect = 2;			// the "+" or "-" confirmation comes in a separate line
		}

		private bool isResponseSane
		{
			get { return received.Count == this.linesToExpect && toSend.Equals(received[0]) && "+".Equals(received[1]); }
		}

		private bool doTrace = false;

		internal override void interpretResponse(long timestamp)
		{
			if (!isResponseSane)
			{
				string errMsg = "bad response to '" + toSend + "' - received '" + received[0] + "'  count=" + received.Count;
				Tracer.Error(errMsg);
				if (queue != null && m_resendCount++ < 3)
				{
					if (queue.HasInteractionsQueued)
					{
						Tracer.Trace("--- fresher command in queue, not resending: " + toSend);
					}
					else
					{
						Tracer.Trace("--- resending: " + toSend);
						queue.Enqueue(this);
					}
				}
				return;
			}

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

				Tracer.Trace("interpretResponse: " + received.Count + "   " + String.Format("{0:F1}", ticksElapsed / 10000.0d) + " ms  " + strb.ToString());
			}
		}
	}

	internal class RWMotorPowerConvert
	{
		internal static string toCommand(bool isLeftMotor, int powerOrSpeed)
		{
			return isLeftMotor ?
				  String.Format("{0}{1:X02}", powerOrSpeed >= 0 ? "A" : "a", Math.Abs(powerOrSpeed))
				: String.Format("{0}{1:X02}", powerOrSpeed >= 0 ? "B" : "b", Math.Abs(powerOrSpeed));
		}
	}

	internal class RQCommandMotorPowerLeft : RQCommand
	{
		internal RQCommandMotorPowerLeft(int powerOrSpeed)
			: base(RWMotorPowerConvert.toCommand(true, powerOrSpeed))
		{
		}
	}

	internal class RQCommandMotorPowerRight : RQCommand
	{
		internal RQCommandMotorPowerRight(int powerOrSpeed)
			: base(RWMotorPowerConvert.toCommand(false, powerOrSpeed))
		{
		}
	}

}
