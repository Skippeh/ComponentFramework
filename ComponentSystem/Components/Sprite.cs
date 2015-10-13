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

        public Sprite(string filePath)
        {
            FilePath = filePath;
        }

        public override void OnCreate()
        {
            Texture = Game.Cache.Load<Texture2D>(FilePath);
        }

        public override void OnDraw()
        {
            var transform = GameObject.Transform;
            Vector2 origin = new Vector2(Texture.Width / 2, Texture.Height / 2);

            Game.SB.Draw(Texture, transform.WorldPosition, null, Tint, transform.Rotation, origin, transform.Scale, SpriteEffects, 0);
        }

        public override GameComponent Clone()
        {
            return new Sprite(FilePath)
            {
                Tint = Tint,
                SpriteEffects = SpriteEffects
            };
        }
    }
}