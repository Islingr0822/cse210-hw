class ChecklistGoal : BaseGoal
{
    
    private int _bonusPoints;
    private int _timesCompleted;
    private int _completedGoal;
    public ChecklistGoal() : base()
    {
        
    }

    public int BonusPoints()
    {
        Console.WriteLine("How many bonus points for this goal? ");
        _bonusPoints = int.Parse(Console.ReadLine());
        return _bonusPoints;
    }

    public int SetTimesCompleted()
    {
        Console.WriteLine("How many times should this goal be completed? ");
        _completedGoal = int.Parse(Console.ReadLine());
        return _completedGoal;
    }

    public override void CreateGoal()
    {
        SetName();
        SetDescription();
        NumberOfPoints();
        SetTimesCompleted();
        BonusPoints();
    }

    public override void RecordEvent()
    {
        MarkComplete();
    }
}