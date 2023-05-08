using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

using System;
using System.Collections.Generic;
using System.Text;

namespace Hi_FIve_Game
{
    static class CollisionResolver
    {
        //---Properties------------------------------------------------------------------

        /// <summary>
        /// Get the delegate that resolves collisions between Characters and Platforms
        /// </summary>
        public static ResolverDelegate CharacterPlatform
        {
            get
            {
                return ResolveCharacterPlatformCollisions;
            }
        }

        /// <summary>
        /// Get the delegate that resolves collisions between Bullets and Platforms
        /// </summary>
        public static ResolverDelegate BulletPlatform
        {
            get
            {
                return ResolveBulletPlatformCollisions;
            }
        }

        /// <summary>
        /// Get the delegate that resolves collisions between Players and Enemies
        /// </summary>
        public static ResolverDelegate PlayerEnemy
        {
            get
            {
                return ResolvePlayerEnemyCollision;
            }
        }

        /// <summary>
        /// Get the delegate that resolves collisions between Bullets and Enemies
        /// </summary>
        public static ResolverDelegate BulletEnemy
        {
            get
            {
                return ResolveBulletEnemyCollision;
            }
        }

        /// <summary>
        /// Get the delegate that resolves collisions between Bullets and Players
        /// </summary>
        public static ResolverDelegate BulletPlayer
        {
            get
            {
                return ResolveBulletPlayerCollision;
            }
        }

        /// <summary>
        /// Get the delegate that resolves collisions between Collectibles and Platforms
        /// </summary>
        public static ResolverDelegate CollectiblePlatform
        {
            get
            {
                return ResolveCollectiblePlatfomCollision;
            }
        }

        /// <summary>
        /// Get the delegate that resolves collisions between Players and Collectibles
        /// </summary>
        public static ResolverDelegate PlayerCollectible
        {
            get
            {
                return ResolvePlayerCollectibleCollision;
            }
        }

        /// <summary>
        /// Get the delgeate that resolves collisions between Players and Triggers
        /// </summary>
        public static ResolverDelegate PlayerTrigger
        {
            get
            {
                return ResolvePlayerTriggerCollision;
            }
        }

        //---Collision Resolvers---------------------------------------------------------

        /// <summary>
        /// Handles the collisions between Characters and Platforms.
        /// Breaks out of method if the sender is not a character or
        /// if the obj is not a platform
        /// </summary>
        /// <param name="sender">Character that is colliding</param>
        /// <param name="obj">Platform being collided with</param>
        /// <param name="overlap">Rectangle of intersection</param>dddddd
        private static void ResolveCharacterPlatformCollisions(GameObject sender, GameObject obj, Rectangle overlap)
        {
            // Assigning this way makes use of C# pattern-matching
            // If the sender is a Character object, it will assign
            // it to the variable character which can be used by anything
            // in the scope of this method

            // Check that the sender is a Character object
            if (!(sender is Character character))
            {
                // If they're not a character then break out
                return;
            }

            // Check that the obj is a Platform object
            if (!(obj is Platform platform))
            {
                // If they're not a platform then break out
                return;
            }

            // Resolve the collision //
            Rectangle characterRect = character.Hitbox;

            // No matter what, set Acceleration to 0
            character.Acceleration = Vector2.Zero;

            // Deactivate the character if they touch a lava platform
            if (platform.Lava)
            {
                // Determine if it is an enemy or player
                if (character is Player player)
                {
                    // Check if they are in god mode
                    if (!player.GodMode)
                    {
                        player.Destroy();

                        return;
                    }
                } else
                {
                    character.Destroy();

                    return;
                }
            }

            // Collide differently for one way platforms
            if (platform.OneWay)
            {
                // Don't collide whn trying to fall through
                if (character.State == CharacterState.FallThrough)
                {
                    return;
                }

                // Players don't collide with one way platform
                if (character is Player play)
                {
                    if (play.GodMode)
                    {
                        return;
                    }
                }

                if (overlap.Width > overlap.Height)
                {

                    // Collide with the top if the character is fully above
                    // the platform and falling onto it
                    if (characterRect.Y + characterRect.Height < platform.Y + platform.Height / 3
                        && character.Velocity.Y > 0)
                    {
                        // Set Y velocity to 0 so the player can still move
                        //character.Acceleration = new Vector2(character.Acceleration.X, 0);

                        // -1 prevents constant falling by placing them in the platform
                        // so that a collision will always be detected and IsOnTop will
                        // evalutate to true
                        characterRect.Y -= overlap.Height - 1;

                        character.Velocity = new Vector2(character.Velocity.X, 0);
                    }
                }
            } else
            {

                // Determine how to move the player //
                // Move left/right if width <= height
                if (overlap.Width <= overlap.Height)
                {
                    // Set X velocity to 0 so player can still fall
                    character.Acceleration = new Vector2(0, character.Acceleration.Y);

                    // Determine left or right by checking the player's
                    // x position against the object's center
                    if (characterRect.X > platform.X + platform.Width / 2)
                    {
                        characterRect.X += overlap.Width;
                    } else
                    {
                        characterRect.X -= overlap.Width;
                    }
                }

                if (overlap.Width > overlap.Height)
                {
                    // Set Y velocity to 0 so the player can still move
                    character.Velocity = new Vector2(character.Velocity.X, 0);

                    // Determine up or down by checking the player's
                    // y position against the object's center
                    if (characterRect.Y > platform.Y + platform.Height / 2)
                    {
                        // +1 prevents getting stuck in the bottom of the platform
                        // by placing them out of the platform so that the player can fall
                        characterRect.Y += overlap.Height + 1;
                    } else if (character.State == CharacterState.Slam || characterRect.Y < platform.Y + platform.Height / 2)
                    {
                        // -1 prevents constant falling by placing them in the platform
                        // so that a collision will always be detected and IsOnTop will
                        // evalutate to true
                        characterRect.Y -= overlap.Height - 1;
                    }
                }
            }

            // Update position so that the character doesn't jitter
            character.X = characterRect.X;
            character.Y = characterRect.Y;
        }

