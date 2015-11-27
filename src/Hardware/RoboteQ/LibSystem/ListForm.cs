using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;

namespace LibSystem
{
	/// <summary>
	/// Summary description for ListForm.
	/// </summary>
	public class ListForm : System.Windows.Forms.Form
	{
		private System.Windows.Forms.TextBox textBox;
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		public ListForm()
		{
			InitializeComponent();
			this.Text = Project.PROGRAM_NAME_HUMAN + " - last up to " + Tracer.MAX_LOGGED + " diagnostic messages";

			Project.setDlgIcon(this);
		}

		public void setLine(string text, int i)
		{
			textBox.Lines[i] = text;
		}

		public void setText(string text)
		{
			textBox.Text = text;
		}

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		protected override void Dispose( bool disposing )
		{
			if( disposing )
			{
				if(components != null)
				{
					components.Dispose();
				}
			}
			base.Dispose( disposing );
		}

		#region Windows Form Designer generated code
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.textBox = new System.Windows.Forms.TextBox();
			this.SuspendLayout();
			// 
			// textBox
			// 
			this.textBox.AcceptsReturn = true;
			this.textBox.Dock = System.Windows.Forms.DockStyle.Fill;
			this.textBox.Multiline = true;
			this.textBox.Name = "textBox";
			this.textBox.ScrollBars = System.Windows.Forms.ScrollBars.Both;
			this.textBox.Size = new System.Drawing.Size(774, 575);
			this.textBox.TabIndex = 0;
			this.textBox.Text = "textBox1";
			// 
			// ListForm
			// 
			this.AutoScale = false;
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.ClientSize = new System.Drawing.Size(774, 575);
			this.Controls.AddRange(new System.Windows.Forms.Control[] {
																		  this.textBox});
			this.Name = "ListForm";
			this.Text = "ListForm";
			this.Load += new System.EventHandler(this.ListForm_Load);
			this.ResumeLayout(false);

		}
		#endregion

		private void ListForm_Load(object sender, System.EventArgs e)
		{
			scrollToEnd();
		}

		public void scrollToEnd()
		{
			// doesn't do the job for some reason. Selection happens all right, but the scroll does not:
			textBox.Select(textBox.Text.Length, 0);
			textBox.ScrollToCaret();
		}

		protected override bool ProcessDialogKey(Keys keyData)
		{
			bool keystrokeProcessed = true;
			switch (keyData) 
			{
				case Keys.Escape:
				case Keys.Alt | Keys.F4:
					break;
				default:
					keystrokeProcessed = false; // let KeyPress event handler handle this keystroke.
					break;
			}
			if(keystrokeProcessed) 
			{
				this.Close();
				return true;
			} 
			else 
			{
				return base.ProcessDialogKey(keyData);
			}
		}
	}
}
