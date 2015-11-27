using System;
using System.Drawing;
using System.Windows.Forms;

namespace Lib3DDraw
{
	public class DrawCoordinateAxes : Draw3DObject
	{
		protected float side = 10;

		public DrawCoordinateAxes(Panel _panel, float _side)
			: base(_panel)
		{
			side = _side;

			// override default matrix based on side:
			float oneOverd = oneOverdFactor / (2 * side);
			matrix = Matrix3.AzimuthElevation(elevation, azimuth, oneOverd);
		}

		public Point3[] AxesCoordinates()
		{
			Point3[] pts = new Point3[4];

			// Create coordinate axes:
			pts[0] = new Point3(2 * side, -side, -side, 1);
			pts[1] = new Point3(-side, 2 * side, -side, 1);
			pts[2] = new Point3(-side, -side, 2 * side, 1);
			pts[3] = new Point3(-side, -side, -side, 1);

			return pts;
		}

		public void AddAxes(Graphics g)
		{
			Point3[] pts = AxesCoordinates();
			for (int i = 0; i < pts.Length; i++)
			{
				pts[i].TransformNormalize(matrix);
			}

			// Create coordinate axes:
			PointF[] pta = new PointF[2];
			pta[0] = Point2D(new PointF(pts[3].X, pts[3].Y));
			pta[1] = Point2D(new PointF(pts[0].X, pts[0].Y));
			g.DrawLine(Pens.Red, pta[0], pta[1]);
			pta[1] = Point2D(new PointF(pts[1].X, pts[1].Y));
			g.DrawLine(Pens.Green, pta[0], pta[1]);
			pta[1] = Point2D(new PointF(pts[2].X, pts[2].Y));
			g.DrawLine(Pens.Blue, pta[0], pta[1]);

		}

	}
}
