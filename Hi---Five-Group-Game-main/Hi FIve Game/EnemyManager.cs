using System;
using System.Collections.Generic;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

// Author: Patrick Doherty
// Start Date: 04/07/21
// Purpose: A class that manages all the Enemies for the level

namespace Hi_FIve_Game
{
    class EnemyManager : GeneralManager<Character>
    {
        // Fields ===========
        private int hasPlayer;
        private Player player;

        // Properties =======
        public int HasPlayer { get { return hasPlayer; } }
        
        // Constructor ======
        public EnemyManager(List<Character> enemies, int screenWidth, int screenHeight, Player player) : 
            base( enemies, screenWidth, screenHeight)
        {
            this.player = player;
            this.hasPlayer = int.MinValue;
        }
        
        // Default Methods ==========

        /// <summary>
        /// Called at the end of a frame to update the lists
        /// & set objects accordingly
        /// </summary>
        public override void EndFrame()
        {
            // Update the globals first
            // then check if things are moved on or off screen
            CheckActive();

            // These lists will hold the items that are being moved between lists
            List<Character> tempOnScreen = new List<Character>();
            List<Character> tempOffScreen = new List<Character>();

            // Update OnScreen & OffScreen Lists
            // Loop through onScreen checking what is & isn't active
            if (onScreen.Count != 0)
            {
                for (int i = 0; i < onScreen.Count; i++)
                {
                    // Check if it is no longer active
                    if (onScreen[i].Active == false)
                    {
                        tempOffScreen.Add(onScreen[i]);
                    }
                }
            }
            if (offScreen.Count != 0)
            {
                for (int i = 0; i < offScreen.Count; i++)
                {
                    // Check if it is now active
                    if (offScreen[i].Active == true)
                    {
                        tempOnScreen.Add(onScreen[i]);
                    }
                }
            }
            if (tempOffScreen.Count != 0)
            {
                for (int i = 0; i < tempOffScreen.Count; i++)
                {
                    // Remove the things that are OffScreen from the OnScren List
                    onScreen.Remove(tempOffScreen[i]);

                    // Add them to Off Screen
                    offScreen.Add(tempOffScreen[i]);
                }
            }
            if (tempOnScreen.Count != 0)
            {
                for (int i = 0; i < tempOnScreen.Count; i++)
                {
                    offScreen.Remove(tempOnScreen[i]);
                    onScreen.Add(tempOnScreen[i]);
                }
            }


            // Reset the Quadrants & sorts them
            quad0.Clear();
            quad1.Clear();
            quad2.Clear();
            quad3.Clear();
            SortQuadrant();

            // Now we Sort the Player
            SortPlayer(player);
        }

        /// <summary>
        /// Called in the reset level method so we don't have to
        /// manually reset the enemies & player
        /// </summary>
        public override void ResetGame()
        {
            // Create a placeholder character
            //Character current;

            objects.Clear();

            // Reset the player
            player.Reset();

            // Clear are on & off screen lists as well as quadrants
            onScreen.Clear();
            offScreen.Clear();
            quad0.Clear();
            quad1.Clear();
            quad2.Clear();
            quad3.Clear();

            // Check active to make sure only things on screen should be active
            CheckActive();

            // Next we will sort active
            SortActive();

            // then sort quadrants & we should be done
            SortQuadrant();
            SortPlayer(player);
        }

        /// <summary>
        /// Sorts all objects & puts them in the respective
        /// General list
        /// </summary>
        public override void SortActive()
        {
            for (int i = 0; i < objects.Count; i++)
            {
                if (objects[i].Active == true)
                {
                    onScreen.Add(objects[i]);
                }
                else
                {
                    offScreen.Add(objects[i]);
                }
            }
        }

        /// <summary>
        /// Sorts all the active objects into the quadrant they're in
        /// </summary>
        public override void SortQuadrant()
        {
            bool isLeft = default;
            bool isTop = default;


            for (int i = 0; i < onScreen.Count; i++)
            {
                // Check if it's in the left or the right
                if (onScreen[i].X <= left)
                {
                    isLeft = true;
                }
                else if (onScreen[i].X <= right &&
                    onScreen[i].X >= left)
                {
                    isLeft = false;
                }

                // Check if it's on the top or bottom
                if (onScreen[i].Y <= top)
                {
                    isTop = true;
                }
                else if (onScreen[i].Y <= bottom &&
                    onScreen[i].Y >= top)
                {
                    isTop = false;
                }

                // Put it into the correspondeing list based off of the bools
                if (isLeft == true &&
                    isTop == true)
                {
                    quad0.Add(onScreen[i]);
                }
                else if (isLeft == false &&
                    isTop == true)
                {
                    quad1.Add(onScreen[i]);
                }
                else if (isLeft == true &&
                    isTop == false)
                {
                    quad2.Add(onScreen[i]);
                }
                else if (isLeft == false &&
                    isTop == false)
                {
                    quad3.Add(onScreen[i]);
                }
            }
        }


        // Extra Methods
        /// <summary>
        /// Calls HoldsPlayer & puts the player in the proper list
        /// </summary>
        /// <param name="item"> The player being added</param>
        private void SortPlayer(Character item)
        {
            int location =  HoldsPlayer();
            switch (location)
            {
                case 0:
                    {
                        // player is in quad 0
                        quad0.Add(item);
                        break;
                    }
                case 1:
                    {
                        // player is in quad 1
                        quad1.Add(item);
                        break;
                    }
                case 2:
                    {
                        // player is in quad 2
                        quad2.Add(item);
                        break;
                    }
                case 3:
                    {
                        // player is in quad 3
                        quad3.Add(item);
                        break;
                    }
            }
        }

        /// <summary>
        /// Looks at the player coordinates & decides what quad they're in
        /// </summary>
        /// <returns>The index of the quadrant that has the player</returns>
        private int HoldsPlayer()
        {
            int returnable = -1;

            if (player.Hitbox.X <= left)
            {
                // They are in quad 0 or 2
                // Check Y values
                if (player.Hitbox.Y <= top)
                {
                    returnable = 0;
                }
                else if (player.Hitbox.Y <= bottom)
                {
                    returnable = 2;
                }
            }
            else if (player.Hitbox.X <= right)
            {
                // They are in quad 1 or 3
                // Check y Values
                if(player.Hitbox.Y <= top)
                {
                    returnable = 1;
                }
                else if(player.Hitbox.Y <= bottom)
                {
                    returnable = 3;
                }
            }

            return returnable;
        }

        private void CheckActive()
        {
            // This method checks an object to see if it should be
            // Drawn on Screen then adjust it's active bool accordingly

            foreach (Enemy enemy in objects)
            {
                // Check if the camera rectangle has it if not,
                // It will be off screen so deactivate it
                if (Camera.Bounds.Contains(enemy.Bounds) == true)
                {
                    enemy.Active = true;
                }
                else if(Camera.Bounds.Intersects(enemy.Bounds))
                {
                    // Check if it should be inside
                    Rectangle overlap = Rectangle.Intersect(Camera.Bounds, enemy.Bounds);
                    if(enemy.Bounds.X < overlap.X)
                    {
                        // The enemy is more to the right then the left so they are onscreen
                        enemy.Active = true;
                    }
                    else if(enemy.Bounds.X > overlap.X)
                    {
                        // The enemy is more to the left so they are offscreen
                        enemy.Active = false;
                    }
                }
                else
                {
                    // They're not inside & they don't intersect so they aren't active
                    enemy.Active = false;
                }
            }
        }
    }
}
