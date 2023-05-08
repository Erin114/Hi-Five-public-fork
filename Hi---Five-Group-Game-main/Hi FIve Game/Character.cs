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
 * Class that contains the necessary fields, properties and methods
 * for moveable GameObjects like Player and Enemy
 */

// Patrick Doherty
// 04/02/21
// Adding fields & implementing reset

namespace Hi_FIve_Game
{
    public enum CharacterState
    {
        Idle,   // Used to check if the player is still
        Walk,   // Used for default movement
        Jump,   // Handles jumping
        Slam,   // Differentiates between falling & slamming (only used by player atm)
        Land,   // Primarily used for animation
        Damaged,// Used for testing invincibility
        Shoot,  // Shooting animations
        FallThrough
    }

    /// <summary>
    /// Base class for player and enemy
    /// </summary>
    abstract class Character : GameObject
    {
        //---Character Fields---//

        // Attributes
        protected int maxHp;

        protected int health;
        protected int prevHealth;

        protected bool isFacingRight; // Used for animation
        protected SpriteEffects flip;

        protected float jumpStrength;

        protected CharacterState state;
        protected CharacterState prevState;

        // Physics
        protected float friction;
        protected float accelConstant;
        protected float speed;
        protected float slamSpeed;
        protected float gravity;
        protected float deltaTime;

        // Guns
        protected Gun equippedGun;

        // Invincibility after getting hit //
        protected int invicibilityTimer;
        protected int invincibilityFrames;

        // Sound effect storage
        protected List<SoundEffect> soundEffects;

        //---Properties---//

        /// <summary>
        /// Get the maximum health the player can have
        /// </summary>
        public int MaxHealth
        {
            get
            {
                return maxHp;
            }
        }

        /// <summary>
        /// Get the Player's current health
        /// </summary>
        public int Health
        {
            get
            {
                return health;
            }
            set
            {
                health = value;
            }
        }

        /// <summary>
        /// Get the change in time between this frame and the previous
        /// </summary>
        public float DeltaTime
        {
            get
            {
                return deltaTime;
            }
        }

        /// <summary>
        /// Get or set the current state of the player
        /// </summary>
        public CharacterState State
        {
            get
            {
                return state;
            }

            set
            {
                state = value;
            }
        }

        public bool Invincible
        {
            get
            {
                return invicibilityTimer > 0;
            }
        }

        //---Constructors---//

        /// <summary>
        /// Create a GameObject with a bounding box and texture
        /// </summary>
        /// <param name="x">X-coordinate (Top-Left)</param>
        /// <param name="y">Y-coordinate (Top-Left)</param>
        /// <param name="width">Object's width</param>
        /// <param name="height">Object's height</param>
        /// <param name="texture">Texture to be drawn with</param>
        /// <param name="hp">Character's max hit points</param>
        public Character(int x, int y, int width, int height, 
            Texture2D texture, int texX, int texY, int hp, List<SoundEffect> sounds) :
            base(x, y, width, height, texture, texX, texY)
        {
            // Set attributes
            maxHp = hp;
            health = hp;
            prevHealth = health;
            flip = SpriteEffects.None;
            isFacingRight = true;
            jumpStrength = 0.95f; // arbitrary
            state = CharacterState.Idle;
            prevState = state;

            // Set physics
            speed = 0.35f;     // arbitrary
            accelConstant = 0.002f;
            slamSpeed = 5 * speed;
            gravity = 0.0025f; // arbitrary
            deltaTime = 0;

            // Guns
            equippedGun = default;

            // No movement at the beginning
            velocity = default;

            // Invincibility timer
            invincibilityFrames = 100;
            invicibilityTimer = 0;

            // Sound Effects storage
            soundEffects = sounds;

            // Assign Collision events
            OnCollision += async (sen, obj, ove) =>
            {
                await Task.Run(() => CollisionResolver.CharacterPlatform(sen, obj, ove));
            };
        }

        /// <summary>
        /// Creates a Character with a source texture at (0, 0)
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <param name="texture"></param>
        /// <param name="hp"></param>
        public Character(int x, int y, int width, int height, Texture2D texture, int hp, List<SoundEffect> sounds)
            : this(x, y, width, height, texture, 0, 0, hp, sounds)
        {
            // Handled by overload
        }

        //---Abstract Methods---//

        /// <summary>
        /// Makes the character jump based on its jumpStrength
        /// </summary>
        public abstract void Jump();

        //---Virtual Methods---//

