using SDL2Engine;
using SDL2;
using static SDL2.SDL;

namespace TileBasedGame
{
    public class TestScript : Script
    {
        int value = 0;
        public override void Start()
        {
            Delay(5.0, () =>
            {
                Console.WriteLine("TestScript.value = " + value);
                this.gameObject.Destroy();
            });
        }
    }

    public class TestScript2 : Script
    {
        public override void Start()
        {
            Delay(3.0, () =>
            {
                Console.WriteLine("TestScript2");
            });
        }
    }

    public class Player : Script
    {
        private DrawableRect? renderer;
        private PhysicsBody? physicsBody;
        public override void Start()
        {
            renderer = AddComponent<FilledRect>();
            renderer.color = new Color(155, 30, 200, 255);
            renderer.SetRect(new Rect(0, 0, 20, 40));
            renderer.anchorPoint = AnchorPoint.BottomCenter;
            BoxCollider.FromDrawableRect(gameObject);
            physicsBody = AddComponent<PhysicsBody>();
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

            // center camera on player
            var camera = GetCamera();
            var cameraPosition = camera.GetPosition();
            var cameraHalfSize = camera.GetVisibleSize() / 2;
            camera.SetPosition(gameObject.transform.position - cameraHalfSize);
        }
    }
}
