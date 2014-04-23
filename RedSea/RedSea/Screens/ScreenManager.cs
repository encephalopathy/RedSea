using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

// Used parts of XNA Menu Sample from the App Hub Website
//

namespace RedSeaGame
{
    public class ScreenManager : DrawableGameComponent
    {
        #region Fields
        List<Screen> screens = new List<Screen>();
        List<Screen> screensToUpdate = new List<Screen>();

        public KeyboardState oldKey;
        public KeyboardState newKey;
        public SpriteBatch spriteBatch;
        Texture2D blankTexture;
        Game redSea;

        bool isInitialized;

        #endregion

        #region Shared Game Utilities

        // Constructor for ScreenManager
        public ScreenManager(Game game) :
            base(game)
        {
            redSea = game;
        }

        // Spritebatch get
        public SpriteBatch SpriteBatch
        {
            get { return spriteBatch; }
        }

        // Keyboard old state get
        public KeyboardState OldKey
        {
            get { return oldKey; }
        }

        // Keyboard new state get
        public KeyboardState NewKey
        {
            get { return newKey; }
        }

        // Game get
        public Game RedSea
        {
            get { return redSea; }
        }

        // Get method for gathering array of screens
        public Screen[] GetScreens()
        {
            return screens.ToArray();
        }

        // Initialization override
        public override void Initialize()
        {
            base.Initialize();

            isInitialized = true;
        }

        #endregion

        #region Load Content

        // LoadContent override
        protected override void LoadContent()
        {
            // Load content belonging to the screen manager.
            ContentManager content = Game.Content;

            spriteBatch = new SpriteBatch(GraphicsDevice);
            blankTexture = content.Load<Texture2D>("Screen_Textures/blank");

            foreach (Screen screen in screens)
            {
                screen.LoadContent();
            }
        }

        #endregion

        #region Unload Content

        // Unload content override
        protected override void UnloadContent()
        {
            // Tell each of the screens to unload their content.
            foreach (Screen screen in screens)
            {
                screen.UnloadContent();
            }
        }

        #endregion

        #region Update Screens

        // Override the Update
        public override void Update(GameTime gameTime)
        {
            // Make a copy of the master screen list, to avoid confusion if
            // the process of updating one screen adds or removes others.
            screensToUpdate.Clear();

            // Gather keyboard states
            oldKey = newKey;
            newKey = Keyboard.GetState();

            // Create list of screens to update
            foreach (Screen screen in screens)
                screensToUpdate.Add(screen);

            bool otherScreenHasFocus = !Game.IsActive;
            bool coveredByOtherScreen = false;

            // Loop as long as there are screens waiting to be updated.
            while (screensToUpdate.Count > 0)
            {
                // Pop the topmost screen off the waiting list.
                Screen screen = screensToUpdate[screensToUpdate.Count - 1];

                screensToUpdate.RemoveAt(screensToUpdate.Count - 1);

                // Update the screen.
                screen.Update(gameTime, otherScreenHasFocus, coveredByOtherScreen);


                if (screen.ScreenState == ScreenState.TransitionOn ||
                    screen.ScreenState == ScreenState.Active)
                {
                    // If this is the first active screen we came across,
                    // give it a chance to handle input.
                    if (!otherScreenHasFocus)
                        otherScreenHasFocus = true;
                }
            }
        }

        #endregion

        #region Draw

        public override void Draw(GameTime gameTime)
        {
            foreach (Screen screen in screens)
            {
                if (screen.ScreenState == ScreenState.Hidden)
                    continue;

                screen.Draw(gameTime);
            }
        }

        #endregion

        #region Functions

        // Add a screen to the list of screens
        public void AddScreen(Screen screen)
        {
            screen.Manager = this;
            screen.IsExiting = false;

            if (isInitialized)
            {
                screen.LoadContent();
            }
            screens.Add(screen);
        }



        public void RemoveScreen(Screen screen)
        {
            if (isInitialized)
            {
                screen.UnloadContent();
            }
            screens.Remove(screen);
        }




        // Fades the background to black when screen is behind a popup.
        public void FadeToBlack(float alpha)
        {
            Viewport viewport = GraphicsDevice.Viewport;

            spriteBatch.Begin();

            spriteBatch.Draw(blankTexture,
                             new Rectangle(0, 0, viewport.Width, viewport.Height),
                             Color.Black * alpha);

            spriteBatch.End();
        }

        // Fade white for end credits
        public void WhiteFade(float alpha)
        {
            Viewport viewport = GraphicsDevice.Viewport;

            spriteBatch.Begin();

            spriteBatch.Draw(blankTexture,
                             new Rectangle(0, 0, viewport.Width, viewport.Height),
                             Color.White * alpha);

            spriteBatch.End();
        }

        #endregion
    }
}