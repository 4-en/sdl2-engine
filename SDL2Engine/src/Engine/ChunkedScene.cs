﻿

using System.Runtime.CompilerServices;

namespace SDL2Engine
{

    public class ChunkMap
    {
        private int chunkSize = 128;

        private Dictionary<string, List<GameObject>> chunks = new Dictionary<string, List<GameObject>>();

        private int lastChunkX1 = -420420;
        private int lastChunkY1 = -420420;
        private int lastChunkX2 = -420420;
        private int lastChunkY2 = -420420;


        public ChunkMap()
        {
        }

        private string GetChunkKey(int x, int y)
        {
            return x + "_" + y;
        }

        public void AddGameObject(GameObject gameObject)
        {
            Vec2D position = gameObject.GetPosition();
            int chunkX = (int)position.x / chunkSize;
            int chunkY = (int)position.y / chunkSize;

            string key = GetChunkKey(chunkX, chunkY);
            if (!chunks.ContainsKey(key))
            {
                chunks[key] = new List<GameObject>();
            }
            chunks[key].Add(gameObject);
        }

        public void RemoveGameObject(GameObject gameObject)
        {
            Vec2D position = gameObject.GetPosition();
            int chunkX = (int)position.x / chunkSize;
            int chunkY = (int)position.y / chunkSize;

            string key = GetChunkKey(chunkX, chunkY);
            if (chunks.ContainsKey(key))
            {
                chunks[key].Remove(gameObject);
            }
        }

        public List<GameObject> GetGameObjectsInChunk(int x, int y)
        {
            string key = GetChunkKey(x, y);
            if (chunks.ContainsKey(key))
            {
                return chunks[key];
            }
            return new List<GameObject>();
        }

        public IEnumerator<GameObject> LoadInBounds(Rect bounds)
        {
            int x1 = (int)bounds.x / chunkSize;
            int y1 = (int)bounds.y / chunkSize;
            int x2 = (int)(bounds.x + bounds.w) / chunkSize;
            int y2 = (int)(bounds.y + bounds.h) / chunkSize;

            if(x1 == lastChunkX1 && y1 == lastChunkY1 && x2 == lastChunkX2 && y2 == lastChunkY2)
            {
                yield break;
            }

            for(int x = x1; x <= x2; x++)
            {
                if(x>=lastChunkX1 && x<=lastChunkX2)
                {
                    continue;
                }
                for (int y = y1; y <= y2; y++)
                {
                    if (y >= lastChunkY1 && y <= lastChunkY2)
                    {
                        continue;
                    }
                    string key = GetChunkKey(x, y);
                    if (chunks.ContainsKey(key))
                    {
                        foreach (GameObject gameObject in chunks[key])
                        {
                            yield return gameObject;
                        }

                        // clear the chunk
                        chunks[key].Clear();

                        // remove the chunk
                        chunks.Remove(key);

                    }
                }
            }

            lastChunkX1 = x1;
            lastChunkY1 = y1;
            lastChunkX2 = x2;
            lastChunkY2 = y2;
        }
    }


    /*
     * A scene that uses tilemaps to load and unload game objects.
     * 
     * Most Important features
     * - Parse tilemap files and create GameObjects based on tile data
     * - store GameObjects in chunks based on their position
     * - Unload GameObjects that are out of Camera view
     * - Keep track of GameObjects that were destroyed during the scene, so
     *   they are not reloaded when the Camera view moves back to them
     */
    public class ChunkedScene : Scene
    {

        private ChunkMap chunkMap = new ChunkMap();

        
        
    }
}