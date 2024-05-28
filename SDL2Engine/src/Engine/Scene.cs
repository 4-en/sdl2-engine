
using System.Collections;

namespace SDL2Engine
{

    public enum SceneType
    {
        GAME = 0,
        UI = 1,
        MENU = 2,
        LOADING = 3
    }

    /*
     * This fixes the issue with having duplicate keys in a SortedList
     * Probably really slow, but it works for now
     * TODO: optimize this later
     */
    class TimedQueue<T> : IEnumerable<T> where T : class
    {
        class NodeValue
        {
            public double key;
            public T value;
            public NodeValue(double key, T value)
            {
                this.key = key;
                this.value = value;
            }
        }

        private LinkedList<NodeValue> list = new();

        public void Add(double key, T value)
        {
            // TODO: optimize this later
            NodeValue nodeValue = new(key, value);
            LinkedListNode<NodeValue>? node = list.First;
            while (node != null)
            {
                if (key <= node.Value.key)
                {
                    list.AddBefore(node, nodeValue);
                    return;
                }
                node = node.Next;
            }
            list.AddLast(nodeValue);
        }

        public void AddBackwards(double key, T value)
        {
            NodeValue nodeValue = new(key, value);
            LinkedListNode<NodeValue>? node = list.Last;
            while (node != null)
            {
                if (key > node.Value.key)
                {
                    list.AddAfter(node, nodeValue);
                    return;
                }
                node = node.Previous;
            }
            list.AddFirst(nodeValue);
        }

        public T? PopBefore(double key)
        {
            LinkedListNode<NodeValue>? node = list.First;
            if (node == null)
            {
                return default;
            }
            if (node.Value.key <= key)
            {
                T value = node.Value.value;
                list.RemoveFirst();
                return value;
            }
            return null;
        }

        public void Clear()
        {
            list.Clear();
        }

        public int Count()
        {
            return list.Count;
        }

        public T? Peek()
        {
            if (list.Count == 0)
            {
                return null;
            }
            return list.First?.Value.value ?? null;
        }

        public IEnumerator<T> GetEnumerator()
        {
            List<T> values = new();
            foreach (NodeValue nodeValue in list)
            {
                values.Add(nodeValue.value);
            }
            return values.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }


    public class Scene : ILoadable
    {

        class ActiveSceneManager : IDisposable
        {
            private Scene? originalScene;
            private Scene tempScene;

            public ActiveSceneManager(Scene scene)
            {
                this.originalScene = SceneManager.GetActiveScene();
                this.tempScene = scene;
                SceneManager.SetActiveScene(scene);
            }

            public void Dispose()
            {
                SceneManager.SetActiveScene(originalScene);
            }
        }

        // TODO: use something like this to limit the number of new objects per frame
        public static readonly uint MAX_ADDS_PER_FRAME = 1;

        private string name = "Scene";
        private int gameObjectsCount = 0;
        private bool doPhysics = true;

        private Camera mainCamera;
        private SceneType sceneType = SceneType.GAME;

        private List<GameObject> gameObjects = new();

        private List<Drawable> drawableList = new();
        private List<Collider> colliderList = new();
        private List<Script> scripts = new();

        private List<Script> toStart = new();

        private TimedQueue<EngineObject> toDestroy = new();
        private HashSet<EngineObject> toAdd = new();
        private List<GameObject> toRemove = new();

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

        public IDisposable Activate()
        {
            return new ActiveSceneManager(this);
        }

        public int GetGameObjectsCount()
        {
            return gameObjectsCount;
        }

        public int GetRootGameObjectsCount()
        {
            return gameObjects.Count;
        }

        public int GetDrawableCount()
        {
            return drawableList.Count;
        }

        public int GetScriptCount()
        {
            return scripts.Count;
        }

        public int GetColliderCount()
        {
            return colliderList.Count;
        }

        public Camera GetCamera()
        {
            return mainCamera;
        }

        public void AddGameObject(GameObject gameObject)
        {
            if (gameObject.GetScene() != null && gameObject.GetScene() != this)
            {
                /*
                Console.WriteLine("WARNING: GameObject already has a scene. This could result in duplicate objects in the scene" +
                    " and unexpected behavior. Make sure to remove the GameObject from the previous scene before adding it to a new scene.");
                return;
                */
                throw new Exception("GameObject already has a scene");
            }

            // if the game object has a parent which is not in this scene, dont add it
            GameObject? parent = gameObject.GetParent();
            if (parent != null && parent.GetScene() != this)
            {
                /*
                Console.WriteLine("WARNING: GameObject has a parent which is not in this scene. Make sure to add the parent to this scene first.");
                return;
                */
                throw new Exception("GameObject has a parent which is not in this scene");
            } else if (parent != null)
            {
                // check if parent is in the toAdd list
                GameObject? deepParent = parent.GetDeepParent();
                if (toAdd.Contains(parent) || (deepParent != null && toAdd.Contains(deepParent)))
                {
                    // if the parent is in the toAdd list, we can assume that it will be added later
                    // in this case, we don't need to add this child to the list as well
                    gameObject.SetScene(this);
                    return;
                }

            }

            this.toAdd.Add(gameObject);
            gameObject.SetScene(this);
        }

