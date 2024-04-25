
namespace SDL2Engine
{
    public class Scene
    {
        private string name = "Scene";

        private Camera mainCamera;

        private List<GameObject> gameObjects = new();

        private List<Drawable> drawableList = new();
        private List<Collider> colliderList = new();
        private List<Script> scripts = new();

        private List<Script> toStart = new();

        private List<GameObject> toRemove = new();
        private List<GameObject> toAdd = new();
        
        public Scene()
        {
            this.mainCamera = new Camera2D(new Vec2D());
        }
        public Scene(Camera camera)
        {
            this.mainCamera = camera;
        }

        public Scene(string name)
        {
            this.mainCamera = new Camera2D(new Vec2D());
            this.name = name;
        }


        public Camera GetCamera()
        {
            return mainCamera;
        }

        public void AddGameObject(GameObject gameObject)
        {
            this.toAdd.Add(gameObject);
            gameObject.SetScene(this);
        }

        private void AddGameObjectComponents(GameObject gameObject)
        {
            List<Component> list = gameObject.GetComponents<Component>();
            for (int i = 0; i < list.Count; i++) {
                Component component = list[i];

                switch (component)
                {
                    case Drawable drawable:
                        drawableList.Add(drawable);
                        break;
                    case Collider collider:
                        colliderList.Add(collider);
                        break;
                    case Script script:
                        scripts.Add(script);
                        toStart.Add(script);
                        break;
                }
            }

            // add children components
            List<GameObject> children = gameObject.GetChildren();
            for (int i = 0; i < children.Count; i++)
            {
                AddGameObjectComponents(children[i]);
            }
        }

        public void RemoveGameObject(GameObject gameObject)
        {
            this.toRemove.Add(gameObject);
        }

        public void RemoveGameObjectComponents(GameObject gameObject)
        {
            List<Component> list = gameObject.GetComponents<Component>();
            for (int i = 0; i < list.Count; i++)
            {
                Component component = list[i];

                switch (component)
                {
                    case Drawable drawable:
                        drawableList.Remove(drawable);
                        break;
                    case Collider collider:
                        colliderList.Remove(collider);
                        break;
                    case Script script:
                        scripts.Remove(script);
                        break;
                }
            }

            // remove children components
            List<GameObject> children = gameObject.GetChildren();
            for (int i = 0; i < children.Count; i++)
            {
                RemoveGameObjectComponents(children[i]);
            }
        }

        public List<GameObject> GetGameObjects()
        {
            return gameObjects;
        }

        public void ActivateComponent<T>(T component) where T : Component
        {
            // Store some component types in lists for quick access
            switch (component)
            {
                case Drawable drawable:
                    drawableList.Add(drawable);
                    break;
                case Collider collider:
                    colliderList.Add(collider);
                    break;
                case Script script:
                    scripts.Add(script);
                    break;
            }
        }

        public void DeactivateComponent<T>(T component) where T : Component
        {
            // Remove some component types from lists
            switch (component)
            {
                case Drawable drawable:
                    drawableList.Remove(drawable);
                    break;
                case Collider collider:
                    colliderList.Remove(collider);
                    break;
                case Script script:
                    scripts.Remove(script);
                    break;
            }
        }

        public void Draw()
        {
            // TODO: draw only drawables
            foreach (GameObject gameObject in gameObjects)
            {
                gameObject.Draw(mainCamera);
            }
        }

        public void Update()
        {
            foreach (GameObject gameObject in toAdd)
            {
                this.gameObjects.Add(gameObject);
                AddGameObjectComponents(gameObject);
            }
            toAdd.Clear();

            // TODO: update only scripts
            foreach (GameObject gameObject in gameObjects)
            {
                gameObject.Update();
            }

            foreach (GameObject gameObject in toRemove)
            {
                this.gameObjects.Remove(gameObject);
                RemoveGameObjectComponents(gameObject);
            }
            toRemove.Clear();
        }

    }

    public static class SceneManager
    {
        private static Scene? activeScene;

        private static List<Scene> scenes = new();

        private static Scene persistentScene = new Scene("Persistent");

        public static Scene? GetActiveScene()
        {
            return activeScene;
        }

        public static void SetActiveScene(Scene scene)
        {
            activeScene = scene;
        }

        public static void DrawScenes()
        {
            for (int i = 0; i < scenes.Count; i++)
            {
                activeScene = scenes[i];
                scenes[i].Draw();
                activeScene = null;
            }
        }

        public static void UpdateScenes()
        {
            for (int i = 0; i < scenes.Count; i++)
            {
                activeScene = scenes[i];
                scenes[i].Update();
                activeScene = null;
            }
        }

        public static GameObject? Find(string name)
        {

            foreach (GameObject gameObject in persistentScene.GetGameObjects())
            {
                if (gameObject.GetName() == name)
                {
                    return gameObject;
                }
            }

            foreach (Scene scene in scenes)
            {
                foreach (GameObject gameObject in scene.GetGameObjects())
                {
                    if (gameObject.GetName() == name)
                    {
                        return gameObject;
                    }
                }
            }
            return null;
        }

        public static void LoadScene(Scene scene)
        {
            scenes.Clear();
            scenes.Add(scene);
        }

        public static void AddScene(Scene scene)
        {
            scenes.Add(scene);
        }

        public static void AddBefore(Scene scene, Scene before)
        {
            scenes.Insert(scenes.IndexOf(before), scene);
        }

        public static void AddAfter(Scene scene, Scene after)
        {
            scenes.Insert(scenes.IndexOf(after) + 1, scene);
        }

        public static void RemoveScene(Scene scene)
        {
            scenes.Remove(scene);
        }

        public static Scene GetPersistentScene()
        {
            return persistentScene;
        }

        public static void SetSceneOrder(Scene scene, int index)
        {
            scenes.Remove(scene);
            scenes.Insert(index, scene);
        }




    }
}
