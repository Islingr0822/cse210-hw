using System;

namespace DnDCharacterManager.Character.Classes
{
    /// <summary>
    /// Sorcerer character class stub.
    /// </summary>
    public class Sorcerer : Character
    {
        public int SorceryPoints { get; set; }
        public int MetamagicOptions { get; set; }

        public Sorcerer() : base()
        {
            SorceryPoints = 2;
            MetamagicOptions = 2;
        }

        public Sorcerer(string name, int level, object race, object background) : base()
        {
            SorceryPoints = 2;
            MetamagicOptions = 2;
        }

        public override void ClassSpecificAbility()
        {
            Metamagic();
        }

        public virtual void Metamagic()
        {
            Console.WriteLine("Sorcerer uses Metamagic.");
        }
    }
}