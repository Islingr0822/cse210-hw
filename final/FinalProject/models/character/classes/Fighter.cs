using System;

namespace DnDCharacterManager.Character.Classes
{
    /// <summary>
    /// Fighter character class stub.
    /// </summary>
    public class Fighter : Character
    {
        public int ActionSurgeUses { get; set; }
        public int SecondWindHeal { get; set; }

        public Fighter() : base()
        {
            ActionSurgeUses = 1;
            SecondWindHeal = 1;
        }

        public Fighter(string name, int level, object race, object background) : base()
        {
            ActionSurgeUses = 1;
            SecondWindHeal = 1;
        }

        public override void ClassSpecificAbility()
        {
            ActionSurge();
        }

        public virtual void ActionSurge()
        {
            Console.WriteLine("Fighter uses Action Surge.");
        }

        public virtual void SecondWind()
        {
            Console.WriteLine("Fighter uses Second Wind.");
        }
    }
}