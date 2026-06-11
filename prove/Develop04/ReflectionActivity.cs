class Reflection : BaseActivity
{
    public Reflection(string description) : base("Reflection", description)
    {
        
    }

    public void RunActivity()
    {
        StartActivity();
        RunCountdown("Think of a time when you stood up for someone else.", 30);

    }
}