using SDL2Engine;
using ShootEmUp.src;
using System.Diagnostics.Tracing;
using static SDL2.SDL;
using static System.Net.Mime.MediaTypeNames;

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
        public static BaseLevel? Instance = null;

        public int LevelID = 0;
        // An array of enemy waves that will spawn in the level
        // This should be changed by inheriting classes to define the level's enemy waves,
        // or simply by setting the array after creating the level object
        public EnemyWave[] Waves = new EnemyWave[0];
        private int currentWave = -1;
        private int maxWaveCount = 0;
        private double waveStartTime = 0;
        private double levelTimer = 0;
        private double duration = 60;
        private bool paused = false;
        private LinkedList<GameObject> Enemies = new LinkedList<GameObject>();
        private GameObject? player;
        private Player? playerScript;
        private EventListener<EnemyKilledEvent>? eventListener = null;
        private TextRenderer? topCenterText = null;

        private static Vec2D worldSize = new Vec2D(2500, 2500);
        public static Vec2D WorldSize
        {
            get { return worldSize; }
        }

        private int score = 0;
        private int money = 0;

        private int combo = 0;
        private double lastKillTime = 0;

        private GameObject? escapeMenu = null;
        private GameObject? shopMenu = null;

        private bool was_setup = false;
        private bool player_died = false;

        public BaseLevel()
        {
            if (Instance == null)
            {
                Instance = this;
            }
            else
            {
                Console.WriteLine("Warning: Multiple instances of BaseLevel created.");
            }
        }

        public void SetupLevel(int levelID, EnemyWave[] waves, double duration = 60)
        {
            LevelID = levelID;
            Waves = waves;
            currentWave = -1;
            was_setup = true;
            this.duration = duration;
        }

        private void CreatePlayer()
        {
            var gameBounds = GetCamera()?.GetWorldSize() ?? new Vec2D(1920, 1080);


            // create the player with Player class
            player = Player.CreatePlayer();
            playerScript = player?.GetComponent<Player>();


            // create the Background
            var background = new GameObject("BackgroundObject");
            background.AddComponent<Background>();

            // collision test object
            /*
            var obstacle = new GameObject("Obstacle");
            var pb = obstacle.AddComponent<PhysicsBody>();
            var bc = obstacle.AddComponent<BoxCollider>();
            var texture = obstacle.AddComponent<TextureRenderer>();
            texture?.SetSource("Assets/Textures/forsenE.png");
            obstacle.transform.position = new Vec2D(400, 500);
            pb.Velocity = new Vec2D(1, 1);
            pb.IsMovable = true;*/

            /*
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
            */
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

            // Add the event listener for enemy killed events
            eventListener = EventBus.AddListener<EnemyKilledEvent>((eventData) =>
            {
                int points = CalculateCombo(eventData.enemy.GetPoints());
                int combo = this.combo - 1;

                // add point effect
                double right = GetCamera().GetVisibleWidth() - 100;
                Vec2D position = new Vec2D(right, 600);

                string text = points.ToString();

                switch(combo)
                {
                    case 2:
                        text = "Combo x2";
                        break;
                    case 5:
                        text = "Combo x5";
                        break;
                    case 10:
                        text = "Combo x10";
                        break;

                }

                int fontSize = 54 + Math.Min(combo, 10) * 5;

                GameText.CreateFixedAt(position, text, 2, fontSize, new SDL2Engine.Color(255, 255, 255, 255), 200, AnchorPoint.BottomRight);

                AddScore(points);
                AddMoney(points);
            });

            // add player damage event listener
            EventBus.AddListener<PlayerDamageEvent>((eventData) =>
            {
                // reset combo
                combo = 0;
            });

            // add text renderer
            var topCenter = new GameObject("TopCenterText");
            topCenterText = topCenter.AddComponent<TextRenderer>();
            topCenterText.SetColor(SDL2Engine.Color.White);
            topCenterText.SetFontSize(48);
            topCenterText.SetFontPath("Assets/Fonts/PressStartRegular.ttf");
            topCenterText.anchorPoint = AnchorPoint.Center;
            topCenter.SetPosition(new Vec2D(GetCamera().GetVisibleWidth() / 2, 100));
            topCenterText.SetText($"{(int)duration}s");
        }

        private int CalculateCombo(int points)
        {
            if (Time.time - lastKillTime < 2)
            {
                combo++;
            }
            else
            {
                combo = 0;
            }

            lastKillTime = Time.time;
            double multiplier = Math.Min(1 + combo, 10);
            int bonus = combo;
            points = (int)(points * multiplier) + bonus;
            // Console.WriteLine("Combo: " + combo + " Multiplier: " + multiplier + " Bonus: " + bonus);
            return points;
        }

        public override void OnDestroy()
        {
            if (eventListener != null)
            {
                EventBus.RemoveListener(eventListener);
            }

            if (BaseLevel.Instance == this)
            {
                BaseLevel.Instance = null;
            }
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

            PlayerData.Instance.TotalScore += points;
            PlayerData.Instance.Money += points;
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
            //return currentWave >= maxWaveCount;
            return duration <= 0;
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
            LevelManager.EndRun(false);
        }

        private void OnWin()
        {
            this.Pause();

            if(player_died)
            {
                OnFail();
                return;
            }

            // End the level as a success
            LevelManager.LoadShop();
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
                else if (this.shopMenu != null)
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

        private void RepositionEntities()
        {
            // if position is out of bounds, reposition the entity with modulo
            var gameBounds = worldSize;

            double minX = -gameBounds.x;
            double minY = -gameBounds.y;
            double maxX = gameBounds.x;
            double maxY = gameBounds.y;

            foreach (GameObject enemy in Enemies)
            {
                var position = enemy.GetPosition();
                if (position.x < minX)
                {
                    position.x = maxX;
                }
                if (position.x > maxX)
                {
                    position.x = minX;
                }
                if (position.y < minY)
                {
                    position.y = maxY;
                }
                if (position.y > maxY)
                {
                    position.y = minY;
                }
                enemy.transform.position = position;
            }

            if (player != null)
            {
                var position = player.GetPosition();
                if (position.x < minX)
                {
                    position.x = maxX;
                }
                if (position.x > maxX)
                {
                    position.x = minX;
                }
                if (position.y < minY)
                {
                    position.y = maxY;
                }
                if (position.y > maxY)
                {
                    position.y = minY;
                }
                player.transform.position = position;
            }
        }

        private int lastDuration = 999999;

        public override void Update()
        {

            if(player_died)
            {
                return;
            }

            HandleInput();
            if (this.paused)
            {
                return;
            }

            // Check if the level is completed
            if (LevelCompleted())
            {
                // End the level
                OnWin();
                return;
            }

            levelTimer += Time.deltaTime;
            duration -= Time.deltaTime;
            int intDuration = (int)duration;
            if (intDuration != lastDuration && intDuration >= 0)
            {
                lastDuration = intDuration;
                topCenterText?.GetGameObject().SetPosition(new Vec2D(GetCamera().GetVisibleWidth() / 2, 100));
                topCenterText?.SetText($"{intDuration}s");
            }

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

            // Reposition entities that are out of bounds
            // TODO: reanable if needed?
            // RepositionEntities();

            // check if the player is dead
            // check for player health
            if ((playerScript?.GetHealth() ?? -1) <= 0)
            {

                player_died = true;

                Delay(2, () =>
                {
                    OnFail();
                });

                GameObject? player = playerScript?.GetGameObject();
                if (player != null)
                {
                    player.Destroy();

                    Effects.ExplosionParticles(player.GetPosition(), 200);
                }
            }


        }
    }
}
