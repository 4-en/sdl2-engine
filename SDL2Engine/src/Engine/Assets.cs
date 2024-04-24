using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SDL2Engine
{

    public class Assets
    {
        string name;
        string path;
        IntPtr asset;
        uint refCount = 0;

        public Assets(string name, string path)
        {
            this.name = name;
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

    public class AssetHandle
    {
        Assets asset;
        public AssetHandle(Assets asset)
        {
            this.asset = asset;
            asset.AddRef();
        }

        ~AssetHandle()
        {
            asset.RemoveRef();
        }

    }

    // Asset manager class
    // This class should be used to load and manage game assets such as textures, sounds, etc.
    // To use an asset, a function should load the game assets with the AssetManager.LoadAsset method
    // This should help to avoid loading the same asset multiple times
    // The AssetManager should also be able to preload game assets, for example when loading a scene
    public static class AssetManager
    {
        private static HashSet<Assets> assets = new HashSet<Assets>();

        public static AssetHandle LoadAsset(string name, string path)
        {
            Assets asset = new Assets(name, path);
            assets.Add(asset);
            return new AssetHandle(asset);
        }

        public static void CleanUnusedAssets()
        {
            foreach (Assets asset in assets)
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
    public static class  AudioManager
    {
        
    }



}
