using System;

namespace DnDCharacterManager.Character.Classes
{
    /// <summary>
    /// Monk character class stub.
    /// </summary>
    public class Monk : Character
    {
        public int KiPoints { get; set; }
        public int MonkLevel { get; set; }

        public Monk() : base()
        {
            KiPoints = 2;
            MonkLevel = 2;
        }

        public Monk(string name, int level, object race, object background) : base()
        {
            KiPoints = 2;
            MonkLevel = 2;
        }

        public override void ClassSpecificAbility()
        {
            FlurryOfBlows();
        }

        public virtual void FlurryOfBlows()
        {
            Console.WriteLine("Monk uses Flurry of Blows.");
        }
    }
}