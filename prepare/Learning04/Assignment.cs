class Assignment
{
    private string  _studentName;
    private string _topic;

    public string GetSummary(string studentName, string topic)
    {
        _studentName = studentName;
        _topic = topic;

        return $"{_studentName} - {_topic}";
    }
}