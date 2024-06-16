using SDL2Engine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShootEmUp.Entities
{
    public class TargetingRocket : ProjectileScript
    {

        public static Prototype CreatePrototype()
        {
            var prototype = new Prototype("TargetingRocket");
            var sprite = prototype.AddComponent<SpriteRenderer>();
            sprite.SetTexture("Assets/Textures/projectiles/rocket.png");
            sprite.SetWorldSize(24, 72);
            var collider = prototype.AddComponent<CircleCollider>();
            collider.SetRadius(25);
            collider.IsTrigger = true;

            prototype.AddComponent<TargetingRocket>();
            var body = prototype.AddComponent<PhysicsBody>();
            body.RotateWithVelocity = true;

            return prototype;
        }

        public GameObject? target = null;
        private bool targetSearchStartet = false;
        public double rotationSpeed = 100;
        public double speed = 500;
        private bool directionLocked = false;


        private IEnumerator FindTarget()
        {

            var targets = FindComponents<IDamageable>();

            Team targetTeam = Team.Enemy;
            if(team == Team.Enemy)
            {
                targetTeam = Team.Player;
            }

            var myPos = gameObject.GetPosition();
            var myDir = GetComponent<PhysicsBody>()?.Velocity ?? new Vec2D(0, 0);
            if(myDir.Length() == 0)
            {
                myDir = new Vec2D(1, 0);
            }

            double bestAngle = 999;
            GameObject? targetObj = null;
            foreach (var target in targets)
            {
                // only one target calculation per frame to balance performance
                // yield return null;
                if(target.GetTeam() != targetTeam)
                {
                    continue;
                }

                var component = target as Component;
                if(component == null)
                {
                    continue;
                }

                var targetPos = component.GetGameObject().GetPosition();

                var dir = targetPos - myPos;
                var angle = myDir.AngleTo(dir);
                angle = Math.Abs(angle);

                if(angle < bestAngle)
                {
                    bestAngle = angle;
                    targetObj = component.GetGameObject();
                }

                
            }

            this.target = targetObj;

            yield return 3.0;
            // 3s cooldown before direction is locked and projectile stops following target
            this.directionLocked = true;

        }

        public override void Start()
        {
            base.Start();

            this.damage = 500;

            if (target == null && !targetSearchStartet)
            {
                StartCoroutine(FindTarget());
                targetSearchStartet = true;
            }
            
        }

        private void AdjustDirection()
        {
            if(target == null || directionLocked)
            {
                return;
            }

            var myPos = gameObject.GetPosition();
            var targetPos = target.GetPosition();

            var targetDir = targetPos - myPos;
            var myDir = GetComponent<PhysicsBody>()?.Velocity ?? new Vec2D(0, 0);

            double optimalAngleDiff = myDir.AngleTo(targetDir);
            double rotation = rotationSpeed * Time.deltaTime;

            if(optimalAngleDiff > 0)
            {
                rotation = -Math.Min(rotation, optimalAngleDiff);
            }
            else
            {
                rotation = -Math.Max(-rotation, optimalAngleDiff);
            }

            myDir = myDir.Rotate(rotation);
            myDir = myDir.Normalize() * speed;
            
            var physicsBody = GetComponent<PhysicsBody>();
            if(physicsBody != null)
            {
                physicsBody.Velocity = myDir;
            }
            
        }

        public override void Update()
        {
            base.Update();

            if (target == null || directionLocked)
            {
                var physicsBody = GetComponent<PhysicsBody>();
                if (physicsBody != null)
                {
                    physicsBody.Velocity = physicsBody.Velocity.Normalize() * speed;
                }
                return;
            }

            AdjustDirection();
        }

    }
}
