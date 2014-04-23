using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using FarseerPhysics.Dynamics;
using FarseerPhysics.Factories;

namespace RedSeaGame
{
    class RedBloodCell : Enemy
    {
        #region Fields and Constructor
        // Sets the torque on the T Cell
        protected const float tq = 10000000f;
        // Sets the maximum force on the T Cell
        protected const int fb = 100000000;
        //bool to check if the cell was previously infected
        private bool wasInfected;
        //bool to check is currently infected
        private bool isInfected;
        //The particle effect used if the red blood cell gets infected
        private ParticleEmitter infectionEffect;
        //Sets the restitution
        protected const float rest = 2.5f;
        
        //RedBloodCell Constructor
        public RedBloodCell(Vector2 location, ref World world, ref Player player) :
             base (location, ContentStore.RedBloodCell.Width, ref world, ref player, tq, fb)
        {
            SetRestitution(rest);
            texture = ContentStore.RedBloodCell;
            isInfected = false;
            wasInfected = isInfected;
            this.player = player;
            infectionEffect = null;
        }

        //Infected check constructor
        public bool IsInfected { get { return isInfected; } }
        #endregion

        #region Methods
        //Checks to see if the RedBloodCell has collided with the player
        public override void cellOnCollision(ref FixtureProxy thisCellFixture, ref FixtureProxy otherCellFixture)
        {
            if (hasCollided(ref thisCellFixture, ref otherCellFixture) && (player.CellFixture == thisCellFixture.Fixture ||
                player.CellFixture == otherCellFixture.Fixture))
            {
                if (isInfected == false)
                    ContentStore.GetCellSound().Play();
                isInfected = true;
            }
            base.cellOnCollision(ref thisCellFixture, ref otherCellFixture);
        }

        //Sets the state of the RedBloodCell to infected if it collided with player
        //and creates the particle effect.
        private void Infect()
        {
            if (isInfected && !wasInfected)
            {
                texture = ContentStore.RedBloodInfection;
                infectionEffect = new ParticleEmitter(center,
                    ContentStore.MalariaParticle, ContentStore.MalariaParticle.Width / 2, 12);
            }
        }

        /*public void Clean()
        {
            isInfected = false;
            texture = texStore.RedBloodCell;
        }*/
        #endregion

        #region Update and Draw
        //Calls the collision method and infect to check to see if a cell has been infected
        //and if it is calls the infection particle efffect. Whent the particle effect is done
        //then the particle effect is set to null to not run again on the same cell.
        public override void Update(GameTime gameTime)
        {
            Infect();
            wasInfected = isInfected;
            if (infectionEffect != null) {
                infectionEffect.Update(cellBody.Position);
                if (infectionEffect.Finish())
                {
                    infectionEffect = null;
                }
            }
            base.Update(gameTime);
        }

        //Plays the infection particle effect until it is done.
        public override void Draw(SpriteBatch spriteBatch)
        {
            if (infectionEffect != null)
            {
                infectionEffect.Draw(spriteBatch);
            }
            base.Draw(spriteBatch);
        }
        #endregion
    }
}
