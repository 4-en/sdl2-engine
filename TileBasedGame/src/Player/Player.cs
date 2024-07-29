using SDL2Engine;
using SDL2;
using static SDL2.SDL;
using TileBasedGame.Entities;
using SDL2Engine.Tiled;

namespace TileBasedGame
{

    public class Player : Entities.Entity
    {
        private DrawableRect? renderer;
        private TileMapData? tileMapData;

        public static double maxHealth = 1000;
        public static double currentHealth = 1000;

        public static Player CreatePlayer()
        {
            var player = new GameObject("Player");
            player.KeepInScene = true;
            var playerComp = player.AddComponent<Player>();
            player.AddComponent<HealthBar>();

            return playerComp;
        }
        public override void Start()
        {
            base.Start();

            this.attackSpeed = 4;
            this.team = Team.Player;
            damage = 0;

            this.SetMaxHealth(maxHealth);
            this.SetHealth(maxHealth);

            tileMapData = FindComponent<TileMapData>();

            var spriteRenderer = AddComponent<SpriteRenderer>();
            if (spriteRenderer != null)
            {
                spriteRenderer.SetTexture("Assets/Textures/adventurer-Sheet.png");
                spriteRenderer.SetSpriteSize(50, 37);
                spriteRenderer.SetSize(25, 25);
                spriteRenderer.AddAnimation(new AnimationInfo("idle1", 0, 4, 0.15));
                spriteRenderer.AddAnimation(new AnimationInfo("run", 8, 6, 0.10));
                spriteRenderer.AddAnimation(new AnimationInfo("jump", 15, 4, 0.07));
                spriteRenderer.AddAnimation(new AnimationInfo("falling", 19, 4, 0.07));
                spriteRenderer.AddAnimation(new AnimationInfo("attack", 42, 5, 0.07));
                spriteRenderer.AddAnimation(new AnimationInfo("crouch", 4, 2, 0.1));
                spriteRenderer.AddAnimation(new AnimationInfo("shoot", 48, 4, 0.03));
                spriteRenderer.PlayAnimation("idle1");
                spriteRenderer.SetAnimationType(AnimationType.Loop);
            }
            AddComponent<PlayerAnimation>();
            /*
            renderer = AddComponent<FilledRect>();
            renderer.color = new Color(155, 30, 200, 255);
            renderer.SetRect(new Rect(0, 0, 10, 20));
            renderer.anchorPoint = AnchorPoint.BottomCenter;
            */
            BoxCollider.FromDrawableRect(gameObject);
            physicsBody = AddComponent<PhysicsBody>();
            AddComponent<CameraController>();
            physicsBody.Bounciness = 0.0;
            physicsBody.Friction = 0;

            var collider = gameObject.GetComponent<BoxCollider>();
            if (collider != null)
            {
                collider.SetSize(10, 20);
            }
            

        }

        private void Testing()
        {
            if(Input.GetKeyDown(SDL_Keycode.SDLK_r))
            {
                gameObject.SetPosition(new Vec2D(0, 0));
            }

            if(Input.GetKeyDown(SDL_Keycode.SDLK_2)) {
                // Explosion effect
                var pos = gameObject.GetPosition();
                Effects.ExplosionParticles(pos, 10, new Color(255, 0, 0), 3);
            }

            if(Input.GetKeyDown(SDL_Keycode.SDLK_3)) {
                // Blood effect
                var pos = gameObject.GetPosition();
                Effects.SpawnBlood(pos, 10, new Vec2D(0, 0), new Color(255, 0, 0));
            }
            /*
            if(Input.GetKeyDown(SDL_Keycode.SDLK_x)) {
                
                // log tile data
                if(tileMapData != null)
                {
                    var tilePos = tileMapData.WorldPosToTilePos(gameObject.GetPosition());
                    int tile = tileMapData.GetTileAt(tilePos.Item1, tilePos.Item2);
                    int bottomTile = tileMapData.GetTileAt(tilePos.Item1, tilePos.Item2 + 1);
                    Console.WriteLine("Tile: " + tile + " Bottom Tile: " + bottomTile);
                }
            }
            */
        }

