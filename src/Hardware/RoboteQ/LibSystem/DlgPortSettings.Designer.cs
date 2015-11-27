namespace LibSystem
{
    partial class DlgPortSettings
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
            this.components = new System.ComponentModel.Container();
            this.comboBoxParity = new System.Windows.Forms.ComboBox();
            this.comboBoxDB = new System.Windows.Forms.ComboBox();
            this.comboBoxSB = new System.Windows.Forms.ComboBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.label21 = new System.Windows.Forms.Label();
            this.label20 = new System.Windows.Forms.Label();
            this.label19 = new System.Windows.Forms.Label();
            this.numericUpDownTC = new System.Windows.Forms.NumericUpDown();
            this.numericUpDownTM = new System.Windows.Forms.NumericUpDown();
            this.numericUpDownTxS = new System.Windows.Forms.NumericUpDown();
            this.checkBoxCheck = new System.Windows.Forms.CheckBox();
            this.checkBoxXC = new System.Windows.Forms.CheckBox();
            this.checkBoxTxX = new System.Windows.Forms.CheckBox();
            this.checkBoxDSR = new System.Windows.Forms.CheckBox();
            this.checkBoxCTS = new System.Windows.Forms.CheckBox();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.label24 = new System.Windows.Forms.Label();
            this.label23 = new System.Windows.Forms.Label();
            this.label22 = new System.Windows.Forms.Label();
            this.numericUpDownRxS = new System.Windows.Forms.NumericUpDown();
            this.numericUpDownHW = new System.Windows.Forms.NumericUpDown();
            this.numericUpDownLW = new System.Windows.Forms.NumericUpDown();
            this.checkBoxGD = new System.Windows.Forms.CheckBox();
            this.checkBoxRxX = new System.Windows.Forms.CheckBox();
            this.label7 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.comboBoxDTR = new System.Windows.Forms.ComboBox();
            this.comboBoxRTS = new System.Windows.Forms.ComboBox();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.label2 = new System.Windows.Forms.Label();
            this.label11 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.groupBox4 = new System.Windows.Forms.GroupBox();
            this.label9 = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.comboBoxXoff = new System.Windows.Forms.ComboBox();
            this.comboBoxXon = new System.Windows.Forms.ComboBox();
            this.checkBoxAR = new System.Windows.Forms.CheckBox();
            this.openFileDialog = new System.Windows.Forms.OpenFileDialog();
            this.saveFileDialog = new System.Windows.Forms.SaveFileDialog();
            this.toolTip = new System.Windows.Forms.ToolTip(this.components);
            this.closeButton = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.defNohLinkLabel = new System.Windows.Forms.LinkLabel();
            this.xonxoffLinkLabel = new System.Windows.Forms.LinkLabel();
            this.ctsrtsLinkLabel = new System.Windows.Forms.LinkLabel();
            this.dsrdtrLinkLabel = new System.Windows.Forms.LinkLabel();
            this.allDefaultsLinkLabel = new System.Windows.Forms.LinkLabel();
            this.okButton = new System.Windows.Forms.Button();
            this.comboBoxPort = new System.Windows.Forms.ComboBox();
            this.comboBoxBaudRate = new System.Windows.Forms.ComboBox();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownTC)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownTM)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownTxS)).BeginInit();
            this.groupBox2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownRxS)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownHW)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownLW)).BeginInit();
            this.groupBox3.SuspendLayout();
            this.groupBox4.SuspendLayout();
            this.SuspendLayout();
            // 
            // comboBoxParity
            // 
            this.comboBoxParity.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxParity.Items.AddRange(new object[] {
            "none",
            "odd",
            "even",
            "mark",
            "space"});
            this.comboBoxParity.Location = new System.Drawing.Point(131, 65);
            this.comboBoxParity.Name = "comboBoxParity";
            this.comboBoxParity.Size = new System.Drawing.Size(74, 21);
            this.comboBoxParity.TabIndex = 4;
            this.toolTip.SetToolTip(this.comboBoxParity, "Parity scheme (except for [none] adds a bit to the frame)");
            // 
            // comboBoxDB
            // 
            this.comboBoxDB.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxDB.Items.AddRange(new object[] {
            "4",
            "5",
            "6",
            "7",
            "8"});
            this.comboBoxDB.Location = new System.Drawing.Point(9, 65);
            this.comboBoxDB.Name = "comboBoxDB";
            this.comboBoxDB.Size = new System.Drawing.Size(75, 21);
            this.comboBoxDB.TabIndex = 6;
            this.toolTip.SetToolTip(this.comboBoxDB, "Number of data bits in the frame (unsupported rates will throw an exception)");
            // 
            // comboBoxSB
            // 
            this.comboBoxSB.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxSB.Items.AddRange(new object[] {
            "1",
            "1.5",
            "2"});
            this.comboBoxSB.Location = new System.Drawing.Point(252, 65);
            this.comboBoxSB.Name = "comboBoxSB";
            this.comboBoxSB.Size = new System.Drawing.Size(75, 21);
            this.comboBoxSB.TabIndex = 8;
            this.toolTip.SetToolTip(this.comboBoxSB, "Number of stop bits added to the frame");
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.label21);
            this.groupBox1.Controls.Add(this.label20);
            this.groupBox1.Controls.Add(this.label19);
            this.groupBox1.Controls.Add(this.numericUpDownTC);
            this.groupBox1.Controls.Add(this.numericUpDownTM);
            this.groupBox1.Controls.Add(this.numericUpDownTxS);
            this.groupBox1.Controls.Add(this.checkBoxCheck);
            this.groupBox1.Controls.Add(this.checkBoxXC);
            this.groupBox1.Controls.Add(this.checkBoxTxX);
            this.groupBox1.Controls.Add(this.checkBoxDSR);
            this.groupBox1.Controls.Add(this.checkBoxCTS);
            this.groupBox1.Location = new System.Drawing.Point(5, 103);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(168, 252);
            this.groupBox1.TabIndex = 10;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Tx Flow Control";
            // 
            // label21
            // 
            this.label21.Location = new System.Drawing.Point(9, 188);
            this.label21.Name = "label21";
            this.label21.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.label21.Size = new System.Drawing.Size(66, 18);
            this.label21.TabIndex = 29;
            this.label21.Text = "TO Mult";
            this.label21.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.toolTip.SetToolTip(this.label21, "Timeout multiplier (ms)");
            // 
            // label20
            // 
            this.label20.Location = new System.Drawing.Point(9, 160);
            this.label20.Name = "label20";
            this.label20.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.label20.Size = new System.Drawing.Size(66, 19);
            this.label20.TabIndex = 28;
            this.label20.Text = "TO Const";
            this.label20.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.toolTip.SetToolTip(this.label20, "Timeout constant (ms)");
            // 
            // label19
            // 
            this.label19.Location = new System.Drawing.Point(9, 132);
            this.label19.Name = "label19";
            this.label19.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.label19.Size = new System.Drawing.Size(66, 19);
            this.label19.TabIndex = 27;
            this.label19.Text = "Q Size";
            this.label19.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.toolTip.SetToolTip(this.label19, "Requested size of transmission buffer (0 means use operating system defaults");
            // 
            // numericUpDownTC
            // 
            this.numericUpDownTC.Increment = new decimal(new int[] {
            500,
            0,
            0,
            0});
            this.numericUpDownTC.Location = new System.Drawing.Point(84, 159);
            this.numericUpDownTC.Maximum = new decimal(new int[] {
            60000,
            0,
            0,
            0});
            this.numericUpDownTC.Name = "numericUpDownTC";
            this.numericUpDownTC.Size = new System.Drawing.Size(75, 20);
            this.numericUpDownTC.TabIndex = 22;
            this.toolTip.SetToolTip(this.numericUpDownTC, "Timeout constant (ms)");
            // 
            // numericUpDownTM
            // 
            this.numericUpDownTM.Increment = new decimal(new int[] {
            500,
            0,
            0,
            0});
            this.numericUpDownTM.Location = new System.Drawing.Point(84, 186);
            this.numericUpDownTM.Maximum = new decimal(new int[] {
            60000,
            0,
            0,
            0});
            this.numericUpDownTM.Name = "numericUpDownTM";
            this.numericUpDownTM.Size = new System.Drawing.Size(75, 20);
            this.numericUpDownTM.TabIndex = 20;
            this.toolTip.SetToolTip(this.numericUpDownTM, "Timeout multiplier (ms)");
            // 
            // numericUpDownTxS
            // 
            this.numericUpDownTxS.Increment = new decimal(new int[] {
            500,
            0,
            0,
            0});
            this.numericUpDownTxS.Location = new System.Drawing.Point(84, 131);
            this.numericUpDownTxS.Maximum = new decimal(new int[] {
            60000,
            0,
            0,
            0});
            this.numericUpDownTxS.Name = "numericUpDownTxS";
            this.numericUpDownTxS.Size = new System.Drawing.Size(75, 20);
            this.numericUpDownTxS.TabIndex = 25;
            this.toolTip.SetToolTip(this.numericUpDownTxS, "Requested size of transmission buffer (0 means use operating system defaults");
            // 
            // checkBoxCheck
            // 
            this.checkBoxCheck.Location = new System.Drawing.Point(9, 224);
            this.checkBoxCheck.Name = "checkBoxCheck";
            this.checkBoxCheck.Size = new System.Drawing.Size(151, 19);
            this.checkBoxCheck.TabIndex = 26;
            this.checkBoxCheck.Text = "Check all sends";
            this.toolTip.SetToolTip(this.checkBoxCheck, "Check the result of all sends (slows performance)");
            // 
            // checkBoxXC
            // 
            this.checkBoxXC.Location = new System.Drawing.Point(9, 103);
            this.checkBoxXC.Name = "checkBoxXC";
            this.checkBoxXC.Size = new System.Drawing.Size(156, 18);
            this.checkBoxXC.TabIndex = 3;
            this.checkBoxXC.Text = "Tx when Rx Xoff";
            this.toolTip.SetToolTip(this.checkBoxXC, "Block transmission when Xoff has been sent (for devices which treat any character" +
                    " as Xon)");
            // 
            // checkBoxTxX
            // 
            this.checkBoxTxX.Location = new System.Drawing.Point(9, 75);
            this.checkBoxTxX.Name = "checkBoxTxX";
            this.checkBoxTxX.Size = new System.Drawing.Size(156, 19);
            this.checkBoxTxX.TabIndex = 2;
            this.checkBoxTxX.Text = "Xon / Xoff";
            this.toolTip.SetToolTip(this.checkBoxTxX, "Xon and Xoff codes control transmission");
            // 
            // checkBoxDSR
            // 
            this.checkBoxDSR.Location = new System.Drawing.Point(9, 46);
            this.checkBoxDSR.Name = "checkBoxDSR";
            this.checkBoxDSR.Size = new System.Drawing.Size(156, 19);
            this.checkBoxDSR.TabIndex = 1;
            this.checkBoxDSR.Text = "DSR";
            this.toolTip.SetToolTip(this.checkBoxDSR, "DSR input controls transmission");
            // 
            // checkBoxCTS
            // 
            this.checkBoxCTS.Location = new System.Drawing.Point(9, 19);
            this.checkBoxCTS.Name = "checkBoxCTS";
            this.checkBoxCTS.Size = new System.Drawing.Size(156, 18);
            this.checkBoxCTS.TabIndex = 0;
            this.checkBoxCTS.Text = "CTS";
            this.toolTip.SetToolTip(this.checkBoxCTS, "CTS input controls transmission");
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.label24);
            this.groupBox2.Controls.Add(this.label23);
            this.groupBox2.Controls.Add(this.label22);
            this.groupBox2.Controls.Add(this.numericUpDownRxS);
            this.groupBox2.Controls.Add(this.numericUpDownHW);
            this.groupBox2.Controls.Add(this.numericUpDownLW);
            this.groupBox2.Controls.Add(this.checkBoxGD);
            this.groupBox2.Controls.Add(this.checkBoxRxX);
            this.groupBox2.Controls.Add(this.label7);
            this.groupBox2.Controls.Add(this.label6);
            this.groupBox2.Controls.Add(this.comboBoxDTR);
            this.groupBox2.Controls.Add(this.comboBoxRTS);
            this.groupBox2.Location = new System.Drawing.Point(173, 103);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(169, 217);
            this.groupBox2.TabIndex = 11;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Rx Flow Control";
            // 
            // label24
            // 
            this.label24.Location = new System.Drawing.Point(9, 188);
            this.label24.Name = "label24";
            this.label24.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.label24.Size = new System.Drawing.Size(66, 18);
            this.label24.TabIndex = 30;
            this.label24.Text = "L. Water";
            this.label24.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // label23
            // 
            this.label23.Location = new System.Drawing.Point(9, 160);
            this.label23.Name = "label23";
            this.label23.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.label23.Size = new System.Drawing.Size(66, 19);
            this.label23.TabIndex = 29;
            this.label23.Text = "H. Water";
            this.label23.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // label22
            // 
            this.label22.Location = new System.Drawing.Point(9, 132);
            this.label22.Name = "label22";
            this.label22.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.label22.Size = new System.Drawing.Size(66, 19);
            this.label22.TabIndex = 28;
            this.label22.Text = "Q Size";
            this.label22.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // numericUpDownRxS
            // 
            this.numericUpDownRxS.Increment = new decimal(new int[] {
            500,
            0,
            0,
            0});
            this.numericUpDownRxS.Location = new System.Drawing.Point(84, 131);
            this.numericUpDownRxS.Maximum = new decimal(new int[] {
            60000,
            0,
            0,
            0});
            this.numericUpDownRxS.Name = "numericUpDownRxS";
            this.numericUpDownRxS.Size = new System.Drawing.Size(75, 20);
            this.numericUpDownRxS.TabIndex = 20;
            this.toolTip.SetToolTip(this.numericUpDownRxS, "Requested size of reception buffer (0 means use operating system defaults");
            // 
            // numericUpDownHW
            // 
            this.numericUpDownHW.Increment = new decimal(new int[] {
            500,
            0,
            0,
            0});
            this.numericUpDownHW.Location = new System.Drawing.Point(84, 159);
            this.numericUpDownHW.Maximum = new decimal(new int[] {
            60000,
            0,
            0,
            0});
            this.numericUpDownHW.Name = "numericUpDownHW";
            this.numericUpDownHW.Size = new System.Drawing.Size(75, 20);
            this.numericUpDownHW.TabIndex = 19;
            this.toolTip.SetToolTip(this.numericUpDownHW, "Number of free spaces in buffer at which reception is disabled");
            // 
            // numericUpDownLW
            // 
            this.numericUpDownLW.Increment = new decimal(new int[] {
            500,
            0,
            0,
            0});
            this.numericUpDownLW.Location = new System.Drawing.Point(84, 186);
            this.numericUpDownLW.Maximum = new decimal(new int[] {
            60000,
            0,
            0,
            0});
            this.numericUpDownLW.Name = "numericUpDownLW";
            this.numericUpDownLW.Size = new System.Drawing.Size(75, 20);
            this.numericUpDownLW.TabIndex = 18;
            this.toolTip.SetToolTip(this.numericUpDownLW, "number of characters remaining in buffer at which reception is re-enabled");
            // 
            // checkBoxGD
            // 
            this.checkBoxGD.Location = new System.Drawing.Point(9, 103);
            this.checkBoxGD.Name = "checkBoxGD";
            this.checkBoxGD.Size = new System.Drawing.Size(132, 18);
            this.checkBoxGD.TabIndex = 10;
            this.checkBoxGD.Text = "Gate on DSR";
            this.toolTip.SetToolTip(this.checkBoxGD, "Received characters are ignored unless DSR is asserted");
            // 
            // checkBoxRxX
            // 
            this.checkBoxRxX.Location = new System.Drawing.Point(9, 75);
            this.checkBoxRxX.Name = "checkBoxRxX";
            this.checkBoxRxX.Size = new System.Drawing.Size(103, 19);
            this.checkBoxRxX.TabIndex = 9;
            this.checkBoxRxX.Text = "Xon / Xoff";
            this.toolTip.SetToolTip(this.checkBoxRxX, "Xon and Xoff are sent to control reception");
            // 
            // label7
            // 
            this.label7.Location = new System.Drawing.Point(5, 45);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(50, 19);
            this.label7.TabIndex = 8;
            this.label7.Text = "DTR";
            this.label7.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // label6
            // 
            this.label6.Location = new System.Drawing.Point(5, 22);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(50, 19);
            this.label6.TabIndex = 7;
            this.label6.Text = "RTS";
            this.label6.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // comboBoxDTR
            // 
            this.comboBoxDTR.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxDTR.Items.AddRange(new object[] {
            "none",
            "online",
            "handshake",
            "gate"});
            this.comboBoxDTR.Location = new System.Drawing.Point(66, 46);
            this.comboBoxDTR.Name = "comboBoxDTR";
            this.comboBoxDTR.Size = new System.Drawing.Size(93, 21);
            this.comboBoxDTR.TabIndex = 6;
            this.toolTip.SetToolTip(this.comboBoxDTR, "The use to which the DTR output is put");
            // 
            // comboBoxRTS
            // 
            this.comboBoxRTS.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxRTS.Items.AddRange(new object[] {
            "none",
            "online",
            "handshake",
            "gate"});
            this.comboBoxRTS.Location = new System.Drawing.Point(66, 19);
            this.comboBoxRTS.Name = "comboBoxRTS";
            this.comboBoxRTS.Size = new System.Drawing.Size(93, 21);
            this.comboBoxRTS.TabIndex = 5;
            this.toolTip.SetToolTip(this.comboBoxRTS, "The use to which the RTS output is put");
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.comboBoxBaudRate);
            this.groupBox3.Controls.Add(this.comboBoxPort);
            this.groupBox3.Controls.Add(this.label2);
            this.groupBox3.Controls.Add(this.label11);
            this.groupBox3.Controls.Add(this.label5);
            this.groupBox3.Controls.Add(this.label3);
            this.groupBox3.Controls.Add(this.label4);
            this.groupBox3.Controls.Add(this.comboBoxDB);
            this.groupBox3.Controls.Add(this.comboBoxParity);
            this.groupBox3.Controls.Add(this.comboBoxSB);
            this.groupBox3.Location = new System.Drawing.Point(5, 0);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(337, 103);
            this.groupBox3.TabIndex = 20;
            this.groupBox3.TabStop = false;
            // 
            // label2
            // 
            this.label2.Location = new System.Drawing.Point(205, 21);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(41, 19);
            this.label2.TabIndex = 13;
            this.label2.Text = "Baud";
            this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.toolTip.SetToolTip(this.label2, "Baud rate (unsupported rates will throw an exception)");
            this.label2.UseMnemonic = false;
            // 
            // label11
            // 
            this.label11.Location = new System.Drawing.Point(50, 21);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(75, 19);
            this.label11.TabIndex = 12;
            this.label11.Text = "Port";
            this.label11.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.toolTip.SetToolTip(this.label11, "Communications port name (\"COM1:\")");
            // 
            // label5
            // 
            this.label5.Location = new System.Drawing.Point(245, 46);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(85, 19);
            this.label5.TabIndex = 11;
            this.label5.Text = "StopBits";
            this.label5.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.toolTip.SetToolTip(this.label5, "Number of stop bits added to the frame");
            // 
            // label3
            // 
            this.label3.Location = new System.Drawing.Point(125, 46);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(85, 19);
            this.label3.TabIndex = 10;
            this.label3.Text = "Parity";
            this.label3.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.toolTip.SetToolTip(this.label3, "Parity scheme (except for [none] adds a bit to the frame)");
            // 
            // label4
            // 
            this.label4.Location = new System.Drawing.Point(5, 46);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(85, 19);
            this.label4.TabIndex = 9;
            this.label4.Text = "DataBits";
            this.label4.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.toolTip.SetToolTip(this.label4, "Number of data bits in the frame (unsupported rates will throw an exception)");
            // 
            // groupBox4
            // 
            this.groupBox4.Controls.Add(this.label9);
            this.groupBox4.Controls.Add(this.label8);
            this.groupBox4.Controls.Add(this.comboBoxXoff);
            this.groupBox4.Controls.Add(this.comboBoxXon);
            this.groupBox4.Location = new System.Drawing.Point(5, 354);
            this.groupBox4.Name = "groupBox4";
            this.groupBox4.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.groupBox4.Size = new System.Drawing.Size(337, 56);
            this.groupBox4.TabIndex = 21;
            this.groupBox4.TabStop = false;
            // 
            // label9
            // 
            this.label9.Location = new System.Drawing.Point(160, 21);
            this.label9.Name = "label9";
            this.label9.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.label9.Size = new System.Drawing.Size(85, 19);
            this.label9.TabIndex = 28;
            this.label9.Text = "Xoff Code";
            this.label9.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.toolTip.SetToolTip(this.label9, "ASCII code to use for Xoff signal");
            // 
            // label8
            // 
            this.label8.Location = new System.Drawing.Point(9, 21);
            this.label8.Name = "label8";
            this.label8.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.label8.Size = new System.Drawing.Size(75, 19);
            this.label8.TabIndex = 27;
            this.label8.Text = "Xon Code";
            this.label8.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.toolTip.SetToolTip(this.label8, "ASCII code to use for Xon signal");
            // 
            // comboBoxXoff
            // 
            this.comboBoxXoff.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxXoff.Location = new System.Drawing.Point(256, 19);
            this.comboBoxXoff.Name = "comboBoxXoff";
            this.comboBoxXoff.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.comboBoxXoff.Size = new System.Drawing.Size(65, 21);
            this.comboBoxXoff.TabIndex = 26;
            this.toolTip.SetToolTip(this.comboBoxXoff, "ASCII code to use for Xoff signal");
            // 
            // comboBoxXon
            // 
            this.comboBoxXon.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxXon.Location = new System.Drawing.Point(88, 19);
            this.comboBoxXon.Name = "comboBoxXon";
            this.comboBoxXon.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.comboBoxXon.Size = new System.Drawing.Size(65, 21);
            this.comboBoxXon.TabIndex = 25;
            this.toolTip.SetToolTip(this.comboBoxXon, "ASCII code to use for Xon signal");
            // 
            // checkBoxAR
            // 
            this.checkBoxAR.Location = new System.Drawing.Point(183, 327);
            this.checkBoxAR.Name = "checkBoxAR";
            this.checkBoxAR.Size = new System.Drawing.Size(152, 19);
            this.checkBoxAR.TabIndex = 22;
            this.checkBoxAR.Text = "Auto re-open";
            this.toolTip.SetToolTip(this.checkBoxAR, "Automatically reopen port if it closes due to an error");
            // 
            // openFileDialog
            // 
            this.openFileDialog.DefaultExt = "JHC";
            this.openFileDialog.Filter = "JHBaseTerm files|*.JHC|All files|*.*";
            this.openFileDialog.Title = "Open XML Settings File";
            // 
            // saveFileDialog
            // 
            this.saveFileDialog.DefaultExt = "JHC";
            this.saveFileDialog.Filter = "JHBaseTerm files|*.JHC|All files|*.*";
            this.saveFileDialog.Title = "Save XML Settings File";
            // 
            // closeButton
            // 
            this.closeButton.Location = new System.Drawing.Point(373, 53);
            this.closeButton.Name = "closeButton";
            this.closeButton.Size = new System.Drawing.Size(109, 22);
            this.closeButton.TabIndex = 23;
            this.closeButton.Text = "Cancel";
            this.closeButton.Click += new System.EventHandler(this.closeButton_Click);
            // 
            // label1
            // 
            this.label1.Location = new System.Drawing.Point(355, 151);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(92, 22);
            this.label1.TabIndex = 24;
            this.label1.Text = "Defaults:";
            // 
            // defNohLinkLabel
            // 
            this.defNohLinkLabel.Location = new System.Drawing.Point(370, 200);
            this.defNohLinkLabel.Name = "defNohLinkLabel";
            this.defNohLinkLabel.Size = new System.Drawing.Size(120, 22);
            this.defNohLinkLabel.TabIndex = 25;
            this.defNohLinkLabel.TabStop = true;
            this.defNohLinkLabel.Text = "No Handshake";
            this.defNohLinkLabel.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.defNohLinkLabel_LinkClicked);
            // 
            // xonxoffLinkLabel
            // 
            this.xonxoffLinkLabel.Location = new System.Drawing.Point(370, 225);
            this.xonxoffLinkLabel.Name = "xonxoffLinkLabel";
            this.xonxoffLinkLabel.Size = new System.Drawing.Size(120, 22);
            this.xonxoffLinkLabel.TabIndex = 26;
            this.xonxoffLinkLabel.TabStop = true;
            this.xonxoffLinkLabel.Text = "Xon / Xoff";
            this.xonxoffLinkLabel.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.xonxoffLinkLabel_LinkClicked);
            // 
            // ctsrtsLinkLabel
            // 
            this.ctsrtsLinkLabel.Location = new System.Drawing.Point(370, 250);
            this.ctsrtsLinkLabel.Name = "ctsrtsLinkLabel";
            this.ctsrtsLinkLabel.Size = new System.Drawing.Size(120, 22);
            this.ctsrtsLinkLabel.TabIndex = 27;
            this.ctsrtsLinkLabel.TabStop = true;
            this.ctsrtsLinkLabel.Text = "CTS / RTS";
            this.ctsrtsLinkLabel.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.ctsrtsLinkLabel_LinkClicked);
            // 
            // dsrdtrLinkLabel
            // 
            this.dsrdtrLinkLabel.Location = new System.Drawing.Point(370, 275);
            this.dsrdtrLinkLabel.Name = "dsrdtrLinkLabel";
            this.dsrdtrLinkLabel.Size = new System.Drawing.Size(120, 22);
            this.dsrdtrLinkLabel.TabIndex = 28;
            this.dsrdtrLinkLabel.TabStop = true;
            this.dsrdtrLinkLabel.Text = "DSR / DTR";
            this.dsrdtrLinkLabel.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.dsrdtrLinkLabel_LinkClicked);
            // 
            // allDefaultsLinkLabel
            // 
            this.allDefaultsLinkLabel.Location = new System.Drawing.Point(370, 175);
            this.allDefaultsLinkLabel.Name = "allDefaultsLinkLabel";
            this.allDefaultsLinkLabel.Size = new System.Drawing.Size(120, 22);
            this.allDefaultsLinkLabel.TabIndex = 30;
            this.allDefaultsLinkLabel.TabStop = true;
            this.allDefaultsLinkLabel.Text = "All";
            this.allDefaultsLinkLabel.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.allDefaultsLinkLabel_LinkClicked);
            // 
            // okButton
            // 
            this.okButton.Location = new System.Drawing.Point(373, 18);
            this.okButton.Name = "okButton";
            this.okButton.Size = new System.Drawing.Size(109, 22);
            this.okButton.TabIndex = 31;
            this.okButton.Text = "OK";
            this.okButton.Click += new System.EventHandler(this.okButton_Click);
            // 
            // comboBoxPort
            // 
            this.comboBoxPort.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxPort.Items.AddRange(new object[] {
            "4",
            "5",
            "6",
            "7",
            "8"});
            this.comboBoxPort.Location = new System.Drawing.Point(131, 19);
            this.comboBoxPort.Name = "comboBoxPort";
            this.comboBoxPort.Size = new System.Drawing.Size(75, 21);
            this.comboBoxPort.TabIndex = 14;
            this.toolTip.SetToolTip(this.comboBoxPort, "Comm port used for communication");
            // 
            // comboBoxBaudRate
            // 
            this.comboBoxBaudRate.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxBaudRate.Items.AddRange(new object[] {
            "4",
            "5",
            "6",
            "7",
            "8"});
            this.comboBoxBaudRate.Location = new System.Drawing.Point(252, 18);
            this.comboBoxBaudRate.Name = "comboBoxBaudRate";
            this.comboBoxBaudRate.Size = new System.Drawing.Size(75, 21);
            this.comboBoxBaudRate.TabIndex = 15;
            this.toolTip.SetToolTip(this.comboBoxBaudRate, "Baud rate used for communication. Must match device baud rate.");
            // 
            // DlgPortSettings
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(491, 417);
            this.Controls.Add(this.okButton);
            this.Controls.Add(this.allDefaultsLinkLabel);
            this.Controls.Add(this.dsrdtrLinkLabel);
            this.Controls.Add(this.ctsrtsLinkLabel);
            this.Controls.Add(this.xonxoffLinkLabel);
            this.Controls.Add(this.defNohLinkLabel);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.closeButton);
            this.Controls.Add(this.checkBoxAR);
            this.Controls.Add(this.groupBox4);
            this.Controls.Add(this.groupBox3);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Name = "DlgPortSettings";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Serial Port Settings";
            this.Load += new System.EventHandler(this.DlgPortSettings_Load);
            this.groupBox1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownTC)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownTM)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownTxS)).EndInit();
            this.groupBox2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownRxS)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownHW)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownLW)).EndInit();
            this.groupBox3.ResumeLayout(false);
            this.groupBox4.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ComboBox comboBoxParity;
        private System.Windows.Forms.ComboBox comboBoxDB;
        private System.Windows.Forms.ComboBox comboBoxSB;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.CheckBox checkBoxCTS;
        private System.Windows.Forms.CheckBox checkBoxDSR;
        private System.Windows.Forms.CheckBox checkBoxTxX;
        private System.Windows.Forms.CheckBox checkBoxXC;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.ComboBox comboBoxRTS;
        private System.Windows.Forms.ComboBox comboBoxDTR;
        private System.Windows.Forms.CheckBox checkBoxRxX;
        private System.Windows.Forms.CheckBox checkBoxGD;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.NumericUpDown numericUpDownLW;
        private System.Windows.Forms.NumericUpDown numericUpDownHW;
        private System.Windows.Forms.NumericUpDown numericUpDownRxS;
        private System.Windows.Forms.NumericUpDown numericUpDownTxS;
        private System.Windows.Forms.GroupBox groupBox4;
        private System.Windows.Forms.NumericUpDown numericUpDownTC;
        private System.Windows.Forms.NumericUpDown numericUpDownTM;
        private System.Windows.Forms.CheckBox checkBoxAR;
        private System.Windows.Forms.ComboBox comboBoxXon;
        private System.Windows.Forms.ComboBox comboBoxXoff;
        private System.Windows.Forms.OpenFileDialog openFileDialog;
        private System.Windows.Forms.SaveFileDialog saveFileDialog;
        private System.Windows.Forms.CheckBox checkBoxCheck;
        private System.Windows.Forms.Label label19;
        private System.Windows.Forms.Label label20;
        private System.Windows.Forms.Label label21;
        private System.Windows.Forms.Label label22;
        private System.Windows.Forms.Label label23;
        private System.Windows.Forms.Label label24;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button closeButton;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.LinkLabel defNohLinkLabel;
        private System.Windows.Forms.LinkLabel xonxoffLinkLabel;
        private System.Windows.Forms.LinkLabel ctsrtsLinkLabel;
        private System.Windows.Forms.LinkLabel dsrdtrLinkLabel;
        private System.Windows.Forms.LinkLabel allDefaultsLinkLabel;
        private System.Windows.Forms.ToolTip toolTip;
        private System.Windows.Forms.Button okButton;
        private System.Windows.Forms.ComboBox comboBoxBaudRate;
        private System.Windows.Forms.ComboBox comboBoxPort;

    }
}