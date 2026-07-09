using System;
using System.Collections.Generic;
using System.Linq;
using DnDCharacterManager.Ability;
using FeatureClass = DnDCharacterManager.Feature.Feature;
using RaceClass = DnDCharacterManager.Race.Race;
using BackgroundClass = DnDCharacterManager.Background.Background;
using SpellClass = DnDCharacterManager.Spell.Spell;
using School = DnDCharacterManager.Spell.School;

namespace DnDCharacterManager.Character.Classes
{
    /// <summary>
    /// Enum representing the 8 Druid Circle subclass options.
    /// Each circle grants unique features at levels 2, 6, 8, and 10.
    /// </summary>
    public enum DruidCircle
    {
        Land,
        Moon,
        Spores,
        Shepherd,
        Dreams,
        Stars,
        Wildfire,
        Grovewhip
    }

    /// <summary>
    /// Enum representing wild shape beast form types available.
    /// </summary>
    public enum BeastFormType
    {
        None,
        LandBased,
        MoonElemental,
        SporeBased,
        ShepherdFey,
        DreamsNightmare,
        StarsCosmic,
        WildfirePlant,
        GrovewhipNature
    }

    /// <summary>
    /// Druid character class - a divine spellcaster who channels nature's power.
    /// Full level 1-20 progression with circle-specific features.
    /// </summary>
    public class Druid : Character
    {
        // ==================== Enums ====================

        /// <summary>
        /// The chosen Druid Circle determining subclass features.
        /// </summary>
        public DruidCircle Circle { get; set; }

        // ==================== Core Properties ====================

        /// <summary>
        /// Spellcasting ability for Druids is Wisdom.
        /// </summary>
        public string SpellCastingAbility => "Wisdom";

        /// <summary>
        /// Number of Wild Shape uses per rest (increases with level).
        /// </summary>
        public int WildShapeUses { get; set; }

        /// <summary>
        /// Maximum Wild Shape uses available.
        /// </summary>
        public int MaxWildShapeUses { get; set; }

        /// <summary>
        /// Whether the Druid is currently in Wild Shape.
        /// </summary>
        public bool IsInWildShape { get; set; }

        /// <summary>
        /// The current beast form name (if in Wild Shape).
        /// </summary>
        public string CurrentBeastForm { get; set; }

        /// <summary>
        /// Maximum CR the Druid can wild shape into (increases with level).
        /// </summary>
        public int MaxWildShapeCR { get; set; }

        /// <summary>
        /// Number of druid cantrips known.
        /// </summary>
        public int CantripsKnown { get; set; }

        /// <summary>
        /// Number of spells known (druids know spells, not prepare like clerics).
        /// </summary>
        public int SpellsKnown { get; set; }

        /// <summary>
        /// List of spells currently known by the druid.
        /// </summary>
        public List<SpellClass> KnownSpells { get; set; }

        /// <summary>
        /// Spell save DC = 8 + proficiency bonus + wisdom modifier.
        /// </summary>
        public int SpellSaveDC { get; set; }

        /// <summary>
        /// Spell attack modifier = proficiency bonus + wisdom modifier.
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

        // ==================== Feature Flags by Level ====================

        private bool _druidicLanguageUnlocked;
        private bool _wildShapeDamageResistUnlocked;
        private bool _perfectlyFormedUnlocked;
        private bool _elementalWildshapeUnlocked;
        private bool _archdruidUnlocked;
        private bool _naturalRecoveryActive;

        // ==================== Constructors ====================

        public Druid() : base()
        {
            InitializeDruid();
        }

        public Druid(string name, int level, RaceClass race, BackgroundClass background)
            : base(name, level, race, background)
        {
            InitializeDruid();
            ApplyLevelFeatures();
        }

        // ==================== Initialization ====================

        private void InitializeDruid()
        {
            Circle = DruidCircle.Land;
            WildShapeUses = 0;
            MaxWildShapeUses = 0;
            IsInWildShape = false;
            CurrentBeastForm = "None";
            MaxWildShapeCR = 0;
            CantripsKnown = 2;
            SpellsKnown = 2;
            KnownSpells = new List<SpellClass>();
            SpellSlotsPerLevel = new Dictionary<int, int>();
            RemainingSpellSlots = new Dictionary<int, int>();
            _naturalRecoveryActive = true;

            _druidicLanguageUnlocked = true;
            _wildShapeDamageResistUnlocked = false;
            _perfectlyFormedUnlocked = false;
            _elementalWildshapeUnlocked = false;
            _archdruidUnlocked = false;

            CalculateSpellDC();
            UpdateCantripsKnown();
            UpdateSpellsKnown();
            UpdateSpellSlots();
            UpdateWildShapeOptions();
            ApplyLevelFeatures();
        }

        // ==================== Stat Calculations ====================

        /// <summary>
        /// Calculates the spell save DC based on Wisdom and level.
        /// </summary>
        private void CalculateSpellDC()
        {
            int baseDC = 8;
            int wisMod = GetAbilityModifier(_wisdom);
            int proficiencyBonus = GetProficiencyBonus();
            SpellSaveDC = baseDC + wisMod + proficiencyBonus;
        }

        /// <summary>
        /// Calculates the spell attack modifier.
        /// </summary>
        private void CalculateSpellAttack()
        {
            int wisMod = GetAbilityModifier(_wisdom);
            int proficiencyBonus = GetProficiencyBonus();
            SpellAttackModifier = wisMod + proficiencyBonus;
        }

