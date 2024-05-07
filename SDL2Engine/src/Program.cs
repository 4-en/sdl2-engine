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
            // destroy if colliding with anything (thats movable, since two static objects can't collide)
            Destroy(this.gameObject);

        }

        public override void Update()
        {
            // rotate with time

            transform.rotation = Time.time * 360 / 3 % 360;
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
            square.AddComponent<TextureRenderer>()?.SetSource("Assets/Textures/forsenE.png");
            var bc = BoxCollider.FromDrawableRect(square);
            square.AddComponent<DestroyOnCollision>();
            square.SetPosition(this.gameObject.GetPosition());


            return square;
            
        }

        private Sound sound = AssetManager.LoadAsset<Sound>("Assets/Audio/test_sound.mp3");
        private Music music = AssetManager.LoadAsset<Music>("Assets/Audio/tetris.mp3");

        public override void Start()
        {
            Console.WriteLine("MouseTracker Start");
            // play music
            if (music != null)
            {
                music.Play(-1);
            }
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
                // play a sound
                var sound = this.sound;
                if (sound != null)
                {
                    
                    if (sound.IsPlaying())
                    {
                        sound.Stop();
                    } else
                    {
                        sound.Play();
                    }
                } else
                {
                    Console.WriteLine("Sound is null");
                }

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
            var gameObject1 = new GameObject("Cube1", world);

            // add a drawable component to the game object
            var cube1 = gameObject1.AddComponent<Cube>();

            //add a collider component to the game object
            var boxCollider1 = gameObject1.AddComponent<BoxCollider>();

            // add a mouse tracker component to the game object
            gameObject1.AddComponent<WASDController>();

            //add a physics component to the game object
            gameObject1.AddComponent<PhysicsBody>();
            gameObject1.GetComponent<PhysicsBody>().Velocity = new Vec2D(-1, -1);
            //gameObject1.GetComponent<PhysicsBody>().Velocity = new Vec2D(-1, 0);

            // add the game object to the world
            //world.AddChild(gameObject1);
            world.AddGameObject(gameObject1);

            gameObject1.SetPosition(new Vec2D(500, 500));
            //gameObject1.SetPosition(new Vec2D(500, 300));



            // create a new game object
            var gameObject2 = new GameObject("Cube2", world);

            // add a drawable component to the game object
            var cube2 = gameObject2.AddComponent<Cube>();

            //add a collider component to the game object
            var boxCollider2 = gameObject2.AddComponent<BoxCollider>();

            //add physics component to the game object
            var physicsBody = gameObject2.AddComponent<PhysicsBody>();
            physicsBody.Velocity = new Vec2D(0, 0);
            physicsBody.IsMovable = false;
            physicsBody.Mass = 2;

            // add the game object to the world
            //world.AddChild(gameObject2);
            world.AddGameObject(gameObject2);

            gameObject2.SetPosition(new Vec2D(280, 300));

            //adjust collider to cube position
            if (gameObject2.collider != null && gameObject2.collider is BoxCollider)
            {
                var c = (BoxCollider)gameObject2.collider;
                c.UpdateColliderPosition(gameObject2.GetPosition());
            }



            // create a new game object
            var gameObject3 = new GameObject("Circle", world);

            // add a drawable component to the game object
            var cube3 = gameObject3.AddComponent<Cube>();

            //add a collider component to the game object
            var circleCollider = gameObject3.AddComponent<CircleCollider>();

            //add physics component to the game object
            var physicsBody3 = gameObject3.AddComponent<PhysicsBody>();

            gameObject3.SetPosition(new Vec2D(100, 500));

            //adjust collider to cube size and position
            if (gameObject3.collider != null && gameObject3.collider is CircleCollider)
            {
                
                var c = (CircleCollider)gameObject3.collider;
                c.UpdateColliderPosition(new Vec2D(gameObject3.GetPosition().x+10,gameObject3.GetPosition().y+10));
                c.UpdateColliderSize(20);
            }

            // add the game object to the world
            //world.AddChild(gameObject3);
            world.AddGameObject(gameObject3);



            // create a new game object
            var gameObject4 = new GameObject("Line", world);

            // add a drawable component to the game object
            var line = gameObject4.AddComponent<Line>();

            //add a collider component to the game object
            var edgeCollider = gameObject4.AddComponent<EdgeCollider>();
            edgeCollider.UpdateColliderPosition(new Vec2D(300, 800), new Vec2D(500, 1100));

            //add physics component to the game object
            var physicsBody2 = gameObject4.AddComponent<PhysicsBody>();
            physicsBody2.Velocity = new Vec2D(0, 0);
            physicsBody2.IsMovable = false;

            

            // add the game object to the world
            //world.AddChild(gameObject4);
            world.AddGameObject(gameObject4);



            return world;


            //var world = new Scene("World");

            // create a new game object
            //var gameObject = new GameObject("Mouse Square", world);

            // add a drawable component to the game object
            //gameObject.AddComponent<RotatingSquare>();

            // add a mouse tracker component to the game object
            //gameObject.AddComponent<MouseTracker>();

            // add the game object to the world
            //world.AddChild(gameObject);

            // gameObject.SetPosition(new Vec2D(500, 500));

            //return world;
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

    class Cube : Drawable
    {
        public int CubeWidth { get; set; } = 50;
        public int CubeHeight { get; set; } = 50;

        public override void Draw(Camera camera)
        {
            


            var renderer = Engine.renderer;

            var root = this.gameObject;




            // set the color to dark blue
            SDL_SetRenderDrawColor(renderer, 255, 255, 255, 255);





            // define the square
            List<Vec2D> points = new List<Vec2D>();
            points.Add(new Vec2D(-CubeWidth / 2, -CubeHeight / 2));
            points.Add(new Vec2D(CubeWidth / 2, -CubeHeight / 2));
            points.Add(new Vec2D(CubeWidth / 2, CubeHeight / 2));
            points.Add(new Vec2D(-CubeWidth / 2, CubeHeight / 2));

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

            //adjust collider to cube size
            var boxCollider = this.gameObject.GetComponent<BoxCollider>();
            if (boxCollider != null)
            {
                boxCollider.UpdateColliderSize(CubeWidth, CubeHeight);
            }
        }
    }

    class WASDController : Script
    {

        private BoxCollider? boxCollider;

        public override void Start()
        {
            //boxCollider = gameObject.GetComponent<BoxCollider>();
        }

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

            // Update BoxCollider position
            if (gameObject.collider != null && gameObject.collider is BoxCollider)
            {
                var c = (BoxCollider )gameObject.collider;
                c.UpdateColliderPosition(gameObject.GetPosition());
            }

        }
    }

    class Line : Drawable
    {
        public int startx { get; set; } = 300;
        public int starty { get; set; } = 800;
        public int endx { get; set; } = 500;
        public int endy { get; set; } = 1100;

        public override void Draw(Camera camera)
        {
            var renderer = Engine.renderer;

            var root = this.gameObject;

            List<Vec2D> points = new List<Vec2D>();
            points.Add(new Vec2D(300, 800));
            points.Add(new Vec2D(500, 1100));

            // convert to camera space
            for (int i = 0; i < points.Count; i++)
            {
                // rotate around center
                Vec2D p = points[i];
                p = camera.WorldToScreen(p, root.GetPosition());
                points[i] = p;
            }

            SDL_SetRenderDrawColor(renderer, 255, 255, 255, 255);
            
            SDL_RenderDrawLine(renderer, (int)points[0].x, (int)points[0].y, (int)points[1].x, (int)points[1].y);

        }
    }

}
