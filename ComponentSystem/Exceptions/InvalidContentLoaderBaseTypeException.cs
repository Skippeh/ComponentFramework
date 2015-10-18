using System;

namespace ComponentSystem.Exceptions
{
    /// <summary>This exception is thrown when a class was decorated with the ContentLoaderAttribute without deriving from a valid base class.</summary>
    public class InvalidContentLoaderBaseTypeException : Exception
    {
        public InvalidContentLoaderBaseTypeException(Type type, Type contentLoaderType) : base("The content loader '" + type.FullName + "' does not inherit from " + contentLoaderType.Name + ".")
        {

        }

        public InvalidContentLoaderBaseTypeException(string message, Exception innerException) : base(message, innerException)
        {
            
        }
    }
}