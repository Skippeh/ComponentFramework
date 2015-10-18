using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Runtime.Serialization;
using System.Text;
using ComponentSystem.Components;
using ComponentSystem.Exceptions;
using Utilities;
using Utilities.Exceptions;

namespace ComponentSystem
{
    /// <summary>
    /// A physical or non physical object in the game world. Consists of one or multiple Components and can have sub GameObjects.
    /// </summary>
    public class GameObject
    {
        /// <summary>Gets or sets the unique name. If the name is already taken an ArgumentException will be thrown.</summary>
        public string Name
        {
            get { return name; }
            set
            {
                if (Siblings.Any(sibling => sibling != this && sibling.Name == value))
                    throw new ArgumentException("There is already a GameObject with this name at the current position in the hierarchy.");

                name = value;
            }
        }

        /// <summary>Gets the GameObject's that have the same parent. If the parent is null the root GameObjects will be returned.</summary>
        public List<GameObject> Siblings => Parent != null ? Parent.Children : Manager.GameObjects;

        /// <summary>Gets the parent of this GameObject.</summary>
        public GameObject Parent { get; private set; }

        /// <summary>Gets or sets whether this GameObject should be drawn.</summary>
        public bool Visible
        {
            get { return visible; }
            set
            {
                if (value)
                    _Show();
                else
                    _Hide();
            }
        }

        /// <summary>Gets or sets whether this GameObject should update and be drawn.</summary>
        public bool Enabled
        {
            get { return enabled; }
            set
            {
                if (value)
                    _Enable();
                else
                    _Disable();
            }
        }

        /// <summary>Gets whether this GameObject has been destroyed. If true then it's not safe to use.</summary>
        public bool Destroyed { get; private set; }

        /// <summary>Gets the GameObject's Transform component.</summary>
        public Transform Transform => cachedTransform ?? (cachedTransform = GetComponent<Transform>());

        internal GameObjectManager Manager => Game.GameObjectManager;
        internal List<GameObject> Children { get; } = new List<GameObject>();
        internal Dictionary<Type, GameComponent> Components { get; private set; } = new Dictionary<Type, GameComponent>();

        private bool enabled = false;
        private bool visible = false;
        private string name = "";
        private Transform cachedTransform;

        internal ComponentBasedGame Game { get; }

        internal GameObject(ComponentBasedGame game)
        {
            Game = game;
        }

        private GameObject() // Cloning requires a constructor with no arguments.
        {
            
        }

        /// <summary>Searches the children of this GameObject for the GameObject with the given name and type.</summary>
        public T FindChild<T>(string name) where T : GameObject
        {
            ThrowIfDestroyed();
            return (T)Children.FirstOrDefault(child => child.Name == name && child.GetType() == typeof(T));
        }

        /// <summary>Searches the children of this GameObject for the GameObject with the given name.</summary>
        public GameObject FindChild(string name)
        {
            return FindChild<GameObject>(name);
        }

        /// <summary>Gets the component of the given type. If this GameObject doesn't have it null will be returned instead.</summary>
        public T GetComponent<T>() where T : GameComponent
        {
            return (T) GetComponent(typeof (T));
        }

        /// <summary>Gets the component of the given type. If this GameObject doesn't have it null will be returned instead.</summary>
        public GameComponent GetComponent(Type type)
        {
            ThrowIfDestroyed();
            ThrowIfMismatchingTypes(type, typeof (GameComponent), "The given type is not of GameComponent.");

            return Components.ContainsKey(type) ? Components[type] : null;
        }

        /// <summary>Adds a component of type <typeparamref name="T"/>.</summary>
        /// <typeparam name="T">The type of component to add.</typeparam>
        public T AddComponent<T>(object parameters = null) where T : GameComponent
        {
            var component = AddComponent(typeof (T), parameters);
            return (T)component;
        }

        /// <summary>Adds a component of the given type.</summary>
        public GameComponent AddComponent(Type type, object parameters = null)
        {
            ThrowIfDestroyed();
            ThrowIfMismatchingTypes(type, typeof (GameComponent), "The given type is not of GameComponent.");

            if (Components.ContainsKey(type))
                throw new ArgumentException("This component has already been added to this GameObject.");
            
            var component = (GameComponent) Activator.CreateInstance(type, BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance, null, null, null);

            // Parse parameters
            var dictParams = new Dictionary<string, object>(); // <PropertyName, Value>

            if (parameters != null)
            {
                foreach (var prop in parameters.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance))
                {
                    dictParams.Add(prop.Name, prop.GetValue(parameters));
                }
            }

