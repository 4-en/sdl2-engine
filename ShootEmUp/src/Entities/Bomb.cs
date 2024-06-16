using SDL2Engine;

namespace ShootEmUp.Entities
{
    public class Bomb : Script, IDamageable, IEnemy
    {
        public GameObject? source = null;
        public Team team = Team.Enemy;
        public double damage = 250;
        public double radius = 100;
        public double timer = 2;
        public double explosionDuration = 0.5;
        private bool exploded = false;

        public override void Start()
        {
            Delay(timer, Explode);
        }

        public override void Update()
        {

        }

        private void Explode()
        {
            if(exploded)
            {
                return;
            }

            exploded = true;

            // add hitbox
            var collider = gameObject.AddComponent<CircleCollider>();
            collider.SetRadius(radius);
            collider.IsTrigger = true;

            gameObject.Destroy(this.explosionDuration);


        }

        public override void OnCollisionEnter(CollisionPair collision)
        {
            var other = collision.GetOther(gameObject);
            var damageable = other.GetComponent<IDamageable>();
            if (damageable == null)
            {
                return;
            }

            damageable.Damage(new Damage(damage, gameObject, team));
        }

        public Team GetTeam()
        {
            return team;
        }

        public void Damage(Damage damage)
        {
            Explode();
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
    }
}
