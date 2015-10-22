using System.Runtime.InteropServices;

namespace SimpleLightingSample
{
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct Vector3B
    {
        public byte X;
        public byte Y;
        public byte Z;

        public Vector3B(byte x, byte y, byte z)
        {
            X = x;
            Y = y;
            Z = z;
        }
    }
}
