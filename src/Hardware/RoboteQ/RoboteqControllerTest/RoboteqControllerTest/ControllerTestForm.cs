using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Text;
using System.Windows.Forms;
using System.Threading;
using System.IO.Ports;

using LibSystem;
using OSC.NET;
using LibOscilloscope;
using LibGui;
using Lib3DDraw;
using LibRoboteqController;
using LibHumanInputDevices;
using LibPicSensors;
using LibLvrGenericHid;

namespace RoboteqControllerTest
{
	public partial class ControllerTestForm : Form
	{
		public Oscilloscope oscilloscope = null;
        public LogitechRumblepad gamepad = null;
        public ProximityModule _picpxmod = null;

		private Hashtable measuredControls = new Hashtable();

		public ControllerTestForm()
		{
			InitializeComponent();

			compassTrackBar.Value = (int)compassBearing;

			rpCloseButton.Enabled = false;

			disconnectControllerButton.Enabled = false;

			{
				string[] ports = SerialPort.GetPortNames();
				ArrayList alPorts = new ArrayList(ports);

				this.comboBoxPort.Items.AddRange(ports);
                this.comboBoxPortCsa.Items.AddRange(ports);

				// a little hack to preset ports:
				int pos = alPorts.IndexOf("COM1");
				if (pos >= 0)
				{
					comboBoxPort.SelectedIndex = pos;
				}

                pos = alPorts.IndexOf("COM4");
				if (pos >= 0)
				{
					comboBoxPortCsa.SelectedIndex = pos;
				}
			}

			oscPortTextBox.Text = "9123";
			oscStopButton.Enabled = false;
			scaOscStopButton.Enabled = false;

			DateTime t1 = DateTime.Now;
			SettingsPersister.ReadOptions();
			TimeSpan ts = DateTime.Now - t1;
			Tracer.WriteLine("ReadOptions() took " + ts.Milliseconds + " ms");

			Tracer.setInterface(statusBar, statusBar2, null); // mainProgressBar);
			Tracer.WriteLine("GUI up");

			periodicMaintenanceTimer = new System.Windows.Forms.Timer();
			periodicMaintenanceTimer.Interval = 300;
			periodicMaintenanceTimer.Tick += new EventHandler(periodicMaintenance);
			periodicMaintenanceTimer.Start();

			Tracer.WriteLine("OK: Maintenance ON");

			//ensureOscilloscope();

            initDashboard();

            //create gamepad device.
            //gamepad = new LogitechRumblepad(this, devicesTreeView);

            //gamepad.leftJoystickVertMoved += new JoystickEventHandler(gamepad_leftJoystickMoved);
            //gamepad.rightJoystickVertMoved += new JoystickEventHandler(gamepad_rightJoystickMoved);
            //gamepad.btnChangedState += new ButtonEventHandler(gamepad_btnChangedState);

			//create picpxmod device.
			_picpxmod = new ProximityModule(this);

            _picpxmod.HasReadFrame += pmFrameCompleteHandler;

            picUsbVendorIdTextBox.Text = string.Format("0x{0:X}", _picpxmod.vendorId);
            picUsbProductIdTextBox.Text = string.Format("0x{0:X}", _picpxmod.productId);
        }

		#region ensureOscilloscope()

