using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Media;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Input.Touch;
using OobiMobile.Classes;
using System.Collections.Generic;
using System.IO.IsolatedStorage;
using System.IO;
using static System.Math;
using TexturePackerLoader;
using TexturePackerMonoGameDefinitions;

namespace OobiMobile
{
    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    public class Game1 : Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        Texture2D MainCha, background, Heart, Line, RopeTex, MidString, StartMenuBG, ScoreBoard, CreditPage, GameOverPage, PauseBG;
        Button PlayButton, CreditButton, PauseButton, ResumeButton, MainMenuButton, RestartButton;
        Texture2D OobiTop, OobiPupil;
        Texture2D pivot, pivotBase;
        Texture2D waterbarBack, waterbarMain;
        SpriteFont GameoverFont;
        List<Texture2D> EnemyIndex;
        List<Texture2D> ColleIndex;
        List<Enemy> EnemyList;
        List<RopeUnit> Rope;
        List<EnemyGenerator> EnemyGenList;
        List<Collectible> ColleList;
        List<Collectible> ColleToDelete;
        List<CollectibleGenerator> ColleGenList;
        List<int> EnemyDamage;
        Song MainMenu;
        SoundEffect ButtonClick, HitByEnemy,  ReleaseOobi, UrgentTime, ColleHit, HitMidString1, HitMidString2;
        SoundEffectInstance UrgentTimeInstance;
        SpriteRender spriteRender;
        Queue<Vector2> TouchSequence;
        //Spritesheet for animations
        SpriteSheet SpriteSheet;
        AnimationManager OobiAnim, WaterAnim, PlaneAnim, RemoverAnim, WaterSplashAnim;
        //score
        int HighestScore;
        float CurrentScore;
        string ScoreFile;
        int ViewportWidth, ViewportHeight, Levels, RopeUnitsNumber;
        bool IsDraggedTooMuch;
        float PressureTime, DryoutSpeed, DraggingTime;
        float PressureTimeLimit, DraggingTimeLimit;
        float RopeLength, MaxDragDistance;
        Vector2 TouchStart, TouchEnd, TouchDirection;
        Vector2 PivotCenter, MidStringPosition;
        MainCharacter mc;
        const float MaxLives = 100.0f;

        //Is Dying
        bool IsDying;
        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";

            graphics.IsFullScreen = true;
            graphics.PreferredBackBufferWidth = 1080;
            graphics.PreferredBackBufferHeight = 1920;
            graphics.SupportedOrientations = DisplayOrientation.Portrait;

        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            // TODO: Add your initialization logic here
            EnemyList = new List<Enemy>();
            EnemyIndex = new List<Texture2D>();
            ColleIndex = new List<Texture2D>();
            EnemyGenList = new List<EnemyGenerator>();
            ColleList = new List<Collectible>();
            ColleToDelete = new List<Collectible>();
            ColleGenList = new List<CollectibleGenerator>();
            EnemyDamage = new List<int>();
            Rope = new List<RopeUnit>();
            ViewportWidth = GraphicsDevice.Viewport.Width;
            ViewportHeight = GraphicsDevice.Viewport.Height;
            PressureTime = 0.0f;
            PressureTimeLimit = 1.0f;
            DryoutSpeed = 3.0f; //how much lives lose per sec
            RopeUnitsNumber = 10;
            Levels = 0;//0 game start, 1 gaming, 2 game over, 3 pause, 4 credits;
            RopeLength = 0.0f;
            //Drag too much
            IsDraggedTooMuch = false;
            DraggingTime = 0.0f;
            DraggingTimeLimit = 3.0f;
            MaxDragDistance = 9.0f * ViewportHeight / 10.0f - PivotCenter.Y;
            //Scores
            CurrentScore = 0.0f;
            ScoreFile = "HighestScore.txt";
            GetHighestScore();
            IsDying = false;
            //TouchSequence
            TouchSequence = new Queue<Vector2>();
            // TODO: Add your initialization logic here
            int[] toe1 = { 0, 1, 2, 3, 4 };
            EnemyGenerator eneGen = new EnemyGenerator(toe1, new Vector2(ViewportWidth / 2.0f, 0.0f), new Vector2(0, 100.0f), new Vector2(ViewportWidth, ViewportHeight), 4.0f, 300.0f);
            EnemyGenList.Add(eneGen);
            int[] toe2 = { 5 }, toe3 = { 6 };
            EnemyGenerator PlaneGen = new EnemyGenerator(toe2, new Vector2(0.0f, ViewportHeight / 4.0f), new Vector2(300.0f, 0.0f), new Vector2(ViewportWidth, ViewportHeight), 10.0f, 200.0f);
            EnemyGenerator RemoverGen = new EnemyGenerator(toe3, new Vector2(ViewportWidth / 2.0f, 0.0f), new Vector2(0.0f, 300.0f), new Vector2(ViewportWidth, ViewportHeight), 10.0f, 200.0f);
            EnemyGenList.Add(PlaneGen);
            EnemyGenList.Add(RemoverGen);

            int[] toc1 = { 0 };
            CollectibleGenerator colGen = new CollectibleGenerator(toc1, new Vector2(ViewportWidth / 2.0f, 0.0f), new Vector2(0, 200.0f), new Vector2(ViewportWidth, ViewportHeight), 2.0f, 500.0f);
            ColleGenList.Add(colGen);
            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            // TODO: use this.Content to load your game content here
            //Animations
            var spriteLoader = new SpriteSheetLoader(Content, GraphicsDevice);
            spriteRender = new SpriteRender(spriteBatch);

