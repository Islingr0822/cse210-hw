using System;
using DnDCharacterManager.Ability;

namespace DnDCharacterManager.Skill
{
    /// <summary>
    /// Represents a character skill.
    /// </summary>
    public class Skill
    {
        protected string _name;
        protected AbilityType _ability;
        protected bool _isProficient;

        public Skill()
        {
            _name = "Unnamed Skill";
            _ability = AbilityType.Strength;
            _isProficient = false;
        }

        public Skill(string name, AbilityType ability, bool isProficient = false)
        {
            _name = name;
            _ability = ability;
            _isProficient = isProficient;
        }

        // Properties
        public string Name { get => _name; set => _name = value; }
        public AbilityType Ability { get => _ability; set => _ability = value; }
        public bool IsProficient { get => _isProficient; set => _isProficient = value; }

        // Methods
        public virtual int CalculateModifier(int baseAbilityScore)
        {
            int modifier = (baseAbilityScore - 10) / 2;
            if (_isProficient)
            {
                modifier += 2; // Beginner proficiency bonus
            }
            return modifier;
        }

        public override string ToString()
        {
            string profPrefix = _isProficient ? "+" : "";
            return $"{_name} ({_ability}) [{profPrefix}{(_isProficient ? "Proficient" : "Not Proficient")}]";
        }
    }

    // Common D&D skills
    public class Acrobatics : Skill
    {
        public Acrobatics(bool proficient = false) : base("Acrobatics", AbilityType.Dexterity, proficient)
        {
        }
    }

    public class AnimalHandling : Skill
    {
        public AnimalHandling(bool proficient = false) : base("Animal Handling", AbilityType.Wisdom, proficient)
        {
        }
    }

    public class Arcana : Skill
    {
        public Arcana(bool proficient = false) : base("Arcana", AbilityType.Intelligence, proficient)
        {
        }
    }

    public class Athletics : Skill
    {
        public Athletics(bool proficient = false) : base("Athletics", AbilityType.Strength, proficient)
        {
        }
    }

    public class Deception : Skill
    {
        public Deception(bool proficient = false) : base("Deception", AbilityType.Charisma, proficient)
        {
        }
    }

    public class History : Skill
    {
        public History(bool proficient = false) : base("History", AbilityType.Intelligence, proficient)
        {
        }
    }

    public class Insight : Skill
    {
        public Insight(bool proficient = false) : base("Insight", AbilityType.Wisdom, proficient)
        {
        }
    }

    public class Intimidation : Skill
    {
        public Intimidation(bool proficient = false) : base("Intimidation", AbilityType.Charisma, proficient)
        {
        }
    }

    public class Investigation : Skill
    {
        public Investigation(bool proficient = false) : base("Investigation", AbilityType.Intelligence, proficient)
        {
        }
    }

    public class Medicine : Skill
    {
        public Medicine(bool proficient = false) : base("Medicine", AbilityType.Wisdom, proficient)
        {
        }
    }

    public class Nature : Skill
    {
        public Nature(bool proficient = false) : base("Nature", AbilityType.Intelligence, proficient)
        {
        }
    }

    public class Perception : Skill
    {
        public Perception(bool proficient = false) : base("Perception", AbilityType.Wisdom, proficient)
        {
        }
    }

    public class Performance : Skill
    {
        public Performance(bool proficient = false) : base("Performance", AbilityType.Charisma, proficient)
        {
        }
    }

    public class Persuasion : Skill
    {
        public Persuasion(bool proficient = false) : base("Persuasion", AbilityType.Charisma, proficient)
        {
        }
    }

    public class Religion : Skill
    {
        public Religion(bool proficient = false) : base("Religion", AbilityType.Intelligence, proficient)
        {
        }
    }

    public class Stealth : Skill
    {
        public Stealth(bool proficient = false) : base("Stealth", AbilityType.Dexterity, proficient)
        {
        }
    }

    public class Survival : Skill
    {
        public Survival(bool proficient = false) : base("Survival", AbilityType.Wisdom, proficient)
        {
        }
    }
}