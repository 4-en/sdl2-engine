using SDL2Engine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static ShootEmUp.IngameUI;

namespace ShootEmUp.src.Entities
{
    internal class ShieldPowerUp : Script
    {
        public static Prototype CreateShieldPowerUpPrototype()
        {
            var prototype = new Prototype("ShieldPowerUp");
            var sprite = prototype.AddComponent<SpriteRenderer>();
            sprite.SetTexture("Assets/Textures/powerups/Armor_Bonus.png");
            sprite.SetWorldSize(75, 75);
            var collider = prototype.AddComponent<CircleCollider>();
            if (collider != null)
            {
                collider.SetRadius(35);
                collider.IsTrigger = true;
            }
            prototype.AddComponent<ShieldPowerUp>();
            prototype.AddComponent<DestroyAndCreateShieldOnCollision>();
            var body = prototype.AddComponent<PhysicsBody>();
            body.RotateWithVelocity = false;

            return prototype;
        }

        private GameObject? player;
        public override void Start()
        {
            player = Find("Player");
        }

    }



    class DestroyAndCreateShieldOnCollision : Script
    {
        public override void OnCollisionEnter(CollisionPair collision)
        {
            var collisionName = collision.GetOther(this.gameObject).GetName();
            if (collisionName.Equals("Player"))
            {
                Destroy(this.gameObject);
                Console.WriteLine("Shield PowerUp Collected");
                HealthBar.backgroundColor = new SDL2Engine.Color(0, 0, 255, 255);

            }
        }

        public override void Update()
        {
        }

    }
}
