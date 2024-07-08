using SDL2Engine;
using SDL2;
using static SDL2.SDL;
using TileBasedGame.Entities;

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
                spriteRenderer.AddAnimation(new AnimationInfo("run", 8, 7, 0.20));
                spriteRenderer.AddAnimation(new AnimationInfo("jump", 15, 4, 0.07));
                spriteRenderer.AddAnimation(new AnimationInfo("falling", 19, 4, 0.07));
                spriteRenderer.AddAnimation(new AnimationInfo("attack", 42, 5, 0.07));
                spriteRenderer.AddAnimation(new AnimationInfo("crouch", 4, 2, 0.1));
                spriteRenderer.AddAnimation(new AnimationInfo("shoot", 48, 4, 0.03));
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
            physicsBody.Friction = 0;

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
        }

        public override void Update()
        {
            base.Update();
            if(physicsBody == null)
            {
                return;
            }

            Testing();

            double runningBoost = 1.0;
            if (Input.GetKeyPressed(SDL_Keycode.SDLK_LSHIFT))
            {
                runningBoost = 1.5;
            }

            bool isShooting = false;
            if(Input.GetKeyDown(SDL_Keycode.SDLK_1))
            {
                TryShoot();
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


            if (Input.GetMouseButtonDown(0))
            {
                gameObject.GetComponent<SpriteRenderer>()?.PlayAnimation("attack");
                gameObject.GetComponent<SpriteRenderer>()?.SetFlipX(!isFacingRight);
                gameObject.GetComponent<SpriteRenderer>()?.SetAnimationType(AnimationType.Once);
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
                    gameObject.GetComponent<SpriteRenderer>()?.SetAnimationType(AnimationType.LoopReversed);
                }

            }
        }
    }


    
}