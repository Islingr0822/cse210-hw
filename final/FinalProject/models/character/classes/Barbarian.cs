using System;
using DnDCharacterManager.Race;
using DnDCharacterManager.Background;
using DnDCharacterManager.Feature;

namespace DnDCharacterManager.Character.Classes
{
    /// <summary>
    /// Barbarian character class.
    /// </summary>
    public class Barbarian : Character
    {
        private bool _isRaging;
        private int _rageCount;

        public Barbarian() : base()
        {
            _isRaging = false;
            _rageCount = 2;
            Features.Add(new RageFeature());
        }

        public Barbarian(string name, int level, Race race, Background background)
            : base(name, level, race, background)
        {
            _isRaging = false;
            _rageCount = 2;
            Features.Add(new RageFeature());
        }

        // Properties
        public bool IsRaging { get => _isRaging; set => _isRaging = value; }
        public int RageCount { get => _rageCount; set => _rageCount = value; }

        // Class-specific ability
        public override void ClassSpecificAbility()
        {
            Rage();
        }

        /// <summary>
        /// Enter a battle fury.
        /// </summary>
        public virtual void Rage()
        {
            if (_rageCount > 0)
            {
                _isRaging = true;
                _rageCount--;
                Console.WriteLine($"{Name} enters a rage! Gain advantage on Strength checks and saves.");
            }
            else
            {
                Console.WriteLine($"{Name} has no rages remaining.");
            }
        }

        public virtual void EndRage()
        {
            _isRaging = false;
            Console.WriteLine($"{Name} stops raging.");
        }

        // Override base methods for Barbarian-specific behavior
        protected override void CalculateBaseStats()
        {
            MaxHitPoints = 12 + (Constitution / 2 - 5);
            HitPoints = MaxHitPoints;
            ArmorClass = 10 + (Dexterity / 2 - 5); // Unarmored defense
        }

        public override void Attack()
        {
            if (_isRaging)
            {
                Console.WriteLine($"{Name} attacks with rage-fueled fury!");
            }
            else
            {
                Console.WriteLine($"{Name} makes a melee attack.");
            }
        }

        public override void ShortRest()
        {
            base.ShortRest();
            Console.WriteLine("Barbarian regains half their remaining Rage uses on a short rest.");
            _rageCount += 1;
        }

        public override void DisplayCharacter()
        {
            string rageStatus = _isRaging ? "Active" : "Inactive";
            Console.WriteLine($"=== {Name} (Level {Level} Barbarian - {_race.Name}) ===");
            Console.WriteLine($"Hit Points: {HitPoints}/{MaxHitPoints} | AC: {ArmorClass} | Speed: {Speed}");
            Console.WriteLine($"Rage: {rageStatus} ({_rageCount} uses remaining)");
            Console.WriteLine("Ability Scores:");
            Console.WriteLine($"  Strength: {Strength} | Dexterity: {Dexterity} | Constitution: {Constitution}");
            Console.WriteLine($"  Intelligence: {Intelligence} | Wisdom: {Wisdom} | Charisma: {Charisma}");
        }
    }
}