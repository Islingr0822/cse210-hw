using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RaceClass = DnDCharacterManager.Race.Race;
using HumanRace = DnDCharacterManager.Race.Human;
using BackgroundClass = DnDCharacterManager.Background.Background;
using BackgroundImpl = DnDCharacterManager.Background.Acolyte;
using InventoryClass = DnDCharacterManager.Inventory.Inventory;
using FeatureClass = DnDCharacterManager.Feature.Feature;
using FeatClass = DnDCharacterManager.Feat.Feat;
using SkillClass = DnDCharacterManager.Skill.Skill;
using SpellBookClass = DnDCharacterManager.Spell.SpellBook;

namespace DnDCharacterManager.Character
{
    /// <summary>
    /// Represents spellcasting information for display purposes.
    /// Provides calculated values for spell-related mechanics without performing dice rolling.
    /// </summary>
    public class SpellcastingInfo
    {
        /// <summary>
        /// The ability score used for spellcasting (e.g., "Intelligence", "Wisdom", "Charisma").
        /// </summary>
        public string SpellCastingAbility { get; }

        /// <summary>
        /// The proficiency bonus at the given level.
        /// </summary>
        public int ProficiencyBonus { get; }

        /// <summary>
        /// The modifier for the spellcasting ability score.
        /// </summary>
        public int AbilityModifier { get; }

        /// <summary>
        /// The Spell Save DC = 8 + proficiency bonus + spellcasting ability modifier.
        /// </summary>
        public int SpellSaveDC => 8 + ProficiencyBonus + AbilityModifier;

        /// <summary>
        /// The Spell Attack Bonus = proficiency bonus + spellcasting ability modifier.
        /// </summary>
        public int SpellAttackBonus => ProficiencyBonus + AbilityModifier;

        /// <summary>
        /// Whether this class can prepare spells from a spell list.
        /// </summary>
        public bool CanPrepareSpells { get; set; }

        /// <summary>
        /// Number of cantrips known by this character.
        /// </summary>
        public int CantripsKnown { get; set; }

        /// <summary>
        /// Maximum number of spells that can be prepared (0 = unlimited preparation tracking).
        /// </summary>
        public int MaxPreparedSpells { get; set; }

        /// <summary>
        /// Spell slots available by level (key = spell level, value = slots per day).
        /// </summary>
        public Dictionary<int, int> SpellSlotsPerLevel { get; set; } = new Dictionary<int, int>();

        /// <summary>
        /// Remaining spell slots by level after rest/expending.
        /// </summary>
        public Dictionary<int, int> RemainingSpellSlots { get; set; } = new Dictionary<int, int>();

        /// <summary>
        /// Constructor for SpellcastingInfo.
        /// </summary>
        /// <param name="spellCastingAbility">The ability score used for spellcasting.</param>
        /// <param name="level">Character level (for proficiency bonus calculation).</param>
        /// <param name="abilityScore">The raw ability score value.</param>
        public SpellcastingInfo(string spellCastingAbility, int level, int abilityScore)
        {
            SpellCastingAbility = spellCastingAbility;
            ProficiencyBonus = GetProficiencyBonusForLevel(level);
            AbilityModifier = CalculateAbilityModifier(abilityScore);
            CanPrepareSpells = false;
            CantripsKnown = 0;
            MaxPreparedSpells = 0;
        }

        /// <summary>
        /// Gets the proficiency bonus for a given character level.
        /// </summary>
        private static int GetProficiencyBonusForLevel(int level)
        {
            if (level <= 4) return 2;
            if (level <= 8) return 3;
            if (level <= 12) return 4;
            if (level <= 16) return 5;
            return 6;
        }

        /// <summary>
        /// Calculates the ability modifier from a raw ability score.
        /// </summary>
        private static int CalculateAbilityModifier(int abilityScore)
        {
            return (abilityScore - 10) / 2;
        }

        /// <summary>
        /// Gets a formatted display string for the Spell Save DC.
        /// </summary>
        public string GetSpellSaveDCDisplay() => $"Spell Save DC: {SpellSaveDC}";

        /// <summary>
        /// Gets a formatted display string for the Spell Attack Bonus.
        /// </summary>
        public string GetSpellAttackDisplay() => $"Spell Attack Bonus: +{SpellAttackBonus}";

        /// <summary>
        /// Gets a formatted display string showing all spellcasting information.
        /// </summary>
        public string GetFullDisplay()
        {
            var lines = new List<string>();
            lines.Add("--- Spellcasting Information ---");
            lines.Add($"Spellcasting Ability: {SpellCastingAbility}");
            lines.Add(GetSpellSaveDCDisplay());
            lines.Add(GetSpellAttackDisplay());
            lines.Add($"Cantrips Known: {CantripsKnown}");

            if (MaxPreparedSpells > 0)
            {
                lines.Add($"Maximum Prepared Spells: {MaxPreparedSpells}");
            }

            if (SpellSlotsPerLevel.Any())
            {
                lines.Add("Spell Slots:");
                foreach (var slot in SpellSlotsPerLevel.OrderBy(s => s.Key))
                {
                    lines.Add($"  {slot.Key}-level: {slot.Value} slots");
                }
            }

            return string.Join(Environment.NewLine, lines);
        }

        /// <summary>
        /// Displays spellcasting information to the console.
        /// </summary>
        public void Display()
        {
            Console.WriteLine(GetFullDisplay());
        }
    }

    /// <summary>
    /// Enum representing common D&D 5e conditions that can affect a character.
    /// </summary>
    public enum CharacterCondition
    {
        None,
        Blinded,
        Deafened,
        Paralyzed,
        Petrified,
        Poisoned,
        Prone,
        Restrained,
        Stunned,
        Unconscious
    }

    /// <summary>
    /// Abstract base class for all D&D characters.
    /// Consolidates character core mechanics, spellcasting info, saving throws, conditions, and combat helpers.
    /// </summary>
    public abstract class Character
    {
        // ==================== Core Properties ====================

