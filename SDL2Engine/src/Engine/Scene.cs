

namespace SDL2Engine
{
    public class Scene : GameObject
    {
        private Camera camera;

        private List<Drawable> drawableList = new();
        private List<Collider> colliderList = new();
        private List<Script> scripts = new();
        public Scene()
        {
            this.Parent = null;
            this.scene = this;
            this.camera = new Camera2D(new Vec2D());
        }
        public Scene(Camera camera)
        {
            this.Parent = null;
            this.scene = this;
            this.camera = camera;
        }

        public Scene(string name)
        {
            this.Parent = null;
            this.scene = this;
            this.camera = new Camera2D(new Vec2D());
            this.name = name;
        }


        public Camera GetCamera()
        {
            return camera;
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
