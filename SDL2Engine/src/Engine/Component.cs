using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace SDL2Engine
{
    /* Base class for all components
     * 
     * Components can be added to GameObjects
     * to provide complex functionality
     * 
     * To create custom components, inherit from Script instead
     */
    public class Component : EngineObject
    {
        protected GameObject gameObject = GameObject.Default;

        // this tracks if the component was enabled on the last frame
        // this is used to call OnEnable and OnDisable
        private bool wasEnabled = false;

        protected Component()
        {

        }

        public void Init(GameObject gameObject)
        {
            this.gameObject = gameObject;
            this.name = gameObject.GetName() + "." + this.GetType().Name;
            this.SetEnabled(gameObject.IsEnabled());
        }

        // Destructor in case the component is removed
        ~Component()
        {
            this.Disable();
        }

        // copy all members of the component
        public static T Instantiate<T>(T source, GameObject? gameObject = null) where T : Component, new()
        {
            T newComponent = new T();
            newComponent.Init(gameObject ?? source.gameObject);

            // use reflection to copy all members of the component
            FieldInfo[] fields = source.GetType().GetFields(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
            foreach (FieldInfo field in fields)
            {
                field.SetValue(newComponent, field.GetValue(source));
            }

            return newComponent;

        }

        public void SetEnabled(bool active)
        {
            if (active == this.enabled)
            {
                return;
            }
            this.enabled = active;

        }

        public bool IsEnabled()
        {
            return enabled;
        }

        public void Enable()
        {
            SetEnabled(true);
        }

        public void Disable()
        {
            SetEnabled(false);
        }

        public Scene? GetScene()
        {
            return gameObject.GetScene();
        }

        // Usefull methods to interact with other components
        public T? GetComponent<T>() where T : Component
        {
            return gameObject.GetComponent<T>();
        }

        public List<T> GetComponents<T>() where T : Component
        {
            return gameObject.GetComponents<T>();
        }

        public T? AddComponent<T>() where T : Component
        {
            return gameObject.AddComponent<T>();
        }

        public bool RemoveComponent(Component script)
        {
            return gameObject.RemoveComponent(script);
        }

        public bool RemoveComponent<T>() where T : Component
        {
            return gameObject.RemoveComponent<T>();
        }

        public Camera? GetCamera()
        {
            var scene = gameObject.GetScene();
            if (scene != null)
            {
                return scene.GetCamera();
            }
            return null;
        }

        public T? GetComponentInParent<T>() where T : Component
        {
            GameObject? parent = gameObject.GetParent();
            while (parent != null)
            {
                T? component = parent.GetComponent<T>();
                if (component != null)
                {
                    return component;
                }
                parent = parent.GetParent();
            }

            return null;
        }

        public T? GetComponentInChildren<T>() where T : Component
        {
            U? GetComponentInChildrenHelper<U>(GameObject parent) where U : Component
            {
                U? component = parent.GetComponent<U>();
                if (component != null)
                {
                    return component;
                }

                List<GameObject> children = parent.GetChildren();
                foreach (GameObject child in children)
                {
                    U? foundComponent = GetComponentInChildrenHelper<U>(child);
                    if (foundComponent != null)
                    {
                        return foundComponent;
                    }
                }

                return null;
            }

            return GetComponentInChildrenHelper<T>(gameObject);
        }

        public List<T> GetComponentsInChildren<T>() where T : Component
        {
            List<U> GetComponentsInChildrenHelper<U>(GameObject parent) where U : Component
            {
                List<U> foundComponents = new();
                List<U> components = parent.GetComponents<U>();
                foundComponents.AddRange(components);

                List<GameObject> children = parent.GetChildren();
                foreach (GameObject child in children)
                {
                    List<U> foundChildComponents = GetComponentsInChildrenHelper<U>(child);
                    foundComponents.AddRange(foundChildComponents);
                }

                return foundComponents;
            }

            return GetComponentsInChildrenHelper<T>(gameObject);
        }

        public List<T> GetComponentsInParent<T>() where T : Component
        {
            List<T> foundComponents = new();
            GameObject? parent = gameObject.GetParent();
            while (parent != null)
            {
                List<T> components = parent.GetComponents<T>();
                foundComponents.AddRange(components);
                parent = parent.GetParent();
            }

            return foundComponents;
        }

        public GameObject GetGameObject()
        {
            return gameObject;
        }

        // SendMessage method to call methods on other components
        public bool SendMessage(string methodName, object[]? args, out object? result)
        {
            result = null;
            var components = gameObject.GetComponents<Component>();
            foreach (Component component in components)
            {
                if (component != this) // Ensure not to call on itself unless intended
                {
                    MethodInfo? method = component.GetType().GetMethod(methodName, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
                    if (method != null)
                    {
                        try
                        {
                            result = method.Invoke(component, args);
                            return true; // Return true if at least one method is successfully invoked
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine("Exception occurred while invoking method: " + e.Message);
                            continue; // Optionally continue to try other components or break out
                        }
                    }
                }
            }

            return false; // Return false if no method was found and invoked
        }

    }

    public class Script : Component
    {

        public bool wasEnabled = false;

        // Base class for all scripts
        // These scripts have a few virtual methods that can be overridden for custom functionality

        // Virtual Methods in order of execution
        // Override these method to provide custom funcionality

        // 1.
        // This method is called once when the Component is created
        public virtual void Awake()
        {
            // Do nothing
        }

        // 2.
        // This method is called during the first frame the script is active
        public virtual void Start()
        {
            // Do nothing
        }

        // 3.
        // This method is called every frame
        public virtual void Update()
        {
            // Do nothing
        }

        // 4.
        // This method is called in fixed intervals to be more consistent with physics
        public virtual void FixedUpdate()
        {
            // Do nothing
        }

        // 5.
        // This method is called every time the script is enabled. Probaly during Update()
        public virtual void OnEnable()
        {
            // Do nothing
        }

        // 5.
        // This method is called every time the script is disabled. Probaly during Update()
        public virtual void OnDisable()
        {
            // Do nothing
        }

        // 6.
        // This method is called every frame after all Update() methods are called
        public virtual void LateUpdate()
        {
            // Do nothing
        }

        // 7.
        // This method is called when the script is destroyed
        public virtual void OnDestroy()
        {
            // Do nothing
        }


        public Transform transform
        {
            get
            {
                return gameObject.transform;
            }

            set
            {
                gameObject.transform = value;

            }
        }

    }

    /*
     * Base class for all game objects
     * 
     * This class is meant to be inherited from
     * and can be overridden to provide custom
     * functionality
     */

    public class Transform : Component
    {
        private Vec2D _position = new Vec2D();
        private Vec2D _localPosition = new Vec2D();

        public void UpdateChildren()
        {
            var children = gameObject.GetChildren();
            foreach (GameObject child in children)
            {
                child.SetParentPosition(position);
            }
        }

        // sets the position to a specific world position
        // adjust _localPosition to keep parent position the same
        public Vec2D position
        {
            get
            {
                return _position;
            }
            set
            {
                var diff = value - _position;
                localPosition += diff;
            }
        }

        // sets the position to a specific local position
        // this is the position relative to the parent
        public Vec2D localPosition
        {
            get
            {
                return _localPosition;
            }
            set
            {
                // remove local position from parent position
                var rootPosition = _position - _localPosition;
                _position = value + rootPosition;
                _localPosition = value;

                UpdateChildren();
            }
        }

        // sets the parent position directly
        public void SetParentPosition(Vec2D parentPosition)
        {
            _position = _localPosition + parentPosition;
            UpdateChildren();
        }

        public Vec2D GetPosition()
        {
            return position;
        }

        public Vec2D GetLocalPosition()
        {
            return localPosition;
        }

        private Vec2D _scale = new Vec2D(1, 1);
        private Vec2D _localScale = new Vec2D(1, 1);
        private Vec2D _rotation = new Vec2D();
        private Vec2D _localRotation = new Vec2D();


    }
}
