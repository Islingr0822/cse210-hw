using System;

namespace DnDCharacterManager.Background
{
    /// <summary>
    /// Base class for all character backgrounds.
    /// </summary>
    public abstract class Background
    {
        protected string _name;
        protected string _description;

        protected Background()
        {
            _name = "Commoner";
            _description = "A simple background.";
        }

        public Background(string name, string description)
        {
            _name = name;
            _description = description;
        }

        // Properties
        public string Name { get => _name; set => _name = value; }
        public string Description { get => _description; set => _description = value; }

        // Methods
        public abstract void GrantProficiencies();

        public override string ToString()
        {
            return $"{_name}: {_description}";
        }
    }

    // Concrete background implementations

    /// <summary>
    /// Aired background.
    /// </summary>
    public class Acolyte : Background
    {
        public Acolyte() : base("Acolyte", "You have spent your life in a temple.")
        {
        }

        public override void GrantProficiencies()
        {
            Console.WriteLine("Acolytes gain proficiency in Religion and Insight.");
        }
    }

    /// <summary>
    /// Soldier background.
    /// </summary>
    public class Soldier : Background
    {
        public Soldier() : base("Soldier", "You have served as a soldier your whole life.")
        {
        }

        public override void GrantProficiencies()
        {
            Console.WriteLine("Soldiers gain proficiency in Athletics and Intimidation.");
        }
    }

    /// <summary>
    /// Criminal background.
    /// </summary>
    public class Criminal : Background
    {
        public Criminal() : base("Criminal", "You have a life of crime behind you.")
        {
        }

        public override void GrantProficiencies()
        {
            Console.WriteLine("Criminals gain proficiency in Deception and Stealth.");
        }
    }

    /// <summary>
    /// Sage background.
    /// </summary>
    public class Sage : Background
    {
        public Sage() : base("Sage", "You have spent years studying and learning.")
        {
        }

        public override void GrantProficiencies()
        {
            Console.WriteLine("Sages gain proficiency in Arcana and History.");
        }
    }

    /// <summary>
    /// Noble background.
    /// </summary>
    public class Noble : Background
    {
        public Noble() : base("Noble", "You were born into privilege and wealth.")
        {
        }

        public override void GrantProficiencies()
        {
            Console.WriteLine("Nobles gain proficiency in History and Persuasion.");
        }
    }

    /// <summary>
    /// Hermit background.
    /// </summary>
    public class Hermit : Background
    {
        public Hermit() : base("Hermit", "You lived in isolation for many years.")
        {
        }

        public override void GrantProficiencies()
        {
            Console.WriteLine("Hermits gain proficiency in Medicine and Nature.");
        }
    }
}