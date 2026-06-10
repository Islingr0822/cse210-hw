class Doctor : Person
{
    private string _degree;

    public Doctor(string degree, string firstName,
    string lastName, int age, int weight) : base (firstName, lastName, age, weight)
    {
        _degree = degree;
    }

    public string GetDoctorInformation()
    {
        return $"Degree: {_degree}, {GetPersonInformation()}";
    }
}