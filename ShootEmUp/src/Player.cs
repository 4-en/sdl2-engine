using SDL2;
using SDL2Engine;
using ShootEmUp.Entities;
using ShootEmUp.Level;
using System;
using System.Collections.Generic;
using System.Diagnostics.Metrics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static SDL2.SDL;
using static ShootEmUp.IngameUI;
using static System.Net.Mime.MediaTypeNames;

namespace ShootEmUp
{

    public class PlayerDamageEvent
    {
        public Damage Damage { get; set; }
        public GameObject Player { get; set; }

        public PlayerDamageEvent(Damage damage, GameObject player)
        {
            Damage = damage;
            Player = player;
        }

    }

    public class Player : Script, IDamageable
    {
        protected Vec2D gameBounds = new Vec2D(1920, 1080);

        public static String spaceshipTexture = "";

        public static int speed;
        public static int maxSpeed;
        public static int minSpeed;
        public static int acceleration;
        public static double rotationSpeed;
        public static int projectileSpeed;
        public static double maxHealth;
        public static double currentHealth;
        public static int damage;
        public static int displayedHighscore;
        public static int displayedMoney;
        public static bool hasShield;
        public int fireRate;
        public int fireRange;
        public int shield;
        internal static int boostSpeed;
        internal static int availableboost;

        public Player()
        {

            spaceshipTexture = "Assets/Textures/spaceships/spaceship5.png";
            speed = 0;
            maxSpeed = 550 + PlayerData.Instance.SpeedUpgradeLevel * 100;
            minSpeed = 25;
            acceleration = 1;
            rotationSpeed = 300;
            projectileSpeed = 800;
            maxHealth = 1000 + PlayerData.Instance.HealthUpgradeLevel * 200;
            currentHealth = maxHealth;
            damage = 50 + PlayerData.Instance.DamageUpgradeLevel * 25;
            displayedHighscore = 0;
            displayedMoney = 0;
            hasShield = false;
            fireRate = 10;
            fireRange = 10;
            shield = 10;
            availableboost = 2;
            boostSpeed = 400;

        }


        public override void Start()
        {
            var fireDrive = new GameObject("FireDrive");
            fireDrive.AddComponent<FireDrive>();



            var texture = AddComponent<TextureRenderer>();
            texture.SetSource(spaceshipTexture);
            texture.SetZIndex(-1);

            AddComponent<CameraFollow>();

            BoxCollider.FromDrawableRect(gameObject);
            AddComponent<KeyboardController>();
            AddComponent<UserInterface>();
            gameObject.transform.position = new Vec2D(0, 0);
            var pb = AddComponent<PhysicsBody>();
            gameObject.AddComponent<BoarderDamager>();

            //camera for level building
            /*
            var camera = GetCamera();
            if (camera != null)
            {
                camera.WorldSize = new Vec2D(5000, 5000);
            }*/


        }
        public override void Update()
        {
        }

        public static GameObject CreatePlayer()
        {
            var player = new GameObject("Player");
            player.AddComponent<Player>();
            ShootEmUp.Entities.HealthBar.AddTo(player, -100);
            return player;
        }

        public void Damage(Damage damage)
        {
            if (Player.hasShield)
            {
                Player.hasShield = false;
                return;
            }

            EventBus.Dispatch(new PlayerDamageEvent(damage, gameObject));

            speed = speed / 2;

            if (speed < minSpeed)
            {
                speed = minSpeed;
            }

            currentHealth -= damage.Value;
        }

        public void Heal(double value)
        {
            currentHealth += value;
            if (currentHealth > maxHealth)
            {
                currentHealth = maxHealth;
            }
        }

        public double GetHealth()
        {
            return currentHealth;
        }

        public double GetMaxHealth()
        {
            return maxHealth;
        }

        public void SetHealth(double value)
        {
            currentHealth = value;
            if (currentHealth > maxHealth)
            {
                currentHealth = maxHealth;
            }
        }

        public void SetMaxHealth(double value)
        {
            if (currentHealth >= maxHealth)
            {
                maxHealth = value;
                currentHealth = value;
            }
            else
            {
                maxHealth = value;
            }

            if (currentHealth > maxHealth)
            {
                currentHealth = maxHealth;
            }
        }

        public Team GetTeam()
        {
            return Team.Player;
        }
    }



    public class KeyboardController : Script
    {
        public int left = (int)SDL_Keycode.SDLK_a;
        public int right = (int)SDL_Keycode.SDLK_d;
        public int up = (int)SDL_Keycode.SDLK_w;
        public int down = (int)SDL_Keycode.SDLK_s;
        public int space = (int)SDL_Keycode.SDLK_SPACE;
        public int enter = (int)SDL_Keycode.SDLK_RETURN;
        public int leftShift = (int)SDL_Keycode.SDLK_LSHIFT;


        public double boostEndTime = 0;
        public double boostDecay = 200;
        public bool isBoosting = false;
        public double projectileRate = 0.2;
        private double lastProjectileTime = 0;



        public override void Start()
        {

        }