        protected string _name;
        protected int _level;
        protected int _experience;
        protected int _hitPoints;
        protected int _maxHitPoints;
        protected int _tempHitPoints;
        protected int _armorClass;
        protected int _speed;
        protected int _proficiencyBonus;
        protected int _initiativeModifier;

        // ==================== Ability Scores ====================

        protected int _strength;
        protected int _dexterity;
        protected int _constitution;
        protected int _intelligence;
        protected int _wisdom;
        protected int _charisma;

        // ==================== Saving Throw Proficiencies ====================

        protected bool _proficientStrength;
        protected bool _proficientDexterity;
        protected bool _proficientConstitution;
        protected bool _proficientIntelligence;
        protected bool _proficientWisdom;
        protected bool _proficientCharisma;

        // ==================== Related Objects ====================

        protected RaceClass _race;
        protected BackgroundClass _background;
        protected InventoryClass _inventory;
        protected List<FeatureClass> _features;
        protected List<FeatClass> _feats;
        protected List<SkillClass> _skills;
        protected SpellBookClass? _spellBook;
        protected SpellcastingInfo? _spellcastingInfo;

        // ==================== Combat State ====================

        protected List<CharacterCondition> _conditions;
        protected Dictionary<string, int> _conditionDurations;
        protected string _concentrationSpellName;
        protected bool _isConcentrating;
        protected int _secondWindUses;
        protected int _maxActionSurgeUses;
        protected int _actionSurgeUsesRemaining;

        // ==================== Constructors ====================

        /// <summary>
        /// Default parameterless constructor for derived classes.
        /// </summary>
        protected Character()
        {
            InitializeCoreValues();
            InitializeRace();
            InitializeBackground();
            ApplyRaceTraits();
            CalculateBaseStats();
            CalculateProficiencyBonus();
        }

        /// <summary>
        /// Parameterized constructor for creating characters with specific values.
        /// </summary>
        public Character(
            string name,
            int level,
            RaceClass race,
            BackgroundClass background)
        {
            _name = name;
            _level = Math.Max(1, Math.Min(20, level));
            _experience = 0;
            _tempHitPoints = 0;
            _armorClass = 10;
            _speed = 30;
            _proficiencyBonus = 2;
            _initiativeModifier = 0;

            _strength = 10;
            _dexterity = 10;
            _constitution = 10;
            _intelligence = 10;
            _wisdom = 10;
            _charisma = 10;

            _proficientStrength = false;
            _proficientDexterity = false;
            _proficientConstitution = false;
            _proficientIntelligence = false;
            _proficientWisdom = false;
            _proficientCharisma = false;

            _race = race;
            _background = background;
            _inventory = new InventoryClass();
            _features = new List<FeatureClass>();
            _feats = new List<FeatClass>();
            _skills = new List<SkillClass>();
            _spellBook = null;
            _spellcastingInfo = null;

            _conditions = new List<CharacterCondition>();
            _conditionDurations = new Dictionary<string, int>();
            _concentrationSpellName = "";
            _isConcentrating = false;
            _secondWindUses = 0;
            _maxActionSurgeUses = 0;
            _actionSurgeUsesRemaining = 0;

            ApplyRaceTraits();
            CalculateBaseStats();
            CalculateProficiencyBonus();
        }

        // ==================== Initialization Helpers ====================

        private void InitializeCoreValues()
        {
            _name = "Unnamed";
            _level = 1;
            _experience = 0;
            _hitPoints = 0;
            _maxHitPoints = 0;
            _tempHitPoints = 0;
            _armorClass = 10;
            _speed = 30;
            _proficiencyBonus = 2;
            _initiativeModifier = 0;

            _strength = 10;
            _dexterity = 10;
            _constitution = 10;
            _intelligence = 10;
            _wisdom = 10;
            _charisma = 10;

            _proficientStrength = false;
            _proficientDexterity = false;
            _proficientConstitution = false;
            _proficientIntelligence = false;
            _proficientWisdom = false;
            _proficientCharisma = false;

            _race = new HumanRace();
            _background = new BackgroundImpl();
            _inventory = new InventoryClass();
            _features = new List<FeatureClass>();
            _feats = new List<FeatClass>();
            _skills = new List<SkillClass>();
            _spellBook = null;
            _spellcastingInfo = null;

            _conditions = new List<CharacterCondition>();
            _conditionDurations = new Dictionary<string, int>();
            _concentrationSpellName = "";
            _isConcentrating = false;
            _secondWindUses = 0;
            _maxActionSurgeUses = 0;
            _actionSurgeUsesRemaining = 0;
        }

        // ==================== Properties ====================

        // Core Properties
        public string Name { get => _name; set => _name = value; }
        public int Level { get => _level; set => _level = Math.Max(1, Math.Min(20, value)); }
        public int Experience { get => _experience; set => _experience = value; }
        public int HitPoints { get => _hitPoints; set => _hitPoints = Math.Max(0, value); }
        public int MaxHitPoints 
        { 
            get => _maxHitPoints; 
            set => _maxHitPoints = Math.Max(0, value); 
        }
        public int TempHitPoints { get => _tempHitPoints; set => _tempHitPoints = Math.Max(0, value); }
        public int ArmorClass { get => _armorClass; set => _armorClass = value; }
        public int Speed { get => _speed; set => _speed = Math.Max(0, value); }
        public int ProficiencyBonus { get => _proficiencyBonus; }
        public int InitiativeModifier { get => _initiativeModifier; set => _initiativeModifier = value; }

        // Ability Score Properties
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

        // Race & Background Properties
        public RaceClass Race { get => _race; set => _race = value; }
        public BackgroundClass Background { get => _background; set => _background = value; }

