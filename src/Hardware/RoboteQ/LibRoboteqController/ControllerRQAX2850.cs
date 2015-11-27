using System;
using System.Collections;
using System.Threading;
using System.Text;
using System.IO.Ports;
using System.Globalization;

using LibSystem;

namespace LibRoboteqController
{
	/// <summary>
	/// class RQController encapsulates all AX2850 commands, queries and mode switching, handles RS232 communication, including monitoring and heartbeat.
	/// it is intended to be the AX2850 hardware controller layer, not aware of robot configuration.
	/// </summary>
	public class ControllerRQAX2850 : ControllerBase
	{
		private string m_portName;
		private SerialPort m_port = null;
		private RQInteractionQueue[] m_queues = new RQInteractionQueue[4];
		private RQInteractionQueue m_commandPowerLeftQueue;
		private RQInteractionQueue m_commandPowerRightQueue;
		private RQInteractionQueue m_commandQueue;
		private RQInteractionQueue m_queryQueue;

		private RQInteractionQueue m_currentQueue = null;
		private object m_currentQueuePadlock = "";

		private Hashtable m_measuredValues = new Hashtable();
		public Hashtable measuredValues { get { return m_measuredValues; } }

		private Logger m_logger = null;
		private ArrayList m_loggedValueNames = new ArrayList();

		private ArrayList m_querySet = new ArrayList(); // of RQQuery, queries to use on regular basis as watchdog quieter.

		#region state variables

		private bool m_isOnline = false;
		public bool isOnline {
			get { return m_isOnline; }			// cable connected, port alive, receiving data and is either grabbed or monitored
			set
			{
				m_isOnline = value;
			}
		} 

		private bool m_isGrabbed = false;
		public bool isGrabbed {
			get { return m_isGrabbed; }		// is online, watchdog satisfied (we are sending stream of queries) and ready for commands
			set
			{
				// the value is ignored - you can set it to true only to make a point
				m_isGrabbed = true;
				m_isMonitored = false;
			}
		}

		private bool m_isMonitored = false;
		public bool isMonitored {
			get { return m_isMonitored; }	// is online, but not in RS232 mode, we are receiving monitoring lines
			set
			{
				// the value is ignored - you can set it to true only to make a point
				m_isMonitored = true;
				m_isGrabbed = false;
			}
		}

		public bool isUnknownState {
			get { return !m_isGrabbed && !m_isMonitored; }
			set
			{
				// after commands that may change the state (like reset) - let the incoming stream set the state as appropriate.
				// the value is ignored - you can set it to true only to make a point
				isOnline = false;
				m_isMonitored = false;
				m_isGrabbed = false;
			}
		}

		#endregion // state variables

		#region lifecycle

		public ControllerRQAX2850(string portName)
		{
			m_portName = portName;

			m_queues[0] = m_commandPowerLeftQueue = new RQMotorCommandQueue("commandPowerLeft");
			m_queues[1] = m_commandPowerRightQueue = new RQMotorCommandQueue("commandPowerRight");
			m_queues[2] = m_commandQueue = new RQInteractionQueue("command");
			m_queues[3] = m_queryQueue = new RQInteractionQueue("query");

			m_querySet.Add(new RQQueryAnalogInputs(m_measuredValues));
			m_querySet.Add(new RQQueryDigitalInputs(m_measuredValues));
			m_querySet.Add(new RQQueryHeatsinkTemperature(m_measuredValues));
			m_querySet.Add(new RQQueryMotorAmps(m_measuredValues));
			m_querySet.Add(new RQQueryMotorPower(m_measuredValues));
			m_querySet.Add(new RQQueryVoltage(m_measuredValues));
			m_querySet.Add(new RQQueryEncoderLeftAbsolute(m_measuredValues));
			m_querySet.Add(new RQQueryEncoderRightAbsolute(m_measuredValues));
			m_querySet.Add(new RQQueryEncoderSpeed(m_measuredValues));

			foreach (RQQuery query in m_querySet)
			{
				foreach(string vName in query.ValueNames)
				{
					m_loggedValueNames.Add(vName);
				}
			}

			m_logger = new Logger(m_measuredValues, m_loggedValueNames);

			startMonitoringThread();
		}

