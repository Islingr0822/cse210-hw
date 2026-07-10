using System;
using System.Collections.Generic;
using FeatureClass = DnDCharacterManager.Feature.Feature;
using SpellType = DnDCharacterManager.Spell.Spell;
using RaceType = DnDCharacterManager.Race.Race;
using BackgroundType = DnDCharacterManager.Background.Background;

namespace DnDCharacterManager.Character.Classes
{
    /// <summary>
    /// Enum representing the different Ranger subclasses.
    /// </summary>
    public enum RangerSubclass
    {
        Hunter,
        BeastMaster
    }

    /// <summary>
    /// Enum for Hunter subclass specialties.
    /// </summary>
    public enum HunterSubtype
    {
        ColossusSlayer,
        DesertWarrior,
        DreadNetherworld,
        Forester,
        Guardian,
        HordeBreaker
    }

    /// <summary>
    /// Enum for Beast Master bond beast types.
    /// </summary>
    public enum BeastMasterBondBeast
    {
        None,
        Hawk,
        Owl,
        Wolf,
        GiantElk,
        GiantConstrictorSnake,
        Pterafoul
    }

    /// <summary>
    /// Enum for Ranger fighting styles.
    /// </summary>
    public enum RangerFightingStyle
    {
        Archery,
        TwoWeaponCombat
    }

    /// <summary>
    /// Ranger character class with full level 1-20 feature progression and subclass support.
    /// Ranges martial combat prowess with partial spellcasting ability.
    /// </summary>
    public class Ranger : Character
    {
        // ==================== Subclass Properties ====================

        private RangerSubclass _subclass;
        private string _subclassName;
        private HunterSubtype _hunterSubtype;
        private BeastMasterBondBeast _bondBeastType;
        private RangerFightingStyle _fightingStyle;

        // ==================== Favored Enemy & Natural Explorer ====================

        private List<string> _favoredEnemies;
        private int _totalFavoredEnemies;
        private List<string> _favoredTerrains;

        // ==================== Beast Master Properties ====================

        private bool _bondActive;
        private string _bondBeastName;
        private int _beastHitPoints;
        private int _beastArmorClass;
        private bool _beastAlwaysAvailable;

        // ==================== Spellcasting Properties ====================

        private List<SpellType> _preparedSpells;
        private Dictionary<int, int> _spellSlots;
        private int _totalSpellSlots;
        private int _cantripsKnown;
        private int _spellsKnown;
        private bool _spellcasterActive;

        // ==================== Feature Flags ====================

        private bool _extraAttackUnlocked;
        private bool _landStrideUnlocked;
        private bool _vanishInShadowsUnlocked;
        private bool _foeSlayerActive;
        private bool _unseenFighterActive;
        private int _outlawInsightRounds;
        private int _beastAttackDamage;
        private int _soulEaterDamageBonus;

        // ==================== Constructors ====================

        public Ranger() : base()
        {
            _subclass = RangerSubclass.Hunter;
            _subclassName = "Hunter";
            _hunterSubtype = HunterSubtype.ColossusSlayer;
            _bondBeastType = BeastMasterBondBeast.None;
            _fightingStyle = RangerFightingStyle.Archery;

            _favoredEnemies = new List<string> { "Beasts" };
            _totalFavoredEnemies = 1;
            _favoredTerrains = new List<string> { "Forest" };

            _bondActive = false;
            _bondBeastName = "";
            _beastHitPoints = 0;
            _beastArmorClass = 10;
            _beastAlwaysAvailable = false;

            _preparedSpells = new List<SpellType>();
            _spellSlots = new Dictionary<int, int>();
            _totalSpellSlots = 0;
            _cantripsKnown = 2;
            _spellsKnown = 4;
            _spellcasterActive = false;

            _extraAttackUnlocked = false;
            _landStrideUnlocked = false;
            _vanishInShadowsUnlocked = false;
            _foeSlayerActive = false;
            _unseenFighterActive = false;
            _outlawInsightRounds = 0;
            _beastAttackDamage = 0;
            _soulEaterDamageBonus = 0;

            ApplyLevelFeatures();
        }

