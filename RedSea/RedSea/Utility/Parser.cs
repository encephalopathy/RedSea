using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace RedSeaGame
{
    class Parser
    {
        #region Private Fields

        private Queue<String> representation;

        private int wallDivs;

        private Point gridDim;

        private Point levelDim;

        private List<Point> reachLocs;
        private List<Point> unreachLocs;

        private Point playerLoc;
        private Point doorLoc;
        private List<Point> redLocs;
        private List<Point> whiteLocs;
        private List<Point> tcellLocs;
        private List<Point> antibodyLocs;

        #endregion

        public Parser(String fileName)
        {
            representation = new Queue<string>();
            gridDim = Point.Zero;
            reachLocs = new List<Point>();
            unreachLocs = new List<Point>();
            playerLoc = Point.Zero;
            doorLoc = Point.Zero;
            redLocs = new List<Point>();
            whiteLocs = new List<Point>();
            tcellLocs = new List<Point>();
            antibodyLocs = new List<Point>();

            readFile(fileName);
            getWallDivisions();
            getGridDimensions();
            getLevelDimensions();
            readLines();
        }

        #region Public Properties

        public int WallDivisions { get { return wallDivs; } }
        public Point GridDimensions { get { return gridDim; } }
        public Point LevelDimensions { get { return levelDim; } }
        public List<Point> ReachableLocations { get { return reachLocs; } }
        public List<Point> UnreachableLocations { get { return unreachLocs; } }
        public Point PlayerInitialLocation { get { return playerLoc; } }
        public Point DoorLocation { get { return doorLoc; } }
        public List<Point> RedCellInitialLocations { get { return redLocs; } }
        public List<Point> WhiteCellInitialLocations { get { return whiteLocs; } }
        public List<Point> TCellInitialLocations { get { return tcellLocs; } }
        public List<Point> AntiBodyInitialLocations { get { return antibodyLocs; } }
        public static int MAX_SIZE_WIDTH { get; private set;}
        public static int MAX_SIZE_HEIGHT { get; private set; }

        #endregion

        // reads and stores the file as a queue of strings
        private void readFile(String fileName)
        {
            foreach (String line in File.ReadLines(fileName))
            {
                representation.Enqueue(line);
            }
        }

        // reads in the number of textures needed to draw the level walls
        private void getWallDivisions()
        {
            String firstLine = representation.First();
            wallDivs = Int16.Parse(firstLine);
            representation.Dequeue();
        }

        // reads in the dimensions of the grid
        private void getGridDimensions()
        {
            String secondLine = representation.First();
            string[] lineContents =secondLine.Split(' ');
            int x = Int16.Parse(lineContents[0]);
            int y = Int16.Parse(lineContents[1]);
            gridDim = new Point(x, y);
            representation.Dequeue();
        }

        // reads in the dimensions of the level
        private void getLevelDimensions()
        {
            String thirdLine = representation.First();
            string[] lineContents = thirdLine.Split(' ');
            int x = Int16.Parse(lineContents[0]);
            if (x > MAX_SIZE_WIDTH)
            {
                MAX_SIZE_WIDTH = x;
            }
            int y = Int16.Parse(lineContents[1]);
            if (y > MAX_SIZE_HEIGHT)
            {
                MAX_SIZE_HEIGHT = y;
            }
            levelDim = new Point(x, y);
            representation.Dequeue();
        }

        // reads in info used to initialize grid and cell positions
        private void readLines()
        {
            int x;
            int y;
            string line;
            char[] charar;

            for (y = 0; y < gridDim.Y; y++)
            {
                line = representation.Dequeue();
                charar = line.Replace(" ", string.Empty).ToCharArray();
                for (x = 0; x < gridDim.X; x++)
                {
                    storeCharMeaning(charar[x], x, y);
                }
            }
        }

        // determines meaning of the chars in the info section of the file
        private void storeCharMeaning(char c, int x, int y)
        {
            switch (c)
            {
                case ('X'):
                    unreachLocs.Add(new Point(x, y));
                    break;
                case ('O'):
                    reachLocs.Add(new Point(x, y));
                    break;
                case ('R'):
                    redLocs.Add(new Point(x, y));
                    reachLocs.Add(new Point(x, y));
                    break;
                case ('W'):
                    whiteLocs.Add(new Point(x, y));
                    reachLocs.Add(new Point(x, y));
                    break;
                case ('T'):
                    tcellLocs.Add(new Point(x, y));
                    reachLocs.Add(new Point(x, y));
                    break;
                case ('A'):
                    antibodyLocs.Add(new Point(x, y));
                    reachLocs.Add(new Point(x, y));
                    break;
                case ('D'):
                    doorLoc = new Point(x, y);
                    reachLocs.Add(new Point(x, y));
                    break;
                case ('P'):
                    playerLoc = new Point(x, y);
                    reachLocs.Add(new Point(x, y));
                    break;
            }
        }

    }
}
