using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Drawing;
using System.Diagnostics;
namespace _3D_Engine
{
    public class Game1 : Game
    {
        private SpriteBatch spriteBatch;
        private Device device;
        private readonly Mesh mesh = new Mesh("Cube", 8,12);
        private readonly Camera camera = new Camera();
        private const int WIDTH = 1900;
        private const int HEIGHT = 1000;
        private double frameRate;
        public Game1()
        {
            var graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            graphics.PreferredBackBufferWidth = WIDTH;
            graphics.PreferredBackBufferHeight = HEIGHT;
            IsMouseVisible = true;
        }

        protected override void Initialize()
        {
            // Choose the back buffer resolution here
            var bmp = new Bitmap(WIDTH, HEIGHT);
            device = new Device(bmp);

            mesh.Vertices[0] = new Vector3(-1, 1, 1);
            mesh.Vertices[1] = new Vector3(1, 1, 1);
            mesh.Vertices[2] = new Vector3(-1, -1, 1);
            mesh.Vertices[3] = new Vector3(1, -1, 1);
            mesh.Vertices[4] = new Vector3(-1, 1, -1);
            mesh.Vertices[5] = new Vector3(1, 1, -1);
            mesh.Vertices[6] = new Vector3(1, -1, -1);
            mesh.Vertices[7] = new Vector3(-1, -1, -1);

            mesh.Faces[0] = new Face { a = 0, b = 1, c = 2 };
            mesh.Faces[1] = new Face { a = 1, b = 2, c = 3 };
            mesh.Faces[2] = new Face { a = 1, b = 3, c = 6 };
            mesh.Faces[3] = new Face { a = 1, b = 5, c = 6 };
            mesh.Faces[4] = new Face { a = 0, b = 1, c = 4 };
            mesh.Faces[5] = new Face { a = 1, b = 4, c = 5 };

            mesh.Faces[6] = new Face { a = 2, b = 3, c = 7 };
            mesh.Faces[7] = new Face { a = 3, b = 6, c = 7 };
            mesh.Faces[8] = new Face { a = 0, b = 2, c = 7 };
            mesh.Faces[9] = new Face { a = 0, b = 4, c = 7 };
            mesh.Faces[10] = new Face { a = 4, b = 5, c = 6 };
            mesh.Faces[11] = new Face { a = 4, b = 6, c = 7 };


            camera.Position = new Vector3(0f, 0f, 10.0f);
            camera.Target = Vector3.Zero;
            

            base.Initialize();
        }

        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);
        }

        protected override void UnloadContent()
        {
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();
            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            frameRate =(1f / gameTime.ElapsedGameTime.TotalSeconds);
            Debug.WriteLine(frameRate);
            GraphicsDevice.Clear(Microsoft.Xna.Framework.Color.CornflowerBlue);
            CompositionTargetRendering();
            base.Draw(gameTime);
        }

        // Rendering loop handler
        private void CompositionTargetRendering()
        {
            device.Clear(255, 255, 255, 255);

            // rotating slightly the cube during each frame rendered
            mesh.Rotation = new Vector3(mesh.Rotation.X + 0.01f, mesh.Rotation.Y + 0.01f, mesh.Rotation.Z + 0.01f);

            // Doing the various matrix operations
            device.Render(camera, mesh);
            // Flushing the back buffer into the front buffer
            using (var stream = device.Present(spriteBatch))
            {
                using (var tex = Texture2D.FromStream(GraphicsDevice, stream))
                {
                    spriteBatch.Begin();
                    spriteBatch.Draw(tex, Vector2.Zero, Microsoft.Xna.Framework.Color.White);
                    spriteBatch.End();
                }
            }

        }
    }
}

/*[SharpDX.DXGI], ApiCode: [DXGI_ERROR_DEVICE_REMOVED/DeviceRemoved], Message: GPU-enhetsinstansen har försatts i väntetillstånd. Använd GetDeviceRemovedReason om du vill ta reda på vad du bör göra.*/
