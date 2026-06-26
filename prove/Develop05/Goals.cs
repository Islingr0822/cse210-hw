class Goals
{
    private List<BaseGoal> _goals = new List<BaseGoal>();

    public void AddGoal(BaseGoal goal)
    {
        _goals.Add(goal);
    }

    public void ListGoals()
    {
        foreach (BaseGoal goal in _goals)
        {
            Console.WriteLine(goal.GetDisplayString());
        }
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
