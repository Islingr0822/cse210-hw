using System;
using System.Collections.Generic;
using System.Linq;
using DnDCharacterManager.Ability;
using FeatureClass = DnDCharacterManager.Feature.Feature;
using RaceClass = DnDCharacterManager.Race.Race;
using BackgroundClass = DnDCharacterManager.Background.Background;
using SpellClass = DnDCharacterManager.Spell.Spell;
using School = DnDCharacterManager.Spell.School;
using DamageType = DnDCharacterManager.Spell.DamageType;
using SpellComponent = DnDCharacterManager.Spell.SpellComponent;

namespace DnDCharacterManager.Character.Classes
{
    /// <summary>
    /// Enum representing the 8 Arcane Traditions for Wizards.
    /// Each tradition grants unique features at levels 2, 6, 10, and 14.
    /// </summary>
    public enum ArcaneTradition
    {
        Evocation,
        Necromancy,
        Transmutation,
        Abjuration,
        Divination,
        Conjuration,
        Enchantment,
        Illusion
    }

    /// <summary>
    /// Wizard character class - the premier arcane spellcaster who learns and prepares spells from a spellbook.
    /// Full level 1-20 progression with tradition-specific features.
    /// </summary>
    public class Wizard : Character
    {
        // ==================== Enums ====================

        /// <summary>
        /// The chosen Arcane Tradition determining subclass features.
        /// </summary>
        public ArcaneTradition Tradition { get; set; }

        // ==================== Core Properties ====================

        /// <summary>
        /// Spellcasting ability for Wizards is Intelligence.
        /// </summary>
        public string SpellCastingAbility => "Intelligence";

        /// <summary>
        /// The Wizard's spellbook containing all learned spells.
        /// </summary>
        public List<SpellClass> Spellbook { get; set; }

        /// <summary>
        /// Number of prepared spells (Wiz level + INT mod, minimum 1).
        /// </summary>
        public int PreparedSpellCount { get; set; }

        /// <summary>
        /// List of spells currently prepared by the wizard.
        /// </summary>
        public List<SpellClass> PreparedSpells { get; set; }

        /// <summary>
        /// Number of wizard cantrips known.
        /// </summary>
        public int CantripsKnown { get; set; }

        /// <summary>
        /// List of cantrip names known (cantrips don't need preparation).
        /// </summary>
        public List<string> CantripNames { get; set; }

        /// <summary>
        /// Spell save DC = 8 + proficiency bonus + intelligence modifier.
        /// </summary>
        public int SpellSaveDC { get; set; }

        /// <summary>
        /// Spell attack modifier = proficiency bonus + intelligence modifier.
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
        /// Gold cost per spell level when copying spells.
        /// </summary>
        public int GoldCostPerSpellLevel => 50; // Standard: 50gp per level per day

        // ==================== Feature Flags by Level ====================

        private bool _ritualCastingUnlocked;
        private bool _spellbookUnlocked;
        private int _cantripsKnownCount;
        private bool _spellMasteryUnlocked;
        private List<string> _masteredSpells;
        private bool _signatureSpellsUnlocked;
        private HashSet<string> _usedSignatureSpells;
        private Random _portentRandom;
        private List<int> _portentRolls;
        private bool _canRolloverPortent;

        // Subclass-specific flags - Evocation
        private bool _evocationSculptSpellsUnlocked;
        private bool _evocationPotentCantripUnlocked;
        private bool _evocationEmpoweredUnlocked;
        private bool _evocationOverchannelUnlocked;

        // Necromancy
        private bool _necromancyHarvestUnlocked;
        private bool _necromancyGraspUnlocked;
        private bool _necromancyCallDeadUnlocked;
        private bool _necromancyMasterOfDeathUnlocked;

        // Abjuration
        private bool _abjurationSpellMasteryUnlocked;
        private bool _abjurationImprovedAbjurationUnlocked;
        private bool _abjurationMinimalSpellUnlocked;
        private bool _abjurationWatcherOfFateUnlocked;

        // Conjuration
        private bool _conjurationPortalsUnlocked;
        private bool _conjurationBereftUnlocked;
        private bool _conjurationMasteryUnlocked;
        private bool _conjurationMasterOfMyriadFormsUnlocked;

        // Divination
        private bool _divinationExpertDivinationUnlocked;
        private bool _divinationThirdEyeUnlocked;

        // Enchantment
        private bool _enchantmentBonusProficienciesUnlocked;
        private bool _enchantmentDissonantWhispersUnlocked;
        private bool _enchantmentRallyingCryUnlocked;

        // Illusion
        private bool _illusionImprovedMinorIllusionUnlocked;
        private bool _illusionMirrorImageUnlocked;
        private bool _illusionThoughtBakerUnlocked;

        // ==================== Constructors ====================

        public Wizard() : base()
        {
            Tradition = ArcaneTradition.Evocation;
            InitializeWizard();
        }

        public Wizard(string name, int level, RaceClass race, BackgroundClass background)
            : base(name, level, race, background)
        {
            Tradition = ArcaneTradition.Evocation;
            InitializeWizard();
            ApplyLevelFeatures();
        }

        public Wizard(string name, int level, RaceClass race, BackgroundClass background, ArcaneTradition tradition)
            : base(name, level, race, background)
        {
            Tradition = tradition;
            InitializeWizard();
            ApplyLevelFeatures();
        }

        // ==================== Initialization ====================

        private void InitializeWizard()
        {
            // Core properties
            Spellbook = new List<SpellClass>();
            PreparedSpells = new List<SpellClass>();
            CantripNames = new List<string>();
            _portentRandom = new Random();
            _portentRolls = new List<int>();
            _masteredSpells = new List<string>();
            _usedSignatureSpells = new HashSet<string>();

            // Default values
            SpellSlotsPerLevel = new Dictionary<int, int>();
            RemainingSpellSlots = new Dictionary<int, int>();
            _signatureSpellsUnlocked = false;

            // Feature flags
            _ritualCastingUnlocked = false;
            _spellbookUnlocked = false;
            _spellMasteryUnlocked = false;
            _canRolloverPortent = true;

            // Subclass feature flags - Evocation
            _evocationSculptSpellsUnlocked = false;
            _evocationPotentCantripUnlocked = false;
            _evocationEmpoweredUnlocked = false;
            _evocationOverchannelUnlocked = false;

            // Necromancy
            _necromancyHarvestUnlocked = false;
            _necromancyGraspUnlocked = false;
            _necromancyCallDeadUnlocked = false;
            _necromancyMasterOfDeathUnlocked = false;

            // Abjuration
            _abjurationSpellMasteryUnlocked = false;
            _abjurationImprovedAbjurationUnlocked = false;
            _abjurationMinimalSpellUnlocked = false;
            _abjurationWatcherOfFateUnlocked = false;

            // Conjuration
            _conjurationPortalsUnlocked = false;
            _conjurationBereftUnlocked = false;
            _conjurationMasteryUnlocked = false;
            _conjurationMasterOfMyriadFormsUnlocked = false;

            // Divination
            _divinationExpertDivinationUnlocked = false;
            _divinationThirdEyeUnlocked = false;

            // Enchantment
            _enchantmentBonusProficienciesUnlocked = false;
            _enchantmentDissonantWhispersUnlocked = false;
            _enchantmentRallyingCryUnlocked = false;

            // Illusion
            _illusionImprovedMinorIllusionUnlocked = false;
            _illusionMirrorImageUnlocked = false;
            _illusionThoughtBakerUnlocked = false;

            // Initial calculations
            CalculateSpellDC();
            CalculateSpellAttack();
            UpdateCantripsKnown();
            UpdatePreparedSpellsCount();
            UpdateSpellSlots();
        }

        // ==================== Stat Calculations ====================

        /// <summary>
        /// Calculates the spell save DC based on Intelligence and level.
        /// Formula: 8 + proficiency bonus + INT modifier
        /// </summary>
        public void CalculateSpellDC()
        {
            int baseDC = 8;
            int intMod = GetAbilityModifier(_intelligence);
            int proficiencyBonus = GetProficiencyBonus();
            SpellSaveDC = baseDC + intMod + proficiencyBonus;
        }

