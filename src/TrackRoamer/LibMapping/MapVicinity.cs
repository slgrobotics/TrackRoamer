using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TrackRoamer.Robotics.LibMapping
{
    /// <summary>
    /// MapVicinity tries to be just a W x H collection of "Relative" cells and a grid of "Geo" cells.
    /// It is not aware of coordinates or distances, direction or detected objects.
    /// MapCells may keep track of objects and have enough intelligence to compute obstacle weight.
    /// </summary>
    public class MapVicinity
    {
        private readonly int nrW;   // "Relative" grid dimensions
        private readonly int nrH;

        private readonly int ngW;   // "Geo" grid dimensions
        private readonly int ngH;

        private MapCell[,] relCells;
        private MapCell[,] geoCells;

        public MapVicinity()
        {
            // we might need this because in Designer the view does not seem to see App.config in MapperSettings properly:
            nrW = MapperSettings.nW < 10 ? 10 : MapperSettings.nW;
            nrH = MapperSettings.nH < 10 ? 10 : MapperSettings.nH;

            relCells = new MapCell[nrH, nrW];

            ngW = MapperSettings.nW < 10 ? 10 : MapperSettings.nW;
            ngH = MapperSettings.nH < 10 ? 10 : MapperSettings.nH;

            geoCells = new MapCell[ngH, ngW];
        }

        public int RelMapWidth { get { return relCells.GetLength(1); } }
        public int RelMapHeight { get { return relCells.GetLength(0); } }

        public int GeoMapWidth { get { return geoCells.GetLength(1); } }
        public int GeoMapHeight { get { return geoCells.GetLength(0); } }

        public MapCell relCellAt(int cellXindex, int cellYindex)
        {
            if (cellXindex < 0 || cellYindex < 0 || cellXindex >= nrW || cellYindex >= nrH)
            {
                return null;
            }

            return relCells[cellYindex, cellXindex];
        }

        public MapCell geoCellAt(int cellXindex, int cellYindex)
        {
            if (cellXindex < 0 || cellYindex < 0 || cellXindex >= ngW || cellYindex >= ngH)
            {
                return null;
            }

            return geoCells[cellYindex, cellXindex];
        }

        public void ClearRelCells()
        {
            foreach (MapCell cell in relCells)
            {
                cell.Clear();
                cell.colors.Clear();
            }
        }

        public void ClearGeoCells()
        {
            foreach (MapCell cell in geoCells)
            {
                cell.Clear();
                cell.colors.Clear();
            }
        }

        /// <summary>
        /// one-time initialization routine
        /// </summary>
        public void init()
        {
            Random random = new Random();

            for (int i = 0; i < nrH; i++)
            {
                for (int j = 0; j < nrW; j++)
                {
                    relCells[i, j] = new MapCell() { val = random.Next(2), x = j, y = i };
                }
            }

            for (int i = 0; i < ngH; i++)
            {
                for (int j = 0; j < ngW; j++)
                {
                    geoCells[i, j] = new MapCell() { val = random.Next(2), x = j, y = i };
                }
            }
        }

        /// <summary>
        /// a lean version of init(), can be called repeatedly. Does not empty the cells.
        /// </summary>
        public void ensureNonEmpty()
        {
            for (int i = 0; i < nrH; i++)
            {
                for (int j = 0; j < nrW; j++)
                {
                    if (relCells[i, j] == null)
                    {
                        relCells[i, j] = new MapCell() { val = 0, x = j, y = i };
                    }
                }
            }

            for (int i = 0; i < ngH; i++)
            {
                for (int j = 0; j < ngW; j++)
                {
                    if (geoCells[i, j] == null)
                    {
                        geoCells[i, j] = new MapCell() { val = 0, x = j, y = i };
                    }
                }
            }
        }
    }
}
