using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using FarseerPhysics.Dynamics;
using FarseerPhysics.Factories;
using FarseerPhysics.Common.Decomposition;
using FarseerPhysics.Common;

namespace RedSeaGame
{
    class WallSplitter
    {
        World world;                // Farseer world used to create collidable bodies in
        Texture2D texture;          // texture of the wall fragment
        List<Vertices> fixList;     // list of vertices forming a new convex polygon
        Vertices verts;             // vertices of the concave polygon drawn in the texture
        List<Fixture> compund;      // the list of fixtures forming the compound polygon
        uint[] data;                // array used to store texture's pixel data

        public WallSplitter(Texture2D texture, ref World world)
        {
            this.texture = texture;
            this.world = world;
        }

        public List<Fixture> MakeWall()
        {
            data = new uint[this.texture.Width * this.texture.Height];
            texture.GetData(data);
            verts = PolygonTools.CreatePolygon(data, this.texture.Width, this.texture.Height, true);
            fixList = FlipcodeDecomposer.ConvexPartition(verts);
            compund = FixtureFactory.CreateCompoundPolygon(world, fixList, 1);
            return compund;
        }


    }
}
