class WritingAssignment : Assignment
{
    private string _title;

    public string GetWritingInformation(string title)
    {
        _title = title;
        return $"Title: {_title}";
    }
}

