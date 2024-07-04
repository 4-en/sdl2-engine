using SDL2Engine;

namespace TileBasedGame.Entities
{
    public class HealthBarRenderer : DrawableRect
    {
        public double health = 1.0;
        public Color bg_color = new Color(255, 0, 0, 255);
        public Color fg_color = new Color(0, 255, 0, 255);

        public HealthBarRenderer()
        {
            this.rect = new Rect(0, 0, 140, 14);
            this.z_index = -100;
        }

        public override void Draw(Camera camera)
        {

            var bg_rect = GetScreenRect();
            var sdl_bg_rect = bg_rect.ToSDLRect();
            var sdl_bg_color = bg_color.ToSDLColor();
            SDL2.SDL.SDL_SetRenderDrawColor(Engine.renderer, sdl_bg_color.r, sdl_bg_color.g, sdl_bg_color.b, sdl_bg_color.a);   
            SDL2.SDL.SDL_RenderFillRect(Engine.renderer, ref sdl_bg_rect);
            // get the part of the health bar that represents the health
            var fg_rect = bg_rect;
            fg_rect.w = (int)(bg_rect.w * health);
            var sdl_fg_rect = fg_rect.ToSDLRect();
            var sdl_fg_color = fg_color.ToSDLColor();
            SDL2.SDL.SDL_SetRenderDrawColor(Engine.renderer, sdl_fg_color.r, sdl_fg_color.g, sdl_fg_color.b, sdl_fg_color.a);
            SDL2.SDL.SDL_RenderFillRect(Engine.renderer, ref sdl_fg_rect);

        }
    }

    public class HealthBar : Script
    {
        private IDamageable? target;
        private HealthBarRenderer? renderer;
        private double animationSpeed = 0.001;
        private double currentHealth = 1.0;
        private double targetHealth = 1.0;

        public override void Start()
        {
            target = GetComponentInParent<IDamageable>();
            if (target == null)
            {
                Destroy(gameObject);
                return;
            }
            renderer = gameObject.GetComponent<HealthBarRenderer>();

            if (renderer == null)
            {
                renderer = gameObject.AddComponent<HealthBarRenderer>();
            }
        }

        public override void Update()
        {
            if (target == null || renderer == null)
            {
                return;
            }

            targetHealth = target.GetHealth() / target.GetMaxHealth();
            if (currentHealth != targetHealth)
            {
                currentHealth = Math.Min(targetHealth, currentHealth + animationSpeed * Time.deltaTime * target.GetMaxHealth());
                renderer.health = currentHealth;
            }
        }

        public static HealthBar AddTo(GameObject gameObject, int yOffset = 0)
        {
            var child = gameObject.CreateChild("HealthBarGameObject");
            var healthBar = child.AddComponent<HealthBar>();
            var renderer = child.AddComponent<HealthBarRenderer>();

            child.SetLocalPosition(new Vec2D(0, yOffset));

            return healthBar;
        }
    }
}
