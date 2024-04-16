﻿using System.Security.Cryptography.X509Certificates;
using SDL2;
using static SDL2.SDL;

namespace SDL2Engine
{

    
    internal class Program
    {
        static World CreateWorld()
        {
            var world = new World();

            // create a new game object
            var gameObject = new GameObject(world);

            // add a drawable component to the game object
            var _ = gameObject.AddComponent<RotatingSquare>();

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
