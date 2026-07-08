using System;

namespace DnDCharacterManager.Character.Classes
{
    /// <summary>
    /// Cleric character class stub.
    /// </summary>
    public class Cleric : Character
    {
        public int ChannelDivinityUses { get; set; }
        public string DivineDomain { get; set; }

        public Cleric() : base()
        {
            ChannelDivinityUses = 0;
            DivineDomain = "Knowledge";
        }

        public Cleric(string name, int level, object race, object background) : base()
        {
            ChannelDivinityUses = 0;
            DivineDomain = "Knowledge";
        }

        public override void ClassSpecificAbility()
        {
            ChannelDivinity();
        }

        public virtual void ChannelDivinity()
        {
            Console.WriteLine("Cleric uses Channel Divinity.");
        }
    }
}