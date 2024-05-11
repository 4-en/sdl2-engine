using Pong;
using Pong.src;
using SDL2;
using SDL2Engine;
using System.Reflection.Metadata.Ecma335;
using static SDL2.SDL;


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

            if (homeScreen != null)
            {
                SceneManager.RemoveScene(homeScreen);
                homeScreen = null;
            }
            var creator = levelCreators[index];
            var new_level = creator();
            SceneManager.SetScene(new_level);
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

        public static void LoadPlayerSelection()
        {
            var playerSelection = UI.PlayerSelectScene();
            SceneManager.SetScene(playerSelection);
        }

        public static void LoadHomeScreen()
        {
            
            homeScreen = CreateHomeScreen();
            SceneManager.SetScene(homeScreen);
            
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

        public static Scene CreateLevel2()
        {
            var level = new Scene("Level2");

            var gameControllerObject = new GameObject("GameController2", level);
            gameControllerObject.AddComponent<Level2GameController>();

            return level;
        }

        public static Scene CreateLevel3()
        {
            var level = new Scene("Level3");

            var gameControllerObject = new GameObject("GameController3", level);
            gameControllerObject.AddComponent<Level3GameController>();

            return level;
        }

        public static Scene CreateLevel4()
        {
            var level = new Scene("Level4");

            var gameControllerObject = new GameObject("GameController4", level);
            gameControllerObject.AddComponent<Level4GameController>();

            return level;
        }

        private static Scene CreateHomeScreen()
            {
                var homeScreen = HomeScreen.CreateScene();
                return homeScreen;
            }
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
                obj.GetComponent<PhysicsBody>().Velocity =   new Vec2D(obj.GetComponent<PhysicsBody>().Velocity.x, - obj.GetComponent<PhysicsBody>().Velocity.y);
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

public class Level4GameController : GameController
{

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