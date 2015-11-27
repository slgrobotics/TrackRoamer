using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace LibSystem
{
	public partial class DlgPortSettings : Form
	{
		private CommBaseSettings m_settings;

		public DlgPortSettings()
		{
			InitializeComponent();

			m_settings = Project.controllerPortSettings;

			FillASCII(comboBoxXon);
			FillASCII(comboBoxXoff);
			//           Project.setDlgIcon(this);
		}

		private void FillASCII(ComboBox cb)
		{
			ASCII asc;
			for (int i = 0; (i < 256); i++)
			{
				asc = (ASCII)i;
				if ((i < 33) || (i > 126))
					cb.Items.Add(asc.ToString());
				else
					cb.Items.Add(new string((char)i, 1));
			}
		}

		private void okButton_Click(object sender, EventArgs e)
		{
			try
			{
				CollectValues();
				Project.controllerPortSettings = m_settings;
			}
			catch (Exception ee)
			{
				Tracer.Error("saving port settings: " + ee.Message);
			}

			SettingsPersister.savePortSettings();
			this.Close();
		}

		private void FillValues()
		{
			comboBoxPort.Text = m_settings.port;
			comboBoxBaudRate.Text = "" + m_settings.baudRate.ToString();
			checkBoxAR.Checked = m_settings.autoReopen;
			comboBoxParity.SelectedIndex = (int)m_settings.parity;
			comboBoxDB.SelectedIndex = comboBoxDB.FindString(m_settings.dataBits.ToString());
			comboBoxSB.SelectedIndex = (int)m_settings.stopBits;
			checkBoxCTS.Checked = m_settings.txFlowCTS;
			checkBoxDSR.Checked = m_settings.txFlowDSR;
			checkBoxTxX.Checked = m_settings.txFlowX;
			checkBoxXC.Checked = m_settings.txWhenRxXoff;
			comboBoxRTS.SelectedIndex = (int)m_settings.useRTS;
			comboBoxDTR.SelectedIndex = (int)m_settings.useDTR;
			checkBoxRxX.Checked = m_settings.rxFlowX;
			checkBoxGD.Checked = m_settings.rxGateDSR;
			comboBoxXon.SelectedIndex = (int)m_settings.XonChar;
			comboBoxXoff.SelectedIndex = (int)m_settings.XoffChar;
			numericUpDownTM.Value = m_settings.sendTimeoutMultiplier;
			numericUpDownTC.Value = m_settings.sendTimeoutConstant;
			numericUpDownLW.Value = m_settings.rxLowWater;
			numericUpDownHW.Value = m_settings.rxHighWater;
			numericUpDownRxS.Value = m_settings.rxQueue;
			checkBoxCheck.Checked = m_settings.checkAllSends;
			numericUpDownTxS.Value = m_settings.txQueue;
		}

		private void CollectValues()
		{
			m_settings.port = comboBoxPort.Text;
			m_settings.baudRate = Convert.ToInt32(comboBoxBaudRate.Text);
			m_settings.autoReopen = checkBoxAR.Checked;
			m_settings.parity = (Parity)comboBoxParity.SelectedIndex;
			m_settings.dataBits = int.Parse(comboBoxDB.Text);
			m_settings.stopBits = (StopBits)comboBoxSB.SelectedIndex;
			m_settings.txFlowCTS = checkBoxCTS.Checked;
			m_settings.txFlowDSR = checkBoxDSR.Checked;
			m_settings.txFlowX = checkBoxTxX.Checked;
			m_settings.txWhenRxXoff = checkBoxXC.Checked;
			m_settings.useRTS = (HSOutput)comboBoxRTS.SelectedIndex;
			m_settings.useDTR = (HSOutput)comboBoxDTR.SelectedIndex;
			m_settings.rxFlowX = checkBoxRxX.Checked;
			m_settings.rxGateDSR = checkBoxGD.Checked;
			m_settings.XonChar = (ASCII)comboBoxXon.SelectedIndex;
			m_settings.XoffChar = (ASCII)comboBoxXoff.SelectedIndex;
			m_settings.sendTimeoutMultiplier = (uint)numericUpDownTM.Value;
			m_settings.sendTimeoutConstant = (uint)numericUpDownTC.Value;
			m_settings.rxLowWater = (int)numericUpDownLW.Value;
			m_settings.rxHighWater = (int)numericUpDownHW.Value;
			m_settings.rxQueue = (int)numericUpDownRxS.Value;
			m_settings.txQueue = (int)numericUpDownTxS.Value;
			m_settings.checkAllSends = checkBoxCheck.Checked;
		}

		private void closeButton_Click(object sender, EventArgs e)
		{
			this.Close();
		}

		private void allDefaultsLinkLabel_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
		{
			m_settings = new CommBaseSettings();
			m_settings.port = Project.controllerPortSettings.port;
			m_settings.baudRate = Project.controllerPortSettings.baudRate;
			m_settings.parity = Parity.none;
			m_settings.autoReopen = true;
			FillValues();
		}

		private void defNohLinkLabel_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
		{
			m_settings.SetStandard(Project.controllerPortSettings.port, Project.controllerPortSettings.baudRate, Handshake.none);
			FillValues();
		}

		private void xonxoffLinkLabel_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
		{
			m_settings.SetStandard(Project.controllerPortSettings.port, Project.controllerPortSettings.baudRate, Handshake.XonXoff);
			FillValues();
		}

		private void ctsrtsLinkLabel_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
		{
			m_settings.SetStandard(Project.controllerPortSettings.port, Project.controllerPortSettings.baudRate, Handshake.CtsRts);
			FillValues();
		}

		private void dsrdtrLinkLabel_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
		{
			m_settings.SetStandard(Project.controllerPortSettings.port, Project.controllerPortSettings.baudRate, Handshake.DsrDtr);
			FillValues();
		}

		private void DlgPortSettings_Load(object sender, EventArgs e)
		{
			this.comboBoxPort.Items.AddRange(new object[] {	  "COM1:",
															  "COM2:",
															  "COM3:",
															  "COM4:",
															  "COM5:",
															  "COM6:",
															  "COM7:",
															  "COM8:"});

			this.comboBoxBaudRate.Items.AddRange(new object[] {	  "1200",
															  "2400",
															  "4800",
															  "9600",
															  "19200",
															  "38400",
															  "57600",
															  "115200"});

			FillValues();
		}
	}
}