using System;

namespace DnDCharacterManager.Feat
{
    /// <summary>
    /// Represents a character feat in D&D 5e.
    /// </summary>
    public class Feat
    {
        protected string _name;
        protected string _description;
        protected string _prerequisiteAbility;
        protected string _prerequisiteFeatures;
        protected string _sourceBook;

        public Feat()
        {
            _name = "Unnamed Feat";
            _description = "No description.";
            _prerequisiteAbility = "None";
            _prerequisiteFeatures = "None";
            _sourceBook = "Player's Handbook";
        }

        public Feat(string name, string description, string prerequisiteAbility = "None", string prerequisiteFeatures = "None", string sourceBook = "Player's Handbook")
        {
            _name = name;
            _description = description;
            _prerequisiteAbility = prerequisiteAbility;
            _prerequisiteFeatures = prerequisiteFeatures;
            _sourceBook = sourceBook;
        }

        // Properties
        public string Name { get => _name; set => _name = value; }
        public string Description { get => _description; set => _description = value; }
        public string PrerequisiteAbility { get => _prerequisiteAbility; set => _prerequisiteAbility = value; }
        public string PrerequisiteFeatures { get => _prerequisiteFeatures; set => _prerequisiteFeatures = value; }
        public string SourceBook { get => _sourceBook; set => _sourceBook = value; }

        // Methods
        public virtual void ApplyBonus()
        {
            Console.WriteLine($"{_name} feat bonus is applied: {_description}");
        }

        public override string ToString()
        {
            string prereqInfo = "";
            if (_prerequisiteAbility != "None")
                prereqInfo += $" Prerequisite: {_prerequisiteAbility}{(_prerequisiteFeatures != "None" ? $" + {_prerequisiteFeatures}" : "")}";
            return $"{_name}: {_description}{(prereqInfo != "" ? $" |{prereqInfo}" : "")}";
        }
    }

    // ===== Core Rulebook Feats =====

    /// <summary>
    /// Increase one ability score of your choice by 2, or two ability scores of your choice by 1.
    /// </summary>
    public class AbilityScoreIncrease : Feat
    {
        public AbilityScoreIncrease() : base("Ability Score Increase",
            "Increase one ability score of your choice by 2, or two ability scores of your choice by 1.",
            "None", "None", "Player's Handbook")
        {
        }
    }

    /// <summary>
    /// You are agile and quick, able to act with speed and deceive others.
    /// </summary>
    public class Actor : Feat
    {
        public Actor() : base("Actor",
            "You gain the following benefits:\n" +
            "- You get a +1 bonus to Dexterity.\n" +
            "- You gain proficiency in the Disguise and Performance skills if you don't already have it.\n" +
            "- You can speak, read, and write Common fluently.\n" +
            "- When you are disguising yourself, you can mimic the speech of another person or make yourself sound foreign-looking.\n" +
            "- You can use the Hide action as a bonus action on your turn.",
            "Charisma 10+", "", "Player's Handbook")
        {
        }
    }

    /// <summary>
    /// Always alert, you have a keen awareness of your surroundings.
    /// </summary>
    public class Alert : Feat
    {
        public Alert() : base("Alert",
            "You gain the following benefits:\n" +
            "- You get a +5 bonus to initiative.\n" +
            "- You can't be surprised except by an incapacitated creature.\n" +
            "- Creatures don't gain advantage from being unseen when they attack you.",
            "None", "None", "Player's Handbook")
        {
        }
    }

    /// <summary>
    /// You've studied under an artificer, learning some of their techniques.
    /// </summary>
    public class ArtificerInitiate : Feat
    {
        public ArtificerInitiate() : base("Artificer Initiate",
            "You learn two cantrips of your choice from the artificer spell list. You also learn one 1st-level artificer spell. You cast these spells using Intelligence.\n" +
            "When you reach level 4, 8, 12, 16, and 20, you can choose a different spell from the artificer spell list to replace the one you replaced.",
            "Intelligence 10+", "", "Xanathar's Guide")
        {
        }
    }

