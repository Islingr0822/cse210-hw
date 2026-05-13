using System;

class Program
{
    static void Main(string[] args)
    {
        Menu();
    }

    static void Menu()
    {
        /*Console.WriteLine("Welcome to the Journal Program!");
        Console.WriteLine("Please select one of the following options: ");

        Console.WriteLine("1. Write");
        Console.WriteLine("2. Display");
        Console.WriteLine("3. Load");
        Console.WriteLine("4. Save");
        Console.WriteLine("5. Quit");*/

        JournalEntry journalEntry = new JournalEntry();
        journalEntry.CreateJournalEntry();
        //journalEntry.DisplayJournalEntry();
        //Console.WriteLine(journalEntry.CreateFileSystemString());
        JournalEntry journalEntry1 = new JournalEntry();
        journalEntry1.CreateJournalEntry();
        
        Journal myJournal = new Journal();
        myJournal.AddJournalEntry(journalEntry);
        myJournal.AddJournalEntry(journalEntry1);
        myJournal.DisplayJournal();
    }
}