
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
        private int gameObjectsCount = 0;

        private Camera mainCamera;
        private SceneType sceneType = SceneType.GAME;

        private List<GameObject> gameObjects = new();

        private List<Drawable> drawableList = new();
        private List<Collider> colliderList = new();
        private List<Script> scripts = new();

        private List<Script> toStart = new();

        private SortedList<double, EngineObject> toDestroy = new();
        private List<EngineObject> toAdd = new();
        
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

        public string GetName()
        {
            return name;
        }

        public int GetGameObjectsCount()
        {
            return gameObjectsCount;
        }

        public int GetRootGameObjectsCount()
        {
            return gameObjects.Count;
        }


        public Camera GetCamera()
        {
            return mainCamera;
        }

        public void AddGameObject(GameObject gameObject)
        {
            if(gameObject.GetScene() != null)
            {

                Console.WriteLine("WARNING: GameObject already has a scene. This could result in duplicate objects in the scene" +
                    " and unexpected behavior. Make sure to remove the GameObject from the previous scene before adding it to a new scene.");
                return;
            }

            // if the game object has a parent which is not in this scene, dont add it
            GameObject? parent = gameObject.GetParent();
            if (parent != null && parent.GetScene() != this)
            {
                Console.WriteLine("WARNING: GameObject has a parent which is not in this scene. Make sure to add the parent to this scene first.");
                return;
            }

            this.toAdd.Add(gameObject);
            gameObject.SetScene(this);
        }

        public void AddGameObjectComponent(GameObject gameObject, Component component)
        {

            // if the game objectis in toAdd, dont add the component to the lists since it will be added later
            if (toAdd.Contains(gameObject))
            {
                return;
            }

            // if the component is already in toAdd, dont add it again
            if (toAdd.Contains(component))
            {
                return;
            }

            this.toAdd.Add(gameObject);
        }

        private void AddGameObjectComponents(GameObject gameObject)
        {
            List<Component> list = gameObject.GetAllComponents();
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
            // increment game objects count
            this.gameObjectsCount++;
        }

        /*
         * Destroy an EngineObject after a delay
         * Either GameObject or Component
         * 
         * Destruction happens at the end of the frame
         */
        public void Destroy(EngineObject engineObject, double delay = 0)
        {
            this.toDestroy.Add(Time.time + delay, engineObject);
        }

        private void DestroyComponent(Component component, bool removeFromGameObject = true)
        {

            // remove from GameObject
            if (removeFromGameObject)
                component.GetGameObject().RemoveComponent(component);

            switch (component)
            {
                case Drawable drawable:
                    var succ = drawableList.Remove(drawable);
                    Console.WriteLine("Drawable removed: " + succ);
                    break;
                case Collider collider:
                    colliderList.Remove(collider);
                    break;
                case Script script:
                    var success = scripts.Remove(script);
                    if (!success)
                    {
                        // maybe the script was already destroyed
                        // dont call OnDestroy again
                        Console.WriteLine("Script was already destroyed");
                        return;
                    }
                    // call OnDisable and OnDestroy
                    script.OnDisable();
                    script.OnDestroy();
                    break;
            }
        }

        private void HandleDestroy(EngineObject engineObject)
        {
            if (engineObject is GameObject gameObject)
            {
                this.DeepDestroyGameObject(gameObject);
            }
            else if (engineObject is Component component)
            {
                DestroyComponent(component);
            }
        }

        /*
         * Destroy a GameObject, its children, and all of their components
         */
        private void DeepDestroyGameObject(GameObject gameObject, bool removeFromParent = true)
        {
            // TODO: optimize this later?
            // this could be slow if there are many components
            // maybe consider using a better data structure for components
            List<Component> list = gameObject.GetAllComponents();
            for (int i = 0; i < list.Count; i++)
            {
                Component component = list[i];

                DestroyComponent(component, removeFromGameObject: false);
            }

            // remove children components
            List<GameObject> children = gameObject.GetChildren();
            for (int i = 0; i < children.Count; i++)
            {
                DeepDestroyGameObject(children[i], removeFromParent: false);
            }

            // remove this game object from its parent if it has one
            GameObject? parent = gameObject.GetParent();
            if (parent != null && removeFromParent)
            {
                parent.GetChildren().Remove(gameObject);
                gameObject.SetParent(null);
            }

            // remove this game object from the scene if it was a root game object
            if(parent == null)
            {
                this.gameObjects.Remove(gameObject);
            }

            this.gameObjectsCount--;
        }

        public List<GameObject> GetGameObjects()
        {
            return gameObjects;
        }

        private void HandleAddComponent<T>(T component) where T : Component
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

        // Iterate through all Drawable components and call their Draw method using the main camera defined in the scene
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
            foreach (EngineObject engineObject in toAdd)
            {
                if (engineObject is GameObject gameObject)
                {
                    // only add the game object if its a root
                    if (gameObject.GetParent() == null)
                    {
                        this.gameObjects.Add(gameObject);
                    }
                    else
                    {
                        // otherwise we assume that the parent(or its parent) is already in the list
                        // TODO: add a check for this
                    }
                    AddGameObjectComponents(gameObject);
                } else if (engineObject is Component component)
                {
                    HandleAddComponent(component);
                }
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
            // also check if the script was enabled or disabled
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

            // late update
            foreach (Script script in scripts)
            {
                if (script.IsEnabled())
                {
                    script.LateUpdate();
                }
            }

            // remove game objects that are scheduled to be removed
            if (toDestroy.Count > 0)
            {
                double firstKey = toDestroy.Keys[0];
                while (firstKey <= Time.time)
                {
                    HandleDestroy(toDestroy[firstKey]);
                    toDestroy.RemoveAt(0);
                    if (toDestroy.Count == 0)
                    {
                        break;
                    }
                    firstKey = toDestroy.Keys[0];
                }
            }

        }

    }

    /*
     * SceneManager
     * This class is responsible for managing scenes
     * Handles loading, unloading, and switching between scenes
     * When a scene is updated or rendered, it is set as the active scene so that new GameObjects can be added to it automatically
     * 
     * Also has a list of persistent game objects that are not destroyed when a scene is unloaded
     * These GameObjects can be accessed from a following scene, for example, a player GameObject
     */
    public static class SceneManager
    {
        private static Scene? activeScene;

        private static List<Scene> scenes = new();

        private static List<GameObject> persistentGameObjects = new();

        public static Scene? GetActiveScene()
        {
            return activeScene;
        }

        public static List<Scene> GetScenes()
        {
            return scenes;
        }

        private static void SetActiveScene(Scene scene)
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
