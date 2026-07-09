using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace DnDCharacterManager.Spell
{
    /// <summary>
    /// Represents a D&D spell with all its properties and effects.
    /// </summary>
    public class Spell
    {
        // ===== Core Properties =====
        protected string _name;
        protected int _level;                    // Spell level (0 = cantrip, 1-9)
        protected School _school;                // School of magic
        protected string _castingTime;           // "1 action", "1 bonus action", etc.
        protected string _range;                 // "Self", "60 feet", "150 feet", etc.
        protected string _duration;              // "Instantaneous", "Concentration, up to 1 minute", etc.
        
        // ===== Additional Properties =====
        protected string _description;           // Full description of what the spell does
        protected SpellComponent _components;    // V, S, M components required
        protected string _materialComponents;    // Material components if any (e.g., "a bit of fur")
        protected DamageType _damageType;        // Type of damage dealt
        protected string _damageAmount;          // Dice notation (e.g., "1d8", "3d6")
        protected string _savingThrow;           // Type of saving throw (e.g., "Dexterity DC 13")
        protected string _targetType;            // "Self", "One creature", "Point" etc.
        protected int _targetCount;              // Number of targets
        protected string _areaOfEffect;          // Area shape and size (e.g., "15-foot cone")
        protected bool _concentration;           // Whether spell requires concentration
        protected bool _ritual;                  // Whether spell can be cast as ritual
        protected int? _spellSlotLevel;          // Minimum class level to use this spell

        // ===== Constructors =====
        public Spell()
        {
            _name = "Unnamed Spell";
            _level = 0;
            _school = School.None;
            _castingTime = "1 action";
            _range = "Self";
            _duration = "Instantaneous";
            _description = "";
            _components = SpellComponent.None;
            _materialComponents = "";
            _damageType = DamageType.None;
            _damageAmount = "";
            _savingThrow = "";
            _targetType = "Self";
            _targetCount = 0;
            _areaOfEffect = "";
            _concentration = false;
            _ritual = false;
            _spellSlotLevel = null;
        }

        public Spell(string name, int level, School school, string castingTime, string range, string duration)
        {
            _name = name;
            _level = level;
            _school = school;
            _castingTime = castingTime;
            _range = range;
            _duration = duration;
            _description = "";
            _components = SpellComponent.None;
            _materialComponents = "";
            _damageType = DamageType.None;
            _damageAmount = "";
            _savingThrow = "";
            _targetType = "Self";
            _targetCount = 0;
            _areaOfEffect = "";
            _concentration = false;
            _ritual = false;
            _spellSlotLevel = null;
        }

        // ===== Full Constructor =====
        public Spell(string name, int level, School school, string castingTime, string range, string duration,
                     string description, SpellComponent components = SpellComponent.None, string materialComponents = "",
                     DamageType damageType = DamageType.None, string damageAmount = "", string savingThrow = "",
                     string targetType = "Self", int targetCount = 0, string areaOfEffect = "",
                     bool concentration = false, bool ritual = false)
        {
            _name = name;
            _level = level;
            _school = school;
            _castingTime = castingTime;
            _range = range;
            _duration = duration;
            _description = description;
            _components = components;
            _materialComponents = materialComponents;
            _damageType = damageType;
            _damageAmount = damageAmount;
            _savingThrow = savingThrow;
            _targetType = targetType;
            _targetCount = targetCount;
            _areaOfEffect = areaOfEffect;
            _concentration = concentration;
            _ritual = ritual;
            _spellSlotLevel = null;
        }

        // ===== Properties =====
        public string Name { get => _name; set => _name = value; }
        public int Level { get => _level; set => _level = value; }
        public School School { get => _school; set => _school = value; }
        public string CastingTime { get => _castingTime; set => _castingTime = value; }
        public string Range { get => _range; set => _range = value; }
        public string Duration { get => _duration; set => _duration = value; }
        public string Description { get => _description; set => _description = value; }
        public SpellComponent Components { get => _components; set => _components = value; }
        public string MaterialComponents { get => _materialComponents; set => _materialComponents = value; }
        public DamageType DamageType { get => _damageType; set => _damageType = value; }
        public string DamageAmount { get => _damageAmount; set => _damageAmount = value; }
        public string SavingThrow { get => _savingThrow; set => _savingThrow = value; }
        public string TargetType { get => _targetType; set => _targetType = value; }
        public int TargetCount { get => _targetCount; set => _targetCount = value; }
        public string AreaOfEffect { get => _areaOfEffect; set => _areaOfEffect = value; }
        public bool IsConcentration { get => _concentration; set => _concentration = value; }
        public bool IsRitual { get => _ritual; set => _ritual = value; }
        public int? SpellSlotLevel { get => _spellSlotLevel; set => _spellSlotLevel = value; }

        // ===== Methods =====
        /// <summary>
        /// Gets the component string for display (e.g., "V, S, M").
        /// </summary>
        public string GetComponentString()
        {
            var parts = new List<string>();
            if ((_components & SpellComponent.Verbal) == SpellComponent.Verbal) parts.Add("V");
            if ((_components & SpellComponent.Somatic) == SpellComponent.Somatic) parts.Add("S");
            if ((_components & SpellComponent.Material) == SpellComponent.Material) parts.Add("M");
            return string.Join(", ", parts);
        }

        /// <summary>
        /// Gets a brief summary line for the spell.
        /// </summary>
        public string GetBriefDescription()
        {
            var info = new List<string>();
            
            if (!string.IsNullOrEmpty(_damageAmount))
            {
                info.Add($"{_damageAmount} {_damageType.ToString().ToLower()} damage");
            }
            
            if (!string.IsNullOrEmpty(_savingThrow))
            {
                info.Add(_savingThrow);
            }
            
            if (_concentration)
            {
                info.Add("Concentration");
            }

            string details = info.Count > 0 ? $" - {string.Join(", ", info)}" : "";
            return $"{_name} ({_level}-level {_school}){details}";
        }

        /// <summary>
        /// Simulates casting the spell (prints effect).
        /// </summary>
        public virtual void Cast()
        {
            if (!string.IsNullOrEmpty(_description))
            {
                Console.WriteLine($"Casting {_name}: {_description}");
            }
            else
            {
                Console.WriteLine($"{_name} is cast! (Level {_level}, {_school})");
            }
        }

        /// <summary>
        /// Returns formatted spell information.
        /// </summary>
        public override string ToString()
        {
            var lines = new List<string>();
            lines.Add($"=== {_name} ===");
            lines.Add($"Level: {_level} | School: {_school}");
            lines.Add($"Casting Time: {_castingTime} | Range: {_range}");
            lines.Add($"Duration: {_duration}");
            
            if (_components != SpellComponent.None)
            {
                lines.Add($"Components: {GetComponentString()}");
            }
            if (!string.IsNullOrEmpty(_materialComponents))
            {
                lines.Add($"Material: {_materialComponents}");
            }
            
            if (_targetCount > 0)
            {
                lines.Add($"Targets: {_targetCount} {_targetType}");
            }
            else
            {
                lines.Add($"Target: {_targetType}");
            }
            
            if (!string.IsNullOrEmpty(_areaOfEffect))
            {
                lines.Add($"Area: {_areaOfEffect}");
            }
            
            if (!_concentration)
            {
                lines.Add($"Concentration: No");
            }
            if (_ritual)
            {
                lines.Add($"Ritual: Yes");
            }
            
            if (!string.IsNullOrEmpty(_damageAmount))
            {
                lines.Add($"Damage: {_damageAmount} {_damageType.ToString().ToLower()}");
            }
            
            if (!string.IsNullOrEmpty(_savingThrow))
            {
                lines.Add($"Saving Throw: {_savingThrow}");
            }
            
            if (!string.IsNullOrEmpty(_description))
            {
                lines.Add($"Description: {_description}");
            }
            
            return string.Join("\n", lines);
        }
    }

    // ==================== Enums ====================

    /// <summary>
    /// Enum for schools of magic.
    /// </summary>
    public enum School
    {
        None,
        Abjuration,     // Protective magic
        Conjuration,    // Summons creatures/objects
        Divination,     // Reveals information
        Enchantment,    // Affects minds
        Evocation,      // Energy projection
        Illusion,       // Deceptive patterns
        Necromancy,     // Life/energy manipulation
        Transmutation   // Changes materials/properties
    }

    /// <summary>
    /// Enum for spell damage types.
    /// </summary>
    public enum DamageType
    {
        None,
        Acid,           // Corrosive substance
        Cold,           // Freezing energy
        Fire,           // Hot blaze
        Force,          // Magical power
        Lightning,      // Electric discharge
        Necrotic,       // Life force drain
        Poison,         // Toxic venom
        Psychic,        // Mental power
        Radiant,        // Divine light
        Thunder,        // Loud noise
        Bleeding        // Protective barrier (renamed from Armor)
    }

    /// <summary>
    /// Enum for spell component types (bitflags).
    /// </summary>
    [Flags]
    public enum SpellComponent
    {
        None = 0,
        Verbal = 1,           // V - Chanting/incantation
        Somatic = 2,          // S - Hand gestures
        Material = 4,         // M - Physical objects
        VerbalSomatic = 3,    // V + S
        VerbalMaterial = 5,   // V + M
        SomaticMaterial = 6,  // S + M
        VerbalSomaticMaterial = 7 // V + S + M
    }

    // ==================== Cantrips (Level 0 Spells) ====================

    public class AcidSplash : Spell
    {
        public AcidSplash() : base(
            "Acid Splash", 0, School.Evocation, "1 action", "60 feet", "Instantaneous",
            "You hurl a drop of acid at a creature within range.",
            SpellComponent.VerbalSomatic, damageType: DamageType.Acid, damageAmount: "1d6",
            targetType: "creature", savingThrow: "Dexterity DC 13")
        {
            _areaOfEffect = "5-foot radius sphere";
            _targetCount = 1;
        }
    }

    public class BurningHands : Spell
    {
        public BurningHands() : base(
            "Burning Hands", 0, School.Evocation, "1 action", "Self", "Instantaneous",
            "Burning fingers of flame shoot from your hand.",
            SpellComponent.VerbalSomatic, damageType: DamageType.Fire, damageAmount: "1d4",
            areaOfEffect: "15-foot cone", savingThrow: "Dexterity DC 13")
        {
            _targetCount = 1;
        }
    }

    public class DetectMagic : Spell
    {
        public DetectMagic() : base(
            "Detect Magic", 0, School.Divination, "1 action", "Self", "Concentration, up to 10 minutes",
            "You sense the presence of magic within range.",
            SpellComponent.VerbalSomatic, ritual: true)
        {
            _targetType = "Self";
        }
    }

    public class FaerieFire : Spell
    {
        public FaerieFire() : base(
            "Faerie Fire", 0, School.Evocation, "1 action", "60 feet", "Concentration, up to 1 minute",
            "Every object in a 20-foot cube within range is outlined in blue, green, or violet.",
            SpellComponent.Verbal,
            areaOfEffect: "20-foot cube", savingThrow: "Dexterity DC 13")
        {
            _targetCount = 5;
        }
    }

    public class FakeLife : Spell
    {
        public FakeLife() : base(
            "Fake Life", 0, School.Necromancy, "1 action", "Self", "Instantaneous",
            "You create a fake version of yourself that lasts for 1 minute.",
            SpellComponent.VerbalSomaticMaterial, materialComponents: "a doll-like figure")
        {
            _targetType = "Self";
        }
    }

    public class Guidance : Spell
    {
        public Guidance() : base(
            "Guidance", 0, School.Divination, "1 bonus action", "Touch", "Concentration, up to 1 minute",
            "You touch one creature. Once before the target makes an ability check, it can roll a d4 and add the number to the ability check.",
            SpellComponent.VerbalSomatic)
        {
            _targetType = "creature";
            _targetCount = 1;
        }
    }

    public class HealingWord : Spell
    {
        public HealingWord() : base(
            "Healing Word", 0, School.Evocation, "1 bonus action", "60 feet", "Instantaneous",
            "A creature of your choice within range regains hit points. The target regains 1d4 + Spellcasting Ability modifier hit points.",
            SpellComponent.Verbal, targetType: "creature", targetCount: 1)
        {
            _damageAmount = "1d4";
        }
    }

    public class MageArmor : Spell
    {
        public MageArmor() : base(
            "Mage Armor", 0, School.Abjuration, "1 action", "Touch", "8 hours",
            "You touch a willing creature who isn't wearing armor. Until the spell ends, the target's base AC becomes 13 + its Dexterity modifier.",
            SpellComponent.VerbalSomaticMaterial, materialComponents: "a piece of cured leather")
        {
            _targetType = "willing creature";
            _targetCount = 1;
        }
    }

    public class MagicMissile : Spell
    {
        public MagicMissile() : base(
            "Magic Missile", 0, School.Evocation, "1 action", "120 feet", "Instantaneous",
            "You create three glowing darts of magical force. Each dart hits a creature of your choice that you can see within range.",
            SpellComponent.VerbalSomatic, damageType: DamageType.Force, damageAmount: "1d4+1",
            targetType: "creature", targetCount: 3)
        { }
    }

    public class Mending : Spell
    {
        public Mending() : base(
            "Mending", 0, School.Transmutation, "1 minute", "Touch", "Instantaneous",
            "This spell repairs a single cracked, broken, or shattered object you can see within range.",
            SpellComponent.VerbalSomaticMaterial)
        {
            _targetType = "object";
        }
    }

    public class ProtectionOfEvard : Spell
    {
        public ProtectionOfEvard() : base(
            "Protection of Evard", 0, School.Illusion, "1 action", "150 feet", "Concentration, up to 1 minute",
            "An invisible, moth-like creature forms a shield around you. It blocks attacks but not your spells.",
            SpellComponent.VerbalSomaticMaterial, materialComponents: "a piece of living crystal")
        {
            _targetType = "Self";
        }
    }

    public class Shield : Spell
    {
        public Shield() : base(
            "Shield", 0, School.Abjuration, "1 reaction", "60 feet", "1 round",
            "An invisible barrier of magical force appears and protects you. Until the start of your next turn, you have a +5 bonus to AC.",
            SpellComponent.VerbalSomatic)
        {
            _targetType = "Self";
        }
    }

    public class Shatter : Spell
    {
        public Shatter() : base(
            "Shatter", 0, School.Evocation, "1 action", "60 feet", "Instantaneous",
            "A creature of your choice within range must make a Constitution saving throw. On a failed save, the target takes 1d8 thunder damage.",
            SpellComponent.VerbalSomatic, damageType: DamageType.Thunder, damageAmount: "1d8",
            areaOfEffect: "5-foot radius sphere", savingThrow: "Constitution DC 13")
        {
            _targetCount = 5;
        }
    }

    public class ShockingGrasp : Spell
    {
        public ShockingGrasp() : base(
            "Shocking Grasp", 0, School.Evocation, "1 action", "Touch", "Instantaneous",
            "You create a flickering energy crackling at your fingertips. Make a melee spell attack. On hit, target takes 1d8 lightning damage.",
            SpellComponent.VerbalSomatic, damageType: DamageType.Lightning, damageAmount: "1d8")
        {
            _targetType = "creature";
            _targetCount = 1;
        }
    }

    public class TruthSeeing : Spell
    {
        public TruthSeeing() : base(
            "Truth Seeing", 0, School.Divination, "1 action", "Touch", "Concentration, up to 1 minute",
            "You touch a willing creature. Its illusions become invisible, and disguised creatures revert to their normal form.",
            SpellComponent.VerbalSomatic)
        {
            _targetType = "willing creature";
            _targetCount = 1;
        }
    }

    public class SacredFlame : Spell
    {
        public SacredFlame() : base(
            "Sacred Flame", 0, School.Evocation, "1 action", "60 feet", "Instantaneous",
            "Flaming light falls from the sky. A creature in that area must make a Dexterity saving throw. On a failure, it takes 1d8 radiant damage.",
            SpellComponent.VerbalSomatic, damageType: DamageType.Radiant, damageAmount: "1d8",
            savingThrow: "Dexterity DC 13")
        {
            _targetType = "creature";
            _targetCount = 1;
        }
    }

    public class ThunderousStrike : Spell
    {
        public ThunderousStrike() : base(
            "Thunderous Strike", 0, School.Evocation, "1 action", "Self", "Instantaneous",
            "You deliver a blow that resonates with thunderous force. The next melee attack deals extra thunder damage.",
            SpellComponent.VerbalSomatic, damageType: DamageType.Thunder, damageAmount: "1d6")
        {
            _targetType = "creature";
            _targetCount = 1;
        }
    }

    public class WizardSpell : Spell
    {
        public WizardSpell() : base(
            "Wizard Spell", 0, School.Evocation, "1 action", "Varies", "Varies",
            "A generic spell that can take various forms depending on the caster's choices.",
            SpellComponent.VerbalSomatic)
        {
            _targetType = "varies";
        }
    }

    public class Command : Spell
    {
        public Command() : base(
            "Command", 0, School.Enchantment, "1 bonus action", "60 feet", "Instantaneous",
            "You speak a one-word command. A creature you can see within range must succeed on a Wisdom save or follow the command.",
            SpellComponent.Verbal,
            targetType: "creature", savingThrow: "Wisdom DC 13")
        { }
    }

    public class CreateDestroyWater : Spell
    {
        public CreateDestroyWater() : base(
            "Create/Destroy Water", 0, School.Transmutation, "1 action", "30 feet", "Instantaneous",
            "You either create or destroy clean water. Creates 10 gallons of water in an open container.",
            SpellComponent.VerbalSomatic)
        { }
    }

    public class GuidingBolt : Spell
    {
        public GuidingBolt() : base(
            "Guiding Bolt", 0, School.Evocation, "1 action", "120 feet", "Instantaneous",
            "A flash of light burns creatures in your vision. Make a ranged spell attack. On hit, target takes 2d6 radiant damage and next attack has advantage.",
            SpellComponent.VerbalSomatic, damageType: DamageType.Radiant, damageAmount: "2d6",
            targetType: "creature", savingThrow: "")
        { }
    }

    // ==================== 1st-Level Spells ====================

    public class Aid : Spell
    {
        public Aid() : base(
            "Aid", 1, School.Enchantment, "1 action", "30 feet", "8 hours",
            "Your spell bolsters allies with vitality and strength. Choose up to three creatures within range. Target's hit point maximum and current HP increase by 5.",
            SpellComponent.VerbalSomaticMaterial, materialComponents: "a tiny strip of green cloth",
            targetType: "creature", targetCount: 3)
        { }
    }

    public class ArcaneLock : Spell
    {
        public ArcaneLock() : base(
            "Arcane Lock", 1, School.Abjuration, "1 action", "Touch", "Until dispelled",
            "You touch a closed door, window, gate, chest, or similar object. It becomes locked and can't be opened for 24 hours. Magical key can be set.",
            SpellComponent.VerbalSomaticMaterial, materialComponents: "gold dust worth 25gp")
        { }
    }

    public class BlindnessDeafness : Spell
    {
        public BlindnessDeafness() : base(
            "Blindness/Deafness", 1, School.Necromancy, "1 action", "60 feet", "1 minute",
            "You can blind or deafen a creature. Target must make a Constitution save. On failure, blinded or deafened for duration.",
            SpellComponent.Verbal, savingThrow: "Constitution DC 14")
        {
            _targetType = "creature";
            _concentration = true;
        }
    }

    public class Blink : Spell
    {
        public Blink() : base(
            "Blink", 1, School.Illusion, "1 action", "Self", "Concentration, up to 1 minute",
            "Roll a d20 at the end of each turn. On 11-20, you blink to another plane. Attacks against you have disadvantage while on other plane.",
            SpellComponent.VerbalSomatic)
        {
            _targetType = "Self";
        }
    }

    public class CalcifiedDefense : Spell
    {
        public CalcifiedDefense() : base(
            "Calcified Defense", 1, School.Abjuration, "1 bonus action", "Self", "Concentration, up to 6 hours",
            "A waxy coat covers your body. When you take damage, you can use your reaction to gain +2 AC until start of your next turn.",
            SpellComponent.VerbalSomaticMaterial, materialComponents: "a bit of slate")
        { }
    }

    public class CureWounds : Spell
    {
        public CureWounds() : base(
            "Cure Wounds", 1, School.Evocation, "1 action", "Touch", "Instantaneous",
            "A creature you touch regains hit points. The target regains 1d8 + Spellcasting Ability modifier hit points.",
            SpellComponent.VerbalSomatic)
        {
            _targetType = "creature";
            _targetCount = 1;
        }
    }

    public class DetectStealth : Spell
    {
        public DetectStealth() : base(
            "Detect Stealth", 1, School.Divination, "1 action", "Self", "Instantaneous",
            "You sense invisible creatures or objects within 30 feet of you for the duration.",
            SpellComponent.VerbalSomatic)
        { }
    }

    public class EnlargeReduce : Spell
    {
        public EnlargeReduce() : base(
            "Enlarge/Reduce", 1, School.Transmutation, "1 action", "30 feet", "Concentration, up to 1 minute",
            "Target creature/object doubles or halves in size. Creatures grow twice as tall and double weight. Attacks deal extra damage when enlarged.",
            SpellComponent.VerbalSomaticMaterial, materialComponents: "a pinch of powdered iron",
            targetType: "creature", savingThrow: "Constitution DC 14")
        { }
    }

    public class FeatherFall : Spell
    {
        public FeatherFall() : base(
            "Feather Fall", 1, School.Transmutation, "1 reaction", "60 feet", "Instantaneous",
            "Choose up to 5 creatures within range. Falling speed of each target reduces to 60 feet per round until spell ends.",
            SpellComponent.VerbalSomaticMaterial, materialComponents: "a small feather or piece of gossamer")
        { }
    }

    public class FogCloud : Spell
    {
        public FogCloud() : base(
            "Fog Cloud", 1, School.Conjuration, "1 action", "90 feet", "Concentration, up to 1 hour",
            "You create a 20-foot-radius cloud of fog. Creatures fully within the cloud are blinded. Wind can disperse the fog.",
            SpellComponent.Verbal)
        {
            _areaOfEffect = "20-foot radius sphere";
        }
    }

    public class Goodberry : Spell
    {
        public Goodberry() : base(
            "Goodberry", 1, School.Transmutation, "1 action", "Self", "Instantaneous",
            "Up to ten berries appear. Each berry can restore 1d4 + 1 hit points. Berries lose potency after 24 hours.",
            SpellComponent.VerbalSomatic)
        { }
    }

    public class HypnoticPattern : Spell
    {
        public HypnoticPattern() : base(
            "Hypnotic Pattern", 1, School.Illusion, "1 action", "120 feet", "Concentration, up to 1 minute",
            "A shimmering magical pattern forms in a 30-foot cube. First creature in range that sees the pattern must make a Wisdom save or be charmed.",
            SpellComponent.VerbalSomatic, savingThrow: "Wisdom DC 14")
        {
            _areaOfEffect = "30-foot cube";
            _targetCount = 5;
            _concentration = true;
        }
    }

    public class InfuseTool : Spell
    {
        public InfuseTool() : base(
            "Infuse Tool", 1, School.Transmutation, "1 hour", "Touch", "Until dispelled",
            "You touch one tool. For the duration, that tool grants advantage on ability checks made with it.",
            SpellComponent.VerbalSomaticMaterial)
        { }
    }

    public class LesserRestoration : Spell
    {
        public LesserRestoration() : base(
            "Lesser Restoration", 1, School.Abjuration, "1 action", "Touch", "Instantaneous",
            "You touch a creature to end one disease or one effect poisoning or charming the target.",
            SpellComponent.VerbalSomatic)
        { }
    }

    public class Longstrider : Spell
    {
        public Longstrider() : base(
            "Longstrider", 1, School.Transmutation, "1 action", "Touch", "1 hour",
            "You touch a creature. The target's walking speed increases to 40 feet for the duration.",
            SpellComponent.VerbalSomatic)
        { }
    }

    public class ProtectionFromEvilGood : Spell
    {
        public ProtectionFromEvilGood() : base(
            "Protection from Evil and Good", 1, School.Abjuration, "1 action", "Touch", "Concentration, up to 10 minutes",
            "Until the spell ends, one willing creature you touch is protected against certain types of creatures.",
            SpellComponent.VerbalSomatic)
        { }
    }

    public class Identify : Spell
    {
        public Identify() : base(
            "Identify", 1, School.Divination, "1 minute", "Touch", "Instantaneous",
            "You choose one object that you must touch. Until the spell ends, you learn the properties of the magic item.",
            SpellComponent.VerbalSomaticMaterial, ritual: true, materialComponents: "a pearl worth 100gp")
        { }
    }

    public class Thunderwave : Spell
    {
        public Thunderwave() : base(
            "Thunderwave", 1, School.Evocation, "1 action", "Self", "Instantaneous",
            "A thunderous boom pulses from you. Each creature within 15 feet must make a Constitution save. On failure, takes 2d8 thunder damage and is pushed away.",
            SpellComponent.VerbalSomatic, damageType: DamageType.Thunder, damageAmount: "2d8",
            areaOfEffect: "15-foot cube", savingThrow: "Constitution DC 14")
        { }
    }

    public class Bless : Spell
    {
        public Bless() : base(
            "Bless", 1, School.Enchantment, "1 action", "30 feet", "Concentration, up to 1 minute",
            "You bless up to three creatures. Whenever a target makes an attack roll or saving throw, it can roll a d4 and add to the result.",
            SpellComponent.Verbal, targetType: "creature", targetCount: 3)
        { }
    }

    public class HellishRebuke : Spell
    {
        public HellishRebuke() : base(
            "Hellish Rebuke", 1, School.Evocation, "1 reaction", "60 feet", "Instantaneous",
            "This spell creates a wrack of flame covering the body of one creature in your vision. The target takes 2d10 fire damage.",
            SpellComponent.VerbalSomatic, damageType: DamageType.Fire, damageAmount: "2d10",
            targetType: "creature")
        { }
    }

    public class Sleep : Spell
    {
        public Sleep() : base(
            "Sleep", 1, School.Enchantment, "1 action", "90 feet", "Concentration, up to 1 minute",
            "This spell sends a creature into magical slumber. Roll 5d8 hit points. Highest HD creatures fall asleep first.",
            SpellComponent.VerbalSomatic)
        { }
    }

    // ==================== 2nd-Level Spells ====================

    public class AlterSelf : Spell
    {
        public AlterSelf() : base(
            "Alter Self", 2, School.Transmutation, "1 action", "Self", "Concentration, up to 1 hour",
            "You assume a different form. Choose beast or humanoid. You gain benefits of chosen form.",
            SpellComponent.VerbalSomatic)
        { }
    }

    public class Blur : Spell
    {
        public Blur() : base(
            "Blur", 2, School.Illusion, "1 action", "Self", "Concentration, up to 1 minute",
            "Your body shimmers. Attacks against you have disadvantage until the spell ends.",
            SpellComponent.Verbal)
        { }
    }

    public class CloudOfDaggers : Spell
    {
        public CloudOfDaggers() : base(
            "Cloud of Daggers", 2, School.Evocation, "1 action", "60 feet", "Concentration, up to 1 minute",
            "You fill the air with floating daggers in a 5-foot cube. Each creature in area takes 4d4 slashing damage.",
            SpellComponent.VerbalSomatic, damageAmount: "4d4",
            areaOfEffect: "5-foot cube")
        { }
    }

    public class CrownOfStars : Spell
    {
        public CrownOfStars() : base(
            "Crown of Stars", 2, School.Evocation, "1 action", "Self", "Concentration, up to 1 hour",
            "Darting stars orbit your head. You can use bonus action to hurl a star at a creature within 30 feet.",
            SpellComponent.VerbalSomatic)
        { }
    }

    public class Darkvision : Spell
    {
        public Darkvision() : base(
            "Darkvision", 2, School.Illusion, "1 action", "Touch", "24 hours",
            "You touch a willing creature. Target can see in darkness up to 60 feet with blinding sight.",
            SpellComponent.VerbalSomatic)
        { }
    }

    public class DetectThoughts : Spell
    {
        public DetectThoughts() : base(
            "Detect Thoughts", 2, School.Divination, "1 action", "Self", "Concentration, up to 1 hour",
            "You can read the thoughts of creatures within range. A creature's mind is completely shielded.",
            SpellComponent.VerbalSomaticMaterial)
        { }
    }

    public class DolphinDance : Spell
    {
        public DolphinDance() : base(
            "Dolphin's Dance", 2, School.Transmutation, "1 action", "Self", "Instantaneous",
            "You perform a graceful leaping movement. You gain bonus to speed and can ignore difficult terrain.",
            SpellComponent.VerbalSomatic)
        { }
    }

    public class FireShield : Spell
    {
        public FireShield() : base(
            "Fire Shield", 2, School.Evocation, "1 action", "Self", "Concentration, up to 10 minutes",
            "You surround yourself with fire or cold. Attacks against you deal extra damage. Creatures hitting you take 2d8 damage.",
            SpellComponent.VerbalSomaticMaterial, materialComponents: "a bit of phoenix down")
        { }
    }

    public class GaseousForm : Spell
    {
        public GaseousForm() : base(
            "Gaseous Form", 2, School.Transmutation, "1 action", "Touch", "Concentration, up to 1 hour",
            "You transform a willing creature into mist. Flying speed of 40 feet, can fly through openings as small as 1 inch.",
            SpellComponent.VerbalSomaticMaterial)
        { }
    }

    public class GustOfWind : Spell
    {
        public GustOfWind() : base(
            "Gust of Wind", 2, School.Evocation, "1 action", "Self", "Concentration, up to 1 minute",
            "A line of strong wind 60 feet long and 10 feet wide blows from you. Creatures must make strength save or be pushed.",
            SpellComponent.VerbalSomatic)
        { }
    }

    public class HeatMetal : Spell
    {
        public HeatMetal() : base(
            "Heat Metal", 2, School.Transmutation, "1 action", "60 feet", "Concentration, up to 1 minute",
            "Choose a nonmagical metal object. Target wearing or holding it takes 2d8 fire damage on save.",
            SpellComponent.VerbalSomaticMaterial, materialComponents: "a piece of iron and a flame",
            damageType: DamageType.Fire)
        { }
    }

    public class HoldPerson : Spell
    {
        public HoldPerson() : base(
            "Hold Person", 2, School.Enchantment, "1 action", "60 feet", "Concentration, up to 1 minute",
            "Choose humanoid you can see. Target must make Wisdom save or be paralyzed for duration.",
            SpellComponent.VerbalSomatic, savingThrow: "Wisdom DC 15")
        { }
    }

    public class LightningArrow : Spell
    {
        public LightningArrow() : base(
            "Lightning Arrow", 2, School.Evocation, "1 action", "150 feet", "Instantaneous",
            "You can make a ranged attack with a bow or crossbow. On hit, target and creatures within 10 feet take lightning damage.",
            SpellComponent.VerbalSomatic, damageType: DamageType.Lightning)
        { }
    }

    public class MagicMouth : Spell
    {
        public MagicMouth() : base(
            "Magic Mouth", 2, School.Illusion, "1 minute", "30 feet", "Concentration, up to 8 hours",
            "You place a message within an object. When someone comes within range of the object, the message is repeated.",
            SpellComponent.VerbalSomaticMaterial)
        { }
    }

    public class MindFog : Spell
    {
        public MindFog() : base(
            "Mind Fog", 2, School.Illusion, "1 action", "150 feet", "Concentration, up to 1 minute",
            "You create a mist in a 20-foot radius. Creatures affected have disadvantage on Wisdom checks and INT checks.",
            SpellComponent.VerbalSomatic)
        { }
    }

    public class MirrorImage : Spell
    {
        public MirrorImage() : base(
            "Mirror Image", 2, School.Illusion, "1 action", "Self", "Concentration, up to 1 minute",
            "Three illusory doubles appear. Attacks target one of the doubles instead of you.",
            SpellComponent.VerbalSomatic)
        { }
    }

    public class MistyStep : Spell
    {
        public MistyStep() : base(
            "Misty Step", 2, School.Conjuration, "1 bonus action", "Self", "Instantaneous",
            "You teleport up to 30 feet to an unoccupied space you can see.",
            SpellComponent.Verbal)
        { }
    }

    public class Pyrotechnics : Spell
    {
        public Pyrotechnics() : base(
            "Pyrotechnics", 2, School.Illusion, "1 action", "60 feet", "Concentration, up to 1 minute",
            "Choose a lighting source within range. Target can explode in bright/dim light or create a cloud of smoke.",
            SpellComponent.VerbalSomatic)
        { }
    }

    public class ScorchingRay : Spell
    {
        public ScorchingRay() : base(
            "Scorching Ray", 2, School.Evocation, "1 action", "120 feet", "Instantaneous",
            "You create three rays of fire. Make ranged spell attacks. On hit, target takes 2d6 fire damage.",
            SpellComponent.VerbalSomatic, damageType: DamageType.Fire, damageAmount: "2d6")
        { }
    }

    public class SeeInvisibility : Spell
    {
        public SeeInvisibility() : base(
            "See Invisibility", 2, School.Divination, "1 action", "Self", "Concentration, up to 1 hour",
            "You see creatures and objects as if they were invisible. Illusions become visible.",
            SpellComponent.VerbalSomaticMaterial)
        { }
    }

    public class Shillelagh : Spell
    {
        public Shillelagh() : base(
            "Shillelagh", 2, School.Transmutation, "1 bonus action", "Self", "Concentration, up to 1 minute",
            "You touch a club or quarterstaff. For duration, it deals 1d4 damage and uses your spellcasting ability for attacks.",
            SpellComponent.VerbalSomaticMaterial)
        { }
    }

    public class SpikeGrowth : Spell
    {
        public SpikeGrowth() : base(
            "Spike Growth", 2, School.Transmutation, "1 action", "Self", "Concentration, up to 1 hour",
            "The ground in a 20-foot radius around you sprouts hard spikes. Creatures moving through the area take 2d4 piercing damage.",
            SpellComponent.VerbalSomaticMaterial, materialComponents: "7 spiked seeds or burrs")
        { }
    }

    public class Web : Spell
    {
        public Web() : base(
            "Web", 2, School.Illusion, "1 action", "60 feet", "Concentration, up to 8 hours",
            "You spin a mass of thorny web. Creatures must make Dexterity save or be restrained.",
            SpellComponent.VerbalSomaticMaterial, materialComponents: "a bit of spiderweb")
        { }
    }

    // ==================== 3rd-Level Spells ====================

    public class BestowCurse : Spell
    {
        public BestowCurse() : base(
            "Bestow Curse", 3, School.Necromancy, "1 action", "Touch", "Concentration, up to 1 minute",
            "You touch a creature. That creature is cursed for the duration. You choose the effect of the curse.",
            SpellComponent.VerbalSomatic)
        { }
    }

    public class Fireball : Spell
    {
        public Fireball() : base(
            "Fireball", 3, School.Evocation, "1 action", "150 feet", "Instantaneous",
            "A bright streak flashes from your pointing finger to a point and blossoms into a fireball explosion. Each creature in area takes 8d6 fire damage.",
            SpellComponent.VerbalSomatic, damageType: DamageType.Fire, damageAmount: "8d6",
            areaOfEffect: "20-foot radius sphere", savingThrow: "Dexterity DC 15")
        { }
    }

    public class Fly : Spell
    {
        public Fly() : base(
            "Fly", 3, School.Transmutation, "1 action", "Touch", "Concentration, up to 1 minute",
            "You touch a willing creature. Target has flying speed of 60 feet for duration.",
            SpellComponent.VerbalSomatic)
        { }
    }

    public class GlyphOfWarding : Spell
    {
        public GlyphOfWarding() : base(
            "Glyph of Warding", 3, School.Abjuration, "1 hour", "Touch", "Until triggered",
            "You create a glyph that expends magical energy when triggered. Choose explosion, explosion, or language trap.",
            SpellComponent.VerbalSomaticMaterial, materialComponents: "ink worth 50gp")
        { }
    }

    public class Haste : Spell
    {
        public Haste() : base(
            "Haste", 3, School.Transmutation, "1 action", "30 feet", "Concentration, up to 1 minute",
            "Choose willing creature. Target speed doubles, gains +2 AC, bonus action on turn, advantage on Dex saves.",
            SpellComponent.VerbalSomaticMaterial)
        { }
    }

    public class HypnoticPatternLv3 : Spell
    {
        public HypnoticPatternLv3() : base(
            "Hypnotic Pattern (3rd)", 3, School.Illusion, "1 action", "120 feet", "Concentration, up to 1 minute",
            "A shimmering pattern weaves through the air. Creatures in a 30-foot cube must make Wisdom save or be charmed.",
            SpellComponent.VerbalSomatic)
        { }
    }

    public class LightningBolt : Spell
    {
        public LightningBolt() : base(
            "Lightning Bolt", 3, School.Evocation, "1 action", "150 feet", "Instantaneous",
            "A stroke of lightning flashes from your fingertips. Each creature along line makes Constitution save. On failure, takes 8d6 lightning damage.",
            SpellComponent.VerbalSomatic, damageType: DamageType.Lightning, damageAmount: "8d6",
            areaOfEffect: "100-foot line", savingThrow: "Constitution DC 15")
        { }
    }

    public class Nondetection : Spell
    {
        public Nondetection() : base(
            "Nondetection", 3, School.Abjuration, "1 action", "Touch", "Concentration, up to 8 hours",
            "You touch a creature or object. Target is hidden from divination magic and scrying.",
            SpellComponent.VerbalSomaticMaterial)
        { }
    }

    public class Polymorph : Spell
    {
        public Polymorph() : base(
            "Polymorph", 3, School.Transmutation, "1 action", "60 feet", "Concentration, up to 1 hour",
            "You transform creature into beast. Target's HP remain same but game stats replace by beast. If reduced to 0 HP, spell ends.",
            SpellComponent.VerbalSomaticMaterial)
        { }
    }

    public class ReverseGravity : Spell
    {
        public ReverseGravity() : base(
            "Reverse Gravity", 3, School.Transmutation, "1 action", "150 feet", "Concentration, up to 1 minute",
            "Gravity reverses in a 50-foot radius sphere. All creatures fall upward until they hit ceiling.",
            SpellComponent.VerbalSomatic)
        { }
    }

    public class Slow : Spell
    {
        public Slow() : base(
            "Slow", 3, School.Transmutation, "1 action", "120 feet", "Concentration, up to 1 minute",
            "Choose creatures in a 40-foot cube. Targets' speed becomes half, AC reduced by 2, disadvantage on Dex saves.",
            SpellComponent.VerbalSomaticMaterial, savingThrow: "Wisdom DC 15")
        { }
    }

    public class StinkingCloud : Spell
    {
        public StinkingCloud() : base(
            "Stinking Cloud", 3, School.Illusion, "1 action", "90 feet", "Concentration, up to 1 minute",
            "You create a 20-foot radius cloud of noxious gas. Creatures in area must succeed on Constitution save or be nauseated.",
            SpellComponent.VerbalSomatic)
        { }
    }

    public class Tongues : Spell
    {
        public Tongues() : base(
            "Tongues", 3, School.Divination, "1 action", "Touch", "Concentration, up to 1 hour",
            "You touch creature. Target can understand and speak all languages for duration.",
            SpellComponent.VerbalSomatic)
        { }
    }

    public class VitriolicSphere : Spell
    {
        public VitriolicSphere() : base(
            "Vitriolic Sphere", 3, School.Evocation, "1 action", "150 feet", "Instantaneous",
            "You create a 20-foot radius sphere of acid. Creatures in center take 10d4 acid damage, creatures at edge take half.",
            SpellComponent.VerbalSomatic, damageType: DamageType.Acid, damageAmount: "10d4",
            areaOfEffect: "20-foot radius sphere", savingThrow: "Dexterity DC 15")
        { }
    }

    // ==================== 4th-Level Spells ====================

    public class ArcaneEye : Spell
    {
        public ArcaneEye() : base(
            "Arcane Eye", 4, School.Divination, "1 action", "Self", "Concentration, up to 1 hour",
            "You create an invisible eye that floats 10 feet above ground. See through eye for duration.",
            SpellComponent.VerbalSomatic)
        { }
    }

    public class Blight : Spell
    {
        public Blight() : base(
            "Blight", 4, School.Necromancy, "1 action", "30 feet", "Instantaneous",
            "Negative energy gathers in your hand. Plant life takes 8d8 necrotic damage. Nonmagical plants destroyed.",
            SpellComponent.VerbalSomatic, damageType: DamageType.Necrotic, damageAmount: "8d8",
            savingThrow: "Constitution DC 16")
        { }
    }

    public class Confusion : Spell
    {
        public Confusion() : base(
            "Confusion", 4, School.Enchantment, "1 action", "90 feet", "Concentration, up to 1 minute",
            "Creatures in 20-foot radius sphere must make Wisdom save or be charmed for duration. Charmed creature rolls d10 each turn.",
            SpellComponent.VerbalSomaticMaterial)
        { }
    }

    public class DimensionDoor : Spell
    {
        public DimensionDoor() : base(
            "Dimension Door", 4, School.Conjuration, "1 action", "Self", "Instantaneous",
            "You teleport up to 500 feet to a spot you can see. You and companions arrive simultaneously.",
            SpellComponent.VerbalSomatic)
        { }
    }

    public class GreaterRestoration : Spell
    {
        public GreaterRestoration() : base(
            "Greater Restoration", 4, School.Necromancy, "1 action", "Touch", "Instantaneous",
            "You touch a creature to end one effect coding ability score reduction, or ability score exam.",
            SpellComponent.VerbalSomaticMaterial, materialComponents: "the prayer bust of saint")
        { }
    }

    public class IceStorm : Spell
    {
        public IceStorm() : base(
            "Ice Storm", 4, School.Evocation, "1 action", "150 feet", "Instantaneous",
            "A column of ice and hail slams down. Creatures in cylinder take 2d6 bludgeoning damage and 4d6 cold damage.",
            SpellComponent.VerbalSomaticMaterial)
        { }
    }

    public class PhantasmalKiller : Spell
    {
        public PhantasmalKiller() : base(
            "Phantasmal Killer", 4, School.Illusion, "1 action", "30 feet", "Concentration, up to 1 minute",
            "You weave illusions. Target must make Wisdom save or take psychic damage each turn.",
            SpellComponent.VerbalSomatic, damageType: DamageType.Psychic)
        { }
    }

    public class Stoneskin : Spell
    {
        public Stoneskin() : base(
            "Stoneskin", 4, School.Abjuration, "1 action", "Touch", "Concentration, up to 1 hour",
            "You touch willing creature. It has advantage on Dexterity saves and takes half damage from nonmagical bludgeoning.",
            SpellComponent.VerbalSomaticMaterial)
        { }
    }

    public class WallOfFire : Spell
    {
        public WallOfFire() : base(
            "Wall of Fire", 4, School.Evocation, "1 action", "60 feet", "Concentration, up to 1 minute",
            "You create a wall of fire. First time creature enters takes 5d8 fire damage. Side you choose deals double.",
            SpellComponent.VerbalSomaticMaterial)
        { }
    }

    public class MordenkainenSword : Spell
    {
        public MordenkainenSword() : base(
            "Mordenkainen's Sword", 4, School.Conjuration, "1 action", "90 feet", "Concentration, up to 1 minute",
            "You create a mighty sword of force that hovers nearby. Bonus action: throw sword at target.",
            SpellComponent.VerbalSomatic)
        { }
    }

    // ==================== 5th-Level Spells ====================

    public class ArcaneHand : Spell
    {
        public ArcaneHand() : base(
            "Arcane Hand", 5, School.Evocation, "1 action", "90 feet", "Concentration, up to 1 minute",
            "You create a large hand that can manipulate objects or attack. Melee spell attack deals 4d8 force damage.",
            SpellComponent.VerbalSomatic)
        { }
    }

    public class ConeOfCold : Spell
    {
        public ConeOfCold() : base(
            "Cone of Cold", 5, School.Evocation, "1 action", "Self", "Instantaneous",
            "A blast of cold air erupts from you. Creatures in 60-foot cone take 8d8 cold damage.",
            SpellComponent.VerbalSomaticMaterial, damageType: DamageType.Cold)
        { }
    }

    public class Creation : Spell
    {
        public Creation() : base(
            "Creation", 5, School.Illusion, "1 minute", "Self", "Instantaneous",
            "You create a nonmagical object of up to 10 cubic feet. Object must be tiny, simple, and inanimate.",
            SpellComponent.VerbalSomaticMaterial)
        { }
    }

    public class DominatePerson : Spell
    {
        public DominatePerson() : base(
            "Dominate Person", 5, School.Enchantment, "1 action", "60 feet", "Concentration, up to 1 minute",
            "You attempt to charm humanoid. Target makes Wisdom save or is charmed. Charmed creature obeys commands.",
            SpellComponent.VerbalSomatic)
        { }
    }

    public class Dream : Spell
    {
        public Dream() : base(
            "Dream", 5, School.Illusion, "1 minute", "Special", "8 hours",
            "You send a message to sleeping creature. Target dreams of you sending message.",
            SpellComponent.VerbalSomaticMaterial)
        { }
    }

    public class Geas : Spell
    {
        public Geas() : base(
            "Geas", 5, School.Enchantment, "1 hour", "60 feet", "30 days",
            "You place commands on creature. If targets fails saves for duration, target takes 5d10 psychic damage.",
            SpellComponent.VerbalSomaticMaterial)
        { }
    }

    public class MassCureWounds : Spell
    {
        public MassCureWounds() : base(
            "Mass Cure Wounds", 5, School.Evocation, "1 action", "60 feet", "Instantaneous",
            "A creature of your choice regains hit points. Up to twelve creatures in range each regain 5d8 + spellcasting ability modifier.",
            SpellComponent.VerbalSomatic)
        { }
    }

    public class Mislead : Spell
    {
        public Mislead() : base(
            "Mislead", 5, School.Illusion, "1 action", "Self", "Concentration, up to 1 hour",
            "You become invisible and create an illusory double. Double can cast spells and move.",
            SpellComponent.VerbalSomaticMaterial)
        { }
    }

    public class PlanarBinding : Spell
    {
        public PlanarBinding() : base(
            "Planar Binding", 5, School.Abjuration, "1 hour", "60 feet", "Concentration, up to 1 hour",
            "You attempt to bind a creature from another plane. Target must make Wisdom save.",
            SpellComponent.VerbalSomaticMaterial)
        { }
    }

    public class Scrying : Spell
    {
        public Scrying() : base(
            "Scrying", 5, School.Divination, "10 minutes", "Self", "Concentration, up to 10 minutes",
            "You can see and hear a particular creature known to you.",
            SpellComponent.VerbalSomaticMaterial)
        { }
    }

    public class TensersTransformation : Spell
    {
        public TensersTransformation() : base(
            "Tenser's Transformation", 5, School.Transmutation, "1 action", "Self", "Concentration, up to 1 hour",
            "You gain temporary HP, enhanced melee attacks, and improved AC.",
            SpellComponent.VerbalSomaticMaterial)
        { }
    }

    // ==================== 6th-Level Spells ====================

    public class ArcaneGate : Spell
    {
        public ArcaneGate() : base(
            "Arcane Gate", 6, School.Conjuration, "1 action", "120 feet", "Concentration, up to 1 minute",
            "You create a gate connecting two locations. Creatures can pass through.",
            SpellComponent.VerbalSomaticMaterial)
        { }
    }

    public class Disintegrate : Spell
    {
        public Disintegrate() : base(
            "Disintegrate", 6, School.Necromancy, "1 action", "60 feet", "Instantaneous",
            "A green ray shoots from your pointing finger. Target takes 10d6 + 40 force damage. If reduced to 0 HP disintegrates.",
            SpellComponent.VerbalSomaticMaterial)
        { }
    }

    public class FalseLife : Spell
    {
        public FalseLife() : base(
            "False Life", 6, School.Necromancy, "1 action", "Self", "Instantaneous",
            "You gain temporary hit points equal to 3d10 + spellcasting ability modifier.",
            SpellComponent.VerbalSomaticMaterial)
        { }
    }

    public class FindThePath : Spell
    {
        public FindThePath() : base(
            "Find The Path", 6, School.Divination, "1 action", "Self", "Concentration, up to 1 hour",
            "Your mind links with a known location. You learn direction and shortest route.",
            SpellComponent.VerbalSomaticMaterial)
        { }
    }

    public class Gate : Spell
    {
        public Gate() : base(
            "Gate", 6, School.Conjuration, "1 action", "120 feet", "Concentration, up to 1 minute",
            "You create a portal to another location. Can summon beings through the gate.",
            SpellComponent.VerbalSomaticMaterial)
        { }
    }

    public class HangarHorde : Spell
    {
        public HangarHorde() : base(
            "Hangar Horde", 6, School.Necromancy, "1 action", "Self", "Instantaneous",
            "You create a horde of undead creatures from dead bodies.",
            SpellComponent.VerbalSomaticMaterial)
        { }
    }

    public class IllusoryDragon : Spell
    {
        public IllusoryDragon() : base(
            "Illusory Dragon", 6, School.Illusion, "1 action", "Self", "Concentration, up to 1 minute",
            "You create a large dragon illusion that can move and deal damage.",
            SpellComponent.VerbalSomatic)
        { }
    }

    public class OtilukeFreezingSphere : Spell
    {
        public OtilukeFreezingSphere() : base(
            "Otiluke's Freezing Sphere", 6, School.Evocation, "1 action", "120 feet", "Instantaneous",
            "A sphere of cold creates in a point. On hit, takes 10d8 cold damage.",
            SpellComponent.VerbalSomaticMaterial)
        { }
    }

    public class ProgrammedIllusion : Spell
    {
        public ProgrammedIllusion() : base(
            "Programmed Illusion", 6, School.Illusion, "1 action", "150 feet", "Concentration, up to 10 days",
            "You create illusion that lasts for duration. Triggered by specific conditions.",
            SpellComponent.VerbalSomaticMaterial)
        { }
    }

    public class TrueSeeing : Spell
    {
        public TrueSeeing() : base(
            "True Seeing", 6, School.Divination, "1 action", "Touch", "Concentration, up to 1 hour",
            "You touch creature. It can see normal and magical darkness, see invisible creatures, see original form.",
            SpellComponent.VerbalSomaticMaterial)
        { }
    }

    // ==================== 7th-Level Spells ====================

    public class ArcherShot : Spell
    {
        public ArcherShot() : base(
            "Archer's Shot", 7, School.Evocation, "1 reaction", "90 feet", "Instantaneous",
            "You make a ranged attack that deals extra force damage.",
            SpellComponent.VerbalSomatic)
        { }
    }

    public class FarStep : Spell
    {
        public FarStep() : base(
            "Far Step", 7, School.Conjuration, "1 bonus action", "Self", "Instantaneous",
            "You teleport to an unoccupied space you can see. Next attack deals extra psychic damage.",
            SpellComponent.VerbalSomatic)
        { }
    }

    public class Farseer : Spell
    {
        public Farseer() : base(
            "Farseer", 7, School.Divination, "1 hour", "Self", "Concentration, up to 1 hour",
            "You can see and hear anywhere on the same plane.",
            SpellComponent.VerbalSomaticMaterial)
        { }
    }

    public class Forcecage : Spell
    {
        public Forcecage() : base(
            "Forcecage", 7, School.Abjuration, "1 action", "60 feet", "Concentration, up to 1 hour",
            "You create a prison of force. Creatures inside must make Charisma save or be trapped.",
            SpellComponent.VerbalSomaticMaterial)
        { }
    }

    public class MirageArcane : Spell
    {
        public MirageArcane() : base(
            "Mirage Arcane", 7, School.Illusion, "10 minutes", "Self", "24 hours",
            "You transform terrain in area. Area becomes difficult terrain.",
            SpellComponent.VerbalSomaticMaterial)
        { }
    }

    public class PowerWordStun : Spell
    {
        public PowerWordStun() : base(
            "Power Word Stun", 7, School.Enchantment, "1 action", "60 feet", "Concentration, up to 1 minute",
            "You speak a word of power. Creature with fewer than 100 HP is stunned.",
            SpellComponent.Verbal)
        { }
    }

    public class PrismaticSpray : Spell
    {
        public PrismaticSpray() : base(
            "Prismatic Spray", 7, School.Illusion, "1 action", "Self", "Instantaneous",
            "A prism of vivid light flashes. Roll d6 for effects: acid, blinding, death, confinement, petrification, or wind.",
            SpellComponent.VerbalSomatic)
        { }
    }

    public class ProjectImage : Spell
    {
        public ProjectImage() : base(
            "Project Image", 7, School.Illusion, "1 action", "500 feet", "Concentration, up to 1 minute",
            "You create an image that can move and cast spells.",
            SpellComponent.VerbalSomaticMaterial)
        { }
    }

    public class Regenerate : Spell
    {
        public Regenerate() : base(
            "Regenerate", 7, School.Transmutation, "1 minute", "Touch", "Instantaneous",
            "You touch creature. It regrows missing limbs and recovers hit points.",
            SpellComponent.VerbalSomaticMaterial)
        { }
    }

    // ==================== 8th-Level Spells ====================

    public class AnimalMessenger : Spell
    {
        public AnimalMessenger() : base(
            "Animal Messenger", 8, School.Enchantment, "1 action", "60 feet", "24 hours",
            "You find a Tiny beast and send it to deliver message.",
            SpellComponent.VerbalSomaticMaterial)
        { }
    }

    public class AntipathySympathy : Spell
    {
        public AntipathySympathy() : base(
            "Antipathy/Sympathy", 8, School.Enchantment, "1 hour", "60 feet", "24 hours",
            "Target creatures either loathe or are drawn to specified type of creature.",
            SpellComponent.VerbalSomaticMaterial)
        { }
    }

    public class Clairvoyance : Spell
    {
        public Clairvoyance() : base(
            "Clairvoyance", 8, School.Divination, "10 minutes", "30 feet", "Concentration, up to 1 hour",
            "You create an invisible sensor. See or hear through sensor.",
            SpellComponent.VerbalSomaticMaterial)
        { }
    }

    public class DominateMonster : Spell
    {
        public DominateMonster() : base(
            "Dominate Monster", 8, School.Enchantment, "1 action", "60 feet", "Concentration, up to 1 minute",
            "You attempt to charm creature. Target makes Wisdom save or is charmed and controlled.",
            SpellComponent.VerbalSomaticMaterial)
        { }
    }

    public class Enthrall : Spell
    {
        public Enthrall() : base(
            "Enthrall", 8, School.Enchantment, "1 action", "60 feet", "Concentration, up to 1 minute",
            "You magically enhance your voice. Creatures within range are charmed.",
            SpellComponent.VerbalSomatic)
        { }
    }

    public class Glibness : Spell
    {
        public Glibness() : base(
            "Glibness", 8, School.Enchantment, "1 action", "Self", "1 hour",
            "Your words become irresistible. Charisma checks gain +20 bonus.",
            SpellComponent.VerbalSomatic)
        { }
    }

    public class HeroesFeast : Spell
    {
        public HeroesFeast() : base(
            "Heroes' Feast", 8, School.Transmutation, "30 minutes", "Self", "Instantaneous",
            "You prepare a magnificent feast for up to twelve creatures. Participants gain advantages.",
            SpellComponent.VerbalSomaticMaterial)
        { }
    }

    public class MassSuggestion : Spell
    {
        public MassSuggestion() : base(
            "Mass Suggestion", 8, School.Enchantment, "1 action", "60 feet", "Concentration, up to 24 hours",
            "You suggest course of action to creatures. Targets make Wisdom save or follow suggestion.",
            SpellComponent.VerbalSomatic)
        { }
    }

    public class OtilukeGrantingSphere : Spell
    {
        public OtilukeGrantingSphere() : base(
            "Otiluke's Granting Sphere", 8, School.Evocation, "1 action", "120 feet", "Instantaneous",
            "You create a sphere of force that deals damage to creatures inside.",
            SpellComponent.VerbalSomaticMaterial)
        { }
    }

    public class Sunbeam : Spell
    {
        public Sunbeam() : base(
            "Sunbeam", 8, School.Evocation, "1 action", "Self", "Concentration, up to 1 minute",
            "A beam of brilliant light flashes from your hand. Each creature in line makes Constitution save.",
            SpellComponent.VerbalSomatic)
        { }
    }

    // ==================== 9th-Level Spells ====================

    public class AstralProjection : Spell
    {
        public AstralProjection() : base(
            "Astral Projection", 9, School.Necromancy, "10 minutes", "Special", "Until dispelled",
            "You and up to eight creatures project astrally. Can enter Ethereal Plane.",
            SpellComponent.VerbalSomaticMaterial)
        { }
    }

    public class BestowCurseLv9 : Spell
    {
        public BestowCurseLv9() : base(
            "Bestow Curse (9th)", 9, School.Necromancy, "1 action", "Touch", "Concentration, up to 1 minute",
            "Enhanced version of Bestow Curse. Can choose from multiple curse effects.",
            SpellComponent.VerbalSomaticMaterial)
        { }
    }

    public class Foresight : Spell
    {
        public Foresight() : base(
            "Foresight", 9, School.Divination, "1 hour", "Touch", "8 hours",
            "You or creature touches gains prophetic visions. Advantage on all saves. Cannot be surprised.",
            SpellComponent.VerbalSomaticMaterial)
        { }
    }

    public class GateLv9 : Spell
    {
        public GateLv9() : base(
            "Gate (9th)", 9, School.Conjuration, "1 action", "60 feet", "Concentration, up to 1 minute",
            "You create a portal connecting two locations. Can summon Inner Planes beings.",
            SpellComponent.VerbalSomaticMaterial)
        { }
    }

    public class IncendiaryCloud : Spell
    {
        public IncendiaryCloud() : base(
            "Incendiary Cloud", 9, School.Evocation, "1 action", "150 feet", "Concentration, up to 1 minute",
            "You create a cloud of swirling embers. Creatures take 6d8 fire damage per turn.",
            SpellComponent.VerbalSomatic)
        { }
    }

    public class MassSuggestionLv9 : Spell
    {
        public MassSuggestionLv9() : base(
            "Mass Suggestion (9th)", 9, School.Enchantment, "1 action", "120 feet", "Concentration, up to 24 hours",
            "You suggest deeply rooted behavior. Targets must make Wisdom save.",
            SpellComponent.VerbalSomatic)
        { }
    }

    public class MeteorSwarmSpell : Spell
    {
        public MeteorSwarmSpell() : base(
            "Meteor Swarm", 9, School.Evocation, "1 action", "1 mile", "Instantaneous",
            "Meteors crash down at four points. Each creature in 40-foot radius spheres take 2d6 fire and bludgeoning damage.",
            SpellComponent.VerbalSomaticMaterial)
        { }
    }

    public class PowerWordHeal : Spell
    {
        public PowerWordHeal() : base(
            "Power Word Heal", 9, School.Evocation, "1 action", "60 feet", "Instantaneous",
            "You speak a word of power. Target regains all hit points.",
            SpellComponent.VerbalSomatic)
        { }
    }

    public class PowerWordKill : Spell
    {
        public PowerWordKill() : base(
            "Power Word Kill", 9, School.Enchantment, "1 action", "60 feet", "Instantaneous",
            "You speak a word of power. Creature with fewer than 100 HP dies.",
            SpellComponent.Verbal)
        { }
    }

    public class TimeStop : Spell
    {
        public TimeStop() : base(
            "Time Stop", 9, School.Transmutation, "1 action", "Self", "Instantaneous",
            "You stop time for everyone else. You take 1d4 + 1 turns.",
            SpellComponent.VerbalSomatic)
        { }
    }

    public class Whirlwind : Spell
    {
        public Whirlwind() : base(
            "Whirlwind", 9, School.Conjuration, "1 action", "150 feet", "Concentration, up to 1 minute",
            "You create a whirlwind that can lift and throw creatures.",
            SpellComponent.VerbalSomaticMaterial)
        { }
    }

    // ==================== Spell Database for Class Access ====================
    
    /// <summary>
    /// Static database of all spells organized by spellcasting ability.
    /// Classes reference this to access their available spell lists.
    /// </summary>
    public static class SpellDatabase
    {
        // Dictionary of all spells by name for quick lookup
        private static readonly Dictionary<string, Spell> _allSpells = new()
        {
            // Cantrips
            ["Acid Splash"] = new AcidSplash(),
            ["Burning Hands"] = new BurningHands(),
            ["Detect Magic"] = new DetectMagic(),
            ["Faerie Fire"] = new FaerieFire(),
            ["Fake Life"] = new FakeLife(),
            ["Guidance"] = new Guidance(),
            ["Healing Word"] = new HealingWord(),
            ["Mage Armor"] = new MageArmor(),
            ["Magic Missile"] = new MagicMissile(),
            ["Mending"] = new Mending(),
            ["Protection of Evard"] = new ProtectionOfEvard(),
            ["Shield"] = new Shield(),
            ["Shatter"] = new Shatter(),
            ["Shocking Grasp"] = new ShockingGrasp(),
            ["Truth Seeing"] = new TruthSeeing(),
            ["Sacred Flame"] = new SacredFlame(),
            ["Thunderous Strike"] = new ThunderousStrike(),
            ["Wizard Spell"] = new WizardSpell(),
            ["Command"] = new Command(),
            ["Create/Destroy Water"] = new CreateDestroyWater(),
            ["Guiding Bolt"] = new GuidingBolt(),
            
            // 1st Level
            ["Aid"] = new Aid(),
            ["Arcane Lock"] = new ArcaneLock(),
            ["Blindness/Deafness"] = new BlindnessDeafness(),
            ["Blink"] = new Blink(),
            ["Calcified Defense"] = new CalcifiedDefense(),
            ["Cure Wounds"] = new CureWounds(),
            ["Detect Stealth"] = new DetectStealth(),
            ["Enlarge/Reduce"] = new EnlargeReduce(),
            ["Feather Fall"] = new FeatherFall(),
            ["Fog Cloud"] = new FogCloud(),
            ["Goodberry"] = new Goodberry(),
            ["Hypnotic Pattern"] = new HypnoticPattern(),
            ["Infuse Tool"] = new InfuseTool(),
            ["Lesser Restoration"] = new LesserRestoration(),
            ["Longstrider"] = new Longstrider(),
            ["Protection from Evil and Good"] = new ProtectionFromEvilGood(),
            ["Identify"] = new Identify(),
            ["Thunderwave"] = new Thunderwave(),
            ["Bless"] = new Bless(),
            ["Hellish Rebuke"] = new HellishRebuke(),
            ["Sleep"] = new Sleep(),
            
            // 2nd Level
            ["Alter Self"] = new AlterSelf(),
            ["Blur"] = new Blur(),
            ["Cloud of Daggers"] = new CloudOfDaggers(),
            ["Crown of Stars"] = new CrownOfStars(),
            ["Darkvision"] = new Darkvision(),
            ["Detect Thoughts"] = new DetectThoughts(),
            ["Dolphin's Dance"] = new DolphinDance(),
            ["Fire Shield"] = new FireShield(),
            ["Gaseous Form"] = new GaseousForm(),
            ["Gust of Wind"] = new GustOfWind(),
            ["Heat Metal"] = new HeatMetal(),
            ["Hold Person"] = new HoldPerson(),
            ["Lightning Arrow"] = new LightningArrow(),
            ["Magic Mouth"] = new MagicMouth(),
            ["Mind Fog"] = new MindFog(),
            ["Mirror Image"] = new MirrorImage(),
            ["Misty Step"] = new MistyStep(),
            ["Pyrotechnics"] = new Pyrotechnics(),
            ["Scorching Ray"] = new ScorchingRay(),
            ["See Invisibility"] = new SeeInvisibility(),
            ["Shillelagh"] = new Shillelagh(),
            ["Spike Growth"] = new SpikeGrowth(),
            ["Web"] = new Web(),
            
            // 3rd Level
            ["Bestow Curse"] = new BestowCurse(),
            ["Fireball"] = new Fireball(),
            ["Fly"] = new Fly(),
            ["Glyph of Warding"] = new GlyphOfWarding(),
            ["Haste"] = new Haste(),
            ["Lightning Bolt"] = new LightningBolt(),
            ["Nondetection"] = new Nondetection(),
            ["Polymorph"] = new Polymorph(),
            ["Reverse Gravity"] = new ReverseGravity(),
            ["Slow"] = new Slow(),
            ["Stinking Cloud"] = new StinkingCloud(),
            ["Tongues"] = new Tongues(),
            ["Vitriolic Sphere"] = new VitriolicSphere(),
            
            // 4th Level
            ["Arcane Eye"] = new ArcaneEye(),
            ["Blight"] = new Blight(),
            ["Confusion"] = new Confusion(),
            ["Dimension Door"] = new DimensionDoor(),
            ["Greater Restoration"] = new GreaterRestoration(),
            ["Ice Storm"] = new IceStorm(),
            ["Phantasmal Killer"] = new PhantasmalKiller(),
            ["Stoneskin"] = new Stoneskin(),
            ["Wall of Fire"] = new WallOfFire(),
            ["Mordenkainen's Sword"] = new MordenkainenSword(),
            
            // 5th Level
            ["Arcane Hand"] = new ArcaneHand(),
            ["Cone of Cold"] = new ConeOfCold(),
            ["Creation"] = new Creation(),
            ["Dominate Person"] = new DominatePerson(),
            ["Dream"] = new Dream(),
            ["Geas"] = new Geas(),
            ["Mass Cure Wounds"] = new MassCureWounds(),
            ["Mislead"] = new Mislead(),
            ["Planar Binding"] = new PlanarBinding(),
            ["Scrying"] = new Scrying(),
            ["Tenser's Transformation"] = new TensersTransformation(),
            
            // 6th Level
            ["Arcane Gate"] = new ArcaneGate(),
            ["Disintegrate"] = new Disintegrate(),
            ["False Life"] = new FalseLife(),
            ["Find the Path"] = new FindThePath(),
            ["Gate"] = new Gate(),
            ["Hangar Horde"] = new HangarHorde(),
            ["Illusory Dragon"] = new IllusoryDragon(),
            ["Otiluke's Freezing Sphere"] = new OtilukeFreezingSphere(),
            ["Programmed Illusion"] = new ProgrammedIllusion(),
            ["True Seeing"] = new TrueSeeing(),
            
            // 7th Level
            ["Archer's Shot"] = new ArcherShot(),
            ["Far Step"] = new FarStep(),
            ["Farseer"] = new Farseer(),
            ["Forcecage"] = new Forcecage(),
            ["Mirage Arcane"] = new MirageArcane(),
            ["Power Word Stun"] = new PowerWordStun(),
            ["Prismatic Spray"] = new PrismaticSpray(),
            ["Project Image"] = new ProjectImage(),
            ["Regenerate"] = new Regenerate(),
            
            // 8th Level
            ["Animal Messenger"] = new AnimalMessenger(),
            ["Antipathy/Sympathy"] = new AntipathySympathy(),
            ["Clairvoyance"] = new Clairvoyance(),
            ["Dominate Monster"] = new DominateMonster(),
            ["Enthrall"] = new Enthrall(),
            ["Glibness"] = new Glibness(),
            ["Heroes' Feast"] = new HeroesFeast(),
            ["Mass Suggestion"] = new MassSuggestion(),
            ["Otiluke's Granting Sphere"] = new OtilukeGrantingSphere(),
            ["Sunbeam"] = new Sunbeam(),
            
            // 9th Level
            ["Astral Projection"] = new AstralProjection(),
            ["Bestow Curse (9th)"] = new BestowCurseLv9(),
            ["Foresight"] = new Foresight(),
            ["Gate (9th)"] = new GateLv9(),
            ["Incendiary Cloud"] = new IncendiaryCloud(),
            ["Mass Suggestion (9th)"] = new MassSuggestionLv9(),
            ["Meteor Swarm"] = new MeteorSwarmSpell(),
            ["Power Word Heal"] = new PowerWordHeal(),
            ["Power Word Kill"] = new PowerWordKill(),
            ["Time Stop"] = new TimeStop(),
            ["Whirlwind"] = new Whirlwind()
        };

        /// <summary>
        /// Spells available to Intelligence-based casters (Artificer).
        /// </summary>
        private static readonly List<string> _intelligenceSpells = new()
        {
            // Cantrips
            "Acid Splash", "Burning Hands", "Detect Magic", "Faerie Fire", "Fake Life",
            "Guidance", "Healing Word", "Mage Armor", "Magic Missile", "Mending",
            "Protection of Evard", "Shield", "Shatter", "Shocking Grasp", "Truth Seeing",
            
            // 1st Level
            "Aid", "Arcane Lock", "Blindness/Deafness", "Blink", "Calcified Defense",
            "Cure Wounds", "Detect Magic", "Detect Stealth", "Enlarge/Reduce", "Feather Fall",
            "Fog Cloud", "Goodberry", "Hypnotic Pattern", "Infuse Tool", "Lesser Restoration",
            "Longstrider", "Mage Armor", "Magic Missile", "Protection from Evil and Good", "Identify",
            "Thunderwave",
            
            // 2nd Level
            "Alter Self", "Blindness/Deafness", "Blur", "Cloud of Daggers", "Crown of Stars",
            "Darkvision", "Detect Thoughts", "Dolphin's Dance", "Enlarge/Reduce", "Fire Shield",
            "Gaseous Form", "Gust of Wind", "Heat Metal", "Hold Person", "Lightning Arrow",
            "Magic Mouth", "Mind Fog", "Mirror Image", "Misty Step", "Pyrotechnics", "Scorching Ray",
            "See Invisibility", "Shatter", "Shillelagh", "Spike Growth", "Water Breathing", "Web",
            
            // 3rd Level
            "Bestow Curse", "Blink", "Contingency", "Dispel Magic", "Fear",
            "Fireball", "Fly", "Gaseous Form", "Glyph of Warding", "Haste",
            "Hypnotic Pattern", "Lightning Bolt", "Nondetection", "Polymorph", "Project Image",
            "Reverse Gravity", "Slow", "Stinking Cloud", "Tongues", "Vitriolic Sphere",
            
            // 4th Level
            "Arcane Eye", "Blight", "Confusion", "Dimension Door", "Greater Restoration",
            "Ice Storm", "Phantasmal Killer", "Stoneskin", "Wall of Fire", "Mordenkainen's Sword",
            
            // 5th Level
            "Arcane Hand", "Cone of Cold", "Creation", "Dominate Person", "Dream",
            "Geas", "Greater Restoration", "Hold Monster", "Insect Plague", "Legend Lore",
            "Mass Cure Wounds", "Mislead", "Planar Binding", "Scrying", "See Invisibility", "Tenser's Transformation"
        };

        /// <summary>
        /// Spells available to Wisdom-based casters (Cleric, Druid, Ranger).
        /// </summary>
        private static readonly List<string> _wisdomSpells = new()
        {
            // Cantrips
            "Guidance", "Sacred Flame", "Thaumaturgy", "Wizard Spell", "Shocking Grasp",
            
            // 1st Level
            "Bless", "Command", "Create/Destroy Water", "Detect Magic", "Guiding Bolt",
            "Healing Word", "Heroism", "Inflict Wounds", "Jump", "Sanctuary",
            "Cure Wounds", "Find Familiar", "Goodberry", "Hunger of Hadar", "Protection from Evil and Good",
            
            // 2nd Level
            "Augury", "Blur", "Darkvision", "Detect Thoughts", "Enhance Ability",
            "Find Steed", "Flaming Sphere", "Glass Strike", "Lesser Restoration", "Moonbeam",
            "Rope Trick", "Scorching Ray", "See Invisibility", "Shillelagh", "Spike Growth",
            
            // 3rd Level
            "Bestow Curse", "Call Lightning", "Clairvoyance", "Dispel Magic", "Fear",
            "Feign Death", "Fly", "Gentle Repose", "Haste", "Hypnotic Pattern",
            "Lightning Bolt", "Main Cure Wounds", "Major Image", "Plant Growth", "Remove Curse",
            "Slow", "Spirit Guardians", "Tongues", "Water Walk",
            
            // 4th Level
            "Blight", "Confusion", "Divination", "Freedom of Movement", "Guardian of Nature",
            "Stone Shape", "Stoneskin", "Wall of Fire", "Wall of Thorns"
        };

        /// <summary>
        /// Spells available to Charisma-based casters (Paladin).
        /// </summary>
        private static readonly List<string> _charismaSpells = new()
        {
            // Cantrips
            "Guidance", "Thunderous Strike",
            
            // 1st Level
            "Bless", "Cure Wounds", "Detect Evil and Good", "Hellish Rebuke", "Humiliate Monster",
            "Protection from Evil and Good", "Wrathful Smite", "Zephyr Strike",
            
            // 2nd Level
            "Aura of Vitality", "Brand Illusion", "Cloud of Daggers", "Crown of Madness",
            "Darkvision", "Enhance Ability", "Find Steed", "Lesser Restoration", "Locate Object",
            "Magic Mouth", "Mask of Many Faces", "Phantom Steed", "Protection from Poison",
            "Searcher's Mark", "Shillelagh", "Spike Growth",
            
            // 3rd Level
            "Banishment", "Beacon of Hope", "Bestow Curse", "Castigate", "Dawn",
            "Daylight", "Dispel Magic", "Fear", "Glyph of Warding", "Haste",
            "Legend Lore", "Lesser Restoration", "Main Cure Wounds", "Mordenkainen's Private Sanctum",
            "Nondetection", "Planeswalker's Touch", "Revivify", "Sending", "Speak with Dead",
            "Tongues", "Void of Madness"
        };

        /// <summary>
        /// Gets all spells available to a specific spellcasting ability.
        /// </summary>
        /// <param name="castingAbility">The spellcasting ability: "Intelligence", "Wisdom", or "Charisma"</param>
        /// <returns>List of spell names available to that casting ability</returns>
        public static List<string> GetSpellsByCastingAbility(string castingAbility)
        {
            return castingAbility switch
            {
                "Intelligence" => new List<string>(_intelligenceSpells),
                "Wisdom" => new List<string>(_wisdomSpells),
                "Charisma" => new List<string>(_charismaSpells),
                _ => new List<string>()
            };
        }

        /// <summary>
        /// Gets all spells of a specific level for a casting ability.
        /// </summary>
        /// <param name="spellLevel">The spell level (0-9)</param>
        /// <param name="castingAbility">The spellcasting ability</param>
        /// <returns>List of Spell objects</returns>
        public static List<Spell> GetSpellsByLevel(int spellLevel, string castingAbility)
        {
            var spells = new List<Spell>();
            var spellNames = GetSpellsByCastingAbility(castingAbility);
            
            foreach (var name in spellNames)
            {
                if (_allSpells.TryGetValue(name, out Spell? spell))
                {
                    if (spell.Level == spellLevel)
                    {
                        spells.Add(spell);
                    }
                }
            }
            
            return spells;
        }

        /// <summary>
        /// Finds a spell by name.
        /// </summary>
        public static Spell? FindSpell(string name)
        {
            _allSpells.TryGetValue(name, out Spell? spell);
            return spell;
        }

        /// <summary>
        /// Checks if a spell exists in the database.
        /// </summary>
        public static bool SpellExists(string name)
        {
            return _allSpells.ContainsKey(name);
        }

        /// <summary>
        /// Gets all available spell names.
        /// </summary>
        public static List<string> GetAllSpellNames()
        {
            return new List<string>(_allSpells.Keys);
        }
    }
}