    /// <summary>
    /// Your years of training have made you skilled and strong.
    /// </summary>
    public class Athlete : Feat
    {
        public Athlete() : base("Athlete",
            "You gain the following benefits:\n" +
            "- You get a +1 bonus to Strength or Dexterity (your choice).\n" +
            "- When you are prone, standing up uses only 5 feet of your movement.\n" +
            "- Spending a bonus action on your turn to climb or swim doesn't reduce your speed.\n" +
            "- You can make a running long jump or a running high jump after moving 5 feet on foot.",
            "None", "None", "Player's Handbook")
        {
        }
    }

    /// <summary>
    /// You are skilled at fighting with two weapons.
    /// </summary>
    public class DualWielder : Feat
    {
        public DualWielder() : base("Dual Wielder",
            "You gain the following benefits:\n" +
            "- You get a +1 bonus to Strength.\n" +
            "- You can use two-weapon fighting even with one-handed weapons.\n" +
            "- You can carry a shield in each hand.",
            "None", "None", "Player's Handbook")
        {
        }
    }

    /// <summary>
    /// Your magic spells are especially powerful.
    /// </summary>
    public class ElementalAdept : Feat
    {
        public ElementalAdept() : base("Elemental Adept",
            "Choose one damage type associated with your spellcasting: acid, cold, fire, lightning, or thunder.\n" +
            "- Spells you cast ignore resistance to the chosen damage type.\n" +
            "- Spells you cast deal maximum damage on a natural 1 on a damage die.",
            "Spellcasting ability", "Ability to cast spells", "Player's Handbook")
        {
        }
    }

    /// <summary>
    /// You are skilled at dealing heavy blows with your melee weapons.
    /// </summary>
    public class GreatWeaponMaster : Feat
    {
        public GreatWeaponMaster() : base("Great Weapon Master",
            "You get the following benefits:\n" +
            "- You gain proficiency with heavy melee weapons.\n" +
            "- Before you make a melee attack with a heavy weapon, you can choose to take a -5 penalty to the roll. If the attack hits and deals damage, the attack deals +10 extra damage.\n" +
            "- When you score a critical hit with a heavy melee weapon, you can deal extra damage to the target.",
            "Proficiency with melee weapons", "Ability to wield heavy weapons", "Player's Handbook")
        {
        }
    }

    /// <summary>
    /// You are nimble and agile like a halfling.
    /// </summary>
    public class HalflingNimble : Feat
    {
        public HalflingNimble() : base("Halfling Nimble",
            "You gain the following benefits:\n" +
            "- You get a +1 bonus to Dexterity.\n" +
            "- Your critical hit range for ranged attacks increases by 1.\n" +
            "- When you make an attack, roll a d4. You can add the number rolled to the attack roll if it would miss or fail.",
            "None", "None", "Player's Handbook")
        {
        }
    }

    /// <summary>
    /// You are trained in heavy armor.
    /// </summary>
    public class HeavyArmorMaster : Feat
    {
        public HeavyArmorMaster() : base("Heavy Armor Master",
            "You gain the following benefits:\n" +
            "- You gain proficiency with heavy armor.\n" +
            "- You get a +1 bonus to Strength.\n" +
            "- While you are wearing heavy armor, non-magical bludgeoning damage you take is reduced by 3.",
            "Proficiency with heavy armor", "", "Player's Handbook")
        {
        }
    }

    /// <summary>
    /// You can inspire others to strive and reach their goals.
    /// </summary>
    public class InspiringLeader : Feat
    {
        public InspiringLeader() : base("Inspiring Leader",
            "You can spend 10 minutes inspiring your companions. Before the inspired creature makes a check, it can roll a number of d6s equal to your Charisma modifier (minimum of one die) and add the result.",
            "Charisma 13+", "", "Player's Handbook")
        {
        }
    }

