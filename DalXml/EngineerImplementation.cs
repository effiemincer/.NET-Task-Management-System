using DalApi;
using DO;
using System.Data.Common;
using System.Linq;

namespace Dal;

internal class EngineerImplementation : IEngineer
{
    readonly string s_engineers_xml = "engineers";

    public int Create(Engineer engineer)
    {
        List<Engineer> Engineers = XMLTools.LoadListFromXMLSerializer<Engineer>("engineers");

        if (Engineers.Any(engineerItem => engineerItem.Id == engineer.Id))
        {
            throw new DalAlreadyExistsException("object with that id already exists!");
        }
        Engineer engineerCopy = new Engineer(
            engineer.Id,
            engineer.FullName,
            engineer.EmailAddress,
            engineer?.ExperienceLevel,
            engineer?.CostPerHour ?? null
            );
        Engineers.Add(engineerCopy);
        XMLTools.SaveListToXMLSerializer<Engineer>(Engineers, "engineers");

        return engineer!.Id;
    }


    public void Delete(int id)
    {
        List<Engineer> Engineers = XMLTools.LoadListFromXMLSerializer<Engineer>("engineers");

        int index = Engineers.FindIndex(d => d.Id == id);
        if (index == -1)
        {
            throw new DalDoesNotExistException($"object of type Dependency with identifier {id} does not exist");
        }

        Engineer inactiveEngineer = Engineers[index] with { Inactive = true };

        Engineers.RemoveAt(index);

        // Add the Inactive dependency
        Engineers.Add(inactiveEngineer);

        XMLTools.SaveListToXMLSerializer<Engineer>(Engineers, "engineer");

    }
    public Engineer? Read(int id) 
    {
        List<Engineer> Engineers = XMLTools.LoadListFromXMLSerializer<Engineer>("engineers");
        Engineer? foundEngineer = Engineers.FirstOrDefault(engineer => engineer.Id == id && engineer.Inactive == false);

        // If an object with the specified Id exists, return a reference to the object; otherwise, return null
        return foundEngineer;

    }

    public Engineer? Read(Func<Engineer, bool> filter)
    {
        List<Engineer> Engineers = XMLTools.LoadListFromXMLSerializer<Engineer>("engineers");
        if (filter == null)
        {
            return null;
        }
        return Engineers.FirstOrDefault(filter);
    }

    public IEnumerable<Engineer?> ReadAll(Func<Engineer, bool>? filter = null)
    {
        List<Engineer> Engineers = XMLTools.LoadListFromXMLSerializer<Engineer>("engineers");
        if (filter != null)
        {
            return from item in Engineers
                   where filter(item) && !item.Inactive
                   select item;
        }
        return from item in Engineers
               where !item.Inactive
               select item;
    }

    public void Reset()
    {
        List<Engineer> Engineers = XMLTools.LoadListFromXMLSerializer<Engineer>("engineers");
        Engineers.Clear();
        XMLTools.SaveListToXMLSerializer<Engineer>(Engineers, "engineers");
    }

    public void Update(Engineer engineer)
    {
        List<Engineer> Engineers = XMLTools.LoadListFromXMLSerializer<Engineer>("engineers");
        int index = Engineers.FindIndex(e => e.Id == engineer.Id && e.Inactive == false);
        if (index == -1)
        {
            throw new DalDoesNotExistException($"object of type Dependency with identifier {engineer.Id} does not exist");
        }

        // Remove the old dependency
        Engineers.RemoveAt(index);

        // Add the updated dependency
        Engineers.Insert(index, engineer);

        XMLTools.SaveListToXMLSerializer<Engineer>(Engineers, "engineers");
    }
}
