using SDL2Engine.Utils;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static SDL2.SDL;
using static SDL2Engine.tests.SerializationTest;

namespace SDL2Engine.tests
{
    internal class ChunkTest
    {
        private static readonly int OBJECT_COUNT = 100_000;
        public static Scene CreateScene()
        {
            Scene scene = new ChunkedScene("ChunkTest");

            using (scene.Activate())
            {
                int rows = 100;
                int cols = OBJECT_COUNT / rows;

                for (int i = 0; i < rows; i++)
                {
                    for (int j = 0; j < cols; j++)
                    {
                        var gameObject = new GameObject("forsenE");
                        var renderer = gameObject.AddComponent<SpriteRenderer>();
                        renderer.SetSource("forsenE.png");
                        renderer.SetWorldSize(new Vec2D(80, 80));
                        gameObject.SetPosition(new Vec2D(j * 100, i * 100));
                        BoxCollider.FromDrawableRect(gameObject);
                        gameObject.AddComponent<OnHoverDestoyer>();

                    }
                }

                var t = Component.CreateWithGameObject<CameraMover>();
                t.Item1.KeepInScene = true;
            }

            return scene;
        }

        public static void Run()
        {
            Console.WriteLine("Running Chunk Test");
            Console.WriteLine($"Creating {OBJECT_COUNT} objects with SpriteRenderer and BoxCollider");
            var scene = CreateScene();
            var engine = new Engine(scene);
            engine.Run();
        }

        public class OnHoverDestoyer : Script
        {
            private BoxCollider? collider = null;

            public override void Start()
            {
                collider = GetComponent<BoxCollider>();
            }

            public override void Update()
            {
                if(collider == null)
                {
                    return;
                }

                var screenRect = GetCamera().WorldToScreen(collider.GetCollisionBox());
                if(MouseHelper.IsRectHovered(screenRect))
                {
                    Destroy(gameObject);
                }
            }
        }

        public class CameraMover : Script
        {
            double speed = 1000;
            double posX = 0;
            double endPos = 2000;
            Camera? cam = null;
            public override void Start()
            {
                endPos = OBJECT_COUNT + 2000;
                cam = GetCamera();
            }

            public override void Update()
            {
                posX += speed * Time.deltaTime;

                if (posX > endPos)
                {
                    posX = 0;
                }

                cam?.SetPosition(new Vec2D(posX, 0));
            }
        }
    }
}
