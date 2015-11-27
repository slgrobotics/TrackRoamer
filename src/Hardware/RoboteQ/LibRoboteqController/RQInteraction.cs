using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace LibRoboteqController
{
	/// <summary>
	/// controller interaction = command, query - involves sent line and a response
	/// </summary>
	internal abstract class RQInteraction
	{
		internal string toSend;
		internal int linesToExpect = 1;					// including echo
		internal ArrayList received = new ArrayList();	// of string
		internal long whenSentTicks;
		internal int timeoutMs = 200;

		internal virtual void reset()
		{
			received.Clear();
		}

		internal abstract void interpretResponse(long timestamp);		// may throw exception
	}
}
