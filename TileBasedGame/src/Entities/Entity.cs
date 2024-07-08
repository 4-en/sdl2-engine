using Newtonsoft.Json;
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
        protected double maxSpeed = 100;
        [JsonProperty]
        protected double acceleration = 100;
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

        public override void Start()
        {
            player = Find("Player");
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
