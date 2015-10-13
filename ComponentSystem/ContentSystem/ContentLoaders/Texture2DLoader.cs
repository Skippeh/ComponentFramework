using System.IO;
using Microsoft.Xna.Framework.Graphics;

namespace ComponentSystem.ContentSystem.ContentLoaders
{
    [ContentLoader]
    internal class Texture2DLoader : ContentLoader<Texture2D>
    {
        public override Texture2D Load(string filePath)
        {
            using (var fs = new FileStream(filePath, FileMode.Open))
            {
                return Texture2D.FromStream(Game.GraphicsDevice, fs);
            }
        }

        public override void Unload(Texture2D content)
        {
            if (content.IsDisposed)
                return;

            content.Dispose();
        }
    }
}