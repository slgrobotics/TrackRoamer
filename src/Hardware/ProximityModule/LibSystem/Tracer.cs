using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using diag = System.Diagnostics;

namespace TrackRoamer.Robotics.Utility.LibSystem
{
    /// <summary>
    /// simple way to trace.
    /// your best bet is using DebugView ( http://technet.microsoft.com/en-us/sysinternals/bb896647.aspx ) to monitor console output.
    /// </summary>
    public class Tracer
    {
        static Tracer()
        {
            diag.Trace.WriteLine("Started " + DateTime.Now);
            diag.Trace.WriteLine("Framework: " + Environment.Version);
        }

        // thread safe
        public static void Trace(string str)
        {
            diag.Trace.WriteLine(str);
        }

        // thread safe
        public static void Error(string str)
        {
            diag.Trace.WriteLine("Error: " + str);
        }

        // thread safe
        public static void Error(Exception ex)
        {
            diag.Trace.WriteLine("Error: " + ex.Message + "\r\n" + ex.StackTrace);
        }
    }
}
