using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using Microsoft.Xna.Framework.Audio;
using FarseerPhysics.Dynamics;

namespace RedSeaGame
{
    class Level : Screen
    {
        #region Private Fields

        private GraphicsDeviceManager graphics;         //
        private ContentManager content;                 // used to load content for the level
        private SpriteBatch spriteBatch;                // used to draw sprites for the level
        private float levelAlpha;                       // The alpha of the level for transitions

        private int level;                              // the level number
        private int levelMax = 6;                       // number of levels in the game
        private Parser parser;                          // parser object that loads level info
        private World world;                            // world object used for collision
        private Wall levelWall;                         // the wall surrounding the level
        private Grid grid;                              // grid object that stores path finding info
        private Point gridCellSize;                     // the size of each gridcell in the grid
        private ContentStore contentStore;              // object used to load and store textures and sounds
        private Camera2D camera;                        // camera that follows the player
        private Player player;                          // object that the player controls
        private List<Enemy> enemies;                    // list of all the enemies in the level
        private int reqReds;                            // required # red blood cells for winning

        private Texture2D winStatement, loseStatement;
        private bool songStart = false;                 // bool for starting the song
        private bool isWin = false;                     // check if player has infected all cells
        private bool isLose = false;                    // check if player loses
        private Song levelSong;                         // Song for the level
        private List<RedBloodCell> bloodCells;          // List of red blood cells to check if they're infectedd

        private bool deathNoise = false;                // Make it so the noise isn't repeated
        private bool completeNoise = false;             // 

        #endregion

        #region Contructor

        public Level(int levelNumber, bool loopAfterLose)
        {
            // Screen loading
            TransitionOnTime = TimeSpan.FromSeconds(6);
            TransitionOffTime = TimeSpan.FromSeconds(6);

            level = levelNumber;
            songStart = loopAfterLose;
        }

        #endregion

        #region Load Level

        #region Main Load Method

        public override void LoadContent()
        {
            // create a SpriteBatch object to be used for drawing sprites in this level
            SpriteBatch spriteBatch = Manager.SpriteBatch;

            // if the ContentManager object has not been initialized, make a new one
            if (content == null)
                content = new ContentManager(Manager.RedSea.Services, "Content");

            // create new World object based at the origin
            world = new World(new Vector2(0, 0));

            // create new Parser object to be used to get information about the level
            parser = new Parser(getFilePath());

            // define the width and height for each grid cell in the grid
            gridCellSize = new Point(parser.LevelDimensions.X / parser.GridDimensions.X,
                                     parser.LevelDimensions.Y / parser.GridDimensions.Y);

            // create new Grid object for the level for the enemy cells' attack behavior
            grid = new Grid(parser.GridDimensions, parser.ReachableLocations, gridCellSize);

            // create new Wall object which defines the border of the entire level
            LoadWall();

            // create new ContentStore object which loads and stores textures and music for the level
            contentStore = new ContentStore(content);

            // create a new camera that will be used to follow the player
            camera = new Camera2D();

            // load the Player object
            LoadPlayer();

            // load all Enemy objects
            LoadEnemies();

            // load the song
            LoadSong();

            // Win/lose statement
            winStatement = content.Load<Texture2D>("Screen_Textures/youWin");
            loseStatement = content.Load<Texture2D>("Screen_Textures/youLose");
        }

        #endregion

        #region Load Helper Methods

        private string getFilePath()
        {
            // change path to content folder to load level data
            return content.RootDirectory + "/Levels/level" + level + ".txt";
        }

        private void LoadWall()
        {
            // initializing the walltextures list
            List<Texture2D> wallTextures = new List<Texture2D>();

            // load each wall fragment and add it to the list of wall fragments
            char div = 'a';
            for (int i = 0; i < parser.WallDivisions; i++)
            {
                string name = "level" + level + div;
                wallTextures.Add(content.Load<Texture2D>("Levels/" + name));
                div++;
            }

            // set the width and height of the level
            int width = parser.LevelDimensions.X;
            int height = parser.LevelDimensions.Y;

            // load the level background image
            Texture2D bgTex = content.Load<Texture2D>("levels/level" + level + "_bg");

            // create the wall object with the background and wall fragments
            levelWall = new Wall(width, height, wallTextures, bgTex, ref world);
        }

