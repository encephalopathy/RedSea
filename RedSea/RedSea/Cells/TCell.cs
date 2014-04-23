using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna;
using Microsoft.Xna.Framework.Graphics;
using FarseerPhysics.Dynamics;
using FarseerPhysics.Factories;

namespace RedSeaGame
{
    class TCell : Enemy
    {
        #region Initialization and Constructor
        /*Good numbers for initialization:
         * Force: 4 000 000
         * Torque: 10 000 000
         * restitution: .7
         */

        public bool alert { get; private set; }
        // Sets the torque on the T Cell
        protected const float tq = 100000000f;
        // Sets the maximum force on the T Cell
        protected const int fb = 100000;
        // Sets the restitution on the T Cell
        protected const float rest = .1f;
        // Sets the range for the spotting view
        protected const float viewRange = 150f;
        // Sets the mass for the TCell
        protected const int mass = 5000;

        // T Cell constructer
        public TCell(Vector2 location, Texture2D content, ref World world, ref Player player)
            : base(location, ContentStore.TCell.Width, ref world, ref player, tq, fb)
        {
            cellBody.Mass = mass;
            range = viewRange;
            SetRestitution(rest);
            texture = ContentStore.TCell;
        }
        #endregion

        #region Update and Draw
        //checks for the alert and passes the detected alert bool for the white blood cells to use.
        public override void Update(GameTime gameTime)
        {
            detect(player.Center);
            alert = playerDetected; 
            base.Update(gameTime);
        }

        //Draws the TCell and draws the circle around the TCell
        public override void Draw(SpriteBatch cellSprite)
        {
            if (alert)
            cellSprite.Draw(ContentStore.TCellAlerted, new Vector2(center.X - ContentStore.TCellAlerted.Width / 2,
                center.Y - ContentStore.TCellAlerted.Height/2), Color.White);
            else
            cellSprite.Draw(ContentStore.TCellNoAlert, new Vector2(center.X - ContentStore.TCellNoAlert.Width / 2,
                center.Y - ContentStore.TCellNoAlert.Height / 2), Color.White);
            base.Draw(cellSprite);
        }
        #endregion
    }
}
