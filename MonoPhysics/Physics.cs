using Box2DSharp.Collision.Shapes;
using Box2DSharp.Common;
using Box2DSharp.Dynamics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;

namespace MonoPhysics
{
    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    public class Physics : Game
    {
        #region Fields

        private Vector2 _ballPosition;
        private float _ballSpeed;
        private Texture2D _ballTexture;
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;

        private float BaseRadius = 10f;
        private World world;

        #endregion Fields

        #region Public Constructors

        /// <summary>
        /// The main constructor
        /// </summary>
        public Physics()
        {
            // Initialize the graphics device manager
            _graphics = new GraphicsDeviceManager(this);
            _graphics.PreferredBackBufferWidth = Program._config.Width;
            _graphics.PreferredBackBufferHeight = Program._config.Height;
            Window.Title = "Physics Demo in MonoGame";
            _graphics.ApplyChanges();

            // Load content
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
            // GRAVITY
            //world = new World(new System.Numerics.Vector2(0, -10));
            // NO GRAVITY
            world = new World(new System.Numerics.Vector2(0, 0));
            CreateBalls(800, 600);
        }

        #endregion Public Constructors

        #region Protected Methods

        /// <summary>
        /// This is called when the game should draw itself
        /// </summary>
        /// <param name="gameTime"></param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Microsoft.Xna.Framework.Color.CornflowerBlue);

            // TODO: Add your drawing code here
            _spriteBatch.Begin();
            _spriteBatch.Draw(
                _ballTexture,
                _ballPosition,
                null,
                Microsoft.Xna.Framework.Color.White,
                0f,
                new Vector2(_ballTexture.Width / 2, _ballTexture.Height / 2),
                Vector2.One,
                SpriteEffects.None,
                0f
            );
            _spriteBatch.End();

            base.Draw(gameTime);
        }

        /// <summary>
        /// Initializes the game
        /// </summary>
        protected override void Initialize()
        {
            // TODO: Add your initialization logic here
            _ballPosition = new Vector2(_graphics.PreferredBackBufferWidth / 2,
                                   _graphics.PreferredBackBufferHeight / 2);
            _ballSpeed = 100f;

            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load any assets
        /// </summary>
        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            // TODO: use this.Content to load your game content here
            _ballTexture = Content.Load<Texture2D>("Assets/Images/ball");
        }

        /// <summary>
        /// Update will be called once per frame and is the place to update any game logic
        /// </summary>
        /// <param name="gameTime"></param>
        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            // TODO: Add your update logic here
            float updatedBallSpeed = _ballSpeed * (float)gameTime.ElapsedGameTime.TotalSeconds;

            var kstate = Keyboard.GetState();

            if (kstate.IsKeyDown(Keys.Up))
            {
                _ballPosition.Y -= updatedBallSpeed;
            }

            if (kstate.IsKeyDown(Keys.Down))
            {
                _ballPosition.Y += updatedBallSpeed;
            }

            if (kstate.IsKeyDown(Keys.Left))
            {
                _ballPosition.X -= updatedBallSpeed;
            }

            if (kstate.IsKeyDown(Keys.Right))
            {
                _ballPosition.X += updatedBallSpeed;
            }

            if (_ballPosition.X > _graphics.PreferredBackBufferWidth - _ballTexture.Width / 2)
            {
                _ballPosition.X = _graphics.PreferredBackBufferWidth - _ballTexture.Width / 2;
            }
            else if (_ballPosition.X < _ballTexture.Width / 2)
            {
                _ballPosition.X = _ballTexture.Width / 2;
            }

            if (_ballPosition.Y > _graphics.PreferredBackBufferHeight - _ballTexture.Height / 2)
            {
                _ballPosition.Y = _graphics.PreferredBackBufferHeight - _ballTexture.Height / 2;
            }
            else if (_ballPosition.Y < _ballTexture.Height / 2)
            {
                _ballPosition.Y = _ballTexture.Height / 2;
            }
            base.Update(gameTime);
        }

        #endregion Protected Methods

        #region Private Methods

        private void CreateBalls(int clientwidth, int clientheight)
        {
            Random random = new Random();

            // Create 5 circles
            for (int i = 0; i < 5; i++)
            {
                float x = (float)(random.NextDouble() * clientwidth);
                float y = (float)(random.NextDouble() * clientheight);

                BodyDef bodyDef = new BodyDef();
                bodyDef.Position.Set(x / 64f, y / 64f); // Convert to meters

                CircleShape circleShape = new CircleShape() { Radius = BaseRadius / 64f };
                FixtureDef fixtureDef = new FixtureDef();
                fixtureDef.Shape = circleShape;

                Body body = world.CreateBody(bodyDef);
                body.CreateFixture(fixtureDef);
                body.BodyType = BodyType.DynamicBody;

                // Store the bodies
                CircleBody circle = new CircleBody(body, BaseRadius);
                Program._log.Debug($"[CREATE] New CircleBody[{i + 1}] at X:{circle.body.GetPosition().X} Y:{circle.body.GetPosition().Y} with r{circle.radius}");
            }
        }

        #endregion Private Methods

        #region Structs

        /// <summary>
        /// A struct to hold a circle body and its radius
        /// </summary>
        public struct CircleBody
        {
            #region Fields

            /// <summary>
            /// The body of the circle (for physics purposes)
            /// </summary>
            public Body body;

            /// <summary>
            /// The radius of the circle
            /// </summary>
            public float radius;

            #endregion Fields

            #region Public Constructors

            /// <summary>
            /// The main constructor for the CircleBody struct
            /// </summary>
            /// <param name="body">Contains the physics object</param>
            /// <param name="radius">The radius of the circle</param>
            public CircleBody(Body body, float radius)
            {
                this.body = body;
                this.radius = radius;
            }

            #endregion Public Constructors
        }

        #endregion Structs
    }
}