        // Object References
        public InventoryClass Inventory { get => _inventory; set => _inventory = value; }
        public List<FeatureClass> Features { get => _features; set => _features = value; }
        public List<FeatClass> Feats { get => _feats; set => _feats = value; }
        public List<SkillClass> Skills { get => _skills; set => _skills = value; }
        public SpellBookClass? SpellBook { get => _spellBook; set => _spellBook = value; }
        public SpellcastingInfo? SpellcastingInfo { get => _spellcastingInfo; set => _spellcastingInfo = value; }

        // Saving Throw Proficiency Properties
        public bool IsProficientInStrengthSaves { get => _proficientStrength; set => _proficientStrength = value; }
        public bool IsProficientInDexteritySaves { get => _proficientDexterity; set => _proficientDexterity = value; }
        public bool IsProficientInConstitutionSaves { get => _proficientConstitution; set => _proficientConstitution = value; }
        public bool IsProficientInIntelligenceSaves { get => _proficientIntelligence; set => _proficientIntelligence = value; }
        public bool IsProficientInWisdomSaves { get => _proficientWisdom; set => _proficientWisdom = value; }
        public bool IsProficientInCharismaSaves { get => _proficientCharisma; set => _proficientCharisma = value; }

        // Combat State Properties
        public List<CharacterCondition> Conditions { get => _conditions; }
        public Dictionary<string, int> ConditionDurations { get => _conditionDurations; }
        public string ConcentrationSpellName { get => _concentrationSpellName; }
        public bool IsConcentrating { get => _isConcentrating; }
        public int SecondWindUses { get => _secondWindUses; set => _secondWindUses = Math.Max(0, value); }
        public int MaxActionSurgeUses { get => _maxActionSurgeUses; }
        public int ActionSurgeUsesRemaining { get => _actionSurgeUsesRemaining; set => _actionSurgeUsesRemaining = Math.Max(0, value); }

        // ==================== Abstract Methods (for derived classes) ====================

        /// <summary>
        /// Implements class-specific special ability. Must be overridden by derived classes.
        /// </summary>
        public abstract void ClassSpecificAbility();

        /// <summary>
        /// Applies class features based on current level. Override in derived classes.
        /// Derived classes typically drive this through their own ApplyLevelFeatures logic,
        /// so the base implementation is a no-op.
        /// </summary>
        public virtual void ApplyClassFeatures() { }

        /// <summary>
        /// Returns class-specific details as a dictionary. Override in derived classes.
        /// </summary>
        public virtual Dictionary<string, object> GetClassDetails() => new Dictionary<string, object>();

        // ==================== Ability Modifier Methods ====================

        /// <summary>
        /// Calculates the modifier for a given ability score value.
        /// Formula: (score - 10) / 2, minimum -5
        /// </summary>
        public int GetAbilityModifier(int abilityScore)
        {
            return Math.Max(-5, (abilityScore - 10) / 2);
        }

        /// <summary>
        /// Gets the modifier for a specific ability by name.
        /// </summary>
        public int GetAbilityModifier(string abilityName)
        {
            return abilityName.ToLowerInvariant() switch
            {
                "strength" or "str" => GetAbilityModifier(_strength),
                "dexterity" or "dex" => GetAbilityModifier(_dexterity),
                "constitution" or "con" => GetAbilityModifier(_constitution),
                "intelligence" or "int" => GetAbilityModifier(_intelligence),
                "wisdom" or "wis" => GetAbilityModifier(_wisdom),
                "charisma" or "cha" => GetAbilityModifier(_charisma),
                _ => 0
            };
        }

        // ==================== Proficiency Bonus Method ====================

        /// <summary>
        /// Calculates the proficiency bonus based on character level.
        /// Levels 1-4: +2, Levels 5-8: +3, Levels 9-12: +4, Levels 13-16: +5, Levels 17-20: +6
        /// </summary>
        private void CalculateProficiencyBonus()
        {
            if (_level <= 4) _proficiencyBonus = 2;
            else if (_level <= 8) _proficiencyBonus = 3;
            else if (_level <= 12) _proficiencyBonus = 4;
            else if (_level <= 16) _proficiencyBonus = 5;
            else _proficiencyBonus = 6;
        }

        /// <summary>
        /// Gets the proficiency bonus for a specific level (static helper).
        /// </summary>
        public static int GetProficiencyBonusForLevel(int level)
        {
            if (level <= 4) return 2;
            if (level <= 8) return 3;
            if (level <= 12) return 4;
            if (level <= 16) return 5;
            return 6;
        }

        /// <summary>
        /// Recalculates derived stats (proficiency bonus, HP, AC, etc.) after
        /// ability scores or level have been changed externally. Useful right
        /// after building a character and setting its ability scores.
        /// </summary>
        public void RecalculateDerivedStats()
        {
            CalculateProficiencyBonus();
            CalculateBaseStats();
        }

        // ==================== Saving Throw Methods ====================

        /// <summary>
        /// Gets the total modifier for a saving throw, including proficiency if applicable.
        /// </summary>
        public int GetSavingThrowModifier(string abilityName)
        {
            int baseModifier = GetAbilityModifier(abilityName);
            bool isProficient = abilityName.ToLowerInvariant() switch
            {
                "strength" or "str" => _proficientStrength,
                "dexterity" or "dex" => _proficientDexterity,
                "constitution" or "con" => _proficientConstitution,
                "intelligence" or "int" => _proficientIntelligence,
                "wisdom" or "wis" => _proficientWisdom,
                "charisma" or "cha" => _proficientCharisma,
                _ => false
            };

            return baseModifier + (isProficient ? _proficiencyBonus : 0);
        }

        /// <summary>
        /// Makes a saving throw against a given DC. Returns true if the save succeeds.
        /// Player should roll d20 + modifier and compare to DC themselves.
        /// </summary>
        public virtual bool MakeSavingThrow(string abilityName, int dc)
        {
            int modifier = GetSavingThrowModifier(abilityName);
            int roll = new Random().Next(1, 21);
            int total = roll + modifier;
            bool success = total >= dc;

            Console.WriteLine($"=== {Name} makes a {abilityName} saving throw ===");
            Console.WriteLine($"Roll: d20({roll}) + {modifier} = {total} vs DC {dc}");
            Console.WriteLine(success ? "Result: SUCCESS!" : "Result: FAILED!");
            return success;
        }

