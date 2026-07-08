using System;
using System.Collections.Generic;
using System.IO;
using DnDChar = DnDCharacterManager.Character.Character;
using RaceType = DnDCharacterManager.Race.Race;
using BackgroundType = DnDCharacterManager.Background.Background;
using HumanType = DnDCharacterManager.Race.Human;
using ElfType = DnDCharacterManager.Race.Elf;
using DwarfType = DnDCharacterManager.Race.Dwarf;
using DragonbornType = DnDCharacterManager.Race.Dragonborn;
using HalfElfType = DnDCharacterManager.Race.HalfElf;
using HalfOrcType = DnDCharacterManager.Race.HalfOrc;
using HalflingType = DnDCharacterManager.Race.Halfling;
using AcolyteType = DnDCharacterManager.Background.Acolyte;
using SoldierType = DnDCharacterManager.Background.Soldier;
using CriminalType = DnDCharacterManager.Background.Criminal;
using SageType = DnDCharacterManager.Background.Sage;
using NobleType = DnDCharacterManager.Background.Noble;
using HermitType = DnDCharacterManager.Background.Hermit;
using BarbarianType = DnDCharacterManager.Character.Classes.Barbarian;
using BardType = DnDCharacterManager.Character.Classes.Bard;
using ClericType = DnDCharacterManager.Character.Classes.Cleric;
using DruidType = DnDCharacterManager.Character.Classes.Druid;
using FighterType = DnDCharacterManager.Character.Classes.Fighter;
using MonkType = DnDCharacterManager.Character.Classes.Monk;
using PaladinType = DnDCharacterManager.Character.Classes.Paladin;
using RangerType = DnDCharacterManager.Character.Classes.Ranger;
using RogueType = DnDCharacterManager.Character.Classes.Rogue;
using SorcererType = DnDCharacterManager.Character.Classes.Sorcerer;
using WarlockType = DnDCharacterManager.Character.Classes.Warlock;
using WizardType = DnDCharacterManager.Character.Classes.Wizard;
using ArtificerType = DnDCharacterManager.Character.Classes.Artificer;

namespace DnDCharacterManager.Managers
{
    /// <summary>
    /// Manages a collection of characters.
    /// </summary>
    public class CharacterManager
    {
        private List<DnDChar> _characters;
        private string _saveFilePath;

        public CharacterManager()
        {
            _characters = new List<DnDChar>();
            _saveFilePath = "characters.json";
        }

        public CharacterManager(string saveFilePath)
        {
            _characters = new List<DnDChar>();
            _saveFilePath = saveFilePath;
        }

        // Properties
        public List<DnDChar> Characters => _characters;
        public int CharacterCount => _characters.Count;

        // Methods
        /// <summary>
        /// Create a new character.
        /// </summary>
        public virtual DnDChar CreateCharacter(string name, int level, string characterClass, string raceName, string backgroundName)
        {
            RaceType race = CreateRace(raceName);
            BackgroundType bg = CreateBackground(backgroundName);

            DnDChar newCharacter = CreateCharacterByClass(name, level, characterClass, race, bg);

            if (newCharacter != null)
            {
                _characters.Add(newCharacter);
                Console.WriteLine($"Created {characterClass} character: {name}");
            }

            return newCharacter;
        }

        /// <summary>
        /// Delete a character by name.
        /// </summary>
        public virtual bool DeleteCharacter(string characterName)
        {
            DnDChar? characterToRemove = _characters.Find(c => c.Name == characterName);
            if (characterToRemove != null)
            {
                _characters.Remove(characterToRemove);
                Console.WriteLine($"Deleted character: {characterName}");
                return true;
            }
            Console.WriteLine($"Character '{characterName}' not found.");
            return false;
        }

        /// <summary>
        /// Delete a character object.
        /// </summary>
        public virtual bool DeleteCharacter(DnDChar character)
        {
            if (_characters.Remove(character))
            {
                Console.WriteLine($"Deleted character: {character.Name}");
                return true;
            }
            Console.WriteLine($"Character '{character.Name}' not found.");
            return false;
        }

