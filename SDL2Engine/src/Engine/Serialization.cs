using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SDL2Engine
{

    // Classes and methods for serialization and deserialization of game objects and scenes
    public static class Serialization
    {

        /*
         * Returns the directory where data can be stored
         * Specific to the platform
         */
        public static string GetStorageRoot()
        {
            // check platform
            var platform = Environment.OSVersion.Platform;

            string gameName = Engine.gameName;
            // replace spaces with underscores
            gameName = "." + gameName.Replace(" ", "_");

            string path = "";

            // check if windows
            if (platform == PlatformID.Win32NT)
            {
                path = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            }

            // check if linux
            if (platform == PlatformID.Unix)
            {
                path = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            }

            // check if mac
            if (platform == PlatformID.MacOSX)
            {
                path = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            }

            // if path is not "" and doesn't end with a slash, add a slash
            if (path != "" && !path.EndsWith("/"))
            {
                path += "/";
            }

            path += gameName + "/";

            return path;
        }

        public static void Save<T>(T o, string path) where T : class
        {
            path = GetStorageRoot() + path;
            // check if path exists
            if (!System.IO.Directory.Exists(path))
            {
                System.IO.Directory.CreateDirectory(path);
            }

            // check if path ends with a slash
            if (!path.EndsWith("/"))
            {
                path += "/";
            }

            // get object type
            Type type = o.GetType();

            // get object properties
            var properties = type.GetProperties();

            // create a new file
            System.IO.StreamWriter file = new System.IO.StreamWriter(path + type.Name + ".json");

            // write object properties to file
            foreach (var property in properties)
            {
                file.WriteLine(property.Name + ": " + property.GetValue(o));
            }

            // close file
            file.Close();
        }   

        public static T? Load<T>(string path) where T : class, new()
        {
            path = GetStorageRoot() + path;
            // check if path exists
            if (!System.IO.Directory.Exists(path))
            {
                return null;
            }

            // load the json file
            string json = System.IO.File.ReadAllText(path);

            // create a new object of type T
            T obj = new T();

            Type type = obj.GetType();
            var properties = type.GetProperties();

            string[] lines = json.Split('\n');

            foreach (var line in lines)
            {
                string[] parts = line.Split(':');
                string propertyName = parts[0].Trim();
                string propertyValue = parts[1].Trim();

                foreach (var property in properties)
                {
                    if (property.Name == propertyName)
                    {
                        property.SetValue(obj, Convert.ChangeType(propertyValue, property.PropertyType));
                    }
                }
            }

            return obj;
        }
        
    }
    
}
