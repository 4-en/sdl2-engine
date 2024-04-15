using SDL2;
using static SDL2.SDL;
using System;

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

        public double LengthSquared()
        {
            return x * x + y * y + z * z;
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

    public interface ICamera
    {
        public Vec2D GetScreenPosition(Vec2D worldPosition);
    }

    public class Camera2D : ICamera
    {
        private Vec2D Position { get; set; }
        private Vec2D Size { get; set; }

        private Vec2D ScreenSize { get; set; }

        public Camera2D(Vec2D position = new Vec2D())
        {
            this.Position = position;
            this.Size = new Vec2D(1920, 1080);
            this.ScreenSize = new Vec2D(1920, 1080);
        }

        public Vec2D GetScreenPosition(Vec2D worldPosition)
        {
            return new Vec2D((worldPosition.x - Position.x) * ScreenSize.x / Size.x, (worldPosition.y - Position.y) * ScreenSize.y / Size.y);
        }

    }

    // Base class for all scripts
    // Add to a GameObject to provide custom functionality
    public class Component
    {
        protected GameObject gameObject;

        public Component(GameObject gameObject)
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

        public void AddComponent(Component script)
        {
            scripts.Add(script);
        }

        public void RemoveComponent(Component script)
        {
            scripts.Remove(script);
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
        virtual public void Draw(ICamera camera)
        {
            // Do nothing
        }

        public void DrawAll(ICamera camera)
        {
            Draw(camera);
            foreach (GameObject child in children)
            {
                child.DrawAll(camera);
            }
        }

    }


    public class World : GameObject
    {
        private ICamera camera;
        public World()
        {
            this.Position = new Vec2D();
            this.ParentPosition = new Vec2D();
            this.Parent = null;
            this.Root = null;
            this.camera = new Camera2D(new Vec2D());
        }

        public World(ICamera camera)
        {
            this.Position = new Vec2D();
            this.ParentPosition = new Vec2D();
            this.Parent = null;
            this.Root = null;
            this.camera = camera;
        }


        public override void Draw(ICamera camera)
        {
            // Draw all children
            foreach (GameObject child in GetChildren())
            {
                child.Draw(camera);
            }
        }

        public ICamera GetCamera()
        {
            return camera;
        }
    }

    public class Time
    {
        public static double time = 0;
        public static double deltaTime = 0;
        public static double fixedDeltaTime = 1.0 / 60.0; // 60 FPS

        public static double lastDrawTime = 0;
        public static double lastUpdateTime = 0;

        public static double GetFPS()
        {
            return 1.0 / deltaTime;
        }
    }

    public class Input
    {
        // TODO: Implement static methods for getting inputs
        public static bool GetKeyDown(int key)
        {
            return false;
        }
    }


    public class Engine
    {

        private World world;
        private bool running = false;
        public bool showDebug = false;

        // SDL variables
        private IntPtr window;
        public static IntPtr renderer;
        private SDL.SDL_Event sdlEvent;

        public Engine(World world)
        {
            this.world = world;
        }

        private void Init()
        {
            // Initialize SDL2
            if (SDL.SDL_Init(SDL.SDL_INIT_VIDEO) < 0)
            {
                Console.WriteLine("SDL could not initialize! SDL Error: " + SDL.SDL_GetError());
                return;
            }

            // Create window
            window = SDL.SDL_CreateWindow("SDL2 Engine Test",
                SDL.SDL_WINDOWPOS_UNDEFINED, SDL.SDL_WINDOWPOS_UNDEFINED,
                800, 600, SDL.SDL_WindowFlags.SDL_WINDOW_SHOWN | SDL.SDL_WindowFlags.SDL_WINDOW_RESIZABLE);
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
            switch (keyEvent.keysym.sym)
            {
                case SDL.SDL_Keycode.SDLK_ESCAPE:
                    running = false;
                    break;
                case SDL.SDL_Keycode.SDLK_F3:
                    showDebug = !showDebug;
                    break;
                default:
                    break;
            }
        }

        private void HandleEvents()
        {

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

                    default:
                        break;
                }
            }
        }

        private void Update()
        {
            world.Update();
        }

        private void DrawDebug()
        {
            // Draw debug information
            SDL.SDL_SetRenderDrawColor(renderer, 0xFF, 0x00, 0x00, 0xFF); // Red
            SDL.SDL_Rect rect = new SDL.SDL_Rect();
            rect.x = 10;
            rect.y = 10;
            rect.w = 100;
            rect.h = 100;
            SDL.SDL_RenderFillRect(renderer, ref rect);

            double fps = Time.GetFPS();
            string fpsText = "FPS: " + fps.ToString("0.00");
            IntPtr font = SDL_ttf.TTF_OpenFont("arial.ttf", 24);
            SDL.SDL_Color color = new SDL.SDL_Color();
            color.r = 0xFF;
            color.g = 0xFF;
            color.b = 0xFF;
            color.a = 0xFF;
            IntPtr surface = SDL_ttf.TTF_RenderText_Solid(font, fpsText, color);
            IntPtr texture = SDL.SDL_CreateTextureFromSurface(renderer, surface);
            SDL.SDL_FreeSurface(surface);
            SDL.SDL_Rect textRect = new SDL.SDL_Rect();
            textRect.x = 10;
            textRect.y = 10;
            SDL.SDL_QueryTexture(texture, out _, out _, out textRect.w, out textRect.h);
            SDL.SDL_RenderCopy(renderer, texture, IntPtr.Zero, ref textRect);
            SDL.SDL_DestroyTexture(texture);
            SDL_ttf.TTF_CloseFont(font);


        }

        private void Draw()
        {
            // Clear screen
            SDL.SDL_SetRenderDrawColor(renderer, 0x1F, 0x1F, 0x1F, 0xFF); // Dark gray
            SDL.SDL_RenderClear(renderer);

            world.DrawAll(world.GetCamera());

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


            Time.time = 0;
            while (running)
            {
                // Handle events on queue
                HandleEvents();

                this.Update();
                this.Draw();
                Time.time += Time.deltaTime;

                // Cap the frame rate
                double currentTime = SDL.SDL_GetTicks();
                double elapsedTime = currentTime - Time.lastDrawTime;
                if (elapsedTime < 1000.0 / 60.0)
                {
                    SDL.SDL_Delay((uint)(1000.0 / 60.0 - elapsedTime));
                }
                Time.deltaTime = (SDL.SDL_GetTicks() - Time.lastUpdateTime) / 1000.0;
                Time.lastUpdateTime = SDL.SDL_GetTicks();
                Time.lastDrawTime = SDL.SDL_GetTicks();

            }

            // Destroy window
            SDL.SDL_DestroyRenderer(renderer);
            SDL.SDL_DestroyWindow(window);

            // Quit SDL subsystems
            SDL.SDL_Quit();

        }
    }
}
