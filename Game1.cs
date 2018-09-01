using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Input.Touch;
using OobiMobile.Classes;
using System.Collections.Generic;
using System.IO.IsolatedStorage;
using System.IO;
using static System.Math;

namespace OobiMobile
{
    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    public class Game1 : Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        Texture2D MainCha, background,  Heart, Line, RopeTex;
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
        List<CollectibleGenerator> ColleGenList;
        List<int> EnemyDamage;

        int HighestScore;
        float CurrentScore;
        string ScoreFile;
        int ViewportWidth, ViewportHeight, Levels, RopeUnitsNumber;
        bool IsDraggedTooMuch;
        float PressureTime, DryoutSpeed, DraggingTime;
        float PressureTimeLimit, DraggingTimeLimit;
        float RopeLength, MaxDragDistance;
        Vector2 TouchStart, TouchEnd, TouchDirection;
        Vector2 PivotCenter;
        MainCharacter mc;
        const float MaxLives = 100.0f;

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";

            graphics.IsFullScreen = true;
            //graphics.PreferredBackBufferWidth = 800;
            //graphics.PreferredBackBufferHeight = 480;
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
            ColleGenList = new List<CollectibleGenerator>();
            EnemyDamage = new List<int>();
            Rope = new List<RopeUnit>();
            ViewportWidth = GraphicsDevice.Viewport.Width;
            ViewportHeight = GraphicsDevice.Viewport.Height;
            PressureTime = 0.0f;
            PressureTimeLimit = 1.0f;
            DryoutSpeed = 5.0f; //how much lives lose per sec
            RopeUnitsNumber = 10;
            Levels = 1;//0 game start, 1 gaming, 2 game over;
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

            // TODO: Add your initialization logic here
            int[] toe1 = { 0, 1, 2, 3 };
            EnemyGenerator eneGen = new EnemyGenerator(toe1, new Vector2(ViewportWidth / 2.0f, 0.0f), new Vector2(0, 100.0f), new Vector2(ViewportWidth, ViewportHeight), 4.0f, 200.0f);
            EnemyGenList.Add(eneGen);

            int[] toc1 = { 0 };
            CollectibleGenerator colGen = new CollectibleGenerator(toc1, new Vector2(ViewportWidth / 2.0f, 0.0f), new Vector2(0, 100.0f), new Vector2(ViewportWidth, ViewportHeight), 3.0f, 500.0f);
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
    
            ColleIndex.Add(Content.Load<Texture2D>("Water_Drop_Still"));

            // create 1x1 texture for line drawing
            Line = new Texture2D(GraphicsDevice, 1, 1);
            Line.SetData<Color>(
                new Color[] { Color.OrangeRed });// fill the texture with white


            PivotCenter = new Vector2(GraphicsDevice.Viewport.Width / 2, GraphicsDevice.Viewport.Height / 2);

