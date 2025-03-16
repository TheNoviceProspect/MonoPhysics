using Box2DSharp.Dynamics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace MonoPhysics
{
    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    public class Physics : Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;

        private Texture2D _ballTexture;
        private Vector2 _ballPosition;
        private float _ballSpeed;

        /// <summary>
        /// A struct to hold a circle body and its radius
        /// </summary>
        public struct CircleBody
        {
            /// <summary>
            /// The body of the circle (for physics purposes)
            /// </summary>
            public Body body;

            /// <summary>
            /// The radius of the circle
            /// </summary>
            public float radius;

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
        }

        /// <summary>
        /// The main constructor
        /// </summary>
        public Physics()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
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

        /// <summary>
        /// This is called when the game should draw itself
        /// </summary>
        /// <param name="gameTime"></param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            // TODO: Add your drawing code here
            _spriteBatch.Begin();
            _spriteBatch.Draw(
                _ballTexture,
                _ballPosition,
                null,
                Color.White,
                0f,
                new Vector2(_ballTexture.Width / 2, _ballTexture.Height / 2),
                Vector2.One,
                SpriteEffects.None,
                0f
            );
            _spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}