using SDL2;
using System.Collections;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using static SDL2.SDL;

namespace SDL2Engine
{

    /*
     *    Asset management system
     * 
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
     *    Should only be instantiated by the AssetManager using LoadAsset<T> where T is the type of asset (e.g. Texture)
     *    
     * 3. AssetHandler
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

    public abstract class IAsset : ILoadable
    {
        public abstract void Load();
        public abstract bool IsLoaded();
        public abstract void Unload();
        public abstract void Dispose();
    }

    public abstract class Asset<T> : IAsset
    {
        protected AssetHandler<T> handler;
        private bool loaded = false;
        private bool loadFailed = false;
        private bool hasRef = false;
        public Asset(AssetHandler<T> handler)
        {
            this.handler = handler;
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

            bool result = handler.LoadAsset();
            loaded = result;
            if (!result)
            {
                loadFailed = true;
            }

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
            if (!hasRef)
            {
                return;
            }

            handler.RemoveRef();
            loaded = false;
        }

        public abstract T GetDefault();

        public T Get()
        {
            if (loaded)
            {
                T? asset = handler.Get();
                if (asset != null)
                {
                    return asset;
                }
                loaded = false;
                loadFailed = true;
            }

            if (loadFailed)
            {
                return GetDefault();
            }

            if (!hasRef)
            {
                return GetDefault();
            }

            Load();

            return loaded && !loadFailed ? handler.Get() ?? GetDefault() : GetDefault(); // <- nice code >:D
        }

        public string GetPath()
        {
            return handler.GetPath();
        }


    }

    /*
     * Possible AssetTypes:
     * - Texture
     * - Audio (Sound, Music)
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

    public class Font : Asset<IntPtr>
    {

        public Font(AssetHandler<IntPtr> handler) : base(handler)
        {
        }

        public override IntPtr GetDefault()
        {
            return IntPtr.Zero;
        }

        public IntPtr RenderTexture(string text, SDL_Color color, out Rect rect)
        {
            rect = new Rect(0, 0);
            var ptr = this.Get();
            if (ptr == IntPtr.Zero)
            {
                return IntPtr.Zero;
            }

            IntPtr surface = SDL_ttf.TTF_RenderUTF8_Blended(ptr, text, color);
            var texture = SDL_CreateTextureFromSurface(Engine.renderer, surface);

            // get the size of the texture
            int w, h;
            SDL_QueryTexture(texture, out _, out _, out w, out h);
            rect = new Rect(0, 0, w, h);
            SDL_FreeSurface(surface);

            return texture;
        }

        public Font GetSize(int size)
        {
            string path = handler.GetPath();
            int at_index = path.LastIndexOf('@');
            if (at_index != -1)
            {
                path = path.Substring(0, at_index);
            }
            path += "@" + size;
            return AssetManager.LoadAsset<Font>(path);
        }
    }


    public class Texture : Asset<IntPtr>
    {
        private Rect? rect = null;
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

        public Rect? GetTextureRect()
        {
            if (this.rect != null)
            {
                return this.rect;
            }

            int w, h;
            SDL_QueryTexture(this.Get(), out _, out _, out w, out h);
            var my_rect = new Rect(w, h);

            // only set the rect if the texture is loaded
            if (IsLoaded())
            {
                this.rect = my_rect;
            }
            return my_rect;
        }
    }

    public abstract class Audio<T> : Asset<T>
    {
        public Audio(AssetHandler<T> handler) : base(handler)
        {
        }

        public abstract bool Play(int loops = 0);
        public abstract bool Stop();
        public abstract bool SetVolume(double volume);
        public abstract bool IsPlaying();

        // TODO: implement fade in/out, stereo panning, distance, set position, etc.
    }

    // Sound class
    // Usually short audio clips that are played once or looped
    // Difference to Music: SDL_mixer only allows one music to be played at a time, but multiple sounds
    // Music is usually longer audio clips that are played in the background
    // Sound should be used for sound effects, music for background music
    public class Sound : Audio<IntPtr>
    {
        private int channel = -1;
        private SDL2.SDL_mixer.ChannelFinishedDelegate ChannelFinishedCallback;
        public Sound(AssetHandler<IntPtr> handler) : base(handler)
        {
            ChannelFinishedCallback = (int channel) =>
            {
                if (channel == this.channel)
                {
                    this.channel = -1;
                }
            };

        }
        // Play the sound
        // int loops: number of times to loop the sound, -1 for infinite looping
        // Can only play one sound at a time
        // To play multiple sounds at the same time (even the same sound/same sound file), create multiple Sound objects
        // If called while the sound is already playing, returns false
        public override bool Play(int loops = 0)
        {
            // Play sound
            IntPtr sound_ref = Get();
            if (sound_ref == IntPtr.Zero)
            {
                Console.WriteLine("Sound: Failed to get sound: " + handler.GetPath());
                return false;
            }
            if (this.channel != -1)
            {
                Stop();
            }
            int sound_channel = SDL2.SDL_mixer.Mix_PlayChannel(-1, sound_ref, loops);
            if (sound_channel == -1)
            {
                Console.WriteLine("Sound: Failed to play sound: " + handler.GetPath());
                return false;
            }
            this.channel = sound_channel;

            SDL2.SDL_mixer.Mix_ChannelFinished(ChannelFinishedCallback);

            return true;
        }

        // Stop the sound if it is playing
        public override bool Stop()
        {
            if (channel == -1)
            {
                return false;
            }
            SDL2.SDL_mixer.Mix_HaltChannel(channel);
            channel = -1;
            return true;
        }

        // Set the volume of the sounds channel
        public override bool SetVolume(double volume)
        {
            //if (channel == -1)
            //{
            //    return false;
            //}
            int result = SDL2.SDL_mixer.Mix_Volume(channel, (int)(volume * 128));
            return result != -1;
        }

        public override bool IsPlaying()
        {
            return channel != -1 && SDL2.SDL_mixer.Mix_Playing(channel) == 1;
        }

        public override IntPtr GetDefault()
        {
            return IntPtr.Zero;
        }
    }

    // Music
    // Usually longer audio clips that are played in the background
    // Difference to Sound: SDL_mixer only allows one music to be played at a time, but multiple sounds
    // Music is usually longer audio clips that are played in the background
    // Sound should be used for sound effects, music for background music
    public class Music : Audio<IntPtr>
    {

        private static string? current_music_path = null;
        public Music(AssetHandler<IntPtr> handler) : base(handler)
        {
        }

        // Play the music
        // int loops: number of times to loop the music, -1 for infinite looping
        // Can only play one music at a time (no matter how many Music objects are created)
        // Playing a new music will stop the currently playing music
        public override bool Play(int loops = 0)
        {
            // Play music
            IntPtr music_ref = Get();
            if (music_ref == IntPtr.Zero)
            {
                Console.WriteLine("Music: Failed to get music: " + handler.GetPath());
                return false;
            }
            int result = SDL2.SDL_mixer.Mix_PlayMusic(music_ref, loops);

            if (result == -1)
            {
                Console.WriteLine("Music: Failed to play music: " + handler.GetPath());
                return false;
            }
            current_music_path = handler.GetPath();
            return true;
        }

        // Stop the music if it is playing
        public override bool Stop()
        {
            SDL2.SDL_mixer.Mix_HaltMusic();
            return true;
        }

        // Set the volume of the music
        public override bool SetVolume(double volume)
        {
            int result = SDL2.SDL_mixer.Mix_VolumeMusic((int)(volume * 128));
            return result != -1;
        }

        // this returns if ANY music is playing, not just this music object
        public override bool IsPlaying()
        {
            return SDL2.SDL_mixer.Mix_PlayingMusic() == 1;
        }

        // This is a bit of a hack to keep track of the currently playing music
        public bool IsThisPlaying()
        {
            var is_playing = IsPlaying();
            if (is_playing && current_music_path == handler.GetPath())
            {
                return true;
            }
            if (!is_playing)
            {
                current_music_path = null;
            }
            return false;
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
            if (loadFailed)
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

    class FontHandler : AssetHandler<IntPtr>
    {
        public FontHandler(string path) : base(path)
        {
        }

        public override bool LoadAsset()
        {
            // Load font
            if (loaded)
            {
                return true;
            }
            if (loadFailed)
            {
                return false;
            }

            // check if path ends with @size
            // if it does, extract the size and load the font with that size
            // if it doesn't, load the font with size 24
            string load_path = path;
            int size = 24;
            int at_index = path.LastIndexOf('@');
            if (at_index != -1)
            {
                string size_str = path.Substring(at_index + 1);
                if (int.TryParse(size_str, out size))
                {
                    load_path = path.Substring(0, at_index);
                }
            }
            if (load_path == "")
            {
                Console.WriteLine("FontHandler: Invalid font path: " + path);
                loadFailed = true;
                return false;
            }


            asset = SDL_ttf.TTF_OpenFont(load_path, size);
            if (asset == IntPtr.Zero)
            {
                Console.WriteLine("FontHandler: Failed to load font: " + path);
                loadFailed = true;
                return false;
            }
            loaded = true;

            return true;
        }

        public override bool UnloadAsset()
        {
            // Unload font
            if (!loaded)
            {
                return false;
            }

            if (asset != IntPtr.Zero)
            {
                SDL_ttf.TTF_CloseFont(asset);
                asset = IntPtr.Zero;
                loaded = false;
                return true;
            }
            return false;
        }
    }

    class TextureHandler : AssetHandler<IntPtr>
    {
        private Rect rect;

        public TextureHandler(string path) : base(path)
        {
        }

        public override bool LoadAsset()
        {


            if (loaded)
            {
                return true;
            }
            if (loadFailed)
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
            int w, h;
            SDL_QueryTexture(asset, out _, out _, out w, out h);
            this.rect = new Rect(100, 100);
            loaded = true;
            return true;
        }

        public Rect GetTextureRect()
        {
            return rect;
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
            if (loadFailed)
            {
                return false;
            }
            asset = SDL2.SDL_mixer.Mix_LoadWAV(path);
            if (asset == IntPtr.Zero)
            {
                Console.WriteLine("SoundHandler: Failed to load sound: " + path);
                loadFailed = true;
                return false;
            }
            loaded = true;

            return true;
        }

        public override bool UnloadAsset()
        {
            // Unload sound
            if (!loaded)
            {
                return false;
            }

            if (asset != IntPtr.Zero)
            {
                SDL2.SDL_mixer.Mix_FreeChunk(asset);
                asset = IntPtr.Zero;
                loaded = false;
                return true;
            }
            return false;
        }

    }

    class MusicHandler : AssetHandler<IntPtr>
    {
        public MusicHandler(string path) : base(path)
        {
        }

        public override bool LoadAsset()
        {
            // Load music
            if (loaded)
            {
                return true;
            }
            if (loadFailed)
            {
                return false;
            }
            asset = SDL2.SDL_mixer.Mix_LoadMUS(path);
            if (asset == IntPtr.Zero)
            {
                Console.WriteLine("MusicHandler: Failed to load music: " + path);
                loadFailed = true;
                return false;
            }
            loaded = true;

            return true;
        }

        public override bool UnloadAsset()
        {
            // Unload music
            if (!loaded)
            {
                return false;
            }

            if (asset != IntPtr.Zero)
            {
                SDL2.SDL_mixer.Mix_FreeMusic(asset);
                asset = IntPtr.Zero;
                loaded = false;
                return true;
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
        private static Dictionary<string, MusicHandler> music_assets = new();
        private static Dictionary<string, FontHandler> font_assets = new();

        public static Texture LoadTexture(string path)
        {
            TextureHandler? handler = null;
            if (texture_assets.ContainsKey(path))
            {
                handler = texture_assets[path];
            }
            else
            {
                handler = new TextureHandler(path);
                texture_assets[path] = handler;
                // handler.LoadAsset();
            }

            return new Texture(handler);
        }

        public static Sound LoadSound(string path)
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

        public static Music LoadMusic(string path)
        {
            MusicHandler? handler = null;
            if (music_assets.ContainsKey(path))
            {
                handler = music_assets[path];
            }
            else
            {
                handler = new MusicHandler(path);
                music_assets[path] = handler;
                // handler.LoadAsset();
            }

            return new Music(handler);
        }

        public static Font LoadFont(string path, int size = -1)
        {
            if (size != -1)
            {
                path += "@" + size;
            }

            FontHandler? handler = null;
            if (font_assets.ContainsKey(path))
            {
                handler = font_assets[path];
            }
            else
            {
                handler = new FontHandler(path);
                font_assets[path] = handler;
                // handler.LoadAsset();
            }

            return new Font(handler);
        }

        /*
         * Can be used to load assets of different types that inherit from Asset<T>
         * If multiple assets with the same path are loaded, the same asset will be returned to avoid loading the same asset multiple times
         * By default, the asset is not loaded when the object is created, but when the Load() or Get() method is called
         * If loading fails for some reason, the Get() method should return a default asset (e.g. a white texture, a silent sound, etc.)
         * 
         * When an asset is no longer needed, the Dispose() method should be called to remove the reference to the asset.
         * The finalizer can also call Dispose() when the object is garbage collected, but it's better to call Dispose() manually.
         */
        public static T LoadAsset<T>(string path) where T : IAsset
        {
            if (typeof(T) == typeof(Texture))
            {
                return LoadTexture(path) as T ?? throw new Exception("Failed to load texture");
            }
            else if (typeof(T) == typeof(Sound))
            {
                return LoadSound(path) as T ?? throw new Exception("Failed to load sound");
            }
            else if (typeof(T) == typeof(Music))
            {
                return LoadMusic(path) as T ?? throw new Exception("Failed to load music");
            }
            else if (typeof(T) == typeof(Font))
            {
                return LoadFont(path) as T ?? throw new Exception("Failed to load font");
            }
            else
            {
                throw new Exception("Unsupported asset type");
            }
        }

        // TODO: implement something that checks for unused assets and unloads them periodically
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
