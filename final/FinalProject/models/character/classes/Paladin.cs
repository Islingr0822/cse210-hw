using System;

namespace DnDCharacterManager.Character.Classes
{
    /// <summary>
    /// Paladin character class stub.
    /// </summary>
    public class Paladin : Character
    {
        public int LayOnHandsPoints { get; set; }
        public int SpellSlotLevel { get; set; }

        public Paladin() : base()
        {
            LayOnHandsPoints = 5;
            SpellSlotLevel = 1;
        }

        public Paladin(string name, int level, object race, object background) : base()
        {
            LayOnHandsPoints = 5;
            SpellSlotLevel = 1;
        }

        public override void ClassSpecificAbility()
        {
            DivineSmite();
        }

        public virtual void DivineSmite()
        {
            Console.WriteLine("Paladin uses Divine Smite.");
        }

        public virtual void LayOnHands(int amount)
        {
            Console.WriteLine($"Paladin uses Lay on Hands for {amount} hit points.");
        }
    }
}