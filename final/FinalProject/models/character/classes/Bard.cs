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
    /// Enum representing the different Colleges of Bards.
    /// </summary>
    public enum BardSubclass
    {
        CollegeOfLore,
        CollegeOfGlamour,
        CollegeOfValor,
        CollegeOfSwords
    }

    /// <summary>
    /// Enum for College of Lore bonus proficiencies.
    /// </summary>
    public enum LoreProficiency
    {
        None,
        History,
        Arcana,
        Nature,
        Religion,
        Insight,
        Medicine,
        Investigation,
        Perception
    }

    /// <summary>
    /// Enum for College of Glamour's Mantle of Majesty features.
    /// </summary>
    public enum GlamourFeature
    {
        FancifulDemestics,
        MantleOfMajesty,
        DivineDance
    }

    /// <summary>
    /// Enum for College of Valor bonus combat styles.
    /// </summary>
    public enum CombatStyle
    {
        None,
        Archery,
        BladesFlourish,
        Defense,
        Dueling,
        GreatWeaponFighting,
        LightFoehammer,
        Protection,
        TwoWeaponFighting
    }

    /// <summary>
    /// Enum for College of Swords combat styles.
    /// </summary>
    public enum SwordStyle
    {
        None,
        Mobile,
        Parry,
        Ricochet,
        Swiftness,
        Whirlwind
    }

    /// <summary>
    /// Enum for the type of blade used by College of Swords.
    /// </summary>
    public enum BladeType
    {
        Rapiers,
        Scimitars,
        Shortswords
    }

    /// <summary>
    /// Bard character class with full level 1-20 feature progression and all PHB colleges.
    /// </summary>
    public class Bard : Character
    {
        // Core bardic mechanics
        private int _bardicInspirationDie; // d6, d8, d10, or d12
        private int _bardicInspirationUses;
        private string _bardicInspirationAbility = "Charisma";

        // Spellcasting properties
        protected Dictionary<int, int> _spellSlotsByLevel;
        protected List<SpellClass> _knownSpells;
        protected List<string> _cantripsKnown;
        protected List<string> _magicalSecrets;
        protected bool _flexibleCastingActive;

        // Expertise and proficiency
        private int _expertiseSkillCount;
        private List<string> _expertisedSkills;
        private bool _versatileSkillProficienciesUnlocked;

        // Song of Rest tracking
        private int _songOfRestDie;

        // Subclass properties
        private BardSubclass _subclass;
        private string _subclassName;
        private LoreProficiency _loreProficiency1;
        private LoreProficiency _loreProficiency2;
        private GlamourFeature _glamourFeature;
        private bool _fancifulDemesticsUsed;
        private CombatStyle _valorCombatStyle;
        private SwordStyle _swordsStyle;
        private BladeType _bladeType;
        private int _rallyCount;
        private bool _rallyUsed;

        // Feature flags based on level
        private bool _fascinatingLoreActive;
        private bool _countercharmUnlocked;
        private bool _superiorCriticalUnlocked;
        private bool _superiorMasteryUsed;
        private bool _echoesOfHistoryUnlocked;
        private bool _collegeFeature2Unlocked;
        private bool _collegeFeature3Unlocked;
        private bool _collegeFeature4Unlocked;

        // College of Glamour specific
        private int _fancifulDemesticsCount;
        private bool _fancifulDemesticsActive;

        // College of Swords specific
        private string _attackTypeName;

        public Bard() : base()
        {
            InitializeBard();
        }

        public Bard(string name, int level, RaceClass race, BackgroundClass background)
            : base(name, level, race, background)
        {
            InitializeBard();
            ApplyLevelFeatures();
        }

        private void InitializeBard()
        {
            // Core bardic mechanics defaults
            _bardicInspirationDie = 6; // d6 at level 1
            _bardicInspirationUses = 2; // CHA mod (10->+0) + 1/2 at level 1 = 1, minimum 1
            _bardicInspirationAbility = "Charisma";

            // Spellcasting defaults
            _spellSlotsByLevel = new Dictionary<int, int>();
            _knownSpells = new List<SpellClass>();
            _cantripsKnown = new List<string>();
            _magicalSecrets = new List<string>();

            // Expertise defaults
            _expertiseSkillCount = 2;
            _expertisedSkills = new List<string>();
            _versatileSkillProficienciesUnlocked = false;

            // Song of Rest
            _songOfRestDie = 6; // d6 at level 1

            // Subclass defaults
            _subclass = BardSubclass.CollegeOfLore;
            _subclassName = "College of Lore";
            _loreProficiency1 = LoreProficiency.None;
            _loreProficiency2 = LoreProficiency.None;
            _glamourFeature = GlamourFeature.FancifulDemestics;
            _fancifulDemesticsUsed = false;
            _valorCombatStyle = CombatStyle.None;
            _swordsStyle = SwordStyle.None;
            _bladeType = BladeType.Rapiers;
            _rallyCount = 0;
            _rallyUsed = false;

            // Feature flags
            _fascinatingLoreActive = false;
            _countercharmUnlocked = false;
            _superiorCriticalUnlocked = false;
            _superiorMasteryUsed = false;
            _echoesOfHistoryUnlocked = false;
            _collegeFeature2Unlocked = false;
            _collegeFeature3Unlocked = false;
            _collegeFeature4Unlocked = false;

            // Glamour specifics
            _fancifulDemesticsCount = 0;
            _fancifulDemesticsActive = false;

            // Swords specifics
            _attackTypeName = "rapiers";

            // Initialize spell slots for level 1
            ApplyLevelFeatures();
        }

        // ==================== Properties ====================

        public int BardicInspirationDie { get => _bardicInspirationDie; set => _bardicInspirationDie = value; }
        public int BardicInspirationUses { get => _bardicInspirationUses; set => _bardicInspirationUses = value; }
        public string BardicInspirationAbility { get => _bardicInspirationAbility; set => _bardicInspirationAbility = value; }
        public Dictionary<int, int> SpellSlotsByLevel { get => _spellSlotsByLevel; set => _spellSlotsByLevel = value; }
        public List<SpellClass> KnownSpells { get => _knownSpells; set => _knownSpells = value; }
        public List<string> CantripsKnown { get => _cantripsKnown; set => _cantripsKnown = value; }
        public List<string> MagicalSecrets { get => _magicalSecrets; set => _magicalSecrets = value; }
        public int ExpertiseSkillCount { get => _expertiseSkillCount; set => _expertiseSkillCount = value; }
        public List<string> ExpertisedSkills { get => _expertisedSkills; set => _expertisedSkills = value; }
        public bool VersatileSkillProficienciesUnlocked { get => _versatileSkillProficienciesUnlocked; }
        public int SongOfRestDie { get => _songOfRestDie; }
        public BardSubclass Subclass { get => _subclass; set => _subclass = value; }
        public string SubclassName { get => _subclassName; set => _subclassName = value; }
        public LoreProficiency LoreProficiency1 { get => _loreProficiency1; set => _loreProficiency1 = value; }
        public LoreProficiency LoreProficiency2 { get => _loreProficiency2; set => _loreProficiency2 = value; }
        public CombatStyle ValorCombatStyle { get => _valorCombatStyle; set => _valorCombatStyle = value; }
        public SwordStyle SwordsStyle { get => _swordsStyle; set => _swordsStyle = value; }
        public BladeType BladeTypeChoice { get => _bladeType; set => _bladeType = value; }
        public int RallyCount { get => _rallyCount; }
        public bool FascinatingLoreActive { get => _fascinatingLoreActive; }
        public bool CountercharmUnlocked { get => _countercharmUnlocked; }
        public bool SuperiorCriticalUnlocked { get => _superiorCriticalUnlocked; }
        public int SpellSlotsTotal => GetTotalSpellSlots();

        // ==================== Core Methods ====================

        public override void ClassSpecificAbility()
        {
            BardicInspiration();
        }

        /// <summary>
        /// Spend a Bardic Inspiration die and add it to an ally's ability check, attack roll, or saving throw.
        /// The character can add the die as a bonus when they make an ability check, attack roll, or saving throw.
        /// </summary>
        public virtual int BardicInspiration()
        {
            if (_bardicInspirationUses <= 0)
            {
                Console.WriteLine($"{Name} has no Bardic Inspiration uses remaining. Recharge on a long rest.");
                return 0;
            }

            _bardicInspirationUses--;

            // Apply Fascinating Lore bonus if active
            int bonus = GetBardicInspirationValue();
            if (_fascinatingLoreActive)
            {
                bonus += 1;
                Console.WriteLine("Fascinating Lore adds +1 to Bardic Inspiration: " + bonus);
            }

            string loreBonus = _fascinatingLoreActive ? "+1" : "";
            Console.WriteLine($"{Name} uses Bardic Inspiration! The target can add d{_bardicInspirationDie}{loreBonus} to their next ability check, attack roll, or saving throw.");
            return bonus;
        }

        /// <summary>
        /// Gain hit points during a short rest using Song of Rest.
        /// Characters within 5 feet of you gain hit points as they gather strength.
        /// </summary>
        public virtual void SongOfRest()
        {
            if (_songOfRestDie < 6)
            {
                Console.WriteLine($"{Name} has not learned Song of Rest yet (requires level 3).");
                return;
            }

            int healing = GetBardicInspirationValue(); // Song uses same die as inspiration
            Console.WriteLine($"{Name} sings a song of rest! Allies within 5 feet regain {healing} hit points.");
        }

        // ==================== Stat Calculations ====================

        /// <summary>
        /// Override base stats calculation for Bard.
        /// Hit Points: d8 per level, AC: Dex-based (light armor), Speed: 30 ft
        /// </summary>
        protected override void CalculateBaseStats()
        {
            int conMod = Math.Max(-5, (Constitution - 10) / 2);

            // Max HP: d8 per level + con mod per level
            int hpFromFirstLevel = 8 + conMod;
            int hpFromHigherLevels = (Level - 1) * (8 + conMod);
            MaxHitPoints = hpFromFirstLevel + hpFromHigherLevels;

            if (HitPoints > MaxHitPoints || HitPoints <= 0)
            {
                HitPoints = MaxHitPoints;
            }

            // Unarmored defense not available to Bard (they use light armor proficiency)
            // Base AC with light armor: 11 + Dex mod
            int dexMod = Math.Max(-5, (Dexterity - 10) / 2);
            ArmorClass = 11 + dexMod;

            // Speed defaults to race speed (typically 30 ft)
            Speed = _race != null ? _race.Speed : 30;
        }

        // ==================== Level Feature Progression ====================

        /// <summary>
        /// Apply features gained at specific bard levels.
        /// </summary>
        private void ApplyLevelFeatures()
        {
            // ===== LEVEL 1: Starting features =====
            if (Level >= 1)
            {
                _bardicInspirationDie = 6;
                UpdateBardicInspirationUses();
                InitializeSpellSlots(1);
                AddDefaultStartingSpells();
            }

            // ===== LEVEL 2: Fascinating Lore =====
            if (Level >= 2)
            {
                _fascinatingLoreActive = true;
                AddLevelTwoSpells();
            }

            // ===== LEVEL 3: College Selection + Song of Rest (d6) =====
            if (Level >= 3)
            {
                _songOfRestDie = 6;
                _collegeFeature2Unlocked = false; // Will be set based on subclass
                UpdateKnownSpells(6);

                // Apply subclass features
                ApplySubclassFeatures();
            }

            // ===== LEVEL 4: ASI (handled by player choice) =====
            if (Level >= 4)
            {
                UpdateKnownSpells(7);
            }

            // ===== LEVEL 5: Bardic Inspiration (d8) + Song of Rest (d8) =====
            if (Level >= 5)
            {
                _bardicInspirationDie = 8;
                _songOfRestDie = 8;
                UpdateBardicInspirationUses();
                InitializeSpellSlots(5);
                AddCantripIfNeeded(true);
                UpdateKnownSpells(8);
            }

            // ===== LEVEL 6: College Feature #2 =====
            if (Level >= 6)
            {
                _collegeFeature2Unlocked = true;
                ApplyCollegeFeature2();
            }

            // ===== LEVEL 7: Countercharm =====
            if (Level >= 7)
            {
                _countercharmUnlocked = true;
                UpdateKnownSpells(9);
            }

            // ===== LEVEL 8: ASI =====
            if (Level >= 8)
            {
                InitializeSpellSlots(8);
                UpdateKnownSpells(10);
            }

            // ===== LEVEL 9: Bardic Inspiration (d10) + Song of Rest (d10) + Flexible Casting =====
            if (Level >= 9)
            {
                _bardicInspirationDie = 10;
                _songOfRestDie = 10;
                _flexibleCastingActive = true;
                UpdateBardicInspirationUses();
                InitializeSpellSlots(9);
                UpdateKnownSpells(11);
            }

            // ===== LEVEL 10: Versatile Skill Proficiencies + College Feature #3 =====
            if (Level >= 10)
            {
                _versatileSkillProficienciesUnlocked = true;
                _collegeFeature3Unlocked = true;
                ApplyCollegeFeature3();
                InitializeSpellSlots(10);
                UpdateKnownSpells(12);
            }

            // ===== LEVEL 11: Bardic Inspiration (d12) + Song of Rest (d12) + Superior Critical =====
            if (Level >= 11)
            {
                _bardicInspirationDie = 12;
                _songOfRestDie = 12;
                _superiorCriticalUnlocked = true;
                UpdateBardicInspirationUses();
                InitializeSpellSlots(11);
                UpdateKnownSpells(14);
            }

            // ===== LEVEL 12: ASI =====
            if (Level >= 12)
            {
                InitializeSpellSlots(12);
                UpdateKnownSpells(15);
            }

            // ===== LEVEL 13: Magical Secrets (first 2 spells) =====
            if (Level >= 13)
            {
                InitializeSpellSlots(13);
                UpdateKnownSpells(15);
            }

            // ===== LEVEL 14: College Feature #4 =====
            if (Level >= 14)
            {
                _collegeFeature4Unlocked = true;
                ApplyCollegeFeature4();
                InitializeSpellSlots(14);
                UpdateKnownSpells(16);
            }

            // ===== LEVEL 15: Song of Rest already at d12 =====
            if (Level >= 15)
            {
                InitializeSpellSlots(15);
                UpdateKnownSpells(18);
            }

            // ===== LEVEL 16: ASI =====
            if (Level >= 16)
            {
                InitializeSpellSlots(16);
                UpdateKnownSpells(18);
            }

            // ===== LEVEL 17: Magical Secrets (second 2 spells) =====
            if (Level >= 17)
            {
                InitializeSpellSlots(17);
                UpdateKnownSpells(19);
            }

            // ===== LEVEL 18: Superior Mastery =====
            if (Level >= 18)
            {
                _superiorMasteryUsed = false; // Reset for each long rest
                InitializeSpellSlots(18);
                UpdateKnownSpells(20);
            }

            // ===== LEVEL 19: ASI =====
            if (Level >= 19)
            {
                InitializeSpellSlots(19);
                UpdateKnownSpells(20);
            }

            // ===== LEVEL 20: Echoes of History =====
            if (Level >= 20)
            {
                _echoesOfHistoryUnlocked = true;
                InitializeSpellSlots(20);
                UpdateKnownSpells(22);
            }
        }

        private void ApplySubclassFeatures()
        {
            switch (_subclass)
            {
                case BardSubclass.CollegeOfLore:
                    _subclassName = "College of Lore";
                    Console.WriteLine($"{Name} chooses the College of Lore!");
                    break;
                case BardSubclass.CollegeOfGlamour:
                    _subclassName = "College of Glamour";
                    Console.WriteLine($"{Name} chooses the College of Glamour!");
                    break;
                case BardSubclass.CollegeOfValor:
                    _subclassName = "College of Valor";
                    Console.WriteLine($"{Name} chooses the College of Valor!");
                    break;
                case BardSubclass.CollegeOfSwords:
                    _subclassName = "College of Swords";
                    Console.WriteLine($"{Name} chooses the College of Swords!");
                    break;
            }
        }

        private void ApplyCollegeFeature2()
        {
            switch (_subclass)
            {
                case BardSubclass.CollegeOfLore:
                    _subclassName += " - Bonus Proficiencies";
                    Console.WriteLine("Cutting Words: When a creature deals damage, you can use your reaction to subtract from that damage (uses CHA mod + bardic level).");
                    break;
                case BardSubclass.CollegeOfGlamour:
                    _subclassName += " - Mantle of Glamour";
                    Console.WriteLine("Unbreakable Mind: You gain resistance to psychic damage and immunity to being charmed.");
                    break;
                case BardSubclass.CollegeOfValor:
                    _subclassName += " - Battle Music";
                    Console.WriteLine("War Song: As a bonus action, you can deal thunder damage equal to your bardic inspiration die + CHA mod to creatures within 5 feet.");
                    break;
                case BardSubclass.CollegeOfSwords:
                    _subclassName += " - Sword Board";
                    Console.WriteLine("Blade Flourish: When you hit with a melee weapon attack, you can use your reaction to add blade flourish damage and effect.");
                    break;
            }
        }

        private void ApplyCollegeFeature3()
        {
            switch (_subclass)
            {
                case BardSubclass.CollegeOfLore:
                    _subclassName += " - Additional Lore";
                    Console.WriteLine("Additional Lore: You learn two additional skill proficiencies.");
                    break;
                case BardSubclass.CollegeOfGlamour:
                    _subclassName += " - Divine Dance";
                    Console.WriteLine("Divine Dance: As a bonus action, you can teleport up to 30 feet to an unoccupied space.");
                    break;
                case BardSubclass.CollegeOfValor:
                    _subclassName += " - Extra Attack";
                    Console.WriteLine("Extra Attack: You can attack twice when you use the Attack action on your turn.");
                    break;
                case BardSubclass.CollegeOfSwords:
                    _subclassName += " - Homespun Whirl";
                    Console.WriteLine("Homespun Whirl: When you hit a creature with a melee weapon attack, you can use your bonus action to make another melee weapon attack.");
                    break;
            }
        }

        private void ApplyCollegeFeature4()
        {
            switch (_subclass)
            {
                case BardSubclass.CollegeOfLore:
                    _subclassName += " - Peerless Skill";
                    Console.WriteLine("Peerless Skill: You can add twice your proficiency bonus to any ability check you're proficient in.");
                    break;
                case BardSubclass.CollegeOfGlamour:
                    _subclassName += " - Hostile Rause";
                    Console.WriteLine("Hostile Rause: When a creature within 30 feet hurts an ally, you can use your reaction to force the creature to make a WIS save or take psychic damage.");
                    break;
                case BardSubclass.CollegeOfValor:
                    _subclassName += " - Combat Muse";
                    Console.WriteLine("Combat Muse: As a bonus action, you can grant an ally bardic inspiration (no action required on their part).");
                    break;
                case BardSubclass.CollegeOfSwords:
                    _subclassName += " - Velocious Blade";
                    Console.WriteLine("Velocious Blade: When you use your blade flourish, you can move up to half your speed without provoking opportunity attacks.");
                    break;
            }
        }

        /// <summary>
        /// Check and apply level-up features when the bard levels up.
        /// </summary>
        public void OnLevelUp()
        {
            ApplyLevelFeatures();
            UpdateBardicInspirationUses();
            Console.WriteLine($"{Name} gains new features at level {Level}!");
            DisplayLevelFeatures();
        }

        // ==================== Spellcasting Methods ====================

        /// <summary>
        /// Learn a spell and add it to the known spells list.
        /// Bards learn spells from the official Bard spell list, plus Magical Secrets.
        /// </summary>
        public virtual void LearnSpell(SpellClass spell)
        {
            if (!_knownSpells.Contains(spell))
            {
                _knownSpells.Add(spell);
                Console.WriteLine($"{Name} has learned the spell: {spell.Name} (Level {spell.Level})");
            }
            else
            {
                Console.WriteLine($"{Name} already knows the spell: {spell.Name}");
            }
        }

        /// <summary>
        /// Add a spell from any class via Magical Secrets.
        /// </summary>
        public virtual void LearnMagicalSecrets(string spell1, string spell2)
        {
            _magicalSecrets.Add(spell1);
            _magicalSecrets.Add(spell2);
            Console.WriteLine($"{Name} learns Magical Secrets: {spell1} and {spell2} from any class.");

            // Create placeholder spells for the magical secrets
            SpellClass spell1Placeholder = new SpellClass(spell1, 3, School.Transmutation, "1 action", "Self", "Instantaneous");
            SpellClass spell2Placeholder = new SpellClass(spell2, 3, School.Transmutation, "1 action", "Self", "Instantaneous");
            _knownSpells.Add(spell1Placeholder);
            _knownSpells.Add(spell2Placeholder);
        }

        /// <summary>
        /// Add a spell to known spells from the bard spell list.
        /// </summary>
        public virtual void AddSpellToKnownList(string spellName, int spellLevel)
        {
            School school = GetSpellSchool(spellLevel);
            SpellClass newSpell = new SpellClass(spellName, spellLevel, school, "1 action", "Self", "Instantaneous");
            _knownSpells.Add(newSpell);
            Console.WriteLine($"{Name} adds {spellName} to known spells.");
        }

        /// <summary>
        /// Forget a learned spell.
        /// </summary>
        public virtual void ForgetSpell(string spellName)
        {
            SpellClass? spellToForget = _knownSpells.Find(s => s.Name == spellName);
            if (spellToForget != null)
            {
                _knownSpells.Remove(spellToForget);
                Console.WriteLine($"{Name} has forgotten the spell: {spellName}");
            }
            else
            {
                Console.WriteLine($"{Name} does not know the spell: {spellName}");
            }
        }

        /// <summary>
        /// Use a spell slot to cast a known spell.
        /// </summary>
        public virtual bool CastSpell(string spellName, int spellSlotLevel)
        {
            SpellClass? spell = _knownSpells.Find(s => s.Name == spellName);
            if (spell == null)
            {
                Console.WriteLine($"{Name} does not know the spell: {spellName}");
                return false;
            }

            if (!_spellSlotsByLevel.ContainsKey(spellSlotLevel) || _spellSlotsByLevel[spellSlotLevel] <= 0)
            {
                Console.WriteLine($"{Name} has no spell slots of level {spellSlotLevel}.");
                return false;
            }

            _spellSlotsByLevel[spellSlotLevel]--;
            Console.WriteLine($"{Name} casts {spellName} using a {spellSlotLevel}-level spell slot.");
            return true;
        }

        /// <summary>
        /// Create spell slots via Flexible Casting (level 9+).
        /// </summary>
        public virtual void CreateSpellSlots(int level, int count)
        {
            if (!_flexibleCastingActive)
            {
                Console.WriteLine($"{Name} has not learned Flexible Casting (requires level 9).");
                return;
            }

            if (!_spellSlotsByLevel.ContainsKey(level))
            {
                _spellSlotsByLevel[level] = 0;
            }
            _spellSlotsByLevel[level] += count;
            Console.WriteLine($"{Name} creates {count} level {level} spell slot(s) via Flexible Casting.");
        }

        /// <summary>
        /// Convert a spell slot back to HP via Flexible Casting.
        /// </summary>
        public virtual int ConvertSpellSlotToHp(int spellSlotLevel)
        {
            if (!_flexibleCastingActive)
            {
                Console.WriteLine($"{Name} has not learned Flexible Casting (requires level 9).");
                return 0;
            }

            if (!_spellSlotsByLevel.ContainsKey(spellSlotLevel) || _spellSlotsByLevel[spellSlotLevel] <= 0)
            {
                Console.WriteLine($"{Name} has no spell slots of level {spellSlotLevel} to convert.");
                return 0;
            }

            _spellSlotsByLevel[spellSlotLevel]--;
            int hpGain = spellSlotLevel * 5;
            Console.WriteLine($"{Name} converts a {spellSlotLevel}-level spell slot into {hpGain} HP.");
            return hpGain;
        }

        /// <summary>
        /// Get the total number of remaining spell slots.
        /// </summary>
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
        /// Initialize spell slots based on the bard's current level using full caster progression.
        /// </summary>
        private void InitializeSpellSlots(int level)
        {
            _spellSlotsByLevel.Clear();

            // Full caster spell slot progression (per D&D 5e PHB p.201)
            if (level >= 1 && level <= 4)
            {
                _spellSlotsByLevel[1] = (level == 1) ? 2 : (level == 2 ? 3 : (level == 3 ? 4 : 4));
            }
            else if (level >= 5 && level <= 10)
            {
                _spellSlotsByLevel[1] = 4;
                _spellSlotsByLevel[2] = level <= 7 ? 3 : 4;
                if (level >= 5) _spellSlotsByLevel[3] = level <= 7 ? 2 : 3;
                if (level >= 9) _spellSlotsByLevel[4] = 1;
                if (level >= 10) _spellSlotsByLevel[4] = 2;
            }
            else if (level >= 11 && level <= 16)
            {
                _spellSlotsByLevel[1] = 4;
                _spellSlotsByLevel[2] = 3;
                _spellSlotsByLevel[3] = 3;
                _spellSlotsByLevel[4] = level <= 13 ? 3 : 4;
                if (level >= 11) _spellSlotsByLevel[5] = 1;
                if (level >= 13) _spellSlotsByLevel[5] = 2;
                if (level >= 15) _spellSlotsByLevel[6] = 1;
                if (level >= 16) _spellSlotsByLevel[6] = 2;
            }
            else if (level >= 17 && level <= 20)
            {
                _spellSlotsByLevel[1] = 4;
                _spellSlotsByLevel[2] = 3;
                _spellSlotsByLevel[3] = 3;
                _spellSlotsByLevel[4] = 3;
                _spellSlotsByLevel[5] = 3;
                if (level >= 17) _spellSlotsByLevel[6] = 1;
                if (level >= 17) _spellSlotsByLevel[7] = 1;
                if (level >= 19) _spellSlotsByLevel[8] = 1;
                if (level >= 20) _spellSlotsByLevel[9] = 1;
            }

            // Update maximum spell slot levels based on bard level
            int maxSpellSlotLevel = Math.Min(9, (level - 1) / 2); // Level 1-2: 1st, 3-4: 2nd, etc.
            if (maxSpellSlotLevel < 1) maxSpellSlotLevel = 1;

            // Ensure all spell slots up to max level exist
            for (int i = 1; i <= maxSpellSlotLevel; i++)
            {
                if (!_spellSlotsByLevel.ContainsKey(i))
                {
                    _spellSlotsByLevel[i] = 0;
                }
            }
        }

        /// <summary>
        /// Update Bardic Inspiration uses based on level and CHA modifier.
        /// Uses: CHA mod + bard level / 2 (minimum 1)
        /// </summary>
        private void UpdateBardicInspirationUses()
        {
            int chaMod = Math.Max(-5, (Charisma - 10) / 2);
            int uses = chaMod + (Level / 2);
            _bardicInspirationUses = Math.Max(1, uses);
        }

        /// <summary>
        /// Get the Bardic Inspiration die value.
        /// Returns the numeric value of the die (6, 8, 10, or 12).
        /// </summary>
        private int GetBardicInspirationValue()
        {
            switch (_bardicInspirationDie)
            {
                case 6: return 6;
                case 8: return 8;
                case 10: return 10;
                case 12: return 12;
                default: return 6;
            }
        }

        /// <summary>
        /// Get the Spell school for a given spell level (for Magical Secrets and other features).
        /// </summary>
        private School GetSpellSchool(int spellLevel)
        {
            // Bards can cast spells from all schools, so return Transmutation as default
            return School.Transmutation;
        }

        /// <summary>
        /// Add default starting spells at level 1.
        /// </summary>
        private void AddDefaultStartingSpells()
        {
            // Standard level 1 Bard starting spells (player can choose which ones)
            List<string> defaultCantrips = new List<string>
            {
                "Friends", "Minor Illusion", "Prestidigitation", "Thunderwave"
            };

            List<string> defaultFirstLevelSpells = new List<string>
            {
                "Cure Wounds", "Healing Word", "Charm Person", "Disguise Self"
            };

            // Add cantrips to known list (stored as strings for tracking)
            foreach (var cantrip in defaultCantrips)
            {
                _cantripsKnown.Add(cantrip);
            }

            // Add first-level spells to known list
            foreach (var spell in defaultFirstLevelSpells)
            {
                SpellClass newSpell = new SpellClass(spell, 1, School.Enchantment, "1 action", "Self", "Instantaneous");
                _knownSpells.Add(newSpell);
            }

            Console.WriteLine($"{Name} starts with cantrips: {string.Join(", ", defaultCantrips)}");
            Console.WriteLine($"{Name} starts with spells: {string.Join(", ", defaultFirstLevelSpells)}");
        }

        /// <summary>
        /// Add level 2+ spells to known list based on subclass.
        /// </summary>
        private void AddLevelTwoSpells()
        {
            // College of Lore gets additional lore spells
            if (_subclass == BardSubclass.CollegeOfLore)
            {
                Console.WriteLine("College of Lore: You can choose spells from any class's spell list.");
            }
        }

        /// <summary>
        /// Update known spells count based on level.
        /// </summary>
        private void UpdateKnownSpells(int totalSpells)
        {
            // This ensures the known spells count matches the expected progression
            // Player chooses which specific spells to add at each level
            Console.WriteLine($"{Name} can now know up to {totalSpells} spells.");
        }

        /// <summary>
        /// Add cantrip based on level progression.
        /// </summary>
        private void AddCantripIfNeeded(bool shouldAdd)
        {
            if (shouldAdd)
            {
                string newCantrip = "Light"; // Placeholder - player chooses
                _cantripsKnown.Add(newCantrip);
                Console.WriteLine($"{Name} learns a new cantrip: {newCantrip}");
            }
        }

        /// <summary>
        /// Check if flexible casting is active.
        /// </summary>
        public bool HasFlexibleCasting => _flexibleCastingActive;

        // ==================== Feature Methods ====================

        /// <summary>
        /// Use Countercharm: Remove frightened and charmed conditions from creatures within 30 feet.
        /// (Level 7+)
        /// </summary>
        public virtual void Countercharm()
        {
            if (!_countercharmUnlocked)
            {
                Console.WriteLine($"{Name} has not learned Countercharm yet (requires level 7).");
                return;
            }

            Console.WriteLine($"{Name} plays a powerful melody. Creatures within 30 feet that are frightened or charmed are removed of those conditions.");
        }

        /// <summary>
        /// Use Superior Critical: When you roll damage for Bardic Inspiration, the die becomes its maximum value.
        /// (Level 11+)
        /// </summary>
        public virtual int UseSuperiorCritical(int baseDamage)
        {
            if (!_superiorCriticalUnlocked)
            {
                return baseDamage;
            }

            Console.WriteLine("Superior Critical! Bardic Inspiration die rolls maximum damage!");
            return baseDamage + GetBardicInspirationValue(); // Add full inspiration value
        }

        /// <summary>
        /// Use Superior Mastery: Use Bardic Inspiration without expending a use (once per long rest).
        /// (Level 18+)
        /// </summary>
        public virtual bool UseSuperiorMastery()
        {
            if (!_superiorMasteryUsed)
            {
                _superiorMasteryUsed = true;
                Console.WriteLine($"Superior Mastery! {Name} uses Bardic Inspiration without expending a use.");
                return true;
            }

            Console.WriteLine($"{Name} has already used Superior Mastery today. Recharge on long rest.");
            return false;
        }

        /// <summary>
        /// Use Echoes of History: When you roll damage for Bardic Inspiration, it becomes maximum value and you regain the use.
        /// (Level 20)
        /// </summary>
        public virtual int UseEchoesOfHistory(int baseDamage)
        {
            if (!_echoesOfHistoryUnlocked)
            {
                return baseDamage;
            }

            _bardicInspirationUses++; // Regain the use
            Console.WriteLine("Echoes of History! Bardic Inspiration deals maximum damage and you regain the use!");
            return GetBardicInspirationValue();
        }

        /// <summary>
        /// Select a Lore proficiency for College of Bards.
        /// </summary>
        public virtual void SelectLoreProficiency(LoreProficiency proficiency, int slot)
        {
            if (_subclass != BardSubclass.CollegeOfLore)
            {
                Console.WriteLine($"{Name} is not a College of Lore bard.");
                return;
            }

            if (slot == 1)
            {
                _loreProficiency1 = proficiency;
                Console.WriteLine($"{Name} selects {_loreProficiency1} as their first Lore Proficiency.");
            }
            else if (slot == 2)
            {
                _loreProficiency2 = proficiency;
                Console.WriteLine($"{Name} selects {_loreProficiency2} as their second Lore Proficiency.");
            }
        }

        /// <summary>
        /// Select a Combat Style for College of Valor.
        /// </summary>
        public virtual void SelectCombatStyle(CombatStyle style)
        {
            if (_subclass != BardSubclass.CollegeOfValor)
            {
                Console.WriteLine($"{Name} is not a College of Valor bard.");
                return;
            }

            _valorCombatStyle = style;
            Console.WriteLine($"{Name} selects {_valorCombatStyle} as their combat style.");
        }

        /// <summary>
        /// Select a Sword Style for College of Swords.
        /// </summary>
        public virtual void SelectSwordStyle(SwordStyle style, BladeType blade)
        {
            if (_subclass != BardSubclass.CollegeOfSwords)
            {
                Console.WriteLine($"{Name} is not a College of Swords bard.");
                return;
            }

            _swordsStyle = style;
            _bladeType = blade;
            Console.WriteLine($"{Name} selects {_swordsStyle} and {_bladeType} as their sword choices.");
        }

        /// <summary>
        /// Rally: College of Valor's level 3 feature. Gain temporary HP equal to Bard level + CHA mod.
        /// (Level 3+)
        /// </summary>
        public virtual int Rally()
        {
            if (_subclass != BardSubclass.CollegeOfValor)
            {
                Console.WriteLine($"{Name} is not a College of Valor bard.");
                return 0;
            }

            int chaMod = Math.Max(-5, (Charisma - 10) / 2);
            int tempHp = Level + chaMod;
            _rallyCount += tempHp;
            Console.WriteLine($"{Name} rallies! Gain {tempHp} temporary hit points.");
            return tempHp;
        }

        /// <summary>
        /// Fanciful Demestics: College of Glamour's level 3 feature.
        /// Create up to six FAIRIES (fanciful demestics) that can attack or assist.
        /// </summary>
        public virtual void UseFancifulDemestics()
        {
            if (_subclass != BardSubclass.CollegeOfGlamour)
            {
                Console.WriteLine($"{Name} is not a College of Glamour bard.");
                return;
            }

            _fancifulDemesticsCount = 6; // Six fanciful demestics
            _fancifulDemesticsActive = true;
            Console.WriteLine($"{Name} creates six fanciful demestics! They can attack or assist allies.");
        }

        /// <summary>
        /// Check if the bard has a specific feature unlocked.
        /// </summary>
        public bool HasFeature(string featureName)
        {
            switch (featureName.ToLower())
            {
                case "bardic inspiration": return true;
                case "song of rest": return Level >= 3;
                case "countercharm": return _countercharmUnlocked;
                case "superior critical": return _superiorCriticalUnlocked;
                case "flexible casting": return _flexibleCastingActive;
                case "versatile skill proficiencies": return _versatileSkillProficienciesUnlocked;
                case "echoes of history": return _echoesOfHistoryUnlocked;
                case "magical secrets": return Level >= 13 || Level >= 17;
                default: return false;
            }
        }

        // ==================== Override Base Methods ====================

        public override void Attack()
        {
            Console.WriteLine($"{Name} makes a weapon attack with their bardic tools.");
            if (_swordsStyle != SwordStyle.None)
            {
                Console.WriteLine($"College of Swords style {_swordsStyle} is active, enhancing the attack.");
            }
        }

        public override void TakeDamage(int damage)
        {
            // Apply Unbreakable Mind resistance (College of Glamour, level 6+)
            if (_subclass == BardSubclass.CollegeOfGlamour && _collegeFeature2Unlocked)
            {
                // Resistance to psychic damage only
                Console.WriteLine($"{Name}'s Unbreakable Mind grants resistance to psychic damage.");
            }

            base.TakeDamage(damage);
        }

        public override void Heal(int amount)
        {
            // Magical Secrets can include healing spells from other classes
            base.Heal(amount);
        }

        public override void ShortRest()
        {
            base.ShortRest();

            // Bard: Regain some spell slots on short rest if using the variant rule
            // Standard rules: no slot recovery, but you can roleplay Song of Rest
            Console.WriteLine($"{Name} regains some vigor through a short rest.");
        }

        public override void LongRest()
        {
            base.LongRest();

            // Regain all Bardic Inspiration uses
            UpdateBardicInspirationUses();

            // Regain all spell slots
            InitializeSpellSlots(Level);

            // Reset long-rest-only features
            _superiorMasteryUsed = false;
            _fancifulDemesticsUsed = false;

            Console.WriteLine($"{Name} regains all Bardic Inspiration uses, spell slots, and feature uses after a long rest.");
        }

        public override void DisplayCharacter()
        {
            int chaMod = Math.Max(-5, (Charisma - 10) / 2);
            int totalSlots = GetTotalSpellSlots();

            Console.WriteLine($"\n{'=',60}");
            Console.WriteLine($"=== {Name} (Level {_level} Bard - {_subclass.ToString().Replace("CollegeOf", "")}) ===");
            Console.WriteLine($"{'-',60}");
            Console.WriteLine($"Hit Points: {HitPoints}/{MaxHitPoints} | AC: {ArmorClass} | Speed: {Speed}");
            Console.WriteLine($"Bardic Inspiration: d{_bardicInspirationDie} ({_bardicInspirationUses} uses remaining)");
            Console.WriteLine($"Song of Rest: d{_songOfRestDie}");
            Console.WriteLine($"Spell Slots: {totalSlots} total | Known Spells: {_knownSpells.Count} | Cantrips: {_cantripsKnown.Count}");

            // Spell slot breakdown
            if (_spellSlotsByLevel != null && _spellSlotsByLevel.Count > 0)
            {
                Console.WriteLine("Spell Slots by Level:");
                foreach (var slot in _spellSlotsByLevel)
                {
                    if (slot.Value > 0)
                    {
                        Console.WriteLine($"  Level {slot.Key}: {slot.Value} remaining");
                    }
                }
            }

            Console.WriteLine("\nAbility Scores:");
            Console.WriteLine($"  Strength:    {Strength} (mod +{Math.Max(-5, (Strength - 10) / 2)})");
            Console.WriteLine($"  Dexterity:   {Dexterity} (mod +{Math.Max(-5, (Dexterity - 10) / 2)})");
            Console.WriteLine($"  Constitution:{Constitution} (mod +{Math.Max(-5, (Constitution - 10) / 2)})");
            Console.WriteLine($"  Intelligence:{Intelligence} (mod +{Math.Max(-5, (Intelligence - 10) / 2)})");
            Console.WriteLine($"  Wisdom:      {Wisdom} (mod +{Math.Max(-5, (Wisdom - 10) / 2)})");
            Console.WriteLine($"  Charisma:    {Charisma} (mod +{chaMod}) <-- Primary Casting Ability");

            // Subclass info
            DisplaySubclassInfo();

            // Known spells summary
            Console.WriteLine("\n=== Known Spells ===");
            if (_knownSpells.Count > 0)
            {
                foreach (var spell in _knownSpells)
                {
                    Console.WriteLine($"  - {spell.Name} (Level {spell.Level})");
                }
            }
            else
            {
                Console.WriteLine("  No spells known yet.");
            }

            // Magical Secrets
            if (_magicalSecrets.Count > 0)
            {
                Console.WriteLine("\n=== Magical Secrets ===");
                foreach (var secret in _magicalSecrets)
                {
                    Console.WriteLine($"  - {secret}");
                }
            }

            Console.WriteLine("\n=== End Character Sheet ===\n");
        }

        private void DisplayLevelFeatures()
        {
            Console.WriteLine("=== New Features Unlocked ===");

            if (_bardicInspirationDie > 6)
            {
                Console.WriteLine($"- Bardic Inspiration die upgraded to d{_bardicInspirationDie}");
            }

            if (_songOfRestDie > 6)
            {
                Console.WriteLine($"- Song of Rest upgraded to d{_songOfRestDie}");
            }

            if (_fascinatingLoreActive)
            {
                Console.WriteLine("- Fascinating Lore: +1 to Bardic Inspiration checks");
            }

            if (_countercharmUnlocked)
            {
                Console.WriteLine("- Countercharm: Remove frightened/charmed from allies within 30 feet");
            }

            if (_superiorCriticalUnlocked)
            {
                Console.WriteLine("- Superior Critical: Crit on 18-20 with Bardic Insp bonus attack");
            }

            if (_flexibleCastingActive)
            {
                Console.WriteLine("- Flexible Casting: Create or use spell slots for HP");
            }

            if (_versatileSkillProficienciesUnlocked)
            {
                Console.WriteLine("- Versatile Skill Proficiencies: All non-magic skills use Charisma");
            }

            if (_superiorMasteryUsed == false && Level >= 18)
            {
                Console.WriteLine("- Superior Mastery: Use Bardic Inspiration without expending a use (once per long rest)");
            }

            if (_echoesOfHistoryUnlocked)
            {
                Console.WriteLine("- Echoes of History: Maximum Bardic Insp damage and regain the use");
            }
        }

        private void DisplaySubclassInfo()
        {
            Console.WriteLine("\n--- Subclass Features ---");

            switch (_subclass)
            {
                case BardSubclass.CollegeOfLore:
                    Console.WriteLine("College of Lore:");
                    if (_loreProficiency1 != LoreProficiency.None)
                    {
                        Console.WriteLine($"  Bonus Proficiencies: {_loreProficiency1}, {_loreProficiency2}");
                    }
                    if (_collegeFeature2Unlocked)
                    {
                        Console.WriteLine("  Cutting Words: Use reaction to subtract from enemy damage rolls.");
                    }
                    if (_collegeFeature3Unlocked)
                    {
                        Console.WriteLine("  Additional Lore: Two additional skill proficiencies.");
                    }
                    if (_collegeFeature4Unlocked)
                    {
                        Console.WriteLine("  Peerless Skill: Double proficiency bonus to any checked skill.");
                    }
                    break;

                case BardSubclass.CollegeOfGlamour:
                    Console.WriteLine("College of Glamour:");
                    if (_collegeFeature2Unlocked)
                    {
                        Console.WriteLine("  Unbreakable Mind: Resistance to psychic, immune to charmed.");
                    }
                    if (_collegeFeature3Unlocked)
                    {
                        Console.WriteLine("  Divine Dance: Bonus action teleport up to 30 feet.");
                    }
                    if (_collegeFeature4Unlocked)
                    {
                        Console.WriteLine("  Hostile Rause: Force creatures to make WIS saves or take psychic damage.");
                    }
                    break;

                case BardSubclass.CollegeOfValor:
                    Console.WriteLine("College of Valor:");
                    if (_valorCombatStyle != CombatStyle.None)
                    {
                        Console.WriteLine($"  Combat Style: {_valorCombatStyle}");
                    }
                    if (_collegeFeature2Unlocked)
                    {
                        Console.WriteLine("  Battle Music: Bonus action thunder damage to nearby enemies.");
                    }
                    if (_collegeFeature3Unlocked)
                    {
                        Console.WriteLine("  Extra Attack: Attack twice when using the Attack action.");
                    }
                    if (_collegeFeature4Unlocked)
                    {
                        Console.WriteLine("  Combat Muse: Grant bardic inspiration as a bonus action.");
                    }
                    break;

                case BardSubclass.CollegeOfSwords:
                    Console.WriteLine("College of Swords:");
                    if (_swordsStyle != SwordStyle.None)
                    {
                        Console.WriteLine($"  Sword Style: {_swordsStyle} with {_bladeType}");
                    }
                    if (_collegeFeature2Unlocked)
                    {
                        Console.WriteLine("  Blade Flourish: Add blade flourish damage and effect to attacks.");
                    }
                    if (_collegeFeature3Unlocked)
                    {
                        Console.WriteLine("  Homespun Whirl: Bonus attack after hitting with melee weapon.");
                    }
                    if (_collegeFeature4Unlocked)
                    {
                        Console.WriteLine("  Velocious Blade: Move without provoking opportunity attacks on blade flourish.");
                    }
                    break;
            }

            // Common features display
            if (_fascinatingLoreActive)
            {
                Console.WriteLine("Fascinating Lore: +1 to Bardic Inspiration checks.");
            }

            if (_countercharmUnlocked)
            {
                Console.WriteLine("Countercharm: Remove frightened/charmed from allies within 30 feet.");
            }
        }

        // ==================== Official PHB Spell List for Bards ====================

        /// <summary>
        /// Get the complete list of official Bard spells from the Player's Handbook.
        /// Organized by spell level (1-9).
        /// </summary>
        public static Dictionary<int, List<string>> GetOfficialBardSpellList()
        {
            return new Dictionary<int, List<string>>
            {
                // Cantrips (0th level)
                [0] = new List<string>
                {
                    "Friends", "Minor Illusion", "Prestidigitation", "Thunderwave",
                    "Blade Ward", "Chaos Bolt", "Dancing Lights", "Fire Bolt",
                    "Light", "Mending", "Message", "Mind Sliver", "Poison Spray",
                    "Polymorph", "Ray of Sickness", "Shape Water", "Shocking Grasp",
                    "Sword Burst", "Thunderclap", "True Strike", "Tsunami"
                },

                // 1st level spells
                [1] = new List<string>
                {
                    "Animal Friendship", "Bane", "Command", "Comprehend Languages",
                    "Detect Magic", "Disguise Self", "Ensnaring Strike", "Fog Cloud",
                    "Forbiddance", "Faerie Fire", "Find Familiar", "Goodberry",
                    "Healing Word", "Hail of Thorns", "Identify", "Illusory Script",
                    "Longstrider", "Protection from Evil and Good", "Thunderwave",
                    "Unseen Servant", "Witch Bolt", "Burning Hands", "Charm Person",
                    "Cure Wounds", "Earth Tremor", "Expeditious Retreat", "Feather Fall",
                    "Find Mine", "Fog Cloud", "Friends", "Grease", "Hellish Rebuke",
                    "Heroism", "Hungry Pit", "Ice Knife", "Jolt Lightning", "Judgement of Hevav",
                    "Lightning Arrow", "Mage Armor", "Mask of Many Faces", "Protection from Poison",
                    "Ray of Sickness", "Shield", "Sleep", "Tasha's Hideous Laughter",
                    "Tongues", "Unseen Servant", "Witch Bolt"
                },

                // 2nd level spells
                [2] = new List<string>
                {
                    "Animal Messenger", "Arcane Eye", "Augury", "Beacon of Hope",
                    "Birds of Prey", "Blindness/Deafness", "Calm Emotions", "Cloud of Daggers",
                    "Continual Flame", "Control Water", "Darkvision", "Enhance Ability",
                    "Enthrall", "Gentle Repose", "Gust of Wind", "Heat Metal",
                    "Hunger of Hadar", "Invisibility", "Knock", "Lesser Restoration",
                    "Locute Animals", "Magic Mouth", "May of Fire", "Mirror Image",
                    "Misty Step", "Phantasmal Force", "Rope Trick", "Scorching Ray",
                    "See Invisibility", "Shatter", "Spider Climb", "Suggestion",
                    "Swift Quiver", "Whirlwind", "Wind Wall"
                },

                // 3rd level spells
                [3] = new List<string>
                {
                    "Animate Dead", "Anti-Life Shell", "Bestow Curse", "Clairvoyance",
                    "Dispel Magic", "Dream", "Fly", "Gaseous Form",
                    "Hallow", "Hypnotic Pattern", "Leomund's Secret Chest", "Leomund's Tiny Hut",
                    "Major Image", "Link of Truth", "Nystul's Magic Aura", "Plant Growth",
                    "Sending", "Sleet Storm", "Slow", "Spell Turning",
                    "Stinking Cloud", "Tongues", "Vampiric Touch"
                },

                // 4th level spells
                [4] = new List<string>
                {
                    "Banishment", "Confusion", "Dimension Door", "Fabricate",
                    "Freedom of Movement", "Greater Invisibility", "Hallucinatory Terrain", "Polymorph",
                    "Stone Shape", "Stoneskin", "Wrathful Smite"
                },

                // 5th level spells
                [5] = new List<string>
                {
                    "Animate Objects", "Awaken", "Dominate Person", "Dream of the Blue Veil",
                    "Hold Monster", "Insect Plague", "Legend Lore", "Mislead",
                    "Modify Memory", "Passwall", "Planar Binding", "Rary's Telepathic Bond",
                    "Scrying", "Seeming", "Telekinesis", "Telepathy",
                    "True Seeing"
                },

                // 6th level spells
                [6] = new List<string>
                {
                    "Find the Path", "Forbid Action", "Globe of Invulnerability", "Guards and Wards",
                    "Mass Suggestion", "Otiluke's Freezing Sphere", "Programed Illusion", "Sunbeam",
                    "Wall of Force"
                },

                // 7th level spells
                [7] = new List<string>
                {
                    "Delay Metamagics", "Finger of Death", "Forcecage", "Miracle",
                    "Plane Shift", "Prismatic Spray", "Project Image", "Regenerate",
                    "Resurrection"
                },

                // 8th level spells
                [8] = new List<string>
                {
                    "Animal Shapes", "Antipathy/Sympathy", "Clay Golem", "Dominate Monster",
                    "Enervation", "Feeblemind", "Incendiary Cloud", "Sunburst"
                },

                // 9th level spells
                [9] = new List<string>
                {
                    "Astral Projection", "Barbed Stone", "Foresight", "Gate",
                    "Imprisonment", "Mass Heal", "Meteor Swarm", "Power Word Heal",
                    "Power Word Stun", "Programmed Illusion", "Time Stop", "True Polymorph",
                    "Wayward Compass"
                }
            };
        }
    }
}