class ChecklistGoal : BaseGoal
{
    private int _bonusPoints;
    private int _timesCompleted;
    private int _completedGoal;

    public ChecklistGoal() : base()
    {
    }

    public ChecklistGoal(
        string name,
        string description,
        int points,
        int bonusPoints,
        int completedGoal,
        int timesCompleted,
        bool status)
        : base(name, description, points, status)
    {
        _bonusPoints = bonusPoints;
        _completedGoal = completedGoal;
        _timesCompleted = timesCompleted;
    }

    public int BonusPoints()
    {
        Console.Write("How many bonus points for this goal? ");
        _bonusPoints = int.Parse(Console.ReadLine());
        return _bonusPoints;
    }

    public int SetTimesCompleted()
    {
        Console.Write("How many times should this goal be completed? ");
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

    public override int RecordEvent()
    {
        if (_status)
        {
            return 0;
        }

        _timesCompleted++;
        int pointsEarned = _numberOfPoints;

        if (_timesCompleted >= _completedGoal)
        {
            _status = true;
            pointsEarned += _bonusPoints;
        }

        return pointsEarned;
    }

    public override string GetDisplayString()
    {
        char statusMarker = ' ';
        if (_status)
        {
            statusMarker = 'X';
        }
        return $"[{statusMarker}] Name: {_name}, Description: {_description}, Completed {_timesCompleted}/{_completedGoal}, points earned {_numberOfPoints}, bonus {_bonusPoints}";
    }

    public override string GetStringRepresentation()
    {
        return $"ChecklistGoal:{_name},{_description},{_numberOfPoints},{_bonusPoints},{_completedGoal},{_timesCompleted},{_status}";
    }

    public static ChecklistGoal CreateFromString(string details)
    {
        string[] parts = details.Split(',');
        return new ChecklistGoal(
            parts[0],
            parts[1],
            int.Parse(parts[2]),
            int.Parse(parts[3]),
            int.Parse(parts[4]),
            int.Parse(parts[5]),
            bool.Parse(parts[6])
        );
    }
}
