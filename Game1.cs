using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Input.Touch;
using OobiMobile.Classes;
using System.Collections.Generic;

namespace OobiMobile
{
    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    public class Game1 : Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        Texture2D MainCha, background, pivot, Heart;
        List<Texture2D> EnemyIndex;
        List<Texture2D> ColleIndex;
        List<Enemy> EnemyList;
        List<EnemyGenerator> EnemyGenList;
        List<Collectible> ColleList;
        List<CollectibleGenerator> ColleGenList;

        int ViewportWidth, ViewportHeight;
        float PressureTime, DryTime;
        float PressureTimeLimit, DryTimeLimit;
        Vector2 TouchStart, TouchEnd, TouchDirection;
        Vector2 PivotCenter;
        MainCharacter mc;

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";

            graphics.IsFullScreen = true;
            //graphics.PreferredBackBufferWidth = 800;
            //graphics.PreferredBackBufferHeight = 480;
            graphics.SupportedOrientations = DisplayOrientation.LandscapeLeft | DisplayOrientation.LandscapeRight;
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
            ViewportWidth = GraphicsDevice.Viewport.Width;
            ViewportHeight = GraphicsDevice.Viewport.Height;
            PressureTime = 0.0f;
            PressureTimeLimit = 1.0f;
            DryTime = 0.0f;
            DryTimeLimit = 3.0f;
            
            
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
            EnemyIndex.Add(Content.Load<Texture2D>("Bee_Placeholder"));
            EnemyIndex.Add(Content.Load<Texture2D>("Tack_1_Placeholder"));
            EnemyIndex.Add(Content.Load<Texture2D>("Tack_2_Placeholder"));
            EnemyIndex.Add(Content.Load<Texture2D>("Tack_3_Placeholder"));
            EnemyIndex.Add(Content.Load<Texture2D>("Tack_4_Placeholder"));
            EnemyIndex.Add(Content.Load<Texture2D>("Tack_5_Placeholder"));

            ColleIndex.Add(Content.Load<Texture2D>("Raindrop_Placeholder"));
            


            PivotCenter = new Vector2(GraphicsDevice.Viewport.Width / 2, GraphicsDevice.Viewport.Height - GraphicsDevice.Viewport.Width / 2 - pivot.Height / 2);

            mc = new MainCharacter(MainCha, PivotCenter, Vector2.Zero, 3);
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
                        if(Vector2.Distance(touch.Position, Vector2.Add(mc.Position, new Vector2(mc.Texture.Width / 2.0f, mc.Texture.Height / 2.0f))) <= mc.ColRadius)
                        {
                            TouchStart = touch.Position;
                            mc.IsDragged = true;
                            mc.Velc = Vector2.Zero;
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
                            //speed due to distance between two points.
                            float speed = Vector2.Distance(Vector2.Add(mc.Position, new Vector2(mc.Texture.Width / 2.0f, mc.Texture.Height / 2.0f)), TouchStart);
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
                    }
                }
            }
            else
            {
                MouseState ms = Mouse.GetState();
                mc.Position = new Vector2(ms.Position.X, ms.Position.Y);
            }

            // TODO: Add your update logic here
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
            //Time
            PressureTime += (float)gameTime.ElapsedGameTime.TotalSeconds;
            DryTime += (float)gameTime.ElapsedGameTime.TotalSeconds;
            if (PressureTime >= PressureTimeLimit) 
            {
                //explode
            }

            if (DryTime >= DryTimeLimit) 
            {
                //game over
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
                if (Vector2.Distance(Vector2.Add(mc.Position , new Vector2(mc.Texture.Width / 2.0f, mc.Texture.Height / 2.0f)), e.position) < e.ColRadius + mc.ColRadius)
                {
                    mc.Lives--;
                    if(mc.Lives <= 0)
                    {
                        //game over
                    }
                    EnemyList.Remove(e);
                    break;
                }
            }
            //collectible
            foreach (Collectible c in ColleList)
            {
                if ( Vector2.Distance(mc.Position + new Vector2(mc.Texture.Width / 2.0f, mc.Texture.Height / 2.0f), c.Position) < mc.ColRadius + c.ColRadius)
                {
                    mc.Lives++;
                    ColleList.Remove(c);
                    break;
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
            //Background & Pivot
            spriteBatch.Begin();

            spriteBatch.Draw(background, new Rectangle(0, 0, GraphicsDevice.Viewport.Width, GraphicsDevice.Viewport.Height), Color.White);
            spriteBatch.Draw(pivot, new Rectangle(GraphicsDevice.Viewport.Width / 2 - pivot.Width / 2, GraphicsDevice.Viewport.Height - GraphicsDevice.Viewport.Width / 2 - pivot.Height / 2, pivot.Width, pivot.Height), Color.White);

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
            if (mc.Lives < 0)
                mc.Lives = 0;
            for (int i = 0; i < mc.Lives; ++i)
            {
                spriteBatch.Draw(Heart, new Vector2(ViewportWidth / 20.0f + i * 1.2f * Heart.Width, ViewportHeight / 40.0f), Color.White);
            }
            spriteBatch.End();
            base.Draw(gameTime);
        }
    }
}