        public Ranger(string name, int level, RaceType race, BackgroundType background)
            : base(name, level, race, background)
        {
            _subclass = RangerSubclass.Hunter;
            _subclassName = "Hunter";
            _hunterSubtype = HunterSubtype.ColossusSlayer;
            _bondBeastType = BeastMasterBondBeast.None;
            _fightingStyle = RangerFightingStyle.Archery;

            _favoredEnemies = new List<string> { "Beasts" };
            _totalFavoredEnemies = 1;
            _favoredTerrains = new List<string> { "Forest" };

            _bondActive = false;
            _bondBeastName = "";
            _beastHitPoints = 0;
            _beastArmorClass = 10;
            _beastAlwaysAvailable = false;

            _preparedSpells = new List<SpellType>();
            _spellSlots = new Dictionary<int, int>();
            _totalSpellSlots = 0;
            _cantripsKnown = 2;
            _spellsKnown = 4;
            _spellcasterActive = false;

            _extraAttackUnlocked = false;
            _landStrideUnlocked = false;
            _vanishInShadowsUnlocked = false;
            _foeSlayerActive = false;
            _unseenFighterActive = false;
            _outlawInsightRounds = 0;
            _beastAttackDamage = 0;
            _soulEaterDamageBonus = 0;

            ApplyLevelFeatures();
        }

        // ==================== Properties ====================

        public RangerSubclass Subclass { get => _subclass; set => _subclass = value; }
        public string SubclassName { get => _subclassName; set => _subclassName = value; }
        public HunterSubtype HunterSubtypeEnum { get => _hunterSubtype; set => _hunterSubtype = value; }
        public BeastMasterBondBeast BondBeastType { get => _bondBeastType; set => _bondBeastType = value; }
        public RangerFightingStyle FightingStyle { get => _fightingStyle; set => _fightingStyle = value; }

        public List<string> FavoredEnemies { get => _favoredEnemies; }
        public int TotalFavoredEnemies { get => _totalFavoredEnemies; }
        public List<string> FavoredTerrains { get => _favoredTerrains; set => _favoredTerrains = value; }

        public bool BondActive { get => _bondActive; set => _bondActive = value; }
        public string BondBeastName { get => _bondBeastName; set => _bondBeastName = value; }
        public int BeastHitPoints { get => _beastHitPoints; set => _beastHitPoints = value; }
        public int BeastArmorClass { get => _beastArmorClass; set => _beastArmorClass = value; }
        public bool BeastAlwaysAvailable { get => _beastAlwaysAvailable; set => _beastAlwaysAvailable = value; }

        public List<SpellType> PreparedSpells { get => _preparedSpells; }
        public Dictionary<int, int> SpellSlots { get => _spellSlots; }
        public int TotalSpellSlots { get => _totalSpellSlots; }
        public int CantripsKnown { get => _cantripsKnown; set => _cantripsKnown = value; }
        public int SpellsKnown { get => _spellsKnown; set => _spellsKnown = value; }

        public bool ExtraAttackUnlocked { get => _extraAttackUnlocked; }
        public bool LandStrideUnlocked { get => _landStrideUnlocked; }
        public bool VanishInShadowsUnlocked { get => _vanishInShadowsUnlocked; }
        public bool FoeSlayerActive { get => _foeSlayerActive; }

        // ==================== Core Methods ====================

        public override void ClassSpecificAbility()
        {
            HunterMark();
        }

        /// <summary>
        /// Mark a favored enemy. The ranger deals extra damage to the marked creature.
        /// At level 1, deals +1 damage. Increases at higher levels.
        /// </summary>
        public virtual void HunterMark()
        {
            if (_favoredEnemies.Count == 0)
            {
                Console.WriteLine($"{Name} has no favored enemies marked. Use SelectFavoredEnemy first.");
                return;
            }

            int damageBonus = GetHunterMarkDamage();
            Console.WriteLine($"{Name} marks a favored enemy! The next melee or ranged attack against the mark deals +{damageBonus} damage.");
        }

        /// <summary>
        /// Select a creature type as a favored enemy.
        /// </summary>
        public virtual void SelectFavoredEnemy(string creatureType)
        {
            if (_favoredEnemies.Contains(creatureType))
            {
                Console.WriteLine($"{Name} already has {creatureType} as a favored enemy.");
                return;
            }

            if (_totalFavoredEnemies >= 3 && _subclass == RangerSubclass.Hunter)
            {
                Console.WriteLine($"{Name} has reached the maximum number of favored enemies for a Hunter ranger.");
                return;
            }

            _favoredEnemies.Add(creatureType);
            _totalFavoredEnemies++;
            Console.WriteLine($"{Name} selects {creatureType} as a favored enemy. Total favored enemies: {_totalFavoredEnemies}");
        }