            // Set parameters
            foreach (var propInfo in type.GetProperties(BindingFlags.Public | BindingFlags.Instance))
            {
                if (dictParams.ContainsKey(propInfo.Name) && !propInfo.CanWrite)
                    throw new PropertyIsReadOnlyException();

                if (dictParams.ContainsKey(propInfo.Name))
                {
                    try
                    {
                        propInfo.SetValue(component, dictParams[propInfo.Name]);
                    }
                    catch (ArgumentException ex)
                    {
                        throw new InvalidPropertyValueTypeException(ex);
                    }
                }
            }

            AddComponent(component);

            component._Create();

            if (Enabled)
                component._Enable();

            if (Visible)
                component._FireShow();

            return component;
        }

        /// <summary>Adds the given component to this GameObject's components and sets its' GameObject to this.</summary>
        private GameComponent AddComponent(GameComponent component)
        {
            component.GameObject = this;
            Components.Add(component.GetType(), component);

            return component;
        }

        /// <summary>Removes the component of type <typeparamref name="T"/>.</summary>
        /// <param name="T">The type of component to remove.</param>
        public void RemoveComponent<T>() where T : GameComponent
        {
            RemoveComponent(typeof (T));
        }

        /// <summary>Removes the component of the given type.</summary>
        public void RemoveComponent(Type type)
        {
            ThrowIfDestroyed();
            ThrowIfMismatchingTypes(type, typeof (GameComponent), "The given type is not of GameComponent.");
            
            var component = Components[type];
            
            component.Enabled = false;

            component._Destroy();
            Components.Remove(type);

            if (type == typeof (Transform))
                cachedTransform = null;
        }

        /// <summary>Adds a child object of type <typeparamref name="T"/> and returns it.</summary>
        /// <typeparam name="T">The type of GameObject to add.</typeparam>
        /// <param name="name">The name of the GameObject.</param>
        public T AddObject<T>(string name = null, params object[] args) where T : GameObject
        {
            ThrowIfDestroyed();

            var argsList = args.ToList();
            argsList.Insert(0, Game);
            args = argsList.ToArray();

            var gameObject = (GameObject) Activator.CreateInstance(typeof (T), BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance, null, args, null);
            
            if (gameObject.Name != "" && Children.Any(child => child.Name == gameObject.Name))
                throw new ArgumentException("There is already a child with the same name.");

            AddObject(gameObject);
            gameObject.Name = name ?? gameObject.GetDefaultName();
            gameObject.AddComponent<Transform>(); // Every GameObject should have a Transform.

            gameObject._Enable();
            gameObject._Show();

            return (T) gameObject;
        }

        /// <summary>Adds a child object and returns it.</summary>
        public GameObject AddObject(string name = null)
        {
            return AddObject<GameObject>(name);
        }

        /// <summary>Removes the child with the matching name. Throws ArgumentException if not found.</summary>
        public GameObject RemoveObject(string name)
        {
            ThrowIfDestroyed();

            var gameObject = Children.FirstOrDefault(child => child.Name == name);

            if (gameObject == null)
                throw new ArgumentException("No child GameObject found with the name '" + name + "'.");

            return RemoveObject(gameObject);
        }

        /// <summary>Adds the given GameObject to this GameObject's children and sets it's parent to this.</summary>
        private GameObject AddObject(GameObject go)
        {
            if (go.Parent != null)
                throw new ArgumentException("This GameObject already has a parent.");

            go.Parent = this;
            Children.Add(go);

            return go;
        }

        /// <summary>Removes the given child.</summary>
        public GameObject RemoveObject(GameObject child)
        {
            ThrowIfDestroyed();

            child._Hide();
            child._Disable();
            child._Destroy();
            Children.Remove(child);
            return child;
        }

        /// <summary>Destroys this GameObject. Unusable after calling.</summary>
        public void Destroy()
        {
            if (Destroyed)
                return;

            if (Parent == null)
                Manager.RemoveObject(this);
            else
                Parent.RemoveObject(this);
        }

        internal void _Destroy()
        {
            if (Destroyed)
                return;

            foreach (var child in Children.ToList())
            {
                RemoveObject(child);
            }

            foreach (var component in Components.ToList())
            {
                RemoveComponent(component.Key);
            }

            Destroyed = true;
            Parent = null;
        }

        internal void _Show()
        {
            ThrowIfDestroyed();

            if (visible)
                return;

            visible = true;

            foreach (var component in Components)
                component.Value._FireShow();
        }

