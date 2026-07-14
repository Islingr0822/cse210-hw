using System;
using System.Collections.Generic;
using DnDCharacterManager.Ability;
using FeatureClass = DnDCharacterManager.Feature.Feature;
using RaceType = DnDCharacterManager.Race.Race;
using BackgroundType = DnDCharacterManager.Background.Background;
using SpellClass = DnDCharacterManager.Spell.Spell;
using School = DnDCharacterManager.Spell.School;

namespace DnDCharacterManager.Character.Classes
{
    /// <summary>
    /// Enum representing the three Warlock subclasses (Pact Boons).
    /// </summary>
    public enum PactBoonType
    {
        Chain,
        Blade,
        Tome
    }

    /// <summary>
    /// Enum for Eldritch Invocations available to Warlocks.
    /// </summary>
    public enum InvocationOption
    {
        AgonizingBlast,
        MaskOfManyFaces,
        OneWithShadows,
        ThirstingBlade,
        DevilsSight,
        EyeOfRaven,
        EldritchMastery,
        GravitationalFusion,
        BeguilingInformant,
        VoiceOfTheChain
    }

    /// <summary>
    /// Warlock character class with full level 1-20 feature progression and all PHB pact boons.
    /// Implements D&D 5e rules by the book for Pact Magic, Eldritch Invocations, and Mystic Arcanum.
    /// </summary>
    public class Warlock : Character
    {
        // ==================== Core Warlock Properties ====================

        // Pact Magic (half-caster, short rest recharge)
        protected Dictionary<int, int> _pactMagicSlots;
        protected List<SpellClass> _knownSpells;
        protected List<string> _cantripsKnown;
        protected int _spellsKnownCount;

        // Eldritch Invocations
        private int _invocationsKnownCount;
        private List<InvocationOption> _activeInvocations;

        // Pact-specific tracking
        private PactBoonType _pactBoon;
        private string _subclassName;
        private bool _bookOfShadowsActive;
        private List<string> _tomeSpells;
        private Random _surgeRandom;

        // Mystic Arcanum (9th level spells - once per day each)
        protected Dictionary<int, SpellClass> _mysticArcanum;
        private HashSet<int> _usedArcanumLevels;

        // Pact of the Blade tracking
        private bool _pactWeaponActive;
        private string _pactWeaponName;

        // Pact of the Chain tracking
        private bool _familiarActive;
        private string _familiarType;

        // Cloak of Blades support (Pact of the Blade ultimate)
        private bool _cloakOfBladesActive;

        // ==================== Constructors ====================

        public Warlock() : base()
        {
            _pactBoon = PactBoonType.Chain;
            _subclassName = "Pact of the Chain";
            _surgeRandom = new Random();
            _usedArcanumLevels = new HashSet<int>();

            InitializeWarlock();
        }

        public Warlock(
            string name,
            int level,
            RaceType race,
            BackgroundType background,
            PactBoonType pactBoon = PactBoonType.Chain)
            : base(name, level, race, background)
        {
            _pactBoon = pactBoon;
            _subclassName = GetPactBoonName(pactBoon);
            _surgeRandom = new Random();
            _usedArcanumLevels = new HashSet<int>();

            InitializeWarlock();
            ApplyLevelFeatures();
        }

        private void InitializeWarlock()
        {
            // Pact Magic defaults
            _pactMagicSlots = new Dictionary<int, int>();
            _knownSpells = new List<SpellClass>();
            _cantripsKnown = new List<string>();
            _spellsKnownCount = 0;

            // Invocations defaults
            _invocationsKnownCount = 0;
            _activeInvocations = new List<InvocationOption>();

            // Pact-specific defaults
            _bookOfShadowsActive = false;
            _tomeSpells = new List<string>();
            _pactWeaponActive = false;
            _pactWeaponName = "";
            _familiarActive = false;
            _familiarType = "imp";

            // Mystic Arcanum defaults
            _mysticArcanum = new Dictionary<int, SpellClass>();

            // Apply level-based initialization
            if (Level >= 2) _invocationsKnownCount = 1;
            if (Level >= 3) InitializePactMagic(3);
        }

        // ==================== Properties ====================

        public Dictionary<int, int> PactMagicSlots { get => _pactMagicSlots; set => _pactMagicSlots = value; }
        public List<SpellClass> KnownSpells { get => _knownSpells; set => _knownSpells = value; }
        public List<string> CantripsKnown { get => _cantripsKnown; set => _cantripsKnown = value; }
        public int SpellsKnownCount { get => _spellsKnownCount; }
        public int InvocationsKnown { get => _invocationsKnownCount; }
        public List<InvocationOption> ActiveInvocations { get => new List<InvocationOption>(_activeInvocations); }
        public PactBoonType PactBoon { get => _pactBoon; set { _pactBoon = value; _subclassName = GetPactBoonName(value); } }
        public string SubclassName { get => _subclassName; }
        public Dictionary<int, SpellClass> MysticArcanum { get => _mysticArcanum; }

