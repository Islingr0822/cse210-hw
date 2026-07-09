using System;
using System.Collections.Generic;
using SpellType = DnDCharacterManager.Spell.Spell;
using ItemType = DnDCharacterManager.Item.Item;
using RaceType = DnDCharacterManager.Race.Race;
using BackgroundType = DnDCharacterManager.Background.Background;

namespace DnDCharacterManager.Character.Classes
{
    /// <summary>
    /// Artificer character class - a master of magic items and infusions.
    /// </summary>
    public class Artificer : Character
    {
        // ==================== Enums ====================

        /// <summary>
        /// Arcane Traditions available to Artificers at level 3.
        /// </summary>
        public enum ArtificerTradition
        {
            None,
            Alchemist,      // Creates alchemical goods and potent spells
            Artillery,      // Channels arcane fire through infused items
            Armorer,        // Creates magical armor constructs (ARM)
            BattleSmith     // Partners with a steel defender construct
        }

        /// <summary>
        /// Spell casting property for Artificers (uses Intelligence).
        /// </summary>
        public string SpellCastingAbility => "Intelligence";

        // ==================== Properties ====================

        /// <summary>
        /// The chosen Arcane Tradition (unlocked at level 3).
        /// </summary>
        public ArtificerTradition ArcaneTradition { get; set; }

        /// <summary>
        /// Type of specialized tool the Artificer excels with.
        /// </summary>
        public string SpecializedTool { get; set; }

        /// <summary>
        /// Number of infusions the Artificer knows.
        /// </summary>
        public int InfusionsKnown { get; set; }

        /// <summary>
        /// Number of infusion slots available to activate infusions.
        /// </summary>
        public int InfusionSlots { get; set; }

        /// <summary>
        /// Names of spells that have been infused into items.
        /// </summary>
        public List<string> InfusedSpells { get; set; }

        /// <summary>
        /// Spells known by the Artificer from their spellbook.
        /// </summary>
        public List<SpellType> KnownSpells { get; set; }

        /// <summary>
        /// Spell save DC for Artificer spells.
        /// </summary>
        public int SpellSaveDC { get; set; }

        /// <summary>
        /// Spell attack modifier for Artificer spells.
        /// </summary>
        public int SpellAttackModifier { get; set; }

        /// <summary>
        /// Spell slots available by level [0=cantrip, 1=1st level, etc.].
        /// </summary>
        public Dictionary<int, int> SpellSlotsPerLevel { get; set; }

        /// <summary>
        /// Remaining spell slots available for rest.
        /// </summary>
        public Dictionary<int, int> RemainingSpellSlots { get; set; }

        // ==================== Static Spell Lists (References to spells) ====================

        /// <summary>
        /// Cantrips (0-level spells) available to Artificers.
        /// </summary>
        private static readonly List<string> CantripList = new()
        {
            "Acid Spray", "Burning Hands", "Detect Magic", "Faerie Fire", "Fake Life",
            "Guidance", "Healing Word", "Mage Armor", "Magic Missile", "Mending",
            "Protection of Evard", "Shield", "Shatter", "Shocking Grasp", "Truth Seeing"
        };

        /// <summary>
        /// 1st-level spells available to Artificers.
        /// </summary>
        private static readonly List<string> Level1SpellList = new()
        {
            "Aid", "Arcane Lock", "Blindness/Deafness", "Blink", "Calcified Defense",
            "Cure Wounds", "Detect Magic", "Detect Stealth", "Enlarge/Reduce", "Feather Fall",
            "Fog Cloud", "Goodberry", "Hypnotic Pattern", "Infuse Tool", "Lesser Restoration",
            "Longstrider", "Mage Armor", "Magic Missile", "Protection from Evil and Good", "Identify",
            "Thunderwave"
        };

        /// <summary>
        /// 2nd-level spells available to Artificers.
        /// </summary>
        private static readonly List<string> Level2SpellList = new()
        {
            "Alter Self", "Blindness/Deafness", "Blur", "Cloud of Daggers", "Crown of Stars",
            "Darkvision", "Detect Thoughts", "Dolphin's Dance", "Enlarge/Reduce", "Fire Shield",
            "Gaseous Form", "Gust of Wind", "Heat Metal", "Hold Person", "Lightning Arrow",
            "Magic Mouth", "Mind Fog", "Mirror Image", "Misty Step", "Pyrotechnics", "Scorching Ray",
            "See Invisibility", "Shatter", "Shillelagh", "Spike Growth", "Water Breathing", "Web"
        };

