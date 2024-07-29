using SDL2Engine;
using SDL2Engine.Tiled;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static SDL2.SDL;

namespace TileBasedGame
{
    public class Level : Script
    {
        private Player? player;
        private Vec2D spawnPosition = new Vec2D(0, 0);
        private int score = 0;
        public int totalTime = 60 * 5; // 5 minutes
        public double remainingTime = 60 * 5; // 5 minutes

        private Rect levelBounds = new Rect(-49999, -49999, 99999, 99999);

        SoundPlayer? deathSound = null;


        // eventlisteners
        private EventListener<PlayerScoreEvent>? playerScoreListener;
        private EventListener<PlayerDamagedEvent>? playerDamagedListener;

        public override void Start()
        {

            deathSound = AddComponent<SoundPlayer>();
            deathSound?.Load("Assets/Audio/death.mp3");
            deathSound?.SetVolume(0.2);

            player = Player.CreatePlayer();

            // set to position of PlayerSpawn object
            GameObject? player_spawn = Find("PlayerSpawn");
            if (player_spawn != null)
            {
                spawnPosition = player_spawn.GetPosition();
            }

            player.GetGameObject().SetPosition(spawnPosition);

            RegisterEventListeners();

            remainingTime = totalTime;
            CreateLevelUI();

            var tileMapData = FindComponent<TileMapData>();

            if (tileMapData != null)
            {

                double tileHeight = tileMapData.GetTileHeight();
                double tileWidth = tileMapData.GetTileWidth();
                double minWorldX = tileMapData.GetMapStartX() * tileWidth;
                double minWorldY = tileMapData.GetMapStartY() * tileHeight;
                double worldWidth = tileMapData.GetMapWidth() * tileWidth;
                double worldHeight =  tileMapData.GetMapHeight() * tileHeight;

                levelBounds = new Rect(minWorldX-100, minWorldY-500, worldWidth+200, worldHeight+700);
            } else
            {
                Console.WriteLine("No TileMapData component");
            }
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

            if(score_renderer != null) {
                score_renderer.SetText("" + score);
            }
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
            score_renderer.SetText(""+score);
            score_renderer.anchorPoint = AnchorPoint.TopRight;
            score_renderer.color = Color.White;
            score_renderer.SetFontSize(100);
            score_renderer.SetFontPath("Assets/Fonts/Arcadeclassic.ttf");
            score_renderer.SetTextScale(0.1);

            time_object = new GameObject("TimeUI");
            time_renderer = time_object.AddComponent<TextRenderer>();
            time_renderer.relativePosition = true;
            time_object.SetPosition(new Vec2D(0.05, 0.05));
            time_renderer.SetText("0");
            time_renderer.anchorPoint = AnchorPoint.TopLeft;
            time_renderer.color = Color.White;
            time_renderer.SetFontSize(100);
            time_renderer.SetFontPath("Assets/Fonts/Arcadeclassic.ttf");
            time_renderer.SetTextScale(0.1);
            /*
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
            */


        }

        private bool failed = false;
        public void FailLevel()
        {
            if(failed) return;
            failed = true;
            
            Delay(2, ()=> {
                LevelManager.LoadGameOverScene(score, (int)remainingTime);
            });
        }

        public void CompleteLevel()
        {
            LevelManager.CompleteCurrentLevel(score);

            LevelManager.LoadLevelCompletedScene(score, (int) remainingTime);
        }

        private bool paused = false;
        private GameObject? escapeMenu;
        private void Pause()
        {
            paused = true;
            GetScene()?.SetPhysics(false);
        }

        private void Unpause()
        {
            paused = false;
            GetScene()?.SetPhysics(true);
        }

        int lastRenderTime = -1;
        public override void Update()
        {

            if (Input.GetKeyDown(SDL_Keycode.SDLK_PLUS)) {
                Console.WriteLine("Completing level...");
                CompleteLevel();
            }

            // Check for pause input
            if (Input.GetKeyDown((int)SDL_Keycode.SDLK_ESCAPE))
            {
                if (this.escapeMenu != null)
                {
                    this.escapeMenu.Destroy();
                    this.escapeMenu = null;
                }
                else
                {
                    var stopState = !!paused;
                    var escapemenu = UI.EscapeMenu("Paused", () =>
                    {

                        this.paused = stopState;
                        GetScene()?.SetPhysics(true);
                        this.escapeMenu = null;
                        return true;
                    });

                    this.escapeMenu = escapemenu;
                    this.Pause();
                }
            }

            if(paused || failed)
            {
                return;
            }

            if (player == null)
            {
                return;
            }

            if (escapeMenu != null) {
                escapeMenu.Destroy();
                escapeMenu = null;
            }

            remainingTime -= Time.deltaTime;

            if (remainingTime < 0)
            {
                Console.WriteLine("Time's up!");
                this.FailLevel();
                return;
            }

            if(player.GetHealth() <= 0)
            {
                Console.WriteLine("Player died");
                deathSound?.Play();
                this.FailLevel();
                return;
            }

            if (time_renderer != null)
            {
                int renderTime = (int)(remainingTime);
                if (lastRenderTime != renderTime)
                {
                    time_renderer.SetText(renderTime + "s");
                    lastRenderTime = renderTime;
                }
            }

            // only check if player is below bounds
            Vec2D player_position = player.GetGameObject().GetPosition();
            double maxY = levelBounds.y + levelBounds.h;
            if (player_position.y > maxY)
            {
                Console.WriteLine("Player fell out of bounds");
                player.Damage(new Entities.Damage(10000));
                FailLevel();
            }


        }


    }
}
