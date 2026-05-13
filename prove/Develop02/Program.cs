using System;

class Program
{
    static void Main(string[] args)
    {
        Journal myJournal = new Journal();
        Console.WriteLine("Welcome to the Journal Program!");

        int numbered_response = Menu();


        while (numbered_response <= 4)
        {
            if (numbered_response == 1)
            {
                JournalEntry journalEntry = new JournalEntry();
                journalEntry.CreateJournalEntry();
                myJournal.AddJournalEntry(journalEntry);
            }
            else if (numbered_response == 2)
            {
                myJournal.DisplayJournal();
            }
            else if (numbered_response == 3)
            {
                Console.WriteLine("Please enter the file name: ");
                string fileName = Console.ReadLine();
                string[] lines = File.ReadAllLines(fileName);
                myJournal.Clear();
                foreach (string line in lines)
                {
                    myJournal.AddJournalEntry(JournalEntry.parseFileData(line));
                }
            }
            else if (numbered_response == 4)
            {
                Console.WriteLine("Please enter the file name: ");
                string fileName = Console.ReadLine();               
                File.WriteAllLines("myJournal.csv", myJournal.GetLinesForFile());
            }


            numbered_response = Menu();

        }
    }

    static int Menu()
    {
        Console.WriteLine("Please select one of the following options: ");

        Console.WriteLine("1. Write");
        Console.WriteLine("2. Display");
        Console.WriteLine("3. Load");
        Console.WriteLine("4. Save");
        Console.WriteLine("5. Quit");

        string response = Console.ReadLine();
        int numbered_response = int.Parse(response);

        return numbered_response;
    }
}