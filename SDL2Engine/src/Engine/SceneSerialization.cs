using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;
using System.Reflection;

namespace SDL2Engine
{
    public class SceneSerialization
    {

        public class EngineContractResolver : DefaultContractResolver
        {
            protected override JsonProperty CreateProperty(MemberInfo member, MemberSerialization memberSerialization)
            {
                var prop = base.CreateProperty(member, memberSerialization);

                // check for NonSerialized attribute
                if (member.GetCustomAttribute<NonSerializedAttribute>() != null)
                {
                    prop.ShouldSerialize = instance =>
                    {
                        return false;
                    };
                    return prop;
                }

                // only serialize primitive types
                var type = prop.PropertyType;

                if (type == null)
                {
                    return prop;
                }
                // allow primitives and strings and array like types
                if (type.IsPrimitive || type == typeof(string) || type.IsArray)
                {
                    return prop;
                }

                // if marked with [Serializable] attribute, serialize
                if (type.GetCustomAttribute<SerializableAttribute>() != null)
                {
                    // preserve reference
                    return prop;
                }

                // TODO: fix previous GetCustomAttribute thats not working correctly
                // maybe it doesn't work with child classes, or it 's not implemented correctly
                /*
                prop.ShouldSerialize = instance =>
                {
                    return false;
                }; */


                return prop;
            }
        }

        [Serializable]
        public struct SceneData
        {
            public string name;
            public int sceneType;
            public Camera mainCamera;
            public List<GameObject> gameObjects;
            public List<ProtoObject> protoObjects;
        }

        private readonly static JsonSerializerSettings SETTINGS = new JsonSerializerSettings
        {
            TypeNameHandling = TypeNameHandling.All,
            PreserveReferencesHandling = PreserveReferencesHandling.Objects,
            Formatting = Formatting.Indented,
            NullValueHandling = NullValueHandling.Include,
            ContractResolver = new EngineContractResolver()
        };

        public static bool SaveObject<T>(T obj, string path)
        {
            string json = JsonConvert.SerializeObject(obj, SETTINGS);

            File.WriteAllText(path, json);

            return true;
        }

        private static Prototype? CreatePrototypeFromScript(string path)
        {
            string prototypeName = Path.GetFileNameWithoutExtension(path);
            string withoutPath = Path.GetFileName(path);

            // check if there is a class with the same name as the file
            // if there is, create a prototype from it

            Type? type = Type.GetType(withoutPath);

            if (type == null || !type.IsSubclassOf(typeof(Component)))
            {
                return null;
            }
            
            Prototype prototype = new Prototype(prototypeName);
            var component = (Component?)Activator.CreateInstance(type);
            if (component == null)
            {
                return null;
            }

            prototype.GameObject.AddComponent(component);

            return prototype;
        }

        public static T? LoadObject<T>(string path)
        {
            if (!File.Exists(path))
            {

                if (typeof(T) == typeof(Prototype))
                {
                    //return CreatePrototypeFromScript(path) as T;
                }

                return default;
            }

            string json = File.ReadAllText(path);

            T? obj = JsonConvert.DeserializeObject<T>(json, SETTINGS);

            return obj;
        }


        public static bool SavePrototype(Prototype prototype)
        {
            
            string path = "Assets/Prototypes";
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
            path += "/" + prototype.GetName() + ".proto";

            return SceneSerialization.SaveObject(prototype, path);
        }

        public static Prototype? LoadPrototype(string name)
        {
            string path = "Assets/Prototypes/" + name + ".proto";

            return SceneSerialization.LoadObject<Prototype>(path);
        }

        public static bool SaveGameObject(GameObject gameObject)
        {
            string path = "Assets/GameObjects";
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
            path += "/" + gameObject.GetName() + ".go";

            return SceneSerialization.SaveObject(gameObject, path);
        }

        public static GameObject? LoadGameObject(string name)
        {
            string path = "Assets/GameObjects/" + name + ".go";

            return SceneSerialization.LoadObject<GameObject>(path);
        }


        public static bool SaveScene(Scene scene)
        {

            // 1. convert to scene data
            SceneData sceneData = new SceneData
            {
                name = scene.GetName(),
                sceneType = (int)scene.GetSceneType(),
                mainCamera = scene.GetCamera(),
                gameObjects = scene.GetGameObjects(),
                protoObjects = new List<ProtoObject>()
            };

            // 2. prepare path
            string path = "Assets/Scenes";
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
            path += "/" + scene.GetName() + ".scene";

            // 3. serialize
            string json = JsonConvert.SerializeObject(sceneData, SETTINGS);

            // 4. write to file
            File.WriteAllText(path, json);

            return true;
        }
        /*
         * Loads a game object that has a prototype name
         */
        private static GameObject LoadFromPrototype(GameObject gameObject)
        {

            // TODO: Implement this method
            return gameObject;
        }

        public static Scene? LoadScene(string name)
        {
            // TODO: handle prefab loading
            // maybe create special prefab class that contains name of prefab and
            // separate file with prefab data
            
            // 1. prepare path
            string path = "Assets/Scenes/" + name + ".scene";
            if (!File.Exists(path))
            {
                return null;
            }

            // 2. read from file
            string json = File.ReadAllText(path);

            // 3. deserialize
            SceneData sceneData = JsonConvert.DeserializeObject<SceneData>(json, SETTINGS);

            // 4. convert to scene
            Scene scene = new Scene(sceneData.name);
            scene.SetSceneType((SceneType)sceneData.sceneType);
            // TODO: scene.SetCamera(sceneData.mainCamera);

            if(sceneData.gameObjects != null)
                for(int i = 0; i < sceneData.gameObjects.Count; i++)
                {
                    GameObject gameObject = sceneData.gameObjects[i];

                    // if gameObject.prototype != null, load prototype
                    // then replace all attributes, children and components that are also in the gameObject
                    // then add the gameObject to the scene
                    if(gameObject.prototype != null)
                    {
                        gameObject = LoadFromPrototype(gameObject);
                    }
                

                    scene.AddGameObject(gameObject);
                }

            if(sceneData.protoObjects!= null)
                for(int i = 0; i < sceneData.protoObjects.Count; i++)
                {
                    ProtoObject protoObject = sceneData.protoObjects[i];
                    GameObject? gameObject = protoObject.Instantiate();
                    if(gameObject == null)
                    {
                        continue;
                    }

                    scene.AddGameObject(gameObject);
                }

            return scene;

        }

    }
}
