using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

using LibSystem;

namespace LibRoboteqController
{
	internal class RQInteractionQueue
	{
		private string m_name;
		internal string Name { get { return m_name; } }

		protected Queue m_queue = new Queue();		// of RQInteraction
		internal virtual bool HasInteractionsQueued { get { return m_queue.Count > 0; } }

		internal object padlock = "";
		internal long lastSentTicks = 0L;

		private RQInteraction m_currentInteraction = null;
		internal bool isProcessingInteraction { get { return m_currentInteraction != null; } }

		internal bool onStringReceived(string str, long timestamp)	// may throw an exception
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

			if (isProcessingInteraction && (DateTime.Now.Ticks - m_currentInteraction.whenSentTicks) / 10000 > m_currentInteraction.timeoutMs)
			{
				Tracer.Error("timeout on " + m_currentInteraction.toSend);
				m_currentInteraction = null;
				ret = true;
			}

			return ret;
		}

		internal RQInteractionQueue(string name)
		{
			m_name = name;
		}

		internal string dequeueForSend()
		{
			m_currentInteraction = (RQInteraction)m_queue.Dequeue();
			m_currentInteraction.whenSentTicks = DateTime.Now.Ticks;
			return m_currentInteraction.toSend;
		}

		//internal object Dequeue()
		//{
		//    return queue.Dequeue();
		//}

		internal virtual void Enqueue(object obj)
		{
			lock (this.padlock)
			{
				m_queue.Enqueue(obj);
			}
		}
	}
}
