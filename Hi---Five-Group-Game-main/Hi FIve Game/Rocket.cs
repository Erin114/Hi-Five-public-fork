using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

using System;
using System.Collections.Generic;
using System.Text;

namespace Hi_FIve_Game
{
    class Rocket : Gun
    {
        /// <summary>
        /// Creates a pistol that the Player can shoot
        /// </summary>
        /// <param name="x">X position of the pistol</param>
        /// <param name="y">Y position of the pistol</param>
        /// <param name="width">Width of the pistol</param>
        /// <param name="height">Height of the pistol</param>
        /// <param name="tex">Pistol's texture</param>
        /// <param name="b">Bullet that the pistol shoots</param>
        public Rocket(int x, int y, int width, int height, Texture2D tex, Bullet b) :
            base("Rocket", x, y, width, height, tex, b)
        {
            fireRate = 30;

            source.X = 66;
            source.Y = 33;
            
        }

        /// <summary>
        /// Shoots one bullet towards the mouse
        /// </summary>
        /// <returns>A list containg every shot bullet</returns>
        public override List<Bullet> Shoot()
        {
            List<Bullet> bullets = new List<Bullet>();

            if (fireTimer == 0)
            {
                // Set firing timer
                fireTimer = fireRate;

                // Make the bullet at the center of the gun
                Bullet b = bulletType.Clone();
                b.X = X - b.Width / 2 + (aimVector.Y * 16);
                b.Y = Y - b.Height / 2 + (aimVector.Y * 16);

                // Set velocity towards where the player aims
                b.Velocity = aimVector * b.Speed;

                // Add to the list for updating
                bullets.Add(b);

                // Set a refernce to what gun the bullet was fried from
                b.FiredFrom = Name;

                // Rotate the bullet
                b.ApplyRotation(MathF.Atan2(aimVector.Y, aimVector.X));

                // Assign the Destruction event
                b.OnDestruction += b.Explode;

                // Calculate recoil
                recoil = Vector2.Zero;
            }

            return bullets;
        }

        /// <summary>
        /// Makes a copy of this Rocket
        /// </summary>
        /// <returns>A copy of the pistol</returns>
        public override Gun Clone()
        {
            return new Rocket(
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
