class Program
{
    public static void Main(string[] args)
    {
        Person myPerson = new Person("Bob", "Roberts", 57, 198);
        Console.WriteLine(myPerson.GetPersonInformation());

        Police myPoliceMan = new Police("Gun", "Robert", "Bob", 50, 170);
        Console.WriteLine(myPoliceMan.GetPersonInformation());

        Console.WriteLine(myPoliceMan.GetpoliceManInformation());

        Doctor myDoctor = new Doctor("M.D.", "John", "Johnson", 43, 187);
        Console.WriteLine(myDoctor.GetDoctorInformation());
    }

    
}
