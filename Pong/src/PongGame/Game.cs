using Pong.src;
using SDL2Engine;


namespace Pong
{

    public static partial class LevelManager
    {
        public static void Start()
        {
            
            LoadHomeScreen();

            var engine = new Engine();
            engine.Run();
        }


        private static Scene? homeScreen;
        private static Scene? level;
        private static int levelIndex = 0;

        private static Dictionary<int, Func<Scene>> levelCreators = new Dictionary<int, Func<Scene>>
        {
            { 0, CreateBaseLevel },
            { 1, CreateLevel1 },
            { 2, CreateLevel2 }
        };

        public static bool RegisterLevelCreator(int index, Func<Scene> creator)
        {
            if (levelCreators.ContainsKey(index))
            {
                return false;
            }

            levelCreators.Add(index, creator);
            return true;
        }

        public static int[] GetLevelIndices()
        {
            return levelCreators.Keys.ToArray();
        }
        
        public static void LoadLevel(int index)
        {
            if (!levelCreators.ContainsKey(index))
            {
                throw new Exception($"No level creator found for index {index}");
            }
            levelIndex = index;
            if (level != null)
            {
                SceneManager.RemoveScene(level);
                level = null;
            }

            var creator = levelCreators[index];
            var new_level = creator();
            SceneManager.AddScene(new_level);
            level = new_level;

        }

        public static void LoadNextLevel()
        {
            int nextLevelIndex = levelIndex;
            if (level != null)
            {
                nextLevelIndex = levelIndex + 1;
            }
            LoadLevel(nextLevelIndex);
        }

        public static void LoadHomeScreen()
        {
            if (homeScreen == null)
            {
                homeScreen = CreateHomeScreen();
                SceneManager.AddScene(homeScreen);
            }
            else
            {
                SceneManager.SetActiveScene(homeScreen);
            }
        }

        public static Scene CreateBaseLevel()
        {
            var level = new Scene("BaseLevel");

            var gameControllerObject = new GameObject("GameController", level);
            gameControllerObject.AddComponent<GameController>();

            return level;
        }

        public static Scene CreateLevel1()
        {
            var level = CreateBaseLevel();

            // so some level specific stuff here...
            // for example, add powerup GameObject to level, change speed of ball, idk
            // you could also create a new GameController script that has different logic for this level

            return level;
        }

        private static Scene CreateLevel2()
        {
            var level = CreateBaseLevel();

            // so some level specific stuff here...

            return level;
        }

        private static Scene CreateHomeScreen()
        {
            var homeScreen = HomeScreen.CreateScene();
            return homeScreen;
        }
    }
}

