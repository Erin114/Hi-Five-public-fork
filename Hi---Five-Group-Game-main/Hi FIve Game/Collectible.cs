using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

using System;
using System.Collections.Generic;
using System.Text;

namespace Hi_FIve_Game
{
    /// <summary>
    /// Types of collectibles that can be picked up
    /// </summary>
    public enum CollectibleType
    {
        RocketAmmo,
        ShotgunAmmo,
        Health,
        ClearDoor
    }

    class Collectible : GameObject
    {
        //---Fields----------------------------------------------------------------------
        CollectibleType collectType;

        //---Properties------------------------------------------------------------------

        public CollectibleType Type
        {
            get
            {
                return collectType;
            }
        }

        //---Constructors----------------------------------------------------------------

        /// <summary>
        /// Create a Collectible with a bounding box, texture, and type
        /// </summary>
        /// <param name="x">X-coordinate (Top-Left)</param>
        /// <param name="y">Y-coordinate (Top-Left)</param>
        /// <param name="tex">Texture to be drawn with</param>
        /// <param name="type">Type of Collectible</param>
        public Collectible(int x, int y,
            Texture2D tex, CollectibleType type) :
            base(x, y, WIDTH, HEIGHT, tex)
        {
            collectType = type;
            
            AssignSprite();

            OnCollision += CollisionResolver.CollectiblePlatform;
        }

        /// <summary>
        /// Pulls the collectible down with gravity
        /// </summary>
        private void ApplyGravity()
        {
            velocity.Y += 0.5f;
        }

        private void AssignSprite()
        {
            switch(collectType)
            {
                case CollectibleType.RocketAmmo:
                    source.X = 0;
                    source.Y = 0;
                    break;

                case CollectibleType.ShotgunAmmo:
                    source.X = 33;
                    source.Y = 0;
                    break;

                case CollectibleType.Health:
                    source.X = 66;
                    source.Y = 0;
                    break;

                case CollectibleType.ClearDoor:
                    source.X = 99;
                    source.Y = 0;
                    break;
            }

            // Reassign the static animation source
            Animation staticAnim = animations[AnimationType.Static];
            staticAnim.source = source;

            animations[AnimationType.Static] = staticAnim;
            currentAnimation = animations[AnimationType.Static];
        }
    }
}