        private void LoadPlayer()
        {
            // create player at its initial location
            player = new Player(new Vector2(parser.PlayerInitialLocation.X * gridCellSize.X, 
                                            parser.PlayerInitialLocation.Y * gridCellSize.Y), ref world);
        }

        private void LoadEnemies()
        {
            // initialize the enemy lists
            enemies = new List<Enemy>();
            bloodCells = new List<RedBloodCell>();

            // save the number of red cells in the level (used for win condition)
            reqReds = parser.RedCellInitialLocations.Count;

            Point initLoc;
            Point stretch = gridCellSize;
            Point centerAdjust = new Point(gridCellSize.X / 2, gridCellSize.Y / 2);

            // create reds at their initial locations
            foreach (Point p in parser.RedCellInitialLocations)
            {
                initLoc = p;

                RedBloodCell newBloodCell = new RedBloodCell(new Vector2((initLoc.X * stretch.X) + centerAdjust.X, 
                                                                         (initLoc.Y * stretch.Y) + centerAdjust.Y),
                                                             ref world, ref player);
                // add to enemies list
                enemies.Add(newBloodCell);
                bloodCells.Add(newBloodCell);
            }

            // create whites at their initial locations
            foreach (Point p in parser.WhiteCellInitialLocations)
            {
                initLoc = p;

                enemies.Add(new WhiteBloodCell(new Vector2((initLoc.X * stretch.X) + centerAdjust.X, 
                                                           (initLoc.Y * stretch.Y) + centerAdjust.Y),
                                               ref world, ref grid, ref player));
            }

            // create Ts at their initial locations
            foreach (Point p in parser.TCellInitialLocations)
            {
                initLoc = p;

                enemies.Add(new TCell(new Vector2((initLoc.X * stretch.X) + centerAdjust.X, (initLoc.Y * stretch.Y) + centerAdjust.Y),
                                             ContentStore.TCell, ref world, ref player));
            }

            // creates antibodies at their initial locations
            foreach (Point p in parser.AntiBodyInitialLocations)
            {
                initLoc = p;

                enemies.Add(new Antibody(new Vector2((initLoc.X * stretch.X) + centerAdjust.X, 
                                                     (initLoc.Y * stretch.Y) + centerAdjust.Y),
                                         ref world, ref player));
            }
        }

        private void LoadSong()
        {
            if (songStart == false)
            {
                // load the song for the current level
                switch (level)
                {
                    case 1:
                        levelSong = content.Load<Song>("Music/FirstLevel");
                        break;
                    case 2:
                        levelSong = content.Load<Song>("Music/SecondSong");
                        break;
                    case 3:
                        levelSong = content.Load<Song>("Music/ThirdLevel");
                        break;
                    case 4:
                        levelSong = content.Load<Song>("Music/FifthLevel");
                        break;
                    case 5:
                        levelSong = content.Load<Song>("Music/FifthLevel");
                        break;
                }
            }
        }

        #endregion

        #endregion

        #region Unload Level

        public void UnLoadContent()
        {

        }

        #endregion

        #region Update

        public override void Update(GameTime gameTime, bool otherScreenHasFocus, bool coveredByOtherScreen)
        {
            base.Update(gameTime, otherScreenHasFocus, false);

            levelAlpha = Math.Max(levelAlpha - 1f / 32, 0);

            if (IsActive)
            {
                // Play song once in the beginning
                if (songStart == false)
                {
                    MediaPlayer.Play(levelSong);
                    MediaPlayer.IsRepeating = true;
                    songStart = true;
                }

                // store keyboard state for player input
                KeyboardState oldKey = Manager.OldKey;
                KeyboardState newKey = Manager.NewKey;
                
                // pause the game when player hits Esc
                if (oldKey.IsKeyDown(Keys.Escape) && newKey.IsKeyUp(Keys.Escape))
                {
                    Manager.AddScreen(new Pause());
                }

                // check for win/lose conditions
                CheckWin(oldKey, newKey);

                // while the player hasn't won or lost
                if (!isWin && !isLose)
                {
                    // update camera's position
                    player.Update(newKey);

                    // center camera over player
                    camera.Position = player.Center;

                    // store the player's location in the grid
                    grid.GetPlayerExactLocation = player.Center;

                    //Upadates what the cells do
                    updateCells(oldKey, newKey, gameTime);

                    // update all enemy locations in the grid
                    grid.UpdateCellLocations(player, enemies);

                    // update the world
                    world.Step((float)gameTime.ElapsedGameTime.TotalMilliseconds * 0.001f);
                }
            }
        }