        /// <summary>
        /// Makes an ability check for a given ability.
        /// Player should roll d20 + modifier and compare to the threshold themselves.
        /// </summary>
        public virtual bool MakeAbilityCheck(string abilityName)
        {
            int modifier = GetAbilityModifier(abilityName);
            int roll = new Random().Next(1, 21);
            int total = roll + modifier;

            Console.WriteLine($"=== {Name} makes an {abilityName} check ===");
            Console.WriteLine($"Roll: d20({roll}) + {modifier} = {total}");
            return true; // Always returns true - player compares to DC themselves
        }

        /// <summary>
        /// Checks if the character is proficient with a given ability/saving throw.
        /// </summary>
        public bool IsProficientWith(string abilityName)
        {
            return abilityName.ToLowerInvariant() switch
            {
                "strength" or "str" => _proficientStrength,
                "dexterity" or "dex" => _proficientDexterity,
                "constitution" or "con" => _proficientConstitution,
                "intelligence" or "int" => _proficientIntelligence,
                "wisdom" or "wis" => _proficientWisdom,
                "charisma" or "cha" => _proficientCharisma,
                _ => false
            };
        }

        // ==================== Initiative Method ====================

        /// <summary>
        /// Calculates the character's initiative roll.
        /// Returns: d20 + DEX modifier
        /// Player should roll the d20 and add the returned modifier.
        /// </summary>
        public virtual int CalculateInitiative()
        {
            int dexMod = GetAbilityModifier(_dexterity);
            int total = dexMod + _initiativeModifier;
            Console.WriteLine($"{Name}'s initiative modifier: +{total} (DEX +{dexMod}, other +{_initiativeModifier})");
            return total;
        }

        // ==================== Temp HP Methods ====================

        /// <summary>
        /// Grants temporary hit points to the character.
        /// Temp HP do not heal actual damage but act as a buffer.
        /// </summary>
        public virtual void GainTemporaryHP(int amount)
        {
            if (amount <= 0)
            {
                Console.WriteLine($"{Name} gains no temporary hit points.");
                return;
            }

            _tempHitPoints += amount;
            Console.WriteLine($"{Name} gains {amount} temporary hit points! (Total temp HP: {_tempHitPoints})");
        }

        /// <summary>
        /// Loses temporary hit points, removing them first before actual HP.
        /// </summary>
        public virtual void LoseTemporaryHP(int amount)
        {
            if (amount <= 0) return;

            int remainingTemp = Math.Min(_tempHitPoints, amount);
            _tempHitPoints -= remainingTemp;
            int actualDamage = amount - remainingTemp;

            Console.WriteLine($"{Name} loses {remainingTemp} temporary hit points.");

            if (actualDamage > 0)
            {
                Console.WriteLine($"({actualDamage} damage spills over to actual hit points.)");
            }
        }

        // ==================== Core Combat Methods ====================

        /// <summary>
        /// Performs a basic attack. Override in derived classes for class-specific behavior.
        /// </summary>
        public virtual void Attack()
        {
            Console.WriteLine($"{_name} attempts an attack.");
        }

        /// <summary>
        /// Takes damage, depleting temporary hit points first.
        /// </summary>
        public virtual void TakeDamage(int damage)
        {
            if (damage <= 0) return;

            // First drain temp HP
            if (_tempHitPoints > 0)
            {
                if (damage <= _tempHitPoints)
                {
                    _tempHitPoints -= damage;
                    Console.WriteLine($"{_name} takes {damage} damage, all absorbed by temporary hit points. (Temp HP: {_tempHitPoints})");
                    return;
                }

                damage -= _tempHitPoints;
                _tempHitPoints = 0;
            }

            // Then apply to actual HP
            int previousHP = _hitPoints;
            _hitPoints = Math.Max(0, _hitPoints - damage);
            Console.WriteLine($"{_name} takes {damage} damage. Remaining HP: {_hitPoints}/{_maxHitPoints}");

            if (_hitPoints == 0)
            {
                Console.WriteLine($"{_name} is at 0 hit points! Making death saving throws.");
            }
        }

        /// <summary>
        /// Heals the character up to their max hit points.
        /// </summary>
        public virtual void Heal(int amount)
        {
            if (amount <= 0) return;

            // Clear temp HP first (healing doesn't affect temp HP)
            int previousHP = _hitPoints;
            _hitPoints = Math.Min(_maxHitPoints, _hitPoints + amount);
            int actualHeal = _hitPoints - previousHP;

            Console.WriteLine($"{_name} heals for {actualHeal} hit points. Current HP: {_hitPoints}/{_maxHitPoints}");
        }

        /// <summary>
        /// Calculates and describes an attack roll.
        /// Returns a string describing the roll result. Player rolls d20 + returned bonus.
        /// </summary>
        public virtual string DescribeAttackRoll(string attackType = "melee")
        {
            int attackBonus = 0;

            if (attackType.ToLowerInvariant() == "melee" || attackType == "melee")
            {
                attackBonus = GetAbilityModifier(_strength) + _proficiencyBonus;
            }
            else if (attackType.ToLowerInvariant() == "ranged")
            {
                attackBonus = GetAbilityModifier(_dexterity) + _proficiencyBonus;
            }
            else if (attackType.ToLowerInvariant() == "spell")
            {
                // Use highest spellcasting ability (assume player sets SpellcastingInfo)
                attackBonus = _proficiencyBonus + Math.Max(
                    GetAbilityModifier(_intelligence),
                    Math.Max(GetAbilityModifier(_wisdom), GetAbilityModifier(_charisma))
                );
            }

            return $"{Name}'s {attackType} attack bonus: +{attackBonus}\nRoll d20 + {attackBonus} vs target AC";
        }

