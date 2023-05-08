using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

using System;
using System.Collections.Generic;
using System.Text;

/*
 * Owen Wicker
 * 4/18/21
 * Make a Camera in order to easily access the screen dimensions
 * as well as allow for easy access to the transformation matrix
 * that the spritebatch uses
 */

namespace Hi_FIve_Game
{
    /// <summary>
    /// Easy storage and access of common variables related to drawing
    /// </summary>
    static class Camera
    {
        //---Fields---------------------------------------------------------------
        private static Matrix transformationMatrix;

        private static int viewWidth;
        private static int viewHeight;

        private static Vector2 position;

        private static Rectangle bounds;
        private static Rectangle levelBounds;

        private static Vector2 followPoint;

        //---Properties------------------------------------------------------------

        /// <summary>
        /// Get the transformation matrix that defines this Camera's position
        /// </summary>
        public static Matrix Transform
        {
            get
            {
                return transformationMatrix;
            }
        }

        /// <summary>
        /// Get the Camera's local position on screen which is always (0, 0)
        /// </summary>
        public static Vector2 LocalPosition
        {
            get
            {
                return Vector2.Zero;
            }
        }

        /// <summary>
        /// Get the Camera's global coordinates
        /// </summary>
        public static Vector2 Position
        {
            get
            {
                // Set position and bounds
                position = Vector2.Transform(Vector2.Zero, Matrix.Invert(transformationMatrix));

                bounds.X = (int)position.X;
                bounds.Y = (int)position.Y;

                return position;
            }
        }

        /// <summary>
        /// Get the Camera's global bounds
        /// </summary>
        public static Rectangle Bounds
        {
            get
            {
                position = Vector2.Transform(Vector2.Zero, Matrix.Invert(transformationMatrix));

                bounds.X = (int)position.X;
                bounds.Y = (int)position.Y;

                return bounds;
            }
        }

        /// <summary>
        /// Get or set the global rectangle containing all GameObjects in the level
        /// </summary>
        public static Rectangle LevelBounds
        {
            get
            {
                return levelBounds;
            }

            set
            {
                levelBounds = value;
            }
        }

        /// <summary>
        /// Get the width of the Camera's viewport
        /// </summary>
        public static int Width
        {
            get
            {
                return viewWidth;
            }
        }

        /// <summary>
        /// Get the height of the Camera's viewport
        /// </summary>
        public static int Height
        {
            get
            {
                return viewHeight;
            }
        }

        /// <summary>
        /// Get the point that the Camera is following
        /// </summary>
        public static Vector2 Following
        {
            get
            {
                return followPoint;
            }
        }

        //---Methods---------------------------------------------------------------

        /// <summary>
        /// Initialize all fields to default values
        /// </summary>
        public static void Initialize()
        {
            // Matrix used to "move" the camera
            transformationMatrix = Matrix.Identity;

            // Width and height of the screen
            viewWidth = 1000;
            viewHeight = 750;

            // Global position of the top left corner of the camera and its bounds
            position = Vector2.Zero;

            bounds = new Rectangle(
                (int)position.X, 
                (int)position.Y, 
                viewWidth, 
                viewHeight
                );

            levelBounds = default;

            // Default follow point
            followPoint = Vector2.Zero;
        }

        /// <summary>
        /// Follows a point
        /// </summary>
        /// <param name="pos">Point to follow</param>
        public static void Follow(Vector2 pos)
        {
            // Set a default case when using this follow method
            if (pos == default)
            {
                transformationMatrix = Matrix.Identity;
                return;
            }

            followPoint = pos; // Set the point that is being followed

            // Create the appropriate transform
            transformationMatrix = Matrix.CreateTranslation(viewWidth / 2 - pos.X, viewHeight / 2 - pos.Y, 0);
        }
    }
}