        /// <summary>
        /// Move the Character by velocity with gravity as well
        /// if it is currently active
        /// </summary>
        public virtual void Update(GameTime gameTime)
        {
            // If the Character dies, Destroy them
            if (health <= 0)
            {
                // Play death sound effect
                Destroy();
            }

            if (isActive)
            {
                // Set change in time betweeen frames
                deltaTime = (float)gameTime.ElapsedGameTime.TotalMilliseconds;

                // Save previous position
                prevPosition = position;

                // Decrement timer if it is active
                if (invicibilityTimer > 0)
                {
                    invicibilityTimer--;

                }

                // Apply the Character's states and movement
                CharacterStateMachine();

                // Limits the velocity vector in the X direction
                if (velocity.X * velocity.X > speed * speed)
                {
                    velocity.X = speed * MathF.Sign(velocity.X);
                }

                // Limits the falling velocity
                if (velocity.Y > slamSpeed)
                {
                    velocity.Y = slamSpeed;
                }

                // Sets the velocity to 0 if it is small enough
                if (velocity.X < 0.01f && velocity.X > -0.01f)
                {
                    velocity.X = 0;
                }

                // Calculate friction
                friction = velocity.X * accelConstant * 2;

                // Only apply friciton when player is not moving
                if (acceleration.X == 0)
                {
                    acceleration.X = -friction;
                }

                // Sets Y acceleration to gravity
                ApplyGravity();

                // Update velocity and position
                velocity += acceleration * deltaTime;
                Position += velocity * deltaTime;

                // Reset acceleration to stop endless travel
                acceleration = Vector2.Zero;

                // Flip the sprite based on aiming
                if (State == CharacterState.Shoot)
                {
                    if (rotation < -MathF.PI / 2 || rotation > MathF.PI / 2)
                    {
                        flip = SpriteEffects.FlipVertically;
                    } else
                    {
                        flip = SpriteEffects.None;
                    }
                }

                // Flip sprite based on velocity when not shooting
                else
                {
                    // Make sure the sprite isn't upside down
                    if (prevState == CharacterState.Shoot)
                    {
                        flip = SpriteEffects.None;
                    }

                    if (velocity.X > 0)
                    {
                        flip = SpriteEffects.None;
                    } else if (velocity.X < 0)
                    {
                        flip = SpriteEffects.FlipHorizontally;
                    }
                }

                // Used for animation
                cycleCount++;

                // Save the previous collision state
                prevCollision = isColliding;

                // Reset collision bool
                isColliding = false;

                // Save the previous Character State
                prevState = state;

                // Know when helth has changed
                prevHealth = health;
            }
        }

        /// <summary>
        /// Draws the character and flips there sprite 
        /// depending on which direction they're facing
        /// </summary>
        public override void Draw(SpriteBatch sb)
        {
            if (isActive)
            {
                // Determine animation
                AnimateCharacter();

                // Invincibility flash after getting hit
                int flash = invicibilityTimer / 5;

                if (flash % 4 != 3)
                {
                    sb.Draw(
                    sprite,
                    new Rectangle(bounds.X + bounds.Width / 2, bounds.Y + bounds.Height / 2, bounds.Width, bounds.Height),
                    currentAnimation.source,
                    tint,
                    rotation,
                    new Vector2(SOURCE_WIDTH / 2, SOURCE_HEIGHT / 2),
                    flip,
                    0f
                    );
                }
            }
        }

        /// <summary>
        /// State machine to control the player
        /// </summary>
        public virtual void CharacterStateMachine()
        {
            // Switch to idle if state is walk or land
            if (State == CharacterState.Walk || State == CharacterState.Land)
            {
                State = CharacterState.Idle;
            }

            // If the Character was not colliding before and is now colliding,
            // then the character has landed
            if (!prevCollision && isColliding)
            {
                State = CharacterState.Land;
            }

            // While the character is in the air, set their state to Jump
            // unless they are slamming or falling through a one-way platform
            if ((!Colliding && State != CharacterState.Slam && State != CharacterState.FallThrough) ||
                (Colliding && velocity.Y != 0))
            {
                State = CharacterState.Jump;

                // Make sure the slam state persists
                if (velocity.Y >= slamSpeed)
                {
                    State = CharacterState.Slam;
                }
            }

            // Apply Character Movement
            CharacterMovement();

            // Send the Character into the damaged state if it loses health
            if (prevHealth > Health)
            {
                State = CharacterState.Damaged;
            }

            // If the animation cannot be interrupted, make sure it persists
            if (!currentAnimation.interruptible && cycleCount != 0)
            {
                State = prevState;
            }
        }

        /// <summary>
        /// Used in the State Machine to control the character's movement
        /// and animations
        /// </summary>
        protected abstract void CharacterMovement();

        /// <summary>
        /// Sets the Character's X velocity to negative speed
        /// </summary>
        protected virtual void MoveLeft()
        {
            acceleration.X = -accelConstant;

            // Only animate the walk if it's Idle or Walking
            if (State == CharacterState.Idle || State == CharacterState.Walk)
            {
                State = CharacterState.Walk;
            }
        }

        /// <summary>
        /// Sets the Character's X velocity to speed
        /// </summary>
        protected virtual void MoveRight()
        {
            acceleration.X = accelConstant;

            // Only animate the walk if it's Idle or Walking
            if (State == CharacterState.Idle || State == CharacterState.Walk)
            {
                State = CharacterState.Walk;
            }
        }

