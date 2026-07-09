using System;
using System.Collections.Generic;
using DnDCharacterManager.Ability;
using RaceClass = DnDCharacterManager.Race.Race;
using BackgroundClass = DnDCharacterManager.Background.Background;
using SpellClass = DnDCharacterManager.Spell.Spell;
using SpellBookClass = DnDCharacterManager.Spell.SpellBook;

namespace DnDCharacterManager.Character.Classes
{
    /// <summary>
    /// Enum representing the three Fighter subclasses.
    /// </summary>
    public enum FighterSubclass
    {
        BattleMaster,
        Champion,
        EldritchKnight
    }

    /// <summary>
    /// Enum for selectable fighting styles.
    /// </summary>
    public enum FightingStyle
    {
        TwoWeaponFighting,
        Dueling,
        GreatWeaponFighting,
        Protection,
        Defensive,
        ThrownWeapon,
        RangedWeapon
    }

    /// <summary>
    /// Enum for Battle Master combat maneuvers.
    /// </summary>
    public enum CombatManeuver
    {
        BarghestsPeek,
        Bluff,
        BashMind,
        BorrisStrikes,
        DazzlingStrike,
        DistractingStrike,
        DrunkenOpportunity,
        EvasiveFootwork,
        FeintingAttack,
        GoadingAttack,
        HelpingStrike,
        PreciseAttack,
        PushingAttack,
        Rally,
        RapellingAttacker,
        ReelingAttack,
        SweepingStrike,
        TacticalAssessment,
        TripAttack
    }

    /// <summary>
    /// Fighter character class with full level 1-20 feature progression and subclass support.
    /// Implements D&D 5e rules by the book.
    /// </summary>
    public class Fighter : Character
    {
        // ==================== Core Fighter Properties ====================

        private int _secondWindUses;
        private int _maxActionSurgeUses;
        private int _actionSurgeUsesRemaining;
        private bool _indomitableUsedThisRest;
        private int _indomitableUsesRemaining;

        // Subclass tracking
        private FighterSubclass _subclass;
        private string _subclassName;
        private FightingStyle _fightingStyle;
        private string _fightingStyleName;

        // Combat style bonus (applied in calculations)
        private int _combatStyleDamageBonus;
        private int _combatStyleAcBonus;

        // ==================== Battle Master Properties ====================

        private int _superiorityDiceCount;
        private int _superiorityDieSize;
        private List<string> _knownManeuvers;
        private const int MaxManeuversKnown = 10;
        private int _maneuversKnown;
        private bool _recoveryUsedThisRest;
        private bool _superioritySurgeUsedThisCombat;

        // ==================== Champion Properties ====================

        private int _improvedCriticalThreshold;
        private int _criticalHitDamageDice;
        private bool _remarkableAthleteActive;

        // ==================== Eldritch Knight Properties ====================

        private SpellBookClass _wizardSpellBook;
        private List<string> _knownSpellsList;
        private List<string> _knownCantripsList;
        private int _spellcastingAbilityLevel;
        private bool _weaponBondReady;
        private string _bondedWeaponName;
        private bool _eldritchStrikeUsedThisRest;
        private int _spellcastingProficiencyBonus;
        private bool _warMagicAvailableThisTurn;
        private Dictionary<int, int> _spellSlotsByLevel;

        // ==================== Constructors ====================

        public Fighter() : base()
        {
            _subclass = FighterSubclass.BattleMaster;
            _subclassName = "Battle Master";
            _fightingStyle = FightingStyle.Dueling;
            _fightingStyleName = "Dueling";
            _secondWindUses = 1;
            _maxActionSurgeUses = 1;
            _actionSurgeUsesRemaining = 1;
            _indomitableUsedThisRest = false;
            _indomitableUsesRemaining = 0;
            _combatStyleDamageBonus = 2;
            _combatStyleAcBonus = 0;

            _superiorityDieSize = 4;
            _superiorityDiceCount = 2;
            _maneuversKnown = 4;
            _knownManeuvers = new List<string> { "PreciseAttack", "GoadingAttack", "Parry", "SweepingStrike" };
            _recoveryUsedThisRest = false;
            _superioritySurgeUsedThisCombat = false;

            _improvedCriticalThreshold = 20;
            _criticalHitDamageDice = 0;
            _remarkableAthleteActive = false;

            _wizardSpellBook = new SpellBookClass();
            _knownSpellsList = new List<string> { "Cure Wounds", "Magic Missile", "Shield" };
            _knownCantripsList = new List<string> { "Light", "Friends", "Mage Hand" };
            _spellcastingAbilityLevel = 0;
            _weaponBondReady = false;
            _eldritchStrikeUsedThisRest = false;
            _spellcastingProficiencyBonus = 2;
            _warMagicAvailableThisTurn = false;
            _spellSlotsByLevel = new Dictionary<int, int>();
        }

        public Fighter(
            string name,
            int level,
            DnDCharacterManager.Race.Race race,
            DnDCharacterManager.Background.Background background,
            FighterSubclass subclass = FighterSubclass.BattleMaster,
            FightingStyle fightingStyle = FightingStyle.Dueling)
            : base(name, level, race, background)
        {
            _subclass = subclass;
            _subclassName = GetSubclassName(subclass);
            _fightingStyle = fightingStyle;
            _fightingStyleName = GetFightingStyleName(fightingStyle);
            _secondWindUses = 1;
            _maxActionSurgeUses = 1;
            _actionSurgeUsesRemaining = 1;
            _indomitableUsedThisRest = false;
            _indomitableUsesRemaining = 0;
            _combatStyleDamageBonus = GetCombatStyleDamageBonus(fightingStyle);
            _combatStyleAcBonus = GetFightingStyleACBonus(fightingStyle);

            InitializeSubclassProperties();
            ApplyLevelFeatures();
        }

