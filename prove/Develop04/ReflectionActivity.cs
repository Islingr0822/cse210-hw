class Reflection : BaseActivity
{
    public Reflection(string description) : base("Reflection", description)
    {
        
    }

    private List<string> prompts = new List<string>
    {
        "Think of a time when you stood up for someone else.",
        "Think of a time when you did something really difficult.",
        "Think of a time when you helped someone in need.",
        "Think of a time when you did something truly selfless."
    };

    private List<string> questions = new List<string>
    {
        "Why was this experience meaningful to you?",
        "Have you ever done anything like this before?",
        "How did you get started?",
        "How did you feel when it was complete?",
        "What made this time different than other times when you were not as successful?",
        "What is your favorite thing about this experience?",
        "What could you learn from this experience that applies to other situations?",
        "What did you learn about yourself through this experience?",
        "How can you keep this experience in mind in the future?"
    };
    
    private Random random = new Random();
    public void RunActivity()
    {
        int promptIndex = random.Next(prompts.Count);
        string selectedPrompt = prompts[promptIndex];
        decimal duration = StartActivity();
        Console.WriteLine($"{selectedPrompt} Click when ready: ");
        Console.ReadKey();
        for (int i = 0; i < Math.Floor(duration / 5); i++)
        {
            int questionIndex = random.Next(questions.Count);
            string currentQuestion = questions[questionIndex];
            RunCountdown(currentQuestion, 5);
        }
        EndActivity();

    }
}