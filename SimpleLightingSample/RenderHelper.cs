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
            GenerateSquareVBO();
            GenerateLineVBO();
        }

        public VBO Square;
        public VBO Line;

        void GenerateSquareVBO()
        {
            Vector2[] vecs = new Vector2[4];
            uint[] inds = new uint[4];
            for (uint u = 0; u < 4; u++)
            {
                inds[u] = u;
            }
            vecs[0] = new Vector2(1, 0);
            vecs[1] = new Vector2(1, 1);
            vecs[2] = new Vector2(0, 1);
            vecs[3] = new Vector2(0, 0);
            Square = new VBO();
            Square.Vertices = vecs.ToList();
            Square.Indices = inds.ToList();
            Square.GenerateVBO();
        }

        void GenerateLineVBO()
        {
            Vector2[] vecs = new Vector2[2];
            uint[] inds = new uint[2];
            inds[0] = 0;
            vecs[0] = new Vector2(0, 0);
            inds[1] = 1;
            vecs[1] = new Vector2(1, 0);
            Line = new VBO();
            Line.Vertices = vecs.ToList();
            Line.Indices = inds.ToList();
            Line.GenerateVBO();
        }

        public void RenderLine(Vector2 start, Vector2 end)
        {
            // TODO: Efficiency!
            float len = (end - start).Length;
            float vecang = Utilities.VectorToAnglesR(start - end);
            Matrix4 mat = Matrix4.CreateScale(len, 1, 1)
                * Matrix4.CreateRotationZ(vecang)
                * Matrix4.CreateTranslation(start.X, start.Y, 0);
            GL.UniformMatrix4(2, false, ref mat);
            GL.BindVertexArray(Line._VAO);
            GL.DrawElements(PrimitiveType.Lines, 2, DrawElementsType.UnsignedInt, IntPtr.Zero);
        }

        public void RenderRectangle(float xmin, float ymin, float xmax, float ymax)
        {
            Matrix4 mat = Matrix4.CreateScale(xmax - xmin, ymax - ymin, 1) * Matrix4.CreateTranslation(xmin, ymin, 0);
            GL.UniformMatrix4(2, false, ref mat);
            GL.BindVertexArray(Square._VAO);
            GL.DrawElements(PrimitiveType.Quads, 4, DrawElementsType.UnsignedInt, IntPtr.Zero);
        }

    }
}