		private void ensureOscilloscope()
		{
			//// put Osc_DLL.dll in C:\Windows

			if (oscilloscope == null)
			{
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
		}

		#endregion // ensureOscilloscope()

		#region initDashboard()

		private void initDashboard()
        {
            rqMeasuredUserControlBatteryVoltage.valueName = "Main_Battery_Voltage";
            measuredControls.Add(rqMeasuredUserControlBatteryVoltage.valueName, rqMeasuredUserControlBatteryVoltage);
            rqMeasuredUserControlBatteryVoltage.minValue = 10.0d;
            rqMeasuredUserControlBatteryVoltage.maxValue = 15.0d;

            rqMeasuredUserControlInternalVoltage.valueName = "Internal_Voltage";
            measuredControls.Add(rqMeasuredUserControlInternalVoltage.valueName, rqMeasuredUserControlInternalVoltage);
			rqMeasuredUserControlInternalVoltage.minValue = 10.0d;
            rqMeasuredUserControlInternalVoltage.maxValue = 15.0d;

            rqMeasuredUserControlSpeedLeft.valueName = "Encoder_Speed_Left";
            measuredControls.Add(rqMeasuredUserControlSpeedLeft.valueName, rqMeasuredUserControlSpeedLeft);
			rqMeasuredUserControlSpeedLeft.minValue = -120;
			rqMeasuredUserControlSpeedLeft.maxValue = 120;

            rqMeasuredUserControlSpeedRight.valueName = "Encoder_Speed_Right";
            measuredControls.Add(rqMeasuredUserControlSpeedRight.valueName, rqMeasuredUserControlSpeedRight);
			rqMeasuredUserControlSpeedRight.minValue = -120;
			rqMeasuredUserControlSpeedRight.maxValue = 120;

            rqMeasuredUserControlCounterLeft.valueName = "Encoder_Absolute_Left";
            measuredControls.Add(rqMeasuredUserControlCounterLeft.valueName, rqMeasuredUserControlCounterLeft);
			rqMeasuredUserControlCounterLeft.maxValue = Double.NaN;

            rqMeasuredUserControlCounterRight.valueName = "Encoder_Absolute_Right";
            measuredControls.Add(rqMeasuredUserControlCounterRight.valueName, rqMeasuredUserControlCounterRight);
			rqMeasuredUserControlCounterRight.maxValue = Double.NaN;

            rqMeasuredUserControlPowerLeft.valueName = "Motor_Power_Left";
            measuredControls.Add(rqMeasuredUserControlPowerLeft.valueName, rqMeasuredUserControlPowerLeft);
			rqMeasuredUserControlPowerLeft.minValue = 0.0d;
			rqMeasuredUserControlPowerLeft.maxValue = 120.0d;

            rqMeasuredUserControlAmpsLeft.valueName = "Motor_Amps_Left";
            measuredControls.Add(rqMeasuredUserControlAmpsLeft.valueName, rqMeasuredUserControlAmpsLeft);
            rqMeasuredUserControlAmpsLeft.minValue = 0.0d;
            rqMeasuredUserControlAmpsLeft.maxValue = 20.0d;

            rqMeasuredUserControlPowerRight.valueName = "Motor_Power_Right";
            measuredControls.Add(rqMeasuredUserControlPowerRight.valueName, rqMeasuredUserControlPowerRight);
			rqMeasuredUserControlPowerRight.minValue = 0.0d;
			rqMeasuredUserControlPowerRight.maxValue = 120.0d;

            rqMeasuredUserControlAmpsRight.valueName = "Motor_Amps_Right";
            measuredControls.Add(rqMeasuredUserControlAmpsRight.valueName, rqMeasuredUserControlAmpsRight);
            rqMeasuredUserControlAmpsRight.minValue = 0.0d;
			rqMeasuredUserControlAmpsRight.maxValue = 20.0d;

            rqMeasuredUserControlTemperatureLeft.valueName = "Heatsink_Temperature_Left";
            measuredControls.Add(rqMeasuredUserControlTemperatureLeft.valueName, rqMeasuredUserControlTemperatureLeft);
            rqMeasuredUserControlTemperatureLeft.minValue = 10.0d;
			rqMeasuredUserControlTemperatureLeft.maxValue = 90.0d;

            rqMeasuredUserControlTemperatureRight.valueName = "Heatsink_Temperature_Right";
            measuredControls.Add(rqMeasuredUserControlTemperatureRight.valueName, rqMeasuredUserControlTemperatureRight);
            rqMeasuredUserControlTemperatureRight.minValue = 10.0d;
            rqMeasuredUserControlTemperatureRight.maxValue = 90.0d;

            rqMeasuredUserControlAnalogInput1.valueName = "Analog_Input_1";
            measuredControls.Add(rqMeasuredUserControlAnalogInput1.valueName, rqMeasuredUserControlAnalogInput1);
			rqMeasuredUserControlAnalogInput1.minValue = -120;
			rqMeasuredUserControlAnalogInput1.maxValue = 120;

            rqMeasuredUserControlAnalogInput2.valueName = "Analog_Input_2";
            measuredControls.Add(rqMeasuredUserControlAnalogInput2.valueName, rqMeasuredUserControlAnalogInput2);
			rqMeasuredUserControlAnalogInput2.minValue = -120;
			rqMeasuredUserControlAnalogInput2.maxValue = 120;

            rqMeasuredUserControlDigitalInputE.valueName = "Digital_Input_E";
            measuredControls.Add(rqMeasuredUserControlDigitalInputE.valueName, rqMeasuredUserControlDigitalInputE);
			rqMeasuredUserControlDigitalInputE.minValue = 0;
			rqMeasuredUserControlDigitalInputE.maxValue = 1;

            rqMeasuredUserControlDigitalInputF.valueName = "Digital_Input_F";
            measuredControls.Add(rqMeasuredUserControlDigitalInputF.valueName, rqMeasuredUserControlDigitalInputF);
			rqMeasuredUserControlDigitalInputF.minValue = 0;
			rqMeasuredUserControlDigitalInputF.maxValue = 1;

            rqMeasuredUserControlDigitalInputEmergencyStop.valueName = "Digital_Input_Emergency_Stop";
            measuredControls.Add(rqMeasuredUserControlDigitalInputEmergencyStop.valueName, rqMeasuredUserControlDigitalInputEmergencyStop);
			rqMeasuredUserControlDigitalInputEmergencyStop.minValue = 0;
			rqMeasuredUserControlDigitalInputEmergencyStop.maxValue = 1;

            // R/C mode measured values:
            rqMeasuredUserControlCommand1.valueName = "Command1";
            measuredControls.Add(rqMeasuredUserControlCommand1.valueName, rqMeasuredUserControlCommand1);

            rqMeasuredUserControlCommand2.valueName = "Command2";
            measuredControls.Add(rqMeasuredUserControlCommand2.valueName, rqMeasuredUserControlCommand2);

            rqMeasuredUserControlValue1.valueName = "Value1";
            measuredControls.Add(rqMeasuredUserControlValue1.valueName, rqMeasuredUserControlValue1);

            rqMeasuredUserControlValue2.valueName = "Value2";
            measuredControls.Add(rqMeasuredUserControlValue2.valueName, rqMeasuredUserControlValue2);
        }
        #endregion // initDashboard()

        #region Periodic Maintenance related

        private System.Windows.Forms.Timer periodicMaintenanceTimer = null;

		public void periodicMaintenance(object obj, System.EventArgs args)
		{
			try
			{
				//Tracer.WriteLine("...maintenance ticker... " + DateTime.Now);

				// oscilloscope.AddData(beam0, beam1, beam2);

				Tracer.AllSync();
				Application.DoEvents();

				if (m_controller != null)
				{
					lock (m_controller)
					{
						syncMeasured();

						if (m_controller.isGrabbed)
						{
							this.motorActivationGroupBox.Enabled = true;
							stopMotorsButton.BackColor = Color.OrangeRed;
							stopMotorsButton.Text = "S T O P";
						}
						else
						{
							this.motorActivationGroupBox.Enabled = false;
							stopMotorsButton.BackColor = this.motorActivationGroupBox.BackColor;
							stopMotorsButton.Text = m_controller.isMonitored ? "MONITORED" : "";
						}
					}
				}

				disconnectControllerButton.Enabled = (m_controller != null);
				connectToControllerButton.Enabled = (m_controller == null);

			}
			catch(Exception exc)
			{
				Tracer.Error("Exception in periodicMaintenance: " + exc);
			}

			periodicMaintenanceTimer.Enabled = true;
		}

		private void syncMeasured()
		{
			foreach (string valueName in measuredControls.Keys)
			{
				RQMeasuredUserControl rqmc = (RQMeasuredUserControl)measuredControls[valueName];

				if (m_controller.measuredValues.ContainsKey(valueName))
				{
					RQMeasuredValue rqmv = (RQMeasuredValue)m_controller.measuredValues[valueName];
					rqmc.measured = rqmv;
				}
			}
        }
        #endregion // Periodic Maintenance related

        private void statusBar_Click(object sender, EventArgs e)
		{
			Tracer.showLog();
		}

		#region Wiimote/Accelerator listener related

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
			m_oscListenerThread.Name = "WiimoteTestOSCListener";
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

					int i = 0;

					switch (addr)
					{
						case "/accel-g":

							currTick = DateTime.Now.Ticks;

							dT = (double)(currTick - prevTick) / 10000000.0d;       // sec

							//   string str = DateTime.Now.ToLongTimeString() + "  OSC packet: " + oscWiimoteData.Address + "   ";

							foreach (object obj in oscWiimoteData.Values)
							{
								//                        str += obj.ToString() + "   ";
								if (i <= 2)
								{
									try
									{
										beams[i] = Convert.ToDouble((float)obj);
									}
									catch
									{
										beams[i] = 0.0d;
									}
								}
								i++;
							}

							oscilloscope.AddData(beams[0], beams[1], beams[2]);

		                    // Tracer.Trace(str.Trim());

							prevTick = currTick;
							break;

						case "/wiimote-g":

							currTick = DateTime.Now.Ticks;

							dT = (double)(currTick - prevTick) / 10000000.0d;       // sec

							//   string str = DateTime.Now.ToLongTimeString() + "  OSC packet: " + oscWiimoteData.Address + "   ";

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

		#endregion // Wiimote listener related

		private ControllerRQAX2850 m_controller;

		private void ensureController()
		{
			if (m_controller == null)
			{
				m_controller = new ControllerRQAX2850(comboBoxPort.Text);
				m_controller.init();
			}
		}

		private void disposeController()
		{
			if (m_controller != null)
			{
				lock (m_controller)
				{
					// Close the port
					m_controller.Dispose();
					m_controller = null;
				}
			}

			foreach (Control cntrl in this.panel1.Controls)
			{
				if (cntrl.GetType() == typeof(RQMeasuredUserControl))
				{
					((RQMeasuredUserControl)cntrl).Cleanup();
				}
			}

			this.motorActivationGroupBox.Enabled = false;
			stopMotorsButton.BackColor = this.motorActivationGroupBox.BackColor;
			stopMotorsButton.Text = "-";
		}

		private void connectToControllerButton_Click(object sender, EventArgs e)
		{
			// start monitoring serial line
			ensureController();
		}
		
		private void grabButton_Click(object sender, EventArgs e)
		{
			// send TCCR to switch controller to Serial Mode
			ensureController();
			m_controller.GrabController();

			stopMotorsButton_Click(null, null);
		}

		private void resetControllerButton_Click(object sender, EventArgs e)
		{
			// reset and possibly switch to R/C mode
			ensureController();
			m_controller.ResetController();
		}

		private void disconnectControllerButton_Click(object sender, EventArgs e)
		{
			// abandon controller, disconnect serial link
			disposeController();
		}

		private void leftMotorPowerTrackBar_ValueChanged(object sender, EventArgs e)
		{
			int iVal = leftMotorPowerTrackBar.Value;
			string cmdVal = String.Format("{0}{1:X02}", iVal >= 0 ? "A":"a", Math.Abs(iVal));
			this.leftMotorPowerLabel.Text = String.Format("{0} == {1}", iVal, cmdVal);

			if (m_controller != null && m_controller.isGrabbed)
			{
				m_controller.SetMotorPowerOrSpeedLeft(iVal);
			}
		}

		private void stopLeftButton_Click(object sender, EventArgs e)
		{
			leftMotorPowerTrackBar.Value = 0;
		}

		private void rightMotorPowerTrackBar_ValueChanged(object sender, EventArgs e)
		{
			int iVal = rightMotorPowerTrackBar.Value;
			string cmdVal = String.Format("{0}{1:X02}", iVal >= 0 ? "B" : "b", Math.Abs(iVal));
			this.rightMotorPowerLabel.Text = String.Format("{0} == {1}", iVal, cmdVal);

			if (m_controller != null && m_controller.isGrabbed)
			{
				m_controller.SetMotorPowerOrSpeedRight(iVal);
			}
		}

		private void stopRightButton_Click(object sender, EventArgs e)
		{
			rightMotorPowerTrackBar.Value = 0;
		}

		private void stopMotorsButton_Click(object sender, EventArgs e)
		{
			leftMotorPowerTrackBar.Value = 0;
			rightMotorPowerTrackBar.Value = 0;
		}

		private void rpOpenButton_Click(object sender, EventArgs e)
		{
            gamepad.init();
			rpCloseButton.Enabled = true;
			rpOpenButton.Enabled = false;
			rpCloseButton2.Enabled = true;
			rpOpenButton2.Enabled = false;
		}

        private void rpCloseButton_Click(object sender, EventArgs e)
        {
            gamepad.Close();
			rpOpenButton.Enabled = true;
			rpCloseButton.Enabled = false;
			rpOpenButton2.Enabled = true;
			rpCloseButton2.Enabled = false;
		}

		private void gamepad_leftJoystickMoved(Object sender, JoystickEventArgs args)
		{
			int pos = args.position;
			int power = (32767 - pos) / 256 / (int)this.rpSensitivityNumericUpDown.Value;

			//Tracer.Trace("left: " + pos);

			leftVertTrackBar.Value = power;
			leftMotorPowerTrackBar.Value = power;
		}

		private void gamepad_rightJoystickMoved(Object sender, JoystickEventArgs args)
		{
			int pos = args.position;
			int power = (32767 - pos) / 256 / (int)this.rpSensitivityNumericUpDown.Value;

			//Tracer.Trace("right: " + pos + "  power: " + power);

			rightVertTrackBar.Value = power;
			rightMotorPowerTrackBar.Value = power;
		}

		void gamepad_btnChangedState(object sender, RPButtonsEventArgs args)
		{
			Tracer.Trace(args.ToString());

			if (args.pressed)
			{
				switch (args.button)
				{
					case 2:
						this.rpSensitivityNumericUpDown.Value++;
						break;
					case 4:
						this.rpSensitivityNumericUpDown.Value--;
						break;
				}
			}
		}

		private string m_csaPortName = "COM4";
		private SerialPort m_csaPort = null;

		private void ensureCsaPort()
		{
			if (m_csaPort == null)
			{
                m_csaPortName = comboBoxPortCsa.Text;

				m_csaPort = new SerialPort(m_csaPortName, 9600, System.IO.Ports.Parity.None, 8, System.IO.Ports.StopBits.One);

				// Attach a method to be called when there is data waiting in the port's buffer
				m_csaPort.DataReceived += new SerialDataReceivedEventHandler(csaPort_DataReceived);
				m_csaPort.ErrorReceived += new SerialErrorReceivedEventHandler(csaPort_ErrorReceived);

				m_csaPort.NewLine = "\r";
				m_csaPort.DtrEnable = true;
				m_csaPort.Handshake = System.IO.Ports.Handshake.None;
				m_csaPort.Encoding = Encoding.ASCII;
				m_csaPort.RtsEnable = true;

				// Open the port for communications
				m_csaPort.Open();
				m_csaPort.DiscardInBuffer();
			}
		}

		void csaPort_ErrorReceived(object sender, SerialErrorReceivedEventArgs e)
		{
			Tracer.Error("csaPort_ErrorReceived: " + e.ToString());
		}

		private StringBuilder m_sb = new StringBuilder();
		private int m_wCnt = 0;		// watchdog "W" consecutive count to make sure we've grabbed the controller all right

		private void csaPort_DataReceived(object sender, SerialDataReceivedEventArgs e)
		{
			long timestamp = DateTime.Now.Ticks;
			while (m_csaPort.BytesToRead > 0)
			{
				byte b = (byte)m_csaPort.ReadByte();
				byte[] bb = new byte[1];
				bb[0] = b;
				Encoding enc = Encoding.ASCII;
				string RxString = enc.GetString(bb);
				if (RxString == "\r")
				{
					onStringReceived(m_sb.ToString(), timestamp);
					m_sb.Remove(0, m_sb.Length);
				}
				else
				{
					m_wCnt = 0;
					m_sb.Append(RxString);
				}
			}

			// Show all the incoming data in the port's buffer
			//Tracer.Trace(port.ReadExisting());
		}

		/// <summary>
		/// hopefully not crippled string came in
		/// </summary>
		/// <param name="str"></param>
		private void onStringReceived(string str, long timestamp)
		{
			try
			{
				interpretCsaString(str);
			}
			catch (Exception exc)
			{
				Tracer.Error(exc.ToString());
			}
		}

		private void interpretCsaString(string str)
		{
			this.csaDataLabel.Text = str;

			string[] tokens = str.Split(new char[] { ' ' });

			int i = 0;

			while (i < tokens.GetLength(0))
			{
				string token = tokens[i];

				//Tracer.Trace(token); // + " " + tokens[i + 1] + " " + tokens[i + 2] + " " + tokens[i + 3]);

				switch (token)
				{
					case "ACC":
						interpretAccelerationData(Convert.ToDouble(tokens[i + 1]), Convert.ToDouble(tokens[i + 2]), Convert.ToDouble(tokens[i + 3]));
						i += 3;
						break;

					case "HDG":
						interpretCompassData(Convert.ToDouble(tokens[i + 1]));
						i += 1;
						break;

					case "SON":
						interpretSonarData(Convert.ToInt32(tokens[i + 1]), Convert.ToDouble(tokens[i + 2]));
						i += 3;
						break;
				}
				i++;
			}
		}

		private const double GfCnv = 0.022d;	// 0.022 puts 1G = 9.8

		private void interpretAccelerationData(double accX, double accY, double accZ)
		{
			accX *= GfCnv;
			accY *= GfCnv;
			accZ *= GfCnv;

			this.accelXLabel.Text = String.Format("{0}", accX);
			this.accelYLabel.Text = String.Format("{0}", accY);
			this.accelZLabel.Text = String.Format("{0}", accZ);

			robotViewControl1.setAccelerometerData(accX, accY, accZ);

			if (oscTransmitter != null)
			{
				OSCMessage oscAccelData = new OSCMessage("/accel-g");
				oscAccelData.Append((float)(accX * 5.0d));
				oscAccelData.Append((float)(accY * 5.0d));
				oscAccelData.Append((float)(accZ * 5.0d));

				oscTransmitter.Send(oscAccelData);
			}
		}

		double lastHeading = -1;

		private void interpretCompassData(double heading)
		{
			heading /= 10.0d;
			this.compassDataLabel.Text = String.Format("{0}", heading);
			if (lastHeading != heading)
			{
				compassBearing = heading;
				this.compassViewControl1.CompassBearing = compassBearing;
				lastHeading = heading;
			}
		}

		private void interpretSonarData(int angleRaw, double distCm)
		{
			this.sonarBearingLabel.Text = String.Format("{0}", angleRaw);
			this.sonarRangeCmLabel.Text = String.Format("{0}", distCm);

			sonarViewControl1.setReading(angleRaw, 0.01d * distCm, DateTime.Now.Ticks);
		}

		private void csaOpenButton_Click(object sender, EventArgs e)
		{
			ensureCsaPort();

			csaCloseButton.Enabled = true;
			csaOpenButton.Enabled = false;

			this.csaDataLabel.Text = "...waiting for serial data on " + m_csaPortName;
		}

		private void csaCloseButton_Click(object sender, EventArgs e)
		{
			csaOpenButton.Enabled = true;
			csaCloseButton.Enabled = false;

			m_csaPort.Close();
			m_csaPort = null;

			this.csaDataLabel.Text = "port " + m_csaPortName + " closed";
		}

		double compassBearing = 45.0d;

		private void compassTrackBar_ValueChanged(object sender, EventArgs e)
		{
			compassBearing = (double)compassTrackBar.Value;
			this.compassDataLabel.Text = String.Format("{0}", compassBearing);
			this.compassViewControl1.CompassBearing = compassBearing;
		}

		private void testSonarLinkLabel_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
		{
			Random random = new Random();

			for(int angleRaw=150; angleRaw <= 1160 ;angleRaw+=50)
			{
                sonarViewControl1.setReading(angleRaw, 3.0d * random.NextDouble(), DateTime.Now.Ticks);
			}
		}

		double m_accX = 1.0d;
		double m_accY = 1.0d;
		double m_accZ = 1.0d;

		private void accelXTrackBar_ValueChanged(object sender, EventArgs e)
		{
			m_accX = accelXTrackBar.Value / 5.0d;
			this.accelXLabel.Text = String.Format("{0}", m_accX);
			robotViewControl1.setAccelerometerData(m_accX, m_accY, m_accZ);
		}

		private void accelYTrackBar_ValueChanged(object sender, EventArgs e)
		{
			m_accY = accelYTrackBar.Value / 5.0d;
			this.accelYLabel.Text = String.Format("{0}", m_accY);
			robotViewControl1.setAccelerometerData(m_accX, m_accY, m_accZ);
		}

		private void accelZTrackBar_ValueChanged(object sender, EventArgs e)
		{
			m_accZ = accelZTrackBar.Value / 5.0d;
			this.accelZLabel.Text = String.Format("{0}", m_accZ);
			robotViewControl1.setAccelerometerData(m_accX, m_accY, m_accZ);
		}

		private OSCTransmitter oscTransmitter = null;

		private void scaOscButton_Click(object sender, EventArgs e)
		{
			scaOscStopButton.Enabled = true;
			scaOscButton.Enabled = false;

			ensureOscilloscope();

			m_oscListenerThread = new Thread(new ThreadStart(oscListenerLoop));
			// see Entry.cs for how the current culture is set:
			m_oscListenerThread.CurrentCulture = Thread.CurrentThread.CurrentCulture; //new CultureInfo("en-US", false);
			m_oscListenerThread.CurrentUICulture = Thread.CurrentThread.CurrentUICulture; //new CultureInfo("en-US", false);
			m_oscListenerThread.IsBackground = true;	// terminate with the main process
			m_oscListenerThread.Name = "AccelTestOSCListener";
			m_oscListenerThread.Start();

			Thread.Sleep(3000);		// let the oscilloscope start

			int oscPort = Convert.ToInt32(oscPortTextBox.Text);

			oscTransmitter = new OSCTransmitter("localhost", oscPort);

			oscTransmitter.Connect();

			Tracer.Trace("OSC connected and transmitting to port " + oscPort);
		}

		private void scaOscStopButton_Click(object sender, EventArgs e)
		{
			oscStopNow = true;
			oscTransmitter.Close();
			oscTransmitter = null;

			Tracer.Trace("stopping OSC loop");
			if (m_oscListenerThread != null)
			{
				Thread.Sleep(1000);		// let the dialog close before we abort the thread
				m_oscListenerThread.Abort();
				m_oscListenerThread = null;
			}
			scaOscButton.Enabled = true;
			scaOscStopButton.Enabled = false;
		}

        ///  <summary>
        ///   Overrides WndProc to enable checking for and handling WM_DEVICECHANGE messages.
        ///  </summary>
        ///  
        ///  <param name="m"> a Windows Message </param>

        protected override void WndProc(ref Message m)
        {
            try
            {
                //  The OnDeviceChange routine processes WM_DEVICECHANGE messages.

                if (m.Msg == DeviceManagement.WM_DEVICECHANGE)
                {
                    _picpxmod.OnDeviceChange(m);
                }

                //  Let the base form process the message.

                base.WndProc(ref m);
            }
            catch (Exception ex)
            {
                Tracer.Error(ex);
                throw;
            }
        }         
        
        private void picUsbConnectButton_Click(object sender, EventArgs e)
        {
            _picpxmod.Open();
        }

        private void picUsbDisconnectButton_Click(object sender, EventArgs e)
        {
            _picpxmod.Close();
        }

        string locker = "";

        private void pmServo1ScrollBar_ValueChanged(object sender, EventArgs e)
        {
            lock (locker)
            {
                int scrollValue = pmServo1ScrollBar.Value;
                pmServo1NumericUpDown.Value = scrollValue;
                _picpxmod.ServoPositionSet((int)pmServoNumberNumericUpDown.Value, (double)scrollValue);
            }
        }

        private void pmServo1NumericUpDown_ValueChanged(object sender, EventArgs e)
        {
            lock (locker)
            {
                int scrollValue = (int)pmServo1NumericUpDown.Value;
                //pmServo1ScrollBar.Value = scrollValue;
                _picpxmod.ServoPositionSet((int)pmServoNumberNumericUpDown.Value, (double)scrollValue);
            }
        }

        private void picUsbDataContinuousStartButton_Click(object sender, EventArgs e)
        {
            _picpxmod.DataContinuousStart();
        }

        private void picUsbDataContinuousStopButton_Click(object sender, EventArgs e)
        {
            _picpxmod.DataContinuousStop();
        }

        int rawAngle1_Min = int.MaxValue;
        double angleDegrees1_Min = -120.0d;
        int rawAngle1_Max = int.MinValue;
        double angleDegrees1_Max = -5.0d;

        int rawAngle2_Min = int.MaxValue;
        double angleDegrees2_Min = 5.0d;
        int rawAngle2_Max = int.MinValue;
        double angleDegrees2_Max = 120.0d;

        private int numRays1;
        private int numRays2;
        int angleRawStep1 = 0;
        int angleRawStep2 = 0;
        double angleDegreesStep1 = 0.0d;
        double angleDegreesStep2 = 0.0d;

        const int TEST_SAMPLES_COUNT = 100;
        int inTestSamples = TEST_SAMPLES_COUNT;
        SortedList<int, int> rawAngle1Values = new SortedList<int, int>();
        SortedList<int, int> rawAngle2Values = new SortedList<int, int>();

        // This will be called whenever the data is read from the board:
        private void pmFrameCompleteHandler(object sender, AsyncInputFrameArgs aira)
        {
            long timestamp = DateTime.Now.Ticks;

            Tracer.Trace("Async sonar frame arrived. " + DateTime.Now);

            StringBuilder frameValue = new StringBuilder();

            frameValue.AppendFormat("{0}\r\n", aira.dPos1Mks);
            frameValue.AppendFormat("{0}\r\n", aira.dPos2Mks);
            frameValue.AppendFormat("{0}\r\n", aira.dPing1DistanceMm);
            frameValue.AppendFormat("{0}\r\n", aira.dPing2DistanceMm);

            pmValuesTextBox.Text = frameValue.ToString();

            int angleRaw1 = (int)aira.dPos1Mks;
            double distCm1 = aira.dPing1DistanceMm / 10.0d;

            int angleRaw2 = (int)aira.dPos2Mks;
            double distCm2 = aira.dPing2DistanceMm / 10.0d;


            if (inTestSamples > 0)
            {
                --inTestSamples;

                // for a while just try figuring out what comes in - sweep angle ranges for both sides

                if (inTestSamples > TEST_SAMPLES_COUNT - 10)        // first few frames are garbled
                {
                    return;
                }

                //rawAngle1_Min = int.MaxValue;
                //rawAngle1_Max = int.MinValue;
                //rawAngle2_Min = int.MaxValue;
                //rawAngle2_Max = int.MinValue;
                //inTestSamples = 200;

                rawAngle1_Min = Math.Min(rawAngle1_Min, angleRaw1);
                rawAngle1_Max = Math.Max(rawAngle1_Max, angleRaw1);

                rawAngle2_Min = Math.Min(rawAngle2_Min, angleRaw2);
                rawAngle2_Max = Math.Max(rawAngle2_Max, angleRaw2);

                if (!rawAngle1Values.ContainsKey(angleRaw1))
                {
                    rawAngle1Values.Add(angleRaw1, angleRaw1);
                }

                if (!rawAngle2Values.ContainsKey(angleRaw2))
                {
                    rawAngle2Values.Add(angleRaw2, angleRaw2);
                }

                if (inTestSamples == 0)     // last count
                {
                    numRays1 = rawAngle1Values.Count;
                    numRays2 = rawAngle2Values.Count;

                    angleRawStep1 = (int)Math.Round((double)(rawAngle1_Max - rawAngle1_Min) / (double)numRays1);
                    angleRawStep2 = (int)Math.Round((double)(rawAngle2_Max - rawAngle2_Min) / (double)numRays2);

                    angleDegreesStep1 = (angleDegrees1_Max - angleDegrees1_Min) / numRays1;
                    angleDegreesStep2 = (angleDegrees2_Max - angleDegrees2_Min) / numRays2;

                    StringBuilder sBuf = new StringBuilder();

                    sBuf.Append("setReading(): numRays1=" + numRays1 + "  angleRawStep1=" + angleRawStep1 + "  rawAngle1_Min=" + rawAngle1_Min + "  rawAngle1_Max=" + rawAngle1_Max + "  angleDegreesStep1=" + angleDegreesStep1 + "\r\nsweep angles1 (us) : ");

                    for (int count = 0; count < rawAngle1Values.Count; count++)
                    {
                        //  Display bytes as 2-character Hex strings.
                        sBuf.AppendFormat("{0} ", rawAngle1Values.ElementAt(count).Key);
                    }

                    sBuf.Append("\r\nsetReading(): numRays2=" + numRays2 + "  angleRawStep2=" + angleRawStep2 + "  rawAngle2_Min=" + rawAngle2_Min + "  rawAngle2_Max=" + rawAngle2_Max + "  angleDegreesStep2=" + angleDegreesStep2 + "\r\nsweep angles2 (us) : ");

                    for (int count = 0; count < rawAngle2Values.Count; count++)
                    {
                        //  Display bytes as 2-character Hex strings.
                        sBuf.AppendFormat("{0} ", rawAngle2Values.ElementAt(count).Key);
                    }

                    Tracer.Trace(sBuf.ToString());
                }
            }

            this.pmBearingLabel1.Text = String.Format("{0} us", angleRaw1);
            this.pmRangeLabel1.Text = String.Format("{0} cm", distCm1);

            pmSonarViewControl1.setReading(angleRaw1, 0.01d * distCm1, timestamp);

            this.pmNraysLabel1.Text = String.Format("{0} rays", pmSonarViewControl1.NumNumbers);


            this.pmBearingLabel2.Text = String.Format("{0} us", angleRaw2);
            this.pmRangeLabel2.Text = String.Format("{0} cm", distCm2);

            pmSonarViewControl2.setReading(angleRaw2, 0.01d * distCm2, timestamp);

            this.pmNraysLabel2.Text = String.Format("{0} rays", pmSonarViewControl2.NumNumbers);

            if (inTestSamples == 0)
            {
                this.pmBearingLabelComb.Text = String.Format("{0} us", angleRaw2);
                this.pmRangeLabelComb.Text = String.Format("{0} cm", distCm2);

                pmSonarViewControlComb.setReading(angleRawToSonarAngle(1, angleRaw1), 0.01d * distCm1, angleRawToSonarAngle(2, angleRaw2), 0.01d * distCm2, timestamp);

                this.pmNraysLabelComb.Text = String.Format("{0} rays", pmSonarViewControlComb.NumNumbers);

                if (angleRaw1 == rawAngle1_Min || angleRaw1 == rawAngle1_Max)
                {
                    Tracer.Trace("Sweep Frame Ready");

                    SonarData p = pmSonarViewControlComb.sonarData;

                    foreach (int angle in p.angles.Keys)
                    {
                        RangeReading rr = p.getLatestReadingAt(angle);
                        double range = rr.rangeMeters * 1000.0d;        // millimeters
                        //ranges.Add(range);
                        Tracer.Trace("&&&&&&&&&&&&&&&&&&&&&&&&&&&& angle=" + angle + " range=" + range);
                        /*
                         * typical measurement:
                         * for dual radar:
                            angles: 26
                            Sweep Frame Ready
                            angle=883 range=2052
                            angle=982 range=2047
                            angle=1081 range=394
                            angle=1179 range=394
                            angle=1278 range=398
                            angle=1377 range=390
                            angle=1475 range=390
                            angle=1574 range=390
                            angle=1673 range=399
                            angle=1771 range=416
                            angle=1870 range=1972
                            angle=1969 range=2182
                            angle=2067 range=3802
                            angle=2166 range=245
                            angle=2265 range=241
                            angle=2364 range=224
                            angle=2462 range=211
                            angle=2561 range=202
                            angle=2660 range=202
                            angle=2758 range=135
                            angle=2857 range=135
                            angle=2956 range=135
                            angle=3054 range=228
                            angle=3153 range=254
                            angle=3252 range=248
                            angle=3350 range=244

                         * for single radar:
                         * PACKET READY -- angles: 26  packets: 1
                             angle=150 range=1440
                             angle=190 range=1450
                             angle=230 range=1450
                             angle=270 range=1450
                             angle=310 range=1460
                             angle=350 range=1540
                             angle=390 range=1540
                             angle=430 range=1700
                             angle=470 range=1700
                             angle=510 range=1740
                             angle=550 range=2260
                             angle=590 range=1100
                             angle=630 range=1100
                             angle=670 range=1090
                             angle=710 range=1100
                             angle=750 range=1090
                             angle=790 range=1090
                             angle=830 range=1090
                             angle=870 range=1090
                             angle=910 range=1700
                             angle=950 range=1710
                             angle=990 range=1730
                             angle=1030 range=1720
                             angle=1070 range=1710
                             angle=1110 range=3500
                             angle=1150 range=3500
                        */
                    }
                }
            }
        }

        // =================================================================================================================

        private int angleRawToSonarAngle(int channel, int rawAngle)
        {
            int ret = 0;

            // these formulas are very dependent on the positioning of the servos/sonars and type of servos. 
            // for my particular robot servos are located upside down, the shaft facing the ground. EXI 227F are fast and dirt cheap, but the metal gears have generally short life compared to new hitec carbon gears.
            // see CommLink::OnMeasurement() for the way the sonar data should look like, and how it will be converted to a 180 degrees sweep.
            switch (channel)
            {
                case 1:     // channel 1 is on the left
                    // ret = rawAngle
                    // ret = rawAngle1_Max + rawAngle2_Max + 99 - rawAngle;         // Futaba S3003 left, upside down, and Hitec HS300 right,  upside down
                    ret = rawAngle1_Max + rawAngle2_Max + 132 - rawAngle;           // Two EXI 227F upside down
                    break;
                case 2:     // channel 2 is on the right
                    // ret = rawAngle2_Max - (rawAngle - rawAngle2_Min) + rawAngle1_Max + 99;
                    // ret = rawAngle;
                    ret = rawAngle1_Min + (rawAngle1_Max - rawAngle) - 99;           // Two EXI 227F upside down
                    break;
            }

            return ret;
        }

        private void pmSetDefaultSweep()
        {
            double sweepStartPos1 = 2100.0d; // us
            double sweepStartPos2 = 850.0d;  // us
            double sweepStep = 148;          // these are not us
            int sweepSteps = 13;
            double sweepMax = 2100.0d;
            double sweepMin = 850;          // us
            double sweepRate = 28;          // must divide by 7, due to microprocessor servo pulse cycle interactions.
            bool initialDirectionUp = false;

            _picpxmod.ServoSweepParams(1, sweepMin, sweepMax, sweepStartPos1, sweepStep, initialDirectionUp, sweepRate);
            _picpxmod.ServoSweepParams(2, sweepMin, sweepMax, sweepStartPos2, sweepStep, initialDirectionUp, sweepRate);
        }

        // =================================================================================================================

        private void pmTestSonar1LinkLabel_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Random random = new Random();

            for (int angleRaw = 150; angleRaw <= 1160; angleRaw += 50)
            {
                pmSonarViewControl1.setReading(angleRaw, 3.0d * random.NextDouble(), DateTime.Now.Ticks);
            }
        }

        private void pmTestSonar2LinkLabel_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Random random = new Random();

            for (int angleRaw = 150; angleRaw <= 1160; angleRaw += 50)
            {
                pmSonarViewControl2.setReading(angleRaw, 3.0d * random.NextDouble(), DateTime.Now.Ticks);
            }
        }

