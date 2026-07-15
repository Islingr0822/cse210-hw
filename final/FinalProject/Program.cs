using System;
using System.Collections.Generic;
using DnDChar = DnDCharacterManager.Character.Character;
using Manager = DnDCharacterManager.Managers.CharacterManager;
using Catalog = DnDCharacterManager.Managers.Catalog;
using SpellBookType = DnDCharacterManager.Spell.SpellBook;

class Program
{
    private static readonly Manager _manager = new Manager();

    static void Main(string[] args)
    {
        Console.WriteLine("=====================================");
        Console.WriteLine("     D&D Character Manager");
        Console.WriteLine("=====================================");
        Console.WriteLine("Track your characters' info. You roll the dice - this keeps the numbers.");

        bool running = true;
        while (running)
        {
            int choice = ShowMenu();
            switch (choice)
            {
                case 1:
                    CreateCharacterFlow();
                    break;
                case 2:
                    _manager.DisplayCharacters();
                    break;
                case 3:
                    ViewCharacterFlow();
                    break;
                case 4:
                    InventoryFlow();
                    break;
                case 5:
                    SpellFlow();
                    break;
                case 6:
                    DeleteCharacterFlow();
                    break;
                case 7:
                    _manager.SaveCharacter();
                    break;
                case 8:
                    _manager.LoadCharacter();
                    break;
                case 9:
                    running = false;
                    Console.WriteLine("Farewell, adventurer!");
                    break;
                default:
                    Console.WriteLine("Please choose a number from 1 to 9.");
                    break;
            }
        }
    }

    static int ShowMenu()
    {
        Console.WriteLine();
        Console.WriteLine("What would you like to do?");
        Console.WriteLine("  1. Create a new character");
        Console.WriteLine("  2. List all characters");
        Console.WriteLine("  3. View a character sheet");
        Console.WriteLine("  4. Manage a character's inventory");
        Console.WriteLine("  5. Manage a character's spells");
        Console.WriteLine("  6. Delete a character");
        Console.WriteLine("  7. Save characters to file");
        Console.WriteLine("  8. Load characters from file");
        Console.WriteLine("  9. Quit");
        return ReadInt("Choice: ", 1, 9, 9);
    }

    static void CreateCharacterFlow()
    {
        Console.WriteLine();
        Console.WriteLine("--- Create a New Character ---");

        Console.Write("Name: ");
        string name = Console.ReadLine();
        if (string.IsNullOrWhiteSpace(name))
        {
            name = "Unnamed Hero";
        }
        name = name.Trim();

        string className = ChooseFromList("Choose a class:", Manager.GetAvailableClasses());
        string raceName = ChooseFromList("Choose a race:", Manager.GetAvailableRaces());
        string backgroundName = ChooseFromList("Choose a background:", Manager.GetAvailableBackgrounds());
        int level = ReadInt("Level (1-20): ", 1, 20, 1);

        DnDChar character = CreateByClass(name, level, className, raceName, backgroundName);
        if (character == null)
        {
            Console.WriteLine("Could not create the character. Returning to menu.");
            return;
        }

        Console.WriteLine();
        Console.WriteLine("Enter ability scores (press Enter to keep the shown default):");
        character.Strength = ReadStat("Strength", character.Strength);
        character.Dexterity = ReadStat("Dexterity", character.Dexterity);
        character.Constitution = ReadStat("Constitution", character.Constitution);
        character.Intelligence = ReadStat("Intelligence", character.Intelligence);
        character.Wisdom = ReadStat("Wisdom", character.Wisdom);
        character.Charisma = ReadStat("Charisma", character.Charisma);

        // Refresh HP/AC and other derived values now that scores are set.
        character.RecalculateDerivedStats();

        // Label the inventory with its owner so displays read nicely.
        character.Inventory.OwnerName = character.Name;

        Console.WriteLine();
        Console.WriteLine($"{character.Name} the level {character.Level} {className} is ready!");
    }