        /// <summary>
        /// Calculates the spell attack modifier.
        /// Formula: proficiency bonus + INT modifier
        /// </summary>
        public void CalculateSpellAttack()
        {
            int intMod = GetAbilityModifier(_intelligence);
            int proficiencyBonus = GetProficiencyBonus();
            SpellAttackModifier = intMod + proficiencyBonus;
        }

        /// <summary>
        /// Gets the proficiency bonus based on character level.
        /// </summary>
        private int GetProficiencyBonus()
        {
            if (Level <= 4) return 2;
            if (Level <= 8) return 3;
            if (Level <= 12) return 4;
            if (Level <= 16) return 5;
            return 6;
        }

        /// <summary>
        /// Gets the ability modifier for a given ability score.
        /// </summary>
        private int GetAbilityModifier(int abilityScore)
        {
            return (abilityScore - 10) / 2;
        }

        // ==================== Cantrip Management ====================

        /// <summary>
        /// Updates the number of cantrips known based on wizard level.
        /// </summary>
        private void UpdateCantripsKnown()
        {
            int newCount = 0;
            if (Level >= 1) newCount = 3;
            if (Level >= 4) newCount = 4;
            if (Level >= 10) newCount = 5;

            CantripsKnown = newCount;

            // Add default starting cantrips if none exist
            if (Level >= 1 && CantripNames.Count == 0)
            {
                CantripNames.Add("Firebolt");
                CantripNames.Add("MageHand");
                CantripNames.Add("MinorIllusion");
            }
        }

        // ==================== Spellbook Management ====================

        /// <summary>
        /// Adds a spell to the wizard's spellbook.
        /// Wizards can add any spell of a level they can cast, at twice the normal cost in time and gold.
        /// </summary>
        public virtual bool AddSpellToSpellbook(SpellClass spell)
        {
            if (spell.Level > GetHighestAvailableSpellSlotLevel())
            {
                Console.WriteLine($"{Name} cannot add {spell.Name} - it is too high of a level.");
                return false;
            }

            // Check if spell is already in spellbook
            if (Spellbook.Any(s => s.Name == spell.Name && s.Level == spell.Level))
            {
                Console.WriteLine($"{Name} already has {spell.Name} in their spellbook.");
                return false;
            }

            Spellbook.Add(spell);
            int goldCost = spell.Level * GoldCostPerSpellLevel;
            Console.WriteLine($"{Name} adds {spell.Name} (level {spell.Level}) to their spellbook! Cost: {goldCost}gp, 2 hours of work.");
            return true;
        }

        /// <summary>
        /// Copies a spell from a scroll or another spellbook into the wizard's spellbook.
        /// Requires materials worth 50gp per spell level and 2 hours per level.
        /// </summary>
        public virtual bool CopySpellFromScroll(SpellClass sourceSpell, int goldPaid)
        {
            if (goldPaid < sourceSpell.Level * GoldCostPerSpellLevel)
            {
                Console.WriteLine($"{Name} needs at least {sourceSpell.Level * GoldCostPerSpellLevel}gp to copy this spell.");
                return false;
            }

            // Create a copy for the spellbook
            var copiedSpell = new SpellClass(
                sourceSpell.Name,
                sourceSpell.Level,
                sourceSpell.School,
                sourceSpell.CastingTime,
                sourceSpell.Range,
                sourceSpell.Duration,
                sourceSpell.Description,
                sourceSpell.Components,
                sourceSpell.MaterialComponents,
                sourceSpell.DamageType,
                sourceSpell.DamageAmount,
                sourceSpell.SavingThrow,
                sourceSpell.TargetType,
                sourceSpell.TargetCount,
                sourceSpell.AreaOfEffect,
                sourceSpell.IsConcentration,
                sourceSpell.IsRitual
            );

            return AddSpellToSpellbook(copiedSpell);
        }

        /// <summary>
        /// Removes a spell from the spellbook.
        /// </summary>
        public virtual bool RemoveSpellFromSpellbook(string spellName)
        {
            var spell = Spellbook.FirstOrDefault(s => s.Name == spellName);
            if (spell == null)
            {
                Console.WriteLine($"{Name} does not have {spellName} in their spellbook.");
                return false;
            }

            // Check if the spell is prepared - cannot remove prepared spells
            var preparedSpell = PreparedSpells.FirstOrDefault(s => s.Name == spellName);
            if (preparedSpell != null)
            {
                Console.WriteLine($"{spellName} is currently prepared. Unprepare it first.");
                return false;
            }

            Spellbook.Remove(spell);
            Console.WriteLine($"{Name} removes {spellName} from their spellbook.");
            return true;
        }

        /// <summary>
        /// Gets all spells in the spellbook filtered by school of magic.
        /// </summary>
        public virtual List<SpellClass> GetSpellsBySchool(School school)
        {
            return Spellbook.FindAll(s => s.School == school);
        }

        /// <summary>
        /// Gets all spells in the spellbook filtered by level.
        /// </summary>
        public virtual List<SpellClass> GetSpellsByLevel(int level)
        {
            return Spellbook.FindAll(s => s.Level == level);
        }

        /// <summary>
        /// Gets all ritual spells in the spellbook.
        /// </summary>
        public virtual List<SpellClass> GetRitualSpells()
        {
            return Spellbook.FindAll(s => s.IsRitual);
        }

        // ==================== Spell Preparation ====================

        /// <summary>
        /// Updates the number of prepared spells based on wizard level and INT modifier.
        /// Formula: Wizard level + INT modifier (minimum 1)
        /// </summary>
        private void UpdatePreparedSpellsCount()
        {
            int intMod = GetAbilityModifier(_intelligence);
            PreparedSpellCount = Math.Max(1, Level + intMod);
        }

        /// <summary>
        /// Prepares spells from the spellbook. Wizards can only prepare a number of spells
        /// equal to their wizard level + INT modifier.
        /// </summary>
        public virtual void PrepareSpells()
        {
            UpdatePreparedSpellsCount();

            if (Spellbook.Count == 0)
            {
                Console.WriteLine($"{Name} has no spells in their spellbook to prepare.");
                return;
            }

            // If all are already prepared, just refresh
            if (PreparedSpells.Count >= PreparedSpellCount)
            {
                Console.WriteLine($"{Name} reorganizes their spellbook and prepares {PreparedSpellCount} spells.");
                return;
            }

            // Get spells not yet prepared
            var available = Spellbook.Where(s => !PreparedSpells.Any(p => p.Name == s.Name && p.Level == s.Level)).ToList();

            int needed = PreparedSpellCount - PreparedSpells.Count;
            int toAdd = Math.Min(needed, available.Count);

            for (int i = 0; i < toAdd; i++)
            {
                PreparedSpells.Add(available[i]);
                Console.WriteLine($"  Prepared: {available[i].Name} (level {available[i].Level})");
            }

            Console.WriteLine($"{Name} has prepared {PreparedSpells.Count}/{PreparedSpellCount} spells from their spellbook.");
        }

        /// <summary>
        /// Checks if a spell is currently prepared.
        /// </summary>
        public virtual bool IsSpellPrepared(string spellName)
        {
            return PreparedSpells.Any(s => s.Name == spellName);
        }

        /// <summary>
        /// Unprepares a specific spell, removing it from prepared list.
        /// </summary>
        public virtual bool UnprepareSpell(string spellName)
        {
            var spell = PreparedSpells.FirstOrDefault(s => s.Name == spellName);
            if (spell == null)
            {
                Console.WriteLine($"{Name} does not have {spellName} prepared.");
                return false;
            }

            PreparedSpells.Remove(spell);
            Console.WriteLine($"{Name} unprepares {spellName}.");
            return true;
        }

        // ==================== Ritual Casting ====================

        /// <summary>
        /// Casts a ritual spell from the spellbook without preparing it.
        /// Requires 10 minutes longer than normal casting time.
        /// Unlocked at level 2.
        /// </summary>
        public virtual bool CastRitualSpell(string spellName)
        {
            if (!_ritualCastingUnlocked)
            {
                Console.WriteLine($"{Name} has not learned ritual casting yet (requires level 2).");
                return false;
            }

            var ritualSpell = Spellbook.FirstOrDefault(s => s.Name == spellName && s.IsRitual);
            if (ritualSpell == null)
            {
                Console.WriteLine($"{Name} does not have a ritual version of {spellName} in their spellbook.");
                return false;
            }

            Console.WriteLine($"{Name} casts {spellName} as a ritual from their spellbook (takes 10 extra minutes).");
            return true;
        }

