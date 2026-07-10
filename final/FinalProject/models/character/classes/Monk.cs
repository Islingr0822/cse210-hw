using System;
using System.Collections.Generic;
using DnDCharacterManager.Ability;
using FeatureClass = DnDCharacterManager.Feature.Feature;
using DnDRace = DnDCharacterManager.Race.Race;
using DnDBackground = DnDCharacterManager.Background.Background;

namespace DnDCharacterManager.Character.Classes
{
    /// <summary>
    /// Enum representing the different Monastic Traditions for Monks.
    /// </summary>
    public enum MonkSubclass
    {
        WayOfTheOpenHand,
        WayOfShadow,
        WayOfTheFourElements,
        WayOfTheLongDeath
    }

    /// <summary>
    /// Enum for Four Elements disciplines available to Monks.
    /// </summary>
    public enum ElementalDiscipline
    {
        None,
        MarkOfTheHuntingWolf,
        MarkOfTheBeast,
        MarkOfTheStorm,
        MarkOfTheKien,
        MarkOfTheRipplingWave,
        MarkOfTheBlazingBear
    }

    /// <summary>
    /// Enum for Water Discipline options.
    /// </summary>
    public enum WaterDiscipline
    {
        None,
        ControlWater,
        CreateOrDestroyWater,
        FogBank
    }

    /// <summary>
    /// Monk character class with full level 1-20 feature progression and all PHB traditions.
    /// Implements D&D 5e rules by the book (PHB p.78-83).
    /// </summary>
    public class Monk : Character
    {
        // ==================== Core Monk Properties ====================

        private int _kiPoints;
        private int _maxKiPoints;
        private int _martialArtsDie;
        private bool _unarmoredDefenseActive;
        private bool _unarmoredMovementUnlocked;
        private bool _slowFallUnlocked;
        private bool _deflectMissilesUnlocked;
        private bool _evasionUnlocked;
        private bool _stunningStrikeMasteryUnlocked;
        private int _unarmoredMovementBonus;
        private bool _emptyBodyUnlocked;
        private bool _diamondSoulUnlocked;
        private bool _timelessBodyUnlocked;
        private bool _bodyOfTheElementsUnlocked;
        private bool _ascendedStateUnlocked;

        // Subclass tracking
        private MonkSubclass _subclass;
        private string _subclassName;

        // Four Elements specific
        private ElementalDiscipline _activeDiscipline;
        private List<ElementalDiscipline> _knownDisciplines;
        private WaterDiscipline _waterDisciplineChoice;
        private bool _elementalAdeptUsed;
        private const int MaxDisciplinesKnown = 3;

        // Shadow specific
        private bool _shadowArtsUnlocked;
        private bool _shadowyDodgeUnlocked;
        private bool _stepOfTheMoonUnlocked;
        private List<string> _shadowSpellsKnown;

        // Open Hand specific
        private bool _openHandTechniqueUnlocked;
        private bool _quiveringPalmAvailable;
        private int _quiveringPalmTargetConstitution;
        private bool _quiveringPalmActive;

        // Long Death specific
        private int _malignTransfigurationDieSize;
        private bool _survivorOfDeathUnlocked;

        // Feature flags based on level
        private bool _patientDefenseUnlocked;
        private bool _stepOfTheWindUnlocked;
        private bool _fastHandsUnlocked;
        private bool _wholenessOfBodyUnlocked;
        private bool _ashenPillarUnlocked;
        private bool _resistElementalDisciplineUnlocked;
        private bool _lawOfBeginningsUnlocked;
        private bool _emptyBodyStealthUnlocked;
        private bool _tongueOfTheSunAndMoonUnlocked;

        // ==================== Constructors ====================

        public Monk() : base()
        {
            InitializeMonk();
        }

        public Monk(string name, int level, DnDRace race, DnDBackground background)
            : base(name, level, race, background)
        {
            InitializeMonk();
            ApplyLevelFeatures();
        }

        private void InitializeMonk()
        {
            // Core monk defaults
            _kiPoints = 2;
            _maxKiPoints = 2;
            _martialArtsDie = 4; // d4 at level 1
            _unarmoredDefenseActive = true;
            _unarmoredMovementUnlocked = false;
            _unarmoredMovementBonus = 0;
            _slowFallUnlocked = false;
            _deflectMissilesUnlocked = false;
            _evasionUnlocked = false;
            _stunningStrikeMasteryUnlocked = false;

            // Subclass defaults
            _subclass = MonkSubclass.WayOfTheOpenHand;
            _subclassName = "Way of the Open Hand";

            // Four Elements defaults
            _activeDiscipline = ElementalDiscipline.None;
            _knownDisciplines = new List<ElementalDiscipline>();
            _waterDisciplineChoice = WaterDiscipline.None;
            _elementalAdeptUsed = false;

            // Shadow defaults
            _shadowArtsUnlocked = false;
            _shadowyDodgeUnlocked = false;
            _stepOfTheMoonUnlocked = false;
            _shadowSpellsKnown = new List<string>();

            // Open Hand defaults
            _openHandTechniqueUnlocked = false;
            _quiveringPalmAvailable = false;
            _quiveringPalmTargetConstitution = 0;
            _quiveringPalmActive = false;

            // Long Death defaults
            _malignTransfigurationDieSize = 4;
            _survivorOfDeathUnlocked = false;

            // Feature flags
            _patientDefenseUnlocked = false;
            _stepOfTheWindUnlocked = false;
            _fastHandsUnlocked = false;
            _wholenessOfBodyUnlocked = false;
            _ashenPillarUnlocked = false;
            _resistElementalDisciplineUnlocked = false;
            _lawOfBeginningsUnlocked = false;
            _emptyBodyUnlocked = false;
            _emptyBodyStealthUnlocked = false;
            _tongueOfTheSunAndMoonUnlocked = false;
        }

        // ==================== Properties ====================

