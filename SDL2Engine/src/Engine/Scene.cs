

namespace SDL2Engine
{
    public class Scene : GameObject
    {
        private Camera camera;

        private List<Drawable> drawableList = new();
        public Scene()
        {
            this.Position = new Vec2D();
            this.ParentPosition = new Vec2D();
            this.Parent = null;
            this.scene = this;
            this.camera = new Camera2D(new Vec2D());
        }

        public Scene(Camera camera)
        {
            this.Position = new Vec2D();
            this.ParentPosition = new Vec2D();
            this.Parent = null;
            this.scene = this;
            this.camera = camera;
        }


        public Camera GetCamera()
        {
            return camera;
        }

        public void ActivateComponent<T>(T component) where T : Component
        {
            // Store some component types in lists for quick access
            if (component is Drawable)
            {
                Drawable? drawable = component as Drawable;
                if (drawable == null)
                {
                    Console.WriteLine("Error: Drawable is null");
                    return;
                }
                drawableList.Add(drawable);

                return;
            }
        }

        public void DeactivateComponent<T>(T component) where T : Component
        {
            // Remove some component types from lists
            if (component is Drawable)
            {
                Drawable? drawable = component as Drawable;
                if (drawable == null)
                {
                    Console.WriteLine("Error: Drawable is null");
                    return;
                }
                drawableList.Remove(drawable);

                return;
            }
        }

    }
}
