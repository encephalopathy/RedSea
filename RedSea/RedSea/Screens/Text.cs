using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

namespace RedSeaGame
{

    class Text : Screen
    {
        #region Private Fields

        ContentManager content;
        private Texture2D TextTexture;  // The text textures
        private Texture2D blank;        // Blank textures
        private Song songText;          // The song played for the text screen
        private float textAlpha;        // The alpha 
        private int text;               // Number of which text to display
        private bool songstart;         // Check whether or not the song has started
        private int textmax = 6;        // text maximum (check for showing the credits)        

        #endregion

        #region Constructor

        public Text(int screenNum)
        {
            TransitionOnTime = TimeSpan.FromSeconds(6);
            TransitionOffTime = TimeSpan.FromSeconds(0.5);
            text = screenNum;
        }

        #endregion

        #region Load Content

        public override void LoadContent()
        {
            if (content == null)
                content = new ContentManager(Manager.Game.Services, "Content");
            blank = content.Load<Texture2D>("Screen_Textures/blank");

            // Determines which text and song to use according to the current level count
            switch (text)
            {
                // Each case is the level number for which text to use
                case 1:
                    TextTexture = content.Load<Texture2D>("Screen_Textures/story1");
                    break;
                case 2:
                    TextTexture = content.Load<Texture2D>("Screen_Textures/story2");
                    songText = content.Load<Song>("Music/TextSong");
                    break;
                case 3:
                    TextTexture = content.Load<Texture2D>("Screen_Textures/story3");
                    songText = content.Load<Song>("Music/TextSong");
                    break;
                case 4:
                    TextTexture = content.Load<Texture2D>("Screen_Textures/story4");
                    songText = content.Load<Song>("Music/TextSound2");
                    break;
                case 5:
                    TextTexture = content.Load<Texture2D>("Screen_Textures/story5");
                    break;
                case 6:
                    TextTexture = content.Load<Texture2D>("Screen_Textures/story5");
                    songText = content.Load<Song>("Music/EndCredits");
                    break;
                case 7:
                    TextTexture = content.Load<Texture2D>("Screen_Textures/credits");
                    break;
                default:
                    TextTexture = content.Load<Texture2D>("Screen_Textures/hellaTest");
                    songText = content.Load<Song>("Music/TextSong");
                    break;
            }
        }

        #endregion

        #region Update

        public override void Update(GameTime gameTime, bool otherScreenHasFocus, bool coveredByOtherScreen)
        {
            base.Update(gameTime, otherScreenHasFocus, false);
            if (IsActive)
            {
                // Setting the music
                if (!songstart)
                {
                    if (songText != null)
                    {
                        MediaPlayer.Play(songText);
                        MediaPlayer.IsRepeating = true;
                    }
                    songstart = true;
                }

                // Lets level 5 load right after level 4
                // No story slide inbetween these two
                if (text == 5)
                {
                    MediaPlayer.Stop();
                    Loading.Load(new Level(text, false), true, Manager);
                }

                // Keyboard stuff
                KeyboardState oldKey = Manager.OldKey;
                KeyboardState newKey = Manager.NewKey;

                textAlpha = Math.Max(textAlpha - 1f / 32, 0);

                // Pause when Escape is pressed
                if (oldKey.IsKeyDown(Keys.Escape) && newKey.IsKeyUp(Keys.Escape))
                {
                    Manager.AddScreen(new Pause());
                }

                // Change screen if 
                if (oldKey.IsKeyDown(Keys.Enter) && newKey.IsKeyUp(Keys.Enter))
                {
                    if(text != textmax)
                        MediaPlayer.Stop();
                    ExitScreen();

                    //Console.WriteLine("Text:" + text);

                    // Load next level
                    if (text < textmax)
                        Loading.Load(new Level(text, false), true, Manager);
                    // If there are no more levels
                    // Load credits screen
                    else if (text == textmax)
                    {
                        Manager.AddScreen(new Text(7));
                    }
                    // Go back to main menu
                    else
                    {
                        Manager.AddScreen(new MainMenu());
                    }
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

            // A blank screen for background
            spriteBatch.Draw(blank,
                             new Rectangle(0, 0, viewport.Width, viewport.Height),
                             Color.Black);

            spriteBatch.Draw(TextTexture, new Vector2(0, 0), Color.White);

            spriteBatch.End();

            // If the game is transitioning on or off, fade it out to black.
            if (TransitionPosition > 0 || textAlpha > 0)
            {
                float alpha = MathHelper.Lerp(1f - TransitionAlpha, 1f, textAlpha / 2);

                // Transition Fade white if it's the last level, or fade black otherwise.
                if (text > 5)
                    Manager.WhiteFade(alpha);
                else
                    Manager.FadeToBlack(alpha);
            }
        }
    }
        #endregion
}