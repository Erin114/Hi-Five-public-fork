using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Audio;
using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Text;

/*
 * Owen Wicker
 * 3/16/21
 * User-controlled character that will be the main protagonist
 */

// Patrick Doherty
// 4/02/21
// Adding Player States & Resets

namespace Hi_FIve_Game
{
    /// <summary>
    /// User-controlled object that can move and shoot
    /// </summary>
    class Player : Character
    {

        //---Fields-----------------------------------------------//

        // Used for player movement
        private KeyboardState kbState;
        private KeyboardState previousKbState;

        // Keeps track of the active on-screen bullets
        private List<Bullet> playerBullets;

        // Internal inventory
        private Dictionary<string, int> ammunition;
        private List<Gun> playerGuns;

        // Where the player is aiming
        private MouseState mState;
        private MouseState previousMState;

        // God mode
        private bool isGod;


        //---Properties--------------------------------------------//

        /// <summary>
        /// Get or set the Player's currently equipped gun
        /// </summary>
        public Gun Equipped
        {
            get
            {
                return equippedGun;
            }

            set
            {
                equippedGun = value;
            }
        }

        /// <summary>
        /// Get or set the amount of ammo of the given GunType
        /// </summary>
        /// <param name="index">The name of the gun</param>
        /// <returns>Amount of ammo for the selected gun</returns>
        public int this[string index]
        {
            get
            {
                return ammunition[index];
            }

            set
            {
                ammunition[index] = value;
            }
        }

        /// <summary>
        /// Get the list of active player bullets
        /// </summary>
        public List<Bullet> ActiveBullets
        {
            get
            {
                return playerBullets;
            }

            set
            {
                playerBullets = value;
            }
        }

        /// <summary>
        /// Get whether the player is in the invincibility state after being hit
        /// </summary>
        public new bool Invincible
        {
            get
            {
                return base.Invincible || GodMode;
            }
        }

        /// <summary>
        /// Get or set if the player is in God mode
        /// </summary>
        public bool GodMode
        {
            get
            {
                return isGod;
            }

            set
            {
                isGod = value;
            }
        }

        //---Constructors-------------------------------------------//

        /// <summary>
        /// Create a GameObject with a bounding box and texture
        /// </summary>
        /// <param name="x">X-coordinate (Top-Left)</param>
        /// <param name="y">Y-coordinate (Top-Left)</param>
        /// <param name="width">Object's width</param>
        /// <param name="height">Object's height</param>
        /// <param name="texture">Texture to be drawn with</param>
        /// <param name="hp">Character's max hit points</param>
        public Player(int x, int y, int width, int height,
            Texture2D texture, int hp, List<SoundEffect>soundEffects) :
            base(x, y, WIDTH, HEIGHT, texture, 0, 0, hp, soundEffects)
        {
            // Create bullet lists
            playerBullets = new List<Bullet>();
            playerGuns = new List<Gun>();
            ammunition = new Dictionary<string, int>();

            // Make the bullet & gun types //

            //// End of Bullet Fields
            ///
            // Game will be played normally at first
            isGod = false;

            // Assign Collision events
            OnCollision += async (sen, obj, ove) =>
            {
                await Task.Run(() => CollisionResolver.PlayerEnemy(sen, obj, ove));
            };

            OnCollision += async (sen, obj, ove) =>
            {
                await Task.Run(() => CollisionResolver.PlayerCollectible(sen, obj, ove));
            };
        }

        //---Override Methods----------------------------------------------------//

