using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace RedSeaGame
{
    class Grid
    {
        #region Fields

        //Stores the movable locations in the game
        private List<List<GridCell>> theGrid;

        private List<GridCell> playerAttachedAntibodyAdjacentLocations;

        //Stors the x and y dimension of each grid cell
        private Point gridCellSize;

        //Stores the enemy locations of the last game tick
        private List<Enemy> oldEnemyLocationsOnScreen;

        //Stores the location of the player on the grid
        private GridCell playerLocation;

        //Stores the current enemy locations during a current game tick
        private List<GridCell> enemyLocations;

        //Stores the Maximum horizontal dimension of the grid
        private int gridMaxHorizontalSize;

        //Stores the player's previous location on the screen
        private Vector2 playerExactOldLocation;

        //Stores the player's current position on the screen
        private Vector2 playerExactLocation;

        #endregion

        #region CONSTRUCTOR
        /// <summary>
        /// Creates grid locations in the game based on which locations can be traveled upon by cells
        /// and which cannot.
        /// </summary>
        /// <param name="gridCellDimensions">The dimensions of the grid</param>
        /// <param name="navigableLocations">Locations that can be traveled upon by cells</param>
        /// <param name="gridCellSize">The height and width of a grid cell</param>
        public Grid(Point gridCellDimensions, List<Point> navigableLocations, Point gridCellSize)
        {
            this.gridCellSize = gridCellSize;
            enemyLocations = new List<GridCell>();
            oldEnemyLocationsOnScreen = new List<Enemy>();
            theGrid = new List<List<GridCell>>();
            for (int y = 0; y < gridCellDimensions.Y; y++)
            {
                List<GridCell> row = new List<GridCell>();
                foreach (Point p in navigableLocations)
                {
                    if (p.Y == y)
                    {
                        row.Add(new GridCell(p, gridCellSize));
                    }
                }
                if (gridMaxHorizontalSize < row.Count)
                {
                    gridMaxHorizontalSize = row.Count;
                }
                theGrid.Add(row);
            }
        }
        #endregion

        #region GETTER METHODS
        //Returns the player's exact location and updates his old location
        public Vector2 GetPlayerExactLocation {
          get
          {
            return playerExactLocation;
          }
          set {
              if (playerExactLocation != null)
              {
                  playerExactOldLocation = playerExactLocation;
              }
              playerExactLocation = value;
          } 
        }
        //Returns the grid cell size
        public Point GridCellSize { get { return gridCellSize; } }

        //Returns the gridcell that the player is currently on
        public GridCell PlayerGridCell { get { return playerLocation; } }

        //Returns a list of gridcells that are occupied by enemies
        public List<GridCell> EnemyGridCells { get { return enemyLocations; } }

        //Returns the height or the vertical length of the grid
        public int GridVerticalSize { get { return theGrid.Count; } }

        //Returns the size of the largest row in the grid
        public int MaxGridHorizontalSize { get { return gridMaxHorizontalSize; } }
        
        //Returns the the size of a row in the grid, throws an exception if it does not exist
        public int GridHorizontalSize(int row)
        {
            if (row > theGrid.Count)
            {
                throw new System.AccessViolationException("You cannot access this element in the grid");
            }
            return theGrid[row].Count;
        }

        //Returns the locations that at a specified row in the grid, Throws an exception if it does not exist
        public List<GridCell> getMoveableLocations(int row)
        {
            if (row > theGrid.Count)
            {
                throw new System.AccessViolationException("You cannot access this element in the grid");
            }
            return theGrid[row];
        }

        #endregion

        #region METHODS
        /// <summary>
        /// Updates the each enemy in the game with their specified locations in the grid.
        /// Also updates the player's location on the grid
        /// </summary>
        /// <param name="player">player</param>
        /// <param name="enemies">enemies</param>
        public void UpdateCellLocations(Player player, List<Enemy> enemies)
        {
            /* BUG catch invalid operations exceptions if Antibody leaves
             * the level.  The game will lag serverly if this happens but will not
             * crash.
             */ 
                if (oldEnemyLocationsOnScreen.Count > 0)
                {
                    foreach (Enemy cell in oldEnemyLocationsOnScreen)
                    {
                        try {
                          getGridCellAtLocation(cell).RemoveAllCells();
                        }
                        catch (InvalidOperationException) {
                        }
                    }
                }
                enemyLocations.Clear();
                oldEnemyLocationsOnScreen = new List<Enemy>(enemies);
                List<Enemy> cahcedEnemies = new List<Enemy>();
                foreach (Enemy enemy in enemies)
                {
                    try
                    {
                        GridCell enemyLocation = getGridCellAtLocation(enemy);
                        if (enemyLocation.ContainsCell(enemy))
                        {
                            enemyLocation.AddCell(enemy);
                            if (enemy is Antibody)
                            {
                                if (!enemyLocation.ContainsCell(player))
                                {
                                    enemyLocations.Add(enemyLocation);
                                }
                            }
                            else
                            {
                                enemyLocations.Add(enemyLocation);
                            }
                        }
                    }
                    catch (InvalidOperationException)
                    {
                        cahcedEnemies.Add(enemy);
                    }
                }

                foreach (Enemy enemy in cahcedEnemies) 
                {
                    (enemy as Antibody).reset();
                }

                if (playerExactOldLocation != Vector2.Zero)
                {
                    getGridCellAtLocation(playerExactOldLocation).RemoveAllCells();
                }

                GridCell playerGridCellLocation = getGridCellAtLocation(GetPlayerExactLocation);
                if (playerGridCellLocation.ContainsCell(player))
                {
                    playerGridCellLocation.AddCell(player);
                    playerLocation = playerGridCellLocation;
                }

        }

        /// <summary>
        /// Returns the cell at the location specified by the cell's center
        /// </summary>
        private GridCell getGridCellAtLocation(Cell cell)
        {
            return getGridCellAtLocation(cell.Center);
        }

        /// <summary>
        /// Returns the cell specified location in the grid
        /// </summary>
        /// <param name="location">location of the current cell</param>
        /// <returns>The grid cell that the cell is currently located in</returns>
        private GridCell getGridCellAtLocation(Vector2 location)
        {
            int columnKey = getHashKey(location.Y, gridCellSize.Y);
            if (columnKey < 0)
            {
                throw new System.InvalidOperationException();
            }
            float offset = location.X - theGrid[columnKey].First().PositionOnScreen.X;
            int x_key = getHashKey(location.X, gridCellSize.X);
            int rowKey = find(theGrid[columnKey], 0, theGrid[columnKey].Count - 1, x_key);
            if (rowKey < 0)
            {
                throw new System.InvalidOperationException();
            }
            return theGrid[columnKey][rowKey];
        }

        //Utility hash code method for efficient traversing down the grid.
        private int getHashKey(float y_coordinate, int divisor)
        {
            return (int)Math.Floor(y_coordinate / divisor);
        }

        #region DEBUG METHODS

        public void printAllGridCellDimensions()
        {
            foreach (List<GridCell> row in theGrid)
            {
                getAllRowLocations(row);
            }
        }

        public void getAllRowLocations(List<GridCell> row)
        {
            Console.WriteLine("//Row//");
            foreach (GridCell column in row)
            {
                Console.Write("( X: " + column.PositionInGrid.X + " , Y: " + column.PositionInGrid.Y + "  )");
            }
            Console.WriteLine();
        }

        public GridCell getGridCellThatCellIsContainedIn(Cell cell)
        {
            foreach (GridCell enemeyCell in EnemyGridCells)
            {
                if (enemeyCell.ContainsCell(cell))
                {
                    return enemeyCell;
                }
            }
            return null;
        }
        #endregion

        //Returns the grid cell location of the specifed row and key values.  Throws an exception if the specified
        //location at theGrid[row][key] does not exist.
        public GridCell getGridCellAtLocation(int row, int key)
        {
            if (key >= theGrid[row].Count)
            {
                throw new System.AccessViolationException("Unable to grab this element at row " + row);
            }
            return theGrid[row][key];
        }

        //Checks to see if the grid cell at row i and column j is contained within the grid.  This
        //method uses a utlity method find to search for a gridcell that has the correct corresponding
        //row value at column i.
        public int canGrabGridCellAtLocation(int i, int j) 
        {
            if (j < theGrid.Count)
            {
                int key = find(theGrid[j], 0, theGrid[j].Count - 1, i);
                return key;
            }
            return -1;
        }

        //Utilizes binary search to search for an element that has the same value as the 
        //the key value.
        private int find(List<GridCell> elements, int min, int max, int key)
        {
            int midpoint = 0;
            if (elements.Count == 0)
            {
                return -1;
            }

            do
            {
                midpoint = (int)Math.Floor((double)(min + (max - min) / 2));
                if (key > elements[midpoint].PositionInGrid.X)
                {
                    min = midpoint + 1;
                }
                else
                {
                    max = midpoint - 1;
                }
            }
            while (!(elements[midpoint].PositionInGrid.X == key) && !(min > max));
            if (elements[midpoint].PositionInGrid.X == key)
            {
                return midpoint;
            }
            else
            {
                return -1;
            }
        }
        #endregion
    }
        
}
