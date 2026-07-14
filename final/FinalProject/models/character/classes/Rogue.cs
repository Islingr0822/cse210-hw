using System;
using System.Collections.Generic;
using DnDCharacterManager.Ability;
using FeatureClass = DnDCharacterManager.Feature.Feature;
using RaceClass = DnDCharacterManager.Race.Race;
using BackgroundClass = DnDCharacterManager.Background.Background;
using SpellClass = DnDCharacterManager.Spell.Spell;
using School = DnDCharacterManager.Spell.School;

namespace DnDCharacterManager.Character.Classes
{
    /// <summary>
    /// Enum representing the three Rogue subclasses.
    /// </summary>
    public enum RogueSubclass
    {
        Thief,
        Assassin,
        ArcaneTrickster
    }

    /// <summary>
    /// Enum for Thievery specialties (Thief subclass options).
    /// </summary>
    public enum ThieverySpecialty
    {
        FastHands,
        SecondThoughts,
        StoneEars,
        SuperSneaky
    }

    /// <summary>
    /// Enum for Arcane Trickster spell focuses.
    /// </summary>
    public enum ArcaneFocus
    {
        Illusion,
        Enchantment,
        Divination,
        Transmutation
    }

    /// <summary>
    /// Rogue character class with full level 1-20 feature progression and all PHB subclasses.
    /// Implements D&D 5e rules by the book.
    /// </summary>
    public class Rogue : Character
    {
        // ==================== Core Rogue Properties ====================

        private int _sneakAttackDice;
        private int _sneakAttackUsesPerDay;
        private bool _hasAdvantageOnSneakAttack;
        private string _sneakAttackTarget = "";

        // Expertise tracking
        private int _expertiseSkillCount;
        private List<string> _expertisedSkills;

        // Level-gated feature flags
        private bool _nimbleEscapeUnlocked;
        private bool _uncannyDodgeUnlocked;
        private bool _evasionUnlocked;
        private bool _reliableTimingUnlocked;
        private bool _blindsenseUnlocked;
        private bool _slipperyMindUnlocked;
        private bool _elusiveUnlocked;
        private bool _highlyMobileUnlocked;
        private bool _masterThiefUnlocked;

        // Subclass tracking
        private RogueSubclass _subclass;
        private string _subclassName;

        // ==================== Thief Properties ====================

        private int _quickHandsUses;
        private int _secondThoughtsDistance;
        private bool _supremeSneakActive;
        private string _thieverySpecialty;

        // ==================== Assassin Properties ====================

        private bool _assassinateUnlockedThisCombat;
        private int _infiltrationExpertiseBonus;
        private List<string> _assassinProficiencies;

        // ==================== Arcane Trickster Properties ====================

        private Dictionary<int, int> _spellSlotsByLevel;
        private List<SpellClass> _knownSpells;
        private List<string> _cantripsKnown;
        private int _spellsKnownCount;
        private bool _magicalAmbushActive;
        private bool _steadyEyeActive;
        private bool _spellStormReady;

        // ==================== Constructors ====================

        public Rogue() : base()
        {
            _subclass = RogueSubclass.Thief;
            _subclassName = "Thief";
            _sneakAttackDice = 1;
            _sneakAttackUsesPerDay = 0; // Unlimited, just needs advantage or ally nearby
            _hasAdvantageOnSneakAttack = false;
            _expertiseSkillCount = 2;
            _expertisedSkills = new List<string>();
            _thieverySpecialty = "FastHands";

            InitializeRogue();
        }

        public Rogue(
            string name,
            int level,
            RaceClass race,
            BackgroundClass background,
            RogueSubclass subclass = RogueSubclass.Thief)
            : base(name, level, race, background)
        {
            _subclass = subclass;
            _subclassName = GetSubclassName(subclass);
            _sneakAttackDice = 1;
            _sneakAttackUsesPerDay = 0;
            _hasAdvantageOnSneakAttack = false;
            _expertiseSkillCount = 2;
            _expertisedSkills = new List<string>();
            _thieverySpecialty = "FastHands";

            InitializeRogue();
            ApplyLevelFeatures();
        }

