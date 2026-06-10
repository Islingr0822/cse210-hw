class Breathing : BaseActivity
{
    public Breathing(string description) : base("Breathing", description)
    {
        
    }

    public void RunActivity()
    {
        StartActivity();
        RunCountdown("Breath in", 4);
        RunCountdown("Breath out", 6);
    }

}