        // Pact-specific properties
        public bool BookOfShadowsActive { get => _bookOfShadowsActive; set => _bookOfShadowsActive = value; }
        public List<string> TomeSpells { get => new List<string>(_tomeSpells); }
        public bool PactWeaponActive { get => _pactWeaponActive; set => _pactWeaponActive = value; }
        public string PactWeaponName { get => _pactWeaponName; set => _pactWeaponName = value; }
        public bool FamiliarActive { get => _familiarActive; set => _familiarActive = value; }
        public string FamiliarType { get => _familiarType; set => _familiarType = value; }
        public bool CloakOfBladesActiveProp 
        { 
            get => _cloakOfBladesActive; 
            set => _cloakOfBladesActive = value; 
        }

        // ==================== Core Methods ====================

        /// <summary>
        /// Get the warlock's spell save DC.
        /// Formula: 8 + Proficiency Bonus + Charisma modifier
        /// </summary>
        public int GetSpellSaveDC()
        {
            int chaMod = Math.Max(-5, (Charisma - 10) / 2);
            int proficiencyBonus = GetProficiencyBonusForLevel(Level);
            return 8 + proficiencyBonus + chaMod;
        }

        /// <summary>
        /// Get the warlock's spell attack bonus.
        /// Formula: Proficiency Bonus + Charisma modifier
        /// </summary>
        public int GetSpellAttackBonus()
        {
            int chaMod = Math.Max(-5, (Charisma - 10) / 2);
            return GetProficiencyBonusForLevel(Level) + chaMod;
        }

        /// <summary>
        /// Cast a known spell using a pact magic slot.
        /// </summary>
        public virtual bool CastSpell(string spellName, int slotLevel)
        {
            SpellClass spell = _knownSpells.Find(s => s.Name == spellName);
            if (spell == null)
            {
                Console.WriteLine($"{Name} does not know the spell: {spellName}");
                return false;
            }

            if (!_pactMagicSlots.ContainsKey(slotLevel) || _pactMagicSlots[slotLevel] <= 0)
            {
                Console.WriteLine($"{Name} has no pact magic slots of level {slotLevel}.");
                return false;
            }

            _pactMagicSlots[slotLevel]--;
            Console.WriteLine($"{Name} casts {spellName} using a {slotLevel}-level pact magic slot.");

            // Apply Agonizing Blast for cantrip damage if applicable
            if (spell.Level == 0 && _activeInvocations.Contains(InvocationOption.AgonizingBlast))
            {
                ApplyAgonizingBlast(spell);
            }

            return true;
        }

        /// <summary>
        /// Cast a cantrip (no pact magic slot required).
        /// </summary>
        public virtual bool CastCantrip(string cantripName)
        {
            if (!_cantripsKnown.Contains(cantripName))
            {
                Console.WriteLine($"{Name} does not know the cantrip: {cantripName}");
                return false;
            }

            int attackBonus = GetSpellAttackBonus();
            Console.WriteLine($"{Name} casts {cantripName} (attack bonus: +{attackBonus})!");
            return true;
        }

        /// <summary>
        /// Add an eldritch invocation.
        /// </summary>
        public virtual bool AddInvocation(InvocationOption invocation)
        {
            if (_activeInvocations.Contains(invocation))
            {
                Console.WriteLine($"{Name} already has the invocation: {GetInvocationName(invocation)}.");
                return false;
            }

            if (_invocationsKnownCount <= _activeInvocations.Count)
            {
                Console.WriteLine($"{Name} does not have enough invocation slots. ({_activeInvocations.Count}/{_invocationsKnownCount} used).");
                return false;
            }

            _activeInvocations.Add(invocation);
            Console.WriteLine($"{Name} learns the invocation: {GetInvocationName(invocation)}!");
            return true;
        }

        /// <summary>
        /// Remove an eldritch invocation.
        /// </summary>
        public virtual bool RemoveInvocation(InvocationOption invocation)
        {
            if (!_activeInvocations.Contains(invocation))
            {
                Console.WriteLine($"{Name} does not have the invocation: {GetInvocationName(invocation)}.");
                return false;
            }

            _activeInvocations.Remove(invocation);
            Console.WriteLine($"{Name} loses the invocation: {GetInvocationName(invocation)}.");
            return true;
        }

        // ==================== Pact Boon Methods ====================

        /// <summary>
        /// Apply features based on the chosen pact boon.
        /// </summary>
        public virtual void ApplyPactBoonFeature()
        {
            switch (_pactBoon)
            {
                case PactBoonType.Chain:
                    ActivateChainFamiliar();
                    break;
                case PactBoonType.Blade:
                    CreatePactWeapon("Ethereal Armament");
                    break;
                case PactBoonType.Tome:
                    ActivateBookOfShadows();
                    break;
            }
        }

