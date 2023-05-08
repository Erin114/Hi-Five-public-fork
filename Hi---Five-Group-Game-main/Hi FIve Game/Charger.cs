using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Audio;
using System;
using System.Collections.Generic;
using System.Text;

namespace Hi_FIve_Game
{
    class Charger : Enemy
    {
        /// <summary>
        /// Create a Charger with a bounding box, a texture, health, and view radius
        /// </summary>
        /// <param name="x">X-coordinate (Top-Left)</param>
        /// <param name="y">Y-coordinate (Top-Left)</param>
        /// <param name="width">Object's width</param>
        /// <param name="height">Object's height</param>
        /// <param name="tex">Texture to be drawn with</param>
        /// <param name="hp">Charger's max hit points</param>
        /// <param name="radius">Distance at which the Player is detected</param>
        public Charger(int x, int y, int width, int height, 
            Texture2D tex, int hp, int radius, List<SoundEffect> soundEffects)
            : base(x, y, width, height, tex, hp, radius, soundEffects)
        {
            // Handled by base
        }

        /// <summary>
        /// Move the enemy and determine its state
        /// </summary>
        public override void Update(GameTime gameTime)
        {

            base.Update(gameTime);
        }

        protected override void CharacterMovement()
        {
            // Make sure the player is within range
            if (IsPlayerInRadius())
            {
                // Determine distance between player and charger
                Vector2 distanceToPlayer = playerCenter - Center;

                // Move based on player's X position reltive to the charger

                // Player is left
                if (distanceToPlayer.X < 0)
                {
                    MoveLeft();
                }

                // Player is right
                if (distanceToPlayer.X > 0)
                {
                    MoveRight();
                }

                // Jump if player is above a certain height
                if (distanceToPlayer.Y < -Height * 1.5f) // 1.5 enemies higher
                {
                    // Set jump height proportional to distance above
                    jumpStrength = -distanceToPlayer.Y / 100;

                    // Limit the jump height ot that of the player
                    if (jumpStrength > 0.9f)
                    {
                        jumpStrength = 0.9f;
                    }

                    // Actually jump
                    if (State != CharacterState.Jump)
                    {
                        PlaySoundEffects(soundEffects[0]);
                        Jump();
                    }
                }
            } 
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
