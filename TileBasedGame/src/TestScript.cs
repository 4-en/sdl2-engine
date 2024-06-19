using SDL2Engine;

namespace TileBasedGame
{
    public class TestScript : Script
    {
        public override void Start()
        {
            Delay(2.0, () =>
            {
                this.gameObject.Destroy();
            });
        }
    }
}