            Vector2 ropeUnitPos = new Vector2(PivotCenter.X, PivotCenter.Y + pivot.Height / 2.0f + RopeTex.Height / 2.0f);
            for(int i = 0; i < RopeUnitsNumber; ++ i)
            {
                Rope.Add(new RopeUnit(ropeUnitPos + new Vector2(0.0f, i * RopeTex.Height), Vector2.Zero, (RopeTex.Width + RopeTex.Height) / 4.0f));
            }
            mc = new MainCharacter(MainCha, new Vector2(PivotCenter.X, Rope[RopeUnitsNumber - 1].Position.Y + RopeTex.Height / 2.0f + MainCha.Height / 2.0f), Vector2.Zero, MaxLives);
            RopeLength = Rope[RopeUnitsNumber - 1].Position.Y + Rope[RopeUnitsNumber - 1].Radius + mc.ColRadius - PivotCenter.Y;


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
            if (TouchPanel.GetCapabilities().IsConnected)
            {
                TouchCollection touchCol = TouchPanel.GetState();

                foreach (TouchLocation touch in touchCol)
                {
                    if (touch.State == TouchLocationState.Pressed)
                    {
                        //if (Vector2.Distance(touch.Position, PivotCenter) > GraphicsDevice.Viewport.Width / 2.0f - mc.ColRadius)
                        //    continue;
                        if (touch.Position.Y < ViewportHeight / 2.0f)
                            continue;
                        if(Vector2.Distance(touch.Position, mc.Position) <= mc.ColRadius)
                        {
                            TouchStart = touch.Position;
                            mc.IsDragged = true;
                            mc.Velc = Vector2.Zero;
                            PressureTime = 0.0f;
                        }
                    }
                    
                    if(touch.State == TouchLocationState.Moved && mc.IsDragged)
                    {
                        if(Vector2.Distance(touch.Position, PivotCenter) < MaxDragDistance)
                        {
                            mc.Position = touch.Position;
                            IsDraggedTooMuch = false;
                            DraggingTime = 0.0f;
                        }
                        else
                        {
                            Vector2 tmpDirection = Vector2.Normalize(touch.Position - PivotCenter);
                            mc.Position = tmpDirection * MaxDragDistance;
                            IsDraggedTooMuch = true;
                        }

                        if (mc.Position.Y < ViewportHeight / 2.0f + mc.ColRadius / 2.0f)
                        {
                            mc.IsDragged = false;
                            PressureTime = 0.0f;
                            if (mc.IsSwipe)
                            {
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
                    }
                    if (touch.State == TouchLocationState.Released)
                    {
                        if (mc.IsDragged)
                        {
                            mc.IsDragged = false;
                            PressureTime = 0.0f;
                            //mc.IsSwipe = true;
                            mc.Velc = Vector2.Zero;
                            if(Vector2.Distance(mc.Position, PivotCenter) < RopeLength && mc.IsSwipe)
                            {
                                TouchEnd = touch.Position;
                                mc.IsDragged = false;
                                TouchDirection = Vector2.Normalize(Vector2.Subtract(TouchEnd, TouchStart));
                                if (TouchEnd == TouchStart)
                                {
                                    TouchDirection = Vector2.Zero;
                                }
                                //speed due to distance between two points.
                                float speed = Vector2.Distance(TouchEnd, TouchStart);
                                mc.Velc = TouchDirection * speed;
                                mc.IsSwipe = false;
                            }
                        }
                        
                        //restart game
                        if (Levels == 2)
                            RestartGame();
                    }
                }
            }
            else
            {
                MouseState ms = Mouse.GetState();
                mc.Position = new Vector2(ms.Position.X, ms.Position.Y);
            }

            // TODO: Add your update logic here
            //test for physics system
            if(Levels == 0)
            {
                
                
            }
            else if(Levels == 1)
            {
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
                foreach (Collectible c in  ColleList)
                {
                    c.ColRadius = (ColleIndex[c.Type].Width + ColleIndex[c.Type].Height) / 4.0f;
                }
                //Collectibles move
                foreach (Collectible c in ColleList)
                {
                    c.Move(gameTime);
                }
                //Collectibles out of screen
                foreach (Collectible c in ColleList)
                {
                    if (c.Position.Y > ViewportHeight || c.Position.X > ViewportWidth)
                    {
                        ColleList.Remove(c);
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
                if(mc.IsDragged)
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
                }


                //hp drops over time
                mc.Lives -= (float)gameTime.ElapsedGameTime.TotalSeconds * DryoutSpeed;

                //Game over
                if (mc.Lives <= 0)
                {
                    Levels = 2;
                    
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
                        mc.Lives -= EnemyDamage[e.type];
                        if (mc.Lives <= 0)
                        {
                            //game over
                            Levels = 2;
                        }
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
                        break;
                    }
                }

                //Scores
                CurrentScore += (float)gameTime.ElapsedGameTime.TotalSeconds * 5.0f;

                if((int)CurrentScore > HighestScore)
                {
                    HighestScore = (int)CurrentScore;
                }
            }
            else if(Levels == 2)//game over
            {
                SetHighestScore();
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
            //Background
            spriteBatch.Begin();
            spriteBatch.Draw(background, new Rectangle(0, 0, GraphicsDevice.Viewport.Width, GraphicsDevice.Viewport.Height), Color.White);
            spriteBatch.End();
            //Rope
            if (Levels == 0)
            {
                spriteBatch.Begin();

                foreach (RopeUnit r in Rope)
                {
                    spriteBatch.Draw(RopeTex, r.Position - new Vector2(RopeTex.Width / 2.0f, RopeTex.Height / 2.0f), Color.White);
                }
                spriteBatch.Draw(mc.Texture, mc.Position - new Vector2(MainCha.Width / 2.0f, MainCha.Height / 2.0f), Color.White);
                spriteBatch.Draw(OobiPupil, mc.Position - new Vector2(OobiPupil.Width / 2.0f, OobiPupil.Height / 2.0f), Color.White);
                spriteBatch.Draw(OobiTop, mc.Position - new Vector2(OobiTop.Width / 2.0f, OobiTop.Height / 2.0f), Color.White);

                spriteBatch.End();

            }
            else if (Levels == 1)
            {
                //Pivot & Line
                spriteBatch.Begin();
                //Rope Units
                foreach (RopeUnit r in Rope)
                {
                    spriteBatch.Draw(RopeTex, r.Position - new Vector2(RopeTex.Width / 2.0f, RopeTex.Height / 2.0f), Color.White);
                }
                //spriteBatch.Draw(background, new Rectangle(0, 0, GraphicsDevice.Viewport.Width, GraphicsDevice.Viewport.Height), Color.White);

                spriteBatch.Draw(pivotBase, PivotCenter - new Vector2(pivotBase.Width / 2.0f, pivotBase.Height / 2.0f), Color.White);

                spriteBatch.Draw(pivot, PivotCenter - new Vector2(pivot.Width / 2.0f, pivot.Height / 2.0f), Color.White);

                //Draw Line
                DrawLine(spriteBatch, Line, PivotCenter, mc.Position, 10);
                
                spriteBatch.End();

                //Enemy & Collectible
                spriteBatch.Begin();
                //Oobi
                spriteBatch.Draw(mc.Texture, mc.Position - new Vector2(MainCha.Width / 2.0f, MainCha.Height / 2.0f), Color.White);
                spriteBatch.Draw(OobiPupil, mc.Position - new Vector2(OobiPupil.Width / 2.0f, OobiPupil.Height / 2.0f), Color.White);
                spriteBatch.Draw(OobiTop, mc.Position - new Vector2(OobiTop.Width / 2.0f, OobiTop.Height / 2.0f), Color.White);
                foreach (Enemy e in EnemyList)
                {
                    e.EnemySize = new Vector2(EnemyIndex[e.type].Width, EnemyIndex[e.type].Height); //actually redundant
                    e.ColRadius = (EnemyIndex[e.type].Width + EnemyIndex[e.type].Height) / 8.0f; //give collison box size
                    spriteBatch.Draw(EnemyIndex[e.type], e.position - e.EnemySize / 2.0f, Color.White);
                }
                foreach (Collectible c in ColleList)
                {
                    c.ColRadius = (ColleIndex[c.Type].Width + ColleIndex[c.Type].Height) / 8.0f;//give collison box size
                    spriteBatch.Draw(ColleIndex[c.Type], c.Position - new Vector2(ColleIndex[c.Type].Width, ColleIndex[c.Type].Height) / 2.0f, Color.White);
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
                // Draw Scores
                spriteBatch.Begin();
                spriteBatch.DrawString(GameoverFont, "Current Score: " + ((int)CurrentScore).ToString(), new Vector2(7.0f * ViewportWidth / 10.0f, 10.0f), Color.Blue);
                spriteBatch.DrawString(GameoverFont, "Highest Score: " + HighestScore.ToString(), new Vector2(7.0f * ViewportWidth / 10.0f, 30.0f), Color.Blue);

                spriteBatch.End();
            }

            else if (Levels == 2)//Game over
            {
                spriteBatch.Begin();
                spriteBatch.DrawString(GameoverFont, "YOU DIED", new Vector2(ViewportWidth / 2.0f - GameoverFont.Texture.Width * 1.5f, ViewportHeight / 2.0f), Color.Red, 0, Vector2.Zero, 5, SpriteEffects.None, 0.1f);
                spriteBatch.DrawString(GameoverFont, "Tap to restart", new Vector2(ViewportWidth / 2.0f - GameoverFont.Texture.Width * 1.5f, 3.0f * ViewportHeight / 4.0f), Color.Red, 0, Vector2.Zero, 5, SpriteEffects.None, 0.1f);

                spriteBatch.End();
            }
            //else
            //{

            //}
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
    }
}
