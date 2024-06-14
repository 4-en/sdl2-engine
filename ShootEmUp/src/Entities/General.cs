using SDL2Engine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShootEmUp.Entities
{
    public enum Team
    {
        Player,
        Enemy,
        Neutral
    }
    public struct Damage
    {
        public double Value;
        public GameObject? Source;
        public Team Team = Team.Enemy;

        public Damage(double value)
        {
            Value = value;
        }

        public Damage(double value, GameObject? source)
        {
            Value = value;
            Source = source;
        }

        public Damage(double value, GameObject? source, Team team)
        {
            Value = value;
            Source = source;
            Team = team;
        }
    }
    public interface IDamageable
    {
        void Damage(Damage damage);

        void Heal(double value);

        double GetHealth();
        double GetMaxHealth();

        void SetHealth(double value);
        void SetMaxHealth(double value);
    }

    public class ProjectileScript : Script
    {
        public bool destroyOnCollision = true;
        public bool destroyOnScreenExit = true;
        public bool destroyOnLifetimeEnd = true;

        public double lifetime = 5;
        public double damage = 100;
        public Team team = Team.Enemy;
        public GameObject? shooter;
        private Collider? collider = null;
        private PhysicsBody? physicsBody = null;
        private bool hasCollided = false;


        public override void Start()
        {
            collider = gameObject.GetComponent<Collider>();
            if (collider != null)
            {
                collider.IsTrigger = true;
            }

            physicsBody = gameObject.GetComponent<PhysicsBody>();
            if (physicsBody != null)
            {
                // physicsBody.RotateWithVelocity = true;
            }
        }

        public override void Update()
        {
            if (destroyOnLifetimeEnd)
            {
                lifetime -= Time.deltaTime;
                if (lifetime <= 0)
                {

                    gameObject.Destroy();
                }
            }
            if(!destroyOnScreenExit)
            {
                return;
            }
            var camera = GetCamera();
            if(camera == null)
            {
                return;
            }

            var screenPosition = camera.WorldToScreen(gameObject.GetPosition());
            double tolerance = 250;
            if (screenPosition.x < -tolerance || screenPosition.x > camera.GetScreenWidth() + tolerance || screenPosition.y < -tolerance || screenPosition.y > camera.GetScreenHeight() + tolerance)
            {
                gameObject.Destroy();
            }

        }

        public override void OnCollisionEnter(CollisionPair collision)
        {

            if (hasCollided)
            {
                return;
            }

            var other = collision.GetOther(gameObject);
            if (other.GetName().Equals("ShieldPowerUp")) return;

            var damageable = other.GetComponent<IDamageable>();
            if (damageable == null)
            {
                return;
            }

            if (physicsBody != null)
            {
                physicsBody.Velocity = Vec2D.Zero;
            }
            //gameObject.GetComponent<SpriteRenderer>()?.SetSource("Assets/Textures/projectile_explosion_sprite_sheet.png");
            gameObject.GetComponent<SpriteRenderer>()?.LoadTexture("Assets/Textures/projectile_explosion_sprite_sheet.png");


            hasCollided = true;

            if (destroyOnCollision)
            {
                // TODO: Add explosion effect and sound
                StartCoroutine(DestroyAfterTime(0.5));
            }

            damageable.Damage(new Damage(damage, shooter, team));
        }

        public IEnumerator DestroyAfterTime(double delay)
        {
            yield return delay;
            gameObject.Destroy();
            yield break;
        }
    }

    
}