using System;

class Program
{
    static void Main(string[] args)
    {
        // BaseActivity myActivity = new BaseActivity("breathing", "this will help you breath");
        // myActivity.StartActivity();
        // myActivity.RunCountdown("Breath in:", 10);
        Console.WriteLine("Welcome to the mindfullness program:");
        string input = "0";
        while (input != "q")
        {
            Console.WriteLine("Please enter your choice: ");
            input = Console.ReadLine();
            if (input == "1")
            {
                Breathing myBreathing = new Breathing("This will help you breath better");
                myBreathing.RunActivity(); 
            }

            if (input == "2")
            {
                Reflection myReflection = new Reflection("This will help you reflect on life");
                myReflection.RunActivity();
            }
        
            if (input == "3")
            {
               Listing myListing = new Listing("This will help you list things");
                myListing.RunActivity(); 
            }

             
        }

    }
}