using System;

namespace _3D_Engine
{
#if WINDOWS || LINUX
    /// <summary>
    /// The main class.
    /// </summary>
    public static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            using (var game = new Game1())
                game.Run();
        }
    }
#endif
}
/*
 https://www.davrous.com/2013/06/13/tutorial-series-learning-how-to-write-a-3d-soft-engine-from-scratch-in-c-typescript-or-javascript/
 http://what-when-how.com/xna-game-studio-4-0-programmingdeveloping-for-windows-phone-7-and-xbox-360/let-the-3d-rendering-start-xna-game-studio-4-0-programming-part-1/
 https://docs.microsoft.com/en-us/previous-versions/windows/xna/bb197041(v%3dxnagamestudio.42)
 https://stackoverflow.com/questions/24701703/c-sharp-faster-alternatives-to-setpixel-and-getpixel-for-bitmaps-for-windows-f
 http://www.songho.ca/opengl/gl_transform.html
 http://rbwhitaker.wikidot.com/monogame-basic-matrices
     */
