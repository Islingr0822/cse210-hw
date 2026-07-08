using System;
using System.Collections.Generic;
using DnDCharacterManager.Item;

namespace DnDCharacterManager.Inventory
{
    /// <summary>
    /// Manages a character's inventory.
    /// </summary>
    public class Inventory
    {
        protected List<Item> _items;
        protected int _capacity;
        protected string _ownerName;

        public Inventory()
        {
            _items = new List<Item>();
            _capacity = 50;
            _ownerName = "Unnamed";
        }

        public Inventory(string ownerName, int capacity = 50)
        {
            _items = new List<Item>();
            _capacity = capacity;
            _ownerName = ownerName;
        }

        // Properties
        public List<Item> Items { get => _items; set => _items = value; }
        public int Capacity { get => _capacity; set => _capacity = value; }
        public string OwnerName { get => _ownerName; set => _ownerName = value; }

        // Methods
        public virtual void AddItem(Item item)
        {
            if (item == null)
            {
                Console.WriteLine("Cannot add a null item.");
                return;
            }

            _items.Add(item);
            Console.WriteLine($"{_ownerName} adds '{item.Name}' to their inventory.");
        }

        public virtual bool RemoveItem(string itemName)
        {
            Item? itemToRemove = _items.Find(i => i.Name == itemName);
            if (itemToRemove != null)
            {
                _items.Remove(itemToRemove);
                Console.WriteLine($"{_ownerName} removes '{itemName}' from their inventory.");
                return true;
            }
            Console.WriteLine($"'{itemName}' not found in inventory.");
            return false;
        }

        public virtual void DisplayInventory()
        {
            Console.WriteLine($"\n=== {_ownerName}'s Inventory ({_items.Count}/{_capacity} slots) ===");
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

        public virtual double GetTotalWeight()
        {
            double total = 0;
            foreach (var item in _items)
            {
                total += item.Weight;
            }
            return total;
        }

        public virtual Item? FindItem(string itemName)
        {
            return _items.Find(i => i.Name == itemName);
        }

        public virtual bool IsFull()
        {
            return _items.Count >= _capacity;
        }
    }
}