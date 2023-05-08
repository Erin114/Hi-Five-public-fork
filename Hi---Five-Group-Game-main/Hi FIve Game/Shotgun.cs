using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

using System;
using System.Collections.Generic;
using System.Text;

namespace Hi_FIve_Game
{
    class Shotgun : Gun
    {
        //---Fields----------------------------------------------------------------------
        private float spread;
        private int pellets;

        /// <summary>
        /// Create a shotgun shoots multiple bullets
        /// </summary>
        /// <param name="x">Gun's X position (Where the bullet comes from)</param>
        /// <param name="y">Gun's Y position (Where the bullet comes from)</param>
        /// <param name="width">Gun's width</param>
        /// <param name="height">Gun's height</param>
        /// <param name="tex">Texture to draw with</param>
        /// <param name="pelletCount">Number of pellets the shotgun shoots</param>
        /// <param name="pelletSpread">Angle of overall pellet spread (radians)</param>
        public Shotgun(int x, int y, int width, int height, Texture2D tex, Bullet b, 
            int pelletCount, float pelletSpread)
            : base("Shotgun", x, y, width, height, tex, b)
        {
            pellets = pelletCount;
            spread = pelletSpread;

            fireRate = 15;

            source.X = 33;
            source.Y = 33;
        }

        /// <summary>
        /// Shoot in a arc centered around the aimVector
        /// </summary>
        /// <returns>A list containing the fired pellets</returns>
        public override List<Bullet> Shoot()
        {
            List<Bullet> bullets = new List<Bullet>();

            if (fireTimer == 0)
            {
                // Set firing timer
                fireTimer = fireRate;

                // Find out where the bullets should start
                float startAngle = spread / 2;

                // Calculate the angle in between pellets
                float angleIncrement = spread / (pellets - 1);

                // Create all the bullets and set their velocities
                for (int i = 0; i < pellets; i++)
                {
                    // Create one of the bullets at the player's center
                    Bullet b = bulletType.Clone();
                    b.X = X - b.Width / 2 + (aimVector.X * 16);
                    b.Y = Y - b.Height / 2 + (aimVector.Y * 16);

                    // Determine the angle based on which bullet is being set
                    float angle = startAngle - (i * angleIncrement);

                    // Rotate the bullet's velocity by that angle
                    b.Velocity = GetRotatedVector(aimVector, angle);
                    b.Velocity *= b.Speed;

                    // Set a refernce to what gun the bullet was fried from
                    b.FiredFrom = Name;

                    // Rotate the bullet
                    b.ApplyRotation(MathF.Atan2(b.Velocity.Y, b.Velocity.X));

                    // Finally, add it to the list for drawing and updating
                    bullets.Add(b);
                }

                // Calculate recoil
                recoil = aimVector * 0.8f;
            }

            return bullets;
        }

        /// <summary>
        /// Makes a copy of this Shotgun
        /// </summary>
        /// <returns>A copy of the Shotgun</returns>
        public override Gun Clone()
        {
            return new Shotgun(
                (int)X,
                (int)Y,
                Width,
                Height,
                sprite,
                bulletType,
                pellets,
                spread
                );
        }
    }
}
