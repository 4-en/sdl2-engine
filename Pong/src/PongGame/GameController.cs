using SDL2;
using SDL2Engine;
using SDL2Engine.UI;
using SDL2Engine.Utils;
using static SDL2.SDL;

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
            if (gameController == null || gameController.IsStopped()) return;

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

    public class MouseController : Script
    {
        private PaddleController? paddleController = null;
        private GameController? gameController = null;

        public override void Start()
        {
            paddleController = gameObject.GetComponent<PaddleController>();
            gameController = FindComponent<GameController>();
        }

        public override void Update()
        {
            if (gameController == null || gameController.IsStopped()) return;

            if (paddleController == null) return;

            var mousePos = Input.GetMousePosition();
            var worldPos = GetCamera()?.ScreenToWorld(mousePos) ?? new Vec2D();

            gameObject.transform.position = new Vec2D(gameObject.transform.position.x, worldPos.y - gameObject.GetComponent<BoxCollider>()?.box.h / 2 ?? 0);
        }
    }

    class BallBounceScript : Script
    {
        SoundPlayer? sound = null;
        public override void Start()
        {
            sound = AddComponent<SoundPlayer>();
            sound?.Load("Assets/Audio/bounce.mp3");
            sound?.SetVolume(0.4);
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

    class BallPaddleCollisionScript : Script
    {
        protected BoxCollider? paddleCollider = null;
        protected BoxCollider? ballCollider = null;

        public override void Start()
        {
            this.paddleCollider = gameObject.GetComponent<BoxCollider>();

            var ball = Find("Ball");
            if (ball != null)
            {
                this.ballCollider = ball.GetComponent<BoxCollider>();
            }
        }

        public override void OnCollisionEnter(CollisionPair collision)
        {

            if (paddleCollider == null || ballCollider == null) return;

            if (collision.obj1.GetName() != "Ball" && collision.obj2.GetName() != "Ball")
            {
                return;
            }

            var ball_body = ballCollider.GetGameObject().GetComponent<PhysicsBody>();

            var ballHeight = ball_body?.GetComponent<FilledRect>()?.GetRect().h ?? 50;

            if (ball_body != null && ballCollider != null)
            {

                double paddleHeight = paddleCollider.box.h;
                double paddleY = gameObject.transform.position.y;
                double paddleMid = paddleY + paddleHeight / 2;
                Vec2D ballCenter = ballCollider.GetGameObject().transform.position;
                ballCenter.y += ballHeight / 2;


                //calculate relative position of the ball on the paddle (-1,1)
                var relativePosition = (ballCenter.y - paddleMid) / (paddleHeight / 2);
                double ball_vel = ball_body.Velocity.Length();

                double bounceBoost = 100;


                double deltaVelocity = relativePosition * ball_vel * 0.5;
                Vec2D newVelocity = ball_body.Velocity + new Vec2D(0, deltaVelocity);
                newVelocity = newVelocity.Normalize() * (ball_body.Velocity.Length() + bounceBoost);
                ball_body.Velocity = newVelocity;

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

        protected GameObject? obsticle = null;

        protected GameObject? scoreObject = null;
        protected GameObject? scoreObject2 = null;
        protected GameObject? scoreObject3 = null;

        protected TextRenderer? scoreText = null;
        protected TextRenderer? scoreText2 = null;
        protected TextRenderer? scoreText3 = null;

        protected TextRenderer? timeText = null;
        protected TextRenderer? countdownText = null;
        private GameObject? escapeMenu = null;

        protected Vec2D gameBounds = new Vec2D(1920, 1080);

        protected double roundTimer = -3;
        protected double powerupTimer = -3;
        private bool roundStarted = false;
        public double timeLimit = 60;
        public double gameTimer = 0;

        private Sound scoreSoundFire = AssetManager.LoadAsset<Sound>("Assets/Audio/Fire.mp3");
        private Sound scoreSoundWater = AssetManager.LoadAsset<Sound>("Assets/Audio/Wave.mp3");

        private double lastColorChangeTime = 0;
        private bool restedColor = false;

        private GameMode gameMode = LevelManager.gameMode;

        private bool stopped = false;
        protected double lastReset = 0;

        public int scoreToWin = 11;

        private Component AddController(PlayerType ptype, GameObject obj)
        {
            switch (ptype)
            {
                case PlayerType.WS:
                    var ws_controller = obj.AddComponent<KeyboardController>();
                    ws_controller.keyUp = (int)SDL_Keycode.SDLK_w;
                    ws_controller.keyDown = (int)SDL_Keycode.SDLK_s;
                    return ws_controller;
                case PlayerType.ArrowKeys:
                    var arrow_controller = obj.AddComponent<KeyboardController>();
                    arrow_controller.keyUp = (int)SDL_Keycode.SDLK_UP;
                    arrow_controller.keyDown = (int)SDL_Keycode.SDLK_DOWN;
                    return arrow_controller;
                case PlayerType.AI:
                    return obj.AddComponent<BetterAIController>();
                case PlayerType.Mouse:
                    return obj.AddComponent<MouseController>();
                case PlayerType.UNFAIR:
                    var ai = obj.AddComponent<BetterAIController>();
                    ai.unfair = true;
                    return ai;
                default:
                    return obj.AddComponent<KeyboardController>();

            }
        }

        public bool IsStopped()
        {
            return stopped;
        }
        public override void Start()
        {
            // create basic game object here
            // eg Ball, Paddles, Borders
            // (anything that needs to have a reference to it)

            gameBounds = GetCamera()?.GetWorldSize() ?? new Vec2D(1920, 1080);

            Console.WriteLine($"Mode: {gameMode}");

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
            player1.AddComponent<BallPaddleCollisionScript>();
            player1.AddComponent<PaddleController>().gameController = this;
            AddController(LevelManager.player1Type, player1);

            player2 = new GameObject("Player2");
            var player2_drawable = player2.AddComponent<FilledRect>();
            player2_drawable.color = new Color(255, 055, 055, 255);
            player2_drawable.SetRect(new Rect(40, 200));
            player2_drawable.anchorPoint = AnchorPoint.TopLeft;
            BoxCollider.FromDrawableRect(player2);
            player2.AddComponent<PaddleController>().gameController = this;
            player2.AddComponent<BallPaddleCollisionScript>();
            AddController(LevelManager.player2Type, player2);

            /*
            var keyboard_controller = player2.AddComponent<KeyboardController>();
            keyboard_controller.keyUp = (int)SDL_Keycode.SDLK_UP;
            keyboard_controller.keyDown = (int)SDL_Keycode.SDLK_DOWN;
            */

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
            scoreText.SetFontPath("Assets/Fonts/Arcadeclassic.ttf");
            scoreSoundWater.SetVolume(0.6);

            scoreObject3 = new GameObject("ScorePlayer-");
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
            scoreText2.SetFontPath("Assets/Fonts/Arcadeclassic.ttf");
            scoreSoundFire.SetVolume(0.8);

            timeText = Component.CreateWithGameObject<TextRenderer>("TimeCounter").Item2;
            timeText.color = new Color(255, 255, 255, 205);
            timeText.SetFontSize(52);
            timeText.anchorPoint = AnchorPoint.TopLeft;
            timeText.GetGameObject().SetPosition(new Vec2D(gameBounds.x - 300, 25));
            timeText.SetText("Round: 0");

            if (gameMode == GameMode.TIMED)
            {
                countdownText = Component.CreateWithGameObject<TextRenderer>("Countdown").Item2;
                countdownText.color = new Color(255, 255, 255, 205);
                countdownText.SetFontSize(52);
                countdownText.anchorPoint = AnchorPoint.TopLeft;
                countdownText.GetGameObject().SetPosition(new Vec2D(25, 25));

                UpdateCountdown();
            }



            ResetGame();

        }

        private void UpdateCountdown()
        {
            if (countdownText == null) return;

            double timeLeft = timeLimit - gameTimer;
            bool tied = player_1_score == player_2_score;
            if (timeLeft < 0) timeLeft = 0;

            if (tied && timeLeft <= 0)
            {
                // sudden death
                countdownText.SetText("SUDDEN DEATH");
            }

            countdownText.SetText($"{Math.Round(timeLeft, 0)}");
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
            roundTimer = -1;
            powerupTimer = -1;
            roundStarted = false;
        }

        public void StartBall()
        {
            if (ball == null) return;

            var body = ball.GetComponent<PhysicsBody>();
            if (body != null)
            {
                int total_score = player_1_score + player_2_score;
                Console.WriteLine(body.Velocity);
                double speed = 800 + total_score * 50; // increase speed as game progresses
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
            lastReset = Time.time;
            player_1_score = 0;
            player_2_score = 0;
            UpdateScoreText();
            ResetBall();
            roundTimer = -3;
            gameTimer = 0;
            powerupTimer = -3;

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
                ResetBall();
                this.stopped = true;
                Console.WriteLine("Game Over");
                string winner_name = player_1_score > player_2_score ? "Player 1" : "Player 2";
                Console.WriteLine($"{winner_name} wins!");

                ShowEndScreen();

            }
        }

        private void HandleHighscore()
        {
            // track player 1 score
            // end game if player 2 scores a point
            if (player_2_score > 0)
            {
                ResetBall();
                this.stopped = true;
                Console.WriteLine("Game Over");
                Console.WriteLine($"Player 1 scored {player_1_score} points");

                ShowEndScreen();
            }
        }

        private void ShowEndScreen()
        {
            if (QualifiedForHighscores())
            {
                var highscoreRoot = new GameObject("HighscoreRoot");
                var highscoreScript = highscoreRoot.AddComponent<HighscoreScript>();
                var hs = new OnlineHighscores<int>(100, GetLevelName());
                highscoreScript.SetHighscores(hs);

                highscoreScript.AddHighscoreState(GetScore());

            }
            else
            {
                var resultRoot = new GameObject("ResultRoot");
                var resultScript = resultRoot.AddComponent<GameResultScript>();
                resultScript.score[0] = player_1_score;
                resultScript.score[1] = player_2_score;
            }
        }

        private void HandleTimed()
        {
            // track player 1 and player 2 scores
            // end game if timeLimit is reached

            UpdateCountdown();

            if (gameTimer > timeLimit)
            {
                ResetBall();
                this.stopped = true;
                Console.WriteLine("Game Over");
                string winner_name = player_1_score > player_2_score ? "Player 1" : "Player 2";
                Console.WriteLine($"{winner_name} wins!");

                ShowEndScreen();
            }
        }

        private bool QualifiedForHighscores()
        {
            return LevelManager.player1Type != PlayerType.AI && LevelManager.player2Type == PlayerType.AI && (gameMode == GameMode.HIGHSCORE || gameMode == GameMode.TIMED);
        }

        private string GetLevelName()
        {
            return $"pong_level_{level_id}_{gameMode}";
        }

        private int GetScore()
        {
            switch (gameMode)
            {
                case GameMode.DUEL:
                    return player_1_score;
                case GameMode.HIGHSCORE:
                    return player_1_score;
                case GameMode.TIMED:
                    return player_1_score - player_2_score;
                default:
                    return 0;
            }
        }

        public override void Update()
        {

            if (timeText != null)
            {
                // 1 decimal place
                timeText.SetText($"Time: {Math.Round(roundTimer, 1)}");
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


            // basic game logic here
            // check for out of bounds
            // keep track of score
            // reset ball if needed

            if (stopped) return;

            if (ball == null) return;

            gameTimer += Time.deltaTime;
            roundTimer += Time.deltaTime;
            powerupTimer += Time.deltaTime;

            // check bounds
            var ball_collider = ball.GetComponent<BoxCollider>();
            double ball_radius = ball_collider?.box.w ?? 50 / 2;
            double tolerance = 2 * ball_radius;

            if (ball.transform.position.x < -tolerance)
            {
                scoreSoundFire.Play();

                player_2_score++;
                if (scoreText2 != null)
                {
                    scoreText2.SetFontSize(120);
                    scoreText2.color = new Color(255, 0, 0, 205);
                }
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
                if (scoreText != null)
                {
                    scoreText.SetFontSize(120);
                    scoreText.color = new Color(30, 144, 255, 255);
                }
                restedColor = false;
                UpdateScoreText();

                lastColorChangeTime = Time.time;

                Console.WriteLine($"Player 1: {player_1_score} Player 2: {player_2_score}");
                ResetBall();
            }

            if (ball.transform.position.y < -tolerance * 10 || ball.transform.position.y > gameBounds.y + tolerance * 10)
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

            // Überprüfen Sie, ob die Zeit für die Farbänderung abgelaufen ist
            if (lastColorChangeTime != 0 & Time.time - lastColorChangeTime > 2) // 3 Sekunden
            {
                if (scoreText2 == null || scoreText == null) return;

                if (restedColor == false)
                {
                    // Setzen Sie die Textfarbe auf die Standardfarbe
                    //Console.WriteLine("Standardfarbe");
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
                case GameMode.TIMED:
                    HandleTimed();
                    break;

            }
        }
    }
}