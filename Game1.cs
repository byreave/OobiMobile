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

        Texture2D MainCha;
        List<Texture2D> EnemyIndex;
        List<Texture2D> ColleIndex;
        List<Enemy> EnemyList;
        List<EnemyGenerator> EnemyGenList;
        Texture2D Rope;

        int ViewportWidth, ViewportHeight;
        MainCharacter mc;

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";

            graphics.IsFullScreen = true;
            graphics.PreferredBackBufferWidth = 800;
            graphics.PreferredBackBufferHeight = 480;
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
            MainCha = Content.Load<Texture2D>("MainCharacter");
            EnemyIndex.Add(Content.Load<Texture2D>("EnemyA"));
            EnemyIndex.Add(Content.Load<Texture2D>("EnemyB"));

            mc = new MainCharacter(MainCha, Vector2.Zero, Vector2.Zero);
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
                    if (touch.State != TouchLocationState.Moved)
                    {

                    }
                }
            }
            else
            {
                MouseState ms = Mouse.GetState();
                mc.position = new Vector2(ms.Position.X, ms.Position.Y);
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
            spriteBatch.Begin();
            spriteBatch.Draw(mc.texture, mc.position, Color.White);
            foreach (Enemy e in EnemyList)
            {
                spriteBatch.Draw(EnemyIndex[e.type], e.position, Color.White);
            }

            spriteBatch.End();
            base.Draw(gameTime);
        }
    }
}
