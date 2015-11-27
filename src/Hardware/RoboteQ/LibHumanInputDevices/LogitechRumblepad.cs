using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;
using System.Windows.Forms;
using System.Drawing;
using System.Threading;
using Microsoft.DirectX.DirectInput;

using LibSystem;

namespace LibHumanInputDevices
{
    public class JoystickEventArgs : EventArgs
    {
		private bool m_mandatory;
		private int m_position;

		public bool mandatory { get { return m_mandatory; } }
		public int position { get { return m_position; } }		// 0 to 32767 (center) to 65535

		public JoystickEventArgs(bool mnd, int pos)
		{
			m_mandatory = mnd;
			m_position = pos;
		}
    }

	public class RPButtonsEventArgs : EventArgs
	{
		private bool m_pressed;
		private int m_button;

		public bool pressed { get { return m_pressed; } }
		public int button { get { return m_button; } }

		public RPButtonsEventArgs(bool prsd, int btn)
		{
			m_pressed = prsd;
			m_button = btn;
		}

		public override string ToString()
		{
			return "Btn: " + m_button + (m_pressed ? " pressed" : " released");
		}
	}

	public delegate void JoystickEventHandler(Object sender, JoystickEventArgs e);
	public delegate void ButtonEventHandler(Object sender, RPButtonsEventArgs e);

	public class LogitechRumblepad
	{
		public event JoystickEventHandler leftJoystickVertMoved;
		public event JoystickEventHandler rightJoystickVertMoved;
		public event ButtonEventHandler btnChangedState;

        private Device m_gamepad = null;
		private Control m_mainForm = null;

        public LogitechRumblepad(Control mainForm, TreeView tvDevices)
        {
			m_mainForm = mainForm;

            //PopulateAllDevices(tvDevices);
        }

        public void init()
        {
			ensureGamepad();

            gamepadTickerTimer = new System.Windows.Forms.Timer();
            gamepadTickerTimer.Interval = 20;
            gamepadTickerTimer.Tick += new EventHandler(gamepadTicker);
            gamepadTickerTimer.Start();

            Tracer.WriteLine("OK: Gamepad ticker ON");
        }

		private void ensureGamepad()
		{
			if (m_gamepad == null)
			{
				int tryCnt = 3;
				while (m_gamepad == null && tryCnt-- > 0)
				{
					try
					{
						DeviceList dl = Manager.GetDevices(DeviceClass.GameControl, EnumDevicesFlags.AttachedOnly);
						foreach (DeviceInstance di in dl)
						{
							m_gamepad = new Device(di.InstanceGuid);
							Tracer.Trace("GameControl device found: " + m_gamepad.DeviceInformation.ProductName + "  -  " + m_gamepad.DeviceInformation.InstanceName);
							if (m_gamepad.DeviceInformation.ProductName.ToLower().IndexOf("rumble") >= 0)
							{
								//set cooperative level.
								m_gamepad.SetCooperativeLevel(
									m_mainForm,
									CooperativeLevelFlags.Exclusive | CooperativeLevelFlags.Background);

								//Set axis mode absolute.
								m_gamepad.Properties.AxisModeAbsolute = true;

								//Acquire joystick for capturing.
								m_gamepad.Acquire();

								string info = "Rumblepad: " + m_gamepad.DeviceInformation.InstanceName + "  state: " + m_gamepad.CurrentJoystickState;
								Tracer.WriteLine(info);
								break;
							}
						}
					}
					catch (Exception exc)
					{
						Tracer.Error(exc.ToString());
						Thread.Sleep(1000);
					}
				}

				if (m_gamepad == null)
				{
					//Throw exception if gamepad not found.
					throw new Exception("No gamepad found.");
				}
			}
		}

		public void Close()
		{
			if (gamepadTickerTimer != null)
			{
				gamepadTickerTimer.Enabled = false;
				gamepadTickerTimer.Dispose();
				gamepadTickerTimer = null;
			}

			if (m_gamepad != null)
			{
				m_gamepad.Unacquire();
				m_gamepad.Dispose();
				m_gamepad = null;
			}
		}

