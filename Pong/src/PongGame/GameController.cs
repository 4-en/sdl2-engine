using SDL2Engine;
using SDL2;
using static SDL2.SDL;

namespace Pong
{
    public class PaddleController : Script
    {
        public double speed = 1000;
        private BoxCollider? boxCollider = null;

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
    }

    public class KeyboardController : Script
    {
        private PaddleController? paddleController = null;
        public uint keyUp = (uint)SDL_Keycode.SDLK_w;
        public uint keyDown = (uint)SDL_Keycode.SDLK_s;

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

    public class GameController : Script
    {
        protected int player_1_score = 0;
        protected int player_2_score = 0;

        protected GameObject? ball = null;
        protected GameObject? player1 = null;
        protected GameObject? player2 = null;

        protected GameObject? topWall = null;
        protected GameObject? bottomWall = null;

        protected Vec2D gameBounds = new Vec2D(1920, 1080);

        private double roundTimer = -3;
        private bool roundStarted = false;

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

            // create the paddles
            player1 = new GameObject("Player1");
            var player1_drawable = player1.AddComponent<FilledRect>();
            player1_drawable.color = new Color(055, 055, 255, 255);
            player1_drawable.SetRect(new Rect(40, 200));
            player1_drawable.anchorPoint = AnchorPoint.TopLeft;
            BoxCollider.FromDrawableRect(player1);
            player1.AddComponent<PaddleController>();
            player1.AddComponent<KeyboardController>();

            player2 = new GameObject("Player2");
            var player2_drawable = player2.AddComponent<FilledRect>();
            player2_drawable.color = new Color(255, 055, 055, 255);
            player2_drawable.SetRect(new Rect(40, 200));
            player2_drawable.anchorPoint = AnchorPoint.TopLeft;
            BoxCollider.FromDrawableRect(player2);
            player2.AddComponent<PaddleController>();
            var keyboard_controller = player2.AddComponent<KeyboardController>();
            keyboard_controller.keyUp = (uint)SDL_Keycode.SDLK_UP;
            keyboard_controller.keyDown = (uint)SDL_Keycode.SDLK_DOWN;

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

            ResetGame();

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
                double direction = rand.NextDouble() * Math.PI / 2 + Math.PI / 4;
                body.Velocity = new Vec2D(Math.Cos(direction), Math.Sin(direction)) * speed;
            }
        }

        public void ResetGame()
        {
            player_1_score = 0;
            player_2_score = 0;
            ResetBall();
            roundTimer = -4;

            // set paddles to starting position
            if (player1 != null)
            {
                player1.transform.position = new Vec2D(50, gameBounds.y / 2);
            }

            if (player2 != null)
            {
                player2.transform.position = new Vec2D(gameBounds.x - 50, gameBounds.y / 2);
            }
        }

        public override void Update()
        {

            // reset game if r is pressed
            if (Input.GetKeyPressed((uint)SDL_Keycode.SDLK_r))
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
            double tolerance = 2*ball_radius;

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

            if(roundTimer > 0 && !roundStarted)
            {
                StartBall();
                roundStarted = true;
            }

            if (player_1_score >= scoreToWin || player_2_score >= scoreToWin)
            {
                Console.WriteLine("Game Over");
                string winner_name = player_1_score > player_2_score ? "Player 1" : "Player 2";
                Console.WriteLine($"{winner_name} wins!");
                ResetGame();
            }

        }
    }
}