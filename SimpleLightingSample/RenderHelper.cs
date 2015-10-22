using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL4;

namespace SimpleLightingSample
{
    public class RenderHelper
    {
        public void Init()
        {
            GenerateLineVBO();
        }

        public VBO Line;

        void GenerateLineVBO()
        {
            Vector2[] vecs = new Vector2[2];
            uint[] inds = new uint[2];
            Vector2[] texs = new Vector2[2];
            for (uint u = 0; u < 2; u++)
            {
                inds[u] = u;
            }
            vecs[0] = new Vector2(0, 0);
            texs[0] = new Vector2(0, 0);
            vecs[1] = new Vector2(1, 0);
            texs[1] = new Vector2(1, 0);
            Line = new VBO();
            Line.Vertices = vecs.ToList();
            Line.Indices = inds.ToList();
            Line.TexCoords = texs.ToList();
            Line.GenerateVBO();
        }

        public void RenderLine(Vector2 start, Vector2 end)
        {
            // TODO: Efficiency!
            float len = (float)(end - start).Length;
            float vecang = Utilities.VectorToAnglesR(start - end);
            Matrix4 mat = Matrix4.CreateScale(len, 1, 1)
                * Matrix4.CreateRotationZ(vecang)
                * Matrix4.CreateTranslation(start.X, start.Y, 0);
            GL.UniformMatrix4(2, false, ref mat);
            GL.BindVertexArray(Line._VAO);
            GL.DrawElements(PrimitiveType.Lines, 2, DrawElementsType.UnsignedInt, IntPtr.Zero);
        }
    }
}
