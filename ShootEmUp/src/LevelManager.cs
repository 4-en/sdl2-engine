using SDL2Engine;
using ShootEmUp.Level;
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
            engine.Init();
            LoadPrototypes.Load();
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

        public static Scene CreateBaseLevel()
        {
            var level = new Scene("TestLevel");
            using (level.Activate())
            {
                var levelScript = Component.CreateWithGameObject<BaseLevel>("Level").Item2;
                levelScript.SetupLevel(
                    0,
                    [
                        new EnemyWave("test.template", 60, 3)
                    ]
                );
            }

            return level;
        }

        public static Scene CreateLevel1()
        {
            var level = CreateBaseLevel();

            return level;
        }

      

        private static Scene CreateHomeScreen()
        {
            var homeScreen = HomeScreen.CreateScene();

            return homeScreen;
        }
    }

}
