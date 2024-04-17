using SDL2;
using System.Reflection;

namespace SDL2Engine
{

    public struct Vec2D
    {
        public double x=0;
        public double y=0;
        public double z=0; // Vec2D has a z component for layering

        public Vec2D(double x = 0, double y = 0, double z = 0)
        {
            this.x = x;
            this.y = y;
            this.z = z;
        }

        public static Vec2D operator +(Vec2D a, Vec2D b)
        {
            return new Vec2D(a.x + b.x, a.y + b.y, a.z);
        }

        public static Vec2D operator -(Vec2D a, Vec2D b)
        {
            return new Vec2D(a.x - b.x, a.y - b.y, a.z);
        }

        public static Vec2D operator *(Vec2D a, double b)
        {
            return new Vec2D(a.x * b, a.y * b, a.z);
        }

        public static Vec2D operator /(Vec2D a, double b)
        {
            return new Vec2D(a.x / b, a.y / b, a.z);
        }

        public static Vec2D operator *(double a, Vec2D b)
        {
            return new Vec2D(a * b.x, a * b.y, b.z);
        }

        public static Vec2D operator /(double a, Vec2D b)
        {
            return new Vec2D(a / b.x, a / b.y, b.z);
        }

        public static Vec2D operator *(Vec2D a, Vec2D b)
        {
            return new Vec2D(a.x * b.x, a.y * b.y, a.z);
        }

        public static Vec2D operator /(Vec2D a, Vec2D b)
        {
            return new Vec2D(a.x / b.x, a.y / b.y, a.z);
        }

        public static Vec2D operator -(Vec2D a)
        {
            return new Vec2D(-a.x, -a.y, a.z);
        }

        public static Vec2D operator +(Vec2D a)
        {
            return new Vec2D(+a.x, +a.y, a.z);
        }

        public static bool operator ==(Vec2D a, Vec2D b)
        {
            return a.x == b.x && a.y == b.y;
        }

        public static bool Equals3D(Vec2D a, Vec2D b)
        {
            return a.x == b.x && a.y == b.y && a.z == b.z;
        }

        public override bool Equals(object? obj)
        {
            return obj is Vec2D v && this == v;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public static bool operator !=(Vec2D a, Vec2D b)
        {
            return a.x != b.x || a.y != b.y;
        }

        public static double Dot(Vec2D a, Vec2D b)
        {
            return a.x * b.x + a.y * b.y;
        }

        public static double Dot3D(Vec2D a, Vec2D b)
        {
            return a.x * b.x + a.y * b.y + a.z * b.z;
        }

        public double Length()
        {
            return Math.Sqrt(x * x + y * y);
        }

        public double Length3D()
        {
            return Math.Sqrt(x * x + y * y + z * z);
        }

        public double LengthSquared()
        {
            return x * x + y * y;
        }

        public double LengthSquared3D()
        {
            return x * x + y * y + z * z;
        }

        public static double Distance(Vec2D a, Vec2D b)
        {
            return (a - b).Length();
        }

        public static double Distance3D(Vec2D a, Vec2D b)
        {
            Vec2D diff = new(a.x - b.x, a.y - b.y, a.z - b.z);
            return diff.Length3D();
        }

        public static double DistanceSquared(Vec2D a, Vec2D b)
        {
            return (a - b).LengthSquared();
        }

        public static double DistanceSquared3D(Vec2D a, Vec2D b)
        {
            Vec2D diff = new(a.x - b.x, a.y - b.y, a.z - b.z);
            return diff.LengthSquared3D();
        }

        public Vec2D Normalize()
        {
            return this / Length();
        }

        public static bool operator >(Vec2D a, Vec2D b)
        {
            return a.LengthSquared() > b.LengthSquared();
        }

        public static bool operator <(Vec2D a, Vec2D b)
        {
            return a.LengthSquared() < b.LengthSquared();
        }

        public static bool operator >=(Vec2D a, Vec2D b)
        {
            return a.LengthSquared() >= b.LengthSquared();
        }

        public static bool operator <=(Vec2D a, Vec2D b)
        {
            return a.LengthSquared() <= b.LengthSquared();
        }

    }

    public struct Vec3D
    {
        public double x;
        public double y;
        public double z;

        public Vec3D(double x = 0, double y = 0, double z = 0)
        {
            this.x = x;
            this.y = y;
            this.z = z;
        }