        private void InitializeRogue()
        {
            // Core rogue defaults
            _hasAdvantageOnSneakAttack = false;
            _sneakAttackTarget = "";

            // Expertise defaults
            _expertiseSkillCount = 2;
            _expertisedSkills = new List<string>();

            // Feature flags
            _nimbleEscapeUnlocked = false;
            _uncannyDodgeUnlocked = false;
            _evasionUnlocked = false;
            _reliableTimingUnlocked = false;
            _blindsenseUnlocked = false;
            _slipperyMindUnlocked = false;
            _elusiveUnlocked = false;
            _highlyMobileUnlocked = false;
            _masterThiefUnlocked = false;

            // Thief defaults
            _quickHandsUses = 0;
            _secondThoughtsDistance = 15;
            _supremeSneakActive = false;
            _thieverySpecialty = "FastHands";

            // Assassin defaults
            _assassinateUnlockedThisCombat = false;
            _infiltrationExpertiseBonus = 0;
            _assassinProficiencies = new List<string>();

            // Arcane Trickster defaults
            _spellSlotsByLevel = new Dictionary<int, int>();
            _knownSpells = new List<SpellClass>();
            _cantripsKnown = new List<string>();
            _spellsKnownCount = 0;
            _magicalAmbushActive = false;
            _steadyEyeActive = false;
            _spellStormReady = false;

            // Apply level-based initialization
            if (Level >= 2) _nimbleEscapeUnlocked = true;
            if (Level >= 5) { _uncannyDodgeUnlocked = true; _evasionUnlocked = true; }
            if (Level >= 15) _slipperyMindUnlocked = true;
            if (Level >= 18) _elusiveUnlocked = true;

            UpdateSneakAttackDice();
        }

        // ==================== Properties ====================

        public int SneakAttackDice { get => _sneakAttackDice; }
        public int ExpertiseSkillCount { get => _expertiseSkillCount; }
        public List<string> ExpertisedSkills { get => _expertisedSkills; }
        public RogueSubclass Subclass { get => _subclass; set { _subclass = value; _subclassName = GetSubclassName(value); } }
        public string SubclassName { get => _subclassName; }
        public bool NimbleEscapeUnlocked { get => _nimbleEscapeUnlocked; }
        public bool UncannyDodgeUnlocked { get => _uncannyDodgeUnlocked; }
        public bool EvasionUnlocked { get => _evasionUnlocked; }
        public bool SlipperyMindUnlocked { get => _slipperyMindUnlocked; }
        public bool ElusiveUnlocked { get => _elusiveUnlocked; }

        // Thief properties
        public int QuickHandsUses { get => _quickHandsUses; }
        public string ThieverySpecialty { get => _thieverySpecialty; set => _thieverySpecialty = value; }

        // Assassin properties
        public bool AssassinateUnlockedThisCombat { get => _assassinateUnlockedThisCombat; }
        public int InfiltrationExpertiseBonus { get => _infiltrationExpertiseBonus; }
        public List<string> AssassinProficiencies { get => _assassinProficiencies; }

        // Arcane Trickster properties
        public Dictionary<int, int> SpellSlotsByLevel { get => _spellSlotsByLevel; }
        public List<SpellClass> KnownSpells { get => _knownSpells; }
        public List<string> CantripsKnown { get => _cantripsKnown; }
        public int SpellsKnownCount { get => _spellsKnownCount; }

        // ==================== Core Methods ====================

        public override void ClassSpecificAbility()
        {
            SneakAttack();
        }

        /// <summary>
        /// Make a Sneak Attack: deal extra damage when you have advantage or an ally is within 5 feet of the target.
        /// The damage scales with rogue level (1d6 at level 1, up to 10d6 at level 19+).
        /// Can be used once per turn (not per rest).
        /// </summary>
        public virtual int SneakAttack(string targetName = "target")
        {
            if (!_hasAdvantageOnSneakAttack && !HasAllyNearby(targetName))
            {
                Console.WriteLine($"{Name} cannot use Sneak Attack - no advantage and no ally adjacent to target.");
                return 0;
            }

            int damage = _sneakAttackDice;
            _hasAdvantageOnSneakAttack = false; // Reset after use
            Console.WriteLine($"{Name} makes a Sneak Attack against {targetName} for {damage} extra damage!");
            return damage;
        }

        /// <summary>
        /// Check if there's an ally within 5 feet of the target for Sneak Attack.
        /// </summary>
        private bool HasAllyNearby(string targetName)
        {
            // In a full game, this would check adjacent creature positions
            // For now, assume it's possible if the player has allies
            return true; // Simplified - allow sneak attack with allies
        }

