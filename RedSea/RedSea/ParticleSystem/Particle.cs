#region Using Statements
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using FarseerPhysics.Dynamics;
using FarseerPhysics.Factories;
#endregion

namespace RedSeaGame
{
    class Particle
    {
        #region Fields

        const float MAX_IMAGE_SIZE = 1f;

        //The position of the particle on the screen
        Vector2 position;

        //The position of the particle emitter
        Vector2 emitterLocation;

        //The distance from where the particle was created to where the center location of the particle emitter
        Vector2 emitterToPositionDisplacement;

        //Sprite texture for the particle
        Texture2D particleTexture;

        //A opacity counter for this sprite
        Color currentOpacity;

        //A scaler to determine the size of our sprite
        float imageScale;
        #endregion

        #region CONSTRUCTOR

        /// <summary>
        /// Used to initialize the x, y coordinates of this particle, along with its texture, the location of
        /// the particle emitter.
        /// </summary>
        /// <param name="x">X coordinate for the particle</param>
        /// <param name="y">y coordinate for the particle</param>
        /// <param name="particleTexture">texture for the particle</param>
        /// <param name="emitterLocation">particle emitter location that generated this particle</param>
        public Particle(float x, float y, Texture2D particleTexture, Vector2 emitterLocation)
        {
            position = new Vector2(x, y);
            this.particleTexture = particleTexture;
            this.emitterLocation = emitterLocation;
            imageScale = MAX_IMAGE_SIZE;
            emitterToPositionDisplacement = position - emitterLocation;
            currentOpacity = Color.White;
        }

        #endregion

        #region METHODS
        /// <summary>
        /// Resets the position of this particle and sets the particle's opacity to 255
        /// </summary>
        public void Create()
        {
            currentOpacity = Color.White;
            position = emitterLocation + emitterToPositionDisplacement;
        }

        /// <summary>
        /// Checks to see if this animation is done
        /// </summary>
        /// <returns></returns>
        public bool AnimationDone()
        {
            return currentOpacity.A <= 0;
        }

        /// <summary>
        /// Animates the particle.  If is still animating, move the particle 1 away from the emitter
        /// location.  If this particle is done animating, move the emitter location to the center of the player
        /// and also move the location in which this particle will be spawned on the screen 
        /// </summary>
        /// <param name="newLocation"></param>
        public void Update(Vector2 newLocation)
        {
            if (AnimationDone())
            {
                Vector2 translation = newLocation - emitterLocation;
                position += translation;
                emitterLocation = newLocation;
                imageScale = 1;
            }
            else
            {
                Vector2 velocityDirection = Vector2.Normalize(position - emitterLocation);
                position += velocityDirection;
            }
        }

        /// <summary>
        /// Draws the particle on the screen if the opacity of that particle is greater than zero.
        /// Decrements the scale and opacity
        /// </summary>
        /// <param name="particleSprite"></param>
        public void Draw(SpriteBatch particleSprite)
        {
            const float lifetime = 51f;
            if (currentOpacity.A > 0)
            {
                particleSprite.Draw(particleTexture, position, 
                    null, currentOpacity, 0f, Vector2.Zero, imageScale, SpriteEffects.None, 0);
                imageScale -= (1.0f / lifetime);
            }
            currentOpacity.A -= 5;
        }
        #endregion
    }
}
