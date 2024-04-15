

namespace SDL2Engine
{

    public struct Vec2D
    {
        public double x;
        public double y;

        public Vec2D(double x = 0, double y = 0)
        {
            this.x = x;
            this.y = y;
        }

        public static Vec2D operator +(Vec2D a, Vec2D b)
        {
            return new Vec2D(a.x + b.x, a.y + b.y);
        }

        public static Vec2D operator -(Vec2D a, Vec2D b)
        {
            return new Vec2D(a.x - b.x, a.y - b.y);
        }

        public static Vec2D operator *(Vec2D a, double b)
        {
            return new Vec2D(a.x * b, a.y * b);
        }

        public static Vec2D operator /(Vec2D a, double b)
        {
            return new Vec2D(a.x / b, a.y / b);
        }

        public static Vec2D operator *(double a, Vec2D b)
        {
            return new Vec2D(a * b.x, a * b.y);
        }

        public static Vec2D operator /(double a, Vec2D b)
        {
            return new Vec2D(a / b.x, a / b.y);
        }

        public static Vec2D operator *(Vec2D a, Vec2D b)
        {
            return new Vec2D(a.x * b.x, a.y * b.y);
        }

        public static Vec2D operator /(Vec2D a, Vec2D b)
        {
            return new Vec2D(a.x / b.x, a.y / b.y);
        }

        public static Vec2D operator -(Vec2D a)
        {
            return new Vec2D(-a.x, -a.y);
        }

        public static Vec2D operator +(Vec2D a)
        {
            return new Vec2D(+a.x, +a.y);
        }

        public static bool operator ==(Vec2D a, Vec2D b)
        {
            return a.x == b.x && a.y == b.y;
        }

        public override readonly bool Equals(object? obj)
        {
            return obj is Vec2D v && this == v;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public static bool operator !=(Vec2D a, Vec2D b)
        {
            return a.x != b.x || a.y != b.y;
        }

        public static double Dot(Vec2D a, Vec2D b)
        {
            return a.x * b.x + a.y * b.y;
        }

        public double Length()
        {
            return Math.Sqrt(x * x + y * y);
        }

        public double LengthSquared()
        {
            return x * x + y * y;
        }

        public Vec2D Normalize()
        {
            return this / Length();
        }

        public static bool operator>(Vec2D a, Vec2D b)
        {
            return a.LengthSquared() > b.LengthSquared();
        }

        public static bool operator<(Vec2D a, Vec2D b)
        {
            return a.LengthSquared() < b.LengthSquared();
        }

        public static bool operator>=(Vec2D a, Vec2D b)
        {
            return a.LengthSquared() >= b.LengthSquared();
        }

        public static bool operator<=(Vec2D a, Vec2D b)
        {
            return a.LengthSquared() <= b.LengthSquared();
        }

    }

    public struct Vec3D
    {
        public double x;
        public double y;
        public double z;

        public Vec3D(double x = 0, double y = 0, double z = 0)
        {
            this.x = x;
            this.y = y;
            this.z = z;
        }

        public static Vec3D operator +(Vec3D a, Vec3D b)
        {
            return new Vec3D(a.x + b.x, a.y + b.y, a.z + b.z);
        }

        public static Vec3D operator -(Vec3D a, Vec3D b)
        {
            return new Vec3D(a.x - b.x, a.y - b.y, a.z - b.z);
        }

        public static Vec3D operator *(Vec3D a, double b)
        {
            return new Vec3D(a.x * b, a.y * b, a.z * b);
        }

        public static Vec3D operator /(Vec3D a, double b)
        {
            return new Vec3D(a.x / b, a.y / b, a.z / b);
        }

        public static Vec3D operator *(double a, Vec3D b)
        {
            return new Vec3D(a * b.x, a * b.y, a * b.z);
        }

        public static Vec3D operator /(double a, Vec3D b)
        {
            return new Vec3D(a / b.x, a / b.y, a / b.z);
        }

        public static Vec3D operator *(Vec3D a, Vec3D b)
        {
            return new Vec3D(a.x * b.x, a.y * b.y, a.z * b.z);
        }

