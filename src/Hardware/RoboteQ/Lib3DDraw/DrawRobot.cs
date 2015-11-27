using System;
using System.Drawing;
using System.Windows.Forms;

namespace Lib3DDraw
{
	public class DrawRobot : DrawCoordinateAxes
	{
		public DrawRobot(Panel _panel)
			: base(_panel, _panel.Height / 4)
		{
		}

		public float thetaX = 20.0f;
		public float thetaY = 20.0f;
		public float thetaZ = 20.0f;

		public void DrawRobotView(Graphics g)
		{
			DrawBox drawBox = new DrawBox(panel, side);

			float oneOverd = oneOverdFactor / (2 * side);
			drawBox.matrix = Matrix3.AzimuthElevation(elevation, azimuth, oneOverd)
									* Matrix3.Rotate3X(thetaX) * Matrix3.Rotate3Y(thetaY) * Matrix3.Rotate3Z(thetaZ);

			drawBox.AddBox(g);

			AddAxes(g);
		}
	}
}