        /// <summary>
        /// 3rd-level spells available to Artificers.
        /// </summary>
        private static readonly List<string> Level3SpellList = new()
        {
            "Bestow Curse", "Blink", "Contingency", "Dispel Magic", "Fear",
            "Fireball", "Fly", "Gaseous Form", "Glyph of Warding", "Haste",
            "Hypnotic Pattern", "Lightning Bolt", "Nondetection", "Polymorph", "Project Image",
            "Reverse Gravity", "Slow", "Stinking Cloud", "Tongues", "Vitriolic Sphere"
        };

        /// <summary>
        /// 4th-level spells available to Artificers.
        /// </summary>
        private static readonly List<string> Level4SpellList = new()
        {
            "Arcane Eye", "Blight", "Confusion", "Dimension Door", "Greater Restoration",
            "Ice Storm", "Phantasmal Killer", "Stoneskin", "Wall of Fire", "Mordenkainen's Sword"
        };

        /// <summary>
        /// 5th-level spells available to Artificers.
        /// </summary>
        private static readonly List<string> Level5SpellList = new()
        {
            "Arcane Hand", "Cone of Cold", "Creation", "Dominate Person", "Dream",
            "Geas", "Greater Restoration", "Hold Monster", "Insect Plague", "Legend Lore",
            "Mass Cure Wounds", "Mislead", "Planar Binding", "Scrying", "See Invisibility", "Tenser's Transformation"
        };

        /// <summary>
        /// 6th-level spells available to Artificers.
        /// </summary>
        private static readonly List<string> Level6SpellList = new()
        {
            "Arcene Gate", "Disintegrate", "False Life", "Giant Analogue", "Find the Path",
            "Find Stella", "Gate", "Glyph of Warding", "Hangar's Horde", "Hold Monster",
            "Illusory Dragon", "Investiture of Flame", "Investiture of Ice", "Investiture of Stone",
            "Investiture of Wind", "Otiluke's Freezing Sphere", "Programmed Illusion", "True Seeing"
        };

        /// <summary>
        /// 7th-level spells available to Artificers.
        /// </summary>
        private static readonly List<string> Level7SpellList = new()
        {
            "Archer's Shot", "Far Step", "Farseer", "Forcecage", "Mirage Arcane",
            "Power Word Stun", "Prismatic Spray", "Project Image", "Regenerate", "Renew"
        };

        /// <summary>
        /// 8th-level spells available to Artificers.
        /// </summary>
        private static readonly List<string> Level8SpellList = new()
        {
            "Animal Messenger", "Antipathy/Sympathy", "Clairvoyance", "Dominate Monster", "Enthrall",
            "Glibness", "Heroes' Feast", "Mass Suggestion", "Otiluke's Granting Sphere", "Sunbeam"
        };

        /// <summary>
        /// 9th-level spells available to Artificers.
        /// </summary>
        private static readonly List<string> Level9SpellList = new()
        {
            "Astral Projection", "Bestow Curse", "Foresight", "Gate", "Incendiary Cloud",
            "Masw Suggestion", "Meteor Swarm", "Power Word Heal", "Power Word Kill", "Time Stop", "Whirlwind"
        };

        // ==================== Constructors ====================

        public Artificer() : base()
        {
            InitializeArtificer();
        }

        public Artificer(string name, int level, RaceType race, BackgroundType background) 
            : base(name, level, race, background)
        {
            InitializeArtificer();
        }

        // ==================== Initialization ====================

        /// <summary>
        /// Initializes all Artificer-specific properties and features.
        /// </summary>
        private void InitializeArtificer()
        {
            InfusionsKnown = 0;
            InfusionSlots = 0;
            ArcaneTradition = ArtificerTradition.None;
            SpecializedTool = "Arcane Toolkit";
            InfusedSpells = new List<string>();
            KnownSpells = new List<SpellType>();
            SpellSlotsPerLevel = new Dictionary<int, int>();
            RemainingSpellSlots = new Dictionary<int, int>();

            CalculateSpellDC();
            UpdateSpellSlots();
            ApplyArtificerFeatures();
        }

