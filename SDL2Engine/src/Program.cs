using System.Security.Cryptography.X509Certificates;
using SDL2;
using static SDL2.SDL;

namespace SDL2Engine
{

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
            else
            {
                this.gameObject.GetScene()?.AddGameObject(square); 
            }
            _ = square.AddComponent<RotatingSquare>();
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
    
    internal class EngineTest
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
