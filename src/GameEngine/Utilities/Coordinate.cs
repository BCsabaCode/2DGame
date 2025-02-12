using System;
using System.Numerics;

namespace GameEngine.Utilities
{
    class Coordinate
    {
        public static readonly float root2 = (float)Math.Sqrt(2.0f);

        public static Vector2 NormalToIso(Vector2 vec2)
        {
            /*  Transformation matrix
             *  2*sqrt(2)   -2*sqrt(2)
             *  sqrt(2)     sqrt(2)
             */

            float x = vec2.X;
            float y = vec2.Y;

            vec2.X = (x * 2.0f * root2 - y * 2.0f * root2);
            vec2.Y = (x * root2 + y * root2);

            return vec2;
        }

        public static Vector2 IsoToNormal(Vector2 vec2)
        {
            /*  Inverz Transformation matrix
             *  sqrt(2)/8   sqrt(2)/4
             *  -sqrt(2)/8     sqrt(2)/4
             */
            float x = vec2.X;
            float y = vec2.Y;

            vec2.X = (x * root2 / 8.0f + y * root2 / 4.0f);
            vec2.Y = (x * -root2 / 8.0f + y * root2 / 4.0f);

            return vec2;
        }

        public static Vector2 VirtualZAxisReduction(Vector2 vec2, float z)
        {
            vec2.X -= z;
            vec2.Y -= z;
            return vec2;
        }
    }
}