        public void AddComponent(Component component)
        {

            var gameObject = component.GetGameObject();
            if(gameObject == null || gameObject == GameObject.Default)
            {
                Console.WriteLine("WARNING: Component has no GameObject. Make sure to add the component to a GameObject before adding it to a scene.");
                return;
            }

            if (gameObject.GetScene() != null && gameObject.GetScene() != this)
            {
                Console.WriteLine("WARNING: GameObject is in another scene. Make sure to remove the GameObject from the previous scene before adding it to a new scene.");
                return;
            }
            component.SetScene(this);

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

            this.toAdd.Add(component);
        }

        private void HandleAddGameObjectComponents(GameObject gameObject)
        {
            // check if the game object is already in the scene
            if (gameObject.GetScene() == null)
            {
                gameObject.SetScene(this);
            }
            else if (gameObject.GetScene() != this)
            {
                Console.WriteLine("WARNING: GameObject is in another scene. Make sure to remove the GameObject from the previous scene before adding it to a new scene.");
                return;
            }

            List<Component> list = gameObject.GetAllComponents();
            for (int i = 0; i < list.Count; i++)
            {
                Component component = list[i];

                HandleAddComponent(component);
            }

            // add children components
            List<GameObject> children = gameObject.GetChildren();
            for (int i = 0; i < children.Count; i++)
            {
                HandleAddGameObjectComponents(children[i]);
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
            if (engineObject.ToBeDestroyed()) return;
            engineObject.MarkToBeDestroyed();

            if (delay == 0)
            {
                // if delay is 0, we can assume that the object will be one of the first in the queue
                this.toDestroy.Add(Time.time + delay, engineObject);
            }
            else
            {
                // if delay is not 0, we need to find the correct position in the queue
                // since most objects will be destroyed with a delay of 0, we can optimize this by starting at the end of the queue
                this.toDestroy.AddBackwards(Time.time + delay, engineObject);
            }
        }

        private void DestroyComponent(Component component, bool removeFromGameObject = true)
        {

            // remove from GameObject
            if (removeFromGameObject) { 
                component.GetGameObject().RemoveComponent(component);
                // since a component without a game object has no further use, we should dispose of its resources
                component.Dispose();
            }

            component._clear_scene_on_destroy();


            switch (component)
            {
                case Drawable drawable:
                    var succ = drawableList.Remove(drawable);
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

        public void RemoveGameObjectFromRoot(GameObject gameObject)
        {
            if (gameObject.GetScene() != this)
            {
                Console.WriteLine("WARNING: GameObject is not in this scene. Make sure to remove the GameObject from the correct scene.");
                return;
            }

            if (gameObject.GetParent() == null)
            {
                Console.WriteLine("WARNING: GameObject has no parent and should not be removed from the root. To remove a GameObject from the scene, use Destroy.");
                return;
            }

            this.toRemove.Add(gameObject);
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

        // Loads GameObjects from a template file and adds them to the scene
        public List<GameObject> LoadTemplate(string path)
        {
            Scene? tempScene = SceneManager.GetActiveScene();
            SceneManager.SetActiveScene(this);
            List<GameObject> gameObjects = SceneTemplate.Load(path);
            SceneManager.SetActiveScene(tempScene);
            return gameObjects;
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

            if (gameObject.GetScene() != this)
            {
                Console.WriteLine("WARNING: GameObject is not in this scene. Make sure to remove the GameObject from the correct scene.");
                return;
            }
            else
            {
                this.gameObjectsCount--;
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
            if (parent == null)
            {
                this.gameObjects.Remove(gameObject);
            }

            gameObject._clear_scene_on_destroy();

            // if the object is marked as persistent, add it to the persistent list in the scene manager
            if (gameObject.IsPersistent())
            {
                // TODO: there might be a bug here where a non persisten parent of a persistent object is destroyed
                // the parent could call Dispose on the persistent object
                // maybe fix this later :)
                // maybe non root objects should not be able to be persistent
                SceneManager.GetPersistentGameObjects().Add(gameObject);
            } else if (removeFromParent)
            {
                // if it is not persistent, we can dispose of it if this was the original
                // object Destroy was called on
                // otherwise, the parent will handle the disposal
                // by calling Dispose recursively on its children and components
                gameObject.Dispose();
            }
            
        }

        public List<GameObject> GetGameObjects()
        {
            return gameObjects;
        }

        public List<Drawable> GetDrawables()
        {
            return drawableList;
        }

        public List<Collider> GetColliders()
        {
            return colliderList;
        }

        /*
         * Find a GameObject by name
         * Returns null if no GameObject with the given name was found
         */
        public GameObject? Find(string name)
        {
            // first check the root game objects
            foreach (GameObject gameObject in gameObjects)
            {
                if (gameObject.GetName() == name)
                {
                    return gameObject;
                }
            }
            
            // if not found, do a deep search
            foreach (GameObject gameObject in gameObjects)
            {
                GameObject? found = gameObject.FindChild(name);
                if (found != null)
                {
                    return found;
                }
            }
            return null;
        }

        public T? FindComponent<T>() where T : Component
        {
            T? component = null;
            
            // first check components of root game objects
            foreach (GameObject gameObject in gameObjects)
            {
                component = gameObject.GetComponent<T>();
                if (component != null)
                {
                    return component;
                }
            }

            // if not found, do a deep search
            foreach (GameObject gameObject in gameObjects)
            {
                component = gameObject.FindComponentInChildren<T>();
                if (component != null)
                {
                    return component;
                }
            }
            return null;
        }

        private void HandleAddComponent<T>(T component) where T : Component
        {
            // add to scene if not already added
            if (component.GetScene() != this)
            {
                component.SetScene(this);
            }

            // Call Awake on the component
            component.Awake();

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
                    toStart.Add(script);
                    break;
            }
        }

        public void SetPhysics(bool doPhysics)
        {
            this.doPhysics = doPhysics;
        }

        // Iterate through all Drawable components and call their Draw method using the main camera defined in the scene
        public void Draw()
        {
            // TODO: optimize this later
            // TODO: sort the drawable list by depth (Vec2D.z)
            // also use occlusion- and frustum culling (at least frustum culling)
            // aaalso, sort objects of the same depth by their texture, so gpu can batch draw calls
            // the sorting should not be done every frame, but only when a drawable is added or removed (and then only once before rendering)
            foreach (Drawable drawable in drawableList)
            {
                if (drawable.IsEnabled())
                    drawable.Draw(mainCamera);
            }
        }

        public void Update()
        {

            // TODO: optimize this later
            // (this seems horrible tbh)
            // add game objects that are scheduled to be added
            //var alreadyAdded = new HashSet<GameObject>();
            //var get_parent_count = (GameObjects go) => go.GetParent() == null ? 0 : 1 + get_parent_count(go.GetParent());
            // sort the game objects by their depth, lowest depth first

            // another option to solve this would be to keep a seperate list of all engine objects that are already in the scene (HashSet)
            // then we could just check if the object is already in the scene
            // the disadvantage  of this would be that we would have to keep track of all objects in the scene
            // this list would also change a lot, since we are adding and removing objects all the time
            // when just doing something based on the toAdd list, we probably only have to check a few objects, if any
            

            foreach (EngineObject engineObject in toAdd)
            {
                if (engineObject is GameObject gameObject)
                {
                    GameObject? parent = gameObject.GetParent();
                    // only add the game object if its a root
                    if (parent == null)
                    {
                        this.gameObjects.Add(gameObject);
                        HandleAddGameObjectComponents(gameObject);
                        //alreadyAdded.Add(gameObject);
                    }
                    else
                    {
                        // otherwise we assume that the parent(or its parent) is already in the list
                        // TODO: add a check for this

                        // since AddGameObjectComponents will recursively add all components,
                        // even of children, we could double add components if we add a parent and a child
                        // in the same frame
                        // if the child is added at a later frame, this is fine, since it hasn't been part
                        // of the parent when the parent was added, and therefore its components haven't been added
                        // GENERALLY, the parent would be added first (even during the same frame), so we are only checking
                        // if the parent has already been added
                        // this could be an issue if we create the child, then the parent, then add the child to the parent

                        // we also have to check every parents parent and so on...
                        // this is a bit slow, but it should work for now
                        bool parentAdded = false;
                        while (parent != null)
                        {
                            if (toAdd.Contains(parent))
                            {
                                parentAdded = true;
                                break;
                            }
                            parent = parent.GetParent();
                        }

                        if (!parentAdded)
                        {
                            HandleAddGameObjectComponents(gameObject);
                            //alreadyAdded.Add(gameObject);
                        }
                    }
                    
                }
                else if (engineObject is Component component)
                {
                    // same issue as above with child game objects
                    // we have to make sure the owning game object is not added during the same frame
                    GameObject? componentsGameObject = component.GetGameObject();
                    bool parentAdded = false;
                    while (componentsGameObject != null)
                    {
                        if (toAdd.Contains(componentsGameObject))
                        {
                            parentAdded = true;
                            break;
                        }
                        componentsGameObject = componentsGameObject.GetParent();
                    }
                    
                    if (!parentAdded)
                    {
                        HandleAddComponent(component);
                    }
                }
            }
            toAdd.Clear();

            // Physics
            // get all game objects with colliders
            if (doPhysics)
            { 
                List<GameObject> goWithPhysics = new();
                foreach (Collider collider in colliderList)
                {
                    // TODO: this might cause problems if we add a parent game object and a child game object
                    // maybe fix this later
                    goWithPhysics.Add(collider.GetGameObject());
                }
                Physics.UpdatePhysics(goWithPhysics);
            }
            

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
                if (script.IsEnabled())
                {
                    if (!script.wasEnabled)
                        script.OnEnable();
                    script.wasEnabled = true;
                    script.Update();
                }
                else
                {
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
            if (toDestroy.Count() > 0)
            {
                double time = Time.time;
                EngineObject? engineObject = toDestroy.PopBefore(time);

                while (engineObject != null)
                {
                    HandleDestroy(engineObject);
                    engineObject = toDestroy.PopBefore(time);
                }
            }

            // remove game objects from the root list
            // this assumes they are now a child of another game object,
            // therefore they are still part of the scene
            if (toRemove.Count > 0)
            {
                foreach (GameObject gameObject in toRemove)
                {
                    gameObjects.Remove(gameObject);
                }
                toRemove.Clear();
            }

        }

        public void Load()
        {
            // TODO: implement this (load all resources)
            // this should call Load on all ILoadable objects in the scene before updating and rendering
            // to keep the game running smoothly, only update up to a certain amount of objects per frame
            // this could be done in the background while another scene is running
            // the scene manager should handle this and can use IsLoaded to check if the scene is ready to rendered
            return;
        }

        // This loads the next GameObject in queue
        public void LoadNext()
        { }

        // This loads n GameObjects in queue
        public void Load(int n)
        {
            // load n objects
            // this should be called in the background while another scene is running
            // the scene manager should handle this and can use IsLoaded to check if the scene is ready to rendered
            return;
        }

        public bool IsLoaded()
        {
            // TODO: implement this
            // this should return true if all resources are loaded
            // after this, the scene can be updated and rendered
          
            return true;
        }

        public void Dispose()
        {
            // this should only be called when the scene is removed from the scene manager
            // (hopefully) remove all game objects and components and calls their Dispose methods
            for(int i = gameObjects.Count - 1; i >= 0; i--)
            {
                DeepDestroyGameObject(gameObjects[i]);
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

        private static List<Scene> toAdd = new();
        private static List<Scene> toRemove = new();

        public static Scene? GetActiveScene()
        {
            return activeScene;
        }

        public static List<Scene> GetScenes()
        {
            return scenes;
        }

        // this is used internally to set the active scene
        // that is currently being updated and rendered
        // this is used to add new GameObjects to the current scene
        internal static void SetActiveScene(Scene? scene)
        {
            activeScene = scene;
        }

        public static void DrawScenes()
        {
            for (int i = 0; i < scenes.Count; i++)
            {
                SetActiveScene(scenes[i]);
                scenes[i].Draw();
            }
        }

        private static void AddRemoveScenes()
        {
            for (int i = 0; i < toAdd.Count; i++)
            {
                if (scenes.Contains(toAdd[i]))
                {
                    Console.WriteLine("WARNING: Scene is already in the scene list");
                    continue;
                }
                toAdd[i].Load();
                scenes.Add(toAdd[i]);
            }
            toAdd.Clear();

            for (int i = 0; i < toRemove.Count; i++)
            {
                scenes.Remove(toRemove[i]);
                if (activeScene == toRemove[i])
                {
                    activeScene = null;
                }
            }
            toRemove.Clear();
        }

        public static void UpdateScenes()
        {
            AddRemoveScenes();

            for (int i = 0; i < scenes.Count; i++)
            {
                SetActiveScene(scenes[i]);
                scenes[i].Update();
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

        /*
         * Replaces the current scene with a new scene
         * Clears all other scenes
         */
        public static void SetScene(Scene scene)
        {
            for (int i = scenes.Count - 1; i >= 0; i--)
            {
                RemoveScene(scenes[i]);
            }

            AddScene(scene);
        }

        public static void AddScene(Scene scene)
        {
            toAdd.Add(scene);
        }

        public static void LoadScene(Scene scene)
        {
            toAdd.Add(scene);
        }

        public static void AddBefore(Scene scene, Scene before)
        {
            // TODO: implement this
            //scene.Load();
            //scenes.Insert(scenes.IndexOf(before), scene);
            AddScene(scene);
        }

        public static void AddAfter(Scene scene, Scene after)
        {
            // TODO: implement this
            //scene.Load();
            //scenes.Insert(scenes.IndexOf(after) + 1, scene);
            AddScene(scene);
        }

        public static void RemoveScene(Scene scene)
        {
            toRemove.Add(scene);
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
