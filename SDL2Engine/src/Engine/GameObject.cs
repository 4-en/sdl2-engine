using System.Reflection;

namespace SDL2Engine
{
    public class GameObject
    {
        public uint layer = 0;
        private bool active = true;
        public string name = "GameObject";
        // Position of the GameObject
        protected Transform _transform = new Transform();
        protected GameObject? Parent { get; set; }
        protected Scene? scene;
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
            newObject.active = source.active;
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

        public void SetActive(bool active)
        {
            this.active = active;

            for (int i = 0; i < components.Count; i++)
            {
                components[i].SetActive(active);
            }
        }

        public bool IsActive()
        {
            return active;
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

        protected void SetScene(Scene? scene)
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
                // prevent transform from being added as a component
                if (newComponent is Transform)
                {
                    return null;
                }

                newComponent.Init(this);
                components.Add(newComponent);
                //newComponent.Start();
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
            // if component is transform, return transform
            if (typeof(T) == typeof(Transform))
            {
                return (T)(Component)_transform;
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
            // draw drawables
            foreach (Component script in components)
            {
                if (script is Drawable)
                {
                    ((Drawable)script).Draw(camera);
                }
            }

            // Draw all children
            foreach (GameObject child in children)
            {
                child.Draw(camera);
            }
        }

    }
}