        private void PopulateAllDevices(TreeView tvDevices)
        {
            //Add "All Devices" node to TreeView
            TreeNode allNode = new TreeNode("All Devices");
            tvDevices.Nodes.Add(allNode);

            //Populate All devices
            foreach (DeviceInstance di in Manager.Devices)
            {

                //Get Device name
                TreeNode nameNode = new TreeNode(di.InstanceName);

                //Is device attached?
                TreeNode attachedNode = new TreeNode(
                    "Attached = " +
                    Manager.GetDeviceAttached(di.ProductGuid));

                //Get device Guid
                TreeNode guidNode = new TreeNode(
                    "Guid = " + di.InstanceGuid);

                //Add nodes
                nameNode.Nodes.Add(attachedNode);
                nameNode.Nodes.Add(guidNode);
                allNode.Nodes.Add(nameNode);
            }

        }

        private void PopulatePointers(TreeView tvDevices)
        {
            //Add "Pointer Devices" node to TreeView
            TreeNode pointerNode = new TreeNode("Pointer Devices");
            tvDevices.Nodes.Add(pointerNode);

            //Populate Attached Mouse/Pointing Devices
            foreach (DeviceInstance di in
                Manager.GetDevices(DeviceClass.Pointer, EnumDevicesFlags.AttachedOnly))		// DeviceClass.GameControl 
            {

                //Get device name
                TreeNode nameNode = new TreeNode(di.InstanceName);
                nameNode.Tag = di;
                TreeNode guidNode = new TreeNode("Guid = " + di.InstanceGuid);

                //Add nodes
                nameNode.Nodes.Add(guidNode);
                pointerNode.Nodes.Add(nameNode);
            }
        }

        private System.Windows.Forms.Timer gamepadTickerTimer = null;

        public void gamepadTicker(object obj, System.EventArgs args)
        {
            try
            {
                //Tracer.WriteLine("...gamepad ticker... " + DateTime.Now);

                queryGamepad();

            }
            catch { }

            gamepadTickerTimer.Enabled = true;
        }


        //private string lastJInfo = "";

		private long lastMandatoryTicks = DateTime.Now.Ticks;
		private int mandatoryIntervalMs = 100;

		private int m_leftVpos = -1;
		private int m_rightVpos = -1;
		private byte[] m_buttonStates = null;

        private void queryGamepad()
        {
            if (m_gamepad != null)
            {
                //Get gamepad State.
                JoystickState state = m_gamepad.CurrentJoystickState;

				bool mandatory = DateTime.Now.Ticks > (lastMandatoryTicks + mandatoryIntervalMs * 10000);

				if (mandatory)
				{
					lastMandatoryTicks = DateTime.Now.Ticks;
				}

				/*
                string info = "";

                //info = state.ToString().Replace("\n"," ") + "  ";

                ////Capture Position.
                info += "X:" + state.X + " ";
                info += "Y:" + state.Y + " ";
                info += "Z:" + state.Z + " ";
                //info += "sldr0:" + state.GetSlider()[0] + " ";
                //info += "sldr1:" + state.GetSlider()[1] + " ";
                //info += "Rx:" + state.Rx + " ";
                //info += "Ry:" + state.Ry + " ";
                info += "Rz:" + state.Rz + " ";
				*/

                //Capture Buttons.
                byte[] buttons = state.GetButtons();
				
				if (m_buttonStates == null)
				{
					m_buttonStates = new byte[buttons.Length];
					for (int i = 0; i < m_buttonStates.Length; i++)
					{
						m_buttonStates[i] = 0;
					}
				}

				for (int i = 0; i < buttons.Length; i++)
                {
					if (buttons[i] != m_buttonStates[i])
                    {
                        //info += "Btn:" + (i + 1) + " ";

						RPButtonsEventArgs btnArgs = null;

						if (buttons[i] > m_buttonStates[i])
						{
							// button pressed
							btnArgs = new RPButtonsEventArgs(true, i + 1);
						}
						else if (buttons[i] < m_buttonStates[i])
						{
							// button released
							btnArgs = new RPButtonsEventArgs(false, i + 1);
						}

						if (btnArgs != null && btnChangedState != null)
						{
							btnChangedState(this, btnArgs);
						}

						m_buttonStates[i] = buttons[i];
                    }
                }

				if ((m_leftVpos != state.Y || mandatory) && leftJoystickVertMoved != null)
				{
					JoystickEventArgs args = new JoystickEventArgs(mandatory, state.Y);
					m_leftVpos = state.Y;
					leftJoystickVertMoved(this, args);
				}

				if ((m_rightVpos != state.Rz || mandatory) && rightJoystickVertMoved != null)
				{
					JoystickEventArgs args = new JoystickEventArgs(mandatory, state.Rz);
					m_rightVpos = state.Rz;
					rightJoystickVertMoved(this, args);
				}

            }
        }


	}
}
