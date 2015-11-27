//-----------------------------------------------------------------------
//  This file is part of Microsoft Robotics Developer Studio Code Samples.
// 
//  Copyright (C) Microsoft Corporation.  All rights reserved.
//
//  $File: DriveControl.Designer.cs $ $Revision: 1 $
//-----------------------------------------------------------------------

namespace TrackRoamer.Robotics.Services.TrackRoamerDashboard
{
    partial class DriveControl
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
			System.Windows.Forms.Label label1;
			System.Windows.Forms.Label label5;
			System.Windows.Forms.Label label6;
			System.Windows.Forms.Label label7;
			System.Windows.Forms.Label label2;
			System.Windows.Forms.Label label4;
			System.Windows.Forms.Label label3;
			System.Windows.Forms.Label label8;
			System.Windows.Forms.Label label10;
			this.cbJoystick = new System.Windows.Forms.ComboBox();
			this.lblX = new System.Windows.Forms.Label();
			this.lblY = new System.Windows.Forms.Label();
			this.lblZ = new System.Windows.Forms.Label();
			this.lblButtons = new System.Windows.Forms.Label();
			this.groupBox1 = new System.Windows.Forms.GroupBox();
			this.chkStop = new System.Windows.Forms.CheckBox();
			this.chkDrive = new System.Windows.Forms.CheckBox();
			this.picJoystick = new System.Windows.Forms.PictureBox();
			this.groupBox2 = new System.Windows.Forms.GroupBox();
			this.linkDirectory = new System.Windows.Forms.LinkLabel();
			this.lblNode = new System.Windows.Forms.Label();
			this.listDirectory = new System.Windows.Forms.ListBox();
			this.btnConnect = new System.Windows.Forms.Button();
			this.txtPort = new System.Windows.Forms.MaskedTextBox();
			this.txtMachine = new System.Windows.Forms.TextBox();
			this.groupBox3 = new System.Windows.Forms.GroupBox();
			this.btnDisconnect = new System.Windows.Forms.Button();
			this.lblDelay = new System.Windows.Forms.Label();
			this.btnConnectLRF = new System.Windows.Forms.Button();
			this.btnStartLRF = new System.Windows.Forms.Button();
			this.picLRF = new System.Windows.Forms.PictureBox();
			this.groupBox4 = new System.Windows.Forms.GroupBox();
			this.lblLag = new System.Windows.Forms.Label();
			this.lblMotor = new System.Windows.Forms.Label();
			this.groupBox5 = new System.Windows.Forms.GroupBox();
			this.btnBrowse = new System.Windows.Forms.Button();
			this.txtLogFile = new System.Windows.Forms.TextBox();
			this.chkLog = new System.Windows.Forms.CheckBox();
			this.saveFileDialog = new System.Windows.Forms.SaveFileDialog();
			this.tabControl1 = new System.Windows.Forms.TabControl();
			this.tabPageConnect = new System.Windows.Forms.TabPage();
			this.tabPageDrive = new System.Windows.Forms.TabPage();
			this.tabPageLog = new System.Windows.Forms.TabPage();
			this.tabPageOther = new System.Windows.Forms.TabPage();
			label1 = new System.Windows.Forms.Label();
			label5 = new System.Windows.Forms.Label();
			label6 = new System.Windows.Forms.Label();
			label7 = new System.Windows.Forms.Label();
			label2 = new System.Windows.Forms.Label();
			label4 = new System.Windows.Forms.Label();
			label3 = new System.Windows.Forms.Label();
			label8 = new System.Windows.Forms.Label();
			label10 = new System.Windows.Forms.Label();
			this.groupBox1.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.picJoystick)).BeginInit();
			this.groupBox2.SuspendLayout();
			this.groupBox3.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.picLRF)).BeginInit();
			this.groupBox4.SuspendLayout();
			this.groupBox5.SuspendLayout();
			this.tabControl1.SuspendLayout();
			this.tabPageConnect.SuspendLayout();
			this.tabPageDrive.SuspendLayout();
			this.SuspendLayout();
			// 
			// label1
			// 
			label1.AutoSize = true;
			label1.Location = new System.Drawing.Point(6, 25);
			label1.Name = "label1";
			label1.Size = new System.Drawing.Size(44, 13);
			label1.TabIndex = 1;
			label1.Text = "Device:";
			// 
			// label5
			// 
			label5.AutoSize = true;
			label5.Location = new System.Drawing.Point(34, 50);
			label5.Name = "label5";
			label5.Size = new System.Drawing.Size(17, 13);
			label5.TabIndex = 5;
			label5.Text = "X:";
			label5.TextAlign = System.Drawing.ContentAlignment.TopRight;
			// 
			// label6
			// 
			label6.AutoSize = true;
			label6.Location = new System.Drawing.Point(34, 67);
			label6.Name = "label6";
			label6.Size = new System.Drawing.Size(17, 13);
			label6.TabIndex = 6;
			label6.Text = "Y:";
			label6.TextAlign = System.Drawing.ContentAlignment.TopRight;
			// 
			// label7
			// 
			label7.AutoSize = true;
			label7.Location = new System.Drawing.Point(34, 84);
			label7.Name = "label7";
			label7.Size = new System.Drawing.Size(17, 13);
			label7.TabIndex = 7;
			label7.Text = "Z:";
			label7.TextAlign = System.Drawing.ContentAlignment.TopRight;
			// 
			// label2
			// 
			label2.AutoSize = true;
			label2.Location = new System.Drawing.Point(6, 101);
			label2.Name = "label2";
			label2.Size = new System.Drawing.Size(46, 13);
			label2.TabIndex = 8;
			label2.Text = "Buttons:";
			label2.TextAlign = System.Drawing.ContentAlignment.TopRight;
			// 
			// label4
			// 
			label4.AutoSize = true;
			label4.Location = new System.Drawing.Point(7, 50);
			label4.Name = "label4";
			label4.Size = new System.Drawing.Size(29, 13);
			label4.TabIndex = 1;
			label4.Text = "Port:";
			// 
			// label3
			// 
			label3.AutoSize = true;
			label3.Location = new System.Drawing.Point(7, 20);
			label3.Name = "label3";
			label3.Size = new System.Drawing.Size(51, 13);
			label3.TabIndex = 0;
			label3.Text = "Machine:";
			// 
			// label8
			// 
			label8.AutoSize = true;
			label8.Location = new System.Drawing.Point(14, 17);
			label8.Name = "label8";
			label8.Size = new System.Drawing.Size(37, 13);
			label8.TabIndex = 16;
			label8.Text = "Motor:";
			label8.TextAlign = System.Drawing.ContentAlignment.TopRight;
			// 
			// label10
			// 
			label10.AutoSize = true;
			label10.Location = new System.Drawing.Point(23, 34);
			label10.Name = "label10";
			label10.Size = new System.Drawing.Size(28, 13);
			label10.TabIndex = 18;
			label10.Text = "Lag:";
			label10.TextAlign = System.Drawing.ContentAlignment.TopRight;
			// 
			// cbJoystick
			// 
			this.cbJoystick.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.cbJoystick.FormattingEnabled = true;
			this.cbJoystick.Location = new System.Drawing.Point(60, 22);
			this.cbJoystick.Name = "cbJoystick";
			this.cbJoystick.Size = new System.Drawing.Size(179, 21);
			this.cbJoystick.TabIndex = 0;
			this.cbJoystick.SelectedIndexChanged += new System.EventHandler(this.cbJoystick_SelectedIndexChanged);
			// 
			// lblX
			// 
			this.lblX.Location = new System.Drawing.Point(60, 50);
			this.lblX.Name = "lblX";
			this.lblX.Size = new System.Drawing.Size(35, 13);
			this.lblX.TabIndex = 2;
			this.lblX.Text = "0";
			this.lblX.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			// 
			// lblY
			// 
			this.lblY.Location = new System.Drawing.Point(60, 67);
			this.lblY.Name = "lblY";
			this.lblY.Size = new System.Drawing.Size(35, 13);
			this.lblY.TabIndex = 3;
			this.lblY.Text = "0";
			this.lblY.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			// 
			// lblZ
			// 
			this.lblZ.Location = new System.Drawing.Point(60, 84);
			this.lblZ.Name = "lblZ";
			this.lblZ.Size = new System.Drawing.Size(35, 13);
			this.lblZ.TabIndex = 4;
			this.lblZ.Text = "0";
			this.lblZ.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			// 
			// lblButtons
			// 
			this.lblButtons.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.lblButtons.Font = new System.Drawing.Font("Lucida Console", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.lblButtons.Location = new System.Drawing.Point(63, 101);
			this.lblButtons.Name = "lblButtons";
			this.lblButtons.Size = new System.Drawing.Size(176, 13);
			this.lblButtons.TabIndex = 9;
			this.lblButtons.Text = "O";
			// 
			// groupBox1
			// 
			this.groupBox1.Controls.Add(this.chkStop);
			this.groupBox1.Controls.Add(this.chkDrive);
			this.groupBox1.Controls.Add(this.picJoystick);
			this.groupBox1.Controls.Add(label1);
			this.groupBox1.Controls.Add(this.lblButtons);
			this.groupBox1.Controls.Add(this.cbJoystick);
			this.groupBox1.Controls.Add(label2);
			this.groupBox1.Controls.Add(this.lblX);
			this.groupBox1.Controls.Add(label7);
			this.groupBox1.Controls.Add(this.lblY);
			this.groupBox1.Controls.Add(label6);
			this.groupBox1.Controls.Add(this.lblZ);
			this.groupBox1.Controls.Add(label5);
			this.groupBox1.Location = new System.Drawing.Point(13, 6);
			this.groupBox1.Name = "groupBox1";
			this.groupBox1.Size = new System.Drawing.Size(245, 151);
			this.groupBox1.TabIndex = 10;
			this.groupBox1.TabStop = false;
			this.groupBox1.Text = "Direct Input Device";
			// 
			// chkStop
			// 
			this.chkStop.Appearance = System.Windows.Forms.Appearance.Button;
			this.chkStop.Location = new System.Drawing.Point(112, 117);
			this.chkStop.Name = "chkStop";
			this.chkStop.Size = new System.Drawing.Size(77, 24);
			this.chkStop.TabIndex = 12;
			this.chkStop.Text = "Stop";
			this.chkStop.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			this.chkStop.UseVisualStyleBackColor = true;
			this.chkStop.CheckedChanged += new System.EventHandler(this.chkStop_CheckedChanged);
			// 
			// chkDrive
			// 
			this.chkDrive.Appearance = System.Windows.Forms.Appearance.Button;
			this.chkDrive.Location = new System.Drawing.Point(9, 117);
			this.chkDrive.Name = "chkDrive";
			this.chkDrive.Size = new System.Drawing.Size(76, 24);
			this.chkDrive.TabIndex = 11;
			this.chkDrive.Text = "Drive";
			this.chkDrive.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			this.chkDrive.UseVisualStyleBackColor = true;
			this.chkDrive.CheckedChanged += new System.EventHandler(this.chkDrive_CheckedChanged);
			// 
			// picJoystick
			// 
			this.picJoystick.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.picJoystick.Location = new System.Drawing.Point(129, 49);
			this.picJoystick.Name = "picJoystick";
			this.picJoystick.Size = new System.Drawing.Size(49, 49);
			this.picJoystick.TabIndex = 10;
			this.picJoystick.TabStop = false;
			this.picJoystick.MouseLeave += new System.EventHandler(this.picJoystick_MouseLeave);
			this.picJoystick.MouseMove += new System.Windows.Forms.MouseEventHandler(this.picJoystick_MouseMove);
			this.picJoystick.MouseUp += new System.Windows.Forms.MouseEventHandler(this.picJoystick_MouseUp);
			// 
			// groupBox2
			// 
			this.groupBox2.Controls.Add(this.linkDirectory);
			this.groupBox2.Controls.Add(this.lblNode);
			this.groupBox2.Controls.Add(this.listDirectory);
			this.groupBox2.Controls.Add(this.btnConnect);
			this.groupBox2.Controls.Add(this.txtPort);
			this.groupBox2.Controls.Add(this.txtMachine);
			this.groupBox2.Controls.Add(label4);
			this.groupBox2.Controls.Add(label3);
			this.groupBox2.Dock = System.Windows.Forms.DockStyle.Fill;
			this.groupBox2.Location = new System.Drawing.Point(3, 3);
			this.groupBox2.Name = "groupBox2";
			this.groupBox2.Size = new System.Drawing.Size(710, 671);
			this.groupBox2.TabIndex = 11;
			this.groupBox2.TabStop = false;
			this.groupBox2.Text = "Connect to DifferentialDrive on a Remote Node";
			// 
			// linkDirectory
			// 
			this.linkDirectory.AutoSize = true;
			this.linkDirectory.Enabled = false;
			this.linkDirectory.LinkArea = new System.Windows.Forms.LinkArea(8, 9);
			this.linkDirectory.Location = new System.Drawing.Point(7, 75);
			this.linkDirectory.Name = "linkDirectory";
			this.linkDirectory.Size = new System.Drawing.Size(94, 17);
			this.linkDirectory.TabIndex = 8;
			this.linkDirectory.TabStop = true;
			this.linkDirectory.Text = "Service Directory:";
			this.linkDirectory.UseCompatibleTextRendering = true;
			this.linkDirectory.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkDirectory_LinkClicked);
			// 
			// lblNode
			// 
			this.lblNode.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.lblNode.AutoEllipsis = true;
			this.lblNode.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
			this.lblNode.Location = new System.Drawing.Point(10, 96);
			this.lblNode.Name = "lblNode";
			this.lblNode.Size = new System.Drawing.Size(691, 15);
			this.lblNode.TabIndex = 7;
			// 
			// listDirectory
			// 
			this.listDirectory.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
						| System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.listDirectory.FormattingEnabled = true;
			this.listDirectory.Location = new System.Drawing.Point(10, 119);
			this.listDirectory.Name = "listDirectory";
			this.listDirectory.Size = new System.Drawing.Size(694, 537);
			this.listDirectory.TabIndex = 5;
			this.listDirectory.DoubleClick += new System.EventHandler(this.listDirectory_DoubleClick);
			// 
			// btnConnect
			// 
			this.btnConnect.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.btnConnect.Location = new System.Drawing.Point(649, 42);
			this.btnConnect.Name = "btnConnect";
			this.btnConnect.Size = new System.Drawing.Size(55, 23);
			this.btnConnect.TabIndex = 4;
			this.btnConnect.Text = "Connect";
			this.btnConnect.UseVisualStyleBackColor = true;
			this.btnConnect.Click += new System.EventHandler(this.btnConnect_Click);
			// 
			// txtPort
			// 
			this.txtPort.Location = new System.Drawing.Point(64, 44);
			this.txtPort.Mask = "99999";
			this.txtPort.Name = "txtPort";
			this.txtPort.Size = new System.Drawing.Size(42, 20);
			this.txtPort.TabIndex = 3;
			this.txtPort.Text = "50001";
			// 
			// txtMachine
			// 
			this.txtMachine.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.txtMachine.Location = new System.Drawing.Point(64, 17);
			this.txtMachine.Name = "txtMachine";
			this.txtMachine.Size = new System.Drawing.Size(640, 20);
			this.txtMachine.TabIndex = 2;
			this.txtMachine.Text = "localhost";
			// 
			// groupBox3
			// 
			this.groupBox3.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
						| System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.groupBox3.Controls.Add(this.btnDisconnect);
			this.groupBox3.Controls.Add(this.lblDelay);
			this.groupBox3.Controls.Add(this.btnConnectLRF);
			this.groupBox3.Controls.Add(this.btnStartLRF);
			this.groupBox3.Controls.Add(this.picLRF);
			this.groupBox3.Location = new System.Drawing.Point(6, 402);
			this.groupBox3.Name = "groupBox3";
			this.groupBox3.Size = new System.Drawing.Size(697, 259);
			this.groupBox3.TabIndex = 12;
			this.groupBox3.TabStop = false;
			this.groupBox3.Text = "Laser Range Finder";
			// 
			// btnDisconnect
			// 
			this.btnDisconnect.Enabled = false;
			this.btnDisconnect.Location = new System.Drawing.Point(6, 76);
			this.btnDisconnect.Name = "btnDisconnect";
			this.btnDisconnect.Size = new System.Drawing.Size(75, 23);
			this.btnDisconnect.TabIndex = 4;
			this.btnDisconnect.Text = "Disconnect";
			this.btnDisconnect.UseVisualStyleBackColor = true;
			this.btnDisconnect.Click += new System.EventHandler(this.btnDisconnect_Click);
			// 
			// lblDelay
			// 
			this.lblDelay.Font = new System.Drawing.Font("Microsoft Sans Serif", 7F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.lblDelay.Location = new System.Drawing.Point(6, 102);
			this.lblDelay.Name = "lblDelay";
			this.lblDelay.Size = new System.Drawing.Size(75, 18);
			this.lblDelay.TabIndex = 3;
			this.lblDelay.Text = "0";
			// 
			// btnConnectLRF
			// 
			this.btnConnectLRF.Enabled = false;
			this.btnConnectLRF.Location = new System.Drawing.Point(6, 47);
			this.btnConnectLRF.Name = "btnConnectLRF";
			this.btnConnectLRF.Size = new System.Drawing.Size(75, 23);
			this.btnConnectLRF.TabIndex = 2;
			this.btnConnectLRF.Text = "Connect";
			this.btnConnectLRF.UseVisualStyleBackColor = true;
			this.btnConnectLRF.Click += new System.EventHandler(this.btnConnectLRF_Click);
			// 
			// btnStartLRF
			// 
			this.btnStartLRF.Enabled = false;
			this.btnStartLRF.Location = new System.Drawing.Point(6, 18);
			this.btnStartLRF.Name = "btnStartLRF";
			this.btnStartLRF.Size = new System.Drawing.Size(75, 23);
			this.btnStartLRF.TabIndex = 1;
			this.btnStartLRF.Text = "Start";
			this.btnStartLRF.UseVisualStyleBackColor = true;
			this.btnStartLRF.Click += new System.EventHandler(this.btnStartLRF_Click);
			// 
			// picLRF
			// 
			this.picLRF.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
						| System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.picLRF.Location = new System.Drawing.Point(87, 18);
			this.picLRF.Name = "picLRF";
			this.picLRF.Size = new System.Drawing.Size(604, 234);
			this.picLRF.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
			this.picLRF.TabIndex = 0;
			this.picLRF.TabStop = false;
			// 
			// groupBox4
			// 
			this.groupBox4.Controls.Add(label10);
			this.groupBox4.Controls.Add(this.lblLag);
			this.groupBox4.Controls.Add(label8);
			this.groupBox4.Controls.Add(this.lblMotor);
			this.groupBox4.Location = new System.Drawing.Point(276, 6);
			this.groupBox4.Name = "groupBox4";
			this.groupBox4.Size = new System.Drawing.Size(195, 71);
			this.groupBox4.TabIndex = 13;
			this.groupBox4.TabStop = false;
			this.groupBox4.Text = "Differential Drive";
			// 
			// lblLag
			// 
			this.lblLag.Location = new System.Drawing.Point(60, 34);
			this.lblLag.Name = "lblLag";
			this.lblLag.Size = new System.Drawing.Size(35, 13);
			this.lblLag.TabIndex = 17;
			this.lblLag.Text = "0";
			this.lblLag.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			// 
			// lblMotor
			// 
			this.lblMotor.Location = new System.Drawing.Point(60, 17);
			this.lblMotor.Name = "lblMotor";
			this.lblMotor.Size = new System.Drawing.Size(35, 13);
			this.lblMotor.TabIndex = 15;
			this.lblMotor.Text = "Off";
			this.lblMotor.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			// 
			// groupBox5
			// 
			this.groupBox5.Controls.Add(this.btnBrowse);
			this.groupBox5.Controls.Add(this.txtLogFile);
			this.groupBox5.Controls.Add(this.chkLog);
			this.groupBox5.Location = new System.Drawing.Point(501, 6);
			this.groupBox5.Name = "groupBox5";
			this.groupBox5.Size = new System.Drawing.Size(195, 71);
			this.groupBox5.TabIndex = 14;
			this.groupBox5.TabStop = false;
			this.groupBox5.Text = "Logging";
			// 
			// btnBrowse
			// 
			this.btnBrowse.Location = new System.Drawing.Point(162, 42);
			this.btnBrowse.Name = "btnBrowse";
			this.btnBrowse.Size = new System.Drawing.Size(27, 23);
			this.btnBrowse.TabIndex = 2;
			this.btnBrowse.Text = "...";
			this.btnBrowse.UseVisualStyleBackColor = true;
			this.btnBrowse.Click += new System.EventHandler(this.btnBrowse_Click);
			// 
			// txtLogFile
			// 
			this.txtLogFile.Location = new System.Drawing.Point(6, 43);
			this.txtLogFile.Name = "txtLogFile";
			this.txtLogFile.Size = new System.Drawing.Size(150, 20);
			this.txtLogFile.TabIndex = 1;
			// 
			// chkLog
			// 
			this.chkLog.AutoSize = true;
			this.chkLog.Location = new System.Drawing.Point(6, 19);
			this.chkLog.Name = "chkLog";
			this.chkLog.Size = new System.Drawing.Size(95, 17);
			this.chkLog.TabIndex = 0;
			this.chkLog.Text = "Log Messages";
			this.chkLog.UseVisualStyleBackColor = true;
			this.chkLog.CheckedChanged += new System.EventHandler(this.chkLog_CheckedChanged);
			// 
			// saveFileDialog
			// 
			this.saveFileDialog.Filter = "Xml log file|*.log;*.xml|All files|*.*";
			this.saveFileDialog.Title = "Log File";
			this.saveFileDialog.FileOk += new System.ComponentModel.CancelEventHandler(this.saveFileDialog_FileOk);
			// 
			// tabControl1
			// 
			this.tabControl1.Controls.Add(this.tabPageConnect);
			this.tabControl1.Controls.Add(this.tabPageDrive);
			this.tabControl1.Controls.Add(this.tabPageLog);
			this.tabControl1.Controls.Add(this.tabPageOther);
			this.tabControl1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.tabControl1.Location = new System.Drawing.Point(0, 0);
			this.tabControl1.Name = "tabControl1";
			this.tabControl1.SelectedIndex = 0;
			this.tabControl1.Size = new System.Drawing.Size(724, 703);
			this.tabControl1.TabIndex = 15;
			// 
			// tabPageConnect
			// 
			this.tabPageConnect.Controls.Add(this.groupBox2);
			this.tabPageConnect.Location = new System.Drawing.Point(4, 22);
			this.tabPageConnect.Name = "tabPageConnect";
			this.tabPageConnect.Padding = new System.Windows.Forms.Padding(3);
			this.tabPageConnect.Size = new System.Drawing.Size(716, 677);
			this.tabPageConnect.TabIndex = 0;
			this.tabPageConnect.Text = "Connect";
			this.tabPageConnect.UseVisualStyleBackColor = true;
			// 
			// tabPageDrive
			// 
			this.tabPageDrive.Controls.Add(this.groupBox1);
			this.tabPageDrive.Controls.Add(this.groupBox3);
			this.tabPageDrive.Controls.Add(this.groupBox5);
			this.tabPageDrive.Controls.Add(this.groupBox4);
			this.tabPageDrive.Location = new System.Drawing.Point(4, 22);
			this.tabPageDrive.Name = "tabPageDrive";
			this.tabPageDrive.Padding = new System.Windows.Forms.Padding(3);
			this.tabPageDrive.Size = new System.Drawing.Size(716, 677);
			this.tabPageDrive.TabIndex = 1;
			this.tabPageDrive.Text = "Drive";
			this.tabPageDrive.UseVisualStyleBackColor = true;
			// 
			// tabPageLog
			// 
			this.tabPageLog.Location = new System.Drawing.Point(4, 22);
			this.tabPageLog.Name = "tabPageLog";
			this.tabPageLog.Size = new System.Drawing.Size(716, 677);
			this.tabPageLog.TabIndex = 2;
			this.tabPageLog.Text = "Log";
			this.tabPageLog.UseVisualStyleBackColor = true;
			// 
			// tabPageOther
			// 
			this.tabPageOther.Location = new System.Drawing.Point(4, 22);
			this.tabPageOther.Name = "tabPageOther";
			this.tabPageOther.Size = new System.Drawing.Size(716, 677);
			this.tabPageOther.TabIndex = 3;
			this.tabPageOther.Text = "Other";
			this.tabPageOther.UseVisualStyleBackColor = true;
			// 
			// DriveControl
			// 
			this.AcceptButton = this.btnConnect;
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(724, 703);
			this.Controls.Add(this.tabControl1);
			this.MinimumSize = new System.Drawing.Size(486, 580);
			this.Name = "DriveControl";
			this.Text = "TrackRoamer Dashboard";
			this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.DriveControl_FormClosed);
			this.Load += new System.EventHandler(this.DriveControl_Load);
			this.groupBox1.ResumeLayout(false);
			this.groupBox1.PerformLayout();
			((System.ComponentModel.ISupportInitialize)(this.picJoystick)).EndInit();
			this.groupBox2.ResumeLayout(false);
			this.groupBox2.PerformLayout();
			this.groupBox3.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.picLRF)).EndInit();
			this.groupBox4.ResumeLayout(false);
			this.groupBox4.PerformLayout();
			this.groupBox5.ResumeLayout(false);
			this.groupBox5.PerformLayout();
			this.tabControl1.ResumeLayout(false);
			this.tabPageConnect.ResumeLayout(false);
			this.tabPageDrive.ResumeLayout(false);
			this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ComboBox cbJoystick;
        private System.Windows.Forms.Label lblX;
        private System.Windows.Forms.Label lblY;
        private System.Windows.Forms.Label lblZ;
        private System.Windows.Forms.Label lblButtons;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.ListBox listDirectory;
        private System.Windows.Forms.Button btnConnect;
        private System.Windows.Forms.MaskedTextBox txtPort;
        private System.Windows.Forms.TextBox txtMachine;
        private System.Windows.Forms.PictureBox picJoystick;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.Button btnConnectLRF;
        private System.Windows.Forms.Button btnStartLRF;
        private System.Windows.Forms.PictureBox picLRF;
        private System.Windows.Forms.CheckBox chkStop;
        private System.Windows.Forms.CheckBox chkDrive;
        private System.Windows.Forms.Button btnDisconnect;
        private System.Windows.Forms.Label lblDelay;
        private System.Windows.Forms.GroupBox groupBox4;
        private System.Windows.Forms.Label lblMotor;
        private System.Windows.Forms.Label lblLag;
        private System.Windows.Forms.GroupBox groupBox5;
        private System.Windows.Forms.TextBox txtLogFile;
        private System.Windows.Forms.CheckBox chkLog;
        private System.Windows.Forms.Button btnBrowse;
        private System.Windows.Forms.SaveFileDialog saveFileDialog;
        private System.Windows.Forms.Label lblNode;
		private System.Windows.Forms.LinkLabel linkDirectory;
		private System.Windows.Forms.TabControl tabControl1;
		private System.Windows.Forms.TabPage tabPageConnect;
		private System.Windows.Forms.TabPage tabPageDrive;
		private System.Windows.Forms.TabPage tabPageLog;
		private System.Windows.Forms.TabPage tabPageOther;
    }
}