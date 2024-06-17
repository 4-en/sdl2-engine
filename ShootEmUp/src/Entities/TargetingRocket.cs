using SDL2Engine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShootEmUp.Entities
{
    public class TargetingRocket : ProjectileScript, IDamageable, IEnemy
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
        public double rotationSpeed = 2;
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

            var CalculateTargetValue = (double angle, double dist) =>
            {
                return (Math.Abs(angle / 100) + 1) * dist;
            };

            double bestTargetValue = 999999;
            GameObject? targetObj = null;
            foreach (var target in targets)
            {
                // only one target calculation per frame to balance performance
                // yield return null;
                if(target.GetTeam() != targetTeam)
                {
                    continue;
                }

                if(!(target is BaseEnemy || target is Player))
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
                var angle = myDir.GetRotation() - dir.GetRotation();
                angle = Math.Abs(angle);

                if(angle > 200) continue;

                double dist = dir.Length();

                double targetValue = CalculateTargetValue(angle, dist);

                if(targetValue < bestTargetValue)
                {
                    bestTargetValue = targetValue;
                    targetObj = component.GetGameObject();
                }

                
            }

            if(targetObj == null)
            {
                yield return 1.0;
                if(this.scene != null)
                    StartCoroutine(FindTarget());
                yield break;
            }

            this.target = targetObj;

            yield return 3.0;
            // 3s cooldown before direction is locked and projectile stops following target
            this.directionLocked = true;

        }


        public override void Start()
        {
            base.Start();


            if (target == null && !targetSearchStartet)
            {
                StartCoroutine(FindTarget());
                targetSearchStartet = true;
            }
            
        }

        private double NormalizeAngle(double angle)
        {
            while (angle > Math.PI) angle -= 2 * Math.PI;
            while (angle < -Math.PI) angle += 2 * Math.PI;
            return angle;
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
            var targetDistanceSqrt = targetDir.LengthSquared();

            if(targetDistanceSqrt < 300)
            {
                target = null;
                return;
            }

            var myDir = new Vec2D(0, 0);
            var physicsBody = GetComponent<PhysicsBody>();
            if(physicsBody != null)
            {
                myDir = physicsBody.Velocity.Normalize();
            }

            if(myDir.Length() == 0)
            {
                myDir = targetDir;
            }

            // TODO: sometimes rotation is wrong/opposite
            // fix later

            double optimalRotation = targetDir.Normalize().GetRotationRadians() - myDir.Normalize().GetRotationRadians();
            optimalRotation = NormalizeAngle(optimalRotation);
            double rotation = rotationSpeed * Time.deltaTime;

            if(optimalRotation > 0)
            {
                rotation = Math.Min(rotation, optimalRotation);
            }
            else
            {
                rotation = Math.Max(-rotation, optimalRotation);
            }

            myDir = myDir.RotateRadians(rotation);
            myDir = myDir.Normalize() * speed;
            
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

        public void Damage(Damage damage)
        {
            EventBus.Dispatch(new EnemyKilledEvent(this));
            gameObject.Destroy();
        }

        public void Heal(double value)
        {
            
        }

        public double GetHealth()
        {
            return 1;
        }

        public double GetMaxHealth()
        {
            return 1;
        }

        public void SetHealth(double value)
        {
        }

        public void SetMaxHealth(double value)
        {
        }

        public Team GetTeam()
        {
            return team;
        }
    }
}
