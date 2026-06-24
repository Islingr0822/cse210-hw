using System.Runtime.CompilerServices;

class BaseGoal
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

    public void SetName()
    {
        Console.Write("What is the name of the goal?: ");
        _name = Console.ReadLine();
    }

    public void SetDescription()
    {
        Console.Write($"Enter the description for {_name} goal: ");
        _description = Console.ReadLine();

    }

    public void NumberOfPoints()
    {
        Console.Write($"Enter the points earned for {_name} goal: ");
        _numberOfPoints = int.Parse(Console.ReadLine());
    }

    public int MarkComplete()
    {
        _status = true;
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
}