using SDL2Engine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShootEmUp.Entities
{
    public class Spawner : Script
    {
        public string PrototypeName = "";
        public Prototype? prototype;
        public double spawnRate = 1;
        public double spawnTimer = 0;
        public int maxSpawns = -1;
        public int totalSpawns = 0;
        public double maxTime = -1;
        private double creationTime = 0;

        public static Prototype CreatePrototype()
        {
            var prototype = new Prototype("Spawner");
            prototype.AddComponent<Spawner>();
            return prototype;
        }

        public override void Start()
        {
            this.creationTime = Time.time;

            if(prototype == null)
            {
                if(PrototypeName == "")
                {
                    Console.WriteLine("Spawner has no prototype name");
                    gameObject.Destroy();
                    return;
                }
                var protoAsset = AssetManager.LoadPrototype(PrototypeName);
                if(protoAsset != null)
                {
                    prototype = protoAsset.Get();
                }
            }

            if(prototype == null)
            {
                Console.WriteLine("Spawner could not find prototype: " + PrototypeName);
                gameObject.Destroy();
            }
        }

        private bool IsDoneSpawning()
        {
            return (maxSpawns != -1 && totalSpawns >= maxSpawns) || (maxTime != -1 && Time.time - creationTime >= maxTime);
        }

        public override void Update()
        {
            spawnTimer += Time.deltaTime;

            if (IsDoneSpawning() || prototype == null)
            {
                gameObject.Destroy();
                return;
            }

            if (spawnTimer >= spawnRate)
            {
                spawnTimer = 0;
                totalSpawns++;
                
                Spawn();
            }
        }

        protected virtual void Spawn()
        {
            if(prototype == null)
            {
                return;
            }

            var spawned = prototype.Instantiate();

            spawned.transform.position = gameObject.transform.position;
        }
    }

    public class EdgeSpawner : Spawner
    {
        public Rect field = new Rect(-2500, -2500, 5000, 5000);

        new public static Prototype CreatePrototype()
        {
            var prototype = new Prototype("EdgeSpawner");
            prototype.AddComponent<EdgeSpawner>();
            return prototype;
        }

        protected override void Spawn()
        {
            if (prototype == null)
            {
                return;
            }

            var spawned = prototype.Instantiate();

            int side = random.Next(4);

            double tolerance = 100;
            double x = 0;
            double y = 0;

            double xVel = 0;
            double yVel = 0;

            switch(side)
            {
                case 0:
                    x = field.x - tolerance;
                    y = random.Next((int)field.y, (int)(field.y + field.h));
                    
                    xVel = random.Next(50, 100);
                    yVel = random.Next(-50, 50);
                    break;
                case 1:
                    x = field.x + field.w + tolerance;
                    y = random.Next((int)field.y, (int)(field.y + field.h));

                    xVel = random.Next(-100, -50);
                    yVel = random.Next(-50, 50);
                    break;
                case 2:
                    x = random.Next((int)field.x, (int)(field.x + field.w));
                    y = field.y - tolerance;

                    xVel = random.Next(-50, 50);
                    yVel = random.Next(50, 100);
                    break;
                case 3:
                    x = random.Next((int)field.x, (int)(field.x + field.w));
                    y = field.y + field.h + tolerance;

                    xVel = random.Next(-50, 50);
                    yVel = random.Next(-100, -50);
                    break;
                default:
                    break;
            }

            spawned.transform.position = new Vec2D(x, y);
            spawned.GetComponent<PhysicsBody>()?.SetVelocity(new Vec2D(xVel, yVel));



        }
    }

    public class RectSpawner : Spawner
    {
        public Rect spawnArea = new Rect(0, 0, 100, 100);
        public Rect directionArea = new Rect(0, 0, 100, 100);
        public double minSpeed = 50;
        public double maxSpeed = 100;

        new public static Prototype CreatePrototype()
        {
            var prototype = new Prototype("RectSpawner");
            prototype.AddComponent<RectSpawner>();
            return prototype;
        }

        protected override void Spawn()
        {
            if (prototype == null)
            {
                return;
            }

            var spawned = prototype.Instantiate();

            double x = random.Next((int)spawnArea.x, (int)(spawnArea.x + spawnArea.w));
            double y = random.Next((int)spawnArea.y, (int)(spawnArea.y + spawnArea.h));

            spawned.transform.position = new Vec2D(x, y);

            Rect direction = spawnArea;
            if(directionArea.w > 0 || directionArea.h > 0)
            {
                direction = directionArea;
            }

            double xVel = random.Next((int)direction.x, (int)(direction.x + direction.w)) - x;
            double yVel = random.Next((int)direction.y, (int)(direction.y + direction.h)) - y;

            double speed = random.Next((int)minSpeed, (int)maxSpeed);

            spawned.GetComponent<PhysicsBody>()?.SetVelocity(new Vec2D(xVel, yVel).Normalize() * speed);
        }
    }
}
