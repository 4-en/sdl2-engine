using SDL2Engine;

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

        public override void Update()
        {
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
