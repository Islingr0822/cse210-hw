using System;

class Program
{
    static void Main(string[] args)
    {
        Console.WriteLine("Welcome to the Goal Program! ");
        Console.WriteLine("");
        string input = "0";
        
        while (input != "q")
        {
            Console.Clear();

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
                
                string goalInput = "q";

                if (goalInput == "1")
                {
                    SimpleGoal myGoal = new SimpleGoal();
                    myGoal.CreateGoal();
                }

                else if (goalInput == "2")
                {
                    EternalGoal myGoal = new EternalGoal();
                    myGoal.CreateGoal();
                }

                else if (goalInput == "3")
                {
                    ChecklistGoal myGoal = new ChecklistGoal();
                    myGoal.CreateGoal();
                }
  
            }

            else if (input == "2")
            {
                
            }
            
            else if (input == "3")
            {
                
            }

            else if (input == "4")
            {
                
            }

            else if (input == "5")
            {
                
            }

            else if (input == "q")
            {
             
            }

            else
            {
                Console.WriteLine("Incorrect value, please try again");
            }
            
            
        }

        

    }
}

