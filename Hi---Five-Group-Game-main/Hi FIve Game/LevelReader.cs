using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Audio;

namespace Hi_FIve_Game
{
    // Name: Troy Corringotn
    // Date: 4/17/21
    // Purpose: To read in levels and put game assets
    // into the game world
    class LevelReader
    {
        // Fields ---------------------------------------------------
        BinaryReader reader;
        FileStream inStream;

        Player player;
        EnemyManager enemyManager;
        CollectibleManager collectManager;
        PlatformManager platManager;

        Texture2D spriteSheet;
        Texture2D chargerSprite;
        Texture2D shooterSprite;

        Gun enemyGun;

        List<SoundEffect> soundEffects;

        // Properties -----------------------------------------------


        // Constructors ---------------------------------------------

        public LevelReader(
            EnemyManager enemyManager, CollectibleManager collectManager,
            PlatformManager platManager, Player player, Texture2D itemTexture,
            Texture2D chargerTexture, Texture2D shooterTexture, 
            Gun enemyGun, List<SoundEffect> soundEffects)
        {
            spriteSheet = itemTexture;
            chargerSprite = chargerTexture;
            shooterSprite = shooterTexture;
            this.enemyGun = enemyGun;
            this.player = player;
            this.enemyManager = enemyManager;
            this.collectManager = collectManager;
            this.platManager = platManager;
            this.soundEffects = soundEffects;
        }

        // Methods --------------------------------------------------

        public void LoadGame(string levelDirect)
        {
            try
            {
                // Load in the level through the parameter
                inStream = File.OpenRead(levelDirect);
                reader = new BinaryReader(inStream);

                // Determine the amount of screens
                int numberOfScreens = reader.ReadInt32();

                // Adjust camera to fit within screens
                Camera.LevelBounds = new Rectangle(0, 0, numberOfScreens * (15 * GameObject.WIDTH), Camera.Height);

                // For loop through the level data, placing objects
                // in their position depending on the current state of the
                // for loop
                // Nonsense value for now...

                for (int currScreen = 0; currScreen < numberOfScreens; currScreen++)
                {
                    // Coordinates of objects loop
                    for (int x = 0; x < 15; x++)
                    {
                        for (int y = 0; y < 15; y++)
                        {
                            // A point to determine the position
                            // of the object
                            // Note: 750 is the max amount of pixels a screen will go
                            // as objects will be at a standard size of 50x50 with
                            // 15 objects per screen (screens in regard to the level editor)
                            Point objectPosition =
                                new Point(
                                    GameObject.WIDTH * x + 750 * currScreen, // The x position
                                    GameObject.HEIGHT * y // The y position
                                    );

                            // Finally, we create the object, the object being created
                            // will depend on the image used in the level editor

                            // *Insert that stuff here*
                            // Maybe use a switch statement..?
                            int objectType = reader.ReadInt32();

                            // Empty space
                            if (objectType == 0)
                            {
                                continue;
                            }
                            // Player
                            if (objectType == 1)
                            {
                                player.X = objectPosition.X;
                                player.Y = objectPosition.Y;
                            }
                            // Charger
                            else if (objectType == 2)
                            {
                                enemyManager.Objects.Add(new Charger
                                    (
                                    objectPosition.X, objectPosition.Y, 
                                    50, 50, 
                                    chargerSprite, 3, 250, soundEffects));
                            }
                            // Shooter
                            else if (objectType == 3)
                            {
                                enemyManager.Objects.Add(new Shooter
                                    (
                                    objectPosition.X, objectPosition.Y, 
                                    50, 50, 
                                    shooterSprite, 3, 500, enemyGun.Clone(), soundEffects));
                            }
                            // Collectibles
                            else if (objectType >= 4 && objectType <= 7)
                            {
                                // Create Collectible Helper Method like the platforms
                                collectManager.Objects.Add(new Collectible(
                                    objectPosition.X,
                                    objectPosition.Y,
                                    spriteSheet,
                                    (CollectibleType)(objectType - 4)
                                    ));
                            }
                            // Platforms
                            else
                            {
                                PlatformForming(objectPosition.X, objectPosition.Y, objectType);
                            }


                        }
                    }
                }
            }
            finally
            {
                // Close the file
                if (reader != null)
                {
                    reader.Close();

                    // Combine platforms
                    CombineRegularPlatforms();
                }
            }
        }


        private void PlatformForming(int xPos, int yPos, int platType)
        {
            platManager.Objects.Add(new Platform(
                xPos, yPos,
                spriteSheet,
                (PlatformType)Enum.ToObject(typeof(PlatformType), platType - 8)
                ));

        }

        /// <summary>
        /// Combines platforms that are next to each other vertically.
        /// This prevents wall jumping / fall stutters.
        /// 
        /// No version for destructible platforms because that would result in
        /// incorrect destructions.
        /// </summary>
        private void CombineRegularPlatforms()
        {
            /* Someone find a better way to do this please */

            // Compare each platform against all others
            foreach (Platform start in platManager.Objects)
            {
                // Go into the check loop if start is a regular platform
                // with a valid hitbox
                if (start.Regular && !start.Hitbox.IsEmpty)
                {
                    // Get the platform to combine with
                    foreach (Platform combine in platManager.Objects)
                    {
                        // Don't try to combine with itself
                        if (start != combine)
                        {
                            // Only combine with regular platforms with valid hitboxes
                            if (combine.Regular && !combine.Hitbox.IsEmpty)
                            {
                                // Make sure the platform is in the right position
                                if (combine.X == start.X &&
                                    (combine.Y == start.Y + start.Height - 1 || combine.Y == start.Y - start.Height + 1))
                                {
                                    // Actually combine them
                                    start.CombineHitbox(combine);
                                }
                            }
                        }
                    }
                }
            }
        }
    }
}
