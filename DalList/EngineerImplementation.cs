
namespace Dal;
using DO;
using DalApi;
using System.Threading.Tasks;

public class EngineerImplementation : IEngineer
{
    public int Create(Engineer engineer)
    {
        // what to do with id's here??
        //int Id = DataSource.Config.NextEngineerId;
        if (DataSource.Engineers.Any(engineerItem => engineerItem.Id == engineer.Id))
        {
            throw new Exception("object with that id already exists!");
        }
        Engineer engineerCopy = new Engineer(
            engineer.Id, 
            engineer.FullName, 
            engineer.EmailAddress,
            engineer?.ExperienceLevel,
            engineer?.CostPerHour ?? null
        );
        DataSource.Engineers.Add(engineerCopy);
        return engineer!.Id;
    }

    public Engineer? Read(int id)
    {
        Engineer? foundEngineer = DataSource.Engineers.FirstOrDefault(engineer => engineer.Id == id && engineer.Inactive == false);

        // If an object with the specified Id exists, return a reference to the object; otherwise, return null
        return foundEngineer;
    }

    public List<Engineer> ReadAll()
    {
        return new List<Engineer>(DataSource.Engineers);
    }

    public void Update(Engineer engineer)
    {
        int index = DataSource.Engineers.FindIndex(e => e.Id == engineer.Id && e.Inactive == false);
        if (index == -1)
        {
            throw new Exception($"object of type Engineer with identifier {engineer.Id} does not exist");
        }

        //print out object
        Console.WriteLine(engineer);
        Console.WriteLine("\nEnter updated information below:\n");

        //Collects Updated information from User - if input is blank then do not change
        Console.WriteLine("Enter Teudat Zehut of the engineer: ");
        string? input = Console.ReadLine();
        int tz = (input == "" || input is null) ? engineer.Id : Convert.ToInt32(input);

        Console.WriteLine("Enter name of the engineer: ");
        input = Console.ReadLine();
        string name = (input == "" || input is null) ? engineer.FullName : input;

        Console.WriteLine("Enter email address of the engineer: ");
        input = Console.ReadLine();
        string email = (input == "" || input is null) ? engineer.EmailAddress : input;

        Console.WriteLine("Enter experience level of the engineer (0-4): ");
        //list of enums and variables 
        Enums.EngineerExperience[] allExperiences = (Enums.EngineerExperience[])Enum.GetValues(typeof(Enums.EngineerExperience));
        Enums.EngineerExperience? experience;

        input = Console.ReadLine();

        if (input == "") experience = engineer.ExperienceLevel;
        else experience = allExperiences[Convert.ToInt32(input)];

        Console.WriteLine("Enter cost per hour of the engineer (XX.XX): ");
        input = Console.ReadLine();
        double? costPerHour = (input == "") ? engineer.CostPerHour : Convert.ToDouble(input);

        Console.WriteLine("Enter whether engineer is inactive (Y/N): ");
        input = Console.ReadLine();
        bool inactive = ((input! == "") ? engineer.Inactive : (input! == "Y"));

        Engineer updatedEng = new Engineer(tz, name, email, experience, costPerHour, inactive);

        // Remove the old engineer
        DataSource.Engineers.RemoveAt(index);

        // Add the updated engineer
        DataSource.Engineers.Insert(index, updatedEng);
    }

    public void Delete(int id)
    {
        int index = DataSource.Engineers.FindIndex(e => e.Id == id );
        if (index == -1)
        {
            throw new Exception($"object of type Engineer with identifier {id} does not exist");
        }

        Engineer inactiveEngineer = DataSource.Engineers[index] with { Inactive = true };

        DataSource.Engineers.RemoveAt(index);

        // Add the inactive engineer
        DataSource.Engineers.Add(inactiveEngineer);
    }
    public void Reset()
    {
        DataSource.Engineers.Clear();
    }
}

