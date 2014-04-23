using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using FarseerPhysics.Dynamics;
using FarseerPhysics.Factories;
using FarseerPhysics.Dynamics.Contacts;

namespace RedSeaGame
{
    class WhiteBloodCell : Enemy
    {
        #region Fields

        //Bounciness of the white blood cell
        private const float whiteBloodCellRest = 2.5f;
        
        //The absolute range of the force applied to the white blood cell when it is drifting
        private const int forceBound = 100000000;

        //The attack time for the white blood cell
        private const int MAX_ATTACK_TIME = 1000;

        //Determines how much the white blood cell spins
        private const float torque = 10000000f;

        //Detection range of the white blood cell
        private const int whiteBloodCellRange = 100;

        //Attack time for the white blood cell to persue the player
        private int attackTime = 0;

        //Grid so a white blood cell
        private Grid gameGrid;

        //Overrides attack 
        public bool attackFlag { get; set; }

        //Boolean to determine if the white blood cell has hit the player
        private bool hitPlayer = false;
        
        #endregion

        #region CONSTRUCTOR
        /// <summary>
        /// Sets the location of the white blood cell on the level as well
        /// as make the cell drift.  The grid passed through the parameter list of white
        /// blood cell is used for pathfinding.
        /// </summary>
        /// <param name="center"></param>
        /// <param name="gameWorld"></param>
        /// <param name="gameGrid"></param>
        /// <param name="playerRef"></param>
        public WhiteBloodCell(Vector2 center, ref World gameWorld, ref Grid gameGrid, ref Player playerRef) 
            : base(center, ContentStore.WhiteBloodCell.Width, ref gameWorld, ref playerRef, torque, forceBound)
        {
            SetRestitution(whiteBloodCellRest);
            this.gameGrid = gameGrid;
            texture = ContentStore.WhiteBloodCell;
            player = playerRef;
            attackFlag = playerDetected;
            range = whiteBloodCellRange;
        }
        #endregion

        #region METHODS

        /// <summary>
        /// Getter method used to determine whether this white blood cell has hit the player.
        /// </summary>
        public bool HitPlayer { get { return hitPlayer; } }

        //sets the springiness for your cell
        protected override void SetRestitution(float springiness)
        {
            base.SetRestitution(springiness);
        }

        //Moves the white blood cell closer to the player every game tick.  Or drifts it away.
        private void Move()
        {
            if (CellBehavior is Attack && attackTime > 0)
            {
                //Console.WriteLine("Attack time is: " + attackTime);;
                ((Attack)CellBehavior).Update();
                attackTime--;
            }
        }

        //If the cell is not attacking, then it is drifting
        private void Drift()
        {
            cellBody.BodyType = BodyType.Dynamic;
            cellBehavior = new Drift(ref cellBody, torque, forceBound);
        }

        //Uses a global attack behavior to determine whether the white blood cell should attack the player or not
        private void Attack()
        {
          cellBehavior = new Attack(this, ref cellBody, ref gameGrid, range);
          attackTime = MAX_ATTACK_TIME;
        }

        /// <summary>
        /// Changes the behavior if the white blood cell to either attack or drift
        /// based on if the was player detected or attack time is zero.
        /// </summary>
        private void ChangeBehavior()
        {
            
            if ((playerDetected || attackFlag) && cellBehavior is Drift)
            {
                Attack();
            }
            else if (attackTime == 0 && cellBehavior is Attack)
            {
                Drift();
            }
            playerDetected = false;
            attackFlag = false;
        }

        /// <summary>
        /// Updates the player location behavior and determines if the player has detected the player.
        /// Also updates the player position on the screen.
        /// </summary>
        /// <param name="gameTime"></param>

        public override void Update(GameTime gameTime)
        {
            detect(player.Center);
            ChangeBehavior();
            Move();
            base.Update(gameTime);
        }

        /// <summary>
        /// Checks to see if the white blood cell has collided with the player.  If it does, the hitPlayer flag is set
        /// </summary>
        public override void cellOnCollision(ref FixtureProxy thisCellFixture, ref FixtureProxy otherCellFixture)
        {
            if (hasCollided(ref thisCellFixture, ref otherCellFixture) && (thisCellFixture.Fixture == player.CellFixture ||
                otherCellFixture.Fixture == player.CellFixture))
            {
                hitPlayer = true;
            }
            base.cellOnCollision(ref thisCellFixture, ref otherCellFixture);
        }

        //Draws the sprite on the screen
        public override void Draw(SpriteBatch cellSprite)
        {
            base.Draw(cellSprite);
        }
        #endregion
    }
}
