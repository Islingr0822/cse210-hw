using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ItemClass = DnDCharacterManager.Item.Item;

namespace DnDCharacterManager.Inventory
{
    /// <summary>
    /// Represents equipment slots in D&D 5e where items can be equipped.
    /// </summary>
    public enum EquipmentSlot
    {
        None,
        Weapon,
        Armor,
        Shield,
        Neck,
        Ring,
        Belt,
        Helm,
        Other
    }

    /// <summary>
    /// Manages a character's inventory including items, gold, and equipment.
    /// </summary>
    public class Inventory
    {
        // ==================== Core Fields ====================

        protected List<ItemClass> _items;
        protected Dictionary<string, int> _itemQuantities; // Track quantities for stackable items
        protected int _capacity;
        protected string _ownerName;

        // ==================== Gold/Currency Fields ====================

        protected int _gold;
        protected int _silver;
        protected int _copper;

        // ==================== Equipment Fields ====================

        protected Dictionary<EquipmentSlot, ItemClass?> _equippedItems;
        protected const int EQUIPPED_SLOT_COUNT = 8;

        // ==================== Events ====================

        /// <summary>
        /// Triggered when an item is added to the inventory.
        /// </summary>
        public event Action<Inventory, ItemClass>? OnItemAdded;

        /// <summary>
        /// Triggered when an item is removed from the inventory.
        /// </summary>
        public event Action<Inventory, string>? OnItemRemoved;

        /// <summary>
        /// Triggered when gold is added to the inventory.
        /// </summary>
        public event Action<Inventory, int>? OnGoldAdded;

        /// <summary>
        /// Triggered when gold is removed from the inventory.
        /// </summary>
        public event Action<Inventory, int>? OnGoldRemoved;

        // ==================== Constructors ====================

        /// <summary>
        /// Creates a default unnamed inventory with 50 capacity and no currency.
        /// </summary>
        public Inventory()
        {
            _items = new List<ItemClass>();
            _itemQuantities = new Dictionary<string, int>();
            _capacity = 50;
            _ownerName = "Unnamed";
            _gold = 0;
            _silver = 0;
            _copper = 0;
            _equippedItems = new Dictionary<EquipmentSlot, ItemClass?>();

            InitializeEquipmentSlots();
        }

        /// <summary>
        /// Creates a named inventory with specified capacity and starting currency.
        /// </summary>
        /// <param name="ownerName">The name of the inventory owner.</param>
        /// <param name="capacity">Maximum number of items the inventory can hold.</param>
        /// <param name="startingGold">Gold pieces to start with.</param>
        /// <param name="startingSilver">Silver pieces to start with.</param>
        /// <param name="startingCopper">Copper pieces to start with.</param>
        public Inventory(string ownerName, int capacity = 50, int startingGold = 0, int startingSilver = 0, int startingCopper = 0)
        {
            _ownerName = ownerName;
            _capacity = capacity;
            _gold = startingGold;
            _silver = startingSilver;
            _copper = startingCopper;

            _items = new List<ItemClass>();
            _itemQuantities = new Dictionary<string, int>();
            _equippedItems = new Dictionary<EquipmentSlot, ItemClass?>();

            InitializeEquipmentSlots();
        }

        /// <summary>
        /// Initializes all equipment slots to null.
        /// </summary>
        private void InitializeEquipmentSlots()
        {
            foreach (EquipmentSlot slot in Enum.GetValues(typeof(EquipmentSlot)))
            {
                if (slot != EquipmentSlot.None)
                {
                    _equippedItems[slot] = null;
                }
            }
        }

        // ==================== Properties ====================

        public List<ItemClass> Items { get => _items; set => _items = value; }
        public Dictionary<string, int> ItemQuantities { get => _itemQuantities; set => _itemQuantities = value; }
        public int Capacity { get => _capacity; set => _capacity = Math.Max(1, value); }
        public string OwnerName { get => _ownerName; set => _ownerName = value; }

        // Currency Properties
        public int Gold { get => _gold; set => _gold = Math.Max(0, value); }
        public int Silver { get => _silver; set => _silver = Math.Max(0, value); }
        public int Copper { get => _copper; set => _copper = Math.Max(0, value); }

        /// <summary>
        /// Total wealth converted to gold pieces (1 Gold = 10 Silver = 100 Copper).
        /// </summary>
        public int TotalGoldValue => _gold + (_silver / 10) + (_copper / 100);

