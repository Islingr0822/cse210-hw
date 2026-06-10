using System;

class Program
{
    static void Main(string[] args)
    {
        BaseActivity myActivity = new BaseActivity("breathing", "this will help you breath");
        myActivity.StartActivity();
        myActivity.RunCountdown("Breath in:", 10);

        Breathing myBreathing = new Breathing("This will help you breath better");
        myBreathing.RunActivity();
        
    }
}