        /// <summary>
        /// Describes a critical hit: double the dice. Player should roll twice and add modifiers.
        /// </summary>
        public virtual string DescribeCriticalHit()
        {
            return $"{Name}'s attack is a CRITICAL HIT! Roll all damage dice twice.";
        }

        // ==================== Condition Methods ====================

        /// <summary>
        /// Applies a condition to the character with a duration (in levels/rounds).
        /// </summary>
        public virtual void ApplyCondition(CharacterCondition condition, int duration = 1)
        {
            if (condition == CharacterCondition.None) return;

            string conditionName = condition.ToString();

            if (_conditions.Contains(condition))
            {
                _conditionDurations[conditionName] = Math.Max(_conditionDurations.GetValueOrDefault(conditionName, 0), duration);
                Console.WriteLine($"{_name}'s {conditionName} condition refreshed to {duration} duration.");
            }
            else
            {
                _conditions.Add(condition);
                _conditionDurations[conditionName] = duration;
                Console.WriteLine($"{_name} gains the {conditionName} condition ({duration} duration).");
            }
        }

        /// <summary>
        /// Applies a condition by string name.
        /// </summary>
        public void ApplyCondition(string conditionName, int duration = 1)
        {
            if (Enum.TryParse(conditionName, true, out CharacterCondition condition))
            {
                ApplyCondition(condition, duration);
            }
            else
            {
                Console.WriteLine($"Unknown condition: {conditionName}");
            }
        }

        /// <summary>
        /// Removes a condition from the character.
        /// </summary>
        public virtual void RemoveCondition(CharacterCondition condition)
        {
            if (_conditions.Remove(condition))
            {
                _conditionDurations.Remove(condition.ToString());
                Console.WriteLine($"{_name} is no longer affected by {condition}.");
            }
        }

        /// <summary>
        /// Removes a condition by string name.
        /// </summary>
        public void RemoveCondition(string conditionName)
        {
            if (Enum.TryParse(conditionName, true, out CharacterCondition condition))
            {
                RemoveCondition(condition);
            }
        }

        /// <summary>
        /// Decrements all condition durations and removes expired conditions.
        /// </summary>
        public virtual void ProcessConditionDuration()
        {
            var expired = new List<string>();

            foreach (var kvp in _conditionDurations.ToList())
            {
                _conditionDurations[kvp.Key]--;
                if (_conditionDurations[kvp.Key] <= 0)
                {
                    expired.Add(kvp.Key);
                }
            }

            foreach (var conditionName in expired)
            {
                if (Enum.TryParse(conditionName, true, out CharacterCondition condition))
                {
                    RemoveCondition(condition);
                }
                _conditionDurations.Remove(conditionName);
            }

            if (expired.Count > 0)
            {
                Console.WriteLine($"{_name}'s conditions expired: {string.Join(", ", expired)}");
            }
        }

        /// <summary>
        /// Gets a list of currently active conditions.
        /// </summary>
        public virtual List<string> GetActiveConditions()
        {
            return _conditions.Select(c => c.ToString()).ToList();
        }

        /// <summary>
        /// Checks if the character is affected by a specific condition.
        /// </summary>
        public virtual bool IsAffectedBy(string effectName)
        {
            if (Enum.TryParse(effectName, true, out CharacterCondition condition))
            {
                return _conditions.Contains(condition);
            }

            // Check feature/feat names too
            return _features.Any(f => f.Name?.ToLowerInvariant() == effectName.ToLowerInvariant()) ||
                   _feats.Any(f => f.Name?.ToLowerInvariant() == effectName.ToLowerInvariant());
        }

        // ==================== Concentration Methods ====================

        /// <summary>
        /// Starts concentration on a spell. Returns false if already concentrating.
        /// </summary>
        public virtual bool StartConcentration(string spellName)
        {
            if (_isConcentrating)
            {
                Console.WriteLine($"{_name} is already concentrating on {_concentrationSpellName}.");
                return false;
            }

            _isConcentrating = true;
            _concentrationSpellName = spellName;
            Console.WriteLine($"{_name} begins concentrating on {spellName}.");
            return true;
        }

        /// <summary>
        /// Ends concentration on the current spell.
        /// </summary>
        public virtual bool EndConcentration()
        {
            if (!_isConcentrating)
            {
                Console.WriteLine($"{_name} is not concentrating on anything.");
                return false;
            }

            string endedSpell = _concentrationSpellName;
            _isConcentrating = false;
            _concentrationSpellName = "";
            Console.WriteLine($"{_name} ends concentration on {endedSpell}.");
            return true;
        }

        /// <summary>
        /// Makes a Constitution saving throw to maintain concentration.
        /// The DC is the higher of 10 or half the damage taken.
        /// Returns true if concentration succeeds. Player rolls d20 + CON mod vs DC.
        /// </summary>
        public virtual bool MakeConcentrationCheck(int dc)
        {
            if (!_isConcentrating)
            {
                Console.WriteLine($"{_name} is not concentrating on anything.");
                return false;
            }

            int conMod = GetAbilityModifier(_constitution);
            int roll = new Random().Next(1, 21);
            int total = roll + conMod;
            bool success = total >= dc;

            Console.WriteLine($"=== Concentration check for {_concentrationSpellName} ===");
            Console.WriteLine($"Roll: d20({roll}) + {conMod} = {total} vs DC {dc}");
            Console.WriteLine(success ? "Concentration maintained!" : "Concentration broken!");

            if (!success)
            {
                EndConcentration();
            }

            return success;
        }

        // ==================== Death Saving Throw Methods ====================

        /// <summary>
        /// Describes death saving throw mechanics for a character at 0 HP.
        /// Player rolls d20 each round.
        /// </summary>
        public virtual string DescribeDeathSaves()
        {
            if (_hitPoints > 0)
            {
                return $"{_name} is conscious and does not need to make death saving throws.";
            }

            return $"{_name} is at 0 hit points! Each round, roll d20:\n" +
                   "  1-9: Failure (3 failures = death)\n" +
                   "  10-19: Success (3 successes = stable\n" +
                   "  20: Recover 1 HP";
        }

