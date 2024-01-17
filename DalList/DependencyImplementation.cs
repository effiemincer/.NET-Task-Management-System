
namespace Dal;
using  DO;
using DalApi;

internal class DependencyImplementation : IDependency
{
    /// <summary>
    /// Dependency Create
    /// </summary>
    /// <param name="dependency"></param>
    /// <returns></returns>
    /// <exception cref="Exception"></exception>
    public int Create(Dependency dependency)
    {
        int Id = DataSource.Config.NextDependencyId;
        if (DataSource.Dependencies.Any(dependencyItem => dependencyItem.Id == Id))
        {
            throw new Exception("object with that id already exists!");
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
        DataSource.Dependencies.Add(dependencyCopy);
        return Id;
    }


    /// <summary>
    /// Dependency Read
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    public Dependency? Read(int id)
    {
        Dependency? foundDependency = DataSource.Dependencies.FirstOrDefault(dependency => dependency.Id == id && dependency.Inactive == false);

        // If an object with the specified Id exists, return a reference to the object; otherwise, return null
        return foundDependency;
    }

    /// <summary>
    /// Dependency ReadAll
    /// </summary>
    /// <returns> the dependency list </returns>
    public List<Dependency> ReadAll()
    {
        return new List<Dependency>(DataSource.Dependencies.FindAll(i => i.Inactive is not true));
    }

    /// <summary>
    /// updates a specific task
    /// </summary>
    /// <param name="dependency"></param>
    /// <exception cref="Exception"></exception>
    public void Update(Dependency dependency)
    {
        int index = DataSource.Dependencies.FindIndex(d => d.Id == dependency.Id && d.Inactive == false);
        if (index == -1)
        {
            throw new Exception($"object of type Dependency with identifier {dependency.Id} does not exist");
        }

        // Remove the old dependency
        DataSource.Dependencies.RemoveAt(index);

        // Add the updated dependency
        DataSource.Dependencies.Insert(index, dependency);
    }

    /// <summary>
    /// deletes a task
    /// </summary>
    /// <param name="id"></param>
    /// <exception cref="Exception"></exception>
    public void Delete(int id)
    {
        int index = DataSource.Dependencies.FindIndex(d => d.Id == id);
        if (index == -1)
        {
            throw new Exception($"object of type Dependency with identifier {id} does not exist");
        }

        Dependency inactiveDependency= DataSource.Dependencies[index] with { Inactive = true };

        DataSource.Dependencies.RemoveAt(index);

        // Add the Inactive dependency
        DataSource.Dependencies.Add(inactiveDependency);
    }   


    /// <summary>
    /// clears all dependencies in the dependencies list
    /// </summary>
    public void Reset()
    {
        DataSource.Dependencies.Clear();
    }
}

