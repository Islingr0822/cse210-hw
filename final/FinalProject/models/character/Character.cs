using System;
using System.Collections.Generic;
using System.Linq;
using DnDCharacterManager.Race;
using DnDCharacterManager.Background;
using DnDCharacterManager.Inventory;
using DnDCharacterManager.Ability;
using DnDCharacterManager.Feature;
using DnDCharacterManager.Feat;
using DnDCharacterManager.Skill;
using DnDCharacterManager.Spell;

namespace DnDCharacterManager.Character
{
    /// <summary>
    /// Abstract base class for all D&D characters.
    /// </summary>
    public abstract class Character
    {
        // Core properties
        protected string _name;
        protected int _level;
        protected int _experience;
        protected int _hitPoints;
        protected int _maxHitPoints;
        protected int _armorClass;
        protected int _speed;

        // Ability scores
        protected int _strength;
        protected int _dexterity;
        protected int _constitution;
        protected int _intelligence;
        protected int _wisdom;
        protected int _charisma;

        // Related objects
        protected Race _race;
        protected Background _background;
        protected Inventory _inventory;
        protected AbilityScore _abilityScores;
        protected List<Feature> _features;
        protected List<Feat> _feats;
        protected List<Skill> _skills;
        protected SpellBook? _spellBook;

        // Constructors
        protected Character()
        {
            _name = "Unnamed";
            _level = 1;
            _experience = 0;
            _hitPoints = 0;
            _maxHitPoints = 0;
            _armorClass = 10;
            _speed = 30;

            _strength = 10;
            _dexterity = 10;
            _constitution = 10;
            _intelligence = 10;
            _wisdom = 10;
            _charisma = 10;

            _race = new Race();
            _background = new Background();
            _inventory = new Inventory();
            _abilityScores = new AbilityScore();
            _features = new List<Feature>();
            _feats = new List<Feat>();
            _skills = new List<Skill>();

            InitializeRace();
            InitializeBackground();
            CalculateBaseStats();
        }

        public Character(
            string name,
            int level,
            Race race,
            Background background)
        {
            _name = name;
            _level = level;
            _experience = 0;
            _armorClass = 10;
            _speed = 30;

            _strength = 10;
            _dexterity = 10;
            _constitution = 10;
            _intelligence = 10;
            _wisdom = 10;
            _charisma = 10;

            _race = race;
            _background = background;
            _inventory = new Inventory();
            _abilityScores = new AbilityScore(_strength, _dexterity, _constitution, _intelligence, _wisdom, _charisma);
            _features = new List<Feature>();
            _feats = new List<Feat>();
            _skills = new List<Skill>();

            ApplyRaceTraits();
            CalculateBaseStats();
        }

        // Properties
        public string Name
        {
            get => _name;
            set => _name = value;
        }

        public int Level
        {
            get => _level;
            set => _level = value;
        }

        public int Experience
        {
            get => _experience;
            set => _experience = value;
        }

        public int HitPoints
        {
            get => _hitPoints;
            set => _hitPoints = value;
        }

        public int MaxHitPoints
        {
            get => _maxHitPoints;
            set => _maxHitPoints = value;
        }

        public int ArmorClass
        {
            get => _armorClass;
            set => _armorClass = value;
        }

        public int Speed
        {
            get => _speed;
            set => _speed = value;
        }

        public int Strength
        {
            get => _strength;
            set => _strength = value;
        }

        public int Dexterity
        {
            get => _dexterity;
            set => _dexterity = value;
        }

        public int Constitution
        {
            get => _constitution;
            set => _constitution = value;
        }

        public int Intelligence
        {
            get => _intelligence;
            set => _intelligence = value;
        }

        public int Wisdom
        {
            get => _wisdom;
            set => _wisdom = value;
        }

        public int Charisma
        {
            get => _charisma;
            set => _charisma = value;
        }

        public Race Race { get => _race; set => _race = value; }
        public Background Background { get => _background; set => _background = value; }
        public Inventory Inventory { get => _inventory; set => _inventory = value; }
        public AbilityScore AbilityScores { get => _abilityScores; set => _abilityScores = value; }

        public List<Feature> Features { get => _features; set => _features = value; }
        public List<Feat> Feats { get => _feats; set => _feats = value; }
        public List<Skill> Skills { get => _skills; set => _skills = value; }

        public SpellBook? SpellBook { get => _spellBook; set => _spellBook = value; }

        // Abstract methods that must be implemented by derived classes
        public abstract void ClassSpecificAbility();

        // Core character methods
        public virtual void Attack()
        {
            Console.WriteLine($"{_name} attempts an attack.");
        }

        public virtual void TakeDamage(int damage)
        {
            _hitPoints = Math.Max(0, _hitPoints - damage);
            Console.WriteLine($"{_name} takes {damage} damage. Remaining HP: {_hitPoints}");
        }

        public virtual void Heal(int amount)
        {
            int previousHP = _hitPoints;
            _hitPoints = Math.Min(_maxHitPoints, _hitPoints + amount);
            Console.WriteLine($"{_name} heals for {(_hitPoints - previousHP)} hit points. Current HP: {_hitPoints}/{_maxHitPoints}");
        }

        public virtual void LevelUp()
        {
            _level++;
            CalculateBaseStats();
            Console.WriteLine($"{_name} has reached level {_level}!");
        }

        public virtual void DisplayCharacter()
        {
            Console.WriteLine($"=== {_name} (Level {_level} {_race.Name} {_background.Name}) ===");
            Console.WriteLine($"Hit Points: {_hitPoints}/{_maxHitPoints} | AC: {_armorClass} | Speed: {_speed}");
            Console.WriteLine("Ability Scores:");
            Console.WriteLine($"  Strength: {_strength} | Dexterity: {_dexterity} | Constitution: {_constitution}");
            Console.WriteLine($"  Intelligence: {_intelligence} | Wisdom: {_wisdom} | Charisma: {_charisma}");
        }

        public virtual void LongRest()
        {
            Console.WriteLine($"{_name} takes a long rest and recovers all hit points.");
            _hitPoints = _maxHitPoints;
        }

        public virtual void ShortRest()
        {
            Console.WriteLine($"{_name} takes a short rest.");
        }

        // Virtual methods for customization
        protected virtual void InitializeRace() { }
        protected virtual void InitializeBackground() { }
        protected virtual void ApplyRaceTraits() { }
        protected virtual void CalculateBaseStats() { }
    }
}