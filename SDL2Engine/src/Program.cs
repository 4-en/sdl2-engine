namespace SDL2Engine
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Starting SDL2 Engine...");

            // create an empty world and add it to the engine
            var myWorld = new World();
            var engine = new Engine(myWorld);


            // run game loop
            engine.Run();

            Console.WriteLine("Exiting SDL2 Engine...");
        }
    }
}
