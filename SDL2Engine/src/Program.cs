using System.Security.Cryptography.X509Certificates;
using SDL2;
using static SDL2.SDL;

using SDL2Engine;
using SDL2Engine.Utils;

namespace SDL2Engine.Testing
{

    internal class EngineEntryPoint
    {
       
#if ENGINE_TEST


        static void Main(string[] args)
        {


            Console.WriteLine("Starting SDL2 Engine...");

            // run test here \/ \/ \/

            

            Console.WriteLine("Exiting SDL2 Engine...");
        }
#endif
    }


}
