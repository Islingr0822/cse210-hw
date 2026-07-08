using System;

namespace DnDCharacterManager.Spell
{
    /// <summary>
    /// Represents a D&D spell.
    /// </summary>
    public class Spell
    {
        protected string _name;
        protected int _level;
        protected School _school;
        protected string _castingTime;
        protected string _range;
        protected string _duration;

        public Spell()
        {
            _name = "Unnamed Spell";
            _level = 0;
            _school = School.None;
            _castingTime = "1 action";
            _range = "Self";
            _duration = "Instantaneous";
        }

        public Spell(string name, int level, School school, string castingTime, string range, string duration)
        {
            _name = name;
            _level = level;
            _school = school;
            _castingTime = castingTime;
            _range = range;
            _duration = duration;
        }

        // Properties
        public string Name { get => _name; set => _name = value; }
        public int Level { get => _level; set => _level = value; }
        public School School { get => _school; set => _school = value; }
        public string CastingTime { get => _castingTime; set => _castingTime = value; }
        public string Range { get => _range; set => _range = value; }
        public string Duration { get => _duration; set => _duration = value; }

        // Methods
        public virtual void Cast()
        {
            Console.WriteLine($"{_name} is cast! (Level {_level}, {_school})");
        }

        public override string ToString()
        {
            return $"{_name} (Level {_level}, {_school}) - Casting Time: {_castingTime}, Range: {_range}, Duration: {_duration}";
        }
    }

    /// <summary>
    /// Enum for spell schools.
    /// </summary>
    public enum School
    {
        None,
        Abjuration,
        Conjuration,
        Divination,
        Enchantment,
        Evocation,
        Illusion,
        Necromancy,
        Transmutation
    }

    // Common spell implementations
    public class Fireball : Spell
    {
        public Fireball() : base("Fireball", 3, School.Evocation, "1 action", "150 feet", "Instantaneous")
        {
        }

        public override void Cast()
        {
            Console.WriteLine("A bead of light shoots forth and explodes in a fiery blast!");
        }
    }

    public class MagicMissile : Spell
    {
        public MagicMissile() : base("Magic Missile", 1, School.Evocation, "1 action", "120 feet", "Instantaneous")
        {
        }
    }

    public class CureWounds : Spell
    {
        public CureWounds() : base("Cure Wounds", 1, School.Evocation, "1 action", "Touch", "Instantaneous")
        {
        }
    }

    public class Shield : Spell
    {
        public Shield() : base("Shield", 1, School.Abjuration, "1 reaction", "60 feet", "1 round")
        {
        }
    }

    public class Identify : Spell
    {
        public Identify() : base("Identify", 0, School.Divination, "1 minute", "Touch", "Instantaneous")
        {
        }
    }
}