        /// <summary>
        /// Deactivates bullets when they collide with platforms.
        /// Breaks out of method if the sender is not a bullet or
        /// if the obj is not a platform
        /// </summary>
        /// <param name="sender">Bullet that is colliding</param>
        /// <param name="obj">Platform being collided with</param>
        /// <param name="overlap">Rectangle of intersection</param>
        private static void ResolveBulletPlatformCollisions(GameObject sender, GameObject obj, Rectangle overlap)
        {
            // Check that the sender is a Bullet object
            if (!(sender is Bullet bullet))
            {
                return;
            } 

            // Check that the obj is a Platform object
            if (obj is Platform platform)
            {
                // If it is a bullet colliding with a platform,
                // then deactivate it
                if (bullet.FiredFrom != "Explode" && !platform.OneWay)
                {
                    bullet.Destroy();
                }

            } else
            {
                // If it's not a platform then break out
                return;
            }

            // Have that explosion destroy platforms
            if (bullet.FiredFrom == "Explode")
            {
                if (platform.Destructible) 
                {
                    platform.Destroy();
                }
            }
        }

        /// <summary>
        /// Stops collectibles from falling through floors or walls
        /// </summary>
        /// <param name="sender">Collectible</param>
        /// <param name="obj">Platform</param>
        /// <param name="overlap">Rectangle containing the overlapping region of Collectible and Platform</param>
        private static void ResolveCollectiblePlatfomCollision(GameObject sender, GameObject obj, Rectangle overlap)
        {
            // Check that the sender is a Character object
            if (!(sender is Collectible collectible))
            {
                // If they're not a collectible then break out
                return;
            }

            // Check that the obj is a Platform object
            if (!(obj is Platform platform))
            {
                // If they're not a character then break out
                return;
            }

            // Resolve the collision //

            Rectangle collectibleRect = collectible.Hitbox;

            // Determine how to move the player //
            // Move left/right if width <= height
            if (overlap.Width <= overlap.Height)
            {
                // Set X velocity to 0 so playter can still fall
                collectible.Velocity = new Vector2(0, collectible.Velocity.Y);

                // Determine left or right by checking the player's
                // x position against the object's center
                if (collectibleRect.X > platform.X + platform.Width / 2)
                {
                    collectibleRect.X += overlap.Width;
                } else
                {
                    collectibleRect.X -= overlap.Width;
                }
            }

            if (overlap.Width > overlap.Height)
            {
                // Set Y velocity to 0 so the player can still move
                collectible.Velocity = new Vector2(collectible.Velocity.X, 0);

                // Determine up or down by checking the player's
                // y position against the object's center
                if (collectibleRect.Y > platform.Y + platform.Height / 2)
                {
                    // +1 prevents getting stuck in the bottom of the platform
                    // by placing them out of the platform so that the player can fall
                    collectibleRect.Y += overlap.Height + 1;
                } else
                {
                    // -1 prevents constant falling by placing them in the platform
                    // so that a collision will always be detected and IsOnTop will
                    // evalutate to true
                    collectibleRect.Y -= overlap.Height - 1;
                }
            }

            // Update position so that the character doesn't jitter
            collectible.X = collectibleRect.X;
            collectible.Y = collectibleRect.Y;
        }