        /// <summary>
        /// Select a terrain type as a favored terrain for Natural Explorer.
        /// </summary>
        public virtual void SelectFavoredTerrain(string terrainType)
        {
            if (_favoredTerrains.Contains(terrainType))
            {
                Console.WriteLine($"{Name} already has {terrainType} as a favored terrain.");
                return;
            }

            _favoredTerrains.Add(terrainType);
            Console.WriteLine($"{Name} selects {terrainType} as a favored terrain. Total favored terrains: {_favoredTerrains.Count}");
        }

        // ==================== Stat Calculations ====================

        /// <summary>
        /// Override base stats calculation for Ranger.
        /// Hit Points: 10 + Con mod at level 1, 8 + Con mod at higher levels.
        /// No class-based AC modification (relies on equipment).
        /// </summary>
        protected override void CalculateBaseStats()
        {
            int conMod = Math.Max(-5, (Constitution - 10) / 2);

            // Max HP: 10 + con mod at level 1, 8 + con mod per level after
            int hpFromFirstLevel = 10 + conMod;
            int hpFromHigherLevels = (Level - 1) * (8 + conMod);
            MaxHitPoints = hpFromFirstLevel + hpFromHigherLevels;

            if (HitPoints > MaxHitPoints || HitPoints <= 0)
            {
                HitPoints = MaxHitPoints;
            }

            // Speed from race (rangers have no speed modification)
            Speed = _race != null ? _race.Speed : 30;
        }

        // ==================== Level Feature Progression ====================

        /// <summary>
        /// Apply features gained at specific ranger levels.
        /// </summary>
        private void ApplyLevelFeatures()
        {
            // Initialize spell slots for level 1
            if (!_spellSlots.ContainsKey(1))
            {
                _spellSlots[1] = 2;
            }

            // Level 3: Hunter or Beast Master subclass feature
            if (_subclass == RangerSubclass.Hunter)
            {
                switch (_hunterSubtype)
                {
                    case HunterSubtype.ColossusSlayer:
                        Features.Add(new FeatureClass("Colossus Slayer", "Your type damage against a creature increases by +1 if it is below its max HP."));
                        break;
                    case HunterSubtype.DesertWarrior:
                        Features.Add(new FeatureClass("Desert Warrior", "When you make an attack with your favored weapon, you gain bonus damage to another attack. Immunity to hunger and thirst."));
                        break;
                    case HunterSubtype.DreadNetherworld:
                        Features.Add(new FeatureClass("Dread Netherworld", "Add +1 to the extra damage roll of your favored enemy for each level over 3. Once per day teleport as an action up to 60 feet to an unoccupied space you can see."));
                        _soulEaterDamageBonus = Level - 3;
                        break;
                    case HunterSubtype.Forester:
                        Features.Add(new FeatureClass("Forester", "Your favored weapon deals +2 damage against Tiny or smaller creatures. Immunity to disease, resistance to poison damage."));
                        break;
                    case HunterSubtype.Guardian:
                        Features.Add(new FeatureClass("Guardian", "When you or a creature within 5 feet is hit by an attack, roll a d4. On a hit, the target gains temporary HP equal to 5 + your ranger level/2."));
                        break;
                    case HunterSubtype.HordeBreaker:
                        Features.Add(new FeatureClass("Horde Breaker", "Once per turn, you can make another attack with your favored weapon against a different creature."));
                        break;
                }
            }
            else if (_subclass == RangerSubclass.BeastMaster)
            {
                Features.Add(new FeatureClass("Beast Bond", "You gain a beast companion that fights alongside you."));
            }

            // Level 5: Extra Attack
            if (Level >= 5)
            {
                _extraAttackUnlocked = true;
                Features.Add(new FeatureClass("Extra Attack", "You can attack twice, instead of once, when you take the Attack action on your turn."));
            }

            // Level 6: Favored Enemy increases to 2 types
            if (Level >= 6)
            {
                _totalFavoredEnemies = Math.Max(_totalFavoredEnemies, 2);
            }

            // Level 8: Spellcasting Focus
            if (Level >= 8)
            {
                _spellcasterActive = true;
            }

            // Level 9: Land's Stride
            if (Level >= 9)
            {
                _landStrideUnlocked = true;
            }

            // Level 10: Subclass feature enhancement
            if (Level >= 10 && _subclass == RangerSubclass.Hunter)
            {
                switch (_hunterSubtype)
                {
                    case HunterSubtype.DesertWarrior:
                        _unseenFighterActive = true;
                        break;
                    case HunterSubtype.DreadNetherworld:
                        _soulEaterDamageBonus = Level - 3;
                        break;
                }
            }

            // Level 11: Hunter's Master Strike (for Colossus Slayer)
            if (Level >= 11 && _subclass == RangerSubclass.Hunter && _hunterSubtype == HunterSubtype.ColossusSlayer)
            {
                Features.Add(new FeatureClass("Hunter's Master Strike", "When you hit with an attack, the target takes extra damage. If the target drops to 0 HP, you can make another attack as a bonus action against a different creature."));
            }

            // Level 14: Shadows of Moors (for Dread Netherworld)
            if (Level >= 14 && _subclass == RangerSubclass.Hunter && _hunterSubtype == HunterSubtype.DreadNetherworld)
            {
                Features.Add(new FeatureClass("Shadows of Moors", "When you use your Dimensional Walk feature, you and up to 3 creatures you choose gain immunity to damage from spells for 1 turn."));
            }

            // Level 15: Multiattack (for Beast Master)
            if (Level >= 15 && _subclass == RangerSubclass.BeastMaster)
            {
                Features.Add(new FeatureClass("Multiattack", "When you attack, your beast can make an attack as a bonus action."));
            }

            // Level 18: Favored Enemy increases to 3 types
            if (Level >= 18)
            {
                _totalFavoredEnemies = Math.Max(_totalFavoredEnemies, 3);
            }

            // Level 20: Foe Slayer
            if (Level >= 20)
            {
                _foeSlayerActive = true;
                Features.Add(new FeatureClass("Foe Slayer", "You become the ultimate hunter. Add your Wisdom modifier to weapon attack rolls. Once per turn, you can make an attack against any creature you target with your favored enemy."));
            }

            UpdateSpellSlots();
        }