        public override void Update()
        {
            if (Input.GetKeyPressed(left))
            {
                gameObject.transform.rotation -= Player.rotationSpeed * Time.deltaTime;
            }
            if (Input.GetKeyPressed(right))
            {
                gameObject.transform.rotation += Player.rotationSpeed * Time.deltaTime;
            }
            if (Input.GetKeyPressed(space))
            {
                if (Time.time - lastProjectileTime > projectileRate)
                {
                    lastProjectileTime = Time.time;

                    Projectile.CreateAt(gameObject);
                }
            }
            if (Input.GetKeyPressed(up))
            {
                Player.speed = Player.speed < Player.maxSpeed ? Player.speed + Player.acceleration : Player.speed;
            }
            if (Input.GetKeyPressed(down))
            {
                Player.speed = Player.speed > 0 && Player.speed > Player.minSpeed ? Player.speed - Player.acceleration : Player.speed;
            }
            if (Input.GetKeyDown(leftShift) && !isBoosting && Player.availableboost > 0)
            {
                isBoosting = true;
                boostEndTime = Time.time + 1.0f; // Boost lasts for 1 second
                Player.speed += Player.boostSpeed;
                Player.availableboost -= 1; // Decrease available boost count
                Console.WriteLine("availableboosts " + Player.availableboost);
            }
            if (isBoosting && Time.time >= boostEndTime)
            {
                // Calculate speed reduction rate per second
                float speedReductionRate = (float)(boostDecay * Time.deltaTime);
                // Reduce speed slowly down to maxSpeed
                Player.speed = (int)Math.Max(Player.maxSpeed, Player.speed - speedReductionRate);

                // Check if speed should be reset to maxSpeed
                if (Player.speed <= Player.maxSpeed)
                {
                    isBoosting = false; // Reset the boost state
                }
                //  Console.WriteLine("Current speed: " + Player.speed);
            }

            if (Input.GetKeyDown(enter))
            {
                if (Player.spaceshipTexture == "Assets/Textures/spaceships/spaceship1.png")
                {
                    gameObject.GetComponent<TextureRenderer>()?.SetSource("Assets/Textures/spaceships/spaceship2.png");
                    Player.spaceshipTexture = "Assets/Textures/spaceships/spaceship2.png";
                }
                else if (Player.spaceshipTexture == "Assets/Textures/spaceships/spaceship2.png")
                {
                    gameObject.GetComponent<TextureRenderer>()?.SetSource("Assets/Textures/spaceships/spaceship3.png");
                    Player.spaceshipTexture = "Assets/Textures/spaceships/spaceship3.png";
                }
                else if (Player.spaceshipTexture == "Assets/Textures/spaceships/spaceship3.png")
                {
                    gameObject.GetComponent<TextureRenderer>()?.SetSource("Assets/Textures/spaceships/spaceship4.png");
                    Player.spaceshipTexture = "Assets/Textures/spaceships/spaceship4.png";
                }
                else if (Player.spaceshipTexture == "Assets/Textures/spaceships/spaceship4.png")
                {
                    gameObject.GetComponent<TextureRenderer>()?.SetSource("Assets/Textures/spaceships/spaceship5.png");
                    Player.spaceshipTexture = "Assets/Textures/spaceships/spaceship5.png";
                }
                else if (Player.spaceshipTexture == "Assets/Textures/spaceships/spaceship5.png")
                {
                    gameObject.GetComponent<TextureRenderer>()?.SetSource("Assets/Textures/spaceships/spaceship1.png");
                    Player.spaceshipTexture = "Assets/Textures/spaceships/spaceship1.png";
                }
            }

            //rotate velocity
            var physicsBody = gameObject.GetComponent<PhysicsBody>();
            if (physicsBody != null)
            {
                physicsBody.Velocity = new Vec2D(Player.speed, 0).Rotate(gameObject.transform.rotation);
            }


            // other keybindings

            // fire target rocket
            if (Input.GetKeyPressed(SDL_Keycode.SDLK_q))
            {
                ShootRocket();
            }
        }

        private double lastFireTime = 0;
        private void ShootRocket()
        {

            if (Time.time - lastFireTime < 1)
            {
                return;
            }

            if(PlayerData.Instance.RocketCount <= 0)
            {
                return;
            }

            PlayerData.Instance.RocketCount -= 1;

            lastFireTime = Time.time;

            var missile = Prototype.Instantiate("TargetingRocket");

            var body = GetComponent<PhysicsBody>();

            if (missile == null)
            {
                Console.WriteLine("Could not instantiate missile");
                return;
            }

            if (body == null)
            {
                Console.WriteLine("Could not find physics body");
                return;
            }

            double rotation = gameObject.transform.rotation;

            missile.SetPosition(gameObject.GetPosition() + new Vec2D(rotation / 180 * double.Pi).Normalize() * 100);

            var missileBody = missile.GetComponent<PhysicsBody>();
            if (missileBody == null)
            {
                Console.WriteLine("Could not find missile physics body");
                return;
            }

            TargetingRocket? targetingRocket = missile.GetComponent<TargetingRocket>();

            if (targetingRocket != null)
            {
                targetingRocket.team = Team.Player;
                targetingRocket.shooter = gameObject;
                targetingRocket.speed = body.Velocity.Length() + 200;
                targetingRocket.damage = PlayerData.Instance.DamageUpgradeLevel * 200 + 200;
            }

            missileBody.Velocity = new Vec2D(rotation / 180 * double.Pi).Normalize() * 200;
            if (missileBody.Velocity.Length() == 0)
            {
                Console.WriteLine("Missile velocity is 0");
            }
        }



    }