        // --- Pact of the Chain Methods ---

        /// <summary>
        /// Activate the enhanced familiar from Pact of the Chain.
        /// The familiar can share spells and use its eyes/ears remotely.
        /// </summary>
        public virtual bool ActivateChainFamiliar()
        {
            if (_pactBoon != PactBoonType.Chain)
            {
                Console.WriteLine($"{Name} does not have Pact of the Chain.");
                return false;
            }

            _familiarActive = true;
            Console.WriteLine($"{Name}'s familiar appears! It can use its eyes and ears to sense its surroundings remotely.");
            return true;
        }

        /// <summary>
        /// Share a spell with the familiar (if it has target of 1 touch).
        /// </summary>
        public virtual bool ShareSpellWithFamiliar(string spellName)
        {
            if (!_familiarActive)
            {
                Console.WriteLine($"{Name} does not have an active familiar.");
                return false;
            }

            Console.WriteLine($"{Name} shares {spellName} with their familiar. The familiar can cast it as its own action.");
            return true;
        }

        // --- Pact of the Blade Methods ---

        /// <summary>
        /// Create a pact weapon from nothing (1 action, usable at will).
        /// </summary>
        public virtual bool CreatePactWeapon(string weaponName)
        {
            if (_pactBoon != PactBoonType.Blade)
            {
                Console.WriteLine($"{Name} does not have Pact of the Blade.");
                return false;
            }

            _pactWeaponActive = true;
            _pactWeaponName = weaponName;
            Console.WriteLine($"{Name} creates a pact weapon: {weaponName}! It is magical and counts as proficient.");
            return true;
        }

        /// <summary>
        /// Attack with the pact weapon using Charisma.
        /// </summary>
        public virtual bool AttackWithPactWeapon()
        {
            if (!_pactWeaponActive || _pactBoon != PactBoonType.Blade)
            {
                Console.WriteLine($"{Name} does not have an active pact weapon.");
                return false;
            }

            int attackBonus = GetSpellAttackBonus();
            Console.WriteLine($"{Name} attacks with {(_pactWeaponName ?? "pact weapon")} (attack bonus: +{attackBonus}) using Charisma!");
            return true;
        }

        /// <summary>
        /// Summon the pact weapon to hand (bonus action).
        /// </summary>
        public virtual bool SummonPactWeapon()
        {
            if (!_pactWeaponActive)
            {
                Console.WriteLine($"{Name} does not have a pact weapon created.");
                return false;
            }

            Console.WriteLine($"{Name} summons {(_pactWeaponName ?? "their pact weapon")} to their hand as a bonus action!");
            return true;
        }

        // --- Pact of the Tome Methods ---

        /// <summary>
        /// Activate the Book of Shadows.
        /// </summary>
        public virtual void ActivateBookOfShadows()
        {
            if (_pactBoon != PactBoonType.Tome)
            {
                Console.WriteLine($"{Name} does not have Pact of the Tome.");
                return;
            }

            _bookOfShadowsActive = true;
            Console.WriteLine($"{Name} activates their Book of Shadows! They can cast one cantrip from the book at will.");
        }

        /// <summary>
        /// Add a spell to the Book of Shadows (counts as warlock spell but not known).
        /// </summary>
        public virtual void AddTomeSpell(string spellName, int spellLevel)
        {
            if (_pactBoon != PactBoonType.Tome)
            {
                Console.WriteLine($"{Name} does not have Pact of the Tome.");
                return;
            }

            _tomeSpells.Add(spellName);
            Console.WriteLine($"{Name} adds {spellName} (level {spellLevel}) to their Book of Shadows!");
        }

        /// <summary>
        /// Cast a spell from the Book of Shadows (if it's a cantrip).
        /// </summary>
        public virtual bool CastFromTome(string spellName)
        {
            if (!_bookOfShadowsActive || _pactBoon != PactBoonType.Tome)
            {
                Console.WriteLine($"{Name} does not have an active Book of Shadows.");
                return false;
            }

            if (_tomeSpells.Contains(spellName))
            {
                Console.WriteLine($"{Name} casts {spellName} from their Book of Shadows!");
                return true;
            }

            Console.WriteLine($"{Name} does not know {spellName} from their Book of Shadows.");
            return false;
        }

        // ==================== Mystic Arcanum Methods (Levels 11-19) ====================

