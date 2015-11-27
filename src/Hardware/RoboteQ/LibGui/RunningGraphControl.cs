using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;

namespace LibGui
{
	public partial class RunningGraphControl : UserControl
	{
		private int m_lastX = -1;
		private int m_lastY = 0;
		private	int m_offsetX = 25;
		private Pen m_pen = new Pen(Color.LightGreen);
		private Pen m_penMinMax = new Pen(Color.DarkGray);
		private Pen m_penOverMax = new Pen(Color.Red);
		private Brush m_bgBrush = new SolidBrush(Color.Black);

		private Color m_bgColor = Color.Black;
		public Color BgColor {
			set {
				m_bgColor = value; m_bgBrush = new SolidBrush(m_bgColor); this.Refresh();
			}
		}

		private double m_maxValue = 20.0d;
		public double maxValue { get { return m_maxValue; } set { m_maxValue = value; this.maxValueLabel.Text = String.Format("{0}", value); } }

		private double m_minValue = 0.0d;
		public double minValue { get { return m_minValue; } set { m_minValue = value; this.minValueLabel.Text = String.Format("{0}", value); } }

		public RunningGraphControl()
		{
			InitializeComponent();

			this.minValueLabel.Text = "0";
		}

		public void Cleanup(Color bgColor)
		{
			BgColor = bgColor;
			m_lastX = -1;
		}

		public void plot(double val)
		{
			try
			{
				int height = this.ClientRectangle.Height;

				if (height > 10 && !Double.IsNaN(m_maxValue) && m_maxValue < 10000.0d)
				{
					if (m_lastX == -1)
					{
						BgColor = Color.Black;
						m_lastX = m_offsetX;
					}

					int x = m_lastX + 1;

					if (x >= this.ClientRectangle.Width - 2)
					{
						x = m_offsetX;
					}

					double scale = (height - 10) / (m_maxValue - m_minValue);
					int midY = height / 2;
					double midValue = (m_maxValue + m_minValue) / 2.0d;
					int maxY = (int)Math.Round((m_maxValue - midValue) * scale) + midY;
					int minY = (int)Math.Round((m_minValue - midValue) * scale) + midY;

					int y = (int)Math.Round(Math.Min((val - midValue) * scale + midY, (double)height));
					int offsetY = 0;

					Graphics g = this.CreateGraphics();
					g.FillRectangle(m_bgBrush, x, 0, 7, height);
					if ((x / 4) % 3 == 0)
					{
						g.DrawLine(m_penMinMax, x - 1, height - maxY - offsetY, x, height - maxY - offsetY);
						g.DrawLine(m_penMinMax, x - 1, height - minY - offsetY, x, height - minY - offsetY);
					}
					if (y <= maxY)
					{
						g.DrawLine(m_pen, x - 1, height - offsetY - m_lastY, x, height - offsetY - y);
					}
					else
					{
						g.DrawLine(m_penOverMax, x, height - offsetY, x, height - offsetY - y);
					}
					m_lastY = y;
					m_lastX = x;
				}
			}
			catch
			{
			}
		}

		private void RunningGraphControl_Paint(object sender, PaintEventArgs pe)
		{
			Graphics g = pe.Graphics;
			g.FillRectangle(m_bgBrush, this.ClientRectangle);

		}
	}
}
