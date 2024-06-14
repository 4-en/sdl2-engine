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
    internal class Asteroid : Script
    {
       
        public double speed;
        public String asteroidTexture= "Assets/Textures/asteroids/asteroid1.png";
        //startingpostition
        public Vec2D position;
        public Vec2D velocity;


        public Asteroid()
        {
            speed = 10;
            asteroidTexture = "Assets/Textures/asteroids/asteroid1.png";
        }

        public Asteroid(double health, double damage, double speed, String texture)
        {
            this.speed = speed;
            this.asteroidTexture = texture;
            this.position = new Vec2D(0, 0);
            this.velocity = new Vec2D(0, 0);
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
            pb.Velocity= velocity;
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