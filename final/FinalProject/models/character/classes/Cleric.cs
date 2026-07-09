using System;
using System.Collections.Generic;
using System.Linq;
using DnDCharacterManager.Ability;
using FeatureClass = DnDCharacterManager.Feature.Feature;
using RaceClass = DnDCharacterManager.Race.Race;
using BackgroundClass = DnDCharacterManager.Background.Background;
using SpellClass = DnDCharacterManager.Spell.Spell;
using School = DnDCharacterManager.Spell.School;
using DamageType = DnDCharacterManager.Spell.DamageType;
using SpellComponent = DnDCharacterManager.Spell.SpellComponent;

namespace DnDCharacterManager.Character.Classes
{
    /// <summary>
    /// Enum representing the 9 Divine Domains for Clerics.
    /// Each domain grants unique features at levels 2, 6, 8, and 17.
    /// </summary>
    public enum DivineDomain
    {
        Knowledge,
        Life,
        Light,
        War,
        Trickery,
        Death,
        Nature,
        Tempest,
        Peace
    }

    /// <summary>
    /// Cleric character class - a divine spellcaster who channels divine power.
    /// Full level 1-20 progression with domain-specific features.
    /// </summary>
    public class Cleric : Character
    {
        // ==================== Enums ====================

        /// <summary>
        /// The chosen Divine Domain determining subclass features.
        /// </summary>
        public DivineDomain Domain { get; set; }

        // ==================== Core Properties ====================

        /// <summary>
        /// Spellcasting ability for Clerics is Wisdom.
        /// </summary>
        public string SpellCastingAbility => "Wisdom";

        /// <summary>
        /// Number of Channel Divinity uses per rest (2 at level 2, increases at higher levels).
        /// </summary>
        public int ChannelDivinityUses { get; set; }

        /// <summary>
        /// Maximum Channel Divinity uses available.
        /// </summary>
        public int MaxChannelDivinityUses { get; set; }

        /// <summary>
        /// Whether a Free Channel Divinity ability is active (level 2+ domain feature).
        /// </summary>
        public bool FreeChannelDivinityActive { get; set; }

        /// <summary>
        /// Lay on Hands pool = Cleric level × 5.
        /// </summary>
        public int LayOnHandsPool { get; set; }

        /// <summary>
        /// Current remaining Lay on Hands points.
        /// </summary>
        public int RemainingLayOnHands { get; set; }

        /// <summary>
        /// Number of cleric cantrips known.
        /// </summary>
        public int CantripsKnown { get; set; }

        /// <summary>
        /// Number of prepared spells (cleric level + wisdom modifier, minimum 1).
        /// </summary>
        public int PreparedSpellCount { get; set; }

        /// <summary>
        /// List of spells currently prepared by the cleric.
        /// </summary>
        public List<SpellClass> PreparedSpells { get; set; }

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

        private bool _divineSenseUnlocked;
        private bool _destroyUndeadUnlocked;
        private int _undeadDestroyThreshold;
        private bool _divineInterventionUnlocked;
        private bool _improvedDivineStrikeUnlocked;
        private bool _divineIntercessionUnlocked;

        // ==================== Constructors ====================

        public Cleric() : base()
        {
            InitializeCleric();
        }

        public Cleric(string name, int level, RaceClass race, BackgroundClass background)
            : base(name, level, race, background)
        {
            InitializeCleric();
            ApplyLevelFeatures();
        }

        // ==================== Initialization ====================

        private void InitializeCleric()
        {
            Domain = DivineDomain.Knowledge;
            ChannelDivinityUses = 0;
            MaxChannelDivinityUses = 0;
            FreeChannelDivinityActive = false;
            LayOnHandsPool = 0;
            RemainingLayOnHands = 0;
            CantripsKnown = 0;
            PreparedSpells = new List<SpellClass>();
            SpellSlotsPerLevel = new Dictionary<int, int>();
            RemainingSpellSlots = new Dictionary<int, int>();

            _divineSenseUnlocked = true;
            _destroyUndeadUnlocked = false;
            _undeadDestroyThreshold = 0;
            _divineInterventionUnlocked = false;
            _improvedDivineStrikeUnlocked = false;
            _divineIntercessionUnlocked = false;

            CalculateSpellDC();
            UpdateCantripsKnown();
            UpdateSpellSlots();
            ApplyLevelFeatures();
        }

        // ==================== Stat Calculations ====================

        /// <summary>
        /// Calculates the spell save DC based on Wisdom and level.
        /// </summary>
        private void CalculateSpellDC()
        {
            int baseDC = 8;
            int wisdomMod = GetAbilityModifier(_wisdom);
            int proficiencyBonus = GetProficiencyBonus();
            SpellSaveDC = baseDC + wisdomMod + proficiencyBonus;
        }

