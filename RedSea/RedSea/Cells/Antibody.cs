using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using FarseerPhysics.Dynamics;
using FarseerPhysics.Factories;
using Microsoft.Xna.Framework.Input;

namespace RedSeaGame
{
    class Antibody : Enemy
    {
        #region Fields

        //The amount of clicks to take in order to get the antibody off the player
        private int life;

        //Flag to check if the antibody is stuck on the player
        private bool stuck;

        //Flag to check to see if the antibody was stuck on the player
        private bool wasStuck;

        //The absolute amount of force that is applied to the antibody when it drifts
        private const int forceBound = 100000000;

        //The amount of torque that is applied to an antibody when it is drifting
        private const float torque = 100000000f;

        private Vector2 originalLoc;

        //Getter flag that tells the other white blood cells that it is now attached to the player
        public bool detected { get; private set; }
        #endregion

        #region CONSTRUCTOR
        //Constructor
        public Antibody(Vector2 location, ref World gameWorld, ref Player player) :
            base(location, ContentStore.Antibody.Width, ref gameWorld, ref player, torque, forceBound)
        {
            texture = ContentStore.Antibody;
            stuck = false;
            SetRestitution(5f);
            range = 0;
            detected = false;
            wasStuck = stuck;
            originalLoc = location;
        }
        #endregion

        #region METHODS
        //If this cell has collided with a player, the stuck flag is true meaning that the cell is stuck to player,
        //And the attach behavior is executed
        public override void cellOnCollision(ref FixtureProxy thisCellFixture, ref FixtureProxy otherCellFixture)
        {
            if (hasCollided(ref thisCellFixture, ref otherCellFixture) && (player.CellFixture == thisCellFixture.Fixture ||
                player.CellFixture == otherCellFixture.Fixture))
            {
                if (!(cellBehavior is Attach))
                {
                    stuck = true;
                    cellBehavior = new Attach(ref cellBody, ref player);
                    life = 8;
                    detected = true;
                }
            }
            base.cellOnCollision(ref thisCellFixture, ref otherCellFixture);
        }

        public void reset()
        {
            cellBody.Position = originalLoc;
            cellBehavior = new Drift(ref cellBody, torque, forceBound);
        }

        //Checks to see if the player has pressed to space bar, After 8 clicks, the anitbody is released from the player
        public void getOff(KeyboardState oldKeyState, KeyboardState newKeyState)
        {
            if (cellBehavior is Attach)
            {
              if (oldKeyState.IsKeyDown(Keys.Space) && newKeyState.IsKeyUp(Keys.Space))
              {
                  ContentStore.AntibodyOff.Play();
                  life--;
              }
            }
        }

        //Unattaches from the player if the antibody was stuck on the player but not isn't.  The antibody's
        //behavior is set to the release behavior
        private void UnattachFromPlayer(GameTime gameTime)
        {
            if (wasStuck && !stuck)
            {
                float theta = ParticleHelper.RandomBetween(0f, (float)(2*Math.PI));
                float radius = (float)Math.Sqrt(ContentStore.Malaria.Width * ContentStore.Malaria.Width + 
                    ContentStore.Malaria.Height * ContentStore.Malaria.Height);
                Vector2 moveDirection = new Vector2(radius * (float)Math.Cos(theta), radius * (float)Math.Sin(theta));
                cellBody.Position = Center + moveDirection;
                moveDirection.Normalize();
                cellBehavior = new Release(ref cellBody, ref cellBehavior, moveDirection, gameTime, 40, 100000000);
            }
        }

        //A check to see if the antibody should release from the player
        private void release()
        {
            if (life == 0)
            {
                detected = false;
                stuck = false;
            }
        }

        //Updates the location of the antibody and checks to see if the antibody is stuck to the
        //player or not.
        public override void Update(GameTime gameTime)
        {
            release();
            UnattachFromPlayer(gameTime);
            wasStuck = stuck;
            if (cellBehavior is Attach)
            {
                ((Attach)cellBehavior).Update();
            }
            base.Update(gameTime);
        }

        //Draws the antibody on the screen
        public override void Draw(SpriteBatch cellSprite)
        {
            cellSprite.Draw(texture, new Rectangle((int)(center.X), (int)(center.Y), texture.Width, texture.Height), null,
                Color.White, CellBody.Rotation, new Vector2((float)(texture.Width / 2), (float)texture.Height / 2),
                SpriteEffects.None, 0.8f);
            if (stuck)
                cellSprite.Draw(ContentStore.AttachNotice, new Vector2(center.X + 20, center.Y + 20), Color.White);
        }
        #endregion
    }
}
