using SDL2Engine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static ShootEmUp.IngameUI;

namespace ShootEmUp.src.Entities
{
    internal class HealthPowerUp : Script
    {
        public static Prototype CreateHealthPowerUpPrototype()
        {
            var prototype = new Prototype("HealthPowerUp");
            var sprite = prototype.AddComponent<SpriteRenderer>();
            sprite.SetTexture("Assets/Textures/powerups/HP_Bonus.png");
            sprite.SetWorldSize(75, 75);
            var collider = prototype.AddComponent<CircleCollider>();
            if (collider != null)
            {
                collider.SetRadius(35);
                collider.IsTrigger = true;
            }
            prototype.AddComponent<ShieldPowerUp>();
            prototype.AddComponent<DestroyAndHealOnCollision>();
            var body = prototype.AddComponent<PhysicsBody>();
            body.RotateWithVelocity = false;

            return prototype;
        }

        private GameObject? player;
        public override void Start()
        {
            // set direction
            var body = GetComponent<PhysicsBody>();
            if (body != null)
                body.Velocity = new Vec2D(0, 1);

            player = Find("Player");
        }

    }



    class DestroyAndHealOnCollision : Script
    {
        public override void OnCollisionEnter(CollisionPair collision)
        {
            var collisionName = collision.GetOther(this.gameObject).GetName();
            if (collisionName.Equals("Player"))
            {
                Destroy(this.gameObject);
                Console.WriteLine("Health PowerUp Collected");
                Console.WriteLine(Player.currentHealth);
                Console.WriteLine(Player.maxHealth);
                Player.currentHealth = Player.maxHealth;

            }
        }

        public override void Update()
        {
        }

    }
}
