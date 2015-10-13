using System;

namespace ComponentSystem
{
    public static class Time
    {
        /// <summary>Gets the amount of seconds that has passed since the last update.</summary>
        public static float DT { get; internal set; }

        /// <summary>Gets the amount of time that has surpassed since the game started.</summary>
        public static TimeSpan ElapsedTime { get; internal set; }
    }
}