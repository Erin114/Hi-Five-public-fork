using System;
using System.Collections.Generic;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

// Author: Patrick Doherty
// Start Date: 04/07/21
// Purpose: A class that manages all the Collectibles for the level

namespace Hi_FIve_Game
{
    class CollectibleManager : GeneralManager<Collectible>
    {
        // Fields ===========
        List<Collectible> collectibles;
        // Properties =======
        
        // Constructor ======
        public CollectibleManager(List<Collectible> collectibles, int screenWidth, int screenHeight) : 
            base(collectibles, screenWidth, screenHeight)
        {
            this.collectibles = collectibles;
        }

        // Methods ==========

        /// <summary>
        /// Called at the end of a frame to update the lists
        /// & set objects accordingly
        /// </summary>
        public override void EndFrame()
        {
            // Check if things are moved on or offscreen
            CheckActive();

            // These lists will hold the items that are being moved between lists
            List<Collectible> tempOnScreen = new List<Collectible>();
            List<Collectible> tempOffScreen = new List<Collectible>();

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
                    if (onScreen[i].Active == true)
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
                    onScreen.Remove(tempOnScreen[i]);
                    onScreen.Add(tempOnScreen[i]);
                }
            }

            // Reset the Quadrants & sorts them
            quad0.Clear();
            quad1.Clear();
            quad2.Clear();
            quad3.Clear();
            SortQuadrant();
        }

        /// <summary>
        /// Resets all collectibles during level loading
        /// </summary>
        public override void ResetGame()
        {
            collectibles.Clear();
            quad0.Clear();
            quad1.Clear();
            quad2.Clear();
            quad3.Clear();
            onScreen.Clear();
            offScreen.Clear();

            // Call Check active since the Reset will always make an object active
            CheckActive();

            SortActive();
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

        /// <summary>
        /// Will solve problems from resetting
        /// Checks if the object is in the camera bounds
        /// </summary>
        private void CheckActive()
        {
            // This method checks an object to see if it should be
            // Drawn on Screen then adjust it's active bool accordingly

            foreach (Collectible collect in objects)
            {
                // Check if the camera rectangle has it if not,
                // It will be off screen so deactivate it
                if (Camera.Bounds.Contains(collect.Bounds) == true)
                {
                    collect.Active = true;
                }
                else if (Camera.Bounds.Intersects(collect.Bounds))
                {
                    // Check if it should be inside
                    Rectangle overlap = Rectangle.Intersect(Camera.Bounds, collect.Bounds);
                    if (collect.Bounds.X < overlap.X)
                    {
                        // The enemy is more to the right then the left so they are onscreen
                        collect.Active = true;
                    }
                    else if (collect.Bounds.X > overlap.X)
                    {
                        // The enemy is more to the left so they are offscreen
                        collect.Active = false;
                    }
                }
                else
                {
                    // They're not inside & they don't intersect so they aren't active
                    collect.Active = false;
                }
            }
        }
    }
}
