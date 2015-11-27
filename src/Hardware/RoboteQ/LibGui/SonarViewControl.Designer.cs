namespace LibGui
{
	partial class SonarViewControl
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
			this.sonarViewPanel = new System.Windows.Forms.Panel();
			this.SuspendLayout();
			// 
			// sonarViewPanel
			// 
			this.sonarViewPanel.Dock = System.Windows.Forms.DockStyle.Fill;
			this.sonarViewPanel.Location = new System.Drawing.Point(0, 0);
			this.sonarViewPanel.Name = "sonarViewPanel";
			this.sonarViewPanel.Size = new System.Drawing.Size(516, 447);
			this.sonarViewPanel.TabIndex = 0;
			this.sonarViewPanel.Paint += new System.Windows.Forms.PaintEventHandler(this.sonarViewPanel_Paint);
			// 
			// SonarViewControl
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.sonarViewPanel);
			this.Name = "SonarViewControl";
			this.Size = new System.Drawing.Size(516, 447);
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.Panel sonarViewPanel;
	}
}
