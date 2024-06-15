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
        public int currentSpawns = 0;
        public double maxTime = -1;
        private double creationTime = 0;

        public Spawner()
        {
            this.creationTime = Time.time;

            if(prototype == null)
            {
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
            return maxSpawns != -1 && currentSpawns >= maxSpawns || maxTime != -1 && Time.time - creationTime >= maxTime;
        }

        public override void Update()
        {
            spawnTimer += Time.deltaTime;

            if (IsDoneSpawning() || prototype == null)
            {
                gameObject.Destroy();
                return;
            }

            if (spawnTimer >= spawnRate && currentSpawns < maxSpawns)
            {
                spawnTimer = 0;
                currentSpawns++;
                
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
}