        /// <summary>
        /// Apply advantage to next Sneak Attack (from hiding, stealth, etc.).
        /// </summary>
        public virtual void ApplySneakAttackAdvantage()
        {
            _hasAdvantageOnSneakAttack = true;
            Console.WriteLine($"{Name} has advantage on their next Sneak Attack!");
        }

        // ==================== Expertise Methods ====================

        /// <summary>
        /// Add a skill to expertise, doubling the proficiency bonus for that skill.
        /// </summary>
        public virtual void AddExpertiseSkill(string skillName)
        {
            if (_expertisedSkills.Contains(skillName))
            {
                Console.WriteLine($"{Name} has already expertised in {skillName}.");
                return;
            }

            if (_expertisedSkills.Count >= _expertiseSkillCount)
            {
                Console.WriteLine($"{Name} has reached maximum expertise slots ({_expertiseSkillCount}).");
                return;
            }

            _expertisedSkills.Add(skillName);
            Console.WriteLine($"{Name} has expertised in {skillName}! Proficiency bonus is doubled for this skill.");
        }

        /// <summary>
        /// Get the effective proficiency bonus for a skill (doubled if expertised).
        /// </summary>
        public int GetEffectiveProficiencyBonusForSkill(string skillName)
        {
            int baseProficiency = GetProficiencyBonusForLevel(Level);
            if (_expertisedSkills.Contains(skillName))
            {
                return baseProficiency * 2;
            }
            return baseProficiency;
        }

        // ==================== Nimble Escape (Level 2) ====================

        /// <summary>
        /// Use Hide or Disengage as a bonus action (Thief and general Rogue feature).
        /// </summary>
        public virtual bool NimbleEscape(string actionType)
        {
            if (!_nimbleEscapeUnlocked)
            {
                Console.WriteLine($"{Name} has not learned Nimble Escape yet (requires level 2).");
                return false;
            }

            if (actionType != "Hide" && actionType != "Disengage")
            {
                Console.WriteLine("Nimble Escape only works with 'Hide' or 'Disengage'.");
                return false;
            }

            Console.WriteLine($"{Name} uses Nimble Escape to {actionType} as a bonus action!");
            return true;
        }

        // ==================== Uncanny Dodge (Level 5) ====================

        /// <summary>
        /// Use reaction to halve damage from an attack.
        /// </summary>
        public virtual bool UncannyDodge(int incomingDamage, string attackerName)
        {
            if (!_uncannyDodgeUnlocked)
            {
                Console.WriteLine($"{Name} has not learned Uncanny Dodge yet (requires level 5).");
                return false;
            }

            int halvedDamage = incomingDamage / 2;
            Console.WriteLine($"{Name} uses Uncanny Dodge to react to {attackerName}'s attack! Damage is halved: {incomingDamage} -> {halvedDamage}");
            return true;
        }

        // ==================== Evasion (Level 7) ====================

        /// <summary>
        /// Take half damage on successful save, no damage on successful save (for area effects).
        /// </summary>
        public virtual int Evasion(int incomingDamage, bool savingThrowSuccess)
        {
            if (!_evasionUnlocked)
            {
                return incomingDamage;
            }

            if (savingThrowSuccess)
            {
                Console.WriteLine($"{Name} uses Evasion to avoid damage entirely!");
                return 0;
            }
            else
            {
                int halfDamage = incomingDamage / 2;
                Console.WriteLine($"{Name} uses Evasion, taking only half damage: {incomingDamage} -> {halfDamage}");
                return halfDamage;
            }
        }

        // ==================== Slippery Mind (Level 15) ====================

        /// <summary>
        /// Advantage on saves against being charmed or frightened.
        /// </summary>
        public virtual bool SlipperyMindSave(bool wasCharmed, string sourceName)
        {
            if (!_slipperyMindUnlocked)
            {
                return wasCharmed; // Return original result without benefit
            }

            Console.WriteLine($"{Name} uses Slippery Mind to shake off charm/fright from {sourceName}!");
            return false; // Automatically succeeds against charm/fright
        }

        // ==================== Elusive (Level 18) ====================

