class MathAssignment : Assignment
{
    private string _textbookSection;
    private string _problems;

    public string GetHomeworkList(string textbookSection, string problems)
    {
        _textbookSection = textbookSection;
        _problems = problems;

        return $"{_textbookSection} {_problems}";
    }

}

