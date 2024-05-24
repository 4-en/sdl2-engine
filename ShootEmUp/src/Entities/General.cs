using SDL2Engine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShootEmUp.Entities
{
    public enum Team
    {
        Player,
        Enemy,
        Neutral
    }
    public struct Damage
    {
        public double Value;
        public GameObject? Source;
        public Team Team = Team.Enemy;

        public Damage(double value)
        {
            Value = value;
        }

        public Damage(double value, GameObject source)
        {
            Value = value;
            Source = source;
        }

        public Damage(double value, GameObject source, Team team)
        {
            Value = value;
            Source = source;
            Team = team;
        }
    }
    public interface IDamageable
    {
        void Damage(Damage damage);

        void Heal(double value);
    }


}
