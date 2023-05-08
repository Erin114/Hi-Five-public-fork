using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

using System;
using System.Collections.Generic;
using System.Text;

/*
 * Owen Wicker
 * 3/16/21
 * Base class that contains all the necessary field and properties
 * that all objects in the game will have
 */

namespace Hi_FIve_Game
{
    /// <summary>
    /// Allows easy storage of different Animations
    /// </summary>
    public enum AnimationType
    {
        Static,
        Idle,
        Walk,
        Slam,
        Jump,
        Land,
        Damage,
        Shoot
    }

    /// <summary>
    /// Stores different animations for gameObjects
    /// </summary>
    struct Animation
    {
        // Texture setting
        public Rectangle source;

        // Animation Control
        public int frameCount;
        public int currentFrame;
        public int cyclesPerFrame;
        public bool interruptible;
    }

    /// <summary>
    /// Resolves collisions in which the sender checks for obj
    /// with the rectangle that describes the overlap of their bounds
    /// </summary>
    /// <param name="sender">GameObject that is checing</param>
    /// <param name="obj">GameObject being checked</param>
    /// <param name="overlap">Rectangle of intersection</param>
    delegate void ResolverDelegate(GameObject sender, GameObject obj, Rectangle overlap);

    /// <summary>
    /// Deactivates (destroys) the GameObject
    /// </summary>
    /// <param name="sender">The destroyed Game Object</param>
    delegate void DestructionDelegate();

    /// <summary>
    /// Base class for Collectible, Character, Platform, and Bullet
    /// </summary>
    class GameObject
    {
        //---Fields---//
        public const int WIDTH = 50;
        public const int HEIGHT = 50;

        public const int SOURCE_WIDTH = 32;
        public const int SOURCE_HEIGHT = 32;

        protected Texture2D sprite;
        protected Animation currentAnimation;
        protected Dictionary<AnimationType, Animation> animations;
        protected int cycleCount;

        protected Rectangle source;
        protected Rectangle bounds;
        protected Rectangle hitbox;

        protected Vector2 position;
        protected Vector2 prevPosition;
        protected Vector2 velocity;
        protected Vector2 acceleration;
        protected Vector2 spawnLocation;
        protected Vector2 boundsOffset;

        protected float rotation;

        protected bool isActive;
        protected bool isColliding;
        protected bool prevCollision;

        protected Color tint;

        public event ResolverDelegate OnCollision;
        public event DestructionDelegate OnDestruction;

        //---Properties---//

        /// <summary>
        /// Get or set the rectangle containing this GameObject
        /// </summary>
        public Rectangle Bounds
        {
            get
            {
                return bounds;
            }
        }

        public Rectangle Hitbox
        {
            get
            {
                return hitbox;
            }

            set
            {
                hitbox = value;
            }
        }

        /// <summary>
        /// Get the object's local center
        /// </summary>
        public Vector2 LocalCenter
        {
            get
            {
                return Vector2.Transform(Center, Camera.Transform);
            }
        }

        /// <summary>
        /// Get the GameObject's center as a 2D Vector
        /// </summary>
        public Vector2 Center
        {
            get
            {
                return new Vector2(X + Width / 2, Y + Height / 2);
            }
        }

        /// <summary>
        /// Get the object's local position
        /// </summary>
        public Vector2 LocalPosition
        {
            get
            {
               return Vector2.Transform(position, Camera.Transform);
            }
        }

        /// <summary>
        /// Get the Object's position in from the previous frame
        /// </summary>
        public Vector2 PreviousPosition
        {
            get
            {
                return prevPosition;
            }
        }

        /// <summary>
        /// Get or set the GameObject's position
        /// </summary>
        public Vector2 Position
        {
            get
            {
                return position;
            }

            set
            {
                position = value;

                // Automatically update the object's bounds when the position changes
                hitbox.X = (int)position.X;
                hitbox.Y = (int)position.Y;

                // Update the hitbox
                bounds.X = (int)(position.X + boundsOffset.X);
                bounds.Y = (int)(position.Y + boundsOffset.Y);
            }
        }

