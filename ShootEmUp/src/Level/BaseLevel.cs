using SDL2Engine;
using ShootEmUp.src;
using ShootEmUp.src.Entities;
using static SDL2.SDL;

namespace ShootEmUp.Level
{
    /*
     * A class that represents an enemy wave. An enemy wave is a group of enemies that spawn at the same time.
     * The struct contains the name of a template file that contains the enemy wave, 
     * the maximum time that the player has to defeat the wave, and the number of times that the wave will spawn.
     * 
     * If the time runs out before the player defeats the wave, the next wave will spawn.
     * If the player clears the wave before the time runs out, the next wave will spawn.
     */
    public class EnemyWave
    {
        // the name of the file that contains the enemy wave
        public string Name = "";
        // the maximum time that the player has to defeat the wave
        public double MaxTime = 60;
        // the number of times that the wave will spawn
        public int Count = 1;

        public EnemyWave(string name, double maxTime, int count)
        {
            Name = name;
            MaxTime = maxTime;
            Count = count;
        }

        public EnemyWave(string name, double maxTime)
        {
            Name = name;
            MaxTime = maxTime;
            Count = 1;
        }

        public EnemyWave(string name)
        {
            Name = name;
            MaxTime = 60;
            Count = 1;
        }

        public EnemyWave()
        {
            Name = "";
            MaxTime = 60;
            Count = 1;
        }
    }

    /*
     * The base level class for the shoot em up game. This class will manage the following:
     * - Creating the level (enemies, player, etc.)
     * - Handling the level's win/lose conditions
     * - General game loop
     */
    public class BaseLevel : Script
    {
        public int LevelID = 0;
        // An array of enemy waves that will spawn in the level
        // This should be changed by inheriting classes to define the level's enemy waves,
        // or simply by setting the array after creating the level object
        public EnemyWave[] Waves = new EnemyWave[0];
        private int currentWave = -1;
        private int maxWaveCount = 0;
        private double waveStartTime = 0;
        private double levelTimer = 0;
        private bool paused = false;
        private LinkedList<GameObject> Enemies = new LinkedList<GameObject>();
        private GameObject? Player;

        private int score = 0;
        private int money = 0;

        private GameObject? escapeMenu = null;
        private GameObject? shopMenu = null;

        private bool was_setup = false;
        public void SetupLevel(int levelID, EnemyWave[] waves)
        {
            LevelID = levelID;
            Waves = waves;
            currentWave = -1;
            was_setup = true;
        }

        private void CreatePlayer()
        {
            var gameBounds = GetCamera()?.GetWorldSize() ?? new Vec2D(1920, 1080);


            // create the player with Player class
            var player = new GameObject("Player");
            player.AddComponent<Player>();


            // create the Background
            var background = new GameObject("Player");
            background.AddComponent<Background>();

            // collision test object
            var obstacle = new GameObject("Obstacle");
            var pb = obstacle.AddComponent<PhysicsBody>();
            var bc = obstacle.AddComponent<BoxCollider>();
            var texture = obstacle.AddComponent<TextureRenderer>();
            texture?.SetSource("Assets/Textures/forsenE.png");
            obstacle.transform.position = new Vec2D(400, 500);
            pb.Velocity = new Vec2D(1, 1);
            pb.IsMovable = true;


            //asteroid
            var asteroid = new GameObject("Asteroid");
            var asteroidComponent = asteroid.AddComponent<Asteroid>();
            asteroidComponent.position = new Vec2D(1000, 200);
            asteroidComponent.velocity = new Vec2D(-50, 10);

            //asteroid
            var asteroid2 = new GameObject("Asteroid");
            var asteroidComponent2 = asteroid2.AddComponent<Asteroid>();
            asteroidComponent2.position = new Vec2D(1200, 200);
            asteroidComponent2.velocity = new Vec2D(-50, 10);
        }

        public override void Start()
        {
            if (!was_setup)
            {
                Console.WriteLine("Level was not setup properly. Exiting...");
                Console.WriteLine("Please call SetupLevel() before starting the level.");
                return;
            }

            // count the total number of waves
            maxWaveCount = 0;
            foreach (EnemyWave wave in Waves)
            {
                maxWaveCount += wave.Count;
            }

            // Create the player
            CreatePlayer();

            // Start the first wave
        }

        private EnemyWave? GetWave(int waveIndex)
        {

            if (currentWave >= maxWaveCount)
            {
                return null;
            }

            EnemyWave wave = new EnemyWave();
            int waveCount = 0;
            for (int i = 0; i < Waves.Length; i++)
            {
                wave = Waves[i];
                waveCount += wave.Count;
                if (currentWave < waveCount)
                {
                    break;
                }
            }

            return wave;
        }

        public void AddScore(int points)
        {
            score += points;
            if (score < 0)
            {
                score = 0;
            }
        }

        public void AddMoney(int amount)
        {
            money += amount;
            if (money < 0)
            {
                money = 0;
            }
        }

        private void NextWave()
        {
            currentWave++;
            if (currentWave >= maxWaveCount)
            {
                return;
            }

            // Spawn the next wave
            var wave = GetWave(currentWave);
            if (wave == null)
            {
                return;
            }
            // Set the timer for the wave
            waveStartTime = Time.time;

            // Load the wave from the template file
            var new_enemies = SceneTemplate.Load(wave.Name);

            // check if the wave was loaded successfully
            if (new_enemies == null || new_enemies.Count == 0)
            {
                Console.WriteLine("Error loading wave: " + wave.Name);
                return;
            }

            // add each enemy to the list of enemies
            foreach (GameObject enemy in new_enemies)
            {
                Enemies.AddLast(enemy);
            }

            Console.WriteLine("Wave " + currentWave + " started");

        }

        private bool LevelCompleted()
        {
            // Check if all waves have been completed
            return currentWave >= maxWaveCount;
        }

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

        private void OnFail()
        {
            this.Pause();
            // End the level as a failure
            // ...
        }

        private void OnWin()
        {
            this.Pause();
            // End the level as a success
            // ...
        }

        private void HandleInput()
        {
            // Check for pause input
            if (Input.GetKeyDown((int)SDL_Keycode.SDLK_ESCAPE))
            {
                if (this.escapeMenu != null)
                {
                    this.escapeMenu.Destroy();
                    this.escapeMenu = null;
                }
                else if(this.shopMenu != null)
                { 
                    this.shopMenu.Destroy();
                    this.shopMenu = null;
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
            if (Input.GetKeyDown((int)SDL_Keycode.SDLK_p))
            {
                SceneManager.SetScene(Shop.CreateScene());
            }
        }

        public override void Update()
        {
            HandleInput();
            if(this.paused)
            {
                return;
            }

            // Check if the level is completed
            if (LevelCompleted())
            {
                // End the level
                // ...
                return;
            }

            levelTimer += Time.deltaTime;

            // Check if the current wave is completed
            if (Enemies.Count == 0)
            {
                NextWave();
            }

            // check if the wave timer has run out
            var wave = GetWave(currentWave);
            if (wave != null)
            {
                if (Time.time - waveStartTime > wave.MaxTime)
                {
                    NextWave();
                }
            }

            // check if the player is dead
            // check for player health
            /*
             * if (Player.GetHealth() <= 0) {
             *    OnFail();
             * }
             */

            // check if the player has completed the level
            if (LevelCompleted())
            {
                OnWin();
            }

        }
    }
}
