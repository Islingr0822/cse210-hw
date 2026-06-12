using System.ComponentModel;

class BaseActivity
{
    private string _name;
    private string _description;
    private int _duration;
    private DateTime _endTime;

    public BaseActivity(string name, string description)
    {
        _name = name;
        _description = description;
        _duration = 0;
        _endTime = DateTime.Now;
    }

    public int StartActivity()
    {
        Console.Clear();
        Console.WriteLine($"Welcome to the {_name} activity!");
        Console.WriteLine("");
        
        Console.WriteLine(_description);
        Console.WriteLine("");
        
        Console.Write("How many seconds for this activity? ");
        _duration = int.Parse(Console.ReadLine());
        Console.Clear();
        return _duration;
    }

    public void EndActivity()
    {
        Console.WriteLine($"Great Job!");
        Console.WriteLine("");
        Console.Write($"You've completed another {_duration} seconds of {_name}");
        Spinner();
        Console.Clear();

    }

    public void Spinner()
    {
        string animationString = "-\\|/";
        DateTime now = DateTime.Now;
        DateTime endTime = now.AddSeconds(5);

        int sleepTime = 100;
        int index = 0;
        while(DateTime.Now < endTime)
        {
            Console.Write(animationString[index++ % animationString.Length]);
            Thread.Sleep(sleepTime);
            Console.Write("\b");
        }
    }
    public void RunCountdown(string message, int duration)
    {
        Console.Write($"{message}: ");
        while(duration >= 0)
        {
            Console.Write($"{duration--, 2}");
            Thread.Sleep(1000);
            Console.Write("\b\b");           
        }
        Console.WriteLine();
    }
 
    
}