        /// <summary>
        /// Get or set the GameObject's X position (Top-Left)
        /// </summary>
        public float X
        {
            get
            {
                return position.X;
            }
            
            set
            {
                position.X = value;
                hitbox.X = (int)value;
                bounds.X = (int)(value + boundsOffset.X);
            }
        }

        /// <summary>
        /// Get or set the GameObject's Y position (Top-Left)
        /// </summary>
        public float Y
        {
            get
            {
                return position.Y;
            }
       
            set
            {
                position.Y = value;
                hitbox.Y = (int)value;
                bounds.Y = (int)(value + boundsOffset.Y);
            }
        }

        /// <summary>
        /// Get or set the GameObject's bound's width
        /// </summary>
        public int Width
        {
            get
            {
                return hitbox.Width;
            }
        }

        /// <summary>
        /// Get or set the GameObject's bound's height
        /// </summary>
        public int Height
        {
            get
            {
                return hitbox.Height;
            }
        }

        /// <summary>
        /// Get or set the magnitude and direction of the GameObject's velocity
        /// </summary>
        public Vector2 Velocity
        {
            get
            {
                return velocity;
            }

            set
            {
                velocity = value;
            }
        }

        /// <summary>
        /// Get or set the magnitude and direction of the GameObject's acceleration
        /// </summary>
        public Vector2 Acceleration
        {
            get
            {
                return acceleration;
            }

            set
            {
                acceleration = value;
            }
        }

        /// <summary>
        /// Get or set whether this GameObject is currently active
        /// </summary>
        public bool Active
        {
            get
            {
                return isActive;
            }

            set
            {
                isActive = value;
            }
        }

        /// <summary>
        /// Get whether this GameObject is currently colliding
        /// </summary>
        public bool Colliding
        {
            get
            {
                return isColliding;
            }
        }

        //---Constructors---//

        /// <summary>
        /// Most customizable way to create a GameObject:
        /// Able to determine sprite source location and size and drawing bounding box
        /// </summary>
        /// <param name="x">X-coordinate (Top-Left)</param>
        /// <param name="y">Y-coordinate (Top-Left)</param>
        /// <param name="width">Object's collision width</param>
        /// <param name="height">Object's collision height</param>
        /// <param name="texture">Texture to be drawn with</param>
        /// <param name="sourceX">The x position of the sprite</param>
        /// <param name="sourceY">The y position of the sprite</param>
        /// <param name="sourceWidth">Width of the sprite</param>
        /// <param name="sourceHeight">Height of the sprite</param>
        /// <param name="drawWidth">Width of drawn object</param>
        /// <param name="drawHeight">Height of drawn object</param>
        public GameObject(int x, int y, int width, int height, 
            Texture2D texture, int sourceX, int sourceY, int sourceWidth, int sourceHeight,
            int drawWidth, int drawHeight)
        {
            // Create the bounding & source rectangles and pass in GameObject's texture
            source = new Rectangle(sourceX, sourceY, sourceWidth, sourceHeight);
            hitbox = new Rectangle(x, y, width, height);
            bounds = new Rectangle(
                (int)(x + width / 2 - drawWidth / 2),
                (int)(y + height / 2 - drawHeight / 2), 
                drawWidth, 
                drawHeight
                );

            // Set a default static animation
            Animation staticAnim = new Animation();
            staticAnim.source = source;
            staticAnim.cyclesPerFrame = 1;
            staticAnim.frameCount = 1;
            staticAnim.currentFrame = 0;

            // Create vectors
            position = new Vector2(x, y);
            prevPosition = position;
            velocity = Vector2.Zero;

            rotation = 0f;

            // Offset between drawing bounds and collision hitbox
            boundsOffset = new Vector2(
                bounds.X - hitbox.X,
                bounds.Y - hitbox.Y
                );

            // Animations / Textures
            sprite = texture;
            animations = new Dictionary<AnimationType, Animation>();
            animations.Add(AnimationType.Static, staticAnim);

            currentAnimation = animations[AnimationType.Static];

            cycleCount = 0;

            // Automatically activated
            isActive = true;

            // Default color
            tint = Color.White;

            // Setting the Spawn Vector for Reset
            spawnLocation = new Vector2(x, y);
        }