        // ==================== Spell Slots Management ====================

        /// <summary>
        /// Updates spell slots based on the full caster table (PHB p.203).
        /// </summary>
        private void UpdateSpellSlots()
        {
            SpellSlotsPerLevel.Clear();
            RemainingSpellSlots.Clear();

            // Full caster slot table (PHB p.203)
            switch (Level)
            {
                // Level 1
                case 1:
                    SpellSlotsPerLevel[1] = 2;
                    break;
                // Level 2
                case 2:
                    SpellSlotsPerLevel[1] = 3;
                    break;
                // Level 3
                case 3:
                    SpellSlotsPerLevel[1] = 4;
                    SpellSlotsPerLevel[2] = 2;
                    break;
                // Level 4
                case 4:
                    SpellSlotsPerLevel[1] = 4;
                    SpellSlotsPerLevel[2] = 3;
                    break;
                // Level 5
                case 5:
                    SpellSlotsPerLevel[1] = 4;
                    SpellSlotsPerLevel[2] = 3;
                    SpellSlotsPerLevel[3] = 2;
                    break;
                // Level 6
                case 6:
                    SpellSlotsPerLevel[1] = 4;
                    SpellSlotsPerLevel[2] = 3;
                    SpellSlotsPerLevel[3] = 3;
                    break;
                // Level 7
                case 7:
                    SpellSlotsPerLevel[1] = 4;
                    SpellSlotsPerLevel[2] = 3;
                    SpellSlotsPerLevel[3] = 3;
                    SpellSlotsPerLevel[4] = 1;
                    break;
                // Level 8
                case 8:
                    SpellSlotsPerLevel[1] = 4;
                    SpellSlotsPerLevel[2] = 3;
                    SpellSlotsPerLevel[3] = 3;
                    SpellSlotsPerLevel[4] = 2;
                    break;
                // Level 9
                case 9:
                    SpellSlotsPerLevel[1] = 4;
                    SpellSlotsPerLevel[2] = 3;
                    SpellSlotsPerLevel[3] = 3;
                    SpellSlotsPerLevel[4] = 3;
                    break;
                // Level 10
                case 10:
                    SpellSlotsPerLevel[1] = 4;
                    SpellSlotsPerLevel[2] = 3;
                    SpellSlotsPerLevel[3] = 3;
                    SpellSlotsPerLevel[4] = 3;
                    SpellSlotsPerLevel[5] = 1;
                    break;
                // Level 11
                case 11:
                    SpellSlotsPerLevel[1] = 4;
                    SpellSlotsPerLevel[2] = 3;
                    SpellSlotsPerLevel[3] = 3;
                    SpellSlotsPerLevel[4] = 3;
                    SpellSlotsPerLevel[5] = 2;
                    break;
                // Level 12
                case 12:
                    SpellSlotsPerLevel[1] = 4;
                    SpellSlotsPerLevel[2] = 3;
                    SpellSlotsPerLevel[3] = 3;
                    SpellSlotsPerLevel[4] = 3;
                    SpellSlotsPerLevel[5] = 2;
                    break;
                // Level 13
                case 13:
                    SpellSlotsPerLevel[1] = 4;
                    SpellSlotsPerLevel[2] = 3;
                    SpellSlotsPerLevel[3] = 3;
                    SpellSlotsPerLevel[4] = 3;
                    SpellSlotsPerLevel[5] = 2;
                    SpellSlotsPerLevel[6] = 1;
                    break;
                // Level 14
                case 14:
                    SpellSlotsPerLevel[1] = 4;
                    SpellSlotsPerLevel[2] = 3;
                    SpellSlotsPerLevel[3] = 3;
                    SpellSlotsPerLevel[4] = 3;
                    SpellSlotsPerLevel[5] = 2;
                    SpellSlotsPerLevel[6] = 1;
                    break;
                // Level 15
                case 15:
                    SpellSlotsPerLevel[1] = 4;
                    SpellSlotsPerLevel[2] = 3;
                    SpellSlotsPerLevel[3] = 3;
                    SpellSlotsPerLevel[4] = 3;
                    SpellSlotsPerLevel[5] = 2;
                    SpellSlotsPerLevel[6] = 1;
                    SpellSlotsPerLevel[7] = 1;
                    break;
                // Level 16
                case 16:
                    SpellSlotsPerLevel[1] = 4;
                    SpellSlotsPerLevel[2] = 3;
                    SpellSlotsPerLevel[3] = 3;
                    SpellSlotsPerLevel[4] = 3;
                    SpellSlotsPerLevel[5] = 2;
                    SpellSlotsPerLevel[6] = 1;
                    SpellSlotsPerLevel[7] = 1;
                    break;
                // Level 17
                case 17:
                    SpellSlotsPerLevel[1] = 4;
                    SpellSlotsPerLevel[2] = 3;
                    SpellSlotsPerLevel[3] = 3;
                    SpellSlotsPerLevel[4] = 3;
                    SpellSlotsPerLevel[5] = 2;
                    SpellSlotsPerLevel[6] = 1;
                    SpellSlotsPerLevel[7] = 1;
                    SpellSlotsPerLevel[8] = 1;
                    break;
                // Level 18
                case 18:
                    SpellSlotsPerLevel[1] = 4;
                    SpellSlotsPerLevel[2] = 3;
                    SpellSlotsPerLevel[3] = 3;
                    SpellSlotsPerLevel[4] = 3;
                    SpellSlotsPerLevel[5] = 2;
                    SpellSlotsPerLevel[6] = 1;
                    SpellSlotsPerLevel[7] = 1;
                    SpellSlotsPerLevel[8] = 1;
                    SpellSlotsPerLevel[9] = 1;
                    break;
                // Level 19
                case 19:
                    SpellSlotsPerLevel[1] = 4;
                    SpellSlotsPerLevel[2] = 3;
                    SpellSlotsPerLevel[3] = 3;
                    SpellSlotsPerLevel[4] = 3;
                    SpellSlotsPerLevel[5] = 2;
                    SpellSlotsPerLevel[6] = 1;
                    SpellSlotsPerLevel[7] = 1;
                    SpellSlotsPerLevel[8] = 1;
                    SpellSlotsPerLevel[9] = 1;
                    break;
                // Level 20
                case 20:
                    SpellSlotsPerLevel[1] = 4;
                    SpellSlotsPerLevel[2] = 3;
                    SpellSlotsPerLevel[3] = 3;
                    SpellSlotsPerLevel[4] = 3;
                    SpellSlotsPerLevel[5] = 2;
                    SpellSlotsPerLevel[6] = 1;
                    SpellSlotsPerLevel[7] = 1;
                    SpellSlotsPerLevel[8] = 1;
                    SpellSlotsPerLevel[9] = 1;
                    break;
            }

            // Copy to remaining slots
            foreach (var slot in SpellSlotsPerLevel)
            {
                RemainingSpellSlots[slot.Key] = slot.Value;
            }

            Console.WriteLine($"{Name}'s spell slots updated for level {Level}:");
            foreach (var slot in RemainingSpellSlots)
            {
                Console.WriteLine($"  {slot.Key}-level slots: {slot.Value}/{slot.Key} remaining");
            }
        }

        /// <summary>
        /// Gets the highest spell slot level available to the wizard.
        /// </summary>
        private int GetHighestAvailableSpellSlotLevel()
        {
            for (int i = 9; i >= 0; i--)
            {
                if (SpellSlotsPerLevel.ContainsKey(i) && SpellSlotsPerLevel[i] > 0)
                    return i;
            }
            return 0;
        }

        // ==================== Core Casting Methods ====================

        /// <summary>
        /// Gets the wizard's spell save DC.
        /// Formula: 8 + Proficiency Bonus + Intelligence modifier
        /// </summary>
        public int GetSpellSaveDC()
        {
            CalculateSpellDC();
            return SpellSaveDC;
        }

