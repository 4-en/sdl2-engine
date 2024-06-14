using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SDL2Engine;

namespace ShootEmUp
{
    public class EnemyKilledEvent
    {
        public BaseEnemy enemy;

        public EnemyKilledEvent(BaseEnemy enemy)
        {
            this.enemy = enemy;
        }
    }
}
