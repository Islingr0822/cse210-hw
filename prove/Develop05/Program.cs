using System;

class Program
{
    static void Main(string[] args)
    {
        Console.WriteLine("Welcome to the Goal Program! ");
        Console.WriteLine("");
        string input = "0";

        Goals myGoals = new Goals();
        Points myPoints = new Points();

        while (input != "q")
        {
            myPoints.DisplayPoints();
            Console.WriteLine("");
            Console.WriteLine("Please select one of the following: ");
            Console.WriteLine("");
            Console.WriteLine("1. Create New Goal");
            Console.WriteLine("2. List Goals");
            Console.WriteLine("3. Save Goals");
            Console.WriteLine("4. Load Goals");
            Console.WriteLine("5. Record Event");
            Console.WriteLine("q. Quit Program");

            Console.Write("Input: ");
            input = Console.ReadLine();

            if (input == "1")
            {
                Console.WriteLine("What type of goal? ");
                Console.WriteLine("");
                Console.WriteLine("1. Simple Goal");
                Console.WriteLine("2. Eternal Goal");
                Console.WriteLine("3. Checklist Goal");
                Console.WriteLine("");

                Console.Write("Input: ");
                string goalInput = Console.ReadLine();

                if (goalInput == "1")
                {
                    SimpleGoal myGoal = new SimpleGoal();
                    myGoal.CreateGoal();
                    myGoals.AddGoal(myGoal);
                }
                else if (goalInput == "2")
                {
                    EternalGoal myGoal = new EternalGoal();
                    myGoal.CreateGoal();
                    myGoals.AddGoal(myGoal);
                }
                else if (goalInput == "3")
                {
                    ChecklistGoal myGoal = new ChecklistGoal();
                    myGoal.CreateGoal();
                    myGoals.AddGoal(myGoal);
                }
            }
            else if (input == "2")
            {
                Console.WriteLine("");
                myGoals.ListGoals();
                Console.WriteLine("");
            }
            else if (input == "3")
            {
                Console.Write("What is the filename for the goal file? ");
                string filename = Console.ReadLine();
                myGoals.SaveGoals(filename, myPoints.GetPointTotal());
            }
            else if (input == "4")
            {
                Console.Write("What is the filename for the goal file? ");
                string filename = Console.ReadLine();
                myGoals.LoadGoals(filename, myPoints);
            }
            else if (input == "5")
            {
                Console.WriteLine("");
                myGoals.ListGoals();
                Console.Write("Which goal did you accomplish? ");
                int goalNumber = int.Parse(Console.ReadLine());
                int pointsEarned = myGoals.RecordEventAt(goalNumber - 1);
                myPoints.UpdatePoints(pointsEarned);

                if (pointsEarned > 0)
                {
                    Console.WriteLine($"Congratulations! You have earned {pointsEarned} points!");
                }
                else
                {
                    Console.WriteLine("You have already completed this goal!");
                }

                Console.WriteLine("");
            }
            else if (input == "q"){}
            else
            {
                Console.WriteLine("Incorrect value, please try again");
            }
        }
    }
}