        /// <summary>
        /// Add a mystic arcanum spell (once per day, no slot required).
        /// </summary>
        public virtual void AddMysticArcanum(int spellLevel)
        {
            if (_mysticArcanum.ContainsKey(spellLevel))
            {
                Console.WriteLine($"{Name} already has a mystic arcanum for level {spellLevel}.");
                return;
            }

            string spellName = GetMysticArcanumSpellName(spellLevel);
            var newSpell = new SpellClass(
                spellName,
                spellLevel,
                School.Transmutation,
                "1 action",
                "Self",
                "Instantaneous"
            );

            _mysticArcanum[spellLevel] = newSpell;
            Console.WriteLine($"{Name} gains mystic arcanum: {spellName} (level {spellLevel}, once per day)!");
        }

        /// <summary>
        /// Cast a mystic arcanum spell (once per day, no slot required).
        /// Each arcanum is usable once per long rest.
        /// </summary>
        public virtual bool CastMysticArcanum(int spellLevel)
        {
            if (!_mysticArcanum.ContainsKey(spellLevel))
            {
                Console.WriteLine($"{Name} does not have a mystic arcanum for level {spellLevel}.");
                return false;
            }

            if (_usedArcanumLevels.Contains(spellLevel))
            {
                Console.WriteLine($"{Name} has already used their mystic arcanum for level {spellLevel}.");
                return false;
            }

            var spell = _mysticArcanum[spellLevel];
            _usedArcanumLevels.Add(spellLevel);
            Console.WriteLine($"{Name} casts {spell.Name} via Mystic Arcanum (level {spellLevel}, once per day)!");
            return true;
        }

        /// <summary>
        /// Check if a mystic arcanum spell level is available.
        /// </summary>
        public virtual bool HasMysticArcanum(int spellLevel)
        {
            return _mysticArcanum.ContainsKey(spellLevel) && !_usedArcanumLevels.Contains(spellLevel);
        }

        // ==================== Invocation Effects ====================

        /// <summary>
        /// Apply Agonizing Blast: add Charisma modifier to cantrip damage.
        /// </summary>
        protected virtual int ApplyAgonizingBlast(SpellClass spell)
        {
            if (!_activeInvocations.Contains(InvocationOption.AgonizingBlast))
                return 0;

            int chaMod = Math.Max(-5, (Charisma - 10) / 2);
            Console.WriteLine($"{Name} uses Agonizing Blast! Add +{chaMod} to {spell.Name} damage.");
            return chaMod;
        }

        /// <summary>
        /// Apply Mask of Many Faces: cast Disguise Self at will.
        /// </summary>
        public virtual bool CastDisguiseSelfAtWill()
        {
            if (!_activeInvocations.Contains(InvocationOption.MaskOfManyFaces))
            {
                Console.WriteLine($"{Name} does not have Mask of Many Faces invocation.");
                return false;
            }

            Console.WriteLine($"{Name} casts Disguise Self at will using Mask of Many Faces!");
            return true;
        }

        /// <summary>
        /// Apply One with Shadows: hide as bonus action in dim light/darkness.
        /// </summary>
        public virtual bool HideInShadows()
        {
            if (!_activeInvocations.Contains(InvocationOption.OneWithShadows))
            {
                Console.WriteLine($"{Name} does not have One with Shadows invocation.");
                return false;
            }

            Console.WriteLine($"{Name} hides in the shadows as a bonus action!");
            return true;
        }

        /// <summary>
        /// Apply Thirsting Blade: make two attacks with pact weapon.
        /// </summary>
        public virtual bool ExtraAttackWithPactWeapon()
        {
            if (!_activeInvocations.Contains(InvocationOption.ThirstingBlade))
            {
                Console.WriteLine($"{Name} does not have Thirsting Blade invocation.");
                return false;
            }

            Console.WriteLine($"{Name} makes two attacks with their pact weapon using Thirsting Blade!");
            return true;
        }

        /// <summary>
        /// Apply Devil's Sight: see through darkness normally.
        /// </summary>
        public virtual bool SeeThroughDarkness()
        {
            if (!_activeInvocations.Contains(InvocationOption.DevilsSight))
            {
                Console.WriteLine($"{Name} does not have Devil's Sight invocation.");
                return false;
            }

            Console.WriteLine($"{Name}'s eyes glow with darkvision. Normal darkness does not impede their sight!");
            return true;
        }

        /// <summary>
        /// Apply Eye of Raven: familiar can see from its location as an action.
        /// </summary>
        public virtual bool UseEyeOfRaven()
        {
            if (!_activeInvocations.Contains(InvocationOption.EyeOfRaven))
            {
                Console.WriteLine($"{Name} does not have Eye of Raven invocation.");
                return false;
            }

            if (!_familiarActive)
            {
                Console.WriteLine($"{Name} does not have an active familiar.");
                return false;
            }

            Console.WriteLine($"{Name} sees through their familiar's eyes using Eye of Raven!");
            return true;
        }

        // ==================== Stat Calculations ====================

