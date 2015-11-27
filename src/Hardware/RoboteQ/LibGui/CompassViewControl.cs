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
	public partial class CompassViewControl : UserControl
	{
		double compassBearing = 0.0d;

		public CompassViewControl()
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
		DescriptionAttribute("Compass Bearing")
		]
		public double CompassBearing
		{
			get { return compassBearing; }
			set
			{
				compassBearing = value;
				this.Refresh();
			}
		}

		#endregion


		private void compassViewPanel_Paint(object sender, PaintEventArgs e)
		{
			Graphics g = e.Graphics;
			g.SmoothingMode = SmoothingMode.AntiAlias;
			DrawCompass dc = new DrawCompass(compassViewPanel);
			dc.DrawCompassView(g, compassBearing);
		}
	}
}
