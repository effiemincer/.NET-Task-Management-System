
namespace Dal;
using  DO;
using DalApi;
using System.Threading.Tasks;

public class DependencyImplementation : IDependency
{
    public int Create(Dependency dependency)
    {
        int Id = DataSource.Config.NextDependencyId;
        if (DataSource.Dependencies.Any(dependencyItem => dependencyItem.Id == Id))
        {
            throw new Exception("object with that id already exists!");
        }
        Dependency dependencyCopy = new Dependency(Id);
        DataSource.Dependencies.Add(dependencyCopy);
        return Id;
    }

    public Dependency? Read(int id)
    {
        Dependency? foundDependency = DataSource.Dependencies.FirstOrDefault(dependency => dependency.Id == id && dependency.inactive == false);

        // If an object with the specified Id exists, return a reference to the object; otherwise, return null
        return foundDependency;
    }

    public List<Dependency> ReadAll()
    {
        return new List<Dependency>(DataSource.Dependencies);
    }

    public void Update(Dependency dependency)
    {
        int index = DataSource.Dependencies.FindIndex(d => d.Id == dependency.Id && d.inactive == false);
        if (index == -1)
        {
            throw new Exception($"object of type Dependency with identifier {dependency.Id} does not exist");
        }

        // Remove the old dependency
        DataSource.Dependencies.RemoveAt(index);

        // Add the updated dependency
        DataSource.Dependencies.Add(dependency);
    }

    public void Delete(int id)
    {
        int index = DataSource.Dependencies.FindIndex(d => d.Id == id);
        if (index == -1)
        {
            throw new Exception($"object of type Dependency with identifier {id} does not exist");
        }

        Dependency inactiveDependency= DataSource.Dependencies[index] with { inactive = true };

        DataSource.Dependencies.RemoveAt(index);

        // Add the inactive engineer
        DataSource.Dependencies.Add(inactiveDependency);
    }   
    
}
