using SDL2Engine.Tiled;
using SDL2Engine;
using SDL2;
using static SDL2.SDL;

namespace SDL2Engine.Tiled
{

    public struct PathfindingSettings {
        public double entityWidth = 1;
        public double entityHeight = 1;
        public double maxJumpHeight = 1;
        public double maxFallHeight = double.MaxValue;

        public int maxSteps = 1000;

        public PathfindingSettings(double entityWidth, double entityHeight, double maxJumpHeight, double maxFallHeight, int maxSteps) {
            this.entityWidth = entityWidth;
            this.entityHeight = entityHeight;
            this.maxJumpHeight = maxJumpHeight;
            this.maxFallHeight = maxFallHeight;
            this.maxSteps = maxSteps;
        }

        public PathfindingSettings() {
            this.entityWidth = 1;
            this.entityHeight = 1;
            this.maxJumpHeight = 1;
            this.maxFallHeight = double.MaxValue;
            this.maxSteps = 1000;
        }

    }
    
    public class TileMapData : Script {

        public static readonly int OUT_OF_BOUNDS = -1;
        public static readonly int AIR = 0;
        public static readonly int OBSTACLE = 1;
        public static readonly int DANGER = 2;


        private int[,] mapData = new int[0, 0];
        private int tileWidth = 0;
        private int tileHeight = 0;
        private int mapWidth = 0;
        private int mapHeight = 0;
        private int mapStartX = 0;
        private int mapStartY = 0;

        public override void Start() {
            if(!dataSet)
            {
                Console.WriteLine("Map data not set");
                gameObject.Destroy();
            }
        }

        public override void Update() {

        }
        private bool dataSet = false;
        public void SetMapData(int[,] mapData, int tileWidth, int tileHeight, int mapWidth, int mapHeight, int mapStartX, int mapStartY) {
            this.mapData = mapData;
            this.tileWidth = tileWidth;
            this.tileHeight = tileHeight;
            this.mapWidth = mapWidth;
            this.mapHeight = mapHeight;
            this.mapStartX = mapStartX;
            this.mapStartY = mapStartY;

            dataSet = true;
        }

        public int[,] GetMapData() {
            return mapData;
        }

        public int[,] GetMapDataAround(int x, int y, int radius) {
            int width = radius * 2 / tileWidth;
            int height = radius * 2 / tileHeight;

            int[,] data = new int[width, height];

            for (int i = 0; i < width; i++) {
                for (int j = 0; j < height; j++) {
                    data[j, i] = mapData[y + j, x + i];
                }
            }

            return data;

        }

        public int GetTileWidth() {
            return tileWidth;
        }

        public int GetTileHeight() {
            return tileHeight;
        }

        public int GetMapWidth() {
            return mapWidth;
        }

        public int GetMapHeight() {
            return mapHeight;
        }

        public int GetMapStartX() {
            return mapStartX;
        }

        public int GetMapStartY() {
            return mapStartY;
        }

        public Vec2D TilePosToWorldPos(int x, int y)
        {
            return new Vec2D(
                (x * tileWidth),
                (y * tileHeight)
                );
        }

        public Tuple<int, int> WorldPosToTilePos(Vec2D pos)
        {
            return new Tuple<int, int>(
                (int)(pos.x / tileWidth),
                (int)(pos.y / tileHeight)
                );
        }

        public int GetTileAt(int x, int y) {
            x = x - mapStartX;
            y = y - mapStartY;

            if (x < 0 || y < 0 || x >= mapWidth || y >= mapHeight) {
                return -1;
            }

            return mapData[y, x];
        }

        public int GetTileAt(Vec2D pos)
        {
            return GetTileAt((int)(pos.x / tileWidth), (int)(pos.y / tileHeight));
        }

        public List<Tuple<int, int>> Pathfind(int startX, int startY, int endX, int endY, PathfindingSettings settings)
        {
            // TODO: implement this
            return new List<Tuple<int, int>>();
        }

        public List<Vec2D> Pathfind(Vec2D start, Vec2D end, PathfindingSettings settings)
        {
            int startX = (int)(start.x / tileWidth);
            int startY = (int)(start.y / tileHeight);
            int endX = (int)(end.x / tileWidth);
            int endY = (int)(end.y / tileHeight);

            var path = Pathfind(startX, startY, endX, endY, settings);

            List<Vec2D> worldPath = new List<Vec2D>();

            foreach (var tile in path)
            {
                int x = tile.Item1;
                int y = tile.Item2;
                worldPath.Add(TilePosToWorldPos(x, y));
            }

            return worldPath;
        }

    }
}