        /// <summary>
        /// Dictionary of equipped items by slot.
        /// </summary>
        public Dictionary<EquipmentSlot, ItemClass?> EquippedItems { get => _equippedItems; set => _equippedItems = value; }

        /// <summary>
        /// Number of current item slots used in the inventory.
        /// </summary>
        public int CurrentItemCount => _items.Count;

        /// <summary>
        /// Number of empty slots remaining.
        /// </summary>
        public int RemainingSlots => _capacity - _items.Count;

        /// <summary>
        /// Total weight of all items in the inventory (including equipped items).
        /// </summary>
        public double TotalWeight { get => GetTotalWeight(); }

        // ==================== Equipment Management ====================

        /// <summary>
        /// Attempts to equip an item from the inventory into the appropriate slot.
        /// Automatically unequips any item already in that slot.
        /// </summary>
        /// <param name="item">The item to equip.</param>
        /// <returns>The equipped item, or null if equipping failed.</returns>
        public virtual ItemClass? EquipItem(ItemClass item)
        {
            if (item == null)
            {
                Console.WriteLine("Cannot equip a null item.");
                return null;
            }

            EquipmentSlot slot = DetermineEquipmentSlot(item);
            if (slot == EquipmentSlot.None)
            {
                Console.WriteLine($"'{item.Name}' cannot be equipped.");
                return null;
            }

            // Unequip current item in this slot if any
            if (_equippedItems[slot] is not null)
            {
                Console.WriteLine($"Unequipping '{_equippedItems[slot]!.Name}' from {slot} slot.");
                RemoveItem(_equippedItems[slot]!.Name);
            }

            // Remove from inventory list (if it's in there)
            if (_items.Contains(item))
            {
                _items.Remove(item);
                UpdateQuantityTracking(item.Name, -1);
            }

            // Equip the new item
            _equippedItems[slot] = item;

            // Call Equip method on the item if it has one
            EquipItemOnObject(item);

            Console.WriteLine($"{_ownerName} equips '{item.Name}' in the {slot} slot.");
            OnItemAdded?.Invoke(this, item);

            return item;
        }

        /// <summary>
        /// Unequips an item from a specific slot, returning it to the inventory.
        /// </summary>
        /// <param name="slot">The equipment slot to unequip from.</param>
        /// <returns>The unequipped item, or null if nothing was in that slot.</returns>
        public virtual ItemClass? UnequipItem(EquipmentSlot slot)
        {
            if (slot == EquipmentSlot.None || _equippedItems[slot] is null)
            {
                Console.WriteLine($"Nothing equipped in the {slot} slot.");
                return null;
            }

            ItemClass? item = _equippedItems[slot];

            // Call Unequip method on the item if it has one
            UnequipItemOnObject(item!);

            // Add back to inventory
            _items.Add(item!);
            UpdateQuantityTracking(item!.Name, 1);

            Console.WriteLine($"{_ownerName} unequips '{item!.Name}' from the {slot} slot.");

            _equippedItems[slot] = null;
            OnItemRemoved?.Invoke(this, item!.Name);

            return item;
        }

        /// <summary>
        /// Unequips an item by its name.
        /// </summary>
        /// <param name="itemName">The name of the item to unequip.</param>
        /// <returns>The equipment slot that was freed, or None.</returns>
        public virtual EquipmentSlot UnequipItem(string itemName)
        {
            foreach (EquipmentSlot slot in Enum.GetValues(typeof(EquipmentSlot)))
            {
                if (slot == EquipmentSlot.None) continue;

                if (_equippedItems[slot] is not null && _equippedItems[slot]!.Name == itemName)
                {
                    UnequipItem(slot);
                    return slot;
                }
            }
            Console.WriteLine($"No equipped item named '{itemName}' found.");
            return EquipmentSlot.None;
        }

        /// <summary>
        /// Gets the equipment slot type for a given item.
        /// </summary>
        private EquipmentSlot DetermineEquipmentSlot(ItemClass item)
        {
            // Check specific types from DnDCharacterManager.Item namespace
            string itemType = item.GetType().Name;

            return itemType switch
            {
                "LongSword" or "LongSwordPlusOne" or "Club" or "Quarterstaff" or "Sickle" or
                "Handaxe" or "LightHammer" or "BattleAxe" or "Greatclub" or "Dagger" or
                "Shortbow" or "CrossbowLight" or "HandaxeOfForce" => EquipmentSlot.Weapon,

                "LeatherArmor" or "StuddedLeather" or "Hide" or "ChainShirt" or "ChainMail" => EquipmentSlot.Armor,

                "StandardShield" or "RattanShield" => EquipmentSlot.Shield,

                _ => EquipmentSlot.Other
            };
        }

