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

        public static int speed;
        public static int maxSpeed;
        public static int minSpeed;
        public static int acceleration;
        public static double rotationSpeed;
        public int health;
        public int damage;
        public int fireRate;
        public int fireRange;
        public int shield;
        
       
        public Player()
        {
            speed = 400;
            maxSpeed = 1000;
            minSpeed = 25;
            acceleration = 1;
            rotationSpeed = 0.5;
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
            texture?.SetSource("Assets/Textures/change_direction_powerup.png");
            BoxCollider.FromDrawableRect(player);
            player.AddComponent<KeyboardController>();
            player.transform.position = new Vec2D(gameBounds.x / 2, gameBounds.y / 2);
            var pb = player.AddComponent<PhysicsBody>();
            pb.Velocity = new Vec2D(100, 0);
        }
        public override void Update() { }
           
            
    }

    public class KeyboardController : Script
    {
        public int left = (int)SDL_Keycode.SDLK_a;
        public int right = (int)SDL_Keycode.SDLK_d;
        public int up = (int)SDL_Keycode.SDLK_w;
        public int down = (int)SDL_Keycode.SDLK_s;
        public int space = (int)SDL_Keycode.SDLK_SPACE;

        



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
                //ShootProjectile();

            }
            if (Input.GetKeyPressed(up))
            {
                Player.speed = Player.speed < Player.maxSpeed ? Player.speed + Player.acceleration : Player.speed;
            }
            if (Input.GetKeyPressed(down))
            {
                Player.speed = Player.speed > 0 && Player.speed > Player.minSpeed ? Player.speed - Player.acceleration : Player.speed;
            }
            //rotate velocity
            gameObject.GetComponent<PhysicsBody>().Velocity = new Vec2D(Player.speed, 0).Rotate(gameObject.transform.rotation);

        }
        /*
        private void ShootProjectile()
        {
            var projectile = new GameObject("Projectile");
            projectile.AddComponent<ProjectileScript>();
            //set the position of the projectile to the position of the player
            //add a small offset to the position of the player to avoid collision with the player  
            projectile.transform.position = gameObject.transform.position + new Vec2D(100, 0).Rotate(gameObject.transform.rotation);
            projectile.transform.rotation = gameObject.transform.rotation;
            var pb = projectile.AddComponent<PhysicsBody>();
            pb.Velocity = new Vec2D(800, 0).Rotate(gameObject.transform.rotation);
            projectile.AddComponent<TextureRenderer>().SetSource("Assets/Textures/projectile.png");
            BoxCollider.FromDrawableRect(projectile);
        }*/
    }
    }
