using ComponentSystem.Logging;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace ComponentSystem.Components
{
    public class Sprite : GameComponent
    {
        public string FilePath { get; set; }
        public Color Tint { get; set; } = Color.White;
        public SpriteEffects SpriteEffects { get; set; } = SpriteEffects.None;

        public Texture2D Texture { get; private set; }
        
        public override void OnCreate()
        {
            if (FilePath == null)
                LogError("FilePath is not set.");

            Texture = Game.Cache.Load<Texture2D>(FilePath);
        }

        public override void OnDraw()
        {
            var transform = GameObject.Transform;
            Vector2 origin = new Vector2(Texture.Width / 2, Texture.Height / 2);

            Game.SB.Draw(Texture, transform.WorldPosition, null, Tint, transform.Rotation, origin, transform.Scale, SpriteEffects, 0);
        }
    }
}