        /// <summary>
        /// Moves the character and shoots based on user input
        /// </summary>
        public override void Update(GameTime gameTime)
        {
            // Prevent the player from endless travelling
            if (isGod)
            {
                acceleration = Vector2.Zero;
                velocity = Vector2.Zero;
            }

            // Update the player if it is alive/active
            if (isActive)
            {
                SwitchGuns();   // Different guns

                // Apply transformations and gravity
                base.Update(gameTime);

                InBounds();     // Constraints

                // Update guns so that they follow the player
                equippedGun.AimAt(new Vector2(mState.Position.X, mState.Position.Y));
                equippedGun.Position = Center;
                equippedGun.Update();
            }

            // Add in explosions
            // Do it before deleting inactive bullets
            // so that they can be grabbed
            for (int i = playerBullets.Count - 1; i > -1; i--)
            {
                // Easier access
                Bullet b = playerBullets[i];

                // Make sure the explosion exists
                if (b.Explosion != null)
                {
                    PlaySoundEffects(soundEffects[4]);
                    playerBullets.Add(b.Explosion);
                }
            }

            // Loop through each active bullet and update it
            // Go from back to front because the oldest bullets will likely
            // be inactive first
            for (int i = playerBullets.Count - 1; i > -1; i--)
            {
                // Just so it looks cleaner
                Bullet b = playerBullets[i];

                // Update and check if they are off the screen
                if (!b.Active)
                {
                    // Remove the bullet it if it is inactive
                    playerBullets.Remove(b);
                } else
                {
                    // Update the bullet and deactivate if necessary
                    b.Update();
                }
            }

            // Set the previous state for the SingleKeyPress and SingleMousePress method
            previousKbState = kbState;
            previousMState = mState;
        }

        /// <summary>
        /// Draws the player as well any bullets they shot
        /// </summary>
        /// <param name="sb">Spritebatch that draws</param>
        public override void Draw(SpriteBatch sb)
        {
            // Draw each gun that is active
            equippedGun.Draw(sb);

            // Draw each bullet
            foreach (Bullet b in playerBullets)
            {
                b.Draw(sb);

                // Draw explosion if it exists
                if (b.Explosion != null)
                {
                    b.Explosion.Draw(sb);
                }
            }

            // Draw Player
            if (isGod)
            {
                base.Draw(sb, Color.PapayaWhip);
            } else
            {
                base.Draw(sb);
            }
        }

        /// <summary>
        /// Applies gravity on the player
        /// </summary>
        public override void ApplyGravity()
        {
            // Only gravitate when playing in normal mode
            if (!GodMode)
            {
                base.ApplyGravity();
            }
        }

        /// <summary>
        /// Resets the player values including reseting guns & the player state
        /// </summary>
        public override void Reset()
        {
            // Equip the pistol
            equippedGun = playerGuns[0];

            // Restock Ammo
            this["Shotgun"] = 10;             // Values may Change in the future ============
            this["Rocket"] = 10;

            // Set Player State
            state = CharacterState.Idle;

            base.Reset();
        }

        //---Player Actions--------------------------------------------------------------

        /// <summary>
        /// Move the Player using keyboard input
        /// </summary>
        protected override void CharacterMovement()
        {
            // Walk
            if (kbState.IsKeyDown(Keys.A))
            {
                MoveLeft();
            }

            if (kbState.IsKeyDown(Keys.D))
            {
                MoveRight();
            }

            // Jump
            if (kbState.IsKeyDown(Keys.W))
            {
                // Jump in normal play
                if (!GodMode)
                {
                    if (SingleKeyPress(Keys.W))
                    {
                        Jump();
                    }
                } else
                {
                    velocity.Y = -speed;
                }
            }

            // Shoot
            if (SingleMousePress(mState.LeftButton, previousMState.LeftButton))
            {
                Shoot();
            }

            // Slam & Fall Through
            if (kbState.IsKeyDown(Keys.S))
            {
                // Slam and Fall Through in normal play
                if (!GodMode)
                {
                    if (SingleKeyPress(Keys.S))
                    {
                        Slam();
                    } else if (State != CharacterState.Slam)
                    {
                        State = CharacterState.FallThrough;
                    }
                } else
                {
                    velocity.Y = speed;
                }
            } else if (kbState.IsKeyUp(Keys.S) && State == CharacterState.FallThrough && !Colliding)
            {
                State = CharacterState.Idle;
            }
        }

        /// <summary>
        /// Accelerates the Player to the left. In God Mode, this method will
        /// adjust the velocity instead of the acceleration
        /// </summary>
        protected override void MoveLeft()
        {
            // Move normally out of god mode
            if (!GodMode)
            {
                base.MoveLeft();
            } else
            {
                velocity.X = -speed;
            }
        }

        /// <summary>
        /// Accelerates the Player to the right. In God Mode, this method will
        /// adjust the velocity instead of the acceleration
        /// </summary>
        protected override void MoveRight()
        {
            // Move normally out of god mode
            if (!GodMode)
            {
                base.MoveRight();
            } else
            {
                velocity.X = speed;
            }
        }