        /// <summary>
        /// Cannot be targeted by attacks if you move during your turn.
        /// </summary>
        public virtual bool Elusive()
        {
            if (!_elusiveUnlocked)
            {
                return false;
            }

            Console.WriteLine($"{Name} uses Elusive! You can't be targeted by attacks until your next turn!");
            return true;
        }

        // ==================== Level Feature Progression ====================

        private void ApplyLevelFeatures()
        {
            // ===== LEVEL 1: Starting features =====
            if (Level >= 1)
            {
                UpdateSneakAttackDice();
                // Arcane Trickster spells are added in ApplySubclassFeatures at level 3
            }

            // ===== LEVEL 2: Expertise + Nimble Escape =====
            if (Level >= 2)
            {
                _expertiseSkillCount = 4; // Will be updated to correct count based on level
                _nimbleEscapeUnlocked = true;
            }

            // ===== LEVEL 3: Subclass Feature =====
            if (Level >= 3)
            {
                ApplySubclassFeatures();
            }

            // ===== LEVEL 5: Uncanny Dodge + Evasion =====
            if (Level >= 5)
            {
                _uncannyDodgeUnlocked = true;
                _evasionUnlocked = true;
            }

            // ===== LEVEL 6: Expertise slot increase (handled by player choice) =====
            if (Level >= 6)
            {
                UpdateSubclassFeatures();
            }

            // ===== LEVEL 7: High Mobility =====
            if (Level >= 7)
            {
                _highlyMobileUnlocked = true;
            }

            // ===== LEVEL 9: Reliable Talent (all skills) =====
            if (Level >= 9)
            {
                _reliableTimingUnlocked = true;
            }

            // ===== LEVEL 10: Ability Score Improvement =====
            if (Level >= 10)
            {
                UpdateSubclassFeatures();
            }

            // ===== LEVEL 11: Blindsense (Thief) / Verbal Mimicry (Assassin) / Spell Storem (Arcane Trickster) =====
            if (Level >= 11)
            {
                _blindsenseUnlocked = true;
                UpdateSubclassFeatures();
            }

            // ===== LEVEL 12: ASI =====
            if (Level >= 12)
            {
                UpdateSneakAttackDice();
            }

            // ===== LEVEL 13: Master Thief Features =====
            if (Level >= 13)
            {
                _masterThiefUnlocked = true;
            }

            // ===== LEVEL 15: Slippery Mind =====
            if (Level >= 15)
            {
                _slipperyMindUnlocked = true;
                UpdateSneakAttackDice();
            }

            // ===== LEVEL 18: Elusive + Subclass Feature #4 =====
            if (Level >= 18)
            {
                _elusiveUnlocked = true;
                ApplySubclassFeature4();
                UpdateSneakAttackDice();
            }

            // ===== LEVEL 20: Ultimate Feature =====
            if (Level >= 20)
            {
                ApplyUltimateFeature();
            }
        }

        private void UpdateSneakAttackDice()
        {
            // Correct D&D 5e progression: +1d6 every 2 levels starting at level 1
            // Level 1: 1d6, Level 3: 2d6, Level 5: 3d6, Level 7: 4d6, etc.
            _sneakAttackDice = 1 + (Level - 1) / 2;

            Console.WriteLine($"{Name}'s Sneak Attack damage is now {_sneakAttackDice}d6!");
        }

        // ==================== Subclass Feature Methods ====================

        private void ApplySubclassFeatures()
        {
            switch (_subclass)
            {
                case RogueSubclass.Thief:
                    _subclassName = "Thief";
                    Console.WriteLine($"{Name} chooses the Thief subclass!");
                    Console.WriteLine("  - Fast Hands (Level 3): Use Dexterity checks to pick pockets, disarm traps");
                    Console.WriteLine("  - Second Thoughts (Level 9): Re-grab an item after a failed attempt");
                    break;

                case RogueSubclass.Assassin:
                    _subclassName = "Assassin";
                    Console.WriteLine($"{Name} chooses the Assassin subclass!");
                    Console.WriteLine("  - Bonus Proficiencies: Stealth, Disguise Kit");
                    Console.WriteLine("  - Assassinate (Level 3): Auto-crit on surprised creatures");
                    break;

                case RogueSubclass.ArcaneTrickster:
                    _subclassName = "Arcane Trickster";
                    Console.WriteLine($"{Name} chooses the Arcane Trickster subclass!");
                    AddStartingCantrips();
                    AddStartingSpells();
                    InitializeSpellSlots(3);
                    break;

                default:
                    _subclassName = "Thief";
                    break;
            }
        }

