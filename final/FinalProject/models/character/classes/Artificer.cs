using System;

namespace DnDCharacterManager.Character.Classes
{
    /// <summary>
    /// Artificer character class stub.
    /// </summary>
    public class Artificer : Character
    {
        public int InfusionSlots { get; set; }
        public string ArtillerieDomain { get; set; }

        public Artificer() : base()
        {
            InfusionSlots = 0;
            ArtillerieDomain = "Armorer";
        }

        public Artificer(string name, int level, object race, object background) : base()
        {
            InfusionSlots = 0;
            ArtillerieDomain = "Armorer";
        }

        public override void ClassSpecificAbility()
        {
            InfuseItem();
        }

        public virtual void InfuseItem()
        {
            Console.WriteLine("Artificer infuses an item with magic.");
        }

        public virtual void CreateMagicItem()
        {
            Console.WriteLine("Artificer crafts a new magic item.");
        }
    }
}