using System;
using System.Drawing;
using System.Windows.Forms;

namespace Lib3DDraw
{
	public class DrawCylinder : Draw3DObject
	{
		protected float r = 10;
		protected float h = 10;

		public DrawCylinder(Panel _panel)
			: base(_panel)
		{
			matrix = Matrix3.Axonometric(35.26f, -45);
		}

		public DrawCylinder(Panel _panel, float r1, float h1)
			: this(_panel)
		{
			r = r1;
			h = h1;
		}

		public Point3[] CircleCoordinates(float y, float rad, int points)
		{
			Point3[] pts = new Point3[points + 1];
			//Matrix3 m = new Matrix3();

			for (int i = 0; i < pts.Length; i++)
			{
				pts[i] = matrix.Cylindrical(rad, i * 360 / (pts.Length - 1), y);
			}
			return pts;
		}

		public Point3[] CircleCoordinates(float y)
		{
			return CircleCoordinates(y, r, 90);
		}

		protected Point3[] ptsTop;
		protected PointF[] ptaTop;

		public void DrawIsometricView(Graphics g, int nPoints)
		{
			ptsTop = CircleCoordinates(h / 2, r, nPoints);
			ptaTop = new PointF[ptsTop.Length];

			Point3[] ptsBottom = CircleCoordinates(-h / 2, r, nPoints);
			PointF[] ptaBottom = new PointF[ptsBottom.Length];
			
			for (int i = 0; i < ptsBottom.Length; i++)
			{
				ptsBottom[i].Transform(matrix);
				ptaBottom[i] = Point2D(new PointF(ptsBottom[i].X,
						  ptsBottom[i].Y));
				ptsTop[i].Transform(matrix);
				ptaTop[i] = Point2D(new PointF(ptsTop[i].X,
						  ptsTop[i].Y));
			}

			PointF ptaTopPrev = ptaTop[0];
			PointF ptaBottomPrev = ptaBottom[0];

			PointF[] ptf = new PointF[4];
			int step = 5;
			for (int i = step; i < ptsTop.Length; i += step)
			{
				ptf[0] = ptaBottomPrev;
				ptf[1] = ptaTopPrev;
				ptf[2] = ptaTopPrev = ptaTop[i];
				ptf[3] = ptaBottomPrev = ptaBottom[i];
				//if (i < ptsTop.Length / 4 || i > ptsTop.Length * 3 / 4)
				if (i <= 40 || i >= 230)
				{
					g.FillPolygon(Brushes.LightGray, ptf);
					//g.DrawPolygon(Pens.Black, ptf);
				}
			}

			//g.FillPolygon(Brushes.LightGreen, ptaTop);
			g.DrawPolygon(Pens.Black, ptaTop);
		}
	}
}
