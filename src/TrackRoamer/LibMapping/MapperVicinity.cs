using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TrackRoamer.Robotics.LibMapping
{
    /// <summary>
    /// MapperVicinity operates on MapVicinity, performing rotations and translations.
    /// It is aware of current robot position and direction.
    /// It keeps track of detected objects.
    /// </summary>
    public class MapperVicinity : List<IDetectedObject>
    {
        MapVicinity map = new MapVicinity();

        public readonly int RelMapWidth;
        public readonly int RelMapHeight;

        public readonly int GeoMapWidth;
        public readonly int GeoMapHeight;

        public RobotState robotState = new RobotState()
        {
            leftPower = -0.4d,
            leftSpeed = -1.0d,

            rightPower = -1.0d,
            rightSpeed = -1.0d,
            medianVelocity = 0.7d,

            manualControl = true,
            manualControlCommand = string.Empty,

            ignoreGps = false,
            ignoreAhrs = false,
            ignoreLaser = false,
            ignoreProximity = false,
            ignoreParkingSensor = false,
            ignoreKinectSounds = false,
            ignoreRedShirt = false,
            ignoreKinectSkeletons = false,
            doLostTargetRoutine = false,
            doPhotos = false,
            doVicinityPlanning = false
        };

        public TurnState turnState { get; set; }

        private Direction _robotDirection = new Direction() { heading = 0.0d };       // robot orientation relative to true North. Bearing can be null if no target is present.
        public Direction robotDirection { get { return _robotDirection; } set { _robotDirection = value; computeMapPositions(); } }

        private GeoPosition _robotPosition = new GeoPosition(0.0d, 0.0d);
        public GeoPosition robotPosition { get { return _robotPosition; } set { _robotPosition = value; computeMapPositions(); } }

        public double currentOdometryTheta;      // radians
        public double currentOdometryX;          // meters
        public double currentOdometryY;          // meters

        public MapperVicinity()
        {
            robotState.robotLengthMeters = MapperSettings.robotLengthMeters;
            robotState.robotWidthMeters = MapperSettings.robotWidthMeters;

            turnState = new TurnState()
            {
                directionInitial = new Direction() { heading = _robotDirection.heading },
                directionCurrent = new Direction() { heading = _robotDirection.heading + 40 },
                directionDesired = new Direction() { heading = _robotDirection.heading + 80 }
            };

            map.init();
            //map.ensureNonEmpty();

            RelMapWidth = map.RelMapWidth;
            RelMapHeight = map.RelMapHeight;

            GeoMapWidth = map.GeoMapWidth;
            GeoMapHeight = map.GeoMapHeight;
        }

        //public void AddDetectedObject(IDetectedObject dobj)
        //{
        //    detectedObjects.Add(dobj);
        //}

        //public void ClearDetectedObjects()
        //{
        //    detectedObjects.Clear();
        //}

        private void ResetDetectedObjectsCellCounters()
        {
            foreach (IDetectedObject dobj in this)
            {
                dobj.cellCounter = 0;
            }
        }

        public MapCell relCellAt(int cellXindex, int cellYindex)
        {
            return map.relCellAt(cellXindex, cellYindex);
        }

        public MapCell geoCellAt(int cellXindex, int cellYindex)
        {
            return map.geoCellAt(cellXindex, cellYindex);
        }

        public MapCell relCellAt(Direction dir, Distance dist)
        {
            RelPosition pos = new RelPosition(dir, dist);

            return relCellAt(pos);
        }

        public MapCell relCellAt(RelPosition pos)
        {
            // relate the relative position to a cell:
            int cellXindex = (int)Math.Floor(RelMapWidth / 2.0d + pos.X / MapperSettings.elementSizeMeters);
            int cellYindex = (int)Math.Floor(RelMapHeight / 2.0d - pos.Y / MapperSettings.elementSizeMeters);

            MapCell cell = relCellAt(cellXindex, cellYindex);

            return cell; // may be null
        }

        public MapCell geoCellAt(GeoPosition pos)
        {
            MapCell cell = null;

            if (robotPosition != null)
            {
                // a grad square is cos(latitude) thinner, we need latitude in radians:
                double midLatRad = robotPosition.Y * Math.PI / 180.0d;
                double latitudeFactor = Math.Cos(midLatRad);
                double cellWidthDegrees = MapperSettings.elementSizeMeters / (Distance.METERS_PER_DEGREE * latitudeFactor);
                double cellHeightDegrees = MapperSettings.elementSizeMeters / Distance.METERS_PER_DEGREE;

                // relate the geo position to a cell:
                int cellXindex = (int)Math.Floor((pos.Lng - robotPosition.Lng) / cellWidthDegrees + RelMapWidth / 2.0d);
                int cellYindex = (int)Math.Floor(-(pos.Lat - robotPosition.Lat) / cellHeightDegrees + RelMapHeight / 2.0d);

                cell = geoCellAt(cellXindex, cellYindex);
            }

            return cell; // may be null
        }

        /// <summary>
        /// compute direction to target at GeoPosition 
        /// </summary>
        /// <param name="targetPos"></param>
        /// <returns></returns>
        public Direction toTarget(GeoPosition targetPos)
        {
            Direction dir = new Direction() { heading = robotPosition.bearing(targetPos) };

            return dir;
        }

        /// <summary>
        /// convert direction and distance from the robot to GeoPosition 
        /// </summary>
        /// <returns></returns>
        public GeoPosition toPosition(Direction dir, Distance dist)
        {
            // TBD
            GeoPosition pos = new GeoPosition(robotPosition);

            return pos;
        }

        // the robot moved by metersX, metersY from current position, while relative to robot orientation.
        // robot coordinates are used, +1m,+1m means right,forward (upwards on the scren).
        public void translate(double metersX, double metersY)
        {
            if (robotDirection.heading.HasValue)
            {
                double headingR = ((double)robotDirection.heading) * Math.PI / 180.0d;

                double metersLat = -metersX * Math.Sin(headingR) + metersY * Math.Cos(headingR);
                double metersLng = metersX * Math.Cos(headingR) + metersY * Math.Sin(headingR);

                robotPosition.translate(new Distance(metersLng), new Distance(metersLat));

                computeMapPositions();
            }
        }

        public void rotate(double angleDegrees)
        {
            // see http://homepages.inf.ed.ac.uk/rbf/HIPR2/rotate.htm
            /*
             * 
             */

            double? heading = robotDirection.heading + angleDegrees;

            robotDirection.course = heading;    // may roll over 360

            computeMapPositions();
        }

        public void rotateTo(double angleDegrees)
        {
            // see http://homepages.inf.ed.ac.uk/rbf/HIPR2/rotate.htm
            /*
             * 
             */
            robotDirection.course = angleDegrees;

            computeMapPositions();
        }

        public void computeMapPositions()
        {
            lock (this)
            {
                try
                {
                    ResetDetectedObjectsCellCounters();
                    map.ClearRelCells();
                    map.ClearGeoCells();

                    foreach (IDetectedObject idobj in this)
                    {
                        idobj.updateRelPosition(robotPosition, robotDirection);

                        if (idobj.objectType != DetectedObjectType.Mark)    // "Mark" objects are not participating in obstacle or cell calculations
                        {
                            // relate the object to a Relative cell:
                            MapCell relCell = relCellAt(idobj.relPosition);

                            if (relCell != null)
                            {
                                relCell.AddDetectedObject(idobj);
                                idobj.cellCounter++;
                            }

                            // relate the object to a Geo cell:
                            MapCell geoCell = geoCellAt(idobj.geoPosition);

                            if (geoCell != null)
                            {
                                geoCell.AddDetectedObject(idobj);
                                idobj.cellCounter++;
                            }
                        }
                    }
                }
                catch (Exception exc)
                {
                    Console.WriteLine("computeMapPositions() - " + exc);
                }

                // purge cells that are out of grids or should be purged anyway:
                this.RemoveAll(x => (x.objectType != DetectedObjectType.Mark && (x.cellCounter == 0 || x.isDead)));
            }
        }
    }
}
