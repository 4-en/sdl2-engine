﻿using SDL2;
using System.Diagnostics;
using System.Reflection;
using static System.Diagnostics.Stopwatch;

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

    public class Rect
    {
        public double x;
        public double y;
        public double w;
        public double h;

        public Rect(double x = 0, double y = 0, double w = 0, double h = 0)
        {
            this.x = x;
            this.y = y;
            this.w = w;
            this.h = h;
        }

        public bool Contains(Vec2D point)
        {
            return point.x >= x && point.x <= x + w && point.y >= y && point.y <= y + h;
        }

        public bool Intersects(Rect other)
        {
            return x < other.x + other.w && x + w > other.x && y < other.y + other.h && y + h > other.y;
        }

        public static Rect operator +(Rect a, Vec2D b)
        {
            return new Rect(a.x + b.x, a.y + b.y, a.w, a.h);
        }

        public static Rect operator -(Rect a, Vec2D b)
        {
            return new Rect(a.x - b.x, a.y - b.y, a.w, a.h);
        }

        public static Rect operator *(Rect a, double b)
        {
            return new Rect(a.x * b, a.y * b, a.w * b, a.h * b);
        }

        public static Rect operator /(Rect a, double b)
        {
            return new Rect(a.x / b, a.y / b, a.w / b, a.h / b);
        }

        // get size
        public Vec2D GetSize()
        {
            return new Vec2D(w, h);
        }

    }

    public class EngineObject
    {
        protected static Random random = new Random(DateTime.Now.Millisecond);

        protected string name = "unnamed";
        protected bool active = true;
        protected Scene? scene = null;
        protected uint uid = GetRandomUID();

        public EngineObject(string name = "unnamed")
        {
            this.name = name;
        }

        public static uint GetRandomUID()
        {
            return (uint)random.Next();
        }

        public static bool Destroy(EngineObject obj, double time = 0)
        {
            /*
             * TODO: Implement Destroy method
             * This method should destroy the object after a certain amount of time
             * and completely remove it from the scene at the end of the frame
             *
             * If the Object is a GameObject, it should also remove all its children and components
             *             
             */

            return false;
        }

        public static bool DestroyImmediate(EngineObject obj)
        {
            /*
             * TODO: Implement DestroyImmediate method
             * This method should destroy the object immediately, without waiting for the end of the frame
             */

            return false;
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

            // add key to down keys
            if (keyEvent.repeat == 0)
            {
                Input.SetKeyDown((uint)keyEvent.keysym.sym);
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
                        //Input.SetMouseButtonPressed(sdlEvent.button.button - 1);
                        break;

                    case SDL.SDL_EventType.SDL_MOUSEBUTTONUP:
                        Input.SetMouseButtonReleased(sdlEvent.button.button - 1);
                        break;


                    // Handle keyboard events
                    case SDL.SDL_EventType.SDL_KEYDOWN:
                        this.HandleKeyboardEvent(sdlEvent.key);
                        break;

                    // release key
                    case SDL.SDL_EventType.SDL_KEYUP:
                        Input.SetKeyReleased((uint)sdlEvent.key.keysym.sym);
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
            // Calls Script.Update() for all active scripts
            scene.Update();

            // Update Physics
            List<GameObject> objects_with_collider = new List<GameObject>();
            // TODO: handle children with colliders and make sure children don't collide with parents
            foreach (GameObject child in scene.GetGameObjects())
            {
                if (child.GetComponent<Collider>() != null)
                {
                    objects_with_collider.Add(child);
                }
            }
            Physics.UpdatePhysics(objects_with_collider);
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
                "Update Duration: " + (1000*Time.updateDuration).ToString("0.00") + " ms",
                "Draw Duration: " + (1000*Time.drawDuration).ToString("0.00") + " ms",
                "Total Duration: " + (1000*Time.totalDuration).ToString("0.00") + " ms",
                "Free Duration: " + (1000*Time.freeDuration).ToString("0.00") + " ms"
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

            scene.Draw();

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

            double ticksToSec = 1.0 / Stopwatch.Frequency;
            long tickStart = Stopwatch.GetTimestamp();
            long beforeUpdate = tickStart;
            double beforeUpdateSec = ticksToSec * beforeUpdate;
            Time.lastUpdateTime = ticksToSec * tickStart;
            Time.lastDrawTime = Time.lastUpdateTime;

            Time.time = 0;
            Time.tick = 0;
            while (running)
            {
                // Handle events on queue
                HandleEvents();

                beforeUpdate = Stopwatch.GetTimestamp();
                beforeUpdateSec = ticksToSec * beforeUpdate;
                Time.deltaTime = (beforeUpdateSec - Time.lastUpdateTime);
                Time.time += Time.deltaTime;
                Time.tick++;
                this.Update();
                Time.lastUpdateTime = ticksToSec * Stopwatch.GetTimestamp();
                Time.updateDuration = Time.lastUpdateTime - beforeUpdateSec;
                this.Draw();
                Time.lastDrawTime = ticksToSec * Stopwatch.GetTimestamp();
                Time.drawDuration = Time.lastDrawTime - Time.lastUpdateTime;

                // Cap the frame rate
                Time.totalDuration = Time.lastDrawTime - beforeUpdateSec;
                Time.freeDuration = 0;
                if (Time.totalDuration < 1.0 / targetFPS)
                {
                    Time.freeDuration = 1.0 / targetFPS - Time.totalDuration;
                    SDL.SDL_Delay((uint)(Time.freeDuration * 1000));
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
