using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

using LibSystem;

namespace LibRoboteqController
{
	internal class RQMotorCommandQueue : RQInteractionQueue
	{
		internal RQMotorCommandQueue(string name) : base(name)
		{
		}

		// cannot send motor commands more frequently than 16ms.
		internal override bool HasInteractionsQueued {
			get {
				return m_queue.Count > 0 && (DateTime.Now.Ticks - lastSentTicks) > 1600000L; 
			}
		}

		// the queue allows only the last command exist in the waiting, if a new one comes it becomes the waiting one.
		internal override void Enqueue(object obj)
		{
			lock (this.padlock)
			{
				m_queue.Clear();
				m_queue.Enqueue(obj);
			}
		}
	}
}
