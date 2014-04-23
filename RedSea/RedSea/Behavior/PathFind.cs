#region Using Statements
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
#endregion

namespace RedSeaGame
{
    /// <summary = A* PATH FINDING>
    /// Determines a path from the starting location to an end location by keeping track of current location's
    /// distance away from the source, and distance towards the end location.
    /// </summary>
    class PathFind
    {
        
        #region Travel State
        /// <summary>
        /// This class is used to determine whether a particular location on our grid intereface is a
        /// movable location on the display screen.  A* does not travel on grid cells that have the fields:
        /// TRAVELED and UNPASSABLE.  This class is used to optimize the efficiency of A* by keeping track
        /// of locations already discovered.
        /// </summary>
        public enum TravelState
        {
            UNPASSABLE,
            UNDISCOVERD,
            DISCOVERED
        }
        #endregion

        #region Fields
        //This list is used to hold the adjacent grid cells that A* discovering while traversing the grid.
        List<GridCell> adjacentGridCells = new List<GridCell>();
        
        //Possible paths holds the locations that A* were discovered while traversing the grid.  These locations
        //could be used to back track to a previous location if A* detects a dead end along its current path.
        List<GridCell>  possiblePaths = new List<GridCell>();

        //A Linked List that holds the shortest path from the start location to the end location on the grid.
        Stack<GridCell> path = new Stack<GridCell>();

        //Stores a reference to the grid that holds all movable locations.
        Grid grid;

        //A 2 dimensional array of travel states that is used to store the state of each location in the grid.
        TravelState[,] discovered;
        GridCell[,] parents;

        //Stores the distance away from the starting source.  This is used for when A* reaches a dead end along
        //its current path and has to back track to a location that is closest to the end source but furthest from
        //the starting source.
        int[,] distances;
        #endregion

        #region CONSTRUCTOR
        /// <summary>
        /// Initializes all the fields of the PathFind class.  The dimensions of the lists fields depend on the movable locations
        /// inside the grid. The dimensions of the array fields are determined by the vertical length of the grid but only movable
        /// locations are used.
        /// </summary>
        public PathFind(Grid grid, GridCell start, GridCell end)
        {
            try
            {
                this.grid = grid;
                float estimateDistance = distEstimate(start.PositionCenterOnScreen.X, start.PositionCenterOnScreen.Y,
                                                      end.PositionCenterOnScreen.X, end.PositionCenterOnScreen.Y);
                int[] dim = new int[grid.GridVerticalSize];
                for (int i = 0; i < grid.GridVerticalSize; i++)
                {
                    dim[i] = grid.GridHorizontalSize(i);
                }

                discovered = new TravelState[Parser.MAX_SIZE_HEIGHT, Parser.MAX_SIZE_WIDTH];
                parents = new GridCell[Parser.MAX_SIZE_HEIGHT, Parser.MAX_SIZE_WIDTH];
                distances = new int[Parser.MAX_SIZE_HEIGHT, Parser.MAX_SIZE_WIDTH];

                makePassableLocations();
                makeOccupiedLocations(grid.EnemyGridCells);

                findPath(estimateDistance, start, end);
            }
            catch (NullReferenceException e)
            {
                print(e.Message);
                print(e.Source);
                print(e.StackTrace);
            }
        }
        #endregion

        #region METHODS

        #region DEBUG METHODS
        /// <summary>
        /// Useful debug methods if A* is ever in need of modification.
        /// </summary>
        private void print(string message)
        {
            Console.WriteLine("A*: " + message);
        }

        void printPath()
        {
            print("A* path");
            foreach (GridCell cell in path)
            {
                print("( " + cell.PositionInGrid.X + " , " + cell.PositionInGrid.Y + " )");
            }
        }
        #endregion

        //Returns the path
        public Stack<GridCell> getPath { get { return path; } }

