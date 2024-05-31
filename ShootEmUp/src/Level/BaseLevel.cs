using SDL2Engine;

namespace ShootEmUp.Level
{
    /*
     * A struct that represents an enemy wave. An enemy wave is a group of enemies that spawn at the same time.
     * The struct contains the name of a template file that contains the enemy wave, 
     * the maximum time that the player has to defeat the wave, and the number of times that the wave will spawn.
     * 
     * If the time runs out before the player defeats the wave, the next wave will spawn.
     * If the player clears the wave before the time runs out, the next wave will spawn.
     */
    public struct EnemyWave
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


        public void SetupLevel(int levelID, EnemyWave[] waves)
        {
            LevelID = levelID;
            Waves = waves;
            currentWave = -1;
        }

        public override void Start()
        {
            // count the total number of waves
            maxWaveCount = 0;
            foreach (EnemyWave wave in Waves)
            {
                maxWaveCount += wave.Count;
            }

            // Create the player here
            // ...

            // Start the first wave
        }

        private void NextWave()
        {
            currentWave++;
            if (currentWave >= maxWaveCount)
            {
                return;
            }

            // Spawn the next wave
            EnemyWave wave = new EnemyWave();
            int waveCount = 0;
            for(int i = 0; i < Waves.Length; i++)
            {
                wave = Waves[i];
                waveCount += wave.Count;
                if (currentWave < waveCount)
                {
                    break;
                }
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

        }

        private bool LevelCompleted()
        {
            // Check if all waves have been completed
            return currentWave >= Waves.Length;
        }

        public override void Update()
        {
            // Check for win/lose conditions
            // Update game loop
        }
    }
}
