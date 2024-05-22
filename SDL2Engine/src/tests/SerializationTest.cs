using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SDL2Engine.tests
{
    internal class SerializationTest
    {

        public static Scene CreateScene()
        {
            Scene scene = new Scene();
            GameObject gameObject = new GameObject();
            scene.AddGameObject(gameObject);
            return scene;
        }

        public static void Run()
        {
            Console.WriteLine("SerializationTest Run");
        }
    }
}
