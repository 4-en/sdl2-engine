using SDL2Engine;
using SDL2;
using static SDL2.SDL;
using SDL2Engine.UI;
using SDL2Engine.Utils;

namespace Pong
{

    public class HighscoreScript : Script
    {
        private int highscore = 0;
        private TextRenderer? textRenderer = null;

        private TextRenderer? scoreText = null;
        private TextRenderer? nameText = null;
        private TextRenderer? highscoresTitle = null;

        public override void Start()
        {
            var highscoresTitle = Component.CreateWithGameObject<TextRenderer>("HighscoresTitle");
            gameObject.AddChild(highscoresTitle.Item1);
            highscoresTitle.Item2.color = new Color(255, 255, 255, 205);
            highscoresTitle.Item2.SetFontSize(100);
            highscoresTitle.Item2.SetText("Highscores");

            highscoresTitle.Item1.transform.position = new Vec2D(1920 / 2, 100);

            this.highscoresTitle = highscoresTitle.Item2;
            this.gameObject.AddChild(highscoresTitle.Item1);

            SetHighscores(GetHighscores());
        }

        private List<Tuple<string, string>> GetHighscores()
        {
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
        }

        public override void Update()
        {
            if(Input.GetKeyDown(SDL_Keycode.SDLK_h))
            {
                gameObject.ToggleEnabled();

                // since gameObject disables all children and components
                // we need to re-enable this script, otherwise we can't enable the gameObject again
                this.Enable();

                Console.WriteLine("Highscore script enabled: " + gameObject.IsEnabled());
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

                name_renderer.Item1.transform.position = new Vec2D(-300, 200);

            }
            else
            {
                nameText.SetText(nameString);
            }

            if (scoreText == null)
            {
                var score_renderer = Component.CreateWithGameObject<TextRenderer>("HighscoreScores");
                gameObject.AddChild(score_renderer.Item1);
                scoreText = score_renderer.Item2;
                scoreText.color = new Color(255, 255, 255, 205);
                scoreText.SetFontSize(50);

                score_renderer.Item1.transform.position = new Vec2D(300, 200);
            }
            else
            {
                scoreText.SetText(scoreString);
            }
        }

    }
    public class PaddleController : Script
    {
        public double speed = 1000;
        private BoxCollider? boxCollider = null;
        public GameController? gameController = null;

        public override void Start()
        {
            boxCollider = gameObject.GetComponent<BoxCollider>();
        }

        public void Move(double strength)
        {
            double movement = speed * Time.deltaTime * strength;
            gameObject.transform.position += new Vec2D(0, movement);

            if (boxCollider == null) return;

            // keep paddle within bounds
            double minY = 0;
            double maxY = GetCamera()?.GetWorldSize().y ?? 1080;
            double paddleHeight = boxCollider.box.h;

            if (gameObject.transform.position.y < minY)
            {
                gameObject.transform.position = new Vec2D(gameObject.transform.position.x, minY);
            }
            else if (gameObject.transform.position.y + paddleHeight > maxY)
            {
                gameObject.transform.position = new Vec2D(gameObject.transform.position.x, maxY - paddleHeight);
            }
        }

        public void MoveUp()
        {
            Move(-1);
        }

        public void MoveDown()
        {
            Move(1);
        }

        public override void OnCollisionEnter(CollisionPair collision)
        {
            if (gameController == null) return;

            gameController.OnPaddleHit();
        }
    }

    public class KeyboardController : Script
    {
        private PaddleController? paddleController = null;
        public int keyUp = (int)SDL_Keycode.SDLK_w;
        public int keyDown = (int)SDL_Keycode.SDLK_s;

        public override void Start()
        {
            paddleController = gameObject.GetComponent<PaddleController>();
        }

        public override void Update()
        {
            if (paddleController == null) return;

            if (Input.GetKeyPressed(keyUp))
            {
                paddleController.MoveUp();
            }
            if (Input.GetKeyPressed(keyDown))
            {
                paddleController.MoveDown();
            }
        }
    }

    class BallBounceScript : Script
    {
        SoundPlayer? sound = null;
        public override void Start()
        {
            sound = AddComponent<SoundPlayer>();
            sound?.Load("Assets/Audio/bounce.mp3");
        }

