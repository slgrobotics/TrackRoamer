using System;
using System.Drawing;
using System.Windows.Forms;

namespace Lib3DDraw
{
	public class Draw3DObject
	{
		protected Panel panel;

		// default view positioning:
		protected float elevation = 30;
		protected float azimuth = -37.5f;
		protected float oneOverdFactor = 0.4f;

		internal Matrix3 matrix;

		public Draw3DObject(Panel _panel)
		{
			panel = _panel;

			float oneOverd = oneOverdFactor / (2 * panel.Height / 4);
			matrix = Matrix3.AzimuthElevation(elevation, azimuth, oneOverd);
		}

		protected PointF Point2D(PointF pt)
		{
			PointF aPoint = new PointF();
			aPoint.X = panel.Width / 2 + pt.X;
			aPoint.Y = panel.Height / 2 - pt.Y;
			return aPoint;
		}
	}
}