        /// <summary>
        /// Makes a death saving throw roll. Player should roll d20 and compare.
        /// </summary>
        public virtual bool MakeDeathSave(bool success)
        {
            if (_hitPoints > 0)
            {
                Console.WriteLine($"{_name} is not at 0 HP.");
                return false;
            }

            Console.WriteLine($"{_name} makes a death saving throw: {(success ? "Success!" : "Failure!")}");
            return success;
        }

        // ==================== Level & Rest Methods ====================

        /// <summary>
        /// Levels up the character, recalculating stats and applying level features.
        /// </summary>
        public virtual void LevelUp()
        {
            int previousLevel = _level;
            _level++;
            CalculateProficiencyBonus();
            CalculateBaseStats();
            ApplyClassFeatures();

            Console.WriteLine($"{_name} has reached level {_level}!");
            Console.WriteLine($"Proficiency bonus is now +{_proficiencyBonus}");
        }

        /// <summary>
        /// Takes a long rest, recovering all HP and resetting per-day abilities.
        /// </summary>
        public virtual void LongRest()
        {
            int previousHP = _hitPoints;
            _hitPoints = _maxHitPoints;
            _tempHitPoints = 0;

            Console.WriteLine($"{_name} takes a long rest and recovers all hit points ({previousHP} -> {_hitPoints}/{_maxHitPoints}).");
            OnLongRest();
        }

        /// <summary>
        /// Hook for derived classes to restore resources on long rest.
        /// </summary>
        protected virtual void OnLongRest()
        {
        }

        /// <summary>
        /// Takes a short rest, recovering some resources.
        /// </summary>
        public virtual void ShortRest()
        {
            Console.WriteLine($"{_name} takes a short rest.");
            OnShortRest();
        }

        /// <summary>
        /// Hook for derived classes to restore resources on short rest.
        /// </summary>
        protected virtual void OnShortRest()
        {
        }

        // ==================== Race & Background Integration Hooks ====================

        /// <summary>
        /// Initializes race-specific values. Override in derived classes.
        /// </summary>
        protected virtual void InitializeRace()
        {
        }

        /// <summary>
        /// Applies racial trait modifiers to ability scores and other properties.
        /// Override in derived classes.
        /// </summary>
        protected virtual void ApplyRaceTraits()
        {
            // Default: no changes, subclasses apply their race bonuses
            if (_race != null && _race.Name != "Unknown")
            {
                Console.WriteLine($"{_name}'s racial traits from {_race.Name} are applied.");
            }
        }

        /// <summary>
        /// Initializes background-specific values. Override in derived classes.
        /// </summary>
        protected virtual void InitializeBackground()
        {
        }

        /// <summary>
        /// Calculates base stats (HP, AC) based on class and race.
        /// Derived classes override this to set class-specific HP/AC; the base
        /// implementation is a no-op so subclasses may safely call base.CalculateBaseStats().
        /// </summary>
        protected virtual void CalculateBaseStats()
        {
        }

        // ==================== Spellcasting Info Methods ====================

        /// <summary>
        /// Gets or creates the spellcasting info for this character.
        /// Override in derived classes to provide class-specific spellcasting details.
        /// </summary>
        public virtual SpellcastingInfo? GetSpellcastingInfo() => _spellcastingInfo;

        /// <summary>
        /// Updates the spellcasting info based on current ability scores and level.
        /// </summary>
        public virtual void UpdateSpellcastingInfo(string abilityName, int abilityScore)
        {
            _spellcastingInfo = new SpellcastingInfo(abilityName, _level, abilityScore);
        }

        // ==================== Display Methods ====================

        /// <summary>
        /// Displays a formatted character sheet to the console.
        /// </summary>
        public virtual void DisplayCharacter()
        {
            Console.WriteLine();
            Console.WriteLine($"=== {_name} (Level {_level}) ===");
            Console.WriteLine($"Race: {_race?.Name ?? "Unknown"} | Background: {_background?.Name ?? "Unknown"}");
            Console.WriteLine($"Hit Points: {_hitPoints}/{_maxHitPoints} | AC: {_armorClass} | Speed: {_speed}");
            Console.WriteLine();
            Console.WriteLine("Ability Scores:");
            Console.WriteLine($"  Strength:    {_strength} (mod +{GetAbilityModifier(_strength)})");
            Console.WriteLine($"  Dexterity:   {_dexterity} (mod +{GetAbilityModifier(_dexterity)})");
            Console.WriteLine($"  Constitution:{_constitution} (mod +{GetAbilityModifier(_constitution)})");
            Console.WriteLine($"  Intelligence:{_intelligence} (mod +{GetAbilityModifier(_intelligence)})");
            Console.WriteLine($"  Wisdom:      {_wisdom} (mod +{GetAbilityModifier(_wisdom)})");
            Console.WriteLine($"  Charisma:    {_charisma} (mod +{GetAbilityModifier(_charisma)})");

            Console.WriteLine();
            Console.WriteLine("Saving Throw Proficiencies:");
            Console.WriteLine($"  STR: {(IsProficientInStrengthSaves ? $"+{GetSavingThrowModifier("strength")}" : $"{GetAbilityModifier("strength")}")}");
            Console.WriteLine($"  DEX: {(IsProficientInDexteritySaves ? $"+{GetSavingThrowModifier("dexterity")}" : $"{GetAbilityModifier("dexterity")}")}");
            Console.WriteLine($"  CON: {(IsProficientInConstitutionSaves ? $"+{GetSavingThrowModifier("constitution")}" : $"{GetAbilityModifier("constitution")}")}");
            Console.WriteLine($"  INT: {(IsProficientInIntelligenceSaves ? $"+{GetSavingThrowModifier("intelligence")}" : $"{GetAbilityModifier("intelligence")}")}");
            Console.WriteLine($"  WIS: {(IsProficientInWisdomSaves ? $"+{GetSavingThrowModifier("wisdom")}" : $"{GetAbilityModifier("wisdom")}")}");
            Console.WriteLine($"  CHA: {(IsProficientInCharismaSaves ? $"+{GetSavingThrowModifier("charisma")}" : $"{GetAbilityModifier("charisma")}")}");

            var spellcastingInfo = GetSpellcastingInfo();
            if (spellcastingInfo != null)
            {
                Console.WriteLine();
                spellcastingInfo.Display();
            }

            if (_conditions.Count > 0)
            {
                Console.WriteLine();
                Console.WriteLine($"Active Conditions: {string.Join(", ", GetActiveConditions())}");
            }

            Console.WriteLine();
            Console.WriteLine("=== End Character Sheet ===");
            Console.WriteLine();
        }

