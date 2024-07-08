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
}
