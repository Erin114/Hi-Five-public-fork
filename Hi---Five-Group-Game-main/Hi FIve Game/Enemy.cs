using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Audio;

using System;
using System.Collections.Generic;
using System.Text;

/*
 * Owen Wicker
 * 3/16/21
 * Base Enemy class that will be used to make more specialized enemies
 */

namespace Hi_FIve_Game
{
    /// <summary>
    /// Base class for charger and shooter enemies that attack the player
    /// </summary>
    abstract class Enemy : Character
    {
        //---Fields---//
        protected float viewRadius;
        protected Vector2 playerCenter;

        //---Properties---//

        /// <summary>
        /// Gets the distance at which a Player will be detected
        /// and pursued by an enemy
        /// </summary>
        public float View
        {
            get
            {
                return viewRadius;
            }
        }

        //---Constructors---//

        /// <summary>
        /// Create an Enemy with a bounding box, a texture, health, and view radius
        /// </summary>
        /// <param name="x">X-coordinate (Top-Left)</param>
        /// <param name="y">Y-coordinate (Top-Left)</param>
        /// <param name="width">Object's width</param>
        /// <param name="height">Object's height</param>
        /// <param name="texture">Texture to be drawn with</param>
        /// <param name="hp">Enemy's max hit points</param>
        /// <param name="radius">Distance at which the Player is detected</param>
        public Enemy(int x, int y, int width, int height, Texture2D texture, int hp, int radius, List<SoundEffect> sounds) :
            base(x, y, width, height, texture, hp, sounds)
        {
            viewRadius = radius;
            playerCenter = Vector2.Zero;

            // Only for sake of a timer
            // Enemy does not have iFrames
            invincibilityFrames = 50;

            // Set speed to 3/4 of regular speed
            speed *= 0.75f;
        }

        //---Common Methods---//
        // Used by subclasses of Enemy //

        /// <summary>
        /// Allow the enemy to jump if it's op top of a platform
        /// </summary>
        public override void Jump()
        {
            // Make the enemy jump
            if (state != CharacterState.Jump)
            {
                PlaySoundEffects(soundEffects[0]);
                velocity.Y = -jumpStrength;
                state = CharacterState.Jump;
            }
        }

        /// <summary>
        /// Determines if the center of the player is within the view radius of the enemy
        /// </summary>
        /// <param name="p">The player</param>
        /// <returns>TRUE if the player is within the radius</returns>
        public bool IsPlayerInRadius()
        {
            // Check when the player is within the enemy's view radius
            return Vector2.Distance(playerCenter, Center) < View;
        }

        /// <summary>
        /// Grabs the center of the player for use in detection
        /// </summary>
        /// <param name="pCenter">The center of the player</param>
        public void SetPlayerCenter(Vector2 pCenter)
        {
            playerCenter = pCenter + new Vector2(0.01f, 0.01f);
        }
    }
}
