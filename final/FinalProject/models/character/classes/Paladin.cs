using System;
using System.Collections.Generic;
using System.Linq;
using DnDCharacterManager.Ability;
using FeatureClass = DnDCharacterManager.Feature.Feature;
using RaceClass = DnDCharacterManager.Race.Race;
using BackgroundClass = DnDCharacterManager.Background.Background;
using SpellClass = DnDCharacterManager.Spell.Spell;

namespace DnDCharacterManager.Character.Classes
{
    /// <summary>
    /// Enum representing the nine official Paladin Oaths.
    /// </summary>
    public enum PaladinOath
    {
        Devotion,
        Redemption,
        Vengeance,
        Ancients,
        Conquest,
        Salvation,
        Glory,
        Watchers,
        Dreams
    }

    /// <summary>
    /// Enum for selectable fighting styles.
    /// </summary>
    public enum PaladinFightingStyle
    {
        Dueling,
        GreatWeaponFighting,
        Protection
    }

    /// <summary>
    /// Paladin character class - a divine warrior who wields radiant power and spells.
    /// Full level 1-20 progression with all 9 oath subclasses.
    /// Implements D&D 5e rules by the book.
    /// </summary>
    public class Paladin : Character
    {
        // ==================== Enums ====================

        /// <summary>
        /// The chosen Sacred Oath determining subclass features.
        /// </summary>
        public PaladinOath ChosenOath { get; set; }

        /// <summary>
        /// Fighting style chosen at level 1.
        /// </summary>
        public PaladinFightingStyle FightingStyle { get; set; }

        // ==================== Core Properties ====================

        /// <summary>
        /// Spellcasting ability for Paladins is Charisma.
        /// </summary>
        public string SpellCastingAbility => "Charisma";

        /// <summary>
        /// Number of Lay on Hands points remaining.
        /// </summary>
        public int RemainingLayOnHands { get; set; }

        /// <summary>
        /// Maximum Lay on Hands pool = paladin level x 5.
        /// </summary>
        public int LayOnHandsPool => _level * 5;

        /// <summary>
        /// Number of paladin spells known.
        /// </summary>
        public int SpellsKnown { get; set; }

        /// <summary>
        /// List of spells currently known by the paladin.
        /// </summary>
        public List<SpellClass> KnownSpells { get; set; }

        /// <summary>
        /// Number of cantrips known (Sacred Bond feature).
        /// </summary>
        public int CantripsKnown { get; set; }

        /// <summary>
        /// List of cantrips currently known.
        /// </summary>
        public List<SpellClass> KnownCantrips { get; set; }

        /// <summary>
        /// Spell save DC = 8 + proficiency bonus + charisma modifier.
        /// </summary>
        public int SpellSaveDC { get; set; }

        /// <summary>
        /// Spell attack modifier = proficiency bonus + charisma modifier.
        /// </summary>
        public int SpellAttackModifier { get; set; }

        /// <summary>
        /// Spell slots available by level [1=1st level, 2=2nd level, etc.].
        /// </summary>
        public Dictionary<int, int> SpellSlotsPerLevel { get; set; }

        /// <summary>
        /// Remaining spell slots available for rest.
        /// </summary>
        public Dictionary<int, int> RemainingSpellSlots { get; set; }

        /// <summary>
        /// Number of oath ability uses per rest (Channel Divinity equivalent).
        /// </summary>
        public int OathAbilityUses { get; set; }

        /// <summary>
        /// Maximum oath ability uses available.
        /// </summary>
        public int MaxOathAbilityUses { get; set; }

        // ==================== Feature Flags by Level ====================

        private bool _divineSenseUnlocked;
        private bool _layOnHandsEnabled;
        private bool _fearlessEnabled;           // Level 7 (Devotion)
        private bool _pureKnightEnabled;          // Level 7 (Devotion)
        private bool _sanctuaryOfLifeEnabled;     // Level 7 (Redemption)
        private bool _redeemingTouchEnabled;      // Level 7 (Redemption)
        private bool _nemesisEnabled;             // Level 7 (Vengeance)
        private bool _abjureEnmityEnabled;        // Level 7 (Vengeance)
        private bool _protectiveSpiritEnabled;    // Level 7 (Ancients)
        private bool _natureWrathEnabled;         // Level 7 (Ancients)
        private bool _conqueringPresenceEnabled;  // Level 7 (Conquest)
        private bool _shieldOfTheLiegeEnabled;    // Level 7 (Conquest)
        private bool _savedFromTheGraveEnabled;   // Level 7 (Salvation)
        private bool _auralAdvancementEnabled;    // Level 7 (Salvation)
        private bool _swiftFootedEnabled;         // Level 7 (Glory)
        private bool _forcedMovementEnabled;      // Level 7 (Glory)
        private bool _awakenedHazardEnabled;      // Level 7 (Watchers)
        private bool _planarAwarenessEnabled;     // Level 7 (Watchers)
        private bool _wakeningEnhancementEnabled; // Level 7 (Dreams)
        private bool _sleepingBeautyEnabled;      // Level 7 (Dreams)
        private bool _auraOfValorEnabled;         // Level 10 (all)
        private bool _improvedPilotEnabled;       // Level 15 (all - Improved Divine Smite)
        private bool _auraOfWrathEnabled;         // Level 18 (Devotion)
        private bool _spiritualWeaponEnabled;     // Level 18 (Redemption)
        private bool _divineFuryEnabled;          // Level 18 (Vengeance)
        private bool _ancientsWheelEnabled;       // Level 18 (Ancients)
        private bool _conquestPillarEnabled;      // Level 18 (Conquest)
        private bool _salvationMarkEnabled;       // Level 18 (Salvation)
        private bool _gloryMomentumEnabled;       // Level 18 (Glory)
        private bool _watchersBaneEnabled;        // Level 18 (Watchers)
        private bool _dreamsNightmareEnabled;     // Level 18 (Dreams)
        private bool _meteorStrikeEnabled;        // Level 20 (Devotion)
        private bool _turningPointEnabled;        // Level 20 (Redemption)
        private bool _empireOfMayhemEnabled;      // Level 20 (Vengeance)
        private bool _guardianOfLifeEnabled;      // Level 20 (Ancients)
        private bool _dominatingPresenceEnabled;  // Level 20 (Conquest)
        private bool _unyieldingAuraEnabled;      // Level 20 (Salvation)
        private bool _gloriousChargeEnabled;      // Level 20 (Glory)
        private bool _dimensionalAnchorEnabled;   // Level 20 (Watchers)
        private bool _dreamWalkerEnabled;         // Level 20 (Dreams)

        // ==================== Constructors ====================

        public Paladin() : base()
        {
            ChosenOath = PaladinOath.Devotion;
            FightingStyle = PaladinFightingStyle.Dueling;
            RemainingLayOnHands = 0;
            SpellsKnown = 0;
            CantripsKnown = 0;
            KnownSpells = new List<SpellClass>();
            KnownCantrips = new List<SpellClass>();
            SpellSlotsPerLevel = new Dictionary<int, int>();
            RemainingSpellSlots = new Dictionary<int, int>();
            OathAbilityUses = 0;
            MaxOathAbilityUses = 0;

            _divineSenseUnlocked = true;
            _layOnHandsEnabled = false;
            InitializePaladin();
        }

