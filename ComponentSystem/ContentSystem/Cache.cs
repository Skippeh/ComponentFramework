using System;
using System.Collections.Generic;
using System.Reflection;

namespace ComponentSystem.ContentSystem
{
    public class Cache
    {
        private readonly Dictionary<string, object> cache = new Dictionary<string, object>(); 
        private readonly Dictionary<Type, object> contentLoaders = new Dictionary<Type, object>(); // <ContentType, ContentLoader<ContentType>>
        private readonly ComponentBasedGame game;

        internal Cache(ComponentBasedGame game)
        {
            this.game = game;
            
            foreach (var type in Assembly.GetExecutingAssembly().GetTypes())
            {
                var attribute = type.GetCustomAttribute<ContentLoaderAttribute>();

                if (attribute != null)
                {
                    attribute.ApplyClass(type);
                    object instance = Activator.CreateInstance(type, true);
                    SetProperty(instance, "Game", game);

                    var contentType = type.BaseType.GetGenericArguments()[0];
                    contentLoaders.Add(contentType, instance);
                }
            }
        }

        public T Load<T>(string filePath) where T : class
        {
            if (cache.ContainsKey(filePath))
                return (T)cache[filePath];

            if (!contentLoaders.ContainsKey(typeof (T)))
                throw new ArgumentException("There is no ContentLoader for this type.");
            
            var loader = contentLoaders[typeof(T)];
            object content = InvokeMethod(loader, "Load", filePath);

            cache.Add(filePath, content);

            return (T) content;
        }

        public void Unload()
        {
            foreach (KeyValuePair<string, object> content in cache)
            {
                InvokeMethod(contentLoaders[content.Value.GetType()], "Unload", content.Value);
            }
        }

        private void SetProperty(object contentLoader, string propertyName, object value)
        {
            var type = contentLoader.GetType();
            var propertyInfo = type.GetProperty(propertyName, BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);

            if (propertyInfo == null)
                throw new ArgumentException("No property found with the name '" + propertyName + "' on type '" + type.Name + "'.");

            propertyInfo.SetValue(contentLoader, value, BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public, null, null, null);
        }

        private object InvokeMethod(object contentLoader, string methodName, params object[] args)
        {
            var type = contentLoader.GetType();
            var methodInfo = type.GetMethod(methodName, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);

            if (methodInfo == null)
                throw new ArgumentException("No method found with the name '" + methodName + "' on type '" + type.Name + "'.");

            var result = methodInfo.Invoke(contentLoader, args);
            return result;
        }
    }
}