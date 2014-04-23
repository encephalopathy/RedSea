using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using FarseerPhysics.Dynamics;
using FarseerPhysics.Factories;

namespace RedSeaGame
{
    class Player : Cell
    {
        #region Fields and Constructor
        //The restitution for the player (Farseer)
        private const float rest = 2f;
        //The friction for the player (Farseer)
        private const float frick = 3f;
        //The variable for slowing down the player over time
        private const float lindamp = .3f;

        public Player(Vector2 location, ref World world) :
             base(location, 10, ref world)
        {
            cellFixture.Restitution = rest;
            cellFixture.Friction = frick;
            texture = ContentStore.Malaria;
            cellBody.LinearDamping = lindamp;
        }
        #endregion

        #region Update and Draw
        public void Update(KeyboardState keyInput)
        {
            //The amount the force changed on each key press
            float forceIncrement = 20000;
            Vector2 force = Vector2.Zero;

            //Changes the force based off what key is pressed
            if (keyInput.IsKeyDown(Keys.Up))
                force += new Vector2(0, -forceIncrement);
            if (keyInput.IsKeyDown(Keys.Down))
                force += new Vector2(0, forceIncrement);
            if (keyInput.IsKeyDown(Keys.Left))
                force += new Vector2(-forceIncrement, 0);
            if (keyInput.IsKeyDown(Keys.Right))
                force += new Vector2(forceIncrement, 0);

            cellBody.ApplyForce(force);
            center = cellBody.Position;
        }

        //DRAW
        public override void Draw(SpriteBatch spriteBatch)
        {
            base.Draw(spriteBatch);
        }
        #endregion

    }
}
