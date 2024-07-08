using SDL2Engine;

namespace TileBasedGame
{
    public static partial class Effects
    {

        class ExplosionParticle : Script
        {
            public double lifeTime = 1;
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

                if(rect != null)
                {
                    startRectSize = (int)rect.GetRect().w;
                    currentRectSize = startRectSize;
                }

                body = GetComponent<PhysicsBody>();
                if(body != null)
                {
                    startVelocity = body.Velocity;
                }


            }

            public override void Update()
            {
                double timePercent = (Time.time - creationTime) / lifeTime;
                if (rect != null)
                {
                    int newSize = (int)(startRectSize * (1 - timePercent));
                    if (newSize != currentRectSize)
                    {
                        rect.SetRect(new Rect(newSize, newSize));
                        currentRectSize = newSize;
                    }
                }

                if (body != null)
                {
                    body.Velocity = startVelocity * (1 - timePercent);
                }

                if(timePercent >= 1)
                {
                    gameObject.Destroy();
                }
            }
        }
        public static void ExplosionParticles(Vec2D position, int count = 10, Color? color=null, double lifeTime = 1)
        {

            for (int i = 0; i < count; i++)
            {
                var particle = new GameObject("ExplosionParticle");

                var rect = particle.AddComponent<FilledRect>();
                rect.color = color ?? new Color(255, 255, 255);
                rect.SetRect(new Rect(2, 2));
                particle.SetPosition(position);

                var body = particle.AddComponent<PhysicsBody>();
                body.IsMovable = true;
                double minVel = 35;
                double maxVel = 50;
                double vel = EngineObject.GetRandom().NextDouble() * (maxVel - minVel) + minVel;
                double angle = EngineObject.GetRandom().NextDouble() * Math.PI * 2;

                body.Velocity = new Vec2D(Math.Cos(angle), Math.Sin(angle)) * vel;
                body.AngularVelocity = EngineObject.GetRandom().NextDouble() * 720 - 360;

                var script = particle.AddComponent<ExplosionParticle>();
                script.lifeTime = lifeTime;
            }
        }
        
    }
}
