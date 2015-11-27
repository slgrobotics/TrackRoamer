using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;

using LibSystem;
using OSC.NET;

namespace LibGui
{
	public partial class WiimoteValuesUserControl : UserControl
	{
		public OSCMessage values {
			set
			{
				int i = 0;

				valueNameLabel.Text = value.Address;

				StringBuilder strb = new StringBuilder();

				foreach (object obj in value.Values)
				{
					double beam;
					try
					{
						beam = Convert.ToDouble((float)obj);
					}
					catch
					{
						beam = 0.0d;
					}
					strb.Append(" ");
					string sVal = String.Format("{0:F3}", beam);
					int len = 10 - sVal.Length;
					while (len-- % 20 != 0)
					{
						strb.Append(" ");
					}
					strb.Append(sVal);
					i++;
				}

				this.valuesLabel.Text = strb.ToString();
			}
		}

		public WiimoteValuesUserControl()
		{
			InitializeComponent();
		}
	}
}
