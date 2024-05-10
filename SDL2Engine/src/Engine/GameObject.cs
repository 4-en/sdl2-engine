using System.Reflection;

namespace SDL2Engine
{
    public class GameObject : EngineObject
    {
        public uint layer = 0;
        // Position of the GameObject
        protected Transform _transform = new Transform();
        protected Collider? _collider;
        protected PhysicsBody? _physicsBody;
        protected Drawable? _drawable;
        protected GameObject? Parent { get; set; }
        private readonly List<GameObject> children = [];
        private readonly List<Component> components = [];
        private static readonly GameObject defaultObject = new("default");

        // if true, the object will not be destroyed when the scene is changed and instead will be moved
        // to a buffer of persistent objects where it can be accessed by any scene
        private bool persistent = false;

        public GameObject(string name = "GameObject", Scene? scene = null)
        {
            this.Parent = null;

            if (scene == null)
            {
                scene = SceneManager.GetActiveScene();
            }

            if (scene != null)
            {
                scene.AddGameObject(this);
            }

            this.name = name;
            this.transform.Init(this);
        }

        public GameObject(GameObject parent, string name = "GameObject")
        {
            this.Parent = parent;

            if (parent != null)
            {
                this.scene = parent.GetScene();
                parent.AddChild(this);
            }
            else
            {
                this.scene = SceneManager.GetActiveScene();
            }

            this.scene?.AddGameObject(this);

            this.name = name;
            this.transform.Init(this);
        }

        public override void Dispose()
        {
            base.Dispose();

            // Dispose all components and children
            foreach (Component component in components)
            {
                component.Dispose();
            }

            transform.Dispose();
            physicsBody?.Dispose();
            collider?.Dispose();
            drawable?.Dispose();

            foreach (GameObject child in children)
            {
                child.Dispose();
            }
        }

        // Creates a new GameObject as a child of this GameObject
        public GameObject CreateChild(string name = "GameObject")
        {
            GameObject newGameObject = new GameObject(this, name);
            this.AddChild(newGameObject);
            return newGameObject;
        }

