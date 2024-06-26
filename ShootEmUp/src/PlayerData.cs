﻿using SDL2Engine;

namespace ShootEmUp
{

    // This class contains all persistent data for the player
    // It is used to save and load data between game sessions or levels
    public class PlayerData
    {

        private static PlayerData? instance;

        // Returns the instance of the PlayerData class
        public static PlayerData Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = Load();
                }
                return instance;
            }
        }
        
        // Don't create instances of this class directly - use the Instance property
        public PlayerData()
        {
            // Initialize the data
        }

        public void Reset()
        {
            total_score = 0;
            level_progress = 0;
            money = 0;
            health_upgrade_level = 0;
            damage_upgrade_level = 0;
            speed_upgrade_level = 0;
            fire_rate_upgrade_level = 0;
            rocket_count = 3;
            boost_count = 3;
            bomb_count = 3;
        }

        private int rocket_count = 3;
        public int RocketCount
        {
            get { return rocket_count; }
            set
            {
                rocket_count = value;
                Save();
            }
        }

        private int boost_count = 3;
        public int BoostCount
        {
            get { return boost_count; }
            set
            {
                boost_count = value;
                Save();
            }
        }

        private int bomb_count = 3;
        public int BombCount
        {
            get { return bomb_count; }
            set
            {
                bomb_count = value;
                Save();
            }
        }

        private int total_score = 0;
        public int TotalScore
        {
            get { return total_score; }
            set { 
                total_score = value;
                Save();
            }
        }

        private int level_progress = 0;
        public int LevelProgress
        {
            get { return level_progress; }
            set
            {
                level_progress = value;
                Save();
            }
        }

        private int money = 0;
        public int Money
        {
            get { return money; }
            set
            {
                money = value;
                Save();
            }
        }

        private int health_upgrade_level = 0;
        public int HealthUpgradeLevel
        {
            get { return health_upgrade_level; }
            set
            {
                health_upgrade_level = value;
                Save();
            }
        }

        private int damage_upgrade_level = 0;
        public int DamageUpgradeLevel
        {
            get { return damage_upgrade_level; }
            set
            {
                damage_upgrade_level = value;
                Save();
            }
        }

        private int speed_upgrade_level = 0;
        public int SpeedUpgradeLevel
        {
            get { return speed_upgrade_level; }
            set
            {
                speed_upgrade_level = value;
                Save();
            }
        }

        private int fire_rate_upgrade_level = 0;
        public int FireRateUpgradeLevel
        {
            get { return fire_rate_upgrade_level; }
            set
            {
                fire_rate_upgrade_level = value;
                Save();
            }
        }

        public void Save()
        {
            // Save the data to a file
            Serialization.SaveObject(this, "player_data");
        }

        private static PlayerData Load()
        {
            // Load the data from a file
            PlayerData? data = Serialization.LoadObject<PlayerData>("player_data");
            if (data == null)
            {
                data = new PlayerData();
                data.Save();
                return data;
            }
            return data;
        }
    }
}
