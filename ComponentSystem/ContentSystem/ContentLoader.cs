namespace ComponentSystem.ContentSystem
{
    public abstract class ContentLoader<TContentType>
    {
        public ComponentBasedGame Game { get; internal set; }

        public abstract TContentType Load(string filePath);
        public abstract void Unload(TContentType content);
    }
}