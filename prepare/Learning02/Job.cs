using System.ComponentModel.DataAnnotations;

public class Job
{
    public string _company;
    public string _jobTitle;
    public int _startDate;
    public int _endDate;

    public void Display()
    {
        Console.WriteLine($"{_jobTitle} ({_company}) {_startDate}-{_endDate}");
    }
    
}