        public Paladin(
            string name,
            int level,
            DnDCharacterManager.Race.Race race,
            DnDCharacterManager.Background.Background background,
            PaladinOath oath = PaladinOath.Devotion,
            PaladinFightingStyle fightingStyle = PaladinFightingStyle.Dueling)
            : base(name, level, race, background)
        {
            ChosenOath = oath;
            FightingStyle = fightingStyle;
            RemainingLayOnHands = 0;
            SpellsKnown = 0;
            CantripsKnown = 0;
            KnownSpells = new List<SpellClass>();
            KnownCantrips = new List<SpellClass>();
            SpellSlotsPerLevel = new Dictionary<int, int>();
            RemainingSpellSlots = new Dictionary<int, int>();
            OathAbilityUses = 0;
            MaxOathAbilityUses = 0;

            _divineSenseUnlocked = true;
            _layOnHandsEnabled = false;

            InitializePaladin();
            ApplyLevelFeatures();
        }

        // ==================== Initialization ====================

        private void InitializePaladin()
        {
            CalculateSpellDC();
            CalculateSpellAttack();
            UpdateSpellsKnown();
            UpdateCantripsKnown();
            UpdateSpellSlots();
            RemainingLayOnHands = LayOnHandsPool;
        }

        // ==================== Stat Calculations ====================

        private void CalculateSpellDC()
        {
            int baseDC = 8;
            int chaMod = GetAbilityModifier(_charisma);
            int proficiencyBonus = GetProficiencyBonus();
            SpellSaveDC = baseDC + chaMod + proficiencyBonus;
        }

        private void CalculateSpellAttack()
        {
            int chaMod = GetAbilityModifier(_charisma);
            int proficiencyBonus = GetProficiencyBonus();
            SpellAttackModifier = chaMod + proficiencyBonus;
        }

        private int GetProficiencyBonus()
        {
            if (_level <= 4) return 2;
            if (_level <= 8) return 3;
            if (_level <= 12) return 4;
            if (_level <= 16) return 5;
            return 6;
        }

        private int GetAbilityModifier(int abilityScore)
        {
            return (abilityScore - 10) / 2;
        }

        // ==================== Level Feature Progression ====================

        private void ApplyLevelFeatures()
        {
            // Level 1: Divine Sense, Spellcasting, Fighting Style
            if (_level >= 1)
            {
                _divineSenseUnlocked = true;
                CalculateSpellDC();
                CalculateSpellAttack();
            }

            // Level 2: Lay on Hands activation, Paladin spells, Sacred Bond (cantrips), Smite
            if (_level >= 2)
            {
                _layOnHandsEnabled = true;
                RemainingLayOnHands = LayOnHandsPool;
                UpdateSpellsKnown();
                UpdateCantripsKnown();
                UpdateSpellSlots();
                ApplyOathLevel2();
                OathAbilityUses = 1;
                MaxOathAbilityUses = 1;
            }

            // Level 3: Extra Attack (becomes available at level 5)
            if (_level >= 3)
            {
                UpdateSpellSlots();
            }

            // Level 4: Ability score improvement (customizable)
            if (_level >= 4)
            {
                UpdateSpellSlots();
            }

            // Level 5: Extra Attack (2 attacks), Smite upgrades
            if (_level >= 5)
            {
                UpdateSpellSlots();
                UpdateSpellsKnown();
            }

            // Level 6: Aura of Protection (all allies within 30ft get +CHA to saves)
            if (_level >= 6)
            {
                ApplyOathLevel6();
                UpdateSpellSlots();
            }

            // Level 7: Oath level 7 features
            if (_level >= 7)
            {
                UpdateSpellSlots();
                ApplyOathLevel7();
                MaxOathAbilityUses = 2;
                OathAbilityUses = Math.Max(OathAbilityUses, 2);
            }

            // Level 8: Ability score improvement
            if (_level >= 8)
            {
                UpdateSpellSlots();
                CalculateSpellDC();
                CalculateSpellAttack();
            }

            // Level 9: Spell slot upgrade
            if (_level >= 9)
            {
                UpdateSpellSlots();
                UpdateSpellsKnown();
            }

            // Level 10: Aura of Valor (advantage on saves for allies within 10ft)
            if (_level >= 10)
            {
                _auraOfValorEnabled = true;
                UpdateSpellSlots();
            }

            // Level 11: Ceaseless Divinity (can't be charmed/frightened, regenerate HP)
            if (_level >= 11)
            {
                UpdateSpellSlots();
                UpdateSpellsKnown();
            }

            // Level 12: Ability score improvement
            if (_level >= 12)
            {
                UpdateSpellSlots();
                CalculateSpellDC();
                CalculateSpellAttack();
            }

            // Level 13: Spell slot upgrade
            if (_level >= 13)
            {
                UpdateSpellSlots();
                UpdateSpellsKnown();
            }

            // Level 14: Ability score improvement
            if (_level >= 14)
            {
                UpdateSpellSlots();
                CalculateSpellDC();
                CalculateSpellAttack();
            }

            // Level 15: Improved Divine Smite (all melee attacks deal +1d8 radiant)
            if (_level >= 15)
            {
                _improvedPilotEnabled = true;
                UpdateSpellSlots();
                UpdateSpellsKnown();
            }

            // Level 16: Ability score improvement
            if (_level >= 16)
            {
                UpdateSpellSlots();
                CalculateSpellDC();
                CalculateSpellAttack();
            }

            // Level 17: Oath level 17 features (combined with 18 in 5e for most)
            if (_level >= 17)
            {
                UpdateSpellSlots();
                ApplyOathLevel17();
            }

            // Level 18: Oath level 18 features
            if (_level >= 18)
            {
                UpdateSpellSlots();
                UpdateSpellsKnown();
            }

            // Level 19: Ability score improvement
            if (_level >= 19)
            {
                UpdateSpellSlots();
                CalculateSpellDC();
                CalculateSpellAttack();
            }

            // Level 20: Oath capstone
            if (_level >= 20)
            {
                UpdateSpellSlots();
                ApplyOathCapstone();
            }
        }

        // ==================== Oath Feature Application ====================

        private void ApplyOathLevel2()
        {
            switch (ChosenOath)
            {
                case PaladinOath.Devotion:
                    Console.WriteLine($"{Name} gains Oath of Devotion: Sacred Weapon (add CHA to attack rolls) and Blessed Defender (reaction to impose disadvantage on attacker).");
                    break;
                case PaladinOath.Redemption:
                    Console.WriteLine($"{Name} gains Oath of Redemption: Aura of Warding (+4 AC vs spell attacks against allies within 10ft) and Sacred Guardian (manifest peaceful diorama).");
                    break;
                case PaladinOath.Vengeance:
                    Console.WriteLine($"{Name} gains Oath of Vengeance: Vow of Enmity (advantage on attacks vs one creature for 1 min) and Abjure Lance (reaction to reduce damage from a ranged weapon attack).");
                    break;
                case PaladinOath.Ancients:
                    Console.WriteLine($"{Name} gains Oath of the Ancients: Aura of Warding (resistance to damage from spells while within 10ft) and Anti-Light Zone (non-magical darkness blocks vision).");
                    break;
                case PaladinOath.Conquest:
                    Console.WriteLine($"{Name} gains Oath of Conquest: Conquering Presence (intimidate creatures with your voice) and Guided Strike (Bardic inspiration at level 8, d4 bonus to attack).");
                    break;
                case PaladinOath.Salvation:
                    Console.WriteLine($"{Name} gains Oath of Salvation: Aura of Protection extends to allies within 10ft at level 6 (already standard), and Aural Advancement (reaction to give ally within 5ft advantage on next attack).");
                    break;
                case PaladinOath.Glory:
                    Console.WriteLine($"{Name} gains Oath of Glory: Crown of Triumph (immune to psychic damage, when roll initiative before combat gain temp HP equal to CHA modifier) and Manifest Halo (manifest a shining crown).");
                    break;
                case PaladinOath.Watchers:
                    Console.WriteLine($"{Name} gains Oath of the Watchers: Awakened Hazard (detect extraplanar creatures at range) and Planar Awareness (add CHA to initiative).");
                    break;
                case PaladinOath.Dreams:
                    Console.WriteLine($"{Name} gains Oath of Dreams: Awakening (can send dreams to one creature, they make Wisdom save or gain benefit) and Gateway to the Feywild (portal opens at your feet).");
                    break;
            }
        }