        /// <summary>
        /// Makes the character jump when 'W' is pressed
        /// if they are on top of a platform
        /// </summary>
        public override void Jump()
        {
            // Make the player jump if it's walking or idle or landing
            if (state != CharacterState.Jump && state != CharacterState.Slam)
            {
                velocity.Y = -jumpStrength;
                state = CharacterState.Jump;
                PlaySoundEffects(soundEffects[0]);
            }

            // God mode flying
            if (isGod && kbState.IsKeyDown(Keys.W))
            {
                velocity.Y = -speed;
            }
        }

        /// <summary>
        /// Decreases the health of the Player when not in God Mode
        /// or in an invincible state. If hit, the player becomes invincible
        /// for a set amount of frames
        /// </summary>
        /// <param name="damage">Amoount health to decrease by</param>
        public override void TakeDamage(int damage)
        {
            // Shouldn't run if the player is invincible anyways
            // but it's good to check just in case
            if (!isGod && !Invincible)
            {
                base.TakeDamage(damage);
            }
        }

        /// <summary>
        /// Changes the Player's equipped gun when the RMB is clicked
        /// </summary>
        private void SwitchGuns()
        {
            // Right click cycle
            if (SingleMousePress(mState.RightButton, previousMState.RightButton))
            {
                // Get which gun is selected as an index
                int index = playerGuns.IndexOf(equippedGun);

                // Check that incrementing won't go out of bounds
                if (index + 1 == playerGuns.Count)
                {
                    equippedGun = playerGuns[0];
                } else
                {
                    equippedGun = playerGuns[index + 1];
                }
            } 
            // Scroll wheel cycle
            else if (mState.ScrollWheelValue != previousMState.ScrollWheelValue)
            {
                // Get which gun is selected as an index
                int index = playerGuns.IndexOf(equippedGun);

                int change = -(int)MathF.Sign(mState.ScrollWheelValue - previousMState.ScrollWheelValue);

                // Check that in/de- crementing won't go out of bounds
                if (index + change == playerGuns.Count )
                {
                    equippedGun = playerGuns[0];
                } else if (index + change < 0)
                {
                    equippedGun = playerGuns[playerGuns.Count - 1];
                } else
                {
                    equippedGun = playerGuns[index + change];
                }
            } 
            // Cycle with number keys
            else
            {
                // Check number keys for each gun up to 10 guns
                for (int i = 0; i < Math.Min(playerGuns.Count, 10); i++)
                {
                    if (SingleKeyPress((Keys)((int)Keys.D1 + i)))
                    {
                        equippedGun = playerGuns[i];
                    }
                }
            }
            
        }

        /// <summary>
        /// Shoots a bullet from the currently selected gun if there is ammo left
        /// </summary>
        private void Shoot()
        {
            // Decrease ammo if god mode is off
            if (!isGod && ammunition[equippedGun.Name] > 0)
            {
                // Shoot the currently equipped gun
                ShootEquippedGun();
            } else if (isGod)
            {
                // Shoot the gun without decreasing or checking for ammo
                // while God mode
                ShootEquippedGun();
            }
        }

        /// <summary>
        /// Call the currently equipped gun's shoot method
        /// and apply recoil onto the player
        /// </summary>
        private void ShootEquippedGun()
        {   
            // Only try to shoot when the gun can fire
            if (equippedGun.Timer == 0)
            {
                if (!(equippedGun is Pistol))
                {
                    // Decrease ammo stored in dictionary
                    ammunition[equippedGun.Name]--;
                }

                // Play the appropriate sound effect for each bullet
                if (equippedGun.Name == "Rocket")
                {
                    PlaySoundEffects(soundEffects[3]);
                } else
                {
                    PlaySoundEffects(soundEffects[2]);
                }

                // Create the correct bullet and fire it
                // Shoot a bullet depending on the type
                playerBullets.AddRange(equippedGun.Shoot());

                // Change state for animations
                state = CharacterState.Shoot;

                // Prevents the player from floating when shooting
                // guns without recoil
                if (equippedGun.Recoil != Vector2.Zero)
                {
                    velocity = equippedGun.Recoil;
                }
            }
        }

