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
    /// Enum representing the three Sorcerer subclasses (Sorcerery Lines).
    /// </summary>
    public enum SorcererSubclass
    {
        DraconicBloodline,
        WildMagic,
        AberrantMind
    }

    /// <summary>
    /// Enum for Draconic Bloodline resistance elements.
    /// </summary>
    public enum DraconicElement
    {
        Fire,
        Cold,
        Acid,
        Lightning,
        Poison
    }

    /// <summary>
    /// Enum for Wild Magic Surge outcomes.
    /// </summary>
    public enum WildSurgeOutcome
    {
        Stable,
        ArcaneJetstream,
        Aging,
        BeastForm,
        FireRisk,
        GreaseSurge,
        HealingTide,
        Teleport,
        SizeChange,
        ElementalBurst
    }

    /// <summary>
    /// Enum for Metamagic options available to Sorcerers.
    /// </summary>
    public enum MetamagicOption
    {
        DistantSpell,
        EmpoweredSpell,
        QuickenSpell,
        SubtleSpell,
        TwinnedSpell
    }

    /// <summary>
    /// Sorcerer character class with full level 1-20 feature progression and all PHB subclasses.
    /// Implements D&D 5e rules by the book for Spellcasting, Wild Magic, and Metamagic.
    /// </summary>
    public class Sorcerer : Character
    {
        // ==================== Core Sorcerer Properties ====================

        // Spellcasting (full caster, CHA-based)
        protected Dictionary<int, int> _spellSlotsByLevel;
        protected List<SpellClass> _knownSpells;
        protected List<string> _cantripsKnown;
        protected int _spellsKnownCount;

        // Metamagic and sorcery points
        private int _metapoints;
        private bool _fontOfMagicUnlocked;
        private bool _flexibleSorceryUnlocked;
        private List<MetamagicOption> _activeMetamagic;
        private string _empoweredSpellAbility = "Charisma";

        // Wild Magic tracking
        private bool _wildSurgeActive;
        private Random _wildSurgeRandom;
        private bool _tidesOfChaosActive;
        private bool _tidesUsedThisCombat;
        private bool _controlledChaosUnlocked;

        // Subclass tracking
        private SorcererSubclass _subclass;
        private string _subclassName;

        // ==================== Draconic Bloodline Properties ====================

        private DraconicElement _draconicElement;
        private bool _dragonWingsActive;
        private int _wingsRemainingMinutes;
        private bool _draconicPresenceUnlocked;
        private bool _elementalAffinityUnlocked;
        private string _resistanceType;

        // ==================== Aberrant Mind Properties ====================

        private bool _psionicSoulActive;
        private bool _mindMendingUsed;
        private bool _telekineticProjectionUnlocked;
        private int _psiDieValue;

        // ==================== Constructors ====================

        public Sorcerer() : base()
        {
            _subclass = SorcererSubclass.WildMagic;
            _subclassName = "Wild Magic";
            _wildSurgeActive = true;
            _wildSurgeRandom = new Random();

            InitializeSorcerer();
        }

        public Sorcerer(
            string name,
            int level,
            RaceClass race,
            BackgroundClass background,
            SorcererSubclass subclass = SorcererSubclass.WildMagic)
            : base(name, level, race, background)
        {
            _subclass = subclass;
            _subclassName = GetSubclassName(subclass);
            _wildSurgeActive = (subclass == SorcererSubclass.WildMagic);
            _wildSurgeRandom = new Random();

            InitializeSorcerer();
            ApplyLevelFeatures();
        }

        private void InitializeSorcerer()
        {
            // Spellcasting defaults
            _spellSlotsByLevel = new Dictionary<int, int>();
            _knownSpells = new List<SpellClass>();
            _cantripsKnown = new List<string>();
            _spellsKnownCount = 0;

            // Metamagic and sorcery points defaults
            _metapoints = 0;
            _fontOfMagicUnlocked = false;
            _flexibleSorceryUnlocked = false;
            _activeMetamagic = new List<MetamagicOption>();
            _empoweredSpellAbility = "Charisma";

            // Wild Magic defaults
            _wildSurgeActive = (_subclass == SorcererSubclass.WildMagic);
            _tidesOfChaosActive = false;
            _tidesUsedThisCombat = false;
            _controlledChaosUnlocked = false;

            // Draconic Bloodline defaults
            _draconicElement = DraconicElement.Fire;
            _dragonWingsActive = false;
            _wingsRemainingMinutes = 0;
            _draconicPresenceUnlocked = false;
            _elementalAffinityUnlocked = false;
            _resistanceType = "Fire";

            // Aberrant Mind defaults
            _psionicSoulActive = (_subclass == SorcererSubclass.AberrantMind);
            _mindMendingUsed = false;
            _telekineticProjectionUnlocked = false;
            _psiDieValue = 1;

            // Apply level-based initialization
            if (Level >= 2) _fontOfMagicUnlocked = true;
            if (Level >= 20) _flexibleSorceryUnlocked = true;

            UpdateMetapoints();
            AddDefaultStartingSpells();
        }

        // ==================== Properties ====================

        public Dictionary<int, int> SpellSlotsByLevel { get => _spellSlotsByLevel; set => _spellSlotsByLevel = value; }
        public List<SpellClass> KnownSpells { get => _knownSpells; set => _knownSpells = value; }
        public List<string> CantripsKnown { get => _cantripsKnown; set => _cantripsKnown = value; }
        public int SpellsKnownCount { get => _spellsKnownCount; }
        public int MetaPoints { get => _metapoints; }
        public bool FontOfMagicUnlocked { get => _fontOfMagicUnlocked; }
        public bool FlexibleSorceryUnlocked { get => _flexibleSorceryUnlocked; }
        public List<MetamagicOption> ActiveMetamagic { get => new List<MetamagicOption>(_activeMetamagic); }
        public bool WildSurgeActive { get => _wildSurgeActive; set => _wildSurgeActive = value; }
        public bool TidesOfChaosActive { get => _tidesOfChaosActive; }
        public bool ControlledChaosUnlocked { get => _controlledChaosUnlocked; }
        public SorcererSubclass Subclass { get => _subclass; set { _subclass = value; _subclassName = GetSubclassName(value); } }
        public string SubclassName { get => _subclassName; }

        // Draconic Bloodline properties
        public DraconicElement DraconicElement { get => _draconicElement; set { _draconicElement = value; _resistanceType = GetElementName(value); } }
        public string ResistanceType { get => _resistanceType; }
        public bool DragonWingsActive { get => _dragonWingsActive; }

        // Aberrant Mind properties
        public bool PsionicSoulActive { get => _psionicSoulActive; }
        public int PsiDieValue { get => _psiDieValue; }

        // ==================== Core Methods ====================

        public override void ClassSpecificAbility()
        {
            if (_subclass == SorcererSubclass.WildMagic && _wildSurgeActive)
            {
                TriggerWildMagicSurge();
            }
            else
            {
                Console.WriteLine($"{Name} channels their wild magic, preparing for the next spell cast.");
            }
        }

        /// <summary>
        /// Get the sorcerer's spell save DC.
        /// Formula: 8 + Proficiency Bonus + Charisma modifier
        /// </summary>
        public int GetSpellSaveDC()
        {
            int chaMod = Math.Max(-5, (Charisma - 10) / 2);
            int proficiencyBonus = GetProficiencyBonusForLevel(Level);
            return 8 + proficiencyBonus + chaMod;
        }

        /// <summary>
        /// Get the sorcerer's spell attack bonus.
        /// Formula: Proficiency Bonus + Charisma modifier
        /// </summary>
        public int GetSpellAttackBonus()
        {
            int chaMod = Math.Max(-5, (Charisma - 10) / 2);
            return GetProficiencyBonusForLevel(Level) + chaMod;
        }

        /// <summary>
        /// Cast a known spell using a spell slot. For Wild Magic sorcerers, 
        /// there's a chance to trigger a Wild Magic Surge.
        /// </summary>
        public virtual bool CastSpell(string spellName, int slotLevel)
        {
            SpellClass? spell = _knownSpells.Find(s => s.Name == spellName);
            if (spell == null)
            {
                Console.WriteLine($"{Name} does not know the spell: {spellName}");
                return false;
            }

            if (!_spellSlotsByLevel.ContainsKey(slotLevel) || _spellSlotsByLevel[slotLevel] <= 0)
            {
                Console.WriteLine($"{Name} has no spell slots of level {slotLevel}.");
                return false;
            }

            _spellSlotsByLevel[slotLevel]--;
            Console.WriteLine($"{Name} casts {spellName} using a {slotLevel}-level spell slot.");

            // Apply Elemental Affinity bonus for Draconic Bloodline
            if (_subclass == SorcererSubclass.DraconicBloodline && _elementalAffinityUnlocked)
            {
                ApplyElementalAffinity(spell);
            }

            // Check for Wild Magic Surge
            if (_subclass == SorcererSubclass.WildMagic && _wildSurgeActive)
            {
                // Use Tides of Chaos advantage to avoid surge
                if (_tidesOfChaosActive && !_tidesUsedThisCombat)
                {
                    _tidesOfChaosActive = false;
                    _tidesUsedThisCombat = true;
                    Console.WriteLine($"{Name} uses Tides of Chaos to avoid a Wild Magic Surge!");
                }
                else
                {
                    CheckForWildMagicSurge();
                }
            }

            return true;
        }

        /// <summary>
        /// Cast a cantrip (no spell slot required).
        /// </summary>
        public virtual bool CastCantrip(string cantripName)
        {
            if (!_cantripsKnown.Contains(cantripName))
            {
                Console.WriteLine($"{Name} does not know the cantrip: {cantripName}");
                return false;
            }

            // Cantrips have no resource cost, just flavor text
            int attackBonus = GetSpellAttackBonus();
            Console.WriteLine($"{Name} casts {cantripName} (attack bonus: +{attackBonus})!");
            return true;
        }

        /// <summary>
        /// Gain sorcery points from Font of Magic feature.
        /// Sorcerers gain sorcery points equal to their sorcerer level.
        /// </summary>
        public virtual void GainSorceryPoints()
        {
            UpdateMetapoints();
            Console.WriteLine($"{Name} now has {_metapoints} sorcery points.");
        }

        /// <summary>
        /// Convert spell slots into sorcery points (Magical Restoration).
        /// Uses the DMG p.50 "Spell Slot to Sorcery Points" table.
        /// </summary>
        public virtual int ConvertSpellToSorceryPoints(int slotLevel)
        {
            if (!_fontOfMagicUnlocked)
            {
                Console.WriteLine($"{Name} has not learned Font of Magic yet (requires level 2).");
                return 0;
            }

            if (!_spellSlotsByLevel.ContainsKey(slotLevel) || _spellSlotsByLevel[slotLevel] <= 0)
            {
                Console.WriteLine($"{Name} has no spell slots of level {slotLevel} to convert.");
                return 0;
            }

            _spellSlotsByLevel[slotLevel]--;

            // Standard DMG p.50: "Spell Slot to Sorcery Points" table:
            // 1st = 2, 2nd = 3, 3rd = 5, 4th = 6, 5th = 7, 6th = 9, 7th = 11, 8th = 13, 9th = 15
            int pointsGained = GetSorceryPointsForSlot(slotLevel);
            _metapoints += pointsGained;
            Console.WriteLine($"{Name} converts a {slotLevel}-level spell slot into {pointsGained} sorcery points.");
            return pointsGained;
        }

        /// <summary>
        /// Use sorcery points to recover a spell slot (Magical Restoration reverse).
        /// Costs sorcery points equal to the slot's level.
        /// </summary>
        public virtual bool RecoverSpellSlot(int slotLevel)
        {
            if (!_fontOfMagicUnlocked)
            {
                Console.WriteLine($"{Name} has not learned Font of Magic yet (requires level 2).");
                return false;
            }

            int cost = slotLevel;
            if (_metapoints < cost)
            {
                Console.WriteLine($"{Name} does not have enough sorcery points (needs {cost}, has {_metapoints}).");
                return false;
            }

            _metapoints -= cost;
            if (!_spellSlotsByLevel.ContainsKey(slotLevel))
            {
                _spellSlotsByLevel[slotLevel] = 0;
            }
            _spellSlotsByLevel[slotLevel]++;
            Console.WriteLine($"{Name} recovers a {slotLevel}-level spell slot for {cost} sorcery points.");
            return true;
        }

        /// <summary>
        /// Add a metamagic option to an active spell.
        /// </summary>
        public virtual bool AddMetamagic(MetamagicOption metamagic, int metaPointsCost = 1)
        {
            if (_activeMetamagic.Count >= 2) // Max 2 metamagics per spell at higher levels
            {
                Console.WriteLine($"{Name} already has 2 metamagic options active. Can only have 2 per spell.");
                return false;
            }

            _activeMetamagic.Add(metamagic);
            _metapoints -= metaPointsCost;
            Console.WriteLine($"{Name} applies {GetMetamagicName(metamagic)} metamagic! (Cost: {metaPointsCost} sorcery points)");
            return true;
        }

        /// <summary>
        /// Use Swift Spell metamagic to cast a spell as a bonus action (level 8+).
        /// </summary>
        public virtual bool SwiftSpell(string spellName, int slotLevel)
        {
            if (Level < 8)
            {
                Console.WriteLine($"{Name} has not learned Swift Spell yet (requires level 8).");
                return false;
            }

            if (!_activeMetamagic.Contains(MetamagicOption.QuickenSpell))
            {
                Console.WriteLine($"{Name} does not have Quicken Spell metamagic active.");
                return false;
            }

            // Cast the spell and consume the slot
            bool success = CastSpell(spellName, slotLevel);
            if (success)
            {
                Console.WriteLine($"{Name} casts {spellName} as a BONUS ACTION using Quicken Spell!");
                _activeMetamagic.Remove(MetamagicOption.QuickenSpell);
            }
            return success;
        }

        // ==================== Draconic Bloodline Methods ====================

        /// <summary>
        /// Apply damage resistance based on the chosen draconic element.
        /// </summary>
        public virtual string GetDamageResistance()
        {
            if (_subclass != SorcererSubclass.DraconicBloodline)
            {
                return "None";
            }

            Console.WriteLine($"{Name} has resistance to {_resistanceType} damage.");
            return _resistanceType;
        }

        /// <summary>
        /// Activate draconian wings for flight (level 6, usable once per long rest).
        /// </summary>
        public virtual bool ActivateWings()
        {
            if (_subclass != SorcererSubclass.DraconicBloodline)
            {
                Console.WriteLine($"{Name} does not have draconic wings.");
                return false;
            }

            if (_dragonWingsActive)
            {
                Console.WriteLine($"{Name}'s wings are already active. They will last for 1 hour.");
                return true;
            }

            _dragonWingsActive = true;
            _wingsRemainingMinutes = 60;
            Console.WriteLine($"{Name}'s wings burst forth! Flight speed: 30 feet for 1 hour.");
            return true;
        }

        /// <summary>
        /// Deactivate draconian wings.
        /// </summary>
        public virtual void DeactivateWings()
        {
            if (_dragonWingsActive)
            {
                _dragonWingsActive = false;
                _wingsRemainingMinutes = 0;
                Console.WriteLine($"{Name}'s wings fade away.");
            }
        }

        /// <summary>
        /// Use Draconic Presence to frighten creatures (level 14, once per long rest).
        /// </summary>
        public virtual bool UseDraconicPresence(int conSaveDC, int numCreatures)
        {
            if (!_draconicPresenceUnlocked)
            {
                Console.WriteLine($"{Name} has not learned Draconic Presence yet (requires level 14).");
                return false;
            }

            Console.WriteLine($"{Name} unleashes their draconic presence! Up to {numCreatures} creatures must make a DC {conSaveDC} WIS save or become frightened for 1 minute.");
            return true;
        }

        /// <summary>
        /// Apply Elemental Affinity: add CHA modifier to one damage roll of matching element (level 14).
        /// </summary>
        private void ApplyElementalAffinity(SpellClass spell)
        {
            if (!_elementalAffinityUnlocked) return;

            int chaMod = Math.Max(-5, (Charisma - 10) / 2);
            Console.WriteLine($"{Name} uses Elemental Affinity! Add +{chaMod} damage to {_resistanceType} spells.");
        }

        // ==================== Wild Magic Methods ====================

        /// <summary>
        /// Check for Wild Magic Surge when casting a spell (1 in 6 chance).
        /// </summary>
        private void CheckForWildMagicSurge()
        {
            if (!_wildSurgeActive || _subclass != SorcererSubclass.WildMagic) return;

            // 1 in 6 chance (rolling d6, need 6)
            int roll = _wildSurgeRandom.Next(1, 7);
            if (roll == 6)
            {
                TriggerWildMagicSurge();
            }
            else
            {
                Console.WriteLine($"Wild Magic check: rolled {roll} (no surge).");
            }
        }

        /// <summary>
        /// Trigger a Wild Magic Surge effect using the PHB d128 table.
        /// </summary>
        public virtual void TriggerWildMagicSurge()
        {
            int surgeRoll = _wildSurgeRandom.Next(0, 128); // d128 per PHB table

            Console.WriteLine("*** WILD MAGIC SURGE! *** (Roll: " + surgeRoll + ")");

            // Simplified Wild Magic Surge table based on PHB Appendix S
            switch (surgeRoll)
            {
                case 0:
                case 1:
                    Console.WriteLine("You teleport up to 30 feet to an unoccupied space you can see.");
                    break;
                case 2:
                case 3:
                    Console.WriteLine("A spectral copy of you appears and follows your movements for 1 minute.");
                    break;
                case 4:
                case 5:
                    Console.WriteLine("Grass instantly grows in a 10-foot radius where you stand. It is overgrown wilderness.");
                    break;
                case 6:
                case 7:
                    Console.WriteLine("You can cast Confusion as an action.");
                    break;
                case 8:
                case 9:
                    Console.WriteLine("Each creature within 30 feet must make a DC 15 CON save or be unable to attack you for 1 minute.");
                    break;
                case 10:
                case 11:
                    Console.WriteLine("Your size changes randomly (roll on random size table).");
                    break;
                case 12:
                case 13:
                    Console.WriteLine("A wave of sonic energy erupts from you. Each creature within 10 feet takes 2d8 thunder damage.");
                    break;
                case 14:
                case 15:
                    Console.WriteLine("You regain 4d8 hit points, but your skin turns blue until you finish a long rest.");
                    break;
                case 16:
                case 17:
                    Console.WriteLine("A burst of elemental energy deals 1d6 damage to you (cannot be prevented).");
                    break;
                default:
                    // Default generic surge effect for remaining rolls
                    int genericRoll = _wildSurgeRandom.Next(0, 5);
                    switch (genericRoll)
                    {
                        case 0:
                            Console.WriteLine("Your spells become unpredictable for 1 minute. You have disadvantage on spell attack rolls.");
                            break;
                        case 1:
                            Console.WriteLine("A gust of wind blows you in a random direction up to 30 feet.");
                            break;
                        case 2:
                            Console.WriteLine("Sparks fly from your fingertips for 1 minute. You can produce flickers of light as a bonus action.");
                            break;
                        case 3:
                            Console.WriteLine("Your voice becomes echoing for 1 hour. Creatures within 5 feet have advantage on Persuasion checks against you.");
                            break;
                        default:
                            Console.WriteLine("You feel a strange energy surge, but nothing happens.");
                            break;
                    }
                    break;
            }
        }

        /// <summary>
        /// Activate Tides of Chaos: Advantage on the next spell attack roll (level 6, once per combat).
        /// </summary>
        public virtual bool ActivateTidesOfChaos()
        {
            if (_subclass != SorcererSubclass.WildMagic)
            {
                Console.WriteLine($"{Name} does not have Tides of Chaos.");
                return false;
            }

            if (_tidesUsedThisCombat && !_controlledChaosUnlocked)
            {
                Console.WriteLine($"{Name} has already used Tides of Chaos this combat.");
                return false;
            }

            _tidesOfChaosActive = true;
            _tidesUsedThisCombat = true;
            Console.WriteLine($"{Name} uses Tides of Chaos! Advantage on the next spell attack roll!");
            return true;
        }

        // ==================== Aberrant Mind Methods ====================

        /// <summary>
        /// Use Psionic Spellcasting: cast spells using Intelligence instead of Charisma (level 6+).
        /// </summary>
        public virtual bool SwitchToPsionicSpellcasting(bool enable)
        {
            if (_subclass != SorcererSubclass.AberrantMind)
            {
                Console.WriteLine($"{Name} does not have Psionic Soul.");
                return false;
            }

            _psionicSoulActive = enable;
            string action = enable ? "activated" : "deactivated";
            Console.WriteLine($"{Name}'s psionic spellcasting has been {action}.");
            return true;
        }

        /// <summary>
        /// Use Mind Mending to heal hit points with sorcery points (level 6+).
        /// </summary>
        public virtual int MindMending(int numPsiDice)
        {
            if (_subclass != SorcererSubclass.AberrantMind)
            {
                Console.WriteLine($"{Name} does not have Psionic Soul.");
                return 0;
            }

            if (numPsiDice > _metapoints)
            {
                Console.WriteLine($"{Name} does not have enough sorcery points for Mind Mending.");
                return 0;
            }

            _metapoints -= numPsiDice;
            int healing = numPsiDice * 2; // Each sorcery point = 2 HP healed
            _hitPoints = Math.Min(_maxHitPoints, _hitPoints + healing);
            Console.WriteLine($"{Name} uses Mind Mending, healing {healing} hit points for {numPsiDice} sorcery points.");
            return healing;
        }

        /// <summary>
        /// Telekinetic Shift: as a reaction when hit by an attack, push creature up to 15 feet (level 20+).
        /// </summary>
        public virtual bool TelekineticShift(int distance)
        {
            if (!_telekineticProjectionUnlocked)
            {
                Console.WriteLine($"{Name} has not learned Telekinetic Projection yet (requires level 20).");
                return false;
            }

            distance = Math.Min(distance, 15);
            Console.WriteLine($"{Name} uses Telekinetic Projection to push a creature up to {distance} feet away!");
            return true;
        }

        // ==================== Stat Calculations ====================

        /// <summary>
        /// Override base stats calculation for Sorcerer.
        /// Hit Points: d6 per level, AC: Dex-based (none by default), Speed: 30 ft
        /// </summary>
        protected override void CalculateBaseStats()
        {
            int conMod = Math.Max(-5, (Constitution - 10) / 2);

            // Max HP: d6 per level + con mod per level
            int hpFromFirstLevel = 6 + conMod;
            int hpFromHigherLevels = (Level - 1) * (6 + conMod);
            MaxHitPoints = hpFromFirstLevel + hpFromHigherLevels;

            if (HitPoints > MaxHitPoints || HitPoints <= 0)
            {
                HitPoints = MaxHitPoints;
            }

            // No armor proficiency by default (AC = 10 + Dex mod if unarmored, but sorcerers do not get Unarmored Defense)
            // Default to wearing no armor
            int dexMod = Math.Max(-5, (Dexterity - 10) / 2);
            ArmorClass = 10 + dexMod;

            // Speed defaults to race speed (typically 30 ft)
            Speed = _race != null ? _race.Speed : 30;

            Console.WriteLine($"{Name}'s stats calculated: HP {MaxHitPoints}, AC {ArmorClass}");
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
            // ===== LEVEL 1: Spellcasting + Wild Magic =====
            if (Level >= 1)
            {
                InitializeSpellSlots(1);
                AddDefaultStartingSpells();
                _spellsKnownCount = 0;
            }

            // ===== LEVEL 2: Font of Magic =====
            if (Level >= 2)
            {
                _fontOfMagicUnlocked = true;
                InitializeSpellSlots(2);
                UpdateMetapoints();
                _spellsKnownCount = 2;
            }

            // ===== LEVEL 3: Subclass Feature + Metamagic =====
            if (Level >= 3)
            {
                ApplySubclassFeatures();
                InitializeSpellSlots(3);
                AddStartingMetamagicOptions();
                _spellsKnownCount = 4;
            }

            // ===== LEVEL 4: ASI OR Metamagic Option =====
            if (Level >= 4)
            {
                InitializeSpellSlots(4);
                _spellsKnownCount = 5;
            }

            // ===== LEVEL 5: Additional Cantrip + Spells Known =====
            if (Level >= 5)
            {
                _cantripsKnown.Add("MageHand"); // Placeholder, player chooses actual cantrip
                InitializeSpellSlots(5);
                UpdateMetapoints();
                _spellsKnownCount = 6;
            }

            // ===== LEVEL 6: Subclass Feature 2 =====
            if (Level >= 6)
            {
                ApplySubclassFeature2();
                AddCantripIfNeeded(true);
                InitializeSpellSlots(6);
                _spellsKnownCount = 7;
            }

            // ===== LEVEL 7: Additional Cantrip =====
            if (Level >= 7)
            {
                AddCantripIfNeeded(false);
                InitializeSpellSlots(7);
                _spellsKnownCount = 8;
            }

            // ===== LEVEL 8: ASI + Extra Metamagic Option =====
            if (Level >= 8)
            {
                InitializeSpellSlots(8);
                UpdateMetapoints();
                AddExtraMetamagicOption();
                _spellsKnownCount = 9;
            }

            // ===== LEVEL 9: Maximum Spell Slot Level 5 =====
            if (Level >= 9)
            {
                InitializeSpellSlots(9);
                _spellsKnownCount = 10;
            }

            // ===== LEVEL 10: Subclass Feature 3 + Skill Proficiency =====
            if (Level >= 10)
            {
                ApplySubclassFeature3();
                InitializeSpellSlots(10);
                UpdateMetapoints();
                _spellsKnownCount = 11;
            }

            // ===== LEVEL 11: Max Spell Slot Level 6 =====
            if (Level >= 11)
            {
                InitializeSpellSlots(11);
                _spellsKnownCount = 12;
            }

            // ===== LEVEL 12: ASI =====
            if (Level >= 12)
            {
                InitializeSpellSlots(12);
                _spellsKnownCount = 13;
            }

            // ===== LEVEL 14: Subclass Feature 4 =====
            if (Level >= 14)
            {
                ApplySubclassFeature4();
                InitializeSpellSlots(14);
                UpdateMetapoints();
                _spellsKnownCount = 14;
            }

            // ===== LEVEL 15: Max Spell Slot Level 8 =====
            if (Level >= 15)
            {
                InitializeSpellSlots(15);
                _spellsKnownCount = 15;
            }

            // ===== LEVEL 16: ASI =====
            if (Level >= 16)
            {
                InitializeSpellSlots(16);
                UpdateMetapoints();
                _spellsKnownCount = 17;
            }

            // ===== LEVEL 18: Extra Metamagic Option (3 total) =====
            if (Level >= 18)
            {
                AddExtraMetamagicOption();
                InitializeSpellSlots(18);
                UpdateMetapoints();
                _spellsKnownCount = 19;
            }

            // ===== LEVEL 19: ASI + Additional Metamagic Option =====
            if (Level >= 19)
            {
                InitializeSpellSlots(19);
                AddExtraMetamagicOption();
                UpdateMetapoints();
                _spellsKnownCount = 20;
            }

            // ===== LEVEL 20: Subclass Ultimate Feature =====
            if (Level >= 20)
            {
                ApplyUltimateFeature();
                InitializeSpellSlots(20);
                UpdateMetapoints();
                _spellsKnownCount = 22;
            }
        }

        private void ApplySubclassFeatures()
        {
            switch (_subclass)
            {
                case SorcererSubclass.DraconicBloodline:
                    _subclassName = "Draconic Bloodline";
                    Console.WriteLine($"{Name} chooses the Draconic Bloodline!");
                    Console.WriteLine("  - Damage Resistance to a draconic element");
                    break;

                case SorcererSubclass.WildMagic:
                    _subclassName = "Wild Magic";
                    Console.WriteLine($"{Name} chooses the Wild Magic subclass!");
                    Console.WriteLine("  - Wild Magic Surge: Random effects when casting spells");
                    break;

                case SorcererSubclass.AberrantMind:
                    _subclassName = "Aberrant Mind";
                    Console.WriteLine($"{Name} chooses the Aberrant Mind subclass!");
                    Console.WriteLine("  - Psionic Soul: Cast spells using Intelligence");
                    break;

                default:
                    _subclassName = "Wild Magic";
                    break;
            }
        }

        private void ApplySubclassFeature2()
        {
            switch (_subclass)
            {
                case SorcererSubclass.DraconicBloodline:
                    Console.WriteLine("Draconic Bloodline Feature 2: Draconic Wings - Flight speed of 30 feet for 1 hour.");
                    break;

                case SorcererSubclass.WildMagic:
                    Console.WriteLine("Wild Magic Feature 2: Tides of Chaos - Advantage on next spell attack after casting a spell.");
                    _tidesOfChaosActive = true;
                    break;

                case SorcererSubclass.AberrantMind:
                    Console.WriteLine("Aberrant Mind Feature 2: Psionic Spellcasting - Can use Intelligence for spellcasting. Psi Die: d4.");
                    _psiDieValue = 4;
                    break;

                default:
                    break;
            }
        }

        private void ApplySubclassFeature3()
        {
            switch (_subclass)
            {
                case SorcererSubclass.DraconicBloodline:
                    Console.WriteLine("Draconic Bloodline Feature 3: Dragon Wings - Use bonus action to grow wings (usable 1/short rest).");
                    break;

                case SorcererSubclass.WildMagic:
                    Console.WriteLine("Wild Magic Feature 3: Controlled Chaos - Can choose to activate/deactivate Tides of Chaos.");
                    _controlledChaosUnlocked = true;
                    break;

                case SorcererSubclass.AberrantMind:
                    Console.WriteLine("Aberrant Mind Feature 3: Psionic Power. Psi Die increases to d6.");
                    _psiDieValue = 6;
                    break;

                default:
                    break;
            }
        }

        private void ApplySubclassFeature4()
        {
            switch (_subclass)
            {
                case SorcererSubclass.DraconicBloodline:
                    Console.WriteLine("Draconic Bloodline Feature 4: Elemental Affinity - Add CHA mod to damage rolls of your element.");
                    _elementalAffinityUnlocked = true;
                    break;

                case SorcererSubclass.WildMagic:
                    Console.WriteLine("Wild Magic Feature 4: Burst of Fanfare - As a bonus action, trigger a random surge effect without rolling.");
                    break;

                case SorcererSubclass.AberrantMind:
                    Console.WriteLine("Aberrant Mind Feature 4: Psionic Teleportation. Psi Die increases to d8. Can use Action to teleport 10 feet.");
                    _psiDieValue = 8;
                    break;

                default:
                    break;
            }
        }

        private void ApplyUltimateFeature()
        {
            switch (_subclass)
            {
                case SorcererSubclass.DraconicBloodline:
                    Console.WriteLine("Draconic Bloodline Ultimate: Dragon Soul - Resistance to all damage types for 1 minute once per long rest.");
                    break;

                case SorcererSubclass.WildMagic:
                    Console.WriteLine("Wild Magic Ultimate: Boundless Surge - Every spell cast triggers a surge, but you choose the effect.");
                    break;

                case SorcererSubclass.AberrantMind:
                    Console.WriteLine("Aberrant Mind Ultimate: Psionic Breakthrough. Psi Die increases to d12. Use bonus action to teleport 60 feet.");
                    _psiDieValue = 12;
                    _telekineticProjectionUnlocked = true;
                    break;

                default:
                    break;
            }
        }

        // ==================== Metamagic Methods ====================

        private void AddStartingMetamagicOptions()
        {
            Console.WriteLine($"{Name} can choose 2 starting Metamagic options: Distant Spell, Empowered Spell, Quicken Spell, Subtle Spell, Twinned Spell.");
            // Default selection - player chooses in actual game
            _activeMetamagic.Add(MetamagicOption.DistantSpell);
            _activeMetamagic.Add(MetamagicOption.SubtleSpell);
            Console.WriteLine("Default selection: Distant Spell and Subtle Spell.");
        }

        private void AddExtraMetamagicOption()
        {
            if (_activeMetamagic.Count >= 5) return; // Max metamagic options

            // Add a random metamagic option not already known
            MetamagicOption[] allOptions = (MetamagicOption[])Enum.GetValues(typeof(MetamagicOption));
            List<MetamagicOption> availableOptions = new List<MetamagicOption>();

            foreach (var option in allOptions)
            {
                if (!_activeMetamagic.Contains(option))
                {
                    availableOptions.Add(option);
                }
            }

            if (availableOptions.Count > 0)
            {
                int selectedIndex = _wildSurgeRandom.Next(availableOptions.Count);
                MetamagicOption newOption = availableOptions[selectedIndex];
                _activeMetamagic.Add(newOption);
                Console.WriteLine($"{Name} learns a new Metamagic option: {GetMetamagicName(newOption)}!");
            }
        }

        private string GetMetamagicName(MetamagicOption option)
        {
            switch (option)
            {
                case MetamagicOption.DistantSpell: return "Distant";
                case MetamagicOption.EmpoweredSpell: return "Empowered";
                case MetamagicOption.QuickenSpell: return "Quicken";
                case MetamagicOption.SubtleSpell: return "Subtle";
                case MetamagicOption.TwinnedSpell: return "Twinned";
                default: return "Unknown";
            }
        }

        // ==================== Spellcasting Methods ====================

        /// <summary>
        /// Get the sorcery points cost for converting a spell slot.
        /// Uses DMG p.50 table values.
        /// </summary>
        private int GetSorceryPointsForSlot(int slotLevel)
        {
            switch (slotLevel)
            {
                case 1: return 2;
                case 2: return 3;
                case 3: return 5;
                case 4: return 6;
                case 5: return 7;
                case 6: return 9;
                case 7: return 11;
                case 8: return 13;
                case 9: return 15;
                default: return slotLevel * 2;
            }
        }

        /// <summary>
        /// Initialize spell slots based on the sorcerer's current level using full caster progression.
        /// </summary>
        private void InitializeSpellSlots(int level)
        {
            _spellSlotsByLevel.Clear();

            // Full caster spell slot progression (per D&D 5e PHB p.201 - Spellcaster table)
            if (level >= 1 && level <= 4)
            {
                if (level == 1) { _spellSlotsByLevel[1] = 2; }
                else if (level == 2) { _spellSlotsByLevel[1] = 3; }
                else if (level == 3) { _spellSlotsByLevel[1] = 4; }
                else if (level >= 4) { _spellSlotsByLevel[1] = 4; }
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

            // Ensure all spell slots up to max level exist
            int maxSpellSlotLevel = Math.Min(9, (level - 1) / 2);
            if (maxSpellSlotLevel < 1) maxSpellSlotLevel = 1;

            for (int i = 1; i <= maxSpellSlotLevel; i++)
            {
                if (!_spellSlotsByLevel.ContainsKey(i))
                {
                    _spellSlotsByLevel[i] = 0;
                }
            }
        }

        /// <summary>
        /// Add default starting spells at level 1.
        /// </summary>
        private void AddDefaultStartingSpells()
        {
            // Standard level 1 Sorcerer starting cantrips (player chooses 4 from list)
            List<string> startingCantrips = new List<string>
            {
                "AcidSplash", "ChaosBolt", "LightningBolt", "Firebolt"
            };

            foreach (var cantrip in startingCantrips)
            {
                _cantripsKnown.Add(cantrip);
            }

            Console.WriteLine($"{Name} starts with cantrips: {string.Join(", ", startingCantrips)}");
        }

        /// <summary>
        /// Add a cantrip if needed (level 5 and level 7).
        /// </summary>
        private void AddCantripIfNeeded(bool shouldAdd)
        {
            if (shouldAdd)
            {
                List<string> additionalCantrips = new List<string>
                {
                    "MageHand", "MinorIllusion", "Prestidigitation", "RayOfFrost"
                };

                // Pick one not already known
                foreach (var cantrip in additionalCantrips)
                {
                    if (!_cantripsKnown.Contains(cantrip))
                    {
                        _cantripsKnown.Add(cantrip);
                        Console.WriteLine($"{Name} learns a new cantrip: {cantrip}");
                        break;
                    }
                }
            }
        }

        private void UpdateMetapoints()
        {
            // Sorcery points = sorcerer level
            _metapoints = Level;
        }

        public int GetTotalSpellSlots()
        {
            int total = 0;
            foreach (var slot in _spellSlotsByLevel)
            {
                total += slot.Value;
            }
            return total;
        }

        // ==================== Override Base Methods ====================

        public override void Attack()
        {
            int attackBonus = GetSpellAttackBonus();
            Console.WriteLine($"{Name} (Level {_level} {_subclassName}) makes a spell attack!");
            Console.WriteLine("  Spell attack bonus: +" + attackBonus);
            Console.WriteLine("Sorcerers excel at direct damage spells (Fireball, Lightning Bolt) and buffing allies.");
        }

        public override void TakeDamage(int damage)
        {
            // Apply Draconic Resistance if applicable
            int finalDamage = damage;
            if (_subclass == SorcererSubclass.DraconicBloodline)
            {
                Console.WriteLine($"{Name}'s draconic heritage grants {_resistanceType} resistance!");
                finalDamage = damage / 2;
            }

            base.TakeDamage(finalDamage);
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

            UpdateMetapoints();

            // Add metamagic option at certain levels
            if (Level == 4 || Level == 8 || Level == 18 || Level == 19)
            {
                AddExtraMetamagicOption();
            }

            ApplyLevelFeatures();
            DisplayLevelFeatures();
        }

        public override void LongRest()
        {
            base.LongRest();

            // Reset daily features
            _tidesUsedThisCombat = false;
            _tidesOfChaosActive = false;
            _mindMendingUsed = false;
            _dragonWingsActive = false;
            _wingsRemainingMinutes = 0;

            // Recover spell slots and sorcery points
            Console.WriteLine($"{Name} recovers all spell slots and sorcery points after a long rest.");
            UpdateMetapoints();
        }

        public override void ShortRest()
        {
            base.ShortRest();

            // Font of Magic: Recover some spell slots on short rest (level 2+)
            if (_fontOfMagicUnlocked)
            {
                Console.WriteLine($"{Name} can use Font of Magic to recover spell slots during a short rest.");
            }
        }

        public override void DisplayCharacter()
        {
            Console.WriteLine();
            Console.WriteLine("=== " + Name + " (Level " + _level + " Sorcerer - " + _subclassName + ") ===");
            Console.WriteLine("Hit Points: " + HitPoints + "/" + MaxHitPoints + " | AC: " + ArmorClass + " | Speed: " + Speed);
            Console.WriteLine("Ability Scores:");
            Console.WriteLine("  Strength:    " + Strength + " (mod +" + Math.Max(-5, (Strength - 10) / 2) + ")");
            Console.WriteLine("  Dexterity:   " + Dexterity + " (mod +" + Math.Max(-5, (Dexterity - 10) / 2) + ")");
            Console.WriteLine("  Constitution:" + Constitution + " (mod +" + Math.Max(-5, (Constitution - 10) / 2) + ")");
            Console.WriteLine("  Intelligence:" + Intelligence + " (mod +" + Math.Max(-5, (Intelligence - 10) / 2) + ")");
            Console.WriteLine("  Wisdom:      " + Wisdom + " (mod +" + Math.Max(-5, (Wisdom - 10) / 2) + ")");
            Console.WriteLine("  Charisma:    " + Charisma + " (mod +" + Math.Max(-5, (Charisma - 10) / 2) + ")");

            Console.WriteLine();
            Console.WriteLine("--- Sorcerer Spellcasting ---");
            Console.WriteLine("Spell Save DC: " + GetSpellSaveDC() + " | Spell Attack Bonus: +" + GetSpellAttackBonus());
            Console.WriteLine("Spells Known: " + _spellsKnownCount + " | Cantrips Known: " + _cantripsKnown.Count);
            Console.WriteLine("Spell Slots: " + GetTotalSpellSlots() + " total");

            Console.WriteLine();
            Console.WriteLine("--- Metamagic ---");
            Console.WriteLine("Sorcery Points: " + _metapoints);
            string metamagicList = _activeMetamagic.Count > 0 ? string.Join(", ", _activeMetamagic.ConvertAll(x => GetMetamagicName(x))) : "None";
            Console.WriteLine("Active Metamagic: " + metamagicList);

            Console.WriteLine();
            Console.WriteLine("--- Subclass Features ---");

            switch (_subclass)
            {
                case SorcererSubclass.DraconicBloodline:
                    Console.WriteLine("  Draconic Resistance: " + _resistanceType);
                    string wingsStatus = _dragonWingsActive ? "Active" : "Inactive";
                    Console.WriteLine("  Dragon Wings: " + wingsStatus);
                    if (_elementalAffinityUnlocked)
                        Console.WriteLine("  Elemental Affinity: +CHA mod to damage rolls");
                    if (_draconicPresenceUnlocked)
                        Console.WriteLine("  Draconic Presence: Fear aura");
                    break;

                case SorcererSubclass.WildMagic:
                    string surgeStatus = _wildSurgeActive ? "Active" : "Inactive";
                    Console.WriteLine("  Wild Magic Surge: " + surgeStatus);
                    if (_tidesOfChaosActive)
                        Console.WriteLine("  Tides of Chaos: Active (advantage on next spell attack)");
                    if (_controlledChaosUnlocked)
                        Console.WriteLine("  Controlled Chaos: Can toggle Tides of Chaos");
                    break;

                case SorcererSubclass.AberrantMind:
                    string psionicStatus = _psionicSoulActive ? "Using Intelligence" : "Using Charisma";
                    Console.WriteLine("  Psionic Soul: " + psionicStatus);
                    Console.WriteLine("  Psi Die: d" + _psiDieValue);
                    if (_telekineticProjectionUnlocked)
                        Console.WriteLine("  Telekinetic Projection: Bonus action teleport");
                    break;
            }

            Console.WriteLine();
            Console.WriteLine("--- Known Spells ---");
            Console.WriteLine("Cantrips: " + string.Join(", ", _cantripsKnown));
            Console.WriteLine("1st Level: ");
            foreach (var spell in _knownSpells)
            {
                if (spell.Level == 1)
                    Console.WriteLine("  - " + spell.Name);
            }

            Console.WriteLine();
            Console.WriteLine("=== End Character Sheet ===");
        }

        private void DisplayLevelFeatures()
        {
            Console.WriteLine();
            Console.WriteLine("--- Gained Features at Level " + Level + " ---");

            if (Level == 2)
            {
                Console.WriteLine("  Font of Magic: Sorcery points and Magical Restoration");
            }
            else if (Level == 3)
            {
                Console.WriteLine("  Subclass Feature: " + _subclassName);
                Console.WriteLine("  Metamagic: Choose 2 metamagic options");
            }
            else if (Level == 6)
            {
                Console.WriteLine("  Subclass Feature 2 unlocked!");
            }
            else if (Level == 8)
            {
                Console.WriteLine("  Swift Spell: Cast spells as bonus action");
            }
            else if (Level == 10)
            {
                Console.WriteLine("  Subclass Feature 3 + Skill Proficiency");
            }
            else if (Level == 14)
            {
                Console.WriteLine("  Subclass Feature 4 unlocked!");
            }
            else if (Level == 20)
            {
                Console.WriteLine("  Subclass Ultimate Feature unlocked!");
                ApplyUltimateFeature();
            }

            Console.WriteLine();
        }

        private string GetSubclassName(SorcererSubclass subclass)
        {
            switch (subclass)
            {
                case SorcererSubclass.DraconicBloodline: return "Draconic Bloodline";
                case SorcererSubclass.WildMagic: return "Wild Magic";
                case SorcererSubclass.AberrantMind: return "Aberrant Mind";
                default: return "Wild Magic";
            }
        }

        private string GetElementName(DraconicElement element)
        {
            switch (element)
            {
                case DraconicElement.Fire: return "Fire";
                case DraconicElement.Cold: return "Cold";
                case DraconicElement.Acid: return "Acid";
                case DraconicElement.Lightning: return "Lightning";
                case DraconicElement.Poison: return "Poison";
                default: return "Fire";
            }
        }
    }
}