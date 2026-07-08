using System;

namespace DnDCharacterManager.Feat
{
    /// <summary>
    /// Represents a character feat.
    /// </summary>
    public class Feat
    {
        protected string _name;
        protected string _description;

        public Feat()
        {
            _name = "Unnamed Feat";
            _description = "No description.";
        }

        public Feat(string name, string description)
        {
            _name = name;
            _description = description;
        }

        // Properties
        public string Name { get => _name; set => _name = value; }
        public string Description { get => _description; set => _description = value; }

        // Methods
        public virtual void ApplyBonus()
        {
            Console.WriteLine($"{_name} feat bonus is applied: {_description}");
        }

        public override string ToString()
        {
            return $"{_name}: {_description}";
        }
    }

    // Concrete feat implementations

    public class TwoWeaponFighting : Feat
    {
        public TwoWeaponFighting() : base("Two-Weapon Fighting", "You can fight with two weapons.")
        {
        }
    }

    public class Sharpshooter : Feat
    {
        public Sharpshooter() : base("Sharpshooter", "Ignore 5 feet of range for ranged attacks.")
        {
        }
    }

    public class HeavyArmorMaster : Feat
    {
        public HeavyArmorMaster() : base("Heavy Armor Master", "Reduce non-magical bludgeoning damage by 3.")
        {
        }
    }

    public class MagicInitiate : Feat
    {
        public MagicInitiate() : base("Magic Initiate", "Learn two cantrips and one 1st-level spell.")
        {
        }
    }

    public class Actor : Feat
    {
        public Actor() : base("Actor", "Improves stealth and disguise abilities.")
        {
        }
    }

    public class WarCaster : Feat
    {
        public WarCaster() : base("War Caster", "Advantage on concentration saves. Spell components with hands can be somatic even while wielding weapons.")
        {
        }
    }
}