using System.Text;

namespace Utilities
{
    /// <summary>Static class with a number of helper extensions for strings.</summary>
    public static class StringUtility
    {
        /// <summary>Repeats the given string a specified number of times.</summary>
        public static string Repeat(this string str, int numTimes)
        {
            return new StringBuilder().Insert(0, str, numTimes).ToString();
        }
    }
}