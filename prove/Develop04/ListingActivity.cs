class Listing : BaseActivity
{
    public Listing(string description) : base("Listing", description)
    {
        
    }

    private List<string> prompts = new List<string>
    {
        "Who are people that you appreciate?",
        "What are personal strengths of yours?",
        "Who are people that you have helped this week?",
        "When have you felt the Holy Ghost this month?",
        "Who are some of your personal heroes?"
    };

    private Random random = new Random();
    public void RunActivity()
    {
        int duration = StartActivity();
        int promptIndex = random.Next(prompts.Count);
        string selectedPrompt = prompts[promptIndex];
        List<string> userAnswers = new List<string>();
        
        Console.WriteLine($"List as many responses as you can to the following prompt: ");
        Console.WriteLine("");
        Console.WriteLine($"--- {selectedPrompt} ---");
        Console.WriteLine("");
        
        DateTime now = DateTime.Now;
        DateTime endTime = now.AddSeconds(duration);

        RunCountdown("You may begin in ", 5);
        
        while (DateTime.Now < endTime)
        {
            Console.Write("> ");
            string input = Console.ReadLine();
        
            if (!string.IsNullOrEmpty(input))
            {
                userAnswers.Add(input);
            }
    }

    Console.WriteLine($"You listed {userAnswers.Count} items!");
        EndActivity();
    }
}