        public override void OnCollisionEnter(CollisionPair collision)
        {
            sound?.Play();
        }

        public override void Update()
        {
            var collider = gameObject.GetComponent<BoxCollider>();
            var filledRect = gameObject.GetComponent<FilledRect>();
            if (collider == null || filledRect == null) return;

            var rect = collider.GetCollisionBox();

            if (SDL2Engine.Utils.MouseHelper.IsRectPressed(GetCamera()?.RectToScreen(rect) ?? new Rect()))
            {
                filledRect.color = new Color(255, 0, 0, 255);
            } else
            {
                filledRect.color = new Color(255, 255, 255, 255);
            }
        }
    }

    public enum GameMode
    {
        DUEL = 0,
        HIGHSCORE = 1
    }

    public class GameController : Script
    {
        protected int player_1_score = 0;
        protected int player_2_score = 0;

        protected GameObject? ball = null;
        protected GameObject? player1 = null;
        protected GameObject? player2 = null;

        protected GameObject? topWall = null;
        protected GameObject? bottomWall = null;

        protected GameObject? scoreObject = null;
        protected TextRenderer? scoreText = null;

        protected Vec2D gameBounds = new Vec2D(1920, 1080);

        private double roundTimer = -3;
        private bool roundStarted = false;

        private GameMode gameMode = GameMode.DUEL;

        public int scoreToWin = 11;
        public override void Start()
        {
            // create basic game object here
            // eg Ball, Paddles, Borders
            // (anything that needs to have a reference to it)

            gameBounds = GetCamera()?.GetWorldSize() ?? new Vec2D(1920, 1080);

            // create the ball
            ball = new GameObject("Ball");
            var ball_drawable = ball.AddComponent<FilledRect>();
            ball_drawable.color = new Color(255, 255, 255, 255);
            ball_drawable.SetRect(new Rect(50, 50));
            ball_drawable.anchorPoint = AnchorPoint.TopLeft;
            BoxCollider.FromDrawableRect(ball);
            ball.AddComponent<PhysicsBody>();
            ball.AddComponent<BallBounceScript>();

            // create the paddles
            player1 = new GameObject("Player1");
            var player1_drawable = player1.AddComponent<FilledRect>();
            player1_drawable.color = new Color(055, 055, 255, 255);
            player1_drawable.SetRect(new Rect(40, 200));
            player1_drawable.anchorPoint = AnchorPoint.TopLeft;
            BoxCollider.FromDrawableRect(player1);
            player1.AddComponent<PaddleController>().gameController = this;
            player1.AddComponent<KeyboardController>();

            player2 = new GameObject("Player2");
            var player2_drawable = player2.AddComponent<FilledRect>();
            player2_drawable.color = new Color(255, 055, 055, 255);
            player2_drawable.SetRect(new Rect(40, 200));
            player2_drawable.anchorPoint = AnchorPoint.TopLeft;
            BoxCollider.FromDrawableRect(player2);
            player2.AddComponent<PaddleController>().gameController = this;
            var keyboard_controller = player2.AddComponent<KeyboardController>();
            keyboard_controller.keyUp = (int)SDL_Keycode.SDLK_UP;
            keyboard_controller.keyDown = (int)SDL_Keycode.SDLK_DOWN;

            // create the walls
            var create_barrier = (string name, Rect area) =>
            {
                var barrier = new GameObject(name);
                var collider = barrier.AddComponent<BoxCollider>();
                collider.box = area;
                return barrier;
            };

            topWall = create_barrier("Top Barrier", new Rect(0, -100, 1920, 100));
            bottomWall = create_barrier("Bottom Barrier", new Rect(0, 1080, 1920, 100));

            scoreObject = new GameObject("Score");
            scoreObject.transform.position = new Vec2D(gameBounds.x / 2, 50);
            scoreText = scoreObject.AddComponent<TextRenderer>();
            scoreText.color = new Color(255, 255, 255, 205);
            scoreText.SetFontSize(100);
            var highscoreRoot = new GameObject("HighscoreRoot");
            var highscoreScript = highscoreRoot.AddComponent<HighscoreScript>();
            

            ResetGame();

        }