        public static Vec3D operator /(Vec3D a, Vec3D b)
        {
            return new Vec3D(a.x / b.x, a.y / b.y, a.z / b.z);
        }

        public static Vec3D operator -(Vec3D a)
        {
            return new Vec3D(-a.x, -a.y, -a.z);
        }

        public static Vec3D operator +(Vec3D a)
        {
            return new Vec3D(+a.x, +a.y, +a.z);
        }

        public static bool operator ==(Vec3D a, Vec3D b)
        {
            return a.x == b.x && a.y == b.y && a.z == b.z;
        }

        public override readonly bool Equals(object? obj)
        {
            return obj is Vec3D v && this == v;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public static bool operator !=(Vec3D a, Vec3D b)
        {
            return a.x != b.x || a.y != b.y || a.z != b.z;
        }

        public static double Dot(Vec3D a, Vec3D b)
        {
            return a.x * b.x + a.y * b.y + a.z * b.z;
        }

        public static Vec3D Cross(Vec3D a, Vec3D b)
        {
            return new Vec3D(a.y * b.z - a.z * b.y, a.z * b.x - a.x * b.z, a.x * b.y - a.y * b.x);
        }

        public double Length()
        {
            return Math.Sqrt(x * x + y * y + z * z);
        }

        public double LengthSquared()
        {
            return x * x + y * y + z * z;
        }

        public Vec3D Normalize()
        {
            return this / Length();
        }

        public static bool operator>(Vec3D a, Vec3D b)
        {
            return a.LengthSquared() > b.LengthSquared();
        }

        public static bool operator<(Vec3D a, Vec3D b)
        {
            return a.LengthSquared() < b.LengthSquared();
        }

        public static bool operator>=(Vec3D a, Vec3D b)
        {
            return a.LengthSquared() >= b.LengthSquared();
        }

        public static bool operator<=(Vec3D a, Vec3D b)
        {
            return a.LengthSquared() <= b.LengthSquared();
        }

    }

    /* Base class for all scripts
     * 
     * This class is meant to be inherited from
     * and can be overridden to provide custom
     * functionality to GameObjects.
     */

    public interface ICamera
    {
        public Vec2D GetScreenPosition(Vec2D worldPosition);
    }

    public class Camera2D : ICamera
    {
        private Vec2D position;
        private Vec2D size;

        private Vec2D screenSize;

        public Camera2D(Vec2D position = new Vec2D())
        {
            this.position = position;
            this.size = new Vec2D(1920, 1080);
            this.screenSize = new Vec2D(1920, 1080);
        }

        public Vec2D GetScreenPosition(Vec2D worldPosition)
        {
            return new Vec2D((worldPosition.x - position.x) * screenSize.x / size.x, (worldPosition.y - position.y) * screenSize.y / size.y);
        }
    }

    // Base class for all scripts
    // Add to a GameObject to provide custom functionality
    public class Component
    {
        GameObject gameObject;

        public Component(GameObject gameObject)
        {
            this.gameObject = gameObject;
        }
        // Override this method to provide custom functionality
        public virtual void Start()
        {
            // Do nothing
        }

        // Override this method to provide custom functionality
        public virtual void Update()
        {
            // Do nothing
        }

    }
    /*
     * Base class for all game objects
     * 
     * This class is meant to be inherited from
     * and can be overridden to provide custom
     * functionality
     */
    public class GameObject
    {
        // Position of the GameObject
        private Vec2D position;
        private Vec2D parentPosition;
        private GameObject? parent;
        private GameObject? root;
        private List<GameObject> children = [];
        private List<Component> scripts = [];

        public GameObject()
        {
            this.position = new Vec2D();
            this.parentPosition = new Vec2D();
            this.parent = null;
            this.root = null;
        }

        public GameObject(GameObject root)
        {
            this.position = new Vec2D();
            this.parentPosition = new Vec2D();
            this.parent = null;
            this.root = root;
        }

        public void UpdateChildPositions()
        {
            var myPosition = this.GetPosition();
            foreach (GameObject child in children)
            {
                child.SetParentPosition(myPosition);
            }
        }

