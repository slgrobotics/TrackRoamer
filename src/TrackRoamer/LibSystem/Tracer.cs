using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Text;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using System.Threading;
using System.IO;
using System.Reflection;
using System.Reflection.Emit;


namespace TrackRoamer.Robotics.Utility.LibSystem
{
	public class Tracer
	{
        private static string m_traceFileName;
        public static bool doFileTrace = true;

        public static string ApplicationStartupPath { get; private set; }

		static Tracer()
		{
            ApplicationStartupPath = Application.StartupPath;

            //m_traceFileName = Path.Combine(m_applicationStartupPath, string.Format("Trackroamer_trace_{0:yyyyMMdd_HHmmss}.txt", DateTime.Now));
            m_traceFileName = Path.Combine(Project.LogPath, string.Format("Trackroamer_trace_{0:yyyyMMdd_HHmmss}.txt", DateTime.Now));

            if (doFileTrace)
            {
                using (FileStream fs = new FileStream(m_traceFileName, FileMode.Create, FileAccess.Write, FileShare.ReadWrite))
                {
                    TextWriter tw = new StreamWriter(fs);
                    tw.WriteLine("Started " + DateTime.Now);
                    tw.WriteLine("Framework: " + Environment.Version + " Program: " + Project.PROGRAM_NAME_HUMAN + " " + Project.PROGRAM_VERSION_HUMAN + " Build: " + Project.PROGRAM_VERSION_RELEASEDATE);
                    TraceVersions(tw);
                    tw.Close();
                }
            }

            Console.WriteLine("Started " + DateTime.Now);
            Console.WriteLine("Framework: " + Environment.Version + " Program: " + Project.PROGRAM_NAME_HUMAN + " " + Project.PROGRAM_VERSION_HUMAN + " Build: " + Project.PROGRAM_VERSION_RELEASEDATE);
        }

        public static void TraceVersions()
        {
            using (FileStream fs = new FileStream(m_traceFileName, FileMode.Append, FileAccess.Write, FileShare.ReadWrite))
            {
                TextWriter tw = new StreamWriter(fs);

                TraceVersions(tw);

                tw.Close();
            }
        }

        public static void TraceVersions(TextWriter tw)
        {
            // Get all the assemblies currently loaded in the application domain.
            Assembly[] myAssemblies = Thread.GetDomain().GetAssemblies();
            for (int i = 0; i < myAssemblies.Length; i++)
            {
                string str = "    " + myAssemblies[i].GetName().Version + "  " + myAssemblies[i].GetName().ToString();
                tw.WriteLine(str);
            }
        }

		// thread safe
		public static void Trace(string str)
		{
            // we want to be quick here, no timestamps unless we write to file
            if (doFileTrace)
            {
                string msg = TimeStamp + str;

                lock (m_traceFileName)
                {
                    File.AppendAllText(m_traceFileName, msg + "\r\n");
                }

                NonBlockingConsole.WriteLine(msg);
            }
            else
            {
                NonBlockingConsole.WriteLine(str);
            }
		}

        // thread safe
        public static void Trace2(string str)
        {
            string msg = TimeStamp + str;

            if (doFileTrace)
            {
                lock (m_traceFileName)
                {
                    File.AppendAllText(m_traceFileName, msg + "\r\n");
                }
            }

            NonBlockingConsole.WriteLine(msg);
        }

		// thread safe
		public static void Error(string str)
		{
            string msg = TimeStamp + "Error: " + str;

            if (doFileTrace)
            {
                lock (m_traceFileName)
                {
                    File.AppendAllText(m_traceFileName, msg + "\r\n");
                }
            }

            NonBlockingConsole.WriteLine(msg);
		}

		// thread safe
        public static void Error(Exception ex)
        {
            string msg = TimeStamp + "Error: " + ex.Message + "\r\n" + ex.StackTrace;

            if (doFileTrace)
            {
                lock (m_traceFileName)
                {
                    File.AppendAllText(m_traceFileName, msg + "\r\n");
                }
            }

            NonBlockingConsole.WriteLine(msg);
        }

        static string TimeStamp { get { DateTime now = DateTime.Now; return string.Format("{0:HH mm ss}.{1:d03}  ", now, now.Millisecond); } }
	}

    // http://stackoverflow.com/questions/3670057/does-console-writeline-block

    public static class NonBlockingConsole
    {
        private static BlockingCollection<string> m_Queue = new BlockingCollection<string>();

        static NonBlockingConsole()
        {
            var thread = new Thread(
              () =>
              {
                  while (true) Console.WriteLine(m_Queue.Take());
              });
            thread.IsBackground = true;
            thread.Start();
        }

        public static void WriteLine(string value)
        {
            m_Queue.Add(value);
        }
    }
}
