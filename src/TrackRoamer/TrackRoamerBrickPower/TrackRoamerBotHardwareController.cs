using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Xml;
using System.Net;

using Microsoft.Ccr.Core;
using Microsoft.Dss.Core;
using Microsoft.Dss.Core.Attributes;
using Microsoft.Dss.ServiceModel.Dssp;
using Microsoft.Dss.Core.DsspHttp;
using Microsoft.Dss.ServiceModel.DsspServiceBase;
using W3C.Soap;

using TrackRoamer.Robotics.Hardware.LibRoboteqController;

using trackroamerbot = TrackRoamer.Robotics.Services.TrackRoamerBot;

using System.IO.Ports;
using System.Threading;

namespace TrackRoamer.Robotics.Services.TrackRoamerBot
{
	enum CommandLoop { STOP = 0, START, ACTIVE, IDLE }

	internal class TrackRoamerBotHardwareController
	{
		TrackRoamerBotOperations _trbPort = null;
        TrackRoamerBotService _service = null;

		public event OnValueReceived onValueReceived_WhiskerLeft;
		public event OnValueReceived onValueReceived_WhiskerRight;

		public event OnValueReceived onValueReceived_EncoderLeftAbsolute;
		public event OnValueReceived onValueReceived_EncoderRightAbsolute;

		public event OnValueReceived onValueReceived_EncoderSpeed;

        public bool Connected { get; private set; }

		public bool Running { get; set; }

		public int Delay { get; set; }

        public bool isInError { get { return m_controller == null ? false : m_controller.isInError; } }

		internal long frameCounter = 0;
		internal long errorCounter = 0;

		#region Constructors
        public TrackRoamerBotHardwareController(TrackRoamerBotOperations trbPort, TrackRoamerBotService service)
        {
			service.LogInfoViaService("TrackRoamerBotHardwareController()");

            Connected = false;
            Running = false;
            Delay = 0;

			_trbPort = trbPort;
            _service = service;
        }

		~TrackRoamerBotHardwareController()
        {
            Close();
        }
        #endregion

		#region RoboteQ RQAX2850 related

		private ControllerRQAX2850 m_controller = null;
		private string sComPort = null;

		private void ensureController()
		{
            _service.LogInfoViaService("TrackRoamerBotHardwareController: ensureController()  sComPort=" + sComPort + "  m_controller=" + m_controller);

            if (m_controller == null)
			{
				m_controller = new ControllerRQAX2850(sComPort);

				m_controller.onValueReceived_EncoderLeftAbsolute += new OnValueReceived(m_controller_onValueReceived_EncoderLeftAbsolute);
				m_controller.onValueReceived_EncoderRightAbsolute += new OnValueReceived(m_controller_onValueReceived_EncoderRightAbsolute);

				m_controller.onValueReceived_EncoderSpeed += new OnValueReceived(m_controller_onValueReceived_EncoderSpeed);

				m_controller.onValueReceived_DigitalInputF += new OnValueReceived(m_controller_onValueReceived_DigitalInputF);
				m_controller.onValueReceived_DigitalInputEmerg += new OnValueReceived(m_controller_onValueReceived_DigitalInputEmerg);

				m_controller.init();
			}

            _service.LogInfoViaService("TrackRoamerBotHardwareController: ensureController() finished");
        }

		void m_controller_onValueReceived_DigitalInputEmerg(object sender, MeasuredValuesEventArgs ev)
		{
			if (onValueReceived_WhiskerRight != null)
			{
				onValueReceived_WhiskerRight(this, ev);
			}
		}

		void m_controller_onValueReceived_DigitalInputF(object sender, MeasuredValuesEventArgs ev)
		{
			if (onValueReceived_WhiskerLeft != null)
			{
				onValueReceived_WhiskerLeft(this, ev);
			}
		}

		void m_controller_onValueReceived_EncoderLeftAbsolute(object sender, MeasuredValuesEventArgs ev)
		{
			if (onValueReceived_EncoderLeftAbsolute != null)
			{
				onValueReceived_EncoderLeftAbsolute(this, ev);
			}
		}

		void m_controller_onValueReceived_EncoderRightAbsolute(object sender, MeasuredValuesEventArgs ev)
		{
			if (onValueReceived_EncoderRightAbsolute != null)
			{
				onValueReceived_EncoderRightAbsolute(this, ev);
			}
		}