        private void pmServoReadButton_Click(object sender, EventArgs e)
        {
            double dPosMks = _picpxmod.ServoPositionGet((int)pmServoNumberNumericUpDown.Value);

            pmServoPositionTextBox.Text = "" + dPosMks;
        }

        private void pmPingReadButton_Click(object sender, EventArgs e)
        {
            double dPingDistanceMm = _picpxmod.PingDistanceGet((int)pmServoNumberNumericUpDown.Value);

            pmPingDistanceTextBox.Text = "" + dPingDistanceMm;

        }

        private void picUsbServoSweepStartButton_Click(object sender, EventArgs e)
        {
            pmSonarViewControl1.Reset();
            _picpxmod.ServoSweepEnable(true);
        }

        private void picUsbServoSweepStopButton_Click(object sender, EventArgs e)
        {
            _picpxmod.ServoSweepEnable(false);
        }

        private void pmSetServoSweepButton_Click(object sender, EventArgs e)
        {
            double sweepMin = double.Parse(pmSweepMinTextBox.Text);
            double sweepMax = double.Parse(pmSweepMaxTextBox.Text);
            double sweepStartPos = double.Parse(pmSweepStartTextBox.Text);
            double sweepStep = double.Parse(pmSweepStepTextBox.Text);
            bool initialDirectionUp = pmSweepDirectionUpCheckBox.Checked;
            double sweepRate = double.Parse(pmSweepRateTextBox.Text);
            _picpxmod.ServoSweepParams((int)pmServoNumberNumericUpDown.Value, sweepMin, sweepMax, sweepStartPos, sweepStep, initialDirectionUp, sweepRate);
        }

        private void pmSetDefaultSweepButton_Click(object sender, EventArgs e)
        {
            pmSetDefaultSweep();
        }

        private void pmFlipSonar1LinkLabel_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            pmSonarViewControl1.flipped = !pmSonarViewControl1.flipped;
            pmFlipSonar1LinkLabel.Text = pmSonarViewControl1.flipped ? "Unflip" : "Flip";
        }

        private void pmFlipSonar2LinkLabel_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            pmSonarViewControl2.flipped = !pmSonarViewControl2.flipped;
            pmFlipSonar2LinkLabel.Text = pmSonarViewControl2.flipped ? "Unflip" : "Flip";
        }

        private void pmSetSafePostureButton_Click(object sender, EventArgs e)
        {
            _picpxmod.SafePosture();
        }

        private void pmFlipSonarCombLinkLabel_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            pmSonarViewControlComb.flipped = !pmSonarViewControlComb.flipped;
            pmFlipSonarCombLinkLabel.Text = pmSonarViewControlComb.flipped ? "Unflip" : "Flip";
        }

        private void pmTestSonarCombLinkLabel_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {

        }
	}
}