        // creates a deep copy of the GameObject
        public static GameObject Instantiate(GameObject source)
        {

            // setups the new object
            GameObject newObject = new GameObject(source.name, source.scene);
            newObject.layer = source.layer;
            newObject.enabled = source.enabled;

            // TODO: check if this works
            // maybe clone all fields of the object instead of copying the reference
            // otherwise, the new object will be a reference to the old object, sharing the same transform, components, and children
            newObject.transform = source.transform;
            newObject.Parent = source.Parent;

            // copy all components
            foreach (Component script in source.components)
            {
                Type type = script.GetType();
                // create a new instance of the script
                object? o = Activator.CreateInstance(type);
                if (o == null || !(o is Component))
                {
                    Console.WriteLine("Failed to create component of type " + type.Name);
                    continue;
                }
                Component newComponent = (Component)o;

                // use reflection to copy all members of the component
                FieldInfo[] fields = type.GetFields(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
                foreach (FieldInfo field in fields)
                {
                    field.SetValue(newComponent, field.GetValue(script));
                }

                // TODO: check if this works
                // this hasn't been tested yet
                newObject.AddComponent(newComponent);
            }

            // copy all children
            foreach (GameObject child in source.children)
            {
                GameObject newChild = GameObject.Instantiate(child);
                // fix references to new object
                newChild.Parent = newObject;
                newObject.children.Add(newChild);
            }

            return newObject;

        }

        public void SetPersistent(bool persistent)
        {
            this.persistent = persistent;
        }

        public bool IsPersistent()
        {
            return this.persistent;
        }


        public static GameObject Default
        {
            get
            {
                return defaultObject;
            }
        }

        public string GetName()
        {
            return name;
        }

        public void UpdateChildPositions()
        {
            this._transform.UpdateChildren();
        }

        public Transform transform
        {
            get
            {
                return _transform;
            }

            set
            {
                // instead of switching the transform, update the position
                _transform.position = value.position;
            }
        }

        public Collider? collider
        {
            get
            {
                return _collider;
            }
        }

        public bool HasCollider()
        {
            return _collider != null;
        }

        public PhysicsBody? physicsBody
        {
            get
            {
                return _physicsBody;
            }
        }

        public bool HasPhysicsBody()
        {
            return _physicsBody != null;
        }

        public Drawable? drawable
        {
            get
            {
                return _drawable;
            }
        }

        public bool IsDrawable()
        {
            return _drawable != null;
        }

        public void SetEnabled(bool active)
        {
            this.enabled = active;

            for (int i = 0; i < components.Count; i++)
            {
                components[i].SetEnabled(active);
            }

            collider?.SetEnabled(active);
            physicsBody?.SetEnabled(active);
            drawable?.SetEnabled(active);


            for (int i = 0; i < children.Count; i++)
            {
                children[i].SetEnabled(active);
            }
        }

        public void Enable()
        {
            this.SetEnabled(true);
        }

        public void Disable()
        {
            this.SetEnabled(false);
        }

        public bool IsEnabled()
        {
            return enabled;
        }

        public bool IsDisabled()
        {
            return !enabled;
        }

        public bool ToggleEnabled()
        {
            this.SetEnabled(!enabled);
            return enabled;
        }

        public void SetParent(GameObject? parent)
        {
            this.Parent = parent;

            if (parent != null)
            {
                this.SetParentPosition(parent.GetPosition());
            }
            else
            {
                this.SetParentPosition(new Vec2D());
            }
        }

        public GameObject? GetParent()
        {
            return Parent;
        }

        public GameObject? GetDeepParent()
        {
            GameObject? parent = this.Parent;
            while (parent != null && parent.Parent != null)
            {
                parent = parent.Parent;
            }

            return parent;
        }

        public void RemoveParent()
        {
            this.Parent = null;
            this.SetParentPosition(new Vec2D());
        }


        public Vec2D GetPosition()
        {
            return this._transform.GetPosition();
        }
        // Set the absolute world position of the GameObject
        public void SetPosition(Vec2D position)
        {
            this._transform.position = position;

        }
        // Use this to set the position of the GameObject relative to the parent
        public void SetLocalPosition(Vec2D position)
        {
            this._transform.localPosition = position;
        }

        // Set the position of the parent GameObject
        // world position = parent position + local position
        public void SetParentPosition(Vec2D position)
        {
            this._transform.SetParentPosition(position);
        }

        public GameObject AddChild()
        {
            GameObject newChild = new GameObject();
            return this.AddChild(newChild);
        }

        public GameObject AddChild(string name)
        {
            GameObject newChild = new GameObject(name);
            return this.AddChild(newChild);
        }

        public GameObject AddChild(GameObject child)
        {
            child.SetParent(this);

            Scene? childScene = child.GetScene();
            if (childScene != null)
            {
                if (childScene != this.scene)
                {
                    /*
                    * this could destroy resources that are still in use
                    * its better to completely forbid adding a GameObject to a different scene
                    childScene.Destroy(child);
                    this.GetScene()?.AddGameObject(child);
                    */
                    throw new Exception("Cannot add GameObject to a different scene");
                }
                else
                {
                    childScene.RemoveGameObjectFromRoot(child);
                }
            }

            child.SetScene(this.scene);
            children.Add(child);

            child.SetParentPosition(this.GetPosition());

            // TODO: remove child from scene if necessary
            // if the old scene is null or different from the new scene, add components to the new scene

            return child;

        }

        public void RemoveChild(GameObject child)
        {
            // check if child is in children
            if (children.Contains(child))
            {
                Destroy(child);
            }
        }

        public List<GameObject> GetChildren()
        {
            return children;
        }

        // Gets first child of type T
        public T? GetChild<T>() where T : GameObject
        {
            foreach (GameObject child in children)
            {
                if (child is T)
                {
                    return (T)child;
                }
            }

            // no child of type T found, search recursively
            foreach (GameObject child in children)
            {
                T? foundChild = child.GetChild<T>();
                if (foundChild != null)
                {
                    return foundChild;
                }
            }
            return null;
        }

        /*
         * Adds a component to the GameObject
         * Usually used to add a new Component to the GameObject
         * by only specifying the type of the Component
         *
         * Can also be used to add an existing Component to the GameObject
         * this should only be done when the Component is not already attached to a GameObject
         * and could lead to unexpected behavior if the Component is already attached to a GameObject
         */
        public T AddComponent<T>(T? instance = null) where T : Component, new()
        {


            T newComponent = instance ?? new T();



            newComponent.Init(this);

            this.scene?.AddComponent(newComponent);

            // add specific built-in components to fields instead of adding them to the components list
            // GameObject should only have one of each of these components
            // if multiple colliders or physics bodies are needed, use a child object or CompositeCollider
            if (newComponent is Collider collider)
            {
                this._collider = collider;
            }

            else if (newComponent is PhysicsBody physicsBody)
            {
                this._physicsBody = physicsBody;
            }

            else if (newComponent is Transform transform)
            {
                _transform = transform;
            }

            else if (newComponent is Drawable drawable)
            {
                _drawable = drawable;
            }

            else if (newComponent is Script script)
            {
                components.Add(newComponent);
                // Commented out because Awake is called by the scene during the first step of the update loop
                // when a new component is added to the scene
                // script.Awake();
            }

            else
            {
                components.Add(newComponent);
            }

            return newComponent;


        }

        public bool RemoveComponent(Component script)
        {
            return components.Remove(script);
        }

        public bool RemoveComponent<T>() where T : Component
        {
            foreach (Component script in components)
            {
                if (script is T)
                {
                    return components.Remove(script);
                }
            }

            return false;
        }

        /*
         * Finds a child GameObject by name
         * Returns the first child with the specified name
         * or null if no child with the specified name was found
         */
        public GameObject? FindChild(string name)
        {
            foreach (GameObject child in children)
            {
                if (child.name == name)
                {
                    return child;
                }
            }

            foreach (GameObject child in children)
            {
                GameObject? found = child.FindChild(name);
                if (found != null)
                {
                    return found;
                }
            }

            return null;
        }
        /*
         * Finds a GameObject by name
         * Returns the first GameObject with the specified name
         * or null if no GameObject with the specified name was found
         * 
         * This method can be very slow if used frequently.
         * Best practice is to store the result in a variable
         * and then use that variable for future references
         * 
         * Search Order:
         * 1. Search this GameObject
         * 2. Search children
         * 3. Search all GameObjects in the scene
         *
         * This method does not search other scenes
         * To search withing all scenes, use SceneManager.Find()
         */
        public GameObject? Find(string name)
        {
            if (this.name == name)
            {
                return this;
            }

            // search children
            GameObject? found = this.FindChild(name);
            if (found != null)
            {
                return found;
            }

            // search all GameObjects in the scene
            // this does not search other scenes
            return this.scene?.Find(name);
        }

        public T? FindComponentInChildren<T>() where T : Component
        {
            foreach (GameObject child in children)
            {
                T? found = child.GetComponent<T>();
                if (found != null)
                {
                    return found;
                }
            }

            return null;
        }

        /*
         * Finds a component of type T
         * 
         * This method can be very slow if used frequently.
         * Best practice is to store the result in a variable
         * and then use that variable for future references
         * 
         * Search Order:
         * 1. Search this GameObject
         * 2. Search children
         * 3. Search all GameObjects in the scene
         * 
         */
        public T? FindComponent<T>() where T : Component
        {
            T? component = GetComponent<T>();
            if (component != null)
            {
                return component;
            }

            // search children
            component = FindComponentInChildren<T>();
            if (component != null)
            {
                return component;
            }

            // search all GameObjects in the scene
            // this does not search other scenes
            return this.scene?.FindComponent<T>();
        }

        // Gets first component of type T
        public T? GetComponent<T>() where T : Component
        {
            // if component is transform, physics body, collider, or drawable, return the field directly
            if (transform is T t_component)
            {
                return t_component;
            }

            if (drawable is T d_component)
            {
                return d_component;
            }

            if (collider is T c_component)
            {
                return c_component;
            }

            if (physicsBody is T p_component)
            {
                return p_component;
            }

            foreach (Component script in components)
            {
                if (script is T)
                {
                    return (T)script;
                }
            }

            return null;
        }

        // Gets all components of type T
        public List<T> GetComponents<T>() where T : Component
        {
            List<T> foundComponents = new List<T>();
            if (transform is T t_component)
            {
                foundComponents.Add(t_component);
            }

            if (drawable is T d_component)
            {
                foundComponents.Add(d_component);
            }

            if (collider is T c_component)
            {
                foundComponents.Add(c_component);
            }

            if (physicsBody is T p_component)
            {
                foundComponents.Add(p_component);
            }


            foreach (Component component in components)
            {
                if (component is T)
                {
                    foundComponents.Add((T)component);
                }
            }

            return foundComponents;
        }

        public List<Component> GetAllComponents()
        {
            return GetComponents<Component>();
        }

    }
}
