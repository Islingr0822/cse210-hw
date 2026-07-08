using System;

namespace DnDCharacterManager.Character.Classes
{
    /// <summary>
    /// Druid character class stub.
    /// </summary>
    public class Druid : Character
    {
        public int WildShapeUses { get; set; }
        public string DruidCircle { get; set; }

        public Druid() : base()
        {
            WildShapeUses = 0;
            DruidCircle = "Land";
        }

        public Druid(string name, int level, object race, object background) : base()
        {
            WildShapeUses = 0;
            DruidCircle = "Land";
        }

        public override void ClassSpecificAbility()
        {
            WildShape();
        }

        public virtual void WildShape()
        {
            Console.WriteLine("Druid uses Wild Shape.");
        }
    }
}