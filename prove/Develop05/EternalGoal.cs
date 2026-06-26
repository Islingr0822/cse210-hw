class EternalGoal : BaseGoal
{
    public EternalGoal() : base()
    {
    }

    public EternalGoal(string name, string description, int points)
        : base(name, description, points, false)
    {
    }

    public override void CreateGoal()
    {
        SetName();
        SetDescription();
        NumberOfPoints();
    }

    public override void RecordEvent()
    {
        AwardPoints();
    }

    public override string GetStringRepresentation()
    {
        return $"EternalGoal:{_name},{_description},{_numberOfPoints}";
    }

    public static EternalGoal CreateFromString(string details)
    {
        string[] parts = details.Split(',');
        return new EternalGoal(parts[0], parts[1], int.Parse(parts[2]));
    }
}
