using SDL2Engine;
using ShootEmUp.Entities;

namespace ShootEmUp
{
    public class BaseEnemy : Script, IDamageable
    {
        public static Prototype CreateBasePrototype()
        {
            var prototype = new Prototype("BaseEnemy");
            var sprite = prototype.AddComponent<SpriteRenderer>();
            sprite.SetTexture("Assets/Textures/spaceshipset32x32/enemy_1.png");
            sprite.SetWorldSize(32, 32);
            BoxCollider.FromDrawableRect(prototype.GameObject);
            prototype.AddComponent<BaseEnemy>();
            prototype.AddComponent<PhysicsBody>();

            return prototype;
        }

        public override void Start()
        {
            // set direction
            var body = GetComponent<PhysicsBody>();
            if (body != null)
                body.Velocity = new Vec2D(0, 1);
        }

        public double speed = 1.0;
        public override void Update()
        {
            
        }

        public double health = 100;
        public void Damage(Damage damage)
        {
            health -= damage.Value;
            if (health <= 0)
            {
                Destroy();
            }
        }

        public void Heal(double value)
        {
            throw new NotImplementedException();
        }
    }
}
