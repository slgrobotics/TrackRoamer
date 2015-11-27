using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Threading;

using LibSystem;
using OSC.NET;
using LibOscilloscope;
using LibGui;

namespace RoboteqControllerTest
{
    public partial class ControllerTestForm : Form
    {
        public Oscilloscope oscilloscope = null;

        public ControllerTestForm()
        {
            InitializeComponent();

            oscPortTextBox.Text = "9123";
            oscStopButton.Enabled = false;

            DateTime t1 = DateTime.Now;
            SettingsPersister.ReadOptions();
            TimeSpan ts = DateTime.Now - t1;
            Tracer.WriteLine("ReadOptions() took " + ts.Milliseconds + " ms");

            Tracer.setInterface(statusBar, statusBar2, null); // mainProgressBar);
            Tracer.WriteLine("GUI up");

            periodicMaintenanceTimer = new System.Windows.Forms.Timer();
            periodicMaintenanceTimer.Interval = 500;
            periodicMaintenanceTimer.Tick += new EventHandler(periodicMaintenance);
            periodicMaintenanceTimer.Start();

            Tracer.WriteLine("OK: Maintenance ON");

            // put Osc_DLL.dll in C:\Windows

			string oscilloscopeIniFile = Project.startupPath + "..\\..\\LibOscilloscope\\Scope_Desk.ini";

			oscilloscope = Oscilloscope.Create(oscilloscopeIniFile, null);

            if (oscilloscope != null)
            {
                Tracer.Trace("loaded oscilloscope DLL");
                oscilloscope.Show();        // appears in separate window
            }
            else
            {
				Tracer.Error("Couldn't load oscilloscope DLL");
				Tracer.Error("Make sure " + oscilloscopeIniFile + " is in place");
				Tracer.Error("Make sure C:\\WINDOWS\\Osc_DLL.dll is in place and registered with regsvr32");
			}
        }

        private System.Windows.Forms.Timer periodicMaintenanceTimer = null;
        private double beam0 = 1.0d;
        private double beam1 = 2.0d;
        private double beam2 = 3.0d;

        public void periodicMaintenance(object obj, System.EventArgs args)
        {
            try
			{
                //Tracer.WriteLine("...maintenance ticker...");

                // oscilloscope.AddData(beam0, beam1, beam2);

                Tracer.AllSync();
                Application.DoEvents();
			} 
			catch {}

			periodicMaintenanceTimer.Enabled = true;
        }

        private void grabButton_Click(object sender, EventArgs e)
        {
            Tracer.Trace("obtaining control of RoboteQ Controller");
            Tracer.Trace2("grab");
        }

        private void rcModeButton_Click(object sender, EventArgs e)
        {
            Tracer.Trace("switching RoboteQ Controller to R/C mode");
            Tracer.Trace2("R/C mode");
        }

        private void statusBar_Click(object sender, EventArgs e)
        {
            Tracer.showLog();
        }

        private void portSettingsButton_Click(object sender, EventArgs e)
        {
            DlgPortSettings dlgPortSettings = new DlgPortSettings();
            dlgPortSettings.ShowDialog(this);
        }


        private static Thread m_oscListenerThread = null;

        private bool oscStopNow = false;

		private Hashtable wiiValuesControls = new Hashtable();		// of WiimoteValuesUserControl
		private WiimoteValuesUserControl wiimoteValuesUserControl;

        private void oscListenButton_Click(object sender, EventArgs e)
        {
            oscStopButton.Enabled = true;
            oscListenButton.Enabled = false;

			foreach (string addr in wiiValuesControls.Keys)
			{
				oscGroupBox.Controls.Remove((WiimoteValuesUserControl)wiiValuesControls[addr]);
			}
			wiiValuesControls.Clear();

            m_oscListenerThread = new Thread(new ThreadStart(oscListenerLoop));
            // see Entry.cs for how the current culture is set:
            m_oscListenerThread.CurrentCulture = Thread.CurrentThread.CurrentCulture; //new CultureInfo("en-US", false);
            m_oscListenerThread.CurrentUICulture = Thread.CurrentThread.CurrentUICulture; //new CultureInfo("en-US", false);
            m_oscListenerThread.IsBackground = true;	// terminate with the main process
            m_oscListenerThread.Name = "Greeting";
            m_oscListenerThread.Start();
        }

