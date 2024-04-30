using SDL2;
using System.Collections;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
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

    public abstract class IAsset : IDisposable
    {
        public abstract void Load();
        public abstract bool IsLoaded();
        public abstract void Unload();
        public abstract void Dispose();
    }

    public abstract class Asset<T> : IAsset
    {
        private AssetHandler<T> handler;
        private bool loaded = false;
        private bool hasRef = false;
        private string path;
        public Asset(AssetHandler<T> handler)
        {
            this.handler = handler;
            this.path = handler.GetPath();
            handler.AddRef();
            hasRef = true;
        }

        ~Asset()
        {
            Dispose();
        }

        public override void Load()
        {
            if (loaded)
            {
                return;
            }

            handler.LoadAsset();
            loaded = true;

        }

        public override bool IsLoaded()
        {
            return loaded;
        }

        public override void Unload()
        {
            Dispose();
        }

        public override void Dispose()
        {
            if(!hasRef)
            {
                return;
            }

            handler.RemoveRef();
            loaded = false;
        }

        public abstract T GetDefault();

        public T Get()
        {
            if(!hasRef)
            {
                return GetDefault();
            }

            if (!loaded)
            {
                Load();
            }

            if(!handler.IsLoaded())
            {
                return GetDefault();
            }

            var res = handler.Get();
            if (res == null)
            {
                return GetDefault();
            }
            return res;
        }


    }

    /*
     * Possible AssetTypes:
     * - Texture
     * - Audio
     * - Font
     * - Binary
     * - Text
     * - JSON
     */

    /* This could be a way to handle different types of assets
     * The Asset class would be a generic class that takes a type parameter
     * and something like Texture would inherit from Asset<IntPtr>,
     * so it returns an IntPtr when calling Get()
     * A Scene could collect all Assets used in the scene and preload them by calling Load() on all of them
     * Also keep track of time so that loading assets doesn't block the main thread for too long
     * Loading the Scene should also have a callback function that returns the progress of the loading
     * or it returns a callable function or an object that can be used to check the progress
     * 
     
    public abstract class TestClass<T>
    {
        public bool Load();
        public bool Unload();
        public abstract T Get(); // this also loads the asset if it's not loaded yet
    }

    public class TestClass2 : TestClass<int>
    {
        public override int Get()
        {
            return 0;
        }
    }
    */


    public class Texture : Asset<IntPtr>
    {
        private static Lazy<IntPtr> default_texture = new(() =>
        {
            int size = 128;
            int numPixels = size * size;
            int bytesPerPixel = 4; // Each pixel is 4 bytes (32 bits)
            IntPtr pixelPtr = Marshal.AllocHGlobal(numPixels * bytesPerPixel);

            // Unsafe code block to manipulate memory directly
            unsafe
            {
                UInt32* pixels = (UInt32*)pixelPtr.ToPointer();
                for (int i = 0; i < numPixels; i++)
                {
                    pixels[i] = (i % size < size / 2) ^ (i / size < size / 2) ? 0x443355FFu : 0x221133FFu;
                }
            }

            var surface = SDL.SDL_CreateRGBSurfaceFrom(pixelPtr, size, size, 32, size * 4, 0xFF000000, 0x00FF0000, 0x0000FF00, 0x000000FF);
            var texture = SDL.SDL_CreateTextureFromSurface(Engine.renderer, surface);
            SDL.SDL_FreeSurface(surface);

            // Free the unmanaged memory once it is no longer needed
            Marshal.FreeHGlobal(pixelPtr);

            return texture;
        });

        public Texture(AssetHandler<IntPtr> handler) : base(handler)
        {
        }

        public override IntPtr GetDefault()
        {
            return default_texture.Value;
        }

        // how to suggest inlining in c#: (usually done automatically by the jit compiler)
        // [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Draw(IntPtr renderer, ref Rect source, ref Rect destination)
        {
            var sdl_source = source.ToSDLRect();
            var sdl_destination = destination.ToSDLRect();
            SDL.SDL_RenderCopy(renderer, Get(), ref sdl_source, ref sdl_destination);
        }
    }

    public class Sound : Asset<IntPtr>
    {
        public Sound(AssetHandler<IntPtr> handler) : base(handler)
        {
        }

        public void Play()
        {
            // Play sound
        }

        public void Pause()
        {
            // Pause sound
        }

        public void Stop()
        {
            // Stop sound
        }

        public void Loop()
        {
            // Loop sound
        }

        public override IntPtr GetDefault()
        {
            return IntPtr.Zero;
        }
    }

    

    public abstract class AssetHandler<T>
    {
        protected string path;
        protected T? asset;
        protected uint refCount = 0;
        protected bool loaded = false;
        protected bool loadFailed = false;

        public AssetHandler(string path)
        {
            this.path = path;

            LoadAsset();
        }

        // This should return true if the asset was loaded successfully or was already loaded
        public abstract bool LoadAsset();

        public bool IsLoaded()
        {
            return loaded;
        }

        // This should return true if the asset was unloaded successfully, otherwise false
        public abstract bool UnloadAsset();

        public T? Get()
        {
            if(loadFailed)
            {
                return default;
            }

            if (!loaded)
            {
                var result = LoadAsset();
                if (!result)
                {
                    loadFailed = true;
                    return default;
                }
                loaded = true;
            }
            return asset;
        }

        public string GetPath()
        {
            return path;
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

        public bool UnloadIfUnused()
        {
            if (refCount == 0)
            {
                UnloadAsset();
                return true;
            }

            return false;
        }

    }

    class TextureHandler : AssetHandler<IntPtr>
    {


        public TextureHandler(string path) : base(path)
        {
        }

        public override bool LoadAsset()
        {
            if(loaded)
            {
                return true;
            }
            if(loadFailed)
            {
                return false;
            }
            asset = SDL_image.IMG_LoadTexture(Engine.renderer, path);
            if (asset == IntPtr.Zero)
            {
                Console.WriteLine("TextureHandler: Failed to load texture: " + path);
                loadFailed = true;
                return false;
                
            }
            loaded = true;
            return true;
        }

        public override bool UnloadAsset()
        {
            if (!loaded)
            {
                return false;
            }

            if (asset != IntPtr.Zero)
            {
                SDL.SDL_DestroyTexture(asset);
                asset = IntPtr.Zero;
                loaded = false;
                return true;
            }
            return false;
        }

    }

    class SoundHandler : AssetHandler<IntPtr>
    {
        public SoundHandler(string path) : base(path)
        {
        }

        public override bool LoadAsset()
        {
            // Load sound
            if (loaded)
            {
                return true;
            }

            return false;
        }

        public override bool UnloadAsset()
        {
            // Unload sound
            if (!loaded)
            {
                return false;
            }

            return false;
        }

    }

    // Asset manager class
    // This class should be used to load and manage game assets such as textures, sounds, etc.
    // To use an asset, a function should load the game assets with the AssetManager.LoadAsset method
    // This should help to avoid loading the same asset multiple times
    // The AssetManager should also be able to preload game assets, for example when loading a scene
    public static class AssetManager
    {


        private static Dictionary<string, TextureHandler> texture_assets = new();
        private static Dictionary<string, SoundHandler> sound_assets = new();

        private static Texture LoadTexture(string path)
        {
            TextureHandler? handler = null;
            if (texture_assets.ContainsKey(path))
            {
                handler = texture_assets[path];
            } else
            {
                handler = new TextureHandler(path);
                texture_assets[path] = handler;
                // handler.LoadAsset();
            }
            
            return new Texture(handler);
        }

        private static Sound LoadSound(string path)
        {
            SoundHandler? handler = null;
            if (sound_assets.ContainsKey(path))
            {
                handler = sound_assets[path];
            }
            else
            {
                handler = new SoundHandler(path);
                sound_assets[path] = handler;
                // handler.LoadAsset();
            }

            return new Sound(handler);
        }


        public static T LoadAsset<T>(string path) where T : IAsset
        {
            if (typeof(T) == typeof(Texture))
            {
                return LoadTexture(path) as T ?? throw new Exception("Failed to load texture");
            }
            else
            {
                return LoadSound(path) as T ?? throw new Exception("Failed to load sound");
            }
        }

        public static void CleanUnusedAssets()
        {
            
        }

    }

    // Audio manager class
    // This class should be used to play and manage audio assets
    // The AudioManager should be able to play, pause, stop, and loop audio assets
    public static class AudioManager
    {

    }



}
