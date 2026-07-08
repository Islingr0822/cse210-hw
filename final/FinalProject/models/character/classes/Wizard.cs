using System;

namespace DnDCharacterManager.Character.Classes
{
    /// <summary>
    /// Wizard character class stub.
    /// </summary>
    public class Wizard : Character
    {
        public int SpellPreparationCount { get; set; }
        public string ArcaneTradition { get; set; }

        public Wizard() : base()
        {
            SpellPreparationCount = 0;
            ArcaneTradition = "School of Magic";
        }

        public Wizard(string name, int level, object race, object background) : base()
        {
            SpellPreparationCount = 0;
            ArcaneTradition = "School of Magic";
        }

        public override void ClassSpecificAbility()
        {
            PrepareSpell();
        }

        public virtual void PrepareSpell()
        {
            Console.WriteLine("Wizard prepares a spell from spellbook.");
        }
    }
}