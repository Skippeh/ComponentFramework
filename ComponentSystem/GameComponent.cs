using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using ComponentSystem.Attributes;
using ComponentSystem.Components;

namespace ComponentSystem
{
    /// <summary>
    /// Represents a part of a GameObject's logic.
    /// </summary>
    public abstract class GameComponent
    {
        /// <summary>Gets the GameObject that owns this component.</summary>
        public GameObject GameObject { get; internal set; }

        /// <summary>Gets the currently running Game.</summary>
        public ComponentBasedGame Game => GameObject.Game;

        /// <summary>Gets the transform of the GameObject.</summary>
        public Transform Transform => GameObject.Transform;

        /// <summary>Gets the parent of the GameObject if any.</summary>
        public GameObject Parent => GameObject.Parent;

        /// <summary>Gets whether this component is destroyed. If true then it's not safe to use.</summary>
        public bool Destroyed { get; private set; }

        /// <summary>Gets or sets whether this component's GameObject should be updated and drawn.</summary>
        public bool Enabled
        {
            get { return enabled; }
            set
            {
                if (!value)
                    _Disable();
                else
                    _Enable();
            }
        }

        /// <summary>Gets or sets whether this components GameObject should be drawn.</summary>
        public bool Visible
        {
            get { return GameObject.Visible; }
            set { GameObject.Visible = value; }
        }

        private bool initialized;
        private bool enabled;

        private static readonly Dictionary<Type, PropertyInfo[]> cloneableProperties = new Dictionary<Type, PropertyInfo[]>();

        protected GameComponent()
        {
            var type = GetType();
            if (!cloneableProperties.ContainsKey(GetType()))
            {
                // Add all public properties that can be set which don't have the IgnoreProperty attribute and is a member of this component's class.
                cloneableProperties.Add(type, type.GetProperties(BindingFlags.Public | BindingFlags.Instance)
                    .Where(prop => prop.CanWrite && prop.SetMethod.IsPublic && prop.DeclaringType == type && prop.GetCustomAttribute<IgnorePropertyAttribute>() == null)
                    .ToArray());
            }
        }

        internal void _Create()
        {
            ThrowIfDestroyed();

            if (initialized)
                return;

            OnCreate();
            initialized = true;
        }

        internal void _Destroy()
        {
            ThrowIfDestroyed();
            
            OnDestroy();
            Destroyed = true;
        }

        internal void _Enable(bool forceEvent = false)
        {
            ThrowIfDestroyed();

            if (enabled && !forceEvent)
                return;

            enabled = true;
            OnEnable();
        }

        internal void _Disable()
        {
            ThrowIfDestroyed();

            if (!enabled)
                return;

            enabled = false;
            OnDisable();
        }

        internal void _PreUpdate()
        {
            ThrowIfDestroyed();

            if (!enabled)
                return;

            OnPreUpdate();
        }

        internal void _Update()
        {
            ThrowIfDestroyed();

            if (!enabled)
                return;
            
            OnUpdate();
        }

        internal void _PostUpdate()
        {
            ThrowIfDestroyed();

            if (!enabled)
                return;

            OnPostUpdate();
        }

        internal void _PreDraw()
        {
            ThrowIfDestroyed();

            if (!enabled || !Visible)
                return;

            OnPreDraw();
        }

        internal void _Draw()
        {
            ThrowIfDestroyed();

            if (!enabled || !Visible)
                return;
            
            OnDraw();
        }

        internal void _PostDraw()
        {
            ThrowIfDestroyed();

            if (!enabled || !Visible)
                return;

            OnPostDraw();
        }

        internal void _FireShow()
        {
            OnVisible();
        }

        internal void _FireHide()
        {
            OnInvisible();
        }

        /// <summary>Called on creation when the component should be created.</summary>
        public virtual void OnCreate() { }

        /// <summary>Called on destruction when the component should be destroyed.</summary>
        public virtual void OnDestroy() { }

        /// <summary>Called when the component has been enabled.</summary>
        public virtual void OnEnable() { }

        /// <summary>Called when the component has been disabled.</summary>
        public virtual void OnDisable() { }

        /// <summary>Called before the GameObject should update itself.</summary>
        public virtual void OnPreUpdate() { }

        /// <summary>Called when the component should update itself.</summary>
        public virtual void OnUpdate() { }

        /// <summary>Called after the GameObject has updated itself.</summary>
        public virtual void OnPostUpdate() { }

        /// <summary>Called before the GameObject should update itself.</summary>
        public virtual void OnPreDraw() { }

        /// <summary>Called when the component should draw itself.</summary>
        public virtual void OnDraw() { }

        /// <summary>Called after the GameObject has drawn itself.</summary>
        public virtual void OnPostDraw() { }

        /// <summary>Called when the GameObject's visibility is turned on.</summary>
        public virtual void OnVisible() { }

        /// <summary>Called when the GameObject's visibility is turned off.</summary>
        public virtual void OnInvisible() { }

        private void ThrowIfDestroyed()
        {
            if (Destroyed)
                throw new InvalidOperationException("This component has been destroyed.");
        }

        /// <summary>Called on the original component when it's being cloned. This is a good time to do any deeper cloning if necessary.</summary>
        protected virtual void OnClone(GameComponent clone) { }

        internal GameComponent Clone(GameObject newOwner)
        {
            if (newOwner == GameObject)
                throw new ArgumentException("The cloned GameComponent's owner can't be the same GameObject as the original.");

            var type = GetType();
            var clone = (GameComponent)Activator.CreateInstance(type, true);
            clone.GameObject = newOwner;
            clone.enabled = enabled;

            foreach (var propInfo in cloneableProperties[type])
            {
                propInfo.SetValue(clone, propInfo.GetValue(this));
            }

            OnClone(clone);
            
            return clone;
        }
    }
}