        public override void Update()
        {
            base.Update();
            if(physicsBody == null)
            {
                return;
            }

            Player.currentHealth = this.GetHealth();
            Player.maxHealth = this.GetMaxHealth();

            Testing();

            double runningBoost = 1.0;
            if (Input.GetKeyPressed(SDL_Keycode.SDLK_LSHIFT))
            {
                runningBoost = 1.8;
            }

            bool isShooting = false;
            if(Input.GetMouseButtonDown(0))
            {
                bool shot = TryShoot();
                if(shot)
                {
                    EventBus.Dispatch(new ShakeEvent(3));
                }
                isShooting = true;
            }
            bool movementKeyPressed = false;
            if(Input.GetKeyDown(SDL_Keycode.SDLK_SPACE))
            {
                Jump();
            }
            if(Input.GetKeyPressed(SDL_Keycode.SDLK_a))
            {
                MoveLeft(runningBoost);
                movementKeyPressed = true;
                if (!isShooting)
                {
                    facingRight = false;
                }
            }
            if(Input.GetKeyPressed(SDL_Keycode.SDLK_d))
            {
                MoveRight(runningBoost);
                movementKeyPressed = true;
                if(!isShooting)
                    facingRight = true;
            }

            if (!movementKeyPressed)
            {
                Decellerate();
            }
        }
    }

    
    public class PlayerAnimation : Script
    {
        private Player? player;

        public override void Start()
        {
            player = gameObject.GetComponent<Player>();
        }
        public override void Update()
        {
            bool isFacingRight = true;
            if(player != null) {
                isFacingRight = player.IsFacingRight();
            }


            if (Input.GetKeyPressed(SDL_Keycode.SDLK_s) && !Input.GetKeyPressed(SDL_Keycode.SDLK_a) && !Input.GetKeyPressed(SDL_Keycode.SDLK_d))
            {
                gameObject.GetComponent<SpriteRenderer>()?.PlayAnimation("crouch");
                gameObject.GetComponent<SpriteRenderer>()?.SetFlipX(!isFacingRight);
                gameObject.GetComponent<SpriteRenderer>()?.SetAnimationType(AnimationType.Once);
            }
            //check if animation attack or crouch else movin animations
            if (gameObject.GetComponent<SpriteRenderer>()?.GetCurrentAnimation() != "attack" && gameObject.GetComponent<SpriteRenderer>()?.GetCurrentAnimation() != "crouch" && gameObject.GetComponent<SpriteRenderer>()?.GetCurrentAnimation() != "shoot")
            {
                if (gameObject.GetComponent<PhysicsBody>()?.Velocity.x == 0 && gameObject.GetComponent<PhysicsBody>()?.Velocity.y == 0)
                {
                    //play animation idle
                    gameObject.GetComponent<SpriteRenderer>()?.PlayAnimation("idle1");
                    gameObject.GetComponent<SpriteRenderer>()?.SetAnimationType(AnimationType.LoopReversed);
                    gameObject.GetComponent<SpriteRenderer>()?.SetFlipX(!isFacingRight);
                }
                else if (gameObject.GetComponent<PhysicsBody>()?.Velocity.y < 0)
                {
                    gameObject.GetComponent<SpriteRenderer>()?.PlayAnimation("jump");
                    gameObject.GetComponent<SpriteRenderer>()?.SetFlipX(!isFacingRight);
                    gameObject.GetComponent<SpriteRenderer>()?.SetAnimationType(AnimationType.OnceAndHold);
                }
                else if (gameObject.GetComponent<PhysicsBody>()?.Velocity.y > 0)
                {
                    gameObject.GetComponent<SpriteRenderer>()?.PlayAnimation("falling");
                    gameObject.GetComponent<SpriteRenderer>()?.SetFlipX(!isFacingRight);
                    gameObject.GetComponent<SpriteRenderer>()?.SetAnimationType(AnimationType.OnceAndHold);
                }
                else
                {
                    gameObject.GetComponent<SpriteRenderer>()?.PlayAnimation("run");
                    gameObject.GetComponent<SpriteRenderer>()?.SetFlipX(!isFacingRight);
                    gameObject.GetComponent<SpriteRenderer>()?.SetAnimationType(AnimationType.Loop);
                }

            }
        }
    }


