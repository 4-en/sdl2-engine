using SDL2Engine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShootEmUp.Entities
{
    public class TargetingShooter : BaseEnemy
    {

        public static Prototype CreatePrototype()
        {
            var prototype = new Prototype("TargetingShooter");
            var sprite = prototype.AddComponent<SpriteRenderer>();
            sprite.SetTexture("Assets/Textures/spaceshipset32x32/player_ship.png");
            sprite.SetWorldSize(120, 120);
            var collider = prototype.AddComponent<CircleCollider>();
            if (collider != null)
            {
                collider.SetRadius(60);
                collider.IsTrigger = true;
            }
            prototype.AddComponent<TargetingShooter>();
            prototype.AddComponent<Damager>();
            var body = prototype.AddComponent<PhysicsBody>();
            body.RotateWithVelocity = true;

            HealthBar.AddTo(prototype.GameObject, -70);

            return prototype;
        }

        public override void Start()
        {
            base.Start();
            SetMaxHealth(100);
            maxSpeed = 200;
            this.SetPoints(50);
        }

        private double nextMissile = 0;
        public double missileRate = 2;
        public override void Update()
        {
            base.Update();

            if(nextMissile <= Time.time)
            {
                nextMissile = Time.time + missileRate;
                var missile = Prototype.Instantiate("TargetingRocket");

                var body = GetComponent<PhysicsBody>();

                if(missile == null)
                {
                    Console.WriteLine("Could not instantiate missile");
                    return;
                }

                if(body == null)
                {
                    Console.WriteLine("Could not find physics body");
                    return;
                }

                missile.SetPosition(gameObject.GetPosition() + body.Velocity.Normalize() * 300);

                var missileBody = missile.GetComponent<PhysicsBody>();
                if(missileBody == null)
                {
                    Console.WriteLine("Could not find missile physics body");
                    return;
                }

                missileBody.Velocity = body.Velocity.Normalize();

                TargetingRocket? rocket = missile.GetComponent<TargetingRocket>();
                if(rocket != null)
                {
                    rocket.shooter = gameObject;
                    rocket.team = Team.Enemy;
                    rocket.damage = 300;
                }
            }
        }
    }
}
