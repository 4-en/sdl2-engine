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
                
                var adder = Component.CreateWithGameObject<ObjectAdder>();
                adder.Item1.KeepInScene = true;

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

        public class ObjectAdder : Script
        {
            private int alreadyAdded = 0;
            private int toAdd = ChunkTest.OBJECT_COUNT;
            private readonly int addsPerFrame = 10;
            private readonly int objectsPerColumn = 100;

            public override void Update()
            {
                if (alreadyAdded >= toAdd)
                {
                    gameObject.Destroy();
                    return;
                }

                for (int i = 0; i < addsPerFrame; i++)
                {
                    var gameObject = new GameObject("forsenE");
                    var renderer = gameObject.AddComponent<SpriteRenderer>();
                    renderer.SetSource("forsenE.png");
                    renderer.SetWorldSize(new Vec2D(80, 80));

                    double xPos = (alreadyAdded % objectsPerColumn) * 100;
                    double yPos = (alreadyAdded / objectsPerColumn) * 100;

                    gameObject.SetPosition(new Vec2D(xPos, yPos));

                    BoxCollider.FromDrawableRect(gameObject);
                    gameObject.AddComponent<OnHoverDestoyer>();
                    alreadyAdded++;
                }
            }
        }

        public class CameraMover : Script
        {
            double speed = 1000;
            double posX = -1000;
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
                    posX = -1000;
                }

                cam?.SetPosition(new Vec2D(posX, 0));
            }
        }
    }
}
