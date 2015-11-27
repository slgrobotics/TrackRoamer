using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Text;
using System.IO.Ports;
using System.Globalization;

using TrackRoamer.Robotics.Utility.LibSystem;

namespace TrackRoamer.Robotics.Hardware.LibRoboteqController
{
	/// <summary>
	/// class RQController encapsulates all AX2850 commands, queries and mode switching, handles RS232 communication, including monitoring and heartbeat.
	/// it is intended to be the AX2850 hardware controller layer, not aware of robot configuration.
	/// </summary>
	public class ControllerRQAX2850 : ControllerBase
	{
        public bool doLogMeasuredValues = false;

		public event OnValueReceived onValueReceived_DigitalInputE;
        public event OnValueReceived onValueReceived_DigitalInputF;         // WhiskerLeft
        public event OnValueReceived onValueReceived_DigitalInputEmerg;     // WhiskerRight
		
		public event OnValueReceived onValueReceived_EncoderLeftAbsolute;
		public event OnValueReceived onValueReceived_EncoderRightAbsolute;

		public event OnValueReceived onValueReceived_EncoderSpeed;

        public event OnValueReceived onValueReceived_Voltage;
        public event OnValueReceived onValueReceived_MotorPower;
        public event OnValueReceived onValueReceived_MotorAmps;
        public event OnValueReceived onValueReceived_AnalogInputs;
        public event OnValueReceived onValueReceived_HeatsinkTemperature;


		protected string m_portName;
        protected SerialPort m_port = null;

		public long frameCounter = 0;
		public long errorCounter = 0;

		private RQInteractionQueue[] m_queues = new RQInteractionQueue[4];
		private RQInteractionQueue m_commandPowerLeftQueue;
		private RQInteractionQueue m_commandPowerRightQueue;
		private RQInteractionQueue m_commandQueue;
		private RQInteractionQueue m_queryQueue;

		private RQInteractionQueue m_currentQueue = null;
		private object m_currentQueuePadlock = "";

		private Dictionary<String, RQMeasuredValue> m_measuredValues = new Dictionary<String, RQMeasuredValue>();
		public Dictionary<String, RQMeasuredValue> measuredValues { get { return m_measuredValues; } }

		private Logger m_logger = null;
		private List<String> m_loggedValueNames = new List<String>();

		private List<RQQuery> m_querySet = new List<RQQuery>(); // of RQQuery, queries to use on regular basis as watchdog quieter.
        private List<RQQuery> m_querySetShort = new List<RQQuery>(); // of RQQuery, queries to use when most important data is needed.

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

        public bool isInError { get; private set; }

		#endregion // state variables

		#region lifecycle

