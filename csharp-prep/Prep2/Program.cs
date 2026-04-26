using System;

class Program
{
    static void Main(string[] args)
    {
        Console.Write("What was your grade percentage in the class? ");
        int gradePercentage = int.Parse(Console.ReadLine());
        string letterGrade;
        
        if (gradePercentage >= 90)
        {
            letterGrade = "A";
        }
        
        else if (gradePercentage >= 80)
        {
            letterGrade = "B";
        }
        
        else if (gradePercentage >= 70)
        {
            letterGrade = "C";
        }
        
        else if (gradePercentage >= 60)
        {
            letterGrade = "D";
        }
        
        else 
        {
            letterGrade = "F";
        }

        Console.WriteLine($"Your letter grade is {letterGrade}");

        if (letterGrade == "A" || letterGrade == "B" || letterGrade == "C")
        {
            Console.WriteLine("Congratulations on passing!");
        }

        else
        {
            Console.WriteLine("Tough luck kid, you'll gettem next time.");
        }
    }
}