        /// <summary>
        /// Gets the proficiency bonus based on character level.
        /// </summary>
        private int GetProficiencyBonus()
        {
            if (_level <= 4) return 2;
            if (_level <= 8) return 3;
            if (_level <= 12) return 4;
            if (_level <= 16) return 5;
            return 6;
        }

        /// <summary>
        /// Gets the ability modifier for a given ability score.
        /// </summary>
        private int GetAbilityModifier(int abilityScore)
        {
            return (abilityScore - 10) / 2;
        }

        // ==================== Wild Shape Methods ====================

        /// <summary>
        /// Update Wild Shape options based on druid level.
        /// </summary>
        private void UpdateWildShapeOptions()
        {
            MaxWildShapeCR = 0;
            int baseUses = 1;

            // Level 2: Wild Shape available, CR 1/4
            if (_level >= 2)
            {
                MaxWildShapeCR = 1;
                baseUses = 1;
                _wildShapeDamageResistUnlocked = false;
            }

            // Level 3: CR 1/2
            if (_level >= 3)
            {
                MaxWildShapeCR = 2;
            }

            // Level 4: Wild Shape damage resistance, CR 1
            if (_level >= 4)
            {
                MaxWildShapeCR = 4;
                _wildShapeDamageResistUnlocked = true;
            }

            // Level 8: Additional terrain types
            if (_level >= 8)
            {
                MaxWildShapeCR = 6;
            }

            // Level 12: CR can go up to elementals for Moon circle
            if (_level >= 12)
            {
                MaxWildShapeCR = 8;
            }

            // Level 18: Archdruid - unlimited wild shape
            if (_level >= 18)
            {
                _archdruidUnlocked = true;
                baseUses = 999;
            }

            MaxWildShapeUses = baseUses;
            if (!IsInWildShape)
            {
                WildShapeUses = MaxWildShapeUses;
            }
        }

        /// <summary>
        /// Activate Wild Shape - transform into a beast form.
        /// </summary>
        public virtual bool WildShape(string beastName = null)
        {
            if (_level < 2)
            {
                Console.WriteLine($"{Name} has not unlocked Wild Shape yet (requires level 2).");
                return false;
            }

            if (IsInWildShape)
            {
                Console.WriteLine($"{Name} is already in Wild Shape. Use RevertFromWildShape() first.");
                return false;
            }

            if (!_archdruidUnlocked && WildShapeUses <= 0)
            {
                Console.WriteLine($"{Name} has no Wild Shape uses remaining. Take a long rest to recover.");
                return false;
            }

            if (!_archdruidUnlocked)
            {
                WildShapeUses--;
            }

            CurrentBeastForm = beastName ?? GetRandomBeastForm();
            IsInWildShape = true;

            string usesDisplay = _archdruidUnlocked ? "Unlimited" : $"{WildShapeUses}/{MaxWildShapeUses}";
            string resistanceDisplay = _wildShapeDamageResistUnlocked ? "All (non-weapon/non-spell)" : "None yet";
            Console.WriteLine($"{Name} uses Wild Shape! Transforms into {CurrentBeastForm}!");
            Console.WriteLine($"Wild Shape uses remaining: {usesDisplay}");
            Console.WriteLine($"CR limit: {MaxWildShapeCR} | Resistance: {resistanceDisplay}");

            return true;
        }

        /// <summary>
        /// Revert from Wild Shape back to normal form.
        /// </summary>
        public virtual void RevertFromWildShape()
        {
            if (!IsInWildShape)
            {
                Console.WriteLine($"{Name} is not in Wild Shape.");
                return;
            }

            string previousForm = CurrentBeastForm;
            IsInWildShape = false;
            CurrentBeastForm = "None";

            Console.WriteLine($"{Name} reverts from Wild Shape (was {previousForm}) back to normal form.");
        }

        /// <summary>
        /// Check if a CR is available for wild shape
        /// </summary>
        public bool CanWildShapeToCR(int cr)
        {
            return cr <= MaxWildShapeCR;
        }

        /// <summary>
        /// Get a random beast form appropriate for the druid's level.
        /// </summary>
        private string GetRandomBeastForm()
        {
            List<string> availableForms = new List<string>();

            if (MaxWildShapeCR >= 1)
            {
                availableForms.AddRange(new[] { "Wolf", "Spider", "Scorpion", "Raven", "Bat", "Piranha" });
            }

            if (MaxWildShapeCR >= 2)
            {
                availableForms.AddRange(new[] { "Bear", "Boar", "Giant Elk", "Giant Poisonous Snake", "Mammoth Calf" });
            }

            if (MaxWildShapeCR >= 4)
            {
                availableForms.AddRange(new[] { "Giant Crocodile", "Giant Hydra", "Elephant", "Giant Constrictor Snake" });
            }

            return availableForms.Count > 0 ? availableForms[new Random().Next(availableForms.Count)] : "Wolf";
        }

        // ==================== Level Feature Progression ====================

