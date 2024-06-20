using System.Reflection;
using System.Runtime.CompilerServices;
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

        [MethodImplAttribute(MethodImplOptions.NoInlining)]
        public static List<GameObject> LoadTMX(string path, Assembly? callingAssembly=null)
        {
            path = AdjustPath(path, "Assets/Tiled/");
            string rootDir = path.Substring(0, path.LastIndexOf("/") + 1);
            var map = new TiledMap(path);
            var tilesets = map.GetTiledTilesets(rootDir);
            var tileLayers = map.Layers.Where(x => x.Type == TiledLayerType.TileLayer);

            var gameObjects = new List<GameObject>();

            callingAssembly ??= Assembly.GetCallingAssembly();

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

                            

                            // Get the tile object from the tileset
                            var tile = map.GetTiledTile(mapTileset, tileset, gid);
                            if (tile == default)
                            {
                                Console.WriteLine("Error: Tile not found");
                                continue;
                            }


                            CreateGameObjectFromTile(gid, callingAssembly, rootDir, map, gameObjects, tileX, tileY, tileset, tile, mapTileset);
                        }
                    }
            }

            return gameObjects;
        }

        private static void CreateGameObjectFromTile(int gid, Assembly callingAssembly, string rootDir, TiledMap map, List<GameObject> gameObjects, int tileX, int tileY, TiledTileset tileset, TiledTile tile, TiledMapTileset mapTileset)
        {
            // Create GameObject
            string source = tileset.Image.Source;
            source = rootDir + source;
            string name = tileset.Tiles[1].Class;

            GameObject gameObject = new GameObject(name);
            gameObject.SetPosition(new Vec2D(tileX, tileY));

            // use animation properties if available

            if (tile.Animations.Length > 0)
            {
                var renderer = gameObject.AddComponent<TiledAnimationRenderer>();

                foreach (var animation in tile.Animations)
                {
                    var tileId = animation.TileId;
                    var duration = animation.Duration;

                    TiledSourceRect? rect = map.GetSourceRect(mapTileset, tileset, tileId + mapTileset.FirstGid);

                    if(rect == null)
                    {
                        Console.WriteLine("Error: Source rect not found");
                        continue;
                    }
                    Rect sourceRect = new Rect(rect.X, rect.Y, rect.Width, rect.Height);

                    renderer.AddAnimationFrame(sourceRect, duration);
                }

                renderer.SetSource(source);
                renderer.anchorPoint = AnchorPoint.TopLeft;
                renderer.SetSize(map.TileWidth, map.TileHeight);

            }
            else
            {

                var renderer = gameObject.AddComponent<TextureRenderer>();
                renderer.SetSource(source);
                var rect = map.GetSourceRect(mapTileset, tileset, gid);
                renderer.SetSourceRect(new Rect(rect.X, rect.Y, rect.Width, rect.Height));
                renderer.anchorPoint = AnchorPoint.TopLeft;
                renderer.SetRect(new Rect(map.TileWidth, map.TileHeight));
            }

            

            gameObjects.Add(gameObject);


            /*
             * This works so far, but seems like a bad solution
             * TODO: maybe fix later, probably not :)
             * 
            // try to get Type, first try from calling assembly, then from executing assembly
            string componentName = "TileBasedGame.TestScript";

            Type? componentType = callingAssembly.GetType(componentName);
            if (componentType == null)
            {
                componentType = Assembly.GetExecutingAssembly().GetType(componentName);
            }


            if (componentType != null)
            {
                var instance = Activator.CreateInstance(componentType);
                if (instance is Component script)
                {
                    gameObject.AddComponent(script);
                }
            }
            else
            {
                Console.WriteLine("Component not found");
            }
            */
        }
    }
}