        public int KiPoints { get => _kiPoints; set => _kiPoints = value; }
        public int MaxKiPoints { get => _maxKiPoints; set => _maxKiPoints = value; }
        public int MartialArtsDie { get => _martialArtsDie; }
        public bool UnarmoredDefenseActive { get => _unarmoredDefenseActive; }
        public bool UnarmoredMovementUnlocked { get => _unarmoredMovementUnlocked; }
        public int UnarmoredMovementBonus { get => _unarmoredMovementBonus; }
        public bool SlowFallAvailable { get => _slowFallUnlocked; }
        public bool DeflectMissilesAvailable { get => _deflectMissilesUnlocked; }
        public MonkSubclass Subclass { get => _subclass; set { _subclass = value; _subclassName = GetSubclassName(value); } }
        public string SubclassName { get => _subclassName; set => _subclassName = value; }
        public bool EvasionAvailable { get => _evasionUnlocked; }
        public bool StunningStrikeMasteryAvailable { get => _stunningStrikeMasteryUnlocked; }

        public ElementalDiscipline ActiveDiscipline { get => _activeDiscipline; set => _activeDiscipline = value; }
        public List<ElementalDiscipline> KnownDisciplines { get => _knownDisciplines; }
        public WaterDiscipline WaterDisciplineChoice { get => _waterDisciplineChoice; set => _waterDisciplineChoice = value; }

        public bool ShadowArtsUnlocked { get => _shadowArtsUnlocked; }
        public bool ShadowyDodgeAvailable { get => _shadowyDodgeUnlocked; }
        public List<string> ShadowSpellsKnown { get => _shadowSpellsKnown; }

        public bool QuiveringPalmAvailable { get => _quiveringPalmAvailable; }
        public int QuiveringPalmDieSize { get => _malignTransfigurationDieSize; }

        // ==================== Core Methods ====================

        public override void ClassSpecificAbility()
        {
            FlurryOfBlows();
        }

        /// <summary>
        /// Spend ki points to make two unarmed strikes as a bonus action.
        /// </summary>
        public virtual bool FlurryOfBlows()
        {
            if (_kiPoints < 1)
            {
                Console.WriteLine($"{Name} has no ki points remaining.");
                return false;
            }

            _kiPoints--;
            int unarmedDamage = GetMartialArtsDieValue();
            int wisMod = Math.Max(-5, (Wisdom - 10) / 2);
            int totalDamage = unarmedDamage + wisMod;

            Console.WriteLine($"{Name} spends 1 ki point to unleash Flurry of Blows!");
            Console.WriteLine($"{Name} makes two unarmed strikes, each dealing {unarmedDamage} + {wisMod} ({totalDamage}) damage.");
            return true;
        }

        /// <summary>
        /// Patient Defense: Spend 1 ki to take the Dodge action as a bonus action.
        /// </summary>
        public virtual bool PatientDefense()
        {
            if (_kiPoints < 1 || !_patientDefenseUnlocked)
            {
                Console.WriteLine($"{Name} cannot use Patient Defense (requires level 3 or no ki points).");
                return false;
            }

            _kiPoints--;
            Console.WriteLine($"{Name} spends 1 ki point to use Patient Defense! Dodge as a bonus action.");
            return true;
        }

        /// <summary>
        /// Step of the Wind: Spend 1 ki to jump higher and climb faster.
        /// Also allows disengaging as a bonus action.
        /// </summary>
        public virtual bool StepOfTheWind()
        {
            if (_kiPoints < 1 || !_stepOfTheWindUnlocked)
            {
                Console.WriteLine($"{Name} cannot use Step of the Wind (requires level 3 or no ki points).");
                return false;
            }

            _kiPoints--;
            int jumpDistance = Speed; // Double jump distance
            Console.WriteLine($"{Name} spends 1 ki point to use Step of the Wind!");
            Console.WriteLine($"{Name} can jump {jumpDistance / 2} feet vertically and climb at double speed this turn.");
            Console.WriteLine($"{Name} can also disengage as a bonus action.");
            return true;
        }

        /// <summary>
        /// Stunning Strike: When you hit a creature with a melee weapon attack, the target must make a CON save.
        /// On a failure, the target is stunned until the end of your next turn.
        /// Costs 2 ki points.
        /// </summary>
        public virtual bool StunningStrike(int targetAC)
        {
            if (_kiPoints < 2)
            {
                Console.WriteLine($"{Name} needs at least 2 ki points for Stunning Strike.");
                return false;
            }

            _kiPoints -= 2;
            int saveDC = GetKiSaveDC();
            Console.WriteLine($"{Name} spends 2 ki points to attempt a Stunning Strike!");
            Console.WriteLine($"Target must make a Constitution saving throw (DC {saveDC}) or be stunned until the end of your next turn.");
            return true;
        }

        /// <summary>
        /// Deflect Missiles: When hit by a ranged weapon attack, reduce damage and potentially catch the missile.
        /// </summary>
        public virtual int DeflectMissiles(int damage)
        {
            if (!_deflectMissilesUnlocked)
            {
                return damage;
            }

            int dexMod = Math.Max(-5, (Dexterity - 10) / 2);
            int reducedDamage = Math.Max(0, damage - 10 - dexMod);

            if (reducedDamage == 0)
            {
                Console.WriteLine($"{Name} deflects the missile completely!");
            }
            else
            {
                Console.WriteLine($"{Name} deflects the missile, reducing damage from {damage} to {reducedDamage}.");
            }

            return reducedDamage;
        }

        /// <summary>
        /// Slow Fall: When you fall, reduce your falling damage by 5 x monk level per ki point spent.
        /// </summary>
        public virtual bool SlowFall(int fallingDamage)
        {
            if (!_slowFallUnlocked || _kiPoints < 1)
            {
                return false;
            }

            _kiPoints--;
            int reduction = _maxKiPoints * 5;
            int reducedDamage = Math.Max(0, fallingDamage - reduction);

            Console.WriteLine($"{Name} spends 1 ki point to slow their fall!");
            Console.WriteLine($"Falling damage reduced from {fallingDamage} to {reducedDamage}.");
            return true;
        }

        /// <summary>
        /// Evasion: When subjected to an effect that allows a DEX save, take no damage on a success and half on a failure.
        /// </summary>
        public virtual int ApplyEvasion(int damage, bool saveSuccessful)
        {
            if (!_evasionUnlocked)
            {
                return damage;
            }

            if (saveSuccessful)
            {
                Console.WriteLine($"{Name}'s Evasion allows them to avoid damage completely!");
                return 0;
            }
            else
            {
                int halfDamage = (int)Math.Ceiling(damage / 2.0);
                Console.WriteLine($"{Name} takes half damage due to Evasion: {halfDamage}.");
                return halfDamage;
            }
        }