        /// <summary>
        /// Calls the Equip method on an item if available.
        /// </summary>
        private void EquipItemOnObject(ItemClass item)
        {
            try
            {
                var equipMethod = item.GetType().GetMethod("Equip");
                equipMethod?.Invoke(item, null);
            }
            catch (Exception)
            {
                // Item doesn't have an Equip method, that's fine
            }
        }

        /// <summary>
        /// Calls the Unequip method on an item if available.
        /// </summary>
        private void UnequipItemOnObject(ItemClass item)
        {
            try
            {
                var unequipMethod = item.GetType().GetMethod("Unequip");
                unequipMethod?.Invoke(item, null);
            }
            catch (Exception)
            {
                // Item doesn't have an Unequip method, that's fine
            }
        }

        /// <summary>
        /// Gets an item from a specific equipment slot.
        /// </summary>
        public virtual ItemClass? GetEquippedItem(EquipmentSlot slot)
        {
            return _equippedItems.ContainsKey(slot) ? _equippedItems[slot] : null;
        }

        /// <summary>
        /// Gets all equipped items as a list.
        /// </summary>
        public virtual List<ItemClass> GetAllEquippedItems()
        {
            return _equippedItems.Values.Where(i => i is not null).Select(i => i!).ToList();
        }

        /// <summary>
        /// Gets the total weight bonus from all equipped items.
        /// </summary>
        public virtual double GetEquippedWeightBonus()
        {
            double total = 0;
            foreach (var item in GetAllEquippedItems())
            {
                total += item.Weight;
            }
            return total;
        }

        // ==================== Item Management ====================

        /// <summary>
        /// Adds an item to the inventory. Respects capacity limits.
        /// </summary>
        /// <param name="item">The item to add.</param>
        /// <param name="quantity">Number of items to add (for stackable items). Defaults to 1.</param>
        /// <returns>True if the item was added successfully, false if inventory is full.</returns>
        public virtual bool AddItem(ItemClass item, int quantity = 1)
        {
            if (item == null)
            {
                Console.WriteLine("Cannot add a null item.");
                return false;
            }

            if (quantity <= 0)
            {
                Console.WriteLine("Quantity must be positive.");
                return false;
            }

            // Check capacity (only for first item, stacks share slots)
            if (quantity > RemainingSlots && _itemQuantities.GetValueOrDefault(item.Name, 0) == 0)
            {
                Console.WriteLine($"Inventory is full. No space for '{item.Name}'.");
                return false;
            }

            for (int i = 0; i < quantity; i++)
            {
                _items.Add(item);
            }

            // Track quantity for stackable items
            UpdateQuantityTracking(item.Name, quantity);

            string qtyStr = quantity > 1 ? $"{quantity}x " : "";
            Console.WriteLine($"{_ownerName} adds '{qtyStr}{item.Name}' to their inventory.");
            OnItemAdded?.Invoke(this, item);

            return true;
        }

        /// <summary>
        /// Removes items by name, with optional quantity.
        /// </summary>
        /// <param name="itemName">The name of the item to remove.</param>
        /// <param name="quantity">Number of items to remove. Defaults to 1.</param>
        /// <returns>True if items were removed, false if not found.</returns>
        public virtual bool RemoveItem(string itemName, int quantity = 1)
        {
            if (quantity <= 0)
            {
                Console.WriteLine("Quantity must be positive.");
                return false;
            }

            ItemClass? itemToRemove = _items.Find(i => i.Name == itemName);
            if (itemToRemove is null)
            {
                Console.WriteLine($"'{itemName}' not found in inventory.");
                return false;
            }

            int actualQuantity = Math.Min(quantity, _itemQuantities.GetValueOrDefault(itemName, 1));
            int removed = 0;

            for (int i = _items.Count - 1; i >= 0 && removed < actualQuantity; i--)
            {
                if (_items[i].Name == itemName)
                {
                    _items.RemoveAt(i);
                    removed++;
                }
            }

            UpdateQuantityTracking(itemName, -removed);
            string qtyStr = removed > 1 ? $"{removed}x " : "";
            Console.WriteLine($"{_ownerName} removes '{qtyStr}{itemName}' from their inventory.");
            OnItemRemoved?.Invoke(this, itemName);

            return true;
        }

