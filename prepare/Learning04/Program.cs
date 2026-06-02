using System;

class Program
{
    static void Main(string[] args)
    {
        
        Assignment assignment1 = new Assignment();
        MathAssignment mathAssignment = new MathAssignment();
        
        Console.WriteLine(assignment1.GetSummary("John", "C#"));
        Console.WriteLine(mathAssignment.GetHomeworkList("Section 4", "Problem 2"));

        Assignment assignment2 = new Assignment();
        WritingAssignment writingAssignment = new WritingAssignment();

        Console.WriteLine(assignment2.GetSummary("Jane", "English"));
        Console.WriteLine(writingAssignment.GetWritingInformation("The Great Gatsby"));
    }
}