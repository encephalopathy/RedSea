using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace RedSeaGame
{

    class Pause : Screen
    {
        ContentManager content;
        Texture2D PauseScr, AreYouSureScr;
        float pauseAlpha;
        bool quitMessage;

        public Pause()
        {
            TransitionOnTime = TimeSpan.FromSeconds(0.5);
            TransitionOffTime = TimeSpan.FromSeconds(0.5);
            quitMessage = false;
        }

        public override void LoadContent()
        {
            if (content == null)
                content = new ContentManager(Manager.Game.Services, "Content");

            PauseScr = content.Load<Texture2D>("Screen_Textures/pause");
            AreYouSureScr = content.Load<Texture2D>("Screen_Textures/areYouSure");
        }

        public override void Update(GameTime gameTime, bool otherScreenHasFocus, bool coveredByOtherScreen)
        {
            KeyboardState oldKey = Manager.OldKey;
            KeyboardState newKey = Manager.NewKey;
            base.Update(gameTime, otherScreenHasFocus, false);

            // Gradually fade in or out depending on whether we are covered by the pause screen.
            if (coveredByOtherScreen)
                pauseAlpha = Math.Min(pauseAlpha + 1f / 32, 1);
            else
                pauseAlpha = Math.Max(pauseAlpha - 1f / 32, 0);


            if ((oldKey.IsKeyDown(Keys.Escape) && newKey.IsKeyUp(Keys.Escape)) ||
                (oldKey.IsKeyDown(Keys.N) && newKey.IsKeyUp(Keys.N) && quitMessage))
            {
                ExitScreen();
            }
            if (oldKey.IsKeyDown(Keys.Q) && newKey.IsKeyUp(Keys.Q))
            {
                quitMessage = true;
            }
            if (oldKey.IsKeyDown(Keys.Y) && newKey.IsKeyUp(Keys.Y) && quitMessage)
            {
                Manager.RedSea.Exit();
            }

        }

        /// <summary>
        /// Draws the gameplay screen.
        /// </summary>
        public override void Draw(GameTime gameTime)
        {
            // This game has a blue background. Why? Because!
            Manager.GraphicsDevice.Clear(ClearOptions.Target,
                                               Color.Black, 0, 0);

            // Our player and enemy are both actually just text strings.
            SpriteBatch spriteBatch = Manager.SpriteBatch;

            // Darken down any other screens that were drawn beneath the popup.
            Manager.FadeToBlack(TransitionAlpha * 2 / 3);

            Color color = Color.White * TransitionAlpha;

            spriteBatch.Begin();
            if (quitMessage)
                spriteBatch.Draw(AreYouSureScr, new Vector2(0, 0), color);
            else
                spriteBatch.Draw(PauseScr, new Vector2(0, 0), color);

            spriteBatch.End();

            // If the game is transitioning on or off, fade it out to black.
            if (TransitionPosition > 0 || pauseAlpha > 0)
            {
                float alpha = MathHelper.Lerp(1f - TransitionAlpha, 1f, pauseAlpha / 2);

                Manager.FadeToBlack(alpha);
            }
        }
    }
}