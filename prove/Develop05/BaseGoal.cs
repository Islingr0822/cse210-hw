using System.Runtime.CompilerServices;

abstract class BaseGoal
{
    
    
    
    private string _name;
    private string _description;
    private int _numberOfPoints;

    private bool _status;
    private string _goalType;
    

    public BaseGoal()
    {
        _name = "";
        _description = "";
        _numberOfPoints = 0;
        _status = false;
        _goalType = "";
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

    public abstract void CreateGoal();
    public abstract void RecordEvent();
    
        
    
}