    /// <summary>
    /// You are lucky and often find fortune when luck should fail you.
    /// </summary>
    public class Lucky : Feat
    {
        public Lucky() : base("Lucky",
            "You have 3 luck points. When you make an attack roll, an ability check, or a saving throw, you can spend one luck point to roll a d20 and use the higher result.\n" +
            "You can also spend a luck point when an attack roll is made against you. Roll a d20 and use the higher of the two results.",
            "None", "None", "Player's Handbook")
        {
        }
    }

    /// <summary>
    /// You have studied magic, learning three cantrips and one 1st-level spell.
    /// </summary>
    public class MagicInitiate : Feat
    {
        public MagicInitiate() : base("Magic Initiate",
            "Choose a class: bard, cleric, druid, sorcerer, warlock, or wizard. You learn two cantrips and one 1st-level spell from that class's spell list.\n" +
            "Your spellcasting ability for these spells depends on the class you chose.\n" +
            "You can cast your 1st-level spell once at its lowest level, regaining the ability to do so when you finish a long rest.",
            "None", "", "Player's Handbook")
        {
        }
    }

    /// <summary>
    /// You are skilled at defeating and capturing creatures.
    /// </summary>
    public class MageSlayer : Feat
    {
        public MageSlayer() : base("Mage Slayer",
            "You gain the following benefits:\n" +
            "- When a creature within 5 feet of you casts a spell, you can use your reaction to make a melee weapon attack against that creature.\n" +
            "- You have advantage on saving throws against spells cast by creatures within 5 feet of you.\n" +
            "- As a reaction, you can complete the casting of a spell that has a casting time of 1 action if another creature casts a spell within 60 feet of you.",
            "None", "Proficiency with martial weapons", "Player's Handbook")
        {
        }
    }

    /// <summary>
    /// You are swift and can cover ground quickly.
    /// </summary>
    public class Mobile : Feat
    {
        public Mobile() : base("Mobile",
            "You gain the following benefits:\n" +
            "- Your speed increases by 10 feet.\n" +
            "- Ignoring difficult terrain costs you no extra movement.\n" +
            "- Disengaging doesn't provoke opportunity attacks.",
            "None", "None", "Player's Handbook")
        {
        }
    }

    /// <summary>
    /// You have some proficiency with medium armor.
    /// </summary>
    public class ModeratelyArmored : Feat
    {
        public ModeratelyArmored() : base("Moderately Armored",
            "You gain the following benefits:\n" +
            "- You gain proficiency with medium armor.\n" +
            "- You get a +1 bonus to an ability score of your choice.",
            "Proficiency with light armor", "", "Player's Handbook")
        {
        }
    }

    /// <summary>
    /// You are skilled at dealing poison.
    /// </summary>
    public class Poisoner : Feat
    {
        public Poisoner() : base("Poisoner",
            "You gain the following benefits:\n" +
            "- Bonus to initiative rolls.\n" +
            "- Advantage on attacks with poisoned weapons.\n" +
            "- You can create poison using a kit.",
            "Proficiency with poison", "", "Player's Handbook")
        {
        }
    }

    /// <summary>
    /// You can cast spells using ritual magic.
    /// </summary>
    public class RitualCaster : Feat
    {
        public RitualCaster() : base("Ritual Caster",
            "Choose a class: bard, cleric, druid, sorcerer, warlock, or wizard. You learn two cantrips and one 1st-level spell from that class's spell list with the ritual tag.\n" +
            "You can cast these spells as rituals. Your spellcasting ability depends on the chosen class.",
            "Spellcasting ability", "", "Player's Handbook")
        {
        }
    }

    /// <summary>
    /// You are skilled at dealing heavy blows.
    /// </summary>
    public class SavageAttacker : Feat
    {
        public SavageAttacker() : base("Savage Attacker",
            "Once per turn, you can reroll one of the damage dice in a weapon attack's damage roll and use the new result.",
            "None", "Proficiency with melee weapons", "Player's Handbook")
        {
        }
    }

