using SDL2Engine;

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
            LevelManager.Start();

            // TestTileLoading();
        }
    }
}
