using System;

namespace ComponentSystem.Exceptions
{
    /// <summary>This exception is thrown when an attempt was made to set the value of a readonly property.</summary>
    public class PropertyIsReadOnlyException : Exception
    {
        public PropertyIsReadOnlyException() : base("This property can not be assigned a value because it's read only or 'set' is private.")
        {

        }
    }
}