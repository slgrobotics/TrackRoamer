using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;

using LibRoboteqController;

namespace LibGui
{
	public partial class RQMeasuredUserControl : UserControl
	{
		private const int tooOldSec = 2;
		private Color m_backColorOrig;

		public string valueName;

		private double m_maxValue = double.NaN;
		public double maxValue { get { return m_maxValue; } set { m_maxValue = value; this.runningGraphControl.maxValue = m_maxValue; } }

		private double m_minValue = double.NaN;
		public double minValue { get { return m_minValue; } set { m_minValue = value; this.runningGraphControl.minValue = m_minValue; } }

		public double factor = 1.0d;

		public RQMeasuredValue measured
		{
			set
			{
				if (value == null)
				{
					this.valueLabel.Text = "N/A";
				}
				else
				{
					double dValue = value.doubleValue * factor;

					double howOldSec = (DateTime.Now.Ticks - value.timestamp) / 10000000.0d;

					if (howOldSec > (double)tooOldSec)
					{
						this.BackColor = Color.Yellow;
					}
					else
					{
						this.BackColor = m_backColorOrig;
					}

					if (!double.IsNaN(maxValue) && dValue > maxValue || !double.IsNaN(minValue) && dValue < minValue)
					{
						this.valueLabel.BackColor = Color.Red;
					}
					else
					{
						this.valueLabel.BackColor = Color.LightGreen;
					}


					this.nameLabel.Text = value.valueName;
					this.extraLabel.Text = value.stringValue + "   " + value.intValue + "    " + String.Format("{0:F2}", howOldSec);
					this.valueLabel.Text = String.Format("{0:F3}", dValue).Replace(".000", "");

					this.runningGraphControl.plot(dValue);
				}
			}
		}

		public RQMeasuredUserControl()
		{
			InitializeComponent();

			m_backColorOrig = Color.Wheat; // this.BackColor;

			Cleanup();
		}

		public void Cleanup()
		{
			this.nameLabel.Text = "";
			this.extraLabel.Text = "";
			this.valueLabel.Text = "";

			this.BackColor = this.valueLabel.BackColor = Color.WhiteSmoke;
			this.runningGraphControl.Cleanup(this.BackColor);
		}
	}
}
