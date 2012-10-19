using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using FantasyEngine.Classes;
using FantasyEngine.Xna;
using FantasyEngineData.Entities;
using FantasyEngineData.Items;
using FantasyEngineData.Skills;

namespace FantasyEngine
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class GameMain : Game
    {
        // Le mot commence par S, fini par X et a un E au milieu.
        // J'espère que tu ne penses pas croche.
        // Réponse : SquareEnix

        #region Static fields

        /// <summary>
        /// SpriteBatch to draw.
        /// </summary>
        public static FantasyEngine.Xna.SpriteBatch spriteBatch;
        /// <summary>
        /// SpriteBatch to draw all the Graphic User Interface;
        /// </summary>
        public static FantasyEngine.Xna.SpriteBatch spriteBatchGUI;

        /// <summary>
        /// Default font at 12pt.
        /// </summary>
        public static SpriteFont font;
        /// <summary>
        /// Default font at 8pt.
        /// </summary>
        public static SpriteFont font8;
        /// <summary>
        /// Cursor for menus.
        /// </summary>
        public static Texture2D cursor;
        #endregion Static fields

        private bool showFps = false;
        private GraphicsDeviceManager graphics;

        public GameMain()
        {
            graphics = new GraphicsDeviceManager(this);
            graphics.PreferredBackBufferWidth = 640;
            graphics.PreferredBackBufferHeight = 480;
            graphics.ApplyChanges();

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
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new FantasyEngine.Xna.SpriteBatch(GraphicsDevice);
            spriteBatchGUI = new FantasyEngine.Xna.SpriteBatch(GraphicsDevice);

#if DEBUG
            showFps = true;
            MediaPlayer.IsMuted = true;
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
            font = Content.Load<SpriteFont>(@"Images\Menus\font_normal");
            font.LineSpacing += 2;

            font8 = Content.Load<SpriteFont>(@"Images\Menus\font_normal 8pt");
            font8.LineSpacing += 2;

            cursor = Content.Load<Texture2D>(@"Images\Menus\cursor");

            Classes.Window.Tileset = new Tileset(Content.Load<Texture2D>(@"Images\Menus\window_tileset"), 8, 8);

            Classes.Window.NextDialog = Content.Load<Texture2D>(@"Images\Menus\next_dialog");

            // Load all jobs in the JobManager
            JobManager.Load(Content.Load<BaseJob>(@"Jobs\OnionKid"));
            JobManager.Load(Content.Load<BaseJob>(@"Jobs\Soldier"));
            JobManager.Load(Content.Load<BaseJob>(@"Jobs\Warrior"));
            JobManager.Load(Content.Load<BaseJob>(@"Jobs\Archer"));
            JobManager.Load(Content.Load<BaseJob>(@"Jobs\Thief"));
            JobManager.Load(Content.Load<BaseJob>(@"Jobs\Black Mage"));
            JobManager.Load(Content.Load<BaseJob>(@"Jobs\White Mage"));
            JobManager.Load(Content.Load<BaseJob>(@"Jobs\Black Wizard"));

            // Load all items in the ItemManager
            ItemManager.Load(Content.Load<BaseItem[]>(@"Items\Items\Consumables"));
            ItemManager.Load(Content.Load<BaseItem[]>(@"Items\Items\Offensives"));
            ItemManager.Load(Content.Load<BaseItem[]>(@"Items\Weapons\Knives"));
            ItemManager.Load(Content.Load<BaseItem[]>(@"Items\Weapons\Swords"));
            ItemManager.Load(Content.Load<BaseItem[]>(@"Items\Armors\Heads"));
            ItemManager.Load(Content.Load<BaseItem[]>(@"Items\Armors\Armors"));
            ItemManager.Load(Content.Load<BaseItem[]>(@"Items\Armors\Arms"));
            ItemManager.Load(Content.Load<BaseItem[]>(@"Items\Armors\Feet"));
            ItemManager.Load(Content.Load<BaseItem[]>(@"Items\Armors\Shields"));

            // Load all skills in the SkillManager
            SkillManager.Load(Content.Load<BaseSkill[]>(@"Skills\White Magics"));
            SkillManager.Load(Content.Load<BaseSkill[]>(@"Skills\Black Magics"));
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
            spriteBatch.Begin();
            Scene.DrawAll(gameTime);
            spriteBatch.End();

            spriteBatchGUI.Begin();
            Scene.DrawAllGUI(gameTime);
            spriteBatchGUI.End();

            if (showFps)
            {
                spriteBatchGUI.BaseBegin();
                spriteBatchGUI.DrawString(font, drawString, new Vector2(10f, 10f), Color.White);
                spriteBatchGUI.End();
            }
        }
    }
}
