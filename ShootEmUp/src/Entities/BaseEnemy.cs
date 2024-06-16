using Newtonsoft.Json;
using SDL2Engine;
using ShootEmUp.Entities;
using System.Runtime.CompilerServices;

namespace ShootEmUp
{

    public class BaseEnemy : Script, IDamageable, IEnemy
    {
        public static Prototype CreateBasePrototype()
        {
            var prototype = new Prototype("BaseEnemy");
            var sprite = prototype.AddComponent<SpriteRenderer>();
            sprite.SetTexture("Assets/Textures/spaceshipset32x32/enemy_1.png");
            sprite.SetWorldSize(200, 200);
            var collider = prototype.AddComponent<CircleCollider>();
            if(collider != null)
            {
                collider.SetRadius(100);
                collider.IsTrigger = true;
            }
            prototype.AddComponent<BaseEnemy>();
            prototype.AddComponent<Damager>();
            var body = prototype.AddComponent<PhysicsBody>();
            body.RotateWithVelocity = true;

            HealthBar.AddTo(prototype.GameObject, -100);

            return prototype;
        }

        private GameObject? player;
        [JsonProperty]
        public double speed = 500.0;
        [JsonProperty]
        public double maxSpeed = 500.0;
        [JsonProperty]
        private double health = 10;
        [JsonProperty]
        private double maxHealth = 10;
        [JsonProperty]
        private double points = 10;
        public override void Start()
        {
            // set direction
            var body = GetComponent<PhysicsBody>();
            if (body != null)
            {
                if (body.Velocity == Vec2D.Zero)
                {
                    body.Velocity = new Vec2D(random.Next(-1, 1), random.Next(-1, 1)).Normalize() * speed;
                }
            }

            player = Find("Player");
        }

        public override void Update()
        {

            if (player != null)
            {
                //TrackPlayer();
                //return;
            }

            
            var body = GetComponent<PhysicsBody>();
            if (body != null)
            {
                if(speed<maxSpeed)
                {
                    speed += 100 * Time.deltaTime;
                } else if(speed>maxSpeed)
                {
                    speed = maxSpeed;
                }
                body.Velocity = body.Velocity.Normalize() * speed;
            }
            
        }

        private void TrackPlayer()
        {
            if (player == null)
            {
                return;
            }
            var playerPosition = player.GetPosition();
            var position = gameObject.GetPosition();
            var direction = (playerPosition - position).Normalize();
            GetComponent<PhysicsBody>()?.SetVelocity(direction * speed);
        }

        private void OnHealthChange()
        {
            // check if health is <= 0
            if (GetHealth() <= 0)
            {
                EventBus.Dispatch(new EnemyKilledEvent(this));
                gameObject.Destroy();

                Effects.ExplosionParticles(gameObject.GetPosition(), 100);
            }

        }

        public void Damage(Damage damage)
        {
            health -= damage.Value;
            OnHealthChange();

            speed = speed * 0.8;
        }

        public void Heal(double value)
        {
            this.health += value;
            if (health > maxHealth)
            {
                health = maxHealth;
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
            OnHealthChange();
        }

        public void SetMaxHealth(double value)
        {
            if (value < health)
            {
                health = value;
                maxHealth = value;
            } else if (health == maxHealth)
            {
                maxHealth = value;
                health = value;
            } else
            {
                maxHealth = value;
            }
            OnHealthChange();
        }

        public int GetPoints()
        {
            return (int)points;
        }

        public Team GetTeam()
        {
            return Team.Enemy;
        }

        public void SetPoints(double value)
        {
            points = value;
        }
    }
}
