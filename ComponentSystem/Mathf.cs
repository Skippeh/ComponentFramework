using System;
using Microsoft.Xna.Framework;

namespace ComponentSystem
{
    public static class Mathf
    {
        public static float PI => (float) Math.PI;
        public static float E => (float) Math.E;

        private static readonly Random random = new Random();

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

        /// <summary>Gets a random float between min and max.</summary>
        public static float Random(float min = 0f, float max = 1f)
        {
            return (float) (min + ((max - min) * random.NextDouble()));
        }

        /// <summary>Gets a random Vector2 between min and max.</summary>
        public static Vector2 Random(Vector2 min, Vector2 max)
        {
            return new Vector2(Random(min.X, max.X), Random(min.Y, max.Y));
        }

        /// <summary>Gets a random point around vector within the specified radius.</summary>
        public static Vector2 Random(Vector2 vector, float radius)
        {
            var angle = (float) random.NextDouble() * MathHelper.TwoPi;
            var direction = new Vector2((float) Math.Cos(angle), (float) Math.Sin(angle));

            return direction * (float) random.NextDouble() * radius;
        }

        public static Vector2 ToDirection(float radians)
        {
            return new Vector2((float) Math.Sin(radians), (float) Math.Cos(radians));
        }
    }
}