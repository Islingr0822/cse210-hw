using System;

namespace DnDCharacterManager.Item
{
    /// <summary>
    /// Abstract base class for all items.
    /// </summary>
    public abstract class Item
    {
        protected string _name;
        protected double _weight;
        protected int _value; // Value in gold pieces

        protected Item()
        {
            _name = "Unnamed Item";
            _weight = 0;
            _value = 0;
        }

        public Item(string name, double weight, int value)
        {
            _name = name;
            _weight = weight;
            _value = value;
        }

        // Properties
        public string Name { get => _name; set => _name = value; }
        public double Weight { get => _weight; set => _weight = value; }
        public int Value { get => _value; set => _value = value; }

        // Methods
        public virtual void DisplayInfo()
        {
            Console.WriteLine($"  Item: {_name} | Weight: {_weight} lbs | Value: {_value} gp");
        }

        public override string ToString()
        {
            return $"{_name} (Weight: {_weight}, Value: {_value} gp)";
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
        protected int _range; // Melee or range in feet

        protected Weapon() : base()
        {
            _damageDice = "1d4";
            _damageType = "bludgeoning";
            _range = 5;
        }

        public Weapon(string name, double weight, int value, string damageDice, string damageType, int range = 5)
            : base(name, weight, value)
        {
            _damageDice = damageDice;
            _damageType = damageType;
            _range = range;
        }

        // Properties
        public string DamageDice { get => _damageDice; set => _damageDice = value; }
        public string DamageType { get => _damageType; set => _damageType = value; }
        public int Range { get => _range; set => _range = value; }

        // Methods
        public virtual int RollDamage()
        {
            // Parse damage dice (e.g., "1d8" -> roll 1d8)
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
            string rangeStr = _range > 0 ? $" | Range: {_range}ft" : " | Melee";
            Console.WriteLine($"  Weapon: {_name} | Damage: {_damageDice}{_damageType}{rangeStr} | Value: {_value} gp");
        }
    }

    // Common weapons
    public class LongSword : Weapon
    {
        public LongSword() : base("Longsword", 3, 15, "1d8", "slashing", 5)
        {
        }
    }

    public class Shortbow : Weapon
    {
        public Shortbow() : base("Shortbow", 2, 25, "1d6", "piercing", 80)
        {
        }
    }

    public class Dagger : Weapon
    {
        public Dagger() : base("Dagger", 1, 5, "1d4", "piercing", 20)
        {
        }
    }

    public class BattleAxe : Weapon
    {
        public BattleAxe() : base("Battle Axe", 4, 10, "1d10", "slashing", 5)
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
        protected int _armorClass;
        protected string _armorType;

        protected Armor() : base()
        {
            _armorClass = 0;
            _armorType = "Light";
        }

        public Armor(string name, double weight, int value, int armorClass, string armorType)
            : base(name, weight, value)
        {
            _armorClass = armorClass;
            _armorType = armorType;
        }

        // Properties
        public int ArmorClass { get => _armorClass; set => _armorClass = value; }
        public string ArmorType { get => _armorType; set => _armorType = value; }

        // Methods
        public virtual void Equip()
        {
            Console.WriteLine($"{_name} ({_armorType}) is equipped. AC bonus: +{_armorClass}");
        }
    }

    // Common armors
    public class LeatherArmor : Armor
    {
        public LeatherArmor() : base("Leather Armor", 2, 10, 11, "Light")
        {
        }
    }

    public class ChainMail : Armor
    {
        public ChainMail() : base("Chain Mail", 3, 75, 16, "Heavy")
        {
        }
    }

    public class ChainMailMail : Armor
    {
        public ChainMailMail() : base("Chainmail", 2, 75, 16, "Medium")
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

        public ShieldItem() : base("Shield", 5, 10)
        {
            _bonusAC = 2;
        }

        public ShieldItem(string name, double weight, int value, int bonusAC)
            : base(name, weight, value)
        {
            _bonusAC = bonusAC;
        }

        // Properties
        public int BonusAC { get => _bonusAC; set => _bonusAC = value; }

        // Methods
        public virtual void Equip()
        {
            Console.WriteLine($"{_name} is equipped. AC bonus: +{_bonusAC}");
        }
    }

    //==========================
    // POTION hierarchy
    //==========================

    /// <summary>
    /// Represents a healing potion.
    /// </summary>
    public class Potion : Item
    {
        protected string _effect;
        protected int _healingAmount;

        public Potion() : base("Potion", 1, 50)
        {
            _effect = "Heals hit points";
            _healingAmount = 10; // Simplified default healing
        }

        public Potion(string name, double weight, int value, string effect, int healingAmount)
            : base(name, weight, value)
        {
            _effect = effect;
            _healingAmount = healingAmount;
        }

        // Properties
        public string Effect { get => _effect; set => _effect = value; }
        public int HealingAmount { get => _healingAmount; set => _healingAmount = value; }

        // Methods
        public virtual void Drink()
        {
            Random rand = new Random();
            int healed = 0;
            string[] parts = _healingAmount.ToString().Split('d');
            if (parts.Length == 2 && int.TryParse(parts[0], out int numDice) && int.TryParse(parts[1], out int dieSides))
            {
                for (int i = 0; i < numDice; i++)
                {
                    healed += rand.Next(1, dieSides + 1);
                }
            }
            Console.WriteLine($"{_name} is consumed! Heals {_healingAmount} hit points.");
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

        public Tool() : base("Tool", 2, 10)
        {
            _toolType = "generic";
        }

        public Tool(string name, double weight, int value, string toolType)
            : base(name, weight, value)
        {
            _toolType = toolType;
        }

        // Properties
        public string ToolType { get => _toolType; set => _toolType = value; }

        // Methods
        public virtual void UseTool()
        {
            Console.WriteLine($"{_name} ({_toolType}) is used.");
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
        protected string _description;

        public AdventuringGear() : base("Adventuring Gear", 1, 0)
        {
            _description = "Generic adventuring gear.";
        }

        public AdventuringGear(string name, double weight, int value, string description)
            : base(name, weight, value)
        {
            _description = description;
        }

        // Properties
        public string Description { get => _description; set => _description = value; }

        // Methods
        public override void DisplayInfo()
        {
            Console.WriteLine($"  Gear: {_name} | {_description} | Weight: {_weight} lbs | Value: {_value} gp");
        }
    }

    // Common adventuring gear
    public class Rope : AdventuringGear
    {
        public Rope() : base("Rope", 10, 1, "10-foot length of hemp rope.")
        {
        }
    }

    public class Torch : AdventuringGear
    {
        public Torch() : base("Torch", 1, 1, "Illuminates in a 20-foot radius for 1 hour.")
        {
        }
    }

    public class Rations : AdventuringGear
    {
        public Rations() : base("Rations (1 day)", 2, 5, "Survival food rations.")
        {
        }
    }
}