		public ControllerRQAX2850(string portName)
		{
			Tracer.Trace("ControllerRQAX2850(" + portName + ")");

			m_portName = portName;

			m_queues[0] = m_commandQueue = new RQInteractionQueue("command", 3);    // highest priority factor
            m_queues[1] = m_commandPowerLeftQueue = new RQMotorCommandQueue("commandPowerLeft");    // one-slot queue; must be "equalized" with the m_commandPowerRightQueue in terms of processing order. PriorityFactor = 2;
			m_queues[2] = m_commandPowerRightQueue = new RQMotorCommandQueue("commandPowerRight");
			m_queues[3] = m_queryQueue = new RQInteractionQueue("query", 1);        // lowest priority factor

            RQQuery rqQueryEncoderSpeed = new RQQueryEncoderSpeed(m_measuredValues);
            rqQueryEncoderSpeed.onValueReceived += new OnValueReceived(rqQueryEncoderSpeed_onValueReceived);
            m_querySet.Add(rqQueryEncoderSpeed);
            m_querySetShort.Add(rqQueryEncoderSpeed);

            RQQuery rqQueryEncoderLeftAbsolute = new RQQueryEncoderLeftAbsolute(m_measuredValues);
            rqQueryEncoderLeftAbsolute.onValueReceived += new OnValueReceived(rqQueryEncoderLeftAbsolute_onValueReceived);
            m_querySet.Add(rqQueryEncoderLeftAbsolute);
            m_querySetShort.Add(rqQueryEncoderLeftAbsolute);

            RQQuery rqQueryEncoderRightAbsolute = new RQQueryEncoderRightAbsolute(m_measuredValues);
            rqQueryEncoderRightAbsolute.onValueReceived += new OnValueReceived(rqQueryEncoderRightAbsolute_onValueReceived);
            m_querySet.Add(rqQueryEncoderRightAbsolute);
            m_querySetShort.Add(rqQueryEncoderRightAbsolute);

            RQQuery rqQueryAnalogInputs = new RQQueryAnalogInputs(m_measuredValues);
            rqQueryAnalogInputs.onValueReceived += new OnValueReceived(rqQueryAnalogInputs_onValueReceived);
			m_querySet.Add(rqQueryAnalogInputs);

			RQQuery rqQueryDigitalInputs = new RQQueryDigitalInputs(m_measuredValues);
			rqQueryDigitalInputs.onValueReceived += new OnValueReceived(rqQueryDigitalInputs_onValueReceived);
			m_querySet.Add(rqQueryDigitalInputs);
            m_querySetShort.Add(rqQueryDigitalInputs);  // whiskers are in priority queue

			RQQuery rqQueryHeatsinkTemperature = new RQQueryHeatsinkTemperature(m_measuredValues);
            rqQueryHeatsinkTemperature.onValueReceived += new OnValueReceived(rqQueryHeatsinkTemperature_onValueReceived);
			m_querySet.Add(rqQueryHeatsinkTemperature);

			RQQuery rqQueryMotorAmps = new RQQueryMotorAmps(m_measuredValues);
            rqQueryMotorAmps.onValueReceived += new OnValueReceived(rqQueryMotorAmps_onValueReceived);
			m_querySet.Add(rqQueryMotorAmps);

			RQQuery rqueryMotorPower = new RQQueryMotorPower(m_measuredValues);
            rqueryMotorPower.onValueReceived += new OnValueReceived(rqueryMotorPower_onValueReceived);
			m_querySet.Add(rqueryMotorPower);

			RQQuery rqQueryVoltage = new RQQueryVoltage(m_measuredValues);
            rqQueryVoltage.onValueReceived += new OnValueReceived(rqQueryVoltage_onValueReceived);
			m_querySet.Add(rqQueryVoltage);

			foreach (RQQuery query in m_querySet)
			{
				foreach(string vName in query.ValueNames)
				{
					m_loggedValueNames.Add(vName);
				}
			}

			isUnknownState = true;
            isInError = false;

			//startMonitoringThread();
		}

        public override void init()					// throws ControllerException
        {
            Tracer.Trace("ControllerRQAX2850:init()");

            ensurePort();
            isUnknownState = true;
        }

        public override void Dispose()
        {
            Tracer.Trace("ControllerRQAX2850:Dispose() -- closing port " + m_portName);
            lock (this)
            {
                if (m_port != null)
                {
                    m_port.Close();
                    m_port = null;
                }
                //killMonitoringThread();
            }
        }

        #endregion // lifecycle

        #region Handlers for measured values coming from the controller as result of queries

        private double? prev_Main_Battery_Voltage = null;
        private double? prev_Internal_Voltage = null;
        private DateTime lastSentVoltage = DateTime.MinValue;

        void rqQueryVoltage_onValueReceived(object sender, MeasuredValuesEventArgs ev)
        {
            RQQueryVoltage query = (RQQueryVoltage)sender;
            double Main_Battery_Voltage = query.doubleValues[0];    // "Main_Battery_Voltage"
            double Internal_Voltage = query.doubleValues[1];        // "Internal_Voltage"

            if (Main_Battery_Voltage != prev_Main_Battery_Voltage || Internal_Voltage != prev_Internal_Voltage || (DateTime.Now - lastSentVoltage).TotalSeconds > 5.0d)     // force send every 5 sec
            {
                // Tracer.Trace("Voltage=" + Main_Battery_Voltage + "   " + Internal_Voltage);
                prev_Main_Battery_Voltage = Main_Battery_Voltage;
                prev_Internal_Voltage = Internal_Voltage;

                if (onValueReceived_Voltage != null)
                {
                    ev.timestamp = query.whenReceivedTicks;
                    ev.value1 = Main_Battery_Voltage;
                    ev.value2 = Internal_Voltage;
                    onValueReceived_Voltage(this, ev);
                }
            }
        }

        private double? prev_Motor_Power_Left = null;
        private double? prev_Motor_Power_Right = null;
        private DateTime lastSentMotor_Power = DateTime.MinValue;