        /// <summary>
        /// Gets the wizard's spell attack bonus.
        /// Formula: Proficiency Bonus + Intelligence modifier
        /// </summary>
        public int GetSpellAttackBonus()
        {
            CalculateSpellAttack();
            return SpellAttackModifier;
        }

        /// <summary>
        /// Casts a prepared spell using a spell slot.
        /// </summary>
        public virtual bool CastPreparedSpell(string spellName, int slotLevel)
        {
            var spell = PreparedSpells.FirstOrDefault(s => s.Name == spellName);
            if (spell == null)
            {
                Console.WriteLine($"{Name} does not have {spellName} prepared.");
                return false;
            }

            if (!RemainingSpellSlots.ContainsKey(slotLevel) || RemainingSpellSlots[slotLevel] <= 0)
            {
                Console.WriteLine($"{Name} has no spell slots of level {slotLevel}.");
                return false;
            }

            // Consume the slot
            RemainingSpellSlots[slotLevel]--;
            Console.WriteLine($"{Name} casts {spellName} using a {slotLevel}-level spell slot.");

            // Apply signature spell free cast check
            if (_signatureSpellsUnlocked && _usedSignatureSpells.Contains(spellName))
            {
                _usedSignatureSpells.Remove(spellName);
                Console.WriteLine($"  {spellName} is cast for free (Signature Spell)!");
            }

            return true;
        }

        /// <summary>
        /// Casts a cantrip (no spell slot required).
        /// </summary>
        public virtual bool CastCantrip(string cantripName)
        {
            if (!CantripNames.Contains(cantripName))
            {
                Console.WriteLine($"{Name} does not know the cantrip: {cantripName}");
                return false;
            }

            int attackBonus = GetSpellAttackBonus();
            Console.WriteLine($"{Name} casts {cantripName} (attack bonus: +{attackBonus})!");

            // Apply Evocation's Potent Cantrip if unlocked
            if (_evocationPotentCantripUnlocked)
            {
                int intMod = GetAbilityModifier(_intelligence);
                Console.WriteLine($"  [Evocation] Potential extra damage: +{intMod} on a saved attack!");
            }

            return true;
        }

        /// <summary>
        /// Casts any spell from the spellbook (even if not prepared) as a ritual.
        /// Only works for spells with the ritual tag.
        /// </summary>
        public virtual bool CastSpellFromSpellbook(string spellName, bool requiresPreparation = true)
        {
            var spell = Spellbook.FirstOrDefault(s => s.Name == spellName);
            if (spell == null)
            {
                Console.WriteLine($"{Name} does not have {spellName} in their spellbook.");
                return false;
            }

            if (requiresPreparation && !IsSpellPrepared(spellName))
            {
                Console.WriteLine($"{spellName} is not prepared. Prepared spells can be cast with slots.");
                return false;
            }

            // Find an available slot
            for (int i = spell.Level; i <= 9; i++)
            {
                if (RemainingSpellSlots.ContainsKey(i) && RemainingSpellSlots[i] > 0)
                {
                    RemainingSpellSlots[i]--;
                    Console.WriteLine($"{Name} casts {spellName} using a {Math.Max(spell.Level, i)}-level spell slot.");
                    return true;
                }
            }

            Console.WriteLine($"{Name} has no available spell slots to cast {spellName}.");
            return false;
        }

        // ==================== Spell Mastery (Levels 3/9/13/17) ====================

        /// <summary>
        /// Adds a spell to the wizard's mastered list. Mastered spells grant advantage
        /// on saving throws against spells that target the wizard, and the wizard can
        /// reroll an enemy's save against their spell.
        /// </summary>
        public virtual void AddMasteredSpell(string spellName, int spellLevel)
        {
            if (!_spellMasteryUnlocked)
            {
                Console.WriteLine($"{Name} has not learned Spell Mastery yet (requires level 3).");
                return;
            }

            if (_masteredSpells.Contains(spellName))
            {
                Console.WriteLine($"{Name} already has {spellName} mastered.");
                return;
            }

            _masteredSpells.Add(spellName);
            Console.WriteLine($"{Name} achieves spell mastery with {spellName} (level {spellLevel})!");
        }

        /// <summary>
        /// Rerolls an enemy's saving throw against a mastered spell.
        /// The enemy must use the new roll.
        /// </summary>
        public virtual bool ReRollSaveDC(string masteredSpell)
        {
            if (!_spellMasteryUnlocked || !_masteredSpells.Contains(masteredSpell))
            {
                Console.WriteLine($"{Name} does not have mastery over {masteredSpell}.");
                return false;
            }

            Console.WriteLine($"{Name} uses Spell Mastery on {masteredSpell}! The target must reroll their saving throw.");
            return true;
        }

        // ==================== Signature Spells (Level 20) ====================

        /// <summary>
        /// Sets a spell as a signature spell. Once per long rest, the wizard can cast
        /// a signature spell using the lowest available slot without expending a slot.
        /// </summary>
        public virtual void SetSignatureSpell(string spellName, int spellLevel)
        {
            if (!_signatureSpellsUnlocked)
            {
                Console.WriteLine($"{Name} has not learned Signature Spells yet (requires level 20).");
                return;
            }

            // Add to prepared spells automatically
            var sourceSpell = Spellbook.FirstOrDefault(s => s.Name == spellName && s.Level == spellLevel);
            if (sourceSpell != null && !PreparedSpells.Any(p => p.Name == spellName))
            {
                PreparedSpells.Add(sourceSpell);
            }

            Console.WriteLine($"{Name} designates {spellName} as a signature spell! It can be cast once per long rest without expending a slot.");
        }

        /// <summary>
        /// Checks if a signature spell is available for free casting.
        /// </summary>
        public virtual bool HasSignatureSpellAvailable(string spellName)
        {
            return _signatureSpellsUnlocked && !_usedSignatureSpells.Contains(spellName);
        }

        // ==================== Subclass Feature Methods ====================

        /// <summary>
        /// Applies subclass features based on the chosen Arcane Tradition.
        /// </summary>
        private void ApplySubclassFeatures()
        {
            switch (Tradition)
            {
                case ArcaneTradition.Evocation:
                    Console.WriteLine($"{Name} chooses the School of Evocation!");
                    Console.WriteLine("  - Sculpt Spells at level 2");
                    break;
                case ArcaneTradition.Necromancy:
                    Console.WriteLine($"{Name} chooses the School of Necromancy!");
                    Console.WriteLine("  - Grasp of the Dead at level 2");
                    break;
                case ArcaneTradition.Transmutation:
                    Console.WriteLine($"{Name} chooses the School of Transmutation!");
                    Console.WriteLine("  - Alchemy at level 2");
                    break;
                case ArcaneTradition.Abjuration:
                    Console.WriteLine($"{Name} chooses the School of Abjuration!");
                    Console.WriteLine("  - Spell Mastery at level 2");
                    break;
                case ArcaneTradition.Divination:
                    Console.WriteLine($"{Name} chooses the School of Divination!");
                    Console.WriteLine("  - Portent at level 2");
                    break;
                case ArcaneTradition.Conjuration:
                    Console.WriteLine($"{Name} chooses the School of Conjuration!");
                    Console.WriteLine("  - Portals at level 2");
                    break;
                case ArcaneTradition.Enchantment:
                    Console.WriteLine($"{Name} chooses the School of Enchantment!");
                    Console.WriteLine("  - Bonus Proficiencies at level 2");
                    break;
                case ArcaneTradition.Illusion:
                    Console.WriteLine($"{Name} chooses the School of Illusion!");
                    Console.WriteLine("  - Minor Illusion Enhancement at level 2");
                    break;
            }
        }

        // ---- Evocation Features ----

        /// <summary>
        /// Sculpt Spells: Choose creatures in your spell's area. They automatically succeed on saves.
        /// </summary>
        public virtual bool SculptSpells()
        {
            if (!_evocationSculptSpellsUnlocked || Tradition != ArcaneTradition.Evocation)
            {
                Console.WriteLine($"{Name} does not have Sculpt Spells (requires School of Evocation, level 2).");
                return false;
            }

            Console.WriteLine($"{Name} uses Sculpt Spells! Allies in the area take no damage from this evocation spell.");
            return true;
        }

