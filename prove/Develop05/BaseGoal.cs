abstract class BaseGoal
{
    protected string _name;
    protected string _description;
    protected int _numberOfPoints;
    protected bool _status;

    public BaseGoal()
    {
        _name = "";
        _description = "";
        _numberOfPoints = 0;
        _status = false;
    }

    protected BaseGoal(string name, string description, int points, bool status)
    {
        _name = name;
        _description = description;
        _numberOfPoints = points;
        _status = status;
    }

    protected void SetName()
    {
        Console.Write("What is the name of the goal?: ");
        _name = Console.ReadLine();
    }

    protected void SetDescription()
    {
        Console.Write($"Enter the description for {_name} goal: ");
        _description = Console.ReadLine();
    }

    protected void NumberOfPoints()
    {
        Console.Write($"Enter the points earned for {_name} goal: ");
        _numberOfPoints = int.Parse(Console.ReadLine());
    }

    public int MarkComplete()
    {
        _status = true;
        return AwardPoints();
    }

    public int AwardPoints()
    {
        return _numberOfPoints;
    }

    public virtual string GetDisplayString()
    {
        char statusMarker = ' ';
        if (_status)
        {
            statusMarker = 'X';
        }
        return $"[{statusMarker}] Name: {_name}, Description: {_description}, points earned {_numberOfPoints}";
    }

    public abstract string GetStringRepresentation();

    public static BaseGoal CreateFromFileLine(string line)
    {
        string[] parts = line.Split(':', 2);
        string goalType = parts[0];
        string details = parts[1];

        if (goalType == "SimpleGoal")
        {
            return SimpleGoal.CreateFromString(details);
        }
        else if (goalType == "EternalGoal")
        {
            return EternalGoal.CreateFromString(details);
        }
        else if (goalType == "ChecklistGoal")
        {
            return ChecklistGoal.CreateFromString(details);
        }

        throw new Exception($"Unknown goal type: {goalType}");
    }

    public abstract void CreateGoal();
    public abstract int RecordEvent();
}
