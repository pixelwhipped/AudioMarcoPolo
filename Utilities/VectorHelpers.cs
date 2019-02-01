using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;

namespace AudioMarcoPolo.Utilities
{
    public static class VectorHelpers
    {
        public static Vector2 Truncate(Vector2 vec, float max_value)
        {
            if (vec.Length() > max_value)
            {
                return Vector2.Multiply(Vector2.Normalize(vec), max_value);
            }
            return vec;
        }
        public static float Hypot(Vector2 a, Vector2 b)
        {
            return (float)Math.Sqrt(Math.Pow(Math.Max(a.X, b.X) - Math.Min(a.X, b.X), 2) + Math.Pow(Math.Max(a.Y, b.Y) - Math.Min(a.Y, b.Y), 2));

        }

        public static float Length(Vector2 v1)
        {
            return (float)Math.Sqrt(v1.X * v1.X + v1.Y * v1.Y);
        }
    }
}
