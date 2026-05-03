using System;

class Program
{
    static void Main(string[] args)
    {   bool keepLoop = true;

        Random rnd = new Random();
        int randomNumber = rnd.Next(1,101);

        while (keepLoop == true)
        {
            Console.Write("What is your guess? ");
            int guess = int.Parse(Console.ReadLine());

            if (guess == randomNumber)
            {
                keepLoop = false;
            }

            else if (guess > randomNumber)
            {
                Console.WriteLine("Good guess, but not quite. Try a little lower.");
            }

            else if (guess < randomNumber)
            {
                Console.WriteLine("Good guess, but not quite. Try a little higher.");
            }
        }

    }
}