        /// <summary>
        /// Updates the quantity tracking dictionary for an item.
        /// </summary>
        private void UpdateQuantityTracking(string itemName, int delta)
        {
            int current = _itemQuantities.GetValueOrDefault(itemName, 0);
            int newValue = current + delta;

            if (newValue <= 0)
            {
                _itemQuantities.Remove(itemName);
            }
            else
            {
                _itemQuantities[itemName] = newValue;
            }
        }

        /// <summary>
        /// Finds an item by name in the inventory.
        /// </summary>
        public virtual ItemClass? FindItem(string itemName)
        {
            return _items.Find(i => i.Name == itemName);
        }

        /// <summary>
        /// Finds all items matching a name in the inventory.
        /// </summary>
        public virtual List<ItemClass> FindAllItems(string itemName)
        {
            return _items.FindAll(i => i.Name == itemName);
        }

        /// <summary>
        /// Checks if the inventory contains a specific item.
        /// </summary>
        public virtual bool Contains(string itemName)
        {
            return _items.Any(i => i.Name == itemName);
        }

        /// <summary>
        /// Gets the quantity of a specific item in the inventory.
        /// </summary>
        public virtual int GetItemCount(string itemName)
        {
            return _itemQuantities.GetValueOrDefault(itemName, 0);
        }

        /// <summary>
        /// Clears all items from the inventory.
        /// </summary>
        public virtual void ClearInventory()
        {
            // Unequip all items first
            foreach (EquipmentSlot slot in Enum.GetValues(typeof(EquipmentSlot)))
            {
                if (_equippedItems[slot] is not null)
                {
                    var item = _equippedItems[slot];
                    UnequipItemOnObject(item!);
                    _equippedItems[slot] = null;
                }
            }

            int count = _items.Count;
            _items.Clear();
            _itemQuantities.Clear();

            Console.WriteLine($"{_ownerName} clears their entire inventory ({count} items removed).");
        }

        /// <summary>
        /// Checks if the inventory is at capacity.
        /// </summary>
        public virtual bool IsFull()
        {
            return _items.Count >= _capacity;
        }

        /// <summary>
        /// Checks if the inventory is empty.
        /// </summary>
        public virtual bool IsEmpty()
        {
            return _items.Count == 0;
        }

        // ==================== Weight Calculations ====================

        /// <summary>
        /// Gets the total weight of all items in the inventory plus equipped items.
        /// </summary>
        public virtual double GetTotalWeight()
        {
            double total = 0;

            // Inventory items
            foreach (var item in _items)
            {
                total += item.Weight;
            }

            // Equipped items
            foreach (var item in GetAllEquippedItems())
            {
                total += item.Weight;
            }

            return total;
        }

        /// <summary>
        /// Gets the carrying capacity based on Strength score.
        /// </summary>
        /// <param name="strength">The character's Strength score.</param>
        /// <returns>The maximum weight the character can carry.</returns>
        public virtual int GetCarryingCapacity(int strength)
        {
            return 15 * strength; // D&D 5e standard: 15 lbs per point of Strength
        }

        /// <summary>
        /// Checks if the character is carrying too much (encumbered).
        /// </summary>
        public virtual bool IsOverweight(int strength)
        {
            return GetTotalWeight() > GetCarryingCapacity(strength);
        }

        // ==================== Gold/Currency Management ====================

        /// <summary>
        /// Adds gold to the inventory. Automatically converts excess silver/copper.
        /// </summary>
        public virtual void AddGold(int amount)
        {
            if (amount <= 0) return;

            _gold += amount;
            Console.WriteLine($"{_ownerName} gains {amount} gold pieces. (Total: {_gold} GP)");
            OnGoldAdded?.Invoke(this, amount);
        }

        /// <summary>
        /// Adds silver to the inventory. Auto-converts to gold at 10+ silver.
        /// </summary>
        public virtual void AddSilver(int amount)
        {
            if (amount <= 0) return;

            _silver += amount;
            ConvertExcessSilver();
            Console.WriteLine($"{_ownerName} gains {amount} silver pieces. (Total: {_silver} SP, {_gold} GP)");
            OnGoldAdded?.Invoke(this, amount / 10); // Report in gold equivalent
        }