        /// <summary>
        /// Calculates the spell attack modifier.
        /// </summary>
        private void CalculateSpellAttack()
        {
            int wisdomMod = GetAbilityModifier(_wisdom);
            int proficiencyBonus = GetProficiencyBonus();
            SpellAttackModifier = wisdomMod + proficiencyBonus;
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

        // ==================== Level Feature Progression ====================

        /// <summary>
        /// Apply features gained at specific cleric levels.
        /// </summary>
        private void ApplyLevelFeatures()
        {
            // Level 1: Divine Sense, Spellcasting (already unlocked by default)
            if (_level >= 1)
            {
                _divineSenseUnlocked = true;
                CalculateSpellDC();
                CalculateSpellAttack();
                UpdateCantripsKnown();
                UpdatePreparedSpellsCount();
            }

            // Level 2: Channel Divinity (1 rest), Domain feature
            if (_level >= 2)
            {
                MaxChannelDivinityUses = 1;
                ChannelDivinityUses = MaxChannelDivinityUses;
                LayOnHandsPool = _level * 5;
                RemainingLayOnHands = LayOnHandsPool;
                FreeChannelDivinityActive = false; // Will be set by domain
                ApplyDomainLevel2();
            }

            // Level 3: Destroy Undead (some domains)
            if (_level >= 3)
            {
                UpdateSpellSlots();
                UpdateCantripsKnown();
                UpdatePreparedSpellsCount();
            }

            // Level 4: Lay on Hands pool update
            if (_level >= 4)
            {
                UpdateSpellSlots();
                LayOnHandsPool = _level * 5;
                RemainingLayOnHands = Math.Max(RemainingLayOnHands, LayOnHandsPool - RemainingLayOnHands);
            }

            // Level 5: Destroy Undead (some domains), Cantrip update
            if (_level >= 5)
            {
                _destroyUndeadUnlocked = true;
                UpdateCantripsKnown();
                UpdateSpellSlots();
                UpdatePreparedSpellsCount();
            }

            // Level 6: Domain feature
            if (_level >= 6)
            {
                ApplyDomainLevel6();
                UpdateSpellSlots();
            }

            // Level 7: Channel Divinity (2 rests)
            if (_level >= 7)
            {
                MaxChannelDivinityUses = 2;
                ChannelDivinityUses = Math.Max(ChannelDivinityUses, 2);
            }

            // Level 8: Domain feature
            if (_level >= 8)
            {
                ApplyDomainLevel8();
                UpdateSpellSlots();
            }

            // Level 9: Divine Intervention unlock (some domains have earlier)
            if (_level >= 9)
            {
                UpdateSpellSlots();
                UpdateCantripsKnown();
            }

            // Level 10: Improved Divine Strike (some domains), Divine Intervention
            if (_level >= 10)
            {
                _divineInterventionUnlocked = true;
                UpdateSpellSlots();
                LayOnHandsPool = _level * 5;
            }

            // Level 11: Channel Divinity recovery on short rest (some domains)
            if (_level >= 11)
            {
                UpdateSpellSlots();
            }

            // Level 12: Extra lay on hands healing
            if (_level >= 12)
            {
                UpdateSpellSlots();
                LayOnHandsPool = _level * 5;
            }

            // Level 13: Cantrip update
            if (_level >= 13)
            {
                UpdateCantripsKnown();
                UpdateSpellSlots();
            }

            // Level 14: Improved Divine Strike (some domains)
            if (_level >= 14)
            {
                _improvedDivineStrikeUnlocked = true;
                UpdateSpellSlots();
            }

            // Level 15: Holy Nimbus (some domains)
            if (_level >= 15)
            {
                UpdateSpellSlots();
            }

            // Level 16: Divine Intercession (some domains)
            if (_level >= 16)
            {
                _divineIntercessionUnlocked = true;
                UpdateSpellSlots();
            }

            // Level 17: Transcendent Presence (some domains)
            if (_level >= 17)
            {
                ApplyDomainLevel17();
                UpdateSpellSlots();
            }

            // Level 18-20: Final updates
            if (_level >= 18)
            {
                UpdateSpellSlots();
            }

            if (_level >= 19)
            {
                UpdateSpellSlots();
            }

            if (_level >= 20)
            {
                UpdateSpellSlots();
            }
        }

        // ==================== Domain Features ====================

        /// <summary>
        /// Applies level 2 features for the chosen domain.
        /// </summary>
        private void ApplyDomainLevel2()
        {
            switch (Domain)
            {
                case DivineDomain.Knowledge:
                    Console.WriteLine($"{Name} gains Knowledge Domain: Bonus proficiencies in History and Arcana, can use Channel Divinity to draw knowledge from a book.");
                    break;
                case DivineDomain.Life:
                    FreeChannelDivinityActive = true;
                    Console.WriteLine($"{Name} gains Life Domain: Enhanced healing spells (+2 + cleric level to healing), can use Channel Divinity to restore hit points.");
                    break;
                case DivineDomain.Light:
                    FreeChannelDivinityActive = true;
                    Console.WriteLine($"{Name} gains Light Domain: Cantrips deal extra radiant damage, can use Channel Divinity to create bright light.");
                    break;
                case DivineDomain.War:
                    Console.WriteLine($"{Name} gains War Domain: Martial weapon and heavy armor proficiency, channel divinity grants extra attack.");
                    break;
                case DivineDomain.Trickery:
                    Console.WriteLine($"{Name} gains Trickery Domain: Blessing of the Trickster (bonus action dodge), Channel Divinity: Invoke Duplicity.");
                    break;
                case DivineDomain.Death:
                    FreeChannelDivinityActive = true;
                    Console.WriteLine($"{Name} gains Death Domain: Enhanced damage vs undead, Channel Divinity: Consume Dead.");
                    break;
                case DivineDomain.Nature:
                    Console.WriteLine($"{Name} gains Nature Domain: Access to nature spell list, Charming Tact (frightened creatures flee).");
                    break;
                case DivineDomain.Tempest:
                    FreeChannelDivinityActive = true;
                    Console.WriteLine($"{Name} gains Tempest Domain: Lightning Bolt cantrip, Wounding Strike, Channel Divinity: Thunderbolt Strike.");
                    break;
                case DivineDomain.Peace:
                    Console.WriteLine($"{Name} gains Peace Domain: Radiant Burst (10-foot radius), Emboldening Bond with allies.");
                    break;
            }
        }

        /// <summary>
        /// Applies level 6 features for the chosen domain.
        /// </summary>
        private void ApplyDomainLevel6()
        {
            switch (Domain)
            {
                case DivineDomain.Knowledge:
                    Console.WriteLine("Blessed Cache: A mystical cache appears with gold and supplies daily.");
                    break;
                case DivineDomain.Life:
                    Console.WriteLine("Nimbus of the Healing Sun: When you cast a healing spell, radiant light radiates from you.");
                    break;
                case DivineDomain.Light:
                    Console.WriteLine("Everbright: Create a 60-foot radius of bright light as an action.");
                    break;
                case DivineDomain.War:
                    Console.WriteLine("War God's Blessing: Add d4 to another creature's attack roll as a reaction.");
                    break;
                case DivineDomain.Trickery:
                    Console.WriteLine("Shadow Lore: Bonus action teleport 30 feet when hidden.");
                    break;
                case DivineDomain.Death:
                    Console.WriteLine("Antlife Veil: Create an anti-life orb that blocks spells and effects.");
                    break;
                case DivineDomain.Nature:
                    Console.WriteLine("Beast Siren: Charm beasts and plant creatures as bonus action.");
                    break;
                case DivineDomain.Tempest:
                    Console.WriteLine("Stormborn: Gain flying speed as a bonus action.");
                    break;
                case DivineDomain.Peace:
                    Console.WriteLine("Prescient Song: Add your Wisdom modifier to initiative rolls.");
                    break;
            }
        }

        /// <summary>
        /// Applies level 8 features for the chosen domain.
        /// </summary>
        private void ApplyDomainLevel8()
        {
            switch (Domain)
            {
                case DivineDomain.Knowledge:
                    Console.WriteLine("Divine Insight: Bonus action to roll a d4 and add to an ability check.");
                    break;
                case DivineDomain.Life:
                    Console.WriteLine("Restore Life: As bonus action, bring a dying creature back to 1 HP.");
                    break;
                case DivineDomain.Light:
                    Console.WriteLine("Radiant Beams: Channel Divinity fires radiant beams at nearby creatures (5d10 damage).");
                    break;
                case DivineDomain.War:
                    Console.WriteLine("War God's Fury: Use your action to make two weapon attacks or cast two cantrips.");
                    break;
                case DivineDomain.Trickery:
                    Console.WriteLine("Vanish and Teleport: When you use Invoke Duplicity, teleport to your illusion.");
                    break;
                case DivineDomain.Death:
                    Console.WriteLine("Emboldening Bond: Creatures within 30 feet gain temporary HP when you heal them.");
                    break;
                case DivineDomain.Nature:
                    Console.WriteLine("Guardian of Nature: Cast Wall of Fire, Sleet Storm, or Dust Storm as bonus action.");
                    break;
                case DivineDomain.Tempest:
                    Console.WriteLine("Heart of the Storm: Channel Divinity deals 4d8 lightning and 4d8 thunder damage.");
                    break;
                case DivineDomain.Peace:
                    Console.WriteLine("Unyielding Mind: Immunity to charm and fear effects.");
                    break;
            }
        }

        /// <summary>
        /// Applies level 17 features for the chosen domain.
        /// </summary>
        private void ApplyDomainLevel17()
        {
            switch (Domain)
            {
                case DivineDomain.Knowledge:
                    Console.WriteLine("Awe-Inspiring Speech: Creatures within 30 feet must make Wisdom save or flee in terror.");
                    break;
                case DivineDomain.Life:
                    Console.WriteLine("Invincible Faith: When damaged, gain resistance to all damage until start of your next turn.");
                    break;
                case DivineDomain.Light:
                    Console.WriteLine("Solar Beacon: Channel Divinity creates radiance dealing 10d10 radiant damage.");
                    break;
                case DivineDomain.War:
                    Console.WriteLine("Avatar of Battle: Reroll 1 or 2 on weapon attack rolls as a reaction.");
                    break;
                case DivineDomain.Trickery:
                    Console.WriteLine("Master of Shadows: Create two duplicates of yourself when hiding.");
                    break;
                case DivineDomain.Death:
                    Console.WriteLine("Lord of Undead: Command undead (CR equal to or less than your level).");
                    break;
                case DivineDomain.Nature:
                    Console.WriteLine("Awaken Forest: Control plants in a 1-mile radius.");
                    break;
                case DivineDomain.Tempest:
                    Console.WriteLine("Thunderbolt Strike: Channel Divinity pushes creatures and deals lightning damage.");
                    break;
                case DivineDomain.Peace:
                    Console.WriteLine("Supreme Peace: As reaction, force two creatures to make Wisdom saves or be charmed.");
                    break;
            }
        }

        // ==================== Spell Slot Progression (Full Caster) ====================

        /// <summary>
        /// Updates spell slots based on cleric level using full caster progression.
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

            // Copy to remaining slots
            foreach (var slot in SpellSlotsPerLevel)
            {
                RemainingSpellSlots[slot.Key] = slot.Value;
            }
        }

