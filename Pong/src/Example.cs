using SDL2Engine;
using SDL2;
using static SDL2.SDL;

namespace Pong.src
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
            if (parent == null)
            {
                return null;
            }

            parent.AddChild(square);
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
            }
            else if (Input.GetMouseButtonDown(2))
            {
                var square = AddSquare(gameObject);
                if (square != null)
                {
                    var rand = new Random();
                    square.SetLocalPosition(new Vec2D(rand.Next(-100, 100), rand.Next(-100, 100)));
                }
            }
            else if (Input.GetMouseButtonDown(1))
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
    class FilledSquare : Drawable
    {

        public override void Draw(Camera camera)
        {
            var renderer = Engine.renderer;

            var root = this.gameObject;

            // set the color to dark blue
            SDL_SetRenderDrawColor(renderer, 255, 255, 255, 255);

            // define the square
            List<Vec2D> points = new List<Vec2D>();
            points.Add(new Vec2D(-50, -50));
            points.Add(new Vec2D(50, -50));
            points.Add(new Vec2D(50, 50));
            points.Add(new Vec2D(-50, 50));

            // convert to camera space
            for (int i = 0; i < points.Count; i++)
            {
                // rotate around center
                Vec2D p = points[i];
                p = camera.WorldToScreen(p, root.GetPosition());
                points[i] = p;
            }

            // draw the filled white square
            var rect = new SDL_Rect();
            rect.x = (int)points[0].x;
            rect.y = (int)points[0].y;
            rect.w = (int)(points[1].x - points[0].x);
            rect.h = (int)(points[2].y - points[1].y);
            SDL_RenderFillRect(renderer, ref rect);
        }
    }

    class WASDController : Script
    {
        public override void Update()
        {
            // a better way to do this would be to use a rigidbody with velocity
            var gameObject = this.gameObject;
            var speed = 5;
            if (Input.GetKeyPressed(((int)SDL_Keycode.SDLK_w)))
            {
                gameObject.transform.position += new Vec2D(0, -speed);
            }
            if (Input.GetKeyPressed(((int)SDL_Keycode.SDLK_s)))
            {
                gameObject.transform.position += new Vec2D(0, speed);
            }
            if (Input.GetKeyPressed(((int)SDL_Keycode.SDLK_a)))
            {
                gameObject.transform.position += new Vec2D(-speed, 0);
            }
            if (Input.GetKeyPressed(((int)SDL_Keycode.SDLK_d)))
            {
                gameObject.transform.position += new Vec2D(speed, 0);
            }
            
        }
    }
    internal class Example
    {

        public static Scene CreateScene()
        {
            var scene = new Scene("Example Scene");

            var mouseTracker = scene.CreateChild("Mouse Tracker");

            _ = mouseTracker.AddComponent<MouseTracker>();

            // see Engine.Rendering.cs for RotatingSquare implementation
            _ = mouseTracker.AddComponent<RotatingSquare>();

            var movingSquare = scene.CreateChild("Moving Square");
            _ = movingSquare.AddComponent<WASDController>();
            _ = movingSquare.AddComponent<FilledSquare>();
            movingSquare.transform.position = new Vec2D(500, 500);

            return scene;
        }
        public static void Run()
        {
            // creates a simple scene with a rotating rectangle that follows the mouse
            // left click to spawn a new rectangle at the mouse position
            var scene = CreateScene();

            // use engine class to run a scene
            // contains the game loop and window creation
            var engine = new SDL2Engine.Engine(scene);

            // create the window and run the game loop until window is closed
            engine.Run();
        }
    }
}