        /// <summary>
        /// Gets a compact character summary (5-6 lines).
        /// </summary>
        public virtual string GetCharacterSummary()
        {
            var sb = new StringBuilder();
            sb.AppendLine($"=== {_name} ===");
            sb.AppendLine($"Level {_level} {_race?.Name ?? "Unknown"} {_background?.Name ?? "Unknown"}");
            sb.AppendLine($"HP: {_hitPoints}/{_maxHitPoints} | AC: {_armorClass} | Speed: {_speed}");
            sb.AppendLine($"STR:{_strength} DEX:{_dexterity} CON:{_constitution} INT:{_intelligence} WIS:{_wisdom} CHA:{_charisma}");
            sb.AppendLine($"Proficiency: +{_proficiencyBonus} | Initiative: +{GetAbilityModifier(_dexterity) + _initiativeModifier}");

            if (_tempHitPoints > 0)
            {
                sb.AppendLine($"Temp HP: {_tempHitPoints}");
            }

            return sb.ToString();
        }

        /// <summary>
        /// Gets a full formatted character sheet as a string.
        /// </summary>
        public virtual string GetFullCharacterSheet()
        {
            var sb = new StringBuilder();

            sb.AppendLine("=".PadRight(50, '='));
            sb.AppendLine($"  CHARACTER SHEET: {_name}");
            sb.AppendLine("=".PadRight(50, '='));

            // Core Info
            sb.AppendLine();
            sb.AppendLine("--- Core Information ---");
            sb.AppendLine($"Name: {_name}");
            sb.AppendLine($"Level: {_level}");
            sb.AppendLine($"Race: {_race?.Name ?? "Unknown"}");
            sb.AppendLine($"Background: {_background?.Name ?? "Unknown"}");
            sb.AppendLine($"Experience: {_experience}");

            // Combat Stats
            sb.AppendLine();
            sb.AppendLine("--- Combat Stats ---");
            sb.AppendLine($"Hit Points: {_hitPoints}/{_maxHitPoints}{(_tempHitPoints > 0 ? $" (+{_tempHitPoints} temp)" : "")}");
            sb.AppendLine($"Armor Class: {_armorClass}");
            sb.AppendLine($"Speed: {_speed} feet");
            sb.AppendLine($"Initiative: +{GetAbilityModifier(_dexterity) + _initiativeModifier}");
            sb.AppendLine($"Proficiency Bonus: +{_proficiencyBonus}");

            // Ability Scores
            sb.AppendLine();
            sb.AppendLine("--- Ability Scores ---");
            sb.AppendLine($"Strength:  {_strength} (mod +{GetAbilityModifier(_strength)})");
            sb.AppendLine($"Dexterity: {_dexterity} (mod +{GetAbilityModifier(_dexterity)})");
            sb.AppendLine($"Constitution: {_constitution} (mod +{GetAbilityModifier(_constitution)})");
            sb.AppendLine($"Intelligence: {_intelligence} (mod +{GetAbilityModifier(_intelligence)})");
            sb.AppendLine($"Wisdom: {_wisdom} (mod +{GetAbilityModifier(_wisdom)})");
            sb.AppendLine($"Charisma: {_charisma} (mod +{GetAbilityModifier(_charisma)})");

            // Saving Throws
            sb.AppendLine();
            sb.AppendLine("--- Saving Throws ---");
            sb.AppendLine($"Strength:  {GetSavingThrowModifier("strength")} {(IsProficientInStrengthSaves ? "✓" : "")}");
            sb.AppendLine($"Dexterity: {GetSavingThrowModifier("dexterity")} {(IsProficientInDexteritySaves ? "✓" : "")}");
            sb.AppendLine($"Constitution: {GetSavingThrowModifier("constitution")} {(IsProficientInConstitutionSaves ? "✓" : "")}");
            sb.AppendLine($"Intelligence: {GetSavingThrowModifier("intelligence")} {(IsProficientInIntelligenceSaves ? "✓" : "")}");
            sb.AppendLine($"Wisdom: {GetSavingThrowModifier("wisdom")} {(IsProficientInWisdomSaves ? "✓" : "")}");
            sb.AppendLine($"Charisma: {GetSavingThrowModifier("charisma")} {(IsProficientInCharismaSaves ? "✓" : "")}");

            // Conditions
            if (_conditions.Count > 0)
            {
                sb.AppendLine();
                sb.AppendLine("--- Active Conditions ---");
                foreach (var condition in _conditions)
                {
                    int duration = _conditionDurations.GetValueOrDefault(condition.ToString(), 0);
                    sb.AppendLine($"  {condition} ({duration} remaining)");
                }
            }

            // Concentration
            if (_isConcentrating)
            {
                sb.AppendLine();
                sb.AppendLine($"--- Concentration: {_concentrationSpellName} ---");
            }

            // Spellcasting
            var sci = GetSpellcastingInfo();
            if (sci != null)
            {
                sb.AppendLine();
                sb.AppendLine("--- Spellcasting ---");
                sb.AppendLine($"Ability: {sci.SpellCastingAbility}");
                sb.AppendLine($"Save DC: {sci.SpellSaveDC}");
                sb.AppendLine($"Attack Bonus: +{sci.SpellAttackBonus}");
                sb.AppendLine($"Cantrips Known: {sci.CantripsKnown}");
                if (sci.MaxPreparedSpells > 0)
                {
                    sb.AppendLine($"Max Prepared Spells: {sci.MaxPreparedSpells}");
                }
            }

            // Features
            if (_features.Count > 0)
            {
                sb.AppendLine();
                sb.AppendLine("--- Features ---");
                foreach (var feature in _features)
                {
                    sb.AppendLine($"  {feature.Name}");
                }
            }

            // Feats
            if (_feats.Count > 0)
            {
                sb.AppendLine();
                sb.AppendLine("--- Feats ---");
                foreach (var feat in _feats)
                {
                    sb.AppendLine($"  {feat.Name}");
                }
            }

            sb.AppendLine();
            sb.AppendLine("=".PadRight(50, '='));

            return sb.ToString();
        }