        // ==================== Cantrip Progression ====================

        /// <summary>
        /// Updates the number of cantrips known based on cleric level.
        /// Levels 1-4: 2 cantrips
        /// Level 5: 3 cantrips
        /// Level 10: 4 cantrips
        /// Level 16: 6 cantrips
        /// </summary>
        private void UpdateCantripsKnown()
        {
            if (_level >= 16)
                CantripsKnown = 6;
            else if (_level >= 10)
                CantripsKnown = 4;
            else if (_level >= 5)
                CantripsKnown = 3;
            else
                CantripsKnown = 2;
        }

        /// <summary>
        /// Updates the number of prepared spells based on cleric level and wisdom modifier.
        /// Prepared spells = cleric level + wisdom modifier (minimum 1).
        /// </summary>
        private void UpdatePreparedSpellsCount()
        {
            int wisMod = GetAbilityModifier(_wisdom);
            PreparedSpellCount = Math.Max(1, _level + wisMod);
        }

        // ==================== Core Methods ====================

        public override void ClassSpecificAbility()
        {
            ChannelDivinity();
        }

        /// <summary>
        /// Divine Sense: As an action, determine which direction is north, south, east, or west.
        /// Determine if there is a celestial, fiend, or undead nearby within 60 feet.
        /// </summary>
        public virtual void DivineSense()
        {
            if (!_divineSenseUnlocked)
            {
                Console.WriteLine($"{Name} has not unlocked Divine Sense yet.");
                return;
            }

            Console.WriteLine($"{Name} uses Divine Sense and detects the presence of celestials, fiends, or undead within 60 feet.");
        }