        // ==================== Properties ====================

        public int SecondWindUses { get => _secondWindUses; set => _secondWindUses = value; }
        public int MaxActionSurgeUses { get => _maxActionSurgeUses; }
        public int ActionSurgeUsesRemaining { get => _actionSurgeUsesRemaining; set => _actionSurgeUsesRemaining = value; }
        public FighterSubclass Subclass { get => _subclass; set { _subclass = value; _subclassName = GetSubclassName(value); } }
        public string SubclassName { get => _subclassName; set => _subclassName = value; }
        public FightingStyle FightingStyle { get => _fightingStyle; set { _fightingStyle = value; _fightingStyleName = GetFightingStyleName(value); UpdateCombatStyleBonuses(); } }
        public string FightingStyleName { get => _fightingStyleName; set => _fightingStyleName = value; }
        public int CombatStyleDamageBonus { get => _combatStyleDamageBonus; }
        public int IndomitableAvailable { get => _indomitableUsesRemaining; }

        public int SuperiorityDiceCount { get => _superiorityDiceCount; }
        public int SuperiorityDieSize { get => _superiorityDieSize; }
        public List<string> KnownManeuvers { get => _knownManeuvers; }
        public int ManeuversKnown { get => _maneuversKnown; }

        public int ImprovedCriticalThreshold { get => _improvedCriticalThreshold; }
        public int CriticalHitDamageDice { get => _criticalHitDamageDice; }

        public SpellBookClass WizardSpellBook { get => _wizardSpellBook; }
        public List<string> KnownSpells { get => _knownSpellsList; }
        public List<string> KnownCantrips { get => _knownCantripsList; }
        public int SpellcastingLevel { get => _spellcastingAbilityLevel; }
        public bool WeaponBondReady { get => _weaponBondReady; }
        public string BondedWeaponName { get => _bondedWeaponName; set => _bondedWeaponName = value; }
        public Dictionary<int, int> SpellSlotsByLevel { get => _spellSlotsByLevel; }

        // ==================== Core Methods ====================

        public override void ClassSpecificAbility()
        {
            ActionSurge();
        }

        public virtual void SecondWind()
        {
            if (_secondWindUses <= 0)
            {
                Console.WriteLine($"{Name} has no Second Wind uses remaining.");
                return;
            }

            int healAmount = new Random().Next(1, 11) + Level;
            _secondWindUses--;
            Heal(healAmount);
            Console.WriteLine($"{Name} uses Second Wind, healing for {healAmount} hit points! (1 use remaining)");
        }

        public virtual bool ActionSurge()
        {
            if (_actionSurgeUsesRemaining <= 0)
            {
                Console.WriteLine($"{Name} has no Action Surge uses remaining.");
                return false;
            }

            _actionSurgeUsesRemaining--;
            Console.WriteLine($"{Name} uses Action Surge and gains one additional action on their next turn!");
            if (_actionSurgeUsesRemaining >= 1)
            {
                Console.WriteLine($"({_actionSurgeUsesRemaining} use remaining)");
            }
            else
            {
                Console.WriteLine("(0 uses remaining)");
            }
            return true;
        }

        public void TakeExtraAction(Action extraAction)
        {
            extraAction?.Invoke();
        }

        public int GetAttackCount()
        {
            if (Level >= 19) return 4;
            if (Level >= 11) return 3;
            if (Level >= 5) return 2;
            return 1;
        }

        public virtual bool Indomitable()
        {
            if (Level < 10 || _indomitableUsesRemaining <= 0)
                return false;

            _indomitableUsesRemaining--;
            if (_indomitableUsesRemaining >= 1)
            {
                Console.WriteLine($"{Name} uses Indomitable Courage to reroll a failed saving throw! ({_indomitableUsesRemaining} use remaining)");
            }
            else
            {
                Console.WriteLine($"{Name} uses Indomitable Courage to reroll a failed saving throw! (0 uses remaining)");
            }
            return true;
        }

        // ==================== Subclass Methods ====================

        public virtual bool UseManeuver(string maneuverName, Action<string> effectAction)
        {
            if (_subclass != FighterSubclass.BattleMaster)
            {
                Console.WriteLine($"{Name} is not a Battle Master fighter.");
                return false;
            }

            if (_superiorityDiceCount <= 0)
            {
                Console.WriteLine($"{Name} has no superiority dice remaining.");
                return false;
            }

            if (!_knownManeuvers.Contains(maneuverName))
            {
                Console.WriteLine($"{Name} does not know the {maneuverName} maneuver.");
                return false;
            }

            _superiorityDiceCount--;
            effectAction?.Invoke(maneuverName);
            Console.WriteLine($"{Name} uses {maneuverName} maneuver! ({_superiorityDiceCount} superiority dice remaining, d{_superiorityDieSize})");
            return true;
        }

