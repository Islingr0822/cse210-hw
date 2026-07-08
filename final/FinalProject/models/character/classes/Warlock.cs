using System;

namespace DnDCharacterManager.Character.Classes
{
    /// <summary>
    /// Warlock character class stub.
    /// </summary>
    public class Warlock : Character
    {
        public int PactMagicSlots { get; set; }
        public string Patron { get; set; }

        public Warlock() : base()
        {
            PactMagicSlots = 1;
            Patron = "Great Old One";
        }

        public Warlock(string name, int level, object race, object background) : base()
        {
            PactMagicSlots = 1;
            Patron = "Great Old One";
        }

        public override void ClassSpecificAbility()
        {
            PactMagic();
        }

        public virtual void PactMagic()
        {
            Console.WriteLine("Warlock uses Pact Magic.");
        }
    }
}