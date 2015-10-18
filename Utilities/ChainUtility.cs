using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Utilities
{
    public static class ChainUtility
    {
        /// <summary>Invokes the actions one at a time on the given object.</summary>
        public static void InvokeMethodsOn<T>(T obj, params Action<T>[] actions)
        {
            InvokeMethodsOn(new T[] { obj }, actions);
        }

        /// <summary>Invokes one action at a time on every object.</summary>
        public static void InvokeMethodsOn<T>(IEnumerable<T> enumerable, params Action<T>[] actions)
        {
            var objects = enumerable.ToArray();

            foreach (var action in actions)
            {
                foreach (var obj in objects)
                {
                    action(obj);
                }
            }
        }
    }
}