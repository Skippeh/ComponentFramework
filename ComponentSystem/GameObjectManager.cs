using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace ComponentSystem
{
    internal class GameObjectManager : DrawableGameComponent
    {
        internal List<GameObject> GameObjects { get; private set; } = new List<GameObject>();
        
        internal GameObjectManager(Game game) : base(game)
        {

        }

        internal void AddObject(GameObject gameObject)
        {
            GameObjects.Add(gameObject);
        }

        internal void RemoveObject(GameObject gameObject)
        {
            gameObject._Destroy();
            GameObjects.Remove(gameObject);
        }

        internal void Destroy()
        {
            foreach (var gameObject in GameObjects.ToList())
            {
                RemoveObject(gameObject);
            }
        }

        public override void Update(GameTime gameTime)
        {
            foreach (var go in GameObjects)
            {
                if (!go.Enabled)
                    continue;

                go._PreUpdate();
            }

            foreach (var go in GameObjects)
            {
                if (!go.Enabled)
                    continue;

                go._Update();
            }

            foreach (var go in GameObjects)
            {
                if (!go.Enabled)
                    continue;

                go._PostUpdate();
            }

            base.Update(gameTime);
        }

        public override void Draw(GameTime gameTime)
        {
            foreach (var go in GameObjects)
            {
                if (!go.Enabled || !go.Visible)
                    continue;

                go._PreDraw();
            }

            foreach (var go in GameObjects)
            {
                if (!go.Enabled || !go.Visible)
                    continue;
                
                go._Draw();
            }

            foreach (var go in GameObjects)
            {
                if (!go.Enabled || !go.Visible)
                    continue;

                go._PostDraw();
            }

            base.Draw(gameTime);
        }
        
        internal string ToHierarchyString()
        {
            var sb = new StringBuilder();

            foreach (var go in GameObjects)
                sb.AppendLine(go.ToHierarchyString());

            return sb.ToString();
        }
    }
}