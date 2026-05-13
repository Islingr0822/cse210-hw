
public class Journal
{
    List<JournalEntry> _journalEntries = new List<JournalEntry>();

    public void AddJournalEntry(JournalEntry journalEntry)
    {
        _journalEntries.Add(journalEntry);
    }

    public void Clear()
    {
        _journalEntries.Clear();
    }

    public string[] GetLinesForFile()
    {
        string[] lines = new string[_journalEntries.Count];
        for (int i = 0; i < _journalEntries.Count; i++)
        {
            lines[i] = _journalEntries[i].CreateFileSystemString();
        }

        return lines;
    }

    public void DisplayJournal()
    {
        foreach (JournalEntry journalEntry in _journalEntries)
        {
            journalEntry.DisplayJournalEntry();
        }
    }
}