        /// <summary>
        /// Check and apply level-up features when the ranger levels up.
        /// </summary>
        public void OnLevelUp()
        {
            ApplyLevelFeatures();
            Console.WriteLine($"{Name} gains new features at level {Level}!");
            DisplayLevelFeatures();
        }

        private void DisplayLevelFeatures()
        {
            Console.WriteLine("=== New Features Unlocked ===");

            if (_extraAttackUnlocked)
            {
                Console.WriteLine("- Extra Attack: Attack twice when you take the Attack action.");
            }

            if (_landStrideUnlocked)
            {
                Console.WriteLine("- Land's Stride: Move through difficult terrain without cost. Immunity to plant encasement.");
            }

            if (_foeSlayerActive)
            {
                Console.WriteLine("- Foe Slayer: Add Wisdom modifier to weapon attack rolls. Once per turn extra attack.");
            }

            if (_unseenFighterActive)
            {
                Console.WriteLine("- Vanish in Sand: Hide as a bonus action.");
            }

            DisplaySpellProgression();
        }

        // ==================== Spellcasting (Half-Caster) ====================

        /// <summary>
        /// Update spell slots based on ranger level using the half-caster progression.
        /// </summary>
        private void UpdateSpellSlots()
        {
            int casterLevel = Math.Max(1, Level / 2);
            _spellSlots.Clear();

            switch (casterLevel)
            {
                case 1:
                    _spellSlots[1] = 2;
                    _totalSpellSlots = 2;
                    _cantripsKnown = 2;
                    _spellsKnown = 4;
                    break;
                case 2:
                    _spellSlots[1] = 3;
                    _totalSpellSlots = 3;
                    _cantripsKnown = 2;
                    _spellsKnown = 5;
                    break;
                case 3:
                    _spellSlots[1] = 4;
                    _spellSlots[2] = 2;
                    _totalSpellSlots = 6;
                    _cantripsKnown = 3;
                    _spellsKnown = 6;
                    break;
                case 4:
                    _spellSlots[1] = 4;
                    _spellSlots[2] = 3;
                    _totalSpellSlots = 7;
                    _cantripsKnown = 3;
                    _spellsKnown = 7;
                    break;
                case 5:
                    _spellSlots[1] = 4;
                    _spellSlots[2] = 3;
                    _spellSlots[3] = 3;
                    _totalSpellSlots = 10;
                    _cantripsKnown = 3;
                    _spellsKnown = 8;
                    break;
                case 6:
                    _spellSlots[1] = 4;
                    _spellSlots[2] = 3;
                    _spellSlots[3] = 3;
                    _spellSlots[4] = 1;
                    _totalSpellSlots = 11;
                    _cantripsKnown = 4;
                    _spellsKnown = 9;
                    break;
                case 7:
                    _spellSlots[1] = 4;
                    _spellSlots[2] = 3;
                    _spellSlots[3] = 3;
                    _spellSlots[4] = 3;
                    _totalSpellSlots = 14;
                    _cantripsKnown = 4;
                    _spellsKnown = 10;
                    break;
                case 8:
                    _spellSlots[1] = 4;
                    _spellSlots[2] = 3;
                    _spellSlots[3] = 3;
                    _spellSlots[4] = 3;
                    _spellSlots[5] = 1;
                    _totalSpellSlots = 15;
                    _cantripsKnown = 4;
                    _spellsKnown = 10;
                    break;
                case 9:
                    _spellSlots[1] = 4;
                    _spellSlots[2] = 3;
                    _spellSlots[3] = 3;
                    _spellSlots[4] = 3;
                    _spellSlots[5] = 2;
                    _totalSpellSlots = 17;
                    _cantripsKnown = 4;
                    _spellsKnown = 11;
                    break;
                case 10:
                case 11:
                    _spellSlots[1] = 4;
                    _spellSlots[2] = 3;
                    _spellSlots[3] = 3;
                    _spellSlots[4] = 3;
                    _spellSlots[5] = 2;
                    _spellSlots[6] = 1;
                    _totalSpellSlots = 18;
                    _cantripsKnown = 4;
                    _spellsKnown = _subclass == RangerSubclass.BeastMaster ? 13 : (_spellsKnown < 13 ? 13 : _spellsKnown);
                    break;
                case 12:
                    _spellSlots[1] = 4;
                    _spellSlots[2] = 3;
                    _spellSlots[3] = 3;
                    _spellSlots[4] = 3;
                    _spellSlots[5] = 2;
                    _spellSlots[6] = 1;
                    _totalSpellSlots = 19;
                    _cantripsKnown = 4;
                    _spellsKnown = 14;
                    break;
                case 13:
                case 14:
                    _spellSlots[1] = 4;
                    _spellSlots[2] = 3;
                    _spellSlots[3] = 3;
                    _spellSlots[4] = 3;
                    _spellSlots[5] = 2;
                    _spellSlots[6] = 1;
                    _spellSlots[7] = 1;
                    _totalSpellSlots = 20;
                    _cantripsKnown = 4;
                    _spellsKnown = 15;
                    break;
                case 15:
                    _spellSlots[1] = 4;
                    _spellSlots[2] = 3;
                    _spellSlots[3] = 3;
                    _spellSlots[4] = 3;
                    _spellSlots[5] = 2;
                    _spellSlots[6] = 1;
                    _spellSlots[7] = 1;
                    _spellSlots[8] = 1;
                    _totalSpellSlots = 21;
                    _cantripsKnown = 4;
                    _spellsKnown = 15;
                    break;
                case 16:
                case 17:
                    _spellSlots[1] = 4;
                    _spellSlots[2] = 3;
                    _spellSlots[3] = 3;
                    _spellSlots[4] = 3;
                    _spellSlots[5] = 2;
                    _spellSlots[6] = 1;
                    _spellSlots[7] = 1;
                    _spellSlots[8] = 1;
                    _spellSlots[9] = 1;
                    _totalSpellSlots = 22;
                    _cantripsKnown = 4;
                    _spellsKnown = 17;
                    break;
                case 18:
                    _spellSlots[1] = 4;
                    _spellSlots[2] = 3;
                    _spellSlots[3] = 3;
                    _spellSlots[4] = 3;
                    _spellSlots[5] = 2;
                    _spellSlots[6] = 1;
                    _spellSlots[7] = 1;
                    _spellSlots[8] = 1;
                    _spellSlots[9] = 2;
                    _totalSpellSlots = 24;
                    _cantripsKnown = 4;
                    _spellsKnown = 18;
                    break;
                case 19:
                case 20:
                    _spellSlots[1] = 4;
                    _spellSlots[2] = 3;
                    _spellSlots[3] = 3;
                    _spellSlots[4] = 3;
                    _spellSlots[5] = 2;
                    _spellSlots[6] = 1;
                    _spellSlots[7] = 1;
                    _spellSlots[8] = 1;
                    _spellSlots[9] = 2;
                    _totalSpellSlots = 24;
                    _cantripsKnown = 4;
                    _spellsKnown = 18;
                    break;
                default:
                    _totalSpellSlots = 0;
                    break;
            }
        }