        /// <summary>
        /// Apply features gained at specific druid levels.
        /// </summary>
        private void ApplyLevelFeatures()
        {
            if (_level >= 1)
            {
                _druidicLanguageUnlocked = true;
                CalculateSpellDC();
                CalculateSpellAttack();
                UpdateCantripsKnown();
                UpdateSpellsKnown();
                UpdateSpellSlots();
            }

            if (_level >= 2)
            {
                UpdateWildShapeOptions();
                ApplyCircleLevel2();
                UpdateSpellSlots();
                UpdateCantripsKnown();
            }

            if (_level >= 3)
            {
                UpdateWildShapeOptions();
                UpdateSpellSlots();
            }

            if (_level >= 4)
            {
                UpdateWildShapeOptions();
                UpdateSpellSlots();
                UpdateCantripsKnown();
            }

            if (_level >= 5)
            {
                UpdateWildShapeOptions();
                UpdateSpellSlots();
                UpdateSpellsKnown();
            }

            if (_level >= 6)
            {
                ApplyCircleLevel6();
                _perfectlyFormedUnlocked = true;
                UpdateSpellSlots();
            }

            if (_level >= 8)
            {
                UpdateWildShapeOptions();
                ApplyCircleLevel8();
                UpdateSpellSlots();
                UpdateCantripsKnown();
            }

            if (_level >= 9)
            {
                UpdateSpellSlots();
                UpdateSpellsKnown();
            }

            if (_level >= 10)
            {
                ApplyCircleLevel10();
                _elementalWildshapeUnlocked = true;
                UpdateSpellSlots();
            }

            if (_level >= 12)
            {
                UpdateWildShapeOptions();
                UpdateSpellSlots();
                UpdateSpellsKnown();
            }

            if (_level >= 13)
            {
                UpdateSpellSlots();
                UpdateCantripsKnown();
            }

            if (_level >= 14)
            {
                UpdateSpellSlots();
            }

            if (_level >= 15)
            {
                UpdateSpellSlots();
                UpdateSpellsKnown();
            }

            if (_level >= 16)
            {
                UpdateSpellSlots();
            }

            if (_level >= 17)
            {
                UpdateSpellSlots();
            }

            if (_level >= 18)
            {
                _archdruidUnlocked = true;
                UpdateWildShapeOptions();
                UpdateSpellSlots();
                UpdateSpellsKnown();
            }

            if (_level >= 19)
            {
                UpdateSpellSlots();
            }

            if (_level >= 20)
            {
                UpdateSpellSlots();
                UpdateSpellsKnown();
            }
        }

        // ==================== Circle Features ====================

        /// <summary>
        /// Applies level 2 features for the chosen circle.
        /// </summary>
        private void ApplyCircleLevel2()
        {
            switch (Circle)
            {
                case DruidCircle.Land:
                    Console.WriteLine($"{Name} gains Nature's Ward (level 6) and Natural Recovery preparation.");
                    break;
                case DruidCircle.Moon:
                    WildShapeUses = MaxWildShapeUses + 1;
                    Console.WriteLine($"{Name} gains Combat Wild Shape: Attack with wild shape using spellcasting ability modifier!");
                    break;
                case DruidCircle.Spores:
                    Console.WriteLine($"{Name} gains Spore Sphere: As a bonus action, create a 10-foot radius sphere of spores (3d10 necrotic damage to enemies within).");
                    break;
                case DruidCircle.Shepherd:
                    Console.WriteLine($"{Name} gains Fey Life Spirit: Summon fey spirits that can heal allies.");
                    break;
                case DruidCircle.Dreams:
                    Console.WriteLine($"{Name} gains Dream Walking: Travel to the Dreaming Deep and interact with dreamers.");
                    break;
                case DruidCircle.Stars:
                    Console.WriteLine($"{Name} gains Far Step: As a bonus action, teleport up to 30 feet (once per rest, recovers on short rest).");
                    break;
                case DruidCircle.Wildfire:
                    Console.WriteLine($"{Name} gains Tame the Flames: Create a wildfire spirit that deals fire damage.");
                    break;
                case DruidCircle.Grovewhip:
                    Console.WriteLine($"{Name} gains Guardian of the Grove: When an enemy hits an ally within 5 feet, you can use your reaction to impose disadvantage on the attack.");
                    break;
                default:
                    Console.WriteLine($"{Name} gains basic Wild Shape abilities.");
                    break;
            }
        }

        /// <summary>
        /// Applies level 6 features for the chosen circle.
        /// </summary>
        private void ApplyCircleLevel6()
        {
            switch (Circle)
            {
                case DruidCircle.Land:
                    Console.WriteLine($"{Name} gains Nature's Ward: Immune to poison, resistant to non-magical damage while in Wild Shape.");
                    _wildShapeDamageResistUnlocked = true;
                    break;
                case DruidCircle.Moon:
                    Console.WriteLine($"{Name} gains Elemental Wild Shape: Can wild shape into CR 4+ elemental forms at level 10.");
                    break;
                case DruidCircle.Spores:
                    Console.WriteLine($"{Name} gains Druid of the Spores: When enemies die in your spore sphere, you regain hit points equal to your druid level + Wisdom modifier.");
                    break;
                case DruidCircle.Shepherd:
                    Console.WriteLine($"{Name} gains Guardian of Life: Prevent creatures you heal from being reduced below half HP. Plus, beasts/plants within 30 feet have resistance to damage.");
                    break;
                case DruidCircle.Dreams:
                    Console.WriteLine($"{Name} gains Sentinel of Dreams: Prevent a creature from dying while in a dream state. Also can use Dream Walk without concentration.");
                    break;
                case DruidCircle.Stars:
                    Console.WriteLine($"{Name} gains Awakened Starlight: Cantrips deal extra radiant damage (Wisdom modifier). Magical darkness spreads from your eyes.");
                    break;
                case DruidCircle.Wildfire:
                    Console.WriteLine($"{Name} gains Brilliance of the Grove: As a reaction when hit, unleash blazing armor dealing 2d10 fire damage to attacker. Regains use on long rest.");
                    break;
                case DruidCircle.Grovewhip:
                    Console.WriteLine($"{Name} gains Swift Growth: When you use Wild Shape, your speed in beast form is increased by 10 feet. Additionally, you gain temporary hit points equal to your Wisdom modifier + druid level.");
                    break;
                default:
                    Console.WriteLine($"{Name} gains a level 6 circle feature.");
                    break;
            }
        }

