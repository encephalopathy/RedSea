using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Prototype
{
    class Pair
    {
        /* Replaced this class's use with Point - a built in class (int, int)/(X,Y)*/

        private int x;
        private int y;

        public Pair()
        {
            x = 0;
            y = 0;
        }

        public Pair(int x, int y)
        {
            this.x = x;
            this.y = y;
        }

        public int X { get { return x; } set { x = value; } }
        public int Y { get { return y; } set { y = value; } }
    }
}
