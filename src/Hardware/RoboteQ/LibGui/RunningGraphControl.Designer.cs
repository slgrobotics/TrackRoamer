namespace LibGui
{
	partial class RunningGraphControl
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
			this.maxValueLabel = new System.Windows.Forms.Label();
			this.minValueLabel = new System.Windows.Forms.Label();
			this.panel1 = new System.Windows.Forms.Panel();
			this.panel1.SuspendLayout();
			this.SuspendLayout();
			// 
			// maxValueLabel
			// 
			this.maxValueLabel.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.maxValueLabel.BackColor = System.Drawing.Color.Transparent;
			this.maxValueLabel.Font = new System.Drawing.Font("Verdana", 6F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.maxValueLabel.ForeColor = System.Drawing.Color.Yellow;
			this.maxValueLabel.Location = new System.Drawing.Point(0, 0);
			this.maxValueLabel.Name = "maxValueLabel";
			this.maxValueLabel.Size = new System.Drawing.Size(25, 10);
			this.maxValueLabel.TabIndex = 0;
			this.maxValueLabel.Text = "-";
			this.maxValueLabel.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// minValueLabel
			// 
			this.minValueLabel.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.minValueLabel.BackColor = System.Drawing.Color.Transparent;
			this.minValueLabel.Font = new System.Drawing.Font("Verdana", 6F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.minValueLabel.ForeColor = System.Drawing.Color.Yellow;
			this.minValueLabel.Location = new System.Drawing.Point(0, 140);
			this.minValueLabel.Name = "minValueLabel";
			this.minValueLabel.Size = new System.Drawing.Size(25, 13);
			this.minValueLabel.TabIndex = 1;
			this.minValueLabel.Text = "-";
			this.minValueLabel.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// panel1
			// 
			this.panel1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
						| System.Windows.Forms.AnchorStyles.Left)));
			this.panel1.BackColor = System.Drawing.Color.Transparent;
			this.panel1.Controls.Add(this.maxValueLabel);
			this.panel1.Controls.Add(this.minValueLabel);
			this.panel1.Location = new System.Drawing.Point(0, 0);
			this.panel1.Name = "panel1";
			this.panel1.Size = new System.Drawing.Size(25, 150);
			this.panel1.TabIndex = 2;
			// 
			// RunningGraphControl
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.panel1);
			this.Name = "RunningGraphControl";
			this.Size = new System.Drawing.Size(517, 150);
			this.Paint += new System.Windows.Forms.PaintEventHandler(this.RunningGraphControl_Paint);
			this.panel1.ResumeLayout(false);
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.Label maxValueLabel;
		private System.Windows.Forms.Label minValueLabel;
		private System.Windows.Forms.Panel panel1;
	}
}
