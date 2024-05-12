using Pong;
using Pong.src;
using SDL2;
using SDL2Engine;
using System.Reflection.Metadata.Ecma335;
using static SDL2.SDL;
using static System.Formats.Asn1.AsnWriter;


namespace Pong
{
    public enum PlayerType
    {
        WS,
        ArrowKeys,
        Mouse,
        Controller,
        AI
    }
    public static partial class LevelManager
    {
        static bool musikplaying = false;

        public static PlayerType player1Type = PlayerType.WS;
        public static PlayerType player2Type = PlayerType.AI;
        public static GameMode gameMode = GameMode.DUEL;

        public static void Start()
        {


            LoadPlayerSelection();

            var engine = new Engine(null, "PongPongPong");
            engine.Run();
        }


        private static Scene? homeScreen;
        private static Scene? level;
        private static int levelIndex = 0;



        public static void LoadPlayerSelection()
        {

            var playerSelection = UI.PlayerSelectScene();
            if (musikplaying == false)
            {
                musikObject = new GameObject("ScorePlayer1", playerSelection);
                musikObject.AddComponent<HomeMusik>();
                musikplaying = true;
            }

            SceneManager.SetScene(playerSelection);

        }

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
            var controller = gameControllerObject.GetComponent<GameController>();
            if (controller != null)
            {
                controller.level_id = 1;
            }
            musikplaying = (bool)(musikObject.GetComponent<HomeMusik>()?.StopMusic(musikplaying));

            // so some level specific stuff here...
            // for example, add powerup GameObject to level, change speed of ball, idk
            // you could also create a new GameController script that has different logic for this level

            return level;
        }

        public static Scene CreateLevel2()
        {
            //  musikObject.GetComponent<HomeMusik>()?.StopMusic();
            var level = new Scene("Level2");

            var gameControllerObject = new GameObject("GameController2", level);
            gameControllerObject.AddComponent<Level2GameController>();

            return level;
        }

        public static Scene CreateLevel3()
        {
            //     musikObject.GetComponent<HomeMusik>()?.StopMusic();

            var level = new Scene("Level3");

            var gameControllerObject = new GameObject("GameController3", level);
            gameControllerObject.AddComponent<Level3GameController>();

            return level;
        }

        public static Scene CreateLevel4()
        {
            //    musikObject.GetComponent<HomeMusik>()?.StopMusic();

            var level = new Scene("Level4");

            var gameControllerObject = new GameObject("GameController4", level);
            gameControllerObject.AddComponent<Level4GameController>();

            return level;
        }

        public static Scene CreateLevel5()
        {
            //    musikObject.GetComponent<HomeMusik>()?.StopMusic();

            var level = new Scene("Level5");

            var gameControllerObject = new GameObject("GameController5", level);
            gameControllerObject.AddComponent<Level123GameController>();

            return level;
        }

        static GameObject musikObject = new GameObject();

