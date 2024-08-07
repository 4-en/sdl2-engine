﻿using SDL2Engine;

namespace TileBasedGame
{
    internal class Program
    {
        static void TestTileLoading()
        {
            var scene = new ChunkedScene("TileTest");
            scene.SetGravity(100);
            scene.LoadTMX("test_map.tmx");

            var engine = new Engine(scene, "TileBasedGame");
            engine.Init();


            engine.Run();


        }

        static void Main(string[] args)
        {
            // print controls
            Console.WriteLine("Controls:");
            Console.WriteLine("W/D to move");
            Console.WriteLine("Space to jump");
            Console.WriteLine("Space in air to air jump");
            Console.WriteLine("Shift to sprint");
            Console.WriteLine("Left mouse click to attack");
            Console.WriteLine("");

            LevelManager.Start();
           

            // TestTileLoading();
        }
    }
}
