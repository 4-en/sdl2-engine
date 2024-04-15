using System.Security.Cryptography.X509Certificates;
using SDL2;
using static SDL2.SDL;

namespace SDL2Engine
{

    class MyWorld : World
    {
        public MyWorld()
        {
          
        }

        public override void Draw(ICamera camera)
        {
            // draw a triangle
            SDL.SDL_SetRenderDrawColor(Engine.renderer, 255, 0, 0, 255);
            SDL.SDL_RenderDrawLine(Engine.renderer, 320, 50, 520, 240);
            SDL.SDL_RenderDrawLine(Engine.renderer, 520, 240, 120, 240);
            SDL.SDL_RenderDrawLine(Engine.renderer, 120, 240, 320, 50);



        }
    }
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Starting SDL2 Engine...");

            // create an empty world and add it to the engine
            var myWorld = new MyWorld();
            var engine = new Engine(myWorld);


            // run game loop
            engine.Run();

            Console.WriteLine("Exiting SDL2 Engine...");
        }
    }
}
