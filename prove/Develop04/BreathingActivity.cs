class Breathing : BaseActivity
{
    public Breathing(string description) : base("Breathing", description)
    {
        
    }

    public void RunActivity()
    {
        decimal duration = StartActivity();
        for (int i = 0; i < Math.Floor(duration / 10); i++)
        {
            RunCountdown("Breath in", 5);
            RunCountdown("Breath out", 5);
        }
        EndActivity();
        
    }

}