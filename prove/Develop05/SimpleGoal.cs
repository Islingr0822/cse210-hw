class SimpleGoal : BaseGoal
{
    public SimpleGoal() : base()
    {
    }

    public SimpleGoal(string name, string description, int points, bool status)
        : base(name, description, points, status)
    {
    }

    public override void CreateGoal()
    {
        SetName();
        SetDescription();
        NumberOfPoints();
    }

    public override int RecordEvent()
    {
        if (_status)
        {
            return 0;
        }

        return MarkComplete();
    }

    public override string GetStringRepresentation()
    {
        return $"SimpleGoal:{_name},{_description},{_numberOfPoints},{_status}";
    }

    public static SimpleGoal CreateFromString(string details)
    {
        string[] parts = details.Split(',');
        return new SimpleGoal(
            parts[0],
            parts[1],
            int.Parse(parts[2]),
            bool.Parse(parts[3])
        );
    }
}
