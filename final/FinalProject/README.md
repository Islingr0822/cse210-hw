# D&D Character Manager

A console application for creating, tracking, and persisting Dungeons & Dragons 5e
characters. The program is a **character sheet keeper**: it stores and displays all
the numbers a player needs (ability scores, HP, AC, class features, inventory, and
known spells), while the player rolls the physical dice and does the in-the-moment
combat math themselves. This was a deliberate design choice — the app is a reliable
reference sheet, not an automated dice roller.

## How to Run

Requires the .NET SDK (targets `net10.0`).

```bash
cd final/FinalProject
dotnet run
```

You'll get an interactive menu. Characters are saved to / loaded from
`characters.txt` in the project folder.

## Menu Options

1. **Create a new character** – choose a class, race, background, level, and enter
   ability scores. Clerics and Druids are additionally prompted for their subclass
   (Divine Domain / Druid Circle).
2. **List all characters** – a quick roster of everyone you've created.
3. **View a character sheet** – the full sheet, including carried inventory and any
   known spells.
4. **Manage a character's inventory** – add items from a catalog, remove them, equip
   weapons/armor/shields, and view the inventory (with weight and currency).
5. **Manage a character's spells** – learn spells from a catalog, forget them, and
   list what's known.
6. **Delete a character**.
7. **Save characters to file**.
8. **Load characters from file**.
9. **Quit**.

## Project Structure

```
FinalProject/
  Program.cs                     # Menu / user-interaction layer
  managers/
    CharacterManager.cs          # Owns the character list + save/load + factories
    Catalog.cs                   # Selectable item & spell registry (also used by load)
  models/
    character/
      Character.cs               # Abstract base class for every character
      classes/                   # 13 concrete classes (Fighter, Wizard, Cleric, ...)
    race/Race.cs                 # Race base class + concrete races
    background/Background.cs     # Background base class + concrete backgrounds
    item/Item.cs                 # Item base class + Weapon/Armor/Shield/Potion/... trees
    inventory/Inventory.cs       # Item storage, equipment slots, and currency
    spell/Spell.cs, SpellBook.cs # Spell base class, spell catalog, and spellbook
    ability/AbilityScore.cs      # Ability scores + modifiers
    feat/, skill/, feature/      # Supporting character-detail classes
```

## Design: The Four OOP Principles

**Abstraction** – Each class has a focused responsibility: `Inventory` manages items
and money, `SpellBook` manages spells, `CharacterManager` handles the collection and
persistence, and `Catalog` is the single source of truth for selectable items/spells.

**Encapsulation** – Fields are `private`/`protected` and exposed through properties
and methods; callers go through the public API (`AddItem`, `LearnSpell`,
`DisplayCharacter`) rather than touching internal state directly.

**Inheritance** – Several base/derived hierarchies:
- `Character` → `Fighter`, `Wizard`, `Cleric`, `Druid`, `Rogue`, ... (13 classes)
- `Item` → `Weapon`, `Armor`, `ShieldItem`, `Potion`, `Tool`, `AdventuringGear`,
  `MagicItem` → and concrete items beneath those (e.g. `LongSword`, `ChainMail`)
- `Spell` → dozens of concrete spells; `Race` and `Background` trees.

**Polymorphism** – Overridden methods drive behavior at runtime: `DisplayCharacter()`,
`CalculateBaseStats()`, and `Attack()` are overridden per class, and each `Item`
subclass overrides `DisplayInfo()` so the same call renders weapon/armor/potion
details differently.

## Persistence Format

Characters are stored one-per-line in a pipe-delimited `key=value` format, e.g.:

```
Class=Fighter|Race=Human|Background=Acolyte|Items=LeatherArmor;Torch;LongSword|Spells=MagicMissile;Fireball|Name=Grommash|Level=3|...
```

On load, the manager rebuilds the correct class via a factory, then reconstructs the
inventory and spellbook by looking each stored key up in `Catalog`. Items and spells
are stored by their type name so the same factory that builds them for the menu also
rebuilds them from disk.

## Notes / Creativity

- **13 fully-distinct character classes**, each with its own subclass features and
  custom sheet output — well beyond a single-class example.
- **A shared catalog** powers both the interactive menus and save/load, so there's
  one place to add a new item or spell and it works everywhere.
- **Rich item and spell hierarchies** (weapons, armor, shields, potions, tools, gear,
  and magic items; dozens of spells) that exercise polymorphic display.
- **Design decision:** the app intentionally leaves dice rolling and damage/healing
  math to the player, acting as a dependable digital character sheet instead.
