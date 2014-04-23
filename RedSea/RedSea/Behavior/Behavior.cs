using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using FarseerPhysics.Dynamics;
using FarseerPhysics.Factories;

namespace RedSeaGame
{
    /// <summary>
    /// The behavior class is a abstract class that enables particular behaviors to be made.
    /// All behaviors share similar attributes; Therefore, all classes that inherit behavior
    /// must provide the use of the methods specified below.
    /// </summary>
    abstract class Behavior
    {
        protected Body CellBody;
        protected abstract void Move();
        public abstract void Update();
    }
}
