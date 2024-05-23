using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SDL2Engine.Utils
{
    public class Spawner : Script
    {
        public Prototype? spawnPrototype;
        public int minSpawnAmount = 1;
        public int maxSpawnAmount = 1;
        public virtual int GetSpawnAmount()
        {
            return random.Next(minSpawnAmount, maxSpawnAmount + 1);
        }

        public double spawnRadius = 0;
        public virtual Vec2D GetSpawnPosition()
        {
            var angle = random.NextDouble() * Math.PI * 2;
            var offset = new Vec2D(Math.Cos(angle), Math.Sin(angle)) * spawnRadius;

            return this.gameObject.GetPosition() + offset;
        }

        public double spawnInterval = 10;
        public double lastSpawnTime = 0;
        public virtual bool CanSpawn()
        {
            if (lastSpawnTime + spawnInterval < Time.time)
            {
                lastSpawnTime = Time.time;
                return true;
            }
            return false;
        }

        public override void Update()
        {
            if (spawnPrototype == null)
            {
                return;
            }

            if (CanSpawn())
            {
                for (int i = 0; i < GetSpawnAmount(); i++)
                {
                    var spawn = spawnPrototype.Instantiate();
                    spawn.SetPosition(GetSpawnPosition());
                }
            }
        }

    }
}
