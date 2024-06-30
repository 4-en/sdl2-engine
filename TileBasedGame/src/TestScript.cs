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

        }

        public override void Update()
        {
            

            if(Input.GetKeyPressed(SDL_Keycode.SDLK_w))
            {
                gameObject.transform.Move(0, -200*Time.deltaTime);
            }
            if(Input.GetKeyPressed(SDL_Keycode.SDLK_s))
            {
                gameObject.transform.Move(0, 200*Time.deltaTime);
            }
            if(Input.GetKeyPressed(SDL_Keycode.SDLK_a))
            {
                gameObject.transform.Move(-200*Time.deltaTime, 0);
            }
            if(Input.GetKeyPressed(SDL_Keycode.SDLK_d))
            {
                gameObject.transform.Move(200*Time.deltaTime, 0);
            }

            // center camera on player
            var camera = GetCamera();
            var cameraPosition = camera.GetPosition();
            var cameraHalfSize = camera.GetVisibleSize() / 2;
            camera.SetPosition(gameObject.transform.position - cameraHalfSize);
        }
    }
}
