using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace RedSeaGame
{
    class GridCell
    {
        #region Fields
        
        //The position where the gridcell is located in the grid
        private Point positionInGrid;

        //The position where the gridcell should be currently drawn on the screen
        //Useful for sprite drawing
        private Point positionOnScreen;

        //THe gridcell center position on the screen
        private Vector2 positionCenterOnScreen;

        //The size of the gridcell
        private Point size;

        //The bounding box that contains the gridcell
        private Rectangle boundingBox;

        //The number of cells present on top of the gridcell
        private List<Cell> cellsPresent;
        #endregion

        #region CONSTRUCTOR
        /// <summary>
        /// Creates a new grid cell based on where it should be located in the grid as well
        /// as what size the gridcell should be.
        /// </summary>
        /// <param name="gridPosition">position in the grid</param>
        /// <param name="dimensions">the size of the gridcell</param>
        public GridCell(Point gridPosition, Point dimensions)
        {
            positionInGrid = gridPosition;
            positionOnScreen = new Point(gridPosition.X * dimensions.X, gridPosition.Y * dimensions.Y);
            this.size = dimensions;
            positionCenterOnScreen = new Vector2(positionOnScreen.X + (dimensions.X / 2), positionOnScreen.Y + (dimensions.Y / 2));
            boundingBox = new Rectangle(positionOnScreen.X, positionOnScreen.Y, dimensions.X, dimensions.Y);
            cellsPresent = new List<Cell>();
        }
        #endregion

        #region GETTER METHODS
        public Point PositionInGrid { get { return positionInGrid; } }
        public Point PositionOnScreen { get { return positionOnScreen; } }
        public Vector2 PositionCenterOnScreen { get { return positionCenterOnScreen; } }
        public Rectangle BoundingBox { get { return boundingBox; } }
        public List<Cell> CellsPresent { get { return cellsPresent; } }
        #endregion

        #region METHODS
        /// <summary>
        /// Returns true of the the cell passed in through the parameter list
        /// is contained on the gridcell
        /// </summary>
        public bool ContainsCell(Cell cell)
        {
            if (boundingBox.Contains((int)cell.Center.X,(int)cell.Center.Y))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        //The cell passed in through the parameters is currently
        //on this grid cell
        public void AddCell(Cell cell)
        {
            cellsPresent.Add(cell);
        }

        //The cell that is passed in through the parameter list
        //is no longer contained on this grid cell
        public void RemoveCell(Cell cell)
        {
            cellsPresent.Remove(cell);
        }

        //No cell is contained on this grid cell
        public void RemoveAllCells()
        {
            cellsPresent.Clear();
        }
        #endregion
    }
}
