using SDL2Engine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShootEmUp.Entities
{
    internal class TargetingShooter : BaseEnemy
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
    }
}
