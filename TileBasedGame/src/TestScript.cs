using SDL2Engine;

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
