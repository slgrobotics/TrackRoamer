namespace LibGui
{
	partial class CompassViewControl
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
			this.compassViewPanel = new System.Windows.Forms.Panel();
			this.SuspendLayout();
			// 
			// compassViewPanel
			// 
			this.compassViewPanel.Dock = System.Windows.Forms.DockStyle.Fill;
			this.compassViewPanel.Location = new System.Drawing.Point(0, 0);
			this.compassViewPanel.Name = "compassViewPanel";
			this.compassViewPanel.Size = new System.Drawing.Size(357, 337);
			this.compassViewPanel.TabIndex = 0;
			this.compassViewPanel.Paint += new System.Windows.Forms.PaintEventHandler(this.compassViewPanel_Paint);
			// 
			// CompassViewControl
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.compassViewPanel);
			this.Name = "CompassViewControl";
			this.Size = new System.Drawing.Size(357, 337);
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.Panel compassViewPanel;
	}
}