        /// <summary>
        /// Get the spell save DC for ranger spells.
        /// DC = 8 + proficiency bonus + Wisdom modifier
        /// </summary>
        public int GetSpellSaveDC()
        {
            int proficiencyBonus = GetProficiencyBonus();
            int wisdomMod = Math.Max(-5, (Wisdom - 10) / 2);
            return 8 + proficiencyBonus + wisdomMod;
        }

        /// <summary>
        /// Get the proficiency bonus based on level.
        /// </summary>
        private int GetProficiencyBonus()
        {
            if (Level >= 17) return 5;
            if (Level >= 9) return 4;
            if (Level >= 5) return 3;
            return 2;
        }

        /// <summary>
        /// Display spellcasting progression information.
        /// </summary>
        private void DisplaySpellProgression()
        {
            if (_spellcasterActive || Level >= 1)
            {
                Console.WriteLine($"\n--- Spellcasting (Wisdom-based, Half-Caster) ---");
                int casterLevel = Math.Max(1, Level / 2);
                Console.WriteLine($"Spellcaster Level: {casterLevel}");
                Console.WriteLine($"Cantrips Known: {_cantripsKnown}");
                Console.WriteLine($"Spells Known: {_spellsKnown}");
                Console.WriteLine($"Spell Save DC: {GetSpellSaveDC()}");
                Console.WriteLine("Spell Slots:");
                foreach (var slot in _spellSlots)
                {
                    Console.WriteLine($"  {slot.Key}st level: {slot.Value} slot(s)");
                }
            }
        }

