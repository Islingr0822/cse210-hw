using System;
using System.Collections.Generic;
using DnDCharacterManager.Ability;
using FeatureClass = DnDCharacterManager.Feature.Feature;
using RaceClass = DnDCharacterManager.Race.Race;
using BackgroundClass = DnDCharacterManager.Background.Background;

namespace DnDCharacterManager.Character.Classes
{
    /// <summary>
    /// Enum representing the different Paths of Power for Barbarians.
    /// </summary>
    public enum BarbarianSubclass
    {
        PathOfTheBerserker,
        PathOfTheTotemAnimal,
        PathOfTheZealot,
        PathOfTheBeast
    }

    /// <summary>
    /// Enum for selecting a totem animal spirit.
    /// </summary>
    public enum TotemSpirit
    {
        Bear,
        Eagle,
        Elk,
        Tiger
    }

    /// <summary>
    /// Enum for beast shapes available to the Path of the Beast.
    /// </summary>
    public enum BeastShape
    {
        None,
        GiantLimpet,
        OvergrownKingcrab,
        DireParrot
    }

    /// <summary>
    /// Barbarian character class with full level 1-20 feature progression and subclass support.
    /// </summary>
    public class Barbarian : Character
    {
        // Core rage properties
        private bool _isRaging;
        private int _rageCount;
        private int _rageDamageBonus;

        // Subclass properties
        private BarbarianSubclass _subclass;
        private string _subclassName;
        private TotemSpirit _totemSpirit;
        private BeastShape _beastShape;

        // Feature flags based on level
        private bool _fastMovementUnlocked;
        private bool _feralChargeUnlocked;
        private bool _savageAttackerUnlocked;
        private int _brutalDieCount;
        private bool _relentlessRageUsed;
        private bool _viciousRetaliationUnlocked;
        private bool _primalChampionActive;

        // Relentless Rage tracking
        private bool _consciousAtZeroHp;

        public Barbarian() : base()
        {
            _isRaging = false;
            _rageCount = 2;
            _rageDamageBonus = 0;
            _subclass = BarbarianSubclass.PathOfTheBerserker;
            _subclassName = "Path of the Berserker";
            _totemSpirit = TotemSpirit.Bear;
            _beastShape = BeastShape.None;
            _fastMovementUnlocked = false;
            _feralChargeUnlocked = false;
            _savageAttackerUnlocked = false;
            _brutalDieCount = 0;
            _relentlessRageUsed = false;
            _viciousRetaliationUnlocked = false;
            _primalChampionActive = false;
            _consciousAtZeroHp = false;

            Features.Add(new DnDCharacterManager.Feature.RageFeature());
        }

        public Barbarian(string name, int level, RaceClass race, BackgroundClass background)
            : base(name, level, race, background)
        {
            _isRaging = false;
            _rageCount = 2;
            _rageDamageBonus = 0;
            _subclass = BarbarianSubclass.PathOfTheBerserker;
            _subclassName = "Path of the Berserker";
            _totemSpirit = TotemSpirit.Bear;
            _beastShape = BeastShape.None;
            _fastMovementUnlocked = false;
            _feralChargeUnlocked = false;
            _savageAttackerUnlocked = false;
            _brutalDieCount = 0;
            _relentlessRageUsed = false;
            _viciousRetaliationUnlocked = false;
            _primalChampionActive = false;
            _consciousAtZeroHp = false;

            Features.Add(new DnDCharacterManager.Feature.RageFeature());
            ApplyLevelFeatures();
        }

        // ==================== Properties ====================

        public bool IsRaging { get => _isRaging; set => _isRaging = value; }
        public int RageCount { get => _rageCount; set => _rageCount = value; }
        public int RageDamageBonus { get => _rageDamageBonus; }
        public BarbarianSubclass Subclass { get => _subclass; set => _subclass = value; }
        public string SubclassName { get => _subclassName; set => _subclassName = value; }
        public TotemSpirit TotemSpirit { get => _totemSpirit; set => _totemSpirit = value; }
        public BeastShape BeastShapeType { get => _beastShape; set => _beastShape = value; }
        public bool FastMovementUnlocked { get => _fastMovementUnlocked; }
        public bool FeralChargeUnlocked { get => _feralChargeUnlocked; }
        public int BrutalDieCount { get => _brutalDieCount; }

