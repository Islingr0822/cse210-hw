class Points
{
    private int _pointTotal = 0;

    public void UpdatePoints(int points)
    {
        _pointTotal += points;
    }

    public int GetPointTotal()
    {
        return _pointTotal;
    }

    public void SetPointTotal(int points)
    {
        _pointTotal = points;
    }

    public void DisplayPoints()
    {
        Console.WriteLine($"Current Points: {_pointTotal}");
    }
}