        void rqueryMotorPower_onValueReceived(object sender, MeasuredValuesEventArgs ev)
        {
            RQQueryMotorPower query = (RQQueryMotorPower)sender;
            double Motor_Power_Left = query.doubleValues[0];     // "Motor_Power_Left"
            double Motor_Power_Right = query.doubleValues[1];    // "Motor_Power_Right"

            if (Motor_Power_Left != prev_Motor_Power_Left || Motor_Power_Right != prev_Motor_Power_Right || (DateTime.Now - lastSentMotor_Power).TotalSeconds > 5.0d)     // force send every 5 sec
            {
                //Tracer.Trace("MotorPower=" + Motor_Power_Left + "   " + Motor_Power_Right);
                prev_Motor_Power_Left = Motor_Power_Left;
                prev_Motor_Power_Right = Motor_Power_Right;

                if (onValueReceived_MotorPower != null)
                {
                    ev.timestamp = query.whenReceivedTicks;
                    ev.value1 = Motor_Power_Left;
                    ev.value2 = Motor_Power_Right;
                    onValueReceived_MotorPower(this, ev);
                }
            }
        }

        private double? prev_Motor_Amps_Left = null;
        private double? prev_Motor_Amps_Right = null;
        private DateTime lastSentMotor_Amps = DateTime.MinValue;

        void rqQueryMotorAmps_onValueReceived(object sender, MeasuredValuesEventArgs ev)
        {
            RQQueryMotorAmps query = (RQQueryMotorAmps)sender;
            double Motor_Amps_Left = query.doubleValues[0];     // "Motor_Amps_Left"
            double Motor_Amps_Right = query.doubleValues[1];    // "Motor_Amps_Right"

            if (Motor_Amps_Left != prev_Motor_Amps_Left || Motor_Amps_Right != prev_Motor_Amps_Right || (DateTime.Now - lastSentMotor_Amps).TotalSeconds > 5.0d)     // force send every 5 sec
            {
                //Tracer.Trace("MotorAmps=" + Motor_Amps_Left + "   " + Motor_Amps_Right);
                prev_Motor_Amps_Left = Motor_Amps_Left;
                prev_Motor_Amps_Right = Motor_Amps_Right;

                if (onValueReceived_MotorAmps != null)
                {
                    ev.timestamp = query.whenReceivedTicks;
                    ev.value1 = Motor_Amps_Left;
                    ev.value2 = Motor_Amps_Right;
                    onValueReceived_MotorAmps(this, ev);
                }
            }
        }

        private double? prev_Heatsink_Temperature_Left = null;
        private double? prev_Heatsink_Temperature_Right = null;
        private DateTime lastSentHeatsink_Temperature = DateTime.MinValue;

        void rqQueryHeatsinkTemperature_onValueReceived(object sender, MeasuredValuesEventArgs ev)
        {
            RQQueryHeatsinkTemperature query = (RQQueryHeatsinkTemperature)sender;
            double Heatsink_Temperature_Left = query.doubleValues[0];     // "Heatsink_Temperature_Left"
            double Heatsink_Temperature_Right = query.doubleValues[1];    // "Heatsink_Temperature_Right"

            if (Heatsink_Temperature_Left != prev_Heatsink_Temperature_Left || Heatsink_Temperature_Right != prev_Heatsink_Temperature_Right || (DateTime.Now - lastSentHeatsink_Temperature).TotalSeconds > 5.0d)     // force send every 5 sec
            {
                //Tracer.Trace("Heatsink Temperature=" + Heatsink_Temperature_Left + "   " + Heatsink_Temperature_Right);
                prev_Heatsink_Temperature_Left = Heatsink_Temperature_Left;
                prev_Heatsink_Temperature_Right = Heatsink_Temperature_Right;

                if (onValueReceived_HeatsinkTemperature != null)
                {
                    ev.timestamp = query.whenReceivedTicks;
                    ev.value1 = Heatsink_Temperature_Left;
                    ev.value2 = Heatsink_Temperature_Right;
                    onValueReceived_HeatsinkTemperature(this, ev);
                }
            }
        }

        private double? prev_Analog_Input_1 = null;
        private double? prev_Analog_Input_2 = null;
        private DateTime lastSentAnalog_Input = DateTime.MinValue;

