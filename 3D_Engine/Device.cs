using System;
using System.Collections.Generic;
using System.Diagnostics;

using Microsoft.Xna.Framework;

using System.IO;

using Microsoft.Xna.Framework.Graphics;

using System.Drawing;
using System.Drawing.Imaging;
using System.Threading.Tasks;

using Color = System.Drawing.Color;


namespace _3D_Engine
{
    internal class Device
    {
        private readonly byte[] backBuffer;
        private readonly Bitmap bitMap;
        private List<Vector2> pointlist2 = new List<Vector2>();
        private const int WIDTH = 1900;
        private const int HEIGHT = 1000;

        public Device(Bitmap bitmap)
        {
            bitMap = bitmap;

            //backbuffersize is equal to the number of pixels to draw on the screen (width * height) * 4(RGBA).
            backBuffer = new byte[bitMap.Width * bitMap.Height * 4];
        }

        //method clears bitmap to a specific color
        //argb or brga??
        public void Clear(byte r, byte g, byte b, byte a)
        {
            for (int index = 0; index < backBuffer.Length; index += 4)
            {
                backBuffer[index] = b;
                backBuffer[index + 1] = r;
                backBuffer[index + 2] = g;
                backBuffer[index + 3] = a;
            }
        }

        //flusing the back buffer into the fron buffer
        public unsafe MemoryStream Present(SpriteBatch spriteBatch)
        {
            BitmapData bData = bitMap.LockBits(new System.Drawing.Rectangle(0, 0, bitMap.Width, bitMap.Height),
                                                ImageLockMode.ReadWrite, bitMap.PixelFormat);

            const byte bitsPerPixel = 32;

            /*This time we convert the IntPtr to a ptr*/
            byte* scan0 = (byte*)bData.Scan0.ToPointer();
            int z = 0;
            for (int i = 0; i < bData.Height; ++i)
            {
                for (int j = 0; j < bData.Width; ++j)
                {
                    byte* data = scan0 + i * bData.Stride + j * bitsPerPixel / 8;

                    //data is a pointer to the first byte of the 3-byte color data
                    data[0] = backBuffer[z + 1];
                    data[1] = backBuffer[z + 2];
                    data[2] = backBuffer[z + 3];
                    z += 4;
                }
            }

            bitMap.UnlockBits(bData);
            MemoryStream stream = new MemoryStream();
            bitMap.Save(stream, ImageFormat.Bmp);
            stream.Position = 0;

            return stream;

            #region old code
            //int z = 0;
            //for (int y = 0; y < _bitMap.Height; y++)
            //{
            //    for (int x = 0; x < _bitMap.Width; x++)
            //    {
            //        _bitMap.SetPixel(x, y, System.Drawing.Color.FromArgb(_backBuffer[z], _backBuffer[z + 1], _backBuffer[z + 2], _backBuffer[z + 3]));
            //        z += 4;
            //    }
            //}

            //var stream = new System.IO.MemoryStream();
            //_bitMap.Save(stream, System.Drawing.Imaging.ImageFormat.Bmp);
            //stream.Position = 0;

            ////var image = Bitmap.FromStream(stream);
            //return stream;

            //Texture2D tex=Texture2D.FromStream(graphicsDevice, stream);
            //spriteBatch.Draw(tex,Vector2.Zero,Microsoft.Xna.Framework.Color.White);
            //using (Graphics e = Graphics.FromImage(_bitMap))
            //{

            //    e.DrawImage(image, 0, 0, _bitMap.Width, _bitMap.Height);
            //    e.Dispose();
            //}
            //stream.Dispose();
            //image.Dispose();
            #endregion
        }

        //konstrura bitmap loopa setpixel

        // Called to put a pixel on screen at a specific X,Y coordinates
        public void PutPixel(int x, int y, System.Drawing.Color color)
        {
            // As we have a 1-D Array for our back buffer we need to know the equivalent cell in 1-D based on the 2D coordinates on screen
            int index = (y * WIDTH + x) * 4;
            if (index > backBuffer.Length - 5) { }
            else
            {
                backBuffer[index] = color.A;
                backBuffer[index + 1] = color.R;
                backBuffer[index + 2] = color.G;
                backBuffer[index + 3] = color.B;
            }
        }

