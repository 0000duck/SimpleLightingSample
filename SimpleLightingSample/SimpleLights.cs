using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OpenTK;
using OpenTK.Input;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL4;

namespace SimpleLightingSample
{
    class SimpleLights
    {
        public static SimpleLights Instance = null;

        static void Main(string[] args)
        {
            Instance = new SimpleLights();
            Instance.Run();
        }

        public GameWindow Window = null;

        public bool InputEnabled = false;

        public void Run()
        {
            Console.WriteLine("Load window...");
            Window = new GameWindow(800, 600, GraphicsMode.Default, "Simple Lighting Sample", GameWindowFlags.Default, DisplayDevice.Default, 4, 3, GraphicsContextFlags.ForwardCompatible);
            Window.Load += Window_Load;
            Window.UpdateFrame += Window_UpdateFrame;
            Window.RenderFrame += Window_RenderFrame;
            Window.Run();
            Console.WriteLine("End program!");
        }

        private void Window_Load(object sender, EventArgs e)
        {
            Console.WriteLine("Load OpenGL...");
            GL.ClearColor(Color4.White);
            GL.Disable(EnableCap.DepthTest);
        }

        private void Window_UpdateFrame(object sender, FrameEventArgs e)
        {
            InputEnabled = Window.Focused;
        }

        private void Window_RenderFrame(object sender, FrameEventArgs e)
        {
            GL.Clear(ClearBufferMask.ColorBufferBit);
            Window.SwapBuffers();
        }
    }
}