        /// <summary "Heuristic"> 
        /// Is used to caclulate the mahattan distance from point (x , y) to point
        /// (otherX, otherY).
        /// </summary>
        float distEstimate(float x, float y, float otherX, float otherY)
        {
            return Math.Abs((x - otherX)) + Math.Abs((y - otherY));
        }

        
        /// <summary>
        /// Uses A* to find the shortest path from the start location to end location.  A* keeps track of
        /// how far it has traveled from where it started initially as well as how far it needs to go
        /// in order to get to the source.
        /// </summary>
        /// <param name="estDistance">Starting distance estimate from the start to end location</param>
        /// <param name="start">Starting location</param>
        /// <param name="end">End location</param>
        void findPath(float estDistance, GridCell start, GridCell end)
        {
            GridCell currentLocation = start;
            adjacentGridCells.Add(currentLocation);
            while (!isEmpty(adjacentGridCells))
            {
                GridCell shortestLocation = getShortestAdjDirection(adjacentGridCells, end);
                if (shortestLocation.Equals(end))
                {
                    break; //We are done
                }
                else
                {
                    adjacentGridCells.Remove(shortestLocation);
                    possiblePaths.Add(shortestLocation); //Add lowest f_score value to the closed set
                    List<GridCell> neighbors = addAdjacentGridCells(shortestLocation);
                    Point startPoint = shortestLocation.PositionInGrid;
                    foreach (GridCell location in neighbors)
                    {
                        Point currentPoint = location.PositionInGrid;

                        if (!adjacentGridCells.Contains(location))
                        {
                            adjacentGridCells.Add(location);
                            parents[currentPoint.X, currentPoint.Y] = shortestLocation;
                        }
                        else if (distances[startPoint.X, startPoint.Y] + 1 < distances[currentPoint.X, currentPoint.Y])
                        {
                            parents[currentPoint.X, currentPoint.Y] = shortestLocation;
                        }
                        distances[currentPoint.X, currentPoint.Y] = distances[startPoint.X, startPoint.Y] + 1;
                    }
               }
           }
            path.Push(reconstructPath(end));
        }

        
        /// <summary>
        /// Reconstucts the path by recursiving calling the end location's parent node and adding that node
        /// to the path
        /// </summary>
        /// <param name="end">Destination location</param>
        /// <returns>The start location</returns>
        private GridCell reconstructPath(GridCell location)
        {
            Point parentNodePosition = location.PositionInGrid;
            if (parents[parentNodePosition.X, parentNodePosition.Y] != null)
            {
                path.Push(location);
                return reconstructPath(parents[parentNodePosition.X, parentNodePosition.Y]);
            }
            else
            {
                return location;
            }
        }

        //Utility functionm used to check if the given list is empty.
        bool isEmpty(List<GridCell> set)
        {
            return (set.Count == 0);
        }

        /// <summary>
        /// Finds the location that is closest to the end location at the current source.  If there are any other locations
        /// that are closer to the end location they are added to a list that keeps track of locations that will be added
        /// to the adjacencentGridCells list.  This optimizes the search space of A* but also restricts A* to search repeated
        /// grid cells that could potentially be shorter than the discovered locations on A* next search pass.
        /// </summary>
        /// <param name="openSet">Adjacent locations with the first element being the current location</param>
        /// <param name="end">End location</param>
        /// <returns>Returns the location that is closest to the end location, Throws a null pointer exception if openSet is empty</returns>
        GridCell getShortestAdjDirection(List<GridCell> openSet, GridCell end) 
        {
            if (isEmpty(openSet))
            {
                throw new System.NullReferenceException("openSet does not exist");
            }
            Point currPoint = openSet.First().PositionInGrid;
            Point endPoint = end.PositionInGrid;

            float f_score = distEstimate(currPoint.X, currPoint.Y, endPoint.X, endPoint.Y) + distances[currPoint.X, currPoint.Y];
            int index = 0;
            for (int i = 0; i < openSet.Count; i++)
            {
                float curr_f_score = distEstimate(openSet[i].PositionInGrid.X, openSet[i].PositionInGrid.Y,
                    end.PositionInGrid.X, end.PositionInGrid.Y)
                    + distances[openSet[i].PositionInGrid.X, openSet[i].PositionInGrid.Y];
                Point currentLocation = openSet[i].PositionInGrid;
                if (f_score > curr_f_score)
                {
                    index = i;
                }
            }

            return openSet[index];
        }