    /// <summary>
    /// You are skilled at ranged attacks.
    /// </summary>
    public class Sharpshooter : Feat
    {
        public Sharpshooter() : base("Sharpshooter",
            "You get the following benefits:\n" +
            "- Long range doesn't impose disadvantage on your ranged attacks.\n" +
            "- You can take a -5 penalty to the attack roll. If the attack hits and deals damage, it deals +10 extra damage.\n" +
            "- Attacking from concealment imposes no penalty.",
            "None", "Proficiency with ranged weapons", "Player's Handbook")
        {
        }
    }

    /// <summary>
    /// You are skilled at hiding and moving silently.
    /// </summary>
    public class ShadowAdept : Feat
    {
        public ShadowAdept() : base("Shadow Adept",
            "You learn the invisibility spell once. Your spellcasting ability is Charisma.\n" +
            "Once you cast it, you can't cast it again until you finish a long rest.",
            "None", "", "Player's Handbook")
        {
        }
    }

    /// <summary>
    /// You are skilled at blocking attacks with your shield.
    /// </summary>
    public class ShieldMaster : Feat
    {
        public ShieldMaster() : base("Shield Master",
            "You gain the following benefits:\n" +
            "- If you are unaffected by an effect that allows you to make a Dexterity saving throw, you can use your reaction to move up to half your speed.\n" +
            "- You get a +2 bonus to Dexterity saves if you are using a shield.\n" +
            "- As a bonus action, you can force a creature within 5 feet of you to make a Dexterity saving throw against your spell save DC.",
            "None", "Proficiency with shields", "Player's Handbook")
        {
        }
    }

    /// <summary>
    /// You are skilled in many areas.
    /// </summary>
    public class Skilled : Feat
    {
        public Skilled() : base("Skilled",
            "You gain proficiency in three skills of your choice. You also gain proficiency with one musical instrument of your choice.",
            "None", "", "Player's Handbook")
        {
        }
    }

    /// <summary>
    /// You are skilled at hiding and stalking prey.
    /// </summary>
    public class Skulker : Feat
    {
        public Skulker() : base("Skulker",
            "You can try to hide even when you are only mildly obscured.\n" +
            "- You can make a Dexterity (Stealth) check as a bonus action on your turn.\n" +
            "- Even if you are being observed, you can attempt to hide if you are in an area of light that results in disadvantaged perception.",
            "Stealth proficiency", "", "Player's Handbook")
        {
        }
    }

    /// <summary>
    /// Your spells can target multiple creatures.
    /// </summary>
    public class SpellSniper : Feat
    {
        public SpellSniper() : base("Spell Sniper",
            "You gain the following benefits:\n" +
            "- Choose one cantrip from any class's spell list. It counts as known to you and uses your spellcasting ability.\n" +
            "- Your range for spells increases by 60 feet.\n" +
            "- You can target creatures that are heavily obscured or invisible.",
            "Spellcasting ability", "Ability to cast at least one spell", "Player's Handbook")
        {
        }
    }

    /// <summary>
    /// You are skilled with crossbows.
    /// </summary>
    public class CrossbowExpert : Feat
    {
        public CrossbowExpert() : base("Crossbow Expert",
            "You gain the following benefits:\n" +
            "- You ignore the loading property of crossbows.\n" +
            "- Your ranged attack attacks with hand crossbows don't suffer disadvantage.\n" +
            "- When you use the Attack action and make a melee attack with a one-handed weapon, you can also make a bonus action attack.",
            "None", "", "Xanathar's Guide")
        {
        }
    }

    /// <summary>
    /// You are resolute in the face of fear.
    /// </summary>
    public class SteadfastSoul : Feat
    {
        public SteadfastSoul() : base("Steadfast Soul",
            "You gain proficiency in Constitution saving throws.\n" +
            "- If you already have this proficiency, choose any saving throw and gain proficiency in it.",
            "Wisdom saving throw proficiency", "", "Xanathar's Guide")
        {
        }
    }

    /// <summary>
    /// You are eager for combat.
    /// </summary>
    public class SuperiorInitiative : Feat
    {
        public SuperiorInitiative() : base("Superior Initiative",
            "You get a +5 bonus to initiative.\n" +
            "- If you already have this benefit, choose any ability score and gain a +1 bonus.",
            "Dexterity 13+", "", "Player's Handbook")
        {
        }
    }

