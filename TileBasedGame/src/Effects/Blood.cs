using SDL2Engine;

namespace TileBasedGame
{
    public static partial class Effects
    {

        class BloodParticle : Script
        {
            public double creationTime = 0;
            private DrawableRect? rect;
            private Vec2D startVelocity = new Vec2D(0, 0);
            private int startRectSize = 0;
            private int currentRectSize = 0;
            private PhysicsBody? body;

            public override void Start()
            {
                creationTime = Time.time;
                rect = GetComponent<FilledRect>();

                if (rect != null)
                {
                    startRectSize = (int)rect.GetRect().w;
                    currentRectSize = startRectSize;
                }

                body = GetComponent<PhysicsBody>();
                if (body != null)
                {
                    startVelocity = body.Velocity;
                }


            }
            private bool collided = false;
            public override void OnCollisionEnter(CollisionPair collision)
            {
                if (collided)
                {
                    return;
                }
                collided = true;
                var myBody = GetComponent<PhysicsBody>();
                if (myBody != null)
                {
                    Destroy(myBody);
                }

                var myCollider = GetComponent<BoxCollider>();
                if (myCollider != null)
                {
                    Destroy(myCollider);
                }

                var other = collision.GetOther(gameObject);

                Vec2D oldPosition = gameObject.GetPosition();
                // add as child to other object
                other.AddChild(gameObject);
                gameObject.SetPosition(oldPosition);
            }

            public override void Update()
            {

            }
        }
        public static void SpawnBlood(Vec2D position, int count = 10, Vec2D? direction = null, Color? color = null)
        {

            if(direction == null || direction == default)
            {
                direction = new Vec2D(0, 0);
            }

            double dirX = direction.Value.x;
            double dirY = direction.Value.y;

            for (int i = 0; i < count; i++)
            {
                var particle = new GameObject("BloodParticle");

                var rect = particle.AddComponent<FilledRect>();
                rect.color = color ?? new Color(235, 025, 010);
                rect.SetRect(new Rect(0.3 + EngineObject.GetRandom().NextDouble() * 1.5, 0.3 + EngineObject.GetRandom().NextDouble() * 1.5));
                particle.SetPosition(position);

                var body = particle.AddComponent<PhysicsBody>();
                body.IsMovable = true;
                body.CollideWithMovingObjects = false;
                var collider = particle.AddComponent<BoxCollider>();
                collider.box = new Rect(0, 0, 0.1, 0.1);
                double minVel = 35;
                double maxVel = 80;
                double vel = EngineObject.GetRandom().NextDouble() * (maxVel - minVel) + minVel;
                double angle = EngineObject.GetRandom().NextDouble() * 2 - 1;
                if(dirX > 0 || dirY > 0)
                {
                    angle = Math.Atan2(dirY, dirX) + angle;
                } else {
                    angle = EngineObject.GetRandom().NextDouble() * Math.PI * 2;
                }

                body.Velocity = new Vec2D(Math.Cos(angle), Math.Sin(angle)) * vel;
                body.AngularVelocity = EngineObject.GetRandom().NextDouble() * 720 - 360;

                var script = particle.AddComponent<BloodParticle>();
            }
        }

    }
}
