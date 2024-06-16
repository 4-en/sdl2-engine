using SDL2Engine;

namespace ShootEmUp.Entities
{
    public class MultiShot : BaseEnemy
    {

        public static Prototype CreatePrototype()
        {
            var prototype = new Prototype("MultiShot");
            var sprite = prototype.AddComponent<SpriteRenderer>();
            sprite.SetTexture("Assets/Textures/spaceships/spaceship4.png");
            sprite.SetWorldSize(160, 160);
            var collider = prototype.AddComponent<CircleCollider>();
            if (collider != null)
            {
                collider.SetRadius(80);
                collider.IsTrigger = true;
            }
            prototype.AddComponent<MultiShot>();
            var damager = prototype.AddComponent<Damager>();
            damager.damage = 1000;
            var body = prototype.AddComponent<PhysicsBody>();
            body.RotateWithVelocity = true;

            HealthBar.AddTo(prototype.GameObject, -70);

            return prototype;
        }

        private GameObject? player = null;

        public override void Start()
        {
            base.Start();
            SetMaxHealth(500);
            maxSpeed = 400;
            this.SetPoints(100);

            player = Find("Player");
        }

        private double nextMissile = 0;
        public double missileRate = 2;
        public int projectileCount = 10;

        private void ShootProjectile()
        {
            if (player == null)
            {
                return;
            }

            double distance = (player.GetPosition() - gameObject.GetPosition()).Length();

            if (distance > 800)
            {
                return;
            }

            var targetPosition = player.GetPosition();
            var direction = (targetPosition - gameObject.GetPosition()).Normalize();
            var angle = direction.GetRotation();
            var startAngle = angle - Math.PI / 6;
            var angleRange = Math.PI / 3;
            var angleStep = angleRange / (projectileCount - 1);

            //projectile
            for(int i = 0; i < projectileCount; i++) {
                var projectile = new GameObject("Projectile");
                var projScript = projectile.AddComponent<ProjectileScript>();
                projScript.damage = 200;

                Vec2D directionVec = new Vec2D(i * angleStep + startAngle);

                projectile.transform.position = gameObject.transform.position + directionVec * 50;
                projectile.transform.rotation = directionVec.GetRotation();

                var pb = projectile.AddComponent<PhysicsBody>();
                pb.Velocity = directionVec * 800;
                var spriteRenderer = projectile.AddComponent<SpriteRenderer>();
                if (spriteRenderer != null)
                {
                    spriteRenderer.SetTexture("Assets/Textures/projectile_sprite_sheet_blue.png");
                    spriteRenderer.SetSpriteSize(400, 400);
                    spriteRenderer.SetSize(100, 100);
                    spriteRenderer.AddAnimation(new AnimationInfo("projectile", 0, 4, 0.075));
                    spriteRenderer.PlayAnimation("projectile");
                    spriteRenderer.SetAnimationType(AnimationType.LoopReversed);
                }
                BoxCollider.FromDrawableRect(projectile);
            }
        }

        Vec2D targetPos = new Vec2D(0, 0);

        public override void Update()
        {
            base.Update();


            if (nextMissile <= Time.time)
            {
                nextMissile = Time.time + missileRate;
                ShootProjectile();
            }

            var pos = gameObject.GetPosition();
            double distance = (targetPos - pos).Length();
            if(distance < 100)
            {
                targetPos = new Vec2D(random.Next(-2000, 2000), random.Next(-2000, 2000));
            }

            var direction = (targetPos - pos).Normalize();
            var body = GetComponent<PhysicsBody>();
            if (body != null)
            {
                body.Velocity = direction * speed;
            }
        }
    }
}