        // Project takes some 3D coordinates and transform them
        // in 2D coordinates using the transformation matrix
        public Vector2 Project(Vector3 cordinates, Matrix transMat)
        {
            // transforming the coordinates
            Vector4 point = Vector4.Transform(cordinates, transMat);

            // point.W = 1 / cordinates.Z;
            // var x = 
            // The transformed coordinates will be based on coordinate system starting on the center of the screen. But drawing on screen normally starts from top left. 
            // We then need to transform them again to have x:0, y:0 on top left.
            float x = point.X / point.W;
            float y = point.Y / point.W;
            x = x * bitMap.Width / 4 + bitMap.Width / 2;
            y = y * bitMap.Height / 4 + bitMap.Height / 2;

            //var x = point.X * _bitMap.Width + _bitMap.Width / 2.0f;
            //var y = point.Y * _bitMap.Height + _bitMap.Height / 2.0f;
            return new Vector2(x, y);
        }

        // DrawPoint calls PutPixel but does the clipping operation before
        public void DrawPoint(Vector2 point, System.Drawing.Color color)
        {
            // Clipping what's visible on screen
            //if (point.X >= 0 && point.Y >= 0 && point.X < _bitMap.Width && point.Y < _bitMap.Height)
            //{
            // Drawing a white point
            PutPixel((int)point.X, (int)point.Y, color);

            //}
        }

        public void DrawLine(Vector2 point0, Vector2 point1)
        {
            //Recursion or iteration look over whats best.

            float dist = (point1 - point0).Length();

            // If the distance between the 2 points is less than 2 pixels
            // We're exiting
            if (dist < 2)
            {
                return;
            }

            // Find the middle point between first & second point
            Vector2 middlePoint = point0 + (point1 - point0) / 2;

            // We draw this point on screen
            DrawPoint(middlePoint, System.Drawing.Color.Black);

            // Recursive algorithm launched between first & middle point
            // and between middle & second point
            DrawLine(point0, middlePoint);
            DrawLine(middlePoint, point1);
        }

        public void DrawBline(Vector2 point0, Vector2 point1, Vector2 point3, int a, int b, int r, int g)
        {
            int x0 = (int)point0.X;
            int y0 = (int)point0.Y;
            int x1 = (int)point1.X;
            int y1 = (int)point1.Y;

            int dx = Math.Abs(x1 - x0);
            int dy = Math.Abs(y1 - y0);
            int sx = x0 < x1 ? 1 : -1;
            int sy = y0 < y1 ? 1 : -1;
            int err = dx - dy;
            List<Vector2> pointlist = new List<Vector2>();
            while (true)
            {
                //Enable Line bellow & disable FillTriangel() for WireFrame rendering mode
                //DrawPoint(new Vector2(x0, y0), System.Drawing.Color.FromArgb(a, r, g, b));
                pointlist.Add(new Vector2(x0, y0));
                if (x0 == x1 && y0 == y1)
                {
                    break;
                }

                int e2 = 2 * err;
                if (e2 > -dy)
                {
                    err -= dy;
                    x0 += sx;
                }

                if (e2 < dx)
                {
                    err += dx;
                    y0 += sy;
                }
            }

            //single thread
            //foreach (var vector3 in pointlist)
            //{
            //    FillTriangle(vector3, point3);
            //}

            //multi thread
            Parallel.ForEach(pointlist, (vector2) =>
            {
                FillTriangle(vector2, point3, a, r, g, b);
            });
        }

