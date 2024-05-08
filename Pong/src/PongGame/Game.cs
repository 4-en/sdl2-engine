using SDL2Engine;


namespace Pong
{


    public static class LevelManager
    {
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

        public static Scene CreateBaseLevel()
        {
            var level = new Scene("BaseLevel");

            return level;
        }

        private static Scene CreateLevel1()
        {
            var level = CreateBaseLevel();

            // so some level specific stuff here...

            return level;
        }

        private static Scene CreateLevel2()
        {
            var level = CreateBaseLevel();

            // so some level specific stuff here...

            return level;
        }

    }
}