        public static Vec3D operator +(Vec3D a, Vec3D b)
        {
            return new Vec3D(a.x + b.x, a.y + b.y, a.z + b.z);
        }

        public static Vec3D operator -(Vec3D a, Vec3D b)
        {
            return new Vec3D(a.x - b.x, a.y - b.y, a.z - b.z);
        }

        public static Vec3D operator *(Vec3D a, double b)
        {
            return new Vec3D(a.x * b, a.y * b, a.z * b);
        }

        public static Vec3D operator /(Vec3D a, double b)
        {
            return new Vec3D(a.x / b, a.y / b, a.z / b);
        }

        public static Vec3D operator *(double a, Vec3D b)
        {
            return new Vec3D(a * b.x, a * b.y, a * b.z);
        }

        public static Vec3D operator /(double a, Vec3D b)
        {
            return new Vec3D(a / b.x, a / b.y, a / b.z);
        }

        public static Vec3D operator *(Vec3D a, Vec3D b)
        {
            return new Vec3D(a.x * b.x, a.y * b.y, a.z * b.z);
        }

        public static Vec3D operator /(Vec3D a, Vec3D b)
        {
            return new Vec3D(a.x / b.x, a.y / b.y, a.z / b.z);
        }

        public static Vec3D operator -(Vec3D a)
        {
            return new Vec3D(-a.x, -a.y, -a.z);
        }

        public static Vec3D operator +(Vec3D a)
        {
            return new Vec3D(+a.x, +a.y, +a.z);
        }

        public static bool operator ==(Vec3D a, Vec3D b)
        {
            return a.x == b.x && a.y == b.y && a.z == b.z;
        }

        public override bool Equals(object? obj)
        {
            return obj is Vec3D v && this == v;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public static bool operator !=(Vec3D a, Vec3D b)
        {
            return a.x != b.x || a.y != b.y || a.z != b.z;
        }

        public static double Dot(Vec3D a, Vec3D b)
        {
            return a.x * b.x + a.y * b.y + a.z * b.z;
        }

        public static Vec3D Cross(Vec3D a, Vec3D b)
        {
            return new Vec3D(a.y * b.z - a.z * b.y, a.z * b.x - a.x * b.z, a.x * b.y - a.y * b.x);
        }

        public double Length()
        {
            return Math.Sqrt(x * x + y * y + z * z);
        }

        public double Length2D()
        {
            return Math.Sqrt(x * x + y * y);
        }

        public double LengthSquared()
        {
            return x * x + y * y + z * z;
        }

        public double LengthSquared2D()
        {
            return x * x + y * y;
        }

        public static double Distance(Vec3D a, Vec3D b)
        {
            return (a - b).Length();
        }

        public static double Distance2D(Vec3D a, Vec3D b)
        {
            return (a - b).Length2D();
        }

        public Vec3D Normalize()
        {
            return this / Length();
        }

        public static bool operator >(Vec3D a, Vec3D b)
        {
            return a.LengthSquared() > b.LengthSquared();
        }

        public static bool operator <(Vec3D a, Vec3D b)
        {
            return a.LengthSquared() < b.LengthSquared();
        }

        public static bool operator >=(Vec3D a, Vec3D b)
        {
            return a.LengthSquared() >= b.LengthSquared();
        }

        public static bool operator <=(Vec3D a, Vec3D b)
        {
            return a.LengthSquared() <= b.LengthSquared();
        }

    }

    /* Base class for all scripts
     * 
     * This class is meant to be inherited from
     * and can be overridden to provide custom
     * functionality to GameObjects.
     */
    public class Component
    {
        protected GameObject gameObject = GameObject.Default;
        private bool active = false;

        protected  Component()
        {
            
        }

        public void Init(GameObject gameObject)
        {
            this.gameObject = gameObject;
            this.SetActive(gameObject.IsActive());
        }

        // Destructor in case the component is removed
        ~Component()
        {
            this.SetActive(false);
        }

        public void SetActive(bool active)
        {
            if (active == this.active)
            {
                return;
            }
            this.active = active;

            if (active)
            {
                GetScene()?.ActivateComponent(this);
            }
            else
            {
                GetScene()?.DeactivateComponent(this);
            }
        }

