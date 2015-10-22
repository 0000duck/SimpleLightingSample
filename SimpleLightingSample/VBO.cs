using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL4;

namespace SimpleLightingSample
{
    public class VBO
    {
        uint _VertexVBO;
        uint _IndexVBO;
        public uint _VAO;
        
        public List<Vector2> Vertices;
        public List<uint> Indices;

        public void CleanLists()
        {
            Vertices = null;
            Indices = null;
        }

        int vC;
        
        public void Prepare()
        {
            Vertices = new List<Vector2>();
            Indices = new List<uint>();
        }

        public bool generated = false;

        public void Destroy()
        {
            if (generated)
            {
                GL.DeleteVertexArray(_VAO);
                GL.DeleteBuffer(_VertexVBO);
            }
        }

        public void oldvert()
        {
            verts = Vertices.ToArray();
            indices = Indices.ToArray();
        }

        Vector2[] verts = null;
        uint[] indices = null;

        public void GenerateVBO()
        {
            if (generated)
            {
                Destroy();
            }
            if (Vertices.Count == 0)
            {
                return;
            }
            vC = Vertices.Count;
            GL.BindVertexArray(0);
            Vector2[] vecs = verts == null ? Vertices.ToArray() : verts;
            uint[] inds = indices == null ? Indices.ToArray() : indices;
            // Vertex buffer
            GL.GenBuffers(1, out _VertexVBO);
            GL.BindBuffer(BufferTarget.ArrayBuffer, _VertexVBO);
            GL.BufferData(BufferTarget.ArrayBuffer, (IntPtr)(vecs.Length * Vector3.SizeInBytes),
                    vecs, BufferUsageHint.StaticDraw);
            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
            // Index buffer
            GL.GenBuffers(1, out _IndexVBO);
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, _IndexVBO);
            GL.BufferData(BufferTarget.ElementArrayBuffer, (IntPtr)(inds.Length * sizeof(uint)),
                    inds, BufferUsageHint.StaticDraw);
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, 0);
            // VAO
            GL.GenVertexArrays(1, out _VAO);
            GL.BindVertexArray(_VAO);
            GL.BindBuffer(BufferTarget.ArrayBuffer, _VertexVBO);
            GL.VertexAttribPointer(0, 2, VertexAttribPointerType.Float, false, 0, 0);
            GL.EnableVertexAttribArray(0);
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, _IndexVBO);
            // Clean up
            GL.BindVertexArray(0);
            GL.DisableVertexAttribArray(0);
            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, 0);
            generated = true;

        }

        public static void BonesIdentity()
        {
            int bones = 200;
            float[] floats = new float[bones * 4 * 4];
            for (int i = 0; i < bones; i++)
            {
                for (int x = 0; x < 4; x++)
                {
                    for (int y = 0; y < 4; y++)
                    {
                        floats[i * 16 + x * 4 + y] = Matrix4.Identity[x, y];
                    }
                }
            }
            GL.UniformMatrix4(8, bones, false, floats);
        }

        public void Render(bool texture)
        {
            if (!generated)
            {
                return;
            }
            GL.BindVertexArray(_VAO);
            GL.DrawElements(PrimitiveType.Triangles, vC, DrawElementsType.UnsignedInt, IntPtr.Zero);
            GL.BindVertexArray(0);
        }
    }
}