        /// <summary>
        /// Calculates the spell save DC based on Intelligence and level.
        /// </summary>
        private void CalculateSpellDC()
        {
            int baseDC = 8;
            int abilityModifier = GetAbilityModifier(_intelligence);
            int proficiencyBonus = GetProficiencyBonus();
            SpellSaveDC = baseDC + abilityModifier + proficiencyBonus;
        }

        /// <summary>
        /// Calculates the spell attack modifier.
        /// </summary>
        private void CalculateSpellAttack()
        {
            int abilityModifier = GetAbilityModifier(_intelligence);
            int proficiencyBonus = GetProficiencyBonus();
            SpellAttackModifier = abilityModifier + proficiencyBonus;
        }

        /// <summary>
        /// Updates spell slots based on Artificer level.
        /// </summary>
        private void UpdateSpellSlots()
        {
            SpellSlotsPerLevel.Clear();
            RemainingSpellSlots.Clear();

            // Artificers are half-casters - they get spell slots starting at level 2
            // Following the Paladin/Ranger half-caster progression
            switch (_level)
            {
                case 1:
                    // No spell slots at level 1
                    break;
                case 2:
                    SpellSlotsPerLevel[1] = 2;
                    break;
                case 3:
                case 4:
                    SpellSlotsPerLevel[1] = 3;
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
                    break;
                case 9:
                case 10:
                    SpellSlotsPerLevel[1] = 4;
                    SpellSlotsPerLevel[2] = 3;
                    SpellSlotsPerLevel[3] = 2;
                    break;
                case 11:
                case 12:
                    SpellSlotsPerLevel[1] = 4;
                    SpellSlotsPerLevel[2] = 3;
                    SpellSlotsPerLevel[3] = 3;
                    break;
                case 13:
                case 14:
                    SpellSlotsPerLevel[1] = 4;
                    SpellSlotsPerLevel[2] = 3;
                    SpellSlotsPerLevel[3] = 3;
                    SpellSlotsPerLevel[4] = 1;
                    break;
                case 15:
                case 16:
                    SpellSlotsPerLevel[1] = 4;
                    SpellSlotsPerLevel[2] = 3;
                    SpellSlotsPerLevel[3] = 3;
                    SpellSlotsPerLevel[4] = 2;
                    break;
                case 17:
                case 18:
                    SpellSlotsPerLevel[1] = 4;
                    SpellSlotsPerLevel[2] = 3;
                    SpellSlotsPerLevel[3] = 3;
                    SpellSlotsPerLevel[4] = 3;
                    SpellSlotsPerLevel[5] = 1;
                    break;
                case 19:
                case 20:
                    SpellSlotsPerLevel[1] = 4;
                    SpellSlotsPerLevel[2] = 3;
                    SpellSlotsPerLevel[3] = 3;
                    SpellSlotsPerLevel[4] = 3;
                    SpellSlotsPerLevel[5] = 2;
                    break;
            }

            // Copy to remaining slots
            foreach (var slot in SpellSlotsPerLevel)
            {
                RemainingSpellSlots[slot.Key] = slot.Value;
            }
        }

        /// <summary>
        /// Applies Artificer features based on current level.
        /// </summary>
        private void ApplyArtificerFeatures()
        {
            // Level 1: Infuse Items - starts with 0 infusions known, gets more at higher levels
            UpdateInfusionSlots();

            // Level 2: Expertise - choose two crafting tools for double proficiency
            if (_level >= 2)
            {
                Console.WriteLine($"{_name} gains Expertise with two crafting tools.");
            }

            // Level 3: Arcane Tradition selection
            if (_level >= 3 && ArcaneTradition == ArtificerTradition.None)
            {
                Console.WriteLine($"{_name} reaches level 3 and can choose an Arcane Tradition.");
            }

            // Recalculate abilities with updated level
            CalculateSpellDC();
            CalculateSpellAttack();
        }

