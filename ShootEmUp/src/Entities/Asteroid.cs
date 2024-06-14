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
            ySpeed = 50;
        }



        public void SetTexture(String texture)
        {
            this.asteroidTexture = texture;
        }

        public override void Start()
        {
            var asteroid = new GameObject("Asteroid");
            var texture = asteroid.AddComponent<TextureRenderer>();
            texture.SetSource(asteroidTexture);
            var pb = asteroid.AddComponent<PhysicsBody>();
            BoxCollider.FromDrawableRect(asteroid);
            pb.Velocity = new Vec2D(xSpeed, ySpeed);
            asteroid.transform.position = position;
            asteroid.AddComponent<RotationScript>();
            asteroid.AddComponent<ProjectileScript>();
        }
        public override void Update()
        {
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