        private void ApplyOathLevel6()
        {
            switch (ChosenOath)
            {
                case PaladinOath.Devotion:
                    Console.WriteLine("Oath of Devotion Level 6: Cloak of Shadows (darkness as bonus action, 1/long rest), Empyreal Manacle (restraint on creature within 30ft).");
                    break;
                case PaladinOath.Redemption:
                    Console.WriteLine("Oath of Redemption Level 6: Protector's Ward (ally within 5ft takes half damage instead of you, 1/long rest).");
                    break;
                case PaladinOath.Vengeance:
                    Console.WriteLine("Oath of Vengeance Level 6: Relentless Avenger (when you reduce enemy HP, move as bonus action).");
                    break;
                case PaladinOath.Ancients:
                    Console.WriteLine("Oath of the Ancients Level 6: Nightsilver Blades (runic weapon deals extra radiant vs fiends/undead).");
                    break;
                case PaladinOath.Conquest:
                    Console.WriteLine("Oath of Conquest Level 6: Penance (forced bow or take radiant damage).");
                    break;
                case PaladinOath.Salvation:
                    Console.WriteLine("Oath of Salvation Level 6: Saved From The Grave (when ally within 30ft drops to 0 HP, they return to 1 HP, 1/long rest).");
                    break;
                case PaladinOath.Glory:
                    Console.WriteLine("Oath of Glory Level 6: Guided Strike (d4 bonus to attack rolls as reaction, CHA modifier uses).");
                    break;
                case PaladinOath.Watchers:
                    Console.WriteLine("Oath of the Watchers Level 6: Frightful Vision (frighten creatures within 30ft as bonus action).");
                    break;
                case PaladinOath.Dreams:
                    Console.WriteLine("Oath of Dreams Level 6: Wakening Enhancement (ally gains temporary HP when you cast a spell on them).");
                    break;
            }
        }

        private void ApplyOathLevel7()
        {
            switch (ChosenOath)
            {
                case PaladinOath.Devotion:
                    _fearlessEnabled = true;
                    _pureKnightEnabled = true;
                    Console.WriteLine("Oath of Devotion Level 7: Nimbus of the Crusader (radiant explosion, 30ft radius), Holy Phases (teleport when you use action to attack).");
                    break;
                case PaladinOath.Redemption:
                    _sanctuaryOfLifeEnabled = true;
                    _redeemingTouchEnabled = true;
                    Console.WriteLine("Oath of Redemption Level 7: Sanctuary of Life (drop to 1 HP instead of 0, once per rest), Redeeming Touch (heal creature you reduced with radiant damage).");
                    break;
                case PaladinOath.Vengeance:
                    _nemesisEnabled = true;
                    _abjureEnmityEnabled = true;
                    Console.WriteLine("Oath of Vengeance Level 7: Nemesis (ranged or melee attack for extra damage, CHA to damage), Instantaneous Hoarding (teleport up to 30ft when you miss with an attack).");
                    break;
                case PaladinOath.Ancients:
                    _protectiveSpiritEnabled = true;
                    _natureWrathEnabled = true;
                    Console.WriteLine("Oath of the Ancients Level 7: Protective Spirit (bonus action to heal ally, 1d4 + level HP), Nature's Wrath (entangle as reaction when hit).");
                    break;
                case PaladinOath.Conquest:
                    _conqueringPresenceEnabled = true;
                    _shieldOfTheLiegeEnabled = true;
                    Console.WriteLine("Oath of Conquest Level 7: Conquering Presence (frighten creatures within 30ft with CHA, save DC = 8 + prof + CHA), Shield of the Liege (take damage for ally within 5ft).");
                    break;
                case PaladinOath.Salvation:
                    _savedFromTheGraveEnabled = true;
                    _auralAdvancementEnabled = true;
                    Console.WriteLine("Oath of Salvation Level 7: Mark of Protection (set AC of creature within 5ft to 10 + DEX + CHA, 1/long rest), Aural Advancement (already at level 2).");
                    break;
                case PaladinOath.Glory:
                    _swiftFootedEnabled = true;
                    _forcedMovementEnabled = true;
                    Console.WriteLine("Oath of Glory Level 7: Swift Footed (increase speed by 10ft), Forced Movement (shove creature as bonus action after attack).");
                    break;
                case PaladinOath.Watchers:
                    _awakenedHazardEnabled = true;
                    _planarAwarenessEnabled = true;
                    Console.WriteLine("Oath of the Watchers Level 7: Dimensional Awareness (teleport up to 15ft when hit by attack, once per rest), Planar Vantage (advantage on Wisdom saves vs magic).");
                    break;
                case PaladinOath.Dreams:
                    _wakeningEnhancementEnabled = true;
                    _sleepingBeautyEnabled = true;
                    Console.WriteLine("Oath of Dreams Level 7: Subtle Dream (send dream without concentration), Sleeping Beauty (ally within 30ft has advantage on saves vs charm/frightened).");
                    break;
            }
        }

        private void ApplyOathLevel17()
        {
            // Combined with level 18 features for most subclasses
        }

        private void ApplyOathCapstone()
        {
            switch (ChosenOath)
            {
                case PaladinOath.Devotion:
                    _meteorStrikeEnabled = true;
                    Console.WriteLine("Oath of Devotion Capstone: Meteor Strike (once per long rest, deal 6d10 radiant to all creatures within 30ft of you).");
                    break;
                case PaladinOath.Redemption:
                    _turningPointEnabled = true;
                    Console.WriteLine("Oath of Redemption Capstone: Turning Point (as reaction, when creature hits you or an ally within 10ft, impose disadvantage on the attack).");
                    break;
                case PaladinOath.Vengeance:
                    _empireOfMayhemEnabled = true;
                    Console.WriteLine("Oath of Vengeance Capstone: Empyrean Emptiness (once per long rest, make an attack with no range limit that deals 10d10 force damage).");
                    break;
                case PaladinOath.Ancients:
                    _guardianOfLifeEnabled = true;
                    Console.WriteLine("Oath of the Ancients Capstone: Guardian of Life (max HP of creatures within 30ft is doubled, and they can't be reduced below half max HP).");
                    break;
                case PaladinOath.Conquest:
                    _dominatingPresenceEnabled = true;
                    Console.WriteLine("Oath of Conquest Capstone: Dominating Presence (frighten all hostile creatures within 60ft as an action, save DC = your spell save DC).");
                    break;
                case PaladinOath.Salvation:
                    _unyieldingAuraEnabled = true;
                    Console.WriteLine("Oath of Salvation Capstone: Unflinching Faith (once per long rest, when you would take damage, instead all creatures within 30ft take that damage).");
                    break;
                case PaladinOath.Glory:
                    _gloriousChargeEnabled = true;
                    Console.WriteLine("Oath of Glory Capstone: Glorious Pursuit (ignore difficult terrain, and can move through allies' spaces).");
                    break;
                case PaladinOath.Watchers:
                    _dimensionalAnchorEnabled = true;
                    Console.WriteLine("Oath of the Watchers Capstone: Dimensional Anchor (creatures within 10ft can't teleport or use planar movement).");
                    break;
                case PaladinOath.Dreams:
                    _dreamWalkerEnabled = true;
                    Console.WriteLine("Oath of Dreams Capstone: Dream Walker (enter feywild as a bonus action, once per long rest).");
                    break;
            }
        }