        /// <summary>
        /// Sets the occupied locations on the grid.  The cell that uses A* cannot pass these locations
        /// All occupied locations are contained within the movable locations.
        /// </summary>
        /// <param name="Occupied">List that stores the occupied locations</param>
        private void makeOccupiedLocations(List<GridCell> Occupied)
        {
            //print("Make occupied locations");
            if (Occupied.Count > discovered.Length)
            {
                throw new System.NullReferenceException("List<GridCell>.Count > number " +
                "of GridCells in (method: makeOccupiedLocations) in class: PathFind");
            }
            foreach (GridCell location in Occupied)
            {
                discovered[location.PositionInGrid.X, location.PositionInGrid.Y] = TravelState.UNPASSABLE;
            }
        }

        /// <summary>
        /// Creates the passable locations in the grid.  These locations depend on the length and width dimensions of the
        /// grid.  Locations that can be traversed are marked as UNDISCOVERED.  Any other locations are marked, by default,
        /// as UNPASSABLE.
        /// </summary>
        private void makePassableLocations()
        {
            //print("Make passable: ");
            for (int i = 0; i < grid.GridVerticalSize; i++)
            {
                List<GridCell> row = grid.getMoveableLocations(i);
                //grid.getAllRowLocations(row);
                foreach (GridCell location in row)
                {
                    //Console.WriteLine(location.PositionInGrid);
                    discovered[location.PositionInGrid.X, location.PositionInGrid.Y] = TravelState.UNDISCOVERD;
                }
            }
        }


        /// <summary>
        /// Grabs the adjacent locations and stores them in the set.  The locations added to the 
        /// set are added in respect to the adjacent locations of the current location.  These adjacent
        /// locations are marked on the left, right, up, down, diagonal left up, right up, etc.
        /// </summary>
        /// <param name="currentLocation">Current location during at this process</param>
        /// <returns>A modified list that contains the adjacent locations with respect to the current location</returns>
        private List<GridCell> addAdjacentGridCells(GridCell currentLocation)
        {
            List<GridCell> set = new List<GridCell>();
            Point position = currentLocation.PositionInGrid;
                int currI = position.X;
                int currJ = position.Y;

                grabAdjacentGridCell(set, currI - 1, currJ - 1);
                grabAdjacentGridCell(set, currI - 1, currJ);
                grabAdjacentGridCell(set, currI - 1, currJ + 1);
                grabAdjacentGridCell(set, currI, currJ - 1);
                grabAdjacentGridCell(set, currI, currJ + 1);
                grabAdjacentGridCell(set, currI + 1, currJ - 1);
                grabAdjacentGridCell(set, currI + 1, currJ);
                grabAdjacentGridCell(set, currI + 1, currJ + 1);
            return set;
        }

        /// <summary>
        /// Calls a method contained within the grid class that check to see if a particular grid cell
        /// is contained within that exact location.  If it is, it adds it to the adjacency list and sets
        /// marks that location as discovered
        /// </summary>
        /// <param name="set">adjacentGridCell list</param>
        /// <param name="x">Horizontal position in the grid</param>
        /// <param name="y">Vertical position in the grid</param>
        /// <param name="startDistance">Distance from the inital starting location to the current source</param>
        private void grabAdjacentGridCell(List<GridCell> set, int x, int y)
        {
            int canGrab = grid.canGrabGridCellAtLocation(x, y);
            if (canGrab > 0)
            {
                if (discovered[x, y] == TravelState.UNDISCOVERD)
                {
                    set.Add(grid.getGridCellAtLocation(y, canGrab));
                    
                    discovered[x, y] = TravelState.DISCOVERED;
                }
            }
        }
        #endregion
    }
}
