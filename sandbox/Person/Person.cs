class Person
{
    private string _firstName;
    private string _lastName;
    private int _age;
    private int _weight;

    public Person(string firstName, string lastName, int age, int weight)
    {
        _firstName = firstName;
        _lastName = lastName;
        _age = age;
        _weight = weight;

    }

    public string GetPersonInformation()
    {
        return $"Name: {_firstName} {_lastName}, age: {_age}, weight: {_weight} lbs";
    }

    public void ChangeWeight(int update)
    {
        _weight += update;
    }

    public void SetAge(int age)
    {
        _age = age;
        if (age < 0 || age > 115)
        {
            _age = 0;
            Console.WriteLine("Incorrect age");
        }
    }
}