		public override void init()					// throws ControllerException
		{
			ensurePort();
			isUnknownState = true;
		}

		public override void Dispose()
		{
			lock (this)
			{
				m_port.Close();
				m_port = null;

				killMonitoringThread();
			}
		}

		#endregion // lifecycle

		public override bool DeviceValid()			// IdentifyDeviceType found something real
		{
			return true;
		}

		public override void IdentifyDeviceType()
		{
		}

		public override bool GrabController()				// returns true on success
		{
			ensurePort();

			byte[] oneCR = new byte[] { 0x0D };

			for (int i = 0; i < 10; i++)
			{
				Thread.Sleep(30);						// 10 doesn't work, 20 and more works fine
				m_port.Write(oneCR, 0, 1);
			}

			isUnknownState = true;

			//m_queryQueue.Enqueue(m_querySet[0]);

			return true;
		}

		public override bool ResetController()				// returns true on success
		{
			ensurePort();
			m_port.WriteLine("%rrrrrr");

			isUnknownState = true;

			return true;
		}

		public override int SetMotorPowerOrSpeedLeft(int powerOrSpeed)
		{
			RQCommandMotorPowerLeft cmd = new RQCommandMotorPowerLeft(powerOrSpeed);

			cmd.queue = m_commandPowerLeftQueue;		// to allow re-queueing of a failed command

			m_commandPowerLeftQueue.Enqueue(cmd);

			return powerOrSpeed;
		}

		public override int SetMotorPowerOrSpeedRight(int powerOrSpeed)
		{
			RQCommandMotorPowerRight cmd = new RQCommandMotorPowerRight(powerOrSpeed);

			cmd.queue = m_commandPowerRightQueue;		// to allow re-queueing of a failed command

			m_commandPowerRightQueue.Enqueue(cmd);

			return powerOrSpeed;
		}

		public override string ToString()
		{
			return "RoboteQ AX2850 Controller";
		}

		#region serial port operations

		private void ensurePort()
		{
			if (m_port == null)
			{
				m_port = new SerialPort(m_portName, 9600, System.IO.Ports.Parity.Even, 7, System.IO.Ports.StopBits.One);

				// Attach a method to be called when there is data waiting in the port's buffer
				m_port.DataReceived += new SerialDataReceivedEventHandler(port_DataReceived);
				m_port.ErrorReceived += new SerialErrorReceivedEventHandler(port_ErrorReceived);

				m_port.NewLine = "\r";
				m_port.DtrEnable = true;
				m_port.Handshake = System.IO.Ports.Handshake.None;
				m_port.Encoding = Encoding.ASCII;
				m_port.RtsEnable = true;

				// Open the port for communications
				m_port.Open();
				m_port.DiscardInBuffer();
			}
		}

		void port_ErrorReceived(object sender, SerialErrorReceivedEventArgs e)
		{
			Tracer.Error("port_ErrorReceived: " + e.ToString());
		}

		private StringBuilder m_sb = new StringBuilder();
		private int m_wCnt = 0;		// watchdog "W" consecutive count to make sure we've grabbed the controller all right

