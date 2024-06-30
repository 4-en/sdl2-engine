

using SDL2Engine.Coro;
using System.Runtime.CompilerServices;

namespace SDL2Engine
{

    public class ChunkMap
    {
        private int chunkSize = 128;
        private int gameObjectCount = 0;

        private Dictionary<string, List<GameObject>> chunks = new Dictionary<string, List<GameObject>>();

        private int lastChunkX1 = int.MinValue;
        private int lastChunkY1 = int.MinValue;
        private int lastChunkX2 = int.MinValue;
        private int lastChunkY2 = int.MinValue;


        public ChunkMap()
        {
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private string GetChunkKey(int x, int y)
        {
            return $"{x}_{y}";
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void AddGameObject(GameObject gameObject)
        {
            Vec2D position = gameObject.GetPosition();
            int chunkX = (int)position.x / chunkSize;
            int chunkY = (int)position.y / chunkSize;

            string key = GetChunkKey(chunkX, chunkY);
            if (!chunks.ContainsKey(key))
            {
                chunks[key] = new List<GameObject>();
            }
            chunks[key].Add(gameObject);

            gameObjectCount++;
        }

        public void RemoveGameObject(GameObject gameObject)
        {
            Vec2D position = gameObject.GetPosition();
            int chunkX = (int)position.x / chunkSize;
            int chunkY = (int)position.y / chunkSize;

            string key = GetChunkKey(chunkX, chunkY);
            if (chunks.ContainsKey(key))
            {
                chunks[key].Remove(gameObject);
                gameObjectCount--;
            }
        }

        public List<GameObject> GetGameObjectsInChunk(int x, int y)
        {
            string key = GetChunkKey(x, y);
            if (chunks.ContainsKey(key))
            {
                return chunks[key];
            }
            return new List<GameObject>();
        }

        public IEnumerator<GameObject> LoadInBounds(Rect bounds)
        {
            int x1 = (int)bounds.x / chunkSize;
            int y1 = (int)bounds.y / chunkSize;
            int x2 = (int)(bounds.x + bounds.w) / chunkSize;
            int y2 = (int)(bounds.y + bounds.h) / chunkSize;

            if(x1 == lastChunkX1 && y1 == lastChunkY1 && x2 == lastChunkX2 && y2 == lastChunkY2)
            {
                yield break;
            }
            List<string> clearedChunks = new List<string>();
            for(int x = x1; x <= x2; x++)
            {
                if(x>=lastChunkX1 && x<=lastChunkX2)
                {
                    //continue;
                }
                for (int y = y1; y <= y2; y++)
                {
                    if (y >= lastChunkY1 && y <= lastChunkY2)
                    {
                        //continue;
                    }
                    string key = GetChunkKey(x, y);
                    if (chunks.ContainsKey(key))
                    {
                        foreach (GameObject gameObject in chunks[key])
                        {
                            yield return gameObject;
                        }

                        int removedCount = chunks[key].Count;

                        gameObjectCount -= removedCount;

                        // remove the chunk
                        clearedChunks.Add(key);

                    }
                }
            }

            foreach(string key in clearedChunks)
            {
                chunks[key].Clear();
                chunks.Remove(key);
            }

            lastChunkX1 = x1;
            lastChunkY1 = y1;
            lastChunkX2 = x2;
            lastChunkY2 = y2;
        }

        public Rect FitToChunkGrid(Rect bounds)
        {
            int x1 = (int)bounds.x / chunkSize;
            int y1 = (int)bounds.y / chunkSize;
            int x2 = (int)(bounds.x + bounds.w) / chunkSize;
            int y2 = (int)(bounds.y + bounds.h) / chunkSize;

            return new Rect(x1 * chunkSize, y1 * chunkSize, (x2 - x1+1) * chunkSize, (y2 - y1+1) * chunkSize);

        }

        public int Count { get => gameObjectCount;}
    }


    /*
     * A scene that uses tilemaps to load and unload game objects.
     * 
     * Most Important features
     * - Parse tilemap files and create GameObjects based on tile data
     * - store GameObjects in chunks based on their position
     * - Unload GameObjects that are out of Camera view
     * - Keep track of GameObjects that were destroyed during the scene, so
     *   they are not reloaded when the Camera view moves back to them
     */
    public class ChunkedScene : Scene
    {

        private ChunkMap chunkMap = new ChunkMap();

        public ChunkedScene() : base()
        {
        }
        public ChunkedScene(Camera camera) : base(camera)
        {
        }

        public ChunkedScene(string name) : base(name)
        {
        }

        public int GetChunkCount()
        {
            return chunkMap.Count;
        }

        private void UnloadComponent(Component component)
        {
            if (component.GetScene() != this)
            {
                throw new Exception("Component has to be in the scene to be moved to chunks");
            }
            // remove the scene reference from the component
            component._clear_active_scene_dangerously();

            // remove the component from the scene
            switch (component)
            {
                case Drawable drawable:
                    var succ = drawableList.Remove(drawable);
                    break;
                case Collider collider:
                    physicsObjects.Remove(collider.GetGameObject());
                    break;
                case PhysicsBody physicsBody:
                    physicsObjects.Remove(physicsBody.GetGameObject());
                    break;
                case Script script:
                    var success = scripts.Remove(script);
                    if (!success)
                    {
                        // maybe the script was already removed
                        // dont call OnDestroy again
                        Console.WriteLine("Script was already removed");
                        return;
                    }
                    // call OnDisable
                    script.OnDisable();
                    break;
            }
        }

        private void DeepUnload(GameObject gameObject)
        {
            if (gameObject.GetScene() != this)
            {
                throw new Exception("GameObject has to be in the scene to be moved to chunks");
            }

            // remove all children
            foreach (GameObject child in gameObject.GetChildren())
            {
                DeepUnload(child);
            }

            foreach(Component component in gameObject.GetAllComponents())
            {
                UnloadComponent(component);
            }

            gameObject._clear_active_scene_dangerously();
            gameObjectsCount--;
        }

        private List<GameObject> toBeUnloaded = new List<GameObject>();
        private void MoveToChunks(GameObject gameObject)
        {
            if(gameObject.GetScene() != this)
            {
                throw new Exception("GameObject has to be in the scene to be moved to chunks");
            }

            if(gameObject.GetActiveScene() != this)
            {
                throw new Exception("GameObject has to be active to be moved to chunks");
            }

            if(gameObject.GetParent() != null)
            {
                throw new Exception("Only root GameObjects can be moved to chunks");
            }

            // remove the scene reference from the GameObject
            DeepUnload(gameObject);

            // add the GameObject to the chunk map
            chunkMap.AddGameObject(gameObject);

            // remove the GameObject from the scene
            toBeUnloaded.Add(gameObject);

            
        }


        private double lastBoundsCheck = -1;
        private double boundsCheckInterval = 0.5;

        public override void Update()
        {
            // get the simulation bounds
            Rect bounds = GetSimulationBounds();
            bounds = chunkMap.FitToChunkGrid(bounds);
            Rect alwaysLoaded = GetAlwaysLoadedBounds();

            var enumerator = chunkMap.LoadInBounds(bounds);
            var toAddFromChunks = new List<GameObject>();
            while(enumerator.MoveNext())
            {
                toAddFromChunks.Add(enumerator.Current);
            }
            toBeUnloaded.Clear();


            // add GameObjects from chunks to the scene
            foreach(GameObject gameObject in toAddFromChunks)
            {
                base.AddGameObject(gameObject);
            }

            if(Time.time - lastBoundsCheck > boundsCheckInterval)
            {
                lastBoundsCheck = Time.time;

                // check if any GameObjects are out of bounds
                // if so, remove them from the scene and add them to the chunk map
                foreach (GameObject gameObject in gameObjects)
                {
                    if(gameObject.ToBeDestroyed() || gameObject.KeepInScene)
                    {
                        // if the GameObject is already scheduled for destruction, we don't need to move it to chunks
                        // it will get destroyed later in the Update method
                        continue;
                    }
                    if ((!alwaysLoaded.Contains(gameObject.GetPosition()))&&(!bounds.Contains(gameObject.GetPosition())))
                    {
                        MoveToChunks(gameObject);
                    }
                }
            }

            // remove toBeUnloaded GameObjects
            foreach(GameObject gameObject in toBeUnloaded)
            {
                gameObjects.Remove(gameObject);
            }


            toBeUnloaded.Clear();

            int added = 0;
            foreach (EngineObject engineObject in toAdd)
            {
                if (added >= Scene.MAX_ADDS_PER_FRAME)
                {
                    break;
                }
                added++;
                if (engineObject is GameObject gameObject)
                {
                    GameObject? parent = gameObject.GetParent();
                    // only add the game object if its a root
                    if (parent == null)
                    {
                        if (gameObject.GetActiveScene() == null)
                        {
                            var pos = gameObject.GetPosition();
                            if (!bounds.Contains(pos))
                            {
                                this.chunkMap.AddGameObject(gameObject);
                                continue;
                            }

                            this.gameObjects.Add(gameObject);
                            HandleAddGameObjectComponents(gameObject);
                        }
                        else
                        {
                            Console.WriteLine($"WARNING: GameObject {gameObject.GetName()} is already in an active scene. This could be caused by adding the GameObject to the scene multiple times, or adding a GameObject that is already in another active scene.");
                        }
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

                        if (parent.GetScene() == this && parent.GetActiveScene() == this && gameObject.GetActiveScene() == null)
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


                    if (componentsGameObject.GetScene() == this && componentsGameObject.GetActiveScene() == this && component.GetActiveScene() == null)
                    {
                        HandleAddComponent(component);
                    }
                }
            }
            // remove the added game objects from the toAdd list
            // make Scene.MAX_ADDS_PER_FRAME + 1 the new first element
            if (toAdd.Count <= Scene.MAX_ADDS_PER_FRAME)
            {
                toAdd.Clear();
            }
            else
            {
                int removedCount = 0;
                while (toAdd.Count > 0 && removedCount < Scene.MAX_ADDS_PER_FRAME)
                {
                    toAdd.RemoveFirst();
                    removedCount++;
                }
            }

            // Physics
            // get all game objects with colliders
            if (doPhysics)
            {
                /*
                List<GameObject> goWithPhysics = new();
                foreach (Collider collider in physicsObjects)
                {
                    // TODO: this might cause problems if we add a parent game object and a child game object
                    // maybe fix this later
                    goWithPhysics.Add(collider.GetGameObject());
                }*/

                // define the physics world
                // 3* viewport size
                Rect rect = GetSimulationBounds();

                Physics.UpdatePhysics(physicsObjects, rect, worldSettings);
            }


            // start scripts that are scheduled to be started
            foreach (Script script in toStart)
            {
                script._Start();
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

            // run coroutines
            coroutineManager.RunScheduledCoroutines();

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

    }
}