        /// <summary>
        /// Updates infusion slots and known infusions based on level.
        /// </summary>
        private void UpdateInfusionSlots()
        {
            switch (_level)
            {
                case 1:
                    InfusionsKnown = 2;
                    InfusionSlots = 1;
                    break;
                case 2:
                    InfusionsKnown = 3;
                    InfusionSlots = 1;
                    break;
                case 3:
                case 4:
                    InfusionsKnown = 4;
                    InfusionSlots = 2;
                    break;
                case 5:
                case 6:
                    InfusionsKnown = 5;
                    InfusionSlots = 3;
                    break;
                case 7:
                case 8:
                    InfusionsKnown = 6;
                    InfusionSlots = 4;
                    break;
                case 9:
                case 10:
                    InfusionsKnown = 7;
                    InfusionSlots = 4;
                    break;
                case 11:
                case 12:
                    InfusionsKnown = 8;
                    InfusionSlots = 4;
                    break;
                case 13:
                case 14:
                    InfusionsKnown = 9;
                    InfusionSlots = 4;
                    break;
                case 15:
                case 16:
                    InfusionsKnown = 10;
                    InfusionSlots = 4;
                    break;
                case 17:
                case 18:
                    InfusionsKnown = 11;
                    InfusionSlots = 4;
                    break;
                case 19:
                case 20:
                    InfusionsKnown = 12;
                    InfusionSlots = 4;
                    break;
            }
        }

        // ==================== Helper Methods ====================

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

        // ================= Class-Specific Methods ====================

        /// <summary>
        /// Executes the class-specific ability of the Artificer.
        /// </summary>
        public override void ClassSpecificAbility()
        {
            InfuseItem();
        }

        /// <summary>
        /// Infuses an object with one of the Artificer's infusions.
        /// </summary>
        public virtual void InfuseItem()
        {
            if (InfusionSlots <= 0)
            {
                Console.WriteLine("No infusion slots available.");
                return;
            }

            if (InfusedSpells.Count >= InfusionSlots)
            {
                Console.WriteLine("All infusion slots are currently occupied.");
                return;
            }

            Console.WriteLine("Artificer infuses an item with magic.");
        }

        /// <summary>
        /// Creates a new magic item using Artificer craftsmanship.
        /// </summary>
        public virtual void CreateMagicItem(ItemType item, SpellType spell)
        {
            if (item == null)
            {
                Console.WriteLine("Cannot create a magic item without a base object.");
                return;
            }

            if (spell == null)
            {
                Console.WriteLine("Cannot create a magic item without a spell to infuse.");
                return;
            }

            Console.WriteLine($"Artificer crafts a new magic item: {item?.Name} infused with {spell?.Name}.");
        }

        // ==================== Infusion System ====================

        /// <summary>
        /// Learns a new infusion (spell that can be applied to items).
        /// </summary>
        public virtual void LearnInfusion(string spellName)
        {
            if (InfusedSpells.Contains(spellName))
            {
                Console.WriteLine($"{_name} already knows this infusion: {spellName}.");
                return;
            }

            if (InfusedSpells.Count >= InfusionsKnown)
            {
                Console.WriteLine($"Cannot learn more infusions. Maximum is {InfusionsKnown}.");
                return;
            }

            // Verify the spell is a valid Artificer infusion
            bool isValidInfusion = CanTrySpell(spellName);
            if (!isValidInfusion)
            {
                Console.WriteLine($"{spellName} is not a valid Artificer infusion.");
                return;
            }

            InfusedSpells.Add(spellName);
            Console.WriteLine($"{_name} has learned the infusion: {spellName}");
        }

        /// <summary>
        /// Forgets an infusion.
        /// </summary>
        public virtual void ForgetInfusion(string spellName)
        {
            if (InfusedSpells.Remove(spellName))
            {
                Console.WriteLine($"{_name} has forgotten the infusion: {spellName}");
            }
            else
            {
                Console.WriteLine($"{_name} does not know the infusion: {spellName}");
            }
        }

