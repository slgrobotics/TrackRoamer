using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Drawing;

using Emgu.CV;
using Emgu.CV.Structure;
using Emgu.CV.UI;
using Emgu.CV.CvEnum;
using Emgu.CV.VideoSurveillance;
using Emgu.Util;

using TrackRoamer.Robotics.Utility.LibSystem;

namespace TrackRoamer.Robotics.Services.TrackRoamerBehaviors
{
    public class VideoSurveillanceTarget : IComparable
    {
        private const double fontScale = 0.5d;
        private static MCvFont _font = new MCvFont(Emgu.CV.CvEnum.FONT.CV_FONT_HERSHEY_SIMPLEX, fontScale, fontScale);

        // side-to-side field of view of the camera in degrees:
        public const double FowW = 62.7d;
        public const double FowH = 47.0d;

        private VideoSurveillanceDecider _decider;

        private int imgWidth, imgHeight;
        public Rectangle BoundingRectangle;
        public System.Drawing.Point Center;
        public Bgr drawingRectangeColor;
        public Bgr drawingRectangeColorMain;
        public Bgr drawingLabelColor;

        public int ID;
        public VideoTargetType TargetType = VideoTargetType.None;
        public DateTime TimeStamp = DateTime.Now;
        public double Rank;                 // approx area in pixels, large number; larger number corresponds to more significant target
        private double rectangleRatio;
        public bool IsMain;

        public double Pan { get; private set; }    // relative bearing from the robot's point of view, adjusted to camera pan/tilt
        public double Tilt { get; private set; }

        #region Lifecycle

        public VideoSurveillanceTarget(VideoSurveillanceDecider decider, int id, MCvBlob blob, double currentPanKinect, double currentTiltKinect)
            : this(decider, id, VideoTargetType.SurveillanceBlob)
        {
            Update(blob, currentPanKinect, currentTiltKinect);
        }

        public VideoSurveillanceTarget(VideoSurveillanceDecider decider, int id, ContourContainer contour, double currentPanKinect, double currentTiltKinect)
            : this(decider, id, VideoTargetType.ColorBlob)
        {
            Update(contour, currentPanKinect, currentTiltKinect);
        }

        private VideoSurveillanceTarget(VideoSurveillanceDecider decider, int id, VideoTargetType targetType)
        {
            _decider = decider;
            TargetType = targetType;
            ID = id;

            // prepare some items to help drawing the object on Image:
            this.imgWidth = _decider.imgWidth;
            this.imgHeight = _decider.imgHeight;
            switch (targetType)
            {
                case VideoTargetType.ColorBlob:
                    drawingRectangeColor = new Bgr(64.0, 64.0, 255.0);
                    drawingRectangeColorMain = new Bgr(32.0, 32.0, 255.0);
                    drawingLabelColor = new Bgr(255.0, 255.0, 128.0);
                    break;

                case VideoTargetType.SurveillanceBlob:
                    drawingRectangeColor = new Bgr(64.0, 255.0, 64.0);
                    drawingRectangeColorMain = new Bgr(32.0, 32.0, 255.0);
                    drawingLabelColor = new Bgr(255.0, 255.0, 128.0);
                    break;
            }
        }

        #endregion // Lifecycle

        public void Update(MCvBlob blob, double currentPanKinect, double currentTiltKinect)
        {
            // keep in mind that due to memory restrictions VideoSurveillance detector was working on the scaled down (to 1/2 size) image.
            // So all points should be multiplied by two to fit the full scale image.

            ID = blob.ID;

            Rectangle br = (System.Drawing.Rectangle)blob;

            BoundingRectangle = new Rectangle(br.X << 1, br.Y << 1, br.Width << 1, br.Height << 1);

            this.Center = new System.Drawing.Point((int)blob.Center.X << 1, (int)blob.Center.Y << 1);

            this.Pan = -FowW * (blob.Center.X - imgWidth / 2.0d) / imgWidth + currentPanKinect;
            this.Tilt = -FowH * (blob.Center.Y - imgHeight / 2.0d) / imgHeight + currentTiltKinect;

            CalculateRank();

            //Console.WriteLine("**********************************************************************    Pan=" + Pan + "   Tilt=" + Tilt);
        }

        public void Update(ContourContainer contour, double currentPanKinect, double currentTiltKinect)
        {
            ID = contour.ID;

            Contour<System.Drawing.Point> contours = contour.contour;
            BoundingRectangle = contours.BoundingRectangle;

            int centerX = BoundingRectangle.X + BoundingRectangle.Width / 2;
            int centerY = BoundingRectangle.Y + BoundingRectangle.Height / 2;

            this.Center = new System.Drawing.Point(centerX, centerY);

            this.Pan = -FowW * (centerX - imgWidth / 2.0d) / imgWidth + currentPanKinect;
            this.Tilt = -FowH * (centerY - imgHeight / 2.0d) / imgHeight + currentTiltKinect;

            CalculateRank();

            //Console.WriteLine("**********************************************************************    Pan=" + Pan + "   Tilt=" + Tilt);
        }

        /// <summary>
        /// Calculate Rank of the object, taking into consideration its area in pixels, height/width ratio, deviation from center of view etc.
        /// </summary>
        public void CalculateRank()
        {
            rectangleRatio = ((double)BoundingRectangle.Size.Height) / ((double)BoundingRectangle.Size.Width);   // tall objects have higher ratio

            // we don't want any long objects counted:
            if (rectangleRatio > 0.7d && rectangleRatio < 2.0d)
            {
                double fromCenterFactor = Math.Abs(((double)this.Center.X - imgWidth / 2.0d) * 2.0d / ((double)this.imgWidth));    // 0 (center) to 1 (sides)

                //Tracer.Trace("fromCenterFactor=" + fromCenterFactor);

                this.Rank = BoundingRectangle.Size.Width * BoundingRectangle.Size.Height * (1.2d - fromCenterFactor);
            }
            else
            {
                this.Rank = 0.0d;
            }
        }

        #region Presentation

        public void Draw(Image<Bgr, byte> img)
        {
            //string blobLabel = string.Format("{0:0}:({1:0},{2:0})", this.Rank / 100.0d, this.Pan, this.Tilt);
            string blobLabel = string.Format("{0:0}:({1:0},{2:0}) {3:0.00},{4}", this.Rank / 100.0d, this.Pan, this.Tilt, this.rectangleRatio, this.ID);

            img.Draw(this.BoundingRectangle, this.IsMain ? drawingRectangeColorMain : drawingRectangeColor, this.IsMain ? 5 : 1);
            img.Draw(blobLabel, ref _font, this.Center, drawingLabelColor);
        }

        public override string ToString()
        {
            return "Pan=" + Pan + "   Tilt=" + Tilt;
        }

        #endregion // Presentation

        // IComparable implementation:
        public int CompareTo(object obj)
        {
            VideoSurveillanceTarget other = obj as VideoSurveillanceTarget;
            return Rank.CompareTo(other.Rank);
        }
    }
}
