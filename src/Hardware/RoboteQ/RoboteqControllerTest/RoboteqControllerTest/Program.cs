using System;
using System.Collections.Generic;
using System.Windows.Forms;

using LibSystem;

namespace RoboteqControllerTest
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Project project;

            try
            {
                project = new LibSystem.Project();
                new LibSystem.Tracer();	// writes first line in trace file
                LibSystem.Tracer.TraceVersions();
            }
            catch { }

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new ControllerTestForm());
        }
    }
}