using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;
using System.Reflection;

namespace SDL2Engine
{
    internal class SceneSerialization
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

                prop.ShouldSerialize = instance =>
                {
                    return false;
                };


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
        }

        private readonly static JsonSerializerSettings SETTINGS = new JsonSerializerSettings
        {
            TypeNameHandling = TypeNameHandling.All,
            PreserveReferencesHandling = PreserveReferencesHandling.Objects,
            Formatting = Formatting.Indented,
            NullValueHandling = NullValueHandling.Include,
            ContractResolver = new EngineContractResolver()
        };

        public static bool SaveScene(Scene scene)
        {
            // TODO: Implement this method

            // 1. convert to scene data
            SceneData sceneData = new SceneData
            {
                name = scene.GetName(),
                sceneType = (int)scene.GetSceneType(),
                mainCamera = scene.GetCamera(),
                gameObjects = scene.GetGameObjects()
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

        public static Scene? LoadScene(string name)
        {
            // TODO: Implement this method
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

            for(int i = 0; i < sceneData.gameObjects.Count; i++)
            {
                GameObject gameObject = sceneData.gameObjects[i];
                scene.AddGameObject(gameObject);
            }

            return scene;

        }

    }
}