        /// <summary>
        /// Potent Cantrip: When a creature succeeds on a save against your cantrip, it takes half damage and you add INT modifier to that damage.
        /// </summary>
        public virtual bool ApplyPotentCantrip(string cantripName)
        {
            if (!_evocationPotentCantripUnlocked || Tradition != ArcaneTradition.Evocation)
            {
                Console.WriteLine($"{Name} does not have Potent Cantrip (requires level 6).");
                return false;
            }

            int intMod = GetAbilityModifier(_intelligence);
            Console.WriteLine($"{Name} uses Potent Cantrip on {cantripName}! Target takes half damage + {intMod}.");
            return true;
        }

        /// <summary>
        /// Empowered Evocation: Add INT modifier to all damage from your evocation spells.
        /// </summary>
        public virtual bool ApplyEmpoweredEvocation()
        {
            if (!_evocationEmpoweredUnlocked || Tradition != ArcaneTradition.Evocation)
            {
                Console.WriteLine($"{Name} does not have Empowered Evocation (requires level 10).");
                return false;
            }

            int intMod = GetAbilityModifier(_intelligence);
            Console.WriteLine($"{Name} uses Empowered Evocation! Add +{intMod} to all evocation spell damage.");
            return true;
        }

        /// <summary>
        /// Overchannel: Maximize damage for one evocation spell, then take 1d6 necrotic damage per level of the spell.
        /// </summary>
        public virtual bool Overchannel(int spellLevel)
        {
            if (!_evocationOverchannelUnlocked || Tradition != ArcaneTradition.Evocation)
            {
                Console.WriteLine($"{Name} does not have Overchannel (requires level 14).");
                return false;
            }

            int damageTaken = spellLevel * _portentRandom.Next(1, 7);
            Console.WriteLine($"{Name} overchannels their evocation magic! Maximum damage for this spell, but takes {damageTaken} necrotic damage.");
            TakeDamage(damageTaken);
            return true;
        }

        // ---- Necromancy Features ----

        /// <summary>
        /// Harvest of the Dead: When a creature dies within 5 feet while you have an undead conjured, heal HP.
        /// </summary>
        public virtual bool HarvestOfTheDead()
        {
            if (!_necromancyHarvestUnlocked || Tradition != ArcaneTradition.Necromancy)
            {
                Console.WriteLine($"{Name} does not have Harvest of the Dead (requires School of Necromancy, level 2).");
                return false;
            }

            int healing = Level;
            _hitPoints = Math.Min(_maxHitPoints, _hitPoints + healing);
            Console.WriteLine($"{Name} uses Harvest of the Dead! Heals {healing} hit points.");
            return true;
        }

        /// <summary>
        /// Grasp of Hasagoth: Resist necrotic damage. When you cast an undead-creating spell, regain HP equal to twice the number of HD.
        /// </summary>
        public virtual bool GraspOfTheDead()
        {
            if (!_necromancyGraspUnlocked || Tradition != ArcaneTradition.Necromancy)
            {
                Console.WriteLine($"{Name} does not have Grasp of the Dead (requires level 6).");
                return false;
            }

            Console.WriteLine($"{Name} uses Grasp of the Dead! You and allies within 30 feet resist necrotic damage until your next turn.");
            return true;
        }

        /// <summary>
        /// Call the Dead: Summon restless spirits of the recently deceased as ghosts.
        /// </summary>
        public virtual bool CallTheDead(int numberOfSpirits)
        {
            if (!_necromancyCallDeadUnlocked || Tradition != ArcaneTradition.Necromancy)
            {
                Console.WriteLine($"{Name} does not have Call the Dead (requires level 10).");
                return false;
            }

            Console.WriteLine($"{Name} calls forth {numberOfSpirits} restless spirits! They attack enemies for 1 hour.");
            return true;
        }

        /// <summary>
        /// Master of Death: When reduced to 0 HP, make a WIS save. On success, drop to 1 HP instead. Usable once per long rest.
        /// </summary>
        public virtual bool UseMasterOfDeath()
        {
            if (!_necromancyMasterOfDeathUnlocked || Tradition != ArcaneTradition.Necromancy)
            {
                Console.WriteLine($"{Name} does not have Master of Death (requires level 14).");
                return false;
            }

            Console.WriteLine($"{Name} uses Master of Death! Instead of dropping to 0 HP, they drop to 1 HP.");
            _hitPoints = 1;
            return true;
        }

        // ---- Abjuration Features ----

        /// <summary>
        /// Spell Mastery: When you cast an abjuration spell with a casting time of 1 action, you can impose disadvantage on the next attack roll against you.
        /// </summary>
        public virtual bool ApplySpellMastery()
        {
            if (!_abjurationSpellMasteryUnlocked || Tradition != ArcaneTradition.Abjuration)
            {
                Console.WriteLine($"{Name} does not have Spell Mastery (requires School of Abjuration, level 2).");
                return false;
            }

            Console.WriteLine($"{Name} uses Spell Mastery! Next enemy attack against them has disadvantage.");
            return true;
        }

        /// <summary>
        /// Improved Abjuration: When you cast an abjuration spell, you and ally within 30 feet gain +2 to AC until your next turn.
        /// Also, you have advantage on saving throws against spells.
        /// </summary>
        public virtual bool ApplyImprovedAbjuration()
        {
            if (!_abjurationImprovedAbjurationUnlocked || Tradition != ArcaneTradition.Abjuration)
            {
                Console.WriteLine($"{Name} does not have Improved Abjuration (requires level 6).");
                return false;
            }

            Console.WriteLine($"{Name} uses Improved Abjuration! +2 AC to allies and advantage on abjuration saves.");
            return true;
        }

        /// <summary>
        /// Minimal Spell: When you cast an abjuration spell that affects only yourself, choose one creature you can see.
        /// The spell's damage against that target is reduced by 1d10 + your INT modifier.
        /// </summary>
        public virtual bool ApplyMinimalSpell(int targetAC)
        {
            if (!_abjurationMinimalSpellUnlocked || Tradition != ArcaneTradition.Abjuration)
            {
                Console.WriteLine($"{Name} does not have Minimal Spell (requires level 10).");
                return false;
            }

            int intMod = GetAbilityModifier(_intelligence);
            int reduction = _portentRandom.Next(1, 11) + intMod;
            Console.WriteLine($"{Name} uses Minimal Spell! Target takes {reduction} less damage from abjuration spells.");
            return true;
        }

        /// <summary>
        /// Watcher of Fate: When you would be hit by an attack, if you have no spell slots, you can use your reaction to impose disadvantage on the attack roll.
        /// </summary>
        public virtual bool UseWatcherOfFate()
        {
            if (!_abjurationWatcherOfFateUnlocked || Tradition != ArcaneTradition.Abjuration)
            {
                Console.WriteLine($"{Name} does not have Watcher of Fate (requires level 14).");
                return false;
            }

            // Check if all spell slots are empty
            bool hasSlots = RemainingSpellSlots.Any(s => s.Value > 0);
            if (hasSlots)
            {
                Console.WriteLine($"{Name} still has spell slots available and cannot use Watcher of Fate.");
                return false;
            }

            Console.WriteLine($"{Name} uses Watcher of Fate! Enemy's attack roll has disadvantage.");
            return true;
        }

        // ---- Conjuration Features ----

        /// <summary>
        /// Portals: When you cast a conjuration spell with a casting time of 1 action, you can instantly teleport up to 240 feet.
        /// Also, creatures you conjure don't count as friendly for purposes of opposing factions.
        /// </summary>
        public virtual bool UsePortals(string destination)
        {
            if (!_conjurationPortalsUnlocked || Tradition != ArcaneTradition.Conjuration)
            {
                Console.WriteLine($"{Name} does not have Portals (requires School of Conjuration, level 2).");
                return false;
            }

            Console.WriteLine($"{Name} uses Portals! Teleports to {destination} instantly.");
            return true;
        }

