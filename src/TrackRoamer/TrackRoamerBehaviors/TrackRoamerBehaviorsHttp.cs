using System;
using System.Collections.Generic;
using System.Text;

using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;

using Microsoft.Dss.ServiceModel.DsspServiceBase;


using TrackRoamer.Robotics.LibBehavior;
using TrackRoamer.Robotics.Utility.LibSystem;

using sicklrf = Microsoft.Robotics.Services.Sensors.SickLRF.Proxy;

namespace TrackRoamer.Robotics.Services.TrackRoamerBehaviors
{
    /// <summary>
    /// part of the class related to HTTP processing - generating images etc.
    /// </summary>
    partial class TrackRoamerBehaviorsService : DsspServiceBase
    {
        protected void GenerateTop(sicklrf.State laserData, int fieldOfView, RoutePlan plan)
        {
            //lock (lockStatusGraphics)
            //{
            
            if (currentStatusGraphics != null)
            {
                bool haveLaser = laserData != null && laserData.DistanceMeasurements != null && (DateTime.Now - _laserData.TimeStamp).TotalSeconds < 2.0d;

                //Bitmap bmp = (_state.MovingState == MovingState.MapSouth) ? currentStatusGraphics.statusBmp : currentStatusGraphics.northBmp;
                //Bitmap bmp = currentStatusGraphics.statusBmp;
                Bitmap bmp = currentStatusGraphics.northBmp;

                lock (bmp)
                {
                    using (Graphics g = Graphics.FromImage(bmp))
                    {
                        g.Clear(Color.LightGray);

                        List<Lbl> labels = new List<Lbl>();

                        if (haveLaser)
                        {
                            double angularOffset = -90 + laserData.AngularRange / 2.0;
                            double piBy180 = Math.PI / 180.0;
                            double halfAngle = laserData.AngularResolution / 2.0;
                            double drangeMax = 0.0d;

                            GraphicsPath path = new GraphicsPath();

                            // make two passes, drawing laser data and label every 20th range:
                            for (int pass = 1; pass <= 2; pass++)
                            {
                                for (int i = 0; i < laserData.DistanceMeasurements.Length; i++)
                                {
                                    int range = laserData.DistanceMeasurements[i];
                                    if (range > 0 && range < 8192)
                                    {
                                        double angle = i * laserData.AngularResolution - angularOffset;
                                        double lowAngle = (angle - halfAngle) * piBy180;
                                        double highAngle = (angle + halfAngle) * piBy180;

                                        double drange = range * StatusGraphics.scale;

                                        float lx = (float)(StatusGraphics.xCenter + drange * Math.Cos(lowAngle));
                                        float ly = (float)(StatusGraphics.xCenter - drange * Math.Sin(lowAngle));
                                        float hx = (float)(StatusGraphics.xCenter + drange * Math.Cos(highAngle));
                                        float hy = (float)(StatusGraphics.xCenter - drange * Math.Sin(highAngle));

                                        if (pass == 1)
                                        {
                                            // on the first pass just add lines to the Path and calculate the max range:
                                            if (i == 0)
                                            {
                                                path.AddLine(StatusGraphics.xCenter, StatusGraphics.imageHeight, lx, ly);
                                            }
                                            path.AddLine(lx, ly, hx, hy);

                                            drangeMax = Math.Max(drangeMax, drange);
                                        }
                                        else
                                        {
                                            // on the second pass draw the perimeter and label every 20th range:
                                            g.DrawLine(Pens.DarkBlue, lx, ly, hx, hy);

                                            if (i > 0 && i % 20 == 0 && i < laserData.DistanceMeasurements.Length - 10)
                                            {
                                                float llx = (float)(StatusGraphics.xCenter + drangeMax * 1.3f * Math.Cos(lowAngle));
                                                float lly = (float)(StatusGraphics.xCenter - drangeMax * 1.3f * Math.Sin(lowAngle));
                                                double roundRange = Math.Round(range / 1000.0d, 1); // meters
                                                string str = "" + roundRange;
                                                labels.Add(new Lbl() { label = str, lx = llx, ly = lly, brush = Brushes.Black });
                                            }
                                        }
                                    }
                                }

                                if (pass == 1)
                                {
                                    // draw the laser sweep on the first pass:
                                    g.FillPath(Brushes.White, path);
                                }
                            }
                        }

                        // draw important decision-influencing boundaries:
                        float startAngle = -150.0f;
                        float sweepAngle = 120.0f;

                        // the "stop moving" distance:
                        float radius = (float)(ObstacleDistanceMm * StatusGraphics.scale);
                        //g.DrawRectangle(Pens.Red, xCenter - radius, imageHeight - radius, radius * 2, radius * 2);
                        g.DrawArc(Pens.Red, StatusGraphics.xCenter - radius, StatusGraphics.imageHeight - radius, radius * 2, radius * 2, startAngle, sweepAngle);
                        rayLabel("stop", labels, ObstacleDistanceMm, -60.0d, Brushes.Red);

                        // the "slow down" distance:
                        radius = (float)(AwareOfObstacleDistanceMm * StatusGraphics.scale);
                        g.DrawArc(Pens.Orange, StatusGraphics.xCenter - radius, StatusGraphics.imageHeight - radius, radius * 2, radius * 2, startAngle, sweepAngle);
                        rayLabel("slow", labels, AwareOfObstacleDistanceMm, -60.0d, Brushes.Orange);

                        // the "stop mapping, enter the open space" distance
                        radius = (float)(SafeDistanceMm * StatusGraphics.scale);
                        g.DrawArc(Pens.LightBlue, StatusGraphics.xCenter - radius, StatusGraphics.imageHeight - radius, radius * 2, radius * 2, startAngle, sweepAngle);
                        rayLabel("enter", labels, SafeDistanceMm, -60.0d, Brushes.LightBlue);

                        // the "max velocity" distance:
                        radius = (float)(FreeDistanceMm * StatusGraphics.scale);
                        g.DrawArc(Pens.Green, StatusGraphics.xCenter - radius, StatusGraphics.imageHeight - radius, radius * 2, radius * 2, startAngle, sweepAngle);
                        rayLabel("free", labels, FreeDistanceMm, -60.0d, Brushes.Green);

                        // the fieldOfView arc:
                        radius = (float)(StatusGraphics.imageHeight - 10);
                        g.DrawArc(Pens.LimeGreen, StatusGraphics.xCenter - radius, StatusGraphics.imageHeight - radius, radius * 2, radius * 2, (float)(-90-fieldOfView), (float)(fieldOfView*2));
                        rayLabel("field of view", labels, (StatusGraphics.imageHeight+5) / StatusGraphics.scale, 10.0d, Brushes.LimeGreen);

                        // now draw the robot. Trackroamer is 680 mm wide.
                        float botHalfWidth = (float)(680 / 2.0d * StatusGraphics.scale);
                        DrawHelper.drawRobotBoundaries(g, botHalfWidth, StatusGraphics.imageWidth / 2, StatusGraphics.imageHeight);

                        // debug- draw a ray pointing to predefined direction; left is positive, right is negative:
                        //double rayLength = 2000.0d;
                        //rayLine("20", g, rayLength, 20.0d, Pens.Cyan, Brushes.DarkCyan);
                        //rayLine("-20", g, rayLength, -20.0d, Pens.Blue, Brushes.Blue);
                        //rayLine("70", g, rayLength, 70.0d, Pens.Red, Brushes.Red);
                        //rayLine("-70", g, rayLength, -70.0d, Pens.Green, Brushes.Green);

                        // draw laser's time stamp:
                        if (haveLaser)
                        {
                            TimeSpan howOld = DateTime.Now - laserData.TimeStamp;
                            g.DrawString(laserData.TimeStamp.ToString() + " (" + howOld + ")", StatusGraphics.fontBmp, Brushes.Black, 0, 0);
                        }

                        HistoryItem latestDecisionsHistory = StatusGraphics.HistoryDecisions.Peek();
                        HistoryItem latestSaidHistory = Talker.HistorySaid.Peek();

                        if (latestSaidHistory != null)
                        {
                            g.DrawString(latestSaidHistory.message, StatusGraphics.fontBmpL, Brushes.Black, StatusGraphics.imageWidth / 2 + 80, StatusGraphics.imageHeight + StatusGraphics.extraHeight - 20);
                        }

                        if (latestDecisionsHistory != null)
                        {
                            g.DrawString(latestDecisionsHistory.message, StatusGraphics.fontBmpL, Brushes.Black, StatusGraphics.imageWidth / 2 + 80, StatusGraphics.imageHeight + StatusGraphics.extraHeight - 40);
                        }

                        g.DrawString(_state.MovingState.ToString(), StatusGraphics.fontBmpL, Brushes.Black, StatusGraphics.imageWidth / 2 - 40, StatusGraphics.imageHeight + StatusGraphics.extraHeight - 20);

                        // draw distance labels over all other graphics:
                        foreach (Lbl lbl in labels)
                        {
                            g.DrawString(lbl.label, StatusGraphics.fontBmp, lbl.brush, lbl.lx, lbl.ly + 20);
                        }

                        // a 200W x 400H rectangle:
                        Rectangle drawRect = new Rectangle(StatusGraphics.imageWidth / 2 - StatusGraphics.extraHeight, StatusGraphics.imageHeight - StatusGraphics.extraHeight * 2, StatusGraphics.extraHeight * 2, StatusGraphics.extraHeight * 4);

                        if (_state.MostRecentProximity != null)
                        {
                            // the MostRecentProximity class here comes from the proxy, and does not have arrangedForDrawing member. Restore it:

                            double[] arrangedForDrawing = new double[8];

                            arrangedForDrawing[0] = _state.MostRecentProximity.mbr;
                            arrangedForDrawing[1] = _state.MostRecentProximity.mbbr;
                            arrangedForDrawing[2] = _state.MostRecentProximity.mbbl;
                            arrangedForDrawing[3] = _state.MostRecentProximity.mbl;

                            arrangedForDrawing[4] = _state.MostRecentProximity.mfr;
                            arrangedForDrawing[5] = _state.MostRecentProximity.mffr;
                            arrangedForDrawing[6] = _state.MostRecentProximity.mffl;
                            arrangedForDrawing[7] = _state.MostRecentProximity.mfl;

                            // draw a 200x400 image of IR proximity sensors:

                            DrawHelper.drawProximityVectors(g, drawRect, arrangedForDrawing, 1);
                        }

                        if (_state.MostRecentParkingSensor != null)
                        {
                            // the MostRecentParkingSensor class here comes from the proxy, and does not have arrangedForDrawing member. Restore it:

                            double[] arrangedForDrawing = new double[4];

                            arrangedForDrawing[0] = _state.MostRecentParkingSensor.parkingSensorMetersRB;
                            arrangedForDrawing[1] = _state.MostRecentParkingSensor.parkingSensorMetersLB;
                            arrangedForDrawing[2] = _state.MostRecentParkingSensor.parkingSensorMetersLF;
                            arrangedForDrawing[3] = _state.MostRecentParkingSensor.parkingSensorMetersRF;

                            // draw a 200x400 image of parking sensors:

                            DrawHelper.drawProximityVectors(g, drawRect, arrangedForDrawing, 2);
                        }

                        if (plan != null && plan.isGoodPlan)
                        {
                            // right turn - positive, left turn - negative, expressed in degrees:
                            int bestHeadingInt = (int)Math.Round((double)plan.bestHeadingRelative(_mapperVicinity));
                            double bestHeadingDistance = plan.legMeters.Value * 1000.0d;

                            using (Pen dirPen = new Pen(Color.Green, 5.0f))
                            {
                                dirPen.EndCap = LineCap.ArrowAnchor;
                                rayLine("best:" + bestHeadingInt, g, bestHeadingDistance * 1.1d, (double)bestHeadingInt, dirPen, Brushes.Green);
                            }

                            //int xLbl = 10;
                            //int yLbl = bmp.Height - 40;

                            //g.DrawString(comment, StatusGraphics.fontBmpL, Brushes.Green, xLbl, yLbl);
                        }

                        if(_mapperVicinity.robotDirection.bearingRelative.HasValue)
                        {
                            int goalBearingInt = (int)Math.Round((double)_mapperVicinity.robotDirection.bearingRelative);
                            double goalDistance = 3000.0d;

                            using (Pen dirPen = new Pen(Color.LimeGreen, 2.0f))
                            {
                                dirPen.EndCap = LineCap.ArrowAnchor;
                                rayLine("goal:" + goalBearingInt, g, goalDistance, (double)goalBearingInt, dirPen, Brushes.LimeGreen);
                            }

                        }
                    }
                }
            }
        }

