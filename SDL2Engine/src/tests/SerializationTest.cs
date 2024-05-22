using SDL2;
using static SDL2.SDL;

namespace SDL2Engine.tests
{
    /*
     * In this test, we will try to serialize and deserialize a scene.
     * First, we will create an empty scene and use inputs to add GameObjects to it.
     * Then we can press s to serialize the scene to a file.
     * After a scene is serialized, we can press l to load the scene from the file.
     * 
     * Ideally, the scene should be the same as the one we serialized.
     */
    internal class SerializationTest
    {

        public static Scene CreateScene()
        {
            Scene scene = new Scene("SerializationTest");
            GameObject gameObject = new GameObject("InputTracker", scene);
            gameObject.AddComponent<InputTracker>();
            return scene;
        }

        public static void Run()
        {
            var scene = CreateScene();
            var engine = new Engine(scene);
            engine.Run();
        }

        public class InputTracker : Script
        {

            private Prototype? forsenEPrototype;

            public override void Start()
            {
                forsenEPrototype = new Prototype("forsenE");
                var renderer = forsenEPrototype.GameObject.AddComponent<SpriteRenderer>();
                renderer.SetSource("Assets/Textures/forsenE.png");

                
            }
            public override void Update()
            {
                if(forsenEPrototype == null)
                {
                    return;
                }

                if(Input.GetMouseButtonDown(Input.MOUSE_BUTTON_LEFT))
                {
                    var gameObject = forsenEPrototype.Instantiate();
                    var mouse_world_pos = GetCamera()?.ScreenToWorld(Input.GetMousePosition());
                    if(mouse_world_pos != null)
                    {
                        gameObject.SetPosition(mouse_world_pos.Value);
                    }
                }

                if(Input.GetKeyDown(SDL_Keycode.SDLK_s))
                {
                    Scene? scene = GetScene();
                    if(scene != null)
                    {
                        SceneSerialization.SaveScene(scene);
                    }
                }

                if(Input.GetKeyDown(SDL_Keycode.SDLK_l))
                {
                    var scene = SceneSerialization.LoadScene("SerializationTest");
                    if (scene != null)
                    {
                        SceneManager.SetScene(scene);
                    }
                }
                
            }
        }
    }

    
}
