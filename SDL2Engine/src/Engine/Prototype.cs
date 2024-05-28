using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SDL2Engine
{
    /* This class is used to create prototypes of game objects that can be instantiated
     * at runtime. This is useful for creating objects that are used multiple times
     * in the game, but are not necessarily the same object. For example, a bullet
     * object that is used multiple times in the game, but each bullet is a different
     * object. 
     */

    [Serializable]
    public class Prototype
    {
        [JsonProperty]
        private string name;
        [JsonProperty]
        private GameObject gameObject;
        // private GameObject previousState;

        [JsonProperty]
        private List<string> child_prototypes;

        public GameObject GameObject { get => gameObject; set => gameObject = value; }

        protected Prototype()
        {
            this.name = "";
            this.gameObject = new GameObject(true, "");
            this.child_prototypes = new List<string>();
        }

        public Prototype(string name)
        {
            this.name = name;
            this.gameObject = new GameObject(true, name);
            this.child_prototypes = new List<string>();
            // this.previousState = gameObject.Clone();

            this.RegisterPrototype();
        }

        public Prototype(string name, GameObject gameObject)
        {
            this.name = name;
            this.gameObject = gameObject;
            this.child_prototypes = new List<string>();

            this.RegisterPrototype();
        }

        public static GameObject? Instantiate(string prototypeName)
        {
            Prototype? proto = AssetManager.LoadPrototype(prototypeName).Get();
            if (proto == null)
            {
                return null;
            }
            return proto.Instantiate();
        }

        public GameObject Instantiate()
        {
            var new_instance = GameObject.Clone();

            // add current scene if it exists
            Scene? activeScene = SceneManager.GetActiveScene();
            if (activeScene != null)
            {
                activeScene.AddGameObject(new_instance);
            }
            return new_instance;
        }

        public GameObject InstantiateWithoutScene()
        {
            return GameObject.Clone();
        }

        // Adds this Prototype to the AssetManager,
        // allowing it be queried by name
        public bool RegisterPrototype()
        {
            if(this.name == "")
            {
                throw new Exception("Prototype name cannot be empty when registering prototype");
            }
            return AssetManager.AddPrototype(this);
        }

        

        public string GetName()
        {
            return name;
        }

        // TODO: implement this method
        // (optional)
        private void ApplyChanges()
        {
            // find differences between previous state and current state
            // apply changes to all instances of this prototype

        }
    }

    [Serializable]
    public class ProtoObject
    {
        public string prototypeName;
        public Dictionary<string, object> properties = new Dictionary<string, object>();

        private ProtoObject()
        {
            this.prototypeName = "";
        }

        public ProtoObject(string prototypeName)
        {
            this.prototypeName = prototypeName;
        }

        public void AddProperty(string key, object value)
        {
            properties.Add(key, value);
        }

        public GameObject? Instantiate()
        {
            Prototype? proto = AssetManager.LoadPrototype(prototypeName).Get();
            if (proto == null)
            {
                return null;
            }

            GameObject obj = proto.InstantiateWithoutScene();

            // only allow custom properties for now

            // position
            if (properties.ContainsKey("position"))
            {
                // get x and y values (and z if it exists, but it's optional)
                // obj to list of doubles
                // Newtonsoft.Json.Linq.JArray -> List<double>
                var newtonArray = (Newtonsoft.Json.Linq.JArray)properties["position"];
                List<double>? pos = newtonArray.ToObject<List<double>>();
                if (pos == null)
                {
                    throw new Exception("Invalid value for position property");
                }
                int len = pos.Count;
                switch(len)
                {
                    case 2:
                        obj.SetPosition(new Vec2D(pos[0], pos[1]));
                        break;
                    case 3:
                        obj.SetPosition(new Vec2D(pos[0], pos[1], pos[2]));
                        break;
                    default:
                        throw new Exception("Invalid number of values for position property");
                }
            }

            // TODO: add more properties

            return obj;
        }
    }
}