        public void SetParent(GameObject? parent)
        {
            this.parent = parent;
            
            if (parent != null)
            {
                this.SetParentPosition(parent.GetPosition());
            }
            else
            {
                this.SetParentPosition(new Vec2D());
            }
        }

        public GameObject? GetParent()
        {
            return parent;
        }

        public void RemoveParent()
        {
            this.parent = null;
            this.SetParentPosition(new Vec2D());
        }

        public Vec2D GetPosition()
        {
            return this.parentPosition + this.position;
        }

        public void SetPosition(Vec2D position)
        {
            this.position = position;
            this.UpdateChildPositions();

        }

        public void SetParentPosition(Vec2D position)
        {
            this.parentPosition = position;
            this.UpdateChildPositions();
        }


        
        public void AddChild(GameObject child)
        {
            child.SetParentPosition(this.GetPosition());
            children.Add(child);

        }

        public void RemoveChild(GameObject child)
        {
            // check if child is in children
            if (children.Contains(child))
            {
                children.Remove(child);
                child.SetParentPosition(new Vec2D());
            }
        }

        public List<GameObject> GetChildren()
        {
            return children;
        }

        // Gets first child of type T
        public T? GetChild<T>() where T : GameObject
        {
            foreach (GameObject child in children)
            {
                if (child is T)
                {
                    return (T)child;
                }
            }

            // no child of type T found, search recursively
            foreach (GameObject child in children)
            {
                T? foundChild = child.GetChild<T>();
                if (foundChild != null)
                {
                    return foundChild;
                }
            }
            return null;
        }

        public void AddComponent(Component script)
        {
            scripts.Add(script);
        }

        public void RemoveComponent(Component script)
        {
            scripts.Remove(script);
        }

        // Gets first component of type T
        public T? GetComponent<T>() where T : Component
        {
            foreach (Component script in scripts)
            {
                if (script is T)
                {
                    return (T)script;
                }
            }

            return null;
        }

        public List<Component> GetScripts()
        {
            return scripts;
        }

        public void Start()
        {
            foreach (Component script in scripts)
            {
                script.Start();
            }

            foreach (GameObject child in children)
            {
                child.Start();
            }
        }

        public void Update()
        {
            foreach (Component script in scripts)
            {
                script.Update();
            }

            foreach (GameObject child in children)
            {
                child.Update();
            }
        }
        
        // Override this method to draw custom graphics
        // By default, this method does nothing
        virtual public void Draw(ICamera camera)
        {
            // Do nothing
        }

        public void DrawAll(ICamera camera)
        {
            Draw(camera);
            foreach (GameObject child in children)
            {
                child.DrawAll(camera);
            }
        }

    }

    public class Time
    {
        public static double time = 0;
        public static double deltaTime = 0;
        public static double fixedDeltaTime = 1.0 / 60.0; // 60 FPS

        public static double lastDrawTime = 0;
        public static double lastUpdateTime = 0;
    }

    public class Input
    {
        // TODO: Implement static methods for getting inputs
        public static bool GetKeyDown(int key)
        {
            return false;
        }
    }

    public class Engine
    {
        private List<GameObject> gameObjects = [];
        private ICamera camera;

        public Engine()
        {
            this.camera = new Camera2D(new Vec2D());
        }

        public Engine(ICamera camera)
        {
            this.camera = camera;
        }

        public void AddGameObject(GameObject gameObject)
        {
            gameObjects.Add(gameObject);
        }

        public void RemoveGameObject(GameObject gameObject)
        {
            gameObjects.Remove(gameObject);
        }

        public void Start()
        {
            foreach (GameObject gameObject in gameObjects)
            {
                gameObject.Start();
            }
        }

        public void Update()
        {
            foreach (GameObject gameObject in gameObjects)
            {
                gameObject.Update();
            }
        }

        public void Draw(ICamera camera)
        {
            foreach (GameObject gameObject in gameObjects)
            {
                gameObject.DrawAll(camera);
            }
        }

        public void Run()
        {
            Time.time = 0;
            while (true)
            {
                this.Update();
                this.Draw(camera);
                Time.time += Time.deltaTime;
            }
        }
    }
}