    internal class HealthBar : Script
    {
        GameObject healthBarBackground = new GameObject("HealthBarBackground");
        GameObject healthBarBorder = new GameObject("HealthBarBorder");
        GameObject healthBarText = new GameObject("HealthBarText");
        GameObject? heart = null;
        int width = 20;
        int height = 4;

        public static SDL2Engine.Color backgroundColor = new SDL2Engine.Color(255, 0, 0, 255);

        public override void Start()
        {
            var camera = GetCamera();

            healthBarBackground.transform.position = new Vec2D(100, camera.GetVisibleHeight() - 100);
            var healthIndicator = healthBarBackground.AddComponent<TextRenderer>();
            healthIndicator.anchorPoint = AnchorPoint.CenterLeft;
            healthIndicator.SetPreferredSize(new Rect(0, 0, width, height));
            healthIndicator.SetColor(Color.Green);
            healthIndicator.SetBackgroundColor(backgroundColor);



            var border = healthBarBorder.AddComponent<TextRenderer>();
            var textRenderHelper = healthBarBorder.AddComponent<TextRenderHelper>();
            healthBarBorder.transform.position = new Vec2D(800, camera.GetVisibleHeight() - 800);
            border.anchorPoint = AnchorPoint.CenterLeft;
            border.SetPreferredSize(new Rect(0, 0, width, height));
            border.SetBorderSize(2);
            border.SetBorderColor(Color.Black);

            var text = healthBarText.AddComponent<TextRenderer>();
            healthBarText.transform.position = new Vec2D(100 + width / 2, camera.GetVisibleHeight() - 100);
            text.anchorPoint = AnchorPoint.Center;
            text.SetText(Player.currentHealth.ToString());
            text.SetColor(Color.White);
            text.SetFontSize(80);
            text.SetTextScale(0.1);
            text.SetFontPath("Assets/Fonts/Arcadeclassic.ttf");

            heart = new GameObject("Heart");
            var heartTexture = heart.AddComponent<SpriteRenderer>();
            heartTexture.SetSize(12, 12);
            heartTexture.relativeToCamera = false;
            heartTexture.SetSource("Assets/Textures/health.png");
            heartTexture.IsVisible(new Rect(4, 4));
            heart.transform.position = new Vec2D(-100, camera.GetVisibleHeight() - 100);

        }

        public override void Update()
        {
            double currentHealth = Player.currentHealth;
            double maxHealth = Player.maxHealth;
            double healthBarWidth = currentHealth / maxHealth * 64;
            var healthIndicator = healthBarBackground.GetComponent<TextRenderer>();
            healthIndicator?.SetRect(new Rect(0, 0, healthBarWidth, 10));
            healthIndicator?.SetBackgroundColor(backgroundColor);

            //update the text
            var text = healthBarText.GetComponent<TextRenderer>();
            text?.SetText(Player.currentHealth.ToString());

            var camera = GetCamera();

            healthBarBackground.transform.position = new Vec2D(210, camera.GetVisibleHeight() - 15);
            healthBarBorder.transform.position = new Vec2D(210, camera.GetVisibleHeight() - 15);
            healthBarText.transform.position = new Vec2D(233 + width / 2, camera.GetVisibleHeight() - 15);
            if (heart == null) return;
            heart.transform.position = new Vec2D(210, camera.GetVisibleHeight() - 15);

        }
    }

}