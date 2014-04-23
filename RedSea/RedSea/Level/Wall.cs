using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using FarseerPhysics.Common;
using FarseerPhysics.Common.Decomposition;
using FarseerPhysics.Dynamics;
using FarseerPhysics.Factories;

namespace RedSeaGame
{
    class Wall
    {
        private int width;                              // width of level
        private int height;                             // height of level
        private List<List<Fixture>> wallFixtures;       // list of all the fixture collections forming the wall fragments
        private List<Texture2D> wallTextures;           // list of all the wall fragments' textures
        private Texture2D bgTexture;                    // texture for the background
        private WallSplitter splitter;                  // WallSplitter object used for creating collidable polygons

        public List<List<Fixture>> WallFixtures { get { return wallFixtures; } }
        public List<Texture2D> WallTexture { get { return wallTextures; } }

        public Wall(int width, int height, List<Texture2D> textures, Texture2D background, ref World world)
        {
            this.width = width;
            this.height = height;

            wallTextures = textures;

            bgTexture = background;

            wallFixtures = new List<List<Fixture>>();
            
            foreach (Texture2D tex in textures)
            {
                // create and add a list of fixtures forming a collidable, convex polygon
                splitter = new WallSplitter(tex, ref world);
                wallFixtures.Add(splitter.MakeWall());
            }
            
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            // draw the background on the very bottom
            spriteBatch.Draw(bgTexture, new Rectangle(0, 0, width, height), null, Color.White, 0, Vector2.Zero, SpriteEffects.None, 1f);
            
            // draw each wall fragment
            foreach (Texture2D tex in wallTextures)
                spriteBatch.Draw(tex, new Rectangle(0,0,width,height),null, Color.White, 0, Vector2.Zero, SpriteEffects.None, .1f);
        }
    }
}
