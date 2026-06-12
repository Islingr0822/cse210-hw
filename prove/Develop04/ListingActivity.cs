class Listing : BaseActivity
{
    public Listing(string description) : base("Listing", description)
    {
        
    }

    public void RunActivity()
    {
        StartActivity();
        EndActivity();
    }
}