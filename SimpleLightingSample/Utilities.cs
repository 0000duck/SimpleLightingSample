using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OpenTK;

namespace SimpleLightingSample
{
    public class Utilities
    {
        public static double PI180 = Math.PI / 180;

        public static float VectorToAnglesR(Vector2 input)
        {
            return (float)(Math.Atan2(input.Y, input.X) + Math.PI);
        }

        public static float VectorToAngles(Vector2 input)
        {
            return (float)(Math.Atan2(input.Y, input.X) * 180f / Math.PI) + 180f;
        }
    }
}
