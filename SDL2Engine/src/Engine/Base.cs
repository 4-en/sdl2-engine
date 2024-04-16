using SDL2;

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

        protected Component()
        {
            gameObject = new GameObject();
        }

        public void Init(GameObject gameObject)
        {
            this.gameObject = gameObject;
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
        // Position of the GameObject
        protected Vec2D Position { get; set; }
        protected Vec2D ParentPosition { get; set; }
        protected GameObject? Parent { get; set; }
        protected GameObject? Root { get; set; }
        private readonly List<GameObject> children = [];
        private readonly List<Component> scripts = [];

        public GameObject()
        {
            this.Position = new Vec2D();
            this.ParentPosition = new Vec2D();
            this.Parent = null;
            this.Root = null;
        }

        public GameObject(GameObject root)
        {
            this.Position = new Vec2D();
            this.ParentPosition = new Vec2D();
            this.Parent = null;
            this.Root = root;
        }

        public void UpdateChildPositions()
        {
            var myPosition = this.GetPosition();
            foreach (GameObject child in children)
            {
                child.SetParentPosition(myPosition);
            }
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

        public GameObject? GetRoot()
        {
            return Root;
        }

        protected void SetRoot(GameObject? root)
        {
            this.Root = root;
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
            child.SetRoot(this.Root);
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
                scripts.Add(newComponent);
                newComponent.Start();
            } else
            {
                Console.WriteLine("Failed to create component of type " + typeof(T).Name);
            }

            return newComponent;
        }

        public bool RemoveComponent(Component script)
        {
            return scripts.Remove(script);
        }

        public bool RemoveComponent<T>() where T : Component
        {
            foreach (Component script in scripts)
            {
                if (script is T)
                {
                    return scripts.Remove(script);
                }
            }

            return false;
        }

        // Gets first component of type T
        public T? GetComponent<T>() where T : Component
        {
            foreach (Component script in scripts)
            {
                if (script is T)
                {
                    return (T)script;
                }
            }

            return null;
        }

        public List<Component> GetScripts()
        {
            return scripts;
        }

        public void Start()
        {
            foreach (Component script in scripts)
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
            foreach (Component script in scripts)
            {
                script.Update();
            }

            foreach (GameObject child in children)
            {
                child.Update();
            }
        }

        // Override this method to draw custom graphics
        // By default, this method does nothing
        public void Draw(Camera camera)
        {
            // draw drawables
            foreach (Component script in scripts)
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


    public class Scene : GameObject
    {
        private Camera camera;
        public Scene()
        {
            this.Position = new Vec2D();
            this.ParentPosition = new Vec2D();
            this.Parent = null;
            this.Root = this;
            this.camera = new Camera2D(new Vec2D());
        }

        public Scene(Camera camera)
        {
            this.Position = new Vec2D();
            this.ParentPosition = new Vec2D();
            this.Parent = null;
            this.Root = this;
            this.camera = camera;
        }


        public Camera GetCamera()
        {
            return camera;
        }


    }

    public class Time
    {
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

    public class Input
    {
        public static UInt32[] DownKeys = new UInt32[16];
        public static UInt32[] PressedKeys = new UInt32[16];
        public static UInt32[] ReleasedKeys = new UInt32[16];

        // TODO: Implement static methods for getting inputs
        public static bool IsKeyDown(UInt32 key)
        {
            for (int i = 0; i < DownKeys.Length; i++)
            {
                if (DownKeys[i] == key)
                {
                    return true;
                }
            }

            return false;
        }

        public static bool IsKeyPressed(UInt32 key)
        {
            for (int i = 0; i < PressedKeys.Length; i++)
            {
                if (PressedKeys[i] == key)
                {
                    return true;
                }
            }

            return false;
        }

        public static bool IsKeyReleased(UInt32 key)
        {
            for (int i = 0; i < ReleasedKeys.Length; i++)
            {
                if (ReleasedKeys[i] == key)
                {
                    return true;
                }
            }

            return false;
        }

        public static void SetKeyDown(UInt32 key)
        {
            for (int i = 0; i < DownKeys.Length; i++)
            {
                if (DownKeys[i] == 0)
                {
                    DownKeys[i] = (UInt32)key;
                    break;
                }
            }
        }

        public static void SetKeyPressed(UInt32 key)
        {
            for (int i = 0; i < PressedKeys.Length; i++)
            {
                if (PressedKeys[i] == 0)
                {
                    PressedKeys[i] = (UInt32)key;
                    break;
                }
            }
        }

        public static void SetKeyReleased(UInt32 key)
        {
            for (int i = 0; i < ReleasedKeys.Length; i++)
            {
                if (ReleasedKeys[i] == 0)
                {
                    ReleasedKeys[i] = (UInt32)key;
                    break;
                }
            }
        }

        public static void ClearKeys()
        {
            for (int i = 0; i < DownKeys.Length; i++)
            {
                DownKeys[i] = 0;
            }

            for (int i = 0; i < PressedKeys.Length; i++)
            {
                PressedKeys[i] = 0;
            }

            for (int i = 0; i < ReleasedKeys.Length; i++)
            {
                ReleasedKeys[i] = 0;
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
            Input.ClearKeys();

            while (SDL.SDL_PollEvent(out sdlEvent) != 0)
            {
                switch (sdlEvent.type)
                {
                    case SDL.SDL_EventType.SDL_QUIT:
                        running = false;
                        break;

                    case SDL.SDL_EventType.SDL_MOUSEMOTION:
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
            uint beforeUpdate = SDL.SDL_GetTicks();
            while (running)
            {
                // Handle events on queue
                HandleEvents();

                beforeUpdate = SDL.SDL_GetTicks();
                Time.deltaTime = (beforeUpdate - Time.lastUpdateTime) / 1000.0;
                Time.time += Time.deltaTime;
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
