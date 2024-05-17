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
        private string name;
        private GameObject gameObject;
        // private GameObject previousState;

        public Prototype(string name)
        {
            this.name = name;
            this.gameObject = new GameObject(true, name);
            // this.previousState = gameObject.Clone();
        }

        public Prototype(string name, GameObject gameObject)
        {
            this.name = name;
            this.gameObject = gameObject;
        }

        public GameObject Instantiate()
        {
            var new_instance = gameObject.Clone();

            // add current scene if it exists
            Scene? activeScene = SceneManager.GetActiveScene();
            if (activeScene != null)
            {
                activeScene.AddGameObject(new_instance);
            }
            return new_instance;
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
}