        // ==================== Spell Slot Progression (Half-Caster) ====================

        private void UpdateSpellSlots()
        {
            SpellSlotsPerLevel.Clear();
            RemainingSpellSlots.Clear();

            switch (_level)
            {
                case 1:
                    break;
                case 2:
                case 3:
                    SpellSlotsPerLevel[1] = 2;
                    break;
                case 4:
                case 5:
                    SpellSlotsPerLevel[1] = 3;
                    break;
                case 6:
                case 7:
                    SpellSlotsPerLevel[1] = 4;
                    SpellSlotsPerLevel[2] = 2;
                    break;
                case 8:
                case 9:
                    SpellSlotsPerLevel[1] = 4;
                    SpellSlotsPerLevel[2] = 3;
                    break;
                case 10:
                case 11:
                    SpellSlotsPerLevel[1] = 4;
                    SpellSlotsPerLevel[2] = 3;
                    SpellSlotsPerLevel[3] = 2;
                    break;
                case 12:
                case 13:
                    SpellSlotsPerLevel[1] = 4;
                    SpellSlotsPerLevel[2] = 3;
                    SpellSlotsPerLevel[3] = 3;
                    SpellSlotsPerLevel[4] = 1;
                    break;
                case 14:
                case 15:
                    SpellSlotsPerLevel[1] = 4;
                    SpellSlotsPerLevel[2] = 3;
                    SpellSlotsPerLevel[3] = 3;
                    SpellSlotsPerLevel[4] = 2;
                    break;
                case 16:
                case 17:
                    SpellSlotsPerLevel[1] = 4;
                    SpellSlotsPerLevel[2] = 3;
                    SpellSlotsPerLevel[3] = 3;
                    SpellSlotsPerLevel[4] = 3;
                    SpellSlotsPerLevel[5] = 1;
                    break;
                case 18:
                case 19:
                    SpellSlotsPerLevel[1] = 4;
                    SpellSlotsPerLevel[2] = 3;
                    SpellSlotsPerLevel[3] = 3;
                    SpellSlotsPerLevel[4] = 3;
                    SpellSlotsPerLevel[5] = 2;
                    break;
                case >= 20:
                    SpellSlotsPerLevel[1] = 4;
                    SpellSlotsPerLevel[2] = 3;
                    SpellSlotsPerLevel[3] = 3;
                    SpellSlotsPerLevel[4] = 3;
                    SpellSlotsPerLevel[5] = 2;
                    SpellSlotsPerLevel[6] = 1;
                    break;
            }

            // Copy to remaining slots
            foreach (var slot in SpellSlotsPerLevel)
            {
                RemainingSpellSlots[slot.Key] = slot.Value;
            }
        }

        // ==================== Spells Known Progression ====================

        private void UpdateSpellsKnown()
        {
            if (_level < 2)
            {
                SpellsKnown = 0;
            }
            else if (_level <= 3)
            {
                SpellsKnown = 2;
            }
            else if (_level <= 4)
            {
                SpellsKnown = 3;
            }
            else if (_level <= 5)
            {
                SpellsKnown = 4;
            }
            else if (_level <= 6)
            {
                SpellsKnown = 5;
            }
            else if (_level <= 7)
            {
                SpellsKnown = 6;
            }
            else if (_level <= 8)
            {
                SpellsKnown = 7;
            }
            else if (_level <= 9)
            {
                SpellsKnown = 8;
            }
            else if (_level <= 10)
            {
                SpellsKnown = 9;
            }
            else if (_level <= 11)
            {
                SpellsKnown = 10;
            }
            else if (_level <= 14)
            {
                SpellsKnown = 11;
            }
            else if (_level <= 15)
            {
                SpellsKnown = 12;
            }
            else if (_level <= 17)
            {
                SpellsKnown = 14;
            }
            else // level 18-20
            {
                SpellsKnown = 15;
            }
        }

        // ==================== Cantrip Progression (Sacred Bond) ====================

        private void UpdateCantripsKnown()
        {
            if (_level >= 11)
            {
                CantripsKnown = 3;
            }
            else if (_level >= 4)
            {
                CantripsKnown = 2;
            }
            else
            {
                CantripsKnown = 0;
            }
        }

        // ==================== Paladin Spell Lists ====================

        private static readonly List<string> PaladinCantrips = new()
        {
            "Light", "Sacred Flame", "Guidance", "Thaumaturgy", "Protection from Evil and Good",
            "Sense Motive", "Purify Food and Drink", "Resistance"
        };

        private static readonly Dictionary<int, List<string>> PaladinSpells = new()
        {
            [1] = new()
            {
                "Bane", "Bless", "Command", "Compelled Duel", "Cure Wounds",
                "Detect Evil and Good", "Heroism", "Protective Word", "Purify Food and Drink",
                "Searing Smite", "Shield of Faith", "Thundering Smite", "Warding Bond"
            },
            [2] = new()
            {
                "Aura of Vitality", "Blindness/Deafness", "Branding Smite", "Cloud of Daggers",
                "Continual Flame", "Crusader's Mantle", "Magic Weapon", "Phantasmal Force",
                "Protection from Poison", "Scorching Ray", "Seekers", "Spiritual Weapon", "Sweet Pursuit"
            },
            [3] = new()
            {
                "Alacrity", "Aura of Life", "Aura of Purity", "Bestow Curse", "Clarity of Mind",
                "Daylight", "Dispel Magic", "Fear", "Flame Strike", "Haste", "Meld into Stone",
                "Plant Growth", "Protection from Energy", "Revivify", "Speak with Dead",
                "Spirit Guardians", "Tongues", "Vampiric Touch"
            },
            [4] = new()
            {
                "Banishment", "Blight", "Control Water", "Divine Favor", "Freedom of Movement",
                "Guardian of Faith", "Lesser Geas", "Mordenkainen's Sword", "Stoneskin"
            },
            [5] = new()
            {
                "Commune", "Contagion", "Destructive Wave", "Dispel Evil and Good", "Hold Monster",
                "Legend Lore", "Mass Cure Wounds", "Planar Binding", "Raise Dead", "Scrying"
            }
        };

        // ==================== Core Methods ====================

        public override void ClassSpecificAbility()
        {
            DivineSmite();
        }

        /// <summary>
        /// Divine Sense: As an action, determine which direction is north, south, east, or west.
        /// Determine if there is a celestial, fiend, or undead nearby within 60 feet.
        /// </summary>
        public virtual void DivineSense()
        {
            if (!_divineSenseUnlocked)
            {
                Console.WriteLine($"{Name} has not unlocked Divine Sense yet.");
                return;
            }

            Console.WriteLine($"{Name} uses Divine Sense and detects the presence of celestials, fiends, or undead within 60 feet.");
        }

        /// <summary>
        /// Divine Smite: As part of your attack against a creature, you can deal extra radiant damage.
        /// The base damage is 2d8, plus 1d8 for every level of spell slot used above 1st.
        /// At 5th level, you can smite twice per short rest.
        /// </summary>
        public virtual void DivineSmite(int targetLevel = 0, int spellSlotLevel = 1)
        {
            if (targetLevel <= 0)
            {
                Console.WriteLine($"{Name} uses Divine Smite on their next successful melee attack!");
                return;
            }

            if (spellSlotLevel < 1 || !RemainingSpellSlots.ContainsKey(spellSlotLevel) || RemainingSpellSlots[spellSlotLevel] <= 0)
            {
                Console.WriteLine($"{Name} doesn't have a spell slot of level {spellSlotLevel} remaining.");
                return;
            }

            RemainingSpellSlots[spellSlotLevel]--;

            // Base damage: 2d8 for 1st level, +1d8 per level above
            int baseDamage = 2 + (spellSlotLevel - 1);
            int totalDamage = baseDamage * 8; // Simplified: just report the dice count

            Console.WriteLine($"{Name} uses Divine Smite! The target takes {baseDamage}d8 radiant damage ({totalDamage} average)! " +
                $"({RemainingSpellSlots[spellSlotLevel]} slot of level {spellSlotLevel} remaining)");
        }

