
namespace Dal;
using DO;
using DalApi;

internal class EngineerImplementation : IEngineer
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

    public Engineer? Read(Func<Engineer, bool> filter)
    {
        if (filter == null)
        {
            return null;
        }
        return DataSource.Engineers.FirstOrDefault(filter);
    }

    //public List<Engineer> ReadAll()
    //{
    //    return new List<Engineer>(DataSource.Engineers.FindAll(i => i.Inactive is not true));
    //}

    public IEnumerable<Engineer> ReadAll(Func<Engineer, bool>? filter = null)
    {
        if (filter != null)
        {
            return from item in DataSource.Engineers
                   where filter(item) && !item.Inactive
                   select item;
        }
        return from item in DataSource.Engineers
               where !item.Inactive
               select item;
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
        DataSource.Engineers.Insert(index, engineer);
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