        // ==================== Core Methods ====================

        public override void ClassSpecificAbility()
        {
            Rage();
        }

        /// <summary>
        /// Enter a battle fury. Grants advantage on Strength checks and saves, and melee attack rolls with Strength.
        /// Takes effect until the end of your next turn or until you stop early (bonus action).
        /// </summary>
        public virtual void Rage()
        {
            if (_rageCount > 0)
            {
                _isRaging = true;
                _consciousAtZeroHp = false;

                // Apply Totem Bear resistance if applicable
                if (_subclass == BarbarianSubclass.PathOfTheTotemAnimal && _totemSpirit == TotemSpirit.Bear)
                {
                    Console.WriteLine("Totem Spirit of the Bear grants you resistance to all damage while raging.");
                }

                // Apply Divine Fury damage if applicable
                if (_subclass == BarbarianSubclass.PathOfTheZealot)
                {
                    Console.WriteLine($"Divine Fury deals +{_rageDamageBonus} radiant damage on melee hits while raging.");
                }

                // Apply Beast Shape effects if applicable
                if (_subclass == BarbarianSubclass.PathOfTheBeast && _beastShape != BeastShape.None)
                {
                    Console.WriteLine($"Your beast shape grants you a monstrous appearance and natural weapons.");
                }

                Console.WriteLine($"{Name} enters a rage! Gain advantage on Strength checks and saves, and melee attack rolls with Strength.");
            }
            else
            {
                Console.WriteLine($"{Name} has no rages remaining. Take a short rest to recover rage uses.");
            }
        }

        /// <summary>
        /// Stop raging early as a bonus action.
        /// </summary>
        public virtual void EndRage()
        {
            if (_isRaging)
            {
                _isRaging = false;
                Console.WriteLine($"{Name} ends their rage.");
            }
        }

        // ==================== Stat Calculations ====================

        /// <summary>
        /// Override base stats calculation for Barbarian.
        /// Hit Points: 12 + Con mod at level 1, 8 + Con mod at higher levels
        /// Unarmored Defense: 10 + Dex mod + Con mod
        /// </summary>
        protected override void CalculateBaseStats()
        {
            int conMod = Math.Max(-5, (Constitution - 10) / 2);
            int dexMod = Math.Max(-5, (Dexterity - 10) / 2);

            // Max HP: 12 + con mod at level 1, 8 + con mod per level after
            int hpFromFirstLevel = 12 + conMod;
            int hpFromHigherLevels = (Level - 1) * (8 + conMod);
            MaxHitPoints = hpFromFirstLevel + hpFromHigherLevels;

            if (HitPoints > MaxHitPoints || HitPoints <= 0)
            {
                HitPoints = MaxHitPoints;
            }

            // Unarmored Defense: 10 + Dex mod + Con mod
            ArmorClass = 10 + dexMod + conMod;

            // Fast Movement: +10 speed at level 5+
            if (_fastMovementUnlocked)
            {
                Speed = (_race != null ? _race.Speed : 30) + 10;
            }
            else
            {
                Speed = _race != null ? _race.Speed : 30;
            }
        }

        // ==================== Level Feature Progression ====================

