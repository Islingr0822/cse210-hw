using System;
using System.Collections.Generic;

namespace DnDCharacterManager.Race
{
    /// <summary>
    /// Abstract base class for all character races.
    /// </summary>
    public abstract class Race
    {
        protected string _name;
        protected Size _size;
        protected int _speed;
        protected bool _darkvision;

        protected Race()
        {
            _name = "Common";
            _size = Size.Medium;
            _speed = 30;
            _darkvision = false;
        }

        public Race(string name, Size size, int speed, bool darkvision)
        {
            _name = name;
            _size = size;
            _speed = speed;
            _darkvision = darkvision;
        }

        // Properties
        public string Name { get => _name; set => _name = value; }
        public Size Size { get => _size; set => _size = value; }
        public int Speed { get => _speed; set => _speed = value; }
        public bool Darkvision { get => _darkvision; set => _darkvision = value; }

        // Methods
        public virtual void ApplyTraits()
        {
            Console.WriteLine($"Applying traits for {_name}.");
        }

        public override string ToString()
        {
            return $"{_name} (Size: {_size}, Speed: {_speed}, Darkvision: {_darkvision})";
        }
    }

    /// <summary>
    /// Enum for character sizes.
    /// </summary>
    public enum Size
    {
        Small,
        Medium,
        Large
    }

    // Concrete race implementations

    /// <summary>
    /// Human race.
    /// </summary>
    public class Human : Race
    {
        public Human() : base("Human", Size.Medium, 30, false)
        {
        }

        public Human(int speed) : base("Human", Size.Medium, speed, false)
        {
        }

        public override void ApplyTraits()
        {
            Console.WriteLine("Humans have no special racial traits.");
        }
    }

    /// <summary>
    /// Elf race.
    /// </summary>
    public class Elf : Race
    {
        public Elf() : base("Elf", Size.Medium, 30, true)
        {
        }

        public override void ApplyTraits()
        {
            Console.WriteLine("Elves have darkvision and faerie fire resilience.");
        }
    }

    /// <summary>
    /// Dwarf race.
    /// </summary>
    public class Dwarf : Race
    {
        public Dwarf() : base("Dwarf", Size.Medium, 25, true)
        {
        }

        public override void ApplyTraits()
        {
            Console.WriteLine("Dwarves have darkvision and resistance to poison damage.");
        }
    }

    /// <summary>
    /// Dragonborn race.
    /// </summary>
    public class Dragonborn : Race
    {
        public Dragonborn() : base("Dragonborn", Size.Medium, 30, false)
        {
        }

        public override void ApplyTraits()
        {
            Console.WriteLine("Dragonborn have breath weapon and resistance.");
        }
    }

    /// <summary>
    /// Half-Elf race.
    /// </summary>
    public class HalfElf : Race
    {
        public HalfElf() : base("Half-Elf", Size.Medium, 30, true)
        {
        }

        public override void ApplyTraits()
        {
            Console.WriteLine("Half-elves have darkvision and charming demeanor.");
        }
    }

    /// <summary>
    /// Half-Orc race.
    /// </summary>
    public class HalfOrc : Race
    {
        public HalfOrc() : base("Half-Orc", Size.Medium, 30, true)
        {
        }

        public override void ApplyTraits()
        {
            Console.WriteLine("Half-orcs have darkvision and fierce demeanor.");
        }
    }

    /// <summary>
    /// Halfling race.
    /// </summary>
    public class Halfling : Race
    {
        public Halfling() : base("Halfling", Size.Small, 25, false)
        {
        }

        public override void ApplyTraits()
        {
            Console.WriteLine("Halflings are nimble and lucky.");
        }
    }
}