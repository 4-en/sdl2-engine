﻿using Newtonsoft.Json;
using SDL2Engine;
using TileBasedGame.Entities;

namespace TileBasedGame.Entities
{

    /*
     * Base interface for Entities (Player/Enemies)
     */
    public abstract class Entity : Script, IEnemy, IDamageable
    {
        private GameObject? player;

        [JsonProperty]
        protected double maxSpeed = 80;
        [JsonProperty]
        protected double acceleration = 250;
        [JsonProperty]
        protected double jumpForce = 120;
        [JsonProperty]
        protected double health = 100;
        [JsonProperty]
        protected double maxHealth = 100;
        [JsonProperty]
        protected double damage = 10;
        [JsonProperty]
        protected double attackSpeed = 1;

        protected double attackCooldown = 0;
        [JsonProperty]
        protected Team team = Team.Enemy;
        [JsonProperty]
        protected int points = 10;
        [JsonProperty]
        protected double range = 100;
        [JsonProperty]
        protected bool facingRight = true;
        [JsonProperty]
        protected int maxAirJumps = 1;
        protected int airJumps = 0;

        protected bool isGrounded = false;
        private ulong lastGroundedTime = 0;

        protected PhysicsBody? physicsBody;


        public override void Start()
        {
            player = Find("Player");
            physicsBody = gameObject.GetComponent<PhysicsBody>();
        }

        public override void Update()
        {
            if(lastGroundedTime + 10 < Time.tick)
            {
                isGrounded = false;
            }
        }

        public override void OnCollisionEnter(CollisionPair collision)
        {
            // check for collisions with walls

            var other = collision.GetOther(gameObject);

            IDamageable? damageable = other.GetComponent<IDamageable>();
            if (damageable != null)
            {
                if (damageable.GetTeam() != team && damageable.GetTeam() != Team.Neutral)
                {
                    damageable.Damage(new Damage(damage, gameObject, team));
                }

            }


        }

        public override void OnCollisionStay(CollisionPair collision)
        {
            var other = collision.GetOther(gameObject);
            if (other.GetName() == "Obstacle")
            {
                // check if collisionPoint is below the gameObject
                var collisionPoint = collision.collisionPoint;

                var gameObjectPosition = gameObject.GetPosition();
                double xDistance = collisionPoint.x - gameObjectPosition.x;
                double yDistance = collisionPoint.y - gameObjectPosition.y;
                if (Math.Abs(xDistance) > Math.Abs(yDistance) || true)
                {

                    isGrounded = true;
                    airJumps = 0;
                    lastGroundedTime = Time.tick;
                }
            }
        }

        protected void AddRecoil()
        {
            double recoil = 40;
            if (facingRight)
            {
                recoil = -recoil;
            }

            physicsBody?.AddVelocity(new Vec2D(recoil, 0));
        }

        protected virtual void Shoot()
        {

        }

        private double lastShot = 0;
        protected void TryShoot()
        {
            if(lastShot + 1 / attackSpeed > Time.time)
            {
                return;
            }
            lastShot = Time.time;
            AddRecoil();
            Shoot();
        }

        protected void MoveLeft(double boost=1.0)
        {
            if (physicsBody == null)
            {
                return;
            }

            // check if velocity is less than max speed
            if (physicsBody.Velocity.x > -maxSpeed * boost)
            {
                // if velocity is in the opposite direction, lower it
                if (physicsBody.Velocity.x > 0)
                {
                    physicsBody.Velocity = new Vec2D(physicsBody.Velocity.x / (1 + 10 * Time.deltaTime));
                }

                physicsBody.AddVelocity(new Vec2D(-acceleration * boost * Time.deltaTime, 0));
            }
        }

        protected void MoveRight(double boost=1.0)
        {
            if (physicsBody == null)
            {
                return;
            }

            // check if velocity is less than max speed
            if (physicsBody.Velocity.x < maxSpeed * boost)
            {

                // if velocity is in the opposite direction, lower it
                if (physicsBody.Velocity.x < 0)
                {
                    physicsBody.Velocity = new Vec2D(physicsBody.Velocity.x / (1 + 10 * Time.deltaTime));
                }

                physicsBody.AddVelocity(new Vec2D(acceleration * boost * Time.deltaTime, 0));
            }
        }

        protected void Jump()
        {
            if (physicsBody == null)
            {
                return;
            }

            if (isGrounded || airJumps < maxAirJumps)
            {
                physicsBody.SetVelocity(new Vec2D(physicsBody.Velocity.x, -jumpForce));
                
                if(!isGrounded)
                {
                    airJumps++;
                    Console.WriteLine("Air jumps: " + airJumps);
                }
            }
        }

        protected void Decellerate()
        {
            if (physicsBody == null)
            {
                return;
            }

            if(!isGrounded)
            {
                return;
            }

            double vel = physicsBody.Velocity.x;

            if (vel > 0)
            {
                vel -= 2 * acceleration * Time.deltaTime;
                if (vel < 0)
                {
                    vel = 0;
                }
            }
            else
            {
                vel += 2 * acceleration * Time.deltaTime;
                if (vel > 0)
                {
                    vel = 0;
                }
            }

            physicsBody.SetVelocity(new Vec2D(vel, physicsBody.Velocity.y));
        }

        public Team GetTeam()
        {
            return team;
        }

        public int GetPoints()
        {
            return points;
        }

        public bool IsFacingRight()
        {
            return facingRight;
        }

        protected void OnHealthChange()
        {
            if (team == Team.Player)
            {
                EventBus.Dispatch(new PlayerDamagedEvent(this, new Damage(health)));

                return;
            }

            // check if health is <= 0
            if (GetHealth() <= 0)
            {
                EventBus.Dispatch(new EnemyKilledEvent(this));
                EventBus.Dispatch(new PlayerScoreEvent(points));
                gameObject.Destroy();

                Effects.ExplosionParticles(gameObject.GetPosition(), 20);
            }

        }

        public void Damage(Damage damage)
        {
            this.health -= damage.Value;
            OnHealthChange();
        }

        public void Heal(double value)
        {
            this.health += value;
            if (this.health > maxHealth)
            {
                this.health = maxHealth;
            }

            OnHealthChange();
        }

        public double GetHealth()
        {
            return health;
        }

        public double GetMaxHealth()
        {
            return maxHealth;
        }

        public void SetHealth(double value)
        {
            this.health = value;
            if (this.health > maxHealth)
            {
                this.health = maxHealth;
            }

            OnHealthChange();
        }

        public void SetMaxHealth(double value)
        {
            bool wasAtMaxHealth = health == maxHealth;
            maxHealth = value;
            if (wasAtMaxHealth)
            {
                health = value;
            }
            else if (health > maxHealth)
            {
                health = maxHealth;
            }

            OnHealthChange();
        }
    }
}