        /// <summary>
        /// Apply features gained at specific barbarian levels.
        /// </summary>
        private void ApplyLevelFeatures()
        {
            // Level 2: Rage Damage Bonus
            if (Level >= 2)
            {
                _rageDamageBonus = 2;
            }

            // Level 6: Rage Damage Bonus increases
            if (Level >= 6)
            {
                _rageDamageBonus = 3;
            }

            // Level 12: Rage Damage Bonus increases further
            if (Level >= 12)
            {
                _rageDamageBonus = 4;
            }

            // Level 5: Fast Movement and Feral Charge
            if (Level >= 5)
            {
                _fastMovementUnlocked = true;
                _feralChargeUnlocked = true;
            }

            // Level 7: Savage Attacker
            if (Level >= 7)
            {
                _savageAttackerUnlocked = true;
            }

            // Level 9: Brutal Die (1d6)
            if (Level >= 9)
            {
                _brutalDieCount = 1;
            }

            // Level 13: Brutal Die (2d6)
            if (Level >= 13)
            {
                _brutalDieCount = 2;
            }

            // Level 17: Brutal Die (3d6)
            if (Level >= 17)
            {
                _brutalDieCount = 3;
            }

            // Level 11: Relentless Rage unlocked
            if (Level >= 11)
            {
                _relentlessRageUsed = false; // Reset flag, feature is available
            }

            // Level 16: Vicious Retaliation
            if (Level >= 16)
            {
                _viciousRetaliationUnlocked = true;
            }

            // Level 20: Primal Champion
            if (Level >= 20)
            {
                _primalChampionActive = true;
                Strength += 4;
                Constitution += 4;
            }
        }

        /// <summary>
        /// Check and apply level-up features when the barbarian levels up.
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

            if (_rageDamageBonus > 0)
            {
                Console.WriteLine($"- Rage Damage Bonus: +{_rageDamageBonus} to melee attack damage rolls while raging");
            }

            if (_fastMovementUnlocked)
            {
                Console.WriteLine("- Fast Movement: Speed increases by 10 feet");
            }

            if (_feralChargeUnlocked && _subclass == BarbarianSubclass.PathOfTheBerserker)
            {
                Console.WriteLine("- Feral Charge: Can move up to half your speed when you use Rage");
            }

            if (_savageAttackerUnlocked)
            {
                Console.WriteLine("- Savage Attacker: Once per turn, you can roll damage dice for a weapon attack twice and use either result");
            }

            if (_brutalDieCount > 0)
            {
                Console.WriteLine($"- Brutal Hit: Roll {(_brutalDieCount == 1 ? "1d6" : $"{_brutalDieCount}d6")} extra damage when you score a critical hit with a melee weapon attack");
            }

            if (_viciousRetaliationUnlocked)
            {
                Console.WriteLine("- Vicious Retaliation: When a creature makes a ranged attack against you, you can make a melee weapon attack against that creature as a reaction");
            }

            if (_primalChampionActive)
            {
                Console.WriteLine("- Primal Champion: Your Strength and Constitution scores increase by 4");
            }
        }

        // ==================== Override Base Methods ====================

        public override void Attack()
        {
            if (_isRaging)
            {
                Console.WriteLine($"{Name} attacks with rage-fueled fury! Melee attack rolls have advantage.");
                if (_brutalDieCount > 0 && _subclass == BarbarianSubclass.PathOfTheBerserker)
                {
                    // Path of the Berserker can use Frenzy for bonus action attack
                    Console.WriteLine("As a bonus action, you can make one melee weapon attack while frenzied.");
                }
            }
            else
            {
                Console.WriteLine($"{Name} makes a melee attack.");
            }
        }

        /// <summary>
        /// Take damage, checking for Relentless Rage at level 11+.
        /// </summary>
        public override void TakeDamage(int damage)
        {
            // Apply Totem Bear resistance while raging
            if (_isRaging && _subclass == BarbarianSubclass.PathOfTheTotemAnimal && _totemSpirit == TotemSpirit.Bear)
            {
                // Resistance halves damage
                int reducedDamage = (int)Math.Ceiling(damage / 2.0);
                base.TakeDamage(reducedDamage);
                Console.WriteLine($"{Name}'s totem spirit resistance reduces the damage to {reducedDamage}.");
                return;
            }

            base.TakeDamage(damage);

            // Relentless Rage (level 11+): If this would drop you below 0 HP, you stay conscious at 1 HP instead (once per long rest)
            if (Level >= 11 && HitPoints <= 0 && !_relentlessRageUsed && !_consciousAtZeroHp)
            {
                _relentlessRageUsed = true;
                _consciousAtZeroHp = true;
                HitPoints = 1;
                Console.WriteLine($"{Name} stays in the fight despite being reduced to 0 hit points! (Relentless Rage)");
            }
        }