        public virtual bool Recovery()
        {
            if (_subclass != FighterSubclass.BattleMaster || Level < 7)
                return false;

            if (_recoveryUsedThisRest)
            {
                Console.WriteLine($"{Name} has already used Recovery since the last long rest.");
                return false;
            }

            int maxDice = GetMaxSuperiorityDice();
            if (_superiorityDiceCount >= maxDice)
            {
                Console.WriteLine($"{Name} already has maximum superiority dice.");
                return false;
            }

            int diceToRegain = Math.Min(maxDice - _superiorityDiceCount, 1);
            if (Level >= 18)
            {
                diceToRegain = Math.Min(maxDice - _superiorityDiceCount, 2);
            }

            _superiorityDiceCount = Math.Min(maxDice, _superiorityDiceCount + diceToRegain);
            _recoveryUsedThisRest = true;

            Console.WriteLine($"{Name} uses Recovery and regains {diceToRegain} superiority dice! ({_superiorityDiceCount} total)");
            return true;
        }

        public virtual int SuperioritySurge(int baseDamage)
        {
            if (_subclass != FighterSubclass.BattleMaster || Level < 15)
                return baseDamage;

            if (_superioritySurgeUsedThisCombat)
                return baseDamage;

            _superioritySurgeUsedThisCombat = true;
            int extraDamage = GetProficiencyBonusForLevel(Level);
            Console.WriteLine($"{Name} uses Superiority Surge! Nearby creatures take {extraDamage} damage!");
            return baseDamage + extraDamage;
        }

        public bool IsCriticalHit(int attackRoll)
        {
            return attackRoll >= _improvedCriticalThreshold;
        }

        public int GetRemarkableAthleteJumpDistance()
        {
            int conMod = Math.Max(-5, (Constitution - 10) / 2);
            return (MaxHitPoints / 2 + conMod);
        }

        public virtual bool SummonBondedWeapon()
        {
            if (_subclass != FighterSubclass.EldritchKnight || !_weaponBondReady)
            {
                Console.WriteLine($"{Name} does not have a bonded weapon.");
                return false;
            }

            Console.WriteLine($"{Name} uses Weapon Bond to summon {_bondedWeaponName}! (appears in empty space within 5 feet)");
            return true;
        }

        public virtual bool EldritchStrike()
        {
            if (_subclass != FighterSubclass.EldritchKnight || Level < 7)
                return false;

            if (_eldritchStrikeUsedThisRest)
            {
                Console.WriteLine($"{Name} has already used Eldritch Strike since the last long rest.");
                return false;
            }

            _eldritchStrikeUsedThisRest = true;
            Console.WriteLine($"{Name} uses Eldritch Strike! Target has disadvantage on saves against spells until end of your next turn!");
            return true;
        }

        public int GetMagicalAmbushInitiativeBonus(int initiativeRoll)
        {
            if (_subclass != FighterSubclass.EldritchKnight || Level < 13)
                return initiativeRoll;

            _spellcastingProficiencyBonus = GetProficiencyBonusForLevel(Level);
            Console.WriteLine($"{Name} uses Magical Ambush! +{_spellcastingProficiencyBonus} bonus to initiative!");
            return initiativeRoll + _spellcastingProficiencyBonus;
        }

        public virtual bool CanWarMagicCastCantrip()
        {
            if (_subclass != FighterSubclass.EldritchKnight || Level < 15)
                return false;

            _warMagicAvailableThisTurn = true;
            Console.WriteLine($"{Name} can cast a cantrip as an additional action due to War Magic!");
            return true;
        }

        public int GetSpellcastingAbilityModifier()
        {
            return Math.Max(-5, (Intelligence - 10) / 2);
        }

        public int GetSpellSaveDC()
        {
            return 8 + GetProficiencyBonusForLevel(Level) + GetSpellcastingAbilityModifier();
        }

        public virtual void LearnEldritchKnightSpell(SpellClass spell)
        {
            if (_subclass != FighterSubclass.EldritchKnight)
            {
                Console.WriteLine($"{Name} is not an Eldritch Knight.");
                return;
            }

            if (spell.Level == 0)
            {
                if (!_knownCantripsList.Contains(spell.Name))
                {
                    _knownCantripsList.Add(spell.Name);
                    Console.WriteLine($"{Name} has learned the cantrip: {spell.Name}");
                }
            }
            else
            {
                if (!_knownSpellsList.Contains(spell.Name))
                {
                    if (_knownSpellsList.Count >= GetMaxKnownSpells())
                    {
                        Console.WriteLine($"{Name} has reached the maximum number of known spells.");
                        return;
                    }
                    _knownSpellsList.Add(spell.Name);
                    Console.WriteLine($"{Name} has learned the spell: {spell.Name} (Level {spell.Level})");
                }
            }

            _wizardSpellBook.LearnSpell(spell);
        }

        public virtual bool CastEldritchKnightSpell(string spellName, int slotLevel)
        {
            if (_subclass != FighterSubclass.EldritchKnight)
                return false;

            SpellClass? spell = _wizardSpellBook.FindSpell(spellName);
            if (spell == null)
            {
                Console.WriteLine($"{Name} does not know the spell: {spellName}");
                return false;
            }

            if (slotLevel < spell.Level)
            {
                Console.WriteLine($"{Name} doesn't have a high enough spell slot for {spellName}.");
                return false;
            }

            if (!_spellSlotsByLevel.ContainsKey(slotLevel) || _spellSlotsByLevel[slotLevel] <= 0)
            {
                Console.WriteLine($"{Name} has no spell slots of level {slotLevel} remaining.");
                return false;
            }

            _spellSlotsByLevel[slotLevel]--;
            spell.Cast();
            int remaining = _spellSlotsByLevel[slotLevel];
            if (remaining == 1)
            {
                Console.WriteLine($"{Name} casts {spellName} using a level {slotLevel} spell slot! (1 slot of level {slotLevel} remaining)");
            }
            else
            {
                Console.WriteLine($"{Name} casts {spellName} using a level {slotLevel} spell slot! ({remaining} slots of level {slotLevel} remaining)");
            }
            return true;
        }

