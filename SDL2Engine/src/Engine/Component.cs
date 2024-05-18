using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace SDL2Engine
{
    /* Base class for all components
     * 
     * Components can be added to GameObjects
     * to provide complex functionality
     * 
     * To create custom components, inherit from Script instead
     */
    public class Component : EngineObject
    {
        [JsonProperty]
        protected GameObject gameObject = GameObject.Default;

        public Component()
        {

        }

        public void Init(GameObject gameObject)
        {
            this.gameObject = gameObject;
            this.name = gameObject.GetName() + "." + this.GetType().Name;
            this.SetEnabled(gameObject.IsEnabled());
        }

        // Destructor in case the component is removed
        ~Component()
        {
            this.Disable();
        }

        // copy all members of the component
        public static T Clone<T>(T source, GameObject? gameObject = null) where T : Component, new()
        {
            T newComponent = new T();

            // use reflection to copy all members of the component
            FieldInfo[] fields = source.GetType().GetFields(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
            foreach (FieldInfo field in fields)
            {
                field.SetValue(newComponent, field.GetValue(source));
            }

            // TODO: check if this works
            GameObject myParent = gameObject ?? source.gameObject;
            return myParent.AddComponent<T>(newComponent);
        }

        /*
         * Create a new component of type T and add it to a GameObject
         *
         * This method is used to create new components
         * and add them to a GameObject
         *
         * This creates a new GameObject
         *
         * The GameObject is named after the component type
         * 
         * This returns a Tuple with the GameObject and the Component
         */
        public static Tuple<GameObject, T> CreateWithGameObject<T>(string? gameObjectName=null) where T : Component, new()
        {
            gameObjectName ??= typeof(T).Name + "GameObject";
            var gameObject = new GameObject(gameObjectName);
            return new Tuple<GameObject, T>(gameObject, gameObject.AddComponent<T>());
        }

        public void SetEnabled(bool status)
        {
            this.enabled = status;
        }

        public bool IsEnabled()
        {
            return enabled;
        }

        public bool IsDisabled()
        {
            return !enabled;
        }

        public void Enable()
        {
            SetEnabled(true);
        }

        public void Disable()
        {
            SetEnabled(false);
        }

        public bool ToggleEnabled()
        {
            SetEnabled(!enabled);
            return enabled;
        }

        // This is called for every compenent when it gets added to a Scene
        // This doesn't necessarily mean that the owning GameObject has been completely initialized
        public virtual void Awake()
        {
            // Do nothing
        }


        // Usefull methods to interact with other components

        // Returns the component of type T attached to the same GameObject
        public T? GetComponent<T>() where T : Component
        {
            return gameObject.GetComponent<T>();
        }

        // Returns all components of type T attached to the same GameObject
        public List<T> GetComponents<T>() where T : Component
        {
            return gameObject.GetComponents<T>();
        }

        // Returns the component of type T attached to any GameObject in the scene
        public T? FindComponent<T>() where T : Component
        {
            return gameObject.FindComponent<T>();
        }

        // Returns the GameObject with the given name in the scene
        public GameObject? Find(string name)
        {
            return gameObject.Find(name);
        }

        // Adds a component of type T to the same GameObject
        public T AddComponent<T>(T? component = null) where T : Component, new()
        {
            return gameObject.AddComponent<T>(component);
        }
        
        // Removes a component from the same GameObject by reference
        public bool RemoveComponent(Component component)
        {
            return gameObject.RemoveComponent(component);
        }

        // Removes a component of type T from the same GameObject
        public bool RemoveComponent<T>() where T : Component
        {
            return gameObject.RemoveComponent<T>();
        }

        // Returns the main camera of the scene
        public Camera? GetCamera()
        {
            var scene = gameObject.GetScene();
            if (scene != null)
            {
                return scene.GetCamera();
            }
            return null;
        }
        
        // Returns a Component of type T attached to the same GameObject or any of its parents
        public T? GetComponentInParent<T>() where T : Component
        {
            GameObject? parent = gameObject.GetParent();
            while (parent != null)
            {
                T? component = parent.GetComponent<T>();
                if (component != null)
                {
                    return component;
                }
                parent = parent.GetParent();
            }

            return null;
        }

        // Returns a Component of type T attached to the same GameObject or any of its children
        public T? GetComponentInChildren<T>() where T : Component
        {
            U? GetComponentInChildrenHelper<U>(GameObject parent) where U : Component
            {
                U? component = parent.GetComponent<U>();
                if (component != null)
                {
                    return component;
                }

                List<GameObject> children = parent.GetChildren();
                foreach (GameObject child in children)
                {
                    U? foundComponent = GetComponentInChildrenHelper<U>(child);
                    if (foundComponent != null)
                    {
                        return foundComponent;
                    }
                }

                return null;
            }

            return GetComponentInChildrenHelper<T>(gameObject);
        }

        // Returns all Components of type T attached to the same GameObject or any of its children
        public List<T> GetComponentsInChildren<T>() where T : Component
        {
            List<U> GetComponentsInChildrenHelper<U>(GameObject parent) where U : Component
            {
                List<U> foundComponents = new();
                List<U> components = parent.GetComponents<U>();
                foundComponents.AddRange(components);

                List<GameObject> children = parent.GetChildren();
                foreach (GameObject child in children)
                {
                    List<U> foundChildComponents = GetComponentsInChildrenHelper<U>(child);
                    foundComponents.AddRange(foundChildComponents);
                }

                return foundComponents;
            }

            return GetComponentsInChildrenHelper<T>(gameObject);
        }

        // Returns all Components of type T attached to the same GameObject or any of its parents
        public List<T> GetComponentsInParent<T>() where T : Component
        {
            List<T> foundComponents = new();
            GameObject? parent = gameObject.GetParent();
            while (parent != null)
            {
                List<T> components = parent.GetComponents<T>();
                foundComponents.AddRange(components);
                parent = parent.GetParent();
            }

            return foundComponents;
        }

        public GameObject GetGameObject()
        {
            return gameObject;
        }

        // SendMessage method to call methods on other components
        // TODO: test if this actually works (looks a bit sketchy)
        public bool SendMessage(string methodName, object[]? args, out object? result)
        {
            result = null;
            var components = gameObject.GetComponents<Component>();
            foreach (Component component in components)
            {
                if (component != this) // Ensure not to call on itself unless intended
                {
                    MethodInfo? method = component.GetType().GetMethod(methodName, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
                    if (method != null)
                    {
                        try
                        {
                            result = method.Invoke(component, args);
                            return true; // Return true if at least one method is successfully invoked
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine("Exception occurred while invoking method: " + e.Message);
                            continue; // Optionally continue to try other components or break out
                        }
                    }
                }
            }

            return false; // Return false if no method was found and invoked
        }

    }

    public class Script : Component
    {

        public bool wasEnabled = false;

        // Base class for all scripts
        // These scripts have a few virtual methods that can be overridden for custom functionality

        // Virtual Methods in order of execution
        // Override these method to provide custom funcionality

        // 1.
        // This method is called once when the Component is created
        /*
         * This is now part of Component
         * 
        public virtual void Awake()
        {
            // Do nothing
        }
        */

        // 2.
        // This method is called during the first frame the script is active
        public virtual void Start()
        {
            // Do nothing
        }

        // 3.
        // This method is called every frame
        public virtual void Update()
        {
            // Do nothing
        }

        // 4.
        // This method is called in fixed intervals to be more consistent with physics
        public virtual void FixedUpdate()
        {
            // Do nothing
        }

        // 5.
        // This method is called every time the script is enabled. Probaly during Update()
        public virtual void OnEnable()
        {
            // Do nothing
        }

        // 5.
        // This method is called every time the script is disabled. Probaly during Update()
        public virtual void OnDisable()
        {
            // Do nothing
        }

        // 6.
        // This method is called every frame after all Update() methods are called
        public virtual void LateUpdate()
        {
            // Do nothing
        }

        // 7.
        // This method is called when the script is destroyed
        public virtual void OnDestroy()
        {
            // Do nothing
        }

        // 8.
        // This method is part of every EngineObject (and therefore every Component)
        // It is called when the object is disposed (it is removed from the scene and is not persistent)
        // this should be used to clean up any resources, especially SDL resources that are not automatically GC'd
        /*
        public override void Dispose()
        {
            // Do nothing
        }
        */


        /*
         * Collision methods
         * 
         * These methods are called when a collision occurs
         * 
         * OnCollisionEnter is called when the collision starts
         * OnCollisionStay is called every frame the collision is happening
         * OnCollisionExit is called when the collision ends (the first frame the collision is not happening)
         * 
         * CollisionPair is a struct that contains the two colliding objects
         * One of them is the object this script is attached to
         * 
         * These methods can be overridden to provide custom functionality
         */
        public virtual void OnCollisionEnter(CollisionPair collision)
        {
            // Do nothing
        }

        public virtual void OnCollisionStay(CollisionPair collision)
        {
            // Do nothing
        }

        public virtual void OnCollisionExit(CollisionPair collision)
        {
            // Do nothing
        }


        public Transform transform
        {
            get
            {
                return gameObject.transform;
            }

            set
            {
                gameObject.transform = value;

            }
        }

    }

    /*
     * Base class for all game objects
     * 
     * This class is meant to be inherited from
     * and can be overridden to provide custom
     * functionality
     */

    public class Transform : Component
    {
        private Vec2D _position = new Vec2D();
        private Vec2D _localPosition = new Vec2D();

        public void UpdateChildren()
        {
            var children = gameObject.GetChildren();
            foreach (GameObject child in children)
            {
                child.SetParentPosition(position);
            }
        }

        public double rotation
        {
            get
            {
                return _rotation;
            }
            set
            {
                _rotation = value;
            }
        }

        // sets the position to a specific world position
        // adjust _localPosition to keep parent position the same
        public Vec2D position
        {
            get
            {
                return _position;
            }
            set
            {
                var diff = value - _position;
                localPosition += diff;
            }
        }

        // sets the position to a specific local position
        // this is the position relative to the parent
        public Vec2D localPosition
        {
            get
            {
                return _localPosition;
            }
            set
            {
                // remove local position from parent position
                var rootPosition = _position - _localPosition;
                _position = value + rootPosition;
                _localPosition = value;

                UpdateChildren();
            }
        }

        public void Move(Vec2D move)
        {
            this.Move(move.x, move.y);
        }

        public void Move(double x, double y)
        {
            _localPosition.x += x;
            _localPosition.y += y;
            _position.x += x;
            _position.y += y;

            UpdateChildren();
        }

        // sets the parent position directly
        public void SetParentPosition(Vec2D parentPosition)
        {
            _position = _localPosition + parentPosition;
            UpdateChildren();
        }

        public Vec2D GetPosition()
        {
            return position;
        }

        public Vec2D GetLocalPosition()
        {
            return localPosition;
        }

        private Vec2D _scale = new Vec2D(1, 1);
        private Vec2D _localScale = new Vec2D(1, 1);
        private double _rotation = 0.0;
        private double _localRotation = 0.0;


    }

    public class SoundPlayer : Component, ILoadable
    {
        public static double sound_volume = 1.0;
        public string source = "";
        public double volume = SoundPlayer.sound_volume;
        private Sound? sound = null;
        public bool playOnAwake = false;


        public void SetSource(string source)
        {
            this.source = source;
            if (sound != null)
            {
                if (sound.IsPlaying())
                {
                    sound.Stop();
                }
                sound.Dispose();
                sound = null;
            }

        }

        public void Load()
        {
            this.Load(null);
        }

        // ILoable methods
        public void Load(string? new_source = null)
        {
            if (sound != null)
            {
                return;
            }

            if (new_source != null)
            {
                this.source = new_source;
            }

            if (source != "")
            {
                sound = AssetManager.LoadSound(source);
                sound.Load();
            }
        }

        public override void Awake()
        {
            base.Awake();
            if (playOnAwake)
            {
                Play();
            }
        }

        public bool IsLoaded()
        {
            return sound != null;
        }

        public override void Dispose()
        {
            base.Dispose();
            if (sound != null)
            {
                sound.Dispose();
            }
        }

        // Playing the sound
        public bool Play(int loop = 0)
        {

            if(sound == null)
            {
                if (source != "")
                {
                    Load();
                }
            }


            if (sound != null)
            {
                var result = sound.Play(loop);
                if (result)
                {
                    sound.SetVolume(volume);
                }
                return result;
            }
            return false;
        }

        public bool Stop()
        {
            if (sound != null)
            {
                return sound.Stop();
            }

            return false;
        }

        public bool IsPlaying()
        {
            if (sound != null)
            {
                return sound.IsPlaying();
            }

            return false;
        }

        public bool SetVolume(double volume)
        {
            if (sound != null)
            {
                return sound.SetVolume(volume);
            }

            return false;
        }

    }

    public class MusicPlayer : Component, ILoadable
    {
        public static double music_volume = 1.0;
        public string source = "";
        public double volume = MusicPlayer.music_volume;
        private Music? music = null;
        public bool playOnAwake = false;

        public void SetSource(string source)
        {
            this.source = source;
            if (music != null)
            {
                if (music.IsPlaying())
                {
                    music.Stop();
                }
                music.Dispose();
                music = null;
            }

        }

        // ILoable methods
        public void Load()
        {
            if (music != null)
            {
                return;
            }

            if (source != "")
            {
                music = AssetManager.LoadMusic(source);
                music.Load();
            }
        }

        public override void Awake()
        {
            base.Awake();
            if (playOnAwake)
            {
                Play();
            }
        }

        public bool IsLoaded()
        {
            return music != null;
        }

        public override void Dispose()
        {
            base.Dispose();
            if (music != null)
            {
                music.Dispose();
            }
        }

        // Playing the music
        public bool Play(int loop = 0)
        {
            if (music != null)
            {
                var result = music.Play(loop);
                if (result)
                {
                    music.SetVolume(volume);
                }
                return result;
            }
            return false;
        }

        public bool Stop()
        {
            if (music != null)
            {
                return music.Stop();
            }

            return false;
        }

        public bool IsPlaying()
        {
            if (music != null)
            {
                return music.IsPlaying();
            }

            return false;
        }

        public bool SetVolume(double volume)
        {
            if (music != null)
            {
                return music.SetVolume(volume);
            }

            return false;
        }

    }
}
