using SDL2Engine;
using ShootEmUp.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static SDL2.SDL;

namespace ShootEmUp
{
    internal class Player : Script
    {
        protected Vec2D gameBounds = new Vec2D(1920, 1080);

        public static String spaceshipTexture = "";

        public static int speed;
        public static int maxSpeed;
        public static int minSpeed;
        public static int acceleration;
        public static double rotationSpeed;
        public static int projectileSpeed;
        public int health;
        public static int damage;
        public int fireRate;
        public int fireRange;
        public int shield;
        
        
       
        public Player()
        {
            
            spaceshipTexture = "Assets/Textures/spaceships/spaceship5.png";
            speed = 400;
            maxSpeed = 1000;
            minSpeed = 25;
            acceleration = 1;
            rotationSpeed = 0.5;
            projectileSpeed = 800;
            health = 100;
            damage = 10;
            fireRate = 10;
            fireRange = 10;
            shield = 10;
            
        }

       
        public override void Start()
        {
            var player = new GameObject("Player");
            var texture = player.AddComponent<TextureRenderer>();
            texture.SetSource(spaceshipTexture);


            player.AddComponent<CameraFollow>();
            BoxCollider.FromDrawableRect(player);
            player.AddComponent<KeyboardController>();
            player.transform.position = new Vec2D(gameBounds.x / 2, gameBounds.y / 2);
            var pb = player.AddComponent<PhysicsBody>();
            pb.Velocity = new Vec2D(Player.speed, 0);
        }
        public override void Update() {
            
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

        



        public override void Start()
        {
            
        }

        public override void Update()
        {

            if (Input.GetKeyPressed(left))
            {
                gameObject.transform.rotation -= Player.rotationSpeed;
            }
            if (Input.GetKeyPressed(right))
            {
                gameObject.transform.rotation += Player.rotationSpeed;
            }
            if (Input.GetKeyDown(space))
            {
                gameObject.AddComponent<Projectile>();
            }
            if (Input.GetKeyPressed(up))
            {
                Player.speed = Player.speed < Player.maxSpeed ? Player.speed + Player.acceleration : Player.speed;
            }
            if (Input.GetKeyPressed(down))
            {
                Player.speed = Player.speed > 0 && Player.speed > Player.minSpeed ? Player.speed - Player.acceleration : Player.speed;
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

        }
        
       

    }

    public class CameraFollow : Script
    {
        Vec2D gameBounds = new Vec2D(1920, 1080);

        public override void Update()
        {
            var camera = GetCamera() as Camera;
            camera?.SetPosition(gameObject.transform.position-gameBounds/2);
        }
    }

    public class Projectile : Script
    {
        public override void Start()
        {
            var projectile = new GameObject("Projectile");
            projectile.AddComponent<ProjectileScript>();
            //set the position of the projectile to the position of the player
            //add a small offset to the position of the player to avoid collision with the player  
            projectile.transform.position = gameObject.transform.position + new Vec2D(75 + Player.speed/10, 0).Rotate(gameObject.transform.rotation);
            projectile.transform.rotation = gameObject.transform.rotation;
            var pb = projectile.AddComponent<PhysicsBody>();
            pb.Velocity = new Vec2D(Player.projectileSpeed, 0).Rotate(gameObject.transform.rotation);
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
}
