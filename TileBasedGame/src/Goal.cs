using SDL2Engine;
using SDL2Engine.Tiled;

namespace TileBasedGame
{
    public class Goal : Script
    {
        SoundPlayer? goalSound = null;
        public override void Start()
        {

            goalSound = AddComponent<SoundPlayer>();
            goalSound?.Load("Assets/Audio/win.mp3");
            goalSound?.SetVolume(0.2);

            BoxCollider collider = gameObject.AddComponent<BoxCollider>();
            
            var tiledData = FindComponent<TileMapData>();
            double width = 16;
            double height = 16;

            if (tiledData != null)
            {
                width = tiledData.GetTileWidth();
                height = tiledData.GetTileHeight();
            }

            collider.box = new Rect(0, 0, width, height);
        }

        public override void OnCollisionEnter(CollisionPair collision)
        {
            var other = collision.GetOther(gameObject);

            if (other.GetComponent<Player>() != null)
            {
                EventBus.Dispatch(new PlayerScoreEvent(100));

                var level = FindComponent<Level>();
                if (level != null)
                {
                    level.CompleteLevel();
                    goalSound?.Play();
                }
            }
        }
    }
}
