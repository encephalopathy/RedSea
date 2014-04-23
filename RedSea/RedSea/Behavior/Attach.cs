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
    class Attach : Behavior
    {
        #region Fields
        //Player's current location
        Player playerLocation;

        //This cell's old location
        Vector2 oldPlayerLocation;
        #endregion

        #region CONSTRUCTOR
        /// <summary>
        /// Sets the cell's position directly onto the player
        /// </summary>
        public Attach(ref Body CellBody, ref Player player)
        {
            this.CellBody = CellBody;
            CellBody.AngularVelocity = 0f;
            CellBody.LinearVelocity = Vector2.Zero;
            playerLocation = player;
            oldPlayerLocation = CellBody.Position;
        }
        #endregion

        #region METHODS
        //Moves the cell proportionally to where the player current center location is
        protected override void Move()
        {
            float XDisplacement = playerLocation.Center.X - oldPlayerLocation.X;
            float YDisplacement = playerLocation.Center.Y - oldPlayerLocation.Y;
            CellBody.Position = new Vector2(CellBody.Position.X + XDisplacement, CellBody.Position.Y + YDisplacement);
            oldPlayerLocation = CellBody.Position;
        }

        //Moves the cell.
        public override void Update()
        {
            Move();
        }
        #endregion
    }
}
