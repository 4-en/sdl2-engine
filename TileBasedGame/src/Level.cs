﻿using SDL2Engine;
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

            CreateLevelUI();
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


        private GameObject? score_object;
        private TextRenderer? score_renderer;

        private GameObject? time_object;
        private TextRenderer? time_renderer;

        private GameObject? health_object;
        private TextRenderer? health_renderer;

        private void CreateLevelUI()
        {
            score_object = new GameObject("ScoreUI");
            score_renderer = score_object.AddComponent<TextRenderer>();
            score_renderer.relativePosition = true;
            score_renderer.relativeToCamera = true;
            score_object.SetPosition(new Vec2D(0.95, 0.05));
            score_renderer.SetText("Score " + score);
            score_renderer.anchorPoint = AnchorPoint.TopRight;
            score_renderer.color = Color.White;
            score_renderer.SetFontSize(100);
            score_renderer.SetFontPath("Assets/Fonts/Arcadeclassic.ttf");
            score_renderer.SetTextScale(0.1);

            time_object = new GameObject("TimeUI");
            time_renderer = time_object.AddComponent<TextRenderer>();
            time_renderer.relativePosition = true;
            time_object.SetPosition(new Vec2D(0.05, 0.05));
            time_renderer.SetText("Time 0");
            time_renderer.anchorPoint = AnchorPoint.TopLeft;
            time_renderer.color = Color.White;
            time_renderer.SetFontSize(100);
            time_renderer.SetFontPath("Assets/Fonts/Arcadeclassic.ttf");
            time_renderer.SetTextScale(0.1);

            health_object = new GameObject("HealthUI");
            health_renderer = health_object.AddComponent<TextRenderer>();
            health_renderer.relativePosition = true;
            health_object.SetPosition(new Vec2D(0.05, 0.95));
            health_renderer.SetText("Health 69");
            health_renderer.anchorPoint = AnchorPoint.BottomLeft;
            health_renderer.color = Color.White;
            health_renderer.SetFontSize(100);
            health_renderer.SetFontPath("Assets/Fonts/Arcadeclassic.ttf");
            health_renderer.SetTextScale(0.1);


        }

        public override void Update()
        {
        }


    }
}