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
using SpellBookType = DnDCharacterManager.Spell.SpellBook;

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
            _saveFilePath = "characters.txt";
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
        /// Save all characters to a file. Each character is stored on a single
        /// pipe-delimited line of key=value pairs (see SerializeCharacterToLine).
        /// </summary>
        public virtual void SaveCharacter(string filePath)
        {
            try
            {
                var lines = new List<string>();
                foreach (var character in _characters)
                {
                    lines.Add(SerializeCharacterToLine(character));
                }

                File.WriteAllLines(filePath, lines);
                Console.WriteLine($"Saved {_characters.Count} character(s) to {filePath}");
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
        /// Load characters from a file, replacing the current roster.
        /// </summary>
        public virtual void LoadCharacter(string filePath)
        {
            try
            {
                if (!File.Exists(filePath))
                {
                    Console.WriteLine($"File '{filePath}' not found. Nothing loaded.");
                    return;
                }

                string[] lines = File.ReadAllLines(filePath);
                _characters.Clear();

                int loaded = 0;
                foreach (string line in lines)
                {
                    if (string.IsNullOrWhiteSpace(line))
                    {
                        continue;
                    }

                    DnDChar? character = DeserializeCharacterFromLine(line);
                    if (character != null)
                    {
                        _characters.Add(character);
                        loaded++;
                    }
                }

                Console.WriteLine($"Loaded {loaded} character(s) from {filePath}");
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
        /// Converts a character to a single storage line. The class, race, and
        /// background names are needed to rebuild the correct object on load;
        /// the remaining fields come from the character's own Serialize().
        /// </summary>
        private static string SerializeCharacterToLine(DnDChar character)
        {
            string className = character.GetType().Name;
            string raceName = character.Race?.Name ?? "Human";
            string backgroundName = character.Background?.Name ?? "Acolyte";

            var parts = new List<string>
            {
                "Class=" + className,
                "Race=" + raceName,
                "Background=" + backgroundName
            };

            // Persist the subclass choice for classes that have one, so it survives a reload.
            if (character is ClericType cleric)
            {
                parts.Add("Domain=" + cleric.Domain);
            }
            else if (character is DruidType druid)
            {
                parts.Add("Circle=" + druid.Circle);
            }

            // Persist inventory contents (by item type name) so the reload can rebuild them
            // through the catalog. Equipped items are included so nothing is lost.
            var itemKeys = new List<string>();
            foreach (var item in character.Inventory.Items)
            {
                itemKeys.Add(item.GetType().Name);
            }
            foreach (var item in character.Inventory.GetAllEquippedItems())
            {
                itemKeys.Add(item.GetType().Name);
            }
            if (itemKeys.Count > 0)
            {
                parts.Add("Items=" + string.Join(";", itemKeys));
            }
            if (character.Inventory.Gold > 0)
            {
                parts.Add("Gold=" + character.Inventory.Gold);
            }

            // Persist known spells (by spell type name).
            if (character.SpellBook != null && character.SpellBook.KnownSpells.Count > 0)
            {
                var spellKeys = new List<string>();
                foreach (var spell in character.SpellBook.KnownSpells)
                {
                    spellKeys.Add(spell.GetType().Name);
                }
                parts.Add("Spells=" + string.Join(";", spellKeys));
            }

            foreach (var kvp in character.Serialize())
            {
                // Guard against delimiters sneaking into free-text values.
                string value = (kvp.Value?.ToString() ?? "").Replace("|", "/").Replace("\n", " ").Replace("\r", " ");
                parts.Add(kvp.Key + "=" + value);
            }

            return string.Join("|", parts);
        }

        /// <summary>
        /// Rebuilds a character from a storage line produced by SerializeCharacterToLine.
        /// </summary>
        private static DnDChar? DeserializeCharacterFromLine(string line)
        {
            var data = new Dictionary<string, object>();
            foreach (string field in line.Split('|'))
            {
                int idx = field.IndexOf('=');
                if (idx < 0)
                {
                    continue;
                }

                string key = field.Substring(0, idx);
                string value = field.Substring(idx + 1);
                data[key] = value;
            }

            string className = data.TryGetValue("Class", out var c) ? c.ToString() : "";
            string raceName = data.TryGetValue("Race", out var r) ? r.ToString() : "Human";
            string backgroundName = data.TryGetValue("Background", out var b) ? b.ToString() : "Acolyte";
            string name = data.TryGetValue("Name", out var n) ? n.ToString() : "Unnamed";
            int level = data.TryGetValue("Level", out var l) ? Convert.ToInt32(l) : 1;

            RaceType race = CreateRace(raceName);
            BackgroundType bg = CreateBackground(backgroundName);

            DnDChar? character = CreateCharacterByClass(name, level, className, race, bg);
            if (character == null)
            {
                Console.WriteLine($"Skipped unknown character class '{className}'.");
                return null;
            }

            character.Deserialize(data);

            // Restore the subclass choice for classes that store one.
            if (character is ClericType cleric && data.TryGetValue("Domain", out var domainValue)
                && Enum.TryParse<DivineDomain>(domainValue.ToString(), true, out var domain))
            {
                cleric.Domain = domain;
            }
            else if (character is DruidType druid && data.TryGetValue("Circle", out var circleValue)
                && Enum.TryParse<DruidCircle>(circleValue.ToString(), true, out var circle))
            {
                druid.Circle = circle;
            }

            // Keep the inventory label in sync with the character it belongs to.
            character.Inventory.OwnerName = character.Name;

            // Rebuild inventory items from their catalog keys.
            if (data.TryGetValue("Items", out var itemsValue))
            {
                foreach (string key in (itemsValue.ToString() ?? "").Split(';', StringSplitOptions.RemoveEmptyEntries))
                {
                    var item = Catalog.CreateItem(key);
                    if (item != null)
                    {
                        character.Inventory.AddItem(item);
                    }
                }
            }

            // Restore gold.
            if (data.TryGetValue("Gold", out var goldValue) && int.TryParse(goldValue.ToString(), out int gold))
            {
                character.Inventory.Gold = gold;
            }

            // Rebuild known spells from their catalog keys.
            if (data.TryGetValue("Spells", out var spellsValue))
            {
                if (character.SpellBook == null)
                {
                    character.SpellBook = new SpellBookType(character.Name);
                }

                foreach (string key in (spellsValue.ToString() ?? "").Split(';', StringSplitOptions.RemoveEmptyEntries))
                {
                    var spell = Catalog.CreateSpell(key);
                    if (spell != null)
                    {
                        character.SpellBook.LearnSpell(spell);
                    }
                }
            }

            return character;
        }

        /// <summary>
        /// Returns the list of character classes the manager can create.
        /// </summary>
        public static List<string> GetAvailableClasses()
        {
            return new List<string>
            {
                "Barbarian", "Bard", "Cleric", "Druid", "Fighter", "Monk", "Paladin",
                "Ranger", "Rogue", "Sorcerer", "Warlock", "Wizard", "Artificer"
            };
        }

        /// <summary>
        /// Returns the list of races the manager can create.
        /// </summary>
        public static List<string> GetAvailableRaces()
        {
            return new List<string>
            {
                "Human", "Elf", "Dwarf", "Dragonborn", "Half-Elf", "Half-Orc", "Halfling"
            };
        }

        /// <summary>
        /// Returns the list of backgrounds the manager can create.
        /// </summary>
        public static List<string> GetAvailableBackgrounds()
        {
            return new List<string>
            {
                "Acolyte", "Soldier", "Criminal", "Sage", "Noble", "Hermit"
            };
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