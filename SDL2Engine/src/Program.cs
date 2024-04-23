﻿using System.Security.Cryptography.X509Certificates;
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
            var gameObject1 = new GameObject("Cube1", world);

            // add a drawable component to the game object
            gameObject1.AddComponent<Cube>();

            // add a mouse tracker component to the game object
            gameObject1.AddComponent<WASDController>();

            //add a collider component to the game object
            gameObject1.AddComponent<BoxCollider>();

            // add the game object to the world
            world.AddChild(gameObject1);

            gameObject1.SetPosition(new Vec2D(500, 500));



            // create a new game object
            var gameObject2 = new GameObject("Cube2", world);

            // add a drawable component to the game object
            gameObject2.AddComponent<Cube>();

            //add a collider component to the game object
            gameObject2.AddComponent<BoxCollider>();

            // add the game object to the world
            world.AddChild(gameObject2);

            gameObject2.SetPosition(new Vec2D(800, 700));

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

        public override void Draw(Camera camera)
        {
            var CubeWidth = 50;
            var CubeHeight = 50;
            var MovementSpeed = 10;

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

}