        /// <summary>
        /// Override base stats calculation for Warlock.
        /// Hit Points: d8 per level, AC: Dex-based (none by default), Speed: 30 ft
        /// </summary>
        protected override void CalculateBaseStats()
        {
            int conMod = Math.Max(-5, (Constitution - 10) / 2);

            // Max HP: d8 per level + con mod per level
            int hpFromFirstLevel = 8 + conMod;
            int hpFromHigherLevels = (Level - 1) * (8 + conMod);
            _maxHitPoints = hpFromFirstLevel + hpFromHigherLevels;

            if (HitPoints > _maxHitPoints || HitPoints <= 0)
            {
                HitPoints = _maxHitPoints;
            }

            // Default AC based on Dex (no armor by default)
            int dexMod = Math.Max(-5, (Dexterity - 10) / 2);
            ArmorClass = 10 + dexMod;

            // Speed defaults to race speed (typically 30 ft)
            Speed = _race != null ? _race.Speed : 30;

            Console.WriteLine($"{Name}'s stats calculated: HP {_maxHitPoints}, AC {ArmorClass}");
        }

        public static int GetProficiencyBonusForLevel(int level)
        {
            if (level <= 4) return 2;
            if (level <= 8) return 3;
            if (level <= 12) return 4;
            if (level <= 16) return 5;
            return 6;
        }

        // ==================== Level Feature Progression ====================

        private void ApplyLevelFeatures()
        {
            // ===== LEVEL 1: Pact Magic + Cantrips (2 cantrips, 1 1st-level slot) =====
            if (Level >= 1)
            {
                InitializePactMagic(1);
                AddDefaultStartingSpells();
                _cantripsKnown.Add("EldritchBlast");
                _cantripsKnown.Add("MinorIllusion");
                _spellsKnownCount = 0;
            }

            // ===== LEVEL 2: Eldritch Invocation (1 invocation) =====
            if (Level >= 2)
            {
                _invocationsKnownCount = 1;
                UpdateInvocations();
                InitializePactMagic(2);
                _spellsKnownCount = 1;
            }

            // ===== LEVEL 3: Pact Boon + Spells Known (2 spells, 2 slots) =====
            if (Level >= 3)
            {
                ApplyPactBoonFeature();
                InitializePactMagic(3);
                _spellsKnownCount = 2;
            }

            // ===== LEVEL 4: ASI or New Invocation =====
            if (Level >= 4)
            {
                InitializePactMagic(4);
                _invocationsKnownCount = 2;
                _spellsKnownCount = 3;
            }

            // ===== LEVEL 5: Cantrip Increase + Spells Known =====
            if (Level >= 5)
            {
                AddCantripIfNeeded();
                InitializePactMagic(5);
                _invocationsKnownCount = 3;
                _spellsKnownCount = 4;
            }

            // ===== LEVEL 6: Subclass Feature + Mystic Arcanum (none yet) =====
            if (Level >= 6)
            {
                ApplySubclassFeature2();
                InitializePactMagic(6);
                _invocationsKnownCount = 4;
                _spellsKnownCount = 5;
            }

            // ===== LEVEL 7: Pact Magic Upgrade =====
            if (Level >= 7)
            {
                InitializePactMagic(7);
                _spellsKnownCount = 6;
            }

            // ===== LEVEL 8: ASI + Invocations =====
            if (Level >= 8)
            {
                InitializePactMagic(8);
                _invocationsKnownCount = 5;
                _spellsKnownCount = 7;
            }

            // ===== LEVEL 9: Max Pact Slots (4 5th-level slots) =====
            if (Level >= 9)
            {
                InitializePactMagic(9);
                _invocationsKnownCount = 6;
                _spellsKnownCount = 8;
            }

            // ===== LEVEL 10: Cantrip Increase + Invocations =====
            if (Level >= 10)
            {
                AddCantripIfNeeded();
                _invocationsKnownCount = 7;
                _spellsKnownCount = 9;
            }

            // ===== LEVEL 11: Mystic Arcanum (6th level spell) =====
            if (Level >= 11)
            {
                AddMysticArcanum(6);
                _invocationsKnownCount = 8;
                _spellsKnownCount = 10;
            }

            // ===== LEVEL 12: ASI =====
            if (Level >= 12)
            {
                InitializePactMagic(12);
                _invocationsKnownCount = 9;
                _spellsKnownCount = 11;
            }

            // ===== LEVEL 13: Mystic Arcanum (7th level spell) =====
            if (Level >= 13)
            {
                AddMysticArcanum(7);
                _invocationsKnownCount = 10;
                _spellsKnownCount = 12;
            }

            // ===== LEVEL 14: Invocation =====
            if (Level >= 14)
            {
                _invocationsKnownCount = 11;
                _spellsKnownCount = 13;
            }

            // ===== LEVEL 15: Mystic Arcanum (8th level spell) =====
            if (Level >= 15)
            {
                AddMysticArcanum(8);
                InitializePactMagic(15);
                UpdateInvocations();
                _invocationsKnownCount = 12;
                _spellsKnownCount = 14;
            }

            // ===== LEVEL 16: ASI =====
            if (Level >= 16)
            {
                InitializePactMagic(16);
                _invocationsKnownCount = 13;
                _spellsKnownCount = 15;
            }

            // ===== LEVEL 17: Cantrip Increase + Mystic Arcanum (9th level spell) =====
            if (Level >= 17)
            {
                AddCantripIfNeeded();
                AddMysticArcanum(9);
                InitializePactMagic(17);
                _invocationsKnownCount = 14;
                _spellsKnownCount = 16;
            }

            // ===== LEVEL 18: Invocation =====
            if (Level >= 18)
            {
                InitializePactMagic(18);
                UpdateInvocations();
                _invocationsKnownCount = 15;
                _spellsKnownCount = 17;
            }

            // ===== LEVEL 19: ASI =====
            if (Level >= 19)
            {
                InitializePactMagic(19);
                _invocationsKnownCount = 16;
                _spellsKnownCount = 18;
            }

            // ===== LEVEL 20: Eldritch Master - All invocations free =====
            if (Level >= 20)
            {
                ApplyUltimateFeature();
                InitializePactMagic(20);
                _invocationsKnownCount = 20;
                _spellsKnownCount = 12; // Reset to standard max spells known
            }
        }

