using System;
using System.Globalization;

class Program
{
    static void Main(string[] args)
    {
        Console.WriteLine("Enter a list of numbers, type 0 when done.");
        
        List<int> numbers = new List<int>();

        int numberInput = 1;

        while (numberInput != 0)
        {
            numberInput = int.Parse(Console.ReadLine());
            numbers.Add(numberInput);
        }

        int sum = numbers.Sum();
        double average = numbers.Average();
        int max = numbers.Max();

        Console.WriteLine($"The sum is: {sum}");
        Console.WriteLine($"The average is: {average}");
        Console.WriteLine($"The largest number is: {max}");
    }

}