using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace DnDCharacterManager.Item
{
    //==========================
    // ENUMS
    //==========================

    /// <summary>
    /// Represents the rarity of an item.
    /// </summary>
    public enum Rarity
    {
        Common,
        Uncommon,
        Rare,
        VeryRare,
        Legendary
    }

    /// <summary>
    /// Represents weapon properties in D&D 5e.
    /// </summary>
    public enum WeaponProperty
    {
        Light,
        Heavy,
        Versatile,
        TwoHanded,
        Thrown,
        Reach,
        Ammunition,
        Loading,
        Special
    }

    /// <summary>
    /// Represents armor types.
    /// </summary>
    public enum ArmorType
    {
        Light,
        Medium,
        Heavy
    }

    //==========================
    // BASE ITEM CLASS
    //==========================

    /// <summary>
    /// Abstract base class for all items.
    /// </summary>
    public abstract class Item
    {
        protected string _name;
        protected double _weight;
        protected int _value; // Value in gold pieces
        protected Rarity _rarity;
        protected string _description;

        protected Item()
        {
            _name = "Unnamed Item";
            _weight = 0;
            _value = 0;
            _rarity = Rarity.Common;
            _description = "A basic item.";
        }

        public Item(string name, double weight, int value, Rarity rarity = Rarity.Common, string description = "")
        {
            _name = name;
            _weight = weight;
            _value = value;
            _rarity = rarity;
            _description = string.IsNullOrEmpty(description) ? $"{name} - A {rarity.ToLower()} item." : description;
        }

        // Properties
        public string Name { get => _name; set => _name = value; }
        public double Weight { get => _weight; set => _weight = value; }
        public int Value { get => _value; set => _value = value; }
        public Rarity Rarity { get => _rarity; set => _rarity = value; }
        public string Description { get => _description; set => _description = value; }

        // Methods
        public virtual void DisplayInfo()
        {
            string rarityStr = _rarity != Rarity.Common ? $" [{_rarity}]" : "";
            Console.WriteLine($"  Item: {_name}{rarityStr} | Weight: {_weight} lbs | Value: {_value} gp");
            Console.WriteLine($"    {_description}");
        }

        public override string ToString()
        {
            string rarityStr = _rarity != Rarity.Common ? $" [{_rarity}]" : "";
            return $"{_name}{rarityStr} (Weight: {_weight}, Value: {_value} gp)";
        }
    }

    //==========================
    // WEAPON hierarchy
    //==========================

    /// <summary>
    /// Base class for weapons.
    /// </summary>
    public class Weapon : Item
    {
        protected string _damageDice;
        protected string _damageType;
        protected int _range; // Melee (0-5) or range in feet
        protected List<WeaponProperty> _properties;
        protected bool _isEquipped;

        protected Weapon() : base()
        {
            _damageDice = "1d4";
            _damageType = "bludgeoning";
            _range = 5;
            _properties = new List<WeaponProperty>();
            _isEquipped = false;
        }

        public Weapon(string name, double weight, int value, string damageDice, string damageType, int range = 5, Rarity rarity = Rarity.Common, List<WeaponProperty>? properties = null, string description = "")
            : base(name, weight, value, rarity, description)
        {
            _damageDice = damageDice;
            _damageType = damageType;
            _range = range;
            _properties = properties ?? new List<WeaponProperty>();
            _isEquipped = false;
        }

        // Properties
        public string DamageDice { get => _damageDice; set => _damageDice = value; }
        public string DamageType { get => _damageType; set => _damageType = value; }
        public int Range { get => _range; set => _range = value; }
        public List<WeaponProperty> Properties { get => _properties; set => _properties = value; }
        public bool IsEquipped { get => _isEquipped; set => _isEquipped = value; }

        // Methods
        public bool HasProperty(WeaponProperty prop)
        {
            return _properties.Contains(prop);
        }

        public virtual void Equip()
        {
            _isEquipped = true;
            Console.WriteLine($"{_name} is equipped.");
        }

        public virtual void Unequip()
        {
            _isEquipped = false;
            Console.WriteLine($"{_name} is unequipped.");
        }

        /// <summary>
        /// Roll damage for this weapon. Returns the raw dice roll result.
        /// </summary>
        public virtual int RollDamage()
        {
            string[] parts = _damageDice.Split('d');
            if (parts.Length == 2 && int.TryParse(parts[0], out int numDice) && int.TryParse(parts[1], out int dieSides))
            {
                int total = 0;
                var rand = new Random();
                for (int i = 0; i < numDice; i++)
                {
                    total += rand.Next(1, dieSides + 1);
                }
                return total;
            }
            return 1;
        }

        public override void DisplayInfo()
        {
            string rangeStr = _range <= 5 ? "Melee" : $"{_range}ft";
            string rarityStr = _rarity != Rarity.Common ? $" [{_rarity}]" : "";
            string equippedStr = _isEquipped ? " (Equipped)" : "";
            string propertiesStr = _properties.Count > 0 ? " [" + string.Join(", ", _properties) + "]" : "";
            
            Console.WriteLine($"  Weapon: {_name}{rarityStr}{equippedStr} | Damage: {_damageDice}{_damageType} | Range: {rangeStr}{propertiesStr} | Value: {_value} gp");
        }
    }

    // Common melee weapons
    public class Club : Weapon
    {
        public Club() : base("Club", 2, 1, "1d4", "bludgeoning", 5, Rarity.Common, new List<WeaponProperty> { WeaponProperty.Light, WeaponProperty.Thrown }, "A simple club.")
        {
        }
    }

    public class Quarterstaff : Weapon
    {
        public Quarterstaff() : base("Quarterstaff", 4, 5, "1d6", "bludgeoning", 5, Rarity.Common, new List<WeaponProperty> { WeaponProperty.Heavy, WeaponProperty.Versatile }, "A long staff.")
        {
        }
    }

    public class Sickle : Weapon
    {
        public Sickle() : base("Sickle", 1, 5, "1d4", "slashing", 5, Rarity.Common, new List<WeaponProperty> { WeaponProperty.Light, WeaponProperty.Thrown }, "A farming sickle.")
        {
        }
    }

    public class Handaxe : Weapon
    {
        public Handaxe() : base("Handaxe", 2, 5, "1d6", "slashing", 5, Rarity.Common, new List<WeaponProperty> { WeaponProperty.Light, WeaponProperty.Thrown }, "A small axe.")
        {
        }
    }

    public class LightHammer : Weapon
    {
        public LightHammer() : base("Light Hammer", 2, 5, "1d4", "bludgeoning", 5, Rarity.Common, new List<WeaponProperty> { WeaponProperty.Light, WeaponProperty.Thrown }, "A small hammer.")
        {
        }
    }

    public class LongSword : Weapon
    {
        public LongSword() : base("Longsword", 3, 15, "1d8", "slashing", 5, Rarity.Common, new List<WeaponProperty> { WeaponProperty.Versatile }, "A versatile blade.")
        {
        }
    }

    public class BattleAxe : Weapon
    {
        public BattleAxe() : base("Battle Axe", 4, 10, "1d10", "slashing", 5, Rarity.Common, new List<WeaponProperty> { WeaponProperty.Heavy, WeaponProperty.TwoHanded }, "A heavy combat axe.")
        {
        }
    }

    public class Greatclub : Weapon
    {
        public Greatclub() : base("Greatclub", 10, 2, "1d10", "bludgeoning", 5, Rarity.Common, new List<WeaponProperty> { WeaponProperty.Heavy, WeaponProperty.TwoHanded }, "A large club.")
        {
        }
    }

    public class Dagger : Weapon
    {
        public Dagger() : base("Dagger", 1, 5, "1d4", "piercing", 5, Rarity.Common, new List<WeaponProperty> { WeaponProperty.Light, WeaponProperty.Versatile, WeaponProperty.Thrown }, "A quick blade.")
        {
        }
    }

    public class Shortbow : Weapon
    {
        public Shortbow() : base("Shortbow", 2, 25, "1d6", "piercing", 80, Rarity.Common, new List<WeaponProperty> { WeaponProperty.Ammunition, WeaponProperty.Light, WeaponProperty.TwoHanded }, "A small bow.")
        {
        }
    }

    public class CrossbowLight : Weapon
    {
        public CrossbowLight() : base("Light Crossbow", 5, 25, "1d10", "piercing", 80, Rarity.Common, new List<WeaponProperty> { WeaponProperty.Ammunition, WeaponProperty.Heavy, WeaponProperty.Loading }, "A ranged weapon.")
        {
        }
    }

    //==========================
    // ARMOR hierarchy
    //==========================

    /// <summary>
    /// Base class for armor.
    /// </summary>
    public class Armor : Item
    {
        protected int _baseAC; // Base armor class bonus
        protected ArmorType _armorType;
        protected bool _isEquipped;
        protected string _strengthRequirement; // "STR 13" or ""

        protected Armor() : base()
        {
            _baseAC = 0;
            _armorType = ArmorType.Light;
            _isEquipped = false;
            _strengthRequirement = "";
        }

        public Armor(string name, double weight, int value, int baseAC, ArmorType armorType, Rarity rarity = Rarity.Common, string strengthRequirement = "", string description = "")
            : base(name, weight, value, rarity, description)
        {
            _baseAC = baseAC;
            _armorType = armorType;
            _strengthRequirement = strengthRequirement;
            _isEquipped = false;
        }

        // Properties
        public int BaseAC { get => _baseAC; set => _baseAC = value; }
        public ArmorType ArmorType { get => _armorType; set => _armorType = value; }
        public bool IsEquipped { get => _isEquipped; set => _isEquipped = value; }
        public string StrengthRequirement { get => _strengthRequirement; set => _strengthRequirement = value; }

        // Methods
        public virtual void Equip()
        {
            if (!string.IsNullOrEmpty(_strengthRequirement))
            {
                Console.WriteLine($"Note: {_name} requires {_strengthRequirement}.");
            }
            _isEquipped = true;
            Console.WriteLine($"{_name} ({_armorType}) is equipped. AC: +{_baseAC}{(!string.IsNullOrEmpty(_strengthRequirement) ? $" (STR required)" : "")}");
        }

        public virtual void Unequip()
        {
            _isEquipped = false;
            Console.WriteLine($"{_name} is unequipped.");
        }

        public override void DisplayInfo()
        {
            string rarityStr = _rarity != Rarity.Common ? $" [{_rarity}]" : "";
            string equippedStr = _isEquipped ? " (Equipped)" : "";
            string strReq = !string.IsNullOrEmpty(_strengthRequirement) ? $" | {_strengthRequirement}" : "";
            
            Console.WriteLine($"  Armor: {_name}{rarityStr}{equippedStr} | AC: +{_baseAC} ({_armorType}){strReq} | Weight: {_weight} lbs | Value: {_value} gp");
        }
    }

    // Light armor
    public class LeatherArmor : Armor
    {
        public LeatherArmor() : base("Leather Armor", 2, 10, 11, ArmorType.Light, Rarity.Common, "", "Soft leather armor.")
        {
        }
    }

    public class StuddedLeather : Armor
    {
        public StuddedLeather() : base("Studded Leather", 5, 45, 12, ArmorType.Light, Rarity.Common, "", "Reinforced leather armor.")
        {
        }
    }

    // Medium armor
    public class Hide : Armor
    {
        public Hide() : base("Hide", 5, 10, 12, ArmorType.Medium, Rarity.Common, "", "Armor made from animal hides.")
        {
        }
    }

    public class ChainShirt : Armor
    {
        public ChainShirt() : base("Chain Shirt", 6, 50, 13, ArmorType.Medium, Rarity.Common, "", "A shirt of metal rings.")
        {
        }
    }

    // Heavy armor
    public class ChainMail : Armor
    {
        public ChainMail() : base("Chain Mail", 3, 75, 16, ArmorType.Heavy, Rarity.Common, "STR 13", "Full chain mail armor.")
        {
        }
    }

    //==========================
    // SHIELD hierarchy
    //==========================

    /// <summary>
    /// Represents a shield.
    /// </summary>
    public class ShieldItem : Item
    {
        protected int _bonusAC;
        protected bool _isEquipped;

        public ShieldItem() : base("Shield", 5, 10)
        {
            _bonusAC = 2;
            _isEquipped = false;
        }

        public ShieldItem(string name, double weight, int value, int bonusAC, Rarity rarity = Rarity.Common, string description = "")
            : base(name, weight, value, rarity, description)
        {
            _bonusAC = bonusAC;
            _isEquipped = false;
        }

        // Properties
        public int BonusAC { get => _bonusAC; set => _bonusAC = value; }
        public bool IsEquipped { get => _isEquipped; set => _isEquipped = value; }

        // Methods
        public virtual void Equip()
        {
            _isEquipped = true;
            Console.WriteLine($"{_name} is equipped. AC bonus: +{_bonusAC}");
        }

        public virtual void Unequip()
        {
            _isEquipped = false;
            Console.WriteLine($"{_name} is unequipped.");
        }

        public override void DisplayInfo()
        {
            string rarityStr = _rarity != Rarity.Common ? $" [{_rarity}]" : "";
            string equippedStr = _isEquipped ? " (Equipped)" : "";
            Console.WriteLine($"  Shield: {_name}{rarityStr}{equippedStr} | AC: +{_bonusAC} | Weight: {_weight} lbs | Value: {_value} gp");
        }
    }

    public class StandardShield : ShieldItem
    {
        public StandardShield() : base("Shield", 5, 10, 2, Rarity.Common, "A standard wooden shield reinforced with metal.")
        {
        }
    }

    public class RattanShield : ShieldItem
    {
        public RattanShield() : base("Rattan Shield", 3, 5, 1, Rarity.Common, "A light shield made of woven reeds.")
        {
        }
    }

    //==========================
    // POTION hierarchy
    //==========================

    /// <summary>
    /// Represents a potion that provides healing when consumed.
    /// Player rolls dice and inputs the result.
    /// </summary>
    public class Potion : Item
    {
        protected string _effectDescription;
        protected bool _isConsumed;

        public Potion() : base("Potion", 1, 50)
        {
            _effectDescription = "Restores hit points when consumed.";
            _isConsumed = false;
        }

        public Potion(string name, double weight, int value, Rarity rarity, string effectDescription)
            : base(name, weight, value, rarity, effectDescription)
        {
            _effectDescription = effectDescription;
            _isConsumed = false;
        }

        // Properties
        public bool IsConsumed { get => _isConsumed; set => _isConsumed = value; }

        // Methods
        /// <summary>
        /// Use this potion by providing the healed amount (player rolls dice and inputs result).
        /// </summary>
        public virtual void Drink(int healedAmount)
        {
            if (_isConsumed)
            {
                Console.WriteLine($"{_name} has already been consumed!");
                return;
            }

            _isConsumed = true;
            Console.WriteLine($"{_name} is consumed! You heal {healedAmount} hit points.");
        }

        public override void DisplayInfo()
        {
            string rarityStr = _rarity != Rarity.Common ? $" [{_rarity}]" : "";
            string consumedStr = _isConsumed ? " (Consumed)" : "";
            Console.WriteLine($"  Potion: {_name}{rarityStr}{consumedStr} | {_effectDescription} | Value: {_value} gp");
        }
    }

    // Standard healing potion variants (per D&D 5e rules)
    public class PotionOfHealing : Potion
    {
        private readonly int _diceSpecification; // e.g., 2 for 2d4
        private readonly string _diceNotation;   // e.g., "2d4"

        public PotionOfHealing(int diceCount, int dieType) 
            : base(
                $"Potion of Healing ({diceCount}d{dieType})",
                0.5,
                diceCount * dieType / 2, // Approximate value based on healing potential
                Rarity.Common,
                $"Heals {diceCount}d{dieType} hit points. Player rolls and inputs result."
              )
        {
            _diceSpecification = diceCount;
            _diceNotation = $"{diceCount}d{dieType}";
        }

        public string DiceNotation { get => _diceNotation; }

        public override void Drink(int healedAmount)
        {
            if (_isConsumed)
            {
                Console.WriteLine($"This {_name} has already been consumed!");
                return;
            }

            _isConsumed = true;
            Console.WriteLine($"You drink the Potion of Healing ({_diceNotation}) and heal {healedAmount} hit points!");
        }
    }

    //==========================
    // TOOL hierarchy
    //==========================

    /// <summary>
    /// Represents a tool.
    /// </summary>
    public class Tool : Item
    {
        protected string _toolType;
        protected bool _isProficient;

        public Tool() : base("Tool", 2, 10)
        {
            _toolType = "generic";
            _isProficient = false;
        }

        public Tool(string name, double weight, int value, string toolType, Rarity rarity = Rarity.Common)
            : base(name, weight, value, rarity, $"A {toolType.ToLower()} tool.")
        {
            _toolType = toolType;
            _isProficient = false;
        }

        // Properties
        public string ToolType { get => _toolType; set => _toolType = value; }
        public bool IsProficient { get => _isProficient; set => _isProficient = value; }

        // Methods
        public virtual void UseTool()
        {
            string proficiencyStr = _isProficient ? " (with proficiency)" : "";
            Console.WriteLine($"{_name} ({_toolType}) is used{proficiencyStr}.");
        }

        public override void DisplayInfo()
        {
            string rarityStr = _rarity != Rarity.Common ? $" [{_rarity}]" : "";
            string proficientStr = _isProficient ? " (Proficient)" : "";
            Console.WriteLine($"  Tool: {_name}{rarityStr}{proficientStr} | Type: {_toolType} | Weight: {_weight} lbs | Value: {_value} gp");
        }
    }

    //==========================
    // ADVENTURING GEAR hierarchy
    //==========================

    /// <summary>
    /// Represents adventuring gear.
    /// </summary>
    public class AdventuringGear : Item
    {
        protected string _category;

        public AdventuringGear() : base("Adventuring Gear", 1, 0)
        {
            _category = "General";
        }

        public AdventuringGear(string name, double weight, int value, string category, Rarity rarity = Rarity.Common, string description = "")
            : base(name, weight, value, rarity, description)
        {
            _category = category;
        }

        // Properties
        public string Category { get => _category; set => _category = value; }

        // Methods
        public override void DisplayInfo()
        {
            string rarityStr = _rarity != Rarity.Common ? $" [{_rarity}]" : "";
            Console.WriteLine($"  Gear: {_name}{rarityStr} | Category: {_category} | {_description} | Weight: {_weight} lbs | Value: {_value} gp");
        }
    }

    // Common adventuring gear
    public class Rope : AdventuringGear
    {
        public Rope() : base("Rope", 10, 1, "Equipment", Rarity.Common, "10-foot length of hemp rope.")
        {
        }
    }

    public class Torch : AdventuringGear
    {
        public Torch() : base("Torch", 1, 1, "Light Source", Rarity.Common, "Illuminates in a 20-foot radius for 1 hour.")
        {
        }
    }

    public class Rations : AdventuringGear
    {
        public Rations() : base("Rations (1 day)", 2, 5, "Food", Rarity.Common, "Survival food rations.")
        {
        }
    }

    public class Waterskin : AdventuringGear
    {
        public Waterskin() : base("Waterskin", 5, 1, "Container", Rarity.Common, "Holds up to 4 pints of water.")
        {
        }
    }

    //==========================
    // MAGIC ITEM hierarchy
    //==========================

    /// <summary>
    /// Represents a magic item with bonuses.
    /// </summary>
    public class MagicItem : Item
    {
        protected string _bonusType; // e.g., "Attack Roll", "Damage Roll", "AC"
        protected int _bonusValue;   // e.g., 1 for +1
        protected bool _isEquipped;

        public MagicItem() : base()
        {
            _bonusType = "";
            _bonusValue = 0;
            _isEquipped = false;
        }

        public MagicItem(string name, double weight, int value, Rarity rarity, string bonusType, int bonusValue, string description = "")
            : base(name, weight, value, rarity, description)
        {
            _bonusType = bonusType;
            _bonusValue = bonusValue;
            _isEquipped = false;
        }

        // Properties
        public string BonusType { get => _bonusType; set => _bonusType = value; }
        public int BonusValue { get => _bonusValue; set => _bonusValue = value; }
        public bool IsEquipped { get => _isEquipped; set => _isEquipped = value; }

        // Methods
        public virtual void Equip()
        {
            _isEquipped = true;
            Console.WriteLine($"{_name} (+{_bonusValue} {_bonusType}) is equipped.");
        }

        public virtual void Unequip()
        {
            _isEquipped = false;
            Console.WriteLine($"{_name} is unequipped.");
        }

        public override void DisplayInfo()
        {
            string rarityStr = _rarity != Rarity.Common ? $" [{_rarity}]" : "";
            string equippedStr = _isEquipped ? " (Equipped)" : "";
            Console.WriteLine($"  Magic Item: {_name}{rarityStr}{equippedStr} | +{_bonusValue} {_bonusType} | Value: {_value} gp ({_rarity})");
        }
    }

    /// <summary>
    /// A magic weapon with attack and damage bonuses.
    /// Inherits from Weapon and adds magical properties.
    /// </summary>
    public class MagicWeapon : Weapon
    {
        protected string _magicalBonusType;
        protected int _magicalBonusValue;

        public MagicWeapon(string name, double weight, int value, Rarity rarity, string damageDice, string damageType, int bonusValue, string description = "")
            : base(name, weight, value, damageDice, damageType, 5, rarity, description: description)
        {
            _magicalBonusValue = bonusValue;
            _magicalBonusType = "Attack & Damage";
            
            // Update description if not provided
            if (string.IsNullOrEmpty(description))
            {
                _description = $"A +{bonusValue} magic weapon.";
            }
        }

        // Properties
        public string MagicalBonusType { get => _magicalBonusType; set => _magicalBonusType = value; }
        public int MagicalBonusValue { get => _magicalBonusValue; set => _magicalBonusValue = value; }

        public override void DisplayInfo()
        {
            string rarityStr = _rarity != Rarity.Common ? $" [{_rarity}]" : "";
            string equippedStr = _isEquipped ? " (Equipped)" : "";
            Console.WriteLine($"  Magic Weapon: {_name}{rarityStr}{equippedStr} | +{_magicalBonusValue} {_magicalBonusType} | Damage: {_damageDice}{_damageType} | Value: {_value} gp");
        }

        /// <summary>
        /// Roll damage including the magical bonus.
        /// </summary>
        public override int RollDamage()
        {
            int baseDamage = base.RollDamage();
            return baseDamage + _magicalBonusValue;
        }
    }

    // Common magic weapons
    public class LongSwordPlusOne : LongSword
    {
        public LongSwordPlusOne() : base()
        {
            _name = "Longsword +1";
            _value = 500;
            _rarity = Rarity.Uncommon;
            _description = "A +1 magic longsword.";
        }
    }

    public class HandaxeOfForce : Handaxe
    {
        public HandaxeOfForce() : base()
        {
            _name = "Handaxe of Force";
            _value = 1000;
            _rarity = Rarity.Rare;
            _description = "A +1 magic handaxe that deals force damage.";
        }
    }
}
