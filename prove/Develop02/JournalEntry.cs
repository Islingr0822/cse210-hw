
public class JournalEntry
{
    string _date;
    string _prompt;
    string _response;
    string[] _prompts =
    {
        "Who was the most interesting person I interacted with today?",
        "What was the best part of my day?",
        "How did I see the hand of the Lord in my life today?",
        "What was the strongest emotion I felt today?",
        "If I had one thing I could do over today, what would it be?"
    };

    Random rnd = new Random();

    public void CreateJournalEntry()
    {
        _date = DateTime.Now.ToShortDateString();
        _prompt = _prompts[rnd.Next(0, _prompts.Length)];
        Console.WriteLine(_prompt);
        _response = Console.ReadLine();

    }

    public static JournalEntry parseFileData(string line)
    {
        string[] parts = line.Split('#', 3);
        JournalEntry entry = new JournalEntry();
        if (parts.Length >= 3)
        {
            entry._date = parts[0];
            entry._prompt = parts[1];
            entry._response = parts[2];
        }

        return entry;
    }

    public void DisplayJournalEntry()
    {
        Console.WriteLine($"{_date}, {_prompt}, {_response}");
    }

    public string CreateFileSystemString()
    {
        return $"{_date}#{_prompt}#{_response}";
    }
}