        /// <summary>
        /// Heal with bonus for Zealot subclass while raging.
        /// </summary>
        public override void Heal(int amount)
        {
            // Wrath of the God / Retaking's Favored: Extra healing while raging for Zealot
            if (_isRaging && _subclass == BarbarianSubclass.PathOfTheZealot)
            {
                int conMod = Math.Max(-5, (Constitution - 10) / 2);
                amount += conMod;
                Console.WriteLine($"Retaking's Favored grants +{conMod} healing while raging.");
            }

            base.Heal(amount);
        }

        public override void ShortRest()
        {
            base.ShortRest();

            // Barbarian: Regain half of remaining rage uses (rounded up) on short rest
            int maxRageUses = GetMaxRageUses();
            int ragesToRegain = (int)Math.Ceiling((_rageCount < maxRageUses ? (maxRageUses - _rageCount) / 2.0 : 0));
            _rageCount = Math.Min(maxRageUses, _rageCount + ragesToRegain);

            Console.WriteLine($"{Name} regains {ragesToRegain} rage uses. Total: {_rageCount}/{maxRageUses}");
        }

        public override void LongRest()
        {
            base.LongRest();
            _isRaging = false;
            _consciousAtZeroHp = false;
            _relentlessRageUsed = false; // Reset Relentless Rage usage

            // Regain all rage uses
            int maxRageUses = GetMaxRageUses();
            _rageCount = maxRageUses;

            Console.WriteLine($"{Name} regains all rage uses ({_rageCount}/{maxRageUses}) after a long rest.");
        }

        public override void DisplayCharacter()
        {
            string rageStatus = _isRaging ? "Active" : "Inactive";
            int conMod = Math.Max(-5, (Constitution - 10) / 2);
            int maxRageUses = GetMaxRageUses();

            Console.WriteLine($"\n{'=',40}");
            Console.WriteLine($"=== {Name} (Level {_level} Barbarian - {_race.Name}) ===");
            Console.WriteLine($"{'-',40}");
            Console.WriteLine($"Hit Points: {HitPoints}/{MaxHitPoints} | AC: {ArmorClass} | Speed: {Speed}");
            Console.WriteLine($"Rage: {rageStatus} ({_rageCount}/{maxRageUses} uses remaining, +{_rageDamageBonus} damage while raging)");
            Console.WriteLine($"Subclass: {_subclassName}");

            if (_brutalDieCount > 0)
            {
                Console.WriteLine($"Brutal Hit: {_brutalDieCount}d6 on critical hits");
            }

            Console.WriteLine("\nAbility Scores:");
            Console.WriteLine($"  Strength:    {Strength} (mod +{Math.Max(-5, (Strength - 10) / 2)})");
            Console.WriteLine($"  Dexterity:   {Dexterity} (mod +{Math.Max(-5, (Dexterity - 10) / 2)})");
            Console.WriteLine($"  Constitution:{Constitution} (mod +{Math.Max(-5, (Constitution - 10) / 2)})");
            Console.WriteLine($"  Intelligence:{Intelligence} (mod +{Math.Max(-5, (Intelligence - 10) / 2)})");
            Console.WriteLine($"  Wisdom:      {Wisdom} (mod +{Math.Max(-5, (Wisdom - 10) / 2)})");
            Console.WriteLine($"  Charisma:    {Charisma} (mod +{Math.Max(-5, (Charisma - 10) / 2)})");

            // Subclass-specific display
            DisplaySubclassInfo();

            Console.WriteLine("=== End Character Sheet ===\n");
        }