        /// <summary>
        /// Wholeness of Body (Level 8+): Spend ki to heal HP. Maximum healing equals monk level.
        /// </summary>
        public virtual bool WholenessOfBody()
        {
            if (!_wholenessOfBodyUnlocked)
            {
                Console.WriteLine($"{Name} has not learned Wholeness of Body yet (requires level 8).");
                return false;
            }

            if (_kiPoints < 3)
            {
                Console.WriteLine($"{Name} needs at least 3 ki points for Wholeness of Body.");
                return false;
            }

            _kiPoints -= 3;
            int healing = _level; // Maximum healing equals monk level
            Heal(healing);
            Console.WriteLine($"{Name} spends 3 ki points to restore {healing} hit points through Wholeness of Body.");
            return true;
        }

        /// <summary>
        /// Get the Ki save DC.
        /// </summary>
        public int GetKiSaveDC()
        {
            int profBonus = GetProficiencyBonusForLevel(Level);
            int wisMod = Math.Max(-5, (Wisdom - 10) / 2);
            return 8 + profBonus + wisMod;
        }

        /// <summary>
        /// Get the value of the martial arts die.
        /// </summary>
        private int GetMartialArtsDieValue()
        {
            switch (_martialArtsDie)
            {
                case 4: return new Random().Next(1, 5);
                case 6: return new Random().Next(1, 7);
                case 8: return new Random().Next(1, 9);
                case 10: return new Random().Next(1, 11);
                case 12: return new Random().Next(1, 13);
                default: return new Random().Next(1, 5);
            }
        }

        // ==================== Stat Calculations ====================

