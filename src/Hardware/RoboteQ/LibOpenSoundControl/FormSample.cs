using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using System.Data;
using System.Threading;


namespace OSC.NET
{
	/// <summary>
	/// Form1 の概要の説明です。
	/// </summary>
	public class FormSample : System.Windows.Forms.Form
	{
		private OSCReceiver receiver = null;
		private OSCTransmitter transmitter = null;
		private Thread listenerThread = null;

		private System.Windows.Forms.LinkLabel linkLabel1;
		private System.Windows.Forms.Button buttonDisconnect;
		private System.Windows.Forms.Label label9;
		private System.Windows.Forms.TextBox textBoxRemoteHost;
		private System.Windows.Forms.Label label7;
		private System.Windows.Forms.Button buttonConnect;
		private System.Windows.Forms.TextBox textBoxRemotePort;
		private System.Windows.Forms.Label label6;
		private System.Windows.Forms.Label label5;
		private System.Windows.Forms.TextBox textBoxLocalPort;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.TextBox textBoxRecieveData;
		private System.Windows.Forms.TextBox textBoxRecieveAddress;
		private System.Windows.Forms.TextBox textBoxSendData;
		private System.Windows.Forms.TextBox textBoxSendAddress;
		private System.Windows.Forms.Button buttonSend;
		private System.Windows.Forms.Label label4;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.Label label8;
		private System.Windows.Forms.Label label10;
		private System.Windows.Forms.Label label11;
		private System.Windows.Forms.Label label12;
		/// <summary>
		/// 必要なデザイナ変数です。
		/// </summary>
		private System.ComponentModel.Container components = null;

		public FormSample()
		{
			//
			// Windows フォーム デザイナ サポートに必要です。
			//
			InitializeComponent();

			//
			// TODO: InitializeComponent 呼び出しの後に、コンストラクタ コードを追加してください。
			//
		}

		/// <summary>
		/// 使用されているリソースに後処理を実行します。
		/// </summary>
		protected override void Dispose( bool disposing )
		{
			if( disposing )
			{
				if (components != null) 
				{
					components.Dispose();
				}
			}
			base.Dispose( disposing );
		}

