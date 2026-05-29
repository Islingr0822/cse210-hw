// Scripture.cs
public class Scripture
{
    private Reference _reference;
    private List<Word> _words;

    public Scripture(string referenceText, string text)
    {
        _reference = new Reference("John", 3, 16);
        
        _words = new List<Word>();
        string[] wordArray = text.Split(' ');
        foreach (string word in wordArray)
        {
            _words.Add(new Word(word));
        }
    }

    public void Display()
    {
        Console.WriteLine(_reference.ToString());
        Console.WriteLine();
        
        foreach (Word word in _words)
        {
            Console.Write(word.GetDisplayText() + " "); 
        }
        Console.WriteLine();
    }

    public void HideRandomWords(int count = 3)
    {
        Random random = new Random();
        List<Word> visibleWords = _words.Where(w => !w.IsHidden()).ToList();
        
        if (visibleWords.Count == 0)
            return;
            
        int wordsToHide = Math.Min(count, visibleWords.Count);
        
        for (int i = 0; i < wordsToHide; i++)
        {
            int randomIndex = random.Next(visibleWords.Count);
            visibleWords[randomIndex].Hide();
            visibleWords.RemoveAt(randomIndex);
        }
    }

    public bool AreAllWordsHidden()
    {
        return _words.All(w => w.IsHidden());
    }
}