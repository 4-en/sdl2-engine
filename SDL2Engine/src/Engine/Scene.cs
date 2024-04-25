
namespace SDL2Engine
{

    public enum SceneType
    {
        GAME = 0,
        UI = 1,
        MENU = 2,
        LOADING = 3
    }

    public class Scene
    {
        private string name = "Scene";

        private Camera mainCamera;
        private SceneType sceneType = SceneType.GAME;

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

        public SceneType GetSceneType()
        {
            return sceneType;
        }

        public void SetSceneType(SceneType sceneType)
        {
            this.sceneType = sceneType;
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

        public void AddGameObjectComponent(GameObject gameObject, Component component)
        {

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
            // TODO: optimize this later?
            // this could be slow if there are many components
            // maybe consider using a better data structure for components
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
            foreach (Drawable drawable in drawableList)
            {
                drawable.Draw(mainCamera);
            }
        }

        public void Update()
        {

            // add game objects that are scheduled to be added
            foreach (GameObject gameObject in toAdd)
            {
                // only add the game object if its a root
                if (gameObject.GetParent() == null)
                { 
                    this.gameObjects.Add(gameObject);
                }
                AddGameObjectComponents(gameObject);
            }
            toAdd.Clear();

            // Physics
            // get all game objects with colliders
            List<GameObject> goWithPhysics = new();
            foreach (Collider collider in colliderList)
            {
                // TODO: this might cause problems if we add a parent game object and a child game object
                // maybe fix this later
                goWithPhysics.Add(collider.GetGameObject());
            }
            Physics.UpdatePhysics(goWithPhysics);


            // start scripts that are scheduled to be started
            foreach (Script script in toStart)
            {
                script.Start();
            }
            toStart.Clear();

            // call update on all scripts
            foreach (Script script in scripts)
            {
                if (script.IsEnabled()) {
                    if (!script.wasEnabled)
                        script.OnEnable();
                        script.wasEnabled = true;
                    script.Update();
                } else {
                    if (script.wasEnabled)
                        script.OnDisable();
                        script.wasEnabled = false;
                }
            }

            // remove game objects that are scheduled to be removed
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

        private static List<GameObject> persistentGameObjects = new();

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

            foreach (GameObject gameObject in persistentGameObjects)
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

        public static void SortScenesBySceneType()
        {
            scenes.Sort((a, b) => a.GetSceneType().CompareTo(b.GetSceneType()));
        }

        public static List<GameObject> GetPersistentGameObjects()
        {
            return persistentGameObjects;
        }

        public static void SetSceneOrder(Scene scene, int index)
        {
            scenes.Remove(scene);
            scenes.Insert(index, scene);
        }




    }
}
