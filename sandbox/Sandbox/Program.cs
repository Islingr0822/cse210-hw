using System;

class Program
{
    static void Main(string[] args)
    {
        Console.WriteLine("Hello Sandbox World!");

        int x = 10;
        if (x == 11)
        {
            Console.WriteLine("Hey Bob");
        }

        for(int i = 0; i <= 32; ++i)
        {
            int powerNumber = (int)Math.Pow(2, i);
            Console.WriteLine($"2 to the power of {i} is: {powerNumber}");

        }

        List<int> myData = new List<int>();
        myData.Add(1);
        myData.Add(2);
        myData.Add(3);
        foreach(int i in myData)
        {
            Console.WriteLine(i);
        }
    }
}