using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using TrackRoamer.Robotics.LibMapping;

namespace TrackRoamer.Robotics.LibBehavior
{
    public class RoutePlanner
    {
        public List<CellPath> cellPaths = new List<CellPath>();

        public MapperVicinity _mapper;

        public double sweepAngleNormal = 80.0d;     // while planning, first consider everything within +-sweepAngleNormal/2 degrees in front of the robot
        public double sweepAngleWide = 180.0d;      // then consider full sweep within +-sweepAngleWide/2 degrees in front of the robot
        public double raySweepDegrees = 5.0d;       // angle between rays

        public double ObstacleDistanceMeters = 0.6d;

        private int nW;
        private int nH;
        private double pathDistanceMax;


        public RoutePlanner(MapperVicinity mapper)
        {
            _mapper = mapper;

            nW = MapperSettings.nW;
            nH = MapperSettings.nH;
            pathDistanceMax = 0.9d * MapperSettings.elementSizeMeters * Math.Min(nW, nH) / 2.0d;     // try not to hit walls - that's why 0.9d
        }

        /// <summary>
        /// creates a RoutePlan, based on mapper's robotDirection and cells on the geo plane (using _mapper.geoCellAt()); analyses obstacles (busy cells)
        /// </summary>
        /// <returns></returns>
        public RoutePlan planRoute()
        {
            DateTime started = DateTime.Now;

            RoutePlan plan = new RoutePlan();

            if (_mapper.robotDirection.heading.HasValue)
            {
                lock (this)
                {
                    try
                    {
                        cellPaths.Clear();

                        double sweepAngleHalf = sweepAngleNormal / 2.0d;

                        double goalBearingRelative = 0.0d;  // straight in front is default

                        if (_mapper.robotDirection.bearing.HasValue)
                        {
                            goalBearingRelative = (double)_mapper.robotDirection.turnRelative;
                        }

                        if (Math.Abs(goalBearingRelative) > sweepAngleHalf)
                        {
                            // robot is pointing away from the goal, make him turn towards the goal first:

                            plan.bestHeading = Direction.to360(_mapper.robotDirection.course + goalBearingRelative);
                            plan.legMeters = null;
                            plan.closestObstacleMeters = _mapper.robotState.robotLengthMeters / 2.0d;
                        }
                        else
                        {
                            int nsteps = (int)Math.Round(sweepAngleHalf / raySweepDegrees);

                            for (int i = -nsteps; i < nsteps; i++)
                            {
                                double pathHeadingRelative = raySweepDegrees * i;

                                Direction dir = new Direction() { heading = _mapper.robotDirection.heading, bearingRelative = pathHeadingRelative };    // related to robot heading;

                                CellPath cellPath = shootRay(dir);

                                cellPath.firstHeadingRelative = pathHeadingRelative;

                                cellPaths.Add(cellPath);
                            }

                            // order (low to high) paths based on their length and deviation from the goal bearing - using pathRatingFunction():
                            CellPath bestPath = cellPaths.OrderBy(c => pathRatingFunction(c, goalBearingRelative, sweepAngleHalf)).Last();

                            bestPath.isBest = true;

                            plan.bestHeading = Direction.to360(_mapper.robotDirection.course + bestPath.firstHeadingRelative);
                            plan.legMeters = bestPath.lengthMeters;
                            plan.closestObstacleMeters = bestPath.lengthMeters - _mapper.robotState.robotLengthMeters;
                        }
                    }
                    catch (Exception exc)
                    {
                        Console.WriteLine("planRoute() - " + exc);
                    }
                }
            }

            plan.timeSpentPlanning = DateTime.Now - started;

            return plan;
        }

        /// <summary>
        /// Rate paths based on their length and deviation from the goal bearing.
        /// Better paths have higher value.
        /// </summary>
        /// <param name="cellPath"></param>
        /// <param name="goalBearingRelative"></param>
        /// <returns></returns>
        private double pathRatingFunction(CellPath cellPath, double goalBearingRelative, double sweepAngleHalf)
        {
            double overObstacleLength = cellPath.lengthMeters - ObstacleDistanceMeters;

            if (overObstacleLength < 0.0d)
            {
                return 0.0d;
            }

            double distanceFactor = Math.Min((pathDistanceMax - ObstacleDistanceMeters) * 0.9d, overObstacleLength);

            double bearing = cellPath.firstHeadingRelative - goalBearingRelative;

            bearing = Direction.to180(bearing);
            // at this point bearing is between -180...180

            if (bearing > sweepAngleHalf)
            {
                return 0.0d;
            }

            double deflection = Math.Abs(bearing) / sweepAngleHalf;     // 0 pointing to goal, 1 pointing to the side ray.

            double directionFactor = 1.0d - 0.9d * deflection;          // 1.0 when pointing to goal, 0.1 when pointing to the side ray

            double rating = distanceFactor * directionFactor;
            //double rating = directionFactor;

            return rating;
        }

