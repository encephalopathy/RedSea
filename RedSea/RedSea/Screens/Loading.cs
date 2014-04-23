using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

//
// Used help from XNA Menu Sample from the App Hub Website
//

namespace RedSeaGame
{
    class Loading : Screen
    {
        #region Fields

        ContentManager content;
        Screen toLoad;
        private bool loading;
        private bool transComplete;
        Texture2D loadScreen;

        #endregion

        #region Constructor

        Loading(ScreenManager screenManager, bool loading, Screen toLoad)
        {
            this.toLoad = toLoad;
            this.loading = loading;

            TransitionOnTime = TimeSpan.FromSeconds(0.5);
            TransitionOffTime = TimeSpan.FromSeconds(0.5);
        }

        #endregion

        #region Static Load Function

        // Static loading method: This will load whatever level we want
        // to load, displaying a loading text as it loads.
        public static void Load(Screen screen, bool loadingLevel,
            ScreenManager manager)
        {
            // Unload all of the other screens: empty the list
            foreach (Screen oldScreen in manager.GetScreens())
            {
                oldScreen.ExitScreen();
            }

            Loading loadingScreen = new Loading(manager, loadingLevel, screen);


            // Add the screen to the screen manager
            manager.AddScreen(loadingScreen);
        }

        #endregion

        #region Load Content

        public override void LoadContent()
        {
            if (content == null)
                content = new ContentManager(Manager.Game.Services, "Content");

            // Load the loading screen
            loadScreen = content.Load<Texture2D>("Screen_Textures/loading");
        }

        #endregion

        #region Update

        public override void Update(GameTime gameTime, bool otherScreenHasFocus,
                                               bool coveredByOtherScreen)
        {
            base.Update(gameTime, otherScreenHasFocus, coveredByOtherScreen);

            // If all the previous screens have finished transitioning
            // off, it is time to actually perform the load.
            if (transComplete)
            {
                Manager.RemoveScreen(this);
                if (toLoad != null)
                    Manager.AddScreen(toLoad);

                // Reset time so it doesn't try to draw them again.
                Manager.RedSea.ResetElapsedTime();
            }
        }

        #endregion

        #region Draw

        public override void Draw(GameTime gameTime)
        {
            if ((ScreenState == ScreenState.Active) &&
                (Manager.GetScreens().Length == 1))
            {
                transComplete = true;
            }

            SpriteBatch spriteBatch = Manager.SpriteBatch;

            spriteBatch.Begin();
            spriteBatch.Draw(loadScreen, new Vector2(0, 0), Color.White);
            spriteBatch.End();

        }
        #endregion
    }
}
