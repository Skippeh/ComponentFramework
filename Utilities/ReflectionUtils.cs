using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Utilities.Exceptions;

namespace Utilities
{
    /// <summary>
    /// Helper methods for <see cref="System.Reflection"/>.
    /// </summary>
    public static class ReflectionUtils
    {
        /// <summary>Gets the properties matching the specified bindingFlags of the given object. <para>Returns: &lt;PropertyName, Value&gt;</para></summary>
        public static Dictionary<string, object> GetProperties(object obj, BindingFlags bindingFlags = BindingFlags.Public | BindingFlags.Instance)
        {
            return obj.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance).ToDictionary(prop => prop.Name, prop => prop.GetValue(obj));
        }

        /// <summary>Sets the target's properties matching the bindingFlags to the values of the matching key in the dictionary.</summary>
        public static void SetPropertiesFrom(Dictionary<string, object> dictionary, object target, BindingFlags bindingFlags = BindingFlags.Instance | BindingFlags.Public)
        {
            var type = target.GetType();
            foreach (var propInfo in type.GetProperties(BindingFlags.Public | BindingFlags.Instance))
            {
                if (dictionary.ContainsKey(propInfo.Name) && !propInfo.CanWrite)
                    throw new PropertyIsReadOnlyException();

                if (dictionary.ContainsKey(propInfo.Name))
                {
                    try
                    {
                        propInfo.SetValue(target, dictionary[propInfo.Name]);
                    }
                    catch (ArgumentException ex)
                    {
                        throw new InvalidPropertyValueTypeException(ex);
                    }
                }
            }
        }

        /// <summary>Sets the properties that matches the given bindingFlags to the target object.</summary>
        public static void SetPropertiesFrom(object parameters, object target, BindingFlags bindingFlags = BindingFlags.Instance | BindingFlags.Public)
        {
            var properties = GetProperties(parameters, bindingFlags);
            SetPropertiesFrom(properties, target);
        }
    }
}