        /// <summary>
        /// Bereft: When you damage a creature with a conjuration spell, you can use your reaction to teleport up to 30 feet.
        /// </summary>
        public virtual bool UseBereft()
        {
            if (!_conjurationBereftUnlocked || Tradition != ArcaneTradition.Conjuration)
            {
                Console.WriteLine($"{Name} does not have Bereft (requires level 6).");
                return false;
            }

            Console.WriteLine($"{Name} uses Bereft! Teleports up to 30 feet after damaging a creature with a conjuration spell.");
            return true;
        }

        /// <summary>
        /// Mastery of Phases: Your ranged spell attacks have range increment of 100 feet. Concentration spells you cast ignore half cover.
        /// </summary>
        public virtual bool ApplyMasteryOfPhases()
        {
            if (!_conjurationMasteryUnlocked || Tradition != ArcaneTradition.Conjuration)
            {
                Console.WriteLine($"{Name} does not have Mastery of Phases (requires level 10).");
                return false;
            }

            Console.WriteLine($"{Name} uses Mastery of Phases! Increased spell attack range and ignores half cover.");
            return true;
        }

        /// <summary>
        /// Master of Myriad Forms: When you cast a conjuration spell, you can choose a creature you conjure to obey your commands.
        /// </summary>
        public virtual bool ApplyMasterOfMyriadForms()
        {
            if (!_conjurationMasterOfMyriadFormsUnlocked || Tradition != ArcaneTradition.Conjuration)
            {
                Console.WriteLine($"{Name} does not have Master of Myriad Forms (requires level 14).");
                return false;
            }

            Console.WriteLine($"{Name} uses Master of Myriad Forms! Conjured creatures now obey your commands.");
            return true;
        }

        // ---- Divination Features ----

        /// <summary>
        /// Portent: Roll 2d20 after finishing a long rest. Replace any roll (yours or an ally's) with one of these rolls.
        /// </summary>
        public virtual bool UsePortent(int rollResult, bool isAllyRoll = true)
        {
            if (_portentRolls.Count == 0)
            {
                Console.WriteLine($"{Name} has no Portent rolls remaining!");
                return false;
            }

            int usedRoll = _portentRolls[0];
            _portentRolls.RemoveAt(0);
            string targetStr = isAllyRoll ? "ally's" : "own";
            Console.WriteLine($"{Name} uses Portent, replacing {targetStr} roll with {usedRoll}!");
            return true;
        }

        /// <summary>
        /// Expert Divination: Other creatures can also use your Portent rolls.
        /// </summary>
        public virtual bool ApplyExpertDivination()
        {
            if (!_divinationExpertDivinationUnlocked || Tradition != ArcaneTradition.Divination)
            {
                Console.WriteLine($"{Name} does not have Expert Divination (requires level 6).");
                return false;
            }

            Console.WriteLine($"{Name} uses Expert Divination! Allies can also use your Portent rolls.");
            return true;
        }

        /// <summary>
        /// The Third Eye: As an action, gain temporary proficiency with a skill or tool, or see spectral tentacles for 1 hour.
        /// </summary>
        public virtual bool ApplyThirdEye()
        {
            if (!_divinationThirdEyeUnlocked || Tradition != ArcaneTradition.Divination)
            {
                Console.WriteLine($"{Name} does not have The Third Eye (requires level 10).");
                return false;
            }

            Console.WriteLine($"{Name} uses The Third Eye! Gain temporary skill/tool proficiency.");
            return true;
        }

        // ---- Enchantment Features ----

        /// <summary>
        /// Bonus Proficiencies: Advantage on saving throws against spells, and resistance to the psychic damage from your spells.
        /// </summary>
        public virtual bool ApplyBonusProficiencies()
        {
            if (!_enchantmentBonusProficienciesUnlocked || Tradition != ArcaneTradition.Enchantment)
            {
                Console.WriteLine($"{Name} does not have Bonus Proficiencies (requires School of Enchantment, level 2).");
                return false;
            }

            Console.WriteLine($"{Name} uses Bonus Proficiencies! Advantage on spell saves and psychic resistance.");
            return true;
        }

        /// <summary>
        /// Dissonant Whispers: When hit by an attack, use reaction to cast Dissonant Whispers as a level 3 spell.
        /// </summary>
        public virtual bool ApplyDissonantWhispers()
        {
            if (!_enchantmentDissonantWhispersUnlocked || Tradition != ArcaneTradition.Enchantment)
            {
                Console.WriteLine($"{Name} does not have Dissonant Whispers (requires level 6).");
                return false;
            }

            Console.WriteLine($"{Name} uses Dissonant Whispers! Target must make a WIS save or be frightened.");
            return true;
        }

        /// <summary>
        /// Rallying Cry: As a bonus action, grant allies within 30 feet temporary HP equal to your wizard level + INT modifier.
        /// </summary>
        public virtual int ApplyRallyingCry()
        {
            if (!_enchantmentRallyingCryUnlocked || Tradition != ArcaneTradition.Enchantment)
            {
                Console.WriteLine($"{Name} does not have Rallying Cry (requires level 10).");
                return 0;
            }

            int intMod = GetAbilityModifier(_intelligence);
            int tempHP = Level + intMod;
            Console.WriteLine($"{Name} uses Rallying Cry! Grants {tempHP} temporary HP to allies within 30 feet.");
            return tempHP;
        }

        // ---- Illusion Features ----

        /// <summary>
        /// Improved Minor Illusion: Can create sound or object as a bonus action.
        /// </summary>
        public virtual bool ApplyImprovedMinorIllusion(string illusionType)
        {
            if (!_illusionImprovedMinorIllusionUnlocked || Tradition != ArcaneTradition.Illusion)
            {
                Console.WriteLine($"{Name} does not have Improved Minor Illusion (requires School of Illusion, level 2).");
                return false;
            }

            Console.WriteLine($"{Name} creates an {illusionType} illusion as a bonus action!");
            return true;
        }

        /// <summary>
        /// Mirror Image: As a reaction when hit by an attack, create illusory doubles. Next attack against you has disadvantage.
        /// </summary>
        public virtual bool ApplyMirrorImage()
        {
            if (!_illusionMirrorImageUnlocked || Tradition != ArcaneTradition.Illusion)
            {
                Console.WriteLine($"{Name} does not have Mirror Image (requires level 6).");
                return false;
            }

            Console.WriteLine($"{Name} uses Mirror Image! Illusory doubles appear. Next attack has disadvantage.");
            return true;
        }

        /// <summary>
        /// Thought Baker: Project illusion directly into a creature's mind within 60 feet as an action.
        /// </summary>
        public virtual bool ApplyThoughtBaker()
        {
            if (!_illusionThoughtBakerUnlocked || Tradition != ArcaneTradition.Illusion)
            {
                Console.WriteLine($"{Name} does not have Thought Baker (requires level 10).");
                return false;
            }

            Console.WriteLine($"{Name} uses Thought Baker! Projects illusion directly into a creature's mind.");
            return true;
        }

        // ==================== Stat Calculations ====================

        /// <summary>
        /// Override base stats calculation for Wizard.
        /// Hit Points: d6 per level, AC: Dex-based (none by default), Speed: 30 ft
        /// </summary>
        protected override void CalculateBaseStats()
        {
            int conMod = Math.Max(-5, (Constitution - 10) / 2);

            // Max HP: d6 per level + con mod per level
            int hpFromFirstLevel = 6 + conMod;
            int hpFromHigherLevels = (Level - 1) * (6 + conMod);
            _maxHitPoints = hpFromFirstLevel + hpFromHigherLevels;

            if (_hitPoints > _maxHitPoints || _hitPoints <= 0)
            {
                _hitPoints = _maxHitPoints;
            }

            // Default AC based on Dex (no armor by default)
            int dexMod = Math.Max(-5, (Dexterity - 10) / 2);
            _armorClass = 10 + dexMod;

            // Speed defaults to race speed (typically 30 ft)
            _speed = _race != null ? _race.Speed : 30;

            Console.WriteLine($"{Name}'s stats calculated: HP {_maxHitPoints}, AC {_armorClass}");
        }

        // ==================== Level Feature Progression ====================

