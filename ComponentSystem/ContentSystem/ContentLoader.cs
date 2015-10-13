namespace ComponentSystem.ContentSystem
{
    internal abstract class ContentLoader<T>
    {
        public ComponentBasedGame Game { get; internal set; }

        public abstract T Load(string filePath);
        public abstract void Unload(T content);
    }
}