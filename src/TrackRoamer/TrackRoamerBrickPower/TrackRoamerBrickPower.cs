/*
* Copyright (c) 2011..., Sergei Grichine
* All rights reserved.
*
* Redistribution and use in source and binary forms, with or without
* modification, are permitted provided that the following conditions are met:
*     * Redistributions of source code must retain the above copyright
*       notice, this list of conditions and the following disclaimer.
*     * Redistributions in binary form must reproduce the above copyright
*       notice, this list of conditions and the following disclaimer in the
*       documentation and/or other materials provided with the distribution.
*     * Neither the name of Sergei Grichine nor the
*       names of contributors may be used to endorse or promote products
*       derived from this software without specific prior written permission.
*
* THIS SOFTWARE IS PROVIDED BY Sergei Grichine ''AS IS'' AND ANY
* EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED
* WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE
* DISCLAIMED. IN NO EVENT SHALL Sergei Grichine BE LIABLE FOR ANY
* DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES
* (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES;
* LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND
* ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT
* (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS
* SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
*
* this is a X11 (BSD Revised) license - you do not have to publish your changes,
* although doing so, donating and contributing is always appreciated
*/

//#define TRACEDEBUG
//#define TRACEDEBUGTICKS
#define TRACELOG

namespace TrackRoamer.Robotics.Services.TrackRoamerBrickPower
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Xml;
    using System.Net;
    using System.IO;

    using Microsoft.Ccr.Core;
    using Microsoft.Dss.Core;
    using Microsoft.Dss.Core.Attributes;
    using Microsoft.Dss.ServiceModel.Dssp;
    using Microsoft.Dss.Core.DsspHttp;
    using Microsoft.Dss.ServiceModel.DsspServiceBase;
    using W3C.Soap;

    using coord = Microsoft.Robotics.Services.Coordination;
    using submgr = Microsoft.Dss.Services.SubscriptionManager;

    using TrackRoamer.Robotics.Utility.LibSystem;
    using TrackRoamer.Robotics.Hardware.LibRoboteqController;

    /// <summary>
    /// The TrackRoamer Power Brick Service
    /// </summary>
	// the boe control code uses a polling loop that blocks a thread
	// The ActivationSettings attribute with Sharing == false makes the runtime dedicate a dispatcher thread pool just for this service.
    // ExecutionUnitsPerDispatcher	- Indicates the number of execution units allocated to the dispatcher
    // ShareDispatcher	            - Inidicates whether multiple service instances can be pooled or not
	[ActivationSettings(ShareDispatcher = false, ExecutionUnitsPerDispatcher = 2)]
	[Contract(Contract.Identifier)]
    [DisplayName("(User) TrackRoamer Power Brick")]
	[Description("Provides access to the TrackRoamer Power Brick")]
    public class TrackRoamerBrickPowerService : DsspServiceBase
    {
        private const string _configFile = ServicePaths.Store + "/TrackRoamer.PowerBrick.config.xml";
        private const int DEFAULT_COM_PORT_NUMBER = 7;  // normally 1

        [InitialStatePartner(Optional = true, ServiceUri = _configFile)]
        private TrackRoamerBrickPowerState _state = new TrackRoamerBrickPowerState();

		[EmbeddedResource("TrackRoamer.Robotics.Services.TrackRoamerBrickPower.TrackRoamerBrickPower.xslt")]
		string _transform = null;

		[ServicePort("/trackroamerbrickpower", AllowMultipleInstances=false)]
        private TrackRoamerBrickPowerOperations _mainPort = new TrackRoamerBrickPowerOperations();

		[Partner("SubMgr", Contract = submgr.Contract.Identifier, CreationPolicy = PartnerCreationPolicy.CreateAlways)]
		private submgr.SubscriptionManagerPort _subMgrPort = new submgr.SubscriptionManagerPort();

        private Port<DateTime> _controlPort = new Port<DateTime>();

        /// <summary>
        /// A CCR port for receiving RoboteQ AX2850 data
        /// </summary>
        private RbtqDataPort _rbtqDataPort = new RbtqDataPort();

        /// <summary>
        /// Communicate with the RoboteQ AX2850 hardware
        /// </summary>
        private BrickConnection _brickConnection = null;

		private Dictionary<Guid, Port<UpdateMotorSpeed>> _coordinationList = new Dictionary<Guid, Port<UpdateMotorSpeed>>();

        private DateTime _initializationTime;


        public TrackRoamerBrickPowerService(DsspServiceCreationPort creationPort) : 
                base(creationPort)
        {
            LogInfo("TrackRoamerBrickPowerService:TrackRoamerBrickPowerService() -- port: " + creationPort.ToString());

            _initializationTime = DateTime.Now;
        }

        /// <summary>
        /// used by Hardware Controller to log all trace messages into a common stream, which is a property of DsspServiceBase.
        /// </summary>
        /// <param name="str"></param>
        public void LogInfoViaService(string str)
        {
            LogInfo(str);
        }

		protected override void Start()
        {
            LogInfo("TrackRoamerBrickPowerService:Start() _state=" + _state);

            if (_state == null)
            {
                // we usually don't come here, as the state is already configured by file - TrackRoamer.TrackRoamerBrickPower.Bot.Config.xml

                LogInfo("TrackRoamerBrickPowerService:Start(): _state == null - initializing...");

                _state = new TrackRoamerBrickPowerState();
                _state.PowerControllerConfig = new PowerControllerConfig();
                _state.PowerControllerConfig.CommPort = DEFAULT_COM_PORT_NUMBER;
                _state.Connected = false;
                _state.FrameCounter = 0;
                _state.Whiskers = new Whiskers();
                _state.MotorSpeed = new MotorSpeed();
                _state.MotorEncoder = new MotorEncoder();
                _state.MotorEncoderSpeed = new MotorEncoderSpeed();
                _state.TimeStamp = DateTime.Now;

                SaveState(_state);
            }
            else
            {
                LogInfo("TrackRoamerBrickPowerService:Start(): _state is supplied by file: " + _configFile);
            }

            if (_state.PowerControllerConfig == null)
            {
                _state.PowerControllerConfig = new PowerControllerConfig();
            }

            _brickConnection = new BrickConnection(_state.PowerControllerConfig, _rbtqDataPort, this);

            // wireup all event handlers to receive AX2850 data:
            _brickConnection.onValueReceived_EncoderLeftAbsolute += new OnValueReceived(_brickConnection_onValueReceived_EncoderLeftAbsolute);
            _brickConnection.onValueReceived_EncoderRightAbsolute += new OnValueReceived(_brickConnection_onValueReceived_EncoderRightAbsolute);

            _brickConnection.onValueReceived_EncoderSpeed += new OnValueReceived(_brickConnection_onValueReceived_EncoderSpeed);

            _brickConnection.onValueReceived_WhiskerLeft += new OnValueReceived(onWhiskerLeft);
            _brickConnection.onValueReceived_WhiskerRight += new OnValueReceived(onWhiskerRight);

            _brickConnection.onValueReceived_AnalogInputs += new OnValueReceived(_brickConnection_onValueReceived_AnalogInputs);
            _brickConnection.onValueReceived_DigitalInputE += new OnValueReceived(_brickConnection_onValueReceived_DigitalInputE);
            _brickConnection.onValueReceived_Voltage += new OnValueReceived(_brickConnection_onValueReceived_Voltage);
            _brickConnection.onValueReceived_MotorPower += new OnValueReceived(_brickConnection_onValueReceived_MotorPower);
            _brickConnection.onValueReceived_MotorAmps += new OnValueReceived(_brickConnection_onValueReceived_MotorAmps);
            _brickConnection.onValueReceived_HeatsinkTemperature += new OnValueReceived(_brickConnection_onValueReceived_HeatsinkTemperature);

            base.Start();   // start MainPortInterleave; wireup [ServiceHandler] methods

            SpawnIterator(ConnectToPowerController);

            MainPortInterleave.CombineWith(
            Arbiter.Interleave(
                new TeardownReceiverGroup(),
                new ExclusiveReceiverGroup(
                                        Arbiter.ReceiveWithIterator(true, _controlPort, ControlLoop),
                                        Arbiter.Receive<Exception>(true, _rbtqDataPort, ExceptionHandler),
                                        Arbiter.Receive<string>(true, _rbtqDataPort, MessageHandler)
                                        ),
                new ConcurrentReceiverGroup()));

            // kick off control loop interval:
            _controlPort.Post(DateTime.UtcNow);
            lastRateSnapshot = DateTime.Now.AddSeconds(frameRateWatchdogDelaySec);     // delay frame rate watchdog
            LogInfo("OK: TrackRoamerBrickPowerService:ControlLoop activated");

			// display HTTP service Uri
            LogInfo("TrackRoamerBrickPowerService: Service uri: ");
		}

        #region Handlers for other measured values coming from the controller

        void _brickConnection_onValueReceived_AnalogInputs(object sender, MeasuredValuesEventArgs e)
        {
            _state.PowerControllerState.Analog_Input_1 = e.value1;
            _state.PowerControllerState.Analog_Input_2 = e.value2;
            _state.TimeStamp = DateTime.Now;
        }

        void _brickConnection_onValueReceived_DigitalInputE(object sender, MeasuredValuesEventArgs e)
        {
            _state.PowerControllerState.Digital_Input_E = e.value1;
            _state.TimeStamp = DateTime.Now;
        }

        void _brickConnection_onValueReceived_Voltage(object sender, MeasuredValuesEventArgs e)
        {
            // the voltages come in as hex bytes 0...255, and then converted by formula:
            //      Measured Main Battery Volts = 55 * Read Value / 256
            //      Measured Internal Volts = 28.5 * Read Value / 256
            // there isn't much precision here, so rounding it to 2 digits seems adequate.
            _state.PowerControllerState.Main_Battery_Voltage = e.value1.HasValue ? Math.Round(e.value1.Value, 2) : (double?)null;
            _state.PowerControllerState.Internal_Voltage = e.value2.HasValue ? Math.Round(e.value2.Value, 2) : (double?)null;
            _state.TimeStamp = DateTime.Now;
        }

        void _brickConnection_onValueReceived_MotorPower(object sender, MeasuredValuesEventArgs e)
        {
            _state.PowerControllerState.Motor_Power_Left = e.value1;
            _state.PowerControllerState.Motor_Power_Right = e.value2;
            _state.TimeStamp = DateTime.Now;
        }

        void _brickConnection_onValueReceived_MotorAmps(object sender, MeasuredValuesEventArgs e)
        {
            // note: Amps behave almost like integers, no precision here and low current will read as 0
            _state.PowerControllerState.Motor_Amps_Left = e.value1;
            _state.PowerControllerState.Motor_Amps_Right = e.value2;
            _state.TimeStamp = DateTime.Now;
        }

        void _brickConnection_onValueReceived_HeatsinkTemperature(object sender, MeasuredValuesEventArgs e)
        {
            _state.PowerControllerState.Heatsink_Temperature_Left = e.value1;
            _state.PowerControllerState.Heatsink_Temperature_Right = e.value2;
            _state.TimeStamp = DateTime.Now;
        }

        #endregion // Handlers for other measured values coming from the controller

        /// <summary>
        /// Handle Errors
        /// </summary>
        /// <param name="ex"></param>
        private void ExceptionHandler(Exception ex)
        {
            LogError(ex.Message);
        }

        /// <summary>
        /// Handle messages
        /// </summary>
        /// <param name="message"></param>
        private void MessageHandler(string message)
        {
            LogInfo(message);
        }


        #region Power Controller Helpers

        /// <summary>
        /// An iterator to connect to a Power Controller.
        /// If no configuration exists, search for the connection.
        /// </summary>
        private IEnumerator<ITask> ConnectToPowerController()
        {
            try
            {
                _state.Connected = false;

                if (_state.PowerControllerConfig.CommPort != 0 && _state.PowerControllerConfig.BaudRate != 0)
                {
                    _state.PowerControllerConfig.ConfigurationStatus = "Opening RoboteQ AX2850 on Port " + _state.PowerControllerConfig.CommPort.ToString();
                    _state.Connected = _brickConnection.Open(_state.PowerControllerConfig.CommPort, _state.PowerControllerConfig.BaudRate);
                }
                else
                {
                    _state.PowerControllerConfig.ConfigurationStatus = "Searching for the RoboteQ AX2850 COM Port";
                    _state.Connected = _brickConnection.FindPowerController();
                    if (_state.Connected)
                    {
                        _state.PowerControllerConfig = _brickConnection.PowerControllerConfig;
                        _state.TimeStamp = DateTime.Now;
                        SaveState(_state);
                    }
                }
            }
            catch (UnauthorizedAccessException ex)
            {
                LogError(ex);
            }
            catch (IOException ex)
            {
                LogError(ex);
            }
            catch (ArgumentOutOfRangeException ex)
            {
                LogError(ex);
            }
            catch (ArgumentException ex)
            {
                LogError(ex);
            }
            catch (InvalidOperationException ex)
            {
                LogError(ex);
            }

            if (!_state.Connected)
            {
                _state.PowerControllerConfig.ConfigurationStatus = "Not Connected";
                LogInfo(LogGroups.Console, "The RoboteQ AX2850 is not detected.\r\n*   To configure the RoboteQ AX2850, navigate to: ");
            }
            else
            {
                _state.PowerControllerConfig.ConfigurationStatus = "Connected";
            }

            _state.TimeStamp = DateTime.Now;

            yield break;
        }

        #endregion // Power Controller Helpers

        #region Event handlers - activated by controller, generate Notifications (updates - e.g. whiskers and Encoders)

        void _brickConnection_onValueReceived_EncoderSpeed(object sender, MeasuredValuesEventArgs ev)
		{
#if TRACEDEBUGTICKS
            LogInfo("TrackRoamerBrickPowerService : received EncoderSpeed : left=" + ev.value1 + "   right=" + ev.value2);
#endif // TRACEDEBUGTICKS

            UpdateMotorEncoderSpeed ume = new UpdateMotorEncoderSpeed();
			ume.Body.Timestamp = new DateTime(ev.timestamp);
			ume.Body.LeftSpeed = ev.value1;
			ume.Body.RightSpeed = ev.value2;

            _state.MotorEncoderSpeed.LeftSpeed = ume.Body.LeftSpeed;
            _state.MotorEncoderSpeed.RightSpeed = ume.Body.RightSpeed;
            _state.MotorEncoderSpeed.Timestamp = ume.Body.Timestamp;
            _state.TimeStamp = DateTime.Now;

            base.SendNotification<UpdateMotorEncoderSpeed>(_subMgrPort, ume);
		}

		private void _brickConnection_onValueReceived_EncoderLeftAbsolute(object sender, MeasuredValuesEventArgs ev)
		{
#if TRACEDEBUGTICKS
			LogInfo("TrackRoamerBrickPowerService : received EncoderLeftAbsolute : " + ev.value1);
#endif // TRACEDEBUGTICKS

            UpdateMotorEncoder ume = new UpdateMotorEncoder();
			ume.Body.Timestamp = new DateTime(ev.timestamp);
			ume.Body.LeftDistance = ev.value1;
			ume.Body.HardwareIdentifier = 1;    // 1 = Left

            _state.MotorEncoder.LeftDistance = ume.Body.LeftDistance;
            _state.MotorEncoder.Timestamp = ume.Body.Timestamp;
            _state.TimeStamp = DateTime.Now;

			base.SendNotification <UpdateMotorEncoder>(_subMgrPort, ume);
		}

		private void _brickConnection_onValueReceived_EncoderRightAbsolute(object sender, MeasuredValuesEventArgs ev)
		{
#if TRACEDEBUGTICKS
            LogInfo("TrackRoamerBrickPowerService : received EncoderRightAbsolute : " + ev.value1);
#endif // TRACEDEBUGTICKS

            UpdateMotorEncoder ume = new UpdateMotorEncoder();
			ume.Body.Timestamp = new DateTime(ev.timestamp);
			ume.Body.RightDistance = ev.value1;
			ume.Body.HardwareIdentifier = 2;    // 2 = Right

            _state.MotorEncoder.RightDistance = ume.Body.RightDistance;
            _state.MotorEncoder.Timestamp = ume.Body.Timestamp;
            _state.TimeStamp = DateTime.Now;

			base.SendNotification <UpdateMotorEncoder>(_subMgrPort, ume);
		}

        /// <summary>
        /// for bumper-type situations, we need to stop movement immediately, and then we can
        /// let the events propagate to the higher level behavior, to cause backup movement for example.
        /// </summary>
        private void stopMotorsNow()
        {
            // If we are connected, send the speed to the robot
            if (ensureHardwareController())
            {
                _brickConnection.SetSpeed(0.0d, 0.0d);
            }
        }

		private void onWhiskerLeft(object sender, MeasuredValuesEventArgs ev)
		{
            LogInfo("TrackRoamerBrickPowerService : WhiskerLeft : " + ev.value1);

            if (ev.value1 > 0 && (!_state.Whiskers.FrontWhiskerLeft.HasValue || !_state.Whiskers.FrontWhiskerLeft.Value))
            {
                // if this is a "whisker pressed" event, do emergency stop:
                stopMotorsNow();
                // Note: UpdateMotorSpeedHandler() will not set positive speed if whiskers are pressed.
            }

            _state.Whiskers.FrontWhiskerLeft = ev.value1 > 0;
            _state.Whiskers.Timestamp = new DateTime(ev.timestamp);
            _state.TimeStamp = DateTime.Now;

			UpdateWhiskers uw = new UpdateWhiskers();
			uw.Body.Timestamp = _state.Whiskers.Timestamp;
			uw.Body.FrontWhiskerLeft = ev.value1 > 0;

			base.SendNotification<UpdateWhiskers>(_subMgrPort, uw);
		}

		private void onWhiskerRight(object sender, MeasuredValuesEventArgs ev)
		{
            LogInfo("TrackRoamerBrickPowerService : WhiskerRight : " + ev.value1);

            if (ev.value1 > 0 && (!_state.Whiskers.FrontWhiskerRight.HasValue || !_state.Whiskers.FrontWhiskerRight.Value))
            {
                // if this is a "whisker pressed" event, do emergency stop.
                stopMotorsNow();
                // Note: UpdateMotorSpeedHandler() will not set positive speed if whiskers are pressed.
            }

            _state.Whiskers.Timestamp = new DateTime(ev.timestamp);
            _state.Whiskers.FrontWhiskerRight = ev.value1 > 0;
            _state.TimeStamp = DateTime.Now;

            UpdateWhiskers uw = new UpdateWhiskers();
			uw.Body.Timestamp = _state.Whiskers.Timestamp;
			uw.Body.FrontWhiskerRight = ev.value1 > 0;

			base.SendNotification<UpdateWhiskers>(_subMgrPort, uw);
		}

        #endregion // Event handlers - activated by controller, generate Notifications (updates - e.g. whiskers and Encoders)

        #region ControlLoop()

        private const int rateCountIntervalSec = 5;
		private long frameCounterLast = 0;
		private long errorCounterLast = 0;
        private const double frameRateWatchdogDelaySec = 10.0d;
        private DateTime lastRateSnapshot = DateTime.Now.AddSeconds(frameRateWatchdogDelaySec);    // frame rate watchdog
        double delay;

        private double ElapsedSecondsSinceStart { get { return (DateTime.Now - _initializationTime).TotalSeconds; } }

        private IEnumerator<ITask> ControlLoop(DateTime timestamp)
        {
            // we come here real often (10ms)
            //LogInfo("TrackRoamerBrickPowerService:ControlLoop()");

            double startTime = ElapsedSecondsSinceStart;
            delay = _state.PowerControllerConfig.Delay / 1000.0d;    // sampling interval in seconds 

            try
            {
                DateTime now = DateTime.Now;

                if (!ensureHardwareController())
                {
                    // LogError(LogGroups.Console, "failed attempt to connect to the Power Brick controller");
                    _state.ConnectAttempts++;
                    _state.TimeStamp = now;

                    //yield break;
                }
                else
                {
                    _state.ConnectAttempts = 0;
                    _state.TimeStamp = now;
                }

                if (!_brickConnection.HcConnected && _state.ConnectAttempts > 3)
                {
                    // this is bad - complain and wait, we can't connect to AX2850
                    delay = 2.0d; // seconds

                    Close(true);
                    lastRateSnapshot = DateTime.Now.AddSeconds(frameRateWatchdogDelaySec);     // delay frame rate watchdog

                    LogError(LogGroups.Console, "Can not connect to the Power Brick controller");
                }
                else
                {
                    // normal operation - call controller loop and update state, then come back in 20ms
                    if (!_state.Dropping)
                    {
                        _brickConnection.ExecuteMain();      // this is where controller work is done, call it often
                    }

                    _state.Connected = _brickConnection.HcConnected;

                    long frameCounter = _brickConnection.frameCounter;
                    long errorCounter = _brickConnection.errorCounter;

                    _state.FrameCounter = frameCounter;
                    _state.ErrorCounter = errorCounter;
                    _state.TimeStamp = now;

                    if (now > lastRateSnapshot.AddSeconds(rateCountIntervalSec))
                    {
                        _state.FrameRate = (int)((frameCounter - this.frameCounterLast) / rateCountIntervalSec);
                        this.frameCounterLast = frameCounter;

                        _state.ErrorRate = (int)((errorCounter - this.errorCounterLast) / rateCountIntervalSec);
                        this.errorCounterLast = errorCounter;

                        _state.TimeStamp = now;
                        lastRateSnapshot = now;

                        // frame rate watchdog:
                        if (_state.FrameRate == 0)      // should be around 80; if 0 - something is terribly wrong.
                        {
                            LogError(LogGroups.Console, "Frame rate watchdog: FrameRate is 0 - resetting Power Brick controller");
                            Close(true);
                            lastRateSnapshot = now.AddSeconds(frameRateWatchdogDelaySec);     // delay frame rate watchdog
                        }
                    }
                }
            }
            finally
            {
                _state.PowerControllerConfig.ConfigurationStatus = _brickConnection.StatusLabel;

                // all times here are in seconds:
                double endTime = ElapsedSecondsSinceStart;
                double elapsed = endTime - startTime;
                double remainderToNextSamplingTime = delay - elapsed;
                if (remainderToNextSamplingTime <= 0.005d)
                {
                    // schedule immediately
                    remainderToNextSamplingTime = 0.005d;    // 5ms
                }

                if (this.ServicePhase == ServiceRuntimePhase.Started)
                {
                    if(remainderToNextSamplingTime > 1.0d)
                    {
                        LogInfo("TrackRoamerBrickPowerService:ControlLoop() - sleeping seconds=" + remainderToNextSamplingTime);
                    }

                    // schedule next sampling interval
                    Activate(this.TimeoutPort(
                        TimeSpan.FromSeconds(remainderToNextSamplingTime)).Receive(
                                (dt) =>  _controlPort.Post(dt)		// call ControlLoop again
                        )
                      );
                }
            }
            yield break;
        }

        #endregion // ControlLoop()

        #region Operation Port Handlers - Get, HttpGet, UpdateMotorSpeed etc.

        [ServiceHandler(ServiceHandlerBehavior.Concurrent)]
        public virtual IEnumerator<ITask> GetHandler(Get get)
        {
            get.ResponsePort.Post(_state);
            yield break;
		}

		[ServiceHandler(ServiceHandlerBehavior.Concurrent)]
		public virtual IEnumerator<ITask> HttpGetHandler(HttpGet httpGet)
		{
			httpGet.ResponsePort.Post(new HttpResponseType(
				HttpStatusCode.OK,
				_state,
				_transform)
			);
			yield break;
		}

		/// <summary>
		/// Replace Handler
		/// </summary>
		/// <param name="replace"></param>
		/// <returns></returns>
		[ServiceHandler(ServiceHandlerBehavior.Exclusive)]
		public virtual IEnumerator<ITask> ReplaceHandler(Replace replace)
		{
            LogInfo("TrackRoamerBrickPowerService:ReplaceHandler()");

			Close();
			
            _state = replace.Body;
			_state.Connected = false;
			_state.Connected = ReConnect();
            _state.TimeStamp = DateTime.Now;
            
            replace.ResponsePort.Post(DefaultReplaceResponseType.Instance);
			yield break;
		}

		/// <summary>
		/// Subscribe Handler
		/// </summary>
		/// <param name="subscribe"></param>
		/// <returns></returns>
		[ServiceHandler(ServiceHandlerBehavior.Exclusive)]
		public virtual IEnumerator<ITask> SubscribeHandler(Subscribe subscribe)
		{
			SubscribeRequestType request = subscribe.Body;

            LogInfo("TrackRoamerBrickPowerService:SubscribeHandler() -- Subscribe request from: " + request.Subscriber);

			yield return Arbiter.Choice(
				SubscribeHelper(_subMgrPort, request, subscribe.ResponsePort),
				delegate(SuccessResult success) { },
				delegate(Exception ex)
				{
					LogError(ex);
					throw ex;
				}
			);

			//_subMgrPort.Post(new submgr.Submit(request.Subscriber, DsspActions.UpdateRequest, _state.MotorSpeed, null));
            //_subMgrPort.Post(new submgr.Submit(request.Subscriber, DsspActions.UpdateRequest, _state.Whiskers, null));

			yield break;
		}

        /*
		/// <summary>
		/// QueryConfig Handler
		/// </summary>
		/// <param name="query"></param>
		/// <returns></returns>
		[ServiceHandler(ServiceHandlerBehavior.Concurrent)]
		public virtual IEnumerator<ITask> QueryConfigHandler(QueryConfig query)
		{
			LogInfo("TrackRoamerBrickPowerService:QueryConfigHandler()");

            query.ResponsePort.Post(_state.PowerControllerConfig);
			yield break;
		}
        */

        /// <summary>
        /// QueryWhiskers Handler
		/// </summary>
		/// <param name="query"></param>
		/// <returns></returns>
		[ServiceHandler(ServiceHandlerBehavior.Concurrent)]
        public virtual IEnumerator<ITask> QueryWhiskersHandler(QueryWhiskers query)
		{
			query.ResponsePort.Post(_state.Whiskers);
			yield break;
		}

		/// <summary>
		/// QueryMotorSpeed Handler
		/// </summary>
		/// <param name="query"></param>
		/// <returns></returns>
		[ServiceHandler(ServiceHandlerBehavior.Concurrent)]
		public virtual IEnumerator<ITask> QueryMotorSpeedHandler(QueryMotorSpeed query)
		{
            LogInfo("TrackRoamerBrickPowerService:QueryMotorSpeedHandler()");

			query.ResponsePort.Post(_state.MotorSpeed);
			yield break;
		}

		/// <summary>
		/// UpdateConfig Handler
		/// </summary>
		/// <param name="update"></param>
		/// <returns></returns>
		[ServiceHandler(ServiceHandlerBehavior.Exclusive)]
		public virtual IEnumerator<ITask> UpdateConfigHandler(UpdateConfig update)
		{
            LogInfo("TrackRoamerBrickPowerService:UpdateConfigHandler()");

			if (update.Body.CommPort >= 0)
			{
                _state.PowerControllerConfig.CommPort = update.Body.CommPort;
                LogInfo("    SerialPort=" + _state.PowerControllerConfig.CommPort);
				_state.Connected = ReConnect();
			}

			update.ResponsePort.Post(DefaultUpdateResponseType.Instance);
			yield break;
		}

		public void SetCoordinatedMotors(UpdateMotorSpeed[] motors)
		{
            //LogInfo("TrackRoamerBrickPowerService:SetCoordinatedMotors()");

            MotorSpeed motorSpeed = new MotorSpeed() { Timestamp = DateTime.Now };

			// Default null which is ignored by the controller.
			motorSpeed.LeftSpeed = null;
			motorSpeed.RightSpeed = null;

			// Combine the motor commands
			foreach (UpdateMotorSpeed ms in motors)
			{
				if (ms.Body.LeftSpeed != null && ms.Body.LeftSpeed >= -1.0 && ms.Body.LeftSpeed <= 1.0)
					motorSpeed.LeftSpeed = ms.Body.LeftSpeed;

				if (ms.Body.RightSpeed != null && ms.Body.RightSpeed >= -1.0 && ms.Body.RightSpeed <= 1.0)
					motorSpeed.RightSpeed = ms.Body.RightSpeed;
			}

			// Send a singe command to the controller:
            UpdateMotorSpeed combinedRequest = new UpdateMotorSpeed(motorSpeed);
			_mainPort.Post(combinedRequest);
			Activate(Arbiter.Choice(combinedRequest.ResponsePort,
				delegate(DefaultUpdateResponseType response)
				{
					// send responses back to the original motors
					foreach (UpdateMotorSpeed ms in motors)
						ms.ResponsePort.Post(DefaultUpdateResponseType.Instance);
				},
				delegate(Fault fault)
				{
					// send failure back to the original motors
					foreach (UpdateMotorSpeed ms in motors)
						ms.ResponsePort.Post(fault);

				}));

		}

		/// <summary>
		/// UpdateMotorSpeed Handler
		/// </summary>
		/// <param name="update"></param>
		/// <returns></returns>
		[ServiceHandler(ServiceHandlerBehavior.Exclusive)]
		public virtual IEnumerator<ITask> UpdateMotorSpeedHandler(UpdateMotorSpeed update)
		{
            //LogInfo("TrackRoamerBrickPowerService:UpdateMotorSpeedHandler()");

			coord.ActuatorCoordination coordination = update.GetHeader<coord.ActuatorCoordination>();
            if (coordination != null && coordination.Count > 1)
			{
				if (!_coordinationList.ContainsKey(coordination.RequestId))
				{
					_coordinationList.Add(coordination.RequestId, new Port<UpdateMotorSpeed>());
					Activate(
						Arbiter.MultipleItemReceive<UpdateMotorSpeed>(
							false,
							_coordinationList[coordination.RequestId],
							coordination.Count,
							SetCoordinatedMotors));
				}
				_coordinationList[coordination.RequestId].Post(update);
				yield break;
			}

			bool changed = ((update.Body.LeftSpeed >= 0 && _state.MotorSpeed.LeftSpeed != update.Body.LeftSpeed)
                				|| (update.Body.RightSpeed >= 0 && _state.MotorSpeed.RightSpeed != update.Body.RightSpeed));

			if (update.Body.LeftSpeed != null)
			{
				if (update.Body.LeftSpeed >= -1.0 && update.Body.LeftSpeed <= 1.0)
				{
                    //LogInfo("TrackRoamerBrickPowerService:UpdateMotorSpeedHandler - LeftSpeed=" + update.Body.LeftSpeed + " requested");
					_state.MotorSpeed.LeftSpeed = update.Body.LeftSpeed;
                }
				else
				{
                    LogInfo("Error: TrackRoamerBrickPowerService:UpdateMotorSpeedHandler - invalid LeftSpeed=" + update.Body.LeftSpeed + " requested, must be between -1.0 and +1.0");
				}
			}

			if (update.Body.RightSpeed != null)
			{
				if (update.Body.RightSpeed >= -1.0 && update.Body.RightSpeed <= 1.0)
				{
                    //LogInfo("TrackRoamerBrickPowerService:UpdateMotorSpeedHandler - RightSpeed=" + update.Body.RightSpeed + " requested");
					_state.MotorSpeed.RightSpeed = update.Body.RightSpeed;
                }
				else
				{
                    LogInfo("Error: TrackRoamerBrickPowerService:UpdateMotorSpeedHandler - invalid RightSpeed=" + update.Body.RightSpeed + " requested, must be between -1.0 and +1.0");
				}
			}

			// If we are connected, send the speed to the robot wheels controller:
			if (ensureHardwareController())
			{
                double? leftSpeed = _state.MotorSpeed.LeftSpeed;
                double? rightSpeed = _state.MotorSpeed.RightSpeed;

                // it will take time for upper layers to react on whiskers. We want to have some protection here, to avoid damage.
                // cannot move forward if whiskers are pressed; replace it with backwards movement at half speed though:
                if (leftSpeed  > 0 && _state.Whiskers.FrontWhiskerLeft.GetValueOrDefault())
                {
                    Tracer.Trace("Warning: TrackRoamerBrickPowerService:UpdateMotorSpeedHandler - left whisker pressed, speed " + leftSpeed + " reversed");
                    leftSpeed  = -leftSpeed / 2;
                }

                if (rightSpeed > 0 && _state.Whiskers.FrontWhiskerRight.GetValueOrDefault())
                {
                    Tracer.Trace("Warning: TrackRoamerBrickPowerService:UpdateMotorSpeedHandler - right whisker pressed, speed " + rightSpeed + " reversed");
                    rightSpeed = -rightSpeed / 2;
                }

                _brickConnection.SetSpeed(leftSpeed, rightSpeed);
			}

			// Send Notifications to subscribers
			if (changed)
			{
				_subMgrPort.Post(new submgr.Submit(_state.MotorSpeed, DsspActions.UpdateRequest));
			}

            _state.TimeStamp = _state.MotorSpeed.Timestamp = DateTime.Now;

			update.ResponsePort.Post(DefaultUpdateResponseType.Instance);
			yield break;
		}

        /// <summary>
        /// SetOutputC Handler
		/// </summary>
		/// <param name="update"></param>
		/// <returns></returns>
        [ServiceHandler(ServiceHandlerBehavior.Exclusive)]
        public virtual IEnumerator<ITask> SetOutputCHandler(SetOutputC update)
        {
            // If we are connected, send the speed to the robot
            if (ensureHardwareController())
            {
                _brickConnection.SetOutputC(update.Body);
                _state.TimeStamp = DateTime.Now;
                _state.OutputC = update.Body ? 1 : 0;
            }

            // Send Notifications to subscribers
            _subMgrPort.Post(new submgr.Submit(update, DsspActions.UpdateRequest));

            update.ResponsePort.Post(DefaultUpdateResponseType.Instance);
            yield break;
        }

        /*
		/// <summary>
        /// UpdateWhiskers Handler
		/// </summary>
		/// <param name="update"></param>
		/// <returns></returns>
		[ServiceHandler(ServiceHandlerBehavior.Exclusive)]
        public virtual IEnumerator<ITask> UpdateWhiskersHandler(UpdateWhiskers update)
		{
			_state.Whiskers.FrontWhiskerLeft = update.Body.FrontWhiskerLeft;
			_state.Whiskers.FrontWhiskerRight = update.Body.FrontWhiskerRight;
			_state.Whiskers.Timestamp = update.Body.Timestamp;
            _state.TimeStamp = DateTime.Now;

			// Send Notifications to subscribers
			_subMgrPort.Post(new submgr.Submit(_state.Whiskers, DsspActions.UpdateRequest));

			update.ResponsePort.Post(DefaultUpdateResponseType.Instance);
			yield break;
		}

		/// <summary>
		/// UpdateMotorEncoder Handler
		/// </summary>
		/// <param name="update"></param>
		/// <returns></returns>
		[ServiceHandler(ServiceHandlerBehavior.Exclusive)]
		public virtual IEnumerator<ITask> UpdateEncoderHandler(UpdateMotorEncoder update)
		{
			//_state.MotorEncoder.HardwareIdentifier = update.Body.HardwareIdentifier;
			_state.MotorEncoder.LeftDistance = update.Body.LeftDistance;
			_state.MotorEncoder.RightDistance = update.Body.RightDistance;
			_state.MotorEncoder.Timestamp = update.Body.Timestamp;

			// Send Notifications to subscribers
			_subMgrPort.Post(new submgr.Submit(_state.MotorEncoder, DsspActions.UpdateRequest));

			update.ResponsePort.Post(DefaultUpdateResponseType.Instance);
			yield break;
		}

		/// <summary>
        /// UpdateMotorEncoderSpeed Handler
		/// </summary>
		/// <param name="update"></param>
		/// <returns></returns>
		[ServiceHandler(ServiceHandlerBehavior.Exclusive)]
        public virtual IEnumerator<ITask> UpdateEncoderSpeedHandler(UpdateMotorEncoderSpeed update)
		{
            _state.MotorEncoderSpeed.LeftSpeed = update.Body.LeftSpeed;
            _state.MotorEncoderSpeed.RightSpeed = update.Body.RightSpeed;
			_state.MotorEncoder.Timestamp = update.Body.Timestamp;

			// Send Notifications to subscribers
            _subMgrPort.Post(new submgr.Submit(_state.MotorEncoderSpeed, DsspActions.UpdateRequest));

			update.ResponsePort.Post(DefaultUpdateResponseType.Instance);
			yield break;
		}
        */

		/// <summary>
		/// ResetMotorEncoder Handler
		/// </summary>
		/// <param name="update"></param>
		/// <returns></returns>
		[ServiceHandler(ServiceHandlerBehavior.Exclusive)]
		public virtual IEnumerator<ITask> ResetEncoderHandler(ResetMotorEncoder reset)
		{
            LogInfo("TrackRoamerBrickPowerService:ResetEncoderHandler() id=" + reset.Body.HardwareIdentifier);

			switch(reset.Body.HardwareIdentifier)
			{
				case 1:
                    _brickConnection.ResetEncoderLeft();
                    _state.MotorEncoder.LeftDistance = 0.0d;
                    _state.TimeStamp = DateTime.Now;
                    break;

				case 2:
                    _brickConnection.ResetEncoderRight();
                    _state.MotorEncoder.RightDistance = 0.0d;
                    _state.TimeStamp = DateTime.Now;
                    break;
			}

			// Send Notifications to subscribers
			_subMgrPort.Post(new submgr.Submit(_state.MotorEncoder, DsspActions.ReplaceRequest));

			reset.ResponsePort.Post(DefaultReplaceResponseType.Instance);
			yield break;
		}

		[ServiceHandler(ServiceHandlerBehavior.Teardown)]
        public virtual IEnumerator<ITask> DropHandler(DsspDefaultDrop drop)
		{
            LogInfo("TrackRoamerBrickPowerService:DropHandler()");

            _state.Dropping = true;     // makes sure the command loop is not calling controller

            _brickConnection.Close();
			base.DefaultDropHandler(drop);

			yield break;
		}

		#endregion // Operation Port Handlers

		#region Hardware Controller Connections
		/// <summary>
		/// Connect and update bot hardware controller with current speed settings
		/// </summary>
		private bool ReConnect()
		{
            LogInfo("TrackRoamerBrickPowerService:ReConnect()");
            Tracer.Trace("TrackRoamerBrickPowerService:ReConnect()");

			Close(true);
			bool connected = ensureHardwareController();
			if (connected)
			{
                if (_state.MotorSpeed != null
                    && _state.MotorSpeed.LeftSpeed.HasValue && _state.MotorSpeed.RightSpeed.HasValue
                    && _state.MotorSpeed.LeftSpeed != 0.0d || _state.MotorSpeed.RightSpeed != 0.0d)
                {
                    _brickConnection.SetSpeed(_state.MotorSpeed.LeftSpeed, _state.MotorSpeed.RightSpeed);
                }
                else
                {
                    _state.MotorSpeed.LeftSpeed = 0.0d;
                    _state.MotorSpeed.RightSpeed = 0.0d;
                    _state.MotorSpeed.Timestamp = DateTime.Now;
                    _brickConnection.SetSpeed(0.0d, 0.0d);  // if in doubt, stop
                }
			}
			return connected;
		}

		/// <summary>
		/// Close the underlying connection to the Hardware Controller.
		/// <remarks>Modifies _state</remarks>
		/// </summary>
		private void Close(bool forceIt = false)
		{
            LogInfo("TrackRoamerBrickPowerService:Close()");

            if (_state.Connected || forceIt)
			{
                _brickConnection.Close();
				_state.Connected = false;
                _state.TimeStamp = DateTime.Now;
            }
		}

        private bool? lastAnnouncedControllerState = null; 

		/// <summary>
        /// Connect to the underlying hardware controller (RoboteQ). Set _state.Connected accordingly.
		/// <remarks>Modifies _state</remarks>
		/// </summary>
        /// <returns>true if controller is online ready for commands. Same as _state.Connected</returns>
		private bool ensureHardwareController()
		{
            _state.Connected = _brickConnection.HcConnected;

            if (!_state.Connected)
            {
                //LogInfo("TrackRoamerBrickPowerService:ConnectToHardwareController() - connecting...");

                string errorMessage = string.Empty;

                _state.Connected = _brickConnection.ReConnect(out errorMessage);
                _state.TimeStamp = DateTime.Now;

                if (!_state.Connected)
                {
                    if (!string.IsNullOrEmpty(errorMessage))
                    {
                        LogError(LogGroups.Console, errorMessage);
                        Talker.Say(10, "cannot initialize Power Brick on COM" + _state.PowerControllerConfig.CommPort);
                    }
                    else
                    {
                        if (!lastAnnouncedControllerState.HasValue || lastAnnouncedControllerState.Value)
                        {
                            lastAnnouncedControllerState = false;
                            Talker.Say(10, "power brick offline");
                        }
                    }
                }
                else
                {
                    //_brickConnection.ResetEncoderLeft();
                    //_brickConnection.ResetEncoderRight();
                    _state.MotorEncoder.LeftDistance = 0.0d;
                    _state.MotorEncoder.RightDistance = 0.0d;
                    _state.MotorEncoderSpeed.LeftSpeed = 0.0d;
                    _state.MotorEncoderSpeed.RightSpeed = 0.0d;
                    _state.TimeStamp = _state.MotorEncoder.Timestamp = _state.MotorEncoderSpeed.Timestamp = DateTime.Now;

                    if (!lastAnnouncedControllerState.HasValue || !lastAnnouncedControllerState.Value)
                    {
                        lastAnnouncedControllerState = true;
                        Talker.Say(5, "power brick online");
                        delay = _state.PowerControllerConfig.Delay / 1000.0d;    // sampling interval in seconds 
                    }
                }
            }

			return _state.Connected;
		}

		#endregion // Hardware Controller Connections

#if TRACEDEBUG
        protected new void LogInfo(string str)
        {
            Tracer.Trace(str);
        }
        protected new void LogError(string str)
        {
            Tracer.Error(str);
        }
#endif // TRACEDEBUG
    }
}
