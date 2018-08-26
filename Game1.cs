using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Input.Touch;
using Oobi.Classes;
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

        Texture2D MainCha, background, pivot;
        List<Texture2D> EnemyIndex;
        List<Texture2D> ColleIndex;
        List<Enemy> EnemyList;
        List<EnemyGenerator> EnemyGenList;

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
            ViewportWidth = GraphicsDevice.Viewport.Width;
            ViewportHeight = GraphicsDevice.Viewport.Height;
            PressureTime = 0.0f;
            PressureTimeLimit = 2.0f;
            DryTime = 0.0f;
            DryTimeLimit = 3.0f;
            
            
            // TODO: Add your initialization logic here
            int[] toe1 = { 0, 1 };
            EnemyGenerator eneGen = new EnemyGenerator(toe1, new Vector2(ViewportWidth / 2.0f, 0.0f), new Vector2(0, 100.0f));
            EnemyGenList.Add(eneGen);
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
            EnemyIndex.Add(Content.Load<Texture2D>("Bee_Placeholder"));
            EnemyIndex.Add(Content.Load<Texture2D>("Tack_1_Placeholder"));
            EnemyIndex.Add(Content.Load<Texture2D>("Tack_2_Placeholder"));
            EnemyIndex.Add(Content.Load<Texture2D>("Tack_3_Placeholder"));
            EnemyIndex.Add(Content.Load<Texture2D>("Tack_4_Placeholder"));
            EnemyIndex.Add(Content.Load<Texture2D>("Tack_5_Placeholder"));
            foreach (Enemy e in EnemyList)
            {
                e.ColRadius = (EnemyIndex[e.type].Width + EnemyIndex[e.type].Height) / 8.0f;
            }


            PivotCenter = new Vector2(GraphicsDevice.Viewport.Width / 2 - pivot.Width / 2, GraphicsDevice.Viewport.Height - GraphicsDevice.Viewport.Width / 2 - pivot.Height / 2);

            mc = new MainCharacter(MainCha, PivotCenter, Vector2.Zero);
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
                        if(Vector2.Distance(touch.Position, mc.Position) <= mc.ColRadius)
                        {
                            TouchStart = touch.Position;
                            mc.IsDragged = true;
                        }
                    }

                    if(touch.State == TouchLocationState.Moved && mc.IsDragged)
                    {
                        mc.Position = touch.Position;
                    }
                    if (touch.State == TouchLocationState.Released)
                    {
                        TouchEnd = touch.Position;
                        mc.IsDragged = false;
                        TouchDirection = Vector2.Normalize(Vector2.Subtract(TouchEnd, TouchStart));
                        //speed due to distance between two points.
                        float speed = Vector2.Distance(TouchEnd, TouchStart);
                        mc.Velc = TouchDirection * speed / 10.0f;
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
            foreach (Enemy e in EnemyList)
            {
                e.Move(gameTime);
            }
            //Time
            PressureTime += (float)gameTime.ElapsedGameTime.TotalSeconds;
            DryTime += (float)gameTime.ElapsedGameTime.TotalSeconds;
            if(PressureTime >= PressureTimeLimit)
            {
                //explode
            }

            if(DryTime >= DryTimeLimit)
            {
                //game over
            }

            //Main character move
            if (mc.IsDragged == false)
            {
                mc.Move(gameTime);
                mc.Gravity(500.0f, gameTime);
                mc.BorderCheck(GraphicsDevice.Viewport.Width, GraphicsDevice.Viewport.Height, ViewportWidth / 2.0f, PivotCenter);
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

            spriteBatch.Begin();
            spriteBatch.Draw(mc.Texture, mc.Position, Color.White);
            foreach (Enemy e in EnemyList)
            {
                spriteBatch.Draw(EnemyIndex[e.type], e.position, Color.White);
            }

            spriteBatch.End();
            base.Draw(gameTime);
        }
    }
}
