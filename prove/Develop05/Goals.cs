class Goals
{
    private string _goalType;
    private List<string> _goals = new List<string>();

        public void AddGoal(string goalType, string goal)
    {
        _goalType = goalType;
        _goals.Add(goal);
    }

    public void ListGoals()
    {
        foreach (string goal in _goals)
        {
            Console.WriteLine(goal);
        }
    }
}