        /// <summary>
        /// Prepare a spell from the ranger's known spells.
        /// </summary>
        public virtual void PrepareSpell(SpellType spell)
        {
            if (!_preparedSpells.Contains(spell))
            {
                _preparedSpells.Add(spell);
                Console.WriteLine($"{Name} has prepared the spell: {spell.Name}");
            }
        }

        /// <summary>
        /// Cast a spell using an available spell slot.
        /// </summary>
        public virtual bool CastSpell(SpellType spell, string target)
        {
            if (spell == null)
            {
                Console.WriteLine($"{Name} has no valid spell to cast.");
                return false;
            }

            int spellLevel = spell.Level;
            if (spellLevel == 0)
            {
                Console.WriteLine($"{Name} casts {spell.Name} on {target} (cantrip).");
                return true;
            }

            if (!_spellSlots.ContainsKey(spellLevel) || _spellSlots[spellLevel] <= 0)
            {
                Console.WriteLine($"{Name} doesn't have any {spellLevel}st level spell slots remaining.");
                return false;
            }

            _spellSlots[spellLevel]--;
            _totalSpellSlots--;
            Console.WriteLine($"{Name} casts {spell.Name} on {target} using a {spellLevel}st level spell slot.");
            return true;
        }

        /// <summary>
        /// Recover spell slots during a long rest.
        /// </summary>
        public override void LongRest()
        {
            base.LongRest();
            UpdateSpellSlots();
            Console.WriteLine($"{Name} recovers all spell slots after a long rest.");
        }

        // ==================== Beast Master Methods ====================

        /// <summary>
        /// Form a bond with a beast companion.
        /// </summary>
        public virtual void BondWithBeast(BeastMasterBondBeast beastType, string name)
        {
            if (_subclass != RangerSubclass.BeastMaster)
            {
                Console.WriteLine($"{Name} is not a Beast Master ranger and cannot bond with a beast.");
                return;
            }

            _bondBeastType = beastType;
            _bondBeastName = name;
            _bondActive = true;
            CalculateBeastStats();
            Console.WriteLine($"{Name} forms a bond with a {_bondBeastType} named {_bondBeastName}!");
            DisplayBeastInfo();
        }

        /// <summary>
        /// Calculate beast companion stats based on ranger level.
        /// </summary>
        private void CalculateBeastStats()
        {
            int beastLevel = Math.Min(Math.Max(Level >= 3 ? 3 : Level, 1), 8);
            int conMod = Math.Max(-5, (10 - 10) / 2);
            _beastHitPoints = (5 + conMod) * beastLevel;

            int acBonus = 0;
            switch (_bondBeastType)
            {
                case BeastMasterBondBeast.Wolf: acBonus = 2; break;
                case BeastMasterBondBeast.GiantElk: acBonus = 3; break;
                case BeastMasterBondBeast.GiantConstrictorSnake: acBonus = 1; break;
                case BeastMasterBondBeast.Hawk: acBonus = 1; break;
                case BeastMasterBondBeast.Owl: acBonus = 1; break;
                default: acBonus = 0; break;
            }
            _beastArmorClass = 10 + acBonus;
            _beastAttackDamage = Math.Max(1, (beastLevel / 2) - 1);
        }

        /// <summary>
        /// Issue a command to the beast companion to attack.
        /// </summary>
        public virtual void BeastAttack()
        {
            if (!_bondActive)
            {
                Console.WriteLine($"{Name} has no bonded beast to command.");
                return;
            }

            int totalDamage = _beastAttackDamage + Math.Max(-5, (10 - 10) / 2);
            Console.WriteLine($"{_bondBeastName} attacks the target for {totalDamage} damage!");
        }

