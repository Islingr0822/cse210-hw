using System;

class Program
{
    static void Main(string[] args)
    {
        // BaseActivity myActivity = new BaseActivity("breathing", "this will help you breath");
        // myActivity.StartActivity();
        // myActivity.RunCountdown("Breath in:", 10);
        Console.WriteLine("Welcome to the mindfullness program!");
        Console.WriteLine("");
        string input = "0";
        while (input != "q")
        {
            Console.WriteLine("Please enter the desired activity: ");
            Console.WriteLine("1. Breathing");
            Console.WriteLine("2. Reflection");
            Console.WriteLine("3. Listing");
            Console.WriteLine("q. Quit");
            Console.Write("Your selection: ");

            input = Console.ReadLine();
            if (input == "1")
            {
                Breathing myBreathing = new Breathing("This activity will help you relax by walking your through breathing in and out slowly. Clear your mind and focus on your breathing.");
                myBreathing.RunActivity(); 
            }

            if (input == "2")
            {
                Reflection myReflection = new Reflection("This activity will help you reflect on times in your life when you have shown strength and resilience. This will help you recognize the power you have and how you can use it in other aspects of your life.");
                myReflection.RunActivity();
            }
        
            if (input == "3")
            {
               Listing myListing = new Listing("This activity will help you reflect on the good things in your life by having you list as many things as you can in a certain area.");
                myListing.RunActivity(); 
            }

             
        }

    }
}