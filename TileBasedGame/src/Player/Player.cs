using SDL2Engine;
using SDL2;
using static SDL2.SDL;

namespace TileBasedGame
{

    public class Player : Entities.Entity
    {
        private DrawableRect? renderer;

        public static Player CreatePlayer()
        {
            var player = new GameObject("Player");
            var playerComp = player.AddComponent<Player>();

            return playerComp;
        }
        public override void Start()
        {
            base.Start();

            var spriteRenderer = AddComponent<SpriteRenderer>();
            if (spriteRenderer != null)
            {
                spriteRenderer.SetTexture("Assets/Textures/adventurer-Sheet.png");
                spriteRenderer.SetSpriteSize(50, 37);
                spriteRenderer.SetSize(25, 25);
                spriteRenderer.AddAnimation(new AnimationInfo("idle1", 0, 4, 0.15));
                spriteRenderer.AddAnimation(new AnimationInfo("run", 8, 7, 0.15));
                spriteRenderer.AddAnimation(new AnimationInfo("jump", 15, 4, 0.07));
                spriteRenderer.AddAnimation(new AnimationInfo("falling", 19, 4, 0.07));
                spriteRenderer.PlayAnimation("idle1");
                spriteRenderer.SetAnimationType(AnimationType.LoopReversed);
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
            physicsBody.Friction = 2;

        }

        public override void Update()
        {
            if(physicsBody == null)
            {
                return;
            }
            
            double maxVelocity = 200.0;
            double stopVelocity = 25;
            double acceleration = 1000.0;

            if(Input.GetKeyDown(SDL_Keycode.SDLK_SPACE))
            {
                physicsBody.AddVelocity(new Vec2D(0, -acceleration/10));
            }
            if(Input.GetKeyPressed(SDL_Keycode.SDLK_a))
            {
                physicsBody.AddVelocity(new Vec2D(-acceleration * Time.deltaTime, 0));
                stopVelocity = 0;
            }
            if(Input.GetKeyPressed(SDL_Keycode.SDLK_d))
            {
                physicsBody.AddVelocity(new Vec2D(acceleration * Time.deltaTime, 0));
                stopVelocity = 0;
            }

            if(physicsBody.Velocity.x > maxVelocity)
            {
                physicsBody.Velocity = new Vec2D(maxVelocity, physicsBody.Velocity.y);
            }

            if(physicsBody.Velocity.x < -maxVelocity)
            {
                physicsBody.Velocity = new Vec2D(-maxVelocity, physicsBody.Velocity.y);
            }

            if(physicsBody.Velocity.x < stopVelocity && physicsBody.Velocity.x > -stopVelocity)
            {
                physicsBody.Velocity = new Vec2D(0, physicsBody.Velocity.y);
            }
        }
    }

    
    public class PlayerAnimation : Script
    {
        public override void Update()
        {
            if(gameObject.GetComponent<PhysicsBody>()?.Velocity.x == 0 && gameObject.GetComponent<PhysicsBody>()?.Velocity.y == 0)
            {
                //play animation idle
                gameObject.GetComponent<SpriteRenderer>()?.PlayAnimation("idle1");
                gameObject.GetComponent<SpriteRenderer>()?.SetAnimationType(AnimationType.LoopReversed);
            }
            else if(gameObject.GetComponent<PhysicsBody>()?.Velocity.y < 0)
            {
                gameObject.GetComponent<SpriteRenderer>()?.PlayAnimation("jump");
                gameObject.GetComponent<SpriteRenderer>()?.SetFlipX(gameObject.GetComponent<PhysicsBody>()?.Velocity.x < 0);
                gameObject.GetComponent<SpriteRenderer>()?.SetAnimationType(AnimationType.OnceAndHold);
            }
            else if (gameObject.GetComponent<PhysicsBody>()?.Velocity.y > 0)
            {
                gameObject.GetComponent<SpriteRenderer>()?.PlayAnimation("falling");
                gameObject.GetComponent<SpriteRenderer>()?.SetFlipX(gameObject.GetComponent<PhysicsBody>()?.Velocity.x < 0);
                gameObject.GetComponent<SpriteRenderer>()?.SetAnimationType(AnimationType.OnceAndHold);
            }
            else
            {
                gameObject.GetComponent<SpriteRenderer>()?.PlayAnimation("run");
                gameObject.GetComponent<SpriteRenderer>()?.SetFlipX(gameObject.GetComponent<PhysicsBody>()?.Velocity.x < 0);
                gameObject.GetComponent<SpriteRenderer>()?.SetAnimationType(AnimationType.LoopReversed);
            }
        }
    }



}