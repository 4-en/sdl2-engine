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

        public static string ConvertPlatformPath(string path)
        {
            var platform = Environment.OSVersion.Platform;

            // check if windows
            if (platform == PlatformID.Win32NT)
            {
                path = path.Replace("/", "\\");
            } else
            {
                path = path.Replace("\\", "/");
            }
            return path;
        }

        public static void SaveObject<T>(T o, string path) where T : class
        {
            path = GetStorageRoot() + path;

            int last_slash = path.LastIndexOf("/");
            string path_without_file = path.Substring(0, last_slash);
            
            // check if path exists
            if (!System.IO.Directory.Exists(path_without_file))
            {
                // create directory
                System.IO.Directory.CreateDirectory(path_without_file);
            }
            
            // get object type
            Type type = o.GetType();

            // get object variables
            var variables = type.GetFields(); 


            // create a new file
            System.IO.StreamWriter file = new System.IO.StreamWriter(path);

            // write object properties to file
            foreach (var variable in variables)
            {
                file.WriteLine(variable.Name + ": " + variable.GetValue(o)?.ToString());
            }

            // close file
            file.Close();
        }   

        public static T? LoadObject<T>(string path) where T : class, new()
        {
            path = GetStorageRoot() + path;
            // check if path exists
            if (!System.IO.File.Exists(path))
            {
                return null;
            }

            // load the json file
            string json = System.IO.File.ReadAllText(path);

            // create a new object of type T
            T obj = new T();

            Type type = obj.GetType();
            var fields = type.GetFields();

            string[] lines = json.Split('\n');

            foreach (var line in lines)
            {
                if (line == "")
                {
                    continue;
                }
                string[] parts = line.Split(':');
                string fieldName = parts[0].Trim();
                string fieldValue = parts[1].Trim();

                foreach (var field in fields)
                {
                    if (field.Name == fieldName)
                    {
                        field.SetValue(obj, Convert.ChangeType(fieldValue, field.FieldType));
                    }
                }
            }

            return obj;
        }

        public static void SaveArray<T>(T[] array, string path)
        {
            path = GetStorageRoot() + path;

            int last_slash = path.LastIndexOf("/");
            string path_without_file = path.Substring(0, last_slash);
            
            // check if path exists
            if (!System.IO.Directory.Exists(path_without_file))
            {
                // create directory
                System.IO.Directory.CreateDirectory(path_without_file);
            }


            // create a new file
            System.IO.StreamWriter file = new System.IO.StreamWriter(path);

            // write object properties to file
            foreach (var obj in array)
            {
                file.WriteLine(obj);
            }

            // close file
            file.Close();
        }

        public static T[] LoadArray<T>(Func<string, T> instantiator, string path)
        {
            path = GetStorageRoot() + path;
            // check if file exists
            if (!System.IO.File.Exists(path))
            {
                Console.WriteLine("File does not exist: " + path);
                return new T[0];
            }

            // load the file
            string file_content = System.IO.File.ReadAllText(path);


            string[] lines = file_content.Split('\n');

            List<T> objects = new List<T>();
            
            foreach (var line in lines)
            {
                if (line == "")
                {
                    continue;
                }
                objects.Add(instantiator(line));
            }

            return objects.ToArray();
        }
        
    }
    
}
