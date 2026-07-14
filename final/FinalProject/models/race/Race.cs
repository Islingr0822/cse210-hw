using System;
using System.Collections.Generic;
using System.Text;

namespace DnDCharacterManager.Race
{
    /// <summary>
    /// Enum representing racial trait flags for quick lookup.
    /// </summary>
    public enum RacialTrait
    {
        None,
        Darkvision,
        SuperiorDarkvision,
        FeyAncestry,
        Trance,
        Sleepless,
        RelentlessEndurance,
        SharpSenses,
        PowerfulBuild,
        DragonResistance,
        BreathWeapon,
        SpellResistance,
        NaturalWeapons,
        PoisonImmunity,
        MagicResistance,
        Mimicry,
        FelineSenses,
        NaturalArmor,
        Trunk,
        FireResistance,
        ColdResistance,
        AcidResistance,
        LightningResistance,
        ThunderResistance
    }

    /// <summary>
    /// Enum for character sizes.
    /// </summary>
    public enum Size
    {
        Tiny,
        Small,
        Medium,
        Large,
        Huge,
        Gargantuan
    }

    /// <summary>
    /// Represents a feature object for race traits.
    /// </summary>
    public class RacialFeature
    {
        protected string _name;
        protected string _description;

        public RacialFeature()
        {
            _name = "Unnamed Feature";
            _description = "No description.";
        }

        public RacialFeature(string name, string description)
        {
            _name = name;
            _description = description;
        }

        public string Name { get => _name; set => _name = value; }
        public string Description { get => _description; set => _description = value; }

        public override string ToString()
        {
            return $"{_name}: {_description}";
        }
    }

    /// <summary>
    /// Abstract base class for all character races.
    /// </summary>
    public abstract class Race
    {
        // ==================== Core Fields ====================

        protected string _name;
        protected Size _size;
        protected int _speed;
        protected bool _darkvision;
        protected int _darkvisionRange;

        // ==================== Enhanced Fields ====================

        protected Dictionary<string, int> _abilityIncreases;
        protected List<string> _languages;
        protected List<RacialFeature> _racialFeatures;
        protected List<RacialTrait> _traitFlags;

        // ==================== Constructors ====================

        /// <summary>
        /// Default parameterless constructor for derived classes.
        /// </summary>
        protected Race()
        {
            _name = "Common";
            _size = Size.Medium;
            _speed = 30;
            _darkvision = false;
            _darkvisionRange = 60;

            _abilityIncreases = new Dictionary<string, int>();
            _languages = new List<string> { "Common" };
            _racialFeatures = new List<RacialFeature>();
            _traitFlags = new List<RacialTrait>();
        }

        /// <summary>
        /// Parameterized constructor for creating races with specific values.
        /// </summary>
        public Race(string name, Size size, int speed, bool darkvision)
        {
            _name = name;
            _size = size;
            _speed = speed;
            _darkvision = darkvision;
            _darkvisionRange = 60;

            _abilityIncreases = new Dictionary<string, int>();
            _languages = new List<string> { "Common" };
            _racialFeatures = new List<RacialFeature>();
            _traitFlags = new List<RacialTrait>();
        }

        /// <summary>
        /// Constructor with darkvision range specification.
        /// </summary>
        public Race(string name, Size size, int speed, bool darkvision, int darkvisionRange)
        {
            _name = name;
            _size = size;
            _speed = speed;
            _darkvision = darkvision;
            _darkvisionRange = darkvisionRange;

            _abilityIncreases = new Dictionary<string, int>();
            _languages = new List<string> { "Common" };
            _racialFeatures = new List<RacialFeature>();
            _traitFlags = new List<RacialTrait>();
        }

        // ==================== Core Properties ====================

        public string Name { get => _name; set => _name = value; }
        public Size Size { get => _size; set => _size = value; }
        public int Speed { get => _speed; set => _speed = value; }
        public bool Darkvision { get => _darkvision; set => _darkvision = value; }
        public int DarkvisionRange { get => _darkvisionRange; set => _darkvisionRange = value; }

        // ==================== Enhanced Properties ====================

        public Dictionary<string, int> AbilityIncreases { get => _abilityIncreases; protected set => _abilityIncreases = value; }
        public List<string> Languages { get => _languages; }
        public List<RacialFeature> RacialFeatures { get => _racialFeatures; }
        public List<RacialTrait> TraitFlags { get => _traitFlags; }

