using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Audio;
using FarseerPhysics.Dynamics;
using FarseerPhysics.Factories;


namespace RedSeaGame
{
    class Enemy : Cell
    {
        #region Fields and Construtor
        // movement behavior of cell
        protected Behavior cellBehavior;
        // bool for if the player is detected
        protected bool playerDetected;
        // the range of the detection range
        protected float range = 0;
        // the player
        protected Player player;
        public Behavior CellBehavior { get { return cellBehavior; } }


        //Constructor used for all enemies
        public Enemy(Vector2 location, int diameter, ref World world, ref Player player, float torque, int forceBound) : 
        base(location, diameter, ref world) {
            cellBody.BodyType = BodyType.Dynamic;
            this.player = player;
            cellBehavior = new Drift(ref cellBody, torque, forceBound);
        }
        #endregion

        #region Methods
        //Figures out the distance between the T Cell and the Player and tells if 
        //the cell is within the detection range of the T Cell
        public virtual void detect(Vector2 playerLocation)
        {
            float dist = Vector2.Distance(center, playerLocation);
            if (dist <= range)
            {
                playerDetected = true;
            }
            else
                playerDetected = false;
        }

        //Determines how much a cell bounces
        //Nice number = 2.5
        protected virtual void SetRestitution(float springiness)
        {
            CellFixture.Restitution = springiness;
        }

        // Loop alarm sound effect when player is detected
        protected void loopAlarmSound(bool detected)
        {
            SoundEffectInstance alarmSound = ContentStore.Alarm.CreateInstance();
            
            alarmSound.IsLooped = true;

            alarmSound.Play();

        }
        #endregion

        #region Update
        public override void Update(GameTime gameTime)
        {

            if (cellBehavior is Drift)
            {
                ((Drift)CellBehavior).Update();
            }
            base.Update(gameTime);
        }
        #endregion
    }
}
