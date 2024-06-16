using SDL2Engine;
using ShootEmUp.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace ShootEmUp.src.Entities
{
    public class SinusEnemy : BaseEnemy
    {
        public bool moveRight = true;
        public bool moveUp = true;
        public int yDif = 200;
        public int startingY = 150;

        public static Prototype CreateSinusEnemyPrototype()
        {
            var prototype = new Prototype("SinusEnemy");
            var sprite = prototype.AddComponent<SpriteRenderer>();
            sprite.SetTexture("Assets/Textures/spaceships/spaceship6.png");
            sprite.SetWorldSize(85, 85);
            var collider = prototype.AddComponent<CircleCollider>();
            if (collider != null)
            {
                collider.SetRadius(50);
                collider.IsTrigger = true;
            }
            var body = prototype.AddComponent<PhysicsBody>();
            prototype.AddComponent<SinusEnemy>();
            body.RotateWithVelocity = false;

            return prototype;
        }

        public override void Update()
        {
            Time.sinusEnemyTime += Time.deltaTime;

            double xVelocity = moveRight ? 300 : -300;

            double verticalOffset = Math.Sin(Time.time * 2) * yDif;
            double yVelocity = (verticalOffset - (gameObject.transform.position.y - startingY)) / Time.deltaTime;

            Vec2D newVelocity = new Vec2D(xVelocity, yVelocity);
            gameObject.GetComponent<PhysicsBody>()!.Velocity = newVelocity;

            if (moveRight && gameObject.transform.position.x > 2500)
            {
                moveRight = false;
            }
            else if (!moveRight && gameObject.transform.position.x < -2500)
            {
                moveRight = true;
            }

            transform.rotation = newVelocity.getRotation();


            //shot
            if (Time.sinusEnemyTime > 1)
            {

                //projectile
                Time.sinusEnemyTime = 0;
                var projectile = new GameObject("Projectile");
                projectile.AddComponent<ProjectileScript>();
                projectile.transform.position = gameObject.transform.position + new Vec2D(100, 0).Rotate(gameObject.transform.rotation); ;
                projectile.transform.rotation = gameObject.transform.rotation;
                var pb = projectile.AddComponent<PhysicsBody>();
                pb.Velocity = new Vec2D(600, 0).Rotate(gameObject.transform.rotation);
                var spriteRenderer = projectile.AddComponent<SpriteRenderer>();
                if (spriteRenderer != null)
                {
                    spriteRenderer.SetTexture("Assets/Textures/projectile_sprite_sheet_blue.png");
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
}
