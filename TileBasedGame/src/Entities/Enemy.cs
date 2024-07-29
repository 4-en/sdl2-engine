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

        private void ChangeState()
        {
            if (player != null)
            {
                Vec2D dist = player.GetPosition() - gameObject.GetPosition();
                double distX = dist.x;
                double distY = dist.y;

                if (Math.Abs(distX) < 100 && Math.Abs(distY) < 30)
                {
                    if (distX < 0)
                    {
                        currentState = BaseAIStates.LEFT;
                    }
                    else
                    {
                        currentState = BaseAIStates.RIGHT;
                    }
                    return;
                }
            } else
            {
                player = Find("Player");
            }

            currentState = (BaseAIStates)random.Next(0, 3);
        }

        public override void Update()
        {
            base.Update();
            if (lastStateChange + 3 < Time.time)
            {
                lastStateChange = Time.time;
                this.ChangeState();
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

            var tilePos = tileMapData.WorldPosToTilePos(gameObject.GetPosition()  + new Vec2D(0, -1));
            int leftTile = tileMapData.GetTileAt(tilePos.Item1 - 1, tilePos.Item2);
            int bottomLeftTile = tileMapData.GetTileAt(tilePos.Item1 - 1, tilePos.Item2 + 1);

            if(Input.GetKeyPressed(SDL2.SDL.SDL_Keycode.SDLK_x))
            {
                Console.WriteLine("Left Tile: " + leftTile);
                Console.WriteLine("Bottom Left Tile: " + bottomLeftTile);
            }

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
                //Console.WriteLine("No tilemap data");
                MoveRight();
                return;
            }

            var tilePos = tileMapData.WorldPosToTilePos(gameObject.GetPosition() + new Vec2D(0, -1));
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

        public override void OnPreCollision(CollisionPair collision)
        {
            base.OnPreCollision(collision);

            var other = collision.GetOther(gameObject);
            IDamageable? otherDamageable = other.GetComponent<IDamageable>();
            if (otherDamageable != null)
            {
                if(otherDamageable.GetTeam() == this.team)
                {
                    collision.Cancel();
                }
            }

        }
    }
}
