using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace RedSeaGame
{
    class ImageParser
    {
        // The textures used to draw each segment of the wall
        Texture2D wallTextureTransparent;
        Texture2D wallTextureHorizontal;
        Texture2D wallTextureVertical;
        Texture2D wallTextureTopLeft;
        Texture2D wallTextureTopRight;
        Texture2D wallTextureBottomLeft;
        Texture2D wallTextureBottomRight;
        
        // The image used to build the wall
        Texture2D levelRepresentation;
        Color[,] levelRepresentationData;

        public int RepWidth { get { return levelRepresentation.Width; } }
        public int RepHeight { get { return levelRepresentation.Height; } }

        // The array describing wall segment data
        public Texture2D[,] wallTexture;

        public void LoadContent(ContentManager Content)
        {
            // assumption that representation image is square
            levelRepresentation = Content.Load<Texture2D>("RepresentationImage");

            wallTextureTransparent = Content.Load<Texture2D>("WallTransparent");

            wallTextureHorizontal = Content.Load<Texture2D>("WallHorizontal");
            wallTextureVertical = Content.Load<Texture2D>("WallVertical");

            wallTextureTopLeft = Content.Load<Texture2D>("WallTopLeft");
            wallTextureTopRight = Content.Load<Texture2D>("WallTopRight");
            wallTextureBottomLeft = Content.Load<Texture2D>("WallBottomLeft");
            wallTextureBottomRight = Content.Load<Texture2D>("WallBottomRight");

            MakeRepresentation();

            MakeWallData();
        }

        private void MakeRepresentation()
        {
            // store the height and width of the image
            int w = levelRepresentation.Width;
            int h = levelRepresentation.Height;

            // create the 2d representation array
            levelRepresentationData = new Color[w, h];

            // create an array to put the image data into
            Color[] tempArray = new Color[w * h];

            // get the image data
            levelRepresentation.GetData(tempArray);

            // put data into the 2d array
            for (int y = 0; y < h; y++)
            {
                for (int x = 0; x < w; x++)
                {
                    levelRepresentationData[x, y] = tempArray[w * y + x];
                }

            }
        }

        private void MakeWallData()
        {
            // store the height and width of the representation image
            int w = levelRepresentation.Width;
            int h = levelRepresentation.Height;

            // create 2d array for storing wall data
            wallTexture = new Texture2D[w, h];

            for (int y = 0; y < h; y++)
            {
                for (int x = 0; x < w; x++)
                {
                    wallTexture[x, y] = GetWallData(x, y);
                }
            }
        }

        public Texture2D GetWallData(int x, int y)
        {
            Color c = levelRepresentationData[x, y];

            Texture2D result = null;

            System.Console.WriteLine("c.ToString(): " + c.ToString());

            switch (c.ToString())
            {
                case "{R:0 G:0 B:0 A:0}":
                    // Transparent
                    result = wallTextureTransparent;
                    break;
                case "{R:0 G:250 B:0 A:255}":
                    // Horiztonal
                    result = wallTextureHorizontal;
                    break;
                case "{R:0 G:150 B:0 A:255}":
                    // Vertical
                    result = wallTextureVertical;
                    break;
                case "{R:250 G:0 B:0 A:255}":
                    // Top Left
                    result = wallTextureTopLeft;
                    break;
                case "{R:200 G:0 B:0 A:255}":
                    // Top Right
                    result = wallTextureTopRight;
                    break;
                case "{R:150 G:0 B:0 A:255}":
                    // Bottom Left
                    result = wallTextureBottomLeft;
                    break;
                case "{R:100 G:0 B:0 A:255}":
                    // Bottom Right
                    result = wallTextureBottomRight;
                    break;

            }

            return result;
        }

    }
}
