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

        Team GetTeam()
        {
            return Team.Enemy;
        }
    }

    public class Damager : Script
    {
        public double damage = 300;
        public Team team = Team.Enemy;
        public bool destroyOnCollision = true;

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

            if (destroyOnCollision)
            {
                gameObject.Destroy();
            }
        }
    }

    public class ProjectileScript : Script
    {
        public bool destroyOnCollision = true;
        public bool destroyOnScreenExit = true;
        public bool destroyOnLifetimeEnd = true;

        public double lifetime = 10;
        public double damage = 100;
        public Team team = Team.Enemy;
        public GameObject? shooter;
        private Collider? collider = null;
        private PhysicsBody? physicsBody = null;
        private bool hasCollided = false;
        private ulong collisionFrame = 0;

        SoundPlayer? shotSound = null;
        SoundPlayer? hitSound = null;
        // SpriteRenderer? spriteExplosionRenderer = null;


        public override void Start()
        {

            shotSound = AddComponent<SoundPlayer>();
            if (team == Team.Player)
            {
                shotSound?.Load("Assets/Audio/shot.wav");
                shotSound?.SetVolume(0.2);
                shotSound?.Play();
            }



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
            if (!destroyOnScreenExit)
            {
                return;
            }
            var camera = GetCamera();
            if (camera == null)
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

            if (hasCollided && collisionFrame != Time.tick)
            {
                return;
            }

            var other = collision.GetOther(gameObject);

            var damageable = other.GetComponent<IDamageable>();
            if (damageable == null)
            {
                return;
            }

            if (shooter == other)
            {
                return;
            }

            if (damageable.GetTeam() == team)
            {
                return;
            }

            if (physicsBody != null)
            {
                physicsBody.Velocity = Vec2D.Zero;
            }

            //set explosion texture
            String currentTexture = gameObject.GetComponent<SpriteRenderer>()?.GetTexture() ?? "";
            if (currentTexture.Equals("Assets/Textures/projectile_sprite_sheet.png"))
            {
                gameObject.GetComponent<SpriteRenderer>()?.LoadTexture("Assets/Textures/projectile_explosion_sprite_sheet.png");
            }
            else
            {

                gameObject.GetComponent<SpriteRenderer>()?.LoadTexture("Assets/Textures/projectile_explosion_sprite_sheet_blue.png");
            }
            //play hit sound
            if (collision.GetOther(gameObject).GetName().Contains("Projectile") || this.gameObject.GetName().Contains("Projectile"))
            {

                hitSound = AddComponent<SoundPlayer>();
                hitSound?.Load("Assets/Audio/hit.wav");
                hitSound?.SetVolume(0.2);
                hitSound?.Play();

                if (collision.GetOther(gameObject).GetComponent<BaseEnemy>() != null)
                {
                    var fire = new GameObject("ExplosionAnimation");
                    var sprite = fire.AddComponent<SpriteRenderer>();
                    sprite.SetTexture("Assets\\Textures\\explosions\\YellowExplosion.png");
                    sprite.SetSpriteSize(32, 32);
                    sprite.SetSize(128, 128);
                    sprite.AddAnimation(new AnimationInfo("fire", 0, 4, 0.075));
                    sprite.PlayAnimation("fire");
                    sprite.SetAnimationType(AnimationType.OnceAndDestroy);
                    sprite.SetZIndex(-10);
                    fire.SetPosition(other.GetPosition());
                }

            }

            hasCollided = true;
            collisionFrame = Time.tick;

            if (destroyOnCollision)
            {
                // TODO: Add explosion effect and sound
                StartCoroutine(DestroyAfterTime(0.5));
            }

            damageable.Damage(new Damage(damage, shooter, team));

            Vec2D random_offset = new Vec2D(random.NextDouble() * 100 - 50, random.NextDouble() * 100 - 50);

            GameText.CreateAt(other.GetPosition() + random_offset, damage.ToString(), 2, 52, new Color(255, 0, 0, 255));
        }

        public IEnumerator DestroyAfterTime(double delay)
        {
            yield return delay;
            gameObject.Destroy();
            yield break;
        }
    }

}