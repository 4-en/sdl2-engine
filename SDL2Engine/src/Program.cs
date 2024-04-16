﻿using System.Security.Cryptography.X509Certificates;
using SDL2;
using static SDL2.SDL;

namespace SDL2Engine
{

    class MouseTracker : Component
    {
        public override void Update()
        {
            var mouseState = SDL_GetMouseState(out int x, out int y);
            var mousePosition = new Vec2D(x, y);
            
            var gameObject = this.gameObject;
            var camera = Camera.GetCamera(gameObject);
            if (camera != null)
            {
                mousePosition = camera.ScreenToWorld(mousePosition);
            }

            gameObject.SetPosition(mousePosition);
        }
    }
    
    internal class Program
    {
        static Scene CreateWorld()
        {
            var world = new Scene();

            // create a new game object
            var gameObject = new GameObject(world);

            // add a drawable component to the game object
            _ = gameObject.AddComponent<RotatingSquare>();

            // add a mouse tracker component to the game object
            _ = gameObject.AddComponent<MouseTracker>();

            // add the game object to the world
            world.AddChild(gameObject);

            gameObject.SetPosition(new Vec2D(500, 500));

            return world;
        }
        static void Main(string[] args)
        {
            Console.WriteLine("Starting SDL2 Engine...");

            // create an empty world and add it to the engine
            var myWorld = CreateWorld();
            var engine = new Engine(myWorld);


            // run game loop
            engine.Run();

            Console.WriteLine("Exiting SDL2 Engine...");
        }
    }
}