        /// <summary>
        /// draws a line from the center (robot position) to a direction, and labels it. 
        /// </summary>
        /// <param name="text">label for the end of the vector (can be null)</param>
        /// <param name="g">bitmap graphics</param>
        /// <param name="rayLengthMm">ray length mm</param>
        /// <param name="rayAngleDegrees">ray angle degrees (right is positive, left is negative)</param>
        /// <param name="pen"></param>
        /// <param name="brush"></param>
        private void rayLine(string text, Graphics g, double rayLengthMm, double rayAngleDegrees, Pen pen, Brush brush)
        {
            float yCenter = StatusGraphics.imageHeight;
            double angle = (90.0d - rayAngleDegrees) * Math.PI / 180.0d;
            float rayX = (float)(StatusGraphics.xCenter + rayLengthMm * StatusGraphics.scale * Math.Cos(angle));
            float rayY = (float)(yCenter - rayLengthMm * StatusGraphics.scale * Math.Sin(angle));
            g.DrawLine(pen, StatusGraphics.xCenter, yCenter, rayX, rayY);
            if (!string.IsNullOrEmpty(text))
            {
                rayX = (float)(StatusGraphics.xCenter + rayLengthMm * 1.07d * StatusGraphics.scale * Math.Cos(angle));
                rayY = (float)(yCenter - rayLengthMm * 1.07d * StatusGraphics.scale * Math.Sin(angle));

                if (pen.Width > 2.0f)
                {
                    g.DrawString(text, StatusGraphics.fontBmpL, brush, rayX, rayY);
                }
                else
                {
                    g.DrawString(text, StatusGraphics.fontBmp, brush, rayX, rayY);
                }
            }
        }

        private void rayLabel(string text, List<Lbl> labels, double rayLengthMm, double rayAngleDegrees, Brush brush)
        {
            float yCenter = StatusGraphics.imageHeight;
            double angle = (90.0d - rayAngleDegrees) * Math.PI / 180.0d;
            float rayX = (float)(StatusGraphics.xCenter + rayLengthMm * StatusGraphics.scale * Math.Cos(angle));
            float rayY = (float)(yCenter - rayLengthMm * StatusGraphics.scale * Math.Sin(angle));
            labels.Add(new Lbl() { label = text, lx = rayX, ly = rayY, brush = brush });
        }

        /// <summary>
        /// draw comment line in the bottom left corner of the North bitmap
        /// </summary>
        /// <param name="comment"></param>
        private void markNorthBitmap(string comment)
        {
            if (currentStatusGraphics != null)
            {
                Bitmap bmp = currentStatusGraphics.northBmp;
                lock (bmp)
                {
                    using (Graphics g = Graphics.FromImage(bmp))
                    {
                        int xLbl = 10;
                        int yLbl = bmp.Height - 20;

                        g.DrawString(comment, StatusGraphics.fontBmpL, Brushes.Red, xLbl, yLbl);
                    }
                }
            }
        }
    }
}
