using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

using System;
using System.Collections.Generic;
using System.Text;

/*
 * Owen Wicker
 * 3/16/21
 * Collidable object to allow for maneuverability within the level
 */

namespace Hi_FIve_Game
{
    /// <summary>
    /// Type of platform determines if it damages the player or not
    /// </summary>
    public enum PlatformType
    {
        /* Regular Platform */
        // Regular Platform Floating
        RegularLeftFloat,
        RegularMidFloat,
        RegularRightFloat,

        // Regular Platform Top
        RegularLeftTop,
        RegularMidTop,
        RegularRightTop,

        // Regular Platform Middle - for use with taller platform blocks
        RegularLeftMid,
        RegularMidMid,
        RegularRightMid,

        /* Destructible Platform */
        // Destructible Platform Floating
        DestructibleLeftFloat,
        DestructibleMidFloat,
        DestructibleRightFloat,

        // Destructible Platform Top
        DestructibleLeftTop,
        DestructibleMidTop,
        DestructibleRightTop,

        // Destructible Platform Middle
        DestructibleLeftMid,
        DestructibleMidMid,
        DestructibleRightMid,

        /* One Way Platform */
        // One Way Platform
        OneWayLeft,
        OneWayMid,
        OneWayRight,

        /* Lava Platform */
        // Lava Platform
        Lava
    }

    /// <summary>
    /// Collidable object
    /// </summary>
    class Platform : GameObject
    {
        //---Fields---//
        private PlatformType platformType;

        //---Properties---//

        /// <summary>
        /// Get the type of Platform
        /// </summary>
        public PlatformType Type
        {
            get
            {
                return platformType;
            }
        }

        /// <summary>
        /// Determines whether this platform is regular
        /// </summary>
        public bool Regular
        {
            get
            {
                switch (platformType)
                {
                    case PlatformType.RegularLeftFloat:
                    case PlatformType.RegularMidFloat:
                    case PlatformType.RegularRightFloat:
                    case PlatformType.RegularLeftTop:
                    case PlatformType.RegularMidTop:
                    case PlatformType.RegularRightTop:
                    case PlatformType.RegularLeftMid:
                    case PlatformType.RegularMidMid:
                    case PlatformType.RegularRightMid:
                        return true;
                    default:
                        return false;
                }
            }
        }

        /// <summary>
        /// Determines whether this platform is destructible
        /// </summary>
        public bool Destructible
        {
            get
            {
                switch (platformType)
                {
                    case PlatformType.DestructibleLeftFloat:
                    case PlatformType.DestructibleMidFloat:
                    case PlatformType.DestructibleRightFloat:
                    case PlatformType.DestructibleLeftTop:
                    case PlatformType.DestructibleMidTop:
                    case PlatformType.DestructibleRightTop:
                    case PlatformType.DestructibleLeftMid:
                    case PlatformType.DestructibleMidMid:
                    case PlatformType.DestructibleRightMid:
                        return true;
                    default:
                        return false;
                }
            }
        }

        /// <summary>
        /// Determines whether this platform is one-way
        /// </summary>
        public bool OneWay
        {
            get
            {
                switch (platformType)
                {
                    case PlatformType.OneWayLeft:
                    case PlatformType.OneWayMid:
                    case PlatformType.OneWayRight:
                        return true;
                    default:
                        return false;
                }
            }
        }

