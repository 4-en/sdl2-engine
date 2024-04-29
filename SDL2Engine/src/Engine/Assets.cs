using SDL2;
using System.Collections;
using static SDL2.SDL;

namespace SDL2Engine
{
    // TODO: rework asset system
    // right now it's a bit of a mess

    /*
     * Idea
     * 1. Create asset reference (for example in GameObject or Component)
     *    Texture myTexture = LoadAsset<Texture>("path/to/texture.png");
     *    (This does not yet load the asset, only creates the object that will reference the asset later)
     *    
     * 2. Load asset when needed
     *    This can be done by the SceneManager when loading a scene, or when the owning object tries 
     *    to access the asset by calling myTexture.Get()
     *    Since the Assets are not loeaded when the AssetReference is created, the SceneManager should
     *    be able to preload all assets needed for a scene in the background, by checking the AssetReferences of
     *    all GameObjects and Components in the scene.
     *    
     * 3. When Load is called, try to request the asset from the AssetManager
     *    The AssetManager will check if the asset is already loaded, in which case a second class
     *    holds a reference to the asset as well as the number of references to itself.
     *    If the asset is not loaded, the AssetManager will load the asset and create a new reference.
     *    
     * 4. When the owning object is destroyed, it should call myTexture.Dispose() to remove the reference
     * 
     * 5. The AssetManager should periodically check for unused assets and unload them
     * 
     * Required classes:
     * 1. Asset (IDisposable)
     *    Base class for all assets
     *    
     * 2. Specific asset classes (Texture, Sound, etc.)
     *    These will inherit from Asset and have a function to access the underlying asset (an IntPtr for example)
     *    
     * 3. ManagedAsset
     *    This class will hold a reference to the asset and the number of references to itself
     *    It is also responsible for loading and unloading the asset
     *    Internally, it has to check what kind of asset it tries to load and use the correct method.
     *    For example, if the asset is a texture, it should use SDL_image.IMG_LoadTexture to load the asset
     *    
     * 4. AssetManager
     *    This class will manage all assets and keep track of which assets are loaded and how many references there are
     *    It's a static class, so it can be accessed from anywhere
     *    
     */
    public interface AssetType
    {
        bool Load(string path);
        bool Unload();
    }

    public class Texture : AssetType
    {
        private string? path = null;
        private IntPtr texture = IntPtr.Zero;
        public bool Load(string path)
        {
            // Load texture from file using SDL2
            if (texture != IntPtr.Zero || this.path == null)
            {
                return false;
            }

            this.path = path;
            this.texture = SDL_image.IMG_LoadTexture(Engine.renderer, path);
            if (texture == IntPtr.Zero)
            {
                Console.WriteLine("Failed to load texture: " + SDL_GetError());
                Console.WriteLine("Texture path: " + this.path);
                return false;
            }

            return true;

        }

        public bool Unload()
        {
            // Unload texture from memory
            if (texture == IntPtr.Zero || this.path == null)
            {
                return false;
            }

            SDL.SDL_DestroyTexture(texture);
            texture = IntPtr.Zero;
            path = null;
            return true;
        }

    }

    public class AssetHandler<T> where T : AssetType
    {
        string path;
        T? asset;
        uint refCount = 0;

        public AssetHandler(string path)
        {
            this.path = path;

            LoadAsset();
        }

        private void LoadAsset()
        {
            // Load asset from file
        }

        public void UnloadAsset()
        {
            // Unload asset from memory
        }

        public T? Get()
        {
            return asset;
        }

        public void AddRef()
        {
            refCount++;
        }

        public void RemoveRef()
        {
            refCount--;
        }

        public uint GetRefCount()
        {
            return refCount;
        }

    }

    public class Asset<T> : IDisposable where T : AssetType
    {
        private AssetHandler<T>? handler = null;
        private T? asset = default(T);
        private bool loaded = false;
        private string path;
        public Asset(string path)
        {
            this.path = path;
        }

        ~Asset()
        {
            Dispose();
        }

        public void Load()
        {
            if (handler == null)
            {
                handler = AssetManager.LoadAsset<T>(path);
                if (handler == null)
                {
                    return;
                }

                handler.AddRef();
                asset = handler.Get();

                if (asset != null)
                {
                    loaded = true;
                }
            }
        }

        public bool IsLoaded()
        {
            return loaded;
        }

        public void Unload()
        {
            Dispose();
        }

        public void Dispose()
        {
            if (handler != null)
            {
                handler.RemoveRef();
                handler = null;
                asset = default(T);
            }
        }

        public T? GetAsset()
        {
            if (asset == null)
            {
                Load();

            }
            return asset;
        }


    }

    // Asset manager class
    // This class should be used to load and manage game assets such as textures, sounds, etc.
    // To use an asset, a function should load the game assets with the AssetManager.LoadAsset method
    // This should help to avoid loading the same asset multiple times
    // The AssetManager should also be able to preload game assets, for example when loading a scene
    public static class AssetManager
    {


        private static Hashtable assets = new Hashtable();
        // private static Dictionary<string, AssetHandler<Texture>> textures = new Dictionary<string, AssetHandler<Texture>>();

        public static AssetHandler<T>? LoadAsset<T>(string path) where T : AssetType
        {
            // check if asset is already loaded
            if (assets.ContainsKey(path))
            {
                var thing = assets[path];
                if (thing is AssetHandler<T> handler)
                {
                    return handler;
                }

                return null;
            }



            AssetHandler<T> asset = new AssetHandler<T>(path);
            assets.Add(path, asset);
            return asset;
        }

        public static void CleanUnusedAssets()
        {
            foreach (AssetHandler<Texture> asset in assets)
            {
                if (asset.GetRefCount() == 0)
                {
                    asset.UnloadAsset();
                    assets.Remove(asset);
                }
            }
        }

    }

    // Audio manager class
    // This class should be used to play and manage audio assets
    // The AudioManager should be able to play, pause, stop, and loop audio assets
    public static class AudioManager
    {

    }



}
