using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace Lib3DDraw
{
	public class DrawCompass : DrawCylinder
	{
		private Pen arrowPen;
		private	Font font = new Font(FontFamily.GenericSansSerif, 10f);
		private	Font fontB = new Font(FontFamily.GenericSansSerif, 12f, FontStyle.Bold);

		public DrawCompass(Panel _panel)
			: base(_panel, _panel.Height / 2 - 20, _panel.Height / 10)
		{
			arrowPen = new Pen(Color.DarkGreen, 3.0f);
			LineCap lineCap = LineCap.ArrowAnchor;
			arrowPen.SetLineCap(lineCap, LineCap.Flat, DashCap.Flat);
		}

		public void DrawCompassView(Graphics g, double bearing)
		{
			base.DrawIsometricView(g, 360);

			string sBearing = String.Format("{0:f1}", bearing);

			bearing = Math.Min(360.0d, (540.0d - bearing) % 360.0d);

			Point3[] ptsTopInner = CircleCoordinates(h / 2 + 2, r / 8, 30);
			PointF[] ptaTopInner = new PointF[ptsTopInner.Length];

			Point3[] ptsTopOuter = CircleCoordinates(h / 2 + 2, r + r / 3, 360);
			PointF[] ptaTopOuter = new PointF[ptsTopOuter.Length];

			for (int i = 0; i < ptsTopInner.Length; i++)
			{
				ptsTopInner[i].Transform(matrix);
				ptaTopInner[i] = Point2D(new PointF(ptsTopInner[i].X, ptsTopInner[i].Y));
			}

			for (int i = 0; i < ptsTopOuter.Length; i++)
			{
				ptsTopOuter[i].Transform(matrix);
				ptaTopOuter[i] = Point2D(new PointF(ptsTopOuter[i].X, ptsTopOuter[i].Y));
			}
			//g.DrawPolygon(Pens.Blue, ptaTopOuter);

			g.FillPolygon(Brushes.Pink, ptaTopInner);
			g.DrawPolygon(Pens.Black, ptaTopInner);

			g.DrawLine(Pens.DarkGray, ptaTop[0], ptaTop[180]);
			g.DrawLine(Pens.DarkGray, ptaTop[90], ptaTop[270]);

			int iBearing = (int)Math.Round(bearing);
			int iOpposite = (iBearing + 540) % 360;
			g.DrawLine(arrowPen, ptaTop[iBearing], ptaTop[iOpposite]);

			g.DrawString(sBearing, fontB, Brushes.Red, ptaTopOuter[iBearing].X-20, ptaTopOuter[iBearing].Y-10);

			g.DrawString("S", font, Brushes.Red, ptaTop[0]);
			g.DrawString("E", font, Brushes.Red, ptaTop[90]);
			g.DrawString("N", font, Brushes.Red, ptaTop[180]);
			g.DrawString("W", font, Brushes.Red, ptaTop[270]);
		}

	}
}
