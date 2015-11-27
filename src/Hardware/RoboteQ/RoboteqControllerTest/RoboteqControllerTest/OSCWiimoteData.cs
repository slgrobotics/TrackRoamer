using System;
using System.Collections.Generic;
using System.Text;

using LibSystem;
using OSC.NET;

namespace RoboteqControllerTest
{
    class OSCWiimoteData : OSCMessage
    {
        public OSCWiimoteData(string address)
            : base(address)
		{
        }

        public override string ToString()
        {
            return "here I am";
        }
    }
}
