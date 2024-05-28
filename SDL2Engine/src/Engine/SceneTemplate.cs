using System;
using System.Xml;

namespace SDL2Engine
{
    /*
     * SceneTemplates are used to load GameObjects from a file.
     * The GameObjects are created by using named Prototype objects.
     * Additionally, the GameObjects can have custom attributes set,
     * for example, the position of the GameObject.
     * 
     * The files have the following format:
     *    A list of top level XML elements, each representing a GameObject:
     *       <Name />
     *       or
     *       <Name attribute1="value1" attribute2="value2" ... />
     *       or
     *       <Name attribute1="value1" attribute2="value2" ...>child1 child2 ...</Name>
     */
    public static class SceneTemplate
    {

        private static IEnumerator<XmlElement> LoadElements(string path)
        {
            string content = File.ReadAllText(path);
            if (content == null || content == "")
            {
                yield break;
            }

            // hack to make the XML valid if there are multiple top level elements
            // shouldn't be a problem as long as the files are small
            string fakeRoot = "<root>" + content + "</root>";
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(fakeRoot);
            var element = doc.DocumentElement;
            if (element == null)
            {
                yield break;
            }

            foreach (XmlElement child in element.ChildNodes)
            {
                yield return child;
            }
        }

        private static void LoadPosition(GameObject gameObject, XmlElement element)
        {
            string[] positionAttributes = { "x", "y", "z", "xr", "yr", "zr" };
            // check if any of the position attributes are set
            bool hasPositionAttributes = false;
            foreach (string attribute in positionAttributes)
            {
                if (element.HasAttribute(attribute))
                {
                    hasPositionAttributes = true;
                    break;
                }
            }
            if (!hasPositionAttributes)
            {
                return;
            }

            var string_to_int_seed = (string s) =>
            {
                int seed = 0;
                foreach (char c in s)
                {
                    seed += (int)c;
                    seed <<= 1;
                }
                return seed;
            };
            int seed = string_to_int_seed(gameObject.GetName());

            var pos = gameObject.GetPosition();
            foreach (string attribute in positionAttributes)
            {
                if (element.HasAttribute(attribute))
                {
                    double value = double.Parse(element.GetAttribute(attribute));
                    switch (attribute)
                    {
                        case "x":
                            pos.x = value;
                            break;
                        case "y":
                            pos.y = value;
                            break;
                        case "z":
                            pos.z = value;
                            break;
                        case "xr":
                            pos.x += value * Rand.StableRandom.NextDouble(seed + (int)(pos.x + 5 * pos.y + 7 * pos.z));
                            break;
                        case "yr":
                            pos.y += value * Rand.StableRandom.NextDouble(seed + (int)(5 * pos.x + 7 * pos.y + pos.z));
                            break;
                        case "zr":
                            pos.z += value * Rand.StableRandom.NextDouble(seed + (int)(7 * pos.x + pos.y + 5 * pos.z));
                            break;
                    }
                }
            }

            gameObject.SetPosition(pos);
        }

        private static void set_attribute(object obj, string attribute, string value)
        {
            var type = obj.GetType();
            string[] parts = attribute.Split('.');
            if (parts.Length < 1)
            {
                return;
            }
            attribute = parts[0];
            string otherAttributes = string.Join(".", parts.Skip(1));
            var property = type.GetProperty(attribute);
            if (property == null)
            {
                Console.WriteLine("Property not found: " + attribute);
                return;
            }

            if (otherAttributes == "")
            {
                property.SetValue(obj, value);
            }
            else
            {
                var subObj = property.GetValue(obj);
                if (subObj == null)
                {
                    return;
                }
                set_attribute(subObj, otherAttributes, value);
            }
        }

        private static void LoadAttributes(GameObject gameObject, XmlElement element)
        {
            // handle special attributes here
            // x, y, z:
            LoadPosition(gameObject, element);


            // load attributes in the format of componentName.attributeName[.subAttributeName] = value
            foreach (XmlAttribute attribute in element.Attributes)
            {
                string[] parts = attribute.Name.Split('.');
                if (parts.Length < 2)
                {
                    continue;
                }
                string componentName = parts[0];
                string other = string.Join(".", parts.Skip(1));

                Component? component = gameObject.GetComponentByClassName(componentName);
                if (component == null)
                {
                    continue;
                }

                set_attribute(component, other, attribute.Value);
            }

        }

        private static GameObject? CreateGameObject(XmlElement element)
        {
            var name = element.Name;
            Prototype? prototype = AssetManager.LoadPrototype(name).Get();
            if (prototype == null)
            {
                return null;
            }
            GameObject gameObject = prototype.Instantiate();

            LoadAttributes(gameObject, element);

            return gameObject;
        }

        public static List<GameObject> Load(string path)
        {
            List<GameObject> gameObjects = new List<GameObject>();
            var elements = LoadElements(path);
            while (elements.MoveNext())
            {
                var element = elements.Current;
                GameObject? gameObject = CreateGameObject(element);
                if (gameObject != null)
                {
                    gameObjects.Add(gameObject);
                }
            }
            return gameObjects;
        }

    }
}
