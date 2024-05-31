using SDL2Engine;

namespace ShootEmUp.Level
{
    public struct EnemyWave
    {
        // the name of the file that contains the enemy wave
        public string Name;
        // the maximum time that the player has to defeat the wave
        public double MaxTime;
        // the number of times that the wave will spawn
        public int Count;

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
    }

    /*
     * The base level class for the shoot em up game. This class will manage the following:
     * - Creating the level (enemies, player, etc.)
     * - Handling the level's win/lose conditions
     * - General game loop
     */
    public class BaseLevel : Script
    {
        public EnemyWave[] Waves = new EnemyWave[0];
        private int currentWave = 0;


        public override void Start()
        {
            // Load all needed prototypes
        }

        public override void Update()
        {
            // Check for win/lose conditions
            // Update game loop
        }
    }
}
