using Newtonsoft.Json;
using SDL2;
using System.Diagnostics;
using System.Reflection;
using System.Runtime.CompilerServices;
using static System.Diagnostics.Stopwatch;

namespace SDL2Engine
{
    // ILoadable interface
    // This interface is used to load and unload resources
    interface ILoadable : IDisposable
    {
        public void Load();
        public bool IsLoaded();
    }

    [Serializable]
    public struct Vec2D
    {
        public static readonly Vec2D Zero = new(0, 0);
        public static readonly Vec2D One = new(1, 1);

        public double x = 0;
        public double y = 0;
        public double z = 0; // Vec2D has a z component for layering

        public Vec2D(double x = 0, double y = 0, double z = 0)
        {
            this.x = x;
            this.y = y;
            this.z = z;
        }

        public Vec2D(double angle)
        {
            x = Math.Cos(angle);
            y = Math.Sin(angle);
            z = 0;
        }

        // to string
        public override string ToString()
        {
            return "(" + x + ", " + y + ")";
        }

        public double AngleTo(Vec2D other)
        {
            return Math.Atan2(other.y - y, other.x - x) * 180 / Math.PI;
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

        // Rotate the vector by a given angle in degrees
        public Vec2D Rotate(double rotation)
        {
            double rad = rotation * Math.PI / 180;
            double cos = Math.Cos(rad);
            double sin = Math.Sin(rad);
            return new Vec2D(x * cos - y * sin, x * sin + y * cos);
        }

        public Vec2D RotateRadians(double rotation)
        {
            double cos = Math.Cos(rotation);
            double sin = Math.Sin(rotation);
            return new Vec2D(x * cos - y * sin, x * sin + y * cos);
        }

        // Get the rotation of the vector in degrees
        public double GetRotation()
        {
            return Math.Atan2(y, x) * 180 / Math.PI;
        }

        public double GetRotationRadians()
        {
            return Math.Atan2(y, x);
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

    [Serializable]
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

    /* Rect struct
     * SDL.SDL_Rect uses int for x, y, w, h
     * We use this struct to store double values
     */
    [Serializable]
    public struct Rect
    {
        public double x;
        public double y;
        public double w;
        public double h;

        public Rect(double w, double h)
        {
            this.x = 0;
            this.y = 0;
            this.h = h;
            this.w = w;
        }

        public Rect(double x, double y, double w, double h)
        {
            this.x = x;
            this.y = y;
            this.w = w;
            this.h = h;
        }

        public Vec2D GetTopLeft()
        {
            return new Vec2D(x, y);
        }

        public Vec2D GetBottomRight()
        {
            return new Vec2D(x + w, y + h);
        }

        public override string ToString()
        {
            return "Rect: x=" + x + ", y=" + y + ", w=" + w + ", h=" + h;
        }

        public Vec2D Center()
        {
            return new Vec2D(x + w / 2, y + h / 2);
        }

        public SDL.SDL_Rect ToSDLRect()
        {
            SDL.SDL_Rect sdlRect = new SDL.SDL_Rect();
            sdlRect.x = (int)Math.Floor(x);
            sdlRect.y = (int)Math.Floor(y);
            sdlRect.w = (int)Math.Ceiling(w);
            sdlRect.h = (int)Math.Ceiling(h);
            return sdlRect;
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

    [Serializable]
    public class EngineObject : IDisposable
    {
        protected static Random random = new Random(DateTime.Now.Millisecond);
        private static uint createdObjects = 0;
        private static uint GetOrder()
        {
            return createdObjects++;
        }

        public static Random GetRandom()
        {
            return random;
        }

        [JsonIgnore]
        private uint creationOrder = GetOrder();

        [JsonProperty]
        protected string name = "unnamed";
        [JsonProperty]
        protected bool enabled = true;
        [JsonIgnore]
        protected Scene? scene = null;
        [JsonIgnore]
        protected Scene? activeScene = null;
        [JsonProperty]
        protected uint uid = GetRandomUID();
        protected bool _to_be_destroyed = false;
        protected bool _disposed = false;

        public EngineObject(string name = "unnamed")
        {
            this.name = name;
        }

        ~EngineObject()
        {
            if (!_disposed)
            {
                Dispose();
            }
        }

        /* Override this method to dispose resources
         *         * This method is called when the object is destroyed
         */
        public virtual void Dispose()
        {
            _disposed = true;
            GC.SuppressFinalize(this);
            //Console.WriteLine("Disposing object: " + name);
        }


        public void MarkToBeDestroyed()
        {
            _to_be_destroyed = true;
        }

        public bool ToBeDestroyed()
        {
            return _to_be_destroyed;
        }

        public static uint GetRandomUID()
        {
            return (uint)random.Next();
        }

        public uint GetUID()
        {
            return uid;
        }

        public uint GetCreationOrder()
        {
            return creationOrder;
        }

        /* This basically removes the object from the scene
         * The object will be destroyed in the next frame
         * 
         * This only destroys the object if its not referenced anywhere else in user code
         */
        public bool Destroy(EngineObject obj, double time = 0)
        {
            // Destroy the object
            Scene? scene = obj.GetScene();

            if (scene == null)
            {
                // try to use the scene of this object
                //scene = this.GetScene();

                // the scene shouldn't be null, so send an error message
                // Console.WriteLine("Scene is null when trying to destroy object: " + obj.name + " in GameObject.Destroy()");
                throw new Exception("Scene is null when trying to destroy object: " + obj.name + " in GameObject.Destroy()");

            }




            scene.Destroy(obj, time);

            return true;
        }

        public bool Destroy(double time = 0)
        {
            // Destroy the object
            Scene? scene = this.GetScene();

            if (scene == null)
            {
                return false;
            }

            scene.Destroy(this, time);
            return true;
        }

        public Scene? GetScene()
        {
            return scene;
        }

        public bool SetScene(Scene? scene)
        {

            if (this.scene != null && scene != this.scene)
            {
                // If this causes issues, implement something to properly remove/switch scenes
                throw new Exception("Scene already set for object: " + name);
            }

            this.scene = scene;
            return true;
        }

        public Scene? GetActiveScene()
        {
            return activeScene;
        }

        public bool SetActiveScene(Scene? scene)
        {
            if(this.scene != null && scene != this.scene)
            {
                // If this causes issues, implement something to properly remove/switch scenes
                throw new Exception("Scene already set for object: " + name);
            }
            this.activeScene = scene;
            return true;
        }

        // this should be called by the scene
        // if not, we could get situations where the object is not properly removed from the scene
        // and then added to another scene
        internal void _clear_scene_dangerously()
        {
            scene = null;
        }

        internal void _clear_active_scene_dangerously()
        {
            activeScene = null;
        }
    }

    public interface IEngine
    {
        void Run();

    }

    public class Engine : IEngine
    {
        private static Engine? instance = null;
        private bool running = false;
        private bool showDebug = false;
        private bool fullscreen = false;
        private bool showColliders = false;
        private static bool forceAspectRatio = false;
        private static double aspectRatio = 16.0 / 9.0;
        public static UInt32 targetFPS = 500;
        public static int windowWidth = 1000;
        public static int windowHeight = (int)(windowWidth / aspectRatio);
        private Font? font = null;
        public static string gameName = "Unknown Game";


        // SDL variables
        private IntPtr window;
        public static IntPtr renderer;
        private SDL.SDL_Event sdlEvent;

        /*
         * Starts the engine with an initial scene
         * this scene will run in the SceneManager and can be used to bootstrap the game, including other scenes
         */
        public Engine(Scene? scene = null, string gameName = "Unknown Game")
        {
            if (instance != null)
            {
                throw new Exception("Engine instance already exists");
            }

            Engine.gameName = gameName;

            instance = this;

            // Set the scene in the SceneManger
            if (scene != null)
            {
                SceneManager.AddScene(scene);
            }
        }

        public Engine(string gameName)
        {
            if (instance != null)
            {
                throw new Exception("Engine instance already exists");
            }

            Engine.gameName = gameName;
            instance = this;
        }

        private bool wasInit = false;
        public void Init()
        {
            if (wasInit)
            {
                return;
            }
            wasInit = true;

            // Initialize SDL2
            if (SDL.SDL_Init(SDL.SDL_INIT_VIDEO) < 0)
            {
                Console.WriteLine("SDL could not initialize! SDL Error: " + SDL.SDL_GetError());
                return;
            }
            else
            {
                Console.WriteLine("SDL initialized");
            }

            // Initialize SDL_image
            if (SDL_image.IMG_Init(SDL_image.IMG_InitFlags.IMG_INIT_PNG | SDL_image.IMG_InitFlags.IMG_INIT_JPG) == 0)
            {
                Console.WriteLine("SDL_image could not initialize! SDL_image Error: " + SDL.SDL_GetError());
                return;
            }
            else
            {
                Console.WriteLine("SDL_image initialized");
            }

            // Initialize SDL_ttf
            if (SDL_ttf.TTF_Init() < 0)
            {
                Console.WriteLine("SDL_ttf could not initialize! SDL_ttf Error: " + SDL.SDL_GetError());
                return;
            }
            else
            {
                Console.WriteLine("SDL_ttf initialized");
            }

            // Initualize SDL_mixer
            if (SDL_mixer.Mix_OpenAudio(44100, SDL_mixer.MIX_DEFAULT_FORMAT, 2, 2048) < 0)
            {
                Console.WriteLine("SDL_mixer could not initialize! SDL_mixer Error: " + SDL.SDL_GetError());
                return;
            }
            else
            {
                Console.WriteLine("SDL_mixer initialized");
            }

            // Create window
            window = SDL.SDL_CreateWindow(Engine.gameName,
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

            // alpha blending
            SDL.SDL_SetRenderDrawBlendMode(renderer, SDL.SDL_BlendMode.SDL_BLENDMODE_BLEND);

            // Initialize renderer color
            SDL.SDL_SetRenderDrawColor(renderer, 0xFF, 0xFF, 0xFF, 0xFF);

            Console.WriteLine("Engine initialized");
            Console.WriteLine();
            Console.WriteLine("Press F3 to toggle debug info");
            Console.WriteLine("Press F4 to toggle collider visibility");
            Console.WriteLine("Press F11 to toggle fullscreen");
            Console.WriteLine();


        }

        private void Cleanup()
        {
            if (font != null)
            {
                font.Dispose();
            }



            // Destroy renderer
            SDL.SDL_DestroyRenderer(renderer);
            renderer = IntPtr.Zero;

            // Destroy window
            SDL.SDL_DestroyWindow(window);
            window = IntPtr.Zero;

            // Quit SDL subsystems
            SDL_mixer.Mix_Quit();
            SDL_ttf.TTF_Quit();
            SDL_image.IMG_Quit();
            SDL.SDL_Quit();

            Console.WriteLine("Engine cleaned up");
        }

        private void HandleKeyboardEvent(SDL.SDL_KeyboardEvent keyEvent)
        {

            // add key to down keys
            if (keyEvent.repeat == 0)
            {
                Input.SetKeyDown((int)keyEvent.keysym.sym);
            }

            switch (keyEvent.keysym.sym)
            {
                case SDL.SDL_Keycode.SDLK_ESCAPE:
                    // running = false;
                    break;
                case SDL.SDL_Keycode.SDLK_F3:
                    showDebug = !showDebug;
                    break;
                case SDL.SDL_Keycode.SDLK_F4:
                    showColliders = !showColliders;
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
                        Input.SetKeyReleased((int)sdlEvent.key.keysym.sym);
                        break;

                    // window resize event
                    case SDL.SDL_EventType.SDL_WINDOWEVENT:
                        if (sdlEvent.window.windowEvent == SDL.SDL_WindowEventID.SDL_WINDOWEVENT_RESIZED)
                        {
                            if (forceAspectRatio)
                            {
                                double newWidth = sdlEvent.window.data1;
                                double newHeight = sdlEvent.window.data2;

                                if (newWidth / newHeight > aspectRatio)
                                {
                                    windowWidth = (int)(newHeight * aspectRatio);
                                    windowHeight = (int)newHeight;
                                }
                                else
                                {
                                    windowWidth = (int)newWidth;
                                    windowHeight = (int)(newWidth / aspectRatio);
                                }

                                SDL.SDL_SetWindowSize(window, windowWidth, windowHeight);
                            }

                            windowWidth = sdlEvent.window.data1;
                            windowHeight = sdlEvent.window.data2;
                        }
                        break;
                    default:
                        break;

                }
            }
        }

        public static void Stop()
        {
            if (instance != null)
            {
                instance.running = false;
            }
        }

        private void Update()
        {
            // Updates all scenes in the following order:
            /*
             * for each scene in scenes
             *  1. Add new game objects
             *  2. Update Physics
             *  3. Call Scripts: (Start, Update, LateUpdate)
             *  4. Remove game objects
             */
            SceneManager.UpdateScenes();

        }

        private void DrawDebug()
        {
            SDL.SDL_Color textColor = new SDL.SDL_Color { r = 255, g = 255, b = 255, a = 255 };
            SDL.SDL_Color backgroundColor = new SDL.SDL_Color { r = 0x1A, g = 0x1A, b = 0x1A, a = 0xFF }; // Black color for background
            if (font == null) {
                font =  AssetManager.LoadFont("Assets/Fonts/Roboto-Regular.ttf", 24);
                font.Load();
            }
            

            if (!font.IsLoaded())
            {
                Console.WriteLine("Failed to load font! SDL_ttf");
                return;
            }

            IntPtr fontPtr = font.Get();

            int totalScenes = SceneManager.GetScenes().Count;
            int totalRootObjects = 0;
            int totalObjects = 0;
            int totalChunkedObjects = 0;
            int totalDrawables = 0;
            int totalVisibleDrawables = 0;
            int totalScripts = 0;
            int totalColliders = 0;
            int totalCoros = 0;
            
            foreach (Scene scene in SceneManager.GetScenes())
            {
                totalRootObjects += scene.GetGameObjects().Count;
                totalObjects += scene.GetGameObjectsCount();
                totalDrawables += scene.GetDrawableCount();
                totalScripts += scene.GetScriptCount();
                totalColliders += scene.GetColliderCount();
                totalCoros += scene.GetCoroutineManager().Count();
                totalVisibleDrawables += scene.GetLastVisibleDrawables();

                if(scene is ChunkedScene cScene)
                {
                    totalChunkedObjects += cScene.GetChunkCount();
                }
            }

            totalChunkedObjects += totalObjects;

            string[] debugStrings =
                {
                    "Press F3 to toggle debug info",
                    "FPS: " + Time.GetFPS().ToString(),
                    "Update Duration: " + (1000*Time.updateDuration).ToString("0.00") + " ms",
                    "Draw Duration: " + (1000*Time.drawDuration).ToString("0.00") + " ms",
                    "Total Duration: " + (1000*Time.totalDuration).ToString("0.00") + " ms",
                    "Free Duration: " + (1000*Time.freeDuration).ToString("0.00") + " ms",
                    "Total Objects: " + totalRootObjects.ToString() + "/" + totalObjects.ToString() + "/" + totalChunkedObjects.ToString(),
                    "Total Drawables: " + totalVisibleDrawables.ToString() + "/" + totalDrawables.ToString(),
                    "Total Scripts: " + totalScripts.ToString(),
                    "Total Colliders: " + totalColliders.ToString(),
                    "Total Coroutines: " + totalCoros.ToString(),
                    "Total Scenes: " + totalScenes.ToString()
                };

            for (int i = 0; i < debugStrings.Length; i++)
            {
                int texW = 0, texH = 0;
                if (SDL_ttf.TTF_SizeText(fontPtr, debugStrings[i], out texW, out texH) != 0)
                {
                    Console.WriteLine("Failed to size text! SDL_ttf Error: " + SDL.SDL_GetError());
                }

                // Background Rectangle
                SDL.SDL_Rect backgroundRect = new SDL.SDL_Rect { x = 5, y = 5 + i * 30, w = texW + 10, h = texH + 5 }; // Slightly larger to create padding
                SDL.SDL_SetRenderDrawColor(renderer, backgroundColor.r, backgroundColor.g, backgroundColor.b, backgroundColor.a);
                SDL.SDL_RenderFillRect(renderer, ref backgroundRect);

                // Render Text
                IntPtr surface = SDL_ttf.TTF_RenderUTF8_Blended(fontPtr, debugStrings[i], textColor);
                IntPtr texture = SDL.SDL_CreateTextureFromSurface(renderer, surface);
                SDL.SDL_Rect dst = new SDL.SDL_Rect { x = 10, y = 10 + i * 30, w = texW, h = texH };
                SDL.SDL_RenderCopy(renderer, texture, IntPtr.Zero, ref dst);

                SDL.SDL_DestroyTexture(texture);
                SDL.SDL_FreeSurface(surface);
            }
        }

        private void DrawColliders()
        {

            // set color to red
            SDL.SDL_SetRenderDrawColor(renderer, 0xFF, 0x00, 0x00, 0xFF);

            foreach (Scene scene in SceneManager.GetScenes())
            {
                var camera = scene.GetCamera();
                if (camera == null)
                {
                    continue;
                }
                foreach (Collider collider in scene.GetColliders())
                {
                    // TODO: test this
                    // this is a bit ugly, but since its just the debug mode, we keep it for now
                    if (collider is BoxCollider boxCollider)
                    {
                        var rect = boxCollider.GetCollisionBox();
                        // to screen
                        SDL.SDL_Rect sdlRect = camera.RectToScreen(rect).ToSDLRect();
                        SDL.SDL_RenderDrawRect(renderer, ref sdlRect);
                    }
                    else if (collider is CircleCollider circleCollider)
                    {
                        var center = circleCollider.GetCollisionCenter();
                        double radius = circleCollider.GetRadius();

                        // to screen
                        Vec2D screenCenter = camera.WorldToScreen(center);
                        double screenRadius = camera.WorldToScreen(radius);

                        // draw the circle
                        // do this manually for now
                        // is there no SDL function for this?
                        // other solution would be to create a texture, but since this is just for debugging, we keep it simple
                        int resolution = 12;
                        double angle = 0;
                        double angleStep = 2 * Math.PI / resolution;
                        Vec2D lastPoint = new Vec2D(screenCenter.x + screenRadius, screenCenter.y);
                        for (int i = 0; i < resolution; i++)
                        {
                            angle += angleStep;
                            Vec2D point = new Vec2D(screenCenter.x + screenRadius * Math.Cos(angle), screenCenter.y + screenRadius * Math.Sin(angle));
                            SDL.SDL_RenderDrawLine(renderer, (int)lastPoint.x, (int)lastPoint.y, (int)point.x, (int)point.y);
                            lastPoint = point;
                        }

                    }
                }
            }

        }

        private void Draw()
        {
            // Clear screen
            SDL.SDL_SetRenderDrawColor(renderer, 0x1F, 0x1F, 0x1F, 0xFF); // Dark gray
            SDL.SDL_RenderClear(renderer);

            

            if (showColliders)
            {
                DrawColliders();
            } else
            {
                SceneManager.DrawScenes();
            }

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
            double temp_double = 0;
            Time.lastUpdateTime = ticksToSec * tickStart;
            Time.lastDrawTime = Time.lastUpdateTime;

            Time.time = 0;
            Time.tick = 0;
            while (running)
            {
                // Handle events on queue
                HandleEvents();

                // Time start of update
                beforeUpdate = Stopwatch.GetTimestamp();
                temp_double = ticksToSec * beforeUpdate;
                Time.deltaTime = temp_double - beforeUpdateSec;
                beforeUpdateSec = temp_double;

                Time.AddFrameTime(Time.deltaTime);
                Time.time += Time.deltaTime;
                Time.tick++;

                // Update the game
                // (Physics, Scripts, etc. )
                this.Update();
                Time.lastUpdateTime = ticksToSec * Stopwatch.GetTimestamp();
                Time.updateDuration = Time.lastUpdateTime - beforeUpdateSec;

                // Draw the game
                this.Draw();
                Time.lastDrawTime = ticksToSec * Stopwatch.GetTimestamp();
                Time.drawDuration = Time.lastDrawTime - Time.lastUpdateTime;

                // Cap the frame rate
                Time.totalDuration = Time.lastDrawTime - beforeUpdateSec;
                Time.freeDuration = 0;
                if (Time.totalDuration < 1.0 / targetFPS)
                {
                    Time.freeDuration = 1.0 / targetFPS - Time.totalDuration;
                    // waste cpu cycles until the next frame
                    double endTime = beforeUpdateSec + 1.0 / targetFPS;
                    while (temp_double < endTime)
                    {
                        int xd;
                        for(int i = 0; i < 1000; i++)
                        {
                            // waste some time
                            xd = i * i;
                            xd = i + xd;
                            xd = xd * ( (123 + xd) / 3);

                        }
                        temp_double = ticksToSec * Stopwatch.GetTimestamp();
                    }

                }

            }

            // Cleanup SDL2
            Cleanup();

        }
    }
}