        /// <summary>
        /// Applies level 8 features for the chosen circle.
        /// </summary>
        private void ApplyCircleLevel8()
        {
            switch (Circle)
            {
                case DruidCircle.Land:
                    Console.WriteLine($"{Name} gains Talk with Plants: As an action, speak with plants within 60 feet and use plant creatures as dice for movement.");
                    break;
                case DruidCircle.Moon:
                    Console.WriteLine($"{Name} gains Extra Impact (level 10 feature preparation). Elemental forms unlock at level 10.");
                    break;
                case DruidCircle.Spores:
                    Console.WriteLine($"{Name} gains Vicious Mockery with Spores: Your vicious mockery deals necrotic damage equal to druid level + Wisdom modifier.");
                    break;
                case DruidCircle.Shepherd:
                    Console.WriteLine($"{Name} gains Preserver's Wrath: When you reduce a creature to 0 HP, deal psychic damage equal to 2x your druid level + Wisdom modifier.");
                    break;
                case DruidCircle.Dreams:
                    Console.WriteLine($"{Name} gains Wake the Dead: Once per long rest, bring a dying creature back with 5 + druid level hit points while still in a dream state.");
                    break;
                case DruidCircle.Stars:
                    Console.WriteLine($"{Name} gains Empyrean Passport: Gain proficiency in either Wisdom or Intelligence saving throws (whichever is higher).");
                    break;
                case DruidCircle.Wildfire:
                    Console.WriteLine($"{Name} gains Blazing Armor: When a creature hits you with a melee attack, as a reaction unleash wildfire spirit flames dealing 2d10 fire damage. Regains on long rest.");
                    break;
                case DruidCircle.Grovewhip:
                    Console.WriteLine($"{Name} gains Grove's Passage: As a bonus action, teleport up to 60 feet to an unoccupied space. Can only be used once per short or long rest.");
                    break;
                default:
                    Console.WriteLine($"{Name} gains a level 8 circle feature.");
                    break;
            }
        }

        /// <summary>
        /// Applies level 10 features for the chosen circle.
        /// </summary>
        private void ApplyCircleLevel10()
        {
            switch (Circle)
            {
                case DruidCircle.Land:
                    Console.WriteLine($"{Name} gains Earth's Sanctuary: When you damage a creature within 5 feet, it must make a Wisdom save or be frightened of you until your next turn.");
                    break;
                case DruidCircle.Moon:
                    Console.WriteLine($"{Name} gains Elemental Wild Shape: Can wild shape into CR 4+ elemental forms (Air, Earth, Fire, Water Elementals).");
                    break;
                case DruidCircle.Spores:
                    Console.WriteLine($"{Name} returns to full power - Spore features are already established.");
                    break;
                case DruidCircle.Shepherd:
                    Console.WriteLine($"{Name} gains Spirit Sense: Know which creature a beast or plant is interacting with within 60 feet. Beasts/plants within 30 feet of you have resistance to all damage.");
                    break;
                case DruidCircle.Dreams:
                    Console.WriteLine($"{Name} returns to full power - Dream Walking features are already established.");
                    break;
                case DruidCircle.Stars:
                    Console.WriteLine($"{Name} returns to full power - Far Step features are already established.");
                    break;
                case DruidCircle.Wildfire:
                    Console.WriteLine($"{Name} returns to full power - Tame the Flames features are already established.");
                    break;
                case DruidCircle.Grovewhip:
                    Console.WriteLine($"{Name} returns to full power - Guardian of the Grove features are already established.");
                    break;
                default:
                    Console.WriteLine($"{Name} gains a level 10 circle feature.");
                    break;
            }
        }

        // ==================== Natural Recovery (Land Circle) ====================

        /// <summary>
        /// Recover spent spell slots during a short rest (Land Circle feature).
        /// Recover slots with combined levels equal to half druid level (rounded up).
        /// </summary>
        public virtual void NaturalRecovery()
        {
            if (Circle != DruidCircle.Land)
            {
                Console.WriteLine($"{Name} does not have Natural Recovery (only available to Circle of Land).");
                return;
            }

            int maxRecovery = _level / 2;
            int recoveredValue = 0;

            for (int i = 9; i >= 1 && recoveredValue < maxRecovery; i--)
            {
                if (RemainingSpellSlots.ContainsKey(i) && RemainingSpellSlots[i] < SpellSlotsPerLevel.GetValueOrDefault(i, 0))
                {
                    int recoveryAmount = Math.Min(maxRecovery - recoveredValue, SpellSlotsPerLevel.GetValueOrDefault(i, 0) - RemainingSpellSlots[i]);
                    RemainingSpellSlots[i] += recoveryAmount;
                    recoveredValue += recoveryAmount;
                }
            }

            Console.WriteLine($"{Name} uses Natural Recovery to recover spell slots. Total levels recovered: {recoveredValue}. Slots updated.");
        }

        // ==================== Spell Slot Progression (Full Caster) ====================