        // The main method of the engine that re-compute each vertex projection
        // during each frame
        /*
{X:-1482,765 Y:2932,765}
{X:-1482,765 Y:-1932,765}
{X:3382,765 Y:2932,765}
{X:3382,765 Y:-1932,765}
{X:-1482,765 Y:2932,765}
{X:-1482,765 Y:-1932,765}
{X:3382,765 Y:2932,765}
{X:3382,765 Y:-1932,765}
             */
        public void Render(Camera camera, params Mesh[] meshes)
        {
            //Matrix translationMatrix = Matrix;
            //Matrix scalingMatrix = Matrix;
            //Matrix rotationMatrix = Matrix;

            //Matrix projectionMatrix = Matrix;

            //Viewport clipCordinates;

            //Matrix Normalisaton;

            //Matrix WindowCordinates;

            //// Matrix view = translate * roll * heading * pitch
            //// Matrix model = rotZ * rotY * rotX * translate
            //// Matrix modelview = view*model

            //Matrix view_translate = Matrix.CreateTranslation(new Vector3(0, 0, -10));
            //Matrix view_roll = Matrix.CreateRotationZ(0);
            //Matrix view_pitch = Matrix.CreateRotationX(0);
            //Matrix view_heading = Matrix.CreateRotationY(0);

            //Matrix view = view_translate * view_roll * view_heading * view_pitch;


            //Matrix model_rotZ = Matrix.CreateRotationZ(0);
            //Matrix model_rotX = Matrix.CreateRotationX(0);
            //Matrix model_rotY = Matrix.CreateRotationY(0);
            //Matrix model_translate = Matrix.CreateTranslation(new Vector3(0, 0, 0));

            //Matrix model = model_rotZ * model_rotY * model_rotX * model_translate;

            //Matrix modelView = view * model;

            //Vector4 object_cordinates = new Vector4(0,0,0,0);

            //Vector4 eyeCordinates = Vector4.Transform(object_cordinates, modelView);

            //Matrix


            // Model --> world--> Viewport--> perspective--> Viewport 


            Vector3 a = new Vector3(0, 0, 0);
            Vector3 c = new Vector3(0, 0, 10);
            Matrix täta = Matrix.CreateFromYawPitchRoll(0, 0, 0);
            Vector3 d = Vector3.Transform(a - c, täta);


            // To understand this part, please read the prerequisites resources
            Matrix viewMatrix = Matrix.CreateTranslation(camera.Position) * Matrix.CreateFromYawPitchRoll(0, 0, 0);

            //var viewMatrix = Matrix.CreateLookAt(camera._Position, camera._Target, Vector3.UnitY);
            //var projectionMatrix = Matrix.CreatePerspectiveFieldOfView(0.78f, (float)_bitMap.Width / _bitMap.Height, 0.01f, 1.0f);
            foreach (Mesh mesh in meshes)
            {
                // Beware to apply rotation before translation 
                Matrix worldMatrix = Matrix.CreateTranslation(mesh.Position) *
                                     Matrix.CreateFromYawPitchRoll(mesh.Rotation.Y, mesh.Rotation.X, mesh.Rotation.Z);

                Matrix transformMatrix = /* projectionMatrix **/ worldMatrix * viewMatrix;
                int alpha = 0;
                int red = 0;
                int green = 0;
                int blue = 0;
                foreach (Face face in mesh.Faces)
                {

                    Vector3 vertexA = mesh.Vertices[face.a];
                    Vector3 vertexB = mesh.Vertices[face.b];
                    Vector3 vertexC = mesh.Vertices[face.c];

                    Vector2 pixelA = Project(vertexA, transformMatrix);
                    Vector2 pixelB = Project(vertexB, transformMatrix);
                    Vector2 pixelC = Project(vertexC, transformMatrix);

                    DrawBline(pixelA, pixelB, pixelC, alpha, blue, red, green);
                    DrawBline(pixelB, pixelC, pixelA, alpha, blue, red, green);
                    DrawBline(pixelC, pixelA, pixelB, alpha, blue, red, green);
                    alpha = alpha + 10;
                    red = red + 10;
                    green = green + 10;
                    blue = blue + 10;
                }

                //foreach (var vertex in mesh.Vertices)
                //{
                //    // First, we project the 3D coordinates into the 2D space
                //    var point = Project(vertex, transformMatrix);
                //    // Then we can draw on screen
                //    DrawPoint(point);
                //}
            }
        }

        private void FillTriangle(Vector2 point0, Vector2 point1, int a, int r, int g, int b)
        {
            int x0 = (int)point0.X;
            int y0 = (int)point0.Y;
            int x1 = (int)point1.X;
            int y1 = (int)point1.Y;

            int dx = Math.Abs(x1 - x0);
            int dy = Math.Abs(y1 - y0);
            int sx = x0 < x1 ? 1 : -1;
            int sy = y0 < y1 ? 1 : -1;
            int err = dx - dy;
            Color color = System.Drawing.Color.FromArgb(a, r, g, b);
            while (true)
            {
                //pointlist2.Add(new Vector2(x0, y0));
                DrawPoint(new Vector2(x0, y0), color);
                if (x0 == x1 && y0 == y1)
                {
                    break;
                }

                int e2 = 2 * err;
                if (e2 > -dy)
                {
                    err -= dy;
                    x0 += sx;
                }

                if (e2 < dx)
                {
                    err += dx;
                    y0 += sy;
                }
            }
        }
    }
}
