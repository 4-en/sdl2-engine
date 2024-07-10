using SDL2Engine;
using SDL2Engine.Tiled;

namespace TileBasedGame.Entities
{

    public enum SlimeType
    {
        GREEN,
        BLUE,
        RED,
        ORANGE,
        PURPLE
    }
    public class Slime : Enemy
    {
        private SlimeType slimeType;
        protected SpriteRenderer? spriteRenderer;


        public override void Start()
        {
            // random slime type
            slimeType = (SlimeType)random.Next(0, 3);

            string path = "Assets/Textures/slimes/Slime_Blue_32x32.png";

            switch (slimeType)
            {
                case SlimeType.GREEN:
                    path = "Assets/Textures/slimes/Slime_Green_32x32.png";
                    break;
                case SlimeType.BLUE:
                    path = "Assets/Textures/slimes/Slime_Blue_32x32.png";
                    break;
                case SlimeType.RED:
                    path = "Assets/Textures/slimes/Slime_Red_32x32.png";
                    break;
                case SlimeType.ORANGE:
                    path = "Assets/Textures/slimes/Slime_Orange_32x32.png";
                    break;
                case SlimeType.PURPLE:
                    path = "Assets/Textures/slimes/Slime_Purple_32x32.png";
                    break;
                default:
                    path = "Assets/Textures/slimes/Slime_Blue_32x32.png";
                    break;
            }

            spriteRenderer = AddComponent<SpriteRenderer>();
            spriteRenderer.SetTexture(path);
            spriteRenderer.SetSpriteSizeByCount(6, 7);
            spriteRenderer.SetSize(12, 12);
            spriteRenderer.AddAnimation(new AnimationInfo("idle1", 0, 1, 1));
            spriteRenderer.AddAnimation(new AnimationInfo("run", 14, 6, 0.10));
            spriteRenderer.AddAnimation(new AnimationInfo("jump", 7, 7, 0.1));
            spriteRenderer.AddAnimation(new AnimationInfo("death", 28, 6, 0.1));
            spriteRenderer.PlayAnimation("idle1");
            spriteRenderer.SetAnimationType(AnimationType.OnceAndHold);


            spriteRenderer.anchorPoint = AnchorPoint.BottomCenter;
            BoxCollider.FromDrawableRect(gameObject);
            physicsBody = AddComponent<PhysicsBody>();
            physicsBody.Bounciness = 0.0;
            physicsBody.Friction = 0;

            maxSpeed = 40;
            acceleration = 40;

            tileMapData = FindComponent<TileMapData>();

            /*
            if(tileMapData == null )
            {
                GameObject? data = Find("TileData");
                Console.WriteLine("TileData: " + data);
                Console.WriteLine("No tilemapdata found");
            }*/
        }

        private string currentAnimation = "idle1";
        private void ChangeAnimation()
        {
            if(physicsBody == null || spriteRenderer == null) {
                return;
            }

            string newAnimation = currentAnimation;

            bool facingLeft = physicsBody.Velocity.x < 0;

            bool inAir = !isGrounded;

            if(died) {
                newAnimation = "death";
            } else if(inAir) {
                newAnimation = "jump";
            } else if(physicsBody.Velocity.x != 0) {
                newAnimation = "run";
            } else {
                newAnimation = "idle1";
            }

            string newAnimationCheck = newAnimation;
            if(facingLeft) {
                newAnimationCheck += "_left";
            } else
            {

               newAnimationCheck += "_right";
            }

            if(currentAnimation != newAnimationCheck) {
                currentAnimation = newAnimationCheck;

                
                
                if(facingLeft)
                {
                    spriteRenderer.SetFlipX(true);
                } else
                {
                    spriteRenderer.SetFlipX(false);
                }

                AnimationType aType = AnimationType.OnceAndHold;
                if(newAnimation == "run") {
                    aType = AnimationType.Loop;
                }

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
            ChangeAnimation();
        }

    }
}
