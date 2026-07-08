using System;

namespace DnDCharacterManager.Character.Classes
{
    /// <summary>
    /// Ranger character class stub.
    /// </summary>
    public class Ranger : Character
    {
        public bool HunterMarkActive { get; set; }
        public int FavoredEnemy { get; set; }

        public Ranger() : base()
        {
            HunterMarkActive = false;
            FavoredEnemy = 1;
        }

        public Ranger(string name, int level, object race, object background) : base()
        {
            HunterMarkActive = false;
            FavoredEnemy = 1;
        }

        public override void ClassSpecificAbility()
        {
            HuntersMark();
        }

        public virtual void HuntersMark()
        {
            Console.WriteLine("Ranger uses Hunter's Mark.");
        }
    }
}