using SDL2Engine;
using SDL2Engine.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using static SDL2.SDL;

namespace Pong
{
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
        private GameController? gameController = null;

        private HighscoreState state = HighscoreState.None;

        public void SetHighscores(Highscores<int> highscores)
        {
            this.highscores = highscores;
        }

        public override void Start()
        {
            gameController = FindComponent<GameController>();

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
                gameController?.ResetGame();
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
            menuButton.Item1.SetLocalPosition(new Vec2D(250, 450));
            menuButton.Item2.SetBorderSize(2);
            menuButton.Item2.SetBorderColor(new Color(255, 255, 255, 255));
            var menuHelper = menuButton.Item1.AddComponent<TextRenderHelper>();
            menuHelper.OnClick += (object? _, TextRenderer _) =>
            {
                Destroy(gameObject);
                if (gameController != null)
                {
                    Destroy(gameController.GetGameObject());
                }
                LevelManager.LoadPlayerSelection();
            };

            menuHelper.OnHover += (object? _, TextRenderer _) =>
            {
                menuButton.Item2.SetBackgroundColor(new Color(123, 0, 0, 150));
            };

            menuHelper.OnLeave += (object? _, TextRenderer _) =>
            {
                menuButton.Item2.SetBackgroundColor(new Color(0, 0, 0, 100));
            };

            var levelButton = Component.CreateWithGameObject<TextRenderer>("LevelButton");
            gameObject.AddChild(levelButton.Item1);
            levelButton.Item2.color = new Color(255, 255, 255, 205);
            levelButton.Item2.SetFontSize(50);
            levelButton.Item2.SetText("Level");
            levelButton.Item2.anchorPoint = AnchorPoint.Center;
            levelButton.Item2.SetPreferredSize(new Rect(200, 100));
            levelButton.Item2.SetBackgroundColor(new Color(0, 0, 0, 100));
            levelButton.Item1.SetLocalPosition(new Vec2D(250, 300));
            levelButton.Item2.SetBorderSize(2);
            levelButton.Item2.SetBorderColor(new Color(255, 255, 255, 255));
            var levelHelper = levelButton.Item1.AddComponent<TextRenderHelper>();
            levelHelper.OnClick += (object? _, TextRenderer _) =>
            {
                Destroy(gameObject);
                LevelManager.LoadHomeScreen();
            };

            levelHelper.OnHover += (object? _, TextRenderer _) =>
            {
                levelButton.Item2.SetBackgroundColor(new Color(123, 0, 0, 150));
            };

            levelHelper.OnLeave += (object? _, TextRenderer _) =>
            {
                levelButton.Item2.SetBackgroundColor(new Color(0, 0, 0, 100));
            };


            //gameObject.SetPosition(new Vec2D(1920 / 2, 100));

            //SetHighscores(GetHighscores());
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

            foreach (var score in scores)
            {
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
    public class GameResultScript : Script
    {
        private GameController? gameController = null;
        private TextRenderer? textRenderer = null;
        private HighscoreScript? highscoreScript = null;
        public int[] score = new int[2];

        public override void Start()
        {
            gameController = FindComponent<GameController>();

            var backgroundOverlay = Component.CreateWithGameObject<FilledRect>("GameResultBackground");
            gameObject.AddChild(backgroundOverlay.Item1);
            backgroundOverlay.Item2.color = new Color(10, 10, 20, 200);
            backgroundOverlay.Item2.SetRect(new Rect(1920, 1080));
            backgroundOverlay.Item2.anchorPoint = AnchorPoint.TopLeft;

            var resultText = Component.CreateWithGameObject<TextRenderer>("ResultText");
            gameObject.AddChild(resultText.Item1);
            resultText.Item2.color = new Color(255, 255, 255, 205);
            resultText.Item2.SetFontSize(100);
            resultText.Item2.SetText("Game Over");
            resultText.Item2.anchorPoint = AnchorPoint.TopCenter;
            resultText.Item2.SetPreferredSize(new Rect(700, 900));
            resultText.Item1.SetLocalPosition(new Vec2D(1920 / 2, 50));

            var winnerText = Component.CreateWithGameObject<TextRenderer>("WinnerText");
            gameObject.AddChild(winnerText.Item1);
            winnerText.Item2.color = new Color(255, 255, 255, 205);
            winnerText.Item2.SetFontSize(50);
            string winnerName = score[0] > score[1] ? "Player 1" : "Player 2";
            winnerText.Item2.SetText(winnerName + " wins!");
            winnerText.Item2.anchorPoint = AnchorPoint.TopCenter;
            winnerText.Item2.SetPreferredSize(new Rect(700, 900));
            winnerText.Item1.SetLocalPosition(new Vec2D(1920 / 2, 200));
            winnerText.Item2.SetFontSize(50);

            var scoreText = Component.CreateWithGameObject<TextRenderer>("ScoreText");
            gameObject.AddChild(scoreText.Item1);
            scoreText.Item2.color = new Color(255, 255, 255, 205);
            scoreText.Item2.SetFontSize(50);
            scoreText.Item2.SetText("Score: " + score[0] + " - " + score[1]);
            scoreText.Item2.anchorPoint = AnchorPoint.TopCenter;
            scoreText.Item2.SetPreferredSize(new Rect(700, 900));
            scoreText.Item1.SetLocalPosition(new Vec2D(1920 / 2, 300));
            scoreText.Item2.SetFontSize(50);

            var restartButton = Component.CreateWithGameObject<TextRenderer>("RestartButton");
            gameObject.AddChild(restartButton.Item1);
            restartButton.Item2.color = new Color(255, 255, 255, 205);
            restartButton.Item2.SetFontSize(50);
            restartButton.Item2.SetText("Restart");
            restartButton.Item2.anchorPoint = AnchorPoint.Center;
            restartButton.Item2.SetPreferredSize(new Rect(200, 100));
            restartButton.Item2.SetBackgroundColor(new Color(0, 0, 0, 100));
            restartButton.Item1.SetLocalPosition(new Vec2D(250, 150));
            restartButton.Item2.SetBorderSize(2);
            restartButton.Item2.SetBorderColor(new Color(255, 255, 255, 255));
            var helper = restartButton.Item1.AddComponent<TextRenderHelper>();
            helper.OnClick += (object? _, TextRenderer _) =>
            {
                Destroy(gameObject);
                gameController?.ResetGame();
            };
            helper.OnHover += (object? _, TextRenderer _) =>
            {
                restartButton.Item2.SetBackgroundColor(new Color(123, 0, 0, 150));
            };

            helper.OnLeave += (object? _, TextRenderer _) =>
            {
                restartButton.Item2.SetBackgroundColor(new Color(0, 0, 0, 100));
            };

            var menuButton = Component.CreateWithGameObject<TextRenderer>("MenuButton");
            gameObject.AddChild(menuButton.Item1);
            menuButton.Item2.color = new Color(255, 255, 255, 205);
            menuButton.Item2.SetFontSize(50);
            menuButton.Item2.SetText("Menu");
            menuButton.Item2.anchorPoint = AnchorPoint.Center;
            menuButton.Item2.SetPreferredSize(new Rect(200, 100));
            menuButton.Item2.SetBackgroundColor(new Color(0, 0, 0, 100));
            menuButton.Item1.SetLocalPosition(new Vec2D(250, 450));
            menuButton.Item2.SetBorderSize(2);
            menuButton.Item2.SetBorderColor(new Color(255, 255, 255, 255));
            var menuHelper = menuButton.Item1.AddComponent<TextRenderHelper>();
            menuHelper.OnClick += (object? _, TextRenderer _) =>
            {
                Destroy(gameObject);
                if (gameController != null)
                {
                    Destroy(gameController.GetGameObject());
                }
                LevelManager.LoadPlayerSelection();
            };

            menuHelper.OnHover += (object? _, TextRenderer _) =>
            {
                menuButton.Item2.SetBackgroundColor(new Color(123, 0, 0, 150));
            };

            menuHelper.OnLeave += (object? _, TextRenderer _) =>
            {
                menuButton.Item2.SetBackgroundColor(new Color(0, 0, 0, 100));
            };

            var levelButton = Component.CreateWithGameObject<TextRenderer>("LevelButton");
            gameObject.AddChild(levelButton.Item1);
            levelButton.Item2.color = new Color(255, 255, 255, 205);
            levelButton.Item2.SetFontSize(50);
            levelButton.Item2.SetText("Level");
            levelButton.Item2.anchorPoint = AnchorPoint.Center;
            levelButton.Item2.SetPreferredSize(new Rect(200, 100));
            levelButton.Item2.SetBackgroundColor(new Color(0, 0, 0, 100));
            levelButton.Item1.SetLocalPosition(new Vec2D(250, 300));
            levelButton.Item2.SetBorderSize(2);
            levelButton.Item2.SetBorderColor(new Color(255, 255, 255, 255));
            var levelHelper = levelButton.Item1.AddComponent<TextRenderHelper>();
            levelHelper.OnClick += (object? _, TextRenderer _) =>
            {
                Destroy(gameObject);
                LevelManager.LoadHomeScreen();
            };

            levelHelper.OnHover += (object? _, TextRenderer _) =>
            {
                levelButton.Item2.SetBackgroundColor(new Color(123, 0, 0, 150));
            };

            levelHelper.OnLeave += (object? _, TextRenderer _) =>
            {
                levelButton.Item2.SetBackgroundColor(new Color(0, 0, 0, 100));
            };

        }
    }
}