        /// <summary>
        /// Player takes damage when colliding and boht the Player and Enemy
        /// are pushed away from each other. Breaks out of method when sender
        /// is not a Player or obj is not an Enemy
        /// </summary>
        /// <param name="sender">Player that is colliding</param>
        /// <param name="obj">Enemy being collided with</param>
        /// <param name="overlap">Rectangle of intersection</param>
        private static void ResolvePlayerEnemyCollision(GameObject sender, GameObject obj, Rectangle overlap)
        {
            // Check that the sender is a Player object
            if (!(sender is Player play) || play.GodMode ||
                (play.Invincible && play.State != CharacterState.Slam))
            {
                // If they're not a character then break out
                return;
            }

            // Check that the obj is a Platform object
            if (!(obj is Enemy enemy))
            {
                // If they're not a character then break out
                return;
            }

            // Resolve the collision //

            // Affects the player velocity
            // Changes when slamming
            Vector2 playerRecoil = Vector2.Zero;

            // Get the rectangles containg the player nad enemy
            Rectangle playerRect = play.Hitbox;
            Rectangle enemyRect = enemy.Hitbox;

            // Damage the enemy if the player collides while slamming
            if (play.State == CharacterState.Slam)
            {
                // damage the enemy from a slam
                enemy.TakeDamage(1);

                // "Recoil" the player so they don't instantly take damage
                playerRecoil = new Vector2(play.Velocity.X, -0.5f);
            } else
            {
                if (!play.Invincible)
                {
                    playerRecoil = new Vector2(enemy.Velocity.X * 5, -0.5f);

                    // Damage the player if they aren't slamming
                    play.TakeDamage(1);
                }
            }

            // Move the player out of the enemy to prevent
            // damage after slamming
            
            // Determine how to move the player //
            // Move left/right if width <= height
            if (overlap.Width <= overlap.Height)
            {
                // Set X velocity to 0 so player can still fall
                play.Velocity = new Vector2(0, play.Velocity.Y);
                enemy.Velocity = new Vector2(0, enemy.Velocity.Y);

                // Determine left or right by checking the player's
                // x position against the object's center
                // Move them outwards and make the player invincible
                // to create a separation so
                // there aren't infintie collisions
                if (playerRect.X > enemy.X + enemy.Width / 2)
                {
                    playerRect.X += overlap.Width;
                    enemyRect.X -= overlap.Width;
                } else
                {
                    playerRect.X -= overlap.Width;
                    enemyRect.X += overlap.Width;
                }
            }

            if (overlap.Width > overlap.Height)
            {
                // Set Y velocity to 0 so the player can still move
                play.Velocity = new Vector2(play.Velocity.X, 0);
                enemy.Velocity = new Vector2(enemy.Velocity.X, 0);

                // Determine up or down by checking the player's
                // y position against the object's center
                if (playerRect.Y > enemy.Y + enemy.Height / 2)
                {
                    playerRect.Y += overlap.Height;
                    enemyRect.Y -= overlap.Height;
                } else
                {
                    playerRect.Y -= overlap.Height;
                    enemyRect.Y += overlap.Height;
                }
            }

            // Change the velocity
            // Only actually affects it when slamming
            play.Velocity = playerRecoil;

            // Update position so that the characters don't jitter
            play.X = playerRect.X;
            play.Y = playerRect.Y;

            enemy.X = enemyRect.X;
            enemy.Y = enemyRect.Y;
        }