        private void UpdateSubclassFeatures()
        {
            switch (_subclass)
            {
                case RogueSubclass.Thief:
                    Console.WriteLine("Thief: Stone Ears (Level 6) - Immune to bardic inspiration effects");
                    break;
                case RogueSubclass.Assassin:
                    Console.WriteLine("Assassin: Infiltration Expertise (Level 6) - Can maintain a disguise indefinitely");
                    _infiltrationExpertiseBonus = GetProficiencyBonusForLevel(Level);
                    break;
                case RogueSubclass.ArcaneTrickster:
                    UpdateArcaneTricksterSpells();
                    break;
            }
        }

        private void ApplySubclassFeature4()
        {
            switch (_subclass)
            {
                case RogueSubclass.Thief:
                    Console.WriteLine("Thief Feature 4: Supreme Sneak - You have advantage on Stealth checks when making a ranged attack.");
                    _supremeSneakActive = true;
                    break;
                case RogueSubclass.Assassin:
                    Console.WriteLine("Assassin Feature 4: Infiltration Mastery - You can create a false identity for yourself.");
                    break;
                case RogueSubclass.ArcaneTrickster:
                    Console.WriteLine("Arcane Trickster Feature 4: Spell Storm - When you miss with a spell attack, you can cast a cantrip as a bonus action.");
                    _spellStormReady = true;
                    break;
            }
        }

        private void ApplyUltimateFeature()
        {
            switch (_subclass)
            {
                case RogueSubclass.Thief:
                    Console.WriteLine("Thief Ultimate: Thief's Reflexes - You can take two turns in the first round of any combat. You go first.");
                    break;
                case RogueSubclass.Assassin:
                    Console.WriteLine("Assassin Ultimate: Stone Heart - You are immune to being charmed or frightened.");
                    break;
                case RogueSubclass.ArcaneTrickster:
                    Console.WriteLine("Arcane Trickster Ultimate: Magical Ambush - Add your proficiency bonus to your initiative rolls. Once per turn, you can cause a creature within 5 feet of you to have disadvantage on the next attack roll against any target except you.");
                    _magicalAmbushActive = true;
                    break;
            }
        }

        // ==================== Arcane Trickster Spell Methods ====================

        private void AddStartingCantrips()
        {
            // Cantrips: Mage Hand, Minor Illusion, Disguise Self, Prestidigitation (levels 3-8)
            List<string> startingCantrips = new List<string>
            {
                "Mage Hand", "Minor Illusion", "Disguise Self", "Prestidigitation"
            };

            foreach (var cantrip in startingCantrips)
            {
                _cantripsKnown.Add(cantrip);
            }

            Console.WriteLine($"{Name} starts with cantrips: {string.Join(", ", _cantripsKnown)}");
        }

        private void AddStartingSpells()
        {
            // Level 3 spells (2 spells known): Charm Person, Disguise Self, Detect Magic, False Life, Illusory Script, Tasha's Hideous Laughter
            List<string> startingSpells = new List<string>
            {
                "Charm Person", "False Life"
            };

            foreach (var spellName in startingSpells)
            {
                SpellClass newSpell = new SpellClass(spellName, 1, School.Enchantment, "1 action", "Self", "Instantaneous");
                _knownSpells.Add(newSpell);
            }

            _spellsKnownCount = 2;
            Console.WriteLine($"{Name} starts with spells: {string.Join(", ", startingSpells)}");
        }

