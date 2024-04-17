using SDL2;
using System.Reflection;

namespace SDL2Engine
{

    public struct Vec2D
    {
        public double x;
        public double y;

        public Vec2D(double x = 0, double y = 0)
        {
            this.x = x;
            this.y = y;
        }

        public static Vec2D operator +(Vec2D a, Vec2D b)
        {
            return new Vec2D(a.x + b.x, a.y + b.y);
        }

        public static Vec2D operator -(Vec2D a, Vec2D b)
        {
            return new Vec2D(a.x - b.x, a.y - b.y);
        }

        public static Vec2D operator *(Vec2D a, double b)
        {
            return new Vec2D(a.x * b, a.y * b);
        }

        public static Vec2D operator /(Vec2D a, double b)
        {
            return new Vec2D(a.x / b, a.y / b);
        }

        public static Vec2D operator *(double a, Vec2D b)
        {
            return new Vec2D(a * b.x, a * b.y);
        }

        public static Vec2D operator /(double a, Vec2D b)
        {
            return new Vec2D(a / b.x, a / b.y);
        }

        public static Vec2D operator *(Vec2D a, Vec2D b)
        {
            return new Vec2D(a.x * b.x, a.y * b.y);
        }

        public static Vec2D operator /(Vec2D a, Vec2D b)
        {
            return new Vec2D(a.x / b.x, a.y / b.y);
        }

        public static Vec2D operator -(Vec2D a)
        {
            return new Vec2D(-a.x, -a.y);
        }

        public static Vec2D operator +(Vec2D a)
        {
            return new Vec2D(+a.x, +a.y);
        }

        public static bool operator ==(Vec2D a, Vec2D b)
        {
            return a.x == b.x && a.y == b.y;
        }

        public override readonly bool Equals(object? obj)
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

        public double Length()
        {
            return Math.Sqrt(x * x + y * y);
        }

        public double LengthSquared()
        {
            return x * x + y * y;
        }

        public static double Distance(Vec2D a, Vec2D b)
        {
            return (a - b).Length();
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

        public override readonly bool Equals(object? obj)
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
        protected GameObject gameObject;
        private bool active = false;

        protected Component()
        {
            gameObject = new GameObject();
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

        // Override this method to provide custom functionality
        public virtual void Start()
        {
            // Do nothing
        }

        // Override this method to provide custom functionality
        public virtual void Update()
        {
            // Do nothing
        }

    }

    /*
     * Base class for all game objects
     * 
     * This class is meant to be inherited from
     * and can be overridden to provide custom
     * functionality
     */
    public class GameObject
    {
        public uint layer = 0;
        private bool active = true;
        // Position of the GameObject
        protected Vec2D Position { get; set; }
        protected Vec2D ParentPosition { get; set; }
        protected GameObject? Parent { get; set; }
        protected Scene? scene;
        private readonly List<GameObject> children = [];
        private readonly List<Component> components = [];

        public GameObject()
        {
            this.Position = new Vec2D();
            this.ParentPosition = new Vec2D();
            this.Parent = null;
            this.scene = null;
        }

        public GameObject(Scene scene)
        {
            this.Position = new Vec2D();
            this.ParentPosition = new Vec2D();
            this.Parent = null;
            this.scene = scene;
        }

        public void UpdateChildPositions()
        {
            var myPosition = this.GetPosition();
            foreach (GameObject child in children)
            {
                child.SetParentPosition(myPosition);
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
            return this.ParentPosition + this.Position;
        }

        public void SetPosition(Vec2D position)
        {
            this.Position = position;
            this.UpdateChildPositions();

        }

        public void SetParentPosition(Vec2D position)
        {
            this.ParentPosition = position;
            this.UpdateChildPositions();
        }



        public void AddChild(GameObject child)
        {
            child.SetParentPosition(this.GetPosition());
            child.SetParent(this);
            child.SetScene(this.scene);
            children.Add(child);

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
                components.Add(newComponent);
                newComponent.Start();
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
                script.Start();
            }

            foreach (GameObject child in children)
            {
                child.Start();
            }
        }

        public void Update()
        {
            // Update all scripts
            for(int i = 0; i < components.Count; i++)
            {
                components[i].Update();
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


    public class Time
    {
        public static ulong tick = 0;
        public static double time = 0;
        public static double deltaTime = 0;

        public static double lastDrawTime = 0;
        public static double lastUpdateTime = 0;

        public static double updateDuration = 0;
        public static double drawDuration = 0;
        public static double totalDuration = 0;
        public static double freeDuration = 0;

        public static double GetFPS()
        {
            return 1.0 / deltaTime;
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