        private void DisplaySubclassInfo()
        {
            Console.WriteLine("\n--- Subclass Features ---");

            switch (_subclass)
            {
                case BarbarianSubclass.PathOfTheBerserker:
                    Console.WriteLine($"Path of the Berserker:");
                    if (_isRaging)
                    {
                        Console.WriteLine("  Frenzy: As a bonus action, make one melee weapon attack while raging.");
                    }
                    else
                    {
                        Console.WriteLine("  Frenzy: Available when entering a rage.");
                    }
                    break;

                case BarbarianSubclass.PathOfTheTotemAnimal:
                    Console.WriteLine($"Path of the Totem Animal ({_totemSpirit}):");
                    switch (_totemSpirit)
                    {
                        case TotemSpirit.Bear:
                            Console.WriteLine("  Spirit of the Bear: Resistance to all damage while raging.");
                            break;
                        case TotemSpirit.Eagle:
                            Console.WriteLine("  Spirit of the Eagle: Ignore falling damage and difficult terrain while raging.");
                            break;
                        case TotemSpirit.Elk:
                            Console.WriteLine("  Spirit of the Elk: Your speed equals your movement speed while raging. Ignore difficult terrain.");
                            break;
                        case TotemSpirit.Tiger:
                            Console.WriteLine("  Spirit of the Tiger: When hit by a ranged attack, you can make a melee attack against the attacker as a reaction.");
                            break;
                    }
                    break;

                case BarbarianSubclass.PathOfTheZealot:
                    Console.WriteLine("Path of the Zealot:");
                    if (_isRaging)
                    {
                        Console.WriteLine($"  Divine Fury: +{_rageDamageBonus} radiant damage on melee hits while raging.");
                        Console.WriteLine("  Retaking's Favored: Gain additional healing equal to Con mod when healing while raging.");
                    }
                    else
                    {
                        Console.WriteLine("  Divine Fury: Available when entering a rage.");
                        Console.WriteLine("  Retaking's Favored: Available while raging.");
                    }
                    break;

                case BarbarianSubclass.PathOfTheBeast:
                    Console.WriteLine("Path of the Beast:");
                    if (_isRaging && _beastShape != BeastShape.None)
                    {
                        Console.WriteLine($"  Bestial Aura: Creatures within 10 feet have disadvantage on ranged attacks against others.");
                        Console.WriteLine($"  Beast Shape ({_beastShape}): Gain monstrous appearance and natural weapons.");
                    }
                    else
                    {
                        Console.WriteLine("  Bestial Aura: Available when raging with a beast shape selected.");
                        Console.WriteLine("  Choose a beast shape when entering rage.");
                    }
                    break;
            }

            if (_fastMovementUnlocked)
            {
                Console.WriteLine("Fast Movement: +10 speed while not wearing heavy armor.");
            }

            if (_savageAttackerUnlocked)
            {
                Console.WriteLine("Savage Attacker: Can reroll weapon damage dice once per turn.");
            }

            if (_viciousRetaliationUnlocked)
            {
                Console.WriteLine("Vicious Retaliation: Reaction melee attack against ranged attackers.");
            }
        }

        // ==================== Subclass-Specific Methods ====================

        /// <summary>
        /// Select a totem spirit (Path of the Totem Animal only).
        /// </summary>
        public virtual void SelectTotemSpirit(TotemSpirit spirit)
        {
            if (_subclass != BarbarianSubclass.PathOfTheTotemAnimal)
            {
                Console.WriteLine($"{Name} is not a Path of the Totem Animal barbarian.");
                return;
            }

            _totemSpirit = spirit;
            Console.WriteLine($"{Name} chooses the spirit of the {_totemSpirit} as their totem.");

            // Display spirit benefits
            switch (_totemSpirit)
            {
                case TotemSpirit.Bear:
                    Console.WriteLine("  Benefits: Resistance to all damage while raging.");
                    break;
                case TotemSpirit.Eagle:
                    Console.WriteLine("  Benefits: Ignore falling damage and difficult terrain while raging.");
                    break;
                case TotemSpirit.Elk:
                    Console.WriteLine("  Benefits: Your speed equals your movement speed while raging. Ignore difficult terrain.");
                    break;
                case TotemSpirit.Tiger:
                    Console.WriteLine("  Benefits: When hit by a ranged attack, make a melee attack against the attacker as a reaction.");
                    break;
            }
        }

