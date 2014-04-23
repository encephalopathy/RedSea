using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;


namespace RedSeaGame
{
    class MainMenu : Screen
    {
        #region Private Fields

        ContentManager content;     
        private Texture2D playGame;         // The play game button selected
        private Texture2D exit;             // The exit button selected
        private Texture2D playGameRed;      // The play game button
        private Texture2D exitRed;          // The exit game button
        private Texture2D redSeaText;       // The game title
        private Texture2D blank;            // Black texture
        private Song song;                  // The main menu song
        private float menuAlpha;           // Alpha for 
        private bool songstart;
        private bool playSelect = true;

        #endregion

        #region Contructor

        public MainMenu()
        {
            TransitionOnTime = TimeSpan.FromSeconds(6);
            TransitionOffTime = TimeSpan.FromSeconds(0.5);
        }

        #endregion

        #region Load Content

        public override void LoadContent()
        {
            if (content == null)
                content = new ContentManager(Manager.Game.Services, "Content");
            // Load all textures for the main menu
            blank = content.Load<Texture2D>("Screen_Textures/blank");
            exit = content.Load<Texture2D>("Screen_Textures/Menu/exit");
            exitRed = content.Load<Texture2D>("Screen_Textures/Menu/exitRed");
            playGame = content.Load<Texture2D>("Screen_Textures/Menu/playGame");
            playGameRed = content.Load<Texture2D>("Screen_Textures/Menu/playGameRed");
            redSeaText = content.Load<Texture2D>("Screen_Textures/titlepage");
            song = content.Load<Song>("Music/MainMenu");
        }

        #endregion

        #region Update

        public override void Update(GameTime gameTime, bool otherScreenHasFocus, bool coveredByOtherScreen)
        {
            base.Update(gameTime, otherScreenHasFocus, false);
            if (IsActive)
            {
                // Play song here
                if (!songstart)
                {
                    MediaPlayer.Play(song);
                    songstart = true;
                }

                // Keyboard input
                KeyboardState oldKey = Manager.OldKey;
                KeyboardState newKey = Manager.NewKey;


                menuAlpha = Math.Max(menuAlpha - 1f / 32, 0);

                // Pause screen when you press escape
                if (oldKey.IsKeyDown(Keys.Escape) && newKey.IsKeyUp(Keys.Escape))
                {
                    Manager.AddScreen(new Pause());
                }

                // Selecting menu option
                if (oldKey.IsKeyDown(Keys.Enter) && newKey.IsKeyUp(Keys.Enter))
                {
                    // Enter into the first text screen
                    if (playSelect)
                    {
                        ExitScreen();
                        //Loading.Load(new Level(5, false), true, Manager);
                        Manager.AddScreen(new Text(1));
                    }
                    // Exit game
                    else
                    {
                        Manager.RedSea.Exit();
                    }
                }

                // Change between current item highlighted
                if ((oldKey.IsKeyDown(Keys.Up) && newKey.IsKeyUp(Keys.Up)) ||
                    (oldKey.IsKeyDown(Keys.Down) && newKey.IsKeyUp(Keys.Down)))
                {
                    if (playSelect)
                        playSelect = false;
                    else
                        playSelect = true;
                }
            }
        }

        #endregion

        #region Draw

        public override void Draw(GameTime gameTime)
        {

            SpriteBatch spriteBatch = Manager.SpriteBatch;
            Viewport viewport = Manager.GraphicsDevice.Viewport;

            spriteBatch.Begin();

            // Draw background
            spriteBatch.Draw(blank,
                             new Rectangle(0, 0, viewport.Width, viewport.Height),
                             Color.Black);

            // Draw the title
            spriteBatch.Draw(redSeaText, new Vector2(0, 0), Color.White);

            // Draws the currently selected button
            if (playSelect)
            {
                spriteBatch.Draw(playGame, new Vector2(450, 300), Color.White);
                spriteBatch.Draw(exitRed, new Vector2(500, 350), Color.White);
            }
            else
            {
                spriteBatch.Draw(playGameRed, new Vector2(450, 300), Color.White);
                spriteBatch.Draw(exit, new Vector2(500, 350), Color.White);
            }

            spriteBatch.End();

            // If the game is transitioning on or off, fade it out to black.
            if (TransitionPosition > 0 || menuAlpha > 0)
            {
                float alpha = MathHelper.Lerp(1f - TransitionAlpha, 1f, menuAlpha / 2);

                Manager.FadeToBlack(alpha);
            }
        }
    }

        #endregion
}
