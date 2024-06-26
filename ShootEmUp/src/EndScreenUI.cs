﻿using SDL2Engine.Utils;
using SDL2Engine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static SDL2.SDL;

namespace ShootEmUp
{

    public static class EndScreen
    {
        public static Scene CreateScene()
        {
            var scene = new Scene("End Screen");
            

            using (scene.Activate())
            {
                GameObject highscore = new GameObject("Highscore");
                var highscoreScript = highscore.AddComponent<HighscoreScript>();
                var hs = new OnlineHighscores<int>(100, "SpaceShooter");
                highscoreScript.SetHighscores(hs);

                highscoreScript.AddHighscoreState(PlayerData.Instance.TotalScore);
            }

            return scene;
        }
    }

    public enum HighscoreState
    {
        None,
        EnterName,
        ShowHighscores
    }
    public class HighscoreScript : Script
    {
        private TextRenderer? textRenderer = null;

        private TextRenderer? scoreText = null;
        private TextRenderer? nameText = null;
        private TextRenderer? highscoresTitle = null;
        private Highscores<int> highscores = new Highscores<int>();

        private HighscoreState state = HighscoreState.None;

        public void SetHighscores(Highscores<int> highscores)
        {
            this.highscores = highscores;
        }

        public override void Start()
        {

            var backgroundOverlay = Component.CreateWithGameObject<FilledRect>("HighscoreBackground");
            gameObject.AddChild(backgroundOverlay.Item1);
            backgroundOverlay.Item2.color = new Color(10, 10, 20, 200);
            backgroundOverlay.Item2.SetRect(new Rect(1920, 1080));
            backgroundOverlay.Item2.anchorPoint = AnchorPoint.TopLeft;

            var highscoresTitle = Component.CreateWithGameObject<TextRenderer>("HighscoresTitle");
            gameObject.AddChild(highscoresTitle.Item1);
            highscoresTitle.Item2.color = new Color(255, 255, 255, 205);
            highscoresTitle.Item2.SetFontSize(100);
            highscoresTitle.Item2.SetText("- Highscores -");
            highscoresTitle.Item2.anchorPoint = AnchorPoint.TopCenter;
            highscoresTitle.Item2.SetPreferredSize(new Rect(700, 900));
            highscoresTitle.Item2.SetBackgroundColor(new Color(0, 0, 0, 100));
            highscoresTitle.Item1.SetLocalPosition(new Vec2D(1920 / 2, 50));
            highscoresTitle.Item2.SetBorderSize(5);
            highscoresTitle.Item2.SetBorderColor(new Color(255, 255, 255, 255));


            this.highscoresTitle = highscoresTitle.Item2;

            var resetButton = Component.CreateWithGameObject<TextRenderer>("ResetButton");
            gameObject.AddChild(resetButton.Item1);
            resetButton.Item2.color = new Color(255, 255, 255, 205);
            resetButton.Item2.SetFontSize(50);
            resetButton.Item2.SetText("Restart");
            resetButton.Item2.anchorPoint = AnchorPoint.Center;
            resetButton.Item2.SetPreferredSize(new Rect(200, 100));
            resetButton.Item2.SetBackgroundColor(new Color(0, 0, 0, 100));
            resetButton.Item1.SetLocalPosition(new Vec2D(250, 150));
            resetButton.Item2.SetBorderSize(2);
            resetButton.Item2.SetBorderColor(new Color(255, 255, 255, 255));
            var helper = resetButton.Item1.AddComponent<TextRenderHelper>();
            helper.OnClick += (object? _, TextRenderer _) =>
            {
                Destroy(gameObject);
                LevelManager.StartNewRun();
            };
            helper.OnHover += (object? _, TextRenderer _) =>
            {
                resetButton.Item2.SetBackgroundColor(new Color(123, 0, 0, 150));
            };

            helper.OnLeave += (object? _, TextRenderer _) =>
            {
                resetButton.Item2.SetBackgroundColor(new Color(0, 0, 0, 100));
            };

            var menuButton = Component.CreateWithGameObject<TextRenderer>("MenuButton");
            gameObject.AddChild(menuButton.Item1);
            menuButton.Item2.color = new Color(255, 255, 255, 205);
            menuButton.Item2.SetFontSize(50);
            menuButton.Item2.SetText("Menu");
            menuButton.Item2.anchorPoint = AnchorPoint.Center;
            menuButton.Item2.SetPreferredSize(new Rect(200, 100));
            menuButton.Item2.SetBackgroundColor(new Color(0, 0, 0, 100));
            menuButton.Item1.SetLocalPosition(new Vec2D(250, 300));
            menuButton.Item2.SetBorderSize(2);
            menuButton.Item2.SetBorderColor(new Color(255, 255, 255, 255));
            var menuHelper = menuButton.Item1.AddComponent<TextRenderHelper>();
            menuHelper.OnClick += (object? _, TextRenderer _) =>
            {
                
                LevelManager.LoadHomeScreen();
            };

            menuHelper.OnHover += (object? _, TextRenderer _) =>
            {
                menuButton.Item2.SetBackgroundColor(new Color(123, 0, 0, 150));
            };

            menuHelper.OnLeave += (object? _, TextRenderer _) =>
            {
                menuButton.Item2.SetBackgroundColor(new Color(0, 0, 0, 100));
            };

        }