        public void UpdateScoreText()
        {
            if (scoreText == null) return;

            string scoreString = $"{player_1_score} - {player_2_score}";
            scoreText.SetText(scoreString);
        }

        public void SetGameMode(GameMode mode)
        {
            if (mode == gameMode) return;

            gameMode = mode;
            ResetGame();
        }

        public void OnPaddleHit()
        {
            // increase speed of ball
            double increase = 50;
            var body = ball?.GetComponent<PhysicsBody>();
            if (body != null)
            {
                body.Velocity += body.Velocity.Normalize()* increase;
            }
        }

        public void ResetBall()
        {
            if (ball == null) return;

            ball.transform.position = gameBounds / 2;
            var body = ball.GetComponent<PhysicsBody>();
            if (body != null)
            {
                body.Velocity = new Vec2D(0, 0);
            }
            roundTimer = -2;
            roundStarted = false;
        }

        public void StartBall()
        {
            if (ball == null) return;

            var body = ball.GetComponent<PhysicsBody>();
            if (body != null)
            {
                int total_score = player_1_score + player_2_score;
                double speed = 500 + total_score * 50; // increase speed as game progresses
                var rand = new System.Random();

                // angle should be between -45 and 45 degrees
                double direction = rand.NextDouble() * Math.PI / 3 - Math.PI / 6;

                if (rand.NextDouble() > 0.5)
                {
                    direction += Math.PI;
                }

                body.Velocity = new Vec2D(Math.Cos(direction), Math.Sin(direction)) * speed;
            }
        }

        public void ResetGame()
        {
            player_1_score = 0;
            player_2_score = 0;
            UpdateScoreText();
            ResetBall();
            roundTimer = -4;

            // set paddles to starting position
            if (player1 != null)
            {
                player1.transform.position = new Vec2D(50, gameBounds.y / 2);
            }

            if (player2 != null)
            {
                player2.transform.position = new Vec2D(gameBounds.x - 50 - 40, gameBounds.y / 2);
            }
        }

        private void HandleDuel()
        {
            // track player 1 and player 2 scores
            // end game if score reaches scoreToWin

            if (player_1_score >= scoreToWin || player_2_score >= scoreToWin)
            {
                Console.WriteLine("Game Over");
                string winner_name = player_1_score > player_2_score ? "Player 1" : "Player 2";
                Console.WriteLine($"{winner_name} wins!");
                ResetGame();
            }
        }

        private void HandleHighscore()
        {
            // track player 1 score
            // end game if player 2 scores a point
            if (player_2_score > 0)
            {
                Console.WriteLine("Game Over");
                Console.WriteLine($"Player 1 scored {player_1_score} points");
                ResetGame();
            }
        }

        public override void Update()
        {

            // reset game if r is pressed
            if (Input.GetKeyPressed((int)SDL_Keycode.SDLK_r))
            {
                ResetGame();
                return;
            }

            // basic game logic here
            // check for out of bounds
            // keep track of score
            // reset ball if needed

            if (ball == null) return;

            roundTimer += Time.deltaTime;

            // check bounds
            var ball_collider = ball.GetComponent<BoxCollider>();
            double ball_radius = ball_collider?.box.w ?? 50 / 2;
            double tolerance = 2 * ball_radius;

            if (ball.transform.position.x < -tolerance)
            {
                player_2_score++;
                Console.WriteLine($"Player 1: {player_1_score} Player 2: {player_2_score}");
                ResetBall();
            }
            else if (ball.transform.position.x > gameBounds.x + tolerance)
            {
                player_1_score++;
                Console.WriteLine($"Player 1: {player_1_score} Player 2: {player_2_score}");
                ResetBall();
            }

            if (ball.transform.position.y < -tolerance || ball.transform.position.y > gameBounds.y + tolerance)
            {
                // somehow the ball is out of bounds, reset without scoring
                Console.WriteLine("Ball out of bounds");
                ResetBall();
            }

            if (roundTimer > 0 && !roundStarted)
            {
                StartBall();
                roundStarted = true;
            }

            UpdateScoreText();

            switch (gameMode)
            {
                case GameMode.DUEL:
                    HandleDuel();
                    break;
                case GameMode.HIGHSCORE:
                    HandleHighscore();
                    break;

            }
        }
    }
}