        public virtual void BondWeapon(string weaponName)
        {
            if (_subclass != FighterSubclass.EldritchKnight)
            {
                Console.WriteLine($"{Name} is not an Eldritch Knight.");
                return;
            }

            _bondedWeaponName = weaponName;
            _weaponBondReady = true;
            Console.WriteLine($"{Name} has bonded with the weapon: {weaponName}");
        }

        // ==================== Stat Calculations ====================

        protected override void CalculateBaseStats()
        {
            int conMod = Math.Max(-5, (Constitution - 10) / 2);

            int hpFromFirstLevel = 10 + conMod;
            int hpFromHigherLevels = (Level - 1) * (6 + conMod);
            MaxHitPoints = hpFromFirstLevel + hpFromHigherLevels;

            if (HitPoints > MaxHitPoints || HitPoints <= 0)
            {
                HitPoints = MaxHitPoints;
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

        private void ApplyLevelFeatures()
        {
            if (Level >= 17)
            {
                _maxActionSurgeUses = 2;
            }
            else
            {
                _maxActionSurgeUses = 1;
            }

            _actionSurgeUsesRemaining = _maxActionSurgeUses;

            if (Level >= 10)
            {
                if (Level >= 14)
                {
                    _indomitableUsesRemaining = 2;
                }
                else
                {
                    _indomitableUsesRemaining = 1;
                }
            }
            else
            {
                _indomitableUsesRemaining = 0;
            }

            if (_subclass == FighterSubclass.BattleMaster)
            {
                ApplyBattleMasterFeatures();
            }
            else if (_subclass == FighterSubclass.Champion)
            {
                ApplyChampionFeatures();
            }
            else if (_subclass == FighterSubclass.EldritchKnight)
            {
                ApplyEldritchKnightFeatures();
            }
        }

        private void ApplyBattleMasterFeatures()
        {
            if (Level >= 2)
            {
                _superiorityDieSize = 4;
                _superiorityDiceCount = 2;
                _maneuversKnown = Math.Min(4, MaxManeuversKnown);
            }

            if (Level >= 7)
            {
                _recoveryUsedThisRest = false;
            }

            if (Level >= 15)
            {
                _superioritySurgeUsedThisCombat = false;
            }

            if (Level >= 15)
            {
                _superiorityDieSize = 12;
            }
            else if (Level >= 7)
            {
                _superiorityDieSize = 8;
            }

            if (Level >= 18) _superiorityDiceCount = 10;
            else if (Level >= 15) _superiorityDiceCount = 8;
            else if (Level >= 12) _superiorityDiceCount = 7;
            else if (Level >= 9) _superiorityDiceCount = 6;
            else if (Level >= 6) _superiorityDiceCount = 5;
            else if (Level >= 3) _superiorityDiceCount = 4;
        }

        private void ApplyChampionFeatures()
        {
            if (Level >= 3)
            {
                _improvedCriticalThreshold = 19;
            }

            if (Level >= 7)
            {
                _remarkableAthleteActive = true;
            }

            if (Level >= 10)
            {
                _criticalHitDamageDice = 1;
            }

            if (Level >= 15)
            {
                _improvedCriticalThreshold = 17;
            }

            if (Level >= 20)
            {
                _criticalHitDamageDice = 2;
            }
        }

        private void ApplyEldritchKnightFeatures()
        {
            if (Level >= 3)
            {
                _spellcastingAbilityLevel = (int)Math.Floor(Level / 2.0);
                InitializeSpellSlots();
                _knownCantripsList = new List<string> { "Light", "Friends" };
                _knownSpellsList = new List<string> { "Cure Wounds", "Magic Missile", "Shield" };
            }

            if (Level >= 3)
            {
                _weaponBondReady = false;
            }

            if (Level >= 7)
            {
                _eldritchStrikeUsedThisRest = false;
            }

            if (Level >= 13)
            {
                _spellcastingProficiencyBonus = GetProficiencyBonusForLevel(Level);
            }

            if (Level >= 15)
            {
                _warMagicAvailableThisTurn = false;
            }

            UpdateSpellSlotsByFighterLevel();
        }

        private int GetMaxSuperiorityDice()
        {
            if (Level >= 18) return 10;
            if (Level >= 15) return 8;
            if (Level >= 12) return 7;
            if (Level >= 9) return 6;
            if (Level >= 6) return 5;
            return 4;
        }

        // ==================== Eldritch Knight Spell Progression ====================

        private void InitializeSpellSlots()
        {
            _spellSlotsByLevel = new Dictionary<int, int>();
            for (int i = 1; i <= 5; i++)
            {
                _spellSlotsByLevel[i] = 0;
            }
            UpdateSpellSlotsByFighterLevel();
        }

        private void UpdateSpellSlotsByFighterLevel()
        {
            if (_subclass != FighterSubclass.EldritchKnight)
                return;

            _spellSlotsByLevel.Clear();

            switch (Level)
            {
                case 1:
                case 2:
                    break;
                case 3:
                    _spellSlotsByLevel[1] = 2;
                    break;
                case 4:
                    _spellSlotsByLevel[1] = 3;
                    break;
                case 5:
                case 6:
                    _spellSlotsByLevel[1] = 4;
                    _spellSlotsByLevel[2] = 2;
                    break;
                case 7:
                case 8:
                    _spellSlotsByLevel[1] = 4;
                    _spellSlotsByLevel[2] = 3;
                    break;
                case 9:
                case 10:
                    _spellSlotsByLevel[1] = 4;
                    _spellSlotsByLevel[2] = 3;
                    _spellSlotsByLevel[3] = 2;
                    break;
                case 11:
                case 12:
                    _spellSlotsByLevel[1] = 4;
                    _spellSlotsByLevel[2] = 3;
                    _spellSlotsByLevel[3] = 3;
                    break;
                case 13:
                case 14:
                    _spellSlotsByLevel[1] = 4;
                    _spellSlotsByLevel[2] = 3;
                    _spellSlotsByLevel[3] = 3;
                    _spellSlotsByLevel[4] = 1;
                    break;
                case 15:
                case 16:
                    _spellSlotsByLevel[1] = 4;
                    _spellSlotsByLevel[2] = 3;
                    _spellSlotsByLevel[3] = 3;
                    _spellSlotsByLevel[4] = 2;
                    break;
                case >= 17:
                    _spellSlotsByLevel[1] = 4;
                    _spellSlotsByLevel[2] = 3;
                    _spellSlotsByLevel[3] = 3;
                    _spellSlotsByLevel[4] = 3;
                    _spellSlotsByLevel[5] = 1;
                    break;
            }
        }

        private int GetMaxKnownSpells()
        {
            if (Level >= 19) return 12;
            if (Level >= 16) return 11;
            if (Level >= 13) return 10;
            if (Level >= 10) return 9;
            if (Level >= 7) return 8;
            if (Level >= 4) return 7;
            if (Level >= 3) return 6;
            return 0;
        }

        private int GetMaxKnownCantrips()
        {
            if (Level >= 19) return 5;
            if (Level >= 13) return 4;
            if (Level >= 7) return 3;
            if (Level >= 3) return 2;
            return 0;
        }

        // ==================== Fighting Style Methods ====================

        private void UpdateCombatStyleBonuses()
        {
            switch (_fightingStyle)
            {
                case FightingStyle.Dueling:
                    _combatStyleDamageBonus = 2;
                    _combatStyleAcBonus = 0;
                    break;
                case FightingStyle.Defensive:
                    _combatStyleDamageBonus = 0;
                    _combatStyleAcBonus = 1;
                    break;
                case FightingStyle.TwoWeaponFighting:
                    _combatStyleDamageBonus = 0;
                    _combatStyleAcBonus = 0;
                    break;
                case FightingStyle.GreatWeaponFighting:
                    _combatStyleDamageBonus = 0;
                    _combatStyleAcBonus = 0;
                    break;
                case FightingStyle.Protection:
                    _combatStyleDamageBonus = 0;
                    _combatStyleAcBonus = 0;
                    break;
                case FightingStyle.ThrownWeapon:
                    _combatStyleDamageBonus = 0;
                    _combatStyleAcBonus = 0;
                    break;
                case FightingStyle.RangedWeapon:
                    _combatStyleDamageBonus = 2;
                    _combatStyleAcBonus = 0;
                    break;
            }
        }

        public int GetFightingStyleDamageBonus() => _combatStyleDamageBonus;
        public int GetFightingStyleACBonus() => _combatStyleAcBonus;

        // ==================== Override Base Methods ====================

        public override void Attack()
        {
            int attackCount = GetAttackCount();
            string plural = attackCount > 1 ? "s" : "";

            if (_fightingStyle == FightingStyle.Dueling)
            {
                Console.WriteLine($"{Name} attacks with Dueling fighting style! (+{_combatStyleDamageBonus} damage)");
            }
            else if (_fightingStyle == FightingStyle.Defensive)
            {
                Console.WriteLine($"{Name} attacks defensively! (+{_combatStyleAcBonus} AC while armored)");
            }

            Console.WriteLine($"{Name} makes {attackCount} attack{plural} with Extra Attack!");

            if (_fightingStyle == FightingStyle.GreatWeaponFighting && Level >= 1)
            {
                Console.WriteLine("Great Weapon Fighting: You can reroll damage dice that show 1 or 2.");
            }
        }

        public override void TakeDamage(int damage)
        {
            base.TakeDamage(damage);
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

            if (_subclass == FighterSubclass.BattleMaster && Level == 7)
            {
                Console.WriteLine("Battle Master: Recovery feature gained!");
            }
            else if (_subclass == FighterSubclass.BattleMaster && Level == 15)
            {
                Console.WriteLine("Battle Master: Superiority Surge feature gained!");
            }
            else if (_subclass == FighterSubclass.Champion && Level == 10)
            {
                if (_criticalHitDamageDice == 1)
                {
                    Console.WriteLine($"Champion: Critical Mastery gained! Now dealing +{_criticalHitDamageDice} critical hit damage die!");
                }
                else
                {
                    Console.WriteLine($"Champion: Critical Mastery gained! Now dealing +{_criticalHitDamageDice} critical hit damage dice!");
                }
            }
            else if (_subclass == FighterSubclass.Champion && Level == 15)
            {
                Console.WriteLine("Champion: Improved Critical threshold improved to 17-20!");
            }
            else if (_subclass == FighterSubclass.EldritchKnight && Level == 7)
            {
                Console.WriteLine("Eldritch Knight: Eldritch Strike feature gained!");
            }
            else if (_subclass == FighterSubclass.EldritchKnight && Level == 13)
            {
                Console.WriteLine("Eldritch Knight: Magical Ambush feature gained!");
            }
            else if (_subclass == FighterSubclass.EldritchKnight && Level == 15)
            {
                Console.WriteLine("Eldritch Knight: War Magic feature gained!");
            }
        }

        public override void LongRest()
        {
            base.LongRest();

            _secondWindUses = 1;
            _actionSurgeUsesRemaining = _maxActionSurgeUses;

            if (Level >= 14)
            {
                _indomitableUsesRemaining = 2;
            }
            else if (Level >= 10)
            {
                _indomitableUsesRemaining = 1;
            }

            if (_subclass == FighterSubclass.BattleMaster)
            {
                _recoveryUsedThisRest = false;
                _superioritySurgeUsedThisCombat = false;
            }

            if (_subclass == FighterSubclass.EldritchKnight)
            {
                _eldritchStrikeUsedThisRest = false;
                UpdateSpellSlotsByFighterLevel();
            }

            Console.WriteLine($"{Name} recovers all abilities after a long rest.");
        }

        public override void ShortRest()
        {
            base.ShortRest();

            _secondWindUses = 1;
            _actionSurgeUsesRemaining = _maxActionSurgeUses;

            if (_subclass == FighterSubclass.EldritchKnight)
            {
                UpdateSpellSlotsByFighterLevel();
            }

            Console.WriteLine($"{Name} recovers Second Wind and Action Surge on a short rest.");
        }

        public override void DisplayCharacter()
        {
            string subclassPrefix = GetSubclassName(_subclass);

            Console.WriteLine();
            Console.WriteLine($"=== {Name} (Level {_level} Fighter - {subclassPrefix}) ===");
            Console.WriteLine($"Hit Points: {HitPoints}/{MaxHitPoints} | AC: {ArmorClass} | Speed: {Speed}");
            Console.WriteLine($"Fighting Style: {_fightingStyleName}");

            Console.WriteLine();
            Console.WriteLine("--- Fighter Abilities ---");
            Console.WriteLine($"Second Wind: {_secondWindUses}/1 (bonus action, 1d10 + level HP)");
            Console.WriteLine($"Action Surge: {_actionSurgeUsesRemaining}/{_maxActionSurgeUses} (one extra action, rests on short/long rest)");

            int attackCount = GetAttackCount();
            Console.WriteLine($"Extra Attack: {attackCount} attacks per turn");

            if (Level >= 10)
            {
                if (_indomitableUsesRemaining == 1)
                {
                    Console.WriteLine($"Indomitable: {_indomitableUsesRemaining} use/rest (reroll saving throw)");
                }
                else
                {
                    Console.WriteLine($"Indomitable: {_indomitableUsesRemaining} uses/rest (reroll saving throw)");
                }
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
            Console.WriteLine();
        }

        private void DisplaySubclassInfo()
        {
            Console.WriteLine();
            Console.WriteLine("--- Subclass Features ---");

            switch (_subclass)
            {
                case FighterSubclass.BattleMaster:
                    Console.WriteLine("Battle Master:");
                    Console.WriteLine($"  Superiority Dice: {_superiorityDiceCount}d{_superiorityDieSize}");
                    Console.WriteLine($"  Maneuvers Known: {_maneuversKnown}/Max Maneuvers: {MaxManeuversKnown}");
                    Console.WriteLine("  Known Maneuvers: " + string.Join(", ", _knownManeuvers));
                    if (Level >= 7)
                    {
                        if (_recoveryUsedThisRest)
                        {
                            Console.WriteLine("  Recovery: used (bonus action to regain superiority dice)");
                        }
                        else
                        {
                            Console.WriteLine("  Recovery: available (bonus action to regain superiority dice)");
                        }
                    }
                    if (Level >= 15)
                    {
                        if (_superioritySurgeUsedThisCombat)
                        {
                            Console.WriteLine("  Superiority Surge: used in combat");
                        }
                        else
                        {
                            Console.WriteLine("  Superiority Surge: available in combat");
                        }
                    }
                    break;

                case FighterSubclass.Champion:
                    Console.WriteLine("Champion:");
                    string critRange;
                    if (_improvedCriticalThreshold == 20)
                    {
                        critRange = "19-20";
                    }
                    else if (_improvedCriticalThreshold == 17)
                    {
                        critRange = "17-20";
                    }
                    else
                    {
                        critRange = "15-20";
                    }
                    Console.WriteLine($"  Improved Critical: Crit on {critRange}");
                    if (_criticalHitDamageDice == 1)
                    {
                        Console.WriteLine($"  Critical Hit Damage Bonus: +{_criticalHitDamageDice} damage die");
                    }
                    else
                    {
                        Console.WriteLine($"  Critical Hit Damage Bonus: +{_criticalHitDamageDice} damage dice");
                    }
                    if (_remarkableAthleteActive)
                    {
                        int jumpDist = GetRemarkableAthleteJumpDistance();
                        Console.WriteLine($"  Remarkable Athlete: Can jump {jumpDist} feet (HP/2 + Con mod in feet)");
                    }
                    break;

                case FighterSubclass.EldritchKnight:
                    Console.WriteLine("Eldritch Knight:");
                    Console.WriteLine($"  Spellcasting Level: {_spellcastingAbilityLevel}");
                    if (_knownSpellsList.Count > 0)
                    {
                        Console.WriteLine($"  Spells Known: {string.Join(", ", _knownSpellsList)}");
                    }
                    else
                    {
                        Console.WriteLine("  Spells Known: None");
                    }
                    if (_knownCantripsList.Count > 0)
                    {
                        Console.WriteLine($"  Cantrips Known: {string.Join(", ", _knownCantripsList)}");
                    }
                    else
                    {
                        Console.WriteLine("  Cantrips Known: None");
                    }
                    if (_weaponBondReady)
                    {
                        Console.WriteLine($"  Bonded Weapon: {_bondedWeaponName}");
                    }
                    if (Level >= 7 && !_eldritchStrikeUsedThisRest)
                    {
                        Console.WriteLine("  Eldritch Strike: Available");
                    }
                    if (Level >= 13)
                    {
                        Console.WriteLine($"  Magical Ambush: +{_spellcastingProficiencyBonus} to initiative");
                    }
                    if (Level >= 15)
                    {
                        Console.WriteLine("  War Magic: Available when using Action Surge");
                    }
                    if (_spellSlotsByLevel.Count > 0)
                    {
                        Console.WriteLine("  Spell Slots:");
                        foreach (var slot in _spellSlotsByLevel)
                        {
                            if (slot.Value > 0)
                            {
                                if (slot.Value == 1)
                                {
                                    Console.WriteLine($"    Level {slot.Key}: {slot.Value} slot");
                                }
                                else
                                {
                                    Console.WriteLine($"    Level {slot.Key}: {slot.Value} slots");
                                }
                            }
                        }
                    }
                    break;
            }
        }

        // ==================== Helper Methods ====================

        private string GetSubclassName(FighterSubclass subclass)
        {
            switch (subclass)
            {
                case FighterSubclass.BattleMaster: return "Battle Master";
                case FighterSubclass.Champion: return "Champion";
                case FighterSubclass.EldritchKnight: return "Eldritch Knight";
                default: return "Unknown";
            }
        }

        private string GetFightingStyleName(FightingStyle style)
        {
            switch (style)
            {
                case FightingStyle.TwoWeaponFighting: return "Two-Weapon Fighting";
                case FightingStyle.Dueling: return "Dueling";
                case FightingStyle.GreatWeaponFighting: return "Great Weapon Fighting";
                case FightingStyle.Protection: return "Protection";
                case FightingStyle.Defensive: return "Defensive";
                case FightingStyle.ThrownWeapon: return "Thrown Weapon";
                case FightingStyle.RangedWeapon: return "Ranged Weapon";
                default: return "Unknown";
            }
        }

        private int GetCombatStyleDamageBonus(FightingStyle style)
        {
            switch (style)
            {
                case FightingStyle.Dueling: return 2;
                case FightingStyle.RangedWeapon: return 2;
                default: return 0;
            }
        }

        private int GetFightingStyleACBonus(FightingStyle style)
        {
            switch (style)
            {
                case FightingStyle.Defensive: return 1;
                default: return 0;
            }
        }

        private void InitializeSubclassProperties()
        {
            if (_subclass == FighterSubclass.BattleMaster)
            {
                ApplyBattleMasterFeatures();
            }
            else if (_subclass == FighterSubclass.Champion)
            {
                ApplyChampionFeatures();
            }
            else if (_subclass == FighterSubclass.EldritchKnight)
            {
                InitializeEldritchKnightProperties();
            }
        }

        private void InitializeEldritchKnightProperties()
        {
            _wizardSpellBook = new SpellBookClass(Name);
            _knownSpellsList = new List<string>();
            _knownCantripsList = new List<string>();
            _spellSlotsByLevel = new Dictionary<int, int>();

            if (Level >= 3)
            {
                _spellcastingAbilityLevel = (int)Math.Floor(Level / 2.0);
                InitializeSpellSlots();
            }
        }

        // ==================== Inner Feature Classes ====================

        /// <summary>
        /// Second Wind feature - Fighter level 1.
        /// </summary>
        public class SecondWindFeature : DnDCharacterManager.Feature.Feature
        {
            public SecondWindFeature() : base("Second Wind", "As a bonus action, regain hit points equal to 1d10 + fighter level.")
            {
            }

            public override void UseFeature()
            {
                Console.WriteLine("Second Wind: As a bonus action, regain 1d10 + fighter level hit points.");
            }
        }

        /// <summary>
        /// Action Surge feature - Fighter level 1.
        /// </summary>
        public class ActionSurgeFeature : DnDCharacterManager.Feature.Feature
        {
            protected int _extraActions;

            public ActionSurgeFeature() : base("Action Surge", "Gain one additional action on your turn. Rests on short/long rest.")
            {
                _extraActions = 1;
            }

            public int ExtraActions { get => _extraActions; set => _extraActions = value; }

            public override void UseFeature()
            {
                Console.WriteLine($"Action Surge: Gain one additional action on the current turn! ({_extraActions} extra action(s))");
            }
        }

        /// <summary>
        /// Martial Adept feature - Battle Master level 2.
        /// </summary>
        public class MartialAdeptFeature : DnDCharacterManager.Feature.Feature
        {
            protected int _learningBonus;

            public MartialAdeptFeature() : base("Martial Adept", "Gain superiority dice and learn combat maneuvers.")
            {
                _learningBonus = 2;
            }

            public int LearningBonus { get => _learningBonus; set => _learningBonus = value; }

            public override void UseFeature()
            {
                Console.WriteLine("Martial Adept: Gain superiority dice and learn combat maneuvers.");
            }
        }

        /// <summary>
        /// Combat Superiority feature - Battle Master level 3.
        /// </summary>
        public class CombatSuperiorityFeature : DnDCharacterManager.Feature.Feature
        {
            protected int _diceCount;
            protected int _dieSize;

            public CombatSuperiorityFeature() : base("Combat Superiority", "Roll superiority dice when using maneuvers.")
            {
                _diceCount = 4;
                _dieSize = 4;
            }

            public int DiceCount { get => _diceCount; set => _diceCount = value; }
            public int DieSize { get => _dieSize; set => _dieSize = value; }

            public override void UseFeature()
            {
                Console.WriteLine($"Combat Superiority: {_diceCount}d{_dieSize} superiority dice available.");
            }
        }

        /// <summary>
        /// Recovery feature - Battle Master level 7.
        /// </summary>
        public class RecoveryFeature : DnDCharacterManager.Feature.Feature
        {
            public RecoveryFeature() : base("Recovery", "As a bonus action, regain HP equal to one superiority die.")
            {
            }

            public override void UseFeature()
            {
                Console.WriteLine("Recovery: As a bonus action, regain HP equal to one superiority die.");
            }
        }

        /// <summary>
        /// Improved Critical feature - Champion level 3.
        /// </summary>
        public class ImprovedCriticalFeature : DnDCharacterManager.Feature.Feature
        {
            protected int _criticalThreshold;

            public ImprovedCriticalFeature() : base("Improved Critical", "Crit on 19-20 (improves at level 15).")
            {
                _criticalThreshold = 19;
            }

            public int CriticalThreshold { get => _criticalThreshold; set => _criticalThreshold = value; }

            public bool IsCriticalHit(int roll)
            {
                return roll >= _criticalThreshold;
            }

            public override void UseFeature()
            {
                Console.WriteLine($"Improved Critical: Critical hit on rolls of {_criticalThreshold}-20!");
            }
        }

        /// <summary>
        /// Remarkable Athlete feature - Champion level 7.
        /// </summary>
        public class RemarkableAthleteFeature : DnDCharacterManager.Feature.Feature
        {
            public RemarkableAthleteFeature() : base("Remarkable Athlete", "Add half fighter level to ability checks and increase jump distance.")
            {
            }

            public override void UseFeature()
            {
                Console.WriteLine("Remarkable Athlete: Add half fighter level to ability checks and increase jump distance.");
            }
        }

        /// <summary>
        /// Critical Mastery feature - Champion level 10.
        /// </summary>
        public class CriticalMasteryFeature : DnDCharacterManager.Feature.Feature
        {
            protected int _extraDamageDice;

            public CriticalMasteryFeature() : base("Critical Mastery", "Roll extra damage die(s) on critical hits.")
            {
                _extraDamageDice = 1;
            }

            public int ExtraDamageDice { get => _extraDamageDice; set => _extraDamageDice = value; }

            public override void UseFeature()
            {
                Console.WriteLine($"Critical Mastery: Roll {_extraDamageDice} additional damage die(s) on critical hits!");
            }
        }

        /// <summary>
        /// Weapon Bond feature - Eldritch Knight level 3.
        /// </summary>
        public class WeaponBondFeature : DnDCharacterManager.Feature.Feature
        {
            protected string _bondedWeapon;

            public WeaponBondFeature() : base("Weapon Bond", "Bond with a weapon and summon it as a bonus action.")
            {
                _bondedWeapon = "Unnamed Weapon";
            }

            public string BondedWeapon { get => _bondedWeapon; set => _bondedWeapon = value; }

            public override void UseFeature()
            {
                Console.WriteLine($"Weapon Bond: {_bondedWeapon} teleports to your hand as a bonus action!");
            }
        }

        /// <summary>
        /// Eldritch Strike feature - Eldritch Knight level 7.
        /// </summary>
        public class EldritchStrikeFeature : DnDCharacterManager.Feature.Feature
        {
            public EldritchStrikeFeature() : base("Eldritch Strike", "Target has disadvantage on saves vs your spells until end of your next turn!")
            {
            }

            public override void UseFeature()
            {
                Console.WriteLine("Eldritch Strike: Target has disadvantage on saving throws against your spells until end of your next turn!");
            }
        }

        /// <summary>
        /// Magical Ambush feature - Eldritch Knight level 13.
        /// </summary>
        public class MagicalAmbushFeature : DnDCharacterManager.Feature.Feature
        {
            public MagicalAmbushFeature() : base("Magical Ambush", "Add proficiency bonus to initiative and spell damage!")
            {
            }

            public override void UseFeature()
            {
                Console.WriteLine("Magical Ambush: Add proficiency bonus to initiative and spell damage!");
            }
        }

        /// <summary>
        /// War Magic feature - Eldritch Knight level 15.
        /// </summary>
        public class WarMagicFeature : DnDCharacterManager.Feature.Feature
        {
            public WarMagicFeature() : base("War Magic", "Cast a cantrip with the extra action from Action Surge!")
            {
            }

            public override void UseFeature()
            {
                Console.WriteLine("War Magic: Cast a cantrip with the extra action from Action Surge!");
            }
        }

        /// <summary>
        /// Superiority Surge feature - Battle Master level 15.
        /// </summary>
        public class SuperioritySurgeFeature : DnDCharacterManager.Feature.Feature
        {
            public SuperioritySurgeFeature() : base("Superiority Surge", "Adjacent creatures take proficiency bonus damage when your maneuver hits!")
            {
            }

            public override void UseFeature()
            {
                Console.WriteLine("Superiority Surge: Adjacent creatures take proficiency bonus damage when your maneuver hits!");
            }
        }
    }
}