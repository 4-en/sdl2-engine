using SDL2Engine;
using System;
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
                    Destroy();
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
                Destroy();
            }

        }

        public override void OnCollisionEnter(CollisionPair collision)
        {
            var other = collision.GetOther(gameObject);


            if (destroyOnCollision)
            {
                // TODO: Add explosion effect and sound
                Destroy();
            }

            var damageable = other.GetComponent<IDamageable>();
            if (damageable != null)
            {
                damageable.Damage(new Damage(damage, shooter, team));
            }
        }


    }


}