        void rqQueryAnalogInputs_onValueReceived(object sender, MeasuredValuesEventArgs ev)
        {
            RQQueryAnalogInputs query = (RQQueryAnalogInputs)sender;
            double Analog_Input_1 = query.doubleValues[0];     // "Analog_Input_1"
            double Analog_Input_2 = query.doubleValues[1];     // "Analog_Input_2"

            if (Analog_Input_1 != prev_Analog_Input_1 || Analog_Input_2 != prev_Analog_Input_2 || (DateTime.Now - lastSentAnalog_Input).TotalSeconds > 5.0d)     // force send every 5 sec
            {
                //Tracer.Trace("Analog Inputs=" + Analog_Input_1 + "   " + Analog_Input_2);
                prev_Analog_Input_1 = Analog_Input_1;
                prev_Analog_Input_2 = Analog_Input_2;

                if (onValueReceived_AnalogInputs != null)
                {
                    ev.timestamp = query.whenReceivedTicks;
                    ev.value1 = Analog_Input_1;
                    ev.value2 = Analog_Input_2;
                    onValueReceived_AnalogInputs(this, ev);
                }
            }
        }

		private double? prev_digitalInputE = null;
		private double? prev_digitalInputF = null;
		private double? prev_digitalInputEmerg = null;

		void rqQueryDigitalInputs_onValueReceived(object sender, MeasuredValuesEventArgs ev)
		{
			RQQueryDigitalInputs query = (RQQueryDigitalInputs)sender;
			double digitalInputE = query.doubleValues[0];
			double digitalInputF = query.doubleValues[1];
			double digitalInputEmerg = query.doubleValues[2];

			if(digitalInputE != prev_digitalInputE)
			{
                Tracer.Trace("ControllerRQAX2850: DigitalInputE changed : " + prev_digitalInputE + " -> " + digitalInputE);
				prev_digitalInputE = digitalInputE;

				if (onValueReceived_DigitalInputE != null)
				{
					//ev = new MeasuredValuesEventArgs();
					ev.timestamp = query.whenReceivedTicks;
					ev.value1 = digitalInputE;
					onValueReceived_DigitalInputE(this, ev);
				}
			}

			if (digitalInputF != prev_digitalInputF)      // WhiskerLeft
			{
                //Tracer.Trace("ControllerRQAX2850: DigitalInputF changed : " + prev_digitalInputF + " -> " + digitalInputF);
				prev_digitalInputF = digitalInputF;

                if (onValueReceived_DigitalInputF != null)
				{
					//ev = new MeasuredValuesEventArgs();
					ev.timestamp = query.whenReceivedTicks;
					ev.value1 = digitalInputF;
					onValueReceived_DigitalInputF(this, ev);
				}
			}

            if (digitalInputEmerg != prev_digitalInputEmerg)    // WhiskerRight
			{
                //Tracer.Trace("ControllerRQAX2850: DigitalInputEmerg changed : " + prev_digitalInputEmerg + " -> " + digitalInputEmerg);
				prev_digitalInputEmerg = digitalInputEmerg;

				if (onValueReceived_DigitalInputEmerg != null)
				{
					//ev = new MeasuredValuesEventArgs();
					ev.timestamp = query.whenReceivedTicks;
					ev.value1 = digitalInputEmerg;
					onValueReceived_DigitalInputEmerg(this, ev);
				}
			}
		}

		private double? prev_encoderLeftAbsolute = null;
		private double? prev_encoderRightAbsolute = null;

		void rqQueryEncoderLeftAbsolute_onValueReceived(object sender, MeasuredValuesEventArgs ev)
		{
			RQQueryEncoderLeftAbsolute query = (RQQueryEncoderLeftAbsolute)sender;
			double encoderLeftAbsolute = query.doubleValues[0];

			if (encoderLeftAbsolute != prev_encoderLeftAbsolute)    // note: nonnull value is not equal to a null value
			{
                //Tracer.Trace("ControllerRQAX2850: EncoderLeftAbsolute changed : " + prev_encoderLeftAbsolute + " -> " + encoderLeftAbsolute);
				prev_encoderLeftAbsolute = encoderLeftAbsolute;

				if (onValueReceived_EncoderLeftAbsolute != null)
				{
					//ev = new MeasuredValuesEventArgs();
					ev.timestamp = query.whenReceivedTicks;
					ev.value1 = encoderLeftAbsolute;
					onValueReceived_EncoderLeftAbsolute(this, ev);
				}
			}
		}

