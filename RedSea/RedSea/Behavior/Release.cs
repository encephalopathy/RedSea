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
    class Release : Behavior
    {
        #region Fields
        //Drift time until this cell should change behavior
        int driftTime;

        //The cell's behavior
        Behavior cellBehavior;
        #endregion

        #region CONSTRUCTOR
        /// <summary>
        /// Initializes the fields of the Release object and applies a linear impulse on this cell
        /// and sends it flying to a random direction.
        /// </summary>
        /// <param name="CellBody">Cell body</param>
        /// <param name="CellBehavior">Current cell behavior</param>
        /// <param name="blastLocation">The amount of fource</param>
        /// <param name="gameTime">Current game Time</param>
        /// <param name="lifeTime">The time in which this cell not change its behavior to drift</param>
        /// <param name="moveImpulse">The amount of momentum that will be applied to this cell</param>
        public Release(ref Body CellBody, ref Behavior CellBehavior, Vector2 direction, GameTime gameTime,
            int lifeTime, int moveImpulse)
        {
            //Console.WriteLine("RELEASE ME");
            this.CellBody = CellBody;
            cellBehavior = CellBehavior;
            CellBody.ApplyLinearImpulse(new Vector2(moveImpulse * direction.X, moveImpulse * direction.Y));
            driftTime = lifeTime;
        }
        #endregion

        #region METHODS
        //Changes the behavior to drift when this cell is done drifting towards its inital direction
        protected override void Move()
        {
            
            if (driftTime == 0)
            {
                float torque = 100000000f;
                int forceBound = 100000000;
                cellBehavior = new Drift(ref CellBody, torque, forceBound);
            }
            driftTime--;
        }

        public override void Update()
        {
            Move();
        }
        #endregion
    }
}
