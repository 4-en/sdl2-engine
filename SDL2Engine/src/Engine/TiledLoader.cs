using TiledCSPlus;

namespace SDL2Engine
{
    public static class TiledLoader
    {

        private static string AdjustPath(string path, string default_path)
        {
            // check if there is a / or \\ in the path
            if (path.Contains("/") || path.Contains("\\"))
            {
                return path;
            }

            return default_path + path;
        }

        public static List<GameObject> LoadTMX(string path)
        {
            path = AdjustPath(path, "Assets/Tiled/");
            string rootDir = path.Substring(0, path.LastIndexOf("/") + 1);
            var map = new TiledMap(path);
            var tilesets = map.GetTiledTilesets(rootDir);
            var tileLayers = map.Layers.Where(x => x.Type == TiledLayerType.TileLayer);

            var gameObjects = new List<GameObject>();

            foreach (var layer in tileLayers)
            {
                foreach (var chunk in layer.Chunks)
                    for (var y = 0; y < chunk.Height; y++)
                    {
                        for (var x = 0; x < chunk.Width; x++)
                        {
                            var index = (y * chunk.Width) + x; // Assuming the default render order is used which is from right to bottom
                            var gid = chunk.Data[index]; // The tileset tile index
                            var tileX = (x * map.TileWidth) + chunk.X * map.TileWidth;
                            var tileY = (y * map.TileHeight) + chunk.Y * map.TileHeight;

                            // Gid 0 is used to tell there is no tile set
                            if (gid == 0)
                            {
                                continue;
                            }

                            // Helper method to fetch the right TieldMapTileset instance. 
                            // This is a connection object Tiled uses for linking the correct tileset to the gid value using the firstgid property.
                            var mapTileset = map.GetTiledMapTileset(gid);

                            // Retrieve the actual tileset based on the firstgid property of the connection object we retrieved just now
                            var tileset = tilesets[mapTileset.FirstGid];

                            // Use the connection object as well as the tileset to figure out the source rectangle.
                            var rect = map.GetSourceRect(mapTileset, tileset, gid);

                            // Create GameObject
                            string source = tileset.Image.Source;
                            source = rootDir + source;
                            string name = tileset.Tiles[1].Class;

                            GameObject gameObject = new GameObject(name);
                            var renderer = gameObject.AddComponent<TextureRenderer>();
                            renderer.SetSource(source);
                            renderer.SetSourceRect(new Rect(rect.X, rect.Y, rect.Width, rect.Height));
                            renderer.anchorPoint = AnchorPoint.TopLeft;
                            renderer.SetRect(new Rect(tileX, tileY, map.TileWidth, map.TileHeight));

                            gameObjects.Add(gameObject);
                        }
                    }
            }

            return gameObjects;
        }
    }
}