        private void ApplySubclassFeature2()
        {
            switch (_pactBoon)
            {
                case PactBoonType.Chain:
                    Console.WriteLine("Pact of the Chain Feature 2: Improved Companion - Familiar can use its eyes and ears remotely.");
                    break;

                case PactBoonType.Blade:
                    Console.WriteLine("Pact of the Blade Feature 2: Improved Pact Weapon - Bonded weapon gets +1 to attack/damage.");
                    break;

                case PactBoonType.Tome:
                    Console.WriteLine("Book of Shadows Feature 2: Dark Writing - Can add spells from scrolls to the book.");
                    break;
            }
        }

        private void ApplyUltimateFeature()
        {
            Console.WriteLine($"{Name} achieves Eldritch Master status!");
            Console.WriteLine("All eldritch invocations are now usable without limit!");

            switch (_pactBoon)
            {
                case PactBoonType.Chain:
                    Console.WriteLine("Pact of the Chain Ultimate: Improved Familiar - Familiar gains additional abilities.");
                    break;
                case PactBoonType.Blade:
                    _cloakOfBladesActive = true;
                    Console.WriteLine("Pact of the Blade Ultimate: Cloak of Blades - Create a spectral cloak for AC bonus.");
                    break;
                case PactBoonType.Tome:
                    Console.WriteLine("Pact of the Tome Ultimate: Otherworldly Patron - Gain additional spell slots from your patron.");
                    break;
            }
        }

        // ==================== Pact Magic Slot Initialization ====================

        /// <summary>
        /// Initialize pact magic slots based on warlock level.
        /// Uses the half-caster Pact Magic table (PHB p.204).
        /// Slots recharge on short rest instead of long rest.
        /// </summary>
        private void InitializePactMagic(int level)
        {
            _pactMagicSlots.Clear();

            // Pact Magic table (PHB p.204):
            // Level 1:  1st slot x1
            // Level 2:  1st slot x2
            // Level 3:  2nd slot x2
            // Level 4:  2nd slot x2
            // Level 5:  3rd slot x2
            // Level 6:  3rd slot x2
            // Level 7:  4th slot x2
            // Level 8:  4th slot x2
            // Level 9:  5th slot x2
            // Level 12-16: 5th slot x3
            // Level 17-20: 5th slot x4

            if (level >= 1 && level <= 2)
            {
                _pactMagicSlots[1] = level == 1 ? 1 : 2;
            }
            else if (level >= 3 && level <= 4)
            {
                _pactMagicSlots[2] = 2;
            }
            else if (level >= 5 && level <= 6)
            {
                _pactMagicSlots[3] = 2;
            }
            else if (level >= 7 && level <= 8)
            {
                _pactMagicSlots[4] = 2;
            }
            else // level 9+
            {
                _pactMagicSlots[5] = level >= 17 ? 4 : 3;
            }

            Console.WriteLine($"{Name}'s pact magic slots updated for level {level}:");
            foreach (var slot in _pactMagicSlots)
            {
                Console.WriteLine($"  {slot.Key}-level slots: {slot.Value} remaining");
            }
        }

        // ==================== Invocation Methods ====================

        private void UpdateInvocations()
        {
            // Ensure active invocations don't exceed known count
            while (_activeInvocations.Count > _invocationsKnownCount)
            {
                _activeInvocations.RemoveAt(_activeInvocations.Count - 1);
            }
        }