        // ==================== Abstract Methods ====================

        /// <summary>
        /// Returns the ability score increases for this race.
        /// </summary>
        public abstract Dictionary<string, int> GetAbilityIncreases();

        /// <summary>
        /// Returns the list of languages this race speaks.
        /// </summary>
        public abstract List<string> GetLanguages();

        /// <summary>
        /// Applies racial features to a character. Override in derived classes.
        /// </summary>
        public virtual void ApplyTraits()
        {
            foreach (var feature in _racialFeatures)
            {
                Console.WriteLine($"  - {feature.Name}: {feature.Description}");
            }
        }

        // ==================== Trait Methods ====================

        /// <summary>
        /// Checks if this race has a specific trait flag.
        /// </summary>
        public bool HasTrait(RacialTrait trait)
        {
            return _traitFlags.Contains(trait);
        }

        /// <summary>
        /// Adds a racial feature as a RacialFeature object.
        /// </summary>
        public void AddRacialFeature(string name, string description)
        {
            var feature = new RacialFeature(name, description);
            _racialFeatures.Add(feature);
        }

        /// <summary>
        /// Adds a trait flag to this race.
        /// </summary>
        public void AddTrait(RacialTrait trait)
        {
            if (!_traitFlags.Contains(trait))
            {
                _traitFlags.Add(trait);
            }
        }

        /// <summary>
        /// Removes a trait flag from this race.
        /// </summary>
        public void RemoveTrait(RacialTrait trait)
        {
            _traitFlags.Remove(trait);
        }

        /// <summary>
        /// Returns all racial features as formatted strings.
        /// </summary>
        public List<string> GetRacialFeatureDescriptions()
        {
            var descriptions = new List<string>();
            foreach (var feature in _racialFeatures)
            {
                descriptions.Add($"{feature.Name}: {feature.Description}");
            }
            return descriptions;
        }

        // ==================== Integration Methods ====================

        /// <summary>
        /// Returns the ability score increases as a display string.
        /// </summary>
        public virtual string GetAbilityIncreaseDescription()
        {
            if (_abilityIncreases.Count == 0)
                return "No ability score increases.";

            var parts = new List<string>();
            foreach (var kvp in _abilityIncreases)
            {
                parts.Add("+" + kvp.Value.ToString() + " " + kvp.Key);
            }
            return string.Join(", ", parts);
        }

        /// <summary>
        /// Gets the list of languages as a comma-separated string.
        /// </summary>
        public virtual string GetLanguageList()
        {
            return string.Join(", ", _languages);
        }

        // ==================== Display Methods ====================

        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.Append(_name);
            sb.Append(" (Size: ");
            sb.Append(_size.ToString());
            sb.Append(", Speed: ");
            sb.Append(_speed.ToString());
            sb.Append(", Darkvision: ");
            if (_darkvision)
            {
                sb.Append(_darkvisionRange.ToString());
                sb.Append("ft");
            }
            else
            {
                sb.Append("None");
            }
            sb.Append(")");

            if (_abilityIncreases.Count > 0)
            {
                sb.Append(" | Abilities: ");
                var abParts = new List<string>();
                foreach (var kvp in _abilityIncreases)
                {
                    abParts.Add("+" + kvp.Value.ToString() + " " + kvp.Key);
                }
                sb.Append(string.Join(", ", abParts));
            }

            if (_languages.Count > 1 || (_languages.Count == 1 && _languages[0] != "Common"))
            {
                sb.Append(" | Languages: ");
                sb.Append(GetLanguageList());
            }

            return sb.ToString();
        }