        private void oscListenerLoop()
        {
            OSCReceiver oscReceiver = null;
            OSCMessage oscWiimoteData = null;

            oscStopNow = false;

            double[] beams = new double[200];

            beams[0] = 0.0d;
            beams[1] = 0.0d;
            beams[2] = 0.0d;

            double accel = 0.0d;
            double accelBase = 0.0d;
            double speed = 0.0d;
            double dist = 0.0d;
            double dT = 0.0d;
            double par4 = 0.0d;

            long currTick;
            long prevTick;

            currTick = prevTick = DateTime.Now.Ticks;

            try
            {
                int oscPort = Convert.ToInt32(oscPortTextBox.Text);

                oscReceiver = new OSCReceiver(oscPort);

                oscReceiver.Connect();

                Tracer.Trace("OSC connected and listening on port " + oscPort);

                while (!oscStopNow && (oscWiimoteData = (OSCMessage)oscReceiver.Receive()) != null)
                {
					string addr = oscWiimoteData.Address;

					if (wiiValuesControls.ContainsKey(addr))
					{
						wiimoteValuesUserControl = (WiimoteValuesUserControl)wiiValuesControls[addr];
					}
					else
					{
						this.Invoke(new MethodInvoker(createWiimoteValuesUserControl));
						wiiValuesControls.Add(addr, wiimoteValuesUserControl);
					}

					wiimoteValuesUserControl.values = oscWiimoteData;


					switch (addr)
					{
						case "/wiimote-g":

							currTick = DateTime.Now.Ticks;

							dT = (double)(currTick - prevTick) / 10000000.0d;       // sec

							//   string str = DateTime.Now.ToLongTimeString() + "  OSC packet: " + oscWiimoteData.Address + "   ";

							int i = 0;

							foreach (object obj in oscWiimoteData.Values)
							{
								//                        str += obj.ToString() + "   ";
								if (i == 0)
								{
									try
									{
										beams[i] = Convert.ToDouble((float)obj);
									}
									catch
									{
										beams[i] = 0.0d;
									}
									accel = beams[i];  //  m/c2
								}
								if (i == 4)
								{
									try
									{
										beams[i] = Convert.ToDouble((float)obj);
									}
									catch
									{
										beams[i] = 0.0d;
									}
									par4 = beams[i];
								}
								i++;
							}


							if (par4 > 0.0d)
							{
								accelBase = accel;
								speed = 0.0d;
								dist = 0.0d;
							}
							else
							{
								accel -= accelBase;
								speed += accel * dT;
								dist += speed * dT;
							}

							// oscilloscope.AddData(beams[0], beams[1], beams[2]);

							oscilloscope.AddData(accel, speed * 100.0d, dist * 100.0d);


							//                    Tracer.Trace(str.Trim());

							prevTick = currTick;
							break;
					}
                }
            }
            catch (Exception exc)
            {
                Tracer.Error(exc.ToString());
            }
            finally
            {
                if (oscReceiver != null)
                {
                    oscReceiver.Close();
                }
                Tracer.Trace("OSC finished and closed");
            }
        }

		private void createWiimoteValuesUserControl()
		{
			// must run in the form's thread
			wiimoteValuesUserControl = new WiimoteValuesUserControl();

			wiimoteValuesUserControl.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top
						| System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			wiimoteValuesUserControl.Location = new System.Drawing.Point(20, 25 * (wiiValuesControls.Count + 1));
			wiimoteValuesUserControl.Size = new System.Drawing.Size(526, 30);
			oscGroupBox.Controls.Add(wiimoteValuesUserControl);
		}

        private void oscStopButton_Click(object sender, EventArgs e)
        {
            oscStopNow = true;

            Tracer.Trace("stopping OSC loop");
            if (m_oscListenerThread != null)
            {
                Thread.Sleep(1000);		// let the dialog close before we abort the thread
                m_oscListenerThread.Abort();
                m_oscListenerThread = null;
            }
            oscListenButton.Enabled = true;
            oscStopButton.Enabled = false;
        }
    }
}