    /// <summary>
    /// Uses the class-specific factory for Cleric/Druid (so a subclass choice is
    /// captured) and the generic factory for everyone else.
    /// </summary>
    static DnDChar CreateByClass(string name, int level, string className, string raceName, string backgroundName)
    {
        switch (className.ToLower())
        {
            case "cleric":
                string domain = ChooseFromList("Choose a divine domain:", Manager.GetAvailableDomains());
                return _manager.CreateCleric(name, level, raceName, backgroundName, domain);
            case "druid":
                string circle = ChooseFromList("Choose a druid circle:", Manager.GetAvailableCircles());
                return _manager.CreateDruid(name, level, raceName, backgroundName, circle);
            default:
                return _manager.CreateCharacter(name, level, className, raceName, backgroundName);
        }
    }

    static void ViewCharacterFlow()
    {
        if (_manager.CharacterCount == 0)
        {
            Console.WriteLine("No characters yet. Create one first.");
            return;
        }

        DnDChar character = ChooseCharacter("Which character do you want to view?");
        if (character == null)
        {
            return;
        }

        character.DisplayCharacter();

        // The sheet also shows what the character is carrying and any spells known.
        character.Inventory.DisplayInventory();
        if (character.SpellBook != null && character.SpellBook.KnownSpells.Count > 0)
        {
            character.SpellBook.DisplaySpells();
        }
    }

    static void DeleteCharacterFlow()
    {
        if (_manager.CharacterCount == 0)
        {
            Console.WriteLine("No characters to delete.");
            return;
        }

        DnDChar character = ChooseCharacter("Which character do you want to delete?");
        if (character != null)
        {
            _manager.DeleteCharacter(character);
        }
    }

    // ==================== Inventory ====================

    static void InventoryFlow()
    {
        if (_manager.CharacterCount == 0)
        {
            Console.WriteLine("No characters yet. Create one first.");
            return;
        }

        DnDChar character = ChooseCharacter("Whose inventory do you want to manage?");
        if (character == null)
        {
            return;
        }

        bool managing = true;
        while (managing)
        {
            Console.WriteLine();
            Console.WriteLine($"--- {character.Name}'s Inventory ---");
            Console.WriteLine("  1. Add an item");
            Console.WriteLine("  2. Remove an item");
            Console.WriteLine("  3. Equip an item");
            Console.WriteLine("  4. Show inventory");
            Console.WriteLine("  5. Back to main menu");

            switch (ReadInt("Choice: ", 1, 5, 5))
            {
                case 1:
                    AddItemFlow(character);
                    break;
                case 2:
                    RemoveItemFlow(character);
                    break;
                case 3:
                    EquipItemFlow(character);
                    break;
                case 4:
                    character.Inventory.DisplayInventory();
                    break;
                case 5:
                    managing = false;
                    break;
            }
        }
    }

    static void AddItemFlow(DnDChar character)
    {
        var entry = ChooseFromCatalog("Choose an item to add:", Catalog.Items);
        character.Inventory.AddItem(entry.Create());
    }

    static void RemoveItemFlow(DnDChar character)
    {
        var names = new List<string>(character.Inventory.ItemQuantities.Keys);
        if (names.Count == 0)
        {
            Console.WriteLine("There are no items to remove.");
            return;
        }

        string name = ChooseFromList("Choose an item to remove:", names);
        character.Inventory.RemoveItem(name);
    }

    static void EquipItemFlow(DnDChar character)
    {
        var names = new List<string>();
        foreach (var item in character.Inventory.Items)
        {
            if (!names.Contains(item.Name))
            {
                names.Add(item.Name);
            }
        }

        if (names.Count == 0)
        {
            Console.WriteLine("There are no unequipped items to equip.");
            return;
        }

        string chosen = ChooseFromList("Choose an item to equip:", names);
        var toEquip = character.Inventory.FindItem(chosen);
        if (toEquip != null)
        {
            character.Inventory.EquipItem(toEquip);
        }
    }

    // ==================== Spells ====================

