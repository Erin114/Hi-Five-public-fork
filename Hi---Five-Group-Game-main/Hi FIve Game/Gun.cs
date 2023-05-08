using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

using System;
using System.Collections.Generic;
using System.Text;

/*
 * Owen Wicker
 * 4/2/21
 * Handle the different types of gun to declutter
 * the Player class
 */

namespace Hi_FIve_Game
{
    /// <summary>
    /// Allows for different guns that can fire in various ways
    /// </summary>
    abstract class Gun : GameObject
    {
        //---Fields----------------------------------------------------------------------
        protected int fireRate; // Frames between firings
        protected int fireTimer;

        protected Bullet bulletType;

        protected string name;

        protected Vector2 aimVector;
        protected Vector2 recoil;

        /// <summary>
        /// Get the name of this gun
        /// </summary>
        public string Name
        {
            get
            {
                return name;
            }
        }

        /// <summary>
        /// Get the force applied to a character after shooting
        /// </summary>
        public Vector2 Recoil
        {
            get
            {
                return -recoil;
            }
        }

        public Vector2 Aim
        {
            get
            {
                return aimVector;
            }
        }

        public int Timer
        {
            get
            {
                return fireTimer;
            }
        }

        /// <summary>
        /// Create a shotgun shoots multiple bullets
        /// </summary>
        /// <param name="x">Gun's X position (Where the bullet comes from)</param>
        /// <param name="y">Gun's Y position (Where the bullet comes from)</param>
        /// <param name="width">Gun's width</param>
        /// <param name="height">Gun's height</param>
        /// <param name="tex">Texture to draw with</param>
        public Gun(string name, int x, int y, int width, int height, Texture2D tex, Bullet b)
            : base(x, y, width, height, tex)
        {
            // Set basic bullet fields
            fireRate = 0; // Just so it's not null
            fireTimer = 0;

            this.name = name;

            bulletType = b.Clone();

            aimVector = new Vector2(0.01f, 0.01f);

            recoil = Vector2.Zero;
        }


        /// <summary>
        /// Aim the gun towards a target vector/point
        /// </summary>
        /// <param name="target">The point to aim at</param>
        public void AimAt(Vector2 target)
        {
            aimVector = target - LocalPosition;
            aimVector.Normalize();
        }

        /// <summary>
        /// Determine how to rotate the gun sprite
        /// and update the fired bullets
        /// </summary>
        public void Update()
        {
            // Calculate the rotation of the gun sprite
            if (isActive)
            {
                // Decrease time until the gun can fire again
                if (fireTimer > 0)
                {
                    fireTimer--;
                }
            }
        }

        /// <summary>
        /// Draws the gun sprite pointing towards the mouse
        /// </summary>
        /// <param name="sb">Sprite Batch to draw with</param>
        public override void Draw(SpriteBatch sb)
        {
            /*
            if (isActive)
            {
                sb.Draw(
                    sprite,
                    bounds,
                    source,
                    tint,
                    angle,
                    new Vector2(SOURCE_WIDTH / 2, SOURCE_HEIGHT / 2),
                    gunFlip,
                    0f
                    );
            }
            */
        }

        /// <summary>
        /// Shoots the gun depending on the gun itself
        /// </summary>
        /// <return>Gives a list conatining the shot bullets</return>
        public abstract List<Bullet> Shoot();

        /// <summary>
        /// Rotates a vector about its origin.
        /// Used primarily for shotgun spread
        /// </summary>
        /// <param name="target">The vector to rotate</param>
        /// <param name="angle">The angle to rotate by in radians</param>
        /// <returns>The rotated vector</returns>
        protected Vector2 GetRotatedVector(Vector2 target, float angle)
        {
            // Set new vector to the same as the target
            Vector2 rotated = Vector2.Zero;

            // Rotate the vector
            rotated.X = target.X * (float)Math.Cos(angle) - target.Y * (float)Math.Sin(angle);
            rotated.Y = target.Y * (float)Math.Cos(angle) + target.X * (float)Math.Sin(angle);

            return rotated;
        }

        public abstract Gun Clone();
    }
}
