using SDL2Engine;

namespace TileBasedGame
{

    internal class TileSaveData
    {
        public int unlockedLevel = 1;
        public int totalScore = 0;

        public TileSaveData() { }
    }

    public static partial class LevelManager
    {
        public static int unlockedLevel = 1;
        public static int totalScore = 0;

        public static void Start()
        {
            

            
            var engine = new Engine(null, "TileBasedGame");
            engine.Init();

            // load save data
            var saveData = Serialization.LoadObject<TileSaveData>("saveData.json");
            if (saveData != null)
            {
                unlockedLevel = saveData.unlockedLevel;
                totalScore = saveData.totalScore;
            }

            LoadHomeScreen();

            engine.Run();
        }


        private static Scene? homeScreen;
        private static Scene? shop;
        private static Scene? level;
        public static int levelIndex = 0;


        public static void CompleteCurrentLevel(int score = 0)
        {
            totalScore += score;
            if(levelIndex >= unlockedLevel)
            {
                unlockedLevel++;

                // save data
                var saveData = new TileSaveData();
                saveData.unlockedLevel = unlockedLevel;
                saveData.totalScore = totalScore;
                Serialization.SaveObject(saveData, "saveData.json");
            }
        }

        public static void LoadLevel(int levelIndex=-1)
        {
            if(levelIndex == -1)
            {
                levelIndex = LevelManager.levelIndex;
            }

            levelIndex--;
            LevelManager.levelIndex = levelIndex;
            LoadNextLevel();
        }

        public static void LoadNextLevel()
        {
            levelIndex++;

            // make sure level is also unlocked
            if(!(levelIndex <= unlockedLevel))
            {
                levelIndex = unlockedLevel;
                Console.WriteLine($"Level {levelIndex} is not unlocked yet.");
            }

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

        public static void LoadGameOverScene(int score, int time)
        {
            var scene = UI.GameOverScene(score, time);
            SceneManager.SetScene(scene);
        }

        public static void LoadLevelCompletedScene(int score, int time)
        {
            var scene = UI.LevelCompletedScene(score, time);
            SceneManager.SetScene(scene);
        }

        public static void LoadShop()
        {
            shop = CreateShop();
            SceneManager.SetScene(shop);
        }

        public static Scene CreateBaseLevel()
        {
            var level = new ChunkedScene("TileTest");
            level.SetGravity(200);
            level.LoadTMX("test_map.tmx");

            using (level.Activate())
            {
                var levelScript = Component.CreateWithGameObject<Level>("Level");
            }

            return level;
        }

        public static Scene CreateLevel1()
        {
            var level = new ChunkedScene("Level 1");
            level.SetGravity(200);
            level.LoadTMX("level1.tmx");

            using (level.Activate())
            {
                var levelScript = Component.CreateWithGameObject<Level>("Level");
            }

            return level;
        }

        public static Scene CreateLevel2()
        {
            var level = new ChunkedScene("Level 2");
            level.SetGravity(200);
            level.LoadTMX("level2.tmx");

            using (level.Activate())
            {
                var levelScript = Component.CreateWithGameObject<Level>("Level");
            }

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