        #region shootRay()

        /// <summary>
        /// registers a geo cell at (x,y) with the cell path, storing current pathDistance.
        /// </summary>
        /// <param name="cellPath"></param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="pathDistance"></param>
        /// <returns></returns>
        protected bool registerCell(CellPath cellPath, int x, int y, double pathDistance, out MapCell mc)
        {
            mc = _mapper.geoCellAt(x, y);

            if (mc == null || mc.val > 0)
            {
                return true;  // ray hit an obstacle or the wall
            }

            //if (!cellPath.ContainsCell(mc))
            {
                cellPath.Add(new CellPathElement() { mapCell = mc, distanceMeters = pathDistance, cellPath = cellPath });
            }

            return false;   // not hit any obstacle or wall
        }

        /// <summary>
        /// fills cells that are along the path from the center to border
        /// </summary>
        /// <param name="angle">degrees, from vertical up, -180 to 180 or 0...360 (any angle will be brought to -180...180 range)</param>
        protected CellPath shootRay(Direction dir)
        {
            CellPath cellPath = new CellPath();
            bool hitObstacle = false;

            double pathDistance = _mapper.robotState.robotLengthMeters / 2.0d;
            MapCell mc = null;

            double angle = (double)dir.bearing;

            int startX = nW / 2;
            int startY = nH / 2;

            int robotWidthCells = (int)Math.Floor(_mapper.robotState.robotWidthMeters / MapperSettings.elementSizeMeters) + 1;
            int halfWidth = (int)Math.Ceiling((robotWidthCells - 1) / 2.0d);

            angle = Direction.to180(angle);

            bool verticalUp = Math.Abs(angle) <= 1.0d;
            bool verticalDown = angle >= 179.0d || angle <= -179.0d;

            int y = startY;
            int dy = angle > 90.0d || angle < -90.0d ? 1 : -1;

            if (verticalUp || verticalDown)
            {
                int endY = verticalUp ? 0 : nH - 1;

                while (!hitObstacle && y >= 0 && y < nH)
                {
                    pathDistance = Math.Abs(y - startY) * MapperSettings.elementSizeMeters;

                    for (int xx = startX - halfWidth; !hitObstacle && xx <= startX + halfWidth; xx++)
                    {
                        hitObstacle = registerCell(cellPath, xx, y, pathDistance, out mc);
                    }
                    y += dy;
                }
            }
            else
            {
                double angleR = (90.0d - angle) * Math.PI / 180.0d;
                double factor = verticalUp ? 100000.0d : (verticalDown ? -100000.0d : Math.Tan(angleR));
                int dx = angle > 0.0d ? 1 : -1;
                bool spreadV = angle >= 45.0d && angle <= 135.0d || angle < -45.0d && angle > -135.0d;

                for (int x = startX; !hitObstacle && x >= 0 && x < nW && y >= 0 && y < nH; x += dx)
                {
                    double pathDistanceX = Math.Abs(x - startX) * MapperSettings.elementSizeMeters;
                    double pathDistanceY = Math.Abs(y - startY) * MapperSettings.elementSizeMeters;

                    pathDistance = Math.Sqrt(pathDistanceX * pathDistanceX + pathDistanceY * pathDistanceY);

                    if (pathDistance >= pathDistanceMax)
                    {
                        break;
                    }

                    int endY = Math.Max(0, Math.Min(startY - ((int)Math.Round((x - startX) * factor)), nH - 1));

                    while (!hitObstacle && y >= 0 && y < nH && (dy > 0 && y <= endY || dy < 0 && y >= endY))
                    {
                        if (spreadV)
                        {
                            hitObstacle = registerCell(cellPath, x, y, pathDistance, out mc);
                        }
                        else
                        {
                            for (int xx = Math.Max(0, x - halfWidth); !hitObstacle && xx <= Math.Min(x + halfWidth, nW - 1); xx++)
                            {
                                hitObstacle = registerCell(cellPath, xx, y, pathDistance, out mc);
                            }
                        }

                        y += dy;
                    }
                    if (spreadV)
                    {
                        for (int yy = Math.Max(0, endY - halfWidth); !hitObstacle && yy <= Math.Min(endY + halfWidth, nH - 1); yy++)
                        {
                            hitObstacle = registerCell(cellPath, x, yy, pathDistance, out mc);
                        }
                    }
                    else if(!hitObstacle)
                    {
                        hitObstacle = registerCell(cellPath, x, endY, pathDistance, out mc);
                    }
                }
            }
            cellPath.lengthMeters = pathDistance;
            cellPath.hitObstacle = hitObstacle;

            return cellPath;
        }
        #endregion // shootRay()
    }
}