        /// <summary>
        /// Display beast companion information.
        /// </summary>
        private void DisplayBeastInfo()
        {
            if (!_bondActive) return;

            Console.WriteLine($"\n--- Beast Companion: {_bondBeastName} ({_bondBeastType}) ---");
            Console.WriteLine($"Hit Points: {_beastHitPoints} | AC: {_beastArmorClass}");
            Console.WriteLine($"Attack Damage: +{_beastAttackDamage}");
            Console.WriteLine("Commands: Help, Attack, Find, Seek, Hide, Disengage, Dodge, Ready");
        }

        /// <summary>
        /// Command the beast to perform an action.
        /// </summary>
        public virtual void CommandBeast(string command)
        {
            if (!_bondActive)
            {
                Console.WriteLine($"{Name} has no bonded beast to command.");
                return;
            }

            switch (command.ToLower())
            {
                case "attack": BeastAttack(); break;
                case "help": Console.WriteLine($"{_bondBeastName} uses the Help action, granting advantage on the next attack against a target."); break;
                case "find": Console.WriteLine($"{_bondBeastName} uses the Find action, searching for threats in the area."); break;
                case "seek": Console.WriteLine($"{_bondBeastName} tracks its prey with keen senses."); break;
                case "hide": Console.WriteLine($"{_bondBeastName} hides and prepares to ambush enemies."); break;
                case "disengage": Console.WriteLine($"{_bondBeastName} disengages from combat without provoking opportunity attacks."); break;
                case "dodge": Console.WriteLine($"{_bondBeastName} focuses on avoiding attacks."); break;
                default: Console.WriteLine($"Unknown command: {command}. Available commands: Attack, Help, Find, Seek, Hide, Disengage, Dodge"); break;
            }
        }

        // ==================== Override Base Methods ====================

        public override void Attack()
        {
            if (_extraAttackUnlocked)
            {
                Console.WriteLine($"{Name} makes two attacks with Extra Attack!");
            }
            else
            {
                Console.WriteLine($"{Name} makes a single attack.");
            }
        }

        public override void TakeDamage(int damage)
        {
            if (_subclass == RangerSubclass.Hunter && _hunterSubtype == HunterSubtype.ColossusSlayer)
            {
                Console.WriteLine($"Colossus Slayer bonus applies! Extra damage dealt.");
            }
            base.TakeDamage(damage);
        }

        private int GetHunterMarkDamage()
        {
            if (_subclass == RangerSubclass.Hunter)
            {
                switch (_hunterSubtype)
                {
                    case HunterSubtype.ColossusSlayer: return Level >= 11 ? 2 : 1;
                    case HunterSubtype.DreadNetherworld: return _soulEaterDamageBonus;
                    case HunterSubtype.Forester: return 2;
                    default: return 1;
                }
            }
            return 1;
        }

        public override void DisplayCharacter()
        {
            Console.WriteLine($"\n{'=', 50}");
            Console.WriteLine($"=== {Name} (Level {_level} Ranger - {_race.Name}) ===");
            Console.WriteLine($"{new string('-', 50)}");
            Console.WriteLine($"Hit Points: {HitPoints}/{MaxHitPoints} | AC: {ArmorClass} | Speed: {Speed}");
            Console.WriteLine($"Subclass: {_subclassName}");
            Console.WriteLine($"Total Favored Enemies: {_totalFavoredEnemies}");

            if (_spellcasterActive || Level >= 1)
            {
                int casterLevel = Math.Max(1, Level / 2);
                Console.WriteLine($"\n--- Spellcasting ---");
                Console.WriteLine($"Spellcaster Level: {casterLevel}");
                Console.WriteLine($"Cantrips Known: {_cantripsKnown}");
                Console.WriteLine($"Spells Known: {_spellsKnown}");
                Console.WriteLine($"Spell Save DC: {GetSpellSaveDC()}");
            }

            if (_subclass == RangerSubclass.BeastMaster && _bondActive)
            {
                DisplayBeastInfo();
            }

            Console.WriteLine("\nAbility Scores:");
            Console.WriteLine($"  Strength:    {Strength} (mod +{Math.Max(-5, (Strength - 10) / 2)})");
            Console.WriteLine($"  Dexterity:   {Dexterity} (mod +{Math.Max(-5, (Dexterity - 10) / 2)})");
            Console.WriteLine($"  Constitution:{Constitution} (mod +{Math.Max(-5, (Constitution - 10) / 2)})");
            Console.WriteLine($"  Intelligence:{Intelligence} (mod +{Math.Max(-5, (Intelligence - 10) / 2)})");
            Console.WriteLine($"  Wisdom:      {Wisdom} (mod +{Math.Max(-5, (Wisdom - 10) / 2)})");
            Console.WriteLine($"  Charisma:    {Charisma} (mod +{Math.Max(-5, (Charisma - 10) / 2)})");

            DisplaySubclassInfo();
            Console.WriteLine("=== End Character Sheet ===\n");
        }