            SpriteSheet = spriteLoader.Load("Oobi.png", Content.Load<Texture2D>("Oobi"));
            //intialize
            InitializeAnimationManagers();
            //StartMenuBG
            StartMenuBG = Content.Load<Texture2D>("UI/Oobi Start Menu without shadows");
            //Credits Page
            CreditPage = Content.Load<Texture2D>("UI/Credits screen");
            //Game Over Page
            GameOverPage = Content.Load<Texture2D>("UI/game o0ver2");
            //ScoreBoard
            ScoreBoard = Content.Load<Texture2D>("UI/high score box");
            //Pause BG
            PauseBG = Content.Load<Texture2D>("UI/pause menu without buttons");
            MainCha = Content.Load<Texture2D>("Oobi_Base");
            OobiTop = Content.Load<Texture2D>("Oobi_Glass Top");
            OobiPupil = Content.Load<Texture2D>("Oobi_Pupil");
            background = Content.Load<Texture2D>("Background_01");
            pivot = Content.Load<Texture2D>("Pivot_Top");
            pivotBase = Content.Load<Texture2D>("Pivot_Base");
            Heart = Content.Load<Texture2D>("Heart");
            GameoverFont = Content.Load<SpriteFont>("gameover");
            RopeTex = Content.Load<Texture2D>("rope");
            waterbarBack = Content.Load<Texture2D>("Waterbar_Backdrop");
            waterbarMain = Content.Load<Texture2D>("Waterbar_Main");
            MidString = Content.Load<Texture2D>("String_Borderline");
            //EnemyIndex.Add(Content.Load<Texture2D>("Bee_Placeholder"));
            //EnemyDamage.Add(20);
            EnemyIndex.Add(Content.Load<Texture2D>("Tack_1_Placeholder"));
            EnemyDamage.Add(10);
            EnemyIndex.Add(Content.Load<Texture2D>("Tack_2_Placeholder"));
            EnemyDamage.Add(10);
            EnemyIndex.Add(Content.Load<Texture2D>("Tack_3_Placeholder"));
            EnemyDamage.Add(10);
            EnemyIndex.Add(Content.Load<Texture2D>("Tack_4_Placeholder"));
            EnemyDamage.Add(10);
            EnemyIndex.Add(Content.Load<Texture2D>("Tack_5_Placeholder"));
            EnemyDamage.Add(10);
            EnemyIndex.Add(Content.Load<Texture2D>("Plane"));
            EnemyDamage.Add(20);
            EnemyIndex.Add(Content.Load<Texture2D>("Remover"));
            EnemyDamage.Add(20);

            ColleIndex.Add(Content.Load<Texture2D>("Water_Drop_Still"));

            //Song ButtonClick, HitByEnemy, MainMenu, ReleaseOobi, TackHit, UrgentTime, ColleHit, HitMidString1, HitMidString2;
            MainMenu = Content.Load<Song>("Main menu");

            ButtonClick = Content.Load<SoundEffect>("Button click");
            HitByEnemy = Content.Load<SoundEffect>("Hit by enemy");
            ReleaseOobi = Content.Load<SoundEffect>("Release the shot");
            UrgentTime = Content.Load<SoundEffect>("Tick Tock sound for urgent moment");
            ColleHit = Content.Load<SoundEffect>("Water collect");
            HitMidString1 = Content.Load<SoundEffect>("Water drop hits middle line");
            HitMidString2 = Content.Load<SoundEffect>("Water drop hits the middle line 2");

            UrgentTimeInstance = UrgentTime.CreateInstance();

            //Buttons PlayButton, CreditButton, PauseButton, ResumeButton, MainMenuButton, RestartButton;
            PlayButton = new Button(Content.Load<Texture2D>("UI/Play Button"), new Vector2(460.0f, 870.0f), 0.5f, Content.Load<Texture2D>("UI/Play Button After"));
            CreditButton = new Button(Content.Load<Texture2D>("UI/Credits Button"), new Vector2(620.0f, 750.0f), 0.5f, Content.Load<Texture2D>("UI/Credits Button After"));
            PauseButton = new Button(Content.Load<Texture2D>("UI/Pause Button"), new Vector2(800.0f, 0.0f), 1.0f, Content.Load<Texture2D>("UI/Pause Button After"));
            ResumeButton = new Button(Content.Load<Texture2D>("UI/Continue Button"), Vector2.Zero, 2.0f, Content.Load<Texture2D>("UI/Continue Button After"));
            ResumeButton.SetPos(new Vector2(ViewportWidth / 2.0f - ResumeButton.Scale * ResumeButton.ButtonTexture.Width / 2.0f, 1000));
            //ResumeButton.Position = new Vector2(ViewportWidth / 2.0f - ResumeButton.Scale * ResumeButton.ButtonTexture.Width / 2.0f, 1400);
            MainMenuButton = new Button(Content.Load<Texture2D>("UI/Main menu button"), new Vector2(), 0.5f, Content.Load<Texture2D>("UI/Main menu button after"));
            float tmpWid = (ViewportWidth - 2 * MainMenuButton.Scale * MainMenuButton.ButtonTexture.Width) / 3.0f;
            MainMenuButton.SetPos(new Vector2(2 * tmpWid + MainMenuButton.Scale * MainMenuButton.ButtonTexture.Width, ViewportHeight / 2.0f - MainMenuButton.Scale * MainMenuButton.ButtonTexture.Height / 2.0f));
            RestartButton = new Button(Content.Load<Texture2D>("UI/Restart Button"), new Vector2(), 0.5f, Content.Load<Texture2D>("UI/Restart Button After"));
            RestartButton.SetPos(new Vector2(tmpWid, ViewportHeight / 2.0f - MainMenuButton.Scale * MainMenuButton.ButtonTexture.Height / 2.0f));

            // create 1x1 texture for line drawing
            Line = new Texture2D(GraphicsDevice, 1, 1);
            Line.SetData<Color>(
                new Color[] { Color.OrangeRed });// fill the texture with white


            PivotCenter = new Vector2(GraphicsDevice.Viewport.Width / 2, GraphicsDevice.Viewport.Height / 2);

            MidStringPosition = PivotCenter - new Vector2(0.0f, MidString.Height / 2.0f + pivotBase.Height / 2.0f);
            Vector2 ropeUnitPos = new Vector2(PivotCenter.X, PivotCenter.Y + pivot.Height / 2.0f + RopeTex.Height / 2.0f);
            for(int i = 0; i < RopeUnitsNumber; ++ i)
            {
                Rope.Add(new RopeUnit(ropeUnitPos + new Vector2(0.0f, i * RopeTex.Height), Vector2.Zero, (RopeTex.Width + RopeTex.Height) / 4.0f));
            }
            mc = new MainCharacter(MainCha, new Vector2(PivotCenter.X, Rope[RopeUnitsNumber - 1].Position.Y + RopeTex.Height / 2.0f + MainCha.Height / 2.0f), Vector2.Zero, MaxLives);
            RopeLength = Rope[RopeUnitsNumber - 1].Position.Y + Rope[RopeUnitsNumber - 1].Radius + mc.ColRadius - PivotCenter.Y;

