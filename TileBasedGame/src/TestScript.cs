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
        public override void Start()
        {
            Console.WriteLine("Player Start");
        }

        public override void Update()
        {
            var camera = GetCamera();

            if(Input.GetKeyPressed(SDL_Keycode.SDLK_w))
            {
                camera.SetPosition(camera.GetPosition() + new Vec2D(0, -1000) * Time.deltaTime);
            }
            if(Input.GetKeyPressed(SDL_Keycode.SDLK_s))
            {
                camera.SetPosition(camera.GetPosition() + new Vec2D(0, 1000) * Time.deltaTime);
            }
            if(Input.GetKeyPressed(SDL_Keycode.SDLK_a))
            {
                camera.SetPosition(camera.GetPosition() + new Vec2D(-1000, 0) * Time.deltaTime);
            }
            if(Input.GetKeyPressed(SDL_Keycode.SDLK_d))
            {
                camera.SetPosition(camera.GetPosition() + new Vec2D(1000, 0) * Time.deltaTime);
            }
        }
    }
}
