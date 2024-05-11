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

        private HighscoreState state = HighscoreState.None;

        public override void Start()
        {
            var highscoresTitle = Component.CreateWithGameObject<TextRenderer>("HighscoresTitle");
            gameObject.AddChild(highscoresTitle.Item1);
            highscoresTitle.Item2.color = new Color(255, 255, 255, 205);
            highscoresTitle.Item2.SetFontSize(100);
            highscoresTitle.Item2.SetText("- Highscores -");
            highscoresTitle.Item2.anchorPoint = AnchorPoint.TopCenter;


            this.highscoresTitle = highscoresTitle.Item2;

            gameObject.SetPosition(new Vec2D(1920 / 2, 100));

            SetHighscores(GetHighscores());
        }

        private List<Tuple<string, string>> GetHighscores()
        {
            return this.highscores.AsString();
            /*
            return new List<Tuple<string, string>>()
            {
                new Tuple<string, string>("Player 1", "100"),
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
            this.highscores.AddHighscore(name, score);

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
                gameObject.AddChild(name_renderer.Item1);
                nameText = name_renderer.Item2;
                nameText.color = new Color(255, 255, 255, 205);
                nameText.SetFontSize(50);
                nameText.anchorPoint = AnchorPoint.TopLeft;

                name_renderer.Item1.SetLocalPosition(new Vec2D(-200, 200));

            }
            nameText.SetText(nameString);


            if (scoreText == null)
            {
                var score_renderer = Component.CreateWithGameObject<TextRenderer>("HighscoreScores");
                gameObject.AddChild(score_renderer.Item1);
                scoreText = score_renderer.Item2;
                scoreText.color = new Color(255, 255, 255, 205);
                scoreText.SetFontSize(50);
                scoreText.anchorPoint = AnchorPoint.TopRight;

                score_renderer.Item1.SetLocalPosition(new Vec2D(200, 200));
            }
            scoreText.SetText(scoreString);

        }

    }
    public class HighScoreRenderer : DrawableRect
    {

    }
}
