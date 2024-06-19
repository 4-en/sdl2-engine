using TiledCS;

namespace SDL2Engine
{
    public static class TiledLoader
    {

        private static string AdjustPath(string path, string default_path)
        {
            // check if there is a / or \\ in the path
            if (path.Contains("/") || path.Contains("\\"))
            {
                return path;
            }

            return default_path + path;
        }
        public static List<GameObject> LoadTMX(string path)
        {
            path = AdjustPath(path, "Assets/Tiled/");

            return new List<GameObject>();
        }
    }
}