        /// <summary>
        /// Channel Divinity: Expose your divine nature to create a powerful effect based on your domain.
        /// Uses 1 Channel Divinity use.
        /// </summary>
        public virtual void ChannelDivinity()
        {
            if (ChannelDivinityUses <= 0)
            {
                Console.WriteLine($"{Name} has no Channel Divinity uses remaining. Take a long rest to recover.");
                return;
            }

            ChannelDivinityUses--;
            Console.WriteLine($"{Name} uses Channel Divinity! Uses remaining: {ChannelDivinityUses}/{MaxChannelDivinityUses}");

            // Domain-specific channel divinity effect
            switch (Domain)
            {
                case DivineDomain.Knowledge:
                    Console.WriteLine("  You reveal the contents of one book or scroll within 60 feet. You can read and understand its content for 1 minute.");
                    break;
                case DivineDomain.Life:
                    Console.WriteLine("  Each creature within 30 feet regains hit points equal to 2 + cleric level (minimum 1).");
                    int lifeHealing = 2 + _level;
                    Console.WriteLine($"  {lifeHealing} hit points are restored to nearby creatures.");
                    break;
                case DivineDomain.Light:
                    Console.WriteLine("  Bright light shines from you in a 30-foot radius. Undead within the area take radiant damage.");
                    Console.WriteLine("  The light lasts until your next turn or you end it as a bonus action.");
                    break;
                case DivineDomain.War:
                    Console.WriteLine("  You can use your action to make two weapon attacks or cast two cantrips until your next turn.");
                    break;
                case DivineDomain.Trickery:
                    Console.WriteLine("  Your illusion duplicates itself and moves to a new location within 60 feet. You can see through its eyes as a bonus action.");
                    break;
                case DivineDomain.Death:
                    Console.WriteLine("  Each undead within 30 feet takes radiant damage equal to your cleric level + wisdom modifier.");
                    Console.WriteLine("  Additionally, any undead that dies from this effect explodes in a burst of radiance.");
                    break;
                case DivineDomain.Nature:
                    Console.WriteLine("  You can cast the Entangle spell without using a spell slot. Creatures restrained have disadvantage on attacks against you.");
                    break;
                case DivineDomain.Tempest:
                    Console.WriteLine("  A bolt of lightning strikes each creature within 30 feet. Each must make a Dexterity save or take 4d8 lightning damage.");
                    break;
                case DivineDomain.Peace:
                    Console.WriteLine("  Radiant energy radiates from you in a 10-foot radius. Hostile creatures within must make a Constitution save or take radiant damage and be charmed.");
                    break;
            }
        }

