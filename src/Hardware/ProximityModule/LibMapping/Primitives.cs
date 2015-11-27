using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;

namespace TrackRoamer.Robotics.LibMapping
{
    /// <summary>
    /// CellPath is an ordered collection of cells, representing a path (line, curve...) on the cell grid.
    /// </summary>
    public class CellPath : List<CellPathElement>
    {
        public double lengthMeters;             // total length of the path
        public double firstHeadingRelative;     // first leg direction
        public bool hitObstacle = false;
        public bool isBest = false;

        public bool ContainsCell(MapCell cell)
        {
            var query = from cpe in this
                        where cpe.mapCell == cell
                        select cpe;

            return query.Count() > 0;
        }
    }

    public class CellPathElement
    {
        public MapCell mapCell;
        public double distanceMeters;
        public CellPath cellPath;
    }
}
