﻿using SDL2;
using SDL2Engine.Coro;
using System.Collections;
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

        public class TestScript : Script
        {
            public override void Start()
            {
                
                Delay(2.0, () =>
                {
                    this.gameObject.Destroy();
                });
            }
        }

        public static Scene CreateScene()
        {
            Scene scene = new Scene("SerializationTest");
            GameObject gameObject = new GameObject("InputTracker", scene);
            gameObject.AddComponent<InputTracker>();
            return scene;
        }

        public static void Run()
        {
            Console.WriteLine("Running SerializationTest");
            Console.WriteLine("Press left mouse button to add a GameObject to the scene");
            Console.WriteLine("Press s to save the scene to a file");
            Console.WriteLine("Press l to load the scene from a file");
            Console.WriteLine("Press t to load a SceneTemplate from a file");
            Console.WriteLine("Press c to test CoroutineManager");
            Console.WriteLine("Example: Press left mouse button a few times to add a GameObject, then press s to save the scene.");
            Console.WriteLine("         Restart the program and press l to load the scene you saved.");
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
                var circleCollider = forsenEPrototype.GameObject.AddComponent<CircleCollider>();
                circleCollider.SetRadius(100);
                renderer.SetSource("forsenE.png");
                forsenEPrototype.GameObject.AddComponent<TestScript>();


            }

            public IEnumerator TestCoro()
            {
                Console.WriteLine("Starting coroutine");
                yield return 1.0;
                Console.WriteLine("Coroutine waited 1.0 seconds");
                yield return 1000ul;
                Console.WriteLine("Coroutine waited 1000 frames");
                yield return null;
                Console.WriteLine("Coroutine waited 1 frame");
                yield return 3.0;
                Console.WriteLine("Coroutine waited 3.0 seconds");
                var gameObject = Prototype.Instantiate("forsenE");
                if (gameObject == null)
                {
                    Console.WriteLine("Failed to instantiate forsenE");
                    yield break;
                }
                var mouse_world_pos = GetCamera()?.ScreenToWorld(Input.GetMousePosition());
                if (mouse_world_pos != null)
                {
                    gameObject.SetPosition(mouse_world_pos.Value);
                }
                yield break;
            }

            public override void Update()
            {
                if (forsenEPrototype == null)
                {
                    return;
                }

                if (Input.GetMouseButtonDown(Input.MOUSE_BUTTON_LEFT))
                {
                    var gameObject = Prototype.Instantiate("forsenE");
                    if (gameObject == null)
                    {
                        Console.WriteLine("Failed to instantiate forsenE");
                        return;
                    }
                    var mouse_world_pos = GetCamera()?.ScreenToWorld(Input.GetMousePosition());
                    if (mouse_world_pos != null)
                    {
                        gameObject.SetPosition(mouse_world_pos.Value);
                    }
                }

                if (Input.GetKeyDown(SDL_Keycode.SDLK_s))
                {
                    Scene? scene = GetScene();
                    if (scene != null)
                    {
                        SceneSerialization.SaveScene(scene);
                    }
                    else
                    {
                        Console.WriteLine("Failed to save scene");
                    }
                }

                if (Input.GetKeyDown(SDL_Keycode.SDLK_l))
                {
                    var scene = SceneSerialization.LoadScene("SerializationTest");
                    if (scene != null)
                    {
                        SceneManager.SetScene(scene);
                    }
                }

                // test SceneTemplate
                if (Input.GetKeyDown(SDL_Keycode.SDLK_t))
                {
                    var objects = SceneTemplate.Load("test.template");
                    Console.WriteLine($"Loaded {objects.Count} objects");
                }

                if (Input.GetKeyDown(SDL_Keycode.SDLK_ESCAPE))
                {
                    Engine.Stop();
                }

                if (Input.GetKeyDown(SDL_Keycode.SDLK_c))
                {
                    StartCoroutine(TestCoro());
                }

                if (Input.GetKeyDown(SDL_Keycode.SDLK_j))
                {
                    // try to send a async request
                    string url = "https://v2.jokeapi.dev/joke/Any?format=txt";
                    StartCoroutine(TestRequest(url));
                }   

            }

            private IEnumerator TestRequest(string url)
            {
                var client = new HttpClient();
                Task<string> task = client.GetStringAsync(url);
                yield return task;
                Console.WriteLine("Received joke. Waiting for good timing...");
                yield return 2.0;
                Console.WriteLine(task.Result);
            }
        }
    }


}