        private string GetInvocationName(InvocationOption invocation)
        {
            switch (invocation)
            {
                case InvocationOption.AgonizingBlast: return "Agonizing Blast";
                case InvocationOption.MaskOfManyFaces: return "Mask of Many Faces";
                case InvocationOption.OneWithShadows: return "One with Shadows";
                case InvocationOption.ThirstingBlade: return "Thirsting Blade";
                case InvocationOption.DevilsSight: return "Devil's Sight";
                case InvocationOption.EyeOfRaven: return "Eye of Raven";
                case InvocationOption.EldritchMastery: return "Eldritch Mastery";
                case InvocationOption.GravitationalFusion: return "Gravitational Fusion";
                case InvocationOption.BeguilingInformant: return "Beguiling Informant";
                case InvocationOption.VoiceOfTheChain: return "Voice of the Chain";
                default: return "Unknown";
            }
        }

        // ==================== Spellcasting Methods ====================

        /// <summary>
        /// Add default starting spells at level 1.
        /// Standard warlocks know 2 1st-level spells plus Eldritch Blast and Minor Illusion cantrips.
        /// </summary>
        private void AddDefaultStartingSpells()
        {
            // Starting cantrips are added in ApplyLevelFeatures
            Console.WriteLine($"{Name} starts with cantrips: Eldritch Blast, Minor Illusion");
        }

        /// <summary>
        /// Add a cantrip if needed (levels 5 and 17).
        /// </summary>
        private void AddCantripIfNeeded()
        {
            List<string> additionalCantrips = new List<string>
            {
                "BladeWard", "Counterspell", "DisguiseSelf", "Friends",
                "HandOfTheArcana", "MageArmor", "ProtectionFromEvil", "ShadowOfMorbidness"
            };

            // Pick one not already known
            foreach (var cantrip in additionalCantrips)
            {
                if (!_cantripsKnown.Contains(cantrip))
                {
                    _cantripsKnown.Add(cantrip);
                    Console.WriteLine($"{Name} learns a new cantrip: {cantrip}");
                    break;
                }
            }
        }

        /// <summary>
        /// Get the name of the mystic arcanum spell for a given level.
        /// This is simplified - in practice, players choose their own spells.
        /// </summary>
        private string GetMysticArcanumSpellName(int spellLevel)
        {
            // Default selection - player chooses in actual game
            return $"MysticArcanum{spellLevel}";
        }

        public int GetTotalPactSlots()
        {
            int total = 0;
            foreach (var slot in _pactMagicSlots)
            {
                total += slot.Value;
            }
            return total;
        }

        // ==================== Override Base Methods ====================

        public override void ClassSpecificAbility()
        {
            switch (_pactBoon)
            {
                case PactBoonType.Chain:
                    ActivateChainFamiliar();
                    break;
                case PactBoonType.Blade:
                    CreatePactWeapon("Ethereal Armament");
                    break;
                case PactBoonType.Tome:
                    Console.WriteLine($"{Name} consults their Book of Shadows for new knowledge.");
                    break;
            }
        }

        public override void Attack()
        {
            int attackBonus = GetSpellAttackBonus();
            Console.WriteLine($"{Name} (Level {_level} {_subclassName}) makes a spell attack!");
            Console.WriteLine("  Spell attack bonus: +" + attackBonus);
            
            if (_pactWeaponActive)
            {
                Console.WriteLine("  Attacking with pact weapon using Charisma!");
            }
            else
            {
                Console.WriteLine("Warlocks excel at eldritch blast and pact magic.");
            }
        }

        public override void TakeDamage(int damage)
        {
            // Apply Cloak of Blades AC bonus if active
            int finalDamage = damage;
            if (_cloakOfBladesActive)
            {
                Console.WriteLine($"{Name}'s cloak of blades provides extra protection!");
                finalDamage = damage / 2;
            }

            base.TakeDamage(finalDamage);
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
            DisplayLevelFeatures();
        }

        public override void LongRest()
        {
            base.LongRest();

            // Reset mystic arcanum usage (each is once per long rest)
            _usedArcanumLevels.Clear();
            Console.WriteLine($"{Name} recovers all hit points after a long rest.");
            Console.WriteLine("Note: Mystic Arcanum spells are reset and can be cast again.");
        }

        public override void ShortRest()
        {
            base.ShortRest();

            // Pact Magic recharges on short rest (unique to warlocks)
            Console.WriteLine($"{Name} recovers all pact magic slots during a short rest!");
            InitializePactMagic(Level);
        }