        /// <summary>
        /// Activates an infusion on a target item.
        /// </summary>
        public virtual bool ActivateInfusion(string spellName, ItemType targetItem)
        {
            if (!InfusedSpells.Contains(spellName))
            {
                Console.WriteLine($"{_name} does not know the infusion: {spellName}");
                return false;
            }

            if (RemainingSpellSlots.GetValueOrDefault(0, 0) <= 0)
            {
                Console.WriteLine("No spell slots remaining to activate infusions.");
                return false;
            }

            if (InfusedSpells.Count >= InfusionSlots)
            {
                Console.WriteLine("All infusion slots are occupied.");
                return false;
            }

            string itemName = targetItem?.Name ?? "the item";
            Console.WriteLine($"{_name} activates the infusion {spellName} on {itemName}.");
            int currentLevel0Slots = RemainingSpellSlots.GetValueOrDefault(0, 0);
            RemainingSpellSlots[0] = Math.Max(0, currentLevel0Slots - 1);
            return true;
        }

        /// <summary>
        /// Displays all known infusions.
        /// </summary>
        public virtual void DisplayInfusions()
        {
            Console.WriteLine($"=== {_name}'s Known Infusions ({InfusedSpells.Count}/{InfusionsKnown}) ===");
            Console.WriteLine($"Infusion Slots: {InfusedSpells.Count}/{InfusionSlots}");

            if (InfusedSpells.Count == 0)
            {
                Console.WriteLine("No infusions known.");
            }
            else
            {
                foreach (var infusion in InfusedSpells)
                {
                    Console.WriteLine($"  - {infusion}");
                }
            }
        }

        // ==================== Spell Management ====================

        /// <summary>
        /// Checks if a spell is available to the Artificer spell list.
        /// </summary>
        public bool CanTrySpell(string spellName)
        {
            foreach (var spellList in new[] { CantripList, Level1SpellList, Level2SpellList, Level3SpellList,
                                               Level4SpellList, Level5SpellList, Level6SpellList, Level7SpellList,
                                               Level8SpellList, Level9SpellList })
            {
                if (spellList.Contains(spellName))
                    return true;
            }
            return false;
        }

        /// <summary>
        /// Gets available spells of a specific level for the Artificer.
        /// </summary>
        public List<string> GetAvailableSpells(int spellLevel)
        {
            return spellLevel switch
            {
                0 => new List<string>(CantripList),
                1 => new List<string>(Level1SpellList),
                2 => new List<string>(Level2SpellList),
                3 => new List<string>(Level3SpellList),
                4 => new List<string>(Level4SpellList),
                5 => new List<string>(Level5SpellList),
                6 => new List<string>(Level6SpellList),
                7 => new List<string>(Level7SpellList),
                8 => new List<string>(Level8SpellList),
                9 => new List<string>(Level9SpellList),
                _ => new List<string>()
            };
        }

        /// <summary>
        /// Gets the level of a spell by name from available Artificer spells.
        /// </summary>
        public int GetSpellLevel(string spellName)
        {
            if (CantripList.Contains(spellName)) return 0;
            if (Level1SpellList.Contains(spellName)) return 1;
            if (Level2SpellList.Contains(spellName)) return 2;
            if (Level3SpellList.Contains(spellName)) return 3;
            if (Level4SpellList.Contains(spellName)) return 4;
            if (Level5SpellList.Contains(spellName)) return 5;
            if (Level6SpellList.Contains(spellName)) return 6;
            if (Level7SpellList.Contains(spellName)) return 7;
            if (Level8SpellList.Contains(spellName)) return 8;
            if (Level9SpellList.Contains(spellName)) return 9;
            return -1;
        }

        /// <summary>
        /// Learns a new spell for the Artificer's repertoire.
        /// </summary>
        public virtual void LearnSpell(SpellType spell)
        {
            if (spell == null) return;

            if (!KnownSpells.Contains(spell))
            {
                KnownSpells.Add(spell);
                Console.WriteLine($"{_name} has learned the spell: {spell.Name}");
            }
            else
            {
                Console.WriteLine($"{_name} already knows the spell: {spell.Name}");
            }
        }

        /// <summary>
        /// Forgets a spell from the Artificer's repertoire.
        /// </summary>
        public virtual void ForgetSpell(string spellName)
        {
            SpellType? spellToForget = KnownSpells.Find(s => s.Name == spellName);
            if (spellToForget != null)
            {
                KnownSpells.Remove(spellToForget);
                Console.WriteLine($"{_name} has forgotten the spell: {spellName}");
            }
            else
            {
                Console.WriteLine($"{_name} does not know the spell: {spellName}");
            }
        }