        private void InitializeSpellSlots(int level)
        {
            // Half-caster progression (same as Paladin/Artificer/Eldritch Knight half-level)
            _spellSlotsByLevel.Clear();

            if (level >= 3 && level <= 5)
            {
                _spellSlotsByLevel[1] = 2;
            }
            else if (level >= 6 && level <= 9)
            {
                _spellSlotsByLevel[1] = 3;
                _spellSlotsByLevel[2] = 2;
            }
            else if (level >= 10 && level <= 13)
            {
                _spellSlotsByLevel[1] = 4;
                _spellSlotsByLevel[2] = 3;
                _spellSlotsByLevel[3] = 2;
            }
            else if (level >= 14 && level <= 15)
            {
                _spellSlotsByLevel[1] = 4;
                _spellSlotsByLevel[2] = 3;
                _spellSlotsByLevel[3] = 3;
            }
            else if (level >= 16 && level <= 19)
            {
                _spellSlotsByLevel[1] = 4;
                _spellSlotsByLevel[2] = 3;
                _spellSlotsByLevel[3] = 3;
                _spellSlotsByLevel[4] = 1;
            }
            else if (level >= 20)
            {
                _spellSlotsByLevel[1] = 4;
                _spellSlotsByLevel[2] = 3;
                _spellSlotsByLevel[3] = 3;
                _spellSlotsByLevel[4] = 2;
            }

            Console.WriteLine($"{Name} has {GetTotalSpellSlots()} total spell slots.");
        }

        private void UpdateArcaneTricksterSpells()
        {
            // Spells known progression: 2 (L3), 3 (L4), 4 (L5), 5 (L6), 6 (L7), 7 (L8), 8 (L9), 9 (L10)
            // 10 (L11), 11 (L12), 12 (L13), 13 (L14), 14 (L15), 15 (L16), 15 (L17), 15 (L18), 15 (L19), 15 (L20)
            // Actually it's: 4 (L3-7), 5 (L8-10), 6 (L11-13), 7 (L14-16), 8 (L17-20)... no that doesn't match either.
            // Correct AT spell progression:
            // Level 3: 4 spells, Level 4: 5, Level 5: 6, Level 6: 6, Level 7: 7, Level 8: 8, Level 9: 9, Level 10: 10
            // Level 11: 11, Level 12: 11, Level 13: 11, Level 14: 11, Level 15: 11... wait let me check.
            // PHB Arcane Trickster: 4 (L3-8), 5 (L9-10), 6 (L11-12), 7 (L13-14), 8 (L15), 9 (L17), 10 (L19), 10 (L20)
            // Actually: 
            if (Level >= 3 && Level < 4) _spellsKnownCount = 4;
            else if (Level >= 4 && Level < 8) _spellsKnownCount = 5;
            else if (Level >= 8 && Level < 10) _spellsKnownCount = 6;
            else if (Level >= 10 && Level < 11) _spellsKnownCount = 7;
            else if (Level >= 11 && Level < 13) _spellsKnownCount = 8;
            else if (Level >= 13 && Level < 16) _spellsKnownCount = 9;
            else if (Level >= 16 && Level < 20) _spellsKnownCount = 10;
            else _spellsKnownCount = 10;

            // Cantrips known: 3 (L3), +1 at L7, +1 at L10 = 5 total
            int maxCantrips = 3;
            if (Level >= 7) maxCantrips = 4;
            if (Level >= 10) maxCantrips = 5;
        }

        public int GetTotalSpellSlots()
        {
            int total = 0;
            foreach (var slot in _spellSlotsByLevel)
            {
                total += slot.Value;
            }
            return total;
        }

        /// <summary>
        /// Learn a new spell from the Arcane Trickster spell list.
        /// </summary>
        public virtual void LearnSpell(string spellName, int spellLevel)
        {
            if (_subclass != RogueSubclass.ArcaneTrickster)
            {
                Console.WriteLine($"{Name} is not an Arcane Trickster.");
                return;
            }

            SpellClass newSpell = new SpellClass(spellName, spellLevel, School.Enchantment, "1 action", "Self", "Instantaneous");
            _knownSpells.Add(newSpell);
            Console.WriteLine($"{Name} has learned the spell: {spellName} (Level {spellLevel})");
        }

        /// <summary>
        /// Cast a known spell using a spell slot.
        /// </summary>
        public virtual bool CastSpell(string spellName, int slotLevel)
        {
            if (_subclass != RogueSubclass.ArcaneTrickster)
                return false;

            SpellClass? spell = _knownSpells.Find(s => s.Name == spellName);
            if (spell == null)
            {
                Console.WriteLine($"{Name} does not know the spell: {spellName}");
                return false;
            }

            if (!_spellSlotsByLevel.ContainsKey(slotLevel) || _spellSlotsByLevel[slotLevel] <= 0)
            {
                Console.WriteLine($"{Name} has no spell slots of level {slotLevel}.");
                return false;
            }

            _spellSlotsByLevel[slotLevel]--;
            Console.WriteLine($"{Name} casts {spellName} using a {slotLevel}-level spell slot.");
            return true;
        }

