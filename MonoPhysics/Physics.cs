using Box2DSharp.Collision.Shapes;
using Box2DSharp.Common;
using Box2DSharp.Dynamics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;

namespace MonoPhysics
{
    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    public class Physics : Game
    {
        #region Fields

        private Texture2D _ballTexture;
        private Texture2D _boundTexture;
        private List<CircleBody> _circleBodies;
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
            _circleBodies = new List<CircleBody>();
            CreateBounds();
            CreateBalls();
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
            foreach (var circle in _circleBodies)
            {
                var position = circle.body.GetPosition();
                var screenPosition = new Vector2(position.X * 64f, position.Y * 64f); // Convert to pixels
                _spriteBatch.Draw(
                    _ballTexture,
                    screenPosition,
                    null,
                    Microsoft.Xna.Framework.Color.White,
                    0f,
                    new Vector2(_ballTexture.Width / 2, _ballTexture.Height / 2),
                    Vector2.One,
                    SpriteEffects.None,
                    0f
                );
            }
            _spriteBatch.End();

            base.Draw(gameTime);
        }

        /// <summary>
        /// Initializes the game
        /// </summary>
        protected override void Initialize()
        {
            // TODO: Add your initialization logic here

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

            // Create a 1x1 pixel texture for drawing bounds
            _boundTexture = new Texture2D(GraphicsDevice, 1, 1);
            _boundTexture.SetData(new[] { Microsoft.Xna.Framework.Color.White });
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

            world.Step((float)gameTime.ElapsedGameTime.TotalSeconds, 8, 3);
            base.Update(gameTime);
        }

        #endregion Protected Methods

        #region Private Methods

        private void CreateBalls()
        {
            Random random = new Random();

            // Create 5 circles
            for (int i = 0; i < 5; i++)
            {
                float x = (float)(random.NextDouble() * Program._config.Width);
                float y = (float)(random.NextDouble() * Program._config.Height);

                BodyDef bodyDef = new BodyDef();
                bodyDef.Position.Set(x / 64f, y / 64f); // Convert to meters

                CircleShape circleShape = new CircleShape() { Radius = BaseRadius / 64f };
                FixtureDef fixtureDef = new FixtureDef();
                fixtureDef.Shape = circleShape;

                Body body = world.CreateBody(bodyDef);
                body.CreateFixture(fixtureDef);
                body.BodyType = BodyType.DynamicBody;

                // Set random initial velocity
                float velocityX = (float)(random.NextDouble() * 2 - 1); // Random value between -1 and 1
                float velocityY = (float)(random.NextDouble() * 2 - 1); // Random value between -1 and 1
                body.SetLinearVelocity(new System.Numerics.Vector2(velocityX, velocityY));

                // Store the bodies
                CircleBody circle = new CircleBody(body, BaseRadius);
                _circleBodies.Add(circle);
                Program._log.Debug($"[CREATE] New CircleBody[{i + 1}] at X:{circle.body.GetPosition().X} Y:{circle.body.GetPosition().Y} with r{circle.radius}");
            }
        }

        private void CreateBounds()
        {
            float width = Program._config.Width / 64f; // Convert to meters
            float height = Program._config.Height / 64f; // Convert to meters

            // Create ground
            BodyDef groundBodyDef = new BodyDef();
            groundBodyDef.Position.Set(0, height);
            Body groundBody = world.CreateBody(groundBodyDef);
            EdgeShape groundBox = new EdgeShape();
            groundBox.SetTwoSided(new System.Numerics.Vector2(0, 0), new System.Numerics.Vector2(width, 0));
            groundBody.CreateFixture(groundBox, 0);

            // Create ceiling
            BodyDef ceilingBodyDef = new BodyDef();
            ceilingBodyDef.Position.Set(0, 0);
            Body ceilingBody = world.CreateBody(ceilingBodyDef);
            EdgeShape ceilingBox = new EdgeShape();
            ceilingBox.SetTwoSided(new System.Numerics.Vector2(0, 0), new System.Numerics.Vector2(width, 0));
            ceilingBody.CreateFixture(ceilingBox, 0);

            // Create left wall
            BodyDef leftWallBodyDef = new BodyDef();
            leftWallBodyDef.Position.Set(0, 0);
            Body leftWallBody = world.CreateBody(leftWallBodyDef);
            EdgeShape leftWallBox = new EdgeShape();
            leftWallBox.SetTwoSided(new System.Numerics.Vector2(0, 0), new System.Numerics.Vector2(width, 0));
            leftWallBody.CreateFixture(leftWallBox, 0);

            // Create right wall
            BodyDef rightWallBodyDef = new BodyDef();
            rightWallBodyDef.Position.Set(width, 0);
            Body rightWallBody = world.CreateBody(rightWallBodyDef);
            EdgeShape rightWallBox = new EdgeShape();
            rightWallBox.SetTwoSided(new System.Numerics.Vector2(0, 0), new System.Numerics.Vector2(width, 0));
            rightWallBody.CreateFixture(rightWallBox, 0);
            Program._log.Debug($"[CREATE] Bounds created: Ground, Ceiling, Left Wall, Right Wall");
        }

        private void DrawBounds()
        {
            float width = Program._config.Width;
            float height = Program._config.Height;

            // Draw ground
            _spriteBatch.Draw(_boundTexture, new Rectangle(0, (int)height - 1, (int)width, 1), Microsoft.Xna.Framework.Color.Red);

            // Draw ceiling
            _spriteBatch.Draw(_boundTexture, new Rectangle(0, 0, (int)width, 1), Microsoft.Xna.Framework.Color.Red);

            // Draw left wall
            _spriteBatch.Draw(_boundTexture, new Rectangle(0, 0, 1, (int)height), Microsoft.Xna.Framework.Color.Red);

            // Draw right wall
            _spriteBatch.Draw(_boundTexture, new Rectangle((int)width - 1, 0, 1, (int)height), Microsoft.Xna.Framework.Color.Red);
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