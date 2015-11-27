using System;
using System.Collections.Generic;
using System.Text;

using TrackRoamer.Robotics.Utility.LibSystem;

namespace TrackRoamer.Robotics.Hardware.LibRoboteqController
{
	internal class RQInteractionQueue
	{
        internal string Name { get; private set; }

        internal int PriorityFactor { get; set; }

        internal double PriorityCurrent { get { return PriorityFactor * (DateTime.Now.Ticks - waitingSinceTicks); } } 
        //internal double PriorityCurrent { get { return PriorityFactor * (DateTime.Now.Ticks - waitingSinceTicks) / TimeSpan.TicksPerSecond + PriorityFactor * 0.3d; } }      // one PriorityFactor counts as a 0.3 second delay

		protected Queue<RQInteraction> m_queue = new Queue<RQInteraction>();	// of RQInteraction

        internal void Clear()
        {
            lock (padlock)
            {
                if (!isProcessingInteraction)
                {
                    m_queue.Clear();
                }
            }
        }

        internal virtual bool HasInteractionsQueued { get { return m_queue.Count > 0; } }   // see RQMotorCommandQueue for override

		internal object padlock = "";
		internal long waitingSinceTicks = 0L;

		private RQInteraction m_currentInteraction = null;
		internal bool isProcessingInteraction { get { return m_currentInteraction != null; } }

		internal bool onStringReceived(string str, long timestamp)	        // may throw an exception
		{
			bool ret = false;

			lock (this.padlock)
			{
				if (m_currentInteraction != null)
				{
					m_currentInteraction.received.Add(str);
					if (m_currentInteraction.received.Count >= m_currentInteraction.linesToExpect)
					{
						// last line of the response
						m_currentInteraction.interpretResponse(timestamp);
						m_currentInteraction = null;
						ret = true;
					}
				}
			}
			return ret;
		}

		internal bool checkForTimeout()
		{
			bool ret = false;

			if (isProcessingInteraction && (DateTime.Now.Ticks - m_currentInteraction.whenSentTicks) / TimeSpan.TicksPerMillisecond > m_currentInteraction.timeoutMs)
			{
				Tracer.Error("RoboteQ timeout on " + m_currentInteraction.toSend);
				m_currentInteraction = null;
				ret = true;
			}

			return ret;
		}

		internal RQInteractionQueue(string _name, int priorityFactor)
		{
			Name = _name;
            PriorityFactor = priorityFactor;
            waitingSinceTicks = DateTime.Now.Ticks;
		}

		internal string dequeueForSend()
		{
			m_currentInteraction = m_queue.Dequeue();
			m_currentInteraction.whenSentTicks = DateTime.Now.Ticks;
			return m_currentInteraction.toSend;
		}

		//internal RQInteraction Dequeue()
		//{
		//    return m_queue.Dequeue();
		//}

		internal virtual void Enqueue(RQInteraction rqi)
		{
			lock (this.padlock)
			{
                if (m_queue.Count == 0)
                {
                    waitingSinceTicks = DateTime.Now.Ticks;
                }
				m_queue.Enqueue(rqi);
			}
		}
	}
}