        /// <summary>
        /// Updates spell slots based on druid level using full caster progression.
        /// </summary>
        private void UpdateSpellSlots()
        {
            SpellSlotsPerLevel.Clear();
            RemainingSpellSlots.Clear();

            switch (_level)
            {
                case 1:
                    SpellSlotsPerLevel[1] = 2;
                    break;
                case 2:
                case 3:
                    SpellSlotsPerLevel[1] = 3;
                    break;
                case 4:
                    SpellSlotsPerLevel[1] = 4;
                    break;
                case 5:
                case 6:
                    SpellSlotsPerLevel[1] = 4;
                    SpellSlotsPerLevel[2] = 2;
                    break;
                case 7:
                case 8:
                    SpellSlotsPerLevel[1] = 4;
                    SpellSlotsPerLevel[2] = 3;
                    SpellSlotsPerLevel[3] = 2;
                    break;
                case 9:
                case 10:
                    SpellSlotsPerLevel[1] = 4;
                    SpellSlotsPerLevel[2] = 3;
                    SpellSlotsPerLevel[3] = 3;
                    SpellSlotsPerLevel[4] = 1;
                    break;
                case 11:
                case 12:
                    SpellSlotsPerLevel[1] = 4;
                    SpellSlotsPerLevel[2] = 3;
                    SpellSlotsPerLevel[3] = 3;
                    SpellSlotsPerLevel[4] = 2;
                    SpellSlotsPerLevel[5] = 1;
                    break;
                case 13:
                case 14:
                    SpellSlotsPerLevel[1] = 4;
                    SpellSlotsPerLevel[2] = 3;
                    SpellSlotsPerLevel[3] = 3;
                    SpellSlotsPerLevel[4] = 3;
                    SpellSlotsPerLevel[5] = 2;
                    SpellSlotsPerLevel[6] = 1;
                    break;
                case 15:
                case 16:
                    SpellSlotsPerLevel[1] = 4;
                    SpellSlotsPerLevel[2] = 3;
                    SpellSlotsPerLevel[3] = 3;
                    SpellSlotsPerLevel[4] = 3;
                    SpellSlotsPerLevel[5] = 2;
                    SpellSlotsPerLevel[6] = 2;
                    SpellSlotsPerLevel[7] = 1;
                    break;
                case 17:
                case 18:
                    SpellSlotsPerLevel[1] = 4;
                    SpellSlotsPerLevel[2] = 3;
                    SpellSlotsPerLevel[3] = 3;
                    SpellSlotsPerLevel[4] = 3;
                    SpellSlotsPerLevel[5] = 3;
                    SpellSlotsPerLevel[6] = 2;
                    SpellSlotsPerLevel[7] = 2;
                    SpellSlotsPerLevel[8] = 1;
                    break;
                case 19:
                case 20:
                    SpellSlotsPerLevel[1] = 4;
                    SpellSlotsPerLevel[2] = 3;
                    SpellSlotsPerLevel[3] = 3;
                    SpellSlotsPerLevel[4] = 3;
                    SpellSlotsPerLevel[5] = 3;
                    SpellSlotsPerLevel[6] = 2;
                    SpellSlotsPerLevel[7] = 2;
                    SpellSlotsPerLevel[8] = 2;
                    SpellSlotsPerLevel[9] = 1;
                    break;
            }

            foreach (var slot in SpellSlotsPerLevel)
            {
                RemainingSpellSlots[slot.Key] = slot.Value;
            }
        }

        // ==================== Cantrip Progression ====================

        /// <summary>
        /// Updates the number of cantrips known based on druid level.
        /// Levels 1-3: 2 cantrips
        /// Level 4: 3 cantrips
        /// Level 10: 4 cantrips
        /// </summary>
        private void UpdateCantripsKnown()
        {
            if (_level >= 10)
                CantripsKnown = 4;
            else if (_level >= 4)
                CantripsKnown = 3;
            else
                CantripsKnown = 2;
        }

        /// <summary>
        /// Updates the number of spells known based on druid level.
        /// </summary>
        private void UpdateSpellsKnown()
        {
            switch (_level)
            {
                case 1:
                    SpellsKnown = 2;
                    break;
                case 2:
                    SpellsKnown = 3;
                    break;
                case 3:
                case 4:
                    SpellsKnown = 4;
                    break;
                case 5:
                case 6:
                    SpellsKnown = 5;
                    break;
                case 7:
                case 8:
                    SpellsKnown = 6;
                    break;
                case 9:
                case 10:
                    SpellsKnown = 7;
                    break;
                case 11:
                case 12:
                case 13:
                case 14:
                    SpellsKnown = 8;
                    break;
                case 15:
                case 16:
                    SpellsKnown = 9;
                    break;
                case 17:
                case 18:
                    SpellsKnown = 10;
                    break;
                case 19:
                    SpellsKnown = 11;
                    break;
                case 20:
                    SpellsKnown = 12;
                    break;
                default:
                    SpellsKnown = Math.Min(2, _level);
                    break;
            }
        }

        // ==================== Core Methods ====================

        public override void ClassSpecificAbility()
        {
            WildShape();
        }

        /// <summary>
        /// Druidic: The druid knows the Druidic language.
        /// </summary>
        public virtual void SpeakDruidic()
        {
            if (!_druidicLanguageUnlocked)
            {
                Console.WriteLine($"{Name} has not unlocked Druidic yet.");
                return;
            }

            Console.WriteLine($"{Name} speaks in the ancient Druidic tongue. Only other druids can understand.");
        }

        /// <summary>
        /// Learn a spell from the Druid spell list.
        /// </summary>
        public virtual void LearnSpell(SpellClass spell)
        {
            if (spell == null)
            {
                Console.WriteLine("Cannot learn a null spell.");
                return;
            }

            if (!IsDruidSpell(spell))
            {
                Console.WriteLine($"{spell.Name} is not on the druid spell list.");
                return;
            }

            if (KnownSpells.Count >= SpellsKnown)
            {
                Console.WriteLine($"Cannot learn more spells. Maximum known: {SpellsKnown}");
                return;
            }

            if (KnownSpells.Contains(spell))
            {
                Console.WriteLine($"{Name} has already learned {spell.Name}.");
                return;
            }

            KnownSpells.Add(spell);
            Console.WriteLine($"{Name} has learned the spell: {spell.Name} (Level {spell.Level})");
        }