        /// <summary>
        /// Adds copper to the inventory. Auto-converts to silver at 100+ copper.
        /// </summary>
        public virtual void AddCopper(int amount)
        {
            if (amount <= 0) return;

            _copper += amount;
            ConvertExcessCopper();
            Console.WriteLine($"{_ownerName} gains {amount} copper pieces. (Total: {_copper} CP)");
            OnGoldAdded?.Invoke(this, amount / 100); // Report in gold equivalent
        }

        /// <summary>
        /// Adds a mixed currency amount to the inventory.
        /// </summary>
        public virtual void AddCurrency(int gold = 0, int silver = 0, int copper = 0)
        {
            int totalAdded = 0;
            string parts = "";

            if (gold > 0)
            {
                _gold += gold;
                totalAdded += gold;
                parts += gold + " GP";
            }
            if (silver > 0)
            {
                _silver += silver;
                ConvertExcessSilver();
                totalAdded += silver / 10;
                if (parts != "") parts += ", ";
                parts += silver + " SP";
            }
            if (copper > 0)
            {
                _copper += copper;
                ConvertExcessCopper();
                totalAdded += copper / 100;
                if (parts != "") parts += ", ";
                parts += copper + " CP";
            }

            if (totalAdded > 0 || parts != "")
            {
                Console.WriteLine($"{_ownerName} gains currency totaling {totalAdded} GP. ({parts})");
                OnGoldAdded?.Invoke(this, totalAdded);
            }
        }

        /// <summary>
        /// Attempts to remove gold from the inventory.
        /// </summary>
        /// <param name="amount">Amount of gold to remove.</param>
        /// <param name="fromLowestFirst">If true, deducts from copper first (realistic spending).</param>
        /// <returns>True if enough gold was available.</returns>
        public virtual bool RemoveGold(int amount, bool fromLowestFirst = true)
        {
            if (amount <= 0) return false;

            if (fromLowestFirst)
            {
                return RemoveFromLowestDenomination(amount);
            }

            // Remove from highest denomination first
            int remaining = amount;

            int goldToRemove = Math.Min(_gold, remaining);
            _gold -= goldToRemove;
            remaining -= goldToRemove;

            if (remaining > 0)
            {
                int silverToRemove = Math.Min(_silver, remaining * 10);
                _silver -= silverToRemove;
                remaining -= silverToRemove / 10;
            }

            if (remaining > 0)
            {
                int copperToRemove = Math.Min(_copper, remaining * 100);
                _copper -= copperToRemove;
                remaining -= copperToRemove / 100;
            }

            if (remaining == 0)
            {
                Console.WriteLine($"{_ownerName} spends {amount} gold pieces. (Remaining: {_gold} GP)");
                OnGoldRemoved?.Invoke(this, amount);
                return true;
            }

            return false;
        }

        /// <summary>
        /// Removes gold starting from the lowest denomination (copper -> silver -> gold).
        /// Simulates spending smaller coins first.
        /// </summary>
        private bool RemoveFromLowestDenomination(int amount)
        {
            int remaining = amount;

            // Spend copper first (100 cp = 1 gp)
            int copperToRemove = Math.Min(_copper, remaining * 100);
            _copper -= copperToRemove;
            remaining -= copperToRemove / 100;

            // Then silver (10 sp = 1 gp)
            if (remaining > 0)
            {
                int silverToRemove = Math.Min(_silver, remaining * 10);
                _silver -= silverToRemove;
                remaining -= silverToRemove / 10;
            }

            // Finally gold
            if (remaining > 0)
            {
                int goldToRemove = Math.Min(_gold, remaining);
                _gold -= goldToRemove;
                remaining -= goldToRemove;
            }

            if (remaining == 0)
            {
                Console.WriteLine($"{_ownerName} spends {amount} gold pieces (copper/silver first). (Remaining: {_gold} GP)");
                OnGoldRemoved?.Invoke(this, amount);
                return true;
            }

            // Failed - restore what was taken
            _copper += copperToRemove;
            Console.WriteLine($"{_ownerName} does not have enough gold! (Need {amount}, missing {remaining}).");
            return false;
        }

        /// <summary>
        /// Checks if the inventory has enough gold to afford something.
        /// </summary>
        public virtual bool CanAfford(int cost)
        {
            return TotalGoldValue >= cost;
        }

