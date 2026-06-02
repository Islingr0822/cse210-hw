public class Reference
{
    private string _book;
    private int _startChapter;
    private int _startVerse;
    private int? _endVerse;

    public Reference(string book, int chapter, int verse)
    {
        _book = book;
        _startChapter = chapter;
        _startVerse = verse;
        _endVerse = null;
    }

    public Reference(string book, int chapter, int startVerse, int endVerse)
    {
        _book = book;
        _startChapter = chapter;
        _startVerse = startVerse;
        _endVerse = endVerse;
    }

    public override string ToString()
    {
        if (_endVerse.HasValue && _endVerse != _startVerse)
        {
            return $"{_book} {_startChapter}:{_startVerse}-{_endVerse}";
        }
        else
        {
            return $"{_book} {_startChapter}:{_startVerse}";
        }
    }
}