        /// <summary>
        /// Able to determine sprite source location and size
        /// </summary>
        /// <param name="x">X-coordinate (Top-Left)</param>
        /// <param name="y">Y-coordinate (Top-Left)</param>
        /// <param name="width">Object's width</param>
        /// <param name="height">Object's height</param>
        /// <param name="texture">Texture to be drawn with</param>
        /// <param name="sourceX">The x position of the sprite</param>
        /// <param name="sourceY">The y position of the sprite</param>
        /// <param name="sourceWidth">Width of the sprite</param>
        /// <param name="sourceHeight">Height of the sprite</param>
        public GameObject(int x, int y, int width, int height,
            Texture2D texture, int sourceX, int sourceY, int sourceWidth, int sourceHeight)
            : this(x, y, width, height, texture, sourceX, sourceY, sourceWidth, sourceHeight, width, height)
        {
            // Handled by overload
        }

        /// <summary>
        /// Name: Troy Corrington
        /// Temporary overload constructor to use only a single
        /// sprite sheet
        /// </summary>
        /// <param name="x">X-coordinate (Top-Left)</param>
        /// <param name="y">Y-coordinate (Top-Left)</param>
        /// <param name="width">Object's width</param>
        /// <param name="height">Object's height</param>
        /// <param name="texture">Texture to be drawn with</param>
        /// <param name="sourceX">The x position of the sprite</param>
        /// <param name="sourceY">The y position of the sprite</param>
        public GameObject(int x, int y, int width, int height,
        Texture2D texture, int sourceX, int sourceY)
        : this(x, y, width, height, texture, sourceX, sourceY, SOURCE_WIDTH, SOURCE_HEIGHT)
        {
            // Handled by overload
        }

        /// <summary>
        /// Create a GameObject with a bounding box and texture
        /// </summary>
        /// <param name="x">X-coordinate (Top-Left)</param>
        /// <param name="y">Y-coordinate (Top-Left)</param>
        /// <param name="width">Object's width</param>
        /// <param name="height">Object's height</param>
        /// <param name="texture">Texture to be drawn with</param>
        public GameObject(int x, int y, int width, int height, Texture2D texture)
            : this(x, y, width, height, texture, 0, 0)
        {
            // Handled by overload
        }

        /// <summary>
        /// Create a GameObject using a Vector2 for its initial position
        /// </summary>
        /// <param name="pos">X and Y location of the Game Object</param>
        /// <param name="width">Width of the Game Object</param>
        /// <param name="height">Height of the Game Object</param>
        /// <param name="texture">Texture to draw with</param>
        /// <param name="texX">X position of the source texture</param>
        /// <param name="texY">Y position of the source texture</param>
        public GameObject(Vector2 pos, int width, int height, Texture2D texture, int texX, int texY)
            : this((int)pos.X, (int)pos.Y, width, height, texture, texX, texY)
        {
            // Handled by overload
        }

        /// <summary>
        /// Create a GameObject using a Vector2 for its initial position
        /// and a constant WIDTH and HEIGHT
        /// </summary>
        /// <param name="pos">X and Y location of the Game Object</param>
        /// <param name="width">Width of the Game Object</param>
        /// <param name="height">Height of the Game Object</param>
        /// <param name="texture"></param>
        /// <param name="texX"></param>
        /// <param name="texY"></param>
        public GameObject(Vector2 pos, Texture2D texture, int texX, int texY)
            : this((int)pos.X, (int)pos.Y, WIDTH, HEIGHT, texture, texX, texY)
        {
            // Handled by overload
        }

        //---Virtual Methods---//

        /// <summary>
        /// Draws the GameObject if it is currently active
        /// </summary>
        /// <param name="sb">SpriteBatch that is drawing</param>
        public virtual void Draw(SpriteBatch sb)
        {
            if (isActive)
            {
                sb.Draw(
                    sprite, // The texture to draw
                    bounds, // The Character's actual position
                    currentAnimation.source, 
                    tint
                    );
            }
        }

        /// <summary>
        /// Adds an animation to the dictionary of animations that this GameOject has
        /// </summary>
        /// <param name="type"></param>
        /// <param name="anim"></param>
        public void AddAnimation(AnimationType type, Animation anim)
        {
            if (animations.ContainsKey(type))
            {
                animations[type] = anim;
            } else
            {
                animations.Add(type, anim);
            }
        }

