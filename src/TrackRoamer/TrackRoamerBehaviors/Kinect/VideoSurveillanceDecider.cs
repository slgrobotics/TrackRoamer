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

namespace TrackRoamer.Robotics.Services.TrackRoamerBehaviors
{
    /// <summary>
    /// different types of object, based on the actual recognition algorithm
    /// </summary>
    public enum VideoTargetType
    {
        None,
        ColorBlob,
        SurveillanceBlob,
        SomethingMoving
    }

    /// <summary>
    /// helps communicate contour and ID to the VideoSurveillanceDecider
    /// </summary>
    public class ContourContainer
    {
        public Contour<Point> contour;
        public int ID;
    }

    /// <summary>
    /// VideoSurveillanceDecider collects all data about objects recognized by video camera (Video Surveillance targets)
    /// </summary>
    public class VideoSurveillanceDecider : Dictionary<int, VideoSurveillanceTarget>
    {
        private const double fontScale = 0.5d;
        private static MCvFont _font = new MCvFont(Emgu.CV.CvEnum.FONT.CV_FONT_HERSHEY_SIMPLEX, fontScale, fontScale);
        private Dictionary<int, VideoSurveillanceTarget> tempTargetStorage = null;
        public VideoSurveillanceTarget mainColorTarget = null;

        internal int imgWidth, imgHeight;

        public VideoSurveillanceDecider(int imgWidth, int imgHeight)
        {
            this.imgWidth = imgWidth;
            this.imgHeight = imgHeight;
        }

        public void Init()
        {
            tempTargetStorage = new Dictionary<int, VideoSurveillanceTarget>();
        }

        public void ComputeMainColorTarget()
        {
            var targets = from t in this
                          where t.Value.TargetType == VideoTargetType.ColorBlob
                          orderby t.Value.Rank descending
                          select t;

            if (targets.Any())
            {
                mainColorTarget = targets.First().Value;     // color blob with the best Rank
                mainColorTarget.IsMain = true;
            }
            else
            {
                mainColorTarget = null;
            }
        }

        /// <summary>
        /// add result of color blob detection 
        /// must run under lock()
        /// </summary>
        /// <param name="contour"></param>
        /// <param name="currentPanKinect"></param>
        /// <param name="currentTiltKinect"></param>
        /// <returns>can return null</returns>
        public VideoSurveillanceTarget Update(ContourContainer contour, double currentPanKinect, double currentTiltKinect)
        {
            VideoSurveillanceTarget target = null;

            if (tempTargetStorage.ContainsKey(contour.ID))
            {
                target = tempTargetStorage[contour.ID];
                target.TimeStamp = DateTime.Now;
                target.Update(contour, currentPanKinect, currentTiltKinect);
            }
            else
            {
                target = new VideoSurveillanceTarget(this, contour.ID, contour, currentPanKinect, currentTiltKinect);
                if (target.Rank > 1.0d)
                {
                    tempTargetStorage.Add(contour.ID, target);
                }
                else
                {
                    target = null;
                }
            }

            return target;
        }

        /// <summary>
        /// add result of video surveillance detection 
        /// must run under lock()
        /// </summary>
        /// <param name="blob"></param>
        /// <param name="currentPanKinect"></param>
        /// <param name="currentTiltKinect"></param>
        /// <returns>can return null</returns>
        public VideoSurveillanceTarget Update(MCvBlob blob, double currentPanKinect, double currentTiltKinect)
        {
            VideoSurveillanceTarget target;

            if (tempTargetStorage.ContainsKey(blob.ID))
            {
                target = tempTargetStorage[blob.ID];
                target.TimeStamp = DateTime.Now;
                target.Update(blob, currentPanKinect, currentTiltKinect);
            }
            else
            {
                target = new VideoSurveillanceTarget(this, blob.ID, blob, currentPanKinect, currentTiltKinect);
                if (target.Rank > 1.0d)
                {
                    tempTargetStorage.Add(blob.ID, target);
                }
                else
                {
                    target = null;
                }
            }

            return target;
        }

        private const int intervalToPurgeS = 2;
        private long lastPurgeTimestamp = 0L;

        /// <summary>
        /// removes old objects from the dictionary, and commits new from tempTargetStorage
        /// must run under lock()
        /// </summary>
        public void PurgeAndCommit()
        {
            DateTime now = DateTime.Now;
            double purgeTimeSeconds = 5.0d;

            long tNow = now.Ticks;

            if (tNow > lastPurgeTimestamp + intervalToPurgeS * TimeSpan.TicksPerSecond)
            {

                var toPurge = this
                    // Select entries that are over purgeTimeSeconds seconds old, or type of ColorBlob (these expire immediately)
                    .Where(f => {
                        VideoSurveillanceTarget t = this[f.Key];
                        return t.TimeStamp.AddSeconds(purgeTimeSeconds) < now || t.TargetType == VideoTargetType.ColorBlob; 
                    })
                    // Reduce the information to just the key
                    .Select(f => f.Key)
                    // Realize the list
                    .ToList();

                // Remove any aged items
                toPurge.ForEach(f => this.Remove(f));

                // commit new, removing conflicting old ones:
                Commit();

                lastPurgeTimestamp = tNow;
            }
        }

        /// <summary>
        /// copy all targets from temp storage into the class base dictionary
        /// must run under lock()
        /// </summary>
        public void Commit()
        {
            // commit new, removing conflicting old ones:
            foreach (var kp in tempTargetStorage)
            {
                if (this.ContainsKey(kp.Key))
                {
                    this.Remove(kp.Key);
                }
                this.Add(kp.Key, kp.Value);
            }
        }

        /// <summary>
        /// removes ColorBlob objects from the dictionary
        /// must run under lock()
        /// </summary>
        public void purgeColorBlobs()
        {
            var toPurge = this
                // Select entries that are color blob types
                .Where(f => { return this[f.Key].TargetType == VideoTargetType.ColorBlob; })
                // Reduce the information to just the key
                .Select(f => f.Key)
                // Realize the list
                .ToList();

            // Remove any aged items
            toPurge.ForEach(f => this.Remove(f));
        }

        /// <summary>
        /// draw all targets on the image. The image must be the same size as during the creation of this object.
        /// must run under lock()
        /// </summary>
        /// <param name="img"></param>
        public void Draw(Image<Bgr, byte> img)
        {
            foreach (int key in this.Keys)
            {
                VideoSurveillanceTarget target = this[key];
                target.Draw(img);
            }
        }
    }
}

