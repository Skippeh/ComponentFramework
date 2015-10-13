using System;

namespace ComponentSystem.ContentSystem
{
    internal sealed class ContentLoaderAttribute : Attribute
    {
        public Type ContentLoaderType;

        internal void ApplyClass(Type loaderType)
        {
            ContentLoaderType = loaderType;
        }
    }
}