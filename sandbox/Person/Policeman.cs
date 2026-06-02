class Police : Person
{
    private string _weapons;

    public Police(string weapons, string firstName,
    string lastName, int age, int weight) : base (firstName, lastName, age, weight)
    {
        _weapons = weapons;
    }

    public string GetpoliceManInformation()
    {
        return $"Weapons: {_weapons}, {GetPersonInformation()}";
    }
}