using System;
using System.Linq;
using System.Reflection;
using ComponentSystem.Components;
using ComponentSystem.ContentSystem;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace ComponentSystem
{
    /// <summary>A game that works with GameObjects and GameComponents to update and draw the game.</summary>
    public abstract class ComponentBasedGame : Game
    {
        public Cache Cache { get; private set; }
        public SpriteBatch SB { get; private set; }

        internal GameObjectManager GameObjectManager { get; private set; }

        /// <summary>
        /// Gets the GraphicsDeviceManager, usually called 'graphics' in xna/monogame.
        /// </summary>
        protected GraphicsDeviceManager Graphics;

        protected ComponentBasedGame()
        {
            Graphics = new GraphicsDeviceManager(this);
            GameObjectManager = new GameObjectManager(this);
            Cache = new Cache(this);
        }

        /// <summary>
        /// Adds a GameObject to the root of the hierarchy.
        /// </summary>
        /// <returns></returns>
        public GameObject AddGameObject(string name = null)
        {
            return AddGameObject<GameObject>(name);
        }

        /// <summary>
        /// Adds a GameObject of type <typeparamref name="T"/> to the root of the hierarchy.
        /// </summary>
        /// <typeparam name="T">The GameObject type.</typeparam>
        /// <param name="name">The name of the GameObject.</param>
        /// <param name="args">The arguments to pass to the GameObject constructor.</param>
        /// <returns></returns>
        public T AddGameObject<T>(string name = null, params object[] args) where T : GameObject
        {
            var argsList = args.ToList();
            argsList.Insert(0, this);
            args = argsList.ToArray();

            var gameObject = (GameObject) Activator.CreateInstance(typeof (T), BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance, null, args, null);
            GameObjectManager.AddObject(gameObject);
            gameObject.Name = name ?? gameObject.GetDefaultName();
            gameObject.AddComponent<Transform>(); // Every GameObject should have a Transform.

            gameObject._Enable();
            gameObject._Show();

            return (T) gameObject;
        }

        protected override void Initialize()
        {
            GameObjectManager = new GameObjectManager(this);
            Components.Add(GameObjectManager);

            SB = new SpriteBatch(GraphicsDevice);

            base.Initialize();
        }

        protected override void Update(GameTime gameTime)
        {
            Time.DT = (float) gameTime.ElapsedGameTime.TotalSeconds;
            Time.ElapsedTime = gameTime.TotalGameTime;

            base.Update(gameTime);
        }

        protected override bool BeginDraw()
        {
            SB.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.AnisotropicWrap);
            return base.BeginDraw();
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);
            base.Draw(gameTime);
        }

        protected override void EndDraw()
        {
            SB.End();
            base.EndDraw();
        }

        protected override void OnExiting(object sender, EventArgs args)
        {
            GameObjectManager.Destroy();
            Cache.Unload();

            base.OnExiting(sender, args);
        }

        /// <summary>
        /// Returns a string that represents the world's GameObject tree.
        /// </summary>
        /// <returns></returns>
        public string ToHierarchyString()
        {
            return GameObjectManager.ToHierarchyString();
        }
    }
}