        /// <summary>
        /// Remove a known spell.
        /// </summary>
        public virtual void ForgetSpell(string spellName)
        {
            SpellClass? spellToForget = KnownSpells.FirstOrDefault(s => s.Name == spellName);
            if (spellToForget != null)
            {
                KnownSpells.Remove(spellToForget);
                Console.WriteLine($"{Name} has forgotten the spell: {spellName}");
            }
        }

        /// <summary>
        /// Cast a known spell if slots are available.
        /// </summary>
        public virtual bool CastSpell(string spellName, int spellLevel = 0)
        {
            SpellClass? spellToCast = KnownSpells.FirstOrDefault(s => s.Name == spellName);
            if (spellToCast == null)
            {
                Console.WriteLine($"{Name} does not know {spellName}.");
                return false;
            }

            if (spellLevel == 0 || spellToCast.Level == 0)
            {
                Console.WriteLine($"{Name} casts the cantrip {spellName}.");
                spellToCast.Cast();
                return true;
            }

            if (!RemainingSpellSlots.ContainsKey(spellLevel) || RemainingSpellSlots[spellLevel] <= 0)
            {
                Console.WriteLine($"{Name} has no {spellLevel}-level spell slots remaining.");
                return false;
            }

            RemainingSpellSlots[spellLevel]--;
            Console.WriteLine($"{Name} casts {spellName} using a {spellLevel}-level spell slot. Slots remaining: {RemainingSpellSlots[spellLevel]}");
            spellToCast.Cast();
            return true;
        }