        /// <summary>
        /// Override base stats calculation for Monk.
        /// Hit Points: d8 per level + Con mod
        /// Unarmored Defense: 10 + Dex mod + Wis mod
        /// </summary>
        protected override void CalculateBaseStats()
        {
            int conMod = Math.Max(-5, (Constitution - 10) / 2);
            int dexMod = Math.Max(-5, (Dexterity - 10) / 2);
            int wisMod = Math.Max(-5, (Wisdom - 10) / 2);

            // Max HP: d8 per level + con mod per level
            int hpFromFirstLevel = 8 + conMod;
            int hpFromHigherLevels = (Level - 1) * (8 + conMod);
            MaxHitPoints = hpFromFirstLevel + hpFromHigherLevels;

            if (HitPoints > MaxHitPoints || HitPoints <= 0)
            {
                HitPoints = MaxHitPoints;
            }

            // Unarmored Defense: 10 + Dex mod + Wis mod
            if (_unarmoredDefenseActive)
            {
                ArmorClass = 10 + dexMod + wisMod;
            }
            else
            {
                ArmorClass = 10 + dexMod; // Standard AC without unarmored defense
            }

            // Speed with Unarmored Movement
            int baseSpeed = _race != null ? _race.Speed : 30;
            if (_unarmoredMovementUnlocked)
            {
                Speed = baseSpeed + _unarmoredMovementBonus;
            }
            else
            {
                Speed = baseSpeed;
            }
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

        /// <summary>
        /// Apply features gained at specific monk levels.
        /// </summary>
        private void ApplyLevelFeatures()
        {
            // ===== LEVEL 1: Martial Arts, Unarmored Defense =====
            if (Level >= 1)
            {
                _martialArtsDie = 4;
                _unarmoredDefenseActive = true;
                UpdateKiPoints();
            }

            // ===== LEVEL 2: Ki, Unarmored Movement, Monastic Tradition =====
            if (Level >= 2)
            {
                UpdateKiPoints();
                _unarmoredMovementUnlocked = true;
                _unarmoredMovementBonus += 10;
                ApplyMonasticTradition();
            }

            // ===== LEVEL 3: Patient Defense, Step of the Wind, Monastic Tradition Features =====
            if (Level >= 3)
            {
                _patientDefenseUnlocked = true;
                _stepOfTheWindUnlocked = true;
                UpdateKiPoints();
                ApplyTraditionFeature1();
            }

            // ===== LEVEL 4: Deflect Missiles, ASI/Fast Hands =====
            if (Level >= 4)
            {
                _deflectMissilesUnlocked = true;
                _fastHandsUnlocked = true;
                UpdateKiPoints();
            }

            // ===== LEVEL 5: Stunning Strike, Slow Fall (+5), Monastic Tradition Feature 2 =====
            if (Level >= 5)
            {
                _slowFallUnlocked = true;
                _martialArtsDie = 6; // d6
                UpdateKiPoints();
                ApplyTraditionFeature2();
            }

            // ===== LEVEL 6: Unarmored Movement Improvement (+15 total) =====
            if (Level >= 6)
            {
                _unarmoredMovementBonus += 5; // Total: +15
                ApplyTraditionFeature3Or4(true);
            }

            // ===== LEVEL 7: Evasion =====
            if (Level >= 7)
            {
                _evasionUnlocked = true;
                UpdateKiPoints();
            }

            // ===== LEVEL 8: Wholeness of Body =====
            if (Level >= 8)
            {
                _wholenessOfBodyUnlocked = true;
                ApplyTraditionFeature3Or4(false);
            }

            // ===== LEVEL 9: Unarmored Mobility (+20 movement total) =====
            if (Level >= 9)
            {
                _unarmoredMovementBonus += 5; // Total: +20
                _martialArtsDie = 8; // d8
                UpdateKiPoints();
            }

            // ===== LEVEL 10: Empty Body =====
            if (Level >= 10)
            {
                _emptyBodyUnlocked = true;
                ApplyTraditionFeature4();
            }

            // ===== LEVEL 11: Still Mind, Stunning Strike Mastery =====
            if (Level >= 11)
            {
                _stunningStrikeMasteryUnlocked = true;
                UpdateKiPoints();
            }

            // ===== LEVEL 12: Improvement to Unarmored Movement (+25 total) =====
            if (Level >= 12)
            {
                _unarmoredMovementBonus += 5; // Total: +25
                UpdateKiPoints();
            }

            // ===== LEVEL 13: Purity of Body =====
            if (Level >= 13)
            {
                UpdateKiPoints();
            }

            // ===== LEVEL 14: Tongue of the Sun and Moon =====
            if (Level >= 14)
            {
                _tongueOfTheSunAndMoonUnlocked = true;
                UpdateKiPoints();
            }

            // ===== LEVEL 15: Diamond Soul =====
            if (Level >= 15)
            {
                _diamondSoulUnlocked = true;
                _martialArtsDie = 10; // d10
                UpdateKiPoints();
            }

            // ===== LEVEL 16: Unarmored Movement Improvement (+30 total) =====
            if (Level >= 16)
            {
                _unarmoredMovementBonus += 5; // Total: +30
                UpdateKiPoints();
            }

            // ===== LEVEL 17: Timeless Body, Body of the Elements =====
            if (Level >= 17)
            {
                _timelessBodyUnlocked = true;
                _bodyOfTheElementsUnlocked = true;
                _martialArtsDie = 12; // d12
                UpdateKiPoints();
                ApplyTraditionFeature5();
            }

            // ===== LEVEL 18: Body of the Element (immune to poison) =====
            if (Level >= 18)
            {
                UpdateKiPoints();
            }

            // ===== LEVEL 19: ASI =====
            if (Level >= 19)
            {
                _martialArtsDie = 12; // Already d12
                UpdateKiPoints();
            }

            // ===== LEVEL 20: Ascended State =====
            if (Level >= 20)
            {
                _ascendedStateUnlocked = true;
                UpdateKiPoints();
            }

            // Apply subclass-specific features
            ApplySubclassLevelFeatures();
        }

        private void UpdateKiPoints()
        {
            // Ki points = monk level at levels 1-19, max ki = level
            _maxKiPoints = Level;
            if (Level >= 20)
            {
                _maxKiPoints = Level;
            }
            if (_kiPoints > _maxKiPoints)
            {
                _kiPoints = _maxKiPoints;
            }
        }

        private void ApplyMonasticTradition()
        {
            switch (_subclass)
            {
                case MonkSubclass.WayOfTheOpenHand:
                    _subclassName = "Way of the Open Hand";
                    Console.WriteLine($"{Name} chooses the Way of the Open Hand!");
                    break;
                case MonkSubclass.WayOfShadow:
                    _subclassName = "Way of the Shadow";
                    Console.WriteLine($"{Name} chooses the Way of the Shadow!");
                    break;
                case MonkSubclass.WayOfTheFourElements:
                    _subclassName = "Way of the Four Elements";
                    Console.WriteLine($"{Name} chooses the Way of the Four Elements!");
                    break;
                case MonkSubclass.WayOfTheLongDeath:
                    _subclassName = "Way of the Long Death";
                    Console.WriteLine($"{Name} chooses the Way of the Long Death!");
                    break;
            }
        }

        private void ApplyTraditionFeature1()
        {
            switch (_subclass)
            {
                case MonkSubclass.WayOfTheOpenHand:
                    _openHandTechniqueUnlocked = true;
                    Console.WriteLine("Open Hand Technique: You can trip or push targets when you use Open Hand Technique.");
                    break;
                case MonkSubclass.WayOfShadow:
                    _shadowArtsUnlocked = true;
                    _shadowSpellsKnown = new List<string> { "Disguise Self", "Silent Image" };
                    Console.WriteLine("Shadow Arts: You can spend ki to cast Disguise Self and Silent Image.");
                    break;
                case MonkSubclass.WayOfTheFourElements:
                    _knownDisciplines.Add(ElementalDiscipline.MarkOfTheHuntingWolf);
                    _knownDisciplines.Add(ElementalDiscipline.MarkOfTheBeast);
                    _knownDisciplines.Add(ElementalDiscipline.MarkOfTheStorm);
                    Console.WriteLine("Elemental Discipline: You learn three martial arts disciplines.");
                    break;
                case MonkSubclass.WayOfTheLongDeath:
                    _malignTransfigurationDieSize = 4;
                    Console.WriteLine("Malign Transfiguration: When you reduce a creature to 0 HP, roll a die and gain temp HP.");
                    break;
            }
        }

        private void ApplyTraditionFeature2()
        {
            switch (_subclass)
            {
                case MonkSubclass.WayOfTheOpenHand:
                    Console.WriteLine("Champions of Movement: You can trip creatures with your Open Hand Technique.");
                    break;
                case MonkSubclass.WayOfShadow:
                    _shadowyDodgeUnlocked = true;
                    Console.WriteLine("Shadowy Dodge: When you miss a melee attack, +4 to AC until start of your next turn.");
                    break;
                case MonkSubclass.WayOfTheFourElements:
                    Console.WriteLine("Fists of Wind: You can use Mark of the Storm as part of Flurry of Blows.");
                    break;
                case MonkSubclass.WayOfTheLongDeath:
                    _quiveringPalmAvailable = true;
                    Console.WriteLine("Quivering Palm: When you stun a creature with Stunning Strike, you can establish a quivering palm.");
                    break;
            }
        }

        private void ApplyTraditionFeature3Or4(bool isFeature3)
        {
            switch (_subclass)
            {
                case MonkSubclass.WayOfTheOpenHand:
                    if (isFeature3)
                    {
                        Console.WriteLine("Petrifying Fist: You can attempt to turn a creature to stone.");
                    }
                    else
                    {
                        _survivorOfDeathUnlocked = true;
                        Console.WriteLine("Survivor of Death: When you roll initiative with 0 ki, regain ki points equal to half your monk level.");
                    }
                    break;
                case MonkSubclass.WayOfShadow:
                    if (isFeature3)
                    {
                        Console.WriteLine("Ephemeral Step: As a bonus action, gain 10ft speed bonus and resistance to physical damage for 1 minute.");
                    }
                    else
                    {
                        _stepOfTheMoonUnlocked = true;
                        Console.WriteLine("Step of the Moon: You can move without traces and see in darkness.");
                    }
                    break;
                case MonkSubclass.WayOfTheFourElements:
                    if (isFeature3)
                    {
                        Console.WriteLine("Resist Elemental Discipline: When you use a discipline to deal damage, you and chosen creatures resist that damage type for 1 minute.");
                    }
                    else
                    {
                        Console.WriteLine("Law of Beginnings: When you spend ki for a discipline, regain ki points equal to the discipline cost.");
                    }
                    break;
                case MonkSubclass.WayOfTheLongDeath:
                    if (isFeature3)
                    {
                        _malignTransfigurationDieSize = 6;
                        Console.WriteLine("Malign Transfiguration die increases to d6.");
                    }
                    else
                    {
                        _survivorOfDeathUnlocked = true;
                        Console.WriteLine("Survivor of Death: When you drop to 0 HP, regain HP equal to your monk level.");
                    }
                    break;
            }
        }

        private void ApplyTraditionFeature4()
        {
            switch (_subclass)
            {
                case MonkSubclass.WayOfTheOpenHand:
                    Console.WriteLine("Control Flames: When you hit with Open Hand Technique, you can move the target up to 10 feet.");
                    break;
                case MonkSubclass.WayOfShadow:
                    Console.WriteLine("Perfect Self: You can become invisible by spending 6 ki points.");
                    break;
                case MonkSubclass.WayOfTheFourElements:
                    Console.WriteLine("Elemental Attunement: You can use your discipline to breathe underwater and resist relevant damage types.");
                    break;
                case MonkSubclass.WayOfTheLongDeath:
                    Console.WriteLine("Embrace of Death: When you reduce a creature to 0 HP, gain temp HP equal to your monk level + WIS mod.");
                    break;
            }
        }

        private void ApplyTraditionFeature5()
        {
            switch (_subclass)
            {
                case MonkSubclass.WayOfTheOpenHand:
                    Console.WriteLine("Collapsing Palm: When you hit a creature stunned by Stunning Strike, deal extra psychic damage.");
                    break;
                case MonkSubclass.WayOfShadow:
                    Console.WriteLine("Panopticism: You can see through shadows and creatures cannot hide from you in dim light.");
                    break;
                case MonkSubclass.WayOfTheFourElements:
                    Console.WriteLine("Master of Elements: You can spend ki to create effects from any discipline you know.");
                    break;
                case MonkSubclass.WayOfTheLongDeath:
                    Console.WriteLine("Suicide Strike: When you deal damage with Quivering Palm, you can choose to kill the target instantly if they have less than your monk level + Wis mod HP.");
                    break;
            }
        }

        private void ApplySubclassLevelFeatures()
        {
            switch (_subclass)
            {
                case MonkSubclass.WayOfTheFourElements:
                    if (Level >= 3)
                    {
                        _ashenPillarUnlocked = true;
                    }
                    if (Level >= 15)
                    {
                        _resistElementalDisciplineUnlocked = true;
                    }
                    if (Level >= 17)
                    {
                        _lawOfBeginningsUnlocked = true;
                    }
                    break;

                case MonkSubclass.WayOfTheLongDeath:
                    if (Level >= 3)
                    {
                        _malignTransfigurationDieSize = Math.Max(4, Level / 3); // d4 at level 3, d6 at level 6+
                    }
                    break;
            }
        }

        /// <summary>
        /// Check and apply level-up features when the monk levels up.
        /// </summary>
        public void OnLevelUp()
        {
            ApplyLevelFeatures();
            UpdateKiPoints();
            Console.WriteLine($"{Name} gains new features at level {Level}!");
            DisplayLevelFeatures();
        }

        private void DisplayLevelFeatures()
        {
            Console.WriteLine("=== New Features Unlocked ===");

            if (_martialArtsDie >= 6)
            {
                Console.WriteLine($"- Martial Arts die increased to d{_martialArtsDie}");
            }

            if (_unarmoredMovementUnlocked && _unarmoredMovementBonus > 0)
            {
                Console.WriteLine($"- Unarmored Movement: +{_unarmoredMovementBonus} speed");
            }

            if (_patientDefenseUnlocked)
            {
                Console.WriteLine("- Patient Defense: Spend 1 ki to Dodge as bonus action");
            }

            if (_stepOfTheWindUnlocked)
            {
                Console.WriteLine("- Step of the Wind: Spend 1 ki for high jump and double climb speed");
            }

            if (_deflectMissilesUnlocked)
            {
                Console.WriteLine("- Deflect Missiles: Reduce damage from ranged attacks");
            }

            if (_slowFallUnlocked)
            {
                Console.WriteLine("- Slow Fall: Reduce falling damage by 5 x monk level per ki");
            }

            if (_stunningStrikeMasteryUnlocked)
            {
                Console.WriteLine("- Stunning Strike Mastery: Reduced save DC for Stunning Strike");
            }

            if (_evasionUnlocked)
            {
                Console.WriteLine("- Evasion: Take no damage on successful DEX save, half on failure");
            }

            if (_wholenessOfBodyUnlocked)
            {
                Console.WriteLine("- Wholeness of Body: Spend 3 ki to heal monk level HP");
            }

            if (_diamondSoulUnlocked)
            {
                Console.WriteLine("- Diamond Soul: As a reaction, add +2 to AC when hit by an attack");
            }

            if (_emptyBodyUnlocked)
            {
                Console.WriteLine("- Empty Body: Spend 4 ki to become invisible and immune to detection for 1 minute");
            }

            if (_tongueOfTheSunAndMoonUnlocked)
            {
                Console.WriteLine("- Tongue of the Sun and Moon: Speak and read all languages, see in dark and dim light");
            }

            DisplaySubclassFeatures();
        }

        private void DisplaySubclassFeatures()
        {
            Console.WriteLine("\n--- Subclass Features ---");

            switch (_subclass)
            {
                case MonkSubclass.WayOfTheOpenHand:
                    Console.WriteLine("Way of the Open Hand:");
                    if (_openHandTechniqueUnlocked)
                    {
                        Console.WriteLine("  Open Hand Technique: Trip, push, or stagger targets");
                    }
                    if (_survivorOfDeathUnlocked)
                    {
                        Console.WriteLine("  Survivor of Death: Regain ki when dropping to 0 HP");
                    }
                    break;

                case MonkSubclass.WayOfShadow:
                    Console.WriteLine("Way of the Shadow:");
                    if (_shadowArtsUnlocked)
                    {
                        Console.WriteLine($"  Shadow Arts: Can cast {_shadowSpellsKnown.Count} shadow spells with ki");
                    }
                    if (_shadowyDodgeUnlocked)
                    {
                        Console.WriteLine("  Shadowy Dodge: +4 AC after missing a melee attack");
                    }
                    break;

                case MonkSubclass.WayOfTheFourElements:
                    Console.WriteLine("Way of the Four Elements:");
                    Console.WriteLine($"  Known Disciplines: {string.Join(", ", _knownDisciplines)}");
                    if (_ashenPillarUnlocked)
                    {
                        Console.WriteLine("  Ashen Pillar: Spend ki to create pillar of ash");
                    }
                    break;

                case MonkSubclass.WayOfTheLongDeath:
                    Console.WriteLine("Way of the Long Death:");
                    if (_quiveringPalmAvailable)
                    {
                        Console.WriteLine($"  Quivering Palm: d{_malignTransfigurationDieSize} damage when target drops to 0 HP");
                    }
                    if (_survivorOfDeathUnlocked)
                    {
                        Console.WriteLine("  Survivor of Death: Gain temp HP when dropping to 0 HP");
                    }
                    break;
            }
        }

        // ==================== Override Base Methods ====================

        public override void Attack()
        {
            int martialArtsDamage = GetMartialArtsDieValue();
            int wisMod = Math.Max(-5, (Wisdom - 10) / 2);
            Console.WriteLine($"{Name} makes a martial arts attack dealing {martialArtsDamage} + {wisMod} damage!");
        }

        public override void TakeDamage(int damage)
        {
            // Apply Deflect Missiles for ranged attacks
            if (_deflectMissilesUnlocked && IsRangedAttack())
            {
                int reducedDamage = DeflectMissiles(damage);
                base.TakeDamage(reducedDamage);
                return;
            }

            // Apply Evasion if applicable (handled in ApplyEvasion method)
            base.TakeDamage(damage);

            // Check for Quivering Palm trigger
            if (_quiveringPalmAvailable && HitPoints <= 0 && _quiveringPalmActive)
            {
                int damageDie = _malignTransfigurationDieSize;
                int conSaveDC = GetKiSaveDC();
                Console.WriteLine($"{Name}'s Quivering Palm is active! Target must make a CON save (DC {conSaveDC}) or take {damageDie} damage.");
            }
        }

        private bool IsRangedAttack()
        {
            // Simplified: In a real implementation, this would be passed as a parameter
            return false;
        }

        public override void Heal(int amount)
        {
            base.Heal(amount);
        }

        /// <summary>
        /// Override level up to apply subclass features.
        /// </summary>
        public override void LevelUp()
        {
            int previousLevel = _level;
            base.LevelUp();

            Console.WriteLine($"{_name} has reached level {_level}!");

            // Update Ki points and martial arts die
            UpdateKiPoints();
            ApplyLevelFeatures();

            // Display new features
            if (Level == 2)
            {
                Console.WriteLine("Monk: Gained Ki points and Unarmored Movement!");
            }
            else if (Level == 3)
            {
                Console.WriteLine("Monk: Gained Monastic Tradition features!");
            }
            else if (Level == 5)
            {
                Console.WriteLine("Monk: Gained Stunning Strike and Slow Fall!");
            }
            else if (Level == 7)
            {
                Console.WriteLine("Monk: Gained Evasion!");
            }
            else if (Level == 10)
            {
                Console.WriteLine("Monk: Gained Empty Body!");
            }
            else if (Level == 15)
            {
                Console.WriteLine("Monk: Gained Diamond Soul!");
            }
            else if (Level == 18)
            {
                Console.WriteLine("Monk: Gained Body of the Elements - Immune to poison damage and effects!");
            }
            else if (Level == 20)
            {
                Console.WriteLine("Monk: Gained Ascended State - Constant Flurry of Blows, Step of the Wind, and Silence!");
            }
        }

        public override void ShortRest()
        {
            base.ShortRest();

            // Monks regain all ki points on a short rest
            int previousKi = _kiPoints;
            _kiPoints = _maxKiPoints;
            Console.WriteLine($"{Name} regains all {_maxKiPoints} ki points after a short rest.");

            // Reset certain features
            _elementalAdeptUsed = false;
            if (_subclass == MonkSubclass.WayOfShadow)
            {
                // Shadow spells can be recovered with ki on short rest
            }
        }

        public override void LongRest()
        {
            base.LongRest();

            // Monks already regain ki on short rest, but reset daily-use features here
            _elementalAdeptUsed = false;
            _quiveringPalmActive = false;
            _quiveringPalmTargetConstitution = 0;
        }

        public override void DisplayCharacter()
        {
            string subclassPrefix = GetSubclassName(_subclass);

            Console.WriteLine();
            Console.WriteLine($"=== {Name} (Level {_level} Monk - {subclassPrefix}) ===");
            Console.WriteLine($"Hit Points: {HitPoints}/{MaxHitPoints} | AC: {ArmorClass} | Speed: {Speed}");
            Console.WriteLine($"Ki Points: {_kiPoints}/{_maxKiPoints} | Martial Arts Die: d{_martialArtsDie}");

            if (_unarmoredDefenseActive)
            {
                int dexMod = Math.Max(-5, (Dexterity - 10) / 2);
                int wisMod = Math.Max(-5, (Wisdom - 10) / 2);
                Console.WriteLine($"Unarmored Defense: 10 + {dexMod} (Dex) + {wisMod} (Wis) = {10 + dexMod + wisMod} AC");
            }

            if (_unarmoredMovementUnlocked && _unarmoredMovementBonus > 0)
            {
                Console.WriteLine($"Unarmored Movement: +{_unarmoredMovementBonus} speed");
            }

            Console.WriteLine();
            Console.WriteLine("--- Monk Abilities ---");
            Console.WriteLine("Martial Arts: Use Wisdom for attack and damage with monk weapons");
            Console.WriteLine($"Flurry of Blows: Spend 1 ki for two additional attacks (bonus action)");
            Console.WriteLine($"Patient Defense: Spend 1 ki to Dodge as bonus action");
            Console.WriteLine($"Step of the Wind: Spend 1 ki for high jump and double climb speed");

            if (_deflectMissilesUnlocked)
            {
                Console.WriteLine("Deflect Missiles: Reduce damage from ranged attacks");
            }

            if (_slowFallUnlocked)
            {
                Console.WriteLine($"Slow Fall: Reduce falling damage by {_maxKiPoints * 5} per ki point");
            }

            if (_evasionUnlocked)
            {
                Console.WriteLine("Evasion: Take no damage on successful DEX save, half on failure");
            }

            if (_stunningStrikeMasteryUnlocked)
            {
                Console.WriteLine("Stunning Strike Mastery: Target must be incapacitated to break Stunning Strike");
            }

            if (_wholenessOfBodyUnlocked)
            {
                Console.WriteLine("Wholeness of Body: Spend 3 ki to heal monk level HP");
            }

            if (_diamondSoulUnlocked)
            {
                Console.WriteLine("Diamond Soul: As a reaction, add +2 to AC when hit by an attack");
            }

            if (_emptyBodyUnlocked)
            {
                Console.WriteLine("Empty Body: Spend 4 ki to become invisible and immune to detection for 1 minute");
            }

            if (_tongueOfTheSunAndMoonUnlocked)
            {
                Console.WriteLine("Tongue of the Sun and Moon: Speak and read all languages, see in dark and dim light");
            }

            DisplaySubclassInfo();

            Console.WriteLine();
            Console.WriteLine("Ability Scores:");
            Console.WriteLine($"  Strength:    {Strength} (mod +{Math.Max(-5, (Strength - 10) / 2)})");
            Console.WriteLine($"  Dexterity:   {Dexterity} (mod +{Math.Max(-5, (Dexterity - 10) / 2)})");
            Console.WriteLine($"  Constitution:{Constitution} (mod +{Math.Max(-5, (Constitution - 10) / 2)})");
            Console.WriteLine($"  Intelligence:{Intelligence} (mod +{Math.Max(-5, (Intelligence - 10) / 2)})");
            Console.WriteLine($"  Wisdom:      {Wisdom} (mod +{Math.Max(-5, (Wisdom - 10) / 2)})");
            Console.WriteLine($"  Charisma:    {Charisma} (mod +{Math.Max(-5, (Charisma - 10) / 2)})");

            Console.WriteLine("=== End Character Sheet ===");
        }

        private void DisplaySubclassInfo()
        {
            Console.WriteLine();
            Console.WriteLine("--- Subclass Features ---");

            switch (_subclass)
            {
                case MonkSubclass.WayOfTheOpenHand:
                    Console.WriteLine("Way of the Open Hand:");
                    if (_openHandTechniqueUnlocked)
                    {
                        Console.WriteLine("  Open Hand Technique: When you hit with Unarmed Strike, target must make CON save or be knocked prone/pushed 15ft");
                    }
                    if (_survivorOfDeathUnlocked)
                    {
                        int kiToRegain = _level / 2;
                        Console.WriteLine($"  Survivor of Death: When you drop to 0 HP, regain {kiToRegain} ki points");
                    }
                    break;

                case MonkSubclass.WayOfShadow:
                    Console.WriteLine("Way of the Shadow:");
                    if (_shadowArtsUnlocked)
                    {
                        Console.WriteLine($"  Shadow Arts: Spend 2 ki to cast {_shadowSpellsKnown.Count} shadow spells (Disguise Self, Silent Image)");
                    }
                    if (_shadowyDodgeUnlocked)
                    {
                        Console.WriteLine("  Shadowy Dodge: When you miss with a melee attack, +4 AC until start of your next turn");
                    }
                    if (_stepOfTheMoonUnlocked)
                    {
                        Console.WriteLine("  Step of the Moon: You can move without leaving traces and see in magical darkness");
                    }
                    break;

                case MonkSubclass.WayOfTheFourElements:
                    Console.WriteLine("Way of the Four Elements:");
                    Console.WriteLine($"  Known Disciplines: {string.Join(", ", _knownDisciplines)}");
                    if (_ashenPillarUnlocked)
                    {
                        Console.WriteLine("  Ashen Pillar: Spend ki to create pillar of ash or hurl a fan of embers");
                    }
                    if (_resistElementalDisciplineUnlocked)
                    {
                        Console.WriteLine("  Resist Elemental Discipline: Resist damage type from your discipline for 1 minute");
                    }
                    if (_lawOfBeginningsUnlocked)
                    {
                        Console.WriteLine("  Law of Beginnings: When you spend ki for a discipline, regain ki equal to its cost");
                    }
                    break;

                case MonkSubclass.WayOfTheLongDeath:
                    Console.WriteLine("Way of the Long Death:");
                    if (_quiveringPalmAvailable)
                    {
                        Console.WriteLine($"  Quivering Palm: When you reduce a creature to 0 HP, establish control over its body. Target makes CON save (DC {GetKiSaveDC()}) or takes {_malignTransfigurationDieSize}d10 necrotic damage");
                    }
                    if (_survivorOfDeathUnlocked)
                    {
                        Console.WriteLine("  Survivor of Death: When you drop to 0 HP, regain HP equal to your monk level + WIS mod");
                    }
                    break;
            }
        }

        // ==================== Subclass-Specific Methods ====================

        /// <summary>
        /// Apply Open Hand Technique effects (Way of the Open Hand).
        /// </summary>
        public virtual bool ApplyOpenHandTechnique(int targetConstitutionSaveDC, string effectType)
        {
            if (!_openHandTechniqueUnlocked)
            {
                Console.WriteLine($"{Name} has not learned Open Hand Technique.");
                return false;
            }

            Console.WriteLine($"{Name} uses Open Hand Technique! Target must make CON save (DC {targetConstitutionSaveDC}) or be {(effectType == "prone" ? "knocked prone" : "pushed 15 feet")}.");
            return true;
        }

        /// <summary>
        /// Cast a shadow spell (Way of the Shadow).
        /// </summary>
        public virtual bool CastShadowSpell(string spellName)
        {
            if (!_shadowArtsUnlocked)
            {
                Console.WriteLine($"{Name} has not learned Shadow Arts.");
                return false;
            }

            if (!_shadowSpellsKnown.Contains(spellName))
            {
                Console.WriteLine($"{Name} does not know the shadow spell: {spellName}");
                return false;
            }

            if (_kiPoints < 2)
            {
                Console.WriteLine($"{Name} needs at least 2 ki points to cast shadow spells.");
                return false;
            }

            _kiPoints -= 2;
            Console.WriteLine($"{Name} spends 2 ki points to cast {spellName} using Shadow Arts!");
            return true;
        }

        /// <summary>
        /// Choose a water discipline (Way of the Four Elements).
        /// </summary>
        public virtual void ChooseWaterDiscipline(WaterDiscipline discipline)
        {
            if (_subclass != MonkSubclass.WayOfTheFourElements)
            {
                Console.WriteLine($"{Name} is not a Way of the Four Elements monk.");
                return;
            }

            _waterDisciplineChoice = discipline;
            Console.WriteLine($"{Name} chooses {_waterDisciplineChoice} as their water discipline.");
        }

        /// <summary>
        /// Learn an elemental discipline (Way of the Four Elements).
        /// </summary>
        public virtual bool LearnElementalDiscipline(ElementalDiscipline discipline)
        {
            if (_subclass != MonkSubclass.WayOfTheFourElements)
            {
                Console.WriteLine($"{Name} is not a Way of the Four Elements monk.");
                return false;
            }

            if (_knownDisciplines.Count >= MaxDisciplinesKnown)
            {
                Console.WriteLine($"{Name} has learned the maximum number of disciplines ({MaxDisciplinesKnown}).");
                return false;
            }

            if (_knownDisciplines.Contains(discipline))
            {
                Console.WriteLine($"{Name} already knows this discipline.");
                return false;
            }

            _knownDisciplines.Add(discipline);
            Console.WriteLine($"{Name} learns the elemental discipline: {discipline}");
            return true;
        }

        /// <summary>
        /// Use Quivering Palm to establish control over a creature (Way of the Long Death).
        /// </summary>
        public virtual bool EstablishQuiveringPalm(int targetConstitution)
        {
            if (!_quiveringPalmAvailable)
            {
                Console.WriteLine($"{Name} has not learned Quivering Palm.");
                return false;
            }

            _quiveringPalmTargetConstitution = targetConstitution;
            _quiveringPalmActive = true;

            int saveDC = GetKiSaveDC();
            Console.WriteLine($"{Name} establishes Quivering Palm over the target!");
            Console.WriteLine($"Target must make a CON save (DC {saveDC}) at the end of 1 minute or take {_malignTransfigurationDieSize}d10 necrotic damage.");
            return true;
        }

        /// <summary>
        /// Activate Malign Transfiguration (Way of the Long Death).
        /// </summary>
        public virtual int ApplyMalignTransfiguration()
        {
            if (_subclass != MonkSubclass.WayOfTheLongDeath)
            {
                return 0;
            }

            int dieValue = new Random().Next(1, _malignTransfigurationDieSize + 1);
            int tempHP = dieValue + Math.Max(-5, (Wisdom - 10) / 2);
            Console.WriteLine($"{Name} activates Malign Transfiguration! Gains {tempHP} temporary hit points.");
            return tempHP;
        }

        /// <summary>
        /// Activate Ascended State (Level 20 feature).
        /// </summary>
        public virtual bool ActivateAscendedState()
        {
            if (!_ascendedStateUnlocked)
            {
                Console.WriteLine($"{Name} has not reached level 20 for Ascended State.");
                return false;
            }

            Console.WriteLine($"{Name} activates Ascended State!");
            Console.WriteLine("Flurry of Blows, Step of the Wind, and Silence are always active. No ki cost required.");
            _ascendedStateUnlocked = true;
            return true;
        }

        /// <summary>
        /// Get the subclassName string for a given subclass enum value.
        /// </summary>
        private string GetSubclassName(MonkSubclass subclass)
        {
            switch (subclass)
            {
                case MonkSubclass.WayOfTheOpenHand: return "Way of the Open Hand";
                case MonkSubclass.WayOfShadow: return "Way of the Shadow";
                case MonkSubclass.WayOfTheFourElements: return "Way of the Four Elements";
                case MonkSubclass.WayOfTheLongDeath: return "Way of the Long Death";
                default: return "Unknown";
            }
        }

        // ==================== Feature Classes ====================

        /// <summary>
        /// Open Hand Technique feature - Way of the Open Hand level 3.
        /// </summary>
        public class OpenHandTechniqueFeature : FeatureClass
        {
            public OpenHandTechniqueFeature() : base("Open Hand Technique", "When you hit a creature with an unarmed strike, the target must make a Constitution saving throw. On a failed save, the target is either knocked prone or pushed up to 15 feet away from you.")
            {
            }

            public override void UseFeature()
            {
                Console.WriteLine("Open Hand Technique: Target must make CON save or be knocked prone/pushed.");
            }
        }

        /// <summary>
        /// Shadow Arts feature - Way of the Shadow level 3.
        /// </summary>
        public class ShadowArtsFeature : FeatureClass
        {
            protected int _kiCost;

            public ShadowArtsFeature() : base("Shadow Arts", "Starting when you choose this path at 3rd level, you can use your ki to imitate the spells of disguise self and silent image. You cast each spell without material components or a spell slot. Each spell costs 2 ki points to cast in this way.")
            {
                _kiCost = 2;
            }

            public int KiCost { get => _kiCost; set => _kiCost = value; }

            public override void UseFeature()
            {
                Console.WriteLine($"Shadow Arts: Cast disguise self or silent image for {_kiCost} ki points.");
            }
        }

        /// <summary>
        /// Elemental Discipline feature - Way of the Four Elements level 3.
        /// </summary>
        public class ElementalDisciplineFeature : FeatureClass
        {
            protected string _disciplineName;

            public ElementalDisciplineFeature() : base("Elemental Discipline", "Starting when you choose this path at 3rd level, you learn elemental disciplines that allow you to mimic the effects of divine spells with your martial arts.")
            {
                _disciplineName = "Mark of the Hunting Wolf";
            }

            public string DisciplineName { get => _disciplineName; set => _disciplineName = value; }

            public override void UseFeature()
            {
                Console.WriteLine($"Elemental Discipline: {_disciplineName} - Spend ki to cast martial arts spells.");
            }
        }

        /// <summary>
        /// Quivering Palm feature - Way of the Long Death level 3.
        /// </summary>
        public class QuiveringPalmFeature : FeatureClass
        {
            protected int _dieSize;

            public QuiveringPalmFeature() : base("Quivering Palm", "At 3rd level, you gain the ability to set up devastating unarmed attacks. When you reduce a creature to 0 hit points with a melee weapon attack, you can establish control over its body. This control lasts for a maximum of 1 day per monk level. If the target succeeds on a Constitution save (DC your ki save DC), the effect ends.")
            {
                _dieSize = 4;
            }

            public int DieSize { get => _dieSize; set => _dieSize = value; }

            public override void UseFeature()
            {
                Console.WriteLine($"Quivering Palm: Target must make CON save (DC {_dieSize * 2 + 8}) or take d10 necrotic damage.");
            }
        }
    }
}