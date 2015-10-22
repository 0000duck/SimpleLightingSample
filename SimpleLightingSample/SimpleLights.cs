using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OpenTK;
using OpenTK.Input;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL4;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace SimpleLightingSample
{
    public class SimpleLights
    {
        public static SimpleLights Instance = null;

        public static int UNIF_PROJECTION = 1;

        public static int UNIF_MODELMATRIX = 2;

        public static int UNIF_COLOR = 3;

        public static int BLOCK_WIDTH = 16;

        public static int MAX_WIDTH = ((800 / BLOCK_WIDTH) / 16) * 16;

        public static int MAX_HEIGHT = ((600 / BLOCK_WIDTH) / 16) * 16;
        
        static void Main(string[] args)
        {
            Instance = new SimpleLights();
            Instance.Run();
        }

        public byte[] Blocks = new byte[MAX_WIDTH * MAX_HEIGHT];

        public int BlockIndex(Vector2 vec)
        {
            if (vec.X < 0 || vec.Y < 0 || vec.X >= MAX_WIDTH || vec.Y >= MAX_HEIGHT)
            {
                throw new Exception("Invalid Block location vector!");
            }
            return (int)(vec.Y * MAX_WIDTH + vec.X);
        }

        public Material GetBlockAt(Vector2 vec)
        {
            return (Material)Blocks[BlockIndex(vec)];
        }

        public void SetBlockAt(Vector2 vec, Material mat)
        {
            Blocks[BlockIndex(vec)] = (byte)mat;
        }

        public int[] Light = new int[MAX_WIDTH * MAX_HEIGHT];

        public Vector3B GetLightAt(Vector2 vec)
        {
            uint b = (uint)Light[BlockIndex(vec)];
            byte X = (byte)(b & 255);
            byte Y = (byte)(b & (256 * 255));
            byte Z = (byte)(b & (256 * 256 * 255));
            return new Vector3B() { X = X, Y = Y, Z = Z };
        }

        public void SetLightAt(Vector2 vec, Vector3B val)
        {
            Light[BlockIndex(vec)] = val.X + val.Y * 256 + val.Z * 256 * 256;
        }

        public GameWindow Window = null;
        
        public bool InputEnabled = false;

        public List<LightSource> Lights = new List<LightSource>();

        public RenderHelper Renderer = null;

        public Shader ShaderObjects;

        public Shader ShaderBlocks;

        public Shader ShaderLights;

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
            GL.ActiveTexture(TextureUnit.Texture0);
            Renderer = new RenderHelper();
            Renderer.Init();
            ShaderObjects = new Shader();
            ShaderObjects.Load("shader_objects");
            ShaderBlocks = new Shader();
            ShaderBlocks.Load("shader_blocks");
            ShaderLights = new Shader();
            ShaderLights.Load("shader_lights");
        }

        private void Mouse_ButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (!InputEnabled)
            {
                return;
            }
            int ex = (e.X / BLOCK_WIDTH) * BLOCK_WIDTH;
            int ey = (e.Y / BLOCK_WIDTH) * BLOCK_WIDTH;
            if (e.Button == MouseButton.Left)
            {
                if (GetBlockAt(new Vector2(ex / BLOCK_WIDTH, ey / BLOCK_WIDTH)) != Material.AIR)
                {
                    Console.WriteLine("Refusing to place stone - block occupied!");
                    return;
                }
                Console.WriteLine("Generating stone at " + ex + ", " + ey);
                SetBlockAt(new Vector2(ex / BLOCK_WIDTH, ey / BLOCK_WIDTH), Material.STONE);
                UpdateBlocks();
                UpdateLights();
            }
            if (e.Button == MouseButton.Middle)
            {
                if (GetBlockAt(new Vector2(ex / BLOCK_WIDTH, ey / BLOCK_WIDTH)) != Material.AIR)
                {
                    Console.WriteLine("Refusing to place light - block occupied!");
                    return;
                }
                Console.WriteLine("Generating light at " + ex + ", " + ey);
                LightSource ls = new LightSource();
                ls.Location = new Vector2(ex, ey);
                ls.Color = new Vector3B((byte)Utilities.random.Next(128, 256), (byte)Utilities.random.Next(128, 256), (byte)Utilities.random.Next(128, 256));
                Lights.Add(ls);
                SetBlockAt(new Vector2(ex / BLOCK_WIDTH, ey / BLOCK_WIDTH), Material.LIGHT);
                UpdateBlocks();
                UpdateLights();
            }
            else if (e.Button == MouseButton.Right)
            {
                Material cur = GetBlockAt(new Vector2(ex / BLOCK_WIDTH, ey / BLOCK_WIDTH));
                if (cur == Material.AIR)
                {
                    Console.WriteLine("Refusing to remove block - block unoccupied!");
                    return;
                }
                else if (cur == Material.LIGHT)
                {
                    Vector2 loc = new Vector2(ex, ey);
                    for (int i = 0; i < Lights.Count; i++)
                    {
                        if (Lights[i].Location == loc)
                        {
                            Lights.RemoveAt(i);
                            break;
                        }
                    }
                }
                Console.WriteLine("Removing block at " + ex + ", " + ey);
                SetBlockAt(new Vector2(ex / BLOCK_WIDTH, ey / BLOCK_WIDTH), Material.AIR);
                UpdateBlocks();
                UpdateLights();
            }
        }

        public int LightTex = -1;

        public static float LIGHT_DIMMING = 0.1f;

        int c = 0;

        public void PropogateLightAt(int x, int y, Vector3B ocolor, float strength)
        {
            c++;
            if (strength <= 0.05)
            {
                return;
            }
            if (x < 0 || y < 0 || x >= MAX_WIDTH || y >= MAX_HEIGHT)
            {
                return;
            }
            Vector2 pos = new Vector2(x, y);
            Material cblock = GetBlockAt(pos);
            if (cblock == Material.AIR || cblock == Material.LIGHT)
            {
                Vector3B oldc = GetLightAt(pos);
                Vector3B col = new Vector3B((byte)(ocolor.X * strength + oldc.X), (byte)(ocolor.Y * strength + oldc.Y), (byte)(ocolor.Z * strength + oldc.Z));
                SetLightAt(pos, col);
                PropogateLightAt(x - 1, y, ocolor, strength - LIGHT_DIMMING);
                PropogateLightAt(x + 1, y, ocolor, strength - LIGHT_DIMMING);
                PropogateLightAt(x, y - 1, ocolor, strength - LIGHT_DIMMING);
                PropogateLightAt(x, y + 1, ocolor, strength - LIGHT_DIMMING);
            }
        }

        public void UpdateLights()
        {
            for (int x = 0; x < MAX_WIDTH; x++)
            {
                for (int y = 0; y < MAX_HEIGHT; y++)
                {
                    SetLightAt(new Vector2(x, y), new Vector3B(0, 0, 0));
                }
            }
            c = 0;
            for (int i = 0; i < Lights.Count; i++)
            {
                PropogateLightAt((int)(Lights[i].Location.X / BLOCK_WIDTH), (int)(Lights[i].Location.Y / BLOCK_WIDTH), Lights[i].Color, 1f);
            }
            Console.WriteLine("Updated with " + c + " block iterations!");
            // Generate Texture
            if (LightTex != -1)
            {
                GL.DeleteTexture(LightTex);
            }
            LightTex = GL.GenTexture();
            GL.BindTexture(TextureTarget.Texture2D, LightTex);
            IntPtr unmanagedPointer = Marshal.AllocHGlobal(Light.Length * 4);
            Marshal.Copy(Light, 0, unmanagedPointer, Light.Length);
            GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgb, MAX_WIDTH, MAX_HEIGHT, 0, PixelFormat.Rgb, PixelType.Byte, unmanagedPointer);
            Marshal.FreeHGlobal(unmanagedPointer);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Nearest);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Nearest);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)TextureWrapMode.ClampToEdge);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)TextureWrapMode.ClampToEdge);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureCompareMode, (int)TextureCompareMode.CompareRefToTexture);
            GL.BindTexture(TextureTarget.Texture2D, 0);
        }

        public int BlockTex = -1;

        public void UpdateBlocks()
        {
            if (BlockTex != -1)
            {
                GL.DeleteTexture(BlockTex);
            }
            BlockTex = GL.GenTexture();
            GL.BindTexture(TextureTarget.Texture2D, BlockTex);
            IntPtr unmanagedPointer = Marshal.AllocHGlobal(Blocks.Length);
            Marshal.Copy(Blocks, 0, unmanagedPointer, Blocks.Length);
            GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.R8, MAX_WIDTH, MAX_HEIGHT, 0, PixelFormat.Red, PixelType.Byte, unmanagedPointer);
            Marshal.FreeHGlobal(unmanagedPointer);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Nearest);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Nearest);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)TextureWrapMode.ClampToEdge);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)TextureWrapMode.ClampToEdge);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureCompareMode, (int)TextureCompareMode.CompareRefToTexture);
            GL.BindTexture(TextureTarget.Texture2D, 0);
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
            // Setup objects / general
            ShaderObjects.Bind();
            GL.Clear(ClearBufferMask.ColorBufferBit);
            Matrix4 Projection = Matrix4.CreateOrthographicOffCenter(0, Window.Width, Window.Height, 0, -1, 1);
            Matrix4 ModelMat = Matrix4.Identity;
            GL.UniformMatrix4(UNIF_PROJECTION, false, ref Projection);
            GL.UniformMatrix4(UNIF_MODELMATRIX, false, ref ModelMat);
            SetRenderColor(Color4.White);
            // Render light buffer
            if (LightTex != -1)
            {
                ShaderLights.Bind();
                GL.UniformMatrix4(UNIF_PROJECTION, false, ref Projection);
                GL.UniformMatrix4(UNIF_MODELMATRIX, false, ref ModelMat);
                GL.BindTexture(TextureTarget.Texture2D, LightTex);
                Renderer.RenderRectangle(0, 0, MAX_WIDTH * BLOCK_WIDTH, MAX_HEIGHT * BLOCK_WIDTH);
            }
            // Render block buffer
            if (BlockTex != -1)
            {
                ShaderBlocks.Bind();
                GL.UniformMatrix4(UNIF_PROJECTION, false, ref Projection);
                GL.UniformMatrix4(UNIF_MODELMATRIX, false, ref ModelMat);
                GL.BindTexture(TextureTarget.Texture2D, BlockTex);
                Renderer.RenderRectangle(0, 0, MAX_WIDTH * BLOCK_WIDTH, MAX_HEIGHT * BLOCK_WIDTH);
            }
            // Render objects
            ShaderObjects.Bind();
            GL.BindTexture(TextureTarget.Texture2D, 0);
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
