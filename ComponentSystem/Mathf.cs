using System;
using Microsoft.Xna.Framework;

namespace ComponentSystem
{
    public static class Mathf
    {
        public static float PI => (float) Math.PI;
        public static float E => (float) Math.E;

        /// <summary>Smoothly interpolates the vector between 0-1-0.</summary>
        public static Vector2 SmoothLerp010(Vector2 a, Vector2 b, float amount)
        {
            return new Vector2(SmoothLerp010(a.X, b.X, amount), SmoothLerp010(a.Y, b.Y, amount));
        }

        /// <summary>Smoothly interpolates the value between 0-1-0.</summary>
        public static float SmoothLerp010(float a, float b, float amount)
        {
            var val = (float) (Math.Abs(Math.Sin(amount * PI)));
            return a + (b - a) * val;
        }
    }
}