		void rqQueryEncoderRightAbsolute_onValueReceived(object sender, MeasuredValuesEventArgs ev)
		{
			RQQueryEncoderRightAbsolute query = (RQQueryEncoderRightAbsolute)sender;
			double encoderRightAbsolute = query.doubleValues[0];

			if (encoderRightAbsolute != prev_encoderRightAbsolute)    // note: nonnull value is not equal to a null value
			{
                //Tracer.Trace("ControllerRQAX2850: EncoderRightAbsolute changed : " + prev_encoderRightAbsolute + " -> " + encoderRightAbsolute);
				prev_encoderRightAbsolute = encoderRightAbsolute;

				if (onValueReceived_EncoderRightAbsolute != null)
				{
					//ev = new MeasuredValuesEventArgs();
					ev.timestamp = query.whenReceivedTicks;
					ev.value1 = encoderRightAbsolute;
					onValueReceived_EncoderRightAbsolute(this, ev);
				}
			}
		}

        /// <summary>
        /// to evaluate speed for the purpose of filling m_queryQueue with either full set of queries,
        /// or encoder-only set.
        /// </summary>
        private int maxEncoderSpeed
        {
            get
            {
                return (int)Math.Round(Math.Max(Math.Abs(prev_encoderLeftSpeed), Math.Abs(prev_encoderRightSpeed)));
            }
        }

        // to simplify maxEncoderSpeed(), we don't use nullables here:
		private double prev_encoderLeftSpeed = 0;
		private double prev_encoderRightSpeed = 0;

		void rqQueryEncoderSpeed_onValueReceived(object sender, MeasuredValuesEventArgs ev)
		{
			RQQueryEncoderSpeed query = (RQQueryEncoderSpeed)sender;
			double leftSpeed = query.doubleValues[0];
			double rightSpeed = query.doubleValues[1];

			if (leftSpeed != prev_encoderLeftSpeed || rightSpeed != prev_encoderRightSpeed)
			{
                //Tracer.Trace("ControllerRQAX2850: EncoderSpeed changed   L: " + prev_encoderLeftSpeed + " -> " + leftSpeed + "    R: " + prev_encoderRightSpeed + " -> " + rightSpeed);
				prev_encoderLeftSpeed = leftSpeed;
				prev_encoderRightSpeed = rightSpeed;

				if (onValueReceived_EncoderSpeed != null)
				{
					//ev = new MeasuredValuesEventArgs();
					ev.timestamp = query.whenReceivedTicks;
					ev.value1 = leftSpeed;
					ev.value2 = rightSpeed;
					onValueReceived_EncoderSpeed(this, ev);
				}
			}
		}

        #endregion // Handlers for measured values coming from the controller

		public override bool DeviceValid()			// IdentifyDeviceType found something real
		{
			return true;
		}

		public override void IdentifyDeviceType()
		{
		}

		public override bool GrabController()				// returns true on success
		{
			Tracer.Trace("ControllerRQAX2850: GrabController()");

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
			Tracer.Trace("ControllerRQAX2850: ResetController()");

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

		public override void ResetEncoderLeft()
		{
			RQCommandResetEncoderLeft cmd = new RQCommandResetEncoderLeft();

			// we want to reset encoder in somewhat synchronous manner - prior to motor commands

			cmd.queue = m_commandQueue;		// to allow re-queueing of a failed command

			m_commandQueue.Enqueue(cmd);
		}

		public override void ResetEncoderRight()
		{
			RQCommandResetEncoderRight cmd = new RQCommandResetEncoderRight();

			// we want to reset encoder in somewhat synchronous manner - prior to motor commands

			cmd.queue = m_commandQueue;		// to allow re-queueing of a failed command

			m_commandQueue.Enqueue(cmd);
		}

        public override void SetOutputC(bool on)
		{
            RQCommandOutputC cmd = new RQCommandOutputC(on);

			cmd.queue = m_commandQueue;		// to allow re-queueing of a failed command

			m_commandQueue.Enqueue(cmd);
		}

		public override string ToString()
		{
			return "RoboteQ AX2850 Controller";
		}

		#region serial port operations

		public virtual void ensurePort()
		{
			if (m_port == null)
			{
				Tracer.Trace("ControllerRQAX2850: ensurePort() -- opening port " + m_portName);

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

		public virtual void port_ErrorReceived(object sender, SerialErrorReceivedEventArgs e)
		{
			errorCounter++;
            Tracer.Error("ControllerRQAX2850: port_ErrorReceived: " + e.ToString());
		}

		private StringBuilder m_sb = new StringBuilder();
		private int m_wCnt = 0;		// watchdog "W" consecutive count to make sure we've grabbed the controller all right

		public void port_DataReceived(object sender, SerialDataReceivedEventArgs e)
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
                    Tracer.Trace("ControllerRQAX2850: got W");
					m_wCnt++;
					if (m_wCnt == 3)
					{
						// we detected watchdog. Set state flags, so that queries and commands may flow and supress watchdog
						isGrabbed = true;
                        Tracer.Trace("OK: ControllerRQAX2850: watchdog detected");
					}
				}
				else
				{
					m_wCnt = 0;
					m_sb.Append(RxString);
				}
			}

			// Show all the incoming data in the port's buffer
            //Tracer.Trace(m_port.ReadExisting());
		}