        /// <summary>
        /// Apply features gained at specific wizard levels.
        /// </summary>
        private void ApplyLevelFeatures()
        {
            // ===== LEVEL 1: Spellcasting, Spellbook, Cantrips (3), Prepared spells (Wiz + INT) =====
            if (_level >= 1)
            {
                _spellbookUnlocked = true;
                CalculateSpellDC();
                CalculateSpellAttack();
                UpdateCantripsKnown();
                UpdatePreparedSpellsCount();

                // Add starting cantrips
                CantripNames.Add("Firebolt");
                CantripNames.Add("MageHand");
                CantripNames.Add("MinorIllusion");

                Console.WriteLine($"{Name} begins with 3 cantrips: Firebolt, Mage Hand, Minor Illusion.");
            }

            // ===== LEVEL 2: Arcane Tradition, Ritual Casting =====
            if (_level >= 2)
            {
                _ritualCastingUnlocked = true;
                ApplySubclassFeatures();
                UpdateSpellSlots();
                UpdateCantripsKnown();
                UpdatePreparedSpellsCount();
            }

            // ===== LEVEL 3: Spell Mastery (1st/2nd level spell) =====
            if (_level >= 3)
            {
                _spellMasteryUnlocked = true;
                UpdateSpellSlots();
                UpdateCantripsKnown();
                UpdatePreparedSpellsCount();
            }

            // ===== LEVEL 4: ASI =====
            if (_level >= 4)
            {
                UpdateSpellSlots();
                UpdateCantripsKnown();
            }

            // ===== LEVEL 5: Extra Progression =====
            if (_level >= 5)
            {
                UpdateSpellSlots();
                UpdateCantripsKnown();
                UpdatePreparedSpellsCount();
            }

            // ===== LEVEL 6: Subclass Feature 2 =====
            if (_level >= 6)
            {
                ApplySubclassFeature2();
                UpdateSpellSlots();
                UpdatePreparedSpellsCount();
            }

            // ===== LEVEL 7: Cantrip Evolution =====
            if (_level >= 7)
            {
                UpdateSpellSlots();
                UpdateCantripsKnown();
                UpdatePreparedSpellsCount();
            }

            // ===== LEVEL 8: ASI =====
            if (_level >= 8)
            {
                UpdateSpellSlots();
                UpdateCantripsKnown();
                CalculateSpellDC();
                CalculateSpellAttack();
            }

            // ===== LEVEL 9: Spell Mastery (3rd/4th level spell) =====
            if (_level >= 9)
            {
                UpdateSpellSlots();
                UpdateCantripsKnown();
                UpdatePreparedSpellsCount();
            }

            // ===== LEVEL 10: Subclass Feature 3 =====
            if (_level >= 10)
            {
                ApplySubclassFeature3();
                UpdateSpellSlots();
                UpdateCantripsKnown();
                UpdatePreparedSpellsCount();
            }

            // ===== LEVEL 11: Extra Progression =====
            if (_level >= 11)
            {
                UpdateSpellSlots();
                UpdatePreparedSpellsCount();
            }

            // ===== LEVEL 12: ASI =====
            if (_level >= 12)
            {
                UpdateSpellSlots();
                UpdateCantripsKnown();
                CalculateSpellDC();
                CalculateSpellAttack();
            }

            // ===== LEVEL 13: Spell Mastery (5th/6th level spell) =====
            if (_level >= 13)
            {
                UpdateSpellSlots();
                UpdatePreparedSpellsCount();
            }

            // ===== LEVEL 14: Subclass Feature 4 =====
            if (_level >= 14)
            {
                ApplySubclassFeature4();
                UpdateSpellSlots();
                UpdatePreparedSpellsCount();
            }

            // ===== LEVEL 15: High-Level Spell Slots (9th level) =====
            if (_level >= 15)
            {
                UpdateSpellSlots();
                UpdateCantripsKnown();
                UpdatePreparedSpellsCount();
            }

            // ===== LEVEL 16: ASI =====
            if (_level >= 16)
            {
                UpdateSpellSlots();
                UpdateCantripsKnown();
                CalculateSpellDC();
                CalculateSpellAttack();
            }

            // ===== LEVEL 17: Spell Mastery (7th/8th level spell) =====
            if (_level >= 17)
            {
                UpdateSpellSlots();
                UpdatePreparedSpellsCount();
            }

            // ===== LEVEL 18: Signature Spells + Subclass Feature 5 =====
            if (_level >= 18)
            {
                _signatureSpellsUnlocked = true;
                _usedSignatureSpells.Clear();
                UpdateSpellSlots();
                UpdateCantripsKnown();
                UpdatePreparedSpellsCount();

                // Apply level 18 subclass feature
                switch (Tradition)
                {
                    case ArcaneTradition.Evocation:
                        _evocationEmpoweredUnlocked = true;
                        Console.WriteLine("Evocation Feature: Empowered Evocation - +Int modifier to evocation damage.");
                        break;
                    case ArcaneTradition.Necromancy:
                        _necromancyMasterOfDeathUnlocked = true;
                        Console.WriteLine("Necromancy Feature: Master of Death - Drop to 1 HP instead of 0.");
                        break;
                    case ArcaneTradition.Abjuration:
                        _abjurationWatcherOfFateUnlocked = true;
                        Console.WriteLine("Abjuration Feature: Watcher of Fate - Disadvantage on attacks when out of slots.");
                        break;
                    case ArcaneTradition.Conjuration:
                        _conjurationMasterOfMyriadFormsUnlocked = true;
                        Console.WriteLine("Conjuration Feature: Master of Myriad Forms - Control conjured creatures.");
                        break;
                    case ArcaneTradition.Divination:
                        _divinationThirdEyeUnlocked = true;
                        Console.WriteLine("Divination Feature: The Third Eye - Gain skill/tool proficiency.");
                        break;
                    case ArcaneTradition.Enchantment:
                        _enchantmentRallyingCryUnlocked = true;
                        Console.WriteLine("Enchantment Feature: Rallying Cry - Grant temp HP to allies.");
                        break;
                    case ArcaneTradition.Illusion:
                        _illusionThoughtBakerUnlocked = true;
                        Console.WriteLine("Illusion Feature: Thought Baker - Project illusions into minds.");
                        break;
                }
            }

            // ===== LEVEL 19: ASI =====
            if (_level >= 19)
            {
                UpdateSpellSlots();
                UpdateCantripsKnown();
                CalculateSpellDC();
                CalculateSpellAttack();
            }

            // ===== LEVEL 20: Mathemagic Master =====
            if (_level >= 20)
            {
                UpdateSpellSlots();
                CantripsKnown = 6;
                UpdatePreparedSpellsCount();

                Console.WriteLine($"{Name} achieves Mathemagic Master status!");
                Console.WriteLine("Cantrips known increased to 6. All spell slots restored.");
            }
        }

        /// <summary>
        /// Applies level 6 subclass features.
        /// </summary>
        private void ApplySubclassFeature2()
        {
            switch (Tradition)
            {
                case ArcaneTradition.Evocation:
                    _evocationPotentCantripUnlocked = true;
                    Console.WriteLine("Evocation Feature 2: Potent Cantrip - Add INT to cantrip damage on save.");
                    break;
                case ArcaneTradition.Necromancy:
                    _necromancyGraspUnlocked = true;
                    Console.WriteLine("Necromancy Feature 2: Grasp of the Dead - Resist necrotic, heal on conjuring.");
                    break;
                case ArcaneTradition.Transmutation:
                    Console.WriteLine("Transmutation Feature 2: Formulas of Transmutation.");
                    break;
                case ArcaneTradition.Abjuration:
                    _abjurationImprovedAbjurationUnlocked = true;
                    Console.WriteLine("Abjuration Feature 2: Improved Abjuration - +2 AC, advantage on saves.");
                    break;
                case ArcaneTradition.Divination:
                    _divinationExpertDivinationUnlocked = true;
                    Console.WriteLine("Divination Feature 2: Expert Divination - Allies use your Portent.");
                    break;
                case ArcaneTradition.Conjuration:
                    _conjurationBereftUnlocked = true;
                    Console.WriteLine("Conjuration Feature 2: Bereft - Teleport after damaging with conjuration.");
                    break;
                case ArcaneTradition.Enchantment:
                    _enchantmentDissonantWhispersUnlocked = true;
                    Console.WriteLine("Enchantment Feature 2: Dissonant Whispers as reaction.");
                    break;
                case ArcaneTradition.Illusion:
                    _illusionMirrorImageUnlocked = true;
                    Console.WriteLine("Illusion Feature 2: Mirror Image as reaction.");
                    break;
            }
        }

