using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Input.Touch;
using OobiMobile.Classes;
using System.Collections.Generic;
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

        Texture2D MainCha, background, pivot, Heart, Line, RopeTex;
        SpriteFont GameoverFont;
        List<Texture2D> EnemyIndex;
        List<Texture2D> ColleIndex;
        List<Enemy> EnemyList;
        List<RopeUnit> Rope;
        List<EnemyGenerator> EnemyGenList;
        List<Collectible> ColleList;
        List<CollectibleGenerator> ColleGenList;
        List<int> EnemyDamage;

        int ViewportWidth, ViewportHeight, Levels, RopeUnitsNumber;
        float PressureTime, DryoutSpeed;
        float PressureTimeLimit;
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
            Levels = 0;//0 game start, 1 gaming, 2 game over;
            
            
            // TODO: Add your initialization logic here
            int[] toe1 = { 0, 1, 2, 3, 4 };
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
            MainCha = Content.Load<Texture2D>("Eye_Placeholder");
            background = Content.Load<Texture2D>("Background_Placeholder");
            pivot = Content.Load<Texture2D>("Pivot_Placeholder");
            Heart = Content.Load<Texture2D>("Heart");
            GameoverFont = Content.Load<SpriteFont>("gameover");
            RopeTex = Content.Load<Texture2D>("rope");
            EnemyIndex.Add(Content.Load<Texture2D>("Bee_Placeholder"));
            EnemyDamage.Add(20);
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
    
            ColleIndex.Add(Content.Load<Texture2D>("Raindrop_Placeholder"));

            // create 1x1 texture for line drawing
            Line = new Texture2D(GraphicsDevice, 1, 1);
            Line.SetData<Color>(
                new Color[] { Color.OrangeRed });// fill the texture with white


            PivotCenter = new Vector2(GraphicsDevice.Viewport.Width / 2, GraphicsDevice.Viewport.Height - GraphicsDevice.Viewport.Width / 2);

            mc = new MainCharacter(MainCha, PivotCenter, Vector2.Zero, MaxLives);
            Vector2 ropeUnitPos = new Vector2(PivotCenter.X, PivotCenter.Y + RopeTex.Height / 2.0f);
            for(int i = 0; i < RopeUnitsNumber; ++ i)
            {
                Rope.Add(new RopeUnit(ropeUnitPos + new Vector2(0.0f, i * RopeTex.Height), Vector2.Zero, (RopeTex.Width + RopeTex.Height) / 4.0f));
            }
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
                        if (Vector2.Distance(touch.Position, PivotCenter) > GraphicsDevice.Viewport.Width / 2.0f - mc.ColRadius)
                            continue;
                        if(Vector2.Distance(touch.Position, Vector2.Add(mc.Position, new Vector2(mc.Texture.Width / 2.0f, mc.Texture.Height / 2.0f))) <= mc.ColRadius * 2.0f)
                        {
                            TouchStart = touch.Position;
                            mc.IsDragged = true;
                            mc.Velc = Vector2.Zero;
                            PressureTime = 0.0f;
                        }
                    }

                    if(touch.State == TouchLocationState.Moved && mc.IsDragged)
                    {
                        if (Vector2.Distance(touch.Position, PivotCenter) < GraphicsDevice.Viewport.Width / 2.0f - mc.ColRadius)
                            mc.Position = Vector2.Subtract(touch.Position, new Vector2(mc.Texture.Width / 2.0f, mc.Texture.Height / 2.0f));
                        else
                        {
                            mc.IsDragged = false;
                            TouchDirection = Vector2.Normalize(Vector2.Subtract(Vector2.Add(mc.Position, new Vector2(mc.Texture.Width / 2.0f, mc.Texture.Height / 2.0f)), TouchStart));
                            if (Vector2.Add(mc.Position, new Vector2(mc.Texture.Width / 2.0f, mc.Texture.Height / 2.0f)) == TouchStart)
                            {
                                TouchDirection = Vector2.Zero;
                            }
                            //speed due to distance between two points.
                            float speed = Vector2.Distance(Vector2.Add(mc.Position, new Vector2(mc.Texture.Width / 2.0f, mc.Texture.Height / 2.0f)), TouchStart) * 2.0f;
                            mc.Velc = TouchDirection * speed;
                        }
                    }
                    if (touch.State == TouchLocationState.Released)
                    {
                        if(mc.IsDragged)
                        {
                            TouchEnd = touch.Position;
                            mc.IsDragged = false;
                            TouchDirection = Vector2.Normalize(Vector2.Subtract(TouchEnd, TouchStart));
                            if(TouchEnd == TouchStart)
                            {
                                TouchDirection = Vector2.Zero;
                            }
                            //speed due to distance between two points.
                            float speed = Vector2.Distance(TouchEnd, TouchStart);
                            mc.Velc = TouchDirection * speed;
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
                for(int i = 0; i < RopeUnitsNumber; ++ i)
                {
                    float tensionForcePrev = 0.0f, tensionForceXPrev = 0.0f, tensionForceYPrev = 0.0f;
                    float tensionForceNext = 0.0f, tensionForceXNext = 0.0f, tensionForceYNext = 0.0f;

                    if (i == 0)//first rope unit connects pivot
                    {
                        tensionForcePrev = (Vector2.Distance(PivotCenter, Rope[i].Position) - Rope[i].Radius - (pivot.Height+pivot.Width) / 2.0f) * PhysicsSystem.TensionK;
                        tensionForceXPrev = (Rope[i].Position.X - PivotCenter.X) / Vector2.Distance(Rope[i].Position, PivotCenter) * tensionForcePrev;
                        tensionForceYPrev = (Rope[i].Position.Y - PivotCenter.Y) / Vector2.Distance(Rope[i].Position, PivotCenter) * tensionForcePrev;
                    }
                    else
                    {
                        tensionForcePrev = (Vector2.Distance(Rope[i-1].Position, Rope[i].Position) - Rope[i].Radius - Rope[i-1].Radius) * PhysicsSystem.TensionK;
                        tensionForceXPrev = (Rope[i].Position.X - Rope[i - 1].Position.X) / Vector2.Distance(Rope[i].Position, Rope[i - 1].Position) * tensionForcePrev;
                        tensionForceYPrev = (Rope[i].Position.Y - Rope[i - 1].Position.Y) / Vector2.Distance(Rope[i].Position, Rope[i - 1].Position) * tensionForcePrev;
                    }

                    if(i == RopeUnitsNumber-1)//last rope unit connects oobi
                    {
                        tensionForceNext = (Vector2.Distance(Rope[i].Position, mc.Position + new Vector2(mc.Texture.Width / 2.0f, mc.Texture.Height / 2.0f)) - Rope[i].Radius - mc.ColRadius) * PhysicsSystem.TensionK;
                        tensionForceXNext = (mc.Position.X - Rope[i].Position.X - mc.Texture.Width / 2.0f) / Vector2.Distance(Rope[i].Position, mc.Position + new Vector2(mc.Texture.Width / 2.0f, mc.Texture.Height / 2.0f)) * tensionForceNext;
                        tensionForceYNext = (mc.Position.Y - Rope[i].Position.Y - mc.Texture.Height / 2.0f) / Vector2.Distance(Rope[i].Position, mc.Position + new Vector2(mc.Texture.Width / 2.0f, mc.Texture.Height / 2.0f)) * tensionForceNext;
                    }
                    else
                    {
                        tensionForceNext = (Vector2.Distance(Rope[i].Position, Rope[i+1].Position) - Rope[i].Radius - Rope[i + 1].Radius) * PhysicsSystem.TensionK;
                        tensionForceXNext = (Rope[i+1].Position.X - Rope[i].Position.X) / Vector2.Distance(Rope[i].Position, Rope[i + 1].Position) * tensionForceNext;
                        tensionForceYNext = (Rope[i+1].Position.Y - Rope[i].Position.Y) / Vector2.Distance(Rope[i].Position, Rope[i + 1].Position) * tensionForceNext;
                    }
                    Rope[i].PhysicsSystem.Force = new Vector2(tensionForceXPrev + tensionForceXNext, tensionForceYPrev + tensionForceYNext + Rope[i].PhysicsSystem.Mass * PhysicsSystem.GravAcc);
                    Rope[i].Speed += Rope[i].PhysicsSystem.Force / Rope[i].PhysicsSystem.Mass;
                }

                foreach (RopeUnit r in Rope)
                {
                    r.Move(gameTime);
                }
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
                //Time
                /*if(mc.IsDragged)
                    PressureTime += (float)gameTime.ElapsedGameTime.TotalSeconds;
                //DryTime += (float)gameTime.ElapsedGameTime.TotalSeconds;
                if (PressureTime >= PressureTimeLimit) 
                {
                    //explode, in this case hp - 10
                    mc.IsDragged = false;
                    mc.Velc = Vector2.Zero;
                    mc.Lives -= 10;
                    PressureTime = 0.0f;
                }*/


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
                    mc.Move(gameTime);
                    mc.Gravity(500.0f, gameTime);
                }
                mc.BorderCheck(GraphicsDevice.Viewport.Width, GraphicsDevice.Viewport.Height, ViewportWidth / 2.0f, PivotCenter);

                //Collision detection
                //enemy
                foreach (Enemy e in EnemyList)
                {
                    if (Vector2.Distance(Vector2.Add(mc.Position, new Vector2(mc.Texture.Width / 2.0f, mc.Texture.Height / 2.0f)), e.position) < e.ColRadius + mc.ColRadius)
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
                    if (Vector2.Distance(mc.Position + new Vector2(mc.Texture.Width / 2.0f, mc.Texture.Height / 2.0f), c.Position) < mc.ColRadius + c.ColRadius)
                    {
                        if (mc.Lives < MaxLives)
                            mc.Lives += 10;
                        ColleList.Remove(c);
                        break;
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
            //Background
            spriteBatch.Begin();
            spriteBatch.Draw(background, new Rectangle(0, 0, GraphicsDevice.Viewport.Width, GraphicsDevice.Viewport.Height), Color.White);
            spriteBatch.End();
            if (Levels == 0)
            {

            }
            else if (Levels == 1)
            {
                //Pivot & Line
                spriteBatch.Begin();
                //spriteBatch.Draw(background, new Rectangle(0, 0, GraphicsDevice.Viewport.Width, GraphicsDevice.Viewport.Height), Color.White);

                spriteBatch.Draw(pivot, new Rectangle(GraphicsDevice.Viewport.Width / 2 - pivot.Width / 2, GraphicsDevice.Viewport.Height - GraphicsDevice.Viewport.Width / 2 - pivot.Height / 2, pivot.Width, pivot.Height), Color.White);

                //Draw Line
                DrawLine(spriteBatch, Line, PivotCenter, mc.Position + new Vector2(mc.Texture.Width / 2.0f, mc.Texture.Height / 2.0f), 10);
                
                spriteBatch.End();

                //Enemy & Collectible
                spriteBatch.Begin();
                spriteBatch.Draw(mc.Texture, mc.Position, Color.White);
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
                for (int i = 0; i < mc.Lives / 10; ++i)
                {
                    spriteBatch.Draw(Heart, new Vector2(ViewportWidth / 20.0f + i * 1.2f * Heart.Width, ViewportHeight / 40.0f), Color.White);
                }
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
            mc.Position = PivotCenter;
            mc.Lives = MaxLives;
            EnemyList.Clear();
            ColleList.Clear();
            Levels = 1;
        }
    }
}