        // ==================== Utility Methods ====================

        /// <summary>
        /// Resets per-day abilities and resources. Called after long rest.
        /// Derived classes should override OnLongRest to add class-specific resets.
        /// </summary>
        public virtual void ResetPerDayAbilities()
        {
            _actionSurgeUsesRemaining = _maxActionSurgeUses;
            _secondWindUses = 1;
            Console.WriteLine($"{_name}'s per-day abilities have been reset.");
        }

        /// <summary>
        /// Resets combat-specific state (temp HP, conditions cleared at end of combat).
        /// </summary>
        public virtual void ResetCombatState()
        {
            _tempHitPoints = 0;
            _conditions.Clear();
            _conditionDurations.Clear();

            if (_isConcentrating)
            {
                EndConcentration();
            }

            Console.WriteLine($"{_name}'s combat state has been reset.");
        }

        // ==================== Serialization Methods ====================

        /// <summary>
        /// Serializes the character's current state to a dictionary.
        /// </summary>
        public virtual Dictionary<string, object> Serialize()
        {
            return new Dictionary<string, object>
            {
                ["Name"] = _name,
                ["Level"] = _level,
                ["Experience"] = _experience,
                ["HitPoints"] = _hitPoints,
                ["MaxHitPoints"] = _maxHitPoints,
                ["TempHitPoints"] = _tempHitPoints,
                ["ArmorClass"] = _armorClass,
                ["Speed"] = _speed,
                ["Strength"] = _strength,
                ["Dexterity"] = _dexterity,
                ["Constitution"] = _constitution,
                ["Intelligence"] = _intelligence,
                ["Wisdom"] = _wisdom,
                ["Charisma"] = _charisma,
                ["ProficientSTR"] = _proficientStrength,
                ["ProficientDEX"] = _proficientDexterity,
                ["ProficientCON"] = _proficientConstitution,
                ["ProficientINT"] = _proficientIntelligence,
                ["ProficientWIS"] = _proficientWisdom,
                ["ProficientCHA"] = _proficientCharisma,
                ["Conditions"] = string.Join(",", _conditions.Select(c => c.ToString())),
                ["ConcentrationSpell"] = _concentrationSpellName
            };
        }

        /// <summary>
        /// Deserializes a character's state from a dictionary.
        /// </summary>
        public virtual void Deserialize(Dictionary<string, object> data)
        {
            if (data.TryGetValue("Name", out var name)) _name = name as string ?? _name;
            if (data.TryGetValue("Level", out var level)) _level = Convert.ToInt32(level);
            if (data.TryGetValue("Experience", out var exp)) _experience = Convert.ToInt32(exp);
            if (data.TryGetValue("HitPoints", out var hp)) _hitPoints = Convert.ToInt32(hp);
            if (data.TryGetValue("MaxHitPoints", out var maxHp)) _maxHitPoints = Convert.ToInt32(maxHp);
            if (data.TryGetValue("TempHitPoints", out var tempHp)) _tempHitPoints = Convert.ToInt32(tempHp);
            if (data.TryGetValue("ArmorClass", out var ac)) _armorClass = Convert.ToInt32(ac);
            if (data.TryGetValue("Speed", out var speed)) _speed = Convert.ToInt32(speed);
            if (data.TryGetValue("Strength", out var str)) _strength = Convert.ToInt32(str);
            if (data.TryGetValue("Dexterity", out var dex)) _dexterity = Convert.ToInt32(dex);
            if (data.TryGetValue("Constitution", out var con)) _constitution = Convert.ToInt32(con);
            if (data.TryGetValue("Intelligence", out var intVal)) _intelligence = Convert.ToInt32(intVal);
            if (data.TryGetValue("Wisdom", out var wis)) _wisdom = Convert.ToInt32(wis);
            if (data.TryGetValue("Charisma", out var cha)) _charisma = Convert.ToInt32(cha);
            if (data.TryGetValue("ProficientSTR", out var pStr)) _proficientStrength = Convert.ToBoolean(pStr);
            if (data.TryGetValue("ProficientDEX", out var pDex)) _proficientDexterity = Convert.ToBoolean(pDex);
            if (data.TryGetValue("ProficientCON", out var pCon)) _proficientConstitution = Convert.ToBoolean(pCon);
            if (data.TryGetValue("ProficientINT", out var pInt)) _proficientIntelligence = Convert.ToBoolean(pInt);
            if (data.TryGetValue("ProficientWIS", out var pWis)) _proficientWisdom = Convert.ToBoolean(pWis);
            if (data.TryGetValue("ProficientCHA", out var pCha)) _proficientCharisma = Convert.ToBoolean(pCha);
            if (data.TryGetValue("ConcentrationSpell", out var concSpell)) _concentrationSpellName = concSpell as string ?? "";

            CalculateProficiencyBonus();
        }
    }
}