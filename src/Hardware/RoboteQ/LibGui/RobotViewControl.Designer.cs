namespace LibGui
{
	partial class RobotViewControl
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

		#region Component Designer generated code

		/// <summary> 
		/// Required method for Designer support - do not modify 
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.robotViewPanel = new System.Windows.Forms.Panel();
			this.SuspendLayout();
			// 
			// robotViewPanel
			// 
			this.robotViewPanel.Dock = System.Windows.Forms.DockStyle.Fill;
			this.robotViewPanel.Location = new System.Drawing.Point(0, 0);
			this.robotViewPanel.Name = "robotViewPanel";
			this.robotViewPanel.Size = new System.Drawing.Size(377, 329);
			this.robotViewPanel.TabIndex = 0;
			this.robotViewPanel.Paint += new System.Windows.Forms.PaintEventHandler(this.robotViewPanel_Paint);
			// 
			// RobotViewControl
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.robotViewPanel);
			this.Name = "RobotViewControl";
			this.Size = new System.Drawing.Size(377, 329);
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.Panel robotViewPanel;
	}
}
