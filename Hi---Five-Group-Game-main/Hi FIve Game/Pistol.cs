using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

using System;
using System.Collections.Generic;
using System.Text;

namespace Hi_FIve_Game
{
    class Pistol : Gun
    {
        //---Fields----------------------------------------------------------------------

        /// <summary>
        /// Creates a pistol that the Player can shoot
        /// </summary>
        /// <param name="x">X position of the pistol</param>
        /// <param name="y">Y position of the pistol</param>
        /// <param name="width">Width of the pistol</param>
        /// <param name="height">Height of the pistol</param>
        /// <param name="tex">Pistol's texture</param>
        /// <param name="b">Bullet that the pistol shoots</param>
        public Pistol(int x, int y, int width, int height, Texture2D tex, Bullet b) :
            base("Pistol", x, y, width, height, tex, b)
        {
            source.Y = 33;

            fireRate = 20;
        }

        /// <summary>
        /// Shoots one bullet towards the mouse
        /// </summary>
        /// <returns>A list containg every shot bullet</returns>
        public override List<Bullet> Shoot()
        {
            // Temp list to contain the bullet
            List<Bullet> bullets = new List<Bullet>();

            if (fireTimer == 0)
            {
                fireTimer = fireRate;

                // Make the bullet at the center of the gun
                Bullet b = bulletType.Clone();
                b.X = X - b.Width / 2 + (aimVector.X * 16);
                b.Y = Y - b.Height / 2 + (aimVector.Y * 16);

                // Set velocity towards where the player aims
                b.Velocity = aimVector * b.Speed;

                // Set a refernce to what gun the bullet was fried from
                b.FiredFrom = Name;

                // Add to the list for updating
                bullets.Add(b);

                // Calculate recoil
                recoil = Vector2.Zero;
            }

            return bullets;
        }

        /// <summary>
        /// Makes a copy of this Pistol
        /// </summary>
        /// <returns>A copy of the pistol</returns>
        public override Gun Clone()
        {
            return new Pistol(
                (int)X,
                (int)Y,
                Width,
                Height,
                sprite,
                bulletType
                );
        }
    }
}
