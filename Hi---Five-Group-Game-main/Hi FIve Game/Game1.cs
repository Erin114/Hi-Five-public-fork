using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using Microsoft.Xna.Framework.Audio;

using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace Hi_FIve_Game
{
    /// <summary>
    /// Enumerators that will keep track
    /// of the game state that the player
    /// is currently within
    /// </summary>
    public enum GameState
    {
        MainMenu,
        LevelSelect,
        Game,
        StageClear,
        GameOver
    }

    public class Game1 : Game
    {
        // Fields ---------------------------------------------------
        private GraphicsDeviceManager _graphics;

        // Level Reading
        private LevelReader levelReader;
        string currentDir;
        string projectDir;

        private KeyboardState kbState;
        private KeyboardState previousKbState;

        private MouseState mouseState;
        private MouseState prevMouseState;

        private SpriteBatch _spriteBatch;
        //private SpriteFont lemonMilk_12;

        private float clockTime;

        // Keeps track of the current game state
        private GameState currentGameState;

        // Sprite
        Texture2D health;
        Texture2D time;
        Texture2D ammo;
        Texture2D playerSprite;
        Texture2D platformSprite;
        Texture2D enemyChargerSprite;
        Texture2D enemyShooterSprite;
        Texture2D buttonSprite;
        Texture2D uniqueScreens;

        // Mouse Sprite
        Texture2D mouse;

        // Static Sprites
        Texture2D staticSpriteSheet;

        // Texts
        private SpriteFont testText;
        private SpriteFont buttonText;

        // Test buttons for state transitions
        private Button levelSelectButton;
        private Button levelTutorialButton;
        private Button levelOneButton;
        private Button levelTwoButton;
        private Button levelThreeButton;
        private Button levelFourButton;
        private Button backToMenuButton;

        // Player and platforms for test level
        Player player;

        List<Platform> platforms;
        List<Character> enemies;
        List<Collectible> collectibles;

        Pistol pistol;
        Shotgun shotgun;
        Rocket rocket;
        EnemyPistol enemyPistol;

        Bullet pistolBullet;
        Bullet shotgunBullet;
        Bullet rocketBullet;

        // The Object Managers
        private EnemyManager enemyManager;
        private PlatformManager platformManager;
        private CollectibleManager collectibleManager;

        // Animation storage
        private Dictionary<AnimationType, Animation> animations;

        // Audio
        // Background Music
        private Song menuSong;
        private Song gameSong;

        // Sound Effects
        public List<SoundEffect> soundEffects;

        // Tutorial Triggers
        private List<Trigger> tutorialMessages;

        // Template Methods -----------------------------------------
        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;

            soundEffects = new List<SoundEffect>();
        }

        protected override void Initialize()
        {
            // Initialize Camera first since it defines the screen dimensions
            Camera.Initialize();

            // Apply screen dimensions
            _graphics.PreferredBackBufferHeight = Camera.Height;
            _graphics.PreferredBackBufferWidth = Camera.Width;
            _graphics.ApplyChanges();

            currentDir = Environment.CurrentDirectory;
            projectDir = Directory.GetParent(currentDir).Parent.Parent.Parent.FullName;

            currentGameState = GameState.MainMenu;

            previousKbState = Keyboard.GetState();
            prevMouseState = Mouse.GetState();

            clockTime = 0.00f;

            platforms = new List<Platform>();
            enemies = new List<Character>();
            collectibles = new List<Collectible>();

            animations = new Dictionary<AnimationType, Animation>();

            tutorialMessages = new List<Trigger>();

            // Volume adjustment
            MediaPlayer.Volume = 0.1f;

            // Makes sure music loops
            MediaPlayer.IsRepeating = true;

            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            // TODO: use this.Content to load your game content here
            health = Content.Load<Texture2D>("heart");
            ammo = Content.Load<Texture2D>("bullet");
            time = Content.Load<Texture2D>("clock");

            // Instruction Screen
            uniqueScreens =
                Content.Load<Texture2D>("Mano Unique Screens");

            // Mouse stuff
            // Load the Texture & make a custom Cursor
            mouse = Content.Load<Texture2D>("Reticle");
            MouseCursor reticle = MouseCursor.FromTexture2D(mouse, 24, 24);

            // Set the Cursor
            Mouse.SetCursor(reticle);

            buttonSprite = Content.Load<Texture2D>("button");
            platformSprite = Content.Load<Texture2D>("square");

            testText = Content.Load<SpriteFont>("TestFont");
            buttonText = Content.Load<SpriteFont>("smallText");

            // Static Sprite Sheet
            staticSpriteSheet =
                Content.Load<Texture2D>
                ("Static Sprites");

            // Player Sprite Sheet
            playerSprite =
                Content.Load<Texture2D>
                ("playerSprites");

            // Enemy Sprite Sheet (Currently just single sprites)
            enemyChargerSprite =
                Content.Load<Texture2D>
                ("chargerSprites");
            enemyShooterSprite =
                Content.Load<Texture2D>
                ("shooterSprites");

            // Buttons
            levelSelectButton
                = new Button(buttonSprite, buttonText, "PLAY", new Rectangle(0, 0, 115, 50));
            backToMenuButton
                = new Button(buttonSprite, buttonText, "BACK", new Rectangle(0, 0, 120, 50));
            levelTutorialButton
                = new Button(buttonSprite, buttonText, "TUTORIAL", new Rectangle(0, 0, 210, 50));
            levelOneButton
                = new Button(buttonSprite, buttonText, "LAVA ROOM", new Rectangle(0, 0, 260, 50));
            levelTwoButton
                = new Button(buttonSprite, buttonText, "SHOT GUN", new Rectangle(0, 0, 230, 50));
            levelThreeButton
                = new Button(buttonSprite, buttonText, "DOOM", new Rectangle(0, 0, 150, 50));
            levelFourButton
                = new Button(buttonSprite, buttonText, "FLY AWAY", new Rectangle(0, 0, 230, 50));

            // Player
            player = new Player(
                100,
                400,
                48,
                40,
                playerSprite,
                5,
                soundEffects
                );

            // Guns/Bullets
            CreateBullets();
            CreateGuns();

            // Add the guns and ammo
            player.AddGun(pistol.Clone());
            player.SetAmmo(pistol, 999);

            player.AddGun(shotgun.Clone());

            player.AddGun(rocket.Clone());

            // Make the Enemy Manager
            enemyManager = new EnemyManager(
                enemies,
                _graphics.PreferredBackBufferWidth,
                _graphics.PreferredBackBufferHeight,
                player);
            // Sort the things
            enemyManager.SortActive();
            enemyManager.SortQuadrant();

            // Make the platform manager
            platformManager = new PlatformManager(
                platforms,
                Camera.Width,
                Camera.Height
                );

            // Make the Collectible Manager
            collectibleManager = new CollectibleManager(
                collectibles,
                _graphics.PreferredBackBufferWidth,
                _graphics.PreferredBackBufferHeight);

            levelReader = new LevelReader(
                enemyManager, collectibleManager, platformManager,
                player, staticSpriteSheet,
                enemyChargerSprite, enemyShooterSprite,
                enemyPistol, soundEffects);

            // Audio Loading
            // Background Music Loading
            menuSong = Content.Load<Song>("Background Music\\Menu Music");
            gameSong = Content.Load<Song>("Background Music\\Level Music");

            // Sound Effects Loading
            soundEffects.Add(Content.Load<SoundEffect>("Sound Effects\\player_SFX_jump"));
            soundEffects.Add(Content.Load<SoundEffect>("Sound Effects\\player_SFX_slam"));
            soundEffects.Add(Content.Load<SoundEffect>("Sound Effects\\player_SFX_shoot"));
            soundEffects.Add(Content.Load<SoundEffect>("Sound Effects\\player_SFX_missileShoot"));
            soundEffects.Add(Content.Load<SoundEffect>("Sound Effects\\player_SFX_explosion"));
            soundEffects.Add(Content.Load<SoundEffect>("Sound Effects\\collectible_SFX_get"));
            soundEffects.Add(Content.Load<SoundEffect>("Sound Effects\\enemy_SFX_hurt"));
            soundEffects.Add(Content.Load<SoundEffect>("Sound Effects\\enemy_SFX_dead"));
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            // Gets current state of the keyboard and mouse
            kbState = Keyboard.GetState();
            mouseState = Mouse.GetState();

            // Only move the camera while in game
            if (currentGameState != GameState.Game)
            {
                Camera.Follow(Vector2.Zero);
            }

            // Switch makes sure the proper game states are
            // being updated, depending on which one the player
            // is in
            switch (currentGameState)
            {
                case GameState.MainMenu:
                    MainMenuUpdate();
                    break;

                case GameState.LevelSelect:
                    LevelSelectUpdate();
                    break;

                case GameState.Game:
                    {
                        GameUpdate(gameTime);
                        // Changes the time for the HUD
                        clockTime = clockTime + (float)gameTime.ElapsedGameTime.TotalSeconds;
                    }
                    break;

                case GameState.StageClear:
                    StageClearUpdate();
                    break;

                case GameState.GameOver:
                    GameOverUpdate(kbState);
                    break;
            }

            // Update the previous keyboard and mouse
            // state before looping back
            previousKbState = kbState;
            prevMouseState = mouseState;


            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            _spriteBatch.Begin(
                SpriteSortMode.Deferred,
                null,
                null,
                null,
                null,
                null,
                Camera.Transform
                );

            // Switch makes sure the proper objects are
            // being drawn depending on which state the
            // player is in
            switch (currentGameState)
            {
                case GameState.MainMenu:
                    MainMenuDraw(_spriteBatch);
                    break;

                case GameState.LevelSelect:
                    LevelSelectDraw(_spriteBatch);
                    break;

                case GameState.Game:
                    GameDraw(_spriteBatch);
                    break;

                case GameState.StageClear:
                    StageClearDraw(_spriteBatch);
                    break;

                case GameState.GameOver:
                    GameOverDraw(_spriteBatch);
                    break;
            }
            //_spriteBatch.DrawString(lemonMilk_12, "Patrick Doherty", position, Color.Black);


            _spriteBatch.End();

            base.Draw(gameTime);
        }

        // Custom Methods -------------------------------------------
        // Helper Methods ===========================================
        //      These methods will be used to help organize our update
        //      and draw methods

        /// <summary>
        /// Manages game updates 
        /// while on the Main Menu
        /// </summary>
        private void MainMenuUpdate()
        {
            // Place buttons
            levelSelectButton.Text = "PLAY";
            levelSelectButton.CenterX = 700;
            levelSelectButton.CenterY = 500;

            // Plays menu song if not already playing
            if (MediaPlayer.Queue.ActiveSong != menuSong)
            {
                MediaPlayer.Play(menuSong);
            }

            // Button Logic
            if (levelSelectButton.isClicked(mouseState, prevMouseState))
            {
                currentGameState = GameState.LevelSelect;
            }
        }

        /// <summary>
        /// Manages the drawing of images 
        /// while on the Main Menu
        /// </summary>
        private void MainMenuDraw(SpriteBatch sb)
        {
            sb.Draw(
                uniqueScreens,
                new Vector2(0, 0),
                new Rectangle(0, 0, 1000, 750),
                Color.White);

            levelSelectButton.Draw(sb);
        }

        /// <summary>
        /// Manages game updates 
        /// while in the Level Select
        /// </summary>
        private void LevelSelectUpdate()
        {
            // We reset the level so we can start over again
            ResetLevel();

            // Place Buttons
            // Tutorial
            levelTutorialButton.CenterX = 40;
            levelTutorialButton.CenterY = 250;

            // Level One
            levelOneButton.CenterX = 40;
            levelOneButton.CenterY = 350;

            // Level Two
            levelTwoButton.CenterX = 40;
            levelTwoButton.CenterY = 450;

            // Level Three
            levelThreeButton.CenterX = 40;
            levelThreeButton.CenterY = 550;

            // Level Three
            levelFourButton.CenterX = 40;
            levelFourButton.CenterY = 650;

            // Back to Instruction Screen
            backToMenuButton.CenterX = 850;
            backToMenuButton.CenterY = 50;


            // Button Logic
            if (levelTutorialButton.isClicked(mouseState, prevMouseState))
            {
                currentGameState = GameState.Game;
                string projectDir = Directory.GetParent(currentDir).Parent.Parent.Parent.FullName;
                player.SetAmmo(pistol.Clone(), 999);
                player.SetAmmo(shotgun.Clone(), 7);
                player.SetAmmo(rocket.Clone(), 3);
                levelReader.LoadGame(@"Content\LevelFolder\level0.level");

                CreateTutorialMessages();

                CreateAnimations();

            }
            if (levelOneButton.isClicked(mouseState, prevMouseState))
            {
                currentGameState = GameState.Game;
                string projectDir = Directory.GetParent(currentDir).Parent.Parent.Parent.FullName;
                player.SetAmmo(pistol.Clone(), 999);
                player.SetAmmo(shotgun.Clone(), 5);
                player.SetAmmo(rocket.Clone(), 2);
                levelReader.LoadGame(@"Content\LevelFolder\level1.level");

                CreateAnimations();

                tutorialMessages.Clear();
            }
            if (levelTwoButton.isClicked(mouseState, prevMouseState))
            {
                currentGameState = GameState.Game;
                string projectDir = Directory.GetParent(currentDir).Parent.Parent.Parent.FullName;
                player.SetAmmo(pistol.Clone(), 999);
                player.SetAmmo(shotgun.Clone(), 3);
                player.SetAmmo(rocket.Clone(), 1);
                levelReader.LoadGame(@"Content\LevelFolder\LevelShotGun.level");

                CreateAnimations();

                tutorialMessages.Clear();
            }
            if (levelThreeButton.isClicked(mouseState, prevMouseState))
            {
                currentGameState = GameState.Game;
                player.SetAmmo(pistol.Clone(), 0);
                player.SetAmmo(shotgun.Clone(), 60);
                player.SetAmmo(rocket.Clone(), 60);
                player.Equipped = shotgun.Clone();

                levelReader.LoadGame(@"Content\LevelFolder\Doom.level");

                CreateAnimations();

                tutorialMessages.Clear();
            }

            if (levelFourButton.isClicked(mouseState, prevMouseState))
            {
                currentGameState = GameState.Game;
                player.SetAmmo(pistol.Clone(), 999);
                player.SetAmmo(shotgun.Clone(), 20);
                player.SetAmmo(rocket.Clone(), 0);

                levelReader.LoadGame(@"Content\LevelFolder\flappyBird.level");

                CreateAnimations();

                tutorialMessages.Clear();
            }

            if (backToMenuButton.isClicked(mouseState, prevMouseState))
            {
                currentGameState = GameState.MainMenu;
            }

            // Same as Main Menu Update
            if (MediaPlayer.Queue.ActiveSong != menuSong)
            {
                MediaPlayer.Play(menuSong);
            }
        }

        /// <summary>
        /// Manages the drawing of images 
        /// while in the Level Select
        /// </summary>
        private void LevelSelectDraw(SpriteBatch sb)
        {
            sb.Draw(
                uniqueScreens,
                new Vector2(0, 0),
                new Rectangle(1001, 0, 1000, 750),
                Color.White);

            levelTutorialButton.Draw(sb);
            levelOneButton.Draw(sb);
            levelTwoButton.Draw(sb);
            levelThreeButton.Draw(sb);
            levelFourButton.Draw(sb);
            backToMenuButton.Draw(sb);
        }

        /// <summary>
        /// Manages game updates 
        /// while the game is running
        /// </summary>
        private void GameUpdate(GameTime gameTime)
        {
            // Check collisions between objects
            CheckCollisions();

            // Update enemies
            foreach (Enemy e in enemyManager.Objects)
            {
                // Let the enemy know where the player is
                e.SetPlayerCenter(
                    player.Center
                    );

                // Update their movement/AI
                e.Update(gameTime);

                // The player is "in sight" of the enemy until
                // it is changed to false in the collision checking
                if (e is Shooter s)
                {
                    s.PlayerInSight = true;
                }
            }

            // Update player
            player.Update(gameTime);


            // Move camera with player's X position
            // After update to avoid weird jittering
            Camera.Follow(new Vector2(player.Center.X, Camera.Height / 2));

            // Bound Camera on the left side
            if (Camera.Position.X < Camera.LevelBounds.X)
            {
                Camera.Follow(Vector2.Zero);
            }

            // Bound Camera on the right side
            if (Camera.Position.X + Camera.Width > Camera.LevelBounds.X + Camera.LevelBounds.Width)
            {
                Camera.Follow(new Vector2(Camera.LevelBounds.Width - Camera.Width / 2, Camera.Height / 2));
            }

            // Pass in user input to the player
            // Doing it here prevents floating bullets on level load
            player.SetKeyboardState(kbState); // Movement
            player.SetMouseState(mouseState); // Aiming

            // Check if the player is still alive
            if (!player.Active)
            {
                currentGameState = GameState.GameOver;
            }

            // Check if Clear Door is active
            foreach(Collectible collectible in collectibles)
            {
                if (collectible.Active == false && collectible.Type == CollectibleType.ClearDoor)
                {
                    currentGameState = GameState.StageClear;
                }
            }


            // Call Endframe to set up managers for the next frame
            enemyManager.EndFrame();

            // Set God mode
            if (SingleKeyPress(Keys.G))
            {
                player.GodMode = !player.GodMode ;
            }

            // Plays game music
            if (MediaPlayer.Queue.ActiveSong != gameSong)
            {
                MediaPlayer.Play(gameSong);
            }
        }

        /// <summary>
        /// Manages the drawing of images 
        /// while the game is running
        /// </summary>
        private void GameDraw(SpriteBatch sb)
        {
            /* Draw Objects before HUD */
            // Platform drawing
            foreach (Platform p in platformManager.Objects)
            {
                p.Draw(sb);
            }

            foreach (Enemy e in enemyManager.Objects)
            {
                e.Draw(sb);
            }

            foreach (Collectible c in collectibleManager.Objects)
            {
                c.Draw(sb);
            }

            foreach(Trigger t in tutorialMessages)
            {
                t.Draw(sb);
            }

            // Player drawing, also draws bullets
            player.Draw(sb);

            // Hud Draw Logic

            // Health Logic
            Point healthLocation = new Point(20, 10);
            Point healthSize = new Point(50, 50);
            Rectangle heartRect;
            for (int i = 0; i < player.Health; i++)
            {
                healthLocation.X = (i * healthSize.X) + 30;
                heartRect = new Rectangle(GlobalToLocal(healthLocation), healthSize);
                sb.Draw(health, heartRect, Color.White);
            }

            // Time Logic
            Point timeLocation = new Point(0, 5);
            timeLocation.X = (healthSize.X * 5) + 200;      //Time starts 200 pixels from the last heart
            Point timeSize = new Point(60, 60);
            Rectangle timeRect = new Rectangle(GlobalToLocal(timeLocation), timeSize);
            sb.Draw(time, timeRect, Color.White);

            // Timer text
            string clock = ($"{GetTime(clockTime)}");
            Vector2 clockPosition = new Vector2(timeRect.Right, timeRect.Height/2);
            sb.DrawString(testText, clock, clockPosition, Color.Black);

            // Ammo Logic
            Point ammoLocation = new Point(0, 5);
            ammoLocation.X =  timeRect.X + 360;
            Point ammoSize = new Point(50, 50);
            Rectangle ammoRect = new Rectangle(ammoLocation, ammoSize);
            Rectangle ammoSource = new Rectangle(0, 33, 32, 32);

            // Change source rect based on equipped gun
            if (player.Equipped is Pistol)
            {
                ammoSource.X = 0;
            } else if (player.Equipped is Shotgun)
            {
                ammoSource.X = 33;
            } else if (player.Equipped is Rocket)
            {
                ammoSource.X = 66;
            }

            sb.Draw(staticSpriteSheet, ammoRect, ammoSource, Color.White);

            //Ammo Type
            string ammoType = $"{player.Equipped.Name}";             // This will get the ammo type of the equiped gun
            Vector2 typePosition = new Vector2( ammoRect.Right + 5, ammoRect.Top);
            sb.DrawString(testText, ammoType, typePosition, Color.Black);

            // Ammo Amount
            string ammoAmount = $"x{player[player.Equipped.Name]}";          // This will get the ammo amount
            Vector2 amountPosition = new Vector2(ammoRect.Right + 5, ammoRect.Bottom - 30);
            sb.DrawString(testText, ammoAmount, amountPosition, Color.Black);
        }

        /// <summary>
        /// Manages game updates 
        /// while on the Stage Clear Screen
        /// </summary>
        private void StageClearUpdate()
        {
            levelSelectButton.Text = "BACK";

            levelSelectButton.CenterX = 450;
            levelSelectButton.CenterY = 650;

            if (levelSelectButton.isClicked(mouseState, prevMouseState))
            {
                currentGameState = GameState.LevelSelect;
            }

            // Stops music upon stage complete
            MediaPlayer.Stop();
        }

        /// <summary>
        /// Manages the drawing of images 
        /// on the Stage Clear Screen
        /// </summary>
        private void StageClearDraw(SpriteBatch sb)
        {
            sb.Draw(
                uniqueScreens,
                new Vector2(0, 0),
                new Rectangle(0, 751, 1000, 750),
                Color.White);

            levelSelectButton.Draw(sb);
        }

        /// <summary>
        /// Manages game updates 
        /// while on the Game Over Screen
        /// </summary>
        private void GameOverUpdate(KeyboardState kbState)
        {
            levelSelectButton.Text = "BACK";

            levelSelectButton.CenterX = 450;
            levelSelectButton.CenterY = 650;

            if (levelSelectButton.isClicked(mouseState, prevMouseState))
            {
                currentGameState = GameState.LevelSelect;
            }

            // Stops music upon game over
            MediaPlayer.Stop();
        }

        /// <summary>
        /// Manages the drawing of images 
        /// while on the Game Over Screen
        /// </summary>
        private void GameOverDraw(SpriteBatch sb)
        {
            sb.Draw(
                uniqueScreens,
                new Vector2(0, 0),
                new Rectangle(1001, 751, 1000, 750),
                Color.White);

            levelSelectButton.Draw(sb);
        }

        /// <summary>
        /// Checks if a single key was just pressed this frame
        /// </summary>
        /// <param name="key">The key being checked</param>
        /// <returns>True if the key has just been pressed, false otherwise</returns>
        bool SingleKeyPress(Keys key)
        {
            if (kbState.IsKeyDown(key) && !previousKbState.IsKeyDown(key))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Check collisions between all relevant objects
        /// which can trigger the GameObject's OnCollision event
        /// </summary>
        private void CheckCollisions()
        {
            // Collision check on platforms
            foreach (Platform p in platforms)
            {
                // Collide player with platforms
                player.CheckCollisions(p);

                foreach (Enemy e in enemies)
                {
                    // Collide enemy with platforms
                    e.CheckCollisions(p);

                    if (e is Shooter s)
                    {
                        if (s.Active && !p.OneWay)
                        {
                            // Get the overlap of the los recctangle and each platform
                            Rectangle losOverlap = Rectangle.Intersect(p.Hitbox, s.LineOfSight);

                            if (losOverlap.Width != 0 && losOverlap.Height != 0)
                            {
                                // Check if the overlap intersects with the midline
                                if ((s.LineOfSight.Width / losOverlap.Width) + (s.LineOfSight.Height / losOverlap.Height) > 1)
                                {
                                    s.PlayerInSight = false;
                                }
                            }
                        }

                        // Bullet to Platform Check
                        foreach (Bullet b in s.ActiveBullets)
                        {
                            b.CheckCollisions(p);
                        }
                    }
                }
                foreach (Bullet b in player.ActiveBullets)
                {
                    // Collide bullet with platforms
                    b.CheckCollisions(p);

                    // Collide rocket explosions with platforms
                    if (b.Explosion != null)
                    {
                        b.Explosion.CheckCollisions(p);
                    }
                }
            }

            // Collectibles Check
            foreach (Collectible c in collectibles)
            {
                player.CheckCollisions(c);
            }

            // Enemy Check
            foreach (Enemy e in enemies)
            {
                // Collide player with enemy
                player.CheckCollisions(e);

                foreach (Bullet b in player.ActiveBullets)
                {
                    // Collide bullet with enemy
                    b.CheckCollisions(e);

                    // Collide rocket explosions with enemies
                    if (b.Explosion != null)
                    {
                        b.Explosion.CheckCollisions(e);
                    }
                }
                if (e is Shooter s)
                {
                    if (s.Active)
                    {
                        // Shooter Bullet to Player Check
                        foreach (Bullet b in s.ActiveBullets)
                        {
                            b.CheckCollisions(player);
                        }
                    }
                }
            }

            // Tutorial Triggers
            foreach (Trigger t in tutorialMessages)
            {
                player.CheckCollisions(t);
            }
        }
        
        /// <summary>
        /// Creates triggers for a better tutorial level
        /// </summary>
        private void CreateTutorialMessages()
        {
            // Start of level - Intro to movement
            Trigger intro = new Trigger(
                0,
                0,
                Camera.Width - 200,
                Camera.Height / 2 + 100,
                testText,
                "Welcome to MANO!\n" +
                "Use 'A' or 'D' to move left or right\n" +
                "and use 'W' to jump."
                );

            // Weapons
            Trigger weapons = new Trigger(
                Camera.Width - 150,
                0,
                750,
                452,
                testText,
                "There are 3 weapons that can be cycled through using three methods:\n" +
                "Number Keys, Scrolling, or Pressing the Right Mouse Button.\n" +
                "The Shotgun and Rocket Launcher have limited ammo so try not to waste!",
                new Vector2(
                    Camera.Width - 100,
                    100
                    )
                );


            // Fall Through
            Trigger greenPlatform = new Trigger(
                1650,
                0,
                400,
                280,
                testText,
                "Press and hold 'S' while on\n" +
                "Green Platforms fo fall through",
                new Vector2(
                    1600, 252 / 2
                    )
                );

            // Chargers
            Trigger charger = new Trigger(
                1850,
                350,
                700,
                400,
                testText,
                "Red Charger enemies will chase the Player\n" +
                "when it gets too close!\n" +
                "Be careful out there!",
                new Vector2(
                    2000,
                    400
                    )
                );

            tutorialMessages.Add(intro);
            tutorialMessages.Add(weapons);
            tutorialMessages.Add(greenPlatform);
            tutorialMessages.Add(charger);
        }

        /// <summary>
        /// Creates the different types of bullets used by the player's guns
        /// </summary>
        private void CreateBullets()
        {
            pistolBullet = new Bullet(
                (int)player.Center.X - player.Width / 8,
                (int)player.Center.Y - player.Height / 8,
                GameObject.WIDTH / 2,
                GameObject.HEIGHT / 2,
                staticSpriteSheet,
                0,
                33,
                10f
                );

            shotgunBullet = new Bullet(
                (int)player.Center.X - player.Width / 8,
                (int)player.Center.Y - player.Height / 8,
                GameObject.WIDTH / 2,
                GameObject.HEIGHT / 2,
                staticSpriteSheet,
                33,
                33,
                10f
                );

            rocketBullet = new Bullet(
                (int)player.Center.X - player.Width / 8,
                (int)player.Center.Y - player.Height / 8,
                (int)(0.8f * GameObject.WIDTH),
                (int)(0.8f * GameObject.HEIGHT),
                staticSpriteSheet,
                66,
                33,
                10f
                );
        }


        /// <summary>
        /// Creates the different types of guns the player can shoot
        /// </summary>
        private void CreateGuns()
        {
            // Make the guns
            pistol = new Pistol(
                (int)player.Center.X - GameObject.WIDTH / 2,
                (int)player.Center.Y - GameObject.HEIGHT / 2,
                GameObject.WIDTH,
                GameObject.HEIGHT,
                staticSpriteSheet,
                pistolBullet
                );

            shotgun = new Shotgun(
                (int)player.Center.X - GameObject.WIDTH / 2,
                (int)player.Center.Y - GameObject.HEIGHT / 2,
                GameObject.WIDTH,
                GameObject.HEIGHT,
                staticSpriteSheet,
                shotgunBullet,
                5,
                (float)Math.PI / 12 // 15 degrees
                );

            rocket = new Rocket(
                (int)player.Center.X - GameObject.WIDTH / 2,
                (int)player.Center.Y - GameObject.HEIGHT / 2,
                GameObject.WIDTH,
                GameObject.HEIGHT,
                staticSpriteSheet,
                rocketBullet
                );

            enemyPistol = new EnemyPistol(
                (int)player.Center.X - GameObject.WIDTH / 2,
                (int)player.Center.Y - GameObject.HEIGHT / 2,
                GameObject.WIDTH,
                GameObject.HEIGHT,
                staticSpriteSheet,
                pistolBullet
                );
        }

        /// <summary>
        /// Creates animations for GameObjects
        /// </summary>
        private void CreateAnimations()
        {
            // Idle Player Animation
            Animation playerIdle = new Animation();
            playerIdle.currentFrame = 0;
            playerIdle.cyclesPerFrame = 1;
            playerIdle.frameCount = 1;
            playerIdle.source = new Rectangle(0, 0, GameObject.SOURCE_WIDTH, GameObject.SOURCE_HEIGHT);
            playerIdle.interruptible = true;

            player.AddAnimation(AnimationType.Idle, playerIdle);

            // Walk Player Animation
            Animation playerWalk = new Animation();
            playerWalk.currentFrame = 0;
            playerWalk.cyclesPerFrame = 4;
            playerWalk.frameCount = 10;
            playerWalk.source = new Rectangle(0, 33, GameObject.SOURCE_WIDTH, GameObject.SOURCE_HEIGHT);
            playerWalk.interruptible = true;

            player.AddAnimation(AnimationType.Walk, playerWalk);

            // Jump Player Animation
            Animation playerJump = new Animation();
            playerJump.currentFrame = 0;
            playerJump.cyclesPerFrame = 1;
            playerJump.frameCount = 2;
            playerJump.source = new Rectangle(0, 66, GameObject.SOURCE_WIDTH, GameObject.SOURCE_HEIGHT);
            playerJump.interruptible = true;

            player.AddAnimation(AnimationType.Jump, playerJump);

            // Slam Player Animation
            Animation playerSlam = new Animation();
            playerSlam.currentFrame = 0;
            playerSlam.cyclesPerFrame = 10;
            playerSlam.frameCount = 1;
            playerSlam.source = new Rectangle(0, 132, GameObject.SOURCE_WIDTH, GameObject.SOURCE_HEIGHT);
            playerSlam.interruptible = false;

            player.AddAnimation(AnimationType.Slam, playerSlam);

            // Land Player Animation
            Animation playerLand = new Animation();
            playerLand.currentFrame = 0;
            playerLand.cyclesPerFrame = 5;
            playerLand.frameCount = 2;
            playerLand.source = new Rectangle(0, 99, GameObject.SOURCE_WIDTH, GameObject.SOURCE_HEIGHT);
            playerLand.interruptible = false;

            player.AddAnimation(AnimationType.Land, playerLand);

            // Damage Player Animation
            Animation playerDamage = new Animation();
            playerDamage.currentFrame = 0;
            playerDamage.cyclesPerFrame = 15;
            playerDamage.frameCount = 1;
            playerDamage.source = new Rectangle(0, 165, GameObject.SOURCE_WIDTH, GameObject.SOURCE_HEIGHT);
            playerDamage.interruptible = false;

            player.AddAnimation(AnimationType.Damage, playerDamage);

            // Shoot Player Animation
            Animation playerShoot = new Animation();
            playerShoot.currentFrame = 0;
            playerShoot.cyclesPerFrame = 3;
            playerShoot.frameCount = 6;
            playerShoot.source = new Rectangle(0, 198, GameObject.SOURCE_WIDTH, GameObject.SOURCE_HEIGHT);
            playerShoot.interruptible = false;

            player.AddAnimation(AnimationType.Shoot, playerShoot);

            // Idle Charger Animation
            Animation chargerIdle = new Animation();
            chargerIdle.currentFrame = 0;
            chargerIdle.cyclesPerFrame = 1;
            chargerIdle.frameCount = 1;
            chargerIdle.source = new Rectangle(0, 0, GameObject.SOURCE_WIDTH, GameObject.SOURCE_HEIGHT);
            chargerIdle.interruptible = true;

            // Walk Charger Animation
            Animation chargerWalk = new Animation();
            chargerWalk.currentFrame = 0;
            chargerWalk.cyclesPerFrame = 4;
            chargerWalk.frameCount = 10;
            chargerWalk.source = new Rectangle(0, 33, GameObject.SOURCE_WIDTH, GameObject.SOURCE_HEIGHT);
            chargerWalk.interruptible = true;

            // Jump Charger Animation
            Animation chargerJump = new Animation();
            chargerJump.currentFrame = 0;
            chargerJump.cyclesPerFrame = 1;
            chargerJump.frameCount = 2;
            chargerJump.source = new Rectangle(0, 66, GameObject.SOURCE_WIDTH, GameObject.SOURCE_HEIGHT);
            chargerJump.interruptible = true;

            // Idle Shooter Animation
            Animation shooterIdle = new Animation();
            shooterIdle.currentFrame = 0;
            shooterIdle.cyclesPerFrame = 1;
            shooterIdle.frameCount = 1;
            shooterIdle.source = new Rectangle(0, 0, GameObject.SOURCE_WIDTH, GameObject.SOURCE_HEIGHT);
            shooterIdle.interruptible = true;

            // Walk Shooter Animation
            Animation shooterWalk = new Animation();
            shooterWalk.currentFrame = 0;
            shooterWalk.cyclesPerFrame = 4;
            shooterWalk.frameCount = 10;
            shooterWalk.source = new Rectangle(0, 33, GameObject.SOURCE_WIDTH, GameObject.SOURCE_HEIGHT);
            shooterWalk.interruptible = true;

            // Shoot Shooter Animation
            Animation shooterShoot = new Animation();
            shooterShoot.currentFrame = 0;
            shooterShoot.cyclesPerFrame = 3;
            shooterShoot.frameCount = 6;
            shooterShoot.source = new Rectangle(0, 66, GameObject.SOURCE_WIDTH, GameObject.SOURCE_HEIGHT);
            shooterShoot.interruptible = false;

            // Load in enemy animations
            foreach (Enemy e in enemyManager.Objects)
            {
                if (e is Charger charger)
                {
                    charger.AddAnimation(AnimationType.Idle, chargerIdle);
                    charger.AddAnimation(AnimationType.Walk, chargerWalk);
                    charger.AddAnimation(AnimationType.Jump, chargerJump);
                }

                if (e is Shooter shooter)
                {
                    shooter.AddAnimation(AnimationType.Idle, shooterIdle);
                    shooter.AddAnimation(AnimationType.Jump, shooterWalk);
                    shooter.AddAnimation(AnimationType.Shoot, shooterShoot);
                }
            }
        }

        /// <summary>
        /// Resets the level to it's default values
        /// respawns Player, & enemies
        /// </summary>
        private void ResetLevel()
        {
            // Reset the time
            clockTime = 0.00f;

            // Go through Reset through managers
            enemyManager.ResetGame();
            collectibleManager.ResetGame();
            platformManager.ResetGame();
        }

        /// <summary>
        /// Convert a Global vector to a Local vector
        /// Do not use on GameObjects as they already have a conversion in place
        /// </summary>
        /// <param name="point">Vector to convert</param>
        /// <returns>A new vector with the applied transformation</returns>
        private Vector2 GlobalToLocal(Vector2 point)
        {
            return Vector2.Transform(point, Matrix.Invert(Camera.Transform));
        }

        /// <summary>
        /// Convert a Global point to a Local point
        /// Do not use on GameObjects as they already have a conversion in place
        /// </summary>
        /// <param name="point">Point to convert</param>
        /// <returns>A new point with the applied transformation</returns>
        private Point GlobalToLocal(Point point)
        {
            Vector2 transform = Vector2.Transform(new Vector2(point.X, point.Y), Matrix.Invert(Camera.Transform));

            return new Point((int)transform.X, (int)transform.Y);
        }

        /// <summary>
        /// Convert a Local point to a Global point
        /// Do not use on GameObjects as they already have a conversion in place
        /// </summary>
        /// <param name="point">Point to convert</param>
        /// <returns>A new point with the applied transformation</returns>
        private Vector2 LocalToGlobal(Vector2 point)
        {
            return Vector2.Transform(point, Camera.Transform);
        }

        /// <summary>
        /// Convert a Local vector to a Global vector
        /// Do not use on GameObjects as they already have a conversion in place
        /// </summary>
        /// <param name="point">Vector to convert</param>
        /// <returns>A new vector with the applied transformation</returns>
        private Point LocalToGlobal(Point point)
        {
            Vector2 transform = Vector2.Transform(new Vector2(point.X, point.Y), Camera.Transform);

            return new Point((int)transform.X, (int)transform.Y);
        }

        private string GetTime(double time)
        {
            double minutes = Math.Floor(time / 60);
            double seconds = time % 60;
            string returnable;

            // Check if we should add a zeros place
            if(seconds < 10)
            {
                seconds = Math.Floor(seconds);
                if(seconds == 10)
                {
                    returnable = minutes + ":" + seconds;
                }
                else
                {
                    returnable = minutes + ":0" + seconds;
                }
                return returnable;
            }
            else
            {
                seconds = Math.Round(seconds);
                if(seconds == 60)
                {
                    seconds = 0;
                    minutes++;
                    returnable = $"{minutes}:0{seconds}";
                }
                else
                {
                    returnable = $"{minutes}:{seconds}";
                }
                return returnable;
            }
            
        }
    }
}