        /// <summary>
        /// Pays an amount, converting all currency as needed.
        /// Uses highest denominations first.
        /// </summary>
        public virtual bool PayAmount(int cost)
        {
            if (cost <= 0 || !CanAfford(cost))
            {
                return false;
            }

            int remaining = cost;

            // Gold first
            int goldPay = Math.Min(_gold, remaining);
            _gold -= goldPay;
            remaining -= goldPay;

            // Then silver
            if (remaining > 0)
            {
                int silverPay = Math.Min(_silver, remaining * 10);
                _silver -= silverPay;
                remaining -= silverPay / 10;
            }

            // Finally copper
            if (remaining > 0)
            {
                int copperPay = Math.Min(_copper, remaining * 100);
                _copper -= copperPay;
                remaining -= copperPay / 100;
            }

            if (remaining == 0)
            {
                Console.WriteLine($"{_ownerName} pays {cost} gold pieces. (Remaining: {_gold} GP, {_silver} SP, {_copper} CP)");
                OnGoldRemoved?.Invoke(this, cost);
                return true;
            }

            // Restore on failure
            _gold += goldPay;
            return false;
        }

        /// <summary>
        /// Converts excess silver (10+) into gold.
        /// </summary>
        private void ConvertExcessSilver()
        {
            int goldFromSilver = _silver / 10;
            if (goldFromSilver > 0)
            {
                _gold += goldFromSilver;
                _silver %= 10;
            }
        }

        /// <summary>
        /// Converts excess copper (100+) into silver.
        /// </summary>
        private void ConvertExcessCopper()
        {
            int silverFromCopper = _copper / 100;
            if (silverFromCopper > 0)
            {
                _silver += silverFromCopper;
                _copper %= 100;
                ConvertExcessSilver(); // Also convert any new excess silver
            }
        }

        // ==================== Categorized Views ====================

        /// <summary>
        /// Gets all items filtered by type category.
        /// </summary>
        /// <param name="category">Category: "weapon", "armor", "shield", "potion", "gear", "magic", "tool"</param>
        public virtual List<ItemClass> GetItemsByCategory(string category)
        {
            string cat = category.ToLowerInvariant();

            return cat switch
            {
                "weapon" => _items.Where(i => i.GetType().BaseType?.Name == "Weapon" || i.GetType().Name.StartsWith("Magic")).ToList(),
                "armor" => _items.Where(i => i.GetType().BaseType?.Name == "Armor").ToList(),
                "shield" => _items.Where(i => i.GetType().BaseType?.Name == "ShieldItem" || i.GetType().Name.Contains("Shield")).ToList(),
                "potion" => _items.Where(i => i.GetType().BaseType?.Name == "Potion").ToList(),
                "gear" => _items.Where(i => i.GetType().BaseType?.Name == "AdventuringGear").ToList(),
                "tool" => _items.Where(i => i.GetType().BaseType?.Name == "Tool").ToList(),
                "magic" => _items.Where(i => i.GetType().IsSubclassOf(typeof(ItemClass)) && i.GetType().Name.Contains("Magic")).ToList(),
                _ => _items.ToList()
            };
        }

        /// <summary>
        /// Gets a summary of equipped equipment.
        /// </summary>
        public virtual string GetEquipmentSummary()
        {
            var sb = new StringBuilder();
            sb.AppendLine("\n=== Equipped Equipment ===");

            foreach (EquipmentSlot slot in Enum.GetValues(typeof(EquipmentSlot)))
            {
                if (slot == EquipmentSlot.None) continue;

                var item = _equippedItems[slot];
                if (item is not null)
                {
                    sb.AppendLine($"  [{slot}]: {item.Name}");
                }
                else
                {
                    sb.AppendLine($"  [{slot}]: (empty)");
                }
            }

            sb.AppendLine("=== End Equipment ===\n");
            return sb.ToString();
        }

        /// <summary>
        /// Gets a compact summary of the entire inventory state.
        /// </summary>
        public virtual string GetInventorySummary()
        {
            var sb = new StringBuilder();
            sb.AppendLine($"=== {_ownerName}'s Inventory ===");
            sb.AppendLine($"Items: {_items.Count}/{_capacity} | Weight: {GetTotalWeight():F1} lbs");
            sb.AppendLine($"Wealth: {_gold} GP, {_silver} SP, {_copper} CP (Total: {TotalGoldValue} GP)");
            sb.AppendLine();

            if (_items.Count == 0)
            {
                sb.AppendLine("Inventory is empty.");
            }
            else
            {
                sb.AppendLine("Items:");
                foreach (var kvp in _itemQuantities.OrderBy(k => k.Key))
                {
                    sb.AppendLine($"  {kvp.Value}x {kvp.Key}");
                }
            }

            return sb.ToString();
        }