        /// <summary>
        /// Recover Channel Divinity uses on a long rest.
        /// </summary>
        public virtual void RecoverChannelDivinity()
        {
            ChannelDivinityUses = MaxChannelDivinityUses;
            Console.WriteLine($"{Name} recovers all {MaxChannelDivinityUses} Channel Divinity uses after a long rest.");
        }

        /// <summary>
        /// Lay on Hands: Touch a creature to heal them using your divine pool.
        /// Maximum pool = cleric level x 5.
        /// </summary>
        public virtual void LayOnHands(Character target, int amount)
        {
            if (target == null)
            {
                Console.WriteLine("No valid target to heal.");
                return;
            }

            if (RemainingLayOnHands <= 0)
            {
                Console.WriteLine($"{Name} has no Lay on Hands points remaining. Take a long rest to recover.");
                return;
            }

            int maxPossibleHeal = target.MaxHitPoints - target.HitPoints;
            int actualHeal = amount;
            if (RemainingLayOnHands < actualHeal) actualHeal = RemainingLayOnHands;
            if (maxPossibleHeal < actualHeal) actualHeal = maxPossibleHeal;

            if (actualHeal <= 0)
            {
                Console.WriteLine($"{target.Name} is already at full health.");
                return;
            }

            RemainingLayOnHands -= actualHeal;
            target.Heal(actualHeal);
            Console.WriteLine($"{Name} uses Lay on Hands on {target.Name} for {actualHeal} hit points. Remaining pool: {RemainingLayOnHands}");
        }

        /// <summary>
        /// Calculate the Lay on Hands pool based on cleric level.
        /// </summary>
        public virtual int CalculateLayOnHandsPool()
        {
            LayOnHandsPool = _level * 5;
            RemainingLayOnHands = Math.Max(RemainingLayOnHands, LayOnHandsPool - (LayOnHandsPool - RemainingLayOnHands));
            return LayOnHandsPool;
        }

        /// <summary>
        /// Destroy Undead: Instantly destroy an undead creature of CR equal to or less than your threshold.
        /// Available at cleric level 5+.
        /// </summary>
        public virtual bool DestroyUndead(int creatureCR)
        {
            if (!_destroyUndeadUnlocked)
            {
                Console.WriteLine($"{Name} has not unlocked Destroy Undead yet (requires level 5).");
                return false;
            }

            // Threshold based on cleric level
            if (_level >= 18) _undeadDestroyThreshold = 6;
            else if (_level >= 14) _undeadDestroyThreshold = 4;
            else if (_level >= 10) _undeadDestroyThreshold = 3;
            else if (_level >= 5) _undeadDestroyThreshold = 2;

            if (creatureCR <= _undeadDestroyThreshold)
            {
                Console.WriteLine($"{Name} destroys the undead creature with divine power!");
                return true;
            }

            Console.WriteLine($"The undead creature is too powerful (CR {creatureCR} > threshold {_undeadDestroyThreshold}).");
            return false;
        }

        /// <summary>
        /// Prepare a spell from your spell list. Clerics prepare spells from the full cleric list.
        /// Maximum prepared = cleric level + wisdom modifier.
        /// </summary>
        public virtual void PrepareSpell(SpellClass spell)
        {
            if (spell == null)
            {
                Console.WriteLine("Cannot prepare a null spell.");
                return;
            }

            if (!IsClericSpell(spell))
            {
                Console.WriteLine($"{spell.Name} is not on the cleric spell list.");
                return;
            }

            if (PreparedSpells.Count >= PreparedSpellCount)
            {
                Console.WriteLine($"Cannot prepare more spells. Maximum prepared: {PreparedSpellCount}");
                return;
            }

            if (PreparedSpells.Contains(spell))
            {
                Console.WriteLine($"{Name} has already prepared {spell.Name}.");
                return;
            }

            PreparedSpells.Add(spell);
            Console.WriteLine($"{Name} has prepared the spell: {spell.Name} (Level {spell.Level})");
        }

        /// <summary>
        /// Remove a prepared spell.
        /// </summary>
        public virtual void RemovePreparedSpell(string spellName)
        {
            SpellClass? spellToRemove = PreparedSpells.FirstOrDefault(s => s.Name == spellName);
            if (spellToRemove != null)
            {
                PreparedSpells.Remove(spellToRemove);
                Console.WriteLine($"{Name} has unprepared the spell: {spellName}");
            }
        }