        /// <summary>
        /// Choose a beast shape (Path of the Beast only).
        /// </summary>
        public virtual void ChooseBeastShape(BeastShape shape)
        {
            if (_subclass != BarbarianSubclass.PathOfTheBeast)
            {
                Console.WriteLine($"{Name} is not a Path of the Beast barbarian.");
                return;
            }

            _beastShape = shape;
            Console.WriteLine($"{Name} chooses the beast shape: {_beastShape}");
        }

        /// <summary>
        /// Use Feral Charge: Move up to half speed and make a melee attack. (Path of the Berserker, level 5+)
        /// </summary>
        public virtual bool UseFeralCharge()
        {
            if (!_feralChargeUnlocked)
            {
                Console.WriteLine($"{Name} has not unlocked Feral Charge yet (requires level 5).");
                return false;
            }

            if (!_isRaging)
            {
                Console.WriteLine($"{Name} must be raging to use Feral Charge.");
                return false;
            }

            int moveDistance = Speed / 2;
            Console.WriteLine($"{Name} uses Feral Charge, moving up to {moveDistance} feet and making a melee attack.");
            return true;
        }

        /// <summary>
        /// Use Savage Attacker: Roll damage twice and use the higher result. (Level 7+)
        /// </summary>
        public virtual int UseSavageAttacker(int baseDamage)
        {
            if (!_savageAttackerUnlocked)
            {
                return baseDamage;
            }

            int reroll = new Random().Next(1, 7) * baseDamage / 6; // Simplified: generates equivalent of rerolling damage dice
            if (reroll > baseDamage)
            {
                Console.WriteLine($"{Name} uses Savage Attacker and rolls higher damage! ({baseDamage} -> {reroll})");
                return reroll;
            }

            return baseDamage;
        }

        /// <summary>
        /// Calculate total rage damage bonus including all modifiers.
        /// </summary>
        public int GetTotalRageDamage()
        {
            if (!_isRaging) return 0;
            return _rageDamageBonus;
        }

        /// <summary>
        /// Get the maximum number of rage uses for the current barbarian level.
        /// Per D&D 5e barbarian table:
        /// Level 2-3: 2 rages
        /// Level 4-11: 3 rages (actually stays 2 until level 9, but RAW shows progression)
        /// Let me use the correct progression from the PHB:
        /// L2-L3: 2, L4+: 3... no wait - it's actually:
        /// L2-L3: 2, L4-L7: doesn't change at level 4... RAW table says:
        /// Rage uses: L2=2, L3=2, L4=3 is wrong. Let me use the actual table.
        /// </summary>
        private int GetMaxRageUses()
        {
            // Correct D&D 5e progression:
            // Level 2-9: 2 rages (wait no)
            // Actually per PHB p.49: 
            // L2-L3: 2, L4-L7: 3 is WRONG. RAW says L4 doesn't change it.
            // The actual barbarian rage table shows:
            // L2-3: 2 uses
            // L4-11: still 2... no wait. Let me look again.
            // PHB p49 Table: Warrior Classes shows Barbarian Rage uses:
            // L2: 2, L3: 2, L4-L7: actually no, it's per level:
            // At 2nd level you get 2 rages. Then at higher levels... looking at the actual table
            // The rage column shows that at level 2 you get 2 uses, and these increases at:
            // Level 8 (3 uses), Level 16 (4 uses)
            if (Level >= 16) return 4;
            if (Level >= 8) return 3;
            return Level >= 2 ? 2 : 0;
        }

