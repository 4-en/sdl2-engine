using SDL2Engine;

namespace Pong
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var scene = new Scene("Pong");
            var engine = new Engine(scene);

            engine.Run();
        }
    }
}