    /// <summary>
    /// You are skilled at fighting with unarmed strikes.
    /// </summary>
    public class TavernBrawler : Feat
    {
        public TavernBrawler() : base("Tavern Brawler",
            "You gain the following benefits:\n" +
            "- You get a +1 bonus to Strength and Constitution.\n" +
            "- Your unarmed strike uses a d4 damage die.\n" +
            "- You have advantage on an Athletics check made to grapple a creature.",
            "None", "", "Player's Handbook")
        {
        }
    }

    /// <summary>
    /// You are skilled at casting spells in combat.
    /// </summary>
    public class WarCaster : Feat
    {
        public WarCaster() : base("War Caster",
            "You gain the following benefits:\n" +
            "- You have advantage on Constitution saving throws that you make to maintain your concentration on a spell when you take damage.\n" +
            "- You can perform the somatic components of spells even if you have weapons or a shield in one or both hands.\n" +
            "- When a creature provokes an opportunity attack from you by leaving your reach, you can use your reaction to cast a spell at that creature. The spell must have a casting time of 1 action.",
            "Spellcasting ability", "", "Player's Handbook")
        {
        }
    }

    /// <summary>
    /// You are skilled with all types of weapons.
    /// </summary>
    public class WeaponMaster : Feat
    {
        public WeaponMaster() : base("Weapon Master",
            "You gain proficiency with all simple and martial weapons.\n" +
            "- You learn two combat styles of your choice.",
            "Proficiency with martial weapons", "", "Player's Handbook")
        {
        }
    }

    /// <summary>
    /// You are skilled at fighting with two weapons.
    /// </summary>
    public class TwoWeaponFighting : Feat
    {
        public TwoWeaponFighting() : base("Two-Weapon Fighting",
            "When you fight with two one-handed weapons, you can add the ability modifier of your off-hand weapon to the damage of the second attack.\n" +
            "- You can use a bonus action to make an attack with a light melee weapon that you're holding in your other hand.",
            "None", "Proficiency with light weapons", "Player's Handbook")
        {
        }
    }

    /// <summary>
    /// You are skilled at controlling the battlefield.
    /// </summary>
    public class Sentinel : Feat
    {
        public Sentinel() : base("Sentinel",
            "You have mastered techniques from two of the greatest warriors in existence:\n" +
            "- When you hit a creature with an opportunity attack, the creature's speed becomes 0 for the rest of the turn.\n" +
            "- Creatures provoke disadvantage on attack rolls against targets other than you while within your reach.\n" +
            "- When a creature within 5 feet of you attacks another target, you can use your reaction to make a melee weapon attack against that creature.",
            "None", "", "Player's Handbook")
        {
        }
    }

    /// <summary>
    /// You are skilled at cooking and feeding others.
    /// </summary>
    public class Chef : Feat
    {
        public Chef() : base("Chef",
            "You gain the following benefits:\n" +
            "- You gain proficiency with cook's utensils.\n" +
            "- At least half an hour of work produces meals that feed up to 5 characters for 1 gold per unit of materials spent.\n" +
            "- You can craft a special meal once you gain the ability to do so.\n" +
            "- You learn how to create healing rations.",
            "None", "Cooking proficiency", "Xanathar's Guide")
        {
        }
    }

    /// <summary>
    /// You are durable and resilient.
    /// </summary>
    public class Durable : Feat
    {
        public Durable() : base("Durable",
            "You gain the following benefits:\n" +
            "- Your Constitution score increases by 1.\n" +
            "- When you spend hit dice to recover hit points, each hit die restores twice your Constitution modifier.\n" +
            "- You can hold your breath for a number of minutes equal to your Constitution modifier (minimum 30 seconds).",
            "None", "", "Player's Handbook")
        {
        }
    }

