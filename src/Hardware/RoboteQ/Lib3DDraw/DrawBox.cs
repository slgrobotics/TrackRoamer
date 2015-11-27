using System;
using System.Drawing;
using System.Windows.Forms;

namespace Lib3DDraw
{
	public class DrawBox : Draw3DObject
	{
		protected float side = 10;

		public DrawBox(Panel _panel)
			: base(_panel)
		{
			// override default matrix based on side:
			float oneOverd = oneOverdFactor / (2 * side);
			matrix = Matrix3.AzimuthElevation(elevation, azimuth, oneOverd);
		}

		public DrawBox(Panel _panel, float _side)
			: this(_panel)
		{
			side = _side;
		}

		public Point3[] BoxCoordinates()
		{
			Point3[] pts = new Point3[8];

			// Create the Box:
			pts[0] = new Point3(side, -side, -side, 1);
			pts[1] = new Point3(side, side, -side, 1);
			pts[2] = new Point3(-side, side, -side, 1);
			pts[3] = new Point3(-side, -side, -side, 1);
			pts[4] = new Point3(-side, -side, side, 1);
			pts[5] = new Point3(side, -side, side, 1);
			pts[6] = new Point3(side, side, side, 1);
			pts[7] = new Point3(-side, side, side, 1);

			return pts;
		}

		public void AddBox(Graphics g)
		{
			Point3[] pts = BoxCoordinates();
			PointF[] pta = new PointF[4];
			for (int i = 0; i < pts.Length; i++)
			{
				pts[i].TransformNormalize(matrix);
			}

			int[] i0, i1;
			i0 = new int[4] { 1, 2, 7, 6 };
			i1 = new int[4] { 2, 3, 4, 7 };
			if (elevation >= 0)
			{
				if (azimuth >= -180 && azimuth < -90)
				{
					i0 = new int[4] { 1, 2, 7, 6 };
					i1 = new int[4] { 2, 3, 4, 7 };
				}
				else if (azimuth >= -90 && azimuth < 0)
				{
					i0 = new int[4] { 3, 4, 5, 0 };
					i1 = new int[4] { 2, 3, 4, 7 };
				}
				else if (azimuth >= 0 && azimuth < 90)
				{
					i0 = new int[4] { 3, 4, 5, 0 };
					i1 = new int[4] { 0, 1, 6, 5 };
				}
				else if (azimuth >= 90 && azimuth <= 180)
				{
					i0 = new int[4] { 1, 2, 7, 6 };
					i1 = new int[4] { 0, 1, 6, 5 };
				}
			}
			else if (elevation < 0)
			{
				if (azimuth >= -180 && azimuth < -90)
				{
					i0 = new int[4] { 0, 1, 6, 5 };
					i1 = new int[4] { 0, 3, 4, 5 };
				}
				else if (azimuth >= -90 && azimuth < 0)
				{
					i0 = new int[4] { 1, 2, 7, 6 };
					i1 = new int[4] { 0, 1, 6, 5 };
				}
				else if (azimuth >= 0 && azimuth < 90)
				{
					i0 = new int[4] { 2, 3, 4, 7 };
					i1 = new int[4] { 1, 2, 7, 6 };
				}
				else if (azimuth >= 90 && azimuth <= 180)
				{
					i0 = new int[4] { 2, 3, 4, 7 };
					i1 = new int[4] { 0, 3, 4, 5 };
				}

			}

			pta[0] = Point2D(new PointF(pts[i0[0]].X, pts[i0[0]].Y));
			pta[1] = Point2D(new PointF(pts[i0[1]].X, pts[i0[1]].Y));
			pta[2] = Point2D(new PointF(pts[i0[2]].X, pts[i0[2]].Y));
			pta[3] = Point2D(new PointF(pts[i0[3]].X, pts[i0[3]].Y));
			//g.FillPolygon(Brushes.LightCoral, pta);
			g.DrawPolygon(Pens.Black, pta);
			pta[0] = Point2D(new PointF(pts[i1[0]].X, pts[i1[0]].Y));
			pta[1] = Point2D(new PointF(pts[i1[1]].X, pts[i1[1]].Y));
			pta[2] = Point2D(new PointF(pts[i1[2]].X, pts[i1[2]].Y));
			pta[3] = Point2D(new PointF(pts[i1[3]].X, pts[i1[3]].Y));
			//g.FillPolygon(Brushes.LightGreen, pta);
			g.DrawPolygon(Pens.Black, pta);
			pta[0] = Point2D(new PointF(pts[4].X, pts[4].Y));
			pta[1] = Point2D(new PointF(pts[5].X, pts[5].Y));
			pta[2] = Point2D(new PointF(pts[6].X, pts[6].Y));
			pta[3] = Point2D(new PointF(pts[7].X, pts[7].Y));
			//g.FillPolygon(Brushes.LightGray, pta);
			g.DrawPolygon(Pens.Black, pta);
		}
	}
}
