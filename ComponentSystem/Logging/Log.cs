using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utilities;

namespace ComponentSystem.Logging
{
    public static class Log
    {
        private static DateTime? lastLogTime = null;

        // Tuple<ForegroundColor, BackgroundColor>
        private static readonly Dictionary<LogType, Tuple<ConsoleColor, ConsoleColor>> colors = new Dictionary<LogType, Tuple<ConsoleColor, ConsoleColor>>
        {
            { LogType.Info,  new Tuple<ConsoleColor, ConsoleColor>(ConsoleColor.Black, ConsoleColor.Gray) },
            { LogType.Debug, new Tuple<ConsoleColor, ConsoleColor>(ConsoleColor.Black, ConsoleColor.Yellow) },
            { LogType.Error, new Tuple<ConsoleColor, ConsoleColor>(ConsoleColor.White, ConsoleColor.Red) }
        };

        public static void Info(Type callerType, params object[] message)
        {
            log(callerType, LogType.Info, message);
        }

        public static void Debug(Type callerType, params object[] message)
        {
            log(callerType, LogType.Debug, message);
        }

        public static void Error(Type callerType, params object[] message)
        {
            log(callerType, LogType.Error, message);
        }

        private static void log(Type callerType, LogType logType, params object[] message)
        {
            var time = DateTime.Now;
            var elapsed = lastLogTime != null ? DateTime.Now - (DateTime)lastLogTime : TimeSpan.Zero;
            lastLogTime = time;

            string strTime = time.ToString("HH:mm:ss.fff");
            string prefix = "[" + logType.ToString().ToUpper() + "] " + elapsed.TotalMilliseconds + " ms ";
            string strMessage = String.Join("\t", message);

            ConsoleColor previousBackground = Console.BackgroundColor;
            ConsoleColor previousForeground = Console.ForegroundColor;
            ConsoleColor foregroundColor = colors[logType].Item1;
            ConsoleColor backgroundColor = colors[logType].Item2;

            if (Console.ForegroundColor != foregroundColor)
                Console.ForegroundColor = foregroundColor;

            if (Console.BackgroundColor != backgroundColor)
                Console.BackgroundColor = backgroundColor;

            Console.Write(prefix);
            
            string rightAlignedText = callerType.Name;
            Console.Write(" ".Repeat(Console.BufferWidth - Console.CursorLeft - rightAlignedText.Length));
            Console.Write(rightAlignedText);

            Console.BackgroundColor = previousBackground;
            Console.ForegroundColor = previousForeground;

            Console.WriteLine(strMessage + "\n");
        }
    }
}