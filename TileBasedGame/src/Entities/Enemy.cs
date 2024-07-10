using SDL2Engine;
using SDL2Engine.Tiled;

namespace TileBasedGame.Entities
{

    internal enum BaseAIStates
    {
        IDLE,
        LEFT,
        RIGHT
    }
    public class Enemy : Entity
    {

        protected TileMapData? tileMapData;

        public override void Start()
        {
            base.Start();

            tileMapData = FindComponent<TileMapData>();

            var renderer = AddComponent<FilledRect>();
            renderer.SetRect(new Rect(0, 0, 12, 12));
            renderer.color = new Color(255, 0, 0, 255);
            renderer.anchorPoint = AnchorPoint.BottomCenter;
            BoxCollider.FromDrawableRect(gameObject);
            physicsBody = AddComponent<PhysicsBody>();
            physicsBody.Bounciness = 0.0;
            physicsBody.Friction = 0;


        }
        double lastStateChange = 0;
        BaseAIStates currentState = BaseAIStates.IDLE;
        public override void Update()
        {
            base.Update();
            if (lastStateChange + 3 < Time.time)
            {
                lastStateChange = Time.time;
                currentState = (BaseAIStates)random.Next(0, 3);
            }


            switch (currentState)
            {
                case BaseAIStates.IDLE:
                    Decellerate();
                    break;
                case BaseAIStates.LEFT:
                    TryMoveLeft();
                    break;
                case BaseAIStates.RIGHT:
                    TryMoveRight();
                    break;
            }
        }

        protected void TryMoveLeft()
        {

            if(tileMapData == null)
            {
                // Console.WriteLine("No tilemap data");
                MoveLeft();
                return;
            }

            var tilePos = tileMapData.WorldPosToTilePos(gameObject.GetPosition());
            int leftTile = tileMapData.GetTileAt(tilePos.Item1 - 1, tilePos.Item2);
            int bottomLeftTile = tileMapData.GetTileAt(tilePos.Item1 - 1, tilePos.Item2 + 1);

            if (leftTile != TileMapData.AIR || bottomLeftTile != TileMapData.OBSTACLE)
            {
                currentState = BaseAIStates.RIGHT;
                return;
            }

            MoveLeft();

        }

        protected void TryMoveRight()
        {
            if (tileMapData == null)
            {
                Console.WriteLine("No tilemap data");
                MoveRight();
                return;
            }

            var tilePos = tileMapData.WorldPosToTilePos(gameObject.GetPosition());
            int rightTile = tileMapData.GetTileAt(tilePos.Item1 + 1, tilePos.Item2);
            int bottomRightTile = tileMapData.GetTileAt(tilePos.Item1 + 1, tilePos.Item2 + 1);

            if (rightTile != TileMapData.AIR || bottomRightTile != TileMapData.OBSTACLE)
            {
                currentState = BaseAIStates.LEFT;
                return;
            }

            MoveRight();

        }

        public override void OnCollisionEnter(CollisionPair collision)
        {
            base.OnCollisionEnter(collision);
        }
    }
}