        // ==================== Trade/Transfer ====================

        /// <summary>
        /// Transfers an item to another inventory. Returns true if successful.
        /// </summary>
        public virtual bool TransferTo(Inventory target, string itemName, int quantity = 1)
        {
            if (!Contains(itemName))
            {
                Console.WriteLine($"{_ownerName} does not have '{itemName}' to transfer.");
                return false;
            }

            if (target.IsFull())
            {
                Console.WriteLine($"{target.OwnerName}'s inventory is full.");
                return false;
            }

            ItemClass? item = FindItem(itemName);
            if (item is null) return false;

            // Remove from source
            RemoveItem(itemName, quantity);

            // Add to target
            target.AddItem(item, quantity);
            Console.WriteLine($"{_ownerName} transfers '{itemName}' to {target.OwnerName}.");

            return true;
        }

        /// <summary>
        /// Swaps an item with another character's item.
        /// </summary>
        public virtual bool SwapItems(Inventory target, string myItem, string theirItem)
        {
            // Remove our item from our inventory
            bool removedFromMe = RemoveItem(myItem);

            // Try to add their item to our inventory
            var theirItemObj = target.FindItem(theirItem);
            if (theirItemObj is null || !removedFromMe)
            {
                if (!removedFromMe)
                {
                    // Try to restore our item
                    AddItem(theirItemObj!, 1);
                }
                Console.WriteLine($"Swap failed: '{myItem}' not found in our inventory or '{theirItem}' not in theirs.");
                return false;
            }

            // Remove from their inventory
            target.RemoveItem(theirItem);

            // Add to our inventory
            AddItem(theirItemObj, 1);
            Console.WriteLine($"{_ownerName} swaps '{myItem}' with {target.OwnerName}'s '{theirItem}'.");

            return true;
        }

        // ==================== Display Methods ====================

        /// <summary>
        /// Displays the full inventory to the console.
        /// </summary>
        public virtual void DisplayInventory()
        {
            string border = new('=', 40);
            Console.WriteLine($"\n{border}");
            Console.WriteLine($"  {_ownerName}'s Inventory ({_items.Count}/{_capacity} slots)");
            Console.WriteLine($"{border}");

            // Currency
            Console.WriteLine($"\n  --- Wealth ---");
            Console.WriteLine($"    Gold:   {_gold} GP");
            Console.WriteLine($"    Silver: {_silver} SP");
            Console.WriteLine($"    Copper: {_copper} CP");
            Console.WriteLine($"    Total:  {TotalGoldValue} GP equivalent");

            // Equipped Equipment
            var equipped = GetAllEquippedItems();
            if (equipped.Count > 0)
            {
                Console.WriteLine($"\n  --- Equipped ---");
                foreach (EquipmentSlot slot in Enum.GetValues(typeof(EquipmentSlot)))
                {
                    if (slot == EquipmentSlot.None) continue;
                    var item = _equippedItems[slot];
                    if (item is not null)
                    {
                        Console.WriteLine($"    [{slot}]: {item.Name}");
                    }
                }
            }

            // Items
            Console.WriteLine($"\n  --- Items ({_items.Count}) ---");
            if (_items.Count == 0)
            {
                Console.WriteLine("    Inventory is empty.");
            }
            else
            {
                foreach (var kvp in _itemQuantities.OrderBy(k => k.Key))
                {
                    Console.WriteLine($"    {kvp.Value}x {kvp.Key}");
                }
            }

            // Weight
            double totalWeight = GetTotalWeight();
            Console.WriteLine($"\n  Total Weight: {totalWeight:F1} lbs");

            Console.WriteLine($"{border}\n");
        }

        /// <summary>
        /// Displays just the items without formatting.
        /// </summary>
        public virtual void DisplayItems()
        {
            Console.WriteLine($"\n=== {_ownerName}'s Items ({_items.Count}/{_capacity}) ===");
            if (_items.Count == 0)
            {
                Console.WriteLine("Inventory is empty.");
            }
            else
            {
                foreach (var item in _items)
                {
                    item.DisplayInfo();
                }
            }
            Console.WriteLine($"Total Weight: {GetTotalWeight()} lbs\n");
        }

        // ==================== Serialization ====================

