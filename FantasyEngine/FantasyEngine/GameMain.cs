using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using FantasyEngine.Classes;

using System.Reflection;
using System.Collections;

namespace FantasyEngine
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class GameMain : Microsoft.Xna.Framework.Game
    {
        // Le mot commence par S, fini par X et a un E au milieu.
        // J'espère que tu ne penses pas croche.
        // Réponse : SquareEnix

        GraphicsDeviceManager graphics;
        protected static Rectangle defaultScissor;

        /// <summary>
        /// SpriteBatch to draw.
        /// </summary>
        public static SpriteBatch spriteBatch;
        protected static RasterizerState rastState = new RasterizerState();
        public static Matrix cameraMatrix = Matrix.Identity;
        private static Matrix _OldCameraMatrix = Matrix.Identity;
        /// <summary>
        /// The Matrix for the camera when the SpriteBatch began.
        /// </summary>
        public static Matrix OldCameraMatrix { get { return _OldCameraMatrix; } }

        /// <summary>
        /// Default font.
        /// </summary>
        public static SpriteFont font;
        /// <summary>
        /// Cursor for menus.
        /// </summary>
        public static Texture2D cursor;

        private bool showFps = false;


        public GameMain()
        {
            graphics = new GraphicsDeviceManager(this);
            graphics.PreferredBackBufferWidth = 640;
            graphics.PreferredBackBufferHeight = 480;

            rastState.ScissorTestEnable = true;

            Content.RootDirectory = "Content";
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
            defaultScissor = GraphicsDevice.ScissorRectangle;
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

#if DEBUG
            showFps = true;
#endif

            Scene.ChangeMainScene(new Title(this));

            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // TODO: use this.Content to load your game content here
            font = Content.Load<SpriteFont>(@"Images\font_normal");
            font.LineSpacing += 2;

            cursor = Content.Load<Texture2D>(@"Images\cursor");

            Classes.Window.Tileset = new Tileset(Content.Load<Texture2D>(@"Images\window_tileset"), 8, 8);

            // Load all items in the ItemManager
            FantasyEngineData.Items.ItemManager.Load(Content.Load<FantasyEngineData.Items.BaseItem[]>(@"Items\Items\Consumables"));
            FantasyEngineData.Items.ItemManager.Load(Content.Load<FantasyEngineData.Items.BaseItem[]>(@"Items\Weapons\Knives"));
            FantasyEngineData.Items.ItemManager.Load(Content.Load<FantasyEngineData.Items.BaseItem[]>(@"Items\Weapons\Swords"));

//#if DEBUG
//            // TESTS
//            //FantasyEngineData.Items.ItemManager.Load(new FantasyEngineData.Items.BaseItem[] { Content.Load<FantasyEngineData.Items.Weapon>(@"Items\Weapons\Sword") });
//            FantasyEngineData.Monster mob = Content.Load<FantasyEngineData.Monster>(@"Monsters\Goblin");
//            // TESTS
//#endif
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        //int fps = 0;
        //float total = 0;

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            // Allows the game to exit
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed
                || Keyboard.GetState(PlayerIndex.One).IsKeyDown(Keys.OemPipe))
                this.Exit();

            Input.UpdateInput(gameTime);

            if (Input.keyStateDown.IsKeyDown(Keys.F12))
                showFps = !showFps;

            if (Input.keyStateDown.IsKeyDown(Keys.F9))
                MediaPlayer.IsMuted = !MediaPlayer.IsMuted;

            //// The time since Update was called last
            //float elapsed = (float)gameTime.ElapsedGameTime.TotalSeconds;
            //total += elapsed;
            //if (total >= 1)
            //{
            //    System.Diagnostics.Debug.WriteLine(fps.ToString());
            //    fps = 0;
            //    total = 0;
            //}
            //fps += 1;

            // TODO: Add your update logic here

            base.Update(gameTime);
        }

        private int drawCount;
        private float drawTimer;
        private string drawString = "FPS: ";

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
#if DEBUG
            GraphicsDevice.Clear(Color.CornflowerBlue); //Debug color.
#else
            GraphicsDevice.Clear(Color.Black);
#endif

            drawCount++;
            drawTimer += (float)gameTime.ElapsedGameTime.TotalSeconds;
            if (drawTimer >= 1f)
            {
                drawTimer -= 1f;
                drawString = "FPS: " + drawCount;
                drawCount = 0;
            }

            // TODO: Add your drawing code here
            spriteBatch.Begin(0, null, null, null, rastState, null, cameraMatrix);
            _OldCameraMatrix = cameraMatrix;
            //spriteBatch.Begin();
            base.Draw(gameTime);
            spriteBatch.End();

            if (showFps)
            {
                spriteBatch.Begin();
                spriteBatch.DrawString(font, drawString, new Vector2(10f, 10f), Color.White);
                spriteBatch.End();
            }
        }

        /// <summary>
        /// Change the region where the drawing can occur.
        /// </summary>
        /// <param name="rectangle">Region where the drawing can occur</param>
        public static void Scissor(Rectangle rectangle)
        {
            spriteBatch.End();
            spriteBatch.GraphicsDevice.ScissorRectangle = rectangle;
            spriteBatch.Begin(0, null, null, null, rastState, null, cameraMatrix);
        }

        /// <summary>
        /// Return to default the region where the drawing can occur.
        /// </summary>
        public static void ScissorReset() { Scissor(defaultScissor); }
    }
}
