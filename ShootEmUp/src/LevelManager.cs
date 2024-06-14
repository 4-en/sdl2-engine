using SDL2Engine;
using ShootEmUp.Level;
using ShootEmUp.src;

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
            var playerData = PlayerData.Instance;
            Console.WriteLine($"Loaded player data - Score: {playerData.TotalScore}, Level: {playerData.LevelProgress}, Money: {playerData.Money}");
            engine.Run();
        }


        private static Scene? homeScreen;
        private static Scene? shop;
        private static Scene? level;
        private static int levelIndex = 0;

        public static void LoadNextLevel()
        {
            levelIndex++;
            switch (levelIndex)
            {
                case 1:
                    level = CreateLevel1();
                    break;
                default:
                    level = CreateBaseLevel();
                    break;
            }
            SceneManager.SetScene(level);
        }

        public static void LoadTutorial()
        {
            // TODO: Implement tutorial level
            level = CreateBaseLevel();
            SceneManager.SetScene(level);
        }

        public static void EndRun(bool win)
        {
            // TODO: Implement end run screen
            var playerData = PlayerData.Instance;
            var endScene = EndScreen.CreateScene();
            SceneManager.SetScene(endScene);
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

        public static Scene CreateShop()
        {
            var shop = Shop.CreateScene();

            return shop;
        }
    }

}
