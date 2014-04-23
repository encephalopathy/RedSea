using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using FarseerPhysics.Dynamics;
using FarseerPhysics.Factories;
using FarseerPhysics.Dynamics.Contacts;

namespace RedSeaGame
{
    class Cell
    {
        #region Fields and constructor
        // center coords of grid cell
        protected Vector2 center;

        // bounding volume for collision
        protected Rectangle bound;

        // texture to be drawn in grid cell
        protected Texture2D texture;

        // diameter of the cell
        protected int diam;

        //Fixture for the cell (Farseer)
        protected Fixture cellFixture;

        //Body for the cell (Farseer)
        protected Body cellBody;

        //The number used to define the ngular dampening on the cells
        const float angdamp = .5f;

        public Vector2 Center { get{return center; } set{center = value;}}
        public Rectangle BoundingBox { get { return bound; } }
        public Texture2D Texture { get { return texture; } }
        public Fixture CellFixture { get { return cellFixture; } }
        public Body CellBody { get { return cellBody; } }

        public Cell() { }

        //Base Cell constructor
        public Cell(Vector2 location, int diameter, ref World world)
        {
            diam = diameter;
            bound = new Rectangle((int)center.X, (int)center.Y, diam, diam);
            cellFixture = FixtureFactory.CreateCircle(world, diam / 2, 1, center);
            cellBody = cellFixture.Body;
            cellBody.BodyType = BodyType.Dynamic;
            cellBody.AngularDamping = angdamp;
            center = location;
            cellBody.Position = center;
            world.ContactManager.OnBroadphaseCollision += cellOnCollision;
        }
        #endregion

        #region Update and Draw
        public virtual void Update(GameTime gameTime)
        {
            center = CellBody.Position;
        }

        //If the white blood cell bounces against any cell, it stops attacking
        public virtual void cellOnCollision(ref FixtureProxy thisCellFixture, ref FixtureProxy otherCellFixture)
        {
        }

        protected bool hasCollided(ref FixtureProxy thisCellFixture, ref FixtureProxy thatCellFixture)
        {
            if (cellFixture == thisCellFixture.Fixture || cellFixture == thatCellFixture.Fixture)
            {
                return true;
            }
            return false;
        }

        public virtual void Draw(SpriteBatch cellSprite)
        {
            //cellSprite.Draw(texture, location, Color.White);
            cellSprite.Draw(texture, new Rectangle((int)(center.X), (int)(center.Y), texture.Width, texture.Height), null, 
                Color.White, CellBody.Rotation, new Vector2((float)(texture.Width / 2), (float)texture.Height / 2), 
                SpriteEffects.None, .5f);
        }
        #endregion
    }
}
