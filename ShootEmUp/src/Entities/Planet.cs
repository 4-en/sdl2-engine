using SDL2Engine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;

namespace ShootEmUp.Entities
{
    internal class Planet : BaseEnemy
    {
        public Planet()
        {
            SetMaxHealth(500);
        }
        public static Prototype CreatePlanetPrototype()
        {
            var prototype = new Prototype("Planet");
            var sprite = prototype.AddComponent<SpriteRenderer>();
            sprite.SetTexture("Assets/Textures/blue_fire_sprite.png");
            sprite.SetWorldSize(80, 80);
            sprite.SetSpriteSize(498, 496);
            sprite.AddAnimation(new AnimationInfo("fireCircle", 0, 13, 0.075));
            sprite.PlayAnimation("fireCircle");
            sprite.SetAnimationType(AnimationType.LoopReversed);
            sprite.SetZIndex(1);
        


        var collider = prototype.AddComponent<CircleCollider>();
            if (collider != null)
            {
                collider.SetRadius(200);
                collider.IsTrigger = true;
            }
            prototype.AddComponent<Planet>();
            var body = prototype.AddComponent<PhysicsBody>();
            body.RotateWithVelocity = true;


            return prototype;
        }

        //deal damage to player if player collides with planet
        public override void OnCollisionEnter(CollisionPair collision)
        {
            var collisionName = collision.GetOther(this.gameObject).GetName();
            if (collisionName.Equals("Player"))
            {
                var p = Find("Player");
                var player = p.GetComponent<Player>();
                var playerPos = gameObject.transform.position;
                if (player != null)
                {
                    playerPos = player.transform.position;
                }

                Player.currentHealth -= 1;
                Vec2D random_offset = new Vec2D(random.NextDouble() * 100 - 50, random.NextDouble() * 100 - 50);
                GameText.CreateAt(playerPos + random_offset, "1", 2, 52, new Color(255, 0, 0, 255));
                OnHealthChange();

                speed = speed * 0.8;
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

        public override void Update()
        {
        }


    }
}