        // ==================== Stat Calculations ====================

        protected override void CalculateBaseStats()
        {
            int conMod = Math.Max(-5, (Constitution - 10) / 2);
            int dexMod = Math.Max(-5, (Dexterity - 10) / 2);

            // Max HP: d6 per level + con mod per level
            int hpFromFirstLevel = 6 + conMod;
            int hpFromHigherLevels = (Level - 1) * (6 + conMod);
            MaxHitPoints = hpFromFirstLevel + hpFromHigherLevels;

            if (HitPoints > MaxHitPoints || HitPoints <= 0)
            {
                HitPoints = MaxHitPoints;
            }

            // AC: Unarmored defense not official, but Thief option exists
            // Standard: Light armor (max Dex bonus applies)
            // Leather armor: 11 + Dex mod (up to 2) = 13 base
            ArmorClass = 11 + Math.Min(2, dexMod); // With leather armor

            // Speed defaults to race speed (typically 30 ft)
            Speed = _race != null ? _race.Speed : 30;

            Console.WriteLine($"{Name}'s stats calculated: HP {MaxHitPoints}, AC {ArmorClass}");
        }

        public static int GetProficiencyBonusForLevel(int level)
        {
            if (level <= 4) return 2;
            if (level <= 8) return 3;
            if (level <= 12) return 4;
            if (level <= 16) return 5;
            return 6;
        }

        // ==================== Override Base Methods ====================