        /// <summary>
        /// Slams the ground and damages enemies
        /// </summary>
        private void Slam()
        {
            // Slam if player is jumping
            if (State == CharacterState.Jump)
            {
                // Slam the ground
                velocity.Y = slamSpeed;

                PlaySoundEffects(soundEffects[1]);

                State = CharacterState.Slam;
            }
        }

        //---Helper Methods-------------------------------------------//

        // Input Methods

        /// <summary>
        /// Sets which keys are pressed to be used for checking movement and shooting
        /// </summary>
        /// <param name="kbState">Current state of the keyboard</param>
        public void SetKeyboardState(KeyboardState kbState)
        {
            this.kbState = kbState;
        }

        /// <summary>
        /// Sets which mouse buttons are pressed and its location
        /// </summary>
        /// <param name="mousePosition">Current state of the mouse</param>
        public void SetMouseState(MouseState mState)
        {
            this.mState = mState;
        }

        // Troy's Method //

        /// <summary>
        /// Checks if a single key was just pressed this frame
        /// </summary>
        /// <param name="key">The key being checked</param>
        /// <returns>True if the key has just been pressed, false otherwise</returns>
        private bool SingleKeyPress(Keys key)
        {
            if (kbState.IsKeyDown(key) && !previousKbState.IsKeyDown(key))
            {
                return true;
            } else
            {
                return false;
            }
        }

        /// <summary>
        /// Adds a gun that player can shoot
        /// </summary>
        /// <param name="gun"></param>
        public void AddGun(Gun gun)
        {
            playerGuns.Add(gun);

            // If the added gun is the first one
            if (playerGuns.Count == 1)
            {
                // then set it as the default
                equippedGun = playerGuns[0];
            }
        }

        /// <summary>
        /// Sets the ammunition for the specified gun
        /// </summary>
        /// <param name="gun"></param>
        /// <param name="amount"></param>
        public void SetAmmo(Gun gun, int amount)
        {
            // Add ammo
            if (ammunition.ContainsKey(gun.Name))
            {
                ammunition[gun.Name] = amount;
            } else
            {
                ammunition.Add(gun.Name, amount);
            }
        }

        // Variation on Troy's Method

        /// <summary>
        /// Checks if a single key was just pressed this frame
        /// </summary>
        /// <param name="key">The key being checked</param>
        /// <returns>True if the key has just been pressed, false otherwise</returns>
        private bool SingleMousePress(ButtonState currentButton, ButtonState previousButton)
        {
            if (currentButton == ButtonState.Pressed && previousButton != ButtonState.Pressed)
            {
                return true;
            } else
            {
                return false;
            }
        }

        // Helper  Bounds Method
        /// <summary>
        /// Checks if the player would move out of bounds & sets them so they are in bounds
        /// </summary>
        /// <param name="height">The Height of the screen</param>
        /// <param name="width"> The Width of the screen</param>
        private void InBounds()
        {
            // Check if the player is in X bounds
            if (X < 0)
            {
                X = 0;
            }
            else if(X > Camera.LevelBounds.Width - Width)
            {
                // Sets the player's X so they are completely on screen
                X = Camera.LevelBounds.Width - Width;
            }
            

            // Check if the player is in Y bounds
            if (Y < -8)
            {
                // -8 because the player sprite is bigger 
                // than it looks with blank spaces
                Y = -8;

                // Set the  Yvelocity to 0 so the player 
                //starts to fall instead of slowly sliding
                velocity.Y = 0;
            }
            else if (Y > 680)
            {
                // The Player Dies & loses the level
                health = 0;
            }
        }


        // Helper Global Movement Method
        /// <summary>
        /// Based off of Owen's Movement Method
        /// This just does the math without moving the player 
        /// It's used to update the global variables
        /// </summary>
        /// <returns>The amount moved</returns>
        public int GlobalMovement()
        {
            // Check which keys are pressed and change the velocity accordingly

            // Left
            if (kbState.IsKeyDown(Keys.A))
            {
                return (int)-speed;
            }
            // Right
            else if (kbState.IsKeyDown(Keys.D))
            {
                return (int)speed;
            }
            // Didn't move this frame
            else
            {
                return 0;
            }
        }
    }
}
