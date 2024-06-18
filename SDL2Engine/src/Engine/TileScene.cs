

namespace SDL2Engine
{

    internal class Tile
    {
        public int x, y;
        public int tile_id;
        public bool was_destroyed = false;
        /*
         * some data here
         */
    }

    internal class TileComponent : Component
    {
        public Tile? tile;
        public TileComponent()
        {
        }
    }

    /*
     * A scene that uses tilemaps to load and unload game objects.
     * 
     * Most Important features
     * - Parse tilemap files and store tile data in 2d array
     * - Create GameObjects from tile data based on Camera view
     * - Unload GameObjects that are out of Camera view
     * - Keep track of GameObjects that were destroyed during the scene, so
     *   they are not reloaded when the Camera view moves back to them
     */
    public class TileScene : Scene
    {

        private Tile[,] tiles;
        private int tileScale;
        private int start_x, start_y;
        private int map_width, map_height;

        public TileScene(string name, int width, int height, int tileScale, int start_x, int start_y) : base(name)
        {
            this.tileScale = tileScale;
            this.start_x = start_x;
            this.start_y = start_y;
            this.map_width = width;
            this.map_height = height;
            tiles = CreateTileArray(width, height);
        }

        private Tile[,] CreateTileArray(int width, int height)
        {
            var tiles = new Tile[width, height];
            for (int i = 0; i < width; i++)
            {
                for (int j = 0; j < height; j++)
                {
                    tiles[i, j] = new Tile();
                }
            }

            return tiles;
        }

        private Tile GetTile(int x, int y)
        {
            return tiles[x, y];
        }

        private Tile GetTileAt(int x, int y)
        {
            return tiles[x - start_x, y - start_y];
        }
        
    }
}