        private List<Tuple<string, string>> GetHighscores()
        {
            return this.highscores.AsString(10);
            /*
            string env_name = Environment.UserName;
            
            return new List<Tuple<string, string>>()
            {
                new Tuple<string, string>(env_name, "100"),
                new Tuple<string, string>("Player 2", "90"),
                new Tuple<string, string>("Player 3", "80"),
                new Tuple<string, string>("Player 4", "70"),
                new Tuple<string, string>("Player 5", "60"),
                new Tuple<string, string>("Player 6", "50"),
                new Tuple<string, string>("Player 7", "40"),
                new Tuple<string, string>("Player 8", "30"),
                new Tuple<string, string>("Player 9", "20"),
                new Tuple<string, string>("Player 10", "10"),
            };
            */

        }

        public void SetState(HighscoreState state)
        {
            this.state = state;

            switch (state)
            {
                case HighscoreState.None:
                    break;
                case HighscoreState.EnterName:
                    break;
                case HighscoreState.ShowHighscores:
                    break;
            }
        }

        public void AddHighscoreState(int score)
        {
            SetState(HighscoreState.ShowHighscores);


            // get windows/linux/mac username
            string name = Environment.UserName;
            Task.Run(() =>
            {
                highscores.Update();
                highscores.AddHighscore(name, score, true);
                SetHighscores(GetHighscores());
            });

        }

        public override void Update()
        {

            if (Input.GetKeyDown(SDL_Keycode.SDLK_h))
            {
                gameObject.ToggleEnabled();

                // since gameObject disables all children and components
                // we need to re-enable this script, otherwise we can't enable the gameObject again
                this.Enable();

            }
        }

        public void SetHighscores(List<Tuple<string, string>> scores)
        {
            string nameString = "";
            string scoreString = "";
            string playerScore = PlayerData.Instance.TotalScore.ToString();
            foreach (var score in scores)
            {
                if(score.Item1 == Environment.UserName && score.Item2 == playerScore)
                {
                    nameString += "> " + score.Item1 + "\n";
                    scoreString += score.Item2 + " <\n";
                    continue;
                }

                nameString += score.Item1 + "\n";
                scoreString += score.Item2 + "\n";
            }

            if (nameText == null)
            {
                var name_renderer = Component.CreateWithGameObject<TextRenderer>("HighscoreNames");
                highscoresTitle?.GetGameObject().AddChild(name_renderer.Item1);
                nameText = name_renderer.Item2;
                nameText.color = new Color(255, 255, 255, 205);
                nameText.SetFontSize(50);
                nameText.anchorPoint = AnchorPoint.TopLeft;

                name_renderer.Item1.SetLocalPosition(new Vec2D(-200, 150));

            }
            nameText.SetText(nameString);


            if (scoreText == null)
            {
                var score_renderer = Component.CreateWithGameObject<TextRenderer>("HighscoreScores");
                highscoresTitle?.GetGameObject().AddChild(score_renderer.Item1);
                scoreText = score_renderer.Item2;
                scoreText.color = new Color(255, 255, 255, 205);
                scoreText.SetFontSize(50);
                scoreText.anchorPoint = AnchorPoint.TopRight;

                score_renderer.Item1.SetLocalPosition(new Vec2D(200, 150));
            }
            scoreText.SetText(scoreString);

        }

    }
}
