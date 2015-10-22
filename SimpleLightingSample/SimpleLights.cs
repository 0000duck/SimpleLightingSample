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
    public class SimpleLights
    {
        public static SimpleLights Instance = null;

        public static int UNIF_PROJECTION = 1;

        public static int UNIF_MODELMATRIX = 2;

        public static int UNIF_COLOR = 3;

        public static int BLOCK_WIDTH = 16;

        static void Main(string[] args)
        {
            Instance = new SimpleLights();
            Instance.Run();
        }

        public GameWindow Window = null;
        
        public bool InputEnabled = false;

        public List<LightSource> Lights = new List<LightSource>();

        public RenderHelper Renderer = null;

        public Shader ShaderObjects;

        public void Run()
        {
            Console.WriteLine("Load window...");
            Window = new GameWindow(800, 600, GraphicsMode.Default, "Simple Lighting Sample", GameWindowFlags.Default, DisplayDevice.Default, 4, 3, GraphicsContextFlags.ForwardCompatible);
            Window.Load += Window_Load;
            Window.UpdateFrame += Window_UpdateFrame;
            Window.RenderFrame += Window_RenderFrame;
            Window.Mouse.ButtonDown += Mouse_ButtonDown;
            Window.Run();
            Console.WriteLine("End program!");
        }

        private void Window_Load(object sender, EventArgs e)
        {
            Console.WriteLine("Load OpenGL...");
            GL.ClearColor(Color4.Black);
            GL.Disable(EnableCap.DepthTest);
            GL.Viewport(0, 0, Window.Width, Window.Height);
            Renderer = new RenderHelper();
            Renderer.Init();
            ShaderObjects = new Shader();
            ShaderObjects.Load("shader_objects");
        }

        private void Mouse_ButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (!InputEnabled)
            {
                return;
            }
            if (e.Button == MouseButton.Left)
            {
                int ex = (e.X / BLOCK_WIDTH) * BLOCK_WIDTH;
                int ey = (e.Y / BLOCK_WIDTH) * BLOCK_WIDTH;
                Console.WriteLine("Generating light at " + ex + ", " + ey);
                LightSource ls = new LightSource();
                ls.Location = new Vector2(ex, ey);
                Lights.Add(ls);
                UpdateLights();
            }
        }

        public void UpdateLights()
        {
            // TODO
        }

        private void Window_UpdateFrame(object sender, FrameEventArgs e)
        {
            InputEnabled = Window.Focused;
        }

        public void SetRenderColor(Color4 color)
        {
            GL.Uniform4(UNIF_COLOR, color);
        }

        private void Window_RenderFrame(object sender, FrameEventArgs e)
        {
            ShaderObjects.Bind();
            GL.Clear(ClearBufferMask.ColorBufferBit);
            Matrix4 Projection = Matrix4.CreateOrthographicOffCenter(0, Window.Width, Window.Height, 0, -1, 1);
            Matrix4 ModelMat = Matrix4.Identity;
            GL.UniformMatrix4(UNIF_PROJECTION, false, ref Projection);
            GL.UniformMatrix4(UNIF_MODELMATRIX, false, ref ModelMat);
            SetRenderColor(Color4.White);
            // TODO: Render light buffer
            SetRenderColor(Color4.White);
            for (int i = 0; i < Lights.Count; i++)
            {
                Vector2 pos = Lights[i].Location;
                Renderer.RenderLine(pos, pos + new Vector2(BLOCK_WIDTH, 0));
                Renderer.RenderLine(pos, pos + new Vector2(0, BLOCK_WIDTH));
                Renderer.RenderLine(pos + new Vector2(0, BLOCK_WIDTH), pos + new Vector2(BLOCK_WIDTH, BLOCK_WIDTH));
                Renderer.RenderLine(pos + new Vector2(BLOCK_WIDTH, 0), pos + new Vector2(BLOCK_WIDTH, BLOCK_WIDTH));
                Renderer.RenderLine(pos, pos + new Vector2(BLOCK_WIDTH, BLOCK_WIDTH));
                Renderer.RenderLine(pos + new Vector2(BLOCK_WIDTH, 0), pos + new Vector2(0, BLOCK_WIDTH));
            }
            Window.SwapBuffers();
        }
    }
}
