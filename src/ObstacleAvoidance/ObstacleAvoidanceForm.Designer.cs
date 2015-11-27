namespace Microsoft.Robotics.Services.ObstacleAvoidanceDrive
{
    partial class ObstacleAvoidanceForm
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
            this.depthProfileCtrl = new System.Windows.Forms.PictureBox();
            this.PIDControllerGroupBox = new System.Windows.Forms.GroupBox();
            this.buttonSaveState = new System.Windows.Forms.Button();
            this.buttonUpdatePidControllers = new System.Windows.Forms.Button();
            this.textBoxAngularIntegralMax = new System.Windows.Forms.TextBox();
            this.label6 = new System.Windows.Forms.Label();
            this.textBoxAngularMin = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.textBoxAngularMax = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.textBoxAngularKd = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.textBoxAngularKi = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.textBoxAngularKp = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.depthProfileCtrl)).BeginInit();
            this.PIDControllerGroupBox.SuspendLayout();
            this.SuspendLayout();
            // 
            // depthProfileCtrl
            // 
            this.depthProfileCtrl.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.depthProfileCtrl.Location = new System.Drawing.Point(3, 1);
            this.depthProfileCtrl.Name = "depthProfileCtrl";
            this.depthProfileCtrl.Size = new System.Drawing.Size(612, 508);
            this.depthProfileCtrl.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.depthProfileCtrl.TabIndex = 1;
            this.depthProfileCtrl.TabStop = false;
            // 
            // PIDControllerGroupBox
            // 
            this.PIDControllerGroupBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.PIDControllerGroupBox.Controls.Add(this.buttonSaveState);
            this.PIDControllerGroupBox.Controls.Add(this.buttonUpdatePidControllers);
            this.PIDControllerGroupBox.Controls.Add(this.textBoxAngularIntegralMax);
            this.PIDControllerGroupBox.Controls.Add(this.label6);
            this.PIDControllerGroupBox.Controls.Add(this.textBoxAngularMin);
            this.PIDControllerGroupBox.Controls.Add(this.label5);
            this.PIDControllerGroupBox.Controls.Add(this.textBoxAngularMax);
            this.PIDControllerGroupBox.Controls.Add(this.label4);
            this.PIDControllerGroupBox.Controls.Add(this.textBoxAngularKd);
            this.PIDControllerGroupBox.Controls.Add(this.label3);
            this.PIDControllerGroupBox.Controls.Add(this.textBoxAngularKi);
            this.PIDControllerGroupBox.Controls.Add(this.label2);
            this.PIDControllerGroupBox.Controls.Add(this.textBoxAngularKp);
            this.PIDControllerGroupBox.Controls.Add(this.label1);
            this.PIDControllerGroupBox.Location = new System.Drawing.Point(3, 515);
            this.PIDControllerGroupBox.Name = "PIDControllerGroupBox";
            this.PIDControllerGroupBox.Size = new System.Drawing.Size(612, 119);
            this.PIDControllerGroupBox.TabIndex = 22;
            this.PIDControllerGroupBox.TabStop = false;
            this.PIDControllerGroupBox.Text = "Angular Component PID Controller";
            // 
            // buttonSaveState
            // 
            this.buttonSaveState.Location = new System.Drawing.Point(483, 52);
            this.buttonSaveState.Name = "buttonSaveState";
            this.buttonSaveState.Size = new System.Drawing.Size(119, 23);
            this.buttonSaveState.TabIndex = 13;
            this.buttonSaveState.Text = "Apply && Save State";
            this.buttonSaveState.UseVisualStyleBackColor = true;
            this.buttonSaveState.Click += new System.EventHandler(this.buttonSaveState_Click);
            // 
            // buttonUpdatePidControllers
            // 
            this.buttonUpdatePidControllers.Location = new System.Drawing.Point(483, 19);
            this.buttonUpdatePidControllers.Name = "buttonUpdatePidControllers";
            this.buttonUpdatePidControllers.Size = new System.Drawing.Size(119, 23);
            this.buttonUpdatePidControllers.TabIndex = 12;
            this.buttonUpdatePidControllers.Text = "Apply";
            this.buttonUpdatePidControllers.UseVisualStyleBackColor = true;
            this.buttonUpdatePidControllers.Click += new System.EventHandler(this.buttonUpdatePidControllers_Click);
            // 
            // textBoxAngularIntegralMax
            // 
            this.textBoxAngularIntegralMax.Location = new System.Drawing.Point(350, 85);
            this.textBoxAngularIntegralMax.Name = "textBoxAngularIntegralMax";
            this.textBoxAngularIntegralMax.Size = new System.Drawing.Size(100, 20);
            this.textBoxAngularIntegralMax.TabIndex = 11;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(274, 88);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(68, 13);
            this.label6.TabIndex = 10;
            this.label6.Text = "Integral Max:";
            // 
            // textBoxAngularMin
            // 
            this.textBoxAngularMin.Location = new System.Drawing.Point(350, 52);
            this.textBoxAngularMin.Name = "textBoxAngularMin";
            this.textBoxAngularMin.Size = new System.Drawing.Size(100, 20);
            this.textBoxAngularMin.TabIndex = 9;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(315, 55);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(27, 13);
            this.label5.TabIndex = 8;
            this.label5.Text = "Min:";
            // 
            // textBoxAngularMax
            // 
            this.textBoxAngularMax.Location = new System.Drawing.Point(350, 19);
            this.textBoxAngularMax.Name = "textBoxAngularMax";
            this.textBoxAngularMax.Size = new System.Drawing.Size(100, 20);
            this.textBoxAngularMax.TabIndex = 7;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(313, 22);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(30, 13);
            this.label4.TabIndex = 6;
            this.label4.Text = "Max:";
            // 
            // textBoxAngularKd
            // 
            this.textBoxAngularKd.Location = new System.Drawing.Point(113, 85);
            this.textBoxAngularKd.Name = "textBoxAngularKd";
            this.textBoxAngularKd.Size = new System.Drawing.Size(100, 20);
            this.textBoxAngularKd.TabIndex = 5;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(32, 88);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(66, 13);
            this.label3.TabIndex = 4;
            this.label3.Text = "K derivative:";
            // 
            // textBoxAngularKi
            // 
            this.textBoxAngularKi.Location = new System.Drawing.Point(113, 52);
            this.textBoxAngularKi.Name = "textBoxAngularKi";
            this.textBoxAngularKi.Size = new System.Drawing.Size(100, 20);
            this.textBoxAngularKi.TabIndex = 3;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(32, 55);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(54, 13);
            this.label2.TabIndex = 2;
            this.label2.Text = "K integral:";
            // 
            // textBoxAngularKp
            // 
            this.textBoxAngularKp.Location = new System.Drawing.Point(113, 19);
            this.textBoxAngularKp.Name = "textBoxAngularKp";
            this.textBoxAngularKp.Size = new System.Drawing.Size(100, 20);
            this.textBoxAngularKp.TabIndex = 1;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(32, 22);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(75, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "K proportional:";
            // 
            // ObstacleAvoidanceForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.GradientActiveCaption;
            this.ClientSize = new System.Drawing.Size(617, 637);
            this.Controls.Add(this.PIDControllerGroupBox);
            this.Controls.Add(this.depthProfileCtrl);
            this.MinimumSize = new System.Drawing.Size(625, 544);
            this.Name = "ObstacleAvoidanceForm";
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.Text = "Obstacle Avoidance";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.ObstacleAvoidanceForm_FormClosed);
            this.Load += new System.EventHandler(this.ObstacleAvoidanceForm_Load);
            ((System.ComponentModel.ISupportInitialize)(this.depthProfileCtrl)).EndInit();
            this.PIDControllerGroupBox.ResumeLayout(false);
            this.PIDControllerGroupBox.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.PictureBox depthProfileCtrl;
        private System.Windows.Forms.GroupBox PIDControllerGroupBox;
        private System.Windows.Forms.Button buttonSaveState;
        private System.Windows.Forms.Button buttonUpdatePidControllers;
        private System.Windows.Forms.TextBox textBoxAngularIntegralMax;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.TextBox textBoxAngularMin;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TextBox textBoxAngularMax;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox textBoxAngularKd;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox textBoxAngularKi;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox textBoxAngularKp;
        private System.Windows.Forms.Label label1;
    }
}