        /// <summary>
        /// Applies level 10 subclass features.
        /// </summary>
        private void ApplySubclassFeature3()
        {
            switch (Tradition)
            {
                case ArcaneTradition.Evocation:
                    _evocationEmpoweredUnlocked = true;
                    Console.WriteLine("Evocation Feature 3: Empowered Evocation - +Int to all evocation damage.");
                    break;
                case ArcaneTradition.Necromancy:
                    _necromancyCallDeadUnlocked = true;
                    Console.WriteLine("Necromancy Feature 3: Call the Dead - Summon restless spirits.");
                    break;
                case ArcaneTradition.Transmutation:
                    Console.WriteLine("Transmutation Feature 3: Shapechanger - Wild shape-like ability.");
                    break;
                case ArcaneTradition.Abjuration:
                    _abjurationMinimalSpellUnlocked = true;
                    Console.WriteLine("Abjuration Feature 3: Minimal Spell - Reduce damage from abjuration.");
                    break;
                case ArcaneTradition.Divination:
                    // Third eye is level 10 for Divination
                    Console.WriteLine("Divination Feature 3: The Third Eye - Gain proficiency bonuses.");
                    break;
                case ArcaneTradition.Conjuration:
                    _conjurationMasteryUnlocked = true;
                    Console.WriteLine("Conjuration Feature 3: Mastery of Phases - Increased range, ignore half cover.");
                    break;
                case ArcaneTradition.Enchantment:
                    _enchantmentRallyingCryUnlocked = true;
                    Console.WriteLine("Enchantment Feature 3: Rallying Cry - Temp HP for allies.");
                    break;
                case ArcaneTradition.Illusion:
                    // Mirror image is level 6 for Illusion, so this is the next feature
                    Console.WriteLine("Illusion Feature 3: Enhanced Illusions.");
                    break;
            }
        }

        /// <summary>
        /// Applies level 14 subclass features.
        /// </summary>
        private void ApplySubclassFeature4()
        {
            switch (Tradition)
            {
                case ArcaneTradition.Evocation:
                    _evocationOverchannelUnlocked = true;
                    Console.WriteLine("Evocation Feature 4: Overchannel - Maximize damage, take self damage.");
                    break;
                case ArcaneTradition.Necromancy:
                    // Master of Death is level 14
                    break;
                case ArcaneTradition.Transmutation:
                    Console.WriteLine("Transmutation Feature 4: Immaculate Mastery.");
                    break;
                case ArcaneTradition.Abjuration:
                    _abjurationWatcherOfFateUnlocked = true;
                    Console.WriteLine("Abjuration Feature 4: Watcher of Fate - Disadvantage on attacks when out of slots.");
                    break;
                case ArcaneTradition.Conjuration:
                    _conjurationMasterOfMyriadFormsUnlocked = true;
                    Console.WriteLine("Conjuration Feature 4: Master of Myriad Forms.");
                    break;
                case ArcaneTradition.Divination:
                    Console.WriteLine("Divination Feature 4: Ability to reroll any d20.");
                    break;
                case ArcaneTradition.Enchantment:
                    // Posturing Predator is level 14
                    break;
                case ArcaneTradition.Illusion:
                    Console.WriteLine("Illusion Feature 4: Master Illusionist.");
                    break;
            }
        }

        // ==================== Rest Methods ====================

        /// <summary>
        /// Override long rest to restore all spell slots and reset certain abilities.
        /// </summary>
        public override void LongRest()
        {
            base.LongRest();

            // Restore all spell slots
            foreach (var slot in SpellSlotsPerLevel)
            {
                RemainingSpellSlots[slot.Key] = slot.Value;
            }

            // Reset signature spells
            _usedSignatureSpells.Clear();

            // Reset Portent rolls for Divination
            if (Tradition == ArcaneTradition.Divination)
            {
                _portentRolls.Clear();
                _portentRolls.Add(_portentRandom.Next(1, 21));
                _portentRolls.Add(_portentRandom.Next(1, 21));
                Console.WriteLine($"{Name} rolls Portent: [{string.Join(", ", _portentRolls)}]");
            }

            // Reset short-rest abilities for Evocation
            if (Tradition == ArcaneTradition.Evocation)
            {
                Console.WriteLine($"{Name} can use Overchannel again after a long rest.");
            }

            Console.WriteLine($"{Name} has fully rested. All spell slots and abilities restored.");
        }

        /// <summary>
        /// Override short rest to restore certain abilities.
        /// For Wizards, this typically restores invocations/manifestations that recharge on short rest.
        /// </summary>
        public override void ShortRest()
        {
            base.ShortRest();
            Console.WriteLine($"{Name} takes a short rest. Some abilities may be restored.");
        }

        // ==================== Override Base Methods ====================

        public override void ClassSpecificAbility()
        {
            if (_spellbookUnlocked)
            {
                int spellCount = Spellbook.Count;
                Console.WriteLine($"{Name} consults their spellbook ({spellCount} spells) and prepares new spells.");
            }
            else
            {
                Console.WriteLine($"{Name} channels arcane energy, preparing for the next spell cast.");
            }
        }

        public override void Attack()
        {
            int attackBonus = GetSpellAttackBonus();
            Console.WriteLine($"{Name} (Level {_level} Wizard) makes an arcane spell attack!");
            Console.WriteLine($"  Spell attack bonus: +{attackBonus}");

            if (_spellbookUnlocked && PreparedSpells.Count > 0)
            {
                var prepSpell = PreparedSpells[0];
                Console.WriteLine($"  Available prepared spells include: {prepSpell.Name}");
            }
        }

        // ==================== Helper Methods ====================

        /// <summary>
        /// Adds starting spells to the spellbook based on wizard level.
        /// </summary>
        private void AddStartingSpells()
        {
            if (Level < 1) return;

            // Starting spells: Choose 2 1st-level wizard spells + INT modifier bonus spells
            int startingSpells = 6 + GetAbilityModifier(_intelligence);

            // Add some default starting spells
            var defaultSpells = new List<(string Name, int Level, School School)>
            {
                ("ArmorOfAgathys", 1, School.Evocation),
                ("BurningHands", 1, School.Evocation),
                ("ChangeSelf", 1, School.Transmutation),
                ("DetectMagic", 1, School.Divination),
                ("DisguiseSelf", 1, School.Illusion),
                ("FeatherFall", 1, School.Transmutation),
                ("FogCloud", 1, School.Conjuration),
                ("LongStrider", 1, School.Transmutation),
                ("MagicMissile", 1, School.Evocation),
                ("Shield", 1, School.Abjuration),
                ("Shatter", 1, School.Evocation)
            };

            int spellsToAdd = Math.Min(startingSpells, defaultSpells.Count);
            for (int i = 0; i < spellsToAdd; i++)
            {
                var spell = new SpellClass(
                    defaultSpells[i].Name,
                    defaultSpells[i].Level,
                    defaultSpells[i].School,
                    "1 action",
                    "60 feet",
                    "Instantaneous"
                );
                Spellbook.Add(spell);
            }

            Console.WriteLine($"{Name} starts with {spellsToAdd} spells in their spellbook.");
        }

        public int GetTotalSpellSlots()
        {
            int total = 0;
            foreach (var slot in RemainingSpellSlots)
            {
                total += slot.Value;
            }
            return total;
        }

        public List<SpellClass> GetAvailableSpells(int minLevel = 0, int maxLevel = 9)
        {
            return Spellbook.FindAll(s => s.Level >= minLevel && s.Level <= maxLevel);
        }

        public string GetTraditionName()
        {
            return Enum.GetName(typeof(ArcaneTradition), Tradition);
        }
    }
}