		/// <summary>
		/// hopefully not crippled string came in - wether a monitoring stream, a response to query or an echo from query or command
		/// </summary>
		/// <param name="str"></param>
        /// <param name="timestamp"></param>
        protected void onStringReceived(string str, long timestamp)
		{
            //Tracer.Trace("ControllerRQAX2850:onStringReceived() str=" + str);

			frameCounter++;
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
                Tracer.Error("ControllerRQAX2850: " + exc.Message);
                errorCounter++;
			}
		}

		#endregion // serial port operations

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

        /// <summary>
        /// only needed when controller is in RC mode, sending constant stream of data for monitoring purposes.
        /// when grabbed, controller is queried via m_queryQueue
        /// </summary>
        /// <param name="monStr"></param>
        /// <param name="timestamp"></param>
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
					measuredValue = m_measuredValues[valueName];
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

        #region ExecuteMain()

        public string CurrentStatusLabel
        {
            get
            {
                return string.Format("{0} for {1:hh\\:mm\\:ss}", lastStatusLabel, (DateTime.Now - lastStatusLabelChanged));
            }
        }

        private string lastStatusLabel = "";
        private DateTime lastStatusLabelChanged = DateTime.Now;

        private DateTime querySetLastSent = DateTime.MinValue;  // when we last sent from m_querySet
        private bool isShortQueryMode = false;  // if true, we are replentishing query queue from the m_querySetShort. when false - from m_querySet

