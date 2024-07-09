using SDL2Engine;
using SDL2Engine.Tiled;

namespace TileBasedGame
{
    public class CameraController : Script {
        
        public double camera_height = 160;
        public double camera_width = 160 * 16 / 9;

        private double shake_x = 0;
        private double shake_y = 0;

        private Player? player;

        private EventListener<PlayerDamagedEvent>? player_damaged_listener;
        private TileMapData? tileMapData;

        public override void Start() {
            // get player component from same game object
            player = GetComponent<Player>();

            // listen to player damage events (shake camera on damage)
            player_damaged_listener = EventBus.AddListener<PlayerDamagedEvent>(OnPlayerDamagedEvent);

            // set camera size
            GetCamera().WorldSize = new Vec2D(camera_width, camera_height);

            // get tilemap data
            tileMapData = FindComponent<TileMapData>();
        }

        private void OnPlayerDamagedEvent(PlayerDamagedEvent e) {
            shake_x = 10;
            shake_y = 10;
        }

        public override void OnDestroy()
        {
            base.OnDestroy();
            if (player_damaged_listener != null)
            {
                EventBus.RemoveListener(player_damaged_listener);
                player_damaged_listener = null;
            }

        }

        public override void Update() {
            if(player == null) {
                return;
            }

            // get player position
            Vec2D player_position = player.GetGameObject().GetPosition();

            // get player facing direction
            bool isFacingRight = player.IsFacingRight();

            // calculate optimal camera position
            double camera_x_ratio = 0.4;
            double camera_y_ratio = 0.75;

            if(!isFacingRight) {
                camera_x_ratio = 1 - camera_x_ratio;
            }

            var camera = GetCamera();
            var visibleWorld = camera.GetVisibleWorld();

            double camera_world_width = visibleWorld.w;
            double camera_world_height = visibleWorld.h;

            double camera_x = player_position.x - camera_width * camera_x_ratio;
            double camera_y = player_position.y - camera_height * camera_y_ratio;

            // smooth camera follow
            double dist = (camera.GetPosition() - new Vec2D(camera_x, camera_y)).Length();

            double maxCameraSpeed = 1000;
            double minCameraSpeed = 100;
            double speed = Math.Max(minCameraSpeed, Math.Min(maxCameraSpeed, dist * 10));

            Vec2D camera_position = camera.GetPosition();
            Vec2D target_position = new Vec2D(camera_x, camera_y);

            if ((camera_position - target_position).Length() < 1)
            {
                camera.SetPosition(target_position);
            }
            else
            {
                Vec2D direction = (target_position - camera_position).Normalize();
                Vec2D velocity = direction * speed * Time.deltaTime;

                camera.SetPosition(camera_position + velocity);
            }

            // shake camera
            // TODO


            // limit camera to world bounds
            if(tileMapData == null)
            {
                return;
            }
            double tileHeight = tileMapData.GetTileHeight();
            double tileWidth = tileMapData.GetTileWidth();
            double minWorldX = tileMapData.GetMapStartX() * tileWidth;
            double minWorldY = tileMapData.GetMapStartY() * tileHeight;
            double maxWorldX = minWorldX + tileMapData.GetMapWidth() * tileWidth;
            double maxWorldY = minWorldY + tileMapData.GetMapHeight() * tileHeight;

            Vec2D camera_position_clamped = camera.GetPosition();
            camera_position_clamped.x = Math.Max(minWorldX, Math.Min(maxWorldX - camera_world_width, camera_position_clamped.x));
            camera_position_clamped.y = Math.Max(minWorldY, Math.Min(maxWorldY - camera_world_height, camera_position_clamped.y));

            camera.SetPosition(camera_position_clamped);

        }
    }
}