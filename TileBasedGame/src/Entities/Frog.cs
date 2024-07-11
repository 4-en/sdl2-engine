using Newtonsoft.Json;
using SDL2;
using SDL2Engine;
using SDL2Engine.Tiled;

namespace TileBasedGame.Entities
{

    public enum FrogType
    {
        GREEN,
        BLUE,
        RED,
        ORANGE,
        PURPLE
    }
    public class Frog : Enemy
    {
        private FrogType slimeType;
        protected SpriteRenderer? spriteRenderer;


        public override void Start()
        {
            // random slime type
            slimeType = (FrogType)random.Next(0, 3);

            string path = "Assets/Textures\\Frog\\GreenBrown\\ToxicFrogGreenBrown_Idle.png";

            spriteRenderer = AddComponent<SpriteRenderer>();
            spriteRenderer.SetTexture(path);
            //spriteRenderer.SetSpriteSizeByCount(8, 1);
            spriteRenderer.SetSpriteSize(48, 48);
            spriteRenderer.SetSize(30, 30);
            spriteRenderer.AddAnimation(new AnimationInfo("idle1", 0, 8, 0.1));
            spriteRenderer.AddAnimation(new AnimationInfo("jump", 7, 7, 0.1));
            spriteRenderer.AddAnimation(new AnimationInfo("death", 28, 6, 0.1));
            spriteRenderer.PlayAnimation("idle1");
            spriteRenderer.SetAnimationType(AnimationType.Loop);

            BoxCollider.FromDrawableRect(gameObject);

            spriteRenderer.anchorPoint = AnchorPoint.Center;
            // BoxCollider.FromDrawableRect(gameObject).SetSize(10, 10);
            var collider = gameObject.GetComponent<BoxCollider>();
            if (collider != null)
            {
                collider.SetSizeAndTransoformPosition(15 / 2, (15 / 2) - 1, 15, 15);
            }
            physicsBody = AddComponent<PhysicsBody>();
            physicsBody.Bounciness = 0.0;
            physicsBody.Friction = 0;

            maxSpeed = 0;
            acceleration = 0;

            tileMapData = FindComponent<TileMapData>();

            /*
            if(tileMapData == null )
            {
                GameObject? data = Find("TileData");
                Console.WriteLine("TileData: " + data);
                Console.WriteLine("No tilemapdata found");
            }*/
        }

        public override void OnCollisionEnter(CollisionPair collision)
        {
            if (died) return;
            // check for collisions with walls

            var other = collision.GetOther(gameObject);

            IDamageable? damageable = other.GetComponent<IDamageable>();
            if (damageable != null)
            {

                damageable.Damage(new Damage(damage, gameObject, team));

            }
        }
        protected void Jump()
        {
            if (died) return;
            if (physicsBody == null)
            {
                return;
            }

            if (isGrounded || airJumps < maxAirJumps)
            {
                physicsBody.SetVelocity(new Vec2D(physicsBody.Velocity.x, -jumpForce));

                if (!isGrounded)
                {
                    airJumps++;
                    // Console.WriteLine("Air jumps: " + airJumps);
                }
            }
        }

        private string currentAnimation = "idle1";
        private void ChangeAnimation()
        {
            if (physicsBody == null || spriteRenderer == null)
            {
                return;
            }

            string newAnimation = currentAnimation;

            bool inAir = !isGrounded;

            if (died)
            {
                newAnimation = "death";
            }
            else if (inAir)
            {
                newAnimation = "jump";
            }

            else
            {
                newAnimation = "idle1";
            }

            string newAnimationCheck = newAnimation;

            if (currentAnimation != newAnimationCheck)
            {
                currentAnimation = newAnimationCheck;

                AnimationType aType = AnimationType.Loop;

                spriteRenderer.PlayAnimation(newAnimation, aType);
            }
        }

        public override void Update()
        {
            if (tileMapData == null)
            {
                tileMapData = FindComponent<TileMapData>();
            }
            base.Update();


            Jump();


            ChangeAnimation();
        }

    }
}