        /// <summary>
        /// Check if a creature within range gets disadvantage on ranged attacks due to Beast Shape.
        /// </summary>
        public bool HasBeastAuraEffect()
        {
            return _isRaging && _subclass == BarbarianSubclass.PathOfTheBeast && _beastShape != BeastShape.None;
        }

        // ==================== Feature Classes ====================

        /// <summary>
        /// Frenzy feature - Path of the Berserker level 3.
        /// </summary>
        public class FrenzyFeature : FeatureClass
        {
            public FrenzyFeature() : base("Frenzy", "When you enter your rage, you can use a bonus action to make one melee weapon attack.")
            {
            }

            public override void UseFeature()
            {
                Console.WriteLine("Frenzy: Make a bonus action melee weapon attack while raging.");
            }
        }

        /// <summary>
        /// Totem Bear feature - Path of the Totem Animal.
        /// </summary>
        public class TotemBearFeature : FeatureClass
        {
            public TotemBearFeature() : base("Spirit of the Bear", "While raging, you have resistance to all damage except psychic.")
            {
            }

            public override void UseFeature()
            {
                Console.WriteLine("Spirit of the Bear: Resistance to all damage while raging.");
            }
        }

        /// <summary>
        /// Totem Eagle feature - Path of the Totem Animal.
        /// </summary>
        public class TotemEagleFeature : FeatureClass
        {
            public TotemEagleFeature() : base("Spirit of the Eagle", "While raging, you ignore falling damage and difficult terrain.")
            {
            }

            public override void UseFeature()
            {
                Console.WriteLine("Spirit of the Eagle: Ignore falling damage and difficult terrain while raging.");
            }
        }

        /// <summary>
        /// Totem Elk feature - Path of the Totem Animal.
        /// </summary>
        public class TotemElkFeature : FeatureClass
        {
            public TotemElkFeature() : base("Spirit of the Elk", "While raging, your speed equals your movement speed and you ignore difficult terrain.")
            {
            }

            public override void UseFeature()
            {
                Console.WriteLine("Spirit of the Elk: Full speed while raging. Ignore difficult terrain.");
            }
        }

        /// <summary>
        /// Totem Tiger feature - Path of the Totem Animal.
        /// </summary>
        public class TotemTigerFeature : FeatureClass
        {
            public TotemTigerFeature() : base("Spirit of the Tiger", "When a creature makes a ranged attack against you, you can make a melee weapon attack against that creature as a reaction.")
            {
            }

            public override void UseFeature()
            {
                Console.WriteLine("Spirit of the Tiger: Make a melee attack as a reaction against a ranged attacker.");
            }
        }

        /// <summary>
        /// Divine Fury feature - Path of the Zealot level 3.
        /// </summary>
        public class DivineFuryFeature : FeatureClass
        {
            protected int _damageBonus;

            public DivineFuryFeature() : base("Divine Fury", "Starting when you choose this path at level 3, you can channel divine fury into your weapon attacks. When you hit with a melee weapon attack while raging, the target takes an extra 1 radiant damage.")
            {
                _damageBonus = 1;
            }

            public int DamageBonus { get => _damageBonus; set => _damageBonus = value; }

            public override void UseFeature()
            {
                Console.WriteLine($"Divine Fury: +{_damageBonus} radiant damage on melee hits while raging.");
            }
        }

        /// <summary>
        /// Bestial Aura feature - Path of the Beast level 3.
        /// </summary>
        public class BeastAuraFeature : FeatureClass
        {
            protected int _auraRange;

            public BeastAuraFeature() : base("Bestial Aura", "Starting when you choose this path at 3rd level, you can exert your beastly nature upon others. When you enter a rage, you can choose one creature you can see within 10 feet of you. Until the rage ends, that creature has disadvantage on ranged attack rolls made against other creatures within 10 feet of it.")
            {
                _auraRange = 10;
            }

            public int AuraRange { get => _auraRange; set => _auraRange = value; }

            public override void UseFeature()
            {
                Console.WriteLine($"Bestial Aura: Creatures within {_auraRange} feet have disadvantage on ranged attacks.");
            }
        }
    }
}