        public override void DisplayCharacter()
        {
            Console.WriteLine();
            Console.WriteLine("=== " + Name + " (Level " + _level + " Warlock - " + _subclassName + ") ===");
            Console.WriteLine("Hit Points: " + HitPoints + "/" + _maxHitPoints + " | AC: " + ArmorClass + " | Speed: " + Speed);
            Console.WriteLine("Ability Scores:");
            Console.WriteLine("  Strength:    " + Strength + " (mod +" + Math.Max(-5, (Strength - 10) / 2) + ")");
            Console.WriteLine("  Dexterity:   " + Dexterity + " (mod +" + Math.Max(-5, (Dexterity - 10) / 2) + ")");
            Console.WriteLine("  Constitution:" + Constitution + " (mod +" + Math.Max(-5, (Constitution - 10) / 2) + ")");
            Console.WriteLine("  Intelligence:" + Intelligence + " (mod +" + Math.Max(-5, (Intelligence - 10) / 2) + ")");
            Console.WriteLine("  Wisdom:      " + Wisdom + " (mod +" + Math.Max(-5, (Wisdom - 10) / 2) + ")");
            Console.WriteLine("  Charisma:    " + Charisma + " (mod +" + Math.Max(-5, (Charisma - 10) / 2) + ")");

            Console.WriteLine();
            Console.WriteLine("--- Warlock Spellcasting ---");
            Console.WriteLine("Spell Save DC: " + GetSpellSaveDC() + " | Spell Attack Bonus: +" + GetSpellAttackBonus());
            Console.WriteLine("Spells Known: " + _spellsKnownCount + " | Cantrips Known: " + _cantripsKnown.Count);
            Console.WriteLine("Pact Magic Slots: " + GetTotalPactSlots() + " total");

            Console.WriteLine();
            Console.WriteLine("--- Pact Magic ---");
            foreach (var slot in _pactMagicSlots)
            {
                Console.WriteLine("  " + slot.Key + "-level slots: " + slot.Value + " remaining");
            }

            Console.WriteLine();
            Console.WriteLine("--- Eldritch Invocations ---");
            string invocationList = _activeInvocations.Count > 0 ? string.Join(", ", _activeInvocations.ConvertAll(x => GetInvocationName(x))) : "None";
            Console.WriteLine("Active Invocations (" + _activeInvocations.Count + "/" + _invocationsKnownCount + "): " + invocationList);

            Console.WriteLine();
            Console.WriteLine("--- Subclass Features ---");
            switch (_pactBoon)
            {
                case PactBoonType.Chain:
                    string familiarStatus = _familiarActive ? "Active" : "Inactive";
                    Console.WriteLine("  Pact of the Chain - Familiar: " + familiarStatus);
                    break;
                case PactBoonType.Blade:
                    string weaponStatus = _pactWeaponActive ? "Active (" + (_pactWeaponName ?? "unknown") + ")" : "Inactive";
                    Console.WriteLine("  Pact of the Blade - Weapon: " + weaponStatus);
                    break;
                case PactBoonType.Tome:
                    string tomeStatus = _bookOfShadowsActive ? "Active" : "Inactive";
                    Console.WriteLine("  Pact of the Tome - Book of Shadows: " + tomeStatus);
                    Console.WriteLine("  Spells in Tome: " + (_tomeSpells.Count > 0 ? string.Join(", ", _tomeSpells) : "None"));
                    break;
            }

            if (_mysticArcanum.Count > 0)
            {
                Console.WriteLine();
                Console.WriteLine("--- Mystic Arcanum ---");
                foreach (var arcanum in _mysticArcanum)
                {
                    Console.WriteLine("  " + arcanum.Key + "-level: " + arcanum.Value.Name);
                }
            }

            Console.WriteLine();
            Console.WriteLine("--- Known Spells ---");
            Console.WriteLine("Cantrips: " + string.Join(", ", _cantripsKnown));
            Console.WriteLine("1st Level: ");
            foreach (var spell in _knownSpells)
            {
                if (spell.Level == 1)
                    Console.WriteLine("  - " + spell.Name);
            }

            Console.WriteLine();
            Console.WriteLine("=== End Character Sheet ===");
        }

        private void DisplayLevelFeatures()
        {
            Console.WriteLine();
            Console.WriteLine("--- Gained Features at Level " + Level + " ---");

            if (Level == 2)
            {
                Console.WriteLine("  Eldritch Invocation learned!");
            }
            else if (Level == 3)
            {
                Console.WriteLine("  Pact Boon chosen: " + _subclassName);
            }
            else if (Level == 11)
            {
                Console.WriteLine("  Mystic Arcanum (6th level) unlocked!");
            }
            else if (Level == 20)
            {
                Console.WriteLine("  Eldritch Master - All invocations unlimited!");
            }

            Console.WriteLine();
        }

        private string GetPactBoonName(PactBoonType pactBoon)
        {
            switch (pactBoon)
            {
                case PactBoonType.Chain: return "Pact of the Chain";
                case PactBoonType.Blade: return "Pact of the Blade";
                case PactBoonType.Tome: return "Pact of the Tome";
                default: return "Pact of the Chain";
            }
        }
    }
}