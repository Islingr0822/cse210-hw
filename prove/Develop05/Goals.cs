class Goals
{
    private List<BaseGoal> _goals = new List<BaseGoal>();

    public void AddGoal(BaseGoal goal)
    {
        _goals.Add(goal);
    }

    public void ListGoals()
    {
        if (_goals.Count == 0)
        {
            Console.WriteLine("You haven't created any goals yet.");
            return;
        }

        for (int i = 0; i < _goals.Count; i++)
        {
            Console.WriteLine($"{i + 1}. {_goals[i].GetDisplayString()}");
        }
    }

    public int RecordEventAt(int index)
    {
        return _goals[index].RecordEvent();
    }

    public void SaveGoals(string filename, int pointTotal)
    {
        using (StreamWriter outputFile = new StreamWriter(filename))
        {
            outputFile.WriteLine(pointTotal);

            foreach (BaseGoal goal in _goals)
            {
                outputFile.WriteLine(goal.GetStringRepresentation());
            }
        }
    }

    public void LoadGoals(string filename, Points points)
    {
        string[] lines = File.ReadAllLines(filename);
        _goals.Clear();

        if (lines.Length == 0)
        {
            return;
        }

        points.SetPointTotal(int.Parse(lines[0]));

        for (int i = 1; i < lines.Length; i++)
        {
            _goals.Add(BaseGoal.CreateFromFileLine(lines[i]));
        }
    }
}
