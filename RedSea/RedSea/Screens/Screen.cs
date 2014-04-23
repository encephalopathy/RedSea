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
    #region Screen Enums

    public enum ScreenState
    {
        TransitionOn,
        Active,
        TransitionOff,
        Hidden,
    }

    #endregion

    public abstract class Screen
    {
        #region Variable Get and Set

        // Current 
        public TimeSpan TransitionOnTime
        {
            get { return transitionOnTime; }
            protected set { transitionOnTime = value; }
        }

        public TimeSpan TransitionOffTime
        {
            get { return transitionOffTime; }
            protected set { transitionOffTime = value; }
        }

        // Gets current 
        public float TransitionPosition
        {
            get { return transitionPosition; }
            protected set { transitionPosition = value; }
        }

        // Gets transition alpha, between 1 and 0
        public float TransitionAlpha
        {
            get { return 1f - TransitionPosition; }
        }


        // Get current screenstates
        public ScreenState ScreenState
        {
            get { return screenState; }
            protected set { screenState = value; }
        }

        // Checks if exiting
        public bool IsExiting
        {
            get { return isExiting; }
            protected internal set { isExiting = value; }
        }

        // Checks if Active
        public bool IsActive
        {
            get
            {
                return !otherScreenHasFocus &&
                       (screenState == ScreenState.TransitionOn ||
                        screenState == ScreenState.Active);
            }
        }

        #endregion

        #region Screen Variables

        // Get the manager that this screen belongs to.
        public ScreenManager Manager;

        // Initializaiton

        TimeSpan transitionOffTime = TimeSpan.Zero;
        TimeSpan transitionOnTime = TimeSpan.Zero;
        float transitionPosition = 1;
        ScreenState screenState = ScreenState.TransitionOn;
        bool otherScreenHasFocus;
        bool isExiting = false;

        #endregion

        #region Virtual Load and Unload 

        public virtual void LoadContent() { }
        public virtual void UnloadContent() { }

        #endregion

        #region Virtual Update

        //
        // Screen update Function
        //
        public virtual void Update(GameTime gameTime, bool otherScreenHasFocus,
                                                     bool coveredByOtherScreen)
        {
            this.otherScreenHasFocus = otherScreenHasFocus;

            if (isExiting)
            {
                // If the screen is going away to die, it should transition off.
                screenState = ScreenState.TransitionOff;

                if (!UpdateTransition(gameTime, transitionOffTime, 1))
                {
                    // When the transition finishes, remove the screen.
                    Manager.RemoveScreen(this);
                }
            }
            else if (coveredByOtherScreen)
            {
                // If the screen is covered by another, it should transition off.
                if (UpdateTransition(gameTime, transitionOffTime, 1))
                {
                    // Still busy transitioning.
                    screenState = ScreenState.TransitionOff;
                }
                else
                {
                    // Transition finished!
                    screenState = ScreenState.Hidden;
                }
            }
            else
            {
                // Otherwise the screen should transition on and become active.
                if (UpdateTransition(gameTime, transitionOnTime, -1))
                {
                    // Still busy transitioning.
                    screenState = ScreenState.TransitionOn;
                }
                else
                {
                    // Transition finished!
                    screenState = ScreenState.Active;
                }
            }
        }

        #endregion

        #region Virtual Draw

        // virtual draw function
        public virtual void Draw(GameTime gameTime) { }

        #endregion

        #region Functions

        // Exits current screen by switching isExiting to true
        public void ExitScreen()
        {
            if (TransitionOffTime == TimeSpan.Zero)
            {
                // If the screen has a zero transition time, remove it immediately.
                Manager.RemoveScreen(this);
            }
            else
            {
                // Otherwise flag that it should transition off and then exit.
                isExiting = true;
            }
        }

        // Helping function for Update: Used for transitioning
        bool UpdateTransition(GameTime gameTime, TimeSpan time, int direction)
        {
            // How much should we move by?
            float transition;

            if (time == TimeSpan.Zero)
                transition = 1;
            else
                transition = (float)(gameTime.ElapsedGameTime.TotalMilliseconds /
                                          time.TotalMilliseconds);

            // Update the transition position.
            transitionPosition += transition * direction;

            // Did we reach the end of the transition?
            if (((direction < 0) && (transitionPosition <= 0)) ||
                ((direction > 0) && (transitionPosition >= 1)))
            {
                transitionPosition = MathHelper.Clamp(transitionPosition, 0, 1);
                return false;
            }

            // Otherwise we are still busy transitioning.
            return true;
        }

        #endregion
    }
}