        /// <summary>
        /// Determines the sprite to draw based on the Character's state
        /// </summary>
        protected void AnimateCharacter()
        {
            // Animate once the previous animation frame has completed
            if (cycleCount % currentAnimation.cyclesPerFrame == 0)
            {
                switch (State)
                {
                    case CharacterState.Idle:
                        // Play the idle animation
                        LoopAnimation(AnimationType.Idle);
                        break;

                    case CharacterState.Walk:
                        // Play the walking animation
                        LoopAnimation(AnimationType.Walk);
                        break;

                    case CharacterState.Jump:
                        // Play the jumping animation
                        PlayAnimation(AnimationType.Jump);

                        // Reset if over
                        if (currentAnimation.currentFrame >= currentAnimation.frameCount)
                        {
                            cycleCount = 0;
                            currentAnimation.currentFrame = 0;

                            animations[AnimationType.Jump] = currentAnimation;

                            State = CharacterState.Idle;
                        }
                        break;

                    case CharacterState.Slam:
                        // Play the slamming animation
                        PlayAnimation(AnimationType.Slam);

                        // Reset if over
                        if (currentAnimation.currentFrame >= currentAnimation.frameCount)
                        {
                            cycleCount = 0;
                            currentAnimation.currentFrame = 0;

                            animations[AnimationType.Slam] = currentAnimation;

                            State = CharacterState.Idle;
                        }
                        break;

                    case CharacterState.Land:
                        // Play the landing animation
                        PlayAnimation(AnimationType.Land);

                        // Reset if over
                        if (currentAnimation.currentFrame >= currentAnimation.frameCount)
                        {
                            cycleCount = 0;
                            currentAnimation.currentFrame = 0;

                            animations[AnimationType.Land] = currentAnimation;

                            State = CharacterState.Idle;
                        }
                        break;

                    case CharacterState.Damaged:
                        // Play the damage animation
                        PlayAnimation(AnimationType.Damage);

                        // Reset if over
                        if (currentAnimation.currentFrame >= currentAnimation.frameCount)
                        {
                            cycleCount = 0;
                            currentAnimation.currentFrame = 0;

                            animations[AnimationType.Damage] = currentAnimation;

                            State = CharacterState.Idle;
                        }
                        break;

                    case CharacterState.Shoot:
                        // Play the shooting animation
                        PlayAnimation(AnimationType.Shoot);

                        // Rotate based on shoot direction if the gun exists
                        if (equippedGun != default)
                        {
                            rotation = MathF.Atan2(equippedGun.Aim.Y, equippedGun.Aim.X);
                        }

                        // Reset if over
                        if (currentAnimation.currentFrame >= currentAnimation.frameCount)
                        {
                            cycleCount = 0;
                            currentAnimation.currentFrame = 0;

                            animations[AnimationType.Shoot] = currentAnimation;

                            State = CharacterState.Idle;
                        }
                        break;

                    case CharacterState.FallThrough:
                        // Show the Jumping Animation as a "Fall" animation
                        PlayAnimation(AnimationType.Jump);

                        // Reset if over
                        if (currentAnimation.currentFrame >= currentAnimation.frameCount)
                        {
                            cycleCount = 0;
                            currentAnimation.currentFrame = 0;

                            animations[AnimationType.Jump] = currentAnimation;
                        }
                        break;
                }

                // No rotation when not shooting
                if (State != CharacterState.Shoot)
                {
                    rotation = 0f;
                }
            }
        }

        /// <summary>
        /// An overload for sprites that require a
        /// tint
        /// </summary>
        public void Draw(SpriteBatch sb, Color tint)
        {
            if (isActive)
            {
                // Invincibility flash after getting hit
                int flash = invicibilityTimer / 5;

                if (flash % 4 != 3)
                {
                    sb.Draw(
                    sprite,
                    new Rectangle(bounds.X + bounds.Width / 2, bounds.Y + bounds.Height / 2, bounds.Width, bounds.Height),
                    currentAnimation.source,
                    tint,
                    rotation,
                    new Vector2(SOURCE_WIDTH / 2, SOURCE_HEIGHT / 2),
                    flip,
                    0f
                    );
                }
            }
        }

        //---Common Methods---//
        // Used amongst all characters //

        /// <summary>
        /// Apply gravity onto the Character so that it falls
        /// unless it is currently on top of a plaform
        /// </summary>
        public virtual void ApplyGravity()
        {
            acceleration.Y = gravity;
        }

        /// <summary>
        /// Decrease the Character's health by damage
        /// </summary>
        /// <param name="damage">Amount of damage dealt to this character</param>
        public virtual void TakeDamage(int damage)
        {

            health -= damage;

            invicibilityTimer = invincibilityFrames;

            state = CharacterState.Damaged;
        }

        public override void Reset()
        {
            // Reset Health
            health = maxHp;

            // Facing Right
            isFacingRight = true;

            // Reset state
            state = CharacterState.Idle;

            base.Reset();
        }

        /// <summary>
        /// Plays a sound effect, turns it into an instance
        /// in order to play multiple sound effects at once
        /// </summary>
        /// <param name="sound"></param>
        public void PlaySoundEffects(SoundEffect sound)
        {
            var instance = sound.CreateInstance();
            instance.Volume = 0.1f;
            instance.IsLooped = false;
            instance.Play();
        }
    }
}