        /// <summary>
        /// ExpendSpellSlotForSmite: Uses a spell slot to deliver a Divine Smite with damage calculation.
        /// </summary>
        public virtual bool ExpendSlotForSmite(string targetName, int spellSlotLevel = 1)
        {
            if (spellSlotLevel < 1 || !RemainingSpellSlots.ContainsKey(spellSlotLevel) || RemainingSpellSlots[spellSlotLevel] <= 0)
            {
                Console.WriteLine($"{Name} doesn't have a spell slot of level {spellSlotLevel} remaining.");
                return false;
            }

            RemainingSpellSlots[spellSlotLevel]--;
            int baseDamage = 2 + (spellSlotLevel - 1);
            Console.WriteLine($"{Name} delivers a Divine Smite to {targetName}! " +
                $"Extra radiant damage: {baseDamage}d8 ({baseDamage * 8} average damage)! " +
                $"({RemainingSpellSlots[spellSlotLevel]} slot of level {spellSlotLevel} remaining)");
            return true;
        }

        /// <summary>
        /// Lay on Hands: Touch a creature to heal them using your divine pool.
        /// Maximum pool = paladin level x 5.
        /// </summary>
        public virtual void LayOnHands(Character target, int amount)
        {
            if (target == null)
            {
                Console.WriteLine("No valid target to heal.");
                return;
            }

            if (!_layOnHandsEnabled)
            {
                Console.WriteLine($"{Name} has not unlocked Lay on Hands yet (requires level 2).");
                return;
            }

            if (RemainingLayOnHands <= 0)
            {
                Console.WriteLine($"{Name} has no Lay on Hands points remaining. Take a long rest to recover.");
                return;
            }

            int maxPossibleHeal = target.MaxHitPoints - target.HitPoints;
            int actualHeal = amount;
            if (RemainingLayOnHands < actualHeal) actualHeal = RemainingLayOnHands;
            if (maxPossibleHeal < actualHeal) actualHeal = maxPossibleHeal;

            if (actualHeal <= 0)
            {
                Console.WriteLine($"{target.Name} is already at full health.");
                return;
            }

            RemainingLayOnHands -= actualHeal;
            target.Heal(actualHeal);
            Console.WriteLine($"{Name} uses Lay on Hands on {target.Name} for {actualHeal} hit points. Remaining pool: {RemainingLayOnHands}/{LayOnHandsPool}");
        }

        /// <summary>
        /// Sacred Weapon: At 2nd level, as a bonus action, add your Charisma modifier to attack rolls.
        /// </summary>
        public virtual bool SacredWeapon()
        {
            if (_level < 2)
            {
                Console.WriteLine($"{Name} has not unlocked Sacred Weapon yet (requires Oath of Devotion at level 2).");
                return false;
            }

            if (ChosenOath != PaladinOath.Devotion)
            {
                Console.WriteLine($"{Name} is not an Oath of Devotion paladin and cannot use Sacred Weapon.");
                return false;
            }

            int chaMod = GetAbilityModifier(_charisma);
            Console.WriteLine($"{Name} uses Sacred Weapon! Next attack roll gets +{chaMod} bonus (from Charisma)!");
            return true;
        }

        /// <summary>
        /// Blessing of the Immortals: Oath of Glory feature - gain temporary HP and movement boost.
        /// </summary>
        public virtual void BlessingOfTheImmortals()
        {
            if (ChosenOath != PaladinOath.Glory)
            {
                Console.WriteLine($"{Name} is not an Oath of Glory paladin.");
                return;
            }

            int tempHP = GetAbilityModifier(_charisma) + _level;
            Console.WriteLine($"{Name} receives Blessing of the Immortals! Gains {tempHP} temporary hit points and increased speed!");
        }

        /// <summary>
        /// Conquering Presence: Oath of Conquest feature - intimidate a creature within 30ft.
        /// </summary>
        public virtual bool ConqueringPresence(Character target)
        {
            if (ChosenOath != PaladinOath.Conquest || target == null)
            {
                Console.WriteLine($"{Name} cannot use Conquering Presence on this target.");
                return false;
            }

            int saveDC = SpellSaveDC;
            Console.WriteLine($"{Name} uses Conquering Presence! {target.Name} must make a Wisdom save (DC {saveDC}) or be frightened for 1 minute!");
            return true;
        }

        /// <summary>
        /// Vow of Enmity: Oath of Vengeance feature - gain advantage on attacks against one creature.
        /// </summary>
        public virtual bool VowOfEnmity(Character target)
        {
            if (ChosenOath != PaladinOath.Vengeance || target == null)
            {
                Console.WriteLine($"{Name} cannot use Vow of Enmity on this target.");
                return false;
            }

            Console.WriteLine($"{Name} uses Vow of Enmity against {target.Name}! Advantage on attack rolls until target is defeated or rested!");
            return true;
        }

        /// <summary>
        /// Aura of Warding: Oath of Redemption feature - allies within 10ft get +4 AC vs spell attacks.
        /// </summary>
        public virtual int GetAuraACBonus()
        {
            if (ChosenOath == PaladinOath.Redemption && _level >= 6)
            {
                return 4;
            }
            if (ChosenOath == PaladinOath.Ancients && _level >= 2)
            {
                return 1; // Passive resistance to spell damage
            }
            return 0;
        }

        /// <summary>
        /// Protective Spirit: Oath of the Ancients feature - heal an ally as a bonus action.
        /// </summary>
        public virtual void ProtectiveSpirit(Character target)
        {
            if (ChosenOath != PaladinOath.Ancients || target == null)
            {
                Console.WriteLine($"{Name} is not an Oath of the Ancients paladin.");
                return;
            }

            Random rand = new Random();
            int healAmount = rand.Next(1, 5) + _level;
            target.Heal(healAmount);
            Console.WriteLine($"{Name} uses Protective Spirit on {target.Name}! Heals for {healAmount} hit points!");
        }

