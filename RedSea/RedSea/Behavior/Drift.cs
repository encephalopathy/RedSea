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
    class Drift : Behavior
    {
        #region Fields
        int ForceBound;
        //Drift takes in a cellBody that maninpulates the movement for the cell using the
        //Farseer physics engine.  The torque value determines how much that cell should
        //Spin initually
        //A Nice torque number would be 100000000

        //The amount of time this cell should not change a direction
        int driftTime;
        #endregion

        #region CONSTRUCTOR
        public Drift (ref Body cellBody, float torque, int bound)
        {
            ForceBound = bound;
            CellBody = cellBody;
            CellBody.ApplyTorque(torque);
            driftTime = 0;
        }
        #endregion

        #region METHODS
        //The move function takes in an float parameter that determines the bounds
        //of the number that will be randomly generated.  The randomly generated number determines what direction
        //the cell will travel in.
        protected override void Move()
        {
            //These vector values may change
            float DirectionX = ParticleHelper.RandomBetween(-ForceBound, ForceBound);
            float DirectionY = ParticleHelper.RandomBetween(-ForceBound, ForceBound);
            CellBody.ApplyForce(new Vector2(DirectionX, DirectionY));
        }

        public override void Update()
        {
            
            if (driftTime == 0)
            {
                Move();
                driftTime = 1000;
            }
            driftTime--;
        }
        #endregion
    }
}
