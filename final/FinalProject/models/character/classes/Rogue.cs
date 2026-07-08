using System;

namespace DnDCharacterManager.Character.Classes
{
    /// <summary>
    /// Rogue character class stub.
    /// </summary>
    public class Rogue : Character
    {
        public int SneakAttackDice { get; set; }
        public bool IsStealthy { get; set; }

        public Rogue() : base()
        {
            SneakAttackDice = 1;
            IsStealthy = false;
        }

        public Rogue(string name, int level, object race, object background) : base()
        {
            SneakAttackDice = 1;
            IsStealthy = false;
        }

        public override void ClassSpecificAbility()
        {
            SneakAttack();
        }

        public virtual void SneakAttack()
        {
            Console.WriteLine("Rogue uses Sneak Attack.");
        }
    }
}