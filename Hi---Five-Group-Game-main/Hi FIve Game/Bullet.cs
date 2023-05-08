using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

using System;
using System.Collections.Generic;
using System.Text;

/*
 * Owen Wicker
 * 3/16/21
 * Projectile class used by the player to fight enemies
 */

namespace Hi_FIve_Game
{
    /// <summary>
    /// Projectile that has modifiable damage, size, and patterns
    /// </summary>
    class Bullet : GameObject
    {
        //---Fields-----------------------------------------------------------------------
        private float speed;

        private string gunFiredFrom;

        private Bullet explosion;
        private float bulletSizeDecreaseRate;

        private int lifetime;

        /// <summary>
        /// Get the speed of the bullet
        /// </summary>
        public float Speed
        {
            get
            {
                return speed;
            }
        }

        /// <summary>
        /// Get a reference to the gun that this bullet was fired from
        /// </summary>
        public string FiredFrom
        {
            get
            {
                return gunFiredFrom;
            }

            set
            {
                gunFiredFrom = value;
            }
        }

        /// <summary>
        /// Gets the resulting explosion from a Rocket.
        /// Returns null if the Bullet is not a rocket
        /// or has not exploded yet
        /// </summary>
        public Bullet Explosion
        {
            get
            {
                return explosion;
            }
        }

        /// <summary>
        /// Create a Bullet with a bounding box, texture
        /// </summary>
        /// <param name="x">X-coordinate (Top-Left)</param>
        /// <param name="y">Y-coordinate (Top-Left)</param>
        /// <param name="width">Object's width</param>
        /// <param name="height">Object's height</param>
        /// <param name="texture">Texture to be drawn with</param>
        /// <param name="spd">How fast the bullet travels</param>
        /// <param name="type">Gun in which the bullet will be fired from</param>
        public Bullet(int x, int y, int width, int height, Texture2D texture, float spd) :
            base(x, y, width, height, texture)
        {
            // Bullet fields
            speed = spd;
            gunFiredFrom = null;
            lifetime = 120; // Bullets only last so long
            bulletSizeDecreaseRate = 0;

            // Assign Collision events
            OnCollision += CollisionResolver.BulletEnemy;   // Only applies to the player
            OnCollision += CollisionResolver.BulletPlatform;// Works with all bullets
            OnCollision += CollisionResolver.BulletPlayer;  // Only applies to the shooter enemy
        }

        /// <summary>
        /// Name: Troy Corrington
        /// temporary overload
        /// </summary>
        public Bullet(int x, int y, int width, int height, 
            Texture2D texture, int textureX, int textureY, float spd) :
            base(x, y, width, height, texture, textureX, textureY, SOURCE_WIDTH, SOURCE_HEIGHT)
        {
            // Bullet fields
            speed = spd;
            gunFiredFrom = null;
            lifetime = 120; // Bullets only last so long

            // Assign Collision events
            OnCollision += CollisionResolver.BulletEnemy;
            OnCollision += CollisionResolver.BulletPlatform;
            OnCollision += CollisionResolver.BulletPlayer;
        }

        //---Bullet Methods---//

        /// <summary>
        /// Updates the Bullet's position by its velocity
        /// </summary>
        public void Update()
        {
            // Deactivate a bullet after the timer runs out
            if (lifetime < 0)
            {
                Destroy();
            }

            if (isActive)
            {
                // Update position
                Position += Velocity;

                // Deactivates bullet if it is off the screen
                DestroyOffScreen();

                // Decrease bullet lifetime
                lifetime--;

                // Animate explosions
                if (gunFiredFrom == "Explode")
                {
                    // Change Bounds and Hitbox depending on the lifetime
                    X += bulletSizeDecreaseRate / 2;
                    Y += bulletSizeDecreaseRate / 2;

                    hitbox.Width -= (int)bulletSizeDecreaseRate;
                    bounds.Width -= (int)bulletSizeDecreaseRate;

                    hitbox.Height -= (int)bulletSizeDecreaseRate;
                    bounds.Height -= (int)bulletSizeDecreaseRate;

                    // Accelerate the decrease of the bounds and hitbox
                    bulletSizeDecreaseRate += 0.2f;
                }
            }
        }

        public override void Draw(SpriteBatch sb)
        {
            if (isActive)
            {
                sb.Draw(
                    sprite,
                    new Rectangle(
                        bounds.X + Width / 2, 
                        bounds.Y + Height / 2, 
                        bounds.Width, 
                        bounds.Height
                        ),
                    source,
                    tint,
                    rotation,
                    new Vector2(SOURCE_WIDTH / 2, SOURCE_HEIGHT / 2),
                    SpriteEffects.None,
                    0
                    );
            }
        }

        /// <summary>
        /// Deactivates the Bullet if it goes off of the local screen space
        /// </summary>
        public void DestroyOffScreen()
        {
            // Check if the bullet is outside the screen
            if (!Hitbox.Intersects(Camera.Bounds))
            {
                // Deactivate
                this.Active = false;
            }
        }

        /// <summary>
        /// Creates a new Bullet in its place that is larger
        /// and will explode platforms and enemies
        /// </summary>
        internal void Explode()
        {
            // Create the explosion bullet
            int explosionWidth = (int)(WIDTH * 1.5f);
            int explosionHeight = (int)(HEIGHT * 1.5f);

            // Make the explosion bullet
            explosion = new Bullet(
                (int)Center.X - explosionWidth / 2,
                (int)Center.Y - explosionHeight / 2,
                explosionWidth,
                explosionHeight,
                sprite,
                99,
                33,
                0f
                );

            // Play explosion sound effect


            // Give it a unique gun name for checking later on
            explosion.gunFiredFrom = "Explode";

            // Explosions should be short
            explosion.lifetime = 12;

            // Used for explosion reactions
            explosion.speed = velocity.X;

            // Controls the animation
            explosion.bulletSizeDecreaseRate = (float)explosionWidth / (2 * explosion.lifetime);
        }

        /// <summary>
        /// Creates a clone of this bullet for use when shooting from a gun
        /// </summary>
        /// <returns>A clone of this bullet</returns>
        public Bullet Clone()
        {
            Bullet b = new Bullet(
                (int)X,
                (int)Y,
                Width,
                Height,
                sprite,
                source.X,
                source.Y,
                Speed
                );

            b.FiredFrom = this.gunFiredFrom;
            b.lifetime = 120;

            return b;
        }

        /// <summary>
        /// Applies a rotation to the bullet's sprite
        /// </summary>
        /// <param name="angle">Angle to rotate by</param>
        public void ApplyRotation(float angle)
        {
            rotation = angle;
        }
    }
}