        public bool IsActive()
        {
            return active;
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
        // This method is called every time the script is enabled. Probaly during Update()
        public virtual void OnEnable()
        {
            // Do nothing
        }

        // 4.
        // This method is called every time the script is disabled. Probaly during Update()
        public virtual void OnDisable()
        {
            // Do nothing
        }

        // 5.
        // This method is called every frame after all Update() methods are called
        public virtual void LateUpdate()
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

    public class Engine
    {

        private Scene scene;
        private bool running = false;
        private bool showDebug = false;
        private bool fullscreen = false;
        public static UInt32 targetFPS = 60;
        public static int windowWidth = 800;
        public static int windowHeight = 600;


        // SDL variables
        private IntPtr window;
        public static IntPtr renderer;
        private SDL.SDL_Event sdlEvent;

        public Engine(Scene scene)
        {
            this.scene = scene;
        }

        private void Init()
        {
            // Initialize SDL2
            if (SDL.SDL_Init(SDL.SDL_INIT_VIDEO) < 0)
            {
                Console.WriteLine("SDL could not initialize! SDL Error: " + SDL.SDL_GetError());
                return;
            }

            if (SDL_ttf.TTF_Init() < 0)
            {
                Console.WriteLine("SDL_ttf could not initialize! SDL_ttf Error: " + SDL.SDL_GetError());
                return;
            }

            // Create window
            window = SDL.SDL_CreateWindow("SDL2 Engine Test",
                SDL.SDL_WINDOWPOS_UNDEFINED, SDL.SDL_WINDOWPOS_UNDEFINED,
                windowWidth, windowHeight, SDL.SDL_WindowFlags.SDL_WINDOW_SHOWN | SDL.SDL_WindowFlags.SDL_WINDOW_RESIZABLE);
            if (window == IntPtr.Zero)
            {
                Console.WriteLine("Window could not be created! SDL Error: " + SDL.SDL_GetError());
                return;
            }

            // Create renderer for window
            renderer = SDL.SDL_CreateRenderer(window, -1, SDL.SDL_RendererFlags.SDL_RENDERER_ACCELERATED);
            if (renderer == IntPtr.Zero)
            {
                Console.WriteLine("Renderer could not be created! SDL Error: " + SDL.SDL_GetError());
                return;
            }

            // Initialize renderer color
            SDL.SDL_SetRenderDrawColor(renderer, 0xFF, 0xFF, 0xFF, 0xFF);


        }

        private void HandleKeyboardEvent(SDL.SDL_KeyboardEvent keyEvent)
        {

            // add key to pressed keys
            if (keyEvent.repeat == 0)
            {
                Input.SetKeyPressed(((uint)keyEvent.keysym.sym));
            }

            switch (keyEvent.keysym.sym)
            {
                case SDL.SDL_Keycode.SDLK_ESCAPE:
                    running = false;
                    break;
                case SDL.SDL_Keycode.SDLK_F3:
                    showDebug = !showDebug;
                    break;
                case SDL.SDL_Keycode.SDLK_F11:
                    fullscreen = !fullscreen;
                    if (fullscreen)
                    {
                        SDL.SDL_SetWindowFullscreen(window, (uint)SDL.SDL_WindowFlags.SDL_WINDOW_FULLSCREEN_DESKTOP);
                    }
                    else
                    {
                        SDL.SDL_SetWindowFullscreen(window, 0);
                    }
                    break;
                default:
                    break;
            }
        }

        private void HandleEvents()
        {

            // clear keys
            Input.ClearInputs();

            while (SDL.SDL_PollEvent(out sdlEvent) != 0)
            {
                switch (sdlEvent.type)
                {
                    case SDL.SDL_EventType.SDL_QUIT:
                        running = false;
                        break;

                    case SDL.SDL_EventType.SDL_MOUSEMOTION:
                        // Set mouse position in Input
                        Input.SetMousePosition(new Vec2D(sdlEvent.motion.x, sdlEvent.motion.y));
                        break;

                    // mouse button events
                    case SDL.SDL_EventType.SDL_MOUSEBUTTONDOWN:
                        Input.SetMouseButtonDown(sdlEvent.button.button - 1);
                        Input.SetMouseButtonPressed(sdlEvent.button.button - 1);
                        break;

                    case SDL.SDL_EventType.SDL_MOUSEBUTTONUP:
                        Input.SetMouseButtonReleased(sdlEvent.button.button - 1);
                        break;


                    // Handle keyboard events
                    case SDL.SDL_EventType.SDL_KEYDOWN:
                        this.HandleKeyboardEvent(sdlEvent.key);
                        break;

                    // window resize event
                    case SDL.SDL_EventType.SDL_WINDOWEVENT:
                        if (sdlEvent.window.windowEvent == SDL.SDL_WindowEventID.SDL_WINDOWEVENT_RESIZED)
                        {
                            windowWidth = sdlEvent.window.data1;
                            windowHeight = sdlEvent.window.data2;
                        }
                        break;
                    default:
                        break;

                }
            }
        }

        private void Update()
        {
            scene.Update();
        }

        private void DrawDebug()
        {
           // Draw debug information

            
            SDL.SDL_Color color = new SDL.SDL_Color();
            color.r = 255;
            color.g = 255;
            color.b = 255;
            color.a = 255;
            IntPtr font = SDL_ttf.TTF_OpenFont("Assets/Fonts/Roboto-Regular.ttf", 24);

            if (font == IntPtr.Zero)
            {
                Console.WriteLine("Failed to load font! SDL_ttf");
                return;
            }

            string[] debugStrings =
            {
                "FPS: " + Time.GetFPS().ToString("0.00"),
                "Update Duration: " + Time.updateDuration.ToString("0.00") + " ms",
                "Draw Duration: " + Time.drawDuration.ToString("0.00") + " ms",
                "Total Duration: " + Time.totalDuration.ToString("0.00") + " ms",
                "Free Duration: " + Time.freeDuration.ToString("0.00") + " ms"
            };

            for (int i = 0; i < debugStrings.Length; i++)
            {
                IntPtr surface = SDL_ttf.TTF_RenderText_Solid(font, debugStrings[i], color);
                IntPtr texture = SDL.SDL_CreateTextureFromSurface(renderer, surface);

                int texW = 0;
                int texH = 0;
                SDL.SDL_QueryTexture(texture, out _, out _, out texW, out texH);

                SDL.SDL_Rect dst = new SDL.SDL_Rect();
                dst.x = 10;
                dst.y = 10 + i * 30;
                dst.w = texW;
                dst.h = texH;

                SDL.SDL_RenderCopy(renderer, texture, IntPtr.Zero, ref dst);

                SDL.SDL_DestroyTexture(texture);
                SDL.SDL_FreeSurface(surface);
            }

            SDL_ttf.TTF_CloseFont(font);
        }

        private void Draw()
        {
            // Clear screen
            SDL.SDL_SetRenderDrawColor(renderer, 0x1F, 0x1F, 0x1F, 0xFF); // Dark gray
            SDL.SDL_RenderClear(renderer);

            scene.Draw(scene.GetCamera());

            if (showDebug)
            {
                DrawDebug();
            }

            // Update screen
            SDL.SDL_RenderPresent(renderer);
        }

        public void Run()
        {
            if (running)
            {
                return;
            }
            running = true;

            // Initialize SDL2
            Init();


            Time.lastUpdateTime = (double)SDL.SDL_GetTicks();
            Time.lastDrawTime = (double)SDL.SDL_GetTicks();

            Time.time = 0;
            Time.tick = 0;
            uint beforeUpdate = SDL.SDL_GetTicks();
            while (running)
            {
                // Handle events on queue
                HandleEvents();

                beforeUpdate = SDL.SDL_GetTicks();
                Time.deltaTime = (beforeUpdate - Time.lastUpdateTime) / 1000.0;
                Time.time += Time.deltaTime;
                Time.tick++;
                this.Update();
                Time.lastUpdateTime = SDL.SDL_GetTicks();
                Time.updateDuration = Time.lastUpdateTime - beforeUpdate;
                this.Draw();
                Time.lastDrawTime = SDL.SDL_GetTicks();
                Time.drawDuration = Time.lastDrawTime - Time.lastUpdateTime;

                // Cap the frame rate
                Time.totalDuration = Time.lastDrawTime - beforeUpdate;
                Time.freeDuration = 0;
                if (Time.totalDuration < 1000.0 / targetFPS)
                {
                    Time.freeDuration = 1000.0 / targetFPS - Time.totalDuration;
                    SDL.SDL_Delay((uint)(1000.0 / targetFPS - Time.totalDuration));
                }

            }

            // Destroy window
            SDL.SDL_DestroyRenderer(renderer);
            SDL.SDL_DestroyWindow(window);

            // Quit SDL subsystems
            SDL.SDL_Quit();

        }
    }
}
