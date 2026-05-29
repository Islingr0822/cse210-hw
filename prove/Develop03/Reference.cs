class Reference
{
    private string _book;
    private int _chapter;
    private int _startVerse;
    private int _endVerse;

    public void SetBook()
    {
        Console.WriteLine("Please enter the book the scripture is in: ");
        _book = Console.ReadLine();
    }
    
    public void SetChapter()
    {
        Console.WriteLine("Please enter the chapter number for the scripture: ");
        _chapter = int.Parse(Console.ReadLine());
    }

    public void SetStartVerse()
    {
        Console.WriteLine("Please enter start verse: ");
        _startVerse = int.Parse(Console.ReadLine());

    }

    public void SetEndVerse()
    {
        Console.WriteLine("Please enter the end verse: ");
        _endVerse = int.Parse(Console.ReadLine());
    }
}