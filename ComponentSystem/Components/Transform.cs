using ComponentSystem.Attributes;
using Microsoft.Xna.Framework;

namespace ComponentSystem.Components
{
    public class Transform : GameComponent
    {
        /// <summary>Gets or sets the local position.</summary>
        public Vector2 Position
        {
            get { return position; }
            set { position = value; }
        }

        /// <summary>Gets the world position.</summary>
        public Vector2 WorldPosition => (GameObject.Parent?.Transform.WorldPosition ?? Vector2.Zero) + Position;

        /// <summary>
        /// The current rotation in radians.
        /// </summary>
        public float Rotation { get; set; }

        /// <summary>Gets or sets the local X coordinate.</summary>
        [IgnoreProperty]
        public float X { get { return position.X; } set { position.X = value; } }

        /// <summary>Gets or sets the local Y coordinate.</summary>
        [IgnoreProperty]
        public float Y { get { return position.Y; } set { position.Y = value; } }

        /// <summary>Gets or sets the scale.</summary>
        public Vector2 Scale { get; set; } = Vector2.One;

        private Vector2 position = Vector2.Zero;

        /// <summary>Returns the local or world position.</summary>
        public Vector2 GetPosition(bool worldPosition)
        {
            return worldPosition ? WorldPosition : Position;
        }

        /// <summary>Sets the position in local or world space.</summary>
        public Transform SetPosition(Vector2 position, bool worldPosition)
        {
            if (worldPosition)
                Position = position - WorldPosition;
            else
                Position = position;
            
            return this;
        }
    }
}