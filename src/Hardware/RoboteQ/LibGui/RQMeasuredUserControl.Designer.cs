namespace LibGui
{
	partial class RQMeasuredUserControl
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
			this.nameLabel = new System.Windows.Forms.Label();
			this.valueLabel = new System.Windows.Forms.Label();
			this.extraLabel = new System.Windows.Forms.Label();
			this.runningGraphControl = new LibGui.RunningGraphControl();
			this.SuspendLayout();
			// 
			// nameLabel
			// 
			this.nameLabel.AutoSize = true;
			this.nameLabel.Location = new System.Drawing.Point(3, 0);
			this.nameLabel.Name = "nameLabel";
			this.nameLabel.Size = new System.Drawing.Size(33, 13);
			this.nameLabel.TabIndex = 0;
			this.nameLabel.Text = "name";
			// 
			// valueLabel
			// 
			this.valueLabel.Font = new System.Drawing.Font("Arial", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.valueLabel.Location = new System.Drawing.Point(3, 15);
			this.valueLabel.Name = "valueLabel";
			this.valueLabel.Size = new System.Drawing.Size(132, 26);
			this.valueLabel.TabIndex = 1;
			this.valueLabel.Text = "value";
			// 
			// extraLabel
			// 
			this.extraLabel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.extraLabel.Location = new System.Drawing.Point(161, 0);
			this.extraLabel.Name = "extraLabel";
			this.extraLabel.Size = new System.Drawing.Size(108, 13);
			this.extraLabel.TabIndex = 2;
			this.extraLabel.Text = "extra";
			this.extraLabel.TextAlign = System.Drawing.ContentAlignment.TopRight;
			// 
			// runningGraphControl
			// 
			this.runningGraphControl.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.runningGraphControl.Location = new System.Drawing.Point(134, 15);
			this.runningGraphControl.Name = "runningGraphControl";
			this.runningGraphControl.Size = new System.Drawing.Size(138, 26);
			this.runningGraphControl.TabIndex = 3;
			// 
			// RQMeasuredUserControl
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.runningGraphControl);
			this.Controls.Add(this.extraLabel);
			this.Controls.Add(this.valueLabel);
			this.Controls.Add(this.nameLabel);
			this.Name = "RQMeasuredUserControl";
			this.Size = new System.Drawing.Size(272, 46);
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.Label nameLabel;
		private System.Windows.Forms.Label valueLabel;
		private System.Windows.Forms.Label extraLabel;
		private RunningGraphControl runningGraphControl;
	}
}
