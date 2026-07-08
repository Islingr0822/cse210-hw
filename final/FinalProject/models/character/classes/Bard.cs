using System;
using DnDCharacterManager.Race;
using DnDCharacterManager.Background;
using DnDCharacterManager.Feature;
using DnDCharacterManager.Spell;

namespace DnDCharacterManager.Character.Classes
{
    /// <summary>
    /// Bard character class.
    /// </summary>
    public class Bard : Character
    {
        private int _bardicInspirationDie;
        private int _spellcastingAbilityDC;

        public Bard() : base()
        {
            _bardicInspirationDie = 4;
            _spellcastingAbilityDC = 8;
            Features.Add(new Feature.BardicInspirationFeature());
            SpellBook = new SpellBook(Name);
        }

        public Bard(string name, int level, Race race, Background background)
            : base(name, level, race, background)
        {
            _bardicInspirationDie = 4;
            _spellcastingAbilityDC = 8;
            Features.Add(new Feature.BardicInspirationFeature());
            SpellBook = new SpellBook(name);
        }

        // Properties
        public int BardicInspirationDie { get => _bardicInspirationDie; set => _bardicInspirationDie = value; }
        public int SpellcastingAbilityDC { get => _spellcastingAbilityDC; set => _spellcastingAbilityDC = value; }

        // Class-specific ability
        public override void ClassSpecificAbility()
        {
            BardicInspiration();
        }

        /// <summary>
        /// Roll bardic inspiration die.
        /// </summary>
        public virtual void BardicInspiration()
        {
            Random rand = new Random();
            int dieRoll = rand.Next(1, _bardicInspirationDie + 1);
            Console.WriteLine($"{Name} uses Bardic Inspiration! Roll a d{_bardicInspirationDie} ({dieRoll}) and add to your check.");
        }

        protected override void CalculateBaseStats()
        {
            MaxHitPoints = 8 + (Constitution / 2 - 5);
            HitPoints = MaxHitPoints;
            ArmorClass = 10 + (Dexterity / 2 - 5);
        }

        public override void DisplayCharacter()
        {
            Console.WriteLine($"=== {Name} (Level {Level} Bard - {_race.Name}) ===");
            Console.WriteLine($"Hit Points: {HitPoints}/{MaxHitPoints} | AC: {ArmorClass} | Speed: {Speed}");
            Console.WriteLine($"Bardic Inspiration: d{_bardicInspirationDie} | Spell DC: {_spellcastingAbilityDC}");
            Console.WriteLine("Ability Scores:");
            Console.WriteLine($"  Strength: {Strength} | Dexterity: {Dexterity} | Constitution: {Constitution}");
            Console.WriteLine($"  Intelligence: {Intelligence} | Wisdom: {Wisdom} | Charisma: {Charisma}");
        }
    }
}