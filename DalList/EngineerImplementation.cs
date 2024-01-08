
namespace Dal;
using DO;
using DalApi;

public class EngineerImplementation : IEngineer
{
    public int Create(Engineer engineer)
    {
        // what to do with id's here??
        int Id = DataSource.Config.NextEngineerId;
        if (DataSource.Engineers.Any(engineerItem => engineerItem.Id == Id))
        {
            throw new Exception("object with that id already exists!");
        }
        Engineer engineerCopy = new Engineer(
            Id, 
            engineer.FullName, 
            engineer.EmailAddress,
            engineer?.ExperienceLevel,
            engineer?.CostPerHour ?? 0
        );
        DataSource.Engineers.Add(engineerCopy);
        return Id;
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

        // Remove the old engineer
        DataSource.Engineers.RemoveAt(index);

        // Add the updated engineer
        DataSource.Engineers.Add(engineer);
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

}
