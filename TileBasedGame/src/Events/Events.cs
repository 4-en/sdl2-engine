using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SDL2Engine;
using TileBasedGame.Entities;

namespace TileBasedGame
{

    public class EnemyKilledEvent
    {
        public IEnemy enemy;

        public EnemyKilledEvent(IEnemy enemy)
        {
            this.enemy = enemy;
        }
    }

    // TODO: Implement this event
    public class ItemPickedUpEvent
    {
        
    }

    public class PlayerDamagedEvent
    {
        public IDamageable player;
        public Damage damage;

        public PlayerDamagedEvent(IDamageable player, Damage damage)
        {
            this.player = player;
            this.damage = damage;
        }
    }
}
