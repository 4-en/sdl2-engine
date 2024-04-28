using System.Security.Cryptography.X509Certificates;
using SDL2;
using static SDL2.SDL;

using SDL2Engine;

namespace SDL2Engine.Testing
{

    class DestroyNotifier : Script
    {
        public override void Start()
        {
            Console.WriteLine("DestroyNotifier Start");
        }

        public override void OnDestroy()
        {
            Console.WriteLine("DestroyNotifier Destroy");
        }
    }

    class MouseTracker : Script
    {
        private GameObject? AddSquare(GameObject? parent = null)
        {
            var square = new GameObject("Child Square");
            if (parent == null)
            {
                parent = this.gameObject.GetParent();
            }

            if (parent != null)
            {
                parent.AddChild(square);
            }
            square.AddComponent<RotatingSquare>();
            square.AddComponent<DestroyNotifier>();
            square.SetPosition(this.gameObject.GetPosition());


            return square;
            
        }

        public override void Start()
        {
            Console.WriteLine("MouseTracker Start");
        }
        public override void Update()
        {
            var mousePosition = Input.GetMousePosition();
            
            var gameObject = this.gameObject;
            var camera = Camera.GetCamera(gameObject);
            if (camera != null)
            {
                mousePosition = camera.ScreenToWorld(mousePosition);
            }

            gameObject.SetPosition(mousePosition);

            if (Input.GetKeyDown((uint)SDL.SDL_Keycode.SDLK_x))
            {
                // remove a random root game object
                var scene = gameObject.GetScene();
                if (scene != null)
                {
                    var rootGameObjects = scene.GetGameObjects();
                    if (rootGameObjects.Count > 0)
                    {
                        var rand = new Random();
                        var index = rand.Next(0, rootGameObjects.Count);
                        var rootGameObject = rootGameObjects[index];
                        Destroy(rootGameObject);
                    }
                }
            }

            if (Input.GetMouseButtonDown(0))
            {
                AddSquare();
            } else if (Input.GetMouseButtonDown(2))
            {
                var square = AddSquare(gameObject);
                if (square != null)
                {
                    var rand = new Random();
                    square.SetLocalPosition(new Vec2D(rand.Next(-100, 100), rand.Next(-100, 100)));
                }
            } else if (Input.GetMouseButtonDown(1))
            {
                // add 1000 squares
                for (int i = 0; i < 1000; i++)
                {
                    var square = AddSquare(gameObject);
                    if (square != null)
                    {
                        var rand = new Random();
                        square.SetLocalPosition(new Vec2D(rand.Next(-100, 100), rand.Next(-100, 100)));
                    }
                }
            }
        }
    }
    
    internal class EngineTest1
    {
        public static Scene CreateScene()
        {
            var world = new Scene("World");

            // create a new game object
            var gameObject = new GameObject("Mouse Square", world);

            // add a drawable component to the game object
            gameObject.AddComponent<RotatingSquare>();

            // add a mouse tracker component to the game object
            gameObject.AddComponent<MouseTracker>();

            // add the game object to the world
            // world.AddGameObject(gameObject);

            gameObject.SetPosition(new Vec2D(500, 500));

            return world;
        }
#if ENGINE_TEST
        static void Main(string[] args)
        {
            Console.WriteLine("Starting SDL2 Engine...");

            // create an empty world and add it to the engine
            var myWorld = CreateScene();
            var engine = new Engine(myWorld);


            // run game loop
            engine.Run();

            Console.WriteLine("Exiting SDL2 Engine...");
        }
#endif
    }
}