        /// <summary>
        /// Determines whether this platform is lava
        /// </summary>
        public bool Lava
        {
            get
            {
                if(platformType == PlatformType.Lava)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }

        //---Constructors---//

        /// <summary>
        /// Create a Platform with a bounding box, texture, and type
        /// </summary>
        /// <param name="x">X-coordinate (Top-Left)</param>
        /// <param name="y">Y-coordinate (Top-Left)</param>
        /// <param name="width">Platform's width</param>
        /// <param name="height">Platform's height</param>
        /// <param name="texture">Texture to be drawn with</param>
        /// <param name="type">Type of platform</param>
        public Platform(int x, int y, int width, int height, Texture2D texture, PlatformType type) :
           base(x, y, width, height, texture)
        {
            platformType = type;

            ApplyTint(Color.Red);
        }

        /// <summary>
        /// Name: Troy Corrington
        /// This is a temporary constructor that will set the platform to a
        /// certain size. Currenlty only uses mid platform sprite
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="texture"></param>
        /// <param name="type"></param>
        public Platform(int x, int y, Texture2D texture, PlatformType type) :
            base(x, y, WIDTH + 1, HEIGHT + 1, texture)
        {
            platformType = type;

            AssignSprite();

            // Reassign the static animation source
            Animation staticAnim = animations[AnimationType.Static];
            staticAnim.source = source;

            animations[AnimationType.Static] = staticAnim;
            currentAnimation = animations[AnimationType.Static];
        }


        /// <summary>
        /// Sets the source position depemding on the platform type
        /// </summary>
        private void AssignSprite()
        {
            switch(platformType)
            {
                /* Regular Platforms */

                // Regular Platform Float
                case PlatformType.RegularLeftFloat:
                    source.X = 0;
                    source.Y = 66;
                    break;

                case PlatformType.RegularMidFloat:
                    source.X = 33;
                    source.Y = 66;
                    break;

                case PlatformType.RegularRightFloat:
                    source.X = 66;
                    source.Y = 66;
                    break;

                //Regular Platform Top
                case PlatformType.RegularLeftTop:
                    source.X = 0;
                    source.Y = 99;
                    break;

                case PlatformType.RegularMidTop:
                    source.X = 33;
                    source.Y = 99;
                    break;

                case PlatformType.RegularRightTop:
                    source.X = 66;
                    source.Y = 99;
                    break;

                //Regular Platform Middle
                case PlatformType.RegularLeftMid:
                    source.X = 0;
                    source.Y = 132;
                    break;

                case PlatformType.RegularMidMid:
                    source.X = 33;
                    source.Y = 132;
                    break;

                case PlatformType.RegularRightMid:
                    source.X = 66;
                    source.Y = 132;
                    break;

                /* Lava Platforms */
                // Lava
                case PlatformType.Lava:
                    source.X = 99;
                    source.Y = 66;
                    break;

                /* Destructible Platforms */

                // Destructible Platform Float
                case PlatformType.DestructibleLeftFloat:
                    source.X = 0;
                    source.Y = 165;
                    break;

                case PlatformType.DestructibleMidFloat:
                    source.X = 33;
                    source.Y = 165;
                    break;

                case PlatformType.DestructibleRightFloat:
                    source.X = 66;
                    source.Y = 165;
                    break;

                // Destructible Platform Top
                case PlatformType.DestructibleLeftTop:
                    source.X = 0;
                    source.Y = 198;
                    break;

                case PlatformType.DestructibleMidTop:
                    source.X = 33;
                    source.Y = 198;
                    break;

                case PlatformType.DestructibleRightTop:
                    source.X = 66;
                    source.Y = 198;
                    break;

                // Destructible Platform Middle
                case PlatformType.DestructibleLeftMid:
                    source.X = 0;
                    source.Y = 231;
                    break;

                case PlatformType.DestructibleMidMid:
                    source.X = 33;
                    source.Y = 231;
                    break;

                case PlatformType.DestructibleRightMid:
                    source.X = 66;
                    source.Y = 231;
                    break;

                /* One Way Platforms */

                // One Way Platform
                case PlatformType.OneWayLeft:
                    source.X = 0;
                    source.Y = 264;
                    break;

                case PlatformType.OneWayMid:
                    source.X = 33;
                    source.Y = 264;
                    break;

                case PlatformType.OneWayRight:
                    source.X = 66;
                    source.Y = 264;
                    break;

                default:
                    break;
            }
        }
    }
}
