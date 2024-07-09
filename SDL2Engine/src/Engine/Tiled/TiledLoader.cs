using SDL2Engine.Tiled;
using System;
using System.Reflection;
using System.Runtime.CompilerServices;
using TiledCSPlus;

namespace SDL2Engine.Tiled
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
        public static List<GameObject> LoadTMX(string path, Assembly? callingAssembly = null, bool sideBorder = true)
        {
            path = AdjustPath(path, "Assets/Tiled/");
            string rootDir = path.Substring(0, path.LastIndexOf("/") + 1);
            var map = new TiledMap(path);

            double height = map.Height * map.TileHeight;

            double aspectRatio = 16.0 / 9.0;
            double width = height * aspectRatio;
            Camera? cam = Camera.GetSceneCamera();
            if (cam != null)
            {
                cam.WorldSize = new Vec2D(width, height);
            }

            var tilesets = map.GetTiledTilesets(rootDir);
            var tileLayers = map.Layers.Where(x => x.Type == TiledLayerType.TileLayer);

            var gameObjects = new List<GameObject>();

            int minX = 0;
            int minY = 0;
            int maxX = 0;
            int maxY = 0;

            callingAssembly ??= Assembly.GetCallingAssembly();
            int layerOrder = tileLayers.Count();
            foreach (var layer in tileLayers)
            {
                bool isObstaceLayer = layer.Class == "Obstacles";
                // Console.WriteLine("Layer: " + layer.Name);
                foreach (var chunk in layer.Chunks)
                    
                    for (var y = 0; y < chunk.Height; y++)
                    {
                        for (var x = 0; x < chunk.Width; x++)
                        {
                            var index = (y * chunk.Width) + x; // Assuming the default render order is used which is from right to bottom
                            var gid = chunk.Data[index]; // The tileset tile index

                            
                            int gridX = x + chunk.X;
                            int gridY = y + chunk.Y;
                            var tileX = gridX * map.TileWidth;
                            var tileY = gridY * map.TileHeight;

                            if (isObstaceLayer)
                            {
                                minX = Math.Min(minX, gridX);
                                maxX = Math.Max(maxX, gridX);
                                
                            }
                            minY = Math.Min(minY, gridY);
                            maxY = Math.Max(maxY, gridY);

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
                            TiledTile? tile = map.GetTiledTile(mapTileset, tileset, gid);

                            // this can be null if the tile does not have any properties
                            // it can still be rendered by getting the rect from the tileset


                            CreateGameObjectFromTile(gid, layer, layerOrder, callingAssembly, rootDir, map, gameObjects, tileX, tileY, tileset, tile, mapTileset);
                        }
                    }
                layerOrder--;
            }

            var objectLayers = map.Layers.Where(x => x.Type == TiledLayerType.ObjectLayer);
            foreach (var layer in objectLayers)
            {
                foreach (var obj in layer.Objects)
                {
                    var position = obj.Position;
                    var size = obj.Size;
                    var gid = obj.Gid;
                    var tileX = position.X;
                    var tileY = position.Y;


                    GameObject gameObject = new GameObject(obj.Name);
                    gameObject.SetPosition(new Vec2D(tileX, tileY));

                    // Add components
                    // first layer components
                    foreach (TiledProperty property in layer.Properties)
                    {
                        if (property.Name == "components")
                        {
                            AddComponentsToTile(gameObject, property.Value, callingAssembly);
                        }
                    }


                    foreach (TiledProperty property in obj.Properties)
                    {
                        if (property.Name == "components")
                        {
                            AddComponentsToTile(gameObject, property.Value, callingAssembly);
                        }
                    }

                    // set properties after adding components, in case properties are part of the components
                    // set layer properties
                    foreach (TiledProperty property in layer.Properties)
                    {
                        SetComponentField(gameObject, property.Name, property.Value);
                    }

                    // set tile properties
                    foreach (TiledProperty property in obj.Properties)
                    {
                        SetComponentField(gameObject, property.Name, property.Value);
                    }



                    gameObjects.Add(gameObject);



                }
            }

            if(sideBorder)
            {
                // add invisible border on each side
                int tileHeight = map.TileHeight;
                int tileWidth = map.TileWidth;

                int[] sides = new int[] { minX - 1, maxX + 1 };

                foreach (int x in sides)
                {
                    for (int y = minY - 1; y <= maxY + 1; y++)
                    {
                        GameObject gameObject = new GameObject("Border");
                        gameObject.SetPosition(new Vec2D(x * tileWidth, y * tileHeight));
                        var boxCollider = gameObject.AddComponent<BoxCollider>();
                        boxCollider.box = new Rect(tileWidth, tileHeight);
                        gameObject.SetName("Obstacle");
                        gameObjects.Add(gameObject);
                    }
                }
            }

            // create tile data component
            GameObject tileData = new GameObject("TileData");
            var dataComp = tileData.AddComponent<TileMapData>();
            int mapHeight = maxY - minY + 1;
            int mapWidth = maxX - minX + 1;
            int[,] mapData = new int[mapHeight, mapWidth];

            foreach (var layer in tileLayers)
            {
                // only add tile data for obstacle layers
                if (layer.Class != "Obstacles")
                {
                    continue;
                }
                foreach (var chunk in layer.Chunks)
                {
                    for (var y = 0; y < chunk.Height; y++)
                    {
                        for (var x = 0; x < chunk.Width; x++)
                        {
                            var index = (y * chunk.Width) + x; // Assuming the default render order is used which is from right to bottom
                            var gid = chunk.Data[index]; // The tileset tile index

                            int gridX = x + chunk.X;
                            int gridY = y + chunk.Y;

                            if (gid == 0)
                            {
                                continue;
                            }

                            mapData[gridY - minY, gridX - minX] = TileMapData.OBSTACLE;
                        }
                    }
                }
            }

            dataComp.SetMapData(
                mapData,
                map.TileWidth,
                map.TileHeight,
                mapWidth,
                mapHeight,
                minX,
                minY
                );

            return gameObjects;
        }

        [MethodImplAttribute(MethodImplOptions.AggressiveInlining)]
        private static void CreateGameObjectFromTile(int gid, TiledLayer layer, int layerOrder, Assembly callingAssembly, string rootDir, TiledMap map, List<GameObject> gameObjects, int tileX, int tileY, TiledTileset tileset, TiledTile? tile, TiledMapTileset mapTileset)
        {
            // Create GameObject
            string source = tileset.Image.Source;
            source = rootDir + source;
            string name = tile?.Class ?? "Tile";

            GameObject gameObject = new GameObject(name);
            gameObject.SetPosition(new Vec2D(tileX, tileY));

            // use animation properties if available

            if (tile != null && tile.Animations.Length > 0)
            {
                var renderer = gameObject.AddComponent<TiledAnimationRenderer>();

                foreach (var animation in tile.Animations)
                {
                    var tileId = animation.TileId;
                    var duration = animation.Duration;

                    TiledSourceRect? rect = map.GetSourceRect(mapTileset, tileset, tileId + mapTileset.FirstGid);

                    if (rect == null)
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
                renderer.SetZIndex(layerOrder*10);

            }
            else
            {

                var renderer = gameObject.AddComponent<TextureRenderer>();
                renderer.SetSource(source);
                var rect = map.GetSourceRect(mapTileset, tileset, gid);
                renderer.SetSourceRect(new Rect(rect.X, rect.Y, rect.Width, rect.Height));
                renderer.anchorPoint = AnchorPoint.TopLeft;
                renderer.SetRect(new Rect(map.TileWidth, map.TileHeight));
                renderer.SetZIndex(layerOrder*10);
            }

            if (layer.Class == "Obstacles")
            {
                BoxCollider.FromDrawableRect(gameObject);
                gameObject.SetName("Obstacle");
            }

            if (layer.Parallax.X != 1.0 || layer.Parallax.Y != 1.0)
            {
                var phelper = gameObject.AddComponent<ParallaxHelper>();
                phelper.parallaxX = layer.Parallax.X;
                phelper.parallaxY = layer.Parallax.Y;
            }

            // Add components
            // first layer components
            foreach (TiledProperty property in layer.Properties)
            {
                if (property.Name == "components")
                {
                    AddComponentsToTile(gameObject, property.Value, callingAssembly);
                }
            }

            // then tile components
            if (tile != null)
            {
                foreach (TiledProperty property in tile.Properties)
                {
                    if (property.Name == "components")
                    {
                        AddComponentsToTile(gameObject, property.Value, callingAssembly);
                    }
                }
            }

            // set properties after adding components, in case properties are part of the components
            // set layer properties
            foreach (TiledProperty property in layer.Properties)
            {
                SetComponentField(gameObject, property.Name, property.Value);
            }

            // set tile properties
            if (tile != null)
            {
                foreach (TiledProperty property in tile.Properties)
                {
                    SetComponentField(gameObject, property.Name, property.Value);
                }
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

        /*
         * Add components that were defined in a Tiles components property
         * to the GameObject
         * Format: namespace1.Component1,namespace1.Component2,namespace2.Component3
         * 
         */

        [MethodImplAttribute(MethodImplOptions.AggressiveInlining)]
        private static void AddComponentsToTile(GameObject gameObject, string componentNames, Assembly callingAssembly)
        {

            var components = componentNames.Split(',');
            foreach (var componentName in components)
            {
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
                    Console.WriteLine($"Component {componentName} not found");
                }
            }

        }

        [MethodImplAttribute(MethodImplOptions.AggressiveInlining)]
        private static void SetComponentField(GameObject gameObject, string attribute, string value)
        {
            string[] parts = attribute.Split('.');
            if (parts.Length < 1)
            {
                return;
            }
            if (parts.Length == 1)
            {
                return;
            }

            // only allow attribute names in the format ComponentName.AttributeName[.AttributeName]
            // basically, the first part should be a component name and it needs at least one dot

            string componentName = parts[0];
            string other = string.Join(".", parts.Skip(1));

            Component? component = gameObject.GetComponentByClassName(componentName);
            if (component == null)
            {
                Console.WriteLine($"Component {componentName} not found");
                return;
            }

            set_attribute(component, other, value);
        }

        [MethodImplAttribute(MethodImplOptions.AggressiveInlining)]
        private static void set_attribute(object obj, string attribute, string value)
        {
            var type = obj.GetType();
            string[] parts = attribute.Split('.');
            if (parts.Length < 1)
            {
                return;
            }
            attribute = parts[0];
            string otherAttributes = string.Join(".", parts.Skip(1));
            var field = type.GetField(attribute, BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
            while (field == null && type.BaseType != null)
            {
                type = type.BaseType;
                field = type.GetField(attribute, BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
            }
            if (field == null)
            {
                Console.WriteLine("Attribute not found: " + attribute);
                return;
            }

            if (otherAttributes == "")
            {
                // convert value to the correct type
                switch (field.FieldType.Name)
                {
                    case "Int32":
                        field.SetValue(obj, int.Parse(value));
                        break;
                    case "Single":
                        field.SetValue(obj, float.Parse(value, System.Globalization.CultureInfo.InvariantCulture));
                        break;
                    case "Double":
                        double doubleValue = double.Parse(value, System.Globalization.CultureInfo.InvariantCulture);
                        field.SetValue(obj, doubleValue);
                        break;
                    case "String":
                        field.SetValue(obj, value);
                        break;
                    case "Boolean":
                        field.SetValue(obj, bool.Parse(value));
                        break;
                    default:
                        Console.WriteLine("Unsupported type: " + field.FieldType.Name);
                        break;
                }
            }
            else
            {
                var subObj = field.GetValue(obj);
                if (subObj == null)
                {
                    return;
                }
                set_attribute(subObj, otherAttributes, value);
            }
        }
    }
}
