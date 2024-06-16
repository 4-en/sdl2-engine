using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SDL2Engine;
using ShootEmUp.Entities;

namespace ShootEmUp
{

    public interface IEnemy
    {
        Team GetTeam()
        {
            return Team.Enemy;
        }

        int GetPoints()
        {
            return 1;
        }
    }
    public class EnemyKilledEvent
    {
        public IEnemy enemy;

        public EnemyKilledEvent(IEnemy enemy)
        {
            this.enemy = enemy;
        }
    }
}