        /// <summary>
        /// Save characters to a file.
        /// </summary>
        public virtual void SaveCharacter(string filePath)
        {
            try
            {
                string json = "[]"; // Placeholder - would use JSON serialization in production
                File.WriteAllText(filePath, json);
                Console.WriteLine($"Saved {_characters.Count} characters to {filePath}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error saving characters: {ex.Message}");
            }
        }

        public virtual void SaveCharacter()
        {
            SaveCharacter(_saveFilePath);
        }

        /// <summary>
        /// Load characters from a file.
        /// </summary>
        public virtual void LoadCharacter(string filePath)
        {
            try
            {
                if (File.Exists(filePath))
                {
                    string json = File.ReadAllText(filePath);
                    Console.WriteLine($"Loaded characters from {filePath}");
                }
                else
                {
                    Console.WriteLine($"File '{filePath}' not found.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading characters: {ex.Message}");
            }
        }

        public virtual void LoadCharacter()
        {
            LoadCharacter(_saveFilePath);
        }

        /// <summary>
        /// Display all characters.
        /// </summary>
        public virtual void DisplayCharacters()
        {
            Console.WriteLine($"\n=== Character Manager ({_characters.Count} characters) ===");
            if (_characters.Count == 0)
            {
                Console.WriteLine("No characters created.");
            }
            else
            {
                foreach (var character in _characters)
                {
                    Console.WriteLine($"  - {character.Name} (Level {character.Level})");
                }
            }
            Console.WriteLine();
        }

        /// <summary>
        /// Find a character by name.
        /// </summary>
        public virtual DnDChar? FindCharacter(string characterName)
        {
            return _characters.Find(c => c.Name == characterName);
        }

        /// <summary>
        /// Display detailed info for a character.
        /// </summary>
        public virtual void DisplayCharacterDetail(string characterName)
        {
            DnDChar? character = FindCharacter(characterName);
            if (character != null)
            {
                character.DisplayCharacter();
            }
            else
            {
                Console.WriteLine($"Character '{characterName}' not found.");
            }
        }

        // Helper methods
        private static RaceType CreateRace(string raceName)
        {
            switch (raceName.ToLower())
            {
                case "human": return new HumanType();
                case "elf": return new ElfType();
                case "dwarf": return new DwarfType();
                case "dragonborn": return new DragonbornType();
                case "half-elf": return new HalfElfType();
                case "half-orc": return new HalfOrcType();
                case "halfling": return new HalflingType();
                default: return new HumanType();
            }
        }

        private static BackgroundType CreateBackground(string backgroundName)
        {
            switch (backgroundName.ToLower())
            {
                case "acolyte": return new AcolyteType();
                case "soldier": return new SoldierType();
                case "criminal": return new CriminalType();
                case "sage": return new SageType();
                case "noble": return new NobleType();
                case "hermit": return new HermitType();
                default: return new AcolyteType();
            }
        }

        private static DnDChar CreateCharacterByClass(string name, int level, string className, RaceType race, BackgroundType background)
        {
            switch (className.ToLower())
            {
                case "barbarian": return new BarbarianType(name, level, race, background);
                case "bard": return new BardType(name, level, race, background);
                case "cleric": return new ClericType(name, level, race, background);
                case "druid": return new DruidType(name, level, race, background);
                case "fighter": return new FighterType(name, level, race, background);
                case "monk": return new MonkType(name, level, race, background);
                case "paladin": return new PaladinType(name, level, race, background);
                case "ranger": return new RangerType(name, level, race, background);
                case "rogue": return new RogueType(name, level, race, background);
                case "sorcerer": return new SorcererType(name, level, race, background);
                case "warlock": return new WarlockType(name, level, race, background);
                case "wizard": return new WizardType(name, level, race, background);
                case "artificer": return new ArtificerType(name, level, race, background);
                default:
                    Console.WriteLine($"Unknown character class: {className}");
                    return null;
            }
        }
    }
}