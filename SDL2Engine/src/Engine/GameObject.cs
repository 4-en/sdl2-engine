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

        public GameObject(string name = "GameObject", Scene? scene = null)
        {
            this.Parent = null;
            this.scene = null;
            this.name = name;
            this.transform.Init(this);
        }

        // Creates a new GameObject as a child of this GameObject
        public GameObject CreateChild(string name = "GameObject")
        {
            GameObject newGameObject = new GameObject(name, this.scene);
            this.AddChild(newGameObject);
            return newGameObject;
        }

        // creates a deep copy of the GameObject
        public static GameObject Instantiate(GameObject source)
        {

            // setups the new object
            GameObject newObject = new GameObject(source.name);
            newObject.layer = source.layer;
            newObject.enabled = source.enabled;
            newObject.transform = source.transform;
            newObject.Parent = source.Parent;
            newObject.scene = source.scene;

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

        public void RemoveParent()
        {
            this.Parent = null;
            this.SetParentPosition(new Vec2D());
        }

        public Scene? GetScene()
        {
            return scene;
        }

        public void SetScene(Scene? scene)
        {
            this.scene = scene;
        }

        public Vec2D GetPosition()
        {
            return this._transform.GetPosition();
        }

        public void SetPosition(Vec2D position)
        {
            this._transform.position = position;

        }

        public void SetLocalPosition(Vec2D position)
        {
            this._transform.localPosition = position;
        }

        public void SetParentPosition(Vec2D position)
        {
            this._transform.SetParentPosition(position);
        }

        public void AddChild(GameObject child)
        {
            child.SetParent(this);
            child.SetScene(this.scene);
            children.Add(child);
            child.SetParentPosition(this.GetPosition());



        }

        public void RemoveChild(GameObject child)
        {
            // check if child is in children
            if (children.Contains(child))
            {
                children.Remove(child);
                child.SetParentPosition(new Vec2D());
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

        public T? AddComponent<T>() where T : Component
        {
            
            T? newComponent = (T?)Activator.CreateInstance(typeof(T));
            
            if (newComponent != null)
            {


                newComponent.Init(this);

                // add specific built-in components to fields instead of adding them to the components list
                // GameObject should only have one of each of these components
                // if multiple colliders or physics bodies are needed, use a child object or CompositeCollider
                if (newComponent is Collider collider)
                {
                    this._collider = collider;
                    return newComponent;
                }

                if (newComponent is PhysicsBody physicsBody)
                {
                    this._physicsBody = physicsBody;
                    return newComponent;
                }

                if (newComponent is Transform transform)
                {
                    _transform = transform;
                    return newComponent;
                }

                if (newComponent is Drawable drawable)
                {
                    _drawable = drawable;
                    return newComponent;
                }


                components.Add(newComponent);
            } else
            {
                Console.WriteLine("Failed to create component of type " + typeof(T).Name);
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

        // Gets first component of type T
        public T? GetComponent<T>() where T : Component
        {
            // if component is transform, physics body, collider, or drawable, return the field directly
            if (typeof(T) == typeof(Transform))
            {
                return (T)(Component)_transform;
            }

            if (typeof(T) == typeof(Collider))
            {
                if (_collider == null)
                {
                    return null;
                }
                return (T)(Component)_collider;
            }

            if (typeof(T) == typeof(PhysicsBody))
            {
                if (_physicsBody == null)
                {
                    return null;
                }
                return (T)(Component)_physicsBody;
            }

            if (typeof(T) == typeof(Drawable))
            {
                if (_drawable == null)
                {
                    return null;
                }
                return (T)(Component)_drawable;
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
            if (typeof(T) == typeof(Transform))
            {
                List<T> transformList = [(T)(Component)_transform];
                return transformList;
            }

            List<T> foundComponents = new List<T>();
            foreach (Component script in components)
            {
                if (script is T)
                {
                    foundComponents.Add((T)script);
                }
            }

            return foundComponents;
        }

        public List<Component> GetScripts()
        {
            return components;
        }

        public void Start()
        {
            foreach (Component script in components)
            {

                // TODO: optimize this by keeping a list of scripts in scene
                // check if script is a script
                if (script is Script)
                {
                    ((Script)script).Start();
                }
            }

            foreach (GameObject child in children)
            {
                child.Start();
            }
        }

        public void Update()
        {
            // TODO: optimize this by keeping a list of scripts in scene
            // Update all scripts
            for(int i = 0; i < components.Count; i++)
            {
                // check if script is a script
                if (components[i] is Script)
                {
                    ((Script)components[i]).Update();
                }
            }

            // Update all children
            for(int i = 0; i < children.Count; i++)
            {
                children[i].Update();
            }
        }

        // Override this method to draw custom graphics
        // By default, this method does nothing
        public void Draw(Camera camera)
        {
            if (_drawable != null)
            {
                _drawable.Draw(camera);
            }

            // Draw all children
            foreach (GameObject child in children)
            {
                child.Draw(camera);
            }
        }

    }
}