		void m_controller_onValueReceived_EncoderSpeed(object sender, MeasuredValuesEventArgs ev)
		{
			if (onValueReceived_EncoderSpeed != null)
			{
				onValueReceived_EncoderSpeed(this, ev);
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
		}

		#endregion // RoboteQ RQAX2850 related

		public bool Connect(int serialPortNumber, out string errorMessage)
		{
            _service.LogInfoViaService("TrackRoamerBotHardwareController:Connect(serialPortNumber=" + serialPortNumber + ")  Connected=" + Connected);

			if (serialPortNumber <= 0)
			{
				errorMessage = "The TrackRoamerBot serial port is not configured!";
				return false;
			}

            if (Connected)
			{
                Close();  // also sets Connected = false;
            }

			try
			{
				sComPort = "COM" + serialPortNumber.ToString();
				ensureController();

				errorMessage = string.Empty;
                Connected = m_controller.isOnline;

                _service.LogInfoViaService((Connected ? "connected to " : "Warning: not connected to ") + sComPort);

                return Connected;
			}
			catch (Exception ex)
			{
				errorMessage = string.Format("Error connecting TrackRoamerBot to port {0}: {1}", serialPortNumber, ex.Message);
				return false;
			}
		}

		public void Close()
		{
            _service.LogInfoViaService("TrackRoamerBotHardwareController:Close()  Connected=" + Connected);

            if (!Connected)
				return;

            Connected = false;

            if (Running)
			{
                Running = false;
				Thread.Sleep(100);
			}

			disposeController();
		}

		public void Start()
		{
            if (!Running)
			{
                _service.LogInfoViaService("TrackRoamerBotHardwareController:Start()");
			}

            Running = true;
		}

		public void Stop()
		{
            if (Running)
			{
                _service.LogInfoViaService("TrackRoamerBotHardwareController:Stop()");
			}

            Running = false;
		}

		public void PollIR(ref bool left, ref bool right)
		{
            _service.LogInfoViaService("TrackRoamerBotHardwareController:PollIR()");
		}

		public void GetWhiskers(ref bool left, ref bool right)
        {
			//left = wLeft;
			//right = wRight;
        }

		public void PollWhiskers(ref bool left, ref bool right)
		{
            _service.LogInfoViaService("TrackRoamerBotHardwareController:PollWhiskers()");
		}

		/// <summary>
		/// -1.0 to 1.0
		/// </summary>
		/// <param name="left"></param>
		/// <param name="right"></param>
		public void SetSpeed(double? left, double? right)
		{
            _service.LogInfoViaService("TrackRoamerBotHardwareController:SetSpeed(L=" + left + ",R=" + right + ")");

            if ((left != null && (left < -1.0d || left > 1.0d))
                || ((right != null) && (right < -1.0d || right > 1.0d)))
            {
                throw new SystemException("Invalid Speed!");
            }

			if (left != null && m_controller != null)
			{
				int speedLeft = (int)(left * 127.0d);
				m_controller.SetMotorPowerOrSpeedLeft(speedLeft);
			}

			if (right != null && m_controller != null)
			{
				int speedRight = (int)(right * 127.0d);
				m_controller.SetMotorPowerOrSpeedRight(speedRight);
			}
		}

		public void ResetEncoderLeft()
		{
            _service.LogInfoViaService("TrackRoamerBotHardwareController : ResetEncoderLeft()");

			m_controller.ResetEncoderLeft();
		}

		public void ResetEncoderRight()
		{
            _service.LogInfoViaService("TrackRoamerBotHardwareController : ResetEncoderRight()");

			m_controller.ResetEncoderRight();
		}


		private DateTime m_lastGrabAttempt = DateTime.Now;
		private int m_betweenGrabsSec = 5;

		internal void ExecuteMain()
		{
            //_service.LogTrace("TrackRoamerBotHardwareController:ExecuteMain()");

            if (!this.Running)
			{
				return;
			}

			if (m_controller != null)
			{
                try
                {
                    if (!m_controller.isGrabbed && DateTime.Now > m_lastGrabAttempt.AddSeconds(m_betweenGrabsSec))
                    {
                        m_controller.GrabController();
                        m_lastGrabAttempt = DateTime.Now;
                    }

                    m_controller.ExecuteMain();

                    this.Connected = m_controller.isOnline;

                    frameCounter = m_controller.frameCounter;
                    errorCounter = m_controller.errorCounter;
                }
                catch (Exception exc)
                {
                    _service.LogInfoViaService("TrackRoamerBotHardwareController:ExecuteMain(): " + exc);
                }
			}
		}
	}
}
