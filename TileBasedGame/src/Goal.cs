using SDL2Engine;
using SDL2Engine.Tiled;

namespace TileBasedGame
{
    public class Goal : Script
    {

        public override void Start()
        {
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
                }
            }
        }
    }
}
