namespace LibGui
{
	partial class WiimoteValuesUserControl
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
			this.valueNameLabel = new System.Windows.Forms.Label();
			this.valuesLabel = new System.Windows.Forms.Label();
			this.SuspendLayout();
			// 
			// valueNameLabel
			// 
			this.valueNameLabel.Location = new System.Drawing.Point(4, 4);
			this.valueNameLabel.Name = "valueNameLabel";
			this.valueNameLabel.Size = new System.Drawing.Size(124, 23);
			this.valueNameLabel.TabIndex = 0;
			this.valueNameLabel.Text = "name";
			this.valueNameLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// valuesLabel
			// 
			this.valuesLabel.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.valuesLabel.Font = new System.Drawing.Font("Courier New", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.valuesLabel.Location = new System.Drawing.Point(147, 4);
			this.valuesLabel.Name = "valuesLabel";
			this.valuesLabel.Size = new System.Drawing.Size(376, 23);
			this.valuesLabel.TabIndex = 1;
			this.valuesLabel.Text = "values";
			this.valuesLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// WiimoteValuesUserControl
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.valuesLabel);
			this.Controls.Add(this.valueNameLabel);
			this.Name = "WiimoteValuesUserControl";
			this.Size = new System.Drawing.Size(526, 30);
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.Label valueNameLabel;
		private System.Windows.Forms.Label valuesLabel;
	}
}
