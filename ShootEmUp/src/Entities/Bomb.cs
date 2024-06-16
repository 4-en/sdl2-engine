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
        private SpriteRenderer? sprite;


        public static Bomb CreateBomb(Vec2D position, Team team, GameObject source, double damage = 250, double radius = 100, double timer = 2, double explosionDuration = 0.5)
        {
            var bombObject = new GameObject("Bomb");
            bombObject.transform.position = position;
            var bombComponent = bombObject.AddComponent<Bomb>();
            bombComponent.team = team;
            bombComponent.source = source;
            bombComponent.damage = damage;
            bombComponent.radius = radius;
            bombComponent.timer = timer;
            bombComponent.explosionDuration = explosionDuration;
            return bombComponent;
        }

        public override void Start()
        {
            Delay(timer, Explode);

            // add bomb sprite
            sprite = gameObject.AddComponent<SpriteRenderer>();
            sprite.SetTexture("Assets/Textures/projectiles/bomb.png");
            sprite.SetWorldSize(50, 50);
        }

        public override void Update()
        {

        }

        public void Explode()
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

            Color color = team == Team.Player ? new Color(255, 255, 255, 255) : new Color(255, 0, 0, 255);

            Effects.ExplosionParticles(gameObject.transform.position, 200, color, 3);

            // play explosion sound
            var sound = gameObject.AddComponent<SoundPlayer>();
            sound.Load("Assets/Audio/explosion.mp3");
            sound.playOnAwake = true;

            // delete bomb sprite
            sprite?.Destroy();


        }

        public override void OnCollisionEnter(CollisionPair collision)
        {
            var other = collision.GetOther(gameObject);
            var damageable = other.GetComponent<IDamageable>();
            if (damageable == null)
            {
                return;
            }

            if (damageable.GetTeam() == team)
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
