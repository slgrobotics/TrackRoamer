using System;
using System.Collections.Generic;
using System.Text;

using TrackRoamer.Robotics.Utility.LibSystem;

namespace TrackRoamer.Robotics.Hardware.LibRoboteqController
{
	internal class RQMotorCommandQueue : RQInteractionQueue
	{
		internal RQMotorCommandQueue(string name) : base(name, 2)
		{
		}

        internal static long lastSentTicksMotorCmd = 0L;    // common for both motor queues

		// cannot send motor commands more frequently than 16ms (62Hz).
		internal override bool HasInteractionsQueued {
			get {
                return m_queue.Count > 0 && (DateTime.Now.Ticks - lastSentTicksMotorCmd) > 100L * TimeSpan.TicksPerMillisecond;      // give it a bit of a slack. 10 times per second is plenty.
			}
		}

		// the queue allows only the last command exist in the waiting, if a new one comes it becomes the waiting one.
		internal override void Enqueue(RQInteraction rqi)
		{
			lock (this.padlock)
			{
                if (m_queue.Count > 0)
                {
                    //Tracer.Trace(Name + " - skipped one");
                    m_queue.Clear();
                }
				m_queue.Enqueue(rqi);
			}
		}
	}
}
