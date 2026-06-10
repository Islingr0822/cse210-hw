using System.Net.NetworkInformation;

class Porgram
{
    public static void Main(string[] args)
    {
        for (int i = 0; i < 20; i++)
        {
            // Console.Write("+");
            // Thread.Sleep(250);
            // Console.Write('\b');

            // Console.Write("-");
            // Thread.Sleep(250);
            // Console.Write("\b");

            string animationString = "-\\|/";
            DateTime now = DateTime.Now;
            DateTime endTime = now.AddSeconds(10);

            int sleepTime = 100;
            int index = 0;
            while(DateTime.Now < endTime)
            {
                Console.Write(animationString[index++ % animationString.Length]);
                Thread.Sleep(sleepTime);
                Console.Write("\b");
            }

        }
    }

    
}