    public class CameraFollow : Script
    {
        // The size of the game window
        Vec2D gameBounds = new Vec2D(1920, 1080);

        // The size of the world in which the camera should follow the player
        protected Vec2D WorldSize = BaseLevel.WorldSize;


        public override void Update()
        {


            var camera = GetCamera();

            // Get the current position of the camera and the player
            Vec2D cameraPosition = camera.GetPosition();
            Vec2D playerPosition = gameObject.GetPosition();

            // Set the new camera position
            UpdateCameraPositionToPlayer(playerPosition, camera.GetVisibleSize(), WorldSize);



        }

        private void UpdateCameraPositionToPlayer(Vec2D playerPosition, Vec2D gameBounds, Vec2D bordersSize)
        {
            var camera = GetCamera();

            double camHalfWidth = camera.GetVisibleWidth() / 2;
            double camHalfHeight = camera.GetVisibleHeight() / 2;

            double x = playerPosition.x;
            double y = playerPosition.y;

            double minX = camHalfWidth - bordersSize.x;
            double minY = camHalfHeight - bordersSize.y;
            double maxX = bordersSize.x - camHalfWidth;
            double maxY = bordersSize.y - camHalfHeight;

            if (x < minX)
            {
                x = minX;
            }
            else if (x > maxX)
            {
                x = maxX;
            }

            if (y < minY)
            {
                y = minY;
            }
            else if (y > maxY)
            {
                y = maxY;
            }

            camera.SetPosition(new Vec2D(x - camHalfWidth, y - camHalfHeight));
        }
        public override void Start()
        {
            //// collision test object
            //var obstacle = new GameObject("Obstacle");
            //var pb = obstacle.AddComponent<PhysicsBody>();
            //var bc = obstacle.AddComponent<BoxCollider>();
            //bc.UpdateColliderSize(40, 40);
            //obstacle.transform.position = new Vec2D((gameBounds.x / 2) - 290, 500);

            // collision test object
            //var obstacle2 = new GameObject("Obstacle");
            //var pb2 = obstacle2.AddComponent<PhysicsBody>();
            //var bc2 = obstacle2.AddComponent<BoxCollider>();

            //bc2.UpdateColliderSize(5000, 300);
            //obstacle2.transform.position = new Vec2D(-2500, 2w

        }
    }




    public class Projectile
    {
        public static void CreateAt(GameObject gameObject)
        {
            var projectile = new GameObject("Projectile");
            var projectileScript = projectile.AddComponent<ProjectileScript>();
            projectileScript.team = Team.Player;
            projectileScript.damage = Player.damage;

            //set the position of the projectile to the position of the player
            //add a small offset to the position of the player to avoid collision with the player  
            projectile.transform.position = gameObject.transform.position + new Vec2D(100, 0).Rotate(gameObject.transform.rotation);
            projectile.transform.rotation = gameObject.transform.rotation;
            var pb = projectile.AddComponent<PhysicsBody>();
            pb.Velocity = new Vec2D(Player.speed + Player.projectileSpeed, 0).Rotate(gameObject.transform.rotation);
            var spriteRenderer = projectile.AddComponent<SpriteRenderer>();
            if (spriteRenderer != null)
            {
                spriteRenderer.SetTexture("Assets/Textures/projectile_sprite_sheet.png");
                spriteRenderer.SetSpriteSize(400, 400);
                spriteRenderer.SetSize(60, 60);
                spriteRenderer.AddAnimation(new AnimationInfo("projectile", 0, 4, 0.075));
                spriteRenderer.PlayAnimation("projectile");
                spriteRenderer.SetAnimationType(AnimationType.LoopReversed);
            }
            BoxCollider.FromDrawableRect(projectile);


        }
    }

    public class FireDrive : Script
    {
        public override void Start()
        {

            var sprite = AddComponent<SpriteRenderer>();
            sprite.SetTexture("Assets/Textures/fire_sprite.png");
            sprite.SetSpriteSize(215, 300);
            sprite.SetSize(90, 90);
            sprite.AddAnimation(new AnimationInfo("fire", 0, 12, 0.075));
            sprite.PlayAnimation("fire");
            sprite.SetAnimationType(AnimationType.LoopReversed);
            sprite.SetZIndex(1);
        }


        public override void Update()
        {
            //follow player
            var player = Find("Player");
            if (player != null)
            {
                gameObject.transform.position = player.transform.position - new Vec2D(70, 0).Rotate(player.transform.rotation);
                gameObject.transform.rotation = player.transform.rotation - 90;
            }
        }
    }
}
