using System;
using System.Collections.Generic;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

// Author: Patrick Doherty
// Start Date: 04/07/21
// Purpose: A class that manages all the Platforms for the level

namespace Hi_FIve_Game
{
    class PlatformManager : GeneralManager<Platform>
    {
        // Fields ===========
        // Properties =======
        // Constructor ======
        public PlatformManager(List<Platform> platforms, int screenWidth, int screenHeight) : 
            base(platforms, screenWidth, screenHeight)
        {

        }
        // Methods ==========

        /// <summary>
        /// Called at the end of a frame to update the lists
        /// & set objects accordingly
        /// </summary>
        public override void EndFrame()
        {
            // Check if things are moved on or off screen
            CheckActive();

            // These lists will hold the items that are being moved between lists
            List<Platform> tempOnScreen = new List<Platform>();
            List<Platform> tempOffScreen = new List<Platform>();

            // Update OnScreen & OffScreen Lists
            // Loop through onScreen checking what is & isn't active
            for (int i = 0; i < onScreen.Count; i++)
            {
                // Check if it is no longer active
                if (onScreen[i].Active == false)
                {
                    tempOffScreen.Add(onScreen[i]);
                }
            }
            for (int i = 0; i < offScreen.Count; i++)
            {
                // Check if it is now active
                if (onScreen[i].Active == true)
                {
                    tempOnScreen.Add(onScreen[i]);
                }
            }
            for (int i = 0; i < tempOffScreen.Count; i++)
            {
                // Remove the things that are OffScreen from the OnScren List
                onScreen.Remove(tempOffScreen[i]);

                // Add them to Off Screen
                offScreen.Add(tempOffScreen[i]);
            }
            for (int i = 0; i < tempOnScreen.Count; i++)
            {
                onScreen.Remove(tempOnScreen[i]);
                onScreen.Add(tempOnScreen[i]);
            }


            // Resets the Quadrants & sorts them
            quad0.Clear();
            quad1.Clear();
            quad2.Clear();
            quad3.Clear();
            SortQuadrant();
        }

        public override void ResetGame()
        {

            objects.Clear();
            quad0.Clear();
            quad1.Clear();
            quad2.Clear();
            quad3.Clear();
            onScreen.Clear();
            offScreen.Clear();

            // Call Check active to give each object it's proper bool
            CheckActive();

            // Sort active since Reset defaults the objecs to being onScreen
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
        /// Checks if the object is contained in the camera bounds
        /// </summary>
        private void CheckActive()
        {
            // This method checks an object to see if it should be
            // Drawn on Screen then adjust it's active bool accordingly

            foreach (Platform platform in objects)
            {
                // Check if the camera rectangle has it if not,
                // It will be off screen so deactivate it
                if (Camera.Bounds.Contains(platform.Bounds) == true)
                {
                    platform.Active = true;
                }
                else if (Camera.Bounds.Intersects(platform.Bounds))
                {
                    // Check if it should be inside
                    Rectangle overlap = Rectangle.Intersect(Camera.Bounds, platform.Bounds);
                    if (platform.Bounds.X < overlap.X)
                    {
                        // The enemy is more to the right then the left so they are onscreen
                        platform.Active = true;
                    }
                    else if (platform.Bounds.X > overlap.X)
                    {
                        // The enemy is more to the left so they are offscreen
                        platform.Active = false;
                    }
                }
                else
                {
                    // They're not inside & they don't intersect so they aren't active
                    platform.Active = false;
                }
            }
        }
    }
}