        /// <summary>
        /// Prepares a spell from the Artificer's spellbook for casting.
        /// </summary>
        public virtual void PrepareSpell(string spellName)
        {
            SpellType? spell = KnownSpells.Find(s => s.Name == spellName);
            if (spell != null)
            {
                Console.WriteLine($"{_name} has prepared the spell: {spellName}");
            }
            else
            {
                Console.WriteLine($"{_name} does not have {spellName} in their spellbook.");
            }
        }

        /// <summary>
        /// Casts a known spell if slots are available.
        /// </summary>
        public virtual bool CastSpell(string spellName)
        {
            SpellType? spell = KnownSpells.Find(s => s.Name == spellName);
            if (spell == null)
            {
                Console.WriteLine($"{_name} does not know the spell: {spellName}");
                return false;
            }

            int spellLevel = spell is null ? 0 : spell.Level;
            if (spellLevel < 1 || RemainingSpellSlots.GetValueOrDefault(spellLevel, 0) <= 0)
            {
                Console.WriteLine($"No spell slots remaining for {spellLevel}-level spells.");
                return false;
            }

            RemainingSpellSlots[spellLevel]--;
            spell.Cast();
            return true;
        }

        /// <summary>
        /// Displays all known spells.
        /// </summary>
        public virtual void DisplayKnownSpells()
        {
            Console.WriteLine($"=== {_name}'s Known Spells ({KnownSpells.Count}) ===");

            // Group spells by level for display
            var spellsByLevel = new Dictionary<int, List<SpellType>>();
            foreach (var spell in KnownSpells)
            {
                if (!spellsByLevel.ContainsKey(spell.Level))
                    spellsByLevel[spell.Level] = new List<SpellType>();
                spellsByLevel[spell.Level].Add(spell);
            }

            foreach (var levelGroup in spellsByLevel)
            {
                string levelLabel = levelGroup.Key == 0 ? "Cantrips" : $"{levelGroup.Key}-Level Spells";
                Console.WriteLine($"--- {levelLabel} ---");
                foreach (var spell in levelGroup.Value)
                {
                    Console.WriteLine($"  - {spell.Name} ({spell.School})");
                }
            }

            if (KnownSpells.Count == 0)
            {
                Console.WriteLine("No spells known.");
            }
        }

        // ==================== Tradition-Specific Abilities ====================

        /// <summary>
        /// Applies the ability of the chosen Arcane Tradition.
        /// </summary>
        public virtual void ApplyTraditionAbility()
        {
            switch (ArcaneTradition)
            {
                case ArtificerTradition.Alchemist:
                    Console.WriteLine($"{_name} uses Alchemical Mastery to create temporary HP potions.");
                    break;
                case ArtificerTradition.Artillery:
                    Console.WriteLine($"{_name} channels arcane fire through their focus.");
                    break;
                case ArtificerTradition.Armorer:
                    Console.WriteLine($"{_name} activates their ARM construct's abilities.");
                    break;
                case ArtificerTradition.BattleSmith:
                    Console.WriteLine($"{_name} commands their Steel Defender in battle.");
                    break;
                default:
                    Console.WriteLine($"{_name} has not chosen an Arcane Tradition.");
                    break;
            }
        }

        // ==================== Override Methods ====================

        /// <summary>
        /// Overrides base stat calculation with Artificer-specific modifications.
        /// </summary>
        protected override void CalculateBaseStats()
        {
            base.CalculateBaseStats();

            // Update spell DC and attack modifier on level change
            CalculateSpellDC();
            CalculateSpellAttack();
            UpdateSpellSlots();
            UpdateInfusionSlots();
            ApplyArtificerFeatures();

            // Calculate hit points (d6 hit die for artificer)
            int conModifier = GetAbilityModifier(_constitution);
            _maxHitPoints = 8 + conModifier; // Level 1
            for (int i = 2; i <= _level; i++)
            {
                _maxHitPoints += 4 + conModifier; // Average of d6 is 4
            }
            _hitPoints = _maxHitPoints;
        }

