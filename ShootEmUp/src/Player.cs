﻿using SDL2;
using SDL2Engine;
using ShootEmUp.Entities;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static SDL2.SDL;
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
        public int fireRate;
        public int fireRange;
        public int shield;



        public Player()
        {

            spaceshipTexture = "Assets/Textures/spaceships/spaceship5.png";
            speed = 400;
            maxSpeed = 800 + PlayerData.Instance.SpeedUpgradeLevel * 100;
            minSpeed = 25;
            acceleration = 1;
            rotationSpeed = 0.5;
            projectileSpeed = 800;
            maxHealth = 1000 + PlayerData.Instance.HealthUpgradeLevel * 200;
            currentHealth = maxHealth;
            damage = 50 + PlayerData.Instance.DamageUpgradeLevel * 10;
            fireRate = 10;
            fireRange = 10;
            shield = 10;

        }


        public override void Start()
        {

            var texture = AddComponent<TextureRenderer>();
            texture.SetSource(spaceshipTexture);


            AddComponent<CameraFollow>();
            BoxCollider.FromDrawableRect(gameObject);
            AddComponent<KeyboardController>(); 
            AddComponent<HealthBar>();
            AddComponent<HealthReducer>();
            gameObject.transform.position = new Vec2D(gameBounds.x / 2, gameBounds.y / 2);
            var pb = AddComponent<PhysicsBody>();
        }
        public override void Update()
        {

        }


    }

    internal class HealthReducer : Script
    {
        public override void Update()
        {
           //key inputs
           if (Input.GetKeyPressed((int)SDL_Keycode.SDLK_1))
            {
                if (Player.currentHealth > 0)
                {

                    Player.currentHealth -= 1;
                }
            }
            if (Input.GetKeyPressed((int)SDL_Keycode.SDLK_2))
            {
                if (Player.currentHealth < Player.maxHealth)
                {
                    Player.currentHealth += 1;
                }
            }
            
        }
    }

    internal class HealthBar : Script
    {
        GameObject healthBarBackground = new GameObject("HealthBarBackground");
        GameObject healthBarBorder = new GameObject("HealthBarBorder");
        GameObject healthBarText = new GameObject("HealthBarText");
        int width = 300;
        int height = 60;


        public override void Start()
        {

            
            healthBarBackground.transform.position = new Vec2D(100, 100);
            var healthIndicator = healthBarBackground.AddComponent<TextRenderer>();
            healthIndicator.anchorPoint = AnchorPoint.CenterLeft;
            healthIndicator.SetPreferredSize(new Rect(0,0,width,height));
            healthIndicator.SetColor(SDL2Engine.Color.Green);
            healthIndicator.SetBackgroundColor(new SDL2Engine.Color(255, 0, 0, 255));



            var border = healthBarBorder.AddComponent<TextRenderer>();
            var textRenderHelper = healthBarBorder.AddComponent<TextRenderHelper>();
            healthBarBorder.transform.position = new Vec2D(100, 100);
            border.anchorPoint = AnchorPoint.CenterLeft;
            border.SetPreferredSize(new Rect(0,0,width,height));
            border.SetBorderSize(2);
            border.SetBorderColor(SDL2Engine.Color.Black);

            var text = healthBarText.AddComponent<TextRenderer>();
            healthBarText.transform.position = new Vec2D(100 + width / 2, 100);
            text.anchorPoint = AnchorPoint.Center;
            text.SetText(Player.currentHealth.ToString());
            text.SetColor(SDL2Engine.Color.White);
            text.SetFontSize(38);
            text.SetFontPath("Assets/Fonts/Arcadeclassic.ttf");

            var titleText = new GameObject("TitleText");
            var text2 = titleText.AddComponent<TextRenderer>();
            titleText.transform.position = new Vec2D(100 + width / 2, 50);
            text2.anchorPoint = AnchorPoint.Center;
            text2.SetText("Health");
            text2.SetColor(SDL2Engine.Color.Red);
            text2.SetFontSize(50);
            text2.SetFontPath("Assets/Fonts/Arcadeclassic.ttf");

            
            var heart = new GameObject("Heart");
            var heartTexture = heart.AddComponent<TextureRenderer>();
            heartTexture.relativeToCamera = false;
            heartTexture.SetSource("Assets/Textures/health.png");
            heartTexture.IsVisible(new Rect(4000, 4000));
            heart.transform.position = new Vec2D(110, 95);

        }

        public override void Update()
        {
            double currentHealth = Player.currentHealth;
            double maxHealth = Player.maxHealth;
            double healthBarWidth = currentHealth / maxHealth * width;
            var healthIndicator = healthBarBackground.GetComponent<TextRenderer>();
            healthIndicator?.SetRect(new Rect(0, 0, healthBarWidth, height));
            Console.WriteLine(healthBarWidth);

            //update the text
            var text = healthBarText.GetComponent<TextRenderer>();
            text?.SetText(Player.currentHealth.ToString());

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
            //Console.WriteLine(cameraPosition.ToString());

            // Calculate the new camera position
            Vec2D newCameraPosition = cameraPosition;

            // Update the camera position on the X-axis if the player is within the world bounds on the X-axis
            if (playerPosition.x >= 0 && playerPosition.x <= WorldSize.x)
            {
                newCameraPosition.x = playerPosition.x - gameBounds.x / 2;
            }

            // Update the camera position on the Y-axis if the player is within the world bounds on the Y-axis
            if (playerPosition.y >= 0 && playerPosition.y <= WorldSize.y)
            {
                newCameraPosition.y = playerPosition.y - gameBounds.y / 2;
            }

            // Set the new camera position
            camera?.SetPosition(newCameraPosition);
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
