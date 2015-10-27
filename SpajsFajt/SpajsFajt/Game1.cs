using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace SpajsFajt
{
    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    public class Game1 : Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        static Camera2D camera;
        private bool host = true;
        private GameServer gameServer;
        private GameClient gameClient;
        private string remoteIP;
        private int remotePort;
        public static IFocus Focus { get; set; }
        public static Vector2 CameraPosition { get { return camera.Position; } }
        private bool toggleKeyUp;

        public static Vector2 ViewportSize { get { return new Vector2(640, 400); } }

        public Game1(string ip, int port, string name, bool host)
        {
            graphics = new GraphicsDeviceManager(this);
            
            
            Content.RootDirectory = "Content";
            camera = new Camera2D(new Point(640,400));
            this.host = host;
            
            remotePort = port;
            remoteIP = ip;
            if (this.host)
            {
                gameServer = new GameServer(port);
            }
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
            TextureManager.Load(Content,"textures.xml");
            camera.Init();
            gameClient = new GameClient();
            gameClient.Connect(remoteIP,remotePort);
            // TODO: use this.Content to load your game content here
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
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();
            if (camera.Focus != Focus)
                camera.Focus = Focus;
            if (Keyboard.GetState().IsKeyDown(Keys.F11) && toggleKeyUp)
            {
                graphics.IsFullScreen = !graphics.IsFullScreen;
                graphics.ApplyChanges();
                toggleKeyUp = false;
            }
            if (Keyboard.GetState().IsKeyUp(Keys.F11))
                toggleKeyUp = true;

            camera.Update(gameTime);
            // TODO: Add your update logic here
            if (gameServer != null)
                gameServer.Update(gameTime);
            gameClient.Update(gameTime);

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);
            spriteBatch.Begin(SpriteSortMode.FrontToBack,BlendState.AlphaBlend,SamplerState.PointClamp,DepthStencilState.Default,null,null,camera.TransformationMatrix);
            spriteBatch.DrawString(TextureManager.GameFont, "Hejsan", Vector2.Zero, Color.BlanchedAlmond);
            gameClient.Draw(spriteBatch);
            spriteBatch.End();
            // TODO: Add your drawing code here

            base.Draw(gameTime);
        }
    }
}
