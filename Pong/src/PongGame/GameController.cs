using SDL2;
using SDL2Engine;
using static SDL2.SDL;
using SDL2Engine.UI;
using SDL2Engine.Utils;

namespace Pong
{

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
            }
            else
            {
                filledRect.color = new Color(255, 255, 255, 255);
            }
        }
    }

    public enum GameMode
    {
        DUEL = 0,
        HIGHSCORE = 1,
        TIMED = 2
    }

    public class GameController : Script
    {
        public int level_id = 0;
        protected int player_1_score = 0;
        protected int player_2_score = 0;

        protected GameObject? ball = null;
        protected GameObject? player1 = null;
        protected GameObject? player2 = null;

        protected GameObject? topWall = null;
        protected GameObject? bottomWall = null;

        protected GameObject? scoreObject = null;
        protected GameObject? scoreObject2 = null;
        protected GameObject? scoreObject3 = null;

        protected TextRenderer? scoreText = null;
        protected TextRenderer? scoreText2 = null;
        protected TextRenderer? scoreText3 = null;

        protected TextRenderer? timeText = null;
        private GameObject? escapeMenu = null;

        protected Vec2D gameBounds = new Vec2D(1920, 1080);

        protected double roundTimer = -3;
        private bool roundStarted = false;
        public double timeLimit = 60;

        private Sound scoreSoundFire = AssetManager.LoadAsset<Sound>("Assets/Audio/Fire.mp3");
        private Sound scoreSoundWater = AssetManager.LoadAsset<Sound>("Assets/Audio/Wave.mp3");

        private double lastColorChangeTime = 0;
        private bool restedColor = false;

        private GameMode gameMode = GameMode.DUEL;

        private bool stopped = false;

        public int scoreToWin = 1;
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
            player1_drawable.color = new Color(30, 144, 255, 255);
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

            scoreObject = new GameObject("ScorePlayer1");
            scoreObject.transform.position = new Vec2D(gameBounds.x / 2 - 90, 80);
            scoreText = scoreObject.AddComponent<TextRenderer>();
            scoreText.color = new Color(255, 255, 255, 205);
            scoreText.SetFontSize(100);

            scoreObject3 = new GameObject("ScorePlayer1");
            scoreObject3.transform.position = new Vec2D(gameBounds.x / 2, 80);
            scoreText3 = scoreObject3.AddComponent<TextRenderer>();
            scoreText3.color = new Color(255, 255, 255, 205);
            scoreText3.SetText("-");
            scoreText3.SetFontSize(100);

            scoreObject2 = new GameObject("ScorePlayer2");
            scoreObject2.transform.position = new Vec2D(gameBounds.x / 2 + 90, 80);
            scoreText2 = scoreObject2.AddComponent<TextRenderer>();
            scoreText2.color = new Color(255, 255, 255, 205);
            scoreText2.SetFontSize(100);

            timeText = Component.CreateWithGameObject<TextRenderer>("TimeCounter").Item2;
            timeText.color = new Color(255, 255, 255, 205);
            timeText.SetFontSize(32);
            timeText.anchorPoint = AnchorPoint.TopLeft;
            timeText.GetGameObject().SetPosition(new Vec2D(gameBounds.x - 200, 25));
            timeText.SetText("Time: 0");



            ResetGame();

        }

        

        public void UpdateScoreText()
        {
            if (scoreText == null || scoreText2 == null) return;

            string scoreString = $"{player_1_score}";
            string scoreString2 = $"{player_2_score}";


            scoreText.SetText(scoreString);
            scoreText2.SetText(scoreString2);
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
                body.Velocity += body.Velocity.Normalize() * increase;
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

            stopped = false;
        }

        private void HandleDuel()
        {
            // track player 1 and player 2 scores
            // end game if score reaches scoreToWin

            if (player_1_score >= scoreToWin || player_2_score >= scoreToWin)
            {
                this.stopped = true;
                Console.WriteLine("Game Over");
                string winner_name = player_1_score > player_2_score ? "Player 1" : "Player 2";
                Console.WriteLine($"{winner_name} wins!");
                
                var resultRoot = new GameObject("ResultRoot");
                var resultScript = resultRoot.AddComponent<GameResultScript>();
                resultScript.score[0] = player_1_score;
                resultScript.score[1] = player_2_score;
                
            }
        }

        private void HandleHighscore()
        {
            // track player 1 score
            // end game if player 2 scores a point
            if (player_2_score > 0)
            {
                this.stopped = true;
                Console.WriteLine("Game Over");
                Console.WriteLine($"Player 1 scored {player_1_score} points");

                var highscoreRoot = new GameObject("HighscoreRoot");
                var highscoreScript = highscoreRoot.AddComponent<HighscoreScript>();
                highscoreScript.AddHighscoreState(player_1_score);
                var hs = new Highscores<int>(100, $"pong_level_{this.level_id}.txt");
                highscoreScript.SetHighscores(hs);
            }
        }

        public override void Update()
        {

            if (timeText != null)
            {
                // 2 decimal places
                timeText.SetText($"Time: {Math.Round(roundTimer, 2)}");
            }

            if (Input.GetKeyDown((int)SDL_Keycode.SDLK_ESCAPE))
            {
                if (this.escapeMenu != null)
                {
                    this.escapeMenu.Destroy();
                    this.escapeMenu = null;
                }
                else
                {
                    var stopState = !!stopped;
                    var escapemenu = UI.EscapeMenu("Paused", () =>
                    {

                        this.stopped = stopState;
                        GetScene()?.SetPhysics(true);
                        this.escapeMenu = null;
                        return true;
                    });

                    this.escapeMenu = escapemenu;
                    this.stopped = true;
                    GetScene()?.SetPhysics(false);
                }
            }

            // reset game if r is pressed
            if (Input.GetKeyPressed((int)SDL_Keycode.SDLK_r))
            {
                ResetGame();
                return;
            }

            if (Input.GetKeyPressed(SDL_Keycode.SDLK_x))
            {
                Console.WriteLine(Time.tick);
                var scene = GetScene();
                if (scene != null)
                {
                    Console.WriteLine("Destroying all game objects");
                    scene.GetGameObjects().ForEach(o => Destroy(o));
                } else { Console.WriteLine("Scene is null"); }
            }

            // basic game logic here
            // check for out of bounds
            // keep track of score
            // reset ball if needed

            if (stopped) return;

            if (ball == null) return;

            roundTimer += Time.deltaTime;

            // check bounds
            var ball_collider = ball.GetComponent<BoxCollider>();
            double ball_radius = ball_collider?.box.w ?? 50 / 2;
            double tolerance = 2 * ball_radius;

            if (ball.transform.position.x < -tolerance)
            {
                scoreSoundFire.Play();

                player_2_score++;
                scoreText2.SetFontSize(120);
                scoreText2.color = new Color(255, 0, 0, 205);
                restedColor = false;
                UpdateScoreText();

                lastColorChangeTime = Time.time;
                // Console.WriteLine("lastColorChangeTime" + lastColorChangeTime);

                Console.WriteLine($"Player 1: {player_1_score} Player 2: {player_2_score}");
                ResetBall();

            }
            else if (ball.transform.position.x > gameBounds.x + tolerance)
            {
                scoreSoundWater.Play();
                player_1_score++;
                scoreText.SetFontSize(120);
                scoreText.color = new Color(30, 144, 255, 255);
                restedColor = false;
                UpdateScoreText();

                lastColorChangeTime = Time.time;

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

            // �berpr�fen Sie, ob die Zeit f�r die Farb�nderung abgelaufen ist
            if (lastColorChangeTime != 0 & Time.time - lastColorChangeTime > 2) // 3 Sekunden
            {
                if (scoreText2 == null || scoreText == null) return;

                if (restedColor == false)
                {
                    // Setzen Sie die Textfarbe auf die Standardfarbe
                    Console.WriteLine("Standardfarbe");
                    scoreText2.SetFontSize(100);
                    scoreText2.color = new Color(255, 255, 255, 205);

                    scoreText.SetFontSize(100);
                    scoreText.color = new Color(255, 255, 255, 205);
                    // Standardfarbe
                    restedColor = true;
                    //UpdateScoreText();
                }
            }
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