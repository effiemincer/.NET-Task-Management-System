using DalApi;
using DO;

namespace Dal;

internal class DependencyImplementation : IDependency
{
    readonly string s_dependencies_xml = "dependencies";

    public int Create(Dependency dependency)
    {
        List<Dependency> Dependencies = XMLTools.LoadListFromXMLSerializer<Dependency>("dependencies");
        
        int Id = Config.NextDependencyId;

        if (Dependencies.Any(dependencyItem => dependencyItem.Id == Id))
        {
            throw new DalAlreadyExistsException("object with that id already exists!");
        }
        Dependency dependencyCopy = new Dependency(
            Id,
            dependency?.DependentTaskId ?? 0,
            dependency?.RequisiteID ?? 0,
            dependency?.CustomerEmail ?? "",
            dependency?.ShippingAddress ?? "",
            dependency?.OrderCreationDate ?? DateTime.Now,
            dependency?.ShippingDate,
            dependency?.DeliveryDate
            );
        Dependencies.Add(dependencyCopy);
        XMLTools.SaveListToXMLSerializer<Dependency>(Dependencies, "dependencies");

        return Id;
    }

    public void Delete(int id)
    {
        List<Dependency> Dependencies = XMLTools.LoadListFromXMLSerializer<Dependency>("dependencies");
       
        int index = Dependencies.FindIndex(d => d.Id == id);
        if (index == -1)
        {
            throw new DalDoesNotExistException($"object of type Dependency with identifier {id} does not exist");
        }

        Dependency inactiveDependency = Dependencies[index] with { Inactive = true };

        Dependencies.RemoveAt(index);

        // Add the Inactive dependency
        Dependencies.Add(inactiveDependency);
        
        XMLTools.SaveListToXMLSerializer<Dependency>(Dependencies, "dependencies");
    }

    public Dependency? Read(int id)
    {
        List<Dependency> Dependencies = XMLTools.LoadListFromXMLSerializer<Dependency>("dependencies");
        Dependency? foundDependency = Dependencies.FirstOrDefault(dependency => dependency.Id == id && dependency.Inactive == false);

        // If an object with the specified Id exists, return a reference to the object; otherwise, return null
        return foundDependency;
    }
    public Dependency? Read(Func<Dependency, bool> filter)
    {
        List<Dependency> Dependencies = XMLTools.LoadListFromXMLSerializer<Dependency>("dependencies");
        if (filter == null)
        {
            return null;
        }
        return Dependencies.FirstOrDefault(filter);
    }

    public IEnumerable<Dependency> ReadAll(Func<Dependency, bool>? filter = null)
    {
        List<Dependency> Dependencies = XMLTools.LoadListFromXMLSerializer<Dependency>("dependencies");
        if (filter != null)
        {
            return from item in Dependencies
                   where filter(item) && !item.Inactive
                   select item;
        }
        return from item in Dependencies
               where !item.Inactive
               select item;
    }

    public void Reset()
    {
        List<Dependency> Dependencies = XMLTools.LoadListFromXMLSerializer<Dependency>("dependencies");
        Dependencies.Clear();
        XMLTools.SaveListToXMLSerializer<Dependency>(Dependencies, "dependencies");
    }

    public void Update(Dependency dependency)
    {
        List<Dependency> Dependencies = XMLTools.LoadListFromXMLSerializer<Dependency>("dependencies");
        int index = Dependencies.FindIndex(d => d.Id == dependency.Id && d.Inactive == false);
        if (index == -1)
        {
            throw new DalDoesNotExistException($"object of type Dependency with identifier {dependency.Id} does not exist");
        }

        // Remove the old dependency
        Dependencies.RemoveAt(index);

        // Add the updated dependency
        Dependencies.Insert(index, dependency);

        XMLTools.SaveListToXMLSerializer<Dependency>(Dependencies, "dependencies");
    }
}
