

namespace SDL2Engine
{
    public class Scene : GameObject
    {
        private Camera camera;
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


    }
}
