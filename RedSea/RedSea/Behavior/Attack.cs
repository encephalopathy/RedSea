using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using FarseerPhysics.Dynamics;
using FarseerPhysics.Factories;

namespace RedSeaGame
{
    class Attack : Behavior
    {
        #region Attack States
        /// <summary>
        /// Attack state class that is used to determine
        /// what type of behavior a white blood cell should
        /// execute.
        /// </summary>
        enum AttackStates
        {
            CASUAL,
            AGGRESSIVE
        }
        #endregion

        #region Fields

        //Defines the current attack state of this cell
        AttackStates state;

        //Path variable that will store A*'s path locations
        Stack<GridCell> path = new Stack<GridCell>();

        //Stores the grid of movable locations
        Grid GameGrid;

        //Stores the current cell
        Cell cell;

        //Detection radius that is used to define whether a cell should attack the player
        //Causally (A*) or Aggressively (Brute force: Go To destination)
        float detectionRange;
        #endregion

        #region CONSTRUCTOR
        /// <summary>
        /// The constructor stops the cell in place and sets its default attack mode to
        /// causal movement.
        /// </summary>
        /// <param name="cell">The current cell that will be attacking</param>
        /// <param name="cellBody">The cell body that controls the physics for the cell</param>
        /// <param name="gameGrid">The grid that is used for pathfinding</param>
        /// <param name="range">Detection range of the current cell</param>
        public Attack(Cell cell, ref Body cellBody, ref Grid gameGrid, float range)
        {
            CellBody = cellBody;
            GameGrid = gameGrid;
            this.cell = cell;
            detectionRange = range;
            state = AttackStates.CASUAL;
            CellBody.LinearVelocity = new Vector2(0f, 0f);
        }
        #endregion

        #region METHODS

        /// <summary>
        /// Makes the cell move towards the player
        /// </summary>
        protected override void Move()
        {
            MoveTowards(GameGrid.GetPlayerExactLocation);
        }

        /// <summary>
        /// Uses A* pathfinding to calculate a path from this cell to the player
        /// </summary>
        private void Perform_AStar()
        {
            PathFind findPlayer = new PathFind(GameGrid, GameGrid.getGridCellThatCellIsContainedIn(cell),
                GameGrid.PlayerGridCell);
            path = findPlayer.getPath;
        }

        /// <summary>
        /// Moves the towards this location relative to where the cell is.  The closer the
        /// location is, the slow the object moves and vice versa.
        /// </summary>
        /// <param name="location">Location to where the cell wants to travel</param>
        protected void MoveTowards(Vector2 location)
        {
            float VelocityX = CalcVelocity(CellBody.Position.X, location.X);
            float VelocityY = CalcVelocity(CellBody.Position.Y, location.Y);
            CellBody.LinearVelocity = new Vector2(VelocityX,
                VelocityY);
        }

        /// <summary>
        /// Calculates the velocity: V-Vo/t such that t = 1
        /// </summary>
        /// <param name="PositionInit"></param>
        /// <param name="PositionFinal"></param>
        /// <returns></returns>
        private float CalcVelocity(float PositionInit, float PositionFinal)
        {
            return PositionFinal - PositionInit;
        }

        /// <summary>
        /// Special method that performs a A* move towards a particlar point on the screen
        /// </summary>
        private void AStarGoTowardsPoint()
        {
            GridCell moveHere = path.Pop();
            MoveTowards(moveHere.PositionCenterOnScreen);
        }

        /// <summary>
        /// Boolean method that checks to see whether this cell should casually move towards
        /// the player or aggressively move towards the player.
        /// </summary>
        /// <returns></returns>
        private bool CheckAttackStates()
        {
            return Vector2.Distance(CellBody.Position, GameGrid.GetPlayerExactLocation)
                < MathHelper.Distance(detectionRange, detectionRange);
        }

        /// <summary>
        /// Causal Move towards A point specified by A* path else if the path is
        /// empty then we recalculate the path by calling A* again
        /// </summary>
        private void CasualMove()
        {
                if (path.Count > 0)
                {
                    AStarGoTowardsPoint();
                }
                else
                {
                    Perform_AStar();
                }
        }

        /// <summary>
        /// Sets the state for whether this cell should move causally or aggressively towards
        /// the player.
        /// </summary>
        /// <returns>The attack state of the player</returns>
        private AttackStates SetState()
        {
            if (CheckAttackStates())
            {
                MoveTowards(GameGrid.PlayerGridCell.PositionCenterOnScreen);
                return AttackStates.AGGRESSIVE;
            }
            else
            {
                return AttackStates.CASUAL;
            }
        }

        /// <summary>
        /// Moves the cell based on the attack state of the cell.
        /// </summary>
        public override void Update()
        {
            state = SetState();
            if (state == AttackStates.CASUAL)
            {
                CasualMove();
            }
            else
            {
              Move();
            }
        }
        #endregion
    }
}