            //Background Sound
            MediaPlayer.IsRepeating = true;
            MediaPlayer.Play(MainMenu);
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// game-specific content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
                Exit();

            // TODO: Add your update logic here
            //start menu
            if (Levels == 0)
            {
                if (TouchPanel.GetCapabilities().IsConnected)
                {
                    TouchCollection touchCol = TouchPanel.GetState();

                    foreach (TouchLocation touch in touchCol)
                    {
                        if (touch.State == TouchLocationState.Moved)
                        {
                            if (PlayButton.IsClicked(touch.Position))
                                PlayButton.IsPressed = true;
                            else
                                PlayButton.IsPressed = false;

                            if (CreditButton.IsClicked(touch.Position))
                                CreditButton.IsPressed = true;
                            else
                                CreditButton.IsPressed = false;

                        }
                        else if (touch.State == TouchLocationState.Released)
                        {
                            if (PlayButton.IsClicked(touch.Position))
                            {
                                Levels = 1;
                                PlayButton.IsPressed = false;
                            }
                            else if (CreditButton.IsClicked(touch.Position))
                            {
                                Levels = 4;
                                CreditButton.IsPressed = false;
                            }
                            else
                            {
                                //do nothing
                            }

                        }
                    }
                }

            }
            else if (Levels == 1)//in game
            {
                // Input Analysis
                if (TouchPanel.GetCapabilities().IsConnected)
                {
                    TouchCollection touchCol = TouchPanel.GetState();

                    foreach (TouchLocation touch in touchCol)
                    {
                        //test++;

                        if (touch.State == TouchLocationState.Pressed)
                        {
                            //if (Vector2.Distance(touch.Position, PivotCenter) > GraphicsDevice.Viewport.Width / 2.0f - mc.ColRadius)
                            //    continue;

                            //Button Click Sound
                            ButtonClick.Play();

                            if(PauseButton.IsClicked(touch.Position))
                            {
                                PauseButton.IsPressed = true;
                            }
                            TouchSequence.Enqueue(touch.Position);
                            if (Vector2.Distance(touch.Position, mc.Position) <= mc.ColRadius && touch.Position.Y > ViewportHeight / 2.0f)
                            {
                                //TouchStart = touch.Position;
                                mc.IsDragged = true;
                                mc.Velc = Vector2.Zero;
                                PressureTime = 0.0f;
                            }
                        }

                        if (touch.State == TouchLocationState.Moved && mc.IsDragged)
                        {
                            //UI pause button
                            if (PauseButton.IsClicked(touch.Position))
                            {
                                PauseButton.IsPressed = true;
                            }
                            else
                                PauseButton.IsPressed = false;
                            mc.Position = touch.Position;
                            if (TouchSequence.Count <= 30)
                                TouchSequence.Enqueue(touch.Position);
                            else
                            {
                                TouchSequence.Dequeue();
                                TouchSequence.Enqueue(touch.Position);
                            }
                            if (mc.Position.Y < ViewportHeight / 2.0f + mc.ColRadius / 2.0f)
                            {
                                mc.IsDragged = false;
                                TouchStart = TouchSequence.Peek();
                                TouchEnd = touch.Position;
                                //mc.IsDragged = false;
                                TouchDirection = Vector2.Normalize(Vector2.Subtract(TouchEnd, TouchStart));
                                if (TouchEnd == TouchStart)
                                {
                                    TouchDirection = Vector2.Zero;
                                }
                                //speed due to distance between two points.
                                float speed = Vector2.Distance(TouchEnd, TouchStart);
                                mc.Velc = TouchDirection * speed;
                            }
                        }
                        /*if (Vector2.Distance(touch.Position, PivotCenter) < GraphicsDevice.Viewport.Width / 2.0f - mc.ColRadius)
                            mc.Position = touch.Position;
                        else
                        {
                            mc.IsDragged = false;
                            TouchDirection = Vector2.Normalize(Vector2.Subtract(mc.Position, TouchStart));
                            if (mc.Position == TouchStart)
                            {
                                TouchDirection = Vector2.Zero;
                            }
                            //speed due to distance between two points.
                            float speed = Vector2.Distance(mc.Position, TouchStart) * 2.0f;
                            mc.Velc = TouchDirection * speed;
                        }*/

                        if (touch.State == TouchLocationState.Released)
                        {
                            
                            if (mc.IsDragged)
                            {
                                //Release Sound
                                ReleaseOobi.Play();
                                mc.IsDragged = false;
                                //PressureTime = 0.0f;
                                //mc.IsSwipe = true;
                                TouchStart = TouchSequence.Peek();
                                TouchEnd = touch.Position;
                                TouchDirection = Vector2.Normalize(Vector2.Subtract(TouchEnd, TouchStart));
                                if (TouchEnd == TouchStart)
                                {
                                    TouchDirection = Vector2.Zero;
                                }
                                //speed due to distance between two points.
                                float speed = Vector2.Distance(TouchEnd, TouchStart);
                                mc.Velc = TouchDirection * speed;
                                TouchSequence.Clear();
                            }
                            if (PauseButton.IsClicked(touch.Position))
                            {
                                Pause();
                                PauseButton.IsPressed = false;
                            }
                        }
                    }
                }
                else
                {
                    MouseState ms = Mouse.GetState();
                    mc.Position = new Vector2(ms.Position.X, ms.Position.Y);
                }
                //Generating Enemies
                foreach (EnemyGenerator eg in EnemyGenList)
                {
                    eg.Generate(gameTime, EnemyList);
                }
                //Enemies move
                foreach (Enemy e in EnemyList)
                {
                    e.Move(gameTime);
                }
                //Enemies out of screen
                foreach (Enemy e in EnemyList)
                {
                    if (e.position.X > ViewportWidth || e.position.Y > ViewportHeight)
                    {
                        EnemyList.Remove(e);
                        break;
                    }
                }
                //Generating Collectibles
                foreach (CollectibleGenerator cg in ColleGenList)
                {
                    cg.Generate(gameTime, ColleList);
                }
                //give radius
                foreach (Collectible c in ColleList)
                {
                    c.ColRadius = (ColleIndex[c.Type].Width + ColleIndex[c.Type].Height) / 4.0f;
                }
                //Collectibles move
                foreach (Collectible c in ColleList)
                {
                    c.Move(gameTime);
                }
                //Collectibles out of screen
                System.Random ran = new System.Random();
                foreach (Collectible c in ColleList)
                {
                    if (c.Position.Y > MidStringPosition.Y - c.ColRadius || c.Position.X > ViewportWidth)
                    {
                        //Randomly play string hit Sound
                        if (ran.Next(2) == 0)
                        {
                            HitMidString1.Play();
                        }
                        else
                            HitMidString2.Play();
                        ColleList.Remove(c);
                        ColleToDelete.Add(c);
                        c.AM = GetNewSplashAM();
                        break;
                    }
                }

                //Rope Unit Move
                for (int i = 0; i < RopeUnitsNumber; ++i)
                {
                    float tensionForcePrev = 0.0f, tensionForceXPrev = 0.0f, tensionForceYPrev = 0.0f;
                    float tensionForceNext = 0.0f, tensionForceXNext = 0.0f, tensionForceYNext = 0.0f;

                    if (i == 0)//first rope unit connects pivot
                    {
                        tensionForcePrev = (Vector2.Distance(PivotCenter, Rope[i].Position) - Rope[i].Radius - (pivot.Height + pivot.Width) / 4.0f) * PhysicsSystem.RopeTensionK;
                        tensionForceXPrev = (PivotCenter.X - Rope[i].Position.X) / Vector2.Distance(Rope[i].Position, PivotCenter) * tensionForcePrev;
                        tensionForceYPrev = (PivotCenter.Y - Rope[i].Position.Y) / Vector2.Distance(Rope[i].Position, PivotCenter) * tensionForcePrev;
                    }
                    else
                    {
                        tensionForcePrev = (Vector2.Distance(Rope[i - 1].Position, Rope[i].Position) - Rope[i].Radius - Rope[i - 1].Radius) * PhysicsSystem.RopeTensionK;
                        tensionForceXPrev = (Rope[i - 1].Position.X - Rope[i].Position.X) / Vector2.Distance(Rope[i].Position, Rope[i - 1].Position) * tensionForcePrev;
                        tensionForceYPrev = (Rope[i - 1].Position.Y - Rope[i].Position.Y) / Vector2.Distance(Rope[i].Position, Rope[i - 1].Position) * tensionForcePrev;
                    }

                    if (i == RopeUnitsNumber - 1)//last rope unit connects oobi
                    {
                        tensionForceNext = (Vector2.Distance(Rope[i].Position, mc.Position) - Rope[i].Radius - mc.ColRadius) * PhysicsSystem.RopeTensionK;
                        tensionForceXNext = (mc.Position.X - Rope[i].Position.X) / Vector2.Distance(Rope[i].Position, mc.Position) * tensionForceNext;
                        tensionForceYNext = (mc.Position.Y - Rope[i].Position.Y) / Vector2.Distance(Rope[i].Position, mc.Position) * tensionForceNext;
                    }
                    else
                    {
                        tensionForceNext = (Vector2.Distance(Rope[i].Position, Rope[i + 1].Position) - Rope[i].Radius - Rope[i + 1].Radius) * PhysicsSystem.RopeTensionK;
                        tensionForceXNext = (Rope[i + 1].Position.X - Rope[i].Position.X) / Vector2.Distance(Rope[i].Position, Rope[i + 1].Position) * tensionForceNext;
                        tensionForceYNext = (Rope[i + 1].Position.Y - Rope[i].Position.Y) / Vector2.Distance(Rope[i].Position, Rope[i + 1].Position) * tensionForceNext;
                    }
                    Rope[i].PhysicsSystem.Force = new Vector2(tensionForceXPrev + tensionForceXNext, tensionForceYPrev + tensionForceYNext + Rope[i].PhysicsSystem.Mass * PhysicsSystem.GravAcc);
                    //Air Fiction
                    Rope[i].PhysicsSystem.Force -= Rope[i].Speed * PhysicsSystem.RopeAirFictionK;
                    Rope[i].Speed += Rope[i].PhysicsSystem.Force / Rope[i].PhysicsSystem.Mass * (float)gameTime.ElapsedGameTime.TotalSeconds;
                }

                foreach (RopeUnit r in Rope)
                {
                    r.Move(gameTime);
                }
                //Time
                /*if(mc.IsDragged)
                    PressureTime += (float)gameTime.ElapsedGameTime.TotalSeconds;
                if(IsDraggedTooMuch)
                    DraggingTime += (float)gameTime.ElapsedGameTime.TotalSeconds;
                if(DraggingTime >= DraggingTimeLimit)
                {
                    //gameover
                }
                if (PressureTime >= PressureTimeLimit) 
                {
                    //If holding too long, then is not swipe
                    mc.IsSwipe = false;
                    //mc.Velc = Vector2.Zero;
                   // mc.Lives -= 10;
                    PressureTime = 0.0f;
                }*/


                //hp drops over time
                mc.Lives -= (float)gameTime.ElapsedGameTime.TotalSeconds * DryoutSpeed;

                //Game over
                if (mc.Lives <= 0)
                {
                    IsDying = true;
                    GameOver();

                }
                //Main character move
                if (mc.IsDragged == false)
                {
                    mc.PhysicsMove(PivotCenter, RopeLength, gameTime);
                    mc.Move(gameTime);

                }
                //mc.BorderCheck(GraphicsDevice.Viewport.Width, GraphicsDevice.Viewport.Height, ViewportWidth / 2.0f, PivotCenter);

                //Collision detection
                //enemy
                foreach (Enemy e in EnemyList)
                {
                    if (Vector2.Distance(mc.Position, e.position) < e.ColRadius + mc.ColRadius)
                    {
                        //Enemy Hit Sound
                        HitByEnemy.Play();
                        mc.Lives -= EnemyDamage[e.type];
                        
                        OobiAnim.Play("Damage");
                        EnemyList.Remove(e);
                        break;
                    }
                }
                //collectible
                foreach (Collectible c in ColleList)
                {
                    if (Vector2.Distance(mc.Position, c.Position) < mc.ColRadius + c.ColRadius)
                    {
                        if (mc.Lives < MaxLives)
                            mc.Lives += 10;
                        ColleList.Remove(c);
                        ColleToDelete.Add(c);
                        c.AM = GetNewSplashAM();
                        // Collect Sound
                        OobiAnim.Play("Collect");
                        ColleHit.Play();
                        break;
                    }
                }

                //Scores
                CurrentScore += (float)gameTime.ElapsedGameTime.TotalSeconds * 5.0f;

                if ((int)CurrentScore > HighestScore)
                {
                    HighestScore = (int)CurrentScore;
                }

                //Urgent sound
                if (mc.Lives <= 20.0f)
                {
                    UrgentTimeInstance.Play();
                }
                else
                    UrgentTimeInstance.Stop();
            }
            else if (Levels == 2)//game over
            {
                UrgentTimeInstance.Stop();
                if (TouchPanel.GetCapabilities().IsConnected)
                {
                    TouchCollection touchCol = TouchPanel.GetState();

                    foreach (TouchLocation touch in touchCol)
                    {
                        if (touch.State == TouchLocationState.Released)
                            RestartGame();
                    }
                }
                SetHighestScore();
            }
            else if (Levels == 3)//Pause
            {
                if (TouchPanel.GetCapabilities().IsConnected)
                {
                    TouchCollection touchCol = TouchPanel.GetState();

                    foreach (TouchLocation touch in touchCol)
                    {
                        if (touch.State == TouchLocationState.Moved)
                        {
                            if(RestartButton.IsClicked(touch.Position))
                                RestartButton.IsPressed = true;
                            else
                                RestartButton.IsPressed = false;

                            if (MainMenuButton.IsClicked(touch.Position))
                                MainMenuButton.IsPressed = true;
                            else
                                MainMenuButton.IsPressed = false;

                            if (ResumeButton.IsClicked(touch.Position))
                                ResumeButton.IsPressed = true;
                            else
                                ResumeButton.IsPressed = false;

                        }
                        else if (touch.State == TouchLocationState.Released)
                        {
                            if(RestartButton.IsClicked(touch.Position))
                            {
                                RestartButton.IsPressed = false;
                                RestartGame();
                            }
                            else if (MainMenuButton.IsClicked(touch.Position))
                            {
                                MainMenuButton.IsPressed = false;
                                RestartGame();
                                Levels = 0;
                            }
                            else if (ResumeButton.IsClicked(touch.Position))
                            {
                                ResumeButton.IsPressed = false;
                                Unpause();
                            }
                            else
                            {
                                //do nothing
                            }

                        }
                    }
                }
            }
            else if (Levels == 4)//Credits
            {
                if(TouchPanel.GetCapabilities().IsConnected)
                {
                    TouchCollection touchCol = TouchPanel.GetState();

                    foreach (TouchLocation touch in touchCol)
                    {
                        if (touch.State == TouchLocationState.Released)
                            Levels = 0;
                    }
                }
            }
            
            
            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            // TODO: Add your drawing code here
            