		#region Windows フォーム デザイナで生成されたコード 
		/// <summary>
		/// デザイナ サポートに必要なメソッドです。このメソッドの内容を
		/// コード エディタで変更しないでください。
		/// </summary>
		private void InitializeComponent()
		{
			this.linkLabel1 = new System.Windows.Forms.LinkLabel();
			this.buttonDisconnect = new System.Windows.Forms.Button();
			this.label9 = new System.Windows.Forms.Label();
			this.textBoxRemoteHost = new System.Windows.Forms.TextBox();
			this.label7 = new System.Windows.Forms.Label();
			this.buttonConnect = new System.Windows.Forms.Button();
			this.textBoxRemotePort = new System.Windows.Forms.TextBox();
			this.label6 = new System.Windows.Forms.Label();
			this.label5 = new System.Windows.Forms.Label();
			this.textBoxLocalPort = new System.Windows.Forms.TextBox();
			this.label1 = new System.Windows.Forms.Label();
			this.textBoxRecieveData = new System.Windows.Forms.TextBox();
			this.textBoxRecieveAddress = new System.Windows.Forms.TextBox();
			this.textBoxSendData = new System.Windows.Forms.TextBox();
			this.textBoxSendAddress = new System.Windows.Forms.TextBox();
			this.buttonSend = new System.Windows.Forms.Button();
			this.label4 = new System.Windows.Forms.Label();
			this.label3 = new System.Windows.Forms.Label();
			this.label2 = new System.Windows.Forms.Label();
			this.label8 = new System.Windows.Forms.Label();
			this.label10 = new System.Windows.Forms.Label();
			this.label11 = new System.Windows.Forms.Label();
			this.label12 = new System.Windows.Forms.Label();
			this.SuspendLayout();
			// 
			// linkLabel1
			// 
			this.linkLabel1.Location = new System.Drawing.Point(352, 176);
			this.linkLabel1.Name = "linkLabel1";
			this.linkLabel1.Size = new System.Drawing.Size(80, 16);
			this.linkLabel1.TabIndex = 43;
			this.linkLabel1.TabStop = true;
			this.linkLabel1.Text = "luvtechno.net";
			this.linkLabel1.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.linkLabel1.Click += new System.EventHandler(this.linkLabel1_Click);
			// 
			// buttonDisconnect
			// 
			this.buttonDisconnect.Enabled = false;
			this.buttonDisconnect.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.buttonDisconnect.Location = new System.Drawing.Point(360, 32);
			this.buttonDisconnect.Name = "buttonDisconnect";
			this.buttonDisconnect.Size = new System.Drawing.Size(72, 24);
			this.buttonDisconnect.TabIndex = 42;
			this.buttonDisconnect.Text = "Disconnect";
			this.buttonDisconnect.Click += new System.EventHandler(this.buttonDisconnect_Click);
			// 
			// label9
			// 
			this.label9.Location = new System.Drawing.Point(16, 24);
			this.label9.Name = "label9";
			this.label9.Size = new System.Drawing.Size(80, 16);
			this.label9.TabIndex = 40;
			this.label9.Text = "Remote Host";
			// 
			// textBoxRemoteHost
			// 
			this.textBoxRemoteHost.Location = new System.Drawing.Point(16, 40);
			this.textBoxRemoteHost.Name = "textBoxRemoteHost";
			this.textBoxRemoteHost.Size = new System.Drawing.Size(80, 19);
			this.textBoxRemoteHost.TabIndex = 39;
			this.textBoxRemoteHost.Text = "localhost";
			// 
			// label7
			// 
			this.label7.Location = new System.Drawing.Point(16, 88);
			this.label7.Name = "label7";
			this.label7.Size = new System.Drawing.Size(64, 16);
			this.label7.TabIndex = 38;
			this.label7.Text = "Address";
			// 
			// buttonConnect
			// 
			this.buttonConnect.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.buttonConnect.Location = new System.Drawing.Point(280, 32);
			this.buttonConnect.Name = "buttonConnect";
			this.buttonConnect.Size = new System.Drawing.Size(72, 24);
			this.buttonConnect.TabIndex = 36;
			this.buttonConnect.Text = "Connect";
			this.buttonConnect.Click += new System.EventHandler(this.buttonConnect_Click);
			// 
			// textBoxRemotePort
			// 
			this.textBoxRemotePort.Location = new System.Drawing.Point(104, 40);
			this.textBoxRemotePort.Name = "textBoxRemotePort";
			this.textBoxRemotePort.Size = new System.Drawing.Size(64, 19);
			this.textBoxRemotePort.TabIndex = 34;
			this.textBoxRemotePort.Text = "5500";
			// 
			// label6
			// 
			this.label6.Location = new System.Drawing.Point(104, 24);
			this.label6.Name = "label6";
			this.label6.Size = new System.Drawing.Size(72, 23);
			this.label6.TabIndex = 35;
			this.label6.Text = "Remote Port";
			// 
			// label5
			// 
			this.label5.Location = new System.Drawing.Point(16, 152);
			this.label5.Name = "label5";
			this.label5.Size = new System.Drawing.Size(64, 16);
			this.label5.TabIndex = 33;
			this.label5.Text = "Address";
			// 
			// textBoxLocalPort
			// 
			this.textBoxLocalPort.Location = new System.Drawing.Point(184, 40);
			this.textBoxLocalPort.Name = "textBoxLocalPort";
			this.textBoxLocalPort.Size = new System.Drawing.Size(80, 19);
			this.textBoxLocalPort.TabIndex = 30;
			this.textBoxLocalPort.Text = "6600";
			// 
			// label1
			// 
			this.label1.Location = new System.Drawing.Point(8, 72);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(64, 23);
			this.label1.TabIndex = 28;
			this.label1.Text = "Send";
			// 
			// textBoxRecieveData
			// 
			this.textBoxRecieveData.Enabled = false;
			this.textBoxRecieveData.Location = new System.Drawing.Point(104, 168);
			this.textBoxRecieveData.Name = "textBoxRecieveData";
			this.textBoxRecieveData.Size = new System.Drawing.Size(64, 19);
			this.textBoxRecieveData.TabIndex = 27;
			this.textBoxRecieveData.Text = "";
			// 
			// textBoxRecieveAddress
			// 
			this.textBoxRecieveAddress.Enabled = false;
			this.textBoxRecieveAddress.Location = new System.Drawing.Point(16, 168);
			this.textBoxRecieveAddress.Name = "textBoxRecieveAddress";
			this.textBoxRecieveAddress.Size = new System.Drawing.Size(80, 19);
			this.textBoxRecieveAddress.TabIndex = 26;
			this.textBoxRecieveAddress.Text = "";
			// 
			// textBoxSendData
			// 
			this.textBoxSendData.Enabled = false;
			this.textBoxSendData.Location = new System.Drawing.Point(104, 104);
			this.textBoxSendData.Name = "textBoxSendData";
			this.textBoxSendData.Size = new System.Drawing.Size(64, 19);
			this.textBoxSendData.TabIndex = 25;
			this.textBoxSendData.Text = "0";
			// 
			// textBoxSendAddress
			// 
			this.textBoxSendAddress.Enabled = false;
			this.textBoxSendAddress.Location = new System.Drawing.Point(16, 104);
			this.textBoxSendAddress.Name = "textBoxSendAddress";
			this.textBoxSendAddress.Size = new System.Drawing.Size(80, 19);
			this.textBoxSendAddress.TabIndex = 24;
			this.textBoxSendAddress.Text = "/test";
			// 
			// buttonSend
			// 
			this.buttonSend.Enabled = false;
			this.buttonSend.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.buttonSend.Location = new System.Drawing.Point(184, 96);
			this.buttonSend.Name = "buttonSend";
			this.buttonSend.Size = new System.Drawing.Size(72, 24);
			this.buttonSend.TabIndex = 23;
			this.buttonSend.Text = "Send";
			this.buttonSend.Click += new System.EventHandler(this.buttonSend_Click);
			// 
			// label4
			// 
			this.label4.Location = new System.Drawing.Point(104, 152);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(40, 23);
			this.label4.TabIndex = 32;
			this.label4.Text = "Data";
			// 
			// label3
			// 
			this.label3.Location = new System.Drawing.Point(184, 24);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(64, 23);
			this.label3.TabIndex = 31;
			this.label3.Text = "Local Port";
			// 
			// label2
			// 
			this.label2.Location = new System.Drawing.Point(8, 136);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(48, 23);
			this.label2.TabIndex = 29;
			this.label2.Text = "Receive";
			// 
			// label8
			// 
			this.label8.Location = new System.Drawing.Point(104, 88);
			this.label8.Name = "label8";
			this.label8.Size = new System.Drawing.Size(40, 23);
			this.label8.TabIndex = 37;
			this.label8.Text = "Data";
			// 
			// label10
			// 
			this.label10.Location = new System.Drawing.Point(8, 8);
			this.label10.Name = "label10";
			this.label10.Size = new System.Drawing.Size(176, 16);
			this.label10.TabIndex = 41;
			this.label10.Text = "Network Configuration";
			// 
			// label11
			// 
			this.label11.Location = new System.Drawing.Point(216, 160);
			this.label11.Name = "label11";
			this.label11.Size = new System.Drawing.Size(216, 16);
			this.label11.TabIndex = 44;
			this.label11.Text = "Copyright (C) 2006 Yoshinori Kawasaki";
			this.label11.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// label12
			// 
			this.label12.Location = new System.Drawing.Point(272, 144);
			this.label12.Name = "label12";
			this.label12.Size = new System.Drawing.Size(160, 16);
			this.label12.TabIndex = 45;
			this.label12.Text = "Open Sound Control for .NET";
			this.label12.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// FormSample
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 12);
			this.ClientSize = new System.Drawing.Size(440, 198);
			this.Controls.Add(this.linkLabel1);
			this.Controls.Add(this.buttonDisconnect);
			this.Controls.Add(this.label9);
			this.Controls.Add(this.textBoxRemoteHost);
			this.Controls.Add(this.label7);
			this.Controls.Add(this.buttonConnect);
			this.Controls.Add(this.textBoxRemotePort);
			this.Controls.Add(this.label6);
			this.Controls.Add(this.label5);
			this.Controls.Add(this.textBoxLocalPort);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.textBoxRecieveData);
			this.Controls.Add(this.textBoxRecieveAddress);
			this.Controls.Add(this.textBoxSendData);
			this.Controls.Add(this.textBoxSendAddress);
			this.Controls.Add(this.buttonSend);
			this.Controls.Add(this.label4);
			this.Controls.Add(this.label3);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.label8);
			this.Controls.Add(this.label10);
			this.Controls.Add(this.label11);
			this.Controls.Add(this.label12);
			this.Name = "FormSample";
			this.Text = "OSC .NET Sample";
			this.ResumeLayout(false);

		}
		#endregion

		/// <summary>
		/// アプリケーションのメイン エントリ ポイントです。
		/// </summary>
		[STAThread]
		static void Main() 
		{
			Application.Run(new FormSample());
		}

		private void updateComponents(bool connected)
		{
			buttonConnect.Enabled = !connected;
			textBoxRemoteHost.Enabled = textBoxRemotePort.Enabled = textBoxLocalPort.Enabled = !connected;
			buttonDisconnect.Enabled = connected;
			textBoxSendAddress.Enabled = textBoxSendData.Enabled = buttonSend.Enabled = connected;
			textBoxRecieveAddress.Enabled = textBoxRecieveData.Enabled = connected;
		}

		private void buttonConnect_Click(object sender, System.EventArgs e)
		{
			//osc = new OSCUDPTalk(textBoxRemoteHost.Text, int.Parse(textBoxRemotePort.Text), int.Parse(textBoxLocalPort.Text), true);
			transmitter = new OSCTransmitter(textBoxRemoteHost.Text, int.Parse(textBoxRemotePort.Text));
			receiver = new OSCReceiver(int.Parse(textBoxLocalPort.Text));
			updateComponents(true);
			listenerThread = new Thread(new ThreadStart(this.receive));
			listenerThread.Start();
		}

		private void disconnect()
		{
			if(transmitter != null) transmitter.Close();
			if(receiver != null) receiver.Close();
			if(listenerThread !=null) listenerThread.Abort();
			listenerThread = null;
			receiver = null;
			transmitter = null;
		}

		private void buttonDisconnect_Click(object sender, System.EventArgs e)
		{
			disconnect();
			updateComponents(false);
		}

		private void buttonSend_Click(object sender, System.EventArgs e)
		{
			OSCMessage msg = new OSCMessage(textBoxSendAddress.Text);
			try
			{
				msg.Append(int.Parse(textBoxSendData.Text));
			}
			catch
			{
				msg.Append(textBoxSendData.Text);
			}

			transmitter.Send(msg);
		}

		private void receive()
		{
			while(true)
			{
				OSCPacket msg = receiver.Receive();
				textBoxRecieveAddress.Text = msg.Address;
				ArrayList objs = msg.Values;
				string data = "";
				bool first = true;
				foreach(object obj in objs)
				{
					if(first) first = false;
					else data += ", ";
					data += obj.ToString();
				}
				textBoxRecieveData.Text = data;
			}
		}

		private void Form1_Closing(object sender, System.ComponentModel.CancelEventArgs e)
		{
			disconnect();
		}

		/// <summary>
		/// Credit
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void linkLabel1_Click(object sender, System.EventArgs e)
		{
			string target = "http://luvtechno.net";
			try
			{
				System.Diagnostics.Process.Start(target);
			}
			catch(System.ComponentModel.Win32Exception noBrowser)
			{
				if (noBrowser.ErrorCode==-2147467259) MessageBox.Show(noBrowser.Message);
			}
			catch (System.Exception other)
			{
				MessageBox.Show(other.Message);
			}
		}
	}
}
