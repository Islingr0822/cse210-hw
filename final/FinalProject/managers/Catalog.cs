using System;
using System.Collections.Generic;
using DnDCharacterManager.Item;
using DnDCharacterManager.Spell;
using ItemType = DnDCharacterManager.Item.Item;
using SpellType = DnDCharacterManager.Spell.Spell;

namespace DnDCharacterManager.Managers
{
    /// <summary>
    /// Central registry of the items and spells a player can pick from.
    /// Each entry's Key equals the concrete type's name, so saved characters
    /// (which store the type name) can be rebuilt through the same factory.
    /// </summary>
    public static class Catalog
    {
        /// <summary>
        /// A single selectable catalog entry: a stable key, a friendly display
        /// label for menus, and a factory that produces a fresh instance.
        /// </summary>
        public class Entry<T>
        {
            public string Key { get; }
            public string DisplayName { get; }
            public Func<T> Create { get; }

            public Entry(string key, string displayName, Func<T> create)
            {
                Key = key;
                DisplayName = displayName;
                Create = create;
            }
        }

        private static readonly List<Entry<ItemType>> _items = new List<Entry<ItemType>>
        {
            // Weapons
            new Entry<ItemType>("Dagger", "Dagger (1d4 piercing, light/thrown)", () => new Dagger()),
            new Entry<ItemType>("LongSword", "Longsword (1d8 slashing, versatile)", () => new LongSword()),
            new Entry<ItemType>("BattleAxe", "Battle Axe (1d10 slashing, heavy)", () => new BattleAxe()),
            new Entry<ItemType>("Greatclub", "Greatclub (1d10 bludgeoning, two-handed)", () => new Greatclub()),
            new Entry<ItemType>("Handaxe", "Handaxe (1d6 slashing, thrown)", () => new Handaxe()),
            new Entry<ItemType>("Quarterstaff", "Quarterstaff (1d6 bludgeoning)", () => new Quarterstaff()),
            new Entry<ItemType>("Shortbow", "Shortbow (1d6 piercing, 80 ft)", () => new Shortbow()),
            new Entry<ItemType>("CrossbowLight", "Light Crossbow (1d10 piercing, 80 ft)", () => new CrossbowLight()),
            new Entry<ItemType>("LongSwordPlusOne", "Longsword +1 (uncommon magic)", () => new LongSwordPlusOne()),

            // Armor
            new Entry<ItemType>("LeatherArmor", "Leather Armor (AC 11, light)", () => new LeatherArmor()),
            new Entry<ItemType>("StuddedLeather", "Studded Leather (AC 12, light)", () => new StuddedLeather()),
            new Entry<ItemType>("ChainShirt", "Chain Shirt (AC 13, medium)", () => new ChainShirt()),
            new Entry<ItemType>("ChainMail", "Chain Mail (AC 16, heavy, STR 13)", () => new ChainMail()),
            new Entry<ItemType>("Hide", "Hide (AC 12, medium)", () => new Hide()),

            // Shield
            new Entry<ItemType>("StandardShield", "Shield (+2 AC)", () => new StandardShield()),

            // Potion
            new Entry<ItemType>("PotionOfHealing", "Potion of Healing (2d4 HP)", () => new PotionOfHealing(2, 4)),

            // Tool
            new Entry<ItemType>("Tool", "Thieves' Tools", () => new Tool("Thieves' Tools", 1, 25, "Thieves")),

            // Adventuring gear
            new Entry<ItemType>("Rope", "Rope (10 ft)", () => new Rope()),
            new Entry<ItemType>("Torch", "Torch", () => new Torch()),
            new Entry<ItemType>("Rations", "Rations (1 day)", () => new Rations()),
            new Entry<ItemType>("Waterskin", "Waterskin", () => new Waterskin())
        };

        private static readonly List<Entry<SpellType>> _spells = new List<Entry<SpellType>>
        {
            // Cantrips
            new Entry<SpellType>("AcidSplash", "Acid Splash (Cantrip, Evocation)", () => new AcidSplash()),
            new Entry<SpellType>("SacredFlame", "Sacred Flame (Cantrip, Evocation)", () => new SacredFlame()),
            new Entry<SpellType>("Guidance", "Guidance (Cantrip, Divination)", () => new Guidance()),

            // Level 1
            new Entry<SpellType>("MagicMissile", "Magic Missile (Lvl 1, Evocation)", () => new MagicMissile()),
            new Entry<SpellType>("CureWounds", "Cure Wounds (Lvl 1, Evocation)", () => new CureWounds()),
            new Entry<SpellType>("HealingWord", "Healing Word (Lvl 1, Evocation)", () => new HealingWord()),
            new Entry<SpellType>("Shield", "Shield (Lvl 1, Abjuration)", () => new DnDCharacterManager.Spell.Shield()),
            new Entry<SpellType>("MageArmor", "Mage Armor (Lvl 1, Abjuration)", () => new MageArmor()),
            new Entry<SpellType>("Bless", "Bless (Lvl 1, Enchantment)", () => new Bless()),
            new Entry<SpellType>("GuidingBolt", "Guiding Bolt (Lvl 1, Evocation)", () => new GuidingBolt()),
            new Entry<SpellType>("Sleep", "Sleep (Lvl 1, Enchantment)", () => new Sleep()),
            new Entry<SpellType>("Thunderwave", "Thunderwave (Lvl 1, Evocation)", () => new Thunderwave()),

            // Level 2
            new Entry<SpellType>("MistyStep", "Misty Step (Lvl 2, Conjuration)", () => new MistyStep()),
            new Entry<SpellType>("Shatter", "Shatter (Lvl 2, Evocation)", () => new Shatter()),
            new Entry<SpellType>("HoldPerson", "Hold Person (Lvl 2, Enchantment)", () => new HoldPerson()),

            // Level 3
            new Entry<SpellType>("Fireball", "Fireball (Lvl 3, Evocation)", () => new Fireball()),
            new Entry<SpellType>("LightningBolt", "Lightning Bolt (Lvl 3, Evocation)", () => new LightningBolt()),
            new Entry<SpellType>("Fly", "Fly (Lvl 3, Transmutation)", () => new Fly()),
            new Entry<SpellType>("Haste", "Haste (Lvl 3, Transmutation)", () => new Haste())
        };

        /// <summary>The selectable item entries, in menu order.</summary>
        public static IReadOnlyList<Entry<ItemType>> Items => _items;

        /// <summary>The selectable spell entries, in menu order.</summary>
        public static IReadOnlyList<Entry<SpellType>> Spells => _spells;

        /// <summary>Creates an item instance from its catalog key, or null if unknown.</summary>
        public static ItemType? CreateItem(string key)
        {
            var entry = _items.Find(e => e.Key == key);
            return entry?.Create();
        }

        /// <summary>Creates a spell instance from its catalog key, or null if unknown.</summary>
        public static SpellType? CreateSpell(string key)
        {
            var entry = _spells.Find(e => e.Key == key);
            return entry?.Create();
        }
    }
}
