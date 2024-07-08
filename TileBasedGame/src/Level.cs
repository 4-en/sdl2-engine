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


        // eventlisteners
        private EventListener<PlayerScoreEvent>? playerScoreListener;
        private EventListener<PlayerDamagedEvent>? playerDamagedListener;

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

            RegisterEventListeners();
        }


        private void RegisterEventListeners() {
            if(playerScoreListener != null || playerDamagedListener != null) {
                return;
            }
            playerScoreListener = EventBus.AddListener<PlayerScoreEvent>(OnPlayerScoreEvent);
            playerDamagedListener = EventBus.AddListener<PlayerDamagedEvent>(OnPlayerDamagedEvent);
        }

        private void UnregisterEventListeners() {
            if(playerScoreListener != null) {
                EventBus.RemoveListener(playerScoreListener);
                playerScoreListener = null;
            }
            if(playerDamagedListener != null) {
                EventBus.RemoveListener(playerDamagedListener);
                playerDamagedListener = null;
            }
        }

        public override void OnDestroy()
        {
            base.OnDestroy();
            UnregisterEventListeners();
        }


        private void OnPlayerScoreEvent(PlayerScoreEvent e)
        {
            score += e.score;
        }

        private void OnPlayerDamagedEvent(PlayerDamagedEvent e)
        {
            if (e.player == player)
            {
                Console.WriteLine("Player damaged: " + e.damage.Value);
            }
        }

        public override void Update()
        {
        }


    }
}