        private void DisplaySubclassInfo()
        {
            Console.WriteLine("\n--- Subclass Features ---");

            if (_subclass == RangerSubclass.Hunter)
            {
                Console.WriteLine($"Hunter ({_hunterSubtype}):");
                switch (_hunterSubtype)
                {
                    case HunterSubtype.ColossusSlayer: Console.WriteLine("  Colossus Slayer: +1 (or +2 at level 11) extra damage against a target below max HP."); break;
                    case HunterSubtype.DesertWarrior: Console.WriteLine("  Desert Warrior: Bonus damage with favored weapon. Immunity to hunger/thirst. Vanish in Sand at level 10."); break;
                    case HunterSubtype.DreadNetherworld: Console.WriteLine($"  Dread Netherworld: Soul Eater deals +{_soulEaterDamageBonus} psychic damage. Dimensional Walk once per day."); break;
                    case HunterSubtype.Forester: Console.WriteLine("  Forester: +2 damage against Tiny creatures. Immunity to disease, resistance to poison."); break;
                    case HunterSubtype.Guardian: Console.WriteLine("  Guardian: When you or ally within 5ft is hit, gain temp HP equal to 5 + ranger level/2."); break;
                    case HunterSubtype.HordeBreaker: Console.WriteLine("  Horde Breaker: Once per turn, make another attack against a different creature."); break;
                }
                if (_extraAttackUnlocked) Console.WriteLine("  Extra Attack: Attack twice when you take the Attack action.");
            }
            else if (_subclass == RangerSubclass.BeastMaster)
            {
                Console.WriteLine($"Beast Master:");
                if (_bondActive)
                {
                    Console.WriteLine($"  Bonded Beast: {_bondBeastName} ({_bondBeastType}) - HP: {_beastHitPoints}, AC: {_beastArmorClass}");
                    Console.WriteLine("  Beast can attack as a bonus action at level 15+.");
                }
                else
                {
                    Console.WriteLine("  No bonded beast. Use BondWithBeast() to form a bond.");
                }
                if (Level >= 15) Console.WriteLine("  Multiattack: Beast can attack as a bonus action.");
            }

            if (_landStrideUnlocked) Console.WriteLine("  Land's Stride: Move through difficult terrain without cost. Immunity to plant encasement.");
            if (_foeSlayerActive) Console.WriteLine("  Foe Slayer: Add Wisdom modifier to weapon attack rolls.");
        }

        /// <summary>
        /// Select a fighting style for the ranger.
        /// </summary>
        public virtual void SelectFightingStyle(RangerFightingStyle style)
        {
            _fightingStyle = style;
            Console.WriteLine($"{Name} selects {style} as their fighting style.");

            switch (style)
            {
                case RangerFightingStyle.Archery:
                    Console.WriteLine("  Benefit: +2 bonus to attack rolls with ranged weapons.");
                    break;
                case RangerFightingStyle.TwoWeaponCombat:
                    Console.WriteLine("  Benefit: When you engage in two-weapon combat, the ability modifier of the master add to damage rolls.");
                    break;
            }
        }

        /// <summary>
        /// Apply fighting style bonuses.
        /// </summary>
        public int GetFightingStyleAttackBonus()
        {
            return _fightingStyle == RangerFightingStyle.Archery ? 2 : 0;
        }

        public int GetFightingStyleDamageBonus()
        {
            return _fightingStyle == RangerFightingStyle.TwoWeaponCombat ? Math.Max(-5, (Dexterity - 10) / 2) : 0;
        }

        /// <summary>
        /// Check if a terrain is a favored terrain for Natural Explorer benefits.
        /// </summary>
        public bool IsFavoredTerrain(string terrainType) => _favoredTerrains.Contains(terrainType);

        /// <summary>
        /// Check if a creature type is a favored enemy.
        /// </summary>
        public bool IsFavoredEnemy(string creatureType) => _favoredEnemies.Contains(creatureType);
    }
}