using System;
using System.Collections.Generic;

namespace DnDCharacterManager.Spell
{
    /// <summary>
    /// Manages a collection of spells known by a character.
    /// </summary>
    public class SpellBook
    {
        protected List<Spell> _knownSpells;
        protected string _ownerName;

        public SpellBook()
        {
            _knownSpells = new List<Spell>();
            _ownerName = "Unnamed";
        }

        public SpellBook(string ownerName)
        {
            _knownSpells = new List<Spell>();
            _ownerName = ownerName;
        }

        // Properties
        public List<Spell> KnownSpells { get => _knownSpells; set => _knownSpells = value; }
        public string OwnerName { get => _ownerName; set => _ownerName = value; }

        // Methods
        public virtual void LearnSpell(Spell spell)
        {
            if (!_knownSpells.Contains(spell))
            {
                _knownSpells.Add(spell);
                Console.WriteLine($"{_ownerName} has learned the spell: {spell.Name}");
            }
            else
            {
                Console.WriteLine($"{_ownerName} already knows the spell: {spell.Name}");
            }
        }

        public virtual void ForgetSpell(string spellName)
        {
            Spell? spellToForget = _knownSpells.Find(s => s.Name == spellName);
            if (spellToForget != null)
            {
                _knownSpells.Remove(spellToForget);
                Console.WriteLine($"{_ownerName} has forgotten the spell: {spellName}");
            }
            else
            {
                Console.WriteLine($"{_ownerName} does not know the spell: {spellName}");
            }
        }

        public virtual void DisplaySpells()
        {
            Console.WriteLine($"=== {_ownerName}'s Known Spells ({_knownSpells.Count}) ===");
            if (_knownSpells.Count == 0)
            {
                Console.WriteLine("No spells known.");
            }
            else
            {
                foreach (var spell in _knownSpells)
                {
                    Console.WriteLine($"  - {spell.Name} (Level {spell.Level}, {spell.School})");
                }
            }
        }

        public virtual Spell? FindSpell(string spellName)
        {
            return _knownSpells.Find(s => s.Name == spellName);
        }
    }
}