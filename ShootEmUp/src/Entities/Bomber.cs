using SDL2Engine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShootEmUp.Entities
{
    internal class Bomber : BaseEnemy
    {

        public static Prototype CreatePrototype()
        {
            var prototype = new Prototype("Bomber");
            var sprite = prototype.AddComponent<SpriteRenderer>();
            sprite.SetTexture("Assets/Textures/spaceshipset32x32/enemy_2.png");
            sprite.SetWorldSize(160, 160);
            var collider = prototype.AddComponent<CircleCollider>();
            if (collider != null)
            {
                collider.SetRadius(80);
                collider.IsTrigger = true;
            }
            prototype.AddComponent<TargetingShooter>();
            prototype.AddComponent<Damager>();
            var body = prototype.AddComponent<PhysicsBody>();
            body.RotateWithVelocity = true;

            HealthBar.AddTo(prototype.GameObject, -80);

            prototype.AddComponent<Bomber>();

            return prototype;
        }

        public double bombInterval = 5;
        public double bombTimer = 0;

        public override void Start()
        {
            base.Start();

            this.SetPoints(100);

            this.SetMaxHealth(500);
        }
        public override void Update()
        {
            base.Update();

            bombTimer += Time.deltaTime;
            if (bombTimer >= bombInterval)
            {
                bombTimer = 0;
                var bomb = Bomb.CreateBomb(gameObject.transform.position, Team.Enemy, gameObject, 500, 300, 2, 0.2);
            }

        }
    }
}
