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

        /// <summary>
        /// we may know position of the obstacle, or just a direction where it is (i.e. sound)
        /// </summary>
        public DetectedObjectKind objectKind { get; set; }

        public DetectedObjectType objectType { get; set; }

        public DetectorType detectorType { get; set; }

        /// <summary>
        /// a wall is likely to have cells behind it inaccessible, while we can drive around/behind a human
        /// </summary>
        public bool hasShadowBehind { get; set; }

        public GeoPosition geoPosition { get; set; }

        public RelPosition relPosition { get; private set; }    // we should only set it via updateRelPosition()

        /// <summary>
        /// recalculates relPosition
        /// </summary>
        /// <param name="myPos">usually robot position</param>
        /// <param name="myDir">usually robot direction</param>
        public void updateRelPosition(GeoPosition myPos, Direction myDir)
        {
            this.relPosition = new RelPosition(this.directionTo(myPos, myDir), this.distanceTo(myPos)); 
        }

        public Direction directionTo(GeoPosition myPos, Direction myDir) { return new Direction() { heading = myDir.heading, bearing = myPos.bearing(this.geoPosition) }; }

        public Distance distanceTo(GeoPosition myPos) { return this.geoPosition.distanceFrom(myPos); }

        public long firstSeen { get; set; }
        public long lastSeen { get; set; }

        public int cellCounter { get; set; }

        public Color color { get; protected set; }

        public void SetColorByType()
        {
            switch (objectType)
            {
                default:
                case DetectedObjectType.Unknown:
                    color = Colors.Gray;
                    break;

                case DetectedObjectType.Mark:
                    color = Colors.Green;
                    break;

                case DetectedObjectType.Obstacle:
                    switch (detectorType)
                    {
                        default:
                        case DetectorType.NONE:
                            color = Colors.Gray;
                            break;
                        case DetectorType.SONAR_SCANNING:
                            color = Colors.Yellow;
                            break;
                        case DetectorType.SONAR_DIRECTED:
                            color = Colors.Orange;
                            break;
                        case DetectorType.IR_DIRECTED:
                            color = Colors.Cyan;
                            break;
                        case DetectorType.KINECT_DEPTH:
                            color = Colors.Brown;
                            break;
                        case DetectorType.KINECT_CAMERA:
                            color = Colors.Brown;
                            break;
                        case DetectorType.WHISKERS:
                            color = Colors.Red;
                            break;
                    }
                    break;

                case DetectedObjectType.Human:
                    switch (detectorType)
                    {
                        case DetectorType.KINECT_SKELETON:
                            color = Colors.Purple;
                            break;
                        case DetectorType.KINECT_CAMERA:
                            color = Colors.Purple;
                            break;
                        case DetectorType.KINECT_MIC:
                            color = Colors.Purple;
                            break;
                        default:
                            color = Colors.Gray;
                            break;
                    }
                    break;

                case DetectedObjectType.Sound:
                    color = Colors.CornflowerBlue;
                    break;

                case DetectedObjectType.MoveTarget:
                    color = Colors.Brown;
                    break;

                case DetectedObjectType.GunTarget:
                    color = Colors.Azure;
                    break;
            }
        }

        public virtual bool isDead
        {
            get
            {
                // this can consider other business rules, but being old sure cuts it:

                return (DateTime.Now.Ticks - lastSeen) > timeToLiveSeconds * TimeSpan.TicksPerSecond;
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

