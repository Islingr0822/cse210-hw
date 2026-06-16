class Program
{

    public static void DisplayPersonInformation(Person person)
    {
        if (person is Doctor doctor)
            Console.WriteLine(doctor.GetDoctorInformation());
        else if (person is Police police)
            Console.WriteLine(police.GetpoliceManInformation());
        else
            Console.WriteLine(person.GetPersonInformation());
    }
    public static void Main(string[] args)
    {
        Person myPerson = new Person("Bob", "Roberts", 57, 198);
        // Console.WriteLine(myPerson.GetPersonInformation());

        Police myPoliceMan = new Police("Gun", "Robert", "Bob", 50, 170);
        // Console.WriteLine(myPoliceMan.GetPersonInformation());

        // Console.WriteLine(myPoliceMan.GetpoliceManInformation());

        Doctor myDoctor = new Doctor("M.D.", "John", "Johnson", 43, 187);
        // Console.WriteLine(myDoctor.GetDoctorInformation());

        
        myDoctor.ChangeWeight(-20);
        // Console.WriteLine(myDoctor.GetDoctorInformation());

        List<Person> myPeople = new List<Person>();
        myPeople.Add(myPerson);
        myPeople.Add(myDoctor);
        myPeople.Add(myPoliceMan);

        foreach (Person person in myPeople)
        {
            DisplayPersonInformation(person);
        }
            
    }

    

    
}
