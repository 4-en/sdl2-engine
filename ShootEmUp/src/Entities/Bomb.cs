using SDL2Engine;
using SDL2;
using static SDL2.SDL;

namespace ShootEmUp.Entities
{

    public class CircleRenderer : Drawable
    {
        public double radius = 100;
        public Color Color = new Color(255, 255, 255, 255);

        public override void Draw(Camera camera)
        {
            int resolution = 36;
            double screenRadius = camera.WorldToScreen(radius);

            var renderer = Engine.renderer;

            Vec2D screenCenter = GetDrawRoot();

            SDL_SetRenderDrawColor(renderer, Color.r, Color.g, Color.b, Color.a);

            // draw the circle
            // do this manually for now
            // is there no SDL function for this?
            // other solution would be to create a texture, but since this is just for debugging, we keep it simple
            double angle = 0;
            double angleStep = 2 * Math.PI / resolution;
            Vec2D lastPoint = new Vec2D(screenCenter.x + screenRadius, screenCenter.y);
            for (int i = 0; i < resolution; i++)
            {
                angle += angleStep;
                Vec2D point = new Vec2D(screenCenter.x + screenRadius * Math.Cos(angle), screenCenter.y + screenRadius * Math.Sin(angle));
                SDL.SDL_RenderDrawLine(renderer, (int)lastPoint.x, (int)lastPoint.y, (int)point.x, (int)point.y);
                lastPoint = point;
            }


        }
    }

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
        private CircleRenderer? hitboxRenderer;


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

            // add hitbox renderer
            var go = gameObject.CreateChild("Hitbox");
            hitboxRenderer = go.AddComponent<CircleRenderer>();
            hitboxRenderer.radius = 1;

            Color color = team == Team.Player ? new Color(255, 255, 255, 255) : new Color(255, 0, 0, 255);
            hitboxRenderer.Color = color;

            StartCoroutine(UpdateRadius());

        }

        private System.Collections.IEnumerator UpdateRadius()
        {
            double time = 0;
            while (time < timer)
            {
                if(hitboxRenderer != null)
                {
                    hitboxRenderer.radius = radius * (time / timer);
                }

                yield return 0.01;
                time += 0.01;
            }
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

            // delete hitbox renderer
            hitboxRenderer?.GetGameObject().Destroy();



        }
        private List<GameObject> hitGameObjects = new List<GameObject>();
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

            if(hitGameObjects.Contains(other))
            {
                return;
            }

            hitGameObjects.Add(other);

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
