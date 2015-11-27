using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Windows.Media;

namespace TrackRoamer.Robotics.LibMapping
{
    /// <summary>
    /// a base object for any detected object appearing in the mapper's view
    /// </summary>
    public abstract class DetectedObjectBase : IDetectedObject
    {
        public int timeToLiveSeconds { get; set; }

        public DetectedObjectType objectType { get; set; }

        public DetectorType detectorType { get; set; }

        public GeoPosition geoPosition { get; set; }

        public RelPosition relPosition { get; set; }

        public long firstSeen { get; set; }
        public long lastSeen { get; set; }

        public int cellCounter { get; set; }

        public Color color { get; set; }

        public virtual bool isDead
        {
            get
            {
                // this can consider other business rules, but being old sure cuts it:

                return (DateTime.Now.Ticks - lastSeen) > timeToLiveSeconds * 10000000L;
            }
        }

        public DetectedObjectBase()
        {
            firstSeen = lastSeen = DateTime.Now.Ticks;
            timeToLiveSeconds = 3;
        }

        public DetectedObjectBase(GeoPosition pos)
            : this()
        {
            geoPosition = (GeoPosition)pos.Clone();
        }

        public DetectedObjectBase(Direction dir, Distance dist)
            : this()
        {
            relPosition = new RelPosition(dir, dist);
        }

        #region ICompareable Members

        /// <summary>
        /// Comparing two obstacles, using business logic which may consider many object properties.
        /// For example, the closer the oject, the larger it is for Compare purposes
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public int CompareTo(object other)
        {
            int ret = this.relPosition.CompareTo(((IDetectedObject)other).relPosition);    // precision up to 10mm

            return ret;
        }

        #endregion // ICompareable Members
    }
}

