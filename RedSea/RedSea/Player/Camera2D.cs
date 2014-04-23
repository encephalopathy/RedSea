using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace RedSeaGame
{
    // code based on example found at
    /* http://www.david-amador.com/2009/10/xna-camera-2d-with-zoom-and-rotation */

    class Camera2D
    {
        protected float _zoom;          // camera zoom
        public Matrix _transformation;  // matrix transformation
        public Vector2 _position;       // camera position
        protected float _rotation;      // camera rotation

        public Camera2D()
        {
            _zoom = 1.0f;
            _rotation = 0.0f;
            _position = Vector2.Zero;
        }

        public float Zoom
        {
            get { return _zoom;}
            set { _zoom = value;if (_zoom < 0.1f) _zoom = 0.1f; }
            // note: negative zoom will flip image
        }

        public float Rotation
        {
            get { return _rotation; }
            set { _rotation = value; }
        }

        public Vector2 Position
        {
            get { return _position; }
            set { _position = value; }
        }

        public void Move(Vector2 amount)
        {
            _position += amount;
        }

        public Matrix get_transformation(GraphicsDevice graphicsDevice)
        {
            _transformation = 
                Matrix.CreateTranslation(
                new Vector3(-_position.X, -_position.Y, 0)) *
                Matrix.CreateRotationZ(Rotation) *
                Matrix.CreateScale(new Vector3(Zoom, Zoom, 0)) *
                Matrix.CreateTranslation(new Vector3(
                    graphicsDevice.Viewport.Width / 2, 
                    graphicsDevice.Viewport.Height / 2, 0));

            return _transformation;
        }
    }
}
