using SDL2;
using System.Diagnostics;
using System.Reflection;
using System.Runtime.CompilerServices;
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

        public Vec2D GetTopLeft()
        {
            return new Vec2D(x, y);
        }

        public Vec2D GetBottomRight()
        {
            return new Vec2D(x + w, y + h);
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
        protected bool enabled = true;
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

        public bool Destroy(EngineObject obj, double time = 0)
        {
            // Destroy the object
            Scene? scene = obj.GetScene();

            if (scene == null)
            {
                // try to use the scene of this object
                scene = this.GetScene();
            }

            if (scene == null)
            {
                return false;
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

        private Scene? GetScene()
        {
            return scene;
        }
    }

    public interface IEngine
    {
        void Run();

        void SetScene(Scene scene);
    }

    public class Engine : IEngine
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

        public void SetScene(Scene scene)
        {
            this.scene = scene;
        }
    }
}