        /// <summary>
        /// Serializes the inventory state to a dictionary for saving/loading.
        /// </summary>
        public virtual Dictionary<string, object> Serialize()
        {
            var itemsData = new List<Dictionary<string, object>>();
            foreach (var item in _items)
            {
                itemsData.Add(new Dictionary<string, object>
                {
                    ["Name"] = item.Name,
                    ["Type"] = item.GetType().Name,
                    ["Weight"] = item.Weight,
                    ["Value"] = item.Value,
                    ["Rarity"] = ((int)item.Rarity).ToString()
                });
            }

            var equippedData = new List<Dictionary<string, string>>();
            foreach (var kvp in _equippedItems)
            {
                if (kvp.Value is not null)
                {
                    equippedData.Add(new Dictionary<string, string>
                    {
                        ["Slot"] = ((int)kvp.Key).ToString(),
                        ["ItemName"] = kvp.Value.Name,
                        ["ItemType"] = kvp.Value.GetType().Name
                    });
                }
            }

            return new Dictionary<string, object>
            {
                ["OwnerName"] = _ownerName,
                ["Capacity"] = _capacity,
                ["Gold"] = _gold,
                ["Silver"] = _silver,
                ["Copper"] = _copper,
                ["Items"] = itemsData,
                ["ItemQuantities"] = _itemQuantities,
                ["EquippedItems"] = equippedData
            };
        }

        /// <summary>
        /// Deserializes an inventory state from a dictionary.
        /// Note: This is a basic implementation. Full item reconstruction would require
        /// a factory or registration system for item types.
        /// </summary>
        public virtual void Deserialize(Dictionary<string, object> data)
        {
            // Clear current state
            _items.Clear();
            _itemQuantities.Clear();
            foreach (EquipmentSlot slot in Enum.GetValues(typeof(EquipmentSlot)))
            {
                _equippedItems[slot] = null;
            }

            if (data.TryGetValue("OwnerName", out var owner)) _ownerName = owner as string ?? _ownerName;
            if (data.TryGetValue("Capacity", out var capacity)) _capacity = Convert.ToInt32(capacity);
            if (data.TryGetValue("Gold", out var goldObj)) _gold = Convert.ToInt32(goldObj);
            if (data.TryGetValue("Silver", out var silverObj)) _silver = Convert.ToInt32(silverObj);
            if (data.TryGetValue("Copper", out var copperObj)) _copper = Convert.ToInt32(copperObj);

            if (data.TryGetValue("Items", out var itemsObj) && itemsObj is List<Dictionary<string, object>> itemsList)
            {
                foreach (var itemDict in itemsList)
                {
                    if (itemDict.TryGetValue("Name", out var name))
                    {
                        // In a full implementation, you would use a factory to create items by type
                        Console.WriteLine($"Deserialized item: {name}");
                    }
                }
            }

            if (data.TryGetValue("ItemQuantities", out var quantitiesObj) && quantitiesObj is Dictionary<string, int> quantitiesDict)
            {
                _itemQuantities = quantitiesDict;
            }
        }

        // ==================== Utility Methods ====================

        /// <summary>
        /// Sorts the inventory by item name alphabetically.
        /// </summary>
        public virtual void SortInventory()
        {
            _items.Sort((a, b) => string.Compare(a.Name, b.Name, StringComparison.OrdinalIgnoreCase));
            Console.WriteLine($"{_ownerName}'s inventory has been sorted.");
        }

        /// <summary>
        /// Organizes the inventory by grouping similar items together.
        /// </summary>
        public virtual void OrganizeInventory()
        {
            var weaponItems = _items.Where(i => i.GetType().BaseType?.Name == "Weapon").ToList();
            var armorItems = _items.Where(i => i.GetType().BaseType?.Name == "Armor").ToList();
            var potionItems = _items.Where(i => i.GetType().BaseType?.Name == "Potion").ToList();
            var otherItems = _items.Except(weaponItems.Concat(armorItems).Concat(potionItems)).ToList();

            _items.Clear();
            _items.AddRange(otherItems);
            _items.AddRange(weaponItems);
            _items.AddRange(armorItems);
            _items.AddRange(potionItems);

            Console.WriteLine($"{_ownerName}'s inventory has been organized.");
        }

        /// <summary>
        /// Prints a compact one-line summary.
        /// </summary>
        public override string ToString()
        {
            return $"{_ownerName}'s Inventory: {_items.Count}/{_capacity} items, {TotalGoldValue} GP total wealth";
        }
    }
}