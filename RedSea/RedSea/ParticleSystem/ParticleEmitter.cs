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
    class ParticleEmitter
    {
        #region Fields
        //Max life for an explosive particle
        const int MAX_LIFE = 51;

        //The length of time for which a particle is animated before it disappears
        int lifeTime;

        //Emitter location is the location where the particles are emitted on the screen
        Vector2 emitterLocation;

        //The distance from the emitter location for which particles are created on the screen
        float creatationRadius;

        //The number of particles created
        int particleCount;

        //The texture of the particle
        Texture2D particleTexture;

        //A bool that is used to tell the cell to either keep or discard this particle emitter
        bool doneEmitting;

        //List of particles that the particle emitter will emit
        List<Particle> particles = new List<Particle>();
        
        #endregion

        #region CONSTRUCTOR
        /// <summary>
        /// Sets the emitter location, particle count, texture, and creation radius for the particle emitter.
        /// </summary>
        /// <param name="location">location of the particle emitter</param>
        /// <param name="texture">texture the particle emitter will store</param>
        /// <param name="radius">the radius in which the particle emitter can create particles</param>
        /// <param name="particleNumber">the number of particles created by the particle emitter</param>
        public ParticleEmitter(Vector2 location, Texture2D texture, float radius, int particleNumber)
        {
            emitterLocation = location;
            creatationRadius = radius;
            particleCount = particleNumber;
            particleTexture = texture;
            doneEmitting = false;
            lifeTime = MAX_LIFE;
            createParticles();
        }
        #endregion

        #region METHODS

        /// <summary>
        ///Creates the amount of particles specified by the particle Count and sets each new particle's location
        ///somewhere within the creation radius of the screen.  The coordinates of each particle is randomized
        ///to give each a explosion a unique particle effect.
        /// </summary>
        private void createParticles()
        {
            for (int i = 0; i < particleCount; i++)
            {
                float x = ParticleHelper.RandomBetween(emitterLocation.X - creatationRadius, emitterLocation.X + creatationRadius);
                float y = ParticleHelper.RandomBetween(emitterLocation.Y - creatationRadius, emitterLocation.Y + creatationRadius);
                particles.Add(new Particle(x, y, particleTexture, emitterLocation));
            }
        }

        /// <summary>
        /// Updates the emitter location if the particles contained within the particle emitter are done
        /// animating.  This fucntion is also responsible for animating each particle in the particles
        /// List during each game tick until the particle emitter is done emitting.
        /// </summary>
        /// <param name="centerLocation"></param>
        public void Update(Vector2 centerLocation)
        {
            if (lifeTime != 0)
            {
                foreach (Particle bubble in particles)
                {
                    bubble.Update(centerLocation);
                    if (bubble.AnimationDone())
                    {
                        bubble.Create();
                    }
                }

                emitterLocation = centerLocation;
            }

            if (lifeTime > 0)
            {
                lifeTime--;
            }
            else if (lifeTime == 0)
            {
                doneEmitting = true;
            }
        }
        
        /// <summary>
        /// Is used to determine whether the particle emitter is done emitting
        /// </summary>
        /// <returns>Is the particle emitter done emitting?</returns>
        public bool Finish()
        {
            return doneEmitting;
        }

        /// <summary>
        /// The Draw method is used to draw each sprite animation
        /// </summary>
        /// <param name="particleSprite"></param>
        public void Draw(SpriteBatch particleSprite)
        {
            foreach (Particle bubble in particles)
            {
                if (bubble.AnimationDone())
                    continue;
                bubble.Draw(particleSprite);
            }
        }
        #endregion
    }
}
