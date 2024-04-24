

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

        public void RemoveGameObject(GameObject gameObject)
        {
            this.toRemove.Add(gameObject);
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
            }
            toRemove.Clear();
        }

    }

    public static class SceneManager
    {
        private static Scene? activeScene;

        public static Scene? GetActiveScene()
        {
            return activeScene;
        }

        public static void SetActiveScene(Scene scene)
        {
            activeScene = scene;
        }
    }
}
