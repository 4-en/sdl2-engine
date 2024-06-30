using System;
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

            double height = map.Height * map.TileHeight;

            double aspectRatio = 16.0 / 9.0;
            double width = height * aspectRatio;
            Camera? cam = Camera.GetSceneCamera();
            if(cam != null)
            {
                cam.WorldSize = new Vec2D(width, height);
            }

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
                            TiledTile? tile = map.GetTiledTile(mapTileset, tileset, gid);
                            
                            // this can be null if the tile does not have any properties
                            // it can still be rendered by getting the rect from the tileset


                            CreateGameObjectFromTile(gid, layer, callingAssembly, rootDir, map, gameObjects, tileX, tileY, tileset, tile, mapTileset);
                        }
                    }
            }

            return gameObjects;
        }

        [MethodImplAttribute(MethodImplOptions.AggressiveInlining)]
        private static void CreateGameObjectFromTile(int gid, TiledLayer layer, Assembly callingAssembly, string rootDir, TiledMap map, List<GameObject> gameObjects, int tileX, int tileY, TiledTileset tileset, TiledTile? tile, TiledMapTileset mapTileset, bool addCollision = false)
        {
            // Create GameObject
            string source = tileset.Image.Source;
            source = rootDir + source;
            string name = tileset.Tiles[1].Class;

            GameObject gameObject = new GameObject(name);
            gameObject.SetPosition(new Vec2D(tileX, tileY));

            // use animation properties if available

            if (tile!=null && tile.Animations.Length > 0)
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

            if(addCollision)
            {
                BoxCollider.FromDrawableRect(gameObject);
            }

            // Add components
            // first layer components
            foreach(TiledProperty property in layer.Properties)
            {
                if(property.Name == "components")
                {
                    AddComponentsToTile(gameObject, property.Value, callingAssembly);
                }
            }

            // then tile components
            if(tile != null)
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
            if(parts.Length == 1)
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