        /// <summary>
        /// Check if a spell is on the paladin spell list.
        /// </summary>
        private bool IsPaladinSpell(SpellClass spell)
        {
            if (spell == null) return false;

            // Check cantrips
            foreach (var cantrip in PaladinCantrips)
            {
                if (cantrip.Equals(spell.Name, StringComparison.OrdinalIgnoreCase))
                    return true;
            }

            // Check spell levels
            foreach (var spellList in PaladinSpells.Values)
            {
                foreach (var s in spellList)
                {
                    if (s.Equals(spell.Name, StringComparison.OrdinalIgnoreCase))
                        return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Learn a paladin spell. All paladin spells are always prepared.
        /// </summary>
        public virtual void LearnSpell(SpellClass spell)
        {
            if (spell == null)
            {
                Console.WriteLine("Cannot learn a null spell.");
                return;
            }

            if (!_layOnHandsEnabled)
            {
                Console.WriteLine($"{Name} has not unlocked spellcasting yet (requires level 2).");
                return;
            }

            if (!IsPaladinSpell(spell))
            {
                Console.WriteLine($"{spell.Name} is not on the paladin spell list.");
                return;
            }

            // Check cantrip
            if (spell.Level == 0)
            {
                if (!KnownCantrips.Any(s => s.Name.Equals(spell.Name, StringComparison.OrdinalIgnoreCase)))
                {
                    if (KnownCantrips.Count >= CantripsKnown)
                    {
                        Console.WriteLine($"{Name} has reached the maximum number of known cantrips.");
                        return;
                    }
                    KnownCantrips.Add(spell);
                    Console.WriteLine($"{Name} has learned the cantrip: {spell.Name}");
                }
                return;
            }

            // Check spell slot level availability
            if (spell.Level > GetMaxAvailableSpellLevel())
            {
                Console.WriteLine($"{Name} cannot learn {spell.Name} yet (requires paladin level {_level + 2}).");
                return;
            }

            if (KnownSpells.Count >= SpellsKnown)
            {
                Console.WriteLine($"{Name} has reached the maximum number of known spells.");
                return;
            }

            KnownSpells.Add(spell);
            Console.WriteLine($"{Name} has learned the spell: {spell.Name} (Level {spell.Level})");
        }

        /// <summary>
        /// Cast a known paladin spell if slots are available.
        /// </summary>
        public virtual bool CastSpell(string spellName, int spellLevel = 0)
        {
            SpellClass spellToCast = KnownSpells.FirstOrDefault(s => s.Name.Equals(spellName, StringComparison.OrdinalIgnoreCase));

            // Try cantrips
            if (spellToCast == null && (spellLevel == 0))
            {
                spellToCast = KnownCantrips.FirstOrDefault(s => s.Name.Equals(spellName, StringComparison.OrdinalIgnoreCase));
                if (spellToCast != null)
                {
                    Console.WriteLine($"{Name} casts the cantrip {spellName}.");
                    spellToCast.Cast();
                    return true;
                }
                Console.WriteLine($"{Name} does not know the cantrip: {spellName}.");
                return false;
            }

            if (spellToCast == null)
            {
                Console.WriteLine($"{Name} does not have {spellName} known.");
                return false;
            }

            // Check if cantrip (level 0)
            if (spellLevel == 0 || spellToCast.Level == 0)
            {
                Console.WriteLine($"{Name} casts the cantrip {spellName}.");
                spellToCast.Cast();
                return true;
            }

            // Check spell slot availability
            if (!RemainingSpellSlots.ContainsKey(spellLevel) || RemainingSpellSlots[spellLevel] <= 0)
            {
                Console.WriteLine($"{Name} has no {spellLevel}-level spell slots remaining.");
                return false;
            }

            // Check spell level validity
            if (spellLevel < spellToCast.Level)
            {
                Console.WriteLine($"{Name} needs at least a level {spellToCast.Level} slot for {spellName}.");
                return false;
            }

            RemainingSpellSlots[spellLevel]--;
            Console.WriteLine($"{Name} casts {spellName} using a {spellLevel}-level spell slot. Slots remaining: {RemainingSpellSlots[spellLevel]}");
            spellToCast.Cast();
            return true;
        }

        /// <summary>
        /// Get the maximum spell level available to the paladin based on their half-caster progression.
        /// </summary>
        private int GetMaxAvailableSpellLevel()
        {
            if (_level < 5) return 0;
            if (_level < 7) return 2;
            if (_level < 9) return 3;
            if (_level < 11) return 4;
            if (_level < 15) return 5;
            if (_level < 17) return 6;
            return 9; // Theoretically possible via multiclassing, but we cap at paladin progression
        }

        /// <summary>
        /// Get the highest spell slot level available to this paladin.
        /// </summary>
        public int GetHighestSpellSlotLevel()
        {
            for (int i = 9; i >= 1; i--)
            {
                if (SpellSlotsPerLevel.ContainsKey(i) && SpellSlotsPerLevel[i] > 0)
                    return i;
            }
            return 0;
        }

        /// <summary>
        /// Oath Ability: Channel Divinity-like ability for the chosen oath.
        /// </summary>
        public virtual bool UseOathAbility()
        {
            if (OathAbilityUses <= 0)
            {
                Console.WriteLine($"{Name} has no oath ability uses remaining. Take a long rest to recover.");
                return false;
            }

            OathAbilityUses--;
            Console.WriteLine($"{Name} uses their oath ability! Uses remaining: {OathAbilityUses}/{MaxOathAbilityUses}");

            // Domain-specific oath ability effect
            switch (ChosenOath)
            {
                case PaladinOath.Devotion:
                    Console.WriteLine("Blessed Warrior: You and allies within 10 feet have advantage on saving throws against spells for 1 minute.");
                    break;
                case PaladinOath.Redemption:
                    Console.WriteLine("Emerald Shield: A shimmering emerald force surrounds you. Creatures within 10 feet must succeed on a DC spell save or be pushed back.");
                    break;
                case PaladinOath.Vengeance:
                    Console.WriteLine("Nemesis's Sight: You can see through illusions and gain advantage on wisdom saves until your next turn.");
                    break;
                case PaladinOath.Ancients:
                    Console.WriteLine("Embrace the Moon: You transform into moonlight and teleport up to 60 feet to an unoccupied space.");
                    break;
                case PaladinOath.Conquest:
                    Console.WriteLine("Rallying Cry: Each ally within 30 feet regains hit points equal to your paladin level + charisma modifier.");
                    break;
                case PaladinOath.Salvation:
                    Console.WriteLine("Mark of Protection: Choose a creature within 5 feet. Until the end of your next turn, that creature has a +5 bonus to AC.");
                    break;
                case PaladinOath.Glory:
                    Console.WriteLine("Crown of Glory: Radiant crown manifests. Each hostile creature within 30 feet must make a charisma save or be frightened.");
                    break;
                case PaladinOath.Watchers:
                    Console.WriteLine("Dimensional Anchor: Until the end of your next turn, the area around you is slightly out of phase with the material plane.");
                    break;
                case PaladinOath.Dreams:
                    Console.WriteLine("Fey Step: You and up to 8 creatures within 30 feet teleport to a unoccupied space within 60 feet.");
                    break;
            }
            return true;
        }

        /// <summary>
        /// Recover oath ability uses on a long rest.
        /// </summary>
        public virtual void RecoverOathAbilities()
        {
            OathAbilityUses = MaxOathAbilityUses;
            Console.WriteLine($"{Name} recovers all {MaxOathAbilityUses} oath ability uses after a long rest.");
        }

        // ==================== Aura Methods ====================

        /// <summary>
        /// Aura of Protection (level 6+): You and friendly creatures within 30 feet gain +CHA modifier to saving throws.
        /// For Oath of Salvation, this is available at level 6 with a wider radius.
        /// </summary>
        public virtual int GetAuraSavingThrowBonus()
        {
            if (_level < 6) return 0;

            int chaMod = GetAbilityModifier(_charisma);
            int radiusBonus = (ChosenOath == PaladinOath.Salvation || ChosenOath == PaladinOath.Ancients) ? 1 : 0;

            Console.WriteLine($"{Name}'s Aura of Protection grants +{chaMod + radiusBonus} to saving throws for allies within {(ChosenOath == PaladinOath.Salvation ? 20 : 30)} feet.");
            return chaMod + radiusBonus;
        }

        /// <summary>
        /// Aura of Valor (level 10): Allies within 10 feet have advantage on saving throws against being frightened.
        /// </summary>
        public virtual bool HasAuraOfValor() => _auraOfValorEnabled && _level >= 10;

        // ==================== Improved Divine Smite ====================

        /// <summary>
        /// Improved Divine Smite (level 15): All melee weapon attacks deal an additional 1d8 radiant damage.
        /// </summary>
        public virtual int GetImprovedDivineSmiteBonus()
        {
            if (_improvedPilotEnabled)
            {
                return 5; // Average of 1d8 is 4.5, rounded to 5 for simplicity
            }
            return 0;
        }

        /// <summary>
        /// Check if this paladin has Improved Divine Smite.
        /// </summary>
        public bool HasImprovedDivineSmite() => _improvedPilotEnabled;

        // ==================== Stat Calculations Override ====================

        protected override void CalculateBaseStats()
        {
            int conMod = GetAbilityModifier(_constitution);

            // Paladin hit die is d10
            int hpFromFirstLevel = 10 + conMod;
            int hpFromHigherLevels = (_level - 1) * (5 + conMod); // Half-caster typically gets fewer HP
            MaxHitPoints = hpFromFirstLevel + hpFromHigherLevels;

            if (HitPoints > MaxHitPoints || HitPoints <= 0)
            {
                HitPoints = MaxHitPoints;
            }

            // Update Lay on Hands pool
            RemainingLayOnHands = Math.Max(RemainingLayOnHands, LayOnHandsPool);
        }

        // ==================== Level Feature Progression Helper ====================

        private void ApplyDomainLevel2() { /* No longer used - functionality moved to ApplyOathLevel2 */ }
        private void ApplyDomainLevel6() { /* No longer used - functionality moved to ApplyOathLevel6 */ }
        private void ApplyDomainLevel8() { /* No longer used */ }
        private void ApplyDomainLevel17() { /* No longer used - functionality moved to ApplyOathLevel17 */ }

        // ==================== Override Base Methods ====================

        public override void Attack()
        {
            int attackCount = _improvedPilotEnabled ? 2 : (_level >= 5 ? 2 : 1);
            string plural = attackCount > 1 ? "s" : "";

            Console.WriteLine($"{Name} attacks with their weapon!");

            if (FightingStyle == PaladinFightingStyle.Dueling)
            {
                Console.WriteLine($"Dueling fighting style: +2 damage with one-handed weapons.");
            }
            else if (FightingStyle == PaladinFightingStyle.GreatWeaponFighting)
            {
                Console.WriteLine("Great Weapon Fighting: You can reroll 1s and 2s on weapon damage dice.");
            }
            else if (FightingStyle == PaladinFightingStyle.Protection)
            {
                Console.WriteLine("Protection fighting style: -2 to attack rolls against targets you are fighting alongside.");
            }

            Console.WriteLine($"{Name} makes {attackCount} attack{plural}.");

            if (_improvedPilotEnabled)
            {
                Console.WriteLine("Improved Divine Smite: All melee attacks deal an extra 1d8 radiant damage!");
            }
        }

        public override void TakeDamage(int damage)
        {
            base.TakeDamage(damage);
        }

        public override void Heal(int amount)
        {
            base.Heal(amount);
        }

        public override void LevelUp()
        {
            int previousLevel = _level;
            base.LevelUp();

            Console.WriteLine($"{_name} has reached level {_level}!");

            ApplyLevelFeatures();

            // Announce key feature unlocks
            if (_level == 2 && ChosenOath != PaladinOath.Devotion)
            {
                Console.WriteLine($"New class features: Lay on Hands, Paladin spellcasting, Smite!");
            }
            else if (_level == 6)
            {
                Console.WriteLine("Aura of Protection gained! Allies within 30 feet gain +CHA to saving throws!");
            }
            else if (_level == 10)
            {
                Console.WriteLine("Aura of Valor gained! Allies within 10 feet have advantage on saves vs frightened!");
            }
            else if (_level == 11)
            {
                Console.WriteLine("Ceaseless Divinity! You can't be charmed or frightened, and regain 5 HP at the start of each turn!");
            }
            else if (_level == 15)
            {
                Console.WriteLine("Improved Divine Smite! All your melee attacks deal an extra 1d8 radiant damage!");
            }
            else if (_level == 20)
            {
                Console.WriteLine($"Oath of {ChosenOath} capstone feature gained!");
            }
        }

        public override void LongRest()
        {
            base.LongRest();

            // Recover Lay on Hands pool (refreshed to max)
            RemainingLayOnHands = LayOnHandsPool;

            // Recover oath ability uses
            OathAbilityUses = MaxOathAbilityUses;

            // Reset spell slots
            foreach (var slot in SpellSlotsPerLevel)
            {
                RemainingSpellSlots[slot.Key] = slot.Value;
            }

            Console.WriteLine($"{Name} recovers all hit points, Lay on Hands ({RemainingLayOnHands}/{LayOnHandsPool}), and oath ability uses after a long rest.");
        }

        public override void ShortRest()
        {
            base.ShortRest();

            // Paladins recover some spell slots on short rest (same as full casters per recent errata)
            // For simplicity, we'll only restore lower-level slots
            if (_level >= 4)
            {
                int slotsToRecover = Math.Min(1, SpellSlotsPerLevel.ContainsKey(1) ? SpellSlotsPerLevel[1] : 0);
                if (slotsToRecover > 0 && RemainingSpellSlots.ContainsKey(1))
                {
                    // Only recover if not already at full
                    if (RemainingSpellSlots[1] < SpellSlotsPerLevel[1])
                    {
                        int recovered = Math.Min(slotsToRecover, SpellSlotsPerLevel[1] - RemainingSpellSlots[1]);
                        RemainingSpellSlots[1] += recovered;
                        Console.WriteLine($"{Name} recovers {recovered} level 1 spell slot(s) on a short rest.");
                    }
                }
            }

            Console.WriteLine($"{Name} recovers some resources after a short rest.");
        }

        public override void DisplayCharacter()
        {
            string oathPrefix = GetOathName(ChosenOath);

            Console.WriteLine();
            Console.WriteLine($"=== {Name} (Level {_level} Paladin - {oathPrefix}) ===");
            Console.WriteLine($"Hit Points: {HitPoints}/{MaxHitPoints} | AC: {ArmorClass} | Speed: {Speed}");
            Console.WriteLine($"Fighting Style: {FightingStyle}");
            Console.WriteLine();

            // Spellcasting info
            if (_level >= 2)
            {
                Console.WriteLine("--- Paladin Spellcasting ---");
                Console.WriteLine($"Spellcasting Ability: Charisma (Modifier +{GetAbilityModifier(_charisma)})");
                Console.WriteLine($"Spell Save DC: {SpellSaveDC}");
                Console.WriteLine($"Spell Attack Modifier: +{SpellAttackModifier}");
                Console.WriteLine($"Spells Known: {KnownSpells.Count}/{SpellsKnown}");
                Console.WriteLine($"Cantrips Known: {CantripsKnown}");
                Console.WriteLine("Spell Slots:");
                foreach (var slot in SpellSlotsPerLevel)
                {
                    if (slot.Value > 0)
                    {
                        Console.WriteLine($"  {slot.Key}-level: {RemainingSpellSlots[slot.Key]}/{slot.Value}");
                    }
                }

                // Known spells list
                if (KnownSpells.Count > 0)
                {
                    Console.WriteLine("Known Spells: " + string.Join(", ", KnownSpells.Select(s => s.Name)));
                }
                if (KnownCantrips.Count > 0)
                {
                    Console.WriteLine("Known Cantrips: " + string.Join(", ", KnownCantrips.Select(s => s.Name)));
                }
            }

            // Lay on Hands
            Console.WriteLine();
            Console.WriteLine("--- Lay on Hands ---");
            Console.WriteLine($"Pool: {RemainingLayOnHands}/{LayOnHandsPool} (Level x 5)");

            // Oath features
            Console.WriteLine();
            Console.WriteLine("--- Sacred Oath Features ---");
            switch (ChosenOath)
            {
                case PaladinOath.Devotion:
                    Console.WriteLine("Oath of Devotion:");
                    if (_fearlessEnabled) Console.WriteLine("  - Nimbus of the Crusader (level 7)");
                    if (_pureKnightEnabled) Console.WriteLine("  - Pure Knight (level 7)");
                    if (_auraOfValorEnabled) Console.WriteLine("  - Aura of Valor (level 10)");
                    if (_improvedPilotEnabled) Console.WriteLine("  - Improved Divine Smite (level 15)");
                    if (_meteorStrikeEnabled) Console.WriteLine("  - Meteor Strike (level 20 capstone)");
                    break;
                case PaladinOath.Redemption:
                    Console.WriteLine("Oath of Redemption:");
                    if (_sanctuaryOfLifeEnabled) Console.WriteLine("  - Sanctuary of Life (level 7)");
                    if (_redeemingTouchEnabled) Console.WriteLine("  - Redeeming Touch (level 7)");
                    if (_auraOfValorEnabled) Console.WriteLine("  - Aura of Valor (level 10)");
                    if (_improvedPilotEnabled) Console.WriteLine("  - Improved Divine Smite (level 15)");
                    if (_turningPointEnabled) Console.WriteLine("  - Turning Point (level 20 capstone)");
                    break;
                case PaladinOath.Vengeance:
                    Console.WriteLine("Oath of Vengeance:");
                    if (_nemesisEnabled) Console.WriteLine("  - Nemesis (level 7)");
                    if (_abjureEnmityEnabled) Console.WriteLine("  - Abjure Enmity (level 7)");
                    if (_auraOfValorEnabled) Console.WriteLine("  - Aura of Valor (level 10)");
                    if (_improvedPilotEnabled) Console.WriteLine("  - Improved Divine Smite (level 15)");
                    if (_empireOfMayhemEnabled) Console.WriteLine("  - Empire of Mayhem (level 20 capstone)");
                    break;
                case PaladinOath.Ancients:
                    Console.WriteLine("Oath of the Ancients:");
                    if (_protectiveSpiritEnabled) Console.WriteLine("  - Protective Spirit (level 7)");
                    if (_natureWrathEnabled) Console.WriteLine("  - Nature's Wrath (level 7)");
                    if (_auraOfValorEnabled) Console.WriteLine("  - Aura of Valor (level 10)");
                    if (_improvedPilotEnabled) Console.WriteLine("  - Improved Divine Smite (level 15)");
                    if (_guardianOfLifeEnabled) Console.WriteLine("  - Guardian of Life (level 20 capstone)");
                    break;
                case PaladinOath.Conquest:
                    Console.WriteLine("Oath of Conquest:");
                    if (_conqueringPresenceEnabled) Console.WriteLine("  - Conquering Presence (level 7)");
                    if (_shieldOfTheLiegeEnabled) Console.WriteLine("  - Shield of the Liege (level 7)");
                    if (_auraOfValorEnabled) Console.WriteLine("  - Aura of Valor (level 10)");
                    if (_improvedPilotEnabled) Console.WriteLine("  - Improved Divine Smite (level 15)");
                    if (_dominatingPresenceEnabled) Console.WriteLine("  - Dominating Presence (level 20 capstone)");
                    break;
                case PaladinOath.Salvation:
                    Console.WriteLine("Oath of Salvation:");
                    if (_savedFromTheGraveEnabled) Console.WriteLine("  - Saved From The Grave (level 7)");
                    if (_auralAdvancementEnabled) Console.WriteLine("  - Aural Advancement (level 7)");
                    if (_auraOfValorEnabled) Console.WriteLine("  - Aura of Valor (level 10)");
                    if (_improvedPilotEnabled) Console.WriteLine("  - Improved Divine Smite (level 15)");
                    if (_unyieldingAuraEnabled) Console.WriteLine("  - Unyielding Aura (level 20 capstone)");
                    break;
                case PaladinOath.Glory:
                    Console.WriteLine("Oath of Glory:");
                    if (_swiftFootedEnabled) Console.WriteLine("  - Swift Footed (level 7)");
                    if (_forcedMovementEnabled) Console.WriteLine("  - Forced Movement (level 7)");
                    if (_auraOfValorEnabled) Console.WriteLine("  - Aura of Valor (level 10)");
                    if (_improvedPilotEnabled) Console.WriteLine("  - Improved Divine Smite (level 15)");
                    if (_gloriousChargeEnabled) Console.WriteLine("  - Glorious Charge (level 20 capstone)");
                    break;
                case PaladinOath.Watchers:
                    Console.WriteLine("Oath of the Watchers:");
                    if (_awakenedHazardEnabled) Console.WriteLine("  - Awakened Hazard (level 7)");
                    if (_planarAwarenessEnabled) Console.WriteLine("  - Planar Awareness (level 7)");
                    if (_auraOfValorEnabled) Console.WriteLine("  - Aura of Valor (level 10)");
                    if (_improvedPilotEnabled) Console.WriteLine("  - Improved Divine Smite (level 15)");
                    if (_dimensionalAnchorEnabled) Console.WriteLine("  - Dimensional Anchor (level 20 capstone)");
                    break;
                case PaladinOath.Dreams:
                    Console.WriteLine("Oath of Dreams:");
                    if (_wakeningEnhancementEnabled) Console.WriteLine("  - Wakening Enhancement (level 7)");
                    if (_sleepingBeautyEnabled) Console.WriteLine("  - Sleeping Beauty (level 7)");
                    if (_auraOfValorEnabled) Console.WriteLine("  - Aura of Valor (level 10)");
                    if (_improvedPilotEnabled) Console.WriteLine("  - Improved Divine Smite (level 15)");
                    if (_dreamWalkerEnabled) Console.WriteLine("  - Dream Walker (level 20 capstone)");
                    break;
            }

            // Oath ability info
            if (_level >= 2)
            {
                Console.WriteLine();
                Console.WriteLine($"Channel Divinity-like: {OathAbilityUses}/{MaxOathAbilityUses} uses");
            }

            Console.WriteLine();
            Console.WriteLine("Ability Scores:");
            Console.WriteLine($"  Strength:    {Strength} (mod +{GetAbilityModifier(Strength)})");
            Console.WriteLine($"  Dexterity:   {Dexterity} (mod +{GetAbilityModifier(Dexterity)})");
            Console.WriteLine($"  Constitution:{Constitution} (mod +{GetAbilityModifier(Constitution)})");
            Console.WriteLine($"  Intelligence:{Intelligence} (mod +{GetAbilityModifier(Intelligence)})");
            Console.WriteLine($"  Wisdom:      {Wisdom} (mod +{GetAbilityModifier(Wisdom)})");
            Console.WriteLine($"  Charisma:    {Charisma} (mod +{GetAbilityModifier(Charisma)})");

            Console.WriteLine();
            Console.WriteLine("=== End Character Sheet ===");
            Console.WriteLine();
        }

        // ==================== Helper Methods ====================

        private string GetOathName(PaladinOath oath)
        {
            switch (oath)
            {
                case PaladinOath.Devotion: return "Oath of Devotion";
                case PaladinOath.Redemption: return "Oath of Redemption";
                case PaladinOath.Vengeance: return "Oath of Vengeance";
                case PaladinOath.Ancients: return "Oath of the Ancients";
                case PaladinOath.Conquest: return "Oath of Conquest";
                case PaladinOath.Salvation: return "Oath of Salvation";
                case PaladinOath.Glory: return "Oath of Glory";
                case PaladinOath.Watchers: return "Oath of the Watchers";
                case PaladinOath.Dreams: return "Oath of Dreams";
                default: return "Unknown Oath";
            }
        }

        // Remove unused domain methods
        private void ApplyLevelFeatures_Old() { }
    }
}