		private void port_DataReceived(object sender, SerialDataReceivedEventArgs e)
		{
			long timestamp = DateTime.Now.Ticks;
			isOnline = true;
			while (m_port.BytesToRead > 0)
			{
				byte b = (byte)m_port.ReadByte();
				byte[] bb = new byte[1];
				bb[0] = b;
				Encoding enc = Encoding.ASCII;
				string RxString = enc.GetString(bb);
				if (RxString == "\r")
				{
					m_wCnt = 0;
					onStringReceived(m_sb.ToString(), timestamp);
					m_sb.Remove(0, m_sb.Length);
				}
				else if (isUnknownState && RxString == "W")
				{
					//Tracer.Trace("got W");
					m_wCnt++;
					if (m_wCnt == 3)
					{
						// we detected watchdog. Set state flags, so that queries and commands may flow and supress watchdog
						isGrabbed = true;
						Tracer.Trace("OK: watchdog detected");
					}
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

		#endregion // serial port operations

		/// <summary>
		/// hopefully not crippled string came in - wether a monitoring stream, a response to query or an echo from query or command
		/// </summary>
		/// <param name="str"></param>
		private void onStringReceived(string str, long timestamp)
		{
			try
			{
				if (str.StartsWith(":") && (str.Length == 25 || str.Length == 33))
				{
					isMonitored = true;

					// :0000000000FE00009898396AB7B70000  - page 103 of controller doc
					// the 9898 is temperature 1 and 2
					// the 396A is voltages 
					// the 0000 at the end is speed - 00 left 00 right

					interpretMonitoringString(str, timestamp);

				}
				else if (isGrabbed)
				{
					lock (m_currentQueuePadlock)
					{
						if (m_currentQueue != null)
						{
							if (m_currentQueue.isProcessingInteraction)
							{
								if (m_currentQueue.onStringReceived(str, timestamp))		// last line will return true
								{
									m_currentQueue = null;
								}
							}
						}
					}
				}
			}
			catch(Exception exc)
			{
			}
		}

		#region interpretMonitoringString()

		// value names here must be in accordance to monitoring line that comes from controller in R/C mode:
		private static string[] monitorStringValueNames = new string[] {
			"Command1", "Command2", "Motor_Power_Left", "Motor_Power_Right", "Analog_Input_1", "Analog_Input_2",
			"Motor_Amps_Left", "Motor_Amps_Right", "Heatsink_Temperature_Left", "Heatsink_Temperature_Right",
			"Main_Battery_Voltage", "Internal_Voltage", "Value1", "Value2",
			"Encoder_Speed_Left", "Encoder_Speed_Right", "1", "2", "3", "4"		// some extra just in case
		};

		private static byte[] monitorStringValueConverters = new byte[] {
			0, 0, 0, 0, 4, 4, 
			0, 0, 3, 3,
			1, 2, 0, 0,
			4, 4, 0, 0, 0, 0
		};

		private void interpretMonitoringString(string monStr, long timestamp)
		{
			// Tracer.Trace("MON: " + monStr);

			int j = 0;
			for (int i = 1; i < monStr.Length && j < monitorStringValueNames.GetLength(0) ; i += 2)
			{
				string sHexVal = monStr.Substring(i, 2);
				string valueName = monitorStringValueNames[j];

				RQMeasuredValue measuredValue = new RQMeasuredValue();
				bool mustAdd = false;

				if (m_measuredValues.ContainsKey(valueName))
				{
					measuredValue = (RQMeasuredValue)m_measuredValues[valueName];
				}
				else
				{
					measuredValue = new RQMeasuredValue();
					mustAdd = true;
				}

				lock (measuredValue)
				{
					measuredValue.timestamp = timestamp;
					measuredValue.valueName = valueName;
					measuredValue.stringValue = sHexVal;
					measuredValue.intValue = Int32.Parse(sHexVal, NumberStyles.HexNumber);
					switch (monitorStringValueConverters[j])
					{
						default:
							measuredValue.doubleValue = (double)measuredValue.intValue;
							break;
						case 1:
							measuredValue.doubleValue = RQVoltage.convertToMainVoltage(sHexVal);
							break;
						case 2:
							measuredValue.doubleValue = RQVoltage.convertToInternalVoltage(sHexVal);
							break;
						case 3:
							measuredValue.doubleValue = RQTemperature.convertToCelcius(sHexVal);
							break;
						case 4:
							measuredValue.intValue = RQCompressedHex.convertToInt(sHexVal);
							measuredValue.doubleValue = (double)measuredValue.intValue;
							break;
					}
				}

				if (mustAdd)
				{
					m_measuredValues.Add(valueName, measuredValue);
				}

				// Tracer.Trace(valueName + "=" + sHexVal + "=" + measuredValue.doubleValue);
				j++;
			}
		}

		#endregion // interpretMonitoringString()

		private void controlLoop()
		{
			while (true)
			{
				lock (this)
				{
					if (isGrabbed)
					{
						Tracer.Trace2("grabbed");

						lock (m_currentQueuePadlock)
						{
							if (m_currentQueue != null)
							{
								// still processing current query
								if (m_currentQueue.checkForTimeout())
								{
									m_currentQueue = null;
								}
							}
						}

						if (m_currentQueue == null)
						{
							// we are for sure not processing any interaction.
							// find first queue in priority list that has a processable interaction:
							RQInteractionQueue queue = null;
							foreach (RQInteractionQueue q in m_queues)
							{
								if (q.isProcessingInteraction || q.HasInteractionsQueued)
								{
									queue = q;
									break;
								}
							}

							if (queue != null)
							{
								lock (queue.padlock)
								{
									if (!queue.isProcessingInteraction)
									{
										// done with the current interaction, get next one:
										if (queue.HasInteractionsQueued)
										{
											m_currentQueue = queue;		// when responses are processed, this is the queue to use.
											queue.lastSentTicks = DateTime.Now.Ticks;
											m_port.WriteLine(queue.dequeueForSend());
										}
									}
								}
							}
							else
							{
								// all queues are empty, replentish the query queue:
								foreach (RQQuery q in m_querySet)
								{
									q.reset();
									m_queryQueue.Enqueue(q);
								}
							}
						}
					}
					else if (isMonitored)
					{
						Tracer.Trace2("monitored");
					}
					else if (isOnline)
					{
						Tracer.Trace2("online - receiving data");
					}
					else
					{
						Tracer.Trace2("not connected");
					}

					logMeasuredValues();
				}

				Thread.Sleep(1);
			}
		}

		private DateTime m_lastMeasuredValuesLoggedTime = DateTime.MinValue;
		private int measuredValuesLogIntervalMs = 1000;
		private DateTime m_firstLogCall;

		private void logMeasuredValues()
		{
			if (m_measuredValues.Count < 2)
			{
				m_firstLogCall = DateTime.Now;
				return;
			}

			//if ((DateTime.Now - m_firstLogCall).TotalSeconds < 10)
			//{
			//    return;
			//}

			if(    (DateTime.Now - m_lastMeasuredValuesLoggedTime).TotalMilliseconds >= measuredValuesLogIntervalMs
				&& (isMonitored || isGrabbed)
				)
			{
				m_lastMeasuredValuesLoggedTime = DateTime.Now;

				Logger.Log();
			}
		}

		#region Monitoring Thread

		private Thread m_monitoringThread = null;
		private DateTime m_started = DateTime.Now;

		private void startMonitoringThread()
		{
			try
			{
				m_monitoringThread = new Thread(new System.Threading.ThreadStart(controlLoop));
				m_monitoringThread.Name = "RoboteqControllerControlLoop";
				m_monitoringThread.IsBackground = true;
				// see Entry.cs for how the current culture is set:
				m_monitoringThread.CurrentCulture = Thread.CurrentThread.CurrentCulture; //new CultureInfo("en-US", false);
				m_monitoringThread.CurrentUICulture = Thread.CurrentThread.CurrentUICulture; //new CultureInfo("en-US", false);
				m_monitoringThread.Priority = ThreadPriority.AboveNormal;
				m_started = DateTime.Now;
				m_monitoringThread.Start();
				Thread.Sleep(0);			//Give thread time to start. By documentation, 0 should work, but it does not!
			}
			catch (Exception exc)
			{
				Tracer.Error("startMonitoringThread" + exc.Message);
			}
		}

		/// <summary>
		/// we need to kill the thread if a long upload/download is in progress and we click "Stop" or close the dialog
		/// </summary>
		/// <returns></returns>
		private bool killMonitoringThread()
		{
			bool ret = (m_monitoringThread != null);

			Thread rtThread = m_monitoringThread;

			Tracer.Trace("killMonitoringThread");
			if (rtThread != null)
			{
				try
				{
					Tracer.Trace("killMonitoringThread - aborting");
					if (rtThread.IsAlive)
					{
						rtThread.Abort();
					}
				}
				catch (Exception e)
				{
					Tracer.Error("killMonitoringThread - while aborting - " + e);
				}
				m_monitoringThread = null;
				//				Project.inhibitRefresh = false;
				Tracer.Trace("killMonitoringThread - finished");
			}
			else
			{
				Tracer.Trace("killMonitoringThread - no thread to kill");
			}
			return ret;
		}

		#endregion // Monitoring Thread
	}
}
