using SDL2Engine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TileBasedGame
{
    public class Level : Script
    {
        private Player? player;
        private Vec2D spawnPosition = new Vec2D(0, 0);
        private int score = 0;

        public override void Start()
        {
            player = Player.CreatePlayer();

            // set to position of PlayerSpawn object
            GameObject? player_spawn = Find("PlayerSpawn");
            if (player_spawn != null)
            {
                spawnPosition = player_spawn.GetPosition();
            }

            player.GetGameObject().SetPosition(spawnPosition);
        }


        public override void Update()
        {
        }


    }
}