            //Rope
            if (Levels == 0)
            {
                spriteBatch.Begin();

                spriteBatch.Draw(StartMenuBG, new Rectangle(0, 0, ViewportWidth, ViewportHeight), Color.White);

                //Buttons
                //Play button
                if (PlayButton.IsPressed)
                {
                    spriteBatch.Draw(PlayButton.ButtonTextureAfterPressed, PlayButton.Position, null, Color.White, 0.0f, Vector2.Zero, RestartButton.Scale, SpriteEffects.None, 0.0f);
                }
                else
                    spriteBatch.Draw(PlayButton.ButtonTexture, PlayButton.Position, null, Color.White, 0.0f, Vector2.Zero, RestartButton.Scale, SpriteEffects.None, 0.0f);
                //Credits button
                if (CreditButton.IsPressed)
                    spriteBatch.Draw(CreditButton.ButtonTextureAfterPressed, CreditButton.Position, null, Color.White, 0.0f, Vector2.Zero, RestartButton.Scale, SpriteEffects.None, 0.0f);
                else
                    spriteBatch.Draw(CreditButton.ButtonTexture, CreditButton.Position, null, Color.White, 0.0f, Vector2.Zero, RestartButton.Scale, SpriteEffects.None, 0.0f);

                spriteBatch.End();

            }
            else if (Levels == 1)
            {
                //Background & ScoreBoard & PauseButton
                spriteBatch.Begin();
                spriteBatch.Draw(background, new Rectangle(0, 0, GraphicsDevice.Viewport.Width, GraphicsDevice.Viewport.Height), Color.White);
                float tmpScoreScale = 0.5f;
                spriteBatch.Draw(ScoreBoard, new Rectangle(0, 0, (int)(ScoreBoard.Width * tmpScoreScale), (int)(ScoreBoard.Height * tmpScoreScale)), Color.White);
                //Draw scores
                spriteBatch.DrawString(GameoverFont, ((int)CurrentScore).ToString(), new Vector2(2.5f * ViewportWidth / 10.0f, 160.0f), Color.Black);
                spriteBatch.DrawString(GameoverFont, HighestScore.ToString(), new Vector2(2.5f * ViewportWidth / 10.0f, 80.0f), Color.Black);

                if (PauseButton.IsPressed)
                    spriteBatch.Draw(PauseButton.ButtonTextureAfterPressed, PauseButton.Position, Color.White);
                else
                    spriteBatch.Draw(PauseButton.ButtonTexture, PauseButton.Position, Color.White);

                //Pivot & Line & MidString
                //Rope Units
                foreach (RopeUnit r in Rope)
                {
                    spriteBatch.Draw(RopeTex, r.Position - new Vector2(RopeTex.Width / 2.0f, RopeTex.Height / 2.0f), Color.White);
                }
                //spriteBatch.Draw(background, new Rectangle(0, 0, GraphicsDevice.Viewport.Width, GraphicsDevice.Viewport.Height), Color.White);

                spriteBatch.Draw(pivotBase, PivotCenter - new Vector2(pivotBase.Width / 2.0f, pivotBase.Height / 2.0f), Color.White);
                spriteBatch.Draw(MidString, new Rectangle(0, (int)MidStringPosition.Y, ViewportWidth, MidString.Height), Color.White);

                spriteBatch.Draw(pivot, PivotCenter - new Vector2(pivot.Width / 2.0f, pivot.Height / 2.0f), Color.White);

                //Draw Line
                //DrawLine(spriteBatch, Line, PivotCenter, mc.Position, 10);
                
                spriteBatch.End();

                //Enemy & Collectible
                spriteBatch.Begin();
                //Oobi
                spriteBatch.Draw(mc.Texture, mc.Position - new Vector2(MainCha.Width / 2.0f, MainCha.Height / 2.0f), Color.White);
                spriteBatch.Draw(OobiPupil, mc.Position - new Vector2(OobiPupil.Width / 2.0f, OobiPupil.Height / 2.0f), Color.White);
                SpriteFrame tmp_sf;
                /*if(IsDying)
                {
                    OobiAnim.Play("Death");
                    tmp_sf = OobiAnim.UpdateOnce(gameTime);
                    if (tmp_sf == null)
                        GameOver();
                }
                else*/
                tmp_sf = OobiAnim.Update(gameTime);
                spriteBatch.Draw(tmp_sf.Texture, mc.Position - new Vector2(MainCha.Width / 2.0f, MainCha.Height / 2.0f), tmp_sf.SourceRectangle,Color.White);

                spriteBatch.Draw(OobiTop, mc.Position - new Vector2(OobiTop.Width / 2.0f, OobiTop.Height / 2.0f), new Color(1.0f, mc.Lives / MaxLives, mc.Lives / MaxLives, 0.5f));
                foreach (Enemy e in EnemyList)
                {
                    e.EnemySize = new Vector2(EnemyIndex[e.type].Width, EnemyIndex[e.type].Height); //actually redundant
                    e.ColRadius = (EnemyIndex[e.type].Width + EnemyIndex[e.type].Height) / 8.0f; //give collison box size
                    if (e.type == 5)
                    {
                        var sf_tmp = PlaneAnim.Update(gameTime);
                        //spriteBatch.Draw(EnemyIndex[e.type], new Rectangle((e.position - e.EnemySize / 4.0f).ToPoint(), (e.EnemySize / 4.0f).ToPoint()), null, Color.White, 0.0f, Vector2.Zero, SpriteEffects.FlipHorizontally, 0.0f);
                        spriteRender.Draw(sf_tmp, e.position, Color.White, 0, 0.25f, SpriteEffects.FlipHorizontally);
                    }
                    else if (e.type == 6)
                    {
                        var sf_tmp = RemoverAnim.Update(gameTime);
                        spriteRender.Draw(sf_tmp, e.position, Color.White, 0, 0.25f, SpriteEffects.FlipHorizontally);
                    }
                    else
                        spriteBatch.Draw(EnemyIndex[e.type], e.position - e.EnemySize / 2.0f, Color.White);
                }
                foreach (Collectible c in ColleList)
                {
                    c.ColRadius = (ColleIndex[c.Type].Width + ColleIndex[c.Type].Height) / 8.0f;//give collison box size
                    //spriteBatch.Draw(ColleIndex[c.Type], c.Position - new Vector2(ColleIndex[c.Type].Width, ColleIndex[c.Type].Height) / 2.0f, Color.White);
                    var sf_tmp = WaterAnim.Update(gameTime);
                    spriteRender.Draw(sf_tmp, c.Position - new Vector2(ColleIndex[c.Type].Width, ColleIndex[c.Type].Height) / 2.0f, Color.White);
                }
                //splash
                foreach (Collectible c in ColleToDelete)
                {
                    //spriteBatch.Draw(ColleIndex[c.Type], c.Position - new Vector2(ColleIndex[c.Type].Width, ColleIndex[c.Type].Height) / 2.0f, Color.White);
                    var sf_tmp = c.AM.UpdateOnce(gameTime);
                    if(sf_tmp == null)
                    {
                        ColleToDelete.Remove(c);
                        break;
                    }
                    spriteRender.Draw(sf_tmp, c.Position - new Vector2(ColleIndex[c.Type].Width, ColleIndex[c.Type].Height) / 2.0f, Color.White);
                }
                spriteBatch.End();
                spriteBatch.Begin();
                //Heart
                /*for (int i = 0; i < mc.Lives / 10; ++i)
                {
                    spriteBatch.Draw(Heart, new Vector2(ViewportWidth / 20.0f + i * 1.2f * Heart.Width, ViewportHeight / 40.0f), Color.White);
                }*/
                //Water bar
                
                int WaterbarBackMarginWidth = ViewportWidth / 20;
                int WaterbarBackWidth = ViewportWidth - 2 * WaterbarBackMarginWidth;
                int WaterbarBackMarginHeight = ViewportHeight / 40;
                int WaterbarBackHeight = ViewportHeight / 20;

                int WaterbarMainMarginWidth = WaterbarBackWidth / 40;
                int WaterbarMainWidth = WaterbarBackWidth - 2 * WaterbarMainMarginWidth;
                int WaterbarMainMarginHeight = WaterbarBackHeight / 20;
                int WaterbarMainHeight = WaterbarBackHeight - 2 * WaterbarMainMarginHeight;


                spriteBatch.Draw(waterbarBack, new Rectangle(WaterbarBackMarginWidth, ViewportHeight - WaterbarBackMarginHeight - WaterbarBackHeight, WaterbarBackWidth, WaterbarBackHeight), Color.White);
                spriteBatch.Draw(waterbarMain, new Rectangle(WaterbarMainMarginWidth + WaterbarBackMarginWidth, ViewportHeight - WaterbarBackMarginHeight - WaterbarBackHeight + WaterbarMainMarginHeight, (int)(WaterbarMainWidth * mc.Lives / MaxLives), WaterbarMainHeight), new Rectangle(0, 0, (int)(waterbarMain.Width * mc.Lives / MaxLives), waterbarMain.Height), Color.White);
                spriteBatch.End();
            }

