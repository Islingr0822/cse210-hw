using System;

namespace DnDCharacterManager.Ability
{
    /// <summary>
    /// Represents ability scores for a character.
    /// </summary>
    public class AbilityScore
    {
        protected int _strength;
        protected int _dexterity;
        protected int _constitution;
        protected int _intelligence;
        protected int _wisdom;
        protected int _charisma;

        public AbilityScore()
        {
            _strength = 10;
            _dexterity = 10;
            _constitution = 10;
            _intelligence = 10;
            _wisdom = 10;
            _charisma = 10;
        }

        public AbilityScore(int strength, int dexterity, int constitution, int intelligence, int wisdom, int charisma)
        {
            _strength = strength;
            _dexterity = dexterity;
            _constitution = constitution;
            _intelligence = intelligence;
            _wisdom = wisdom;
            _charisma = charisma;
        }

        // Properties
        public int Strength { get => _strength; set => _strength = value; }
        public int Dexterity { get => _dexterity; set => _dexterity = value; }
        public int Constitution { get => _constitution; set => _constitution = value; }
        public int Intelligence { get => _intelligence; set => _intelligence = value; }
        public int Wisdom { get => _wisdom; set => _wisdom = value; }
        public int Charisma { get => _charisma; set => _charisma = value; }

        // Methods
        /// <summary>
        /// Gets the modifier for a given ability score.
        /// </summary>
        public int GetModifier(AbilityType ability)
        {
            int score = GetAbilityValue(ability);
            return (score - 10) / 2;
        }

        /// <summary>
        /// Gets the modifier for a named ability.
        /// </summary>
        public int GetModifier(string abilityName)
        {
            int score = GetAbilityValue(abilityName);
            return (score - 10) / 2;
        }

        private int GetAbilityValue(AbilityType ability)
        {
            switch (ability)
            {
                case AbilityType.Strength: return _strength;
                case AbilityType.Dexterity: return _dexterity;
                case AbilityType.Constitution: return _constitution;
                case AbilityType.Intelligence: return _intelligence;
                case AbilityType.Wisdom: return _wisdom;
                case AbilityType.Charisma: return _charisma;
                default: return 10;
            }
        }

        private int GetAbilityValue(string abilityName)
        {
            switch (abilityName.ToLower())
            {
                case "strength": return _strength;
                case "dexterity": return _dexterity;
                case "constitution": return _constitution;
                case "intelligence": return _intelligence;
                case "wisdom": return _wisdom;
                case "charisma": return _charisma;
                default: return 10;
            }
        }

        public virtual void DisplayScores()
        {
            Console.WriteLine($"Strength: {_strength} | Dexterity: {_dexterity} | Constitution: {_constitution}");
            Console.WriteLine($"Intelligence: {_intelligence} | Wisdom: {_wisdom} | Charisma: {_charisma}");
        }

        public override string ToString()
        {
            return $"STR:{_strength} DEX:{_dexterity} CON:{_constitution} INT:{_intelligence} WIS:{_wisdom} CHA:{_charisma}";
        }
    }

    /// <summary>
    /// Enum for ability types.
    /// </summary>
    public enum AbilityType
    {
        Strength,
        Dexterity,
        Constitution,
        Intelligence,
        Wisdom,
        Charisma
    }
}