        /// <summary>
        /// Cast a prepared spell if slots are available.
        /// </summary>
        public virtual bool CastSpell(string spellName, int spellLevel = 0)
        {
            SpellClass? spellToCast = PreparedSpells.FirstOrDefault(s => s.Name == spellName);
            if (spellToCast == null)
            {
                // Try cantrips
                spellToCast = CantripSpells.FirstOrDefault(s => s.Name == spellName);
                if (spellToCast != null)
                {
                    Console.WriteLine($"{Name} casts the cantrip {spellName}.");
                    spellToCast.Cast();
                    return true;
                }

                Console.WriteLine($"{Name} does not have {spellName} prepared.");
                return false;
            }

            // Check if cantrip (level 0)
            if (spellLevel == 0 || spellToCast.Level == 0)
            {
                Console.WriteLine($"{Name} casts the cantrip {spellName}.");
                spellToCast.Cast();
                return true;
            }

            // Check spell slot availability
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
        /// Divine Intervention: At level 8+, call upon your deity for help.
        /// Roll d100: on 1-25, intervention succeeds.
        /// </summary>
        public virtual void DivineIntervention()
        {
            if (!_divineInterventionUnlocked)
            {
                Console.WriteLine($"{Name} has not unlocked Divine Intervention yet (requires level 8+).");
                return;
            }

            int roll = new Random().Next(1, 101);
            if (roll <= 25)
            {
                Console.WriteLine($"{Name} calls upon their deity and divine intervention succeeds! The deity grants aid.");
                Console.WriteLine("The intervention takes the form of a favorable event, a powerful ally, or a miraculous effect.");
            }
            else
            {
                Console.WriteLine($"{Name} calls upon their deity, but divine intervention fails (rolled {roll}/100).");
            }
        }

        /// <summary>
        /// Improved Divine Strike: Add radiant damage to weapon attacks.
        /// Available at cleric level 8+ for most domains.
        /// </summary>
        public virtual void ImprovedDivineStrike()
        {
            if (!_improvedDivineStrikeUnlocked)
            {
                Console.WriteLine($"{Name} has not unlocked Improved Divine Strike yet.");
                return;
            }

            int damage = Domain == DivineDomain.Life ? 3 : 5;
            Console.WriteLine($"{Name}'s next weapon attack deals an extra {damage} radiant damage.");
        }

        /// <summary>
        /// Check if a spell is on the cleric spell list.
        /// </summary>
        private bool IsClericSpell(SpellClass spell)
        {
            if (spell == null) return false;
            // Check cantrips
            foreach (var cantrip in ClericCantrips)
            {
                if (cantrip.Equals(spell.Name, StringComparison.OrdinalIgnoreCase))
                    return true;
            }
            // Check spell levels
            foreach (var spellList in ClericSpells.Values)
            {
                foreach (var s in spellList)
                {
                    if (s.Equals(spell.Name, StringComparison.OrdinalIgnoreCase))
                        return true;
                }
            }
            return false;
        }

        // ==================== Cleric Spell Lists ====================

        /// <summary>
        /// Cantrips (0-level spells) available to Clerics.
        /// </summary>
        private static readonly List<string> ClericCantrips = new()
        {
            "Guidance", "Light", "Sacred Flame", "Thaumaturgy", "Toll the Dead",
            "Word of Radiance", "Blessing of Knowledge", "Chill Touch", "Detect Magic",
            "Mending", "Purify Food and Drink", "Resistance"
        };

        /// <summary>
        /// Full cleric spell list by level.
        /// </summary>
        private static readonly Dictionary<int, List<string>> ClericSpells = new()
        {
            [1] = new()
            {
                "Bless", "Cure Wounds", "Detect Evil and Good", "Detect Magic", "Guiding Bolt",
                "Hellish Rebuke", "Protection from Evil and Good", "Sanctuary", "Thunderwave"
            },
            [2] = new()
            {
                "Aid", "Blindness/Deafness", "Crown of Stars", "Cordon of Arrows", "Flaming Sphere",
                "Gust of Wind", "Infernal Chain", "Lesser Restoration", "Magic Weapon", "Scorching Ray",
                "See Invisibility", "Spirit Shroud", "Suggestion", "Warding Bond"
            },
            [3] = new()
            {
                "Bestow Curse", "Clarity of Thought", "Dispel Magic", "Feign Death", "Fear",
                "Flame Strike", "Glyph of Warding", "Haste", "Magic Circle", "Mind Blank",
                "Moment of Decision", "Protection from Energy", "Reincarnate", "Sending",
                "Spirit Guardians", "Tenser's Transformation", "Vampiric Touch"
            },
            [4] = new()
            {
                "Banishment", "Blight", "Control Water", "Divine Favor", "Freedom of Movement",
                "Guardian of Faith", "Stoneskin", "Tabernacle"
            },
            [5] = new()
            {
                "Contagion", "Dispel Evil and Good", "Guarded Ward", "Insect Plague", "Ley Line Rune",
                "Mass Healing Word", "Planar Binding", "Raise Dead", "Scrying", "Temple of the Gods"
            },
            [6] = new()
            {
                "Find the Path", "Forbiddance", "Giant's Constitute", "Heal", "Holy Weapon",
                "Leap of the Dayling", "Word of Recall"
            },
            [7] = new()
            {
                "Control Weather", "Divine Word", "Delayed Blast Fireball", "Mystic Chain Ravages",
                "Set Free", "Symbol", "Teleportation Circle"
            },
            [8] = new()
            {
                "Ante", "Dominate Monster", "Ecstatic Vision", "Holy Emotion", "Incendiary Cloud",
                "Maze", "Philosopher's Stone", "Sunburst"
            },
            [9] = new()
            {
                "Astral Projection", "Gate", "Imprisonment", "Mask of the Wild", "Power Word Heal",
                "Psychic Scream", "True Polymorph", "Weird", "Wish"
            }
        };

        /// <summary>
        /// Cantrip spell objects for reference.
        /// </summary>
        private static readonly List<SpellClass> CantripSpells = new()
        {
            new SpellClass("Guidance", 0, School.Divination, "1 bonus action", "Touch", "Concentration, up to 1 minute",
                "You touch one creature. Once before the target makes an ability check, it can roll a d4 and add the number to the ability check.",
                SpellComponent.VerbalSomatic),
            new SpellClass("Light", 0, School.Evocation, "1 action", "Touch", "1 hour",
                "One touched object sheds bright light in a 20-foot radius.",
                SpellComponent.VerbalSomaticMaterial, materialComponents: "a bit of phosphorus or worms"),
            new SpellClass("Sacred Flame", 0, School.Evocation, "1 action", "60 feet", "Instantaneous",
                "Flaming light falls from the sky. A creature in that area must make a Dexterity saving throw.",
                SpellComponent.VerbalSomatic, damageType: DamageType.Radiant, damageAmount: "1d8",
                savingThrow: "Dexterity DC"),
            new SpellClass("Thaumaturgy", 0, School.Transmutation, "1 action", "Self", "Up to 1 minute",
                "You manifest a minor wonder, a sign of divine power.",
                SpellComponent.Verbal),
            new SpellClass("Toll the Dead", 0, School.Necromancy, "1 action", "60 feet", "Instantaneous",
                "You point at one creature within range and make the tolling sound of a bell. The target must make a Wisdom save or take damage.",
                SpellComponent.Verbal, damageType: DamageType.Necrotic, damageAmount: "1d8",
                savingThrow: "Wisdom DC"),
            new SpellClass("Word of Radiance", 0, School.Evocation, "1 action", "Self", "Instantaneous",
                "A burst of radiant energy pulses from you. Each creature in a 5-foot radius must make a Dexterity save or take radiant damage.",
                SpellComponent.Verbal, damageType: DamageType.Radiant, damageAmount: "1d6")
        };

        // ==================== Helper Methods ====================

        /// <summary>
        /// Get available cleric spells of a specific level.
        /// </summary>
        public List<string> GetAvailableSpells(int spellLevel)
        {
            if (spellLevel == 0)
                return new List<string>(ClericCantrips);

            if (ClericSpells.ContainsKey(spellLevel))
                return new List<string>(ClericSpells[spellLevel]);

            return new List<string>();
        }

        /// <summary>
        /// Check if a spell is on the cleric spell list.
        /// </summary>
        public bool IsSpellOnClericList(string spellName)
        {
            foreach (var cantrip in ClericCantrips)
            {
                if (cantrip.Equals(spellName, StringComparison.OrdinalIgnoreCase))
                    return true;
            }

            foreach (var spells in ClericSpells.Values)
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
        /// Get the maximum spell slot level available to this cleric.
        /// </summary>
        public int GetMaxSpellSlotLevel()
        {
            if (_level < 2) return 0; // No slots at level 1
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
        /// Overrides base stat calculation for Cleric.
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

            // Recalculate spell DC and attack on level change
            CalculateSpellDC();
            CalculateSpellAttack();
            UpdateCantripsKnown();
            UpdatePreparedSpellsCount();

            // Update Lay on Hands pool
            LayOnHandsPool = _level * 5;
        }

        /// <summary>
        /// Overrides level up to apply cleric features.
        /// </summary>
        public override void LevelUp()
        {
            _level++;
            CalculateBaseStats();
            UpdateSpellSlots();
            ApplyLevelFeatures();
            Console.WriteLine($"{_name} has reached level {_level}! Cleric features updated.");

            // Check for new feature unlocks
            if (_level == 2)
                Console.WriteLine("New feature: Channel Divinity and Domain features unlocked!");
            if (_level == 5)
                Console.WriteLine("New feature: Destroy Undead unlocked!");
            if (_level == 8)
                Console.WriteLine("New feature: Divine Intervention unlocked!");
            if (_level == 17)
                Console.WriteLine("New feature: Domain divinity feature unlocked!");
        }

        /// <summary>
        /// Overrides display to include cleric-specific information.
        /// </summary>
        public override void DisplayCharacter()
        {
            Console.WriteLine($"\n=== {_name} (Level {_level} Cleric - {Domain}) ===");
            Console.WriteLine($"Hit Points: {_hitPoints}/{_maxHitPoints} | AC: {_armorClass} | Speed: {_speed}");
            Console.WriteLine("\n--- Ability Scores ---");
            Console.WriteLine($"  Strength:    {_strength} (mod +{GetAbilityModifier(_strength)})");
            Console.WriteLine($"  Dexterity:   {_dexterity} (mod +{GetAbilityModifier(_dexterity)})");
            Console.WriteLine($"  Constitution:{_constitution} (mod +{GetAbilityModifier(_constitution)})");
            Console.WriteLine($"  Intelligence:{_intelligence} (mod +{GetAbilityModifier(_intelligence)})");
            Console.WriteLine($"  Wisdom:      {_wisdom} (mod +{GetAbilityModifier(_wisdom)})");
            Console.WriteLine($"  Charisma:    {_charisma} (mod +{GetAbilityModifier(_charisma)})");

            Console.WriteLine("\n--- Cleric Features ---");
            Console.WriteLine($"Spellcasting Ability: {SpellCastingAbility}");
            Console.WriteLine($"Spell Save DC: {SpellSaveDC} | Spell Attack Modifier: {SpellAttackModifier}");
            Console.WriteLine($"Cantrips Known: {CantripsKnown}");
            Console.WriteLine($"Prepared Spells: {PreparedSpells.Count}/{PreparedSpellCount}");

            // Domain features
            Console.WriteLine($"\n--- Divine Domain: {Domain} ---");
            if (_divineSenseUnlocked)
                Console.WriteLine("  Divine Sense: Detect celestials, fiends, or undead");
            if (_destroyUndeadUnlocked)
                Console.WriteLine($"  Destroy Undead (CR <= {_undeadDestroyThreshold})");
            if (_divineInterventionUnlocked)
                Console.WriteLine("  Divine Intervention: Chance to call upon deity");
            if (_improvedDivineStrikeUnlocked)
                Console.WriteLine("  Improved Divine Strike: Extra radiant damage on attacks");

            // Channel Divinity
            Console.WriteLine($"\n--- Channel Divinity ---");
            Console.WriteLine($"  Uses: {ChannelDivinityUses}/{MaxChannelDivinityUses}");

            // Lay on Hands
            Console.WriteLine($"\n--- Lay on Hands ---");
            Console.WriteLine($"  Pool: {RemainingLayOnHands}/{LayOnHandsPool}");

            // Spell slots
            if (SpellSlotsPerLevel.Count > 0)
            {
                Console.WriteLine("\n--- Spell Slots ---");
                foreach (var slot in SpellSlotsPerLevel)
                {
                    Console.WriteLine($"  {slot.Key}-level: {RemainingSpellSlots.GetValueOrDefault(slot.Key, 0)}/{slot.Value} remaining");
                }
            }

            // Prepared spells
            if (PreparedSpells.Count > 0)
            {
                Console.WriteLine("\n--- Prepared Spells ---");
                foreach (var spell in PreparedSpells)
                {
                    Console.WriteLine($"  - {spell.Name} (Level {spell.Level}, {spell.School})");
                }
            }

            // Cantrips
            Console.WriteLine($"\n--- Cantrips Known ({CantripsKnown}) ---");
            foreach (var cantrip in ClericCantrips.Take(CantripsKnown))
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

            // Recover all spell slots
            foreach (var slot in SpellSlotsPerLevel)
            {
                RemainingSpellSlots[slot.Key] = slot.Value;
            }

            // Recover Channel Divinity uses
            RecoverChannelDivinity();

            Console.WriteLine($"{_name} has recovered all spell slots and Channel Divinity uses after a long rest.");
        }

        /// <summary>
        /// Overrides short rest to potentially recover resources.
        /// </summary>
        public override void ShortRest()
        {
            base.ShortRest();

            // Some domains can recover Channel Divinity on short rest
            if (FreeChannelDivinityActive && ChannelDivinityUses < MaxChannelDivinityUses)
            {
                ChannelDivinityUses = MaxChannelDivinityUses;
                Console.WriteLine($"{_name} recovers Channel Divinity uses on short rest.");
            }

            // Clerics can recover spell slots using invigorating words (Wisdom mod x 1d6 HP each)
            int wisMod = GetAbilityModifier(_wisdom);
            if (wisMod > 0)
            {
                int slotsToRecover = Math.Max(1, wisMod);
                // Simple implementation: recover lowest level slot
                var sortedSlots = RemainingSpellSlots.OrderByDescending(s => s.Key).ToList();
                foreach (var slot in sortedSlots)
                {
                    if (slot.Key > 0 && slot.Value < SpellSlotsPerLevel[slot.Key])
                    {
                        RemainingSpellSlots[slot.Key]++;
                        slotsToRecover--;
                        if (slotsToRecover <= 0) break;
                    }
                }
            }
        }

        /// <summary>
        /// Take damage, applying Life domain resistance if applicable.
        /// </summary>
        public override void TakeDamage(int damage)
        {
            // Life domain: When you would take damage, you can use your reaction to gain resistance
            if (Domain == DivineDomain.Life)
            {
                Console.WriteLine("Life Domain: You could use your reaction to gain resistance to the next instance of damage.");
            }

            base.TakeDamage(damage);
        }

        /// <summary>
        /// Heal with bonus for Life domain.
        /// </summary>
        public override void Heal(int amount)
        {
            int finalAmount = amount;

            // Life domain: Healing spells you cast heal 2 + cleric level extra
            if (Domain == DivineDomain.Life && Level >= 2)
            {
                finalAmount += 2 + _level;
                Console.WriteLine($"Life Domain's Disciple of Life adds +{2 + _level} to healing.");
            }

            base.Heal(finalAmount);
        }
    }
}