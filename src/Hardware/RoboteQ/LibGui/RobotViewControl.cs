using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Data;
using System.Text;
using System.Windows.Forms;

using Lib3DDraw;

namespace LibGui
{
	public partial class RobotViewControl : UserControl
	{
		private double accelerationX = 0.0d;
		private double accelerationY = 0.0d;
		private double accelerationZ = 0.0d;

		public RobotViewControl()
		{
			InitializeComponent();

			this.SetStyle(ControlStyles.ResizeRedraw, true);
			this.SetStyle(ControlStyles.DoubleBuffer, true);
			this.SetStyle(ControlStyles.AllPaintingInWmPaint, true);
			this.SetStyle(ControlStyles.UserPaint, true);
		}

		#region Custom Properties
		/// <summary>
		/// 
		/// New properties, available in in Designer's properties browser
		/// 
		/// </summary>

		[
		CategoryAttribute("Appearance"),
		DescriptionAttribute("Acceleration X")
		]
		public double AccelerationX
		{
			get { return accelerationX; }
			set
			{
				accelerationX = value;
				this.Refresh();
			}
		}

		[
		CategoryAttribute("Appearance"),
		DescriptionAttribute("Acceleration Y")
		]
		public double AccelerationY
		{
			get { return accelerationY; }
			set
			{
				accelerationY = value;
				this.Refresh();
			}
		}

		[
		CategoryAttribute("Appearance"),
		DescriptionAttribute("Acceleration Z")
		]
		public double AccelerationZ
		{
			get { return accelerationZ; }
			set
			{
				accelerationZ = value;
				this.Refresh();
			}
		}

		#endregion

		public void setAccelerometerData(double accX, double accY, double accZ)
		{
			gTotal = Math.Sqrt(accX*accX + accY*accY + accZ*accZ);

			thetaY = (float)Math.Acos(accY / gTotal) * 180.0f / (float)Math.PI;
			thetaZ = (float)Math.Atan(accZ / accX) * 180.0f / (float)Math.PI;

			this.Refresh();
		}

		private Font font = new Font(FontFamily.GenericSansSerif, 10f);
		private Font fontB = new Font(FontFamily.GenericSansSerif, 12f, FontStyle.Bold);

		private double gTotal = 10.0d;

		private float thetaX = 0.0f;
		private float thetaY = 0.0f;
		private float thetaZ = 0.0f;

		private void robotViewPanel_Paint(object sender, PaintEventArgs e)
		{
			Graphics g = e.Graphics;
			g.SmoothingMode = SmoothingMode.AntiAlias;
			DrawRobot dr = new DrawRobot(this.robotViewPanel);

			dr.thetaX = thetaX;
			dr.thetaY = thetaY;
			dr.thetaZ = thetaZ;

			dr.DrawRobotView(g);

			g.DrawString(String.Format("{0:f2}",gTotal), fontB, Brushes.Red, 20f, 20f);
			g.DrawString(String.Format("{0:f2} {1:f2} {2:f2}", thetaX, thetaY, thetaZ), font, Brushes.Red, 20f, 40f);
		}
	}
}