            else if (Levels == 2)//Game over
            {
                spriteBatch.Begin();
                spriteBatch.Draw(background, new Rectangle(0, 0, GraphicsDevice.Viewport.Width, GraphicsDevice.Viewport.Height), Color.White);

                spriteBatch.Draw(GameOverPage, new Vector2(ViewportWidth / 2.0f - GameOverPage.Width / 2.0f, ViewportHeight / 2.0f - GameOverPage.Height / 2.0f), Color.White);
                spriteBatch.End();
            }
            else if(Levels == 3) //pause
            {
                spriteBatch.Begin();
                spriteBatch.Draw(background, new Rectangle(0, 0, GraphicsDevice.Viewport.Width, GraphicsDevice.Viewport.Height), Color.White);

                spriteBatch.Draw(PauseBG, new Rectangle(0, 0, GraphicsDevice.Viewport.Width, GraphicsDevice.Viewport.Height), Color.White);
                if(RestartButton.IsPressed)
                    spriteBatch.Draw(RestartButton.ButtonTextureAfterPressed, RestartButton.Position, null, Color.White, 0.0f, Vector2.Zero, RestartButton.Scale, SpriteEffects.None, 0.0f);
                else
                    spriteBatch.Draw(RestartButton.ButtonTexture, RestartButton.Position, null, Color.White, 0.0f, Vector2.Zero, RestartButton.Scale, SpriteEffects.None, 0.0f);

                if (MainMenuButton.IsPressed)
                    spriteBatch.Draw(MainMenuButton.ButtonTextureAfterPressed, MainMenuButton.Position, null, Color.White, 0.0f, Vector2.Zero, MainMenuButton.Scale, SpriteEffects.None, 0.0f);
                else
                    spriteBatch.Draw(MainMenuButton.ButtonTexture, MainMenuButton.Position, null, Color.White, 0.0f, Vector2.Zero, MainMenuButton.Scale, SpriteEffects.None, 0.0f);

                if (ResumeButton.IsPressed)
                    spriteBatch.Draw(ResumeButton.ButtonTextureAfterPressed, ResumeButton.Position, null, Color.White, 0.0f, Vector2.Zero, ResumeButton.Scale, SpriteEffects.None, 0.0f);
                else
                    spriteBatch.Draw(ResumeButton.ButtonTexture, ResumeButton.Position, null, Color.White, 0.0f, Vector2.Zero, ResumeButton.Scale, SpriteEffects.None, 0.0f);


                spriteBatch.End();
            }
            else if(Levels == 4) //credit
            {
                spriteBatch.Begin();
                spriteBatch.Draw(CreditPage, new Rectangle(0, 0, GraphicsDevice.Viewport.Width, GraphicsDevice.Viewport.Height), Color.White);
                spriteBatch.End();
            }
            base.Draw(gameTime);
        }
        //https://gamedev.stackexchange.com/questions/44015/how-can-i-draw-a-simple-2d-line-in-xna-without-using-3d-primitives-and-shders
        void DrawLine(SpriteBatch sb, Texture2D t, Vector2 start, Vector2 end, int width)
        {
            Vector2 edge = end - start;
            // calculate angle to rotate line
            float angle =
                (float)Atan2(edge.Y, edge.X);


            sb.Draw(t,
                new Rectangle(// rectangle defines shape of line and position of start of line
                    (int)start.X - width / 2,
                    (int)start.Y,
                    (int)edge.Length(), //sb will strech the texture to fill this rectangle
                    width), //width of line, change this to make thicker line
                null,
                Color.Red, //colour of line
                angle,     //angle of line (calulated above)
                new Vector2(0, 0), // point in line about which to rotate
                SpriteEffects.None,
                0);

        }

        void RestartGame()
        {
            //reset oobi position and lives
            
            EnemyList.Clear();
            ColleList.Clear();
            Levels = 1;
            IsDying = false;
            //Rope position
            Vector2 ropeUnitPos = new Vector2(PivotCenter.X, PivotCenter.Y + pivot.Height / 2.0f + RopeTex.Height / 2.0f);
            for (int i = 0; i < RopeUnitsNumber; i++)
            {
                Rope[i].Position = ropeUnitPos + new Vector2(0.0f, i * RopeTex.Height);
            }
            //Main Character reset
            mc.Position = new Vector2(PivotCenter.X, Rope[RopeUnitsNumber - 1].Position.Y + RopeTex.Height / 2.0f + MainCha.Height / 2.0f);
            mc.Velc = Vector2.Zero;
            mc.Lives = MaxLives;
            //Scores
            CurrentScore = 0;
            GetHighestScore();
        }

        void GetHighestScore()
        {
            var store = IsolatedStorageFile.GetUserStoreForApplication();
            if (store.FileExists(ScoreFile))
            {
                var fs = store.OpenFile(ScoreFile, FileMode.Open);
                using (StreamReader sr = new StreamReader(fs))
                {
                    HighestScore = System.Convert.ToInt16(sr.ReadLine());
                }
            }
            else
            {
                var fs = store.CreateFile(ScoreFile);
                using (StreamWriter sw = new StreamWriter(fs))
                {
                    sw.Write("0");
                }
                HighestScore = 0;
            }
        }

        void SetHighestScore()
        {
            var store = IsolatedStorageFile.GetUserStoreForApplication();
            if (store.FileExists(ScoreFile))
            {
                var fs = store.OpenFile(ScoreFile, FileMode.Open);
                using (StreamWriter sw = new StreamWriter(fs))
                {
                    sw.Write(HighestScore.ToString());
                }
            }
            else
            {
                var fs = store.CreateFile(ScoreFile);
                using (StreamWriter sw = new StreamWriter(fs))
                {
                    sw.Write(HighestScore.ToString());
                }
            }
        }

        void GameOver()
        {
            Levels = 2;
        }

        void Pause()
        {
            Levels = 3;
        }

        void Unpause()
        {
            Levels = 1;
        }

        void InitializeAnimationManagers()
        {
            string[] IdleSprites =
            {
                OobiAnimation.Oobi_Idle_Oobi_Idle_01,
                OobiAnimation.Oobi_Idle_Oobi_Idle_02
            };
            string[] BlinkSprites =
            {
                OobiAnimation.Oobi_Blink_Oobi_Blink_01,
                OobiAnimation.Oobi_Blink_Oobi_Blink_02,
                OobiAnimation.Oobi_Blink_Oobi_Blink_03,
                OobiAnimation.Oobi_Blink_Oobi_Blink_04,
                OobiAnimation.Oobi_Blink_Oobi_Blink_05
            };
            string[] DeathSprites =
            {
                OobiAnimation.Oobi_Death_Oobi_Death_01,
                OobiAnimation.Oobi_Death_Oobi_Death_02,
                OobiAnimation.Oobi_Death_Oobi_Death_03,
                OobiAnimation.Oobi_Death_Oobi_Death_04,
                OobiAnimation.Oobi_Death_Oobi_Death_05,
                OobiAnimation.Oobi_Death_Oobi_Death_06,
                OobiAnimation.Oobi_Death_Oobi_Death_07,
                OobiAnimation.Oobi_Death_Oobi_Death_08

            };
            string[] CollectSprites =
            {
                OobiAnimation.Oobi_DropCollect_Oobi_DropCollect_01,
                OobiAnimation.Oobi_DropCollect_Oobi_DropCollect_02,
                OobiAnimation.Oobi_DropCollect_Oobi_DropCollect_03
            };
            string[] DamageSprites =
            {
                OobiAnimation.Oobi_TakeDamage_Oobi_TakeDamage_01,
                OobiAnimation.Oobi_TakeDamage_Oobi_TakeDamage_02,
                OobiAnimation.Oobi_TakeDamage_Oobi_TakeDamage_03
            };

            string[] PlaneSprites =
            {
                OobiAnimation.Plane_Plane_001,
                OobiAnimation.Plane_Plane_003,
                OobiAnimation.Plane_Plane_005,
                OobiAnimation.Plane_Plane_007,
                OobiAnimation.Plane_Plane_009,
                OobiAnimation.Plane_Plane_011,
                OobiAnimation.Plane_Plane_013,
                OobiAnimation.Plane_Plane_015,
                OobiAnimation.Plane_Plane_017,
                OobiAnimation.Plane_Plane_019,
                OobiAnimation.Plane_Plane_021,
                OobiAnimation.Plane_Plane_023,
                OobiAnimation.Plane_Plane_025,
                OobiAnimation.Plane_Plane_027,
                OobiAnimation.Plane_Plane_029,
                OobiAnimation.Plane_Plane_031,
                OobiAnimation.Plane_Plane_033,
                OobiAnimation.Plane_Plane_035,
                OobiAnimation.Plane_Plane_037,
                OobiAnimation.Plane_Plane_039
            };

            string[] RemoverSprites =
            {
                OobiAnimation.Remover_Clap1,
                OobiAnimation.Remover_Clap3,
                OobiAnimation.Remover_Clap5,
                OobiAnimation.Remover_Clap7,
                OobiAnimation.Remover_Clap9,
                OobiAnimation.Remover_Clap11,
                OobiAnimation.Remover_Clap13,
                OobiAnimation.Remover_Clap15,
                OobiAnimation.Remover_Clap17,
                OobiAnimation.Remover_Clap19
            };

            string[] WaterIdleSprites =
            {
                OobiAnimation.WaterDrop_Drop_Animations_Drop_Idle_01,
                OobiAnimation.WaterDrop_Drop_Animations_Drop_Idle_02,
                OobiAnimation.WaterDrop_Drop_Animations_Drop_Idle_03,
                OobiAnimation.WaterDrop_Drop_Animations_Drop_Idle_04,
                OobiAnimation.WaterDrop_Drop_Animations_Drop_Idle_05,
                OobiAnimation.WaterDrop_Drop_Animations_Drop_Idle_06,
                OobiAnimation.WaterDrop_Drop_Animations_Drop_Idle_07,
                OobiAnimation.WaterDrop_Drop_Animations_Drop_Idle_08,
                OobiAnimation.WaterDrop_Drop_Animations_Drop_Idle_09,
                OobiAnimation.WaterDrop_Drop_Animations_Drop_Idle_10,
                OobiAnimation.WaterDrop_Drop_Animations_Drop_Idle_11

            };

            string[] WaterSplashSprites =
            {
                OobiAnimation.WaterDrop_Splash_Animations_Drop_Splash_01,
                OobiAnimation.WaterDrop_Splash_Animations_Drop_Splash_02,
                OobiAnimation.WaterDrop_Splash_Animations_Drop_Splash_03,
                OobiAnimation.WaterDrop_Splash_Animations_Drop_Splash_04,
                OobiAnimation.WaterDrop_Splash_Animations_Drop_Splash_05,
                OobiAnimation.WaterDrop_Splash_Animations_Drop_Splash_06,
                OobiAnimation.WaterDrop_Splash_Animations_Drop_Splash_07,
                OobiAnimation.WaterDrop_Splash_Animations_Drop_Splash_08
            };
            Animation OobiIdle = new Animation("Idle", 0.3f, IdleSprites);
            Animation OobiDeath = new Animation("Death", 1.0f, DeathSprites);
            Animation OobiBlink = new Animation("Blink", 0.5f, BlinkSprites);
            Animation OobiDamage = new Animation("Damage", 0.3f, BlinkSprites);
            Animation OobiCollect = new Animation("Collect", 0.3f, CollectSprites);

            Animation WaterIdle = new Animation("WaterIdle", 2.0f, WaterIdleSprites);
            Animation WaterSplash = new Animation("WaterSplash", 0.5f, WaterSplashSprites);

            Animation Plane = new Animation("Plane", 5.0f, PlaneSprites);
            Animation RemoverAni = new Animation("Remover", 5.0f, RemoverSprites);
            Animation[] OobiAnis = { OobiIdle, OobiDeath, OobiBlink, OobiDamage, OobiCollect };
            Animation[] WaterAnis = { WaterIdle};
            Animation[] WaterSplashAnis = { WaterSplash };
            Animation[] PlaneAnis = { Plane };
            Animation[] RemoverAnis = { RemoverAni };

            OobiAnim = new AnimationManager("Idle", OobiAnis, SpriteSheet);
            WaterAnim = new AnimationManager("WaterIdle", WaterAnis, SpriteSheet);
            PlaneAnim = new AnimationManager("Plane", PlaneAnis, SpriteSheet);
            RemoverAnim = new AnimationManager("Remover", RemoverAnis, SpriteSheet);
            WaterSplashAnim = new AnimationManager("WaterSplash", WaterSplashAnis, SpriteSheet);
        }

        AnimationManager GetNewSplashAM()
        {
            string[] WaterSplashSprites =
            {
                OobiAnimation.WaterDrop_Splash_Animations_Drop_Splash_01,
                OobiAnimation.WaterDrop_Splash_Animations_Drop_Splash_02,
                OobiAnimation.WaterDrop_Splash_Animations_Drop_Splash_03,
                OobiAnimation.WaterDrop_Splash_Animations_Drop_Splash_04,
                OobiAnimation.WaterDrop_Splash_Animations_Drop_Splash_05,
                OobiAnimation.WaterDrop_Splash_Animations_Drop_Splash_06,
                OobiAnimation.WaterDrop_Splash_Animations_Drop_Splash_07,
                OobiAnimation.WaterDrop_Splash_Animations_Drop_Splash_08
            };
            Animation WaterSplash = new Animation("WaterSplash", 0.5f, WaterSplashSprites);
            Animation[] WaterSplashAnis = { WaterSplash };

            AnimationManager am = new AnimationManager("WaterSplash", WaterSplashAnis, SpriteSheet);
            return am;
        }
    }
}
