using SDL2Engine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TileBasedGame
{
    public static partial class LevelManager
    {



        public static void Start()
        {
            LoadHomeScreen();
            var engine = new Engine(null, "TileBasedGame");
            engine.Init();
            engine.Run();
        }


        private static Scene? homeScreen;
        private static Scene? shop;
        private static Scene? level;
        public static int levelIndex = 0;

        public static void LoadNextLevel()
        {
            levelIndex++;
            switch (levelIndex)
            {
                case 1:
                    level = CreateLevel1();
                    break;
                case 2:
                    level = CreateLevel2();
                    break;
                case 3:
                    level = CreateLevel3();
                    break;
                case 4:
                    level = CreateLevel4();
                    break;
                case 5:
                    level = CreateLevel5();
                    break;
                default:
                    level = CreateBaseLevel();
                    return;
            }
            SceneManager.SetScene(level);
        }

        public static void StartNewRun()
        {
            levelIndex = 0;
            LoadNextLevel();
        }

        public static void LoadTutorial()
        {
            // TODO: Implement tutorial level
            level = CreateBaseLevel();
            SceneManager.SetScene(level);
        }

        public static void LoadHomeScreen()
        {

            homeScreen = CreateHomeScreen();
            SceneManager.SetScene(homeScreen);

        }

        public static void LoadShop()
        {
            shop = CreateShop();
            SceneManager.SetScene(shop);
        }

        public static Scene CreateBaseLevel()
        {
            var level = new ChunkedScene("TileTest");
            level.SetGravity(100);
            level.LoadTMX("test_map.tmx");
            return level;
        }

        public static Scene CreateLevel1()
        {
            var level = CreateBaseLevel();
            return level;
        }

        public static Scene CreateLevel2()
        {
            var level = CreateBaseLevel();
            return level;
        }

        public static Scene CreateLevel3()
        {
            var level = CreateBaseLevel();
            return level;
        }

        public static Scene CreateLevel4()
        {
            var level = CreateBaseLevel();
            return level;
        }

        public static Scene CreateLevel5()
        {
            var level = CreateBaseLevel();
            return level;
        }

        private static Scene CreateHomeScreen()
        {
            var homeScreen = HomeScreen.CreateScene();

            return homeScreen;
        }

        public static Scene CreateShop()
        {
            //var shop = Shop.CreateScene();
            var shop = CreateBaseLevel();

            return shop;
        }
    }
}
