using SDL2Engine;
using static SDL2.SDL;
using ShootEmUp.Entities;

namespace ShootEmUp
{

    public class GameController : Script
    {
        public int level_id = 0;

        private GameObject? escapeMenu = null;

        protected Vec2D gameBounds = new Vec2D(1920, 1080);

        protected double roundTimer = -3;
        public double gameTimer = 0;

        protected TextRenderer? timeText = null;
        private bool stopped = false;


        public bool IsStopped()
        {
            return stopped;
        }
        public override void Start()
        {
            // create basic game object here
            // (anything that needs to have a reference to it)

            gameBounds = GetCamera()?.GetWorldSize() ?? new Vec2D(1920, 1080);


            // create the player with Player class
            var player = new GameObject("Player");
            player.AddComponent<Player>();

            //background image
            var background = new GameObject("Background");
            background.AddComponent<TextureRenderer>()?.SetSource("Assets/Textures/grid.jpg");
            background.transform.position = new Vec2D(gameBounds.x / 2, gameBounds.y / 2);

            //Time Counter
            timeText = Component.CreateWithGameObject<TextRenderer>("TimeCounter").Item2;
            timeText.color = new Color(255, 255, 255, 205);
            timeText.SetFontSize(52);
            timeText.anchorPoint = AnchorPoint.TopLeft;
            timeText.GetGameObject().SetPosition(new Vec2D(gameBounds.x - 300, 25));

            // collision test object
            var obstacle = new GameObject("Obstacle");
            var pb = obstacle.AddComponent<PhysicsBody>();
            pb.IsMovable = false;
            var bc = obstacle.AddComponent<BoxCollider>();
            var texture = obstacle.AddComponent<TextureRenderer>();
            texture?.SetSource("Assets/Textures/forsenE.png");
            obstacle.transform.position = new Vec2D(400,500);
            pb.Velocity= new Vec2D(1, 1);
            pb.IsMovable = true;

            //asteroid
            var asteroid = new GameObject("Asteroid");
            asteroid.AddComponent<Asteroid>();
            var asteroidComponent = asteroid.GetComponent<Asteroid>();
            asteroidComponent.position = new Vec2D(1000, 200);
            asteroidComponent.velocity = new Vec2D(-50, 10);
            
            








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




            // basic game logic here
            if (stopped) return;


            gameTimer += Time.deltaTime;
            roundTimer += Time.deltaTime;





        }
    }



}



