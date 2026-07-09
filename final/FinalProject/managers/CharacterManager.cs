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
using DivineDomain = DnDCharacterManager.Character.Classes.DivineDomain;
using DruidCircle = DnDCharacterManager.Character.Classes.DruidCircle;

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

        // ==================== Cleric-Specific Factory Methods ====================

        /// <summary>
        /// Create a cleric character with a specific divine domain.
        /// </summary>
        public virtual ClericType CreateCleric(string name, int level, RaceType race, BackgroundType background, DivineDomain domain)
        {
            ClericType cleric = new ClericType(name, level, race, background);
            cleric.Domain = domain;
            _characters.Add(cleric);
            Console.WriteLine($"Created Cleric character: {name} with {domain} domain");
            return cleric;
        }

        /// <summary>
        /// Create a cleric character with a specific divine domain by string name.
        /// </summary>
        public virtual ClericType CreateCleric(string name, int level, string raceName, string backgroundName, string domainName)
        {
            RaceType race = CreateRace(raceName);
            BackgroundType bg = CreateBackground(backgroundName);
            
            DivineDomain domain;
            if (Enum.TryParse(domainName, true, out domain))
            {
                // Parse succeeded
            }
            else
            {
                Console.WriteLine($"Invalid domain '{domainName}'. Defaulting to Knowledge.");
                domain = DivineDomain.Knowledge;
            }

            return CreateCleric(name, level, race, bg, domain);
        }

        /// <summary>
        /// Get available divine domains for the player to choose from.
        /// </summary>
        public static List<string> GetAvailableDomains()
        {
            return new List<string>
            {
                "Knowledge", "Life", "Light", "War",
                "Trickery", "Death", "Nature", "Tempest", "Peace"
            };
        }

        // ==================== Druid-Specific Factory Methods ====================

        /// <summary>
        /// Create a druid character with a specific divine domain.
        /// </summary>
        public virtual DruidType CreateDruid(string name, int level, RaceType race, BackgroundType background, DruidCircle circle)
        {
            DruidType druid = new DruidType(name, level, race, background);
            druid.Circle = circle;
            _characters.Add(druid);
            Console.WriteLine($"Created Druid character: {name} with Circle of {circle}");
            return druid;
        }

        /// <summary>
        /// Create a druid character with a specific divine domain by string name.
        /// </summary>
        public virtual DruidType CreateDruid(string name, int level, string raceName, string backgroundName, string circleName)
        {
            RaceType race = CreateRace(raceName);
            BackgroundType bg = CreateBackground(backgroundName);

            DruidCircle circle;
            if (Enum.TryParse(circleName, true, out circle))
            {
                // Parse succeeded
            }
            else
            {
                Console.WriteLine($"Invalid circle '{circleName}'. Defaulting to Land.");
                circle = DruidCircle.Land;
            }

            return CreateDruid(name, level, race, bg, circle);
        }

        /// <summary>
        /// Get available druid circles for the player to choose from.
        /// </summary>
        public static List<string> GetAvailableCircles()
        {
            return new List<string>
            {
                "Land", "Moon", "Spores", "Shepherd",
                "Dreams", "Stars", "Wildfire", "Grovewhip"
            };
        }

        /// <summary>
        /// Get a description of a druid circle.
        /// </summary>
        public static string GetCircleDescription(DruidCircle circle)
        {
            switch (circle)
            {
                case DruidCircle.Land:
                    return "Nature's Ward and Natural Recovery. Master of the natural world.";
                case DruidCircle.Moon:
                    return "Combat Wild Shape and Elemental Wild Shape. Fight as a beast.";
                case DruidCircle.Spores:
                    return "Spore Sphere and death thym benefits. Channel necrotic energy.";
                case DruidCircle.Shepherd:
                    return "Fey Life Spirit and Guardian of Life. Protect allies with fey spirits.";
                case DruidCircle.Dreams:
                    return "Dream Walking and Sentinel of Dreams. Travel the Dreaming Deep.";
                case DruidCircle.Stars:
                    return "Far Step and Awakened Starlight. Harness cosmic power.";
                case DruidCircle.Wildfire:
                    return "Tame the Flames and Brilliance of the Grove. Control wildfire spirits.";
                case DruidCircle.Grovewhip:
                    return "Guardian of the Grove and Swift Growth. Defend the grove fiercely.";
                default:
                    return "Unknown circle.";
            }
        }

        /// <summary>
        /// Get a description of a divine domain.
        /// </summary>
        public static string GetDomainDescription(DivineDomain domain)
        {
            switch (domain)
            {
                case DivineDomain.Knowledge:
                    return "Bonus proficiencies in History and Arcana. Channel Divinity reveals book contents.";
                case DivineDomain.Life:
                    return "Enhanced healing spells. Channel Divinity restores hit points.";
                case DivineDomain.Light:
                    return "Cantrips deal extra radiant damage. Channel Divinity creates bright light.";
                case DivineDomain.War:
                    return "Martial weapon and heavy armor proficiency. Channel Divinity grants extra attack.";
                case DivineDomain.Trickery:
                    return "Blessing of the Trickster. Channel Divinity: Invoke Duplicity.";
                case DivineDomain.Death:
                    return "Enhanced damage vs undead. Channel Divinity: Consume Dead.";
                case DivineDomain.Nature:
                    return "Access to nature spell list. Charming Tact feature.";
                case DivineDomain.Tempest:
                    return "Lightning Bolt cantrip. Wounding Strike. Channel Divinity: Thunderbolt Strike.";
                case DivineDomain.Peace:
                    return "Radiant Burst feature. Emboldening Bond with allies.";
                default:
                    return "Unknown domain.";
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