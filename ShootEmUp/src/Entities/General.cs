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
    }

    public class ProjectileScript : Script
    {
        public bool destroyOnCollision = true;
        public bool destroyOnScreenExit = true;
        public bool destroyOnLifetimeEnd = true;

        public double lifetime = 5;
        public double damage = 1;
        public Team team = Team.Enemy;
        public GameObject? shooter;

        public override void Update()
        {
            if (destroyOnLifetimeEnd)
            {
                lifetime -= Time.deltaTime;
                if (lifetime <= 0)
                {
                    //gameObject.Destroy();
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
                //gameObject.Destroy();
            }

        }

        public override void OnCollisionEnter(CollisionPair collision)
        {
            var other = collision.GetOther(gameObject);
            gameObject.GetComponent<PhysicsBody>().Velocity = new Vec2D(0, 0);
            gameObject.GetComponent<SpriteRenderer>().SetSource("Assets/Textures/projectile_explosion_sprite_sheet.png");


            if (destroyOnCollision)
            {
                // TODO: Add explosion effect and sound
                StartCoroutine(DestroyAfterTime(0.5));
            }

            var damageable = other.GetComponent<IDamageable>();
            if (damageable != null)
            {
                damageable.Damage(new Damage(damage, shooter, team));
            }
        }

        public IEnumerator DestroyAfterTime(double delay)
        {
            Console.WriteLine("Starting coroutine");
            yield return delay;
            gameObject.Destroy();
            yield break;
        }
    }

    
}