        /// <summary>
        /// Plays the passed in animation one time
        /// </summary>
        /// <param name="type">The animation to play</param>
        public void PlayAnimation(AnimationType type)
        {
            // Make sure that the Animation exists in the dicitionary
            try
            {
                // Grab the animation to be played
                currentAnimation = animations[type];
            } catch {
                return;
            }

            // Modify the frame being drawn
            currentAnimation.currentFrame++;

            // Reset the current frame if it's too big
            if (currentAnimation.currentFrame >= currentAnimation.frameCount)
            {
                // Reassign to dictionary
                animations[type] = currentAnimation;
                return;
            }

            // Set the source texture position based on the current frame
            currentAnimation.source.X = currentAnimation.currentFrame * (SOURCE_WIDTH + 1);

            // Reassign to dictionary
            animations[type] = currentAnimation;
        }

        /// <summary>
        /// Plays the passed in animation on a loop
        /// </summary>
        /// <param name="type">The animation to play</param>
        public void LoopAnimation(AnimationType type)
        {
            // Make sure that the Animation exists in the dicitionary
            try
            {
                // Grab the animation to be played
                currentAnimation = animations[type];
            } catch
            {
                return;
            }

            // Modify the frame being drawn
            currentAnimation.currentFrame++;

            // Reset the current frame if it's too big
            if (currentAnimation.currentFrame >= currentAnimation.frameCount)
            {
                cycleCount = 0;
                currentAnimation.currentFrame = 0;
            }

            // Set the source texture position based on the current frame
            currentAnimation.source.X = currentAnimation.currentFrame * (SOURCE_WIDTH + 1);

            // Reassign to dictionary
            animations[type] = currentAnimation;
        }

        /// <summary>
        /// Change the color of the object
        /// when it is drawn
        /// </summary>
        /// <param name="c">The tint to apply</param>
        public void ApplyTint(Color c)
        {
            tint = c;
        }

        /// <summary>
        /// Checks if this gameObject and another are colliding with each other.
        /// Triggers the OnCollision event if they are
        /// </summary>
        /// <param name="obj">GameObject to check</param>
        public virtual void CheckCollisions(GameObject obj)
        {
            // Make sure the objects are active
            if (this.Active && obj.Active)
            {
                // Make sure both objects have a valid Hitbox
                if (this.Hitbox != default(Rectangle) && obj.Hitbox != default(Rectangle))
                {

                    // Determine if the two objects collide
                    if (Hitbox.Intersects(obj.Hitbox) && OnCollision != null)
                    {
                        // Update collision bool
                        if (!(obj is Trigger))
                        {
                            isColliding = true;
                        }

                        obj.isColliding = true;

                        // Trigger collision event
                        OnCollision(this, obj, Rectangle.Intersect(Hitbox, obj.Hitbox));

                        return;
                    }
                }
            }
        }

        /// <summary>
        /// Destroys this Game Object by deactivating it
        /// and calling the OnDestruction event
        /// </summary>
        public void Destroy()
        {
            // Make sure the object is actually active
            if (this.Active)
            {
                // Deactivate
                this.Active = false;

                // Call Destruction event
                if (OnDestruction != null)
                {
                    OnDestruction();
                }

                // Send it to a galaxy far, far away
                // But after calling the event so it's new
                // position doesn't corrupt the Jedi order
                Position = new Vector2(-100, -100);
            }
        }

        /// <summary>
        /// Resets the objects values
        /// includes start location
        /// & activating it
        /// </summary>
        public virtual void Reset()
        {
            // Reset the objects locations
            X = spawnLocation.X;
            Y = spawnLocation.Y;

            // Reset velocity
            velocity = Vector2.Zero;

            // Make sure it's active
            isActive = true;
        }

        /// <summary>
        /// Combines this object's hitbox with another game object's hitbox
        /// in order to cut down on collision checking.
        /// </summary>
        /// <param name="gameObj">GameObject to combine with</param>
        public void CombineHitbox(GameObject gameObj) 
        {
            // Combine the two hitboxes
            Hitbox = Rectangle.Union(Hitbox, gameObj.Hitbox);

            // Change the other object's hitbox to be "empty"
            gameObj.Hitbox = default(Rectangle);
        }
    }
}