        private static Scene CreateHomeScreen()
        {
            var homeScreen = HomeScreen.CreateScene();


            return homeScreen;
        }
    }


    class HomeMusik : Script
    {
        private Sound scoreSoundFire;
        private bool musicStarted = false;

        public override void Start()
        {
            scoreSoundFire = AssetManager.LoadAsset<Sound>("Assets/Audio/Homemusik.mp3");
            scoreSoundFire.SetVolume(0.3);
        }

        public override void Update()
        {
            // Starte die Musik nur, wenn sie noch nicht gestartet wurde
            if (!musicStarted)
            {
                scoreSoundFire.Play();
                musicStarted = true;
            }
        }

        public bool StopMusic(bool musikplaying)
        {
            // Stoppe die Musik
            scoreSoundFire.Stop();
            musikplaying = false;
            return musikplaying;
        }
    }
    class SpeedPowerup : Script
    {



        public override void Start()
        {
            var powerup = new GameObject("Powerup");
            powerup.AddComponent<TextureRenderer>()?.SetSource("Assets/Textures/speed_powerup.png");
            var bc = BoxCollider.FromDrawableRect(powerup);
            bc.IsTrigger = true;
            powerup.AddComponent<DestroyAndIncreaseSpeedOnCollision>();
            powerup.SetPosition(new Vec2D(500, 500));
        }
        public override void Update()
        {

        }
    }

    class DestroyAndIncreaseSpeedOnCollision : Script
    {
        public override void OnCollisionEnter(CollisionPair collision)
        {
            // destroy if colliding with anything (thats movable, since two static objects can't collide)
            Destroy(this.gameObject);
            //increase ball speed
            foreach (var obj in this.gameObject.GetScene().GetGameObjects())
            {
                if (obj.GetName() == "Ball")
                {
                    obj.GetComponent<PhysicsBody>().Velocity = obj.GetComponent<PhysicsBody>().Velocity * new Vec2D(1.5, 1.5);
                }
            }

        }

        public override void Update()
        {
            // rotate with time

            transform.rotation = Time.time * 360 / 3 % 360;
        }
    }

    class DestroyAndChangeDirectionOnCollision : Script
    {
        public override void OnCollisionEnter(CollisionPair collision)
        {
            // destroy if colliding with anything (thats movable, since two static objects can't collide)
            Destroy(this.gameObject);
            //change ball direction
            foreach (var obj in this.gameObject.GetScene().GetGameObjects())
            {
                if (obj.GetName() == "Ball")
                {
                    obj.GetComponent<PhysicsBody>().Velocity = new Vec2D(obj.GetComponent<PhysicsBody>().Velocity.x, -obj.GetComponent<PhysicsBody>().Velocity.y);
                }
            }

        }

        public override void Update()
        {
            // rotate with time

            transform.rotation = Time.time * 360 / 3 % 360;
        }
    }

    public class Level2GameController : GameController
    {

        public Level2GameController()
        {
            this.level_id = 2;
        }

        public override void Update()
        {

            base.Update();
            Vec2D gameBounds = new Vec2D(1920, 1080);

            if (powerupTimer > 10)
            {
                powerupTimer = 0;
                var powerup = new GameObject("Powerup");
                powerup.AddComponent<TextureRenderer>()?.SetSource("Assets/Textures/speed_powerup.png");
                var bc = BoxCollider.FromDrawableRect(powerup);
                bc.IsTrigger = true;
                powerup.AddComponent<DestroyAndIncreaseSpeedOnCollision>();
                //random value between x and y
                int randomX = new Random().Next((int)gameBounds.x / 2 - 500, (int)gameBounds.x / 2 + 500);
                var randomY = new Random().Next((int)gameBounds.y / 2 - 500, (int)gameBounds.y / 2 + 500);

                powerup.SetPosition(new Vec2D(randomX, randomY));
            }


        }
    }

    public class Level3GameController : GameController
    {

        public Level3GameController()
        {
            this.level_id = 3;
        }

        public override void Update()
        {

            base.Update();
            Vec2D gameBounds = new Vec2D(1920, 1080);

            if (powerupTimer > 6)
            {
                powerupTimer = 0;
                var powerup = new GameObject("Powerup");
                var texture = powerup.AddComponent<TextureRenderer>();
                texture?.SetSource("Assets/Textures/change_direction_powerup.png");

                var bc = BoxCollider.FromDrawableRect(powerup);
                bc.IsTrigger = true;
                powerup.AddComponent<DestroyAndChangeDirectionOnCollision>();
                //random value between x and y
                int randomX = new Random().Next((int)gameBounds.x / 2 - 500, (int)gameBounds.x / 2 + 500);
                var randomY = new Random().Next((int)gameBounds.y / 2 - 500, (int)gameBounds.y / 2 + 500);

                powerup.SetPosition(new Vec2D(randomX, randomY));
            }


        }
    }

    public class Level123GameController : GameController
    {

        private List<GameObject> powerups = new List<GameObject>();
        protected double resetChecker = 0;
        public Level123GameController()
        {
            this.level_id = 6;
        }
        public override void Update()
        {

            base.Update();

            if (powerupTimer > 3)
            {
                powerupTimer = 0;

                if (random.NextDouble() > 0.7)
                {
                    AddPortal();
                }

            }

            if(resetChecker != lastReset)
            {
                foreach (var powerup in powerups)
                {
                    powerup.Destroy();
                }
                powerups.Clear();
                resetChecker = lastReset;
            }


        }

        private void AddPortal()
        {
            var portal = new GameObject("Portal");
            portal.AddComponent<PortalScript>();

            this.gameObject.AddChild(portal);
            this.powerups.Add(portal);

            if (powerups.Count > 2)
            {
                var toRemove = powerups[0];
                powerups.RemoveAt(0);
                toRemove.Destroy();
            }
        }
    }

    public class Level4GameController : GameController
    {
        public Level4GameController()
        {
            this.level_id = 4;
        }
        public override void Update()
        {

            base.Update();
            Vec2D gameBounds = new Vec2D(1920, 1080);

            if (powerupTimer > 5)
            {

                var rand = new Random().Next(0, 2);
                var powerup = new GameObject("Powerup");
                var texture = powerup.AddComponent<TextureRenderer>();
                var bc = BoxCollider.FromDrawableRect(powerup);
                bc.IsTrigger = true;

                if (rand < 1)
                {



                    texture?.SetSource("Assets/Textures/change_direction_powerup.png");


                    powerup.AddComponent<DestroyAndChangeDirectionOnCollision>();

                }
                else
                {


                    texture?.SetSource("Assets/Textures/speed_powerup.png");

                    powerup.AddComponent<DestroyAndIncreaseSpeedOnCollision>();

                }
                //random value between x and y
                int randomX = new Random().Next((int)gameBounds.x / 2 - 500, (int)gameBounds.x / 2 + 500);
                var randomY = new Random().Next((int)gameBounds.y / 2 - 500, (int)gameBounds.y / 2 + 500);

                powerup.SetPosition(new Vec2D(randomX, randomY));
                powerupTimer = 0;
            }


        }
    }
}