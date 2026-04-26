using System;

class Program
{
    static void Main(string[] args)
    {
        Console.Write("What is your first name: ");
        string firstName; 
        firstName = Console.ReadLine();

        Console.Write("What is your last name? ");
        string lastName;
        lastName = Console.ReadLine();

        Console.Write($"Your name is {lastName}, {firstName} {lastName}");
    }
}