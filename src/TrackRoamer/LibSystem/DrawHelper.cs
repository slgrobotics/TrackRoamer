using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;

namespace TrackRoamer.Robotics.Utility.LibSystem
{
    /// <summary>
    /// helps draw elements on bitmaps
    /// </summary>
    public static class DrawHelper
    {
        public static Brush[] brushes = new Brush[8];   // for 8 possible values in rays length
        public static Font fontBmp = new Font(FontFamily.GenericSansSerif, 16, GraphicsUnit.Pixel);

        static DrawHelper()
        {
            for (int i = 0; i < 8; i++)
            {
                Color col = ColorHelper.LinearColor(Color.OrangeRed, Color.LightGreen, 0, 7, i);
                brushes[i] = new SolidBrush(col);
            }
        }

        // this is how sensors are pointed on the robot:
        private static double[] angles4 = { 77.5d, 102.5d, 257.5d, -77.5d };        // four Parking Sensor heads, two looking forward, two - backwards at a slight angle
        private static double[] angles8 = { 37.5d, 62.5d, 117.5d, 142.5d, 217.5d, 242.5d, -62.5d, -37.5d };     // eight IR sensors on the corners

        /// <summary>
        /// draws proximity vectors overlay, semi-transparent 8 rays of color and length contained in a rectangle
        /// </summary>
        /// <param name="destGraphics"></param>
        /// <param name="destRect"></param>
        /// <param name="arrangedForDrawing">array of distances in meters arranged in order for drawing - starting with BR, BBR, BBL and on clockwise</param>
        /// <param name="type">1 for IR sensors, 2 for Parking Sensor</param>
        public static void drawProximityVectors(Graphics destGraphics, Rectangle destRect, double[] arrangedForDrawing, int type)
        {
            int maxRayRadius = destRect.Width / 2;

            using (Bitmap proxBmp = new Bitmap(destRect.Width, destRect.Height))
            {
                using (Graphics gp = Graphics.FromImage(proxBmp))
                {
                    for (int i = 0; i < arrangedForDrawing.GetLength(0); i++)
                    {
                        double meters = arrangedForDrawing[i];

                        //meters = 1.0d;
                        //if (i == 0) meters = 0.2d;
                        //if (i == 1) meters = 0.6d;

                        int indx = 0;
                        if (type == 1)
                        {
                            if (meters > 0.1d && meters < 1.41d)     // reasonable range for IR sensor
                            {
                                indx = (int)Math.Round(meters * 5.0d);
                            }
                        }
                        else
                        {
                            if (meters > 0.1d && meters < 2.41d)     // reasonable range for parking sensor
                            {
                                indx = (int)Math.Round(meters * 5.0d);
                            }
                        }

                        //indx = i; //debug colors

                        int sizeXY = maxRayRadius * (indx + 1) / 4;
                        int centerX = destRect.Width / 2;
                        int centerY = destRect.Height / 2;
                        int startX = centerX - sizeXY / 2;
                        int startY = centerY - sizeXY / 2;
                        //double angle = 45 * i + 5;
                        double angle = (type == 1 ? angles8[i] : angles4[i]) - 5;

                        // draw a ray:
                        gp.FillPie(brushes[indx > 7 ? 7 : indx], startX, startY, sizeXY, sizeXY, (int)angle, 10);

                        angle = (-angle + 90.0d - 5.0d) * Math.PI / 180.0d;
                        sizeXY = maxRayRadius / 4;
                        int startXX = (int)Math.Round((maxRayRadius - sizeXY) * Math.Sin(angle) - sizeXY / 2.0d) + centerX;
                        int startYY = (int)Math.Round((maxRayRadius - sizeXY) * Math.Cos(angle) - sizeXY / 2.0d) + centerY;

                        // draw a rad circle when distance is short:
                        if (meters < 1.19d)
                        {
                            gp.FillEllipse(Brushes.Red, startXX, startYY, sizeXY, sizeXY);
                            //gp.DrawString("" + i, fontBmp, Brushes.Black, (float)startXX, (float)startYY);
                        }
                    }

                    ColorMatrix matrix = new ColorMatrix();
                    matrix.Matrix33 = 0.3f; //opacity 0 = completely transparent, 1 = completely opaque

                    ImageAttributes attributes = new ImageAttributes();
                    attributes.SetColorMatrix(matrix, ColorMatrixFlag.Default, ColorAdjustType.Bitmap);

                    destGraphics.DrawImage((Image)proxBmp,
                                 destRect,
                                 0, 0, destRect.Width, destRect.Height,
                                 GraphicsUnit.Pixel,
                                 attributes);
                }
            }
        }

        public static void drawRobotBoundaries(Graphics g, float botHalfWidth, int startPointX, int startPointY)
        {
            g.DrawLine(Pens.Red, startPointX, startPointY - botHalfWidth, startPointX, startPointY);
            g.DrawLine(Pens.Red, startPointX - 3, startPointY - botHalfWidth, startPointX + 3, startPointY - botHalfWidth);
            g.DrawLine(Pens.Red, startPointX - botHalfWidth, startPointY - 3, startPointX - botHalfWidth, startPointY);
            g.DrawLine(Pens.Red, startPointX + botHalfWidth, startPointY - 3, startPointX + botHalfWidth, startPointY);
            g.DrawLine(Pens.Red, startPointX - botHalfWidth, startPointY - 1, startPointX + botHalfWidth, startPointY - 1);
        }
    }
}
