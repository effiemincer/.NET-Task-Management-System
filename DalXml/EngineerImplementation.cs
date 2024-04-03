using DalApi;
using DO;

namespace Dal;

internal class EngineerImplementation : IEngineer
{
    // XML file name for engineers
    readonly string s_engineers_xml = "engineers";

    // Create a new Engineer in the system
    public int Create(Engineer engineer)
    {
        // Load existing engineers from XML
        List<Engineer> Engineers = XMLTools.LoadListFromXMLSerializer<Engineer>("engineers");

        // Check if an Engineer with the same ID already exists
        if (Engineers.Any(engineerItem => engineerItem.Id == engineer.Id))
        {
            throw new DalAlreadyExistsException("Object with that id already exists!");
        }

        Engineer engineerCopy = new Engineer(
            engineer.Id,
            engineer.FullName,
            engineer.EmailAddress,
            engineer.CostPerHour,
            engineer?.ExperienceLevel
        );

        Engineers.Add(engineerCopy);

        // Save the updated list to XML
        XMLTools.SaveListToXMLSerializer<Engineer>(Engineers, "engineers");

        // Return the generated Engineer's ID
        return engineer!.Id;
    }

    // Soft delete an Engineer by marking it as inactive
    public void Delete(int id)
    {
        // Load existing engineers from XML
        List<Engineer> Engineers = XMLTools.LoadListFromXMLSerializer<Engineer>("engineers");

        // Find the index of the Engineer to delete
        int index = Engineers.FindIndex(d => d.Id == id);
        if (index == -1)
        {
            throw new DalDoesNotExistException($"Object of type Engineer with identifier {id} does not exist");
        }

        // Create a new inactive Engineer
        Engineer inactiveEngineer = Engineers[index] with { Inactive = true };

        // Remove the old Engineer
        Engineers.RemoveAt(index);

        // Add the new inactive Engineer
        Engineers.Add(inactiveEngineer);

        // Save the updated list to XML
        XMLTools.SaveListToXMLSerializer<Engineer>(Engineers, "engineers");
    }

    // Read a specific Engineer by ID
    public Engineer? Read(int id)
    {
        // Load existing engineers from XML
        List<Engineer> Engineers = XMLTools.LoadListFromXMLSerializer<Engineer>("engineers");

        // Find the Engineer with the specified ID that is not inactive
        Engineer? foundEngineer = Engineers.FirstOrDefault(engineer => engineer.Id == id && engineer.Inactive == false);

        // Return the found Engineer or null if not found
        return foundEngineer;
    }

    // Read an Engineer based on a filter predicate
    public Engineer? Read(Func<Engineer, bool> filter)
    {
        // Load existing engineers from XML
        List<Engineer> Engineers = XMLTools.LoadListFromXMLSerializer<Engineer>("engineers");

        // Check if the filter is null
        if (filter == null)
        {
            return null;
        }

        // Return the first Engineer that matches the filter
        return Engineers.FirstOrDefault(filter);
    }

    // Read all Engineers based on an optional filter
    public IEnumerable<Engineer?> ReadAll(Func<Engineer, bool>? filter = null)
    {
        // Load existing engineers from XML
        List<Engineer> Engineers = XMLTools.LoadListFromXMLSerializer<Engineer>("engineers");

        // Check if a filter is provided
        if (filter != null)
        {
            // Return filtered Engineers that are not inactive
            return from item in Engineers
                   where filter(item) && !item.Inactive
                   select item;
        }

        // Return all Engineers that are not inactive
        return from item in Engineers
               where !item.Inactive
               select item;
    }

    // Reset the list of Engineers, clearing all entries
    public void Reset()
    {
        // Load existing engineers from XML
        List<Engineer> Engineers = XMLTools.LoadListFromXMLSerializer<Engineer>("engineers");

        // Clear the list of Engineers
        Engineers.Clear();

        // Save the empty list to XML
        XMLTools.SaveListToXMLSerializer<Engineer>(Engineers, "engineers");
    }

    // Update an existing Engineer
    public void Update(Engineer engineer)
    {
        // Load existing engineers from XML
        List<Engineer> Engineers = XMLTools.LoadListFromXMLSerializer<Engineer>("engineers");

        // Find the index of the Engineer to update
        int index = Engineers.FindIndex(e => e.Id == engineer.Id && e.Inactive == false);
        if (index == -1)
        {
            throw new DalDoesNotExistException($"Object of type Engineer with identifier {engineer.Id} does not exist");
        }

        // Remove the old Engineer
        Engineers.RemoveAt(index);

        // Add the updated Engineer
        Engineers.Insert(index, engineer);

        // Save the updated list to XML
        XMLTools.SaveListToXMLSerializer<Engineer>(Engineers, "engineers");
    }

    void ICrud<Engineer>.PermanentDelete(int id)
    {
        throw new NotImplementedException();
    }
}
