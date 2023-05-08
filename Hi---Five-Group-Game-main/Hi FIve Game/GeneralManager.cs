using System;
using System.Collections.Generic;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

// Author: Patrick Doherty
// Start Date: 04/07/21
// Purpose: Make the base manager that the others inherit

namespace Hi_FIve_Game
{
    abstract class GeneralManager<T>
    {
        // Fields ================
        protected List<T> objects;

        // The lists for the different quadarants
        protected List<T> quad0;
        protected List<T> quad1;
        protected List<T> quad2;
        protected List<T> quad3;

        // General lists for whether an object is active or not
        protected List<T> onScreen;
        protected List<T> offScreen;

        // Quadarants are numbered like this
        //  0  |  1
        //  2  |  3

        private Rectangle[] quadrants;

        // These are the far sides of each quadrant
        protected int left;
        protected int right;
        protected int top;
        protected int bottom;


        // Constructor ===========
        public GeneralManager(List<T> objects, int screenWidth, int screenHeight)
        {
            quad0 = new List<T>();
            quad1 = new List<T>();
            quad2 = new List<T>();
            quad3 = new List<T>();


            onScreen = new List<T>();
            offScreen = new List<T>();

            this.objects = objects;
            this.quadrants = new Rectangle[4];
            quadrants[0] = new Rectangle(
                0, 
                0, 
                screenWidth / 2, 
                screenHeight / 2);
            quadrants[1] = new Rectangle(
                screenWidth / 2, 
                0,
                screenWidth / 2,
                screenHeight / 2);
            quadrants[2] = new Rectangle(
                0, 
                screenHeight / 2,
                screenWidth / 2,
                screenHeight / 2);
            quadrants[3] = new Rectangle(
                screenWidth / 2,
                screenHeight / 2,
                screenWidth / 2,
                screenHeight / 2);

            // These are the far sides of each quadrant
            left = Quadrants[0].X + Quadrants[0].Width;
            right = Quadrants[3].X + Quadrants[3].Width;
            top = Quadrants[0].Y + Quadrants[0].Height;
            bottom = Quadrants[3].Y + Quadrants[3].Height;

        }
        
        // Properties ============
        public List<T> Objects { get { return objects; } set { objects = value; } }
        public Rectangle[] Quadrants { get { return quadrants; } set { quadrants = value; } }

        // Methods ===============
        /// <summary>
        /// Resets all of the objects to their start state
        /// </summary>
        public abstract void ResetGame();

        /// <summary>
        /// Sorts all objects & puts them in the respective
        /// General list
        /// </summary>
        public abstract void SortActive();

        /// <summary>
        /// Sorts all the active objects into the quadrant they're in
        /// </summary>
        public abstract void SortQuadrant();

        /// <summary>
        /// Called at the end of a frame to update the lists
        /// & set objects accordingly
        /// </summary>
        public abstract void EndFrame();
    }
}