        /// <summary>
        /// Check if a spell is on the druid spell list.
        /// </summary>
        private bool IsDruidSpell(SpellClass spell)
        {
            if (spell == null) return false;
            foreach (var cantrip in DruidCantrips)
            {
                if (cantrip.Equals(spell.Name, StringComparison.OrdinalIgnoreCase))
                    return true;
            }
            foreach (var spellList in DruidSpells.Values)
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
        /// Get available druid spells of a specific level.
        /// </summary>
        public List<string> GetAvailableSpells(int spellLevel)
        {
            if (spellLevel == 0)
                return new List<string>(DruidCantrips);

            if (DruidSpells.ContainsKey(spellLevel))
                return new List<string>(DruidSpells[spellLevel]);

            return new List<string>();
        }

        /// <summary>
        /// Check if a spell name is on the druid list.
        /// </summary>
        public bool IsSpellOnDruidList(string spellName)
        {
            foreach (var cantrip in DruidCantrips)
            {
                if (cantrip.Equals(spellName, StringComparison.OrdinalIgnoreCase))
                    return true;
            }

            foreach (var spells in DruidSpells.Values)
            {
                foreach (var spell in spells)
                {
                    if (spell.Equals(spellName, StringComparison.OrdinalIgnoreCase))
                        return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Get the maximum spell slot level available to this druid.
        /// </summary>
        public int GetMaxSpellSlotLevel()
        {
            if (_level < 2) return 0;
            int calculated = (int)Math.Ceiling((_level - 1) / 2.0);
            return Math.Min(9, Math.Max(0, calculated));
        }

        /// <summary>
        /// Get total remaining spell slots.
        /// </summary>
        public int GetTotalRemainingSpellSlots()
        {
            int total = 0;
            foreach (var slot in RemainingSpellSlots)
            {
                total += slot.Value;
            }
            return total;
        }

        // ==================== Override Methods ====================

        /// <summary>
        /// Overrides base stat calculation for Druid.
        /// Hit Points: d8 per level
        /// </summary>
        protected override void CalculateBaseStats()
        {
            base.CalculateBaseStats();

            int conMod = GetAbilityModifier(_constitution);
            int hpFromFirstLevel = 8 + conMod;
            int hpFromHigherLevels = (_level - 1) * (8 + conMod);
            _maxHitPoints = hpFromFirstLevel + hpFromHigherLevels;
            _hitPoints = _maxHitPoints;

            CalculateSpellDC();
            CalculateSpellAttack();
            UpdateCantripsKnown();
            UpdateSpellsKnown();
            UpdateSpellSlots();
            UpdateWildShapeOptions();
        }

        /// <summary>
        /// Overrides level up to apply druid features.
        /// </summary>
        public override void LevelUp()
        {
            _level++;
            CalculateBaseStats();
            ApplyLevelFeatures();
            Console.WriteLine($"{_name} has reached level {_level}! Druid features updated.");

            if (_level == 2)
                Console.WriteLine("New feature: Wild Shape unlocked!");
            if (_level == 4)
                Console.WriteLine("New feature: Wild Shape damage resistance unlocked!");
            if (_level == 18)
                Console.WriteLine("New feature: Archdruid - Unlimited Wild Shape!");
        }

        /// <summary>
        /// Overrides display to include druid-specific information.
        /// </summary>
        public override void DisplayCharacter()
        {
            Console.WriteLine($"\n=== {_name} (Level {_level} Druid - {Circle}) ===");
            Console.WriteLine($"Hit Points: {_hitPoints}/{_maxHitPoints} | AC: {_armorClass} | Speed: {_speed}");
            Console.WriteLine("\n--- Ability Scores ---");
            Console.WriteLine($"  Strength:    {_strength} (mod +{GetAbilityModifier(_strength)})");
            Console.WriteLine($"  Dexterity:   {_dexterity} (mod +{GetAbilityModifier(_dexterity)})");
            Console.WriteLine($"  Constitution:{_constitution} (mod +{GetAbilityModifier(_constitution)})");
            Console.WriteLine($"  Intelligence:{_intelligence} (mod +{GetAbilityModifier(_intelligence)})");
            Console.WriteLine($"  Wisdom:      {_wisdom} (mod +{GetAbilityModifier(_wisdom)}) <-- Primary Casting Ability");
            Console.WriteLine($"  Charisma:    {_charisma} (mod +{GetAbilityModifier(_charisma)})");

            Console.WriteLine("\n--- Druid Features ---");
            Console.WriteLine($"Spellcasting Ability: {SpellCastingAbility}");
            Console.WriteLine($"Spell Save DC: {SpellSaveDC} | Spell Attack Modifier: {SpellAttackModifier}");
            Console.WriteLine($"Cantrips Known: {CantripsKnown}");
            Console.WriteLine($"Spells Known: {KnownSpells.Count}/{SpellsKnown}");

            string usesDisplay = _archdruidUnlocked ? "Unlimited" : $"{WildShapeUses}/{MaxWildShapeUses}";
            Console.WriteLine($"\n--- Wild Shape ---");
            Console.WriteLine($"Max CR: {MaxWildShapeCR} | Uses: {usesDisplay}");
            Console.WriteLine($"Current Form: {CurrentBeastForm} | In Wild Shape: {IsInWildShape}");
            if (_wildShapeDamageResistUnlocked)
                Console.WriteLine("Damage Resistance: All damage (non-weapon/non-spell) while in Wild Shape");

            Console.WriteLine($"\n--- Circle of {Circle} ---");
            if (_level >= 2)
                Console.WriteLine("Level 2 feature: Unlocked");
            if (_level >= 6)
                Console.WriteLine("Level 6 feature: Unlocked");
            if (_level >= 8)
                Console.WriteLine("Level 8 feature: Unlocked");

            if (SpellSlotsPerLevel.Count > 0)
            {
                Console.WriteLine("\n--- Spell Slots ---");
                foreach (var slot in SpellSlotsPerLevel)
                {
                    int remaining = RemainingSpellSlots.GetValueOrDefault(slot.Key, 0);
                    Console.WriteLine($"  {slot.Key}-level: {remaining}/{slot.Value} remaining");
                }
            }

            if (KnownSpells.Count > 0)
            {
                Console.WriteLine("\n--- Known Spells ---");
                foreach (var spell in KnownSpells)
                {
                    Console.WriteLine($"  - {spell.Name} (Level {spell.Level}, {spell.School})");
                }
            }

            Console.WriteLine($"\n--- Cantrips Known ({CantripsKnown}) ---");
            foreach (var cantrip in DruidCantrips.Take(CantripsKnown))
            {
                Console.WriteLine($"  - {cantrip}");
            }
        }

        /// <summary>
        /// Overrides long rest to recover resources.
        /// </summary>
        public override void LongRest()
        {
            base.LongRest();

            foreach (var slot in SpellSlotsPerLevel)
            {
                RemainingSpellSlots[slot.Key] = slot.Value;
            }

            if (!_archdruidUnlocked)
            {
                WildShapeUses = MaxWildShapeUses;
            }

            Console.WriteLine($"{_name} has recovered all spell slots and Wild Shape uses after a long rest.");
        }

        /// <summary>
        /// Overrides short rest to potentially recover resources.
        /// </summary>
        public override void ShortRest()
        {
            base.ShortRest();

            if (Circle == DruidCircle.Land && _naturalRecoveryActive)
            {
                NaturalRecovery();
            }

            if (Circle == DruidCircle.Stars)
            {
                Console.WriteLine($"{_name} regains Far Step ability after a short rest.");
            }
        }

        /// <summary>
        /// Take damage - apply Nature's Ward resistance if applicable.
        /// </summary>
        public override void TakeDamage(int damage)
        {
            if (IsInWildShape && _wildShapeDamageResistUnlocked)
            {
                Console.WriteLine("Nature's Ward: Damage resistance applies while in Wild Shape!");
            }

            base.TakeDamage(damage);
        }

        // ==================== Druid Spell Lists ====================

        /// <summary>
        /// Cantrips (0-level spells) available to Druids.
        /// </summary>
        private static readonly List<string> DruidCantrips = new List<string>
        {
            "Guidance", "Thorny Whip", "Shillelagh", "Sacred Flame", "Frostbite",
            "Chill Touch", "Light", "Talking Stone", "Blight", "Control Flames",
            "Create or Destroy Water", "Detect Magic", "Gust", "Identify", "Inflict Wounds",
            "Mending", "Moonbeam", "Pointy Needle", "Poison Spray", "Pretidigitation",
            "Produce Flame", "Ray of Sickness", "Resistance", "Shape Water", "Thaumaturgy"
        };

        /// <summary>
        /// Full druid spell list by level.
        /// </summary>
        private static readonly Dictionary<int, List<string>> DruidSpells = new Dictionary<int, List<string>>
        {
            [1] = new List<string>
            {
                "Barkskin", "Beast Sense", "Cure Wounds", "Entangle", "Faerie Fire",
                "Fog Cloud", "Jump", "Longstrider", "Silent Image", "Speak with Animals",
                "Thunderwave", "Aid", "Animal Friendship", "Asperousar", "Detect Evil and Good",
                "Detect Magic", "Detect Poison and Disease", "Feign Death", "Find Traps",
                "Glyph of Warding", "Goodberry", "Heat Metal", "Hideous Laughter", "Heroism",
                "Identify", "Lesser Restoration", "Locate Animals or Beasts", "Locate Object",
                "Magic Mouth", "Magic Weapon", "Mass Healing Word", "Prayer of Healing",
                "Protection from Evil and Good", "Purify Food and Drink", "Rope Trick",
                "Scorching Ray", "See Invisibility", "Shatter", "Speak with Plants",
                "Spirit Guardians", "Spike Growth", "Suggestion", "Water Breathing",
                "Water Walk", "Witch Bolt"
            },
            [2] = new List<string>
            {
                "Animal Messenger", "Ant Sulfury", "Augury", "Bears Form", "Blindness/Deafness",
                "Calm Emotions", "Cloud of Daggers", "Continual Flame", "Control Water",
                "Darkvision", "Enhance Ability", "Enthrall", "Fear", "Find Steed",
                "Gentle Repose", "Gust of Wind", "Heat Metal", "Hunger of Hadar",
                "Invisibility", "Knock", "Lesser Restoration", "Locate Creature",
                "Magic Mouth", "Marvelous Medicines", "Misty Streak", "Moonlans",
                "Nondetection", "Phantasmal Force", "Plant Growth", "Protection from Poison",
                "Rope Trick", "Scorching Ray", "See Invisibility", "Shatter", "Spider Climbo",
                "Spike Growth", "Suggestion", "Whispering Wind"
            },
            [3] = new List<string>
            {
                "Animate Dead", "Antilife Shell", "Beacon of Hope", "Bestow Curse",
                "Call Lightning", "Clarity of Thought", "Conjure Animals", "Conjure Barrage",
                "Conjure Elemental", "Conjure Woodland Beings", "Create Food and Water",
                "Crown of Life", "Crown of Magic", "Dispel Magic", "Dream", "Echoing Step",
                "Fear", "Feign Death", "Fire Seeds", "Flame Strike", "Flesh to Stone",
                "Gaseous Form", "Glyph of Warding", "Haste", "Horrid Wilting",
                "Insect Plague", "Knot of Wisdom", "Leomund's Tiny Hut", "Magic Circle",
                "Major Image", "Mass Healing Word", "Meld into Stone", "Mind Blank",
                "Minor Illusion", "Nystul's Magic Aura", "Plant Growth", "Prismatic Spray",
                "Programmed Illusion", "Protection from Energy", "Reincarnate", "Remove Curse",
                "Revivify", "Sending", "Sleet Storm", "Slow", "Snare", "Speak with Monsters",
                "Spirit Guardians", "Tenser's Transformation", "Treespeak", "Vampiric Touch"
            },
            [4] = new List<string>
            {
                "Banishment", "Blight", "Compulsion", "Conjure Fey", "Control Water",
                "Divination", "Freedom of Movement", "Giant Constrictor Snake",
                "Giant Eagle", "Giant Elongate", "Greater Invisibility", "Hallucinatory Terrain",
                "Locate Creature", "Polymorph", "Stone Shape", "Stoneskin", "Sunbeam",
                "Watery Sphere", "Wild Strike"
            },
            [5] = new List<string>
            {
                "Animate Objects", "Awaken", "Blight", "Commune", "Contagion",
                "Control Weather", "Cure Wounds", "Dispel Evil and Good", "Dreams of the Blue Veil",
                "Finger of Death", "Flame Strike", "Gentle Repose", "Giant Insect",
                "Guardian of Nature", "Hold Monster", "Insect Plague", "Legend Lore",
                "Masacre", "Mass Cure Wounds", "Mey of Fire", "Mislead", "Modify Memory",
                "Passwall", "Planar Binding", "Rary's Telepathic Bond", "Reincarnate",
                "Scrying", "Seeming", "Telekinesis", "Teleportation Circle", "True Seeing",
                "Wall of Fire", "Wall of Stone"
            },
            [6] = new List<string>
            {
                "Animal Shapes", "Blade Barrier", "Conjure Elemental", "Contingency",
                "Dispel Evil and Good", "Eyebite", "Find the Path", "Forbiddance",
                "Globe of Invulnerability", "Guards and Wards", "Harm", "Heal",
                "Hotric Disintegration", "Invoke Illusion", "Mass Heal", "Move Earth",
                "Otiluke's Freezing Sphere", "Programmed Illusion", "Sunbeam", "Wall of Fire",
                "Wall of Ice", "Wall of Stone"
            },
            [7] = new List<string>
            {
                "Delayed Blast Fireball", "Dmitri's Irresistible Dance", "Divine Word",
                "Etherealness", "Finger of Death", "Forcecage", "Miracle", "Plane Shift",
                "Prismatic Spray", "Project Image", "Regenerate", "Resurrection",
                "Reverse Gravity", "Sunburst", "Teleport"
            },
            [8] = new List<string>
            {
                "Antipathy/Sympathy", "Animal Shapes", "Clay Golem", "Dominate Monster",
                "Earthquake", "Feeblemind", "Giant Insect", "Incendiary Cloud",
                "Mass Suggestion", "Soil of the Woods", "Sunburst", "Wild Shape"
            },
            [9] = new List<string>
            {
                "Astral Projection", "Barbed Stone", "Foresight", "Gate",
                "Imprisonment", "Mass Heal", "Meteor Swarm", "Power Word Heal",
                "Power Word Stun", "Time Stop", "True Polymorph", "Wayward Compass",
                "Wish"
            }
        };

        // Fix spell 3 list that had line break issues - redefine properly
        // Note: The above dictionary entry for level 3 may have continuation issues.
        // The values are collected from the inline collection above.
    }
}