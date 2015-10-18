using System;

namespace Utilities.Exceptions
{
    /// <summary>This exception is thrown when a value given to a property could not be converted to the property's type.</summary>
    public class InvalidPropertyValueTypeException : Exception
    {
        public InvalidPropertyValueTypeException(Exception innerException = null) : base("The given value can not be converted to the property's type.", innerException)
        {
            
        }
    }
}