        /// <summary>
        /// Displays all race information to the console.
        /// </summary>
        public virtual void DisplayInfo()
        {
            Console.WriteLine("=== " + _name + " ===");
            Console.WriteLine("Size: " + _size.ToString());
            Console.WriteLine("Speed: " + _speed.ToString() + " ft");
            if (_darkvision)
            {
                Console.WriteLine("Darkvision: " + _darkvisionRange.ToString() + " ft");
            }
            else
            {
                Console.WriteLine("Darkvision: None");
            }

            if (_abilityIncreases.Count > 0)
            {
                Console.WriteLine("Ability Score Increases:");
                foreach (var kvp in _abilityIncreases)
                {
                    Console.WriteLine("  +" + kvp.Value.ToString() + " " + kvp.Key);
                }
            }

            if (_languages.Count > 0)
            {
                Console.WriteLine("Languages: " + GetLanguageList());
            }

            if (_racialFeatures.Count > 0)
            {
                Console.WriteLine("Racial Features:");
                foreach (var feature in _racialFeatures)
                {
                    Console.WriteLine("  - " + feature.Name + ": " + feature.Description);
                }
            }

            if (_traitFlags.Count > 0)
            {
                Console.Write("Traits: ");
                var traitNames = new List<string>();
                foreach (var trait in _traitFlags)
                {
                    if (trait != RacialTrait.None)
                    {
                        traitNames.Add(trait.ToString());
                    }
                }
                Console.WriteLine(string.Join(", ", traitNames));
            }
        }
    }

    // ==================== Core Race Implementations (PHB) ====================

    /// <summary>
    /// Human race - versatile and adaptable.
    /// </summary>
    public class Human : Race
    {
        public Human() : base("Human", Size.Medium, 30, false)
        {
            _abilityIncreases = new Dictionary<string, int>
            {
                { "Strength", 1 },
                { "Dexterity", 1 },
                { "Constitution", 1 },
                { "Intelligence", 1 },
                { "Wisdom", 1 },
                { "Charisma", 1 }
            };

            _languages = new List<string> { "Common" };

            AddRacialFeature("Extra Language", "Humans can speak Common and one extra language of their choice.");
            AddTrait(RacialTrait.SharpSenses);
        }

        public Human(int speed) : base("Human", Size.Medium, speed, false)
        {
            _abilityIncreases = new Dictionary<string, int>
            {
                { "Strength", 1 },
                { "Dexterity", 1 },
                { "Constitution", 1 },
                { "Intelligence", 1 },
                { "Wisdom", 1 },
                { "Charisma", 1 }
            };

            _languages = new List<string> { "Common" };
            AddRacialFeature("Extra Language", "Humans can speak Common and one extra language of their choice.");
            AddTrait(RacialTrait.SharpSenses);
        }

        public override Dictionary<string, int> GetAbilityIncreases() { return _abilityIncreases; }
        public override List<string> GetLanguages() { return _languages; }
    }

    /// <summary>
    /// Elf race - graceful and wise, with darkvision and fey ancestry.
    /// </summary>
    public class Elf : Race
    {
        public Elf() : base("Elf", Size.Medium, 30, true, 60)
        {
            _abilityIncreases = new Dictionary<string, int>
            {
                { "Dexterity", 2 }
            };

            _languages = new List<string> { "Elvish" };

            AddRacialFeature("Fey Ancestry", "You have advantage on saving throws against being charmed, and magic cannot put you to sleep.");
            AddRacialFeature("Trance", "Elves don't need to sleep. Instead, they meditate deeply for 4 hours a day. After resting, elves get the same benefit as a human from 8 hours of sleep.");
            AddTrait(RacialTrait.Darkvision);
            AddTrait(RacialTrait.FeyAncestry);
            AddTrait(RacialTrait.Trance);
        }

        public override Dictionary<string, int> GetAbilityIncreases() { return _abilityIncreases; }
        public override List<string> GetLanguages() { return _languages; }
    }

    /// <summary>
    /// High Elf variant of Elf with extra benefits.
    /// </summary>
    public class HighElf : Elf
    {
        public HighElf() : base()
        {
            _name = "High Elf";
            RemoveTrait(RacialTrait.Darkvision);

            AddRacialFeature("Cantrip", "You can know one additional wizard cantrip of your choice from the wizard spell list. Intelligence is your spellcasting ability.");
            AddRacialFeature("Languages", "You can read, speak, and write two extra languages of your choice.");
        }
    }

    /// <summary>
    /// Wood Elf variant of Elf with increased speed.
    /// </summary>
    public class WoodElf : Elf
    {
        public WoodElf() : base()
        {
            _name = "Wood Elf";
            _speed = 35;

            AddRacialFeature("Fast Speed", "Your walking speed increases to 35 feet.");
            AddRacialFeature("Skill Mastery", "You gain proficiency with one skill of your choice.");
        }
    }

    /// <summary>
    /// Drow (Dark Elf) variant.
    /// </summary>
    public class Drow : Elf
    {
        public Drow() : base()
        {
            _name = "Drow";

            AddRacialFeature("Sunlight Sensitivity", "You have disadvantage on attack rolls and Wisdom (Perception) checks that rely on sight in bright sunlight.");
            AddRacialFeature("Fey Magic", "You can cast the faerie fire spell once with this trait and regain the ability to do so when you finish a long rest.");

            _abilityIncreases["Charisma"] = 1;
        }
    }

    /// <summary>
    /// Dwarf race - resilient and stubborn, with darkvision and poison resistance.
    /// </summary>
    public class Dwarf : Race
    {
        public Dwarf() : base("Dwarf", Size.Medium, 25, true, 60)
        {
            _abilityIncreases = new Dictionary<string, int>
            {
                { "Constitution", 2 }
            };

            _languages = new List<string> { "Dwarvish" };

            AddRacialFeature("Dark Resilience", "You have resistance against poison damage and advantage on saving throws against it.");
            AddRacialFeature("Stonecunning", "Proficiency with tools related to stone. You also have heightened sense for danger around burrows, traps, and construction.");
            AddTrait(RacialTrait.Darkvision);
            AddTrait(RacialTrait.PoisonImmunity);
        }

        public override Dictionary<string, int> GetAbilityIncreases() { return _abilityIncreases; }
        public override List<string> GetLanguages() { return _languages; }
    }

    /// <summary>
    /// Hill Dwarf variant with extra wisdom and hit points.
    /// </summary>
    public class HillDwarf : Dwarf
    {
        public HillDwarf() : base()
        {
            _name = "Hill Dwarf";

            AddRacialFeature("Extra Hit Point", "Your hit point maximum increases by 1, and it increases by 1 every time you gain a level.");
        }
    }

    /// <summary>
    /// Mountain Dwarf variant with armor proficiency.
    /// </summary>
    public class MountainDwarf : Dwarf
    {
        public MountainDwarf() : base()
        {
            _name = "Mountain Dwarf";

            AddRacialFeature("Armor Proficiency", "You can train in light and medium armor.");
        }
    }

    /// <summary>
    /// Dragonborn race - proud and powerful, with breath weapon and resistance.
    /// </summary>
    public class Dragonborn : Race
    {
        private string _breathWeaponType;
        private string _breathDamageType;
        private int _breathWeaponScale;

        public Dragonborn() : base("Dragonborn", Size.Medium, 30, false)
        {
            _abilityIncreases = new Dictionary<string, int>
            {
                { "Strength", 2 },
                { "Charisma", 1 }
            };

            _languages = new List<string> { "Draconic" };

            _breathDamageType = "fire";
            _breathWeaponScale = 2;

            AddRacialFeature("Breath Weapon", "Choose a 5-foot cube within reach. Each creature in that area must make a Dexterity saving throw. On a failed save, a creature takes 2d6 damage based on your draconic ancestry.");
            AddRacialFeature("Resistance", "You have resistance to the damage type from your breath weapon.");

            AddTrait(RacialTrait.BreathWeapon);
            AddTrait(RacialTrait.DragonResistance);
        }

        public Dragonborn(string breathWeaponType, string damageType) : base("Dragonborn", Size.Medium, 30, false)
        {
            _abilityIncreases = new Dictionary<string, int>
            {
                { "Strength", 2 },
                { "Charisma", 1 }
            };

            _languages = new List<string> { "Draconic" };
            _breathWeaponType = breathWeaponType;
            _breathDamageType = damageType;
            _breathWeaponScale = 2;

            AddRacialFeature("Breath Weapon", "Choose a 5-foot cube within reach. Each creature in that area must make a Dexterity saving throw. On a failed save, a creature takes " + _breathWeaponScale + "d6 damage of the " + damageType + " type.");
            AddRacialFeature("Resistance", "You have resistance to the damage type from your breath weapon.");

            AddTrait(RacialTrait.BreathWeapon);
            AddTrait(RacialTrait.DragonResistance);
        }

        public override Dictionary<string, int> GetAbilityIncreases() { return _abilityIncreases; }
        public override List<string> GetLanguages() { return _languages; }

        public string BreathWeaponType { get { return _breathWeaponType; } set { _breathWeaponType = value; } }
        public string BreathDamageType { get { return _breathDamageType; } set { _breathDamageType = value; } }
    }

    /// <summary>
    /// Half-Elf race - charming and adaptable.
    /// </summary>
    public class HalfElf : Race
    {
        private List<string> _skillProficiencies;

        public HalfElf() : base("Half-Elf", Size.Medium, 30, true, 60)
        {
            _abilityIncreases = new Dictionary<string, int>();
            _languages = new List<string> { "Elvish", "Common" };

            _skillProficiencies = new List<string>();

            AddRacialFeature("Fey Ancestry", "You have advantage on saving throws against being charmed, and magic cannot put you to sleep.");
            AddRacialFeature("Skill Versatility", "You gain proficiency with one skill of your choice.");
            AddTrait(RacialTrait.Darkvision);
            AddTrait(RacialTrait.FeyAncestry);
        }

        public HalfElf(Dictionary<string, int> abilityIncreases) : base("Half-Elf", Size.Medium, 30, true, 60)
        {
            _abilityIncreases = abilityIncreases;
            _languages = new List<string> { "Elvish", "Common" };

            AddRacialFeature("Fey Ancestry", "You have advantage on saving throws against being charmed, and magic cannot put you to sleep.");
            AddRacialFeature("Skill Versatility", "You gain proficiency with one skill of your choice.");
            AddTrait(RacialTrait.Darkvision);
            AddTrait(RacialTrait.FeyAncestry);
        }

        public override Dictionary<string, int> GetAbilityIncreases() { return _abilityIncreases; }
        public override List<string> GetLanguages() { return _languages; }

        public List<string> SkillProficiencies { get { return _skillProficiencies; } set { _skillProficiencies = value; } }
    }

    /// <summary>
    /// Half-Orc race - fierce and resilient.
    /// </summary>
    public class HalfOrc : Race
    {
        public HalfOrc() : base("Half-Orc", Size.Medium, 30, true, 60)
        {
            _abilityIncreases = new Dictionary<string, int>
            {
                { "Strength", 2 },
                { "Constitution", 1 }
            };

            _languages = new List<string> { "Dwarvish" };

            AddRacialFeature("Savage Attacks", "You can roll one additional weapon damage die when determining damage for a melee attack.");
            AddRacialFeature("Relentless Endurance", "When you are reduced to 0 hit points but not killed outright, you can drop to 1 hit point instead. You can't use this feature again until you finish a long rest.");
            AddRacialFeature("Menacing", "You gain proficiency with the Intimidation skill.");
            AddTrait(RacialTrait.Darkvision);
            AddTrait(RacialTrait.RelentlessEndurance);
        }

        public override Dictionary<string, int> GetAbilityIncreases() { return _abilityIncreases; }
        public override List<string> GetLanguages() { return _languages; }
    }

    /// <summary>
    /// Halfling race - nimble and lucky.
    /// </summary>
    public class Halfling : Race
    {
        public Halfling() : base("Halfling", Size.Small, 25, false)
        {
            _abilityIncreases = new Dictionary<string, int>
            {
                { "Dexterity", 2 }
            };

            _languages = new List<string> { "Halfling" };

            AddRacialFeature("Lucky", "When you roll a 1 on a d20 for an attack roll, ability check, or saving throw, you can reroll the die and must use the new roll.");
            AddRacialFeature("Braavenful", "You can move through the space of any creature that is larger than yours.");
            AddTrait(RacialTrait.PowerfulBuild);
        }

        public override Dictionary<string, int> GetAbilityIncreases() { return _abilityIncreases; }
        public override List<string> GetLanguages() { return _languages; }
    }

    /// <summary>
    /// Lightfoot Halfling variant.
    /// </summary>
    public class LightfootHalfling : Halfling
    {
        public LightfootHalfling() : base()
        {
            _name = "Lightfoot Halfling";

            AddRacialFeature("Naturally Stealthy", "You can attempt to hide even when you are only obscured by a creature that is at least one size larger than you.");
        }
    }

    /// <summary>
    /// Stout Halfling variant.
    /// </summary>
    public class StoutHalfling : Halfling
    {
        public StoutHalfling() : base()
        {
            _name = "Stout Halfling";

            AddRacialFeature("Stout Resilience", "You have resistance to poison damage.");
        }
    }
}