		public void ExecuteMain()
		{
			string statusLabel = "";

			lock (this)
			{
				if (isGrabbed)
				{
					statusLabel = "grabbed";

					lock (m_currentQueuePadlock)
					{
						if (m_currentQueue != null)
						{
							// still processing current query
							if (m_currentQueue.checkForTimeout())
							{
								m_currentQueue = null;
                                statusLabel = "timeout on AX2850 serial connection";
                                isUnknownState = true;
                                isInError = true;
							}
						}
					}

                trySendNow:

					if (m_currentQueue == null && !isInError)
					{
						// we are (almost) for sure not processing any interaction (talking to the controller).
						// if no queue is talking, find first queue in priority list that has a processable interaction (has something to send).
                        // PriorityCurrent depends on time spent waiting grows fast with time multiplied by the queue priority factor; isProcessingInteraction==true always gets first place.
                        RQInteractionQueue queue = (from q in m_queues
                                     where q.HasInteractionsQueued || q.isProcessingInteraction
                                     orderby q.isProcessingInteraction descending, q.PriorityCurrent descending
                                     select q).FirstOrDefault();

                        DateTime now = DateTime.Now;

						if (queue != null)
						{
                            // queue is either talking to the controller, or has something to send
							lock (queue.padlock)
							{
								if (!queue.isProcessingInteraction) // if queue is processing interaction, we skip this cycle and let the read handlers do their job.
								{
									// done with the current interaction, get next one:
									if (queue.HasInteractionsQueued)
									{
										m_currentQueue = queue;		// when responses are processed, this is the queue to use.
                                        if (queue is RQMotorCommandQueue)
                                        {
                                            // we cannot send motor commands more frequently than 16ms. Mark the moment when we send a command
                                            // - this is static, common for both Left and Right motor queues:
                                            RQMotorCommandQueue.lastSentTicksMotorCmd = now.Ticks;
                                            if (!isShortQueryMode)
                                            {
                                                m_queryQueue.Clear();   // prepare to be filled from m_querySetShort 
                                                isShortQueryMode = (now - querySetLastSent).TotalSeconds < 1.2d;
                                                if (isShortQueryMode)
                                                {
                                                    foreach (RQQuery q in m_querySetShort)
                                                    {
                                                        q.reset();
                                                        m_queryQueue.Enqueue(q);
                                                    }
                                                }
                                                else
                                                {
                                                    foreach (RQQuery q in m_querySet)
                                                    {
                                                        q.reset();
                                                        m_queryQueue.Enqueue(q);
                                                    }
                                                    querySetLastSent = now;
                                                }
                                            }
                                            //Console.Write("M");
                                        }
                                        //else
                                        //{
                                        //    //Console.Write(isShortQueryMode ? "Q" : "L");
                                        //}
                                        queue.waitingSinceTicks = now.Ticks;
										m_port.WriteLine(queue.dequeueForSend());       // send a command or a query to controller.
									}
								}
							}
						}
						else
						{
							// all queues are empty (nothing to send), replentish the query queue.
                            // decide which query set to send - when moving fast, encoders and whiskers have priority.
                            if ((now - querySetLastSent).TotalSeconds > 1.2d || maxEncoderSpeed == 0)
                            {
                                // sending from m_querySet:

                                //Tracer.Trace("====================================================================== LONG : " + maxEncoderSpeed + "   " + prev_encoderLeftSpeed + "   " + prev_encoderRightSpeed);
                                isShortQueryMode = false;
                                foreach (RQQuery q in m_querySet)
                                {
                                    q.reset();
                                    m_queryQueue.Enqueue(q);
                                }
                                querySetLastSent = now;
                            }
                            else
                            {
                                // sending from m_querySetShort:

                                //Tracer.Trace("-------------------------------- SHORT -------------------------------------: " + maxEncoderSpeed + "   " + prev_encoderLeftSpeed + "   " + prev_encoderRightSpeed);
                                isShortQueryMode = true;
                                foreach (RQQuery q in m_querySetShort)
                                {
                                    q.reset();
                                    m_queryQueue.Enqueue(q);
                                }
                            }

                            // we now definitely have something to send, use this cycle to start the next interaction:
                            goto trySendNow;
						}
					}
				}
				else if (isMonitored)
				{
                    isInError = false;
					statusLabel = "monitored";
				}
				else if (isOnline)
				{
                    isInError = false;
                    statusLabel = "online - receiving data";
				}
				else
				{
                    isInError = false;
                    statusLabel = "not connected";
				}

                // trace when state changes:
				if (!statusLabel.Equals(lastStatusLabel))
				{
					Tracer.Trace2("AX2850 : " + statusLabel);
					lastStatusLabel = statusLabel;
                    lastStatusLabelChanged = DateTime.Now;
				}

                if (doLogMeasuredValues)
                {
                    // create and append to a file like c:\temp\log_20120916_144334.csv for post-run motor performance analysis
                    logMeasuredValues();
                }
			}
		}

        #endregion // ExecuteMain()

        #region logMeasuredValues()

        private DateTime m_lastMeasuredValuesLoggedTime = DateTime.MinValue;
		private int measuredValuesLogIntervalMs = 1000;
		//private DateTime m_firstLogCall;

		private void logMeasuredValues()
		{
			if (m_measuredValues.Count < 2)
			{
				//m_firstLogCall = DateTime.Now;
				return;
			}

			//if ((DateTime.Now - m_firstLogCall).TotalSeconds < 10)
			//{
			//    return;
			//}

            if (m_logger == null)
            {
                m_logger = new Logger(m_measuredValues, m_loggedValueNames);
            }

			if(    (DateTime.Now - m_lastMeasuredValuesLoggedTime).TotalMilliseconds >= measuredValuesLogIntervalMs
				&& (isMonitored || isGrabbed)
				)
			{
				m_lastMeasuredValuesLoggedTime = DateTime.Now;

				Logger.Log();
			}
		}

        #endregion // logMeasuredValues()
    }
}