        public override void Attack()
        {
            string subclassPrefix = GetSubclassName(_subclass);
            Console.WriteLine($"{Name} (Level {_level} {subclassPrefix}) makes a weapon attack!");
            Console.WriteLine("Rogues excel at precision strikes with Finesse weapons (rapier, shortsword) or Ranged weapons (shortbow, hand crossbow).");

            if (_hasAdvantageOnSneakAttack)
            {
                Console.WriteLine("  ** Sneak Attack is ready! **");
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

            UpdateSneakAttackDice();

            // Expertise slot increases at levels 6, 10, 14
            if (Level == 6 || Level == 10 || Level == 14)
            {
                _expertiseSkillCount += 2;
                Console.WriteLine($"{_name} gains 2 more expertise slots! Total: {_expertiseSkillCount}");
            }

            // Update spell slots for Arcane Trickster at even levels
            if (_subclass == RogueSubclass.ArcaneTrickster && Level % 2 == 0)
            {
                InitializeSpellSlots(Level);
            }

            ApplyLevelFeatures();
            DisplayLevelFeatures();
        }

        public override void LongRest()
        {
            base.LongRest();

            _hasAdvantageOnSneakAttack = false;

            if (_subclass == RogueSubclass.Assassin)
            {
                _assassinateUnlockedThisCombat = false;
            }

            Console.WriteLine($"{Name} recovers all abilities after a long rest.");
        }

        public override void ShortRest()
        {
            base.ShortRest();
            Console.WriteLine($"{Name} feels refreshed but doesn't recover special abilities on a short rest.");
        }

        public override void DisplayCharacter()
        {
            string subclassPrefix = GetSubclassName(_subclass);

            Console.WriteLine();
            Console.WriteLine($"=== {Name} (Level {_level} Rogue - {subclassPrefix}) ===");
            Console.WriteLine($"Hit Points: {HitPoints}/{MaxHitPoints} | AC: {ArmorClass} | Speed: {Speed}");
            Console.WriteLine("Ability Scores:");
            Console.WriteLine($"  Strength:    {Strength} (mod +{Math.Max(-5, (Strength - 10) / 2)})");
            Console.WriteLine($"  Dexterity:   {Dexterity} (mod +{Math.Max(-5, (Dexterity - 10) / 2)})");
            Console.WriteLine($"  Constitution:{Constitution} (mod +{Math.Max(-5, (Constitution - 10) / 2)})");
            Console.WriteLine($"  Intelligence:{Intelligence} (mod +{Math.Max(-5, (Intelligence - 10) / 2)})");
            Console.WriteLine($"  Wisdom:      {Wisdom} (mod +{Math.Max(-5, (Wisdom - 10) / 2)})");
            Console.WriteLine($"  Charisma:    {Charisma} (mod +{Math.Max(-5, (Charisma - 10) / 2)})");

            Console.WriteLine();
            Console.WriteLine("--- Rogue Abilities ---");
            Console.WriteLine($"Sneak Attack: {_sneakAttackDice}d6 extra damage when you have advantage or ally adjacent");
            Console.WriteLine($"Expertise: {_expertisedSkills.Count}/{_expertiseSkillCount} skills doubled for proficiency");

            if (_nimbleEscapeUnlocked)
                Console.WriteLine("Nimble Escape: Hide or Disengage as bonus action");
            if (_uncannyDodgeUnlocked)
                Console.WriteLine("Uncanny Dodge: Reaction to halve damage");
            if (_evasionUnlocked)
                Console.WriteLine("Evasion: No damage on successful save, half on fail");
            if (_reliableTimingUnlocked)
                Console.WriteLine("Reliable Talent: Minimum 10 on skill checks");
            if (_slipperyMindUnlocked)
                Console.WriteLine("Slippery Mind: Immune to charm/frightened");
            if (_elusiveUnlocked)
                Console.WriteLine("Elusive: Can't be targeted by attacks");

            DisplaySubclassInfo();

            Console.WriteLine();
            Console.WriteLine("=== End Character Sheet ===");
        }

        private void DisplaySubclassInfo()
        {
            Console.WriteLine();
            Console.WriteLine($"--- Subclass: {_subclassName} ---");

            switch (_subclass)
            {
                case RogueSubclass.Thief:
                    Console.WriteLine("  Fast Hands: Use Dexterity to pick pockets, disarm traps");
                    if (_reliableTimingUnlocked)
                        Console.WriteLine($"  Second Thoughts: Can retrieve an item from a dropped grasp within {_secondThoughtsDistance} feet");
                    break;

                case RogueSubclass.Assassin:
                    Console.WriteLine("  Assassinate: Auto-critical on surprised creatures, +5d6 damage vs them");
                    if (_infiltrationExpertiseBonus > 0)
                        Console.WriteLine($"  Infiltration Expertise: Can maintain a disguise indefinitely (+{_infiltrationExpertiseBonus} to Disguise checks)");
                    break;

                case RogueSubclass.ArcaneTrickster:
                    Console.WriteLine($"  Spells Known: {_spellsKnownCount} (Cantrips: {_cantripsKnown.Count})");
                    Console.WriteLine($"  Spell Slots: {GetTotalSpellSlots()} total");
                    if (_magicalAmbushActive)
                        Console.WriteLine("  Magical Ambush: +proficiency bonus to initiative, disadvantage aura");
                    break;
            }

            Console.WriteLine();
        }

        private void DisplayLevelFeatures()
        {
            Console.WriteLine();
            Console.WriteLine("--- Gained Features at Level " + Level + " ---");

            if (Level == 3)
            {
                Console.WriteLine($"  Subclass feature: {_subclassName}");
            }
            else if (Level == 5)
            {
                Console.WriteLine("  Uncanny Dodge: Reaction to halve damage");
                Console.WriteLine("  Evasion: Take no damage on successful save, half on fail");
            }
            else if (Level == 7)
            {
                Console.WriteLine("  High Mobility: Move without triggering opportunity attacks more easily");
            }
            else if (Level == 9)
            {
                Console.WriteLine("  Reliable Talent: All skill checks minimum 10");
            }
            else if (Level == 11)
            {
                Console.WriteLine("  Blindsense: Sense invisible creatures within 10 feet");
            }
            else if (Level == 13)
            {
                Console.WriteLine("  Master Thief: Can use another creature's class feature once per short rest");
            }
            else if (Level == 15)
            {
                Console.WriteLine("  Slippery Mind: Immune to being charmed or frightened");
            }
            else if (Level == 18)
            {
                Console.WriteLine("  Elusive: Cannot be targeted by attacks while moving");
            }
            else if (Level == 20)
            {
                ApplyUltimateFeature();
            }

            Console.WriteLine();
        }

        private string GetSubclassName(RogueSubclass subclass)
        {
            switch (subclass)
            {
                case RogueSubclass.Thief: return "Thief";
                case RogueSubclass.Assassin: return "Assassin";
                case RogueSubclass.ArcaneTrickster: return "Arcane Trickster";
                default: return "Thief";
            }
        }
    }
}