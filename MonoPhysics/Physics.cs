using Box2DSharp.Collision.Shapes;
using Box2DSharp.Common;
using Box2DSharp.Dynamics;
using ImGuiNET;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using Num = System.Numerics;

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
        private ImGuiRenderer _imGuiRenderer;
        private IntPtr _imGuiTexture;
        private SpriteBatch _spriteBatch;
        private byte[] _textBuffer = new byte[100];
        private Texture2D _xnaTexture;
        private float BaseRadius = 10f;
        private Num.Vector3 clear_color = new Num.Vector3(114f / 255f, 144f / 255f, 154f / 255f);

        // Direct port of the example at https://github.com/ocornut/imgui/blob/master/examples/sdl_opengl2_example/main.cpp
        private float f = 0.0f;

        private bool show_details_window = false;
        private bool show_test_window = false;
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

        #region Public Methods

        /// <summary>
        /// This creates a new monogame texture from a given function
        /// </summary>
        /// <param name="device"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <param name="paint"></param>
        /// <returns></returns>
        public static Texture2D CreateTexture(GraphicsDevice device, int width, int height, Func<int, Microsoft.Xna.Framework.Color> paint)
        {
            //initialize a texture
            var texture = new Texture2D(device, width, height);

            //the array holds the color for each pixel in the texture
            Microsoft.Xna.Framework.Color[] data = new Microsoft.Xna.Framework.Color[width * height];
            for (var pixel = 0; pixel < data.Length; pixel++)
            {
                //the function applies the color according to the specified pixel
                data[pixel] = paint(pixel);
            }

            //set the color
            texture.SetData(data);

            return texture;
        }

        #endregion Public Methods

        #region Protected Methods

        /// <summary>
        /// This is called when the game should draw itself
        /// </summary>
        /// <param name="gameTime"></param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Microsoft.Xna.Framework.Color.CornflowerBlue);
            DrawBounds();
            DrawBalls();
            // TODO: Add your drawing code here

            // Call BeforeLayout first to set things up
            _imGuiRenderer.BeforeLayout(gameTime);

            // Draw our UI
            ImGuiLayout();

            // Call AfterLayout now to finish up and draw all the things
            _imGuiRenderer.AfterLayout();

            base.Draw(gameTime);
        }

        /// <summary>
        /// Some default layout in ImGui, just to show how it works.
        /// </summary>
        protected virtual void ImGuiLayout()
        {
            // 1. Show a simple window
            // Tip: if we don't call ImGui.Begin()/ImGui.End() the widgets appears in a window automatically called "Debug"
            {
                ImGui.Text("Hello, world!");
                ImGui.SliderFloat("float", ref f, 0.0f, 1.0f, string.Empty);
                ImGui.ColorEdit3("clear color", ref clear_color);
                if (ImGui.Button("Test Window")) show_test_window = !show_test_window;
                if (ImGui.Button("Another Window")) show_details_window = !show_details_window;
                ImGui.Text(string.Format("Application average {0:F3} ms/frame ({1:F1} FPS)", 1000f / ImGui.GetIO().Framerate, ImGui.GetIO().Framerate));

                ImGui.InputText("Text input", _textBuffer, 100);

                ImGui.Text("Texture sample");
                ImGui.Image(_imGuiTexture, new Num.Vector2(300, 150), Num.Vector2.Zero, Num.Vector2.One, Num.Vector4.One, Num.Vector4.One); // Here, the previously loaded texture is used
            }

            // 2. Show another simple window, this time using an explicit Begin/End pair
            if (show_details_window)
            {
                ImGui.SetNextWindowSize(new Num.Vector2(300, 400), ImGuiCond.FirstUseEver);
                ImGui.Begin("Physics Bodies Info", ref show_details_window);

                if (ImGui.CollapsingHeader("Boundary Boxes"))
                {
                    var body = world.BodyList.First;
                    if (ImGui.TreeNode("Ground"))
                    {
                        var pos = body.Value.GetPosition();
                        ImGui.Text($"Position: X={pos.X:F2}, Y={pos.Y:F2}");
                        ImGui.Text($"Width: {Program._config.Width / 64f:F2}m");
                        ImGui.TreePop();
                    }

                    body = body.Next;
                    if (ImGui.TreeNode("Ceiling"))
                    {
                        var pos = body.Value.GetPosition();
                        ImGui.Text($"Position: X={pos.X:F2}, Y={pos.Y:F2}");
                        ImGui.Text($"Width: {Program._config.Width / 64f:F2}m");
                        ImGui.TreePop();
                    }

                    body = body.Next;
                    if (ImGui.TreeNode("Left Wall"))
                    {
                        var pos = body.Value.GetPosition();
                        ImGui.Text($"Position: X={pos.X:F2}, Y={pos.Y:F2}");
                        ImGui.Text($"Height: {Program._config.Height / 64f:F2}m");
                        ImGui.TreePop();
                    }

                    body = body.Next;
                    if (ImGui.TreeNode("Right Wall"))
                    {
                        var pos = body.Value.GetPosition();
                        ImGui.Text($"Position: X={pos.X:F2}, Y={pos.Y:F2}");
                        ImGui.Text($"Height: {Program._config.Height / 64f:F2}m");
                        ImGui.TreePop();
                    }
                }

                if (ImGui.CollapsingHeader("Dynamic Bodies"))
                {
                    for (int i = 0; i < _circleBodies.Count; i++)
                    {
                        var circle = _circleBodies[i];
                        var position = circle.body.GetPosition();
                        var velocity = circle.body.LinearVelocity;

                        if (ImGui.TreeNode($"Ball {i + 1}"))
                        {
                            ImGui.Text($"Position: X={position.X:F2}, Y={position.Y:F2}");
                            ImGui.Text($"Velocity: X={velocity.X:F2}, Y={velocity.Y:F2}");
                            ImGui.Text($"Radius: {circle.radius:F2}");
                            ImGui.Text($"Body Type: {circle.body.BodyType}");
                            ImGui.TreePop();
                        }
                    }
                }

                ImGui.End();
            }

            // 3. Show the ImGui test window. Most of the sample code is in ImGui.ShowTestWindow()
            if (show_test_window)
            {
                ImGui.SetNextWindowPos(new Num.Vector2(650, 20), ImGuiCond.FirstUseEver);
                ImGui.ShowDemoWindow(ref show_test_window);
            }
        }

        /// <summary>
        /// Initializes the game
        /// </summary>
        protected override void Initialize()
        {
            // TODO: Add your initialization logic here
            _imGuiRenderer = new ImGuiRenderer(this);
            _imGuiRenderer.RebuildFontAtlas();

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

            // First, load the texture as a Texture2D (can also be done using the XNA/FNA content pipeline)
            _xnaTexture = CreateTexture(GraphicsDevice, 300, 150, pixel =>
            {
                var red = (pixel % 300) / 2;
                return new Microsoft.Xna.Framework.Color(red, 1, 1);
            });

            // Then, bind it to an ImGui-friendly pointer, that we can use during regular ImGui.** calls (see below)
            _imGuiTexture = _imGuiRenderer.BindTexture(_xnaTexture);
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
            float padding = Program._config.BoundaryPadding;
            float minX = padding;
            float maxX = Program._config.Width - padding;
            float minY = padding;
            float maxY = Program._config.Height - padding;

            // Create 5 circles
            for (int i = 0; i < 5; i++)
            {
                float x = minX + (float)(random.NextDouble() * (maxX - minX));
                float y = minY + (float)(random.NextDouble() * (maxY - minY));

                BodyDef bodyDef = new BodyDef();
                bodyDef.Position.Set(x / 64f, y / 64f); // Convert to meters

                CircleShape circleShape = new CircleShape() { Radius = BaseRadius / 64f };
                FixtureDef fixtureDef = new FixtureDef();
                fixtureDef.Shape = circleShape;

                Body body = world.CreateBody(bodyDef);
                body.CreateFixture(fixtureDef);
                body.BodyType = BodyType.DynamicBody;

                float velocityX = (float)(random.NextDouble() * 2 - 1);
                float velocityY = (float)(random.NextDouble() * 2 - 1);
                body.SetLinearVelocity(new System.Numerics.Vector2(velocityX, velocityY));

                CircleBody circle = new CircleBody(body, BaseRadius);
                _circleBodies.Add(circle);
                Program._log.Debug($"[CREATE] New CircleBody[{i + 1}] at X:{circle.body.GetPosition().X} Y:{circle.body.GetPosition().Y} with r{circle.radius}");
            }
        }

        private void CreateBounds()
        {
            float padding = Program._config.BoundaryPadding / 64f; // Convert to meters
            float width = (Program._config.Width / 64f) - (padding * 2);
            float height = (Program._config.Height / 64f) - (padding * 2);

            // Ground (bottom wall) - horizontal
            BodyDef groundBodyDef = new BodyDef();
            groundBodyDef.Position.Set(padding, height);
            Body groundBody = world.CreateBody(groundBodyDef);
            EdgeShape groundBox = new EdgeShape();
            groundBox.SetTwoSided(new System.Numerics.Vector2(0, 0), new System.Numerics.Vector2(width, 0));
            groundBody.CreateFixture(groundBox, 0);

            // Ceiling (top wall) - horizontal
            BodyDef ceilingBodyDef = new BodyDef();
            ceilingBodyDef.Position.Set(padding, padding);
            Body ceilingBody = world.CreateBody(ceilingBodyDef);
            EdgeShape ceilingBox = new EdgeShape();
            ceilingBox.SetTwoSided(new System.Numerics.Vector2(0, 0), new System.Numerics.Vector2(width, 0));
            ceilingBody.CreateFixture(ceilingBox, 0);

            // Left wall - vertical (now exactly between top and bottom)
            BodyDef leftWallBodyDef = new BodyDef();
            leftWallBodyDef.Position.Set(padding, padding);
            Body leftWallBody = world.CreateBody(leftWallBodyDef);
            EdgeShape leftWallBox = new EdgeShape();
            leftWallBox.SetTwoSided(new System.Numerics.Vector2(0, 0), new System.Numerics.Vector2(0, height - padding));
            leftWallBody.CreateFixture(leftWallBox, 0);

            // Right wall - vertical (now exactly between top and bottom)
            BodyDef rightWallBodyDef = new BodyDef();
            rightWallBodyDef.Position.Set(width + padding, padding);
            Body rightWallBody = world.CreateBody(rightWallBodyDef);
            EdgeShape rightWallBox = new EdgeShape();
            rightWallBox.SetTwoSided(new System.Numerics.Vector2(0, 0), new System.Numerics.Vector2(0, height - padding));
            rightWallBody.CreateFixture(rightWallBox, 0);
        }

        private void DrawBalls()
        {
            var currentNode = world.BodyList.First;

            // Skip the four boundary bodies
            for (int i = 0; i < 4; i++)
            {
                currentNode = currentNode.Next;
            }
            _spriteBatch.Begin();
            while (currentNode != null)
            {
                var currentBody = currentNode.Value;
                var fixture = currentBody.FixtureList.FirstOrDefault();
                if (fixture?.Shape is CircleShape circle)
                {
                    var position = currentBody.GetPosition();
                    var screenPosition = new Vector2(position.X * 64f, position.Y * 64f);

                    _spriteBatch.Draw(
                        _ballTexture,
                        screenPosition,
                        null,
                        Microsoft.Xna.Framework.Color.White,
                        currentBody.GetAngle(),
                        new Vector2(_ballTexture.Width / 2, _ballTexture.Height / 2),
                        Vector2.One,
                        SpriteEffects.None,
                        0f
                    );
                }
                currentNode = currentNode.Next;
            }
            _spriteBatch.End();
        }

        private void DrawBounds()
        {
            int thickness = 2;
            var currentNode = world.BodyList.First;

            _spriteBatch.Begin();
            while (currentNode != null)
            {
                var currentBody = currentNode.Value;
                // Get the fixture and its shape
                var fixture = currentBody.FixtureList.FirstOrDefault();
                if (fixture?.Shape is EdgeShape edge)
                {
                    // Convert Box2D coordinates to screen coordinates (multiply by 64f)
                    var pos = currentBody.GetPosition();
                    var vertex1 = new Vector2(
                        (pos.X + edge.Vertex1.X) * 64f,
                        (pos.Y + edge.Vertex1.Y) * 64f
                    );
                    var vertex2 = new Vector2(
                        (pos.X + edge.Vertex2.X) * 64f,
                        (pos.Y + edge.Vertex2.Y) * 64f
                    );

                    _spriteBatch.DrawLine(vertex1, vertex2, Microsoft.Xna.Framework.Color.Red, thickness);
                }
                currentNode = currentNode.Next;
            }
            _spriteBatch.End();
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