    /// <summary>
    /// You are resilient and hard to affect.
    /// </summary>
    public class Resilient : Feat
    {
        public Resilient() : base("Resilient",
            "Choose one ability score: Constitution, Dexterity, Intelligence, Wisdom, or Strength.\n" +
            "- The chosen ability score increases by 1.\n" +
            "- You gain proficiency in saving throws using the chosen ability.",
            "Constitution saving throw proficiency", "", "Player's Handbook")
        {
        }
    }

    /// <summary>
    /// You can move things with your mind.
    /// </summary>
    public class Telekinetic : Feat
    {
        public Telekinetic() : base("Telekinetic",
            "You gain the following benefits:\n" +
            "- One ability score of your choice increases by 1.\n" +
            "- You learn the mage hand cantrip. The hand can manipulate objects and attack.\n" +
            "- As a bonus action, you can try to shove a creature within 30 feet.",
            "Spellcasting ability", "", "Xanathar's Guide")
        {
        }
    }

    /// <summary>
    /// You have telepathic abilities.
    /// </summary>
    public class Telepathic : Feat
    {
        public Telepathic() : base("Telepathic",
            "You gain the following benefits:\n" +
            "- One ability score of your choice increases by 1.\n" +
            "- You learn the thaumaturgy cantrip.\n" +
            "- You can communicate telepathically with any creature within 30 feet that understands a language.",
            "Spellcasting ability", "", "Xanathar's Guide")
        {
        }
    }

    /// <summary>
    /// You are skilled at grappling.
    /// </summary>
    public class Grappler : Feat
    {
        public Grappler() : base("Grappler",
            "You gain the following benefits:\n" +
            "- You have advantage on combat checks made to grapple a creature.\n" +
            "- You can use your action to attempt to pin a creature you are grappling. The target must succeed on a Strength or Dexterity save (your choice) or be restrained until you stop grappling it.\n" +
            "- Grappling doesn't impose disadvantage on attack rolls against the target.",
            "Strong hands", "", "Player's Handbook")
        {
        }
    }

    /// <summary>
    /// You are skilled at blocking attacks.
    /// </summary>
    public class Crasher : Feat
    {
        public Crasher() : base("Crasher",
            "You gain the following benefits:\n" +
            "- Your speed increases by 10 feet when you use a bonus action to dash.\n" +
            "- When you are within 5 feet of a creature and move closer to it, that creature must succeed on a Strength saving throw or be knocked prone.\n" +
            "- You gain advantage on attack rolls against creatures that are down.",
            "None", "", "Player's Handbook")
        {
        }
    }

    /// <summary>
    /// You have studied eldritch secrets.
    /// </summary>
    public class EldritchAdept : Feat
    {
        public EldritchAdept() : base("Eldritch Adept",
            "You learn the eldritch blast cantrip from the warlock spell list.\n" +
            "You gain an eldritch invocation of your choice. If the invocation has a prerequisite, you can choose it only if you meet the prerequisite.\n" +
            "If you have no levels in any class that grants invocations, you must meet the prerequisite to select the invocation.",
            "Spellcasting or pact magic ability", "Shadow of Mobe line", "Tasha's Cauldron")
        {
        }
    }

    /// <summary>
    /// You are skilled at dealing poison damage.
    /// </summary>
    public class PoisonedAdept : Feat
    {
        public PoisonedAdept() : base("Poisoned Adept",
            "You gain the following benefits:\n" +
            "- You learn the poison spray cantrip.\n" +
            "- When you deal poison damage, the target takes maximum damage on a roll of 1.\n" +
            "- You have advantage on saving throws against poison.",
            "Spellcasting ability", "", "Xanathar's Guide")
        {
        }
    }

    /// <summary>
    /// You are skilled at casting spells with area effects.
    /// </summary>
    public class SpellThief : Feat
    {
        public SpellThief() : base("Spell Thief",
            "You gain the following benefits:\n" +
            "- When a creature within 60 feet of you casts a spell, you can use your reaction to attempt to steal it.\n" +
            "- You learn one spell of your choice from any class's spell list.\n" +
            "- You have advantage on saving throws against spells.",
            "Spellcasting ability", "", "Tasha's Cauldron")
        {
        }
    }
}