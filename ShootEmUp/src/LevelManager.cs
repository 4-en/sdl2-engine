using SDL2Engine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShootEmUp
{
    public static partial class LevelManager
    {
        


        public static void Start()
        {
            LoadHomeScreen();
            var engine = new Engine(null, "ShootEmUp");
            engine.Run();
        }


        private static Scene? homeScreen;
        private static Scene? level;
        private static int levelIndex = 0;


        public static void LoadHomeScreen()
        {

            homeScreen = CreateHomeScreen();
            SceneManager.SetScene(homeScreen);

        }

        public static Tuple<Scene, GameObject> CreateBaseLevel()
        {
            var level = new Scene("BaseLevel");
            var gameControllerObject = new GameObject("GameController", level);
            gameControllerObject.AddComponent<GameController>();

            return new Tuple<Scene, GameObject>(level, gameControllerObject);
        }

        public static Scene CreateLevel1()
        {
            var tuple = CreateBaseLevel();
            var level = tuple.Item1;
            var gameControllerObject = tuple.Item2;
            

            return level;
        }

      

        private static Scene CreateHomeScreen()
        {
            var homeScreen = HomeScreen.CreateScene();

            return homeScreen;
        }
    }

}