        private void updateCells(KeyboardState oldKey, KeyboardState newKey, GameTime gameTime)
        {
            bool alert = false;
            // update each enemy cell
            foreach (Cell e in enemies)
            {
                // check for player trying to escape an antibody
                if (e is Antibody)
                {
                    ((Antibody)e).getOff(oldKey, newKey);
                    if (((Antibody)e).detected)
                    {
                        //Console.WriteLine("Alert is true");
                        alert = true;
                    }
                }

                if (e is TCell)
                {
                    if (((TCell)e).alert)
                    {
                        //Console.WriteLine("Alert is true");
                        alert = true;
                    }
                }

                
            }

            foreach (Enemy cell in enemies)
            {
                if (cell is WhiteBloodCell)
                {
                    //Console.WriteLine("Called here 1");
                    if (alert)
                    {
                        //Console.WriteLine("Called here 2");
                        ((WhiteBloodCell)cell).attackFlag = true;
                    }
                }

                // update the enemy cell
                cell.Update(gameTime);
            }
        }

        private void CheckWin(KeyboardState oldKey, KeyboardState newKey)
        {
            // Check if player has won
            //  once the infectCount reaches the required amount, player wins
            int infectCount = 0;
            if (!isWin || !isLose)
            {
                foreach (Enemy enemy in enemies)
                {
                    if (enemy is RedBloodCell)
                    {
                        if (((RedBloodCell)enemy).IsInfected)
                            infectCount++;
                    }
                }
                if (infectCount == reqReds)
                {
                    if (completeNoise == false)
                    {
                        ContentStore.LevelComplete.Play();
                        completeNoise = true;
                    }
                    isWin = true;
                }
            }

            // Check if lose
            //  if the player hits a white blood cell, player loses
            foreach (Enemy cell in enemies)
            {
                if (cell is WhiteBloodCell)
                {
                    if (((WhiteBloodCell)cell).HitPlayer)
                    {
                        if (deathNoise == false)
                        {
                            ContentStore.Death.Play();
                            deathNoise = true;
                        }
                        isLose = true;
                    }
                }
            }

            // Complete level automatically for debugging
            if (oldKey.IsKeyDown(Keys.F12) && newKey.IsKeyUp(Keys.F12))
            {
                isWin = true;
            }

            // When player wins or loses
            //  Win: load next level
            //  Lose: reload the level
            if (oldKey.IsKeyDown(Keys.Enter) && newKey.IsKeyUp(Keys.Enter))
            {
                if (isWin)
                {
                    ++level;
                    if (level > levelMax)
                    {
                        Console.WriteLine("level: " + level + " levelMax: " + levelMax);
                        Manager.AddScreen(new MainMenu());
                    }
                    else
                    {
                        ExitScreen();
                        Manager.AddScreen(new Text(level));
                    }
                }
                if (isLose)
                {
                    ExitScreen();
                    Loading.Load(new Level(level, true), true, Manager);
                }

            }
        }

        #endregion

        #region Draw

        public override void Draw(GameTime gameTime)
        {
            spriteBatch = Manager.SpriteBatch;
            Viewport viewport = Manager.GraphicsDevice.Viewport;

            // begin drawing
            // this is where the camera is used to change the view
            spriteBatch.Begin(SpriteSortMode.BackToFront, BlendState.AlphaBlend,
                              null, null, null, null, camera.get_transformation(Manager.RedSea.GraphicsDevice));

            // draw the walls of the level
            levelWall.Draw(spriteBatch);

            // draw each enemy
            foreach (Cell e in enemies)
                e.Draw(spriteBatch);

            // draw the player
            player.Draw(spriteBatch);

            // draw win or lose notification
            if (isWin)
                spriteBatch.Draw(winStatement, new Vector2(player.Center.X - 100, player.Center.Y - 50),
                    null, Color.White, 0, Vector2.Zero, 1, SpriteEffects.None, 0f);
            if (isLose)
                spriteBatch.Draw(loseStatement, new Vector2(player.Center.X - 100, player.Center.Y - 50),
                    null, Color.White, 0, Vector2.Zero, 1, SpriteEffects.None, 0f);

            // stop drawing
            spriteBatch.End();
        }

        #endregion
    }
}
