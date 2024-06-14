﻿using SDL2;
using SDL2Engine;
using ShootEmUp.Entities;
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
        public static int maxHealth;
        public static int currentHealth;
        public static int damage;
        public static int displayedHighscore;
        public static int displayedMoney;
        public int fireRate;
        public int fireRange;
        public int shield;



        public Player()
        {

            spaceshipTexture = "Assets/Textures/spaceships/spaceship5.png";
            speed = 0;
            maxSpeed = 800 + PlayerData.Instance.SpeedUpgradeLevel * 100;
            minSpeed = 25;
            acceleration = 1;
            rotationSpeed = 0.5;
            projectileSpeed = 800;
            maxHealth = 1000 + PlayerData.Instance.HealthUpgradeLevel * 200;
            currentHealth = maxHealth;
            damage = 50 + PlayerData.Instance.DamageUpgradeLevel * 10;
            displayedHighscore = 0;
            displayedMoney = 0;
            fireRate = 10;
            fireRange = 10;
            shield = 10;

        }


        public override void Start()
        {

            var texture = AddComponent<TextureRenderer>();
            texture.SetSource(spaceshipTexture);
            texture.SetZIndex(-10);


            AddComponent<CameraFollow>();

            BoxCollider.FromDrawableRect(gameObject);
            AddComponent<KeyboardController>();
            AddComponent<UserInterface>();
            gameObject.transform.position = new Vec2D(gameBounds.x / 2, gameBounds.y / 2);
            var pb = AddComponent<PhysicsBody>();


        }
        public override void Update()
        {

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
        // The size of the game window
        Vec2D gameBounds = new Vec2D(1920, 1080);

        // The size of the world in which the camera should follow the player
        protected Vec2D WorldSize = new Vec2D(2500, 2500);


        public override void Update()
        {


            var camera = GetCamera() as Camera;

            // Get the current position of the camera and the player
            Vec2D cameraPosition = camera.GetPosition();
            Vec2D playerPosition = gameObject.GetPosition();

            // Set the new camera position
            camera?.UpdateCameraPositionToPlayer(playerPosition, gameBounds, WorldSize);



        }
        //public override void Start()
        //{
        //    // collision test object
        //    var obstacle = new GameObject("Obstacle");
        //    var pb = obstacle.AddComponent<PhysicsBody>();
        //    var bc = obstacle.AddComponent<BoxCollider>();
        //    bc.UpdateColliderSize(40, 40);
        //    obstacle.transform.position = new Vec2D((gameBounds.x / 2) - 290, 500);

        //    // collision test object
        //    var obstacle2 = new GameObject("Obstacle");
        //    var pb2 = obstacle2.AddComponent<PhysicsBody>();
        //    var bc2 = obstacle2.AddComponent<BoxCollider>();
        //    bc2.UpdateColliderSize(40, 40);
        //    obstacle2.transform.position = new Vec2D((gameBounds.x / 2) + 290, 500);

        //}
    }




    public class Projectile : Script
    {
        public override void Start()
        {
            var projectile = new GameObject("Projectile");
            projectile.AddComponent<ProjectileScript>();
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
}
