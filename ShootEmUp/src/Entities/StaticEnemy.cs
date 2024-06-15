using SDL2Engine;
using ShootEmUp.Entities;

namespace ShootEmUp
{
    public class StaticEnemy : BaseEnemy
    {

        public static Prototype CreateStaticEnemyPrototype()
        {
            var prototype = new Prototype("StaticEnemy");
            var sprite = prototype.AddComponent<SpriteRenderer>();
            sprite.SetTexture("Assets/Textures/enemy5.png");
            sprite.SetWorldSize(325, 325);
            var collider = prototype.AddComponent<CircleCollider>();
            if (collider != null)
            {
                collider.SetRadius(100);
                collider.IsTrigger = true;
            }
            var body = prototype.AddComponent<PhysicsBody>();
            prototype.AddComponent<StaticEnemy>();
            body.RotateWithVelocity = false;

            return prototype;
        }

        public override void Update()
        {

            Time.staticEnemyShootTime += Time.deltaTime;
            if (Time.staticEnemyShootTime > 2)
            {

                //right projectile
                Time.staticEnemyShootTime = 0;
                var projectileRight = new GameObject("Projectile");
                projectileRight.AddComponent<ProjectileScript>();
                projectileRight.transform.position = gameObject.transform.position + new Vec2D(135, 0);
                var pb = projectileRight.AddComponent<PhysicsBody>();
                pb.Velocity = new Vec2D(250, 0);
                var spriteRendererRight = projectileRight.AddComponent<SpriteRenderer>();
                if (spriteRendererRight != null)
                {
                    spriteRendererRight.SetTexture("Assets/Textures/projectile_sprite_sheet_blue.png");
                    spriteRendererRight.SetSpriteSize(400, 400);
                    spriteRendererRight.SetSize(60, 60);
                    spriteRendererRight.AddAnimation(new AnimationInfo("projectile", 0, 4, 0.075));
                    spriteRendererRight.PlayAnimation("projectile");
                    spriteRendererRight.SetAnimationType(AnimationType.LoopReversed);
                }
                BoxCollider.FromDrawableRect(projectileRight);

                //left projectile
                var projectileLeft = new GameObject("Projectile");
                projectileLeft.AddComponent<ProjectileScript>();
                projectileLeft.transform.position = gameObject.transform.position + new Vec2D(-135, 0);
                projectileLeft.transform.rotation = 180;
                var pbLeft = projectileLeft.AddComponent<PhysicsBody>();
                pbLeft.Velocity = new Vec2D(-250, 0);
                var spriteRendererLeft = projectileLeft.AddComponent<SpriteRenderer>();
                if (spriteRendererLeft != null)
                {
                    spriteRendererLeft.SetTexture("Assets/Textures/projectile_sprite_sheet_blue.png");
                    spriteRendererLeft.SetSpriteSize(400, 400);
                    spriteRendererLeft.SetSize(60, 60);
                    spriteRendererLeft.AddAnimation(new AnimationInfo("projectile", 0, 4, 0.075));
                    spriteRendererLeft.PlayAnimation("projectile");
                    spriteRendererLeft.SetAnimationType(AnimationType.LoopReversed);
                }
                BoxCollider.FromDrawableRect(projectileLeft);


            }   

        }
    }
}