        /// <summary>
        /// Overrides level up to apply Artificer features.
        /// </summary>
        public override void LevelUp()
        {
            _level++;
            CalculateBaseStats();
            Console.WriteLine($"{_name} has reached level {_level}! Artificer features updated.");
        }

        /// <summary>
        /// Overrides display to include Artificer-specific information.
        /// </summary>
        public override void DisplayCharacter()
        {
            Console.WriteLine($"=== {_name} (Level {_level} Artificer) ===");
            Console.WriteLine($"Hit Points: {_hitPoints}/{_maxHitPoints} | AC: {_armorClass} | Speed: {_speed}");
            Console.WriteLine("Ability Scores:");
            Console.WriteLine($"  Strength: {_strength} | Dexterity: {_dexterity} | Constitution: {_constitution}");
            Console.WriteLine($"  Intelligence: {_intelligence} | Wisdom: {_wisdom} | Charisma: {_charisma}");

            // Artificer-specific display
            Console.WriteLine("\n--- Artificer Features ---");
            Console.WriteLine($"Spell Casting Ability: {SpellCastingAbility}");
            Console.WriteLine($"Spell Save DC: {SpellSaveDC} | Spell Attack Modifier: {SpellAttackModifier}");
            Console.WriteLine($"Infusions Known: {InfusedSpells.Count}/{InfusionsKnown} | Infusion Slots: {InfusedSpells.Count}/{InfusionSlots}");
            Console.WriteLine($"Arcane Tradition: {ArcaneTradition}");
            Console.WriteLine($"Specialized Tool: {SpecializedTool}");

            // Spell slots display
            if (SpellSlotsPerLevel.Count > 0)
            {
                Console.WriteLine("\n--- Spell Slots ---");
                foreach (var slot in SpellSlotsPerLevel)
                {
                    string levelLabel = slot.Key == 0 ? "Cantrip" : $"{slot.Key}-Level";
                    Console.WriteLine($"  {levelLabel}: {RemainingSpellSlots.GetValueOrDefault(slot.Key, 0)}/{slot.Value} remaining");
                }
            }

            // Spellbook display
            if (KnownSpells.Count > 0)
            {
                Console.WriteLine("\n--- Known Spells ---");
                foreach (var spell in KnownSpells)
                {
                    Console.WriteLine($"  - {spell.Name} (Level {spell.Level}, {spell.School})");
                }
            }

            // Infusions display
            if (InfusedSpells.Count > 0)
            {
                Console.WriteLine("\n--- Infusions ---");
                foreach (var infusion in InfusedSpells)
                {
                    Console.WriteLine($"  - {infusion}");
                }
            }
        }

        /// <summary>
        /// Overrides long rest to recover spell slots and infusion resources.
        /// </summary>
        public override void LongRest()
        {
            base.LongRest();

            // Recover all spell slots
            foreach (var slot in SpellSlotsPerLevel)
            {
                RemainingSpellSlots[slot.Key] = slot.Value;
            }
            Console.WriteLine($"{_name} has recovered all spell slots during the long rest.");
        }

        /// <summary>
        /// Overrides short rest to recover infusion slots.
        /// </summary>
        public override void ShortRest()
        {
            base.ShortRest();

            // Artificers can recover some spell slots on a short rest
            int slotsToRecover = _level >= 20 ? 2 : (_level >= 10 ? 1 : 0);
            if (slotsToRecover > 0)
            {
                // Recover lowest level available slot
                var sortedSlots = new List<KeyValuePair<int, int>>(RemainingSpellSlots);
                sortedSlots.Sort((a, b) => a.Key.CompareTo(b.Key));
                foreach (var slot in sortedSlots)
                {
                    if (slot.Key == 0 && SpellSlotsPerLevel.ContainsKey(slot.Key)) continue; // Skip cantrips
                    int currentValue = RemainingSpellSlots[slot.Key];
                    if (currentValue < SpellSlotsPerLevel[slot.Key])
                    {
                        RemainingSpellSlots[slot.Key] = currentValue + 1;
                        slotsToRecover--;
                        if (slotsToRecover <= 0) break;
                    }
                }
                Console.WriteLine($"{_name} has recovered {Math.Min(slotsToRecover > 0 ? 0 : 1, slotsToRecover)} spell slot(s) during the short rest.");
            }
        }
    }
}