        internal void _Hide()
        {
            ThrowIfDestroyed();

            if (!visible)
                return;

            visible = false;

            foreach (var component in Components)
                component.Value._FireHide();
        }

        internal void _Enable()
        {
            ThrowIfDestroyed();

            if (enabled)
                return;

            enabled = true;

            foreach (var component in Components)
                component.Value._Enable();
        }

        internal void _Disable()
        {
            ThrowIfDestroyed();

            if (!enabled)
                return;

            enabled = false;

            foreach (var component in Components)
                component.Value._Disable();
        }

        internal void _PreUpdate()
        {
            ThrowIfDestroyed();

            if (!Enabled)
                return;

            foreach (var component in Components)
                component.Value._PreUpdate();
        }

        internal void _Update()
        {
            ThrowIfDestroyed();

            if (!Enabled)
                return;

            foreach (var component in Components)
                component.Value._Update();
        }

        internal void _PostUpdate()
        {
            ThrowIfDestroyed();

            if (!Enabled)
                return;

            foreach (var component in Components)
                component.Value._PostUpdate();
        }

        internal void _PreDraw()
        {
            ThrowIfDestroyed();

            if (!Enabled || !Visible)
                return;

            foreach (var component in Components)
                component.Value._PreDraw();
        }

        internal void _Draw()
        {
            ThrowIfDestroyed();

            if (!Enabled || !Visible)
                return;

            foreach (var component in Components)
                component.Value._Draw();
        }

        internal void _PostDraw()
        {
            ThrowIfDestroyed();

            if (!Enabled || !Visible)
                return;

            foreach (var component in Components)
                component.Value._PostDraw();
        }

        private void ThrowIfDestroyed()
        {
            if (Destroyed)
                throw new InvalidOperationException("This object has been destroyed.");
        }

        private void ThrowIfMismatchingTypes(Type type, Type targetType, string exceptionMessage)
        {
            if (type != targetType && !type.IsSubclassOf(targetType))
                throw new ArgumentException(exceptionMessage);
        }

        internal string GetDefaultName(int count = 0)
        {
            return GetUniqueName(GetType().Name);
        }

        internal string GetUniqueName(string name, int count = 0)
        {
            var result = name + (count > 0
                                                 ? (" (" + (count + 1) + ")")
                                                 : "");

            if (Siblings.Any(go => go.Name == result))
                return GetUniqueName(name, count + 1);

            return result;
        }

        public override string ToString()
        {
            return Name;
        }

        /// <summary>Returns a string representing this GameObject's hierarchy.</summary>
        public string ToHierarchyString(int indent = 0)
        {
            ThrowIfDestroyed();

            var sb = new StringBuilder();
            string strIndent = "——".Repeat(indent) + (indent > 0 ? "→" : "");
            string componentsString = String.Join(", ", Components.Where(component => component.Value.GetType() != typeof(Transform)).Select(component => component.Value.GetType().Name));

            sb.Append(strIndent + "'" + Name + (componentsString.Length > 0 ? "' (" + componentsString + ")" : "'"));

            foreach (var child in Children)
            {
                sb.Append("\n" + child.ToHierarchyString(indent + 1));
            }

            return sb.ToString();
        }

        /// <summary>
        ///  <para>Clones this GameObject and returns the new instance.</para>
        ///  <para>If parent is specified, the clone will be added as a child to that object, otherwise it'll be added to the original object's parent.</para>
        /// </summary>
        /// <param name="parent">If not null then this object will be added as a child to the given GameObject instead of the original object's parent.</param>
        public GameObject Clone(GameObject parent = null)
        {
            ThrowIfDestroyed();

            var clone = new GameObject(Game);

            clone.visible = visible;
            clone.enabled = enabled;

            if (parent == null)
            {
                if (Parent != null)
                    Parent.AddObject(clone);
                else
                    Manager.AddObject(clone);
            }
            else
                parent.AddObject(clone);

            clone.name = clone.GetUniqueName(name + (parent == null ? " (Clone)" : ""));

            // Clone components
            foreach (var keyval in Components)
            {
                Type type = keyval.Key;
                GameComponent component = keyval.Value;

                clone.AddComponent(component.Clone(clone));
            }

            // Clone children
            foreach (var child in Children)
            {
                child.Clone(clone);
            }
            
            // Fire events
            foreach (var component in clone.Components.Values)
            {
                component._Create();

                if (clone.enabled)
                    component._Enable(true);

                if (clone.visible)
                    component._FireShow();
            }

            return clone;
        }
    }
}