        /// <summary>
        /// Mkaes the player "Collect" the collectible when they colldie with it
        /// </summary>
        /// <param name="sender">Player</param>
        /// <param name="obj">Collectible</param>
        /// <param name="overlap">Rectangle of intersection</param>
        private static void ResolvePlayerCollectibleCollision(GameObject sender, GameObject obj, Rectangle overlap)
        {
            // Check that the sender is a Player object
            if (!(sender is Player play))
            {
                // If they're not a character then break out
                return;
            }

            // Check that the obj is a Collectible object
            if (!(obj is Collectible collectible))
            {
                // If they're not a collectible then break out
                return;
            }

            // Determine what to do based on collectible type
            switch (collectible.Type)
            {
                case CollectibleType.Health:
                    // Increase health if the player needs it
                    if (play.Health < play.MaxHealth)
                    {
                        play.Health++;
                    } else
                    {
                        // Don't pick it up if they don't need it
                        return;
                    }

                    break;
                case CollectibleType.ShotgunAmmo:
                    // Increase shotgun ammo
                    play["Shotgun"]++;
                    break;
                case CollectibleType.RocketAmmo:
                    // Increase rocket ammo
                    play["Rocket"]++;
                    break;
                case CollectibleType.ClearDoor:
                    break;
                default:
                    // Really nothing to do if the collectible doesn't have a type
                    return;
            }

            // Deactivate collectible to show the player has picked it up
            collectible.Active = false;
        }

        /// <summary>
        /// Damages enemies when a bullet hits them and deactivates the bullet
        /// </summary>
        /// <param name="sender">Bullet that is colliding</param>
        /// <param name="obj">Enemy being collided with</param>
        /// <param name="overlap">Rectangle of intersection</param>
        private static void ResolveBulletEnemyCollision(GameObject sender, GameObject obj, Rectangle overlap)
        {
            // Check that the sender is a Character object
            if (!(sender is Bullet bullet))
            {
                // If they're not a character then break out
                return;
            }
            // Check that the obj is a Platform object
            if (!(obj is Enemy enemy))
            {
                // If they're not an enemy then break out
                return;
            }

            // Deactivate bullet
            if (bullet.FiredFrom != "Explode")
            {
                bullet.Destroy();
            }

            if (bullet.FiredFrom != "Rocket")
            {
                if (bullet.FiredFrom == "Explode")
                {
                    enemy.TakeDamage(3);
                } else
                {
                    // Decrease health
                    enemy.TakeDamage(1);
                }

                // Push the enemy in the direction the bullet is travelling as a reaction
                // to getting shot

                enemy.Y -= 1; // Pop up the enemy so it can move

                if (bullet.FiredFrom != "Explode")
                {
                    enemy.Velocity = new Vector2(Math.Sign(bullet.Velocity.X) * 0.5f, Math.Sign(bullet.Velocity.Y) * 0.5f);
                } else
                {
                    enemy.Velocity = new Vector2(Math.Sign(bullet.Velocity.X), 1f);
                }
            }
        }

        /// <summary>
        /// Damages enemies when a bullet hits them and deactivates the bullet
        /// </summary>
        /// <param name="sender">Bullet that is colliding</param>
        /// <param name="obj">Enemy being collided with</param>
        /// <param name="overlap">Rectangle of intersection</param>
        private static void ResolveBulletPlayerCollision(GameObject sender, GameObject obj, Rectangle overlap)
        {
            // Check that the sender is a Character object
            if (!(sender is Bullet bullet))
            {
                // If they're not a character then break out
                return;
            }

            // Check that the obj is a Platform object
            if (!(obj is Player play))
            {
                // If they're not a player then break out
                return;
            }

            // Deactivate bullet
            if (bullet.FiredFrom != "Explode")
            {
                bullet.Destroy();
            }
            
            // React to the getting hit
            if (!play.Invincible)
            {
                // Push the player back and up as a kind of reaction to getting shot
                play.Velocity = new Vector2(Math.Sign(bullet.Velocity.X) * 10, play.Velocity.Y);

                // Decrease health
                play.TakeDamage(1);
            }
        }

        private static void ResolvePlayerTriggerCollision(GameObject sender, GameObject obj, Rectangle overlap)
        {
            if (!(sender is Player play))
            {
                return;
            }

            if (!(obj is Trigger trigger))
            {
                return;
            }

            // Activate the trigger if there is a collision
            trigger.Active = true;
        }
    }
}
