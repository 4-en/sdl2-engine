using SDL2Engine;
using ShootEmUp.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace ShootEmUp
{
    internal class Asteroid : BaseEnemy
    {

        public Asteroid(double health, double damage, int xSpeed, int ySpeed, String texture)
        {
            this.xSpeed = xSpeed;
            this.ySpeed = ySpeed;
            this.asteroidTexture = texture;
            this.position = new Vec2D(0, 0);
            this.velocity = new Vec2D(xSpeed, ySpeed);
        }



        public static Prototype CreateAsteroidPrototype()
        {
            var prototype = new Prototype("Asteroid");
            var sprite = prototype.AddComponent<SpriteRenderer>();
            sprite.SetTexture("Assets/Textures/asteroids/asteroid1.png");
            sprite.SetWorldSize(100, 100);
            var collider = prototype.AddComponent<CircleCollider>();
            if (collider != null)
            {
                collider.SetRadius(50);
                collider.IsTrigger = true;
            }
            var body = prototype.AddComponent<PhysicsBody>();
            prototype.AddComponent<Asteroid>();
            body.RotateWithVelocity = false;

            return prototype;
        }


        public int xSpeed;
        public int ySpeed;
        public String asteroidTexture = "Assets/Textures/asteroids/asteroid1.png";
        //startingpostition
        public Vec2D position;
        public Vec2D velocity;


        public Asteroid()
        {
            asteroidTexture = "Assets/Textures/asteroids/asteroid1.png";
            xSpeed = 0;
            ySpeed = 0;
        }



        public void SetTexture(String texture)
        {
            this.asteroidTexture = texture;
        }

        public override void Start()
        {
            
        }
        public override void Update()
        {
            //set velocity
            var pb = GetComponent<PhysicsBody>();
            if (pb != null)
            {
                pb.Velocity = new Vec2D(xSpeed,ySpeed);
            }
        }
        //damage on collision
        public override void OnCollisionEnter(CollisionPair collision)
        {
            var collisionName = collision.GetOther(this.gameObject).GetName();
            if (collisionName.Equals("Player"))
            {
                Destroy(this.gameObject);
                Console.WriteLine("Asteroid hit player");
                Player.currentHealth -= 50;

                var p = Find("Player");
                var player = p.GetComponent<Player>();
                var playerPos = gameObject.transform.position;
                if (player != null)
                {
                    playerPos = player.transform.position;
                }

                Vec2D random_offset = new Vec2D(random.NextDouble() * 100 - 50, random.NextDouble() * 100 - 50);
                GameText.CreateAt(playerPos + random_offset, "50", 2, 52, new Color(255, 0, 0, 255));
                OnHealthChange();
            }
        }

        private void OnHealthChange()
        {
            // check if health is <= 0
            if (GetHealth() <= 0)
            {
                EventBus.Dispatch(new EnemyKilledEvent(this));
            }

        }

    }
}


class RotationScript : Script
{

    public override void Update()
    {
        transform.rotation = Time.time * 180 / 3 % 360;
    }
}