// Program.cs
class Program
{
    static void Main(string[] args)
    {
        string referenceText = "John 3:16";
        string scriptureText = "For God so loved the world that he gave his one and only Son, that whoever believes in him shall not perish but have eternal life.";
        
        Scripture scripture = new Scripture(referenceText, scriptureText);
        
        Console.WriteLine("Press Enter to practice hiding words, or type 'quit' to exit.");
        
        while (!scripture.AreAllWordsHidden())
        {
            Console.Clear();
            
            scripture.Display();
            
            Console.Write("\nPress Enter to continue, or type 'quit' to exit: ");
            string input = Console.ReadLine();
            
            if (input?.ToLower().Trim() == "quit")
            {
                break;
            }
            
            scripture.HideRandomWords(2);
        }
        
        Console.Clear();
        Console.WriteLine("Scripture fully hidden! Great job memorizing!");
    }
}