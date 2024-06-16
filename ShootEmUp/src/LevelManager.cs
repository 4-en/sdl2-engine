using SDL2Engine;
using ShootEmUp.Level;
using ShootEmUp.src;
using System.Security.Cryptography.X509Certificates;

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
                case 6:
                    level = CreateLevel6();
                    break;
                case 7:
                    level = CreateFinalLevel();
                    break;
                default:
                    EndRun(true);
                    return;
            }
            SceneManager.SetScene(level);
        }

        public static void StartNewRun()
        {
            levelIndex = 0;
            PlayerData.Instance.Reset();
            LoadNextLevel();
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
                        new EnemyWave("test.template", 60, 3),
                    ],
                    20
                );

                var music = Component.CreateWithGameObject<MusicPlayer>("Music Player");
                var player = music.Item2;

                player.playOnAwake = true;
                player.SetSource("Assets/Audio/music1.mp3");
            }

            return level;
        }

        public static Scene CreateLevel1()
        {
            var level = new Scene("Level1");
            using (level.Activate())
            {
                var levelScript = Component.CreateWithGameObject<BaseLevel>("Level").Item2;
                levelScript.SetupLevel(
                    0,
                    [
                        new EnemyWave("Wave1.template", 5, 1),
                        new EnemyWave("Wave2.template", 60, 1),
                    ],
                    30
                );

                var music = Component.CreateWithGameObject<MusicPlayer>("Music Player");
                var player = music.Item2;

                player.playOnAwake = true;
                player.SetSource("Assets/Audio/music1.mp3");
            }

            return level;
        }

        public static Scene CreateLevel2()
        {

            var level = new Scene("Level2");
            using (level.Activate())
            {
                var levelScript = Component.CreateWithGameObject<BaseLevel>("Level").Item2;
                levelScript.SetupLevel(
                    0,
                    [
                        new EnemyWave("Wave3.template", 15, 1),
                        new EnemyWave("Wave2.template", 20, 1),
                        new EnemyWave("Wave4.template", 5, 1),
                        new EnemyWave("Wave2.template", 20, 1),
                    ],
                    60
                );

                var music = Component.CreateWithGameObject<MusicPlayer>("Music Player");
                var player = music.Item2;

                player.playOnAwake = true;
                player.SetSource("Assets/Audio/music2.mp3");
            }

            return level;
        }

        public static Scene CreateLevel3()
        {
            var level = new Scene("Level3");
            using (level.Activate())
            {
                var levelScript = Component.CreateWithGameObject<BaseLevel>("Level 3").Item2;
                levelScript.SetupLevel(
                    3,
                    [
                        new EnemyWave("Wave1.template", 15, 1),
                        new EnemyWave("Wave5.template", 20, 1),
                        new EnemyWave("Wave4.template", 10, 1),
                        new EnemyWave("Wave2.template", 5, 1),

                    ],
                    60
                );

                var music = Component.CreateWithGameObject<MusicPlayer>("Music Player");
                var player = music.Item2;

                player.playOnAwake = true;
                player.SetSource("Assets/Audio/music1.mp3");
            }

            return level;
        }

        public static Scene CreateLevel4()
        {
            var level = new Scene("Level 4");
            using (level.Activate())
            {
                var levelScript = Component.CreateWithGameObject<BaseLevel>("Level 4").Item2;
                levelScript.SetupLevel(
                    4,
                    [
                        new EnemyWave("Level4Setup.template", 60, 1),
                    ],
                    80
                );

                var music = Component.CreateWithGameObject<MusicPlayer>("Music Player");
                var player = music.Item2;

                player.playOnAwake = true;
                player.SetSource("Assets/Audio/music2.mp3");
            }

            return level;
        }

        public static Scene CreateLevel5()
        {
            var level = new Scene("Level 5");
            using (level.Activate())
            {
                var levelScript = Component.CreateWithGameObject<BaseLevel>("Level 5").Item2;
                levelScript.SetupLevel(
                    5,
                    [
                        new EnemyWave("JustBombers.template", 20, 3),
                    ],
                    80
                );

                var music = Component.CreateWithGameObject<MusicPlayer>("Music Player");
                var player = music.Item2;

                player.playOnAwake = true;
                player.SetSource("Assets/Audio/music1.mp3");
            }

            return level;
        }

        public static Scene CreateLevel6()
        {

            var level = new Scene("Level 6");
            using (level.Activate())
            {
                var levelScript = Component.CreateWithGameObject<BaseLevel>("Level 6").Item2;
                levelScript.SetupLevel(
                    6,
                    [
                        new EnemyWave("Level6Start.template", 30, 1),
                        new EnemyWave("Level6Waves.template", 20, 2),
                    ],
                    100
                );

                var music = Component.CreateWithGameObject<MusicPlayer>("Music Player");
                var player = music.Item2;

                player.playOnAwake = true;
                player.SetSource("Assets/Audio/music2.mp3");
            }

            return level;

        }

        public static Scene CreateFinalLevel()
        {
            // basically impossible level
            var level = new Scene("Final Level");
            using (level.Activate())
            {
                var levelScript = Component.CreateWithGameObject<BaseLevel>("Final Level").Item2;
                levelScript.SetupLevel(
                    7,
                    [
                        new EnemyWave("Wave3.template", 20, 1),
                        new EnemyWave("Level6Waves.template", 10, 100),
                    ],
                    600
                );

                var music = Component.CreateWithGameObject<MusicPlayer>("Music Player");
                var player = music.Item2;

                player.playOnAwake = true;
                player.SetSource("Assets/Audio/music3.mp3");
            }

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
