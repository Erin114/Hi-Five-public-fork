using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Audio;
using System;
using System.Collections.Generic;
using System.Text;

namespace Hi_FIve_Game
{
    class Shooter : Enemy
    {
        //---Fields----------------------------------------------------------------------

        // Thin rectangle pointing from player's center
        // to enemy's center to check for any platforms
        // that are in the way
        private Rectangle lineOfSight;

        // Determines if the enemy can see the player
        private bool inSight;

        private List<Bullet> enemyBullets;

        //---Properties------------------------------------------------------------------

        /// <summary>
        /// Get the 
        /// </summary>
        public Rectangle LineOfSight
        {
            get
            {
                return lineOfSight;
            }
        }

        /// <summary>
        /// Get or set whether the enemy can currently see the player's center
        /// </summary>
        public bool PlayerInSight
        {
            get
            {
                return inSight;
            }

            set
            {
                inSight = value;
            }
        }

        /// <summary>
        /// Get the gun that the shooter is using
        /// </summary>
        public Gun Equipped
        {
            get
            {
                return equippedGun;
            }
        }

        public List<Bullet> ActiveBullets
        {
            get
            {
                return enemyBullets;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <param name="tex"></param>
        /// <param name="hp"></param>
        /// <param name="radius"></param>
        public Shooter(int x, int y, int width, int height, 
            Texture2D tex, int hp, int radius, Gun g, List<SoundEffect> soundEffects)
            : base(x, y, width, height, tex, hp, radius, soundEffects)
        {
            // Smaller jump height
            jumpStrength /= 2;

            // Set a null line of sight initially
            lineOfSight = default(Rectangle);

            // Assign the gun
            equippedGun = g;

            // Create a bullet list
            enemyBullets = new List<Bullet>();

            // Asssign events
            OnDestruction += equippedGun.Destroy; // Destroy gun when shooter is destroyed
        }

        /// <summary>
        /// Move the enemy
        /// </summary>
        public override void Update(GameTime gameTime)
        {
            if (isActive)
            {
                // Create a line of sight towards the player
                lineOfSight = new Rectangle(
                    (int)Math.Min(playerCenter.X, Center.X),
                    (int)Math.Min(playerCenter.Y, Center.Y),
                    (int)Math.Abs(playerCenter.X - Center.X),
                    (int)Math.Abs(playerCenter.Y - Center.Y)
                    );

                // Allow the gun to shoot and move it with the player
                equippedGun.Update();
                equippedGun.Position = Center;
            }

            // Loop through each active bullet and update it
            // Go from back to front because the oldest bullets will likely
            // be inactive first
            for (int i = enemyBullets.Count - 1; i > -1; i--)
            {
                // Just so it looks cleaner
                Bullet b = enemyBullets[i];

                // Make sure the explosion exists
                if (b.Explosion != null)
                {
                    enemyBullets.Add(b.Explosion);
                }

                // Update and check if they are off the screen
                if (!b.Active)
                {
                    // Remove the bullet it if it is inactive
                    enemyBullets.Remove(b);
                } else
                {
                    // Update the bullet and deactivate if necessary
                    b.Update();
                }
            }

            base.Update(gameTime);
        }

        // <summary>
        /// Draws the player as well any bullets they shot
        /// </summary>
        /// <param name="sb">Spritebatch that draws</param>
        public override void Draw(SpriteBatch sb)
        {
            // Draw each bullet
            foreach (Bullet b in enemyBullets)
            {
                b.Draw(sb);

                if (b.Explosion != null)
                {
                    b.Explosion.Draw(sb);
                }
            }

            base.Draw(sb);
        }

        /// <summary>
        /// Move the enemy based on its position in relation
        /// to the player
        /// </summary>
        protected override void CharacterMovement()
        {
            // Make sure player is in range and
            // can be seen
            if (IsPlayerInRadius() && inSight)
            {
                // Get distance from player to Shooter
                Vector2 distanceToPlayer = playerCenter - Center;

                float distance = distanceToPlayer.Length();

                // Move away if the player is too close
                if (distance < viewRadius * 0.25f)
                {
                    // Make the shooter move through jumping
                    Jump();

                    // Only move while in the air
                    if (State == CharacterState.Jump)
                    {
                        // Player is Left
                        if (distanceToPlayer.X < 0)
                        {
                            MoveRight();
                        }

                        // Player is Right
                        if (distanceToPlayer.X > 0)
                        {
                            MoveLeft();
                        }
                    }
                }

                // Move closer if the player starts to run
                else if (distance > viewRadius * 0.75f)
                {
                    // Make the shooter move through jumping
                    Jump();

                    // Only move while in the air
                    if (State == CharacterState.Jump)
                    {

                        // Player is Left
                        if (distanceToPlayer.X < 0)
                        {
                            MoveLeft();
                        }

                        // Player is Right
                        if (distanceToPlayer.X > 0)
                        {
                            MoveRight();
                        }
                    }
                }

                // Only shoot if on the ground
                else if (state != CharacterState.Jump)
                {
                    equippedGun.AimAt(Vector2.Transform(playerCenter, Camera.Transform));
                    enemyBullets.AddRange(equippedGun.Shoot());

                    State = CharacterState.Shoot;

                    PlaySoundEffects(soundEffects[2]);
                }
            }
        }

        /// <summary>
        /// Resets the Shooter's position and gets rid of stray bullets
        /// </summary>
        public override void Reset()
        {
            // Clear bullets from the screen
            enemyBullets.Clear();

            // Set gun to active
            equippedGun.Active = true;

            base.Reset();
        }

        /// <summary>
        /// An ovveride of TakeDamage that plays sounds
        /// depending on how much health the shooter
        /// currently has
        /// </summary>
        /// <param name="damage">The damage taken</param>
        public override void TakeDamage(int damage)
        {
            if (Health <= 1)
            {
                PlaySoundEffects(soundEffects[7]);
            }
            else
            {
                PlaySoundEffects(soundEffects[6]);
            }

            base.TakeDamage(damage);
        }
    }
}
