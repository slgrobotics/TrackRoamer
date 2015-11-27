using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Data;
using System.Text;
using System.Windows.Forms;

using LibSystem;

namespace LibGui
{
	public delegate void HitRedZone(object sender, EventArgs e);

	[DefaultEventAttribute("RedZoneEvent")]
	public partial class SonarViewControl : UserControl
	{
		public SonarData sonarData = new SonarData();

        public bool flipped = false;

		public event HitRedZone RedZoneEvent;
		private int center;
		private int centerX;
		private int centerY;

		private float redZone;
		private Color arrowColor;
		private int numNumbers;
		private int angleRawCurr = 0;
		private long timestampLastReading = 0L;

		public SonarViewControl()
		{
			InitializeComponent();

			this.BackColor = Color.Black;
			this.SetStyle(ControlStyles.ResizeRedraw, true);
			this.SetStyle(ControlStyles.DoubleBuffer, true);
			this.SetStyle(ControlStyles.AllPaintingInWmPaint, true);
			this.SetStyle(ControlStyles.UserPaint, true);
			this.RedZone = 360;
			this.arrowColor = System.Drawing.Color.Red;
			this.numNumbers = 10;
		}

        public void Reset()
        {
            sonarData = new SonarData();
        }

		public void setReading(int angleRaw, double rangeMeters, long timestamp)
		{
			Tracer.Trace("sonar: " + angleRaw + "   " + rangeMeters);

			angleRawCurr = angleRaw;
            timestampLastReading = timestamp;

			sonarData.addRangeReading(angleRaw, rangeMeters);

			Tracer.Trace("angles: " + sonarData.angles.Count);

			NumNumbers = sonarData.angles.Count;

		}

        public void setReading(int angleRaw1, double rangeMeters1, int angleRaw2, double rangeMeters2, long timestamp)
		{
            Tracer.Trace("sonar: " + angleRaw1 + "   " + rangeMeters1 + "  " + angleRaw2 + "   " + rangeMeters2);

            timestampLastReading = timestamp;

			angleRawCurr = angleRaw1;

			sonarData.addRangeReading(angleRaw1, rangeMeters1);

			NumNumbers = sonarData.angles.Count;

			angleRawCurr = angleRaw2;

			sonarData.addRangeReading(angleRaw2, rangeMeters2);

			Tracer.Trace("angles: " + sonarData.angles.Count);

			NumNumbers = sonarData.angles.Count;
		}

		#region Custom Events
		/// <summary>
		/// Custom events that appear in Properties browser
		/// </summary>

		private void RedZoneHit()
		{
			if (RedZoneEvent != null)
				RedZoneEvent(this, new System.EventArgs());
		}

		#endregion

		#region Custom Properties
		/// <summary>
		/// 
		/// New properties, available in in Designer's properties browser
		/// 
		/// </summary>

		[
		CategoryAttribute("Appearance"),
		DescriptionAttribute("The angle at which the red zone of the dial starts.  RedZone event triggered when arm enters red zone.")
		]
		public float RedZone
		{
			get { return redZone; }
			set
			{
				redZone = value;
				this.Refresh();
			}
		}

		[
		CategoryAttribute("Appearance"),
		DescriptionAttribute("Initial color of arm")
		]
		public Color ArrowColor
		{
			get { return arrowColor; }
			set
			{
				arrowColor = value;
				this.Refresh();
			}
		}

		[
		CategoryAttribute("Appearance"),
		DescriptionAttribute("the number of markings that appear on the dial")
		]
		public int NumNumbers
		{
			get { return numNumbers; }
			set
			{
				//if(numNumbers != value)
				{
					numNumbers = value;
					this.Refresh();
				}
			}
		}

		#endregion

		private void sonarViewPanel_Paint(object sender, PaintEventArgs e)
		{
			this.center = (int)Math.Min(ClientRectangle.Width, ClientRectangle.Height);

			centerX = ClientRectangle.Width / 2;
			centerY = ClientRectangle.Height - ClientRectangle.Height / 10;
			//center = center / 2;
			center = centerY;

			PointF centerPt = new PointF(centerX, ClientRectangle.Height - ClientRectangle.Height / 20);

			Graphics g = e.Graphics;
			
			using (GraphicsPath gp = new GraphicsPath())
			{
				gp.AddEllipse(0, 0, 2 * centerX, 2 * centerY);
				this.Region = new Region(gp);

				if (numNumbers >= 2)
				{
					using (Brush brush = new SolidBrush(Color.Yellow))
					{
						int i = 0;

						float angleStep = 180.0f / (numNumbers - 1);		// sweep range in sectors by sector size
						float angle = flipped ? 0.0f : 180.0f;				// first ray direction

						foreach (int key in sonarData.angles.Keys)
						{
							Matrix matrix = new Matrix();
							matrix.RotateAt(angle - 90.0f, centerPt);
							g.Transform = matrix;

							int angleRaw = (int)sonarData.angles[key];

							RangeReading reading = sonarData.getLatestReadingAt(angleRaw);

							double rangeCm = 0.0d;

							if (reading != null)
							{
								rangeCm = Math.Round(reading.rangeMeters * 100);

								// 0 = full range,  centerY = zero range
								int lineLength = (int)((300.0d - rangeCm) * centerY / 300.0d);

								if (angleRaw == angleRawCurr && (DateTime.Now.Ticks - timestampLastReading) < 300000000L)
								{
									g.DrawLine(Pens.Red, centerX, centerY, centerX, lineLength);
								}
								else
								{
									g.DrawLine(Pens.Yellow, centerX, centerY, centerX, lineLength);
								}

								g.DrawString(String.Format("{0:f0}", rangeCm),
									this.Font, brush,
									centerX - 6, centerX * 5 / 100,
									StringFormat.GenericTypographic);
							}
							else
							{
								g.DrawString("*",
									this.Font, brush,
									centerX - 6, centerX * 5 / 100,
									StringFormat.GenericTypographic);
							}

                            if (flipped)
                            {
                                angle += angleStep;
                            }
                            else
                            {
                                angle -= angleStep;
                            }
                            i++;
						}

					}
				}
			}

			//using (GraphicsPath gp2 = new GraphicsPath())
			//{
			//    using (Pen pen = new Pen(arrowColor, 12))
			//    {
			//        Matrix matrix = new Matrix();
			//        matrix.RotateAt(0, centerPt);
			//        g.Transform = matrix;
			//        pen.EndCap = LineCap.ArrowAnchor;
			//        g.DrawLine(pen, centerX, centerY, centerX, centerY / 8);
			//        //g.DrawLine(pen, center, center, (center * 9) / 10, center);
			//        //g.DrawLine(pen, center, center, (center * 11) / 10, center);
			//        //g.DrawLine(pen, center, center, center, (center * 11) / 10);
			//    }
			//}
		}
	}
}
