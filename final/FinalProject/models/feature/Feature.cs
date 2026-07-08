using System;

namespace DnDCharacterManager.Feature
{
    /// <summary>
    /// Represents a character feature.
    /// </summary>
    public class Feature
    {
        protected string _name;
        protected string _description;

        public Feature()
        {
            _name = "Unnamed Feature";
            _description = "No description.";
        }

        public Feature(string name, string description)
        {
            _name = name;
            _description = description;
        }

        // Properties
        public string Name { get => _name; set => _name = value; }
        public string Description { get => _description; set => _description = value; }

        // Methods
        public virtual void UseFeature()
        {
            Console.WriteLine($"{_name} feature is used: {_description}");
        }

        public override string ToString()
        {
            return $"{_name}: {_description}";
        }
    }

    // Class-specific features

    public class RageFeature : Feature
    {
        public RageFeature() : base("Rage", "Enter a battle fury for 1 minute.")
        {
        }
    }

    public class BardicInspirationFeature : Feature
    {
        public BardicInspirationFeature() : base("Bardic Inspiration", "Roll a die and add it to an ally's check.")
        {
        }
    }

    public class DivineSenseFeature : Feature
    {
        public DivineSenseFeature() : base("Divine Sense", "Detect the presence of celestials, fiends, or undead.")
        {
        }
    }

    public class SpellcastingFeature : Feature
    {
        public SpellcastingFeature() : base("Spellcasting", "Cast spells using your class spellcasting ability.")
        {
        }
    }

    public class UnarmoredDefenseFeature : Feature
    {
        public UnarmoredDefenseFeature() : base("Unarmored Defense", "While wearing no armor, your AC equals 10 + Dex + Wis mod.")
        {
        }
    }
}