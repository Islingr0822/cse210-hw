using System;

class Program
{
    static void Main(string[] args)
    {
        // BaseGoal myGoal = new BaseGoal();
        // myGoal.SetName();
        // myGoal.SetDescription();
        // myGoal.NumberOfPoints();
        // Console.WriteLine(myGoal.GetDisplayString());
        // myGoal.MarkComplete();
        // Console.WriteLine(myGoal.GetDisplayString());

        SimpleGoal myGoal = new SimpleGoal();
        myGoal.CreateGoal();
        Console.WriteLine(myGoal.GetDisplayString());
        myGoal.RecordEvent();
        Console.WriteLine(myGoal.GetDisplayString());
    }
}