    static void SpellFlow()
    {
        if (_manager.CharacterCount == 0)
        {
            Console.WriteLine("No characters yet. Create one first.");
            return;
        }

        DnDChar character = ChooseCharacter("Whose spells do you want to manage?");
        if (character == null)
        {
            return;
        }

        // Give the character a spellbook the first time they use one.
        if (character.SpellBook == null)
        {
            character.SpellBook = new SpellBookType(character.Name);
        }

        bool managing = true;
        while (managing)
        {
            Console.WriteLine();
            Console.WriteLine($"--- {character.Name}'s Spellbook ---");
            Console.WriteLine("  1. Learn a spell");
            Console.WriteLine("  2. Forget a spell");
            Console.WriteLine("  3. Show known spells");
            Console.WriteLine("  4. Back to main menu");

            switch (ReadInt("Choice: ", 1, 4, 4))
            {
                case 1:
                    LearnSpellFlow(character);
                    break;
                case 2:
                    ForgetSpellFlow(character);
                    break;
                case 3:
                    character.SpellBook.DisplaySpells();
                    break;
                case 4:
                    managing = false;
                    break;
            }
        }
    }

    static void LearnSpellFlow(DnDChar character)
    {
        var entry = ChooseFromCatalog("Choose a spell to learn:", Catalog.Spells);
        character.SpellBook.LearnSpell(entry.Create());
    }

    static void ForgetSpellFlow(DnDChar character)
    {
        var spells = character.SpellBook.KnownSpells;
        if (spells.Count == 0)
        {
            Console.WriteLine("No spells known yet.");
            return;
        }

        var names = new List<string>();
        foreach (var spell in spells)
        {
            names.Add(spell.Name);
        }

        string chosen = ChooseFromList("Choose a spell to forget:", names);
        character.SpellBook.ForgetSpell(chosen);
    }

    // ==================== Input Helpers ====================

    static DnDChar ChooseCharacter(string prompt)
    {
        List<DnDChar> characters = _manager.Characters;
        Console.WriteLine();
        Console.WriteLine(prompt);
        for (int i = 0; i < characters.Count; i++)
        {
            Console.WriteLine($"  {i + 1}. {characters[i].Name} (Level {characters[i].Level})");
        }

        int choice = ReadInt("Choice: ", 1, characters.Count, 1);
        return characters[choice - 1];
    }

    static string ChooseFromList(string title, List<string> options)
    {
        Console.WriteLine();
        Console.WriteLine(title);
        for (int i = 0; i < options.Count; i++)
        {
            Console.WriteLine($"  {i + 1}. {options[i]}");
        }

        int choice = ReadInt("Choice: ", 1, options.Count, 1);
        return options[choice - 1];
    }

    /// <summary>
    /// Presents catalog entries (items or spells) by their friendly name and
    /// returns the chosen entry so the caller can build a fresh instance.
    /// </summary>
    static Catalog.Entry<T> ChooseFromCatalog<T>(string title, IReadOnlyList<Catalog.Entry<T>> entries)
    {
        Console.WriteLine();
        Console.WriteLine(title);
        for (int i = 0; i < entries.Count; i++)
        {
            Console.WriteLine($"  {i + 1}. {entries[i].DisplayName}");
        }

        int choice = ReadInt("Choice: ", 1, entries.Count, 1);
        return entries[choice - 1];
    }

    static int ReadStat(string label, int currentValue)
    {
        Console.Write($"  {label} [{currentValue}]: ");
        string input = Console.ReadLine();
        if (string.IsNullOrWhiteSpace(input))
        {
            return currentValue;
        }

        if (int.TryParse(input.Trim(), out int value))
        {
            return value;
        }

        Console.WriteLine("  Not a number - keeping the current value.");
        return currentValue;
    }

    static int ReadInt(string prompt, int min, int max, int fallback)
    {
        Console.Write(prompt);
        string input = Console.ReadLine();

        if (int.TryParse(input?.Trim(), out int value) && value >= min && value <= max)
        {
            return value;
        }

        Console.WriteLine($"Invalid entry - using {fallback}.");
        return fallback;
    }
}
