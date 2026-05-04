using System;

class Program
{
    static void Main(string[] args)
    {
        Console.WriteLine("Hello Learning02 World!");

        Job job1 = new Job();

        job1._company = "Microslop";
        job1._jobTitle = "Vibe Coder";
        job1._startDate = 2022;
        job1._endDate = 2025;
        
        Job job2 = new Job();

        job2._company = "Apple";
        job2._jobTitle = "Software Engineer";
        job2._startDate = 2022;
        job2._endDate = 2025;        
        
        Resume myResume = new Resume();

        myResume._name = "Tristan Sands";

        myResume._jobs.Add(job1);
        myResume._jobs.Add(job2);

        myResume.Display();
    }
}